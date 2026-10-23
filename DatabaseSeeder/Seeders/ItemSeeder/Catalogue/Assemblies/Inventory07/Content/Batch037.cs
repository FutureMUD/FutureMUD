// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "earlymodern_religious_eastern_crown_mitre", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae055775d8996ef90eedd8362c6dbf9f17c45c83faa681d3a6018b183dffa41d"),
		new("item", "earlymodern_russian_clothing_narrow_wool_trousers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1175f5acc60f4b95eda137e1cd32a777e938901ae7d710562c158232d1adcb1e"),
		new("item", "earlymodern_russian_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "942e14f0fcf1e7e32956e910da6e5c3c5f06422739c4e9b8256b4d02e1bec9cd"),
		new("item", "earlymodern_ryukyu_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3fa6475891a0d6994c36adcdd4f94e76f7e27f5198c552a0b75a1155a081e12d"),
		new("item", "earlymodern_sahel_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bff13d8722cef35e3d8302ee4faa704edf3e48fb6fd762aff4ef1298fc9f2ad1"),
		new("item", "earlymodern_science_mercury_barometer", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "64d8c0c06fa4514b8ff05d250ce742ac8db2ef759fba1751d14618a5d00fd4ab"),
		new("item", "earlymodern_science_specimen_cabinet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "34d7dbdefc7a7460009aa12a197bfa2b20617b58e59d6869bdf1c5d2c5641c2a"),
		new("item", "earlymodern_seasia_clothing_wrapped_headcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2063c087046a90a80c766a5eee441a25e6cde0f52493b5c7b752428158d4b639"),
		new("item", "earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2a02996c2d4b7b2c254c8675bd44bf22aa0ecc79229bb6bb3d582e26d377476"),
		new("item", "earlymodern_swahili_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "285161814e4db648b9c3a7b990d791783d0499f126addccbd866bd5fc73bc1c0"),
		new("item", "earlymodern_westafrican_military_canoe_command_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "650377089199a9c4b798037635c2f3baf14d8e41fdfebd95a7a31879675c5a1f"),
		new("item", "earlymodern_westafrican_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "57cf324f47e6396b93962c91ed52004e153d4ae9bebe918d0b8b0cdac9e8dff4"),
		new("item", "earlymodern_western_clothing_linen_day_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9738d6429ba0fe3f1d107ae11a9d98c74327643c78d068a2c6464e445bc905e1"),
		new("item", "earlymodern_western_clothing_plain_wool_waistcoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "258e5ebe66dd002a97022b7209769ec38cb6d2928e3146dacfdd7639a8751931"),
		new("item", "earlymodern_writing_dispatch_packet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0edfe80b7f146fda2adbd62deae4394361ab4992651289028d5db712183c857e"),
		new("item", "earlymodern_writing_indentured_contract", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a21898e873abdc7db74f995bcdfa1487d4505e7a90a79ce49a60d23724b5356e"),
		new("item", "earlymodern_writing_playbill", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "83f8dbcf6f1ac0082e25284a01bf81fd1ff8131f4bef3f76ba8c78671b3a0604"),
		new("item", "historic_lit_oil_lamp", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:historic_oil_lamp"], ItemSeederOwnershipPolicy.RequiredRelationship, "8332636d22a949835b5d4bc1ff7259fcc50ef099b51219d17303f05b3cd147f6"),
		new("item", "historic_tanning_rack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01ab3aa938b638f594f0ba99992edc89396270d437a69da8005631a3fb466b79"),
		new("item", "industrial_agriculture_forestry_fishing_0008_field_trap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec5eac1c929c5288004c10eccc4ad6a24906db1d7e44d672043340135bea3a54"),
		new("item", "industrial_agriculture_forestry_fishing_0021_wall_mounted_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d9def4a6eb6a6a63329c1d625553c7258219ad7e2fafaa46adcab382778fe25"),
		new("item", "industrial_agriculture_forestry_fishing_0025_wall_mounted_spade", "industrial", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0001_field_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "77d40ca8693544c619275727a378eed529f26f4701c691d12d1ee2d59ff0443d"),
		new("item", "industrial_automation_access_control_0001_precision_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b662f3070650aac86ae8564bfdcd2921f3e74d8f11a5ff5857d51afe583ce2e"),
		new("item", "industrial_clothing_footwear_uniforms_0026_reinforced_skirt", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "910d5880e9a954e528723fe587dd90959b14b9e31ffdeb33267f705f1a1c5952"),
		new("item", "industrial_clothing_footwear_uniforms_0035_compact_boots", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "ed01071b4e324715ee4a2fa8d97252a150ef5d4221c27bfbdf1782d09182fb4e"),
		new("item", "industrial_clothing_footwear_uniforms_0040_compact_scarf", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "ee49191e8505cb8866682ea6f0a8cbd1bde986d48bb5e3a058180e50b604d170"),
		new("item", "industrial_clothing_footwear_uniforms_0069_folding_gloves", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "54c4a03e3332563859fc6c1133fe7f3cc9205fb342f55a1b0699c56eaf14f5d5"),
		new("item", "industrial_construction_utilities_0002_reinforced_brick", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "430d02be5eb09ba7123b44e5ce6799b47f6dc789b37e7325a6568618784c35e5"),
		new("item", "industrial_construction_utilities_0005_reinforced_valve", "industrial", ["industrial"], ["item:industrial_construction_utilities_0001_reinforced_rubble"], ItemSeederOwnershipPolicy.RequiredRelationship, "4980e8f5557a568df2c80b129069f530843fca8b678f2df99802568460a2bc50"),
		new("item", "industrial_construction_utilities_0019_compact_grate", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a430626c62f9feb3d05d8e5987f6e399dd2a7a47a3f37bbc1cf4e6f49e88b61a"),
		new("item", "industrial_electrical_power_lighting_0007_heavy_duty_lead", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba901d006582eff5b2be43ae0d3ba2be1e3aeedab15fd1b66f2f780a2908a9b9"),
		new("item", "industrial_electrical_power_lighting_0023_portable_socket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e906597347b4c4d311347f42480dfeb8288dae51fa2541a354adfd1ab0aae3a"),
		new("item", "industrial_emergency_rescue_civic_0005_lined_sign", "industrial", ["industrial"], ["item:industrial_emergency_rescue_civic_0001_lined_helmet"], ItemSeederOwnershipPolicy.RequiredRelationship, "3f242042433dd44886251a120ee171d401ad643ded3d589eb983e97c66f24bbc"),
		new("item", "industrial_food_drink_packaged_0002_stackable_preserve", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "02c08b89d4fdfad6ced793e34213a6ba3470c0f165c833d9384c7b7697ed898c"),
		new("item", "industrial_food_drink_packaged_0011_field_crumbs", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "e35e5803a6d889bc2e41ddc91261cbe4e439c4525bc44bc4317de32dbf8f18d8"),
		new("item", "industrial_food_drink_packaged_0021_field_cracker", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7860a805d7b4c01388e52c69deba9ec414e8cf8028be01ca208e01de4142fb74"),
		new("item", "industrial_food_drink_packaged_0033_countertop_cracker", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "64cbac1981c6146e1fc9b48333a4bc3f6cf1ecdcf7fde82c8b5fb1c38b89f365"),
		new("item", "industrial_food_drink_packaged_0034_countertop_packet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5af30c55d63eb253f517755e0187db87df41d3f397ef0377998ebbbfd91a0cc1"),
		new("item", "industrial_food_drink_packaged_0038_wall_mounted_preserve", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f95734fa3b0b262ebec194f2a8765f0e983f8c56e5cd942c1ce7089e063649ff"),
		new("item", "industrial_household_furniture_storage_0005_lined_chest", "industrial", ["industrial"], ["item:industrial_household_furniture_storage_0001_lined_bedstead"], ItemSeederOwnershipPolicy.RequiredRelationship, "18a1487ff61ef5fc3ac806e54b5170ce90e1f93c6db3e78c9e84068b4fe54b94"),
	];
}
