// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch054() =>
	[
		new("item", "medieval_tableware_shared_east_asian_small_tea_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "28260de417068821d4daeab92ce7f4abc0f19629bf4f626f277cfde936612588"),
		new("item", "medieval_tableware_small_breaking_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf90f6e097da298572972b211d0ee759053f76e4ba9febc74bde8a460c441933"),
		new("item", "medieval_tableware_small_wine_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a136168a0726f27ff859bd95f9c264051ebbdd9067428e3c5dd6766672cc65bd"),
		new("item", "medieval_tableware_stave_built_tankard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "53d2f0c27bfe1822d145f79a4ae005e71a0e3f277f349b79265a0c368b8d4377"),
		new("item", "medieval_tableware_table_beer_cask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d4e90315429902e89ccd867ace68788c745d95f7be7e811c1c11dab19d0e991"),
		new("item", "medieval_tableware_vinegar_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "556b1154d57b11fd2597bd91a61edf5255a57be051f85d024191eccaf5bc293c"),
		new("item", "medieval_tableware_wooden_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cd7998dba097831c890e4aed2b7f9934397e3eb3bff4efdfd1b513642aa822a"),
		new("item", "medieval_tool_bone_file", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "90cf7c876f38816a52a913bab920687081ae93b3ad545e53e8c5a1f9190eca58"),
		new("item", "medieval_tool_dishing_form", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6db2dcc70d305599994afef006399b150c51874a1872e90970b18f8118c6c647"),
		new("item", "medieval_tool_flour_sieve", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "23c4c048cdf6e43a029bbae7f902beb99f2cc8316dac9eca77204923eca30b8c"),
		new("item", "medieval_tool_forest_saw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "de143d1e4ed75ecf0b3c8927757f0ecb16ba63c77e0ce44b67d6a7201d8077e1"),
		new("item", "medieval_tool_fuller_tool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "14d1eabed91d42cdae1cd42b049387a2f45f7d21883e95c1002f0ed4c8278750"),
		new("item", "medieval_tool_glass_blocks", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa7aa1406a7634bcad29c452881df197264276748562fe69e5d69615ceb3f061"),
		new("item", "medieval_tool_leather_creaser", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c70e847623c6cfbf5bab71d7417635c591da9ad96ce3177a7ccee9a2a240954"),
		new("item", "medieval_tool_lye_leaching_barrel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7358f18208307848dd2b3ab20765d98f18cdc185d3c0a483b097302ecc0da097"),
		new("item", "medieval_tool_manuscript_pricker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "481f7757c0d20b92a946fe1021307429c5f12828bc94125767506919e8ff4c8e"),
		new("item", "medieval_tool_masons_trowel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "33e180907e094407b8dfa5547b7e771f58d851fab5b0e59ff12b812a6c3b7bfe"),
		new("item", "medieval_tool_ore_roaster", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "375f1e79eab158d8ff1b44c0db0e7a1ca3b91d119769d0ea28833dabd281e2b7"),
		new("item", "medieval_tool_pounce_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bf72b317acab03852687c9cecbb13b19306857c2393be8e377a6c100ad7dbe5"),
		new("item", "medieval_tool_shuttle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b10cf5516e0aa37bb7b6cc4fa9308c885195e291c1e80a76e9dd1771352e7bce"),
		new("item", "medieval_tool_spokeshave", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "528d441abe4f07368f8381d19da2defccd2d8776676527bec01219fed679a454"),
		new("item", "medieval_tool_sword_vise", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a96cd90671a6edef405a2687a75d748ed8bb450b184aaa66ea7f11c28a09efe5"),
		new("item", "medieval_tool_warping_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb6c3a181fcd9f2eb542f397623fc983225764a607301f496530134bffe01e6c"),
		new("item", "medieval_tool_weaving_bodkin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1916beca3e2ea45a9a2a99a58be3ffdc9cc95689bb0cdc4f8fb5ca494e0ff083"),
		new("item", "medieval_tool_wire_cutter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4bc16e346702e8f7fcc33f99e3263416a17c24bce9aa77628203d99f137c122e"),
		new("item", "medieval_trade_armorer_rivet_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4193b3c0b89c5a09fa614855e6d5a0f76454bc9487cb78960ba7f5ca7029923"),
		new("item", "medieval_trade_balance_weight_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cae3b91cd7fca726e7b24689866e8d10d8ae403efcaec363cde5599203326091"),
		new("item", "medieval_trade_bamboo_storage_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "57a2b8f2b141460d36de4a2bdb527a6a76446160a058bde9c8413a64ba6c0d69"),
		new("item", "medieval_trade_bean_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f4da472023595211593c6839bc49bb78ed52ffb3e4e373b5e95af0ea7e83e0c"),
		new("item", "medieval_trade_bronze_ingot_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ed5af8e968b088cb89f98b4ba15ea638ccd84f66ad132c8c96e833f2e23705e"),
		new("item", "medieval_trade_dyestuff_barrel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "79dbf7216eaeb89c6a2386bc723fcd6a528f57b9fd7b9be2f33c9a7d350efcd9"),
		new("item", "medieval_trade_egg_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ace13b993d45fbbbac69f7d65cabec4d4eb9c1e2d336f632dd813a5d6e83291"),
		new("item", "medieval_trade_hemp_grain_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "77a4b76a412e9dd4500a742323b253e46621cc67ee80b2bfa8e8b56b65b4d0df"),
		new("item", "medieval_trade_large_oak_hogshead", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "74acaa64989218ce231330ce2dc50f26814efb064f007ec293c3f79b7b04b84f"),
		new("item", "medieval_trade_lead_lined_canister", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3067a58e04ddd05ed0d65a112371dc45cafc585ce3f7824071a519a6e9aa1402"),
		new("item", "medieval_water_south_asian_teak_bath_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "500631c4ceda421b7530b9cfc0d81f15d7a8a3de62f710586f2a430fd0ee6810"),
		new("item", "medieval_western_moulded_chamber_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "07137c9fa531ee54617b44b803e4b775e2a2175167e5107b66c9c5562c8a3109"),
		new("item", "medieval_wool_deel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee0d4937b4303c62e7414603568e79b565484533b4f3f81a372574c76c72e5b8"),
		new("item", "medieval_wool_over_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d473b147070627ae3c3223a0cb3626de87d70cb8ab3b954a46123f742524d868"),
		new("item", "medieval_wool_saya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "74a15a6857cdee3b700e53cff8793ab06bcdf858b7b25c99bf1ae44691ec2664"),
	];
}
