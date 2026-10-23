#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedMildSteelLampForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0006ReinforcedLamp() => new()
	{
		Metadata = new("Transport support and spares/make reinforced mild steel lamp for workshop vehicle servicing [craft_industrial_transport_support_spares_0006_reinforced_lamp]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0006_reinforced_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel lamp for workshop vehicle servicing [craft_industrial_transport_support_spares_0006_reinforced_lamp]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced mild steel lamp for workshop vehicle servicing from prepared mild steel stock.",
		Action = "making reinforced mild steel lamp for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel lamp for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2077.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2077 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0006_reinforced_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel lamp for workshop vehicle servicing (@industrial_transport_support_spares_0006_reinforced_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedSpringSteelPumpForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0007ReinforcedPump() => new()
	{
		Metadata = new("Transport support and spares/make reinforced spring steel pump for workshop vehicle servicing [craft_industrial_transport_support_spares_0007_reinforced_pump]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0007_reinforced_pump"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced spring steel pump for workshop vehicle servicing [craft_industrial_transport_support_spares_0007_reinforced_pump]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced spring steel pump for workshop vehicle servicing from prepared spring steel stock.",
		Action = "making reinforced spring steel pump for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced spring steel pump for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2118.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2118 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0007_reinforced_pump", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced spring steel pump for workshop vehicle servicing (@industrial_transport_support_spares_0007_reinforced_pump)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedNaturalRubberJackForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0008ReinforcedJack() => new()
	{
		Metadata = new("Transport support and spares/make reinforced natural rubber jack for workshop vehicle servicing [craft_industrial_transport_support_spares_0008_reinforced_jack]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0008_reinforced_jack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber jack for workshop vehicle servicing [craft_industrial_transport_support_spares_0008_reinforced_jack]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced natural rubber jack for workshop vehicle servicing from prepared natural rubber stock.",
		Action = "making reinforced natural rubber jack for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber jack for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2158.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2158 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0008_reinforced_jack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber jack for workshop vehicle servicing (@industrial_transport_support_spares_0008_reinforced_jack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedOakChainForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0009ReinforcedChain() => new()
	{
		Metadata = new("Transport support and spares/make reinforced oak chain for workshop vehicle servicing [craft_industrial_transport_support_spares_0009_reinforced_chain]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0009_reinforced_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak chain for workshop vehicle servicing [craft_industrial_transport_support_spares_0009_reinforced_chain]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced oak chain for workshop vehicle servicing from prepared oak stock.",
		Action = "making reinforced oak chain for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak chain for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2199.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2199 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0009_reinforced_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak chain for workshop vehicle servicing (@industrial_transport_support_spares_0009_reinforced_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedMildSteelTyreForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0010ReinforcedTyre() => new()
	{
		Metadata = new("Transport support and spares/make reinforced mild steel tyre for workshop vehicle servicing [craft_industrial_transport_support_spares_0010_reinforced_tyre]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0010_reinforced_tyre"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel tyre for workshop vehicle servicing [craft_industrial_transport_support_spares_0010_reinforced_tyre]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced mild steel tyre for workshop vehicle servicing from prepared mild steel stock.",
		Action = "making reinforced mild steel tyre for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel tyre for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2240.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2240 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0010_reinforced_tyre", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel tyre for workshop vehicle servicing (@industrial_transport_support_spares_0010_reinforced_tyre)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactSpringSteelScrapForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0011CompactScrap() => new()
	{
		Metadata = new("Transport support and spares/make compact spring steel scrap for workshop vehicle servicing [craft_industrial_transport_support_spares_0011_compact_scrap]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0011_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact spring steel scrap for workshop vehicle servicing [craft_industrial_transport_support_spares_0011_compact_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make a compact spring steel scrap for workshop vehicle servicing from prepared spring steel stock.",
		Action = "making compact spring steel scrap for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact spring steel scrap for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2280.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2280 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0011_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact spring steel scrap for workshop vehicle servicing (@industrial_transport_support_spares_0011_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactNaturalRubberWheelForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0012CompactWheel() => new()
	{
		Metadata = new("Transport support and spares/make compact natural rubber wheel for workshop vehicle servicing [craft_industrial_transport_support_spares_0012_compact_wheel]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0012_compact_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber wheel for workshop vehicle servicing [craft_industrial_transport_support_spares_0012_compact_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make a compact natural rubber wheel for workshop vehicle servicing from prepared natural rubber stock.",
		Action = "making compact natural rubber wheel for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber wheel for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2321.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2321 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0012_compact_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber wheel for workshop vehicle servicing (@industrial_transport_support_spares_0012_compact_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactOakAxleForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0013CompactAxle() => new()
	{
		Metadata = new("Transport support and spares/make compact oak axle for workshop vehicle servicing [craft_industrial_transport_support_spares_0013_compact_axle]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0013_compact_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak axle for workshop vehicle servicing [craft_industrial_transport_support_spares_0013_compact_axle]",
		Category = "Transport support and spares",
		Blurb = "Make a compact oak axle for workshop vehicle servicing from prepared oak stock.",
		Action = "making compact oak axle for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact oak axle for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2362.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2362 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0013_compact_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak axle for workshop vehicle servicing (@industrial_transport_support_spares_0013_compact_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactMildSteelBearingForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0014CompactBearing() => new()
	{
		Metadata = new("Transport support and spares/make compact mild steel bearing for workshop vehicle servicing [craft_industrial_transport_support_spares_0014_compact_bearing]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0014_compact_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel bearing for workshop vehicle servicing [craft_industrial_transport_support_spares_0014_compact_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make a compact mild steel bearing for workshop vehicle servicing from prepared mild steel stock.",
		Action = "making compact mild steel bearing for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact mild steel bearing for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2402.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2402 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0014_compact_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel bearing for workshop vehicle servicing (@industrial_transport_support_spares_0014_compact_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactSpringSteelSpringForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0015CompactSpring() => new()
	{
		Metadata = new("Transport support and spares/make compact spring steel spring for workshop vehicle servicing [craft_industrial_transport_support_spares_0015_compact_spring]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0015_compact_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact spring steel spring for workshop vehicle servicing [craft_industrial_transport_support_spares_0015_compact_spring]",
		Category = "Transport support and spares",
		Blurb = "Make a compact spring steel spring for workshop vehicle servicing from prepared spring steel stock.",
		Action = "making compact spring steel spring for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact spring steel spring for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2443.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2443 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0015_compact_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact spring steel spring for workshop vehicle servicing (@industrial_transport_support_spares_0015_compact_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePlainCottonScrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0001PlainScrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make plain cotton scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0001_plain_scrap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a plain cotton scrap for domestic husbandry from prepared cotton stock.",
		Action = "making plain cotton scrap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress plain cotton scrap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1984.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1984 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton scrap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePlainLeatherHoeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0002PlainHoe() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make plain leather hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0002_plain_hoe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0002_plain_hoe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0002_plain_hoe]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a plain leather hoe for domestic husbandry from prepared leather stock.",
		Action = "making plain leather hoe for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress plain leather hoe for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2025.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2025 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0002_plain_hoe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather hoe for domestic husbandry (@industrialised_agriculture_forestry_fishing_0002_plain_hoe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePlainMildSteelRakeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0003PlainRake() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make plain mild steel rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0003_plain_rake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0003_plain_rake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0003_plain_rake]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a plain mild steel rake for domestic husbandry from prepared mild steel stock.",
		Action = "making plain mild steel rake for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress plain mild steel rake for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2066.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2066 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0003_plain_rake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel rake for domestic husbandry (@industrialised_agriculture_forestry_fishing_0003_plain_rake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePlainOakForkForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0004PlainFork() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make plain oak fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0004_plain_fork]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0004_plain_fork"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0004_plain_fork]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a plain oak fork for domestic husbandry from prepared oak stock.",
		Action = "making plain oak fork for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress plain oak fork for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2106.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2106 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0004_plain_fork", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak fork for domestic husbandry (@industrialised_agriculture_forestry_fishing_0004_plain_fork)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePlainCottonSpadeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0005PlainSpade() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make plain cotton spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0005_plain_spade]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0005_plain_spade"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0005_plain_spade]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a plain cotton spade for domestic husbandry from prepared cotton stock.",
		Action = "making plain cotton spade for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress plain cotton spade for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2147.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2147 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0005_plain_spade", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton spade for domestic husbandry (@industrialised_agriculture_forestry_fishing_0005_plain_spade)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeSomePlainLeatherShearsForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0006PlainShears() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make some plain leather shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0006_plain_shears]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0006_plain_shears"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain leather shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0006_plain_shears]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make some plain leather shears for domestic husbandry from prepared leather stock.",
		Action = "making some plain leather shears for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress some plain leather shears for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2188.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2188 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0006_plain_shears", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain leather shears for domestic husbandry (@industrialised_agriculture_forestry_fishing_0006_plain_shears)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePlainMildSteelNetForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0007PlainNet() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make plain mild steel net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0007_plain_net]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0007_plain_net"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0007_plain_net]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a plain mild steel net for domestic husbandry from prepared mild steel stock.",
		Action = "making plain mild steel net for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress plain mild steel net for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2229.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2229 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0007_plain_net", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel net for domestic husbandry (@industrialised_agriculture_forestry_fishing_0007_plain_net)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePlainOakTrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0008PlainTrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make plain oak trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0008_plain_trap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0008_plain_trap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0008_plain_trap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a plain oak trap for domestic husbandry from prepared oak stock.",
		Action = "making plain oak trap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress plain oak trap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2269.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2269 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0008_plain_trap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak trap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0008_plain_trap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePlainCottonBasketForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0009PlainBasket() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make plain cotton basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0009_plain_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0009_plain_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0009_plain_basket]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a plain cotton basket for domestic husbandry from prepared cotton stock.",
		Action = "making plain cotton basket for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress plain cotton basket for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2310.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2310 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0009_plain_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton basket for domestic husbandry (@industrialised_agriculture_forestry_fishing_0009_plain_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePlainLeatherYokeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0010PlainYoke() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make plain leather yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0010_plain_yoke]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0010_plain_yoke"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0010_plain_yoke]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a plain leather yoke for domestic husbandry from prepared leather stock.",
		Action = "making plain leather yoke for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress plain leather yoke for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2351.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2351 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0010_plain_yoke", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather yoke for domestic husbandry (@industrialised_agriculture_forestry_fishing_0010_plain_yoke)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeReinforcedMildSteelScrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0011ReinforcedScrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make reinforced mild steel scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0011_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0011_reinforced_scrap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a reinforced mild steel scrap for domestic husbandry from prepared mild steel stock.",
		Action = "making reinforced mild steel scrap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel scrap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2391.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2391 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel scrap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeReinforcedOakHoeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0012ReinforcedHoe() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make reinforced oak hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0012_reinforced_hoe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0012_reinforced_hoe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0012_reinforced_hoe]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a reinforced oak hoe for domestic husbandry from prepared oak stock.",
		Action = "making reinforced oak hoe for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress reinforced oak hoe for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2432.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2432 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0012_reinforced_hoe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak hoe for domestic husbandry (@industrialised_agriculture_forestry_fishing_0012_reinforced_hoe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeReinforcedCottonRakeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0013ReinforcedRake() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make reinforced cotton rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0013_reinforced_rake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0013_reinforced_rake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0013_reinforced_rake]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a reinforced cotton rake for domestic husbandry from prepared cotton stock.",
		Action = "making reinforced cotton rake for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton rake for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2473.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2473 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0013_reinforced_rake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton rake for domestic husbandry (@industrialised_agriculture_forestry_fishing_0013_reinforced_rake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeReinforcedLeatherForkForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0014ReinforcedFork() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make reinforced leather fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0014_reinforced_fork]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0014_reinforced_fork"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0014_reinforced_fork]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a reinforced leather fork for domestic husbandry from prepared leather stock.",
		Action = "making reinforced leather fork for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress reinforced leather fork for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2514.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2514 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0014_reinforced_fork", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather fork for domestic husbandry (@industrialised_agriculture_forestry_fishing_0014_reinforced_fork)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeReinforcedMildSteelSpadeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0015ReinforcedSpade() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make reinforced mild steel spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0015_reinforced_spade]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0015_reinforced_spade"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0015_reinforced_spade]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a reinforced mild steel spade for domestic husbandry from prepared mild steel stock.",
		Action = "making reinforced mild steel spade for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel spade for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2554.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2554 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0015_reinforced_spade", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel spade for domestic husbandry (@industrialised_agriculture_forestry_fishing_0015_reinforced_spade)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeSomeReinforcedOakShearsForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0016ReinforcedShears() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make some reinforced oak shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0016_reinforced_shears]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0016_reinforced_shears"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced oak shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0016_reinforced_shears]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make some reinforced oak shears for domestic husbandry from prepared oak stock.",
		Action = "making some reinforced oak shears for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress some reinforced oak shears for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2595.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2595 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0016_reinforced_shears", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced oak shears for domestic husbandry (@industrialised_agriculture_forestry_fishing_0016_reinforced_shears)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeReinforcedCottonNetForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0017ReinforcedNet() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make reinforced cotton net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0017_reinforced_net]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0017_reinforced_net"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0017_reinforced_net]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a reinforced cotton net for domestic husbandry from prepared cotton stock.",
		Action = "making reinforced cotton net for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton net for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2636.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2636 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0017_reinforced_net", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton net for domestic husbandry (@industrialised_agriculture_forestry_fishing_0017_reinforced_net)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeReinforcedLeatherTrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0018ReinforcedTrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make reinforced leather trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0018_reinforced_trap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0018_reinforced_trap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0018_reinforced_trap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a reinforced leather trap for domestic husbandry from prepared leather stock.",
		Action = "making reinforced leather trap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress reinforced leather trap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2676.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2676 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0018_reinforced_trap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather trap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0018_reinforced_trap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeReinforcedMildSteelBasketForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0019ReinforcedBasket() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make reinforced mild steel basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0019_reinforced_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0019_reinforced_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0019_reinforced_basket]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a reinforced mild steel basket for domestic husbandry from prepared mild steel stock.",
		Action = "making reinforced mild steel basket for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel basket for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2717.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2717 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0019_reinforced_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel basket for domestic husbandry (@industrialised_agriculture_forestry_fishing_0019_reinforced_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeReinforcedOakYokeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0020ReinforcedYoke() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make reinforced oak yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0020_reinforced_yoke]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0020_reinforced_yoke"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0020_reinforced_yoke]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a reinforced oak yoke for domestic husbandry from prepared oak stock.",
		Action = "making reinforced oak yoke for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress reinforced oak yoke for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2758.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2758 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0020_reinforced_yoke", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak yoke for domestic husbandry (@industrialised_agriculture_forestry_fishing_0020_reinforced_yoke)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeCompactCottonScrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0021CompactScrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make compact cotton scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0021_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0021_compact_scrap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a compact cotton scrap for domestic husbandry from prepared cotton stock.",
		Action = "making compact cotton scrap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress compact cotton scrap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2798.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2798 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton scrap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeCompactLeatherHoeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0022CompactHoe() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make compact leather hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0022_compact_hoe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0022_compact_hoe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0022_compact_hoe]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a compact leather hoe for domestic husbandry from prepared leather stock.",
		Action = "making compact leather hoe for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress compact leather hoe for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2839.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2839 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0022_compact_hoe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather hoe for domestic husbandry (@industrialised_agriculture_forestry_fishing_0022_compact_hoe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeCompactMildSteelRakeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0023CompactRake() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make compact mild steel rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0023_compact_rake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0023_compact_rake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0023_compact_rake]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a compact mild steel rake for domestic husbandry from prepared mild steel stock.",
		Action = "making compact mild steel rake for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress compact mild steel rake for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2880.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2880 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0023_compact_rake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel rake for domestic husbandry (@industrialised_agriculture_forestry_fishing_0023_compact_rake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeCompactOakForkForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0024CompactFork() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make compact oak fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0024_compact_fork]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0024_compact_fork"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0024_compact_fork]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a compact oak fork for domestic husbandry from prepared oak stock.",
		Action = "making compact oak fork for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress compact oak fork for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2921.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2921 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0024_compact_fork", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak fork for domestic husbandry (@industrialised_agriculture_forestry_fishing_0024_compact_fork)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeCompactCottonSpadeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0025CompactSpade() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make compact cotton spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0025_compact_spade]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0025_compact_spade"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0025_compact_spade]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a compact cotton spade for domestic husbandry from prepared cotton stock.",
		Action = "making compact cotton spade for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress compact cotton spade for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2961.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2961 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0025_compact_spade", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton spade for domestic husbandry (@industrialised_agriculture_forestry_fishing_0025_compact_spade)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeSomeCompactLeatherShearsForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0026CompactShears() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make some compact leather shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0026_compact_shears]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0026_compact_shears"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact leather shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0026_compact_shears]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make some compact leather shears for domestic husbandry from prepared leather stock.",
		Action = "making some compact leather shears for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress some compact leather shears for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3002.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3002 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0026_compact_shears", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact leather shears for domestic husbandry (@industrialised_agriculture_forestry_fishing_0026_compact_shears)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeCompactMildSteelNetForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0027CompactNet() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make compact mild steel net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0027_compact_net]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0027_compact_net"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0027_compact_net]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a compact mild steel net for domestic husbandry from prepared mild steel stock.",
		Action = "making compact mild steel net for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress compact mild steel net for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3043.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3043 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0027_compact_net", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel net for domestic husbandry (@industrialised_agriculture_forestry_fishing_0027_compact_net)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeCompactOakTrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0028CompactTrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make compact oak trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0028_compact_trap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0028_compact_trap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0028_compact_trap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a compact oak trap for domestic husbandry from prepared oak stock.",
		Action = "making compact oak trap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress compact oak trap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3083.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3083 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0028_compact_trap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak trap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0028_compact_trap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeCompactCottonBasketForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0029CompactBasket() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make compact cotton basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0029_compact_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0029_compact_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0029_compact_basket]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a compact cotton basket for domestic husbandry from prepared cotton stock.",
		Action = "making compact cotton basket for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress compact cotton basket for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3124.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3124 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0029_compact_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton basket for domestic husbandry (@industrialised_agriculture_forestry_fishing_0029_compact_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeCompactLeatherYokeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0030CompactYoke() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make compact leather yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0030_compact_yoke]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0030_compact_yoke"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0030_compact_yoke]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a compact leather yoke for domestic husbandry from prepared leather stock.",
		Action = "making compact leather yoke for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress compact leather yoke for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3165.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3165 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0030_compact_yoke", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather yoke for domestic husbandry (@industrialised_agriculture_forestry_fishing_0030_compact_yoke)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeHeavyDutyMildSteelScrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0031HeavyDutyScrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make heavy-duty mild steel scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0031_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0031_heavy_duty_scrap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a heavy-duty mild steel scrap for domestic husbandry from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3205.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3205 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeHeavyDutyOakHoeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0032HeavyDutyHoe() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make heavy-duty oak hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0032_heavy_duty_hoe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0032_heavy_duty_hoe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0032_heavy_duty_hoe]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a heavy-duty oak hoe for domestic husbandry from prepared oak stock.",
		Action = "making heavy-duty oak hoe for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak hoe for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3246.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3246 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0032_heavy_duty_hoe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak hoe for domestic husbandry (@industrialised_agriculture_forestry_fishing_0032_heavy_duty_hoe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeHeavyDutyCottonRakeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0033HeavyDutyRake() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make heavy-duty cotton rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0033_heavy_duty_rake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0033_heavy_duty_rake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0033_heavy_duty_rake]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a heavy-duty cotton rake for domestic husbandry from prepared cotton stock.",
		Action = "making heavy-duty cotton rake for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton rake for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3287.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3287 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0033_heavy_duty_rake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton rake for domestic husbandry (@industrialised_agriculture_forestry_fishing_0033_heavy_duty_rake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeHeavyDutyLeatherForkForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0034HeavyDutyFork() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make heavy-duty leather fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0034_heavy_duty_fork]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0034_heavy_duty_fork"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0034_heavy_duty_fork]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a heavy-duty leather fork for domestic husbandry from prepared leather stock.",
		Action = "making heavy-duty leather fork for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather fork for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3328.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3328 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0034_heavy_duty_fork", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather fork for domestic husbandry (@industrialised_agriculture_forestry_fishing_0034_heavy_duty_fork)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeHeavyDutyMildSteelSpadeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0035HeavyDutySpade() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make heavy-duty mild steel spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0035_heavy_duty_spade]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0035_heavy_duty_spade"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0035_heavy_duty_spade]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a heavy-duty mild steel spade for domestic husbandry from prepared mild steel stock.",
		Action = "making heavy-duty mild steel spade for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel spade for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3368.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3368 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0035_heavy_duty_spade", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel spade for domestic husbandry (@industrialised_agriculture_forestry_fishing_0035_heavy_duty_spade)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeSomeHeavyDutyOakShearsForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0036HeavyDutyShears() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make some heavy-duty oak shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0036_heavy_duty_shears]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0036_heavy_duty_shears"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty oak shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0036_heavy_duty_shears]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make some heavy-duty oak shears for domestic husbandry from prepared oak stock.",
		Action = "making some heavy-duty oak shears for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty oak shears for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3409.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3409 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0036_heavy_duty_shears", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty oak shears for domestic husbandry (@industrialised_agriculture_forestry_fishing_0036_heavy_duty_shears)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeHeavyDutyCottonNetForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0037HeavyDutyNet() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make heavy-duty cotton net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0037_heavy_duty_net]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0037_heavy_duty_net"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0037_heavy_duty_net]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a heavy-duty cotton net for domestic husbandry from prepared cotton stock.",
		Action = "making heavy-duty cotton net for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton net for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3450.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3450 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0037_heavy_duty_net", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton net for domestic husbandry (@industrialised_agriculture_forestry_fishing_0037_heavy_duty_net)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeHeavyDutyLeatherTrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0038HeavyDutyTrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make heavy-duty leather trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0038_heavy_duty_trap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0038_heavy_duty_trap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0038_heavy_duty_trap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a heavy-duty leather trap for domestic husbandry from prepared leather stock.",
		Action = "making heavy-duty leather trap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather trap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3490.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3490 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0038_heavy_duty_trap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather trap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0038_heavy_duty_trap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeHeavyDutyMildSteelBasketForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0039HeavyDutyBasket() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make heavy-duty mild steel basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0039_heavy_duty_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0039_heavy_duty_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0039_heavy_duty_basket]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a heavy-duty mild steel basket for domestic husbandry from prepared mild steel stock.",
		Action = "making heavy-duty mild steel basket for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel basket for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3531.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3531 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0039_heavy_duty_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel basket for domestic husbandry (@industrialised_agriculture_forestry_fishing_0039_heavy_duty_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeHeavyDutyOakYokeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0040HeavyDutyYoke() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make heavy-duty oak yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0040_heavy_duty_yoke]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0040_heavy_duty_yoke"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0040_heavy_duty_yoke]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a heavy-duty oak yoke for domestic husbandry from prepared oak stock.",
		Action = "making heavy-duty oak yoke for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak yoke for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3572.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3572 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0040_heavy_duty_yoke", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak yoke for domestic husbandry (@industrialised_agriculture_forestry_fishing_0040_heavy_duty_yoke)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFoldingCottonScrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0041FoldingScrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make folding cotton scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0041_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0041_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0041_folding_scrap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a folding cotton scrap for domestic husbandry from prepared cotton stock.",
		Action = "making folding cotton scrap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress folding cotton scrap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3612.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3612 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0041_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton scrap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0041_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFoldingLeatherHoeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0042FoldingHoe() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make folding leather hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0042_folding_hoe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0042_folding_hoe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0042_folding_hoe]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a folding leather hoe for domestic husbandry from prepared leather stock.",
		Action = "making folding leather hoe for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress folding leather hoe for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3653.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3653 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0042_folding_hoe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather hoe for domestic husbandry (@industrialised_agriculture_forestry_fishing_0042_folding_hoe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFoldingMildSteelRakeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0043FoldingRake() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make folding mild steel rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0043_folding_rake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0043_folding_rake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0043_folding_rake]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a folding mild steel rake for domestic husbandry from prepared mild steel stock.",
		Action = "making folding mild steel rake for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress folding mild steel rake for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3694.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3694 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0043_folding_rake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel rake for domestic husbandry (@industrialised_agriculture_forestry_fishing_0043_folding_rake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFoldingOakForkForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0044FoldingFork() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make folding oak fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0044_folding_fork]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0044_folding_fork"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0044_folding_fork]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a folding oak fork for domestic husbandry from prepared oak stock.",
		Action = "making folding oak fork for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress folding oak fork for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3735.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3735 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0044_folding_fork", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak fork for domestic husbandry (@industrialised_agriculture_forestry_fishing_0044_folding_fork)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFoldingCottonSpadeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0045FoldingSpade() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make folding cotton spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0045_folding_spade]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0045_folding_spade"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton spade for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0045_folding_spade]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a folding cotton spade for domestic husbandry from prepared cotton stock.",
		Action = "making folding cotton spade for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress folding cotton spade for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3775.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3775 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0045_folding_spade", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton spade for domestic husbandry (@industrialised_agriculture_forestry_fishing_0045_folding_spade)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeSomeFoldingLeatherShearsForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0046FoldingShears() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make some folding leather shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0046_folding_shears]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0046_folding_shears"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding leather shears for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0046_folding_shears]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make some folding leather shears for domestic husbandry from prepared leather stock.",
		Action = "making some folding leather shears for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress some folding leather shears for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3816.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3816 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0046_folding_shears", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding leather shears for domestic husbandry (@industrialised_agriculture_forestry_fishing_0046_folding_shears)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFoldingMildSteelNetForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0047FoldingNet() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make folding mild steel net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0047_folding_net]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0047_folding_net"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel net for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0047_folding_net]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a folding mild steel net for domestic husbandry from prepared mild steel stock.",
		Action = "making folding mild steel net for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress folding mild steel net for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3857.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3857 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0047_folding_net", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel net for domestic husbandry (@industrialised_agriculture_forestry_fishing_0047_folding_net)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFoldingOakTrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0048FoldingTrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make folding oak trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0048_folding_trap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0048_folding_trap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak trap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0048_folding_trap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a folding oak trap for domestic husbandry from prepared oak stock.",
		Action = "making folding oak trap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress folding oak trap for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3897.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3897 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0048_folding_trap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak trap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0048_folding_trap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFoldingCottonBasketForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0049FoldingBasket() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make folding cotton basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0049_folding_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0049_folding_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0049_folding_basket]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a folding cotton basket for domestic husbandry from prepared cotton stock.",
		Action = "making folding cotton basket for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress folding cotton basket for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3938.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3938 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0049_folding_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton basket for domestic husbandry (@industrialised_agriculture_forestry_fishing_0049_folding_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFoldingLeatherYokeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0050FoldingYoke() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make folding leather yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0050_folding_yoke]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0050_folding_yoke"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather yoke for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0050_folding_yoke]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a folding leather yoke for domestic husbandry from prepared leather stock.",
		Action = "making folding leather yoke for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress folding leather yoke for domestic husbandry craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3979.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3979 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0050_folding_yoke", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather yoke for domestic husbandry (@industrialised_agriculture_forestry_fishing_0050_folding_yoke)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
