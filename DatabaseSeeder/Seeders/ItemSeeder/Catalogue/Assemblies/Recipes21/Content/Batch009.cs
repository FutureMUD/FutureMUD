#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0074WorkshopHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0074_workshop_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0074_workshop_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0074_workshop_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making workshop cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4400.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4400 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0074_workshop_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0074_workshop_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0075WorkshopWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0075_workshop_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0075_workshop_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0075_workshop_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making workshop oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4441.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4441 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0075_workshop_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0075_workshop_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0076WorkshopDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0076_workshop_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0076_workshop_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0076_workshop_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making workshop tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4481.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4481 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0076_workshop_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0076_workshop_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0077WorkshopLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0077_workshop_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0077_workshop_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0077_workshop_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making workshop mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4522.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4522 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0077_workshop_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0077_workshop_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0078WorkshopVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0078_workshop_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0078_workshop_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0078_workshop_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making workshop cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4563.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4563 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0078_workshop_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0078_workshop_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0079WorkshopPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0079_workshop_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0079_workshop_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0079_workshop_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop oak plane for domestic maintenance from prepared oak stock.",
		Action = "making workshop oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4604.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4604 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0079_workshop_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0079_workshop_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0080WorkshopSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0080_workshop_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0080_workshop_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0080_workshop_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making workshop tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4644.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4644 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0080_workshop_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0080_workshop_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0081WorkshopPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0081_workshop_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0081_workshop_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0081_workshop_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making workshop mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4685.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4685 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0081_workshop_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0081_workshop_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0082WorkshopGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0082_workshop_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0082_workshop_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0082_workshop_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making workshop cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4726.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4726 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0082_workshop_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0082_workshop_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0083WorkshopAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0083_workshop_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0083_workshop_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0083_workshop_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making workshop oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4766.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4766 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0083_workshop_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0083_workshop_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWorkshopToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0084WorkshopGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make workshop tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0084_workshop_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0084_workshop_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0084_workshop_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a workshop tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making workshop tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress workshop tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4807.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4807 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0084_workshop_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0084_workshop_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0085InstitutionalScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0085_institutional_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0085_institutional_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0085_institutional_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making institutional mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4848.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4848 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0085_institutional_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0085_institutional_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0086InstitutionalHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0086_institutional_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0086_institutional_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0086_institutional_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making institutional cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4888.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4888 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0086_institutional_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0086_institutional_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0087InstitutionalWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0087_institutional_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0087_institutional_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0087_institutional_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making institutional oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4929.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4929 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0087_institutional_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0087_institutional_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0088InstitutionalDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0088_institutional_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0088_institutional_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0088_institutional_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making institutional tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4970.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4970 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0088_institutional_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0088_institutional_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0089InstitutionalLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0089_institutional_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0089_institutional_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0089_institutional_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making institutional mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5010.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5010 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0089_institutional_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0089_institutional_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0090InstitutionalVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0090_institutional_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0090_institutional_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0090_institutional_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making institutional cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5051.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5051 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0090_institutional_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0090_institutional_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0091InstitutionalPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0091_institutional_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0091_institutional_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0091_institutional_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional oak plane for domestic maintenance from prepared oak stock.",
		Action = "making institutional oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5092.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5092 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0091_institutional_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0091_institutional_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0092InstitutionalSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0092_institutional_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0092_institutional_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0092_institutional_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making institutional tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5133.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5133 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0092_institutional_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0092_institutional_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0093InstitutionalPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0093_institutional_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0093_institutional_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0093_institutional_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making institutional mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5173.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5173 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0093_institutional_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0093_institutional_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0094InstitutionalGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0094_institutional_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0094_institutional_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0094_institutional_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making institutional cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5214.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5214 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0094_institutional_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0094_institutional_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0095InstitutionalAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0095_institutional_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0095_institutional_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0095_institutional_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making institutional oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5255.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5255 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0095_institutional_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0095_institutional_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeInstitutionalToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0096InstitutionalGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make institutional tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0096_institutional_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0096_institutional_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0096_institutional_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an institutional tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making institutional tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress institutional tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5295.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5295 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0096_institutional_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0096_institutional_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0097WeatherproofScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0097_weatherproof_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0097_weatherproof_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0097_weatherproof_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making weatherproof mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5336.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5336 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0097_weatherproof_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0097_weatherproof_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0098WeatherproofHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0098_weatherproof_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0098_weatherproof_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0098_weatherproof_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making weatherproof cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5377.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5377 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0098_weatherproof_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0098_weatherproof_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0099WeatherproofWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0099_weatherproof_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0099_weatherproof_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0099_weatherproof_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making weatherproof oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5418.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5418 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0099_weatherproof_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0099_weatherproof_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0100WeatherproofDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0100_weatherproof_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0100_weatherproof_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0100_weatherproof_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making weatherproof tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5458.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5458 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0100_weatherproof_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0100_weatherproof_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0101WeatherproofLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0101_weatherproof_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0101_weatherproof_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0101_weatherproof_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making weatherproof mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5499.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5499 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0101_weatherproof_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0101_weatherproof_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0102WeatherproofVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0102_weatherproof_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0102_weatherproof_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0102_weatherproof_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making weatherproof cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 95.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("95 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0102_weatherproof_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0102_weatherproof_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0103WeatherproofPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0103_weatherproof_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0103_weatherproof_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0103_weatherproof_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof oak plane for domestic maintenance from prepared oak stock.",
		Action = "making weatherproof oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0103_weatherproof_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0103_weatherproof_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0104WeatherproofSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0104_weatherproof_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0104_weatherproof_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0104_weatherproof_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making weatherproof tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 176.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("176 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0104_weatherproof_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0104_weatherproof_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0105WeatherproofPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0105_weatherproof_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0105_weatherproof_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0105_weatherproof_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making weatherproof mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 217.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("217 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0105_weatherproof_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0105_weatherproof_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0106WeatherproofGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0106_weatherproof_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0106_weatherproof_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0106_weatherproof_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making weatherproof cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 257.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("257 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0106_weatherproof_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0106_weatherproof_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0107WeatherproofAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0107_weatherproof_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0107_weatherproof_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0107_weatherproof_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making weatherproof oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 298.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("298 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0107_weatherproof_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0107_weatherproof_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeWeatherproofToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0108WeatherproofGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make weatherproof tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0108_weatherproof_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0108_weatherproof_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0108_weatherproof_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a weatherproof tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making weatherproof tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress weatherproof tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 339.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("339 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0108_weatherproof_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0108_weatherproof_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0109ServiceScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make service mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0109_service_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0109_service_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0109_service_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making service mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 380.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("380 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0109_service_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0109_service_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0110ServiceHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make service cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0110_service_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0110_service_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0110_service_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making service cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0110_service_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0110_service_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0111ServiceWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make service oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0111_service_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0111_service_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0111_service_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making service oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 461.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("461 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0111_service_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0111_service_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0112ServiceDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make service tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0112_service_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0112_service_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0112_service_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making service tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 502.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("502 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0112_service_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0112_service_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0113ServiceLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make service mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0113_service_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0113_service_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0113_service_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making service mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 542.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("542 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0113_service_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0113_service_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0114ServiceVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make service cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0114_service_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0114_service_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0114_service_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making service cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 583.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("583 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0114_service_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0114_service_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0115ServicePlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make service oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0115_service_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0115_service_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0115_service_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service oak plane for domestic maintenance from prepared oak stock.",
		Action = "making service oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 624.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("624 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0115_service_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0115_service_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0116ServiceSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make service tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0116_service_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0116_service_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0116_service_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making service tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 664.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("664 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0116_service_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0116_service_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0117ServicePress() => new()
	{
		Metadata = new("Tools, repair and machinery/make service mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0117_service_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0117_service_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0117_service_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making service mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 705.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("705 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0117_service_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0117_service_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0118ServiceGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make service cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0118_service_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0118_service_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0118_service_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making service cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 746.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("746 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0118_service_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0118_service_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0119ServiceAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make service oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0119_service_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0119_service_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0119_service_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making service oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 787.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("787 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0119_service_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0119_service_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeServiceToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0120ServiceGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make service tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0120_service_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0120_service_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0120_service_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a service tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making service tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress service tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 827.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("827 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0120_service_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0120_service_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0121PrecisionScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0121_precision_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0121_precision_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0121_precision_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making precision mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 868.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("868 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0121_precision_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0121_precision_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0122PrecisionHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0122_precision_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0122_precision_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0122_precision_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making precision cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 909.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("909 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0122_precision_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0122_precision_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0123PrecisionWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0123_precision_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0123_precision_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0123_precision_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making precision oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 949.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("949 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0123_precision_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0123_precision_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0124PrecisionDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0124_precision_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0124_precision_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0124_precision_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making precision tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 990.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("990 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0124_precision_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0124_precision_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0125PrecisionLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0125_precision_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0125_precision_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0125_precision_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making precision mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1031.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1031 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0125_precision_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0125_precision_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0126PrecisionVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0126_precision_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0126_precision_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0126_precision_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making precision cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1071.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1071 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0126_precision_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0126_precision_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0127PrecisionPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0127_precision_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0127_precision_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0127_precision_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision oak plane for domestic maintenance from prepared oak stock.",
		Action = "making precision oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1112.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1112 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0127_precision_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0127_precision_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0128PrecisionSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0128_precision_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0128_precision_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0128_precision_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making precision tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1153.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1153 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0128_precision_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0128_precision_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0129PrecisionPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0129_precision_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0129_precision_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0129_precision_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making precision mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1194.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1194 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0129_precision_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0129_precision_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0130PrecisionGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0130_precision_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0130_precision_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0130_precision_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making precision cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1234.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1234 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0130_precision_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0130_precision_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0131PrecisionAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0131_precision_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0131_precision_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0131_precision_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making precision oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1275.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1275 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0131_precision_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0131_precision_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePrecisionToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0132PrecisionGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make precision tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0132_precision_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0132_precision_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0132_precision_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a precision tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making precision tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress precision tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1316.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1316 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0132_precision_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0132_precision_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeEconomyMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0133EconomyScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make economy mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0133_economy_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0133_economy_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0133_economy_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make an economy mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making economy mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress economy mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1356.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1356 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0133_economy_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0133_economy_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
