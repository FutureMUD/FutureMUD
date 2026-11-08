#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes23Stock
{
	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactMildSteelBatForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0023CompactBat() => new()
	{
		Metadata = new("Sports, recreation and music/make compact mild steel bat for domestic recreation [craft_industrialised_sports_recreation_music_0023_compact_bat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0023_compact_bat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel bat for domestic recreation [craft_industrialised_sports_recreation_music_0023_compact_bat]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact mild steel bat for domestic recreation from prepared mild steel stock.",
		Action = "making compact mild steel bat for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact mild steel bat for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3213.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3213 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0023_compact_bat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel bat for domestic recreation (@industrialised_sports_recreation_music_0023_compact_bat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactOakRacketForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0024CompactRacket() => new()
	{
		Metadata = new("Sports, recreation and music/make compact oak racket for domestic recreation [craft_industrialised_sports_recreation_music_0024_compact_racket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0024_compact_racket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak racket for domestic recreation [craft_industrialised_sports_recreation_music_0024_compact_racket]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact oak racket for domestic recreation from prepared oak stock.",
		Action = "making compact oak racket for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact oak racket for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3254.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3254 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0024_compact_racket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak racket for domestic recreation (@industrialised_sports_recreation_music_0024_compact_racket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactLeatherClubForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0025CompactClub() => new()
	{
		Metadata = new("Sports, recreation and music/make compact leather club for domestic recreation [craft_industrialised_sports_recreation_music_0025_compact_club]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0025_compact_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather club for domestic recreation [craft_industrialised_sports_recreation_music_0025_compact_club]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact leather club for domestic recreation from prepared leather stock.",
		Action = "making compact leather club for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact leather club for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3295.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3295 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0025_compact_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather club for domestic recreation (@industrialised_sports_recreation_music_0025_compact_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactCottonDrumForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0026CompactDrum() => new()
	{
		Metadata = new("Sports, recreation and music/make compact cotton drum for domestic recreation [craft_industrialised_sports_recreation_music_0026_compact_drum]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0026_compact_drum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton drum for domestic recreation [craft_industrialised_sports_recreation_music_0026_compact_drum]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact cotton drum for domestic recreation from prepared cotton stock.",
		Action = "making compact cotton drum for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact cotton drum for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3335.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3335 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0026_compact_drum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton drum for domestic recreation (@industrialised_sports_recreation_music_0026_compact_drum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactMildSteelFluteForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0027CompactFlute() => new()
	{
		Metadata = new("Sports, recreation and music/make compact mild steel flute for domestic recreation [craft_industrialised_sports_recreation_music_0027_compact_flute]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0027_compact_flute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel flute for domestic recreation [craft_industrialised_sports_recreation_music_0027_compact_flute]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact mild steel flute for domestic recreation from prepared mild steel stock.",
		Action = "making compact mild steel flute for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact mild steel flute for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3376.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3376 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0027_compact_flute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel flute for domestic recreation (@industrialised_sports_recreation_music_0027_compact_flute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactOakBoardForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0028CompactBoard() => new()
	{
		Metadata = new("Sports, recreation and music/make compact oak board for domestic recreation [craft_industrialised_sports_recreation_music_0028_compact_board]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0028_compact_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak board for domestic recreation [craft_industrialised_sports_recreation_music_0028_compact_board]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact oak board for domestic recreation from prepared oak stock.",
		Action = "making compact oak board for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact oak board for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3417.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3417 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0028_compact_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak board for domestic recreation (@industrialised_sports_recreation_music_0028_compact_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactLeatherCounterForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0029CompactCounter() => new()
	{
		Metadata = new("Sports, recreation and music/make compact leather counter for domestic recreation [craft_industrialised_sports_recreation_music_0029_compact_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0029_compact_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather counter for domestic recreation [craft_industrialised_sports_recreation_music_0029_compact_counter]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact leather counter for domestic recreation from prepared leather stock.",
		Action = "making compact leather counter for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact leather counter for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3457.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3457 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0029_compact_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather counter for domestic recreation (@industrialised_sports_recreation_music_0029_compact_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactCottonSkateForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0030CompactSkate() => new()
	{
		Metadata = new("Sports, recreation and music/make compact cotton skate for domestic recreation [craft_industrialised_sports_recreation_music_0030_compact_skate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0030_compact_skate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton skate for domestic recreation [craft_industrialised_sports_recreation_music_0030_compact_skate]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact cotton skate for domestic recreation from prepared cotton stock.",
		Action = "making compact cotton skate for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact cotton skate for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3498.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3498 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0030_compact_skate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton skate for domestic recreation (@industrialised_sports_recreation_music_0030_compact_skate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyMildSteelScrapForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0031HeavyDutyScrap() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty mild steel scrap for domestic recreation [craft_industrialised_sports_recreation_music_0031_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for domestic recreation [craft_industrialised_sports_recreation_music_0031_heavy_duty_scrap]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty mild steel scrap for domestic recreation from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3539.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3539 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for domestic recreation (@industrialised_sports_recreation_music_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyOakBallForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0032HeavyDutyBall() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty oak ball for domestic recreation [craft_industrialised_sports_recreation_music_0032_heavy_duty_ball]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0032_heavy_duty_ball"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak ball for domestic recreation [craft_industrialised_sports_recreation_music_0032_heavy_duty_ball]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty oak ball for domestic recreation from prepared oak stock.",
		Action = "making heavy-duty oak ball for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak ball for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3579.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3579 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0032_heavy_duty_ball", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak ball for domestic recreation (@industrialised_sports_recreation_music_0032_heavy_duty_ball)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyLeatherBatForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0033HeavyDutyBat() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty leather bat for domestic recreation [craft_industrialised_sports_recreation_music_0033_heavy_duty_bat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0033_heavy_duty_bat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather bat for domestic recreation [craft_industrialised_sports_recreation_music_0033_heavy_duty_bat]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty leather bat for domestic recreation from prepared leather stock.",
		Action = "making heavy-duty leather bat for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather bat for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3620.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3620 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0033_heavy_duty_bat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather bat for domestic recreation (@industrialised_sports_recreation_music_0033_heavy_duty_bat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyCottonRacketForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0034HeavyDutyRacket() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty cotton racket for domestic recreation [craft_industrialised_sports_recreation_music_0034_heavy_duty_racket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0034_heavy_duty_racket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton racket for domestic recreation [craft_industrialised_sports_recreation_music_0034_heavy_duty_racket]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty cotton racket for domestic recreation from prepared cotton stock.",
		Action = "making heavy-duty cotton racket for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton racket for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3661.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3661 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0034_heavy_duty_racket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton racket for domestic recreation (@industrialised_sports_recreation_music_0034_heavy_duty_racket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyMildSteelClubForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0035HeavyDutyClub() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty mild steel club for domestic recreation [craft_industrialised_sports_recreation_music_0035_heavy_duty_club]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0035_heavy_duty_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel club for domestic recreation [craft_industrialised_sports_recreation_music_0035_heavy_duty_club]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty mild steel club for domestic recreation from prepared mild steel stock.",
		Action = "making heavy-duty mild steel club for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel club for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3702.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3702 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0035_heavy_duty_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel club for domestic recreation (@industrialised_sports_recreation_music_0035_heavy_duty_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyOakDrumForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0036HeavyDutyDrum() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty oak drum for domestic recreation [craft_industrialised_sports_recreation_music_0036_heavy_duty_drum]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0036_heavy_duty_drum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak drum for domestic recreation [craft_industrialised_sports_recreation_music_0036_heavy_duty_drum]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty oak drum for domestic recreation from prepared oak stock.",
		Action = "making heavy-duty oak drum for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak drum for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3742.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3742 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0036_heavy_duty_drum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak drum for domestic recreation (@industrialised_sports_recreation_music_0036_heavy_duty_drum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyLeatherFluteForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0037HeavyDutyFlute() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty leather flute for domestic recreation [craft_industrialised_sports_recreation_music_0037_heavy_duty_flute]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0037_heavy_duty_flute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather flute for domestic recreation [craft_industrialised_sports_recreation_music_0037_heavy_duty_flute]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty leather flute for domestic recreation from prepared leather stock.",
		Action = "making heavy-duty leather flute for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather flute for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3783.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3783 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0037_heavy_duty_flute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather flute for domestic recreation (@industrialised_sports_recreation_music_0037_heavy_duty_flute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyCottonBoardForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0038HeavyDutyBoard() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty cotton board for domestic recreation [craft_industrialised_sports_recreation_music_0038_heavy_duty_board]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0038_heavy_duty_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton board for domestic recreation [craft_industrialised_sports_recreation_music_0038_heavy_duty_board]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty cotton board for domestic recreation from prepared cotton stock.",
		Action = "making heavy-duty cotton board for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton board for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3824.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3824 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0038_heavy_duty_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton board for domestic recreation (@industrialised_sports_recreation_music_0038_heavy_duty_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyMildSteelCounterForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0039HeavyDutyCounter() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty mild steel counter for domestic recreation [craft_industrialised_sports_recreation_music_0039_heavy_duty_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0039_heavy_duty_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel counter for domestic recreation [craft_industrialised_sports_recreation_music_0039_heavy_duty_counter]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty mild steel counter for domestic recreation from prepared mild steel stock.",
		Action = "making heavy-duty mild steel counter for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel counter for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3864.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3864 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0039_heavy_duty_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel counter for domestic recreation (@industrialised_sports_recreation_music_0039_heavy_duty_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeHeavyDutyOakSkateForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0040HeavyDutySkate() => new()
	{
		Metadata = new("Sports, recreation and music/make heavy-duty oak skate for domestic recreation [craft_industrialised_sports_recreation_music_0040_heavy_duty_skate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0040_heavy_duty_skate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak skate for domestic recreation [craft_industrialised_sports_recreation_music_0040_heavy_duty_skate]",
		Category = "Sports, recreation and music",
		Blurb = "Make a heavy-duty oak skate for domestic recreation from prepared oak stock.",
		Action = "making heavy-duty oak skate for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak skate for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3905.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3905 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0040_heavy_duty_skate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak skate for domestic recreation (@industrialised_sports_recreation_music_0040_heavy_duty_skate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingLeatherScrapForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0041FoldingScrap() => new()
	{
		Metadata = new("Sports, recreation and music/make folding leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0041_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0041_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0041_folding_scrap]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding leather scrap for domestic recreation from prepared leather stock.",
		Action = "making folding leather scrap for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding leather scrap for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3946.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3946 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0041_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather scrap for domestic recreation (@industrialised_sports_recreation_music_0041_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingCottonBallForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0042FoldingBall() => new()
	{
		Metadata = new("Sports, recreation and music/make folding cotton ball for domestic recreation [craft_industrialised_sports_recreation_music_0042_folding_ball]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0042_folding_ball"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton ball for domestic recreation [craft_industrialised_sports_recreation_music_0042_folding_ball]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding cotton ball for domestic recreation from prepared cotton stock.",
		Action = "making folding cotton ball for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding cotton ball for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3986.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3986 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0042_folding_ball", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton ball for domestic recreation (@industrialised_sports_recreation_music_0042_folding_ball)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingMildSteelBatForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0043FoldingBat() => new()
	{
		Metadata = new("Sports, recreation and music/make folding mild steel bat for domestic recreation [craft_industrialised_sports_recreation_music_0043_folding_bat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0043_folding_bat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel bat for domestic recreation [craft_industrialised_sports_recreation_music_0043_folding_bat]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding mild steel bat for domestic recreation from prepared mild steel stock.",
		Action = "making folding mild steel bat for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding mild steel bat for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4027.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4027 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0043_folding_bat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel bat for domestic recreation (@industrialised_sports_recreation_music_0043_folding_bat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingOakRacketForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0044FoldingRacket() => new()
	{
		Metadata = new("Sports, recreation and music/make folding oak racket for domestic recreation [craft_industrialised_sports_recreation_music_0044_folding_racket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0044_folding_racket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak racket for domestic recreation [craft_industrialised_sports_recreation_music_0044_folding_racket]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding oak racket for domestic recreation from prepared oak stock.",
		Action = "making folding oak racket for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding oak racket for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4068.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4068 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0044_folding_racket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak racket for domestic recreation (@industrialised_sports_recreation_music_0044_folding_racket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingLeatherClubForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0045FoldingClub() => new()
	{
		Metadata = new("Sports, recreation and music/make folding leather club for domestic recreation [craft_industrialised_sports_recreation_music_0045_folding_club]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0045_folding_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather club for domestic recreation [craft_industrialised_sports_recreation_music_0045_folding_club]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding leather club for domestic recreation from prepared leather stock.",
		Action = "making folding leather club for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding leather club for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4108.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4108 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0045_folding_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather club for domestic recreation (@industrialised_sports_recreation_music_0045_folding_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingCottonDrumForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0046FoldingDrum() => new()
	{
		Metadata = new("Sports, recreation and music/make folding cotton drum for domestic recreation [craft_industrialised_sports_recreation_music_0046_folding_drum]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0046_folding_drum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton drum for domestic recreation [craft_industrialised_sports_recreation_music_0046_folding_drum]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding cotton drum for domestic recreation from prepared cotton stock.",
		Action = "making folding cotton drum for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding cotton drum for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4149.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4149 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0046_folding_drum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton drum for domestic recreation (@industrialised_sports_recreation_music_0046_folding_drum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingMildSteelFluteForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0047FoldingFlute() => new()
	{
		Metadata = new("Sports, recreation and music/make folding mild steel flute for domestic recreation [craft_industrialised_sports_recreation_music_0047_folding_flute]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0047_folding_flute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel flute for domestic recreation [craft_industrialised_sports_recreation_music_0047_folding_flute]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding mild steel flute for domestic recreation from prepared mild steel stock.",
		Action = "making folding mild steel flute for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding mild steel flute for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4190.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4190 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0047_folding_flute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel flute for domestic recreation (@industrialised_sports_recreation_music_0047_folding_flute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingOakBoardForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0048FoldingBoard() => new()
	{
		Metadata = new("Sports, recreation and music/make folding oak board for domestic recreation [craft_industrialised_sports_recreation_music_0048_folding_board]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0048_folding_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak board for domestic recreation [craft_industrialised_sports_recreation_music_0048_folding_board]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding oak board for domestic recreation from prepared oak stock.",
		Action = "making folding oak board for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding oak board for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4231.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4231 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0048_folding_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak board for domestic recreation (@industrialised_sports_recreation_music_0048_folding_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingLeatherCounterForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0049FoldingCounter() => new()
	{
		Metadata = new("Sports, recreation and music/make folding leather counter for domestic recreation [craft_industrialised_sports_recreation_music_0049_folding_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0049_folding_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather counter for domestic recreation [craft_industrialised_sports_recreation_music_0049_folding_counter]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding leather counter for domestic recreation from prepared leather stock.",
		Action = "making folding leather counter for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding leather counter for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4271.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4271 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0049_folding_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather counter for domestic recreation (@industrialised_sports_recreation_music_0049_folding_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeFoldingCottonSkateForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0050FoldingSkate() => new()
	{
		Metadata = new("Sports, recreation and music/make folding cotton skate for domestic recreation [craft_industrialised_sports_recreation_music_0050_folding_skate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0050_folding_skate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton skate for domestic recreation [craft_industrialised_sports_recreation_music_0050_folding_skate]",
		Category = "Sports, recreation and music",
		Blurb = "Make a folding cotton skate for domestic recreation from prepared cotton stock.",
		Action = "making folding cotton skate for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress folding cotton skate for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4312.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4312 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0050_folding_skate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton skate for domestic recreation (@industrialised_sports_recreation_music_0050_folding_skate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableMildSteelScrapForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0051PortableScrap() => new()
	{
		Metadata = new("Sports, recreation and music/make portable mild steel scrap for domestic recreation [craft_industrialised_sports_recreation_music_0051_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0051_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel scrap for domestic recreation [craft_industrialised_sports_recreation_music_0051_portable_scrap]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable mild steel scrap for domestic recreation from prepared mild steel stock.",
		Action = "making portable mild steel scrap for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable mild steel scrap for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4353.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4353 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0051_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel scrap for domestic recreation (@industrialised_sports_recreation_music_0051_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableOakBallForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0052PortableBall() => new()
	{
		Metadata = new("Sports, recreation and music/make portable oak ball for domestic recreation [craft_industrialised_sports_recreation_music_0052_portable_ball]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0052_portable_ball"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak ball for domestic recreation [craft_industrialised_sports_recreation_music_0052_portable_ball]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable oak ball for domestic recreation from prepared oak stock.",
		Action = "making portable oak ball for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable oak ball for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4393.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4393 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0052_portable_ball", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak ball for domestic recreation (@industrialised_sports_recreation_music_0052_portable_ball)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableLeatherBatForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0053PortableBat() => new()
	{
		Metadata = new("Sports, recreation and music/make portable leather bat for domestic recreation [craft_industrialised_sports_recreation_music_0053_portable_bat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0053_portable_bat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather bat for domestic recreation [craft_industrialised_sports_recreation_music_0053_portable_bat]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable leather bat for domestic recreation from prepared leather stock.",
		Action = "making portable leather bat for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable leather bat for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4434.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4434 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0053_portable_bat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather bat for domestic recreation (@industrialised_sports_recreation_music_0053_portable_bat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableCottonRacketForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0054PortableRacket() => new()
	{
		Metadata = new("Sports, recreation and music/make portable cotton racket for domestic recreation [craft_industrialised_sports_recreation_music_0054_portable_racket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0054_portable_racket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton racket for domestic recreation [craft_industrialised_sports_recreation_music_0054_portable_racket]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable cotton racket for domestic recreation from prepared cotton stock.",
		Action = "making portable cotton racket for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable cotton racket for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4475.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4475 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0054_portable_racket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton racket for domestic recreation (@industrialised_sports_recreation_music_0054_portable_racket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableMildSteelClubForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0055PortableClub() => new()
	{
		Metadata = new("Sports, recreation and music/make portable mild steel club for domestic recreation [craft_industrialised_sports_recreation_music_0055_portable_club]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0055_portable_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel club for domestic recreation [craft_industrialised_sports_recreation_music_0055_portable_club]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable mild steel club for domestic recreation from prepared mild steel stock.",
		Action = "making portable mild steel club for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable mild steel club for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4516.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4516 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0055_portable_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel club for domestic recreation (@industrialised_sports_recreation_music_0055_portable_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableOakDrumForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0056PortableDrum() => new()
	{
		Metadata = new("Sports, recreation and music/make portable oak drum for domestic recreation [craft_industrialised_sports_recreation_music_0056_portable_drum]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0056_portable_drum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak drum for domestic recreation [craft_industrialised_sports_recreation_music_0056_portable_drum]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable oak drum for domestic recreation from prepared oak stock.",
		Action = "making portable oak drum for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable oak drum for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4556.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4556 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0056_portable_drum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak drum for domestic recreation (@industrialised_sports_recreation_music_0056_portable_drum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableLeatherFluteForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0057PortableFlute() => new()
	{
		Metadata = new("Sports, recreation and music/make portable leather flute for domestic recreation [craft_industrialised_sports_recreation_music_0057_portable_flute]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0057_portable_flute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather flute for domestic recreation [craft_industrialised_sports_recreation_music_0057_portable_flute]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable leather flute for domestic recreation from prepared leather stock.",
		Action = "making portable leather flute for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable leather flute for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4597.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4597 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0057_portable_flute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather flute for domestic recreation (@industrialised_sports_recreation_music_0057_portable_flute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableCottonBoardForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0058PortableBoard() => new()
	{
		Metadata = new("Sports, recreation and music/make portable cotton board for domestic recreation [craft_industrialised_sports_recreation_music_0058_portable_board]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0058_portable_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton board for domestic recreation [craft_industrialised_sports_recreation_music_0058_portable_board]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable cotton board for domestic recreation from prepared cotton stock.",
		Action = "making portable cotton board for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable cotton board for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4638.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4638 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0058_portable_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton board for domestic recreation (@industrialised_sports_recreation_music_0058_portable_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableMildSteelCounterForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0059PortableCounter() => new()
	{
		Metadata = new("Sports, recreation and music/make portable mild steel counter for domestic recreation [craft_industrialised_sports_recreation_music_0059_portable_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0059_portable_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel counter for domestic recreation [craft_industrialised_sports_recreation_music_0059_portable_counter]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable mild steel counter for domestic recreation from prepared mild steel stock.",
		Action = "making portable mild steel counter for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable mild steel counter for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4678.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4678 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0059_portable_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel counter for domestic recreation (@industrialised_sports_recreation_music_0059_portable_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePortableOakSkateForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0060PortableSkate() => new()
	{
		Metadata = new("Sports, recreation and music/make portable oak skate for domestic recreation [craft_industrialised_sports_recreation_music_0060_portable_skate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0060_portable_skate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak skate for domestic recreation [craft_industrialised_sports_recreation_music_0060_portable_skate]",
		Category = "Sports, recreation and music",
		Blurb = "Make a portable oak skate for domestic recreation from prepared oak stock.",
		Action = "making portable oak skate for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress portable oak skate for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4719.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4719 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0060_portable_skate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak skate for domestic recreation (@industrialised_sports_recreation_music_0060_portable_skate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopLeatherScrapForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0061WorkshopScrap() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0061_workshop_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0061_workshop_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0061_workshop_scrap]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop leather scrap for domestic recreation from prepared leather stock.",
		Action = "making workshop leather scrap for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop leather scrap for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4760.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4760 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0061_workshop_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop leather scrap for domestic recreation (@industrialised_sports_recreation_music_0061_workshop_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopCottonBallForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0062WorkshopBall() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop cotton ball for domestic recreation [craft_industrialised_sports_recreation_music_0062_workshop_ball]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0062_workshop_ball"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton ball for domestic recreation [craft_industrialised_sports_recreation_music_0062_workshop_ball]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop cotton ball for domestic recreation from prepared cotton stock.",
		Action = "making workshop cotton ball for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop cotton ball for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4800.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4800 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0062_workshop_ball", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton ball for domestic recreation (@industrialised_sports_recreation_music_0062_workshop_ball)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopMildSteelBatForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0063WorkshopBat() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop mild steel bat for domestic recreation [craft_industrialised_sports_recreation_music_0063_workshop_bat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0063_workshop_bat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel bat for domestic recreation [craft_industrialised_sports_recreation_music_0063_workshop_bat]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop mild steel bat for domestic recreation from prepared mild steel stock.",
		Action = "making workshop mild steel bat for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel bat for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4841.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4841 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0063_workshop_bat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel bat for domestic recreation (@industrialised_sports_recreation_music_0063_workshop_bat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopOakRacketForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0064WorkshopRacket() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop oak racket for domestic recreation [craft_industrialised_sports_recreation_music_0064_workshop_racket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0064_workshop_racket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak racket for domestic recreation [craft_industrialised_sports_recreation_music_0064_workshop_racket]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop oak racket for domestic recreation from prepared oak stock.",
		Action = "making workshop oak racket for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop oak racket for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4882.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4882 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0064_workshop_racket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak racket for domestic recreation (@industrialised_sports_recreation_music_0064_workshop_racket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopLeatherClubForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0065WorkshopClub() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop leather club for domestic recreation [craft_industrialised_sports_recreation_music_0065_workshop_club]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0065_workshop_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop leather club for domestic recreation [craft_industrialised_sports_recreation_music_0065_workshop_club]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop leather club for domestic recreation from prepared leather stock.",
		Action = "making workshop leather club for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop leather club for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4922.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4922 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0065_workshop_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop leather club for domestic recreation (@industrialised_sports_recreation_music_0065_workshop_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopCottonDrumForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0066WorkshopDrum() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop cotton drum for domestic recreation [craft_industrialised_sports_recreation_music_0066_workshop_drum]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0066_workshop_drum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton drum for domestic recreation [craft_industrialised_sports_recreation_music_0066_workshop_drum]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop cotton drum for domestic recreation from prepared cotton stock.",
		Action = "making workshop cotton drum for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop cotton drum for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4963.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4963 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0066_workshop_drum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton drum for domestic recreation (@industrialised_sports_recreation_music_0066_workshop_drum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopMildSteelFluteForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0067WorkshopFlute() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop mild steel flute for domestic recreation [craft_industrialised_sports_recreation_music_0067_workshop_flute]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0067_workshop_flute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel flute for domestic recreation [craft_industrialised_sports_recreation_music_0067_workshop_flute]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop mild steel flute for domestic recreation from prepared mild steel stock.",
		Action = "making workshop mild steel flute for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel flute for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5004.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5004 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0067_workshop_flute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel flute for domestic recreation (@industrialised_sports_recreation_music_0067_workshop_flute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopOakBoardForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0068WorkshopBoard() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop oak board for domestic recreation [craft_industrialised_sports_recreation_music_0068_workshop_board]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0068_workshop_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak board for domestic recreation [craft_industrialised_sports_recreation_music_0068_workshop_board]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop oak board for domestic recreation from prepared oak stock.",
		Action = "making workshop oak board for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop oak board for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5045.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5045 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0068_workshop_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak board for domestic recreation (@industrialised_sports_recreation_music_0068_workshop_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopLeatherCounterForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0069WorkshopCounter() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop leather counter for domestic recreation [craft_industrialised_sports_recreation_music_0069_workshop_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0069_workshop_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop leather counter for domestic recreation [craft_industrialised_sports_recreation_music_0069_workshop_counter]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop leather counter for domestic recreation from prepared leather stock.",
		Action = "making workshop leather counter for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop leather counter for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5085.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5085 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0069_workshop_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop leather counter for domestic recreation (@industrialised_sports_recreation_music_0069_workshop_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWorkshopCottonSkateForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0070WorkshopSkate() => new()
	{
		Metadata = new("Sports, recreation and music/make workshop cotton skate for domestic recreation [craft_industrialised_sports_recreation_music_0070_workshop_skate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0070_workshop_skate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton skate for domestic recreation [craft_industrialised_sports_recreation_music_0070_workshop_skate]",
		Category = "Sports, recreation and music",
		Blurb = "Make a workshop cotton skate for domestic recreation from prepared cotton stock.",
		Action = "making workshop cotton skate for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress workshop cotton skate for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5126.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5126 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0070_workshop_skate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton skate for domestic recreation (@industrialised_sports_recreation_music_0070_workshop_skate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalMildSteelScrapForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0071InstitutionalScrap() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional mild steel scrap for domestic recreation [craft_industrialised_sports_recreation_music_0071_institutional_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0071_institutional_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel scrap for domestic recreation [craft_industrialised_sports_recreation_music_0071_institutional_scrap]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional mild steel scrap for domestic recreation from prepared mild steel stock.",
		Action = "making institutional mild steel scrap for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel scrap for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5167.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5167 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0071_institutional_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel scrap for domestic recreation (@industrialised_sports_recreation_music_0071_institutional_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalOakBallForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0072InstitutionalBall() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional oak ball for domestic recreation [craft_industrialised_sports_recreation_music_0072_institutional_ball]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0072_institutional_ball"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak ball for domestic recreation [craft_industrialised_sports_recreation_music_0072_institutional_ball]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional oak ball for domestic recreation from prepared oak stock.",
		Action = "making institutional oak ball for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional oak ball for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5207.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5207 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0072_institutional_ball", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak ball for domestic recreation (@industrialised_sports_recreation_music_0072_institutional_ball)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalLeatherBatForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0073InstitutionalBat() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional leather bat for domestic recreation [craft_industrialised_sports_recreation_music_0073_institutional_bat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0073_institutional_bat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional leather bat for domestic recreation [craft_industrialised_sports_recreation_music_0073_institutional_bat]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional leather bat for domestic recreation from prepared leather stock.",
		Action = "making institutional leather bat for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional leather bat for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5248.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5248 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0073_institutional_bat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional leather bat for domestic recreation (@industrialised_sports_recreation_music_0073_institutional_bat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalCottonRacketForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0074InstitutionalRacket() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional cotton racket for domestic recreation [craft_industrialised_sports_recreation_music_0074_institutional_racket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0074_institutional_racket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton racket for domestic recreation [craft_industrialised_sports_recreation_music_0074_institutional_racket]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional cotton racket for domestic recreation from prepared cotton stock.",
		Action = "making institutional cotton racket for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional cotton racket for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5289.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5289 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0074_institutional_racket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton racket for domestic recreation (@industrialised_sports_recreation_music_0074_institutional_racket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalMildSteelClubForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0075InstitutionalClub() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional mild steel club for domestic recreation [craft_industrialised_sports_recreation_music_0075_institutional_club]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0075_institutional_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel club for domestic recreation [craft_industrialised_sports_recreation_music_0075_institutional_club]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional mild steel club for domestic recreation from prepared mild steel stock.",
		Action = "making institutional mild steel club for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel club for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5330.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5330 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0075_institutional_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel club for domestic recreation (@industrialised_sports_recreation_music_0075_institutional_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalOakDrumForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0076InstitutionalDrum() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional oak drum for domestic recreation [craft_industrialised_sports_recreation_music_0076_institutional_drum]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0076_institutional_drum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak drum for domestic recreation [craft_industrialised_sports_recreation_music_0076_institutional_drum]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional oak drum for domestic recreation from prepared oak stock.",
		Action = "making institutional oak drum for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional oak drum for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5370.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5370 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0076_institutional_drum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak drum for domestic recreation (@industrialised_sports_recreation_music_0076_institutional_drum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalLeatherFluteForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0077InstitutionalFlute() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional leather flute for domestic recreation [craft_industrialised_sports_recreation_music_0077_institutional_flute]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0077_institutional_flute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional leather flute for domestic recreation [craft_industrialised_sports_recreation_music_0077_institutional_flute]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional leather flute for domestic recreation from prepared leather stock.",
		Action = "making institutional leather flute for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional leather flute for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5411.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5411 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0077_institutional_flute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional leather flute for domestic recreation (@industrialised_sports_recreation_music_0077_institutional_flute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalCottonBoardForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0078InstitutionalBoard() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional cotton board for domestic recreation [craft_industrialised_sports_recreation_music_0078_institutional_board]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0078_institutional_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton board for domestic recreation [craft_industrialised_sports_recreation_music_0078_institutional_board]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional cotton board for domestic recreation from prepared cotton stock.",
		Action = "making institutional cotton board for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional cotton board for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5452.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5452 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0078_institutional_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton board for domestic recreation (@industrialised_sports_recreation_music_0078_institutional_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalMildSteelCounterForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0079InstitutionalCounter() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional mild steel counter for domestic recreation [craft_industrialised_sports_recreation_music_0079_institutional_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0079_institutional_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel counter for domestic recreation [craft_industrialised_sports_recreation_music_0079_institutional_counter]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional mild steel counter for domestic recreation from prepared mild steel stock.",
		Action = "making institutional mild steel counter for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel counter for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5492.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5492 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0079_institutional_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel counter for domestic recreation (@industrialised_sports_recreation_music_0079_institutional_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeInstitutionalOakSkateForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0080InstitutionalSkate() => new()
	{
		Metadata = new("Sports, recreation and music/make institutional oak skate for domestic recreation [craft_industrialised_sports_recreation_music_0080_institutional_skate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0080_institutional_skate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak skate for domestic recreation [craft_industrialised_sports_recreation_music_0080_institutional_skate]",
		Category = "Sports, recreation and music",
		Blurb = "Make an institutional oak skate for domestic recreation from prepared oak stock.",
		Action = "making institutional oak skate for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress institutional oak skate for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 88.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("88 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0080_institutional_skate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak skate for domestic recreation (@industrialised_sports_recreation_music_0080_institutional_skate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeWeatherproofLeatherScrapForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0081WeatherproofScrap() => new()
	{
		Metadata = new("Sports, recreation and music/make weatherproof leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0081_weatherproof_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0081_weatherproof_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0081_weatherproof_scrap]",
		Category = "Sports, recreation and music",
		Blurb = "Make a weatherproof leather scrap for domestic recreation from prepared leather stock.",
		Action = "making weatherproof leather scrap for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress weatherproof leather scrap for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 129.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("129 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0081_weatherproof_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof leather scrap for domestic recreation (@industrialised_sports_recreation_music_0081_weatherproof_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactCottonDrumForWorkshopRecreationCraftIndustrialSportsRecreationMusic0001CompactDrum() => new()
	{
		Metadata = new("Sports, recreation and music/make compact cotton drum for workshop recreation [craft_industrial_sports_recreation_music_0001_compact_drum]", "crafts", ["industrial"], ["item:industrial_sports_recreation_music_0001_compact_drum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton drum for workshop recreation [craft_industrial_sports_recreation_music_0001_compact_drum]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact cotton drum for workshop recreation from prepared cotton stock.",
		Action = "making compact cotton drum for workshop recreation",
		ActiveCraftItemSdesc = "an in-progress compact cotton drum for workshop recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2318.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2318 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_sports_recreation_music_0001_compact_drum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton drum for workshop recreation (@industrial_sports_recreation_music_0001_compact_drum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
