using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AncientEquipmentCraftingKnowledge = "Ancient Equipment Crafting";
	private const string AncientWeaponcraftingKnowledge = "Ancient Weaponcrafting";
	private const string AncientArmourcraftingKnowledge = "Ancient Armourcrafting";
	private const string AncientToolmakingKnowledge = "Ancient Toolmaking";
	private const string AncientCommonClothingKnowledge = "Ancient Common Clothing Crafting";

	private static readonly string[] AntiquityMilitaryCraftFunctionalRoots =
	[
		"Functions / Military Equipment"
	];

	private static readonly string[] AntiquityCraftToolFunctionalRoots =
	[
		"Functions / Tools",
		"Functions / Separation",
		"Functions / Joining",
		"Functions / Sharpening"
	];

	private static readonly string[] AntiquityCommonClothingStableReferences =
	[
		"antiquity_linen_loincloth",
		"antiquity_linen_breastband",
		"antiquity_simple_woven_sash",
		"antiquity_fine_woven_sash",
		"antiquity_papyrus_sandals",
		"antiquity_fine_papyrus_sandals",
		"antiquity_wrapped_linen_headcloth",
		"antiquity_fine_linen_headcloth",
		"antiquity_linen_shoulder_shawl",
		"antiquity_fine_linen_shoulder_shawl",
		"antiquity_front_knotted_girdle",
		"antiquity_fine_front_knotted_girdle",
		"antiquity_conical_felt_cap",
		"antiquity_fine_conical_felt_cap",
		"antiquity_rounded_felt_cap",
		"antiquity_tall_kyrbasia",
		"antiquity_fluted_felt_hat",
		"antiquity_glass_usekh_collar"
	];

	private static readonly string[] AntiquityCraftToolStableReferences =
	[
		"antiquity_bronze_textile_shears",
		"antiquity_bronze_sewing_needle",
		"antiquity_retting_trough",
		"antiquity_flax_break",
		"antiquity_fibre_hackle",
		"antiquity_drop_spindle",
		"antiquity_distaff",
		"antiquity_warp_weighted_loom",
		"antiquity_clay_loom_weight",
		"antiquity_weaving_shuttle",
		"antiquity_weavers_sword",
		"antiquity_dye_vat",
		"antiquity_mordant_cauldron",
		"antiquity_fullers_trough",
		"antiquity_fullers_mallet",
		"antiquity_tenter_frame",
		"antiquity_bronze_awl_punch",
		"antiquity_oak_stitching_pony",
		"antiquity_bronze_edge_beveller",
		"antiquity_bronze_leather_gouge",
		"antiquity_bronze_leather_creaser",
		"antiquity_oak_shoe_last",
		"antiquity_bronze_leather_wax_pot",
		"antiquity_bronze_hide_scraper",
		"antiquity_oak_tanning_beam",
		"antiquity_oak_tanning_rack",
		"antiquity_oak_tanning_paddle",
		"antiquity_bronze_dehairing_knife",
		"antiquity_oak_brain_tanning_bucket"
	];

	private static readonly string[] AntiquityEquipmentMetalMaterials =
	[
		"bronze",
		"lead",
		"wrought iron"
	];

	private static readonly string[] AntiquityEquipmentWoodMaterials =
	[
		"acacia",
		"ash",
		"cedar",
		"cypress",
		"oak",
		"pine",
		"sycamore",
		"willow",
		"wood",
		"yew"
	];

	private static readonly string[] AntiquityEquipmentTextileMaterials =
	[
		"canvas",
		"felt",
		"hemp",
		"linen",
		"papyrus",
		"wool"
	];

	private static readonly string[] AntiquityEquipmentLeatherMaterials =
	[
		"cow hide",
		"deer leather",
		"fur",
		"leather"
	];

	private static readonly string[] AntiquityEquipmentHardMaterials =
	[
		"stone"
	];

	private static readonly string[] AntiquityEquipmentCeramicMaterials =
	[
		"earthenware",
		"fired clay",
		"glazed ceramic",
		"terracotta"
	];

	private static readonly string[] AntiquityEquipmentToolBlankMaterials =
	[
		"bone",
		"glass",
		"horn",
		"ivory",
		"shell",
		"stone"
	];

	private static readonly string[] AntiquityUnlitWorkshopApparatusStableReferences =
	[
		"antiquity_workshop_hearth",
		"antiquity_updraft_kiln",
		"antiquity_glory_hole_furnace",
		"antiquity_annealing_lehr",
		"antiquity_clay_smelting_furnace"
	];

	private static readonly string[] AntiquityLitWorkshopApparatusStableReferences =
	[
		"antiquity_lit_workshop_hearth",
		"antiquity_lit_updraft_kiln",
		"antiquity_lit_glory_hole_furnace",
		"antiquity_lit_annealing_lehr",
		"antiquity_lit_clay_smelting_furnace"
	];

	private static readonly string[] AntiquityEquipmentCultureTerms =
	[
		"achaemenid",
		"anatolian",
		"athenian",
		"campanian",
		"carthaginian",
		"celtic",
		"corinthian",
		"egyptian",
		"etruscan",
		"gallic",
		"germanic",
		"greek",
		"hellenic",
		"kushite",
		"lydian",
		"meroitic",
		"nubian",
		"parthian",
		"persian",
		"phrygian",
		"punic",
		"roman",
		"sarmatian",
		"scythian",
		"spartan",
		"urartian"
	];

	private enum AntiquityEquipmentCraftFamily
	{
		CommonClothing,
		CraftTool,
		Armour,
		Shield,
		Ammunition,
		Bow,
		Sling,
		Spear,
		Blade,
		HaftedWeapon,
		WoodenWeapon,
		GeneralWeapon
	}

	private sealed record AntiquityEquipmentCraftPath(
		AntiquityEquipmentCraftFamily Family,
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

	private void AddAntiquityHeatSourceCraft(string name, string category, string traitName, string knowledgeName,
		string knowledgeSubtype, string blurb, string unlitStableReference, string litStableReference, double charcoalGrams,
		int minimumTraitValue, Difficulty difficulty)
	{
		AddAntiquityCraft(
			name,
			category,
			blurb,
			name,
			$"{blurb} in progress",
			knowledgeName,
			traitName,
			minimumTraitValue,
			difficulty,
			AntiquityHeatSourcePhases(),
			[
				StableSimpleItemInput(unlitStableReference),
				CommodityInput(charcoalGrams, "charcoal")
			],
			[],
			[StableSimpleProduct(litStableReference)],
			[StableUnusedInputProduct(unlitStableReference, 1)],
			knowledgeSubtype: knowledgeSubtype,
			knowledgeDescription: $"{knowledgeName} covers lighting, stoking, and maintaining reusable antiquity workshop heat sources.",
			knowledgeLongDescription: $"{knowledgeName} covers lighting, stoking, and maintaining reusable antiquity workshop heat sources.");
	}

	private static (int Seconds, string Echo, string FailEcho)[] AntiquityHeatSourcePhases()
	{
		return
		[
			(20, "$0 clear|clears ash from $i1 and lay|lays $i2 into the fire bed.", "$0 clear|clears ash from $i1, but pack|packs the charcoal poorly."),
			(35, "$0 coax|coaxes the charcoal into a steady working heat.", "$0 coax|coaxes the charcoal, but the heat gutters and chokes."),
			(25, "$0 settle|settles $p1 into a usable heat.", "$0 lose|loses the heat before the work is ready, leaving only $f1 usable.")
		];
	}

	private void SeedAntiquityEquipmentFinishedCraft(string stableReference, GameItemProto item,
		AntiquityEquipmentCraftPath path, IDictionary<string, int> usedCraftNames)
	{
		var material = GetMaterialName(item);
		var componentNames = GetItemComponentNames(item).ToList();
		var variableStyle = GetHouseholdVariableStyle(item, componentNames);
		var (inputs, variableInputIndex) =
			BuildAntiquityEquipmentFinalInputs(stableReference, item, material, path, variableStyle);
		var products = new[]
		{
			BuildAntiquityEquipmentProduct(item, variableStyle, variableInputIndex)
		};
		var visibleName = SanitiseAntiquityEquipmentVisibleName(item.ShortDescription);
		var craftName = BuildUniqueVisibleCraftName(usedCraftNames, "make",
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
			GetAntiquityEquipmentFinalPhases(path),
			inputs,
			path.Tools,
			products,
			knowledgeSubtype: path.KnowledgeSubtype,
			knowledgeDescription: $"{path.Knowledge} covers antiquity {path.KnowledgeSubtype.ToLowerInvariant()} without tying visible craft text to a culture name.",
			knowledgeLongDescription: $"{path.Knowledge} covers antiquity {path.KnowledgeSubtype.ToLowerInvariant()} without tying visible craft text to a culture name.");
	}

	private (List<string> Inputs, int VariableInputIndex) BuildAntiquityEquipmentFinalInputs(string stableReference,
		GameItemProto item, string material, AntiquityEquipmentCraftPath path,
		AntiquityHouseholdVariableStyle variableStyle)
	{
		var inputs = new List<string>();
		var variableInputIndex = 1;
		var needsColour = variableStyle != AntiquityHouseholdVariableStyle.None;
		var primaryAmount = Math.Max(30.0, item.Weight);
		var lowerText = $"{stableReference} {item.ShortDescription}".ToLowerInvariant();

		void AddInput(string input, bool variableSource = false)
		{
			inputs.Add(input);
			if (variableSource)
			{
				variableInputIndex = inputs.Count;
			}
		}

		switch (path.Family)
		{
			case AntiquityEquipmentCraftFamily.CommonClothing:
				BuildCommonClothingInputs(inputs, AddInput, material, lowerText, primaryAmount, needsColour);
				break;
			case AntiquityEquipmentCraftFamily.CraftTool:
				BuildCraftToolInputs(inputs, AddInput, material, lowerText, primaryAmount, path.PrimaryStockTag);
				break;
			case AntiquityEquipmentCraftFamily.Shield:
				AddInput(CommodityInput(primaryAmount * 0.65, GetEquipmentWoodMaterial(material, lowerText), "Shield Board Stock"));
				AddInput(CommodityInput(Math.Max(450.0, primaryAmount * 0.25), "leather", "Shield Facing Stock",
					colour: needsColour, fineColour: needsColour), needsColour);
				AddInput(CommodityInput(250.0, GetEquipmentMetalMaterial(material, lowerText), "Weapon Head Stock"));
				AddInput(CommodityInput(120.0, "leather", "Leather Strap", colour: true, fineColour: true));
				break;
			case AntiquityEquipmentCraftFamily.Ammunition:
				BuildAmmunitionInputs(inputs, AddInput, material, lowerText, primaryAmount);
				break;
			case AntiquityEquipmentCraftFamily.Bow:
				AddInput(CommodityInput(primaryAmount, GetEquipmentWoodMaterial(material, lowerText), "Bow Stave"));
				AddInput(CommodityInput(100.0, "linen", "Military Cord Stock", colour: true, fineColour: true));
				AddInput(CommodityInput(80.0, "leather", "Leather Strap", colour: true, fineColour: true), needsColour);
				break;
			case AntiquityEquipmentCraftFamily.Sling:
				AddInput(CommodityInput(primaryAmount, material, IsLeatherEquipmentMaterial(material) ? "Leather Thong" : "Military Cord Stock",
					colour: needsColour, fineColour: needsColour), needsColour);
				AddInput(CommodityInput(60.0, "linen", "Military Cord Stock", colour: true, fineColour: true));
				break;
			case AntiquityEquipmentCraftFamily.Spear:
				AddInput(CommodityInput(primaryAmount * 0.35, GetEquipmentMetalMaterial(material, lowerText), "Weapon Head Stock"));
				AddInput(CommodityInput(primaryAmount * 0.65, GetEquipmentWoodMaterial(material, lowerText), "Weapon Shaft Stock"));
				AddInput(CommodityInput(80.0, "linen", "Military Cord Stock", colour: true, fineColour: true), needsColour);
				break;
			case AntiquityEquipmentCraftFamily.Blade:
				AddInput(CommodityInput(primaryAmount, GetEquipmentMetalMaterial(material, lowerText), "Weapon Blade Stock"));
				AddInput(CommodityInput(160.0, GetEquipmentWoodMaterial(material, lowerText), "Tool Blank Stock"));
				AddInput(CommodityInput(90.0, "leather", "Leather Strap", colour: true, fineColour: true), needsColour);
				break;
			case AntiquityEquipmentCraftFamily.HaftedWeapon:
				AddInput(CommodityInput(primaryAmount * 0.55, GetEquipmentMetalMaterial(material, lowerText), "Weapon Head Stock"));
				AddInput(CommodityInput(primaryAmount * 0.45, GetEquipmentWoodMaterial(material, lowerText), "Weapon Shaft Stock"));
				AddInput(CommodityInput(100.0, "linen", "Military Cord Stock", colour: true, fineColour: true), needsColour);
				break;
			case AntiquityEquipmentCraftFamily.WoodenWeapon:
				AddInput(CommodityInput(primaryAmount, GetEquipmentWoodMaterial(material, lowerText), "Weapon Shaft Stock"));
				AddInput(CommodityInput(120.0, "leather", "Leather Strap", colour: true, fineColour: true), needsColour);
				break;
			case AntiquityEquipmentCraftFamily.Armour:
				BuildArmourInputs(inputs, AddInput, material, lowerText, primaryAmount, path.PrimaryStockTag, needsColour);
				break;
			default:
				AddInput(CommodityInput(primaryAmount, material, path.PrimaryStockTag,
					colour: needsColour, fineColour: needsColour), needsColour);
				AddInput(CommodityInput(90.0, "linen", "Military Cord Stock", colour: true, fineColour: true), needsColour);
				break;
		}

		return (inputs, variableInputIndex);
	}

	private static void BuildCommonClothingInputs(List<string> inputs, Action<string, bool> addInput,
		string material, string lowerText, double primaryAmount, bool needsColour)
	{
		if (material.Equals("glass", StringComparison.OrdinalIgnoreCase))
		{
			addInput(CommodityInput(primaryAmount, "glass", "Bead Stock", colour: true, fineColour: true), true);
			addInput(CommodityInput(60.0, "linen", "Military Cord Stock", colour: true, fineColour: true), false);
			return;
		}

		if (material.Equals("papyrus", StringComparison.OrdinalIgnoreCase))
		{
			addInput(CommodityInput(primaryAmount, "papyrus", "Basketry Splint",
				colour: needsColour, fineColour: needsColour), needsColour);
			addInput(CommodityInput(80.0, "linen", "Spun Yarn", colour: true, fineColour: true), false);
			return;
		}

		var textileMaterial = GetEquipmentTextileMaterial(material, lowerText);
		addInput(CommodityInput(primaryAmount, textileMaterial,
			material.Equals("felt", StringComparison.OrdinalIgnoreCase) ? "Fulled Cloth" : "Garment Cloth",
			colour: true, fineColour: true), true);
		addInput(CommodityInput(80.0, textileMaterial, "Spun Yarn", colour: true, fineColour: true), false);

		if (lowerText.Contains("sandal", StringComparison.OrdinalIgnoreCase))
		{
			addInput(CommodityInput(160.0, "leather", "Leather Sole", colour: true, fineColour: true), false);
		}
	}

	private static void BuildCraftToolInputs(List<string> inputs, Action<string, bool> addInput,
		string material, string lowerText, double primaryAmount, string primaryStockTag)
	{
		if (!primaryStockTag.Equals("Tool Blank Stock", StringComparison.OrdinalIgnoreCase))
		{
			addInput(CommodityInput(primaryAmount, material, primaryStockTag), false);
			if (ContainsAny(lowerText, "hearth", "kiln", "furnace", "lehr"))
			{
				addInput(CommodityInput(Math.Max(500.0, primaryAmount * 0.15), "clay"), false);
				addInput(CommodityInput(Math.Max(250.0, primaryAmount * 0.05), "charcoal"), false);
			}

			return;
		}

		if (IsWoodEquipmentMaterial(material))
		{
			addInput(CommodityInput(primaryAmount, material, "Tool Blank Stock"), false);
		}
		else if (IsMetalEquipmentMaterial(material))
		{
			addInput(CommodityInput(primaryAmount, material, "Tool Blank Stock"), false);
		}
		else if (IsTextileEquipmentMaterial(material))
		{
			addInput(CommodityInput(primaryAmount, material, "Tool Blank Stock", colour: true, fineColour: true), false);
		}
		else
		{
			addInput(CommodityInput(primaryAmount, material, "Tool Blank Stock"), false);
		}

		if (!IsMetalEquipmentMaterial(material) && ContainsAny(lowerText, "bronze", "iron"))
		{
			addInput(CommodityInput(350.0, GetEquipmentMetalMaterial(material, lowerText), "Tool Blank Stock"), false);
		}

		if (!IsWoodEquipmentMaterial(material) && ContainsAny(lowerText, "oak", "wood", "wooden", "board", "frame", "handle"))
		{
			addInput(CommodityInput(900.0, GetEquipmentWoodMaterial(material, lowerText), "Tool Blank Stock"), false);
		}

		if (lowerText.Contains("wax", StringComparison.OrdinalIgnoreCase))
		{
			addInput(CommodityInput(120.0, "beeswax", "Candlemaking Wax"), false);
		}
	}

	private static void BuildAmmunitionInputs(List<string> inputs, Action<string, bool> addInput,
		string material, string lowerText, double primaryAmount)
	{
		if (ContainsAny(lowerText, "bullet", "shot"))
		{
			addInput(CommodityInput(primaryAmount, IsMetalEquipmentMaterial(material) ? material : "stone", "Weapon Head Stock"), false);
			return;
		}

		addInput(CommodityInput(primaryAmount * 0.55, GetEquipmentWoodMaterial(material, lowerText), "Weapon Shaft Stock"), false);
		addInput(CommodityInput(120.0, GetEquipmentMetalMaterial(material, lowerText), "Weapon Head Stock"), false);
		addInput(CommodityInput(50.0, "linen", "Fletching Stock", colour: true, fineColour: true), false);
		addInput(CommodityInput(30.0, "linen", "Military Cord Stock", colour: true, fineColour: true), false);
	}

	private static void BuildArmourInputs(List<string> inputs, Action<string, bool> addInput,
		string material, string lowerText, double primaryAmount, string primaryStockTag, bool needsColour)
	{
		if (primaryStockTag.Equals("Armour Textile Padding", StringComparison.OrdinalIgnoreCase))
		{
			var textile = GetEquipmentTextileMaterial(material, lowerText);
			addInput(CommodityInput(primaryAmount, textile, "Armour Textile Padding", colour: true, fineColour: true), true);
			addInput(CommodityInput(Math.Max(300.0, primaryAmount * 0.25), textile, "Garment Cloth",
				colour: true, fineColour: true), false);
			addInput(CommodityInput(120.0, "linen", "Military Cord Stock", colour: true, fineColour: true), false);
			return;
		}

		var stockMaterial = IsLeatherEquipmentMaterial(material)
			? material
			: GetEquipmentMetalMaterial(material, lowerText);
		addInput(CommodityInput(primaryAmount, stockMaterial, primaryStockTag,
			colour: needsColour && IsLeatherEquipmentMaterial(stockMaterial),
			fineColour: needsColour && IsLeatherEquipmentMaterial(stockMaterial)), needsColour && IsLeatherEquipmentMaterial(stockMaterial));

		if (needsColour && !IsLeatherEquipmentMaterial(stockMaterial))
		{
			addInput(CommodityInput(120.0, "wool", "Military Cord Stock", colour: true, fineColour: true), true);
		}

		if (primaryStockTag.Equals("Armour Scale Stock", StringComparison.OrdinalIgnoreCase) ||
		    primaryStockTag.Equals("Armour Lamella Stock", StringComparison.OrdinalIgnoreCase) ||
		    primaryStockTag.Equals("Armour Ring Stock", StringComparison.OrdinalIgnoreCase))
		{
			addInput(CommodityInput(220.0, "linen", "Military Cord Stock", colour: true, fineColour: true), false);
		}

		addInput(CommodityInput(260.0, "linen", "Armour Textile Padding", colour: true, fineColour: true), false);
		addInput(CommodityInput(160.0, "leather", "Leather Strap", colour: true, fineColour: true), false);
	}

	private static string BuildAntiquityEquipmentProduct(GameItemProto item,
		AntiquityHouseholdVariableStyle variableStyle, int variableInputIndex)
	{
		return variableStyle switch
		{
			AntiquityHouseholdVariableStyle.BasicColour =>
				$"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable Colour=$i{variableInputIndex}",
			AntiquityHouseholdVariableStyle.FineColour =>
				$"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable Colour=$i{variableInputIndex}, Fine Colour=$i{variableInputIndex}",
			AntiquityHouseholdVariableStyle.TwoColour =>
				$"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable Colour=$i{variableInputIndex}, Colour2=$i{variableInputIndex}",
			_ => $"SimpleProduct - 1x {item.ShortDescription} (#{item.Id})"
		};
	}

	private void AddAntiquityEquipmentCommodityCraft(string name, string category, string traitName, string knowledgeName,
		string knowledgeSubtype, string blurb, IEnumerable<string> inputs, IEnumerable<string> tools,
		IEnumerable<string> products)
	{
		AddAntiquityCraft(
			name,
			category,
			blurb,
			name,
			$"{blurb} in progress",
			knowledgeName,
			traitName,
			15,
			Difficulty.Easy,
			AntiquityEquipmentIntermediatePhases(),
			inputs,
			tools,
			products,
			knowledgeSubtype: knowledgeSubtype,
			knowledgeDescription: $"{knowledgeName} covers preparing reusable stock commodities for antiquity equipment crafts.",
			knowledgeLongDescription: $"{knowledgeName} covers preparing reusable stock commodities for antiquity equipment crafts.");
	}

	private static (int Seconds, string Echo, string FailEcho)[] AntiquityEquipmentIntermediatePhases()
	{
		return
		[
			(25, "$0 inspect|inspects $i1 and select|selects usable stock.", "$0 inspect|inspects $i1, but miss|misses flaws in the material."),
			(35, "$0 work|works the stock into a consistent prepared form.", "$0 work|works the stock unevenly."),
			(25, "$0 finish|finishes the prepared stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.")
		];
	}

	private static (int Seconds, string Echo, string FailEcho)[] GetAntiquityEquipmentFinalPhases(
		AntiquityEquipmentCraftPath path)
	{
		return
		[
			(30, "$0 lay|lays out $i1 and check|checks the prepared stock.", "$0 lay|lays out $i1, but overlook|overlooks a flaw in the stock."),
			(40, "$0 work|works through the main shaping and fitting with $t1.", "$0 work|works the piece poorly with $t1."),
			(35, "$0 trim|trims, bind|binds, and finish|finishes the piece into its final form.", "$0 trim|trims and bind|binds the piece, but the fit is poor."),
			(25, "$0 set|sets aside $p1 after a final inspection.", "$0 set|sets aside the work, but it is not worth keeping.")
		];
	}

	private HashSet<long> GetTagIdsUnderRoots(IEnumerable<string> roots)
	{
		var rootList = roots.ToList();
		return _tagsByFullPath
			.Where(x => rootList.Any(root => TagPathMatchesRoot(x.Key, root)))
			.Select(x => x.Value.Id)
			.ToHashSet();
	}

	private static bool TagPathMatchesRoot(string tagPath, string root)
	{
		return tagPath.Equals(root, StringComparison.OrdinalIgnoreCase) ||
		       tagPath.StartsWith($"{root} /", StringComparison.OrdinalIgnoreCase);
	}

	private static string BuildUniqueVisibleCraftName(IDictionary<string, int> usedCraftNames,
		string prefix, string displayName)
	{
		var baseName = $"{prefix} {displayName}".Replace("  ", " ", StringComparison.OrdinalIgnoreCase).Trim();
		if (!usedCraftNames.TryGetValue(baseName, out var count))
		{
			usedCraftNames[baseName] = 1;
			return baseName;
		}

		count++;
		usedCraftNames[baseName] = count;
		return $"{baseName} pattern {count}";
	}

	private static string SanitiseAntiquityEquipmentVisibleName(string shortDescription)
	{
		var text = SanitiseHouseholdCraftDisplayName(shortDescription);
		foreach (var cultureTerm in AntiquityEquipmentCultureTerms)
		{
			text = text.Replace(cultureTerm, "", StringComparison.OrdinalIgnoreCase);
		}

		while (text.Contains("  ", StringComparison.Ordinal))
		{
			text = text.Replace("  ", " ", StringComparison.Ordinal);
		}

		return text.Replace(" ,", ",", StringComparison.Ordinal)
			.Replace(" -", "-", StringComparison.Ordinal)
			.Trim();
	}

	private static string StripLeadingArticle(string text)
	{
		foreach (var article in new[] { "a pair of ", "an ", "a ", "the " })
		{
			if (text.StartsWith(article, StringComparison.OrdinalIgnoreCase))
			{
				return text[article.Length..];
			}
		}

		return text;
	}

	private static bool IsMetalEquipmentMaterial(string material)
	{
		return AntiquityEquipmentMetalMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsWoodEquipmentMaterial(string material)
	{
		return AntiquityEquipmentWoodMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsTextileEquipmentMaterial(string material)
	{
		return AntiquityEquipmentTextileMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsLeatherEquipmentMaterial(string material)
	{
		return AntiquityEquipmentLeatherMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static string GetEquipmentMetalMaterial(string material, string lowerText)
	{
		if (lowerText.Contains("iron", StringComparison.OrdinalIgnoreCase))
		{
			return "wrought iron";
		}

		if (lowerText.Contains("lead", StringComparison.OrdinalIgnoreCase))
		{
			return "lead";
		}

		if (IsMetalEquipmentMaterial(material))
		{
			return material;
		}

		return "bronze";
	}

	private static string GetEquipmentWoodMaterial(string material, string lowerText)
	{
		foreach (var wood in AntiquityEquipmentWoodMaterials)
		{
			if (lowerText.Contains(wood, StringComparison.OrdinalIgnoreCase))
			{
				return wood;
			}
		}

		return IsWoodEquipmentMaterial(material) ? material : "ash";
	}

	private static string GetEquipmentTextileMaterial(string material, string lowerText)
	{
		foreach (var textile in AntiquityEquipmentTextileMaterials)
		{
			if (lowerText.Contains(textile, StringComparison.OrdinalIgnoreCase))
			{
				return textile;
			}
		}

		return IsTextileEquipmentMaterial(material) ? material : "linen";
	}
}
