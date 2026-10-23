// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch060() =>
	[
		new("item", "renaissance_furniture_rse_corner_table_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e11a2b7911be0b85df48b515f4403442d6fb235383438daf0d7d56aa42cfaa2"),
		new("item", "renaissance_furniture_rse_urn_stand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a3a540719915f344360fa16453f3cee80a09fdf2373b6673556f6911f9f57ce"),
		new("item", "renaissance_furniture_sai_household_cupboard_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53a4fb42376707443a92afe2742a1dbcf7c36bcd86e357d7eaadbc7614d4a150"),
		new("item", "renaissance_furniture_sai_lock_fitted_cupboard_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "453975af8ebfcdc3bdb78d9eee6c815353c93d9061032d769205ce2dd49f8124"),
		new("item", "renaissance_furniture_sas_stepped_display_shelves_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f25e922c58977f43d554740a115b14d32e70ace346533a2baa9c229e01b62d8"),
		new("item", "renaissance_furniture_stp_statue_pedestal_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa85601a729502808d7396b5c95748d46a666b161abe7b0479cd06e07e849b95"),
		new("item", "renaissance_furniture_wer_corner_table_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "017e257cc2bc9a62011a838f48da45f208046f7c9c39b05671f2131ac18ecb0c"),
		new("item", "renaissance_furniture_wer_metal_armour_display_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eef44534fce2764ec9013917114cbadfd41ebcc326e7af607b7c96e36d37ca23"),
		new("item", "renaissance_institution_preaching_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18d7fd298c9f011d6bc22cf6997fd67613f590aa6960db5b4c3c8aff2952ebc8"),
		new("item", "renaissance_jewellery_and_belt_ornament_616", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fbef76e9f5f1434a3b1e54a3f3c7f5018fb33c64278ee2ee2f143297d9541b28"),
		new("item", "renaissance_jewellery_and_pin_611", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c893077774f098dd758cd551658e683a471050e29459a3fea5d27214531d4ae"),
		new("item", "renaissance_jewellery_and_scripture_box_626", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a1eaa7327f9a06ae7d1925f17c5fcf9e4ea258604c84a0e9f4b5339c101e9ba"),
		new("item", "renaissance_jewellery_and_temple_ring_618", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7416a2e9b98564607fc57c384941ae09de281ce9ad2bbd5aa83464afe511f81a"),
		new("item", "renaissance_jewellery_car_badge_647", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "951d4d8105024d29c3d6aec93596143f3676fac5f61732a8278fa7fed6c37616"),
		new("item", "renaissance_jewellery_car_ring_631", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40b3787ef0d003276d5a3ec8d754bfdf1356f559bbe4738a8ed863d7dbd7d5ad"),
		new("item", "renaissance_jewellery_cef_anklet_128", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "73ac79be538f53c40085eb722dfc32eb8f01ecbeba4901babfe8343e1f0ebf80"),
		new("item", "renaissance_jewellery_cef_belt_ornament_136", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "58c3fea1b8bfe6aa892a031c8b514cca48c9d07d18d4f72f5bcb9ea51ac5a68e"),
		new("item", "renaissance_jewellery_cef_chain_necklace_125", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ff3c39dbff70f354809ec7388554aea3f21a635eea04e2379923fc086e514db"),
		new("item", "renaissance_jewellery_cef_diadem_134", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8d2c162b768d96e35216a48e751237baa9d03300bc73b616aa3f6730b9ae181"),
		new("item", "renaissance_jewellery_cef_pin_131", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a81b25a2e41b6fb0d13d6104c6fe59556faf60ba7fea470e75d2fad376d5b20"),
		new("item", "renaissance_jewellery_cef_scripture_box_146", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c65882aea1db21fa03ff7651f69e1b7118e6a941fd419eb22c7b25d9b1764ff"),
		new("item", "renaissance_jewellery_cef_signet_ring_122", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4fd74728d525dbc26c2eb8c6e37f50acc3bac6a867fcbc4a12d8e779b4ed5908"),
		new("item", "renaissance_jewellery_cef_wreath_140", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a79d8cb34cc2f2618a1717ea69d868b5621866c6cf662fbeb76e2054514f7e1"),
		new("item", "renaissance_jewellery_cen_incense_holder_88", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bc63e014e662d1558d95ca93962fabd9f1b76989da0e90cde3aee7b6339aa46"),
		new("item", "renaissance_jewellery_col_brooch_700", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "adc83abd736cb851e0d054e92e81504248b40a6b3441cd48792741ea064fd65e"),
		new("item", "renaissance_jewellery_col_torc_709", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ac7287552d547268739bad772a692a0e886ee9cf2a7f68e66a110dfbafc7d8e"),
		new("item", "renaissance_jewellery_col_votive_token_715", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ed1fb0b9b8756913c6aa4a69aaf871a9071d91f540f3f51ae227eb68209a69b"),
		new("item", "renaissance_jewellery_common_anklet_158", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aafb3e4cec3db6adee39cee4418dc5f83c096915e63a5d798229ba903c6a5ada"),
		new("item", "renaissance_jewellery_common_armlet_37", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "052467b788587456c87d24e2f2d8a9e14bb44aaa66fbe1c9bbbee855bec2af6e"),
		new("item", "renaissance_jewellery_common_badge_107", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e217e043a7a850e44ef5505697e583d409a43b9f8aa8fd0f137b03d8f1f06ce"),
		new("item", "renaissance_jewellery_common_belt_ornament_16", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe2c7d9e04f4d6559d32ded031961b5d88d5f95757766d07cf84a2c8e0225831"),
		new("item", "renaissance_jewellery_common_bracelet_126", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9201a3cbfe2eac96f053b4e8df906da6ad84aa0eb4aa608f3bd920c73c9943c7"),
		new("item", "renaissance_jewellery_common_brooch_40", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9a3ae66f9ba55f4b0a9a5383b4bcdf1812bc75017e5b04774ccdf3641cb19bc"),
		new("item", "renaissance_jewellery_common_devotional_medal_202", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5817dd94e33a558f00fbb52c8e0548029d97b914dec9e8013cd707e5e8101c8"),
		new("item", "renaissance_jewellery_common_diadem_104", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "039e7eb1504d71ad6352ffb591f276b7069040c04f230b9fa8ccec2bba13312d"),
		new("item", "renaissance_jewellery_common_earrings_219", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7dfb4f5050dd0b73381d2dfa95e747c238e7fe43374f6f392bb667a30025798"),
		new("item", "renaissance_jewellery_common_earrings_99", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "819fe6d97abe3559d340ab9cdaa3b11b565b60d0124c11bccc5005ff3bf234e8"),
		new("item", "renaissance_jewellery_common_pendant_153", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4865a067ba0633c412d93aadf687b19593beff56db7c15c1bfea9fd0afe060c"),
		new("item", "renaissance_jewellery_common_pendant_63", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0fc95e0a71b6a17dadbaa632c7029e93f97398b03173804ef16a59edd3581de7"),
		new("item", "renaissance_jewellery_common_pin_161", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "226b040fee799f2db3cfe7e0af2d9930e31328739dd9942a59c4a626fc67a86b"),
	];
}
