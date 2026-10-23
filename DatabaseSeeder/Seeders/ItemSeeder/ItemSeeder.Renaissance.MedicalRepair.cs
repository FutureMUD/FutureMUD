#nullable enable

using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedRenaissanceMedicalAndRepair()
	{
		var entries = EraMedicalRepairCatalogue.Renaissance;
		SeedStraightforwardEraCatalogueItems("Renaissance medical and repair catalogue", entries.Select(x => x.ToItemSpec()));
	}
}
