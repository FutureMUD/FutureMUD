// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch029() =>
	[
		new("item", "antiquity_parchment_quire", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb9ada95212e33a3f8d1411b64cc502f53e330c647cf4d1c65bc5e97ed84b3c0"),
		new("item", "antiquity_pitch_soaked_torch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c0303b80bd5031e53422cec2de13ecda064955c306d24adb4aa95140fc44ddb"),
		new("item", "antiquity_plain_linen_cushion", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "56b8f6f949280d43b74d1b81f7f238590327b0304effdb947ce8604f1ccaa7c1"),
		new("item", "antiquity_plain_wax_candle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aff6b2cdcebd348c7f96fb6f6c30b430af3adf024285f49a483c66e19a8c3913"),
		new("item", "antiquity_portable_garment_cabinet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8248d0dfaaf720b95bc28eb09b400b99366e43826801cdbbc7001ce04a4c5d8"),
		new("item", "antiquity_roman_crested_montefortino_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b393801d28a691f93afaf8df964d57c948190086a929ebab0e2f9732068ca61"),
		new("item", "antiquity_roman_late_scale_coat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a749a8ad10fddcfc6265e61127198f7546e59a4550a84b49b3646c9cf30a7cf"),
		new("item", "antiquity_roman_montefortino_bronze_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "729cca45d336e7ea9827095a77f7ad39805f612a6e0b1efeaa46589b64f5194a"),
		new("item", "antiquity_sarapis_wool_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "06833d5be6f242d09eeeb4bced89fc4437a6c83edf0f74ca688e9bc99b4651c1"),
		new("item", "antiquity_sealable_scroll", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e2ee7ecf47e5162f9fdd4859b40cc5fab4c6d2a657ce8578e8448d84fbbefb4"),
		new("item", "antiquity_sealed_clay_tablet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a9b9253103f071557fefb47a5198ec7aaf7148cd28f1ee8eaf91a6a059bc539"),
		new("item", "antiquity_shield_common_wicker_shield", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5fb36bd301e8ab9dec0496ba7ee3dc9710147858ec0fb933c4c627d55c469fa"),
		new("item", "antiquity_shield_hellenic_pelta", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "06085ff06d1d5cabe45ebd0dac1ae3a6d2ce17d20e840ef32255ba26637203e1"),
		new("item", "antiquity_shield_kushite_large_oxhide_shield", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "769087eea8b7a6cbbc96848975976e83f1071a903e114d04f7d239d3724ac3d5"),
		new("item", "antiquity_shield_sarmatian_round_targe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd06344abbd642469b1d1f77be172d2fab9971ed826be8397c0388624d767c7e"),
		new("item", "antiquity_slow_potters_wheel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ca58908ea3475f996fe24895b76928671d6d241b924ce9c23dfc662a6ebc6f4"),
		new("item", "antiquity_small_offering_table", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f992b47029e9ae3b199693257f038d6454a916e7aae3d3c3d61ce4aab2e6809c"),
		new("item", "antiquity_steppe_saddlebag_pack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1ab0e619b2f44f445872ff8da6ce6b4a91f17fdd06a99f5ac0823680482a6b2"),
		new("item", "antiquity_stone_mallet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3158b7d858d312f6b6de3ae07a407fe126e9c8344bbc1995b872d3bf03696e7"),
		new("item", "antiquity_straight_linen_kalasiris", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7916a73c5f09bee563806aa4bfd2f75c7367e20f4890465810e7452e81032a02"),
		new("item", "antiquity_stuffed_wool_cushion", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5ec6d3da7193ceffcb6d3aac83977d4e805a4cada2b1529f0b03aa529c180b8"),
		new("item", "antiquity_tableware_anatolian_red_burnished_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9d09c2607f3c8030d6f1b66dc540a646041a4c7b28bd90c387b68634c2ca521"),
		new("item", "antiquity_tableware_germanic_bread_trencher", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "92d72b28cee83bee30ff172d2e76cb060e68f9b097729354700d12e612811dff"),
		new("item", "antiquity_tableware_italic_silver_wine_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aea651234fc54fbf8a7c0c5384358cbd2cca394b4dfeff9ab8e3a81ffc871752"),
		new("item", "antiquity_tableware_nubian_red_black_goblet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a307c22af630352611039a41d7e8f675265d5264d434419bf154dc16100c3e9f"),
		new("item", "antiquity_tableware_scythian_gold_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc63d4ef5541bb7b1326802a32dcf79f63205af8c1f72e19e49d5693ce36d3bd"),
		new("item", "antiquity_tableware_scythian_hide_lined_wooden_tray", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa7b95ce220be55f8bbf208027c796bcdd6a203c6699247a622116c2518314f0"),
		new("item", "antiquity_tableware_scythian_wooden_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4f719a68828734f5a5e2278e9ac131b193e16749e9c2b592c72f33664b99dca"),
		new("item", "antiquity_tall_narrow_trade_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "56f3041d2f615d3f3bf5766ac528ea4f9cf01a7e660727cf21bf78e2251a80b6"),
		new("item", "antiquity_twine_shuttle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3480bc92585616541cf90df454137c73bb8f35c623218175abf51b65e266428c"),
		new("item", "antiquity_weapon_common_simple_sling", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "95e9cc840a8abde57a893180faf83885d5916892f5342cfcded022eeb3945111"),
		new("item", "antiquity_weapon_common_staff_sling", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "94891a090117c751947fbadaad4fea1709b30d88faa4160ac3ccbd20c9cab595"),
		new("item", "antiquity_weapon_egyptian_light_javelin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6f659ee81dc533dfa5e6327a0a1dcd295db80c0c2a131478b989c1ddb04fb73"),
		new("item", "antiquity_weapon_egyptian_short_spear", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa101dcad2d22fa2ae31e9dd2b4927fdc31e37bfa304204f77967f5b78517095"),
		new("item", "antiquity_weapon_etruscan_short_sword", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dae2bbecb5b81f4782dd421feced502dfb3aa286a4654902ee0894feb4f1f89"),
		new("item", "antiquity_weapon_kushite_short_sword", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6ba138a83ae45289ec0a51544d56c18ed64442857b6733149c802dc53f9d99b"),
		new("item", "antiquity_weapon_persian_akinakes", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f060c214802b6f92661fe1a4951e09e875f513948b51bb336b5a27708f797e11"),
		new("item", "antiquity_weapon_persian_sagaris_axe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5585121306edeca4633cb7e42633c56ab14b96bed492c19d89e245b76c8b7978"),
		new("item", "antiquity_weapon_roman_late_spatha", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb067d6e7c5c751f1dc69a4c45c3ca699930af47b568fd9aea006056d023626c"),
		new("item", "antiquity_weavers_sword", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5141eea1b6190d93884af40fc10061b52a9388117dc11f2737c7514d68f86b67"),
	];
}
