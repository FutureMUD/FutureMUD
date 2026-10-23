// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch054() =>
	[
		new("item", "preindustrial_food_campfire_twist_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "450d4774abb65f703384c96bb597f1034bf722db99a3bf8a6681e6bdb4e46a7a"),
		new("item", "preindustrial_food_candying_fruit_jar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb16b5eb5b1a37f6fa66eba147fdd463185524c632f844b5290d848818ac6e86"),
		new("item", "preindustrial_food_carrot_and_lentil_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d50ffbc8c3aaae62d9ddc79a3bc1d0da1649f65b4a0c2dba3aca4bee3f5077da"),
		new("item", "preindustrial_food_carrot_garlic_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "935d2b546b1d4055126130f54c82a5402d20cfd9e08099b1a25c2d82939fad74"),
		new("item", "preindustrial_food_charred_baby_turnips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a25c17d170bb768a02aa2f0213611c6ab91c4384429a24b10e018efe8294c0b"),
		new("item", "preindustrial_food_charred_heel_supper", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "22b27b5fc812db9278b3a8fb8513b4bfeff5bc40cc99b2515b83577f3f083960"),
		new("item", "preindustrial_food_charred_okra_pods", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "426eb59754c1d714530f6c7b2d94e256ad0892f75817c624e829feb42c87a13c"),
		new("item", "preindustrial_food_charred_oyster_mushrooms", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "960f8cd7664cde405f3691146a4c2fa8eed96ba6d1572bb4a3eddf26e1f20eee"),
		new("item", "preindustrial_food_cheese_and_nut_plate", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf8af3d19ee738788c4514336a0fb552d0d75ae27b9e58a5ef1daf45855c59c1"),
		new("item", "preindustrial_food_cheese_dumpling_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7fa9906778867376d09e174f4df5f653e5bc5a50d9732bcf12513c220bf05af"),
		new("item", "preindustrial_food_chickpeas_with_turnips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b722fe8f8c7464a5bd6ece29ffe118769a1b8f74bc842f2b854c5b4b5c331fa5"),
		new("item", "preindustrial_food_chili_fruit_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fde0cff87dad6a15cf03484ab090f9ae3fa1161713bb1c1f546586315983d32"),
		new("item", "preindustrial_food_cinnamon_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd82d75fa86693d5848bb16ab403c1ca7702e79be24bdcdfbbcb9969d042ab50"),
		new("item", "preindustrial_food_cleaned_offal_set", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee998390a41e3ef1ad7bef5f7e7523097baba4128ceeefdb6ddc25894fe02748"),
		new("item", "preindustrial_food_coarse_barley_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b3919f2c66fa649c8f1d061b186a015940a65f0fcac944cac798f9d27b15841"),
		new("item", "preindustrial_food_crisp_noodle_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5a36845b3ea62cd1f0628e84488a641fbd54dd9f77ca2a9218e79d8f555edc5"),
		new("item", "preindustrial_food_crustacean_feast_pie", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5aa02ad4a38a634f03922b8649d78a9a301210e929bf9bc38c29d79e8346706"),
		new("item", "preindustrial_food_dried_plums", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c33a6d754538dc051a3f2b7ccfe15b0120e20df4bf2ddc04c2c75c387f946e2"),
		new("item", "preindustrial_food_dried_small_fish_packet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4084c447f50b6bde8ddcf560c561dbcde00faa7bfada7fba3acca320900017f"),
		new("item", "preindustrial_food_fatty_end_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "987e012db2818cbb11eb29ff674011b53d3f4fd7de27466892c3c2cc70216ba7"),
		new("item", "preindustrial_food_filled_uncured_sausage_ring", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3fabaf1518341e1a4577fbd72ed7f10930eb37c939ecd99a0965a12354c33ca"),
		new("item", "preindustrial_food_filled_unsealed_preserve_crock", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "83f8c592eba7f203d4869a90bcaa73a9635225ac5b9966fc8dd17a6341bb545a"),
		new("item", "preindustrial_food_fine_cut_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e0106d8081f95221ef580ab3b079240b62b4337754f45a27a37ee3d06daa819"),
		new("item", "preindustrial_food_fire_toasted_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68a0b1723505e28b081c27792e95b38309b22cafcfe6f1e0ebc21fe35f00d6e5"),
		new("item", "preindustrial_food_fish_and_millet_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "166453d8090146699994c9904215c1064275b461e285b18fd932d6ab7bda8275"),
		new("item", "preindustrial_food_fish_and_onion_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "37ebff81dad9fecc6e819333ef79ea1303121ac957cc79483ed5373e11d36bc9"),
		new("item", "preindustrial_food_fish_skin_sour_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "608890530d95ffeb8c3232d06f2e8969db468ace09e97a99ccd7afba560cb345"),
		new("item", "preindustrial_food_flatbread_dough_stack", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c928c6604a754d40b4fdff6e46f047fbf96a1791016e6153fc1bab9aa84ae163"),
		new("item", "preindustrial_food_flower_shaped_steamed_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7750d49a25077eb7991d4126c5ff7c5989914890461517974755c3a1f6c7d1a6"),
		new("item", "preindustrial_food_fowl_and_nut_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67c0a496fe1a77417e44b229f9731b1c0cafb9d1ac76e89a078c079869165111"),
		new("item", "preindustrial_food_fowl_meatloaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16059e575f96d4560962a1212aa1431086426fd341dacf9785084b75981a381d"),
		new("item", "preindustrial_food_fresh_date_cluster", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8c857b0a22adfea4f976062ed8ce28bc7b921ea6e9a17564d47d89210d2cd8b"),
		new("item", "preindustrial_food_fresh_pomegranate_arils", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8fe157d0e00776060a074d19adfe54a0091885b8a0ca0b166b10dfb65918434"),
		new("item", "preindustrial_food_fried_cheese_cakes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "26b74d7da27ffb2becda3b13de8ae15c9b6c2155c82fc35593c402e75f077e73"),
		new("item", "preindustrial_food_fried_fruit_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e6a5630c05ffab720e4a048eafb9a52f5e6525789ab47aad5809eaf48e0a7bd"),
		new("item", "preindustrial_food_fruit_glazed_roast", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "025b578a8c8926f5e5b8f153ebd16f254cb692a772e4934bd54a29b081df6585"),
		new("item", "preindustrial_food_fruit_stuffed_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0efdb92d7096d791910636ceb03f09e84732dc2ae65035d4702596487b43d28e"),
		new("item", "preindustrial_food_garlic_greens_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40a7aff3b3dc3a03d7e8688543dc751701b0f2e4a75c0baadb3e26f66e861607"),
		new("item", "preindustrial_food_garlic_herb_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf6149d99e1f49b46323cbf868b678a461e2925f2d3a2db1d9ba121e86b86156"),
		new("item", "preindustrial_food_grilled_cheese_wedge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "37f1324b6253d6424de71a14e878739ef846aedfc7651034cdfc6f4917c0e41d"),
	];
}
