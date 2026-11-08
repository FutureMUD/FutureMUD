// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch039() =>
	[
		new("item", "earlymodern_religious_tibetan_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5116bd70cb765d6913260d73aee74676a4c0e0a26f3dbb995a01fabdfb217a66"),
		new("item", "earlymodern_religious_zoroastrian_kusti", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bee594ec1a4bcc97ec6a35f00f80f9a2bc370b8a4b3d1d3f8aa1406782eb9fb"),
		new("item", "earlymodern_russian_clothing_long_linen_rubakha", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9218e678f4ca5289ead00c41d1aae137ee9a6936dbfcb97908f2f5f01fb25b92"),
		new("item", "earlymodern_science_herbarium_folder", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcaf38446fd073ec4b85197b1f17f7e0c97d161788f6fe57ad756d529c10696f"),
		new("item", "earlymodern_spanishcolonial_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "21202bf33ff4e8d2c037758fa338b7501ee5753c34db3a4a37e06a878af87b3a"),
		new("item", "earlymodern_steppe_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df9f74e5de53e89c0bb57f99293e5aeed0f0e399ee12c719d986214e853e44fa"),
		new("item", "earlymodern_swahili_military_coastal_guard_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c260ae5068fa8457d8965f72a9d2e9397c143c8286cf32fdc8ed3c4c37eecabf"),
		new("item", "earlymodern_westcentralafrica_clothing_short_tradecloth_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "069db7c2d3b923532f8c2b4aa961607055579649ad3f642ffcc7ab4280e4b561"),
		new("item", "earlymodern_western_clothing_buckled_leather_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1437c3b8079d846da1c3886c0e33899bb35649c5be6e5c25b5c86c93a219fb6f"),
		new("item", "earlymodern_western_clothing_linen_shoulder_kerchief", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac4b990304e5a7077138691819551d0eb0cf059f0b111262e04d24f8a47b1ce1"),
		new("item", "earlymodern_western_clothing_long_broadcloth_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0c8a9470dedfd3c3435030958ef5905e63dc25a6679ad31951e2ade0756282e"),
		new("item", "earlymodern_western_military_mitre_grenadier_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c5009f33943b54a6483959bf0925a187c037406c706a1b82259d8a55cbf8913"),
		new("item", "earlymodern_western_naval_heavy_watch_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b29bc3e4f153afc6279d8eb71645cc60385d96a0a330ee18c38fbf0e7ce16656"),
		new("item", "earlymodern_writing_deed_box", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "062070f405230cbbfae43b3ba6234ebd737d60111f764c2d5adcccf0cf4f395f"),
		new("item", "earlymodern_writing_gazette", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "510fb814b971393db1c2c45a19dab044120db594e7491867a24ea42f64bcf1ff"),
		new("item", "industrial_clothing_footwear_uniforms_0004_plain_trousers", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "aef7f172864ee76676c5cfda4c8317ea1010e7496a505f1715e4d002c64dbcf0"),
		new("item", "industrial_clothing_footwear_uniforms_0007_plain_jacket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a3a9c502fc667f59c16183dab43a9a28f36b5c1a65d377a1273a2b352f7a6eb"),
		new("item", "industrial_clothing_footwear_uniforms_0021_reinforced_dress", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "159af1e8dfe9473b7f0fa0b3aa8762937d3bee9fee4e90c6dbd2b95a6495e704"),
		new("item", "industrial_clothing_footwear_uniforms_0023_reinforced_hat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "30e5f1d3a3a068df6c6d360da572acf0ef3f8ab83f44d7ed04e39c2e1ba3abe4"),
		new("item", "industrial_clothing_footwear_uniforms_0052_heavy_duty_jacket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "58951c7af5d6ae0691ac70418db61ac8a80d43f169d23c3c4a7e355f986d0e5f"),
		new("item", "industrial_construction_utilities_0012_compact_brick", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "21e3fe9a4be79a549dc34574808ce40f362489a57ece2e570bab9b3870334bdd"),
		new("item", "industrial_electrical_power_lighting_0004_heavy_duty_switch", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "82a8d0c99f11f3e972c9698db3d42d0afe77ec9e1b128d55d44598584465208c"),
		new("item", "industrial_electrical_power_lighting_0016_folding_lantern", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfba19e926e1e4484a6b318fff5beef282139469bccfaccdccd3bc991b403cee"),
		new("item", "industrial_emergency_rescue_civic_0002_lined_hose", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "375e0a73395a95501ae6fecaf2a6afce76d644758366f87c64241eeed7988ca3"),
		new("item", "industrial_food_drink_packaged_0016_field_cake", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f250b99327e5c38f33a028710e4d0e0f4450da4df00e7f480890faa39558e4a"),
		new("item", "industrial_household_furniture_storage_0001_lined_bedstead", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "06827f8dccbdbc81a9b855d6fad28dfc628a19b92180fd1b39bb292d4ee442ab"),
		new("item", "industrial_household_furniture_storage_0017_ventilated_chest", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "83c4baf09dcf73331f67c8166edf25ccb38497ac9158fb23adc6a55ff2cbcc00"),
		new("item", "industrial_household_furniture_storage_0019_stackable_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "306146755caa1a44aaa64cc0e7f9445fa3279644781f4b87870685ea8b6380ea"),
		new("item", "industrial_household_furniture_storage_0020_stackable_chair", "industrial", ["industrial"], ["item:industrial_household_furniture_storage_0001_lined_bedstead"], ItemSeederOwnershipPolicy.RequiredRelationship, "15b60431df8dd6c1bca555c907943a4513f8617956bc799858503a5c5418bbbc"),
		new("item", "industrial_household_furniture_storage_0025_stackable_bedstead", "industrial", ["industrial"], ["item:industrial_household_furniture_storage_0001_lined_bedstead"], ItemSeederOwnershipPolicy.RequiredRelationship, "9ec1ca5c25faef99f7a75665b4e2e49d1f4ad44b5344286e22ceb7af62c3d58b"),
		new("item", "industrial_kitchen_appliances_0004_institutional_grater", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "92791908190282e2b7554f609fc1e0ad3211c3a941ad682498bc3b6f00f45bf4"),
		new("item", "industrial_kitchen_appliances_0020_weatherproof_toaster", "industrial", ["industrial"], ["item:industrial_kitchen_appliances_0001_institutional_kettle"], ItemSeederOwnershipPolicy.RequiredRelationship, "eac48b0deb543f880a7f9c776d7cbf4a2693ac6cd8f7b3a45e7753854d101511"),
		new("item", "industrial_logistics_retail_hospitality_0008_compact_basket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c99b353b5b1992809b96154501edd92788308810be1a883e6fa725bfd98ef9c"),
		new("item", "industrial_office_school_mail_payment_0007_travel_satchel", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "30b6221f410c051ba884a244fa7b47a64463deff2830b316563b67a764568a0e"),
		new("item", "industrial_office_school_mail_payment_0025_plain_folder", "industrial", ["industrial"], ["item:industrial_office_school_mail_payment_0001_travel_folder"], ItemSeederOwnershipPolicy.RequiredRelationship, "50be498b1166fc23a5dd5ffc2e49d632b5b0f638e139f921c3f8fbc1ca1e2b5b"),
		new("item", "industrial_ppe_ballistic_0027_wall_mounted_mask", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "56d8dfed102658bd6210272fbc5110759661b3cd4e3ea5b7e6c67a3bbdf1f70d"),
		new("item", "industrial_raw_materials_chemicals_waste_0021_countertop_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3098748e43d4622b23f31a2fbab97f20acda8ffd0e6ec6fe327cb2b7cde34e91"),
		new("item", "industrial_science_education_0011_field_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2c56c9ab38a0a4e5431216881bb354573b613f3ce7a3efd0e086b15d5a1658d"),
		new("item", "industrial_science_education_0024_countertop_flask", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "807cb07a8b30eef1f6690054f4b30e3a6a05e6f5256b9d67f7bfaec488180ae6"),
		new("item", "industrial_telecom_media_photography_0020_heavy_duty_cable", "industrial", ["industrial"], ["item:industrial_telecom_media_photography_0001_compact_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "b6fc0dbbafc005d3a4332a0da450d7bdae56be163b18e2f83301be06ed143a98"),
	];
}
