#nullable enable

using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string PreIndustrialFoodCommodityTagPath =
		"Materials / Food Products / Pre-Industrial Food Commodities";

	private static readonly string[] PreIndustrialFoodCommodityTagNames =
	[
		"Grain Cleaning Stock",
		"Cleaned Grain Commodity",
		"Flour Commodity",
		"Meal Commodity",
		"Bran Commodity",
		"Malted Grain Commodity",
		"Dough Commodity",
		"Oilseed Mash Commodity",
		"Oilseed Cake Commodity",
		"Fruit Must Commodity",
		"Wort Commodity",
		"Raw Meat Commodity",
		"Salted Meat Commodity",
		"Dried Meat Commodity",
		"Smoked Meat Commodity",
		"Raw Fish Commodity",
		"Salted Fish Commodity",
		"Dried Fish Commodity",
		"Smoked Fish Commodity"
	];

	private static readonly string[] PreIndustrialFoodFunctionalToolTagPaths =
	[
		"Functions / Tools / Foodmaking Tools / Threshing Flail",
		"Functions / Tools / Foodmaking Tools / Fruit Press",
		"Functions / Tools / Foodmaking Tools / Oil Press",
		"Functions / Tools / Foodmaking Tools / Smoking Rack",
		"Functions / Tools / Foodmaking Tools / Salting Trough",
		"Functions / Tools / Foodmaking Tools / Bake Oven",
		"Functions / Tools / Foodmaking Tools / Kneading Trough"
	];

	private static readonly string[] PreIndustrialFoodIngredientTagPaths =
	[
		"Materials / Natural Materials / Food / Grain Crop",
		"Materials / Natural Materials / Food / Oilseed Crop",
		"Materials / Natural Materials / Food / Fruit Must Crop",
		"Food and Drink / Raw Ingredients / Raw Non-Fish Meat Cut",
		"Food and Drink / Raw Ingredients / Raw Fish Cut"
	];

	internal static IReadOnlyCollection<string> PreIndustrialFoodCommodityTagsForTesting =>
		PreIndustrialFoodCommodityTagNames;

	private void SeedSharedPreIndustrialFoodFoundation()
	{
		EnsureAntiquityTagPath(PreIndustrialFoodCommodityTagPath);
		foreach (var name in PreIndustrialFoodCommodityTagNames)
		{
			EnsureAntiquityTagPath($"{PreIndustrialFoodCommodityTagPath} / {name}");
		}

		foreach (var path in PreIndustrialFoodFunctionalToolTagPaths)
		{
			EnsureAntiquityTagPath(path);
		}

		foreach (var path in PreIndustrialFoodIngredientTagPaths)
		{
			EnsureAntiquityTagPath(path);
		}
	}
}
