// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch054() =>
	[
		new("item", "preindustrial_door_screen_06", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "55e44e43beb922797faac423ffd18f2a68deb237ccd3d6165c85da342f5952cb"),
		new("item", "preindustrial_door_warehouse_gate_09", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9239e00f43d90cb97cfdfbaa937942f66205f35bb94bbf07eb224d145e2439f"),
		new("item", "preindustrial_food_acorn_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c50e83879b7335f726630abc31b0beb4f2e63a3a0139640178e105f3fa99733"),
		new("item", "preindustrial_food_anise_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1df408e2eca52fa8b8db484af99dff15b38da17133f0f68f5f2cbdd1ddb2f7b6"),
		new("item", "preindustrial_food_army_grain_ration_disc", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8636e704edb05e34a1c33a78bf5d84e4c58eeba73438ce9694133ff516fd03a5"),
		new("item", "preindustrial_food_ash_baked_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a44e0a5e6cd2699705c64c7f12cfd03740c943f2e812e5e4cd1d634c859e9093"),
		new("item", "preindustrial_food_aubergine_and_chickpea_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5869354c08e9a73eb74e5a792a0eac846ccfd5a32358b82cd773aef5597ad742"),
		new("item", "preindustrial_food_aubergine_and_lentil_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "449a7ff6a5c50a780645d60226ddd490f5de7851ca644baefaca2694600087e9"),
		new("item", "preindustrial_food_barley_batter_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fae43881c6cad9f124e3ab931da76d295392d7a3dcc4413d432f4596a1b31372"),
		new("item", "preindustrial_food_barley_gruel_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "420ce2cee6e18240c6b101c41d30edb5db2c31ab02b0aa0fb2177695489597ca"),
		new("item", "preindustrial_food_beef_marrow_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f76a9879f02cc67ab3a15b61ccbe101bc7433ebd8cf6fbadea812dead69273cf"),
		new("item", "preindustrial_food_beetroot_garlic_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6408e3cebc1f81215cc13d934a9c7cfb7a78f481d196b5aa5c1d249fbc7826e"),
		new("item", "preindustrial_food_boiled_snout", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "52339244095a711f31f1c4be376638763d109bd87d28168295081225e53bc8d1"),
		new("item", "preindustrial_food_braised_radish_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "116d36334e7c997d2feec5d5ffd5a135848e05cdac2e6d9e71af1cc3298ebd4e"),
		new("item", "preindustrial_food_bread_and_herb_torte", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a409bbdfd2ac86b1671bf9c20c33051ebe504d684eeebe8c1e1691fa291f6e88"),
		new("item", "preindustrial_food_bread_stuffed_fowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdf596fe07119ab669e959785a0a0f2d3c5b77c990f53d1e6c19020c400be63f"),
		new("item", "preindustrial_food_broth_poached_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "941a6bfa3ea425413bc632547ed337ed0f6df6cb203730294827b894dc8bd690"),
		new("item", "preindustrial_food_buttered_grain", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "04a6215c27f2ea194fba898e90c09029d205281666c5a031ce8f17fea52b4c1f"),
		new("item", "preindustrial_food_cabbage_herb_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb9d7d2a52da19d09fb3364e0e2734e66b4eb28befe8f2c1804c6a3fbedd11e1"),
		new("item", "preindustrial_food_caraway_rye_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4cc8bb821a0ad6af6b2426db396c9f6fbd26a992ad7f36151c8fe8a8c6cf6bcf"),
		new("item", "preindustrial_food_carrot_and_onion_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2800787ad1212e5bd764b575fd1c8efebc94d373207d676ee6766ba261c1ae46"),
		new("item", "preindustrial_food_charred_fish_steak", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b82cc0361f9075a53c9fde7a4baa282ae190c13d7750ac79d0f459b352d9bc9e"),
		new("item", "preindustrial_food_cheese_and_onion_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0276fbde602cf9652fa6361dc1282f9b78c8030b41e0761eef2545e88286c821"),
		new("item", "preindustrial_food_cheese_fritters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4a9af8524b29feaaf3b77b2a7f1b2b4c12a01fbaa26d9a5379cd85f06848991"),
		new("item", "preindustrial_food_chicken_herb_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "03a442aba6a511555e85ab135d716c5843ef36f973af68bde48836e2a9663a8f"),
		new("item", "preindustrial_food_chickpea_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e35ccd74d26a69e42b9ec524e2c061ceb0179bc990cb86d095a20005e3dbb530"),
		new("item", "preindustrial_food_chickpea_herb_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87a5e868e4e2d9fb15fa3aaa4849d3cce061533d38997b04758862aa9c0ba6f9"),
		new("item", "preindustrial_food_chickpea_root_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3757071a5d05d277c7a614ff5df916911f3859c82a25512e6e53634c1f74278"),
		new("item", "preindustrial_food_clam_fritters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "43643420f8d2050c4dc5671a478a69f794898784b484670038af9b0882ba351f"),
		new("item", "preindustrial_food_clarified_butter_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a90b280eb490d9a7652d5f14a2af2e447c215228c623e2cb86dd6cbf1e4d29e9"),
		new("item", "preindustrial_food_cleaned_roe_sac", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ac868e852869760835b5c47120df58a537df1ff44847648c269086e968143eb"),
		new("item", "preindustrial_food_coarse_barley_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f73b52981b2091566fd8c58309d5a55a70d549e6a42c6772eed454e1b690b9e"),
		new("item", "preindustrial_food_crustacean_skewer", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc3df00242ffb8ea424a0bbe4c71a15d038b5a5e31611bed049191818fd4eada"),
		new("item", "preindustrial_food_cured_meat_sops", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b00e4ca44bec1d8e73d5e3d5c587ad6ebaffbe6697652035a2b3efcb30df5f76"),
		new("item", "preindustrial_food_date_paste_travel_bar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6d0a0efb37da5490f16c83f84b9c6b0fc0165f1a78fbae3b88eb03d1f8836d8"),
		new("item", "preindustrial_food_dried_berries", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed6a042d2e33920c7b5186be7de8e20991430bc950e15fc8858abdd2f1fa4d04"),
		new("item", "preindustrial_food_dried_fruit_leather", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d14302719f4850b4a651f3e04c617960aea6abbf2bceacc72941df7a82554613"),
		new("item", "preindustrial_food_dripping_toast", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf6d5513dad1743c0437070e728ffe5299254773bf287e0b68e0ae663aee4827"),
		new("item", "preindustrial_food_egg_baked_bread_custard", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c66dee081db06e3b82903db711a3c7daadb4ea28677f9ddeeb97126f8900df54"),
		new("item", "preindustrial_food_egg_ribbon_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "34ad7837c1e680274c3589fcd727e5a748f578bb1cb7501304cc876208e4598c"),
	];
}
