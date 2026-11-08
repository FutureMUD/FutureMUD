#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0052FoldingPot() => new()
	{
		Metadata = new("Kitchen and appliances/make folding mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0052_folding_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0052_folding_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0052_folding_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making folding mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2616.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2616 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0052_folding_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0052_folding_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0053FoldingGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make folding cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0053_folding_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0053_folding_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0053_folding_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making folding cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2656.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2656 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0053_folding_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0053_folding_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0054FoldingWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make folding copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0054_folding_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0054_folding_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0054_folding_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding copper whisk for domestic cookery from prepared copper stock.",
		Action = "making folding copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2697.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2697 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0054_folding_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper whisk for domestic cookery (@industrialised_kitchen_appliances_0054_folding_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0055FoldingLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make folding ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0055_folding_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0055_folding_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0055_folding_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making folding ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2738.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2738 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0055_folding_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0055_folding_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0056FoldingStove() => new()
	{
		Metadata = new("Kitchen and appliances/make folding mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0056_folding_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0056_folding_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0056_folding_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making folding mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2779.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2779 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0056_folding_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0056_folding_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0057FoldingToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make folding cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0057_folding_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0057_folding_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0057_folding_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making folding cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2819.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2819 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0057_folding_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0057_folding_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingCopperMixerForDomesticCookeryCraftIndustrialisedKitchenAppliances0058FoldingMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make folding copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0058_folding_mixer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0058_folding_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0058_folding_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding copper mixer for domestic cookery from prepared copper stock.",
		Action = "making folding copper mixer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding copper mixer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2860.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2860 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0058_folding_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper mixer for domestic cookery (@industrialised_kitchen_appliances_0058_folding_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingCeramicPressForDomesticCookeryCraftIndustrialisedKitchenAppliances0059FoldingPress() => new()
	{
		Metadata = new("Kitchen and appliances/make folding ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0059_folding_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0059_folding_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0059_folding_press]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding ceramic press for domestic cookery from prepared ceramic stock.",
		Action = "making folding ceramic press for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding ceramic press for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2901.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2901 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0059_folding_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic press for domestic cookery (@industrialised_kitchen_appliances_0059_folding_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingMildSteelStrainerForDomesticCookeryCraftIndustrialisedKitchenAppliances0060FoldingStrainer() => new()
	{
		Metadata = new("Kitchen and appliances/make folding mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0060_folding_strainer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0060_folding_strainer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0060_folding_strainer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding mild steel strainer for domestic cookery from prepared mild steel stock.",
		Action = "making folding mild steel strainer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding mild steel strainer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2941.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2941 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0060_folding_strainer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel strainer for domestic cookery (@industrialised_kitchen_appliances_0060_folding_strainer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0061PortableScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make portable cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0061_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0061_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0061_portable_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making portable cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2982.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2982 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0061_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0061_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0062PortableKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make portable copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0062_portable_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0062_portable_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0062_portable_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable copper kettle for domestic cookery from prepared copper stock.",
		Action = "making portable copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3023.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3023 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0062_portable_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper kettle for domestic cookery (@industrialised_kitchen_appliances_0062_portable_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0063PortablePan() => new()
	{
		Metadata = new("Kitchen and appliances/make portable ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0063_portable_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0063_portable_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0063_portable_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making portable ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3064.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3064 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0063_portable_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0063_portable_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0064PortablePot() => new()
	{
		Metadata = new("Kitchen and appliances/make portable mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0064_portable_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0064_portable_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0064_portable_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making portable mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3104.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3104 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0064_portable_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0064_portable_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0065PortableGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make portable cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0065_portable_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0065_portable_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0065_portable_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making portable cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3145.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3145 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0065_portable_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0065_portable_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0066PortableWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make portable copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0066_portable_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0066_portable_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0066_portable_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable copper whisk for domestic cookery from prepared copper stock.",
		Action = "making portable copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3186.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3186 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0066_portable_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper whisk for domestic cookery (@industrialised_kitchen_appliances_0066_portable_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0067PortableLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make portable ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0067_portable_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0067_portable_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0067_portable_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making portable ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3226.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3226 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0067_portable_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0067_portable_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0068PortableStove() => new()
	{
		Metadata = new("Kitchen and appliances/make portable mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0068_portable_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0068_portable_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0068_portable_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making portable mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3267.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3267 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0068_portable_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0068_portable_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0069PortableToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make portable cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0069_portable_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0069_portable_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0069_portable_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making portable cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3308.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3308 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0069_portable_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0069_portable_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableCopperMixerForDomesticCookeryCraftIndustrialisedKitchenAppliances0070PortableMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make portable copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0070_portable_mixer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0070_portable_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0070_portable_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable copper mixer for domestic cookery from prepared copper stock.",
		Action = "making portable copper mixer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable copper mixer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3348.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3348 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0070_portable_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper mixer for domestic cookery (@industrialised_kitchen_appliances_0070_portable_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableCeramicPressForDomesticCookeryCraftIndustrialisedKitchenAppliances0071PortablePress() => new()
	{
		Metadata = new("Kitchen and appliances/make portable ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0071_portable_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0071_portable_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0071_portable_press]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable ceramic press for domestic cookery from prepared ceramic stock.",
		Action = "making portable ceramic press for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable ceramic press for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3389.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3389 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0071_portable_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic press for domestic cookery (@industrialised_kitchen_appliances_0071_portable_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePortableMildSteelStrainerForDomesticCookeryCraftIndustrialisedKitchenAppliances0072PortableStrainer() => new()
	{
		Metadata = new("Kitchen and appliances/make portable mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0072_portable_strainer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0072_portable_strainer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0072_portable_strainer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a portable mild steel strainer for domestic cookery from prepared mild steel stock.",
		Action = "making portable mild steel strainer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress portable mild steel strainer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3430.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3430 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0072_portable_strainer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel strainer for domestic cookery (@industrialised_kitchen_appliances_0072_portable_strainer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0073WorkshopScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0073_workshop_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0073_workshop_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0073_workshop_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making workshop cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3471.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3471 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0073_workshop_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0073_workshop_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0074WorkshopKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0074_workshop_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0074_workshop_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0074_workshop_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop copper kettle for domestic cookery from prepared copper stock.",
		Action = "making workshop copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3511.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3511 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0074_workshop_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop copper kettle for domestic cookery (@industrialised_kitchen_appliances_0074_workshop_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0075WorkshopPan() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0075_workshop_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0075_workshop_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0075_workshop_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making workshop ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3552.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3552 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0075_workshop_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0075_workshop_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0076WorkshopPot() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0076_workshop_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0076_workshop_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0076_workshop_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making workshop mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3593.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3593 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0076_workshop_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0076_workshop_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0077WorkshopGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0077_workshop_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0077_workshop_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0077_workshop_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making workshop cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3633.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3633 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0077_workshop_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0077_workshop_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0078WorkshopWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0078_workshop_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0078_workshop_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0078_workshop_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop copper whisk for domestic cookery from prepared copper stock.",
		Action = "making workshop copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3674.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3674 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0078_workshop_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop copper whisk for domestic cookery (@industrialised_kitchen_appliances_0078_workshop_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0079WorkshopLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0079_workshop_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0079_workshop_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0079_workshop_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making workshop ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3715.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3715 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0079_workshop_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0079_workshop_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0080WorkshopStove() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0080_workshop_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0080_workshop_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0080_workshop_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making workshop mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3755.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3755 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0080_workshop_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0080_workshop_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0081WorkshopToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0081_workshop_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0081_workshop_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0081_workshop_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making workshop cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3796.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3796 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0081_workshop_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0081_workshop_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopCopperMixerForDomesticCookeryCraftIndustrialisedKitchenAppliances0082WorkshopMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0082_workshop_mixer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0082_workshop_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0082_workshop_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop copper mixer for domestic cookery from prepared copper stock.",
		Action = "making workshop copper mixer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop copper mixer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3837.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3837 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0082_workshop_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop copper mixer for domestic cookery (@industrialised_kitchen_appliances_0082_workshop_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopCeramicPressForDomesticCookeryCraftIndustrialisedKitchenAppliances0083WorkshopPress() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0083_workshop_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0083_workshop_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0083_workshop_press]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop ceramic press for domestic cookery from prepared ceramic stock.",
		Action = "making workshop ceramic press for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic press for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3878.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3878 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0083_workshop_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic press for domestic cookery (@industrialised_kitchen_appliances_0083_workshop_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWorkshopMildSteelStrainerForDomesticCookeryCraftIndustrialisedKitchenAppliances0084WorkshopStrainer() => new()
	{
		Metadata = new("Kitchen and appliances/make workshop mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0084_workshop_strainer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0084_workshop_strainer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0084_workshop_strainer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a workshop mild steel strainer for domestic cookery from prepared mild steel stock.",
		Action = "making workshop mild steel strainer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel strainer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3918.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3918 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0084_workshop_strainer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel strainer for domestic cookery (@industrialised_kitchen_appliances_0084_workshop_strainer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0085InstitutionalScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0085_institutional_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0085_institutional_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0085_institutional_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making institutional cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3959.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3959 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0085_institutional_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0085_institutional_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0086InstitutionalKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0086_institutional_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0086_institutional_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0086_institutional_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional copper kettle for domestic cookery from prepared copper stock.",
		Action = "making institutional copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4000.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4000 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0086_institutional_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper kettle for domestic cookery (@industrialised_kitchen_appliances_0086_institutional_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0087InstitutionalPan() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0087_institutional_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0087_institutional_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0087_institutional_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making institutional ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4040.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4040 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0087_institutional_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0087_institutional_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0088InstitutionalPot() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0088_institutional_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0088_institutional_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0088_institutional_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making institutional mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4081.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4081 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0088_institutional_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0088_institutional_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0089InstitutionalGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0089_institutional_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0089_institutional_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0089_institutional_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making institutional cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4122.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4122 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0089_institutional_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0089_institutional_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0090InstitutionalWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0090_institutional_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0090_institutional_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0090_institutional_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional copper whisk for domestic cookery from prepared copper stock.",
		Action = "making institutional copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4162.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4162 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0090_institutional_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper whisk for domestic cookery (@industrialised_kitchen_appliances_0090_institutional_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0091InstitutionalLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0091_institutional_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0091_institutional_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0091_institutional_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making institutional ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4203.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4203 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0091_institutional_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0091_institutional_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0092InstitutionalStove() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0092_institutional_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0092_institutional_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0092_institutional_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making institutional mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4244.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4244 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0092_institutional_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0092_institutional_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0093InstitutionalToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0093_institutional_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0093_institutional_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0093_institutional_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making institutional cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4284.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4284 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0093_institutional_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0093_institutional_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCopperMixerForDomesticCookeryCraftIndustrialisedKitchenAppliances0094InstitutionalMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0094_institutional_mixer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0094_institutional_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0094_institutional_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional copper mixer for domestic cookery from prepared copper stock.",
		Action = "making institutional copper mixer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional copper mixer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4325.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4325 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0094_institutional_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper mixer for domestic cookery (@industrialised_kitchen_appliances_0094_institutional_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCeramicPressForDomesticCookeryCraftIndustrialisedKitchenAppliances0095InstitutionalPress() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0095_institutional_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0095_institutional_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0095_institutional_press]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional ceramic press for domestic cookery from prepared ceramic stock.",
		Action = "making institutional ceramic press for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic press for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4366.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4366 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0095_institutional_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic press for domestic cookery (@industrialised_kitchen_appliances_0095_institutional_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalMildSteelStrainerForDomesticCookeryCraftIndustrialisedKitchenAppliances0096InstitutionalStrainer() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0096_institutional_strainer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0096_institutional_strainer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0096_institutional_strainer]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional mild steel strainer for domestic cookery from prepared mild steel stock.",
		Action = "making institutional mild steel strainer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel strainer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4407.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4407 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0096_institutional_strainer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel strainer for domestic cookery (@industrialised_kitchen_appliances_0096_institutional_strainer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0097WeatherproofScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0097_weatherproof_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0097_weatherproof_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0097_weatherproof_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making weatherproof cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4447.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4447 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0097_weatherproof_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0097_weatherproof_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0098WeatherproofKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0098_weatherproof_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0098_weatherproof_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0098_weatherproof_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof copper kettle for domestic cookery from prepared copper stock.",
		Action = "making weatherproof copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4488.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4488 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0098_weatherproof_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof copper kettle for domestic cookery (@industrialised_kitchen_appliances_0098_weatherproof_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0099WeatherproofPan() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0099_weatherproof_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0099_weatherproof_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0099_weatherproof_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making weatherproof ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4529.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4529 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0099_weatherproof_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0099_weatherproof_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0100WeatherproofPot() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0100_weatherproof_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0100_weatherproof_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0100_weatherproof_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making weatherproof mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4569.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4569 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0100_weatherproof_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0100_weatherproof_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0101WeatherproofGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0101_weatherproof_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0101_weatherproof_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0101_weatherproof_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making weatherproof cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4610.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4610 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0101_weatherproof_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0101_weatherproof_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0102WeatherproofWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0102_weatherproof_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0102_weatherproof_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0102_weatherproof_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof copper whisk for domestic cookery from prepared copper stock.",
		Action = "making weatherproof copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4651.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4651 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0102_weatherproof_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof copper whisk for domestic cookery (@industrialised_kitchen_appliances_0102_weatherproof_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0103WeatherproofLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0103_weatherproof_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0103_weatherproof_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0103_weatherproof_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making weatherproof ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4692.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4692 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0103_weatherproof_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0103_weatherproof_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0104WeatherproofStove() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0104_weatherproof_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0104_weatherproof_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0104_weatherproof_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making weatherproof mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4732.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4732 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0104_weatherproof_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0104_weatherproof_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0105WeatherproofToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0105_weatherproof_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0105_weatherproof_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0105_weatherproof_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making weatherproof cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4773.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4773 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0105_weatherproof_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0105_weatherproof_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofCopperMixerForDomesticCookeryCraftIndustrialisedKitchenAppliances0106WeatherproofMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0106_weatherproof_mixer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0106_weatherproof_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0106_weatherproof_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof copper mixer for domestic cookery from prepared copper stock.",
		Action = "making weatherproof copper mixer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof copper mixer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4814.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4814 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0106_weatherproof_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof copper mixer for domestic cookery (@industrialised_kitchen_appliances_0106_weatherproof_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofCeramicPressForDomesticCookeryCraftIndustrialisedKitchenAppliances0107WeatherproofPress() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0107_weatherproof_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0107_weatherproof_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0107_weatherproof_press]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof ceramic press for domestic cookery from prepared ceramic stock.",
		Action = "making weatherproof ceramic press for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof ceramic press for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4854.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4854 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0107_weatherproof_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof ceramic press for domestic cookery (@industrialised_kitchen_appliances_0107_weatherproof_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeWeatherproofMildSteelStrainerForDomesticCookeryCraftIndustrialisedKitchenAppliances0108WeatherproofStrainer() => new()
	{
		Metadata = new("Kitchen and appliances/make weatherproof mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0108_weatherproof_strainer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0108_weatherproof_strainer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0108_weatherproof_strainer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a weatherproof mild steel strainer for domestic cookery from prepared mild steel stock.",
		Action = "making weatherproof mild steel strainer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel strainer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4895.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4895 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0108_weatherproof_strainer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel strainer for domestic cookery (@industrialised_kitchen_appliances_0108_weatherproof_strainer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeServiceCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0109ServiceScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make service cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0109_service_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0109_service_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0109_service_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make a service cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making service cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress service cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4936.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4936 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0109_service_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0109_service_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeServiceCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0110ServiceKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make service copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0110_service_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0110_service_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0110_service_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a service copper kettle for domestic cookery from prepared copper stock.",
		Action = "making service copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress service copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4976.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4976 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0110_service_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service copper kettle for domestic cookery (@industrialised_kitchen_appliances_0110_service_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeServiceCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0111ServicePan() => new()
	{
		Metadata = new("Kitchen and appliances/make service ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0111_service_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0111_service_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0111_service_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make a service ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making service ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress service ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5017.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5017 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0111_service_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0111_service_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
