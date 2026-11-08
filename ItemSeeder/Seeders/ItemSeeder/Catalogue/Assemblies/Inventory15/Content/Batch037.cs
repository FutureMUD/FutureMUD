// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory15Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "industrial_food_drink_packaged_0023_countertop_crumbs", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "29aa2eb260dba9a3f8a38424b7e359a815b118429e3e3fd20bc8b5ae594a98cf"),
		new("item", "industrial_kitchen_appliances_0003_institutional_pot", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "0fb07bfbe0c8c69233946419035ca369317c4312198a5254ae4160ffca602f07"),
		new("item", "industrial_kitchen_appliances_0018_weatherproof_ladle", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "648ad9e8767c295b30af1d1083902b356df6510659936499c9dfda9fc91cd84e"),
		new("item", "industrial_logistics_retail_hospitality_0002_compact_crate", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcc4674e8acc2ce010190098f5e620ad89b47413b8fdd6c315ea51841bc5611b"),
		new("item", "industrial_medical_mobility_0010_service_brace", "industrial", ["industrial"], ["item:industrial_medical_mobility_0001_service_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "3037b4a7ba3425ecf00f3dc3f905d2bf865956391ad7d7685a59532f32c4cbd0"),
		new("item", "industrial_medical_mobility_0020_precision_brace", "industrial", ["industrial"], ["item:industrial_medical_mobility_0001_service_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "fd477f2a6ef721c13d26b641cf471f1bd3308525416290e7a9420400dd0a43f6"),
		new("item", "industrial_office_school_mail_payment_0031_plain_satchel", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "2053b5e3315f02998e3616ab2a1d70e4ccdff268fcaf2c41467341f35055a2b4"),
		new("item", "industrial_office_school_mail_payment_0035_reinforced_notebook", "industrial", ["industrial"], ["item:industrial_office_school_mail_payment_0001_travel_folder"], ItemSeederOwnershipPolicy.RequiredRelationship, "5b0a541a6a989bcf95acafb83cb7ab3118dfeda86249347b0a6f38faf8ca0c06"),
		new("item", "industrial_ppe_ballistic_0001_field_liner", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b904db4253e43a1e44e6398192b985159da07f58331ad871782cd9356c127f8"),
		new("item", "industrial_ppe_ballistic_0014_countertop_boots", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d93703d6686035a458d3a2ec03ad88e273c30f139ecaf5a5adf08ce013df00a2"),
		new("item", "industrial_ppe_ballistic_0017_countertop_mask", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3c02988b90e681fbf0e7e3f8518b91eec4f93e42788e2706f87b9c7402e958e"),
		new("item", "industrial_ppe_ballistic_0019_countertop_vest", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a63c76b608666936ddb87ee4926085a19175da7c4510593495234baeb4aac2a"),
		new("item", "industrial_ppe_ballistic_0025_wall_mounted_helmet", "industrial", ["industrial"], ["item:industrial_ppe_ballistic_0001_field_liner"], ItemSeederOwnershipPolicy.RequiredRelationship, "43a7ba1964de1844fd073a004643cf05057f45249c35a1b020aba4dea690abd2"),
		new("item", "industrial_printed_media_signage_0008_lined_notice", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1ed0dc0e8676b8b454758e03dc19d263be1ee15beb7a9e401b0647f3552b6d7"),
		new("item", "industrial_science_education_0006_stackable_model", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f1c9b3ca73386ab056a7c15c3a3e41be46386c7c71729b7027bbdb88eee0fd7"),
		new("item", "industrial_science_education_0009_stackable_balance", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae28c9500552a4817f0ea0246a87b1f3a39ace2ee780fff7969f52a8ffcdd102"),
		new("item", "industrial_telecom_media_photography_0005_compact_tripod", "industrial", ["industrial"], ["item:industrial_telecom_media_photography_0001_compact_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "5e156471060018f5fea9d788b6a07ad5ef5cc99653650682621ce47dca2f79f0"),
		new("item", "industrial_telecom_media_photography_0007_compact_recorder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a186554c2d73a581a66e6c40dd0ce31ac15d356238f5dd2aad8c38cccbf677aa"),
		new("item", "industrial_telecom_media_photography_0009_compact_microphone", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "064c5147149b82c8ad28167a0e5dbb4506c4666aee67bbeed0640146c72c741c"),
		new("item", "industrial_telecom_media_photography_0016_heavy_duty_speaker", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c0ec0c761c207722ce2c01b236051c9d4090ead9c4b12dde3816b133050b787"),
		new("item", "industrial_telecom_media_photography_0024_folding_camera", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e310462493d03a347dbc767618783560dd74c10049ace735cb86ad25dde4d27"),
		new("item", "industrial_telecom_media_photography_0026_folding_speaker", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dad08b29c53f504ed0cae2e42e9837ce2022ae5641ade1efd9917a35c1ae036"),
		new("item", "industrial_tools_repair_machinery_0007_ventilated_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f981050b27a124b91de539d7a6d1502272989d3034936cec681dd37a99b264bf"),
		new("item", "industrial_tools_repair_machinery_0014_ventilated_saw", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8ff6eb90880fb0161b53ab59f706250173e7ee9dee2b48dbe4b1ed60a578c0a"),
		new("item", "industrial_tools_repair_machinery_0018_ventilated_grinder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8d665126fc61d3c1eadaef6420623d1f11fb06cf6f6845004d1b2b29083cd5d"),
		new("item", "industrial_tools_repair_machinery_0021_stackable_wrench", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "263bf12516f45d64233c73cea567da4c28ec172b034e2ad84224c7324861b556"),
		new("item", "industrial_tools_repair_machinery_0027_stackable_press", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "24142df863d062ca63e85fad13f28ee34fa935e76db2a65594ae5b27719b28a0"),
		new("item", "industrial_tools_repair_machinery_0037_field_plane", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5b2a95b0fbdaaf711beb4c750ae28b82dd2b4d2f6144bde01c4538791d33975"),
		new("item", "industrial_tools_repair_machinery_0046_countertop_drill", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2455bdcf799c0f6889064799c30f672003151ab5d59584384274e81d3cb90cb"),
		new("item", "industrial_transport_support_spares_0024_heavy_duty_bearing", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7773c084f8d55f512afd7a2521b7c5d2a2c2bed7110e20686cf289b92623155b"),
		new("item", "industrial_transport_support_spares_0039_folding_chain", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "be1e7688ef3a83b867c01dab37ff652e8395137aff3cd15b7f8a95d8658c718e"),
		new("item", "industrial_weapons_police_military_0010_institutional_signal", "industrial", ["industrial"], ["item:industrial_weapons_police_military_0001_institutional_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "b40a8fa53c4676c7524fb64d09e04a41e270ec0f89d69789d800f3bb94735eb0"),
		new("item", "industrial_weapons_police_military_0013_weatherproof_baton", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6d20ba5a45b63fcf3509e9b9276e2b80c3a782edb613cbd5202ef1ef055d783"),
		new("item", "industrial_weapons_police_military_0022_service_holster", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e6e9dd25b27ea76b3d78286bbb3317d5695ca58733a32323d457328104f4abd"),
		new("item", "industrialised_agriculture_forestry_fishing_0004_plain_fork", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "257c9455faddd94b48e110e3ab008b0c508a75af4199e77f0da2ea205ed7de63"),
		new("item", "industrialised_agriculture_forestry_fishing_0005_plain_spade", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "a6d60f110c94b1fbd46642bb70f66d50ca962c79ac51ba701f20e4728444e167"),
		new("item", "industrialised_agriculture_forestry_fishing_0050_folding_yoke", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "faba20172889ffb9f98e61ce6d7b63984e4d332916b1d544e61c486da45844c9"),
		new("item", "industrialised_agriculture_forestry_fishing_0054_portable_fork", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba52e49e952887ea31e721cd994208811734935ffc64f1e6acc5a9dccc861b74"),
		new("item", "industrialised_agriculture_forestry_fishing_0070_workshop_yoke", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "9f61affd53ddcce19f57b61ba8d34f6cfb224713250a33f8a8d89f30efe2ba81"),
		new("item", "industrialised_agriculture_forestry_fishing_0075_institutional_spade", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "afd9c35eb758f4584814c97966df62ab23963a7b7dcaf09a0102177739c2dd19"),
	];
}
