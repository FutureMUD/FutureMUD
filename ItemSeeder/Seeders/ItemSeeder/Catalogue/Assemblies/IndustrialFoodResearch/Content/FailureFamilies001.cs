#nullable enable

namespace DatabaseSeeder.Seeders;

public static partial class IndustrialFoodResearchStock
{
	public static CatalogueFoodFailureFamiliesResearchRow FailureFamilies0001() => new(
		SourceFile: "Food/failure-families.tsv", SourceLine: 2,
		StableReference: "failure_food_unusable",
		Description: "an unusable failed food preparation",
		Hazard: "none claimed",
		ReviewState: "DependencyReviewed"
	);
}
