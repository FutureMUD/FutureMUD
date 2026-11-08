// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory14Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch070() =>
	[
		new("vehicle", "vehicle_revolution_coastal_fishing_smack", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "3712e94b952f1c26fa514e2a8b478e2d1de78fb177af746bd74af8db9ecc7acc"),
		new("vehicle", "vehicle_revolution_packet_schooner", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "4274527da8bda67335411de12e77364828ffecd1d844b59c5859617198c02786"),
		new("vehicle", "vehicle_revolution_rail_livestock_wagon", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2848780720bcae14552bb1a3a37014fb4fc90b5c494e7f7a485780736aae1a2"),
		new("vehicle", "vehicle_revolution_river_workboat", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "c568d5c5208feaf9700fd99f79bc637badf59543a2f9aaaea4abe753ebb8d73d"),
		new("vehicle", "vehicle_revolution_survey_cutter", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "042136dbca90a409b760d43fac786e66559b75cece631064060138bad4c43381"),
	];
}
