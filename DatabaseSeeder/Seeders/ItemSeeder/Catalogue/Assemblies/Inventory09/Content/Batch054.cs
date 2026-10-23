// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch054() =>
	[
		new("item", "preindustrial_food_cheese_toast", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7ec644427639e2e5163cfa0583847e9af777849e1876a8e68b5e461764076ed"),
		new("item", "preindustrial_food_chicken_mushroom_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "26f985c5afafd0cf8d7e6ace3c4792ad5038570255ad0921386e26f4794416e0"),
		new("item", "preindustrial_food_cleaned_mollusc_meats", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d7c5c96c8cc705fca6ae97679163aa3b7e3297323878424f2a792123e2d3094"),
		new("item", "preindustrial_food_coarse_chaff_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0bfb38d887f84a0e09e797c1c109bb8dc9f5899f2afd79106de8f1b87371ba1"),
		new("item", "preindustrial_food_coarse_meat_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eec3507a33896b81a20d4144d6f7ca35a285e18589fd9967d46a2d4385a1f0e3"),
		new("item", "preindustrial_food_cooking_fruit_pulp", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5f0dd44265aec65dbfd849a3aa8b68b82e9f9f437a653e4c0db2c6e2016c014"),
		new("item", "preindustrial_food_crustacean_barley_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6945e80f0c410cf48855bf9adb39b51ae63932915169190f966557494ace877f"),
		new("item", "preindustrial_food_date_spice_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "490cf9e749a8b6a690322454c6ee33c3442313290a2f85280a44ba1442a9a72d"),
		new("item", "preindustrial_food_dried_meat_and_nuts", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "72532804b77d00197405d6221b6634e05c804ce3b5b9cf2bda821d692c93fb89"),
		new("item", "preindustrial_food_egg_rich_griddle_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a4e595eb0e59610f76543cdeb3a384b071ec7bcbbb596d08e28233a6e9aaa6d"),
		new("item", "preindustrial_food_fava_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75bd775ff8f303df2d78268884fea375775ee772d6e3f30828b8c27cd809e5f7"),
		new("item", "preindustrial_food_fish_custard", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bd49f19c9329b33386053c88bedc1cc10fc1364d27bb55038d4b9743e7dfed5"),
		new("item", "preindustrial_food_fish_pastie", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2664255f85e98124d8cee314b6870a78866a6f346829acd05a70ab2a92c28e66"),
		new("item", "preindustrial_food_fowl_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a19a9a2548a7173c7ff9241356afe0607b0634d26f77856956fa8974ddb856ff"),
		new("item", "preindustrial_food_fried_wheat_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "79f66b9cea4a81b85ef4ba62a7402e2e33b83c42e0dc2ed2ddbdb111874521ae"),
		new("item", "preindustrial_food_goose_and_cabbage_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f10a03e8ffb534768819e521c63e620ea1cea7a7c10065068c400195ae053f1"),
		new("item", "preindustrial_food_gourd_with_leafy_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "14cb655fb9b05a42d35fb246d9aca6e08a62a627cf0784157b7803ebcf04479a"),
		new("item", "preindustrial_food_greens_filled_crepe", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce03aa0fd07cbda40e53fcfcb5f0d265bb467c01a011bf8c4e7c3aae6fcf017f"),
		new("item", "preindustrial_food_grilled_fish_fillet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "477db9a1d385984a404525242b7a2b94e39b22e066888ca18cc4f33481256db0"),
		new("item", "preindustrial_food_herb_drop_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "767db48fee160c240ec02e426798c15d3fd6e6dc10fb05a206fa5dd273947f61"),
		new("item", "preindustrial_food_herb_poached_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "787de59eef2b3fc829f4f263f791047e4b215a50681d088592d4d8862fb41501"),
		new("item", "preindustrial_food_herder_sour_grain_round", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a6d02406414b995bd8b0352810e71a6d8d3853d08c12ad49456d3ee6512af53"),
		new("item", "preindustrial_food_hollowed_stuffed_turnip", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1603c3fe93493019a88ccd2194bd95c4935f6709a3ed4fc4033151cdd21459ba"),
		new("item", "preindustrial_food_honey_glazed_cured_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "558bbfe760985fa2b7580f2d1180d906e372abee6693ad299ff490f46e39d85e"),
		new("item", "preindustrial_food_honeyed_dried_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab6156d3378840d68aa1fb13bd8fecd81cfa8a33128bce24f34ad75ec6caa99b"),
		new("item", "preindustrial_food_honeyed_fresh_curds", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a05c9d87b9c5e94637da056a3f59bb67e17373d423a49a5deaec8914c0a14de2"),
		new("item", "preindustrial_food_lamb_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3fe2f89334f76eefdb2cd3f809aa7c73a2c54a749619db3391ed00db51288d3"),
		new("item", "preindustrial_food_leaf_steamed_small_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ce41fe12ca6603aca8ff4f0f389ccd9ecf316b583d996b9f9c5d43435dc05bd"),
		new("item", "preindustrial_food_leaf_wrapped_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f9b4bf704d3e3652015f4deae0366f1488fb80f4b173278b4e72cc56453744c"),
		new("item", "preindustrial_food_leek_and_onion_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82f340c31792b33e85ae70eea56b7d6a8b03b2bb59215c4b7db6bc97d96264c2"),
		new("item", "preindustrial_food_leek_batter_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9d96ea5853d66093f28ecea1344ffc7d720c23fe650fa8ea207632874366ad2"),
		new("item", "preindustrial_food_leek_herb_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "70e3c4f2e874bb394acbbfe8ed0a79ecdf73c940e3c343f1d9739e8a2968a783"),
		new("item", "preindustrial_food_luxury_preserved_fruit_box", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca2f020bda34f0e263ea9b63b8be4da502a449a2ecd281acefc23b4827d2abe6"),
		new("item", "preindustrial_food_marrow_on_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "24557578128dce320b8a5a315ffa810f3c10cc369b83601d25d143df76aa8f8a"),
		new("item", "preindustrial_food_meat_fried_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3782523d8ecfc700bec217b4ed0078e6e234ac73ebc65720425a1275246f6356"),
		new("item", "preindustrial_food_millet_lace_cracker", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d006d1293d6117fba342b222226a221a6a161dafbd486343bb55b39238492c0a"),
		new("item", "preindustrial_food_millet_seed_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e76dc422232517847334a42c973c0a85cdc5bbb39c547d13112668fcd7741e9b"),
		new("item", "preindustrial_food_mixed_dried_fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5f1e4d31928150719755960cec47e1ceb7b2a2712adf3d635be7d12cd898a20"),
		new("item", "preindustrial_food_nut_grain_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6d74e6eebfc9193a863ef712245f7945ab1cf32986b61ad596ebc167d452732"),
		new("item", "preindustrial_food_nut_rice", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d40130cc803cfcdf68b263d176067964c893615353084299b2a51bdcb04d1071"),
	];
}
