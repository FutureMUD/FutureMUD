#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal static class CompiledIndustrialFoodResearch
{
	private static readonly Lazy<IndustrialisedFoodCatalogueDocument> Cached = new(Build);
	internal static IndustrialisedFoodCatalogueDocument Document => Cached.Value;
	private static IndustrialisedFoodCatalogueDocument Build()
	{
		var concepts = IndustrialFoodResearchStock.Concepts().ToArray();
		var adopted = IndustrialFoodResearchStock.AdoptedDependencies().ToArray();
		var servings = IndustrialFoodResearchStock.Servings().ToArray();
		var entries = IndustrialFoodResearchStock.ServingEntries().ToArray();
		// Compatibility projection for the existing research validator; the source records have named fields.
		var supporting = new Dictionary<string, IReadOnlyList<string[]>>(StringComparer.Ordinal)
		{
			["ingredients.tsv"] = IndustrialFoodResearchStock.Ingredients().Select(row => new[] { row.Profile, row.Order, row.Role, row.Category, row.Description, row.SourceReference, row.Quantity, row.Unit, row.Allergens }).ToArray(),
			["nutrition-profiles.tsv"] = IndustrialFoodResearchStock.NutritionProfiles().Select(row => new[] { row.Profile, row.SatiationHours, row.ThirstHours, row.WaterMillilitres, row.AlcoholMillilitres, row.Basis, row.ReviewState }).ToArray(),
			["dietary-metadata.tsv"] = IndustrialFoodResearchStock.DietaryMetadata().Select(row => new[] { row.Profile, row.Contents, row.AnimalFeedPurposes, row.ReviewState }).ToArray(),
			["freshness-profiles.tsv"] = IndustrialFoodResearchStock.FreshnessProfiles().Select(row => new[] { row.Profile, row.StaleDuration, row.SpoilDuration, row.StaleTarget, row.SpoiledTarget, row.ReviewState }).ToArray(),
			["exceptional-skins.tsv"] = IndustrialFoodResearchStock.ExceptionalSkins().Select(row => new[] { row.StableReference, row.BaseReference, row.Rationale, row.ReviewState }).ToArray(),
			["production-routes.tsv"] = IndustrialFoodResearchStock.ProductionRoutes().Select(row => new[] { row.StableReference, row.Kind, row.Mechanisation, row.ReviewState }).ToArray(),
			["craft-inputs.tsv"] = IndustrialFoodResearchStock.CraftInputs().Select(row => new[] { row.Route, row.Order, row.Reference, row.Quantity, row.Unit }).ToArray(),
			["craft-tools.tsv"] = IndustrialFoodResearchStock.CraftTools().Select(row => new[] { row.Route, row.Order, row.Tag, row.Use }).ToArray(),
			["craft-phases.tsv"] = IndustrialFoodResearchStock.CraftPhases().Select(row => new[] { row.Route, row.Order, row.DurationSeconds, row.Description }).ToArray(),
			["craft-products.tsv"] = IndustrialFoodResearchStock.CraftProducts().Select(row => new[] { row.Route, row.Order, row.Reference, row.Quantity, row.Unit, row.FailureFamily }).ToArray(),
			["failure-families.tsv"] = IndustrialFoodResearchStock.FailureFamilies().Select(row => new[] { row.StableReference, row.Description, row.Hazard, row.ReviewState }).ToArray(),
			["evidence-candidates.tsv"] = IndustrialFoodResearchStock.EvidenceCandidates().Select(row => new[] { row.StableReference, row.EvidenceClass, row.Source, row.ComparableFamily, row.ReviewState }).ToArray(),
		};
		IndustrialisedFoodCatalogue.Validate(concepts, adopted, servings, entries, supporting);
		return new(concepts, adopted, servings, entries, supporting);
	}
}
