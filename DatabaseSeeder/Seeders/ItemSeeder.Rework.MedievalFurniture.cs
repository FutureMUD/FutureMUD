#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalHouseholdFurniture()
	{
		#region Medieval Household Furniture

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Furniture"
			]
		);

		CreateItem(
			"medieval_household_three_legged_stool",
			"stool",
			"a three-legged wooden stool",
			null,
			"This three-legged stool is compact, sturdy, and suited to kitchens, workshops, stalls, and servants' corners.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6500.0,
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
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval household furniture item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Lighting",
				"Functions / Household Items / Household Lighting",
				"Functions / Material Functions / Fire"
			]
		);

		CreateItem(
			"medieval_household_iron_lantern",
			"lantern",
			"an iron hand lantern",
			null,
			"This iron lantern has a pierced body, a hinged door, and a handle for household, stable, street, or watch use.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			26.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Lighting",
				"Functions / Household Items / Household Lighting",
				"Functions / Material Functions / Fire"
			],
			[
				"Holdable",
				"Lantern",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval household furniture item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Lighting",
				"Functions / Material Functions / Fire",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_household_charcoal_brazier",
			"brazier",
			"a small charcoal brazier",
			null,
			"This small brazier is pierced for air flow and suited to chamber heat, workshop heat, cooking support, or ritual atmosphere.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Lighting",
				"Functions / Material Functions / Fire",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval household furniture item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Lighting",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods"
			]
		);

		CreateItem(
			"medieval_household_candle_stand",
			"stand",
			"a pricket candle stand",
			null,
			"This candle stand has a narrow stem, a pricket point, and a small drip pan for hall, chapel, chamber, or counting-room light.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			24.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Lighting",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval household furniture item."
		);

		EnsureMedievalItemMaterialAndTags("glass", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Market / Household Goods / Luxury Decorations",
				"Market / Religious Goods",
				"Functions / Household Items / Household Decorations"
			]
		);

		CreateItem(
			"medieval_household_stained_glass_panel",
			"panel",
			"a small stained glass panel",
			null,
			"This small leaded panel has coloured glass quarries set in came, suitable for chapels, halls, shrines, wealthy chambers, or lantern repair stock.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			95.0m,
			false,
			false,
			"glass",
			[
				"Eras / Medieval",
				"Market / Household Goods / Luxury Decorations",
				"Market / Religious Goods",
				"Functions / Household Items / Household Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval household furniture item."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_household_roof_tile_stack",
			"tiles",
			"a stack of glazed roof tiles",
			null,
			"This stack of fired roof tiles has glazed faces and rough backs, useful for town roofs, chapel repairs, kilns, and high-status building props.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			36.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval household furniture item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_household_door_bar",
			"bar",
			"a heavy door bar",
			null,
			"This heavy door bar is a visible security prop for gates, storerooms, halls, and shops until richer door-hardware item behaviour is needed.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			22.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval household furniture item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_household_iron_lockplate",
			"lockplate",
			"an iron lockplate",
			null,
			"This iron lockplate has a keyhole, rivet marks, and enough weight for a chest, strongbox, shop door, or storeroom door.",
			SizeCategory.Small,
			ItemQuality.Standard,
			950.0,
			24.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval household furniture item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_household_keyring",
			"keyring",
			"a ring of warded keys",
			null,
			"This keyring carries several simple warded keys for halls, cupboards, chests, shops, or institutional offices.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			320.0,
			20.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval household furniture item."
		);

		#endregion
	}
}
