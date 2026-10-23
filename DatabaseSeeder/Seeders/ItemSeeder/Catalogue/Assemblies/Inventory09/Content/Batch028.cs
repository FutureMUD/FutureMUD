// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch028() =>
	[
		new("item", "antiquity_small_painted_plaque", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e07409d07eb1aedc53bd7a04125685c224e1e9836db3d3d3acbc7741cf0c0fd"),
		new("item", "antiquity_stone_public_well", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a27914452f9c929f08a18f8107d47902f8aed8df1105806877d215d9faeb5027"),
		new("item", "antiquity_tableware_hellenic_shallow_meze_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "596c69922849676b5a11b8c531c8dd3b6d564c1318b28917397c6e291bf2f634"),
		new("item", "antiquity_tableware_hellenic_skyphos", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ccbeeaac275aae852ded76c011d143fef0ba035fb1f1baaaae2da7c90167301"),
		new("item", "antiquity_tableware_italic_terra_sigillata_serving_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd182422745334291a3a324e0dedc2901fe27d562c960b1eee51d1b9c2efe399"),
		new("item", "antiquity_tableware_persian_bronze_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f2f20efdeda70d78b8d20482dc00d97a4fcf2baa803e5fe19d1e172af540e22"),
		new("item", "antiquity_weapon_anatolian_long_dagger", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "769edb4877b7431cd3b57d819306ae102d97ad6abd6950dd4684540374ba77b7"),
		new("item", "antiquity_weapon_common_bronze_dagger", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "120fc8c5172fc6dfcbab4e869eb06226218f910aad7388ca4c0c08f09549889d"),
		new("item", "antiquity_weapon_common_iron_utility_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b77f9dde9e10129d9ce25e3fa2af3006cde0cfb3be5baf8ef175d551f00d7bc"),
		new("item", "antiquity_weapon_roman_pugio_dagger", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3e561b47be78c24374f39e146e47bbdc3fdd8f1ca3ebaa531092d0cdce46f26"),
		new("item", "earlymodern_brazil_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2360c1a1b9bd4022b66b077df25fd8b903bdec6fb615a372e755dd984f65bf14"),
		new("item", "earlymodern_british_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "59332f4559e51cb421c0eff6a6ffac1b52b12f6cbe6ed81377e66f2da7dd5cf2"),
		new("item", "earlymodern_british_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c233b5bdbe2a471836551e867deccef53042d56fbb6a31f15d3b1fd3d72db95f"),
		new("item", "earlymodern_caribbean_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1b929513eaac80465c6acaae4cdfb16694d3c0c9b5f9894e1ba2a6d5b58aa76"),
		new("item", "earlymodern_caribbean_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "37750b8e6673a0bc1923c014c461cf57a00277a8a4b475b29acb3490e7fb99e1"),
		new("item", "earlymodern_colonialnorthamerica_clothing_hooded_wool_capote", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "29124f9af10459044c164c17231b1d02eeeef588373b39aa061c521ba82d2a3f"),
		new("item", "earlymodern_colonialnorthamerica_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e98b76095e570c690be557d05db57d0a9db5a8406a6388cd5b3e4dc844df2de"),
		new("item", "earlymodern_container_bra_tea_bowl_33", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc949bf24e6ed79f82ed4a874b5318a2d62629209480e5abefeb2e205b53af2f"),
		new("item", "earlymodern_container_bra_tea_caddy_27", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6469246c41970f66936f28060bd0ce9ba75629351f86ad23010a25d72ad4b87"),
		new("item", "earlymodern_container_car_account_box_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2744d5d2ac44ffabc6e5dab62225ef2c326c0ad1502356c77ad88bc85ec51fc2"),
		new("item", "earlymodern_container_cna_tea_pot_33", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f3d34ed0301d32fb882aa8f30033d7ae82ab8f4f8eb7ccb6f3f43931f6c3152"),
		new("item", "earlymodern_container_dut_document_case_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4810be736a40f1e3188501fce3872211ae329b2eeeba46d52dadf7fa8d75d0c"),
		new("item", "earlymodern_container_eng_linen_hamper_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "076da07bb4ea74d1cc125dcbf04e5c950b3450bf6baadd6959bb4e9f7f3020dc"),
		new("item", "earlymodern_container_eng_storage_jar_32", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "66c88c9d763bf87064590f48185458a98ccfec52c5931a05326965cffb2345ef"),
		new("item", "earlymodern_container_ger_bottle_28", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "de4b118306d99939025a7eb2844206c71daa9713e65fb69e57281abbd416ba28"),
		new("item", "earlymodern_container_ibe_coffee_pot_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1270323c3c0ece575a5bb2d2c50cfd8de5d7bd016c4d0c0a76d6c8b88b23893b"),
		new("item", "earlymodern_container_ibe_ewer_30", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e2ba5ea19c00461cd5fd7821b250148146a8307b2ff5d339623f54dd843ef37"),
		new("item", "earlymodern_container_ibe_sample_case_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "146a3d97456e63a95e3bc9b660c59d433adc3c3626f7c66ed36bcae487f600a7"),
		new("item", "earlymodern_container_ina_account_box_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a56495e585974b2c4d19b6ec4a2514c49735e439005d32e5212232c95576d67"),
		new("item", "earlymodern_container_ina_document_case_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5e66c6841570228e11e4c3e35c71ae774108693d5d2f6827e8c2c3af8f20c42"),
		new("item", "earlymodern_container_ita_account_box_30", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d65661b71f7d8fa077f6a900bcb4d3ba810f79649d27d491486da9fcda441c5"),
		new("item", "earlymodern_container_jos_document_case_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b409ae41b5cdf25f6450ea81cee110b7504210f8942a92a691785d3219b5a3e7"),
		new("item", "earlymodern_container_jos_spice_basket_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "070e6db737bf8cc1b74485880615f99f2f12b1510fcb777ce6b412d74e40e401"),
		new("item", "earlymodern_container_lmg_travel_coffer_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e543771de19a3b3b2c43301589182c4a66e97caafbda2f7bcbba8f7376cacf8"),
		new("item", "earlymodern_container_mes_account_box_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "82e9541d845da7fc09876d4b06a0d38b4807935f04b51c9349153eeb5426c44f"),
		new("item", "earlymodern_container_mse_sample_case_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ff2aa85df3aa887ad71143e99d0296b8b703c9a451692422f77bf5d32bae3b3"),
		new("item", "earlymodern_container_mse_storage_jar_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e9c90a21fe5e2a996287b95a8b9f61dea582e5303990b4f099a6dea0667740d"),
		new("item", "earlymodern_container_ott_coffee_pot_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7e3d5a429a0df5edfe7f03c38b0e8e9ecf31ac18636f4ee06d5b1d4264af72c"),
		new("item", "earlymodern_container_plh_linen_hamper_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ee5b84f4d934fc0d4f531f22a39f72ecbd43f39532172b7a6db71e675cf11df"),
		new("item", "earlymodern_container_plh_spice_basket_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "60b5b19e642ff7549bbbcbf2fb0064b092589991ef8d87c365bea408396e16c3"),
	];
}
