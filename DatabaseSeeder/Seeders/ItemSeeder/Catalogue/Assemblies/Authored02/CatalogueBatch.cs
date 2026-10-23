#nullable enable

namespace DatabaseSeeder.Seeders;

public static partial class Authored02Stock
{
	public static CatalogueItemDefinition? Find(string key) => key switch
	{
		_ => null
	};
}
