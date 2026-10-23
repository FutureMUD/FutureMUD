// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch053() =>
	[
		new("item", "preindustrial_food_brined_meat_joint", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9d10d6a16be640880c8f748f0dcf1f6b2b1415422b33752d3e1b364382e9afd"),
		new("item", "preindustrial_food_broth_boiled_rice", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cc592da7e3975f3e4b468231d0bfdf021e0e29dabe452e7e2c12d2928ed86c3"),
		new("item", "preindustrial_food_broth_simmered_lake_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "02442f4dfaf00c3ff59ccc78c797e8e596a92386530139bf4fdc08891b2597a7"),
		new("item", "preindustrial_food_cabbage_and_onion_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0027e15f08c11d15b3027fe135da0be8271a88cfe77931050ceae7bddd9fa867"),
		new("item", "preindustrial_food_cacao_sugar_custard", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2269792ab183ac76347bc547a21f9f9f6a51cf65b081b0ebe2980042f4fd07ee"),
		new("item", "preindustrial_food_charred_winter_radish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "30966a8451650cddd13e7d48b1a31d7ca9ec8ae0cd6f9f219fa85b661328bdf7"),
		new("item", "preindustrial_food_cheese_batter_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e465e37e95b987c61065c9c5584b0d5f75b4b00909c6ce6849b0d78cce796ffe"),
		new("item", "preindustrial_food_cheese_fried_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c42bfbd9eb238c51ca624356954e7684b0bf6d0f74ecba117978b70a32321429"),
		new("item", "preindustrial_food_chicken_and_barley_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4186131a45dbefe0099c440ed8068cc7df9148eed157ef2586252e45a97818ab"),
		new("item", "preindustrial_food_chicken_and_rice_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "195e4bd5caf6caaacb2bc15cd979131896da8a461fbc556b71718b559b960f1c"),
		new("item", "preindustrial_food_coal_grilled_oysters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0b1e4bf7ecb15b3f57ad09c2eb735cb787cab1ad215abc4dc7601366e756de0"),
		new("item", "preindustrial_food_corn_griddle_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dde3157b8214291f61e25008fd2c57e3bb5dfb027abbd615e3941d6f217228ab"),
		new("item", "preindustrial_food_crisp_fish_collars", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "da5092dfd9dd18a78e9a781504a364adc167a77f7aeea5c3b3f613280c441408"),
		new("item", "preindustrial_food_curd_and_grain_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "47788ceb02c849094d2f51984e9e634d36d7024cb978bf2f952ff8aafe20fc72"),
		new("item", "preindustrial_food_cut_pasta_squares", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "014f49faa85f715e505d2aeeda9a4295f32f3aff47f4cb1ce87b53bac5d58de0"),
		new("item", "preindustrial_food_dried_beef_strips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdbc934ce462b3c8a7c423c9c8bc039903bca5c15be2f6bae4733925dac77c83"),
		new("item", "preindustrial_food_dried_heart", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0e7c08e51daa08f247147acd481120d901421ab4cb238cb92f38eb01c4bd75c"),
		new("item", "preindustrial_food_dried_onion_braid", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "418330d5a8f19df8728934f64858e5f985b105d2062968dff7695d84997db587"),
		new("item", "preindustrial_food_dried_rice_noodle_bundle", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8c106427f17111ce3a9f036b0d7a1cd7717da8d6d5c6ea6e519a33f64ce163d"),
		new("item", "preindustrial_food_dried_sourdough_starter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53e547eb5553d2ebd9a635faec8e0cf07c60ed26e69aed71e59ce591a2fec939"),
		new("item", "preindustrial_food_dried_venison_strips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cc5e7ae95cfbed8c82a8060484292cb9312c10a4b9fd213ee7f99f149a1f14d"),
		new("item", "preindustrial_food_egg_bound_meat_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c67a65e2c7217748c59c49ea95f57dfb5eb3a334fd8692ffd00e2a6909a96931"),
		new("item", "preindustrial_food_egg_cheese_filling_portion", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "446ed0da9c86ea023bc5f76ba1777038a8e525d04c5b2303e3ce8e1a97112254"),
		new("item", "preindustrial_food_egg_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9466ee91f48dbcfbaed10a573e349f5307121a18513dfe3a8efc5fa37f8a0fbd"),
		new("item", "preindustrial_food_eggs_baked_with_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82bb35052bf047aa1481368f23f7c3fd88eea99c7b86773f0c09dc8db58b68d5"),
		new("item", "preindustrial_food_fish_and_bread_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8d4e070c6f62f6faca745dc3806315c50640efae487b53bbf226a9a771a101b"),
		new("item", "preindustrial_food_fish_bread_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "743fea55a3718eb9222519997ecd3e330466a675a476fb3020982b0e11f96d45"),
		new("item", "preindustrial_food_fish_broth_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7235630b1234f0243b3c4a41f7658068fa3f0ca9e7434c3507de7c1cef7fcfdb"),
		new("item", "preindustrial_food_fish_egg_custard", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81621d53e556aad60bd56650025323feba9110984d6cfe7ea90acc4b8468a870"),
		new("item", "preindustrial_food_fish_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "183cce495663cdb400c45942cef976d2249f52351a57238df26afaf6d7d7b216"),
		new("item", "preindustrial_food_fish_fillet_in_herb_crust", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c94a80f16a1e6a99ba882dcf43a74a3dfb821320c8fb7dfda3149211bb5ca73d"),
		new("item", "preindustrial_food_fish_powder_travel_pouch", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "37efbdfef7a5d9d15c9c1a79f6b9505c9021ff0eee0d22c2a53a3e01dce6ec5c"),
		new("item", "preindustrial_food_fowl_and_fruit_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7867249f1d1d22baa971997b13d1db16ff1663507f50584c78e7ae73fd2b6c81"),
		new("item", "preindustrial_food_fresh_fruit_nut_salad", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e40d7ea119a959b47ff1b5fdf4e0909bb1c811bc0956ec183482f9efe88d7f3a"),
		new("item", "preindustrial_food_fruit_braised_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "031c3ca1765eddda01420f632b3b4553cda802c83396fb3efbc1a823b7af960f"),
		new("item", "preindustrial_food_fruit_glazed_fowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bacdb579716cdf72deec2ad379cc2f3032c5a91919fc36c662b26dbee0fd60e9"),
		new("item", "preindustrial_food_fruit_nut_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1cf5b10385af0e64b66664e52bf5a48104c65569d4aa4a324099f0fc1b2a92bc"),
		new("item", "preindustrial_food_fruit_spice_spread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "31564c4071f1106c5898ab771eafbd6daabded1884df422ac2a9fdd373edcc86"),
		new("item", "preindustrial_food_fruit_stuffed_fowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "173056c516d8718d3fcefd535f928a1db74f699e2caf0785dcf3a02e378da7af"),
		new("item", "preindustrial_food_garlic_cabbage_parcels", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40f748f3318f76fb4cf8f57d6b20e0693f74d75859dfcee0347fed568e7885d0"),
	];
}
