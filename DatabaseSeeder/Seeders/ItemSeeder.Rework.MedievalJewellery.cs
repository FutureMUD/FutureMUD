#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalJewelleryAndDevotionalGoods()
	{
		#region Early Anglo-Saxon/Insular

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_early_anglo_saxon_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Early Anglo-Saxon/Insular slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Devotional token."
		);

		#endregion

		#region Late Anglo-Saxon/Anglo-Danish

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_anglo_danish_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Late Anglo-Saxon/Anglo-Danish slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Devotional token."
		);

		#endregion

		#region Norse

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_norse_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Norse slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Devotional token."
		);

		#endregion

		#region Norman/Angevin

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_norman_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Norman/Angevin slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Devotional token."
		);

		#endregion

		#region High Medieval Britain/Marcher

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_high_british_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the High Medieval Britain/Marcher slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Devotional token."
		);

		#endregion

		#region Gaelic/Welsh/Highland

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_gaelic_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Gaelic/Welsh/Highland slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Devotional token."
		);

		#endregion

		#region Carolingian/Frankish

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_carolingian_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Carolingian/Frankish slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Devotional token."
		);

		#endregion

		#region Capetian/Low Countries

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_capetian_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Capetian/Low Countries slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Devotional token."
		);

		#endregion

		#region German/HRE/Alpine-North Italian

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_german_hre_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the German/HRE/Alpine-North Italian slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Devotional token."
		);

		#endregion

		#region Iberian Christian

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_iberian_christian_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Iberian Christian slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Devotional token."
		);

		#endregion

		#region al-Andalus/Maghreb

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_andalusi_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the al-Andalus/Maghreb slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Devotional token."
		);

		#endregion

		#region Byzantine

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_byzantine_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Byzantine slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Devotional token."
		);

		#endregion

		#region Abbasid/Persianate

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_abbasid_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Abbasid/Persianate slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Devotional token."
		);

		#endregion

		#region Fatimid Egypt/Ifriqiya

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_fatimid_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Fatimid Egypt/Ifriqiya slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Devotional token."
		);

		#endregion

		#region Seljuk/Ayyubid/early Mamluk

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_seljuk_ayyubid_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Seljuk/Ayyubid/early Mamluk slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Devotional token."
		);

		#endregion

		#region Kyivan Rus/Novgorod

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_rus_novgorod_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Kyivan Rus/Novgorod slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Devotional token."
		);

		#endregion

		#region Steppe Turkic/Cuman/Mongol-adjacent

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_steppe_turkic_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Steppe Turkic/Cuman/Mongol-adjacent slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Devotional token."
		);

		#endregion

		#region Song China

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_song_china_pilgrim_token",
			"token",
			"a regional devotional token",
			null,
			"This small devotional token is deliberately generic enough for the Song China slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Devotional token."
		);

		#endregion

		#region Medieval Jewellery and Devotional Goods

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_wooden_rosary",
			"rosary",
			"a wooden prayer bead strand",
			null,
			"This prayer bead strand has small polished beads, a cord loop, and a plain devotional pendant.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			90.0,
			12.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("silver", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Jewellery"
			]
		);

		CreateItem(
			"medieval_jewellery_silver_brooch",
			"brooch",
			"a silver cloak brooch",
			null,
			"This silver brooch has a bright pin, a framed face, and enough strength to hold a cloak or formal mantle closed.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			45.0,
			75.0m,
			false,
			false,
			"silver",
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Jewellery"
			],
			[
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_reliquary_locket",
			"locket",
			"a small reliquary locket",
			null,
			"This small locket is fitted with a tiny inner compartment for a relic, prayer slip, pressed flower, or private devotional token.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			60.0,
			90.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Container_Pouch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Jewellery"
			]
		);

		CreateItem(
			"medieval_jewellery_bronze_ring_pin",
			"pin",
			"a bronze ring pin",
			null,
			"This bronze ring pin is a practical cloak or mantle fastener with a plain hoop and a sharpened pin tongue.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			18.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Jewellery"
			],
			[
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Jewellery"
			]
		);

		CreateItem(
			"medieval_jewellery_enamel_disc_brooch",
			"brooch",
			"an enamelled disc brooch",
			null,
			"This disc brooch has a bright enamel face, a pin back, and enough finish for court, burgher, or churchly display.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			50.0,
			82.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Jewellery"
			],
			[
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts",
				"Functions / Worn Items / Jewellery"
			]
		);

		CreateItem(
			"medieval_jewellery_inlaid_belt_mount",
			"mount",
			"an inlaid belt mount",
			null,
			"This small belt mount has a polished face and rivet holes for decorating a court belt, guild belt, or military officer's harness.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			30.0,
			38.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts",
				"Functions / Worn Items / Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Beltable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("silver", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Jewellery"
			]
		);

		CreateItem(
			"medieval_jewellery_court_circlet",
			"circlet",
			"a narrow court circlet",
			null,
			"This narrow circlet is polished and lightly decorated, suitable for a court household, noble ceremony, or high-status ritual role.",
			SizeCategory.Small,
			ItemQuality.Good,
			180.0,
			140.0m,
			false,
			false,
			"silver",
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Jewellery"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			]
		);

		CreateItem(
			"medieval_devotional_icon_pendant",
			"pendant",
			"a painted icon pendant",
			null,
			"This small icon pendant has a painted face under a simple frame, with a cord loop for devotional wear or shrine display.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			18.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery / Necklaces"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery"
			]
		);

		CreateItem(
			"medieval_devotional_pilgrim_badge",
			"badge",
			"a cast pilgrim badge",
			null,
			"This small badge has a cast devotional emblem and pin points for attaching to a hat, cloak, satchel, or shrine cloth.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			28.0,
			14.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Functions / Worn Items / Jewellery"
			],
			[
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_devotional_reliquary_box",
			"box",
			"a small reliquary box",
			null,
			"This small reliquary box has a close-fitting lid and room for a wrapped relic, prayer slip, seal tag, or private devotional keepsake.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			95.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Market / Writing Materials"
			]
		);

		CreateItem(
			"medieval_devotional_scripture_tablet",
			"tablet",
			"a small devotional tablet",
			null,
			"This small tablet has a smoothed face for a painted, carved, or written devotional passage and a hole for hanging.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Religious Goods",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Religious Goods"
			]
		);

		CreateItem(
			"medieval_offering_basin",
			"basin",
			"a devotional offering basin",
			null,
			"This small bronze basin can sit in a chapel, shrine, or hall to receive votive offerings and devotional gifts.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1400.0,
			24.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Religious Goods"
			],
			[
				"Holdable",
				"OfferingReceiver_Antiquity_VotiveBasin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Uses the live OfferingReceiver stock component prototype until medieval-specific devotional receiver variants are worth splitting."
		);

		EnsureMedievalItemMaterialAndTags("silver", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Jewellery"
			]
		);

		CreateItem(
			"medieval_jewellery_silver_finger_ring",
			"ring",
			"a simple silver finger ring",
			null,
			"This simple silver ring has a plain polished band suitable for betrothal, guild display, devotional wear, or personal ornament.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			46.0m,
			false,
			false,
			"silver",
			[
				"Eras / Medieval",
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval jewellery or devotional item."
		);

		#endregion
	}
}
