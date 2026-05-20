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

	private void SeedAntiquityMedicalCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		SeedAntiquityMedicalIntermediateCommodityCrafts();

		var usedCraftNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		foreach (var stableReference in AntiquityMedicalStableReferences)
		{
			if (!TryLookupReworkItem(stableReference, out var item))
			{
				continue;
			}

			SeedAntiquityMedicalFinishedCraft(stableReference, item, usedCraftNames);
		}
	}

	private void SeedAntiquityMedicalIntermediateCommodityCrafts()
	{
		AddAntiquityMedicalCommodityCraft(
			"prepare clean dressing stock",
			"Medicine",
			"Medicine",
			AncientMedicalTreatmentKnowledge,
			"Treatment Supplies",
			"wash, boil and fold cloth into clean dressing stock",
			[
				CommodityInput(900.0, "linen", "Garment Cloth", colour: true, fineColour: true),
				"LiquidUse - 2 litres of Water"
			],
			[
				"TagTool - Held - an item with the Shears tag",
				"TagTool - InRoom - an item with the Cooking Pot tag",
				"TagTool - InRoom - an item with the Fire tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(720.0)} of linen commodity; tag Dressing Stock; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

		AddAntiquityMedicalCommodityCraft(
			"compound honey poultice stock",
			"Medicine",
			"Medicine",
			AncientHerbalRemediesKnowledge,
			"Poultices",
			"mix honey, clean cloth and herbs into poultice stock",
			[
				CommodityInput(420.0, "honey"),
				CommodityInput(160.0, "yarrow"),
				CommodityInput(220.0, "linen", "Dressing Stock", colour: true, fineColour: true)
			],
			[
				"TagTool - Held - an item with the Mortar and Pestle tag",
				"TagTool - Held - an item with the Ointment Spatula tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(620.0)} of honey commodity; tag Poultice Stock"]);

		AddAntiquityMedicalCommodityCraft(
			"render herbal salve stock",
			"Medicine",
			"Medicine",
			AncientHerbalRemediesKnowledge,
			"Salves",
			"warm resin, oil and herbs into salve stock",
			[
				CommodityInput(240.0, "resin"),
				CommodityInput(180.0, "aloe vera"),
				CommodityInput(120.0, "garlic")
			],
			[
				"TagTool - InRoom - an item with the Cooking Pot tag",
				"TagTool - InRoom - an item with the Fire tag",
				"TagTool - Held - an item with the Ointment Spatula tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(430.0)} of resin commodity; tag Salve Stock"]);

		AddAntiquityMedicalCommodityCraft(
			"steep medicinal decoction stock",
			"Medicine",
			"Medicine",
			AncientHerbalRemediesKnowledge,
			"Decoctions",
			"steep bark, leaves and roots into medicinal decoction stock",
			[
				CommodityInput(220.0, "willow"),
				CommodityInput(120.0, "mint"),
				CommodityInput(80.0, "mandrake"),
				"LiquidUse - 1 litre of Water"
			],
			[
				"TagTool - InRoom - an item with the Cooking Pot tag",
				"TagTool - InRoom - an item with the Fire tag",
				"TagTool - Held - an item with the Medicine Strainer tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(320.0)} of herb commodity; tag Decoction Stock"]);

		AddAntiquityMedicalCommodityCraft(
			"press fumigation cone stock",
			"Medicine",
			"Medicine",
			AncientHerbalRemediesKnowledge,
			"Fumigations",
			"press dried herbs, charcoal and resin into fumigation stock",
			[
				CommodityInput(140.0, "henbane"),
				CommodityInput(120.0, "mandrake"),
				CommodityInput(120.0, "charcoal"),
				CommodityInput(80.0, "resin")
			],
			[
				"TagTool - Held - an item with the Mortar and Pestle tag",
				"TagTool - Held - an item with the Ointment Spatula tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(360.0)} of henbane commodity; tag Fumigation Stock"]);

		AddAntiquityMedicalCommodityCraft(
			"twist gut suture stock",
			"Medicine",
			"Tailoring",
			AncientMedicalTreatmentKnowledge,
			"Sutures",
			"clean and twist gut into suture stock",
			[
				CommodityInput(260.0, "gut"),
				"LiquidUse - 1 litre of Water"
			],
			[
				"TagTool - Held - an item with the Sewing Needle tag",
				"TagTool - Held - an item with the Shears tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(160.0)} of gut commodity; tag Suture Stock"]);

		AddAntiquityMedicalCommodityCraft(
			"shave willow splint stock",
			"Carpentry",
			"Carpentry",
			AncientMedicalTreatmentKnowledge,
			"Splints",
			"shave straight willow into padded splint stock",
			[CommodityInput(900.0, "willow", "Furniture Timber Stock")],
			[
				"TagTool - Held - an item with the Hand Saw tag",
				"TagTool - Held - an item with the Rasp tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(720.0)} of willow commodity; tag Splint Stock"]);

		AddAntiquityMedicalCommodityCraft(
			"prepare prosthetic stock",
			"Carpentry",
			"Carpentry",
			AncientProstheticsKnowledge,
			"Prosthetics",
			"shape wood, leather and padding into prosthetic stock",
			[
				CommodityInput(1500.0, "cedar", "Furniture Timber Stock"),
				CommodityInput(300.0, "leather", "Leather Strap", colour: true, fineColour: true),
				CommodityInput(180.0, "linen", "Dressing Stock", colour: true, fineColour: true)
			],
			[
				"TagTool - Held - an item with the Hand Saw tag",
				"TagTool - Held - an item with the Rasp tag",
				"TagTool - Held - an item with the Sewing Needle tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(1500.0)} of cedar commodity; tag Prosthetic Stock"]);

		AddAntiquityMedicalCommodityCraft(
			"forge surgical tool blanks",
			"Toolmaking",
			"Blacksmithing",
			AncientSurgicalInstrumentKnowledge,
			"Surgical Tools",
			"forge bronze into surgical tool blanks",
			[CommodityInput(900.0, "bronze")],
			ForgeTools(),
			[$"CommodityProduct - {FormatCommodityAmount(720.0)} of bronze commodity; tag Surgical Tool Blank"]);

		AddAntiquityMedicalCommodityCraft(
			"sort herbal remedy stock",
			"Medicine",
			"Medicine",
			AncientHerbalRemediesKnowledge,
			"Materia Medica",
			"sort dried medicinal herbs into remedy stock",
			[
				CommodityInput(160.0, "yarrow"),
				CommodityInput(160.0, "mint"),
				CommodityInput(120.0, "ephedra"),
				CommodityInput(80.0, "foxglove")
			],
			[
				"TagTool - Held - an item with the Medicine Strainer tag",
				"TagTool - Held - an item with the Mortar and Pestle tag"
			],
			[$"CommodityProduct - {FormatCommodityAmount(420.0)} of herb commodity; tag Herbal Remedy Stock"]);

		foreach (var stock in new[]
		         {
			         (Material: "willow", StockTag: "Decoction Stock", Blurb: "shave and dry willow bark for decoctions"),
			         (Material: "mint", StockTag: "Decoction Stock", Blurb: "dry mint leaves for infusions"),
			         (Material: "ephedra", StockTag: "Decoction Stock", Blurb: "dry ephedra stems for bracing brews"),
			         (Material: "foxglove", StockTag: "Decoction Stock", Blurb: "measure foxglove leaves for tinctures"),
			         (Material: "mandrake", StockTag: "Decoction Stock", Blurb: "slice mandrake root for draughts"),
			         (Material: "poppy latex", StockTag: "Decoction Stock", Blurb: "thicken poppy latex for measured draughts"),
			         (Material: "garlic", StockTag: "Salve Stock", Blurb: "crush garlic into salve stock"),
			         (Material: "aloe vera", StockTag: "Salve Stock", Blurb: "scrape aloe pulp into cooling salve stock"),
			         (Material: "yarrow", StockTag: "Salve Stock", Blurb: "crush yarrow into styptic stock"),
			         (Material: "henbane", StockTag: "Fumigation Stock", Blurb: "dry henbane for fumigation stock"),
			         (Material: "mandrake", StockTag: "Fumigation Stock", Blurb: "dry mandrake for fumigation stock")
		         })
		{
			var safeMaterialName = stock.Material.Replace(" ", "-", StringComparison.OrdinalIgnoreCase);
			AddAntiquityMedicalCommodityCraft(
				$"prepare {safeMaterialName} {stock.StockTag.ToLowerInvariant().Replace(" ", "-", StringComparison.OrdinalIgnoreCase)}",
				"Medicine",
				"Medicine",
				AncientHerbalRemediesKnowledge,
				"Materia Medica",
				stock.Blurb,
				[CommodityInput(220.0, stock.Material)],
				HerbalRemedyTools(),
				[$"CommodityProduct - {FormatCommodityAmount(180.0)} of {stock.Material} commodity; tag {stock.StockTag}"]);
		}
	}

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

	private void SeedAntiquityMedicalFinishedCraft(string stableReference, GameItemProto item,
		IDictionary<string, int> usedCraftNames)
	{
		var path = GetAntiquityMedicalCraftPath(stableReference);
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
			GetAntiquityMedicalFinalPhases(path),
			BuildAntiquityMedicalFinalInputs(stableReference, item, path),
			path.Tools,
			[StableSimpleProduct(stableReference)],
			knowledgeSubtype: path.KnowledgeSubtype,
			knowledgeDescription: $"{path.Knowledge} covers antiquity {path.KnowledgeSubtype.ToLowerInvariant()} with culture-neutral visible craft text.",
			knowledgeLongDescription: $"{path.Knowledge} covers antiquity {path.KnowledgeSubtype.ToLowerInvariant()} with culture-neutral visible craft text.");
	}

	private static AntiquityMedicalCraftPath GetAntiquityMedicalCraftPath(string stableReference)
	{
		if (ContainsAny(stableReference, "scalpel", "forceps", "clamp", "cautery", "saw", "probe"))
		{
			return MedicalPath("Toolmaking", "Blacksmithing", AncientSurgicalInstrumentKnowledge,
				"Surgical Tools", "Surgical Tool Blank", "make", "making", 25, Difficulty.Normal, SurgicalToolTools());
		}

		if (stableReference.Contains("suture_needle", StringComparison.OrdinalIgnoreCase))
		{
			return MedicalPath("Toolmaking", "Bonecarving", AncientSurgicalInstrumentKnowledge,
				"Surgical Tools", "Suture Stock", "polish", "polishing", 20, Difficulty.Normal,
				["TagTool - Held - an item with the Scraper Knife tag", "TagTool - Held - an item with the Rasp tag"]);
		}

		if (stableReference.Contains("suture", StringComparison.OrdinalIgnoreCase))
		{
			return MedicalPath("Medicine", "Medicine", AncientMedicalTreatmentKnowledge, "Sutures",
				"Suture Stock", "assemble", "assembling", 20, Difficulty.Normal, TreatmentKitTools());
		}

		if (ContainsAny(stableReference, "splint", "sling"))
		{
			return MedicalPath("Medicine", "Medicine", AncientMedicalTreatmentKnowledge, "Splints",
				stableReference.Contains("sling", StringComparison.OrdinalIgnoreCase) ? "Dressing Stock" : "Splint Stock",
				"make", "making", 15, Difficulty.Normal, TreatmentKitTools());
		}

		if (ContainsAny(stableReference, "crutch", "cane", "peg_leg", "prosthetic", "hook_hand", "clay_eye"))
		{
			return MedicalPath("Prosthetics", stableReference.Contains("hook", StringComparison.OrdinalIgnoreCase) ? "Blacksmithing" : "Carpentry",
				AncientProstheticsKnowledge, "Prosthetics", "Prosthetic Stock", "make", "making", 25,
				Difficulty.Normal, ProstheticTools(stableReference));
		}

		if (ContainsAny(stableReference, "poultice", "salve", "styptic", "draught", "tincture", "infusion", "packets", "fumigation", "smoke"))
		{
			var stock = ContainsAny(stableReference, "fumigation", "smoke")
				? "Fumigation Stock"
				: stableReference.Contains("poultice", StringComparison.OrdinalIgnoreCase)
					? "Poultice Stock"
					: ContainsAny(stableReference, "salve", "styptic")
					? "Salve Stock"
					: "Decoction Stock";
			return MedicalPath("Medicine", "Medicine", AncientHerbalRemediesKnowledge, "Herbal Remedies",
				stock, "prepare", "preparing", 20, Difficulty.Normal, HerbalRemedyTools());
		}

		if (ContainsAny(stableReference, "pouch", "roll"))
		{
			return MedicalPath("Medicine", "Medicine", AncientMedicalTreatmentKnowledge, "Treatment Kits",
				"Dressing Stock", "assemble", "assembling", 20, Difficulty.Normal, TreatmentKitTools());
		}

		return MedicalPath("Medicine", "Medicine", AncientMedicalTreatmentKnowledge, "Treatment Supplies",
			"Dressing Stock", "make", "making", 15, Difficulty.Easy, TreatmentKitTools());
	}

	private static AntiquityMedicalCraftPath MedicalPath(string category, string skill, string knowledge,
		string knowledgeSubtype, string primaryStockTag, string verb, string gerund, int minimumTraitValue,
		Difficulty difficulty, IReadOnlyList<string> tools)
	{
		return new AntiquityMedicalCraftPath(category, skill, knowledge, knowledgeSubtype, primaryStockTag, verb,
			gerund, minimumTraitValue, difficulty, tools);
	}

	private List<string> BuildAntiquityMedicalFinalInputs(string stableReference, GameItemProto item,
		AntiquityMedicalCraftPath path)
	{
		var material = GetMaterialName(item);
		var amount = Math.Max(10.0, item.Weight);
		var inputs = new List<string>
		{
			CommodityInput(amount, material, path.PrimaryStockTag)
		};

		if (ContainsAny(stableReference, "kit", "pouch", "roll") && !stableReference.Contains("bandage", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(180.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true));
			inputs.Add(CommodityInput(80.0, "linen", "Spun Yarn", colour: true, fineColour: true));
		}

		if (ContainsAny(stableReference, "bandage", "dressing", "cloth", "pad", "compress", "sling"))
		{
			inputs.Add(CommodityInput(60.0, "linen", "Spun Yarn", colour: true, fineColour: true));
		}

		if (ContainsAny(stableReference, "poultice", "dressing", "antiseptic"))
		{
			inputs.Add(CommodityInput(120.0, "honey", "Poultice Stock"));
		}

		if (ContainsAny(stableReference, "salve", "styptic"))
		{
			inputs.Add(CommodityInput(80.0, "resin", "Salve Stock"));
		}

		if (ContainsAny(stableReference, "draught", "tincture", "infusion", "packets"))
		{
			inputs.Add(CommodityInput(70.0, "fired clay", "Pottery Clay Body"));
			inputs.Add("LiquidUse - 250 millilitres of Water");
		}

		if (ContainsAny(stableReference, "fumigation", "smoke"))
		{
			inputs.Add(CommodityInput(40.0, "charcoal", "Fumigation Stock"));
		}

		if (ContainsAny(stableReference, "splint", "crutch", "cane"))
		{
			inputs.Add(CommodityInput(90.0, "linen", "Dressing Stock", colour: true, fineColour: true));
			inputs.Add(CommodityInput(80.0, "leather", "Leather Strap", colour: true, fineColour: true));
		}

		if (ContainsAny(stableReference, "prosthetic", "peg_leg", "hook_hand", "clay_eye"))
		{
			inputs.Add(CommodityInput(160.0, "leather", "Leather Strap", colour: true, fineColour: true));
			inputs.Add(CommodityInput(100.0, "linen", "Dressing Stock", colour: true, fineColour: true));
		}

		if (ContainsAny(stableReference, "scalpel", "forceps", "clamp", "cautery", "saw", "probe", "hook_hand"))
		{
			inputs.Add(CommodityInput(60.0, "wood", "Tool Blank Stock"));
		}

		return inputs;
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

	private static IReadOnlyList<(int Seconds, string Echo, string FailEcho)> GetAntiquityMedicalFinalPhases(
		AntiquityMedicalCraftPath path)
	{
		return
		[
			(30, $"$0 lay|lays out $i1 and begin|begins {path.Gerund} the medical item.", "$0 lay|lays out $i1, but the first preparation is careless."),
			(45, "$0 shape|shapes, bind|binds, mix|mixes, or finish|finishes the work with $t1.", "$0 work|works with $t1, but the item comes together poorly."),
			(30, "$0 inspect|inspects $p1 and set|sets it aside for use.", "$0 inspect|inspects the failed work and salvage|salvages only $f1.")
		];
	}

	private static IReadOnlyList<string> TreatmentKitTools()
	{
		return
		[
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		];
	}

	private static IReadOnlyList<string> HerbalRemedyTools()
	{
		return
		[
			"TagTool - Held - an item with the Mortar and Pestle tag",
			"TagTool - Held - an item with the Medicine Strainer tag",
			"TagTool - Held - an item with the Ointment Spatula tag"
		];
	}

	private static IReadOnlyList<string> SurgicalToolTools()
	{
		return
		[
			"TagTool - InRoom - an item with the Lit Smelting Furnace tag",
			"TagTool - InRoom - an item with the Anvil tag",
			"TagTool - Held - an item with the Hammer tag",
			"TagTool - Held - an item with the Forge Tongs tag"
		];
	}

	private static IReadOnlyList<string> ProstheticTools(string stableReference)
	{
		if (stableReference.Contains("hook", StringComparison.OrdinalIgnoreCase))
		{
			return SurgicalToolTools();
		}

		if (stableReference.Contains("clay_eye", StringComparison.OrdinalIgnoreCase))
		{
			return
			[
				"TagTool - Held - an item with the Trowel tag",
				"TagTool - Held - an item with the Pigment Shell tag"
			];
		}

		return
		[
			"TagTool - Held - an item with the Hand Saw tag",
			"TagTool - Held - an item with the Rasp tag",
			"TagTool - Held - an item with the Sewing Needle tag"
		];
	}
}
