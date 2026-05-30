#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalComponentGapItems()
	{
		#region Medieval Component Gap Props

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Luxury Decorations",
				"Functions / Household Items / Leisure Goods"
			]
		);

		CreateItem(
			"medieval_music_psaltery",
			"psaltery",
			"a small psaltery",
			null,
			"This small psaltery has a shallow soundbox and wire strings. It is a social prop until musical instrument components exist.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			40.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Luxury Decorations",
				"Functions / Household Items / Leisure Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Seeded as a prop because richer runtime support is deferred to a future engine component."
		);

		EnsureMedievalItemMaterialAndTags("bone", MaterialBehaviourType.Bone,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Household Items / Leisure Goods"
			]
		);

		CreateItem(
			"medieval_game_chess_set",
			"set",
			"a carved chess set",
			null,
			"This carved chess set includes a folding board and simple pieces. It remains a prop until rules-aware game-set components exist.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			55.0m,
			false,
			false,
			"bone",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Household Items / Leisure Goods"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Seeded as a prop because richer runtime support is deferred to a future engine component."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_horse_tack_display_set",
			"set",
			"a horse tack display set",
			null,
			"This bundle of bridle, straps, and harness fittings is useful as stock decor or trade goods until animal tack and harness components exist.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			38.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Seeded as a prop because richer runtime support is deferred to a future engine component."
		);

		#endregion
	}
}
