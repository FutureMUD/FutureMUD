// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch054() =>
	[
		new("item", "medieval_writing_silver_leaf_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab55b3ef942a4663bfead8aef337e0e4a5f4f34bf17508a73971a95110a8d7a6"),
		new("item", "preindustrial_artillery_sponge", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66817a14e09b79e2907399440c7184ec463cb5f9209f0b248d06f7f8f3a67702"),
		new("item", "preindustrial_door_bamboo_screen_hanging", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3146ee725788c04add52e5d6e34116b8c266221e6e95c2e816726f0c298eff2c"),
		new("item", "preindustrial_door_door_bar_83", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed732ddb7e2cc294b73765bdbb30f1b6be955955b7351baac5fb940819773d4c"),
		new("item", "preindustrial_door_door_latch_66", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7957956d165ebc0f4a82ec74bc1336dfb1379ccdb2e8e2ed2cbee1ce2de0f4c"),
		new("item", "preindustrial_door_secure_door_18", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3d57cea8c8a1c0765e4465b2ded6ad65983902eab997367f8807aa544eb3085"),
		new("item", "preindustrial_food_apple_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e10ea722f8d2abec1ed7609903ab16c2eb405f4ab6c845172fa276329b425a9d"),
		new("item", "preindustrial_food_aubergine_with_onions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "98c92bdd4e856a7d3d615fc8c4c2ee20028f26f5c906319d1b6864f17e0ce50e"),
		new("item", "preindustrial_food_baked_egg_dish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "99cc2aabe9e10fa9bba8c1e6a8c6cf4e36996fe711714fc41f925db4dabcdcf2"),
		new("item", "preindustrial_food_baked_onions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82b11a89e5aaa7aac8d51a95cfe36476780b138d1eab9711c3d08b7396cbe061"),
		new("item", "preindustrial_food_baked_stuffed_clams", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a831f1800a933582f0e2a45c6a0e888d4aec54e05ebdc0e38045d1afc8a84741"),
		new("item", "preindustrial_food_beef_and_cabbage_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ba9650fbd82bc234592d517da1bfe5d847c264f9a5e1f2dfb60cd2943c4bd38"),
		new("item", "preindustrial_food_beef_herb_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eaae49223b23a7232ae4b2c469a32af3b39c8bdc6b3fc67a0ad0cf31a025b6ee"),
		new("item", "preindustrial_food_berry_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a25ff46d36e6e33fd8e52d61809640c99f54ee53fb97205f5964313541e8abc2"),
		new("item", "preindustrial_food_black_bean_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c02e1e3d14301e7c1ac67cf4911e3b04c89fec943680ce7fb1f07bfc243c6b1f"),
		new("item", "preindustrial_food_blood_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd0685a1870363a3589392455bd248a9147a606d5b14991c1c4d727c64cb0b7f"),
		new("item", "preindustrial_food_blood_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78f79aec6e7d62d3f3c72bbb6a7edf33ad2f61ca60daad8d66c00fd3f763b8e5"),
		new("item", "preindustrial_food_boiled_asparagus_spears", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b214879eb94329efe8341869ddbde4ee9bba84875618fb6b070206cf381aeafc"),
		new("item", "preindustrial_food_boiled_grain_with_egg", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c3585e39675065970fd2a30104f71d057e255a10fbf558590f88f73c9423cda"),
		new("item", "preindustrial_food_boiled_hare", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "360cb15401e58baf0e29836cd7504b6b665daef5f65abca250a080118c7addad"),
		new("item", "preindustrial_food_boiled_millet_bread_ball", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "247908ef3e0655f3fd3bd6fff0a603615b12a4af8fea96342c4c9c33ff6d642e"),
		new("item", "preindustrial_food_boiled_tripe", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53ca1ffba003f5ceca479c7e088e5a035081951d4c70ac492b62a84b67a0591b"),
		new("item", "preindustrial_food_boiled_wheat_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e66d8e3096251a22e824ed1a3fc33be0b11c4e08263d3983a9eec614dc385e0"),
		new("item", "preindustrial_food_braided_bread_dough", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b6a8be6286129e44b441770ba2faa428bb4d870833ee1bfaad6594e5da0f4d5"),
		new("item", "preindustrial_food_bread_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8529b869b0705e3b23309796c4cf296d5c3e86c911b14a281a3123d666082ff"),
		new("item", "preindustrial_food_bread_meat_sops", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee3a78519499015183bfec5f544e537cc04d31da41a02c0dbf7c133c84c56750"),
		new("item", "preindustrial_food_brined_meat_slices", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0277e4c87ae46677b3ed9e3964989467dbcd48ab595c6a8cce5917aa7966732"),
		new("item", "preindustrial_food_broad_bean_herb_patties", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "43356d1876da1c34dda09bf20487a1a0148c0256ee3a7cfd30d7f109a6936bd0"),
		new("item", "preindustrial_food_brown_lentil_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "638d68dfebaa25cd5f1979b27c2cdce3267c6890cefc7d7316fa6e1007996455"),
		new("item", "preindustrial_food_burnt_smokehouse_meat_ends", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d3f731a3725894523085147fc404f3875e06f13d6d5ad3fb62ab0fd92467ae4"),
		new("item", "preindustrial_food_butter_fried_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "668945b31d63f70f921985c485345614211fe8b6799fefcda232bb0fd46a34f4"),
		new("item", "preindustrial_food_buttered_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3aa6761023b3a839830b5586f7ceb2e18b183a29e064cfa072e2b7bf532f45c0"),
		new("item", "preindustrial_food_cabbage_and_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8a96d8a1a26e6b3e77b352f08f523236049b9e8386e8b6224785b5fd606fcc4"),
		new("item", "preindustrial_food_cabbage_and_pea_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e61330c61a921542064337f5627bdc1cc7375e9e8141fa5051471d202247819"),
		new("item", "preindustrial_food_carrot_greens_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3dcdb0256926b098ff18b01025c5b94d482882d530267ef43912d113d2c4bc4"),
		new("item", "preindustrial_food_carrot_with_peas", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "007544b4e3c9ec8dcd0bf7c73ba101e18392850b1624199ce2430713363c67e8"),
		new("item", "preindustrial_food_cheese_crusted_roots", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b11ef8b8b1605b18276f6176204631b2620ae30e94b7dbe7ed113204ba5517d2"),
		new("item", "preindustrial_food_cheese_in_oil", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "edda4994bc698c0b201e658d06c5dfcc09b2d2b68353aa5123b5c9fada85b5bf"),
		new("item", "preindustrial_food_cool_smoked_meat_slab", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc1221d796c606e753e8cfee0b28567a699a95ef6acfaf10b085b6977d964678"),
		new("item", "preindustrial_food_cowpea_leaf_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d1b915860cc07a8f9cbe0c57fcfaf3cf9bfb167a0484b57d07ed738b9dc0374"),
	];
}
