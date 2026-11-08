// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch032() =>
	[
		new("item", "earlymodern_door_sic_warehouse_gate_275", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e461973622d9a0d03d841b8d543bd50aa4c696fc365927e12f86c9a81c5e451a"),
		new("item", "earlymodern_door_swa_keyring_527", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a941ff7c0721951c35f24a866f5ddef284005317709252fcd01210475f30282e"),
		new("item", "earlymodern_door_swa_panel_door_514", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad3d6772b146475438ceb6c7e8816f018e10c153cc628db42b1119896b4580fb"),
		new("item", "earlymodern_door_wa_garden_gate_447", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cb4e2137643cf92c4823fd633c3316c5c598f4790e28ea08c93fe63ca63f0ec"),
		new("item", "earlymodern_ethiopian_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40b2f604d659afb2f21e4f8fc4e21f402e29d3748e5567d35ad324fce27433e0"),
		new("item", "earlymodern_food_caribbean_enslaved_cornmeal_ration", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0eb14fe0dc2ce9bdeb29bd7869e388a7510176e67bc77bcd779ef79261bc5815"),
		new("item", "earlymodern_food_caribbean_plantation_sugar_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "03e0ba00664cecfd062f288fd7ee3b1142fd2a81d5bd40e8d5842f3946b1bcae"),
		new("item", "earlymodern_food_dutch_company_spice_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ce170d74cb35e52f01745a6661302476492c5a4d5a21ee3f27cf3e7025b6cf7"),
		new("item", "earlymodern_food_french_chocolate_ice_cream", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7460763ec6f9e499e02bc9b40e437a3cdd3db6b85e9c65107da71da6bbf08084"),
		new("item", "earlymodern_food_german_coffee_house_linzer_torte", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "470bbf2459dc1b080f15461ad3ceaba6a599f201a383ea4cc4880d08383edfeb"),
		new("item", "earlymodern_food_german_potato_dumplings", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9975bd1914f4f12cd9e40004bc1d00d4b3ac001f913af54058386c1c35943a0f"),
		new("item", "earlymodern_food_iberian_pepper_stuffed_cod", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "33e84594d96cd3d0bd240920c68a4e0f6228e095d60e3cb70018c643c9f4b8be"),
		new("item", "earlymodern_food_joseon_royal_gujeolpan", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "feb4749b3680d9579a4e7b85666d7707a47e7d0543c9161673f621d92bd2abac"),
		new("item", "earlymodern_food_kimchi_salted_cabbage_stage", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc2dd6d29c171a72b9b747cc5bc716e01f1377159648d40466c932be3082a0ae"),
		new("item", "earlymodern_food_mesoamerican_pork_tamale", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd5cf3703f4cfad792111e1e6f192e5ae167eadc98fc68be085b0dfd958304af"),
		new("item", "earlymodern_food_naval_sea_pie_layers", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c43e1c72edb25396c2de23f2bba1a20d5e4f9e758c8de9ca500c598d575f6996"),
		new("item", "earlymodern_food_qing_sweet_potato_famine_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f03be6968004e72a8a9d7f2fd53a8e425ab20e50123140838a9cf20daddbb02b"),
		new("item", "earlymodern_food_tokugawa_bento_theatre_box", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1f966fa4ba34d1cbf440ee0a042dae68b4519570a317d3c7b42dddc8d78f79c"),
		new("item", "earlymodern_footwear_edo_lacquered_geta", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e65b3e4636165cf726453accabb596f6ccc4c7c80803ed72ecaefa6520b7eec7"),
		new("item", "earlymodern_footwear_joseon_leather_ridingboots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4f366b367e4dc0ce3536abcb889814ba2ab9091e71c5eefc1b82ecf7caa80f6"),
		new("item", "earlymodern_footwear_sahel_quilted_ridingboots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e3a35ec7cc1c1d4a55cfd878b1c56a3f2fa97e7bb79509c1f6ad663a0b89124"),
		new("item", "earlymodern_french_clothing_long_skirted_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cdf6f7bcce9658bdefc3681bf64969aa5a9ff826a6fc349014abe8d0c058d7ce"),
		new("item", "earlymodern_french_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a658fb6ce8d682fc5dda80f698ad55bc353914125a5bc290fb886b9fea38338"),
		new("item", "earlymodern_french_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3da49a02516e4b8fcdb98267021f1909b50f28bb68b534c83939c45c3ffdd89"),
		new("item", "earlymodern_frontier_military_braided_dolman", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0d49536b3d842531b90547e66f92c8a8a54deab8cc8eb381f1d134d761ad132"),
		new("item", "earlymodern_frontier_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "32fb110bef111745e31c00365f9b8a9b650de237ba30b2c83c007dd75654a8bd"),
		new("item", "earlymodern_furniture_bra_mirror_case_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c4fcfa37aca893143c66ae5bcd963fe4c89eff4e22ae94fa047770c92423439"),
		new("item", "earlymodern_furniture_cna_armour_stand_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ea589ee15c805ce3089f9e883be92c9304ceacb74e8e6d6d572872e8c33cadc"),
		new("item", "earlymodern_furniture_dut_armour_stand_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a89566e1910d5658d6326ab2bbc54e06b50429fa911e7e2925bedbd8be1501f"),
		new("item", "earlymodern_furniture_dut_bookcase_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c2c2b868ccdebc8c7f0aa135b815cc525e83bae6dcfa82d43e2985c835b7547"),
		new("item", "earlymodern_furniture_dut_mirror_case_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6dc6408a9e2b9d3dac14fd72e2409325770b8a60f43ab8b9fc92ee2339f13768"),
		new("item", "earlymodern_furniture_eng_writing_desk_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e48f12fcde2ba66b685236fbc358381c8a297fe111dc4e371a8f676138b90f06"),
		new("item", "earlymodern_furniture_frb_bookcase_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "578080a9204d32098cce8c98d589855a9b93022e171a3fc8a5a69bd8246529bd"),
		new("item", "earlymodern_furniture_ger_writing_desk_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f55e571de3fe60f4185ea8a0d78307406846708c885286d92b9a3e0b5613966b"),
		new("item", "earlymodern_furniture_gmt_dining_table_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb766f92ddb252fb166537a4e1a61c7bdefd193271e4ac8414c9c9637dc670b6"),
		new("item", "earlymodern_furniture_gmt_writing_desk_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4db02041fddb2fd128ef3fe8e59533c791f5e825e84beb99b8b5ffbad7a11649"),
		new("item", "earlymodern_furniture_ias_bookcase_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6da6f85714b47fb116f5e24d5528cab0e725ea48d5879820da7d5f04b1c6a830"),
		new("item", "earlymodern_furniture_ias_display_plinth_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a8e8052fa6532097e52f6db94cd91ed370a8e76bb75527f032fcb707f6de9af"),
		new("item", "earlymodern_furniture_ias_side_table_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "39bf5dd6742dc69a4e05d54b057994925c268fb3a07fdeaf317a8f81f2389455"),
		new("item", "earlymodern_furniture_ibe_dining_table_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "38973e1eff3d5007a98f895e1928906ba7fd4733426011885a04f0dd882d6b5a"),
	];
}
