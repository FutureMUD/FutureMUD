#nullable enable

using DatabaseSeeder.Seeders;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DatabaseSeeder;

internal static class IndustrialisedFoodDependencyAudit
{
	internal const string RelativePath = "Design Documents/Seeding/Industrialised_Food_Dependency_Audit.tsv";
	private const string Header = "RecordKind\tStableReference\tSourceFile\tSourceLine\tPhysicalKind\tFamily\tAdmissions\tConsumerOrIngredient\tMaterialOrLiquid\tComponents\tTags\tNutrition\tDietary\tFreshness\tHealth\tProductionRoute\tFailureFamily\tEvidence\tServingParticipation\tReviewState\tValidation\tCatalogueSourceSha256";

	internal static string Generate(IndustrialisedFoodCatalogueDocument document, string sourceHash)
	{
		var servings = document.ServingEntries.ToLookup(x => x.StableReference, x => x.ServingReference, StringComparer.Ordinal);
		var output = new StringBuilder(Header).Append('\n');
		foreach (var row in document.Concepts.OrderBy(x => x.Source, StringComparer.Ordinal).ThenBy(x => x.Line))
		{
			output.AppendJoin('\t', new[]
			{
				"concept", row.StableReference, $"Food/{row.Source}", row.Line.ToString(CultureInfo.InvariantCulture),
				row.PhysicalKind.ToString(), row.Family, string.Join(';', row.Admissions), row.IngredientProfile,
				row.PhysicalKind == IndustrialisedFoodPhysicalKind.Item ? row.Material : row.LiquidDependency,
				string.Join(';', row.Components), string.Join(';', row.Tags),
				row.NutritionProfile, row.DietaryProfile, row.FreshnessProfile, string.Join(';', row.HealthBindings),
				row.ProductionRoute, row.FailureFamily, row.EvidenceRoute,
				string.Join(';', servings[row.StableReference].Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal)),
				row.ReviewState.ToString(), "dependency-graph-resolved;content-unreviewed", sourceHash
			}).Append('\n');
		}
		foreach (var row in document.AdoptedDependencies.OrderBy(x => x.Source, StringComparer.Ordinal).ThenBy(x => x.Line))
		{
			output.AppendJoin('\t', new[]
			{
				"adopted-dependency", row.StableReference, $"Food/{row.Source}", row.Line.ToString(CultureInfo.InvariantCulture),
				string.Empty, string.Empty, string.Empty, row.ConsumerReference, string.Empty, string.Empty, string.Empty,
				string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, row.ProductionRole,
				string.Empty, string.Empty, string.Empty, row.ReviewState.ToString(),
				"consumer-resolved", sourceHash
			}).Append('\n');
		}
		return output.ToString();
	}
}
