#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public static partial class MedicalStock
{
	public static IReadOnlyList<CatalogueItemDefinition> Items()
	{
		var items = new List<CatalogueItemDefinition>(178);
		items.AddRange(Batch001());
		items.AddRange(Batch002());
		return items.AsReadOnly();
	}
}
