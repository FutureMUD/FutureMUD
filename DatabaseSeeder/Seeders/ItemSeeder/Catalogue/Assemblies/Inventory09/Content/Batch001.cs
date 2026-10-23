// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Clay_Tablet_Surface", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "65b8da664cd7d0ce3179de215ee99ddba14f04d3f6c833789acc7cc8068bfad8"),
		new("component", "Antiquity_Parchment_Scroll_Surface", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e3f97e98f5870f1155772bdceda9e1e8cab4fa3bd770b7b7886da636f9b93f1"),
		new("component", "PreparedFood_Antiquity_Preserved", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c18c9b88fc78d6a45e67b69e9e36fc1f9ffd762f06474404e1d720e87a8233b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_benin_chile_pepper_soup", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e2daf4d44f303d016bc537b5f3d7fc7ee407d2ce456454f33cc53cea7411da0"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_brazilian_enslaved_bean_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2aa659e4a03ff8350b067871e6a47aede54acb1797eed01ecd2b506a406c47ae"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_steak_kidney_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b486f0899bbcd003cf165ea91d8e05a7773bd42b64a63e89d922c94e0b402c8a"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_sussex_like_pond_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2315c142cc92774943ac1078500c59c9b2e1d0fe4c84a9fb1192efed028105c3"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_company_mango_like_chutney_toast", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8df0b3138d5a536d820ea9aa504a0fcdaf2e0b20169d45014b73aefd41224e07"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_coffee_biscuit", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "759fc78173b0f01f6b44727a5b493b23afb6dc2251aac364723940a3f82bc3ae"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_fruit_sorbet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e7e717d3092ed84eb23873c3b9995907cc395626ce7e92f68625f1cc0d0c733"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_iberian_cod_pil_pil", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "24e64912fb6175d436bde50e0997669e97d72bb1776f1de41825e73bf9f2ae35"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_iberian_new_world_tomato_fry", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f4b73ccf7a5deabc9a5c62dd168dd949d165d34538f9e5afad3c4d89a4979ba"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_mango_like_ice", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7207d16dc3508358fa1e45918a373a40b735ed4fd8050dd47a1bef6aa0f6da01"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_murgh_musallam", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e06579b449ac346d3a2b4af326938b13ad1b17c191aee0f41ad1cdf8da80280"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_green_pepper_dolma", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c8554ef66b638ebf343e98c984f3fdf321f4d24b2d24eee72439600350dc4d4"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_imaret_meat_rice_ration", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f553bb2ea1611ecbed9520f0f4f5f90a6be97a651bf7c5dc6390ca6b9103bec2"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_candied_sweet_potato", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e54c7586a0345199927749c1491ef02977f7215f644e8effbaeaa4d5928f52f9"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_eight_treasure_duck", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "031ae51f304aad1fec280fe5bf244cb4b25eea3592ef36f0bd7f3bacd725b181"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_maize_millet_relief_porridge", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9af835f0af2f2963cd249c2fa43ba257391bec05cf64413c91ba7adfb13623c"),
		new("component", "PreparedFood_Catalogue_medieval_food_boars_head_ceremonial_service", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "40c9cabf8c866440901506b836bd780d3503d7b7795126c5eb8bcef462ad4d6a"),
		new("component", "PreparedFood_Catalogue_medieval_food_byzantine_fish_roe_spread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3200ed3468bbaeeb25139c72e1bd47bda53201403baf2470c9f8c0f4849f035"),
		new("component", "PreparedFood_Catalogue_medieval_food_byzantine_monastic_lentil_paste", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a815f1679962b0873046ff447de6587965e301f09099693b0319e113b65826eb"),
		new("component", "PreparedFood_Catalogue_medieval_food_court_spice_wafer_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b2299193bed523cee7f9c8b72d0d6ff76ead65f23877e57f70e9c6d0ecee477"),
		new("component", "PreparedFood_Catalogue_medieval_food_korean_mandu_broth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a75579866fdb23b4bf00d0f816880b33991ce41478bb402a4aefa93807d2759f"),
		new("component", "PreparedFood_Catalogue_medieval_food_lotus_like_seed_moon_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "668658c73ee76feca528b72fb7c38f3337b8c7529212832c3bc2ef3d04bfa142"),
		new("component", "PreparedFood_Catalogue_medieval_food_mande_leaf_sauce_rice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd1b62875ae02fac5d66094fa5ca4bf69c8241b1cf084d64d397f0883b242d9f"),
		new("component", "PreparedFood_Catalogue_medieval_food_monastic_plain_khichri", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b28d0ce183e7cca4398baa4f87dd4cf40a491aed5ed0c56c56dd5c2f551c5ad0"),
		new("component", "PreparedFood_Catalogue_medieval_food_shishbarak_meat_dumplings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d61166ab27e3dd1908ff4134b3ee97921c6a04c45b9a68accf7e346350f4061"),
		new("component", "PreparedFood_Catalogue_medieval_food_spiced_wine_trencher_sops", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3a706cc53ddf8a53ef613a14bbf6518ce4e1ae850e6b59599d90fee5e8b4241"),
		new("component", "PreparedFood_Catalogue_medieval_food_yuan_mutton_wonton_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "31605e6cf6de67ebbb4fbf64d1804784952793cbf4357b41dcb222a2ced07c5b"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_ale_soaked_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "20b5c2932098aa0ec3c7a95d4fd87cf9de9723c7ae5b5cbbc3d27403f7e2e5bb"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_army_grain_ration_disc", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cc2165c68431f39e0c0ceb9f822341c73d07e3e598b0539424173400e9bd7ee"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_aubergine_and_chickpea_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5e5ab1de46a14f1fc41080607bf4b4b987d5fd27f2efe8de3cf10f689830291"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_baked_squash_halves", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "09ef3993d3f200ea95ca43ed9599ccb46cfa91b866fc337646460b69a0f1d875"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_and_onion_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed3ec89c9fda146ab35e44d26974e623eb5af81ace1ca75bd12d747fb86e1024"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_batter_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "412dddd3312b104efda5d96ecaa3814863c3ac182929b8a6dfe4e73b5427bf9a"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_chickpea_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "daa6c8eaf0d29da752c1b794f1bc3a7f082d44d87eca7c1cb65f5e8ea3a2ae45"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_and_lentil_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36d79a074c1a609250189a2cfd3227ea721286f3535ddbdd1526c07a2a208da3"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_braised_radish_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea1d2b2aa71f1b8aeafc594daf8978578c837f124769f27acc597de197d6c516"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bread_stuffed_fowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dca1faa715365a4508b06bb50c778e4a622028908a3065d2a8604219ae038526"),
	];
}
