#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalArmour()
	{
		#region Early Anglo-Saxon/Insular

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_early_anglo_saxon_armour",
			"armour",
			"a mail shirt over a belted tunic",
			null,
			"This armour package represents a mail shirt over a belted tunic, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Military-status armour variant."
		);

		#endregion

		#region Late Anglo-Saxon/Anglo-Danish

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_anglo_danish_armour",
			"armour",
			"a mail coat with a nasal helm",
			null,
			"This armour package represents a mail coat with a nasal helm, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Military-status armour variant."
		);

		#endregion

		#region Norse

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_norse_armour",
			"armour",
			"a riveted mail shirt and conical helm",
			null,
			"This armour package represents a riveted mail shirt and conical helm, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Military-status armour variant."
		);

		#endregion

		#region Norman/Angevin

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_norman_armour",
			"armour",
			"a long mail hauberk and nasal helm",
			null,
			"This armour package represents a long mail hauberk and nasal helm, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Military-status armour variant."
		);

		#endregion

		#region High Medieval Britain/Marcher

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_high_british_armour",
			"armour",
			"a mail hauberk over a padded aketon",
			null,
			"This armour package represents a mail hauberk over a padded aketon, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Military-status armour variant."
		);

		#endregion

		#region Gaelic/Welsh/Highland

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_gaelic_armour",
			"armour",
			"a padded coat with light mail patches",
			null,
			"This armour package represents a padded coat with light mail patches, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Military-status armour variant."
		);

		#endregion

		#region Carolingian/Frankish

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_carolingian_armour",
			"armour",
			"a mail shirt with a reinforced helm",
			null,
			"This armour package represents a mail shirt with a reinforced helm, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Military-status armour variant."
		);

		#endregion

		#region Capetian/Low Countries

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_capetian_armour",
			"armour",
			"a padded aketon under mail",
			null,
			"This armour package represents a padded aketon under mail, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Military-status armour variant."
		);

		#endregion

		#region German/HRE/Alpine-North Italian

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_german_hre_armour",
			"armour",
			"a hauberk with plate-reinforced fittings",
			null,
			"This armour package represents a hauberk with plate-reinforced fittings, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Military-status armour variant."
		);

		#endregion

		#region Iberian Christian

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_iberian_christian_armour",
			"armour",
			"a mail shirt over a quilted coat",
			null,
			"This armour package represents a mail shirt over a quilted coat, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Military-status armour variant."
		);

		#endregion

		#region al-Andalus/Maghreb

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_andalusi_armour",
			"armour",
			"a quilted coat with mail reinforcement",
			null,
			"This armour package represents a quilted coat with mail reinforcement, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Military-status armour variant."
		);

		#endregion

		#region Byzantine

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_byzantine_armour",
			"armour",
			"a lamellar corselet over a padded coat",
			null,
			"This armour package represents a lamellar corselet over a padded coat, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Lamellar",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Military-status armour variant."
		);

		#endregion

		#region Abbasid/Persianate

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_abbasid_armour",
			"armour",
			"a lamellar coat with padded sleeves",
			null,
			"This armour package represents a lamellar coat with padded sleeves, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Lamellar",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Military-status armour variant."
		);

		#endregion

		#region Fatimid Egypt/Ifriqiya

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_fatimid_armour",
			"armour",
			"a padded coat with scale panels",
			null,
			"This armour package represents a padded coat with scale panels, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Military-status armour variant."
		);

		#endregion

		#region Seljuk/Ayyubid/early Mamluk

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_seljuk_ayyubid_armour",
			"armour",
			"a lamellar riding coat and mail aventail",
			null,
			"This armour package represents a lamellar riding coat and mail aventail, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Lamellar",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Military-status armour variant."
		);

		#endregion

		#region Kyivan Rus/Novgorod

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_rus_novgorod_armour",
			"armour",
			"a mail shirt under a fur-trimmed coat",
			null,
			"This armour package represents a mail shirt under a fur-trimmed coat, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Chainmail",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Military-status armour variant."
		);

		#endregion

		#region Steppe Turkic/Cuman/Mongol-adjacent

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_steppe_turkic_armour",
			"armour",
			"a lamellar riding coat over quilted cloth",
			null,
			"This armour package represents a lamellar riding coat over quilted cloth, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Lamellar",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Military-status armour variant."
		);

		#endregion

		#region Song China

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_song_china_armour",
			"armour",
			"a lamellar vest over a padded robe",
			null,
			"This armour package represents a lamellar vest over a padded robe, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			180.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Armour"
			],
			[
				"Holdable",
				"Armour_Lamellar",
				"Wear_Hauberk",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Military-status armour variant."
		);

		#endregion
	}
}
