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
		PrimaryProductionCraftSpecs()
			.Select(x => new PrimaryProductionCraftSpecTestData(
				x.Name,
				x.Category,
				x.Trait,
				x.KnowledgeSubtype,
				x.Inputs.ToArray(),
				x.Tools.ToArray(),
				x.Products.ToArray(),
				x.FailProducts.ToArray()))
			.ToArray();

	private void SeedPrimaryProductionCommodityCrafts()
	{
		if (!ShouldSeedHistoricCrafts())
		{
			return;
		}

		foreach (var spec in PrimaryProductionCraftSpecs())
		{
			AddPrimaryProductionCraft(spec);
		}
	}

	private Craft? AddPrimaryProductionCraft(PrimaryProductionCraftSpec spec)
	{
		return AddCraft(
			spec.Name,
			spec.Category,
			spec.Blurb,
			spec.Action,
			spec.ActiveDescription,
			PrimaryProductionKnowledge,
			spec.Trait,
			spec.MinimumTraitValue,
			spec.Difficulty,
			Outcome.MinorFail,
			5,
			3,
			false,
			PrimaryProductionCommodityPhases(),
			spec.Inputs,
			spec.Tools,
			spec.Products,
			spec.FailProducts,
			knowledgeSubtype: spec.KnowledgeSubtype,
			knowledgeDescription: "Shared historic primary-production commodity transformations.",
			knowledgeLongDescription: "Shared primary-production knowledge for turning raw deposits and bulk feedstocks into stock commodities used by mining, quarrying, smelting, masonry, clay, glass, salt, alkali, tar, peat, and pigment workflows.");
	}

	private static IReadOnlyList<PrimaryProductionCraftSpec> PrimaryProductionCraftSpecs()
	{
		return
		[
			Define(
				"assay iron ore sample",
				"Primary Production / Prospecting",
				"Labouring",
				10,
				Difficulty.Easy,
				"Prospecting",
				"assay an iron ore sample",
				[CommodityInput(150.0, "hematite", "Sample Ore Commodity")],
				[HeldTool("Prospecting Tool"), HeldTool("Smelting Tool")],
				[CommodityOutput(120.0, "hematite", "Sample Ore Commodity")],
				[CommodityOutput(30.0, "stone rubble", "Ore Tailings Commodity")]),

			DefineOrePreparation("iron", "hematite", true),
			DefineOreSorting("iron", "hematite"),
			DefineOreWashing("iron", "hematite"),
			DefineOreRoasting("iron", "hematite"),

			DefineOrePreparation("copper", "malachite", true),
			DefineOreSorting("copper", "malachite"),
			DefineOreWashing("copper", "malachite"),
			DefineOreRoasting("copper", "malachite"),

			DefineOrePreparation("tin", "cassiterite", true),
			DefineOreSorting("tin", "cassiterite"),
			DefineOreWashing("tin", "cassiterite"),

			DefineOrePreparation("lead", "galena", true),
			DefineOreSorting("lead", "galena"),
			DefineOreWashing("lead", "galena"),
			DefineOreRoasting("lead", "galena"),

			Define(
				"dress rough limestone block",
				"Primary Production / Masonry",
				"Masonry",
				20,
				Difficulty.Normal,
				"Masonry",
				"dress rough limestone into squared building stone",
				[CommodityInput(30000.0, "limestone", "Rough Stone Block Commodity")],
				[HeldTool("Masonry Tool"), HeldTool("Quarrying Tool")],
				[CommodityOutput(24000.0, "limestone", "Dressed Stone Block Commodity")],
				[CommodityOutput(6000.0, "stone rubble", "Stone Rubble Commodity")]),

			Define(
				"dress rough sandstone block",
				"Primary Production / Masonry",
				"Masonry",
				20,
				Difficulty.Normal,
				"Masonry",
				"dress rough sandstone into squared building stone",
				[CommodityInput(30000.0, "sandstone", "Rough Stone Block Commodity")],
				[HeldTool("Masonry Tool"), HeldTool("Quarrying Tool")],
				[CommodityOutput(24000.0, "sandstone", "Dressed Stone Block Commodity")],
				[CommodityOutput(6000.0, "stone rubble", "Stone Rubble Commodity")]),

			Define(
				"break rubble into aggregate",
				"Primary Production / Masonry",
				"Labouring",
				10,
				Difficulty.Easy,
				"Masonry",
				"break rubble into graded aggregate",
				[CommodityInput(20000.0, "stone rubble", "Stone Rubble Commodity")],
				[HeldTool("Masonry Tool")],
				[CommodityOutput(15000.0, "gravel", "Aggregate Commodity")],
				[CommodityOutput(4000.0, "stone rubble", "Mine Spoil Commodity")]),

			Define(
				"slake quicklime",
				"Primary Production / Binders",
				"Masonry",
				20,
				Difficulty.Normal,
				"Binders",
				"slake quicklime with water",
				[CommodityInput(5000.0, "calcium oxide", "Quicklime Commodity"), WaterUse(8.0)],
				[HeldTool("Masonry Tool")],
				[CommodityOutput(6500.0, "calcium hydroxide", "Slaked Lime Commodity")],
				[CommodityOutput(750.0, "calcium oxide", "Primary Production Waste")]),

			Define(
				"mix lime mortar",
				"Primary Production / Binders",
				"Masonry",
				20,
				Difficulty.Easy,
				"Binders",
				"mix slaked lime and sand into mortar",
				[
					CommodityInput(5000.0, "calcium hydroxide", "Slaked Lime Commodity"),
					CommodityInput(10000.0, "sand", "Aggregate Commodity"),
					WaterUse(5.0)
				],
				[HeldTool("Masonry Tool")],
				[CommodityOutput(14000.0, "mortar", "Mortar Commodity")],
				[CommodityOutput(1000.0, "stone rubble", "Primary Production Waste")]),

			Define(
				"calcine gypsum plaster",
				"Primary Production / Binders",
				"Masonry",
				25,
				Difficulty.Normal,
				"Binders",
				"calcine gypsum into plaster stock",
				[CommodityInput(8000.0, "gypsum"), CommodityInput(2000.0, "charcoal", "Charcoal Fuel Commodity")],
				[RoomTool("Kiln Tool")],
				[CommodityOutput(6500.0, "plaster", "Mortar Commodity")],
				[CommodityOutput(1000.0, "stone rubble", "Primary Production Waste")]),

			Define(
				"temper clay body",
				"Primary Production / Clay",
				"Pottery",
				15,
				Difficulty.Easy,
				"Clay",
				"temper clay with sand and water",
				[CommodityInput(9000.0, "clay"), CommodityInput(1500.0, "sand", "Aggregate Commodity"), WaterUse(4.0)],
				[HeldTool("Kiln Tool")],
				[CommodityOutput(9500.0, "prepared clay", "Prepared Clay Commodity")],
				[CommodityOutput(1000.0, "clay", "Primary Production Waste")]),

			Define(
				"mould green bricks",
				"Primary Production / Brickmaking",
				"Pottery",
				15,
				Difficulty.Easy,
				"Brickmaking",
				"mould prepared clay into green brick stock",
				[CommodityInput(10000.0, "prepared clay", "Prepared Clay Commodity")],
				[HeldTool("Kiln Tool")],
				[CommodityOutput(9000.0, "green brick", "Green Brick Commodity")],
				[CommodityOutput(750.0, "prepared clay", "Primary Production Waste")]),

			Define(
				"mould roof tiles",
				"Primary Production / Tilemaking",
				"Pottery",
				20,
				Difficulty.Normal,
				"Tilemaking",
				"mould prepared clay into roof tile stock",
				[CommodityInput(10000.0, "prepared clay", "Prepared Clay Commodity")],
				[HeldTool("Kiln Tool")],
				[CommodityOutput(8500.0, "roof tile", "Roof Tile Commodity")],
				[CommodityOutput(1000.0, "prepared clay", "Primary Production Waste")]),

			Define(
				"prepare refractory clay body",
				"Primary Production / Refractory",
				"Pottery",
				25,
				Difficulty.Normal,
				"Refractory",
				"temper fire clay into refractory clay body",
				[CommodityInput(8000.0, "fire clay", "Fireclay Commodity"), CommodityInput(2000.0, "sand", "Aggregate Commodity"), WaterUse(3.0)],
				[HeldTool("Kiln Tool")],
				[CommodityOutput(9000.0, "fire clay", "Crucible Clay Commodity")],
				[CommodityOutput(750.0, "stone rubble", "Primary Production Waste")]),

			Define(
				"mould clay crucible stock",
				"Primary Production / Refractory",
				"Pottery",
				25,
				Difficulty.Normal,
				"Refractory",
				"mould refractory clay into crucible stock",
				[CommodityInput(5000.0, "fire clay", "Crucible Clay Commodity")],
				[HeldTool("Smelting Tool"), HeldTool("Kiln Tool")],
				[CommodityOutput(4200.0, "fire clay", "Crucible Clay Commodity")],
				[CommodityOutput(500.0, "fire clay", "Primary Production Waste")]),

			Define(
				"prepare soda lime glass batch",
				"Primary Production / Glass",
				"Glassworking",
				25,
				Difficulty.Normal,
				"Glass",
				"mix sand, soda, and lime into glass batch",
				[
					CommodityInput(7000.0, "sand", "Aggregate Commodity"),
					CommodityInput(1500.0, "soda ash", "Potash Commodity"),
					CommodityInput(1500.0, "calcium oxide", "Quicklime Commodity")
				],
				[HeldTool("Kiln Tool")],
				[CommodityOutput(9000.0, "glass batch", "Glass Batch Commodity")],
				[CommodityOutput(1000.0, "stone rubble", "Primary Production Waste")]),

			Define(
				"consolidate iron bloom",
				"Primary Production / Ironworking",
				"Smelting",
				30,
				Difficulty.Hard,
				"Ironworking",
				"consolidate an iron bloom into billet stock",
				[CommodityInput(8000.0, "iron bloom", "Bloom Commodity"), CommodityInput(2000.0, "charcoal", "Charcoal Fuel Commodity")],
				[RoomTool("Smelting Tool"), HeldTool("Smelting Tool")],
				[CommodityOutput(5500.0, "wrought iron billet", "Metal Billet Commodity")],
				[CommodityOutput(2000.0, "slag", "Slag Commodity")]),

			Define(
				"draw wrought iron bar stock",
				"Primary Production / Ironworking",
				"Blacksmithing",
				30,
				Difficulty.Hard,
				"Ironworking",
				"draw wrought iron billet into bar stock",
				[CommodityInput(5000.0, "wrought iron billet", "Metal Billet Commodity"), CommodityInput(1200.0, "charcoal", "Charcoal Fuel Commodity")],
				[RoomTool("Smelting Tool"), HeldTool("Smelting Tool")],
				[CommodityOutput(4300.0, "wrought iron", "Metal Bar Stock Commodity")],
				[CommodityOutput(600.0, "slag", "Slag Commodity")]),

			Define(
				"cast copper ingot",
				"Primary Production / Non-Ferrous",
				"Smelting",
				25,
				Difficulty.Hard,
				"Non-Ferrous Metals",
				"cast copper into ingot stock",
				[CommodityInput(5000.0, "copper"), CommodityInput(1200.0, "charcoal", "Charcoal Fuel Commodity")],
				[RoomTool("Smelting Tool"), HeldTool("Smelting Tool")],
				[CommodityOutput(4600.0, "copper", "Metal Ingot Commodity")],
				[CommodityOutput(500.0, "slag", "Slag Commodity")]),

			Define(
				"cast tin ingot",
				"Primary Production / Non-Ferrous",
				"Smelting",
				25,
				Difficulty.Normal,
				"Non-Ferrous Metals",
				"cast tin into ingot stock",
				[CommodityInput(4000.0, "tin"), CommodityInput(800.0, "charcoal", "Charcoal Fuel Commodity")],
				[RoomTool("Smelting Tool"), HeldTool("Smelting Tool")],
				[CommodityOutput(3650.0, "tin", "Metal Ingot Commodity")],
				[CommodityOutput(300.0, "slag", "Slag Commodity")]),

			Define(
				"alloy bronze billet",
				"Primary Production / Alloying",
				"Smelting",
				30,
				Difficulty.Hard,
				"Alloying",
				"alloy copper and tin into bronze billet stock",
				[
					CommodityInput(4500.0, "copper", "Metal Ingot Commodity"),
					CommodityInput(600.0, "tin", "Metal Ingot Commodity"),
					CommodityInput(1200.0, "charcoal", "Charcoal Fuel Commodity")
				],
				[RoomTool("Smelting Tool"), HeldTool("Smelting Tool")],
				[CommodityOutput(4600.0, "bronze", "Metal Billet Commodity")],
				[CommodityOutput(400.0, "slag", "Slag Commodity")]),

			Define(
				"crush rock salt",
				"Primary Production / Salt",
				"Labouring",
				10,
				Difficulty.Easy,
				"Salt",
				"crush rock salt into usable salt stock",
				[CommodityInput(6000.0, "halite", "Raw Ore Commodity")],
				[HeldTool("Saltworking Tool")],
				[CommodityOutput(5200.0, "salt", "Salt Commodity")],
				[CommodityOutput(500.0, "stone rubble", "Primary Production Waste")]),

			Define(
				"leach wood ash",
				"Primary Production / Alkali",
				"Surviving",
				15,
				Difficulty.Normal,
				"Alkali",
				"leach wood ash into lye stock",
				[CommodityInput(5000.0, "wood ash"), WaterUse(10.0)],
				[HeldTool("Alkali Tool")],
				[CommodityOutput(3500.0, "lye", "Lye Commodity")],
				[CommodityOutput(1000.0, "wood ash", "Primary Production Waste")]),

			Define(
				"evaporate lye to potash",
				"Primary Production / Alkali",
				"Labouring",
				15,
				Difficulty.Normal,
				"Alkali",
				"evaporate lye into potash stock",
				[CommodityInput(4000.0, "lye", "Lye Commodity"), CommodityInput(1500.0, "charcoal", "Charcoal Fuel Commodity")],
				[RoomTool("Alkali Tool"), RoomTool("Kiln Tool")],
				[CommodityOutput(1400.0, "potash", "Potash Commodity")],
				[CommodityOutput(500.0, "wood ash", "Primary Production Waste")]),

			Define(
				"boil tar into pitch",
				"Primary Production / Tar And Pitch",
				"Surviving",
				15,
				Difficulty.Normal,
				"Tar And Pitch",
				"boil tar into thick pitch",
				[CommodityInput(6000.0, "tar", "Tar Commodity"), CommodityInput(1500.0, "charcoal", "Charcoal Fuel Commodity")],
				[RoomTool("Tar Burning Tool"), HeldTool("Tar Burning Tool")],
				[CommodityOutput(4200.0, "pitch", "Pitch Commodity")],
				[CommodityOutput(800.0, "tar", "Primary Production Waste")]),

			Define(
				"prepare pitch sealant",
				"Primary Production / Tar And Pitch",
				"Labouring",
				10,
				Difficulty.Easy,
				"Tar And Pitch",
				"prepare pitch into spreadable sealant stock",
				[CommodityInput(4000.0, "pitch", "Pitch Commodity"), CommodityInput(400.0, "wood ash")],
				[HeldTool("Tar Burning Tool")],
				[CommodityOutput(3800.0, "pitch", "Pitch Commodity")],
				[CommodityOutput(400.0, "wood ash", "Primary Production Waste")]),

			Define(
				"grind ochre pigment",
				"Primary Production / Pigment",
				"Painting",
				15,
				Difficulty.Easy,
				"Pigment",
				"grind ochre into fine pigment stock",
				[CommodityInput(2000.0, "ochre pigment")],
				[HeldTool("Pigment Processing Tool")],
				[CommodityOutput(1600.0, "ochre pigment", "Pigment Commodity")],
				[CommodityOutput(250.0, "stone rubble", "Primary Production Waste")]),

			Define(
				"prepare malachite pigment",
				"Primary Production / Pigment",
				"Painting",
				20,
				Difficulty.Normal,
				"Pigment",
				"grind and wash malachite into green pigment stock",
				[CommodityInput(2000.0, "malachite", "Sample Ore Commodity"), WaterUse(1.0)],
				[HeldTool("Pigment Processing Tool")],
				[CommodityOutput(1200.0, "malachite pigment", "Pigment Commodity")],
				[CommodityOutput(500.0, "stone rubble", "Ore Tailings Commodity")]),

			Define(
				"cut dried peat blocks",
				"Primary Production / Peat",
				"Labouring",
				10,
				Difficulty.Easy,
				"Peat",
				"trim dried peat into stackable fuel blocks",
				[CommodityInput(10000.0, "dried peat")],
				[HeldTool("Peat Cutting Tool")],
				[CommodityOutput(9000.0, "dried peat", "Peat Fuel Commodity")],
				[CommodityOutput(500.0, "dried peat", "Primary Production Waste")])
		];
	}

	private static PrimaryProductionCraftSpec DefineOrePreparation(string ore, string material, bool producesTailings)
	{
		return Define(
			$"break raw {ore} ore",
			"Primary Production / Ore Preparation",
			"Labouring",
			10,
			Difficulty.Easy,
			"Ore Preparation",
			$"break raw {ore} ore into sortable pieces",
			[CommodityInput(10000.0, material, "Raw Ore Commodity")],
			[HeldTool("Mining Tool")],
			[CommodityOutput(8500.0, material, "Broken Ore Commodity")],
			producesTailings
				? [CommodityOutput(1000.0, "stone rubble", "Ore Tailings Commodity")]
				: []);
	}

	private static PrimaryProductionCraftSpec DefineOreSorting(string ore, string material)
	{
		return Define(
			$"sort broken {ore} ore",
			"Primary Production / Ore Preparation",
			"Labouring",
			15,
			Difficulty.Easy,
			"Ore Preparation",
			$"sort broken {ore} ore by hand",
			[CommodityInput(8500.0, material, "Broken Ore Commodity")],
			[HeldTool("Mining Tool"), HeldTool("Hauling Tool")],
			[CommodityOutput(6500.0, material, "Sorted Ore Commodity")],
			[CommodityOutput(1500.0, "stone rubble", "Ore Tailings Commodity")]);
	}

	private static PrimaryProductionCraftSpec DefineOreWashing(string ore, string material)
	{
		return Define(
			$"wash sorted {ore} ore",
			"Primary Production / Ore Preparation",
			"Labouring",
			15,
			Difficulty.Normal,
			"Ore Preparation",
			$"wash sorted {ore} ore into clean concentrate",
			[CommodityInput(6500.0, material, "Sorted Ore Commodity"), WaterUse(6.0)],
			[HeldTool("Mining Tool"), HeldTool("Hauling Tool")],
			[CommodityOutput(5200.0, material, "Washed Ore Commodity")],
			[CommodityOutput(900.0, "stone rubble", "Ore Tailings Commodity")]);
	}

	private static PrimaryProductionCraftSpec DefineOreRoasting(string ore, string material)
	{
		return Define(
			$"roast washed {ore} ore",
			"Primary Production / Ore Preparation",
			"Smelting",
			20,
			Difficulty.Normal,
			"Ore Preparation",
			$"roast washed {ore} ore for smelting",
			[CommodityInput(5200.0, material, "Washed Ore Commodity"), CommodityInput(1500.0, "charcoal", "Charcoal Fuel Commodity")],
			[RoomTool("Kiln Tool"), RoomTool("Smelting Tool")],
			[CommodityOutput(4800.0, material, "Roasted Ore Commodity")],
			[CommodityOutput(500.0, "stone rubble", "Ore Tailings Commodity")]);
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
		var prefixedName = $"primary production - {name}";
		return new PrimaryProductionCraftSpec(
			prefixedName,
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

	private static (int Seconds, string Echo, string FailEcho)[] PrimaryProductionCommodityPhases()
	{
		return
		[
			(25, "$0 inspect|inspects $i1 and set|sets up the working area.", "$0 inspect|inspects $i1, but miss|misses important impurities and awkward pieces."),
			(45, "$0 work|works the stock with $t1, keeping the batch moving and sorted.", "$0 work|works the stock unevenly with $t1."),
			(40, "$0 finish|finishes the transformation and set|sets aside $p1.", "$0 botch|botches the final handling and recover|recovers only $f1.")
		];
	}

	private static string CommodityOutput(double grams, string material, string tag)
	{
		return $"CommodityProduct - {FormatCommodityAmount(grams)} of {material} commodity; tag {tag}";
	}

	private static string HeldTool(string tag)
	{
		return $"TagTool - Held - an item with the {tag} tag";
	}

	private static string RoomTool(string tag)
	{
		return $"TagTool - InRoom - an item with the {tag} tag";
	}

	private static string WaterUse(double litres)
	{
		return $"LiquidUse - {litres.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)} litres of Water";
	}
}
