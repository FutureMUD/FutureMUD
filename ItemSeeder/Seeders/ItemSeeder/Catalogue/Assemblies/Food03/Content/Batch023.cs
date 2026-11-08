#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Food03Stock
{
	public static CatalogueFoodDefinition PreindustrialFoodStaleBreadSoup() => new(
		Item: new()
	{
		Metadata = new("preindustrial_food_stale_bread_soup", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialFoodCatalogue.cs:353", []),
		Noun = "bowl",
		ShortDescription = "a bowl of stale bread soup",
		LongDescription = null,
		FullDescription = "Old bread has broken into sour-smelling lumps within a thin beige soup.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 380,
		Cost = 1m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bread",
		Tags = ["Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Scope / Shared", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Soup", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Social Register / Bleak"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Catalogue_preindustrial_food_stale_bread_soup"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hand-authored pre-industrial food catalogue row. Scope: Shared. Admission profile: Universal. Nutrition band: BleakThin.",
		AllowLegacyShortDescriptionMatch = false,
	},
		Scope: FoodCatalogueScope.Shared,
		Kind: FoodCatalogueKind.Prepared,
		Family: FoodCatalogueFamily.Soup,
		Taste: "Softened crust lends a toasted taste despite the staleness.",
		Nutrition: FoodNutritionBand.BleakThin,
		Freshness: FoodFreshnessBand.Cooked,
		AdmissionProfile: FoodAdmissionProfile.Universal,
		Source: "DatabaseSeeder/Seeders/ItemSeeder/FoodCatalogue/Shared/soup-stew.food-items.tsv:247"
	);

	public static CatalogueFoodDefinition PreindustrialFoodBranAndGreensGruel() => new(
		Item: new()
	{
		Metadata = new("preindustrial_food_bran_and_greens_gruel", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialFoodCatalogue.cs:353", []),
		Noun = "bowl",
		ShortDescription = "a bowl of bran and greens gruel",
		LongDescription = null,
		FullDescription = "Coarse brown bran suspends wilted leaves in a dark, gritty gruel.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Poor,
		WeightInGrams = 380,
		Cost = 1m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "vegetable",
		Tags = ["Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Scope / Shared", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Soup", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Social Register / Bleak"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Catalogue_preindustrial_food_bran_and_greens_gruel"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hand-authored pre-industrial food catalogue row. Scope: Shared. Admission profile: Universal. Nutrition band: BleakSolid.",
		AllowLegacyShortDescriptionMatch = false,
	},
		Scope: FoodCatalogueScope.Shared,
		Kind: FoodCatalogueKind.Prepared,
		Family: FoodCatalogueFamily.Soup,
		Taste: "It tastes rough, bitter, and faintly nutty.",
		Nutrition: FoodNutritionBand.BleakSolid,
		Freshness: FoodFreshnessBand.Cooked,
		AdmissionProfile: FoodAdmissionProfile.Universal,
		Source: "DatabaseSeeder/Seeders/ItemSeeder/FoodCatalogue/Shared/soup-stew.food-items.tsv:248"
	);

	public static CatalogueFoodDefinition PreindustrialFoodRootPeelingSoup() => new(
		Item: new()
	{
		Metadata = new("preindustrial_food_root_peeling_soup", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialFoodCatalogue.cs:353", []),
		Noun = "bowl",
		ShortDescription = "a bowl of root peeling soup",
		LongDescription = null,
		FullDescription = "Curled root peelings and trimmed ends float in a cloudy, earth-coloured liquor.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 380,
		Cost = 1m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "vegetable",
		Tags = ["Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Scope / Shared", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Soup", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Social Register / Bleak"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Catalogue_preindustrial_food_root_peeling_soup"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hand-authored pre-industrial food catalogue row. Scope: Shared. Admission profile: Universal. Nutrition band: BleakThin.",
		AllowLegacyShortDescriptionMatch = false,
	},
		Scope: FoodCatalogueScope.Shared,
		Kind: FoodCatalogueKind.Prepared,
		Family: FoodCatalogueFamily.Soup,
		Taste: "The broth is austere, earthy, and mildly sweet.",
		Nutrition: FoodNutritionBand.BleakThin,
		Freshness: FoodFreshnessBand.Cooked,
		AdmissionProfile: FoodAdmissionProfile.Universal,
		Source: "DatabaseSeeder/Seeders/ItemSeeder/FoodCatalogue/Shared/soup-stew.food-items.tsv:249"
	);

	public static CatalogueFoodDefinition PreindustrialFoodPulseWaterBroth() => new(
		Item: new()
	{
		Metadata = new("preindustrial_food_pulse_water_broth", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialFoodCatalogue.cs:353", []),
		Noun = "bowl",
		ShortDescription = "a bowl of pulse water broth",
		LongDescription = null,
		FullDescription = "Cloudy water drained from cooked pulses carries only a few skins and fragments.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 380,
		Cost = 1m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "food",
		Tags = ["Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Scope / Shared", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Soup", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Social Register / Bleak"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Catalogue_preindustrial_food_pulse_water_broth"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hand-authored pre-industrial food catalogue row. Scope: Shared. Admission profile: Universal. Nutrition band: BleakSolid.",
		AllowLegacyShortDescriptionMatch = false,
	},
		Scope: FoodCatalogueScope.Shared,
		Kind: FoodCatalogueKind.Prepared,
		Family: FoodCatalogueFamily.Soup,
		Taste: "It tastes faintly earthy, starchy, and plainly salted.",
		Nutrition: FoodNutritionBand.BleakSolid,
		Freshness: FoodFreshnessBand.Cooked,
		AdmissionProfile: FoodAdmissionProfile.Universal,
		Source: "DatabaseSeeder/Seeders/ItemSeeder/FoodCatalogue/Shared/soup-stew.food-items.tsv:250"
	);

	public static CatalogueFoodDefinition PreindustrialFoodForagedLeafPottage() => new(
		Item: new()
	{
		Metadata = new("preindustrial_food_foraged_leaf_pottage", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialFoodCatalogue.cs:353", []),
		Noun = "bowl",
		ShortDescription = "a bowl of foraged leaf pottage",
		LongDescription = null,
		FullDescription = "Mixed wild leaves cook into a dense green-brown pottage with fibrous stems throughout.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Poor,
		WeightInGrams = 380,
		Cost = 1m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "vegetable",
		Tags = ["Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Scope / Shared", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Stew", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Social Register / Bleak"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Catalogue_preindustrial_food_foraged_leaf_pottage"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hand-authored pre-industrial food catalogue row. Scope: Shared. Admission profile: Universal. Nutrition band: BleakThin.",
		AllowLegacyShortDescriptionMatch = false,
	},
		Scope: FoodCatalogueScope.Shared,
		Kind: FoodCatalogueKind.Prepared,
		Family: FoodCatalogueFamily.Stew,
		Taste: "It is bitter, mineral, and sturdier than it looks.",
		Nutrition: FoodNutritionBand.BleakThin,
		Freshness: FoodFreshnessBand.Cooked,
		AdmissionProfile: FoodAdmissionProfile.Universal,
		Source: "DatabaseSeeder/Seeders/ItemSeeder/FoodCatalogue/Shared/soup-stew.food-items.tsv:251"
	);
}
