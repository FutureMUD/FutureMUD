// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch030() =>
	[
		new("item", "earlymodern_furniture_ger_mirror_case_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed921b0fa5f3f7c3195f9e521b376eaa43aa9d5e95cb52de408964cbc32b0e7d"),
		new("item", "earlymodern_furniture_gmt_drawer_chest_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d23613084472ee291eff7237ba48bc1fa73790c49ea5277c57e1e59d1c4f278d"),
		new("item", "earlymodern_furniture_kon_archive_wall_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "414f3f9f0094971ce3c26c342fd1d37ba862435c440240ee2ea17e1b3498b7aa"),
		new("item", "earlymodern_furniture_kon_side_table_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ada600d36b8c52fb0f6434aee55f23609f76adc96429d465170eae04ecd571ac"),
		new("item", "earlymodern_furniture_lmg_drawer_chest_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d935266b117300d07f46daf05d619f2922ed019dc6eb1cbb96afbe4c72184058"),
		new("item", "earlymodern_furniture_mag_armoire_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b1612eb965310da4ef9e48203d42b362dfe92f53b19c8693dafbc8f8e56e59d"),
		new("item", "earlymodern_furniture_mes_linen_press_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f40ddf66f8aedc76bc23a01e468fcbdac449c008900098f7b1d96cd1698c49e"),
		new("item", "earlymodern_furniture_mug_bench_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0190ad8ac1159260713a06ee92b9d483f86edf451ec6451603aaa42a0d9d028"),
		new("item", "earlymodern_furniture_qin_mirror_case_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f78dc1884ef772126f22b16b298b07481364f9e2cc3bb71cac3f699c6d7c255"),
		new("item", "earlymodern_furniture_ryu_writing_desk_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "486afbd5cb1638e8a72697b6438a3c16264ac83f941e6c1039da3bc6a2eddb6b"),
		new("item", "earlymodern_furniture_saf_side_table_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b985f6887b384e1259cbdd9f4faefa236fc6e6c825914a78bcce2e0acc00ffeb"),
		new("item", "earlymodern_furniture_sca_weapon_rack_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d90313e32b826c8127ad2a184067005feb1cad5b257a4f2cf0bc34cc5244e8c2"),
		new("item", "earlymodern_furniture_sea_bookcase_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5129ebe835ad5c66f96802edccab7b516d7ea63ca66ed6eb58756845b54e7ea6"),
		new("item", "earlymodern_furniture_sea_drawer_chest_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aed0077561cc9d3a6aae5c6187fad93395c9c82816d18b12b65d172629d7c679"),
		new("item", "earlymodern_furniture_sic_bench_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f2d08c732e46c39788a94d8e6722ab8303ddab764a9b51609ad947f8403f2f7"),
		new("item", "earlymodern_furniture_spa_bench_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "39d0e32da6d2a7a256b712f8b2ecced156a511bcddd7f79dd7de1323c400e79a"),
		new("item", "earlymodern_german_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "35daabbc57346ff96d604f4f1ad03af3a5134ce38b6852c3a70e1183e2bcb79a"),
		new("item", "earlymodern_headwear_frontier_leather_road_hood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be5267d48293482f11f351638346dad2dc3f2fc403bba70061249a421ac39d92"),
		new("item", "earlymodern_headwear_joseon_narrow_gat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "899c4289a1218b87763a8218eb2ec0d2a5651f042c7632c1ad563b9cb0510702"),
		new("item", "earlymodern_iberian_clothing_broad_crowned_felt_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d126d79e6fb09a11e29eaac6d7958feae8dffc0dad767502c300f82c631d7ba6"),
		new("item", "earlymodern_iberian_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d30e30ace21e2070853177d33b5ea9f7aefd1e4efd5400409e674be7047f4e6"),
		new("item", "earlymodern_indianocean_clothing_light_cotton_head_veil", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "06ed6d2facd94b7953be01d7c493b2b7ba399e6ac88159293ef57197f80ec3eb"),
		new("item", "earlymodern_italian_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "504150610b1b88b96cf04d0cd70a900eca23e62e9ddc8aca0f6180e1e0139c12"),
		new("item", "earlymodern_jewellery_and_circlet_673", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b52b9c4007edafb94cc9b90ee4c251bf524f93406052b55e339e5a7a42ece09"),
		new("item", "earlymodern_jewellery_car_anklet_688", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2610091cfb2722697dd771475306a99c3aef5b01e6971f08e0075c33bc6eb1d2"),
		new("item", "earlymodern_jewellery_car_belt_ornament_696", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dcc46afceba70e4957e7d6b176ee87e487c1cd73926caff041c8204ffde51cb"),
		new("item", "earlymodern_jewellery_car_diadem_694", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9da99564aa7c96375211169096b8002362f1d88afcfff4652e86dd6ac16a01ce"),
		new("item", "earlymodern_jewellery_ena_ring_601", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dce23ad8cbbac080b6fa6c5cfa9d794a77d657dcbac27eae6c89e6570a96e2c1"),
		new("item", "earlymodern_jewellery_eth_chain_necklace_525", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "14644b41e3a1366234168c9136c37cadc880ee63762108a7d84fa652ea5de186"),
		new("item", "earlymodern_jewellery_gb_hairpin_52", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9d57882add7aed0d1fad086e7395809b78e4b2f1fe3d020db69f9138a55f542"),
		new("item", "earlymodern_jewellery_gmt_brooch_710", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "35a69e4f38a8dda621a28a984cac280217449ce8aa3b655dfb5a1db77bde0c9e"),
		new("item", "earlymodern_jewellery_gmt_earrings_709", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f961393f1380c442e4b659432485a889ab6eb2edc4920a4bb27d067cccadf20"),
		new("item", "earlymodern_jewellery_hre_earrings_89", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "19c26d0f3e6c59190cfe6b2e9e16ee459fdb3b429c3018aaebbecf78de36077f"),
		new("item", "earlymodern_jewellery_ib_armlet_67", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "41b84d337f7ffcebbb4163284c9a9f23701954bb2f79c54d444e5caf1ccf8f69"),
		new("item", "earlymodern_jewellery_ib_hairpin_72", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e870289dc75be8d1f6c52434ee6336330bd3e70baceaa4294928bff7f09a108"),
		new("item", "earlymodern_jewellery_it_bead_necklace_104", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4962e50833ce5e74bd71e849b18350369317ee3cbe768fca9de09e8de0c4df33"),
		new("item", "earlymodern_jewellery_it_belt_ornament_116", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e991d813b93ae20606e1643b6aabfcdc1354a52e7c6488e80fc8f1eb12774aa9"),
		new("item", "earlymodern_jewellery_it_pin_111", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "de7f5e54fde8d26bdd7cb612c244c3557801b9517d199422687c2b4a18902fba"),
		new("item", "earlymodern_jewellery_jos_brooch_350", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "be27804b7ae27a869e9c988f324ed5622b1cab4b0782b638468d7321b2acd8ce"),
		new("item", "earlymodern_jewellery_jos_earrings_349", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e40f3c4a5ed5bbb4abae30e026cb3ec476e12c1d969849abb7dddb7eb28f50d"),
	];
}
