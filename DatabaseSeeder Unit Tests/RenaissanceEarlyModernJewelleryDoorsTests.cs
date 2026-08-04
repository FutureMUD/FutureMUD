#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RenaissanceEarlyModernJewelleryDoorsTests
{
	private sealed record CatalogueContract(string FileName, int ExpectedRows);

	private static readonly CatalogueContract[] CatalogueContracts =
	[
		new("FutureMUD_PreIndustrial_Jewellery_Doors_Item_Catalogue.csv", 150),
		new("FutureMUD_Renaissance_Jewellery_Devotional_Item_Catalogue.csv", 940),
		new("FutureMUD_Renaissance_Doors_Locks_Gates_Item_Catalogue.csv", 910),
		new("FutureMUD_EarlyModern_Jewellery_Devotional_Item_Catalogue.csv", 720),
		new("FutureMUD_EarlyModern_Doors_Locks_Gates_Item_Catalogue.csv", 670)
	];

	[TestMethod]
	public void CataloguesMeetTheCommittedAvailabilityAndReferenceContract()
	{
		var allReferences = new List<string>();
		foreach (var contract in CatalogueContracts)
		{
			var references = ReadCatalogueReferences(contract.FileName);
			Assert.AreEqual(contract.ExpectedRows, references.Count,
				$"Unexpected row count for {contract.FileName}.");
			Assert.AreEqual(references.Count, references.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
				$"Duplicate stable reference in {contract.FileName}.");
			allReferences.AddRange(references);
		}

		Assert.AreEqual(3390, allReferences.Count);
		Assert.AreEqual(3390, allReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(allReferences.All(reference => Regex.IsMatch(reference,
			"^(?:preindustrial|renaissance|earlymodern)_(?:jewellery|door)_[a-z0-9_]+$")),
			"Catalogue references must stay lowercase, product-focused, and free of cultural display labels.");
		Assert.IsFalse(allReferences.Any(reference => reference.Contains("_pass_", StringComparison.OrdinalIgnoreCase) ||
			reference.Contains("_expansion_", StringComparison.OrdinalIgnoreCase) ||
			reference.Contains("_rework_", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void GeneratedCreateItemCallsExactlyMatchTheCanonicalCatalogues()
	{
		var expected = CatalogueContracts
			.SelectMany(contract => ReadCatalogueReferences(contract.FileName))
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToArray();
		var actual = GeneratedSeederFiles()
			.SelectMany(File.ReadLines)
			.SelectMany(line => Regex.Matches(line,
				"^\\s*\\\"((?:preindustrial|renaissance|earlymodern)_(?:jewellery|door)_[a-z0-9_]+)\\\",$",
				RegexOptions.IgnoreCase).Select(match => match.Groups[1].Value))
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToArray();

		CollectionAssert.AreEqual(expected, actual,
			"Every catalogue row must have one literal generated CreateItem call, with no undocumented prototype.");
	}

	[TestMethod]
	public void CulturalCoverageIncludesEveryContractedRenaissanceAndEarlyModernFamily()
	{
		var renaissanceJewelleryCultures = CultureCodes(ReadCatalogueReferences(
			"FutureMUD_Renaissance_Jewellery_Devotional_Item_Catalogue.csv"), "renaissance_jewellery_");
		var renaissanceDoorCultures = CultureCodes(ReadCatalogueReferences(
			"FutureMUD_Renaissance_Doors_Locks_Gates_Item_Catalogue.csv"), "renaissance_door_");
		var earlyModernJewelleryCultures = CultureCodes(ReadCatalogueReferences(
			"FutureMUD_EarlyModern_Jewellery_Devotional_Item_Catalogue.csv"), "earlymodern_jewellery_");
		var earlyModernDoorCultures = CultureCodes(ReadCatalogueReferences(
			"FutureMUD_EarlyModern_Doors_Locks_Gates_Item_Catalogue.csv"), "earlymodern_door_");

		Assert.AreEqual(24, renaissanceJewelleryCultures.Count);
		Assert.AreEqual(24, renaissanceDoorCultures.Count);
		Assert.AreEqual(36, earlyModernJewelleryCultures.Count);
		Assert.AreEqual(36, earlyModernDoorCultures.Count);
		StringAssert.Contains(ReadSource("Design Documents", "Seeding",
			"FutureMUD_Renaissance_EarlyModern_Jewellery_Doors_Admission_Ledger.md"),
			"Renaissance-owned common | 220 | 240 | Renaissance, Early Modern");
	}

	[TestMethod]
	public void GeneratedDependencyContractIsSelfConsistentAndReportsMissingPrerequisites()
	{
		var materials = ItemSeeder.RenaissanceEarlyModernJewelleryDoorsMaterialsForTesting;
		var tags = ItemSeeder.RenaissanceEarlyModernJewelleryDoorsTagsForTesting;
		var components = ItemSeeder.RenaissanceEarlyModernJewelleryDoorsComponentsForTesting;
		Assert.AreEqual(0, ItemSeeder.ValidateRenaissanceEarlyModernJewelleryDoorsDependenciesForTesting(
			materials, tags, components).Count);

		var missingMaterialIssues = ItemSeeder.ValidateRenaissanceEarlyModernJewelleryDoorsDependenciesForTesting(
			materials.Where(x => !x.Equals("gold", StringComparison.OrdinalIgnoreCase)), tags, components);
		CollectionAssert.Contains(missingMaterialIssues.ToArray(), "Missing material: gold");
	}

	[TestMethod]
	public void MaintainedCoreCataloguesContainEveryGeneratedDependency()
	{
		using var materialsDocument = JsonDocument.Parse(ReadSource("Design Documents", "Data", "Seeded_Materials.json"));
		var materials = materialsDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Material Name").GetString()!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		using var componentsDocument = JsonDocument.Parse(ReadSource("Design Documents", "Data", "Seeded_Item_Components.json"));
		var components = componentsDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Component Name").GetString()!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var tags = File.ReadLines(SourcePath("Design Documents", "Data", "SeededTagHierarchy.csv"))
			.Skip(1)
			.Select(line => line.Split('\t'))
			.Where(parts => parts.Length >= 3)
			.Select(parts => parts[2])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		CollectionAssert.IsSubsetOf(ItemSeeder.RenaissanceEarlyModernJewelleryDoorsMaterialsForTesting.ToArray(),
			materials.ToArray());
		CollectionAssert.IsSubsetOf(ItemSeeder.RenaissanceEarlyModernJewelleryDoorsComponentsForTesting.ToArray(),
			components.ToArray());
		CollectionAssert.IsSubsetOf(ItemSeeder.RenaissanceEarlyModernJewelleryDoorsTagsForTesting.ToArray(),
			tags.ToArray());
	}

	[TestMethod]
	public void CraftLayerCoversSharedAndEraOwnedRowsWithProductFocusedWorkshopRoutes()
	{
		var source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs");
		foreach (var prefix in new[]
		         {
				 "preindustrial_jewellery_", "preindustrial_door_", "renaissance_jewellery_", "renaissance_door_",
				 "earlymodern_jewellery_", "earlymodern_door_"
		         })
		{
			StringAssert.Contains(source, prefix);
		}

		StringAssert.Contains(source, "an in-progress {StripLeadingArticle(displayName)} craft");
		StringAssert.Contains(source, "Renaissance and Early Modern Jewellery");
		StringAssert.Contains(source, "Renaissance and Early Modern Joinery");
		StringAssert.Contains(source, "Renaissance and Early Modern Locksmithing");
		StringAssert.Contains(source, "SimpleProduct - 1x {displayName} (#{item.Id})");
		StringAssert.Contains(source, "RenaissanceEarlyModernJewelleryDoorsCraftNamesByStableReference");
		Assert.AreEqual(3390, ItemSeeder.RenaissanceEarlyModernJewelleryDoorsCraftNamesForTesting.Count);
		Assert.AreEqual(3390, ItemSeeder.RenaissanceEarlyModernJewelleryDoorsCraftNamesForTesting.Values
			.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		foreach (var trait in new[] { "Silversmithing", "Glassworking", "Scrimshawing", "Gemcraft", "Carpentry", "Weaving" })
		{
			StringAssert.Contains(source, trait);
		}
		Assert.IsTrue(ItemSeeder.ShouldSeedRenaissanceEarlyModernJewelleryDoorCraftsForTesting("earlymodern"));
		Assert.IsTrue(ItemSeeder.ShouldSeedRenaissanceEarlyModernJewelleryDoorCraftsForTesting("medieval"));
		Assert.IsFalse(ItemSeeder.ShouldSeedRenaissanceEarlyModernJewelleryDoorCraftsForTesting("industrial"));
	}

	private static IReadOnlyList<string> ReadCatalogueReferences(string fileName)
	{
		var path = SourcePath("Design Documents", "Seeding", fileName);
		Assert.IsTrue(File.Exists(path), $"Missing canonical catalogue {fileName}.");
		return File.ReadLines(path)
			.Skip(1)
			.Select(line => line[..line.IndexOf(',')])
			.ToArray();
	}

	private static IReadOnlyCollection<string> CultureCodes(IEnumerable<string> references, string prefix)
	{
		return references
			.Where(reference => reference.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			.Select(reference => reference[prefix.Length..].Split('_')[0])
			.Where(code => !code.Equals("common", StringComparison.OrdinalIgnoreCase))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private static IEnumerable<string> GeneratedSeederFiles()
	{
		return Directory.GetFiles(SourcePath("DatabaseSeeder", "Seeders"),
			"ItemSeeder.*Jewellery*Generated.cs")
			.Concat(Directory.GetFiles(SourcePath("DatabaseSeeder", "Seeders"),
				"ItemSeeder.*Doors*Generated.cs"))
			.Distinct(StringComparer.OrdinalIgnoreCase);
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(SourcePath(parts));
	}

	private static string SourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
