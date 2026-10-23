#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes23Stock
{
	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePlainCottonPelletForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0007PlainPellet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make plain cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0007_plain_pellet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0007_plain_pellet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0007_plain_pellet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a plain cotton pellet for domestic materials processing from prepared cotton stock.",
		Action = "making plain cotton pellet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress plain cotton pellet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2784.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2784 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0007_plain_pellet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton pellet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0007_plain_pellet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePlainMildSteelOffcutForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0008PlainOffcut() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make plain mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0008_plain_offcut]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0008_plain_offcut"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0008_plain_offcut]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a plain mild steel offcut for domestic materials processing from prepared mild steel stock.",
		Action = "making plain mild steel offcut for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress plain mild steel offcut for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2825.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2825 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0008_plain_offcut", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel offcut for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0008_plain_offcut)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeSomePlainCopperShavingsForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0009PlainShavings() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make some plain copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0009_plain_shavings]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0009_plain_shavings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0009_plain_shavings]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make some plain copper shavings for domestic materials processing from prepared copper stock.",
		Action = "making some plain copper shavings for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress some plain copper shavings for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2866.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2866 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0009_plain_shavings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain copper shavings for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0009_plain_shavings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePlainAluminiumSlagForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0010PlainSlag() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make plain aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0010_plain_slag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0010_plain_slag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0010_plain_slag]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a plain aluminium slag for domestic materials processing from prepared aluminium stock.",
		Action = "making plain aluminium slag for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress plain aluminium slag for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2906.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2906 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0010_plain_slag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain aluminium slag for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0010_plain_slag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeReinforcedPaperScrapForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0011ReinforcedScrap() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make reinforced paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0011_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0011_reinforced_scrap]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a reinforced paper scrap for domestic materials processing from prepared paper stock.",
		Action = "making reinforced paper scrap for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress reinforced paper scrap for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2947.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2947 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper scrap for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeReinforcedCottonIngotForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0012ReinforcedIngot() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make reinforced cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0012_reinforced_ingot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0012_reinforced_ingot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0012_reinforced_ingot]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a reinforced cotton ingot for domestic materials processing from prepared cotton stock.",
		Action = "making reinforced cotton ingot for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton ingot for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2988.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2988 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0012_reinforced_ingot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton ingot for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0012_reinforced_ingot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeReinforcedMildSteelSheetForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0013ReinforcedSheet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make reinforced mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0013_reinforced_sheet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0013_reinforced_sheet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0013_reinforced_sheet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a reinforced mild steel sheet for domestic materials processing from prepared mild steel stock.",
		Action = "making reinforced mild steel sheet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel sheet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3028.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3028 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0013_reinforced_sheet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel sheet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0013_reinforced_sheet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeReinforcedCopperWireForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0014ReinforcedWire() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make reinforced copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0014_reinforced_wire]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0014_reinforced_wire"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0014_reinforced_wire]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a reinforced copper wire for domestic materials processing from prepared copper stock.",
		Action = "making reinforced copper wire for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress reinforced copper wire for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3069.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3069 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0014_reinforced_wire", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper wire for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0014_reinforced_wire)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeReinforcedAluminiumRodForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0015ReinforcedRod() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make reinforced aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0015_reinforced_rod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0015_reinforced_rod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0015_reinforced_rod]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a reinforced aluminium rod for domestic materials processing from prepared aluminium stock.",
		Action = "making reinforced aluminium rod for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress reinforced aluminium rod for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3110.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3110 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0015_reinforced_rod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced aluminium rod for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0015_reinforced_rod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeReinforcedPaperPowderForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0016ReinforcedPowder() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make reinforced paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0016_reinforced_powder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0016_reinforced_powder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0016_reinforced_powder]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a reinforced paper powder for domestic materials processing from prepared paper stock.",
		Action = "making reinforced paper powder for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress reinforced paper powder for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3150.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3150 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0016_reinforced_powder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper powder for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0016_reinforced_powder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeReinforcedCottonPelletForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0017ReinforcedPellet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make reinforced cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0017_reinforced_pellet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0017_reinforced_pellet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0017_reinforced_pellet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a reinforced cotton pellet for domestic materials processing from prepared cotton stock.",
		Action = "making reinforced cotton pellet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton pellet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3191.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3191 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0017_reinforced_pellet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton pellet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0017_reinforced_pellet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeReinforcedMildSteelOffcutForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0018ReinforcedOffcut() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make reinforced mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0018_reinforced_offcut]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0018_reinforced_offcut"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0018_reinforced_offcut]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a reinforced mild steel offcut for domestic materials processing from prepared mild steel stock.",
		Action = "making reinforced mild steel offcut for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel offcut for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3232.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3232 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0018_reinforced_offcut", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel offcut for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0018_reinforced_offcut)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeSomeReinforcedCopperShavingsForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0019ReinforcedShavings() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make some reinforced copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0019_reinforced_shavings]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0019_reinforced_shavings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0019_reinforced_shavings]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make some reinforced copper shavings for domestic materials processing from prepared copper stock.",
		Action = "making some reinforced copper shavings for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress some reinforced copper shavings for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3273.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3273 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0019_reinforced_shavings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced copper shavings for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0019_reinforced_shavings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeReinforcedAluminiumSlagForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0020ReinforcedSlag() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make reinforced aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0020_reinforced_slag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0020_reinforced_slag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0020_reinforced_slag]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a reinforced aluminium slag for domestic materials processing from prepared aluminium stock.",
		Action = "making reinforced aluminium slag for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress reinforced aluminium slag for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3313.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3313 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0020_reinforced_slag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced aluminium slag for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0020_reinforced_slag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeCompactPaperScrapForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0021CompactScrap() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make compact paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0021_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0021_compact_scrap]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a compact paper scrap for domestic materials processing from prepared paper stock.",
		Action = "making compact paper scrap for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress compact paper scrap for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3354.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3354 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper scrap for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeCompactCottonIngotForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0022CompactIngot() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make compact cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0022_compact_ingot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0022_compact_ingot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0022_compact_ingot]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a compact cotton ingot for domestic materials processing from prepared cotton stock.",
		Action = "making compact cotton ingot for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress compact cotton ingot for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3395.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3395 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0022_compact_ingot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton ingot for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0022_compact_ingot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeCompactMildSteelSheetForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0023CompactSheet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make compact mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0023_compact_sheet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0023_compact_sheet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0023_compact_sheet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a compact mild steel sheet for domestic materials processing from prepared mild steel stock.",
		Action = "making compact mild steel sheet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress compact mild steel sheet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3435.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3435 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0023_compact_sheet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel sheet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0023_compact_sheet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeCompactCopperWireForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0024CompactWire() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make compact copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0024_compact_wire]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0024_compact_wire"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0024_compact_wire]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a compact copper wire for domestic materials processing from prepared copper stock.",
		Action = "making compact copper wire for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress compact copper wire for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3476.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3476 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0024_compact_wire", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper wire for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0024_compact_wire)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeCompactAluminiumRodForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0025CompactRod() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make compact aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0025_compact_rod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0025_compact_rod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0025_compact_rod]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a compact aluminium rod for domestic materials processing from prepared aluminium stock.",
		Action = "making compact aluminium rod for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress compact aluminium rod for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3517.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3517 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0025_compact_rod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact aluminium rod for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0025_compact_rod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeCompactPaperPowderForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0026CompactPowder() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make compact paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0026_compact_powder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0026_compact_powder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0026_compact_powder]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a compact paper powder for domestic materials processing from prepared paper stock.",
		Action = "making compact paper powder for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress compact paper powder for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3557.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3557 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0026_compact_powder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper powder for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0026_compact_powder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeCompactCottonPelletForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0027CompactPellet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make compact cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0027_compact_pellet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0027_compact_pellet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0027_compact_pellet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a compact cotton pellet for domestic materials processing from prepared cotton stock.",
		Action = "making compact cotton pellet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress compact cotton pellet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3598.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3598 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0027_compact_pellet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton pellet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0027_compact_pellet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeCompactMildSteelOffcutForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0028CompactOffcut() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make compact mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0028_compact_offcut]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0028_compact_offcut"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0028_compact_offcut]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a compact mild steel offcut for domestic materials processing from prepared mild steel stock.",
		Action = "making compact mild steel offcut for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress compact mild steel offcut for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3639.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3639 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0028_compact_offcut", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel offcut for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0028_compact_offcut)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeSomeCompactCopperShavingsForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0029CompactShavings() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make some compact copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0029_compact_shavings]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0029_compact_shavings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0029_compact_shavings]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make some compact copper shavings for domestic materials processing from prepared copper stock.",
		Action = "making some compact copper shavings for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress some compact copper shavings for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3680.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3680 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0029_compact_shavings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact copper shavings for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0029_compact_shavings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeCompactAluminiumSlagForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0030CompactSlag() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make compact aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0030_compact_slag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0030_compact_slag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0030_compact_slag]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a compact aluminium slag for domestic materials processing from prepared aluminium stock.",
		Action = "making compact aluminium slag for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress compact aluminium slag for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3720.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3720 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0030_compact_slag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact aluminium slag for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0030_compact_slag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeHeavyDutyPaperScrapForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0031HeavyDutyScrap() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make heavy-duty paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0031_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0031_heavy_duty_scrap]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a heavy-duty paper scrap for domestic materials processing from prepared paper stock.",
		Action = "making heavy-duty paper scrap for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper scrap for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3761.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3761 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper scrap for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeHeavyDutyCottonIngotForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0032HeavyDutyIngot() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make heavy-duty cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0032_heavy_duty_ingot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0032_heavy_duty_ingot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0032_heavy_duty_ingot]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a heavy-duty cotton ingot for domestic materials processing from prepared cotton stock.",
		Action = "making heavy-duty cotton ingot for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton ingot for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3802.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3802 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0032_heavy_duty_ingot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton ingot for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0032_heavy_duty_ingot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeHeavyDutyMildSteelSheetForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0033HeavyDutySheet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make heavy-duty mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0033_heavy_duty_sheet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0033_heavy_duty_sheet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0033_heavy_duty_sheet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a heavy-duty mild steel sheet for domestic materials processing from prepared mild steel stock.",
		Action = "making heavy-duty mild steel sheet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel sheet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3842.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3842 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0033_heavy_duty_sheet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel sheet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0033_heavy_duty_sheet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeHeavyDutyCopperWireForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0034HeavyDutyWire() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make heavy-duty copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0034_heavy_duty_wire]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0034_heavy_duty_wire"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0034_heavy_duty_wire]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a heavy-duty copper wire for domestic materials processing from prepared copper stock.",
		Action = "making heavy-duty copper wire for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper wire for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3883.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3883 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0034_heavy_duty_wire", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper wire for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0034_heavy_duty_wire)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeHeavyDutyAluminiumRodForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0035HeavyDutyRod() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make heavy-duty aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0035_heavy_duty_rod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0035_heavy_duty_rod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0035_heavy_duty_rod]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a heavy-duty aluminium rod for domestic materials processing from prepared aluminium stock.",
		Action = "making heavy-duty aluminium rod for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty aluminium rod for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3924.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3924 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0035_heavy_duty_rod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty aluminium rod for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0035_heavy_duty_rod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeHeavyDutyPaperPowderForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0036HeavyDutyPowder() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make heavy-duty paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0036_heavy_duty_powder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0036_heavy_duty_powder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0036_heavy_duty_powder]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a heavy-duty paper powder for domestic materials processing from prepared paper stock.",
		Action = "making heavy-duty paper powder for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper powder for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3964.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3964 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0036_heavy_duty_powder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper powder for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0036_heavy_duty_powder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeHeavyDutyCottonPelletForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0037HeavyDutyPellet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make heavy-duty cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0037_heavy_duty_pellet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0037_heavy_duty_pellet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0037_heavy_duty_pellet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a heavy-duty cotton pellet for domestic materials processing from prepared cotton stock.",
		Action = "making heavy-duty cotton pellet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton pellet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4005.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4005 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0037_heavy_duty_pellet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton pellet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0037_heavy_duty_pellet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeHeavyDutyMildSteelOffcutForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0038HeavyDutyOffcut() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make heavy-duty mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0038_heavy_duty_offcut]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0038_heavy_duty_offcut"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0038_heavy_duty_offcut]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a heavy-duty mild steel offcut for domestic materials processing from prepared mild steel stock.",
		Action = "making heavy-duty mild steel offcut for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel offcut for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4046.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4046 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0038_heavy_duty_offcut", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel offcut for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0038_heavy_duty_offcut)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeSomeHeavyDutyCopperShavingsForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0039HeavyDutyShavings() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make some heavy-duty copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0039_heavy_duty_shavings]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0039_heavy_duty_shavings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0039_heavy_duty_shavings]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make some heavy-duty copper shavings for domestic materials processing from prepared copper stock.",
		Action = "making some heavy-duty copper shavings for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty copper shavings for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4087.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4087 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0039_heavy_duty_shavings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty copper shavings for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0039_heavy_duty_shavings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeHeavyDutyAluminiumSlagForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0040HeavyDutySlag() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make heavy-duty aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0040_heavy_duty_slag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0040_heavy_duty_slag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0040_heavy_duty_slag]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a heavy-duty aluminium slag for domestic materials processing from prepared aluminium stock.",
		Action = "making heavy-duty aluminium slag for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty aluminium slag for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4127.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4127 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0040_heavy_duty_slag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty aluminium slag for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0040_heavy_duty_slag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeFoldingPaperScrapForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0041FoldingScrap() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make folding paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0041_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0041_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0041_folding_scrap]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a folding paper scrap for domestic materials processing from prepared paper stock.",
		Action = "making folding paper scrap for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress folding paper scrap for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4168.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4168 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0041_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper scrap for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0041_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeFoldingCottonIngotForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0042FoldingIngot() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make folding cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0042_folding_ingot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0042_folding_ingot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0042_folding_ingot]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a folding cotton ingot for domestic materials processing from prepared cotton stock.",
		Action = "making folding cotton ingot for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress folding cotton ingot for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4209.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4209 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0042_folding_ingot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton ingot for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0042_folding_ingot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeFoldingMildSteelSheetForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0043FoldingSheet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make folding mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0043_folding_sheet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0043_folding_sheet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0043_folding_sheet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a folding mild steel sheet for domestic materials processing from prepared mild steel stock.",
		Action = "making folding mild steel sheet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress folding mild steel sheet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4249.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4249 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0043_folding_sheet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel sheet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0043_folding_sheet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeFoldingCopperWireForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0044FoldingWire() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make folding copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0044_folding_wire]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0044_folding_wire"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0044_folding_wire]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a folding copper wire for domestic materials processing from prepared copper stock.",
		Action = "making folding copper wire for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress folding copper wire for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4290.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4290 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0044_folding_wire", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper wire for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0044_folding_wire)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeFoldingAluminiumRodForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0045FoldingRod() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make folding aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0045_folding_rod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0045_folding_rod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0045_folding_rod]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a folding aluminium rod for domestic materials processing from prepared aluminium stock.",
		Action = "making folding aluminium rod for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress folding aluminium rod for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4331.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4331 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0045_folding_rod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding aluminium rod for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0045_folding_rod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeFoldingPaperPowderForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0046FoldingPowder() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make folding paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0046_folding_powder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0046_folding_powder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0046_folding_powder]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a folding paper powder for domestic materials processing from prepared paper stock.",
		Action = "making folding paper powder for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress folding paper powder for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4371.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4371 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0046_folding_powder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper powder for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0046_folding_powder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeFoldingCottonPelletForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0047FoldingPellet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make folding cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0047_folding_pellet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0047_folding_pellet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton pellet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0047_folding_pellet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a folding cotton pellet for domestic materials processing from prepared cotton stock.",
		Action = "making folding cotton pellet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress folding cotton pellet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4412.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4412 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0047_folding_pellet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton pellet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0047_folding_pellet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeFoldingMildSteelOffcutForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0048FoldingOffcut() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make folding mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0048_folding_offcut]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0048_folding_offcut"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel offcut for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0048_folding_offcut]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a folding mild steel offcut for domestic materials processing from prepared mild steel stock.",
		Action = "making folding mild steel offcut for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress folding mild steel offcut for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4453.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4453 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0048_folding_offcut", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel offcut for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0048_folding_offcut)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeSomeFoldingCopperShavingsForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0049FoldingShavings() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make some folding copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0049_folding_shavings]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0049_folding_shavings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding copper shavings for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0049_folding_shavings]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make some folding copper shavings for domestic materials processing from prepared copper stock.",
		Action = "making some folding copper shavings for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress some folding copper shavings for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4494.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4494 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0049_folding_shavings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding copper shavings for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0049_folding_shavings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeFoldingAluminiumSlagForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0050FoldingSlag() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make folding aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0050_folding_slag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0050_folding_slag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding aluminium slag for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0050_folding_slag]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a folding aluminium slag for domestic materials processing from prepared aluminium stock.",
		Action = "making folding aluminium slag for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress folding aluminium slag for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4534.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4534 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0050_folding_slag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding aluminium slag for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0050_folding_slag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePortablePaperScrapForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0051PortableScrap() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make portable paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0051_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0051_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0051_portable_scrap]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a portable paper scrap for domestic materials processing from prepared paper stock.",
		Action = "making portable paper scrap for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress portable paper scrap for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4575.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4575 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0051_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper scrap for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0051_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeStackablePaperScrapForDomesticMaterialsProcessingCraftIndustrialRawMaterialsChemicalsWaste0001StackableScrap() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make stackable paper scrap for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0001_stackable_scrap]", "crafts", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0001_stackable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper scrap for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0001_stackable_scrap]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a stackable paper scrap for domestic materials processing from prepared paper stock.",
		Action = "making stackable paper scrap for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress stackable paper scrap for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2540.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2540 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_raw_materials_chemicals_waste_0001_stackable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper scrap for domestic materials processing (@industrial_raw_materials_chemicals_waste_0001_stackable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeStackableCottonIngotForDomesticMaterialsProcessingCraftIndustrialRawMaterialsChemicalsWaste0002StackableIngot() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make stackable cotton ingot for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0002_stackable_ingot]", "crafts", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0002_stackable_ingot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton ingot for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0002_stackable_ingot]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a stackable cotton ingot for domestic materials processing from prepared cotton stock.",
		Action = "making stackable cotton ingot for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress stackable cotton ingot for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2581.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2581 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_raw_materials_chemicals_waste_0002_stackable_ingot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton ingot for domestic materials processing (@industrial_raw_materials_chemicals_waste_0002_stackable_ingot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeStackableMildSteelSheetForDomesticMaterialsProcessingCraftIndustrialRawMaterialsChemicalsWaste0003StackableSheet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make stackable mild steel sheet for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0003_stackable_sheet]", "crafts", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0003_stackable_sheet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable mild steel sheet for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0003_stackable_sheet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a stackable mild steel sheet for domestic materials processing from prepared mild steel stock.",
		Action = "making stackable mild steel sheet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress stackable mild steel sheet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2621.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2621 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_raw_materials_chemicals_waste_0003_stackable_sheet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable mild steel sheet for domestic materials processing (@industrial_raw_materials_chemicals_waste_0003_stackable_sheet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeStackableCopperWireForDomesticMaterialsProcessingCraftIndustrialRawMaterialsChemicalsWaste0004StackableWire() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make stackable copper wire for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0004_stackable_wire]", "crafts", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0004_stackable_wire"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable copper wire for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0004_stackable_wire]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a stackable copper wire for domestic materials processing from prepared copper stock.",
		Action = "making stackable copper wire for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress stackable copper wire for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2662.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2662 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_raw_materials_chemicals_waste_0004_stackable_wire", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable copper wire for domestic materials processing (@industrial_raw_materials_chemicals_waste_0004_stackable_wire)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeStackableAluminiumRodForDomesticMaterialsProcessingCraftIndustrialRawMaterialsChemicalsWaste0005StackableRod() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make stackable aluminium rod for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0005_stackable_rod]", "crafts", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0005_stackable_rod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable aluminium rod for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0005_stackable_rod]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a stackable aluminium rod for domestic materials processing from prepared aluminium stock.",
		Action = "making stackable aluminium rod for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress stackable aluminium rod for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2703.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2703 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_raw_materials_chemicals_waste_0005_stackable_rod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable aluminium rod for domestic materials processing (@industrial_raw_materials_chemicals_waste_0005_stackable_rod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeStackablePaperPowderForDomesticMaterialsProcessingCraftIndustrialRawMaterialsChemicalsWaste0006StackablePowder() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make stackable paper powder for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0006_stackable_powder]", "crafts", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0006_stackable_powder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper powder for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0006_stackable_powder]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a stackable paper powder for domestic materials processing from prepared paper stock.",
		Action = "making stackable paper powder for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress stackable paper powder for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2743.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2743 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_raw_materials_chemicals_waste_0006_stackable_powder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper powder for domestic materials processing (@industrial_raw_materials_chemicals_waste_0006_stackable_powder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeStackableCottonPelletForDomesticMaterialsProcessingCraftIndustrialRawMaterialsChemicalsWaste0007StackablePellet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make stackable cotton pellet for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0007_stackable_pellet]", "crafts", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0007_stackable_pellet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton pellet for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0007_stackable_pellet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a stackable cotton pellet for domestic materials processing from prepared cotton stock.",
		Action = "making stackable cotton pellet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress stackable cotton pellet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2784.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2784 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_raw_materials_chemicals_waste_0007_stackable_pellet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton pellet for domestic materials processing (@industrial_raw_materials_chemicals_waste_0007_stackable_pellet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeStackableMildSteelOffcutForDomesticMaterialsProcessingCraftIndustrialRawMaterialsChemicalsWaste0008StackableOffcut() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make stackable mild steel offcut for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0008_stackable_offcut]", "crafts", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0008_stackable_offcut"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable mild steel offcut for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0008_stackable_offcut]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a stackable mild steel offcut for domestic materials processing from prepared mild steel stock.",
		Action = "making stackable mild steel offcut for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress stackable mild steel offcut for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2825.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2825 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_raw_materials_chemicals_waste_0008_stackable_offcut", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable mild steel offcut for domestic materials processing (@industrial_raw_materials_chemicals_waste_0008_stackable_offcut)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakeSomeStackableCopperShavingsForDomesticMaterialsProcessingCraftIndustrialRawMaterialsChemicalsWaste0009StackableShavings() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make some stackable copper shavings for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0009_stackable_shavings]", "crafts", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0009_stackable_shavings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some stackable copper shavings for domestic materials processing [craft_industrial_raw_materials_chemicals_waste_0009_stackable_shavings]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make some stackable copper shavings for domestic materials processing from prepared copper stock.",
		Action = "making some stackable copper shavings for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress some stackable copper shavings for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2866.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2866 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_raw_materials_chemicals_waste_0009_stackable_shavings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some stackable copper shavings for domestic materials processing (@industrial_raw_materials_chemicals_waste_0009_stackable_shavings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainOakWasteForDomesticObservanceCraftIndustrialisedReligiousInstitutional0001PlainWaste() => new()
	{
		Metadata = new("Religious and institutional/make plain oak waste for domestic observance [craft_industrialised_religious_institutional_0001_plain_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0001_plain_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak waste for domestic observance [craft_industrialised_religious_institutional_0001_plain_waste]",
		Category = "Religious and institutional",
		Blurb = "Make a plain oak waste for domestic observance from prepared oak stock.",
		Action = "making plain oak waste for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain oak waste for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2651.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2651 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0001_plain_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak waste for domestic observance (@industrialised_religious_institutional_0001_plain_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainMildSteelLecternForDomesticObservanceCraftIndustrialisedReligiousInstitutional0002PlainLectern() => new()
	{
		Metadata = new("Religious and institutional/make plain mild steel lectern for domestic observance [craft_industrialised_religious_institutional_0002_plain_lectern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0002_plain_lectern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel lectern for domestic observance [craft_industrialised_religious_institutional_0002_plain_lectern]",
		Category = "Religious and institutional",
		Blurb = "Make a plain mild steel lectern for domestic observance from prepared mild steel stock.",
		Action = "making plain mild steel lectern for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain mild steel lectern for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2692.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2692 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0002_plain_lectern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel lectern for domestic observance (@industrialised_religious_institutional_0002_plain_lectern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainCottonKneelerForDomesticObservanceCraftIndustrialisedReligiousInstitutional0003PlainKneeler() => new()
	{
		Metadata = new("Religious and institutional/make plain cotton kneeler for domestic observance [craft_industrialised_religious_institutional_0003_plain_kneeler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0003_plain_kneeler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton kneeler for domestic observance [craft_industrialised_religious_institutional_0003_plain_kneeler]",
		Category = "Religious and institutional",
		Blurb = "Make a plain cotton kneeler for domestic observance from prepared cotton stock.",
		Action = "making plain cotton kneeler for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain cotton kneeler for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2732.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2732 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0003_plain_kneeler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton kneeler for domestic observance (@industrialised_religious_institutional_0003_plain_kneeler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainCeramicVesselForDomesticObservanceCraftIndustrialisedReligiousInstitutional0004PlainVessel() => new()
	{
		Metadata = new("Religious and institutional/make plain ceramic vessel for domestic observance [craft_industrialised_religious_institutional_0004_plain_vessel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0004_plain_vessel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic vessel for domestic observance [craft_industrialised_religious_institutional_0004_plain_vessel]",
		Category = "Religious and institutional",
		Blurb = "Make a plain ceramic vessel for domestic observance from prepared ceramic stock.",
		Action = "making plain ceramic vessel for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain ceramic vessel for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2773.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2773 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0004_plain_vessel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic vessel for domestic observance (@industrialised_religious_institutional_0004_plain_vessel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainOakScreenForDomesticObservanceCraftIndustrialisedReligiousInstitutional0005PlainScreen() => new()
	{
		Metadata = new("Religious and institutional/make plain oak screen for domestic observance [craft_industrialised_religious_institutional_0005_plain_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0005_plain_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak screen for domestic observance [craft_industrialised_religious_institutional_0005_plain_screen]",
		Category = "Religious and institutional",
		Blurb = "Make a plain oak screen for domestic observance from prepared oak stock.",
		Action = "making plain oak screen for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain oak screen for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2814.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2814 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0005_plain_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak screen for domestic observance (@industrialised_religious_institutional_0005_plain_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainMildSteelCandlestickForDomesticObservanceCraftIndustrialisedReligiousInstitutional0006PlainCandlestick() => new()
	{
		Metadata = new("Religious and institutional/make plain mild steel candlestick for domestic observance [craft_industrialised_religious_institutional_0006_plain_candlestick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0006_plain_candlestick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel candlestick for domestic observance [craft_industrialised_religious_institutional_0006_plain_candlestick]",
		Category = "Religious and institutional",
		Blurb = "Make a plain mild steel candlestick for domestic observance from prepared mild steel stock.",
		Action = "making plain mild steel candlestick for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain mild steel candlestick for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2855.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2855 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0006_plain_candlestick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel candlestick for domestic observance (@industrialised_religious_institutional_0006_plain_candlestick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
