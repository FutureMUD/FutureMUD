// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Papyrus_Scroll_Surface", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3079f03bd52c228f59d64f901c9799132880860e0caa29add29dd2d1b03c57f"),
		new("component", "Antiquity_Parchment_Codex", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a633c9528799aaa3ff8a6786d8e5cdc4e949669cf771613393b09cc25751b7d3"),
		new("component", "PreparedFood_Antiquity_Bread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9ddfe928feead4363871809cbd847f1960bb3f92e5b977f767c9701ffa03949"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_minced_tongue_sandwich", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ceddf271c4c23f8781ac50d73eae23bc9c2a2e67edfc83a3177e938f86027b0f"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_naval_pease_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fa5195d3f77ee16391a1749d4b2618886471b15974b8d484e54ad7df698b7f1"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_yorkshire_batter_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4116c54f834492eeb7a3e88690d03338f8da6e7bcb63f0d7a793063f1afa2d45"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_caribbean_molasses_tamarind_sweet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aab9a62906ec105900616b51a358b91ac1bc7c64b35a55123f10c813a53feae1"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_company_ship_curry_pie", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f39b62719d2adeb4661a0354488b865acbb68016b017820fb0bc75cdaa8bec72"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_bechamel_baked_fish", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "622c47201db99b3bb659a8e81c86773bb9eb45622a78230605e17f0d7d829b65"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_entremet_spinach_custard", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4102e4cb76c4a3dd8e1d00a21f20bee39e36084de181e4af2712d92d3ea316e7"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_goan_vindaloo_like_pork", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "393c515bb33504b7311c68cb5ac2f26decc42cf0aa49e264963b2a4bb5f0344e"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_louisiana_african_rice_fritter", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a542461b42ecf5f6d3b878a4b54dd3fc51f2bd6246a44d0321d3d5acb9997c7"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_hunkar_style_aubergine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "199107523f18f83c9caf039d65aa538e7814b21636778314eb914b5c50bbf5b4"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_beggars_chicken", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "24ceefc57d9c38aef6695d775ea2df3a58ce8fc6a36ebb4563cde67d7d091a73"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_chile_oil_noodles", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "09791869681142fd2303eccf8c3009973a271bf985458d0ff03e2b919f571177"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_tempura_assortment", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fd0c5906656aeed49537534c57bd04add086596796082cecec050f2965cb6e0"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_viennese_coffee_house_nut_crescent", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "551a0d8729fb732d89aced8439c7ccbd59f1b2fc9d88b3f162ecaf6df14ff809"),
		new("component", "PreparedFood_Catalogue_medieval_food_byzantine_layered_sesame_pastry", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a150869a738226080a115f9f8f1e24e0da52c604c949ca026acb79cf5f31b124"),
		new("component", "PreparedFood_Catalogue_medieval_food_egg_and_fruit_fast_pie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "059cc45a83129068d5e57c3c23c07be318f9afea1601074cb5dcdc08fbe986aa"),
		new("component", "PreparedFood_Catalogue_medieval_food_fish_mortrews_for_fast_day", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "362e3cd3fcba614f39653b1077cc757438dfd0d3fc4e0d71f3c42d9bc61707a0"),
		new("component", "PreparedFood_Catalogue_medieval_food_galantine_of_fish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3585f469b49df775b402995733ea9d13387a1fef11787c48b25a01468391a480"),
		new("component", "PreparedFood_Catalogue_medieval_food_itriyya_meat_broth_noodles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "769fc3a3b6b2fde20872018c425eed565bf4623a037aeb3a03fe8504a1555a53"),
		new("component", "PreparedFood_Catalogue_medieval_food_judhab_layered_chicken_bread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "39cd766538cfeafaca48a616129aca8868868bfaf592106f470518910ab299d3"),
		new("component", "PreparedFood_Catalogue_medieval_food_lenten_almond_wafer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0eda393f1713efde5f8767bef3b53a4b144895624255918eeb3406cc53f72c78"),
		new("component", "PreparedFood_Catalogue_medieval_food_monastic_fasting_pease_pudding", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d0fda323ac4139595e21ad66c7de902aee7a8a6e574eaecc7874afd6410598d"),
		new("component", "PreparedFood_Catalogue_medieval_food_raised_game_pie_with_jelly", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f2f1babc7f1db6cfb32959006b937d31e6f9e2da445c47d5e4a80601609b2c5"),
		new("component", "PreparedFood_Catalogue_medieval_food_rose_sugar_preserve_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3fbf9a71eaa9e9fc067c53f634e5ba3a6a16df104923a9cb0deabe704f098d4"),
		new("component", "PreparedFood_Catalogue_medieval_food_song_cold_sesame_noodles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "86f6823bb0fd37f8833fec81af292460cec5ce45cfcdcc08c893cca50fe0244f"),
		new("component", "PreparedFood_Catalogue_medieval_food_temple_black_gram_vada", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1b4cea39135d3ee2f6745241b6425bef29b2740b74a803c23df84e6d8f6d09f"),
		new("component", "PreparedFood_Catalogue_medieval_food_west_african_honey_bean_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6a111d735f45644d2c43975846fe9f293b898c5e9976c8124b010cc7acc76bf"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_apple_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b441645c27e01de52b596cb9b32b2dc1fded31246c71e995c2f719ca1353258f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_apple_fruit_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "05b6fd023a53dd5bf2b26f2e9b8fe1a2e957a5b975fa793bb249f550f77cca52"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_baked_quince", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "58ba49e0e18bf5e22c65a3f2a90f408dadee94fb2b1417407beb5101d7f9bc40"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_baked_yoghurt_dish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66be1b72df732823382467f66e937628c1e97ed152b5f61190dcbb0a2752b371"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_vegetable_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3cef49bd201a161a1b9bf9acd65ea88f43a82cb8179f05e708dd1113a3dee91"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_cabbage_wedges", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddf5e0bb3717e7c47519ceaa73ea775fdf0a526bdd5adb3d54a3cd4b68645574"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_chestnut_bowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3bc987e078311206a7cc739451803a1f02aa0f7ba425857e453bdabaaf3f4f9"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_braised_leeks", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6456484de435f7e7f7e815e11b4509dc0e6f777bdc2188fd17461e63ecc7e509"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bread_crumb_dumpling", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81fe529e72d911972b802f977d9bc1d00dc67725006ec2ce936215090cfa396d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_brined_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7760153d5d4b0a576643a03facc9681b2e809251369b905ab5db33b541ffedb"),
	];
}
