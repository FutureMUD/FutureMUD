#nullable enable

using System;

namespace DatabaseSeeder.Seeders;

internal static class CompiledAuthoredItemCatalogue
{
	internal static CatalogueItemDefinition Get(string key, string? origin = null)
	{
		if ((key.StartsWith("preindustrial_food_", StringComparison.Ordinal) ||
		     key.StartsWith("medieval_food_", StringComparison.Ordinal) ||
		     key.StartsWith("renaissance_food_", StringComparison.Ordinal) ||
		     key.StartsWith("earlymodern_food_", StringComparison.Ordinal)) &&
		    CompiledFoodCatalogue.Find(key) is { } food) return food.Item;
		if (origin == "SeedDocumentedClothingItems")
			return ClothingSources01Stock.Find(key) ?? ClothingSources02Stock.Find(key) ?? throw new InvalidOperationException($"No clothing definition exists for {key}.");
		return
			Authored01Stock.Find(key) ??
			Authored02Stock.Find(key) ??
			Authored03Stock.Find(key) ??
			Authored04Stock.Find(key) ??
			Authored05Stock.Find(key) ??
			ClothingSources01Stock.Find(key) ?? ClothingSources02Stock.Find(key) ??
			throw new InvalidOperationException($"No authored item definition exists for {key} from {origin}.");
	}
}
