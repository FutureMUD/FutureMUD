// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory11Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch053() =>
	[
		new("item", "medieval_trade_rope_tied_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf6b2710a0e46c40010cc27bd0c1d5a1164998038328dc53e24bbc930d8a7714"),
		new("item", "medieval_trade_seal_and_weight_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4420ec71e6c959d95322bfa476306c59580b3c1a4c38e21aaf2faea36bef810a"),
		new("item", "medieval_trade_tanner_bark_barrel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef321f8b743daa50badc2168ca3a104f9bbb5000b47e540609d1d159f9363e64"),
		new("item", "medieval_trade_walnut_merchant_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "02eb9004023a0784c05cf24320c5f0330faf149414c2aa6a1dd2765c077dabf9"),
		new("item", "medieval_trade_wax_tablet_shipping_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "70dc130c0722ad86591cdb5b00d405c2ebe16e45da5457598c2ca0fb4a9160f5"),
		new("item", "medieval_trade_waxed_canvas_spice_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9358f22352fe6ad2e8788aca03bb6bf65aad2dc53a8e05440875d92bbc41d05"),
		new("item", "medieval_water_butcher_wash_sink", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8820ac98021d8e4548a4770485487bebf9e887ba88ba7966690921b2491fcfa"),
		new("item", "medieval_water_stone_scullery_sink", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "94d5fb3efac19281d01fdde46b3200c1a9b9db6a4de8ae9c8c8cafca995150d3"),
		new("item", "medieval_wool_izar_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "44ba53cde1f71650beac2f4d936f1f0d1cdfc624df5b1d02fb28acd2b29e424f"),
		new("item", "medieval_wool_kirtle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9cca169040b97eae8528ac0928d6229d7b2035bb34eaa40aa7bfe35c1af2986"),
		new("item", "medieval_wool_market_headcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4238f14e1918af2053cdba875bb020aa09f372e868f9a9e867b3c5de5ef10796"),
		new("item", "medieval_wool_shawl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4ee99da496c1f040412e581a86f5a834f65192e90781157e0738981261ee0fa"),
		new("item", "medieval_workshop_book_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcd88f04411533cabd4b22fa3ca34d7e09fea8ee1692f7f7b22a2b391ad6de6a"),
		new("item", "medieval_workshop_glass_glory_hole", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "09e0fba6a6367841dd9dbb783e2f173baba7e4d466460d618fbff75c2969cc74"),
		new("item", "medieval_workshop_lauter_tun", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3be7af72354cce65c6845fdf88e7b2e8a86d217a92d5adffdcc704b8c0df3d28"),
		new("item", "medieval_workshop_lit_forge", "medieval", ["medieval"], ["item:medieval_workshop_forge"], ItemSeederOwnershipPolicy.RequiredRelationship, "194f14e2cb93ff3c71ee93d64aa91a4d56968e5faae134ff6766cb29dcb24540"),
		new("item", "medieval_workshop_lying_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b08fa3fea544393954b34fd4bba6e6c8ec0fb5d2d5b3beff28b4f3dc1a44ff95"),
		new("item", "medieval_workshop_mash_tun", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f33564e1cc379f4b39eee4e71da070aa56a8303054db5e61c9f86f1c0e5e5cb"),
		new("item", "medieval_workshop_ropewalk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd59f675c17a0914d1f2ea32b2bdab6ab075260145a168923cc3e48166a0b66e"),
		new("item", "medieval_woven_kosode_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e82fb08ad3753f52a6a5cffc06d393514e943957d021a23e4ce010c16a84e433"),
		new("item", "medieval_writing_block_carving_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fc8900c27970ae9d10ec5b1ca21d28fa666a24d4145f7d62b9b74dd80105f48"),
		new("item", "medieval_writing_bronze_framed_wax_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d59d9d3f8fa837310091ad95f82dabc76995192257a003571545533befc458db"),
		new("item", "medieval_writing_cedar_scroll_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f74d9b4a8c77769a0796004d37312f39102e684e08d4e1303d20377a32a7861"),
		new("item", "medieval_writing_clay_sealing_lump", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e0d6b143c5d8c62a2a7980664fc0e50cfae5fff29108304ff759e88f7e3eba5"),
		new("item", "medieval_writing_fine_quill_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "624766d30a1b9f45b9f7dc6e4180fa08cf201decb733cdffd8c354736965ad5b"),
		new("item", "medieval_writing_lead_seal_matrix", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e132c92c9ccfec8b6952af961be6710138bf4015ec16a7211fb815803b0e6f15"),
		new("item", "medieval_writing_palm_leaf_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ca7b946e6205847ba6996f6f80e6ff2223c95e7e34cc4abfbffc9b428a1937c"),
		new("item", "medieval_writing_paper_sizing_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f03525e6986479f70ea03669c41b02a18e2f07d8a17fdd5cc0e197fa8d9c80ac"),
		new("item", "medieval_writing_parchment_whitening_chalk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "07b282517bde7d532bb467d03b7f844203cdb905af55e8f40b21fb5824e65e2a"),
		new("item", "medieval_writing_pen_rest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "99a00b55ecc75ded37a4ddf03b5d6583478e22f9d4729f4fb3855d34f457728d"),
		new("item", "medieval_writing_school_tablet_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3388fe65db7544391e12a33b4be4f33715094527683a2137053f82d65902a12b"),
		new("item", "medieval_writing_scribing_chisel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "22776e4cb4ebb306f98708940ea4821bb0e3d47cf6a6dadfc73336005e4bcfd2"),
		new("item", "medieval_writing_scroll_end_knob", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "abe0cc7c26386e7e218591d727dccee0718623ad050de1839c4fb8b74787d80f"),
		new("item", "medieval_writing_scroll_roller_rod", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "876e4c0fd9f29fdaa332f0cb7094a3f82c74c8752c29cddca6ac263ad3230540"),
		new("item", "medieval_writing_stone_pigment_muller", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6810745bfb631e27529e6feed907613f1c6e2c9c35cecb906e03c2515b125b41"),
		new("item", "medieval_writing_wax_triptych", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd89d067aa996f92c63206e3d7d06e194971f6133a4e81ef27ea9d9fbf7fc9d3"),
		new("item", "medieval_writing_wrapped_parchment_manuscript_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ab088d34c4445b9c9682c73bf890aaf1f97d0422ed0a22ddf31c85d08811da6"),
		new("item", "preindustrial_door_hinge_pair_73", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b959c1bafce19fe1018703429be1a7c6e1b0a27e0211a87e78c520ef7050203b"),
		new("item", "preindustrial_door_lockable_plank_household_door", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4cb27608beaacf511174bc96751d80e5d19e4a45eaf795777b42e5a58b107757"),
		new("item", "preindustrial_door_warehouse_gate_23", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc421a32219d33411fc77ded254487d3d76897ea3a04fc0497d671dbb61639d7"),
	];
}
