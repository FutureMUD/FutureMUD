// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "earlymodern_writing_company_ledger", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8887284453840c141723965aba2cacfa15af5807256362189b6311a86786f411"),
		new("item", "historic_awl_punch", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "57450c9bafffe4dad97fe28d8022f240d11dd9f86e3a4d3f2abdc23df8502d12"),
		new("item", "historic_forge_tongs", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7bcf50080cf88ef34ef72993fae5a5388b08852fbdec3c7a6a5466cb1670af60"),
		new("item", "historic_hand_quern", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4dff7afd335f1bf5a81cc1533511b8e884a7a1970e415d1f382a5bf73023371c"),
		new("item", "industrial_agriculture_forestry_fishing_0020_countertop_yoke", "industrial", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0001_field_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "daa302bb753ec346394085e114b639a53ee71ac78a717f35ddb5b13ef4607bf5"),
		new("item", "industrial_clothing_footwear_uniforms_0044_compact_coat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e33981a55819c5ccd2c93bc7be5712807275a8d8ad0ecf09cb1ef6ec1a731b4"),
		new("item", "industrial_clothing_footwear_uniforms_0050_heavy_duty_boots", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "0d68b945e0dc45ca97c93637189ae0f5092a54e8d80c7062b3d5002764542a3a"),
		new("item", "industrial_clothing_footwear_uniforms_0063_folding_shirt", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "17a2d084efd4a6f96e25fe215c69b4b69a2e23497fdb716b6a1f4dac3046cae2"),
		new("item", "industrial_clothing_footwear_uniforms_0066_folding_dress", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d38e2d2f4bde4b6fc2ae8658203cab8eddb68159309d2a97b0f5d04ebdd145e"),
		new("item", "industrial_clothing_footwear_uniforms_0068_folding_hat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f99284679316ae3e97e4da419e4cb3b253dc3125ea7490ef66cc8a15989002b3"),
		new("item", "industrial_construction_utilities_0003_reinforced_tile", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c778eee255f72b08b3eee4629323eb089edf4ae4d12a590be8c4e42fd9733dce"),
		new("item", "industrial_construction_utilities_0027_heavy_duty_beam", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9714b02a5c03ac290698f110f61af084855d7efc7ce7e272bf5193f9c37d6ff0"),
		new("item", "industrial_emergency_rescue_civic_0013_ventilated_lamp", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7044fffe094ffc3f52d2419f038bf97eb4f40baa281fa75f18d5cd8d9699cd2c"),
		new("item", "industrial_food_drink_packaged_0009_stackable_cracker", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "719bb5c5a5f5bb4fc3cd27139a8618e3cc36aa0037169cf1f1e6582053fed9c7"),
		new("item", "industrial_food_drink_packaged_0019_field_pickle", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce8dcc1bc294e4063dd35ddd15ed924ff95bb8ed47f0c14b5eecc6bcb0f87798"),
		new("item", "industrial_food_drink_packaged_0042_wall_mounted_sausage", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4f91b7642af0328638e25fc93f6ccde4e252a37c52324ab66e78b373dd888ff"),
		new("item", "industrial_household_furniture_storage_0013_ventilated_bedstead", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "599fa38e64746250a95b26c089871abceec93c0f0ed4ef9eb31db09172eb7c54"),
		new("item", "industrial_kitchen_appliances_0006_institutional_ladle", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "eabee75a774ffae8ef1dd8b9a62895c1d331f871871c509372beb6074a7d0d81"),
		new("item", "industrial_kitchen_appliances_0022_weatherproof_press", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "554918b5ff35a040c161eced09e9ced5f4ce88aea57d53c00067f34de0c63b71"),
		new("item", "industrial_medical_mobility_0002_service_bandage", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1947328f860828c1e754335261f80da1c085c4bcc1df661f77edc57109263771"),
		new("item", "industrial_medical_mobility_0008_service_thermometer", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a68bda32917950076eb37c2508f9d5cd775a5c922568291b5970d02a60bd1bdd"),
		new("item", "industrial_office_school_mail_payment_0019_utility_satchel", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "04427f234c2b0ff0fbae03aec1b311633182364ac16f73bca82970653560f948"),
		new("item", "industrial_ppe_ballistic_0003_field_gloves", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa922d9ca473cf6fbd982c1edb27a0422821c117c6832d54c9105ffad8cce5b0"),
		new("item", "industrial_ppe_ballistic_0004_field_boots", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b19fb2ae458bca6fae8c430e2d623779a3b76990bf6a82215f1db65456fd143"),
		new("item", "industrial_ppe_ballistic_0018_countertop_visor", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "19e4ddab5fc0c0b98ce82e45f96c19a4d6e695b877eea434ab37b1813a77ac48"),
		new("item", "industrial_raw_materials_chemicals_waste_0024_countertop_wire", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8394842e7247d7ce3e6e2c8121f597e251770ac1adb34964d95304fd828327a4"),
		new("item", "industrial_science_education_0008_stackable_pointer", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4eb118ed5550986354b8f0a16dd1810e3df9905ebc060a3841e9f34d41b252e"),
		new("item", "industrial_science_education_0013_field_slide", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8797ea8625abb14f417c438df8d6584b855613834ed7cc74164127baaf2c10d6"),
		new("item", "industrial_science_education_0022_countertop_specimen", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5cfbc1ab1eb1e98152afbe996c7ed9ff33dd78aeebfef22ceff754775186eddf"),
		new("item", "industrial_sports_recreation_music_0001_compact_drum", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ec1cd3c067a7b174f0cef338f6610d6a1c04c49d7559bab76f50e3dd20623f5"),
		new("item", "industrial_sports_recreation_music_0005_compact_skate", "industrial", ["industrial"], ["item:industrial_sports_recreation_music_0001_compact_drum"], ItemSeederOwnershipPolicy.RequiredRelationship, "eae95a972c51867b404c9109bd355632d7be352865439d9041e91fa56f7505c7"),
		new("item", "industrial_telecom_media_photography_0027_folding_recorder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "cce143d94d86cc07ad343d4a6ebb8f1dbd7000e5d5763608dc5015b4467c0e57"),
		new("item", "industrial_telecom_media_photography_0029_folding_microphone", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4cad59eb18f8fd80784595bd164cfb72794cd1864a7f89ab609303562ab406d"),
		new("item", "industrial_tools_repair_machinery_0012_ventilated_vise", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5662817edd7554ad198aba0a93bb5c092a59cf7d874302e9146204162906f987"),
		new("item", "industrial_tools_repair_machinery_0034_field_drill", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7ebaf82b85a4dc300c7055b5ca7482bfaef07137b1b3c3c887b3f4244d59cfc"),
		new("item", "industrial_tools_repair_machinery_0051_countertop_press", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "23b46c43c5f86de5fbccb8318c494ad9efa337fd9b7065f8f2c1bf3f2702b0ab"),
		new("item", "industrial_transport_support_spares_0011_compact_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "807ded418721454e6efa0d306bf2cd0bc3b3d51ebae763bb41ef33f0c7ea648c"),
		new("item", "industrial_weapons_police_military_0002_institutional_holster", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3428e02182cfb1de33ed64dbe6b05683dab0fd05bd96d259918284a350199ea"),
		new("item", "industrial_weapons_police_military_0008_institutional_helmet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "61c221d3f4fcff925414c1fcd1051ff1c7c173c7bb86fa353427eaa9be517688"),
		new("item", "industrialised_agriculture_forestry_fishing_0017_reinforced_net", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "82ebc3272c3ddbf591687de42a8fa5567c1116c6ccd7eff656104d4593d56506"),
	];
}
