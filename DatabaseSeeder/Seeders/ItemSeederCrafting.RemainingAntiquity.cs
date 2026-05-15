using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private sealed record AntiquityCultureGarmentCraftSpec(
		string StableReference,
		string Name,
		string DisplayName,
		string Material,
		int Cloth,
		int Yarn,
		bool Fine,
		int Minimum,
		Difficulty Difficulty,
		bool GeneratedProduct = false,
		int SecondaryCloth = 0,
		int Hair = 0);

	private void SeedAntiquityCultureGarmentCrafts(
		IReadOnlyDictionary<string, string> stableReferences,
		string knowledge,
		string knowledgeSubtype,
		string knowledgeDescription,
		IEnumerable<AntiquityCultureGarmentCraftSpec> garments)
	{
		var missingGarments = stableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		void AddCultureGarmentCraft(string name, string blurb, string action, string itemDescription,
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
				knowledge,
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
				knowledgeSubtype: knowledgeSubtype,
				knowledgeDescription: knowledgeDescription,
				knowledgeLongDescription: knowledgeDescription);
		}

		var assemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and mark|marks the garment panels, folds, or drape lines.", "$0 lay|lays out $i1, but mark|marks the garment unevenly."),
			(35, "$0 cut|cuts the cloth with $t2 and turn|turns the edges into narrow hems.", "$0 cut|cuts with $t2, but the hems pull crookedly."),
			(45, "$0 stitch|stitches seams, hems, folds, and fastening points with $t1 and $i2.", "$0 stitch|stitches with $t1, but the joins sit awkwardly and weakly."),
			(30, "$0 shake|shakes out $p1, checking its drape and finished line.", "$0 shake|shakes out only $f1 after the cloth is spoiled.")
		};
		var patternedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and $i3, matching the coloured cloth lengths and pattern lines.", "$0 lay|lays out $i1 and $i3, but the colours and pattern lines do not align."),
			(35, "$0 cut|cuts the cloth with $t2 and square|squares the visible edges.", "$0 cut|cuts with $t2, but the patterned edges pull crookedly."),
			(45, "$0 stitch|stitches the panels and decorative bands with $t1 and $i2.", "$0 stitch|stitches with $t1, but the bands bunch and weaken."),
			(30, "$0 lift|lifts $p1 to check its patterned fall.", "$0 lift|lifts only $f1 after the patterned cloth is spoiled.")
		};
		var furTrimmedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and trim|trims the fur stock from $i3.", "$0 lay|lays out $i1 and $i3, but cut|cuts the trim unevenly."),
			(40, "$0 cut|cuts the cloth with $t2 and baste|bastes the fur along the exposed edges.", "$0 cut|cuts with $t2, but the trim and cloth do not sit together cleanly."),
			(50, "$0 stitch|stitches the fur trim to the garment with $t1 and $i2.", "$0 stitch|stitches with $t1, but the fur trim bunches and pulls loose."),
			(30, "$0 lift|lifts $p1, checking its warmth and finished trim.", "$0 lift|lifts only $f1 after the trimming work fails.")
		};
		var sewingTools = new[]
		{
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};

		foreach (var garment in garments)
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

			if (garment.Hair > 0)
			{
				inputs.Add($"CommodityTag - {garment.Hair} grams of a material tagged as Hair");
			}

			AddCultureGarmentCraft(
				garment.Name,
				$"assemble {garment.DisplayName} from {garment.Material} garment cloth",
				garment.Name,
				$"{garment.Material} cloth being assembled into {knowledgeSubtype} clothing",
				garment.Minimum,
				garment.Difficulty,
				garment.Hair > 0 ? furTrimmedAssemblyPhases : garment.GeneratedProduct ? patternedAssemblyPhases : assemblyPhases,
				inputs,
				sewingTools,
				[garment.GeneratedProduct ? StableSimpleProduct(garment.StableReference) : StableVariableProduct(garment.StableReference, garment.Fine)]);
		}
	}

	private void SeedAntiquityPunicClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string punicKnowledge = "Punic Textilecraft";
		const string punicKnowledgeDescription =
			"Punic and Phoenician garment knowledge for assembling fitted linen tunics, waistcloths, overblouses, folded robes, hoods, mantles, gowns, overdrapes, and star-bordered linen robes.";

		SeedAntiquityCultureGarmentCrafts(
			PunicAntiquityClothingStableReferences,
			punicKnowledge,
			"Punic",
			punicKnowledgeDescription,
			[
				new("antiquity_short_fitted_linen_tunic", "assemble a Punic short fitted linen tunic", "a Punic short fitted linen tunic", "linen", 340, 20, false, 15, Difficulty.Normal),
				new("antiquity_patterned_linen_waistcloth", "assemble a Punic patterned linen waistcloth", "a Punic patterned linen waistcloth", "linen", 220, 15, true, 30, Difficulty.Normal),
				new("antiquity_short_sleeved_linen_overblouse", "assemble a Punic short sleeved linen overblouse", "a Punic short sleeved linen overblouse", "linen", 300, 20, true, 35, Difficulty.Normal),
				new("antiquity_long_linen_inner_robe", "assemble a Punic long linen inner robe", "a Punic long linen inner robe", "linen", 670, 30, false, 20, Difficulty.Normal),
				new("antiquity_one_shoulder_wool_mantle", "assemble a Punic one shoulder wool mantle", "a Punic one shoulder wool mantle", "wool", 940, 35, false, 20, Difficulty.Normal),
				new("antiquity_long_folded_linen_robe", "assemble a Punic long folded linen robe", "a Punic long folded linen robe", "linen", 745, 35, false, 25, Difficulty.Normal),
				new("antiquity_loose_linen_hood", "assemble a Punic loose linen hood", "a Punic loose linen hood", "linen", 125, 10, false, 15, Difficulty.Easy),
				new("antiquity_fine_full_linen_gown", "assemble a Punic fine full linen gown", "a Punic fine full linen gown", "linen", 820, 40, true, 40, Difficulty.Hard),
				new("antiquity_left_shoulder_overdrape", "assemble a Punic left shoulder overdrape", "a Punic left shoulder overdrape", "linen", 280, 20, true, 35, Difficulty.Normal),
				new("antiquity_star_bordered_linen_robe", "assemble a Punic star bordered linen robe", "a Punic star bordered linen robe", "linen", 780, 40, true, 40, Difficulty.Hard)
			]);
	}

	private void SeedAntiquityPersianClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string persianKnowledge = "Persian Textilecraft";
		const string persianKnowledgeDescription =
			"Persian and Median garment knowledge for assembling sarapis tunics, anaxyrides trousers, kandyes, pleated court robes and gowns, cloth belts, and head-and-neck veils.";

		SeedAntiquityCultureGarmentCrafts(
			PersianAntiquityClothingStableReferences,
			persianKnowledge,
			"Persian",
			persianKnowledgeDescription,
			[
				new("antiquity_sarapis_wool_tunic", "assemble a Persian wool sarapis", "a Persian wool sarapis", "wool", 690, 35, false, 20, Difficulty.Normal),
				new("antiquity_fine_sarapis_linen_tunic", "assemble a Persian fine linen sarapis", "a Persian fine linen sarapis", "linen", 495, 30, true, 35, Difficulty.Normal),
				new("antiquity_anaxyrides_wool_trousers", "assemble Persian wool anaxyrides", "Persian wool anaxyrides", "wool", 590, 30, false, 20, Difficulty.Normal),
				new("antiquity_fine_patterned_anaxyrides", "assemble Persian fine patterned anaxyrides", "Persian fine patterned anaxyrides", "wool", 440, 35, true, 40, Difficulty.Hard, true, 120),
				new("antiquity_wool_kandys", "assemble a Persian wool kandys", "a Persian wool kandys", "wool", 1290, 50, false, 25, Difficulty.Hard),
				new("antiquity_fine_wool_kandys", "assemble a Persian fine wool kandys", "a Persian fine wool kandys", "wool", 1190, 50, true, 45, Difficulty.Hard),
				new("antiquity_pleated_court_robe", "assemble a Persian pleated court robe", "a Persian pleated court robe", "linen", 930, 50, true, 45, Difficulty.Hard),
				new("antiquity_fine_pleated_court_gown", "assemble a Persian fine pleated court gown", "a Persian fine pleated court gown", "linen", 835, 45, true, 45, Difficulty.Hard),
				new("antiquity_wide_cloth_belt", "assemble a Persian wide cloth belt", "a Persian wide cloth belt", "linen", 150, 10, false, 15, Difficulty.Easy),
				new("antiquity_fine_wide_cloth_belt", "assemble a Persian fine wide cloth belt", "a Persian fine wide cloth belt", "linen", 135, 10, true, 30, Difficulty.Normal),
				new("antiquity_full_head_and_neck_veil", "assemble a Persian full head and neck veil", "a Persian full head and neck veil", "linen", 175, 15, true, 35, Difficulty.Normal)
			]);
	}

	private void SeedAntiquityEtruscanClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string etruscanKnowledge = "Etruscan Textilecraft";
		const string etruscanKnowledgeDescription =
			"Etruscan garment knowledge for assembling short-sleeved linen tunics, bordered wool tunics, curved tebennas, wrapped skirts, shoulder cloaks, fitted gowns, and linen head mantles.";

		SeedAntiquityCultureGarmentCrafts(
			EtruscanAntiquityClothingStableReferences,
			etruscanKnowledge,
			"Etruscan",
			etruscanKnowledgeDescription,
			[
				new("adjacent_antiquity_short_sleeved_linen_tunic", "assemble an Etruscan short sleeved linen tunic", "an Etruscan short sleeved linen tunic", "linen", 395, 25, false, 15, Difficulty.Normal),
				new("adjacent_antiquity_bordered_wool_tunic", "assemble an Etruscan bordered wool tunic", "an Etruscan bordered wool tunic", "wool", 530, 30, true, 35, Difficulty.Normal),
				new("adjacent_antiquity_curved_tebenna", "assemble an Etruscan curved wool tebenna", "an Etruscan curved wool tebenna", "wool", 860, 35, false, 25, Difficulty.Normal),
				new("adjacent_antiquity_fine_curved_tebenna", "assemble an Etruscan fine curved wool tebenna", "an Etruscan fine curved wool tebenna", "wool", 780, 35, true, 40, Difficulty.Hard),
				new("adjacent_antiquity_wrapped_linen_skirt", "assemble an Etruscan wrapped linen skirt", "an Etruscan wrapped linen skirt", "linen", 340, 20, false, 15, Difficulty.Normal),
				new("adjacent_antiquity_rectangular_shoulder_cloak", "assemble an Etruscan rectangular shoulder cloak", "an Etruscan rectangular shoulder cloak", "wool", 740, 35, false, 20, Difficulty.Normal),
				new("adjacent_antiquity_fitted_linen_gown", "assemble an Etruscan fitted linen gown", "an Etruscan fitted linen gown", "linen", 590, 35, true, 40, Difficulty.Hard),
				new("adjacent_antiquity_linen_head_mantle", "assemble an Etruscan linen head mantle", "an Etruscan linen head mantle", "linen", 165, 15, true, 30, Difficulty.Normal)
			]);
	}

	private void SeedAntiquityAnatolianClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string anatolianKnowledge = "Anatolian Textilecraft";
		const string anatolianKnowledgeDescription =
			"Anatolian garment knowledge for assembling belted wool tunics, banded tunics, leg wraps, hooded cloaks, forward-pointing felt caps, wool capes, patterned robes, fringed mantles, wrapped skirts, and rectangular veils.";

		SeedAntiquityCultureGarmentCrafts(
			AnatolianAntiquityClothingStableReferences,
			anatolianKnowledge,
			"Anatolian",
			anatolianKnowledgeDescription,
			[
				new("adjacent_antiquity_belted_wool_tunic", "assemble an Anatolian belted wool tunic", "an Anatolian belted wool tunic", "wool", 590, 30, false, 20, Difficulty.Normal),
				new("adjacent_antiquity_fine_banded_tunic", "assemble an Anatolian fine banded wool tunic", "an Anatolian fine banded wool tunic", "wool", 450, 35, true, 40, Difficulty.Hard, true, 130),
				new("adjacent_antiquity_banded_leg_wraps", "assemble Anatolian banded wool leg wraps", "Anatolian banded wool leg wraps", "wool", 160, 10, false, 15, Difficulty.Easy),
				new("adjacent_antiquity_hooded_wool_cloak", "assemble an Anatolian hooded wool cloak", "an Anatolian hooded wool cloak", "wool", 1000, 45, false, 25, Difficulty.Hard),
				new("adjacent_antiquity_forward_pointing_felt_cap", "assemble an Anatolian forward pointing felt cap", "an Anatolian forward pointing felt cap", "felt", 105, 8, false, 20, Difficulty.Normal),
				new("adjacent_antiquity_short_wool_cape", "assemble an Anatolian short wool cape", "an Anatolian short wool cape", "wool", 495, 25, false, 20, Difficulty.Normal),
				new("adjacent_antiquity_fine_patterned_wool_robe", "assemble an Anatolian fine patterned wool robe", "an Anatolian fine patterned wool robe", "wool", 855, 45, true, 45, Difficulty.Hard),
				new("adjacent_antiquity_fringed_wool_mantle", "assemble an Anatolian fringed wool mantle", "an Anatolian fringed wool mantle", "wool", 720, 40, true, 40, Difficulty.Hard),
				new("adjacent_antiquity_wool_wrapped_skirt", "assemble an Anatolian wrapped wool skirt", "an Anatolian wrapped wool skirt", "wool", 515, 25, false, 20, Difficulty.Normal),
				new("adjacent_antiquity_fine_rectangular_veil", "assemble an Anatolian fine rectangular linen veil", "an Anatolian fine rectangular linen veil", "linen", 110, 10, true, 30, Difficulty.Normal)
			]);
	}

	private void SeedAntiquityScythianSarmatianClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string scythianKnowledge = "Scythian-Sarmatian Textilecraft";
		const string scythianKnowledgeDescription =
			"Scythian and Sarmatian garment knowledge for assembling felt riding caps, riding tunics and trousers, open caftans, fur-trimmed caftans, split riding skirts, and long felt coats.";

		SeedAntiquityCultureGarmentCrafts(
			ScythianSarmatianAntiquityClothingStableReferences,
			scythianKnowledge,
			"Scythian-Sarmatian",
			scythianKnowledgeDescription,
			[
				new("adjacent_antiquity_felt_riding_cap", "assemble a Scythian felt riding cap", "a Scythian felt riding cap", "felt", 90, 8, false, 20, Difficulty.Normal),
				new("adjacent_antiquity_tall_felt_cap", "assemble a Scythian tall felt cap", "a Scythian tall felt cap", "felt", 130, 10, true, 35, Difficulty.Hard),
				new("adjacent_antiquity_riding_tunic", "assemble a Scythian riding tunic", "a Scythian riding tunic", "wool", 645, 35, false, 20, Difficulty.Normal),
				new("adjacent_antiquity_wool_riding_trousers", "assemble Scythian wool riding trousers", "Scythian wool riding trousers", "wool", 495, 25, false, 20, Difficulty.Normal),
				new("adjacent_antiquity_patterned_riding_trousers", "assemble Scythian patterned riding trousers", "Scythian patterned riding trousers", "wool", 470, 30, true, 35, Difficulty.Hard),
				new("adjacent_antiquity_open_riding_caftan", "assemble a Scythian open riding caftan", "a Scythian open riding caftan", "wool", 820, 40, false, 25, Difficulty.Hard),
				new("adjacent_antiquity_fur_trimmed_caftan", "assemble a Scythian fur trimmed caftan", "a Scythian fur trimmed caftan", "wool", 1060, 45, true, 45, Difficulty.Hard, Hair: 140),
				new("adjacent_antiquity_split_riding_skirt", "assemble a Scythian split riding skirt", "a Scythian split riding skirt", "wool", 590, 30, false, 20, Difficulty.Normal),
				new("adjacent_antiquity_long_felt_coat", "assemble a Scythian long felt coat", "a Scythian long felt coat", "felt", 1270, 50, true, 45, Difficulty.Hard)
			]);
	}
}
