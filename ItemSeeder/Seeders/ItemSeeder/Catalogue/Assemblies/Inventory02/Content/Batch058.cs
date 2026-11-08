// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch058() =>
	[
		new("item", "primary_production_mining_gad", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a63623f429a26d3d5f30e5b23e3861f8c4a0bad41bb1e5fb443673b7840b4b6b"),
		new("item", "primary_production_ore_sack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "080b0bc46605d131a67e18ea478ae09a09c3f4e26b7a9c71bbc996357679bbe0"),
		new("item", "primary_production_prospecting_hammer", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8a1a6cb7d98fa00f9d077ae3f8466285219bf19bc2e0af1a43fe30767d51c34"),
		new("item", "primary_production_shell_lime_deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0c7710a7df8577445c4da6ed1543b48e8ba2fa70ca70aaee7c9afa175d4878a"),
		new("item", "primary_production_timber_prop", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a637398eef82484114af66521af6f2e337fa34028af0051ea87d6bafb16e90c"),
		new("item", "renaissance_africancourt_fitted_shortjacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "535032065b735e507963c901e0b7e850f4cdc156efd6313bfc947f0db2667339"),
		new("item", "renaissance_africancourt_raffia_brimhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b83f8b486855480dd279bc3725f417c8c0c9e7b93484bcfa4ac3fb521a446510"),
		new("item", "renaissance_africancourt_soft_embroideredcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "84c4e3a278d5a1a6c230f7e37e0296709ce8159a728932c37b92aaf0f4b98448"),
		new("item", "renaissance_andean_short_worktunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f873fac4c98a4d2788c396ef2035c56dd5afbde55dde8a43b84349dbcc02d57c"),
		new("item", "renaissance_art_drawing_board", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b471b79682c0c49bd58006f54b7531c44de0baece35c8926c37ee4f0675c0661"),
		new("item", "renaissance_art_pigment_shell", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "587f67e4fbdb7b97d34acf06b3e6290c416535a886cbef529510ba76140808f1"),
		new("item", "renaissance_container_aca_domestic_utensil_case_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "885f554d5f5a1e22be8e141baa04e94416560f328a6c9cc9f684f4d7fcf74e85"),
		new("item", "renaissance_container_aca_trade_small_trade_keg_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cfa2943cfcb99ac0d5e6b0f779d2fbd988312ef781c9f19d0d38d1bdfaf9c16"),
		new("item", "renaissance_container_car_personal_shoulder_bag_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8e7fcab3911c0b528e246b810cae5e3eeb8229f32be5adf251745afc325743a"),
		new("item", "renaissance_container_car_trade_deep_trade_hamper_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a68d32c50319a0911f421c2f483e01f5840fa2ef06779c0c6876a857e430bff"),
		new("item", "renaissance_container_col_personal_shoulder_bag_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6100b5e0fc124fc683c6ce211428fdd26dbb4b401e40970fda1bba92f055600c"),
		new("item", "renaissance_container_eal_domestic_serving_tray_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "991243515d0eaa8b4539217c171373ef787c08a3266f4421bcb84e598dade704"),
		new("item", "renaissance_container_eon_personal_document_wallet_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f78a4e6dcc6d14be48a6e2931e6eb1c458a5462ab5a19d28f87786199dc80f70"),
		new("item", "renaissance_container_iba_personal_shoulder_basket_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "92df504ac9694a119b2a88423480e2789688363dc5384709ac67451035981cb3"),
		new("item", "renaissance_container_ind_domestic_serving_tray_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "303cfa8bd5b476d68b0a828fc672dddc15133b693ac2bb12f6c38af0700c2fc3"),
		new("item", "renaissance_container_jpn_personal_shoulder_satchel_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9600011af95f8152e0bcd4aac53bacf89ae22760987c2f1af0995461d867f6f0"),
		new("item", "renaissance_container_mar_trade_sealed_transport_jar_08", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef864a2b2ca680f8920a54033cdfac105ce3637235a634ef266258f98cef63b0"),
		new("item", "renaissance_container_mes_liquid_washing_basin_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8f2502d747a02c760325f1cb8ccb1044c5751884030a699168b4242623ccb05"),
		new("item", "renaissance_container_msea_domestic_utensil_case_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cc5cf4d6fd6d6a56fb8c698ace0f708b6f3da4cf6a40d8de285ebeed6d19bdc"),
		new("item", "renaissance_container_nba_trade_strong_trade_chest_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c61bb75fb503590ef6079e60330b65ee7ff7c0a66c9b2308835740a87d209bce"),
		new("item", "renaissance_container_ott_trade_segmented_packing_crate_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3caf78f199c704e80f476a5e1e41585cd342ce81410d39c127aec1ce03a1608d"),
		new("item", "renaissance_container_pip_domestic_condiment_box_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df4f23beae1a8d57c90b7333d7be2a8259b7fc2cfd057e15394476882e8f543f"),
		new("item", "renaissance_container_sas_domestic_household_platter_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc1025fc7d7685992853fce3dfa8817e458646d1cf79455481ad4382e9f24d16"),
		new("item", "renaissance_container_sas_liquid_great_storage_vat_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfa2b2b0e73ce3ba5d378841fcfeda1e8fbe984dce5ed3865f68e86ae1e93b57"),
		new("item", "renaissance_container_wer_trade_sealable_archive_coffer_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "305a8453b3e775640c3ed817a1edc61f85ce346411ee2d8725c4a23b1b5fb94c"),
		new("item", "renaissance_domestic_apothecary_perfumer_instrument_tray", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e33294caeb99a3e9702bdf99663394ed752217ea5689ebe3b1bbcb5f4cb03ac1"),
		new("item", "renaissance_domestic_civic_guild_serving_board", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66198393a778b6ff11502328e8384df8ef5b9bddeff745187e755ca61fbd7045"),
		new("item", "renaissance_domestic_elite_household_silver_charger", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e70865d5fdf0360937514b7ae54a2111e0a080fdbda5bb167792813cc259222e"),
		new("item", "renaissance_domestic_ottoman_bazaar_iznik_tankard", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b0857c7df909042af20bdedec4e2d23960f7abb95384981958bf33292ddc9db"),
		new("item", "renaissance_domestic_southeast_asian_port_lacquer_bowl", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca3ebd4341454b37600a181416fcf4f52fb0678af11119e71bbdf2a966981e67"),
		new("item", "renaissance_domestic_warehouse_dock_salt_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "133528972cfdb8a73478dd05d4c5e761fb0899e43b466db3e935045201abde96"),
		new("item", "renaissance_door_and_gate_dropbar_582", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb7f06b98c403a5e1cc453d38a935d45645a2ea24e8c04e8954f865754a14cd1"),
		new("item", "renaissance_door_and_hatch_573", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5eaf699ad50eb0e66212089116875cd8aebb7d8601a5d33b7a4fb60257365565"),
		new("item", "renaissance_door_and_portcullis_pawl_584", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e76e533f2f377820c709acf5d07eb36879af7b9d9a244b06236d982107f9ffa7"),
		new("item", "renaissance_door_and_secure_door_564", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "173a36b7a8408c7ca779c7ee5534294dd49e31f0ffb1297754a702a91a50e638"),
	];
}
