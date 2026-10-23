// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch067() =>
	[
		new("tag", "Materials / Natural Materials / Food / Grain Crop", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c3fc6c56e7ee8acaac86311d31d362055edb944b98ad029ecabc99123c36c54"),
		new("vehicle", "vehicle_medieval_river_wherry", "vehicles", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ccac9bb6c6e71febc457ecc409ee77a1d576854d09d4aecc068649e73d29d0ac"),
		new("vehicle", "vehicle_preindustrial_dugout_canoe", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "378f349543d65dab82d5e7f4c9d8b06913f8633726c5725d9611fe3b8342f371"),
		new("vehicle", "vehicle_preindustrial_river_cargo_barge", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "89be64f26f3ee374245a19774638367d127b90393d172416a0c9ea9bf41c7894"),
		new("vehicle", "vehicle_revolution_harbour_tender", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fb1e7ca36034c113a201525e7d6b5206f349df4f7775a7ed16ba2494e4bcaaa"),
		new("vehicle", "vehicle_revolution_horse_tram", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "22d96a766ba98f7648ba6cb6a85406c041575fdcfc642ae44b3f508f04d8a28a"),
		new("vehicle", "vehicle_revolution_rail_brake_van", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "67d5f5cc9dfd524f5b4bec30f32459a1e14c631fdf6cc4d040a8083a165ab587"),
	];
}
