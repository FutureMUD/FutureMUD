#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeWorkshopGlassTerminalForDomesticDataWorkCraftIndustrialisedComputingNetworking0062WorkshopTerminal() => new()
	{
		Metadata = new("Computing and networking/make workshop glass terminal for domestic data work [craft_industrialised_computing_networking_0062_workshop_terminal]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0062_workshop_terminal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop glass terminal for domestic data work [craft_industrialised_computing_networking_0062_workshop_terminal]",
		Category = "Computing and networking",
		Blurb = "Make a workshop glass terminal for domestic data work from prepared glass stock.",
		Action = "making workshop glass terminal for domestic data work",
		ActiveCraftItemSdesc = "an in-progress workshop glass terminal for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3467.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3467 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0062_workshop_terminal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop glass terminal for domestic data work (@industrialised_computing_networking_0062_workshop_terminal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeWorkshopNaturalRubberAdapterForDomesticDataWorkCraftIndustrialisedComputingNetworking0063WorkshopAdapter() => new()
	{
		Metadata = new("Computing and networking/make workshop natural rubber adapter for domestic data work [craft_industrialised_computing_networking_0063_workshop_adapter]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0063_workshop_adapter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop natural rubber adapter for domestic data work [craft_industrialised_computing_networking_0063_workshop_adapter]",
		Category = "Computing and networking",
		Blurb = "Make a workshop natural rubber adapter for domestic data work from prepared natural rubber stock.",
		Action = "making workshop natural rubber adapter for domestic data work",
		ActiveCraftItemSdesc = "an in-progress workshop natural rubber adapter for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3508.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3508 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0063_workshop_adapter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop natural rubber adapter for domestic data work (@industrialised_computing_networking_0063_workshop_adapter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeWorkshopMildSteelSwitchForDomesticDataWorkCraftIndustrialisedComputingNetworking0064WorkshopSwitch() => new()
	{
		Metadata = new("Computing and networking/make workshop mild steel switch for domestic data work [craft_industrialised_computing_networking_0064_workshop_switch]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0064_workshop_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel switch for domestic data work [craft_industrialised_computing_networking_0064_workshop_switch]",
		Category = "Computing and networking",
		Blurb = "Make a workshop mild steel switch for domestic data work from prepared mild steel stock.",
		Action = "making workshop mild steel switch for domestic data work",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel switch for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3549.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3549 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0064_workshop_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel switch for domestic data work (@industrialised_computing_networking_0064_workshop_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakeWorkshopCopperCabinetForDomesticDataWorkCraftIndustrialisedComputingNetworking0065WorkshopCabinet() => new()
	{
		Metadata = new("Computing and networking/make workshop copper cabinet for domestic data work [craft_industrialised_computing_networking_0065_workshop_cabinet]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0065_workshop_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop copper cabinet for domestic data work [craft_industrialised_computing_networking_0065_workshop_cabinet]",
		Category = "Computing and networking",
		Blurb = "Make a workshop copper cabinet for domestic data work from prepared copper stock.",
		Action = "making workshop copper cabinet for domestic data work",
		ActiveCraftItemSdesc = "an in-progress workshop copper cabinet for domestic data work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3589.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3589 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0065_workshop_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop copper cabinet for domestic data work (@industrialised_computing_networking_0065_workshop_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainGlassScrapForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0001PlainScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make plain glass scrap for domestic communication [craft_industrialised_telecom_media_photography_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass scrap for domestic communication [craft_industrialised_telecom_media_photography_0001_plain_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain glass scrap for domestic communication from prepared glass stock.",
		Action = "making plain glass scrap for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain glass scrap for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1096.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1096 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass scrap for domestic communication (@industrialised_telecom_media_photography_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainNaturalRubberTelephoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0002PlainTelephone() => new()
	{
		Metadata = new("Telecom, media and photography/make plain natural rubber telephone for domestic communication [craft_industrialised_telecom_media_photography_0002_plain_telephone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0002_plain_telephone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber telephone for domestic communication [craft_industrialised_telecom_media_photography_0002_plain_telephone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain natural rubber telephone for domestic communication from prepared natural rubber stock.",
		Action = "making plain natural rubber telephone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber telephone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1136.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1136 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0002_plain_telephone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber telephone for domestic communication (@industrialised_telecom_media_photography_0002_plain_telephone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainMildSteelReceiverForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0003PlainReceiver() => new()
	{
		Metadata = new("Telecom, media and photography/make plain mild steel receiver for domestic communication [craft_industrialised_telecom_media_photography_0003_plain_receiver]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0003_plain_receiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel receiver for domestic communication [craft_industrialised_telecom_media_photography_0003_plain_receiver]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain mild steel receiver for domestic communication from prepared mild steel stock.",
		Action = "making plain mild steel receiver for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain mild steel receiver for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1177.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1177 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0003_plain_receiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel receiver for domestic communication (@industrialised_telecom_media_photography_0003_plain_receiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainCopperCameraForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0004PlainCamera() => new()
	{
		Metadata = new("Telecom, media and photography/make plain copper camera for domestic communication [craft_industrialised_telecom_media_photography_0004_plain_camera]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0004_plain_camera"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper camera for domestic communication [craft_industrialised_telecom_media_photography_0004_plain_camera]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain copper camera for domestic communication from prepared copper stock.",
		Action = "making plain copper camera for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain copper camera for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1218.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1218 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0004_plain_camera", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper camera for domestic communication (@industrialised_telecom_media_photography_0004_plain_camera)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainGlassTripodForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0005PlainTripod() => new()
	{
		Metadata = new("Telecom, media and photography/make plain glass tripod for domestic communication [craft_industrialised_telecom_media_photography_0005_plain_tripod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0005_plain_tripod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass tripod for domestic communication [craft_industrialised_telecom_media_photography_0005_plain_tripod]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain glass tripod for domestic communication from prepared glass stock.",
		Action = "making plain glass tripod for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain glass tripod for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1258.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1258 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0005_plain_tripod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass tripod for domestic communication (@industrialised_telecom_media_photography_0005_plain_tripod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainNaturalRubberSpeakerForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0006PlainSpeaker() => new()
	{
		Metadata = new("Telecom, media and photography/make plain natural rubber speaker for domestic communication [craft_industrialised_telecom_media_photography_0006_plain_speaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0006_plain_speaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber speaker for domestic communication [craft_industrialised_telecom_media_photography_0006_plain_speaker]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain natural rubber speaker for domestic communication from prepared natural rubber stock.",
		Action = "making plain natural rubber speaker for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber speaker for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1299.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1299 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0006_plain_speaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber speaker for domestic communication (@industrialised_telecom_media_photography_0006_plain_speaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainMildSteelRecorderForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0007PlainRecorder() => new()
	{
		Metadata = new("Telecom, media and photography/make plain mild steel recorder for domestic communication [craft_industrialised_telecom_media_photography_0007_plain_recorder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0007_plain_recorder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel recorder for domestic communication [craft_industrialised_telecom_media_photography_0007_plain_recorder]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain mild steel recorder for domestic communication from prepared mild steel stock.",
		Action = "making plain mild steel recorder for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain mild steel recorder for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1340.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1340 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0007_plain_recorder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel recorder for domestic communication (@industrialised_telecom_media_photography_0007_plain_recorder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainCopperProjectorForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0008PlainProjector() => new()
	{
		Metadata = new("Telecom, media and photography/make plain copper projector for domestic communication [craft_industrialised_telecom_media_photography_0008_plain_projector]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0008_plain_projector"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper projector for domestic communication [craft_industrialised_telecom_media_photography_0008_plain_projector]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain copper projector for domestic communication from prepared copper stock.",
		Action = "making plain copper projector for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain copper projector for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1380.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1380 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0008_plain_projector", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper projector for domestic communication (@industrialised_telecom_media_photography_0008_plain_projector)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainGlassMicrophoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0009PlainMicrophone() => new()
	{
		Metadata = new("Telecom, media and photography/make plain glass microphone for domestic communication [craft_industrialised_telecom_media_photography_0009_plain_microphone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0009_plain_microphone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass microphone for domestic communication [craft_industrialised_telecom_media_photography_0009_plain_microphone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain glass microphone for domestic communication from prepared glass stock.",
		Action = "making plain glass microphone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain glass microphone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1421.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1421 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0009_plain_microphone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass microphone for domestic communication (@industrialised_telecom_media_photography_0009_plain_microphone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePlainNaturalRubberCableForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0010PlainCable() => new()
	{
		Metadata = new("Telecom, media and photography/make plain natural rubber cable for domestic communication [craft_industrialised_telecom_media_photography_0010_plain_cable]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0010_plain_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber cable for domestic communication [craft_industrialised_telecom_media_photography_0010_plain_cable]",
		Category = "Telecom, media and photography",
		Blurb = "Make a plain natural rubber cable for domestic communication from prepared natural rubber stock.",
		Action = "making plain natural rubber cable for domestic communication",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber cable for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1462.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1462 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0010_plain_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber cable for domestic communication (@industrialised_telecom_media_photography_0010_plain_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedMildSteelScrapForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0011ReinforcedScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced mild steel scrap for domestic communication [craft_industrialised_telecom_media_photography_0011_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel scrap for domestic communication [craft_industrialised_telecom_media_photography_0011_reinforced_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced mild steel scrap for domestic communication from prepared mild steel stock.",
		Action = "making reinforced mild steel scrap for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel scrap for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1503.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1503 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel scrap for domestic communication (@industrialised_telecom_media_photography_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedCopperTelephoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0012ReinforcedTelephone() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced copper telephone for domestic communication [craft_industrialised_telecom_media_photography_0012_reinforced_telephone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0012_reinforced_telephone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper telephone for domestic communication [craft_industrialised_telecom_media_photography_0012_reinforced_telephone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced copper telephone for domestic communication from prepared copper stock.",
		Action = "making reinforced copper telephone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced copper telephone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1543.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1543 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0012_reinforced_telephone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper telephone for domestic communication (@industrialised_telecom_media_photography_0012_reinforced_telephone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedGlassReceiverForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0013ReinforcedReceiver() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced glass receiver for domestic communication [craft_industrialised_telecom_media_photography_0013_reinforced_receiver]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0013_reinforced_receiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass receiver for domestic communication [craft_industrialised_telecom_media_photography_0013_reinforced_receiver]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced glass receiver for domestic communication from prepared glass stock.",
		Action = "making reinforced glass receiver for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced glass receiver for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1584.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1584 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0013_reinforced_receiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass receiver for domestic communication (@industrialised_telecom_media_photography_0013_reinforced_receiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedNaturalRubberCameraForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0014ReinforcedCamera() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced natural rubber camera for domestic communication [craft_industrialised_telecom_media_photography_0014_reinforced_camera]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0014_reinforced_camera"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber camera for domestic communication [craft_industrialised_telecom_media_photography_0014_reinforced_camera]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced natural rubber camera for domestic communication from prepared natural rubber stock.",
		Action = "making reinforced natural rubber camera for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber camera for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1625.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1625 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0014_reinforced_camera", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber camera for domestic communication (@industrialised_telecom_media_photography_0014_reinforced_camera)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedMildSteelTripodForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0015ReinforcedTripod() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced mild steel tripod for domestic communication [craft_industrialised_telecom_media_photography_0015_reinforced_tripod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0015_reinforced_tripod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel tripod for domestic communication [craft_industrialised_telecom_media_photography_0015_reinforced_tripod]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced mild steel tripod for domestic communication from prepared mild steel stock.",
		Action = "making reinforced mild steel tripod for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel tripod for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1665.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1665 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0015_reinforced_tripod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel tripod for domestic communication (@industrialised_telecom_media_photography_0015_reinforced_tripod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedCopperSpeakerForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0016ReinforcedSpeaker() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced copper speaker for domestic communication [craft_industrialised_telecom_media_photography_0016_reinforced_speaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0016_reinforced_speaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper speaker for domestic communication [craft_industrialised_telecom_media_photography_0016_reinforced_speaker]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced copper speaker for domestic communication from prepared copper stock.",
		Action = "making reinforced copper speaker for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced copper speaker for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1706.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1706 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0016_reinforced_speaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper speaker for domestic communication (@industrialised_telecom_media_photography_0016_reinforced_speaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedGlassRecorderForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0017ReinforcedRecorder() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced glass recorder for domestic communication [craft_industrialised_telecom_media_photography_0017_reinforced_recorder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0017_reinforced_recorder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass recorder for domestic communication [craft_industrialised_telecom_media_photography_0017_reinforced_recorder]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced glass recorder for domestic communication from prepared glass stock.",
		Action = "making reinforced glass recorder for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced glass recorder for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1747.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1747 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0017_reinforced_recorder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass recorder for domestic communication (@industrialised_telecom_media_photography_0017_reinforced_recorder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedNaturalRubberProjectorForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0018ReinforcedProjector() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced natural rubber projector for domestic communication [craft_industrialised_telecom_media_photography_0018_reinforced_projector]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0018_reinforced_projector"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber projector for domestic communication [craft_industrialised_telecom_media_photography_0018_reinforced_projector]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced natural rubber projector for domestic communication from prepared natural rubber stock.",
		Action = "making reinforced natural rubber projector for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber projector for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1788.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1788 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0018_reinforced_projector", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber projector for domestic communication (@industrialised_telecom_media_photography_0018_reinforced_projector)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedMildSteelMicrophoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0019ReinforcedMicrophone() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced mild steel microphone for domestic communication [craft_industrialised_telecom_media_photography_0019_reinforced_microphone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0019_reinforced_microphone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel microphone for domestic communication [craft_industrialised_telecom_media_photography_0019_reinforced_microphone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced mild steel microphone for domestic communication from prepared mild steel stock.",
		Action = "making reinforced mild steel microphone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel microphone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1828.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1828 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0019_reinforced_microphone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel microphone for domestic communication (@industrialised_telecom_media_photography_0019_reinforced_microphone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeReinforcedCopperCableForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0020ReinforcedCable() => new()
	{
		Metadata = new("Telecom, media and photography/make reinforced copper cable for domestic communication [craft_industrialised_telecom_media_photography_0020_reinforced_cable]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0020_reinforced_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper cable for domestic communication [craft_industrialised_telecom_media_photography_0020_reinforced_cable]",
		Category = "Telecom, media and photography",
		Blurb = "Make a reinforced copper cable for domestic communication from prepared copper stock.",
		Action = "making reinforced copper cable for domestic communication",
		ActiveCraftItemSdesc = "an in-progress reinforced copper cable for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1869.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1869 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0020_reinforced_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper cable for domestic communication (@industrialised_telecom_media_photography_0020_reinforced_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactGlassScrapForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0021CompactScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make compact glass scrap for domestic communication [craft_industrialised_telecom_media_photography_0021_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass scrap for domestic communication [craft_industrialised_telecom_media_photography_0021_compact_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact glass scrap for domestic communication from prepared glass stock.",
		Action = "making compact glass scrap for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact glass scrap for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1910.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1910 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass scrap for domestic communication (@industrialised_telecom_media_photography_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactNaturalRubberTelephoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0022CompactTelephone() => new()
	{
		Metadata = new("Telecom, media and photography/make compact natural rubber telephone for domestic communication [craft_industrialised_telecom_media_photography_0022_compact_telephone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0022_compact_telephone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber telephone for domestic communication [craft_industrialised_telecom_media_photography_0022_compact_telephone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact natural rubber telephone for domestic communication from prepared natural rubber stock.",
		Action = "making compact natural rubber telephone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber telephone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1950.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1950 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0022_compact_telephone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber telephone for domestic communication (@industrialised_telecom_media_photography_0022_compact_telephone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactMildSteelReceiverForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0023CompactReceiver() => new()
	{
		Metadata = new("Telecom, media and photography/make compact mild steel receiver for domestic communication [craft_industrialised_telecom_media_photography_0023_compact_receiver]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0023_compact_receiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel receiver for domestic communication [craft_industrialised_telecom_media_photography_0023_compact_receiver]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact mild steel receiver for domestic communication from prepared mild steel stock.",
		Action = "making compact mild steel receiver for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact mild steel receiver for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1991.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1991 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0023_compact_receiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel receiver for domestic communication (@industrialised_telecom_media_photography_0023_compact_receiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactCopperCameraForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0024CompactCamera() => new()
	{
		Metadata = new("Telecom, media and photography/make compact copper camera for domestic communication [craft_industrialised_telecom_media_photography_0024_compact_camera]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0024_compact_camera"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper camera for domestic communication [craft_industrialised_telecom_media_photography_0024_compact_camera]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact copper camera for domestic communication from prepared copper stock.",
		Action = "making compact copper camera for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact copper camera for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2032.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2032 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0024_compact_camera", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper camera for domestic communication (@industrialised_telecom_media_photography_0024_compact_camera)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactGlassTripodForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0025CompactTripod() => new()
	{
		Metadata = new("Telecom, media and photography/make compact glass tripod for domestic communication [craft_industrialised_telecom_media_photography_0025_compact_tripod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0025_compact_tripod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass tripod for domestic communication [craft_industrialised_telecom_media_photography_0025_compact_tripod]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact glass tripod for domestic communication from prepared glass stock.",
		Action = "making compact glass tripod for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact glass tripod for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2072.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2072 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0025_compact_tripod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass tripod for domestic communication (@industrialised_telecom_media_photography_0025_compact_tripod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactNaturalRubberSpeakerForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0026CompactSpeaker() => new()
	{
		Metadata = new("Telecom, media and photography/make compact natural rubber speaker for domestic communication [craft_industrialised_telecom_media_photography_0026_compact_speaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0026_compact_speaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber speaker for domestic communication [craft_industrialised_telecom_media_photography_0026_compact_speaker]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact natural rubber speaker for domestic communication from prepared natural rubber stock.",
		Action = "making compact natural rubber speaker for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber speaker for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2113.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2113 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0026_compact_speaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber speaker for domestic communication (@industrialised_telecom_media_photography_0026_compact_speaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactMildSteelRecorderForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0027CompactRecorder() => new()
	{
		Metadata = new("Telecom, media and photography/make compact mild steel recorder for domestic communication [craft_industrialised_telecom_media_photography_0027_compact_recorder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0027_compact_recorder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel recorder for domestic communication [craft_industrialised_telecom_media_photography_0027_compact_recorder]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact mild steel recorder for domestic communication from prepared mild steel stock.",
		Action = "making compact mild steel recorder for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact mild steel recorder for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2154.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2154 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0027_compact_recorder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel recorder for domestic communication (@industrialised_telecom_media_photography_0027_compact_recorder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactCopperProjectorForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0028CompactProjector() => new()
	{
		Metadata = new("Telecom, media and photography/make compact copper projector for domestic communication [craft_industrialised_telecom_media_photography_0028_compact_projector]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0028_compact_projector"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper projector for domestic communication [craft_industrialised_telecom_media_photography_0028_compact_projector]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact copper projector for domestic communication from prepared copper stock.",
		Action = "making compact copper projector for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact copper projector for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2194.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2194 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0028_compact_projector", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper projector for domestic communication (@industrialised_telecom_media_photography_0028_compact_projector)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactGlassMicrophoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0029CompactMicrophone() => new()
	{
		Metadata = new("Telecom, media and photography/make compact glass microphone for domestic communication [craft_industrialised_telecom_media_photography_0029_compact_microphone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0029_compact_microphone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass microphone for domestic communication [craft_industrialised_telecom_media_photography_0029_compact_microphone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact glass microphone for domestic communication from prepared glass stock.",
		Action = "making compact glass microphone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact glass microphone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2235.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2235 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0029_compact_microphone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass microphone for domestic communication (@industrialised_telecom_media_photography_0029_compact_microphone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactNaturalRubberCableForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0030CompactCable() => new()
	{
		Metadata = new("Telecom, media and photography/make compact natural rubber cable for domestic communication [craft_industrialised_telecom_media_photography_0030_compact_cable]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0030_compact_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber cable for domestic communication [craft_industrialised_telecom_media_photography_0030_compact_cable]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact natural rubber cable for domestic communication from prepared natural rubber stock.",
		Action = "making compact natural rubber cable for domestic communication",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber cable for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2276.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2276 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0030_compact_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber cable for domestic communication (@industrialised_telecom_media_photography_0030_compact_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyMildSteelScrapForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0031HeavyDutyScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty mild steel scrap for domestic communication [craft_industrialised_telecom_media_photography_0031_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for domestic communication [craft_industrialised_telecom_media_photography_0031_heavy_duty_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty mild steel scrap for domestic communication from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2317.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2317 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for domestic communication (@industrialised_telecom_media_photography_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyCopperTelephoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0032HeavyDutyTelephone() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty copper telephone for domestic communication [craft_industrialised_telecom_media_photography_0032_heavy_duty_telephone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0032_heavy_duty_telephone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper telephone for domestic communication [craft_industrialised_telecom_media_photography_0032_heavy_duty_telephone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty copper telephone for domestic communication from prepared copper stock.",
		Action = "making heavy-duty copper telephone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper telephone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2357.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2357 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0032_heavy_duty_telephone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper telephone for domestic communication (@industrialised_telecom_media_photography_0032_heavy_duty_telephone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyGlassReceiverForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0033HeavyDutyReceiver() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty glass receiver for domestic communication [craft_industrialised_telecom_media_photography_0033_heavy_duty_receiver]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0033_heavy_duty_receiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass receiver for domestic communication [craft_industrialised_telecom_media_photography_0033_heavy_duty_receiver]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty glass receiver for domestic communication from prepared glass stock.",
		Action = "making heavy-duty glass receiver for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass receiver for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2398.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2398 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0033_heavy_duty_receiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass receiver for domestic communication (@industrialised_telecom_media_photography_0033_heavy_duty_receiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyNaturalRubberCameraForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0034HeavyDutyCamera() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty natural rubber camera for domestic communication [craft_industrialised_telecom_media_photography_0034_heavy_duty_camera]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0034_heavy_duty_camera"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber camera for domestic communication [craft_industrialised_telecom_media_photography_0034_heavy_duty_camera]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty natural rubber camera for domestic communication from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber camera for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber camera for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2439.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2439 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0034_heavy_duty_camera", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber camera for domestic communication (@industrialised_telecom_media_photography_0034_heavy_duty_camera)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyMildSteelTripodForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0035HeavyDutyTripod() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty mild steel tripod for domestic communication [craft_industrialised_telecom_media_photography_0035_heavy_duty_tripod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0035_heavy_duty_tripod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel tripod for domestic communication [craft_industrialised_telecom_media_photography_0035_heavy_duty_tripod]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty mild steel tripod for domestic communication from prepared mild steel stock.",
		Action = "making heavy-duty mild steel tripod for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel tripod for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2479.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2479 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0035_heavy_duty_tripod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel tripod for domestic communication (@industrialised_telecom_media_photography_0035_heavy_duty_tripod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyCopperSpeakerForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0036HeavyDutySpeaker() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty copper speaker for domestic communication [craft_industrialised_telecom_media_photography_0036_heavy_duty_speaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0036_heavy_duty_speaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper speaker for domestic communication [craft_industrialised_telecom_media_photography_0036_heavy_duty_speaker]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty copper speaker for domestic communication from prepared copper stock.",
		Action = "making heavy-duty copper speaker for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper speaker for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2520.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2520 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0036_heavy_duty_speaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper speaker for domestic communication (@industrialised_telecom_media_photography_0036_heavy_duty_speaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyGlassRecorderForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0037HeavyDutyRecorder() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty glass recorder for domestic communication [craft_industrialised_telecom_media_photography_0037_heavy_duty_recorder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0037_heavy_duty_recorder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass recorder for domestic communication [craft_industrialised_telecom_media_photography_0037_heavy_duty_recorder]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty glass recorder for domestic communication from prepared glass stock.",
		Action = "making heavy-duty glass recorder for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass recorder for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2561.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2561 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0037_heavy_duty_recorder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass recorder for domestic communication (@industrialised_telecom_media_photography_0037_heavy_duty_recorder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyNaturalRubberProjectorForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0038HeavyDutyProjector() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty natural rubber projector for domestic communication [craft_industrialised_telecom_media_photography_0038_heavy_duty_projector]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0038_heavy_duty_projector"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber projector for domestic communication [craft_industrialised_telecom_media_photography_0038_heavy_duty_projector]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty natural rubber projector for domestic communication from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber projector for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber projector for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2602.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2602 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0038_heavy_duty_projector", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber projector for domestic communication (@industrialised_telecom_media_photography_0038_heavy_duty_projector)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyMildSteelMicrophoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0039HeavyDutyMicrophone() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty mild steel microphone for domestic communication [craft_industrialised_telecom_media_photography_0039_heavy_duty_microphone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0039_heavy_duty_microphone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel microphone for domestic communication [craft_industrialised_telecom_media_photography_0039_heavy_duty_microphone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty mild steel microphone for domestic communication from prepared mild steel stock.",
		Action = "making heavy-duty mild steel microphone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel microphone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2642.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2642 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0039_heavy_duty_microphone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel microphone for domestic communication (@industrialised_telecom_media_photography_0039_heavy_duty_microphone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyCopperCableForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0040HeavyDutyCable() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty copper cable for domestic communication [craft_industrialised_telecom_media_photography_0040_heavy_duty_cable]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0040_heavy_duty_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper cable for domestic communication [craft_industrialised_telecom_media_photography_0040_heavy_duty_cable]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty copper cable for domestic communication from prepared copper stock.",
		Action = "making heavy-duty copper cable for domestic communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper cable for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2683.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2683 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0040_heavy_duty_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper cable for domestic communication (@industrialised_telecom_media_photography_0040_heavy_duty_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingGlassScrapForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0041FoldingScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make folding glass scrap for domestic communication [craft_industrialised_telecom_media_photography_0041_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0041_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass scrap for domestic communication [craft_industrialised_telecom_media_photography_0041_folding_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding glass scrap for domestic communication from prepared glass stock.",
		Action = "making folding glass scrap for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding glass scrap for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2724.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2724 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0041_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass scrap for domestic communication (@industrialised_telecom_media_photography_0041_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingNaturalRubberTelephoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0042FoldingTelephone() => new()
	{
		Metadata = new("Telecom, media and photography/make folding natural rubber telephone for domestic communication [craft_industrialised_telecom_media_photography_0042_folding_telephone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0042_folding_telephone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber telephone for domestic communication [craft_industrialised_telecom_media_photography_0042_folding_telephone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding natural rubber telephone for domestic communication from prepared natural rubber stock.",
		Action = "making folding natural rubber telephone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber telephone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2764.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2764 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0042_folding_telephone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber telephone for domestic communication (@industrialised_telecom_media_photography_0042_folding_telephone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingMildSteelReceiverForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0043FoldingReceiver() => new()
	{
		Metadata = new("Telecom, media and photography/make folding mild steel receiver for domestic communication [craft_industrialised_telecom_media_photography_0043_folding_receiver]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0043_folding_receiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel receiver for domestic communication [craft_industrialised_telecom_media_photography_0043_folding_receiver]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding mild steel receiver for domestic communication from prepared mild steel stock.",
		Action = "making folding mild steel receiver for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding mild steel receiver for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2805.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2805 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0043_folding_receiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel receiver for domestic communication (@industrialised_telecom_media_photography_0043_folding_receiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingCopperCameraForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0044FoldingCamera() => new()
	{
		Metadata = new("Telecom, media and photography/make folding copper camera for domestic communication [craft_industrialised_telecom_media_photography_0044_folding_camera]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0044_folding_camera"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper camera for domestic communication [craft_industrialised_telecom_media_photography_0044_folding_camera]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding copper camera for domestic communication from prepared copper stock.",
		Action = "making folding copper camera for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding copper camera for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2846.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2846 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0044_folding_camera", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper camera for domestic communication (@industrialised_telecom_media_photography_0044_folding_camera)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingGlassTripodForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0045FoldingTripod() => new()
	{
		Metadata = new("Telecom, media and photography/make folding glass tripod for domestic communication [craft_industrialised_telecom_media_photography_0045_folding_tripod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0045_folding_tripod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass tripod for domestic communication [craft_industrialised_telecom_media_photography_0045_folding_tripod]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding glass tripod for domestic communication from prepared glass stock.",
		Action = "making folding glass tripod for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding glass tripod for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2886.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2886 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0045_folding_tripod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass tripod for domestic communication (@industrialised_telecom_media_photography_0045_folding_tripod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingNaturalRubberSpeakerForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0046FoldingSpeaker() => new()
	{
		Metadata = new("Telecom, media and photography/make folding natural rubber speaker for domestic communication [craft_industrialised_telecom_media_photography_0046_folding_speaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0046_folding_speaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber speaker for domestic communication [craft_industrialised_telecom_media_photography_0046_folding_speaker]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding natural rubber speaker for domestic communication from prepared natural rubber stock.",
		Action = "making folding natural rubber speaker for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber speaker for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2927.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2927 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0046_folding_speaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber speaker for domestic communication (@industrialised_telecom_media_photography_0046_folding_speaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingMildSteelRecorderForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0047FoldingRecorder() => new()
	{
		Metadata = new("Telecom, media and photography/make folding mild steel recorder for domestic communication [craft_industrialised_telecom_media_photography_0047_folding_recorder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0047_folding_recorder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel recorder for domestic communication [craft_industrialised_telecom_media_photography_0047_folding_recorder]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding mild steel recorder for domestic communication from prepared mild steel stock.",
		Action = "making folding mild steel recorder for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding mild steel recorder for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2968.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2968 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0047_folding_recorder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel recorder for domestic communication (@industrialised_telecom_media_photography_0047_folding_recorder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingCopperProjectorForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0048FoldingProjector() => new()
	{
		Metadata = new("Telecom, media and photography/make folding copper projector for domestic communication [craft_industrialised_telecom_media_photography_0048_folding_projector]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0048_folding_projector"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper projector for domestic communication [craft_industrialised_telecom_media_photography_0048_folding_projector]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding copper projector for domestic communication from prepared copper stock.",
		Action = "making folding copper projector for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding copper projector for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3009.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3009 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0048_folding_projector", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper projector for domestic communication (@industrialised_telecom_media_photography_0048_folding_projector)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingGlassMicrophoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0049FoldingMicrophone() => new()
	{
		Metadata = new("Telecom, media and photography/make folding glass microphone for domestic communication [craft_industrialised_telecom_media_photography_0049_folding_microphone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0049_folding_microphone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass microphone for domestic communication [craft_industrialised_telecom_media_photography_0049_folding_microphone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding glass microphone for domestic communication from prepared glass stock.",
		Action = "making folding glass microphone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding glass microphone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3049.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3049 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0049_folding_microphone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass microphone for domestic communication (@industrialised_telecom_media_photography_0049_folding_microphone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeFoldingNaturalRubberCableForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0050FoldingCable() => new()
	{
		Metadata = new("Telecom, media and photography/make folding natural rubber cable for domestic communication [craft_industrialised_telecom_media_photography_0050_folding_cable]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0050_folding_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber cable for domestic communication [craft_industrialised_telecom_media_photography_0050_folding_cable]",
		Category = "Telecom, media and photography",
		Blurb = "Make a folding natural rubber cable for domestic communication from prepared natural rubber stock.",
		Action = "making folding natural rubber cable for domestic communication",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber cable for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3090.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3090 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0050_folding_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber cable for domestic communication (@industrialised_telecom_media_photography_0050_folding_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableMildSteelScrapForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0051PortableScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make portable mild steel scrap for domestic communication [craft_industrialised_telecom_media_photography_0051_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0051_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel scrap for domestic communication [craft_industrialised_telecom_media_photography_0051_portable_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable mild steel scrap for domestic communication from prepared mild steel stock.",
		Action = "making portable mild steel scrap for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable mild steel scrap for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3131.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3131 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0051_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel scrap for domestic communication (@industrialised_telecom_media_photography_0051_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableCopperTelephoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0052PortableTelephone() => new()
	{
		Metadata = new("Telecom, media and photography/make portable copper telephone for domestic communication [craft_industrialised_telecom_media_photography_0052_portable_telephone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0052_portable_telephone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper telephone for domestic communication [craft_industrialised_telecom_media_photography_0052_portable_telephone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable copper telephone for domestic communication from prepared copper stock.",
		Action = "making portable copper telephone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable copper telephone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3171.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3171 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0052_portable_telephone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper telephone for domestic communication (@industrialised_telecom_media_photography_0052_portable_telephone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableGlassReceiverForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0053PortableReceiver() => new()
	{
		Metadata = new("Telecom, media and photography/make portable glass receiver for domestic communication [craft_industrialised_telecom_media_photography_0053_portable_receiver]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0053_portable_receiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable glass receiver for domestic communication [craft_industrialised_telecom_media_photography_0053_portable_receiver]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable glass receiver for domestic communication from prepared glass stock.",
		Action = "making portable glass receiver for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable glass receiver for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3212.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3212 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0053_portable_receiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable glass receiver for domestic communication (@industrialised_telecom_media_photography_0053_portable_receiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableNaturalRubberCameraForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0054PortableCamera() => new()
	{
		Metadata = new("Telecom, media and photography/make portable natural rubber camera for domestic communication [craft_industrialised_telecom_media_photography_0054_portable_camera]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0054_portable_camera"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable natural rubber camera for domestic communication [craft_industrialised_telecom_media_photography_0054_portable_camera]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable natural rubber camera for domestic communication from prepared natural rubber stock.",
		Action = "making portable natural rubber camera for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable natural rubber camera for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3253.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3253 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0054_portable_camera", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable natural rubber camera for domestic communication (@industrialised_telecom_media_photography_0054_portable_camera)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableMildSteelTripodForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0055PortableTripod() => new()
	{
		Metadata = new("Telecom, media and photography/make portable mild steel tripod for domestic communication [craft_industrialised_telecom_media_photography_0055_portable_tripod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0055_portable_tripod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel tripod for domestic communication [craft_industrialised_telecom_media_photography_0055_portable_tripod]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable mild steel tripod for domestic communication from prepared mild steel stock.",
		Action = "making portable mild steel tripod for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable mild steel tripod for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3293.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3293 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0055_portable_tripod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel tripod for domestic communication (@industrialised_telecom_media_photography_0055_portable_tripod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableCopperSpeakerForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0056PortableSpeaker() => new()
	{
		Metadata = new("Telecom, media and photography/make portable copper speaker for domestic communication [craft_industrialised_telecom_media_photography_0056_portable_speaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0056_portable_speaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper speaker for domestic communication [craft_industrialised_telecom_media_photography_0056_portable_speaker]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable copper speaker for domestic communication from prepared copper stock.",
		Action = "making portable copper speaker for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable copper speaker for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3334.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3334 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0056_portable_speaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper speaker for domestic communication (@industrialised_telecom_media_photography_0056_portable_speaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
