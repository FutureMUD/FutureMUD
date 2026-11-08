// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch055() =>
	[
		new("item", "preindustrial_food_rustic_rye_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e333efd0bd590f5fb0ea8b0da3cdec691533d372ca5dd82bf8590d18619fd461"),
		new("item", "preindustrial_food_rye_march_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcb9a74a6124eeb061ed81e913ba4b72e744b5127fd0c3b37d96480884404e12"),
		new("item", "preindustrial_food_saffron_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e67affc7b55f5aded0191ff726d52d529614577671311834841d1d45c27057d"),
		new("item", "preindustrial_food_salt_packed_meat_chunks", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "503febde2452ad7cea504b41dfc4e5ab45416863ee0584a06489764283e82d10"),
		new("item", "preindustrial_food_salted_roe_travel_slice", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "31849c6eb3e5738a4edbff2ce660096dea255853852cbaf001b8dab04873de30"),
		new("item", "preindustrial_food_savoury_filled_steamed_bun", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c0d81c8847c4f97006831837b62a73f6d27b8c4056dfeddeb8f88f77712399e"),
		new("item", "preindustrial_food_scallop_custard_pie", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd169055ba4bc085930244dc03808d31de5e35539a3545b7d37801f5d86393b0"),
		new("item", "preindustrial_food_seed_crusted_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a8388f31ffeac732b55f09cbd6684deb81b54d40572f4f917934e08cb49d980"),
		new("item", "preindustrial_food_sesame_grain_cracker", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7311abd746103dd63b98ecbc8980f110ea72272386a6fd8318b9507d5cea922b"),
		new("item", "preindustrial_food_sesame_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f8b8c855b546d77ad4653aec2a3191f6aaf5002516d767695188d0e43d254cb"),
		new("item", "preindustrial_food_shellfish_and_rice_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cebdfdbe445897e7e8fa2d32e6be9c98b670769c79b574e34b75bbea9ca1f07"),
		new("item", "preindustrial_food_shellfish_herb_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ce56dd7a533eead24eb4fa19b977d94d0352d82deaca2a790ede33aac442256"),
		new("item", "preindustrial_food_small_roll_dough_batch", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aee17cfe8d00eb73dbc02ec64fee5aeb98f44f6f7af40bc6245c5a7546e93b37"),
		new("item", "preindustrial_food_smoked_fatback", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8135b6106981b950de36997864ceadd21f1c613c5d85f773fcd88f2b434e149"),
		new("item", "preindustrial_food_smoked_fish_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "08e31c6b8b48884458021518b51e9bc637b6a9714139094162dcaa62bbbaa646"),
		new("item", "preindustrial_food_smoked_liver", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "219be961e792124696ad78e7bc07f3b56cdce83e69584d22dd6b6bc64de00b5e"),
		new("item", "preindustrial_food_sour_beetroot_salad", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f98448a3ba1d468ef781af1071d66bd7c43523cfc180dc6e3a30be016769e1f"),
		new("item", "preindustrial_food_sour_glazed_roast_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "19b1aa5d26dce23ec8b6ee26311c0e8051464a871c47fb74d0cbd4238f9c957d"),
		new("item", "preindustrial_food_spice_glazed_preserved_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "847f74afcf8e17ded49f3428f2cc9308a84cc9657ae449f86395244a1fea504a"),
		new("item", "preindustrial_food_spiced_crustacean_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7deae03acbf24daf97cdd8215935675e6c34ea7b730fb981cb43861f7d7773f"),
		new("item", "preindustrial_food_spiced_dried_fish_sheet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b966f9dd5d4e938456a26eae1304f0f6e684a7c13b5a61d8e3d189f6c709f29"),
		new("item", "preindustrial_food_spiced_herb_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be65316e9c2c087c9645e6cb359d2f5a05cbc7c4302219419266991a84ec40b6"),
		new("item", "preindustrial_food_spiced_sweet_egg_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a30ef6fdf8c70a67cc5a1b71162a1eeb167b395b39b7c6566404ab78562abc87"),
		new("item", "preindustrial_food_split_baked_crustacean", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "31366c836d27ac45fe4c0894774c76e1159708fb92be0b35d2a64a2202f4b875"),
		new("item", "preindustrial_food_stewed_dried_fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "698e2df3d58cc9cc2374762000d140a60ded5c904914190ab043cb96841ca7ad"),
		new("item", "preindustrial_food_stuffed_cabbage_leaves", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "095205c5a179f562324d46d371be8b01dcfec5ddfa87388fbd13b7d11a226c4c"),
		new("item", "preindustrial_food_stuffed_heart", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f7f017add63126eb673a6a6cb0231ef6acd38d256b195b63302afbd3bd95a9f"),
		new("item", "preindustrial_food_sugar_cured_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eab62ed11548428c0ab45aee449373d2789d80b3aa3d1306b4271d1cc1870de6"),
		new("item", "preindustrial_food_thick_pea_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "599503aa63c043acb9ee794f16196c47f48473bfa317e837f4f5fd9d58ef74a9"),
		new("item", "preindustrial_food_torn_pasta_scraps", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f160e11c7006511166420e2cc8078568244aa4bc039425413c8fbe24bd445ade"),
		new("item", "preindustrial_food_tripe_and_grain_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "29d56facde13ab5853f3fb9c4a801e4144f0c6fa51e9bfab6eda592dfa7efcab"),
		new("item", "preindustrial_food_turnip_cabbage_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5522f0203c36daa77932b6cb4a107463a0655d3297188192b33eaa4a3c49215b"),
		new("item", "preindustrial_food_vinegar_asparagus_salad", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "feb43966627e68e7eaeb9418cf255bbd277ffc59045d08555237a2117a267100"),
		new("item", "preindustrial_food_walnut_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6575f58553abc62f655e8638c64178e2e17afa449916a9a587eafd50efee2073"),
		new("item", "preindustrial_food_wine_washed_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c089ec422c6005876f219a36fe600eb327791cff1cf451b5c0c969e30766297c"),
		new("item", "preindustrial_food_winter_stored_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "520cd5c2b49d2372cd7aef7caf1a1af585dc852f8b867fc422fdd29be8f1b8c9"),
		new("item", "preindustrial_food_yoghurt_herb_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4cdba1fde11e2f5efcb2c68ac7dea60a17a06eee749fe73f7324964e98efb8f"),
		new("item", "preindustrial_jewellery_earrings_09", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78442a9c76d1ef01f4b5827d19274b08ab3cce0690db590f8cf4045c67337cb5"),
		new("item", "preindustrial_jewellery_waist_chain_55", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66bae85ed20db1088e6968f7690df47b59ec4b0e96dc132edde8155a3ec0433a"),
		new("item", "preindustrial_military_support_double_hanger_sword_belt", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b299591aaaf9c7fc8ba23aabfe694043447cf8ad90bade215a943918120345b"),
	];
}
