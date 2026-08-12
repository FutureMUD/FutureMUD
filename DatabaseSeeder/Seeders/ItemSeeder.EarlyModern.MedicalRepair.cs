#nullable enable

using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedEarlyModernMedicalAndRepair()
	{
		SeedStraightforwardEraCatalogueItems(
			"Renaissance survivals and Early Modern medical and repair additions",
			EraMedicalRepairCatalogue.Renaissance
				.Concat(EraMedicalRepairCatalogue.EarlyModern)
				.Select(x => x.ToItemSpec()));
	}
}
