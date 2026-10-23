// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch038() =>
	[
		new("item", "earlymodern_ryukyu_clothing_broad_woven_sash", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9323ab4c68e08a66ddc6836fdf84df4762ee82c682cf781467ab67b466d21288"),
		new("item", "earlymodern_steppe_military_felt_campaign_cloak", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8345ac0017f88c8de30d3f0765ff081874bf89d89c5093c0eaa2447d87311502"),
		new("item", "earlymodern_western_clothing_plain_leather_mules", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87ec0fc3cbcff8d3c73846483f6ef87b2483f4bbcba039b94b36482b4c69ffa9"),
		new("item", "earlymodern_western_clothing_plain_mantua_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "41f2232fa73f11bedb900d28da495a550cbe7490cf759fc7f2d80e04e4092b0c"),
		new("item", "earlymodern_western_military_canvas_gaiters", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66f1cf4658e72739f73dfe844bf6b1f760b69fb4ae34385f7be6ee832de370e2"),
		new("item", "earlymodern_western_military_officer_frock_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a101fd0e8c5888379a91361da89f67c076eae645b7f199e318bc6572db1274f"),
		new("item", "earlymodern_western_naval_officer_tropical_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a89482ffc7b9049258515232debf0c5b18b8cd96e37ff1424d122309ce977166"),
		new("item", "industrial_agriculture_forestry_fishing_0002_field_hoe", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f8da2ecfcbd233cd1cbf93f8533371b36a51993ffd44ba7b06e7e338814b155"),
		new("item", "industrial_agriculture_forestry_fishing_0005_field_spade", "industrial", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0001_field_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "7af6cb882445cf55b8e7d9fe9f01412c105438938e53d03ade53d4b3246ff242"),
		new("item", "industrial_agriculture_forestry_fishing_0011_countertop_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dd843628159432c2d7b976a104764a8cfb4e19addd4e605ac043d5e57e881bd"),
		new("item", "industrial_agriculture_forestry_fishing_0016_countertop_shears", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d85ab22794e339b10d92fee7f9a6d3cbee99723ab3517014ebd00b80469b74c7"),
		new("item", "industrial_cleaning_personal_care_0008_wall_mounted_mop", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c6dfba8a908d33cdfea4803deac48ae1e9d571fc782c3ec104db82440343d2b"),
		new("item", "industrial_clothing_footwear_uniforms_0015_plain_smock", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "f62678a4b66b2592d15351630be92fc3db0848320bd9d53199b9efd92b26b408"),
		new("item", "industrial_clothing_footwear_uniforms_0037_compact_jacket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "330fc3df058d3afcf878903e126248ce045dd8944f73c145e911addbac81e521"),
		new("item", "industrial_construction_utilities_0007_reinforced_beam", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "798464a09809ec6b4d59a163feb261d314ff87870fbdaacc8dcb4e9348032156"),
		new("item", "industrial_construction_utilities_0021_heavy_duty_rubble", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d5626bb5a3807a69129cf1bf9c671bbee37511faff491f4084de0272ed049c0"),
		new("item", "industrial_electrical_power_lighting_0014_folding_switch", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c19f22ec417a33df41c19579b13ca1d9a5ce8c248c9a86288dad65a4ca961b9f"),
		new("item", "industrial_food_drink_packaged_0017_field_pudding", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b29a59d1b08d55f1fcf2781b6d59b92d67ad5f3601775bbf3cc50888f33b830"),
		new("item", "industrial_food_drink_packaged_0028_countertop_cake", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8920fbfe6ce278da04719d972b95667835db54d430a40a6dbe6a16341271c723"),
		new("item", "industrial_food_drink_packaged_0031_countertop_pickle", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b02260a298011b34cde9c4c719fe1aa0cc189b2c8615992ac3193ccb52980fe1"),
		new("item", "industrial_kitchen_appliances_0017_weatherproof_whisk", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "090c527ab41a1692ace7123e0319e3d6f7d692ff5d2abe7bf34dbdb643d0afee"),
		new("item", "industrial_logistics_retail_hospitality_0006_compact_scale", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8523df8f69e0d72528c2e23afb00eb0ff0991444c6e12fa8e6b1c48856185386"),
		new("item", "industrial_logistics_retail_hospitality_0018_heavy_duty_basket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "abf12ccce163633b0758eeb87db3dbc0783a06ef103b692e096b05c680a3b2c6"),
		new("item", "industrial_logistics_retail_hospitality_0020_heavy_duty_sign", "industrial", ["industrial"], ["item:industrial_logistics_retail_hospitality_0001_compact_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "ac77e1be1a017b04fcc32375feb13bc64a76e9a4a8dd032935f3622d6b20e8b9"),
		new("item", "industrial_logistics_retail_hospitality_0030_folding_sign", "industrial", ["industrial"], ["item:industrial_logistics_retail_hospitality_0001_compact_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "dcf5c2b1f0c58f396773a57f16efea088938384f855f59217b60856ef5e23da3"),
		new("item", "industrial_logistics_retail_hospitality_0031_portable_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b5414ba0a52d3a40089bd89473f066a6bf9643cd557e6e1bb502d1ad331eb50"),
		new("item", "industrial_medical_mobility_0007_service_forceps", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "12196c0b29f6a24ebe42b7dd8a3ac586d769eeb39c8dd4db2585ae22986f6bd2"),
		new("item", "industrial_medical_mobility_0014_precision_crutch", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f52f58561df21c3d78d17beb166597fef4cf2255ccc533e40d1878aec5045d42"),
		new("item", "industrial_office_school_mail_payment_0003_travel_tray", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d53d938a0061b17dd358ee485f96c3dd7a6c66b689607630cdd5870de8fc89b2"),
		new("item", "industrial_office_school_mail_payment_0016_utility_ruler", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "90ec6c5af81d37204df7ff83af824b10b11b2276ff705e7ed494e0dceb21f152"),
		new("item", "industrial_office_school_mail_payment_0026_plain_stamp", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "11ede99dc44e88d16f68f40779b20729719ec3f1f0a2bbd70fa66e05fd89f105"),
		new("item", "industrial_ppe_ballistic_0020_countertop_coat", "industrial", ["industrial"], ["item:industrial_ppe_ballistic_0001_field_liner"], ItemSeederOwnershipPolicy.RequiredRelationship, "3e8e45a92265853dbcc76162724e6fdf9bb30e7489da08563b90d9596d56f329"),
		new("item", "industrial_ppe_ballistic_0026_wall_mounted_gaiters", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1f2d1786afb62814827216b92b285d0f70d66575b3c4456dc61ee82ffd53495"),
		new("item", "industrial_printed_media_signage_0003_economy_ticket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "09d1871ab38f5fa3cf43797f59499411449f12e37a986219dd16c0d3e2296949"),
		new("item", "industrial_raw_materials_chemicals_waste_0011_field_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "417076975fc8e2507dcc99a094ffa06203199ba7a0fd0b008147d6f306220b38"),
		new("item", "industrial_science_education_0014_field_flask", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "472e00cebb9595a923ebf2e17543ecfbce534e5a3707ccae3ae5aa3e3a69d979"),
		new("item", "industrial_sports_recreation_music_0008_heavy_duty_bat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2badd0e3b38086af2f135711f9f30bda87bae0cfff5c5a75935284387d58834c"),
		new("item", "industrial_sports_recreation_music_0010_heavy_duty_club", "industrial", ["industrial"], ["item:industrial_sports_recreation_music_0001_compact_drum"], ItemSeederOwnershipPolicy.RequiredRelationship, "da7982217dae2461badcd44a9508bbd59454ed9deab3bcc49fd6666cabd26c50"),
		new("item", "industrial_telecom_media_photography_0010_compact_cable", "industrial", ["industrial"], ["item:industrial_telecom_media_photography_0001_compact_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "458a79ccdc3a0692087c34708f80d21ce1d5e1bbfcec4fe0dc4116425e1afbc8"),
		new("item", "industrial_tools_repair_machinery_0022_stackable_drill", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bcd5133cfe3631df051f2be159d7525c6cec41e51103efad899d7438208a671"),
	];
}
