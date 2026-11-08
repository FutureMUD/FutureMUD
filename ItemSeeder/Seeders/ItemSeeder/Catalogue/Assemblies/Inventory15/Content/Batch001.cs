// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory15Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Parchment_Bifolium_Surface", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d55025d95ca82bbd5e0cf83751045e9149849e45491e93c49603ab26d1d4b5ea"),
		new("component", "Antiquity_Parchment_Sheet_Surface", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "91f1377bf0ad065dbaa3add3df7a35597f7692c2ab75893818ee9c49208a2365"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_andean_chile_cheese_potato", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2231cc1dd18aa428d561896339c1ad37da70d5891d1caf8ce4e9c8434e99a93b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_pound_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c7bb3b2f6c160172a236ddc0fadf252a00e49688e20e6975648a6a486ac0a3c"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_caribbean_festival_like_fried_dough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "63e795272ad2117066287a0930c1a2a94897a4de070283f8cbbc42a289c303fe"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_company_madras_curry_plate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a083a6d9d8c3385518af3303142644c10096b32beca99bf40bc824b79a313092"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_chocolate_dipping_biscuit", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "409d619584be89082511f4930175ba80cdd88df6e7f589c99bc9f69444adbe14"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_iberian_cacao_almond_turron", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc6afd4091e877900836a1a748dcb615abe66e0568931db499c253cd32217fdb"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_joseon_sinseollo_brazier", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9dc65df1ad518bb591444493f12d9e92629d78b3601ddf2c4be10eaa1f992d7"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tea_table_cucumber_like_pickle_toast", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c46b3acffaafc62afa31dcdb0d65d4b589f9b08e4db0a1e509fe2754edfa29f7"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_buckwheat_noodle_stall", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7bc2489cb7e69b20026c0f63ada92030c9cf3e91aa3e00dc185d5e73717eecd"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_famine_sweet_potato", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "39940cad9c384baa830775ea32f26115154218697751e58585891ca50615d321"),
		new("component", "PreparedFood_Catalogue_medieval_food_alms_house_bread_pottage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "27aade9a2b6487a7685bb8167994a1332348e37567c0132099c0810f4ce8dbd6"),
		new("component", "PreparedFood_Catalogue_medieval_food_coloured_blancmanger_quarters", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba6ff029756152685c2478e3724ca1dd5d79998381979cbc8e1e6faf71b8fa3e"),
		new("component", "PreparedFood_Catalogue_medieval_food_khushkananaj_filled_biscuit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca286e05f18289e9b6ffca9b885c65e0e78d7e6b8da0896a15680ac2a1cccac1"),
		new("component", "PreparedFood_Catalogue_medieval_food_mali_court_rice_meat_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0bd9eb15271285c2a95b7191fcaf6e644136794d920d76055be6117965c49b32"),
		new("component", "PreparedFood_Catalogue_medieval_food_medieval_fermented_dosa_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "508e9d2eea27be5b64af4017a59ad0a5887f978b0ec5381df97f30f97089d229"),
		new("component", "PreparedFood_Catalogue_medieval_food_rosewater_almond_launde", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e3ad4eac25d99a117976fc4a76a17ed7fcf7e1848771f2b3d1a2a250f448d53"),
		new("component", "PreparedFood_Catalogue_medieval_food_stamped_spice_gingerbread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d2843ae24157f84d722a41ef750b0e6d632aae5a286ccad1294bfb78c732f74"),
		new("component", "PreparedFood_Catalogue_medieval_food_zirbaj_white_spiced_fowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "607fa092cce1b034b9a8dc7a15b37f579fdabed599001c16e2e409a822664b5f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_aged_hard_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "06032e6a027ce0108ad275718e527e1abd65b0a323e6c7a80f456fad43871da7"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_apple_griddle_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "05c5448ad79d8e23662856a56dca6555be7873f4510717a0ef553e9fc06d4128"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_fish_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "919ec628493423e5afa7deb74a3af2885b9c882302566a2a026a1d32b60ea3e9"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_meal_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d1a7c549b718e4396850c6dba6fb7e445203ef8ec382d4978bd5a7495986696"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_with_cabbage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "728c1063316ba4423ce448897aa66af5368df0194c83cc98070cdec66e227898"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_berry_barley_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3eb3b9a9be88986b3ea9fa97f9b78f1c794474dd6041396373c089a77adeb383"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_blood_and_onion_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "915ff2271819389395d633f9dc12172c08a558ea28bb0efa210d835c2522af47"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_eel_coils", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffe2e6a1e90148877c590ebce454b5353ff2e5b64e770b8729e1e9059acf18d8"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_garlic_cloves", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0836c72cd0c0821748092f933067a11b512f676478dfdc1fbeaac06086d8fe0"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_grain_with_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a361b8083d5d7adbedab5035efbfd2a80445de54288357c5b8061cbe931c3011"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_mushroom_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf44db37566dcc5107c36bffe565cd676e38951352e213de1d0ceb044f786043"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_oat_groats", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ac5110ad1a8da6855da8e1040c18b5a315b513e701e0a6cf1a3072fa2590f48"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_sorghum", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a5cf5cf05bf3d630956b2ef34808b3a169f963350c40a5d714afd239428b93c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_braised_kale_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb7b2fb1173c0bd78dfb1ef6f6b0160894ec7131dc37bea69c9a712d56998b7e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bread_and_mushroom_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "338a77c36ecf742e66ece9de7afd76d6252a04b9445f2c002f88614e5344824c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_buckwheat_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50f2249e0b4b79e863cbc0245848158d495b6abb8ab227123e16d21bc3d2d7ae"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_buttered_cabbage_ribbons", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "edb5c5fc8eadfb1429fbf3dee958d79095685fd9be422c2bc44dcee57663d820"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_buttered_oat_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e96ee7da5d692a1156b701da6e9ade6b60ade2752cdec3327d0b2bfe82a36dea"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cabbage_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fca72c28d183a9fe0bcb3b8ac92802e7fee24d1f8f226f826626da7659e5e58c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_carrot_and_cabbage_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1bedf3a6d0b75157020deb717d6a57575e7664656611135e04733e73dcf26438"),
	];
}
