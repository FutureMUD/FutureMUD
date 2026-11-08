#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes19Stock
{
	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeWorkshopLeatherSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0102WorkshopSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some workshop leather socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0102_workshop_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0102_workshop_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some workshop leather socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0102_workshop_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some workshop leather socks for domestic wear from prepared leather stock.",
		Action = "making some workshop leather socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some workshop leather socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4318.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4318 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0102_workshop_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some workshop leather socks for domestic wear (@industrialised_clothing_footwear_uniforms_0102_workshop_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopCottonVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0103WorkshopVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop cotton vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0103_workshop_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0103_workshop_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0103_workshop_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop cotton vest for domestic wear from prepared cotton stock.",
		Action = "making workshop cotton vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop cotton vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4358.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4358 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0103_workshop_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton vest for domestic wear (@industrialised_clothing_footwear_uniforms_0103_workshop_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopLinenCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0104WorkshopCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop linen coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0104_workshop_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0104_workshop_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop linen coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0104_workshop_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop linen coat for domestic wear from prepared linen stock.",
		Action = "making workshop linen coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop linen coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4399.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4399 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0104_workshop_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop linen coat for domestic wear (@industrialised_clothing_footwear_uniforms_0104_workshop_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopWoolSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0105WorkshopSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop wool smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0105_workshop_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0105_workshop_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop wool smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0105_workshop_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop wool smock for domestic wear from prepared wool stock.",
		Action = "making workshop wool smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop wool smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4440.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4440 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0105_workshop_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop wool smock for domestic wear (@industrialised_clothing_footwear_uniforms_0105_workshop_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalLeatherRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0106InstitutionalRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional leather rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0106_institutional_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0106_institutional_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional leather rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0106_institutional_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional leather rag for domestic wear from prepared leather stock.",
		Action = "making institutional leather rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional leather rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4480.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4480 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0106_institutional_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional leather rag for domestic wear (@industrialised_clothing_footwear_uniforms_0106_institutional_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalCottonApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0107InstitutionalApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional cotton apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0107_institutional_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0107_institutional_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0107_institutional_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional cotton apron for domestic wear from prepared cotton stock.",
		Action = "making institutional cotton apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional cotton apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4521.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4521 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0107_institutional_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton apron for domestic wear (@industrialised_clothing_footwear_uniforms_0107_institutional_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalLinenShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0108InstitutionalShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional linen shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0108_institutional_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0108_institutional_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional linen shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0108_institutional_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional linen shirt for domestic wear from prepared linen stock.",
		Action = "making institutional linen shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional linen shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4562.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4562 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0108_institutional_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional linen shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0108_institutional_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeInstitutionalWoolTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0109InstitutionalTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some institutional wool trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0109_institutional_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0109_institutional_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some institutional wool trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0109_institutional_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some institutional wool trousers for domestic wear from prepared wool stock.",
		Action = "making some institutional wool trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some institutional wool trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4602.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4602 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0109_institutional_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some institutional wool trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0109_institutional_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeInstitutionalLeatherBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0110InstitutionalBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some institutional leather boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0110_institutional_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0110_institutional_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some institutional leather boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0110_institutional_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some institutional leather boots for domestic wear from prepared leather stock.",
		Action = "making some institutional leather boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some institutional leather boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4643.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4643 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0110_institutional_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some institutional leather boots for domestic wear (@industrialised_clothing_footwear_uniforms_0110_institutional_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalCottonDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0111InstitutionalDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional cotton dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0111_institutional_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0111_institutional_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0111_institutional_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional cotton dress for domestic wear from prepared cotton stock.",
		Action = "making institutional cotton dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional cotton dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4684.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4684 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0111_institutional_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton dress for domestic wear (@industrialised_clothing_footwear_uniforms_0111_institutional_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalLinenJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0112InstitutionalJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional linen jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0112_institutional_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0112_institutional_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional linen jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0112_institutional_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional linen jacket for domestic wear from prepared linen stock.",
		Action = "making institutional linen jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional linen jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4724.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4724 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0112_institutional_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional linen jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0112_institutional_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalWoolHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0113InstitutionalHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional wool hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0113_institutional_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0113_institutional_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional wool hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0113_institutional_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional wool hat for domestic wear from prepared wool stock.",
		Action = "making institutional wool hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional wool hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4765.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4765 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0113_institutional_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional wool hat for domestic wear (@industrialised_clothing_footwear_uniforms_0113_institutional_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeInstitutionalLeatherGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0114InstitutionalGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some institutional leather gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0114_institutional_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0114_institutional_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some institutional leather gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0114_institutional_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some institutional leather gloves for domestic wear from prepared leather stock.",
		Action = "making some institutional leather gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some institutional leather gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4806.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4806 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0114_institutional_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some institutional leather gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0114_institutional_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalCottonScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0115InstitutionalScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional cotton scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0115_institutional_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0115_institutional_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0115_institutional_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional cotton scarf for domestic wear from prepared cotton stock.",
		Action = "making institutional cotton scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional cotton scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4847.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4847 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0115_institutional_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0115_institutional_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalLinenSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0116InstitutionalSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional linen skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0116_institutional_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0116_institutional_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional linen skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0116_institutional_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional linen skirt for domestic wear from prepared linen stock.",
		Action = "making institutional linen skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional linen skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4887.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4887 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0116_institutional_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional linen skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0116_institutional_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeInstitutionalWoolSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0117InstitutionalSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some institutional wool socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0117_institutional_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0117_institutional_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some institutional wool socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0117_institutional_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some institutional wool socks for domestic wear from prepared wool stock.",
		Action = "making some institutional wool socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some institutional wool socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4928.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4928 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0117_institutional_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some institutional wool socks for domestic wear (@industrialised_clothing_footwear_uniforms_0117_institutional_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalLeatherVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0118InstitutionalVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional leather vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0118_institutional_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0118_institutional_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional leather vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0118_institutional_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional leather vest for domestic wear from prepared leather stock.",
		Action = "making institutional leather vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional leather vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4969.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4969 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0118_institutional_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional leather vest for domestic wear (@industrialised_clothing_footwear_uniforms_0118_institutional_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalCottonCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0119InstitutionalCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional cotton coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0119_institutional_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0119_institutional_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0119_institutional_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional cotton coat for domestic wear from prepared cotton stock.",
		Action = "making institutional cotton coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional cotton coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5009.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5009 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0119_institutional_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton coat for domestic wear (@industrialised_clothing_footwear_uniforms_0119_institutional_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeInstitutionalLinenSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0120InstitutionalSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make institutional linen smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0120_institutional_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0120_institutional_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional linen smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0120_institutional_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an institutional linen smock for domestic wear from prepared linen stock.",
		Action = "making institutional linen smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress institutional linen smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5050.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5050 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0120_institutional_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional linen smock for domestic wear (@industrialised_clothing_footwear_uniforms_0120_institutional_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofWoolRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0121WeatherproofRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof wool rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0121_weatherproof_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0121_weatherproof_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof wool rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0121_weatherproof_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof wool rag for domestic wear from prepared wool stock.",
		Action = "making weatherproof wool rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof wool rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5091.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5091 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0121_weatherproof_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof wool rag for domestic wear (@industrialised_clothing_footwear_uniforms_0121_weatherproof_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofLeatherApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0122WeatherproofApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof leather apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0122_weatherproof_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0122_weatherproof_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof leather apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0122_weatherproof_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof leather apron for domestic wear from prepared leather stock.",
		Action = "making weatherproof leather apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof leather apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5132.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5132 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0122_weatherproof_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof leather apron for domestic wear (@industrialised_clothing_footwear_uniforms_0122_weatherproof_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofCottonShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0123WeatherproofShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof cotton shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0123_weatherproof_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0123_weatherproof_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0123_weatherproof_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof cotton shirt for domestic wear from prepared cotton stock.",
		Action = "making weatherproof cotton shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5172.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5172 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0123_weatherproof_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0123_weatherproof_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeWeatherproofLinenTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0124WeatherproofTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some weatherproof linen trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0124_weatherproof_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0124_weatherproof_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some weatherproof linen trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0124_weatherproof_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some weatherproof linen trousers for domestic wear from prepared linen stock.",
		Action = "making some weatherproof linen trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some weatherproof linen trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5213.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5213 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0124_weatherproof_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some weatherproof linen trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0124_weatherproof_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeWeatherproofWoolBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0125WeatherproofBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some weatherproof wool boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0125_weatherproof_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0125_weatherproof_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some weatherproof wool boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0125_weatherproof_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some weatherproof wool boots for domestic wear from prepared wool stock.",
		Action = "making some weatherproof wool boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some weatherproof wool boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5254.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5254 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0125_weatherproof_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some weatherproof wool boots for domestic wear (@industrialised_clothing_footwear_uniforms_0125_weatherproof_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofLeatherDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0126WeatherproofDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof leather dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0126_weatherproof_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0126_weatherproof_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof leather dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0126_weatherproof_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof leather dress for domestic wear from prepared leather stock.",
		Action = "making weatherproof leather dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof leather dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5294.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5294 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0126_weatherproof_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof leather dress for domestic wear (@industrialised_clothing_footwear_uniforms_0126_weatherproof_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofCottonJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0127WeatherproofJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof cotton jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0127_weatherproof_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0127_weatherproof_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0127_weatherproof_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof cotton jacket for domestic wear from prepared cotton stock.",
		Action = "making weatherproof cotton jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5335.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5335 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0127_weatherproof_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0127_weatherproof_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofLinenHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0128WeatherproofHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof linen hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0128_weatherproof_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0128_weatherproof_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof linen hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0128_weatherproof_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof linen hat for domestic wear from prepared linen stock.",
		Action = "making weatherproof linen hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof linen hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5376.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5376 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0128_weatherproof_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof linen hat for domestic wear (@industrialised_clothing_footwear_uniforms_0128_weatherproof_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeWeatherproofWoolGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0129WeatherproofGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some weatherproof wool gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0129_weatherproof_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0129_weatherproof_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some weatherproof wool gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0129_weatherproof_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some weatherproof wool gloves for domestic wear from prepared wool stock.",
		Action = "making some weatherproof wool gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some weatherproof wool gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5416.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5416 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0129_weatherproof_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some weatherproof wool gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0129_weatherproof_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofLeatherScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0130WeatherproofScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof leather scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0130_weatherproof_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0130_weatherproof_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof leather scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0130_weatherproof_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof leather scarf for domestic wear from prepared leather stock.",
		Action = "making weatherproof leather scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof leather scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5457.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5457 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0130_weatherproof_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof leather scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0130_weatherproof_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofCottonSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0131WeatherproofSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof cotton skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0131_weatherproof_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0131_weatherproof_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0131_weatherproof_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof cotton skirt for domestic wear from prepared cotton stock.",
		Action = "making weatherproof cotton skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5498.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5498 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0131_weatherproof_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0131_weatherproof_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeWeatherproofLinenSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0132WeatherproofSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some weatherproof linen socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0132_weatherproof_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0132_weatherproof_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some weatherproof linen socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0132_weatherproof_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some weatherproof linen socks for domestic wear from prepared linen stock.",
		Action = "making some weatherproof linen socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some weatherproof linen socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 94.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("94 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0132_weatherproof_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some weatherproof linen socks for domestic wear (@industrialised_clothing_footwear_uniforms_0132_weatherproof_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofWoolVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0133WeatherproofVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof wool vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0133_weatherproof_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0133_weatherproof_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof wool vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0133_weatherproof_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof wool vest for domestic wear from prepared wool stock.",
		Action = "making weatherproof wool vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof wool vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 134.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("134 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0133_weatherproof_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof wool vest for domestic wear (@industrialised_clothing_footwear_uniforms_0133_weatherproof_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofLeatherCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0134WeatherproofCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof leather coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0134_weatherproof_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0134_weatherproof_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof leather coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0134_weatherproof_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof leather coat for domestic wear from prepared leather stock.",
		Action = "making weatherproof leather coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof leather coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 175.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("175 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0134_weatherproof_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof leather coat for domestic wear (@industrialised_clothing_footwear_uniforms_0134_weatherproof_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWeatherproofCottonSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0135WeatherproofSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make weatherproof cotton smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0135_weatherproof_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0135_weatherproof_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0135_weatherproof_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a weatherproof cotton smock for domestic wear from prepared cotton stock.",
		Action = "making weatherproof cotton smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 216.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("216 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0135_weatherproof_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton smock for domestic wear (@industrialised_clothing_footwear_uniforms_0135_weatherproof_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceLinenRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0136ServiceRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service linen rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0136_service_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0136_service_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service linen rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0136_service_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service linen rag for domestic wear from prepared linen stock.",
		Action = "making service linen rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service linen rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 256.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("256 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0136_service_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service linen rag for domestic wear (@industrialised_clothing_footwear_uniforms_0136_service_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceWoolApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0137ServiceApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service wool apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0137_service_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0137_service_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service wool apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0137_service_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service wool apron for domestic wear from prepared wool stock.",
		Action = "making service wool apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service wool apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 297.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("297 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0137_service_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service wool apron for domestic wear (@industrialised_clothing_footwear_uniforms_0137_service_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceLeatherShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0138ServiceShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service leather shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0138_service_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0138_service_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service leather shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0138_service_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service leather shirt for domestic wear from prepared leather stock.",
		Action = "making service leather shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service leather shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0138_service_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service leather shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0138_service_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeServiceCottonTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0139ServiceTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some service cotton trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0139_service_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0139_service_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some service cotton trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0139_service_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some service cotton trousers for domestic wear from prepared cotton stock.",
		Action = "making some service cotton trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some service cotton trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 378.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("378 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0139_service_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some service cotton trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0139_service_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeServiceLinenBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0140ServiceBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some service linen boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0140_service_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0140_service_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some service linen boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0140_service_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some service linen boots for domestic wear from prepared linen stock.",
		Action = "making some service linen boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some service linen boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 419.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("419 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0140_service_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some service linen boots for domestic wear (@industrialised_clothing_footwear_uniforms_0140_service_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceWoolDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0141ServiceDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service wool dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0141_service_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0141_service_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service wool dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0141_service_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service wool dress for domestic wear from prepared wool stock.",
		Action = "making service wool dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service wool dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 460.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("460 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0141_service_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service wool dress for domestic wear (@industrialised_clothing_footwear_uniforms_0141_service_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceLeatherJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0142ServiceJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service leather jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0142_service_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0142_service_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service leather jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0142_service_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service leather jacket for domestic wear from prepared leather stock.",
		Action = "making service leather jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service leather jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 501.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("501 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0142_service_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service leather jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0142_service_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceCottonHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0143ServiceHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service cotton hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0143_service_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0143_service_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0143_service_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service cotton hat for domestic wear from prepared cotton stock.",
		Action = "making service cotton hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service cotton hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 541.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("541 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0143_service_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton hat for domestic wear (@industrialised_clothing_footwear_uniforms_0143_service_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeServiceLinenGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0144ServiceGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some service linen gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0144_service_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0144_service_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some service linen gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0144_service_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some service linen gloves for domestic wear from prepared linen stock.",
		Action = "making some service linen gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some service linen gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 582.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("582 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0144_service_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some service linen gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0144_service_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceWoolScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0145ServiceScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service wool scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0145_service_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0145_service_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service wool scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0145_service_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service wool scarf for domestic wear from prepared wool stock.",
		Action = "making service wool scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service wool scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 623.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("623 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0145_service_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service wool scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0145_service_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceLeatherSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0146ServiceSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service leather skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0146_service_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0146_service_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service leather skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0146_service_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service leather skirt for domestic wear from prepared leather stock.",
		Action = "making service leather skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service leather skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 663.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("663 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0146_service_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service leather skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0146_service_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeServiceCottonSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0147ServiceSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some service cotton socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0147_service_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0147_service_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some service cotton socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0147_service_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some service cotton socks for domestic wear from prepared cotton stock.",
		Action = "making some service cotton socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some service cotton socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 704.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("704 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0147_service_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some service cotton socks for domestic wear (@industrialised_clothing_footwear_uniforms_0147_service_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceLinenVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0148ServiceVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service linen vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0148_service_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0148_service_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service linen vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0148_service_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service linen vest for domestic wear from prepared linen stock.",
		Action = "making service linen vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service linen vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 745.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("745 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0148_service_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service linen vest for domestic wear (@industrialised_clothing_footwear_uniforms_0148_service_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceWoolCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0149ServiceCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service wool coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0149_service_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0149_service_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service wool coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0149_service_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service wool coat for domestic wear from prepared wool stock.",
		Action = "making service wool coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service wool coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 785.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("785 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0149_service_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service wool coat for domestic wear (@industrialised_clothing_footwear_uniforms_0149_service_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeServiceLeatherSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0150ServiceSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make service leather smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0150_service_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0150_service_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service leather smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0150_service_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a service leather smock for domestic wear from prepared leather stock.",
		Action = "making service leather smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress service leather smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 826.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("826 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0150_service_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service leather smock for domestic wear (@industrialised_clothing_footwear_uniforms_0150_service_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionCottonRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0151PrecisionRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision cotton rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0151_precision_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0151_precision_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cotton rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0151_precision_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision cotton rag for domestic wear from prepared cotton stock.",
		Action = "making precision cotton rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision cotton rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 867.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("867 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0151_precision_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cotton rag for domestic wear (@industrialised_clothing_footwear_uniforms_0151_precision_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionLinenApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0152PrecisionApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision linen apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0152_precision_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0152_precision_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision linen apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0152_precision_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision linen apron for domestic wear from prepared linen stock.",
		Action = "making precision linen apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision linen apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 908.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("908 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0152_precision_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision linen apron for domestic wear (@industrialised_clothing_footwear_uniforms_0152_precision_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionWoolShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0153PrecisionShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision wool shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0153_precision_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0153_precision_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision wool shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0153_precision_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision wool shirt for domestic wear from prepared wool stock.",
		Action = "making precision wool shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision wool shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 948.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("948 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0153_precision_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision wool shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0153_precision_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePrecisionLeatherTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0154PrecisionTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some precision leather trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0154_precision_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0154_precision_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some precision leather trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0154_precision_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some precision leather trousers for domestic wear from prepared leather stock.",
		Action = "making some precision leather trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some precision leather trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 989.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("989 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0154_precision_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some precision leather trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0154_precision_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePrecisionCottonBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0155PrecisionBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some precision cotton boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0155_precision_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0155_precision_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some precision cotton boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0155_precision_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some precision cotton boots for domestic wear from prepared cotton stock.",
		Action = "making some precision cotton boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some precision cotton boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1030.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1030 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0155_precision_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some precision cotton boots for domestic wear (@industrialised_clothing_footwear_uniforms_0155_precision_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionLinenDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0156PrecisionDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision linen dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0156_precision_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0156_precision_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision linen dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0156_precision_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision linen dress for domestic wear from prepared linen stock.",
		Action = "making precision linen dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision linen dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1070.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1070 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0156_precision_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision linen dress for domestic wear (@industrialised_clothing_footwear_uniforms_0156_precision_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionWoolJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0157PrecisionJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision wool jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0157_precision_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0157_precision_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision wool jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0157_precision_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision wool jacket for domestic wear from prepared wool stock.",
		Action = "making precision wool jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision wool jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1111.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1111 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0157_precision_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision wool jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0157_precision_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionLeatherHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0158PrecisionHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision leather hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0158_precision_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0158_precision_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision leather hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0158_precision_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision leather hat for domestic wear from prepared leather stock.",
		Action = "making precision leather hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision leather hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1152.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1152 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0158_precision_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision leather hat for domestic wear (@industrialised_clothing_footwear_uniforms_0158_precision_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePrecisionCottonGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0159PrecisionGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some precision cotton gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0159_precision_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0159_precision_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some precision cotton gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0159_precision_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some precision cotton gloves for domestic wear from prepared cotton stock.",
		Action = "making some precision cotton gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some precision cotton gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1192.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1192 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0159_precision_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some precision cotton gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0159_precision_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionLinenScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0160PrecisionScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision linen scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0160_precision_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0160_precision_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision linen scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0160_precision_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision linen scarf for domestic wear from prepared linen stock.",
		Action = "making precision linen scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision linen scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1233.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1233 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0160_precision_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision linen scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0160_precision_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionWoolSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0161PrecisionSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision wool skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0161_precision_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0161_precision_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision wool skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0161_precision_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision wool skirt for domestic wear from prepared wool stock.",
		Action = "making precision wool skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision wool skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1274.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1274 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0161_precision_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision wool skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0161_precision_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
