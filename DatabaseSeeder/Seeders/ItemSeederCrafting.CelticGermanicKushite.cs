using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedAntiquityCelticClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string celticKnowledge = "Celtic Textilecraft";
		const string celticKnowledgeDescription =
			"Celtic garment knowledge for assembling sleeved wool tunics, braccae, checked cloaks, mantles, gowns, skirts, and linen veils.";

		var missingGarments = CelticAntiquityClothingStableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		void AddCelticGarmentCraft(string name, string blurb, string action, string itemDescription,
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
				celticKnowledge,
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
				knowledgeSubtype: "Celtic",
				knowledgeDescription: celticKnowledgeDescription,
				knowledgeLongDescription: celticKnowledgeDescription);
		}

		var celticAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and mark|marks a rectangular Celtic garment pattern.", "$0 lay|lays out $i1, but mark|marks the garment unevenly."),
			(35, "$0 cut|cuts the cloth with $t2 and turn|turns the main edges into hems.", "$0 cut|cuts with $t2, but the edges wander out of line."),
			(45, "$0 stitch|stitches seams, folds, and fastening points with $t1, using $i2 for the strongest joins.", "$0 stitch|stitches with $t1, but the joins sit awkwardly and weakly."),
			(30, "$0 shake|shakes out $p1, checking the drape and fit over a belted layer.", "$0 shake|shakes out only $f1 after spoiling the cloth.")
		};
		var checkedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and $i3, matching the checked cloth lengths and border direction.", "$0 lay|lays out $i1 and $i3, but the checks do not line up cleanly."),
			(35, "$0 cut|cuts the cloth with $t2 and square|squares the cloak edges.", "$0 cut|cuts with $t2, but the pattern pulls crookedly along the edge."),
			(45, "$0 stitch|stitches the cloak edge and reinforcing yarn with $t1.", "$0 stitch|stitches with $t1, but the border bunches and weakens."),
			(30, "$0 lift|lifts $p1 to check its checked fall across the shoulders.", "$0 lift|lifts only $f1 after the checked cloth is spoiled.")
		};
		var sewingTools = new[]
		{
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};

		foreach (var garment in new[]
		{
			(StableReference: "antiquity_sleeved_common_wool_tunic", Name: "assemble a Celtic sleeved wool tunic", DisplayName: "a Celtic sleeved wool tunic", Material: "wool", Cloth: 590, SecondaryCloth: 0, Yarn: 30, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Easy),
			(StableReference: "antiquity_wool_braccae", Name: "assemble Celtic wool braccae", DisplayName: "Celtic wool braccae", Material: "wool", Cloth: 500, SecondaryCloth: 0, Yarn: 25, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_rectangular_wool_cloak", Name: "assemble a Celtic rectangular wool cloak", DisplayName: "a Celtic rectangular wool cloak", Material: "wool", Cloth: 1160, SecondaryCloth: 0, Yarn: 40, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_bordered_wool_tunic", Name: "assemble a Celtic fine bordered wool tunic", DisplayName: "a Celtic fine bordered wool tunic", Material: "wool", Cloth: 530, SecondaryCloth: 0, Yarn: 30, Fine: true, GeneratedProduct: false, Minimum: 35, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_wool_braccae", Name: "assemble Celtic fine wool braccae", DisplayName: "Celtic fine wool braccae", Material: "wool", Cloth: 470, SecondaryCloth: 0, Yarn: 30, Fine: true, GeneratedProduct: false, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_fine_checked_wool_cloak", Name: "assemble a Celtic fine checked wool cloak", DisplayName: "a Celtic fine checked wool cloak", Material: "wool", Cloth: 900, SecondaryCloth: 250, Yarn: 45, Fine: true, GeneratedProduct: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_long_sleeved_wool_tunic", Name: "assemble a Celtic long sleeved wool tunic", DisplayName: "a Celtic long sleeved wool tunic", Material: "wool", Cloth: 650, SecondaryCloth: 0, Yarn: 30, Fine: false, GeneratedProduct: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_wool_wrap_skirt", Name: "assemble a Celtic wool wrap skirt", DisplayName: "a Celtic wool wrap skirt", Material: "wool", Cloth: 535, SecondaryCloth: 0, Yarn: 25, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_broad_wool_mantle", Name: "assemble a Celtic broad wool mantle", DisplayName: "a Celtic broad wool mantle", Material: "wool", Cloth: 960, SecondaryCloth: 0, Yarn: 35, Fine: false, GeneratedProduct: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_sleeved_wool_gown", Name: "assemble a Celtic fine sleeved wool gown", DisplayName: "a Celtic fine sleeved wool gown", Material: "wool", Cloth: 740, SecondaryCloth: 0, Yarn: 40, Fine: true, GeneratedProduct: false, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_fine_bordered_wool_mantle", Name: "assemble a Celtic fine bordered wool mantle", DisplayName: "a Celtic fine bordered wool mantle", Material: "wool", Cloth: 910, SecondaryCloth: 0, Yarn: 40, Fine: true, GeneratedProduct: false, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_linen_shoulder_veil", Name: "assemble a Celtic linen shoulder veil", DisplayName: "a Celtic linen shoulder veil", Material: "linen", Cloth: 110, SecondaryCloth: 0, Yarn: 10, Fine: true, GeneratedProduct: false, Minimum: 30, Difficulty: Difficulty.Normal)
		})
		{
			var characteristicRequirements = garment.Fine
				? "characteristic Colour any; characteristic Fine Colour any"
				: "characteristic Colour any";
			var inputs = new List<string>
			{
				$"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
				$"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
			};
			if (garment.SecondaryCloth > 0)
			{
				inputs.Add($"Commodity - {garment.SecondaryCloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}");
			}

			AddCelticGarmentCraft(
				garment.Name,
				$"assemble {garment.DisplayName} from {garment.Material} garment cloth",
				garment.Name,
				$"{garment.Material} cloth being assembled into Celtic clothing",
				garment.Minimum,
				garment.Difficulty,
				garment.GeneratedProduct ? checkedAssemblyPhases : celticAssemblyPhases,
				inputs,
				sewingTools,
				[garment.GeneratedProduct ? StableSimpleProduct(garment.StableReference) : StableVariableProduct(garment.StableReference, garment.Fine)]);
		}
	}

	private void SeedAntiquityGermanicClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string germanicKnowledge = "Germanic Textilecraft";
		const string germanicKnowledgeDescription =
			"Germanic garment knowledge for assembling wool tunics, trousers, cloaks, scarves, gowns, mantles, veils, and skin capes for northern antiquity dress.";

		var missingGarments = GermanicAntiquityClothingStableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		void AddGermanicGarmentCraft(string name, string blurb, string action, string itemDescription,
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
				germanicKnowledge,
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
				knowledgeSubtype: "Germanic",
				knowledgeDescription: germanicKnowledgeDescription,
				knowledgeLongDescription: germanicKnowledgeDescription);
		}

		var germanicAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and mark|marks a straight northern garment cut.", "$0 lay|lays out $i1, but mark|marks the garment unevenly."),
			(35, "$0 cut|cuts the cloth with $t2 and shape|shapes the edges, sleeves, or trouser legs.", "$0 cut|cuts with $t2, but the edges and openings sit awkwardly."),
			(45, "$0 stitch|stitches strong seams and hems with $t1, reinforcing them with $i2.", "$0 stitch|stitches with $t1, but the seams pull out of line."),
			(30, "$0 arrange|arranges $p1, checking its warmth and layered fit.", "$0 arrange|arranges only $f1 after the cloth is spoiled.")
		};
		var patternedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(30, "$0 lay|lays out $i1 and $i3, matching the checked lengths before cutting.", "$0 lay|lays out $i1 and $i3, but the checked lengths do not align."),
			(35, "$0 cut|cuts the scarf lengths with $t2 and square|squares the ends.", "$0 cut|cuts with $t2, but the ends pull crooked."),
			(40, "$0 stitch|stitches the edges and fringes the ends with $t1.", "$0 stitch|stitches with $t1, but the fringes loosen unevenly."),
			(25, "$0 lift|lifts $p1 to check its fall around the neck and shoulders.", "$0 lift|lifts only $f1 after the scarf is spoiled.")
		};
		var furLinedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and trim|trims the fur lining stock from $i3.", "$0 lay|lays out $i1 and $i3, but cut|cuts the lining unevenly."),
			(40, "$0 cut|cuts the wool with $t2 and baste|bastes the lining edge into place.", "$0 cut|cuts with $t2, but the wool and lining do not sit together cleanly."),
			(50, "$0 stitch|stitches the fur lining to the cloak with $t1, reinforcing the front opening and collar.", "$0 stitch|stitches with $t1, but the lining bunches and pulls loose."),
			(30, "$0 heft|hefts $p1 to check its warmth and fall.", "$0 heft|hefts only $f1 after the lining work fails.")
		};
		var skinCapePhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 sort|sorts $i1 and $i2 into matching hair-out panels.", "$0 sort|sorts $i1 and $i2 poorly, leaving the panels mismatched."),
			(40, "$0 trim|trims the skin panels with $t2 and overlap|overlaps the strongest edges.", "$0 trim|trims with $t2, but the panels gape and twist."),
			(50, "$0 stitch|stitches the cape together with $t1 and $i3, leaving the hair side outward.", "$0 stitch|stitches with $t1, but the cape pulls apart under its own weight."),
			(30, "$0 shake|shakes out $p1, checking the shoulder fall.", "$0 shake|shakes out only $f1 after the skin panels are spoiled.")
		};
		var sewingTools = new[]
		{
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};

		foreach (var garment in new[]
		{
			(StableReference: "antiquity_straight_wool_tunic", Name: "assemble a Germanic straight wool tunic", DisplayName: "a Germanic straight wool tunic", Material: "wool", Cloth: 620, SecondaryCloth: 0, Yarn: 30, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Easy),
			(StableReference: "antiquity_narrow_wool_trousers", Name: "assemble Germanic narrow wool trousers", DisplayName: "Germanic narrow wool trousers", Material: "wool", Cloth: 515, SecondaryCloth: 0, Yarn: 25, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_heavy_wool_cloak", Name: "assemble a Germanic heavy wool cloak", DisplayName: "a Germanic heavy wool cloak", Material: "wool", Cloth: 1210, SecondaryCloth: 0, Yarn: 45, Fine: false, GeneratedProduct: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_banded_wool_tunic", Name: "assemble a Germanic fine banded wool tunic", DisplayName: "a Germanic fine banded wool tunic", Material: "wool", Cloth: 570, SecondaryCloth: 0, Yarn: 35, Fine: true, GeneratedProduct: false, Minimum: 35, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_tapered_wool_trousers", Name: "assemble Germanic fine tapered wool trousers", DisplayName: "Germanic fine tapered wool trousers", Material: "wool", Cloth: 495, SecondaryCloth: 0, Yarn: 30, Fine: true, GeneratedProduct: false, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_long_straight_wool_tunic", Name: "assemble a Germanic long straight wool tunic", DisplayName: "a Germanic long straight wool tunic", Material: "wool", Cloth: 670, SecondaryCloth: 0, Yarn: 30, Fine: false, GeneratedProduct: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_overlapping_wool_skirt", Name: "assemble a Germanic overlapping wool skirt", DisplayName: "a Germanic overlapping wool skirt", Material: "wool", Cloth: 570, SecondaryCloth: 0, Yarn: 25, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_checked_wool_scarf", Name: "assemble a Germanic checked wool scarf", DisplayName: "a Germanic checked wool scarf", Material: "wool", Cloth: 210, SecondaryCloth: 40, Yarn: 12, Fine: false, GeneratedProduct: true, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_long_wool_gown", Name: "assemble a Germanic fine long wool gown", DisplayName: "a Germanic fine long wool gown", Material: "wool", Cloth: 780, SecondaryCloth: 0, Yarn: 40, Fine: true, GeneratedProduct: false, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_fine_heavy_wool_mantle", Name: "assemble a Germanic fine heavy wool mantle", DisplayName: "a Germanic fine heavy wool mantle", Material: "wool", Cloth: 1010, SecondaryCloth: 0, Yarn: 40, Fine: true, GeneratedProduct: false, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_linen_head_veil", Name: "assemble a Germanic linen head veil", DisplayName: "a Germanic linen head veil", Material: "linen", Cloth: 120, SecondaryCloth: 0, Yarn: 10, Fine: true, GeneratedProduct: false, Minimum: 30, Difficulty: Difficulty.Normal)
		})
		{
			var characteristicRequirements = garment.Fine
				? "characteristic Colour any; characteristic Fine Colour any"
				: "characteristic Colour any";
			var inputs = new List<string>
			{
				$"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
				$"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
			};
			if (garment.SecondaryCloth > 0)
			{
				inputs.Add($"Commodity - {garment.SecondaryCloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}");
			}

			AddGermanicGarmentCraft(
				garment.Name,
				$"assemble {garment.DisplayName} from {garment.Material} garment cloth",
				garment.Name,
				$"{garment.Material} cloth being assembled into Germanic clothing",
				garment.Minimum,
				garment.Difficulty,
				garment.GeneratedProduct ? patternedAssemblyPhases : germanicAssemblyPhases,
				inputs,
				sewingTools,
				[garment.GeneratedProduct ? StableSimpleProduct(garment.StableReference) : StableVariableProduct(garment.StableReference, garment.Fine)]);
		}

		AddGermanicGarmentCraft(
			"assemble a Germanic fur-lined wool cloak",
			"assemble a Germanic fur-lined wool cloak from fine wool cloth and fur lining stock",
			"assemble a Germanic fur-lined wool cloak",
			"fine wool and fur being assembled into Germanic winter clothing",
			40,
			Difficulty.Hard,
			furLinedAssemblyPhases,
			[
				"Commodity - 1 kilogram 350 grams of wool; piletag Garment Cloth; characteristic Colour any; characteristic Fine Colour any",
				"Commodity - 45 grams of wool; piletag Spun Yarn; characteristic Colour any",
				"CommodityTag - 300 grams of a material tagged as Hair"
			],
			sewingTools,
			[StableVariableProduct("antiquity_fur_lined_wool_cloak", true)]);

		AddGermanicGarmentCraft(
			"assemble a Germanic woolly skin cape",
			"assemble a Germanic woolly skin cape from processed hair-on skin",
			"assemble a Germanic woolly skin cape",
			"hair-on animal skin being assembled into Germanic winter clothing",
			25,
			Difficulty.Normal,
			skinCapePhases,
			[
				"CommodityTag - 1 kilogram 200 grams of a material tagged as Animal Skin",
				"CommodityTag - 250 grams of a material tagged as Hair",
				"Commodity - 30 grams of wool; piletag Spun Yarn; characteristic Colour any"
			],
			sewingTools,
			[StableSimpleProduct("antiquity_woolly_skin_cape")]);
	}

	private void SeedAntiquityKushiteClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string kushiteKnowledge = "Kushite Textilecraft";
		const string kushiteKnowledgeDescription =
			"Kushite garment knowledge for assembling Nile Valley linen and cotton kilts, shoulder cloths, robes, shawls, headdresses, beadwork, skirts, dresses, and headcloths.";

		var missingGarments = KushiteAntiquityClothingStableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		void AddKushiteGarmentCraft(string name, string blurb, string action, string itemDescription,
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
				kushiteKnowledge,
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
				knowledgeSubtype: "Kushite",
				knowledgeDescription: kushiteKnowledgeDescription,
				knowledgeLongDescription: kushiteKnowledgeDescription);
		}

		var kushiteAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(30, "$0 smooth|smooths out $i1 and mark|marks the wrap, fold, or opening lines.", "$0 smooth|smooths out $i1, but mark|marks the cloth unevenly."),
			(35, "$0 cut|cuts the cloth with $t2 and fold|folds the edges into narrow hems.", "$0 cut|cuts with $t2, but the hems pull crookedly."),
			(40, "$0 stitch|stitches and reinforce|reinforces the folded or draped garment points with $t1.", "$0 stitch|stitches with $t1, but the joins sit awkwardly."),
			(25, "$0 shake|shakes out $p1, checking the fall of the cloth.", "$0 shake|shakes out only $f1 after the cloth is spoiled.")
		};
		var kushiteBeadedPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(30, "$0 smooth|smooths out $i1 and measure|measures the linen for a beaded edge or apron run.", "$0 smooth|smooths out $i1, but measure|measures the beadwork unevenly."),
			(35, "$0 cut|cuts the linen with $t2 and sew|sews the supporting hems.", "$0 cut|cuts with $t2, but leave|leaves the supporting hems weak."),
			(45, "$0 thread|threads $i3 onto $t1 and stitch|stitches the bead rows into place.", "$0 thread|threads $i3 onto $t1, but the bead rows sag and bunch."),
			(25, "$0 lift|lifts $p1 to check the fall of the beaded linen.", "$0 lift|lifts only $f1 after the beadwork pulls loose.")
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
			(StableReference: "adjacent_antiquity_narrow_linen_kilt", Name: "assemble a Kushite narrow linen kilt", DisplayName: "a Kushite narrow linen kilt", Material: "linen", Cloth: 210, Yarn: 15, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy),
			(StableReference: "adjacent_antiquity_linen_shoulder_cloth", Name: "assemble a Kushite linen shoulder cloth", DisplayName: "a Kushite linen shoulder cloth", Material: "linen", Cloth: 170, Yarn: 10, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy),
			(StableReference: "adjacent_antiquity_cotton_wrap_skirt", Name: "assemble a Kushite cotton wrap skirt", DisplayName: "a Kushite cotton wrap skirt", Material: "cotton", Cloth: 310, Yarn: 20, BeadStock: 0, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_sleeveless_linen_tunic", Name: "assemble a Kushite sleeveless linen tunic", DisplayName: "a Kushite sleeveless linen tunic", Material: "linen", Cloth: 285, Yarn: 25, BeadStock: 0, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_fringed_linen_robe", Name: "assemble a Kushite fringed linen robe", DisplayName: "a Kushite fringed linen robe", Material: "linen", Cloth: 500, Yarn: 40, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_tasseled_linen_shawl", Name: "assemble a Kushite tasseled linen shawl", DisplayName: "a Kushite tasseled linen shawl", Material: "linen", Cloth: 190, Yarn: 30, BeadStock: 0, Fine: true, Minimum: 30, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_tall_linen_headdress", Name: "assemble a Kushite tall linen headdress", DisplayName: "a Kushite tall linen headdress", Material: "linen", Cloth: 235, Yarn: 25, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_beaded_linen_girdle", Name: "assemble a Kushite beaded linen girdle", DisplayName: "a Kushite beaded linen girdle", Material: "linen", Cloth: 210, Yarn: 25, BeadStock: 40, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_cotton_draped_dress", Name: "assemble a Kushite cotton draped dress", DisplayName: "a Kushite cotton draped dress", Material: "cotton", Cloth: 430, Yarn: 30, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_linen_bead_apron", Name: "assemble a Kushite linen bead apron", DisplayName: "a Kushite linen bead apron", Material: "linen", Cloth: 150, Yarn: 20, BeadStock: 50, Fine: true, Minimum: 35, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_plain_cotton_headcloth", Name: "assemble a Kushite plain cotton headcloth", DisplayName: "a Kushite plain cotton headcloth", Material: "cotton", Cloth: 80, Yarn: 8, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy)
		})
		{
			var characteristicRequirements = garment.Fine
				? "characteristic Colour any; characteristic Fine Colour any"
				: "characteristic Colour any";
			var inputs = new List<string>
			{
				$"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
				$"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
			};

			if (garment.BeadStock > 0)
			{
				inputs.Add($"CommodityTag - {garment.BeadStock} grams of a material tagged as Glass; piletag Bead Stock");
			}

			AddKushiteGarmentCraft(
				garment.Name,
				$"assemble {garment.DisplayName} from {garment.Material} garment cloth",
				garment.Name,
				$"{garment.Material} cloth being assembled into Kushite clothing",
				garment.Minimum,
				garment.Difficulty,
				garment.BeadStock > 0 ? kushiteBeadedPhases : kushiteAssemblyPhases,
				inputs,
				garment.BeadStock > 0 ? beadingTools : sewingTools,
				[StableVariableProduct(garment.StableReference, garment.Fine)]);
		}
	}
}
