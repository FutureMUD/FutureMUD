// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch068() =>
	[
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Brine Spring Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "71d30dbb5beab28607d8cc66b92357b653be69dd4d937eea5883f8e7d04035c0"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Rock Salt Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07374a2208071c5de08a4a8ac30c1637cefab627ee46fce579cb40a988d91119"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Sulfur Deposit Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "37d85666e00222bb21a1c257427dfa8708a08d5f1cfa86baf74032b8e7771eb0"),
		new("tag", "Functions / Musical Instruments", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a29cd6333e07566b62d85809d4f288cdd4c50277d999b8d4abf84c155279c81e"),
		new("tag", "Functions / Tools / Metalworking Tools / Bellows", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c4c16495c3ca3369599c0e0433b93be87271bf4b4fa00bf0a1e3eecad4450b2"),
		new("tag", "Functions / Tools / Pottery Tools", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7f46ba4e7f89ba9a14ab80cfba201f5571e0a1cfa495a9311d86bdb740856c1"),
		new("tag", "Market / Construction Materials", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b437764cae6405e72d71386b738a0f5cfc64a54e47ed8a697f54c60673190fcb"),
		new("tag", "Market / Transportation / Horse Tack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "24fc9c426ca2c96edc641bb77b53992d56c28374ad72c8d28315721c6f5bba4c"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Dried Fish Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3dbd0a80116692ae63cfb6c19ab7ab8ac4b85d874d2ceb88c0b3cbcce2c72802"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d052713de07bb66172604061bd2b4dd48fa25f8652e6577bbad8591ecffedaa"),
		new("tag", "Materials / Natural Materials / Food / Vegetable Prep Crop", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4511ddea83f62e291db43c9c19dacc939757c490442ac9b300da8ac4ce6f4ea0"),
		new("vehicle", "vehicle_antiquity_light_war_chariot", "vehicles", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e60f51d7bdea0ddd6aea737660f0dcf73e0b89a6889ae76e57d383e5ae876317"),
		new("vehicle", "vehicle_antiquity_reed_coracle", "vehicles", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "72961782e6037317d171ff9af84ccc1a4a3cdcd7bbb8e058e83d3b4caa98e2c4"),
		new("vehicle", "vehicle_revolution_farm_tractor_wagon", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "34176bf2bcd1be792575b85c05a3d83715b5cbbca628f4ca0be863ece1d4bfb6"),
		new("vehicle", "vehicle_revolution_fire_engine", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f30b71a54fcfcc20d64d00cb2d6667dc1095549ad40a30048c8b03145f5f554"),
		new("vehicle", "vehicle_revolution_fishing_dory", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0f5126be5015d98ac19ef9807549b65c9fee0c5c7a039a960deaf35f96f4971"),
		new("vehicle", "vehicle_revolution_harbour_launch", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "17e0c2a8a1f33e14f89588bb53d604108afa5c4e7a7b93aade44eef94b3d1653"),
		new("vehicle", "vehicle_revolution_mine_tram_wagon", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "58e6c4bc9585621d867b38fb50d2c78bfebb22871590c79ac95db6c552036a6d"),
		new("vehicle", "vehicle_revolution_rail_open_goods_wagon", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "90875360d1f2656fcf08a9312d2cca5f980e90a81cfb4808b52cfcb60c01820a"),
	];
}
