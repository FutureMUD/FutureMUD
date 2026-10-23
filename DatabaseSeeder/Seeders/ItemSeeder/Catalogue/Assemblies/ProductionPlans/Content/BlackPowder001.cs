#nullable enable

using System.Collections.Generic;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ProductionPlanStock
{
	public static IReadOnlyList<CataloguePrimaryProductionContract> BlackPowder001() =>
	[
		new("mill and corn black powder", "Powdermaking", "Powdermaking", "Gunpowder Manufacture", ["Commodity - 750 grams of saltpeter; piletag Saltpeter Commodity", "Commodity - 150 grams of charcoal; piletag Charcoal Fuel Commodity", "Commodity - 100 grams of sulfur", "LiquidUse - 0.25 litres of Water"], ["TagTool - Held - an item with the Mortar and Pestle tag"], ["CommodityProduct - 900 grams of gunpowder commodity; tag Gunpowder Commodity"], []),
	];
}
