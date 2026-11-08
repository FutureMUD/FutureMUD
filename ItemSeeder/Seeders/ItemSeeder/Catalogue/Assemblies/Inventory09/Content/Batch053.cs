// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch053() =>
	[
		new("item", "medieval_writing_bamboo_pen_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b51754c16913c72caa09b40c4f5b36e3843494dda20543c180d41bf4c93d7ea9"),
		new("item", "medieval_writing_birch_bark_letter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e74b126ce0d5e674b058a87e739b1afaa10a97320771b15003030358c73e7b25"),
		new("item", "medieval_writing_blank_commentary_gathering", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0378130aed26c14f2453ab9f46079e8c628b1c2949e5376cdca049f9a3ff82d"),
		new("item", "medieval_writing_book_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4897f55758a452868b11159863246acaae9bbefc92e547da460ba825c3c0c1ab"),
		new("item", "medieval_writing_brass_office_seal", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ca3b68d9abdbb51edea06094af34cd742abd49cc1ed18679f10ae06b7652966"),
		new("item", "medieval_writing_endband_needle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f510675c7cdb6a6abf264b236731b7d3bbdc3921ec3af8ccf4836914bfeb96a4"),
		new("item", "medieval_writing_notary_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "96692660f6247450c2e56e89561bc15618559e91f3e3049d6a16e62751f21672"),
		new("item", "medieval_writing_plain_quill_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "452db88aa104c78518bff9dca098474f1dffa5d1954c8a39fc717d1ba7e08ea3"),
		new("item", "medieval_writing_powdered_chalk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9b837dec5bf64e388adaa24bdf3aa14d461b23e96a42aba075b0e7149a39565"),
		new("item", "medieval_writing_ruled_rag_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "efce8fbc0b9942ed42910c9adf03dedcbd8272f05f59cf308dcaea9d8a40ae7d"),
		new("item", "medieval_writing_scroll_seal_cord", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ca5aacb8bb96134fc32ea459d655db0ca092afb7a5e071d075b2fabf1bd09b3"),
		new("item", "medieval_writing_shell_palette", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4fa576a3ea1eaa95861287362fb5cd754a51f6b92eb7a75bb08a1f6e3d2759cb"),
		new("item", "medieval_writing_silk_manuscript_wrapper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8f093f0668822c275eeec24cc9d7266b94bed3670c1380a3c933caf92fe04d8"),
		new("item", "medieval_writing_wax_diptych", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4d7d6d4a36d94e8593e218abeb3d22377c8216545f1d76a8cd2af47fe5e4180"),
		new("item", "preindustrial_door_courtyard_gate_36", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c317590699d9827d79d5a5c46084851f0ecef39a98372c81f94153092bd7d9e9"),
		new("item", "preindustrial_door_courtyard_gate_50", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b33ba8b137f8ea14db7f643bedaf5c1e8efeafa56b1852993fe9a398cd36a46a"),
		new("item", "preindustrial_door_door_bar_68", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f875b0e653cfcda76017887484a8966bd928ac8c71e9007b39c6850f2db6d2e4"),
		new("item", "preindustrial_door_garden_gate_52", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a89959397acf03d292473078e9cbdf14317abc31da31d3c43aafd1b4c743152"),
		new("item", "preindustrial_door_lockable_postern_gate", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61b4872c2b0f934bc5f6a3dcac2f15ddc434762c7ec844025f379cd9f6219440"),
		new("item", "preindustrial_door_panel_door_29", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "44c3deb4f3d5707cc40db036475a5694e38281f8bafd04fd77f05b8798094811"),
		new("item", "preindustrial_food_baked_quince", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a94dab720ae53b6178fb44f431be901c929411ea0b0989421bf3ec740201c1e"),
		new("item", "preindustrial_food_barley_supper_roll", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "23baedd2d34353a55a55b36b8203ba5f983c37715b53b554d41185d80332ab6a"),
		new("item", "preindustrial_food_blood_and_onion_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "655a59e1a93912a318fa86065fa0651b0cea0356f77b1dfecb1fd09f0bc693c8"),
		new("item", "preindustrial_food_boiled_carrot_rounds", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0190a3d37764d83ce63206c51469a8852e961b340ab3583931f62db372597594"),
		new("item", "preindustrial_food_boiled_eel_coils", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c99387778b6e60d0835808578bd22f2de44adf76bba2687dde89456b28c8bcd"),
		new("item", "preindustrial_food_boiled_garlic_cloves", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d66feadc00cc1eb9a63c6b2c7b75ba88e2654c7851168b48c1000572fb0a328"),
		new("item", "preindustrial_food_boiled_grain_with_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "beef43ea00cb93a6d768b5d45e4f522c0985ee2bd35daf81f5b22a7811bc8e86"),
		new("item", "preindustrial_food_boiled_sorghum", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ad42419be28e1db8f97b387f0030ff719a5648ff6bd3c7e6dfd7babfda3e6eb"),
		new("item", "preindustrial_food_boiled_turnip_chunks", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50c79dd4927d0410b7d1163a19a5d138ff778d90a8361d498336be89ba44df50"),
		new("item", "preindustrial_food_braised_kale_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c02bd258bf7b1ed1109d62f3556374f52e71bd9847d49a33eb301bf94909dae"),
		new("item", "preindustrial_food_braised_leeks", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac02665117c5c892204d1864b594718862a2df7b7e63b10d6be9b104a754f933"),
		new("item", "preindustrial_food_bread_and_mushroom_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16d1f8851617a3798e808aee8bd5f02c210a897088819ced4e60b180b77d0b6f"),
		new("item", "preindustrial_food_cabbage_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1c79bd9f8f285c373f6e0b6437bf9c2d6dfad136de160e8ec0f6d26feca6243"),
		new("item", "preindustrial_food_candied_nuts", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b41edf4cfa019beaa6fa9bd7e01c68c775617713dde0a8939d8d694824d7293d"),
		new("item", "preindustrial_food_carrot_and_cabbage_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "46598af6066abcb7bd86994baa014797684939020fbe0d728df18d9ffe838905"),
		new("item", "preindustrial_food_carrot_and_onion_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "149e40caa328248a3525fd6044e634c9a5d1f59f1cc6c477cd4b596d44691aa9"),
		new("item", "preindustrial_food_cheese_and_grain_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb31f47e79aa4484baaa16f519e769a1b6ffdd8e99898f2019fc8af108b54b9b"),
		new("item", "preindustrial_food_cheese_and_onion_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eeaf1ed0f8932884ebb5d4ffaae4db9d2b9bae3f582f9cdcd6dbde2f8b224222"),
		new("item", "preindustrial_food_cheese_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "64687f1bf9e82c80102dcd22d8eca34c5e4cdfc9f33414d258f7fc04f25b9918"),
		new("item", "preindustrial_food_cheese_stuffing_mix", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9bd38cb8d858d6d8ab68cfc4c826855330c13ee13ee17fc05a51d461a0747edc"),
	];
}
