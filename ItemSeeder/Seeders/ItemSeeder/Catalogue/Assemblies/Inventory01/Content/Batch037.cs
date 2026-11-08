// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "earlymodern_writing_natural_philosophy_journal", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a85f074b508590c4a07715246a0f7620aca2eecb27bd30f0a9f3e97f9163ef3d"),
		new("item", "industrial_agriculture_forestry_fishing_0018_countertop_trap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "77a4d08be95cc526a8e6140e0c2233595549bdfbd2b99f8230ee88bc4edfa2d0"),
		new("item", "industrial_agriculture_forestry_fishing_0023_wall_mounted_rake", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "29b55001fc6875cec3f2a780e67a25cc04417b35509de424d9ba22742f976568"),
		new("item", "industrial_clothing_footwear_uniforms_0005_plain_boots", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "d7f7599373d2000665ef6a2bbfb76d01d1d3308a2a7abb78a3e0973fea71bc7b"),
		new("item", "industrial_clothing_footwear_uniforms_0012_plain_socks", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff2eb20f1d5fd3e158b794044754e7d22ca7630d42a2e67000ff58aceb071989"),
		new("item", "industrial_clothing_footwear_uniforms_0042_compact_socks", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa8a3f1a0ce8f756c4c00d6b8836c2f420ffca6a0b6b76c145e7c55f40997785"),
		new("item", "industrial_clothing_footwear_uniforms_0064_folding_trousers", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d267ea3aa7c42c75279054512eb4945b426361f89dd4fee6a4f1174ae158c1a5"),
		new("item", "industrial_construction_utilities_0001_reinforced_rubble", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "857295bc8a8ffeb61d2bf5cbfc6fd57185a35bf2e331a6c4bfbf8d17d2bce2ad"),
		new("item", "industrial_construction_utilities_0014_compact_pipe", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "65d1b20d432dca6c0b3b26765d4da1a0c54147fd8b6c73694b5e95e2f144ad73"),
		new("item", "industrial_construction_utilities_0017_compact_beam", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8759fa67da89aaf67bd87d60396331458d779532e715953b90d758f2ac9d9fb0"),
		new("item", "industrial_electrical_power_lighting_0021_portable_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d52c7b2b6d75a84969c1a71dec107e966891ca7f6163984a070fb0977e28830"),
		new("item", "industrial_food_drink_packaged_0005_stackable_pudding", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "2fa6c138073a8ff8e6be43d3e348c1b87ec7ec86b26698204e6e1b226bcc27f7"),
		new("item", "industrial_food_drink_packaged_0015_field_loaf", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "3abd034dd429cf61fe96f0055f1e6b48d33af63629844a16b624557e2d0e6be5"),
		new("item", "industrial_food_drink_packaged_0022_field_packet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0e7f3712bedfe4f273c2d73187bd40ab588c43af087e777d899efe9ec642009"),
		new("item", "industrial_food_drink_packaged_0024_countertop_biscuit", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cde4793aa2916cf7f33fcbdd7bf930d861421c4388f589c4280a9629828c5bc"),
		new("item", "industrial_food_drink_packaged_0027_countertop_loaf", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "19c0297236a0995b0f8fe8ca25620aeb04087dc6ebbe8a6b97d5d3ae494b8737"),
		new("item", "industrial_food_drink_packaged_0049_travel_ration", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0fab976cfb3ca2fad6d997cbb3c779ad2338d8529cfc83a22444c01af7e74f7"),
		new("item", "industrial_logistics_retail_hospitality_0007_compact_ticket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f4e345d4e1650c5af932997d27d0b39b9dae2aa12a75d4c6a23acc7dfd7d3f0"),
		new("item", "industrial_logistics_retail_hospitality_0015_heavy_duty_counter", "industrial", ["industrial"], ["item:industrial_logistics_retail_hospitality_0001_compact_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "c352ae51497645be77ff760808d76e1e67a82e716daa8a7db7d050336ee19122"),
		new("item", "industrial_logistics_retail_hospitality_0021_folding_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b8c9c0da21e565fdc56dd1fff4974ecad7f850566e4a19a71fcea0c335992cd"),
		new("item", "industrial_medical_mobility_0025_economy_stretcher", "industrial", ["industrial"], ["item:industrial_medical_mobility_0001_service_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "73e2ef5c9ed710cea8d9f9accb1e44d9d63859a79109a8c1f89f54d4c2aa2150"),
		new("item", "industrial_printed_media_signage_0004_economy_label", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a10621bfefc5497a4c3f7ed0a5eb7b0322c7c7bb91deea5d3c2eea350f07c94"),
		new("item", "industrial_raw_materials_chemicals_waste_0017_field_pellet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbca3fbf640d15700b602452b2540832f7fd540b073d1b9246b1acc22db0e47c"),
		new("item", "industrial_raw_materials_chemicals_waste_0022_countertop_ingot", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "bca4e63278ef9ca03021becc18cbce65698be165211db234e47c4d06f102d2b8"),
		new("item", "industrial_religious_institutional_0003_weatherproof_kneeler", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4a772eee152bec9e59b51697122d9c168f8cd4a4106ae5f426de32902d4b754"),
		new("item", "industrial_science_education_0001_stackable_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "271d46bf46aa2e30414d02fcf53c2758012f84f742716a27c46563cc2191f4f7"),
		new("item", "industrial_science_education_0007_stackable_chart", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8d97ebffc349d71c9c1fed69d49bd94e6a899c65c62fd88ef369ba786c2b501"),
		new("item", "industrial_tools_repair_machinery_0011_ventilated_lathe", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8560dd19c249ad86915d55956625f4ae2743b61071cd4dc3ceb9d15451d7aaf4"),
		new("item", "industrial_tools_repair_machinery_0035_field_lathe", "industrial", ["industrial"], ["item:industrial_tools_repair_machinery_0001_lined_plane"], ItemSeederOwnershipPolicy.RequiredRelationship, "2171757e6fc2ab4dc260c44818426444a360e17f23b51aad68d81744d6a03856"),
		new("item", "industrial_tools_repair_machinery_0048_countertop_vise", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "58743a244e4863031bc98de2fcade2ddf7ed03c4cc61347744cb42970e6a8a13"),
		new("item", "industrial_transport_support_spares_0015_compact_spring", "industrial", ["industrial"], ["item:industrial_transport_support_spares_0001_reinforced_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "710085eff6c5b88c6c5c31c8c9072b79e405b68d627aa4a8883941636f00bd7c"),
		new("item", "industrial_transport_support_spares_0021_heavy_duty_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "06c5edd4d3ce4554ee9d0d0c9f82a12bd7f7c1634a8c9a2c395d55f486e90d9f"),
		new("item", "industrial_weapons_police_military_0020_weatherproof_signal", "industrial", ["industrial"], ["item:industrial_weapons_police_military_0001_institutional_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "b08f1128c11f0d3781b01ffef4b01e4abfa130a30a9c9f86eb615d5cc1207e93"),
		new("item", "industrial_weapons_police_military_0023_service_baton", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba823ed7215951b537dd237d378637737bed93548e34bc9006005cfd05d19f75"),
		new("item", "industrialised_agriculture_forestry_fishing_0055_portable_spade", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "527cec8a0fa99129cf0b92b78393381ef783d674c4d7e2afa134512ae128d42c"),
		new("item", "industrialised_agriculture_forestry_fishing_0091_service_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "5000512eb98d41e7f174622b15dfceec769aef1a7b90011b20ff2f37d683fe6e"),
		new("item", "industrialised_agriculture_forestry_fishing_0095_service_spade", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "8837e30415eee227b0140d85ba6df83e804fdb8bdfee10b05c2d760e4de743ed"),
		new("item", "industrialised_agriculture_forestry_fishing_0096_service_shears", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "214344847894a78098ef904ec22fba89202784e78092f67563d59655abb61d2f"),
		new("item", "industrialised_agriculture_forestry_fishing_0109_precision_basket", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad5c4ce97ad47799e29a5d0552d2365e2871376fbf944befd2d5e5f044c9abaf"),
		new("item", "industrialised_agriculture_forestry_fishing_0114_economy_fork", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "44bfce113a9b5db1ec34214013cefe5e34f3b229d40d01d9cda18c45ade360bc"),
	];
}
