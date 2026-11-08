// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch057() =>
	[
		new("item", "preindustrial_food_spiced_roast_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b815b6c44d27c31efe3dc522dc621899c54e39be784d503ce4f5edaf751b5a2"),
		new("item", "preindustrial_food_spiced_roasted_nuts", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f2068435cd8520ca19b4a423c288a2677ea87e61609a9ae44fb1220d07b48bc"),
		new("item", "preindustrial_food_squash_and_bean_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8641670d2216f3a10b85ce63bb1936f990f5e08fec879e176dde19e944367fff"),
		new("item", "preindustrial_food_stale_bread_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1dfa28948bad5ddc3ffc8b1a91f0d17be13e4d72853d121f7dfd3ca30a557b7"),
		new("item", "preindustrial_food_steamed_barley_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a107ce406b6f2096e581fe643605632daa0831e735d4c1baea097e38ec05eded"),
		new("item", "preindustrial_food_steamed_herb_grain_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ea554d4904eecacf3d027106d0f8226db5bca52159e503545330c86a2b69bce"),
		new("item", "preindustrial_food_syrup_preserved_fruit_crock", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "25071a6c46d9727b0c09cee38aca429b7039719ab0c7a32bba5017044eb450fd"),
		new("item", "preindustrial_food_tart_shell_blanks", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f64dbaa4fc3a53fc2866557f742d1aa1a53c797425f17f4be8c6373bf3d05ca3"),
		new("item", "preindustrial_food_thick_chicken_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4aad297b3b5ffd7fa80698c9db56f6a2c8f1656b88b5720c1a8a0ba6e22e787c"),
		new("item", "preindustrial_food_thick_meat_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "abb03bad8c986e8e11aa67c6324910beb277a5fc0e6de6ae3cf74be7b56c65e5"),
		new("item", "preindustrial_food_three_pulse_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2aeef97918cde276434257c41bfd7395a8d301d8af49978b70448b45baa9078a"),
		new("item", "preindustrial_food_turnip_and_leek_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7a46a6492e8538105216022272977afaafa3fe522acdb6210621feb16a32952"),
		new("item", "preindustrial_food_turnip_herb_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7bb52f3cc8416740d49fdfe877eef7ba997d8c5096837143b071a230e63da96"),
		new("item", "preindustrial_food_turnip_with_wilted_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1bfc0849712ccc1af5c5bbe21c2fe681d2019122da44882e693b1631039d9993"),
		new("item", "preindustrial_food_vinegar_braised_pork", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "983b75238c6345ea9fd88729aeab02b3bb12a8b871ebe7c7858f5066ba059a76"),
		new("item", "preindustrial_food_wayfarers_ring_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "94c99c731a85e5d825d0bda76eb48ea116aa4f04f4f3d08c1d5a76a11622b895"),
		new("item", "preindustrial_jewellery_hairpin_52", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5070374a152feb880ac5cadf120dd0f4b81e44268cea06e52cbb061515cd1a69"),
		new("item", "preindustrial_jewellery_waist_chain_35", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d1234682219efce3f932c5c1fa60ce67898e092b2261ed359c633fcaeac669a"),
		new("item", "preindustrial_leisure_chess_board", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9590952e93dd9f1da8f283ac700dac34f50d484463bc1b40ada3d8d6c85739dc"),
		new("item", "preindustrial_military_support_fine_bolt_case", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07b9c8b334054222d8335053e8c9b9d0145c43b085ccbce2c037145ccc664759"),
		new("item", "preindustrial_military_support_rawhide_axe_edge_cover", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8949887a9e2384f862b65ecae848d7a6d656a94574cbf134c0b690388e5aa5ed"),
		new("item", "preindustrial_optics_telescope", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a74632558c27de4b6885a91f37356608fda6d1eea45a3a12441f4e5156b1c5e6"),
		new("item", "preindustrial_printing_blank_form", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7237bc322111f4b6af89a1c10c90a75e6f3e55c8f2b7956499b3300c5a36d6d0"),
		new("item", "preindustrial_printing_drying_rack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d9eeb400ea55551dde7f1b28e5c8108ac164d78bc9a2cf1676eafcaaeef8fcc"),
		new("item", "preindustrial_printing_hand_press", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a263bea8e7922899161f60117616edf905d516ed7a89a8bb04fd3f559d6f516"),
		new("item", "preindustrial_tool_jewellers_burnisher", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "405f6eee7d9f15ce0c418614e26558cfca120403b879febe7cb54fcee0f7d6d5"),
		new("item", "preindustrial_tool_locksmithing_installation_kit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb5d3d96d82b325c28fe37b490ea66edd0c762892faff54c839ce8ae3772e948"),
		new("item", "preindustrial_trade_blacksmith_scrap_bin", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e988e37beefc8ceeb487b474efcfd9ddb5555573d0069f522cd6c64f9c274b53"),
		new("item", "preindustrial_trade_bolt_cloth_chest", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "615e17dad53cde4d18cb8ace4407e5691c692d7c21e475cf6978799b190f1718"),
		new("item", "preindustrial_trade_cotton_bale", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "efd49f5d5ef3987f73136c840b8d9455b0c6469a4a425c455f542ff6e36d5f7a"),
		new("item", "preindustrial_trade_dockside_bond_chest", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e289762a65c2fefe9475a1159a5f0f015c328ab1564d2db7b6c125a3090502d"),
		new("item", "preindustrial_trade_oak_malt_bin", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d664c9b6524c0e4a56dc3384025e66d2a28885be17ea359357789560e318fd70"),
		new("item", "preindustrial_trade_saffron_sachet", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "159c301cb90233861ab4d4ace7879e399c4ff191d0d27752c3e0f97b022b2913"),
		new("item", "preindustrial_trade_tobacco_bale", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e35a1e2a29fb909d0cda3c1fdd07bdb7f2e66054a224521eb8b081f718150692"),
		new("item", "preindustrial_water_bronze_fountain_spout", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8f336f8d4f098b18124420b5ea612e81396307368a1a0f13c959387392f265a"),
		new("item", "preindustrial_workshop_book_press", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c109d62673a58a59f24b0936036d2c46904f4430a26453dba2441b9ca7413ce"),
		new("item", "preindustrial_writing_qalam", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a26e537164bc1ee2e4ced27ab819f02db9eb2e251fdc5050daf4b4cf9d2f4aa5"),
		new("item", "preindustrial_writing_small_parchment_codex", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "63afdad12eddf9626e2aafdbf6643e575b14f04654ee3da0abff8de9636ea843"),
		new("item", "primary_production_charcoal_rake", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "420f28aa0b14d384cbad726d63f9e5aad03e0f6e1bd6d23b5ade5a0ad4b28665"),
		new("item", "primary_production_mattock", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "14e53a74485db86fa282cdb169b6706c5e92603c61a2fbdeab0b84e8e9885a41"),
	];
}
