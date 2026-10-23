// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_food_mesoamerican_cheese_chile_tamale", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c9882d19044e103931af340c7d9253e40cbd8e39b4c606d3f89ea735a4bad99"),
		new("item", "earlymodern_food_ottoman_asure_palace_garnish", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "006b70f84b98ddf5e5d5f3400dd8ed8721ee21d70696ae4ab2313f0ad504db32"),
		new("item", "earlymodern_food_ottoman_coffee_house_baklava", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "673b6d516d0752125a1d1d22d12f9d33b914f6da1a8139d4ae3165be2f5b263a"),
		new("item", "earlymodern_food_pulled_sugar_decoration_ribbon", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "21700ee8e03007c89da448dc40d6b5969351c9456142906cec5e6451bd4ccae8"),
		new("item", "earlymodern_food_punch_house_spiced_seed_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "40c9596ba479bfe759454bca9cb36410c8e054396904b9581e551a8035d9e585"),
		new("item", "earlymodern_food_soaking_salt_cod_bundle", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f03df13e8b36b00fe7f5b1ff67b6aab7c71e063ddc0f47f2cec0a3b03b70cc8e"),
		new("item", "earlymodern_food_unbaked_ship_sea_pie_layers", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c0f163b96735f3a98e1b88a624e407c0ea5dd8089f7eccebeed8e81a265864d"),
		new("item", "earlymodern_food_viennese_coffee_house_nut_crescent", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b85160da850bf0b009aec8942f9288121d8666c9d0f96915dc218152c50120b8"),
		new("item", "earlymodern_footwear_andean_camelid_overshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2241caf99a509aa2f350798e5ee1042cbfd60b15e25bb5e8a29d36251e356e01"),
		new("item", "earlymodern_footwear_heeled_wooden_pattens", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8f3790393725c5d2ff7e84afd3f2096a1bc5d647306cf3894e14e1db4cb8b37"),
		new("item", "earlymodern_footwear_qing_flowerpot_platformshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4044f365c31c33b2bc7d2b75b97e12ceba610341e8ec07c1409b5e1a38354b40"),
		new("item", "earlymodern_frontier_military_tall_fur_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "210111ad27a09335456a8f8b62e52afc16c3cbcb5763df78e755521cebed1a8a"),
		new("item", "earlymodern_frontier_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa8b7d6e6b8dd282daea6a541780812835fa9df59bd470c4e866999c51b034bb"),
		new("item", "earlymodern_frontier_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "751bfcfbbde505c22f0bb8d07005c18966b35f8a81ac7be3f96bf90b99b5278e"),
		new("item", "earlymodern_furniture_and_counting_desk_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc80c8525a871ebb5b027475f0b6ac45926f8fee6eff667220d4ffc696f24fc9"),
		new("item", "earlymodern_furniture_bra_archive_wall_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa71d07eca75b688985d9aeeed93d2b8c7f7a4290f29194d9af5302169c86f7d"),
		new("item", "earlymodern_furniture_bra_bookcase_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0729e79fcbbf298644e9758653b9961e91c022f0ef5e89119d4559cc3109c23"),
		new("item", "earlymodern_furniture_bra_cupboard_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "67f6658e095abe86c0a5635eadbc578c96b3415d71d6e9c528073a3d0de535a2"),
		new("item", "earlymodern_furniture_bra_dining_table_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "10babec9b5e891d13ab49e0653e40e0e56fcc51ebe40f4a8f2db53ceb81b5d03"),
		new("item", "earlymodern_furniture_car_linen_press_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "72ae811c9a3566a1be99bd1e3efece6e0dd7abb3bfbee91b0e2fa78354487cb0"),
		new("item", "earlymodern_furniture_cna_cupboard_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "21801a418931adc4524b3266976089135a9129720ad776570ac5b4981ecea0b6"),
		new("item", "earlymodern_furniture_eng_dining_table_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd93f1235053c77ea9b1c0b66856374a521e9f1ccb56c8d764cacce90f134c50"),
		new("item", "earlymodern_furniture_eng_linen_press_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf1040aeae4ad3cc78677857f3900778dcf5607921a6a843fef41855dfd8cb23"),
		new("item", "earlymodern_furniture_eng_wardrobe_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcf7af1a8195ade9eb879ffdae2bfa4a2f7ac738862c044e974bcaf1bea9e245"),
		new("item", "earlymodern_furniture_eth_side_table_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b0f86f5573f702dc90ea89daba8f365f4463b7e495f9e3c30ae2d188b60a57f"),
		new("item", "earlymodern_furniture_frb_dining_table_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "07369716ba790395a021b167994efaa1a9e3986c0498cd3dc6f6ef777e7df1db"),
		new("item", "earlymodern_furniture_frb_screen_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e11f2a16a55ade710871b9c5e7c69ea91dbff8e97b894159f499fae56c270c55"),
		new("item", "earlymodern_furniture_ger_dining_table_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "feb6cec71b414cfeca8f2a5a9908153910191db1b9ad79b0fbcb2e5d46e7d56e"),
		new("item", "earlymodern_furniture_ger_side_table_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "984b15649076d3dfc02fe75ac966f1b589b8cd40a28e1626fc8aba5ab58a5f21"),
		new("item", "earlymodern_furniture_ger_wardrobe_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "65517748048f1136d3122bcaf4cb99ea9d450886dc53aeffb5a4bd23a6a879c6"),
		new("item", "earlymodern_furniture_ibe_bench_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "287853116c914cf5dbb827097f0a2f55af1dcd725196f6c63dd0f99c9179ec8b"),
		new("item", "earlymodern_furniture_ibe_mirror_case_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c59adb3bf128f96aa36331054e8e8d2676887245ecb7e4cb5c71cdacea4ea08"),
		new("item", "earlymodern_furniture_ibe_writing_desk_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "51d7e6baf07d6ec0dacfce9339a75961a6cae1c4f46836a47b6a7d77bfb2c9af"),
		new("item", "earlymodern_furniture_ina_clock_case_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "571e54aaa75c146f48535e2bd260c2c8df881f5239ec2e8354f80b9906a72228"),
		new("item", "earlymodern_furniture_ita_bench_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e94feb6d5a5c860a28be1e2ffa3af225f1f1b5ee4fc6631b9fd09035f7365d8c"),
		new("item", "earlymodern_furniture_ita_clock_case_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4192d88a71ef14eaf9e84d2cce74bf72d81c7f62496ed40a25b0a6c0f7cd463a"),
		new("item", "earlymodern_furniture_ita_mirror_case_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "78612329787ebff423bf59fd82239291ed611e894c2771b767c05f9383bc3ac0"),
		new("item", "earlymodern_furniture_kon_dining_table_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b145fd2f3c2c3b10677aa38d36a8f358b008a4fac59dc698c015a925839da9b"),
		new("item", "earlymodern_furniture_kon_writing_desk_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "94a0a59d6df6cceb523f03c69bab79e5a36ca529557243a4c50f8edc6176bcfb"),
		new("item", "earlymodern_furniture_mes_display_cabinet_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8dcb96143a12053c66b57c419f28a44904ea5a074a682ec46c67341e5c97cfa7"),
	];
}
