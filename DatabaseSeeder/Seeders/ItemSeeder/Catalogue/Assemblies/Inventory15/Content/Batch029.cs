// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory15Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch029() =>
	[
		new("item", "antiquity_tableware_nubian_blackware_serving_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4b7de6164f62a95d1a5f2d5336cbc02f2190a9c6f6d4af990da62e8afa4eb7e"),
		new("item", "antiquity_tableware_persian_bronze_rhyton", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "72c8a27788320d7be678c8dc7964a4f119f8fadc3cdbe9afc69d014f1274d0ec"),
		new("item", "antiquity_textile_repair_kit", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e07e9f4d5dd72cc666122ec329f35486d426b68be5c8ed25f853b1cc1c94aa9"),
		new("item", "antiquity_wax_tablet_locker", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "555ecbc45f591ac218e91fc6a5ba4702c5d18b62f3e1a19b463ca64c0ec7573d"),
		new("item", "antiquity_weapon_germanic_broad_seax", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5c128d855763cfcb3a4994888c39d8ddc92ed7582808639b43a4d3ee81d6c6e"),
		new("item", "antiquity_weapon_persian_composite_bow", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5ab4d454430454f151058c402cd777aa91d60982a7fbbdde398d52dbbc40b12"),
		new("item", "antiquity_weapon_persian_light_javelin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "baf4000bcf3ef4deadbeee0a516b162ce71922c6dd05fff989c8bd4ad0e5a2b4"),
		new("item", "antiquity_weapon_scythian_akinakes", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c16ed63707791b5dbeff643bbf5ec9a518d8b977eb9846ed5237d31811bac511"),
		new("item", "antiquity_wool_travel_mantle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7ffd747b981f31a076009d7f68dadc07e484ed136e2e7f50e630c72fe478968"),
		new("item", "antiquity_wool_udones", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e385e7aff4ba03274dfbf333496ad2dad897a674f7b28a83f44fdadc9f4a9309"),
		new("item", "antiquity_workshop_hearth", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "278d48336504aca810f0610073f0291aadfd441d7b57ec5f2f2920d23abccdd3"),
		new("item", "antiquity_wound_cleaning_kit", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "04a4ae3cda11b9d7473bcff32806495ac5def5797120e64263bdfbc79b53923d"),
		new("item", "antiquity_wrought_iron_gate_lever_lock", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "227fe8cbe8e5aa8e9343f10f8e7811dac2380d4e33bbba00ccfa1b7bbb68a18c"),
		new("item", "earlymodern_andean_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7c6dc5cdfb04f062dd6ea8da0535d6214eb7838013297d891d266f2d1708180"),
		new("item", "earlymodern_andean_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef40594010fe1a1c7d8abcb1b5d9bb74d92474dfa8c1e8402312c0bdd8aa2c67"),
		new("item", "earlymodern_centraleuropean_clothing_laced_wool_bodice", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c27398f86fef19292bbb03d7494cb6de16798d422823ac3bc808620005cbe29"),
		new("item", "earlymodern_colonial_military_frontier_hunting_shirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebb194278b5cbaf504daa833409e214b8fd6ab75ee4e81165619d8dc86728372"),
		new("item", "earlymodern_container_car_document_case_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e24dc327f49478c3757a2c12ce2e04c8b39122bc84973fd781d30ce5ebc685ca"),
		new("item", "earlymodern_container_frb_document_case_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2500751e816f1048ae44a36159da92a61a624e0cbad6e604bef923b241cc2d8"),
		new("item", "earlymodern_container_ger_tankard_27", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "607107aa01be076652096f62caf6471a6ed3c60c3a946525cec4cf6f408adcde"),
		new("item", "earlymodern_container_ibe_document_case_33", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a220ec6f9adea4b2bcacfdb5eaa04ec31426cf684e6e4ec0699b5bd9c471b3f6"),
		new("item", "earlymodern_container_ibe_tea_bowl_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4c78c84eb2e6864c69f3dacbdd0a5a26d1cf2cc25ff29e585a1b333b8159472"),
		new("item", "earlymodern_container_ina_punch_bowl_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "17cb184adf96ad9d83171b9e023bd6e966892de20001e9971ecc2a81031e992f"),
		new("item", "earlymodern_container_ina_tea_bowl_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f26bae0219d06cb73ed7ac9a923e17a97f89fbc0e62b200db98ac1825e50bd75"),
		new("item", "earlymodern_container_ita_tea_bowl_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f0ff3d1d33f5509a7ad1e29ccc4a0bcbc1ff42484d92c5334a12a31e43b3121"),
		new("item", "earlymodern_container_jos_bottle_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "16a878f7102e1109b33a2e1e3f8f6c0d2b1f7f305ba71b3021f826f1880e100c"),
		new("item", "earlymodern_container_jos_tankard_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3630ea59ae327c7022c2cf70fbec5ccd753d3d2ae46662dd37d9507c0b28702"),
		new("item", "earlymodern_container_jos_tea_caddy_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e63e19e22110c034bb27afce1ed3d4e8da3e98baf678cb4a73312db003d82ec9"),
		new("item", "earlymodern_container_kon_tea_pot_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "33bf1198eaaade51c8ff204e44ac67317fa2c1148b501fdcce83c2ea9448a95f"),
		new("item", "earlymodern_container_lmg_punch_bowl_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2dc2d30000d0a3f1dd3570508aa69dcd25a135e271aaa0c83e03cfd8760c4437"),
		new("item", "earlymodern_container_lmg_tea_pot_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1930ee2a938cdfa6bc5c86f3afd94463c5cd859ff95476c934cc453ea0a5a3ba"),
		new("item", "earlymodern_container_mag_storage_jar_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "16b6832c0e142f6b81d36ac058acbce0a275e755ab35dd4661a09be924610ced"),
		new("item", "earlymodern_container_mes_travel_coffer_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "083b8302f1c33417f9ad184748c5358ab130d84f20ea42bd5a12d20bd4078a08"),
		new("item", "earlymodern_container_mse_tea_caddy_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "00f976e5104f9891b5f3fbb93ed3301b469a8615e4e3f834776dfd125d418953"),
		new("item", "earlymodern_container_plh_coffee_pot_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba9fee9f0b3b477a9b575e5a1e8268fe27fd546ca0e94e285ee167f67e505e1e"),
		new("item", "earlymodern_container_plh_storage_jar_27", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe3af5b703b95e5d1dfeaee5a96220a004162253cfe6eceb8026092fa58ddc36"),
		new("item", "earlymodern_container_sca_document_case_30", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "70655240075427a879789df8ba2368ca0c1b3cc978da0f6cd3417539d1e0cd78"),
		new("item", "earlymodern_container_sic_ewer_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3afccf3a27cc684ca5cfa3b14bcf6be57d615aa68d7ae3d9c94fe0e79f94f2f2"),
		new("item", "earlymodern_door_and_secure_door_631", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "52a4fc6530ae6784d7a7a37a5aee4d1591633d96b3b862171cb10233653f0117"),
		new("item", "earlymodern_door_and_warded_lock_643", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "15d0812f1d1d962e74180a7dfd505edad9fb015a896b097713b4368aea4cbb6c"),
	];
}
