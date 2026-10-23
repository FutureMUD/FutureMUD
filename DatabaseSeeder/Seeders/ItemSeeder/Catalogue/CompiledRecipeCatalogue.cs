#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

internal static partial class CompiledRecipeCatalogue
{
	internal static IEnumerable<CatalogueRecipeDefinition> Select(IReadOnlySet<string> eras, bool capture)
	{
		foreach (var recipe in Batch01(eras, capture)) yield return recipe;
		foreach (var recipe in Batch02(eras, capture)) yield return recipe;
		foreach (var recipe in Batch03(eras, capture)) yield return recipe;
		foreach (var recipe in Batch04(eras, capture)) yield return recipe;
		foreach (var recipe in Batch05(eras, capture)) yield return recipe;
		foreach (var recipe in Batch06(eras, capture)) yield return recipe;
		foreach (var recipe in Batch07(eras, capture)) yield return recipe;
		foreach (var recipe in Batch08(eras, capture)) yield return recipe;
		foreach (var recipe in Batch09(eras, capture)) yield return recipe;
		foreach (var recipe in Batch10(eras, capture)) yield return recipe;
		foreach (var recipe in Batch11(eras, capture)) yield return recipe;
		foreach (var recipe in Batch12(eras, capture)) yield return recipe;
		foreach (var recipe in Batch13(eras, capture)) yield return recipe;
		foreach (var recipe in Batch14(eras, capture)) yield return recipe;
		foreach (var recipe in Batch15(eras, capture)) yield return recipe;
		foreach (var recipe in Batch16(eras, capture)) yield return recipe;
		foreach (var recipe in Batch17(eras, capture)) yield return recipe;
		foreach (var recipe in Batch18(eras, capture)) yield return recipe;
		foreach (var recipe in Batch19(eras, capture)) yield return recipe;
		foreach (var recipe in Batch20(eras, capture)) yield return recipe;
		foreach (var recipe in Batch21(eras, capture)) yield return recipe;
		foreach (var recipe in Batch22(eras, capture)) yield return recipe;
		foreach (var recipe in Batch23(eras, capture)) yield return recipe;
		foreach (var recipe in Batch24(eras, capture)) yield return recipe;
		foreach (var recipe in Batch25(eras, capture)) yield return recipe;
		foreach (var recipe in Batch26(eras, capture)) yield return recipe;
		foreach (var recipe in Batch27(eras, capture)) yield return recipe;
		foreach (var recipe in Batch28(eras, capture)) yield return recipe;
		foreach (var recipe in Batch29(eras, capture)) yield return recipe;
		foreach (var recipe in Batch30(eras, capture)) yield return recipe;
		foreach (var recipe in Batch31(eras, capture)) yield return recipe;
		foreach (var recipe in Batch32(eras, capture)) yield return recipe;
		foreach (var recipe in Batch33(eras, capture)) yield return recipe;
		foreach (var recipe in Batch34(eras, capture)) yield return recipe;
		foreach (var recipe in Batch35(eras, capture)) yield return recipe;
		foreach (var recipe in Batch36(eras, capture)) yield return recipe;
	}
}
