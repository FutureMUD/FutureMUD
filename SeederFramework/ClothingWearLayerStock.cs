#nullable enable

using System.Collections.Generic;
using System.Globalization;

namespace DatabaseSeeder.Seeders;

internal sealed record ClothingWearLayerStock(string Name, string SourceComponent, double LayerWeight, bool? Bulky = null)
{
	internal string Description => $"Clothing wear configuration based on {SourceComponent}, consuming {LayerWeight.ToString("R", CultureInfo.InvariantCulture)} layer weight." +
		(Bulky is { } bulky ? bulky ? " This configuration is bulky." : " This configuration permits non-bulky layering." : "");
}
