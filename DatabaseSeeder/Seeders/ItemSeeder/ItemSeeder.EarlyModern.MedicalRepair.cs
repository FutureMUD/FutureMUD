#nullable enable

using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedEarlyModernMedicalAndRepair()
	{
		var entries = EraMedicalRepairCatalogue.Entries;
		SeedStraightforwardEraCatalogueItems(
			"Renaissance survivals and Early Modern medical and repair additions",
			entries.Where(x => x.StableReference.StartsWith("renaissance_", System.StringComparison.Ordinal))
				.Concat(entries.Where(x => x.StableReference.StartsWith("earlymodern_", System.StringComparison.Ordinal)))
				.Select(x => x.ToItemSpec()));
	}
}
