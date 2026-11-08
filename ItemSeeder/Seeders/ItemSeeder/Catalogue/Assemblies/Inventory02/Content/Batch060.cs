// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch060() =>
	[
		new("item", "renaissance_door_stp_barrier_404", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b54f3f3ce1bc933a69c37b34c4956855d8fec10580bbd359835ad095cdc8c43"),
		new("item", "renaissance_door_stp_lattice_gate_399", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "882267bd08a2322923d4256fd8b629dd79403f53e5a76749f5dc65445ca9bfa1"),
		new("item", "renaissance_door_stp_lockable_door_395", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e581561aa3b8f87a2e55e9f4c767fdc5170c4f277d7801b458884af50cb73247"),
		new("item", "renaissance_door_stp_strike_plate_419", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6094158ab13b976f16f5d89be3aeb125de74a8e6a1354bff60440b8c563ed0dd"),
		new("item", "renaissance_door_wer_hook_latch_26", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0aed112f541d159a52ffe99a9f210ad57def08304b44436f6ecde738a985492"),
		new("item", "renaissance_door_wer_screen_06", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d72f575221d36698dd3d19af34596ca35f7af98fe410f65555e8eb5672a33dc"),
		new("item", "renaissance_drug_antimony_wine_bottle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd5cd78a0b326b3f20eef42ab52e6a175bda5c4540554be7b78c036cba5fbb11"),
		new("item", "renaissance_drug_opium_tincture_bottle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a584d92b4cc0faf11385c54a25165405e6f4b0bfeeb361342a5cb7bec6dfad2"),
		new("item", "renaissance_drug_sarsaparilla_syrup_bottle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac54dd303af443573e7e4447e7d71ae245026ef109c353700273adede2f2c53c"),
		new("item", "renaissance_food_andean_quinoa_like_grain_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "808c567ee178c908e14c9e35a2b62e6ff037fa4f1fc9a901566117ed95fefd8c"),
		new("item", "renaissance_food_aztec_market_insect_seed_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5fd7e17cf440737a6c12daaeedc8f5f312781069477ad9c7252c397de0eb850"),
		new("item", "renaissance_food_clarifying_meat_jelly_bag", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ccbac6c555fe4ef619ffbe1fb633864b53544670f6969aeb289618b45cbf55bc"),
		new("item", "renaissance_food_italian_vermicelli_almond_fast", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d78f897b783d4bf9b668d982516859a6d9fb626d124b0a0de1410442f879c9e8"),
		new("item", "renaissance_food_japanese_miso_pickled_tofu", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce65339fc402746083d3563f26bacaf1f969c0b499ec1c5933441526d56d5073"),
		new("item", "renaissance_food_joseon_new_chile_preserve", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af1469e1a485cd58b860219d1c9b81ae17ec1ef428f47e3625eb7e956a6e959c"),
		new("item", "renaissance_food_mande_ground_seed_leaf_sauce", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f54ea6209b35d097dcefbb698d0ebab7f1403c47de9460329451bd9daebd0bef"),
		new("item", "renaissance_food_maya_achiote_turkey_pit_packet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e511df3d8cfd2b46585a352efe6ac793ec919960ac94f4e56754f90a5b7aa4c1"),
		new("item", "renaissance_food_ming_dragon_whisker_sweet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "983cd9b5612eb60df339da61176fc14a948f2255386ef94e6914c243c6af327e"),
		new("item", "renaissance_food_ming_sweet_potato_ration", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb76789f0fadc41f3a314d930f0fe1f6b914cb39d9afe440e322d7941fb64be1"),
		new("item", "renaissance_food_nixtamal_wet_maize_dough", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab2fe196f23abd2ed469c52a6cc75bbb097fc4787fb7acee1ffbaf7a44703509"),
		new("item", "renaissance_food_ottoman_cabbage_sarma_rolls", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b4024de1d4f8a077080112d22a97f0c94e95ab83eee3cae8d30c5e0331950de"),
		new("item", "renaissance_food_swahili_sugar_almond_pastry", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb6bfb061703244adbf4ef4203ab04e0beda0c20a795c8b422df30d4ebced7b2"),
		new("item", "renaissance_frontier_split_sleeve_noblecoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dafec4eb7c3436cea24ed42b837ec791e93fe3bce5151733ac16768a04f4a33"),
		new("item", "renaissance_furniture_aca_corner_table_01_lockable", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50a05ce6767d67223cd471eb9779cd0740dbb4a02a47802520356138cf9d724a"),
		new("item", "renaissance_furniture_cen_side_table_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8687f57f546c83e323b6c562b080734ca002d43686412fc0245e593e42d0dc8c"),
		new("item", "renaissance_furniture_col_garment_armoire_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "836b6d83adc2f197e571069d095934d745a6227cc69549266d13c156bb48017f"),
		new("item", "renaissance_furniture_col_lock_fitted_wardrobe_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6f279a62268dbc83a8fefe5ce515f18b9f0763d5d3eec4cdd7a535e315109c0"),
		new("item", "renaissance_furniture_col_low_drawer_chest_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "262080056def31bb4a3df7d4a9299dada4a5deb72be117d116e535785f81ae48"),
		new("item", "renaissance_furniture_eal_hanging_cabinet_01_lockable", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b73a5d79b545d982b5460167b57c6748020125e9e40fb5e14e7011ef2675c19"),
		new("item", "renaissance_furniture_eal_statue_pedestal_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f0202fb82b9111dd465b9d9719c0aae203a6ddca24f9b90e719345b01c68147"),
		new("item", "renaissance_furniture_iba_campaign_armour_tree_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8216653ec1787cfe85298080233421b2199c25c98c02b8323f4c01ed2507096"),
		new("item", "renaissance_furniture_iba_helmet_cuirass_stand_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7247ae49db9df3828e165d1f774eb4958ec3584dabc8d073978352cf7c070f39"),
		new("item", "renaissance_furniture_iba_map_drawers_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b27081b817ad9d80eaa77f9af89db0938d8932aca59d2da126c2303374fd9c80"),
		new("item", "renaissance_furniture_ind_map_desk_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "19d3b5ab1724752468277f958ecb00695d5ccfd1f0f6ea8d2813379f0f3ecb27"),
		new("item", "renaissance_furniture_japanese_ryukyuan_document_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e1a0691ba7280d2521521c6f6bd487ac3ee5337cd9881919e32476394b687e0"),
		new("item", "renaissance_furniture_japanese_ryukyuan_folding_serving_table", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3cf8f705ca1cca3c2fb182274f24e1935f1dd2494fac4c297f0e5b81bdd2c33"),
		new("item", "renaissance_furniture_jpn_urn_stand_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "707867589cce371a625177d9abd3dd472deb2149b5cd427d9b7d703a8fcb2961"),
		new("item", "renaissance_furniture_mar_bow_rack_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e2df2cc2d0521effcc43d09e8367cf2356ca2db147a81fd47a90c01347b1f74"),
		new("item", "renaissance_furniture_mar_nightstand_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "27ea5a043e9445efb12f5bba20a384b64e7795910294aadac3b76651d6afbcc4"),
		new("item", "renaissance_furniture_mar_plate_hutch_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "268aeefdf932b032495e9885440dba9c2baaf52cd196eb73555a7cdd137ae480"),
	];
}
