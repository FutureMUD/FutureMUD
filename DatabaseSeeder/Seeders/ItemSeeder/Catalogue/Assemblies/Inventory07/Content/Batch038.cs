// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch038() =>
	[
		new("item", "industrial_household_furniture_storage_0007_ventilated_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ed2189428de2b7821f7c29c9e11b4c13522e16e8f7d12106268400580808fac"),
		new("item", "industrial_household_furniture_storage_0011_ventilated_cabinet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8ca4812de8345564037f4c60848de9308fbeac09ae2e91de2d247009fc399a1"),
		new("item", "industrial_kitchen_appliances_0009_institutional_mixer", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc96e9abb7ae5027fadf603bb8c20e6014051506328373974a1d3bc3bd5feccc"),
		new("item", "industrial_kitchen_appliances_0010_institutional_press", "industrial", ["industrial"], ["item:industrial_kitchen_appliances_0001_institutional_kettle"], ItemSeederOwnershipPolicy.RequiredRelationship, "124ab0d63d9fe411d7f581a694cd2b44b03a2c76282e2a01c72d2a8e9161d8c3"),
		new("item", "industrial_kitchen_appliances_0015_weatherproof_pot", "industrial", ["industrial"], ["item:industrial_kitchen_appliances_0001_institutional_kettle"], ItemSeederOwnershipPolicy.RequiredRelationship, "b039af338c81a5df28640fa988223b61074b8e41bea19dd79499e2a6ae730ede"),
		new("item", "industrial_kitchen_appliances_0023_weatherproof_strainer", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "aedf4493792283cbb35d14247f2d779686ff0eaec8a62920249bb5cd7ecb971c"),
		new("item", "industrial_kitchen_appliances_0025_service_kettle", "industrial", ["industrial"], ["item:industrial_kitchen_appliances_0001_institutional_kettle"], ItemSeederOwnershipPolicy.RequiredRelationship, "1edb84f3a04342b4773d3fa8f42988245a1afcb9faf21125e7e23199dbd17eb0"),
		new("item", "industrial_logistics_retail_hospitality_0011_heavy_duty_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fe6756aac76d074a3fe6e93d86e63f68e956135dd2febe95e25df59449c05e5"),
		new("item", "industrial_logistics_retail_hospitality_0019_heavy_duty_case", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "62c9ed0e4ebffad7aa4a06e33289e124b3fb1a1a6220298289ed2789426955b1"),
		new("item", "industrial_office_school_mail_payment_0014_utility_stamp", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "86d9cfddbb3bb07c627ed92e5aa463d5eacf918dff7e859ddbd18ea941ecf809"),
		new("item", "industrial_office_school_mail_payment_0028_plain_ruler", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9f962ba27401bc4db1b17b818e1288855621e2ea6ba878a5e44a3892c577553"),
		new("item", "industrial_office_school_mail_payment_0029_plain_pen", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7484f55b4b8b5b49f957742eb13887b63524b659dae4ce221996221656318ea1"),
		new("item", "industrial_raw_materials_chemicals_waste_0003_stackable_sheet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd1bbde3ccbc08982ddca771a36a3f0eebf5ed8fcfd102e0c22a26f58028d076"),
		new("item", "industrial_raw_materials_chemicals_waste_0016_field_powder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b13ad2f1ca83b39b428ab7050c69af2f4416409d89e42666ddb1f3f1aae41ff"),
		new("item", "industrial_religious_institutional_0001_weatherproof_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "24cb5a01de4c844593e231413fc11e3954b1b6f9e4bd268e949e252872a10789"),
		new("item", "industrial_science_education_0004_stackable_flask", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "29a9a8ab947b8b1066cd0b427bc5592230999402f626b245e572dd9a2a9aa86d"),
		new("item", "industrial_science_education_0005_stackable_beaker", "industrial", ["industrial"], ["item:industrial_science_education_0001_stackable_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "43a0df7f32883569ce6b44b962b5f7cf7e43f7e8ac0db5fd22951fb0de77a538"),
		new("item", "industrial_science_education_0010_stackable_case", "industrial", ["industrial"], ["item:industrial_science_education_0001_stackable_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "b8484563815e965a419e79777850a30b08d68bc317bf6357d2f74bf65effdf71"),
		new("item", "industrial_telecom_media_photography_0017_heavy_duty_recorder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "73ef8b24640d5625bcce560b7f8ac6b3b2e207ee919eafc11aad0883f4b4bc1c"),
		new("item", "industrial_tools_repair_machinery_0003_lined_press", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "58935331a5cc37e2776a539d75c6231b9468c55a1259b9e605647d4f4309fef8"),
		new("item", "industrial_transport_support_spares_0031_folding_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "68c2267bf95cee30aa986edbf9c97de811cd3863e250024aeaad61885ae29e62"),
		new("item", "industrial_transport_support_spares_0034_folding_bearing", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f102d14db7b42b4e12202166984d80c4d472b9907e451c85c9068ba146aad1e0"),
		new("item", "industrial_weapons_police_military_0003_institutional_baton", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9a4f57ef443f8ff926ab4556829aaca7c3c2e92c93d5c94f16afb3990c7783d"),
		new("item", "industrial_weapons_police_military_0012_weatherproof_holster", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7fa3ae0c7ff4a3abedd13ff4e3cfabb7ed9476fe00614834d3896482b39f230"),
		new("item", "industrial_weapons_police_military_0018_weatherproof_helmet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6cbfb4966ab6703121a6121f8622ab7ea1177d5533f2515c81ae00f698f4e0e"),
		new("item", "industrialised_agriculture_forestry_fishing_0001_plain_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e3a69ea213862f5e0e3b4fcd3c60c4c192823a80d7700d2f2bab2ea285e6495"),
		new("item", "industrialised_agriculture_forestry_fishing_0006_plain_shears", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "9630f66c8fc6ce428db7b58820090796cfd2eddebcf4cb10aa61d8980e7f951a"),
		new("item", "industrialised_agriculture_forestry_fishing_0016_reinforced_shears", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b4ac1c615bcaeec001d2a2878701f13b072a0f81dad2cfc395a02bfdba1b1e7"),
		new("item", "industrialised_agriculture_forestry_fishing_0040_heavy_duty_yoke", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "fc32951b78f87c062fda4308778526944eae16e22266b8597d2d3fc35ad6c189"),
		new("item", "industrialised_agriculture_forestry_fishing_0056_portable_shears", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bad7cb6d251c49f1e15ec90097c88ad1793e965512a0a4918d69a4bdeaaa4db"),
		new("item", "industrialised_agriculture_forestry_fishing_0069_workshop_basket", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9c94667753fd6b48d5eb5f1b184e604fbb64ca063157c4f0170a933fe39f63e"),
		new("item", "industrialised_agriculture_forestry_fishing_0071_institutional_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "28b71780f56831d0012a466141fde8c420593aa989f44d8f3e240bc97c123b35"),
		new("item", "industrialised_agriculture_forestry_fishing_0076_institutional_shears", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "78d9ddcffccf08c0937e3cb2ff341a73fd8cf2f037f75dfc6c41fff5ad9ba6b8"),
		new("item", "industrialised_agriculture_forestry_fishing_0087_weatherproof_net", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "14cfd669b75a7d182ec6d842bf96b5d2597521411922dbd69ccbc61a97ede296"),
		new("item", "industrialised_agriculture_forestry_fishing_0137_ventilated_net", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd9d4668a5607973b40dc1b33e49a9b47b7b1e1dc4ba9d82d6258e32d3d2d8e0"),
		new("item", "industrialised_automation_access_control_0004_plain_relay", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fb0e8cd0d31c952667d2799601ee2c33e40f0804e72ea08a16a337af0fa8a5c"),
		new("item", "industrialised_automation_access_control_0009_plain_panel", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "207b776f289ea1fde27b5ef32c79b2061b5b65e2841a8ea9662a6137742dcd74"),
		new("item", "industrialised_automation_access_control_0019_reinforced_panel", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "5734f2601548500adaad7841dc013ce72c207276e92ba1f53227819cf859b46e"),
		new("item", "industrialised_automation_access_control_0021_compact_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "437bcd2357a3628cd4e676cfb924c76e92bbc2380664e7aa0fbdec22456fb3d5"),
		new("item", "industrialised_automation_access_control_0050_folding_bell", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "fbd50d97846ad22f2f3fe959f04c2853237f18bdd24921387440a5e393ffe344"),
	];
}
