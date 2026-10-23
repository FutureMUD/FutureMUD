#nullable enable

using System;
using System.Collections.Generic;

namespace DatabaseSeeder;

internal static class CompiledItemSeederInventory
{
	private static readonly Lazy<ItemSeederManifestDocument> Cached = new(Build);
	internal static ItemSeederManifestDocument Document => Cached.Value;
	private static ItemSeederManifestDocument Build()
	{
		var entries = new List<ItemSeederManifestEntry>();
		entries.AddRange(Inventory00Stock.Entries());
		entries.AddRange(Inventory01Stock.Entries());
		entries.AddRange(Inventory02Stock.Entries());
		entries.AddRange(Inventory03Stock.Entries());
		entries.AddRange(Inventory04Stock.Entries());
		entries.AddRange(Inventory05Stock.Entries());
		entries.AddRange(Inventory06Stock.Entries());
		entries.AddRange(Inventory07Stock.Entries());
		entries.AddRange(Inventory08Stock.Entries());
		entries.AddRange(Inventory09Stock.Entries());
		entries.AddRange(Inventory10Stock.Entries());
		entries.AddRange(Inventory11Stock.Entries());
		entries.AddRange(Inventory12Stock.Entries());
		entries.AddRange(Inventory13Stock.Entries());
		entries.AddRange(Inventory14Stock.Entries());
		entries.AddRange(Inventory15Stock.Entries());
		return ItemSeederManifestCatalogue.BuildDocument(entries, "compiled", DateTime.UnixEpoch);
	}
}
