#nullable enable

using MudSharp.Models;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string HistoricFoundationKnowledge = "Historic Workshop Foundations";
	private const string MedievalWorkshopKnowledge = "Medieval Workshop Practice";
	private const string MedievalClothingKnowledgePrefix = "Medieval Clothing Pattern";
	private const string MedievalArmourKnowledgePrefix = "Medieval Armour Pattern";
	private const string MedievalWeaponKnowledgePrefix = "Medieval Weapon Pattern";
	private const string MedievalShieldKnowledgePrefix = "Medieval Shield Pattern";
	private const string MedievalFoodKnowledgePrefix = "Medieval Foodway Pattern";
	private const string MedievalAdministrationKnowledgePrefix = "Medieval Administration Pattern";

	private bool ShouldSeedHistoricCrafts()
	{
		return _questionAnswers?.TryGetValue("eras", out var eras) == true &&
		       (eras.Contains("antiquity", StringComparison.InvariantCultureIgnoreCase) ||
		        eras.Contains("medieval", StringComparison.InvariantCultureIgnoreCase));
	}

	private bool ShouldSeedMedievalCrafts()
	{
		return _questionAnswers?.TryGetValue("eras", out var eras) == true &&
		       eras.Contains("medieval", StringComparison.InvariantCultureIgnoreCase);
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

	private Craft? AddMedievalCraft(
		string name,
		string category,
		string blurb,
		string action,
		string itemDescription,
		string knowledgeName,
		string traitName,
		int? minimumTraitValue,
		Difficulty difficulty,
		IEnumerable<(int Seconds, string Echo, string FailEcho)> phases,
		IEnumerable<string> inputs,
		IEnumerable<string> tools,
		IEnumerable<string> products,
		IEnumerable<string>? failProducts = null,
		string knowledgeSubtype = "General",
		string? knowledgeDescription = null)
	{
		return AddCraft(
			name,
			category,
			blurb,
			action,
			itemDescription,
			knowledgeName,
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
			knowledgeDescription: knowledgeDescription ?? $"{knowledgeName} covers medieval {knowledgeSubtype.ToLowerInvariant()} without culture names in visible craft text.",
			knowledgeLongDescription: knowledgeDescription ?? $"{knowledgeName} covers medieval {knowledgeSubtype.ToLowerInvariant()} without culture names in visible craft text.");
	}

	private static (int Seconds, string Echo, string FailEcho)[] MedievalFinishedPhases()
	{
		return
		[
			(30, "$0 lay|lays out the prepared stock and check|checks it for flaws.", "$0 lay|lays out the prepared stock, but miss|misses several serious flaws."),
			(45, "$0 shape|shapes, trim|trims, and fit|fits the working pieces together.", "$0 shape|shapes the working pieces poorly, leaving awkward joins and weak points."),
			(45, "$0 bind|binds, stitch|stitches, rivet|rivets, or finish|finishes the main assembly.", "$0 botch|botches the finishing work and spoil|spoils the assembly."),
			(30, "$0 set|sets aside $p1 and inspect|inspects the finished work.", "$0 set|sets aside only $f1 after the work fails.")
		];
	}

	private static (int Seconds, string Echo, string FailEcho)[] MedievalLightingPhases()
	{
		return
		[
			(20, "$0 prepare|prepares the fuel, wick, or charcoal bed.", "$0 prepare|prepares the fuel poorly, scattering it around the work area."),
			(25, "$0 coax|coaxes the flame into the prepared item.", "$0 coax|coaxes the flame badly, and it will not take."),
			(20, "$0 set|sets aside $p1 once the flame is steady.", "$0 end|ends up with only $f1 after the flame fails.")
		];
	}

	private static (int Seconds, string Echo, string FailEcho)[] MedievalStockPhases()
	{
		return
		[
			(25, "$0 sort|sorts the prepared materials and set|sets the workshop tools in order.", "$0 sort|sorts the materials poorly and miss|misses obvious flaws."),
			(45, "$0 work|works the materials through the required medieval process.", "$0 work|works the materials unevenly and spoil|spoils part of the batch."),
			(30, "$0 finish|finishes the batch and set|sets aside $p1.", "$0 finish|finishes badly and leave|leaves only $f1.")
		];
	}

	private static string CommodityOutput(double grams, string material, string pileTag, bool colour = false,
		bool fineColour = false)
	{
		var text = $"CommodityProduct - {FormatCommodityAmount(grams)} of {material} commodity; tag {pileTag}";
		if (colour)
		{
			text += "; characteristic Colour from $i1";
		}

		if (fineColour)
		{
			text += "; characteristic Fine Colour from $i1";
		}

		return text;
	}

	private static string BuildRegionalCraftName(string verb, string subject, int index)
	{
		return $"{verb} {subject} regional pattern {index.ToString("00", System.Globalization.CultureInfo.InvariantCulture)}";
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
				MedievalFinishedPhases(),
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
				MedievalLightingPhases(),
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

	private void SeedMedievalProductionChainCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		foreach (var spec in MedievalHouseholdToolItemSpecs())
		{
			AddMedievalGeneralSpecCraft(spec, MedievalSpecCraftName("make", spec), "Production Tools");
		}

		void AddStockCraft(string name, string category, string traitName, int minimumTraitValue,
			Difficulty difficulty, IEnumerable<string> inputs, IEnumerable<string> tools,
			IEnumerable<string> products)
		{
			AddMedievalCraft(
				name,
				category,
				name,
				name,
				$"{name} in progress",
				MedievalWorkshopKnowledge,
				traitName,
				minimumTraitValue,
				difficulty,
				MedievalStockPhases(),
				inputs,
				tools,
				products,
				knowledgeSubtype: "Production Chains",
				knowledgeDescription: "Medieval workshop production chains for textiles, leather, armour, books, seals, food, glass, ceramics, and trade measures.");
		}

		AddStockCraft("spin linen yarn stock", "Tailoring", "Tailoring", 10, Difficulty.Easy,
			[CommodityInput(650.0, "linen")],
			["TagTool - Held - an item with the Drop Spindle tag"],
			[CommodityOutput(520.0, "linen", "Spun Yarn", colour: true, fineColour: true)]);
		AddStockCraft("spin wool yarn stock", "Tailoring", "Tailoring", 10, Difficulty.Easy,
			[CommodityInput(700.0, "wool")],
			["TagTool - Held - an item with the Drop Spindle tag"],
			[CommodityOutput(560.0, "wool", "Spun Yarn", colour: true, fineColour: true)]);
		AddStockCraft("spin silk yarn stock", "Tailoring", "Tailoring", 20, Difficulty.Normal,
			[CommodityInput(420.0, "silk")],
			["TagTool - Held - an item with the Drop Spindle tag"],
			[CommodityOutput(330.0, "silk", "Spun Yarn", colour: true, fineColour: true)]);
		AddStockCraft("spin cotton yarn stock", "Tailoring", "Tailoring", 10, Difficulty.Easy,
			[CommodityInput(650.0, "cotton")],
			["TagTool - Held - an item with the Drop Spindle tag"],
			[CommodityOutput(520.0, "cotton", "Spun Yarn", colour: true, fineColour: true)]);
		AddStockCraft("weave linen garment cloth stock", "Tailoring", "Tailoring", 15, Difficulty.Normal,
			[CommodityInput(620.0, "linen", "Spun Yarn", colour: true, fineColour: true)],
			["TagTool - InRoom - an item with the Hand Loom tag"],
			[CommodityOutput(500.0, "linen", "Garment Cloth", colour: true, fineColour: true)]);
		AddStockCraft("weave wool garment cloth stock", "Tailoring", "Tailoring", 15, Difficulty.Normal,
			[CommodityInput(680.0, "wool", "Spun Yarn", colour: true, fineColour: true)],
			["TagTool - InRoom - an item with the Hand Loom tag"],
			[CommodityOutput(540.0, "wool", "Garment Cloth", colour: true, fineColour: true)]);
		AddStockCraft("weave silk garment cloth stock", "Tailoring", "Tailoring", 25, Difficulty.Hard,
			[CommodityInput(520.0, "silk", "Spun Yarn", colour: true, fineColour: true)],
			["TagTool - InRoom - an item with the Hand Loom tag"],
			[CommodityOutput(420.0, "silk", "Garment Cloth", colour: true, fineColour: true)]);
		AddStockCraft("weave cotton garment cloth stock", "Tailoring", "Tailoring", 15, Difficulty.Normal,
			[CommodityInput(620.0, "cotton", "Spun Yarn", colour: true, fineColour: true)],
			["TagTool - InRoom - an item with the Hand Loom tag"],
			[CommodityOutput(500.0, "cotton", "Garment Cloth", colour: true, fineColour: true)]);
		AddStockCraft("full wool cloth stock", "Tailoring", "Tailoring", 20, Difficulty.Normal,
			[
				CommodityInput(900.0, "wool", "Garment Cloth", colour: true, fineColour: true),
				CommodityInput(80.0, "chalk dust")
			],
			[
				"TagTool - InRoom - an item with the Fulling Stocks tag",
				"TagTool - InRoom - an item with the Cloth Tenter Frame tag"
			],
			[CommodityOutput(760.0, "wool", "Fulled Cloth", colour: true, fineColour: true)]);
		AddStockCraft("full felt cloth stock", "Tailoring", "Tailoring", 20, Difficulty.Normal,
			[
				CommodityInput(820.0, "wool", "Garment Cloth", colour: true, fineColour: true),
				CommodityInput(80.0, "chalk dust")
			],
			[
				"TagTool - InRoom - an item with the Fulling Stocks tag",
				"TagTool - InRoom - an item with the Cloth Tenter Frame tag"
			],
			[CommodityOutput(680.0, "felt", "Fulled Cloth", colour: true, fineColour: true)]);
		AddStockCraft("prepare leather panel stock", "Leathermaking", "Leathermaking", 15, Difficulty.Normal,
			[
				CommodityInput(1200.0, "leather"),
				CommodityInput(80.0, "beeswax")
			],
			["TagTool - InRoom - an item with the Tanning Rack tag", "TagTool - Held - an item with the Awl Punch tag"],
			[CommodityOutput(960.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true)]);
		AddStockCraft("prepare fur panel stock", "Leathermaking", "Leathermaking", 15, Difficulty.Normal,
			[CommodityInput(650.0, "fur")],
			["TagTool - InRoom - an item with the Tanning Rack tag", "TagTool - Held - an item with the Shears tag"],
			[CommodityOutput(500.0, "fur", "Fur Panel Stock", colour: true, fineColour: true)]);
		AddStockCraft("cut leather strap stock", "Leathermaking", "Leathermaking", 15, Difficulty.Easy,
			[CommodityInput(520.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true)],
			["TagTool - Held - an item with the Shears tag", "TagTool - Held - an item with the Awl Punch tag"],
			[CommodityOutput(420.0, "leather", "Leather Strap", colour: true, fineColour: true)]);
		AddStockCraft("saw furniture timber stock", "Carpentry", "Carpentry", 15, Difficulty.Easy,
			[CommodityInput(2400.0, "oak")],
			["TagTool - Held - an item with the Planer tag", "TagTool - Held - an item with the Hammer tag"],
			[CommodityOutput(2000.0, "oak", "Furniture Timber Stock")]);
		AddStockCraft("plane furniture panel stock", "Carpentry", "Carpentry", 20, Difficulty.Normal,
			[CommodityInput(1800.0, "oak", "Furniture Timber Stock")],
			["TagTool - Held - an item with the Planer tag", "TagTool - Held - an item with the Awl Punch tag"],
			[CommodityOutput(1400.0, "oak", "Furniture Panel Stock")]);
		AddStockCraft("mix pottery clay body stock", "Pottery", "Pottery", 10, Difficulty.Easy,
			[
				CommodityInput(1600.0, "clay"),
				"LiquidUse - 1 litre of Water"
			],
			["TagTool - InRoom - an item with the Hot Fire tag"],
			[CommodityOutput(1400.0, "earthenware", "Pottery Clay Body")]);
		AddStockCraft("prepare glass batch stock", "Glassworking", "Glassworking", 20, Difficulty.Normal,
			[
				CommodityInput(900.0, "sand"),
				CommodityInput(220.0, "slaked lime")
			],
			["TagTool - InRoom - an item with the Hot Fire tag"],
			[CommodityOutput(980.0, "glass", "Glass Batch", colour: true, fineColour: true)]);
		AddStockCraft("blow glass vessel blank stock", "Glassworking", "Glassworking", 25, Difficulty.Normal,
			[CommodityInput(780.0, "glass", "Glass Batch", colour: true, fineColour: true)],
			["TagTool - InRoom - an item with the Hot Fire tag"],
			[CommodityOutput(620.0, "glass", "Glass Vessel Blank", colour: true, fineColour: true)]);
		AddStockCraft("forge iron tool blank stock", "Blacksmithing", "Blacksmithing", 15, Difficulty.Normal,
			[CommodityInput(1200.0, "wrought iron")],
			[
				"TagTool - InRoom - an item with the Anvil tag",
				"TagTool - Held - an item with the Hammer tag",
				"TagTool - Held - an item with the Forge Tongs tag"
			],
			[CommodityOutput(1000.0, "wrought iron", "Tool Blank Stock")]);
		AddStockCraft("cast bronze tool blank stock", "Blacksmithing", "Blacksmithing", 15, Difficulty.Normal,
			[CommodityInput(1000.0, "bronze")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Forge Tongs tag"],
			[CommodityOutput(850.0, "bronze", "Tool Blank Stock")]);
		AddStockCraft("draw silver tool blank stock", "Silversmithing", "Silversmithing", 20, Difficulty.Normal,
			[CommodityInput(300.0, "silver")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"],
			[CommodityOutput(240.0, "silver", "Tool Blank Stock")]);
		AddStockCraft("carve wooden tool blank stock", "Carpentry", "Carpentry", 10, Difficulty.Easy,
			[CommodityInput(900.0, "oak")],
			["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"],
			[CommodityOutput(720.0, "oak", "Tool Blank Stock")]);
		AddStockCraft("split willow tool blank stock", "Carpentry", "Carpentry", 10, Difficulty.Easy,
			[CommodityInput(700.0, "willow")],
			["TagTool - Held - an item with the Awl Punch tag"],
			[CommodityOutput(560.0, "willow", "Tool Blank Stock")]);
		AddStockCraft("shape bone tool blank stock", "Bonecarving", "Bonecarving", 15, Difficulty.Normal,
			[CommodityInput(500.0, "bone")],
			["TagTool - Held - an item with the Awl Punch tag"],
			[CommodityOutput(380.0, "bone", "Tool Blank Stock")]);
		AddStockCraft("shape stone tool blank stock", "Masonry", "Masonry", 15, Difficulty.Normal,
			[CommodityInput(1800.0, "stone")],
			["TagTool - Held - an item with the Hammer tag"],
			[CommodityOutput(1400.0, "stone", "Tool Blank Stock")]);
		AddStockCraft("twist hemp cord stock", "Ropemaking", "Ropemaking", 10, Difficulty.Easy,
			[CommodityInput(520.0, "hemp")],
			["TagTool - Held - an item with the Shears tag"],
			[CommodityOutput(420.0, "hemp", "Tool Blank Stock")]);
		AddStockCraft("shape weapon shaft stock", "Carpentry", "Carpentry", 15, Difficulty.Normal,
			[CommodityInput(900.0, "oak", "Tool Blank Stock")],
			["TagTool - Held - an item with the Planer tag"],
			[CommodityOutput(720.0, "oak", "Weapon Shaft Stock")]);
		AddStockCraft("forge weapon blade stock", "Weaponcrafting", "Weaponcrafting", 20, Difficulty.Normal,
			[CommodityInput(1200.0, "wrought iron", "Tool Blank Stock")],
			[
				"TagTool - InRoom - an item with the Anvil tag",
				"TagTool - Held - an item with the Hammer tag",
				"TagTool - Held - an item with the Forge Tongs tag"
			],
			[CommodityOutput(960.0, "wrought iron", "Weapon Blade Stock")]);
		AddStockCraft("forge weapon head stock", "Weaponcrafting", "Weaponcrafting", 20, Difficulty.Normal,
			[CommodityInput(900.0, "wrought iron", "Tool Blank Stock")],
			[
				"TagTool - InRoom - an item with the Anvil tag",
				"TagTool - Held - an item with the Hammer tag",
				"TagTool - Held - an item with the Forge Tongs tag"
			],
			[CommodityOutput(720.0, "wrought iron", "Weapon Head Stock")]);
		AddStockCraft("cut fletching stock", "Tailoring", "Tailoring", 10, Difficulty.Easy,
			[CommodityInput(160.0, "linen", "Garment Cloth", colour: true, fineColour: true)],
			["TagTool - Held - an item with the Shears tag"],
			[CommodityOutput(120.0, "linen", "Fletching Stock", colour: true, fineColour: true)]);
		AddStockCraft("twist military cord stock", "Ropemaking", "Ropemaking", 15, Difficulty.Easy,
			[
				CommodityInput(320.0, "linen", "Spun Yarn", colour: true, fineColour: true),
				CommodityInput(40.0, "beeswax")
			],
			["TagTool - Held - an item with the Shears tag"],
			[CommodityOutput(260.0, "linen", "Military Cord Stock", colour: true, fineColour: true)]);
		AddStockCraft("shape shield board stock", "Carpentry", "Carpentry", 20, Difficulty.Normal,
			[CommodityInput(2600.0, "oak", "Furniture Panel Stock")],
			["TagTool - Held - an item with the Planer tag", "TagTool - Held - an item with the Hammer tag"],
			[CommodityOutput(2200.0, "oak", "Shield Board Stock")]);
		AddStockCraft("stretch shield facing stock", "Leathermaking", "Leathermaking", 20, Difficulty.Normal,
			[CommodityInput(900.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true)],
			["TagTool - InRoom - an item with the Tanning Rack tag", "TagTool - Held - an item with the Shears tag"],
			[CommodityOutput(760.0, "leather", "Shield Facing Stock", colour: true, fineColour: true)]);
		AddStockCraft("cut armour lamella stock", "Armourcrafting", "Armourcrafting", 25, Difficulty.Hard,
			[CommodityInput(1200.0, "wrought iron", "Tool Blank Stock")],
			[
				"TagTool - InRoom - an item with the Armourer's Anvil tag",
				"TagTool - Held - an item with the Planishing Hammer tag"
			],
			[CommodityOutput(920.0, "wrought iron", "Armour Lamella Stock")]);
		AddStockCraft("mill flour commodity stock", "Milling", "Milling", 10, Difficulty.Easy,
			[CommodityInput(900.0, "wheat")],
			["TagTool - Held - an item with the Rotary Quern tag"],
			[CommodityOutput(720.0, "wheat", "Flour Commodity")]);

		AddStockCraft("full broadcloth stock", "Tailoring", "Tailoring", 25, Difficulty.Normal,
			[
				CommodityInput(1300.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
				CommodityInput(120.0, "chalk dust")
			],
			[
				"TagTool - InRoom - an item with the Fulling Stocks tag",
				"TagTool - Held - an item with the Napping Shears tag",
				"TagTool - InRoom - an item with the Cloth Tenter Frame tag"
			],
			[CommodityOutput(1050.0, "wool", "Broadcloth Stock", colour: true, fineColour: true)]);
		AddStockCraft("prepare embroidered trim stock", "Tailoring", "Tailoring", 25, Difficulty.Normal,
			[
				CommodityInput(260.0, "linen", "Garment Cloth", colour: true, fineColour: true),
				CommodityInput(80.0, "silk", "Spun Yarn", colour: true, fineColour: true)
			],
			[
				"TagTool - Held - an item with the Sewing Needle tag",
				"TagTool - Held - an item with the Embroidery Frame tag"
			],
			[CommodityOutput(220.0, "silk", "Embroidered Trim Stock", colour: true, fineColour: true)]);
		AddStockCraft("weave tablet band stock", "Tailoring", "Tailoring", 20, Difficulty.Normal,
			[
				CommodityInput(260.0, "wool", "Spun Yarn", colour: true, fineColour: true),
				CommodityInput(60.0, "linen", "Spun Yarn", colour: true)
			],
			[
				"TagTool - Held - an item with the Tablet Weaving Cards tag",
				"TagTool - Held - an item with the Sewing Needle tag"
			],
			[CommodityOutput(240.0, "wool", "Tablet-Woven Band Stock", colour: true, fineColour: true)]);
		AddStockCraft("quilt armour padding stock", "Tailoring", "Tailoring", 25, Difficulty.Normal,
			[
				CommodityInput(1200.0, "linen", "Garment Cloth", colour: true, fineColour: true),
				CommodityInput(420.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
				CommodityInput(120.0, "linen", "Spun Yarn", colour: true)
			],
			["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"],
			[CommodityOutput(1350.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true)]);
		AddStockCraft("prepare silk brocade panel stock", "Tailoring", "Tailoring", 35, Difficulty.Hard,
			[
				CommodityInput(900.0, "silk", "Garment Cloth", colour: true, fineColour: true),
				CommodityInput(120.0, "silk", "Spun Yarn", colour: true, fineColour: true)
			],
			[
				"TagTool - InRoom - an item with the Hand Loom tag",
				"TagTool - Held - an item with the Embroidery Frame tag"
			],
			[CommodityOutput(820.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true)]);

		AddStockCraft("cut turnshoe upper stock", "Leathermaking", "Leathermaking", 20, Difficulty.Normal,
			[
				CommodityInput(900.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
				CommodityInput(80.0, "beeswax")
			],
			[
				"TagTool - Held - an item with the Shoe Last tag",
				"TagTool - Held - an item with the Awl Punch tag",
				"TagTool - Held - an item with the Shears tag"
			],
			[CommodityOutput(760.0, "leather", "Turnshoe Upper Stock", colour: true, fineColour: true)]);
		AddStockCraft("prepare scabbard leather stock", "Leathermaking", "Leathermaking", 25, Difficulty.Normal,
			[
				CommodityInput(950.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
				CommodityInput(120.0, "beeswax")
			],
			[
				"TagTool - Held - an item with the Leather Paring Knife tag",
				"TagTool - Held - an item with the Awl Punch tag"
			],
			[CommodityOutput(820.0, "leather", "Scabbard Leather Stock", colour: true, fineColour: true)]);
		AddStockCraft("pare bookbinding leather stock", "Bookbinding", "Leathermaking", 25, Difficulty.Normal,
			[
				CommodityInput(720.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
				CommodityInput(80.0, "beeswax")
			],
			[
				"TagTool - Held - an item with the Leather Paring Knife tag",
				"TagTool - InRoom - an item with the Book Press tag"
			],
			[CommodityOutput(560.0, "leather", "Bookbinding Leather Stock", colour: true, fineColour: true)]);

		AddStockCraft("shape coopered stave stock", "Carpentry", "Carpentry", 20, Difficulty.Normal,
			[CommodityInput(1800.0, "oak", "Furniture Timber Stock")],
			[
				"TagTool - Held - an item with the Croze tag",
				"TagTool - Held - an item with the Planer tag"
			],
			[CommodityOutput(1500.0, "oak", "Coopered Staves")]);
		AddStockCraft("forge cask hoop stock", "Blacksmithing", "Blacksmithing", 20, Difficulty.Normal,
			[CommodityInput(900.0, "wrought iron", "Tool Blank Stock")],
			[
				"TagTool - InRoom - an item with the Anvil tag",
				"TagTool - Held - an item with the Hammer tag",
				"TagTool - Held - an item with the Forge Tongs tag"
			],
			[CommodityOutput(760.0, "wrought iron", "Hoop Stock")]);

		AddStockCraft("draw mail wire stock", "Blacksmithing", "Blacksmithing", 30, Difficulty.Hard,
			[
				CommodityInput(1200.0, "wrought iron", "Tool Blank Stock"),
				CommodityInput(140.0, "beeswax")
			],
			[
				"TagTool - InRoom - an item with the Drawplate tag",
				"TagTool - Held - an item with the Hammer tag",
				"TagTool - Held - an item with the Forge Tongs tag"
			],
			[CommodityOutput(980.0, "wrought iron", "Mail Wire Stock")]);
		AddStockCraft("rivet mail ring stock", "Armourcrafting", "Armourcrafting", 30, Difficulty.Hard,
			[CommodityInput(900.0, "wrought iron", "Mail Wire Stock")],
			[
				"TagTool - Held - an item with the Mail Riveting Tongs tag",
				"TagTool - InRoom - an item with the Armourer's Anvil tag",
				"TagTool - Held - an item with the Planishing Hammer tag"
			],
			[CommodityOutput(760.0, "wrought iron", "Armour Ring Stock")]);
		AddStockCraft("assemble mail panel stock", "Armourcrafting", "Armourcrafting", 35, Difficulty.Hard,
			[CommodityInput(1200.0, "wrought iron", "Armour Ring Stock")],
			[
				"TagTool - Held - an item with the Mail Riveting Tongs tag",
				"TagTool - InRoom - an item with the Armourer's Anvil tag"
			],
			[CommodityOutput(980.0, "wrought iron", "Mail Panel Stock")]);
		AddStockCraft("forge crossbow prod stock", "Bowmaking", "Bowmaking", 30, Difficulty.Hard,
			[
				CommodityInput(1200.0, "wrought iron", "Tool Blank Stock"),
				CommodityInput(120.0, "beeswax")
			],
			[
				"TagTool - InRoom - an item with the Anvil tag",
				"TagTool - Held - an item with the Hammer tag",
				"TagTool - Held - an item with the Forge Tongs tag",
				"TagTool - InRoom - an item with the Bow Press tag"
			],
			[CommodityOutput(980.0, "wrought iron", "Crossbow Prod Stock")]);
		AddStockCraft("carve crossbow tiller stock", "Bowmaking", "Bowmaking", 25, Difficulty.Normal,
			[CommodityInput(1600.0, "oak", "Weapon Shaft Stock")],
			[
				"TagTool - Held - an item with the Tillering Stick tag",
				"TagTool - InRoom - an item with the Crossbow Tiller Jig tag",
				"TagTool - Held - an item with the Awl Punch tag"
			],
			[CommodityOutput(1300.0, "oak", "Crossbow Tiller Stock")]);
		AddStockCraft("fit crossbow nut lockwork stock", "Locksmithing", "Locksmithing", 30, Difficulty.Hard,
			[
				CommodityInput(500.0, "wrought iron", "Tool Blank Stock"),
				CommodityInput(160.0, "bone", "Tool Blank Stock")
			],
			[
				"TagTool - Held - an item with the Locksmith File Set tag",
				"TagTool - InRoom - an item with the Crossbow Tiller Jig tag"
			],
			[CommodityOutput(520.0, "wrought iron", "Crossbow Lockwork Stock")]);

		AddStockCraft("beat rag paper pulp stock", "Papermaking", "Tailoring", 20, Difficulty.Normal,
			[
				CommodityInput(900.0, "linen", "Garment Cloth", colour: true),
				"LiquidUse - 3 litres of Water"
			],
			[
				"TagTool - InRoom - an item with the Papermaking Vat tag",
				"TagTool - Held - an item with the Hammer tag"
			],
			[CommodityOutput(720.0, "Paper", "Paper Pulp Stock", colour: true, fineColour: true)]);
		AddStockCraft("pull paper sheet stock", "Papermaking", "Tailoring", 25, Difficulty.Normal,
			[
				CommodityInput(700.0, "Paper", "Paper Pulp Stock", colour: true, fineColour: true),
				CommodityInput(60.0, "wheat", "Flour Commodity")
			],
			[
				"TagTool - Held - an item with the Mould and Deckle tag",
				"TagTool - InRoom - an item with the Papermaking Vat tag"
			],
			[CommodityOutput(520.0, "Paper", "Paper Sheet Stock", colour: true, fineColour: true)]);
		AddStockCraft("scrape parchment sheet stock", "Writing", "Leathermaking", 20, Difficulty.Normal,
			[
				CommodityInput(1200.0, "parchment"),
				CommodityInput(80.0, "chalk dust")
			],
			[
				"TagTool - Held - an item with the Leather Paring Knife tag",
				"TagTool - Held - an item with the Shears tag"
			],
			[CommodityOutput(900.0, "parchment", "Parchment Sheet Stock", colour: true, fineColour: true)]);
		AddStockCraft("twist seal cord stock", "Ropemaking", "Ropemaking", 15, Difficulty.Easy,
			[
				CommodityInput(360.0, "linen", "Spun Yarn", colour: true, fineColour: true),
				CommodityInput(40.0, "beeswax")
			],
			["TagTool - Held - an item with the Shears tag", "TagTool - Held - an item with the Wax Spatula tag"],
			[CommodityOutput(300.0, "linen", "Seal Cord Stock", colour: true, fineColour: true)]);
		AddStockCraft("compound sealing wax stock", "Candlemaking", "Candlemaking", 15, Difficulty.Easy,
			[
				CommodityInput(420.0, "beeswax"),
				CommodityInput(80.0, "resin"),
				CommodityInput(35.0, "red ochre pigment")
			],
			["TagTool - InRoom - an item with the Fire tag", "TagTool - Held - an item with the Wax Spatula tag"],
			[CommodityOutput(480.0, "beeswax", "Sealing Wax Stock")]);

		AddStockCraft("press cheese curd stock", "Cooking", "Cooking", 15, Difficulty.Easy,
			[
				CommodityInput(1400.0, "milk"),
				CommodityInput(40.0, "salt")
			],
			["TagTool - InRoom - an item with the Cheese Press tag"],
			[CommodityOutput(760.0, "curd", "Cheese Curd Stock")]);
		AddStockCraft("age cheese wheel stock", "Cooking", "Cooking", 20, Difficulty.Normal,
			[
				CommodityInput(700.0, "curd", "Cheese Curd Stock"),
				CommodityInput(30.0, "salt")
			],
			["TagTool - InRoom - an item with the Cheese Press tag"],
			[CommodityOutput(620.0, "cream cheese", "Cheese Wheel Stock")]);
		AddStockCraft("prepare brewing mash stock", "Brewing", "Brewing", 20, Difficulty.Normal,
			[
				CommodityInput(1600.0, "barley"),
				CommodityInput(120.0, "hop"),
				"LiquidUse - 5 litres of Water"
			],
			[
				"TagTool - InRoom - an item with the Lauter Tun tag",
				"TagTool - Held - an item with the Grain Sieve tag"
			],
			[CommodityOutput(1800.0, "barley", "Brewing Mash Stock")]);
		AddStockCraft("rack ale stock", "Brewing", "Brewing", 20, Difficulty.Normal,
			[CommodityInput(1600.0, "barley", "Brewing Mash Stock")],
			["TagTool - InRoom - an item with the Lauter Tun tag"],
			[CommodityOutput(1300.0, "barley", "Ale Stock")]);
		AddStockCraft("rack cider stock", "Brewing", "Brewing", 15, Difficulty.Easy,
			[
				CommodityInput(1500.0, "apple"),
				CommodityInput(220.0, "honey")
			],
			["TagTool - InRoom - an item with the Lauter Tun tag"],
			[CommodityOutput(1200.0, "apple", "Cider Stock")]);
		AddStockCraft("rack mead stock", "Brewing", "Brewing", 20, Difficulty.Normal,
			[
				CommodityInput(1200.0, "honey"),
				"LiquidUse - 4 litres of Water"
			],
			["TagTool - InRoom - an item with the Lauter Tun tag"],
			[CommodityOutput(1200.0, "honey", "Mead Stock")]);

		AddStockCraft("mix glaze slurry stock", "Pottery", "Pottery", 20, Difficulty.Normal,
			[
				CommodityInput(700.0, "slaked lime"),
				CommodityInput(260.0, "lead white pigment"),
				"LiquidUse - 2 litres of Water"
			],
			["TagTool - Held - an item with the Glazing Basin tag"],
			[CommodityOutput(900.0, "slaked lime", "Glaze Slurry Stock")]);
		AddStockCraft("press roof tile blank stock", "Pottery", "Pottery", 15, Difficulty.Easy,
			[
				CommodityInput(2000.0, "clay"),
				"LiquidUse - 1 litre of Water"
			],
			["TagTool - Held - an item with the Tile Mould tag"],
			[CommodityOutput(1800.0, "earthenware", "Tile Blank Stock")]);
		AddStockCraft("cut stained glass quarry stock", "Glassworking", "Glassworking", 25, Difficulty.Normal,
			[
				CommodityInput(900.0, "glass", "Glass Vessel Blank", colour: true, fineColour: true),
				CommodityInput(80.0, "lead")
			],
			["TagTool - Held - an item with the Grozing Iron tag"],
			[CommodityOutput(720.0, "glass", "Stained Glass Quarry Stock", colour: true, fineColour: true)]);
		AddStockCraft("cast lead came stock", "Blacksmithing", "Blacksmithing", 15, Difficulty.Normal,
			[CommodityInput(900.0, "lead")],
			[
				"TagTool - InRoom - an item with the Hot Fire tag",
				"TagTool - Held - an item with the Lead Knife tag"
			],
			[CommodityOutput(760.0, "lead", "Lead Came Stock")]);
		AddStockCraft("lead stained glass panel stock", "Glassworking", "Glassworking", 30, Difficulty.Hard,
			[
				CommodityInput(620.0, "glass", "Stained Glass Quarry Stock", colour: true, fineColour: true),
				CommodityInput(420.0, "lead", "Lead Came Stock")
			],
			[
				"TagTool - Held - an item with the Grozing Iron tag",
				"TagTool - Held - an item with the Lead Knife tag"
			],
			[CommodityOutput(980.0, "glass", "Stained Glass Panel Stock", colour: true, fineColour: true)]);
		AddStockCraft("cast lantern pane stock", "Glassworking", "Glassworking", 25, Difficulty.Normal,
			[
				CommodityInput(620.0, "glass", "Glass Batch", colour: true, fineColour: true),
				CommodityInput(80.0, "lead")
			],
			[
				"TagTool - InRoom - an item with the Hot Fire tag",
				"TagTool - Held - an item with the Lantern Pane Mould tag"
			],
			[CommodityOutput(520.0, "glass", "Lantern Pane Stock", colour: true, fineColour: true)]);

		AddStockCraft("cast standard weight blank stock", "Blacksmithing", "Blacksmithing", 25, Difficulty.Normal,
			[CommodityInput(1800.0, "bronze", "Tool Blank Stock")],
			[
				"TagTool - InRoom - an item with the Anvil tag",
				"TagTool - Held - an item with the Forge Tongs tag"
			],
			[CommodityOutput(1600.0, "bronze", "Standard Weight Blank")]);
		AddStockCraft("wrap sealable bale wrapper stock", "Tailoring", "Tailoring", 15, Difficulty.Easy,
			[
				CommodityInput(1500.0, "linen", "Broadcloth Stock", colour: true, fineColour: true),
				CommodityInput(100.0, "hemp", "Tool Blank Stock"),
				CommodityInput(80.0, "beeswax")
			],
			["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"],
			[CommodityOutput(1500.0, "linen", "Sealable Bale Wrapper Stock", colour: true, fineColour: true)]);
		AddStockCraft("split tally stick stock", "Carpentry", "Carpentry", 15, Difficulty.Easy,
			[CommodityInput(600.0, "willow", "Tool Blank Stock")],
			["TagTool - Held - an item with the Awl Punch tag"],
			[CommodityOutput(480.0, "willow", "Tally Stick Stock")]);
		AddStockCraft("fit warded lockwork stock", "Locksmithing", "Locksmithing", 30, Difficulty.Hard,
			[
				CommodityInput(950.0, "wrought iron", "Tool Blank Stock"),
				CommodityInput(120.0, "beeswax")
			],
			[
				"TagTool - InRoom - an item with the Anvil tag",
				"TagTool - Held - an item with the Locksmith File Set tag"
			],
			[CommodityOutput(820.0, "wrought iron", "Lockwork Stock")]);
	}

	private (string Category, string Trait, IReadOnlyList<string> Inputs, IReadOnlyList<string> Tools, Difficulty Difficulty)
		GetHistoricFoundationCraftPath(MedievalItemSpec spec)
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

	private (string Category, string Trait, IReadOnlyList<string> Inputs, IReadOnlyList<string> Tools, Difficulty Difficulty,
		string Verb, string Gerund) GetMedievalGeneralCraftPath(MedievalItemSpec spec)
	{
		var components = spec.Components;
		var inputs = new List<string>();
		if (components.Any(x => x.Contains("PaperSheet", StringComparison.OrdinalIgnoreCase) ||
		                        x.Contains("Book_", StringComparison.OrdinalIgnoreCase) ||
		                        x.Contains("Destroyable_Paper", StringComparison.OrdinalIgnoreCase)))
		{
			var paperStock = spec.StableReference.Contains("paper", StringComparison.OrdinalIgnoreCase)
				? ("Paper", "Paper Sheet Stock")
				: ("parchment", "Parchment Sheet Stock");
			inputs.Add(CommodityInput(Math.Max(80.0, spec.WeightInGrams * 0.60), paperStock.Item1, paperStock.Item2));
			if (components.Any(x => x.Contains("Sealable", StringComparison.OrdinalIgnoreCase)))
			{
				inputs.Add(CommodityInput(40.0, "beeswax", "Sealing Wax Stock"));
				inputs.Add(CommodityInput(30.0, "linen", "Seal Cord Stock", colour: true, fineColour: true));
			}

			if (components.Any(x => x.Contains("Book_", StringComparison.OrdinalIgnoreCase)))
			{
				inputs.Add(CommodityInput(180.0, "leather", "Bookbinding Leather Stock", colour: true, fineColour: true));
			}

			return ("Writing", "Tailoring", inputs,
				components.Any(x => x.Contains("Book_", StringComparison.OrdinalIgnoreCase))
					? ["TagTool - Held - an item with the Sewing Needle tag", "TagTool - InRoom - an item with the Book Press tag"]
					: ["TagTool - Held - an item with the Sewing Needle tag"],
				components.Any(x => x.Contains("Book_", StringComparison.OrdinalIgnoreCase)) ? Difficulty.Hard : Difficulty.Normal,
				"prepare", "preparing");
		}

		if (components.Any(x => x.Contains("SealStamp", StringComparison.OrdinalIgnoreCase)))
		{
			inputs.Add(CommodityInput(Math.Max(120.0, spec.WeightInGrams * 0.75), spec.Material, "Tool Blank Stock"));
			return ("Silversmithing", "Silversmithing", inputs,
				["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"],
				Difficulty.Hard, "engrave", "engraving");
		}

		if (components.Any(x => x.Contains("Lantern", StringComparison.OrdinalIgnoreCase)))
		{
			inputs.Add(CommodityInput(Math.Max(500.0, spec.WeightInGrams * 0.60), spec.Material, "Tool Blank Stock"));
			inputs.Add(CommodityInput(60.0, "linen", "Spun Yarn", colour: true));
			inputs.Add(CommodityInput(220.0, "glass", "Lantern Pane Stock", colour: true, fineColour: true));
			return ("Blacksmithing", "Blacksmithing", inputs,
				["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"],
				Difficulty.Normal, "make", "making");
		}

		switch (spec.MaterialType)
		{
			case MaterialBehaviourType.Ceramic:
				return ("Pottery", "Pottery",
					[CommodityInput(Math.Max(250.0, spec.WeightInGrams * 0.80), spec.Material, "Pottery Clay Body")],
					["TagTool - InRoom - an item with the Hot Fire tag"],
					Difficulty.Normal, "fire", "firing");
			case MaterialBehaviourType.Wood:
				return ("Carpentry", "Carpentry",
					[CommodityInput(Math.Max(350.0, spec.WeightInGrams * 0.50), spec.Material, "Furniture Panel Stock")],
					["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"],
					Difficulty.Normal, "build", "building");
			case MaterialBehaviourType.Metal:
				return ("Blacksmithing", "Blacksmithing",
					[CommodityInput(Math.Max(120.0, spec.WeightInGrams * 0.70), spec.Material, "Tool Blank Stock")],
					["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"],
					spec.Quality >= ItemQuality.Good ? Difficulty.Hard : Difficulty.Normal, "make", "making");
			case MaterialBehaviourType.Leather:
				inputs.Add(CommodityInput(Math.Max(220.0, spec.WeightInGrams * 0.60), spec.Material, "Prepared Leather Panel", colour: true, fineColour: spec.Quality >= ItemQuality.Good));
				if (components.Any(x => x.Contains("Sealable", StringComparison.OrdinalIgnoreCase)))
				{
					inputs.Add(CommodityInput(40.0, "beeswax", "Sealing Wax Stock"));
					inputs.Add(CommodityInput(30.0, "linen", "Seal Cord Stock", colour: true, fineColour: true));
				}

				return ("Leathermaking", "Leathermaking", inputs,
					["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Shears tag"],
					Difficulty.Normal, "sew", "sewing");
			case MaterialBehaviourType.Fabric:
				inputs.Add(CommodityInput(Math.Max(180.0, spec.WeightInGrams * 0.55), spec.Material, "Garment Cloth", colour: true, fineColour: spec.Quality >= ItemQuality.Good));
				inputs.Add(CommodityInput(50.0, spec.Material, "Spun Yarn", colour: true));
				if (components.Any(x => x.Contains("Sealable", StringComparison.OrdinalIgnoreCase)))
				{
					inputs.Add(CommodityInput(40.0, "beeswax", "Sealing Wax Stock"));
					inputs.Add(CommodityInput(30.0, "linen", "Seal Cord Stock", colour: true, fineColour: true));
				}

				return ("Tailoring", "Tailoring", inputs,
					["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"],
					Difficulty.Normal, "sew", "sewing");
			case MaterialBehaviourType.Bone:
				return ("Bonecarving", "Bonecarving",
					[CommodityInput(Math.Max(80.0, spec.WeightInGrams * 0.80), spec.Material, "Tool Blank Stock")],
					["TagTool - Held - an item with the Awl Punch tag"],
					Difficulty.Normal, "carve", "carving");
			case MaterialBehaviourType.Wax:
				return ("Candlemaking", "Candlemaking",
					[CommodityInput(Math.Max(50.0, spec.WeightInGrams), "beeswax")],
					[],
					Difficulty.Easy, "make", "making");
			default:
				return ("Crafting", "Crafting",
					[CommodityInput(Math.Max(120.0, spec.WeightInGrams * 0.50), spec.Material, "Tool Blank Stock")],
					["TagTool - Held - an item with the Hammer tag"],
					Difficulty.Normal, "make", "making");
		}
	}

	private void AddMedievalGeneralSpecCraft(MedievalItemSpec spec, string craftName, string knowledgeSubtype,
		string? knowledgeName = null, int? minimumTraitValue = null)
	{
		var path = GetMedievalGeneralCraftPath(spec);
		AddMedievalFinishedCraft(
			spec.StableReference,
			craftName,
			path.Category,
			path.Trait,
			knowledgeName ?? MedievalWorkshopKnowledge,
			minimumTraitValue ?? (path.Difficulty == Difficulty.Hard ? 30 : 15),
			path.Difficulty,
			path.Inputs,
			path.Tools,
			path.Verb,
			path.Gerund,
			knowledgeSubtype);
	}

	private static string MedievalSpecCraftName(string verb, MedievalItemSpec spec)
	{
		return $"{verb} {VisibleCraftName(spec.ShortDescription)} [{spec.StableReference}]";
	}

	private static string MedievalExplicitOutfitPieceCraftName(string verb, MedievalItemSpec spec)
	{
		return $"{verb} {spec.ShortDescription} [{spec.StableReference}]";
	}

	private static (string Category, string Trait, IReadOnlyList<string> Inputs, IReadOnlyList<string> Tools, Difficulty Difficulty, string Verb, string Gerund)
		GetMedievalExplicitOutfitPieceCraftPath(MedievalItemSpec spec)
	{
		var visibleName = VisibleCraftName(spec.ShortDescription);
		var inputs = new List<string>();
		switch (spec.MaterialType)
		{
			case MaterialBehaviourType.Leather:
				inputs.Add(CommodityInput(
					Math.Max(160.0, spec.WeightInGrams * 0.65),
					spec.Material,
					visibleName.Contains("shoe", StringComparison.OrdinalIgnoreCase) ||
					visibleName.Contains("boot", StringComparison.OrdinalIgnoreCase) ||
					visibleName.Contains("sandal", StringComparison.OrdinalIgnoreCase) ||
					visibleName.Contains("slipper", StringComparison.OrdinalIgnoreCase)
						? "Turnshoe Upper Stock"
						: "Prepared Leather Panel",
					colour: true,
					fineColour: spec.Quality >= ItemQuality.Good));
				inputs.Add(CommodityInput(60.0, "linen", "Spun Yarn", colour: true));
				return ("Leathermaking", "Leathermaking", inputs,
					[
						"TagTool - Held - an item with the Awl Punch tag",
						"TagTool - Held - an item with the Sewing Needle tag",
						"TagTool - Held - an item with the Shears tag"
					],
					Difficulty.Normal, "sew", "sewing");
			case MaterialBehaviourType.Fabric:
				if (spec.Material.Equals("paper", StringComparison.OrdinalIgnoreCase))
				{
					var isBoundPaperItem =
						!visibleName.Contains("book pouch", StringComparison.OrdinalIgnoreCase) &&
						(visibleName.Contains("notebook", StringComparison.OrdinalIgnoreCase) ||
						 visibleName.Contains("booklet", StringComparison.OrdinalIgnoreCase) ||
						 visibleName.Contains("book", StringComparison.OrdinalIgnoreCase));
					inputs.Add(CommodityInput(Math.Max(60.0, spec.WeightInGrams * 0.70), "paper", "Paper Sheet Stock",
						colour: true, fineColour: spec.Quality >= ItemQuality.Good));
					if (isBoundPaperItem)
					{
						inputs.Add(CommodityInput(70.0, "leather", "Bookbinding Leather Stock", colour: true,
							fineColour: true));
					}

					return ("Writing", "Tailoring", inputs,
						isBoundPaperItem
							? ["TagTool - Held - an item with the Sewing Needle tag", "TagTool - InRoom - an item with the Book Press tag"]
							: ["TagTool - Held - an item with the Sewing Needle tag"],
						Difficulty.Normal, "prepare", "preparing");
				}

				var clothStock =
					spec.Material.Equals("silk", StringComparison.OrdinalIgnoreCase) &&
					(spec.Quality >= ItemQuality.Good || visibleName.Contains("silk", StringComparison.OrdinalIgnoreCase))
						? "Silk Brocade Panel"
						: visibleName.Contains("padded", StringComparison.OrdinalIgnoreCase) ||
					visibleName.Contains("arming", StringComparison.OrdinalIgnoreCase) ||
					visibleName.Contains("gambeson", StringComparison.OrdinalIgnoreCase) ||
					visibleName.Contains("aketon", StringComparison.OrdinalIgnoreCase) ||
					visibleName.Contains("shield-wall", StringComparison.OrdinalIgnoreCase)
						? "Quilted Armour Padding"
						: spec.Material.Equals("felt", StringComparison.OrdinalIgnoreCase)
							? "Fulled Cloth"
							: visibleName.Contains("fine", StringComparison.OrdinalIgnoreCase) ||
						  visibleName.Contains("lined", StringComparison.OrdinalIgnoreCase) ||
						  visibleName.Contains("noble", StringComparison.OrdinalIgnoreCase) ||
						  visibleName.Contains("merchant", StringComparison.OrdinalIgnoreCase) ||
						  spec.Quality >= ItemQuality.Good
							? "Broadcloth Stock"
							: "Garment Cloth";
				inputs.Add(CommodityInput(Math.Max(180.0, spec.WeightInGrams * 0.65), spec.Material, clothStock,
					colour: true, fineColour: spec.Quality >= ItemQuality.Good));
				inputs.Add(CommodityInput(55.0,
					spec.Material.Equals("felt", StringComparison.OrdinalIgnoreCase) ? "wool" : spec.Material,
					"Spun Yarn", colour: true));
				if (visibleName.Contains("lamellar", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("scale panels", StringComparison.OrdinalIgnoreCase))
				{
					inputs.Add(CommodityInput(420.0, "wrought iron", "Armour Lamella Stock"));
				}

				if (visibleName.Contains("fur-edged", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("fur-lined", StringComparison.OrdinalIgnoreCase))
				{
					inputs.Add(CommodityInput(120.0, "fur", "Fur Panel Stock", colour: true, fineColour: true));
				}

				if (visibleName.Contains("tablet-banded", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("tablet-woven", StringComparison.OrdinalIgnoreCase) ||
				    spec.StableReference.Contains("tablet_banded", StringComparison.OrdinalIgnoreCase) ||
				    spec.StableReference.Contains("tablet_woven", StringComparison.OrdinalIgnoreCase))
				{
					inputs.Add(CommodityInput(35.0, "wool", "Tablet-Woven Band Stock", colour: true, fineColour: true));
				}

				if (visibleName.Contains("embroidered", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("bordered", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("panelled", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("braid", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("bliaut", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("tiraz", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("surcoat", StringComparison.OrdinalIgnoreCase) ||
				    visibleName.Contains("hangerok", StringComparison.OrdinalIgnoreCase))
				{
					inputs.Add(CommodityInput(45.0, spec.Material.Equals("silk", StringComparison.OrdinalIgnoreCase) ? "silk" : "linen",
						"Embroidered Trim Stock", colour: true, fineColour: true));
				}

				return ("Tailoring", "Tailoring", inputs,
					[
						"TagTool - Held - an item with the Sewing Needle tag",
						"TagTool - Held - an item with the Shears tag"
					],
					Difficulty.Normal, "sew", "sewing");
			case MaterialBehaviourType.Hair:
				inputs.Add(CommodityInput(Math.Max(90.0, spec.WeightInGrams * 0.55), spec.Material, "Fur Panel Stock",
					colour: true, fineColour: spec.Quality >= ItemQuality.Good));
				inputs.Add(CommodityInput(35.0, "linen", "Spun Yarn", colour: true));
				return ("Tailoring", "Tailoring", inputs,
					[
						"TagTool - Held - an item with the Sewing Needle tag",
						"TagTool - Held - an item with the Shears tag"
					],
					Difficulty.Normal, "sew", "sewing");
			case MaterialBehaviourType.Wood:
				return ("Carpentry", "Carpentry",
					[CommodityInput(Math.Max(80.0, spec.WeightInGrams * 0.70), spec.Material, "Tool Blank Stock")],
					["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"],
					Difficulty.Normal, "make", "making");
			case MaterialBehaviourType.Metal:
				return ("Metalworking", "Blacksmithing",
					[CommodityInput(Math.Max(80.0, spec.WeightInGrams * 0.70), spec.Material, "Tool Blank Stock")],
					["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"],
					spec.Quality >= ItemQuality.Good ? Difficulty.Hard : Difficulty.Normal, "make", "making");
			case MaterialBehaviourType.Wax:
				return ("Candlemaking", "Candlemaking",
					[CommodityInput(Math.Max(50.0, spec.WeightInGrams), spec.Material)],
					[],
					Difficulty.Easy, "make", "making");
			default:
				return ("Crafting", "Crafting",
					[CommodityInput(Math.Max(80.0, spec.WeightInGrams * 0.70), spec.Material, "Tool Blank Stock")],
					["TagTool - Held - an item with the Hammer tag"],
					Difficulty.Normal, "make", "making");
		}
	}

	private static (string Category, string Trait, IReadOnlyList<string> Inputs, IReadOnlyList<string> Tools, Difficulty Difficulty, string Verb, string Gerund)
		ApplyMedievalExplicitOutfitPieceCraftSpec(
			MedievalItemSpec spec,
			(string Category, string Trait, IReadOnlyList<string> Inputs, IReadOnlyList<string> Tools, Difficulty Difficulty, string Verb, string Gerund) path)
	{
		if (!MedievalAuthoredOutfitPieceCraftSpecs.TryGetValue(spec.StableReference, out var craftSpec))
		{
			return path;
		}

		return (path.Category, path.Trait, craftSpec.Inputs, craftSpec.Tools, path.Difficulty, path.Verb,
			path.Gerund);
	}

	private static string MedievalExplicitOutfitPieceProduct(
		GameItemProto item,
		MedievalItemSpec spec,
		IReadOnlyList<string> inputs)
	{
		if (!MedievalAuthoredOutfitPieces.TryGetValue(spec.StableReference, out var authoredSpec) ||
		    string.IsNullOrWhiteSpace(authoredSpec.VariableColourComponent))
		{
			return $"SimpleProduct - 1x {item.ShortDescription} (#{item.Id})";
		}

		var mappings = BuildMedievalExplicitOutfitPieceProductVariableMappings(authoredSpec, inputs);
		return mappings.Count == 0
			? $"SimpleProduct - 1x {item.ShortDescription} (#{item.Id})"
			: $"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable {string.Join(", ", mappings)}";
	}

	private static IReadOnlyList<string> BuildMedievalExplicitOutfitPieceProductVariableMappings(
		MedievalAuthoredOutfitPieceSpec authoredSpec,
		IReadOnlyList<string> inputs)
	{
		if (string.IsNullOrWhiteSpace(authoredSpec.VariableColourComponent))
		{
			return [];
		}

		var primaryIndex = FirstMedievalExplicitOutfitPieceColourInputIndex(inputs) ??
		                   (inputs.Count > 0 ? 1 : null);
		if (primaryIndex is null)
		{
			return [];
		}

		return authoredSpec.VariableColourComponent switch
		{
			"Variable_Colour" => [$"Colour=$i{primaryIndex.Value}"],
			"Variable_DrabColour" => [$"Colour=$i{primaryIndex.Value}", $"Drab Colour=$i{primaryIndex.Value}"],
			"Variable_FineColour" => [$"Colour=$i{primaryIndex.Value}", $"Fine Colour=$i{primaryIndex.Value}"],
			"Variable_2Colour" => BuildTwoColourMedievalExplicitOutfitPieceProductVariableMappings(
				inputs,
				primaryIndex.Value),
			"Variable_2DrabColour" => BuildTwoColourMedievalExplicitOutfitPieceProductVariableMappings(
				inputs,
				primaryIndex.Value),
			"Variable_2FineColour" => BuildTwoColourMedievalExplicitOutfitPieceProductVariableMappings(
				inputs,
				primaryIndex.Value),
			_ => []
		};
	}

	private static IReadOnlyList<string> BuildTwoColourMedievalExplicitOutfitPieceProductVariableMappings(
		IReadOnlyList<string> inputs,
		int primaryIndex)
	{
		var secondaryIndex = SecondaryMedievalExplicitOutfitPieceColourInputIndex(inputs, primaryIndex) ??
		                     primaryIndex;
		return [$"Colour1=$i{primaryIndex}", $"Colour2=$i{secondaryIndex}"];
	}

	private static int? FirstMedievalExplicitOutfitPieceColourInputIndex(IReadOnlyList<string> inputs)
	{
		for (var index = 0; index < inputs.Count; index++)
		{
			if (MedievalExplicitOutfitPieceInputCarriesColour(inputs[index]))
			{
				return index + 1;
			}
		}

		return null;
	}

	private static int? SecondaryMedievalExplicitOutfitPieceColourInputIndex(
		IReadOnlyList<string> inputs,
		int primaryIndex)
	{
		var decoratedInputIndex = inputs
			.Select((input, index) => (Input: input, Index: index + 1))
			.Where(x => x.Index != primaryIndex)
			.LastOrDefault(x => MedievalExplicitOutfitPieceInputIsDecorativeColourSource(x.Input))
			.Index;
		if (decoratedInputIndex > 0)
		{
			return decoratedInputIndex;
		}

		var colourInputIndex = inputs
			.Select((input, index) => (Input: input, Index: index + 1))
			.Where(x => x.Index != primaryIndex)
			.Where(x => !x.Input.Contains("Spun Yarn", StringComparison.OrdinalIgnoreCase))
			.LastOrDefault(x => MedievalExplicitOutfitPieceInputCarriesColour(x.Input))
			.Index;
		if (colourInputIndex > 0)
		{
			return colourInputIndex;
		}

		return inputs
			.Select((input, index) => (Input: input, Index: index + 1))
			.Where(x => x.Index != primaryIndex)
			.LastOrDefault(x => MedievalExplicitOutfitPieceInputCarriesColour(x.Input))
			.Index;
	}

	private static bool MedievalExplicitOutfitPieceInputCarriesColour(string input)
	{
		return ContainsAny(input,
			"characteristic Colour any",
			"characteristic Fine Colour any",
			"characteristic Drab Colour any",
			"characteristic Colour1 any",
			"characteristic Colour2 any");
	}

	private static bool MedievalExplicitOutfitPieceInputIsDecorativeColourSource(string input)
	{
		return ContainsAny(input,
			"Band Stock",
			"Trim Stock",
			"Brocade",
			"Fur Panel Stock",
			"Seal Cord Stock");
	}

	internal static IReadOnlyCollection<(string StableReference, string CraftName, IReadOnlyCollection<string> Inputs, IReadOnlyCollection<string> Tools)> MedievalExplicitOutfitPieceCraftsForTesting =>
		MedievalExplicitOutfitPieceItemSpecs()
			.Select(spec =>
			{
				var path = ApplyMedievalExplicitOutfitPieceCraftSpec(spec,
					GetMedievalExplicitOutfitPieceCraftPath(spec));
				return (spec.StableReference, MedievalExplicitOutfitPieceCraftName(path.Verb, spec),
					(IReadOnlyCollection<string>)path.Inputs.ToArray(),
					(IReadOnlyCollection<string>)path.Tools.ToArray());
			})
			.ToArray();

	internal static IReadOnlyCollection<(string StableReference, string VariableColourComponent, IReadOnlyCollection<string> ProductVariableMappings)> MedievalExplicitOutfitPieceProductVariableMappingsForTesting =>
		MedievalAuthoredOutfitPieces.Values
			.Where(x => !string.IsNullOrWhiteSpace(x.VariableColourComponent))
			.Select(x => (x.StableReference, x.VariableColourComponent!,
				(IReadOnlyCollection<string>)BuildMedievalExplicitOutfitPieceProductVariableMappings(
					x,
					x.CraftInputs).ToArray()))
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalCraftedItemStableReferencesForTesting =>
		MedievalAllItemSpecs()
			.Where(x => string.IsNullOrWhiteSpace(x.MorphToUniqueReference))
			.Select(x => x.StableReference)
			.ToArray();

	private void SeedMedievalClothingCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		var clothingSpecs = MedievalClothingItemSpecs();
		for (var cultureIndex = 0; cultureIndex < MedievalCultureProfiles.Length; cultureIndex++)
		{
			var culture = MedievalCultureProfiles[cultureIndex];
			foreach (var status in MedievalStatusRoleProfiles)
			{
				var stablePrefix = $"medieval_clothing_{culture.Key}_{status.Key}_";
				foreach (var item in clothingSpecs.Where(x => x.StableReference.StartsWith(stablePrefix, StringComparison.OrdinalIgnoreCase)))
				{
					var subject = item.StableReference[stablePrefix.Length..].Replace('_', ' ');
					var clothAmount = Math.Max(90.0, item.WeightInGrams * 0.75);
					var yarnAmount = Math.Max(25.0, item.WeightInGrams * 0.10);
					var inputs = new List<string>();
					if (item.MaterialType == MaterialBehaviourType.Leather)
					{
						var leatherStock = subject.Contains("shoe", StringComparison.OrdinalIgnoreCase) ||
						                   subject.Contains("boot", StringComparison.OrdinalIgnoreCase) ||
						                   subject.Contains("sandal", StringComparison.OrdinalIgnoreCase) ||
						                   subject.Contains("turnshoe", StringComparison.OrdinalIgnoreCase)
							? "Turnshoe Upper Stock"
							: "Prepared Leather Panel";
						inputs.Add(CommodityInput(clothAmount, item.Material, leatherStock, colour: true,
							fineColour: item.Quality >= ItemQuality.Good));
						inputs.Add(CommodityInput(yarnAmount, "linen", "Spun Yarn", colour: true));
					}
					else
					{
						var clothStock =
							status.Key.Equals("noble", StringComparison.OrdinalIgnoreCase) && item.Material.Equals("silk", StringComparison.OrdinalIgnoreCase)
								? "Silk Brocade Panel"
								: status.Key.Equals("military", StringComparison.OrdinalIgnoreCase) &&
								  (subject.Contains("padded", StringComparison.OrdinalIgnoreCase) ||
								   subject.Contains("arming", StringComparison.OrdinalIgnoreCase))
									? "Quilted Armour Padding"
									: status.Key.Equals("merchant", StringComparison.OrdinalIgnoreCase) ||
									  status.Key.Equals("clergy", StringComparison.OrdinalIgnoreCase)
										? "Broadcloth Stock"
										: "Garment Cloth";
						inputs.Add(CommodityInput(clothAmount, item.Material, clothStock, colour: true,
							fineColour: item.Quality >= ItemQuality.Good));
						inputs.Add(CommodityInput(yarnAmount,
							item.MaterialType == MaterialBehaviourType.Fabric ? item.Material : "linen", "Spun Yarn",
							colour: true));

						if (status.Key.Equals("noble", StringComparison.OrdinalIgnoreCase) ||
						    status.Key.Equals("merchant", StringComparison.OrdinalIgnoreCase))
						{
							inputs.Add(CommodityInput(45.0, item.Material.Equals("silk", StringComparison.OrdinalIgnoreCase) ? "silk" : "linen",
								"Embroidered Trim Stock", colour: true, fineColour: true));
						}

						if (culture.ClothingCue.Contains("tablet", StringComparison.OrdinalIgnoreCase) ||
						    culture.ClothingCue.Contains("band", StringComparison.OrdinalIgnoreCase) ||
						    culture.ClothingCue.Contains("edging", StringComparison.OrdinalIgnoreCase))
						{
							inputs.Add(CommodityInput(35.0, "wool", "Tablet-Woven Band Stock", colour: true,
								fineColour: true));
						}
					}
					var tools = item.MaterialType == MaterialBehaviourType.Leather
						? new[]
						{
							"TagTool - Held - an item with the Awl Punch tag",
							"TagTool - Held - an item with the Sewing Needle tag",
							"TagTool - Held - an item with the Shears tag"
						}
						: new[]
						{
							"TagTool - Held - an item with the Sewing Needle tag",
							"TagTool - Held - an item with the Shears tag"
						};

					AddMedievalCraft(
						BuildRegionalCraftName("sew", $"{status.Key} {subject}", cultureIndex + 1),
						"Tailoring",
						$"sew a {status.Display.ToLowerInvariant()} wardrobe piece",
						$"sewing a {status.Display.ToLowerInvariant()} wardrobe piece",
						"a wardrobe piece being assembled",
						$"{MedievalClothingKnowledgePrefix} {culture.Display}",
						"Tailoring",
						status.Key.Equals("noble", StringComparison.OrdinalIgnoreCase) ? 35 : 20,
						status.Key.Equals("noble", StringComparison.OrdinalIgnoreCase) ? Difficulty.Hard : Difficulty.Normal,
						MedievalFinishedPhases(),
						inputs,
						tools,
						[StableSimpleProduct(item.StableReference)],
						knowledgeSubtype: status.Display);
				}
			}
		}

		foreach (var item in MedievalExplicitOutfitPieceItemSpecs())
		{
			var path = ApplyMedievalExplicitOutfitPieceCraftSpec(item,
				GetMedievalExplicitOutfitPieceCraftPath(item));
			AddMedievalFinishedCraft(
				item.StableReference,
				MedievalExplicitOutfitPieceCraftName(path.Verb, item),
				path.Category,
				path.Trait,
				$"{MedievalClothingKnowledgePrefix} Explicit Outfits",
				path.Difficulty == Difficulty.Hard ? 35 : 20,
				path.Difficulty,
				path.Inputs,
				path.Tools,
				path.Verb,
				path.Gerund,
				"Explicit Outfits",
				dbItem => new[] { MedievalExplicitOutfitPieceProduct(dbItem, item, path.Inputs) });
		}
	}

	private void SeedMedievalEquipmentCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		for (var index = 0; index < MedievalCultureProfiles.Length; index++)
		{
			var culture = MedievalCultureProfiles[index];
			var armourStable = $"medieval_military_{culture.Key}_armour";
			AddMedievalCraft(
				BuildRegionalCraftName("assemble", "military armour", index + 1),
				"Armourcrafting",
				"assemble a regional armour pattern",
				"assembling a regional armour pattern",
				"armour being assembled",
				$"{MedievalArmourKnowledgePrefix} {culture.Display}",
				"Armourcrafting",
				35,
				Difficulty.Hard,
				MedievalFinishedPhases(),
				[
					CommodityInput(3600.0, "wrought iron", culture.ArmourDescription.Contains("lamellar", StringComparison.OrdinalIgnoreCase) ? "Armour Lamella Stock" : "Mail Panel Stock"),
					CommodityInput(1200.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CommodityInput(300.0, "leather", "Leather Strap", colour: true)
				],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - Held - an item with the Forge Tongs tag"
				],
				[StableSimpleProduct(armourStable)],
				knowledgeSubtype: "Armour");

			var weaponStable = $"medieval_weapon_{culture.Key}_{StableReferenceToken(culture.WeaponNoun)}";
			var weaponStock = culture.WeaponComponent.Contains("Spear", StringComparison.OrdinalIgnoreCase) ||
			                  culture.WeaponComponent.Contains("Axe", StringComparison.OrdinalIgnoreCase) ||
			                  culture.WeaponComponent.Contains("Mace", StringComparison.OrdinalIgnoreCase) ||
			                  culture.WeaponComponent.Contains("Warhammer", StringComparison.OrdinalIgnoreCase)
				? "Weapon Head Stock"
				: "Weapon Blade Stock";
			AddMedievalCraft(
				BuildRegionalCraftName("forge", "military weapon", index + 1),
				"Weaponcrafting",
				"forge a regional weapon pattern",
				"forging a regional weapon pattern",
				"a weapon being forged",
				$"{MedievalWeaponKnowledgePrefix} {culture.Display}",
				"Weaponcrafting",
				30,
				Difficulty.Hard,
				MedievalFinishedPhases(),
				[
					CommodityInput(1400.0, "wrought iron", weaponStock),
					CommodityInput(700.0, "oak", "Weapon Shaft Stock"),
					CommodityInput(120.0, "leather", "Leather Strap", colour: true)
				],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - Held - an item with the Forge Tongs tag"
				],
				[StableSimpleProduct(weaponStable)],
				knowledgeSubtype: "Weapons");

			AddMedievalCraft(
				BuildRegionalCraftName("build", "military shield", index + 1),
				"Armourcrafting",
				"build a regional shield pattern",
				"building a regional shield pattern",
				"a shield being built",
				$"{MedievalShieldKnowledgePrefix} {culture.Display}",
				"Armourcrafting",
				25,
				Difficulty.Normal,
				MedievalFinishedPhases(),
				[
					CommodityInput(2600.0, "oak", "Shield Board Stock"),
					CommodityInput(650.0, "leather", "Shield Facing Stock", colour: true),
					CommodityInput(250.0, "wrought iron", "Tool Blank Stock")
				],
				[
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - Held - an item with the Awl Punch tag"
				],
				[StableSimpleProduct($"medieval_shield_{culture.Key}")],
				knowledgeSubtype: "Shields");

			AddMedievalFinishedCraft($"medieval_military_{culture.Key}_helmet",
				BuildRegionalCraftName("forge", "military helmet", index + 1), "Armourcrafting",
				"Armourcrafting", $"{MedievalArmourKnowledgePrefix} {culture.Display}", 30, Difficulty.Hard,
				[
					CommodityInput(1400.0, "wrought iron", "Tool Blank Stock"),
					CommodityInput(120.0, "leather", "Leather Strap", colour: true)
				],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - Held - an item with the Forge Tongs tag"
				],
				"forge", "forging", "Armour Accessories");
			AddMedievalFinishedCraft($"medieval_military_{culture.Key}_padded_coif",
				BuildRegionalCraftName("quilt", "padded coif", index + 1), "Tailoring",
				"Tailoring", $"{MedievalArmourKnowledgePrefix} {culture.Display}", 20, Difficulty.Normal,
				[
					CommodityInput(540.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CommodityInput(80.0, "linen", "Spun Yarn", colour: true)
				],
				["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"],
				"quilt", "quilting", "Armour Accessories");
			AddMedievalFinishedCraft($"medieval_military_{culture.Key}_sidearm_harness",
				BuildRegionalCraftName("sew", "sidearm harness", index + 1), "Leathermaking",
				"Leathermaking", $"{MedievalWeaponKnowledgePrefix} {culture.Display}", 20, Difficulty.Normal,
				[
					CommodityInput(620.0, "leather", "Scabbard Leather Stock", colour: true, fineColour: true),
					CommodityInput(80.0, "wrought iron", "Tool Blank Stock")
				],
				["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Shears tag"],
				"sew", "sewing", "Weapon Accessories");
			AddMedievalFinishedCraft($"medieval_military_{culture.Key}_arrow_quiver",
				BuildRegionalCraftName("sew", "missile quiver", index + 1), "Leathermaking",
				"Leathermaking", $"{MedievalWeaponKnowledgePrefix} {culture.Display}", 20, Difficulty.Normal,
				[
					CommodityInput(720.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
					CommodityInput(60.0, "linen", "Spun Yarn", colour: true)
				],
				["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Shears tag"],
				"sew", "sewing", "Weapon Accessories");
			AddMedievalFinishedCraft($"medieval_military_{culture.Key}_field_pack",
				BuildRegionalCraftName("sew", "field pack", index + 1), "Leathermaking",
				"Leathermaking", $"{MedievalWeaponKnowledgePrefix} {culture.Display}", 20, Difficulty.Normal,
				[
					CommodityInput(1200.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
					CommodityInput(100.0, "linen", "Spun Yarn", colour: true)
				],
				["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Shears tag"],
				"sew", "sewing", "Campaign Accessories");
			AddMedievalFinishedCraft($"medieval_military_{culture.Key}_war_banner",
				BuildRegionalCraftName("sew", "war banner", index + 1), "Tailoring",
				"Tailoring", $"{MedievalWeaponKnowledgePrefix} {culture.Display}", 20, Difficulty.Normal,
				[
					CommodityInput(620.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CommodityInput(80.0, "linen", "Spun Yarn", colour: true)
				],
				["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"],
				"sew", "sewing", "Campaign Accessories");
		}

		AddMedievalFinishedCraft("medieval_weapon_common_crossbow", "assemble reinforced medieval crossbow",
			"Bowmaking", "Bowmaking", MedievalWorkshopKnowledge, 35, Difficulty.Hard,
			[
				CommodityInput(1300.0, "oak", "Crossbow Tiller Stock"),
				CommodityInput(950.0, "wrought iron", "Crossbow Prod Stock"),
				CommodityInput(480.0, "wrought iron", "Crossbow Lockwork Stock"),
				CommodityInput(120.0, "linen", "Military Cord Stock", colour: true, fineColour: true)
			],
			[
				"TagTool - InRoom - an item with the Bow Press tag",
				"TagTool - Held - an item with the Tillering Stick tag",
				"TagTool - InRoom - an item with the Crossbow Tiller Jig tag"
			],
			"assemble", "assembling", "Crossbows");
		AddMedievalCraft(
			"assemble crossbow bolts",
			"Bowmaking",
			"assemble crossbow bolts",
			"assembling crossbow bolts",
			"crossbow bolts being assembled",
			MedievalWorkshopKnowledge,
			"Bowmaking",
			20,
			Difficulty.Normal,
			MedievalFinishedPhases(),
			[
				CommodityInput(800.0, "oak", "Weapon Shaft Stock"),
				CommodityInput(360.0, "wrought iron", "Weapon Head Stock"),
				CommodityInput(80.0, "linen", "Fletching Stock", colour: true, fineColour: true)
			],
			[
				"TagTool - Held - an item with the Tillering Stick tag",
				"TagTool - Held - an item with the Shears tag"
			],
			[StableSimpleProduct("medieval_weapon_common_crossbow_bolts", 12)],
			knowledgeSubtype: "Crossbows");
	}

	private void SeedMedievalWritingAdministrationCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		var explicitlyCrafted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		for (var index = 0; index < MedievalCultureProfiles.Length; index++)
		{
			var culture = MedievalCultureProfiles[index];
			AddMedievalCraft(
				BuildRegionalCraftName("prepare", "sealed document bundle", index + 1),
				"Writing",
				"prepare a sealed document bundle",
				"preparing a sealed document bundle",
				"a sealed document bundle being prepared",
				$"{MedievalAdministrationKnowledgePrefix} {culture.Display}",
				"Tailoring",
				20,
				Difficulty.Normal,
				MedievalFinishedPhases(),
				[
					CommodityInput(260.0, "parchment", "Parchment Sheet Stock"),
					CommodityInput(80.0, "linen", "Seal Cord Stock", colour: true, fineColour: true),
					CommodityInput(40.0, "beeswax", "Sealing Wax Stock")
				],
				["TagTool - Held - an item with the Sewing Needle tag"],
				[StableSimpleProduct($"medieval_writing_{culture.Key}_office_bundle")],
				knowledgeSubtype: "Administration");
			AddMedievalFinishedCraft($"medieval_writing_{culture.Key}_record_tablet",
				BuildRegionalCraftName("prepare", "record tablet", index + 1), "Writing",
				"Tailoring", $"{MedievalAdministrationKnowledgePrefix} {culture.Display}", 15, Difficulty.Normal,
				[CommodityInput(260.0, "parchment", "Parchment Sheet Stock"), CommodityInput(160.0, "oak", "Furniture Panel Stock")],
				["TagTool - Held - an item with the Sewing Needle tag"], "prepare", "preparing", "Administration");
			AddMedievalFinishedCraft($"medieval_writing_{culture.Key}_tally_bundle",
				BuildRegionalCraftName("notch", "tally bundle", index + 1), "Writing",
				"Carpentry", $"{MedievalAdministrationKnowledgePrefix} {culture.Display}", 15, Difficulty.Easy,
				[CommodityInput(220.0, "willow", "Tally Stick Stock"), CommodityInput(30.0, "linen", "Seal Cord Stock", colour: true, fineColour: true)],
				["TagTool - Held - an item with the Awl Punch tag"], "notch", "notching", "Administration");
			AddMedievalFinishedCraft($"medieval_writing_{culture.Key}_seal_tag_packet",
				BuildRegionalCraftName("prepare", "seal tag packet", index + 1), "Writing",
				"Tailoring", $"{MedievalAdministrationKnowledgePrefix} {culture.Display}", 15, Difficulty.Easy,
				[CommodityInput(80.0, "linen", "Seal Cord Stock", colour: true, fineColour: true), CommodityInput(40.0, "parchment", "Parchment Sheet Stock")],
				["TagTool - Held - an item with the Sewing Needle tag"], "prepare", "preparing", "Administration");
			explicitlyCrafted.Add($"medieval_writing_{culture.Key}_office_bundle");
			explicitlyCrafted.Add($"medieval_writing_{culture.Key}_record_tablet");
			explicitlyCrafted.Add($"medieval_writing_{culture.Key}_tally_bundle");
			explicitlyCrafted.Add($"medieval_writing_{culture.Key}_seal_tag_packet");
		}

		AddMedievalFinishedCraft("medieval_writing_parchment_charter", "prepare sealable parchment charter", "Writing",
			"Tailoring", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(220.0, "parchment", "Parchment Sheet Stock"), CommodityInput(40.0, "beeswax", "Sealing Wax Stock"), CommodityInput(25.0, "linen", "Seal Cord Stock", colour: true, fineColour: true)],
			["TagTool - Held - an item with the Sewing Needle tag"], "prepare", "preparing", "Administration");
		AddMedievalFinishedCraft("medieval_writing_sealable_envelope", "fold sealable parchment envelope", "Writing",
			"Tailoring", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(120.0, "parchment", "Parchment Sheet Stock"), CommodityInput(30.0, "beeswax", "Sealing Wax Stock")],
			["TagTool - Held - an item with the Sewing Needle tag"], "fold", "folding", "Administration");
		AddMedievalFinishedCraft("medieval_writing_office_signet_ring", "engrave office signet ring", "Silversmithing",
			"Silversmithing", MedievalWorkshopKnowledge, 35, Difficulty.Hard,
			[CommodityInput(120.0, "bronze", "Tool Blank Stock")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"], "engrave", "engraving", "Administration");
		AddMedievalFinishedCraft("medieval_writing_office_seal_matrix", "engrave office seal matrix", "Silversmithing",
			"Silversmithing", MedievalWorkshopKnowledge, 35, Difficulty.Hard,
			[CommodityInput(180.0, "bronze", "Tool Blank Stock")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"], "engrave", "engraving", "Administration");
		AddMedievalFinishedCraft("medieval_writing_wax_seal_cake", "make sealing wax cake", "Candlemaking",
			"Candlemaking", MedievalWorkshopKnowledge, 15, Difficulty.Easy,
			[CommodityInput(80.0, "beeswax", "Sealing Wax Stock"), CommodityInput(20.0, "linen", "Seal Cord Stock", colour: true, fineColour: true)],
			[], "make", "making", "Administration");
		AddMedievalFinishedCraft("medieval_writing_paper_sheet", "size loose paper sheet", "Papermaking",
			"Tailoring", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(60.0, "Paper", "Paper Sheet Stock", colour: true, fineColour: true)],
			["TagTool - Held - an item with the Mould and Deckle tag"], "size", "sizing", "Paper");
		AddMedievalFinishedCraft("medieval_writing_bound_codex", "bind small parchment codex", "Bookbinding",
			"Leathermaking", MedievalWorkshopKnowledge, 35, Difficulty.Hard,
			[
				CommodityInput(420.0, "parchment", "Parchment Sheet Stock", colour: true, fineColour: true),
				CommodityInput(220.0, "leather", "Bookbinding Leather Stock", colour: true, fineColour: true),
				CommodityInput(80.0, "linen", "Seal Cord Stock", colour: true, fineColour: true)
			],
			[
				"TagTool - InRoom - an item with the Bookbinder's Sewing Frame tag",
				"TagTool - InRoom - an item with the Book Press tag",
				"TagTool - Held - an item with the Leather Paring Knife tag"
			],
			"bind", "binding", "Books");
		AddMedievalFinishedCraft("medieval_trade_balance_scale", "assemble merchant balance scale", "Blacksmithing",
			"Blacksmithing", MedievalWorkshopKnowledge, 30, Difficulty.Hard,
			[CommodityInput(1200.0, "bronze", "Standard Weight Blank"), CommodityInput(120.0, "linen", "Seal Cord Stock", colour: true, fineColour: true)],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"], "assemble", "assembling", "Measurement");
		AddMedievalFinishedCraft("medieval_trade_standard_weight_set", "cast standard weight set", "Blacksmithing",
			"Blacksmithing", MedievalWorkshopKnowledge, 30, Difficulty.Hard,
			[CommodityInput(1600.0, "bronze", "Standard Weight Blank")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Forge Tongs tag"], "cast", "casting", "Measurement");
		AddMedievalFinishedCraft("medieval_trade_false_weight_set", "cast false weight set", "Blacksmithing",
			"Blacksmithing", MedievalWorkshopKnowledge, 35, Difficulty.Hard,
			[CommodityInput(1500.0, "bronze", "Standard Weight Blank")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Forge Tongs tag"], "cast", "casting", "Measurement");
		AddMedievalFinishedCraft("medieval_trade_grain_measure", "build wooden grain measure", "Carpentry",
			"Carpentry", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(1400.0, "oak", "Furniture Panel Stock")],
			["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"], "build", "building", "Measurement");
		AddMedievalFinishedCraft("medieval_trade_tax_customs_kit", "assemble tax and customs kit", "Carpentry",
			"Carpentry", MedievalWorkshopKnowledge, 30, Difficulty.Hard,
			[
				CommodityInput(900.0, "oak", "Furniture Panel Stock"),
				CommodityInput(800.0, "bronze", "Standard Weight Blank"),
				CommodityInput(80.0, "beeswax", "Sealing Wax Stock"),
				CommodityInput(160.0, "willow", "Tally Stick Stock")
			],
			["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"], "assemble", "assembling", "Measurement");
		AddMedievalFinishedCraft("medieval_trade_sealable_bale", "wrap sealable trade bale", "Tailoring",
			"Tailoring", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(1200.0, "linen", "Sealable Bale Wrapper Stock", colour: true, fineColour: true), CommodityInput(80.0, "beeswax", "Sealing Wax Stock"), CommodityInput(80.0, "linen", "Seal Cord Stock", colour: true, fineColour: true)],
			["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"], "wrap", "wrapping", "Administration");
		AddMedievalFinishedCraft("medieval_writing_seal_cord_bundle", "bundle prepared seal cord", "Ropemaking",
			"Ropemaking", MedievalWorkshopKnowledge, 15, Difficulty.Easy,
			[CommodityInput(80.0, "linen", "Seal Cord Stock", colour: true, fineColour: true)],
			["TagTool - Held - an item with the Shears tag"], "bundle", "bundling", "Administration");
		AddMedievalFinishedCraft("medieval_writing_notary_kit", "assemble notary sealing kit", "Writing",
			"Tailoring", MedievalWorkshopKnowledge, 30, Difficulty.Hard,
			[
				CommodityInput(420.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
				CommodityInput(160.0, "parchment", "Parchment Sheet Stock"),
				CommodityInput(120.0, "beeswax", "Sealing Wax Stock"),
				CommodityInput(90.0, "linen", "Seal Cord Stock", colour: true, fineColour: true),
				CommodityInput(120.0, "bronze", "Tool Blank Stock")
			],
			[
				"TagTool - Held - an item with the Sewing Needle tag",
				"TagTool - Held - an item with the Wax Spatula tag"
			],
			"assemble", "assembling", "Administration");
		AddMedievalFinishedCraft("medieval_surveyor_measuring_rope", "knot measuring rope", "Ropemaking",
			"Ropemaking", MedievalWorkshopKnowledge, 15, Difficulty.Easy,
			[CommodityInput(800.0, "hemp", "Tool Blank Stock")],
			["TagTool - Held - an item with the Shears tag"], "knot", "knotting", "Measurement Props");
		explicitlyCrafted.UnionWith(
		[
			"medieval_writing_parchment_charter",
			"medieval_writing_sealable_envelope",
			"medieval_writing_office_signet_ring",
			"medieval_writing_office_seal_matrix",
			"medieval_writing_wax_seal_cake",
			"medieval_writing_paper_sheet",
			"medieval_writing_bound_codex",
			"medieval_trade_balance_scale",
			"medieval_trade_standard_weight_set",
			"medieval_trade_false_weight_set",
			"medieval_trade_grain_measure",
			"medieval_trade_tax_customs_kit",
			"medieval_trade_sealable_bale",
			"medieval_writing_seal_cord_bundle",
			"medieval_writing_notary_kit",
			"medieval_surveyor_measuring_rope"
		]);

		foreach (var spec in MedievalWritingAdministrationItemSpecs()
			         .Where(x => !explicitlyCrafted.Contains(x.StableReference)))
		{
			AddMedievalGeneralSpecCraft(spec, MedievalSpecCraftName("prepare", spec), "Administration");
		}
	}

	private void SeedMedievalMedicalApothecaryCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		var explicitlyCrafted = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"medieval_medical_linen_bandage_roll",
			"medieval_medical_apothecary_mortar",
			"medieval_medical_herb_pouch",
			"medieval_medical_field_stretcher"
		};

		AddMedievalFinishedCraft("medieval_medical_linen_bandage_roll", "roll clean linen bandage", "Tailoring",
			"Tailoring", MedievalWorkshopKnowledge, 10, Difficulty.Easy,
			[CommodityInput(180.0, "linen", "Garment Cloth", colour: true)],
			["TagTool - Held - an item with the Shears tag"], "roll", "rolling", "Medical");
		AddMedievalFinishedCraft("medieval_medical_apothecary_mortar", "carve apothecary mortar", "Masonry",
			"Masonry", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(1800.0, "stone", "Tool Blank Stock")],
			["TagTool - Held - an item with the Hammer tag"], "carve", "carving", "Medical");
		AddMedievalFinishedCraft("medieval_medical_herb_pouch", "sew apothecary herb pouch", "Leathermaking",
			"Leathermaking", MedievalWorkshopKnowledge, 15, Difficulty.Normal,
			[CommodityInput(260.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true)],
			["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Shears tag"], "sew", "sewing", "Medical");
		AddMedievalFinishedCraft("medieval_medical_field_stretcher", "assemble canvas field stretcher", "Tailoring",
			"Tailoring", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(1500.0, "linen", "Garment Cloth", colour: true), CommodityInput(1200.0, "oak", "Furniture Timber Stock")],
			["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"], "assemble", "assembling", "Medical");

		foreach (var spec in MedievalMedicalApothecaryItemSpecs()
			         .Where(x => !explicitlyCrafted.Contains(x.StableReference)))
		{
			AddMedievalGeneralSpecCraft(spec, MedievalSpecCraftName("make", spec), "Medical");
		}
	}

	private void SeedMedievalJewelleryDevotionalCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		var explicitlyCrafted = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"medieval_devotional_wooden_rosary",
			"medieval_jewellery_silver_brooch",
			"medieval_devotional_reliquary_locket",
			"medieval_offering_basin"
		};

		AddMedievalFinishedCraft("medieval_devotional_wooden_rosary", "string wooden prayer beads", "Carpentry",
			"Carpentry", MedievalWorkshopKnowledge, 15, Difficulty.Normal,
			[CommodityInput(160.0, "oak", "Tool Blank Stock"), CommodityInput(40.0, "linen", "Spun Yarn", colour: true)],
			["TagTool - Held - an item with the Awl Punch tag"], "string", "stringing", "Devotional");
		AddMedievalFinishedCraft("medieval_jewellery_silver_brooch", "make silver cloak brooch", "Silversmithing",
			"Silversmithing", MedievalWorkshopKnowledge, 25, Difficulty.Hard,
			[CommodityInput(80.0, "silver", "Tool Blank Stock")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"], "make", "making", "Jewellery");
		AddMedievalFinishedCraft("medieval_devotional_reliquary_locket", "make reliquary locket", "Silversmithing",
			"Silversmithing", MedievalWorkshopKnowledge, 30, Difficulty.Hard,
			[CommodityInput(120.0, "bronze", "Tool Blank Stock")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"], "make", "making", "Devotional");
		AddMedievalFinishedCraft("medieval_offering_basin", "hammer offering basin", "Blacksmithing",
			"Blacksmithing", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(900.0, "bronze", "Tool Blank Stock")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"], "hammer", "hammering", "Devotional");

		foreach (var spec in MedievalJewelleryDevotionalItemSpecs()
			         .Where(x => !explicitlyCrafted.Contains(x.StableReference)))
		{
			AddMedievalGeneralSpecCraft(spec, MedievalSpecCraftName("make", spec), "Jewellery and Devotional");
		}
	}

	private void SeedMedievalFurnitureAndContainerCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		var explicitlyCrafted = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"medieval_household_trestle_table",
			"medieval_household_boarded_chest",
			"medieval_household_lockable_strongbox",
			"medieval_household_aumbry_cupboard",
			"medieval_household_iron_lantern",
			"medieval_household_stained_glass_panel",
			"medieval_household_roof_tile_stack"
		};

		AddMedievalFinishedCraft("medieval_household_trestle_table", "build trestle work table", "Carpentry",
			"Carpentry", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(5200.0, "oak", "Furniture Timber Stock")],
			["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"], "build", "building", "Furniture");
		AddMedievalFinishedCraft("medieval_household_boarded_chest", "build boarded oak chest", "Carpentry",
			"Carpentry", MedievalWorkshopKnowledge, 25, Difficulty.Normal,
			[CommodityInput(4300.0, "oak", "Furniture Panel Stock"), CommodityInput(400.0, "wrought iron", "Lockwork Stock")],
			["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"], "build", "building", "Containers");
		AddMedievalFinishedCraft("medieval_household_lockable_strongbox", "build lockable strongbox", "Locksmithing",
			"Locksmithing", MedievalWorkshopKnowledge, 35, Difficulty.Hard,
			[CommodityInput(2200.0, "oak", "Furniture Panel Stock"), CommodityInput(800.0, "wrought iron", "Lockwork Stock"), CommodityInput(40.0, "beeswax", "Sealing Wax Stock")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Locksmith File Set tag"], "build", "building", "Containers");
		AddMedievalFinishedCraft("medieval_household_aumbry_cupboard", "build aumbry cupboard", "Carpentry",
			"Carpentry", MedievalWorkshopKnowledge, 25, Difficulty.Normal,
			[CommodityInput(3200.0, "oak", "Furniture Panel Stock")],
			["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"], "build", "building", "Furniture");
		AddMedievalFinishedCraft("medieval_household_iron_lantern", "fit iron lantern with panes", "Blacksmithing",
			"Blacksmithing", MedievalWorkshopKnowledge, 25, Difficulty.Normal,
			[
				CommodityInput(900.0, "wrought iron", "Tool Blank Stock"),
				CommodityInput(260.0, "glass", "Lantern Pane Stock", colour: true, fineColour: true),
				CommodityInput(60.0, "linen", "Spun Yarn", colour: true)
			],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"], "fit", "fitting", "Lighting");
		AddMedievalFinishedCraft("medieval_household_stained_glass_panel", "lead stained glass household panel", "Glassworking",
			"Glassworking", MedievalWorkshopKnowledge, 30, Difficulty.Hard,
			[
				CommodityInput(1200.0, "glass", "Stained Glass Panel Stock", colour: true, fineColour: true),
				CommodityInput(80.0, "beeswax", "Sealing Wax Stock")
			],
			["TagTool - Held - an item with the Grozing Iron tag", "TagTool - Held - an item with the Lead Knife tag"], "lead", "leading", "Luxury Decorations");
		AddMedievalFinishedCraft("medieval_household_roof_tile_stack", "fire glazed roof tile stack", "Pottery",
			"Pottery", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[
				CommodityInput(2200.0, "earthenware", "Tile Blank Stock"),
				CommodityInput(450.0, "slaked lime", "Glaze Slurry Stock")
			],
			["TagTool - InRoom - an item with the Hot Fire tag", "TagTool - Held - an item with the Glazing Basin tag"], "fire", "firing", "Building Goods");

		foreach (var spec in MedievalFurnitureContainerItemSpecs()
			         .Where(x => !explicitlyCrafted.Contains(x.StableReference)))
		{
			AddMedievalGeneralSpecCraft(spec, MedievalSpecCraftName("make", spec), "Furniture and Containers");
		}
	}

	private void SeedMedievalFoodBeverageCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		var explicitlyCrafted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		for (var index = 0; index < MedievalCultureProfiles.Length; index++)
		{
			var culture = MedievalCultureProfiles[index];
			AddMedievalCraft(
				BuildRegionalCraftName("arrange", "meal platter", index + 1),
				"Cooking",
				"arrange a regional meal platter",
				"arranging a regional meal platter",
				"a meal platter being arranged",
				$"{MedievalFoodKnowledgePrefix} {culture.Display}",
				"Cooking",
				15,
				Difficulty.Easy,
				MedievalFinishedPhases(),
				[CommodityInput(900.0, "oak", "Furniture Panel Stock")],
				["TagTool - Held - an item with the Rotary Quern tag"],
				[StableSimpleProduct($"medieval_food_{culture.Key}_meal_platter")],
				knowledgeSubtype: "Foodways");
			explicitlyCrafted.Add($"medieval_food_{culture.Key}_meal_platter");

			AddMedievalFinishedCraft($"medieval_food_{culture.Key}_staple_bread",
				BuildRegionalCraftName("prepare", "staple bread", index + 1), "Cooking",
				"Cooking", $"{MedievalFoodKnowledgePrefix} {culture.Display}", 15, Difficulty.Easy,
				[CommodityInput(520.0, "oak", "Furniture Panel Stock")],
				["TagTool - Held - an item with the Rotary Quern tag"],
				"prepare", "preparing", "Foodways");
			AddMedievalFinishedCraft($"medieval_food_{culture.Key}_pottage_bowl",
				BuildRegionalCraftName("fire", "pottage bowl", index + 1), "Pottery",
				"Pottery", $"{MedievalFoodKnowledgePrefix} {culture.Display}", 15, Difficulty.Normal,
				[CommodityInput(650.0, "earthenware", "Pottery Clay Body")],
				["TagTool - InRoom - an item with the Hot Fire tag"],
				"fire", "firing", "Foodways");
			AddMedievalFinishedCraft($"medieval_food_{culture.Key}_preserved_provision",
				BuildRegionalCraftName("wrap", "preserved provision", index + 1), "Tailoring",
				"Tailoring", $"{MedievalFoodKnowledgePrefix} {culture.Display}", 15, Difficulty.Easy,
				[
					CommodityInput(380.0, "linen", "Garment Cloth", colour: true),
					CommodityInput(40.0, "linen", "Spun Yarn", colour: true)
				],
				["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"],
				"wrap", "wrapping", "Foodways");
			AddMedievalFinishedCraft($"medieval_food_{culture.Key}_drinking_vessel",
				BuildRegionalCraftName("fire", "drinking vessel", index + 1), "Pottery",
				"Pottery", $"{MedievalFoodKnowledgePrefix} {culture.Display}", 15, Difficulty.Normal,
				[CommodityInput(520.0, "earthenware", "Pottery Clay Body")],
				["TagTool - InRoom - an item with the Hot Fire tag"],
				"fire", "firing", "Foodways");
			AddMedievalFinishedCraft($"medieval_food_{culture.Key}_feast_dish",
				BuildRegionalCraftName("carve", "feast dish", index + 1), "Carpentry",
				"Carpentry", $"{MedievalFoodKnowledgePrefix} {culture.Display}", 20, Difficulty.Normal,
				[CommodityInput(1200.0, "oak", "Furniture Panel Stock")],
				["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"],
				"carve", "carving", "Foodways");
			AddMedievalFinishedCraft($"medieval_food_{culture.Key}_market_ration",
				BuildRegionalCraftName("wrap", "market ration", index + 1), "Tailoring",
				"Tailoring", $"{MedievalFoodKnowledgePrefix} {culture.Display}", 15, Difficulty.Easy,
				[
					CommodityInput(420.0, "linen", "Garment Cloth", colour: true),
					CommodityInput(40.0, "linen", "Spun Yarn", colour: true)
				],
				["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"],
				"wrap", "wrapping", "Foodways");
			explicitlyCrafted.Add($"medieval_food_{culture.Key}_staple_bread");
			explicitlyCrafted.Add($"medieval_food_{culture.Key}_pottage_bowl");
			explicitlyCrafted.Add($"medieval_food_{culture.Key}_preserved_provision");
			explicitlyCrafted.Add($"medieval_food_{culture.Key}_drinking_vessel");
			explicitlyCrafted.Add($"medieval_food_{culture.Key}_feast_dish");
			explicitlyCrafted.Add($"medieval_food_{culture.Key}_market_ration");
		}

		AddMedievalFinishedCraft("medieval_food_grain_measure_sack", "sew measured grain sack", "Tailoring",
			"Tailoring", MedievalWorkshopKnowledge, 15, Difficulty.Easy,
			[CommodityInput(800.0, "linen", "Garment Cloth", colour: true)],
			["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"], "sew", "sewing", "Food and Drink");
		AddMedievalFinishedCraft("medieval_food_wine_measure_jug", "glaze marked wine measure jug", "Pottery",
			"Pottery", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(1000.0, "earthenware", "Pottery Clay Body"), CommodityInput(180.0, "slaked lime", "Glaze Slurry Stock")],
			["TagTool - InRoom - an item with the Hot Fire tag", "TagTool - Held - an item with the Glazing Basin tag"], "glaze", "glazing", "Food and Drink");
		AddMedievalFinishedCraft("medieval_food_oil_measure_jug", "fire marked oil measure jug", "Pottery",
			"Pottery", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(900.0, "earthenware", "Pottery Clay Body"), CommodityInput(160.0, "slaked lime", "Glaze Slurry Stock")],
			["TagTool - InRoom - an item with the Hot Fire tag", "TagTool - Held - an item with the Glazing Basin tag"], "fire", "firing", "Food and Drink");
		AddMedievalFinishedCraft("medieval_food_butter_churn", "cooper small butter churn", "Carpentry",
			"Carpentry", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(4200.0, "oak", "Coopered Staves"), CommodityInput(360.0, "wrought iron", "Hoop Stock")],
			["TagTool - Held - an item with the Croze tag", "TagTool - Held - an item with the Hammer tag"], "cooper", "coopering", "Food and Drink");
		AddMedievalFinishedCraft("medieval_food_ale_cask", "fill coopered ale cask", "Brewing",
			"Brewing", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[
				CommodityInput(7200.0, "oak", "Coopered Staves"),
				CommodityInput(600.0, "wrought iron", "Hoop Stock"),
				CommodityInput(900.0, "barley", "Ale Stock")
			],
			["TagTool - InRoom - an item with the Lauter Tun tag", "TagTool - Held - an item with the Croze tag"], "fill", "filling", "Food and Drink");
		AddMedievalFinishedCraft("medieval_food_cider_cask", "fill coopered cider cask", "Brewing",
			"Brewing", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[
				CommodityInput(7200.0, "oak", "Coopered Staves"),
				CommodityInput(600.0, "wrought iron", "Hoop Stock"),
				CommodityInput(900.0, "apple", "Cider Stock")
			],
			["TagTool - InRoom - an item with the Lauter Tun tag", "TagTool - Held - an item with the Croze tag"], "fill", "filling", "Food and Drink");
		AddMedievalFinishedCraft("medieval_food_mead_crock", "seal mead storage crock", "Brewing",
			"Brewing", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[
				CommodityInput(1000.0, "earthenware", "Pottery Clay Body"),
				CommodityInput(850.0, "honey", "Mead Stock"),
				CommodityInput(40.0, "beeswax", "Sealing Wax Stock")
			],
			["TagTool - InRoom - an item with the Hot Fire tag"], "seal", "sealing", "Food and Drink");
		AddMedievalFinishedCraft("medieval_food_brewing_tub", "cooper wide brewing tub", "Carpentry",
			"Carpentry", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(9000.0, "oak", "Coopered Staves"), CommodityInput(700.0, "wrought iron", "Hoop Stock")],
			["TagTool - Held - an item with the Croze tag", "TagTool - Held - an item with the Hammer tag"], "cooper", "coopering", "Food and Drink");
		explicitlyCrafted.Add("medieval_food_grain_measure_sack");
		explicitlyCrafted.Add("medieval_food_wine_measure_jug");
		explicitlyCrafted.Add("medieval_food_oil_measure_jug");
		explicitlyCrafted.Add("medieval_food_butter_churn");
		explicitlyCrafted.Add("medieval_food_ale_cask");
		explicitlyCrafted.Add("medieval_food_cider_cask");
		explicitlyCrafted.Add("medieval_food_mead_crock");
		explicitlyCrafted.Add("medieval_food_brewing_tub");

		foreach (var spec in MedievalFoodAndBeverageItemSpecs()
			         .Where(x => !explicitlyCrafted.Contains(x.StableReference)))
		{
			AddMedievalGeneralSpecCraft(spec, MedievalSpecCraftName("make", spec), "Food and Drink");
		}
	}

	private void SeedMedievalRepairKitCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		AddMedievalFinishedCraft("medieval_textile_repair_kit", "assemble textile repair kit", "Tailoring",
			"Tailoring", MedievalWorkshopKnowledge, 15, Difficulty.Easy,
			[CommodityInput(450.0, "linen", "Garment Cloth", colour: true), CommodityInput(60.0, "linen", "Spun Yarn", colour: true), CommodityInput(40.0, "beeswax")],
			["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"], "assemble", "assembling", "Repair Kits");
		AddMedievalFinishedCraft("medieval_leather_repair_kit", "assemble leather repair kit", "Leathermaking",
			"Leathermaking", MedievalWorkshopKnowledge, 15, Difficulty.Easy,
			[CommodityInput(500.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true), CommodityInput(80.0, "linen", "Spun Yarn", colour: true), CommodityInput(40.0, "beeswax")],
			["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Sewing Needle tag"], "assemble", "assembling", "Repair Kits");
		AddMedievalFinishedCraft("medieval_metal_repair_kit", "assemble metal repair kit", "Blacksmithing",
			"Blacksmithing", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(900.0, "wrought iron", "Tool Blank Stock"), CommodityInput(80.0, "leather", "Leather Strap", colour: true), CommodityInput(40.0, "beeswax")],
			["TagTool - InRoom - an item with the Anvil tag", "TagTool - Held - an item with the Hammer tag"], "assemble", "assembling", "Repair Kits");
	}

	private void SeedMedievalComponentGapCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		AddMedievalFinishedCraft("medieval_music_psaltery", "build small psaltery prop", "Carpentry",
			"Carpentry", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(900.0, "oak", "Furniture Panel Stock"), CommodityInput(120.0, "wrought iron", "Tool Blank Stock")],
			["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Awl Punch tag"], "build", "building", "Component Gap Props");
		AddMedievalFinishedCraft("medieval_game_chess_set", "carve chess set prop", "Bonecarving",
			"Bonecarving", MedievalWorkshopKnowledge, 25, Difficulty.Normal,
			[CommodityInput(700.0, "bone", "Tool Blank Stock")],
			["TagTool - Held - an item with the Awl Punch tag"], "carve", "carving", "Component Gap Props");
		AddMedievalFinishedCraft("medieval_horse_tack_display_set", "assemble horse tack display prop", "Leathermaking",
			"Leathermaking", MedievalWorkshopKnowledge, 20, Difficulty.Normal,
			[CommodityInput(1400.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true), CommodityInput(180.0, "wrought iron", "Tool Blank Stock")],
			["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Shears tag"], "assemble", "assembling", "Component Gap Props");
	}

	private void AddMedievalFinishedCraft(
		string stableReference,
		string craftName,
		string category,
		string traitName,
		string knowledgeName,
		int? minimumTraitValue,
		Difficulty difficulty,
		IEnumerable<string> inputs,
		IEnumerable<string> tools,
		string verb,
		string gerund,
		string knowledgeSubtype,
		Func<GameItemProto, IEnumerable<string>>? productsFactory = null)
	{
		if (!TryLookupReworkItem(stableReference, out var item))
		{
			return;
		}

		var visibleName = VisibleCraftName(item.ShortDescription);
		var products = productsFactory?.Invoke(item) ?? new[] { StableSimpleProduct(stableReference) };
		AddMedievalCraft(
			craftName,
			category,
			$"{verb} {visibleName}",
			$"{gerund} {visibleName}",
			$"{visibleName} being made",
			knowledgeName,
			traitName,
			minimumTraitValue,
			difficulty,
			MedievalFinishedPhases(),
			inputs,
			tools,
			products,
			knowledgeSubtype: knowledgeSubtype);
	}
}
