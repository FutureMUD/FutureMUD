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
		"ceramic", "earthenware", "fired clay", "terracotta"
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

	private void SeedAntiquityFurnitureAndContainerCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		SeedAntiquityHouseholdIntermediateCommodityCrafts();

		var householdTagIds = _tagsByFullPath
			.Where(x => x.Key.StartsWith("Market / Household Goods /", StringComparison.OrdinalIgnoreCase))
			.Select(x => x.Value.Id)
			.ToHashSet();

		var targetItems = _items
			.Where(x => x.Key.StartsWith("antiquity_", StringComparison.OrdinalIgnoreCase))
			.Where(x => x.Value.GameItemProtosTags.Any(y => householdTagIds.Contains(y.TagId)))
			.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToList();

		foreach (var target in targetItems)
		{
			SeedAntiquityHouseholdFinishedCraft(target.Key, target.Value);
		}
	}

	private void SeedAntiquityHouseholdIntermediateCommodityCrafts()
	{
		foreach (var material in AntiquityHouseholdWoodMaterials)
		{
			AddAntiquityHouseholdCommodityCraft(
				$"prepare {material} furniture timber stock",
				"Carpentry",
				"Carpentry",
				AncientWoodworkingKnowledge,
				"Woodworking",
				$"dress {material} into furniture timber stock",
				[CommodityInput(1200.0, material)],
				[
					"TagTool - Held - an item with the Hand Saw tag",
					"TagTool - Held - an item with the Adze tag"
				],
				[$"CommodityProduct - {FormatCommodityAmount(1000.0)} of {material} commodity; tag Furniture Timber Stock"]);

			AddAntiquityHouseholdCommodityCraft(
				$"saw {material} furniture panel stock",
				"Carpentry",
				"Carpentry",
				AncientWoodworkingKnowledge,
				"Woodworking",
				$"saw {material} into furniture panel stock",
				[CommodityInput(1200.0, material, "Furniture Timber Stock")],
				[
					"TagTool - Held - an item with the Hand Saw tag",
					"TagTool - Held - an item with the Wood Clamp tag"
				],
				[$"CommodityProduct - {FormatCommodityAmount(900.0)} of {material} commodity; tag Furniture Panel Stock"]);

			AddAntiquityHouseholdCommodityCraft(
				$"carve {material} household vessel stock",
				"Carpentry",
				"Carpentry",
				AncientWoodworkingKnowledge,
				"Woodworking",
				$"carve {material} into household vessel blanks",
				[CommodityInput(1000.0, material, "Furniture Timber Stock")],
				[
					"TagTool - Held - an item with the Wood Chisel tag",
					"TagTool - Held - an item with the Rasp tag"
				],
				[$"CommodityProduct - {FormatCommodityAmount(800.0)} of {material} commodity; tag Carved Wood Stock"]);
		}

		foreach (var material in new[] { "oak", "cedar", "cypress", "pine", "willow", "wood" })
		{
			AddAntiquityHouseholdCommodityCraft(
				$"shape {material} coopered staves",
				"Coopering",
				"Coopering",
				AncientCooperingKnowledge,
				"Coopering",
				$"shape {material} into coopered staves",
				[CommodityInput(1500.0, material, "Furniture Timber Stock")],
				[
					"TagTool - Held - an item with the Cooper's Adze tag",
					"TagTool - Held - an item with the Croze tag"
				],
				[$"CommodityProduct - {FormatCommodityAmount(1150.0)} of {material} commodity; tag Coopered Staves"]);
		}

		foreach (var material in new[] { "bronze", "wrought iron", "willow", "wood" })
		{
			string[] hoopTools = IsMetalMaterial(material)
				?
				[
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - InRoom - an item with the Anvil tag"
				]
				:
				[
					"TagTool - Held - an item with the Basket Knife tag",
					"TagTool - Held - an item with the Packing Bone tag"
				];
			AddAntiquityHouseholdCommodityCraft(
				$"make {material} hoop stock",
				IsMetalMaterial(material) ? "Blacksmithing" : "Coopering",
				IsMetalMaterial(material) ? "Blacksmithing" : "Coopering",
				AncientCooperingKnowledge,
				"Coopering",
				$"make {material} hoop stock",
				[CommodityInput(800.0, material)],
				hoopTools,
				[$"CommodityProduct - {FormatCommodityAmount(650.0)} of {material} commodity; tag Hoop Stock"]);
		}

		foreach (var material in new[] { "willow", "papyrus", "hemp" })
		{
			AddAntiquityHouseholdCommodityCraft(
				$"split {material} basketry splints",
				"Basketry",
				"Basketry",
				AncientBasketryKnowledge,
				"Basketry",
				$"split {material} into basketry splints",
				[CommodityInput(1000.0, material)],
				[
					"TagTool - Held - an item with the Basket Knife tag",
					"TagTool - Held - an item with the Reed Splitter tag"
				],
				[$"CommodityProduct - {FormatCommodityAmount(850.0)} of {material} commodity; tag Basketry Splint"]);

			AddAntiquityHouseholdCommodityCraft(
				$"weave {material} reed matting",
				"Basketry",
				"Basketry",
				AncientBasketryKnowledge,
				"Basketry",
				$"weave {material} into reed matting",
				[CommodityInput(1000.0, material, "Basketry Splint", colour: true, fineColour: true)],
				[
					"TagTool - Held - an item with the Weaving Bodkin tag",
					"TagTool - Held - an item with the Packing Bone tag"
				],
				[
					$"CommodityProduct - {FormatCommodityAmount(800.0)} of {material} commodity; tag Reed Matting; characteristic Colour from $i1; characteristic Fine Colour from $i1"
				]);
		}

		foreach (var material in AntiquityHouseholdLeatherMaterials)
		{
			AddAntiquityHouseholdCommodityCraft(
				$"prepare {material} leather panel stock",
				"Leathermaking",
				"Leathermaking",
				AncientLeatherContainersKnowledge,
				"Leather Containers",
				$"prepare {material} into container panels",
				[CommodityInput(1000.0, material, colour: true, fineColour: true)],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - InRoom - an item with the Leather Stitching Pony tag"
				],
				[
					$"CommodityProduct - {FormatCommodityAmount(850.0)} of {material} commodity; tag Prepared Leather Panel; characteristic Colour from $i1; characteristic Fine Colour from $i1"
				]);
		}

		foreach (var material in AntiquityHouseholdTextileMaterials.Where(x => !x.Equals("papyrus", StringComparison.OrdinalIgnoreCase)))
		{
			AddAntiquityHouseholdCommodityCraft(
				$"prepare {material} household cloth panels",
				"Tailoring",
				"Tailoring",
				AncientHouseholdCraftingKnowledge,
				"Textile Furnishings",
				$"prepare {material} into household cloth panels",
				[CommodityInput(1000.0, material, colour: true, fineColour: true)],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag"
				],
				[
					$"CommodityProduct - {FormatCommodityAmount(850.0)} of {material} commodity; tag Garment Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"
				]);
		}

		AddAntiquityHouseholdCommodityCraft(
			"twist linen lamp wick",
			"Ropemaking",
			"Ropemaking",
			AncientLightingKnowledge,
			"Lighting",
			"twist linen into lamp wick",
			[CommodityInput(300.0, "linen", "Spun Yarn", colour: true)],
			[
				"TagTool - Held - an item with the Rope Hook tag",
				"TagTool - Held - an item with the Twine Shuttle tag"
			],
			[
				"CommodityProduct - 240 grams of linen commodity; tag Lamp Wick; characteristic Colour from $i1"
			]);

		AddAntiquityHouseholdCommodityCraft(
			"purify beeswax candlemaking stock",
			"Candlemaking",
			"Candlemaking",
			AncientLightingKnowledge,
			"Lighting",
			"purify beeswax for candle moulding",
			[CommodityInput(1000.0, "beeswax")],
			["TagTool - InRoom - an item with the Fire tag"],
			["CommodityProduct - 850 grams of beeswax commodity; tag Candlemaking Wax"]);

		AddAntiquityHouseholdCommodityCraft(
			"prepare pitch for torches",
			"Candlemaking",
			"Candlemaking",
			AncientLightingKnowledge,
			"Lighting",
			"prepare pitch for household lighting",
			[CommodityInput(1000.0, "pitch")],
			["TagTool - InRoom - an item with the Fire tag"],
			["CommodityProduct - 850 grams of pitch commodity; tag Prepared Pitch"]);

		foreach (var material in AntiquityHouseholdCeramicMaterials)
		{
			AddAntiquityHouseholdCommodityCraft(
				$"levigate {material} pottery clay body",
				"Pottery",
				"Pottery",
				AncientCeramicVesselmakingKnowledge,
				"Ceramics",
				$"levigate clay into {material} pottery body",
				[CommodityInput(1400.0, "clay"), "LiquidUse - 2 litres of Water"],
				["TagTool - Held - an item with the Clay Knife tag"],
				[$"CommodityProduct - {FormatCommodityAmount(1200.0)} of {material} commodity; tag Pottery Clay Body"]);

			AddAntiquityHouseholdCommodityCraft(
				$"throw {material} wet vessel blank",
				"Pottery",
				"Pottery",
				AncientCeramicVesselmakingKnowledge,
				"Ceramics",
				$"throw a wet {material} vessel blank",
				[CommodityInput(1200.0, material, "Pottery Clay Body")],
				[
					"TagTool - InRoom - an item with the Potter's Wheel tag",
					"TagTool - Held - an item with the Potter's Rib tag"
				],
				[$"CommodityProduct - {FormatCommodityAmount(1000.0)} of {material} commodity; tag Wet Vessel Blank"]);

			AddAntiquityHouseholdCommodityCraft(
				$"bisque fire {material} vessel blank",
				"Pottery",
				"Pottery",
				AncientCeramicVesselmakingKnowledge,
				"Ceramics",
				$"bisque fire a {material} vessel blank",
				[CommodityInput(1000.0, material, "Wet Vessel Blank", colour: true, fineColour: true)],
				["TagTool - InRoom - an item with the Kiln tag"],
				[
					$"CommodityProduct - {FormatCommodityAmount(900.0)} of {material} commodity; tag Bisque Vessel Blank; characteristic Colour from $i1; characteristic Fine Colour from $i1"
				]);
		}

		foreach (var material in AntiquityHouseholdGlassMaterials)
		{
			AddAntiquityHouseholdCommodityCraft(
				$"mix {material} glass batch",
				"Glassworking",
				"Glassworking",
				AncientGlassworkingKnowledge,
				"Glassworking",
				$"mix {material} glass batch",
				[CommodityInput(1200.0, material, colour: true, fineColour: true)],
				["TagTool - InRoom - an item with the Crucible tag"],
				[
					$"CommodityProduct - {FormatCommodityAmount(1100.0)} of {material} commodity; tag Glass Batch; characteristic Colour from $i1; characteristic Fine Colour from $i1"
				]);

			AddAntiquityHouseholdCommodityCraft(
				$"blow {material} vessel blank",
				"Glassworking",
				"Glassworking",
				AncientGlassworkingKnowledge,
				"Glassworking",
				$"blow {material} into a vessel blank",
				[CommodityInput(1100.0, material, "Glass Batch", colour: true, fineColour: true)],
				[
					"TagTool - Held - an item with the Blowpipe tag",
					"TagTool - Held - an item with the Pontil Rod tag",
					"TagTool - InRoom - an item with the Annealing Lehr tag"
				],
				[
					$"CommodityProduct - {FormatCommodityAmount(850.0)} of {material} commodity; tag Glass Vessel Blank; characteristic Colour from $i1; characteristic Fine Colour from $i1"
				]);
		}

		foreach (var material in AntiquityHouseholdMetalMaterials)
		{
			AddAntiquityHouseholdCommodityCraft(
				$"cast {material} vessel blank",
				IsPreciousMetal(material) ? "Silversmithing" : "Blacksmithing",
				IsPreciousMetal(material) ? "Silversmithing" : "Blacksmithing",
				AncientMetalVesselmakingKnowledge,
				"Metal Vessels",
				$"cast {material} into a vessel blank",
				[CommodityInput(1200.0, material)],
				[
					"TagTool - Held - an item with the Crucible tag",
					"TagTool - Held - an item with the Crucible Tongs tag",
					"TagTool - InRoom - an item with the Vessel Casting Mould tag"
				],
				[$"CommodityProduct - {FormatCommodityAmount(1000.0)} of {material} commodity; tag Cast Vessel Blank"]);
		}

		foreach (var material in AntiquityHouseholdStoneHornMaterials)
		{
			AddAntiquityHouseholdCommodityCraft(
				$"rough out {material} vessel blank",
				material.Equals("ivory", StringComparison.OrdinalIgnoreCase) || material.Equals("horn", StringComparison.OrdinalIgnoreCase)
					? "Scrimshawing"
					: "Masonry",
				material.Equals("ivory", StringComparison.OrdinalIgnoreCase) || material.Equals("horn", StringComparison.OrdinalIgnoreCase)
					? "Scrimshawing"
					: "Masonry",
				AncientStoneCarvingKnowledge,
				"Carving",
				$"rough out {material} into a household vessel blank",
				[CommodityInput(1200.0, material)],
				[
					"TagTool - Held - an item with the Stone Chisel tag",
					"TagTool - Held - an item with the Stone Mallet tag"
				],
				[$"CommodityProduct - {FormatCommodityAmount(900.0)} of {material} commodity; tag Stone Vessel Blank"]);
		}

		foreach (var material in new[] { "ivory", "horn", "glass", "gold", "silver" })
		{
			AddAntiquityHouseholdCommodityCraft(
				$"prepare {material} inlay stock",
				IsMetalMaterial(material) ? "Silversmithing" : material.Equals("glass", StringComparison.OrdinalIgnoreCase) ? "Glassworking" : "Scrimshawing",
				IsMetalMaterial(material) ? "Silversmithing" : material.Equals("glass", StringComparison.OrdinalIgnoreCase) ? "Glassworking" : "Scrimshawing",
				AncientHouseholdCraftingKnowledge,
				"Decoration",
				$"prepare {material} inlay stock",
				[CommodityInput(500.0, material, colour: true, fineColour: true)],
				[
					"TagTool - Held - an item with the Bow Drill tag",
					"TagTool - Held - an item with the Polishing Stone tag"
				],
				[
					$"CommodityProduct - {FormatCommodityAmount(420.0)} of {material} commodity; tag Inlay Stock; characteristic Colour from $i1; characteristic Fine Colour from $i1"
				]);
		}

		AddPaintPigmentCraft("prepare lamp black paint pigment", "soot", "black", "lamp black");
		AddPaintPigmentCraft("prepare bone black paint pigment", "bone black pigment", "black", "bone black");
		AddPaintPigmentCraft("prepare chalk white paint pigment", "chalk dust", "white", "chalk white");
		AddPaintPigmentCraft("prepare lead white paint pigment", "lead white pigment", "white", "lead white");
		AddPaintPigmentCraft("prepare red household paint pigment", "madder root", "red", "madder red");
		AddPaintPigmentCraft("prepare red ochre paint pigment", "red ochre pigment", "red", "red ochre");
		AddPaintPigmentCraft("prepare yellow ochre paint pigment", "yellow ochre pigment", "yellow", "yellow ochre");
		AddPaintPigmentCraft("prepare ochre household paint pigment", "ochre pigment", "yellow", "ochre");
		AddPaintPigmentCraft("prepare hematite red paint pigment", "hematite", "red", "hematite red");
		AddPaintPigmentCraft("prepare cinnabar red paint pigment", "cinnabar", "red", "cinnabar red");
		AddPaintPigmentCraft("prepare realgar orange paint pigment", "realgar", "orange", "realgar orange");
		AddPaintPigmentCraft("prepare orpiment yellow paint pigment", "orpiment", "yellow", "orpiment yellow");
		AddPaintPigmentCraft("prepare malachite green paint pigment", "malachite", "green", "malachite green");
		AddPaintPigmentCraft("prepare verdigris green paint pigment", "verdigris pigment", "green", "verdigris green");
		AddPaintPigmentCraft("prepare blue household paint pigment", "indigo dye cake", "dark blue", "deep indigo");
		AddPaintPigmentCraft("prepare azurite blue paint pigment", "azurite", "blue", "azurite blue");
		AddPaintPigmentCraft("prepare lapis blue paint pigment", "lapis lazuli", "blue", "lapis blue");
		AddPaintPigmentCraft("prepare egyptian blue paint pigment", "egyptian blue frit", "blue", "egyptian blue");
		AddLakePigmentCraft("prepare madder lake pigment", "madder root");
		AddLakePigmentCraft("prepare kermes lake pigment", "kermes grain");
		AddLakePigmentCraft("prepare indigo lake pigment", "indigo dye cake");
		AddLakePigmentCraft("prepare woad lake pigment", "woad leaves");
		AddLakePigmentCraft("prepare saffron lake pigment", "saffron");
		AddLakePigmentCraft("prepare weld lake pigment", "weld");
		AddLakePigmentCraft("prepare tyrian purple lake pigment", "murex purple dye");
		AddLacquerFinishCraft("prepare red lacquer finish", "red", "crimson");
		AddLacquerFinishCraft("prepare dark lacquer finish", "black", "jet black");
	}

	private void AddPaintPigmentCraft(string name, string pigmentMaterial, string colour, string fineColour)
	{
		AddAntiquityHouseholdCommodityCraft(
			name,
			"Dyeing",
			"Dyeing",
			AncientHouseholdCraftingKnowledge,
			"Decoration",
			name,
			[CommodityInput(400.0, pigmentMaterial), "Commodity - 80 grams of alum mordant", "LiquidUse - 500 millilitres of Water"],
			["TagTool - InRoom - an item with the Dye Vat tag"],
			[
				$"CommodityProduct - 300 grams of {pigmentMaterial} commodity; tag Paint Pigment; characteristic Colour={colour}; characteristic Fine Colour={fineColour}"
			]);
	}

	private void AddLakePigmentCraft(string name, string dyeMaterial)
	{
		AddAntiquityHouseholdCommodityCraft(
			name,
			"Dyeing",
			"Dyeing",
			AncientHouseholdCraftingKnowledge,
			"Decoration",
			name,
			[
				CommodityInput(280.0, dyeMaterial, "Textile Dye Stock", colour: true, fineColour: true),
				"Commodity - 100 grams of chalk dust",
				"Commodity - 80 grams of alum mordant",
				"LiquidUse - 500 millilitres of Water"
			],
			[
				"TagTool - InRoom - an item with the Dye Vat tag",
				"TagTool - InRoom - an item with the Mordant Cauldron tag",
				"TagTool - Held - an item with the Dye Strainer tag"
			],
			[
				$"CommodityProduct - 240 grams of {dyeMaterial} commodity; tag Lake Pigment; characteristic Colour from $i1; characteristic Fine Colour from $i1"
			]);
	}

	private void AddLacquerFinishCraft(string name, string colour, string fineColour)
	{
		AddAntiquityHouseholdCommodityCraft(
			name,
			"Lacquerwork",
			"Lacquerwork",
			AncientHouseholdCraftingKnowledge,
			"Decoration",
			name,
			[CommodityInput(500.0, "resin"), CommodityInput(120.0, "ochre pigment")],
			[
				"TagTool - Held - an item with the Lacquerer's Brush tag",
				"TagTool - Held - an item with the Lacquer Spatula tag"
			],
			[
				$"CommodityProduct - 450 grams of resin commodity; tag Lacquer Finish; characteristic Colour={colour}; characteristic Fine Colour={fineColour}"
			]);
	}

	private void SeedAntiquityHouseholdFinishedCraft(string stableReference, GameItemProto item)
	{
		var material = GetMaterialName(item);
		var componentNames = GetItemComponentNames(item).ToList();
		var path = GetAntiquityHouseholdCraftPath(stableReference, item, material, componentNames);
		var variableStyle = GetHouseholdVariableStyle(item, componentNames);
		var variableSource = GetHouseholdVariableSource(stableReference, item, material, variableStyle);
		var (inputs, variableInputIndex) =
			BuildAntiquityHouseholdFinalInputs(stableReference, item, material, path, variableStyle, variableSource);
		var products = new[]
		{
			BuildAntiquityHouseholdProduct(item, variableStyle, variableInputIndex)
		};
		var knowledge = GetAntiquityHouseholdKnowledge(stableReference, item, path);
		var displayName = SanitiseHouseholdCraftDisplayName(item.ShortDescription);

		AddCraft(
			$"make {stableReference["antiquity_".Length..].Replace('_', ' ')}",
			path.Category,
			$"{path.Verb} {displayName}",
			$"{path.Gerund} {displayName}",
			$"{displayName} being made",
			knowledge.Name,
			path.Skill,
			path.MinimumTraitValue,
			path.Difficulty,
			Outcome.MinorFail,
			5,
			4,
			false,
			GetAntiquityHouseholdFinalPhases(path),
			inputs,
			path.Tools,
			products,
			[],
			knowledgeSubtype: knowledge.Subtype,
			knowledgeDescription: knowledge.Description,
			knowledgeLongDescription: knowledge.Description);
	}

	private (List<string> Inputs, int VariableInputIndex) BuildAntiquityHouseholdFinalInputs(string stableReference,
		GameItemProto item, string material, AntiquityHouseholdCraftPath path,
		AntiquityHouseholdVariableStyle variableStyle, AntiquityHouseholdVariableSource variableSource)
	{
		var inputs = new List<string>();
		var lowerStable = stableReference.ToLowerInvariant();
		var lowerDescription = item.ShortDescription.ToLowerInvariant();
		var useFineColour = variableStyle == AntiquityHouseholdVariableStyle.FineColour;
		var useColour = variableStyle is AntiquityHouseholdVariableStyle.BasicColour or AntiquityHouseholdVariableStyle.FineColour;
		var primaryAmount = Math.Max(25.0, item.Weight);
		var variableInputIndex = 1;

		bool UseColourFor(AntiquityHouseholdVariableSource source)
		{
			return useColour && variableSource == source;
		}

		bool UseFineColourFor(AntiquityHouseholdVariableSource source)
		{
			return useFineColour && variableSource == source;
		}

		void AddInput(string input, AntiquityHouseholdVariableSource source = AntiquityHouseholdVariableSource.None)
		{
			inputs.Add(input);
			if (variableSource == source)
			{
				variableInputIndex = inputs.Count;
			}
		}

		if (material.Equals("beeswax", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityInput(primaryAmount, "beeswax", "Candlemaking Wax",
					colour: UseColourFor(AntiquityHouseholdVariableSource.PrimaryStock),
					fineColour: UseFineColourFor(AntiquityHouseholdVariableSource.PrimaryStock)),
				AntiquityHouseholdVariableSource.PrimaryStock);
			AddInput(CommodityInput(25.0, "linen", "Lamp Wick"));
			return (inputs, variableInputIndex);
		}

		if (lowerDescription.Contains("torch", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("torch", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityInput(primaryAmount, material, "Furniture Timber Stock",
					colour: UseColourFor(AntiquityHouseholdVariableSource.PrimaryStock),
					fineColour: UseFineColourFor(AntiquityHouseholdVariableSource.PrimaryStock)),
				AntiquityHouseholdVariableSource.PrimaryStock);
			AddInput(CommodityInput(180.0, "pitch", "Prepared Pitch"));
			AddInput(CommodityInput(30.0, "linen", "Lamp Wick"));
			return (inputs, variableInputIndex);
		}

		AddInput(CommodityInput(primaryAmount, material, path.PrimaryStockTag,
				colour: UseColourFor(AntiquityHouseholdVariableSource.PrimaryStock),
				fineColour: UseFineColourFor(AntiquityHouseholdVariableSource.PrimaryStock)),
			AntiquityHouseholdVariableSource.PrimaryStock);

		if (lowerStable.Contains("coopered", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("cask", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("barrel", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("bucket", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("tub", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityInput(300.0, "bronze", "Hoop Stock"));
		}

		if (lowerDescription.Contains("leather", StringComparison.OrdinalIgnoreCase) &&
		    !IsLeatherMaterial(material))
		{
			AddInput(CommodityInput(450.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
				AntiquityHouseholdVariableSource.LeatherPanel);
		}

		if ((lowerDescription.Contains("linen", StringComparison.OrdinalIgnoreCase) ||
		     lowerDescription.Contains("cloth", StringComparison.OrdinalIgnoreCase) ||
		     lowerDescription.Contains("wool", StringComparison.OrdinalIgnoreCase) ||
		     lowerDescription.Contains("felt", StringComparison.OrdinalIgnoreCase)) &&
		    !IsTextileMaterial(material))
		{
			AddInput(CommodityInput(650.0,
					lowerDescription.Contains("wool", StringComparison.OrdinalIgnoreCase) ? "wool" : "linen",
					"Garment Cloth", colour: true, fineColour: true),
				AntiquityHouseholdVariableSource.ClothPanel);
		}

		if (lowerDescription.Contains("rush", StringComparison.OrdinalIgnoreCase) ||
		    lowerDescription.Contains("reed", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityInput(450.0, "willow", "Reed Matting", colour: true, fineColour: true),
				AntiquityHouseholdVariableSource.ReedMatting);
		}

		if (lowerDescription.Contains("bronze", StringComparison.OrdinalIgnoreCase) &&
		    !material.Equals("bronze", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityInput(250.0, "bronze", "Cast Vessel Blank"));
		}

		if (lowerDescription.Contains("gold", StringComparison.OrdinalIgnoreCase) &&
		    !material.Equals("gold", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityInput(120.0, "gold", "Inlay Stock", colour: true, fineColour: true));
		}

		if (lowerDescription.Contains("silver", StringComparison.OrdinalIgnoreCase) &&
		    !material.Equals("silver", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityInput(120.0, "silver", "Inlay Stock", colour: true, fineColour: true));
		}

		if (lowerDescription.Contains("inlaid", StringComparison.OrdinalIgnoreCase) ||
		    lowerDescription.Contains("inlay", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityInput(180.0, "ivory", "Inlay Stock", colour: true, fineColour: true));
		}

		if (lowerDescription.Contains("painted", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("painted", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("black_figure", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("red_figure", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("bichrome", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityPileInput(180.0, "Paint Pigment", colour: true, fineColour: true),
				AntiquityHouseholdVariableSource.PaintPigment);
		}

		if (lowerDescription.Contains("lacquer", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("lacquer", StringComparison.OrdinalIgnoreCase))
		{
			AddInput(CommodityInput(250.0, "resin", "Lacquer Finish", colour: true, fineColour: true),
				AntiquityHouseholdVariableSource.LacquerFinish);
		}

		return (inputs, variableInputIndex);
	}

	private AntiquityHouseholdCraftPath GetAntiquityHouseholdCraftPath(string stableReference, GameItemProto item,
		string material, IReadOnlyCollection<string> componentNames)
	{
		var lowerStable = stableReference.ToLowerInvariant();
		var lowerDescription = item.ShortDescription.ToLowerInvariant();
		var hasContainer = componentNames.Any(x => x.StartsWith("Container_", StringComparison.OrdinalIgnoreCase));

		if (material.Equals("beeswax", StringComparison.OrdinalIgnoreCase))
		{
			return CandlePath();
		}

		if (lowerStable.Contains("torch", StringComparison.OrdinalIgnoreCase) ||
		    lowerDescription.Contains("torch", StringComparison.OrdinalIgnoreCase))
		{
			return TorchPath();
		}

		if (lowerStable.Contains("lacquer", StringComparison.OrdinalIgnoreCase) ||
		    lowerDescription.Contains("lacquer", StringComparison.OrdinalIgnoreCase))
		{
			return LacquerPath();
		}

		if (IsCoopered(lowerStable, lowerDescription) && IsWoodMaterial(material))
		{
			return CooperingPath();
		}

		if (IsBasketry(lowerStable, lowerDescription, material))
		{
			return BasketryPath();
		}

		if (IsLeatherMaterial(material))
		{
			return LeatherPath();
		}

		if (IsTextileMaterial(material))
		{
			return TextilePath();
		}

		if (IsCeramicMaterial(material))
		{
			return PotteryPath();
		}

		if (IsGlassMaterial(material))
		{
			return GlassPath();
		}

		if (IsMetalMaterial(material))
		{
			return MetalPath(material);
		}

		if (IsStoneHornMaterial(material))
		{
			return StonePath(material);
		}

		if (IsWoodMaterial(material))
		{
			if (IsPanelFurniture(lowerStable, lowerDescription))
			{
				return WoodPanelPath();
			}

			return hasContainer ? WoodCarvingPath() : WoodworkingPath();
		}

		return HouseholdGeneralPath();
	}

	private static AntiquityHouseholdCraftPath WoodworkingPath() => new(
		"Carpentry", "Carpentry", AncientWoodworkingKnowledge, "Woodworking", "Furniture Timber Stock", "build",
		"building", 20, Difficulty.Normal,
		[
			"TagTool - Held - an item with the Hand Saw tag",
			"TagTool - Held - an item with the Adze tag",
			"TagTool - Held - an item with the Wood Chisel tag"
		]);

	private static AntiquityHouseholdCraftPath WoodPanelPath() => new(
		"Carpentry", "Carpentry", AncientWoodworkingKnowledge, "Woodworking", "Furniture Panel Stock", "assemble",
		"assembling", 25, Difficulty.Normal,
		[
			"TagTool - Held - an item with the Hand Saw tag",
			"TagTool - Held - an item with the Wood Chisel tag",
			"TagTool - Held - an item with the Wood Clamp tag"
		]);

	private static AntiquityHouseholdCraftPath WoodCarvingPath() => new(
		"Carpentry", "Carpentry", AncientWoodworkingKnowledge, "Woodworking", "Carved Wood Stock", "carve",
		"carving", 25, Difficulty.Normal,
		[
			"TagTool - Held - an item with the Wood Chisel tag",
			"TagTool - Held - an item with the Rasp tag",
			"TagTool - Held - an item with the Bow Drill tag"
		]);

	private static AntiquityHouseholdCraftPath BasketryPath() => new(
		"Basketry", "Basketry", AncientBasketryKnowledge, "Basketry", "Basketry Splint", "weave", "weaving", 15,
		Difficulty.Easy,
		[
			"TagTool - Held - an item with the Basket Knife tag",
			"TagTool - Held - an item with the Reed Splitter tag",
			"TagTool - Held - an item with the Weaving Bodkin tag"
		]);

	private static AntiquityHouseholdCraftPath CooperingPath() => new(
		"Coopering", "Coopering", AncientCooperingKnowledge, "Coopering", "Coopered Staves", "cooper",
		"coopering", 30, Difficulty.Normal,
		[
			"TagTool - Held - an item with the Cooper's Adze tag",
			"TagTool - Held - an item with the Croze tag",
			"TagTool - Held - an item with the Hoop Driver tag"
		]);

	private static AntiquityHouseholdCraftPath TextilePath() => new(
		"Tailoring", "Tailoring", AncientHouseholdCraftingKnowledge, "Textile Furnishings", "Garment Cloth", "sew",
		"sewing", 15, Difficulty.Easy,
		[
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		]);

	private static AntiquityHouseholdCraftPath LeatherPath() => new(
		"Leathermaking", "Leathermaking", AncientLeatherContainersKnowledge, "Leather Containers",
		"Prepared Leather Panel", "stitch", "stitching", 20, Difficulty.Normal,
		[
			"TagTool - Held - an item with the Awl Punch tag",
			"TagTool - InRoom - an item with the Leather Stitching Pony tag",
			"TagTool - Held - an item with the Edge Beveller tag"
		]);

	private static AntiquityHouseholdCraftPath PotteryPath() => new(
		"Pottery", "Pottery", AncientCeramicVesselmakingKnowledge, "Ceramics", "Bisque Vessel Blank", "finish",
		"finishing", 20, Difficulty.Normal,
		[
			"TagTool - InRoom - an item with the Potter's Wheel tag",
			"TagTool - Held - an item with the Potter's Rib tag",
			"TagTool - InRoom - an item with the Kiln tag"
		]);

	private static AntiquityHouseholdCraftPath GlassPath() => new(
		"Glassworking", "Glassworking", AncientGlassworkingKnowledge, "Glassworking", "Glass Vessel Blank", "finish",
		"finishing", 35, Difficulty.Hard,
		[
			"TagTool - Held - an item with the Blowpipe tag",
			"TagTool - Held - an item with the Pontil Rod tag",
			"TagTool - InRoom - an item with the Annealing Lehr tag"
		]);

	private static AntiquityHouseholdCraftPath MetalPath(string material) => new(
		IsPreciousMetal(material) ? "Silversmithing" : "Blacksmithing",
		IsPreciousMetal(material) ? "Silversmithing" : "Blacksmithing",
		AncientMetalVesselmakingKnowledge,
		"Metal Vessels",
		"Cast Vessel Blank",
		"finish",
		"finishing",
		IsPreciousMetal(material) ? 40 : 25,
		IsPreciousMetal(material) ? Difficulty.Hard : Difficulty.Normal,
		[
			"TagTool - Held - an item with the Crucible tag",
			"TagTool - Held - an item with the Crucible Tongs tag",
			"TagTool - Held - an item with the Bronze Burnisher tag"
		]);

	private static AntiquityHouseholdCraftPath StonePath(string material) => new(
		material.Equals("ivory", StringComparison.OrdinalIgnoreCase) || material.Equals("horn", StringComparison.OrdinalIgnoreCase)
			? "Scrimshawing"
			: "Masonry",
		material.Equals("ivory", StringComparison.OrdinalIgnoreCase) || material.Equals("horn", StringComparison.OrdinalIgnoreCase)
			? "Scrimshawing"
			: "Masonry",
		AncientStoneCarvingKnowledge,
		"Carving",
		"Stone Vessel Blank",
		"carve",
		"carving",
		30,
		Difficulty.Normal,
		[
			"TagTool - Held - an item with the Stone Chisel tag",
			"TagTool - Held - an item with the Stone Mallet tag",
			"TagTool - Held - an item with the Polishing Stone tag"
		]);

	private static AntiquityHouseholdCraftPath CandlePath() => new(
		"Candlemaking", "Candlemaking", AncientLightingKnowledge, "Lighting", "Candlemaking Wax", "mould",
		"moulding", 15, Difficulty.Easy,
		[
			"TagTool - InRoom - an item with the Candle Mould tag"
		]);

	private static AntiquityHouseholdCraftPath TorchPath() => new(
		"Candlemaking", "Candlemaking", AncientLightingKnowledge, "Lighting", "Furniture Timber Stock", "bind",
		"binding", 10, Difficulty.Easy,
		[
			"TagTool - Held - an item with the Basket Knife tag"
		]);

	private static AntiquityHouseholdCraftPath LacquerPath() => new(
		"Lacquerwork", "Lacquerwork", AncientHouseholdCraftingKnowledge, "Decoration", "Furniture Panel Stock", "lacquer",
		"lacquering", 35, Difficulty.Hard,
		[
			"TagTool - Held - an item with the Lacquerer's Brush tag",
			"TagTool - Held - an item with the Lacquer Spatula tag"
		]);

	private static AntiquityHouseholdCraftPath HouseholdGeneralPath() => new(
		"Crafting", "Crafting", AncientHouseholdCraftingKnowledge, "General", "Household Craft Stock", "make",
		"making", 15, Difficulty.Normal,
		[]);

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

	private static (int Seconds, string Echo, string FailEcho)[] GetAntiquityHouseholdFinalPhases(AntiquityHouseholdCraftPath path)
	{
		return
		[
			(30, "$0 lay|lays out $i1 and check|checks the prepared stock for defects.", "$0 lay|lays out $i1, but miss|misses an obvious flaw in the stock."),
			(40, "$0 work|works the main body of the piece with $t1.", "$0 work|works the piece poorly with $t1."),
			(35, "$0 fit|fits, trim|trims, and smooth|smooths the household piece into its final form.", "$0 fit|fits the piece together, but the form twists out of true."),
			(25, "$0 finish|finishes the work and set|sets aside $p1.", "$0 finish|finishes the work, but the result is not worth keeping.")
		];
	}

	private static string BuildAntiquityHouseholdProduct(GameItemProto item, AntiquityHouseholdVariableStyle variableStyle,
		int variableInputIndex)
	{
		return variableStyle switch
		{
			AntiquityHouseholdVariableStyle.BasicColour =>
				$"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable Colour=$i{variableInputIndex}",
			AntiquityHouseholdVariableStyle.FineColour =>
				$"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable Colour=$i{variableInputIndex}, Fine Colour=$i{variableInputIndex}",
			_ => $"SimpleProduct - 1x {item.ShortDescription} (#{item.Id})"
		};
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

	private static AntiquityHouseholdVariableSource GetHouseholdVariableSource(string stableReference, GameItemProto item,
		string material, AntiquityHouseholdVariableStyle variableStyle)
	{
		if (variableStyle is AntiquityHouseholdVariableStyle.None or AntiquityHouseholdVariableStyle.TwoColour)
		{
			return AntiquityHouseholdVariableSource.None;
		}

		var lowerStable = stableReference.ToLowerInvariant();
		var lowerDescription = item.ShortDescription.ToLowerInvariant();
		if (lowerDescription.Contains("painted", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("painted", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("black_figure", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("red_figure", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("bichrome", StringComparison.OrdinalIgnoreCase))
		{
			return AntiquityHouseholdVariableSource.PaintPigment;
		}

		if (lowerDescription.Contains("lacquer", StringComparison.OrdinalIgnoreCase) ||
		    lowerStable.Contains("lacquer", StringComparison.OrdinalIgnoreCase))
		{
			return AntiquityHouseholdVariableSource.LacquerFinish;
		}

		if (IsTextileMaterial(material) || IsLeatherMaterial(material) || IsGlassMaterial(material))
		{
			return AntiquityHouseholdVariableSource.PrimaryStock;
		}

		if (lowerDescription.Contains("leather", StringComparison.OrdinalIgnoreCase))
		{
			return AntiquityHouseholdVariableSource.LeatherPanel;
		}

		if (lowerDescription.Contains("linen", StringComparison.OrdinalIgnoreCase) ||
		    lowerDescription.Contains("cloth", StringComparison.OrdinalIgnoreCase) ||
		    lowerDescription.Contains("wool", StringComparison.OrdinalIgnoreCase) ||
		    lowerDescription.Contains("felt", StringComparison.OrdinalIgnoreCase))
		{
			return AntiquityHouseholdVariableSource.ClothPanel;
		}

		if (lowerDescription.Contains("rush", StringComparison.OrdinalIgnoreCase) ||
		    lowerDescription.Contains("reed", StringComparison.OrdinalIgnoreCase))
		{
			return AntiquityHouseholdVariableSource.ReedMatting;
		}

		return AntiquityHouseholdVariableSource.PrimaryStock;
	}

	private static (string Name, string Subtype, string Description) GetAntiquityHouseholdKnowledge(string stableReference,
		GameItemProto item, AntiquityHouseholdCraftPath path)
	{
		var culture = GetAntiquityHouseholdCulture(stableReference, item.ShortDescription);
		if (culture is not null)
		{
			return (AntiquityHouseholdCultureKnowledgeNames[culture], culture,
				$"{culture} household crafting knowledge for antiquity furniture, containers, and domestic goods.");
		}

		return (path.Knowledge, path.KnowledgeSubtype,
			$"{path.Knowledge} covers antiquity household crafts using {path.Skill} methods.");
	}

	private static string? GetAntiquityHouseholdCulture(string stableReference, string shortDescription)
	{
		var text = $"{stableReference} {shortDescription}".ToLowerInvariant();
		if (ContainsAny(text, "hellenic", "greek", "attic", "corinthian", "spartan", "athenian", "black_figure", "red_figure"))
		{
			return "Hellenic";
		}

		if (ContainsAny(text, "roman", "campanian", "terra_sigillata", "samian", "pompeian"))
		{
			return "Roman";
		}

		if (ContainsAny(text, "egyptian", "nilotic", "faience", "canopic"))
		{
			return "Egyptian";
		}

		if (ContainsAny(text, "kushite", "nubian", "meroitic"))
		{
			return "Kushite";
		}

		if (ContainsAny(text, "punic", "carthaginian"))
		{
			return "Punic";
		}

		if (ContainsAny(text, "persian", "achaemenid", "parthian"))
		{
			return "Persian";
		}

		if (ContainsAny(text, "etruscan", "bucchero"))
		{
			return "Etruscan";
		}

		if (ContainsAny(text, "anatolian", "phrygian", "lydian", "hittite", "urartian"))
		{
			return "Anatolian";
		}

		if (ContainsAny(text, "scythian", "sarmatian", "steppe"))
		{
			return "Scythian-Sarmatian";
		}

		if (ContainsAny(text, "celtic", "gallic", "la_tene"))
		{
			return "Celtic";
		}

		if (ContainsAny(text, "germanic", "gothic", "nordic"))
		{
			return "Germanic";
		}

		return null;
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

	private static bool IsWoodMaterial(string material)
	{
		return AntiquityHouseholdWoodMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsTextileMaterial(string material)
	{
		return AntiquityHouseholdTextileMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsLeatherMaterial(string material)
	{
		return AntiquityHouseholdLeatherMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsCeramicMaterial(string material)
	{
		return AntiquityHouseholdCeramicMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsGlassMaterial(string material)
	{
		return AntiquityHouseholdGlassMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsMetalMaterial(string material)
	{
		return AntiquityHouseholdMetalMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsPreciousMetal(string material)
	{
		return material.Equals("gold", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("silver", StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsStoneHornMaterial(string material)
	{
		return AntiquityHouseholdStoneHornMaterials.Contains(material, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsCoopered(string stableReference, string shortDescription)
	{
		return ContainsAny(stableReference, "barrel", "cask", "tub", "bucket", "coopered") ||
		       ContainsAny(shortDescription, "barrel", "cask", "tub", "bucket", "coopered");
	}

	private static bool IsBasketry(string stableReference, string shortDescription, string material)
	{
		return ContainsAny(stableReference, "basket", "wicker", "reed", "rush", "matting", "hamper") ||
		       ContainsAny(shortDescription, "basket", "wicker", "reed", "rush", "matting", "hamper") ||
		       material.Equals("papyrus", StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsPanelFurniture(string stableReference, string shortDescription)
	{
		return ContainsAny(stableReference, "chest", "box", "coffer", "cabinet", "cupboard", "footlocker", "screen", "shrine") ||
		       ContainsAny(shortDescription, "chest", "box", "coffer", "cabinet", "cupboard", "footlocker", "screen", "shrine");
	}
}
