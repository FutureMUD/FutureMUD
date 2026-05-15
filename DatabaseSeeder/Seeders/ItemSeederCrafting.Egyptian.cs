using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedAntiquityEgyptianClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string ancientKnowledge = "Ancient Textile Production";
		const string egyptianKnowledge = "Egyptian Textilecraft";
		const string ancientKnowledgeDescription =
			"Shared ancient techniques for preparing textile fibres, spinning yarn, weaving cloth, dyeing, fulling, and textile ornament stock.";
		const string egyptianKnowledgeDescription =
			"Egyptian garment knowledge for assembling linen kilts, shoulder cloths, sleeveless tunics, robes, shawls, headdresses, and beaded linen dress pieces.";

		AddCraft(
			"sort glass beads for textile edging",
			"Tailoring",
			"sort glass beads into bead stock for textile edging",
			"sorting glass beads",
			"glass beads being sorted and strung for textile edging",
			ancientKnowledge,
			"Tailoring",
			10,
			Difficulty.Easy,
			Outcome.MinorFail,
			5,
			3,
			false,
			[
				(25, "$0 sort|sorts $i1 into small, even beads suitable for garment edging.", "$0 sort|sorts $i1 poorly, mixing chipped and uneven pieces into the bead stock."),
				(30, "$0 thread|threads trial runs on $t1, setting aside beads with usable holes and regular shapes.", "$0 thread|threads trial runs on $t1, but crack|cracks too many of the beads."),
				(25, "$0 gather|gathers $p1 into measured bead stock.", "$0 gather|gathers only $f1 from the flawed bead sorting.")
			],
			["CommodityTag - 130 grams of a material tagged as Glass"],
			["TagTool - Held - an item with the Beading Needle tag"],
			["CommodityProduct - 110 grams of glass commodity; tag Bead Stock"],
			["CommodityProduct - 40 grams of glass commodity; tag Bead Stock"],
			knowledgeSubtype: "Textiles",
			knowledgeDescription: ancientKnowledgeDescription,
			knowledgeLongDescription: ancientKnowledgeDescription);

		var missingGarments = EgyptianAntiquityClothingStableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		void AddEgyptianGarmentCraft(string name, string blurb, string action, string itemDescription,
			int minimumTraitValue, Difficulty difficulty,
			IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
			IEnumerable<string> tools, IEnumerable<string> products)
		{
			AddCraft(
				name,
				"Tailoring",
				blurb,
				action,
				itemDescription,
				egyptianKnowledge,
				"Tailoring",
				minimumTraitValue,
				difficulty,
				Outcome.MinorFail,
				5,
				3,
				false,
				phases,
				inputs,
				tools,
				products,
				[],
				knowledgeSubtype: "Egyptian",
				knowledgeDescription: egyptianKnowledgeDescription,
				knowledgeLongDescription: egyptianKnowledgeDescription);
		}

		var linenAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(30, "$0 smooth|smooths out $i1 and mark|marks the linen for the required wrap, fold, or body opening.", "$0 smooth|smooths out $i1, but mark|marks the linen unevenly."),
			(35, "$0 cut|cuts the linen with $t2 and turn|turns the edges into narrow hems.", "$0 cut|cuts with $t2, but the hems pull crookedly."),
			(40, "$0 stitch|stitches, pleat|pleats, and reinforce|reinforces the garment's folds and fastening points with $t1.", "$0 stitch|stitches with $t1, but the folds sit awkwardly and the fastening points are weak."),
			(25, "$0 shake|shakes out $p1, checking the drape and fall.", "$0 shake|shakes out only $f1 after the linen is spoiled.")
		};
		var beadedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(30, "$0 smooth|smooths out $i1 and measure|measures the linen panel for a beaded edge.", "$0 smooth|smooths out $i1, but measure|measures the beaded edge poorly."),
			(35, "$0 cut|cuts the linen with $t2 and sew|sews the supporting hems.", "$0 cut|cuts with $t2, but leave|leaves the supporting hems uneven."),
			(45, "$0 thread|threads $i3 onto $t1 and stitch|stitches the bead rows into the garment edge.", "$0 thread|threads $i3 onto $t1, but the bead rows sag and bunch."),
			(25, "$0 lift|lifts $p1 to check the hang of the beaded linen.", "$0 lift|lifts only $f1 after the beadwork pulls loose.")
		};
		var sewingTools = new[]
		{
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};
		var beadingTools = new[]
		{
			"TagTool - Held - an item with the Beading Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};

		foreach (var garment in new[]
		{
			(StableReference: "adjacent_antiquity_narrow_linen_kilt", Name: "assemble an Egyptian narrow linen kilt", DisplayName: "an Egyptian narrow linen kilt", Cloth: 210, Yarn: 15, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy),
			(StableReference: "adjacent_antiquity_linen_shoulder_cloth", Name: "assemble an Egyptian linen shoulder cloth", DisplayName: "an Egyptian linen shoulder cloth", Cloth: 170, Yarn: 10, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy),
			(StableReference: "adjacent_antiquity_sleeveless_linen_tunic", Name: "assemble an Egyptian sleeveless linen tunic", DisplayName: "an Egyptian sleeveless linen tunic", Cloth: 285, Yarn: 25, BeadStock: 0, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_fringed_linen_robe", Name: "assemble an Egyptian fringed linen robe", DisplayName: "an Egyptian fringed linen robe", Cloth: 500, Yarn: 40, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_tasseled_linen_shawl", Name: "assemble an Egyptian tasseled linen shawl", DisplayName: "an Egyptian tasseled linen shawl", Cloth: 190, Yarn: 30, BeadStock: 0, Fine: true, Minimum: 30, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_tall_linen_headdress", Name: "assemble an Egyptian tall linen headdress", DisplayName: "an Egyptian tall linen headdress", Cloth: 235, Yarn: 25, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_beaded_linen_girdle", Name: "assemble an Egyptian beaded linen girdle", DisplayName: "an Egyptian beaded linen girdle", Cloth: 210, Yarn: 25, BeadStock: 40, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_linen_bead_apron", Name: "assemble an Egyptian linen bead apron", DisplayName: "an Egyptian linen bead apron", Cloth: 150, Yarn: 20, BeadStock: 50, Fine: true, Minimum: 35, Difficulty: Difficulty.Normal)
		})
		{
			var characteristicRequirements = garment.Fine
				? "characteristic Colour any; characteristic Fine Colour any"
				: "characteristic Colour any";
			var inputs = new List<string>
			{
				$"Commodity - {garment.Cloth} grams of linen; piletag Garment Cloth; {characteristicRequirements}",
				$"Commodity - {garment.Yarn} grams of linen; piletag Spun Yarn; characteristic Colour any"
			};

			if (garment.BeadStock > 0)
			{
				inputs.Add($"CommodityTag - {garment.BeadStock} grams of a material tagged as Glass; piletag Bead Stock");
			}

			AddEgyptianGarmentCraft(
				garment.Name,
				$"assemble {garment.DisplayName} from linen garment cloth",
				garment.Name,
				"linen cloth being assembled into Egyptian clothing",
				garment.Minimum,
				garment.Difficulty,
				garment.BeadStock > 0 ? beadedAssemblyPhases : linenAssemblyPhases,
				inputs,
				garment.BeadStock > 0 ? beadingTools : sewingTools,
				[StableVariableProduct(garment.StableReference, garment.Fine)]);
		}
	}
}
