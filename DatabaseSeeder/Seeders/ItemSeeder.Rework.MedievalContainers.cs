#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalContainers()
	{
		#region Medieval Containers

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			]
		);

		CreateItem(
			"medieval_household_trestle_table",
			"table",
			"a trestle work table",
			null,
			"This trestle table has removable boards and a broad working top for household work, trade, writing, or meals.",
			SizeCategory.Large,
			ItemQuality.Standard,
			32000.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Container_Table",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods"
			]
		);

		CreateItem(
			"medieval_household_aumbry_cupboard",
			"cupboard",
			"a small wall aumbry cupboard",
			null,
			"This small cupboard has a plain door, shelves, and peg holes for fitting it into a chapel, hall, counting room, or storeroom.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			48.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods"
			],
			[
				"Holdable",
				"Container_Cupboard",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			]
		);

		CreateItem(
			"medieval_household_plank_bench",
			"bench",
			"a long plank bench",
			null,
			"This long plank bench has trestle feet and a worn sitting surface for halls, taverns, workshops, courts, or market booths.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			28.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Container_Bench_Surface",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Luxury Furniture"
			]
		);

		CreateItem(
			"medieval_household_lordly_chair",
			"chair",
			"a carved high-backed chair",
			null,
			"This high-backed chair has a carved crest rail, heavy legs, and a seat meant for a hall, dais, chamber, or guild room.",
			SizeCategory.Large,
			ItemQuality.Good,
			28000.0,
			85.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Container_Couch_Surface",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			]
		);

		CreateItem(
			"medieval_household_rope_bedframe",
			"bedframe",
			"a rope-laced bedframe",
			null,
			"This bedframe has pegged rails, a rope-laced support, and enough space for mattress, blankets, and small bedside goods.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			52000.0,
			75.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Container_Bed_Surface",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			]
		);

		CreateItem(
			"medieval_household_straw_mattress",
			"mattress",
			"a straw-stuffed mattress",
			null,
			"This cloth mattress is stuffed with straw or rushes, tied at the edges, and sized for a simple bed, cot, or infirmary pallet.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			22.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Container_Cot_Surface",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			]
		);

		CreateItem(
			"medieval_household_wall_shelf",
			"shelf",
			"a pegged wall shelf",
			null,
			"This wall shelf has peg holes, a raised lip, and enough surface space for bowls, lamps, books, jars, or devotional goods.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			18.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Container_Wall_Shelf",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials"
			]
		);

		CreateItem(
			"medieval_household_book_shelves",
			"shelves",
			"a narrow book shelf",
			null,
			"This narrow shelf unit is built for codices, rolls, account bundles, wax tablets, and small chapel or school goods.",
			SizeCategory.Large,
			ItemQuality.Standard,
			21000.0,
			46.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Container_Bookcase_Shelves",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_household_market_counter",
			"counter",
			"a market counter",
			null,
			"This market counter has a broad display top, a front plank face, and enough working space for trade, measuring, or food service.",
			SizeCategory.Large,
			ItemQuality.Standard,
			36000.0,
			58.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Container_Counter",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials"
			]
		);

		CreateItem(
			"medieval_household_writing_desk",
			"desk",
			"a small writing desk",
			null,
			"This small desk has a sloped writing face and shallow space for parchment, wax tablets, pens, seal cord, and account scraps.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			62.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Container_Desk_Surface",
				"Container_Desk_Drawers",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods",
				"Market / Writing Materials"
			]
		);

		CreateItem(
			"medieval_household_lectern",
			"lectern",
			"a standing wooden lectern",
			null,
			"This standing lectern has a sloped book rest, a peg for a chain or cord, and a base suited to chapels, schools, or halls.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			48.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Container_Desk_Surface",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_household_storage_barrel",
			"barrel",
			"a coopered storage barrel",
			null,
			"This coopered barrel has hoop marks, a tight bung, and a broad enough body for grain, dry goods, ale, wine, oil, or salted stores.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			38.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_Barrel",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_household_wicker_basket",
			"basket",
			"a wicker market basket",
			null,
			"This wicker basket has a round belly, a wrapped handle, and room for produce, tools, laundry, kindling, or household trade goods.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			10.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Open_Bin",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_household_canvas_sack",
			"sack",
			"a stout canvas sack",
			null,
			"This canvas sack is heavy enough for grain, wool, charcoal, laundry, or pack goods, with a cord tie at the mouth.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			8.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Sack",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_household_market_stall",
			"stall",
			"a collapsible market stall",
			null,
			"This market stall has folding trestles, a plank top, and a simple cloth shade for fairs, streets, docks, or town squares.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			46000.0,
			68.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Container_Counter",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval container item."
		);

		#endregion
	}
}
