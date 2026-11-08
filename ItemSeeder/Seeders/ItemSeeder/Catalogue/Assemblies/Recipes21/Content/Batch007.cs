#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyCeramicSocketForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0003HeavyDutySocket() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty ceramic socket for workshop power service [craft_industrial_electrical_power_lighting_0003_heavy_duty_socket]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0003_heavy_duty_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic socket for workshop power service [craft_industrial_electrical_power_lighting_0003_heavy_duty_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty ceramic socket for workshop power service from prepared ceramic stock.",
		Action = "making heavy-duty ceramic socket for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic socket for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1288.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1288 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0003_heavy_duty_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic socket for workshop power service (@industrial_electrical_power_lighting_0003_heavy_duty_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyNaturalRubberSwitchForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0004HeavyDutySwitch() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty natural rubber switch for workshop power service [craft_industrial_electrical_power_lighting_0004_heavy_duty_switch]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0004_heavy_duty_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber switch for workshop power service [craft_industrial_electrical_power_lighting_0004_heavy_duty_switch]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty natural rubber switch for workshop power service from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber switch for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber switch for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1329.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1329 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0004_heavy_duty_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber switch for workshop power service (@industrial_electrical_power_lighting_0004_heavy_duty_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyCopperFuseForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0005HeavyDutyFuse() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty copper fuse for workshop power service [craft_industrial_electrical_power_lighting_0005_heavy_duty_fuse]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0005_heavy_duty_fuse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper fuse for workshop power service [craft_industrial_electrical_power_lighting_0005_heavy_duty_fuse]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty copper fuse for workshop power service from prepared copper stock.",
		Action = "making heavy-duty copper fuse for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper fuse for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1370.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1370 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0005_heavy_duty_fuse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper fuse for workshop power service (@industrial_electrical_power_lighting_0005_heavy_duty_fuse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyMildSteelLanternForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0006HeavyDutyLantern() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty mild steel lantern for workshop power service [craft_industrial_electrical_power_lighting_0006_heavy_duty_lantern]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0006_heavy_duty_lantern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel lantern for workshop power service [craft_industrial_electrical_power_lighting_0006_heavy_duty_lantern]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty mild steel lantern for workshop power service from prepared mild steel stock.",
		Action = "making heavy-duty mild steel lantern for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel lantern for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1410.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1410 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0006_heavy_duty_lantern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel lantern for workshop power service (@industrial_electrical_power_lighting_0006_heavy_duty_lantern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyGlassLeadForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0007HeavyDutyLead() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty glass lead for workshop power service [craft_industrial_electrical_power_lighting_0007_heavy_duty_lead]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0007_heavy_duty_lead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass lead for workshop power service [craft_industrial_electrical_power_lighting_0007_heavy_duty_lead]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty glass lead for workshop power service from prepared glass stock.",
		Action = "making heavy-duty glass lead for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass lead for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1451.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1451 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0007_heavy_duty_lead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass lead for workshop power service (@industrial_electrical_power_lighting_0007_heavy_duty_lead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyCeramicJunctionForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0008HeavyDutyJunction() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty ceramic junction for workshop power service [craft_industrial_electrical_power_lighting_0008_heavy_duty_junction]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0008_heavy_duty_junction"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic junction for workshop power service [craft_industrial_electrical_power_lighting_0008_heavy_duty_junction]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty ceramic junction for workshop power service from prepared ceramic stock.",
		Action = "making heavy-duty ceramic junction for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic junction for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1492.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1492 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0008_heavy_duty_junction", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic junction for workshop power service (@industrial_electrical_power_lighting_0008_heavy_duty_junction)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyNaturalRubberMeterForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0009HeavyDutyMeter() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty natural rubber meter for workshop power service [craft_industrial_electrical_power_lighting_0009_heavy_duty_meter]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0009_heavy_duty_meter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber meter for workshop power service [craft_industrial_electrical_power_lighting_0009_heavy_duty_meter]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty natural rubber meter for workshop power service from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber meter for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber meter for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1532.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1532 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0009_heavy_duty_meter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber meter for workshop power service (@industrial_electrical_power_lighting_0009_heavy_duty_meter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyCopperTransformerForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0010HeavyDutyTransformer() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty copper transformer for workshop power service [craft_industrial_electrical_power_lighting_0010_heavy_duty_transformer]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0010_heavy_duty_transformer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper transformer for workshop power service [craft_industrial_electrical_power_lighting_0010_heavy_duty_transformer]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty copper transformer for workshop power service from prepared copper stock.",
		Action = "making heavy-duty copper transformer for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper transformer for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1573.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1573 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0010_heavy_duty_transformer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper transformer for workshop power service (@industrial_electrical_power_lighting_0010_heavy_duty_transformer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingMildSteelScrapForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0011FoldingScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make folding mild steel scrap for workshop power service [craft_industrial_electrical_power_lighting_0011_folding_scrap]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0011_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel scrap for workshop power service [craft_industrial_electrical_power_lighting_0011_folding_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding mild steel scrap for workshop power service from prepared mild steel stock.",
		Action = "making folding mild steel scrap for workshop power service",
		ActiveCraftItemSdesc = "an in-progress folding mild steel scrap for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1614.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1614 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0011_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel scrap for workshop power service (@industrial_electrical_power_lighting_0011_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainMildSteelScrapForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0001PlainScrap() => new()
	{
		Metadata = new("Automation and access control/make plain mild steel scrap for domestic site control [craft_industrialised_automation_access_control_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel scrap for domestic site control [craft_industrialised_automation_access_control_0001_plain_scrap]",
		Category = "Automation and access control",
		Blurb = "Make a plain mild steel scrap for domestic site control from prepared mild steel stock.",
		Action = "making plain mild steel scrap for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain mild steel scrap for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1318.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1318 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel scrap for domestic site control (@industrialised_automation_access_control_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainCopperKeycardForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0002PlainKeycard() => new()
	{
		Metadata = new("Automation and access control/make plain copper keycard for domestic site control [craft_industrialised_automation_access_control_0002_plain_keycard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0002_plain_keycard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper keycard for domestic site control [craft_industrialised_automation_access_control_0002_plain_keycard]",
		Category = "Automation and access control",
		Blurb = "Make a plain copper keycard for domestic site control from prepared copper stock.",
		Action = "making plain copper keycard for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain copper keycard for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1358.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1358 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0002_plain_keycard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper keycard for domestic site control (@industrialised_automation_access_control_0002_plain_keycard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainGlassReaderForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0003PlainReader() => new()
	{
		Metadata = new("Automation and access control/make plain glass reader for domestic site control [craft_industrialised_automation_access_control_0003_plain_reader]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0003_plain_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass reader for domestic site control [craft_industrialised_automation_access_control_0003_plain_reader]",
		Category = "Automation and access control",
		Blurb = "Make a plain glass reader for domestic site control from prepared glass stock.",
		Action = "making plain glass reader for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain glass reader for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1399.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1399 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0003_plain_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass reader for domestic site control (@industrialised_automation_access_control_0003_plain_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainMildSteelRelayForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0004PlainRelay() => new()
	{
		Metadata = new("Automation and access control/make plain mild steel relay for domestic site control [craft_industrialised_automation_access_control_0004_plain_relay]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0004_plain_relay"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel relay for domestic site control [craft_industrialised_automation_access_control_0004_plain_relay]",
		Category = "Automation and access control",
		Blurb = "Make a plain mild steel relay for domestic site control from prepared mild steel stock.",
		Action = "making plain mild steel relay for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain mild steel relay for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1440.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1440 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0004_plain_relay", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel relay for domestic site control (@industrialised_automation_access_control_0004_plain_relay)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainCopperAlarmForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0005PlainAlarm() => new()
	{
		Metadata = new("Automation and access control/make plain copper alarm for domestic site control [craft_industrialised_automation_access_control_0005_plain_alarm]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0005_plain_alarm"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper alarm for domestic site control [craft_industrialised_automation_access_control_0005_plain_alarm]",
		Category = "Automation and access control",
		Blurb = "Make a plain copper alarm for domestic site control from prepared copper stock.",
		Action = "making plain copper alarm for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain copper alarm for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1481.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1481 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0005_plain_alarm", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper alarm for domestic site control (@industrialised_automation_access_control_0005_plain_alarm)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainGlassSensorForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0006PlainSensor() => new()
	{
		Metadata = new("Automation and access control/make plain glass sensor for domestic site control [craft_industrialised_automation_access_control_0006_plain_sensor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0006_plain_sensor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass sensor for domestic site control [craft_industrialised_automation_access_control_0006_plain_sensor]",
		Category = "Automation and access control",
		Blurb = "Make a plain glass sensor for domestic site control from prepared glass stock.",
		Action = "making plain glass sensor for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain glass sensor for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1521.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1521 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0006_plain_sensor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass sensor for domestic site control (@industrialised_automation_access_control_0006_plain_sensor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainMildSteelLatchForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0007PlainLatch() => new()
	{
		Metadata = new("Automation and access control/make plain mild steel latch for domestic site control [craft_industrialised_automation_access_control_0007_plain_latch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0007_plain_latch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel latch for domestic site control [craft_industrialised_automation_access_control_0007_plain_latch]",
		Category = "Automation and access control",
		Blurb = "Make a plain mild steel latch for domestic site control from prepared mild steel stock.",
		Action = "making plain mild steel latch for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain mild steel latch for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1562.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1562 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0007_plain_latch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel latch for domestic site control (@industrialised_automation_access_control_0007_plain_latch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainCopperIndicatorForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0008PlainIndicator() => new()
	{
		Metadata = new("Automation and access control/make plain copper indicator for domestic site control [craft_industrialised_automation_access_control_0008_plain_indicator]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0008_plain_indicator"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper indicator for domestic site control [craft_industrialised_automation_access_control_0008_plain_indicator]",
		Category = "Automation and access control",
		Blurb = "Make a plain copper indicator for domestic site control from prepared copper stock.",
		Action = "making plain copper indicator for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain copper indicator for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1603.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1603 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0008_plain_indicator", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper indicator for domestic site control (@industrialised_automation_access_control_0008_plain_indicator)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainGlassPanelForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0009PlainPanel() => new()
	{
		Metadata = new("Automation and access control/make plain glass panel for domestic site control [craft_industrialised_automation_access_control_0009_plain_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0009_plain_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass panel for domestic site control [craft_industrialised_automation_access_control_0009_plain_panel]",
		Category = "Automation and access control",
		Blurb = "Make a plain glass panel for domestic site control from prepared glass stock.",
		Action = "making plain glass panel for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain glass panel for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1643.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1643 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0009_plain_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass panel for domestic site control (@industrialised_automation_access_control_0009_plain_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePlainMildSteelBellForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0010PlainBell() => new()
	{
		Metadata = new("Automation and access control/make plain mild steel bell for domestic site control [craft_industrialised_automation_access_control_0010_plain_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0010_plain_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel bell for domestic site control [craft_industrialised_automation_access_control_0010_plain_bell]",
		Category = "Automation and access control",
		Blurb = "Make a plain mild steel bell for domestic site control from prepared mild steel stock.",
		Action = "making plain mild steel bell for domestic site control",
		ActiveCraftItemSdesc = "an in-progress plain mild steel bell for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1684.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1684 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0010_plain_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel bell for domestic site control (@industrialised_automation_access_control_0010_plain_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedCopperScrapForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0011ReinforcedScrap() => new()
	{
		Metadata = new("Automation and access control/make reinforced copper scrap for domestic site control [craft_industrialised_automation_access_control_0011_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper scrap for domestic site control [craft_industrialised_automation_access_control_0011_reinforced_scrap]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced copper scrap for domestic site control from prepared copper stock.",
		Action = "making reinforced copper scrap for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced copper scrap for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1725.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1725 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper scrap for domestic site control (@industrialised_automation_access_control_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedGlassKeycardForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0012ReinforcedKeycard() => new()
	{
		Metadata = new("Automation and access control/make reinforced glass keycard for domestic site control [craft_industrialised_automation_access_control_0012_reinforced_keycard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0012_reinforced_keycard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass keycard for domestic site control [craft_industrialised_automation_access_control_0012_reinforced_keycard]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced glass keycard for domestic site control from prepared glass stock.",
		Action = "making reinforced glass keycard for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced glass keycard for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1766.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1766 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0012_reinforced_keycard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass keycard for domestic site control (@industrialised_automation_access_control_0012_reinforced_keycard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedMildSteelReaderForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0013ReinforcedReader() => new()
	{
		Metadata = new("Automation and access control/make reinforced mild steel reader for domestic site control [craft_industrialised_automation_access_control_0013_reinforced_reader]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0013_reinforced_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel reader for domestic site control [craft_industrialised_automation_access_control_0013_reinforced_reader]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced mild steel reader for domestic site control from prepared mild steel stock.",
		Action = "making reinforced mild steel reader for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel reader for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1806.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1806 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0013_reinforced_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel reader for domestic site control (@industrialised_automation_access_control_0013_reinforced_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedCopperRelayForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0014ReinforcedRelay() => new()
	{
		Metadata = new("Automation and access control/make reinforced copper relay for domestic site control [craft_industrialised_automation_access_control_0014_reinforced_relay]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0014_reinforced_relay"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper relay for domestic site control [craft_industrialised_automation_access_control_0014_reinforced_relay]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced copper relay for domestic site control from prepared copper stock.",
		Action = "making reinforced copper relay for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced copper relay for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1847.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1847 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0014_reinforced_relay", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper relay for domestic site control (@industrialised_automation_access_control_0014_reinforced_relay)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedGlassAlarmForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0015ReinforcedAlarm() => new()
	{
		Metadata = new("Automation and access control/make reinforced glass alarm for domestic site control [craft_industrialised_automation_access_control_0015_reinforced_alarm]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0015_reinforced_alarm"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass alarm for domestic site control [craft_industrialised_automation_access_control_0015_reinforced_alarm]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced glass alarm for domestic site control from prepared glass stock.",
		Action = "making reinforced glass alarm for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced glass alarm for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1888.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1888 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0015_reinforced_alarm", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass alarm for domestic site control (@industrialised_automation_access_control_0015_reinforced_alarm)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedMildSteelSensorForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0016ReinforcedSensor() => new()
	{
		Metadata = new("Automation and access control/make reinforced mild steel sensor for domestic site control [craft_industrialised_automation_access_control_0016_reinforced_sensor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0016_reinforced_sensor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel sensor for domestic site control [craft_industrialised_automation_access_control_0016_reinforced_sensor]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced mild steel sensor for domestic site control from prepared mild steel stock.",
		Action = "making reinforced mild steel sensor for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel sensor for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1928.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1928 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0016_reinforced_sensor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel sensor for domestic site control (@industrialised_automation_access_control_0016_reinforced_sensor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedCopperLatchForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0017ReinforcedLatch() => new()
	{
		Metadata = new("Automation and access control/make reinforced copper latch for domestic site control [craft_industrialised_automation_access_control_0017_reinforced_latch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0017_reinforced_latch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper latch for domestic site control [craft_industrialised_automation_access_control_0017_reinforced_latch]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced copper latch for domestic site control from prepared copper stock.",
		Action = "making reinforced copper latch for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced copper latch for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1969.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1969 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0017_reinforced_latch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper latch for domestic site control (@industrialised_automation_access_control_0017_reinforced_latch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedGlassIndicatorForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0018ReinforcedIndicator() => new()
	{
		Metadata = new("Automation and access control/make reinforced glass indicator for domestic site control [craft_industrialised_automation_access_control_0018_reinforced_indicator]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0018_reinforced_indicator"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass indicator for domestic site control [craft_industrialised_automation_access_control_0018_reinforced_indicator]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced glass indicator for domestic site control from prepared glass stock.",
		Action = "making reinforced glass indicator for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced glass indicator for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2010.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2010 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0018_reinforced_indicator", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass indicator for domestic site control (@industrialised_automation_access_control_0018_reinforced_indicator)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedMildSteelPanelForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0019ReinforcedPanel() => new()
	{
		Metadata = new("Automation and access control/make reinforced mild steel panel for domestic site control [craft_industrialised_automation_access_control_0019_reinforced_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0019_reinforced_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel panel for domestic site control [craft_industrialised_automation_access_control_0019_reinforced_panel]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced mild steel panel for domestic site control from prepared mild steel stock.",
		Action = "making reinforced mild steel panel for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel panel for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2050.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2050 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0019_reinforced_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel panel for domestic site control (@industrialised_automation_access_control_0019_reinforced_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeReinforcedCopperBellForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0020ReinforcedBell() => new()
	{
		Metadata = new("Automation and access control/make reinforced copper bell for domestic site control [craft_industrialised_automation_access_control_0020_reinforced_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0020_reinforced_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper bell for domestic site control [craft_industrialised_automation_access_control_0020_reinforced_bell]",
		Category = "Automation and access control",
		Blurb = "Make a reinforced copper bell for domestic site control from prepared copper stock.",
		Action = "making reinforced copper bell for domestic site control",
		ActiveCraftItemSdesc = "an in-progress reinforced copper bell for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2091.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2091 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0020_reinforced_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper bell for domestic site control (@industrialised_automation_access_control_0020_reinforced_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactGlassScrapForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0021CompactScrap() => new()
	{
		Metadata = new("Automation and access control/make compact glass scrap for domestic site control [craft_industrialised_automation_access_control_0021_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass scrap for domestic site control [craft_industrialised_automation_access_control_0021_compact_scrap]",
		Category = "Automation and access control",
		Blurb = "Make a compact glass scrap for domestic site control from prepared glass stock.",
		Action = "making compact glass scrap for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact glass scrap for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2132.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2132 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass scrap for domestic site control (@industrialised_automation_access_control_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactMildSteelKeycardForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0022CompactKeycard() => new()
	{
		Metadata = new("Automation and access control/make compact mild steel keycard for domestic site control [craft_industrialised_automation_access_control_0022_compact_keycard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0022_compact_keycard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel keycard for domestic site control [craft_industrialised_automation_access_control_0022_compact_keycard]",
		Category = "Automation and access control",
		Blurb = "Make a compact mild steel keycard for domestic site control from prepared mild steel stock.",
		Action = "making compact mild steel keycard for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact mild steel keycard for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2172.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2172 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0022_compact_keycard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel keycard for domestic site control (@industrialised_automation_access_control_0022_compact_keycard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactCopperReaderForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0023CompactReader() => new()
	{
		Metadata = new("Automation and access control/make compact copper reader for domestic site control [craft_industrialised_automation_access_control_0023_compact_reader]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0023_compact_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper reader for domestic site control [craft_industrialised_automation_access_control_0023_compact_reader]",
		Category = "Automation and access control",
		Blurb = "Make a compact copper reader for domestic site control from prepared copper stock.",
		Action = "making compact copper reader for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact copper reader for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2213.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2213 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0023_compact_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper reader for domestic site control (@industrialised_automation_access_control_0023_compact_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactGlassRelayForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0024CompactRelay() => new()
	{
		Metadata = new("Automation and access control/make compact glass relay for domestic site control [craft_industrialised_automation_access_control_0024_compact_relay]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0024_compact_relay"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass relay for domestic site control [craft_industrialised_automation_access_control_0024_compact_relay]",
		Category = "Automation and access control",
		Blurb = "Make a compact glass relay for domestic site control from prepared glass stock.",
		Action = "making compact glass relay for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact glass relay for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2254.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2254 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0024_compact_relay", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass relay for domestic site control (@industrialised_automation_access_control_0024_compact_relay)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactMildSteelAlarmForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0025CompactAlarm() => new()
	{
		Metadata = new("Automation and access control/make compact mild steel alarm for domestic site control [craft_industrialised_automation_access_control_0025_compact_alarm]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0025_compact_alarm"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel alarm for domestic site control [craft_industrialised_automation_access_control_0025_compact_alarm]",
		Category = "Automation and access control",
		Blurb = "Make a compact mild steel alarm for domestic site control from prepared mild steel stock.",
		Action = "making compact mild steel alarm for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact mild steel alarm for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2295.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2295 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0025_compact_alarm", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel alarm for domestic site control (@industrialised_automation_access_control_0025_compact_alarm)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactCopperSensorForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0026CompactSensor() => new()
	{
		Metadata = new("Automation and access control/make compact copper sensor for domestic site control [craft_industrialised_automation_access_control_0026_compact_sensor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0026_compact_sensor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper sensor for domestic site control [craft_industrialised_automation_access_control_0026_compact_sensor]",
		Category = "Automation and access control",
		Blurb = "Make a compact copper sensor for domestic site control from prepared copper stock.",
		Action = "making compact copper sensor for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact copper sensor for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2335.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2335 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0026_compact_sensor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper sensor for domestic site control (@industrialised_automation_access_control_0026_compact_sensor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactGlassLatchForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0027CompactLatch() => new()
	{
		Metadata = new("Automation and access control/make compact glass latch for domestic site control [craft_industrialised_automation_access_control_0027_compact_latch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0027_compact_latch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass latch for domestic site control [craft_industrialised_automation_access_control_0027_compact_latch]",
		Category = "Automation and access control",
		Blurb = "Make a compact glass latch for domestic site control from prepared glass stock.",
		Action = "making compact glass latch for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact glass latch for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2376.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2376 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0027_compact_latch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass latch for domestic site control (@industrialised_automation_access_control_0027_compact_latch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactMildSteelIndicatorForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0028CompactIndicator() => new()
	{
		Metadata = new("Automation and access control/make compact mild steel indicator for domestic site control [craft_industrialised_automation_access_control_0028_compact_indicator]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0028_compact_indicator"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel indicator for domestic site control [craft_industrialised_automation_access_control_0028_compact_indicator]",
		Category = "Automation and access control",
		Blurb = "Make a compact mild steel indicator for domestic site control from prepared mild steel stock.",
		Action = "making compact mild steel indicator for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact mild steel indicator for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2417.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2417 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0028_compact_indicator", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel indicator for domestic site control (@industrialised_automation_access_control_0028_compact_indicator)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactCopperPanelForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0029CompactPanel() => new()
	{
		Metadata = new("Automation and access control/make compact copper panel for domestic site control [craft_industrialised_automation_access_control_0029_compact_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0029_compact_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper panel for domestic site control [craft_industrialised_automation_access_control_0029_compact_panel]",
		Category = "Automation and access control",
		Blurb = "Make a compact copper panel for domestic site control from prepared copper stock.",
		Action = "making compact copper panel for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact copper panel for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2457.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2457 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0029_compact_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper panel for domestic site control (@industrialised_automation_access_control_0029_compact_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeCompactGlassBellForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0030CompactBell() => new()
	{
		Metadata = new("Automation and access control/make compact glass bell for domestic site control [craft_industrialised_automation_access_control_0030_compact_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0030_compact_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass bell for domestic site control [craft_industrialised_automation_access_control_0030_compact_bell]",
		Category = "Automation and access control",
		Blurb = "Make a compact glass bell for domestic site control from prepared glass stock.",
		Action = "making compact glass bell for domestic site control",
		ActiveCraftItemSdesc = "an in-progress compact glass bell for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2498.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2498 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0030_compact_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass bell for domestic site control (@industrialised_automation_access_control_0030_compact_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeHeavyDutyMildSteelScrapForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0031HeavyDutyScrap() => new()
	{
		Metadata = new("Automation and access control/make heavy-duty mild steel scrap for domestic site control [craft_industrialised_automation_access_control_0031_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for domestic site control [craft_industrialised_automation_access_control_0031_heavy_duty_scrap]",
		Category = "Automation and access control",
		Blurb = "Make a heavy-duty mild steel scrap for domestic site control from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for domestic site control",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2539.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2539 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for domestic site control (@industrialised_automation_access_control_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeHeavyDutyCopperKeycardForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0032HeavyDutyKeycard() => new()
	{
		Metadata = new("Automation and access control/make heavy-duty copper keycard for domestic site control [craft_industrialised_automation_access_control_0032_heavy_duty_keycard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0032_heavy_duty_keycard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper keycard for domestic site control [craft_industrialised_automation_access_control_0032_heavy_duty_keycard]",
		Category = "Automation and access control",
		Blurb = "Make a heavy-duty copper keycard for domestic site control from prepared copper stock.",
		Action = "making heavy-duty copper keycard for domestic site control",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper keycard for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2580.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2580 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0032_heavy_duty_keycard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper keycard for domestic site control (@industrialised_automation_access_control_0032_heavy_duty_keycard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeHeavyDutyGlassReaderForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0033HeavyDutyReader() => new()
	{
		Metadata = new("Automation and access control/make heavy-duty glass reader for domestic site control [craft_industrialised_automation_access_control_0033_heavy_duty_reader]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0033_heavy_duty_reader"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass reader for domestic site control [craft_industrialised_automation_access_control_0033_heavy_duty_reader]",
		Category = "Automation and access control",
		Blurb = "Make a heavy-duty glass reader for domestic site control from prepared glass stock.",
		Action = "making heavy-duty glass reader for domestic site control",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass reader for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2620.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2620 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0033_heavy_duty_reader", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass reader for domestic site control (@industrialised_automation_access_control_0033_heavy_duty_reader)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeHeavyDutyMildSteelRelayForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0034HeavyDutyRelay() => new()
	{
		Metadata = new("Automation and access control/make heavy-duty mild steel relay for domestic site control [craft_industrialised_automation_access_control_0034_heavy_duty_relay]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0034_heavy_duty_relay"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel relay for domestic site control [craft_industrialised_automation_access_control_0034_heavy_duty_relay]",
		Category = "Automation and access control",
		Blurb = "Make a heavy-duty mild steel relay for domestic site control from prepared mild steel stock.",
		Action = "making heavy-duty mild steel relay for domestic site control",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel relay for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2661.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2661 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0034_heavy_duty_relay", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel relay for domestic site control (@industrialised_automation_access_control_0034_heavy_duty_relay)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeHeavyDutyCopperAlarmForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0035HeavyDutyAlarm() => new()
	{
		Metadata = new("Automation and access control/make heavy-duty copper alarm for domestic site control [craft_industrialised_automation_access_control_0035_heavy_duty_alarm]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0035_heavy_duty_alarm"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper alarm for domestic site control [craft_industrialised_automation_access_control_0035_heavy_duty_alarm]",
		Category = "Automation and access control",
		Blurb = "Make a heavy-duty copper alarm for domestic site control from prepared copper stock.",
		Action = "making heavy-duty copper alarm for domestic site control",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper alarm for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2702.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2702 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0035_heavy_duty_alarm", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper alarm for domestic site control (@industrialised_automation_access_control_0035_heavy_duty_alarm)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakeHeavyDutyGlassSensorForDomesticSiteControlCraftIndustrialisedAutomationAccessControl0036HeavyDutySensor() => new()
	{
		Metadata = new("Automation and access control/make heavy-duty glass sensor for domestic site control [craft_industrialised_automation_access_control_0036_heavy_duty_sensor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0036_heavy_duty_sensor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass sensor for domestic site control [craft_industrialised_automation_access_control_0036_heavy_duty_sensor]",
		Category = "Automation and access control",
		Blurb = "Make a heavy-duty glass sensor for domestic site control from prepared glass stock.",
		Action = "making heavy-duty glass sensor for domestic site control",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass sensor for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2742.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2742 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_automation_access_control_0036_heavy_duty_sensor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass sensor for domestic site control (@industrialised_automation_access_control_0036_heavy_duty_sensor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePrecisionCopperScrapForDomesticSiteControlCraftIndustrialAutomationAccessControl0001PrecisionScrap() => new()
	{
		Metadata = new("Automation and access control/make precision copper scrap for domestic site control [craft_industrial_automation_access_control_0001_precision_scrap]", "crafts", ["industrial"], ["item:industrial_automation_access_control_0001_precision_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision copper scrap for domestic site control [craft_industrial_automation_access_control_0001_precision_scrap]",
		Category = "Automation and access control",
		Blurb = "Make a precision copper scrap for domestic site control from prepared copper stock.",
		Action = "making precision copper scrap for domestic site control",
		ActiveCraftItemSdesc = "an in-progress precision copper scrap for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1318.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1318 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_automation_access_control_0001_precision_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision copper scrap for domestic site control (@industrial_automation_access_control_0001_precision_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AutomationAndAccessControlMakePrecisionGlassKeycardForDomesticSiteControlCraftIndustrialAutomationAccessControl0002PrecisionKeycard() => new()
	{
		Metadata = new("Automation and access control/make precision glass keycard for domestic site control [craft_industrial_automation_access_control_0002_precision_keycard]", "crafts", ["industrial"], ["item:industrial_automation_access_control_0002_precision_keycard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision glass keycard for domestic site control [craft_industrial_automation_access_control_0002_precision_keycard]",
		Category = "Automation and access control",
		Blurb = "Make a precision glass keycard for domestic site control from prepared glass stock.",
		Action = "making precision glass keycard for domestic site control",
		ActiveCraftItemSdesc = "an in-progress precision glass keycard for domestic site control craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1358.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1358 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_automation_access_control_0002_precision_keycard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision glass keycard for domestic site control (@industrial_automation_access_control_0002_precision_keycard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0001PlainScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0001_plain_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making plain mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1429.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1429 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainCastIronHammerForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0002PlainHammer() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0002_plain_hammer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0002_plain_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron hammer for domestic maintenance [craft_industrialised_tools_repair_machinery_0002_plain_hammer]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain cast iron hammer for domestic maintenance from prepared cast iron stock.",
		Action = "making plain cast iron hammer for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain cast iron hammer for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1470.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1470 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0002_plain_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron hammer for domestic maintenance (@industrialised_tools_repair_machinery_0002_plain_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainOakWrenchForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0003PlainWrench() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0003_plain_wrench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0003_plain_wrench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak wrench for domestic maintenance [craft_industrialised_tools_repair_machinery_0003_plain_wrench]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain oak wrench for domestic maintenance from prepared oak stock.",
		Action = "making plain oak wrench for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain oak wrench for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1510.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1510 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0003_plain_wrench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak wrench for domestic maintenance (@industrialised_tools_repair_machinery_0003_plain_wrench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainToolSteelDrillForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0004PlainDrill() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0004_plain_drill]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0004_plain_drill"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain tool steel drill for domestic maintenance [craft_industrialised_tools_repair_machinery_0004_plain_drill]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain tool steel drill for domestic maintenance from prepared tool steel stock.",
		Action = "making plain tool steel drill for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain tool steel drill for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1551.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1551 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0004_plain_drill", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain tool steel drill for domestic maintenance (@industrialised_tools_repair_machinery_0004_plain_drill)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainMildSteelLatheForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0005PlainLathe() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0005_plain_lathe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0005_plain_lathe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel lathe for domestic maintenance [craft_industrialised_tools_repair_machinery_0005_plain_lathe]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain mild steel lathe for domestic maintenance from prepared mild steel stock.",
		Action = "making plain mild steel lathe for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain mild steel lathe for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1592.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1592 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0005_plain_lathe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel lathe for domestic maintenance (@industrialised_tools_repair_machinery_0005_plain_lathe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainCastIronViseForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0006PlainVise() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0006_plain_vise]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0006_plain_vise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron vise for domestic maintenance [craft_industrialised_tools_repair_machinery_0006_plain_vise]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain cast iron vise for domestic maintenance from prepared cast iron stock.",
		Action = "making plain cast iron vise for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain cast iron vise for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1632.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1632 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0006_plain_vise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron vise for domestic maintenance (@industrialised_tools_repair_machinery_0006_plain_vise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainOakPlaneForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0007PlainPlane() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0007_plain_plane]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0007_plain_plane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak plane for domestic maintenance [craft_industrialised_tools_repair_machinery_0007_plain_plane]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain oak plane for domestic maintenance from prepared oak stock.",
		Action = "making plain oak plane for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain oak plane for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1673.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1673 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0007_plain_plane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak plane for domestic maintenance (@industrialised_tools_repair_machinery_0007_plain_plane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainToolSteelSawForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0008PlainSaw() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0008_plain_saw]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0008_plain_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain tool steel saw for domestic maintenance [craft_industrialised_tools_repair_machinery_0008_plain_saw]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain tool steel saw for domestic maintenance from prepared tool steel stock.",
		Action = "making plain tool steel saw for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain tool steel saw for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1714.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1714 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0008_plain_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain tool steel saw for domestic maintenance (@industrialised_tools_repair_machinery_0008_plain_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainMildSteelPressForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0009PlainPress() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0009_plain_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0009_plain_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel press for domestic maintenance [craft_industrialised_tools_repair_machinery_0009_plain_press]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain mild steel press for domestic maintenance from prepared mild steel stock.",
		Action = "making plain mild steel press for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain mild steel press for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1755.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1755 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0009_plain_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel press for domestic maintenance (@industrialised_tools_repair_machinery_0009_plain_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainCastIronGaugeForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0010PlainGauge() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0010_plain_gauge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0010_plain_gauge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron gauge for domestic maintenance [craft_industrialised_tools_repair_machinery_0010_plain_gauge]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain cast iron gauge for domestic maintenance from prepared cast iron stock.",
		Action = "making plain cast iron gauge for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain cast iron gauge for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1795.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1795 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0010_plain_gauge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron gauge for domestic maintenance (@industrialised_tools_repair_machinery_0010_plain_gauge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainOakAnvilForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0011PlainAnvil() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0011_plain_anvil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0011_plain_anvil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak anvil for domestic maintenance [craft_industrialised_tools_repair_machinery_0011_plain_anvil]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain oak anvil for domestic maintenance from prepared oak stock.",
		Action = "making plain oak anvil for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain oak anvil for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1836.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1836 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0011_plain_anvil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak anvil for domestic maintenance (@industrialised_tools_repair_machinery_0011_plain_anvil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakePlainToolSteelGrinderForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0012PlainGrinder() => new()
	{
		Metadata = new("Tools, repair and machinery/make plain tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0012_plain_grinder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0012_plain_grinder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain tool steel grinder for domestic maintenance [craft_industrialised_tools_repair_machinery_0012_plain_grinder]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a plain tool steel grinder for domestic maintenance from prepared tool steel stock.",
		Action = "making plain tool steel grinder for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress plain tool steel grinder for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1877.0, Material: "tool steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1877 grams of tool steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0012_plain_grinder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain tool steel grinder for domestic maintenance (@industrialised_tools_repair_machinery_0012_plain_grinder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ToolsRepairAndMachineryMakeReinforcedMildSteelScrapForDomesticMaintenanceCraftIndustrialisedToolsRepairMachinery0013ReinforcedScrap() => new()
	{
		Metadata = new("Tools, repair and machinery/make reinforced mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0013_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_tools_repair_machinery_0013_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel scrap for domestic maintenance [craft_industrialised_tools_repair_machinery_0013_reinforced_scrap]",
		Category = "Tools, repair and machinery",
		Blurb = "Make a reinforced mild steel scrap for domestic maintenance from prepared mild steel stock.",
		Action = "making reinforced mild steel scrap for domestic maintenance",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel scrap for domestic maintenance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1917.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1917 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_tools_repair_machinery_0013_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel scrap for domestic maintenance (@industrialised_tools_repair_machinery_0013_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
