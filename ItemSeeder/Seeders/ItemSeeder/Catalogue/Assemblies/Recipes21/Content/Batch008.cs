#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0014ReinforcedHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0014_reinforced_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0014_reinforced_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0014_reinforced_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making reinforced cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1958.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1958 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0014_reinforced_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0014_reinforced_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0015ReinforcedWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0015_reinforced_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0015_reinforced_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0015_reinforced_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making reinforced oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1999.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1999 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0015_reinforced_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0015_reinforced_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0016ReinforcedDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0016_reinforced_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0016_reinforced_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0016_reinforced_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making reinforced tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2039.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2039 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0016_reinforced_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0016_reinforced_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0017ReinforcedLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0017_reinforced_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0017_reinforced_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0017_reinforced_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making reinforced mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2080.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2080 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0017_reinforced_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0017_reinforced_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0018ReinforcedVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0018_reinforced_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0018_reinforced_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0018_reinforced_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making reinforced cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2121.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2121 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0018_reinforced_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0018_reinforced_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0019ReinforcedPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0019_reinforced_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0019_reinforced_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0019_reinforced_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced oak plane for domestic maintenance from prepared oak stock.",
		Action = "making reinforced oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2162.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2162 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0019_reinforced_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0019_reinforced_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0020ReinforcedSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0020_reinforced_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0020_reinforced_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0020_reinforced_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making reinforced tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2202.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2202 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0020_reinforced_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0020_reinforced_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0021ReinforcedPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0021_reinforced_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0021_reinforced_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0021_reinforced_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making reinforced mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2243.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2243 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0021_reinforced_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0021_reinforced_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0022ReinforcedGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0022_reinforced_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0022_reinforced_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0022_reinforced_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making reinforced cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2284.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2284 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0022_reinforced_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0022_reinforced_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0023ReinforcedAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0023_reinforced_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0023_reinforced_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0023_reinforced_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making reinforced oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2324.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2324 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0023_reinforced_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0023_reinforced_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0024ReinforcedGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0024_reinforced_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0024_reinforced_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0024_reinforced_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making reinforced tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2365.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2365 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0024_reinforced_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0024_reinforced_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0025CompactScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0025_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0025_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0025_compact_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making compact mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2406.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2406 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0025_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0025_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0026CompactHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0026_compact_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0026_compact_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0026_compact_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making compact cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2446.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2446 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0026_compact_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0026_compact_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0027CompactWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0027_compact_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0027_compact_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0027_compact_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making compact oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2487.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2487 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0027_compact_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0027_compact_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0028CompactDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0028_compact_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0028_compact_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0028_compact_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making compact tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2528.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2528 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0028_compact_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0028_compact_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0029CompactLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0029_compact_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0029_compact_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0029_compact_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making compact mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2568.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2568 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0029_compact_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0029_compact_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0030CompactVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0030_compact_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0030_compact_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0030_compact_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making compact cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2609.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2609 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0030_compact_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0030_compact_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0031CompactPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0031_compact_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0031_compact_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0031_compact_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact oak plane for domestic maintenance from prepared oak stock.",
		Action = "making compact oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2650.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2650 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0031_compact_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0031_compact_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0032CompactSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0032_compact_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0032_compact_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0032_compact_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making compact tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2691.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2691 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0032_compact_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0032_compact_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0033CompactPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0033_compact_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0033_compact_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0033_compact_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making compact mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2731.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2731 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0033_compact_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0033_compact_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0034CompactGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0034_compact_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0034_compact_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0034_compact_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making compact cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2772.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2772 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0034_compact_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0034_compact_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0035CompactAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0035_compact_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0035_compact_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0035_compact_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making compact oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2813.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2813 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0035_compact_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0035_compact_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeCompactToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0036CompactGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make compact tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0036_compact_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0036_compact_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0036_compact_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a compact tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making compact tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress compact tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2853.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2853 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0036_compact_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0036_compact_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0037HeavyDutyScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0037_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0037_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0037_heavy_duty_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2894.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2894 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0037_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0037_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0038HeavyDutyHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0038_heavy_duty_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0038_heavy_duty_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0038_heavy_duty_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making heavy-duty cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2935.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2935 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0038_heavy_duty_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0038_heavy_duty_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0039HeavyDutyWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0039_heavy_duty_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0039_heavy_duty_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0039_heavy_duty_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making heavy-duty oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2976.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2976 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0039_heavy_duty_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0039_heavy_duty_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0040HeavyDutyDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0040_heavy_duty_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0040_heavy_duty_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0040_heavy_duty_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making heavy-duty tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3016.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3016 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0040_heavy_duty_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0040_heavy_duty_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0041HeavyDutyLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0041_heavy_duty_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0041_heavy_duty_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0041_heavy_duty_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making heavy-duty mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3057.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3057 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0041_heavy_duty_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0041_heavy_duty_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0042HeavyDutyVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0042_heavy_duty_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0042_heavy_duty_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0042_heavy_duty_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making heavy-duty cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3098.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3098 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0042_heavy_duty_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0042_heavy_duty_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0043HeavyDutyPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0043_heavy_duty_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0043_heavy_duty_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0043_heavy_duty_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty oak plane for domestic maintenance from prepared oak stock.",
		Action = "making heavy-duty oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3138.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3138 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0043_heavy_duty_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0043_heavy_duty_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0044HeavyDutySaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0044_heavy_duty_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0044_heavy_duty_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0044_heavy_duty_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making heavy-duty tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3179.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3179 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0044_heavy_duty_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0044_heavy_duty_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0045HeavyDutyPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0045_heavy_duty_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0045_heavy_duty_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0045_heavy_duty_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making heavy-duty mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3220.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3220 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0045_heavy_duty_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0045_heavy_duty_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0046HeavyDutyGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0046_heavy_duty_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0046_heavy_duty_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0046_heavy_duty_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making heavy-duty cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3260.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3260 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0046_heavy_duty_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0046_heavy_duty_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0047HeavyDutyAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0047_heavy_duty_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0047_heavy_duty_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0047_heavy_duty_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making heavy-duty oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3301.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3301 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0047_heavy_duty_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0047_heavy_duty_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeHeavyDutyToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0048HeavyDutyGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make heavy-duty tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0048_heavy_duty_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0048_heavy_duty_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0048_heavy_duty_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a heavy-duty tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making heavy-duty tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress heavy-duty tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3342.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3342 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0048_heavy_duty_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0048_heavy_duty_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0049FoldingScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0049_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0049_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0049_folding_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making folding mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3383.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3383 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0049_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0049_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0050FoldingHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0050_folding_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0050_folding_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0050_folding_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making folding cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3423.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3423 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0050_folding_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0050_folding_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0051FoldingWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0051_folding_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0051_folding_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0051_folding_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making folding oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3464.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3464 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0051_folding_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0051_folding_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0052FoldingDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0052_folding_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0052_folding_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0052_folding_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making folding tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3505.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3505 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0052_folding_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0052_folding_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0053FoldingLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0053_folding_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0053_folding_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0053_folding_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making folding mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3545.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3545 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0053_folding_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0053_folding_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0054FoldingVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0054_folding_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0054_folding_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0054_folding_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making folding cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3586.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3586 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0054_folding_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0054_folding_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0055FoldingPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0055_folding_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0055_folding_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0055_folding_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding oak plane for domestic maintenance from prepared oak stock.",
		Action = "making folding oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3627.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3627 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0055_folding_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0055_folding_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0056FoldingSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0056_folding_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0056_folding_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0056_folding_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making folding tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3667.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3667 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0056_folding_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0056_folding_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0057FoldingPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0057_folding_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0057_folding_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0057_folding_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making folding mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3708.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3708 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0057_folding_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0057_folding_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0058FoldingGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0058_folding_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0058_folding_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0058_folding_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making folding cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3749.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3749 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0058_folding_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0058_folding_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0059FoldingAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0059_folding_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0059_folding_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0059_folding_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making folding oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3790.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3790 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0059_folding_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0059_folding_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeFoldingToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0060FoldingGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make folding tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0060_folding_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0060_folding_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0060_folding_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a folding tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making folding tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress folding tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3830.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3830 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0060_folding_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0060_folding_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0061PortableScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0061_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0061_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0061_portable_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making portable mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3871.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3871 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0061_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0061_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0062PortableHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0062_portable_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0062_portable_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0062_portable_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making portable cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3912.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3912 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0062_portable_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0062_portable_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0063PortableWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0063_portable_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0063_portable_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0063_portable_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making portable oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3952.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3952 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0063_portable_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0063_portable_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0064PortableDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0064_portable_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0064_portable_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0064_portable_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making portable tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3993.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3993 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0064_portable_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0064_portable_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0065PortableLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0065_portable_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0065_portable_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0065_portable_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making portable mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4034.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4034 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0065_portable_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0065_portable_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0066PortableVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0066_portable_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0066_portable_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0066_portable_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making portable cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4074.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4074 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0066_portable_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0066_portable_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0067PortablePlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0067_portable_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0067_portable_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0067_portable_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable oak plane for domestic maintenance from prepared oak stock.",
		Action = "making portable oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4115.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4115 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0067_portable_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0067_portable_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0068PortableSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0068_portable_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0068_portable_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0068_portable_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making portable tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4156.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4156 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0068_portable_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0068_portable_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0069PortablePress() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0069_portable_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0069_portable_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0069_portable_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making portable mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4196.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4196 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0069_portable_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0069_portable_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0070PortableGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0070_portable_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0070_portable_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0070_portable_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making portable cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4237.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4237 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0070_portable_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0070_portable_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0071PortableAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0071_portable_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0071_portable_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0071_portable_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making portable oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4278.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4278 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0071_portable_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0071_portable_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePortableToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0072PortableGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make portable tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0072_portable_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0072_portable_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0072_portable_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a portable tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making portable tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress portable tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4319.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4319 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0072_portable_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0072_portable_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0073WorkshopScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0073_workshop_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0073_workshop_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0073_workshop_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making workshop mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4359.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4359 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0073_workshop_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0073_workshop_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
