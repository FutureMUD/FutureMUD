// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch033() =>
	[
		new("item", "earlymodern_furniture_ibe_wardrobe_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "906a374992d938a97634e88b4820820f664ddbf2840aab8c59a4c6ab1a9921bc"),
		new("item", "earlymodern_furniture_ina_dining_table_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bdcf39415af99dafeaf1740eeeac0970bbbf21d1999b7b4c2c8294b952da2652"),
		new("item", "earlymodern_furniture_ina_wardrobe_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "61aa580219b5dc547812a6e22a90cfe91aa020d18e51e1c9e884794a136f7a37"),
		new("item", "earlymodern_furniture_ita_linen_press_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebf79c5c03d59909f71bc654eb4eb1bec8fc4fc56849649a60acd72687a6da1f"),
		new("item", "earlymodern_furniture_lmg_writing_desk_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3de68b0abdb2a85f6ac5dbee7784f81d580220bc5c1d7e824fa8c15ac96393e"),
		new("item", "earlymodern_furniture_mag_wardrobe_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7a870f718932f448f51c3210a14595cfa9373e265e56df7164069221a972ce1"),
		new("item", "earlymodern_furniture_mag_weapon_rack_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9af7860ccc953693f64078d734b897ede2efffe9f7686aae79758282eccf6d43"),
		new("item", "earlymodern_furniture_mes_dining_table_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "719ba48c0f7c774d20e154b3f82f35c1f28dd3d8d641948b8388e15482cdc27d"),
		new("item", "earlymodern_furniture_mes_wardrobe_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b22dec9f007fd7d2fa4a4ec38275384939963e000c9b11b70cceb8abd88f1d4"),
		new("item", "earlymodern_furniture_ott_display_plinth_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4441ff2915b6968fad7aec5583b5fa106cf8dffbadea7afa9057182e86949334"),
		new("item", "earlymodern_furniture_plh_armoire_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "406faa37396fd77e533ea9be47a355bbc7083e11635fbb182c87248ea96abe14"),
		new("item", "earlymodern_furniture_rus_counting_desk_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fac0cbd9f175033b974e9a9789bd139d6dce410bf923afbf68da08df92d91c37"),
		new("item", "earlymodern_furniture_ryu_weapon_rack_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "937b623af571df64050c7f7bf1be1d7c3909f03cd3ce3ee8309e447acef20270"),
		new("item", "earlymodern_furniture_saf_linen_press_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d45cf106a7f3622e18ad1392a97fa51748a8a3e0bf8178c69150f829245e71ba"),
		new("item", "earlymodern_furniture_sah_counting_desk_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1de6cd9b45bfdc50e727c5d4ded9012810818b0cd136cb550e08f613b8be4e31"),
		new("item", "earlymodern_furniture_sca_linen_press_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "46a830944cd9c5f46c9786a7ee765634a3253d2b3e2c8a0e56ec8f29c6d7354a"),
		new("item", "earlymodern_furniture_sea_counting_desk_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e487181b731b6f84fb436f763a64f3c4e0a6900a0d5b9009f8df2d5f2030370d"),
		new("item", "earlymodern_furniture_spa_archive_wall_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f62f0ad3ff08074c05c5feb94c9f97c7f566d6976a79480f298c9f304d8fda1"),
		new("item", "earlymodern_furniture_spa_armoire_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "efa15685f881e1f37481cd970608a3ebe1ba3a59559909e38bdb330b59cfb174"),
		new("item", "earlymodern_furniture_swa_bench_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbfc98eb560b4e96537d7913ae470c0e9c0f577002d1c951aaf6a4022ad56b3e"),
		new("item", "earlymodern_furniture_waf_writing_desk_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "242daa0a56048a3cd9d01a863b9351eec0641ca3cd42c99d0335278dfa14c964"),
		new("item", "earlymodern_german_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "88d043c9d324b3bf0bc3ea5a0020452a9ca7cb20a61b9440aabacbf8c702da20"),
		new("item", "earlymodern_globalmaritime_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4a0b05daf0ab3b2946a168925f1dcf3e9c05ba64507afe67ce9e8204bf661b4"),
		new("item", "earlymodern_headwear_edo_deep_travel_kasa", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "25593fa341d5d4bfae167a2380fd46eb114bb4e0718e1766458e2efecc7e2bde"),
		new("item", "earlymodern_headwear_ottoman_quilted_house_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aee8fc6a8d81564433740cf6206d02a91cb42e43b5e2b7f8880e7b11146fb4ba"),
		new("item", "earlymodern_headwear_padded_naval_watchhood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "399feb98824fc6f69cc5aadaec57f691d6d2744a913ba891e175f30d04183e74"),
		new("item", "earlymodern_headwear_qing_winter_court_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "138a32ad0a5d5d6abb24ec01e944f4beea78b2200152c7446be14dcf06b33eba"),
		new("item", "earlymodern_household_punch_bowl", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2550214d4a37e9bf498082286296b63c28bfbf706cd0111d6e0fb98864ebbe20"),
		new("item", "earlymodern_household_teapot", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "854322b0c9a1098d0d6e39c1e3ee398abed9ceaec1c783b2d71b520b91d44698"),
		new("item", "earlymodern_jewellery_and_brooch_670", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2704bbd707aead542a98589e73c9d8e712911e0e7a49a1eb72875c7d19ca4ed7"),
		new("item", "earlymodern_jewellery_and_torc_679", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e32c491399a4cab3a04c69e3c6dbd0d90e60105c3ed997c55e14b467a9a67d8b"),
		new("item", "earlymodern_jewellery_bra_pendant_583", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "29562e0a5398837be4985bc243aaa34139ccd97eccb076d10b2c761e1989b424"),
		new("item", "earlymodern_jewellery_car_badge_697", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a0cf80358a1d2b234f120afef372621a6aecf080f403439819b538e071786fb"),
		new("item", "earlymodern_jewellery_edo_armlet_367", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "67f01c209651a28c9dd2909f572877a5723f9217b087ec4176725d67032e2659"),
		new("item", "earlymodern_jewellery_edo_bracelet_366", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f204d0f7225dd77fb01d9a3d121aa7b71c0a34cf75fe61f39cff916bba2fbf48"),
		new("item", "earlymodern_jewellery_ena_pendant_603", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "070d305c78e0fbf3da324465085e22a7efbc1a7832220fc5615279fae1a4852b"),
		new("item", "earlymodern_jewellery_eth_pin_531", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "42b5f312cd3d959915d06d2634e18dff130b61e7fb2301ef61c02540572fe07f"),
		new("item", "earlymodern_jewellery_eth_signet_ring_522", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "83f2594c03b64580784d1f4ce19d31a7783f3069f40a42a23d6beea58cd7180c"),
		new("item", "earlymodern_jewellery_fr_anklet_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ce9cb3a0cd327e88e60bd63cd2c9f4f225d02e6e363b85eec801ab404a26483"),
		new("item", "earlymodern_jewellery_fr_bead_necklace_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ece78a797dfdbd07d04509a4835d80fca00c802e1e7439364dffff8ffa041f40"),
	];
}
