// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch036() =>
	[
		new("item", "earlymodern_sahel_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fbd89d11a4343dd7bcffcfab993dca70ae15de420a278779d31bf8711879e33"),
		new("item", "earlymodern_science_laboratory_retort", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3234cb3249d52aeb19b156eb6cc3cbcfac16e63bdb9f2efa3724cbd1c7ebd365"),
		new("item", "earlymodern_southasian_clothing_chintz_merchant_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "24b368ee143e742234cd61ff80abfa5423b78c586b252842df3e46fdf947abdb"),
		new("item", "earlymodern_westafrican_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "05dbc5708f7a4fa3a618f04bc7572789ec16bf691c136261d06e106a86cd426d"),
		new("item", "earlymodern_westcentralafrica_clothing_tradecloth_shirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffb4227e6f176dbf1b743dcceb3ea2cbb22e5c7d106ea6519868369ef1aa47c4"),
		new("item", "earlymodern_western_clothing_canvas_stays", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68838e4bd9949b928f242f43abd1b247e1ace24f7e6e4c684510fe4cca33047b"),
		new("item", "earlymodern_western_clothing_fitted_wool_bodice", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0db1ba3abf481856dfdd19272058e0b820f4021d45fa4b313d7ad4609b2cd0fc"),
		new("item", "historic_lit_workshop_hearth", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:historic_workshop_hearth"], ItemSeederOwnershipPolicy.RequiredRelationship, "292ae71d1e6685df348c3541cd2d6a0d1cded5c409cab5dec1e272ce9f673414"),
		new("item", "historic_textile_shears", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68be1f7488b82fa6fa8a3bb21424c57d082d5dad209355d2be1ab891e99f0e7f"),
		new("item", "historic_workshop_anvil", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c3067e55a4db127cb0d49625c8e2db7e47064e1ab6d99ef5d1fdd482f1c6a99"),
		new("item", "industrial_agriculture_forestry_fishing_0015_countertop_spade", "industrial", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0001_field_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "bb0069400ea077690bb4075c06653bd68487a4d3ece51198f2a9d6c7ee098c50"),
		new("item", "industrial_automation_access_control_0002_precision_keycard", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3887ed30f07750b5112eacc3dcb27c242819613c0cf834622f4f907f974602c2"),
		new("item", "industrial_cleaning_personal_care_0003_wall_mounted_comb", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4102fde27516248e3dea27fc6f672d62a2ae40f5ade6b3f0838aa05a7b2d345a"),
		new("item", "industrial_cleaning_personal_care_0004_wall_mounted_razor", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3af8d8a216c7b067b9cdc0e1d0795c62d7b01e2278837a3cbcffa19717f8f1e"),
		new("item", "industrial_clothing_footwear_uniforms_0003_plain_shirt", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1658b74b37922f2dc6018b4ba9d71fdb933b8254a628ac1a19fec2ab6c07cf36"),
		new("item", "industrial_clothing_footwear_uniforms_0006_plain_dress", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6f24ea8c29b57bd6a9cdf595c524bfbed2958109e7525ebdb0bdc01215bcbe3"),
		new("item", "industrial_clothing_footwear_uniforms_0008_plain_hat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0a1b42437a14b2e877a85a25e06edbc78d2c4b36e3e794b78a47ea248790220"),
		new("item", "industrial_clothing_footwear_uniforms_0011_plain_skirt", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1df364154ffcb725e415f68996e6068d5fe541f173439ad31436a9e716b9285"),
		new("item", "industrial_clothing_footwear_uniforms_0029_reinforced_coat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0192cc6edf14ceb93e6823b9fd05346f75fec4cf4ec11f919088c569af7e234"),
		new("item", "industrial_clothing_footwear_uniforms_0030_reinforced_smock", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "47a59f1bff944a5fd7794a80e6f4dfb097c437c255c0d61580e697da4d5093d5"),
		new("item", "industrial_clothing_footwear_uniforms_0041_compact_skirt", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f2ca9ec22715cf089552b9d4755476378a1a606d0f09adcecc412de9c1d1086"),
		new("item", "industrial_clothing_footwear_uniforms_0053_heavy_duty_hat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "804d33f39eb147ea14219957fe6f77d13ac2106d568cfa71de2455c673fd4463"),
		new("item", "industrial_clothing_footwear_uniforms_0058_heavy_duty_vest", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "83a6257884a55e11b1eb6330758db5668b8781817eab90e32669aef3f9d7b8f2"),
		new("item", "industrial_clothing_footwear_uniforms_0060_heavy_duty_smock", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "64ac484d2fa0d95e814f5527ebc8ee90a90cbbe25bde84848a3aaf685dc8d220"),
		new("item", "industrial_construction_utilities_0008_reinforced_panel", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "05abe56ecce2bee0072c5189eff05f453ff86e774bfa5e2ea197cd6298dcd420"),
		new("item", "industrial_construction_utilities_0009_reinforced_grate", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3e90660c63a422d73e8143d55e367b7a6ee122307bec4970f6e6c0c04be0376"),
		new("item", "industrial_construction_utilities_0011_compact_rubble", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "14be894c8d56c8b64426141625892e476d54f9604bd156fb436fa94d41661020"),
		new("item", "industrial_construction_utilities_0013_compact_tile", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c01b70eea1306fa74f211770f8d60d87e6c38cecf372d4aa5701b412e10255cf"),
		new("item", "industrial_construction_utilities_0015_compact_valve", "industrial", ["industrial"], ["item:industrial_construction_utilities_0001_reinforced_rubble"], ItemSeederOwnershipPolicy.RequiredRelationship, "c4a24efacb6f8469ff57efa8844f538ed623b8f5a0fb9ad6c95e65ba13f4cf53"),
		new("item", "industrial_electrical_power_lighting_0018_folding_junction", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "0438df1284c3f70c10ef8eacc525c7fdae3b069b331adf8df00486765fb98277"),
		new("item", "industrial_electrical_power_lighting_0020_folding_transformer", "industrial", ["industrial"], ["item:industrial_electrical_power_lighting_0001_heavy_duty_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "6f24d6e825ba937ff32e1f88562323bf78ab16b12f05f9d9cf628b03d1b01933"),
		new("item", "industrial_emergency_rescue_civic_0003_lined_lamp", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7ff76c9f356739cd4530fbd2d33abca5ac05466dc29e5bc79ba7c3cb2eb4c16"),
		new("item", "industrial_food_drink_packaged_0014_field_preserve", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "e75eece05f8dabb75c476115bea814d20b515ed819184491b618a36a6cdc888a"),
		new("item", "industrial_food_drink_packaged_0029_countertop_pudding", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ba029e8229af417dff8e13d0fce3fc02a60203ddd2f2d18904a6f39b7920347"),
		new("item", "industrial_food_drink_packaged_0030_countertop_sausage", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "10948260a8da2b4600ca8dcc8854870c2cabe0fa1700a92dbb15848a611075fc"),
		new("item", "industrial_food_drink_packaged_0039_wall_mounted_loaf", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fa6a8f2899bfa8871e954f737d63c6de805ba051cc7252120798627d48f45f3"),
		new("item", "industrial_household_furniture_storage_0002_lined_washstand", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "914f2afd12653c86c734a1abdb4f250f978c34086f14338f7f8393bcb09ad45f"),
		new("item", "industrial_household_furniture_storage_0010_ventilated_table", "industrial", ["industrial"], ["item:industrial_household_furniture_storage_0001_lined_bedstead"], ItemSeederOwnershipPolicy.RequiredRelationship, "ac55a44a072ab35a93a07b47831364ba8bac0ef77b03a7afeac8210f91488bc9"),
		new("item", "industrial_kitchen_appliances_0024_service_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c6983719d15d0861735e97f740523c7ca3d9736acb2729fbeba53aabf403ba3"),
		new("item", "industrial_logistics_retail_hospitality_0003_compact_trolley", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2647ef12b5e41b548bfd0be7f56e5b4adfab9adcec864bf77c69b1a1f3958c04"),
	];
}
