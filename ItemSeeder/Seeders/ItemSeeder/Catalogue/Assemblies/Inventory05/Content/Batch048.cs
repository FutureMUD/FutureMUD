// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch048() =>
	[
		new("item", "medieval_food_smoked_fish_ration", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "04735d875715958d9e43d9805926e702af669e69f4f8433cee3c0a393d677a6d"),
		new("item", "medieval_food_west_african_honey_bean_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4675ffe912542f988468b1e4ba9e5baeffde2df39ee2fffe6ff419db528cfe9f"),
		new("item", "medieval_furniture_carved_lectern_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "713122005e6db8abf9b2978a2d53a090f43f5c6f45e3775524a86bf83ed18c3a"),
		new("item", "medieval_furniture_carved_throne", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "87175801d6a9f52ddc5ef7f333fd5c3569ccf4befdaa29ca92e7599b9e9153d7"),
		new("item", "medieval_furniture_east_asian_platform_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fbb7181602ca87f1fb52c5fb03dc26253066bb09a73c75c654c63824ffab3a19"),
		new("item", "medieval_furniture_games_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a17d212c777327d590318a24d224481b4f4285a745daebd271595ff20997081"),
		new("item", "medieval_furniture_islamicate_reading_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8018881b9358ec227ed0c5a0671fd0385aa7df87e5df6682994e5d3fa3e3bc2"),
		new("item", "medieval_furniture_low_box_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7fc71378e2d1f34eec71a29223dd159eafac35d4b8e100211684a188aa70ec3"),
		new("item", "medieval_furniture_narrow_cot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e83c9abd81a191c9f9dbefcbc81403992b7be47c375a0009a6d9fd21430f1b2e"),
		new("item", "medieval_furniture_northern_box_bed_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1ba316539fcd98e6d53a863a31d02a5bc73d9c26997d00a9cd6da7d19ef1502"),
		new("item", "medieval_furniture_northern_wool_sleeping_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3f19835d1fd6ee8f550d3ce0f4744023d8ded0729388000da8faa391b763650"),
		new("item", "medieval_furniture_portable_cot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9be5aee4e0461416e10bda26019d51216171b78a4dbf82f093b266d52e6f022"),
		new("item", "medieval_furniture_round_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f56c2370d15b412cd8e7b9ea0cc419fdf6a524f7aacc4503fae303769e33abcd"),
		new("item", "medieval_furniture_settle_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a450d82d8900109740cd85400b20663a45c64d144df2b8244e0fc91245d3df2"),
		new("item", "medieval_furniture_south_asian_carved_swing_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "23d3b519131b77919f4527e150b2313f5c4580ba67fcdaaddd6a92c8f5b4d189"),
		new("item", "medieval_furniture_steppe_collapsible_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f829c88761f3459062f17dfc450d21153af3e6a6ff686827eb48a4c16a0ef32f"),
		new("item", "medieval_furniture_thick_sleeping_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce5add7e9fc7a596d93052101313eaff9783c7ec70294bc27fa9089385f9e894"),
		new("item", "medieval_furniture_western_trestle_bench_pair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "30e2d3c5eaa30a90641336828a5427fe6fbfa4abc6e25c745a38b4b800eda8b1"),
		new("item", "medieval_heat_iron_ember_pan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "24e2daf3bb28451680fa4714881e443a4506f93aa7a891592bbe5bce21a54045"),
		new("item", "medieval_hemp_sandals", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "74d3e6122a39f190bee5e52d9eeb79cce2a7ee8859345586a3f0dfd5270ed777"),
		new("item", "medieval_hindu_bronze_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "73b885cb4f1075e88eff85f7ea4c6b16290a6a4f0929bfb2c16db9c2961bb593"),
		new("item", "medieval_hindu_household_shrine_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3fbdd9e2ca9096782a77c9cbab0d160c0e6efe0700eca50216e8db365ac02382"),
		new("item", "medieval_industry_stock_cord_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a31276a1739634c00cb1fcf42e4d2d825b478896eca8fbcb03b01cb184979fc1"),
		new("item", "medieval_industry_stock_glass_bead_blanks", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e28953304b175fb89d468ca5b9274d191eebb5edb3800f7552c9aa049193c7e"),
		new("item", "medieval_industry_stock_leather_thong_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc00848cb34234dc4edb6fc5761ff67b34e1cc608460fae5171a0d29da6aebed"),
		new("item", "medieval_islamic_quran_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b4c7cda48ca11e5d6e6212397abaef01b8f8e70fd7b44e725a83954ec07133a"),
		new("item", "medieval_islamicate_brass_scrollwork_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2e57027287f9c9ec2d3f060c0e55960e1ac61dfbfc22a450406b929522579f3"),
		new("item", "medieval_japanese_yumaki_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c82a05718509cbff0f4ea1d1dc0985a9bd036650f3afe179724730414a08ad1e"),
		new("item", "medieval_jewellery_badge_motif_court_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "08c623df5eab55cce844b5f013aabaad0c52b2084c85cec139ef9631bef74c01"),
		new("item", "medieval_jewellery_badge_shape_brass_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2022ebd59c198a704e2de0f38b4af9f66d5d1f799325e23529ab2ed024ed9fe"),
		new("item", "medieval_jewellery_brooch_garnet_coloured_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e38ff6915fb9d22cb06aee435e2ecdb8673332f7b2bf4127445fc955717ac998"),
		new("item", "medieval_jewellery_brooch_shape_silver_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "369009951b069e56088474022c914f102735a684bd6ea019c90f7c3f43caa1d8"),
		new("item", "medieval_jewellery_brooch_silver_annular_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "001c7a805a1117cb4b672932250c3ff66c93ecd774ab38b43c6f43cbc57341ff"),
		new("item", "medieval_jewellery_brooch_small_pewter_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca1f1fcfacf2d012574898dbf0204961d32c8f97fe4adc9cf179bd380832aae8"),
		new("item", "medieval_jewellery_ear_pair_of_bronze_hoop_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c07033c559f548a22decd1df51366cb3b5067045ae2e0e3750a5caa4bbddb6a"),
		new("item", "medieval_jewellery_ear_pair_of_coral_drop_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "739a6556cf77c44e16287556ab80e835dd1466c25863c9465c1c2b6ec434d376"),
		new("item", "medieval_jewellery_ear_pair_of_silver_wire_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "72f2a479659e4bc18dd45cf11cb00686b71e5c4d55053ef9783ea345e96bfc38"),
		new("item", "medieval_jewellery_ear_single_bronze_earring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf94143dafab76a1bca1936e57412d2edb9b5617649b6e912fac84aeaab83bdc"),
		new("item", "medieval_jewellery_garland_wilted_daisy_head_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_daisy_head_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, "362dee97cf0cf21fcd018c90b60dfc1213985bf9404c154b609bdac6905c8b3f"),
		new("item", "medieval_jewellery_garland_wilted_jasmine_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_jasmine_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, "12798109f57731cf93989ec6a764625e0a7792a48c03bb3ec601f0139d790633"),
	];
}
