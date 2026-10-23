// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch051() =>
	[
		new("item", "medieval_shared_iron_hasp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e7c856f0b2800938947daa7ef71634aa4eb857543180ad1aed13c0195db70f3"),
		new("item", "medieval_shared_linked_key_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aefb5a7a7daf63689754651dea63efdf4c266f3fea7260ac91d6e6cb8bcd2651"),
		new("item", "medieval_shinto_white_joe_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "626b9291c96e38a41618d1f2b75d31331785815f787d414493896a712d1914c4"),
		new("item", "medieval_soft_leather_shoes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "11e68a94ee417f79d263fef62e70974a35c3443efef0473a7670682554a6966b"),
		new("item", "medieval_steppe_pack_chest_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3464c30871e395e2616f5e73dc03d4d4db1f96e7bcfe88d0eaba551ce130e6ec"),
		new("item", "medieval_tableware_brass_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "50ad50ce75bb6013cb7a5f1be16455fdeb03368b8d2794ac459d27c09b7569d4"),
		new("item", "medieval_tableware_brass_tankard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d8b43b643391559b37825abafebed17b2a201bc6c8f97e7972939c10a6b1592"),
		new("item", "medieval_tableware_covered_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a21c4f965c26fc2a2ee17a87c2216f94b35d20f5d3037275603161a7821c6077"),
		new("item", "medieval_tableware_earthenware_bottle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bfaae6cc44c27938d7b1948d480dd4f637450a9af9b5c63dc2d3e0a7894c10d"),
		new("item", "medieval_tableware_glass_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e98fe0a29b1ca44f7458ac943f971f0e835db4027375e7d831bac99817fd4ad1"),
		new("item", "medieval_tableware_lidded_stoneware_stein", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "65e14aee1b92d5f35d9924f36b98f2125aab15037ff628011df766290483d452"),
		new("item", "medieval_tableware_oak_tankard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ace27036a7bc4811ae957bd60958cfb23644bcd70db083783d970123a3fe291e"),
		new("item", "medieval_tableware_pewter_serving_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ca88c8ca9864950b07db2c33f878db23f68c1d6143ff52a267b1975c0c5b27d"),
		new("item", "medieval_tableware_porcelain_serving_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ca45bd151a5d38330d40628a99fa7ff18dfaf5e45adb4c7ae6a3d64c8ccd22e"),
		new("item", "medieval_tableware_rinsing_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b32da4146058ce01a6d626f35088986d619ef653d72152a689ff6bb83d2c8d87"),
		new("item", "medieval_tableware_shared_east_asian_bamboo_steaming_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "de2fef7bb256697b1ca1bc91f3f410b1679521b82014f03b78bfb9d75605cb61"),
		new("item", "medieval_tableware_shared_east_asian_covered_food_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a79d9b5073fe19b061286157167a128b789257592d5d8e17a5ac9033e79e04a"),
		new("item", "medieval_tableware_shared_east_asian_porcelain_rice_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc78648b9da09c6a2d4103194c72dec9485a8eb286933ef96fc4dd9259248508"),
		new("item", "medieval_tableware_shared_steppe_and_caravan_portable_cheese_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b4b2a72384c717d549a1f7d029d4a7344cce0a9bcf522db1557b3c54acb64bd"),
		new("item", "medieval_tableware_stoneware_porringer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7ab0a7789deaa89f36f468810793c4aafdd6f56361445a8cfa5d9424f64d043"),
		new("item", "medieval_tableware_tall_ale_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f24dc6834b7d2eec1f44331667dcd218a29043ab7f412384650033b6da53b736"),
		new("item", "medieval_tableware_terracotta_serving_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "21987cc2e4e6ffad75140c348f17e2861a1c033446e29d6a56768a69e1b61b4d"),
		new("item", "medieval_tableware_wash_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a6a88edef6178e4033be03f03f54a7d17dc8d17139197535b60e3481760243a"),
		new("item", "medieval_tool_block_clearing_chisel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a10f99f198ff0eecff773765ecb50d810d59baf463f104c16d5304fe99b9666"),
		new("item", "medieval_tool_butchers_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c78cc49569ea788ef48c6607ce49314e1fd0c37393f58a77db283732bcab1326"),
		new("item", "medieval_tool_dye_stirring_pole", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "87dfb639bc89330dfcae33d97e8a894c2d2c123c76fb877390725a1a460cefbb"),
		new("item", "medieval_tool_ink_dauber", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6db08c963cd12d6246b7be2345813b39aaee677c6004faf34fcd3a90048fd2d6"),
		new("item", "medieval_tool_jewellers_burnisher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "649115398e8954981c12fe984008e8e0faa81153aeb2c4b09bc1da503be6ebc4"),
		new("item", "medieval_tool_mordant_cauldron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "af164b643a57a46a7f6919984427909c9e21014fd2c1efe5055348b50d0e1e5d"),
		new("item", "medieval_tool_quenching_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "18a304cb47d27c9b36f23fa18fdd8de68241b1f1957f36dae4a8021d55dc33e3"),
		new("item", "medieval_tool_tanning_paddle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "649f80804a922e7049a15bff0ab389eb8e318b299b38f17c359d840d92d1139e"),
		new("item", "medieval_tool_wood_chisel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4142fc26fcbdd8726a294b2c273b8ab81148548768c6dcd0ba30fdc7eeb49c91"),
		new("item", "medieval_trade_apothecary_ingredient_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "60cb3cb6f9cba3c767f23471b886a76cf576aed6f75aecff3cf65e7f14d8c4bb"),
		new("item", "medieval_trade_ash_lime_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d92056a0b5cc582429eabe5277576c1bc60246ae8c62c96c6cfe1204e3f9b28"),
		new("item", "medieval_trade_bamboo_tea_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "749ca92c161e234b6c269e1dc68213067ec7d13e8d93c71c7b99d8c5ae64e254"),
		new("item", "medieval_trade_beech_nail_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca66d76b11152d3be0b4e9d8116cd0becc1a1759b7fc5478be456a986b722d20"),
		new("item", "medieval_trade_charcoal_barrel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c0c56ec3745786551c419ba0a32584bfa55bc46db289e09cf2ed7a59d19099a"),
		new("item", "medieval_trade_cypress_export_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbadd2198dd05898cffc3fd3cbb8009a0ed724b0296825e1f046308630e8525c"),
		new("item", "medieval_trade_herb_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "22dea12a8e955ca2b9f92d4d0d98aa87c7c9fe4426704a20b21144ebf26c0e94"),
		new("item", "medieval_trade_oak_plank_coffer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee9c41e4a1052e996ebc0378a4315e21503acc69c7dab0188a1406200c7ece8b"),
	];
}
