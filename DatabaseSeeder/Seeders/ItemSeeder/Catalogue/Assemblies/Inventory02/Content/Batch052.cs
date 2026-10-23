// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch052() =>
	[
		new("item", "medieval_military_tall_rawhide_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a1d6f7713dc849dd1e07f2841570482c25d0ccfc19cc417d340d32c8c911243"),
		new("item", "medieval_military_tempered_bodkin_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "28342716780c551e8c531a2aa7381de6280a8d471c09810ff5f0900919e04a48"),
		new("item", "medieval_military_wicker_arrow_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9710604aa7e2c10bc3ecb5ed03390404615b8dc09ef0b7cb36e5bb97b102543a"),
		new("item", "medieval_military_wootz_war_sword", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcac13b33e83cab64de7d8e9d0a9ef5cf165ec684c3377891bc4f6ce6aa1e6ad"),
		new("item", "medieval_military_worn_mail_byrnie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c036ee469739122d3f02ce25a39f06a1b429e47563ed997c421cfafbab4a3fc"),
		new("item", "medieval_military_worn_small_targe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae2d44713e8f4bb0c4a54e732fa5d362892e04d6a5cf61848c81fdbbb8f26ae9"),
		new("item", "medieval_military_yew_longbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e5ff77ebeb519e2e3baf212f3323fb8ea2464689381ae64851673ab3e36f34a"),
		new("item", "medieval_physician_wool_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3152a273fc87e2ded686fd9792fd243022037dd55487eb3bb3da14daa1cfef92"),
		new("item", "medieval_plain_leather_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "17bf8bc9a950bb432be93527391359e1185eef0e6f802453de23b4ce5ee1217f"),
		new("item", "medieval_religious_hanging_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "79fc2c3079f862516566d306123b8e98b7fffd7c5ad2358c0398d5cae14b5bd1"),
		new("item", "medieval_religious_lustral_water_bucket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2be066d45ad3ffd2fdd010ed542f3a36722a422e938d86d43242236a1b2fe6ff"),
		new("item", "medieval_religious_stone_offering_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "51cc504a756a5fe1c844f1245dcdf7f24ffbab9f1cb7de85b1b8888f3389159f"),
		new("item", "medieval_repair_glass_menders_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbdb15a398e79250626aee0d33ee568205f5887df3ee999b9f0d5513e14d2f2c"),
		new("item", "medieval_rus_fur_hat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "54b851f06d87334e11e4ecd80a9e2c457fba79fa928077728424691ff85f2ea7"),
		new("item", "medieval_shared_bar_keeper_brackets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "92af7cff3b059cd7ee4cb9e95c02ade6bf88648b85c4990e8465d3c7a8ea3dc7"),
		new("item", "medieval_shared_drawbolt_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "41855ba12a6f66575020734c18ab41002b17840e19bbbe72be99b4abb0bc5320"),
		new("item", "medieval_shared_long_warded_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7031016a58f47f36bac4653756976eb3ac7db1b262a6d145313f6381e8fa57fc"),
		new("item", "medieval_shared_portcullis_pawl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d98b4942edc168c2149be59c7248b995309cfbff3dd7c0713c135bc0b878e9b"),
		new("item", "medieval_shared_repaired_warded_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5310b9e36a9941832f6fdab8043308e0147aa2870eed781802a0d87c2b675738"),
		new("item", "medieval_shinto_miko_red_hakama", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4674197081591c6bf3fe617c94e03e5f402b7e520138d97dac7ecf49e5c00a7"),
		new("item", "medieval_shinto_shrine_donation_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1839a24abc0c0695ca32526bfca92cb1594766084a462d9fffb51291e3ad3b02"),
		new("item", "medieval_short_cotton_work_qamis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2fe6777e035daaf4ee165e0c513e7f636f8e6d26019c2379e1e6e6ae184ad16e"),
		new("item", "medieval_short_wool_work_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "312cb46b55854ea53910c7f83873d3187b84adc63b07d4b96387fc246c0304b3"),
		new("item", "medieval_short_work_caftan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fb683d669c1ce8151e3f4007013c834fc813202907b99655bb04a1c43676a9a"),
		new("item", "medieval_short_work_sagion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebb63e87d57b3de1c684058f5597bcf6f23f2eda1591414fec0467104866d033"),
		new("item", "medieval_south_asian_brass_courtyard_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6ea9fb86037ff9ff07ebed088ac80625453df48f6e72666aeef521547202ffc"),
		new("item", "medieval_tableware_ash_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb0ced8837b0a92201dc79076ae955d869a5c84e1ced153294ce5a6a58f2d807"),
		new("item", "medieval_tableware_brass_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "de6624b437a5dd03212b4eed721706e639f133bf4b44893f7246ce2da5f120a0"),
		new("item", "medieval_tableware_bronze_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "56243ad6bd613454602a0e8c1d364b717daa1db73cbb8428ebfdd1dde9e774d5"),
		new("item", "medieval_tableware_earthenware_dinner_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "13935abfbc4314e1955bf51054a1227e84745e78b2de2aa42d1a91c3e1178674"),
		new("item", "medieval_tableware_fine_ceramic_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c081261000a532894bc8da86e8f503017356dee1b52b867ef26b195cac92228e"),
		new("item", "medieval_tableware_glass_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "da099a42f7a6464a853af351b5f7f693e94748571d93f87bee921180224894df"),
		new("item", "medieval_tableware_large_hall_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ca7d7948497829a19a9446f31ef9d91ea2229789467b42f55c39f22b732e3ba"),
		new("item", "medieval_tableware_paired_table_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "175ed02dc1b973b2b11c02044fafd7daa639ec86947e1a0777f60000b818a0cc"),
		new("item", "medieval_tableware_round_wooden_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f90f3db00dd54ae7fa6a485a1a166034cb07c835d877e8b2b2091c04402fac81"),
		new("item", "medieval_tableware_sealed_water_urn", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed757371c8e22caab1bff7ee9d9fc149340df993205f6e33c46fc7eac41e86da"),
		new("item", "medieval_tableware_serving_platter_stoneware", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7a23f8671a0a01f3bf0f8efaa9bdc6afda2db5461b76210ce44459e5035948e"),
		new("item", "medieval_tableware_shared_south_asian_water_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1e87ee0b3d13daa466e07cf67d241ef97ede34471ca55df734f718cf6d1b637"),
		new("item", "medieval_tableware_shared_steppe_and_caravan_camp_drinking_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "987b036eec3fb7432db03d7b809b48f250f58de4e9e60c57b981a8312cf3be97"),
		new("item", "medieval_tableware_shared_steppe_and_caravan_wooden_saddle_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2dab4073629aa3f57939f42239892560c6a7ebe83b99dc4ad3bf9f4f8ccf3975"),
	];
}
