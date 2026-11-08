#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes19Stock
{
	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePrecisionLeatherSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0162PrecisionSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some precision leather socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0162_precision_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0162_precision_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some precision leather socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0162_precision_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some precision leather socks for domestic wear from prepared leather stock.",
		Action = "making some precision leather socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some precision leather socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1314.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1314 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0162_precision_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some precision leather socks for domestic wear (@industrialised_clothing_footwear_uniforms_0162_precision_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionCottonVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0163PrecisionVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision cotton vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0163_precision_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0163_precision_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cotton vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0163_precision_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision cotton vest for domestic wear from prepared cotton stock.",
		Action = "making precision cotton vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision cotton vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1355.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1355 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0163_precision_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cotton vest for domestic wear (@industrialised_clothing_footwear_uniforms_0163_precision_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionLinenCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0164PrecisionCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision linen coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0164_precision_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0164_precision_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision linen coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0164_precision_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision linen coat for domestic wear from prepared linen stock.",
		Action = "making precision linen coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision linen coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1396.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1396 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0164_precision_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision linen coat for domestic wear (@industrialised_clothing_footwear_uniforms_0164_precision_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePrecisionWoolSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0165PrecisionSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make precision wool smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0165_precision_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0165_precision_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision wool smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0165_precision_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a precision wool smock for domestic wear from prepared wool stock.",
		Action = "making precision wool smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress precision wool smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1437.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1437 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0165_precision_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision wool smock for domestic wear (@industrialised_clothing_footwear_uniforms_0165_precision_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyLeatherRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0166EconomyRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy leather rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0166_economy_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0166_economy_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy leather rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0166_economy_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy leather rag for domestic wear from prepared leather stock.",
		Action = "making economy leather rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy leather rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1477.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1477 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0166_economy_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy leather rag for domestic wear (@industrialised_clothing_footwear_uniforms_0166_economy_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyCottonApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0167EconomyApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy cotton apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0167_economy_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0167_economy_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cotton apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0167_economy_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy cotton apron for domestic wear from prepared cotton stock.",
		Action = "making economy cotton apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy cotton apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1518.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1518 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0167_economy_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cotton apron for domestic wear (@industrialised_clothing_footwear_uniforms_0167_economy_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyLinenShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0168EconomyShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy linen shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0168_economy_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0168_economy_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy linen shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0168_economy_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy linen shirt for domestic wear from prepared linen stock.",
		Action = "making economy linen shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy linen shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1559.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1559 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0168_economy_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy linen shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0168_economy_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeEconomyWoolTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0169EconomyTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some economy wool trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0169_economy_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0169_economy_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some economy wool trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0169_economy_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some economy wool trousers for domestic wear from prepared wool stock.",
		Action = "making some economy wool trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some economy wool trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1599.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1599 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0169_economy_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some economy wool trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0169_economy_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeEconomyLeatherBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0170EconomyBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some economy leather boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0170_economy_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0170_economy_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some economy leather boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0170_economy_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some economy leather boots for domestic wear from prepared leather stock.",
		Action = "making some economy leather boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some economy leather boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1640.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1640 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0170_economy_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some economy leather boots for domestic wear (@industrialised_clothing_footwear_uniforms_0170_economy_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyCottonDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0171EconomyDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy cotton dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0171_economy_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0171_economy_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cotton dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0171_economy_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy cotton dress for domestic wear from prepared cotton stock.",
		Action = "making economy cotton dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy cotton dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1681.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1681 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0171_economy_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cotton dress for domestic wear (@industrialised_clothing_footwear_uniforms_0171_economy_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyLinenJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0172EconomyJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy linen jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0172_economy_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0172_economy_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy linen jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0172_economy_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy linen jacket for domestic wear from prepared linen stock.",
		Action = "making economy linen jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy linen jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1722.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1722 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0172_economy_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy linen jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0172_economy_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyWoolHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0173EconomyHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy wool hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0173_economy_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0173_economy_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy wool hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0173_economy_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy wool hat for domestic wear from prepared wool stock.",
		Action = "making economy wool hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy wool hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1762.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1762 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0173_economy_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy wool hat for domestic wear (@industrialised_clothing_footwear_uniforms_0173_economy_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeEconomyLeatherGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0174EconomyGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some economy leather gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0174_economy_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0174_economy_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some economy leather gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0174_economy_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some economy leather gloves for domestic wear from prepared leather stock.",
		Action = "making some economy leather gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some economy leather gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1803.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1803 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0174_economy_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some economy leather gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0174_economy_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyCottonScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0175EconomyScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy cotton scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0175_economy_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0175_economy_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cotton scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0175_economy_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy cotton scarf for domestic wear from prepared cotton stock.",
		Action = "making economy cotton scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy cotton scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1844.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1844 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0175_economy_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cotton scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0175_economy_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyLinenSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0176EconomySkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy linen skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0176_economy_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0176_economy_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy linen skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0176_economy_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy linen skirt for domestic wear from prepared linen stock.",
		Action = "making economy linen skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy linen skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1884.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1884 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0176_economy_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy linen skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0176_economy_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeEconomyWoolSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0177EconomySocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some economy wool socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0177_economy_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0177_economy_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some economy wool socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0177_economy_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some economy wool socks for domestic wear from prepared wool stock.",
		Action = "making some economy wool socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some economy wool socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1925.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1925 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0177_economy_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some economy wool socks for domestic wear (@industrialised_clothing_footwear_uniforms_0177_economy_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyLeatherVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0178EconomyVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy leather vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0178_economy_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0178_economy_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy leather vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0178_economy_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy leather vest for domestic wear from prepared leather stock.",
		Action = "making economy leather vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy leather vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1966.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1966 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0178_economy_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy leather vest for domestic wear (@industrialised_clothing_footwear_uniforms_0178_economy_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyCottonCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0179EconomyCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy cotton coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0179_economy_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0179_economy_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cotton coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0179_economy_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy cotton coat for domestic wear from prepared cotton stock.",
		Action = "making economy cotton coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy cotton coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2006.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2006 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0179_economy_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cotton coat for domestic wear (@industrialised_clothing_footwear_uniforms_0179_economy_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeEconomyLinenSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0180EconomySmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make economy linen smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0180_economy_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0180_economy_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy linen smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0180_economy_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make an economy linen smock for domestic wear from prepared linen stock.",
		Action = "making economy linen smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress economy linen smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2047.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2047 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0180_economy_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy linen smock for domestic wear (@industrialised_clothing_footwear_uniforms_0180_economy_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedWoolRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0181LinedRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined wool rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0181_lined_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0181_lined_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined wool rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0181_lined_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined wool rag for domestic wear from prepared wool stock.",
		Action = "making lined wool rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined wool rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2088.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2088 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0181_lined_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined wool rag for domestic wear (@industrialised_clothing_footwear_uniforms_0181_lined_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedLeatherApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0182LinedApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined leather apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0182_lined_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0182_lined_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined leather apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0182_lined_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined leather apron for domestic wear from prepared leather stock.",
		Action = "making lined leather apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined leather apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2128.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2128 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0182_lined_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined leather apron for domestic wear (@industrialised_clothing_footwear_uniforms_0182_lined_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedCottonShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0183LinedShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined cotton shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0183_lined_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0183_lined_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cotton shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0183_lined_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined cotton shirt for domestic wear from prepared cotton stock.",
		Action = "making lined cotton shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined cotton shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2169.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2169 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0183_lined_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cotton shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0183_lined_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeLinedLinenTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0184LinedTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some lined linen trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0184_lined_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0184_lined_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some lined linen trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0184_lined_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some lined linen trousers for domestic wear from prepared linen stock.",
		Action = "making some lined linen trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some lined linen trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2210.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2210 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0184_lined_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some lined linen trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0184_lined_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeLinedWoolBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0185LinedBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some lined wool boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0185_lined_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0185_lined_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some lined wool boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0185_lined_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some lined wool boots for domestic wear from prepared wool stock.",
		Action = "making some lined wool boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some lined wool boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2251.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2251 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0185_lined_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some lined wool boots for domestic wear (@industrialised_clothing_footwear_uniforms_0185_lined_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedLeatherDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0186LinedDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined leather dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0186_lined_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0186_lined_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined leather dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0186_lined_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined leather dress for domestic wear from prepared leather stock.",
		Action = "making lined leather dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined leather dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2291.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2291 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0186_lined_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined leather dress for domestic wear (@industrialised_clothing_footwear_uniforms_0186_lined_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedCottonJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0187LinedJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined cotton jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0187_lined_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0187_lined_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cotton jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0187_lined_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined cotton jacket for domestic wear from prepared cotton stock.",
		Action = "making lined cotton jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined cotton jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2332.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2332 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0187_lined_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cotton jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0187_lined_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedLinenHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0188LinedHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined linen hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0188_lined_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0188_lined_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined linen hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0188_lined_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined linen hat for domestic wear from prepared linen stock.",
		Action = "making lined linen hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined linen hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2373.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2373 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0188_lined_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined linen hat for domestic wear (@industrialised_clothing_footwear_uniforms_0188_lined_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeLinedWoolGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0189LinedGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some lined wool gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0189_lined_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0189_lined_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some lined wool gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0189_lined_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some lined wool gloves for domestic wear from prepared wool stock.",
		Action = "making some lined wool gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some lined wool gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2413.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2413 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0189_lined_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some lined wool gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0189_lined_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedLeatherScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0190LinedScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined leather scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0190_lined_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0190_lined_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined leather scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0190_lined_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined leather scarf for domestic wear from prepared leather stock.",
		Action = "making lined leather scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined leather scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2454.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2454 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0190_lined_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined leather scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0190_lined_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedCottonSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0191LinedSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined cotton skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0191_lined_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0191_lined_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cotton skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0191_lined_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined cotton skirt for domestic wear from prepared cotton stock.",
		Action = "making lined cotton skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined cotton skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2495.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2495 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0191_lined_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cotton skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0191_lined_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeLinedLinenSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0192LinedSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some lined linen socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0192_lined_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0192_lined_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some lined linen socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0192_lined_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some lined linen socks for domestic wear from prepared linen stock.",
		Action = "making some lined linen socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some lined linen socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2536.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2536 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0192_lined_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some lined linen socks for domestic wear (@industrialised_clothing_footwear_uniforms_0192_lined_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedWoolVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0193LinedVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined wool vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0193_lined_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0193_lined_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined wool vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0193_lined_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined wool vest for domestic wear from prepared wool stock.",
		Action = "making lined wool vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined wool vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2576.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2576 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0193_lined_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined wool vest for domestic wear (@industrialised_clothing_footwear_uniforms_0193_lined_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedLeatherCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0194LinedCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined leather coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0194_lined_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0194_lined_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined leather coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0194_lined_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined leather coat for domestic wear from prepared leather stock.",
		Action = "making lined leather coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined leather coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2617.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2617 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0194_lined_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined leather coat for domestic wear (@industrialised_clothing_footwear_uniforms_0194_lined_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeLinedCottonSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0195LinedSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make lined cotton smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0195_lined_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0195_lined_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cotton smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0195_lined_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a lined cotton smock for domestic wear from prepared cotton stock.",
		Action = "making lined cotton smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress lined cotton smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2658.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2658 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0195_lined_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cotton smock for domestic wear (@industrialised_clothing_footwear_uniforms_0195_lined_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedLinenRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0196VentilatedRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated linen rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0196_ventilated_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0196_ventilated_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated linen rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0196_ventilated_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated linen rag for domestic wear from prepared linen stock.",
		Action = "making ventilated linen rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated linen rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2698.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2698 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0196_ventilated_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated linen rag for domestic wear (@industrialised_clothing_footwear_uniforms_0196_ventilated_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedWoolApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0197VentilatedApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated wool apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0197_ventilated_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0197_ventilated_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated wool apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0197_ventilated_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated wool apron for domestic wear from prepared wool stock.",
		Action = "making ventilated wool apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated wool apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2739.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2739 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0197_ventilated_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated wool apron for domestic wear (@industrialised_clothing_footwear_uniforms_0197_ventilated_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedLeatherShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0198VentilatedShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated leather shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0198_ventilated_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0198_ventilated_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated leather shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0198_ventilated_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated leather shirt for domestic wear from prepared leather stock.",
		Action = "making ventilated leather shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated leather shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2780.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2780 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0198_ventilated_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated leather shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0198_ventilated_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeVentilatedCottonTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0199VentilatedTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some ventilated cotton trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0199_ventilated_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0199_ventilated_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some ventilated cotton trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0199_ventilated_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some ventilated cotton trousers for domestic wear from prepared cotton stock.",
		Action = "making some ventilated cotton trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some ventilated cotton trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2820.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2820 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0199_ventilated_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some ventilated cotton trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0199_ventilated_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeVentilatedLinenBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0200VentilatedBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some ventilated linen boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0200_ventilated_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0200_ventilated_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some ventilated linen boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0200_ventilated_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some ventilated linen boots for domestic wear from prepared linen stock.",
		Action = "making some ventilated linen boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some ventilated linen boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2861.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2861 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0200_ventilated_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some ventilated linen boots for domestic wear (@industrialised_clothing_footwear_uniforms_0200_ventilated_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedWoolDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0201VentilatedDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated wool dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0201_ventilated_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0201_ventilated_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated wool dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0201_ventilated_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated wool dress for domestic wear from prepared wool stock.",
		Action = "making ventilated wool dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated wool dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2902.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2902 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0201_ventilated_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated wool dress for domestic wear (@industrialised_clothing_footwear_uniforms_0201_ventilated_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedLeatherJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0202VentilatedJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated leather jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0202_ventilated_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0202_ventilated_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated leather jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0202_ventilated_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated leather jacket for domestic wear from prepared leather stock.",
		Action = "making ventilated leather jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated leather jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2943.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2943 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0202_ventilated_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated leather jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0202_ventilated_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedCottonHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0203VentilatedHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated cotton hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0203_ventilated_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0203_ventilated_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated cotton hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0203_ventilated_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated cotton hat for domestic wear from prepared cotton stock.",
		Action = "making ventilated cotton hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated cotton hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2983.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2983 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0203_ventilated_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated cotton hat for domestic wear (@industrialised_clothing_footwear_uniforms_0203_ventilated_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeVentilatedLinenGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0204VentilatedGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some ventilated linen gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0204_ventilated_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0204_ventilated_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some ventilated linen gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0204_ventilated_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some ventilated linen gloves for domestic wear from prepared linen stock.",
		Action = "making some ventilated linen gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some ventilated linen gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3024.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3024 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0204_ventilated_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some ventilated linen gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0204_ventilated_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedWoolScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0205VentilatedScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated wool scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0205_ventilated_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0205_ventilated_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated wool scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0205_ventilated_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated wool scarf for domestic wear from prepared wool stock.",
		Action = "making ventilated wool scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated wool scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3065.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3065 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0205_ventilated_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated wool scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0205_ventilated_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedLeatherSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0206VentilatedSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated leather skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0206_ventilated_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0206_ventilated_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated leather skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0206_ventilated_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated leather skirt for domestic wear from prepared leather stock.",
		Action = "making ventilated leather skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated leather skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3105.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3105 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0206_ventilated_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated leather skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0206_ventilated_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeVentilatedCottonSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0207VentilatedSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some ventilated cotton socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0207_ventilated_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0207_ventilated_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some ventilated cotton socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0207_ventilated_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some ventilated cotton socks for domestic wear from prepared cotton stock.",
		Action = "making some ventilated cotton socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some ventilated cotton socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3146.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3146 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0207_ventilated_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some ventilated cotton socks for domestic wear (@industrialised_clothing_footwear_uniforms_0207_ventilated_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedLinenVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0208VentilatedVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated linen vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0208_ventilated_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0208_ventilated_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated linen vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0208_ventilated_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated linen vest for domestic wear from prepared linen stock.",
		Action = "making ventilated linen vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated linen vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3187.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3187 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0208_ventilated_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated linen vest for domestic wear (@industrialised_clothing_footwear_uniforms_0208_ventilated_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedWoolCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0209VentilatedCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated wool coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0209_ventilated_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0209_ventilated_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated wool coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0209_ventilated_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated wool coat for domestic wear from prepared wool stock.",
		Action = "making ventilated wool coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated wool coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3227.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3227 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0209_ventilated_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated wool coat for domestic wear (@industrialised_clothing_footwear_uniforms_0209_ventilated_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeVentilatedLeatherSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0210VentilatedSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make ventilated leather smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0210_ventilated_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0210_ventilated_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated leather smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0210_ventilated_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a ventilated leather smock for domestic wear from prepared leather stock.",
		Action = "making ventilated leather smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress ventilated leather smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3268.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3268 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0210_ventilated_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated leather smock for domestic wear (@industrialised_clothing_footwear_uniforms_0210_ventilated_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeStackableCottonRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0211StackableRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make stackable cotton rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0211_stackable_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0211_stackable_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0211_stackable_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a stackable cotton rag for domestic wear from prepared cotton stock.",
		Action = "making stackable cotton rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress stackable cotton rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3309.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3309 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0211_stackable_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton rag for domestic wear (@industrialised_clothing_footwear_uniforms_0211_stackable_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeStackableLinenApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0212StackableApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make stackable linen apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0212_stackable_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0212_stackable_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable linen apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0212_stackable_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a stackable linen apron for domestic wear from prepared linen stock.",
		Action = "making stackable linen apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress stackable linen apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3350.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3350 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0212_stackable_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable linen apron for domestic wear (@industrialised_clothing_footwear_uniforms_0212_stackable_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeStackableWoolShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0213StackableShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make stackable wool shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0213_stackable_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0213_stackable_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable wool shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0213_stackable_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a stackable wool shirt for domestic wear from prepared wool stock.",
		Action = "making stackable wool shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress stackable wool shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3390.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3390 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0213_stackable_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable wool shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0213_stackable_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeStackableLeatherTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0214StackableTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some stackable leather trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0214_stackable_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0214_stackable_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some stackable leather trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0214_stackable_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some stackable leather trousers for domestic wear from prepared leather stock.",
		Action = "making some stackable leather trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some stackable leather trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3431.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3431 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0214_stackable_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some stackable leather trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0214_stackable_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeStackableCottonBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0215StackableBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some stackable cotton boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0215_stackable_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0215_stackable_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some stackable cotton boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0215_stackable_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some stackable cotton boots for domestic wear from prepared cotton stock.",
		Action = "making some stackable cotton boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some stackable cotton boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3472.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3472 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0215_stackable_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some stackable cotton boots for domestic wear (@industrialised_clothing_footwear_uniforms_0215_stackable_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeStackableLinenDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0216StackableDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make stackable linen dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0216_stackable_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0216_stackable_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable linen dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0216_stackable_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a stackable linen dress for domestic wear from prepared linen stock.",
		Action = "making stackable linen dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress stackable linen dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3512.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3512 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0216_stackable_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable linen dress for domestic wear (@industrialised_clothing_footwear_uniforms_0216_stackable_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainWoolRagForFieldWearCraftIndustrialClothingFootwearUniforms0001PlainRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain wool rag for field wear [craft_industrial_clothing_footwear_uniforms_0001_plain_rag]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain wool rag for field wear [craft_industrial_clothing_footwear_uniforms_0001_plain_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain wool rag for field wear from prepared wool stock.",
		Action = "making plain wool rag for field wear",
		ActiveCraftItemSdesc = "an in-progress plain wool rag for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 207.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("207 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0001_plain_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain wool rag for field wear (@industrial_clothing_footwear_uniforms_0001_plain_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLeatherApronForFieldWearCraftIndustrialClothingFootwearUniforms0002PlainApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain leather apron for field wear [craft_industrial_clothing_footwear_uniforms_0002_plain_apron]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0002_plain_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather apron for field wear [craft_industrial_clothing_footwear_uniforms_0002_plain_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain leather apron for field wear from prepared leather stock.",
		Action = "making plain leather apron for field wear",
		ActiveCraftItemSdesc = "an in-progress plain leather apron for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 248.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("248 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0002_plain_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather apron for field wear (@industrial_clothing_footwear_uniforms_0002_plain_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainCottonShirtForFieldWearCraftIndustrialClothingFootwearUniforms0003PlainShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain cotton shirt for field wear [craft_industrial_clothing_footwear_uniforms_0003_plain_shirt]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0003_plain_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton shirt for field wear [craft_industrial_clothing_footwear_uniforms_0003_plain_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain cotton shirt for field wear from prepared cotton stock.",
		Action = "making plain cotton shirt for field wear",
		ActiveCraftItemSdesc = "an in-progress plain cotton shirt for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 288.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("288 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0003_plain_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton shirt for field wear (@industrial_clothing_footwear_uniforms_0003_plain_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePlainLinenTrousersForFieldWearCraftIndustrialClothingFootwearUniforms0004PlainTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some plain linen trousers for field wear [craft_industrial_clothing_footwear_uniforms_0004_plain_trousers]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0004_plain_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain linen trousers for field wear [craft_industrial_clothing_footwear_uniforms_0004_plain_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some plain linen trousers for field wear from prepared linen stock.",
		Action = "making some plain linen trousers for field wear",
		ActiveCraftItemSdesc = "an in-progress some plain linen trousers for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 329.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("329 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0004_plain_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain linen trousers for field wear (@industrial_clothing_footwear_uniforms_0004_plain_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePlainWoolBootsForFieldWearCraftIndustrialClothingFootwearUniforms0005PlainBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some plain wool boots for field wear [craft_industrial_clothing_footwear_uniforms_0005_plain_boots]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0005_plain_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain wool boots for field wear [craft_industrial_clothing_footwear_uniforms_0005_plain_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some plain wool boots for field wear from prepared wool stock.",
		Action = "making some plain wool boots for field wear",
		ActiveCraftItemSdesc = "an in-progress some plain wool boots for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 370.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("370 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0005_plain_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain wool boots for field wear (@industrial_clothing_footwear_uniforms_0005_plain_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
