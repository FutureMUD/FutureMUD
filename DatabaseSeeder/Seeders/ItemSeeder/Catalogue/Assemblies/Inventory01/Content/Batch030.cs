// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch030() =>
	[
		new("item", "earlymodern_deccan_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "17c5ea118b1a0ea487f17b71d57bfc2415c6924a2e58c6bc3648af855902d552"),
		new("item", "earlymodern_door_bra_escutcheon_566", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6759eb8ed974195aa845049848b1621cc64f735d4aa8f1fd3c4c2ee59dda8718"),
		new("item", "earlymodern_door_bra_good_warded_lock_568", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cd167f2a23082e80d435e35d547865522c3df628a5d1c3c51a86b5d9eb7d13b"),
		new("item", "earlymodern_door_car_shutter_651", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1587966563aac4e2a5d3ab9061e303d0b7510ccd8fc1b511b599023cf91c034a"),
		new("item", "earlymodern_door_edo_courtyard_gate_350", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f6061222d49aa06230f7156d09605cf74dbd795d17481758f318b029412b321"),
		new("item", "earlymodern_door_edo_panel_door_343", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b06733c013a840a3d65f72d92dd77b227acbd0a7b201d868f6852b1346c86e90"),
		new("item", "earlymodern_door_edo_secure_door_346", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7513d6b67f0eab39305a4fe07460c4eb8e3cee0d7a2805ec8e3ba03ec12c23c7"),
		new("item", "earlymodern_door_ena_good_warded_lock_587", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c66b000584db61ad4382edf596c4df574ce624b8b15b9258e1e4a0ca795229da"),
		new("item", "earlymodern_door_ena_grille_581", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7a6dbfb4420fcbe26a8a2390100a6f990b2daf6ac22d3637234e7cd82b1d3c6"),
		new("item", "earlymodern_door_eth_lockable_door_497", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "031010612b09b90a82645a859502d7c07adee68eff59af2a925c887afa659f93"),
		new("item", "earlymodern_door_gb_secure_door_42", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "42575cd3552bc676a3043a47b13782f9e33988e26018817c53663935ede07b44"),
		new("item", "earlymodern_door_gb_warehouse_gate_47", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bed9cc9e9d0411d268108a48263cab8605f9312629c164669803b3033c8f4ed5"),
		new("item", "earlymodern_door_gmt_panel_door_666", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "39db72e3633bcb3bdff0ff5a3855789704ab209b7bd501d086e610622ef65169"),
		new("item", "earlymodern_door_hre_lockable_door_79", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "35d121e7183efce96159cdf87e79ba18a844cc9651c1e5f607ce763417e2eb71"),
		new("item", "earlymodern_door_hre_shutter_81", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a18218d9bf527e274bfaa9c645c8b2c36d845d2688d41bee3c674e187c6996d0"),
		new("item", "earlymodern_door_ias_braced_door_420", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "479c14ab3ee1c7a1fcf5d14fab5f59ee7330087c7e5c5c03b5f1ddfbc63204c5"),
		new("item", "earlymodern_door_it_secure_door_99", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f8de5688e6263d0286fdc2d8c9ad547269863bfdfc3d74ad896b10f8058396c"),
		new("item", "earlymodern_door_it_shutter_100", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9eb3b30eaa85a21c245d3343046b7224bbe3183b8e07db54912cefbf0922aa18"),
		new("item", "earlymodern_door_kongo_panel_door_457", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ad72c8c5aad6a16e839ae3d4b0d811875fc4b878d74f4a0dec08557c62ecf80"),
		new("item", "earlymodern_door_mag_master_warded_lock_208", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ccb6c13df2fb7828f8b471d21258c0599c7beb98229b2fd2f37cd5054b5e8e8c"),
		new("item", "earlymodern_door_mca_lattice_gate_615", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "95d0ceffaf333f38dd173e9da8956ced1923c2a518c6fccae721cda82dfe9d4c"),
		new("item", "earlymodern_door_mca_lockable_door_611", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6a05ddc1ab86a4d6411da42582a26d01390d391f1f7f897765b70a4eeb05911"),
		new("item", "earlymodern_door_mca_master_warded_lock_626", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "452598b6341e2622922ed8f70f0008b114f8c8b9920dc45ddcec3a2111ca65ac"),
		new("item", "earlymodern_door_mrd_secure_door_251", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5bf697561d8c93576a2984c1b9edc4998f392d081b6207cc1ec88eb0737e91e"),
		new("item", "earlymodern_door_mrd_warehouse_gate_256", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f8a71d7eaccda0f33b044a60ee61b5f3b589ed06d7ba847724a400de27fbd21"),
		new("item", "earlymodern_door_msea_panel_door_400", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a664008fd26643636655b1976c3eb290d827ebfa9bd05136957b61353c7e411"),
		new("item", "earlymodern_door_msea_screen_405", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e68fe0b17953e853095d637d3500312bc08261804ecec94ef343e0e252fb4f2"),
		new("item", "earlymodern_door_msea_warded_lock_415", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee160b701ed3e7384180e9244a9087a14e2759def38d946e9a08c920337bc8a8"),
		new("item", "earlymodern_door_mug_escutcheon_243", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0faf5e314337128f6db083267148a0fa833ea53f23e9d4430198c77f7b138c0b"),
		new("item", "earlymodern_door_mug_good_warded_lock_245", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d61ab0380e8be56d31c7027a2121e017d6f9e27819311386a3b2e30be8581e51"),
		new("item", "earlymodern_door_nl_strike_plate_31", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d44cfd7f5abea2c3ec8f42cac727a01488618a74fa26e9053ffceb6364929cb7"),
		new("item", "earlymodern_door_ott_grille_182", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe7714169286968725d64760d70a355667b970980c004d1d14c9bc918b30ab33"),
		new("item", "earlymodern_door_qing_shutter_290", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee977ff5ad58cabb0a816290d2f7ac2ca9944163ca67fdc57ed4f485d68f1866"),
		new("item", "earlymodern_door_saf_escutcheon_224", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5916ab452c7156616241f6293aa09378403b2df2d71a2ac154b689e70e69932"),
		new("item", "earlymodern_door_saf_garden_gate_219", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "64c1861f9b6e95805e6feb0f8fe62f68ca3d3d5e7e5fbdd22d3dd6b4054910f0"),
		new("item", "earlymodern_door_sahel_lattice_gate_482", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "613c815aa188770e838182cc3aace5062d6dae73ba5c42f9c3e57bdd67ec3634"),
		new("item", "earlymodern_door_sahel_master_warded_lock_493", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9bd89c1bb73840feed83f77d40aeeb340170bc0fe864ac8dc5f3310c4830a70"),
		new("item", "earlymodern_door_sea_escutcheon_395", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6eddcaafad00ffd4572c51681563e724f60a276582e0a7d6d72c1cbd9886316f"),
		new("item", "earlymodern_door_sea_lattice_gate_387", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d558505a1ad6fc4a09a54da7a2b13ad7f2d1b11d39be7a500f81ee9e53a1026"),
		new("item", "earlymodern_door_sic_good_warded_lock_283", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "76dfc9721516668e9c2629e8db4ec8afdeed5b3cc9955c494534f7ae846492ac"),
	];
}
