// Maintained catalogue dispatch. Explicit content is under Catalogue/Assemblies/HistoricalSpecs*.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static IReadOnlyList<CatalogueItemDefinition> EarlyModernHouseholdItemSpecs => CompiledHistoricalSpecCatalogue.EarlyModernHouseholdItemSpecs;
}
