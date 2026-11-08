// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Bronze_Stylus", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "47a00d5f946eb9104311ae4936b01393cfab9520445d1688cfbab3cf1a0753c7"),
		new("component", "Antiquity_Reed_Stylus", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "da7bf563ceba524a89933fbc16a95818a97f082dd1f2b943bf3b66823e216477"),
		new("component", "PreparedFood_Antiquity_Stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3153a6b24ebc1a7925d2454c1621d35e5237e270470b336d46a1643355c2c83"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_andean_chuno_egg_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "570f2f3cc6ffc0ab538ba38fbe2a6fe539465f991c9c6d0c24b7a3427e084b79"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_tea_rusk", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "403a68394486e95d539c15c6c64a2fddf4f65c587ccb6584e6cfa9a101cb31af"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_caribbean_enslaved_cornmeal_ration", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a3d57b8358f45eed0cddeb943d6103abda91a56eaf0b8ae5d360e257600c88d"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_dutch_poffertjes_plate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf6b14fbae295774d6603a3251605c8597d37093f14186b6ea31510b060b694c"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_brown_ragout_meat", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb20750336630ee6ec090cdbc62521af05e511b4d76e36e53cf5901b6c5be02e"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_german_coffee_house_linzer_torte", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d93d2d389eb8762fbd513aded53b0d5ed1d1e28b169d33925caa586224047cc5"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mesoamerican_pork_tamale", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe864aa4fb66a6af874939ee657bd1d6afa0f7b4c60487dece84917161fd1145"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mexican_cacao_almond_marzipan", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "21579bb6e0df1b39a31005c9ca77cca9e10ff89993a94ecec1cff466f0653fbc"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_dum_pukht_meat_rice", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "42f34a20d86b97147cc64430bab70341b9f92061e6e732ded48911a0020fa39e"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_quince_jam_cheese_plate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "76f37f33db28441e2e993a259f2c6c28b1e0b39611b41d61a69fba299e86c6f2"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_starch_delight_roll", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2662f11684a7fe026d135273885b9628834c5d238badb26a86509e654167ecc4"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_woodlands_hominy_bean_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f10b936078dd03915c4a792976f31316221c45c7f8a14bd04aa64fde1456e22"),
		new("component", "PreparedFood_Catalogue_medieval_food_almond_rice_muhallabiya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ff2f2d13d01c3a2c6ae3c6f7498b6da04b6ce1c2211be194b98f722ad9d259e"),
		new("component", "PreparedFood_Catalogue_medieval_food_almond_thickened_fasting_sauce_fish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a75fcec5c9da325c734a0a57a0e7d50d1112406ccbd2b555360a7c560feff730"),
		new("component", "PreparedFood_Catalogue_medieval_food_cheese_filled_rissoles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1f52ad2c3c6f91e9be9ba43e849aaee06729e981e898b4bc09b5690ed7bee0e"),
		new("component", "PreparedFood_Catalogue_medieval_food_herb_flower_omelette", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b39a535d4a549c3b4b58ae02dc2ff07e189bdb0b9972643403a122190aee896"),
		new("component", "PreparedFood_Catalogue_medieval_food_japanese_bean_yokan_block", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce422a80abf059f0261068856647123784d0d18321e399f85449148d2c50ebca"),
		new("component", "PreparedFood_Catalogue_medieval_food_leaf_steamed_spiced_fish_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cad6c83b8143e94b334e10e3d80d46c44d8af1d64130e6a0416b8fc1fe7f6314"),
		new("component", "PreparedFood_Catalogue_medieval_food_meat_rice_muhallabiya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9baca584b6b6314e40d768499df4963a240f732ad02504a978a9635286cb8f3"),
		new("component", "PreparedFood_Catalogue_medieval_food_sahel_honey_millet_feast_mound", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4aca44ee7927d6af9c0c08abe5a419b4d0181a7196af06882e7a7fcb2c1b5641"),
		new("component", "PreparedFood_Catalogue_medieval_food_sainted_figure_marchpane", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ec99079337691e95e251b58dcaadbed8db50cfb281ac2cba5fa127c44a26b14"),
		new("component", "PreparedFood_Catalogue_medieval_food_sanbusak_almond_sweet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "84372344d27e5633bcf17cd30139e71e45296a49f01ce30eeb96ab1d8b16ee17"),
		new("component", "PreparedFood_Catalogue_medieval_food_sanbusak_cheese_parcels", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7c4f0e38acbac6cf9cd7a24b2acecd14d5fdbe36d682261420c87cc40a41b94"),
		new("component", "PreparedFood_Catalogue_medieval_food_sikbaj_sweet_sour_meat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7664b9a2be8ce681ffa6064cae8756a05eb334cd580ac2da4970d4187d218b4d"),
		new("component", "PreparedFood_Catalogue_medieval_food_song_candied_fruit_skewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f81f791652e26868ae3c377eaf063aeadeba554c91f6dfac52cfdeb4cfa92359"),
		new("component", "PreparedFood_Catalogue_medieval_food_steamed_jaggery_rice_dumplings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "691fa3bf088025e43be2397e4d8ac70719f8c2c8633f99f343b172f9b45c9646"),
		new("component", "PreparedFood_Catalogue_medieval_food_sweet_bean_zongzi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8de0eed3b9b8107cdbf5b6cc81c00d19881e6cbb43d0eb22a313f1b3bca0242"),
		new("component", "PreparedFood_Catalogue_medieval_food_west_african_leaf_wrapped_grain_pudding", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2125ed940f9108db8f4928d326dea3d445c1fdc044369f2df05bcb7d710e01a"),
		new("component", "PreparedFood_Catalogue_medieval_food_yuan_knife_cut_noodle_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f48feede0eaa5ca0c3c1312a10783fa2ae7a7bee073175bd47b0c612f909d577"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_apple_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1ac59b84de79871140367d9bb05f7ec397ad888d80f6d0073ff90acc4837635"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_baked_onions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5ac5b7f553b96b6093fb90ae8a061202ba281cf7d0afa6f5e88481c3e3a35b4"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_baked_stuffed_clams", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50aca78e4d22afeec98a0d7b642d94f10ec256f77b940ef6676cbcd9b0ddc3ac"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beer_barm_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d9d4cb0b7a87cf45b70e0102a5dd5322f5b205f097a8cdd7d12522202299e53"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_and_leek_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a31fa2bed9f8976bf9058c4a0978521cfec216015e07afc196e8252d54d570b1"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3ead71dd0fd4606958c1ebdc8b0287c899ed981fb9cba1a3e9b2f037be8d9f1"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_black_bean_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f3ac1110063760a2bdae3c69ddd646439583179da65537a30da9d2f61feadd8"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_grain_with_egg", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8205b0dfcc6619b9038dab76054dd42676025d9d200bbcad8f27618a26079acb"),
	];
}
