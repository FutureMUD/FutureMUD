#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string HistoricRootTagPath = "Eras / Historic";

	private void EnsureHistoricFoundationMaterialAndTags(string material, MaterialBehaviourType materialType, IEnumerable<string> tags)
	{
		EnsureAntiquityComponentGapMaterial(material, materialType);
		foreach (var tag in tags)
		{
			EnsureAntiquityTagPath(tag);
		}
	}

	private void SeedHistoricCommonWorkshopItems()
	{
		#region Historic Foundation Items

		EnsureHistoricFoundationMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Material Functions / Fire"
			]
		);

		CreateItem(
			"historic_workshop_hearth",
			"hearth",
			"an unlit workshop hearth",
			null,
			"This low clay hearth has a charcoal bed and a battered lip for pots, wax vessels, glue, pitch, and small workshop heating jobs.",
			SizeCategory.Large,
			ItemQuality.Standard,
			28000.0,
			42.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Material Functions / Fire"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Cross-era workshop heat source for antiquity and medieval installs."
		);

		EnsureHistoricFoundationMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Material Functions / Fire",
				"Functions / Material Functions / Hot Fire"
			]
		);

		CreateItem(
			"historic_lit_workshop_hearth",
			"hearth",
			"a lit workshop hearth",
			null,
			"This low clay hearth burns with a bed of glowing charcoal, giving steady heat for ordinary workshop preparation.",
			SizeCategory.Large,
			ItemQuality.Standard,
			28200.0,
			46.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Material Functions / Fire",
				"Functions / Material Functions / Hot Fire"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			"historic_workshop_hearth",
			"$0 burns low and settles back into $1.",
			TimeSpan.FromHours(2.0),
			null,
			"Lit state for the shared historic hearth."
		);

		EnsureHistoricFoundationMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Pottery Tools / Kiln"
			]
		);

		CreateItem(
			"historic_updraft_kiln",
			"kiln",
			"an unlit updraft kiln",
			null,
			"This squat updraft kiln has a low firebox, pierced setting floor, and enough chamber space for lamps, small vessels, tiles, and sealed wares.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			65000.0,
			120.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Pottery Tools / Kiln"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Cross-era pottery kiln shared by antiquity and medieval installs."
		);

		EnsureHistoricFoundationMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Pottery Tools / Kiln",
				"Functions / Tools / Pottery Tools / Kiln / Lit Kiln",
				"Functions / Material Functions / Hot Fire"
			]
		);

		CreateItem(
			"historic_lit_updraft_kiln",
			"kiln",
			"a lit updraft kiln",
			null,
			"This updraft kiln is stoked with charcoal heat, drawing fire through the chamber for firing vessels, lamps, tiles, and small wares.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			65200.0,
			128.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Pottery Tools / Kiln",
				"Functions / Tools / Pottery Tools / Kiln / Lit Kiln",
				"Functions / Material Functions / Hot Fire"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			"historic_updraft_kiln",
			"$0 cools and dies back into $1.",
			TimeSpan.FromHours(4.0),
			null,
			"Lit state for the shared historic kiln."
		);

		EnsureHistoricFoundationMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Loom",
				"Functions / Tools / Textilecraft Tools / Weaving Tools / Warp-Weighted Loom"
			]
		);

		CreateItem(
			"historic_warp_weighted_loom",
			"loom",
			"a warp-weighted loom",
			null,
			"This timber loom leans against a frame with rows of hanging loom weights, ready for broad cloth, bands, and household textiles.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			75.0m,
			false,
			false,
			"oak",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Loom",
				"Functions / Tools / Textilecraft Tools / Weaving Tools / Warp-Weighted Loom"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Loom",
				"Functions / Tools / Textilecraft Tools / Weaving Tools / Hand Loom"
			]
		);

		CreateItem(
			"historic_treadle_loom",
			"loom",
			"a timber treadle loom",
			null,
			"This treadle loom has a heavier timber frame, working pedals, and a beam wide enough for bolts of cloth.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			36000.0,
			120.0m,
			false,
			false,
			"oak",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Loom",
				"Functions / Tools / Textilecraft Tools / Weaving Tools / Hand Loom"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Spinning Tools / Drop Spindle"
			]
		);

		CreateItem(
			"historic_drop_spindle",
			"spindle",
			"a weighted drop spindle",
			null,
			"This small weighted spindle draws prepared fibre into yarn for ordinary household, workshop, and market textile production.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			4.0m,
			false,
			false,
			"oak",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Spinning Tools / Drop Spindle"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Sewing Needle",
				"Functions / Joining / Sewing"
			]
		);

		CreateItem(
			"historic_sewing_needle",
			"needle",
			"an iron sewing needle",
			null,
			"This slender iron needle has a sharp point and narrow eye, suitable for clothing, light leather, book cloth, and household sewing.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			18.0,
			2.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Sewing Needle",
				"Functions / Joining / Sewing"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Separation / Shearing / Shears"
			]
		);

		CreateItem(
			"historic_textile_shears",
			"shears",
			"a pair of iron textile shears",
			null,
			"These springy iron shears have broad blades and worn grips, useful for cloth, thread, felt, and light leather cutting.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Separation / Shearing / Shears"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Leatherworking Tools / Awl Punch"
			]
		);

		CreateItem(
			"historic_awl_punch",
			"awl",
			"an iron awl punch",
			null,
			"This iron awl punch has a tapered point and simple wooden grip for opening stitch holes in leather, parchment, and heavy cloth.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			9.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Leatherworking Tools / Awl Punch"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Dyeing Tools / Dye Vat"
			]
		);

		CreateItem(
			"historic_dye_vat",
			"vat",
			"a timber dye vat",
			null,
			"This timber dye vat is darkly stained around the rim and broad enough to soak skeins, narrow cloth, or small garments.",
			SizeCategory.Large,
			ItemQuality.Standard,
			32000.0,
			60.0m,
			false,
			false,
			"oak",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Dyeing Tools / Dye Vat"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Tanning Tools / Tanning Rack"
			]
		);

		CreateItem(
			"historic_tanning_rack",
			"rack",
			"a hide tanning rack",
			null,
			"This timber rack is pegged and corded for stretching hides while they dry, smoke, oil, or take tannin.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			42.0m,
			false,
			false,
			"oak",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Tanning Tools / Tanning Rack"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("stone", MaterialBehaviourType.Stone,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Foodmaking Tools / Hand Quern",
				"Functions / Tools / Milling Tools / Rotary Quern"
			]
		);

		CreateItem(
			"historic_hand_quern",
			"quern",
			"a rotary hand quern",
			null,
			"This two-stone rotary quern has a worn upper stone, a side handle, and a central feed hole for grinding grain or dry dye materials.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7800.0,
			24.0m,
			false,
			false,
			"stone",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Foodmaking Tools / Hand Quern",
				"Functions / Tools / Milling Tools / Rotary Quern"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Historic",
				"Market / Lighting",
				"Functions / Household Items / Household Lighting"
			]
		);

		CreateItem(
			"historic_oil_lamp",
			"lamp",
			"an unlit clay oil lamp",
			null,
			"This small clay oil lamp has a shallow reservoir, pinched wick spout, and soot-darkened lip from earlier use.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			180.0,
			5.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Historic",
				"Market / Lighting",
				"Functions / Household Items / Household Lighting"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Historic",
				"Market / Lighting",
				"Functions / Household Items / Household Lighting",
				"Functions / Material Functions / Fire"
			]
		);

		CreateItem(
			"historic_lit_oil_lamp",
			"lamp",
			"a lit clay oil lamp",
			null,
			"This clay oil lamp burns with a small steady flame at the wick, throwing enough light for close work or table use.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			190.0,
			6.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Historic",
				"Market / Lighting",
				"Functions / Household Items / Household Lighting",
				"Functions / Material Functions / Fire"
			],
			[
				"Holdable",
				"Lantern",
				"Destroyable_Misc"
			],
			"historic_oil_lamp",
			"$0 gutters and goes out, leaving $1.",
			TimeSpan.FromHours(3.0),
			null,
			"Lit state for the shared historic lamp."
		);

		EnsureHistoricFoundationMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Anvil"
			]
		);

		CreateItem(
			"historic_workshop_anvil",
			"anvil",
			"a low workshop anvil",
			null,
			"This low iron anvil has a broad working face, battered edges, and a fitted timber block for small fittings, rings, blades, and sheet work.",
			SizeCategory.Large,
			ItemQuality.Standard,
			34000.0,
			70.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Anvil"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Forge Tongs"
			]
		);

		CreateItem(
			"historic_forge_tongs",
			"tongs",
			"a pair of forge tongs",
			null,
			"These long iron tongs have squared jaws and wrapped grips for shifting heated blanks between a hearth, furnace, and anvil.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1280.0,
			24.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Forge Tongs"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Striking Tools / Hammer",
				"Functions / Tools / Metalworking Tools / Forge Hammer"
			]
		);

		CreateItem(
			"historic_workshop_hammer",
			"hammer",
			"an iron workshop hammer",
			null,
			"This compact iron hammer has a square striking face, peen, and smooth haft for riveting, shaping, and general workshop blows.",
			SizeCategory.Small,
			ItemQuality.Standard,
			980.0,
			20.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Striking Tools / Hammer",
				"Functions / Tools / Metalworking Tools / Forge Hammer"
			],
			[
				"Holdable",
				"Melee_Improvised Bludgeon",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		EnsureHistoricFoundationMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Bellows"
			]
		);

		CreateItem(
			"historic_bellows",
			"bellows",
			"a leather workshop bellows",
			null,
			"These leather bellows are fixed between timber boards with a narrow nozzle, ready to drive air into hearths and furnaces.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Historic",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Bellows"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Shared historic foundation item promoted from cross-era workshop and household support."
		);

		#endregion
	}

	private static IReadOnlyList<EraItemSpec> HistoricFoundationItemSpecs()
	{
		const string ToolTag = "Market / Professional Tools / Standard Tools";
		return
		[
			new("historic_workshop_hearth", "hearth", "an unlit workshop hearth",
				"This low clay hearth has a charcoal bed and a battered lip for pots, wax vessels, glue, pitch, and small workshop heating jobs.",
				SizeCategory.Large, ItemQuality.Standard, 28000.0, 42.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, ToolTag, "Functions / Material Functions / Fire"], ["Holdable", "Destroyable_Furniture"],
				"Cross-era workshop heat source for antiquity and medieval installs."),
			new("historic_lit_workshop_hearth", "hearth", "a lit workshop hearth",
				"This low clay hearth burns with a bed of glowing charcoal, giving steady heat for ordinary workshop preparation.",
				SizeCategory.Large, ItemQuality.Standard, 28200.0, 46.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, ToolTag, "Functions / Material Functions / Fire", "Functions / Material Functions / Hot Fire"], ["Holdable", "Destroyable_Furniture"],
				"Lit state for the shared historic hearth.",
				"historic_workshop_hearth", "$0 burns low and settles back into $1.", TimeSpan.FromHours(2)),
			new("historic_updraft_kiln", "kiln", "an unlit updraft kiln",
				"This squat updraft kiln has a low firebox, pierced setting floor, and enough chamber space for lamps, small vessels, tiles, and sealed wares.",
				SizeCategory.Huge, ItemQuality.Standard, 65000.0, 120.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Pottery Tools / Kiln"], ["Holdable", "Destroyable_Furniture"],
				"Cross-era pottery kiln shared by antiquity and medieval installs."),
			new("historic_lit_updraft_kiln", "kiln", "a lit updraft kiln",
				"This updraft kiln is stoked with charcoal heat, drawing fire through the chamber for firing vessels, lamps, tiles, and small wares.",
				SizeCategory.Huge, ItemQuality.Standard, 65200.0, 128.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Pottery Tools / Kiln", "Functions / Tools / Pottery Tools / Kiln / Lit Kiln", "Functions / Material Functions / Hot Fire"], ["Holdable", "Destroyable_Furniture"],
				"Lit state for the shared historic kiln.",
				"historic_updraft_kiln", "$0 cools and dies back into $1.", TimeSpan.FromHours(4)),
			new("historic_warp_weighted_loom", "loom", "a warp-weighted loom",
				"This timber loom leans against a frame with rows of hanging loom weights, ready for broad cloth, bands, and household textiles.",
				SizeCategory.Large, ItemQuality.Standard, 24000.0, 75.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Loom", "Functions / Tools / Textilecraft Tools / Weaving Tools / Warp-Weighted Loom"], ["Holdable", "Destroyable_Furniture"]),
			new("historic_treadle_loom", "loom", "a timber treadle loom",
				"This treadle loom has a heavier timber frame, working pedals, and a beam wide enough for bolts of cloth.",
				SizeCategory.Huge, ItemQuality.Standard, 36000.0, 120.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Loom", "Functions / Tools / Textilecraft Tools / Weaving Tools / Hand Loom"], ["Holdable", "Destroyable_Furniture"]),
			new("historic_drop_spindle", "spindle", "a weighted drop spindle",
				"This small weighted spindle draws prepared fibre into yarn for ordinary household, workshop, and market textile production.",
				SizeCategory.Tiny, ItemQuality.Standard, 120.0, 4.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Spinning Tools / Drop Spindle"], ["Holdable", "Destroyable_Misc"]),
			new("historic_sewing_needle", "needle", "an iron sewing needle",
				"This slender iron needle has a sharp point and narrow eye, suitable for clothing, light leather, book cloth, and household sewing.",
				SizeCategory.VerySmall, ItemQuality.Standard, 18.0, 2.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Sewing Needle", "Functions / Joining / Sewing"], ["Holdable", "Destroyable_Misc"]),
			new("historic_textile_shears", "shears", "a pair of iron textile shears",
				"These springy iron shears have broad blades and worn grips, useful for cloth, thread, felt, and light leather cutting.",
				SizeCategory.Small, ItemQuality.Standard, 780.0, 18.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Separation / Shearing / Shears"], ["Holdable", "Destroyable_Misc"]),
			new("historic_awl_punch", "awl", "an iron awl punch",
				"This iron awl punch has a tapered point and simple wooden grip for opening stitch holes in leather, parchment, and heavy cloth.",
				SizeCategory.Small, ItemQuality.Standard, 160.0, 9.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Leatherworking Tools / Awl Punch"], ["Holdable", "Destroyable_Misc"]),
			new("historic_dye_vat", "vat", "a timber dye vat",
				"This timber dye vat is darkly stained around the rim and broad enough to soak skeins, narrow cloth, or small garments.",
				SizeCategory.Large, ItemQuality.Standard, 32000.0, 60.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Dyeing Tools / Dye Vat"], ["Holdable", "Destroyable_Furniture"]),
			new("historic_tanning_rack", "rack", "a hide tanning rack",
				"This timber rack is pegged and corded for stretching hides while they dry, smoke, oil, or take tannin.",
				SizeCategory.Large, ItemQuality.Standard, 18000.0, 42.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Tanning Tools / Tanning Rack"], ["Holdable", "Destroyable_Furniture"]),
			new("historic_hand_quern", "quern", "a rotary hand quern",
				"This two-stone rotary quern has a worn upper stone, a side handle, and a central feed hole for grinding grain or dry dye materials.",
				SizeCategory.Normal, ItemQuality.Standard, 7800.0, 24.0m, "stone", MaterialBehaviourType.Stone,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Foodmaking Tools / Hand Quern", "Functions / Tools / Milling Tools / Rotary Quern"], ["Holdable", "Destroyable_Misc"]),
			new("historic_oil_lamp", "lamp", "an unlit clay oil lamp",
				"This small clay oil lamp has a shallow reservoir, pinched wick spout, and soot-darkened lip from earlier use.",
				SizeCategory.Tiny, ItemQuality.Standard, 180.0, 5.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, "Market / Lighting", "Functions / Household Items / Household Lighting"], ["Holdable", "Destroyable_Misc"]),
			new("historic_lit_oil_lamp", "lamp", "a lit clay oil lamp",
				"This clay oil lamp burns with a small steady flame at the wick, throwing enough light for close work or table use.",
				SizeCategory.Tiny, ItemQuality.Standard, 190.0, 6.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, "Market / Lighting", "Functions / Household Items / Household Lighting", "Functions / Material Functions / Fire"], ["Holdable", "Lantern", "Destroyable_Misc"],
				"Lit state for the shared historic lamp.",
				"historic_oil_lamp", "$0 gutters and goes out, leaving $1.", TimeSpan.FromHours(3)),
			new("historic_workshop_anvil", "anvil", "a low workshop anvil",
				"This low iron anvil has a broad working face, battered edges, and a fitted timber block for small fittings, rings, blades, and sheet work.",
				SizeCategory.Large, ItemQuality.Standard, 34000.0, 70.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Metalworking Tools / Anvil"], ["Holdable", "Destroyable_HeavyMetal"]),
			new("historic_forge_tongs", "tongs", "a pair of forge tongs",
				"These long iron tongs have squared jaws and wrapped grips for shifting heated blanks between a hearth, furnace, and anvil.",
				SizeCategory.Normal, ItemQuality.Standard, 1280.0, 24.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Metalworking Tools / Forge Tongs"], ["Holdable", "Destroyable_Misc"]),
			new("historic_workshop_hammer", "hammer", "an iron workshop hammer",
				"This compact iron hammer has a square striking face, peen, and smooth haft for riveting, shaping, and general workshop blows.",
				SizeCategory.Small, ItemQuality.Standard, 980.0, 20.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Striking Tools / Hammer", "Functions / Tools / Metalworking Tools / Forge Hammer"], ["Holdable", "Melee_Improvised Bludgeon", "Destroyable_Weapon"]),
			new("historic_bellows", "bellows", "a leather workshop bellows",
				"These leather bellows are fixed between timber boards with a narrow nozzle, ready to drive air into hearths and furnaces.",
				SizeCategory.Normal, ItemQuality.Standard, 1800.0, 24.0m, "leather", MaterialBehaviourType.Leather,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Metalworking Tools / Bellows"], ["Holdable", "Destroyable_Misc"])
		];
	}

	internal static IReadOnlyCollection<string> HistoricFoundationStableReferencesForTesting =>
		HistoricFoundationItemSpecs()
			.Select(x => x.StableReference)
			.ToArray();

	internal static IReadOnlyCollection<EraItemSpecTestData> HistoricFoundationItemSpecsForTesting =>
		HistoricFoundationItemSpecs()
			.Select(x => new EraItemSpecTestData(
				x.StableReference,
				x.Noun,
				x.ShortDescription,
				x.FullDescription,
				x.Size,
				x.Quality,
				x.WeightInGrams,
				x.Cost,
				x.Material,
				x.MaterialType,
				x.Tags,
				x.Components,
				x.BuilderNotes))
			.ToArray();
}
