#nullable enable

using Recipes = MudSharp_Unit_Tests.CompiledRecipeFixtures;

using DatabaseSeeder.Seeders;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederAntiquityPigmentAndDyeTests
{
	private static readonly string[] NewFinePigmentColours =
	[
		"madder red",
		"kermes scarlet",
		"lac crimson",
		"alkanet purple",
		"orchil violet",
		"tyrian purple",
		"woad blue",
		"egyptian blue",
		"azurite blue",
		"lapis blue",
		"malachite green",
		"verdigris green",
		"orpiment yellow",
		"realgar orange",
		"hematite red",
		"cinnabar red",
		"red ochre",
		"yellow ochre",
		"lamp black",
		"bone black",
		"lead white",
		"chalk white",
		"walnut brown",
		"oak-gall black",
		"pomegranate yellow",
		"saffron yellow",
		"henna orange"
	];

	private static readonly string[] TextileFineDyeColours =
	[
		"madder red",
		"kermes scarlet",
		"lac crimson",
		"alkanet purple",
		"orchil violet",
		"tyrian purple",
		"woad blue",
		"pomegranate yellow",
		"saffron yellow",
		"henna orange",
		"walnut brown",
		"oak-gall black"
	];

	[TestMethod]
	public void UsefulTags_DefinePigmentAndDyeCommodityTags()
	{
		var tagSource = ReadSource("DatabaseSeeder", "Assets", "Manifests", "CoreTags.tsv");

		AssertContains(tagSource, "AddTag(context, \"Textile Dye Stock\", \"Textile Commodity\")");
		AssertContains(tagSource, "AddTag(context, \"Lake Pigment\", \"Paint Pigment\")");
	}

	[TestMethod]
	public void MaterialSeeder_AddsAncientPigmentAndDyeInputs()
	{
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder", "CoreDataSeeder.Materials.cs");

		foreach (var expected in new[]
		         {
			         "woad leaves", "weld", "kermes grain", "alkanet root", "henna leaf", "pomegranate rind",
			         "walnut hull", "oak gall", "orchil lichen", "lac dye cake", "murex purple dye",
			         "orpiment", "realgar", "verdigris pigment", "lead white pigment", "red ochre pigment",
			         "yellow ochre pigment", "egyptian blue frit", "bone black pigment"
		         })
		{
			AssertContains(materialSource, expected);
		}

		foreach (var expected in new[]
		         {
			         "EnsureTag(materials[\"saffron\"], \"Textile Dye\")",
			         "EnsureAlias(materials[\"woad leaves\"], \"woad\", \"isatis\")",
			         "EnsureAlias(materials[\"murex purple dye\"], \"tyrian purple\", \"royal purple\", \"murex dye\")",
			         "EnsureAlias(materials[\"soot\"], \"lamp black\", \"carbon black\")"
		         })
		{
			AssertContains(materialSource, expected);
		}
	}

	[TestMethod]
	public void ColourProfiles_AddPigmentAndDyeValues()
	{
		var coreSource = SeederSourceTestHelper.ReadPartialFamily("CoreDataSeeder");

		foreach (var expected in new[] { "Basic_Colours", "Fine_Colours", "Drab_Colours", "Most_Colours" })
		{
			AssertContains(coreSource, $"Name = \"{expected}\"");
		}

		AssertContains(coreSource, "\"olive\"");

		foreach (var expected in NewFinePigmentColours)
		{
			AssertContains(coreSource, $"\"{expected}\"");
		}

		foreach (var expected in new[]
		         {
			         "faded madder red", "dull ochre", "dusty red ochre", "dull egyptian blue",
			         "faded woad blue", "smoky lamp black", "chalky white", "tarnished lead white",
			         "muddy walnut brown", "dull verdigris green", "faded tyrian purple",
			         "stained saffron yellow"
		         })
		{
			AssertContains(coreSource, $"\"{expected}\"");
		}
	}

	[TestMethod]
	public void AntiquityTextileDyeCrafts_UseDyeStockIntermediates()
	{
		Recipes.RequiresTags("Textile Dye Stock");
		foreach (var colour in TextileFineDyeColours) Assert.IsTrue(Recipes.Recipes.SelectMany(recipe => recipe.Products).OfType<CatalogueCommodityRecipeProduct>()
			.Any(product => product.Characteristics.Any(characteristic => characteristic.Definition == "Fine Colour" && characteristic.Value == colour)), colour);
		Recipes.HasRecipes("prepare madder red dye stock", "prepare kermes scarlet dye stock", "prepare tyrian purple dye stock", "prepare oak-gall black dye stock");
	}

	[TestMethod]
	public void AntiquityHouseholdPigmentCrafts_CoverMineralOrganicAndToxicPigments()
	{
		foreach (var colour in NewFinePigmentColours) Assert.IsTrue(Recipes.Recipes.SelectMany(recipe => recipe.Products).OfType<CatalogueCommodityRecipeProduct>()
			.Any(product => product.Characteristics.Any(characteristic => characteristic.Definition == "Fine Colour" && characteristic.Value == colour)), colour);
	}

	[TestMethod]
	public void AntiquityPigmentAndDyeCrafts_EmitProfileBackedColours()
	{
		var coreSource = SeederSourceTestHelper.ReadPartialFamily("CoreDataSeeder");
		var basicColours = new[] { "black", "white", "grey", "light grey", "dark grey", "red", "dark red", "blue", "dark blue", "green", "brown", "dark green", "orange", "light blue", "light green", "yellow", "light red", "purple", "pink", "olive" }.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var emitted = Recipes.Recipes.SelectMany(recipe => recipe.Products).OfType<CatalogueCommodityRecipeProduct>()
			.Where(product => product.Tag is "Paint Pigment" or "Textile Dye Stock").SelectMany(product => product.Characteristics).Where(characteristic => characteristic.Value is not null).ToArray();
		Assert.IsTrue(emitted.Length > 0);
		foreach (var characteristic in emitted)
			if (characteristic.Definition == "Colour") Assert.IsTrue(basicColours.Contains(characteristic.Value!), characteristic.Value);
			else if (characteristic.Definition == "Fine Colour") AssertContains(coreSource, $"\"{characteristic.Value}\"");
	}

	[TestMethod]
	public void AntiquityHouseholdDesignDoc_DescribesPigmentAndDyeSuite()
	{
		var designDoc = ReadSource("Design Documents", "Seeding", "Antiquity_Furniture_Container_Crafting_Suite.md");

		foreach (var expected in new[]
		         {
			         "## Pigment and Dye Suite",
			         "`Textile Dye Stock`",
			         "`Lake Pigment`",
			         "toxic historical pigments",
			         "does not add new poison or material-hazard mechanics"
		         })
		{
			AssertContains(designDoc, expected);
		}
	}

	private static void AssertContains(string source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparison.Ordinal), $"Expected source to contain: {expected}");
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts))));
	}
}
