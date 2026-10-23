// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch069() =>
	[
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Scope / Renaissance", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3e98c77a7da83bf7e295ada56eaf4c959085e14f0a822267efe2c262ec7e4e8"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Smoked Meat Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3241ece6f41559ed69296129604628091b2756975c24d89e8ff60412736bc644"),
		new("tag", "Materials / Natural Materials", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "47fb25b2c88b2d781ca0370816fefdafccd309f9f426ddf598581361cdc6b8b8"),
		new("vehicle", "vehicle_medieval_clinker_rowboat", "vehicles", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "75ea05986fec87d2a22cb8fbebb680cb124b78d803f57a08d7a2a637a6ea0aa7"),
		new("vehicle", "vehicle_preindustrial_ferry_barge", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6985fd985710f91f6f00368bb9f31d263f80381e2590e04539d867ca5951601a"),
		new("vehicle", "vehicle_renaissance_artillery_limber", "vehicles", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa9c8aa3a51a31e7568401128482f3b19f360813f86c7ac32efb77cbcd0dfba6"),
		new("vehicle", "vehicle_renaissance_city_carriage", "vehicles", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9008a90e28650c48c0808e1c1c58f434d3f34584ffee2fa4c63e8bdf490bc54b"),
		new("vehicle", "vehicle_revolution_brewery_dray", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe186f7d9fc7198085e029cc28a5b881d3840d74b465767c4c6d9e934a4461f1"),
		new("vehicle", "vehicle_revolution_canal_cargo_boat", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "de732022c0c9560b16c8cc64185af68d73c405313265cb6d07f8747ddbb81b85"),
		new("vehicle", "vehicle_revolution_sailing_cutter", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1ae5ee21da489bb01941331737c570526d5ade132ec27b83b2ba2f1f88d6265"),
	];
}
