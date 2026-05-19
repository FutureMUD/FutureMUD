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
		var tagHierarchy = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv");

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

	[TestMethod]
	public void AntiquityCrafting_AllCurrentTagToolsHaveSeededItemCoverage()
	{
		var craftSource = ReadSeederSources("ItemSeederCrafting.Antiquity*.cs");
		var itemSource = ReadSeederSources("ItemSeeder.Rework.Antiquity*.cs");

		var toolTags = Regex.Matches(craftSource,
				@"TagTool\s*-\s*(?:Held|InRoom|In Room|Wielded)\s*-\s*an item with the (?<tag>.+?) tag",
				RegexOptions.IgnoreCase)
			.Select(x => x.Groups["tag"].Value.Trim())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		var seededToolTagLeafs = Regex.Matches(itemSource, @"""(?<tag>Functions / [^""]+)""")
			.Select(x => x.Groups["tag"].Value.Split(" / ").Last().Trim())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		var missing = toolTags
			.Where(x => !seededToolTagLeafs.Contains(x))
			.ToList();

		Assert.AreEqual(0, missing.Count,
			$"Every current antiquity TagTool should have at least one seeded rework item carrying the exact tool tag. Missing: {string.Join(", ", missing)}");
	}

	[TestMethod]
	public void AntiquityCrafting_LitWorkshopItemsHaveMorphTargetsTimersAndToolTags()
	{
		var createItemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var householdToolSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityHouseholdTools.cs");
		var equipmentCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");

		foreach (var expected in new[]
		{
			"item.MorphGameItemProtoId = morphItem.Id;",
			"item.MorphTimeSeconds = (int)morphTimer.Value.TotalSeconds;",
			"item.MorphEmote = morphEmote;",
			"item.OnDestroyedGameItemProtoId = destroyedItem.Id;"
		})
		{
			AssertContains(createItemSource, expected);
		}

		foreach (var spec in new[]
		{
			(Lit: "antiquity_lit_workshop_hearth", Unlit: "antiquity_workshop_hearth", ToolTag: "Functions / Material Functions / Fire"),
			(Lit: "antiquity_lit_clay_smelting_furnace", Unlit: "antiquity_clay_smelting_furnace", ToolTag: "Functions / Tools / Smelting Tools / Smelting Furnace / Lit Smelting Furnace"),
			(Lit: "antiquity_lit_updraft_kiln", Unlit: "antiquity_updraft_kiln", ToolTag: "Functions / Tools / Pottery Tools / Kiln / Lit Kiln"),
			(Lit: "antiquity_lit_annealing_lehr", Unlit: "antiquity_annealing_lehr", ToolTag: "Functions / Tools / Glassblowing Tools / Annealing Lehr / Lit Annealing Lehr")
		})
		{
			var block = ExtractCallBlockContaining(householdToolSource, spec.Lit);
			AssertContains(block, spec.Unlit);
			AssertContains(block, spec.ToolTag);
			AssertContains(block, "TimeSpan.FromHours(");
		}

		foreach (var expected in new[]
		{
			"SeedAntiquityWorkshopHeatSourceCrafts();",
			"light a workshop hearth",
			"stoke an updraft pottery kiln",
			"stoke a clay smelting furnace",
			"stoke an annealing lehr",
			"CommodityInput(charcoalGrams, \"charcoal\")",
			"lay|lays $i2 into the fire bed",
			"StableUnusedInputProduct(unlitStableReference, 1)"
		})
		{
			AssertContains(equipmentCraftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityCrafting_ProducedIntermediateTagsAreConsumedOrDocumentedReusableStock()
	{
		var craftSource = ReadSeederSources("ItemSeederCrafting.Antiquity*.cs");
		var docsSource =
			ReadSource("Design Documents", "Crafting", "Antiquity_Equipment_Crafting_Suite.md") +
			ReadSource("Design Documents", "Crafting", "Antiquity_Furniture_Container_Crafting_Suite.md") +
			ReadSource("Design Documents", "Crafting", "Antiquity_Writing_Implements_Crafting_Suite.md") +
			ReadSource("Design Documents", "Crafting", "Antiquity_Medical_Crafting_Suite.md");

		var producedTags = ExtractLiteralCommodityProductTags(craftSource);
		var consumedTags = ExtractLiteralCommodityInputTags(craftSource);

		var undocumentedReusableOutputs = producedTags
			.Where(x => !consumedTags.Contains(x))
			.Where(x => !docsSource.Contains($"`{x}`", StringComparison.Ordinal))
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		Assert.AreEqual(0, undocumentedReusableOutputs.Count,
			$"Produced intermediate tags should be consumed downstream or documented as reusable stock outputs. Missing docs: {string.Join(", ", undocumentedReusableOutputs)}");
	}

	[TestMethod]
	public void AntiquityCrafting_DocumentationMentionsEveryCraftSourceStableReference()
	{
		var craftSource = ReadSeederSources("ItemSeederCrafting.Antiquity*.cs");
		var docsSource = ReadDesignDocumentSources("Antiquity_*.md");

		var craftStableReferences = ExtractStableReferences(craftSource)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();
		var documentedStableReferences = ExtractStableReferences(docsSource);

		var missing = craftStableReferences
			.Where(x => !documentedStableReferences.Contains(x))
			.ToList();

		Assert.AreEqual(0, missing.Count,
			$"Every stable reference explicitly named by current antiquity craft source should be catalogued in the antiquity docs. Missing: {string.Join(", ", missing)}");
	}

	[TestMethod]
	public void AntiquityMedicalCookingPotCrafts_RequireLitFireState()
	{
		var medicalCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityMedical.cs");

		foreach (var craftName in new[]
		{
			"prepare clean dressing stock",
			"render herbal salve stock",
			"steep medicinal decoction stock"
		})
		{
			var block = ExtractCallBlockContaining(medicalCraftSource, craftName);
			AssertContains(block, "TagTool - InRoom - an item with the Cooking Pot tag");
			AssertContains(block, "TagTool - InRoom - an item with the Fire tag");
		}
	}

	[TestMethod]
	public void AntiquityCrafting_CatalogueAuditDocumentsRemainingLogicalGaps()
	{
		var auditDoc = ReadSource("Design Documents", "Crafting", "Antiquity_Crafting_Audit.md");

		foreach (var expected in new[]
		{
			"SeedAntiquityJewellery",
			"support tools and unlit workshop apparatus",
			"glass furnace",
			"maintained rather than generated"
		})
		{
			AssertContains(auditDoc, expected);
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

	private static string ExtractCallBlockContaining(string source, string marker)
	{
		var start = source.IndexOf(marker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find source block for {marker}.");
		var end = source.IndexOf(");", start, StringComparison.Ordinal);
		Assert.IsTrue(end >= 0, $"Could not find end of source block for {marker}.");
		return source[start..end];
	}

	private static HashSet<string> ExtractLiteralCommodityProductTags(string source)
	{
		return Regex.Matches(source, @"CommodityProduct\s*-\s*[^""\]]*?;\s*tag\s+(?<tag>[^;""\]\}]+)")
			.Select(x => x.Groups["tag"].Value.Trim())
			.Where(IsLiteralTagReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static HashSet<string> ExtractLiteralCommodityInputTags(string source)
	{
		var pileTags = Regex.Matches(source, @"piletag\s+(?<tag>[^;""\]\)]+)")
			.Select(x => x.Groups["tag"].Value.Trim());
		var helperTags = Regex.Matches(source, @"CommodityInput\([^,\)]*,\s*""[^""]+""\s*,\s*""(?<tag>[^""]+)""")
			.Select(x => x.Groups["tag"].Value.Trim());

		return pileTags
			.Concat(helperTags)
			.Where(IsLiteralTagReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsLiteralTagReference(string tag)
	{
		return !string.IsNullOrWhiteSpace(tag) &&
		       !tag.Contains('{', StringComparison.Ordinal) &&
		       !tag.Contains('$', StringComparison.Ordinal);
	}

	private static HashSet<string> ExtractStableReferences(string source)
	{
		return Regex.Matches(source, @"\b(?:antiquity|adjacent_antiquity|jewellery)_[a-z0-9_]+\b")
			.Select(x => x.Value)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static string ReadSeederSources(string pattern)
	{
		var sourceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
		return string.Concat(Directory
			.EnumerateFiles(Path.Combine(sourceRoot, "DatabaseSeeder", "Seeders"), pattern)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Select(File.ReadAllText));
	}

	private static string ReadDesignDocumentSources(string pattern)
	{
		var sourceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
		return string.Concat(Directory
			.EnumerateFiles(Path.Combine(sourceRoot, "Design Documents", "Crafting"), pattern)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Select(File.ReadAllText));
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
