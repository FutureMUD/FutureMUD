using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedAntiquityRomanClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string romanKnowledge = "Roman Textilecraft";
		const string romanKnowledgeDescription =
			"Roman garment knowledge for assembling tunicae, togae, pallae, stolae, and practical woollen mantles.";

		var missingGarments = RomanAntiquityClothingStableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		void AddRomanGarmentCraft(string name, string blurb, string action, string itemDescription,
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
				romanKnowledge,
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
				knowledgeSubtype: "Roman",
				knowledgeDescription: romanKnowledgeDescription,
				knowledgeLongDescription: romanKnowledgeDescription);
		}

		var romanAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and mark|marks the Roman garment dimensions and drape lines.", "$0 lay|lays out $i1, but mark|marks the garment dimensions unevenly."),
			(35, "$0 cut|cuts the cloth with $t2 and shape|shapes the edges, openings, or rounded fall required by the garment.", "$0 cut|cuts with $t2, but the edges and openings sit awkwardly."),
			(40, "$0 thread|threads $t1 and finish|finishes hems, straps, joins, or weighted fold points as needed.", "$0 thread|threads $t1, but the hems and joins pull out of line."),
			(30, "$0 arrange|arranges $p1, checking the weight and drape.", "$0 arrange|arranges only $f1 after the cloth is spoiled.")
		};
		var sewingTools = new[]
		{
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};

		foreach (var garment in new[]
		{
			(StableReference: "antiquity_knee_length_wool_tunica", Name: "assemble a Roman knee-length wool tunica", DisplayName: "a Roman knee-length wool tunica", Material: "wool", Cloth: 540, Yarn: 25, Fine: false, Minimum: 15, Difficulty: Difficulty.Easy),
			(StableReference: "antiquity_wool_travel_mantle", Name: "assemble a Roman wool travel mantle", DisplayName: "a Roman wool travel mantle", Material: "wool", Cloth: 920, Yarn: 35, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_linen_tunica", Name: "assemble a Roman fine linen tunica", DisplayName: "a Roman fine linen tunica", Material: "linen", Cloth: 360, Yarn: 25, Fine: true, Minimum: 35, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_wool_toga", Name: "assemble a Roman wool toga", DisplayName: "a Roman wool toga", Material: "wool", Cloth: 2800, Yarn: 90, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_long_wool_tunica", Name: "assemble a Roman long wool tunica", DisplayName: "a Roman long wool tunica", Material: "wool", Cloth: 650, Yarn: 30, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_wool_palla", Name: "assemble a Roman wool palla", DisplayName: "a Roman wool palla", Material: "wool", Cloth: 870, Yarn: 35, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_long_linen_tunica", Name: "assemble a Roman fine long linen tunica", DisplayName: "a Roman fine long linen tunica", Material: "linen", Cloth: 400, Yarn: 30, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_wool_stola", Name: "assemble a Roman wool stola", DisplayName: "a Roman wool stola", Material: "wool", Cloth: 860, Yarn: 40, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_fine_wool_palla", Name: "assemble a Roman fine wool palla", DisplayName: "a Roman fine wool palla", Material: "wool", Cloth: 820, Yarn: 35, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard)
		})
		{
			var characteristicRequirements = garment.Fine
				? "characteristic Colour any; characteristic Fine Colour any"
				: "characteristic Colour any";
			AddRomanGarmentCraft(
				garment.Name,
				$"assemble {garment.DisplayName} from {garment.Material} garment cloth",
				garment.Name,
				$"{garment.Material} cloth being assembled into Roman clothing",
				garment.Minimum,
				garment.Difficulty,
				romanAssemblyPhases,
				[
					$"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
					$"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
				],
				sewingTools,
				[StableVariableProduct(garment.StableReference, garment.Fine)]);
		}
	}
}
