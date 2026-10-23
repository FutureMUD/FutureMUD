#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingCeramicWasteForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0041FoldingWaste() => new()
	{
		Metadata = new("Cleaning and personal care/make folding ceramic waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0041_folding_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0041_folding_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0041_folding_waste]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding ceramic waste for domestic hygiene from prepared ceramic stock.",
		Action = "making folding ceramic waste for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding ceramic waste for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2390.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2390 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0041_folding_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic waste for domestic hygiene (@industrialised_cleaning_personal_care_0041_folding_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingOakBrushForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0042FoldingBrush() => new()
	{
		Metadata = new("Cleaning and personal care/make folding oak brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0042_folding_brush]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0042_folding_brush"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0042_folding_brush]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding oak brush for domestic hygiene from prepared oak stock.",
		Action = "making folding oak brush for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding oak brush for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2431.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2431 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0042_folding_brush", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak brush for domestic hygiene (@industrialised_cleaning_personal_care_0042_folding_brush)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingCottonCombForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0043FoldingComb() => new()
	{
		Metadata = new("Cleaning and personal care/make folding cotton comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0043_folding_comb]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0043_folding_comb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0043_folding_comb]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding cotton comb for domestic hygiene from prepared cotton stock.",
		Action = "making folding cotton comb for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding cotton comb for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2472.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2472 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0043_folding_comb", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton comb for domestic hygiene (@industrialised_cleaning_personal_care_0043_folding_comb)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingMildSteelRazorForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0044FoldingRazor() => new()
	{
		Metadata = new("Cleaning and personal care/make folding mild steel razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0044_folding_razor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0044_folding_razor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0044_folding_razor]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding mild steel razor for domestic hygiene from prepared mild steel stock.",
		Action = "making folding mild steel razor for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding mild steel razor for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2512.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2512 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0044_folding_razor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel razor for domestic hygiene (@industrialised_cleaning_personal_care_0044_folding_razor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingCeramicSpongeForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0045FoldingSponge() => new()
	{
		Metadata = new("Cleaning and personal care/make folding ceramic sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0045_folding_sponge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0045_folding_sponge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0045_folding_sponge]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding ceramic sponge for domestic hygiene from prepared ceramic stock.",
		Action = "making folding ceramic sponge for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding ceramic sponge for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2553.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2553 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0045_folding_sponge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic sponge for domestic hygiene (@industrialised_cleaning_personal_care_0045_folding_sponge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingOakClothForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0046FoldingCloth() => new()
	{
		Metadata = new("Cleaning and personal care/make folding oak cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0046_folding_cloth]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0046_folding_cloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0046_folding_cloth]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding oak cloth for domestic hygiene from prepared oak stock.",
		Action = "making folding oak cloth for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding oak cloth for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2594.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2594 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0046_folding_cloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak cloth for domestic hygiene (@industrialised_cleaning_personal_care_0046_folding_cloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingCottonBroomForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0047FoldingBroom() => new()
	{
		Metadata = new("Cleaning and personal care/make folding cotton broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0047_folding_broom]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0047_folding_broom"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0047_folding_broom]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding cotton broom for domestic hygiene from prepared cotton stock.",
		Action = "making folding cotton broom for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding cotton broom for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2634.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2634 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0047_folding_broom", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton broom for domestic hygiene (@industrialised_cleaning_personal_care_0047_folding_broom)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingMildSteelMopForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0048FoldingMop() => new()
	{
		Metadata = new("Cleaning and personal care/make folding mild steel mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0048_folding_mop]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0048_folding_mop"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0048_folding_mop]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding mild steel mop for domestic hygiene from prepared mild steel stock.",
		Action = "making folding mild steel mop for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding mild steel mop for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2675.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2675 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0048_folding_mop", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel mop for domestic hygiene (@industrialised_cleaning_personal_care_0048_folding_mop)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingCeramicBucketForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0049FoldingBucket() => new()
	{
		Metadata = new("Cleaning and personal care/make folding ceramic bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0049_folding_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0049_folding_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0049_folding_bucket]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding ceramic bucket for domestic hygiene from prepared ceramic stock.",
		Action = "making folding ceramic bucket for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding ceramic bucket for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2716.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2716 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0049_folding_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic bucket for domestic hygiene (@industrialised_cleaning_personal_care_0049_folding_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeFoldingOakSoapdishForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0050FoldingSoapdish() => new()
	{
		Metadata = new("Cleaning and personal care/make folding oak soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0050_folding_soapdish]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0050_folding_soapdish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0050_folding_soapdish]",
		Category = "Cleaning and personal care",
		Blurb = "Make a folding oak soapdish for domestic hygiene from prepared oak stock.",
		Action = "making folding oak soapdish for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress folding oak soapdish for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2757.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2757 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0050_folding_soapdish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak soapdish for domestic hygiene (@industrialised_cleaning_personal_care_0050_folding_soapdish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableCottonWasteForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0051PortableWaste() => new()
	{
		Metadata = new("Cleaning and personal care/make portable cotton waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0051_portable_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0051_portable_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0051_portable_waste]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable cotton waste for domestic hygiene from prepared cotton stock.",
		Action = "making portable cotton waste for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable cotton waste for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2797.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2797 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0051_portable_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton waste for domestic hygiene (@industrialised_cleaning_personal_care_0051_portable_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableMildSteelBrushForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0052PortableBrush() => new()
	{
		Metadata = new("Cleaning and personal care/make portable mild steel brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0052_portable_brush]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0052_portable_brush"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0052_portable_brush]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable mild steel brush for domestic hygiene from prepared mild steel stock.",
		Action = "making portable mild steel brush for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable mild steel brush for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2838.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2838 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0052_portable_brush", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel brush for domestic hygiene (@industrialised_cleaning_personal_care_0052_portable_brush)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableCeramicCombForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0053PortableComb() => new()
	{
		Metadata = new("Cleaning and personal care/make portable ceramic comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0053_portable_comb]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0053_portable_comb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0053_portable_comb]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable ceramic comb for domestic hygiene from prepared ceramic stock.",
		Action = "making portable ceramic comb for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable ceramic comb for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2879.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2879 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0053_portable_comb", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic comb for domestic hygiene (@industrialised_cleaning_personal_care_0053_portable_comb)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableOakRazorForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0054PortableRazor() => new()
	{
		Metadata = new("Cleaning and personal care/make portable oak razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0054_portable_razor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0054_portable_razor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0054_portable_razor]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable oak razor for domestic hygiene from prepared oak stock.",
		Action = "making portable oak razor for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable oak razor for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2919.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2919 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0054_portable_razor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak razor for domestic hygiene (@industrialised_cleaning_personal_care_0054_portable_razor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableCottonSpongeForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0055PortableSponge() => new()
	{
		Metadata = new("Cleaning and personal care/make portable cotton sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0055_portable_sponge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0055_portable_sponge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0055_portable_sponge]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable cotton sponge for domestic hygiene from prepared cotton stock.",
		Action = "making portable cotton sponge for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable cotton sponge for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2960.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2960 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0055_portable_sponge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton sponge for domestic hygiene (@industrialised_cleaning_personal_care_0055_portable_sponge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableMildSteelClothForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0056PortableCloth() => new()
	{
		Metadata = new("Cleaning and personal care/make portable mild steel cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0056_portable_cloth]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0056_portable_cloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0056_portable_cloth]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable mild steel cloth for domestic hygiene from prepared mild steel stock.",
		Action = "making portable mild steel cloth for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable mild steel cloth for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3001.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3001 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0056_portable_cloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel cloth for domestic hygiene (@industrialised_cleaning_personal_care_0056_portable_cloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableCeramicBroomForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0057PortableBroom() => new()
	{
		Metadata = new("Cleaning and personal care/make portable ceramic broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0057_portable_broom]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0057_portable_broom"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0057_portable_broom]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable ceramic broom for domestic hygiene from prepared ceramic stock.",
		Action = "making portable ceramic broom for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable ceramic broom for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3042.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3042 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0057_portable_broom", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic broom for domestic hygiene (@industrialised_cleaning_personal_care_0057_portable_broom)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableOakMopForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0058PortableMop() => new()
	{
		Metadata = new("Cleaning and personal care/make portable oak mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0058_portable_mop]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0058_portable_mop"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0058_portable_mop]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable oak mop for domestic hygiene from prepared oak stock.",
		Action = "making portable oak mop for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable oak mop for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3082.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3082 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0058_portable_mop", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak mop for domestic hygiene (@industrialised_cleaning_personal_care_0058_portable_mop)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableCottonBucketForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0059PortableBucket() => new()
	{
		Metadata = new("Cleaning and personal care/make portable cotton bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0059_portable_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0059_portable_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0059_portable_bucket]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable cotton bucket for domestic hygiene from prepared cotton stock.",
		Action = "making portable cotton bucket for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable cotton bucket for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3123.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3123 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0059_portable_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton bucket for domestic hygiene (@industrialised_cleaning_personal_care_0059_portable_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePortableMildSteelSoapdishForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0060PortableSoapdish() => new()
	{
		Metadata = new("Cleaning and personal care/make portable mild steel soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0060_portable_soapdish]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0060_portable_soapdish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0060_portable_soapdish]",
		Category = "Cleaning and personal care",
		Blurb = "Make a portable mild steel soapdish for domestic hygiene from prepared mild steel stock.",
		Action = "making portable mild steel soapdish for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress portable mild steel soapdish for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3164.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3164 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0060_portable_soapdish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel soapdish for domestic hygiene (@industrialised_cleaning_personal_care_0060_portable_soapdish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeWorkshopCeramicWasteForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0061WorkshopWaste() => new()
	{
		Metadata = new("Cleaning and personal care/make workshop ceramic waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0061_workshop_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0061_workshop_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0061_workshop_waste]",
		Category = "Cleaning and personal care",
		Blurb = "Make a workshop ceramic waste for domestic hygiene from prepared ceramic stock.",
		Action = "making workshop ceramic waste for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic waste for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3204.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3204 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0061_workshop_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic waste for domestic hygiene (@industrialised_cleaning_personal_care_0061_workshop_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeWorkshopOakBrushForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0062WorkshopBrush() => new()
	{
		Metadata = new("Cleaning and personal care/make workshop oak brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0062_workshop_brush]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0062_workshop_brush"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0062_workshop_brush]",
		Category = "Cleaning and personal care",
		Blurb = "Make a workshop oak brush for domestic hygiene from prepared oak stock.",
		Action = "making workshop oak brush for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress workshop oak brush for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3245.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3245 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0062_workshop_brush", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak brush for domestic hygiene (@industrialised_cleaning_personal_care_0062_workshop_brush)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeWallMountedCottonWasteForDomesticHygieneCraftIndustrialCleaningPersonalCare0001WallMountedWaste() => new()
	{
		Metadata = new("Cleaning and personal care/make wall-mounted cotton waste for domestic hygiene [craft_industrial_cleaning_personal_care_0001_wall_mounted_waste]", "crafts", ["industrial"], ["item:industrial_cleaning_personal_care_0001_wall_mounted_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted cotton waste for domestic hygiene [craft_industrial_cleaning_personal_care_0001_wall_mounted_waste]",
		Category = "Cleaning and personal care",
		Blurb = "Make a wall-mounted cotton waste for domestic hygiene from prepared cotton stock.",
		Action = "making wall-mounted cotton waste for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress wall-mounted cotton waste for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 762.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("762 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_cleaning_personal_care_0001_wall_mounted_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted cotton waste for domestic hygiene (@industrial_cleaning_personal_care_0001_wall_mounted_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeWallMountedMildSteelBrushForDomesticHygieneCraftIndustrialCleaningPersonalCare0002WallMountedBrush() => new()
	{
		Metadata = new("Cleaning and personal care/make wall-mounted mild steel brush for domestic hygiene [craft_industrial_cleaning_personal_care_0002_wall_mounted_brush]", "crafts", ["industrial"], ["item:industrial_cleaning_personal_care_0002_wall_mounted_brush"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted mild steel brush for domestic hygiene [craft_industrial_cleaning_personal_care_0002_wall_mounted_brush]",
		Category = "Cleaning and personal care",
		Blurb = "Make a wall-mounted mild steel brush for domestic hygiene from prepared mild steel stock.",
		Action = "making wall-mounted mild steel brush for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress wall-mounted mild steel brush for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 803.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("803 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_cleaning_personal_care_0002_wall_mounted_brush", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted mild steel brush for domestic hygiene (@industrial_cleaning_personal_care_0002_wall_mounted_brush)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeWallMountedCeramicCombForDomesticHygieneCraftIndustrialCleaningPersonalCare0003WallMountedComb() => new()
	{
		Metadata = new("Cleaning and personal care/make wall-mounted ceramic comb for domestic hygiene [craft_industrial_cleaning_personal_care_0003_wall_mounted_comb]", "crafts", ["industrial"], ["item:industrial_cleaning_personal_care_0003_wall_mounted_comb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted ceramic comb for domestic hygiene [craft_industrial_cleaning_personal_care_0003_wall_mounted_comb]",
		Category = "Cleaning and personal care",
		Blurb = "Make a wall-mounted ceramic comb for domestic hygiene from prepared ceramic stock.",
		Action = "making wall-mounted ceramic comb for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress wall-mounted ceramic comb for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 844.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("844 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_cleaning_personal_care_0003_wall_mounted_comb", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted ceramic comb for domestic hygiene (@industrial_cleaning_personal_care_0003_wall_mounted_comb)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeWallMountedOakRazorForDomesticHygieneCraftIndustrialCleaningPersonalCare0004WallMountedRazor() => new()
	{
		Metadata = new("Cleaning and personal care/make wall-mounted oak razor for domestic hygiene [craft_industrial_cleaning_personal_care_0004_wall_mounted_razor]", "crafts", ["industrial"], ["item:industrial_cleaning_personal_care_0004_wall_mounted_razor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted oak razor for domestic hygiene [craft_industrial_cleaning_personal_care_0004_wall_mounted_razor]",
		Category = "Cleaning and personal care",
		Blurb = "Make a wall-mounted oak razor for domestic hygiene from prepared oak stock.",
		Action = "making wall-mounted oak razor for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress wall-mounted oak razor for domestic hygiene craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 884.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("884 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_cleaning_personal_care_0004_wall_mounted_razor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted oak razor for domestic hygiene (@industrial_cleaning_personal_care_0004_wall_mounted_razor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainPaperWastepaperForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0001PlainWastepaper() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0001_plain_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0001_plain_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0001_plain_wastepaper]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain paper wastepaper for domestic administration from prepared paper stock.",
		Action = "making plain paper wastepaper for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain paper wastepaper for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 873.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("873 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0001_plain_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper wastepaper for domestic administration (@industrialised_office_school_mail_payment_0001_plain_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainOakLedgerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0002PlainLedger() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0002_plain_ledger]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0002_plain_ledger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0002_plain_ledger]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain oak ledger for domestic administration from prepared oak stock.",
		Action = "making plain oak ledger for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain oak ledger for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 914.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("914 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0002_plain_ledger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak ledger for domestic administration (@industrialised_office_school_mail_payment_0002_plain_ledger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainMildSteelNotebookForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0003PlainNotebook() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0003_plain_notebook]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0003_plain_notebook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0003_plain_notebook]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain mild steel notebook for domestic administration from prepared mild steel stock.",
		Action = "making plain mild steel notebook for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain mild steel notebook for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 955.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("955 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0003_plain_notebook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel notebook for domestic administration (@industrialised_office_school_mail_payment_0003_plain_notebook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainLeatherEnvelopeForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0004PlainEnvelope() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0004_plain_envelope]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0004_plain_envelope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0004_plain_envelope]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain leather envelope for domestic administration from prepared leather stock.",
		Action = "making plain leather envelope for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain leather envelope for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 996.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("996 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0004_plain_envelope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather envelope for domestic administration (@industrialised_office_school_mail_payment_0004_plain_envelope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainPaperFolderForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0005PlainFolder() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0005_plain_folder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0005_plain_folder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0005_plain_folder]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain paper folder for domestic administration from prepared paper stock.",
		Action = "making plain paper folder for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain paper folder for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1036.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1036 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0005_plain_folder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper folder for domestic administration (@industrialised_office_school_mail_payment_0005_plain_folder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainOakStampForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0006PlainStamp() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0006_plain_stamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0006_plain_stamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0006_plain_stamp]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain oak stamp for domestic administration from prepared oak stock.",
		Action = "making plain oak stamp for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain oak stamp for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1077.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1077 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0006_plain_stamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak stamp for domestic administration (@industrialised_office_school_mail_payment_0006_plain_stamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainMildSteelTrayForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0007PlainTray() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0007_plain_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0007_plain_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0007_plain_tray]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain mild steel tray for domestic administration from prepared mild steel stock.",
		Action = "making plain mild steel tray for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain mild steel tray for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1118.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1118 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0007_plain_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel tray for domestic administration (@industrialised_office_school_mail_payment_0007_plain_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainLeatherRulerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0008PlainRuler() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0008_plain_ruler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0008_plain_ruler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0008_plain_ruler]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain leather ruler for domestic administration from prepared leather stock.",
		Action = "making plain leather ruler for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain leather ruler for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1158.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1158 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0008_plain_ruler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather ruler for domestic administration (@industrialised_office_school_mail_payment_0008_plain_ruler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainPaperPenForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0009PlainPen() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0009_plain_pen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0009_plain_pen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0009_plain_pen]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain paper pen for domestic administration from prepared paper stock.",
		Action = "making plain paper pen for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain paper pen for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1199.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1199 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0009_plain_pen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper pen for domestic administration (@industrialised_office_school_mail_payment_0009_plain_pen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainOakPencilForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0010PlainPencil() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0010_plain_pencil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0010_plain_pencil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0010_plain_pencil]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain oak pencil for domestic administration from prepared oak stock.",
		Action = "making plain oak pencil for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain oak pencil for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1240.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1240 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0010_plain_pencil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak pencil for domestic administration (@industrialised_office_school_mail_payment_0010_plain_pencil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainMildSteelSatchelForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0011PlainSatchel() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0011_plain_satchel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0011_plain_satchel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0011_plain_satchel]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain mild steel satchel for domestic administration from prepared mild steel stock.",
		Action = "making plain mild steel satchel for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain mild steel satchel for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1280.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1280 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0011_plain_satchel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel satchel for domestic administration (@industrialised_office_school_mail_payment_0011_plain_satchel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePlainLeatherRegisterForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0012PlainRegister() => new()
	{
		Metadata = new("Office, school, mail and payment/make plain leather register for domestic administration [craft_industrialised_office_school_mail_payment_0012_plain_register]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0012_plain_register"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather register for domestic administration [craft_industrialised_office_school_mail_payment_0012_plain_register]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a plain leather register for domestic administration from prepared leather stock.",
		Action = "making plain leather register for domestic administration",
		ActiveCraftItemSdesc = "an in-progress plain leather register for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1321.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1321 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0012_plain_register", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather register for domestic administration (@industrialised_office_school_mail_payment_0012_plain_register)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedPaperWastepaperForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0013ReinforcedWastepaper() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0013_reinforced_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0013_reinforced_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0013_reinforced_wastepaper]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced paper wastepaper for domestic administration from prepared paper stock.",
		Action = "making reinforced paper wastepaper for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced paper wastepaper for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1362.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1362 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0013_reinforced_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper wastepaper for domestic administration (@industrialised_office_school_mail_payment_0013_reinforced_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedOakLedgerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0014ReinforcedLedger() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0014_reinforced_ledger]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0014_reinforced_ledger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0014_reinforced_ledger]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced oak ledger for domestic administration from prepared oak stock.",
		Action = "making reinforced oak ledger for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced oak ledger for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1402.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1402 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0014_reinforced_ledger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak ledger for domestic administration (@industrialised_office_school_mail_payment_0014_reinforced_ledger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedMildSteelNotebookForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0015ReinforcedNotebook() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0015_reinforced_notebook]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0015_reinforced_notebook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0015_reinforced_notebook]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced mild steel notebook for domestic administration from prepared mild steel stock.",
		Action = "making reinforced mild steel notebook for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel notebook for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1443.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1443 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0015_reinforced_notebook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel notebook for domestic administration (@industrialised_office_school_mail_payment_0015_reinforced_notebook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedLeatherEnvelopeForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0016ReinforcedEnvelope() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0016_reinforced_envelope]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0016_reinforced_envelope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0016_reinforced_envelope]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced leather envelope for domestic administration from prepared leather stock.",
		Action = "making reinforced leather envelope for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced leather envelope for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1484.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1484 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0016_reinforced_envelope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather envelope for domestic administration (@industrialised_office_school_mail_payment_0016_reinforced_envelope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedPaperFolderForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0017ReinforcedFolder() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0017_reinforced_folder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0017_reinforced_folder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0017_reinforced_folder]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced paper folder for domestic administration from prepared paper stock.",
		Action = "making reinforced paper folder for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced paper folder for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1525.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1525 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0017_reinforced_folder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper folder for domestic administration (@industrialised_office_school_mail_payment_0017_reinforced_folder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedOakStampForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0018ReinforcedStamp() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0018_reinforced_stamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0018_reinforced_stamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0018_reinforced_stamp]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced oak stamp for domestic administration from prepared oak stock.",
		Action = "making reinforced oak stamp for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced oak stamp for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1565.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1565 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0018_reinforced_stamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak stamp for domestic administration (@industrialised_office_school_mail_payment_0018_reinforced_stamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedMildSteelTrayForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0019ReinforcedTray() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0019_reinforced_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0019_reinforced_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0019_reinforced_tray]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced mild steel tray for domestic administration from prepared mild steel stock.",
		Action = "making reinforced mild steel tray for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel tray for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1606.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1606 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0019_reinforced_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel tray for domestic administration (@industrialised_office_school_mail_payment_0019_reinforced_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedLeatherRulerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0020ReinforcedRuler() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0020_reinforced_ruler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0020_reinforced_ruler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0020_reinforced_ruler]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced leather ruler for domestic administration from prepared leather stock.",
		Action = "making reinforced leather ruler for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced leather ruler for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1647.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1647 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0020_reinforced_ruler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather ruler for domestic administration (@industrialised_office_school_mail_payment_0020_reinforced_ruler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedPaperPenForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0021ReinforcedPen() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0021_reinforced_pen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0021_reinforced_pen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0021_reinforced_pen]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced paper pen for domestic administration from prepared paper stock.",
		Action = "making reinforced paper pen for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced paper pen for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1687.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1687 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0021_reinforced_pen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper pen for domestic administration (@industrialised_office_school_mail_payment_0021_reinforced_pen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedOakPencilForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0022ReinforcedPencil() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0022_reinforced_pencil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0022_reinforced_pencil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0022_reinforced_pencil]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced oak pencil for domestic administration from prepared oak stock.",
		Action = "making reinforced oak pencil for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced oak pencil for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1728.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1728 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0022_reinforced_pencil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak pencil for domestic administration (@industrialised_office_school_mail_payment_0022_reinforced_pencil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedMildSteelSatchelForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0023ReinforcedSatchel() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0023_reinforced_satchel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0023_reinforced_satchel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0023_reinforced_satchel]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced mild steel satchel for domestic administration from prepared mild steel stock.",
		Action = "making reinforced mild steel satchel for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel satchel for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1769.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1769 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0023_reinforced_satchel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel satchel for domestic administration (@industrialised_office_school_mail_payment_0023_reinforced_satchel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeReinforcedLeatherRegisterForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0024ReinforcedRegister() => new()
	{
		Metadata = new("Office, school, mail and payment/make reinforced leather register for domestic administration [craft_industrialised_office_school_mail_payment_0024_reinforced_register]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0024_reinforced_register"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather register for domestic administration [craft_industrialised_office_school_mail_payment_0024_reinforced_register]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a reinforced leather register for domestic administration from prepared leather stock.",
		Action = "making reinforced leather register for domestic administration",
		ActiveCraftItemSdesc = "an in-progress reinforced leather register for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1810.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1810 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0024_reinforced_register", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather register for domestic administration (@industrialised_office_school_mail_payment_0024_reinforced_register)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactPaperWastepaperForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0025CompactWastepaper() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0025_compact_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0025_compact_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0025_compact_wastepaper]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact paper wastepaper for domestic administration from prepared paper stock.",
		Action = "making compact paper wastepaper for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact paper wastepaper for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1850.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1850 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0025_compact_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper wastepaper for domestic administration (@industrialised_office_school_mail_payment_0025_compact_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactOakLedgerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0026CompactLedger() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0026_compact_ledger]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0026_compact_ledger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0026_compact_ledger]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact oak ledger for domestic administration from prepared oak stock.",
		Action = "making compact oak ledger for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact oak ledger for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1891.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1891 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0026_compact_ledger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak ledger for domestic administration (@industrialised_office_school_mail_payment_0026_compact_ledger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactMildSteelNotebookForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0027CompactNotebook() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0027_compact_notebook]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0027_compact_notebook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0027_compact_notebook]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact mild steel notebook for domestic administration from prepared mild steel stock.",
		Action = "making compact mild steel notebook for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact mild steel notebook for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1932.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1932 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0027_compact_notebook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel notebook for domestic administration (@industrialised_office_school_mail_payment_0027_compact_notebook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactLeatherEnvelopeForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0028CompactEnvelope() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0028_compact_envelope]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0028_compact_envelope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0028_compact_envelope]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact leather envelope for domestic administration from prepared leather stock.",
		Action = "making compact leather envelope for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact leather envelope for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1972.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1972 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0028_compact_envelope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather envelope for domestic administration (@industrialised_office_school_mail_payment_0028_compact_envelope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactPaperFolderForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0029CompactFolder() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0029_compact_folder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0029_compact_folder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0029_compact_folder]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact paper folder for domestic administration from prepared paper stock.",
		Action = "making compact paper folder for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact paper folder for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2013.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2013 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0029_compact_folder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper folder for domestic administration (@industrialised_office_school_mail_payment_0029_compact_folder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactOakStampForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0030CompactStamp() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0030_compact_stamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0030_compact_stamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0030_compact_stamp]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact oak stamp for domestic administration from prepared oak stock.",
		Action = "making compact oak stamp for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact oak stamp for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2054.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2054 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0030_compact_stamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak stamp for domestic administration (@industrialised_office_school_mail_payment_0030_compact_stamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactMildSteelTrayForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0031CompactTray() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0031_compact_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0031_compact_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0031_compact_tray]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact mild steel tray for domestic administration from prepared mild steel stock.",
		Action = "making compact mild steel tray for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact mild steel tray for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2094.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2094 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0031_compact_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel tray for domestic administration (@industrialised_office_school_mail_payment_0031_compact_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactLeatherRulerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0032CompactRuler() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0032_compact_ruler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0032_compact_ruler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0032_compact_ruler]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact leather ruler for domestic administration from prepared leather stock.",
		Action = "making compact leather ruler for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact leather ruler for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2135.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2135 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0032_compact_ruler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather ruler for domestic administration (@industrialised_office_school_mail_payment_0032_compact_ruler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactPaperPenForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0033CompactPen() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0033_compact_pen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0033_compact_pen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0033_compact_pen]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact paper pen for domestic administration from prepared paper stock.",
		Action = "making compact paper pen for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact paper pen for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2176.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2176 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0033_compact_pen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper pen for domestic administration (@industrialised_office_school_mail_payment_0033_compact_pen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactOakPencilForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0034CompactPencil() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0034_compact_pencil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0034_compact_pencil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0034_compact_pencil]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact oak pencil for domestic administration from prepared oak stock.",
		Action = "making compact oak pencil for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact oak pencil for domestic administration craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2216.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2216 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0034_compact_pencil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak pencil for domestic administration (@industrialised_office_school_mail_payment_0034_compact_pencil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
