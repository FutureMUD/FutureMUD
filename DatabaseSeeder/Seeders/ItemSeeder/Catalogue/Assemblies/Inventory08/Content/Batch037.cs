// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "industrial_cleaning_personal_care_0007_wall_mounted_broom", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d047c6fc1dbd667eb1f4af43c322ba2c6fe85414ab685fa39f8d275b574f8a78"),
		new("item", "industrial_cleaning_personal_care_0010_wall_mounted_soapdish", "industrial", ["industrial"], ["item:industrial_cleaning_personal_care_0001_wall_mounted_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "e9428fe2e16c559bf81299b48680447d817a328dfafd3a19872a34cd081ff45c"),
		new("item", "industrial_clothing_footwear_uniforms_0028_reinforced_vest", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5bbf503fb1f3be10c9a31679ae2ef038fd2ec67b90705cb4e294ad33f6492a0"),
		new("item", "industrial_clothing_footwear_uniforms_0038_compact_hat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a99c2df15b979d5bb5bb90f32a304b7cb2f08819b561d8fa60643991920bb2c8"),
		new("item", "industrial_clothing_footwear_uniforms_0070_folding_scarf", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "fc590687474ba8e0654a700624cc3c9e2e25e7c3fb4333ced47c7122bd631d1f"),
		new("item", "industrial_construction_utilities_0004_reinforced_pipe", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9897ab4a506ed71f50cb7e3673abbf97388b176b0308ccf9aa417202191654d9"),
		new("item", "industrial_electrical_power_lighting_0003_heavy_duty_socket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a00b83d40378280b9094a5e30832796d375d6c4155e6dbd75c2f30a7237c339"),
		new("item", "industrial_electrical_power_lighting_0008_heavy_duty_junction", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "846c96e99e6410208b93335da2b527b9823320a6776288d3d6241d528bcfad88"),
		new("item", "industrial_electrical_power_lighting_0010_heavy_duty_transformer", "industrial", ["industrial"], ["item:industrial_electrical_power_lighting_0001_heavy_duty_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "c68a795eeecfd7f26b2a14e183aea26df7652dd1e99504079bd3164828d8f33d"),
		new("item", "industrial_electrical_power_lighting_0011_folding_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5345afa336e36509b12fe48ec8731d7576d4871d66c8b852428cbfcd273eef76"),
		new("item", "industrial_electrical_power_lighting_0019_folding_meter", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "76c513e42fc4ab2132b929874aed4e90f2a34d2d3dc0614472723e44c93c9771"),
		new("item", "industrial_emergency_rescue_civic_0011_ventilated_helmet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6d7be50f7607be86081673844ab87df4ca410523802fd89684f094f00b66f3c"),
		new("item", "industrial_emergency_rescue_civic_0015_ventilated_sign", "industrial", ["industrial"], ["item:industrial_emergency_rescue_civic_0001_lined_helmet"], ItemSeederOwnershipPolicy.RequiredRelationship, "57d568e99e7a1c3ef9f50350d6d898c4a5d56c22882c6c5136b32cae915ca79d"),
		new("item", "industrial_food_drink_packaged_0003_stackable_loaf", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d64ef9376298644b71520772da26a7a3a282c73b6a00a17fb732c7c6a76d4890"),
		new("item", "industrial_food_drink_packaged_0035_wall_mounted_crumbs", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "3953530a3b7f8533cb74a699124683e1576ce69873f819f95fe84bce42ee1a4c"),
		new("item", "industrial_food_drink_packaged_0041_wall_mounted_pudding", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f2264d63c55575305de43521bc43ec007560b5b8c884319433984257e569289"),
		new("item", "industrial_food_drink_packaged_0050_travel_preserve", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "fef95d220d4b931f04c0ac99848176414f50f304e24f9252fd00e581688b4641"),
		new("item", "industrial_household_furniture_storage_0008_ventilated_chair", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1370f040e7964c5f6e4b68a7a0ba8f6b1cd0296be4be5acfd675fc66d6e2e3d1"),
		new("item", "industrial_household_furniture_storage_0012_ventilated_shelf", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a785cfbd670961fa6335408d8c90b9faf05f4b544dde9e37683658c97d94015"),
		new("item", "industrial_logistics_retail_hospitality_0010_compact_sign", "industrial", ["industrial"], ["item:industrial_logistics_retail_hospitality_0001_compact_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "3587e0c6cdb6e19a29fa3f06e130b2c5ebb3342d1cfc683a4eca6d8932d27de2"),
		new("item", "industrial_logistics_retail_hospitality_0024_folding_tray", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fdab159a6b6c0e7d7893a5fed7052c71f95e5f772f6fb8cea6cb8f8a12e1d3d"),
		new("item", "industrial_medical_mobility_0001_service_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a53a9f95625c9e49d5044da4abddf1d7278c13db3df3ddd3ac0e23082cd59bd7"),
		new("item", "industrial_medical_mobility_0004_service_crutch", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a4ef68c997bd834d703c1109c3fd09b521926e7b787b0a96c8736cefe54f571"),
		new("item", "industrial_medical_mobility_0005_service_stretcher", "industrial", ["industrial"], ["item:industrial_medical_mobility_0001_service_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "0663318a1b7dc16eb498447d90247bedabf75e7bdc91d5c541d18b2d6521aa69"),
		new("item", "industrial_medical_mobility_0019_precision_case", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "18971b2e85575962418cbce9e984b36d4634aaf103deeddd9c6cc29e2db8bebb"),
		new("item", "industrial_office_school_mail_payment_0011_utility_notebook", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b5433e134ab67a9e3e8599f8b051668e3d7099d9928400ee8fb2071429515ae"),
		new("item", "industrial_office_school_mail_payment_0034_reinforced_ledger", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "44dbb05dab752f7f360ee97dc5e385f4efb486759f5d6b767f2d2ed9d8490d22"),
		new("item", "industrial_ppe_ballistic_0010_field_coat", "industrial", ["industrial"], ["item:industrial_ppe_ballistic_0001_field_liner"], ItemSeederOwnershipPolicy.RequiredRelationship, "e7eaed876679bb091970338322e848b5e46c889d3eeb39628b85f0892c9a1fb0"),
		new("item", "industrial_ppe_ballistic_0012_countertop_apron", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "61d84c7f5f531f43cbef3cc5fe8d4a341809543c415bea5e9eabbdd12108ebae"),
		new("item", "industrial_ppe_ballistic_0028_wall_mounted_visor", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "08201e27029143afe902262c67b427422a63d68b0b4b7213c54772d41748967e"),
		new("item", "industrial_ppe_ballistic_0029_wall_mounted_vest", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4be8cf5009074c83f5ca50af69841a3ea204d14ca8814dd066c520def90e7f1"),
		new("item", "industrial_printed_media_signage_0005_economy_chart", "industrial", ["industrial"], ["item:industrial_printed_media_signage_0001_economy_manual"], ItemSeederOwnershipPolicy.RequiredRelationship, "7dbe48ad2682f38adabcc0d7aedf4cace6c4ab75775b75a474af76707e75f44b"),
		new("item", "industrial_raw_materials_chemicals_waste_0006_stackable_powder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "193e2e5ed5bf0e3396379cdfdcfc935c353ed3b7cbb49f02cfe9835083ed72e1"),
		new("item", "industrial_science_education_0018_field_pointer", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f2c53b077da16a4a9f2eb90ba467b53efe1c96156bece7ea02a7fff34dbf14d"),
		new("item", "industrial_science_education_0021_countertop_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5d41a3ad74f239341e466a6042f6ae07f314090ef3bfc5d9145017ba240c59a"),
		new("item", "industrial_sports_recreation_music_0002_compact_flute", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a18c16766ace487e00b2c08df858d639b67dcf558fdc221c1b4c9e1c9e45e5b"),
		new("item", "industrial_telecom_media_photography_0002_compact_telephone", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d30da89815007e266ceb5531288eb439693e8b80acdf880f7215382e10ddfae"),
		new("item", "industrial_telecom_media_photography_0028_folding_projector", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "68702e11f524df80b542b87d01902e1cb571dee7d39c66925e025a7809566681"),
		new("item", "industrial_tools_repair_machinery_0029_stackable_anvil", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "acffbe4d9a67e204997835af8ea110e88ff3582841263a9552a47c6207963ff1"),
		new("item", "industrial_tools_repair_machinery_0032_field_hammer", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1eaa5479f9756bbee9af29cf9ff9ee70d0f439412627e5ff1515d7d27f39fd0e"),
	];
}
