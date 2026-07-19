using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AncientWritingMaterialsKnowledge = "Ancient Writing Materials";
	private const string AncientPapyrusMakingKnowledge = "Ancient Papyrus Making";
	private const string AncientParchmentMakingKnowledge = "Ancient Parchment Making";
	private const string AncientTabletMakingKnowledge = "Ancient Tablet Making";
	private const string AncientInkAndPigmentKnowledge = "Ancient Ink and Pigment Making";
	private const string AncientScribingImplementsKnowledge = "Ancient Scribing Implements";
	private const string AncientScrollAndCodexBindingKnowledge = "Ancient Scroll and Codex Binding";

	private static readonly string[] AntiquityWritingStableReferences =
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

	private sealed record AntiquityWritingCraftPath(
		string Category,
		string Skill,
		string Knowledge,
		string KnowledgeSubtype,
		string PrimaryStockTag,
		string Verb,
		string Gerund,
		int MinimumTraitValue,
		Difficulty Difficulty,
		IReadOnlyList<string> Tools);

	private void SeedAntiquityWritingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		SeedAntiquityWritingIntermediateCommodityCrafts();

		var usedCraftNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		foreach (var stableReference in AntiquityWritingStableReferences)
		{
			if (!TryLookupReworkItem(stableReference, out var item))
			{
				continue;
			}

			SeedAntiquityWritingFinishedCraft(stableReference, item, usedCraftNames);
		}
	}

	private void SeedAntiquityWritingIntermediateCommodityCrafts()
	{
		AddAntiquityWritingCommodityCraft(
			"prepare papyrus sheet stock",
			"Papyrusmaking",
			"Basketry",
			AncientPapyrusMakingKnowledge,
			"Papyrus Stock",
			"strip and press papyrus pith into sheet stock",
			[
				CommodityInput(1100.0, "papyrus"),
				"LiquidUse - 2 litres of Water"
			],
			[
				"TagTool - Held - an item with the Papyrus Strip Knife tag",
				"TagTool - InRoom - an item with the Papyrus Pressing Board tag",
				"TagTool - Held - an item with the Papyrus Burnishing Shell tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(850.0)} of papyrus commodity; tag Papyrus Sheet Stock"]);

		AddAntiquityWritingCommodityCraft(
			"scrape parchment sheet stock",
			"Parchmentmaking",
			"Leathermaking",
			AncientParchmentMakingKnowledge,
			"Parchment Stock",
			"scrape stretched hide into parchment sheet stock",
			[
				CommodityInput(1200.0, "leather", "Prepared Hide"),
				"Commodity - 150 grams of slaked lime",
				"LiquidUse - 2 litres of Water"
			],
			[
				"TagTool - InRoom - an item with the Parchment Stretching Frame tag",
				"TagTool - Held - an item with the Parchment Scraping Knife tag",
				"TagTool - Held - an item with the Parchment Pumice tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(720.0)} of parchment commodity; tag Parchment Sheet Stock"]);

		AddAntiquityWritingCommodityCraft(
			"make waxed tablet board stock",
			"Tabletmaking",
			"Carpentry",
			AncientTabletMakingKnowledge,
			"Wax Tablet Stock",
			"recess wooden boards and fill them with writing wax",
			[
				CommodityInput(900.0, "cedar", "Furniture Panel Stock"),
				CommodityInput(320.0, "beeswax")
			],
			[
				"TagTool - Held - an item with the Chisel tag",
				"TagTool - Held - an item with the Wax Spatula tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(980.0)} of beeswax commodity; tag Waxed Tablet Board"]);

		AddAntiquityWritingCommodityCraft(
			"shape clay tablet blanks",
			"Tabletmaking",
			"Pottery",
			AncientTabletMakingKnowledge,
			"Clay Tablet Stock",
			"levigate clay and shape tablet blanks",
			[
				CommodityInput(1200.0, "clay"),
				"LiquidUse - 1 litre of Water"
			],
			[
				"TagTool - Held - an item with the Trowel tag",
				"TagTool - Held - an item with the Stylus tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(1000.0)} of clay commodity; tag Tablet Blank"]);

		AddAntiquityWritingCommodityCraft(
			"smooth wooden writing block stock",
			"Tabletmaking",
			"Carpentry",
			AncientTabletMakingKnowledge,
			"Wooden Tablet Stock",
			"plane and smooth wooden blocks for writing",
			[CommodityInput(900.0, "cedar", "Furniture Timber Stock")],
			[
				"TagTool - Held - an item with the Adze tag",
				"TagTool - Held - an item with the Scraper Knife tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(720.0)} of cedar commodity; tag Tablet Blank"]);

		AddAntiquityWritingCommodityCraft(
			"grind soot ink stock",
			"Inkmaking",
			"Painting",
			AncientInkAndPigmentKnowledge,
			"Ink Stock",
			"grind soot and binder into ink stock",
			[
				CommodityInput(300.0, "soot"),
				CommodityInput(80.0, "resin"),
				"LiquidUse - 250 millilitres of Water"
			],
			[
				"TagTool - Held - an item with the Pigment Muller tag",
				"TagTool - Held - an item with the Pigment Shell tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(320.0)} of soot commodity; tag Ink Stock"]);

		AddAntiquityWritingCommodityCraft(
			"bundle reed pen blanks",
			"Toolmaking",
			"Carpentry",
			AncientScribingImplementsKnowledge,
			"Pen Stock",
			"cut papyrus stems into pen and stylus blanks",
			[CommodityInput(240.0, "papyrus")],
			["TagTool - Held - an item with the Pen Knife tag"],
			[$"CommodityProduct - {FormatCommodityAmount(190.0)} of papyrus commodity; tag Pen Blank"]);

		AddAntiquityWritingCommodityCraft(
			"cure quill pen blanks",
			"Toolmaking",
			"Tailoring",
			AncientScribingImplementsKnowledge,
			"Pen Stock",
			"cure feathers into quill pen blanks",
			[CommodityInput(120.0, "feather")],
			[
				"TagTool - Held - an item with the Pen Knife tag",
				"TagTool - InRoom - an item with the Quill Curing Sand tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(90.0)} of feather commodity; tag Pen Blank"]);

		AddAntiquityWritingCommodityCraft(
			"shape hard stylus blanks",
			"Toolmaking",
			"Bonecarving",
			AncientScribingImplementsKnowledge,
			"Stylus Stock",
			"shape bone into stylus and handle blanks",
			[CommodityInput(260.0, "bone")],
			[
				"TagTool - Held - an item with the Scraper Knife tag",
				"TagTool - Held - an item with the Parchment Pumice tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(210.0)} of bone commodity; tag Pen Blank"]);

		AddAntiquityWritingCommodityCraft(
			"cut charcoal writing stock",
			"Toolmaking",
			"Carpentry",
			AncientScribingImplementsKnowledge,
			"Pen Stock",
			"cut charcoal into short writing sticks",
			[CommodityInput(180.0, "charcoal")],
			["TagTool - Held - an item with the Scraper Knife tag"],
			[$"CommodityProduct - {FormatCommodityAmount(140.0)} of charcoal commodity; tag Pen Blank"]);

		AddAntiquityWritingCommodityCraft(
			"forge bronze stylus stock",
			"Toolmaking",
			"Blacksmithing",
			AncientScribingImplementsKnowledge,
			"Stylus Stock",
			"forge bronze into stylus and pen-knife blanks",
			[CommodityInput(360.0, "bronze")],
			ForgeTools(),
			[$"CommodityProduct - {FormatCommodityAmount(300.0)} of bronze commodity; tag Pen Blank"]);

		AddAntiquityWritingCommodityCraft(
			"prepare scrollmaking stock",
			"Scrollmaking",
			"Tailoring",
			AncientScrollAndCodexBindingKnowledge,
			"Scrollmaking Stock",
			"prepare rods, ties and cover stock for scrolls",
			[
				CommodityInput(220.0, "cedar", "Furniture Timber Stock"),
				CommodityInput(80.0, "linen", "Spun Yarn", colour: true, fineColour: true)
			],
			[
				"TagTool - Held - an item with the Scroll Roller Rod tag",
				"TagTool - Held - an item with the Scroll Smoothing Stone tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(240.0)} of cedar commodity; tag Scrollmaking Stock"]);

		AddAntiquityWritingCommodityCraft(
			"prepare codex binding stock",
			"Bookbinding",
			"Leathermaking",
			AncientScrollAndCodexBindingKnowledge,
			"Bookbinding Stock",
			"pare leather, boards and sewing supports for codices",
			[
				CommodityInput(380.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
				CommodityInput(260.0, "cedar", "Furniture Panel Stock"),
				CommodityInput(80.0, "linen", "Spun Yarn", colour: true, fineColour: true)
			],
			[
				"TagTool - Held - an item with the Leather Paring Knife tag",
				"TagTool - Held - an item with the Bookbinder's Punch tag",
				"TagTool - Held - an item with the Bookbinder's Needle tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(520.0)} of leather commodity; tag Bookbinding Stock"]);
	}

	private void AddAntiquityWritingCommodityCraft(
		string name,
		string category,
		string skill,
		string knowledge,
		string knowledgeSubtype,
		string blurb,
		IEnumerable<string> inputs,
		IEnumerable<string> tools,
		IEnumerable<string> products)
	{
		AddAntiquityCraft(
			name,
			category,
			blurb,
			name,
			$"{blurb} for writing materials",
			knowledge,
			skill,
			20,
			Difficulty.Normal,
			GetAntiquityWritingCommodityPhases(),
			inputs,
			tools,
			products,
			knowledgeSubtype: knowledgeSubtype,
			knowledgeDescription: $"{knowledge} covers antiquity {knowledgeSubtype.ToLowerInvariant()} production for scribal work.",
			knowledgeLongDescription: $"{knowledge} covers antiquity {knowledgeSubtype.ToLowerInvariant()} production for scribal work.");
	}

	private void SeedAntiquityWritingFinishedCraft(string stableReference, GameItemProto item,
		IDictionary<string, int> usedCraftNames)
	{
		var path = GetAntiquityWritingCraftPath(stableReference);
		var visibleName = SanitiseHouseholdCraftDisplayName(item.ShortDescription);
		var craftName = BuildUniqueAntiquityEquipmentCraftName(usedCraftNames, path.Verb,
			StripLeadingArticle(visibleName));

		AddAntiquityCraft(
			craftName,
			path.Category,
			$"{path.Verb} {visibleName}",
			$"{path.Gerund} {visibleName}",
			$"{visibleName} being made",
			path.Knowledge,
			path.Skill,
			path.MinimumTraitValue,
			path.Difficulty,
			GetAntiquityWritingFinalPhases(path),
			BuildAntiquityWritingFinalInputs(stableReference, item, path),
			path.Tools,
			[StableSimpleProduct(stableReference)],
			knowledgeSubtype: path.KnowledgeSubtype,
			knowledgeDescription: $"{path.Knowledge} covers antiquity {path.KnowledgeSubtype.ToLowerInvariant()} without culture-specific visible craft text.",
			knowledgeLongDescription: $"{path.Knowledge} covers antiquity {path.KnowledgeSubtype.ToLowerInvariant()} without culture-specific visible craft text.");
	}

	private AntiquityWritingCraftPath GetAntiquityWritingCraftPath(string stableReference)
	{
		if (stableReference.Contains("papyrus_sheet", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Equals("antiquity_loose_papyrus_sheet", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Papyrusmaking", "Basketry", AncientPapyrusMakingKnowledge, "Papyrus Goods",
				"Papyrus Sheet Stock", "make", "making", 15, Difficulty.Easy, PapyrusTools());
		}

		if (stableReference.Contains("papyrus_scroll", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Scrollmaking", "Tailoring", AncientScrollAndCodexBindingKnowledge, "Scrolls",
				"Papyrus Sheet Stock", "assemble", "assembling", 20, Difficulty.Normal, ScrollmakingTools());
		}

		if (stableReference.Contains("parchment_codex", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Bookbinding", "Leathermaking", AncientScrollAndCodexBindingKnowledge, "Codices",
				"Bookbinding Stock", "bind", "binding", 30, Difficulty.Hard, BookbindingTools());
		}

		if (stableReference.Contains("parchment_scroll", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Scrollmaking", "Tailoring", AncientScrollAndCodexBindingKnowledge, "Scrolls",
				"Parchment Sheet Stock", "assemble", "assembling", 25, Difficulty.Normal, ScrollmakingTools());
		}

		if (stableReference.Contains("parchment", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Parchmentmaking", "Leathermaking", AncientParchmentMakingKnowledge, "Parchment Goods",
				"Parchment Sheet Stock", "make", "making", 20, Difficulty.Normal, ParchmentTools());
		}

		if (stableReference.Contains("wax", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Tabletmaking", "Carpentry", AncientTabletMakingKnowledge, "Wax Tablets",
				"Waxed Tablet Board", "assemble", "assembling", 20, Difficulty.Normal, WaxTabletTools());
		}

		if (stableReference.Contains("clay_tablet", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("ostracon", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Tabletmaking", "Pottery", AncientTabletMakingKnowledge, "Clay Tablets",
				"Tablet Blank", "shape", "shaping", 15, Difficulty.Normal, ClayTabletTools());
		}

		if (stableReference.Contains("wooden_writing_block", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Tabletmaking", "Carpentry", AncientTabletMakingKnowledge, "Wooden Tablets",
				"Tablet Blank", "smooth", "smoothing", 15, Difficulty.Easy, WoodTabletTools());
		}

		if (stableReference.Contains("inkwell", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Tabletmaking", "Pottery", AncientInkAndPigmentKnowledge, "Ink Vessels",
				"Pottery Clay Body", "make", "making", 15, Difficulty.Normal,
				["TagTool - Held - an item with the Trowel tag"]);
		}

		if (stableReference.Contains("ink", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Inkmaking", "Painting", AncientInkAndPigmentKnowledge, "Ink",
				"Ink Stock", "prepare", "preparing", 20, Difficulty.Normal, InkTools());
		}

		if (stableReference.Contains("stylus", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("pen", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("brush", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("charcoal_writing_stick", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Toolmaking", GetWritingImplementSkill(stableReference),
				AncientScribingImplementsKnowledge, "Writing Implements",
				stableReference.Contains("bronze", StringComparison.OrdinalIgnoreCase) ? "Tool Blank Stock" : "Pen Blank",
				"make", "making", 15, Difficulty.Normal, ScribingImplementTools(stableReference));
		}

		if (stableReference.Contains("scroll_case", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("scroll_tie", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Scrollmaking", "Tailoring", AncientScrollAndCodexBindingKnowledge, "Document Containers",
				"Scrollmaking Stock", "make", "making", 15, Difficulty.Normal, ScrollmakingTools());
		}

		if (stableReference.Contains("pouch", StringComparison.OrdinalIgnoreCase))
		{
			return WritingPath("Bookbinding", "Leathermaking", AncientScrollAndCodexBindingKnowledge, "Document Containers",
				"Bookbinding Stock", "sew", "sewing", 20, Difficulty.Normal,
				["TagTool - Held - an item with the Leather Paring Knife tag", "TagTool - Held - an item with the Bookbinder's Needle tag"]);
		}

		return WritingPath("Bookbinding", "Tailoring", AncientScrollAndCodexBindingKnowledge, "Document Containers",
			"Bookbinding Stock", "sew", "sewing", 15, Difficulty.Normal,
			["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"]);
	}

	private static AntiquityWritingCraftPath WritingPath(string category, string skill, string knowledge,
		string knowledgeSubtype, string primaryStockTag, string verb, string gerund, int minimumTraitValue,
		Difficulty difficulty, IReadOnlyList<string> tools)
	{
		return new AntiquityWritingCraftPath(category, skill, knowledge, knowledgeSubtype, primaryStockTag, verb,
			gerund, minimumTraitValue, difficulty, tools);
	}

	private List<string> BuildAntiquityWritingFinalInputs(string stableReference, GameItemProto item,
		AntiquityWritingCraftPath path)
	{
		var material = GetMaterialName(item);
		var amount = Math.Max(10.0, item.Weight);
		var inputs = new List<string>
		{
			CommodityInput(Math.Max(10.0, amount), material, path.PrimaryStockTag)
		};

		if (stableReference.Contains("scroll", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(40.0, "linen", "Scrollmaking Stock", colour: true, fineColour: true));
		}

		if (stableReference.Contains("codex", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(260.0, "parchment", "Parchment Sheet Stock"));
			inputs.Add(CommodityInput(180.0, "cedar", "Furniture Panel Stock"));
			inputs.Add(CommodityInput(120.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true));
		}

		if (stableReference.Contains("wax", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(120.0, "beeswax"));
		}

		if (stableReference.Contains("inkwell", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("ink_pot", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(80.0, "fired clay", "Pottery Clay Body"));
		}

		if (stableReference.Contains("pouch", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(100.0, "linen", "Spun Yarn", colour: true, fineColour: true));
		}

		if (stableReference.Contains("tablet_wrap", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(40.0, "linen", "Spun Yarn", colour: true, fineColour: true));
		}

		if (stableReference.Contains("pen_knife", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("scraper_knife", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(40.0, "wood", "Tool Blank Stock"));
		}

		return inputs;
	}

	private static IReadOnlyList<(int Seconds, string Echo, string FailEcho)> GetAntiquityWritingCommodityPhases()
	{
		return
		[
			(35, "$0 sort|sorts and clean|cleans the writing material stock.", "$0 sort|sorts the stock poorly, leaving grit and flaws in it."),
			(45, "$0 work|works the stock with $t1, shaping it for scribal use.", "$0 work|works the stock unevenly with $t1."),
			(35, "$0 finish|finishes and set|sets aside $p1.", "$0 finish|finishes the work badly, leaving only $f1.")
		];
	}

	private static IReadOnlyList<(int Seconds, string Echo, string FailEcho)> GetAntiquityWritingFinalPhases(
		AntiquityWritingCraftPath path)
	{
		return
		[
			(30, $"$0 lay|lays out $i1 and begin|begins {path.Gerund} the scribal item.", "$0 lay|lays out $i1, but the first measurements are uneven."),
			(40, "$0 shape|shapes, trim|trims, and fit|fits the material with $t1.", "$0 shape|shapes the material poorly with $t1."),
			(35, "$0 finish|finishes $p1 and check|checks the surface, fit, or balance.", "$0 finish|finishes the piece badly, leaving only $f1.")
		];
	}

	private static IReadOnlyList<string> PapyrusTools()
	{
		return
		[
			"TagTool - Held - an item with the Papyrus Strip Knife tag",
			"TagTool - Held - an item with the Papyrus Burnishing Shell tag"
		];
	}

	private static IReadOnlyList<string> ParchmentTools()
	{
		return
		[
			"TagTool - Held - an item with the Parchment Scraping Knife tag",
			"TagTool - Held - an item with the Parchment Pumice tag"
		];
	}

	private static IReadOnlyList<string> ScrollmakingTools()
	{
		return
		[
			"TagTool - Held - an item with the Scroll Smoothing Stone tag",
			"TagTool - Held - an item with the Scroll Roller Rod tag"
		];
	}

	private static IReadOnlyList<string> BookbindingTools()
	{
		return
		[
			"TagTool - Held - an item with the Bookbinder's Needle tag",
			"TagTool - Held - an item with the Bookbinder's Punch tag",
			"TagTool - Held - an item with the Leather Paring Knife tag"
		];
	}

	private static IReadOnlyList<string> WaxTabletTools()
	{
		return
		[
			"TagTool - Held - an item with the Chisel tag",
			"TagTool - Held - an item with the Wax Spatula tag"
		];
	}

	private static IReadOnlyList<string> ClayTabletTools()
	{
		return
		[
			"TagTool - Held - an item with the Stylus tag",
			"TagTool - Held - an item with the Trowel tag"
		];
	}

	private static IReadOnlyList<string> WoodTabletTools()
	{
		return
		[
			"TagTool - Held - an item with the Adze tag",
			"TagTool - Held - an item with the Scraper Knife tag"
		];
	}

	private static IReadOnlyList<string> InkTools()
	{
		return
		[
			"TagTool - Held - an item with the Pigment Muller tag",
			"TagTool - Held - an item with the Pigment Shell tag"
		];
	}

	private static IReadOnlyList<string> ScribingImplementTools(string stableReference)
	{
		if (stableReference.Contains("bronze", StringComparison.OrdinalIgnoreCase))
		{
			return ForgeTools();
		}

		return
		[
			"TagTool - Held - an item with the Pen Knife tag",
			"TagTool - Held - an item with the Scraper Knife tag"
		];
	}

	private static string GetWritingImplementSkill(string stableReference)
	{
		if (stableReference.Contains("bronze", StringComparison.OrdinalIgnoreCase))
		{
			return "Blacksmithing";
		}

		if (stableReference.Contains("bone", StringComparison.OrdinalIgnoreCase))
		{
			return "Bonecarving";
		}

		if (stableReference.Contains("quill", StringComparison.OrdinalIgnoreCase))
		{
			return "Tailoring";
		}

		return "Carpentry";
	}

	private static bool IsAntiquityWritingSuiteStableReference(string stableReference)
	{
		return AntiquityWritingStableReferences.Contains(stableReference, StringComparer.OrdinalIgnoreCase);
	}
}
