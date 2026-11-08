// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch056() =>
	[
		new("item", "preindustrial_food_onion_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53b649f410990d95587a2cfa4d570aab04df125c6792d9a22ccde6532bbd99ea"),
		new("item", "preindustrial_food_parched_wheat_travel_pouch", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7fadf96627f1f5f653bfb05db23a23928c4af8b0fdad30d2af736a0aed63472"),
		new("item", "preindustrial_food_peppered_fresh_curds", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a81f342992f2b211dda0dafb22f9a7f9aea426d688c7a31999ee45c30648531"),
		new("item", "preindustrial_food_pickled_mushrooms", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b79fd26666537c5c18fdfbd101742062214d43612fb805b5933fd651ad4a36b"),
		new("item", "preindustrial_food_plain_boiled_goose", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "00bc27ef68825110019f6733d8285153a7d39556896e529ee7f1d8e658f8de43"),
		new("item", "preindustrial_food_plain_fried_liver", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7402dbfe49dc9832e34afa2e0fa48bff2cc35c67003fc840ced31d2900417f2"),
		new("item", "preindustrial_food_plain_strained_yoghurt", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "945e30b72ba0724fe986a458eca1338106f28337441d55e56f6377f168266246"),
		new("item", "preindustrial_food_poached_pears_in_honey", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c124fc0af31b06cd2d2a486c4e20d855bf6404f2e69618c00f08ca5a13001b1"),
		new("item", "preindustrial_food_poppyseed_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61a88df6de497312d919840a9e05ef910c7e9ef60ad6517abd24834c36d2a1ba"),
		new("item", "preindustrial_food_pork_leek_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6d51a0eac73c8dab3b5bd5efd369eaa0e6b8e267af36a6f6801cf88a808b23d"),
		new("item", "preindustrial_food_prepared_intestine_casings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eca362057e998bdd6444b242396631499d112c3b46515bca6c6e89fa8a4b547a"),
		new("item", "preindustrial_food_preserved_meat_confit_shreds", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be16375bf8d56b111f30b1d026e64fad27b73e1c592a6b6cdb8fbc2b02a73d9d"),
		new("item", "preindustrial_food_pressed_boiled_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a80f97782fa7564edb366f49be153f0665e0cde3d216f1592f4e34eac38a499"),
		new("item", "preindustrial_food_pressed_dried_fish_block", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "08b12eae43685c72e08fa789a9c7bfc768639e11e86dea9e9f42aab376d54d47"),
		new("item", "preindustrial_food_pulse_grain_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8cae4eded042b255dcb75436886a4cda088e675526f9156010d5e7564d115e4"),
		new("item", "preindustrial_food_pulse_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "10cc1e579eee578bbb876163212b26e4d3d81ddd549460431b2dccc594ee1cd7"),
		new("item", "preindustrial_food_rice_and_greens_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9fb9b23567ad0368356b4f89fe90bd7f24c137ac7d7614e4fb8423e8b55ea5ce"),
		new("item", "preindustrial_food_rice_and_lentil_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5cac7c0ae509e9acfba3c10ec7bd6487b5a643315be4a0356f7334d84ea7318f"),
		new("item", "preindustrial_food_rice_flour_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cab33c8551460d5e623e679e0df5e78bbac4b066944f658301123978bee61804"),
		new("item", "preindustrial_food_roast_goat_joint", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0335c000fc7fcf153b08be9bddd31c7405b68e024a803e8ea1bcae68b70d15d6"),
		new("item", "preindustrial_food_roast_partridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c3c4b79bc8a02dd9d717f8e7272ea934028a3cc20134cd35127ac5e37f0220a"),
		new("item", "preindustrial_food_roast_quail", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "faa2b8826b835b5b31dd043b7f15824d05cb946b86575d25f22b55ed23c71070"),
		new("item", "preindustrial_food_roasted_figs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "63ba37ff044b1d45f255c23cc25894d935d3f5ebee361ecc69fabd783954d0e4"),
		new("item", "preindustrial_food_roasted_gourd_slices", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd64bcc83ef943147b9fdfd18edf9d2f13e5e896a0ce86ea8882bd3abd2022df"),
		new("item", "preindustrial_food_roasted_mixed_roots", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "834d726d2ec40ce34d30bb952457a40ab8d7a0635ffcf2a9af1c77b0a19da8ea"),
		new("item", "preindustrial_food_roasted_okra", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "97e150e5523e6ef80b187c37d0b6ea60f7ea9423c4f4bbae9515c7a9d60c2d57"),
		new("item", "preindustrial_food_root_and_fish_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d73341b46477d726122c9c3530f9ac14e5eb64009243b51229a8e4febf495ed"),
		new("item", "preindustrial_food_salt_baked_beetroots", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d84077f2a26b09cf38d5a1aa3090b665764bc569a2227e66ea05f375a2124ff8"),
		new("item", "preindustrial_food_salt_crust_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "97ac641bf511aca318a0ae82a14c236270f96f9df9903474296e76f3455d0032"),
		new("item", "preindustrial_food_salted_fish_roe_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9ad629e0e5a0206f6c76567859e62094b310203c443c36bb27b2ee50b95abe4"),
		new("item", "preindustrial_food_salted_fish_side", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cce8efa849fcb276429b51cbc098f4847f772dbe8d01abb53528eea6a14c7f77"),
		new("item", "preindustrial_food_salted_parched_grain_mix", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4f597c042f0e7d5b2a03485540179a997f320c19b6b16050b0bd3e638827fbd"),
		new("item", "preindustrial_food_savoury_dumpling_filling", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3586f59a91ca11ab29a728bb8608157dca448f12e88460b9a43d1166a6f19194"),
		new("item", "preindustrial_food_seared_scallop_medallions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "abbeae67bd6eefe448abd70b2e49e426f7d1b65784c0459099bd57c418609066"),
		new("item", "preindustrial_food_shellfish_barley_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcbddff3dfb9a78f3615ca2fb29932a8faf9d136b93996dd794d0705cbac3628"),
		new("item", "preindustrial_food_smoked_game", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8778ecc1d99b6316c0d1e7d295300d048f1f8fcddeb1c3eaf80fbbe01499988"),
		new("item", "preindustrial_food_sorghum_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "63b31a5e29505ca5e2297c6e4bd8a2312e2ce47d4b770ddf21036d29591e6e47"),
		new("item", "preindustrial_food_sorghum_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "10ed847c0229c82b8f916f40ff3488c1314dc0aa6880e0c98a3ff9afbbe42ea4"),
		new("item", "preindustrial_food_spiced_feast_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5ad00a96eeda7962881326d5e00f077b3fd65a0ecb8797a2f0944d3aae15cfb"),
		new("item", "preindustrial_food_spiced_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "088938c2f04b52167b02624294947e52b3fa9b3d87a4dedc4aab9e53fa1e8603"),
	];
}
