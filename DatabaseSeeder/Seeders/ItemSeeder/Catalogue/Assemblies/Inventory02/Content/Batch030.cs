// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch030() =>
	[
		new("item", "antiquity_wine_measure_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "df90fa4d7ad3d2c4cfd07235f5ea6ae6d74cd3ea1ee84870affc9a9a3e46b140"),
		new("item", "antiquity_wooden_headrest", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c8b6726faf93ff8be4d69cdd47c3d73c39807039323acd0a9a67224a8cef9d3"),
		new("item", "antiquity_wool_exomis", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "47414f2d5a38cab5d260c04dd64fbd5a0bcec5760ffd7e06d47ba0bf1488f19d"),
		new("item", "antiquity_wool_toga", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e8a32000a2ee7d5099f19568cb1dfe7a014deead66c305b317645d2a1e0d5f5"),
		new("item", "antiquity_wool_work_smock", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "61ec6f13da4888578d43d823a871546e0b76840a23ee093a0807bacbda429af9"),
		new("item", "antiquity_woven_reed_pallet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e8936231946ad160d9f5ce8e99b1de52da79ea7b6fec0c459cb7058482eb7ba"),
		new("item", "earlymodern_andeancolonial_clothing_shaped_wool_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9b3c91a1ddaa0a68ca89e8aa9fbcbc613ef6c593ee34e7efb4202931d43bce7"),
		new("item", "earlymodern_brazil_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f745b3e3e8a8529ccc7896375cc9d61a8d72667cb0c0aa65056b272bd273c53"),
		new("item", "earlymodern_colonial_clothing_long_linen_shift", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fa472f054d6498122cf82bd341384730bc443137b38be3d440f52855b907022"),
		new("item", "earlymodern_colonial_clothing_plain_knee_breeches", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76e5e3f3b181e064ff2e001157bf70ed9d4face7dee5ffd49643e684c8f62914"),
		new("item", "earlymodern_colonialnorthamerica_clothing_wool_shortgown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "537717bfd24ca35b01b20edfba88502d38c85551401dfdd979bf5ddd12d9c0b6"),
		new("item", "earlymodern_colonialnorthamerica_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4cfe2348a4a9e00f480323482609bf4802377b2f5d14c7a9b850107ef8e769e"),
		new("item", "earlymodern_container_and_storage_jar_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3ab3a1b24bc5b8c1e3c7a620045aa558bfd1804740768d05d476a066db3f123"),
		new("item", "earlymodern_container_car_travel_coffer_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "af88f7922e930dd10e7f5583c2ecf5aeb0b2ef1f3298727c480102ada2bf3fa5"),
		new("item", "earlymodern_container_dut_punch_bowl_29", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "859d16661a2406e3932ee77a58dea833c159ffdaade99ea521f4e1056aec4af1"),
		new("item", "earlymodern_container_dut_sample_case_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e752342283d4585017210c821a7ba85432a411a2747d40d8dc468676d2425108"),
		new("item", "earlymodern_container_dut_tea_pot_28", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8973ab846002466278100e306065746ff19caf2e85d20ec63a82181ac9b27a8e"),
		new("item", "earlymodern_container_frb_travel_coffer_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "53eea10ef65d796e48758fc27e32df618381f7ce31941590043dedc7bf235c6a"),
		new("item", "earlymodern_container_gmt_tea_caddy_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "82e8e49b886b6c4a731b7c12f386f3d915d9acb6cd8325a701f177c88879d962"),
		new("item", "earlymodern_container_ias_coffee_pot_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5f775e426dc5503df3a7c2f75b75bec7ee07368a7cabe24ddc1fc6bc02475eb"),
		new("item", "earlymodern_container_ias_lockbox_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2feeb366c0b82029a582a9c9b1c194d0b650a9ba9ee7b86861e792442b6a207b"),
		new("item", "earlymodern_container_ibe_linen_hamper_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ebe7e6c0490bf0e0f1fc357490c0d8acc13981e43a9d2bd6256035ad92bc465"),
		new("item", "earlymodern_container_ibe_travel_coffer_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "abac78975f5b7565b63c3711d3cfb7fdf589cf0e6b8e37f49d225626b1fc8036"),
		new("item", "earlymodern_container_kon_sample_case_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4bcd960dbd9fff170546b7659812faedc1d97134e978f71fa8814a02268c8217"),
		new("item", "earlymodern_container_lmg_bottle_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "730903636ec12d5591a7ada3d0fa6b9a28cee6050979d74ad0dbd80064d8ff99"),
		new("item", "earlymodern_container_mag_sample_case_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f51fc52f0ec609803a9e21741a17f36bc0f4c90187c84a74d28b85b8b2b169e"),
		new("item", "earlymodern_container_mag_tea_bowl_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3ea85c9050246a2a4838696c639d153daae5effa5c9bc01e533468f2b166aaf"),
		new("item", "earlymodern_container_mes_bottle_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f8105ec92fe5467d079126d12c4abd236a44a3f6cdac97b6dabd0186b1bca2f"),
		new("item", "earlymodern_container_mug_tea_pot_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa7698e90ea9fe8ac1c5ce87356f8e57838d7fb8dc60cdaa0b2ee43a2a9dcffc"),
		new("item", "earlymodern_container_qin_storage_jar_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c385fa904d917ae033bb88a00bd7266479be4cd4edee8d28284ef662b51eeadf"),
		new("item", "earlymodern_container_saf_account_box_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cab65262b2208885f903b56864db72c830292895dce7b880d866b5a0f74a916"),
		new("item", "earlymodern_container_saf_tea_bowl_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b05a2f150f9d81ad2c4e0256010e93041ffcdc6e721cc4845d9c675a40d87738"),
		new("item", "earlymodern_container_sah_lockbox_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "75fcd2ac22aa21fd17e9b25ee36e624caaffdfebf70e520745ef732bbbef3025"),
		new("item", "earlymodern_container_sah_tea_pot_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6ebc0ce41d48e78733f892ed38b9ff08a74f85a32c8e1dc3e8765de7dddf6a3"),
		new("item", "earlymodern_container_spa_account_box_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3420821650668113dad27f334e23f876f0aa596325e80f2d4420ea59a37bb41"),
		new("item", "earlymodern_container_waf_tea_bowl_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bce76e744c5d891db3dfbdc2e786ff9ad4b83c3eb3b00b3e39fd0826369c0a78"),
		new("item", "earlymodern_deccan_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5666339562742724843b2180529439c1998f9f278ac172503a44bf2f81969e77"),
		new("item", "earlymodern_door_bra_keyring_565", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8f6e6e577aa64b4816b6b27bd0bf691a4ee368e8cf8e770babe08938b8ca650"),
		new("item", "earlymodern_door_bra_warded_lock_567", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b1047bc22c7c53ad6f548c3a36d2662a90d2fa6bb44b4f2a3a125364208f774"),
		new("item", "earlymodern_door_car_courtyard_gate_654", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4a15d603b8d88b30a77c6d5242029381335c1cb47abdc03619e2f93ad9de6fc"),
	];
}
