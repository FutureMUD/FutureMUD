// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch060() =>
	[
		new("item", "renaissance_andean_fourcorner_wovencap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "28583728b5173456e60e274e3aa91619d7f3594fd0866ac005d19661de5f1c6f"),
		new("item", "renaissance_contact_tradecloth_panelskirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "951d73c0d999007f3c4e14bcd4a4315d4babbcf0980d551755d2b7ec51fe11c1"),
		new("item", "renaissance_container_and_liquid_pouring_ewer_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0e54b478cfe346ef4fd5211ec5405420f7204eaad46d1b3675e6b2b77a528db"),
		new("item", "renaissance_container_and_personal_document_wallet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "20d0b6fc427c120e5964ef89962a5c1ead0290430ecc16d4b0114e99ef268948"),
		new("item", "renaissance_container_and_trade_large_transport_jar_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2630071dec0a39a5a9dbe3691839e91627878ce05de7e8d3d00d2dc550044f6"),
		new("item", "renaissance_container_car_liquid_medium_storage_jar_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d07f942b9ffce76672d7ce6f0c44b8ec8028c744460c81eace664a566c7ae4f"),
		new("item", "renaissance_container_cen_trade_deep_trade_hamper_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "256bf85a901b413d991b64ded187c7603248c993ca643793f938d5cbface0760"),
		new("item", "renaissance_container_col_trade_marked_commodity_sack_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e85dd7e084f26b16255735379162ee867f56e62713400f01cc5e74e7e19081f"),
		new("item", "renaissance_container_eal_personal_shoulder_basket_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40e2daf80d798c40c7f48bd7547b573747af1e8b41673a8eccc35a759b9a4944"),
		new("item", "renaissance_container_ind_personal_waterskin_sling_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2a8e7658d0367ea084d2e621c0886e6320dd0f5c6a9b17dc3dcbf3b1b4b9f16"),
		new("item", "renaissance_container_mar_trade_fitted_sample_case_06", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "08b57dec1f54c1864bcbc3e7155ee29717564aa6e22d53528068201b81d7c214"),
		new("item", "renaissance_container_mea_liquid_great_storage_vat_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5b2dc9f78b143011f871c924cd7cd80c1d1186c11e366142b727d160d3bcdaf"),
		new("item", "renaissance_container_nac_trade_segmented_packing_crate_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bad85256e2983e538c8816a24dcafa28f03b8aa3c7c9000e96d4f8d54238dc58"),
		new("item", "renaissance_container_ott_liquid_medium_storage_jar_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df7f2fd578269a954e1b3112ddb85ed86b7fb965d364f4c22ab6c228a3dbdc35"),
		new("item", "renaissance_container_pip_personal_waist_purse_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ab42b4b100a1ac6b5ca07e35a50ae7e39fad3ed611b426d89b8342fe925da1a"),
		new("item", "renaissance_container_sas_liquid_small_footed_cup_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e72ab2bb0d4f58b6f333cb41b695224cc2ac393d214fac9cf8ac9faeb663936b"),
		new("item", "renaissance_container_stp_personal_shoulder_basket_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab6bf6ff43f37188e907759649e77fbe33f5f9d6897fe771f6cb84835ae40566"),
		new("item", "renaissance_container_stp_trade_locking_trade_coffer_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "adec74f143686382ae8e72a4e27f05abc330c65671f8ba3060cfb50ad1b217d8"),
		new("item", "renaissance_domestic_andean_storehouse_flared_drinking_cup", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ca963ce6c74e805d4f786a8a686f21ffaf97d0d2021d7c0b769a10f17bbf9a3"),
		new("item", "renaissance_domestic_east_asian_literati_stem_cup", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c16e6e2e1fa80efb3526220d856c1a1ebc152a531c88427e12065a50532a81d"),
		new("item", "renaissance_domestic_elite_household_inlaid_tray", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8459fa7f15c1338594c3632bf76b3c532ff6014c54f8b0a01cd84076773c40f4"),
		new("item", "renaissance_domestic_japanese_ryukyuan_serving_dish", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "28014a288aecea109bf2dede1e682877f6de97e4a4d9259f42f66a9688ac433f"),
		new("item", "renaissance_domestic_ottoman_bazaar_storage_jar", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "de31b1bafd0b3daf4df70ee911702bf101a5537fbd06cf17184e773d400a5dd5"),
		new("item", "renaissance_domestic_steppe_caravan_serving_tray", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0160ce658ba28fd3e46eccb35b3c6965c2b448a3240bf0c0ffcc802b66183cb4"),
		new("item", "renaissance_domestic_warehouse_dock_dockside_mug", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0237fd5bc4de437f17a4685d586e693176d3d9a1cf874c6dcceec64d5493204c"),
		new("item", "renaissance_domestic_west_african_court_wooden_platter", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0895d375e747f02880479c74e78a7fda18a0453b73c03adee84e85b2168b3c82"),
		new("item", "renaissance_door_aca_braced_door_436", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3ea6f0d1c3ab93dab050e6ed1215c6167bbe8d2d1652c4ad9af8c8b7006ca75"),
		new("item", "renaissance_door_aca_grille_431", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3e7bba8d862943cf9061e00433bfa97f7729122ac4cf004ba0e93d1a744ba47"),
		new("item", "renaissance_door_aca_hasp_445", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf29fb8de671b5ca71af1d293667d6d7683a04346b51dca943f571680360dc63"),
		new("item", "renaissance_door_and_lockable_door_577", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ad69680a4fc9b13ed2026063c506204316cd615ed90a3a4f8afcdd37a64c707"),
		new("item", "renaissance_door_and_warehouse_gate_569", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b88b8bde8a89027b07a2cd823117840aa5642928089da3cdbc97610bbe2a785f"),
		new("item", "renaissance_door_car_door_latch_609", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b4a9cb924a5aeb0b3fd9167189d0d92c34c2b501084d040bc9c95e19f69a6d3"),
		new("item", "renaissance_door_car_panel_door_603", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c99d55a517d97b2c40838e63d39807266c00fc4488d0731fa91d7185ce89f79"),
		new("item", "renaissance_door_cef_barrier_124", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a498fa254b7bad8d32f2d23d299340c33328a13765fff0b2525e89e55d6b6b53"),
		new("item", "renaissance_door_cef_door_latch_133", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36fab3539a0437a2653aee0ad8b9ee1b080c8368c68c3fad2d8abda7a68eb316"),
		new("item", "renaissance_door_cen_hook_latch_82", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dda41ba654d5e142356106e6a14b66df91e644685772d0f64626f0d8ab5aeeb4"),
		new("item", "renaissance_door_cen_shutter_61", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c07918a14e92797031931b09b7ac7cc4fa9e40f6bc320a20d03becbea15a254"),
		new("item", "renaissance_door_col_braced_door_646", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "27abbd2e913ad7026cf551f75b1ac548f0f33d57de505971d56004429a0e7ff3"),
		new("item", "renaissance_door_col_wicket_658", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d4b8fa559c3ab281e2263caacee755c1f076d07f8268a928b3204e778a8ff5b"),
		new("item", "renaissance_door_common_barrier_82", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81d81ee244ea95df3bb09ef519f5ca40c47ce6cb54cff89e0a584474cc0ff23c"),
	];
}
