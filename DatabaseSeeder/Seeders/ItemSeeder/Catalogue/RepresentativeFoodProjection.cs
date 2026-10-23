#nullable enable

namespace DatabaseSeeder.Seeders;

internal static partial class RepresentativeCatalogue
{
	internal static CatalogueFoodDefinition Pottage() => CompiledFoodCatalogue.Find("medieval_food_lenten_almond_fish_pottage")!;
}
