// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch053() =>
	[
		new("item", "medieval_tableware_silvered_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "654776fe00810956fbf83098f5939971516518a200d33d9ba2a289aedde4bbd2"),
		new("item", "medieval_tableware_small_nut_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a046f1fab9b98c806380de6f96c0960cc108a5613f8e7e8d62c5a41ab7c21a58"),
		new("item", "medieval_tableware_stoneware_washing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ed86e2a2c84ac47676880628f0319c90e778702a480c9940a2261c14bdda738"),
		new("item", "medieval_tableware_wood_hooped_water_bucket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ecbfa543c593c6a7ecbb6614f8aadde73317b654c3dfcb5b300a9b9c2c42500"),
		new("item", "medieval_tool_crucible", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "711bb5d04d0a2f7628c02741725d56c297d1ad76971578f175b5818f8e771bdf"),
		new("item", "medieval_tool_mortar_and_pestle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a93c64c58965eeb6666fdfdbd3fd5d84a178da67b16ff602a174ee78a7e1626f"),
		new("item", "medieval_tool_point_chisel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b668bad4e43d698fdf295f1db08d2ed47245b65cabf2182cae5f79df1200c748"),
		new("item", "medieval_tool_rag_sorting_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b9f251a1d9895b9a526e63e92e33a960b6d3fcc0e405cb2a02d00cb56a463c4"),
		new("item", "medieval_tool_smithing_punch_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d846b0e24cd6422668b85fc741ff6fad80ad767b9c3c831b7a3cf1ac3ef2ba1a"),
		new("item", "medieval_tool_splitting_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "df220f75043573ca0502ef63bf94d7671a515c3148cf74bac5db25682b73f02f"),
		new("item", "medieval_trade_hemp_scrap_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4c45b6e2fabb4d303285cd25c798a493e96e98055642062697b1d2611fa2b15"),
		new("item", "medieval_trade_merchant_counting_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9bff6b8e498ae582648119cb1b662584386ed7adce543809ee97edea0bf8f2d"),
		new("item", "medieval_trade_slatted_pine_produce_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "53b23ee667520ddc1ff582c80838971df30554ea9273d346f96025491c2d9ced"),
		new("item", "medieval_trade_stoneware_lidded_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "566c1455d04ead6c419ce6ed8695c221cc087845ff315f5179d5ca64e42e0809"),
		new("item", "medieval_trade_terracotta_grain_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9589330fc91a172f549a3748de3849fe77fe6ccff7be1e702be1c14823105fb0"),
		new("item", "medieval_trade_waxed_trade_barrel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4efeebe6f7bab5c5ba1e64cdfd7f5f4bb9481956c9cc241258884627744cee6c"),
		new("item", "medieval_tucked_wool_work_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "320fbed55877151f2a1342970fe071ea428d076af37dfe596fb4eb878c2eafaa"),
		new("item", "medieval_water_east_asian_stone_garden_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "821e1a0931e18314c4a2c460d3d89f94f2a9b26f0a7dcf6f5f400b71aa2fdc53"),
		new("item", "medieval_water_iron_well_pump", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0df882a53d1b7a6ba8adff91e17f0f05475a7b9fc596b4855666c5fdf35443f8"),
		new("item", "medieval_water_mediterranean_terracotta_cistern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ebba236f3330d05c7e4e2e9417db604d589a19c5431134fb6aa3086cff1e57f"),
		new("item", "medieval_water_public_stone_cistern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b246ff2c33de7a8360ef0a2a56dbc4d71951ebb668ae853d66495ea446c46ccf"),
		new("item", "medieval_water_refilling_oak_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "26c91b517bef8ed01487581d6090284656614e89868ddddd3eb2b7d3e5306393"),
		new("item", "medieval_water_south_asian_stepwell_draw_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "10b8e3ddd362e755cf9a735416325c78d24b7654f367c6bc79393f93507e77ca"),
		new("item", "medieval_weavers_cotton_shouldercloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b084eedf5708ca03416cdbd3ba26949c1d6a8fc54db9effcbab00e0b3d429633"),
		new("item", "medieval_wool_jubba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "73759070d5c0b40d30e4a937e3a41318a391e7da30b35d0c09b9fb1ba14994df"),
		new("item", "medieval_workshop_bake_oven", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "15a040261453807c9fdd484cb5ce5595512180cb5aaa03a5ebdcbd4a9acc4978"),
		new("item", "medieval_workshop_lit_smelting_furnace", "medieval", ["medieval"], ["item:medieval_workshop_smelting_furnace"], ItemSeederOwnershipPolicy.RequiredRelationship, "049c8fb2be6d676293573fd87dbe65d3f522e33b879a7b7063752b5ac3925672"),
		new("item", "medieval_writing_bamboo_practice_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f738f28e9f587e18c06fd1c3ae952ecb1f7bae1d908d6138834850fa8b26f081"),
		new("item", "medieval_writing_bamboo_scroll_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba06e2063ffeb77eedf52ecdb2fcbb69463268890c4971e1463c64b5f21c4a6c"),
		new("item", "medieval_writing_blank_parchment_charter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e0a10e4df464e58a5bc2eab705a8d034ea38e441b812c04031cb2b33828d378"),
		new("item", "medieval_writing_cedar_book_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4d1d0cba32864bbc4ab5ec1531002c6fdb4a02fb4b8a4f5708ebe0bd00b56be"),
		new("item", "medieval_writing_charter_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "11deb2d3eaa8924b7483bba4330b3d67514b5ce6a453773010dc2c0fbf81b4d2"),
		new("item", "medieval_writing_iron_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b3ea8b799aca3b7fcab6bd681d1a853b42525f1b97f92ea776e4665c5e8eede"),
		new("item", "medieval_writing_long_papyrus_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "87fc31c4b90821c4f90f49ee6d07a392055ec712e4bb3f8de00870ea32149b4b"),
		new("item", "medieval_writing_miniature_detail_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e92b7dd727b08715b6a5ba8307f15d36276a7c8859ab122a7a5489d17c1b7fba"),
		new("item", "medieval_writing_palm_leaf_wrapper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "16d98ea9a292a2583dc08e78c6a9a618d6fc0fc8037ae952cda6104352b1621c"),
		new("item", "medieval_writing_papyrus_archive_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a186e76f992e624a3a7b95bb7236497725ab114437ac4d849f7cd81660f61e8"),
		new("item", "medieval_writing_print_registration_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1536c15c000b4b3c73464881daf2f7350323667352b3bd103579c84a9fa458bb"),
		new("item", "medieval_writing_reed_pen_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a75754ff0472fb3a3f44dc3409a756b64033660c75a9c7ac805e8d8574e063eb"),
		new("item", "medieval_writing_seal_matrix_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe332da2a0a650cc1318a9f6e83739605a792c868e48f6eed9fc28606353477f"),
	];
}
