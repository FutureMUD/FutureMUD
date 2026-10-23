// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "industrial_clothing_footwear_uniforms_0067_folding_jacket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f20112ccccb62a1cb2932483c95e95b01f28968494c49cc064da956db1ac81b"),
		new("item", "industrial_construction_utilities_0006_reinforced_fitting", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8096714b2cd072000248e65642059386cb23ca14c218a6144c25ae92e0e35067"),
		new("item", "industrial_construction_utilities_0018_compact_panel", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "55b5c92f06ed71af700568babc5e5f3281d19288156e14765965b90fcbced43f"),
		new("item", "industrial_electrical_power_lighting_0030_portable_transformer", "industrial", ["industrial"], ["item:industrial_electrical_power_lighting_0001_heavy_duty_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "c0207b89af251a0877d4cff16273a031024a953a28330b9a44f3da1122e508ef"),
		new("item", "industrial_emergency_rescue_civic_0014_ventilated_barrier", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "32a4c7d83cb13598a79e4ad7c29090c535850b4b12d4b600722e7e7688c2c9b9"),
		new("item", "industrial_food_drink_packaged_0004_stackable_cake", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2e5efe1734883b29cc8aaf4d50ee3682aa8f003c251596ac23ce898d61e7af3"),
		new("item", "industrial_food_drink_packaged_0018_field_sausage", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "040e560961cb3855f1f6d5ce567d816b08d9700816f219e44ff87293ca52eff2"),
		new("item", "industrial_food_drink_packaged_0020_field_sweet", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "0bc199ca6225d1549a8f3fc75756792e8a73a3ade2a909a290ef820dbb1948ca"),
		new("item", "industrial_food_drink_packaged_0037_wall_mounted_ration", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f1620e5a895d1491c511fe3c49feb22b73170bce42d716ee85d703f1ac1b540"),
		new("item", "industrial_household_furniture_storage_0006_lined_bench", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4faaa738249b0f895ddaa831ff2610052dc174603d2eeae775431879658d5d92"),
		new("item", "industrial_household_furniture_storage_0015_ventilated_screen", "industrial", ["industrial"], ["item:industrial_household_furniture_storage_0001_lined_bedstead"], ItemSeederOwnershipPolicy.RequiredRelationship, "12872242c07d458ce2366baf61ea9c6198e431ad89ff307bbbfdd3a6af3a79cc"),
		new("item", "industrial_household_furniture_storage_0021_stackable_stool", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a6be0abeb652ea1d22e8611bf79335316877043c023318175efd48e5a577a6c"),
		new("item", "industrial_household_furniture_storage_0024_stackable_shelf", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f2a776e94995f08e7514d34d12831660f13bd76f5719802fb7ef426e767ad3b"),
		new("item", "industrial_kitchen_appliances_0001_institutional_kettle", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed0f188ac8f9c41abbf352497260ac7557f602d9067f564efb4a097db4d2fa99"),
		new("item", "industrial_kitchen_appliances_0008_institutional_toaster", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6d0d333bdb9d454365a78ddf639d9e22a53a9ab52398d66385cd7f66060321c"),
		new("item", "industrial_kitchen_appliances_0013_weatherproof_kettle", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "45751eb47320126044034903210cf52e049dc081356ddf7e257ff1b0b3f06016"),
		new("item", "industrial_logistics_retail_hospitality_0012_heavy_duty_crate", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d3942a9ed91aab6288dac9e0251ce1fb2cb4e84289b299a9d36a6a12efd557f"),
		new("item", "industrial_logistics_retail_hospitality_0017_heavy_duty_ticket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2061ea04121d06f3d67ecfb2de86d921f56fe01f90246f71ce2dd7ed75e14e30"),
		new("item", "industrial_logistics_retail_hospitality_0022_folding_crate", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "34dc286d22ad757214af2afc941cd0e5008f2ed47145c0417bec8c89f65ade89"),
		new("item", "industrial_medical_mobility_0023_economy_splint", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "aab93b8df7f94e6af3dc7cdddae76459ecf216b5257b272d060348c59a4778d2"),
		new("item", "industrial_office_school_mail_payment_0015_utility_tray", "industrial", ["industrial"], ["item:industrial_office_school_mail_payment_0001_travel_folder"], ItemSeederOwnershipPolicy.RequiredRelationship, "4d855a332d2c329c505fde525d2b85d5d08b13fe6217f278e2deb00345672450"),
		new("item", "industrial_office_school_mail_payment_0018_utility_pencil", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e2f1c97afd51d698e796ebd8a1440722e556050e2dd3ae145903a52336c33f4"),
		new("item", "industrial_office_school_mail_payment_0032_plain_register", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "74925a1d1899b305812718032b7236dc62cd04e76a0927604d2ea07f830fcb80"),
		new("item", "industrial_ppe_ballistic_0013_countertop_gloves", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "039b4ed194ffa8587801acecdc1d846d31a33cfadc70dc5ce44bc978b5b4eb35"),
		new("item", "industrial_ppe_ballistic_0032_travel_apron", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "53f583bc202dd02842db5a63cdf2044ba34e58c5160bd50c1e60253e90160b09"),
		new("item", "industrial_raw_materials_chemicals_waste_0012_field_ingot", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9056cb7e99d0ef0e8355234c7e32150589f4a8b7eb4d719ac1ea772f2c17cd3"),
		new("item", "industrial_raw_materials_chemicals_waste_0014_field_wire", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8a8f302d9b5a6558c4547539ee89f057501d1a833da55d7837170d2cab7ab0f"),
		new("item", "industrial_religious_institutional_0002_weatherproof_lectern", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7d5274a24d2e41017a1689502edbb031466f5afa1728d34e632270ba71f016c"),
		new("item", "industrial_telecom_media_photography_0004_compact_camera", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b20af8f9fcf711ca1ab8cf9df16550094b8b84e7a9cfc6f2c003176986a6d72c"),
		new("item", "industrial_telecom_media_photography_0006_compact_speaker", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "35439a743b1e7d1adc5c19d391c477463756efcc986e3eaf4434999eb3d58034"),
		new("item", "industrial_telecom_media_photography_0025_folding_tripod", "industrial", ["industrial"], ["item:industrial_telecom_media_photography_0001_compact_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "18cfc4cce69742888f9096ba5ae2d7ee09eb7356e878a6a303714eed3b90d106"),
		new("item", "industrial_tools_repair_machinery_0009_ventilated_wrench", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "422c3e658c3c3793fa114c60fe59ef1ff00ef98a9721676e661c56f273d8e553"),
		new("item", "industrial_tools_repair_machinery_0020_stackable_hammer", "industrial", ["industrial"], ["item:industrial_tools_repair_machinery_0001_lined_plane"], ItemSeederOwnershipPolicy.RequiredRelationship, "3792f734b4b8553a9ddf1d8e93120dcebb6fe3a017a86d1f67c2dc146737e0e8"),
		new("item", "industrial_tools_repair_machinery_0047_countertop_lathe", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c9311a622e61f2caa0d341e4c05875f454e0bdefacac472564e291e7d72447a"),
		new("item", "industrial_transport_support_spares_0019_compact_chain", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "165efcaf53ac7e07ad278c2ef731201268ebfb54402fcc3fcfb3215a8f10cdee"),
		new("item", "industrial_weapons_police_military_0030_service_signal", "industrial", ["industrial"], ["item:industrial_weapons_police_military_0001_institutional_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "e7b22167d9646cb90fe78563ee10bfec7febb97d3fc4e787b178f270ebde96d6"),
		new("item", "industrialised_agriculture_forestry_fishing_0003_plain_rake", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e3abf1ec7049dd07b33e07b878a8fcd38d9c26c0816b7501f223bc372b951b2"),
		new("item", "industrialised_agriculture_forestry_fishing_0029_compact_basket", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "602a3bbc7d6c65f85f82609b44e4ef745ecb473addd462341159e2da61395000"),
		new("item", "industrialised_agriculture_forestry_fishing_0051_portable_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2a9230247221d77483a627665513d4c1a37dcf6584092e2ade8d01701dcaa63"),
		new("item", "industrialised_agriculture_forestry_fishing_0082_weatherproof_hoe", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e99bff810719a173e38337778c08724c1facea6e561372375c6ff88e64a20a35"),
	];
}
