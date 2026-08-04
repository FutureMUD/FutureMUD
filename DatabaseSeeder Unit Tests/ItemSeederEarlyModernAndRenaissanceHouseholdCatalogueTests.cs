#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederEarlyModernAndRenaissanceHouseholdCatalogueTests
{
	private static readonly string[] ContainmentComponentPrefixes =
		["Container_", "LContainer_", "LockingContainer_", "CashRegister_"];

	private static readonly IReadOnlyDictionary<string, int> ExpectedEarlyModernQualityCounts =
		new Dictionary<string, int>(StringComparer.Ordinal)
		{
			["Poor"] = 15,
			["Substandard"] = 43,
			["Standard"] = 383,
			["Good"] = 414,
			["VeryGood"] = 73,
			["Great"] = 29,
			["Excellent"] = 43
		};

	[TestMethod]
	public void EarlyModernHouseholdCatalogue_HasExactParityFamilyAndQualityAllocation()
	{
		var items = ItemSeeder.EarlyModernHouseholdItemSpecsForTesting;

		Assert.AreEqual(1000, items.Count);
		Assert.AreEqual(520, items.Count(x => x.Family == "Furniture"));
		Assert.AreEqual(480, items.Count(x => x.Family == "ContainerService"));
		CollectionAssert.AreEquivalent(
			ExpectedEarlyModernQualityCounts.ToArray(),
			items.GroupBy(x => x.Quality).ToDictionary(x => x.Key, x => x.Count()).ToArray());
		CollectionAssert.AreEquivalent(
			new[] { "Tiny", "VerySmall", "Small", "Normal", "Large", "VeryLarge", "Huge", "Enormous" },
			items.Select(x => x.Size).Distinct(StringComparer.Ordinal).ToArray());
	}

	[TestMethod]
	public void EarlyModernHouseholdCatalogue_CoversEachCultureWithAtLeastFifteenNewRows()
	{
		const string culturePrefix = "Culture / Early Modern / Shared / ";
		var cultureRows = ItemSeeder.EarlyModernHouseholdItemSpecsForTesting
			.SelectMany(item => item.Tags
				.Where(tag => tag.StartsWith(culturePrefix, StringComparison.Ordinal))
				.Select(tag => tag[culturePrefix.Length..]))
			.GroupBy(tag => tag, StringComparer.Ordinal)
			.ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);

		Assert.AreEqual(36, cultureRows.Count);
		Assert.IsTrue(cultureRows.Values.All(count => count >= 15));
		Assert.AreEqual(975, cultureRows.Values.Sum());
		CollectionAssert.AreEquivalent(
			ReadEarlyModernCultureTags().OrderBy(x => x).ToArray(),
			cultureRows.Keys.OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void HouseholdCatalogues_UseUniqueProductFocusedDescriptionsWithoutRenaissanceBoilerplate()
	{
		var renaissance = ItemSeeder.RenaissanceHouseholdItemSpecsForTesting;
		var earlyModern = ItemSeeder.EarlyModernHouseholdItemSpecsForTesting;

		Assert.AreEqual(1000, renaissance.Count);
		AssertUnique(renaissance.Select(x => x.StableReference), "Renaissance stable reference");
		AssertUnique(renaissance.Select(x => x.ShortDescription), "Renaissance short description");
		AssertUnique(renaissance.Select(x => x.FullDescription), "Renaissance full description");
		AssertUnique(earlyModern.Select(x => x.StableReference), "Early Modern stable reference");
		AssertUnique(earlyModern.Select(x => x.ShortDescription), "Early Modern short description");
		AssertUnique(earlyModern.Select(x => x.FullDescription), "Early Modern full description");

		foreach (var item in renaissance.Select(x => (x.StableReference, x.FullDescription))
			         .Concat(earlyModern.Select(x => (x.StableReference, x.FullDescription))))
		{
			Assert.IsTrue(Regex.IsMatch(item.StableReference, "^[a-z0-9_]+$"), item.StableReference);
			Assert.IsTrue(item.FullDescription.Length >= 160, $"{item.StableReference} needs a substantive full description.");
			Assert.IsTrue(SentenceCount(item.FullDescription) is >= 3 and <= 4,
				$"{item.StableReference} must have three to four physical-detail sentences.");
			Assert.IsFalse(item.FullDescription.Contains(
				"Its construction and fittings follow the documented Renaissance household form",
				StringComparison.Ordinal));
			Assert.IsFalse(item.FullDescription.Contains("generic stock appearance", StringComparison.OrdinalIgnoreCase));
		}
	}

	[TestMethod]
	public void RenaissanceHouseholdDescriptionOverlay_MapsEveryLiveReferenceExactlyOnce()
	{
		var overlayLines = File.ReadLines(Path.Combine(
				SourceRoot(),
				"Design Documents",
				"Seeding",
				"FutureMUD_Renaissance_Household_Description_Overlay.tsv"))
			.ToArray();
		Assert.AreEqual("StableReference\tLongDescription\tFullDescription", overlayLines[0]);
		var overlayRows = overlayLines
			.Skip(1)
			.Select(line => line.Split('\t'))
			.ToArray();
		Assert.IsTrue(overlayRows.All(columns => columns.Length == 3));
		AssertUnique(overlayRows.Select(columns => columns[0]), "Renaissance description-overlay stable reference");
		CollectionAssert.AreEquivalent(
			ItemSeeder.RenaissanceHouseholdItemSpecsForTesting.Select(x => x.StableReference).OrderBy(x => x).ToArray(),
			overlayRows.Select(columns => columns[0]).OrderBy(x => x).ToArray());
		Assert.IsTrue(overlayRows.All(columns => SentenceCount(columns[2]) is >= 3 and <= 4));
	}

	[TestMethod]
	public void HouseholdCatalogues_RespectPortabilityContainmentAndFiniteLiquidRules()
	{
		var earlyModern = ItemSeeder.EarlyModernHouseholdItemSpecsForTesting;
		var renaissance = ItemSeeder.RenaissanceHouseholdItemSpecsForTesting;

		foreach (var item in earlyModern)
		{
			var holdable = item.Components.Contains("Holdable", StringComparer.Ordinal);
			Assert.AreEqual(!holdable, !string.IsNullOrWhiteSpace(item.LongDescription),
				$"{item.StableReference} has an incorrect portable/fixed long-description policy.");
			if (!holdable)
			{
				Assert.AreEqual("Furniture", item.Family, $"{item.StableReference} is fixed but not furniture.");
			}

			AssertContainmentRules(item.StableReference, item.Components);
		}

		foreach (var item in renaissance)
		{
			var holdable = item.Components.Contains("Holdable", StringComparer.Ordinal);
			Assert.AreEqual(!holdable, !string.IsNullOrWhiteSpace(item.LongDescription),
				$"{item.StableReference} has an incorrect portable/fixed long-description policy.");
			AssertContainmentRules(item.StableReference, item.Components);
		}

		foreach (var item in earlyModern.Where(x => x.StableReference.StartsWith("earlymodern_container_", StringComparison.Ordinal) &&
				itemContainsLockLanguage(x.ShortDescription)))
		{
			Assert.IsTrue(item.Components.Any(component => component.StartsWith("LockingContainer_", StringComparison.Ordinal)),
				$"{item.StableReference} advertises a lock without a locking component.");
		}
	}

	[TestMethod]
	public void HouseholdCatalogues_ResolveMaintainedMaterialsComponentsAndTagPaths()
	{
		var materials = ReadExportNames("Seeded_Materials.json", "Material Name");
		var components = ReadExportNames("Seeded_Item_Components.json", "Component Name");
		var tags = File.ReadLines(Path.Combine(SourceRoot(), "Design Documents", "Data", "SeededTagHierarchy.csv"))
			.Skip(1)
			.Select(line => line.Split('\t'))
			.Where(columns => columns.Length == 3)
			.Select(columns => columns[2])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach (var item in ItemSeeder.RenaissanceHouseholdItemSpecsForTesting
			         .Select(x => (x.StableReference, x.Material, x.Tags, x.Components))
			         .Concat(ItemSeeder.EarlyModernHouseholdItemSpecsForTesting
				         .Select(x => (x.StableReference, x.Material, x.Tags, x.Components))))
		{
			Assert.IsTrue(materials.Contains(item.Material), $"{item.StableReference} uses missing material {item.Material}.");
			foreach (var tag in item.Tags)
			{
				Assert.IsTrue(tags.Contains(tag), $"{item.StableReference} uses missing tag {tag}.");
			}

			foreach (var component in item.Components)
			{
				Assert.IsTrue(components.Contains(component), $"{item.StableReference} uses missing component {component}.");
			}
		}
	}

	private static bool itemContainsLockLanguage(string shortDescription)
	{
		return shortDescription.Contains("lock", StringComparison.OrdinalIgnoreCase);
	}

	private static void AssertContainmentRules(string stableReference, IReadOnlyCollection<string> components)
	{
		var containmentProviders = components.Count(component => ContainmentComponentPrefixes.Any(prefix =>
			component.StartsWith(prefix, StringComparison.Ordinal)));
		Assert.IsTrue(containmentProviders <= 1, $"{stableReference} has multiple containment providers.");
		if (components.Any(component => component.StartsWith("LContainer_", StringComparison.Ordinal)))
		{
			Assert.IsFalse(components.Any(component => component.Contains("WaterSource", StringComparison.OrdinalIgnoreCase)),
				$"{stableReference} falsely combines a finite vessel and water source.");
		}
	}

	private static int SentenceCount(string text)
	{
		return text.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;
	}

	private static void AssertUnique(IEnumerable<string> values, string label)
	{
		var duplicates = values
			.GroupBy(value => value, StringComparer.OrdinalIgnoreCase)
			.Where(group => group.Count() > 1)
			.Select(group => group.Key)
			.ToArray();
		Assert.AreEqual(0, duplicates.Length, $"Duplicate {label}: {string.Join(", ", duplicates)}");
	}

	private static HashSet<string> ReadEarlyModernCultureTags()
	{
		const string prefix = "Culture / Early Modern / Shared / ";
		return File.ReadLines(Path.Combine(SourceRoot(), "Design Documents", "Data", "SeededTagHierarchy.csv"))
			.Skip(1)
			.Select(line => line.Split('\t'))
			.Where(columns => columns.Length == 3 && columns[2].StartsWith(prefix, StringComparison.Ordinal))
			.Select(columns => columns[2][prefix.Length..])
			.ToHashSet(StringComparer.Ordinal);
	}

	private static HashSet<string> ReadExportNames(string fileName, string propertyName)
	{
		using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(SourceRoot(), "Design Documents", "Data", fileName)));
		return document.RootElement
			.EnumerateArray()
			.Select(item => item.GetProperty(propertyName).GetString())
			.Where(value => !string.IsNullOrWhiteSpace(value))
			.Select(value => value!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static string SourceRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate repository root from test output path.");
		return directory.FullName;
	}
}
