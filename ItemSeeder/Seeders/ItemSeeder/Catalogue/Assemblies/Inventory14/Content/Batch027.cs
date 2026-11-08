// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory14Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("craft", "Writing and Administration/make sutra printing block", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_sutra_printing_block"], ItemSeederOwnershipPolicy.StockAggregate, "329da646dd7b5d11b639ae62ea05a06491a31e9546f0ecf765809220e20be0da"),
		new("craft", "Writing and Administration/make watermark wire", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_watermark_wire"], ItemSeederOwnershipPolicy.StockAggregate, "aeb028d9d3e6819b8ef7ea77d9e5b47e6a72c6ae4bf6d77ac93a13990ec45b75"),
		new("craft", "Writing and administration/bind chestnut cacao-sample portable writing box", "crafts", ["renaissance"], ["item:renaissance_container_iba_personal_portable_writing_box_03"], ItemSeederOwnershipPolicy.StockAggregate, "5a38972d5ec5ebc7e6154942416073ffd7ebb29256d2374d0ae89634729628b4"),
		new("craft", "Writing and administration/bind round-fronted woven-panel wood writing table", "crafts", ["renaissance"], ["item:renaissance_furniture_and_writing_table_03"], ItemSeederOwnershipPolicy.StockAggregate, "6b7418fbf0c3c6519410d31b63f2be11e0b0085626d971a437e18aebfb809d70"),
		new("craft", "Writing and administration/bind slender iron-bound oak writing desk", "crafts", ["renaissance"], ["item:renaissance_furniture_eon_writing_desk_04"], ItemSeederOwnershipPolicy.StockAggregate, "0f5fdc5e767b1479460ed601935690f4a8c917e6b14037fe7af4d39b4f1ec936"),
		new("item", "adjacent_antiquity_felt_boot_liners", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c48a0e14c4fb5c79f7ff1d3a2fa7a48400210e5cf0490b9b09f2ae074ca6bdd"),
		new("item", "adjacent_antiquity_fine_curved_tebenna", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2942102bcec37da4fbd3de7a3b63ee6e811e784a961191660f29bcab9d486996"),
		new("item", "adjacent_antiquity_rectangular_shoulder_cloak", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "73ace2c922d3b8dcd5550bae08c4e128ee9326b96e3eccc58c957ddcb9a4fd12"),
		new("item", "adjacent_antiquity_short_sleeved_linen_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "90b9b90eec5fa9ffe437b0473a6289acdd5491bbdb99bab2cba530662ca8842f"),
		new("item", "adjacent_antiquity_soft_riding_boots", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "39150ac87212fd7e5424319e94dd082fb02a7cc21cfee50116d99c564abe9fcb"),
		new("item", "adjacent_antiquity_wool_neck_wrap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "14eb91ae0988b02276874f963867e0e04b32de610dce4ab973d64274adfe1afa"),
		new("item", "antiquity_accessory_kushite_tall_arrow_quiver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a2fd7ffbebc199260c1a6f6813945d974ed7e7924bdf165e00b5ba39fb02796"),
		new("item", "antiquity_accessory_persian_arrow_pouch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa2d0f9d1fd8db709e460e11fe982542e552204350081f64c19948e7418efb50"),
		new("item", "antiquity_accessory_persian_bowcase_strap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e70b60e734172850736998a8bc629f1cc3318b404930bd54587474be56af8d9d"),
		new("item", "antiquity_ammo_egyptian_reed_arrow", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "776d71fe22098611ce0198ffa836413f67d6f1cb070cd9167c0c9728ca3d39c9"),
		new("item", "antiquity_ammo_kushite_barbed_arrow", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5471840a8aed2c7804b96cec52cbf9f9b13a64ce342de4ef2a9afbfbebccb05a"),
		new("item", "antiquity_anatolian_bronze_chest_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "429ea0d88300d414211ba64f48a5df50cb05ec28a514a0acc51f23ce4a5ced7c"),
		new("item", "antiquity_ankle_leather_boots", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca0a136d44ed9933e8ae830f79633afe48a0ece06d44693b0cb9814b813587a2"),
		new("item", "antiquity_bronze_buckled_leather_belt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8ac1fc6888e2ce616d8e253cdbd503761c1a894dd78ad0c33985313895731c0"),
		new("item", "antiquity_bronze_footed_table", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c6ecff5c07d9bbafb1c3d368b7104e0ef09e4227c8204f412f9c0efb9a22fbe"),
		new("item", "antiquity_bronze_shrine_latch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c96c4f0ceac297f4705894cdf8ecd0c27025d092cf0f52b15e958e0c9a789cff"),
		new("item", "antiquity_bung_borer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "22faff79b77ec12ad7679eea2d6a9350af9bccd30f455838d88de52b8a5d1d52"),
		new("item", "antiquity_camel_cargo_saddle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2c6e92dc29fec89bd5e4ede6d094467d836b171f0c0c9d684be85027d0032d5"),
		new("item", "antiquity_carved_prosthetic_hand", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "35a95401fa7e7308580a9310a74b7be85e1a832d7413ec1bce255bd36d2a0ca8"),
		new("item", "antiquity_chariot_harness", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "45d8f8a4ac0f95d960657bc8811d4cfda3092c77c73a93d026fddd9b7a69c42e"),
		new("item", "antiquity_coopers_croze", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e3304adb610b0c0b38ffdb0dc87694d37d96576efc7844fcc42471985c7ad3a"),
		new("item", "antiquity_ebony_inlaid_casket", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ea77a06183a956b674b6e49a85a71317596d0428466bfe07c7f4ded3b6f76f3"),
		new("item", "antiquity_fine_banded_wool_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6cd1638d86122e8e246ce2dbd913edcb2a1c2d9db836bf19d62650095b02e4e"),
		new("item", "antiquity_fine_leather_sandals", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "54ab047a2dabe37e52c423b295fe0416511753f7c66c153dff0c67a2a9eb4e14"),
		new("item", "antiquity_fine_linen_chiton", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c5502c365776daff369bd6fa9f8dc5db8256d2a1e76d557c2eb48e3c02e2f49"),
		new("item", "antiquity_flat_wooden_carrying_tray", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d66dd86b7bc93c7e36ffbc40046ff9bcc8d6ac733eab2ed7ce768becc8170bc"),
		new("item", "antiquity_food_aging_spiced_kumis_amphora", "antiquity", ["antiquity"], ["item:antiquity_food_finished_spiced_kumis_amphora"], ItemSeederOwnershipPolicy.RequiredRelationship, "23072f162a27a2f47ddac0ba2ce631eddc41235bb7680f09f253e757339bcece"),
		new("item", "antiquity_food_anatolian_honeyed_pastry", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "70da86b512a08fe9104db7a14df5cb6655a27f8a05c0f225a5aff085b226cad7"),
		new("item", "antiquity_food_anatolian_oilseed_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e4c12ce857bca851819f7c37fae40ad9be91b9a9534f6fc1277cfc8eebac3d4"),
		new("item", "antiquity_food_butchers_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0530fd7367fcbc601b6d7d4d85c9aeabe9a3e51ae2fc2729ed0fb5d64510de53"),
		new("item", "antiquity_food_celtic_fruit_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aae458044baa2bee17529b49f295eb75d58243e4d9bb992ad2c727146642da53"),
		new("item", "antiquity_food_etruscan_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "635405883871ccd56f39c52896d475163723e686e295352aa31f04d66e35fb7b"),
		new("item", "antiquity_food_kushite_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3115ebe94d333854c6e8e36de8757889de4406a20f9ee64b89e955dec4890566"),
		new("item", "antiquity_food_persian_fruit_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cda498d4123a2c0f565c02c210a546f09d0d1d043d09c3121bf2c51658d0744"),
		new("item", "antiquity_food_pitchfork", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "167e7e2582aee4c989625a2032e752c3fb574c74e5f6af82af54baa187a491fe"),
	];
}
