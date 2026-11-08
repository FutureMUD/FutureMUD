#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace DatabaseSeeder.Seeders;

// Selection and persistence remain in the existing writers during staged migration.
internal static class CompiledStockCatalogue
{
	private static readonly Lazy<IReadOnlyList<CatalogueItemDefinition>> MedicalLazy = new(MedicalStock.Items);
	private static readonly Lazy<IReadOnlyList<CatalogueItemDefinition>> IndustrialLazy = new(() =>
		Industrial01Stock.Items().Concat(Industrial02Stock.Items()).Concat(Industrial03Stock.Items())
			.Concat(Industrial04Stock.Items()).ToArray());
	private static readonly Lazy<IReadOnlyDictionary<string, string>> Targets = new(() =>
		Medical.Select(item => (item.Metadata.StableKey, Target: "Catalogue/Assemblies/Medical/Content"))
			.Concat(Industrial.Select(item => (item.Metadata.StableKey, Target: "Catalogue/Assemblies/Industrial*/Content")))
			.ToDictionary(item => item.StableKey, item => item.Target, StringComparer.OrdinalIgnoreCase));
	private static readonly Lazy<IReadOnlyDictionary<string, CatalogueItemDefinition>> MedicalIndex = new(() =>
		Medical.ToDictionary(item => item.Metadata.StableKey, StringComparer.OrdinalIgnoreCase));
	private static readonly Lazy<IReadOnlyDictionary<string, CatalogueItemDefinition>> IndustrialIndex = new(() =>
		Industrial.ToDictionary(item => item.Metadata.StableKey, StringComparer.OrdinalIgnoreCase));
	internal static IReadOnlyList<CatalogueItemDefinition> Medical => MedicalLazy.Value;
	internal static IReadOnlyList<CatalogueItemDefinition> Industrial => IndustrialLazy.Value;
	internal static IReadOnlyList<CataloguePriceEvidenceDefinition> Prices => Industrial01Stock.Prices();
	internal static bool TryTarget(string stableKey, out string? target) => Targets.Value.TryGetValue(stableKey, out target);
	internal static bool TryMedicalItem(string key, out CatalogueItemDefinition? item) => MedicalIndex.Value.TryGetValue(key, out item);
	internal static CatalogueItemDefinition IndustrialItem(string key) => IndustrialIndex.Value[key];

	internal static EraMedicalRepairCatalogueEntry ToMedicalEntry(CatalogueItemDefinition item) =>
		new(item.Metadata.StableKey, item.Noun, item.ShortDescription, item.FullDescription, item.Size, item.Quality,
			item.WeightInGrams, item.Cost, item.Material, item.Tags.ToArray(), item.Components.ToArray(),
			item.BuilderNotes!, item.MedicalCategory!);

	internal static IndustrialisedItemCatalogueRow ToIndustrialRow(CatalogueItemDefinition item)
	{
		var details = item.Industrial ?? throw new InvalidOperationException($"Missing industrial evidence for {item.Metadata.StableKey}.");
		return new(item.Metadata.Source, details.SourceLine, item.Metadata.StableKey, item.Metadata.Module,
			details.Domain, item.Metadata.EraAdmissions, item.Noun, item.ShortDescription, item.FullDescription,
			item.Size, item.Quality, item.WeightInGrams, item.Cost, item.Material, item.Tags, item.Components,
			item.ProfileBindings, details.SupportedClaims, item.MorphToStableReference, item.MorphSeconds ?? 0,
			item.MorphEmote, item.DestroyedItemStableReference, item.Metadata.Evidence, details.SourceNote,
			details.Craftable, details.LifecycleKind);
	}

	internal static IndustrialisedPriceEvidenceRow ToPriceRow(CataloguePriceEvidenceDefinition price) =>
		new(price.Source, price.Line, price.EvidenceId, price.EraBand, price.Locale, price.StartYear, price.EndYear,
			price.Currency, price.NominalPrice, price.QuotedUnit, price.DailyWage, price.LabourDays, price.CostIndex,
			price.SourceClass, price.ComparableFamily, price.Confidence, price.SourceUrl, price.SourcePage, price.Notes);

	// This developer measurement also works from the published executable outside a checkout.
	internal static void Benchmark()
	{
		var timer = Stopwatch.StartNew();
		var allocated = GC.GetTotalAllocatedBytes(true);
		var stock =
			(Industrial.Select(ToIndustrialRow).ToArray(), Prices.Select(ToPriceRow).ToArray());
		var medical = EraMedicalRepairCatalogue.Entries;
		timer.Stop();
		allocated = GC.GetTotalAllocatedBytes(true) - allocated;
		var bytes = JsonSerializer.SerializeToUtf8Bytes(new { Items = stock.Item1, Prices = stock.Item2, Medical = medical });
		Console.WriteLine(JsonSerializer.Serialize(new
		{
			Mode = "compiled-stock", ConstructionMilliseconds = timer.Elapsed.TotalMilliseconds,
			AllocatedBytes = allocated, Items = stock.Item1.Length, Prices = stock.Item2.Length, Medical = medical.Count,
			Digest = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()
		}));
	}
}
