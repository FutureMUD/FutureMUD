// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch069() =>
	[
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Scope", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d928a371073cbcf0fb6eb93b618ad761dba914747a555bee2e043291fe9ff9f4"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Scope / Medieval", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6a71cf801b7fb603f92731df985713d9bfc9202729c9b1a79c13f901bd9de06"),
		new("tag", "Materials / Liquids / Ritual Offerings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "22fee0e9cb881cbef3e629e0d19cdb1ea36def9fca22d49178509e9011c9a4f0"),
		new("tag", "Materials / Natural Materials / Food / Ready Fruit Crop", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fef3a4c12434e9ac06f07a18168528d9c6533e156c170fbc228ab4916f3e0055"),
		new("vehicle", "vehicle_earlymodern_coastal_schooner", "vehicles", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "38a60112ac4a5949a4f58d7ae85a982b970bb8dffe95e101e720973e26650d81"),
		new("vehicle", "vehicle_earlymodern_coastal_sloop", "vehicles", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2efbf06dc05acfba4144b9a6ab083b3c632feda5e736a1e35229a5abd8e20dcf"),
		new("vehicle", "vehicle_renaissance_artillery_wagon", "vehicles", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a936b4d962ad3177cd367027f558e3a6b295badb6d7bd93fcc6c4299c0e41021"),
		new("vehicle", "vehicle_revolution_canal_inspection_skiff", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "26d18d3280a8e4633d5e006765208f2bf36a6ba2c2c61229bd68c79cbf0a2362"),
		new("vehicle", "vehicle_revolution_river_ferry_boat", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "918d047d4e3165b697bb72b58cca704b87aa975e3055e32d1bfeb786d67d579e"),
		new("vehicle", "vehicle_revolution_river_sailing_barge", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "2796527f095241773af5ec8fa1ffa8f4e89a446a6bb6d16494f718dafd489068"),
	];
}
