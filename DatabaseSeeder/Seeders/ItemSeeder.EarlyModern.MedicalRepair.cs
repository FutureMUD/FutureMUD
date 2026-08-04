#nullable enable

using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedEarlyModernMedicalAndRepair()
	{
		SeedStraightforwardEraCatalogueItems("Early Modern medical and repair catalogue", EraMedicalRepairCatalogue.EarlyModern.Select(x => x.ToItemSpec()));
	}
}
