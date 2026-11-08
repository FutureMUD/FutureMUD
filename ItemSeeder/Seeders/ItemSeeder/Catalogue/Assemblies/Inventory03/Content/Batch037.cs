// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "industrial_household_furniture_storage_0014_ventilated_washstand", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c026656a12823f42d6a2818c5fc1cfe26bfb3b382ce106e5352cde2cd6e4a0e"),
		new("item", "industrial_household_furniture_storage_0016_ventilated_rack", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3046c2dfc9bf39db4509d3276acbc843d114067ea9ccd1b2f7e491c6b531bb4"),
		new("item", "industrial_kitchen_appliances_0011_institutional_strainer", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a07f49572b40300a3ff7a17a1df5f967004d544c9b6155806fab65f1cac2d4b"),
		new("item", "industrial_logistics_retail_hospitality_0009_compact_case", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "79aa264c8363c55ab1a16582d72da398bcbdaeac5810def14f721a218e2ea6aa"),
		new("item", "industrial_logistics_retail_hospitality_0033_portable_trolley", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "22f50a619782c5b7dd8421bc7b54524b48e410911e612e4ef40e1d04140b49c0"),
		new("item", "industrial_medical_mobility_0012_precision_bandage", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "688e8cb312d4a32c8d9fb32fd8d6869eedd31632d4d78b31007335be3d9c291c"),
		new("item", "industrial_medical_mobility_0024_economy_crutch", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f91b9c2d22991478956abd9112cbe4c0540cab9f2b0b6ca4950d4987eb15f1c"),
		new("item", "industrial_office_school_mail_payment_0013_utility_folder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2645730b0c37d68ad7aad7e2178493be86a726002233d773401815b23ea4f24e"),
		new("item", "industrial_office_school_mail_payment_0030_plain_pencil", "industrial", ["industrial"], ["item:industrial_office_school_mail_payment_0001_travel_folder"], ItemSeederOwnershipPolicy.RequiredRelationship, "b9dca4592c09eb7e7d94fadcd087df74e6ef657157da0b32bb8d9b9d60ecf894"),
		new("item", "industrial_ppe_ballistic_0008_field_visor", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee6bcbd7979ea8750cca91f4639d4c2e4f5cbb120ad3e3ae34ade3b1009849c2"),
		new("item", "industrial_ppe_ballistic_0016_countertop_gaiters", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "164dfa59a285a87b36912ffff14b16fa54d49f8823fcd465f38201716fc21ff1"),
		new("item", "industrial_ppe_ballistic_0030_wall_mounted_coat", "industrial", ["industrial"], ["item:industrial_ppe_ballistic_0001_field_liner"], ItemSeederOwnershipPolicy.RequiredRelationship, "05bb7f9d3e6f5f9bd74abc39e24d4782988870b78a02d456a443a6d04a031e0d"),
		new("item", "industrial_ppe_ballistic_0035_travel_helmet", "industrial", ["industrial"], ["item:industrial_ppe_ballistic_0001_field_liner"], ItemSeederOwnershipPolicy.RequiredRelationship, "281ad7d5caca28547e2c374259b041f49e40425b7996ea5327d6e0ddc108c8ee"),
		new("item", "industrial_printed_media_signage_0009_lined_placard", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d20ebb46425684e57adc6b37660c290fb3df004c4e317e7ccad4e5a9743b98f"),
		new("item", "industrial_telecom_media_photography_0019_heavy_duty_microphone", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "10707c3fe0cbd0c028678893518f6bb078675d3fd22d0f22275c0c8cf7228707"),
		new("item", "industrial_tools_repair_machinery_0010_ventilated_drill", "industrial", ["industrial"], ["item:industrial_tools_repair_machinery_0001_lined_plane"], ItemSeederOwnershipPolicy.RequiredRelationship, "bf1fd51b1a084d4901987680cce8a8492aa8d5b0524a1c47ab3894146d925440"),
		new("item", "industrial_tools_repair_machinery_0013_ventilated_plane", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ae4505df90976c554deeaf79f661c44881e84df829cad8044341ee91d91d182"),
		new("item", "industrial_tools_repair_machinery_0016_ventilated_gauge", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "537db99c20a14aa557797e9ab3b80da01bda5b557343c3b37312f6a794a0b02b"),
		new("item", "industrial_tools_repair_machinery_0049_countertop_plane", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d844a4bb4a881d8eeb5d3f118030d63b71f4b41ca4119f088aa5b6ac4d09a27"),
		new("item", "industrial_tools_repair_machinery_0055_wall_mounted_scrap", "industrial", ["industrial"], ["item:industrial_tools_repair_machinery_0001_lined_plane"], ItemSeederOwnershipPolicy.RequiredRelationship, "41a0d7905effb2b3703ab5c77036a26d0c48552c4014454e48f46e4fb4e256c6"),
		new("item", "industrial_transport_support_spares_0003_reinforced_axle", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c0bfa6b97c78fc677e2153b6bcfef339806ce3bef4e972abe4009edebc7f1f6"),
		new("item", "industrial_transport_support_spares_0007_reinforced_pump", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4514e01d49a02ea34b2a58b68bc721408447f15a91cfdd5b4c4737e497f399b"),
		new("item", "industrial_transport_support_spares_0035_folding_spring", "industrial", ["industrial"], ["item:industrial_transport_support_spares_0001_reinforced_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "330f3158e52c8a898bf1d97f1340b39d18ce9cac05ad7d5980cc87c993b37765"),
		new("item", "industrial_weapons_police_military_0033_precision_baton", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "99cb5962f122011cab9472b07d434acbcf9c2838bb45d4a631f55c99d1a74046"),
		new("item", "industrialised_agriculture_forestry_fishing_0012_reinforced_hoe", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "d45210ce8e8a6ef407e40eed49a67db15e794e8bd882bc4511f434d26078ea02"),
		new("item", "industrialised_agriculture_forestry_fishing_0048_folding_trap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "89dd8dcbee4e62126c0f16b9ad28afd04cd52592d560768a1b343b4730a41c43"),
		new("item", "industrialised_agriculture_forestry_fishing_0085_weatherproof_spade", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "eeea1a6049ca7687fe6c19d3c93540bdc4cbd1c1cb126c6fa007df91a619c255"),
		new("item", "industrialised_agriculture_forestry_fishing_0093_service_rake", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "61a9b2a4a240d70ca05b11d186c69ef6085ab68599f4011e3f57480ef23c4624"),
		new("item", "industrialised_agriculture_forestry_fishing_0131_ventilated_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "588f7c758757ddbd1b475fa67f5a9153c35e956e9628cb989b5d4469765805dc"),
		new("item", "industrialised_automation_access_control_0023_compact_reader", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ce7f41ee642d0e65d7094f2fadc9cdb912082e13b8c5cd1952e8b261fd2aa5a"),
		new("item", "industrialised_automation_access_control_0052_portable_keycard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "0fffdca9c63d66b15e6b7a45ab0ed149724031f9e70a0f4e40f1aeacd77d7974"),
		new("item", "industrialised_automation_access_control_0063_workshop_reader", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b79d42fa601f8aa7d89b957db130a14da638c83122a4318858d26c65acee51b"),
		new("item", "industrialised_automation_access_control_0079_institutional_panel", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "506da9ab1a6d50f1a7d1dbd2dddc9f16943fad9d9289fed50dd1c896fa6cbd71"),
		new("item", "industrialised_automation_access_control_0088_weatherproof_indicator", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b45c65d4ef6706952ab108eeb3a2d6548d4fa140e1b1e52e3ce32404f3ffd30"),
		new("item", "industrialised_automation_access_control_0092_service_keycard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e182ef46804e8477e59c4c5c2cdd7581a460dcb6745c3312c547f6601f90107f"),
		new("item", "industrialised_automation_access_control_0100_service_bell", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "a7358438998341f871a55c6f881a9cda676ff97652090796fdc4e2b7f8b169a9"),
		new("item", "industrialised_cleaning_personal_care_0009_plain_bucket", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "1094438f57ad93446dc0a511f5e52c4887483f1f48822ee8fc82bb1d532cd7b8"),
		new("item", "industrialised_cleaning_personal_care_0023_compact_comb", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4390f1219b9b071c537b5c51feba7d8c93340a5815d1bd729a2f2e1df6b49df"),
		new("item", "industrialised_cleaning_personal_care_0028_compact_mop", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8ae027c7a625d7b099937fc7c180dcc2fde0f6326f3c63bddd480548711869b"),
		new("item", "industrialised_cleaning_personal_care_0042_folding_brush", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5fa0e4af1b3bcf761e7e5cb6bd544203ad6e03fdcbae6d50975f95b3fd4b054"),
	];
}
