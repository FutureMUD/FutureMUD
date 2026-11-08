// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch055() =>
	[
		new("item", "preindustrial_food_plain_stewed_pears", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "38c5467b65a15b69cfe5e988cdcab8c9ad0f52eaa0f9cbfbc4b24d64edcdcae8"),
		new("item", "preindustrial_food_poor_fish_skin_ration", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8cbf93d790bc1770b520f1b3b6e52a424dd6cbdc7649005cea132986bff0bbcc"),
		new("item", "preindustrial_food_potted_smoked_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "15b85d8208a6f92881a23e3e0a4ff815bb4c44997220fe517e93ca19bcb46ce9"),
		new("item", "preindustrial_food_pounded_dried_meat_flakes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb4b1bb994787c6532eaddaf36f6502cce5be1ca4f8501243b6ecbc924c42cb0"),
		new("item", "preindustrial_food_prepared_offal_stuffing", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "487c3f1fdb1d1b6e07e59e4578a4c9a3bb2f1eba3c6f83725fbb1b5146f3c7bf"),
		new("item", "preindustrial_food_pulse_savoury_spread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "424491268246a34b728a0d9e588f84d917440aa94bebb511fe46b2d324f6757d"),
		new("item", "preindustrial_food_pulse_stuffed_turnips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a25553424841487b545be487fb762daad5e0fdae345cffc7a9d5bb3c7a5add84"),
		new("item", "preindustrial_food_raw_blood_sausage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "098458a1c4c220eeedfca4943855ebf75da359e9a506a5e9aa19418b4450b492"),
		new("item", "preindustrial_food_rice_dumpling_wrappers", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3bda2e7f8864a29e53d4c8d0ec8516559c75fb549c1db5cf71df0c87440f44f"),
		new("item", "preindustrial_food_rice_flour_crisp_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d88708a97a5c7ac67371a9145fb1f640905c1757c16118035de56cefe2c6aa0b"),
		new("item", "preindustrial_food_rice_meat_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "83a9e141170b10738aa20690f6e9ae06374f46a4d2125290be6f03d4855c2966"),
		new("item", "preindustrial_food_rice_noodle_sheet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0719981c678bc21a664baf6a2aa11839ab7c0519ec7672b6f0bb9529fa2c1d51"),
		new("item", "preindustrial_food_roast_spice_packet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b7ebf00dc8ae08db5491702e8f92ba0d062896b52eceeb943642a6e56aaf8b5"),
		new("item", "preindustrial_food_roast_sweetbreads", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01b0d79610af1c86ed8e825a8dac37c8c3e9dedadc10872fda96fc70b84af582"),
		new("item", "preindustrial_food_roasted_root_spread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a656b50c9d83c16631b55c7f23ce3b51cb96f49f32875d6f72d99a00c4b7647"),
		new("item", "preindustrial_food_roe_and_egg_custard", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3aeb7f47d88d0806fdd89dbd54502207d4ddf47d06fcba3677ed59275067b76c"),
		new("item", "preindustrial_food_rosewater_custard", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "986776fd78e552883923fb8fe8c8abe8e38dc756bd84766fe0855f55f25b2b40"),
		new("item", "preindustrial_food_rye_and_turnip_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7841dadc3a7b665a6e72afab70aba060d85136bef0493603e5630df627894724"),
		new("item", "preindustrial_food_rye_hearth_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a39820810df9549f1c7100c6b2080294b0370f9f59825655b41eecb572cb49f2"),
		new("item", "preindustrial_food_rye_snap_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "484df9e43cb8cbf997b0bddf82efafa46ebe68e58da3bdb4afd9bd00d40bdb91"),
		new("item", "preindustrial_food_saffron_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fffb3461a6eb1adbf3cf7c728c2953311cbfeb206826618e0b3fa7ddb1e939e"),
		new("item", "preindustrial_food_salted_herb_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2565cfb6ca2d5dc47fe86e7f3ebc617401a408dee49811653d2fe18e4b3741f3"),
		new("item", "preindustrial_food_salted_meat_in_grain", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e5e7f5f07e1e5c8813c0ebaa7cdf5be672cfb3739c3b816dc4867e63891744d"),
		new("item", "preindustrial_food_scallion_steamed_bun", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "302b92a5078c10a110170c5ee07a971e963553f4d7caa0e63eecbe99ee150aad"),
		new("item", "preindustrial_food_scorched_ration_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a900409ffd5a776fff9e47019c1286ae5403b47a3504a9093e6001cefcd1e2f"),
		new("item", "preindustrial_food_seed_pressed_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87f7af78d5f288ed608ddbddb655bd70d27eef1e55c2ea4dde605718237a0d34"),
		new("item", "preindustrial_food_sesame_honey_brittle", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ec8e83411909466d042b73957ab1d972a17bc2b6dfd1b8563a00ef8fc7dac20"),
		new("item", "preindustrial_food_sesame_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fda9878770f1e6c876b539e6076da0cdf86ae2deb98e0acca1d1e38110a3ae37"),
		new("item", "preindustrial_food_shellfish_greens_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "32c6775da42bcccb5087ead175d771945644a5f36380ba7fc4a70c8cb89ac9db"),
		new("item", "preindustrial_food_ship_biscuit_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f720cb1c3d87f22121d8cc9481527c208cdbe68a404438a2ed881c4229d5f2b2"),
		new("item", "preindustrial_food_smoke_dried_whole_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3fdeeae2a6fbd6311adb5a7f48a781e3557db64f73a084955463d75be120e224"),
		new("item", "preindustrial_food_smoked_meat_knuckle", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "afd2e79edefcd51da92df8092c7d654ba440e804ad6f6e3b88772dbb7426172d"),
		new("item", "preindustrial_food_sorghum_root_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "885a3dfea200d9ba09a2eed73f97a3399b082691e79c85d9ff639372df67bd8e"),
		new("item", "preindustrial_food_sorghum_round_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc435687c06a85ef10288fa3e0533c06371f59b0d8bb24fa9cfc056559a42208"),
		new("item", "preindustrial_food_sour_radish_salad", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8a8b3f47b85e9b86835a6b625650fdb0412241b48765f025abd80465dfa42db"),
		new("item", "preindustrial_food_spiced_breading_portion", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "600086a40e9e99dfd9c32dfd6a381418e62feedf2bbf9f143c6338a2c5f9902d"),
		new("item", "preindustrial_food_spiced_dried_fruit_packet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "86161ac54933e23455d764ccbe2a5b6ccec210ffd793c58568e767bd559867e6"),
		new("item", "preindustrial_food_spiced_fish_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "835294cc41761ef873e88395df53a0ef8a7b3f3db7684a42227aec6d6f89f788"),
		new("item", "preindustrial_food_spiced_lentil_cakes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "13c9213b5ce91294f12e3d2d315f4653380bc5143d47b7cc17ee4640d7d47122"),
		new("item", "preindustrial_food_spiced_yoghurt", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be11d32e6a270774f1a5e1570f2fe78b027aec0d2386132687f38c36d7ea77a9"),
	];
}
