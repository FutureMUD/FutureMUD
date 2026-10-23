// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch052() =>
	[
		new("item", "medieval_tablion_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2b70645c78df81f6923d69956d3e9f2326a26bc74bec01a32433583b2c56546"),
		new("item", "medieval_tool_bung_borer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "32a14a6f5dc7b0d6cd277551cd6578fad4a3c42d9d78fda3aec1072e20940bb0"),
		new("item", "medieval_tool_dehairing_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed81b752c6a848bf0ddf9e1e35cae10bde10d2e3debbd52e957586aa3aa93b90"),
		new("item", "medieval_tool_fullers_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "69249b667d1f9d514e4e70e6911ee05e2da9791dbc127aac737d4896ffdcf939"),
		new("item", "medieval_tool_glass_jacks", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "65523dd3ae4af2015d6a1c50e4059b94c1490c86f2bdaf661003bad39107b068"),
		new("item", "medieval_tool_leather_paring_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad73b62d11a7a8fd0ac8079eb17d5a7226d6978cdfcfed192924d5b18c1dd2da"),
		new("item", "medieval_tool_ore_crusher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "24f2ba8a628b174300195a91de452939222277ef4c827fc8c81e75a1abbeff94"),
		new("item", "medieval_tool_parchment_lunellum", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a22d2ee8b64cf8484672d4e7a9c0b5cb4da75d497dfaa877bde14471d4c0347"),
		new("item", "medieval_tool_potters_wheel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "99d6e41555211991799a6809bddc7b95e8e9e2f12a065422f052de730c6aa88d"),
		new("item", "medieval_tool_quill_curing_sand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6f45ff4e06db0600a23bcfdb14d09421df813a0160c772ffb77d26d5fad15f5"),
		new("item", "medieval_tool_winnowing_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "19655ddcf48cfcab17a5d708db03312fb1c3574e329ab8bd072eb2fa1046006f"),
		new("item", "medieval_tool_wood_clamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0119e5a0c4edfba116e2a6790ab68bdcc2282223ba19438641d7966f4d51c86f"),
		new("item", "medieval_trade_ash_tool_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5037d450b534d20be22bb0599086133209c36b4abb44a86211ec91dbe26778f"),
		new("item", "medieval_trade_fletcher_feather_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d92e2dc72bcedbc8af77d964df12725ea3ed9efc6bbd880bab26c47dddf86c1"),
		new("item", "medieval_trade_nested_packing_boxes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6d8a35bf595a4ffe2c7ec50526b4d0e166d0e6af9b376008ae526a841422d09"),
		new("item", "medieval_trade_packed_glassware_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3c0e778ea7dd38a52254ac2adb5043fe78758c16c11e06f1f593b9856c5dae9"),
		new("item", "medieval_trade_river_cargo_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc89fd2e161dad5c02deb4616a2499213f0ea5ac4b0808a3560b90af81f57c3c"),
		new("item", "medieval_trade_small_dry_goods_barrel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bce6e9681a2986ca8258a79659cb3ed70a83dcbe94b3b0e9757b50b5019fe661"),
		new("item", "medieval_trade_wool_fleece_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f55593063bd36bac429701e58b364d000052940799f71ff99c552cf6699d7698"),
		new("item", "medieval_water_controlled_stone_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e625f878daea2559e2d5099d4502d7490fd01eac1e1aef3781a47c12297b64d"),
		new("item", "medieval_water_east_asian_cypress_bath_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "759cf0d67e3e0df76cddf0342bb591cd02329e24475c7f6b3dd30e1a34e9af5f"),
		new("item", "medieval_water_oak_livestock_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8af5d7d41e241e77a0d01791c34da03900fc9a838cf7fddeba5908c7157e897a"),
		new("item", "medieval_water_small_ceramic_hand_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea56c5f61a304bb3b215e44be5182f7122ec811094b41df706a5f4db18c89c09"),
		new("item", "medieval_wool_paenula", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f21579a70f6fde8d214e808d75e5fd1358be9266a120522bf29585c720c62a62"),
		new("item", "medieval_workshop_lime_kiln", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "62acff1fc156b1a206c2e5f83b646f616b9b7af14cf8348253c55bd3c83a92f0"),
		new("item", "medieval_writing_bone_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "96400eb25fb7867387f0b6463e5f2cc95c4af344298efed78f13ec48baa1e37b"),
		new("item", "medieval_writing_bronze_signet_stamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc1776ce6a3f8943e71eaba8c009867244b53a6391c1488681c5726d3fb7c747"),
		new("item", "medieval_writing_copyists_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c639d11492608f2a98e8f39898a1959d8fa8728416f19416e2d4aa08c9641cd"),
		new("item", "medieval_writing_fine_parchment_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecab4c8c1af81e598d4d609ededd607868a5474f339e4b1b7bbcd76115db4b18"),
		new("item", "medieval_writing_linen_scroll_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3de7079430b66a039c826f52ab981f62435a482e9dba6b3c47645eadf099e0dc"),
		new("item", "medieval_writing_monastic_rule_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "297865c0811ad5f7203c20e38d3f4974e4470eb1bf05819ef79f6fb6eeb05265"),
		new("item", "medieval_writing_paper_dampening_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b38898652cb6d7b9aa8d1edb48b9ed56c985f14ca55e3c61a6c61297e3eb9e38"),
		new("item", "medieval_writing_reusable_tally_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "41e270cd9acb1772833a619026a4e8f8491f919d91a138977fb82ee98b9bec60"),
		new("item", "medieval_writing_scholastic_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff79a7b8e5fd1be7c8e93bbbf38648f441efbf2795262404af06f7a5ea49a4df"),
		new("item", "medieval_writing_scraped_parchment_bifolium", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ea33cf1ff8c46c5b15dea8652e29f8643193fd5c0350ddff84b5da7718c834d"),
		new("item", "medieval_writing_scripture_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "df296341247d34f2de4d37ec3a06d3f544353528baea0543bc1218706d8b80fd"),
		new("item", "medieval_writing_stone_inkstone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "85dc2b617535d0ba7c5816c48c3c5bd5c5ba884a3eda1bc99c063bffc5af5842"),
		new("item", "medieval_writing_travelling_scribe_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0481c9927cf4501ab8583a882f5cfe95924351e876dbd6ad9260e2a42aee9c30"),
		new("item", "medieval_writing_trimmed_reed_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5bc92acfd58dd65e38e49b3887c84225faf037fb1b893ec720a5cc6c461ac1a"),
		new("item", "medieval_writing_wooden_practice_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "175b65953fbfb8d0ed947fe756a5ac50ad32e0fd4c9a4ea3f3b654f8157d770e"),
	];
}
