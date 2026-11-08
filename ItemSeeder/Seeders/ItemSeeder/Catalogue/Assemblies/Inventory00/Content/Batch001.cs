// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Ink_Brush", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb5b45b78a35fe8afcf49e043bb4f07123e4a79d1d3be201aeba0f0c02c26bfd"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_chocolate_puff", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "285d8337be9cc394df50547625ea3d96bd7621214ace7a32007ae0ddc20d50cb"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_naval_duff_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2e60d743180a2c23a07075eecd5b1d87048efba793f2959fde08087a299115b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_portable_soup_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a1736d2291887028be864ccd3204f8b9f85a76ca9179407456e4e95e823b962"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_caribbean_plantation_sugar_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1c7006bad4f027959668f9781866c4e5770594027805bb63a4c61731ab82a82"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_glazed_root_entremet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dd6f6402949c915231883225afad8fb5b0087589296702f2b56eca1160b1907"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_puff_pastry_game_pie", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e59d405be16ca5807920c4281bd7d8f00692e097679cc3d2b8bf5e9ee562991b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_ramadan_gullac", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1f3c274a4e44ed1effbc41812c9ac09d38fa26ac2d69b4e5f7affe2fbc06bb2"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_tomato_lamb_dolma", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0356be374a215041184cd124fc22f38dd86bf01d6cb5dfc8cec7c58ca4bb6649"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_punch_house_salted_meat_pie", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8295b0cc9835b312af9656df8adf71ca3fbf5024e72e5d9713ddf333e973bf86"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_imperial_manhan_platter", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f2eb7a57065250cb0066a9f16d096c3950044c4e39d796a8da758f1517abfbc"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_sweet_potato_famine_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c018ad48d201dc0b374d7c17bf82232383a6d840edac1695374852416509fff"),
		new("component", "PreparedFood_Catalogue_medieval_food_apricot_judhab_pudding", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "22f736b6f11a30eb47d685f941eec1f511c582e24a8be45aeee013fa240f702e"),
		new("component", "PreparedFood_Catalogue_medieval_food_byzantine_cheese_honey_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "89b4e97e29ca7257dd6a62b4b795a724af58c33cd46677b5e80562ba6b5a9e39"),
		new("component", "PreparedFood_Catalogue_medieval_food_chaucerian_market_hot_pie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4766e328309e0485b9404f5ac82f1d4bd2f2da3fccb8138bf5759ec77423d9d6"),
		new("component", "PreparedFood_Catalogue_medieval_food_court_couscous_meat_pyramid", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e0b7c783038159a68c8997e0b216e27885fffa91143a143b90e5ec6382791b0"),
		new("component", "PreparedFood_Catalogue_medieval_food_ethiopian_monastic_pulse_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a23faf57a6e8d2908c3c0945527a32aa482589b277d4d171c02736b33f98a73"),
		new("component", "PreparedFood_Catalogue_medieval_food_green_herb_tansy", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbb734eb32a949e594bedf33f05b263c19b66d422cd4e0a28496a77df3cde3c6"),
		new("component", "PreparedFood_Catalogue_medieval_food_korean_honey_yakgwa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfadbea4b80486c56b2893e7037887c43de9ae0b16e88cd30159eecff18ae7ac"),
		new("component", "PreparedFood_Catalogue_medieval_food_madira_fermented_milk_meat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2bc80f97f201cea3d7efe0f68c4e8d2ec447f69b3e36f93b3f44f1db20fa5e5"),
		new("component", "PreparedFood_Catalogue_medieval_food_qatayif_almond_pockets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e975c914afda9a594eb44a79d7b751de88fa3c68ba4ccc92e65de9e06520d022"),
		new("component", "PreparedFood_Catalogue_medieval_food_sultanate_spiced_rice_pilaf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "41485c49e44fe1d37f507e14152109313378e3ad51417a2567bfcc474a72310a"),
		new("component", "PreparedFood_Catalogue_medieval_food_swahili_date_rice_sweet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a663e6ccd901160d047e4055df07a58f5f631f5573c58b1cba5c20d1b66f8e58"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_aubergine_with_onions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e79ec45d4f980284c4efa105a1a169861c7c35ae009136965c4161d1ea7ecb3"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_baked_berry_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f8e80632102946b8a49417a77252f90533e5c02a35ea82c18ff2a7cddb66848"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_baked_egg_dish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f93b4bc67a96c28cb8e6791c9d74eafbe3be442657c516d3927cfd9aee3b3ea6"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_carrot_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f36341eb635876124187c102410f4cb33c4c8db93ce7ccc854a7bb9558e4a13c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_stuffed_cabbage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc59f9bdde34fdda0837c819ae72621dd08bf20b1e695c89dda6bbf9d0c4a18c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beef_root_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e665f60b7f8c1bb5f2cbd835bc8ff82d0f94ac6a19ade2990ccf8e7a3c6d5e9"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_blood_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4132b8fb1b57b587769ade0bf841c2398f6116cc100eaee5fdecb69b4f25d5f7"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_garden_vegetables", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49fd90410afca59b5e05b8273bf589b318ab912cc046d56e68b14974f2ba21ac"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_hare", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4811b625aa6291c3f78e1e4424750ecebfe147f0045fe278dfd98fd60bafcb6"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_then_baked_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cb9ad3070166ad05352beb40f790449ba5cbc7c6e09eb3eac3a5b5ec988f3fb"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_wheat_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f249d835928c20d5989f7e5d85cf3a35917dd64e1f7e9c1832bfb67af025319"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bread_and_leek_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec548ace7b286da48d8a37ea956ab3990f4aaf3f15abc7d97244033f3adb30dd"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bread_egg_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "583c5b96df2fcd6e0167d7d9bff0d84afe93b3455c218535b88471009d022c77"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_browned_crumb_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "02ae9204ec0453d5873fd1881737e0bf0ac5a6fb80ed538e92a04b06d43d3d83"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_burnt_smokehouse_meat_ends", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "99447fd17f85382f14ac2af0953911260ee0f8f9bff99d971cb84cee748ef584"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cabbage_and_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f338ff41a51e7812830d14c1c0a593563f5497e8a4fc6323b73fac4110cfe86c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_carrot_and_lentil_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "889b006854a05583be6c4fcc23fe94d21469e4474daf771a3d54f25b4265cb53"),
	];
}
