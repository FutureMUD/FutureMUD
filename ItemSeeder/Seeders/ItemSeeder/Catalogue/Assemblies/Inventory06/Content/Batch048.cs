// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch048() =>
	[
		new("item", "medieval_food_dosa_fermenting_batter_crock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bacce42fecd7d41cd3913a29ea2d500fe65d7ee9da51af27576f1be0060eac80"),
		new("item", "medieval_food_galantine_sauce_base", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5f77542e53ddd9d5f23ad3636ed951d820998e8b00ab9f265ac71cc8ba88e13"),
		new("item", "medieval_food_japanese_sweet_bean_manju", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "556587d17e61b9bc5c5269750d935e972522bb049724e52094386867ba40f67e"),
		new("item", "medieval_food_lenten_fish_coffin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "106a4dce43a33e198b6fae7f2e30aee0e194a8e31c896388dee5b3bae83dd59c"),
		new("item", "medieval_food_smoked_meat_ration", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb304bae78209cf06f3fa0b39849c390dbca6d3c1c4c1353b9e584bc6799467a"),
		new("item", "medieval_food_stockfish_almond_pie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "986f359e400dcd4fc770dc0e83211e605a3a27c1a08308979a7b578df4260858"),
		new("item", "medieval_food_swahili_date_rice_sweet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "63dcf313a0e0e018aa6522e9f8dfe6ef19255adea14620768bfa4d41fc702b84"),
		new("item", "medieval_food_sweet_fruit_tansy", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9229d876e1bf739e3a6f27377eb2f86f834ff566c910c1813ddcdbcd2e66b67e"),
		new("item", "medieval_food_yuan_knife_cut_noodle_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2035c4af46471283c73d987d24c94d3aa29bbb82377a2c918e6befff66c0370"),
		new("item", "medieval_furniture_account_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ced70774ee19b93d6f7e69b140c031774427c1a9cd28a2effe1045e467dea9a3"),
		new("item", "medieval_furniture_bedside_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "12bc9c969f4aa0ea3dfb8929a0f2292b7cce57a124df036474c1267ea423d1b8"),
		new("item", "medieval_furniture_guardroom_cot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e258923fa9fe0c1e7672fffc9c65271e43a90d2e8b20cbe023c09c4c818abe9d"),
		new("item", "medieval_furniture_islamicate_merchant_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d616fa229a3f347feb3e2b7edbabc5a34641cbf8b5631aab5988516cff03d863"),
		new("item", "medieval_furniture_ladderback_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2622c3420ec3384a367dbe4a4b301da14d396e882f0f82a4b9f259baee3f2083"),
		new("item", "medieval_furniture_leather_travel_bedroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fffeef9bb7b84fe657c29708abd19f2ba152ca56a2597433b43baaf05f198d23"),
		new("item", "medieval_furniture_long_hall_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed72060842e159d9b17f6b49c1aa079bff163fe4c841ba8407bf86c13e8df992"),
		new("item", "medieval_furniture_mediterranean_bench_couch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d27b42ad61604286d9146a0572b8fae8f9375247dde0c289cce9f6fb49cd7ecb"),
		new("item", "medieval_furniture_mediterranean_daybed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "41910360b36cce9f919fd9683d09604c70fa1ad357bcd2d876d616c587d82059"),
		new("item", "medieval_furniture_peddlers_display_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8cf9e52542ff4658e2ad145a6b9aa8e07fe180aeeca55fbb20569b970086a429"),
		new("item", "medieval_furniture_scribes_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "124238b9d73cdfd2c4e61fe9d47ba6ad66aa331eca06899030143ac096953259"),
		new("item", "medieval_glass_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "103a9f9dbd186074847adfac2768be2ab52d0da16ad5898ce4e7bacd4838d376"),
		new("item", "medieval_heat_clay_charcoal_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "70147d004c32202ba4d88053955b6f4c4bd942129cc19a1dfccd727bdd200f8c"),
		new("item", "medieval_heat_small_charcoal_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d236074668ba1e82cefafe223e7a6673583bbbace585f87c15b699594880584c"),
		new("item", "medieval_heat_terracotta_fire_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f44fd5c65a818100a22431207d434bfc18de6bf9ee9df1fd2963080ad6e567cb"),
		new("item", "medieval_hindu_kaupina", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c73b6cc004d28f5e603e0013c8b4b09c612b6ab439b907ff9dd510a1a55f2963"),
		new("item", "medieval_hindu_ochre_shoulder_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d868553ca196a3836a7d6ff2d0b5eebf32b39dd9ad455e4aee9d82cebc00c91"),
		new("item", "medieval_hindu_white_uttariya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "65231dea3b06edfc6df05bbd9bb1f5b34af6d7a1a32df304dfaa2319a7948c77"),
		new("item", "medieval_holy_roman_empire_german_iron_fitted_strong_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "713639cfcac8d3e32cf757255cfb155f04dc3b006e0aa4cf45170f3628fd8a68"),
		new("item", "medieval_iberian_manto", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "00ca4e9b0e70544c27f3f8f02ec269998027ea301fc53e4875ba71d5ca590d16"),
		new("item", "medieval_industry_stock_bronze_bar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "443472c20b34d78b31da66354f64f18a98fc94cd35147ec1c323f43897f24635"),
		new("item", "medieval_industry_stock_canvas_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c51b0335222d88d43d9b1e3bb8c24140f6d9b7e64ff3c0e936f96626066d6a8"),
		new("item", "medieval_industry_stock_glue_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d17ddf31872eda7a9db7f17210f2ffe6d51d9e5671672909f3bc0e981fa93105"),
		new("item", "medieval_islamic_brass_hanging_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc6d8e1fef2537d070918615f88981f5018a41411fae212b0c9c383a50bbe6af"),
		new("item", "medieval_islamic_water_storage_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b9ec7ab1a9a87800a0346570af6c8c08fcf3ebb75e5307174c3c5db26c3ed59"),
		new("item", "medieval_japanese_hitatare", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bd4e804728d92d75ee1cadf4caddd757e810aedc2ee99378e02b4b2437854df"),
		new("item", "medieval_jewellery_brooch_horn_cloak_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff8a1f8baf1d4e98117cf989253bd55810f237e4aabfdb755be502d86417385c"),
		new("item", "medieval_jewellery_brooch_paired_silver_brooch_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "02d1698a1121144bdf5cbf483c80776604077cbb6ccf1d87b83dcb2da7bacee8"),
		new("item", "medieval_jewellery_common_knotted_wool_wristband", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1f432333c2c0015ab1c55f0e27472bd99c0c4714e6918a2828adb9354842a4d"),
		new("item", "medieval_jewellery_ear_pair_of_mother_of_pearl_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b604dd69f1c6198660f4f5d701c4177155ff3706acd5089a833dfcf4614922c"),
		new("item", "medieval_jewellery_ear_tiny_turquoise_nose_stud", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c56ba667612e5662cc51663bbe8f006687ec60108e8c98a308f9af885c742d89"),
	];
}
