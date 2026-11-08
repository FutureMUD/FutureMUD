// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch039() =>
	[
		new("item", "industrial_kitchen_appliances_0021_weatherproof_mixer", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "91fed7643ed10a1725e3142e236f547b4ef50ded518caefcfce6f45ca51335a4"),
		new("item", "industrial_logistics_retail_hospitality_0014_heavy_duty_tray", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3092fae1df9d07df1bd82f82c62fd653dc69b1c63b69cd91cfe2f8ef7326b11d"),
		new("item", "industrial_logistics_retail_hospitality_0016_heavy_duty_scale", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c75364ff1dfab8fb1fce5667784df254d3743143b85ac91e8a1a568741fa3ebb"),
		new("item", "industrial_logistics_retail_hospitality_0023_folding_trolley", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1da5680daeb5fd2efd389044b773e1470e73e42556d61e1dc1274cfd98e425d"),
		new("item", "industrial_logistics_retail_hospitality_0026_folding_scale", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f011adc587726eecc8835a6839ec61736ef02627e28bbcd89f95a5d828deaa35"),
		new("item", "industrial_medical_mobility_0017_precision_forceps", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "743614f58ef537ce89d8789b65dc36a8c92b661e09c7010c11d3f631425b8e24"),
		new("item", "industrial_medical_mobility_0022_economy_bandage", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c3f7877206ce2088dfb1f7bf1b589a02813b96a4c31f5c784851e66d726c3e1"),
		new("item", "industrial_office_school_mail_payment_0006_travel_pencil", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "af2be3b5dbf10b20546092b2aca73ff05dc9c27da964500312eb1fbd1f26724f"),
		new("item", "industrial_ppe_ballistic_0005_field_helmet", "industrial", ["industrial"], ["item:industrial_ppe_ballistic_0001_field_liner"], ItemSeederOwnershipPolicy.RequiredRelationship, "637c0199479e01e25a8e0c1577cf5ed9103c05d6b042336c27d8064ce4a39bb7"),
		new("item", "industrial_printed_media_signage_0002_economy_pamphlet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b43e1c3eee53d1f27bf2e045bdf9f93a3a6e43cdfba133bf48c74fc364999b50"),
		new("item", "industrial_raw_materials_chemicals_waste_0002_stackable_ingot", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9bd5752ffa4aba1788ae638e3c48196616e60b07f3a8bb85e6cfa3dcff5b0b1"),
		new("item", "industrial_raw_materials_chemicals_waste_0019_field_shavings", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a4560ea8d8beb04dbf560feb97904e5d3c71c4d371e714421f9cc462a42df15"),
		new("item", "industrial_raw_materials_chemicals_waste_0023_countertop_sheet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc2a4ad3f34b79203fa1fcb209a8406d4ecccf580f062017eaa17e4e3e5ed11a"),
		new("item", "industrial_science_education_0003_stackable_slide", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc0c86dafe971c11cb4c0c0dc7e295562d98873716f9b0c4b23ff7319038e553"),
		new("item", "industrial_science_education_0017_field_chart", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a600e2cb021c61eac36a1334810420482720840333410d071042a17fd039d4f"),
		new("item", "industrial_science_education_0025_countertop_beaker", "industrial", ["industrial"], ["item:industrial_science_education_0001_stackable_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "fba6950472ed49a0f50198533c375291642f97a52b9fb8929ef3f324da6143f4"),
		new("item", "industrial_sports_recreation_music_0004_compact_counter", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "bec4c3ee9de5bce937211e02e9285853fc1763f525bf99d3ed635e63333c40fb"),
		new("item", "industrial_telecom_media_photography_0001_compact_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5757a242255d2c92dbad120c32ddc2b160649a7382e9e7bb4bee033233fbcb52"),
		new("item", "industrial_telecom_media_photography_0018_heavy_duty_projector", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f77dfa86a41c9f6884b973b4fa85008a6807eb1ddd167a469baad6af03e9740"),
		new("item", "industrial_tools_repair_machinery_0001_lined_plane", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "29a0e63b2eb73cda6ca6bd1ff63d1e037f6d8ab51e41e7cad22033f2373dc4a0"),
		new("item", "industrial_tools_repair_machinery_0019_stackable_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c197363d64e39e5d748ad8db70a4a00b2702ede7969436d9e1703af76f0eff4"),
		new("item", "industrial_tools_repair_machinery_0025_stackable_plane", "industrial", ["industrial"], ["item:industrial_tools_repair_machinery_0001_lined_plane"], ItemSeederOwnershipPolicy.RequiredRelationship, "2c413e1d97ec0c63f3c20ce25999536cb549c3f2e07aa3bfa0eeb202e213924c"),
		new("item", "industrial_tools_repair_machinery_0042_field_grinder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "990b5600d3e715b830372e7d0b239c55dd4d8eb6d7fdd47a8797c2067ff1538b"),
		new("item", "industrial_transport_support_spares_0006_reinforced_lamp", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "79141795b020a4e93fbab029d49bfbb2e11f561167a2fc4acf5a6c4b1ff961a2"),
		new("item", "industrial_weapons_police_military_0027_service_shield", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d612c686b4ebeee12bb48c4ea5cfc9cbb16b8305142b8c6a3c0b8b731776af7a"),
		new("item", "industrialised_agriculture_forestry_fishing_0018_reinforced_trap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "c84f74a9c5b3aca0f9aba2182987f894b250a277ea897e2bddd9a45aee7a27de"),
		new("item", "industrialised_agriculture_forestry_fishing_0021_compact_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed67bb486adc31c3feb049b0f7741ae9f74c3597d2c1ddee43e2e778c68c75ee"),
		new("item", "industrialised_agriculture_forestry_fishing_0027_compact_net", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6aad7f33be7e973b3fef835e32a58b494a0295610d88d7cbb4057bfb097634c"),
		new("item", "industrialised_agriculture_forestry_fishing_0046_folding_shears", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a00f32c859708f14c0ce3e9fbc9497c25bc447f076a8f3e5cbd13b1621184e8"),
		new("item", "industrialised_agriculture_forestry_fishing_0060_portable_yoke", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "4c6653491c8c5af65a91aeec252aa38687fd2f21d1e274f4520b3508e3060c9d"),
		new("item", "industrialised_agriculture_forestry_fishing_0102_precision_hoe", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c236fe106886050e42e3c2bbc7ddfb7ecff5508e6bfc634337aa6cbe150935d"),
		new("item", "industrialised_agriculture_forestry_fishing_0125_lined_spade", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "e2ef67f579c9028abf210b1d0130af53ea8d974445f7cfe3ecc92597cc8bb8a4"),
		new("item", "industrialised_agriculture_forestry_fishing_0139_ventilated_basket", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "a672901c37283936f50e607335011ae165bc30170e92caa89c37552690bf0945"),
		new("item", "industrialised_agriculture_forestry_fishing_0143_stackable_rake", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "60f27f4be15478746661972cccb5624faa4fa802fc867db30c922626f4971267"),
		new("item", "industrialised_automation_access_control_0001_plain_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "446bce7b2566075ab6521f6844798dd5bb081e888ee4a647a9a95ed7a8cf851d"),
		new("item", "industrialised_automation_access_control_0011_reinforced_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "28617b71dd96fe48e46488ca21e0a77a1800a9848752523d4837d89504705fe9"),
		new("item", "industrialised_automation_access_control_0034_heavy_duty_relay", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "60ba385cc03cef3b03c4d90ac255059c795d6545038ffbecaabb5b29e95c4a5c"),
		new("item", "industrialised_automation_access_control_0047_folding_latch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e42020ec00d359bf3c972cc51b9d1bd2d3e12c9271861e865aac009b994eb699"),
		new("item", "industrialised_cleaning_personal_care_0004_plain_razor", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ae01795b81a88b5b91e7d4689851265162a092a02730d4f4dc71e5ef6bd54e0"),
		new("item", "industrialised_cleaning_personal_care_0007_plain_broom", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "4993ef3c4ba27986395716fb1de3094684e0659ca675a5d6024d0ab5e7f0f38e"),
	];
}
