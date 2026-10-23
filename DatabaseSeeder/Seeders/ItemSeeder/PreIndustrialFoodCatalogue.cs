#nullable enable

using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DatabaseSeeder.Seeders;

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

internal static class PreIndustrialFoodCatalogue
{
	private static readonly Lazy<IReadOnlyList<PreIndustrialFoodItemCatalogueEntry>> LazyItems =
		new(() => CompiledFoodCatalogue.Items.Select(ItemSeeder.ToLegacyFood).ToArray());

	private static readonly Lazy<IReadOnlyList<PreIndustrialFoodLiquidCatalogueEntry>> LazyLiquids =
		new(() => FoodLiquidsStock.All().ToArray());

	internal static IReadOnlyList<PreIndustrialFoodItemCatalogueEntry> Items => LazyItems.Value;
	internal static IReadOnlyList<PreIndustrialFoodLiquidCatalogueEntry> Liquids => LazyLiquids.Value;

}
