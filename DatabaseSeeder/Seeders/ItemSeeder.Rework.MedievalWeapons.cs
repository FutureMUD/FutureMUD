#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalWeaponsShieldsAccessories()
	{
		#region Early Anglo-Saxon/Insular

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_early_anglo_saxon_seax",
			"seax",
			"a broad iron seax",
			null,
			"This broad single-edged blade has a plain wooden grip, a serviceable iron back, and a belt-ready sheath.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Shortsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_early_anglo_saxon_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a mail shirt over a belted tunic, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_early_anglo_saxon_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a mail shirt over a belted tunic, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_early_anglo_saxon_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a broad iron seax.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_early_anglo_saxon_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_early_anglo_saxon_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Early Anglo-Saxon/Insular. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_early_anglo_saxon_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Military banner and unit-marker accessory."
		);

		#endregion

		#region Common Weapons and Accessories

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_early_anglo_saxon",
			"shield",
			"a round linden shield",
			null,
			"This round shield has a limewood body, rawhide edging, and a simple iron boss at the centre.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Round",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_anglo_danish",
			"shield",
			"a painted round shield",
			null,
			"This painted round shield has a stout boss, leather hand-grip, and layered timber planks under hide facing.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Round",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_norse",
			"shield",
			"a bossed round shield",
			null,
			"This round shield is light enough to move quickly, with painted boards and a heavy iron boss.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Round",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_norman",
			"shield",
			"a kite shield",
			null,
			"This long kite shield narrows to a point, with a curved face and painted linen over timber.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Thureos",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_high_british",
			"shield",
			"a heater shield",
			null,
			"This heater shield has a compact triangular face suited to mounted and foot combat alike.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Thureos",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_gaelic",
			"shield",
			"a small hide targe",
			null,
			"This compact hide-faced shield is easy to carry through hills, woods, and rough border country.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Targe",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_carolingian",
			"shield",
			"a large round shield",
			null,
			"This large round shield has painted hide, a domed boss, and a grip set for shield-wall work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Round",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_capetian",
			"shield",
			"a heater shield",
			null,
			"This heater shield has a linened face, a sturdy hand-grip, and a shape suited to heraldic painting.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Thureos",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_german_hre",
			"shield",
			"a reinforced heater shield",
			null,
			"This reinforced heater shield has a painted face, leather edging, and a compact shape for town or field service.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Thureos",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_iberian_christian",
			"shield",
			"an almond shield",
			null,
			"This almond-shaped shield is hide-faced, bright-painted, and built for mounted or frontier fighting.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Thureos",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_andalusi",
			"shield",
			"a hide adarga",
			null,
			"This hide shield is light, curved, and tough, with layered leather stiffened for fast mounted work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Dhal",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_byzantine",
			"shield",
			"a painted oval shield",
			null,
			"This oval shield has painted linen over timber, a central grip, and a practical campaign curve.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Thureos",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_abbasid",
			"shield",
			"a round dhal shield",
			null,
			"This round shield has a hide face, metal bosses, and a compact form suited to infantry or mounted guard use.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Dhal",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_fatimid",
			"shield",
			"a round hide shield",
			null,
			"This round hide shield is light, dark, and edged with stitching around a metal-bossed centre.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Dhal",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_seljuk_ayyubid",
			"shield",
			"a round cavalry shield",
			null,
			"This round shield is hide-faced, compact, and reinforced with metal bosses for mobile fighting.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Dhal",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_rus_novgorod",
			"shield",
			"a tall oval shield",
			null,
			"This tall shield is timber-built, hide-faced, and edged for spear or axe fighting.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Thureos",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_steppe_turkic",
			"shield",
			"a compact hide shield",
			null,
			"This compact shield is hide-faced and light enough for fast mounted handling.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Dhal",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Armour / Shields"
			]
		);

		CreateItem(
			"medieval_shield_song_china",
			"shield",
			"a rattan shield",
			null,
			"This rattan shield is woven thick, lacquered dark, and fitted with a secure hand grip.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			55.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Armour / Shields"
			],
			[
				"Holdable",
				"Shield_Wicker",
				"Melee_Shield",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Military role shield."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Military Goods / Weapons / Crossbows"
			]
		);

		CreateItem(
			"medieval_weapon_common_crossbow",
			"crossbow",
			"a reinforced medieval crossbow",
			null,
			"This crossbow has a carved tiller, fitted nut, iron prod, stirrup, and cord ready for towns, castles, militias, hunts, or siege-adjacent scenes.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3800.0,
			180.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Military Goods / Weapons / Crossbows"
			],
			[
				"Holdable",
				"Crossbow",
				"Melee_Improvised Bludgeon",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval common weapon stock. Crossbow production uses tiller, prod, nut/lockwork, and bowstring subassemblies."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Military Goods / Weapons / Crossbows",
				"Market / Military Goods / Ammunition"
			]
		);

		CreateItem(
			"medieval_weapon_common_crossbow_bolts",
			"bolt",
			"a crossbow bolt",
			null,
			"This short bolt has a stout wooden shaft, quarrel head, and short fletching for crossbow ranges and armour-piercing scenes.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			75.0,
			2.5m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Military Goods / Weapons / Crossbows",
				"Market / Military Goods / Ammunition"
			],
			[
				"Holdable",
				"Stack_Number",
				"Ammo_BroadheadBolt",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval common missile stock. Bolt making consumes shaft, head, and fletching stock."
		);

		#endregion

		#region Late Anglo-Saxon/Anglo-Danish

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_anglo_danish_long_seax",
			"long seax",
			"a long iron seax",
			null,
			"This long single-edged blade sits between knife and sword, with a narrow fuller and a wrapped grip.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Shortsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_anglo_danish_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a mail coat with a nasal helm, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_anglo_danish_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a mail coat with a nasal helm, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_anglo_danish_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a long iron seax.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_anglo_danish_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_anglo_danish_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_anglo_danish_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Military banner and unit-marker accessory."
		);

		#endregion

		#region Norse

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_norse_bearded_axe",
			"bearded axe",
			"a bearded iron axe",
			null,
			"This bearded axe has a broad lower blade, a compact iron head, and a smooth haft for close fighting or ship work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Axe",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_norse_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a riveted mail shirt and conical helm, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_norse_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a riveted mail shirt and conical helm, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_norse_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a bearded iron axe.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_norse_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_norse_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Norse. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_norse_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Military banner and unit-marker accessory."
		);

		#endregion

		#region Norman/Angevin

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_norman_arming_sword",
			"arming sword",
			"a cruciform arming sword",
			null,
			"This straight double-edged sword has a cruciform guard, wheel pommel, and serviceable iron blade.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_norman_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a long mail hauberk and nasal helm, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_norman_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a long mail hauberk and nasal helm, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_norman_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a cruciform arming sword.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_norman_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_norman_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Norman/Angevin. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_norman_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Military banner and unit-marker accessory."
		);

		#endregion

		#region High Medieval Britain/Marcher

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_high_british_arming_sword",
			"arming sword",
			"a well-balanced arming sword",
			null,
			"This arming sword has a bright iron blade, straight guard, and a grip wrapped for mounted or foot service.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_high_british_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a mail hauberk over a padded aketon, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_high_british_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a mail hauberk over a padded aketon, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_high_british_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a well-balanced arming sword.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_high_british_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_high_british_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: High Medieval Britain/Marcher. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_high_british_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Military banner and unit-marker accessory."
		);

		#endregion

		#region Gaelic/Welsh/Highland

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_gaelic_long_spear",
			"long spear",
			"a long leaf-bladed spear",
			null,
			"This spear has a leaf-shaped iron head, a stout ash shaft, and a rawhide binding below the socket.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Long Spear",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_gaelic_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a padded coat with light mail patches, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_gaelic_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a padded coat with light mail patches, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_gaelic_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a long leaf-bladed spear.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_gaelic_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_gaelic_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Gaelic/Welsh/Highland. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_gaelic_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Military banner and unit-marker accessory."
		);

		#endregion

		#region Carolingian/Frankish

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_carolingian_spatha",
			"spatha",
			"a broad iron spatha",
			null,
			"This broad double-edged sword has a modest fuller, simple guard, and balanced grip for close formation fighting.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_carolingian_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a mail shirt with a reinforced helm, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_carolingian_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a mail shirt with a reinforced helm, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_carolingian_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a broad iron spatha.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_carolingian_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_carolingian_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Carolingian/Frankish. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_carolingian_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Military banner and unit-marker accessory."
		);

		#endregion

		#region Capetian/Low Countries

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_capetian_arming_sword",
			"arming sword",
			"a narrow arming sword",
			null,
			"This narrow sword has a straight guard, a tapered blade, and a leather-wrapped grip worn smooth by drill.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_capetian_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a padded aketon under mail, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_capetian_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a padded aketon under mail, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_capetian_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a narrow arming sword.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_capetian_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_capetian_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Capetian/Low Countries. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_capetian_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Military banner and unit-marker accessory."
		);

		#endregion

		#region German/HRE/Alpine-North Italian

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_german_hre_war_hammer",
			"war hammer",
			"a beaked iron war hammer",
			null,
			"This compact war hammer has a striking face, a rear beak, and a dense haft for armoured fighting.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Warhammer",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_german_hre_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a hauberk with plate-reinforced fittings, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_german_hre_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a hauberk with plate-reinforced fittings, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_german_hre_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a beaked iron war hammer.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_german_hre_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_german_hre_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: German/HRE/Alpine-North Italian. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_german_hre_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Military banner and unit-marker accessory."
		);

		#endregion

		#region Iberian Christian

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_iberian_christian_falcata_like_sword",
			"falcata-like sword",
			"a curved iron war sword",
			null,
			"This curved war sword has a forward-weighted edge, a guarded grip, and a blade meant for hard cutting blows.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_iberian_christian_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a mail shirt over a quilted coat, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_iberian_christian_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a mail shirt over a quilted coat, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_iberian_christian_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a curved iron war sword.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_iberian_christian_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_iberian_christian_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Iberian Christian. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_iberian_christian_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Military banner and unit-marker accessory."
		);

		#endregion

		#region al-Andalus/Maghreb

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_andalusi_saif",
			"saif",
			"a slender iron saif",
			null,
			"This slender sword has a lively balance, a wrapped grip, and a long cutting edge with a slight curve.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_andalusi_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a quilted coat with mail reinforcement, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_andalusi_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a quilted coat with mail reinforcement, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_andalusi_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a slender iron saif.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_andalusi_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_andalusi_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: al-Andalus/Maghreb. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_andalusi_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Military banner and unit-marker accessory."
		);

		#endregion

		#region Byzantine

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_byzantine_paramerion",
			"paramerion",
			"a curved iron paramerion",
			null,
			"This curved sword has a guarded grip, a polished edge, and fittings suited to formal military service.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_byzantine_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a lamellar corselet over a padded coat, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_byzantine_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a lamellar corselet over a padded coat, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_byzantine_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a curved iron paramerion.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_byzantine_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_byzantine_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Byzantine. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_byzantine_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Military banner and unit-marker accessory."
		);

		#endregion

		#region Abbasid/Persianate

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_abbasid_straight_sword",
			"straight sword",
			"a straight iron sword",
			null,
			"This straight sword has a narrow blade, rounded pommel, and carefully wrapped grip for court or field service.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_abbasid_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a lamellar coat with padded sleeves, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_abbasid_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a lamellar coat with padded sleeves, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_abbasid_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a straight iron sword.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_abbasid_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_abbasid_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Abbasid/Persianate. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_abbasid_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Military banner and unit-marker accessory."
		);

		#endregion

		#region Fatimid Egypt/Ifriqiya

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_fatimid_guard_spear",
			"guard spear",
			"a socketed guard spear",
			null,
			"This spear has a neat iron head, a long straight shaft, and ferrules for standing guard or field service.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Long Spear",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_fatimid_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a padded coat with scale panels, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_fatimid_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a padded coat with scale panels, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_fatimid_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a socketed guard spear.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_fatimid_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_fatimid_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_fatimid_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Military banner and unit-marker accessory."
		);

		#endregion

		#region Seljuk/Ayyubid/early Mamluk

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_seljuk_ayyubid_cavalry_mace",
			"cavalry mace",
			"a flanged iron mace",
			null,
			"This flanged mace has a compact head, short haft, and enough weight to threaten armour from horseback.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Mace",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_seljuk_ayyubid_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a lamellar riding coat and mail aventail, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_seljuk_ayyubid_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a lamellar riding coat and mail aventail, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_seljuk_ayyubid_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a flanged iron mace.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_seljuk_ayyubid_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_seljuk_ayyubid_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_seljuk_ayyubid_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Military banner and unit-marker accessory."
		);

		#endregion

		#region Kyivan Rus/Novgorod

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_rus_novgorod_war_axe",
			"war axe",
			"a broad iron war axe",
			null,
			"This broad axe has a socketed iron head, reinforced haft, and a practical edge for field or river work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Axe",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_rus_novgorod_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a mail shirt under a fur-trimmed coat, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_rus_novgorod_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a mail shirt under a fur-trimmed coat, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_rus_novgorod_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a broad iron war axe.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_rus_novgorod_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_rus_novgorod_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Kyivan Rus/Novgorod. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_rus_novgorod_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Military banner and unit-marker accessory."
		);

		#endregion

		#region Steppe Turkic/Cuman/Mongol-adjacent

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_steppe_turkic_sabre",
			"sabre",
			"a curved riding sabre",
			null,
			"This curved sabre has a lively single edge, a grip suited to mounted use, and plain iron fittings.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_steppe_turkic_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a lamellar riding coat over quilted cloth, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_steppe_turkic_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a lamellar riding coat over quilted cloth, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_steppe_turkic_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a curved riding sabre.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_steppe_turkic_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_steppe_turkic_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_steppe_turkic_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Military banner and unit-marker accessory."
		);

		#endregion

		#region Song China

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Weapons"
			]
		);

		CreateItem(
			"medieval_weapon_song_china_dao",
			"dao",
			"a single-edged iron dao",
			null,
			"This single-edged dao has a gentle curve, a guarded grip, and a plain scabbard suited to militia or guard work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Weapons"
			],
			[
				"Holdable",
				"Melee_Longsword",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Military role weapon."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Armour"
			]
		);

		CreateItem(
			"medieval_military_song_china_helmet",
			"helmet",
			"a regional medieval helmet",
			null,
			"This helmet follows the same regional equipment language as a lamellar vest over a padded robe, giving builders a head-protection counterpart to the seeded armour package.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			52.0m,
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
				"Wear_Hat",
				"Armour_Chainmail",
				"Destroyable_Armour"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Military helmet accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_song_china_padded_coif",
			"coif",
			"a padded military coif",
			null,
			"This padded coif is cut to sit under or beside a lamellar vest over a padded robe, with quilting, tie cords, and a practical campaign finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Armour",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Armour_HeavyClothing",
				"Insulation_Major",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Military padded headwear."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_song_china_sidearm_harness",
			"harness",
			"a leather sidearm harness",
			null,
			"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: a single-edged iron dao.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			24.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Weapon Accessories",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Sheath_Large",
				"Wear_Waist",
				"Beltable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Military sidearm carriage."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			]
		);

		CreateItem(
			"medieval_military_song_china_arrow_quiver",
			"quiver",
			"a leather arrow quiver",
			null,
			"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			22.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Weapon Accessories",
				"Market / Military Goods / Weapons / Bows"
			],
			[
				"Holdable",
				"Container_Quiver",
				"Wear_Shoulder",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Military missile-carriage accessory."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_military_song_china_field_pack",
			"pack",
			"a military field pack",
			null,
			"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Weapon Accessories",
				"Market / Household Goods / Standard Wares"
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
			"Medieval culture slice: Song China. Campaign-carrying accessory."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			]
		);

		CreateItem(
			"medieval_military_song_china_war_banner",
			"banner",
			"a regional war banner",
			null,
			"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			28.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Military Goods / Standards and Banners",
				"Market / Clothing / Standard Clothing"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Military banner and unit-marker accessory."
		);

		#endregion
	}
}
