// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_door_and_strike_plate_639", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fed6710f0fe7c50029b879e76a2de1bab5c956bb6ae70a97d56c3e2c2fc988c"),
		new("item", "earlymodern_door_bra_strike_plate_563", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3235e94256f28592d2961a6618cb10b59f4c5ec5054a6af61087422f7209b487"),
		new("item", "earlymodern_door_car_braced_door_648", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aea1ec6e160471dc0bafb08a99b4f6937f72cb32ea78efc612fb903bc33bbd1f"),
		new("item", "earlymodern_door_edo_good_warded_lock_359", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "499437900a36065be6cf5f75b1d38a1064ade800ab2523bdf01621e3c800ac10"),
		new("item", "earlymodern_door_edo_lattice_gate_349", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6ba3f5a840919e391bc8d3299131f9bc8bf51131e4e556d986b4e4dcd10548c"),
		new("item", "earlymodern_door_ena_braced_door_572", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa5367f0c75d57e6d92d86878735a28016f5e2f414bbd60aec51612eea1fc390"),
		new("item", "earlymodern_door_ena_hinge_pair_583", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "877eb96a67e7017cd7509e4b5042f79741b7ef3fd55fc925edb581a4d3da6653"),
		new("item", "earlymodern_door_eth_warehouse_gate_503", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "193ee1ea57198d8140ead24b19e276caa069e5026f7284d3821c0e3fb5c2a424"),
		new("item", "earlymodern_door_fr_hinge_pair_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf8b5dbda803ed08ba25545014f62fa7813f8cfff0329c1d3fec00d4c5d90442"),
		new("item", "earlymodern_door_gmt_grille_676", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d2a07f3f804bcd1b52e05463911e8818ea66b307cdd899aec1c6b5eeb97b9e9"),
		new("item", "earlymodern_door_gmt_warded_lock_681", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "89009a280601d784a4724c47f769d26088cdd069f7428413f9b8e6fe6ca193f5"),
		new("item", "earlymodern_door_hre_garden_gate_86", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec7838d0788f18ae3cb0c50e1cf0d87dd45516a2667f1259eb9151033daf7a45"),
		new("item", "earlymodern_door_hre_lattice_gate_83", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9ba0af90f69bafc1b0f6c0869dee575c97a86e444c1e25a0ad4f1cf5f60da86"),
		new("item", "earlymodern_door_ias_good_warded_lock_435", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad2fe8146dfa9b0fa7fb395ba0891238ee245faa95314a912db4d5a1d2627c95"),
		new("item", "earlymodern_door_ias_lockable_door_421", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "901391301867488c377d2b5c70412358dd7f3cc04622b1f0e23893c76cd9a39d"),
		new("item", "earlymodern_door_ias_panel_door_419", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc4d54f3dbe66f36af883ab2fb364aa410347272dc47ff4f86994789b06f6adf"),
		new("item", "earlymodern_door_ib_secure_door_61", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ece6b93dcc693e079f8beb1e86b7506115953ea7bd1faeefeb94e126ec5d43f8"),
		new("item", "earlymodern_door_ina_lockable_door_592", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e24ebcbb13923a322580085c2a46c116d604c90b93293a51bcbafd1dcde49285"),
		new("item", "earlymodern_door_it_hinge_pair_108", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f1ca00130350eca0d8c64cdd4c72a1629e00f742898169f1f6ae96e9da6e5af"),
		new("item", "earlymodern_door_it_lockable_door_98", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4164b5a1f856270e6a54f5ae79d418a3b09e0881b952fa16aed53936720c245"),
		new("item", "earlymodern_door_jos_keyring_337", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fa2008536429aa92c345310d298e1be1d987c2fe6828a5251b50b9063734527"),
		new("item", "earlymodern_door_jos_master_warded_lock_341", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2e3e2ca2a63ed425bf534c3c8dc17bd59422154561c19fee7ec623cd80c26e3"),
		new("item", "earlymodern_door_jos_shutter_328", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2035f57b495600741051563b39b4404a532ec156b792db291b78b4211fa8cda4"),
		new("item", "earlymodern_door_jos_warded_key_342", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "adb0c1f2aec2a4931a76a017183952dc93d862afc0c33f4861aff8a4e1a57840"),
		new("item", "earlymodern_door_mrd_garden_gate_257", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d924a526ff49ba26349cb6ac6a513a632e429cb562bea9b7c2f6af235c70ad4"),
		new("item", "earlymodern_door_mrd_lattice_gate_254", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e54dab40274d6285ef6486a76858a36fc66f8d3a28e7400f1865efe4bbd2dec"),
		new("item", "earlymodern_door_mrd_lockable_door_250", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "76041cfe02e6f4bd9975e94990c29b0d6e45bed49be47f5b6851adc220797fe9"),
		new("item", "earlymodern_door_msea_warded_key_418", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "46c11b1a03faa1380f76259204abbc0235c8f27852fae0805e85b2cb9464e0ed"),
		new("item", "earlymodern_door_nl_warded_key_38", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "90f1ea6f4e36574f9baa353d37d87e1dd0f1061ca2ca988209e18c4ee018a7e3"),
		new("item", "earlymodern_door_qing_hinge_pair_298", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "35ea5c4ae38ae360cee99107e146effd80eaf02543bd78708ee202140ac067a8"),
		new("item", "earlymodern_door_qing_panel_door_286", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "50476850fba6329157eb3aa29f75b30d277959a094acc77224305a7c4a6169e4"),
		new("item", "earlymodern_door_qing_secure_door_289", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "516ea16305b9696da6dc9500a930733ebbcf393e9f6c79b030bc5a9f0986d957"),
		new("item", "earlymodern_door_rus_braced_door_154", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dda4be2a67a76b2f66bccdeaf0cf93a12405f9ba9d33cf1d08fa59b68496e990"),
		new("item", "earlymodern_door_rus_hinge_pair_165", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b96f6955b88881a95bfc16fa63333daf051cacb720382f53f72f5515065ba31c"),
		new("item", "earlymodern_door_rus_master_warded_lock_170", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8784411b70a8207ad10c2d34267ddc8f745d60e0b4ad4a4ea33dd51a30983cd3"),
		new("item", "earlymodern_door_rus_screen_158", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "655acf3b1c42537278a4f6b83f23c01a41c7b32b97e4d7684eca32129baff66c"),
		new("item", "earlymodern_door_saf_keyring_223", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9df9d445bb8ae6c8bc5abd738be2bec3e6422813b12d075ecc5f0063540e470"),
		new("item", "earlymodern_door_saf_secure_door_213", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d5f893b962699d843984003c18adc722720b18148d6f722f08c5bca111f46cb"),
		new("item", "earlymodern_door_saf_strike_plate_221", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "55011f8edec5b89ad8a9ec6151c24fd3abe2146f4cc7c66a5d173fed5095bb6d"),
		new("item", "earlymodern_door_sahel_screen_481", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4fa55004f34607eb5a97ce454a06ce1ee0ba1519e06a9a93be183f9ae31030f8"),
	];
}
