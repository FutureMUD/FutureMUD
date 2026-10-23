#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Frozen;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal static class CompiledFoodCatalogue
{
	internal static readonly IReadOnlyList<CatalogueFoodDefinition> Items = [.. Food01Stock.All(), .. Food02Stock.All(), .. Food03Stock.All()];
	private static readonly FrozenDictionary<string, CatalogueFoodDefinition> ByKey = Items.ToFrozenDictionary(x => x.Metadata.StableKey, StringComparer.OrdinalIgnoreCase);
	internal static CatalogueFoodDefinition? Find(string key) => ByKey.GetValueOrDefault(key);
}
