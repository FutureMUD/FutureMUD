// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("commodity-spoilage", "Dried meat commodity spoilage", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "54ddb329a2609b54fafc1788a585d3895ac76908c2682c611b5d3e99896f4454"),
		new("component", "Antiquity_Charcoal_Stick", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cdcaff3cc0e6c86f6d4980a6f7df477bb1700d416ec8b65242d75bfe31354576"),
		new("component", "PreparedFood_Antiquity_Sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5fc45576502361e00cf7040b49cec1d20e6e58536f2a7301a73b714f054839f"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_chelsea_currant_bun", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "13484867bc7f7276cb9986bc27cda7c9988d2e7eab264aac64b128c7c276d48f"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_syllabub_cream", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "428e1784b9ba8c083154c80d2647737a1142402100f9e39ce3622c9dbd8b414b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_caribbean_cassava_bammy", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f99e4c45a774096917dbd0c07a0b407daac36fc7d7ffac58260855a184d5a54c"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_dutch_shipboard_cheese_ration", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e3d602155cc6dd8d74f374ae0ecf89e0edcc1357576cf2f465ec903aa01a630"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_german_apple_strudel", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8df4da84171880ca66d0f3184301439550366c8988a74fcba62740ec3385c9f"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_gold_coast_smoked_fish_maize", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f67a3d82013ed520796571eab21f60c8cee200b409223cea7433f1988b7c0452"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_iberian_cod_fritter_plate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1478a109d9e455d04781283d46f23178ed78efe6d5e98d6fd988bfa2154ad8db"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mediterranean_tomato_aubergine_bake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c7b5dfbd5ab55e3b2aa4d797802af1dfdc8ce341ade20ff2f6a6f5325025663"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mesoamerican_chocolate_pan_dulce", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e3aa7ae5f3366c1eef59fbe8096ee3c2dbfb6421e05c5e2fc5ff46f9c802677"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_pistachio_like_kulfi", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ebad7faeee5b842da811371b40a3288fe92719b3801502bbf8e56ced9351a93"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_plantation_molasses_ship_biscuit", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3400e4e048ed4d0bbdc3f706da0fb04e8b0ef16c2afa4c976554835292ec9ac4"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_safavid_crisp_rice_crust_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "78ee1dbaff81c63254fb317d0366f19f80769d709f8eb22a35f587ac1fa99d68"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_safavid_pomegranate_nut_fowl", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6010f963592c9017bbdeda756b9ace893cd682279e20744e95d0b19d9b201871"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_safavid_tea_almond_biscuit", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7598cbc4923748f3ef10114657eef93bb079d601b05c3e1a4dfc5ca7746741fb"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_woodlands_pemmican_berry_block", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "05d514c67566b38278a1856230a1dc42ba42603779fd254df87bd0ebe1b36547"),
		new("component", "PreparedFood_Catalogue_medieval_food_almond_frumenty_for_lent", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a4ae34ffde79789f53b71b7376104ea425b047cc14c8588a494345f1af7391f"),
		new("component", "PreparedFood_Catalogue_medieval_food_appam_fermented_bowl_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7418f9e641f28fe02149282999ddae9bd7543f91160c652eec859130f823e66"),
		new("component", "PreparedFood_Catalogue_medieval_food_bazmaward_fasting_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "437c457c62385eccc33c3716703677cfb70a43ae760495101da6d0a88b4d1140"),
		new("component", "PreparedFood_Catalogue_medieval_food_bread_trencher_meat_sops", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "510580f85cac17ed4c43c7fcf43e1dc9ad3dadc7574b19e85c7e00c0dfbccaed"),
		new("component", "PreparedFood_Catalogue_medieval_food_buddhist_mock_fish_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e78061d93bf30fffe5c6f00c577b09856f4fbbfd7901b0e8ce8c1ed5b3860477"),
		new("component", "PreparedFood_Catalogue_medieval_food_byzantine_fasting_stuffed_leaves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd15d4ce19f3d8b2cbb717b62d730488f04621fff98787813878f1b29bd4bac5"),
		new("component", "PreparedFood_Catalogue_medieval_food_chicken_rice_blancmanger", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8df2ad8b9d824cedc07c16f0ed93637257f982e1527dde39ecaf3390e640d96"),
		new("component", "PreparedFood_Catalogue_medieval_food_faludhaj_starch_confection", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2b756dc3747fcda85f02ac16dfc582bc3b82e9f0bb0cee5d64a0713dc454da7"),
		new("component", "PreparedFood_Catalogue_medieval_food_gilded_marchpane_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aaa37a845be9ebc9911bb51bf9713c0d3be917b30509bdd17161fac59064b5d0"),
		new("component", "PreparedFood_Catalogue_medieval_food_infirmary_bread_panada", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7c183aba7fac26d47af3e18496a2ba00b9a39ea6b857cb52def78b9b7c0acd7"),
		new("component", "PreparedFood_Catalogue_medieval_food_korean_royal_rice_cake_layers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "22791b376fd62343fc0240c57eead99f97be594a34f0160b726c97089995ff0d"),
		new("component", "PreparedFood_Catalogue_medieval_food_rose_lauzinaj_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca3e6fcde84b1ee6177ff903519c2f3f80b65cf33ddb6bedcd916cc831ff74b8"),
		new("component", "PreparedFood_Catalogue_medieval_food_saffron_milk_rice_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "928247acad1c2b3a785b8b003afeaa54f9f5e6380934fb7cff92322e6a26ea5f"),
		new("component", "PreparedFood_Catalogue_medieval_food_sahel_sour_milk_millet_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c867e45fda120673789a16ff2f1ad516b206cf11f74aa22678ddba1f103185ff"),
		new("component", "PreparedFood_Catalogue_medieval_food_sanbusak_meat_triangles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dff39d8894a9c6c0af7dfd26ab966d56f3c7b255677ec1da9649fd9e776d4d56"),
		new("component", "PreparedFood_Catalogue_medieval_food_sultanate_stuffed_samosa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa09c2ac4f26c1b4def3683787527c88b11a84587e3e8b0ec1c9e12d62f8f55c"),
		new("component", "PreparedFood_Catalogue_medieval_food_swahili_coast_spiced_fish_rice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4d44f0f19943deea5096b073f73055161afaae958aff53668765b8f1a5d4934"),
		new("component", "PreparedFood_Catalogue_medieval_food_temple_sweet_rice_payasa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee47ed75175f5318d836d39a8503714ac5722afb3df71e5f36d53f56cac9311f"),
		new("component", "PreparedFood_Catalogue_medieval_food_zalabiya_syrup_coils", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "baf6a2f83923c7ac918e65282a0a86a12a163c9a541355f3705a8464ce7e56a8"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_acorn_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4de509e6bb26bcb5c4ee1846ac94318a0dc17f61c76c288c1aaf4213557a85e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_garden_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e1f5f371582591063645348898f3770e0bc636778ec935a18dc7d3d352462b6"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_beet_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2da0cd419c5a6e46b5c2587ab9a4f2ad05f801daa4ce4b8c6a2fa5cf53be199c"),
	];
}
