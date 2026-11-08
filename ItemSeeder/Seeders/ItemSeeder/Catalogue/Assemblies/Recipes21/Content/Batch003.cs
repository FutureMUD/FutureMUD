#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainGlassTerminalForDomesticDataWorkCraftIndustrialisedComputingNetworking0002PlainTerminal() => new()
	{
		Metadata = new("Computing and networking/make plain glass terminal for domestic data work [craft_industrialised_computing_networking_0002_plain_terminal]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0002_plain_terminal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass terminal for domestic data work [craft_industrialised_computing_networking_0002_plain_terminal]",
		Category = "Computing and networking",
		Blurb = "Make a plain glass terminal for domestic data work from prepared glass stock.",
		Action = "making plain glass terminal for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain glass terminal for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1025.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1025 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0002_plain_terminal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass terminal for domestic data work (@industrialised_computing_networking_0002_plain_terminal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainNaturalRubberAdapterForDomesticDataWorkCraftIndustrialisedComputingNetworking0003PlainAdapter() => new()
	{
		Metadata = new("Computing and networking/make plain natural rubber adapter for domestic data work [craft_industrialised_computing_networking_0003_plain_adapter]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0003_plain_adapter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber adapter for domestic data work [craft_industrialised_computing_networking_0003_plain_adapter]",
		Category = "Computing and networking",
		Blurb = "Make a plain natural rubber adapter for domestic data work from prepared natural rubber stock.",
		Action = "making plain natural rubber adapter for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber adapter for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1066.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1066 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0003_plain_adapter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber adapter for domestic data work (@industrialised_computing_networking_0003_plain_adapter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainMildSteelSwitchForDomesticDataWorkCraftIndustrialisedComputingNetworking0004PlainSwitch() => new()
	{
		Metadata = new("Computing and networking/make plain mild steel switch for domestic data work [craft_industrialised_computing_networking_0004_plain_switch]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0004_plain_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel switch for domestic data work [craft_industrialised_computing_networking_0004_plain_switch]",
		Category = "Computing and networking",
		Blurb = "Make a plain mild steel switch for domestic data work from prepared mild steel stock.",
		Action = "making plain mild steel switch for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain mild steel switch for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1107.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1107 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0004_plain_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel switch for domestic data work (@industrialised_computing_networking_0004_plain_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainCopperCabinetForDomesticDataWorkCraftIndustrialisedComputingNetworking0005PlainCabinet() => new()
	{
		Metadata = new("Computing and networking/make plain copper cabinet for domestic data work [craft_industrialised_computing_networking_0005_plain_cabinet]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0005_plain_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper cabinet for domestic data work [craft_industrialised_computing_networking_0005_plain_cabinet]",
		Category = "Computing and networking",
		Blurb = "Make a plain copper cabinet for domestic data work from prepared copper stock.",
		Action = "making plain copper cabinet for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain copper cabinet for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1147.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1147 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0005_plain_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper cabinet for domestic data work (@industrialised_computing_networking_0005_plain_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainGlassConsoleForDomesticDataWorkCraftIndustrialisedComputingNetworking0006PlainConsole() => new()
	{
		Metadata = new("Computing and networking/make plain glass console for domestic data work [craft_industrialised_computing_networking_0006_plain_console]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0006_plain_console"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass console for domestic data work [craft_industrialised_computing_networking_0006_plain_console]",
		Category = "Computing and networking",
		Blurb = "Make a plain glass console for domestic data work from prepared glass stock.",
		Action = "making plain glass console for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain glass console for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1188.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1188 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0006_plain_console", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass console for domestic data work (@industrialised_computing_networking_0006_plain_console)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainNaturalRubberReaderForDomesticDataWorkCraftIndustrialisedComputingNetworking0007PlainReader() => new()
	{
		Metadata = new("Computing and networking/make plain natural rubber reader for domestic data work [craft_industrialised_computing_networking_0007_plain_reader]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0007_plain_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber reader for domestic data work [craft_industrialised_computing_networking_0007_plain_reader]",
		Category = "Computing and networking",
		Blurb = "Make a plain natural rubber reader for domestic data work from prepared natural rubber stock.",
		Action = "making plain natural rubber reader for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber reader for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1229.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1229 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0007_plain_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber reader for domestic data work (@industrialised_computing_networking_0007_plain_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainMildSteelKeyboardForDomesticDataWorkCraftIndustrialisedComputingNetworking0008PlainKeyboard() => new()
	{
		Metadata = new("Computing and networking/make plain mild steel keyboard for domestic data work [craft_industrialised_computing_networking_0008_plain_keyboard]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0008_plain_keyboard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel keyboard for domestic data work [craft_industrialised_computing_networking_0008_plain_keyboard]",
		Category = "Computing and networking",
		Blurb = "Make a plain mild steel keyboard for domestic data work from prepared mild steel stock.",
		Action = "making plain mild steel keyboard for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain mild steel keyboard for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1269.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1269 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0008_plain_keyboard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel keyboard for domestic data work (@industrialised_computing_networking_0008_plain_keyboard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainCopperMonitorForDomesticDataWorkCraftIndustrialisedComputingNetworking0009PlainMonitor() => new()
	{
		Metadata = new("Computing and networking/make plain copper monitor for domestic data work [craft_industrialised_computing_networking_0009_plain_monitor]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0009_plain_monitor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper monitor for domestic data work [craft_industrialised_computing_networking_0009_plain_monitor]",
		Category = "Computing and networking",
		Blurb = "Make a plain copper monitor for domestic data work from prepared copper stock.",
		Action = "making plain copper monitor for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain copper monitor for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1310.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1310 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0009_plain_monitor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper monitor for domestic data work (@industrialised_computing_networking_0009_plain_monitor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainGlassCableForDomesticDataWorkCraftIndustrialisedComputingNetworking0010PlainCable() => new()
	{
		Metadata = new("Computing and networking/make plain glass cable for domestic data work [craft_industrialised_computing_networking_0010_plain_cable]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0010_plain_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass cable for domestic data work [craft_industrialised_computing_networking_0010_plain_cable]",
		Category = "Computing and networking",
		Blurb = "Make a plain glass cable for domestic data work from prepared glass stock.",
		Action = "making plain glass cable for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain glass cable for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1351.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1351 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0010_plain_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass cable for domestic data work (@industrialised_computing_networking_0010_plain_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedNaturalRubberScrapForDomesticDataWorkCraftIndustrialisedComputingNetworking0011ReinforcedScrap() => new()
	{
		Metadata = new("Computing and networking/make reinforced natural rubber scrap for domestic data work [craft_industrialised_computing_networking_0011_reinforced_scrap]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber scrap for domestic data work [craft_industrialised_computing_networking_0011_reinforced_scrap]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced natural rubber scrap for domestic data work from prepared natural rubber stock.",
		Action = "making reinforced natural rubber scrap for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber scrap for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1392.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1392 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber scrap for domestic data work (@industrialised_computing_networking_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedMildSteelTerminalForDomesticDataWorkCraftIndustrialisedComputingNetworking0012ReinforcedTerminal() => new()
	{
		Metadata = new("Computing and networking/make reinforced mild steel terminal for domestic data work [craft_industrialised_computing_networking_0012_reinforced_terminal]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0012_reinforced_terminal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel terminal for domestic data work [craft_industrialised_computing_networking_0012_reinforced_terminal]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced mild steel terminal for domestic data work from prepared mild steel stock.",
		Action = "making reinforced mild steel terminal for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel terminal for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1432.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1432 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0012_reinforced_terminal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel terminal for domestic data work (@industrialised_computing_networking_0012_reinforced_terminal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedCopperAdapterForDomesticDataWorkCraftIndustrialisedComputingNetworking0013ReinforcedAdapter() => new()
	{
		Metadata = new("Computing and networking/make reinforced copper adapter for domestic data work [craft_industrialised_computing_networking_0013_reinforced_adapter]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0013_reinforced_adapter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper adapter for domestic data work [craft_industrialised_computing_networking_0013_reinforced_adapter]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced copper adapter for domestic data work from prepared copper stock.",
		Action = "making reinforced copper adapter for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced copper adapter for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1473.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1473 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0013_reinforced_adapter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper adapter for domestic data work (@industrialised_computing_networking_0013_reinforced_adapter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedGlassSwitchForDomesticDataWorkCraftIndustrialisedComputingNetworking0014ReinforcedSwitch() => new()
	{
		Metadata = new("Computing and networking/make reinforced glass switch for domestic data work [craft_industrialised_computing_networking_0014_reinforced_switch]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0014_reinforced_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass switch for domestic data work [craft_industrialised_computing_networking_0014_reinforced_switch]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced glass switch for domestic data work from prepared glass stock.",
		Action = "making reinforced glass switch for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced glass switch for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1514.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1514 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0014_reinforced_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass switch for domestic data work (@industrialised_computing_networking_0014_reinforced_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedNaturalRubberCabinetForDomesticDataWorkCraftIndustrialisedComputingNetworking0015ReinforcedCabinet() => new()
	{
		Metadata = new("Computing and networking/make reinforced natural rubber cabinet for domestic data work [craft_industrialised_computing_networking_0015_reinforced_cabinet]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0015_reinforced_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber cabinet for domestic data work [craft_industrialised_computing_networking_0015_reinforced_cabinet]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced natural rubber cabinet for domestic data work from prepared natural rubber stock.",
		Action = "making reinforced natural rubber cabinet for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber cabinet for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1554.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1554 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0015_reinforced_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber cabinet for domestic data work (@industrialised_computing_networking_0015_reinforced_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedMildSteelConsoleForDomesticDataWorkCraftIndustrialisedComputingNetworking0016ReinforcedConsole() => new()
	{
		Metadata = new("Computing and networking/make reinforced mild steel console for domestic data work [craft_industrialised_computing_networking_0016_reinforced_console]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0016_reinforced_console"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel console for domestic data work [craft_industrialised_computing_networking_0016_reinforced_console]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced mild steel console for domestic data work from prepared mild steel stock.",
		Action = "making reinforced mild steel console for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel console for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1595.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1595 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0016_reinforced_console", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel console for domestic data work (@industrialised_computing_networking_0016_reinforced_console)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedCopperReaderForDomesticDataWorkCraftIndustrialisedComputingNetworking0017ReinforcedReader() => new()
	{
		Metadata = new("Computing and networking/make reinforced copper reader for domestic data work [craft_industrialised_computing_networking_0017_reinforced_reader]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0017_reinforced_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper reader for domestic data work [craft_industrialised_computing_networking_0017_reinforced_reader]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced copper reader for domestic data work from prepared copper stock.",
		Action = "making reinforced copper reader for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced copper reader for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1636.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1636 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0017_reinforced_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper reader for domestic data work (@industrialised_computing_networking_0017_reinforced_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedGlassKeyboardForDomesticDataWorkCraftIndustrialisedComputingNetworking0018ReinforcedKeyboard() => new()
	{
		Metadata = new("Computing and networking/make reinforced glass keyboard for domestic data work [craft_industrialised_computing_networking_0018_reinforced_keyboard]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0018_reinforced_keyboard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass keyboard for domestic data work [craft_industrialised_computing_networking_0018_reinforced_keyboard]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced glass keyboard for domestic data work from prepared glass stock.",
		Action = "making reinforced glass keyboard for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced glass keyboard for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1676.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1676 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0018_reinforced_keyboard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass keyboard for domestic data work (@industrialised_computing_networking_0018_reinforced_keyboard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedNaturalRubberMonitorForDomesticDataWorkCraftIndustrialisedComputingNetworking0019ReinforcedMonitor() => new()
	{
		Metadata = new("Computing and networking/make reinforced natural rubber monitor for domestic data work [craft_industrialised_computing_networking_0019_reinforced_monitor]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0019_reinforced_monitor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber monitor for domestic data work [craft_industrialised_computing_networking_0019_reinforced_monitor]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced natural rubber monitor for domestic data work from prepared natural rubber stock.",
		Action = "making reinforced natural rubber monitor for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber monitor for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1717.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1717 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0019_reinforced_monitor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber monitor for domestic data work (@industrialised_computing_networking_0019_reinforced_monitor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeReinforcedMildSteelCableForDomesticDataWorkCraftIndustrialisedComputingNetworking0020ReinforcedCable() => new()
	{
		Metadata = new("Computing and networking/make reinforced mild steel cable for domestic data work [craft_industrialised_computing_networking_0020_reinforced_cable]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0020_reinforced_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel cable for domestic data work [craft_industrialised_computing_networking_0020_reinforced_cable]",
		Category = "Computing and networking",
		Blurb = "Make a reinforced mild steel cable for domestic data work from prepared mild steel stock.",
		Action = "making reinforced mild steel cable for domestic data work",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel cable for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1758.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1758 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0020_reinforced_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel cable for domestic data work (@industrialised_computing_networking_0020_reinforced_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactCopperScrapForDomesticDataWorkCraftIndustrialisedComputingNetworking0021CompactScrap() => new()
	{
		Metadata = new("Computing and networking/make compact copper scrap for domestic data work [craft_industrialised_computing_networking_0021_compact_scrap]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper scrap for domestic data work [craft_industrialised_computing_networking_0021_compact_scrap]",
		Category = "Computing and networking",
		Blurb = "Make a compact copper scrap for domestic data work from prepared copper stock.",
		Action = "making compact copper scrap for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact copper scrap for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1799.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1799 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper scrap for domestic data work (@industrialised_computing_networking_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactGlassTerminalForDomesticDataWorkCraftIndustrialisedComputingNetworking0022CompactTerminal() => new()
	{
		Metadata = new("Computing and networking/make compact glass terminal for domestic data work [craft_industrialised_computing_networking_0022_compact_terminal]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0022_compact_terminal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass terminal for domestic data work [craft_industrialised_computing_networking_0022_compact_terminal]",
		Category = "Computing and networking",
		Blurb = "Make a compact glass terminal for domestic data work from prepared glass stock.",
		Action = "making compact glass terminal for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact glass terminal for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1839.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1839 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0022_compact_terminal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass terminal for domestic data work (@industrialised_computing_networking_0022_compact_terminal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactNaturalRubberAdapterForDomesticDataWorkCraftIndustrialisedComputingNetworking0023CompactAdapter() => new()
	{
		Metadata = new("Computing and networking/make compact natural rubber adapter for domestic data work [craft_industrialised_computing_networking_0023_compact_adapter]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0023_compact_adapter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber adapter for domestic data work [craft_industrialised_computing_networking_0023_compact_adapter]",
		Category = "Computing and networking",
		Blurb = "Make a compact natural rubber adapter for domestic data work from prepared natural rubber stock.",
		Action = "making compact natural rubber adapter for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber adapter for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1880.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1880 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0023_compact_adapter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber adapter for domestic data work (@industrialised_computing_networking_0023_compact_adapter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactMildSteelSwitchForDomesticDataWorkCraftIndustrialisedComputingNetworking0024CompactSwitch() => new()
	{
		Metadata = new("Computing and networking/make compact mild steel switch for domestic data work [craft_industrialised_computing_networking_0024_compact_switch]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0024_compact_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel switch for domestic data work [craft_industrialised_computing_networking_0024_compact_switch]",
		Category = "Computing and networking",
		Blurb = "Make a compact mild steel switch for domestic data work from prepared mild steel stock.",
		Action = "making compact mild steel switch for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact mild steel switch for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1921.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1921 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0024_compact_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel switch for domestic data work (@industrialised_computing_networking_0024_compact_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactCopperCabinetForDomesticDataWorkCraftIndustrialisedComputingNetworking0025CompactCabinet() => new()
	{
		Metadata = new("Computing and networking/make compact copper cabinet for domestic data work [craft_industrialised_computing_networking_0025_compact_cabinet]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0025_compact_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper cabinet for domestic data work [craft_industrialised_computing_networking_0025_compact_cabinet]",
		Category = "Computing and networking",
		Blurb = "Make a compact copper cabinet for domestic data work from prepared copper stock.",
		Action = "making compact copper cabinet for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact copper cabinet for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1961.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1961 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0025_compact_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper cabinet for domestic data work (@industrialised_computing_networking_0025_compact_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactGlassConsoleForDomesticDataWorkCraftIndustrialisedComputingNetworking0026CompactConsole() => new()
	{
		Metadata = new("Computing and networking/make compact glass console for domestic data work [craft_industrialised_computing_networking_0026_compact_console]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0026_compact_console"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass console for domestic data work [craft_industrialised_computing_networking_0026_compact_console]",
		Category = "Computing and networking",
		Blurb = "Make a compact glass console for domestic data work from prepared glass stock.",
		Action = "making compact glass console for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact glass console for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2002.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2002 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0026_compact_console", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass console for domestic data work (@industrialised_computing_networking_0026_compact_console)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactNaturalRubberReaderForDomesticDataWorkCraftIndustrialisedComputingNetworking0027CompactReader() => new()
	{
		Metadata = new("Computing and networking/make compact natural rubber reader for domestic data work [craft_industrialised_computing_networking_0027_compact_reader]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0027_compact_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber reader for domestic data work [craft_industrialised_computing_networking_0027_compact_reader]",
		Category = "Computing and networking",
		Blurb = "Make a compact natural rubber reader for domestic data work from prepared natural rubber stock.",
		Action = "making compact natural rubber reader for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber reader for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2043.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2043 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0027_compact_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber reader for domestic data work (@industrialised_computing_networking_0027_compact_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactMildSteelKeyboardForDomesticDataWorkCraftIndustrialisedComputingNetworking0028CompactKeyboard() => new()
	{
		Metadata = new("Computing and networking/make compact mild steel keyboard for domestic data work [craft_industrialised_computing_networking_0028_compact_keyboard]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0028_compact_keyboard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel keyboard for domestic data work [craft_industrialised_computing_networking_0028_compact_keyboard]",
		Category = "Computing and networking",
		Blurb = "Make a compact mild steel keyboard for domestic data work from prepared mild steel stock.",
		Action = "making compact mild steel keyboard for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact mild steel keyboard for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2083.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2083 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0028_compact_keyboard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel keyboard for domestic data work (@industrialised_computing_networking_0028_compact_keyboard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactCopperMonitorForDomesticDataWorkCraftIndustrialisedComputingNetworking0029CompactMonitor() => new()
	{
		Metadata = new("Computing and networking/make compact copper monitor for domestic data work [craft_industrialised_computing_networking_0029_compact_monitor]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0029_compact_monitor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper monitor for domestic data work [craft_industrialised_computing_networking_0029_compact_monitor]",
		Category = "Computing and networking",
		Blurb = "Make a compact copper monitor for domestic data work from prepared copper stock.",
		Action = "making compact copper monitor for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact copper monitor for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2124.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2124 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0029_compact_monitor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper monitor for domestic data work (@industrialised_computing_networking_0029_compact_monitor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeCompactGlassCableForDomesticDataWorkCraftIndustrialisedComputingNetworking0030CompactCable() => new()
	{
		Metadata = new("Computing and networking/make compact glass cable for domestic data work [craft_industrialised_computing_networking_0030_compact_cable]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0030_compact_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass cable for domestic data work [craft_industrialised_computing_networking_0030_compact_cable]",
		Category = "Computing and networking",
		Blurb = "Make a compact glass cable for domestic data work from prepared glass stock.",
		Action = "making compact glass cable for domestic data work",
		ActiveCraftItemSdesc = "an in-progress compact glass cable for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2165.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2165 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0030_compact_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass cable for domestic data work (@industrialised_computing_networking_0030_compact_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyNaturalRubberScrapForDomesticDataWorkCraftIndustrialisedComputingNetworking0031HeavyDutyScrap() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty natural rubber scrap for domestic data work [craft_industrialised_computing_networking_0031_heavy_duty_scrap]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber scrap for domestic data work [craft_industrialised_computing_networking_0031_heavy_duty_scrap]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty natural rubber scrap for domestic data work from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber scrap for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber scrap for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2206.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2206 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber scrap for domestic data work (@industrialised_computing_networking_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyMildSteelTerminalForDomesticDataWorkCraftIndustrialisedComputingNetworking0032HeavyDutyTerminal() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty mild steel terminal for domestic data work [craft_industrialised_computing_networking_0032_heavy_duty_terminal]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0032_heavy_duty_terminal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel terminal for domestic data work [craft_industrialised_computing_networking_0032_heavy_duty_terminal]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty mild steel terminal for domestic data work from prepared mild steel stock.",
		Action = "making heavy-duty mild steel terminal for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel terminal for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2246.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2246 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0032_heavy_duty_terminal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel terminal for domestic data work (@industrialised_computing_networking_0032_heavy_duty_terminal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyCopperAdapterForDomesticDataWorkCraftIndustrialisedComputingNetworking0033HeavyDutyAdapter() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty copper adapter for domestic data work [craft_industrialised_computing_networking_0033_heavy_duty_adapter]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0033_heavy_duty_adapter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper adapter for domestic data work [craft_industrialised_computing_networking_0033_heavy_duty_adapter]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty copper adapter for domestic data work from prepared copper stock.",
		Action = "making heavy-duty copper adapter for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper adapter for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2287.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2287 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0033_heavy_duty_adapter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper adapter for domestic data work (@industrialised_computing_networking_0033_heavy_duty_adapter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyGlassSwitchForDomesticDataWorkCraftIndustrialisedComputingNetworking0034HeavyDutySwitch() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty glass switch for domestic data work [craft_industrialised_computing_networking_0034_heavy_duty_switch]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0034_heavy_duty_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass switch for domestic data work [craft_industrialised_computing_networking_0034_heavy_duty_switch]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty glass switch for domestic data work from prepared glass stock.",
		Action = "making heavy-duty glass switch for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass switch for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2328.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2328 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0034_heavy_duty_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass switch for domestic data work (@industrialised_computing_networking_0034_heavy_duty_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyNaturalRubberCabinetForDomesticDataWorkCraftIndustrialisedComputingNetworking0035HeavyDutyCabinet() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty natural rubber cabinet for domestic data work [craft_industrialised_computing_networking_0035_heavy_duty_cabinet]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0035_heavy_duty_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber cabinet for domestic data work [craft_industrialised_computing_networking_0035_heavy_duty_cabinet]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty natural rubber cabinet for domestic data work from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber cabinet for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber cabinet for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2368.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2368 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0035_heavy_duty_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber cabinet for domestic data work (@industrialised_computing_networking_0035_heavy_duty_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyMildSteelConsoleForDomesticDataWorkCraftIndustrialisedComputingNetworking0036HeavyDutyConsole() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty mild steel console for domestic data work [craft_industrialised_computing_networking_0036_heavy_duty_console]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0036_heavy_duty_console"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel console for domestic data work [craft_industrialised_computing_networking_0036_heavy_duty_console]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty mild steel console for domestic data work from prepared mild steel stock.",
		Action = "making heavy-duty mild steel console for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel console for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2409.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2409 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0036_heavy_duty_console", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel console for domestic data work (@industrialised_computing_networking_0036_heavy_duty_console)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyCopperReaderForDomesticDataWorkCraftIndustrialisedComputingNetworking0037HeavyDutyReader() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty copper reader for domestic data work [craft_industrialised_computing_networking_0037_heavy_duty_reader]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0037_heavy_duty_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper reader for domestic data work [craft_industrialised_computing_networking_0037_heavy_duty_reader]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty copper reader for domestic data work from prepared copper stock.",
		Action = "making heavy-duty copper reader for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper reader for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2450.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2450 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0037_heavy_duty_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper reader for domestic data work (@industrialised_computing_networking_0037_heavy_duty_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyGlassKeyboardForDomesticDataWorkCraftIndustrialisedComputingNetworking0038HeavyDutyKeyboard() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty glass keyboard for domestic data work [craft_industrialised_computing_networking_0038_heavy_duty_keyboard]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0038_heavy_duty_keyboard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass keyboard for domestic data work [craft_industrialised_computing_networking_0038_heavy_duty_keyboard]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty glass keyboard for domestic data work from prepared glass stock.",
		Action = "making heavy-duty glass keyboard for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass keyboard for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2490.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2490 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0038_heavy_duty_keyboard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass keyboard for domestic data work (@industrialised_computing_networking_0038_heavy_duty_keyboard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyNaturalRubberMonitorForDomesticDataWorkCraftIndustrialisedComputingNetworking0039HeavyDutyMonitor() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty natural rubber monitor for domestic data work [craft_industrialised_computing_networking_0039_heavy_duty_monitor]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0039_heavy_duty_monitor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber monitor for domestic data work [craft_industrialised_computing_networking_0039_heavy_duty_monitor]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty natural rubber monitor for domestic data work from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber monitor for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber monitor for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2531.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2531 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0039_heavy_duty_monitor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber monitor for domestic data work (@industrialised_computing_networking_0039_heavy_duty_monitor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeHeavyDutyMildSteelCableForDomesticDataWorkCraftIndustrialisedComputingNetworking0040HeavyDutyCable() => new()
	{
		Metadata = new("Computing and networking/make heavy-duty mild steel cable for domestic data work [craft_industrialised_computing_networking_0040_heavy_duty_cable]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0040_heavy_duty_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel cable for domestic data work [craft_industrialised_computing_networking_0040_heavy_duty_cable]",
		Category = "Computing and networking",
		Blurb = "Make a heavy-duty mild steel cable for domestic data work from prepared mild steel stock.",
		Action = "making heavy-duty mild steel cable for domestic data work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel cable for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2572.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2572 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0040_heavy_duty_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel cable for domestic data work (@industrialised_computing_networking_0040_heavy_duty_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingCopperScrapForDomesticDataWorkCraftIndustrialisedComputingNetworking0041FoldingScrap() => new()
	{
		Metadata = new("Computing and networking/make folding copper scrap for domestic data work [craft_industrialised_computing_networking_0041_folding_scrap]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0041_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper scrap for domestic data work [craft_industrialised_computing_networking_0041_folding_scrap]",
		Category = "Computing and networking",
		Blurb = "Make a folding copper scrap for domestic data work from prepared copper stock.",
		Action = "making folding copper scrap for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding copper scrap for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2612.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2612 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0041_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper scrap for domestic data work (@industrialised_computing_networking_0041_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingGlassTerminalForDomesticDataWorkCraftIndustrialisedComputingNetworking0042FoldingTerminal() => new()
	{
		Metadata = new("Computing and networking/make folding glass terminal for domestic data work [craft_industrialised_computing_networking_0042_folding_terminal]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0042_folding_terminal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass terminal for domestic data work [craft_industrialised_computing_networking_0042_folding_terminal]",
		Category = "Computing and networking",
		Blurb = "Make a folding glass terminal for domestic data work from prepared glass stock.",
		Action = "making folding glass terminal for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding glass terminal for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2653.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2653 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0042_folding_terminal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass terminal for domestic data work (@industrialised_computing_networking_0042_folding_terminal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingNaturalRubberAdapterForDomesticDataWorkCraftIndustrialisedComputingNetworking0043FoldingAdapter() => new()
	{
		Metadata = new("Computing and networking/make folding natural rubber adapter for domestic data work [craft_industrialised_computing_networking_0043_folding_adapter]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0043_folding_adapter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber adapter for domestic data work [craft_industrialised_computing_networking_0043_folding_adapter]",
		Category = "Computing and networking",
		Blurb = "Make a folding natural rubber adapter for domestic data work from prepared natural rubber stock.",
		Action = "making folding natural rubber adapter for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber adapter for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2694.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2694 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0043_folding_adapter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber adapter for domestic data work (@industrialised_computing_networking_0043_folding_adapter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingMildSteelSwitchForDomesticDataWorkCraftIndustrialisedComputingNetworking0044FoldingSwitch() => new()
	{
		Metadata = new("Computing and networking/make folding mild steel switch for domestic data work [craft_industrialised_computing_networking_0044_folding_switch]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0044_folding_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel switch for domestic data work [craft_industrialised_computing_networking_0044_folding_switch]",
		Category = "Computing and networking",
		Blurb = "Make a folding mild steel switch for domestic data work from prepared mild steel stock.",
		Action = "making folding mild steel switch for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding mild steel switch for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2735.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2735 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0044_folding_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel switch for domestic data work (@industrialised_computing_networking_0044_folding_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingCopperCabinetForDomesticDataWorkCraftIndustrialisedComputingNetworking0045FoldingCabinet() => new()
	{
		Metadata = new("Computing and networking/make folding copper cabinet for domestic data work [craft_industrialised_computing_networking_0045_folding_cabinet]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0045_folding_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper cabinet for domestic data work [craft_industrialised_computing_networking_0045_folding_cabinet]",
		Category = "Computing and networking",
		Blurb = "Make a folding copper cabinet for domestic data work from prepared copper stock.",
		Action = "making folding copper cabinet for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding copper cabinet for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2775.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2775 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0045_folding_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper cabinet for domestic data work (@industrialised_computing_networking_0045_folding_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingGlassConsoleForDomesticDataWorkCraftIndustrialisedComputingNetworking0046FoldingConsole() => new()
	{
		Metadata = new("Computing and networking/make folding glass console for domestic data work [craft_industrialised_computing_networking_0046_folding_console]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0046_folding_console"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass console for domestic data work [craft_industrialised_computing_networking_0046_folding_console]",
		Category = "Computing and networking",
		Blurb = "Make a folding glass console for domestic data work from prepared glass stock.",
		Action = "making folding glass console for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding glass console for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2816.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2816 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0046_folding_console", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass console for domestic data work (@industrialised_computing_networking_0046_folding_console)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingNaturalRubberReaderForDomesticDataWorkCraftIndustrialisedComputingNetworking0047FoldingReader() => new()
	{
		Metadata = new("Computing and networking/make folding natural rubber reader for domestic data work [craft_industrialised_computing_networking_0047_folding_reader]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0047_folding_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber reader for domestic data work [craft_industrialised_computing_networking_0047_folding_reader]",
		Category = "Computing and networking",
		Blurb = "Make a folding natural rubber reader for domestic data work from prepared natural rubber stock.",
		Action = "making folding natural rubber reader for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber reader for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2857.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2857 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0047_folding_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber reader for domestic data work (@industrialised_computing_networking_0047_folding_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingMildSteelKeyboardForDomesticDataWorkCraftIndustrialisedComputingNetworking0048FoldingKeyboard() => new()
	{
		Metadata = new("Computing and networking/make folding mild steel keyboard for domestic data work [craft_industrialised_computing_networking_0048_folding_keyboard]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0048_folding_keyboard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel keyboard for domestic data work [craft_industrialised_computing_networking_0048_folding_keyboard]",
		Category = "Computing and networking",
		Blurb = "Make a folding mild steel keyboard for domestic data work from prepared mild steel stock.",
		Action = "making folding mild steel keyboard for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding mild steel keyboard for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2897.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2897 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0048_folding_keyboard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel keyboard for domestic data work (@industrialised_computing_networking_0048_folding_keyboard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingCopperMonitorForDomesticDataWorkCraftIndustrialisedComputingNetworking0049FoldingMonitor() => new()
	{
		Metadata = new("Computing and networking/make folding copper monitor for domestic data work [craft_industrialised_computing_networking_0049_folding_monitor]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0049_folding_monitor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper monitor for domestic data work [craft_industrialised_computing_networking_0049_folding_monitor]",
		Category = "Computing and networking",
		Blurb = "Make a folding copper monitor for domestic data work from prepared copper stock.",
		Action = "making folding copper monitor for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding copper monitor for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2938.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2938 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0049_folding_monitor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper monitor for domestic data work (@industrialised_computing_networking_0049_folding_monitor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeFoldingGlassCableForDomesticDataWorkCraftIndustrialisedComputingNetworking0050FoldingCable() => new()
	{
		Metadata = new("Computing and networking/make folding glass cable for domestic data work [craft_industrialised_computing_networking_0050_folding_cable]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0050_folding_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass cable for domestic data work [craft_industrialised_computing_networking_0050_folding_cable]",
		Category = "Computing and networking",
		Blurb = "Make a folding glass cable for domestic data work from prepared glass stock.",
		Action = "making folding glass cable for domestic data work",
		ActiveCraftItemSdesc = "an in-progress folding glass cable for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2979.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2979 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0050_folding_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass cable for domestic data work (@industrialised_computing_networking_0050_folding_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableNaturalRubberScrapForDomesticDataWorkCraftIndustrialisedComputingNetworking0051PortableScrap() => new()
	{
		Metadata = new("Computing and networking/make portable natural rubber scrap for domestic data work [craft_industrialised_computing_networking_0051_portable_scrap]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0051_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable natural rubber scrap for domestic data work [craft_industrialised_computing_networking_0051_portable_scrap]",
		Category = "Computing and networking",
		Blurb = "Make a portable natural rubber scrap for domestic data work from prepared natural rubber stock.",
		Action = "making portable natural rubber scrap for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable natural rubber scrap for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3020.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3020 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0051_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable natural rubber scrap for domestic data work (@industrialised_computing_networking_0051_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableMildSteelTerminalForDomesticDataWorkCraftIndustrialisedComputingNetworking0052PortableTerminal() => new()
	{
		Metadata = new("Computing and networking/make portable mild steel terminal for domestic data work [craft_industrialised_computing_networking_0052_portable_terminal]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0052_portable_terminal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel terminal for domestic data work [craft_industrialised_computing_networking_0052_portable_terminal]",
		Category = "Computing and networking",
		Blurb = "Make a portable mild steel terminal for domestic data work from prepared mild steel stock.",
		Action = "making portable mild steel terminal for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable mild steel terminal for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3060.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3060 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0052_portable_terminal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel terminal for domestic data work (@industrialised_computing_networking_0052_portable_terminal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableCopperAdapterForDomesticDataWorkCraftIndustrialisedComputingNetworking0053PortableAdapter() => new()
	{
		Metadata = new("Computing and networking/make portable copper adapter for domestic data work [craft_industrialised_computing_networking_0053_portable_adapter]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0053_portable_adapter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper adapter for domestic data work [craft_industrialised_computing_networking_0053_portable_adapter]",
		Category = "Computing and networking",
		Blurb = "Make a portable copper adapter for domestic data work from prepared copper stock.",
		Action = "making portable copper adapter for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable copper adapter for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3101.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3101 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0053_portable_adapter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper adapter for domestic data work (@industrialised_computing_networking_0053_portable_adapter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableGlassSwitchForDomesticDataWorkCraftIndustrialisedComputingNetworking0054PortableSwitch() => new()
	{
		Metadata = new("Computing and networking/make portable glass switch for domestic data work [craft_industrialised_computing_networking_0054_portable_switch]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0054_portable_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable glass switch for domestic data work [craft_industrialised_computing_networking_0054_portable_switch]",
		Category = "Computing and networking",
		Blurb = "Make a portable glass switch for domestic data work from prepared glass stock.",
		Action = "making portable glass switch for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable glass switch for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3142.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3142 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0054_portable_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable glass switch for domestic data work (@industrialised_computing_networking_0054_portable_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableNaturalRubberCabinetForDomesticDataWorkCraftIndustrialisedComputingNetworking0055PortableCabinet() => new()
	{
		Metadata = new("Computing and networking/make portable natural rubber cabinet for domestic data work [craft_industrialised_computing_networking_0055_portable_cabinet]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0055_portable_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable natural rubber cabinet for domestic data work [craft_industrialised_computing_networking_0055_portable_cabinet]",
		Category = "Computing and networking",
		Blurb = "Make a portable natural rubber cabinet for domestic data work from prepared natural rubber stock.",
		Action = "making portable natural rubber cabinet for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable natural rubber cabinet for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3182.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3182 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0055_portable_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable natural rubber cabinet for domestic data work (@industrialised_computing_networking_0055_portable_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableMildSteelConsoleForDomesticDataWorkCraftIndustrialisedComputingNetworking0056PortableConsole() => new()
	{
		Metadata = new("Computing and networking/make portable mild steel console for domestic data work [craft_industrialised_computing_networking_0056_portable_console]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0056_portable_console"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel console for domestic data work [craft_industrialised_computing_networking_0056_portable_console]",
		Category = "Computing and networking",
		Blurb = "Make a portable mild steel console for domestic data work from prepared mild steel stock.",
		Action = "making portable mild steel console for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable mild steel console for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3223.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3223 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0056_portable_console", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel console for domestic data work (@industrialised_computing_networking_0056_portable_console)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableCopperReaderForDomesticDataWorkCraftIndustrialisedComputingNetworking0057PortableReader() => new()
	{
		Metadata = new("Computing and networking/make portable copper reader for domestic data work [craft_industrialised_computing_networking_0057_portable_reader]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0057_portable_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper reader for domestic data work [craft_industrialised_computing_networking_0057_portable_reader]",
		Category = "Computing and networking",
		Blurb = "Make a portable copper reader for domestic data work from prepared copper stock.",
		Action = "making portable copper reader for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable copper reader for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3264.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3264 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0057_portable_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper reader for domestic data work (@industrialised_computing_networking_0057_portable_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableGlassKeyboardForDomesticDataWorkCraftIndustrialisedComputingNetworking0058PortableKeyboard() => new()
	{
		Metadata = new("Computing and networking/make portable glass keyboard for domestic data work [craft_industrialised_computing_networking_0058_portable_keyboard]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0058_portable_keyboard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable glass keyboard for domestic data work [craft_industrialised_computing_networking_0058_portable_keyboard]",
		Category = "Computing and networking",
		Blurb = "Make a portable glass keyboard for domestic data work from prepared glass stock.",
		Action = "making portable glass keyboard for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable glass keyboard for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3304.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3304 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0058_portable_keyboard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable glass keyboard for domestic data work (@industrialised_computing_networking_0058_portable_keyboard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableNaturalRubberMonitorForDomesticDataWorkCraftIndustrialisedComputingNetworking0059PortableMonitor() => new()
	{
		Metadata = new("Computing and networking/make portable natural rubber monitor for domestic data work [craft_industrialised_computing_networking_0059_portable_monitor]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0059_portable_monitor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable natural rubber monitor for domestic data work [craft_industrialised_computing_networking_0059_portable_monitor]",
		Category = "Computing and networking",
		Blurb = "Make a portable natural rubber monitor for domestic data work from prepared natural rubber stock.",
		Action = "making portable natural rubber monitor for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable natural rubber monitor for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3345.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3345 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0059_portable_monitor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable natural rubber monitor for domestic data work (@industrialised_computing_networking_0059_portable_monitor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePortableMildSteelCableForDomesticDataWorkCraftIndustrialisedComputingNetworking0060PortableCable() => new()
	{
		Metadata = new("Computing and networking/make portable mild steel cable for domestic data work [craft_industrialised_computing_networking_0060_portable_cable]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0060_portable_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel cable for domestic data work [craft_industrialised_computing_networking_0060_portable_cable]",
		Category = "Computing and networking",
		Blurb = "Make a portable mild steel cable for domestic data work from prepared mild steel stock.",
		Action = "making portable mild steel cable for domestic data work",
		ActiveCraftItemSdesc = "an in-progress portable mild steel cable for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3386.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3386 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0060_portable_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel cable for domestic data work (@industrialised_computing_networking_0060_portable_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeWorkshopCopperScrapForDomesticDataWorkCraftIndustrialisedComputingNetworking0061WorkshopScrap() => new()
	{
		Metadata = new("Computing and networking/make workshop copper scrap for domestic data work [craft_industrialised_computing_networking_0061_workshop_scrap]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0061_workshop_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop copper scrap for domestic data work [craft_industrialised_computing_networking_0061_workshop_scrap]",
		Category = "Computing and networking",
		Blurb = "Make a workshop copper scrap for domestic data work from prepared copper stock.",
		Action = "making workshop copper scrap for domestic data work",
		ActiveCraftItemSdesc = "an in-progress workshop copper scrap for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3427.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3427 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0061_workshop_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop copper scrap for domestic data work (@industrialised_computing_networking_0061_workshop_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
