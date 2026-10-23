#nullable enable

namespace DatabaseSeeder.Seeders;

internal static partial class RepresentativeCatalogue
{
	internal static string TargetFor(string stableKey)
	{
		if (CompiledStockCatalogue.TryTarget(stableKey, out var target)) return target!;
		return stableKey switch
		{
			"medieval_linen_braies" => "RepresentativeCatalogue.Braies",
			"medieval_food_lenten_almond_fish_pottage" => "RepresentativeCatalogue.Pottage",
			"medieval_outfit_0001" => "RepresentativeCatalogue.EarlyAngloSaxon",
			"Basketry/weave wicker beehive" => "RepresentativeCatalogue.WickerBeehive",
			"vehicle_antiquity_two_wheeled_handcart" => "RepresentativeVehicleCatalogue.RepresentativeHandcart",
			_ => "retained legacy source; bulk conversion pending"
		};
	}
}
