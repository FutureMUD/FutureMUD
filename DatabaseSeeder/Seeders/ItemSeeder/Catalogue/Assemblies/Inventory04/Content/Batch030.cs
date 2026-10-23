// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch030() =>
	[
		new("item", "antiquity_one_shoulder_wool_mantle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "33c517a1a389e7ac2d6da3038ac16c1d85e7eb8e1982c2ff769321e79067e235"),
		new("item", "antiquity_plain_trestle_table", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "382a114bcca115a8facd01221be95fe6127ef2165bd2f255809ce8f0212a07b9"),
		new("item", "antiquity_poppy_latex_draught", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c96746281fb37d71cf9b05aead19eca279278c6b52d39926b00605fcb94443d"),
		new("item", "antiquity_rectangular_wool_cloak", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cb38176ee328c381bf24c691f409929f9de79647acb49b6c6f95544fc49379f"),
		new("item", "antiquity_roman_aproned_military_belt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4921c73103b77ab9934a8631095852bd38b5e84e95889baae2452a34c4cb0feb"),
		new("item", "antiquity_roman_imperial_gallic_iron_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "76df58419032286dc04914255b06ebb091e88da4ec453586bcce67f87d28def9"),
		new("item", "antiquity_roman_intercisa_iron_ridge_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "be0a2acf19d259d2c8e5e85447d945468bc8fe17fe75f2f419acb8145d261a51"),
		new("item", "antiquity_sarmatian_lamellar_greaves", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f93c014c7b053e6bdbbb1fa5ef051e74e1e553b1b31144b8db9f3630a85fd44"),
		new("item", "antiquity_scroll_pigeonhole_shelves", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dccf5febd23b29b46d2784e9340ed16d47aa10e489ee15a9b60c5943aef9dc2a"),
		new("item", "antiquity_scythian_conical_scale_cap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce3856e45f2e14e2b73e4c23dd27736205e24654f23c15f83ead5dd5227c17f7"),
		new("item", "antiquity_shield_common_round_wooden_shield", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "442180a4fea2dfed89228030db5ee9b48dec76899ef240dd16efa91315fc2bc1"),
		new("item", "antiquity_shield_hellenic_thureos", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "817088987b7cef8c8b75baa7ff0af7c2b566db7559a53299880bbcee0ab983d4"),
		new("item", "antiquity_short_sleeved_linen_overblouse", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a0d6628a2191d90ba69ab2a0625be0847d6031e656c0a5897aae952f2d729f8"),
		new("item", "antiquity_simple_linen_shendyt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "978a7d6de7b23816bffe8dadc0e93eb0f3887ec6b66eca887d0236d805e72c17"),
		new("item", "antiquity_smoked_hide_meat_bag", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca5fac208e1e875c14722e52d2548c5824df4afed069ec4adb850964349f72b8"),
		new("item", "antiquity_stone_mortar_and_pestle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8073cc5fbfdbf88c4107235fc46294f761cd4c77581aa28042e1bbbf970a6695"),
		new("item", "antiquity_tableware_anatolian_animal_rhyton", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8f4ce36dc55bdf0a73d61eebdb70f538758fb261e86412cc0cf6734279482e1"),
		new("item", "antiquity_tableware_egyptian_palmwood_offering_tray", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9bba2bf109f0a11c5e829145623d08eed7c163cc6656283403ce6d2679f9f07"),
		new("item", "antiquity_tableware_persian_shallow_gold_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3691cdde60ee5b219fee1e64882cadaf3d76615b5ebb190127d4847e2d7776c0"),
		new("item", "antiquity_tableware_persian_silver_charger", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f9516d1b647a2e63d120bfd01544b86014a357609a2a4d2bbe14ab77c3c4a12"),
		new("item", "antiquity_tableware_scythian_gold_mount_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "11a43847f5f3662d0289904d1a654bf999400466f601c34d827330defc6ee3c7"),
		new("item", "antiquity_weapon_anatolian_light_javelin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "15c042051122f66d68d0c6d6da8a7e7d246f7a404ce3e8cd21d7ac5d74a280d8"),
		new("item", "antiquity_weapon_egyptian_bronze_war_axe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "42efd1e0c4ff28eb0604614a9940ee7030711684505e4903ce812927dc9e54eb"),
		new("item", "antiquity_weapon_germanic_throwing_axe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc2b7edbeb40a92d6584798a10653a019541ca17d9cd0359dd93dff21ee4487f"),
		new("item", "antiquity_weapon_kushite_hand_axe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b6af2e0c65f4b5511d1425ce3d03f3fbe162ff6e1c39977c9f4866d1a74bc80"),
		new("item", "antiquity_weapon_punic_light_javelin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bea873ad88c75d962a374976798ed67205537689707ee4a204a8c5c31f0637f"),
		new("item", "antiquity_weapon_scythian_light_javelin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "17c5a51c0b7b3d35d3541f2e7ca6950850ed2de719363362fe049a22de95f069"),
		new("item", "antiquity_wooden_lyre", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a5cf55d23b70a9c61dab98808ef83d3b09190bc10d4603c1b6420cb15f4f5da"),
		new("item", "antiquity_wool_footwraps", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "97a415690a51df50f76aae86bd159c3fad7e87dbf1967144761b12daecbb574f"),
		new("item", "antiquity_yarrow_styptic_pad", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "eabd6f8ec85041fd602bff4c4d80ea5e629f9a8331e4dada9256583d3a2d7573"),
		new("item", "earlymodern_africanatlantic_clothing_short_cotton_blouse", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc64650ec121738fe8a5e2761f2d42f72c75445f4a6103239985695cf9308d16"),
		new("item", "earlymodern_andeancolonial_clothing_wool_poncho", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8db9e8444ba0420a7d1eed3a7f6c1525367c036514ddf7aefa1038a101503289"),
		new("item", "earlymodern_brazil_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d549b678c04192fe9d0bfdf303aefe81add4ae0a2793aafcfd0b20c226ecabab"),
		new("item", "earlymodern_british_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d99880fe28642a3263a995a948d5f4339bff578190687188865cc88ba0dee39"),
		new("item", "earlymodern_caribbean_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e419d99d7a692f870c43068c9fffbe98fd4363bf92672c9590853ee1f15f7e6"),
		new("item", "earlymodern_colonial_clothing_plain_linen_shirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87345ccb1c4d8fe079ee7ef0616efd55b71f7da615e3b3a469e628e01c336f20"),
		new("item", "earlymodern_container_dut_coffee_pot_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bd77aab1893e25e5b3f805cc21cf15b2e963ea0e456e9b052389f23e94d4c5d"),
		new("item", "earlymodern_container_edo_account_box_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "796f8fe73e351d1d4c3c0f00cc817f3a4d38d089c68982d6219baffb2658ddab"),
		new("item", "earlymodern_container_eng_ewer_31", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "da0ebfd4507df6a8fb77f5062f1201826146a192fd2a393da866701baf285564"),
		new("item", "earlymodern_container_eng_tea_pot_27", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a5ab043fc9159e04e36bf26d9633d324735ead02359484e966b26e4fd3703ed"),
	];
}
