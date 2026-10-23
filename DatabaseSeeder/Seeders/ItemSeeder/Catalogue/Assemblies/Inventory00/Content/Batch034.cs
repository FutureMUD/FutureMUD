// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch034() =>
	[
		new("item", "earlymodern_military_ranged_straight_throwing_knife_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc91517aaa50ef811e6f6e37d08a76f82930a5e82df0849c1c3ada190f2e67d3"),
		new("item", "earlymodern_military_ranged_windlass_crossbow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d561847634d4c8220b0187f79326ff7dd1c83603577ff8921af72053d80336c"),
		new("item", "earlymodern_military_shield_studded_targe_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4046dfbbff61b89fad4d84fd26bb6f4eeedd3e0b34919d01d767ff3924bc7d01"),
		new("item", "earlymodern_military_training_practice_glaive_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfa65725ba5bc06fc9122cc13071f5432ad1b26d8166c71e33029e9e0d70d303"),
		new("item", "earlymodern_military_training_practice_smallsword_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e33f1c15f6c6e66170cac8bfa2cd158c6f54a597f6611b3c9b1435e357537c1"),
		new("item", "earlymodern_ming_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b68c9307c1c09e641aaadf229837b2e598450d1d4b190f8a7348e1139e54bae4"),
		new("item", "earlymodern_mughal_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "17fdded923dd0fc0cf15b98c58c857ff9a31e4b6bca0aece24225dcaad8bd1f1"),
		new("item", "earlymodern_noble_jewellery_gold_signet_ring", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "22bd96dee6861743641963fafe4eb0c13bccc4cf7942ed03aa3abeb6dd0c3a74"),
		new("item", "earlymodern_noble_shared_dress_gloves", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "71c88a3643029a36c072779f8059a268da12057b44c94d08f3e388c33025a618"),
		new("item", "earlymodern_noble_shared_high_riding_boots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3702713a7f24e9d4475887d785344e5c72ca3a9359db5e68c726c3df18ce4512"),
		new("item", "earlymodern_ottoman_military_tall_felt_service_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2ee998ce8b7a858dc0faf5fedf3bfb0968e6e9a26e592766176605d45fe9959"),
		new("item", "earlymodern_ottoman_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0f386014ea5b58dae3e1470a23b47e99edc3034dbf743485cec216ca05fa800"),
		new("item", "earlymodern_qing_clothing_sidefastened_womens_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75b6f954730434f229a27e9e888bbea2df64d0c47f230a45801e854caa6daf8c"),
		new("item", "earlymodern_qing_clothing_work_apron", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7668f49d9997c45b131d44450d0e97b0a0af9edc28b8af7d01c7d812baadc228"),
		new("item", "earlymodern_qing_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c98ec424a11fa6331ba5a58aa1a45a795b4ce62adaafe672e9f48780754af4c"),
		new("item", "earlymodern_religious_local_woven_headband", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f7c7dd38ce1dfd213855b2bb151dba6e5b219018e75b153760b05c7cb3b0d77"),
		new("item", "earlymodern_religious_zoroastrian_padan", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3f109131c5aca088e882e7c124993015a68f57805c71443cd50bdef5478662b"),
		new("item", "earlymodern_religious_zoroastrian_priest_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49b5a130c11f71a32aa03ab9df44d6e0c5d4347c309b83b6c6d61e1ba4e6e1f5"),
		new("item", "earlymodern_russian_clothing_checked_wool_wrap_skirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c425d49d418196b308680ab103260d71b0669b5e715dacb008eb6a9f35f42bab"),
		new("item", "earlymodern_russian_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "21ee777f4b4e4310525547ec09d527ad0f128b8b8884c84a0d4bff9c70b83ac4"),
		new("item", "earlymodern_science_compound_microscope", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8af89004f7dd8d5adcac05864dd0942534ea8e5dfbe2304026599a3acb953a2b"),
		new("item", "earlymodern_science_pocket_watch", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "55fffe6adcade2263ead21ffc4f90329a68891c8e1a7bb47af27ded8d685e605"),
		new("item", "earlymodern_southasian_clothing_light_cotton_shawl", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9bc50f01831470be0ed7773be1805f3368ab3e8e808e6539ad79cf51824196e"),
		new("item", "earlymodern_southindian_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "27b41deaee0a0f0e296835e7b5d81cdb01e7e117d5565e09dfbfe3f0cfc273f2"),
		new("item", "earlymodern_swahili_naval_officer_command_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7513e99c590832f3762b7b9670e2296b4dcc558543cf7bc27536480081e0576"),
		new("item", "earlymodern_swahili_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9898322025d029f1b8b5d3a3b3b43a7dbade74b4d489394ba054f5d98ef2119d"),
		new("item", "earlymodern_western_clothing_knitted_wool_stockings", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eda7b01cf80277c4f507eac2c0a76110cc2764c0880497ac1578345c394c60d0"),
		new("item", "earlymodern_western_military_split_skirt_riding_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f79e96d416e1cf4cebe4b35c3b078c2d78802d36054f603be5d461700b28138c"),
		new("item", "earlymodern_writing_inkstand", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f053df790997438cf3cded3dd5a7700bd2fa67101366bd2f4517fb9ce2798c7"),
		new("item", "industrial_clothing_footwear_uniforms_0024_reinforced_gloves", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1770dce4e5669d3331348d24e7f3d49ee161394e64e788bddc997af1efdfc3c"),
		new("item", "industrial_clothing_footwear_uniforms_0031_compact_rag", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c576a97ba446de0c19731c8c94b0876a448beaa5d213273e60c5d754ce3897c"),
		new("item", "industrial_clothing_footwear_uniforms_0047_heavy_duty_apron", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "66ca916ae65ce4b11cf38701b69c2e71916954b36b956f698b735b5528621176"),
		new("item", "industrial_clothing_footwear_uniforms_0055_heavy_duty_scarf", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "45bbcea35ffee561f504eaca645ee62cb06473d14e2e13ac2437f2e3bf5b0b6d"),
		new("item", "industrial_construction_utilities_0026_heavy_duty_fitting", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4899ce3450d2226487d53dc8dd7bf60e8832a72111a6ae054c62903b0e20391"),
		new("item", "industrial_construction_utilities_0030_heavy_duty_conduit", "industrial", ["industrial"], ["item:industrial_construction_utilities_0001_reinforced_rubble"], ItemSeederOwnershipPolicy.RequiredRelationship, "6742c79a986f85615f26c9990e63cb8fa20b103f2a5bc45bcbf1cb2273812fd7"),
		new("item", "industrial_electrical_power_lighting_0002_heavy_duty_lamp", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "64f66ce677c7c692296183f69ba60c961dec6a2be60085c2bd4c73f851a9165f"),
		new("item", "industrial_electrical_power_lighting_0027_portable_lead", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "be6ebfd25823b666d231c8a68f1b3657bee3af38bb634bd9bde8dfdcd1faedf9"),
		new("item", "industrial_emergency_rescue_civic_0007_ventilated_bucket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "310ecd4c073aca0d21dfb8185c302bf671d19b402efa325e64f3d7766183fd67"),
		new("item", "industrial_emergency_rescue_civic_0010_ventilated_stretcher", "industrial", ["industrial"], ["item:industrial_emergency_rescue_civic_0001_lined_helmet"], ItemSeederOwnershipPolicy.RequiredRelationship, "642bdc03680973d21581b0e4087c9a6bda1f566cd53abe2de9ce0b5507d2d510"),
		new("item", "industrial_food_drink_packaged_0007_stackable_pickle", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "37a904eb29c579edd3d92a4518509f0468e77d3e00bfe86a0df4d79447b5d304"),
	];
}
