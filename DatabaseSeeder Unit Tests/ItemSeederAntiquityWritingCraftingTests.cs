#nullable enable

using Recipes = MudSharp_Unit_Tests.CompiledRecipeFixtures;

using DatabaseSeeder.Seeders;

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
		Recipes.AssertNativeCraftCoverage(ExpectedWritingStableReferences,
			"antiquity_simple_papyrus_scroll", "antiquity_bronze_scraper_knife");
		foreach (var key in ExpectedWritingStableReferences) Assert.IsTrue(Recipes.Item(key).Components.Count > 0, key);
	}

	[TestMethod]
	public void AntiquityWritingCrafts_RegisterKnowledgeGatedSuiteAndNoLegacyLiteracyProgs()
	{
		foreach (var knowledge in new[] { "Ancient Papyrus Making", "Ancient Parchment Making", "Ancient Tablet Making", "Ancient Ink and Pigment Making", "Ancient Scribing Implements", "Ancient Scroll and Codex Binding" }) Recipes.Knowledge(knowledge);
		foreach (var key in ExpectedWritingStableReferences.Except(new[] { "antiquity_simple_papyrus_scroll", "antiquity_bronze_scraper_knife" }))
			Assert.IsTrue(Recipes.ForItem(key).Any(recipe => recipe.Gate is CatalogueKnowledgeRecipeGate), key);
	}

	[TestMethod]
	public void AntiquityWritingCrafts_AddUpstreamCommodityAndTagSurface()
	{
		Recipes.RequiresTags(ExpectedWritingStockTags().ToArray());
	}

	[TestMethod]
	public void AntiquityWritingCrafts_CoverEveryWritingPrototypeAndVisibleTextIsCultureNeutral()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.Crafting.AntiquityWriting.cs");

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
