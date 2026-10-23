// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "industrial_logistics_retail_hospitality_0004_compact_tray", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2eb4213f6cea9a9f150296aacb94af30ac485ea99fe1b534c6aacdf36d64e0e"),
		new("item", "industrial_logistics_retail_hospitality_0013_heavy_duty_trolley", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "65be9bfb5cb8c656d80bc924fe72d28af279922b5ec741afadf91c670b8e344c"),
		new("item", "industrial_logistics_retail_hospitality_0028_folding_basket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "604a1a86b3be70ae74abc0d0d97dd88d972aa4dbf5ad22c18a0a914836e37745"),
		new("item", "industrial_medical_mobility_0003_service_splint", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "008fc44b310f19fb214136840383cc97e93362e82100a1d5ce1c99ff36c862c2"),
		new("item", "industrial_medical_mobility_0006_service_syringe", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5a2ac97d75b6cdd378703799c40f7464526ac69965bbb0c02ec97d916abe4ad"),
		new("item", "industrial_medical_mobility_0009_service_case", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b836f78188ee9a309868b512bed27fd0974ee8273f7aae61303e5801ee35ba7f"),
		new("item", "industrial_medical_mobility_0011_precision_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "36c52d7af79c49cfa2b24017ccd2e345f1e312260176660bb22fd318471f8bc2"),
		new("item", "industrial_office_school_mail_payment_0009_utility_wastepaper", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d7a6e21d421f348405be216fa96c8802294a343e2e02a5724a2a622599c836d"),
		new("item", "industrial_office_school_mail_payment_0022_plain_ledger", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d798e0d5ce136a7b9fb6ed60114fed7280434ae0d99cb6289ac1a0eee8d665be"),
		new("item", "industrial_ppe_ballistic_0031_travel_liner", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee545291382665e949eef9e6516fa1c2b9d4077895e1493ccf4fa8843e756d33"),
		new("item", "industrial_ppe_ballistic_0034_travel_boots", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9b0072529f6604eee32de0a89e6a650f89b395971e1eaf85345bee812aff408"),
		new("item", "industrial_printed_media_signage_0010_lined_map", "industrial", ["industrial"], ["item:industrial_printed_media_signage_0001_economy_manual"], ItemSeederOwnershipPolicy.RequiredRelationship, "0f4b146a1a8ca1c2ce35c3d00f0a51afe852d74bf7de3b32d4deccdae161983c"),
		new("item", "industrial_sports_recreation_music_0009_heavy_duty_racket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce514db9999d5d81c24be07f12d475087f11b09bff7c378672ca878f6ea11a24"),
		new("item", "industrial_telecom_media_photography_0008_compact_projector", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd4fa4c4becb7a61bb162a79d4e0ecb4bc104c050e7b7245675eb5f2e8d1607e"),
		new("item", "industrial_telecom_media_photography_0021_folding_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6d4a75c5d797eded99ea1d985a8c8001a8644b599e1d120395c0e075c13a9b3"),
		new("item", "industrial_tools_repair_machinery_0004_lined_gauge", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d63faf4398bff545f51fabde21168f0e8552c715d2a4a819c35aa6563951663"),
		new("item", "industrial_tools_repair_machinery_0023_stackable_lathe", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3085aacddfdba62a811582fdca8cba331aeb3c0ef324a6ca57f9e34a50117058"),
		new("item", "industrial_tools_repair_machinery_0045_countertop_wrench", "industrial", ["industrial"], ["item:industrial_tools_repair_machinery_0001_lined_plane"], ItemSeederOwnershipPolicy.RequiredRelationship, "a584598f84dc39a87229b4f7c854e5272f8df2154276699fa49fdd1a548baf8a"),
		new("item", "industrial_tools_repair_machinery_0052_countertop_gauge", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f39031101fa8ef7bf615627e3c04386425b2ed7c45b0af29a5a80f5600ddeb5"),
		new("item", "industrial_transport_support_spares_0004_reinforced_bearing", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9bf038d44c8617c4b58d98154e04da8ff2b76c8ad1e216dcd39428bd9c4ff7e3"),
		new("item", "industrial_weapons_police_military_0001_institutional_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9436e71ac1114e16f444cec4306dcb734a26fcd6074bfbe4277c26c987c2e924"),
		new("item", "industrial_weapons_police_military_0007_institutional_shield", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "70a0c2e32ca456472f996ea550c62efdf7a6696b4213e4acb79ee7875d6d18bb"),
		new("item", "industrial_weapons_police_military_0011_weatherproof_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c03013abf1dcf13af6d8371e60facfcbf5bc29395b2a97846292eb6de67fa5d"),
		new("item", "industrial_weapons_police_military_0015_weatherproof_cartridge", "industrial", ["industrial"], ["item:industrial_weapons_police_military_0001_institutional_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "9e7edf698982c7b64fac3d799044113ed6ce703c3a8654ea49c74448d8431c82"),
		new("item", "industrial_weapons_police_military_0017_weatherproof_shield", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "631e4c720ff58fcf5555a40efe54fbd004fadec586e7c435c764e864240242f4"),
		new("item", "industrial_weapons_police_military_0026_service_magazine", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c3b4ebb97c3c467ae8c1f3b716c7126236f318f6c206cdc46e3699f939b1e0a"),
		new("item", "industrialised_agriculture_forestry_fishing_0024_compact_fork", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "43d6bad6e14633cb3d9b07ca7a84a09180c9afab6d5d76246910baf7b07930f0"),
		new("item", "industrialised_agriculture_forestry_fishing_0026_compact_shears", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e33649fa009ecc26f137252a22d8c634d624e470ae374ae768c17d36df2af62"),
		new("item", "industrialised_agriculture_forestry_fishing_0059_portable_basket", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4031ab04a6d6632cbefe6c22cfdf10e5bda4d76f8fd7a827ac4167ff18832fd"),
		new("item", "industrialised_agriculture_forestry_fishing_0061_workshop_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ee2e48c467dc2ed82690f9cbcfeafe787685ed5fce92e3d286ae0862dd9b9ee"),
		new("item", "industrialised_agriculture_forestry_fishing_0064_workshop_fork", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "45485d6ced0d9b3108fd24e70036b661aa0775a18cf49f029090a7b8a2ac40e3"),
		new("item", "industrialised_agriculture_forestry_fishing_0066_workshop_shears", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bf20730154f4e8a28290c09b1e85fe5e3fe236f6626d6dbd666263d5fa84fcd"),
		new("item", "industrialised_agriculture_forestry_fishing_0067_workshop_net", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "25313a2ad6d8e81ce4ccee1edd6cc205e359b97672000df8f97912e5cffbb5ad"),
		new("item", "industrialised_agriculture_forestry_fishing_0118_economy_trap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7605177385d4c7d0377a9b2b53fae082899ebc3ecd4f51655b0465d5437fdf5"),
		new("item", "industrialised_agriculture_forestry_fishing_0144_stackable_fork", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "09dff2ba4c0115314551a623a0fafa32e7fefecf038d86175cfe3a1420c4c8c6"),
		new("item", "industrialised_automation_access_control_0006_plain_sensor", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a8232c284b617b298329b7403f8b7bdcca0e12202a53720c7c46ff86dbc77ab"),
		new("item", "industrialised_automation_access_control_0020_reinforced_bell", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_automation_access_control_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "7fd558ac2800b797be36b2b524c705a9e57192456d67cb35fccb80b28014ca65"),
		new("item", "industrialised_automation_access_control_0039_heavy_duty_panel", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd937564f950abcab4b064b2f6494392c4cf451dcab4bdef7559fa02f29a6e0e"),
		new("item", "industrialised_automation_access_control_0048_folding_indicator", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae8477ba3e4f46257b8c391c65a7c61f50d924435c7002d5556f54184720dce2"),
		new("item", "industrialised_automation_access_control_0064_workshop_relay", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "a25367b166498292a30e6515ab16987f8df962e80e9f7b644d19f2ece63d4a50"),
	];
}
