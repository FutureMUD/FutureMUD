// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory11Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("craft", "Writing and Administration/make sharpened reed stylus", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_reed_stylus"], ItemSeederOwnershipPolicy.StockAggregate, "da347020bb0d67323e75eea316fc2c08da012bb673e87b6ef38b513d31ba9b45"),
		new("craft", "Writing and Administration/make stitched account book", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_east_asian_account_book"], ItemSeederOwnershipPolicy.StockAggregate, "e180b46409d3f834ced57b56284bace908c3742a6c388f783d8d0aee57758df1"),
		new("craft", "Writing and Administration/make wooden book press", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_book_press"], ItemSeederOwnershipPolicy.StockAggregate, "fc734e505f851338e107e889a1f922a39839b81d0531f186018130c65b10b622"),
		new("craft", "Writing and administration/bind leather-bound commonplace book", "crafts", ["renaissance"], ["item:renaissance_writing_commonplace_book"], ItemSeederOwnershipPolicy.StockAggregate, "16d7be9e8176fbdb8e2490c4534f9bc8c5ad1f81d47793b6bd578a273daf0214"),
		new("craft", "Writing and administration/bind raised wood writing table", "crafts", ["renaissance"], ["item:renaissance_furniture_and_writing_table_02"], ItemSeederOwnershipPolicy.StockAggregate, "01d693d3dd78095a1867c14869efb0ecf027af04fa64a77156eed14f9b486bdf"),
		new("craft", "Writing and administration/bind red sealing-wax stick", "crafts", ["renaissance"], ["item:renaissance_writing_sealing_wax_stick"], ItemSeederOwnershipPolicy.StockAggregate, "cf6c5b12dc258abd8ff30b2dc94135af7266f4790db2928a37a2105dbb2337c5"),
		new("craft", "Writing and administration/bind shoulder-slung portable writing case", "crafts", ["renaissance"], ["item:renaissance_personal_elite_household_portable_writing_case"], ItemSeederOwnershipPolicy.StockAggregate, "9d7ab5d264417d5ba622ae2e8a923a61c8e9ee4f8412727bad7f94aac5790f8d"),
		new("item", "adjacent_antiquity_forward_pointing_felt_cap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f654d8bc39a60ce4c3cfe38f897056f7d093a200555536e393f4a2ca53714470"),
		new("item", "adjacent_antiquity_wool_craftsman_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c70632207fa41b77d3bb514d702b5b4fe2bb448c8ae996f180d08ba84f67ac65"),
		new("item", "antiquity_accessory_scythian_axe_loop", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2639084c7e2a9bfcb0aa87b9a2c35cb51d6802760fc3d44b3c8d622980e9c78b"),
		new("item", "antiquity_ammo_common_target_arrow", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d67725d4ffbe12d4fca9ba4d4398f09578da6989d1db1c8b1eaffcb8b250a99e"),
		new("item", "antiquity_anaxyrides_wool_trousers", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "444af8e1ee669125c4e137e92b8563b0c6ae4e60db19e575f0cf82414e2544ff"),
		new("item", "antiquity_bow_drill", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fa46ac53ebc111fd7fad5ce4b70c9ff1e86107e7fb564a9b5b127c0a17a7830"),
		new("item", "antiquity_bronze_dehairing_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "77f2b382d964d0a8c9e51eb3119d244841ccdbcfa6ec1940e1f310f7c467808d"),
		new("item", "antiquity_bronze_sewing_needle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "31d574a3f0f83029b945ed05e0d84191380e6699f489c19c52cb5fa613b567b8"),
		new("item", "antiquity_bronze_workshop_pliers", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "118463ba2df03e12a31177422d5a226f86d8d1587d0df97fedf9e248923f588d"),
		new("item", "antiquity_cedar_scroll_case", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f290c490a4b4c2d4170e2127b16938ad1c760e12d18f569b234b0e24cd2bf9da"),
		new("item", "antiquity_checked_wool_hanging", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4321ec0755f9cbdcde902c8fc317080e81db98e4a0a75e25151c85ccef535616"),
		new("item", "antiquity_conical_felt_cap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ee5afb8c5cf7be04dea5115655cacaa03b6d6c3823c0d6473b42c59fe316708"),
		new("item", "antiquity_divination_lots", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "495d2567cd60c0c3153ef327e39d8ff30663086a25efc6c202e2ea9f962c5e40"),
		new("item", "antiquity_etruscan_cheek_guarded_bronze_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d87107544adae9336eec327d889d59d7d507abf39706547a9a7c60b276c0678"),
		new("item", "antiquity_fine_patterned_cushion", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fbde2901234d5db5c0516a0a6b1b544b1d3c0773782576b2fa98bc3a4d5ccfa4"),
		new("item", "antiquity_fine_woven_sash", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "66f6ac9218d9cce1d273e2013ebaab4569e24d21bb726b372323c2378c5a27ba"),
		new("item", "antiquity_food_aging_spiced_beer_amphora", "antiquity", ["antiquity"], ["item:antiquity_food_finished_spiced_beer_amphora"], ItemSeederOwnershipPolicy.RequiredRelationship, "25348e9c8d4c490cf63f2a116e312cf2c43508a47243cc5ba77e8515cc353053"),
		new("item", "antiquity_food_celtic_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "074d3afa4774318d3c6b3c526cf2a052fcec1ebb22f5517bf7f1d6156adc15a7"),
		new("item", "antiquity_food_finished_date_beer_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "09757fb30b8636c67f131ae2801429d1ef05f4214f082f231b0d82054f1708da"),
		new("item", "antiquity_food_persian_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5b94507acfcb6292dcf681dff382daf227937cead8ea3bb5145c7af16db2b83"),
		new("item", "antiquity_food_scythian_sarmatian_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6d200a20a43d8068c509ee0aed3dff5a1f1817fc9d944adf3f6e36da36edf91"),
		new("item", "antiquity_fur_throw", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f0df97e59719936554ca1a242b6f3c86387afe6fc7ac6d0f6eb8060c0091c4b"),
		new("item", "antiquity_heavy_hooded_wool_cloak", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c919c33eef6ca60e71d055795074b66e20f7c3202e6fb53ebf5c53580f1e9aa"),
		new("item", "antiquity_hellenic_leather_pteruges_girdle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbd603e1d1811c9016d937b8473c354ce0ad60a1fdb715af94332dfda25dd244"),
		new("item", "antiquity_linen_door_curtain", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bd7a5c000a1be34e3c7d05c0a922882683fd29e7f7ca4feb05942c9bfca29fd"),
		new("item", "antiquity_liquid_glazed_wine_ewer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c54bc4296b93d2df96a153de0a488340bdc7dae7b519df55a9dc721ed912bd0"),
		new("item", "antiquity_liquid_hooped_trade_barrel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef1335a0ebaf03b4c364557fb2ecbaa23536a655b3c4853839166aed13dda32c"),
		new("item", "antiquity_liquid_oak_hogshead", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9d6843c3750f63d5b4fd861470c5e2008d8e4e5706299b0cfa2c515084ae8fd"),
		new("item", "antiquity_liquid_steppe_milk_skin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cec80d4b6caa36ef2d5efb31c87a003e72ef5501deedea41739977d806128b3a"),
		new("item", "antiquity_liquid_white_ground_lekythos", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c327a50632b963ba15419c5aa391d2eb605f6c2aa38905c44fc2ae8a08d08b6"),
		new("item", "antiquity_lit_clay_smelting_furnace", "antiquity", ["antiquity"], ["item:antiquity_clay_smelting_furnace"], ItemSeederOwnershipPolicy.RequiredRelationship, "bcb22e77520c6c94878e05cb04250f0c45cb11f5f93549f8b5a462d268941e08"),
		new("item", "antiquity_mule_pannier_set", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb86abbdc0aa587874b0ba201ab77519444c2725d5db4645459c4cfdd4f1b9e0"),
		new("item", "antiquity_oak_shoe_last", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "639083c4016c0cb742d55cee7fd98bbbaae294bc579d78bfeb665a34b762af74"),
	];
}
