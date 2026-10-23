// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch056() =>
	[
		new("item", "preindustrial_food_spit_roasted_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b36ee104e94cab5626767ed8164ab765e2c33dcbeca709a043e51679251981f2"),
		new("item", "preindustrial_food_sprouted_grain_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cd1387094e25757a406a766206876797d47ded6a18fbb4c40c6ec14483bb634"),
		new("item", "preindustrial_food_stale_fish_crumb_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fddecfef4b2db298e72b5be8658a8a30a9d5b9c136b02a84a5783d6515dd3779"),
		new("item", "preindustrial_food_steamed_fish_fillet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9cb2b0b3c6bce58847956674c686e0d28d4da6390732d79ea239251c0d8fe1b"),
		new("item", "preindustrial_food_steamed_plain_buns", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0dbf721cc071fab12a12ea27fda0495451cd27203e01e0c01c225a996352b95"),
		new("item", "preindustrial_food_stewed_berries", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a811db930bd2b27124628d6bd258130f28920f192ad72cb39cf55dd289f682e"),
		new("item", "preindustrial_food_stuffed_cheese_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "da7a68647793c22d1be75fb48ea26756be41296982e99b83f5ff6dce817e2a8e"),
		new("item", "preindustrial_food_stuffed_stomach", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d63ce379f2d383e1c9ff6a34cf46a6f2d80c7ca37d520735577a8b13a99a953c"),
		new("item", "preindustrial_food_thick_lentil_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9429007fc71a5e6e85272508f1b91074af1198513a9f2abb5c64784c57660afc"),
		new("item", "preindustrial_food_thin_fish_broth_bowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b67bc81e72cdeeccbc0ff9d492b20e729ec8ebb8730081df89a19f493dec2186"),
		new("item", "preindustrial_food_tongue_in_jelly", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5e81ca682749cbd477ef3c571eb6201e4f2098903e8351df94aceeb51f7ce96"),
		new("item", "preindustrial_food_warm_whey_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "186c9328473cc737d08ca5ced8de315c6a2aaf2e151cd8368eaa96ad13df3f93"),
		new("item", "preindustrial_food_whole_boiled_crustacean", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "14f3b816277da882a5611e88523a8245fe417c6af405c30bba5099c1a4a16bff"),
		new("item", "preindustrial_food_wine_stewed_fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a90d468baa0906cc3f78fb79f406f04015aed3f0ae0c4f231305b7c1fd92fd3f"),
		new("item", "preindustrial_food_yoghurt_and_greens_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af800974c2dde78a1b32befeab98e934311c21540f1ce744159d074931d9537d"),
		new("item", "preindustrial_jewellery_diadem_34", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae85d059a398fba81921a13defa4408b78fd0b876e1e5c4be3048931c518355d"),
		new("item", "preindustrial_jewellery_ring_21", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6270d5e50b43361091b2f9812e6e7fcd5caf88101ea8c2fbfabe858a15310b1"),
		new("item", "preindustrial_jewellery_signet_ring_22", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c88d2b5d1e864539254caa9e8e22078dc043e819a2a6183c89379645c7ade72a"),
		new("item", "preindustrial_military_support_crossbow_rack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "174189589af0613564d7af92f53a68d9560d9ca7c05b7e1db9f89e167003c835"),
		new("item", "preindustrial_military_support_rough_leather_arrow_quiver", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a979103d4127162d56684c47e719c75fcdc3af673d81c82387418d37561f3a53"),
		new("item", "preindustrial_military_support_spearhead_rawhide_cover", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7bd8afb0b2e34b2a3afa58b504b6b2896e2b1e99a83af3ba9c5d85f62a360147"),
		new("item", "preindustrial_navigation_pair_of_dividers", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1386de86f9991c2da2f1be17331d797839955b472c239e1e427cb0c1af582bd7"),
		new("item", "preindustrial_surveying_plane_table", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f968707f64ee288fce9840794485c125582c7a4f5d898f708d565d41c7e2a4d"),
		new("item", "preindustrial_tool_crossguard_fixture", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "25c8163bb0275c524f6dc68fe0dd37565ace05a92544c19574b2b08bf2886a6f"),
		new("item", "preindustrial_tool_hand_saw", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2204254b7f5083026ff9f6f47bc5d1b03875db1cfe4e5de5cc13f6d7f5bf9871"),
		new("item", "preindustrial_tool_ore_crusher", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "54234a5afa6c08f6e0970fdb091c5c51878dc8baed728dd1c6386d55d934b9c3"),
		new("item", "preindustrial_trade_ash_fruit_crate", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dad626a3feb0fb442de71441c008bdbe74f1536f884d94456d220ae0865125aa"),
		new("item", "preindustrial_trade_hemp_grain_sack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4cdc20e916019cd6ea10d14257044f14ab9a71579b5da663c2bb79252649e608"),
		new("item", "preindustrial_trade_indigo_cake_box", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cb8557f29e1e20ab486c52e7dca9b62b02dbc239cd26a20bfdfd9d6be4358ef"),
		new("item", "preindustrial_trade_slatted_pine_produce_crate", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c37172a59df117441f70bed0571a263eb59590b0dfcf0a6ea3e372daaeb437d"),
		new("item", "preindustrial_trade_small_dry_goods_barrel", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ca62bb806fdae199e25575189259544c8720f9cfe65f182490aa0bac1892fe2"),
		new("item", "preindustrial_trade_ventilated_fish_crate", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f469e836f973cdc56b9522768336b72832cf5466f1f55d07add6bbe09337f063"),
		new("item", "preindustrial_trade_warehouse_strong_chest", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1447b57e33238e9f162530966cf325e805a7f2fc33ff31646bd29eddb990bb1"),
		new("item", "preindustrial_workshop_bake_oven", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db91a02410d5e79a4455416e769cbbc7fa7baff18a2a17bc89f507e6eff5d002"),
		new("item", "preindustrial_workshop_bookbinders_sewing_frame", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "331e62496366debe9ecf9ec96f66753acfb9393fc6a77f2ddf0e7379f119d566"),
		new("item", "preindustrial_workshop_lime_kiln", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07f9f4dc6b7046aea023c51cbaec6f43917c20477f7e866e3f49415a45f0410a"),
		new("item", "preindustrial_writing_fine_parchment_bifolium", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "032d2384b31c801d33c429c2e008bc563d51670101e32cb05574502c8ed968b9"),
		new("item", "preindustrial_writing_plain_parchment_codex", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eadbbf9dbb765eefbfed7c387702207e9c9227961ddc5b528dedbe846ddb7af7"),
		new("item", "preindustrial_writing_rag_paper_codex", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e881611b1d43180cd7925b1e97e9fcb6d2626c69910b8ae8424a272cdee048df"),
		new("item", "preindustrial_writing_swan_quill_pen", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5bbc1042a22d03e864a598659759d7acf024046c93fc579b01d067d74b0e735"),
	];
}
