// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory15Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch069() =>
	[
		new("tag", "Materials / Liquids", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "14d822bdf0a641f7eae4210d8a3760420c2425c92cbc6e5962ee3b4531e5d90d"),
		new("vehicle", "vehicle_antiquity_coastal_sailing_boat", "vehicles", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7860fe54b4d26b14c6e3e48259e083cc17ec9fa8d3d650a840a2c1a078f92bff"),
		new("vehicle", "vehicle_earlymodern_post_chaise", "vehicles", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b29bebf1286f263e26628c68903d275199611d066ffa75c9c1caf6055d5cd135"),
		new("vehicle", "vehicle_earlymodern_stagecoach", "vehicles", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e62cfdc23b14c7e5daf46524566f9957201f105c53cf31e7606845961e1ecfbb"),
		new("vehicle", "vehicle_medieval_covered_wagon", "vehicles", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2c7aa6a2221bb78e6b53c27a443f7823e93159f12dee26a5e61c15730970e86"),
		new("vehicle", "vehicle_revolution_street_omnibus", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "9080dc9b386f436cc073232a3775cd27ee259cee54d8fdd36ed8b429cbc683df"),
	];
}
