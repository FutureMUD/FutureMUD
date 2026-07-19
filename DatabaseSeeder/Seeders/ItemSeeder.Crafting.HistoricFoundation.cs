#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string HistoricFoundationKnowledge = "Historic Workshop Foundations";

	private bool ShouldSeedHistoricCrafts()
	{
		return _questionAnswers?.TryGetValue("eras", out var eras) == true &&
		       (eras.Contains("antiquity", StringComparison.InvariantCultureIgnoreCase) ||
		        eras.Contains("medieval", StringComparison.InvariantCultureIgnoreCase));
	}

	private Craft? AddHistoricCraft(
		string name,
		string category,
		string blurb,
		string action,
		string itemDescription,
		string traitName,
		int? minimumTraitValue,
		Difficulty difficulty,
		IEnumerable<(int Seconds, string Echo, string FailEcho)> phases,
		IEnumerable<string> inputs,
		IEnumerable<string> tools,
		IEnumerable<string> products,
		IEnumerable<string>? failProducts = null,
		string knowledgeSubtype = "Foundations")
	{
		return AddCraft(
			name,
			category,
			blurb,
			action,
			itemDescription,
			HistoricFoundationKnowledge,
			traitName,
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
			failProducts ?? [],
			knowledgeSubtype: knowledgeSubtype,
			knowledgeDescription: "Shared historic workshop foundations for antiquity and medieval installs.",
			knowledgeLongDescription: "Shared historic workshop foundations for cross-era stock such as hearths, looms, kilns, shears, awls, querns, and general apparatus.");
	}

	private static (int Seconds, string Echo, string FailEcho)[] HistoricFoundationFinishedPhases()
	{
		return
		[
			(30, "$0 lay|lays out the prepared stock and check|checks it for flaws.", "$0 lay|lays out the prepared stock, but miss|misses several serious flaws."),
			(45, "$0 shape|shapes, trim|trims, and fit|fits the working pieces together.", "$0 shape|shapes the working pieces poorly, leaving awkward joins and weak points."),
			(45, "$0 bind|binds, stitch|stitches, rivet|rivets, or finish|finishes the main assembly.", "$0 botch|botches the finishing work and spoil|spoils the assembly."),
			(30, "$0 set|sets aside $p1 and inspect|inspects the finished work.", "$0 set|sets aside only $f1 after the work fails.")
		];
	}

	private static (int Seconds, string Echo, string FailEcho)[] HistoricFoundationLightingPhases()
	{
		return
		[
			(20, "$0 prepare|prepares the fuel, wick, or charcoal bed.", "$0 prepare|prepares the fuel poorly, scattering it around the work area."),
			(25, "$0 coax|coaxes the flame into the prepared item.", "$0 coax|coaxes the flame badly, and it will not take."),
			(20, "$0 set|sets aside $p1 once the flame is steady.", "$0 end|ends up with only $f1 after the flame fails.")
		];
	}

	private static string VisibleCraftName(string shortDescription)
	{
		return StripLeadingArticle(shortDescription).ToLowerInvariant();
	}

	private void SeedHistoricFoundationCrafts()
	{
		if (!ShouldSeedHistoricCrafts())
		{
			return;
		}

		var specs = HistoricFoundationItemSpecs();
		foreach (var spec in specs.Where(x => string.IsNullOrWhiteSpace(x.MorphToUniqueReference)))
		{
			var (category, trait, inputs, tools, difficulty) = GetHistoricFoundationCraftPath(spec);
			var visibleName = VisibleCraftName(spec.ShortDescription);
			AddHistoricCraft(
				$"make {visibleName}",
				category,
				$"make {visibleName}",
				$"making {visibleName}",
				$"{visibleName} under construction",
				trait,
				15,
				difficulty,
				HistoricFoundationFinishedPhases(),
				inputs,
				tools,
				[StableSimpleProduct(spec.StableReference)],
				knowledgeSubtype: "Workshop Apparatus");
		}

		foreach (var spec in specs.Where(x => !string.IsNullOrWhiteSpace(x.MorphToUniqueReference)))
		{
			var visibleName = VisibleCraftName(spec.ShortDescription);
			AddHistoricCraft(
				$"light {visibleName}",
				"Cooking",
				$"light {visibleName}",
				$"lighting {visibleName}",
				$"{visibleName} being lit",
				"Cooking",
				10,
				Difficulty.Easy,
				HistoricFoundationLightingPhases(),
				[
					StableSimpleItemInput(spec.MorphToUniqueReference!),
					spec.StableReference.Contains("lamp", StringComparison.OrdinalIgnoreCase)
						? CommodityInput(80.0, "linen", "Spun Yarn", colour: true)
						: CommodityInput(600.0, "oak", "Tool Blank Stock")
				],
				[],
				[StableSimpleProduct(spec.StableReference)],
				[StableUnusedInputProduct(spec.MorphToUniqueReference!, 1)],
				knowledgeSubtype: "Lighting");
		}
	}

	private (string Category, string Trait, IReadOnlyList<string> Inputs, IReadOnlyList<string> Tools, Difficulty Difficulty)
		GetHistoricFoundationCraftPath(EraItemSpec spec)
	{
		if (spec.Components.Any(x => x.Contains("Lantern", StringComparison.OrdinalIgnoreCase)))
		{
			return ("Pottery", "Pottery",
				[CommodityInput(260.0, spec.Material, "Pottery Clay Body")],
				["TagTool - InRoom - an item with the Hot Fire tag"],
				Difficulty.Easy);
		}

		return spec.MaterialType switch
		{
			MaterialBehaviourType.Ceramic => ("Pottery", "Pottery",
				[CommodityInput(Math.Max(500.0, spec.WeightInGrams * 0.25), spec.Material, "Pottery Clay Body")],
				["TagTool - InRoom - an item with the Hot Fire tag"],
				Difficulty.Normal),
			MaterialBehaviourType.Wood => ("Carpentry", "Carpentry",
				[CommodityInput(Math.Max(500.0, spec.WeightInGrams * 0.20), spec.Material, "Furniture Timber Stock")],
				["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"],
				Difficulty.Normal),
			MaterialBehaviourType.Metal => ("Blacksmithing", "Blacksmithing",
				[CommodityInput(Math.Max(300.0, spec.WeightInGrams * 0.20), spec.Material, "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - Held - an item with the Forge Tongs tag"
				],
				Difficulty.Hard),
			MaterialBehaviourType.Leather => ("Leathermaking", "Leathermaking",
				[CommodityInput(Math.Max(300.0, spec.WeightInGrams * 0.50), spec.Material, "Prepared Leather Panel", colour: true, fineColour: true)],
				["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Shears tag"],
				Difficulty.Normal),
			_ => ("Crafting", "Tailoring",
				[CommodityInput(Math.Max(200.0, spec.WeightInGrams * 0.50), spec.Material, "Tool Blank Stock")],
				["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"],
				Difficulty.Normal)
		};
	}
}