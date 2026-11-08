#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactMildSteelLanternForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0026CompactLantern() => new()
	{
		Metadata = new("Electrical power and lighting/make compact mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0026_compact_lantern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0026_compact_lantern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0026_compact_lantern]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact mild steel lantern for domestic power service from prepared mild steel stock.",
		Action = "making compact mild steel lantern for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact mild steel lantern for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2224.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2224 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0026_compact_lantern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel lantern for domestic power service (@industrialised_electrical_power_lighting_0026_compact_lantern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactGlassLeadForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0027CompactLead() => new()
	{
		Metadata = new("Electrical power and lighting/make compact glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0027_compact_lead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0027_compact_lead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0027_compact_lead]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact glass lead for domestic power service from prepared glass stock.",
		Action = "making compact glass lead for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact glass lead for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2265.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2265 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0027_compact_lead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass lead for domestic power service (@industrialised_electrical_power_lighting_0027_compact_lead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactCeramicJunctionForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0028CompactJunction() => new()
	{
		Metadata = new("Electrical power and lighting/make compact ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0028_compact_junction]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0028_compact_junction"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0028_compact_junction]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact ceramic junction for domestic power service from prepared ceramic stock.",
		Action = "making compact ceramic junction for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact ceramic junction for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2306.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2306 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0028_compact_junction", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic junction for domestic power service (@industrialised_electrical_power_lighting_0028_compact_junction)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactNaturalRubberMeterForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0029CompactMeter() => new()
	{
		Metadata = new("Electrical power and lighting/make compact natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0029_compact_meter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0029_compact_meter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0029_compact_meter]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact natural rubber meter for domestic power service from prepared natural rubber stock.",
		Action = "making compact natural rubber meter for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber meter for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2346.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2346 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0029_compact_meter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber meter for domestic power service (@industrialised_electrical_power_lighting_0029_compact_meter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactCopperTransformerForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0030CompactTransformer() => new()
	{
		Metadata = new("Electrical power and lighting/make compact copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0030_compact_transformer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0030_compact_transformer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0030_compact_transformer]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact copper transformer for domestic power service from prepared copper stock.",
		Action = "making compact copper transformer for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact copper transformer for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2387.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2387 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0030_compact_transformer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper transformer for domestic power service (@industrialised_electrical_power_lighting_0030_compact_transformer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyMildSteelScrapForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0031HeavyDutyScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0031_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0031_heavy_duty_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty mild steel scrap for domestic power service from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2428.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2428 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for domestic power service (@industrialised_electrical_power_lighting_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyGlassLampForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0032HeavyDutyLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0032_heavy_duty_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0032_heavy_duty_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0032_heavy_duty_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty glass lamp for domestic power service from prepared glass stock.",
		Action = "making heavy-duty glass lamp for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass lamp for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2468.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2468 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0032_heavy_duty_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass lamp for domestic power service (@industrialised_electrical_power_lighting_0032_heavy_duty_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyCeramicSocketForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0033HeavyDutySocket() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0033_heavy_duty_socket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0033_heavy_duty_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0033_heavy_duty_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty ceramic socket for domestic power service from prepared ceramic stock.",
		Action = "making heavy-duty ceramic socket for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic socket for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2509.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2509 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0033_heavy_duty_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic socket for domestic power service (@industrialised_electrical_power_lighting_0033_heavy_duty_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyNaturalRubberSwitchForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0034HeavyDutySwitch() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0034_heavy_duty_switch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0034_heavy_duty_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0034_heavy_duty_switch]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty natural rubber switch for domestic power service from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber switch for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber switch for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2550.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2550 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0034_heavy_duty_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber switch for domestic power service (@industrialised_electrical_power_lighting_0034_heavy_duty_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyCopperFuseForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0035HeavyDutyFuse() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0035_heavy_duty_fuse]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0035_heavy_duty_fuse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0035_heavy_duty_fuse]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty copper fuse for domestic power service from prepared copper stock.",
		Action = "making heavy-duty copper fuse for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper fuse for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2590.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2590 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0035_heavy_duty_fuse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper fuse for domestic power service (@industrialised_electrical_power_lighting_0035_heavy_duty_fuse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyMildSteelLanternForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0036HeavyDutyLantern() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0036_heavy_duty_lantern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0036_heavy_duty_lantern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0036_heavy_duty_lantern]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty mild steel lantern for domestic power service from prepared mild steel stock.",
		Action = "making heavy-duty mild steel lantern for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel lantern for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2631.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2631 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0036_heavy_duty_lantern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel lantern for domestic power service (@industrialised_electrical_power_lighting_0036_heavy_duty_lantern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyGlassLeadForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0037HeavyDutyLead() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0037_heavy_duty_lead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0037_heavy_duty_lead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0037_heavy_duty_lead]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty glass lead for domestic power service from prepared glass stock.",
		Action = "making heavy-duty glass lead for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass lead for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2672.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2672 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0037_heavy_duty_lead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass lead for domestic power service (@industrialised_electrical_power_lighting_0037_heavy_duty_lead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyCeramicJunctionForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0038HeavyDutyJunction() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0038_heavy_duty_junction]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0038_heavy_duty_junction"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0038_heavy_duty_junction]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty ceramic junction for domestic power service from prepared ceramic stock.",
		Action = "making heavy-duty ceramic junction for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic junction for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2713.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2713 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0038_heavy_duty_junction", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic junction for domestic power service (@industrialised_electrical_power_lighting_0038_heavy_duty_junction)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyNaturalRubberMeterForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0039HeavyDutyMeter() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0039_heavy_duty_meter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0039_heavy_duty_meter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0039_heavy_duty_meter]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty natural rubber meter for domestic power service from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber meter for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber meter for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2753.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2753 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0039_heavy_duty_meter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber meter for domestic power service (@industrialised_electrical_power_lighting_0039_heavy_duty_meter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyCopperTransformerForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0040HeavyDutyTransformer() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0040_heavy_duty_transformer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0040_heavy_duty_transformer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0040_heavy_duty_transformer]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty copper transformer for domestic power service from prepared copper stock.",
		Action = "making heavy-duty copper transformer for domestic power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper transformer for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2794.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2794 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0040_heavy_duty_transformer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper transformer for domestic power service (@industrialised_electrical_power_lighting_0040_heavy_duty_transformer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingMildSteelScrapForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0041FoldingScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make folding mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0041_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0041_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0041_folding_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding mild steel scrap for domestic power service from prepared mild steel stock.",
		Action = "making folding mild steel scrap for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding mild steel scrap for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2835.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2835 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0041_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel scrap for domestic power service (@industrialised_electrical_power_lighting_0041_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingGlassLampForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0042FoldingLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make folding glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0042_folding_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0042_folding_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0042_folding_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding glass lamp for domestic power service from prepared glass stock.",
		Action = "making folding glass lamp for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding glass lamp for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2875.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2875 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0042_folding_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass lamp for domestic power service (@industrialised_electrical_power_lighting_0042_folding_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingCeramicSocketForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0043FoldingSocket() => new()
	{
		Metadata = new("Electrical power and lighting/make folding ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0043_folding_socket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0043_folding_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0043_folding_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding ceramic socket for domestic power service from prepared ceramic stock.",
		Action = "making folding ceramic socket for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding ceramic socket for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2916.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2916 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0043_folding_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic socket for domestic power service (@industrialised_electrical_power_lighting_0043_folding_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingNaturalRubberSwitchForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0044FoldingSwitch() => new()
	{
		Metadata = new("Electrical power and lighting/make folding natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0044_folding_switch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0044_folding_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0044_folding_switch]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding natural rubber switch for domestic power service from prepared natural rubber stock.",
		Action = "making folding natural rubber switch for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber switch for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2957.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2957 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0044_folding_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber switch for domestic power service (@industrialised_electrical_power_lighting_0044_folding_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingCopperFuseForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0045FoldingFuse() => new()
	{
		Metadata = new("Electrical power and lighting/make folding copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0045_folding_fuse]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0045_folding_fuse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0045_folding_fuse]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding copper fuse for domestic power service from prepared copper stock.",
		Action = "making folding copper fuse for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding copper fuse for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2998.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2998 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0045_folding_fuse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper fuse for domestic power service (@industrialised_electrical_power_lighting_0045_folding_fuse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingMildSteelLanternForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0046FoldingLantern() => new()
	{
		Metadata = new("Electrical power and lighting/make folding mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0046_folding_lantern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0046_folding_lantern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0046_folding_lantern]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding mild steel lantern for domestic power service from prepared mild steel stock.",
		Action = "making folding mild steel lantern for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding mild steel lantern for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3038.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3038 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0046_folding_lantern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel lantern for domestic power service (@industrialised_electrical_power_lighting_0046_folding_lantern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingGlassLeadForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0047FoldingLead() => new()
	{
		Metadata = new("Electrical power and lighting/make folding glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0047_folding_lead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0047_folding_lead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0047_folding_lead]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding glass lead for domestic power service from prepared glass stock.",
		Action = "making folding glass lead for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding glass lead for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3079.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3079 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0047_folding_lead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass lead for domestic power service (@industrialised_electrical_power_lighting_0047_folding_lead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingCeramicJunctionForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0048FoldingJunction() => new()
	{
		Metadata = new("Electrical power and lighting/make folding ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0048_folding_junction]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0048_folding_junction"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0048_folding_junction]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding ceramic junction for domestic power service from prepared ceramic stock.",
		Action = "making folding ceramic junction for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding ceramic junction for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3120.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3120 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0048_folding_junction", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic junction for domestic power service (@industrialised_electrical_power_lighting_0048_folding_junction)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingNaturalRubberMeterForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0049FoldingMeter() => new()
	{
		Metadata = new("Electrical power and lighting/make folding natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0049_folding_meter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0049_folding_meter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0049_folding_meter]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding natural rubber meter for domestic power service from prepared natural rubber stock.",
		Action = "making folding natural rubber meter for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber meter for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3160.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3160 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0049_folding_meter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber meter for domestic power service (@industrialised_electrical_power_lighting_0049_folding_meter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeFoldingCopperTransformerForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0050FoldingTransformer() => new()
	{
		Metadata = new("Electrical power and lighting/make folding copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0050_folding_transformer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0050_folding_transformer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0050_folding_transformer]",
		Category = "Electrical power and lighting",
		Blurb = "Make a folding copper transformer for domestic power service from prepared copper stock.",
		Action = "making folding copper transformer for domestic power service",
		ActiveCraftItemSdesc = "an in-progress folding copper transformer for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3201.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3201 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0050_folding_transformer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper transformer for domestic power service (@industrialised_electrical_power_lighting_0050_folding_transformer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableMildSteelScrapForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0051PortableScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make portable mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0051_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0051_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0051_portable_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable mild steel scrap for domestic power service from prepared mild steel stock.",
		Action = "making portable mild steel scrap for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable mild steel scrap for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3242.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3242 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0051_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel scrap for domestic power service (@industrialised_electrical_power_lighting_0051_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableGlassLampForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0052PortableLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make portable glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0052_portable_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0052_portable_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0052_portable_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable glass lamp for domestic power service from prepared glass stock.",
		Action = "making portable glass lamp for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable glass lamp for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3282.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3282 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0052_portable_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable glass lamp for domestic power service (@industrialised_electrical_power_lighting_0052_portable_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableCeramicSocketForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0053PortableSocket() => new()
	{
		Metadata = new("Electrical power and lighting/make portable ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0053_portable_socket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0053_portable_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0053_portable_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable ceramic socket for domestic power service from prepared ceramic stock.",
		Action = "making portable ceramic socket for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable ceramic socket for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3323.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3323 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0053_portable_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic socket for domestic power service (@industrialised_electrical_power_lighting_0053_portable_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableNaturalRubberSwitchForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0054PortableSwitch() => new()
	{
		Metadata = new("Electrical power and lighting/make portable natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0054_portable_switch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0054_portable_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0054_portable_switch]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable natural rubber switch for domestic power service from prepared natural rubber stock.",
		Action = "making portable natural rubber switch for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable natural rubber switch for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3364.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3364 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0054_portable_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable natural rubber switch for domestic power service (@industrialised_electrical_power_lighting_0054_portable_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableCopperFuseForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0055PortableFuse() => new()
	{
		Metadata = new("Electrical power and lighting/make portable copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0055_portable_fuse]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0055_portable_fuse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0055_portable_fuse]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable copper fuse for domestic power service from prepared copper stock.",
		Action = "making portable copper fuse for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable copper fuse for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3405.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3405 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0055_portable_fuse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper fuse for domestic power service (@industrialised_electrical_power_lighting_0055_portable_fuse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableMildSteelLanternForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0056PortableLantern() => new()
	{
		Metadata = new("Electrical power and lighting/make portable mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0056_portable_lantern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0056_portable_lantern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0056_portable_lantern]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable mild steel lantern for domestic power service from prepared mild steel stock.",
		Action = "making portable mild steel lantern for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable mild steel lantern for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3445.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3445 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0056_portable_lantern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel lantern for domestic power service (@industrialised_electrical_power_lighting_0056_portable_lantern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableGlassLeadForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0057PortableLead() => new()
	{
		Metadata = new("Electrical power and lighting/make portable glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0057_portable_lead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0057_portable_lead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0057_portable_lead]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable glass lead for domestic power service from prepared glass stock.",
		Action = "making portable glass lead for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable glass lead for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3486.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3486 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0057_portable_lead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable glass lead for domestic power service (@industrialised_electrical_power_lighting_0057_portable_lead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableCeramicJunctionForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0058PortableJunction() => new()
	{
		Metadata = new("Electrical power and lighting/make portable ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0058_portable_junction]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0058_portable_junction"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0058_portable_junction]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable ceramic junction for domestic power service from prepared ceramic stock.",
		Action = "making portable ceramic junction for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable ceramic junction for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3527.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3527 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0058_portable_junction", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic junction for domestic power service (@industrialised_electrical_power_lighting_0058_portable_junction)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableNaturalRubberMeterForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0059PortableMeter() => new()
	{
		Metadata = new("Electrical power and lighting/make portable natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0059_portable_meter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0059_portable_meter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0059_portable_meter]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable natural rubber meter for domestic power service from prepared natural rubber stock.",
		Action = "making portable natural rubber meter for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable natural rubber meter for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3567.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3567 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0059_portable_meter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable natural rubber meter for domestic power service (@industrialised_electrical_power_lighting_0059_portable_meter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePortableCopperTransformerForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0060PortableTransformer() => new()
	{
		Metadata = new("Electrical power and lighting/make portable copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0060_portable_transformer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0060_portable_transformer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0060_portable_transformer]",
		Category = "Electrical power and lighting",
		Blurb = "Make a portable copper transformer for domestic power service from prepared copper stock.",
		Action = "making portable copper transformer for domestic power service",
		ActiveCraftItemSdesc = "an in-progress portable copper transformer for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3608.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3608 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0060_portable_transformer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper transformer for domestic power service (@industrialised_electrical_power_lighting_0060_portable_transformer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopMildSteelScrapForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0061WorkshopScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0061_workshop_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0061_workshop_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0061_workshop_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop mild steel scrap for domestic power service from prepared mild steel stock.",
		Action = "making workshop mild steel scrap for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel scrap for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3649.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3649 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0061_workshop_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel scrap for domestic power service (@industrialised_electrical_power_lighting_0061_workshop_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopGlassLampForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0062WorkshopLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0062_workshop_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0062_workshop_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0062_workshop_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop glass lamp for domestic power service from prepared glass stock.",
		Action = "making workshop glass lamp for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop glass lamp for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3689.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3689 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0062_workshop_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop glass lamp for domestic power service (@industrialised_electrical_power_lighting_0062_workshop_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopCeramicSocketForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0063WorkshopSocket() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0063_workshop_socket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0063_workshop_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0063_workshop_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop ceramic socket for domestic power service from prepared ceramic stock.",
		Action = "making workshop ceramic socket for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic socket for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3730.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3730 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0063_workshop_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic socket for domestic power service (@industrialised_electrical_power_lighting_0063_workshop_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopNaturalRubberSwitchForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0064WorkshopSwitch() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0064_workshop_switch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0064_workshop_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0064_workshop_switch]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop natural rubber switch for domestic power service from prepared natural rubber stock.",
		Action = "making workshop natural rubber switch for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop natural rubber switch for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3771.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3771 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0064_workshop_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop natural rubber switch for domestic power service (@industrialised_electrical_power_lighting_0064_workshop_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopCopperFuseForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0065WorkshopFuse() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0065_workshop_fuse]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0065_workshop_fuse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0065_workshop_fuse]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop copper fuse for domestic power service from prepared copper stock.",
		Action = "making workshop copper fuse for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop copper fuse for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3812.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3812 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0065_workshop_fuse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop copper fuse for domestic power service (@industrialised_electrical_power_lighting_0065_workshop_fuse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopMildSteelLanternForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0066WorkshopLantern() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0066_workshop_lantern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0066_workshop_lantern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0066_workshop_lantern]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop mild steel lantern for domestic power service from prepared mild steel stock.",
		Action = "making workshop mild steel lantern for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel lantern for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3852.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3852 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0066_workshop_lantern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel lantern for domestic power service (@industrialised_electrical_power_lighting_0066_workshop_lantern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopGlassLeadForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0067WorkshopLead() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0067_workshop_lead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0067_workshop_lead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0067_workshop_lead]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop glass lead for domestic power service from prepared glass stock.",
		Action = "making workshop glass lead for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop glass lead for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3893.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3893 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0067_workshop_lead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop glass lead for domestic power service (@industrialised_electrical_power_lighting_0067_workshop_lead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopCeramicJunctionForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0068WorkshopJunction() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0068_workshop_junction]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0068_workshop_junction"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0068_workshop_junction]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop ceramic junction for domestic power service from prepared ceramic stock.",
		Action = "making workshop ceramic junction for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic junction for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3934.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3934 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0068_workshop_junction", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic junction for domestic power service (@industrialised_electrical_power_lighting_0068_workshop_junction)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopNaturalRubberMeterForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0069WorkshopMeter() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0069_workshop_meter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0069_workshop_meter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0069_workshop_meter]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop natural rubber meter for domestic power service from prepared natural rubber stock.",
		Action = "making workshop natural rubber meter for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop natural rubber meter for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3974.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3974 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0069_workshop_meter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop natural rubber meter for domestic power service (@industrialised_electrical_power_lighting_0069_workshop_meter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWorkshopCopperTransformerForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0070WorkshopTransformer() => new()
	{
		Metadata = new("Electrical power and lighting/make workshop copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0070_workshop_transformer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0070_workshop_transformer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0070_workshop_transformer]",
		Category = "Electrical power and lighting",
		Blurb = "Make a workshop copper transformer for domestic power service from prepared copper stock.",
		Action = "making workshop copper transformer for domestic power service",
		ActiveCraftItemSdesc = "an in-progress workshop copper transformer for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4015.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4015 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0070_workshop_transformer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop copper transformer for domestic power service (@industrialised_electrical_power_lighting_0070_workshop_transformer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalMildSteelScrapForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0071InstitutionalScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0071_institutional_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0071_institutional_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0071_institutional_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional mild steel scrap for domestic power service from prepared mild steel stock.",
		Action = "making institutional mild steel scrap for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel scrap for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4056.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4056 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0071_institutional_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel scrap for domestic power service (@industrialised_electrical_power_lighting_0071_institutional_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalGlassLampForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0072InstitutionalLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0072_institutional_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0072_institutional_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0072_institutional_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional glass lamp for domestic power service from prepared glass stock.",
		Action = "making institutional glass lamp for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional glass lamp for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4096.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4096 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0072_institutional_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional glass lamp for domestic power service (@industrialised_electrical_power_lighting_0072_institutional_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalCeramicSocketForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0073InstitutionalSocket() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0073_institutional_socket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0073_institutional_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0073_institutional_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional ceramic socket for domestic power service from prepared ceramic stock.",
		Action = "making institutional ceramic socket for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic socket for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4137.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4137 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0073_institutional_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic socket for domestic power service (@industrialised_electrical_power_lighting_0073_institutional_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalNaturalRubberSwitchForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0074InstitutionalSwitch() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0074_institutional_switch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0074_institutional_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0074_institutional_switch]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional natural rubber switch for domestic power service from prepared natural rubber stock.",
		Action = "making institutional natural rubber switch for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional natural rubber switch for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4178.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4178 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0074_institutional_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional natural rubber switch for domestic power service (@industrialised_electrical_power_lighting_0074_institutional_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalCopperFuseForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0075InstitutionalFuse() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0075_institutional_fuse]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0075_institutional_fuse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0075_institutional_fuse]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional copper fuse for domestic power service from prepared copper stock.",
		Action = "making institutional copper fuse for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional copper fuse for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4218.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4218 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0075_institutional_fuse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper fuse for domestic power service (@industrialised_electrical_power_lighting_0075_institutional_fuse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalMildSteelLanternForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0076InstitutionalLantern() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0076_institutional_lantern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0076_institutional_lantern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0076_institutional_lantern]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional mild steel lantern for domestic power service from prepared mild steel stock.",
		Action = "making institutional mild steel lantern for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel lantern for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4259.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4259 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0076_institutional_lantern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel lantern for domestic power service (@industrialised_electrical_power_lighting_0076_institutional_lantern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalGlassLeadForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0077InstitutionalLead() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0077_institutional_lead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0077_institutional_lead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0077_institutional_lead]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional glass lead for domestic power service from prepared glass stock.",
		Action = "making institutional glass lead for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional glass lead for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4300.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4300 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0077_institutional_lead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional glass lead for domestic power service (@industrialised_electrical_power_lighting_0077_institutional_lead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalCeramicJunctionForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0078InstitutionalJunction() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0078_institutional_junction]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0078_institutional_junction"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0078_institutional_junction]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional ceramic junction for domestic power service from prepared ceramic stock.",
		Action = "making institutional ceramic junction for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic junction for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4341.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4341 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0078_institutional_junction", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic junction for domestic power service (@industrialised_electrical_power_lighting_0078_institutional_junction)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalNaturalRubberMeterForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0079InstitutionalMeter() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0079_institutional_meter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0079_institutional_meter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0079_institutional_meter]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional natural rubber meter for domestic power service from prepared natural rubber stock.",
		Action = "making institutional natural rubber meter for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional natural rubber meter for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4381.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4381 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0079_institutional_meter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional natural rubber meter for domestic power service (@industrialised_electrical_power_lighting_0079_institutional_meter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeInstitutionalCopperTransformerForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0080InstitutionalTransformer() => new()
	{
		Metadata = new("Electrical power and lighting/make institutional copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0080_institutional_transformer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0080_institutional_transformer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0080_institutional_transformer]",
		Category = "Electrical power and lighting",
		Blurb = "Make an institutional copper transformer for domestic power service from prepared copper stock.",
		Action = "making institutional copper transformer for domestic power service",
		ActiveCraftItemSdesc = "an in-progress institutional copper transformer for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4422.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4422 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0080_institutional_transformer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper transformer for domestic power service (@industrialised_electrical_power_lighting_0080_institutional_transformer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWeatherproofMildSteelScrapForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0081WeatherproofScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make weatherproof mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0081_weatherproof_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0081_weatherproof_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0081_weatherproof_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a weatherproof mild steel scrap for domestic power service from prepared mild steel stock.",
		Action = "making weatherproof mild steel scrap for domestic power service",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel scrap for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4463.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4463 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0081_weatherproof_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel scrap for domestic power service (@industrialised_electrical_power_lighting_0081_weatherproof_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWeatherproofGlassLampForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0082WeatherproofLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make weatherproof glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0082_weatherproof_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0082_weatherproof_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0082_weatherproof_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make a weatherproof glass lamp for domestic power service from prepared glass stock.",
		Action = "making weatherproof glass lamp for domestic power service",
		ActiveCraftItemSdesc = "an in-progress weatherproof glass lamp for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4503.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4503 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0082_weatherproof_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof glass lamp for domestic power service (@industrialised_electrical_power_lighting_0082_weatherproof_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeWeatherproofCeramicSocketForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0083WeatherproofSocket() => new()
	{
		Metadata = new("Electrical power and lighting/make weatherproof ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0083_weatherproof_socket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0083_weatherproof_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0083_weatherproof_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make a weatherproof ceramic socket for domestic power service from prepared ceramic stock.",
		Action = "making weatherproof ceramic socket for domestic power service",
		ActiveCraftItemSdesc = "an in-progress weatherproof ceramic socket for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4544.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4544 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0083_weatherproof_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof ceramic socket for domestic power service (@industrialised_electrical_power_lighting_0083_weatherproof_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyMildSteelScrapForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0001HeavyDutyScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty mild steel scrap for workshop power service [craft_industrial_electrical_power_lighting_0001_heavy_duty_scrap]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0001_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for workshop power service [craft_industrial_electrical_power_lighting_0001_heavy_duty_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty mild steel scrap for workshop power service from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1207.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1207 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0001_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for workshop power service (@industrial_electrical_power_lighting_0001_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeHeavyDutyGlassLampForWorkshopPowerServiceCraftIndustrialElectricalPowerLighting0002HeavyDutyLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make heavy-duty glass lamp for workshop power service [craft_industrial_electrical_power_lighting_0002_heavy_duty_lamp]", "crafts", ["industrial"], ["item:industrial_electrical_power_lighting_0002_heavy_duty_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass lamp for workshop power service [craft_industrial_electrical_power_lighting_0002_heavy_duty_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make a heavy-duty glass lamp for workshop power service from prepared glass stock.",
		Action = "making heavy-duty glass lamp for workshop power service",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass lamp for workshop power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1247.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1247 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_electrical_power_lighting_0002_heavy_duty_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass lamp for workshop power service (@industrial_electrical_power_lighting_0002_heavy_duty_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
