// Maintained catalogue dispatch. Explicit content is under Catalogue/Assemblies/HistoricalSpecs*.
#nullable enable

using System.Collections.Generic;

using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static IReadOnlyList<CatalogueItemDefinition> RenaissanceMilitaryItemSpecs => CompiledHistoricalSpecCatalogue.RenaissanceMilitaryItemSpecs;

	private static readonly OutfitManifestSpec[] RenaissanceMilitaryArmourOutfitManifestSpecs =
		[.. Outfits01Stock.RenaissanceMilitaryArmourOutfitManifestSpecs().Concat(Outfits02Stock.RenaissanceMilitaryArmourOutfitManifestSpecs()).Select(ToLegacyOutfit)];

	internal const int RenaissanceMilitaryMeleeWeaponsRowCount = 14;
	internal const int RenaissanceMilitaryRangedWeaponsRowCount = 11;
	internal const int RenaissanceMilitaryFirearmsAmmunitionRowCount = 15;
	internal const int RenaissanceMilitaryArtilleryRowCount = 17;
	internal const int RenaissanceMilitaryArmourBardingRowCount = 98;
	internal const int RenaissanceMilitaryShieldsRowCount = 8;
	internal const int RenaissanceMilitaryMilitarySupportFieldGearRowCount = 13;
	internal const int RenaissanceMilitaryCatalogueRowCount = 176;
	internal const int RenaissanceMilitaryArmourOutfitCount = 62;
}
