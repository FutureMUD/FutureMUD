#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedMildSteelClubForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0015ReinforcedClub() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced mild steel club for domestic recreation [craft_industrialised_sports_recreation_music_0015_reinforced_club]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0015_reinforced_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel club for domestic recreation [craft_industrialised_sports_recreation_music_0015_reinforced_club]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced mild steel club for domestic recreation from prepared mild steel stock.",
		Action = "making reinforced mild steel club for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel club for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2888.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2888 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0015_reinforced_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel club for domestic recreation (@industrialised_sports_recreation_music_0015_reinforced_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedOakDrumForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0016ReinforcedDrum() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced oak drum for domestic recreation [craft_industrialised_sports_recreation_music_0016_reinforced_drum]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0016_reinforced_drum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak drum for domestic recreation [craft_industrialised_sports_recreation_music_0016_reinforced_drum]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced oak drum for domestic recreation from prepared oak stock.",
		Action = "making reinforced oak drum for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced oak drum for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2928.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2928 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0016_reinforced_drum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak drum for domestic recreation (@industrialised_sports_recreation_music_0016_reinforced_drum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedLeatherFluteForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0017ReinforcedFlute() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced leather flute for domestic recreation [craft_industrialised_sports_recreation_music_0017_reinforced_flute]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0017_reinforced_flute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather flute for domestic recreation [craft_industrialised_sports_recreation_music_0017_reinforced_flute]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced leather flute for domestic recreation from prepared leather stock.",
		Action = "making reinforced leather flute for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced leather flute for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2969.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2969 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0017_reinforced_flute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather flute for domestic recreation (@industrialised_sports_recreation_music_0017_reinforced_flute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedCottonBoardForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0018ReinforcedBoard() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced cotton board for domestic recreation [craft_industrialised_sports_recreation_music_0018_reinforced_board]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0018_reinforced_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton board for domestic recreation [craft_industrialised_sports_recreation_music_0018_reinforced_board]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced cotton board for domestic recreation from prepared cotton stock.",
		Action = "making reinforced cotton board for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton board for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3010.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3010 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0018_reinforced_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton board for domestic recreation (@industrialised_sports_recreation_music_0018_reinforced_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedMildSteelCounterForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0019ReinforcedCounter() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced mild steel counter for domestic recreation [craft_industrialised_sports_recreation_music_0019_reinforced_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0019_reinforced_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel counter for domestic recreation [craft_industrialised_sports_recreation_music_0019_reinforced_counter]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced mild steel counter for domestic recreation from prepared mild steel stock.",
		Action = "making reinforced mild steel counter for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel counter for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3050.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3050 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0019_reinforced_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel counter for domestic recreation (@industrialised_sports_recreation_music_0019_reinforced_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedOakSkateForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0020ReinforcedSkate() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced oak skate for domestic recreation [craft_industrialised_sports_recreation_music_0020_reinforced_skate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0020_reinforced_skate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak skate for domestic recreation [craft_industrialised_sports_recreation_music_0020_reinforced_skate]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced oak skate for domestic recreation from prepared oak stock.",
		Action = "making reinforced oak skate for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced oak skate for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3091.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3091 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0020_reinforced_skate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak skate for domestic recreation (@industrialised_sports_recreation_music_0020_reinforced_skate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactLeatherScrapForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0021CompactScrap() => new()
	{
		Metadata = new("Sports, recreation and music/make compact leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0021_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0021_compact_scrap]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact leather scrap for domestic recreation from prepared leather stock.",
		Action = "making compact leather scrap for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact leather scrap for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3132.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3132 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather scrap for domestic recreation (@industrialised_sports_recreation_music_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeCompactCottonBallForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0022CompactBall() => new()
	{
		Metadata = new("Sports, recreation and music/make compact cotton ball for domestic recreation [craft_industrialised_sports_recreation_music_0022_compact_ball]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0022_compact_ball"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton ball for domestic recreation [craft_industrialised_sports_recreation_music_0022_compact_ball]",
		Category = "Sports, recreation and music",
		Blurb = "Make a compact cotton ball for domestic recreation from prepared cotton stock.",
		Action = "making compact cotton ball for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress compact cotton ball for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3172.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3172 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0022_compact_ball", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton ball for domestic recreation (@industrialised_sports_recreation_music_0022_compact_ball)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
