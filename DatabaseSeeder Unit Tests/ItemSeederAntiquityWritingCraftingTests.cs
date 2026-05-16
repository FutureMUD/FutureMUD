#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederAntiquityWritingCraftingTests
{
	private static readonly string[] ExpectedWritingStableReferences =
	[
		"antiquity_loose_papyrus_sheet",
		"antiquity_papyrus_sheet_bundle",
		"antiquity_simple_papyrus_scroll",
		"antiquity_sealed_papyrus_scroll",
		"antiquity_loose_parchment_sheet",
		"antiquity_parchment_bifolium",
		"antiquity_parchment_quire",
		"antiquity_parchment_scroll",
		"antiquity_parchment_codex",
		"antiquity_wax_writing_tablet",
		"antiquity_hinged_wax_diptych",
		"antiquity_wax_triptych",
		"antiquity_unfired_clay_tablet",
		"antiquity_fired_clay_tablet",
		"antiquity_clay_tablet_envelope",
		"antiquity_smoothed_wooden_writing_block",
		"antiquity_potsherd_ostracon",
		"antiquity_reed_pen",
		"antiquity_quill_pen",
		"antiquity_ink_brush",
		"antiquity_charcoal_writing_stick",
		"antiquity_bone_stylus",
		"antiquity_bronze_stylus",
		"antiquity_reed_stylus",
		"antiquity_bronze_pen_knife",
		"antiquity_bronze_scraper_knife",
		"antiquity_soot_ink_cake",
		"antiquity_small_inkwell",
		"antiquity_liquid_black_ink_pot",
		"antiquity_cedar_scroll_case",
		"antiquity_leather_codex_pouch",
		"antiquity_linen_tablet_wrap",
		"antiquity_papyrus_scroll_tie"
	];

	[TestMethod]
	public void AntiquityWritingItems_AreSeededWithFunctionalComponentAndTagCoverage()
	{
		var rootSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.cs");
		var reworkRootSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityWriting.cs");

		AssertContains(rootSource, "SeedReworkItems();");
		AssertContains(reworkRootSource, "SeedAntiquityWritingImplementsAndDocuments();");

		foreach (var stableReference in ExpectedWritingStableReferences)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
		}

		foreach (var expected in new[]
		{
			"EnsureAntiquityScribingImplementComponent",
			"EnsureAntiquityInscribableSurfaceComponent",
			"EnsureAntiquityPaperSheetComponent",
			"EnsureAntiquityBookComponent",
			"\"ScribingImplement\"",
			"\"InscribableSurface\"",
			"\"PaperSheet\"",
			"\"Book\"",
			"WritingImplementType.ReedPen",
			"WritingImplementType.Charcoal",
			"WritingImplementType.Stylus"
		})
		{
			AssertContains(itemSource, expected);
		}

		foreach (var expected in new[]
		{
			"Functions / Writing Surface / Loose Sheet",
			"Functions / Writing Surface / Scroll",
			"Functions / Writing Surface / Codex",
			"Functions / Writing Surface / Wax Tablet",
			"Functions / Writing Surface / Clay Tablet",
			"Functions / Writing Surface / Wooden Writing Block",
			"Functions / Writing Surface / Ostracon",
			"Market / Writing Materials / Writing Implements",
			"Market / Writing Materials / Document Containers"
		})
		{
			AssertContains(itemSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityWritingCrafts_RegisterKnowledgeGatedSuiteAndNoLegacyLiteracyProgs()
	{
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityWriting.cs");
		var householdCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityHousehold.cs");

		AssertContains(craftRoot, "SeedAntiquityWritingCrafts();");
		AssertContains(householdCraftSource, "IsAntiquityWritingSuiteStableReference");

		foreach (var expected in new[]
		{
			"private void SeedAntiquityWritingCrafts()",
			"SeedAntiquityWritingIntermediateCommodityCrafts();",
			"AddAntiquityCraft(",
			"Ancient Papyrus Making",
			"Ancient Parchment Making",
			"Ancient Tablet Making",
			"Ancient Ink and Pigment Making",
			"Ancient Scribing Implements",
			"Ancient Scroll and Codex Binding",
			"knowledgeSubtype:",
			"knowledgeDescription:"
		})
		{
			AssertContains(craftSource, expected);
		}

		foreach (var forbidden in new[] { "HasLiteracy", "HasHandwriting", "HasPainting" })
		{
			Assert.IsFalse(craftSource.Contains(forbidden, StringComparison.Ordinal),
				$"Writing crafts should use the knowledge-gated AddCraft overload, not bespoke {forbidden} progs.");
		}
	}

	[TestMethod]
	public void AntiquityWritingCrafts_AddUpstreamCommodityAndTagSurface()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityWriting.cs");
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var tagHierarchy = ReadSource("Design Documents", "SeededTagHierarchy.csv");

		foreach (var expected in ExpectedWritingStockTags())
		{
			AssertContains(craftSource, expected);
			AssertContains(tagSource, $"AddTag(context, \"{expected}\",");
			AssertContains(tagHierarchy, $"Writing Craft Stock / {expected}");
		}

		foreach (var expected in ExpectedWritingSurfaceTags())
		{
			AssertContains(tagSource, $"AddTag(context, \"{expected}\",");
			AssertContains(tagHierarchy, expected);
		}

		foreach (var expected in new[]
		{
			"Charcoal Stick",
			"Papyrus",
			"Clay Tablets",
			"Writing Implements",
			"Document Containers",
			"Scrolls",
			"Codices"
		})
		{
			AssertContains(tagSource, $"AddTag(context, \"{expected}\",");
			AssertContains(tagHierarchy, expected);
		}
	}

	[TestMethod]
	public void AntiquityWritingCrafts_CoverEveryWritingPrototypeAndVisibleTextIsCultureNeutral()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityWriting.cs");

		foreach (var stableReference in ExpectedWritingStableReferences)
		{
			AssertContains(craftSource, $"\"{stableReference}\"");
		}

		foreach (var banned in new[]
		{
			"roman", "hellenic", "egyptian", "celtic", "germanic", "kushite", "punic", "persian",
			"etruscan", "anatolian", "scythian", "sarmatian"
		})
		{
			Assert.IsFalse(craftSource.Contains(banned, StringComparison.OrdinalIgnoreCase),
				$"Visible writing craft construction should not contain culture term {banned}.");
		}
	}

	private static IEnumerable<string> ExpectedWritingStockTags()
	{
		return
		[
			"Papyrus Sheet Stock",
			"Parchment Sheet Stock",
			"Tablet Blank",
			"Waxed Tablet Board",
			"Ink Stock",
			"Pen Blank",
			"Bookbinding Stock",
			"Scrollmaking Stock"
		];
	}

	private static IEnumerable<string> ExpectedWritingSurfaceTags()
	{
		return
		[
			"Writing Surface",
			"Loose Sheet",
			"Scroll",
			"Codex",
			"Wax Tablet",
			"Clay Tablet",
			"Wooden Writing Block",
			"Ostracon"
		];
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
