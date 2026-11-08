#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeEconomyCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0134EconomyHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make economy cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0134_economy_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0134_economy_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0134_economy_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an economy cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making economy cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress economy cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1397.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1397 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0134_economy_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0134_economy_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeEconomyOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0135EconomyWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make economy oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0135_economy_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0135_economy_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0135_economy_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an economy oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making economy oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress economy oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1438.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1438 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0135_economy_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0135_economy_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeEconomyToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0136EconomyDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make economy tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0136_economy_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0136_economy_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0136_economy_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an economy tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making economy tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress economy tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1478.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1478 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0136_economy_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0136_economy_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeEconomyMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0137EconomyLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make economy mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0137_economy_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0137_economy_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0137_economy_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an economy mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making economy mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress economy mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1519.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1519 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0137_economy_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0137_economy_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeEconomyCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0138EconomyVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make economy cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0138_economy_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0138_economy_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0138_economy_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an economy cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making economy cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress economy cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1560.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1560 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0138_economy_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0138_economy_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeEconomyOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0139EconomyPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make economy oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0139_economy_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0139_economy_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0139_economy_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an economy oak plane for domestic maintenance from prepared oak stock.",
		Action = "making economy oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress economy oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1601.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1601 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0139_economy_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0139_economy_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeEconomyToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0140EconomySaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make economy tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0140_economy_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0140_economy_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0140_economy_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an economy tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making economy tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress economy tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1641.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1641 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0140_economy_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0140_economy_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeEconomyMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0141EconomyPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make economy mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0141_economy_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0141_economy_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0141_economy_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an economy mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making economy mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress economy mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1682.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1682 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0141_economy_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0141_economy_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeLinedOakPlaneForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0001LinedPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make lined oak plane for workshop maintenance [craft_industrial_tools_repair_machinery_0001_lined_plane]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0001_lined_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined oak plane for workshop maintenance [craft_industrial_tools_repair_machinery_0001_lined_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a lined oak plane for workshop maintenance from prepared oak stock.",
		Action = "making lined oak plane for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress lined oak plane for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1429.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1429 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0001_lined_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined oak plane for workshop maintenance (@industrial_tools_repair_machinery_0001_lined_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeLinedToolSteelSawForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0002LinedSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make lined tool steel saw for workshop maintenance [craft_industrial_tools_repair_machinery_0002_lined_saw]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0002_lined_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined tool steel saw for workshop maintenance [craft_industrial_tools_repair_machinery_0002_lined_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a lined tool steel saw for workshop maintenance from prepared tool steel stock.",
		Action = "making lined tool steel saw for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress lined tool steel saw for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1470.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1470 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0002_lined_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined tool steel saw for workshop maintenance (@industrial_tools_repair_machinery_0002_lined_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeLinedMildSteelPressForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0003LinedPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make lined mild steel press for workshop maintenance [craft_industrial_tools_repair_machinery_0003_lined_press]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0003_lined_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined mild steel press for workshop maintenance [craft_industrial_tools_repair_machinery_0003_lined_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a lined mild steel press for workshop maintenance from prepared mild steel stock.",
		Action = "making lined mild steel press for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress lined mild steel press for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1510.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1510 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0003_lined_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined mild steel press for workshop maintenance (@industrial_tools_repair_machinery_0003_lined_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeLinedCastIronGaugeForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0004LinedGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make lined cast iron gauge for workshop maintenance [craft_industrial_tools_repair_machinery_0004_lined_gauge]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0004_lined_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cast iron gauge for workshop maintenance [craft_industrial_tools_repair_machinery_0004_lined_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a lined cast iron gauge for workshop maintenance from prepared cast iron stock.",
		Action = "making lined cast iron gauge for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress lined cast iron gauge for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1551.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1551 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0004_lined_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cast iron gauge for workshop maintenance (@industrial_tools_repair_machinery_0004_lined_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeLinedOakAnvilForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0005LinedAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make lined oak anvil for workshop maintenance [craft_industrial_tools_repair_machinery_0005_lined_anvil]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0005_lined_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined oak anvil for workshop maintenance [craft_industrial_tools_repair_machinery_0005_lined_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a lined oak anvil for workshop maintenance from prepared oak stock.",
		Action = "making lined oak anvil for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress lined oak anvil for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1592.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1592 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0005_lined_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined oak anvil for workshop maintenance (@industrial_tools_repair_machinery_0005_lined_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeLinedToolSteelGrinderForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0006LinedGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make lined tool steel grinder for workshop maintenance [craft_industrial_tools_repair_machinery_0006_lined_grinder]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0006_lined_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined tool steel grinder for workshop maintenance [craft_industrial_tools_repair_machinery_0006_lined_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a lined tool steel grinder for workshop maintenance from prepared tool steel stock.",
		Action = "making lined tool steel grinder for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress lined tool steel grinder for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1632.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1632 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0006_lined_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined tool steel grinder for workshop maintenance (@industrial_tools_repair_machinery_0006_lined_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedMildSteelScrapForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0007VentilatedScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated mild steel scrap for workshop maintenance [craft_industrial_tools_repair_machinery_0007_ventilated_scrap]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0007_ventilated_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated mild steel scrap for workshop maintenance [craft_industrial_tools_repair_machinery_0007_ventilated_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated mild steel scrap for workshop maintenance from prepared mild steel stock.",
		Action = "making ventilated mild steel scrap for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated mild steel scrap for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1673.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1673 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0007_ventilated_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated mild steel scrap for workshop maintenance (@industrial_tools_repair_machinery_0007_ventilated_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedCastIronHammerForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0008VentilatedHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated cast iron hammer for workshop maintenance [craft_industrial_tools_repair_machinery_0008_ventilated_hammer]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0008_ventilated_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated cast iron hammer for workshop maintenance [craft_industrial_tools_repair_machinery_0008_ventilated_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated cast iron hammer for workshop maintenance from prepared cast iron stock.",
		Action = "making ventilated cast iron hammer for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated cast iron hammer for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1714.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1714 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0008_ventilated_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated cast iron hammer for workshop maintenance (@industrial_tools_repair_machinery_0008_ventilated_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedOakWrenchForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0009VentilatedWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated oak wrench for workshop maintenance [craft_industrial_tools_repair_machinery_0009_ventilated_wrench]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0009_ventilated_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated oak wrench for workshop maintenance [craft_industrial_tools_repair_machinery_0009_ventilated_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated oak wrench for workshop maintenance from prepared oak stock.",
		Action = "making ventilated oak wrench for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated oak wrench for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1755.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1755 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0009_ventilated_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated oak wrench for workshop maintenance (@industrial_tools_repair_machinery_0009_ventilated_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedToolSteelDrillForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0010VentilatedDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated tool steel drill for workshop maintenance [craft_industrial_tools_repair_machinery_0010_ventilated_drill]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0010_ventilated_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated tool steel drill for workshop maintenance [craft_industrial_tools_repair_machinery_0010_ventilated_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated tool steel drill for workshop maintenance from prepared tool steel stock.",
		Action = "making ventilated tool steel drill for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated tool steel drill for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1795.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1795 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0010_ventilated_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated tool steel drill for workshop maintenance (@industrial_tools_repair_machinery_0010_ventilated_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedMildSteelLatheForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0011VentilatedLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated mild steel lathe for workshop maintenance [craft_industrial_tools_repair_machinery_0011_ventilated_lathe]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0011_ventilated_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated mild steel lathe for workshop maintenance [craft_industrial_tools_repair_machinery_0011_ventilated_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated mild steel lathe for workshop maintenance from prepared mild steel stock.",
		Action = "making ventilated mild steel lathe for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated mild steel lathe for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1836.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1836 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0011_ventilated_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated mild steel lathe for workshop maintenance (@industrial_tools_repair_machinery_0011_ventilated_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedCastIronViseForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0012VentilatedVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated cast iron vise for workshop maintenance [craft_industrial_tools_repair_machinery_0012_ventilated_vise]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0012_ventilated_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated cast iron vise for workshop maintenance [craft_industrial_tools_repair_machinery_0012_ventilated_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated cast iron vise for workshop maintenance from prepared cast iron stock.",
		Action = "making ventilated cast iron vise for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated cast iron vise for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1877.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1877 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0012_ventilated_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated cast iron vise for workshop maintenance (@industrial_tools_repair_machinery_0012_ventilated_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedOakPlaneForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0013VentilatedPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated oak plane for workshop maintenance [craft_industrial_tools_repair_machinery_0013_ventilated_plane]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0013_ventilated_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated oak plane for workshop maintenance [craft_industrial_tools_repair_machinery_0013_ventilated_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated oak plane for workshop maintenance from prepared oak stock.",
		Action = "making ventilated oak plane for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated oak plane for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1917.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1917 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0013_ventilated_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated oak plane for workshop maintenance (@industrial_tools_repair_machinery_0013_ventilated_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedToolSteelSawForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0014VentilatedSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated tool steel saw for workshop maintenance [craft_industrial_tools_repair_machinery_0014_ventilated_saw]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0014_ventilated_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated tool steel saw for workshop maintenance [craft_industrial_tools_repair_machinery_0014_ventilated_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated tool steel saw for workshop maintenance from prepared tool steel stock.",
		Action = "making ventilated tool steel saw for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated tool steel saw for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1958.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1958 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0014_ventilated_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated tool steel saw for workshop maintenance (@industrial_tools_repair_machinery_0014_ventilated_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedMildSteelPressForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0015VentilatedPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated mild steel press for workshop maintenance [craft_industrial_tools_repair_machinery_0015_ventilated_press]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0015_ventilated_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated mild steel press for workshop maintenance [craft_industrial_tools_repair_machinery_0015_ventilated_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated mild steel press for workshop maintenance from prepared mild steel stock.",
		Action = "making ventilated mild steel press for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated mild steel press for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1999.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1999 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0015_ventilated_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated mild steel press for workshop maintenance (@industrial_tools_repair_machinery_0015_ventilated_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedCastIronGaugeForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0016VentilatedGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated cast iron gauge for workshop maintenance [craft_industrial_tools_repair_machinery_0016_ventilated_gauge]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0016_ventilated_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated cast iron gauge for workshop maintenance [craft_industrial_tools_repair_machinery_0016_ventilated_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated cast iron gauge for workshop maintenance from prepared cast iron stock.",
		Action = "making ventilated cast iron gauge for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated cast iron gauge for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2039.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2039 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0016_ventilated_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated cast iron gauge for workshop maintenance (@industrial_tools_repair_machinery_0016_ventilated_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedOakAnvilForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0017VentilatedAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated oak anvil for workshop maintenance [craft_industrial_tools_repair_machinery_0017_ventilated_anvil]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0017_ventilated_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated oak anvil for workshop maintenance [craft_industrial_tools_repair_machinery_0017_ventilated_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated oak anvil for workshop maintenance from prepared oak stock.",
		Action = "making ventilated oak anvil for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated oak anvil for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2080.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2080 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0017_ventilated_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated oak anvil for workshop maintenance (@industrial_tools_repair_machinery_0017_ventilated_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeVentilatedToolSteelGrinderForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0018VentilatedGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make ventilated tool steel grinder for workshop maintenance [craft_industrial_tools_repair_machinery_0018_ventilated_grinder]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0018_ventilated_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated tool steel grinder for workshop maintenance [craft_industrial_tools_repair_machinery_0018_ventilated_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a ventilated tool steel grinder for workshop maintenance from prepared tool steel stock.",
		Action = "making ventilated tool steel grinder for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress ventilated tool steel grinder for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2121.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2121 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0018_ventilated_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated tool steel grinder for workshop maintenance (@industrial_tools_repair_machinery_0018_ventilated_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeStackableMildSteelScrapForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0019StackableScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make stackable mild steel scrap for workshop maintenance [craft_industrial_tools_repair_machinery_0019_stackable_scrap]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0019_stackable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable mild steel scrap for workshop maintenance [craft_industrial_tools_repair_machinery_0019_stackable_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a stackable mild steel scrap for workshop maintenance from prepared mild steel stock.",
		Action = "making stackable mild steel scrap for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress stackable mild steel scrap for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2162.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2162 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0019_stackable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable mild steel scrap for workshop maintenance (@industrial_tools_repair_machinery_0019_stackable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeStackableCastIronHammerForWorkshopMaintenanceCraftIndustrialToolsRepairMachinery0020StackableHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make stackable cast iron hammer for workshop maintenance [craft_industrial_tools_repair_machinery_0020_stackable_hammer]", "crafts", ["industrial"], ["item:industrial_tools_repair_machinery_0020_stackable_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cast iron hammer for workshop maintenance [craft_industrial_tools_repair_machinery_0020_stackable_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a stackable cast iron hammer for workshop maintenance from prepared cast iron stock.",
		Action = "making stackable cast iron hammer for workshop maintenance",
		ActiveCraftItemSdesc = "an in-progress stackable cast iron hammer for workshop maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2202.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2202 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_tools_repair_machinery_0020_stackable_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cast iron hammer for workshop maintenance (@industrial_tools_repair_machinery_0020_stackable_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainOakRubbleForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0001PlainRubble() => new()
	{
		Metadata = new("Construction and utilities/make plain oak rubble for domestic building work [craft_industrialised_construction_utilities_0001_plain_rubble]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0001_plain_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak rubble for domestic building work [craft_industrialised_construction_utilities_0001_plain_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make a plain oak rubble for domestic building work from prepared oak stock.",
		Action = "making plain oak rubble for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain oak rubble for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1540.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1540 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0001_plain_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak rubble for domestic building work (@industrialised_construction_utilities_0001_plain_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainCeramicBrickForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0002PlainBrick() => new()
	{
		Metadata = new("Construction and utilities/make plain ceramic brick for domestic building work [craft_industrialised_construction_utilities_0002_plain_brick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0002_plain_brick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic brick for domestic building work [craft_industrialised_construction_utilities_0002_plain_brick]",
		Category = "Construction and utilities",
		Blurb = "Make a plain ceramic brick for domestic building work from prepared ceramic stock.",
		Action = "making plain ceramic brick for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain ceramic brick for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1581.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1581 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0002_plain_brick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic brick for domestic building work (@industrialised_construction_utilities_0002_plain_brick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainMildSteelTileForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0003PlainTile() => new()
	{
		Metadata = new("Construction and utilities/make plain mild steel tile for domestic building work [craft_industrialised_construction_utilities_0003_plain_tile]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0003_plain_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel tile for domestic building work [craft_industrialised_construction_utilities_0003_plain_tile]",
		Category = "Construction and utilities",
		Blurb = "Make a plain mild steel tile for domestic building work from prepared mild steel stock.",
		Action = "making plain mild steel tile for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain mild steel tile for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1621.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1621 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0003_plain_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel tile for domestic building work (@industrialised_construction_utilities_0003_plain_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainCastIronPipeForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0004PlainPipe() => new()
	{
		Metadata = new("Construction and utilities/make plain cast iron pipe for domestic building work [craft_industrialised_construction_utilities_0004_plain_pipe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0004_plain_pipe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron pipe for domestic building work [craft_industrialised_construction_utilities_0004_plain_pipe]",
		Category = "Construction and utilities",
		Blurb = "Make a plain cast iron pipe for domestic building work from prepared cast iron stock.",
		Action = "making plain cast iron pipe for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain cast iron pipe for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1662.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1662 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0004_plain_pipe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron pipe for domestic building work (@industrialised_construction_utilities_0004_plain_pipe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainOakValveForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0005PlainValve() => new()
	{
		Metadata = new("Construction and utilities/make plain oak valve for domestic building work [craft_industrialised_construction_utilities_0005_plain_valve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0005_plain_valve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak valve for domestic building work [craft_industrialised_construction_utilities_0005_plain_valve]",
		Category = "Construction and utilities",
		Blurb = "Make a plain oak valve for domestic building work from prepared oak stock.",
		Action = "making plain oak valve for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain oak valve for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1703.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1703 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0005_plain_valve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak valve for domestic building work (@industrialised_construction_utilities_0005_plain_valve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainCeramicFittingForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0006PlainFitting() => new()
	{
		Metadata = new("Construction and utilities/make plain ceramic fitting for domestic building work [craft_industrialised_construction_utilities_0006_plain_fitting]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0006_plain_fitting"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic fitting for domestic building work [craft_industrialised_construction_utilities_0006_plain_fitting]",
		Category = "Construction and utilities",
		Blurb = "Make a plain ceramic fitting for domestic building work from prepared ceramic stock.",
		Action = "making plain ceramic fitting for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain ceramic fitting for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1744.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1744 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0006_plain_fitting", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic fitting for domestic building work (@industrialised_construction_utilities_0006_plain_fitting)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainMildSteelBeamForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0007PlainBeam() => new()
	{
		Metadata = new("Construction and utilities/make plain mild steel beam for domestic building work [craft_industrialised_construction_utilities_0007_plain_beam]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0007_plain_beam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel beam for domestic building work [craft_industrialised_construction_utilities_0007_plain_beam]",
		Category = "Construction and utilities",
		Blurb = "Make a plain mild steel beam for domestic building work from prepared mild steel stock.",
		Action = "making plain mild steel beam for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain mild steel beam for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1784.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1784 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0007_plain_beam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel beam for domestic building work (@industrialised_construction_utilities_0007_plain_beam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainCastIronPanelForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0008PlainPanel() => new()
	{
		Metadata = new("Construction and utilities/make plain cast iron panel for domestic building work [craft_industrialised_construction_utilities_0008_plain_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0008_plain_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron panel for domestic building work [craft_industrialised_construction_utilities_0008_plain_panel]",
		Category = "Construction and utilities",
		Blurb = "Make a plain cast iron panel for domestic building work from prepared cast iron stock.",
		Action = "making plain cast iron panel for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain cast iron panel for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1825.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1825 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0008_plain_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron panel for domestic building work (@industrialised_construction_utilities_0008_plain_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainOakGrateForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0009PlainGrate() => new()
	{
		Metadata = new("Construction and utilities/make plain oak grate for domestic building work [craft_industrialised_construction_utilities_0009_plain_grate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0009_plain_grate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak grate for domestic building work [craft_industrialised_construction_utilities_0009_plain_grate]",
		Category = "Construction and utilities",
		Blurb = "Make a plain oak grate for domestic building work from prepared oak stock.",
		Action = "making plain oak grate for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain oak grate for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1866.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1866 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0009_plain_grate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak grate for domestic building work (@industrialised_construction_utilities_0009_plain_grate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePlainCeramicConduitForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0010PlainConduit() => new()
	{
		Metadata = new("Construction and utilities/make plain ceramic conduit for domestic building work [craft_industrialised_construction_utilities_0010_plain_conduit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0010_plain_conduit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic conduit for domestic building work [craft_industrialised_construction_utilities_0010_plain_conduit]",
		Category = "Construction and utilities",
		Blurb = "Make a plain ceramic conduit for domestic building work from prepared ceramic stock.",
		Action = "making plain ceramic conduit for domestic building work",
		ActiveCraftItemSdesc = "an in-progress plain ceramic conduit for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1906.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1906 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0010_plain_conduit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic conduit for domestic building work (@industrialised_construction_utilities_0010_plain_conduit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedMildSteelRubbleForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0011ReinforcedRubble() => new()
	{
		Metadata = new("Construction and utilities/make reinforced mild steel rubble for domestic building work [craft_industrialised_construction_utilities_0011_reinforced_rubble]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0011_reinforced_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel rubble for domestic building work [craft_industrialised_construction_utilities_0011_reinforced_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced mild steel rubble for domestic building work from prepared mild steel stock.",
		Action = "making reinforced mild steel rubble for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel rubble for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1947.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1947 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0011_reinforced_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel rubble for domestic building work (@industrialised_construction_utilities_0011_reinforced_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCastIronBrickForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0012ReinforcedBrick() => new()
	{
		Metadata = new("Construction and utilities/make reinforced cast iron brick for domestic building work [craft_industrialised_construction_utilities_0012_reinforced_brick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0012_reinforced_brick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron brick for domestic building work [craft_industrialised_construction_utilities_0012_reinforced_brick]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced cast iron brick for domestic building work from prepared cast iron stock.",
		Action = "making reinforced cast iron brick for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron brick for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1988.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1988 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0012_reinforced_brick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron brick for domestic building work (@industrialised_construction_utilities_0012_reinforced_brick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedOakTileForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0013ReinforcedTile() => new()
	{
		Metadata = new("Construction and utilities/make reinforced oak tile for domestic building work [craft_industrialised_construction_utilities_0013_reinforced_tile]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0013_reinforced_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak tile for domestic building work [craft_industrialised_construction_utilities_0013_reinforced_tile]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced oak tile for domestic building work from prepared oak stock.",
		Action = "making reinforced oak tile for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced oak tile for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2028.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2028 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0013_reinforced_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak tile for domestic building work (@industrialised_construction_utilities_0013_reinforced_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCeramicPipeForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0014ReinforcedPipe() => new()
	{
		Metadata = new("Construction and utilities/make reinforced ceramic pipe for domestic building work [craft_industrialised_construction_utilities_0014_reinforced_pipe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0014_reinforced_pipe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic pipe for domestic building work [craft_industrialised_construction_utilities_0014_reinforced_pipe]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced ceramic pipe for domestic building work from prepared ceramic stock.",
		Action = "making reinforced ceramic pipe for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic pipe for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2069.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2069 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0014_reinforced_pipe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic pipe for domestic building work (@industrialised_construction_utilities_0014_reinforced_pipe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedMildSteelValveForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0015ReinforcedValve() => new()
	{
		Metadata = new("Construction and utilities/make reinforced mild steel valve for domestic building work [craft_industrialised_construction_utilities_0015_reinforced_valve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0015_reinforced_valve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel valve for domestic building work [craft_industrialised_construction_utilities_0015_reinforced_valve]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced mild steel valve for domestic building work from prepared mild steel stock.",
		Action = "making reinforced mild steel valve for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel valve for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2110.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2110 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0015_reinforced_valve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel valve for domestic building work (@industrialised_construction_utilities_0015_reinforced_valve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCastIronFittingForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0016ReinforcedFitting() => new()
	{
		Metadata = new("Construction and utilities/make reinforced cast iron fitting for domestic building work [craft_industrialised_construction_utilities_0016_reinforced_fitting]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0016_reinforced_fitting"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron fitting for domestic building work [craft_industrialised_construction_utilities_0016_reinforced_fitting]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced cast iron fitting for domestic building work from prepared cast iron stock.",
		Action = "making reinforced cast iron fitting for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron fitting for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2150.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2150 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0016_reinforced_fitting", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron fitting for domestic building work (@industrialised_construction_utilities_0016_reinforced_fitting)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedOakBeamForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0017ReinforcedBeam() => new()
	{
		Metadata = new("Construction and utilities/make reinforced oak beam for domestic building work [craft_industrialised_construction_utilities_0017_reinforced_beam]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0017_reinforced_beam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak beam for domestic building work [craft_industrialised_construction_utilities_0017_reinforced_beam]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced oak beam for domestic building work from prepared oak stock.",
		Action = "making reinforced oak beam for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced oak beam for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2191.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2191 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0017_reinforced_beam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak beam for domestic building work (@industrialised_construction_utilities_0017_reinforced_beam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCeramicPanelForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0018ReinforcedPanel() => new()
	{
		Metadata = new("Construction and utilities/make reinforced ceramic panel for domestic building work [craft_industrialised_construction_utilities_0018_reinforced_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0018_reinforced_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic panel for domestic building work [craft_industrialised_construction_utilities_0018_reinforced_panel]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced ceramic panel for domestic building work from prepared ceramic stock.",
		Action = "making reinforced ceramic panel for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic panel for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2232.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2232 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0018_reinforced_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic panel for domestic building work (@industrialised_construction_utilities_0018_reinforced_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedMildSteelGrateForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0019ReinforcedGrate() => new()
	{
		Metadata = new("Construction and utilities/make reinforced mild steel grate for domestic building work [craft_industrialised_construction_utilities_0019_reinforced_grate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0019_reinforced_grate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel grate for domestic building work [craft_industrialised_construction_utilities_0019_reinforced_grate]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced mild steel grate for domestic building work from prepared mild steel stock.",
		Action = "making reinforced mild steel grate for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel grate for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2273.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2273 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0019_reinforced_grate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel grate for domestic building work (@industrialised_construction_utilities_0019_reinforced_grate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCastIronConduitForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0020ReinforcedConduit() => new()
	{
		Metadata = new("Construction and utilities/make reinforced cast iron conduit for domestic building work [craft_industrialised_construction_utilities_0020_reinforced_conduit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0020_reinforced_conduit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron conduit for domestic building work [craft_industrialised_construction_utilities_0020_reinforced_conduit]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced cast iron conduit for domestic building work from prepared cast iron stock.",
		Action = "making reinforced cast iron conduit for domestic building work",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron conduit for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2313.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2313 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0020_reinforced_conduit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron conduit for domestic building work (@industrialised_construction_utilities_0020_reinforced_conduit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactOakRubbleForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0021CompactRubble() => new()
	{
		Metadata = new("Construction and utilities/make compact oak rubble for domestic building work [craft_industrialised_construction_utilities_0021_compact_rubble]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0021_compact_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak rubble for domestic building work [craft_industrialised_construction_utilities_0021_compact_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make a compact oak rubble for domestic building work from prepared oak stock.",
		Action = "making compact oak rubble for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact oak rubble for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2354.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2354 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0021_compact_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak rubble for domestic building work (@industrialised_construction_utilities_0021_compact_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactCeramicBrickForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0022CompactBrick() => new()
	{
		Metadata = new("Construction and utilities/make compact ceramic brick for domestic building work [craft_industrialised_construction_utilities_0022_compact_brick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0022_compact_brick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic brick for domestic building work [craft_industrialised_construction_utilities_0022_compact_brick]",
		Category = "Construction and utilities",
		Blurb = "Make a compact ceramic brick for domestic building work from prepared ceramic stock.",
		Action = "making compact ceramic brick for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact ceramic brick for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2395.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2395 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0022_compact_brick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic brick for domestic building work (@industrialised_construction_utilities_0022_compact_brick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactMildSteelTileForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0023CompactTile() => new()
	{
		Metadata = new("Construction and utilities/make compact mild steel tile for domestic building work [craft_industrialised_construction_utilities_0023_compact_tile]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0023_compact_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel tile for domestic building work [craft_industrialised_construction_utilities_0023_compact_tile]",
		Category = "Construction and utilities",
		Blurb = "Make a compact mild steel tile for domestic building work from prepared mild steel stock.",
		Action = "making compact mild steel tile for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact mild steel tile for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2435.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2435 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0023_compact_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel tile for domestic building work (@industrialised_construction_utilities_0023_compact_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactCastIronPipeForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0024CompactPipe() => new()
	{
		Metadata = new("Construction and utilities/make compact cast iron pipe for domestic building work [craft_industrialised_construction_utilities_0024_compact_pipe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0024_compact_pipe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron pipe for domestic building work [craft_industrialised_construction_utilities_0024_compact_pipe]",
		Category = "Construction and utilities",
		Blurb = "Make a compact cast iron pipe for domestic building work from prepared cast iron stock.",
		Action = "making compact cast iron pipe for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact cast iron pipe for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2476.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2476 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0024_compact_pipe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron pipe for domestic building work (@industrialised_construction_utilities_0024_compact_pipe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactOakValveForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0025CompactValve() => new()
	{
		Metadata = new("Construction and utilities/make compact oak valve for domestic building work [craft_industrialised_construction_utilities_0025_compact_valve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0025_compact_valve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak valve for domestic building work [craft_industrialised_construction_utilities_0025_compact_valve]",
		Category = "Construction and utilities",
		Blurb = "Make a compact oak valve for domestic building work from prepared oak stock.",
		Action = "making compact oak valve for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact oak valve for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2517.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2517 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0025_compact_valve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak valve for domestic building work (@industrialised_construction_utilities_0025_compact_valve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactCeramicFittingForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0026CompactFitting() => new()
	{
		Metadata = new("Construction and utilities/make compact ceramic fitting for domestic building work [craft_industrialised_construction_utilities_0026_compact_fitting]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0026_compact_fitting"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic fitting for domestic building work [craft_industrialised_construction_utilities_0026_compact_fitting]",
		Category = "Construction and utilities",
		Blurb = "Make a compact ceramic fitting for domestic building work from prepared ceramic stock.",
		Action = "making compact ceramic fitting for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact ceramic fitting for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2558.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2558 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0026_compact_fitting", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic fitting for domestic building work (@industrialised_construction_utilities_0026_compact_fitting)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactMildSteelBeamForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0027CompactBeam() => new()
	{
		Metadata = new("Construction and utilities/make compact mild steel beam for domestic building work [craft_industrialised_construction_utilities_0027_compact_beam]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0027_compact_beam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel beam for domestic building work [craft_industrialised_construction_utilities_0027_compact_beam]",
		Category = "Construction and utilities",
		Blurb = "Make a compact mild steel beam for domestic building work from prepared mild steel stock.",
		Action = "making compact mild steel beam for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact mild steel beam for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2598.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2598 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0027_compact_beam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel beam for domestic building work (@industrialised_construction_utilities_0027_compact_beam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactCastIronPanelForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0028CompactPanel() => new()
	{
		Metadata = new("Construction and utilities/make compact cast iron panel for domestic building work [craft_industrialised_construction_utilities_0028_compact_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0028_compact_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron panel for domestic building work [craft_industrialised_construction_utilities_0028_compact_panel]",
		Category = "Construction and utilities",
		Blurb = "Make a compact cast iron panel for domestic building work from prepared cast iron stock.",
		Action = "making compact cast iron panel for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact cast iron panel for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2639.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2639 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0028_compact_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron panel for domestic building work (@industrialised_construction_utilities_0028_compact_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactOakGrateForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0029CompactGrate() => new()
	{
		Metadata = new("Construction and utilities/make compact oak grate for domestic building work [craft_industrialised_construction_utilities_0029_compact_grate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0029_compact_grate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak grate for domestic building work [craft_industrialised_construction_utilities_0029_compact_grate]",
		Category = "Construction and utilities",
		Blurb = "Make a compact oak grate for domestic building work from prepared oak stock.",
		Action = "making compact oak grate for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact oak grate for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2680.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2680 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0029_compact_grate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak grate for domestic building work (@industrialised_construction_utilities_0029_compact_grate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactCeramicConduitForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0030CompactConduit() => new()
	{
		Metadata = new("Construction and utilities/make compact ceramic conduit for domestic building work [craft_industrialised_construction_utilities_0030_compact_conduit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0030_compact_conduit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic conduit for domestic building work [craft_industrialised_construction_utilities_0030_compact_conduit]",
		Category = "Construction and utilities",
		Blurb = "Make a compact ceramic conduit for domestic building work from prepared ceramic stock.",
		Action = "making compact ceramic conduit for domestic building work",
		ActiveCraftItemSdesc = "an in-progress compact ceramic conduit for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2720.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2720 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0030_compact_conduit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic conduit for domestic building work (@industrialised_construction_utilities_0030_compact_conduit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyMildSteelRubbleForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0031HeavyDutyRubble() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty mild steel rubble for domestic building work [craft_industrialised_construction_utilities_0031_heavy_duty_rubble]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0031_heavy_duty_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel rubble for domestic building work [craft_industrialised_construction_utilities_0031_heavy_duty_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty mild steel rubble for domestic building work from prepared mild steel stock.",
		Action = "making heavy-duty mild steel rubble for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel rubble for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2761.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2761 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0031_heavy_duty_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel rubble for domestic building work (@industrialised_construction_utilities_0031_heavy_duty_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyCastIronBrickForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0032HeavyDutyBrick() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty cast iron brick for domestic building work [craft_industrialised_construction_utilities_0032_heavy_duty_brick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0032_heavy_duty_brick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron brick for domestic building work [craft_industrialised_construction_utilities_0032_heavy_duty_brick]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty cast iron brick for domestic building work from prepared cast iron stock.",
		Action = "making heavy-duty cast iron brick for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron brick for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2802.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2802 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0032_heavy_duty_brick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron brick for domestic building work (@industrialised_construction_utilities_0032_heavy_duty_brick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
