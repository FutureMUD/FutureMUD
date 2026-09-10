#nullable enable

using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedRenaissanceMedicalAndRepair()
	{
		SeedStraightforwardEraCatalogueItems("Renaissance medical and repair catalogue", EraMedicalRepairCatalogue.Renaissance.Select(x => x.ToItemSpec()));
	}
}
