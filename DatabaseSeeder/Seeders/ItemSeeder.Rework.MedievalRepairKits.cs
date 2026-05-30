#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalRepairKits()
	{
		#region Medieval Repair Kits

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Repairing"
			]
		);

		CreateItem(
			"medieval_textile_repair_kit",
			"kit",
			"a textile repair kit",
			null,
			"This compact repair kit holds patches, thread, a needle case, and small shears for clothing, padding, and textile furnishings.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			22.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Repairing"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Repair_Cloth",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval repair-kit item using general material-family RepairKit components."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Repairing"
			]
		);

		CreateItem(
			"medieval_leather_repair_kit",
			"kit",
			"a leather repair kit",
			null,
			"This leather repair kit contains awls, waxed thread, small patches, and strap offcuts for field and workshop repairs.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			28.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Repairing"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Repair_Leather",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval repair-kit item using general material-family RepairKit components."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Repairing"
			]
		);

		CreateItem(
			"medieval_metal_repair_kit",
			"kit",
			"a metal repair kit",
			null,
			"This metal repair kit carries rivets, wire, small plates, and fitting blanks for armour, tools, locks, and household hardware.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			38.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Repairing"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Repair_Metal",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval repair-kit item using general material-family RepairKit components."
		);

		#endregion
	}
}
