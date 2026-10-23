#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes19Stock
{
	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLeatherDressForFieldWearCraftIndustrialClothingFootwearUniforms0006PlainDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain leather dress for field wear [craft_industrial_clothing_footwear_uniforms_0006_plain_dress]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0006_plain_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather dress for field wear [craft_industrial_clothing_footwear_uniforms_0006_plain_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain leather dress for field wear from prepared leather stock.",
		Action = "making plain leather dress for field wear",
		ActiveCraftItemSdesc = "an in-progress plain leather dress for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 410.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("410 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0006_plain_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather dress for field wear (@industrial_clothing_footwear_uniforms_0006_plain_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainCottonJacketForFieldWearCraftIndustrialClothingFootwearUniforms0007PlainJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain cotton jacket for field wear [craft_industrial_clothing_footwear_uniforms_0007_plain_jacket]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0007_plain_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton jacket for field wear [craft_industrial_clothing_footwear_uniforms_0007_plain_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain cotton jacket for field wear from prepared cotton stock.",
		Action = "making plain cotton jacket for field wear",
		ActiveCraftItemSdesc = "an in-progress plain cotton jacket for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 451.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("451 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0007_plain_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton jacket for field wear (@industrial_clothing_footwear_uniforms_0007_plain_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLinenHatForFieldWearCraftIndustrialClothingFootwearUniforms0008PlainHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain linen hat for field wear [craft_industrial_clothing_footwear_uniforms_0008_plain_hat]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0008_plain_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain linen hat for field wear [craft_industrial_clothing_footwear_uniforms_0008_plain_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain linen hat for field wear from prepared linen stock.",
		Action = "making plain linen hat for field wear",
		ActiveCraftItemSdesc = "an in-progress plain linen hat for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 492.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("492 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0008_plain_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain linen hat for field wear (@industrial_clothing_footwear_uniforms_0008_plain_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePlainWoolGlovesForFieldWearCraftIndustrialClothingFootwearUniforms0009PlainGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some plain wool gloves for field wear [craft_industrial_clothing_footwear_uniforms_0009_plain_gloves]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0009_plain_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain wool gloves for field wear [craft_industrial_clothing_footwear_uniforms_0009_plain_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some plain wool gloves for field wear from prepared wool stock.",
		Action = "making some plain wool gloves for field wear",
		ActiveCraftItemSdesc = "an in-progress some plain wool gloves for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 532.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("532 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0009_plain_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain wool gloves for field wear (@industrial_clothing_footwear_uniforms_0009_plain_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLeatherScarfForFieldWearCraftIndustrialClothingFootwearUniforms0010PlainScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain leather scarf for field wear [craft_industrial_clothing_footwear_uniforms_0010_plain_scarf]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0010_plain_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather scarf for field wear [craft_industrial_clothing_footwear_uniforms_0010_plain_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain leather scarf for field wear from prepared leather stock.",
		Action = "making plain leather scarf for field wear",
		ActiveCraftItemSdesc = "an in-progress plain leather scarf for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 573.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("573 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0010_plain_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather scarf for field wear (@industrial_clothing_footwear_uniforms_0010_plain_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainCottonSkirtForFieldWearCraftIndustrialClothingFootwearUniforms0011PlainSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain cotton skirt for field wear [craft_industrial_clothing_footwear_uniforms_0011_plain_skirt]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0011_plain_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton skirt for field wear [craft_industrial_clothing_footwear_uniforms_0011_plain_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain cotton skirt for field wear from prepared cotton stock.",
		Action = "making plain cotton skirt for field wear",
		ActiveCraftItemSdesc = "an in-progress plain cotton skirt for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 614.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("614 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0011_plain_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton skirt for field wear (@industrial_clothing_footwear_uniforms_0011_plain_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePlainLinenSocksForFieldWearCraftIndustrialClothingFootwearUniforms0012PlainSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some plain linen socks for field wear [craft_industrial_clothing_footwear_uniforms_0012_plain_socks]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0012_plain_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain linen socks for field wear [craft_industrial_clothing_footwear_uniforms_0012_plain_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some plain linen socks for field wear from prepared linen stock.",
		Action = "making some plain linen socks for field wear",
		ActiveCraftItemSdesc = "an in-progress some plain linen socks for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 654.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("654 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0012_plain_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain linen socks for field wear (@industrial_clothing_footwear_uniforms_0012_plain_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainWoolVestForFieldWearCraftIndustrialClothingFootwearUniforms0013PlainVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain wool vest for field wear [craft_industrial_clothing_footwear_uniforms_0013_plain_vest]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0013_plain_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain wool vest for field wear [craft_industrial_clothing_footwear_uniforms_0013_plain_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain wool vest for field wear from prepared wool stock.",
		Action = "making plain wool vest for field wear",
		ActiveCraftItemSdesc = "an in-progress plain wool vest for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 695.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("695 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0013_plain_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain wool vest for field wear (@industrial_clothing_footwear_uniforms_0013_plain_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLeatherCoatForFieldWearCraftIndustrialClothingFootwearUniforms0014PlainCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain leather coat for field wear [craft_industrial_clothing_footwear_uniforms_0014_plain_coat]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0014_plain_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather coat for field wear [craft_industrial_clothing_footwear_uniforms_0014_plain_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain leather coat for field wear from prepared leather stock.",
		Action = "making plain leather coat for field wear",
		ActiveCraftItemSdesc = "an in-progress plain leather coat for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 736.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("736 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0014_plain_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather coat for field wear (@industrial_clothing_footwear_uniforms_0014_plain_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainCottonSmockForFieldWearCraftIndustrialClothingFootwearUniforms0015PlainSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain cotton smock for field wear [craft_industrial_clothing_footwear_uniforms_0015_plain_smock]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0015_plain_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton smock for field wear [craft_industrial_clothing_footwear_uniforms_0015_plain_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain cotton smock for field wear from prepared cotton stock.",
		Action = "making plain cotton smock for field wear",
		ActiveCraftItemSdesc = "an in-progress plain cotton smock for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 777.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("777 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0015_plain_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton smock for field wear (@industrial_clothing_footwear_uniforms_0015_plain_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLinenRagForFieldWearCraftIndustrialClothingFootwearUniforms0016ReinforcedRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced linen rag for field wear [craft_industrial_clothing_footwear_uniforms_0016_reinforced_rag]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0016_reinforced_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced linen rag for field wear [craft_industrial_clothing_footwear_uniforms_0016_reinforced_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced linen rag for field wear from prepared linen stock.",
		Action = "making reinforced linen rag for field wear",
		ActiveCraftItemSdesc = "an in-progress reinforced linen rag for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 817.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("817 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0016_reinforced_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced linen rag for field wear (@industrial_clothing_footwear_uniforms_0016_reinforced_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedWoolApronForFieldWearCraftIndustrialClothingFootwearUniforms0017ReinforcedApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced wool apron for field wear [craft_industrial_clothing_footwear_uniforms_0017_reinforced_apron]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0017_reinforced_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced wool apron for field wear [craft_industrial_clothing_footwear_uniforms_0017_reinforced_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced wool apron for field wear from prepared wool stock.",
		Action = "making reinforced wool apron for field wear",
		ActiveCraftItemSdesc = "an in-progress reinforced wool apron for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 858.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("858 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0017_reinforced_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced wool apron for field wear (@industrial_clothing_footwear_uniforms_0017_reinforced_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLeatherShirtForFieldWearCraftIndustrialClothingFootwearUniforms0018ReinforcedShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced leather shirt for field wear [craft_industrial_clothing_footwear_uniforms_0018_reinforced_shirt]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0018_reinforced_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather shirt for field wear [craft_industrial_clothing_footwear_uniforms_0018_reinforced_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced leather shirt for field wear from prepared leather stock.",
		Action = "making reinforced leather shirt for field wear",
		ActiveCraftItemSdesc = "an in-progress reinforced leather shirt for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 899.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("899 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0018_reinforced_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather shirt for field wear (@industrial_clothing_footwear_uniforms_0018_reinforced_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeReinforcedCottonTrousersForFieldWearCraftIndustrialClothingFootwearUniforms0019ReinforcedTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some reinforced cotton trousers for field wear [craft_industrial_clothing_footwear_uniforms_0019_reinforced_trousers]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0019_reinforced_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced cotton trousers for field wear [craft_industrial_clothing_footwear_uniforms_0019_reinforced_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some reinforced cotton trousers for field wear from prepared cotton stock.",
		Action = "making some reinforced cotton trousers for field wear",
		ActiveCraftItemSdesc = "an in-progress some reinforced cotton trousers for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 939.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("939 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0019_reinforced_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced cotton trousers for field wear (@industrial_clothing_footwear_uniforms_0019_reinforced_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeReinforcedLinenBootsForFieldWearCraftIndustrialClothingFootwearUniforms0020ReinforcedBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some reinforced linen boots for field wear [craft_industrial_clothing_footwear_uniforms_0020_reinforced_boots]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0020_reinforced_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced linen boots for field wear [craft_industrial_clothing_footwear_uniforms_0020_reinforced_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some reinforced linen boots for field wear from prepared linen stock.",
		Action = "making some reinforced linen boots for field wear",
		ActiveCraftItemSdesc = "an in-progress some reinforced linen boots for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 980.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("980 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0020_reinforced_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced linen boots for field wear (@industrial_clothing_footwear_uniforms_0020_reinforced_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedWoolDressForFieldWearCraftIndustrialClothingFootwearUniforms0021ReinforcedDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced wool dress for field wear [craft_industrial_clothing_footwear_uniforms_0021_reinforced_dress]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0021_reinforced_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced wool dress for field wear [craft_industrial_clothing_footwear_uniforms_0021_reinforced_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced wool dress for field wear from prepared wool stock.",
		Action = "making reinforced wool dress for field wear",
		ActiveCraftItemSdesc = "an in-progress reinforced wool dress for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1021.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1021 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0021_reinforced_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced wool dress for field wear (@industrial_clothing_footwear_uniforms_0021_reinforced_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLeatherJacketForFieldWearCraftIndustrialClothingFootwearUniforms0022ReinforcedJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced leather jacket for field wear [craft_industrial_clothing_footwear_uniforms_0022_reinforced_jacket]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0022_reinforced_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather jacket for field wear [craft_industrial_clothing_footwear_uniforms_0022_reinforced_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced leather jacket for field wear from prepared leather stock.",
		Action = "making reinforced leather jacket for field wear",
		ActiveCraftItemSdesc = "an in-progress reinforced leather jacket for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1062.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1062 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0022_reinforced_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather jacket for field wear (@industrial_clothing_footwear_uniforms_0022_reinforced_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedCottonHatForFieldWearCraftIndustrialClothingFootwearUniforms0023ReinforcedHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced cotton hat for field wear [craft_industrial_clothing_footwear_uniforms_0023_reinforced_hat]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0023_reinforced_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton hat for field wear [craft_industrial_clothing_footwear_uniforms_0023_reinforced_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced cotton hat for field wear from prepared cotton stock.",
		Action = "making reinforced cotton hat for field wear",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton hat for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1102.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1102 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0023_reinforced_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton hat for field wear (@industrial_clothing_footwear_uniforms_0023_reinforced_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeReinforcedLinenGlovesForFieldWearCraftIndustrialClothingFootwearUniforms0024ReinforcedGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some reinforced linen gloves for field wear [craft_industrial_clothing_footwear_uniforms_0024_reinforced_gloves]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0024_reinforced_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced linen gloves for field wear [craft_industrial_clothing_footwear_uniforms_0024_reinforced_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some reinforced linen gloves for field wear from prepared linen stock.",
		Action = "making some reinforced linen gloves for field wear",
		ActiveCraftItemSdesc = "an in-progress some reinforced linen gloves for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1143.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1143 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0024_reinforced_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced linen gloves for field wear (@industrial_clothing_footwear_uniforms_0024_reinforced_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedWoolScarfForFieldWearCraftIndustrialClothingFootwearUniforms0025ReinforcedScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced wool scarf for field wear [craft_industrial_clothing_footwear_uniforms_0025_reinforced_scarf]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0025_reinforced_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced wool scarf for field wear [craft_industrial_clothing_footwear_uniforms_0025_reinforced_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced wool scarf for field wear from prepared wool stock.",
		Action = "making reinforced wool scarf for field wear",
		ActiveCraftItemSdesc = "an in-progress reinforced wool scarf for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1184.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1184 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0025_reinforced_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced wool scarf for field wear (@industrial_clothing_footwear_uniforms_0025_reinforced_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLeatherSkirtForFieldWearCraftIndustrialClothingFootwearUniforms0026ReinforcedSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced leather skirt for field wear [craft_industrial_clothing_footwear_uniforms_0026_reinforced_skirt]", "crafts", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0026_reinforced_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather skirt for field wear [craft_industrial_clothing_footwear_uniforms_0026_reinforced_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced leather skirt for field wear from prepared leather stock.",
		Action = "making reinforced leather skirt for field wear",
		ActiveCraftItemSdesc = "an in-progress reinforced leather skirt for field wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1224.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1224 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_clothing_footwear_uniforms_0026_reinforced_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather skirt for field wear (@industrial_clothing_footwear_uniforms_0026_reinforced_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakePlainMildSteelLinerForDomesticProtectionCraftIndustrialisedPpeBallistic0001PlainLiner() => new()
	{
		Metadata = new("PPE and ballistic protection/make plain mild steel liner for domestic protection [craft_industrialised_ppe_ballistic_0001_plain_liner]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0001_plain_liner"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel liner for domestic protection [craft_industrialised_ppe_ballistic_0001_plain_liner]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a plain mild steel liner for domestic protection from prepared mild steel stock.",
		Action = "making plain mild steel liner for domestic protection",
		ActiveCraftItemSdesc = "an in-progress plain mild steel liner for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 318.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("318 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0001_plain_liner", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel liner for domestic protection (@industrialised_ppe_ballistic_0001_plain_liner)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakePlainLeatherApronForDomesticProtectionCraftIndustrialisedPpeBallistic0002PlainApron() => new()
	{
		Metadata = new("PPE and ballistic protection/make plain leather apron for domestic protection [craft_industrialised_ppe_ballistic_0002_plain_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0002_plain_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather apron for domestic protection [craft_industrialised_ppe_ballistic_0002_plain_apron]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a plain leather apron for domestic protection from prepared leather stock.",
		Action = "making plain leather apron for domestic protection",
		ActiveCraftItemSdesc = "an in-progress plain leather apron for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 359.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("359 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0002_plain_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather apron for domestic protection (@industrialised_ppe_ballistic_0002_plain_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomePlainCottonGlovesForDomesticProtectionCraftIndustrialisedPpeBallistic0003PlainGloves() => new()
	{
		Metadata = new("PPE and ballistic protection/make some plain cotton gloves for domestic protection [craft_industrialised_ppe_ballistic_0003_plain_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0003_plain_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain cotton gloves for domestic protection [craft_industrialised_ppe_ballistic_0003_plain_gloves]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some plain cotton gloves for domestic protection from prepared cotton stock.",
		Action = "making some plain cotton gloves for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some plain cotton gloves for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 399.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("399 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0003_plain_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain cotton gloves for domestic protection (@industrialised_ppe_ballistic_0003_plain_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomePlainWoolBootsForDomesticProtectionCraftIndustrialisedPpeBallistic0004PlainBoots() => new()
	{
		Metadata = new("PPE and ballistic protection/make some plain wool boots for domestic protection [craft_industrialised_ppe_ballistic_0004_plain_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0004_plain_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain wool boots for domestic protection [craft_industrialised_ppe_ballistic_0004_plain_boots]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some plain wool boots for domestic protection from prepared wool stock.",
		Action = "making some plain wool boots for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some plain wool boots for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 440.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("440 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0004_plain_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain wool boots for domestic protection (@industrialised_ppe_ballistic_0004_plain_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakePlainMildSteelHelmetForDomesticProtectionCraftIndustrialisedPpeBallistic0005PlainHelmet() => new()
	{
		Metadata = new("PPE and ballistic protection/make plain mild steel helmet for domestic protection [craft_industrialised_ppe_ballistic_0005_plain_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0005_plain_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel helmet for domestic protection [craft_industrialised_ppe_ballistic_0005_plain_helmet]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a plain mild steel helmet for domestic protection from prepared mild steel stock.",
		Action = "making plain mild steel helmet for domestic protection",
		ActiveCraftItemSdesc = "an in-progress plain mild steel helmet for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 481.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("481 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0005_plain_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel helmet for domestic protection (@industrialised_ppe_ballistic_0005_plain_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomePlainLeatherGaitersForDomesticProtectionCraftIndustrialisedPpeBallistic0006PlainGaiters() => new()
	{
		Metadata = new("PPE and ballistic protection/make some plain leather gaiters for domestic protection [craft_industrialised_ppe_ballistic_0006_plain_gaiters]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0006_plain_gaiters"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain leather gaiters for domestic protection [craft_industrialised_ppe_ballistic_0006_plain_gaiters]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some plain leather gaiters for domestic protection from prepared leather stock.",
		Action = "making some plain leather gaiters for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some plain leather gaiters for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 521.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("521 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0006_plain_gaiters", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain leather gaiters for domestic protection (@industrialised_ppe_ballistic_0006_plain_gaiters)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakePlainCottonMaskForDomesticProtectionCraftIndustrialisedPpeBallistic0007PlainMask() => new()
	{
		Metadata = new("PPE and ballistic protection/make plain cotton mask for domestic protection [craft_industrialised_ppe_ballistic_0007_plain_mask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0007_plain_mask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton mask for domestic protection [craft_industrialised_ppe_ballistic_0007_plain_mask]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a plain cotton mask for domestic protection from prepared cotton stock.",
		Action = "making plain cotton mask for domestic protection",
		ActiveCraftItemSdesc = "an in-progress plain cotton mask for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 562.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("562 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0007_plain_mask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton mask for domestic protection (@industrialised_ppe_ballistic_0007_plain_mask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakePlainWoolVisorForDomesticProtectionCraftIndustrialisedPpeBallistic0008PlainVisor() => new()
	{
		Metadata = new("PPE and ballistic protection/make plain wool visor for domestic protection [craft_industrialised_ppe_ballistic_0008_plain_visor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0008_plain_visor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain wool visor for domestic protection [craft_industrialised_ppe_ballistic_0008_plain_visor]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a plain wool visor for domestic protection from prepared wool stock.",
		Action = "making plain wool visor for domestic protection",
		ActiveCraftItemSdesc = "an in-progress plain wool visor for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 603.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("603 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0008_plain_visor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain wool visor for domestic protection (@industrialised_ppe_ballistic_0008_plain_visor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakePlainMildSteelVestForDomesticProtectionCraftIndustrialisedPpeBallistic0009PlainVest() => new()
	{
		Metadata = new("PPE and ballistic protection/make plain mild steel vest for domestic protection [craft_industrialised_ppe_ballistic_0009_plain_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0009_plain_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel vest for domestic protection [craft_industrialised_ppe_ballistic_0009_plain_vest]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a plain mild steel vest for domestic protection from prepared mild steel stock.",
		Action = "making plain mild steel vest for domestic protection",
		ActiveCraftItemSdesc = "an in-progress plain mild steel vest for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 644.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("644 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0009_plain_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel vest for domestic protection (@industrialised_ppe_ballistic_0009_plain_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakePlainLeatherCoatForDomesticProtectionCraftIndustrialisedPpeBallistic0010PlainCoat() => new()
	{
		Metadata = new("PPE and ballistic protection/make plain leather coat for domestic protection [craft_industrialised_ppe_ballistic_0010_plain_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0010_plain_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather coat for domestic protection [craft_industrialised_ppe_ballistic_0010_plain_coat]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a plain leather coat for domestic protection from prepared leather stock.",
		Action = "making plain leather coat for domestic protection",
		ActiveCraftItemSdesc = "an in-progress plain leather coat for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 684.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("684 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0010_plain_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather coat for domestic protection (@industrialised_ppe_ballistic_0010_plain_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeReinforcedCottonLinerForDomesticProtectionCraftIndustrialisedPpeBallistic0011ReinforcedLiner() => new()
	{
		Metadata = new("PPE and ballistic protection/make reinforced cotton liner for domestic protection [craft_industrialised_ppe_ballistic_0011_reinforced_liner]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0011_reinforced_liner"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton liner for domestic protection [craft_industrialised_ppe_ballistic_0011_reinforced_liner]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a reinforced cotton liner for domestic protection from prepared cotton stock.",
		Action = "making reinforced cotton liner for domestic protection",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton liner for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 725.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("725 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0011_reinforced_liner", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton liner for domestic protection (@industrialised_ppe_ballistic_0011_reinforced_liner)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeReinforcedWoolApronForDomesticProtectionCraftIndustrialisedPpeBallistic0012ReinforcedApron() => new()
	{
		Metadata = new("PPE and ballistic protection/make reinforced wool apron for domestic protection [craft_industrialised_ppe_ballistic_0012_reinforced_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0012_reinforced_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced wool apron for domestic protection [craft_industrialised_ppe_ballistic_0012_reinforced_apron]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a reinforced wool apron for domestic protection from prepared wool stock.",
		Action = "making reinforced wool apron for domestic protection",
		ActiveCraftItemSdesc = "an in-progress reinforced wool apron for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 766.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("766 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0012_reinforced_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced wool apron for domestic protection (@industrialised_ppe_ballistic_0012_reinforced_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeReinforcedMildSteelGlovesForDomesticProtectionCraftIndustrialisedPpeBallistic0013ReinforcedGloves() => new()
	{
		Metadata = new("PPE and ballistic protection/make some reinforced mild steel gloves for domestic protection [craft_industrialised_ppe_ballistic_0013_reinforced_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0013_reinforced_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced mild steel gloves for domestic protection [craft_industrialised_ppe_ballistic_0013_reinforced_gloves]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some reinforced mild steel gloves for domestic protection from prepared mild steel stock.",
		Action = "making some reinforced mild steel gloves for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some reinforced mild steel gloves for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 806.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("806 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0013_reinforced_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced mild steel gloves for domestic protection (@industrialised_ppe_ballistic_0013_reinforced_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeReinforcedLeatherBootsForDomesticProtectionCraftIndustrialisedPpeBallistic0014ReinforcedBoots() => new()
	{
		Metadata = new("PPE and ballistic protection/make some reinforced leather boots for domestic protection [craft_industrialised_ppe_ballistic_0014_reinforced_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0014_reinforced_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced leather boots for domestic protection [craft_industrialised_ppe_ballistic_0014_reinforced_boots]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some reinforced leather boots for domestic protection from prepared leather stock.",
		Action = "making some reinforced leather boots for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some reinforced leather boots for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 847.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("847 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0014_reinforced_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced leather boots for domestic protection (@industrialised_ppe_ballistic_0014_reinforced_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeReinforcedCottonHelmetForDomesticProtectionCraftIndustrialisedPpeBallistic0015ReinforcedHelmet() => new()
	{
		Metadata = new("PPE and ballistic protection/make reinforced cotton helmet for domestic protection [craft_industrialised_ppe_ballistic_0015_reinforced_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0015_reinforced_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton helmet for domestic protection [craft_industrialised_ppe_ballistic_0015_reinforced_helmet]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a reinforced cotton helmet for domestic protection from prepared cotton stock.",
		Action = "making reinforced cotton helmet for domestic protection",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton helmet for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 888.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("888 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0015_reinforced_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton helmet for domestic protection (@industrialised_ppe_ballistic_0015_reinforced_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeReinforcedWoolGaitersForDomesticProtectionCraftIndustrialisedPpeBallistic0016ReinforcedGaiters() => new()
	{
		Metadata = new("PPE and ballistic protection/make some reinforced wool gaiters for domestic protection [craft_industrialised_ppe_ballistic_0016_reinforced_gaiters]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0016_reinforced_gaiters"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced wool gaiters for domestic protection [craft_industrialised_ppe_ballistic_0016_reinforced_gaiters]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some reinforced wool gaiters for domestic protection from prepared wool stock.",
		Action = "making some reinforced wool gaiters for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some reinforced wool gaiters for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 928.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("928 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0016_reinforced_gaiters", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced wool gaiters for domestic protection (@industrialised_ppe_ballistic_0016_reinforced_gaiters)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeReinforcedMildSteelMaskForDomesticProtectionCraftIndustrialisedPpeBallistic0017ReinforcedMask() => new()
	{
		Metadata = new("PPE and ballistic protection/make reinforced mild steel mask for domestic protection [craft_industrialised_ppe_ballistic_0017_reinforced_mask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0017_reinforced_mask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel mask for domestic protection [craft_industrialised_ppe_ballistic_0017_reinforced_mask]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a reinforced mild steel mask for domestic protection from prepared mild steel stock.",
		Action = "making reinforced mild steel mask for domestic protection",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel mask for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 969.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("969 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0017_reinforced_mask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel mask for domestic protection (@industrialised_ppe_ballistic_0017_reinforced_mask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeReinforcedLeatherVisorForDomesticProtectionCraftIndustrialisedPpeBallistic0018ReinforcedVisor() => new()
	{
		Metadata = new("PPE and ballistic protection/make reinforced leather visor for domestic protection [craft_industrialised_ppe_ballistic_0018_reinforced_visor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0018_reinforced_visor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather visor for domestic protection [craft_industrialised_ppe_ballistic_0018_reinforced_visor]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a reinforced leather visor for domestic protection from prepared leather stock.",
		Action = "making reinforced leather visor for domestic protection",
		ActiveCraftItemSdesc = "an in-progress reinforced leather visor for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1010.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1010 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0018_reinforced_visor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather visor for domestic protection (@industrialised_ppe_ballistic_0018_reinforced_visor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeReinforcedCottonVestForDomesticProtectionCraftIndustrialisedPpeBallistic0019ReinforcedVest() => new()
	{
		Metadata = new("PPE and ballistic protection/make reinforced cotton vest for domestic protection [craft_industrialised_ppe_ballistic_0019_reinforced_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0019_reinforced_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton vest for domestic protection [craft_industrialised_ppe_ballistic_0019_reinforced_vest]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a reinforced cotton vest for domestic protection from prepared cotton stock.",
		Action = "making reinforced cotton vest for domestic protection",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton vest for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1050.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1050 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0019_reinforced_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton vest for domestic protection (@industrialised_ppe_ballistic_0019_reinforced_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeReinforcedWoolCoatForDomesticProtectionCraftIndustrialisedPpeBallistic0020ReinforcedCoat() => new()
	{
		Metadata = new("PPE and ballistic protection/make reinforced wool coat for domestic protection [craft_industrialised_ppe_ballistic_0020_reinforced_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0020_reinforced_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced wool coat for domestic protection [craft_industrialised_ppe_ballistic_0020_reinforced_coat]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a reinforced wool coat for domestic protection from prepared wool stock.",
		Action = "making reinforced wool coat for domestic protection",
		ActiveCraftItemSdesc = "an in-progress reinforced wool coat for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1091.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1091 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0020_reinforced_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced wool coat for domestic protection (@industrialised_ppe_ballistic_0020_reinforced_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeCompactMildSteelLinerForDomesticProtectionCraftIndustrialisedPpeBallistic0021CompactLiner() => new()
	{
		Metadata = new("PPE and ballistic protection/make compact mild steel liner for domestic protection [craft_industrialised_ppe_ballistic_0021_compact_liner]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0021_compact_liner"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel liner for domestic protection [craft_industrialised_ppe_ballistic_0021_compact_liner]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a compact mild steel liner for domestic protection from prepared mild steel stock.",
		Action = "making compact mild steel liner for domestic protection",
		ActiveCraftItemSdesc = "an in-progress compact mild steel liner for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1132.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1132 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0021_compact_liner", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel liner for domestic protection (@industrialised_ppe_ballistic_0021_compact_liner)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeCompactLeatherApronForDomesticProtectionCraftIndustrialisedPpeBallistic0022CompactApron() => new()
	{
		Metadata = new("PPE and ballistic protection/make compact leather apron for domestic protection [craft_industrialised_ppe_ballistic_0022_compact_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0022_compact_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather apron for domestic protection [craft_industrialised_ppe_ballistic_0022_compact_apron]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a compact leather apron for domestic protection from prepared leather stock.",
		Action = "making compact leather apron for domestic protection",
		ActiveCraftItemSdesc = "an in-progress compact leather apron for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1173.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1173 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0022_compact_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather apron for domestic protection (@industrialised_ppe_ballistic_0022_compact_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeCompactCottonGlovesForDomesticProtectionCraftIndustrialisedPpeBallistic0023CompactGloves() => new()
	{
		Metadata = new("PPE and ballistic protection/make some compact cotton gloves for domestic protection [craft_industrialised_ppe_ballistic_0023_compact_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0023_compact_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact cotton gloves for domestic protection [craft_industrialised_ppe_ballistic_0023_compact_gloves]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some compact cotton gloves for domestic protection from prepared cotton stock.",
		Action = "making some compact cotton gloves for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some compact cotton gloves for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1213.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1213 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0023_compact_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact cotton gloves for domestic protection (@industrialised_ppe_ballistic_0023_compact_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeCompactWoolBootsForDomesticProtectionCraftIndustrialisedPpeBallistic0024CompactBoots() => new()
	{
		Metadata = new("PPE and ballistic protection/make some compact wool boots for domestic protection [craft_industrialised_ppe_ballistic_0024_compact_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0024_compact_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact wool boots for domestic protection [craft_industrialised_ppe_ballistic_0024_compact_boots]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some compact wool boots for domestic protection from prepared wool stock.",
		Action = "making some compact wool boots for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some compact wool boots for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1254.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1254 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0024_compact_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact wool boots for domestic protection (@industrialised_ppe_ballistic_0024_compact_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeCompactMildSteelHelmetForDomesticProtectionCraftIndustrialisedPpeBallistic0025CompactHelmet() => new()
	{
		Metadata = new("PPE and ballistic protection/make compact mild steel helmet for domestic protection [craft_industrialised_ppe_ballistic_0025_compact_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0025_compact_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel helmet for domestic protection [craft_industrialised_ppe_ballistic_0025_compact_helmet]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a compact mild steel helmet for domestic protection from prepared mild steel stock.",
		Action = "making compact mild steel helmet for domestic protection",
		ActiveCraftItemSdesc = "an in-progress compact mild steel helmet for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1295.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1295 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0025_compact_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel helmet for domestic protection (@industrialised_ppe_ballistic_0025_compact_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeCompactLeatherGaitersForDomesticProtectionCraftIndustrialisedPpeBallistic0026CompactGaiters() => new()
	{
		Metadata = new("PPE and ballistic protection/make some compact leather gaiters for domestic protection [craft_industrialised_ppe_ballistic_0026_compact_gaiters]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0026_compact_gaiters"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact leather gaiters for domestic protection [craft_industrialised_ppe_ballistic_0026_compact_gaiters]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some compact leather gaiters for domestic protection from prepared leather stock.",
		Action = "making some compact leather gaiters for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some compact leather gaiters for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1335.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1335 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0026_compact_gaiters", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact leather gaiters for domestic protection (@industrialised_ppe_ballistic_0026_compact_gaiters)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeCompactCottonMaskForDomesticProtectionCraftIndustrialisedPpeBallistic0027CompactMask() => new()
	{
		Metadata = new("PPE and ballistic protection/make compact cotton mask for domestic protection [craft_industrialised_ppe_ballistic_0027_compact_mask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0027_compact_mask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton mask for domestic protection [craft_industrialised_ppe_ballistic_0027_compact_mask]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a compact cotton mask for domestic protection from prepared cotton stock.",
		Action = "making compact cotton mask for domestic protection",
		ActiveCraftItemSdesc = "an in-progress compact cotton mask for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1376.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1376 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0027_compact_mask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton mask for domestic protection (@industrialised_ppe_ballistic_0027_compact_mask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeCompactWoolVisorForDomesticProtectionCraftIndustrialisedPpeBallistic0028CompactVisor() => new()
	{
		Metadata = new("PPE and ballistic protection/make compact wool visor for domestic protection [craft_industrialised_ppe_ballistic_0028_compact_visor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0028_compact_visor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact wool visor for domestic protection [craft_industrialised_ppe_ballistic_0028_compact_visor]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a compact wool visor for domestic protection from prepared wool stock.",
		Action = "making compact wool visor for domestic protection",
		ActiveCraftItemSdesc = "an in-progress compact wool visor for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1417.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1417 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0028_compact_visor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact wool visor for domestic protection (@industrialised_ppe_ballistic_0028_compact_visor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
