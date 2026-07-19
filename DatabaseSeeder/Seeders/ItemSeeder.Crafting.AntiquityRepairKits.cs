using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static readonly string[] AntiquityRepairKitStableReferences =
	[
		"antiquity_textile_repair_kit",
		"antiquity_leather_repair_kit",
		"antiquity_wood_repair_kit",
		"antiquity_metal_repair_kit",
		"antiquity_stone_repair_kit",
		"antiquity_ceramic_repair_kit",
		"antiquity_hard_organic_repair_kit",
		"antiquity_field_repair_bundle"
	];

	private sealed record AntiquityRepairKitCraftSpec(
		string StableReference,
		string Name,
		string Category,
		string TraitName,
		string Blurb,
		string Action,
		int MinimumTraitValue,
		Difficulty Difficulty,
		IReadOnlyList<string> Inputs,
		IReadOnlyList<string> Tools);

	private void SeedAntiquityRepairKitCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		foreach (var kit in AntiquityRepairKitCrafts())
		{
			if (!TryLookupReworkItem(kit.StableReference, out _))
			{
				continue;
			}

			AddAntiquityCraft(
				kit.Name,
				kit.Category,
				kit.Blurb,
				kit.Action,
				"a repair kit assembly",
				AncientToolmakingKnowledge,
				kit.TraitName,
				kit.MinimumTraitValue,
				kit.Difficulty,
				AntiquityRepairKitAssemblyPhases(),
				kit.Inputs,
				kit.Tools,
				[StableSimpleProduct(kit.StableReference)],
				knowledgeSubtype: "Repair Kits",
				knowledgeDescription: "Ancient Toolmaking covers assembling portable repair kits from prepared workshop stock.",
				knowledgeLongDescription: "Ancient Toolmaking covers assembling portable repair kits from prepared workshop stock for textile, leather, wood, metal, stone, ceramic, hard organic, and mixed field repairs.");
		}
	}

	private static IReadOnlyList<AntiquityRepairKitCraftSpec> AntiquityRepairKitCrafts()
	{
		return
		[
			new(
				"antiquity_textile_repair_kit",
				"assemble textile repair kit",
				"Tailoring",
				"Tailoring",
				"assemble a textile repair kit",
				"assembling a textile repair kit",
				12,
				Difficulty.Easy,
				[
					CommodityInput(450.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CommodityInput(60.0, "linen", "Spun Yarn", colour: true),
					CommodityInput(40.0, "beeswax")
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag"
				]),

			new(
				"antiquity_leather_repair_kit",
				"assemble leather repair kit",
				"Leathermaking",
				"Leathermaking",
				"assemble a leather repair kit",
				"assembling a leather repair kit",
				14,
				Difficulty.Easy,
				[
					CommodityInput(500.0, "deer leather", "Prepared Leather Panel", colour: true, fineColour: true),
					CommodityInput(100.0, "leather", "Military Cord Stock", colour: true),
					CommodityInput(50.0, "beeswax")
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag"
				]),

			new(
				"antiquity_wood_repair_kit",
				"assemble woodwork repair kit",
				"Carpentry",
				"Carpentry",
				"assemble a woodwork repair kit",
				"assembling a woodwork repair kit",
				16,
				Difficulty.Easy,
				[
					CommodityInput(600.0, "oak", "Tool Blank Stock"),
					CommodityInput(300.0, "oak", "Furniture Timber Stock"),
					CommodityInput(60.0, "beeswax")
				],
				[
					"TagTool - Held - an item with the Hand Saw tag",
					"TagTool - Held - an item with the Wood Chisel tag",
					"TagTool - Held - an item with the Rasp tag"
				]),

			new(
				"antiquity_metal_repair_kit",
				"assemble metal repair kit",
				"Blacksmithing",
				"Blacksmithing",
				"assemble a metal repair kit",
				"assembling a metal repair kit",
				18,
				Difficulty.Normal,
				[
					CommodityInput(800.0, "bronze", "Tool Blank Stock"),
					CommodityInput(250.0, "bronze", "Door Hardware Stock"),
					CommodityInput(60.0, "beeswax")
				],
				[
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Forge Tongs tag"
				]),

			new(
				"antiquity_stone_repair_kit",
				"assemble stone repair kit",
				"Masonry",
				"Masonry",
				"assemble a stone repair kit",
				"assembling a stone repair kit",
				16,
				Difficulty.Easy,
				[
					CommodityInput(900.0, "limestone", "Tool Blank Stock"),
					CommodityInput(120.0, "leather", "Prepared Leather Panel", colour: true)
				],
				[
					"TagTool - Held - an item with the Stone Chisel tag",
					"TagTool - Held - an item with the Stone Mallet tag",
					"TagTool - Held - an item with the Polishing Stone tag"
				]),

			new(
				"antiquity_ceramic_repair_kit",
				"assemble ceramic repair kit",
				"Pottery",
				"Pottery",
				"assemble a ceramic repair kit",
				"assembling a ceramic repair kit",
				16,
				Difficulty.Easy,
				[
					CommodityInput(700.0, "earthenware", "Bisque Vessel Blank", colour: true, fineColour: true),
					CommodityInput(120.0, "clay", "Pottery Clay Body")
				],
				[
					"TagTool - Held - an item with the Potter's Rib tag",
					"TagTool - InRoom - an item with the Potter's Wheel tag",
					"TagTool - InRoom - an item with the Lit Kiln tag"
				]),

			new(
				"antiquity_hard_organic_repair_kit",
				"assemble bone and horn repair kit",
				"Scrimshawing",
				"Scrimshawing",
				"assemble a bone and horn repair kit",
				"assembling a bone and horn repair kit",
				16,
				Difficulty.Easy,
				[
					CommodityInput(650.0, "bone", "Tool Blank Stock"),
					CommodityInput(120.0, "leather", "Prepared Leather Panel", colour: true)
				],
				[
					"TagTool - Held - an item with the Stone Chisel tag",
					"TagTool - Held - an item with the Polishing Stone tag"
				]),

			new(
				"antiquity_field_repair_bundle",
				"assemble mixed field repair bundle",
				"Salvaging",
				"Salvaging",
				"assemble a mixed field repair bundle",
				"assembling a mixed field repair bundle",
				12,
				Difficulty.Normal,
				[
					CommodityInput(250.0, "leather", "Prepared Leather Panel", colour: true),
					CommodityInput(250.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CommodityInput(200.0, "oak", "Tool Blank Stock"),
					CommodityInput(150.0, "bronze", "Tool Blank Stock"),
					CommodityInput(60.0, "beeswax")
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - Held - an item with the Sewing Needle tag"
				])
		];
	}

	private static (int Seconds, string Echo, string FailEcho)[] AntiquityRepairKitAssemblyPhases()
	{
		return
		[
			(25, "$0 lay|lays out the prepared repair stock and sort|sorts it into usable packets.", "$0 lay|lays out the prepared stock, but miss|misses damaged or unsuitable pieces."),
			(35, "$0 trim|trims patches, fasteners, and small fittings with $t1.", "$0 trim|trims the repair stock poorly, spoiling some of the useful pieces."),
			(35, "$0 bind|binds the packets and tools into a compact repair kit.", "$0 bind|binds the packets poorly, leaving the kit loose and confused."),
			(25, "$0 close|closes $p1 and check|checks that the supplies are secure.", "$0 close|closes the kit, but the contents are too disordered to keep.")
		];
	}
}
