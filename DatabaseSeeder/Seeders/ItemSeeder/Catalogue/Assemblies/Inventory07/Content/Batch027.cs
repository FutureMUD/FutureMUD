// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("craft", "Writing and administration/bind fitted palm-leaf document chest", "crafts", ["renaissance"], ["item:renaissance_trade_southeast_asian_port_palm_leaf_document_chest"], ItemSeederOwnershipPolicy.StockAggregate, "b07e2fb3570ebb4ffe272d96c3abfde31571888261a27a79c56463557f1013db"),
		new("craft", "Writing and administration/bind folded civic warrant", "crafts", ["renaissance"], ["item:renaissance_writing_civic_warrant"], ItemSeederOwnershipPolicy.StockAggregate, "00e12445a168504d5461b8ac6c4b9c00b4dcc97f1e0483ed28763cfe3e83c041"),
		new("craft", "Writing and administration/bind plain-framed wood writing table", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_writing_table_01"], ItemSeederOwnershipPolicy.StockAggregate, "809597042369a8073d6049b94f64b7c824fbe626e14b93a04dd25b22729cb63b"),
		new("craft", "Writing and administration/bind rattan medicine-packet portable writing box", "crafts", ["renaissance"], ["item:renaissance_container_eal_personal_portable_writing_box_02"], ItemSeederOwnershipPolicy.StockAggregate, "534431506bf36bcaa9702bd780c731bb5d89d67585b92f4c970a70591b44904d"),
		new("craft", "Writing and administration/bind tooled leather document satchel", "crafts", ["renaissance"], ["item:renaissance_personal_ottoman_bazaar_document_satchel"], ItemSeederOwnershipPolicy.StockAggregate, "fe49b3c62a99b7f00b054b0d3db2e4e1bf013567fc2b7f2b87393be778f9ff5f"),
		new("item", "adjacent_antiquity_curved_tebenna", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "edad496ef01e508e5efc4d7ea9d407d1cdc2238c823c9dbfa489354d34e52653"),
		new("item", "adjacent_antiquity_fitted_linen_gown", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a94f3bd5b352a6e679bc2e8e1f0c724de4dd85f779ed30a696cf4aad4c10fe4"),
		new("item", "adjacent_antiquity_linen_head_mantle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "298f9a3c0699951d166353fa253734a42201ba4240b922a8711546d672e6c393"),
		new("item", "antiquity_accessory_scythian_gorytos", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9613d1a6fb024839c2a83d2846ba5816f358aa56bda7319a84d723dacf447982"),
		new("item", "antiquity_ammo_common_field_point_arrow", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc15016fbcd9c17ea278c7eec5fee4a25dece76804087a991b2a60c9c126f0bd"),
		new("item", "antiquity_anatolian_forward_curved_bronze_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "66d9c261af049250eaf25ba451373e80ebe802a5090aa5655a3c71a3790f512d"),
		new("item", "antiquity_anatolian_leather_cavalry_boots", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "15e4a9a9d06d5116803fe13d9f3e02940568406bbbfa0ae14f9419a7ca101b7f"),
		new("item", "antiquity_bathhouse_plunge_pool", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "96a3366426d78fc8ebeabcb25770f6f955aaa8961f4a9ed66465019c7ba63372"),
		new("item", "antiquity_bead_door_hanging", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c88b455f187f43e1e4d27150d522a9e1dfb58fd86d2f6258cb71e5a32028eb84"),
		new("item", "antiquity_broad_wool_mantle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfd58c57ebf64ee75415bb5d5c67687ee94707417bbfa06b843512b252ab977f"),
		new("item", "antiquity_bronze_awl_punch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "82b9e86876e54ff1813f9716bb4d0f43943e20f5444cee309637b2f481fa7845"),
		new("item", "antiquity_bronze_hand_saw", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f2bb8eef2a72a4176c11b5bf77f7ea452fe1af7d638b5b5d57647c2091a3cc6"),
		new("item", "antiquity_bronze_oil_lamp", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f5ba69f096696f02fa37d233ff6ab3be7592609312e624bfcadce34e094af0d"),
		new("item", "antiquity_bronze_shrine_grille", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e0aa2dc524d0327c46f40495f27d33ac460944cc995e88b238be3a6a0986a0e"),
		new("item", "antiquity_bronze_signet_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "006fbddae2e507b82c0eca80c64e6c449c07b57f0e9522246370593236ae91b4"),
		new("item", "antiquity_bronze_tripod_stand", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7e078173288115103f91e1a7fb33a90c65fe418004f91f6995317dd5614a1b2"),
		new("item", "antiquity_bronze_warded_key", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a6d342cbf8dfa23748b53de2162934a8a03989b38aea85b1662c8dac56da2bf"),
		new("item", "antiquity_city_gate_leaf", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "78f811a21cc15e899a4651776cc333610aad00b0516a01d92903518682fbf113"),
		new("item", "antiquity_clay_loom_weight", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "752d933c355d7ceba234510afe01eada0b9a00219283b8362d9dbb529a229448"),
		new("item", "antiquity_coopers_adze", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a9f68bc5c845ee772f17b12d4e263e4c5a834947c290771b8592c029d56200a"),
		new("item", "antiquity_etruscan_round_bronze_pectoral", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "86fb54f5711d696f373fba2a2c19967608ce69f211ae748d2a52118614831de1"),
		new("item", "antiquity_fine_linen_shoulder_shawl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "44377a40ebfc8659af1757618a4cf7525df10a3fb7ed38e3cfef7c86744328f2"),
		new("item", "antiquity_fine_linen_tunica", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fa2b73d6c5929460a821c9761e46649b5ecde344a677c72b8f832adc9284f33"),
		new("item", "antiquity_fine_steel_lockpick_case", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4573df81f551f059d03033b6ebde78120a86e33f00f527a4ca08d44725e5d534"),
		new("item", "antiquity_folding_curule_stool", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d0891c80f751a19402ea87a08d717b233e01ac9ab1bc0ab92c8010911a669e7"),
		new("item", "antiquity_food_anatolian_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac4413f4a766a01bb8975a9b96cb0b7bcc3d0b28a43828cd92f0ec660e9fb854"),
		new("item", "antiquity_food_celtic_honeyed_pastry", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "60daa1142379462a074d8e45a45365fa456b633432f1e2cf1619a9a4889742ef"),
		new("item", "antiquity_food_celtic_oilseed_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c4495127a39d91ea3c5b6b9e928edae8924303fab84a137300fbe1d05abae95"),
		new("item", "antiquity_food_celtic_sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4aa531337d40c8f111b1572a4a577978f09429a83d6ee36a62949a59380d6028"),
		new("item", "antiquity_food_drying_rack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "72b59f966ad144be237112929833883a0a6a4eca0c4c70aea1fdfbf11ae0aa38"),
		new("item", "antiquity_food_egyptian_fruit_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "59ea8782ffd74909e2fe5912320fb7131ef3713286689b67f3af9fbf4e0fe6fb"),
		new("item", "antiquity_food_etruscan_pulse_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e50a658d1ed257d5734cb81e8e3174acc7cfb558235c44730a8a4e09f5274bbd"),
		new("item", "antiquity_food_fermenting_beer_amphora", "antiquity", ["antiquity"], ["item:antiquity_food_finished_beer_amphora"], ItemSeederOwnershipPolicy.RequiredRelationship, "33a74cccf5929f311662b0a7dd1461161c01d1e08225fcf6126f03ee853232db"),
		new("item", "antiquity_food_hellenic_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "039165d536dc982aa1081c1e90ab6013650db88abb8dbce6dc5ba941f333c861"),
		new("item", "antiquity_food_kushite_pulse_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "04dbfe2ec98d7e25a47ae47b198ec1ef0e23572846a5e88536de17df5ae69a3e"),
	];
}
