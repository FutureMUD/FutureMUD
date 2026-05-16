#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederAntiquityRemainingCraftingTests
{
	private static readonly string[] AntiquityReworkMethods =
	[
		"SeedAntiquityClothing",
		"SeedAntiquityJewellery",
		"SeedAntiquityArmour",
		"SeedAntiquityContainers",
		"SeedAntiquityDoorsAndLocks",
		"SeedAntiquityHouseholdFurniture",
		"SeedAntiquityWeaponsShieldsAccessories"
	];

	private static readonly string[] HouseholdCraftRoots =
	[
		"Market / Household Goods",
		"Market / Writing Materials",
		"Market / Religious Goods",
		"Market / Lighting",
		"Market / Domestic Heating",
		"Market / Construction Materials",
		"Materials / Writing Product"
	];

	[TestMethod]
	public void AntiquityRemainingCrafts_SourceAuditCoversAllCurrentTargets()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");
		var existingCraftSource =
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs") +
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs") +
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityHousehold.cs");
		var equipmentCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");
		var allCraftSource = existingCraftSource + equipmentCraftSource;

		var items = AntiquityReworkMethods
			.SelectMany(method => ParseItemsInMethod(itemSource, method))
			.ToList();

		Assert.AreEqual(995, items.Count, "The audit should track the current antiquity item catalogue.");
		Assert.AreEqual(29, items.Count(IsAntiquityCraftToolTarget));
		Assert.AreEqual(18, items.Count(x => equipmentCraftSource.Contains($"\"{x.StableReference}\"", StringComparison.Ordinal) &&
		                                     x.MethodName.Equals("SeedAntiquityClothing", StringComparison.Ordinal) &&
		                                     !IsAntiquityCraftToolTarget(x)));
		Assert.AreEqual(193, items.Count(x => IsMilitaryEquipmentTarget(x, existingCraftSource)));
		Assert.AreEqual(398, items.Count(IsHouseholdDynamicTarget));

		var uncovered = items
			.Where(item => !IsCoveredByCraftSuites(item, existingCraftSource, equipmentCraftSource, allCraftSource))
			.Select(item => $"{item.MethodName}:{item.StableReference}")
			.ToList();

		Assert.AreEqual(0, uncovered.Count,
			$"Expected every antiquity prototype to be covered by an existing or new craft suite. Missing: {string.Join(", ", uncovered)}");
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_RegisterKnowledgeGatedSuitesAndCorrectAccessProgs()
	{
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var equipmentCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");

		AssertContains(craftRoot, "SeedAntiquityEquipmentCrafts();");
		AssertContains(craftRoot, "AddProg(\"HasWeaponcrafting\"");
		AssertContains(craftRoot, "_traits[\"Weaponcrafting\"]?.Id ?? _traits[\"Weaponsmith\"]?.Id");
		AssertContains(craftRoot, "AddProg(\"HasArmourcrafting\"");
		AssertContains(craftRoot, "_traits[\"Armourcrafting\"]?.Id ?? _traits[\"Armourer\"]?.Id");

		foreach (var expected in new[]
		{
			"private void SeedAntiquityEquipmentCrafts()",
			"SeedAntiquityEquipmentIntermediateCommodityCrafts();",
			"Ancient Equipment Crafting",
			"Ancient Weaponcrafting",
			"Ancient Armourcrafting",
			"Ancient Toolmaking",
			"Ancient Common Clothing Crafting",
			"AddAntiquityCraft(",
			"knowledgeSubtype:",
			"knowledgeDescription:",
			"Market / Military Goods"
		})
		{
			AssertContains(equipmentCraftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_AddUpstreamCommodityAndTagSurface()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var tagHierarchy = ReadSource("Design Documents", "SeededTagHierarchy.csv");

		foreach (var expected in ExpectedEquipmentStockTags())
		{
			AssertContains(craftSource, expected);
			AssertContains(tagSource, $"AddTag(context, \"{expected}\",");
			AssertContains(tagHierarchy, $"Antiquity Equipment Stock / {expected}");
		}

		foreach (var expected in new[]
		{
			"CommodityProduct -",
			"Weapon Blade Stock",
			"Weapon Head Stock",
			"Weapon Shaft Stock",
			"Bow Stave",
			"Fletching Stock",
			"Shield Board Stock",
			"Shield Facing Stock",
			"Helmet Bowl Stock",
			"Armour Plate Stock",
			"Armour Ring Stock",
			"Armour Scale Stock",
			"Armour Lamella Stock",
			"Armour Textile Padding",
			"Tool Blank Stock",
			"Door Hardware Stock"
		})
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_KeepVisibleCraftStringsCultureNeutral()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");

		AssertContains(craftSource, "SanitiseAntiquityEquipmentVisibleName(item.ShortDescription)");
		AssertContains(craftSource, "BuildUniqueAntiquityEquipmentCraftName");
		Assert.IsFalse(craftSource.Contains("stableReference[\"antiquity_\"", StringComparison.Ordinal),
			"Visible equipment craft names should not be generated from stable references because those may contain culture names.");

		var visibleConstructionRegion = ExtractMethodBody(craftSource, "SeedAntiquityEquipmentFinishedCraft");
		foreach (var banned in new[] { "roman", "hellenic", "egyptian", "celtic", "germanic", "kushite", "punic", "persian", "etruscan", "anatolian", "scythian", "sarmatian" })
		{
			Assert.IsFalse(visibleConstructionRegion.Contains(banned, StringComparison.OrdinalIgnoreCase),
				$"Visible equipment craft construction should not contain culture term {banned}.");
		}
	}

	private static bool IsCoveredByCraftSuites(SeededAntiquityItem item, string existingCraftSource,
		string equipmentCraftSource, string allCraftSource)
	{
		return item.MethodName.Equals("SeedAntiquityJewellery", StringComparison.Ordinal) ||
		       existingCraftSource.Contains($"\"{item.StableReference}\"", StringComparison.Ordinal) ||
		       IsHouseholdDynamicTarget(item) ||
		       equipmentCraftSource.Contains($"\"{item.StableReference}\"", StringComparison.Ordinal) ||
		       IsMilitaryEquipmentTarget(item, existingCraftSource) ||
		       allCraftSource.Contains($"\"{item.StableReference}\"", StringComparison.Ordinal);
	}

	private static bool IsHouseholdDynamicTarget(SeededAntiquityItem item)
	{
		return item.MethodName is "SeedAntiquityContainers" or "SeedAntiquityDoorsAndLocks" or "SeedAntiquityHouseholdFurniture" &&
		       HasAnyRoot(item.Tags, HouseholdCraftRoots);
	}

	private static bool IsMilitaryEquipmentTarget(SeededAntiquityItem item, string existingCraftSource)
	{
		return HasRoot(item.Tags, "Market / Military Goods") &&
		       !existingCraftSource.Contains($"\"{item.StableReference}\"", StringComparison.Ordinal);
	}

	private static bool IsAntiquityCraftToolTarget(SeededAntiquityItem item)
	{
		return item.MethodName.Equals("SeedAntiquityClothing", StringComparison.Ordinal) &&
		       HasRoot(item.Tags, "Market / Professional Tools / Standard Tools");
	}

	private static IEnumerable<string> ExpectedEquipmentStockTags()
	{
		return
		[
			"Weapon Blade Stock",
			"Weapon Head Stock",
			"Weapon Shaft Stock",
			"Bow Stave",
			"Fletching Stock",
			"Military Cord Stock",
			"Shield Board Stock",
			"Shield Facing Stock",
			"Helmet Bowl Stock",
			"Armour Plate Stock",
			"Armour Ring Stock",
			"Armour Scale Stock",
			"Armour Lamella Stock",
			"Armour Textile Padding",
			"Tool Blank Stock",
			"Door Hardware Stock"
		];
	}

	private static List<SeededAntiquityItem> ParseItemsInMethod(string source, string methodName)
	{
		var body = ExtractMethodBody(source, methodName);
		var matches = Regex.Matches(body,
			@"CreateItem\s*\(\s*""(?<ref>[^""]+)""\s*,\s*""[^""]*""\s*,\s*""(?<short>[^""]*)""[\s\S]*?\n\s*""(?<material>[^""]+)""\s*,\s*\n\s*\[(?<tags>[\s\S]*?)\]\s*,",
			RegexOptions.Multiline);

		return matches
			.Select(match => new SeededAntiquityItem(
				methodName,
				match.Groups["ref"].Value,
				match.Groups["short"].Value,
				match.Groups["material"].Value,
				Regex.Matches(match.Groups["tags"].Value, @"""(?<tag>[^""]+)""")
					.Select(tagMatch => tagMatch.Groups["tag"].Value)
					.ToList()))
			.ToList();
	}

	private static bool HasAnyRoot(IEnumerable<string> tags, IEnumerable<string> roots)
	{
		return roots.Any(root => HasRoot(tags, root));
	}

	private static bool HasRoot(IEnumerable<string> tags, string root)
	{
		return tags.Any(tag => tag.Equals(root, StringComparison.OrdinalIgnoreCase) ||
		                       tag.StartsWith($"{root} /", StringComparison.OrdinalIgnoreCase));
	}

	private static string ExtractMethodBody(string source, string methodName)
	{
		var marker = $"private void {methodName}(";
		var start = source.IndexOf(marker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find method {methodName}.");
		var openBrace = source.IndexOf('{', start);
		Assert.IsTrue(openBrace >= 0, $"Could not find body for method {methodName}.");

		var depth = 0;
		for (var i = openBrace; i < source.Length; i++)
		{
			switch (source[i])
			{
				case '{':
					depth++;
					break;
				case '}':
					depth--;
					if (depth == 0)
					{
						return source[(openBrace + 1)..i];
					}
					break;
			}
		}

		Assert.Fail($"Could not extract body for method {methodName}.");
		return string.Empty;
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

	private sealed record SeededAntiquityItem(
		string MethodName,
		string StableReference,
		string ShortDescription,
		string Material,
		IReadOnlyList<string> Tags);
}
