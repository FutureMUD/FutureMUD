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

	private static IReadOnlyList<(int Seconds, string Echo, string FailEcho)> GetAntiquityWritingCommodityPhases()
	{
		return
		[
			(35, "$0 sort|sorts and clean|cleans the writing material stock.", "$0 sort|sorts the stock poorly, leaving grit and flaws in it."),
			(45, "$0 work|works the stock with $t1, shaping it for scribal use.", "$0 work|works the stock unevenly with $t1."),
			(35, "$0 finish|finishes and set|sets aside $p1.", "$0 finish|finishes the work badly, leaving only $f1.")
		];
	}

	private static bool IsAntiquityWritingSuiteStableReference(string stableReference)
	{
		return AntiquityWritingStableReferences.Contains(stableReference, StringComparer.OrdinalIgnoreCase);
	}
}
