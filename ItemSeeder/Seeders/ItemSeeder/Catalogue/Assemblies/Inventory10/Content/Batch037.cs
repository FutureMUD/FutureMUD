// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "industrial_construction_utilities_0016_compact_fitting", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4726a2f288a170561bc20282457309c8d1ae9b9f19611f359acfd864a8b12bc"),
		new("item", "industrial_construction_utilities_0023_heavy_duty_tile", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e110450d2507e33abbf46c5a51ea710dcedcaa3bebb4b4ecfee7c5783617865"),
		new("item", "industrial_construction_utilities_0025_heavy_duty_valve", "industrial", ["industrial"], ["item:industrial_construction_utilities_0001_reinforced_rubble"], ItemSeederOwnershipPolicy.RequiredRelationship, "c309fc4dbd4402209143a0f7d5c2f68c7326382473b1f94f79e51eabf7f4d7d9"),
		new("item", "industrial_electrical_power_lighting_0006_heavy_duty_lantern", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a27c905e127e941240b964011499fc26507b35e76928b3d5f78935cfe03d00c"),
		new("item", "industrial_electrical_power_lighting_0012_folding_lamp", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ea89a149b882ccd36faf1c875776251988c79d22bd0354c09613c52f6d7a60f"),
		new("item", "industrial_electrical_power_lighting_0015_folding_fuse", "industrial", ["industrial"], ["item:industrial_electrical_power_lighting_0001_heavy_duty_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "dd5c3740e370e4db5aff2f3eeb18557d83e8e8993437ab049bea312a1788a2e9"),
		new("item", "industrial_emergency_rescue_civic_0004_lined_barrier", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "35051cd97a4a7e07fd5a50e2874a0e1ce70cc0daa81d332d48a484fa9fa7b5fa"),
		new("item", "industrial_emergency_rescue_civic_0006_ventilated_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "427f1cd41269e70c4fd9e8a0f6890e46a1e5aa39ecb7d20b0f72be8cb32f5167"),
		new("item", "industrial_food_drink_packaged_0010_stackable_packet", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "c6b6552e8dc893b7cd0d6e9e9d5b741ab4fded183b5f87984967d133e85237f9"),
		new("item", "industrial_food_drink_packaged_0013_field_ration", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "829879e0215041d676f7bf40e7231a0bbb4721373c4c9c560b240ebd0e74019f"),
		new("item", "industrial_food_drink_packaged_0046_wall_mounted_packet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "20b185a84b8f82c09c809efd002a4ab3bca80f873010af713a4c171d27e9a411"),
		new("item", "industrial_kitchen_appliances_0005_institutional_whisk", "industrial", ["industrial"], ["item:industrial_kitchen_appliances_0001_institutional_kettle"], ItemSeederOwnershipPolicy.RequiredRelationship, "93ff1700cfa7d15e8049a1f3621fb5b9d8825956a2f6fd6e85c1c540febc68e8"),
		new("item", "industrial_kitchen_appliances_0012_weatherproof_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "612e0f8edd2322a760f76bbee59890a423ecf0fb2f0a676d40fc57ad43968a52"),
		new("item", "industrial_kitchen_appliances_0014_weatherproof_pan", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "06b6d36bc0e19f9981d55d24ba25208e43dad9f073df1c0af8075fb0e7425c35"),
		new("item", "industrial_office_school_mail_payment_0008_travel_register", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc2d3c3959741ac8d729c0ab73643a950a1338e91fc6589fbc0e03d762503df2"),
		new("item", "industrial_office_school_mail_payment_0027_plain_tray", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "e47468906a6ee6d0e899271a96710adb388e137b9183c43c7adcd2680fb4f478"),
		new("item", "industrial_ppe_ballistic_0002_field_apron", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ab6352b90a193e6ca8937b199f90a614ff431cd68d1062ef9fc1838f07c6a41"),
		new("item", "industrial_ppe_ballistic_0021_wall_mounted_liner", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1ab85c1bdd4602da864d4332ce431bda9996af963a2563de277132b14e4d48c"),
		new("item", "industrial_printed_media_signage_0001_economy_manual", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9088aa5d397794eb5ead770d42b04afc332cdc1c5563be503228a12df7bfb7a8"),
		new("item", "industrial_raw_materials_chemicals_waste_0004_stackable_wire", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "6dbd2394d469ba7c23f9dde908cfe0387f5b54a3edc4553cd2f3101f585356b0"),
		new("item", "industrial_raw_materials_chemicals_waste_0015_field_rod", "industrial", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0001_stackable_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "962799b32ab992daa07640a19ed0fb9d5a29ed0d2c3f83ae082704eb82d4fa41"),
		new("item", "industrial_raw_materials_chemicals_waste_0020_field_slag", "industrial", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0001_stackable_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "ce4b0441c15075920086a05670c921e2336460b4bf0946c9522ccc57b08f9e1f"),
		new("item", "industrial_religious_institutional_0004_weatherproof_vessel", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8912b88796ec23b75e4aee5ff74bcfc198cf9f9be928ac00a310acb04fa752f"),
		new("item", "industrial_science_education_0019_field_balance", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "021e01562308ad1567dfc5bfdde77f23bfdc149c6aa75d3aeef46a7560a0d9a2"),
		new("item", "industrial_sports_recreation_music_0006_heavy_duty_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "57127447bf2e85a4fdf20517f41eea308435f31b273151eb95afac189580e970"),
		new("item", "industrial_telecom_media_photography_0012_heavy_duty_telephone", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "6296ccf0a15b86ecefd93b79e65d19faba6f955dc2280a768265d28c065be107"),
		new("item", "industrial_telecom_media_photography_0022_folding_telephone", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "74d04d1306fb7a3f650f012e3a982e57371f611cdb184180f93e6cec06fb91a5"),
		new("item", "industrial_telecom_media_photography_0023_folding_receiver", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "df18faa56bdb5baa8e512cc07f440d2ab8dc2b665cf97fb2e799139efa8ed044"),
		new("item", "industrial_tools_repair_machinery_0026_stackable_saw", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d516826fb0037703354d82d140f8d04e403005053c10d53ab6dbb2f5d783fc74"),
		new("item", "industrial_tools_repair_machinery_0028_stackable_gauge", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c73d9ab3e2c96f8e09ad28bc21a1b96370c23a576c69358ab9f09baffb43990c"),
		new("item", "industrial_tools_repair_machinery_0031_field_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "404b8efe17eed75f1022ed032e2d76ffb6ce4314195c7cc6314fd80fb3375b1b"),
		new("item", "industrial_tools_repair_machinery_0043_countertop_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "59acd421b129cbb2c3f87d4da7b41edb5c4019fd71b2548b2b5b135a1e47ad84"),
		new("item", "industrial_transport_support_spares_0037_folding_pump", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e57e73a45c6c1c3653f8dfcbf61e3c44ed69f14c3189278c102b8a26ceb01d4"),
		new("item", "industrial_weapons_police_military_0029_service_pouch", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f00c28ea1b7b9861699b2e03afcadf4e0505667d4cd03ebfacee7b3db92177f2"),
		new("item", "industrialised_agriculture_forestry_fishing_0063_workshop_rake", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "4863d85f7cc512c700280bd6e1564c8bf6292728c1c1441deaecaafc95b489f2"),
		new("item", "industrialised_agriculture_forestry_fishing_0080_institutional_yoke", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "2a8406aab2bb61dfac8ba56d50ddc6e762957cac83498c084e0eae7f69fc710f"),
		new("item", "industrialised_agriculture_forestry_fishing_0088_weatherproof_trap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "44f2ccb11446b2ad40e3cef68a3c8b6bcce9964c9505d6c2610fec941f7bba3d"),
		new("item", "industrialised_agriculture_forestry_fishing_0104_precision_fork", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "83f2101aaeecbf24149e3a2031bf3435a27d838515d469e015054eaea69ade72"),
		new("item", "industrialised_agriculture_forestry_fishing_0126_lined_shears", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "62fe437927daffdeee2a3738a2235f09c1f43fdb520d2bf0d10bc156c2914333"),
		new("item", "industrialised_agriculture_forestry_fishing_0138_ventilated_trap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "9009654053c9eb068ca3c7a5c81547b2a0372877534ec33ab69821bfb4f051c1"),
	];
}
