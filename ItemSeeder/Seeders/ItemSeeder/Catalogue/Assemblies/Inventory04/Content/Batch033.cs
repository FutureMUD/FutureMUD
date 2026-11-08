// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch033() =>
	[
		new("item", "earlymodern_footwear_tarred_deck_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0dc2a2181c64ee3b43ffd72b77a4d4a85beff344418e0436f3e83fbbb3c0ce18"),
		new("item", "earlymodern_french_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "47a8bbf4ba766724430d94397a6ef37d7223980593d6654e8ef6423a6c00f930"),
		new("item", "earlymodern_furniture_and_bench_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "07db38fb47ab1234e68dcc741386e05563c2cc3f4716fd8b50a00b1bedcb80fd"),
		new("item", "earlymodern_furniture_cna_mirror_case_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0ade437173ae5a04cf11538764ea9d6503db648263a0a2f3a38edf793436f50"),
		new("item", "earlymodern_furniture_edo_archive_wall_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "78d1aca9b6679adc8260cf0601463865572e8a1ec10d2b0644421a7d45981174"),
		new("item", "earlymodern_furniture_edo_display_plinth_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce8f5ee3742a6f01026e3da03a8e28138c17deb67e9ee361e76d910288989a63"),
		new("item", "earlymodern_furniture_edo_linen_press_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c20c3334373132299b44062179218c8fab5fa03c61c3e2388495e20ae3e6aa20"),
		new("item", "earlymodern_furniture_edo_wardrobe_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bb60b882b50bf00b0499cfe7b10f74c1c38e7bd3d5d8720ce727e50e50d44f3"),
		new("item", "earlymodern_furniture_eng_bench_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f5bfe44bce106c1d8eab07036ab9722da870e7209fec93e1b3d2013589026a2"),
		new("item", "earlymodern_furniture_eth_clock_case_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b511bdbf7089332ec8a176c45e30515b9c2f855945e9a2b277ed6dedc3de5ca1"),
		new("item", "earlymodern_furniture_frb_armour_stand_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "87f631fb3b7b36dc268c79f0839eb794482cf5561f5b189379f392f095fc3502"),
		new("item", "earlymodern_furniture_frb_bench_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ef2798a5a0c82a6934ec49a32c6dba935333ea66110d964ffee539eccd432ac"),
		new("item", "earlymodern_furniture_ger_cupboard_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c24a60dd70b8b2dcc557a0627eb7e26ffd3765ba37607d272036585bc87cc642"),
		new("item", "earlymodern_furniture_ger_display_plinth_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9bcbb054e87af5a2247f668c7cca1ba758e73bcd8c0299f603abd48a23b25f1c"),
		new("item", "earlymodern_furniture_gmt_display_plinth_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "19e7be6840d130b2e1e1b40947f7220fec1d406eaa4e5f8d5debb6b624d84382"),
		new("item", "earlymodern_furniture_gmt_linen_press_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3dc57c605491c895391f906e7a94e404a01a51079ed6dcc8948d4c8dd1385926"),
		new("item", "earlymodern_furniture_gmt_wardrobe_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c5e4a56f0b253dc0e4fc3c6a06b7038ed48e89eed6722628be5b90e71101ec7"),
		new("item", "earlymodern_furniture_ias_writing_desk_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b56ce7fff815ab8150109d06bba8e7144a5052e96a3850e46ab9938c91a4918c"),
		new("item", "earlymodern_furniture_ibe_archive_wall_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ccccbf2642e5f420a019b9d6cf7919d8db2024814ad1d16880621f8ed193a08"),
		new("item", "earlymodern_furniture_ita_armoire_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa450199ab230ca61009cf8c45165482e3e1f6ae8c23272aa087157f4cdbe78e"),
		new("item", "earlymodern_furniture_ita_side_table_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff58f7778ca5c878c71183ef507c59ad5556837566914bbd5b5e05c6f98c0df7"),
		new("item", "earlymodern_furniture_kon_armour_stand_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2167db7450770f5afda068562665523584ffbb249446bf7884da4d0b04f1ce1e"),
		new("item", "earlymodern_furniture_lmg_linen_press_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "37eaddd12bfbeebee36073b07b492315b92fdbf3eda8926482c5424cd5af3e7b"),
		new("item", "earlymodern_furniture_lmg_wardrobe_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "95ef1fef5f753c2597b68c49554cb3ec20c2d6dbe8a3e4d050d795348519c09a"),
		new("item", "earlymodern_furniture_mag_display_cabinet_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2b3ae9a30eb041de5426c0642e244e6e6cf58a6bef05ecc33aa79c5fb145f0e"),
		new("item", "earlymodern_furniture_mag_linen_press_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0449150cc227ae9f24bf2da095c1f07a0615a48c1fd364d5be942bf447b0ac9e"),
		new("item", "earlymodern_furniture_mes_cupboard_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "61b84dfa2aa4176a92b98d7f6b2863e164096bb7d267097a707a8f10e14777cb"),
		new("item", "earlymodern_furniture_mrd_armoire_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc980cd8015dd463d1ef44ad9a0fff2719d7f586fd4c42afe4e1e24f1ec3be01"),
		new("item", "earlymodern_furniture_mrd_bookcase_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "94078c7a81064aa41a63a3efffb0832ea536004e14dfb52773a1d683392e1801"),
		new("item", "earlymodern_furniture_mrd_screen_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "87eb99e9d9c8add1929c2c84f917dd2325e9d3e236ebaef52f14dbddc5c740fc"),
		new("item", "earlymodern_furniture_mrd_wardrobe_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "94df1ae80d136aa95766130d6e1ecb9dc36901c06da33fc6c4e6ba157c3ded46"),
		new("item", "earlymodern_furniture_mug_mirror_case_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a51b8a90e0a08cc6fa205966f89a4c559c017926e78ab102668a0d5d3cb7301b"),
		new("item", "earlymodern_furniture_ott_screen_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "963ad04a5219b7e3ad9b52db2ed13deadbfda7c32a83fc412d3810c4ac18c324"),
		new("item", "earlymodern_furniture_plh_wardrobe_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9402b657df390a3833f8eca2d6722f1053704883ec8253cd37d2e8ea8426b82"),
		new("item", "earlymodern_furniture_ryu_counting_desk_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "60988bb451c3ee4a40a98dd995ec0fc13e868e0f3ccb7ffd833cef696ec70e6f"),
		new("item", "earlymodern_furniture_saf_armoire_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "805deda448fb374d2f03b2b333b863d6c83e8bb29a7f3fd8d581a667ff9b8eec"),
		new("item", "earlymodern_furniture_saf_display_cabinet_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ba08af9cf5bc60a8bebdcb20c6a1dd55cf2b5fe646170c6a8f12b730357df3b"),
		new("item", "earlymodern_furniture_sah_display_plinth_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "86cfe5d14a6f53433fed132dcebaa7dc5938c8cb64281216813c6e4e3060f91e"),
		new("item", "earlymodern_furniture_sea_cupboard_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bb7b9295ad655180507e1f2ba521fa67c4796f69728af993e8fb5cae8ef45a4"),
		new("item", "earlymodern_furniture_spa_linen_press_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "40240bed3178ae2f8fddeb7cc133a29a2be6fc138f200cc52111c00424f1ca61"),
	];
}
