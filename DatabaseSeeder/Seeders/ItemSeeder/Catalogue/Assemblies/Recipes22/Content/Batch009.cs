#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingPaperBasketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0048FoldingBasket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding paper basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0048_folding_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0048_folding_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0048_folding_basket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding paper basket for domestic goods handling from prepared paper stock.",
		Action = "making folding paper basket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding paper basket for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4008.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4008 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0048_folding_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper basket for domestic goods handling (@industrialised_logistics_retail_hospitality_0048_folding_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingCottonCaseForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0049FoldingCase() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding cotton case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0049_folding_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0049_folding_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0049_folding_case]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding cotton case for domestic goods handling from prepared cotton stock.",
		Action = "making folding cotton case for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding cotton case for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4049.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4049 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0049_folding_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton case for domestic goods handling (@industrialised_logistics_retail_hospitality_0049_folding_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingOakSignForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0050FoldingSign() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding oak sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0050_folding_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0050_folding_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0050_folding_sign]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding oak sign for domestic goods handling from prepared oak stock.",
		Action = "making folding oak sign for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding oak sign for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4090.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4090 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0050_folding_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak sign for domestic goods handling (@industrialised_logistics_retail_hospitality_0050_folding_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortableMildSteelWasteForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0051PortableWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable mild steel waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0051_portable_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0051_portable_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0051_portable_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable mild steel waste for domestic goods handling from prepared mild steel stock.",
		Action = "making portable mild steel waste for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable mild steel waste for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4130.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4130 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0051_portable_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel waste for domestic goods handling (@industrialised_logistics_retail_hospitality_0051_portable_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortablePaperCrateForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0052PortableCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable paper crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0052_portable_crate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0052_portable_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0052_portable_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable paper crate for domestic goods handling from prepared paper stock.",
		Action = "making portable paper crate for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable paper crate for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4171.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4171 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0052_portable_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper crate for domestic goods handling (@industrialised_logistics_retail_hospitality_0052_portable_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortableCottonTrolleyForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0053PortableTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable cotton trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0053_portable_trolley]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0053_portable_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0053_portable_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable cotton trolley for domestic goods handling from prepared cotton stock.",
		Action = "making portable cotton trolley for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable cotton trolley for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4212.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4212 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0053_portable_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton trolley for domestic goods handling (@industrialised_logistics_retail_hospitality_0053_portable_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortableOakTrayForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0054PortableTray() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable oak tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0054_portable_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0054_portable_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0054_portable_tray]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable oak tray for domestic goods handling from prepared oak stock.",
		Action = "making portable oak tray for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable oak tray for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4253.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4253 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0054_portable_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak tray for domestic goods handling (@industrialised_logistics_retail_hospitality_0054_portable_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortableMildSteelCounterForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0055PortableCounter() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable mild steel counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0055_portable_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0055_portable_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0055_portable_counter]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable mild steel counter for domestic goods handling from prepared mild steel stock.",
		Action = "making portable mild steel counter for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable mild steel counter for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4293.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4293 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0055_portable_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel counter for domestic goods handling (@industrialised_logistics_retail_hospitality_0055_portable_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortablePaperScaleForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0056PortableScale() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable paper scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0056_portable_scale]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0056_portable_scale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0056_portable_scale]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable paper scale for domestic goods handling from prepared paper stock.",
		Action = "making portable paper scale for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable paper scale for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4334.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4334 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0056_portable_scale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper scale for domestic goods handling (@industrialised_logistics_retail_hospitality_0056_portable_scale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortableCottonTicketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0057PortableTicket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable cotton ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0057_portable_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0057_portable_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0057_portable_ticket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable cotton ticket for domestic goods handling from prepared cotton stock.",
		Action = "making portable cotton ticket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable cotton ticket for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4375.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4375 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0057_portable_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton ticket for domestic goods handling (@industrialised_logistics_retail_hospitality_0057_portable_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortableOakBasketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0058PortableBasket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable oak basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0058_portable_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0058_portable_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0058_portable_basket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable oak basket for domestic goods handling from prepared oak stock.",
		Action = "making portable oak basket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable oak basket for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4415.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4415 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0058_portable_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak basket for domestic goods handling (@industrialised_logistics_retail_hospitality_0058_portable_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortableMildSteelCaseForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0059PortableCase() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable mild steel case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0059_portable_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0059_portable_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0059_portable_case]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable mild steel case for domestic goods handling from prepared mild steel stock.",
		Action = "making portable mild steel case for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable mild steel case for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4456.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4456 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0059_portable_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel case for domestic goods handling (@industrialised_logistics_retail_hospitality_0059_portable_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePortablePaperSignForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0060PortableSign() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make portable paper sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0060_portable_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0060_portable_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0060_portable_sign]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a portable paper sign for domestic goods handling from prepared paper stock.",
		Action = "making portable paper sign for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress portable paper sign for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4497.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4497 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0060_portable_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper sign for domestic goods handling (@industrialised_logistics_retail_hospitality_0060_portable_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopCottonWasteForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0061WorkshopWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop cotton waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0061_workshop_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0061_workshop_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0061_workshop_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop cotton waste for domestic goods handling from prepared cotton stock.",
		Action = "making workshop cotton waste for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop cotton waste for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4538.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4538 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0061_workshop_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton waste for domestic goods handling (@industrialised_logistics_retail_hospitality_0061_workshop_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopOakCrateForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0062WorkshopCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop oak crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0062_workshop_crate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0062_workshop_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0062_workshop_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop oak crate for domestic goods handling from prepared oak stock.",
		Action = "making workshop oak crate for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop oak crate for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4578.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4578 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0062_workshop_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak crate for domestic goods handling (@industrialised_logistics_retail_hospitality_0062_workshop_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopMildSteelTrolleyForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0063WorkshopTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop mild steel trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0063_workshop_trolley]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0063_workshop_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0063_workshop_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop mild steel trolley for domestic goods handling from prepared mild steel stock.",
		Action = "making workshop mild steel trolley for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel trolley for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4619.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4619 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0063_workshop_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel trolley for domestic goods handling (@industrialised_logistics_retail_hospitality_0063_workshop_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopPaperTrayForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0064WorkshopTray() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop paper tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0064_workshop_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0064_workshop_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop paper tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0064_workshop_tray]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop paper tray for domestic goods handling from prepared paper stock.",
		Action = "making workshop paper tray for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop paper tray for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4660.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4660 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0064_workshop_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop paper tray for domestic goods handling (@industrialised_logistics_retail_hospitality_0064_workshop_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopCottonCounterForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0065WorkshopCounter() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop cotton counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0065_workshop_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0065_workshop_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0065_workshop_counter]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop cotton counter for domestic goods handling from prepared cotton stock.",
		Action = "making workshop cotton counter for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop cotton counter for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4700.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4700 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0065_workshop_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton counter for domestic goods handling (@industrialised_logistics_retail_hospitality_0065_workshop_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopOakScaleForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0066WorkshopScale() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop oak scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0066_workshop_scale]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0066_workshop_scale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0066_workshop_scale]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop oak scale for domestic goods handling from prepared oak stock.",
		Action = "making workshop oak scale for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop oak scale for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4741.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4741 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0066_workshop_scale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak scale for domestic goods handling (@industrialised_logistics_retail_hospitality_0066_workshop_scale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopMildSteelTicketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0067WorkshopTicket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop mild steel ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0067_workshop_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0067_workshop_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0067_workshop_ticket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop mild steel ticket for domestic goods handling from prepared mild steel stock.",
		Action = "making workshop mild steel ticket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel ticket for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4782.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4782 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0067_workshop_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel ticket for domestic goods handling (@industrialised_logistics_retail_hospitality_0067_workshop_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopPaperBasketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0068WorkshopBasket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop paper basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0068_workshop_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0068_workshop_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop paper basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0068_workshop_basket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop paper basket for domestic goods handling from prepared paper stock.",
		Action = "making workshop paper basket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop paper basket for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4822.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4822 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0068_workshop_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop paper basket for domestic goods handling (@industrialised_logistics_retail_hospitality_0068_workshop_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopCottonCaseForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0069WorkshopCase() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop cotton case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0069_workshop_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0069_workshop_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0069_workshop_case]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop cotton case for domestic goods handling from prepared cotton stock.",
		Action = "making workshop cotton case for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop cotton case for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4863.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4863 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0069_workshop_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton case for domestic goods handling (@industrialised_logistics_retail_hospitality_0069_workshop_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeWorkshopOakSignForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0070WorkshopSign() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make workshop oak sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0070_workshop_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0070_workshop_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0070_workshop_sign]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a workshop oak sign for domestic goods handling from prepared oak stock.",
		Action = "making workshop oak sign for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress workshop oak sign for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4904.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4904 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0070_workshop_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak sign for domestic goods handling (@industrialised_logistics_retail_hospitality_0070_workshop_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalMildSteelWasteForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0071InstitutionalWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional mild steel waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0071_institutional_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0071_institutional_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0071_institutional_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional mild steel waste for domestic goods handling from prepared mild steel stock.",
		Action = "making institutional mild steel waste for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel waste for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4944.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4944 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0071_institutional_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel waste for domestic goods handling (@industrialised_logistics_retail_hospitality_0071_institutional_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalPaperCrateForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0072InstitutionalCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional paper crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0072_institutional_crate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0072_institutional_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional paper crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0072_institutional_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional paper crate for domestic goods handling from prepared paper stock.",
		Action = "making institutional paper crate for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional paper crate for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4985.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4985 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0072_institutional_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional paper crate for domestic goods handling (@industrialised_logistics_retail_hospitality_0072_institutional_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalCottonTrolleyForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0073InstitutionalTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional cotton trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0073_institutional_trolley]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0073_institutional_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0073_institutional_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional cotton trolley for domestic goods handling from prepared cotton stock.",
		Action = "making institutional cotton trolley for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional cotton trolley for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5026.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5026 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0073_institutional_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton trolley for domestic goods handling (@industrialised_logistics_retail_hospitality_0073_institutional_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalOakTrayForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0074InstitutionalTray() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional oak tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0074_institutional_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0074_institutional_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0074_institutional_tray]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional oak tray for domestic goods handling from prepared oak stock.",
		Action = "making institutional oak tray for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional oak tray for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5067.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5067 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0074_institutional_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak tray for domestic goods handling (@industrialised_logistics_retail_hospitality_0074_institutional_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalMildSteelCounterForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0075InstitutionalCounter() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional mild steel counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0075_institutional_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0075_institutional_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0075_institutional_counter]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional mild steel counter for domestic goods handling from prepared mild steel stock.",
		Action = "making institutional mild steel counter for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel counter for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5107.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5107 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0075_institutional_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel counter for domestic goods handling (@industrialised_logistics_retail_hospitality_0075_institutional_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalPaperScaleForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0076InstitutionalScale() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional paper scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0076_institutional_scale]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0076_institutional_scale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional paper scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0076_institutional_scale]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional paper scale for domestic goods handling from prepared paper stock.",
		Action = "making institutional paper scale for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional paper scale for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5148.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5148 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0076_institutional_scale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional paper scale for domestic goods handling (@industrialised_logistics_retail_hospitality_0076_institutional_scale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalCottonTicketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0077InstitutionalTicket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional cotton ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0077_institutional_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0077_institutional_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0077_institutional_ticket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional cotton ticket for domestic goods handling from prepared cotton stock.",
		Action = "making institutional cotton ticket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional cotton ticket for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5189.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5189 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0077_institutional_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton ticket for domestic goods handling (@industrialised_logistics_retail_hospitality_0077_institutional_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalOakBasketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0078InstitutionalBasket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional oak basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0078_institutional_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0078_institutional_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0078_institutional_basket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional oak basket for domestic goods handling from prepared oak stock.",
		Action = "making institutional oak basket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional oak basket for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5229.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5229 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0078_institutional_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak basket for domestic goods handling (@industrialised_logistics_retail_hospitality_0078_institutional_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalMildSteelCaseForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0079InstitutionalCase() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional mild steel case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0079_institutional_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0079_institutional_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0079_institutional_case]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional mild steel case for domestic goods handling from prepared mild steel stock.",
		Action = "making institutional mild steel case for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel case for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5270.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5270 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0079_institutional_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel case for domestic goods handling (@industrialised_logistics_retail_hospitality_0079_institutional_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeInstitutionalPaperSignForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0080InstitutionalSign() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make institutional paper sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0080_institutional_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0080_institutional_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional paper sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0080_institutional_sign]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make an institutional paper sign for domestic goods handling from prepared paper stock.",
		Action = "making institutional paper sign for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress institutional paper sign for domestic goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5311.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5311 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0080_institutional_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional paper sign for domestic goods handling (@industrialised_logistics_retail_hospitality_0080_institutional_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactCottonWasteForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0001CompactWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact cotton waste for workshop goods handling [craft_industrial_logistics_retail_hospitality_0001_compact_waste]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0001_compact_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton waste for workshop goods handling [craft_industrial_logistics_retail_hospitality_0001_compact_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact cotton waste for workshop goods handling from prepared cotton stock.",
		Action = "making compact cotton waste for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact cotton waste for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2096.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2096 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0001_compact_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton waste for workshop goods handling (@industrial_logistics_retail_hospitality_0001_compact_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactOakCrateForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0002CompactCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact oak crate for workshop goods handling [craft_industrial_logistics_retail_hospitality_0002_compact_crate]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0002_compact_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak crate for workshop goods handling [craft_industrial_logistics_retail_hospitality_0002_compact_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact oak crate for workshop goods handling from prepared oak stock.",
		Action = "making compact oak crate for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact oak crate for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2136.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2136 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0002_compact_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak crate for workshop goods handling (@industrial_logistics_retail_hospitality_0002_compact_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactMildSteelTrolleyForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0003CompactTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact mild steel trolley for workshop goods handling [craft_industrial_logistics_retail_hospitality_0003_compact_trolley]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0003_compact_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel trolley for workshop goods handling [craft_industrial_logistics_retail_hospitality_0003_compact_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact mild steel trolley for workshop goods handling from prepared mild steel stock.",
		Action = "making compact mild steel trolley for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact mild steel trolley for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2177.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2177 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0003_compact_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel trolley for workshop goods handling (@industrial_logistics_retail_hospitality_0003_compact_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactPaperTrayForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0004CompactTray() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact paper tray for workshop goods handling [craft_industrial_logistics_retail_hospitality_0004_compact_tray]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0004_compact_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper tray for workshop goods handling [craft_industrial_logistics_retail_hospitality_0004_compact_tray]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact paper tray for workshop goods handling from prepared paper stock.",
		Action = "making compact paper tray for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact paper tray for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2218.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2218 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0004_compact_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper tray for workshop goods handling (@industrial_logistics_retail_hospitality_0004_compact_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactCottonCounterForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0005CompactCounter() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact cotton counter for workshop goods handling [craft_industrial_logistics_retail_hospitality_0005_compact_counter]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0005_compact_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton counter for workshop goods handling [craft_industrial_logistics_retail_hospitality_0005_compact_counter]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact cotton counter for workshop goods handling from prepared cotton stock.",
		Action = "making compact cotton counter for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact cotton counter for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2258.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2258 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0005_compact_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton counter for workshop goods handling (@industrial_logistics_retail_hospitality_0005_compact_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactOakScaleForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0006CompactScale() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact oak scale for workshop goods handling [craft_industrial_logistics_retail_hospitality_0006_compact_scale]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0006_compact_scale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak scale for workshop goods handling [craft_industrial_logistics_retail_hospitality_0006_compact_scale]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact oak scale for workshop goods handling from prepared oak stock.",
		Action = "making compact oak scale for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact oak scale for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2299.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2299 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0006_compact_scale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak scale for workshop goods handling (@industrial_logistics_retail_hospitality_0006_compact_scale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactMildSteelTicketForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0007CompactTicket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact mild steel ticket for workshop goods handling [craft_industrial_logistics_retail_hospitality_0007_compact_ticket]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0007_compact_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel ticket for workshop goods handling [craft_industrial_logistics_retail_hospitality_0007_compact_ticket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact mild steel ticket for workshop goods handling from prepared mild steel stock.",
		Action = "making compact mild steel ticket for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact mild steel ticket for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2340.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2340 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0007_compact_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel ticket for workshop goods handling (@industrial_logistics_retail_hospitality_0007_compact_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactPaperBasketForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0008CompactBasket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact paper basket for workshop goods handling [craft_industrial_logistics_retail_hospitality_0008_compact_basket]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0008_compact_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper basket for workshop goods handling [craft_industrial_logistics_retail_hospitality_0008_compact_basket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact paper basket for workshop goods handling from prepared paper stock.",
		Action = "making compact paper basket for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact paper basket for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2380.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2380 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0008_compact_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper basket for workshop goods handling (@industrial_logistics_retail_hospitality_0008_compact_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactCottonCaseForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0009CompactCase() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact cotton case for workshop goods handling [craft_industrial_logistics_retail_hospitality_0009_compact_case]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0009_compact_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton case for workshop goods handling [craft_industrial_logistics_retail_hospitality_0009_compact_case]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact cotton case for workshop goods handling from prepared cotton stock.",
		Action = "making compact cotton case for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact cotton case for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2421.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2421 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0009_compact_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton case for workshop goods handling (@industrial_logistics_retail_hospitality_0009_compact_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactOakSignForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0010CompactSign() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact oak sign for workshop goods handling [craft_industrial_logistics_retail_hospitality_0010_compact_sign]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0010_compact_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak sign for workshop goods handling [craft_industrial_logistics_retail_hospitality_0010_compact_sign]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact oak sign for workshop goods handling from prepared oak stock.",
		Action = "making compact oak sign for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress compact oak sign for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2462.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2462 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0010_compact_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak sign for workshop goods handling (@industrial_logistics_retail_hospitality_0010_compact_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyMildSteelWasteForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0011HeavyDutyWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty mild steel waste for workshop goods handling [craft_industrial_logistics_retail_hospitality_0011_heavy_duty_waste]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0011_heavy_duty_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel waste for workshop goods handling [craft_industrial_logistics_retail_hospitality_0011_heavy_duty_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty mild steel waste for workshop goods handling from prepared mild steel stock.",
		Action = "making heavy-duty mild steel waste for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel waste for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2502.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2502 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0011_heavy_duty_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel waste for workshop goods handling (@industrial_logistics_retail_hospitality_0011_heavy_duty_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyPaperCrateForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0012HeavyDutyCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty paper crate for workshop goods handling [craft_industrial_logistics_retail_hospitality_0012_heavy_duty_crate]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0012_heavy_duty_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper crate for workshop goods handling [craft_industrial_logistics_retail_hospitality_0012_heavy_duty_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty paper crate for workshop goods handling from prepared paper stock.",
		Action = "making heavy-duty paper crate for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper crate for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2543.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2543 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0012_heavy_duty_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper crate for workshop goods handling (@industrial_logistics_retail_hospitality_0012_heavy_duty_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyCottonTrolleyForWorkshopGoodsHandlingCraftIndustrialLogisticsRetailHospitality0013HeavyDutyTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty cotton trolley for workshop goods handling [craft_industrial_logistics_retail_hospitality_0013_heavy_duty_trolley]", "crafts", ["industrial"], ["item:industrial_logistics_retail_hospitality_0013_heavy_duty_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton trolley for workshop goods handling [craft_industrial_logistics_retail_hospitality_0013_heavy_duty_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty cotton trolley for workshop goods handling from prepared cotton stock.",
		Action = "making heavy-duty cotton trolley for workshop goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton trolley for workshop goods handling craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2584.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2584 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_logistics_retail_hospitality_0013_heavy_duty_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton trolley for workshop goods handling (@industrial_logistics_retail_hospitality_0013_heavy_duty_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainGlassWasteForDomesticInstructionCraftIndustrialisedScienceEducation0001PlainWaste() => new()
	{
		Metadata = new("Science and education/make plain glass waste for domestic instruction [craft_industrialised_science_education_0001_plain_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0001_plain_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass waste for domestic instruction [craft_industrialised_science_education_0001_plain_waste]",
		Category = "Science and education",
		Blurb = "Make a plain glass waste for domestic instruction from prepared glass stock.",
		Action = "making plain glass waste for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain glass waste for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2207.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2207 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0001_plain_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass waste for domestic instruction (@industrialised_science_education_0001_plain_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainPaperSpecimenForDomesticInstructionCraftIndustrialisedScienceEducation0002PlainSpecimen() => new()
	{
		Metadata = new("Science and education/make plain paper specimen for domestic instruction [craft_industrialised_science_education_0002_plain_specimen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0002_plain_specimen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper specimen for domestic instruction [craft_industrialised_science_education_0002_plain_specimen]",
		Category = "Science and education",
		Blurb = "Make a plain paper specimen for domestic instruction from prepared paper stock.",
		Action = "making plain paper specimen for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain paper specimen for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2247.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2247 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0002_plain_specimen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper specimen for domestic instruction (@industrialised_science_education_0002_plain_specimen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainOakSlideForDomesticInstructionCraftIndustrialisedScienceEducation0003PlainSlide() => new()
	{
		Metadata = new("Science and education/make plain oak slide for domestic instruction [craft_industrialised_science_education_0003_plain_slide]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0003_plain_slide"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak slide for domestic instruction [craft_industrialised_science_education_0003_plain_slide]",
		Category = "Science and education",
		Blurb = "Make a plain oak slide for domestic instruction from prepared oak stock.",
		Action = "making plain oak slide for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain oak slide for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2288.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2288 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0003_plain_slide", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak slide for domestic instruction (@industrialised_science_education_0003_plain_slide)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainMildSteelFlaskForDomesticInstructionCraftIndustrialisedScienceEducation0004PlainFlask() => new()
	{
		Metadata = new("Science and education/make plain mild steel flask for domestic instruction [craft_industrialised_science_education_0004_plain_flask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0004_plain_flask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel flask for domestic instruction [craft_industrialised_science_education_0004_plain_flask]",
		Category = "Science and education",
		Blurb = "Make a plain mild steel flask for domestic instruction from prepared mild steel stock.",
		Action = "making plain mild steel flask for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain mild steel flask for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2329.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2329 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0004_plain_flask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel flask for domestic instruction (@industrialised_science_education_0004_plain_flask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainGlassBeakerForDomesticInstructionCraftIndustrialisedScienceEducation0005PlainBeaker() => new()
	{
		Metadata = new("Science and education/make plain glass beaker for domestic instruction [craft_industrialised_science_education_0005_plain_beaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0005_plain_beaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass beaker for domestic instruction [craft_industrialised_science_education_0005_plain_beaker]",
		Category = "Science and education",
		Blurb = "Make a plain glass beaker for domestic instruction from prepared glass stock.",
		Action = "making plain glass beaker for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain glass beaker for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2369.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2369 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0005_plain_beaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass beaker for domestic instruction (@industrialised_science_education_0005_plain_beaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainPaperModelForDomesticInstructionCraftIndustrialisedScienceEducation0006PlainModel() => new()
	{
		Metadata = new("Science and education/make plain paper model for domestic instruction [craft_industrialised_science_education_0006_plain_model]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0006_plain_model"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper model for domestic instruction [craft_industrialised_science_education_0006_plain_model]",
		Category = "Science and education",
		Blurb = "Make a plain paper model for domestic instruction from prepared paper stock.",
		Action = "making plain paper model for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain paper model for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0006_plain_model", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper model for domestic instruction (@industrialised_science_education_0006_plain_model)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainOakChartForDomesticInstructionCraftIndustrialisedScienceEducation0007PlainChart() => new()
	{
		Metadata = new("Science and education/make plain oak chart for domestic instruction [craft_industrialised_science_education_0007_plain_chart]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0007_plain_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak chart for domestic instruction [craft_industrialised_science_education_0007_plain_chart]",
		Category = "Science and education",
		Blurb = "Make a plain oak chart for domestic instruction from prepared oak stock.",
		Action = "making plain oak chart for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain oak chart for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2451.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2451 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0007_plain_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak chart for domestic instruction (@industrialised_science_education_0007_plain_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainMildSteelPointerForDomesticInstructionCraftIndustrialisedScienceEducation0008PlainPointer() => new()
	{
		Metadata = new("Science and education/make plain mild steel pointer for domestic instruction [craft_industrialised_science_education_0008_plain_pointer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0008_plain_pointer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel pointer for domestic instruction [craft_industrialised_science_education_0008_plain_pointer]",
		Category = "Science and education",
		Blurb = "Make a plain mild steel pointer for domestic instruction from prepared mild steel stock.",
		Action = "making plain mild steel pointer for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain mild steel pointer for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2492.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2492 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0008_plain_pointer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel pointer for domestic instruction (@industrialised_science_education_0008_plain_pointer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainGlassBalanceForDomesticInstructionCraftIndustrialisedScienceEducation0009PlainBalance() => new()
	{
		Metadata = new("Science and education/make plain glass balance for domestic instruction [craft_industrialised_science_education_0009_plain_balance]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0009_plain_balance"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass balance for domestic instruction [craft_industrialised_science_education_0009_plain_balance]",
		Category = "Science and education",
		Blurb = "Make a plain glass balance for domestic instruction from prepared glass stock.",
		Action = "making plain glass balance for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain glass balance for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2532.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2532 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0009_plain_balance", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass balance for domestic instruction (@industrialised_science_education_0009_plain_balance)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePlainPaperCaseForDomesticInstructionCraftIndustrialisedScienceEducation0010PlainCase() => new()
	{
		Metadata = new("Science and education/make plain paper case for domestic instruction [craft_industrialised_science_education_0010_plain_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0010_plain_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper case for domestic instruction [craft_industrialised_science_education_0010_plain_case]",
		Category = "Science and education",
		Blurb = "Make a plain paper case for domestic instruction from prepared paper stock.",
		Action = "making plain paper case for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress plain paper case for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2573.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2573 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0010_plain_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper case for domestic instruction (@industrialised_science_education_0010_plain_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedOakWasteForDomesticInstructionCraftIndustrialisedScienceEducation0011ReinforcedWaste() => new()
	{
		Metadata = new("Science and education/make reinforced oak waste for domestic instruction [craft_industrialised_science_education_0011_reinforced_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0011_reinforced_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak waste for domestic instruction [craft_industrialised_science_education_0011_reinforced_waste]",
		Category = "Science and education",
		Blurb = "Make a reinforced oak waste for domestic instruction from prepared oak stock.",
		Action = "making reinforced oak waste for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced oak waste for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2614.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2614 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0011_reinforced_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak waste for domestic instruction (@industrialised_science_education_0011_reinforced_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedMildSteelSpecimenForDomesticInstructionCraftIndustrialisedScienceEducation0012ReinforcedSpecimen() => new()
	{
		Metadata = new("Science and education/make reinforced mild steel specimen for domestic instruction [craft_industrialised_science_education_0012_reinforced_specimen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0012_reinforced_specimen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel specimen for domestic instruction [craft_industrialised_science_education_0012_reinforced_specimen]",
		Category = "Science and education",
		Blurb = "Make a reinforced mild steel specimen for domestic instruction from prepared mild steel stock.",
		Action = "making reinforced mild steel specimen for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel specimen for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2654.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2654 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0012_reinforced_specimen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel specimen for domestic instruction (@industrialised_science_education_0012_reinforced_specimen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedGlassSlideForDomesticInstructionCraftIndustrialisedScienceEducation0013ReinforcedSlide() => new()
	{
		Metadata = new("Science and education/make reinforced glass slide for domestic instruction [craft_industrialised_science_education_0013_reinforced_slide]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0013_reinforced_slide"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass slide for domestic instruction [craft_industrialised_science_education_0013_reinforced_slide]",
		Category = "Science and education",
		Blurb = "Make a reinforced glass slide for domestic instruction from prepared glass stock.",
		Action = "making reinforced glass slide for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced glass slide for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2695.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2695 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0013_reinforced_slide", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass slide for domestic instruction (@industrialised_science_education_0013_reinforced_slide)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedPaperFlaskForDomesticInstructionCraftIndustrialisedScienceEducation0014ReinforcedFlask() => new()
	{
		Metadata = new("Science and education/make reinforced paper flask for domestic instruction [craft_industrialised_science_education_0014_reinforced_flask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0014_reinforced_flask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper flask for domestic instruction [craft_industrialised_science_education_0014_reinforced_flask]",
		Category = "Science and education",
		Blurb = "Make a reinforced paper flask for domestic instruction from prepared paper stock.",
		Action = "making reinforced paper flask for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced paper flask for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2736.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2736 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0014_reinforced_flask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper flask for domestic instruction (@industrialised_science_education_0014_reinforced_flask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
