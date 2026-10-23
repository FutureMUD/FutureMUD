// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch070() =>
	[
		new("vehicle", "vehicle_preindustrial_fishing_skiff", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "716c5ec8f41b8ad6b36ec05d6f3f8f5132cbeee8d16532f0afe54969764d579e"),
		new("vehicle", "vehicle_preindustrial_winter_sledge", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78f6af5976fe5e584a395e21341cf03418144663047a59f9b553d02b77feed2c"),
		new("vehicle", "vehicle_revolution_fishing_lugger", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "eccd589a609911e19059391b80501e0b516baf4e063bcb4950d849750de5c8a8"),
		new("vehicle", "vehicle_revolution_rail_flat_wagon", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "93ec6a4b8b3db748fec1b498721fe60b8a94a5068eb4aebe8ba6abe97efa0c06"),
		new("vehicle", "vehicle_revolution_rail_luggage_van", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5b6574e8c52fa9fbac065ca58cca1757be368836e1b34ee1f12337745bab08a"),
	];
}
