using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AncientHouseholdCraftingKnowledge = "Ancient Household Crafting";
	private const string AncientWoodworkingKnowledge = "Ancient Woodworking and Joinery";
	private const string AncientBasketryKnowledge = "Ancient Basketry";
	private const string AncientCooperingKnowledge = "Ancient Coopering";
	private const string AncientLeatherContainersKnowledge = "Ancient Leather Containers";
	private const string AncientCeramicVesselmakingKnowledge = "Ancient Ceramic Vesselmaking";
	private const string AncientGlassworkingKnowledge = "Ancient Glassworking";
	private const string AncientMetalVesselmakingKnowledge = "Ancient Metal Vesselmaking";
	private const string AncientStoneCarvingKnowledge = "Ancient Stone Bone and Horn Carving";
	private const string AncientLightingKnowledge = "Ancient Lighting and Heating";

	private static readonly string[] AntiquityHouseholdCraftFunctionalRoots =
	[
		"Functions / Household Items",
		"Functions / Writing Goods"
	];

	private sealed record AntiquityHouseholdCraftPath(
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

	private enum AntiquityHouseholdVariableStyle
	{
		None,
		BasicColour,
		FineColour,
		TwoColour
	}

	private enum AntiquityHouseholdVariableSource
	{
		None,
		PrimaryStock,
		ClothPanel,
		LeatherPanel,
		ReedMatting,
		PaintPigment,
		LacquerFinish
	}

	private static readonly string[] AntiquityHouseholdWoodMaterials =
	[
		"acacia", "ash", "beech", "birch", "boxwood", "cedar", "cypress", "ebony", "oak", "pine", "sycamore",
		"walnut", "willow", "wood"
	];

	private static readonly string[] AntiquityHouseholdTextileMaterials =
	[
		"canvas", "felt", "hemp", "linen", "papyrus", "silk", "wool"
	];

	private static readonly string[] AntiquityHouseholdLeatherMaterials =
	[
		"deer leather", "fur", "leather"
	];

	private static readonly string[] AntiquityHouseholdCeramicMaterials =
	[
		"ceramic", "earthenware", "fired clay", "terracotta", "tile"
	];

	private static readonly string[] AntiquityHouseholdGlassMaterials =
	[
		"glass", "soda-lime glass"
	];

	private static readonly string[] AntiquityHouseholdMetalMaterials =
	[
		"bronze", "gold", "silver", "wrought iron"
	];

	private static readonly string[] AntiquityHouseholdStoneHornMaterials =
	[
		"alabaster", "horn", "ivory"
	];

	private static readonly IReadOnlyDictionary<string, string> AntiquityHouseholdCultureKnowledgeNames =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["Hellenic"] = "Hellenic Household Crafting",
			["Roman"] = "Roman Household Crafting",
			["Egyptian"] = "Egyptian Household Crafting",
			["Kushite"] = "Kushite Household Crafting",
			["Punic"] = "Punic Household Crafting",
			["Persian"] = "Persian Household Crafting",
			["Etruscan"] = "Etruscan Household Crafting",
			["Anatolian"] = "Anatolian Household Crafting",
			["Scythian-Sarmatian"] = "Scythian-Sarmatian Household Crafting",
			["Celtic"] = "Celtic Household Crafting",
			["Germanic"] = "Germanic Household Crafting"
		};

	private void AddAntiquityHouseholdCommodityCraft(string name, string category, string traitName, string knowledgeName,
		string knowledgeSubtype, string blurb, IEnumerable<string> inputs, IEnumerable<string> tools,
		IEnumerable<string> products)
	{
		AddCraft(
			name,
			category,
			blurb,
			name,
			$"{blurb} in progress",
			knowledgeName,
			traitName,
			15,
			Difficulty.Easy,
			Outcome.MinorFail,
			5,
			3,
			false,
			AntiquityHouseholdIntermediatePhases(),
			inputs,
			tools,
			products,
			[],
			knowledgeSubtype: knowledgeSubtype,
			knowledgeDescription: $"{knowledgeName} covers preparing stock commodities for antiquity household crafts.",
			knowledgeLongDescription: $"{knowledgeName} covers preparing stock commodities for antiquity household crafts.");
	}

	private static (int Seconds, string Echo, string FailEcho)[] AntiquityHouseholdIntermediatePhases()
	{
		return
		[
			(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1, but misjudge|misjudges the usable pieces."),
			(35, "$0 work|works the material into a consistent prepared stock.", "$0 work|works the material unevenly."),
			(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.")
		];
	}

	private string GetMaterialName(GameItemProto item)
	{
		return _materials.First(x => x.Value.Id == item.MaterialId).Key;
	}

	private IEnumerable<string> GetItemComponentNames(GameItemProto item)
	{
		foreach (var component in item.GameItemProtosGameItemComponentProtos)
		{
			if (component.GameItemComponent is not null)
			{
				yield return component.GameItemComponent.Name;
				continue;
			}

			var componentName = _components.Values
				.FirstOrDefault(x => x.Id == component.GameItemComponentProtoId && x.RevisionNumber == component.GameItemComponentRevision)
				?.Name;
			if (componentName is not null)
			{
				yield return componentName;
			}
		}
	}

	private static AntiquityHouseholdVariableStyle GetHouseholdVariableStyle(GameItemProto item,
		IReadOnlyCollection<string> componentNames)
	{
		if (componentNames.Any(x => x.StartsWith("Variable_2", StringComparison.OrdinalIgnoreCase)))
		{
			return AntiquityHouseholdVariableStyle.TwoColour;
		}

		if (componentNames.Contains("Variable_FineColour", StringComparer.OrdinalIgnoreCase))
		{
			return AntiquityHouseholdVariableStyle.FineColour;
		}

		if (componentNames.Contains("Variable_BasicColour", StringComparer.OrdinalIgnoreCase) ||
		    item.ShortDescription.Contains("$colour", StringComparison.OrdinalIgnoreCase))
		{
			return AntiquityHouseholdVariableStyle.BasicColour;
		}

		return AntiquityHouseholdVariableStyle.None;
	}

	private static string CommodityInput(double grams, string material, string? pileTag = null, bool colour = false,
		bool fineColour = false)
	{
		var text = $"Commodity - {FormatCommodityAmount(grams)} of {material}";
		if (!string.IsNullOrWhiteSpace(pileTag))
		{
			text += $"; piletag {pileTag}";
		}

		if (colour)
		{
			text += "; characteristic Colour any";
		}

		if (fineColour)
		{
			text += "; characteristic Fine Colour any";
		}

		return text;
	}

	private static string CommodityPileInput(double grams, string pileTag, bool colour = false, bool fineColour = false)
	{
		var text = $"CommodityTag - {FormatCommodityAmount(grams)} of a material tagged as Materials; piletag {pileTag}";
		if (colour)
		{
			text += "; characteristic Colour any";
		}

		if (fineColour)
		{
			text += "; characteristic Fine Colour any";
		}

		return text;
	}

	private static string FormatCommodityAmount(double grams)
	{
		var rounded = Math.Max(1, (int)Math.Ceiling(grams / 5.0) * 5);
		var kilograms = rounded / 1000;
		var remainingGrams = rounded % 1000;
		var parts = new List<string>();
		if (kilograms > 0)
		{
			parts.Add(kilograms == 1 ? "1 kilogram" : $"{kilograms} kilograms");
		}

		if (remainingGrams > 0 || parts.Count == 0)
		{
			parts.Add(remainingGrams == 1 ? "1 gram" : $"{remainingGrams} grams");
		}

		return string.Join(" ", parts);
	}

	private static string SanitiseHouseholdCraftDisplayName(string shortDescription)
	{
		return shortDescription
			.Replace("$colour1", "coloured", StringComparison.OrdinalIgnoreCase)
			.Replace("$colour2", "coloured", StringComparison.OrdinalIgnoreCase)
			.Replace("$colour", "coloured", StringComparison.OrdinalIgnoreCase);
	}

	private static bool ContainsAny(string text, params string[] values)
	{
		return values.Any(value => text.Contains(value, StringComparison.OrdinalIgnoreCase));
	}
}
