// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch035() =>
	[
		new("item", "earlymodern_joseon_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2fa1a254b65cc3c6ded0ef33609c08e0e3c06111e702c50b38a77b38b1f5639"),
		new("item", "earlymodern_kongo_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c65a3c2487fb2e21e65aa0cff1d2aafdfd0dd85a8df834d1bcde249054bd58b"),
		new("item", "earlymodern_kongo_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f8ec389ec9583fa79e477ad8a5b51f40e221ed238d6fb2e539946ce330a05fe"),
		new("item", "earlymodern_mainlandseasia_military_command_vest", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7253bfa17360a12ea86d11c8a1f3efdee488db3bd1d6f93eafa06aaec1a344a7"),
		new("item", "earlymodern_mainlandseasia_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f854d16c56174fef5d4d5b131b264ae83334173d899f47290b61eb9debf5c2a1"),
		new("item", "earlymodern_maritimeseasia_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db318a83be85ac1d5e0449249e10ae50fd6384b9dcb7c588cf8ca53f72bd4493"),
		new("item", "earlymodern_maritimeseasia_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a6471f71ad7a6b6e7cb60211d16e13052bc25bf3948c8ba8c031153014b3dbe"),
		new("item", "earlymodern_medical_filter_funnel", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "80460b67fc9948e41a032dff22767903c1a7e580f730070b4ac81febec25647e"),
		new("item", "earlymodern_mesoamerican_military_militia_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9454ad8a102a050119a260438247e001f969aebdd5ddaec299c8b488c32f5e6"),
		new("item", "earlymodern_military_accessory_breastplate_carrying_case_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb8254d1a2c6df555d17587569df79154aff9641021f510b95c1b99ed0847452"),
		new("item", "earlymodern_military_accessory_campaign_bread_bag_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dd49e672e0a7cca13f60a0a158d171c517cf08085890db589556d695df376dd"),
		new("item", "earlymodern_military_accessory_cuirass_hanger_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "97f9f323b70d2e2662215d6c1e834f1596f70969675d63341b93847741afacd0"),
		new("item", "earlymodern_military_accessory_cutlass_scabbard_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "57fed2c0c97f5c12bd0374ae8ba09da229d65213efb5b68647ba61f549a30d7e"),
		new("item", "earlymodern_military_accessory_gauntlet_storage_box_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2958ecd62d04c64e3ccef9426ff94af112d669805a879d0048448ac1d7af87a"),
		new("item", "earlymodern_military_accessory_match_cord_pouch_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "11c3a1209deac99f7c8f766930b0f7a85c29c12d7980794daf2b02522c4ccc0b"),
		new("item", "earlymodern_military_accessory_military_fife_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f15344bc5147a0b89dc3794b94fbce9b7ae6ebd984e7a95a5df964eb1b91d9a"),
		new("item", "earlymodern_military_accessory_officers_field_flask_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eda99884c68a311ccb63f67a74d2d924e8e82b910a14cd2a38f0328c80f8dec4"),
		new("item", "earlymodern_military_accessory_paired_saddle_holsters_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "777c0c9d56f49256adb59613107704dfa328e123f52734fe8837981f696f17c5"),
		new("item", "earlymodern_military_accessory_rapier_scabbard_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0dc1b4b8bd38f93e854bfd179ac4803912bbc813f6d87e5940e5d5f5b1ea7ea"),
		new("item", "earlymodern_military_accessory_repeating_crossbow_bolt_box_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1273d5ef9dd563937833a8f525540c0e877d8e4feefc92e28b29c91039fa13c5"),
		new("item", "earlymodern_military_accessory_sabre_scabbard_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b98d1b61c46509714c24e44300e37696f39bd2af2aaaeffc4ec9ac351eab18e9"),
		new("item", "earlymodern_military_accessory_wooden_cartridge_box_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9ac0390a9cc59d7276f7e6a3428e8a1e033353157e3c54c4214fde0a2effc4a"),
		new("item", "earlymodern_military_accessory_wooden_field_canteen_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce0d776e36c7c9b45092f5976e9856000849fe2a4bd0372737779628d24c3e3c"),
		new("item", "earlymodern_military_ammunition_light_buckshot_charge", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6dcf6f3c672fc2af34e192aabdb36c3cf6d15370a11072dd763051dcd03904c"),
		new("item", "earlymodern_military_ammunition_paper_cartridge_080", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a783f2661320b25879039f4354b7a9fb26173ea2f56ffeac6c90cda7429e1588"),
		new("item", "earlymodern_military_ammunition_round_ball_055", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd8e6c61229b43c2628734dde18a6dc4afdcc7ac7a80e5a7d0f1decd3ef70157"),
		new("item", "earlymodern_military_ammunition_selected_sling_stone", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbcf4be83afd5aff5a18a9289e86080f5e7d0b03b081e7bb8b63e34cd365447a"),
		new("item", "earlymodern_military_armour_brigandine_vest_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac3544135a5d4ec9f03d4db2118b14b4c440e83b6581f8cb3352563684f8eacb"),
		new("item", "earlymodern_military_armour_conical_banner_helmet_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a5a9087667e3090f88e7d7a12c8bef0e4641f0859e476b6d6e470678748685a"),
		new("item", "earlymodern_military_armour_cotton_quilted_coat_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc9834dd7df82ba2f2445c1d14d8668bffbb8bf40578acb560efbfb355903405"),
		new("item", "earlymodern_military_armour_cuirassiers_closed_helmet_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6c62f09b2d744fdd42b083fd941d190a6f43e13ce73d1298e89ddcb891d1921"),
		new("item", "earlymodern_military_armour_cuirassiers_cuirass_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f263d9e5bb0253a7b4353ffc16d76519b547fdea203b32d5eb19b480cd45c5fa"),
		new("item", "earlymodern_military_armour_felt_war_coat_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0dc675a698dcceacbfae21d6af2f2534a464a5a8f71b987c63ae28f2d489b549"),
		new("item", "earlymodern_military_armour_full_plate_leg_harness_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b881a47b7308190b6d1bb4e94f1be3392d0254c113d8b8ba8f63c008f40b620"),
		new("item", "earlymodern_military_armour_kulah_khud_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "91499dbee18910d155d86bcfd508d5f9d85938b78ae9224a23cdba80ba74c764"),
		new("item", "earlymodern_military_armour_lamellar_chanfron_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "286115e2fbf34d6e0e495167c424feff537f458da245472d57a0faa3431b258e"),
		new("item", "earlymodern_military_armour_lance_rest_breastplate_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f55634518a6b1b6d6296db87dddb068850c4d06d55e6d42a7d286ddb4aa5643"),
		new("item", "earlymodern_military_armour_leather_bracers_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "010d223ebc5f71a8661c0132cc6fcf88cba41e0986723dfbc85a799717ec945c"),
		new("item", "earlymodern_military_armour_leather_cuisses_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5979574d3b1504b1a19e49535a87e73f135207daf95c607be58fbf3001f62c5"),
		new("item", "earlymodern_military_armour_leather_tassets_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "399f282c804b6b2095edb02ad2b0d03de72126034006f07cbb26de921b7a0fb1"),
	];
}
