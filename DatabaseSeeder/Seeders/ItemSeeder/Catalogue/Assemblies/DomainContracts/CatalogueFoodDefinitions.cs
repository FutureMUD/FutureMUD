#nullable enable

using MudSharp.GameItems;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public enum FoodCatalogueScope
{
	Shared,
	Medieval,
	Renaissance,
	EarlyModern
}

public enum FoodCatalogueKind
{
	Prepared,
	Intermediate
}

public enum FoodCatalogueFamily
{
	Grain,
	Bread,
	Porridge,
	Noodle,
	Dumpling,
	Pulse,
	Vegetable,
	Soup,
	Stew,
	Meat,
	Poultry,
	Offal,
	Fish,
	Shellfish,
	Dairy,
	Egg,
	Preserved,
	Fruit,
	Nut,
	Sweet,
	Condiment,
	Intermediate,
	Broth,
	DairyDrink,
	FruitDrink,
	GrainDrink,
	FermentedDrink,
	Wine,
	Spirit,
	Tea,
	Coffee,
	Chocolate,
	Sauce,
	Oil,
	Vinegar,
	Syrup
}

public enum FoodNutritionBand
{
	None,
	BleakThin,
	BleakSolid,
	Light,
	Standard,
	Staple,
	Hearty,
	Rich,
	Feast,
	Sweet,
	Preserved,
	Fresh,
	Condiment
}

public enum FoodFreshnessBand
{
	None,
	Fresh,
	Cooked,
	Bread,
	Dry,
	Preserved,
	Fermented,
	ShelfStable
}

public enum FoodAdmissionProfile
{
	Universal,
	RegionalOldWorld,
	European,
	Islamicate,
	SouthAsian,
	EastAsian,
	SubSaharanAfrican,
	IndigenousAmerican,
	Mesoamerican,
	Andean,
	MaritimeTrade,
	SugarTrade,
	TeaTrade,
	CoffeeTrade,
	CacaoTrade,
	NewWorldPostContact,
	EraSpecific
}

public sealed record PreIndustrialFoodLiquidCatalogueEntry(
	string StableReference,
	FoodCatalogueScope Scope,
	FoodCatalogueFamily Family,
	string Name,
	string Description,
	string LongDescription,
	string Taste,
	string Smell,
	string Colour,
	double AlcoholLitresPerLitre,
	double WaterLitresPerLitre,
	double FoodSatiatedHoursPerLitre,
	double DrinkSatiatedHoursPerLitre,
	FoodAdmissionProfile AdmissionProfile);

public sealed record CatalogueFoodDefinition(
	CatalogueItemDefinition Item,
	FoodCatalogueScope Scope,
	FoodCatalogueKind Kind,
	FoodCatalogueFamily Family,
	string Taste,
	FoodNutritionBand Nutrition,
	FoodFreshnessBand Freshness,
	FoodAdmissionProfile AdmissionProfile,
	string Source)
{
	public CatalogueMetadata Metadata => Item.Metadata;
	public string Noun => Item.Noun;
	public string ShortDescription => Item.ShortDescription;
	public string FullDescription => Item.FullDescription;
	public string Material => Item.Material;
	public ItemQuality Quality => Item.Quality;
	public double WeightInGrams => Item.WeightInGrams;
	public decimal Cost => Item.Cost;
}
