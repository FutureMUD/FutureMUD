// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_military_aventail_spangenhelm", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf12605dbc34ee420026060ee7c343a83c05883319b0edebbdea4fe2b9a2d8a3"),
		new("item", "medieval_military_belly_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "af3da5470a522af341b8ccc59d8855d42fa74e2ff80a48718f82bb7921d082b5"),
		new("item", "medieval_military_bodkin_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e35f78146e9aacaa9a61ffeca3d992cbe424d26b63da92ff580a178f37bade9b"),
		new("item", "medieval_military_broad_weapon_baldric", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4bcfe3489558646e01f8a78deada369cb0b0bf0fcf7d986b8b706236a58c915f"),
		new("item", "medieval_military_coat_of_plates", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "51009c767bca452b534278910cb6b2859848dadba09e5b19c02d14f8ce67a792"),
		new("item", "medieval_military_fine_cord_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6ab8f1990379dc9e0812a4298e730c6960af3f70a41cf3e84cf94b0f484ee12"),
		new("item", "medieval_military_fine_riveted_mail_hauberk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd0fa30644e62eba700427ca23d279b8c7d7e2d6d1caf487704d9772c6b7c87e"),
		new("item", "medieval_military_fine_short_sword", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f34e51a445403e6ef444b9bc8ac93064187ea9a41c9c8e2f61482115972a92eb"),
		new("item", "medieval_military_heavy_tower_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "575aaecbececfb63670fe73df577dbed2055d2cbc5862557835fd9e571ba77eb"),
		new("item", "medieval_military_kettle_style_helmet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0daa0c6be23a3b52b4aa32eff314f1a4d9c6ed0dd7b7096917c00d260b534000"),
		new("item", "medieval_military_leather_baldric", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8277d009a09a1f7d21f9ab5c28f0c3a2f086af7467bad80f73e1caecde3824bc"),
		new("item", "medieval_military_leather_peytral", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c992efb25659d4a1b599741b969aa1056383a62ba118fd1794011167abb4595"),
		new("item", "medieval_military_padded_horse_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6ea40a54e0b465bb0ede38703e9dba28f243163a822281eaa7c646460d0ed11"),
		new("item", "medieval_military_padded_mittens", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "638eb941b27fa2ed5d64ca624a3deca50bb50ffb6303d1095ac6d43589dbae97"),
		new("item", "medieval_military_painted_kite_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e868254724a6cf96acf30fd4eccd265fe680b5d9e71ca4e412d4f85da709cee4"),
		new("item", "medieval_military_plain_adarga", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc191e1c203e0a921d023895862ea7fe7ea7ba73528bc3927d41e92e2a344337"),
		new("item", "medieval_military_plain_steel_dagger", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4cc5a272e34cbf3aadd642776a7b198df0c8bfcb348cf86a551a5113a323cc2"),
		new("item", "medieval_military_plain_steel_vambraces", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "64e9e507db1d25560e1de26b8ab45fc41d5ad5a54e3cd1e4be85bc141499aecb"),
		new("item", "medieval_military_pot_lid_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2493d645b976f21ebca551d7a701250b8798670ad9026dd27808037349f31224"),
		new("item", "medieval_military_recurved_riders_shortbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e528011640744458fcc04ecddb600b3b763f8acd609f777ca376e042112a1fe"),
		new("item", "medieval_military_riders_short_bowcase", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2fbc84f826de4aca3b608ae7b6675f311dbe8e72f08bbdd3808257f45721ae3"),
		new("item", "medieval_military_simple_weapon_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf0fec8e280ef80b9754112dead5fb2a03f21f86ce01fb9c2d248d6fac4763d2"),
		new("item", "medieval_military_sling_stone_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b00c4ca5497c3da48fbaddf073c7a7990a1586108a7d6d9e906bf265f3114c2f"),
		new("item", "medieval_military_small_latch_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cc0ca40995093e3bd13183b49b60dfd614b9c27513c64fe9c77ddde77b7a164"),
		new("item", "medieval_military_worn_buckler", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "39f9fc4fe2248856c3005ea7e3dcb630ab484b273f0370dd15cb843cc22ff40f"),
		new("item", "medieval_northern_ship_chest_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f1040c491c8c0079c4ad60d15a696a79151d887882496b4cc799029b8513930"),
		new("item", "medieval_padded_silk_overrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "25e7c3b42576f029a70e469d030e5cae3ceb06776bb5ecd84b65316cec393f3b"),
		new("item", "medieval_pair_oval_brooches", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "844251b5447ba159b1df9cab84f4c08df7144c95a7eb5931efb8b13f1bd7a601"),
		new("item", "medieval_rectangular_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb2115a714a27db5c1a6e9fefcbba09498096f98a177fd4d3c639c667c977056"),
		new("item", "medieval_regional_goryeo_pine_merchant_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e5f1fb8afb78b652ebf5d72b8055fd1d94744b3a37a1c08748c5886b26e5457"),
		new("item", "medieval_regional_magyar_bronze_mounted_saddle_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "11522af41594584d3f00a1442701d417bd14e228f25156a1720048cc8e372986"),
		new("item", "medieval_religious_painted_screen_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b60b2add8057651639ba1baeaae45190d588e8616dcf75f06dc63035149af61"),
		new("item", "medieval_religious_water_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba214fabf0ccaf3c41e732fd5bc5e05296fb4d6a14ffd9e74b0b16f71f781d42"),
		new("item", "medieval_repair_glass_poor_lead_patch_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "19e878197208e20896c390c9868e6a919ca8837e15990f312096efb64a6acd17"),
		new("item", "medieval_shared_cupboard_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8afc2e19b421935e4e5b04e5e5f11f978959c15c5f746617d25a73b98c90b40d"),
		new("item", "medieval_shared_door_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "924825f6733fa105eb683faa0a04aa2bbf1123ea6c57b05fd31fd8f4c0514f33"),
		new("item", "medieval_shared_sliding_bolt_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2c972ef2344096629895b1f132fad464054c0528f78238dfc521b284648d4ed"),
		new("item", "medieval_shared_small_chest_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "76cd77adc22f213247ce3d06815e684e409736b505d60579aae380d545f05e9b"),
		new("item", "medieval_shared_tag_holed_warded_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "54e5885b4b116233034eb2f01c953e7134714a9a45b569615e7456bbc5458eb8"),
		new("item", "medieval_shinto_offering_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0553b148f81e4841f5df46c0a21b8831ac35def7009c8839a848078d5b22a486"),
	];
}
