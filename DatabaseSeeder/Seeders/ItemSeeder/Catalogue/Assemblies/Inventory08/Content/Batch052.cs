// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch052() =>
	[
		new("item", "medieval_tableware_shared_south_asian_stone_grinding_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "657084d3192960e3200b6be445983f86267e2c733de23ac630ed9f7574c57079"),
		new("item", "medieval_tableware_shaving_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7bb793a01c437b9fc91373159abb36d7ec0113af5874dda2ab02518ab1dfe75"),
		new("item", "medieval_tableware_small_laver_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e78087565982ad5f7f607b7e22dc0e3b2a1f4995ee2ffa1219b42141aaca517f"),
		new("item", "medieval_tableware_table_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb5e347bc534d103faf3dd67e7a95961714c0e05a90703d2d2ccffd3956b70a9"),
		new("item", "medieval_tableware_wooden_mug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f69d1eda0440ece3dba9d9ce55acd0296c04663541134b16befc8efd382df61"),
		new("item", "medieval_tool_armourers_stake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "55d47a0f1d06b59b8a8a389a2bf1b899a7592efef863df8bf503b559c80fd561"),
		new("item", "medieval_tool_bookbinders_punch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2735b5ff1f98c07ca4caab810492d4bdd6e284f61c24d8a5835209d440eeb572"),
		new("item", "medieval_tool_currying_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e424a8d0c8bb620e75b6e70c22be4db7f8e6f5b601cfb8949b19f6374c47fb06"),
		new("item", "medieval_tool_grindstone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d672d7ab4ff5fe0967da757d2d27d3100dbf5d9ad2e516eb0521d67e9c9d45f"),
		new("item", "medieval_tool_ointment_spatula", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b16b3b13d0b09f19b9ecdf89607ac9efdab1c63513b6c6d1a4d06f29fb3185a2"),
		new("item", "medieval_tool_potters_rib", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "59fea1b58a013d5b54ea69d3a4b7cdacb3a198e2e0bef5c0cb374cb2df2b531d"),
		new("item", "medieval_tool_surgical_probe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "51c512c5fc205224a57e8a2f4d21dad4fea345f71081d487f43aa2a6d02fde70"),
		new("item", "medieval_tool_threshing_flail", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9accde4cf720cd21f2c7ab97f3761eda88ac2b09197574b2d71d7024904b0a6c"),
		new("item", "medieval_trade_coin_counting_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b9b789f6be6e94c64ed7dcd80671da8daf5f4ea77cea0eb6af6012dde4b6477"),
		new("item", "medieval_trade_leather_waste_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "512761e173a9b74a21a677b2f7e2e09c1f5b2a93b45af943179f8f97634666da"),
		new("item", "medieval_trade_market_scales_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e71c819ed800568a23415fa74f1f2e0689be62aa3915fc344661fc66482d9a46"),
		new("item", "medieval_trade_tanner_bark_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4271b36e3dcd1146b55450a7cc7ae1169b516fa34bd04990537df99338b4be3"),
		new("item", "medieval_trade_wool_sorting_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "300bb383ef5d1e18e45b407104b42e8d62ee62af1a48f63a4e440a7db8621b3b"),
		new("item", "medieval_tucked_work_kirtle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "21c74cd1434ce4a2e705ccf71ca55cfda9a12300cccee3d13fd587ead828890b"),
		new("item", "medieval_water_mediterranean_courtyard_fountain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0bdffe119d5c3ba5ba587a9d26cd1936e67f0103c49864d474678212afb99f54"),
		new("item", "medieval_water_northern_bogwater_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b85f165cbaca75b8e528a15cf6e3792ef55e90673ad72fc547a3e61d5b547783"),
		new("item", "medieval_water_south_asian_brass_washing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0699f8856f3298fb8d8b2cf9e92431775482bf7c81f9741a51b0749f3bc0c0f"),
		new("item", "medieval_water_stone_wellhead", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ab43af5ef8edcbdeb93f47c3e8326457e131ff9d8a3f328750f828ee1bb0475"),
		new("item", "medieval_wool_burnous", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1ff230588104cc0f8db1b84609896f02f52d451ad036f0f51cab2457194ce43"),
		new("item", "medieval_writing_account_wax_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b1e0802cdb7bffe3d4bfa7f144bb89e9d8f38779b6b7fc8e2a5a51876f94dc2"),
		new("item", "medieval_writing_blank_paper_seal_tags", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6edc408caa9feb22d59559323b815eb025d4914dccbcbacbf3272e54b66abd76"),
		new("item", "medieval_writing_book_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "86c476789646717947ec4eb4e4b1ddf352b94d18a38a60cd879f3dc3d519b19f"),
		new("item", "medieval_writing_bronze_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "45e39e9cef55052757955a46f54e31d10c670ee454254556ff7dca82ebbd7016"),
		new("item", "medieval_writing_calligrapher_tool_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb1e8a6f137a4288eab9813a9e27cf3fdc421041eb1b60c79c6d7721f4b023a0"),
		new("item", "medieval_writing_clay_sealable_document", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc70848e8bb5ae71fb6f5364a003d9aa78ee76ed2130299868d6093e80a8d7ec"),
		new("item", "medieval_writing_folded_rag_paper_letter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aab3145a8a180033e59afd86016d618433df0b1757b5e7843295f953b519a2fa"),
		new("item", "medieval_writing_gesso_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ad1f1e0f66eb25174796c2e1e08e6948a28f20c757b6a76aafb7929abdba476"),
		new("item", "medieval_writing_inked_printing_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9294e981808fffb930663df46fe20ad4134afac0499f0709bbd394ce8104739"),
		new("item", "medieval_writing_leather_paring_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb1a479ef19c41c31c99612784458eb1fe2a720408adc0b77a62bcf4b1e9a175"),
		new("item", "medieval_writing_manuscript_rest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1043b2d7301a4ab2e4f253536427d4493e3d69c69f9abe7bcd9a03df6fad679"),
		new("item", "medieval_writing_parchment_lunellum", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9326d9a283464b357a8f84508f006937a1a2c1533f9c3616ecf19b2815a2c0d3"),
		new("item", "medieval_writing_pierced_palm_leaf_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d32dabb23d5d01f6185c097fc4de2432789258a3dc3b990934b5d9917ba2393"),
		new("item", "medieval_writing_plain_parchment_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ca41c8fce9ccc96746a1873d0f8ef1a423b68c2817d6376999607849a306bd1"),
		new("item", "medieval_writing_portable_scroll_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad08822af676007ba97506148657450b561dbf268f0ad8427817879c51ca7d8a"),
		new("item", "medieval_writing_printing_paste_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5e414fdee3f0ba4f1d4c40d6f55268427c01e104a3db3287d9253fbc5a2b02c"),
	];
}
