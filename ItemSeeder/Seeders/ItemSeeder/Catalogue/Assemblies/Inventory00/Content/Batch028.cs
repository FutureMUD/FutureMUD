// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch028() =>
	[
		new("item", "earlymodern_container_plh_tankard_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b926afd29009b3f812bad7aff362ca26095deeabc9549ba1297b4ef678b4b228"),
		new("item", "earlymodern_container_qin_account_box_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d06826899b1ed0b54c9182f7e8752ffd0f353654180a7da79405e35539c3179"),
		new("item", "earlymodern_container_sca_tankard_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c3ae0487fcdebbd7eb5b0b83cc03c5c7562717c8c81e407f7eeb36d04759dd7"),
		new("item", "earlymodern_container_sic_document_case_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e670dd2c1203742b539ef2a389df57792339c5278dbccc8f42ff79e933f48d98"),
		new("item", "earlymodern_container_sic_punch_bowl_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c10b046228eea5ffb4f209bf283223ad903c370abeb14315572d1630a353c10e"),
		new("item", "earlymodern_container_sic_tea_bowl_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "31d2b4ca7d7763e71b436cf1805a648e96852b038b806758470cf38e726bad5b"),
		new("item", "earlymodern_container_swa_tea_bowl_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d00a8f180efad48135ca6ef11de1abc2226cd484678a8c2937a5d7089c39bbf9"),
		new("item", "earlymodern_deccan_military_long_angarkha", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6898a7f198a80f12cd2a60af8bf39eb8556e454423084deca13bfc068410d30e"),
		new("item", "earlymodern_deccan_military_pointed_turban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78e519a977963b4ea459e8cc3f7b097a5aa1610fd2f2cb7c9b5f4844605325a7"),
		new("item", "earlymodern_door_and_good_warded_lock_644", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bbc59ade4b5bd38d42a8007b6c533292f08ea9bc6f3513edefe6132156f855f"),
		new("item", "earlymodern_door_bra_hinge_pair_564", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d61df438669c0381df76c8e4a995fa97b7841119a9e37e377a1a3497014eb008"),
		new("item", "earlymodern_door_car_garden_gate_656", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f34a0bdd1c15520895ec4a7121d484c9c1355f6868ef90491742b08f584c141"),
		new("item", "earlymodern_door_car_lattice_gate_653", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e453d504fc5ffbc6f67a632278f6e815ea8c22a118d211d95c701764cee3f774"),
		new("item", "earlymodern_door_car_master_warded_lock_664", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a641255ce023c1abfe532d510000d85fc6b983e37399b49de23b1e82fcb0c217"),
		new("item", "earlymodern_door_car_strike_plate_658", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "87d304d765d89d54e82ceeb61abaa9eee7af85e2789a5b64f415a0e3a36e9532"),
		new("item", "earlymodern_door_fr_garden_gate_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b31e7b8b0067e1a950612266e18fddc876f7e8f8f9de6303916770e1279f6e29"),
		new("item", "earlymodern_door_fr_strike_plate_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "34f383f9138689780c45373100a37bc5f9e4b4c45100a5a4cbab5e90db81aee1"),
		new("item", "earlymodern_door_gb_good_warded_lock_55", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8351508dd5534d380b8b6b2a92dfe602fd6e66aa2a4c6f6f0b0b4bccfb3aa6cd"),
		new("item", "earlymodern_door_gb_panel_door_39", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "01de251f9042b3fc633c1492817c2fa05f60673922205cee0a70d520a8b499fc"),
		new("item", "earlymodern_door_hre_courtyard_gate_84", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c251132f096cd83a9427d401626f02ea404d4f291a2bc0503e5f34164ef0a8a"),
		new("item", "earlymodern_door_ib_hinge_pair_70", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "da14a654ddd235808f2aa2e76d7b783cc144dfaf5222a42857e931c3a4ac2b3e"),
		new("item", "earlymodern_door_ib_shutter_62", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8dc999c9ca775fbdbd7466c48b878c53e5096084260a9c138506b02f7ef0f791"),
		new("item", "earlymodern_door_ib_warded_lock_73", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1efcf39863a03287028badb3a0d259ee004fe9a3d2b9f1f71c96090f358c9179"),
		new("item", "earlymodern_door_ina_garden_gate_599", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2bf45d93038cfe26acbe6e8f5ac26af77ac39d6ae0d0493665b41b3d17b8fda"),
		new("item", "earlymodern_door_it_strike_plate_107", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "680689d432baf553fe6255eb7a3421bc2eee966dc85c229f952ed5c1b13cdbe0"),
		new("item", "earlymodern_door_jos_panel_door_324", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4e2adff19c98329b576439a4bf24441516046704786711747d60f52323f18e6"),
		new("item", "earlymodern_door_jos_screen_329", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "588d02691a862138b52b43a838d0378ff4b6799e55557a8d8be461016e268136"),
		new("item", "earlymodern_door_mag_strike_plate_202", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c688ce9c4f90d9a27da061fee5991cc69099a14834c0afc08dc600094abd112"),
		new("item", "earlymodern_door_mca_braced_door_610", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e05edaa59f267f45869ab134a382f0d7c77275086bb151bded3f6eb808b6b996"),
		new("item", "earlymodern_door_mca_strike_plate_620", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3dd919eea49383f1142cd9f8f71bbe94707782fdef54fda61dec7512f44f774"),
		new("item", "earlymodern_door_mca_warehouse_gate_617", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "14f7d512561c86c7ad8217f7464309cf98bf7b965aa39205d25bed5e1f90eb33"),
		new("item", "earlymodern_door_ming_lattice_gate_311", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "08c0f83a27c87b14918b86c8f56ca9ee78c3d4844fd2a7586704212ef03df8bd"),
		new("item", "earlymodern_door_ming_master_warded_lock_322", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ef87cc8b35c03c654b57e93fb0d848ba7270fb40b542b869bc03f51e9b3974f"),
		new("item", "earlymodern_door_ming_strike_plate_316", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6c885e7dbc4e95cf9e1fb09aa701df29b4145c0772445a2a1dbd561055599e1"),
		new("item", "earlymodern_door_mrd_courtyard_gate_255", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a93f2301125b429055a6bb0520d1635d8c42c6806b351b64a850cb36652d163"),
		new("item", "earlymodern_door_mug_secure_door_232", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfef3b0d879df82be40b1cf503ccef897c8d359ce8701406782d95fd38b1f0b4"),
		new("item", "earlymodern_door_mug_warded_lock_244", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f57f1c73feb40be3a99e2421a2c7cd9a5c1e75d3741c5511772f2fa100e4ef63"),
		new("item", "earlymodern_door_nl_good_warded_lock_36", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c324a56eeb5b1dc0f25d9c8d37c81475764fb3af48279039c17a2f8f7992527f"),
		new("item", "earlymodern_door_ott_braced_door_173", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac5104e89dad15e539e00279c3936287d28b9021256990bbbc245c5328329535"),
		new("item", "earlymodern_door_ott_hinge_pair_184", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "478f488e27f64f5aaa44faa15ef03c8507540b49a48acf159aa8af64bbc2b481"),
	];
}
