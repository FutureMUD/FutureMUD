// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_food_warm_choux_working_paste", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f4b42393f670c4f08f712709f76de85f304d858821d95674aebc06b432a2f8a"),
		new("item", "earlymodern_footwear_low_hunting_boots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61374160808ad17bf1f2da00f8d3f0df8e2258a534ab8cf6335fad02b31550b2"),
		new("item", "earlymodern_footwear_pilgrim_road_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b61c57bcf2d5bbd5643c76a34dc4e9b42a5a2e9be70dfcfb1b6bfd822d7beea"),
		new("item", "earlymodern_footwear_strap_buckled_halfboots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "89a472fc70f7e5aa2d1a5d3610661c07d4effc2af515056719a33b3ed3e93278"),
		new("item", "earlymodern_furniture_and_writing_desk_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9c70e4e883db04bb33966ad8e1cd61bdf07951b41b7e1e56023edfef636305e"),
		new("item", "earlymodern_furniture_bra_screen_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0efb76da71f28539a8083247e70053d2f4a5d558918b2bf0fa8fe0bb9d5e7e2b"),
		new("item", "earlymodern_furniture_car_drawer_chest_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bea8a4cc32e36d47ef10f20b880ee4af0124117e3aee5c7208397e79c711771"),
		new("item", "earlymodern_furniture_car_screen_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c6011686a081a79bce821bc2a1228b7bf0224d9134f941abe064c2c3c7c9e00"),
		new("item", "earlymodern_furniture_car_wardrobe_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcafa34a0e07c3f251069766ed23a659c60376e257da5400867a5440594740b2"),
		new("item", "earlymodern_furniture_cna_archive_wall_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c19ac148321eb0f3092cb7b8cd6f392aa5909a75cf8695fed4d01ed1521356e9"),
		new("item", "earlymodern_furniture_eng_screen_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "876ce1cd7606344a5a3f5a82f7c34a4beab2002d1d39107c8685bb676d4a0e8f"),
		new("item", "earlymodern_furniture_ina_display_cabinet_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3011882e9a88bff3c47a25d6c86f30fb920d28f94d8dd9729bd073d7d6bdda1f"),
		new("item", "earlymodern_furniture_ita_dining_table_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbf580521135784c067c0019b94bfd455822b1bec16b76c82807ddda36d1a7d2"),
		new("item", "earlymodern_furniture_jos_linen_press_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a17f73755413b18dd89988446ae3384987431bfbda9a6ae1784124b0b9a21e96"),
		new("item", "earlymodern_furniture_jos_writing_desk_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c091ac4e68029e289446c25214ba5c5b177ef1f2747446785129b9e9bf6ccd97"),
		new("item", "earlymodern_furniture_kon_bench_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dd209a56fc4b91d2df7f9170507f7042e34d182af68968329e47d0e0cf2f7b7"),
		new("item", "earlymodern_furniture_kon_mirror_case_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ad10ab891b60709fcc45f8ff4e1264b248d10accf6e7171e8e82c91cfcda28e"),
		new("item", "earlymodern_furniture_mag_archive_wall_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7661eb98889aa248b109e5aad638e3c6b4bbf485179047d4b56f386757fbb42e"),
		new("item", "earlymodern_furniture_mse_side_table_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "525db60aae241119d5c16b9498ebe18fd010eb8a1b56e5aef8cc14c1335d09c9"),
		new("item", "earlymodern_furniture_mug_armoire_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c170b95f12676fee8015c04cd655686a3713dbf6105e59fdf1e63dd7963ec968"),
		new("item", "earlymodern_furniture_ott_archive_wall_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce012472538f58dc46c5a0023853bf7193c3beeda56fb8b1c97e6c8e5b7c0f48"),
		new("item", "earlymodern_furniture_ott_weapon_rack_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "31d21d2cf6aa520864b4cb5791e99b9e199439cae9ce498957d4d04ecd0e43f1"),
		new("item", "earlymodern_furniture_plh_display_plinth_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c5fa1eb4d420afd02088406a1749089246235ee134a2e8b690857414a111e99"),
		new("item", "earlymodern_furniture_rus_wardrobe_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7a400b5ee679dd39b32b27c40429208fecd5d739db59d4fbfd087610c29ae0f"),
		new("item", "earlymodern_furniture_ryu_display_cabinet_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfc32407304bd2e54f422a623e0bd2195912c5cbd04bead4a145b1658bf4263b"),
		new("item", "earlymodern_furniture_saf_screen_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d2e0978111deb7ec91c4da6bf1d9676e3c7a49be1b75861065f773be3575434"),
		new("item", "earlymodern_furniture_sah_side_table_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fc4fd6cbb2765c64cfa96bfa59172f2ef8cd7169bdb72ae876008a22c87ba7e"),
		new("item", "earlymodern_furniture_sca_counting_desk_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a517b9e42dfe9f55c9f0242d361f6507ad9727ec5036136e67784203505de11f"),
		new("item", "earlymodern_furniture_sea_linen_press_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3afd45400753a66f05ddfe06ec50d4a7a09064016e2d02a560e432dfe07c2447"),
		new("item", "earlymodern_furniture_spa_cupboard_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "131e81b1129475f24e22e23a16074ade11020e718e163e19bf0fd20d64c01482"),
		new("item", "earlymodern_furniture_swa_clock_case_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3972231e326badb22199776e0aca6ae50a5a31ddcee5df413a7b0c875b0e4f2"),
		new("item", "earlymodern_furniture_swa_screen_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6028e36e73f9259e6836bfbe97b9047bece3363ea41a2ab176d92565d44a9ae"),
		new("item", "earlymodern_german_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d86fea2a711d36112ec8dc37a0f868aaea9fbb0961686eed44f3a3db6dc810b"),
		new("item", "earlymodern_globalmaritime_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f66c57f2b0269eedc17562a731356a239b17e237d89e42bb5a56f509ed21257"),
		new("item", "earlymodern_globalmaritime_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c687b577e2b76709af3386bde94723c53bfd400d3fa1f7710b2733b6c30b163c"),
		new("item", "earlymodern_globalmaritime_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad1711b508728535d1e5e8ca09d1067da3015e6c4e43779745ce986f9abef3e5"),
		new("item", "earlymodern_headwear_fontange_lace_headdress", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c65c45fd4c9d78ebeb4f64fe03dedc7c28c270b62edf07e3756d635b7576e718"),
		new("item", "earlymodern_headwear_russian_womens_fur_bonnet", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "579aef3e099bf24c19a7315093a8847951d8b9103b78ec38b6f70db08842a9b1"),
		new("item", "earlymodern_headwear_tall_crowned_capotain", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f51ffdbbe441b36ded3329f3608b4b6a7ba73721aeba88ccae505383bb4e99a3"),
		new("item", "earlymodern_household_coffee_grinder", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2927acc53a3f0e398c6d825f086263d560f265affe1e2b92f3d894f5e2ce247b"),
	];
}
