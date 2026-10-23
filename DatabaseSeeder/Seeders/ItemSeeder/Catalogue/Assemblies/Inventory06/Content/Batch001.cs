// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Quill_Pen", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "84db0ec9dd5656473012bb59b3e3bfe8fd74592e0ccb48398e78b488a2271dc3"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_bath_sugar_bun", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed2569941d789dfc91ac466c3d9f7e80399e6fa8f7e8d13bb574d7a39432ee54"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_potted_shrimp_toast", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a5e89413843c7dd4d2ac5f1d0bf8f6fc0820df531e5c21d9f951f5e34d43402"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_company_mulligatawny_like_soup", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3e1c92bcf88581bef9ea88ee73df8670cc4ded0806564153de6fcbcbc290112"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_dutch_oliebol_currant_fritter", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "15661bdcdfe64035e48b26df26eb2c48c83edc06f476b995d4c6ff07f04bb24b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_hors_doeuvre_anchovy_toasts", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0ba4f6afbe42848162464a10d586aa26dad5b2af31f22b510cb88fd9921b66d"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_service_royal_poultry", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ceb35da3d9160d27774af377d24f4f8ce062bfc75fc30020f1b70d1a49daccc"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_gold_coast_maize_kenkey_like", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "11ddbbb6ad86514776b85c91f8c713919dd920b3507ee65b94e62463dcd8ac0b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_joseon_chile_cabbage_kimchi", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "59e97562449d7c70baa8abf4522b16d6fe971ea631edb129f3f00c793e9ac929"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_chile_lamb_skewers", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f4ba524b03c2aa5b7ef017b1a03d4fefb8c3376409f628949171ea4e420d663"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_eggplant_nut_korma", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "70cd14a7f1c08208720a267f150cdaeda62b8ac19b87edecefef8c415102b104"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_tea_smoked_duck", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb7b8d93326f0054d64a7eca3d5c831f3fb93958e691ee442f984637bbc59481"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_safavid_jeweled_fowl_rice", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c70d36ddbb1f11afeee48a32db3aa7d5ee81121126a767306e38f5d1f66fff3"),
		new("component", "PreparedFood_Catalogue_medieval_food_almond_milk_mortrews", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a168f7841900977fcd8510ca7ffcc975467e32621b1432f8f8ce2a48b6c82aae"),
		new("component", "PreparedFood_Catalogue_medieval_food_buddhist_mushroom_noodle_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "154d31ee8f7d25be58614455dc7f2401c10d588310a2abe193dee727256eed76"),
		new("component", "PreparedFood_Catalogue_medieval_food_byzantine_court_stuffed_fowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a0a1ce4859da3182940ec7b0e195bc8fb733bef74b92202bf86240dfd234345"),
		new("component", "PreparedFood_Catalogue_medieval_food_byzantine_spiced_meat_paste", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c795baea4029348e3e5dc9002cfc6d3d89699fa24d00cf3d76bde87847151fbe"),
		new("component", "PreparedFood_Catalogue_medieval_food_ethiopian_christian_fast_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc9e9a44fd0c996c5fa687f1111c0a02aa2a25afa7b57b8296071cc45f200065"),
		new("component", "PreparedFood_Catalogue_medieval_food_itriyya_almond_fast_noodles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f330f4eb80b1c908069a4087fc1dc3b2b9ccdc19c723af3633bfd1ee755a845"),
		new("component", "PreparedFood_Catalogue_medieval_food_japanese_zen_sesame_tofu", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "37b8aeff2e0d5ba7230e3ecfeff583293f75d31fce31590692b073926ae62d2b"),
		new("component", "PreparedFood_Catalogue_medieval_food_malt_sugar_animal_figure", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "10b430582a88743451dfa68a553bf4e0d4b1effa1040f82cefd1b376d58892cb"),
		new("component", "PreparedFood_Catalogue_medieval_food_peacock_revested_feast", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0058a6605a3be798d52fb447f9a2f4a305e97063899292f50aaa841ea3992d62"),
		new("component", "PreparedFood_Catalogue_medieval_food_persianate_fruit_meatballs", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2891d04d8316adbf1c3533c684167996d103596677d368cb4a0627f2a47092a0"),
		new("component", "PreparedFood_Catalogue_medieval_food_persianate_rice_crust_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ae7b62956b4c3a2354e1e35117e447fdc0a63fc7f21c40d54e8588b228aac70"),
		new("component", "PreparedFood_Catalogue_medieval_food_south_asian_market_kachori", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "13fa954119d546e25aa6e82f0c66db2d419d3e7a3112ca551c7b798dea4079bf"),
		new("component", "PreparedFood_Catalogue_medieval_food_sultanate_almond_meat_halwa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "174ac804cbe733aad4409bc20f2652cf5ad28df1b5dfeb73ecfec82b60749a61"),
		new("component", "PreparedFood_Catalogue_medieval_food_swahili_coconut_like_fish_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "071f4b849fc419a12a5ce5681b4e7572889da8c95eb752857541b267644a9ccb"),
		new("component", "PreparedFood_Catalogue_medieval_food_sweet_spiced_egg_tart", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b972340657d8572dedd045e8dd54400cca14203c393779f2cc8b85d41e450660"),
		new("component", "PreparedFood_Catalogue_medieval_food_temple_coconut_like_modaka", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7332ee048c358ccd8b0d1d822e3c80b72776fb5820fbe6adf5050070a6b78f32"),
		new("component", "PreparedFood_Catalogue_medieval_food_trans_saharan_date_millet_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b147e3fa900bcf14c93bc786b6ff0decc3334328d4dcf85c7eb33292fe12f868"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_apple_oat_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a67895c09f8e0502ca02071b9ec7772a086e1d02930a8236482cb865add5a064"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_baked_fresh_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e31e674734f4874d0de459c8cb307db921697d7c4591c2e191e80885bab9ddb0"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_crusted_fried_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a96904d5cc250ff40bf654c78c0f7d5dcbb0699fb1bd228f82f4a3e75c057d0"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_and_cabbage_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "28538869bd950d35fd6ac5e743967fcd0198324f54d0a893c12abb25c65aa6df"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_herb_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ae213a64e9a8903a6d3efefcf4621269dd088e1fd9dd6b64228ec64b10068e3"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_onion_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be6b049279874eff7c152e476630d12ae40271225b4546cd7d4c003ee98e3a4d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_berry_honey_ration_bar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d450ef0f9cdf7a539962d654266b942b64cfcffbc48d7443bcc716e149ab2a4c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_bread_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c60488db07aef74be5be6944717b92e398a857592536be5e8bc485ba1cfa381b"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_grain_with_mushrooms", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8d0ad7a6388fe278fc8ad26616087ef645fbc1670ecf8155a48ce60f11bb773"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_head_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ae58e008684c5abd61ff3c376cb8d4a6926dc3a768e4f9add3454912c1dc502"),
	];
}
