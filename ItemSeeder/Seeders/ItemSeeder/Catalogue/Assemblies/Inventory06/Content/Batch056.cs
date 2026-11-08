// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch056() =>
	[
		new("item", "preindustrial_food_torn_dough_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ed849874822a370c6892801f733e8869dddcf6bd1068d7b617573074bf46325"),
		new("item", "preindustrial_food_trimmed_meat_for_drying", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df99a245ada4162e2a06b7736fecd8150fe13c7e15c2124512cf386859ad9a50"),
		new("item", "preindustrial_food_vegetable_egg_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "56f5d6f61f95c5f36e5f66aaba7e2dc023530d8015b3fe9a1c4eaf8025059062"),
		new("item", "preindustrial_food_vinegar_carrot_salad", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e022f766d90e16b678486374f2d3a18cf0259e26355f589b07f078d5ee2d458b"),
		new("item", "preindustrial_food_vinegar_glazed_turnips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb1e76633536404a2cf3ae12b8004c7beb82add599783c43bdf5b4865cc80070"),
		new("item", "preindustrial_food_vinegar_sopped_ration", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec268d0dae25d0ee4d561d4392dcec746c93a6a6f9ff9a6c80e382d3dbab7e6d"),
		new("item", "preindustrial_food_wheat_berry_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf23a2d6af3a340e3300ed3e731694f359073d744d942d260e29d210e7a170e6"),
		new("item", "preindustrial_food_white_bean_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "00f5006357a0e056fae82c4f0ef7f58d47b406ec9d8ba508fc50413930e4e125"),
		new("item", "preindustrial_food_wine_braised_mutton", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fb48d76f80ae3521777c04f339d80cef9749a019f8f4c17c631db01bfc1182e"),
		new("item", "preindustrial_jewellery_armlet_47", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0da7db33505583fa96d4b5e316386c0ef06ee6121cdfbc00ea5d83b56b91c402"),
		new("item", "preindustrial_jewellery_torc_19", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a836b420489962566a99521725e55dd95ca1b149b4ec32923681b20b83e55a2"),
		new("item", "preindustrial_leisure_draughts_board", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea2e7b4b3a57a79a3c0a7649538eb7a0d15911527c229132178e33de7c0d98b9"),
		new("item", "preindustrial_military_support_fine_display_armour_stand", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c09600309f0849d0b77e5281ab9ca0f3cae462cd433b21832d71eaf32cb922ab"),
		new("item", "preindustrial_military_support_plain_arming_belt", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ab7c4ffc421e151fb0b9b826e8c025356fa123ccff2bd7c42aef41ef923da1e"),
		new("item", "preindustrial_printing_broadside_sheet", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e65f641ba38287bc7044bd1ba3c2f4ce3cc4ec323b65ab00dba4e2766b86dcf"),
		new("item", "preindustrial_printing_printed_map_sheet", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "80b4f0d8a9498797a7c1da3517387d139a9f32dd971cc5410e41e471596570bf"),
		new("item", "preindustrial_tool_adze", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebc539237a2ba6728f3de7d0d6a183f9424d1fceea7a7a0b71f5fa2794afbfdc"),
		new("item", "preindustrial_tool_ball_stake", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39a0008f4afd8ff1eab10bd2575ec0700ec28aa13bb0de0b0d8a4cbd069fa50b"),
		new("item", "preindustrial_tool_coopers_adze", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "35702b3eb4d69162c94cce97b15ad93fa09313a77e2f20700584d66348343420"),
		new("item", "preindustrial_tool_felling_axe", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a641ab9177d3ded2959ac3476726d4648f0d19a989e6b9d8f6b2d5171587bd8"),
		new("item", "preindustrial_tool_press_mold", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d74935ea2aaad5aa7456cf0bdea823d36ea36fc5ab58a5c9ad1be39d8b80739"),
		new("item", "preindustrial_tool_slag_skimmer", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bdacaaf188016f72d76ad5a2c88ae46ebaab7f4492563ed98f4d888018427577"),
		new("item", "preindustrial_trade_ash_lime_bin", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "89f7c18d2fae6ef7ec43fd650aecb0f54b3384584112e341904b2812e7ce3def"),
		new("item", "preindustrial_trade_cedar_trade_tub", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2443cdd90981ad5e75faa1aa4feece560b3b2bfc3d3af00e6919af7b1cec781a"),
		new("item", "preindustrial_trade_nail_keg", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "35c24c00b4826162b442fc5eaa72d1c27c6dade46e79e224be126c96b26867ee"),
		new("item", "preindustrial_trade_oak_plank_coffer", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c2b80353543269a23cf3b0d822454d34602e0309dd40c9ac9dd7491e539f1a1"),
		new("item", "preindustrial_trade_pine_wool_hamper", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9faaa33f61af8c2aed8f10eda7f0acf1766736a1ab5764193fada3c31f451909"),
		new("item", "preindustrial_trade_rope_coil_crate", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a18354b4a06b0d7327016d2985c7b34e02223d4a90f4a708aa3c41d4006c152"),
		new("item", "preindustrial_trade_rope_maker_hemp_crate", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "92f7cab57fe77aa30d595d7e9c2633c3cbf0ef5fa7b77c79058be5da6e2ff496"),
		new("item", "preindustrial_trade_spice_sample_lockbox", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae3beb792d16750e64518e8d473643e6d2165a91df51806d51130076a17fe329"),
		new("item", "preindustrial_trade_wax_block_crate", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce0fdd03acb3e254b2ba00744b8f58b8f8e413471436a8e64035e186ccb0f005"),
		new("item", "preindustrial_trade_waxed_canvas_spice_sack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfd6209eebbb1b3b01621f5b2188ef74eab73df5aa83efd876faf0deb009ffce"),
		new("item", "preindustrial_trade_weights_lockbox", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01a4e91189f6e60ac62f673a00c0612a83ba0a4115cbbf3588e123fed3a52269"),
		new("item", "preindustrial_workshop_glass_glory_hole", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "987f9dbd22931fb96b57a956de1c3586c61b0f7b6beb24401f875692a6385c35"),
		new("item", "preindustrial_workshop_papermakers_vat", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6d363facd8014a609b047f0b584798863e198e06bf413f4d62f6794a579cab6"),
		new("item", "preindustrial_writing_fine_rag_paper_sheet", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "419920bffacb1703146e72cfb2aaef03b446a22b99ef23a273711f1d9f37a34a"),
		new("item", "preindustrial_writing_ivory_stylus", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3b4280f2f31253ad9b4e525585be14c41e7a711a79533748cc659279b59021d"),
		new("item", "preindustrial_writing_large_parchment_codex", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "57458c759ff48d51bd05ab0ee9503b3831b939d4341cac1a7bdddab0a5b5b3eb"),
		new("item", "preindustrial_writing_merchant_account_ledger", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9e37be86e4b3ef82a1a4f206a9128cbc7cade6606a2967ecbb7782ef4a98116"),
		new("item", "primary_production_azurite_deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dddac4764864c414f823a0a89f7023dc03908c637e6dd5473c4a53200c012d2b"),
	];
}
