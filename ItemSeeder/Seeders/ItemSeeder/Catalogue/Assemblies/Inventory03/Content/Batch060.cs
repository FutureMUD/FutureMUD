// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch060() =>
	[
		new("item", "renaissance_frontier_muscovite_long_caftan", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec0c11428edf3c1c1d311e11cc5781bc298fa3a0fa96689c70f55b7fa1989d22"),
		new("item", "renaissance_furniture_aca_glass_front_cabinet_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "172b4cdbcd13ee5df68848472e838ebef03680d28b81a64b986110d65b901af3"),
		new("item", "renaissance_furniture_and_freestanding_weapon_rack_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e50c353d103ad80674ad8fba18633b9900fc0d59bdc5c3b9588479690b2dd014"),
		new("item", "renaissance_furniture_and_shield_rack_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67d0aad3970c32e12617fca5671e98e861f2dfc08b1a5a6cd309983cf4778b61"),
		new("item", "renaissance_furniture_car_stepped_display_shelves_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba9dba67286c370fa6489c6488dce42c2e28db471f0ad65bc5dc2a4f55ffc760"),
		new("item", "renaissance_furniture_cef_lock_fitted_weapon_cabinet_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "77d9f068757a98a85a1941873d6c174c800ea6bea8fecf21e153e7688d87ab4f"),
		new("item", "renaissance_furniture_cef_tall_wardrobe_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "70684777a2d1493aadc9b43829fcc281822d3a6ba8a93d86e49dcb6e937ef058"),
		new("item", "renaissance_furniture_cen_chest_of_drawers_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39213abf267d5dea6d2a33b8668eaf5754449308a94e9c6e5a29afb97ab8fc7c"),
		new("item", "renaissance_furniture_cen_low_drawer_chest_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "caeedccbec09db0ace13f22542a53aaf3db51186f451f88e29cee0a5aab50ebc"),
		new("item", "renaissance_furniture_cen_secretary_cabinet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f76bd305891edccd3168ee81a0b240a295280d247285f4a1b4f9cec997c90311"),
		new("item", "renaissance_furniture_col_end_table_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed6b194c2535fa290cf99e4dc0038d5447e0a3c42addef13d681fff3184188ae"),
		new("item", "renaissance_furniture_elite_household_bridal_cassone", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2a09815c3de984096155a622cc903edd11b07753f506642404eb8f501539c8b"),
		new("item", "renaissance_furniture_iba_cloak_cupboard_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cc75591f41676a47bfb92d9ffab0958faf54736677f53c2a17f40e9873da031"),
		new("item", "renaissance_furniture_ind_harness_stand_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "97b257fa8f4f194607122f9e11a549051e9f6d42ddc6452e8ad636c31a4d4647"),
		new("item", "renaissance_furniture_ind_travelling_wardrobe_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3fce7e6c07e565984d24d5e46e9c7ec72225598ea03a3509ff92126024bd5843"),
		new("item", "renaissance_furniture_jpn_household_cupboard_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "34a34bb58e08b590ffa768228f732251a53c7bbfc313fdb2af5c4e3c2a5379fc"),
		new("item", "renaissance_furniture_mar_helmet_cuirass_stand_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2ee2153eefdcb7523b82911a99583a9224a1350f6dc1b5588571e3e2e1aef56"),
		new("item", "renaissance_furniture_mar_lock_fitted_wardrobe_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b785de7dc6e0dab13b214a3ad5a9623c2ad65c129c3ff8c8d8e928dbec1b7c84"),
		new("item", "renaissance_furniture_mar_matchlock_rack_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cae4356cfe53fdd9d96aecaeaa9b3c6389276a20a85f3dd3b600dce34017838"),
		new("item", "renaissance_furniture_mar_writing_desk_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "563f3a33a4128c43c263bf9f01df4f4d8892c18e6879abd4fb1172b645cf6e86"),
		new("item", "renaissance_furniture_mea_lamp_stand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e23741c28cf9532b54a0429b5b224ca9b0d00d3aaedbd3772b50d6a2198e7a8a"),
		new("item", "renaissance_furniture_mea_lock_fitted_cupboard_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8db8af1637a21086c9fe8cc07a54905b13b9f8dfaa47866dde5e0488b33f6420"),
		new("item", "renaissance_furniture_mes_campaign_weapon_rack_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0d9ce6651b849931b12735b217e4ba4c553f4359824be6e6cea59b88c1671ef"),
		new("item", "renaissance_furniture_mes_sword_rack_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba9cd6739b08b5d3a01477ef4eb8eaf5bf6b38bba5f3239d534b946ebda2a9eb"),
		new("item", "renaissance_furniture_mesoamerican_market_jar_storage_platform", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "72e635f72b4d7bb1096f871070a273b921b341e6e3fa1cc7258bb88d27dafe9e"),
		new("item", "renaissance_furniture_msea_tall_wardrobe_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ac8eaeb583a2dc20d2d68221988baff34700868c876d2025afcaa6e638229ac"),
		new("item", "renaissance_furniture_nba_freestanding_weapon_rack_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81eca6441b121cb87981c68abe6092af525da4a6327b38e50bc9eebfd213f1ba"),
		new("item", "renaissance_furniture_ott_crossbow_rack_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c34385c6c0f04f3981cfd955ab563378e22aa4c15e9656982e460c98b700fcb"),
		new("item", "renaissance_furniture_pip_curiosity_case_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1769c54af2b586fc8d7115473d5623cc044446fa3b8c78719816de5b12a0c06e"),
		new("item", "renaissance_furniture_sem_lock_fitted_nightstand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c41f3426e885c2166d4f34fb01dd6ba53fde94405b08ed245bbce95b582f5090"),
		new("item", "renaissance_furniture_wer_armour_plinth_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "56db2c278e9fb355f4df79d731670f9a7184ec38d473a6d9e96d18fb33265fa2"),
		new("item", "renaissance_furniture_western_counting_house_strongbox_bench", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e2b3d9f98eeb9879855b927a0331472b4b1f7534c08a70a3291f90a41b2d7a2"),
		new("item", "renaissance_jewellery_aca_devotional_badge_473", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3682390840a5b496cbba9fb21f8d36b8c6f8f9d5308bcd3b5ae8d2818f544a78"),
		new("item", "renaissance_jewellery_and_anklet_608", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0bdc0b3f01ac2883d2f0763945b4493ab2684f35b198e6d04d25019aeccced41"),
		new("item", "renaissance_jewellery_cef_pendant_123", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f071cfd55d05a29b5642a4ff8a1d216a015da8575ef61cebb097cc3248705314"),
		new("item", "renaissance_jewellery_cen_devotional_badge_83", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e493083a79ff25a5fb9ec869aacd4019fd49f86d24d5f89d5c507714280d13cf"),
		new("item", "renaissance_jewellery_cen_earrings_69", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1262126094c69100e80a1db821d9a589fe4d137c401a72cee9ae2cd72bc91fd"),
		new("item", "renaissance_jewellery_col_anklet_698", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "913f4888345a94cceed1c7654bb2a0c979c2142b310756dce18fb628dc14edf4"),
		new("item", "renaissance_jewellery_col_chain_necklace_695", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18641c9e170f0594f24072ed52b293cec48794cefe11f5668b8e1ee8ff95d2d2"),
		new("item", "renaissance_jewellery_col_prayer_beads_711", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e91290c3f0583573b753c0094f17face1e08699f2dffc0f7a76ceb9544bbe06"),
	];
}
