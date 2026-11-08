#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

/// <summary>Semantic queries over literal definitions, independent of their source-file layout.</summary>
internal static class CompiledRecipeFixtures
{
	private static readonly Lazy<CatalogueRecipeDefinition[]> RecipesLazy = new(() => CompiledRecipeCatalogue.Select(
		new HashSet<string>(["antiquity", "medieval", "renaissance", "earlymodern", "industrial"], StringComparer.OrdinalIgnoreCase), true).ToArray());
	private static readonly Lazy<CatalogueItemDefinition[]> ItemsLazy = new(() =>
		Directory.EnumerateFiles(AppContext.BaseDirectory, "DatabaseSeeder.Catalogue.*.dll")
			.OrderBy(path => path, StringComparer.Ordinal).Select(path => Assembly.Load(AssemblyName.GetAssemblyName(path)))
			.SelectMany(assembly => assembly.GetExportedTypes()).SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
			.Where(method => method.ReturnType == typeof(CatalogueItemDefinition) && method.GetParameters().Length == 0)
			.Select(method => (CatalogueItemDefinition)method.Invoke(null, null)!)
			.Concat(CompiledFoodCatalogue.Items.Select(food => food.Item))
			.GroupBy(item => item.Metadata.StableKey, StringComparer.OrdinalIgnoreCase).Select(group => group.First()).ToArray());
	internal static IReadOnlyList<CatalogueRecipeDefinition> Recipes => RecipesLazy.Value;
	internal static IReadOnlyList<CatalogueItemDefinition> Items => ItemsLazy.Value;
	private static readonly Lazy<Dictionary<string, CatalogueItemDefinition>> ItemIndex = new(() =>
		Items.ToDictionary(item => item.Metadata.StableKey, StringComparer.OrdinalIgnoreCase));
	private static readonly Lazy<Dictionary<string, CatalogueRecipeDefinition[]>> OutputIndex = new(() =>
		Recipes.SelectMany(recipe => Outputs(recipe).Select(key => (Key: key, Recipe: recipe)))
			.GroupBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.Select(pair => pair.Recipe).Distinct().ToArray(), StringComparer.OrdinalIgnoreCase));
	internal static CatalogueItemDefinition Item(string key) => ItemIndex.Value[key];
	internal static CatalogueItemDefinition[] OriginalGroup(string member)
	{
		using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
			"Fixtures", "item-catalogue-original-members.json")));
		var keys = document.RootElement.GetProperty("Members").EnumerateArray()
			.Single(group => group.GetProperty("Member").GetString() == member).GetProperty("Keys");
		return keys.EnumerateArray().Select(key => Item(key.GetString()!)).ToArray();
	}
	internal static IEnumerable<string> Outputs(CatalogueRecipeDefinition recipe) => recipe.Products.SelectMany(product => product switch
	{
		CatalogueItemRecipeProduct value => [value.ItemStableReference],
		CatalogueCookedRecipeProduct value => [value.ItemStableReference],
		CatalogueVariableRecipeProduct value => [value.ItemStableReference],
		CatalogueLiquidRecipeProduct value => [value.ItemStableReference],
		CatalogueProgCookedRecipeProduct value => SelectorItems(value.Prog),
		CatalogueProgRecipeProduct value => SelectorItems(value.Prog),
		_ => Array.Empty<string>()
	});
	private static string[] SelectorItems(string prog) => RecipeSupportStock.Find(prog) is { } definition
		? Regex.Matches(definition.Text, "loaditem\\(\"([^\"]+)\"\\)").Select(match => match.Groups[1].Value).ToArray() : [];
	internal static CatalogueRecipeDefinition[] ForItem(string key) => OutputIndex.Value.GetValueOrDefault(key, []);
	internal static CatalogueRecipeDefinition Named(string name) => Recipes.Single(recipe => recipe.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	internal static void ItemsAreCrafted(IEnumerable<string> keys)
	{
		foreach (var key in keys)
		{
			Assert.IsNotNull(Item(key), key);
			Assert.IsTrue(ForItem(key).Length > 0, $"No typed recipe or literal selector produces {key}.");
		}
	}
	internal static void AssertNativeCraftCoverage(IEnumerable<string> keys, params string[] legacyGaps)
	{
		// These exact gaps were observed in the original native installer, not introduced by conversion.
		var missing = keys.Where(key => ForItem(key).Length == 0).OrderBy(key => key, StringComparer.Ordinal).ToArray();
		CollectionAssert.AreEqual(legacyGaps.OrderBy(key => key, StringComparer.Ordinal).ToArray(), missing,
			"The native legacy craftability boundary changed.");
		ItemsAreCrafted(keys.Except(legacyGaps, StringComparer.Ordinal));
	}
	internal static void Knowledge(string knowledge, IEnumerable<string>? keys = null)
	{
		var recipes = Recipes.Where(recipe => recipe.Gate is CatalogueKnowledgeRecipeGate gate && gate.Knowledge == knowledge).ToArray();
		Assert.IsTrue(recipes.Length > 0, knowledge);
		if (keys is not null)
			foreach (var key in keys) Assert.IsTrue(recipes.Any(recipe => Outputs(recipe).Contains(key)), $"{key} has no {knowledge} route.");
	}
	internal static void CopiesColour(IEnumerable<string> keys)
	{
		foreach (var key in keys)
		{
			var recipes = ForItem(key);
			Assert.IsTrue(recipes.Any(recipe => recipe.Products.OfType<CatalogueVariableRecipeProduct>()
				.Any(product => product.ItemStableReference == key && product.Variables.Any(variable =>
					variable.Definition is "Colour" or "Fine Colour" && variable.InputIndex > 0 && variable.InputIndex <= recipe.Inputs.Count))), key);
		}
	}
	internal static IEnumerable<string> Tags(CatalogueRecipeDefinition recipe) => recipe.Inputs.SelectMany(input => input switch
	{
		CatalogueCommodityRecipeInput value => value.PileTag is null ? [] : new[] { value.PileTag },
		CatalogueCommodityTagRecipeInput value => value.PileTag is null ? [value.MaterialTag] : new[] { value.MaterialTag, value.PileTag },
		CatalogueTagRecipeInput value => [value.Tag],
		CatalogueLiquidTagRecipeInput value => [value.LiquidTag],
		_ => Array.Empty<string>()
	}).Concat(recipe.Tools.OfType<CatalogueTagRecipeTool>().Select(tool => tool.Tag))
		.Concat(recipe.Products.Concat(recipe.FailProducts).OfType<CatalogueCommodityRecipeProduct>().Select(product => product.Tag).OfType<string>());
	internal static void RequiresTags(params string[] tags)
	{
		var actual = Recipes.SelectMany(Tags).ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var tag in tags) Assert.IsTrue(actual.Contains(tag), $"No typed recipe references {tag}.");
	}
	internal static void HasRecipes(params string[] names)
	{
		foreach (var name in names) Assert.IsTrue(Recipes.Any(recipe => recipe.Name.Equals(name, StringComparison.OrdinalIgnoreCase)), name);
	}
}
