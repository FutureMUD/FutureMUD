// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch069() =>
	[
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Fuel Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "304c9c123983f78bf94051743a5cee2b0d3b82eb809d7e4a5e3196ff369379ed"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Gold-Bearing Gravel Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "001f1ec1e70e5874893cae9d7017bd2f07786d8a32d172fb21965e3455c7b424"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Visible Resource Deposit / Volcanic Ash Pozzolana Deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7f7c6283ceee537cc35322f0494e91a6ee8126670fc81ac6b78caea64fbd7e5"),
		new("tag", "Functions / Tools / Agricultural Tools / Beekeeping Tools", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cf21e144996c970d77dc5dc4e0e42c72021a0c8d02cdcbacb79a920376aec25"),
		new("tag", "Functions / Tools / Timekeeping Tools", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8996d0d74f187413709ec8f72c59367d4bbd7a9c49f297c4ad2667dcff4ccbb"),
		new("tag", "Materials / Food Products / Antiquity Food Commodities / Pulse Meal Commodity", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "47f02d237afb5a8f401b7b68789be9c6bfb2739478cb1adf91a2cd380e34085f"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Grain", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cb7d7bbe3d42155162268d4dac49cbff9a7c237aa5bede9420880c2b073ae80"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Intermediate", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecafb61d3c669dca7700dab1bda5b55fea27eacf9449beb4704ac8dd39b833df"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Meal Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3bf7d9ab73f53084c931557a0999c557158d297c8153d0cacea7f4f9e82e01b"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Scope / Shared", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f637d822afb7c90e7dbe2b481ba72254379b05c9b5112aba9f1d9ab7bc0d5f72"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Smoked Fish Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "222c0cc4ed718e9a7e2a16a97bfdd2450946cf0bba858750647ee74384dbb6d8"),
		new("vehicle", "vehicle_antiquity_racing_chariot", "vehicles", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3aa4589e8eeb4c8027d131b75b9049ebe8e1b7c2e1c2b966d9534acf674ac1e"),
		new("vehicle", "vehicle_revolution_cab", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "9823a070af98058efddbbfed1b2b3351243fc9e8a071f9e2a75c2d5d9e9646ea"),
		new("vehicle", "vehicle_revolution_quarry_tram_wagon", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8baf95fae57850c7011e640d37b25bb1605d2a70586ef481b1dcef515c938af"),
		new("vehicle", "vehicle_revolution_rail_covered_goods_van", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1c80cb9a9ec33a56d6be1f6aee08e81d532744382873e38e330f4ba778f5721"),
		new("vehicle", "vehicle_revolution_rail_passenger_car", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7a94129a2ae3335aedadcca156397def5c4d9a69d82572f435d5c26105596a1"),
	];
}
