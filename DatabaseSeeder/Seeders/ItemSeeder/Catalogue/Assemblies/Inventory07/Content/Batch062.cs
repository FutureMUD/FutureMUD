// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch062() =>
	[
		new("item", "renaissance_jewellery_car_devotional_medal_652", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b0717e5d4151c36cf7bbd8f636a8bd80fcea35fdfd7c3cd19055f91e4d834e2"),
		new("item", "renaissance_jewellery_car_diadem_644", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9e4ce5477c75675d64e47016f9284a91af3664f3c861f7f0f5cf85db081585b"),
		new("item", "renaissance_jewellery_car_pendant_633", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4e3a1e8b55324144ab4d3ec6fa961a7b33db67c5bdf8272808f896dd960fe3b"),
		new("item", "renaissance_jewellery_car_pin_641", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "777d449f86a683f182c7f9c4de199946f1c5baa947e8c49e2b60c0e90bb96386"),
		new("item", "renaissance_jewellery_car_portable_panel_659", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7354c429b024efb870dc94934d387825ebc6312f54aacf3206525b6bf89f3512"),
		new("item", "renaissance_jewellery_car_temple_ring_648", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec8546832914cdada6d1eb2a39882224b596b0a1018c4948ec4923de64e9bf44"),
		new("item", "renaissance_jewellery_cef_badge_137", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cd6b75633035e8e0810e0e63a10bd4695860231e208eadc46dc104a311c660e"),
		new("item", "renaissance_jewellery_cen_prayer_beads_81", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6b54c7c2e2cb205b917989cba901c3f5c1636daf085aa856b79c4ac3730ed93"),
		new("item", "renaissance_jewellery_col_devotional_badge_713", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1569342cd1b1e0cc949d0066f476c4ad032271e6b4d10666de2ed0684c2aab53"),
		new("item", "renaissance_jewellery_col_processional_ornament_720", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "65ab9a561a95d15323bbb2f4baa654fc37033e1c1bc481585d9fc2288480a54f"),
		new("item", "renaissance_jewellery_col_ring_691", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dd48a7abae3772e88e74b396da89a8952c9f87c347a8c1dcd6e54a5b478929c"),
		new("item", "renaissance_jewellery_col_signet_ring_692", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1627952c15ead778b20e09f45c9c5342dd107ee34f618fd16fb2efd0c760da69"),
		new("item", "renaissance_jewellery_common_armlet_217", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c55cfdbc9ed31a2943169d3f5dfb8ec98841120381d5068956cb53c8c2dfe92f"),
		new("item", "renaissance_jewellery_common_armlet_97", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd13e415a78e2e11dfcb4a7bec576468aa04e113b856d1a9b3c9902326bdb266"),
		new("item", "renaissance_jewellery_common_belt_ornament_76", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "99387c037903d5779f1864eb697852d91a50feb2410c99d57b2c657209bd657f"),
		new("item", "renaissance_jewellery_common_brooch_220", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a117dcb8df0f15c75c6454a16736b10dd28594b26df8d5d4ef32635e1b4fb82"),
		new("item", "renaissance_jewellery_common_circlet_193", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea4c400b6e7e8dedf3a01035dcc8f18d58ae58388cf01806b0611a46ee72e54c"),
		new("item", "renaissance_jewellery_common_devotional_badge_53", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "815ca7b6e38e211df68f3a83c92f51619d12bca757b5b482d2bc3df492ea28fe"),
		new("item", "renaissance_jewellery_common_diadem_164", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bdfdb918fb09eb7b1502656539e43d6bd57d05c4ef899a75072aa85e2c9824ef"),
		new("item", "renaissance_jewellery_common_earrings_39", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36bd23d51fadaa2c670048e5f038b071a81dbed7d4e3174ba8650a0351cc8bd7"),
		new("item", "renaissance_jewellery_common_ring_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f5f1a68614886b354b174dc4e382c209d7b4c64503e098956a33f13409089a9"),
		new("item", "renaissance_jewellery_eon_devotional_badge_173", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5440dfec936a23d36b40c774d73f4b3bdbb2f7f2b4bc49ca5620bdf5f3382107"),
		new("item", "renaissance_jewellery_ind_brooch_550", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "42c1e94d049c8b4f24aea93dab6f17896e1414cf610e998c149c62eebcdc0e29"),
		new("item", "renaissance_jewellery_ind_earrings_549", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "893eee3c7fd09262d4c33534c14cd63f6be0f2f33b90cb4576927ae581b9f25f"),
		new("item", "renaissance_jewellery_ind_prayer_beads_561", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f77b7731d2c99dd952245d69d8dde38ddf4fe1293d6ec88072b5cd943dced768"),
		new("item", "renaissance_jewellery_jpn_ring_301", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "458537011e376fbec31dae849f7c3208079be8a0f56ec86aca1d2a4ab1c58d4b"),
		new("item", "renaissance_jewellery_mea_ring_331", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "45ff8c2715a3e36a4c339e9aeff9529e9436d922384c6ae3bb7845baa5d6fcc8"),
		new("item", "renaissance_jewellery_mes_anklet_578", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "904600c45e7e3d4f4fe1c343f9809333b79b5ef6e80ef7e8c92ecbf7bfc0773f"),
		new("item", "renaissance_jewellery_mes_pin_581", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d369c6bbc620b5aa23e695904fafc9f816162ec6d2d11ab0473633a953c02b74"),
		new("item", "renaissance_jewellery_mes_temple_ring_588", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "85ed5f350d139f5cd8b1833e59c6bcff8e4ae9210ae833cd4083babd7c46b1cf"),
		new("item", "renaissance_jewellery_mes_wreath_590", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9976b2d2b3f8d3087bda23152d32f060483598240123f14c176a30c2ffaffd2b"),
		new("item", "renaissance_jewellery_msea_belt_ornament_406", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2743ca9b184885eb713aa2b774326c771baa5fdf8b0a2aa1623540dfa8a5eca1"),
		new("item", "renaissance_jewellery_nac_bracelet_666", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "05a42528745f3b7bdd637df8dccdb7b0a1d87c4d11b9550bac0856a668feafea"),
		new("item", "renaissance_jewellery_nba_belt_ornament_106", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5917b833bd00fcd22ffb6cd857a4222de937f3c1b3aaa5198d606f84cb090813"),
		new("item", "renaissance_jewellery_nba_temple_ring_108", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3719ce45809bd4d0ea81760c8fb47b3b78f28e3f6ff2e0588679f977db3d38d8"),
		new("item", "renaissance_jewellery_ott_brooch_190", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b21ab85730cb9f4eaf454c4a1ee5d84b4780f80187cde6b2d8bf7153dc992967"),
		new("item", "renaissance_jewellery_pip_circlet_223", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ecf7714194c6de5e7444b5c8efe7b71bf7480224be86bfb443bd40d6e630a5d"),
		new("item", "renaissance_jewellery_rse_prayer_beads_531", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "72336c1c24005a49da326901cf6c802ce391695b3d0ced88e1e92549b32ae41a"),
		new("item", "renaissance_jewellery_rse_processional_ornament_540", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dc0023ee31544afe69f65916d399aad489e4896064c98f6592d3f0ba1e5d7f5"),
		new("item", "renaissance_jewellery_sai_badge_497", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ff56e8cf94832ee2ea32d5fc48125d7d4d02dfeab878a031c5176b7cc823ca5"),
	];
}
