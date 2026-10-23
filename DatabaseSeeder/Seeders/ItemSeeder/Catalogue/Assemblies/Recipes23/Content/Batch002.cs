#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes23Stock
{
	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactMildSteelFluteForWorkshopRecreationCraftIndustrialSportsRecreationMusic0002CompactFlute() => new()
	{
		Metadata = new("Sports, recreation and music/make compact mild steel flute for workshop recreation [craft_industrial_sports_recreation_music_0002_compact_flute]", "crafts", ["industrial"], ["item:industrial_sports_recreation_music_0002_compact_flute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel flute for workshop recreation [craft_industrial_sports_recreation_music_0002_compact_flute]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact mild steel flute for workshop recreation from prepared mild steel stock.",
		Action = "making compact mild steel flute for workshop recreation",
		ActiveCraftItemSdesc = "an in-progress compact mild steel flute for workshop recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2358.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2358 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_sports_recreation_music_0002_compact_flute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel flute for workshop recreation (@industrial_sports_recreation_music_0002_compact_flute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactOakBoardForWorkshopRecreationCraftIndustrialSportsRecreationMusic0003CompactBoard() => new()
	{
		Metadata = new("Sports, recreation and music/make compact oak board for workshop recreation [craft_industrial_sports_recreation_music_0003_compact_board]", "crafts", ["industrial"], ["item:industrial_sports_recreation_music_0003_compact_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak board for workshop recreation [craft_industrial_sports_recreation_music_0003_compact_board]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact oak board for workshop recreation from prepared oak stock.",
		Action = "making compact oak board for workshop recreation",
		ActiveCraftItemSdesc = "an in-progress compact oak board for workshop recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2399.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2399 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_sports_recreation_music_0003_compact_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak board for workshop recreation (@industrial_sports_recreation_music_0003_compact_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactLeatherCounterForWorkshopRecreationCraftIndustrialSportsRecreationMusic0004CompactCounter() => new()
	{
		Metadata = new("Sports, recreation and music/make compact leather counter for workshop recreation [craft_industrial_sports_recreation_music_0004_compact_counter]", "crafts", ["industrial"], ["item:industrial_sports_recreation_music_0004_compact_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather counter for workshop recreation [craft_industrial_sports_recreation_music_0004_compact_counter]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact leather counter for workshop recreation from prepared leather stock.",
		Action = "making compact leather counter for workshop recreation",
		ActiveCraftItemSdesc = "an in-progress compact leather counter for workshop recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2440.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2440 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_sports_recreation_music_0004_compact_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather counter for workshop recreation (@industrial_sports_recreation_music_0004_compact_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainCottonScrapForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0001PlainScrap() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain cotton scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0001_plain_scrap]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain cotton scrap for domestic emergency response from prepared cotton stock.",
		Action = "making plain cotton scrap for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain cotton scrap for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2429.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2429 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton scrap for domestic emergency response (@industrialised_emergency_rescue_civic_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainNaturalRubberBucketForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0002PlainBucket() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain natural rubber bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0002_plain_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0002_plain_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0002_plain_bucket]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain natural rubber bucket for domestic emergency response from prepared natural rubber stock.",
		Action = "making plain natural rubber bucket for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber bucket for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2470.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2470 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0002_plain_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber bucket for domestic emergency response (@industrialised_emergency_rescue_civic_0002_plain_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainMildSteelLadderForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0003PlainLadder() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain mild steel ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0003_plain_ladder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0003_plain_ladder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0003_plain_ladder]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain mild steel ladder for domestic emergency response from prepared mild steel stock.",
		Action = "making plain mild steel ladder for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain mild steel ladder for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2510.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2510 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0003_plain_ladder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel ladder for domestic emergency response (@industrialised_emergency_rescue_civic_0003_plain_ladder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainOakBellForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0004PlainBell() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain oak bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0004_plain_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0004_plain_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0004_plain_bell]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain oak bell for domestic emergency response from prepared oak stock.",
		Action = "making plain oak bell for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain oak bell for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2551.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2551 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0004_plain_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak bell for domestic emergency response (@industrialised_emergency_rescue_civic_0004_plain_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainCottonStretcherForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0005PlainStretcher() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain cotton stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0005_plain_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0005_plain_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0005_plain_stretcher]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain cotton stretcher for domestic emergency response from prepared cotton stock.",
		Action = "making plain cotton stretcher for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain cotton stretcher for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2592.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2592 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0005_plain_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton stretcher for domestic emergency response (@industrialised_emergency_rescue_civic_0005_plain_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainNaturalRubberHelmetForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0006PlainHelmet() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain natural rubber helmet for domestic emergency response [craft_industrialised_emergency_rescue_civic_0006_plain_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0006_plain_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber helmet for domestic emergency response [craft_industrialised_emergency_rescue_civic_0006_plain_helmet]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain natural rubber helmet for domestic emergency response from prepared natural rubber stock.",
		Action = "making plain natural rubber helmet for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber helmet for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2632.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2632 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0006_plain_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber helmet for domestic emergency response (@industrialised_emergency_rescue_civic_0006_plain_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainMildSteelHoseForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0007PlainHose() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain mild steel hose for domestic emergency response [craft_industrialised_emergency_rescue_civic_0007_plain_hose]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0007_plain_hose"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel hose for domestic emergency response [craft_industrialised_emergency_rescue_civic_0007_plain_hose]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain mild steel hose for domestic emergency response from prepared mild steel stock.",
		Action = "making plain mild steel hose for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain mild steel hose for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2673.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2673 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0007_plain_hose", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel hose for domestic emergency response (@industrialised_emergency_rescue_civic_0007_plain_hose)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainOakLampForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0008PlainLamp() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain oak lamp for domestic emergency response [craft_industrialised_emergency_rescue_civic_0008_plain_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0008_plain_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak lamp for domestic emergency response [craft_industrialised_emergency_rescue_civic_0008_plain_lamp]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain oak lamp for domestic emergency response from prepared oak stock.",
		Action = "making plain oak lamp for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain oak lamp for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2714.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2714 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0008_plain_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak lamp for domestic emergency response (@industrialised_emergency_rescue_civic_0008_plain_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainCottonBarrierForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0009PlainBarrier() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain cotton barrier for domestic emergency response [craft_industrialised_emergency_rescue_civic_0009_plain_barrier]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0009_plain_barrier"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton barrier for domestic emergency response [craft_industrialised_emergency_rescue_civic_0009_plain_barrier]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain cotton barrier for domestic emergency response from prepared cotton stock.",
		Action = "making plain cotton barrier for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain cotton barrier for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2754.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2754 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0009_plain_barrier", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton barrier for domestic emergency response (@industrialised_emergency_rescue_civic_0009_plain_barrier)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakePlainNaturalRubberSignForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0010PlainSign() => new()
	{
		Metadata = new("Emergency, rescue and civic/make plain natural rubber sign for domestic emergency response [craft_industrialised_emergency_rescue_civic_0010_plain_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0010_plain_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber sign for domestic emergency response [craft_industrialised_emergency_rescue_civic_0010_plain_sign]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a plain natural rubber sign for domestic emergency response from prepared natural rubber stock.",
		Action = "making plain natural rubber sign for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber sign for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2795.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2795 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0010_plain_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber sign for domestic emergency response (@industrialised_emergency_rescue_civic_0010_plain_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedMildSteelScrapForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0011ReinforcedScrap() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced mild steel scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0011_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0011_reinforced_scrap]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced mild steel scrap for domestic emergency response from prepared mild steel stock.",
		Action = "making reinforced mild steel scrap for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel scrap for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2836.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2836 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel scrap for domestic emergency response (@industrialised_emergency_rescue_civic_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedOakBucketForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0012ReinforcedBucket() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced oak bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0012_reinforced_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0012_reinforced_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0012_reinforced_bucket]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced oak bucket for domestic emergency response from prepared oak stock.",
		Action = "making reinforced oak bucket for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced oak bucket for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2877.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2877 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0012_reinforced_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak bucket for domestic emergency response (@industrialised_emergency_rescue_civic_0012_reinforced_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedCottonLadderForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0013ReinforcedLadder() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced cotton ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0013_reinforced_ladder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0013_reinforced_ladder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0013_reinforced_ladder]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced cotton ladder for domestic emergency response from prepared cotton stock.",
		Action = "making reinforced cotton ladder for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton ladder for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2917.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2917 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0013_reinforced_ladder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton ladder for domestic emergency response (@industrialised_emergency_rescue_civic_0013_reinforced_ladder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedNaturalRubberBellForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0014ReinforcedBell() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced natural rubber bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0014_reinforced_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0014_reinforced_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0014_reinforced_bell]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced natural rubber bell for domestic emergency response from prepared natural rubber stock.",
		Action = "making reinforced natural rubber bell for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber bell for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2958.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2958 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0014_reinforced_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber bell for domestic emergency response (@industrialised_emergency_rescue_civic_0014_reinforced_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedMildSteelStretcherForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0015ReinforcedStretcher() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced mild steel stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0015_reinforced_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0015_reinforced_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0015_reinforced_stretcher]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced mild steel stretcher for domestic emergency response from prepared mild steel stock.",
		Action = "making reinforced mild steel stretcher for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel stretcher for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2999.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2999 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0015_reinforced_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel stretcher for domestic emergency response (@industrialised_emergency_rescue_civic_0015_reinforced_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedOakHelmetForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0016ReinforcedHelmet() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced oak helmet for domestic emergency response [craft_industrialised_emergency_rescue_civic_0016_reinforced_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0016_reinforced_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak helmet for domestic emergency response [craft_industrialised_emergency_rescue_civic_0016_reinforced_helmet]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced oak helmet for domestic emergency response from prepared oak stock.",
		Action = "making reinforced oak helmet for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced oak helmet for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3039.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3039 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0016_reinforced_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak helmet for domestic emergency response (@industrialised_emergency_rescue_civic_0016_reinforced_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedCottonHoseForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0017ReinforcedHose() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced cotton hose for domestic emergency response [craft_industrialised_emergency_rescue_civic_0017_reinforced_hose]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0017_reinforced_hose"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton hose for domestic emergency response [craft_industrialised_emergency_rescue_civic_0017_reinforced_hose]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced cotton hose for domestic emergency response from prepared cotton stock.",
		Action = "making reinforced cotton hose for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton hose for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3080.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3080 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0017_reinforced_hose", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton hose for domestic emergency response (@industrialised_emergency_rescue_civic_0017_reinforced_hose)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedNaturalRubberLampForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0018ReinforcedLamp() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced natural rubber lamp for domestic emergency response [craft_industrialised_emergency_rescue_civic_0018_reinforced_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0018_reinforced_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber lamp for domestic emergency response [craft_industrialised_emergency_rescue_civic_0018_reinforced_lamp]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced natural rubber lamp for domestic emergency response from prepared natural rubber stock.",
		Action = "making reinforced natural rubber lamp for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber lamp for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3121.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3121 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0018_reinforced_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber lamp for domestic emergency response (@industrialised_emergency_rescue_civic_0018_reinforced_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedMildSteelBarrierForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0019ReinforcedBarrier() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced mild steel barrier for domestic emergency response [craft_industrialised_emergency_rescue_civic_0019_reinforced_barrier]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0019_reinforced_barrier"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel barrier for domestic emergency response [craft_industrialised_emergency_rescue_civic_0019_reinforced_barrier]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced mild steel barrier for domestic emergency response from prepared mild steel stock.",
		Action = "making reinforced mild steel barrier for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel barrier for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3161.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3161 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0019_reinforced_barrier", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel barrier for domestic emergency response (@industrialised_emergency_rescue_civic_0019_reinforced_barrier)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeReinforcedOakSignForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0020ReinforcedSign() => new()
	{
		Metadata = new("Emergency, rescue and civic/make reinforced oak sign for domestic emergency response [craft_industrialised_emergency_rescue_civic_0020_reinforced_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0020_reinforced_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak sign for domestic emergency response [craft_industrialised_emergency_rescue_civic_0020_reinforced_sign]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a reinforced oak sign for domestic emergency response from prepared oak stock.",
		Action = "making reinforced oak sign for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress reinforced oak sign for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3202.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3202 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0020_reinforced_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak sign for domestic emergency response (@industrialised_emergency_rescue_civic_0020_reinforced_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactCottonScrapForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0021CompactScrap() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact cotton scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0021_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0021_compact_scrap]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact cotton scrap for domestic emergency response from prepared cotton stock.",
		Action = "making compact cotton scrap for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact cotton scrap for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3243.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3243 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton scrap for domestic emergency response (@industrialised_emergency_rescue_civic_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactNaturalRubberBucketForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0022CompactBucket() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact natural rubber bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0022_compact_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0022_compact_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0022_compact_bucket]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact natural rubber bucket for domestic emergency response from prepared natural rubber stock.",
		Action = "making compact natural rubber bucket for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber bucket for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3284.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3284 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0022_compact_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber bucket for domestic emergency response (@industrialised_emergency_rescue_civic_0022_compact_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactMildSteelLadderForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0023CompactLadder() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact mild steel ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0023_compact_ladder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0023_compact_ladder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0023_compact_ladder]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact mild steel ladder for domestic emergency response from prepared mild steel stock.",
		Action = "making compact mild steel ladder for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact mild steel ladder for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3324.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3324 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0023_compact_ladder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel ladder for domestic emergency response (@industrialised_emergency_rescue_civic_0023_compact_ladder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactOakBellForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0024CompactBell() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact oak bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0024_compact_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0024_compact_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0024_compact_bell]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact oak bell for domestic emergency response from prepared oak stock.",
		Action = "making compact oak bell for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact oak bell for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3365.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3365 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0024_compact_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak bell for domestic emergency response (@industrialised_emergency_rescue_civic_0024_compact_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactCottonStretcherForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0025CompactStretcher() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact cotton stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0025_compact_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0025_compact_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0025_compact_stretcher]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact cotton stretcher for domestic emergency response from prepared cotton stock.",
		Action = "making compact cotton stretcher for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact cotton stretcher for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3406.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3406 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0025_compact_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton stretcher for domestic emergency response (@industrialised_emergency_rescue_civic_0025_compact_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactNaturalRubberHelmetForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0026CompactHelmet() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact natural rubber helmet for domestic emergency response [craft_industrialised_emergency_rescue_civic_0026_compact_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0026_compact_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber helmet for domestic emergency response [craft_industrialised_emergency_rescue_civic_0026_compact_helmet]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact natural rubber helmet for domestic emergency response from prepared natural rubber stock.",
		Action = "making compact natural rubber helmet for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber helmet for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3446.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3446 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0026_compact_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber helmet for domestic emergency response (@industrialised_emergency_rescue_civic_0026_compact_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactMildSteelHoseForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0027CompactHose() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact mild steel hose for domestic emergency response [craft_industrialised_emergency_rescue_civic_0027_compact_hose]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0027_compact_hose"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel hose for domestic emergency response [craft_industrialised_emergency_rescue_civic_0027_compact_hose]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact mild steel hose for domestic emergency response from prepared mild steel stock.",
		Action = "making compact mild steel hose for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact mild steel hose for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3487.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3487 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0027_compact_hose", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel hose for domestic emergency response (@industrialised_emergency_rescue_civic_0027_compact_hose)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactOakLampForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0028CompactLamp() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact oak lamp for domestic emergency response [craft_industrialised_emergency_rescue_civic_0028_compact_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0028_compact_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak lamp for domestic emergency response [craft_industrialised_emergency_rescue_civic_0028_compact_lamp]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact oak lamp for domestic emergency response from prepared oak stock.",
		Action = "making compact oak lamp for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact oak lamp for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3528.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3528 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0028_compact_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak lamp for domestic emergency response (@industrialised_emergency_rescue_civic_0028_compact_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactCottonBarrierForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0029CompactBarrier() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact cotton barrier for domestic emergency response [craft_industrialised_emergency_rescue_civic_0029_compact_barrier]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0029_compact_barrier"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton barrier for domestic emergency response [craft_industrialised_emergency_rescue_civic_0029_compact_barrier]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact cotton barrier for domestic emergency response from prepared cotton stock.",
		Action = "making compact cotton barrier for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact cotton barrier for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3568.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3568 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0029_compact_barrier", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton barrier for domestic emergency response (@industrialised_emergency_rescue_civic_0029_compact_barrier)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeCompactNaturalRubberSignForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0030CompactSign() => new()
	{
		Metadata = new("Emergency, rescue and civic/make compact natural rubber sign for domestic emergency response [craft_industrialised_emergency_rescue_civic_0030_compact_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0030_compact_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact natural rubber sign for domestic emergency response [craft_industrialised_emergency_rescue_civic_0030_compact_sign]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a compact natural rubber sign for domestic emergency response from prepared natural rubber stock.",
		Action = "making compact natural rubber sign for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress compact natural rubber sign for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3609.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3609 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0030_compact_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact natural rubber sign for domestic emergency response (@industrialised_emergency_rescue_civic_0030_compact_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyMildSteelScrapForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0031HeavyDutyScrap() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty mild steel scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0031_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0031_heavy_duty_scrap]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty mild steel scrap for domestic emergency response from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3650.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3650 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for domestic emergency response (@industrialised_emergency_rescue_civic_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyOakBucketForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0032HeavyDutyBucket() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty oak bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0032_heavy_duty_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0032_heavy_duty_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0032_heavy_duty_bucket]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty oak bucket for domestic emergency response from prepared oak stock.",
		Action = "making heavy-duty oak bucket for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak bucket for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3691.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3691 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0032_heavy_duty_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak bucket for domestic emergency response (@industrialised_emergency_rescue_civic_0032_heavy_duty_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyCottonLadderForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0033HeavyDutyLadder() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty cotton ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0033_heavy_duty_ladder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0033_heavy_duty_ladder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0033_heavy_duty_ladder]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty cotton ladder for domestic emergency response from prepared cotton stock.",
		Action = "making heavy-duty cotton ladder for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton ladder for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3731.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3731 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0033_heavy_duty_ladder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton ladder for domestic emergency response (@industrialised_emergency_rescue_civic_0033_heavy_duty_ladder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyNaturalRubberBellForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0034HeavyDutyBell() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty natural rubber bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0034_heavy_duty_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0034_heavy_duty_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0034_heavy_duty_bell]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty natural rubber bell for domestic emergency response from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber bell for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber bell for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3772.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3772 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0034_heavy_duty_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber bell for domestic emergency response (@industrialised_emergency_rescue_civic_0034_heavy_duty_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyMildSteelStretcherForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0035HeavyDutyStretcher() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty mild steel stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0035_heavy_duty_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0035_heavy_duty_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0035_heavy_duty_stretcher]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty mild steel stretcher for domestic emergency response from prepared mild steel stock.",
		Action = "making heavy-duty mild steel stretcher for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel stretcher for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3813.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3813 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0035_heavy_duty_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel stretcher for domestic emergency response (@industrialised_emergency_rescue_civic_0035_heavy_duty_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyOakHelmetForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0036HeavyDutyHelmet() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty oak helmet for domestic emergency response [craft_industrialised_emergency_rescue_civic_0036_heavy_duty_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0036_heavy_duty_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak helmet for domestic emergency response [craft_industrialised_emergency_rescue_civic_0036_heavy_duty_helmet]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty oak helmet for domestic emergency response from prepared oak stock.",
		Action = "making heavy-duty oak helmet for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak helmet for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3853.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3853 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0036_heavy_duty_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak helmet for domestic emergency response (@industrialised_emergency_rescue_civic_0036_heavy_duty_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyCottonHoseForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0037HeavyDutyHose() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty cotton hose for domestic emergency response [craft_industrialised_emergency_rescue_civic_0037_heavy_duty_hose]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0037_heavy_duty_hose"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton hose for domestic emergency response [craft_industrialised_emergency_rescue_civic_0037_heavy_duty_hose]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty cotton hose for domestic emergency response from prepared cotton stock.",
		Action = "making heavy-duty cotton hose for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton hose for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3894.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3894 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0037_heavy_duty_hose", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton hose for domestic emergency response (@industrialised_emergency_rescue_civic_0037_heavy_duty_hose)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyNaturalRubberLampForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0038HeavyDutyLamp() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty natural rubber lamp for domestic emergency response [craft_industrialised_emergency_rescue_civic_0038_heavy_duty_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0038_heavy_duty_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty natural rubber lamp for domestic emergency response [craft_industrialised_emergency_rescue_civic_0038_heavy_duty_lamp]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty natural rubber lamp for domestic emergency response from prepared natural rubber stock.",
		Action = "making heavy-duty natural rubber lamp for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty natural rubber lamp for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3935.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3935 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0038_heavy_duty_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty natural rubber lamp for domestic emergency response (@industrialised_emergency_rescue_civic_0038_heavy_duty_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyMildSteelBarrierForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0039HeavyDutyBarrier() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty mild steel barrier for domestic emergency response [craft_industrialised_emergency_rescue_civic_0039_heavy_duty_barrier]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0039_heavy_duty_barrier"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel barrier for domestic emergency response [craft_industrialised_emergency_rescue_civic_0039_heavy_duty_barrier]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty mild steel barrier for domestic emergency response from prepared mild steel stock.",
		Action = "making heavy-duty mild steel barrier for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel barrier for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3975.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3975 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0039_heavy_duty_barrier", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel barrier for domestic emergency response (@industrialised_emergency_rescue_civic_0039_heavy_duty_barrier)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeHeavyDutyOakSignForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0040HeavyDutySign() => new()
	{
		Metadata = new("Emergency, rescue and civic/make heavy-duty oak sign for domestic emergency response [craft_industrialised_emergency_rescue_civic_0040_heavy_duty_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0040_heavy_duty_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak sign for domestic emergency response [craft_industrialised_emergency_rescue_civic_0040_heavy_duty_sign]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a heavy-duty oak sign for domestic emergency response from prepared oak stock.",
		Action = "making heavy-duty oak sign for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak sign for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4016.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4016 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0040_heavy_duty_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak sign for domestic emergency response (@industrialised_emergency_rescue_civic_0040_heavy_duty_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeFoldingCottonScrapForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0041FoldingScrap() => new()
	{
		Metadata = new("Emergency, rescue and civic/make folding cotton scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0041_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0041_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton scrap for domestic emergency response [craft_industrialised_emergency_rescue_civic_0041_folding_scrap]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a folding cotton scrap for domestic emergency response from prepared cotton stock.",
		Action = "making folding cotton scrap for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress folding cotton scrap for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4057.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4057 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0041_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton scrap for domestic emergency response (@industrialised_emergency_rescue_civic_0041_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeFoldingNaturalRubberBucketForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0042FoldingBucket() => new()
	{
		Metadata = new("Emergency, rescue and civic/make folding natural rubber bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0042_folding_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0042_folding_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding natural rubber bucket for domestic emergency response [craft_industrialised_emergency_rescue_civic_0042_folding_bucket]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a folding natural rubber bucket for domestic emergency response from prepared natural rubber stock.",
		Action = "making folding natural rubber bucket for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress folding natural rubber bucket for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4098.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4098 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0042_folding_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding natural rubber bucket for domestic emergency response (@industrialised_emergency_rescue_civic_0042_folding_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeFoldingMildSteelLadderForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0043FoldingLadder() => new()
	{
		Metadata = new("Emergency, rescue and civic/make folding mild steel ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0043_folding_ladder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0043_folding_ladder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel ladder for domestic emergency response [craft_industrialised_emergency_rescue_civic_0043_folding_ladder]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a folding mild steel ladder for domestic emergency response from prepared mild steel stock.",
		Action = "making folding mild steel ladder for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress folding mild steel ladder for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4138.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4138 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0043_folding_ladder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel ladder for domestic emergency response (@industrialised_emergency_rescue_civic_0043_folding_ladder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeFoldingOakBellForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0044FoldingBell() => new()
	{
		Metadata = new("Emergency, rescue and civic/make folding oak bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0044_folding_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0044_folding_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak bell for domestic emergency response [craft_industrialised_emergency_rescue_civic_0044_folding_bell]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a folding oak bell for domestic emergency response from prepared oak stock.",
		Action = "making folding oak bell for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress folding oak bell for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4179.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4179 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0044_folding_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak bell for domestic emergency response (@industrialised_emergency_rescue_civic_0044_folding_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeFoldingCottonStretcherForDomesticEmergencyResponseCraftIndustrialisedEmergencyRescueCivic0045FoldingStretcher() => new()
	{
		Metadata = new("Emergency, rescue and civic/make folding cotton stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0045_folding_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_emergency_rescue_civic_0045_folding_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton stretcher for domestic emergency response [craft_industrialised_emergency_rescue_civic_0045_folding_stretcher]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a folding cotton stretcher for domestic emergency response from prepared cotton stock.",
		Action = "making folding cotton stretcher for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress folding cotton stretcher for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4220.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4220 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_emergency_rescue_civic_0045_folding_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton stretcher for domestic emergency response (@industrialised_emergency_rescue_civic_0045_folding_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeLinedNaturalRubberHelmetForDomesticEmergencyResponseCraftIndustrialEmergencyRescueCivic0001LinedHelmet() => new()
	{
		Metadata = new("Emergency, rescue and civic/make lined natural rubber helmet for domestic emergency response [craft_industrial_emergency_rescue_civic_0001_lined_helmet]", "crafts", ["industrial"], ["item:industrial_emergency_rescue_civic_0001_lined_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined natural rubber helmet for domestic emergency response [craft_industrial_emergency_rescue_civic_0001_lined_helmet]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a lined natural rubber helmet for domestic emergency response from prepared natural rubber stock.",
		Action = "making lined natural rubber helmet for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress lined natural rubber helmet for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2429.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2429 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_emergency_rescue_civic_0001_lined_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined natural rubber helmet for domestic emergency response (@industrial_emergency_rescue_civic_0001_lined_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeLinedMildSteelHoseForDomesticEmergencyResponseCraftIndustrialEmergencyRescueCivic0002LinedHose() => new()
	{
		Metadata = new("Emergency, rescue and civic/make lined mild steel hose for domestic emergency response [craft_industrial_emergency_rescue_civic_0002_lined_hose]", "crafts", ["industrial"], ["item:industrial_emergency_rescue_civic_0002_lined_hose"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined mild steel hose for domestic emergency response [craft_industrial_emergency_rescue_civic_0002_lined_hose]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a lined mild steel hose for domestic emergency response from prepared mild steel stock.",
		Action = "making lined mild steel hose for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress lined mild steel hose for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2470.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2470 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_emergency_rescue_civic_0002_lined_hose", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined mild steel hose for domestic emergency response (@industrial_emergency_rescue_civic_0002_lined_hose)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeLinedOakLampForDomesticEmergencyResponseCraftIndustrialEmergencyRescueCivic0003LinedLamp() => new()
	{
		Metadata = new("Emergency, rescue and civic/make lined oak lamp for domestic emergency response [craft_industrial_emergency_rescue_civic_0003_lined_lamp]", "crafts", ["industrial"], ["item:industrial_emergency_rescue_civic_0003_lined_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined oak lamp for domestic emergency response [craft_industrial_emergency_rescue_civic_0003_lined_lamp]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a lined oak lamp for domestic emergency response from prepared oak stock.",
		Action = "making lined oak lamp for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress lined oak lamp for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2510.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2510 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_emergency_rescue_civic_0003_lined_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined oak lamp for domestic emergency response (@industrial_emergency_rescue_civic_0003_lined_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeLinedCottonBarrierForDomesticEmergencyResponseCraftIndustrialEmergencyRescueCivic0004LinedBarrier() => new()
	{
		Metadata = new("Emergency, rescue and civic/make lined cotton barrier for domestic emergency response [craft_industrial_emergency_rescue_civic_0004_lined_barrier]", "crafts", ["industrial"], ["item:industrial_emergency_rescue_civic_0004_lined_barrier"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cotton barrier for domestic emergency response [craft_industrial_emergency_rescue_civic_0004_lined_barrier]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a lined cotton barrier for domestic emergency response from prepared cotton stock.",
		Action = "making lined cotton barrier for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress lined cotton barrier for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2551.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2551 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_emergency_rescue_civic_0004_lined_barrier", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cotton barrier for domestic emergency response (@industrial_emergency_rescue_civic_0004_lined_barrier)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeLinedNaturalRubberSignForDomesticEmergencyResponseCraftIndustrialEmergencyRescueCivic0005LinedSign() => new()
	{
		Metadata = new("Emergency, rescue and civic/make lined natural rubber sign for domestic emergency response [craft_industrial_emergency_rescue_civic_0005_lined_sign]", "crafts", ["industrial"], ["item:industrial_emergency_rescue_civic_0005_lined_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined natural rubber sign for domestic emergency response [craft_industrial_emergency_rescue_civic_0005_lined_sign]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a lined natural rubber sign for domestic emergency response from prepared natural rubber stock.",
		Action = "making lined natural rubber sign for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress lined natural rubber sign for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2592.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2592 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_emergency_rescue_civic_0005_lined_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined natural rubber sign for domestic emergency response (@industrial_emergency_rescue_civic_0005_lined_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition EmergencyRescueAndCivicMakeVentilatedMildSteelScrapForDomesticEmergencyResponseCraftIndustrialEmergencyRescueCivic0006VentilatedScrap() => new()
	{
		Metadata = new("Emergency, rescue and civic/make ventilated mild steel scrap for domestic emergency response [craft_industrial_emergency_rescue_civic_0006_ventilated_scrap]", "crafts", ["industrial"], ["item:industrial_emergency_rescue_civic_0006_ventilated_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated mild steel scrap for domestic emergency response [craft_industrial_emergency_rescue_civic_0006_ventilated_scrap]",
		Category = "Emergency, rescue and civic",
		Blurb = "Make a ventilated mild steel scrap for domestic emergency response from prepared mild steel stock.",
		Action = "making ventilated mild steel scrap for domestic emergency response",
		ActiveCraftItemSdesc = "an in-progress ventilated mild steel scrap for domestic emergency response craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2632.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2632 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_emergency_rescue_civic_0006_ventilated_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated mild steel scrap for domestic emergency response (@industrial_emergency_rescue_civic_0006_ventilated_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePlainPaperScrapForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0001PlainScrap() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make plain paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper scrap for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0001_plain_scrap]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a plain paper scrap for domestic materials processing from prepared paper stock.",
		Action = "making plain paper scrap for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress plain paper scrap for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2540.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2540 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper scrap for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePlainCottonIngotForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0002PlainIngot() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make plain cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0002_plain_ingot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0002_plain_ingot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton ingot for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0002_plain_ingot]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a plain cotton ingot for domestic materials processing from prepared cotton stock.",
		Action = "making plain cotton ingot for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress plain cotton ingot for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2581.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2581 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0002_plain_ingot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton ingot for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0002_plain_ingot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePlainMildSteelSheetForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0003PlainSheet() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make plain mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0003_plain_sheet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0003_plain_sheet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel sheet for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0003_plain_sheet]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a plain mild steel sheet for domestic materials processing from prepared mild steel stock.",
		Action = "making plain mild steel sheet for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress plain mild steel sheet for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2621.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2621 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0003_plain_sheet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel sheet for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0003_plain_sheet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePlainCopperWireForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0004PlainWire() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make plain copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0004_plain_wire]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0004_plain_wire"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper wire for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0004_plain_wire]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a plain copper wire for domestic materials processing from prepared copper stock.",
		Action = "making plain copper wire for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress plain copper wire for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2662.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2662 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0004_plain_wire", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper wire for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0004_plain_wire)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePlainAluminiumRodForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0005PlainRod() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make plain aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0005_plain_rod]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0005_plain_rod"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain aluminium rod for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0005_plain_rod]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a plain aluminium rod for domestic materials processing from prepared aluminium stock.",
		Action = "making plain aluminium rod for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress plain aluminium rod for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2703.0, Material: "aluminium", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2703 grams of aluminium", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0005_plain_rod", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain aluminium rod for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0005_plain_rod)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RawMaterialsChemicalsAndWasteMakePlainPaperPowderForDomesticMaterialsProcessingCraftIndustrialisedRawMaterialsChemicalsWaste0006PlainPowder() => new()
	{
		Metadata = new("Raw materials, chemicals and waste/make plain paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0006_plain_powder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_raw_materials_chemicals_waste_0006_plain_powder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper powder for domestic materials processing [craft_industrialised_raw_materials_chemicals_waste_0006_plain_powder]",
		Category = "Raw materials, chemicals and waste",
		Blurb = "Make a plain paper powder for domestic materials processing from prepared paper stock.",
		Action = "making plain paper powder for domestic materials processing",
		ActiveCraftItemSdesc = "an in-progress plain paper powder for domestic materials processing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2743.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2743 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_raw_materials_chemicals_waste_0006_plain_powder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper powder for domestic materials processing (@industrialised_raw_materials_chemicals_waste_0006_plain_powder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
