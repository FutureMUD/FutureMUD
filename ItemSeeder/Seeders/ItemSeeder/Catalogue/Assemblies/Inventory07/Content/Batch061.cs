// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch061() =>
	[
		new("item", "renaissance_food_mughal_yakhni_rice", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "26d505dcb4e025980256c7e23dae35c13ce3d61eb615dee2215b203408143a8d"),
		new("item", "renaissance_food_ottoman_vine_leaf_sarma", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8eb4992a3ac2c489a4b6df649b2e855a7c76475449a0f8be31c6ae8b808a082d"),
		new("item", "renaissance_frontier_felted_winter_boots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0da7324356b33c79ec6b15e2814b67da31670fcd03398aa1f08e60df2983dce6"),
		new("item", "renaissance_furniture_aca_compact_garment_cabinet_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4758d49a070f0751e1d2d71f682abdafffdb9f757e4b59624654f7f8991ec976"),
		new("item", "renaissance_furniture_aca_linen_press_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbbcca935bce274b162954b2fe9bb294ce1427ae75e01e85d0bf06b71e4bf5c1"),
		new("item", "renaissance_furniture_aca_statue_pedestal_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a2ff27fab14434c9bbc716db9c4c7183e528f5ed436cd2be2cb5298b50f1da9"),
		new("item", "renaissance_furniture_aca_writing_desk_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0009508f1c41c664a28f02c6df7b5553fe570d66244167288f25f7315e2a6c02"),
		new("item", "renaissance_furniture_and_writing_table_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1865719619b3ef3848c768a99b184829be56266ebfb52d151b986ffc40ba5eb1"),
		new("item", "renaissance_furniture_cef_glazed_display_case_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "337df1fa876bdfbee87fb70d939bcd14293126745a48ed552a3c9accaa389954"),
		new("item", "renaissance_furniture_cen_campaign_desk_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b837fcc06ef98ba6812ea49127d936991b81a2262730479836fc4bde0542ef8"),
		new("item", "renaissance_furniture_cen_lock_fitted_armoire_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4190284f4eb70d32cde45879631c9c88d6c44a784cf4e42c86c9a03b59416fed"),
		new("item", "renaissance_furniture_cen_nightstand_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "739c97585bc578a1e00ad3276aa055527812e13b49279c68371ea038d3c2f4af"),
		new("item", "renaissance_furniture_civic_guild_charter_cupboard", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f69d7100047784e886c0eda9d08ef8bfa1ba75ea63d4513a36cf15bc93a8d9a"),
		new("item", "renaissance_furniture_col_chest_of_drawers_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf59108970a6ea8e9affd8b4a71512ef6743e0ed94849566a0cd95ee35350d0a"),
		new("item", "renaissance_furniture_eal_document_drawers_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "afe23a3f91e1f39d218736a5a017096861ce9247ce66b70e3120407827319a79"),
		new("item", "renaissance_furniture_east_asian_literati_garment_cabinet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc329e61c6edbd5cd09aa4b307056d9b73431dab659c7651901b709e4111ed3d"),
		new("item", "renaissance_furniture_elite_household_display_credenza", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ebbce1155b78069ae7bfdc21e5592ea10ec140a46c00a3b3d9fd28e9feade49"),
		new("item", "renaissance_furniture_eon_slant_front_desk_05_lockable", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfd7523a0384b8cecc3774e649820fbb10a24b56e2d80a3225977ebadf72a8d9"),
		new("item", "renaissance_furniture_iba_wall_cupboard_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5daf126a1de5453f47212f3f136cbb6c4a49c969a61fb95709f1b54dee8d1715"),
		new("item", "renaissance_furniture_jpn_lamp_table_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df5b815b8505cfc64698d59e9e216fa4ff58a70906f4ac72cca9e92c02185e02"),
		new("item", "renaissance_furniture_mar_apothecary_drawers_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6162acdf8ae751c5fa72e08916dd0841acb2f94defb44e045d031b9225b03dc2"),
		new("item", "renaissance_furniture_mes_writing_table_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3186f8387d8f5ed05996a023494095db8099ce57d62601411dfa2e107c8c611b"),
		new("item", "renaissance_furniture_mesoamerican_market_platform", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec9fe137e1fb72a2cd4a3141aa23d628517762d854049c57bf444b8ad654fe41"),
		new("item", "renaissance_furniture_nba_bow_rack_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca43214384e4af07deb85b4e7fef3cab83ba1de63537cda9c239b5f9b4ba58a0"),
		new("item", "renaissance_furniture_persianate_court_garden_service_table", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e79f0d12bb39a0817962da5a71799291474d0c5a177c52c350c09537042ff4ab"),
		new("item", "renaissance_furniture_pip_metal_armour_display_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8512af8ec968515db166aedd63e5a067ab968ede5937823662c021d1574cb777"),
		new("item", "renaissance_furniture_rse_matchlock_rack_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef53866b2e2e9947454b339396a28f70a046b1e51f98fe52bbea0c87801073ba"),
		new("item", "renaissance_furniture_sas_folding_display_stand_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3c1c366dadbb9e3fc5c01e496671745f3c3740aa8935de10a628b33253da414"),
		new("item", "renaissance_furniture_sem_campaign_weapon_rack_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3e724aa208ccfe34f3d0c57402bb050cb1b1979d20f5bcde4d74e1359244cae"),
		new("item", "renaissance_furniture_southeast_asian_port_market_counter", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2d12be020b32c0a1e0aa00fe1ae61b77915eb725e55f8534e5f1a9487aea942"),
		new("item", "renaissance_furniture_stp_freestanding_weapon_rack_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f0b973502f0f590db5691b8f5472a3dfa3aef6e7d034591f48d7df2a747e663"),
		new("item", "renaissance_furniture_stp_lock_fitted_weapon_cabinet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e6bc63df1515f8175dc5d27e730f7c36d08199a531d49b2f8a1c46d1db22d02"),
		new("item", "renaissance_furniture_warehouse_dock_cask_rinsing_vat", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "498521df3b8ed21305337bba5bff8bae18e5eeae738e055b8d820057698a39e7"),
		new("item", "renaissance_furniture_warehouse_dock_dockside_counter", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e21971d128a1c39cad80418bf533e394b27456bfd3392f4134fef41df61c0bb2"),
		new("item", "renaissance_furniture_wer_tall_drawer_cabinet_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5af2a24586eabe851de3a465faf8e1cc1abca97f0418bf8b43a5bbfddd9489ea"),
		new("item", "renaissance_institution_full_cowl", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f18a6c45d5286cde6e97211eda64e8da03c92cf67eff83e0566af97d34cb544"),
		new("item", "renaissance_japanese_quilted_sleevelessvest", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fc0013d11e34d194baea1e2e92f1a5957b01f2424ec39c21ba07a2a3a8ff0bc"),
		new("item", "renaissance_jewellery_aca_circlet_463", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d52ce577a99949a83b93beff1440c7ef71243b9f6effd1879bedf77cb6592bcc"),
		new("item", "renaissance_jewellery_aca_prayer_beads_471", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bfa752e22ed16c6209825e40bd6d8d36001728d9e9ae70eb1506cffb26a1788"),
		new("item", "renaissance_jewellery_aca_waist_chain_465", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50786216da4adaba9512872f9ce5033155e552192d93fca8ffc8076d4c4c0d17"),
	];
}
