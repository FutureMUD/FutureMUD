// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch051() =>
	[
		new("item", "medieval_food_royal_filled_porcupine_meatball", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1a23c88e683e7997b13a4a2d9da332f195e0dad0db75179ecde9e1af88a3976"),
		new("item", "medieval_food_saffron_cheese_tart", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e0759e617f598cc1b9613554ed731f9aecd8ba200d7d1ccfdaacc0ad8498e91"),
		new("item", "medieval_food_song_braised_gluten_mock_meat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d17a390b1532ecf4471223f5ac84c8c42f02ff62ce41ea5accd9cbd0aeec760"),
		new("item", "medieval_food_sour_tamarind_rice_offering", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eeb21e046673ccda2f10aafd436cfe62195e9dd3d7d28d668c41a436a951ed4b"),
		new("item", "medieval_food_swahili_coconut_like_fish_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c19a813d5a737ecc0c870a60c60a0953737f843a2cc3e2d53c103ef15b2efcae"),
		new("item", "medieval_furniture_backless_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "868b96a36cf4babe476262099e43298b73cdd15472859e7e3a42e1cc34d493f3"),
		new("item", "medieval_furniture_carved_armchair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "80259c32fad94732fe6c8ce5f8b36acfff51f98dc7609265ea9a7782d9befc91"),
		new("item", "medieval_furniture_clerk_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f330e2dd951346217ee0b71363ebc3f73c850d46a181d028c2230fd2f9b237d3"),
		new("item", "medieval_furniture_copyists_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "64fd3eb227aaa3ea5273ac606d258b357b9545cf2c67d39bb3c9c8210148ed76"),
		new("item", "medieval_furniture_islamicate_divan_platform", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "318b0870856b5d2b675adeae81811a5cc933ba311d9a08e15d0c2eb812d7dc70"),
		new("item", "medieval_furniture_low_wooden_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9284395d40223006faed560a05d1e85a0708ac584da20f497cb5abe5feb28087"),
		new("item", "medieval_furniture_northern_long_fire_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "83d1485efd7f1c5102d54a8a8f1bce445ee204f36adde8ea2fd3f15a18ac65e5"),
		new("item", "medieval_furniture_northern_narrow_bunk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a10fc49230524e005f027323c836f1dd5a637919b76516ff115418bd1de73df7"),
		new("item", "medieval_furniture_northern_trestle_feast_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "28449d738402b54143f4eeea75a9dba22ff03a633db207adef2d8ddcd1d8c4c8"),
		new("item", "medieval_furniture_padded_floor_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dcfc8bdd30dd0696424c03b9626ac37a6d4f31e0b8a2d82b42d58b60d4ccc903"),
		new("item", "medieval_furniture_plain_wooden_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "90ef574743188ae4145a5eb5ac5113c30806c97ac2312c83f7972782855f6d46"),
		new("item", "medieval_furniture_simple_hall_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "98d290881df8123c0144650d1ce812caa52238bb07ebc96dde00446faddf1bef"),
		new("item", "medieval_furniture_simple_sideboard_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "26c90a83a3feb218ac533a8e7cbacdd7a8bc58e0c528d12a3ab4fcda7ba955cb"),
		new("item", "medieval_furniture_steppe_wagon_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "95ef9724511aebe54215d11c6d5ca070fafa09ce9de608bee2c732f1884d973c"),
		new("item", "medieval_furniture_tally_table_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4cfb28cf10cffb279094491f259f7591c7e6b007696deebb522567eaa99506c"),
		new("item", "medieval_goryeo_baji", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ffd0754796b183c1518e88be26505817b5070e3bdcc82a03ee2bd8687a2d76d"),
		new("item", "medieval_goryeo_po_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "17e3f412ce8d308917047f0fd46b87116be86b198be5cd0eeb31934e8452dd78"),
		new("item", "medieval_goryeo_shop_po", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "56c04c44d7800d90a93cf83853efc9df4f2f0ec135ca7deb0919b6ca41392b2c"),
		new("item", "medieval_heat_bedchamber_ember_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c7e21384bf6520cbb57eeffb38ef83cc17a588533dd2d91e8f023d4bdc1f7a4"),
		new("item", "medieval_heian_kamakura_japan_cypress_document_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "75607576389e3ee513650f7e0951b46aab590d344a0525fea5a09867255d228d"),
		new("item", "medieval_high_leather_boots", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2af3d1a03c7d4ce6cce8c2ae20115c543e6505f959ea20b1e58b044c4d1ba22"),
		new("item", "medieval_hindu_multi_wick_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd35da777684d1522748c5dc447988b6301a913bdf2e029749a76b0c3b4e1dea"),
		new("item", "medieval_hindu_puja_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dca58ef500a1fb45145e6378d1a091e31950004a2e8de2d73fad564db315e749"),
		new("item", "medieval_industry_stock_armour_ring_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "291f61130d42b63a841be2de5a0a4c7f6e5386a2767dac61fe1ae0d7b7fdc47b"),
		new("item", "medieval_islamic_reed_mat_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d18b5cde692ff58b071e9f867586bddd7791b41ec6c2fc75169797d996d00ec3"),
		new("item", "medieval_jewellery_badge_gilded_bronze_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7459a8196c344c99f83fe24627974b0b2198af6c42860dc2f6ca96428a8aa1c6"),
		new("item", "medieval_jewellery_brooch_brass_ring_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "537b58b929b7af6eddaae461b16f593a407f5461816fe2e663ae3b0b7e00c4c8"),
		new("item", "medieval_jewellery_brooch_glass_inlaid_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "56ab49fb7b24035cfd55f0b2442b8e41be8628e6d72eeee995b4c61cd6c84279"),
		new("item", "medieval_jewellery_brooch_rock_crystal_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b301cd51a5d0e65512a3b205cd16e5822149b4076943ca6ce964b750e0b269a3"),
		new("item", "medieval_jewellery_common_cowrie_shell_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "08851d2ce9252242d930ec21fc23d5d7e53e17b23911b592d0ec77186bfd1336"),
		new("item", "medieval_jewellery_common_leather_token_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f76ef0b902a26683ca16fa35c4fc2dc986000c598c79083ee2010ba60082bf6"),
		new("item", "medieval_jewellery_garland_dried_laurel_wreath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d9258b0dbdbcf95a502053dbe5a64d0b709f2c7995c3c113797bee807010f59"),
		new("item", "medieval_jewellery_head_silver_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "93ea9513a50ed240f3599092c436af689bc2a0e0503f2513d1d55cb3dfdaf15b"),
		new("item", "medieval_jewellery_head_small_gold_coronet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "87775634ce36eb469204b535dc529e512900b7d3a93a808c193cbdb0c820664a"),
		new("item", "medieval_jewellery_limb_paired_glass_bangle_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "57b8476094063f612f4a0a15454bc724ddf2092bb1fb9c677e8da3689e6fd14a"),
	];
}
