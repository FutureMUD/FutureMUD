#nullable enable

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
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");

		AssertContains(tagSource, "AddTag(context, \"Textile Dye Stock\", \"Textile Commodity\")");
		AssertContains(tagSource, "AddTag(context, \"Lake Pigment\", \"Paint Pigment\")");
	}

	[TestMethod]
	public void MaterialSeeder_AddsAncientPigmentAndDyeInputs()
	{
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

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
		var coreSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.cs");

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
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");

		foreach (var expected in new[]
		         {
			         "AddTextileDyeStockCraft(",
			         "tag Textile Dye Stock",
			         "piletag Textile Dye Stock",
			         "prepare madder red dye stock",
			         "prepare kermes scarlet dye stock",
			         "prepare tyrian purple dye stock",
			         "prepare oak-gall black dye stock",
			         "dye wool cloth kermes scarlet",
			         "dye linen cloth pomegranate yellow",
			         "dye cotton cloth henna orange",
			         "dye felt cloth oak-gall black"
		         })
		{
			AssertContains(craftSource, expected);
		}

		foreach (var expected in TextileFineDyeColours)
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityHouseholdPigmentCrafts_CoverMineralOrganicAndToxicPigments()
	{
		var householdSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityHousehold.cs");

		foreach (var expected in new[]
		         {
			         "prepare lamp black paint pigment",
			         "prepare lead white paint pigment",
			         "prepare cinnabar red paint pigment",
			         "prepare orpiment yellow paint pigment",
			         "prepare realgar orange paint pigment",
			         "prepare malachite green paint pigment",
			         "prepare verdigris green paint pigment",
			         "prepare egyptian blue paint pigment",
			         "prepare madder lake pigment",
			         "prepare kermes lake pigment",
			         "prepare indigo lake pigment",
			         "prepare woad lake pigment",
			         "prepare saffron lake pigment",
			         "prepare tyrian purple lake pigment",
			         "tag Lake Pigment",
			         "tag Paint Pigment",
			         "CommodityPileInput(180.0, \"Paint Pigment\", colour: true, fineColour: true)"
		         })
		{
			AssertContains(householdSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityPigmentAndDyeCrafts_EmitProfileBackedColours()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var householdSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityHousehold.cs");
		var coreSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.cs");
		var source = $"{craftSource}\n{householdSource}";
		var basicColours = new[]
			{
				"black", "white", "grey", "light grey", "dark grey", "red", "dark red", "blue", "dark blue",
				"green", "brown", "dark green", "orange", "light blue", "light green", "yellow", "light red",
				"purple", "pink", "olive"
			}
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		var emitted = Regex.Matches(
				source,
				@"Add(?:TextileDyeStockCraft|PaintPigmentCraft)\(""[^""]+"", ""[^""]+"", ""(?<basic>[^""]+)"", ""(?<fine>[^""]+)""",
				RegexOptions.Singleline)
			.Cast<Match>()
			.Concat(Regex.Matches(
					source,
					@"AddDyeCraft\(""[^""]+"", ""[^""]+"", ""[^""]+"", ""(?<basic>[^""]+)"", ""(?<fine>[^""]+)""",
					RegexOptions.Singleline)
				.Cast<Match>())
			.Select(x => (Basic: x.Groups["basic"].Value, Fine: x.Groups["fine"].Value))
			.ToList();

		Assert.IsTrue(emitted.Count > 0, "Expected to find emitted colour arguments in pigment and dye crafts.");

		foreach (var (basic, fine) in emitted)
		{
			Assert.IsTrue(basicColours.Contains(basic), $"Basic colour '{basic}' is not in the seeded Basic_Colours profile.");
			AssertContains(coreSource, $"\"{fine}\"");
		}
	}

	[TestMethod]
	public void AntiquityHouseholdDesignDoc_DescribesPigmentAndDyeSuite()
	{
		var designDoc = ReadSource("Design Documents", "Crafting", "Antiquity_Furniture_Container_Crafting_Suite.md");

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
