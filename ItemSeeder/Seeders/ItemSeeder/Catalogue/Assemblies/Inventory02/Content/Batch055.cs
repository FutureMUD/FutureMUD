// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch055() =>
	[
		new("item", "preindustrial_food_crisp_fish_skin", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a59bffc859bb69cdbbd325b87e4f627791a5b34acdfb503b4fc986346490e98"),
		new("item", "preindustrial_food_crusted_rice", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d63aa80ea18511e2d3bbb51f78652263998f959719a05374958ddbfd9cdcf8a6"),
		new("item", "preindustrial_food_deep_oyster_pie", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a20b7c2bf058e8eb1ec65edbe2d10c498deb2e3299ec16b40fcac5e19ebc42e"),
		new("item", "preindustrial_food_dried_figs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6b22e3326a9ebee68515ef99e9546b9658936ccdd753c1a396a31edc7d864be"),
		new("item", "preindustrial_food_dried_meat_shavings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "832b0ac2dde39d4d947ae766781a3fe210d071ece5226303c156fd6e56687d0e"),
		new("item", "preindustrial_food_egg_feast_platter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0264ba47ed625c895d5fecd29467936dbeb1689c1afe2c0c2d177b5bda624af9"),
		new("item", "preindustrial_food_fat_mixed_dried_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f0237b4fd48ce0781d5134e971c9be85c19b751f294805df13668b612233a36"),
		new("item", "preindustrial_food_fig_and_grain_travel_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d688c2559224e5d175a69c8b34885f88ce2ad37d3e49772cbc7ad3c1862672d"),
		new("item", "preindustrial_food_fig_seed_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a48cc59ad8344fd58d63b9b174ed4150ea72388e3b2fba47894aa4c8821c9d6"),
		new("item", "preindustrial_food_fish_and_barley_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ff4ac8f7ac3279d1be6114d16e4dac2aa6dfba1395a2daabe6b432c91be6dd8"),
		new("item", "preindustrial_food_fowl_and_onion_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2568cf1dc6bc1e4a56ad3bc9c70b5974b4e0db8342ea61147b8b5e8ea60aa917"),
		new("item", "preindustrial_food_fowl_back_broth_meal", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e33dd4598a99c10b5e2472a9232506bfa798b777f15a10baddfe8020bdbcd70"),
		new("item", "preindustrial_food_fresh_fig_plate", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9fd5de9c763f3a488bbf6eb8d3f9d5cf21fc376a2d15f68c5f2cc8c621dd153"),
		new("item", "preindustrial_food_fruit_paste_finishing_block", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01c5d7df10be2b531826d627f804d8adf73cf3ae534b7bfede29be3eb5e60d17"),
		new("item", "preindustrial_food_fruit_stuffed_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5524a66798c959ebdf2f4d187be8a57b1f9688d53bbd144ac9d7d6e544c5d43"),
		new("item", "preindustrial_food_fruit_sweet_feast_platter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fc7f79b1f1e2d94ebb7ded6385590a6101211bcba033deee832c3f8f5992b96"),
		new("item", "preindustrial_food_garlic_crusted_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9ab4347c0c797e9da26e87c9826e4da1f73d5ee8084d9d95fadd2baded8c52f"),
		new("item", "preindustrial_food_garlic_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa979144fb89506225f83e7643c59f13c1ad3596545334ba6c24437b63be3b8b"),
		new("item", "preindustrial_food_green_herb_shellfish_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "43d17f0aae7094b1c175223231b22113655392f09f40fb44fd3545b0048fce44"),
		new("item", "preindustrial_food_griddled_cockles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a8a3bc7cf0dfac319f5fde971de14646901eee7e11b5a680c5d2837d4142032"),
		new("item", "preindustrial_food_herb_butter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb93ce3557336ca115ccd2457596fe69171c472985ade87c647df2ca70f27c3c"),
		new("item", "preindustrial_food_herb_crusted_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e952a80189267f23aa681da49df60e99f61602bf2e0e35302e5f7dd943532163"),
		new("item", "preindustrial_food_herb_roast_fowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "831831ca4e23cabe580a88f9ba5d6ac735855ae13f3233e2f19896f1d18e5c55"),
		new("item", "preindustrial_food_herb_stuffed_baked_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c0eea5bb36ce0dd90bbf3f890c6f312335822af98bece0b57d36b5aa1be7273"),
		new("item", "preindustrial_food_herbed_boiled_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1420b0c5f08d8f5a6e24729f33e9b7d303a44f6a909c5684e2fc17123037ed7b"),
		new("item", "preindustrial_food_horse_gram_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "287f669af715c30d9a12e236ddbfd5bf84604f5cc280860c93b08c02f2802cfd"),
		new("item", "preindustrial_food_hot_smoked_fish_portion", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "214d1ef80cb8874654bf94ab9188306e421a58f45a1b0624cc281f8d54c1aa5f"),
		new("item", "preindustrial_food_lentil_and_onion_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7796ccbd775ddf17b03c580900693bfd527829f0d974cdd98b87b6abcee05d4"),
		new("item", "preindustrial_food_lentil_meat_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c8aaa071cd15c51c83fa99cb69561578fa1d8c0657e49ce6c62116ae9baeacb"),
		new("item", "preindustrial_food_liver_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8381e8eb2fae5aab4e29dcbfcf1f53b62bbbc2ffdc863f80a8b8af0d09e1429f"),
		new("item", "preindustrial_food_maple_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e32b8fd17a39cf316d6c3cadf38352bc2db2230d04ea39c50930e4097cca043"),
		new("item", "preindustrial_food_meat_and_fruit_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e138bccb01d5eb859d3c0b6bb741b04052848bf6f9da7838489a110696c64ae"),
		new("item", "preindustrial_food_meat_and_yogurt_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e685328a2e306d589c886e52bed12b7486692a891d697d9c960e627cc4b6216c"),
		new("item", "preindustrial_food_meat_fruit_campaign_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "092a08f004e2066f7e25f3c35c5d9fe88a2462ba32076e65f455a89b9fe780df"),
		new("item", "preindustrial_food_melted_cheese_bowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d507a4578c73fc581835c1a3f8ddb327670e503ecfc4538004f864f09431bfb2"),
		new("item", "preindustrial_food_minced_fowl_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce9942f1047bb0f4b3e095332cd650478f328dcff7ac41ef533b130c0047bde6"),
		new("item", "preindustrial_food_mixed_green_pickle", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "220d0200a6e7c8961df6e2cde4fba8cc646b3e7b4fecb93de7509ed5e909aa19"),
		new("item", "preindustrial_food_mixed_pulse_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dafa6aff50ef26945b15c746ae9d86d6b931fe0680a2474d038b95e2f772bba9"),
		new("item", "preindustrial_food_molasses_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "15813971e33acb797fd9fa5c973305095544457781278a000c67d4c3a9946f6d"),
		new("item", "preindustrial_food_nut_and_fruit_roll", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75f016df482c89b19dd1b2982774c60242569cf747f67f40755ee671b49d54ed"),
	];
}
