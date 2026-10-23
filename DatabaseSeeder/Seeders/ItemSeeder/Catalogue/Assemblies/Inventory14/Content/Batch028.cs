// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory14Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch028() =>
	[
		new("item", "antiquity_food_punic_honeyed_pastry", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4fef1f9e497fd476e986c8dd42f67a7f1b22fa4d24f38b4621aac3b06e4509a"),
		new("item", "antiquity_food_punic_oilseed_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6118288e7eb54623ff23fef365043b7d9f4c2bc32ac56484417c92dbfe8ccbfc"),
		new("item", "antiquity_food_roman_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "af73537f5f5ebccf79e9bda1bb02a3caad09d0b2cd441286741efc7bd9225ded"),
		new("item", "antiquity_food_scythian_sarmatian_fruit_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ebd9b0f9f8ba3ee856f5d3be4de64116dc877b70d8099745e291ecf5ce75f29"),
		new("item", "antiquity_fullers_trough", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0dfb52df0c2fc534060f536d61f9443c811ff453d93eddc5b6b386257aa2bdb5"),
		new("item", "antiquity_goatskin_bellows", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "faf493ed0df7ef04649e34def046c6824195209425e17f84e95f51cf8333cfda"),
		new("item", "antiquity_household_terracotta_figurine", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ccf59a1f6b2543fbf82c080fb357795e02732d65bd22d09775ae75b7a5856366"),
		new("item", "antiquity_ivory_inlaid_box", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "99efd332aa725ea15edcaf1e5941e72f5fca2b18e4b05733f57c6af2fced9e59"),
		new("item", "antiquity_knee_length_wool_tunica", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ee595af534def1049da58d658a8fe768a3ea5fbc3691b2bb5d7831a5e35e29b"),
		new("item", "antiquity_lined_cistern", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "564aca70c6db50c7ff86e3abfdd55db54f51f27c5939af46914ad8cfb52554b6"),
		new("item", "antiquity_linen_arm_sling", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc0cc4a5dca52dacd4c83b71429fa94851ab057a3570ef7e794b2deb7ceb4b89"),
		new("item", "antiquity_liquid_calcite_perfume_jar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9207ddb48d49edd7cfae41f3ff80b7a9218480d840344489d6c4a89b210492ec"),
		new("item", "antiquity_liquid_narrow_punic_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f482d0e64b202f99d151d706ba36a8bf0615923474d4a7c9b46bb2ac2021d7fa"),
		new("item", "antiquity_liquid_painted_kushite_canteen", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa152184360a6b6e2efaeb2300cf9754f3c71060b1d14234afe22ae194371439"),
		new("item", "antiquity_liquid_papyrus_slung_water_jar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6ad11629333a4fed29a737a5ef3b9d317f3e9e441fdc29282ced13f422b2eff"),
		new("item", "antiquity_liquid_shallow_kylix", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4e906cb88abe352781b76d3b7458e5d24039f882562f85a1650e6f8babfab99"),
		new("item", "antiquity_liquid_silver_court_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "071583bb4d9d4ee056d113bb31077841d32134fb65ec84eff98161569528d222"),
		new("item", "antiquity_lit_annealing_lehr", "antiquity", ["antiquity"], ["item:antiquity_annealing_lehr"], ItemSeederOwnershipPolicy.RequiredRelationship, "9fa20116d9590f52511562c0e092afca8e4e571852178fcaea5d33539377e4af"),
		new("item", "antiquity_lit_glory_hole_furnace", "antiquity", ["antiquity"], ["item:antiquity_glory_hole_furnace"], ItemSeederOwnershipPolicy.RequiredRelationship, "9c7702f95d07502696d6a8cee821bd049be0377c38027bf6479ec25a5d34031a"),
		new("item", "antiquity_lockable_bronze_clad_civic_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca8def9371097ba1dbcdca5d5a478f14e5c5a7a27c18b5c092229c5078557f90"),
		new("item", "antiquity_lockable_bronze_latticed_gate", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8b33058338f0d194a70e77f231c4e5a21258f322ad010abaa3ef370a44a88e1"),
		new("item", "antiquity_long_iron_gate_key", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "494a8fb377e4a47931a044e9c74246a332d3d44bd490ff9f3ac4814ae50fede2"),
		new("item", "antiquity_long_wool_tunica", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7684b0088729611abb677999dac0bb2350ecfd3835429af9869d2178fcb0555"),
		new("item", "antiquity_loose_papyrus_sheet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "841624489ee7dc98a16f2f56d859a7c55fd2a340f1c80b062b112a0584ad84c9"),
		new("item", "antiquity_low_strapped_leather_shoes", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d173fb28eed7fb274393c8a98a78062455ba06bd0ed9dac680c1a422cf21535"),
		new("item", "antiquity_mandrake_smoke_cone", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4dae1ffaaca1c78c13c0e84c65222877cd83f5ea34109c62d75677d7d47422e9"),
		new("item", "antiquity_moulded_terracotta_lamp", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "988455cd33fe758f9463180562f2cd47452bf5360871cdcaedae69e879462896"),
		new("item", "antiquity_papyrus_amulet_pouch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "997cf6e2f8fcea1761b0ad0cc1b0d300e2586aabe45c793f2acc24cf1d273adc"),
		new("item", "antiquity_papyrus_pressing_board", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "88d49ab1ee86c252181ce07da89e547ac0beee394aa6e2fa1aa4addbfd184dec"),
		new("item", "antiquity_persian_padded_riding_coat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5228ae15bd24c6ab0e57c33125732ed2265897ea858026da8ff7bbc9f6be44c"),
		new("item", "antiquity_quill_curing_sand", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "da88e3b9c91071896a849b140d506c43c5b4934bf60d70a687b369c5bfbabeaf"),
		new("item", "antiquity_reed_flute", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fac6b05e284e7c594289501408a005cfd2e11a3085628c596f228505c4370976"),
		new("item", "antiquity_reed_stylus", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "af7ae688ec7065ac1601ae67d43ef40d6b47a5971888b8f1a0b88fe255444424"),
		new("item", "antiquity_retting_trough", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e73eee5e12319956feb1891b69571198348e49e8afe6f00bfd9f18c72ace4aed"),
		new("item", "antiquity_roman_bronze_muscle_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f57790e41acf5da73376f341a5f3da2445732b0654de1dbb4a21d7be67a0b6fa"),
		new("item", "antiquity_senet_game_board", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "229cc015912e0d2b1515aea93a620c8da47ff5089699344db0da174945a6efd3"),
		new("item", "antiquity_sharpening_whetstone", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c2b07cf12b1ef72356a222083a614ed6d2c8b3ca2e0c75a2854f3ff2c1fd62d"),
		new("item", "antiquity_sheer_linen_overshirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4591621ce682005d88e397de21e9e719f1ece02f50086e3b921f24c8ab476a5d"),
		new("item", "antiquity_shield_common_round_hide_shield", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4fca8b43562a5e06809a12263b3986ae6a5a15ec854e9aaf88d80787cc58c7d"),
		new("item", "antiquity_shield_punic_round_hide_shield", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "49a1316475b29017b25c6adb2f842c5942fb868cba8ee2298c091c13169de4da"),
	];
}
