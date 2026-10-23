// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch029() =>
	[
		new("item", "earlymodern_andeancolonial_clothing_full_wool_pollera", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e46cb06e88a80d45a315e1e1916edca6fa5b7f508c25f11f7dfa786ce3ae31b"),
		new("item", "earlymodern_british_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "628fd8745907d19ced387ee85b67ea78629af6934ff181cf4bd30bf1d03af964"),
		new("item", "earlymodern_caribbean_naval_officer_island_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76ed0b3f102f7f340e7862c7ee25cb2104d8d57a7a02803d7431e0843240d33a"),
		new("item", "earlymodern_caribbean_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e72cf4085ef5d3c8aeeb9390ee4cf06d38dc7455cc42a91b6eaa230d5b005641"),
		new("item", "earlymodern_colonialnorthamerica_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3f04a3fb24c965f54bc916defebb330e7b976a0ba5fa6667c67a1c12b8e2b53"),
		new("item", "earlymodern_container_bra_bottle_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "197b9f9771f07a3964394afd888a44d0de70b4fe3c2649267af19a78afc4ccb9"),
		new("item", "earlymodern_container_bra_document_case_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfaa08613b9410f781c382008bd47f064907a0ea39029c4afb62a98879932da1"),
		new("item", "earlymodern_container_dut_bottle_31", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4660b8e9ba9ab556e04148743ab2cc33a4a66ed2928b9242475d7fa0efcd5c7b"),
		new("item", "earlymodern_container_dut_travel_coffer_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "987789dc4daeb9358f4d37895fe37da99eb3c8bc694ce619b9ab6ef627026eeb"),
		new("item", "earlymodern_container_edo_ewer_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "880315e756465ee6f0a57155149e768bd66b3eb716dce4aabc624ccc6e4d5bff"),
		new("item", "earlymodern_container_edo_linen_hamper_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "19b1beb732d118d21477e736047e53456be005c973814a11b13e3e525e26ddcb"),
		new("item", "earlymodern_container_edo_purse_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a834da554855cbae7eca15631f2753b5e6919ac58d54fb99b0f17e1898d269e8"),
		new("item", "earlymodern_container_edo_spice_basket_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b82265bf97bd11e0a6a672abe569e0041c53547c06fd470dfea65d968e038b8e"),
		new("item", "earlymodern_container_eng_document_case_34", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3382455c26ff86dce79de92e7d7f595ed1eb0d805309dc0f65b10343d5b92542"),
		new("item", "earlymodern_container_eng_punch_bowl_28", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "47fbab3ab695463a5666fb2f6f2fdd5edb66274f70e65a3256e895600f5713fe"),
		new("item", "earlymodern_container_eth_linen_hamper_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb7a920765f32a51266baf79257e5de6eb2dbc8d2ce7f83ed31d41896bd3c504"),
		new("item", "earlymodern_container_ger_tea_bowl_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "201d12b525dfee9526ce943bdf942bf4df734cf2e6940ecd157df673a7733e8c"),
		new("item", "earlymodern_container_ias_sample_case_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc4cf72a4b2fabf5d003068109c60cc24d2cb46cf9d99ab9f72661d7d95f7191"),
		new("item", "earlymodern_container_ias_tea_bowl_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3e6c7e76ee53cf5d91533a3cb6af430b3b42c276e5f1360f1ec811bb1288a11"),
		new("item", "earlymodern_container_ina_ewer_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5d8f7689bc1adcc676c2b3e20edb677c6a0b37af3664e72b262537a0a0833fc"),
		new("item", "earlymodern_container_kon_ewer_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfac6883b65a02a296047e9a422e3ab697c3d143c062166ab22c328925cb4588"),
		new("item", "earlymodern_container_kon_tankard_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ca9dfe7515497960160ac9ee92052e6732e724baabfe2150c9e189835007abe"),
		new("item", "earlymodern_container_lmg_ewer_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cdced522afde3a7bb96db1cc2b46338b66cb4b6d89452669b502608030bcc9bc"),
		new("item", "earlymodern_container_lmg_lockbox_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cff30554b45aba8f9ea641fb5f723e65384f38f3a6007116a678f860108abae8"),
		new("item", "earlymodern_container_mag_ewer_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6afe408cf2174e1ce21a780c55af1ab98cf466a49cace9706eab2df9ee9122a7"),
		new("item", "earlymodern_container_mes_tankard_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "22b147cfa2a14679bf58ed19fda8690ba30926b3e45718089377b0d7ada3d64b"),
		new("item", "earlymodern_container_mug_sample_case_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ac657ce7b059472c8ec56209673621e1ea9ce5ffecbaf88846452c74b35431e"),
		new("item", "earlymodern_container_ott_purse_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "74de79e8aae6d98196ffbd75001ff1d16ef7d00ade74174996ed522552c99a81"),
		new("item", "earlymodern_container_plh_bottle_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "76c8573667bf5fe36742b96a5556cc3d0f2859fd9ac18b7b9354794d66a8ab0a"),
		new("item", "earlymodern_container_qin_punch_bowl_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "52784a0b74c07f6b57e0fca3b4776e8a74ba573d532db6b875e40095dffa4e86"),
		new("item", "earlymodern_container_rus_punch_bowl_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "113b440b5a3a77f708c6be2d4218efae6ad702e159162be3a11de3e3d7cf1f1c"),
		new("item", "earlymodern_container_rus_tea_pot_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9176bcc0a7a7c9db8595fde9374fde5643aefd28ec538999c0f919d4ebeaef8a"),
		new("item", "earlymodern_container_sah_purse_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "97486219ef2a99b68646a02ea7da1115ddf32a9ad3bc9bad0a9b4d7fbea9377a"),
		new("item", "earlymodern_container_sah_spice_basket_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa320f9d257cf75d3ade53a6671d0d38e1be347c5d1031e73b697905a77a357d"),
		new("item", "earlymodern_container_sic_coffee_pot_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "024f21d29f2ccf902fc0bd5b2b5ffce2ce036ea1f6873eae64369bd31ca9e51c"),
		new("item", "earlymodern_container_swa_coffee_pot_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "17b876c2cc6b97948a2eba6438d3a93e4c163a59550929c0f6be200ac9c2db3a"),
		new("item", "earlymodern_container_swa_sample_case_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4630575abed489f1a9d5f89800f4809e6d5b85da65540d492402bcebb090b3a2"),
		new("item", "earlymodern_container_swa_tankard_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfeab180165b6da055afef499a882c01639684fa8c6bf4eaa1ca7dc24b3399de"),
		new("item", "earlymodern_container_waf_spice_basket_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0297bcec0937eef95d0e62a8392c8475af41e80de081c29c4f115e400579dbe9"),
		new("item", "earlymodern_door_and_escutcheon_642", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c8907af94b5009ee850d20127b93b82a33a47d1771846860c7514242a1f7e98"),
	];
}
