using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AncientMedicalTreatmentKnowledge = "Ancient Medical Treatment Supplies";
	private const string AncientHerbalRemediesKnowledge = "Ancient Herbal Remedies";
	private const string AncientSurgicalInstrumentKnowledge = "Ancient Surgical Instruments";
	private const string AncientProstheticsKnowledge = "Ancient Prosthetics and Mobility Aids";

	private static readonly string[] AntiquityMedicalStableReferences =
	[
		"antiquity_linen_bandage_roll",
		"antiquity_honeyed_linen_dressing",
		"antiquity_vinegar_wound_cloth",
		"antiquity_yarrow_styptic_pad",
		"antiquity_wool_compress",
		"antiquity_wooden_splint_pair",
		"antiquity_linen_arm_sling",
		"antiquity_linen_tending_kit",
		"antiquity_wound_cleaning_kit",
		"antiquity_honey_antiseptic_kit",
		"antiquity_gut_suture_spool",
		"antiquity_bone_suture_needle",
		"antiquity_suturing_kit",
		"antiquity_bronze_surgical_probe",
		"antiquity_bronze_scalpel",
		"antiquity_bronze_forceps",
		"antiquity_bronze_arterial_clamp",
		"antiquity_bronze_cautery_iron",
		"antiquity_bronze_bone_saw",
		"antiquity_willow_bark_packets",
		"antiquity_poppy_latex_draught",
		"antiquity_mandrake_draught_vial",
		"antiquity_ephedra_brew_packets",
		"antiquity_foxglove_tincture_vial",
		"antiquity_mint_infusion_bundle",
		"antiquity_honey_poultice_pot",
		"antiquity_garlic_salve_pot",
		"antiquity_aloe_burn_salve_pot",
		"antiquity_henbane_fumigation_cone",
		"antiquity_mandrake_smoke_cone",
		"antiquity_herbalist_pouch",
		"antiquity_surgical_tool_roll",
		"antiquity_willow_crutch",
		"antiquity_simple_walking_cane",
		"antiquity_padded_wooden_peg_leg",
		"antiquity_carved_prosthetic_foot",
		"antiquity_carved_prosthetic_hand",
		"antiquity_bronze_hook_hand",
		"antiquity_painted_clay_eye"
	];

	private sealed record AntiquityMedicalCraftPath(
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

	private void AddAntiquityMedicalCommodityCraft(
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
			$"{blurb} for medical work",
			knowledge,
			skill,
			20,
			Difficulty.Normal,
			GetAntiquityMedicalCommodityPhases(),
			inputs,
			tools,
			products,
			knowledgeSubtype: knowledgeSubtype,
			knowledgeDescription: $"{knowledge} covers antiquity {knowledgeSubtype.ToLowerInvariant()} production for medicine.",
			knowledgeLongDescription: $"{knowledge} covers antiquity {knowledgeSubtype.ToLowerInvariant()} production for medicine.");
	}

	private static IReadOnlyList<(int Seconds, string Echo, string FailEcho)> GetAntiquityMedicalCommodityPhases()
	{
		return
		[
			(30, "$0 sort|sorts the medical materials and remove|removes spoilage or grit.", "$0 sort|sorts the materials poorly, leaving spoilage in the stock."),
			(45, "$0 work|works the materials with $t1 into usable medical stock.", "$0 work|works the materials unevenly with $t1."),
			(30, "$0 finish|finishes and set|sets aside $p1.", "$0 finish|finishes badly, leaving only $f1.")
		];
	}
}
