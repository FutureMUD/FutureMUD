// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_container_ger_punch_bowl_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cac09ac1473c2fc29133412a5f1651b98d3b133fc54f0290817a1df5c542291"),
		new("item", "earlymodern_container_ger_tea_pot_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4feaa8d38f081a03550c9ed395c3845078a3ab65b71f44830d7c41f2db8867cd"),
		new("item", "earlymodern_container_ias_linen_hamper_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "235fbe17642b5e6fea10239f5750300f4d1dd57c7d091af5195c2de0fd2cc39d"),
		new("item", "earlymodern_container_ina_tankard_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "66f39be770b33a441eb13fbaff61c69e8e820b4997d0fc4e19b44e031a3c97fb"),
		new("item", "earlymodern_container_lmg_account_box_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0597e9f7dde14658579258ead4a012053d3db94c499a67bb95042b581db3bf1"),
		new("item", "earlymodern_container_mag_tea_pot_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc526f83ff0b157207ce61915d601601b3ea0b41c96f6d0fadd32bdb500ec774"),
		new("item", "earlymodern_container_mes_lockbox_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f96d38d9bc49b68308ed6cbd0a4a2ea29441a00fa35f09d7b9dd5aa0779e457"),
		new("item", "earlymodern_container_mes_spice_basket_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "10fd45f0830f6e0217fe07608281bdf4f2baa3462b3df31e3305916c86a47c3e"),
		new("item", "earlymodern_container_mrd_tankard_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a361f2cb544f07e447db1bad03875b32d199f4b3a8ba5b9c717ec9c29ce74ed0"),
		new("item", "earlymodern_container_mug_account_box_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "98f472995d58f038ab7b62b8a0a23e7f89f852dacf9fab9829fbb393444c1659"),
		new("item", "earlymodern_container_mug_coffee_pot_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "08c1608e11e80f45ced9e35d5c3bb5ab7619147fadcbe839719f64ccda329218"),
		new("item", "earlymodern_container_mug_tea_bowl_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "698e5588461c4ae3f7e67bdcc18c15478842f825ad923b6257660c134beb0b00"),
		new("item", "earlymodern_container_rus_tea_bowl_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "add0063435ad0170ec89ae29c4752ec63601b1bfc8eb6d283320ff5839394484"),
		new("item", "earlymodern_container_sea_storage_jar_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "89f394bab46ff326ceac1387fd27f06cad445a6b78764e8a4219df5f513e0ac7"),
		new("item", "earlymodern_container_sea_tea_caddy_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dba29f7c3e13e4ee860c5676f8759f3e7ad7f563eb6f8571bdde816e921ecce3"),
		new("item", "earlymodern_container_sic_account_box_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab8a0d77e055ce161a34e3068c169f8ee089cf14af77003ad6d9c633a9386b78"),
		new("item", "earlymodern_container_spa_linen_hamper_30", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "85669554b12f799d71b924c005aea480ec3c6e62947fd5ba1005baaf46a76967"),
		new("item", "earlymodern_container_swa_bottle_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f4db839a36e8a881b7689f79449f0f1abbcfe7afc5fdbb7514daa40a15af0b7"),
		new("item", "earlymodern_deccan_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c6cfb445297d6ee7ff7506b466d4faf63625357a43f763fc82734f6a7dd3fc8"),
		new("item", "earlymodern_door_bra_shutter_556", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "97783be8807fb648708bd764d0deeb77c57ee3da5c088c573c750f334e0bf7e4"),
		new("item", "earlymodern_door_edo_grille_353", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f59a0b37792d9a84eafd4796297ab84213c8a3677f28b6c761b14e2a96b27944"),
		new("item", "earlymodern_door_ena_screen_576", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f1e79e60891ceda034ff08e0c79bb5067b4505afcb0030f56cdf3c8079304f4"),
		new("item", "earlymodern_door_fr_secure_door_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "80db7557898b961152b28b25edbd7cae8f00d8ec49523bb3e25b15b1c65b05a7"),
		new("item", "earlymodern_door_fr_warded_lock_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "74499b38cad98d7ef4a127f98b7f5d14c28b0de3d0bcd983711b9c3237655b3f"),
		new("item", "earlymodern_door_gb_master_warded_lock_56", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4e9200e5d6a1b712db33abff0bf7951179e00d1922c720ee14bff6a1b959eca"),
		new("item", "earlymodern_door_ias_lattice_gate_425", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "092d53ba5ac79af80bc6b124f43342dd60fbcf6124e53e396a8d08c3d41a3dbc"),
		new("item", "earlymodern_door_ib_grille_68", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e9004ac9787d97929844b7082661910a4db19971c7130154dc0f131cefd1a94"),
		new("item", "earlymodern_door_jos_garden_gate_333", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ee95a3c6c81d6c662dadbe983e0cf4e0bae15866b3d4f74b5e83c69ca4a5633"),
		new("item", "earlymodern_door_jos_good_warded_lock_340", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bc2c145078b7f18c55b51af443d318378f2eb9f5f936245c37ee9cab32e6e3f"),
		new("item", "earlymodern_door_jos_hinge_pair_336", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "89eb1b70eec7dd2df8a5811558d8a39f44c655ee13b5fae29ca82c46a01c5565"),
		new("item", "earlymodern_door_jos_warded_lock_339", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd9a8141674a9766fbfc7a8c27773aa1181df59a7c3acc929c822eb30c6d4644"),
		new("item", "earlymodern_door_kongo_secure_door_460", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd52119aaaeda4278830ab883e54f514cb7fcf6d8b8bf008d824ca048a6e1d25"),
		new("item", "earlymodern_door_kongo_shutter_461", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca05159ff8af1ac442c5071c1b481d473dc6090bfc8fbc9c5f808d4cec55ce8f"),
		new("item", "earlymodern_door_kongo_warehouse_gate_465", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a7be5d9829e93872288de3a84f6a84c0ea7069e859834476efcbc3b41a1051a"),
		new("item", "earlymodern_door_ming_shutter_309", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1dd50ebb90e3299cb8b4b3703371acb7d663df5ab8afa1a72b21771795fa5a6"),
		new("item", "earlymodern_door_msea_courtyard_gate_407", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e063fa4d6850269d2b55e5461bcf561bbd7632fcd309dd16a7fb35c05520801f"),
		new("item", "earlymodern_door_msea_grille_410", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a5049e668f55b43f10507204cf6b36081a8cbc4cd17350fedac297a93872530"),
		new("item", "earlymodern_door_mug_grille_239", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ac46ca1ed3460e580f5d1a70aa5ef6371dbc466582d87bbfc778e263f0572e0"),
		new("item", "earlymodern_door_mug_hinge_pair_241", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e63d86eb31803c3fde7adce6b128743ad720ed168ec7ff8e06e66f278ae715d"),
		new("item", "earlymodern_door_mug_keyring_242", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f2025d0809bba533b47893726af487edeac251ca695db49c87130b5591b3f12"),
	];
}
