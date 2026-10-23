#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactNaturalRubberWheelForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0022CompactWheel() => new()
	{
		Metadata = new("Transport support and spares/make compact natural rubber wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0022_compact_wheel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0022_compact_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0022_compact_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make a compact natural rubber wheel for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making compact natural rubber wheel for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber wheel for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2728.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2728 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0022_compact_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber wheel for domestic vehicle servicing (@industrialised_transport_support_spares_0022_compact_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactOakAxleForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0023CompactAxle() => new()
	{
		Metadata = new("Transport support and spares/make compact oak axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0023_compact_axle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0023_compact_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0023_compact_axle]",
		Category = "Transport support and spares",
		Blurb = "Make a compact oak axle for domestic vehicle servicing from prepared oak stock.",
		Action = "making compact oak axle for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact oak axle for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2769.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2769 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0023_compact_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak axle for domestic vehicle servicing (@industrialised_transport_support_spares_0023_compact_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactMildSteelBearingForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0024CompactBearing() => new()
	{
		Metadata = new("Transport support and spares/make compact mild steel bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0024_compact_bearing]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0024_compact_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0024_compact_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make a compact mild steel bearing for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making compact mild steel bearing for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact mild steel bearing for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2809.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2809 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0024_compact_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel bearing for domestic vehicle servicing (@industrialised_transport_support_spares_0024_compact_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactSpringSteelSpringForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0025CompactSpring() => new()
	{
		Metadata = new("Transport support and spares/make compact spring steel spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0025_compact_spring]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0025_compact_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact spring steel spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0025_compact_spring]",
		Category = "Transport support and spares",
		Blurb = "Make a compact spring steel spring for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making compact spring steel spring for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact spring steel spring for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2850.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2850 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0025_compact_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact spring steel spring for domestic vehicle servicing (@industrialised_transport_support_spares_0025_compact_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactNaturalRubberLampForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0026CompactLamp() => new()
	{
		Metadata = new("Transport support and spares/make compact natural rubber lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0026_compact_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0026_compact_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0026_compact_lamp]",
		Category = "Transport support and spares",
		Blurb = "Make a compact natural rubber lamp for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making compact natural rubber lamp for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber lamp for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2891.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2891 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0026_compact_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber lamp for domestic vehicle servicing (@industrialised_transport_support_spares_0026_compact_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactOakPumpForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0027CompactPump() => new()
	{
		Metadata = new("Transport support and spares/make compact oak pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0027_compact_pump]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0027_compact_pump"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0027_compact_pump]",
		Category = "Transport support and spares",
		Blurb = "Make a compact oak pump for domestic vehicle servicing from prepared oak stock.",
		Action = "making compact oak pump for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact oak pump for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2932.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2932 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0027_compact_pump", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak pump for domestic vehicle servicing (@industrialised_transport_support_spares_0027_compact_pump)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactMildSteelJackForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0028CompactJack() => new()
	{
		Metadata = new("Transport support and spares/make compact mild steel jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0028_compact_jack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0028_compact_jack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0028_compact_jack]",
		Category = "Transport support and spares",
		Blurb = "Make a compact mild steel jack for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making compact mild steel jack for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact mild steel jack for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2972.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2972 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0028_compact_jack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel jack for domestic vehicle servicing (@industrialised_transport_support_spares_0028_compact_jack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactSpringSteelChainForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0029CompactChain() => new()
	{
		Metadata = new("Transport support and spares/make compact spring steel chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0029_compact_chain]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0029_compact_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact spring steel chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0029_compact_chain]",
		Category = "Transport support and spares",
		Blurb = "Make a compact spring steel chain for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making compact spring steel chain for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact spring steel chain for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3013.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3013 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0029_compact_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact spring steel chain for domestic vehicle servicing (@industrialised_transport_support_spares_0029_compact_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactNaturalRubberTyreForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0030CompactTyre() => new()
	{
		Metadata = new("Transport support and spares/make compact natural rubber tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0030_compact_tyre]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0030_compact_tyre"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0030_compact_tyre]",
		Category = "Transport support and spares",
		Blurb = "Make a compact natural rubber tyre for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making compact natural rubber tyre for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber tyre for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3054.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3054 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0030_compact_tyre", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber tyre for domestic vehicle servicing (@industrialised_transport_support_spares_0030_compact_tyre)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutyOakScrapForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0031HeavyDutyScrap() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty oak scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0031_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0031_heavy_duty_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty oak scrap for domestic vehicle servicing from prepared oak stock.",
		Action = "making heavy-duty oak scrap for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak scrap for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3094.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3094 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak scrap for domestic vehicle servicing (@industrialised_transport_support_spares_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutyMildSteelWheelForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0032HeavyDutyWheel() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty mild steel wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0032_heavy_duty_wheel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0032_heavy_duty_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0032_heavy_duty_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty mild steel wheel for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making heavy-duty mild steel wheel for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel wheel for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3135.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3135 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0032_heavy_duty_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel wheel for domestic vehicle servicing (@industrialised_transport_support_spares_0032_heavy_duty_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutySpringSteelAxleForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0033HeavyDutyAxle() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty spring steel axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0033_heavy_duty_axle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0033_heavy_duty_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty spring steel axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0033_heavy_duty_axle]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty spring steel axle for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making heavy-duty spring steel axle for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty spring steel axle for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3176.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3176 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0033_heavy_duty_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty spring steel axle for domestic vehicle servicing (@industrialised_transport_support_spares_0033_heavy_duty_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutyNaturalRubberBearingForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0034HeavyDutyBearing() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty natural rubber bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0034_heavy_duty_bearing]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0034_heavy_duty_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0034_heavy_duty_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty natural rubber bearing for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber bearing for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber bearing for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3216.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3216 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0034_heavy_duty_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber bearing for domestic vehicle servicing (@industrialised_transport_support_spares_0034_heavy_duty_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutyOakSpringForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0035HeavyDutySpring() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty oak spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0035_heavy_duty_spring]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0035_heavy_duty_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0035_heavy_duty_spring]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty oak spring for domestic vehicle servicing from prepared oak stock.",
		Action = "making heavy-duty oak spring for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak spring for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3257.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3257 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0035_heavy_duty_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak spring for domestic vehicle servicing (@industrialised_transport_support_spares_0035_heavy_duty_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutyMildSteelLampForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0036HeavyDutyLamp() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty mild steel lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0036_heavy_duty_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0036_heavy_duty_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0036_heavy_duty_lamp]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty mild steel lamp for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making heavy-duty mild steel lamp for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel lamp for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3298.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3298 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0036_heavy_duty_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel lamp for domestic vehicle servicing (@industrialised_transport_support_spares_0036_heavy_duty_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutySpringSteelPumpForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0037HeavyDutyPump() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty spring steel pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0037_heavy_duty_pump]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0037_heavy_duty_pump"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty spring steel pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0037_heavy_duty_pump]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty spring steel pump for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making heavy-duty spring steel pump for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty spring steel pump for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3339.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3339 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0037_heavy_duty_pump", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty spring steel pump for domestic vehicle servicing (@industrialised_transport_support_spares_0037_heavy_duty_pump)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutyNaturalRubberJackForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0038HeavyDutyJack() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty natural rubber jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0038_heavy_duty_jack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0038_heavy_duty_jack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0038_heavy_duty_jack]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty natural rubber jack for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber jack for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber jack for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3379.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3379 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0038_heavy_duty_jack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber jack for domestic vehicle servicing (@industrialised_transport_support_spares_0038_heavy_duty_jack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutyOakChainForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0039HeavyDutyChain() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty oak chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0039_heavy_duty_chain]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0039_heavy_duty_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0039_heavy_duty_chain]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty oak chain for domestic vehicle servicing from prepared oak stock.",
		Action = "making heavy-duty oak chain for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak chain for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3420.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3420 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0039_heavy_duty_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak chain for domestic vehicle servicing (@industrialised_transport_support_spares_0039_heavy_duty_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeHeavyDutyMildSteelTyreForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0040HeavyDutyTyre() => new()
	{
		Metadata = new("Transport support and spares/make heavy-duty mild steel tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0040_heavy_duty_tyre]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0040_heavy_duty_tyre"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0040_heavy_duty_tyre]",
		Category = "Transport support and spares",
		Blurb = "Make a heavy-duty mild steel tyre for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making heavy-duty mild steel tyre for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel tyre for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3461.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3461 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0040_heavy_duty_tyre", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel tyre for domestic vehicle servicing (@industrialised_transport_support_spares_0040_heavy_duty_tyre)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingSpringSteelScrapForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0041FoldingScrap() => new()
	{
		Metadata = new("Transport support and spares/make folding spring steel scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0041_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0041_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding spring steel scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0041_folding_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make a folding spring steel scrap for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making folding spring steel scrap for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding spring steel scrap for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3501.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3501 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0041_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding spring steel scrap for domestic vehicle servicing (@industrialised_transport_support_spares_0041_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingNaturalRubberWheelForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0042FoldingWheel() => new()
	{
		Metadata = new("Transport support and spares/make folding natural rubber wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0042_folding_wheel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0042_folding_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0042_folding_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make a folding natural rubber wheel for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making folding natural rubber wheel for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber wheel for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3542.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3542 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0042_folding_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber wheel for domestic vehicle servicing (@industrialised_transport_support_spares_0042_folding_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingOakAxleForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0043FoldingAxle() => new()
	{
		Metadata = new("Transport support and spares/make folding oak axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0043_folding_axle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0043_folding_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0043_folding_axle]",
		Category = "Transport support and spares",
		Blurb = "Make a folding oak axle for domestic vehicle servicing from prepared oak stock.",
		Action = "making folding oak axle for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding oak axle for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3583.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3583 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0043_folding_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak axle for domestic vehicle servicing (@industrialised_transport_support_spares_0043_folding_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingMildSteelBearingForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0044FoldingBearing() => new()
	{
		Metadata = new("Transport support and spares/make folding mild steel bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0044_folding_bearing]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0044_folding_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0044_folding_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make a folding mild steel bearing for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making folding mild steel bearing for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding mild steel bearing for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3623.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3623 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0044_folding_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel bearing for domestic vehicle servicing (@industrialised_transport_support_spares_0044_folding_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingSpringSteelSpringForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0045FoldingSpring() => new()
	{
		Metadata = new("Transport support and spares/make folding spring steel spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0045_folding_spring]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0045_folding_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding spring steel spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0045_folding_spring]",
		Category = "Transport support and spares",
		Blurb = "Make a folding spring steel spring for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making folding spring steel spring for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding spring steel spring for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3664.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3664 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0045_folding_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding spring steel spring for domestic vehicle servicing (@industrialised_transport_support_spares_0045_folding_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingNaturalRubberLampForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0046FoldingLamp() => new()
	{
		Metadata = new("Transport support and spares/make folding natural rubber lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0046_folding_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0046_folding_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0046_folding_lamp]",
		Category = "Transport support and spares",
		Blurb = "Make a folding natural rubber lamp for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making folding natural rubber lamp for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber lamp for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3705.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3705 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0046_folding_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber lamp for domestic vehicle servicing (@industrialised_transport_support_spares_0046_folding_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingOakPumpForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0047FoldingPump() => new()
	{
		Metadata = new("Transport support and spares/make folding oak pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0047_folding_pump]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0047_folding_pump"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0047_folding_pump]",
		Category = "Transport support and spares",
		Blurb = "Make a folding oak pump for domestic vehicle servicing from prepared oak stock.",
		Action = "making folding oak pump for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding oak pump for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3746.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3746 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0047_folding_pump", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak pump for domestic vehicle servicing (@industrialised_transport_support_spares_0047_folding_pump)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingMildSteelJackForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0048FoldingJack() => new()
	{
		Metadata = new("Transport support and spares/make folding mild steel jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0048_folding_jack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0048_folding_jack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0048_folding_jack]",
		Category = "Transport support and spares",
		Blurb = "Make a folding mild steel jack for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making folding mild steel jack for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding mild steel jack for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3786.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3786 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0048_folding_jack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel jack for domestic vehicle servicing (@industrialised_transport_support_spares_0048_folding_jack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingSpringSteelChainForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0049FoldingChain() => new()
	{
		Metadata = new("Transport support and spares/make folding spring steel chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0049_folding_chain]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0049_folding_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding spring steel chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0049_folding_chain]",
		Category = "Transport support and spares",
		Blurb = "Make a folding spring steel chain for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making folding spring steel chain for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding spring steel chain for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3827.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3827 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0049_folding_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding spring steel chain for domestic vehicle servicing (@industrialised_transport_support_spares_0049_folding_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeFoldingNaturalRubberTyreForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0050FoldingTyre() => new()
	{
		Metadata = new("Transport support and spares/make folding natural rubber tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0050_folding_tyre]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0050_folding_tyre"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0050_folding_tyre]",
		Category = "Transport support and spares",
		Blurb = "Make a folding natural rubber tyre for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making folding natural rubber tyre for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber tyre for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3868.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3868 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0050_folding_tyre", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber tyre for domestic vehicle servicing (@industrialised_transport_support_spares_0050_folding_tyre)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableOakScrapForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0051PortableScrap() => new()
	{
		Metadata = new("Transport support and spares/make portable oak scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0051_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0051_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0051_portable_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make a portable oak scrap for domestic vehicle servicing from prepared oak stock.",
		Action = "making portable oak scrap for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable oak scrap for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3908.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3908 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0051_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak scrap for domestic vehicle servicing (@industrialised_transport_support_spares_0051_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableMildSteelWheelForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0052PortableWheel() => new()
	{
		Metadata = new("Transport support and spares/make portable mild steel wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0052_portable_wheel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0052_portable_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0052_portable_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make a portable mild steel wheel for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making portable mild steel wheel for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable mild steel wheel for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3949.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3949 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0052_portable_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel wheel for domestic vehicle servicing (@industrialised_transport_support_spares_0052_portable_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableSpringSteelAxleForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0053PortableAxle() => new()
	{
		Metadata = new("Transport support and spares/make portable spring steel axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0053_portable_axle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0053_portable_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable spring steel axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0053_portable_axle]",
		Category = "Transport support and spares",
		Blurb = "Make a portable spring steel axle for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making portable spring steel axle for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable spring steel axle for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3990.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3990 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0053_portable_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable spring steel axle for domestic vehicle servicing (@industrialised_transport_support_spares_0053_portable_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableNaturalRubberBearingForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0054PortableBearing() => new()
	{
		Metadata = new("Transport support and spares/make portable natural rubber bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0054_portable_bearing]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0054_portable_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable natural rubber bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0054_portable_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make a portable natural rubber bearing for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making portable natural rubber bearing for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable natural rubber bearing for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4030.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4030 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0054_portable_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable natural rubber bearing for domestic vehicle servicing (@industrialised_transport_support_spares_0054_portable_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableOakSpringForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0055PortableSpring() => new()
	{
		Metadata = new("Transport support and spares/make portable oak spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0055_portable_spring]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0055_portable_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0055_portable_spring]",
		Category = "Transport support and spares",
		Blurb = "Make a portable oak spring for domestic vehicle servicing from prepared oak stock.",
		Action = "making portable oak spring for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable oak spring for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4071.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4071 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0055_portable_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak spring for domestic vehicle servicing (@industrialised_transport_support_spares_0055_portable_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableMildSteelLampForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0056PortableLamp() => new()
	{
		Metadata = new("Transport support and spares/make portable mild steel lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0056_portable_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0056_portable_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0056_portable_lamp]",
		Category = "Transport support and spares",
		Blurb = "Make a portable mild steel lamp for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making portable mild steel lamp for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable mild steel lamp for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4112.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4112 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0056_portable_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel lamp for domestic vehicle servicing (@industrialised_transport_support_spares_0056_portable_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableSpringSteelPumpForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0057PortablePump() => new()
	{
		Metadata = new("Transport support and spares/make portable spring steel pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0057_portable_pump]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0057_portable_pump"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable spring steel pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0057_portable_pump]",
		Category = "Transport support and spares",
		Blurb = "Make a portable spring steel pump for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making portable spring steel pump for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable spring steel pump for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4152.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4152 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0057_portable_pump", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable spring steel pump for domestic vehicle servicing (@industrialised_transport_support_spares_0057_portable_pump)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableNaturalRubberJackForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0058PortableJack() => new()
	{
		Metadata = new("Transport support and spares/make portable natural rubber jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0058_portable_jack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0058_portable_jack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable natural rubber jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0058_portable_jack]",
		Category = "Transport support and spares",
		Blurb = "Make a portable natural rubber jack for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making portable natural rubber jack for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable natural rubber jack for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4193.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4193 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0058_portable_jack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable natural rubber jack for domestic vehicle servicing (@industrialised_transport_support_spares_0058_portable_jack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableOakChainForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0059PortableChain() => new()
	{
		Metadata = new("Transport support and spares/make portable oak chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0059_portable_chain]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0059_portable_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0059_portable_chain]",
		Category = "Transport support and spares",
		Blurb = "Make a portable oak chain for domestic vehicle servicing from prepared oak stock.",
		Action = "making portable oak chain for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable oak chain for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4234.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4234 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0059_portable_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak chain for domestic vehicle servicing (@industrialised_transport_support_spares_0059_portable_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePortableMildSteelTyreForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0060PortableTyre() => new()
	{
		Metadata = new("Transport support and spares/make portable mild steel tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0060_portable_tyre]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0060_portable_tyre"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0060_portable_tyre]",
		Category = "Transport support and spares",
		Blurb = "Make a portable mild steel tyre for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making portable mild steel tyre for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress portable mild steel tyre for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4275.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4275 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0060_portable_tyre", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel tyre for domestic vehicle servicing (@industrialised_transport_support_spares_0060_portable_tyre)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopSpringSteelScrapForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0061WorkshopScrap() => new()
	{
		Metadata = new("Transport support and spares/make workshop spring steel scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0061_workshop_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0061_workshop_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop spring steel scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0061_workshop_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop spring steel scrap for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making workshop spring steel scrap for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop spring steel scrap for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4315.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4315 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0061_workshop_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop spring steel scrap for domestic vehicle servicing (@industrialised_transport_support_spares_0061_workshop_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopNaturalRubberWheelForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0062WorkshopWheel() => new()
	{
		Metadata = new("Transport support and spares/make workshop natural rubber wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0062_workshop_wheel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0062_workshop_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop natural rubber wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0062_workshop_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop natural rubber wheel for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making workshop natural rubber wheel for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop natural rubber wheel for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4356.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4356 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0062_workshop_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop natural rubber wheel for domestic vehicle servicing (@industrialised_transport_support_spares_0062_workshop_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopOakAxleForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0063WorkshopAxle() => new()
	{
		Metadata = new("Transport support and spares/make workshop oak axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0063_workshop_axle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0063_workshop_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0063_workshop_axle]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop oak axle for domestic vehicle servicing from prepared oak stock.",
		Action = "making workshop oak axle for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop oak axle for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4397.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4397 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0063_workshop_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak axle for domestic vehicle servicing (@industrialised_transport_support_spares_0063_workshop_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopMildSteelBearingForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0064WorkshopBearing() => new()
	{
		Metadata = new("Transport support and spares/make workshop mild steel bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0064_workshop_bearing]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0064_workshop_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0064_workshop_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop mild steel bearing for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making workshop mild steel bearing for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel bearing for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4437.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4437 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0064_workshop_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel bearing for domestic vehicle servicing (@industrialised_transport_support_spares_0064_workshop_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopSpringSteelSpringForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0065WorkshopSpring() => new()
	{
		Metadata = new("Transport support and spares/make workshop spring steel spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0065_workshop_spring]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0065_workshop_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop spring steel spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0065_workshop_spring]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop spring steel spring for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making workshop spring steel spring for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop spring steel spring for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4478.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4478 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0065_workshop_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop spring steel spring for domestic vehicle servicing (@industrialised_transport_support_spares_0065_workshop_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopNaturalRubberLampForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0066WorkshopLamp() => new()
	{
		Metadata = new("Transport support and spares/make workshop natural rubber lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0066_workshop_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0066_workshop_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop natural rubber lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0066_workshop_lamp]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop natural rubber lamp for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making workshop natural rubber lamp for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop natural rubber lamp for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4519.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4519 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0066_workshop_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop natural rubber lamp for domestic vehicle servicing (@industrialised_transport_support_spares_0066_workshop_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopOakPumpForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0067WorkshopPump() => new()
	{
		Metadata = new("Transport support and spares/make workshop oak pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0067_workshop_pump]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0067_workshop_pump"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0067_workshop_pump]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop oak pump for domestic vehicle servicing from prepared oak stock.",
		Action = "making workshop oak pump for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop oak pump for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4560.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4560 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0067_workshop_pump", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak pump for domestic vehicle servicing (@industrialised_transport_support_spares_0067_workshop_pump)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopMildSteelJackForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0068WorkshopJack() => new()
	{
		Metadata = new("Transport support and spares/make workshop mild steel jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0068_workshop_jack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0068_workshop_jack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0068_workshop_jack]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop mild steel jack for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making workshop mild steel jack for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel jack for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4600.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4600 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0068_workshop_jack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel jack for domestic vehicle servicing (@industrialised_transport_support_spares_0068_workshop_jack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopSpringSteelChainForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0069WorkshopChain() => new()
	{
		Metadata = new("Transport support and spares/make workshop spring steel chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0069_workshop_chain]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0069_workshop_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop spring steel chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0069_workshop_chain]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop spring steel chain for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making workshop spring steel chain for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop spring steel chain for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4641.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4641 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0069_workshop_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop spring steel chain for domestic vehicle servicing (@industrialised_transport_support_spares_0069_workshop_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeWorkshopNaturalRubberTyreForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0070WorkshopTyre() => new()
	{
		Metadata = new("Transport support and spares/make workshop natural rubber tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0070_workshop_tyre]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0070_workshop_tyre"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop natural rubber tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0070_workshop_tyre]",
		Category = "Transport support and spares",
		Blurb = "Make a workshop natural rubber tyre for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making workshop natural rubber tyre for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress workshop natural rubber tyre for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4682.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4682 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0070_workshop_tyre", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop natural rubber tyre for domestic vehicle servicing (@industrialised_transport_support_spares_0070_workshop_tyre)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeInstitutionalOakScrapForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0071InstitutionalScrap() => new()
	{
		Metadata = new("Transport support and spares/make institutional oak scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0071_institutional_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0071_institutional_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0071_institutional_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make an institutional oak scrap for domestic vehicle servicing from prepared oak stock.",
		Action = "making institutional oak scrap for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress institutional oak scrap for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4722.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4722 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0071_institutional_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak scrap for domestic vehicle servicing (@industrialised_transport_support_spares_0071_institutional_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeInstitutionalMildSteelWheelForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0072InstitutionalWheel() => new()
	{
		Metadata = new("Transport support and spares/make institutional mild steel wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0072_institutional_wheel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0072_institutional_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0072_institutional_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make an institutional mild steel wheel for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making institutional mild steel wheel for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel wheel for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4763.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4763 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0072_institutional_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel wheel for domestic vehicle servicing (@industrialised_transport_support_spares_0072_institutional_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeInstitutionalSpringSteelAxleForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0073InstitutionalAxle() => new()
	{
		Metadata = new("Transport support and spares/make institutional spring steel axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0073_institutional_axle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0073_institutional_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional spring steel axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0073_institutional_axle]",
		Category = "Transport support and spares",
		Blurb = "Make an institutional spring steel axle for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making institutional spring steel axle for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress institutional spring steel axle for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4804.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4804 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0073_institutional_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional spring steel axle for domestic vehicle servicing (@industrialised_transport_support_spares_0073_institutional_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeInstitutionalNaturalRubberBearingForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0074InstitutionalBearing() => new()
	{
		Metadata = new("Transport support and spares/make institutional natural rubber bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0074_institutional_bearing]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0074_institutional_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional natural rubber bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0074_institutional_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make an institutional natural rubber bearing for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making institutional natural rubber bearing for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress institutional natural rubber bearing for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4844.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4844 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0074_institutional_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional natural rubber bearing for domestic vehicle servicing (@industrialised_transport_support_spares_0074_institutional_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeInstitutionalOakSpringForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0075InstitutionalSpring() => new()
	{
		Metadata = new("Transport support and spares/make institutional oak spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0075_institutional_spring]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0075_institutional_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0075_institutional_spring]",
		Category = "Transport support and spares",
		Blurb = "Make an institutional oak spring for domestic vehicle servicing from prepared oak stock.",
		Action = "making institutional oak spring for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress institutional oak spring for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4885.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4885 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0075_institutional_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak spring for domestic vehicle servicing (@industrialised_transport_support_spares_0075_institutional_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeInstitutionalMildSteelLampForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0076InstitutionalLamp() => new()
	{
		Metadata = new("Transport support and spares/make institutional mild steel lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0076_institutional_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0076_institutional_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0076_institutional_lamp]",
		Category = "Transport support and spares",
		Blurb = "Make an institutional mild steel lamp for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making institutional mild steel lamp for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel lamp for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4926.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4926 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0076_institutional_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel lamp for domestic vehicle servicing (@industrialised_transport_support_spares_0076_institutional_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedOakScrapForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0001ReinforcedScrap() => new()
	{
		Metadata = new("Transport support and spares/make reinforced oak scrap for workshop vehicle servicing [craft_industrial_transport_support_spares_0001_reinforced_scrap]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0001_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak scrap for workshop vehicle servicing [craft_industrial_transport_support_spares_0001_reinforced_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced oak scrap for workshop vehicle servicing from prepared oak stock.",
		Action = "making reinforced oak scrap for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak scrap for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1873.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1873 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0001_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak scrap for workshop vehicle servicing (@industrial_transport_support_spares_0001_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedMildSteelWheelForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0002ReinforcedWheel() => new()
	{
		Metadata = new("Transport support and spares/make reinforced mild steel wheel for workshop vehicle servicing [craft_industrial_transport_support_spares_0002_reinforced_wheel]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0002_reinforced_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel wheel for workshop vehicle servicing [craft_industrial_transport_support_spares_0002_reinforced_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced mild steel wheel for workshop vehicle servicing from prepared mild steel stock.",
		Action = "making reinforced mild steel wheel for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel wheel for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1914.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1914 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0002_reinforced_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel wheel for workshop vehicle servicing (@industrial_transport_support_spares_0002_reinforced_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedSpringSteelAxleForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0003ReinforcedAxle() => new()
	{
		Metadata = new("Transport support and spares/make reinforced spring steel axle for workshop vehicle servicing [craft_industrial_transport_support_spares_0003_reinforced_axle]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0003_reinforced_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced spring steel axle for workshop vehicle servicing [craft_industrial_transport_support_spares_0003_reinforced_axle]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced spring steel axle for workshop vehicle servicing from prepared spring steel stock.",
		Action = "making reinforced spring steel axle for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced spring steel axle for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1955.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1955 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0003_reinforced_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced spring steel axle for workshop vehicle servicing (@industrial_transport_support_spares_0003_reinforced_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedNaturalRubberBearingForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0004ReinforcedBearing() => new()
	{
		Metadata = new("Transport support and spares/make reinforced natural rubber bearing for workshop vehicle servicing [craft_industrial_transport_support_spares_0004_reinforced_bearing]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0004_reinforced_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber bearing for workshop vehicle servicing [craft_industrial_transport_support_spares_0004_reinforced_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced natural rubber bearing for workshop vehicle servicing from prepared natural rubber stock.",
		Action = "making reinforced natural rubber bearing for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber bearing for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1995.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1995 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0004_reinforced_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber bearing for workshop vehicle servicing (@industrial_transport_support_spares_0004_reinforced_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedOakSpringForWorkshopVehicleServicingCraftIndustrialTransportSupportSpares0005ReinforcedSpring() => new()
	{
		Metadata = new("Transport support and spares/make reinforced oak spring for workshop vehicle servicing [craft_industrial_transport_support_spares_0005_reinforced_spring]", "crafts", ["industrial"], ["item:industrial_transport_support_spares_0005_reinforced_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak spring for workshop vehicle servicing [craft_industrial_transport_support_spares_0005_reinforced_spring]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced oak spring for workshop vehicle servicing from prepared oak stock.",
		Action = "making reinforced oak spring for workshop vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak spring for workshop vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2036.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2036 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_transport_support_spares_0005_reinforced_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak spring for workshop vehicle servicing (@industrial_transport_support_spares_0005_reinforced_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
