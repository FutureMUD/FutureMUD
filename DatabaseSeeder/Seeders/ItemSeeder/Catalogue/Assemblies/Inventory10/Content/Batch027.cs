// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("item", "antiquity_accessory_scythian_akinakes_scabbard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d87e0f6492158f58807b7169825292edf53560e54d1a16f8c80a4833e0b6e946"),
		new("item", "antiquity_birch_collapsible_rack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "85e2c05c9d758f096c474959b128a02ce58f4c54b5be77cdeb13313affb70241"),
		new("item", "antiquity_bronze_forceps", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "263ceaf2bfdced8e55cba4e7e63d584ea3e8a320ed0a6bdf28c63187cfb4877d"),
		new("item", "antiquity_bronze_war_horn", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a9bdb07e7e81c4ed9e468043fae7e05a960a90323b23ecc561d06aeab10d112"),
		new("item", "antiquity_bronze_wood_auger", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d48b7a22e45b8a201c032bc67df5f40401c06a00f68e65cc338187096c78a19"),
		new("item", "antiquity_carved_elite_bed", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ac7668476f4ae50a332e05857d31e05309de691f544bc419950831a4cf0e6a6"),
		new("item", "antiquity_checked_wool_scarf", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "02e02b15107b1f4c0f81e4f9b1439b1bce93752562dfd441af83e0bcff8de7c0"),
		new("item", "antiquity_city_sally_port_grate", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "756ee70b229b70e516a2331a450f162bd5816feeb40eef38d84af6d92d343d8b"),
		new("item", "antiquity_drop_spindle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "197c7553788c2f869f999431bcbe6a91be65d7a7b722d4cc2b163b48618831b7"),
		new("item", "antiquity_egyptian_scale_kilt_guard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "abdd1812c6d2b977085f1f5db338de939da3be29ba0c54f0820e801be82d68aa"),
		new("item", "antiquity_etruscan_anatomical_bronze_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a75a9a66cb2a22ccde6e8240fb1d5a9acdc1beb7e78a9ba3de61f1a94cb2990"),
		new("item", "antiquity_felt_riding_pack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "983984c3f17d094c1d80708baa886e383fdd7a92beddd65cedd171ae0b88a9f8"),
		new("item", "antiquity_food_anatolian_preserved_meat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cd6b5aa9ff0ff5d57be67a592c62cfa5fe0b6be178163dd017241f2dba005c6"),
		new("item", "antiquity_food_anatolian_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b013b1e9397bced37b9ec48f42be8b837039574feea2ab29e2f2e0673173269b"),
		new("item", "antiquity_food_cooking_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe9c5eaa7f99a83047b132890a7cdc762cc1f2f2d1cfeaf6adfa0ffbef045016"),
		new("item", "antiquity_food_egyptian_sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "66b8d11f68e70efb195c20f4b71557fda238d39f3208dbf27005ffd2c38a7d3b"),
		new("item", "antiquity_food_etruscan_porridge", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "94e6b51818f373ed65e244f1507b6a26eea841ae98cd2e3a955289c77b1205e7"),
		new("item", "antiquity_food_fermenting_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7752004c18140535cbd271124dfa83517225d0814ef5ace80db0d1e940fa531"),
		new("item", "antiquity_food_fermenting_white_wine_amphora", "antiquity", ["antiquity"], ["item:antiquity_food_finished_white_wine_amphora"], ItemSeederOwnershipPolicy.RequiredRelationship, "23b11133ebf58d875b9d91224f64dfde80270dc0c07d8f23646c3221aab2e7c4"),
		new("item", "antiquity_food_fruit_press", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "26081c41779da945654e854abca18f49f9ddb42591d73ab7b7b812ad127fa42c"),
		new("item", "antiquity_food_hellenic_fruit_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e11e45edc0642ca2d72338f8173eb06ac2886e090c8cea58c46f7e6e937602fa"),
		new("item", "antiquity_food_kushite_porridge", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "38dfe90546b013685b88637b45f39d7148fcf169cd7ddce54b55787d188a757e"),
		new("item", "antiquity_food_punic_preserved_meat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "576af7b4b03978f6af5512a311ec9235db7dc29c2a480a89b934ba05714d1506"),
		new("item", "antiquity_food_punic_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c6a6a71baa0af71136a756c1e86af16c7f9dbae35a5f294a1b567b078ca1130"),
		new("item", "antiquity_food_roman_porridge", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "711fc01fb3d0d7c07bae5a1b34f8caefb8143ddad6ff88ef83d103a193139071"),
		new("item", "antiquity_funeral_offering_tray", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5bbb745a079b7b80c7aa3187f0c0e03ab69635ed22c2f1d77f9dd18cfdc037d"),
		new("item", "antiquity_glass_usekh_collar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4efcb68a4090c958c3ab68b9ec9b65cdcef9295e1b744cfabb04ea046622cc4d"),
		new("item", "antiquity_hellenic_corinthian_bronze_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "588bf8c15e53ded3c0c3cd79f60685024da462c0da86252bc847f57298a251ce"),
		new("item", "antiquity_ink_brush", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a35d5f747fc8f172bb2bfc390134dc13d6ddb809c9884d75cf87deca2d54dc9"),
		new("item", "antiquity_kushite_leather_scale_breastguard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "69b5f2a90978b675eaf377962811074de5d5d711b6d186a36c1035e3b0f1d15a"),
		new("item", "antiquity_linen_bandage_roll", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "32cf53120a56436560998b180a0cf27201d94880197a87c7404129d8e52bff1c"),
		new("item", "antiquity_linen_tending_kit", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7bf6a673785e5a7f31d206404a1cde96050af1ef75291c724bf7b8930390d82b"),
		new("item", "antiquity_liquid_ash_ale_cask", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e01ba622ce8e8ebd02d96cbe3d7b292ca914c2a71ef94fe76dd167ece412c86"),
		new("item", "antiquity_liquid_black_topped_water_jar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7b3eace07a94f357435a97420702af15ccab93d1d3fc96fa5066eaec3d5ac92"),
		new("item", "antiquity_liquid_glass_perfume_pendant", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2349c34ea615eb30eb1e19ea1ef3f86d0b237c4644ad88ce9dd9fb1914d62beb"),
		new("item", "antiquity_liquid_low_wash_basin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb655b7d6fd419658726944d38e9e649cf590df79b1c0757b177f3f01d9632c5"),
		new("item", "antiquity_liquid_painted_punic_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd4cc39621ae10a4a4f5360bc9a35e6a03c53a6c0dfdfa1648de1f1958c20647"),
		new("item", "antiquity_liquid_sauce_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "363b49a8484099e46f7722cfc24672d11dd52f6b6371050c69a55aad27d69e4c"),
		new("item", "antiquity_liquid_simple_clay_drinking_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6e4ef2f74adb505d32899dfd9daaa810c87afb9ef2e7c6a0d1a410dbcdbc0b3"),
		new("item", "antiquity_liquid_soldier_shoulder_canteen", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "25099421eeb821edb98590d445f2fdbd8292d2949421d9cf60163b545c3d37b0"),
	];
}
