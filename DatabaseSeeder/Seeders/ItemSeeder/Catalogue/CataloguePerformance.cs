#nullable enable

using System;
using System.Diagnostics;
using System.Text.Json;

namespace DatabaseSeeder.Seeders;

internal static class CataloguePerformance
{
	internal sealed record ConstructionMeasurement(string Method, int Count, double Milliseconds, long AllocatedBytes, double TotalWeight);

	internal static ConstructionMeasurement MeasureConstruction(bool deserialize, int count)
	{
		// Repeated representative payload, deliberately not presented as 42,756 distinct compiled definitions.
		var json = JsonSerializer.Serialize(RepresentativeCatalogue.Braies(), CatalogueDefinitionCapture.JsonOptions);
		_ = JsonSerializer.Deserialize<CatalogueItemDefinition>(json, CatalogueDefinitionCapture.JsonOptions);
		var values = new CatalogueItemDefinition[count];
		var allocated = GC.GetTotalAllocatedBytes(true);
		var timer = Stopwatch.StartNew();
		for (var i = 0; i < count; i++) values[i] = deserialize
			? JsonSerializer.Deserialize<CatalogueItemDefinition>(json, CatalogueDefinitionCapture.JsonOptions)!
			: RepresentativeCatalogue.Braies();
		timer.Stop();
		allocated = GC.GetTotalAllocatedBytes(true) - allocated;
		var totalWeight = 0.0;
		foreach (var value in values) totalWeight += value.WeightInGrams;
		GC.KeepAlive(values);
		return new(deserialize ? "JSON representative payload" : "Compiled representative literal", count,
			timer.Elapsed.TotalMilliseconds, allocated, totalWeight);
	}
}
