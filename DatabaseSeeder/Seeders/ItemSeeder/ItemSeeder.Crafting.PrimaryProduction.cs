#nullable enable

using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string PrimaryProductionKnowledge = "Primary Production - Historic Commodity Work";

	internal sealed record PrimaryProductionCraftSpecTestData(
		string Name,
		string Category,
		string Trait,
		string KnowledgeSubtype,
		IReadOnlyCollection<string> Inputs,
		IReadOnlyCollection<string> Tools,
		IReadOnlyCollection<string> Products,
		IReadOnlyCollection<string> FailProducts);

	private sealed record PrimaryProductionCraftSpec(
		string Name,
		string Category,
		string Blurb,
		string Action,
		string ActiveDescription,
		string Trait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		string KnowledgeSubtype,
		IReadOnlyList<string> Inputs,
		IReadOnlyList<string> Tools,
		IReadOnlyList<string> Products,
		IReadOnlyList<string> FailProducts);

	internal static IReadOnlyCollection<PrimaryProductionCraftSpecTestData> PrimaryProductionCraftSpecsForTesting =>
		ProductionPlanStock.PrimaryProduction().Select(x => new PrimaryProductionCraftSpecTestData(x.Name, x.Category,
			x.Trait, x.KnowledgeSubtype, x.Inputs.ToArray(), x.Tools.ToArray(), x.Products.ToArray(), x.FailProducts.ToArray())).ToArray();

	internal static IReadOnlyCollection<PrimaryProductionCraftSpecTestData> BlackPowderCraftSpecsForTesting =>
		ProductionPlanStock.BlackPowder().Select(x => new PrimaryProductionCraftSpecTestData(x.Name, x.Category,
			x.Trait, x.KnowledgeSubtype, x.Inputs.ToArray(), x.Tools.ToArray(), x.Products.ToArray(), x.FailProducts.ToArray())).ToArray();

	private void SeedBlackPowderCrafts()
	{
		foreach (var spec in BlackPowderCraftSpecs())
		{
			AddCraft(
				spec.Name,
				spec.Category,
				spec.Blurb,
				spec.Action,
				spec.ActiveDescription,
				"Gunpowder Manufacture - Corning Black Powder",
				spec.Trait,
				spec.MinimumTraitValue,
				spec.Difficulty,
				Outcome.MinorFail,
				5,
				3,
				false,
				BlackPowderCraftPhases(),
				spec.Inputs,
				spec.Tools,
				spec.Products,
				spec.FailProducts,
				knowledgeSubtype: spec.KnowledgeSubtype,
				knowledgeDescription: "Safe preparation of measured black-powder ingredients.",
				knowledgeLongDescription: "Practical knowledge for weighing, separately milling, damp-incorporating, and corning saltpeter, charcoal, and sulfur into physical gunpowder commodity stock.");
		}
	}

	private static IReadOnlyList<PrimaryProductionCraftSpec> BlackPowderCraftSpecs()
	{
		return
		[
			new PrimaryProductionCraftSpec(
				"mill and corn black powder",
				"Powdermaking",
				"weigh, mill, damp-incorporate, and corn a batch of black powder",
				"milling and corning black powder",
				"an in-progress batch of black powder",
				"Powdermaking",
				25,
				Difficulty.Hard,
				"Gunpowder Manufacture",
				[
					CommodityInput(750.0, "saltpeter", "Saltpeter Commodity"),
					CommodityInput(150.0, "charcoal", "Charcoal Fuel Commodity"),
					CommodityInput(100.0, "sulfur"),
					WaterUse(0.25)
				],
				[HeldTool("Mortar and Pestle")],
				[CommodityOutput(900.0, "gunpowder", "Gunpowder Commodity")],
				[])
		];
	}

	private static (int Seconds, string Echo, string FailEcho)[] BlackPowderCraftPhases()
	{
		return
		[
			(30,
				"$0 carefully weigh|weighs $i1, $i2, and $i3 into separate portions.",
				"$0 mismeasure|mismeasures the separate portions of $i1, $i2, and $i3."),
			(60,
				"$0 grind|grinds each dry ingredient separately with $t1, keeping the powders apart.",
				"$0 grind|grinds the ingredients unevenly with $t1, leaving coarse material through the batch."),
			(60,
				"$0 dampen|dampens the powders with $i4 and carefully incorporate|incorporates them into a uniform paste.",
				"$0 dampen|dampens the powders with $i4, but fail|fails to incorporate the mixture evenly."),
			(45,
				"$0 break|breaks the damp cake into even grains and set|sets aside $p1 to dry.",
				"$0 break|breaks the damp cake into irregular grains, spoiling the batch before it can be dried.")
		];
	}

	private static PrimaryProductionCraftSpec Define(
		string name,
		string category,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		string knowledgeSubtype,
		string blurb,
		IReadOnlyList<string> inputs,
		IReadOnlyList<string> tools,
		IReadOnlyList<string> products,
		IReadOnlyList<string>? failProducts = null)
	{
		return new PrimaryProductionCraftSpec(
			name,
			category,
			blurb,
			name,
			$"{name} in progress",
			trait,
			minimumTraitValue,
			difficulty,
			knowledgeSubtype,
			inputs,
			tools,
			products,
			failProducts ?? []);
	}

	private static string CommodityOutput(double grams, string material, string tag)
	{
		return $"CommodityProduct - {FormatCommodityAmount(grams)} of {material} commodity; tag {tag}";
	}

	private static string HeldTool(string tag)
	{
		return $"TagTool - Held - an item with the {tag} tag";
	}

	private static string WaterUse(double litres)
	{
		return $"LiquidUse - {litres.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)} litres of Water";
	}
}
