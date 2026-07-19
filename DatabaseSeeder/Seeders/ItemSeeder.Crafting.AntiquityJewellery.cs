using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AncientJewelleryCraftingKnowledge = "Ancient Jewellery Crafting";

	private static readonly string[] AntiquityJewelleryTagRoots =
	[
		"Functions / Worn Items / Jewellery"
	];

	private static readonly string[] AntiquityJewelleryMetals =
	[
		"bronze",
		"copper",
		"electrum",
		"gold",
		"silver",
		"wrought iron"
	];

	private static readonly string[] AntiquityJewelleryBeadMaterials =
	[
		"agate",
		"amber",
		"bone",
		"carnelian",
		"emerald",
		"garnet",
		"glass",
		"glazed ceramic",
		"ivory",
		"jasper",
		"lapis lazuli",
		"pearl",
		"quartz",
		"shell",
		"turquoise",
		"wood"
	];

	private static readonly (string Alias, string Material)[] AntiquityJewelleryMaterialAliases =
	[
		("lapis", "lapis lazuli")
	];

	private static readonly string[] AntiquityJewelleryCultureTerms =
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

	private sealed record AntiquityJewelleryCraftPath(
		string Category,
		string Skill,
		string KnowledgeSubtype,
		string PrimaryStockTag,
		string Verb,
		string Gerund,
		int MinimumTraitValue,
		Difficulty Difficulty,
		IReadOnlyList<string> Tools);

	private void SeedAntiquityJewelleryCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		SeedAntiquityJewelleryIntermediateCommodityCrafts();

		var jewelleryTagIds = GetTagIdsUnderRoots(AntiquityJewelleryTagRoots);
		var targetItems = _items
			.Where(x => x.Key.StartsWith("jewellery_", StringComparison.OrdinalIgnoreCase))
			.Where(x => x.Value.GameItemProtosTags.Any(y => jewelleryTagIds.Contains(y.TagId)))
			.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToList();

		var usedCraftNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		foreach (var target in targetItems)
		{
			SeedAntiquityJewelleryFinishedCraft(target.Key, target.Value, usedCraftNames);
		}
	}

	private void SeedAntiquityJewelleryIntermediateCommodityCrafts()
	{
		foreach (var material in AntiquityJewelleryMetals)
		{
			AddAntiquityJewelleryCommodityCraft(
				$"prepare {material} jewellery metal stock",
				IsPreciousJewelleryMetal(material) ? "Silversmithing" : "Blacksmithing",
				IsPreciousJewelleryMetal(material) ? "Silversmithing" : "Blacksmithing",
				"Metal Jewellery",
				$"prepare {material} for jewellery work",
				[CommodityInput(500.0, material)],
				MetalJewelleryStockTools(material),
				[$"CommodityProduct - {FormatCommodityAmount(420.0)} of {material} commodity; tag Jewellery Metal Stock"]);

			AddAntiquityJewelleryCommodityCraft(
				$"draw {material} jewellery wire stock",
				IsPreciousJewelleryMetal(material) ? "Silversmithing" : "Blacksmithing",
				IsPreciousJewelleryMetal(material) ? "Silversmithing" : "Blacksmithing",
				"Metal Jewellery",
				$"draw {material} into jewellery wire",
				[CommodityInput(420.0, material, "Jewellery Metal Stock")],
				[
					"TagTool - Held - an item with the Pliers tag",
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - InRoom - an item with the Anvil tag"
				],
				[$"CommodityProduct - {FormatCommodityAmount(320.0)} of {material} commodity; tag Jewellery Wire Stock"]);
		}

		foreach (var material in AntiquityJewelleryBeadMaterials)
		{
			AddAntiquityJewelleryCommodityCraft(
				$"drill {material} jewellery bead stock",
				GetJewelleryBeadSkill(material),
				GetJewelleryBeadSkill(material),
				"Beads and Settings",
				$"drill and polish {material} into jewellery beads",
				[CommodityInput(260.0, material, colour: true, fineColour: true)],
				JewelleryBeadTools(material),
				[
					$"CommodityProduct - {FormatCommodityAmount(190.0)} of {material} commodity; tag Jewellery Bead Stock; characteristic Colour from $i1; characteristic Fine Colour from $i1"
				]);

			AddAntiquityJewelleryCommodityCraft(
				$"cut {material} jewellery setting stock",
				GetJewelleryBeadSkill(material),
				GetJewelleryBeadSkill(material),
				"Beads and Settings",
				$"cut and polish {material} into jewellery settings",
				[CommodityInput(240.0, material, colour: true, fineColour: true)],
				JewelleryBeadTools(material),
				[
					$"CommodityProduct - {FormatCommodityAmount(170.0)} of {material} commodity; tag Jewellery Setting Stock; characteristic Colour from $i1; characteristic Fine Colour from $i1"
				]);
		}
	}

	private void AddAntiquityJewelleryCommodityCraft(string name, string category, string traitName, string knowledgeSubtype,
		string blurb, IEnumerable<string> inputs, IEnumerable<string> tools, IEnumerable<string> products)
	{
		AddAntiquityCraft(
			name,
			category,
			blurb,
			name,
			$"{blurb} in progress",
			AncientJewelleryCraftingKnowledge,
			traitName,
			15,
			Difficulty.Easy,
			AntiquityJewelleryIntermediatePhases(),
			inputs,
			tools,
			products,
			knowledgeSubtype: knowledgeSubtype,
			knowledgeDescription: $"{AncientJewelleryCraftingKnowledge} covers reusable stock preparation for antiquity jewellery.",
			knowledgeLongDescription: $"{AncientJewelleryCraftingKnowledge} covers reusable stock preparation for antiquity jewellery.");
	}

	private void SeedAntiquityJewelleryFinishedCraft(string stableReference, GameItemProto item,
		IDictionary<string, int> usedCraftNames)
	{
		var material = GetMaterialName(item);
		var materialScanText = $"{stableReference} {item.ShortDescription} {item.FullDescription}".ToLowerInvariant();
		var path = GetAntiquityJewelleryCraftPath(material, materialScanText);
		var inputs = BuildAntiquityJewelleryFinalInputs(item, material, materialScanText, path);
		var displayName = SanitiseAntiquityJewelleryVisibleName(item.ShortDescription);
		var craftName = BuildUniqueAntiquityEquipmentCraftName(usedCraftNames, "make", StripLeadingArticle(displayName));

		AddAntiquityCraft(
			craftName,
			path.Category,
			$"{path.Verb} {displayName}",
			$"{path.Gerund} {displayName}",
			$"{displayName} being made",
			AncientJewelleryCraftingKnowledge,
			path.Skill,
			path.MinimumTraitValue,
			path.Difficulty,
			AntiquityJewelleryFinalPhases(),
			inputs,
			path.Tools,
			[$"SimpleProduct - 1x {item.ShortDescription} (#{item.Id})"],
			knowledgeSubtype: path.KnowledgeSubtype,
			knowledgeDescription: $"{AncientJewelleryCraftingKnowledge} covers culture-neutral antiquity jewellery making from prepared metal, bead, and setting stock.",
			knowledgeLongDescription: $"{AncientJewelleryCraftingKnowledge} covers culture-neutral antiquity jewellery making from prepared metal, bead, and setting stock.");
	}

	private static List<string> BuildAntiquityJewelleryFinalInputs(GameItemProto item, string material,
		string lowerText, AntiquityJewelleryCraftPath path)
	{
		var primaryAmount = Math.Max(8.0, item.Weight * 0.7);
		var inputs = new List<string>
		{
			CommodityInput(primaryAmount, material, path.PrimaryStockTag,
				colour: path.PrimaryStockTag.Equals("Jewellery Bead Stock", StringComparison.OrdinalIgnoreCase),
				fineColour: path.PrimaryStockTag.Equals("Jewellery Bead Stock", StringComparison.OrdinalIgnoreCase))
		};

		if (ContainsAny(lowerText, "necklace", "collar", "headband", "brow band", "diadem", "circlet"))
		{
			inputs.Add(CommodityInput(45.0, "linen", "Military Cord Stock", colour: true, fineColour: true));
		}

		if (ContainsAny(lowerText, "bracelet", "armlet", "anklet", "earring", "ring", "torc", "fibula", "brooch") &&
		    !path.PrimaryStockTag.Equals("Jewellery Wire Stock", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(35.0, GetJewelleryMetalMaterial(material, lowerText), "Jewellery Wire Stock"));
		}

		var accentMaterial = GetJewelleryAccentMaterial(material, lowerText);
		if (accentMaterial is not null && !accentMaterial.Equals(material, StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(30.0, accentMaterial, "Jewellery Setting Stock", colour: true, fineColour: true));
		}

		if (ContainsAny(lowerText, "bead", "pearl", "amber", "glass", "lapis", "turquoise", "agate") &&
		    !path.PrimaryStockTag.Equals("Jewellery Bead Stock", StringComparison.OrdinalIgnoreCase))
		{
			inputs.Add(CommodityInput(40.0, accentMaterial ?? GetJewelleryBeadMaterial(material, lowerText),
				"Jewellery Bead Stock", colour: true, fineColour: true));
		}

		return inputs;
	}

	private static AntiquityJewelleryCraftPath GetAntiquityJewelleryCraftPath(string material, string lowerText)
	{
		if (IsJewelleryMetal(material))
		{
			var stockTag = ContainsAny(lowerText, "wire", "chain", "coil", "spiral", "hoop", "torc", "ankle ring")
				? "Jewellery Wire Stock"
				: "Jewellery Metal Stock";
			var category = IsPreciousJewelleryMetal(material) ? "Silversmithing" : "Blacksmithing";
			var minimum = IsPreciousJewelleryMetal(material) ? 35 : 20;
			var difficulty = IsPreciousJewelleryMetal(material) || GetJewelleryAccentMaterial(material, lowerText) is not null
				? Difficulty.Hard
				: Difficulty.Normal;
			return new AntiquityJewelleryCraftPath(category, category, "Metal Jewellery", stockTag, "shape", "shaping",
				minimum, difficulty, MetalJewelleryFinalTools(material, lowerText));
		}

		if (material.Equals("glass", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("glazed ceramic", StringComparison.OrdinalIgnoreCase))
		{
			return new AntiquityJewelleryCraftPath("Glassworking", "Glassworking", "Glass Beads",
				"Jewellery Bead Stock", "string", "stringing", 25, Difficulty.Normal,
				[
					"TagTool - Held - an item with the Beading Needle tag",
					"TagTool - InRoom - an item with the Lit Glory Hole tag",
					"TagTool - Held - an item with the Polishing Stone tag"
				]);
		}

		if (material.Equals("bone", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("ivory", StringComparison.OrdinalIgnoreCase))
		{
			return new AntiquityJewelleryCraftPath("Bonecarving", "Bonecarving", "Bone and Ivory Jewellery",
				"Jewellery Bead Stock", "carve", "carving", 20, Difficulty.Normal,
				[
					"TagTool - Held - an item with the Bow Drill tag",
					"TagTool - Held - an item with the Polishing Stone tag"
				]);
		}

		if (material.Equals("wood", StringComparison.OrdinalIgnoreCase))
		{
			return new AntiquityJewelleryCraftPath("Carpentry", "Carpentry", "Wooden Bead Jewellery",
				"Jewellery Bead Stock", "string", "stringing", 15, Difficulty.Easy, JewelleryBeadTools(material));
		}

		return new AntiquityJewelleryCraftPath("Gemcraft", "Gemcraft", "Beads and Settings",
			"Jewellery Bead Stock", "polish", "polishing", 25, Difficulty.Normal, JewelleryBeadTools(material));
	}

	private static (int Seconds, string Echo, string FailEcho)[] AntiquityJewelleryIntermediatePhases()
	{
		return
		[
			(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1, but miss|misses flaws in the material."),
			(35, "$0 work|works the material into small, regular stock.", "$0 work|works the material unevenly."),
			(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.")
		];
	}

	private static (int Seconds, string Echo, string FailEcho)[] AntiquityJewelleryFinalPhases()
	{
		return
		[
			(30, "$0 lay|lays out $i1 and check|checks the prepared jewellery stock.", "$0 lay|lays out $i1, but overlook|overlooks a flaw in the stock."),
			(40, "$0 shape|shapes, pierce|pierces, and fit|fits the main form with $t1.", "$0 shape|shapes the piece poorly with $t1."),
			(35, "$0 polish|polishes and secure|secures the finer details.", "$0 polish|polishes the piece, but the fitting slips out of true."),
			(25, "$0 set|sets aside $p1 after a final inspection.", "$0 set|sets aside the work, but it is not worth keeping.")
		];
	}

	private static string[] MetalJewelleryStockTools(string material)
	{
		return IsPreciousJewelleryMetal(material)
			?
			[
				"TagTool - Held - an item with the Crucible tag",
				"TagTool - Held - an item with the Crucible Tongs tag",
				"TagTool - InRoom - an item with the Lit Smelting Furnace tag"
			]
			:
			[
				"TagTool - InRoom - an item with the Lit Smelting Furnace tag",
				"TagTool - Held - an item with the Forge Tongs tag",
				"TagTool - InRoom - an item with the Anvil tag",
				"TagTool - Held - an item with the Hammer tag"
			];
	}

	private static string[] MetalJewelleryFinalTools(string material, string lowerText)
	{
		if (IsPreciousJewelleryMetal(material) || ContainsAny(lowerText, "gilt", "inlaid", "garnet", "emerald", "pearl"))
		{
			return
			[
				"TagTool - Held - an item with the Pliers tag",
				"TagTool - Held - an item with the Bronze Burnisher tag",
				"TagTool - Held - an item with the Polishing Stone tag"
			];
		}

		return
		[
			"TagTool - Held - an item with the Pliers tag",
			"TagTool - Held - an item with the Hammer tag",
			"TagTool - InRoom - an item with the Anvil tag"
		];
	}

	private static string[] JewelleryBeadTools(string material)
	{
		if (material.Equals("glass", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("glazed ceramic", StringComparison.OrdinalIgnoreCase))
		{
			return
			[
				"TagTool - Held - an item with the Bow Drill tag",
				"TagTool - Held - an item with the Polishing Stone tag",
				"TagTool - InRoom - an item with the Lit Glory Hole tag"
			];
		}

		return
		[
			"TagTool - Held - an item with the Bow Drill tag",
			"TagTool - Held - an item with the Polishing Stone tag"
		];
	}

	private static bool IsJewelleryMetal(string material)
	{
		return AntiquityJewelleryMetals.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsPreciousJewelleryMetal(string material)
	{
		return material.Equals("gold", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("silver", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("electrum", StringComparison.OrdinalIgnoreCase);
	}

	private static string GetJewelleryBeadSkill(string material)
	{
		if (material.Equals("glass", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("glazed ceramic", StringComparison.OrdinalIgnoreCase))
		{
			return "Glassworking";
		}

		if (material.Equals("bone", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("ivory", StringComparison.OrdinalIgnoreCase))
		{
			return "Bonecarving";
		}

		if (material.Equals("wood", StringComparison.OrdinalIgnoreCase))
		{
			return "Carpentry";
		}

		return "Gemcraft";
	}

	private static string GetJewelleryMetalMaterial(string material, string lowerText)
	{
		foreach (var metal in AntiquityJewelleryMetals)
		{
			if (lowerText.Contains(metal, StringComparison.OrdinalIgnoreCase))
			{
				return metal;
			}
		}

		return IsJewelleryMetal(material) ? material : "bronze";
	}

	private static string GetJewelleryBeadMaterial(string material, string lowerText)
	{
		return GetJewelleryAccentMaterial(material, lowerText) ?? (AntiquityJewelleryBeadMaterials.Contains(material,
			StringComparer.OrdinalIgnoreCase)
			? material
			: "glass");
	}

	private static string? GetJewelleryAccentMaterial(string material, string lowerText)
	{
		foreach (var accent in AntiquityJewelleryBeadMaterials)
		{
			if (!accent.Equals(material, StringComparison.OrdinalIgnoreCase) &&
			    lowerText.Contains(accent, StringComparison.OrdinalIgnoreCase))
			{
				return accent;
			}
		}

		foreach (var alias in AntiquityJewelleryMaterialAliases)
		{
			if (!alias.Material.Equals(material, StringComparison.OrdinalIgnoreCase) &&
			    lowerText.Contains(alias.Alias, StringComparison.OrdinalIgnoreCase))
			{
				return alias.Material;
			}
		}

		return null;
	}

	private static string SanitiseAntiquityJewelleryVisibleName(string shortDescription)
	{
		var text = SanitiseHouseholdCraftDisplayName(shortDescription);
		foreach (var cultureTerm in AntiquityJewelleryCultureTerms)
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
}
