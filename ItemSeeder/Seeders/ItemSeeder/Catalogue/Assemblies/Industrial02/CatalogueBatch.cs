#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public static partial class Industrial02Stock
{
	public static IReadOnlyList<CatalogueItemDefinition> Items()
	{
		var items = new List<CatalogueItemDefinition>(1700);
		items.AddRange(Batch001());
		items.AddRange(Batch002());
		items.AddRange(Batch003());
		items.AddRange(Batch004());
		items.AddRange(Batch005());
		items.AddRange(Batch006());
		items.AddRange(Batch007());
		items.AddRange(Batch008());
		items.AddRange(Batch009());
		items.AddRange(Batch010());
		items.AddRange(Batch011());
		items.AddRange(Batch012());
		items.AddRange(Batch013());
		items.AddRange(Batch014());
		items.AddRange(Batch015());
		items.AddRange(Batch016());
		items.AddRange(Batch017());
		return items.AsReadOnly();
	}
}
