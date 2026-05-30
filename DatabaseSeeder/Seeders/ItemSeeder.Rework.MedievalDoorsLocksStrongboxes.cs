#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalDoorsLocksAndStrongboxes()
	{
		#region Medieval Doors Locks and Strongboxes

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_household_boarded_chest",
			"chest",
			"a boarded oak chest",
			null,
			"This oak chest has iron straps, a hinged lid, and enough interior room for cloth, documents, or portable household goods.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			62.0m,
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
				"Container_Trunk",
				"Sealable_Container_Wax",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_household_lockable_strongbox",
			"strongbox",
			"a lockable iron-bound strongbox",
			null,
			"This compact strongbox has iron straps, a lock plate, and sealing points for wax or clay tamper marks.",
			SizeCategory.Small,
			ItemQuality.Good,
			8500.0,
			90.0m,
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
				"LockingContainer_Lockbox",
				"Sealable_Container_Wax",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_household_blanket_chest",
			"chest",
			"a blanket chest",
			null,
			"This broad blanket chest has a hinged lid and enough interior space for bedding, spare clothing, household linen, or seasonal gear.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30000.0,
			54.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Blanket_Box",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval lock, seal, chest, or security item."
		);

		#endregion
	}
}
