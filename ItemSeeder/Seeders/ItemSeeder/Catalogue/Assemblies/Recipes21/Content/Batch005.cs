#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableGlassRecorderForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0057PortableRecorder() => new()
	{
		Metadata = new("Telecom, media and photography/make portable glass recorder for domestic communication [craft_industrialised_telecom_media_photography_0057_portable_recorder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0057_portable_recorder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable glass recorder for domestic communication [craft_industrialised_telecom_media_photography_0057_portable_recorder]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable glass recorder for domestic communication from prepared glass stock.",
		Action = "making portable glass recorder for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable glass recorder for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3375.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3375 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0057_portable_recorder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable glass recorder for domestic communication (@industrialised_telecom_media_photography_0057_portable_recorder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableNaturalRubberProjectorForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0058PortableProjector() => new()
	{
		Metadata = new("Telecom, media and photography/make portable natural rubber projector for domestic communication [craft_industrialised_telecom_media_photography_0058_portable_projector]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0058_portable_projector"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable natural rubber projector for domestic communication [craft_industrialised_telecom_media_photography_0058_portable_projector]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable natural rubber projector for domestic communication from prepared natural rubber stock.",
		Action = "making portable natural rubber projector for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable natural rubber projector for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3416.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3416 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0058_portable_projector", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable natural rubber projector for domestic communication (@industrialised_telecom_media_photography_0058_portable_projector)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableMildSteelMicrophoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0059PortableMicrophone() => new()
	{
		Metadata = new("Telecom, media and photography/make portable mild steel microphone for domestic communication [craft_industrialised_telecom_media_photography_0059_portable_microphone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0059_portable_microphone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel microphone for domestic communication [craft_industrialised_telecom_media_photography_0059_portable_microphone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable mild steel microphone for domestic communication from prepared mild steel stock.",
		Action = "making portable mild steel microphone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable mild steel microphone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3456.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3456 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0059_portable_microphone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel microphone for domestic communication (@industrialised_telecom_media_photography_0059_portable_microphone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakePortableCopperCableForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0060PortableCable() => new()
	{
		Metadata = new("Telecom, media and photography/make portable copper cable for domestic communication [craft_industrialised_telecom_media_photography_0060_portable_cable]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0060_portable_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable copper cable for domestic communication [craft_industrialised_telecom_media_photography_0060_portable_cable]",
		Category = "Telecom, media and photography",
		Blurb = "Make a portable copper cable for domestic communication from prepared copper stock.",
		Action = "making portable copper cable for domestic communication",
		ActiveCraftItemSdesc = "an in-progress portable copper cable for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3497.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3497 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0060_portable_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable copper cable for domestic communication (@industrialised_telecom_media_photography_0060_portable_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopGlassScrapForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0061WorkshopScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop glass scrap for domestic communication [craft_industrialised_telecom_media_photography_0061_workshop_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0061_workshop_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop glass scrap for domestic communication [craft_industrialised_telecom_media_photography_0061_workshop_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop glass scrap for domestic communication from prepared glass stock.",
		Action = "making workshop glass scrap for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop glass scrap for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3538.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3538 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0061_workshop_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop glass scrap for domestic communication (@industrialised_telecom_media_photography_0061_workshop_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopNaturalRubberTelephoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0062WorkshopTelephone() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop natural rubber telephone for domestic communication [craft_industrialised_telecom_media_photography_0062_workshop_telephone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0062_workshop_telephone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop natural rubber telephone for domestic communication [craft_industrialised_telecom_media_photography_0062_workshop_telephone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop natural rubber telephone for domestic communication from prepared natural rubber stock.",
		Action = "making workshop natural rubber telephone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop natural rubber telephone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3578.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3578 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0062_workshop_telephone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop natural rubber telephone for domestic communication (@industrialised_telecom_media_photography_0062_workshop_telephone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopMildSteelReceiverForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0063WorkshopReceiver() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop mild steel receiver for domestic communication [craft_industrialised_telecom_media_photography_0063_workshop_receiver]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0063_workshop_receiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel receiver for domestic communication [craft_industrialised_telecom_media_photography_0063_workshop_receiver]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop mild steel receiver for domestic communication from prepared mild steel stock.",
		Action = "making workshop mild steel receiver for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel receiver for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3619.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3619 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0063_workshop_receiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel receiver for domestic communication (@industrialised_telecom_media_photography_0063_workshop_receiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopCopperCameraForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0064WorkshopCamera() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop copper camera for domestic communication [craft_industrialised_telecom_media_photography_0064_workshop_camera]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0064_workshop_camera"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop copper camera for domestic communication [craft_industrialised_telecom_media_photography_0064_workshop_camera]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop copper camera for domestic communication from prepared copper stock.",
		Action = "making workshop copper camera for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop copper camera for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3660.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3660 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0064_workshop_camera", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop copper camera for domestic communication (@industrialised_telecom_media_photography_0064_workshop_camera)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopGlassTripodForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0065WorkshopTripod() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop glass tripod for domestic communication [craft_industrialised_telecom_media_photography_0065_workshop_tripod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0065_workshop_tripod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop glass tripod for domestic communication [craft_industrialised_telecom_media_photography_0065_workshop_tripod]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop glass tripod for domestic communication from prepared glass stock.",
		Action = "making workshop glass tripod for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop glass tripod for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3700.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3700 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0065_workshop_tripod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop glass tripod for domestic communication (@industrialised_telecom_media_photography_0065_workshop_tripod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopNaturalRubberSpeakerForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0066WorkshopSpeaker() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop natural rubber speaker for domestic communication [craft_industrialised_telecom_media_photography_0066_workshop_speaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0066_workshop_speaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop natural rubber speaker for domestic communication [craft_industrialised_telecom_media_photography_0066_workshop_speaker]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop natural rubber speaker for domestic communication from prepared natural rubber stock.",
		Action = "making workshop natural rubber speaker for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop natural rubber speaker for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3741.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3741 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0066_workshop_speaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop natural rubber speaker for domestic communication (@industrialised_telecom_media_photography_0066_workshop_speaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopMildSteelRecorderForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0067WorkshopRecorder() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop mild steel recorder for domestic communication [craft_industrialised_telecom_media_photography_0067_workshop_recorder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0067_workshop_recorder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel recorder for domestic communication [craft_industrialised_telecom_media_photography_0067_workshop_recorder]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop mild steel recorder for domestic communication from prepared mild steel stock.",
		Action = "making workshop mild steel recorder for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel recorder for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3782.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3782 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0067_workshop_recorder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel recorder for domestic communication (@industrialised_telecom_media_photography_0067_workshop_recorder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopCopperProjectorForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0068WorkshopProjector() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop copper projector for domestic communication [craft_industrialised_telecom_media_photography_0068_workshop_projector]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0068_workshop_projector"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop copper projector for domestic communication [craft_industrialised_telecom_media_photography_0068_workshop_projector]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop copper projector for domestic communication from prepared copper stock.",
		Action = "making workshop copper projector for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop copper projector for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3823.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3823 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0068_workshop_projector", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop copper projector for domestic communication (@industrialised_telecom_media_photography_0068_workshop_projector)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopGlassMicrophoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0069WorkshopMicrophone() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop glass microphone for domestic communication [craft_industrialised_telecom_media_photography_0069_workshop_microphone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0069_workshop_microphone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop glass microphone for domestic communication [craft_industrialised_telecom_media_photography_0069_workshop_microphone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop glass microphone for domestic communication from prepared glass stock.",
		Action = "making workshop glass microphone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop glass microphone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3863.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3863 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0069_workshop_microphone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop glass microphone for domestic communication (@industrialised_telecom_media_photography_0069_workshop_microphone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeWorkshopNaturalRubberCableForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0070WorkshopCable() => new()
	{
		Metadata = new("Telecom, media and photography/make workshop natural rubber cable for domestic communication [craft_industrialised_telecom_media_photography_0070_workshop_cable]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0070_workshop_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop natural rubber cable for domestic communication [craft_industrialised_telecom_media_photography_0070_workshop_cable]",
		Category = "Telecom, media and photography",
		Blurb = "Make a workshop natural rubber cable for domestic communication from prepared natural rubber stock.",
		Action = "making workshop natural rubber cable for domestic communication",
		ActiveCraftItemSdesc = "an in-progress workshop natural rubber cable for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3904.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3904 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0070_workshop_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop natural rubber cable for domestic communication (@industrialised_telecom_media_photography_0070_workshop_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalMildSteelScrapForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0071InstitutionalScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional mild steel scrap for domestic communication [craft_industrialised_telecom_media_photography_0071_institutional_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0071_institutional_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel scrap for domestic communication [craft_industrialised_telecom_media_photography_0071_institutional_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional mild steel scrap for domestic communication from prepared mild steel stock.",
		Action = "making institutional mild steel scrap for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel scrap for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3945.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3945 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0071_institutional_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel scrap for domestic communication (@industrialised_telecom_media_photography_0071_institutional_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalCopperTelephoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0072InstitutionalTelephone() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional copper telephone for domestic communication [craft_industrialised_telecom_media_photography_0072_institutional_telephone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0072_institutional_telephone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper telephone for domestic communication [craft_industrialised_telecom_media_photography_0072_institutional_telephone]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional copper telephone for domestic communication from prepared copper stock.",
		Action = "making institutional copper telephone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional copper telephone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3985.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3985 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0072_institutional_telephone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper telephone for domestic communication (@industrialised_telecom_media_photography_0072_institutional_telephone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalGlassReceiverForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0073InstitutionalReceiver() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional glass receiver for domestic communication [craft_industrialised_telecom_media_photography_0073_institutional_receiver]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0073_institutional_receiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional glass receiver for domestic communication [craft_industrialised_telecom_media_photography_0073_institutional_receiver]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional glass receiver for domestic communication from prepared glass stock.",
		Action = "making institutional glass receiver for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional glass receiver for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4026.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4026 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0073_institutional_receiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional glass receiver for domestic communication (@industrialised_telecom_media_photography_0073_institutional_receiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalNaturalRubberCameraForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0074InstitutionalCamera() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional natural rubber camera for domestic communication [craft_industrialised_telecom_media_photography_0074_institutional_camera]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0074_institutional_camera"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional natural rubber camera for domestic communication [craft_industrialised_telecom_media_photography_0074_institutional_camera]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional natural rubber camera for domestic communication from prepared natural rubber stock.",
		Action = "making institutional natural rubber camera for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional natural rubber camera for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4067.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4067 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0074_institutional_camera", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional natural rubber camera for domestic communication (@industrialised_telecom_media_photography_0074_institutional_camera)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalMildSteelTripodForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0075InstitutionalTripod() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional mild steel tripod for domestic communication [craft_industrialised_telecom_media_photography_0075_institutional_tripod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0075_institutional_tripod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel tripod for domestic communication [craft_industrialised_telecom_media_photography_0075_institutional_tripod]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional mild steel tripod for domestic communication from prepared mild steel stock.",
		Action = "making institutional mild steel tripod for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel tripod for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4107.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4107 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0075_institutional_tripod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel tripod for domestic communication (@industrialised_telecom_media_photography_0075_institutional_tripod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalCopperSpeakerForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0076InstitutionalSpeaker() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional copper speaker for domestic communication [craft_industrialised_telecom_media_photography_0076_institutional_speaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0076_institutional_speaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper speaker for domestic communication [craft_industrialised_telecom_media_photography_0076_institutional_speaker]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional copper speaker for domestic communication from prepared copper stock.",
		Action = "making institutional copper speaker for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional copper speaker for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4148.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4148 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0076_institutional_speaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper speaker for domestic communication (@industrialised_telecom_media_photography_0076_institutional_speaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalGlassRecorderForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0077InstitutionalRecorder() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional glass recorder for domestic communication [craft_industrialised_telecom_media_photography_0077_institutional_recorder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0077_institutional_recorder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional glass recorder for domestic communication [craft_industrialised_telecom_media_photography_0077_institutional_recorder]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional glass recorder for domestic communication from prepared glass stock.",
		Action = "making institutional glass recorder for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional glass recorder for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4189.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4189 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0077_institutional_recorder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional glass recorder for domestic communication (@industrialised_telecom_media_photography_0077_institutional_recorder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalNaturalRubberProjectorForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0078InstitutionalProjector() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional natural rubber projector for domestic communication [craft_industrialised_telecom_media_photography_0078_institutional_projector]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0078_institutional_projector"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional natural rubber projector for domestic communication [craft_industrialised_telecom_media_photography_0078_institutional_projector]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional natural rubber projector for domestic communication from prepared natural rubber stock.",
		Action = "making institutional natural rubber projector for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional natural rubber projector for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4230.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4230 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0078_institutional_projector", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional natural rubber projector for domestic communication (@industrialised_telecom_media_photography_0078_institutional_projector)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalMildSteelMicrophoneForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0079InstitutionalMicrophone() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional mild steel microphone for domestic communication [craft_industrialised_telecom_media_photography_0079_institutional_microphone]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0079_institutional_microphone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel microphone for domestic communication [craft_industrialised_telecom_media_photography_0079_institutional_microphone]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional mild steel microphone for domestic communication from prepared mild steel stock.",
		Action = "making institutional mild steel microphone for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel microphone for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4270.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4270 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0079_institutional_microphone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel microphone for domestic communication (@industrialised_telecom_media_photography_0079_institutional_microphone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeInstitutionalCopperCableForDomesticCommunicationCraftIndustrialisedTelecomMediaPhotography0080InstitutionalCable() => new()
	{
		Metadata = new("Telecom, media and photography/make institutional copper cable for domestic communication [craft_industrialised_telecom_media_photography_0080_institutional_cable]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_telecom_media_photography_0080_institutional_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper cable for domestic communication [craft_industrialised_telecom_media_photography_0080_institutional_cable]",
		Category = "Telecom, media and photography",
		Blurb = "Make an institutional copper cable for domestic communication from prepared copper stock.",
		Action = "making institutional copper cable for domestic communication",
		ActiveCraftItemSdesc = "an in-progress institutional copper cable for domestic communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4311.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4311 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_telecom_media_photography_0080_institutional_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper cable for domestic communication (@industrialised_telecom_media_photography_0080_institutional_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactGlassScrapForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0001CompactScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make compact glass scrap for workshop communication [craft_industrial_telecom_media_photography_0001_compact_scrap]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0001_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass scrap for workshop communication [craft_industrial_telecom_media_photography_0001_compact_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact glass scrap for workshop communication from prepared glass stock.",
		Action = "making compact glass scrap for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact glass scrap for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0001_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass scrap for workshop communication (@industrial_telecom_media_photography_0001_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactNaturalRubberTelephoneForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0002CompactTelephone() => new()
	{
		Metadata = new("Telecom, media and photography/make compact natural rubber telephone for workshop communication [craft_industrial_telecom_media_photography_0002_compact_telephone]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0002_compact_telephone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber telephone for workshop communication [craft_industrial_telecom_media_photography_0002_compact_telephone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact natural rubber telephone for workshop communication from prepared natural rubber stock.",
		Action = "making compact natural rubber telephone for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber telephone for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0002_compact_telephone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber telephone for workshop communication (@industrial_telecom_media_photography_0002_compact_telephone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactMildSteelReceiverForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0003CompactReceiver() => new()
	{
		Metadata = new("Telecom, media and photography/make compact mild steel receiver for workshop communication [craft_industrial_telecom_media_photography_0003_compact_receiver]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0003_compact_receiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel receiver for workshop communication [craft_industrial_telecom_media_photography_0003_compact_receiver]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact mild steel receiver for workshop communication from prepared mild steel stock.",
		Action = "making compact mild steel receiver for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact mild steel receiver for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0003_compact_receiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel receiver for workshop communication (@industrial_telecom_media_photography_0003_compact_receiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactCopperCameraForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0004CompactCamera() => new()
	{
		Metadata = new("Telecom, media and photography/make compact copper camera for workshop communication [craft_industrial_telecom_media_photography_0004_compact_camera]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0004_compact_camera"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper camera for workshop communication [craft_industrial_telecom_media_photography_0004_compact_camera]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact copper camera for workshop communication from prepared copper stock.",
		Action = "making compact copper camera for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact copper camera for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0004_compact_camera", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper camera for workshop communication (@industrial_telecom_media_photography_0004_compact_camera)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactGlassTripodForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0005CompactTripod() => new()
	{
		Metadata = new("Telecom, media and photography/make compact glass tripod for workshop communication [craft_industrial_telecom_media_photography_0005_compact_tripod]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0005_compact_tripod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass tripod for workshop communication [craft_industrial_telecom_media_photography_0005_compact_tripod]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact glass tripod for workshop communication from prepared glass stock.",
		Action = "making compact glass tripod for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact glass tripod for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0005_compact_tripod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass tripod for workshop communication (@industrial_telecom_media_photography_0005_compact_tripod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactNaturalRubberSpeakerForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0006CompactSpeaker() => new()
	{
		Metadata = new("Telecom, media and photography/make compact natural rubber speaker for workshop communication [craft_industrial_telecom_media_photography_0006_compact_speaker]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0006_compact_speaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber speaker for workshop communication [craft_industrial_telecom_media_photography_0006_compact_speaker]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact natural rubber speaker for workshop communication from prepared natural rubber stock.",
		Action = "making compact natural rubber speaker for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber speaker for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0006_compact_speaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber speaker for workshop communication (@industrial_telecom_media_photography_0006_compact_speaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactMildSteelRecorderForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0007CompactRecorder() => new()
	{
		Metadata = new("Telecom, media and photography/make compact mild steel recorder for workshop communication [craft_industrial_telecom_media_photography_0007_compact_recorder]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0007_compact_recorder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel recorder for workshop communication [craft_industrial_telecom_media_photography_0007_compact_recorder]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact mild steel recorder for workshop communication from prepared mild steel stock.",
		Action = "making compact mild steel recorder for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact mild steel recorder for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0007_compact_recorder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel recorder for workshop communication (@industrial_telecom_media_photography_0007_compact_recorder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactCopperProjectorForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0008CompactProjector() => new()
	{
		Metadata = new("Telecom, media and photography/make compact copper projector for workshop communication [craft_industrial_telecom_media_photography_0008_compact_projector]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0008_compact_projector"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper projector for workshop communication [craft_industrial_telecom_media_photography_0008_compact_projector]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact copper projector for workshop communication from prepared copper stock.",
		Action = "making compact copper projector for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact copper projector for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0008_compact_projector", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper projector for workshop communication (@industrial_telecom_media_photography_0008_compact_projector)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactGlassMicrophoneForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0009CompactMicrophone() => new()
	{
		Metadata = new("Telecom, media and photography/make compact glass microphone for workshop communication [craft_industrial_telecom_media_photography_0009_compact_microphone]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0009_compact_microphone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass microphone for workshop communication [craft_industrial_telecom_media_photography_0009_compact_microphone]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact glass microphone for workshop communication from prepared glass stock.",
		Action = "making compact glass microphone for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact glass microphone for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0009_compact_microphone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass microphone for workshop communication (@industrial_telecom_media_photography_0009_compact_microphone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeCompactNaturalRubberCableForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0010CompactCable() => new()
	{
		Metadata = new("Telecom, media and photography/make compact natural rubber cable for workshop communication [craft_industrial_telecom_media_photography_0010_compact_cable]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0010_compact_cable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber cable for workshop communication [craft_industrial_telecom_media_photography_0010_compact_cable]",
		Category = "Telecom, media and photography",
		Blurb = "Make a compact natural rubber cable for workshop communication from prepared natural rubber stock.",
		Action = "making compact natural rubber cable for workshop communication",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber cable for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0010_compact_cable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber cable for workshop communication (@industrial_telecom_media_photography_0010_compact_cable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TelecomMediaAndPhotographyMakeHeavyDutyMildSteelScrapForWorkshopCommunicationCraftIndustrialTelecomMediaPhotography0011HeavyDutyScrap() => new()
	{
		Metadata = new("Telecom, media and photography/make heavy-duty mild steel scrap for workshop communication [craft_industrial_telecom_media_photography_0011_heavy_duty_scrap]", "crafts", ["industrial"], ["item:industrial_telecom_media_photography_0011_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for workshop communication [craft_industrial_telecom_media_photography_0011_heavy_duty_scrap]",
		Category = "Telecom, media and photography",
		Blurb = "Make a heavy-duty mild steel scrap for workshop communication from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for workshop communication",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for workshop communication craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_telecom_media_photography_0011_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for workshop communication (@industrial_telecom_media_photography_0011_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainMildSteelScrapForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0001PlainScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make plain mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0001_plain_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain mild steel scrap for domestic power service from prepared mild steel stock.",
		Action = "making plain mild steel scrap for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain mild steel scrap for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel scrap for domestic power service (@industrialised_electrical_power_lighting_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainGlassLampForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0002PlainLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make plain glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0002_plain_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0002_plain_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0002_plain_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain glass lamp for domestic power service from prepared glass stock.",
		Action = "making plain glass lamp for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain glass lamp for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0002_plain_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass lamp for domestic power service (@industrialised_electrical_power_lighting_0002_plain_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainCeramicSocketForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0003PlainSocket() => new()
	{
		Metadata = new("Electrical power and lighting/make plain ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0003_plain_socket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0003_plain_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0003_plain_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain ceramic socket for domestic power service from prepared ceramic stock.",
		Action = "making plain ceramic socket for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain ceramic socket for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0003_plain_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic socket for domestic power service (@industrialised_electrical_power_lighting_0003_plain_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainNaturalRubberSwitchForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0004PlainSwitch() => new()
	{
		Metadata = new("Electrical power and lighting/make plain natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0004_plain_switch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0004_plain_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0004_plain_switch]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain natural rubber switch for domestic power service from prepared natural rubber stock.",
		Action = "making plain natural rubber switch for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber switch for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0004_plain_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber switch for domestic power service (@industrialised_electrical_power_lighting_0004_plain_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainCopperFuseForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0005PlainFuse() => new()
	{
		Metadata = new("Electrical power and lighting/make plain copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0005_plain_fuse]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0005_plain_fuse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0005_plain_fuse]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain copper fuse for domestic power service from prepared copper stock.",
		Action = "making plain copper fuse for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain copper fuse for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0005_plain_fuse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper fuse for domestic power service (@industrialised_electrical_power_lighting_0005_plain_fuse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainMildSteelLanternForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0006PlainLantern() => new()
	{
		Metadata = new("Electrical power and lighting/make plain mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0006_plain_lantern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0006_plain_lantern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0006_plain_lantern]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain mild steel lantern for domestic power service from prepared mild steel stock.",
		Action = "making plain mild steel lantern for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain mild steel lantern for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0006_plain_lantern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel lantern for domestic power service (@industrialised_electrical_power_lighting_0006_plain_lantern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainGlassLeadForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0007PlainLead() => new()
	{
		Metadata = new("Electrical power and lighting/make plain glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0007_plain_lead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0007_plain_lead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0007_plain_lead]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain glass lead for domestic power service from prepared glass stock.",
		Action = "making plain glass lead for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain glass lead for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0007_plain_lead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass lead for domestic power service (@industrialised_electrical_power_lighting_0007_plain_lead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainCeramicJunctionForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0008PlainJunction() => new()
	{
		Metadata = new("Electrical power and lighting/make plain ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0008_plain_junction]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0008_plain_junction"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0008_plain_junction]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain ceramic junction for domestic power service from prepared ceramic stock.",
		Action = "making plain ceramic junction for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain ceramic junction for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0008_plain_junction", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic junction for domestic power service (@industrialised_electrical_power_lighting_0008_plain_junction)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainNaturalRubberMeterForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0009PlainMeter() => new()
	{
		Metadata = new("Electrical power and lighting/make plain natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0009_plain_meter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0009_plain_meter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0009_plain_meter]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain natural rubber meter for domestic power service from prepared natural rubber stock.",
		Action = "making plain natural rubber meter for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber meter for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0009_plain_meter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber meter for domestic power service (@industrialised_electrical_power_lighting_0009_plain_meter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakePlainCopperTransformerForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0010PlainTransformer() => new()
	{
		Metadata = new("Electrical power and lighting/make plain copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0010_plain_transformer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0010_plain_transformer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0010_plain_transformer]",
		Category = "Electrical power and lighting",
		Blurb = "Make a plain copper transformer for domestic power service from prepared copper stock.",
		Action = "making plain copper transformer for domestic power service",
		ActiveCraftItemSdesc = "an in-progress plain copper transformer for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0010_plain_transformer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper transformer for domestic power service (@industrialised_electrical_power_lighting_0010_plain_transformer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedMildSteelScrapForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0011ReinforcedScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0011_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0011_reinforced_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced mild steel scrap for domestic power service from prepared mild steel stock.",
		Action = "making reinforced mild steel scrap for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel scrap for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel scrap for domestic power service (@industrialised_electrical_power_lighting_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedGlassLampForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0012ReinforcedLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0012_reinforced_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0012_reinforced_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0012_reinforced_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced glass lamp for domestic power service from prepared glass stock.",
		Action = "making reinforced glass lamp for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced glass lamp for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1654.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1654 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0012_reinforced_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass lamp for domestic power service (@industrialised_electrical_power_lighting_0012_reinforced_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedCeramicSocketForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0013ReinforcedSocket() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0013_reinforced_socket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0013_reinforced_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0013_reinforced_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced ceramic socket for domestic power service from prepared ceramic stock.",
		Action = "making reinforced ceramic socket for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic socket for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1695.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1695 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0013_reinforced_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic socket for domestic power service (@industrialised_electrical_power_lighting_0013_reinforced_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedNaturalRubberSwitchForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0014ReinforcedSwitch() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0014_reinforced_switch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0014_reinforced_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0014_reinforced_switch]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced natural rubber switch for domestic power service from prepared natural rubber stock.",
		Action = "making reinforced natural rubber switch for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber switch for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1736.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1736 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0014_reinforced_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber switch for domestic power service (@industrialised_electrical_power_lighting_0014_reinforced_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedCopperFuseForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0015ReinforcedFuse() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0015_reinforced_fuse]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0015_reinforced_fuse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0015_reinforced_fuse]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced copper fuse for domestic power service from prepared copper stock.",
		Action = "making reinforced copper fuse for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced copper fuse for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1777.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1777 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0015_reinforced_fuse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper fuse for domestic power service (@industrialised_electrical_power_lighting_0015_reinforced_fuse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedMildSteelLanternForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0016ReinforcedLantern() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0016_reinforced_lantern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0016_reinforced_lantern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel lantern for domestic power service [craft_industrialised_electrical_power_lighting_0016_reinforced_lantern]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced mild steel lantern for domestic power service from prepared mild steel stock.",
		Action = "making reinforced mild steel lantern for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel lantern for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1817.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1817 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0016_reinforced_lantern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel lantern for domestic power service (@industrialised_electrical_power_lighting_0016_reinforced_lantern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedGlassLeadForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0017ReinforcedLead() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0017_reinforced_lead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0017_reinforced_lead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass lead for domestic power service [craft_industrialised_electrical_power_lighting_0017_reinforced_lead]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced glass lead for domestic power service from prepared glass stock.",
		Action = "making reinforced glass lead for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced glass lead for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1858.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1858 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0017_reinforced_lead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass lead for domestic power service (@industrialised_electrical_power_lighting_0017_reinforced_lead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedCeramicJunctionForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0018ReinforcedJunction() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0018_reinforced_junction]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0018_reinforced_junction"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic junction for domestic power service [craft_industrialised_electrical_power_lighting_0018_reinforced_junction]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced ceramic junction for domestic power service from prepared ceramic stock.",
		Action = "making reinforced ceramic junction for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic junction for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1899.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1899 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0018_reinforced_junction", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic junction for domestic power service (@industrialised_electrical_power_lighting_0018_reinforced_junction)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedNaturalRubberMeterForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0019ReinforcedMeter() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0019_reinforced_meter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0019_reinforced_meter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber meter for domestic power service [craft_industrialised_electrical_power_lighting_0019_reinforced_meter]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced natural rubber meter for domestic power service from prepared natural rubber stock.",
		Action = "making reinforced natural rubber meter for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber meter for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1939.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1939 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0019_reinforced_meter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber meter for domestic power service (@industrialised_electrical_power_lighting_0019_reinforced_meter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeReinforcedCopperTransformerForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0020ReinforcedTransformer() => new()
	{
		Metadata = new("Electrical power and lighting/make reinforced copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0020_reinforced_transformer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0020_reinforced_transformer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper transformer for domestic power service [craft_industrialised_electrical_power_lighting_0020_reinforced_transformer]",
		Category = "Electrical power and lighting",
		Blurb = "Make a reinforced copper transformer for domestic power service from prepared copper stock.",
		Action = "making reinforced copper transformer for domestic power service",
		ActiveCraftItemSdesc = "an in-progress reinforced copper transformer for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1980.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1980 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0020_reinforced_transformer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper transformer for domestic power service (@industrialised_electrical_power_lighting_0020_reinforced_transformer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactMildSteelScrapForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0021CompactScrap() => new()
	{
		Metadata = new("Electrical power and lighting/make compact mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0021_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel scrap for domestic power service [craft_industrialised_electrical_power_lighting_0021_compact_scrap]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact mild steel scrap for domestic power service from prepared mild steel stock.",
		Action = "making compact mild steel scrap for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact mild steel scrap for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2021.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2021 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel scrap for domestic power service (@industrialised_electrical_power_lighting_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactGlassLampForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0022CompactLamp() => new()
	{
		Metadata = new("Electrical power and lighting/make compact glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0022_compact_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0022_compact_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass lamp for domestic power service [craft_industrialised_electrical_power_lighting_0022_compact_lamp]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact glass lamp for domestic power service from prepared glass stock.",
		Action = "making compact glass lamp for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact glass lamp for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2061.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2061 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0022_compact_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass lamp for domestic power service (@industrialised_electrical_power_lighting_0022_compact_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactCeramicSocketForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0023CompactSocket() => new()
	{
		Metadata = new("Electrical power and lighting/make compact ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0023_compact_socket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0023_compact_socket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic socket for domestic power service [craft_industrialised_electrical_power_lighting_0023_compact_socket]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact ceramic socket for domestic power service from prepared ceramic stock.",
		Action = "making compact ceramic socket for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact ceramic socket for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2102.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2102 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0023_compact_socket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic socket for domestic power service (@industrialised_electrical_power_lighting_0023_compact_socket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactNaturalRubberSwitchForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0024CompactSwitch() => new()
	{
		Metadata = new("Electrical power and lighting/make compact natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0024_compact_switch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0024_compact_switch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber switch for domestic power service [craft_industrialised_electrical_power_lighting_0024_compact_switch]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact natural rubber switch for domestic power service from prepared natural rubber stock.",
		Action = "making compact natural rubber switch for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber switch for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2143.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2143 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0024_compact_switch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber switch for domestic power service (@industrialised_electrical_power_lighting_0024_compact_switch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ElectricalPowerAndLightingMakeCompactCopperFuseForDomesticPowerServiceCraftIndustrialisedElectricalPowerLighting0025CompactFuse() => new()
	{
		Metadata = new("Electrical power and lighting/make compact copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0025_compact_fuse]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_electrical_power_lighting_0025_compact_fuse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper fuse for domestic power service [craft_industrialised_electrical_power_lighting_0025_compact_fuse]",
		Category = "Electrical power and lighting",
		Blurb = "Make a compact copper fuse for domestic power service from prepared copper stock.",
		Action = "making compact copper fuse for domestic power service",
		ActiveCraftItemSdesc = "an in-progress compact copper fuse for domestic power service craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2184.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2184 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_electrical_power_lighting_0025_compact_fuse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper fuse for domestic power service (@industrialised_electrical_power_lighting_0025_compact_fuse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
