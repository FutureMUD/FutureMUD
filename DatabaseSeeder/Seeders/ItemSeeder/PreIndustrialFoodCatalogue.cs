#nullable enable

using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DatabaseSeeder.Seeders;

internal enum FoodCatalogueScope
{
	Shared,
	Medieval,
	Renaissance,
	EarlyModern
}

internal enum FoodCatalogueKind
{
	Prepared,
	Intermediate
}

internal enum FoodCatalogueFamily
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

internal enum FoodNutritionBand
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

internal enum FoodFreshnessBand
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

internal enum FoodAdmissionProfile
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

internal sealed record PreIndustrialFoodItemCatalogueEntry(
	string StableReference,
	FoodCatalogueScope Scope,
	FoodCatalogueKind Kind,
	FoodCatalogueFamily Family,
	string Noun,
	string ShortDescription,
	string FullDescription,
	string Taste,
	string Material,
	FoodNutritionBand Nutrition,
	FoodFreshnessBand Freshness,
	ItemQuality Quality,
	double WeightInGrams,
	decimal Cost,
	FoodAdmissionProfile AdmissionProfile);

internal sealed record PreIndustrialFoodLiquidCatalogueEntry(
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

internal static class PreIndustrialFoodCatalogue
{
	private const string ItemHeader =
		"stable_reference\tscope\tkind\tfamily\tnoun\tshort_description\tfull_description\ttaste\tmaterial\tnutrition\tfreshness\tquality\tweight_grams\tcost\tadmission_profile";

	private const string LiquidHeader =
		"stable_reference\tscope\tfamily\tname\tdescription\tlong_description\ttaste\tsmell\tcolour\talcohol_per_litre\twater_per_litre\tfood_satiation_per_litre\tdrink_satiation_per_litre\tadmission_profile";

	private static readonly Lazy<IReadOnlyList<PreIndustrialFoodItemCatalogueEntry>> LazyItems =
		new(() => ReadItemResources(typeof(PreIndustrialFoodCatalogue).Assembly));

	private static readonly Lazy<IReadOnlyList<PreIndustrialFoodLiquidCatalogueEntry>> LazyLiquids =
		new(() => ReadLiquidResources(typeof(PreIndustrialFoodCatalogue).Assembly));

	internal static IReadOnlyList<PreIndustrialFoodItemCatalogueEntry> Items => LazyItems.Value;
	internal static IReadOnlyList<PreIndustrialFoodLiquidCatalogueEntry> Liquids => LazyLiquids.Value;

	internal static IReadOnlyList<PreIndustrialFoodItemCatalogueEntry> ReadItemResources(Assembly assembly)
	{
		return ReadResources(assembly, ".food-items.tsv", ItemHeader, ParseItem);
	}

	internal static IReadOnlyList<PreIndustrialFoodLiquidCatalogueEntry> ReadLiquidResources(Assembly assembly)
	{
		return ReadResources(assembly, ".food-liquids.tsv", LiquidHeader, ParseLiquid);
	}

	private static IReadOnlyList<T> ReadResources<T>(
		Assembly assembly,
		string suffix,
		string expectedHeader,
		Func<string[], string, int, T> parser)
	{
		var results = new List<T>();
		var resources = assembly
			.GetManifestResourceNames()
			.Where(x => x.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
			.OrderBy(x => x, StringComparer.Ordinal)
			.ToArray();

		if (resources.Length == 0)
		{
			throw new InvalidOperationException($"No embedded pre-industrial food catalogue resources ending in {suffix} were found.");
		}

		foreach (var resource in resources)
		{
			using var stream = assembly.GetManifestResourceStream(resource) ??
				throw new InvalidOperationException($"Could not open embedded food catalogue resource {resource}.");
			using var reader = new StreamReader(stream);
			var header = reader.ReadLine()?.TrimStart('\uFEFF');
			if (!string.Equals(header, expectedHeader, StringComparison.Ordinal))
			{
				throw new InvalidDataException($"Food catalogue resource {resource} has an unexpected header.");
			}

			var lineNumber = 1;
			while (reader.ReadLine() is { } line)
			{
				lineNumber++;
				if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
				{
					continue;
				}

				var rooms = line.Split('\t');
				results.Add(parser(rooms, resource, lineNumber));
			}
		}

		return results;
	}

	private static PreIndustrialFoodItemCatalogueEntry ParseItem(string[] rooms, string resource, int lineNumber)
	{
		RequireRoomCount(rooms, 15, resource, lineNumber);
		return new PreIndustrialFoodItemCatalogueEntry(
			Required(rooms[0], resource, lineNumber, "stable_reference"),
			ParseEnum<FoodCatalogueScope>(rooms[1], resource, lineNumber, "scope"),
			ParseEnum<FoodCatalogueKind>(rooms[2], resource, lineNumber, "kind"),
			ParseEnum<FoodCatalogueFamily>(rooms[3], resource, lineNumber, "family"),
			Required(rooms[4], resource, lineNumber, "noun"),
			Required(rooms[5], resource, lineNumber, "short_description"),
			Required(rooms[6], resource, lineNumber, "full_description"),
			rooms[7].Trim(),
			Required(rooms[8], resource, lineNumber, "material"),
			ParseEnum<FoodNutritionBand>(rooms[9], resource, lineNumber, "nutrition"),
			ParseEnum<FoodFreshnessBand>(rooms[10], resource, lineNumber, "freshness"),
			ParseEnum<ItemQuality>(rooms[11], resource, lineNumber, "quality"),
			ParseDouble(rooms[12], resource, lineNumber, "weight_grams"),
			ParseDecimal(rooms[13], resource, lineNumber, "cost"),
			ParseEnum<FoodAdmissionProfile>(rooms[14], resource, lineNumber, "admission_profile"));
	}

	private static PreIndustrialFoodLiquidCatalogueEntry ParseLiquid(string[] rooms, string resource, int lineNumber)
	{
		RequireRoomCount(rooms, 14, resource, lineNumber);
		return new PreIndustrialFoodLiquidCatalogueEntry(
			Required(rooms[0], resource, lineNumber, "stable_reference"),
			ParseEnum<FoodCatalogueScope>(rooms[1], resource, lineNumber, "scope"),
			ParseEnum<FoodCatalogueFamily>(rooms[2], resource, lineNumber, "family"),
			Required(rooms[3], resource, lineNumber, "name"),
			Required(rooms[4], resource, lineNumber, "description"),
			Required(rooms[5], resource, lineNumber, "long_description"),
			Required(rooms[6], resource, lineNumber, "taste"),
			Required(rooms[7], resource, lineNumber, "smell"),
			Required(rooms[8], resource, lineNumber, "colour"),
			ParseDouble(rooms[9], resource, lineNumber, "alcohol_per_litre"),
			ParseDouble(rooms[10], resource, lineNumber, "water_per_litre"),
			ParseDouble(rooms[11], resource, lineNumber, "food_satiation_per_litre"),
			ParseDouble(rooms[12], resource, lineNumber, "drink_satiation_per_litre"),
			ParseEnum<FoodAdmissionProfile>(rooms[13], resource, lineNumber, "admission_profile"));
	}

	private static void RequireRoomCount(string[] rooms, int expected, string resource, int lineNumber)
	{
		if (rooms.Length != expected)
		{
			throw new InvalidDataException(
				$"Food catalogue resource {resource} line {lineNumber} has {rooms.Length} columns; expected {expected}.");
		}
	}

	private static string Required(string value, string resource, int lineNumber, string field)
	{
		var result = value.Trim();
		return !string.IsNullOrWhiteSpace(result)
			? result
			: throw new InvalidDataException($"Food catalogue resource {resource} line {lineNumber} has no {field}.");
	}

	private static T ParseEnum<T>(string value, string resource, int lineNumber, string field) where T : struct, Enum
	{
		return Enum.TryParse<T>(value.Trim(), true, out var result)
			? result
			: throw new InvalidDataException(
				$"Food catalogue resource {resource} line {lineNumber} has invalid {field} value '{value}'.");
	}

	private static double ParseDouble(string value, string resource, int lineNumber, string field)
	{
		return double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
			? result
			: throw new InvalidDataException(
				$"Food catalogue resource {resource} line {lineNumber} has invalid {field} value '{value}'.");
	}

	private static decimal ParseDecimal(string value, string resource, int lineNumber, string field)
	{
		return decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var result)
			? result
			: throw new InvalidDataException(
				$"Food catalogue resource {resource} line {lineNumber} has invalid {field} value '{value}'.");
	}
}
