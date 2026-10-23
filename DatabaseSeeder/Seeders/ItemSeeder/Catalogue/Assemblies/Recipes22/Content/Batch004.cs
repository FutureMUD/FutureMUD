#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedMildSteelBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0013ReinforcedBaton() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced mild steel baton for domestic security [craft_industrialised_weapons_police_military_0013_reinforced_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0013_reinforced_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel baton for domestic security [craft_industrialised_weapons_police_military_0013_reinforced_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced mild steel baton for domestic security from prepared mild steel stock.",
		Action = "making reinforced mild steel baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2251.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2251 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0013_reinforced_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel baton for domestic security (@industrialised_weapons_police_military_0013_reinforced_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedLeatherScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0014ReinforcedScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced leather scabbard for domestic security [craft_industrialised_weapons_police_military_0014_reinforced_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0014_reinforced_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather scabbard for domestic security [craft_industrialised_weapons_police_military_0014_reinforced_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced leather scabbard for domestic security from prepared leather stock.",
		Action = "making reinforced leather scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced leather scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2291.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2291 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0014_reinforced_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather scabbard for domestic security (@industrialised_weapons_police_military_0014_reinforced_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedCottonCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0015ReinforcedCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0015_reinforced_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0015_reinforced_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0015_reinforced_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced cotton cartridge for domestic security from prepared cotton stock.",
		Action = "making reinforced cotton cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2332.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2332 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0015_reinforced_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton cartridge for domestic security (@industrialised_weapons_police_military_0015_reinforced_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedOakMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0016ReinforcedMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced oak magazine for domestic security [craft_industrialised_weapons_police_military_0016_reinforced_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0016_reinforced_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak magazine for domestic security [craft_industrialised_weapons_police_military_0016_reinforced_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced oak magazine for domestic security from prepared oak stock.",
		Action = "making reinforced oak magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced oak magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2373.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2373 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0016_reinforced_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak magazine for domestic security (@industrialised_weapons_police_military_0016_reinforced_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedMildSteelShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0017ReinforcedShield() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced mild steel shield for domestic security [craft_industrialised_weapons_police_military_0017_reinforced_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0017_reinforced_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel shield for domestic security [craft_industrialised_weapons_police_military_0017_reinforced_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced mild steel shield for domestic security from prepared mild steel stock.",
		Action = "making reinforced mild steel shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2413.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2413 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0017_reinforced_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel shield for domestic security (@industrialised_weapons_police_military_0017_reinforced_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedLeatherHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0018ReinforcedHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced leather helmet for domestic security [craft_industrialised_weapons_police_military_0018_reinforced_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0018_reinforced_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather helmet for domestic security [craft_industrialised_weapons_police_military_0018_reinforced_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced leather helmet for domestic security from prepared leather stock.",
		Action = "making reinforced leather helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced leather helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2454.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2454 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0018_reinforced_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather helmet for domestic security (@industrialised_weapons_police_military_0018_reinforced_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedCottonPouchForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0019ReinforcedPouch() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced cotton pouch for domestic security [craft_industrialised_weapons_police_military_0019_reinforced_pouch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0019_reinforced_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton pouch for domestic security [craft_industrialised_weapons_police_military_0019_reinforced_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced cotton pouch for domestic security from prepared cotton stock.",
		Action = "making reinforced cotton pouch for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton pouch for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2495.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2495 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0019_reinforced_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton pouch for domestic security (@industrialised_weapons_police_military_0019_reinforced_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedOakSignalForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0020ReinforcedSignal() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced oak signal for domestic security [craft_industrialised_weapons_police_military_0020_reinforced_signal]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0020_reinforced_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak signal for domestic security [craft_industrialised_weapons_police_military_0020_reinforced_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced oak signal for domestic security from prepared oak stock.",
		Action = "making reinforced oak signal for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced oak signal for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2536.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2536 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0020_reinforced_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak signal for domestic security (@industrialised_weapons_police_military_0020_reinforced_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactMildSteelScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0021CompactScrap() => new()
	{
		Metadata = new("Weapons, police and military/make compact mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0021_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0021_compact_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact mild steel scrap for domestic security from prepared mild steel stock.",
		Action = "making compact mild steel scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact mild steel scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2576.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2576 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel scrap for domestic security (@industrialised_weapons_police_military_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactLeatherHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0022CompactHolster() => new()
	{
		Metadata = new("Weapons, police and military/make compact leather holster for domestic security [craft_industrialised_weapons_police_military_0022_compact_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0022_compact_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather holster for domestic security [craft_industrialised_weapons_police_military_0022_compact_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact leather holster for domestic security from prepared leather stock.",
		Action = "making compact leather holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact leather holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2617.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2617 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0022_compact_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather holster for domestic security (@industrialised_weapons_police_military_0022_compact_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactCottonBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0023CompactBaton() => new()
	{
		Metadata = new("Weapons, police and military/make compact cotton baton for domestic security [craft_industrialised_weapons_police_military_0023_compact_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0023_compact_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton baton for domestic security [craft_industrialised_weapons_police_military_0023_compact_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact cotton baton for domestic security from prepared cotton stock.",
		Action = "making compact cotton baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact cotton baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2658.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2658 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0023_compact_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton baton for domestic security (@industrialised_weapons_police_military_0023_compact_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactOakScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0024CompactScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make compact oak scabbard for domestic security [craft_industrialised_weapons_police_military_0024_compact_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0024_compact_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak scabbard for domestic security [craft_industrialised_weapons_police_military_0024_compact_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact oak scabbard for domestic security from prepared oak stock.",
		Action = "making compact oak scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact oak scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2698.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2698 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0024_compact_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak scabbard for domestic security (@industrialised_weapons_police_military_0024_compact_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactMildSteelCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0025CompactCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make compact mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0025_compact_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0025_compact_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0025_compact_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact mild steel cartridge for domestic security from prepared mild steel stock.",
		Action = "making compact mild steel cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact mild steel cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2739.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2739 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0025_compact_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel cartridge for domestic security (@industrialised_weapons_police_military_0025_compact_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactLeatherMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0026CompactMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make compact leather magazine for domestic security [craft_industrialised_weapons_police_military_0026_compact_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0026_compact_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather magazine for domestic security [craft_industrialised_weapons_police_military_0026_compact_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact leather magazine for domestic security from prepared leather stock.",
		Action = "making compact leather magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact leather magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2780.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2780 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0026_compact_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather magazine for domestic security (@industrialised_weapons_police_military_0026_compact_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactCottonShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0027CompactShield() => new()
	{
		Metadata = new("Weapons, police and military/make compact cotton shield for domestic security [craft_industrialised_weapons_police_military_0027_compact_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0027_compact_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton shield for domestic security [craft_industrialised_weapons_police_military_0027_compact_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact cotton shield for domestic security from prepared cotton stock.",
		Action = "making compact cotton shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact cotton shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2820.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2820 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0027_compact_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton shield for domestic security (@industrialised_weapons_police_military_0027_compact_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactOakHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0028CompactHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make compact oak helmet for domestic security [craft_industrialised_weapons_police_military_0028_compact_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0028_compact_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak helmet for domestic security [craft_industrialised_weapons_police_military_0028_compact_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact oak helmet for domestic security from prepared oak stock.",
		Action = "making compact oak helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact oak helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2861.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2861 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0028_compact_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak helmet for domestic security (@industrialised_weapons_police_military_0028_compact_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactMildSteelPouchForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0029CompactPouch() => new()
	{
		Metadata = new("Weapons, police and military/make compact mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0029_compact_pouch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0029_compact_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0029_compact_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact mild steel pouch for domestic security from prepared mild steel stock.",
		Action = "making compact mild steel pouch for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact mild steel pouch for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2902.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2902 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0029_compact_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel pouch for domestic security (@industrialised_weapons_police_military_0029_compact_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeCompactLeatherSignalForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0030CompactSignal() => new()
	{
		Metadata = new("Weapons, police and military/make compact leather signal for domestic security [craft_industrialised_weapons_police_military_0030_compact_signal]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0030_compact_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather signal for domestic security [craft_industrialised_weapons_police_military_0030_compact_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make a compact leather signal for domestic security from prepared leather stock.",
		Action = "making compact leather signal for domestic security",
		ActiveCraftItemSdesc = "an in-progress compact leather signal for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2943.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2943 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0030_compact_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather signal for domestic security (@industrialised_weapons_police_military_0030_compact_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyCottonScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0031HeavyDutyScrap() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty cotton scrap for domestic security [craft_industrialised_weapons_police_military_0031_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0031_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton scrap for domestic security [craft_industrialised_weapons_police_military_0031_heavy_duty_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty cotton scrap for domestic security from prepared cotton stock.",
		Action = "making heavy-duty cotton scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2983.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2983 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0031_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton scrap for domestic security (@industrialised_weapons_police_military_0031_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyOakHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0032HeavyDutyHolster() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty oak holster for domestic security [craft_industrialised_weapons_police_military_0032_heavy_duty_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0032_heavy_duty_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak holster for domestic security [craft_industrialised_weapons_police_military_0032_heavy_duty_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty oak holster for domestic security from prepared oak stock.",
		Action = "making heavy-duty oak holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3024.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3024 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0032_heavy_duty_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak holster for domestic security (@industrialised_weapons_police_military_0032_heavy_duty_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyMildSteelBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0033HeavyDutyBaton() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty mild steel baton for domestic security [craft_industrialised_weapons_police_military_0033_heavy_duty_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0033_heavy_duty_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel baton for domestic security [craft_industrialised_weapons_police_military_0033_heavy_duty_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty mild steel baton for domestic security from prepared mild steel stock.",
		Action = "making heavy-duty mild steel baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3065.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3065 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0033_heavy_duty_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel baton for domestic security (@industrialised_weapons_police_military_0033_heavy_duty_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyLeatherScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0034HeavyDutyScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty leather scabbard for domestic security [craft_industrialised_weapons_police_military_0034_heavy_duty_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0034_heavy_duty_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather scabbard for domestic security [craft_industrialised_weapons_police_military_0034_heavy_duty_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty leather scabbard for domestic security from prepared leather stock.",
		Action = "making heavy-duty leather scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3105.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3105 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0034_heavy_duty_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather scabbard for domestic security (@industrialised_weapons_police_military_0034_heavy_duty_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyCottonCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0035HeavyDutyCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0035_heavy_duty_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0035_heavy_duty_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0035_heavy_duty_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty cotton cartridge for domestic security from prepared cotton stock.",
		Action = "making heavy-duty cotton cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3146.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3146 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0035_heavy_duty_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton cartridge for domestic security (@industrialised_weapons_police_military_0035_heavy_duty_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyOakMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0036HeavyDutyMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty oak magazine for domestic security [craft_industrialised_weapons_police_military_0036_heavy_duty_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0036_heavy_duty_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak magazine for domestic security [craft_industrialised_weapons_police_military_0036_heavy_duty_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty oak magazine for domestic security from prepared oak stock.",
		Action = "making heavy-duty oak magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3187.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3187 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0036_heavy_duty_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak magazine for domestic security (@industrialised_weapons_police_military_0036_heavy_duty_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyMildSteelShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0037HeavyDutyShield() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty mild steel shield for domestic security [craft_industrialised_weapons_police_military_0037_heavy_duty_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0037_heavy_duty_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel shield for domestic security [craft_industrialised_weapons_police_military_0037_heavy_duty_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty mild steel shield for domestic security from prepared mild steel stock.",
		Action = "making heavy-duty mild steel shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3227.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3227 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0037_heavy_duty_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel shield for domestic security (@industrialised_weapons_police_military_0037_heavy_duty_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyLeatherHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0038HeavyDutyHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty leather helmet for domestic security [craft_industrialised_weapons_police_military_0038_heavy_duty_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0038_heavy_duty_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather helmet for domestic security [craft_industrialised_weapons_police_military_0038_heavy_duty_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty leather helmet for domestic security from prepared leather stock.",
		Action = "making heavy-duty leather helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3268.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3268 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0038_heavy_duty_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather helmet for domestic security (@industrialised_weapons_police_military_0038_heavy_duty_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyCottonPouchForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0039HeavyDutyPouch() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty cotton pouch for domestic security [craft_industrialised_weapons_police_military_0039_heavy_duty_pouch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0039_heavy_duty_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton pouch for domestic security [craft_industrialised_weapons_police_military_0039_heavy_duty_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty cotton pouch for domestic security from prepared cotton stock.",
		Action = "making heavy-duty cotton pouch for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton pouch for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3309.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3309 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0039_heavy_duty_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton pouch for domestic security (@industrialised_weapons_police_military_0039_heavy_duty_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeHeavyDutyOakSignalForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0040HeavyDutySignal() => new()
	{
		Metadata = new("Weapons, police and military/make heavy-duty oak signal for domestic security [craft_industrialised_weapons_police_military_0040_heavy_duty_signal]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0040_heavy_duty_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak signal for domestic security [craft_industrialised_weapons_police_military_0040_heavy_duty_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make a heavy-duty oak signal for domestic security from prepared oak stock.",
		Action = "making heavy-duty oak signal for domestic security",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak signal for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3350.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3350 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0040_heavy_duty_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak signal for domestic security (@industrialised_weapons_police_military_0040_heavy_duty_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingMildSteelScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0041FoldingScrap() => new()
	{
		Metadata = new("Weapons, police and military/make folding mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0041_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0041_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0041_folding_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding mild steel scrap for domestic security from prepared mild steel stock.",
		Action = "making folding mild steel scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding mild steel scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3390.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3390 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0041_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel scrap for domestic security (@industrialised_weapons_police_military_0041_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingLeatherHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0042FoldingHolster() => new()
	{
		Metadata = new("Weapons, police and military/make folding leather holster for domestic security [craft_industrialised_weapons_police_military_0042_folding_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0042_folding_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather holster for domestic security [craft_industrialised_weapons_police_military_0042_folding_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding leather holster for domestic security from prepared leather stock.",
		Action = "making folding leather holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding leather holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3431.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3431 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0042_folding_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather holster for domestic security (@industrialised_weapons_police_military_0042_folding_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingCottonBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0043FoldingBaton() => new()
	{
		Metadata = new("Weapons, police and military/make folding cotton baton for domestic security [craft_industrialised_weapons_police_military_0043_folding_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0043_folding_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton baton for domestic security [craft_industrialised_weapons_police_military_0043_folding_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding cotton baton for domestic security from prepared cotton stock.",
		Action = "making folding cotton baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding cotton baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3472.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3472 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0043_folding_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton baton for domestic security (@industrialised_weapons_police_military_0043_folding_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingOakScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0044FoldingScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make folding oak scabbard for domestic security [craft_industrialised_weapons_police_military_0044_folding_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0044_folding_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak scabbard for domestic security [craft_industrialised_weapons_police_military_0044_folding_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding oak scabbard for domestic security from prepared oak stock.",
		Action = "making folding oak scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding oak scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3512.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3512 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0044_folding_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak scabbard for domestic security (@industrialised_weapons_police_military_0044_folding_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingMildSteelCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0045FoldingCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make folding mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0045_folding_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0045_folding_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0045_folding_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding mild steel cartridge for domestic security from prepared mild steel stock.",
		Action = "making folding mild steel cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding mild steel cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3553.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3553 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0045_folding_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel cartridge for domestic security (@industrialised_weapons_police_military_0045_folding_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingLeatherMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0046FoldingMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make folding leather magazine for domestic security [craft_industrialised_weapons_police_military_0046_folding_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0046_folding_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather magazine for domestic security [craft_industrialised_weapons_police_military_0046_folding_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding leather magazine for domestic security from prepared leather stock.",
		Action = "making folding leather magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding leather magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3594.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3594 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0046_folding_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather magazine for domestic security (@industrialised_weapons_police_military_0046_folding_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingCottonShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0047FoldingShield() => new()
	{
		Metadata = new("Weapons, police and military/make folding cotton shield for domestic security [craft_industrialised_weapons_police_military_0047_folding_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0047_folding_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton shield for domestic security [craft_industrialised_weapons_police_military_0047_folding_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding cotton shield for domestic security from prepared cotton stock.",
		Action = "making folding cotton shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding cotton shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3634.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3634 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0047_folding_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton shield for domestic security (@industrialised_weapons_police_military_0047_folding_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingOakHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0048FoldingHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make folding oak helmet for domestic security [craft_industrialised_weapons_police_military_0048_folding_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0048_folding_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak helmet for domestic security [craft_industrialised_weapons_police_military_0048_folding_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding oak helmet for domestic security from prepared oak stock.",
		Action = "making folding oak helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding oak helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3675.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3675 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0048_folding_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak helmet for domestic security (@industrialised_weapons_police_military_0048_folding_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingMildSteelPouchForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0049FoldingPouch() => new()
	{
		Metadata = new("Weapons, police and military/make folding mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0049_folding_pouch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0049_folding_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0049_folding_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding mild steel pouch for domestic security from prepared mild steel stock.",
		Action = "making folding mild steel pouch for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding mild steel pouch for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3716.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3716 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0049_folding_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel pouch for domestic security (@industrialised_weapons_police_military_0049_folding_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeFoldingLeatherSignalForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0050FoldingSignal() => new()
	{
		Metadata = new("Weapons, police and military/make folding leather signal for domestic security [craft_industrialised_weapons_police_military_0050_folding_signal]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0050_folding_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather signal for domestic security [craft_industrialised_weapons_police_military_0050_folding_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make a folding leather signal for domestic security from prepared leather stock.",
		Action = "making folding leather signal for domestic security",
		ActiveCraftItemSdesc = "an in-progress folding leather signal for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3757.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3757 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0050_folding_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather signal for domestic security (@industrialised_weapons_police_military_0050_folding_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableCottonScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0051PortableScrap() => new()
	{
		Metadata = new("Weapons, police and military/make portable cotton scrap for domestic security [craft_industrialised_weapons_police_military_0051_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0051_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton scrap for domestic security [craft_industrialised_weapons_police_military_0051_portable_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable cotton scrap for domestic security from prepared cotton stock.",
		Action = "making portable cotton scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable cotton scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3797.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3797 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0051_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton scrap for domestic security (@industrialised_weapons_police_military_0051_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableOakHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0052PortableHolster() => new()
	{
		Metadata = new("Weapons, police and military/make portable oak holster for domestic security [craft_industrialised_weapons_police_military_0052_portable_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0052_portable_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak holster for domestic security [craft_industrialised_weapons_police_military_0052_portable_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable oak holster for domestic security from prepared oak stock.",
		Action = "making portable oak holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable oak holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3838.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3838 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0052_portable_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak holster for domestic security (@industrialised_weapons_police_military_0052_portable_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableMildSteelBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0053PortableBaton() => new()
	{
		Metadata = new("Weapons, police and military/make portable mild steel baton for domestic security [craft_industrialised_weapons_police_military_0053_portable_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0053_portable_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel baton for domestic security [craft_industrialised_weapons_police_military_0053_portable_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable mild steel baton for domestic security from prepared mild steel stock.",
		Action = "making portable mild steel baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable mild steel baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3879.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3879 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0053_portable_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel baton for domestic security (@industrialised_weapons_police_military_0053_portable_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableLeatherScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0054PortableScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make portable leather scabbard for domestic security [craft_industrialised_weapons_police_military_0054_portable_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0054_portable_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather scabbard for domestic security [craft_industrialised_weapons_police_military_0054_portable_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable leather scabbard for domestic security from prepared leather stock.",
		Action = "making portable leather scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable leather scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3919.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3919 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0054_portable_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather scabbard for domestic security (@industrialised_weapons_police_military_0054_portable_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableCottonCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0055PortableCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make portable cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0055_portable_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0055_portable_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0055_portable_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable cotton cartridge for domestic security from prepared cotton stock.",
		Action = "making portable cotton cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable cotton cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3960.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3960 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0055_portable_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton cartridge for domestic security (@industrialised_weapons_police_military_0055_portable_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableOakMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0056PortableMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make portable oak magazine for domestic security [craft_industrialised_weapons_police_military_0056_portable_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0056_portable_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak magazine for domestic security [craft_industrialised_weapons_police_military_0056_portable_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable oak magazine for domestic security from prepared oak stock.",
		Action = "making portable oak magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable oak magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4001.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4001 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0056_portable_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak magazine for domestic security (@industrialised_weapons_police_military_0056_portable_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableMildSteelShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0057PortableShield() => new()
	{
		Metadata = new("Weapons, police and military/make portable mild steel shield for domestic security [craft_industrialised_weapons_police_military_0057_portable_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0057_portable_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel shield for domestic security [craft_industrialised_weapons_police_military_0057_portable_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable mild steel shield for domestic security from prepared mild steel stock.",
		Action = "making portable mild steel shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable mild steel shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4041.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4041 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0057_portable_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel shield for domestic security (@industrialised_weapons_police_military_0057_portable_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableLeatherHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0058PortableHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make portable leather helmet for domestic security [craft_industrialised_weapons_police_military_0058_portable_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0058_portable_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather helmet for domestic security [craft_industrialised_weapons_police_military_0058_portable_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable leather helmet for domestic security from prepared leather stock.",
		Action = "making portable leather helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable leather helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4082.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4082 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0058_portable_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather helmet for domestic security (@industrialised_weapons_police_military_0058_portable_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableCottonPouchForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0059PortablePouch() => new()
	{
		Metadata = new("Weapons, police and military/make portable cotton pouch for domestic security [craft_industrialised_weapons_police_military_0059_portable_pouch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0059_portable_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton pouch for domestic security [craft_industrialised_weapons_police_military_0059_portable_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable cotton pouch for domestic security from prepared cotton stock.",
		Action = "making portable cotton pouch for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable cotton pouch for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4123.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4123 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0059_portable_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton pouch for domestic security (@industrialised_weapons_police_military_0059_portable_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePortableOakSignalForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0060PortableSignal() => new()
	{
		Metadata = new("Weapons, police and military/make portable oak signal for domestic security [craft_industrialised_weapons_police_military_0060_portable_signal]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0060_portable_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak signal for domestic security [craft_industrialised_weapons_police_military_0060_portable_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make a portable oak signal for domestic security from prepared oak stock.",
		Action = "making portable oak signal for domestic security",
		ActiveCraftItemSdesc = "an in-progress portable oak signal for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4164.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4164 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0060_portable_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak signal for domestic security (@industrialised_weapons_police_military_0060_portable_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopMildSteelScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0061WorkshopScrap() => new()
	{
		Metadata = new("Weapons, police and military/make workshop mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0061_workshop_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0061_workshop_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0061_workshop_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop mild steel scrap for domestic security from prepared mild steel stock.",
		Action = "making workshop mild steel scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4204.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4204 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0061_workshop_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel scrap for domestic security (@industrialised_weapons_police_military_0061_workshop_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopLeatherHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0062WorkshopHolster() => new()
	{
		Metadata = new("Weapons, police and military/make workshop leather holster for domestic security [craft_industrialised_weapons_police_military_0062_workshop_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0062_workshop_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop leather holster for domestic security [craft_industrialised_weapons_police_military_0062_workshop_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop leather holster for domestic security from prepared leather stock.",
		Action = "making workshop leather holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop leather holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4245.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4245 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0062_workshop_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop leather holster for domestic security (@industrialised_weapons_police_military_0062_workshop_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopCottonBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0063WorkshopBaton() => new()
	{
		Metadata = new("Weapons, police and military/make workshop cotton baton for domestic security [craft_industrialised_weapons_police_military_0063_workshop_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0063_workshop_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton baton for domestic security [craft_industrialised_weapons_police_military_0063_workshop_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop cotton baton for domestic security from prepared cotton stock.",
		Action = "making workshop cotton baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop cotton baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4286.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4286 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0063_workshop_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton baton for domestic security (@industrialised_weapons_police_military_0063_workshop_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopOakScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0064WorkshopScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make workshop oak scabbard for domestic security [craft_industrialised_weapons_police_military_0064_workshop_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0064_workshop_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak scabbard for domestic security [craft_industrialised_weapons_police_military_0064_workshop_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop oak scabbard for domestic security from prepared oak stock.",
		Action = "making workshop oak scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop oak scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4326.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4326 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0064_workshop_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak scabbard for domestic security (@industrialised_weapons_police_military_0064_workshop_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopMildSteelCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0065WorkshopCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make workshop mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0065_workshop_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0065_workshop_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0065_workshop_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop mild steel cartridge for domestic security from prepared mild steel stock.",
		Action = "making workshop mild steel cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4367.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4367 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0065_workshop_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel cartridge for domestic security (@industrialised_weapons_police_military_0065_workshop_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopLeatherMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0066WorkshopMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make workshop leather magazine for domestic security [craft_industrialised_weapons_police_military_0066_workshop_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0066_workshop_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop leather magazine for domestic security [craft_industrialised_weapons_police_military_0066_workshop_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop leather magazine for domestic security from prepared leather stock.",
		Action = "making workshop leather magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop leather magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4408.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4408 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0066_workshop_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop leather magazine for domestic security (@industrialised_weapons_police_military_0066_workshop_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopCottonShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0067WorkshopShield() => new()
	{
		Metadata = new("Weapons, police and military/make workshop cotton shield for domestic security [craft_industrialised_weapons_police_military_0067_workshop_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0067_workshop_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton shield for domestic security [craft_industrialised_weapons_police_military_0067_workshop_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop cotton shield for domestic security from prepared cotton stock.",
		Action = "making workshop cotton shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop cotton shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4448.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4448 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0067_workshop_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton shield for domestic security (@industrialised_weapons_police_military_0067_workshop_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopOakHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0068WorkshopHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make workshop oak helmet for domestic security [craft_industrialised_weapons_police_military_0068_workshop_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0068_workshop_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak helmet for domestic security [craft_industrialised_weapons_police_military_0068_workshop_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop oak helmet for domestic security from prepared oak stock.",
		Action = "making workshop oak helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop oak helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4489.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4489 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0068_workshop_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak helmet for domestic security (@industrialised_weapons_police_military_0068_workshop_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopMildSteelPouchForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0069WorkshopPouch() => new()
	{
		Metadata = new("Weapons, police and military/make workshop mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0069_workshop_pouch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0069_workshop_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0069_workshop_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop mild steel pouch for domestic security from prepared mild steel stock.",
		Action = "making workshop mild steel pouch for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel pouch for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4530.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4530 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0069_workshop_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel pouch for domestic security (@industrialised_weapons_police_military_0069_workshop_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWorkshopLeatherSignalForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0070WorkshopSignal() => new()
	{
		Metadata = new("Weapons, police and military/make workshop leather signal for domestic security [craft_industrialised_weapons_police_military_0070_workshop_signal]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0070_workshop_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop leather signal for domestic security [craft_industrialised_weapons_police_military_0070_workshop_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make a workshop leather signal for domestic security from prepared leather stock.",
		Action = "making workshop leather signal for domestic security",
		ActiveCraftItemSdesc = "an in-progress workshop leather signal for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4570.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4570 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0070_workshop_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop leather signal for domestic security (@industrialised_weapons_police_military_0070_workshop_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalCottonScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0071InstitutionalScrap() => new()
	{
		Metadata = new("Weapons, police and military/make institutional cotton scrap for domestic security [craft_industrialised_weapons_police_military_0071_institutional_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0071_institutional_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton scrap for domestic security [craft_industrialised_weapons_police_military_0071_institutional_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional cotton scrap for domestic security from prepared cotton stock.",
		Action = "making institutional cotton scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional cotton scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4611.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4611 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0071_institutional_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton scrap for domestic security (@industrialised_weapons_police_military_0071_institutional_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalOakHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0072InstitutionalHolster() => new()
	{
		Metadata = new("Weapons, police and military/make institutional oak holster for domestic security [craft_industrialised_weapons_police_military_0072_institutional_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0072_institutional_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak holster for domestic security [craft_industrialised_weapons_police_military_0072_institutional_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional oak holster for domestic security from prepared oak stock.",
		Action = "making institutional oak holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional oak holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4652.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4652 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0072_institutional_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak holster for domestic security (@industrialised_weapons_police_military_0072_institutional_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
