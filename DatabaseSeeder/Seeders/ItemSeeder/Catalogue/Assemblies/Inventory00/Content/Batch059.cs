// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch059() =>
	[
		new("item", "renaissance_furniture_jpn_armour_tree_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eda07eb99a8a51afb082f276ac320c017a8ef1e6735e6f96809dfbba6557dad4"),
		new("item", "renaissance_furniture_mar_freestanding_weapon_rack_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "836a840d11605032a19754880e16c1e5fe9e7a39f2b01960dcfde751313b79d3"),
		new("item", "renaissance_furniture_mar_lock_fitted_display_cabinet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a72ab89d2090e4a132e2feb7ea564d8fcb31c47775e5db445341212264d1aeac"),
		new("item", "renaissance_furniture_mar_lock_fitted_wardrobe_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50dcf6a9dfebdea593454d309d57f918573314c5ba33494cd456fb74a0b6b441"),
		new("item", "renaissance_furniture_mea_chest_of_drawers_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a0923ced0a621010b0758dbd8947af569f8e0cd11a360fe6f66fbf5b2e37f66"),
		new("item", "renaissance_furniture_mea_scroll_display_cabinet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "72d0d4feda2f5dd2faceb0662e70abf8ebbb50156a58f6094a79f8014acb3fe1"),
		new("item", "renaissance_furniture_mea_writing_desk_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53d6afa4f4a6d628741380d891cc4665c637540a8704b013f4776dae259a3640"),
		new("item", "renaissance_furniture_msea_folding_display_stand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a110bbed638cc29705fa4fa01948777c5fd8889b24980fc4f4895d4800454919"),
		new("item", "renaissance_furniture_msea_spear_rack_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df822b4905e285af499aee38d3d04d0d326f056cf35aea58e0bf170f5b4a975f"),
		new("item", "renaissance_furniture_nac_open_display_shelves_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "56f4049bca0e0bdbaa44f87a67b33e72c925fde4bc5f3a347ac4b3b6259ce643"),
		new("item", "renaissance_furniture_nba_many_drawer_cabinet_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a3c6fe8b1cd222e126bbd2ab7121736b744c7a9dd5c43b0b8c5225a4bbba0e0"),
		new("item", "renaissance_furniture_ott_porcelain_display_cabinet_06", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d2bd3239ae79d563d922f7701812f02ae813686bac3fd73475b7e2f2f97f092"),
		new("item", "renaissance_furniture_ottoman_bazaar_dye_mixing_vat", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78972612d2c80e4ed17c9b7bf25c6e586e3b6639a9a4565e5557f72030592a33"),
		new("item", "renaissance_furniture_persianate_court_niche_display_cabinet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "963e79bd3fc089cae018cb394408b27c48b11404066b0aeb2a5775eb39365db1"),
		new("item", "renaissance_furniture_pip_bedside_table_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d542173e5a75ed2e8609aa7b2f4cb2973e31f3209c268ced17affdd60268d287"),
		new("item", "renaissance_furniture_rse_lock_fitted_wardrobe_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bbb5d050862806a06e5e476b52d37f488bb8a0d4045dfb904af7722ad6c0710"),
		new("item", "renaissance_furniture_shipboard_purser_water_butt", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7378ec37308cb40510f94b9229e332b63197cf70e7c5f6e98bbe1c4158156c83"),
		new("item", "renaissance_furniture_steppe_caravan_felt_storage_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e2f81d5b7fef93fa042e3267ac34ac44e5d87495781a46a123c6ef21c614d04"),
		new("item", "renaissance_furniture_urban_kitchen_flour_meal_bin", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82c807c79cfb2c6b761f9f20b814638542dd5422deed0075085f4e18b147b955"),
		new("item", "renaissance_furniture_wer_harness_stand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "22cc4ba7a643969e917d8324add1ae5b23ad30532713e04a6901aef6d6133607"),
		new("item", "renaissance_furniture_wer_lock_fitted_weapon_cabinet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2ee27db810b41d66b9469790bc0dd805697c467baabdb500593f0bbc25403b4"),
		new("item", "renaissance_furniture_wer_specimen_cabinet_03_lockable", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac5ee23f95ea70523cf369d520da9dd33e2baff1da33f5bf82e3a662979620dd"),
		new("item", "renaissance_furniture_west_african_court_brassware_shelf", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9653991640cf531a33503c19ae941e95243542b1dac74737edc768f6636ffee"),
		new("item", "renaissance_japanese_straw_raincape", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6818a7dab51b8b0b990f6d047b8fb829bd5a5b12fa5a58bdcbc44017b2979ea"),
		new("item", "renaissance_jewellery_aca_badge_467", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad3e07c770b5be6c03c5857fde674bf6ff7b5f66f128e0b1aa6d7c1c9ad527a2"),
		new("item", "renaissance_jewellery_and_hairpin_612", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "20b6f936082f5a52859176e50c374271ba7391e9dcdfcd71479d3b9deeaaf618"),
		new("item", "renaissance_jewellery_car_incense_holder_658", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a936f1957020307f6d39f7f546d9ca94c2285416027a0d66580d72bc2ac3fd20"),
		new("item", "renaissance_jewellery_car_offering_bowl_657", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4f70c5446ceb104c413ca4b11ca85708f0c496fe85bf233988de74b996ddd7d"),
		new("item", "renaissance_jewellery_car_prayer_beads_651", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01f9000fc8f029bfcf812fd963880bc0c1bc7296d62aa8ea57a87607b012b38e"),
		new("item", "renaissance_jewellery_cef_devotional_badge_143", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "366c4450adbaea883adb8fa87ab10da98ffbee0aad2fbfc24494aaa88ab47cab"),
		new("item", "renaissance_jewellery_cef_hairpin_132", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "474e915e14f8c2563883123107257dc82ef0cac2bf6bb831f859cba8e1636957"),
		new("item", "renaissance_jewellery_cef_prayer_beads_141", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "60219690d5802d3d4b6503f5cd27baab80e29577bc9a2dcf55db6d2451a12b53"),
		new("item", "renaissance_jewellery_cen_chain_necklace_65", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "06e45143310ae67cae27c1765b8adc946b083a6425f4b0c21dcf3055b17d96a2"),
		new("item", "renaissance_jewellery_cen_diadem_74", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c53ad138b7c5eef5b1d5ba95b4548902be269bbfaac69a805187dfcc614b4e0"),
		new("item", "renaissance_jewellery_common_badge_197", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0bbd09452671091ee01b21dbf6be973973a3ba66ed584c87896f6d58a866aa4f"),
		new("item", "renaissance_jewellery_common_bead_necklace_124", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb4072af6d9d08335760433a3a0ac5e3b4b7a1dccae6daaf53e641334bf77962"),
		new("item", "renaissance_jewellery_common_chain_necklace_215", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "854601c5179754ecf3541591862bfabe3f5ea3675f0814adb3d8cdc47e441b7f"),
		new("item", "renaissance_jewellery_common_circlet_13", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce6d220184a40d9cbcd9f956c007820f33e6b1d356f73abb4c7a39d9964017b2"),
		new("item", "renaissance_jewellery_common_devotional_medal_52", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aad8905486be041a05b99ee18c7ed7b707dc845cbaae8b2ebb6aed1d713f262f"),
		new("item", "renaissance_jewellery_common_diadem_194", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "585939560f96bd8f28bfa4d7b5e01844ada04f5b497029e0b76b326fc0a45a79"),
	];
}
