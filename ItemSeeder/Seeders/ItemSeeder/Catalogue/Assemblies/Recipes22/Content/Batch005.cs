#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalMildSteelBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0073InstitutionalBaton() => new()
	{
		Metadata = new("Weapons, police and military/make institutional mild steel baton for domestic security [craft_industrialised_weapons_police_military_0073_institutional_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0073_institutional_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel baton for domestic security [craft_industrialised_weapons_police_military_0073_institutional_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional mild steel baton for domestic security from prepared mild steel stock.",
		Action = "making institutional mild steel baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4693.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4693 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0073_institutional_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel baton for domestic security (@industrialised_weapons_police_military_0073_institutional_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalLeatherScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0074InstitutionalScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make institutional leather scabbard for domestic security [craft_industrialised_weapons_police_military_0074_institutional_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0074_institutional_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional leather scabbard for domestic security [craft_industrialised_weapons_police_military_0074_institutional_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional leather scabbard for domestic security from prepared leather stock.",
		Action = "making institutional leather scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional leather scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4733.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4733 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0074_institutional_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional leather scabbard for domestic security (@industrialised_weapons_police_military_0074_institutional_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalCottonCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0075InstitutionalCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make institutional cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0075_institutional_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0075_institutional_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0075_institutional_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional cotton cartridge for domestic security from prepared cotton stock.",
		Action = "making institutional cotton cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional cotton cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4774.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4774 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0075_institutional_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton cartridge for domestic security (@industrialised_weapons_police_military_0075_institutional_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalOakMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0076InstitutionalMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make institutional oak magazine for domestic security [craft_industrialised_weapons_police_military_0076_institutional_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0076_institutional_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak magazine for domestic security [craft_industrialised_weapons_police_military_0076_institutional_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional oak magazine for domestic security from prepared oak stock.",
		Action = "making institutional oak magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional oak magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4815.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4815 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0076_institutional_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak magazine for domestic security (@industrialised_weapons_police_military_0076_institutional_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalMildSteelShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0077InstitutionalShield() => new()
	{
		Metadata = new("Weapons, police and military/make institutional mild steel shield for domestic security [craft_industrialised_weapons_police_military_0077_institutional_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0077_institutional_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel shield for domestic security [craft_industrialised_weapons_police_military_0077_institutional_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional mild steel shield for domestic security from prepared mild steel stock.",
		Action = "making institutional mild steel shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4855.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4855 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0077_institutional_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel shield for domestic security (@industrialised_weapons_police_military_0077_institutional_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalLeatherHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0078InstitutionalHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make institutional leather helmet for domestic security [craft_industrialised_weapons_police_military_0078_institutional_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0078_institutional_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional leather helmet for domestic security [craft_industrialised_weapons_police_military_0078_institutional_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional leather helmet for domestic security from prepared leather stock.",
		Action = "making institutional leather helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional leather helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4896.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4896 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0078_institutional_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional leather helmet for domestic security (@industrialised_weapons_police_military_0078_institutional_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalCottonPouchForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0079InstitutionalPouch() => new()
	{
		Metadata = new("Weapons, police and military/make institutional cotton pouch for domestic security [craft_industrialised_weapons_police_military_0079_institutional_pouch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0079_institutional_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton pouch for domestic security [craft_industrialised_weapons_police_military_0079_institutional_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional cotton pouch for domestic security from prepared cotton stock.",
		Action = "making institutional cotton pouch for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional cotton pouch for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4937.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4937 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0079_institutional_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton pouch for domestic security (@industrialised_weapons_police_military_0079_institutional_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalOakSignalForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0080InstitutionalSignal() => new()
	{
		Metadata = new("Weapons, police and military/make institutional oak signal for domestic security [craft_industrialised_weapons_police_military_0080_institutional_signal]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0080_institutional_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak signal for domestic security [craft_industrialised_weapons_police_military_0080_institutional_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional oak signal for domestic security from prepared oak stock.",
		Action = "making institutional oak signal for domestic security",
		ActiveCraftItemSdesc = "an in-progress institutional oak signal for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4978.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4978 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0080_institutional_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak signal for domestic security (@industrialised_weapons_police_military_0080_institutional_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofMildSteelScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0081WeatherproofScrap() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0081_weatherproof_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0081_weatherproof_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0081_weatherproof_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof mild steel scrap for domestic security from prepared mild steel stock.",
		Action = "making weatherproof mild steel scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5018.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5018 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0081_weatherproof_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel scrap for domestic security (@industrialised_weapons_police_military_0081_weatherproof_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofLeatherHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0082WeatherproofHolster() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof leather holster for domestic security [craft_industrialised_weapons_police_military_0082_weatherproof_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0082_weatherproof_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof leather holster for domestic security [craft_industrialised_weapons_police_military_0082_weatherproof_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof leather holster for domestic security from prepared leather stock.",
		Action = "making weatherproof leather holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof leather holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5059.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5059 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0082_weatherproof_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof leather holster for domestic security (@industrialised_weapons_police_military_0082_weatherproof_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofCottonBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0083WeatherproofBaton() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof cotton baton for domestic security [craft_industrialised_weapons_police_military_0083_weatherproof_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0083_weatherproof_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton baton for domestic security [craft_industrialised_weapons_police_military_0083_weatherproof_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof cotton baton for domestic security from prepared cotton stock.",
		Action = "making weatherproof cotton baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5100.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5100 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0083_weatherproof_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton baton for domestic security (@industrialised_weapons_police_military_0083_weatherproof_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofOakScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0084WeatherproofScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof oak scabbard for domestic security [craft_industrialised_weapons_police_military_0084_weatherproof_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0084_weatherproof_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak scabbard for domestic security [craft_industrialised_weapons_police_military_0084_weatherproof_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof oak scabbard for domestic security from prepared oak stock.",
		Action = "making weatherproof oak scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5140.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5140 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0084_weatherproof_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak scabbard for domestic security (@industrialised_weapons_police_military_0084_weatherproof_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofMildSteelCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0085WeatherproofCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0085_weatherproof_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0085_weatherproof_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0085_weatherproof_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof mild steel cartridge for domestic security from prepared mild steel stock.",
		Action = "making weatherproof mild steel cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5181.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5181 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0085_weatherproof_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel cartridge for domestic security (@industrialised_weapons_police_military_0085_weatherproof_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofLeatherMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0086WeatherproofMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof leather magazine for domestic security [craft_industrialised_weapons_police_military_0086_weatherproof_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0086_weatherproof_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof leather magazine for domestic security [craft_industrialised_weapons_police_military_0086_weatherproof_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof leather magazine for domestic security from prepared leather stock.",
		Action = "making weatherproof leather magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof leather magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5222.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5222 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0086_weatherproof_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof leather magazine for domestic security (@industrialised_weapons_police_military_0086_weatherproof_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofCottonShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0087WeatherproofShield() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof cotton shield for domestic security [craft_industrialised_weapons_police_military_0087_weatherproof_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0087_weatherproof_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton shield for domestic security [craft_industrialised_weapons_police_military_0087_weatherproof_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof cotton shield for domestic security from prepared cotton stock.",
		Action = "making weatherproof cotton shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5262.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5262 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0087_weatherproof_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton shield for domestic security (@industrialised_weapons_police_military_0087_weatherproof_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofOakHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0088WeatherproofHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof oak helmet for domestic security [craft_industrialised_weapons_police_military_0088_weatherproof_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0088_weatherproof_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak helmet for domestic security [craft_industrialised_weapons_police_military_0088_weatherproof_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof oak helmet for domestic security from prepared oak stock.",
		Action = "making weatherproof oak helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5303.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5303 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0088_weatherproof_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak helmet for domestic security (@industrialised_weapons_police_military_0088_weatherproof_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofMildSteelPouchForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0089WeatherproofPouch() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0089_weatherproof_pouch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0089_weatherproof_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0089_weatherproof_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof mild steel pouch for domestic security from prepared mild steel stock.",
		Action = "making weatherproof mild steel pouch for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel pouch for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5344.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5344 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0089_weatherproof_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel pouch for domestic security (@industrialised_weapons_police_military_0089_weatherproof_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofLeatherSignalForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0090WeatherproofSignal() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof leather signal for domestic security [craft_industrialised_weapons_police_military_0090_weatherproof_signal]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0090_weatherproof_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof leather signal for domestic security [craft_industrialised_weapons_police_military_0090_weatherproof_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof leather signal for domestic security from prepared leather stock.",
		Action = "making weatherproof leather signal for domestic security",
		ActiveCraftItemSdesc = "an in-progress weatherproof leather signal for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5384.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5384 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0090_weatherproof_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof leather signal for domestic security (@industrialised_weapons_police_military_0090_weatherproof_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeServiceCottonScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0091ServiceScrap() => new()
	{
		Metadata = new("Weapons, police and military/make service cotton scrap for domestic security [craft_industrialised_weapons_police_military_0091_service_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0091_service_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton scrap for domestic security [craft_industrialised_weapons_police_military_0091_service_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a service cotton scrap for domestic security from prepared cotton stock.",
		Action = "making service cotton scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress service cotton scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5425.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5425 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0091_service_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton scrap for domestic security (@industrialised_weapons_police_military_0091_service_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeServiceOakHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0092ServiceHolster() => new()
	{
		Metadata = new("Weapons, police and military/make service oak holster for domestic security [craft_industrialised_weapons_police_military_0092_service_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0092_service_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak holster for domestic security [craft_industrialised_weapons_police_military_0092_service_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a service oak holster for domestic security from prepared oak stock.",
		Action = "making service oak holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress service oak holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5466.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5466 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0092_service_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak holster for domestic security (@industrialised_weapons_police_military_0092_service_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeServiceMildSteelBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0093ServiceBaton() => new()
	{
		Metadata = new("Weapons, police and military/make service mild steel baton for domestic security [craft_industrialised_weapons_police_military_0093_service_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0093_service_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel baton for domestic security [craft_industrialised_weapons_police_military_0093_service_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a service mild steel baton for domestic security from prepared mild steel stock.",
		Action = "making service mild steel baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress service mild steel baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 62.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("62 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0093_service_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel baton for domestic security (@industrialised_weapons_police_military_0093_service_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeServiceLeatherScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0094ServiceScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make service leather scabbard for domestic security [craft_industrialised_weapons_police_military_0094_service_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0094_service_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service leather scabbard for domestic security [craft_industrialised_weapons_police_military_0094_service_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make a service leather scabbard for domestic security from prepared leather stock.",
		Action = "making service leather scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress service leather scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 102.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("102 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0094_service_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service leather scabbard for domestic security (@industrialised_weapons_police_military_0094_service_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeServiceCottonCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0095ServiceCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make service cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0095_service_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0095_service_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton cartridge for domestic security [craft_industrialised_weapons_police_military_0095_service_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make a service cotton cartridge for domestic security from prepared cotton stock.",
		Action = "making service cotton cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress service cotton cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0095_service_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton cartridge for domestic security (@industrialised_weapons_police_military_0095_service_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeServiceOakMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0096ServiceMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make service oak magazine for domestic security [craft_industrialised_weapons_police_military_0096_service_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0096_service_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak magazine for domestic security [craft_industrialised_weapons_police_military_0096_service_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make a service oak magazine for domestic security from prepared oak stock.",
		Action = "making service oak magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress service oak magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 184.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("184 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0096_service_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak magazine for domestic security (@industrialised_weapons_police_military_0096_service_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeServiceMildSteelShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0097ServiceShield() => new()
	{
		Metadata = new("Weapons, police and military/make service mild steel shield for domestic security [craft_industrialised_weapons_police_military_0097_service_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0097_service_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel shield for domestic security [craft_industrialised_weapons_police_military_0097_service_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make a service mild steel shield for domestic security from prepared mild steel stock.",
		Action = "making service mild steel shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress service mild steel shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 224.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("224 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0097_service_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel shield for domestic security (@industrialised_weapons_police_military_0097_service_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeServiceLeatherHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0098ServiceHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make service leather helmet for domestic security [craft_industrialised_weapons_police_military_0098_service_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0098_service_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service leather helmet for domestic security [craft_industrialised_weapons_police_military_0098_service_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make a service leather helmet for domestic security from prepared leather stock.",
		Action = "making service leather helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress service leather helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 265.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("265 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0098_service_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service leather helmet for domestic security (@industrialised_weapons_police_military_0098_service_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalCottonScrapForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0001InstitutionalScrap() => new()
	{
		Metadata = new("Weapons, police and military/make institutional cotton scrap for workshop security [craft_industrial_weapons_police_military_0001_institutional_scrap]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0001_institutional_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton scrap for workshop security [craft_industrial_weapons_police_military_0001_institutional_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional cotton scrap for workshop security from prepared cotton stock.",
		Action = "making institutional cotton scrap for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional cotton scrap for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1762.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1762 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0001_institutional_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton scrap for workshop security (@industrial_weapons_police_military_0001_institutional_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalOakHolsterForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0002InstitutionalHolster() => new()
	{
		Metadata = new("Weapons, police and military/make institutional oak holster for workshop security [craft_industrial_weapons_police_military_0002_institutional_holster]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0002_institutional_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak holster for workshop security [craft_industrial_weapons_police_military_0002_institutional_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional oak holster for workshop security from prepared oak stock.",
		Action = "making institutional oak holster for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional oak holster for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1803.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1803 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0002_institutional_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak holster for workshop security (@industrial_weapons_police_military_0002_institutional_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalMildSteelBatonForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0003InstitutionalBaton() => new()
	{
		Metadata = new("Weapons, police and military/make institutional mild steel baton for workshop security [craft_industrial_weapons_police_military_0003_institutional_baton]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0003_institutional_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel baton for workshop security [craft_industrial_weapons_police_military_0003_institutional_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional mild steel baton for workshop security from prepared mild steel stock.",
		Action = "making institutional mild steel baton for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel baton for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1844.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1844 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0003_institutional_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel baton for workshop security (@industrial_weapons_police_military_0003_institutional_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalLeatherScabbardForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0004InstitutionalScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make institutional leather scabbard for workshop security [craft_industrial_weapons_police_military_0004_institutional_scabbard]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0004_institutional_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional leather scabbard for workshop security [craft_industrial_weapons_police_military_0004_institutional_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional leather scabbard for workshop security from prepared leather stock.",
		Action = "making institutional leather scabbard for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional leather scabbard for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1884.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1884 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0004_institutional_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional leather scabbard for workshop security (@industrial_weapons_police_military_0004_institutional_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalCottonCartridgeForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0005InstitutionalCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make institutional cotton cartridge for workshop security [craft_industrial_weapons_police_military_0005_institutional_cartridge]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0005_institutional_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton cartridge for workshop security [craft_industrial_weapons_police_military_0005_institutional_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional cotton cartridge for workshop security from prepared cotton stock.",
		Action = "making institutional cotton cartridge for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional cotton cartridge for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1925.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1925 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0005_institutional_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton cartridge for workshop security (@industrial_weapons_police_military_0005_institutional_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalOakMagazineForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0006InstitutionalMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make institutional oak magazine for workshop security [craft_industrial_weapons_police_military_0006_institutional_magazine]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0006_institutional_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak magazine for workshop security [craft_industrial_weapons_police_military_0006_institutional_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional oak magazine for workshop security from prepared oak stock.",
		Action = "making institutional oak magazine for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional oak magazine for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1966.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1966 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0006_institutional_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak magazine for workshop security (@industrial_weapons_police_military_0006_institutional_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalMildSteelShieldForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0007InstitutionalShield() => new()
	{
		Metadata = new("Weapons, police and military/make institutional mild steel shield for workshop security [craft_industrial_weapons_police_military_0007_institutional_shield]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0007_institutional_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel shield for workshop security [craft_industrial_weapons_police_military_0007_institutional_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional mild steel shield for workshop security from prepared mild steel stock.",
		Action = "making institutional mild steel shield for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel shield for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2006.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2006 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0007_institutional_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel shield for workshop security (@industrial_weapons_police_military_0007_institutional_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalLeatherHelmetForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0008InstitutionalHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make institutional leather helmet for workshop security [craft_industrial_weapons_police_military_0008_institutional_helmet]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0008_institutional_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional leather helmet for workshop security [craft_industrial_weapons_police_military_0008_institutional_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional leather helmet for workshop security from prepared leather stock.",
		Action = "making institutional leather helmet for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional leather helmet for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2047.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2047 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0008_institutional_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional leather helmet for workshop security (@industrial_weapons_police_military_0008_institutional_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalCottonPouchForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0009InstitutionalPouch() => new()
	{
		Metadata = new("Weapons, police and military/make institutional cotton pouch for workshop security [craft_industrial_weapons_police_military_0009_institutional_pouch]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0009_institutional_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton pouch for workshop security [craft_industrial_weapons_police_military_0009_institutional_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional cotton pouch for workshop security from prepared cotton stock.",
		Action = "making institutional cotton pouch for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional cotton pouch for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2088.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2088 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0009_institutional_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton pouch for workshop security (@industrial_weapons_police_military_0009_institutional_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeInstitutionalOakSignalForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0010InstitutionalSignal() => new()
	{
		Metadata = new("Weapons, police and military/make institutional oak signal for workshop security [craft_industrial_weapons_police_military_0010_institutional_signal]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0010_institutional_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak signal for workshop security [craft_industrial_weapons_police_military_0010_institutional_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make an institutional oak signal for workshop security from prepared oak stock.",
		Action = "making institutional oak signal for workshop security",
		ActiveCraftItemSdesc = "an in-progress institutional oak signal for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2128.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2128 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0010_institutional_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak signal for workshop security (@industrial_weapons_police_military_0010_institutional_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofMildSteelScrapForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0011WeatherproofScrap() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof mild steel scrap for workshop security [craft_industrial_weapons_police_military_0011_weatherproof_scrap]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0011_weatherproof_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel scrap for workshop security [craft_industrial_weapons_police_military_0011_weatherproof_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof mild steel scrap for workshop security from prepared mild steel stock.",
		Action = "making weatherproof mild steel scrap for workshop security",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel scrap for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2169.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2169 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0011_weatherproof_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel scrap for workshop security (@industrial_weapons_police_military_0011_weatherproof_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofLeatherHolsterForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0012WeatherproofHolster() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof leather holster for workshop security [craft_industrial_weapons_police_military_0012_weatherproof_holster]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0012_weatherproof_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof leather holster for workshop security [craft_industrial_weapons_police_military_0012_weatherproof_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof leather holster for workshop security from prepared leather stock.",
		Action = "making weatherproof leather holster for workshop security",
		ActiveCraftItemSdesc = "an in-progress weatherproof leather holster for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2210.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2210 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0012_weatherproof_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof leather holster for workshop security (@industrial_weapons_police_military_0012_weatherproof_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeWeatherproofCottonBatonForWorkshopSecurityCraftIndustrialWeaponsPoliceMilitary0013WeatherproofBaton() => new()
	{
		Metadata = new("Weapons, police and military/make weatherproof cotton baton for workshop security [craft_industrial_weapons_police_military_0013_weatherproof_baton]", "crafts", ["industrial"], ["item:industrial_weapons_police_military_0013_weatherproof_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton baton for workshop security [craft_industrial_weapons_police_military_0013_weatherproof_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a weatherproof cotton baton for workshop security from prepared cotton stock.",
		Action = "making weatherproof cotton baton for workshop security",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton baton for workshop security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2251.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2251 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_weapons_police_military_0013_weatherproof_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton baton for workshop security (@industrial_weapons_police_military_0013_weatherproof_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainSpringSteelScrapForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0001PlainScrap() => new()
	{
		Metadata = new("Transport support and spares/make plain spring steel scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain spring steel scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0001_plain_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make a plain spring steel scrap for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making plain spring steel scrap for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain spring steel scrap for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1873.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1873 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain spring steel scrap for domestic vehicle servicing (@industrialised_transport_support_spares_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainNaturalRubberWheelForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0002PlainWheel() => new()
	{
		Metadata = new("Transport support and spares/make plain natural rubber wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0002_plain_wheel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0002_plain_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0002_plain_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make a plain natural rubber wheel for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making plain natural rubber wheel for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber wheel for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1914.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1914 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0002_plain_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber wheel for domestic vehicle servicing (@industrialised_transport_support_spares_0002_plain_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainOakAxleForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0003PlainAxle() => new()
	{
		Metadata = new("Transport support and spares/make plain oak axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0003_plain_axle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0003_plain_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0003_plain_axle]",
		Category = "Transport support and spares",
		Blurb = "Make a plain oak axle for domestic vehicle servicing from prepared oak stock.",
		Action = "making plain oak axle for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain oak axle for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1955.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1955 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0003_plain_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak axle for domestic vehicle servicing (@industrialised_transport_support_spares_0003_plain_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainMildSteelBearingForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0004PlainBearing() => new()
	{
		Metadata = new("Transport support and spares/make plain mild steel bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0004_plain_bearing]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0004_plain_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0004_plain_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make a plain mild steel bearing for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making plain mild steel bearing for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain mild steel bearing for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1995.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1995 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0004_plain_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel bearing for domestic vehicle servicing (@industrialised_transport_support_spares_0004_plain_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainSpringSteelSpringForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0005PlainSpring() => new()
	{
		Metadata = new("Transport support and spares/make plain spring steel spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0005_plain_spring]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0005_plain_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain spring steel spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0005_plain_spring]",
		Category = "Transport support and spares",
		Blurb = "Make a plain spring steel spring for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making plain spring steel spring for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain spring steel spring for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2036.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2036 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0005_plain_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain spring steel spring for domestic vehicle servicing (@industrialised_transport_support_spares_0005_plain_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainNaturalRubberLampForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0006PlainLamp() => new()
	{
		Metadata = new("Transport support and spares/make plain natural rubber lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0006_plain_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0006_plain_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0006_plain_lamp]",
		Category = "Transport support and spares",
		Blurb = "Make a plain natural rubber lamp for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making plain natural rubber lamp for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber lamp for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2077.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2077 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0006_plain_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber lamp for domestic vehicle servicing (@industrialised_transport_support_spares_0006_plain_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainOakPumpForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0007PlainPump() => new()
	{
		Metadata = new("Transport support and spares/make plain oak pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0007_plain_pump]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0007_plain_pump"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0007_plain_pump]",
		Category = "Transport support and spares",
		Blurb = "Make a plain oak pump for domestic vehicle servicing from prepared oak stock.",
		Action = "making plain oak pump for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain oak pump for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2118.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2118 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0007_plain_pump", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak pump for domestic vehicle servicing (@industrialised_transport_support_spares_0007_plain_pump)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainMildSteelJackForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0008PlainJack() => new()
	{
		Metadata = new("Transport support and spares/make plain mild steel jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0008_plain_jack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0008_plain_jack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0008_plain_jack]",
		Category = "Transport support and spares",
		Blurb = "Make a plain mild steel jack for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making plain mild steel jack for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain mild steel jack for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2158.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2158 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0008_plain_jack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel jack for domestic vehicle servicing (@industrialised_transport_support_spares_0008_plain_jack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainSpringSteelChainForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0009PlainChain() => new()
	{
		Metadata = new("Transport support and spares/make plain spring steel chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0009_plain_chain]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0009_plain_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain spring steel chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0009_plain_chain]",
		Category = "Transport support and spares",
		Blurb = "Make a plain spring steel chain for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making plain spring steel chain for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain spring steel chain for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2199.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2199 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0009_plain_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain spring steel chain for domestic vehicle servicing (@industrialised_transport_support_spares_0009_plain_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakePlainNaturalRubberTyreForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0010PlainTyre() => new()
	{
		Metadata = new("Transport support and spares/make plain natural rubber tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0010_plain_tyre]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0010_plain_tyre"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain natural rubber tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0010_plain_tyre]",
		Category = "Transport support and spares",
		Blurb = "Make a plain natural rubber tyre for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making plain natural rubber tyre for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress plain natural rubber tyre for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2240.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2240 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0010_plain_tyre", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain natural rubber tyre for domestic vehicle servicing (@industrialised_transport_support_spares_0010_plain_tyre)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedOakScrapForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0011ReinforcedScrap() => new()
	{
		Metadata = new("Transport support and spares/make reinforced oak scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0011_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0011_reinforced_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced oak scrap for domestic vehicle servicing from prepared oak stock.",
		Action = "making reinforced oak scrap for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak scrap for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2280.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2280 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak scrap for domestic vehicle servicing (@industrialised_transport_support_spares_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedMildSteelWheelForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0012ReinforcedWheel() => new()
	{
		Metadata = new("Transport support and spares/make reinforced mild steel wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0012_reinforced_wheel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0012_reinforced_wheel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel wheel for domestic vehicle servicing [craft_industrialised_transport_support_spares_0012_reinforced_wheel]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced mild steel wheel for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making reinforced mild steel wheel for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel wheel for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2321.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2321 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0012_reinforced_wheel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel wheel for domestic vehicle servicing (@industrialised_transport_support_spares_0012_reinforced_wheel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedSpringSteelAxleForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0013ReinforcedAxle() => new()
	{
		Metadata = new("Transport support and spares/make reinforced spring steel axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0013_reinforced_axle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0013_reinforced_axle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced spring steel axle for domestic vehicle servicing [craft_industrialised_transport_support_spares_0013_reinforced_axle]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced spring steel axle for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making reinforced spring steel axle for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced spring steel axle for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2362.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2362 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0013_reinforced_axle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced spring steel axle for domestic vehicle servicing (@industrialised_transport_support_spares_0013_reinforced_axle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedNaturalRubberBearingForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0014ReinforcedBearing() => new()
	{
		Metadata = new("Transport support and spares/make reinforced natural rubber bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0014_reinforced_bearing]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0014_reinforced_bearing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber bearing for domestic vehicle servicing [craft_industrialised_transport_support_spares_0014_reinforced_bearing]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced natural rubber bearing for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making reinforced natural rubber bearing for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber bearing for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2402.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2402 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0014_reinforced_bearing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber bearing for domestic vehicle servicing (@industrialised_transport_support_spares_0014_reinforced_bearing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedOakSpringForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0015ReinforcedSpring() => new()
	{
		Metadata = new("Transport support and spares/make reinforced oak spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0015_reinforced_spring]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0015_reinforced_spring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak spring for domestic vehicle servicing [craft_industrialised_transport_support_spares_0015_reinforced_spring]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced oak spring for domestic vehicle servicing from prepared oak stock.",
		Action = "making reinforced oak spring for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak spring for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2443.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2443 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0015_reinforced_spring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak spring for domestic vehicle servicing (@industrialised_transport_support_spares_0015_reinforced_spring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedMildSteelLampForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0016ReinforcedLamp() => new()
	{
		Metadata = new("Transport support and spares/make reinforced mild steel lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0016_reinforced_lamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0016_reinforced_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel lamp for domestic vehicle servicing [craft_industrialised_transport_support_spares_0016_reinforced_lamp]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced mild steel lamp for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making reinforced mild steel lamp for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel lamp for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2484.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2484 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0016_reinforced_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel lamp for domestic vehicle servicing (@industrialised_transport_support_spares_0016_reinforced_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedSpringSteelPumpForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0017ReinforcedPump() => new()
	{
		Metadata = new("Transport support and spares/make reinforced spring steel pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0017_reinforced_pump]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0017_reinforced_pump"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced spring steel pump for domestic vehicle servicing [craft_industrialised_transport_support_spares_0017_reinforced_pump]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced spring steel pump for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making reinforced spring steel pump for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced spring steel pump for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2524.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2524 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0017_reinforced_pump", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced spring steel pump for domestic vehicle servicing (@industrialised_transport_support_spares_0017_reinforced_pump)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedNaturalRubberJackForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0018ReinforcedJack() => new()
	{
		Metadata = new("Transport support and spares/make reinforced natural rubber jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0018_reinforced_jack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0018_reinforced_jack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced natural rubber jack for domestic vehicle servicing [craft_industrialised_transport_support_spares_0018_reinforced_jack]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced natural rubber jack for domestic vehicle servicing from prepared natural rubber stock.",
		Action = "making reinforced natural rubber jack for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced natural rubber jack for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2565.0, Material: "natural rubber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2565 grams of natural rubber", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0018_reinforced_jack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced natural rubber jack for domestic vehicle servicing (@industrialised_transport_support_spares_0018_reinforced_jack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedOakChainForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0019ReinforcedChain() => new()
	{
		Metadata = new("Transport support and spares/make reinforced oak chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0019_reinforced_chain]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0019_reinforced_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak chain for domestic vehicle servicing [craft_industrialised_transport_support_spares_0019_reinforced_chain]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced oak chain for domestic vehicle servicing from prepared oak stock.",
		Action = "making reinforced oak chain for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak chain for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2606.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2606 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0019_reinforced_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak chain for domestic vehicle servicing (@industrialised_transport_support_spares_0019_reinforced_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeReinforcedMildSteelTyreForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0020ReinforcedTyre() => new()
	{
		Metadata = new("Transport support and spares/make reinforced mild steel tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0020_reinforced_tyre]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0020_reinforced_tyre"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel tyre for domestic vehicle servicing [craft_industrialised_transport_support_spares_0020_reinforced_tyre]",
		Category = "Transport support and spares",
		Blurb = "Make a reinforced mild steel tyre for domestic vehicle servicing from prepared mild steel stock.",
		Action = "making reinforced mild steel tyre for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel tyre for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2647.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2647 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0020_reinforced_tyre", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel tyre for domestic vehicle servicing (@industrialised_transport_support_spares_0020_reinforced_tyre)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TransportSupportAndSparesMakeCompactSpringSteelScrapForDomesticVehicleServicingCraftIndustrialisedTransportSupportSpares0021CompactScrap() => new()
	{
		Metadata = new("Transport support and spares/make compact spring steel scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0021_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0021_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact spring steel scrap for domestic vehicle servicing [craft_industrialised_transport_support_spares_0021_compact_scrap]",
		Category = "Transport support and spares",
		Blurb = "Make a compact spring steel scrap for domestic vehicle servicing from prepared spring steel stock.",
		Action = "making compact spring steel scrap for domestic vehicle servicing",
		ActiveCraftItemSdesc = "an in-progress compact spring steel scrap for domestic vehicle servicing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2687.0, Material: "spring steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2687 grams of spring steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_transport_support_spares_0021_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact spring steel scrap for domestic vehicle servicing (@industrialised_transport_support_spares_0021_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
