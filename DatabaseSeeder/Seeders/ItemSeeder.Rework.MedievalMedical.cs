#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalMedicalAndApothecaryItems()
	{
		#region Medieval Medical and Apothecary

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			]
		);

		CreateItem(
			"medieval_medical_linen_bandage_roll",
			"bandage",
			"a roll of clean linen bandage",
			null,
			"This clean linen bandage is rolled tightly for field, household, infirmary, or monastic use.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			5.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"Bandage_Simple",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("stone", MaterialBehaviourType.Stone,
			[
				"Eras / Medieval",
				"Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_medical_apothecary_mortar",
			"mortar",
			"a stone apothecary mortar",
			null,
			"This stone mortar has a matching pestle, a stained bowl, and enough weight for roots, gums, mineral powders, and dried herbs.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2200.0,
			26.0m,
			false,
			false,
			"stone",
			[
				"Eras / Medieval",
				"Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle",
				"Functions / Medical Items",
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
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			]
		);

		CreateItem(
			"medieval_medical_herb_pouch",
			"pouch",
			"an apothecary herb pouch",
			null,
			"This small pouch is divided into labelled folds for dried herbs, simples, gums, and small wrapped powders.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			160.0,
			12.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
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
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_medical_field_stretcher",
			"stretcher",
			"a canvas field stretcher",
			null,
			"This field stretcher uses two poles and a laced canvas bed for moving wounded people through halls, streets, camps, or battlefields.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"DragAid_Antiquity_FieldStretcher",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Uses the live DragAid stock component prototype until medieval-specific variants are worth splitting."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			]
		);

		CreateItem(
			"medieval_medical_poultice_cloth",
			"cloth",
			"a folded poultice cloth",
			null,
			"This folded cloth is ready for herbs, honey, vinegar, warm mash, or other poultice treatments in household or infirmary scenes.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			100.0,
			5.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"Bandage_Simple",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			]
		);

		CreateItem(
			"medieval_medical_salve_pot",
			"pot",
			"a small salve pot",
			null,
			"This small pottery pot is sized for ointments, fats, honeyed preparations, balms, or other external treatments.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			]
		);

		CreateItem(
			"medieval_medical_apothecary_jar",
			"jar",
			"a labelled apothecary jar",
			null,
			"This labelled jar has a narrow mouth and a cloth cover for dried simples, syrups, oils, powders, gums, or mineral preparations.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			16.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("bone", MaterialBehaviourType.Bone,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			]
		);

		CreateItem(
			"medieval_medical_cupping_horn",
			"horn",
			"a polished cupping horn",
			null,
			"This polished horn has a smooth rim and narrow end, suitable as a medical prop for cupping, suction, or healer kit scenes.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			10.0m,
			false,
			false,
			"bone",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_medical_surgical_roll",
			"roll",
			"a surgeon's tool roll",
			null,
			"This leather roll has narrow pockets for knives, probes, needles, clamps, hooks, thread, and wrapped instrument blanks.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			34.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
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
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_medical_bone_saw",
			"saw",
			"a small bone saw",
			null,
			"This small iron saw has a stiff blade and a wrapped grip for surgeon, barber, butcher, or battlefield medical scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			680.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_medical_splint_set",
			"splints",
			"a bundled splint set",
			null,
			"This bundled splint set contains thin boards, cloth ties, and padding for immobilising limbs in household, camp, or infirmary work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			700.0,
			12.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_medical_crutch_pair",
			"crutches",
			"a pair of wooden crutches",
			null,
			"This pair of crutches has padded tops, hand grips, and adjusted lengths for convalescent movement or infirmary furnishing.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			22.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			]
		);

		CreateItem(
			"medieval_medical_physicians_bag",
			"bag",
			"a physician's shoulder bag",
			null,
			"This leather shoulder bag has internal loops and pockets for jars, bandages, surgical rolls, wax tablets, and small instruments.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1400.0,
			54.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"Container_Tote",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine",
				"Market / Religious Goods"
			]
		);

		CreateItem(
			"medieval_medical_monastic_infirmary_kit",
			"kit",
			"a monastic infirmary kit",
			null,
			"This portable kit bundles bandage rolls, salve pots, herb pouches, a small cup, and record scraps for a monastery or charitable infirmary.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			48.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine",
				"Market / Religious Goods"
			],
			[
				"Holdable",
				"Container_Pack",
				"Wear_Backpack",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_medical_herb_drying_tray",
			"tray",
			"a herb drying tray",
			null,
			"This shallow tray has a woven base for drying herbs, flowers, roots, cloth strips, or apothecary ingredients.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			12.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine",
				"Market / Professional Tools / Standard Tools"
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
			"Medieval medical and apothecary item."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			]
		);

		CreateItem(
			"medieval_medical_bleeding_bowl",
			"bowl",
			"a small bleeding bowl",
			null,
			"This small bowl has a pouring lip and stained interior, suitable for barber-surgeon, infirmary, or ritual medical scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			14.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Functions / Medical Items",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval medical and apothecary item."
		);

		#endregion
	}
}
