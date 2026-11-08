// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch051() =>
	[
		new("item", "medieval_tableware_shared_steppe_and_caravan_travel_meat_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "05632b12ac84d9e7411b3959d320a75b7c6f6cc196112142691c2ffe3e491d93"),
		new("item", "medieval_tableware_wooden_ale_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f95dcdfa36a2547b43e686fb6620688cfad6c988ce0fc85891379e859164e7f"),
		new("item", "medieval_tool_charcoal_rake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0acb27ec052012387a85d1b45ca88a87ae2eb2ef272f1f6a77d9efe232381654"),
		new("item", "medieval_tool_crossguard_fixture", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "80d65d4f93fbaedbf1a2e8b982acab911b8c0481f4131328bc71dbf3e9ef3833"),
		new("item", "medieval_tool_cupping_vessel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "394b35af911dddc5957b516802c5c6d0ff87d6d0711a24d4977cb1b445822716"),
		new("item", "medieval_tool_fullers_mallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c0d22d65b97129b3117c12462c0ee85fee95ed8ae6ec8cff553a3cd237ca7a6"),
		new("item", "medieval_tool_hand_saw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f065f025d243d298244a736fda0e8c7f0a566eba6956da6d548dd11dca91de8a"),
		new("item", "medieval_tool_hoop_driver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e610f6637786cbeb81697fc0e232cb157de585ac82472c6a9f45184f0091611"),
		new("item", "medieval_tool_impression_spoon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5657a7de2b63d3c78772285526ab0e05505a8edfaaa39b18b5c56524af83dbb0"),
		new("item", "medieval_tool_niddy_noddy", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ba9599696b980e4f386aa36a3c069c0b5e15ab4f0abc13a61a34a5595ad3568"),
		new("item", "medieval_town_leine", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e003664009d9cc8c2443d3b268a87e308d5963c65a8e000cc80acf671b6069f"),
		new("item", "medieval_trade_ash_fruit_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "90ed7bbc4223999b67b46b387b6e61176410a200b25524168fcb31728b57d36b"),
		new("item", "medieval_trade_bowyer_sinew_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e8af009fcfc4e63427384a045f32062e228488d3725592be9db5276adf3c70a"),
		new("item", "medieval_trade_elm_sealed_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b392408f469646514ddca521b15c1d8b4c8fe15340e512e6836ecc77eea7fc4f"),
		new("item", "medieval_trade_glassware_packing_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd274f93b6a92394ff91f4d1f3df23331ac32cb92d854e32f6b91add2cca8143"),
		new("item", "medieval_trade_jute_meal_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6ff28471e1f8fc510b2e091123c08764b4b694ed558ce381df9d33b0bef1f92"),
		new("item", "medieval_trade_medicine_trade_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dfc491030b3ea4908e1f60ef43b16258c6b1b0e08dccbba66a50979eb5c03fe"),
		new("item", "medieval_trade_oak_ore_sample_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a03289f4f8d40f0c12d8be1b97381544a1fe5e6b129a57b6c55631a8dfadfb7c"),
		new("item", "medieval_trade_pine_flour_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "81041866283c8d646944bf53bde91779478917770e458af40b579c98363b2ce5"),
		new("item", "medieval_trade_pottery_packing_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "370eb362cde0a25818809a06e7f5d1608de7286f421d8766c04a92dc7f41c431"),
		new("item", "medieval_trade_pottery_shard_barrel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd3f70d1a0c3779e9a2e4f40cc7819f75834ceefee75a5dfb113440f1b110ab4"),
		new("item", "medieval_trade_wagon_cargo_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0b2ba6467992ca903516c1827942a9b9fc46d21aedcd2828710523c876b0050"),
		new("item", "medieval_water_lake_draw_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad1b3129866bd252a7624975ab6281e5bcab7203ba7aca4ca13a8182fa0fe245"),
		new("item", "medieval_wool_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "63d0f0840eb3e4d773451f257fb70003fff803db61660bb20210d1171e3b94e8"),
		new("item", "medieval_wool_leg_ties", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6507a890cfb1cdd7add1b34f5b34fa6abbbfadaf7de1bb537610a6b0a35c078d"),
		new("item", "medieval_wool_leg_wraps", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac5692f5dc2d68be92697f83d0fdef7bce14476e10eae2e2afc76d8536515809"),
		new("item", "medieval_writing_account_table_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b2a585b7fec51da9e17ffd68f9b86e6d835ad7c6a3ab0b73d2d0a867bfe817d"),
		new("item", "medieval_writing_bamboo_printing_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba0e990d077f398570be49c29a83d2a51918f6fef30a2edd060b9e47973bb7a7"),
		new("item", "medieval_writing_bamboo_slip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a84789fc805d1ea480835c13b15682501a3e1b00848d8f8701696819e41f7cf9"),
		new("item", "medieval_writing_blank_woodblock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c165cdd5966cd46dbf95a0a58f6796e28fdd6f1b0440a4360f7b91f99757edec"),
		new("item", "medieval_writing_block_rubbing_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e2ecd15d4305d3bb67c3299fdf0d15682e1a7ba92d8a50a2431952e2570c1c4"),
		new("item", "medieval_writing_broad_reed_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c7b4cf4c7c6cb9d39f1c12277d875709faf18dd5c735a9d546f964371a4d095"),
		new("item", "medieval_writing_carbon_ink_stick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb071d9d816dbdbdc1714dcf9c57e15bd48aefc46e245074a806158bb30028a2"),
		new("item", "medieval_writing_cedar_writing_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3efb0d9165693d4e812fba43209317f24870137f239d1f681f2d65be0c2e9f56"),
		new("item", "medieval_writing_chancery_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c2ec2caa31300c242faf637103b02333b06bad1de540798a87939e7cd9a11c5"),
		new("item", "medieval_writing_document_sorting_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5a4dc73cd4b35713df1bcbe89e82be714370c13ed5d50ecf78857cf05540bbb"),
		new("item", "medieval_writing_double_ceramic_inkwell", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fafd98a2a84ca4382fc5f38275a8e56429eede7f59f1572d374ec1a0f1101f20"),
		new("item", "medieval_writing_east_asian_official_slip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "81815d38b5f4ecea6312fe6fae6f22b9e87f88a65376b6b93842c37888c04505"),
		new("item", "medieval_writing_gold_leaf_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d00bd3ef5fa89953437c4b9aff7ec3b6112fe413f48ad53095f3b389029aa6e"),
		new("item", "medieval_writing_green_pigment_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d41c21f47cfe9225fa8d49088cf4de025328c0aa2d8b0339168068532353d6f0"),
	];
}
