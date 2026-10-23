#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes19Stock
{
	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeCompactLeatherSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0042CompactSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some compact leather socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0042_compact_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0042_compact_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact leather socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0042_compact_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some compact leather socks for domestic wear from prepared leather stock.",
		Action = "making some compact leather socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some compact leather socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1876.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1876 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0042_compact_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact leather socks for domestic wear (@industrialised_clothing_footwear_uniforms_0042_compact_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactCottonVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0043CompactVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact cotton vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0043_compact_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0043_compact_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0043_compact_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact cotton vest for domestic wear from prepared cotton stock.",
		Action = "making compact cotton vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact cotton vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1916.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1916 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0043_compact_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton vest for domestic wear (@industrialised_clothing_footwear_uniforms_0043_compact_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactLinenCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0044CompactCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact linen coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0044_compact_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0044_compact_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact linen coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0044_compact_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact linen coat for domestic wear from prepared linen stock.",
		Action = "making compact linen coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact linen coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1957.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1957 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0044_compact_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact linen coat for domestic wear (@industrialised_clothing_footwear_uniforms_0044_compact_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactWoolSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0045CompactSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact wool smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0045_compact_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0045_compact_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact wool smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0045_compact_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact wool smock for domestic wear from prepared wool stock.",
		Action = "making compact wool smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact wool smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1998.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1998 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0045_compact_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact wool smock for domestic wear (@industrialised_clothing_footwear_uniforms_0045_compact_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyLeatherRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0046HeavyDutyRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty leather rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0046_heavy_duty_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0046_heavy_duty_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0046_heavy_duty_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty leather rag for domestic wear from prepared leather stock.",
		Action = "making heavy-duty leather rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2038.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2038 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0046_heavy_duty_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather rag for domestic wear (@industrialised_clothing_footwear_uniforms_0046_heavy_duty_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyCottonApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0047HeavyDutyApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty cotton apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0047_heavy_duty_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0047_heavy_duty_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0047_heavy_duty_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty cotton apron for domestic wear from prepared cotton stock.",
		Action = "making heavy-duty cotton apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2079.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2079 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0047_heavy_duty_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton apron for domestic wear (@industrialised_clothing_footwear_uniforms_0047_heavy_duty_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyLinenShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0048HeavyDutyShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty linen shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0048_heavy_duty_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0048_heavy_duty_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty linen shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0048_heavy_duty_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty linen shirt for domestic wear from prepared linen stock.",
		Action = "making heavy-duty linen shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty linen shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2120.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2120 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0048_heavy_duty_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty linen shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0048_heavy_duty_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeHeavyDutyWoolTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0049HeavyDutyTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some heavy-duty wool trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0049_heavy_duty_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0049_heavy_duty_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty wool trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0049_heavy_duty_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some heavy-duty wool trousers for domestic wear from prepared wool stock.",
		Action = "making some heavy-duty wool trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty wool trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2160.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2160 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0049_heavy_duty_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty wool trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0049_heavy_duty_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeHeavyDutyLeatherBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0050HeavyDutyBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some heavy-duty leather boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0050_heavy_duty_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0050_heavy_duty_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty leather boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0050_heavy_duty_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some heavy-duty leather boots for domestic wear from prepared leather stock.",
		Action = "making some heavy-duty leather boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty leather boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2201.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2201 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0050_heavy_duty_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty leather boots for domestic wear (@industrialised_clothing_footwear_uniforms_0050_heavy_duty_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyCottonDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0051HeavyDutyDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty cotton dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0051_heavy_duty_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0051_heavy_duty_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0051_heavy_duty_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty cotton dress for domestic wear from prepared cotton stock.",
		Action = "making heavy-duty cotton dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2242.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2242 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0051_heavy_duty_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton dress for domestic wear (@industrialised_clothing_footwear_uniforms_0051_heavy_duty_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyLinenJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0052HeavyDutyJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty linen jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0052_heavy_duty_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0052_heavy_duty_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty linen jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0052_heavy_duty_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty linen jacket for domestic wear from prepared linen stock.",
		Action = "making heavy-duty linen jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty linen jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2282.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2282 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0052_heavy_duty_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty linen jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0052_heavy_duty_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyWoolHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0053HeavyDutyHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty wool hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0053_heavy_duty_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0053_heavy_duty_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty wool hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0053_heavy_duty_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty wool hat for domestic wear from prepared wool stock.",
		Action = "making heavy-duty wool hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty wool hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2323.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2323 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0053_heavy_duty_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty wool hat for domestic wear (@industrialised_clothing_footwear_uniforms_0053_heavy_duty_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeHeavyDutyLeatherGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0054HeavyDutyGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some heavy-duty leather gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0054_heavy_duty_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0054_heavy_duty_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty leather gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0054_heavy_duty_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some heavy-duty leather gloves for domestic wear from prepared leather stock.",
		Action = "making some heavy-duty leather gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty leather gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2364.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2364 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0054_heavy_duty_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty leather gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0054_heavy_duty_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyCottonScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0055HeavyDutyScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty cotton scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0055_heavy_duty_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0055_heavy_duty_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0055_heavy_duty_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty cotton scarf for domestic wear from prepared cotton stock.",
		Action = "making heavy-duty cotton scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2405.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2405 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0055_heavy_duty_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0055_heavy_duty_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyLinenSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0056HeavyDutySkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty linen skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0056_heavy_duty_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0056_heavy_duty_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty linen skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0056_heavy_duty_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty linen skirt for domestic wear from prepared linen stock.",
		Action = "making heavy-duty linen skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty linen skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2445.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2445 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0056_heavy_duty_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty linen skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0056_heavy_duty_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeHeavyDutyWoolSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0057HeavyDutySocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some heavy-duty wool socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0057_heavy_duty_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0057_heavy_duty_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty wool socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0057_heavy_duty_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some heavy-duty wool socks for domestic wear from prepared wool stock.",
		Action = "making some heavy-duty wool socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty wool socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2486.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2486 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0057_heavy_duty_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty wool socks for domestic wear (@industrialised_clothing_footwear_uniforms_0057_heavy_duty_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyLeatherVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0058HeavyDutyVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty leather vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0058_heavy_duty_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0058_heavy_duty_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0058_heavy_duty_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty leather vest for domestic wear from prepared leather stock.",
		Action = "making heavy-duty leather vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2527.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2527 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0058_heavy_duty_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather vest for domestic wear (@industrialised_clothing_footwear_uniforms_0058_heavy_duty_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyCottonCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0059HeavyDutyCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty cotton coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0059_heavy_duty_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0059_heavy_duty_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0059_heavy_duty_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty cotton coat for domestic wear from prepared cotton stock.",
		Action = "making heavy-duty cotton coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2567.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2567 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0059_heavy_duty_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton coat for domestic wear (@industrialised_clothing_footwear_uniforms_0059_heavy_duty_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeHeavyDutyLinenSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0060HeavyDutySmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make heavy-duty linen smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0060_heavy_duty_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0060_heavy_duty_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty linen smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0060_heavy_duty_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a heavy-duty linen smock for domestic wear from prepared linen stock.",
		Action = "making heavy-duty linen smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress heavy-duty linen smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2608.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2608 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0060_heavy_duty_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty linen smock for domestic wear (@industrialised_clothing_footwear_uniforms_0060_heavy_duty_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingWoolRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0061FoldingRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding wool rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0061_folding_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0061_folding_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding wool rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0061_folding_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding wool rag for domestic wear from prepared wool stock.",
		Action = "making folding wool rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding wool rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2649.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2649 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0061_folding_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding wool rag for domestic wear (@industrialised_clothing_footwear_uniforms_0061_folding_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingLeatherApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0062FoldingApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding leather apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0062_folding_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0062_folding_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0062_folding_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding leather apron for domestic wear from prepared leather stock.",
		Action = "making folding leather apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding leather apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2690.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2690 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0062_folding_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather apron for domestic wear (@industrialised_clothing_footwear_uniforms_0062_folding_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingCottonShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0063FoldingShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding cotton shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0063_folding_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0063_folding_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0063_folding_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding cotton shirt for domestic wear from prepared cotton stock.",
		Action = "making folding cotton shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding cotton shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2730.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2730 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0063_folding_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0063_folding_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeFoldingLinenTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0064FoldingTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some folding linen trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0064_folding_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0064_folding_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding linen trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0064_folding_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some folding linen trousers for domestic wear from prepared linen stock.",
		Action = "making some folding linen trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some folding linen trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2771.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2771 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0064_folding_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding linen trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0064_folding_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeFoldingWoolBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0065FoldingBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some folding wool boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0065_folding_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0065_folding_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding wool boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0065_folding_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some folding wool boots for domestic wear from prepared wool stock.",
		Action = "making some folding wool boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some folding wool boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2812.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2812 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0065_folding_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding wool boots for domestic wear (@industrialised_clothing_footwear_uniforms_0065_folding_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingLeatherDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0066FoldingDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding leather dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0066_folding_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0066_folding_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0066_folding_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding leather dress for domestic wear from prepared leather stock.",
		Action = "making folding leather dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding leather dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2852.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2852 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0066_folding_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather dress for domestic wear (@industrialised_clothing_footwear_uniforms_0066_folding_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingCottonJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0067FoldingJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding cotton jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0067_folding_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0067_folding_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0067_folding_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding cotton jacket for domestic wear from prepared cotton stock.",
		Action = "making folding cotton jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding cotton jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2893.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2893 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0067_folding_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0067_folding_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingLinenHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0068FoldingHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding linen hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0068_folding_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0068_folding_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding linen hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0068_folding_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding linen hat for domestic wear from prepared linen stock.",
		Action = "making folding linen hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding linen hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2934.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2934 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0068_folding_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding linen hat for domestic wear (@industrialised_clothing_footwear_uniforms_0068_folding_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeFoldingWoolGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0069FoldingGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some folding wool gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0069_folding_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0069_folding_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding wool gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0069_folding_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some folding wool gloves for domestic wear from prepared wool stock.",
		Action = "making some folding wool gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some folding wool gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2974.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2974 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0069_folding_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding wool gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0069_folding_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingLeatherScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0070FoldingScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding leather scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0070_folding_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0070_folding_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0070_folding_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding leather scarf for domestic wear from prepared leather stock.",
		Action = "making folding leather scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding leather scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3015.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3015 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0070_folding_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0070_folding_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingCottonSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0071FoldingSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding cotton skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0071_folding_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0071_folding_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0071_folding_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding cotton skirt for domestic wear from prepared cotton stock.",
		Action = "making folding cotton skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding cotton skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3056.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3056 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0071_folding_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0071_folding_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeFoldingLinenSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0072FoldingSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some folding linen socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0072_folding_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0072_folding_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding linen socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0072_folding_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some folding linen socks for domestic wear from prepared linen stock.",
		Action = "making some folding linen socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some folding linen socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3097.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3097 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0072_folding_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding linen socks for domestic wear (@industrialised_clothing_footwear_uniforms_0072_folding_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingWoolVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0073FoldingVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding wool vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0073_folding_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0073_folding_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding wool vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0073_folding_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding wool vest for domestic wear from prepared wool stock.",
		Action = "making folding wool vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding wool vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3137.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3137 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0073_folding_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding wool vest for domestic wear (@industrialised_clothing_footwear_uniforms_0073_folding_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingLeatherCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0074FoldingCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding leather coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0074_folding_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0074_folding_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0074_folding_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding leather coat for domestic wear from prepared leather stock.",
		Action = "making folding leather coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding leather coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3178.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3178 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0074_folding_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather coat for domestic wear (@industrialised_clothing_footwear_uniforms_0074_folding_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeFoldingCottonSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0075FoldingSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make folding cotton smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0075_folding_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0075_folding_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0075_folding_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a folding cotton smock for domestic wear from prepared cotton stock.",
		Action = "making folding cotton smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress folding cotton smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3219.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3219 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0075_folding_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton smock for domestic wear (@industrialised_clothing_footwear_uniforms_0075_folding_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableLinenRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0076PortableRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable linen rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0076_portable_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0076_portable_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable linen rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0076_portable_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable linen rag for domestic wear from prepared linen stock.",
		Action = "making portable linen rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable linen rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3259.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3259 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0076_portable_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable linen rag for domestic wear (@industrialised_clothing_footwear_uniforms_0076_portable_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableWoolApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0077PortableApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable wool apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0077_portable_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0077_portable_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable wool apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0077_portable_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable wool apron for domestic wear from prepared wool stock.",
		Action = "making portable wool apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable wool apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3300.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3300 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0077_portable_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable wool apron for domestic wear (@industrialised_clothing_footwear_uniforms_0077_portable_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableLeatherShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0078PortableShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable leather shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0078_portable_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0078_portable_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0078_portable_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable leather shirt for domestic wear from prepared leather stock.",
		Action = "making portable leather shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable leather shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3341.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3341 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0078_portable_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0078_portable_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePortableCottonTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0079PortableTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some portable cotton trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0079_portable_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0079_portable_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some portable cotton trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0079_portable_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some portable cotton trousers for domestic wear from prepared cotton stock.",
		Action = "making some portable cotton trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some portable cotton trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3381.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3381 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0079_portable_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some portable cotton trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0079_portable_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePortableLinenBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0080PortableBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some portable linen boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0080_portable_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0080_portable_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some portable linen boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0080_portable_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some portable linen boots for domestic wear from prepared linen stock.",
		Action = "making some portable linen boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some portable linen boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3422.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3422 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0080_portable_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some portable linen boots for domestic wear (@industrialised_clothing_footwear_uniforms_0080_portable_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableWoolDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0081PortableDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable wool dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0081_portable_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0081_portable_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable wool dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0081_portable_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable wool dress for domestic wear from prepared wool stock.",
		Action = "making portable wool dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable wool dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3463.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3463 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0081_portable_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable wool dress for domestic wear (@industrialised_clothing_footwear_uniforms_0081_portable_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableLeatherJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0082PortableJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable leather jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0082_portable_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0082_portable_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0082_portable_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable leather jacket for domestic wear from prepared leather stock.",
		Action = "making portable leather jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable leather jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3504.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3504 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0082_portable_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0082_portable_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableCottonHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0083PortableHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable cotton hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0083_portable_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0083_portable_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0083_portable_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable cotton hat for domestic wear from prepared cotton stock.",
		Action = "making portable cotton hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable cotton hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3544.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3544 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0083_portable_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton hat for domestic wear (@industrialised_clothing_footwear_uniforms_0083_portable_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePortableLinenGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0084PortableGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some portable linen gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0084_portable_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0084_portable_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some portable linen gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0084_portable_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some portable linen gloves for domestic wear from prepared linen stock.",
		Action = "making some portable linen gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some portable linen gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3585.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3585 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0084_portable_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some portable linen gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0084_portable_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableWoolScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0085PortableScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable wool scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0085_portable_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0085_portable_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable wool scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0085_portable_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable wool scarf for domestic wear from prepared wool stock.",
		Action = "making portable wool scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable wool scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3626.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3626 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0085_portable_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable wool scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0085_portable_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableLeatherSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0086PortableSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable leather skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0086_portable_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0086_portable_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0086_portable_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable leather skirt for domestic wear from prepared leather stock.",
		Action = "making portable leather skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable leather skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3666.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3666 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0086_portable_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0086_portable_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePortableCottonSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0087PortableSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some portable cotton socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0087_portable_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0087_portable_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some portable cotton socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0087_portable_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some portable cotton socks for domestic wear from prepared cotton stock.",
		Action = "making some portable cotton socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some portable cotton socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3707.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3707 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0087_portable_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some portable cotton socks for domestic wear (@industrialised_clothing_footwear_uniforms_0087_portable_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableLinenVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0088PortableVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable linen vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0088_portable_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0088_portable_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable linen vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0088_portable_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable linen vest for domestic wear from prepared linen stock.",
		Action = "making portable linen vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable linen vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3748.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3748 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0088_portable_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable linen vest for domestic wear (@industrialised_clothing_footwear_uniforms_0088_portable_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableWoolCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0089PortableCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable wool coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0089_portable_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0089_portable_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable wool coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0089_portable_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable wool coat for domestic wear from prepared wool stock.",
		Action = "making portable wool coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable wool coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3788.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3788 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0089_portable_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable wool coat for domestic wear (@industrialised_clothing_footwear_uniforms_0089_portable_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePortableLeatherSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0090PortableSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make portable leather smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0090_portable_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0090_portable_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0090_portable_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a portable leather smock for domestic wear from prepared leather stock.",
		Action = "making portable leather smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress portable leather smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3829.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3829 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0090_portable_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather smock for domestic wear (@industrialised_clothing_footwear_uniforms_0090_portable_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopCottonRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0091WorkshopRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop cotton rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0091_workshop_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0091_workshop_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0091_workshop_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop cotton rag for domestic wear from prepared cotton stock.",
		Action = "making workshop cotton rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop cotton rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3870.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3870 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0091_workshop_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton rag for domestic wear (@industrialised_clothing_footwear_uniforms_0091_workshop_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopLinenApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0092WorkshopApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop linen apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0092_workshop_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0092_workshop_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop linen apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0092_workshop_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop linen apron for domestic wear from prepared linen stock.",
		Action = "making workshop linen apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop linen apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3911.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3911 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0092_workshop_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop linen apron for domestic wear (@industrialised_clothing_footwear_uniforms_0092_workshop_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopWoolShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0093WorkshopShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop wool shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0093_workshop_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0093_workshop_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop wool shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0093_workshop_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop wool shirt for domestic wear from prepared wool stock.",
		Action = "making workshop wool shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop wool shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3951.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3951 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0093_workshop_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop wool shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0093_workshop_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeWorkshopLeatherTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0094WorkshopTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some workshop leather trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0094_workshop_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0094_workshop_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some workshop leather trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0094_workshop_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some workshop leather trousers for domestic wear from prepared leather stock.",
		Action = "making some workshop leather trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some workshop leather trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3992.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3992 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0094_workshop_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some workshop leather trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0094_workshop_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeWorkshopCottonBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0095WorkshopBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some workshop cotton boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0095_workshop_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0095_workshop_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some workshop cotton boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0095_workshop_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some workshop cotton boots for domestic wear from prepared cotton stock.",
		Action = "making some workshop cotton boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some workshop cotton boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4033.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4033 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0095_workshop_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some workshop cotton boots for domestic wear (@industrialised_clothing_footwear_uniforms_0095_workshop_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopLinenDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0096WorkshopDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop linen dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0096_workshop_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0096_workshop_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop linen dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0096_workshop_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop linen dress for domestic wear from prepared linen stock.",
		Action = "making workshop linen dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop linen dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4073.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4073 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0096_workshop_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop linen dress for domestic wear (@industrialised_clothing_footwear_uniforms_0096_workshop_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopWoolJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0097WorkshopJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop wool jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0097_workshop_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0097_workshop_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop wool jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0097_workshop_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop wool jacket for domestic wear from prepared wool stock.",
		Action = "making workshop wool jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop wool jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4114.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4114 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0097_workshop_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop wool jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0097_workshop_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopLeatherHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0098WorkshopHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop leather hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0098_workshop_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0098_workshop_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop leather hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0098_workshop_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop leather hat for domestic wear from prepared leather stock.",
		Action = "making workshop leather hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop leather hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4155.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4155 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0098_workshop_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop leather hat for domestic wear (@industrialised_clothing_footwear_uniforms_0098_workshop_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeWorkshopCottonGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0099WorkshopGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some workshop cotton gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0099_workshop_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0099_workshop_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some workshop cotton gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0099_workshop_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some workshop cotton gloves for domestic wear from prepared cotton stock.",
		Action = "making some workshop cotton gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some workshop cotton gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4195.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4195 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0099_workshop_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some workshop cotton gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0099_workshop_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopLinenScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0100WorkshopScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop linen scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0100_workshop_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0100_workshop_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop linen scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0100_workshop_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop linen scarf for domestic wear from prepared linen stock.",
		Action = "making workshop linen scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop linen scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4236.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4236 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0100_workshop_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop linen scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0100_workshop_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeWorkshopWoolSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0101WorkshopSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make workshop wool skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0101_workshop_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0101_workshop_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop wool skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0101_workshop_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a workshop wool skirt for domestic wear from prepared wool stock.",
		Action = "making workshop wool skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress workshop wool skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4277.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4277 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0101_workshop_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop wool skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0101_workshop_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
