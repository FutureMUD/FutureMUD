#nullable enable

namespace DatabaseSeeder.Seeders;

public static partial class IndustrialFoodResearchStock
{
	public static CatalogueFoodFreshnessProfilesResearchRow FreshnessProfiles0001() => new(
		SourceFile: "Food/freshness-profiles.tsv", SourceLine: 2,
		Profile: "freshness_chilled",
		StaleDuration: "2 days",
		SpoilDuration: "7 days",
		StaleTarget: "industrialised_food_non_alcoholic_001",
		SpoiledTarget: "industrialised_food_non_alcoholic_002",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodFreshnessProfilesResearchRow FreshnessProfiles0002() => new(
		SourceFile: "Food/freshness-profiles.tsv", SourceLine: 3,
		Profile: "freshness_shelf",
		StaleDuration: "180 days",
		SpoilDuration: "365 days",
		StaleTarget: "industrialised_food_non_alcoholic_001",
		SpoiledTarget: "industrialised_food_non_alcoholic_002",
		ReviewState: "DependencyReviewed"
	);
}
