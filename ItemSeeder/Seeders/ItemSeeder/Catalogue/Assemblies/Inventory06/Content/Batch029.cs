// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch029() =>
	[
		new("item", "earlymodern_container_gmt_purse_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3a113aa6825a4a2023b6baa427f51b3902b7e7f167b73cdd6557e675b52e595"),
		new("item", "earlymodern_container_gmt_spice_basket_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6c8b026aecac7db13d6fbd5526cb6c4fca28f6bc0d4a196a5c47508a54e4c41"),
		new("item", "earlymodern_container_ias_tea_caddy_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d82a1643bc4c1da9663041e8e1e4b00f2753de67793fd0c2bd54bb8f5a2eb86"),
		new("item", "earlymodern_container_ita_tankard_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "316392091046f29c464586b86c193ef5bdd02f07f76f547c7393e9bf652cbf72"),
		new("item", "earlymodern_container_jos_punch_bowl_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "91aa84267ed31eb47e4a89450f4be1bc55168f1e67daeeeb21bf61743614988a"),
		new("item", "earlymodern_container_mag_bottle_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e3f5bcfc269907c56e7f31489d48f7309359a48ab0978302a40f9f0faff579f"),
		new("item", "earlymodern_container_mag_tankard_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "551b1f1b420cd06f3078b356a702c055108bc3f7301914bfc860c45512e5fa7a"),
		new("item", "earlymodern_container_mes_linen_hamper_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "be50c39a8ac7dd7459b7738ddf16c465d242d0b9c007b3bbbaa9c5f9ee9fb51a"),
		new("item", "earlymodern_container_mrd_storage_jar_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2fc67f8ecaa42bc68fb905954b40507797869c30ab0cb2cf386ace19059b8e0"),
		new("item", "earlymodern_container_mse_account_box_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce5b5d9d19e56a21795b9f9134dd10dc178d99a0a4068b4812c9fe8afbc5a59d"),
		new("item", "earlymodern_container_ott_punch_bowl_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2485f442c69c1040f30be9c3ae19a0f33049b6ec9939359b5beaaaa6ea91bb52"),
		new("item", "earlymodern_container_ott_spice_basket_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "71d231d990b4dd87d0cfb03028a977f51899dfe50d43d9e3daf31185ef7f09da"),
		new("item", "earlymodern_container_ott_tea_pot_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ffd3692581b4720592c90d4fac8eb4d1c12ac1ac009dba07c6041e2f6549a5f"),
		new("item", "earlymodern_container_qin_document_case_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8087186f7fd91e04bec1eb3383374cb7e6a240646e9ab3bd9de621fd6fa36a3c"),
		new("item", "earlymodern_container_rus_lockbox_29", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "748dea486b478923fd2aaf7b26b1d0e74748954dc9a30bcf0b34a89d7a40cef9"),
		new("item", "earlymodern_container_rus_purse_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "12718cee2a6f9216d1e30ac85ff79d74b27322f1b04f97cde652535e3389d3dd"),
		new("item", "earlymodern_container_sca_ewer_27", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "71112248bae8277dd76f1ba0ab74a0322b4ec6ae99c9057e1a7601c1b43c217e"),
		new("item", "earlymodern_container_spa_purse_31", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4902d03fdcf39362c62ed27e3d6bff7c3fd460e5cbce3a8648803f6b8a87bc5"),
		new("item", "earlymodern_container_waf_lockbox_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "39df1c4322865463cb380ddc7711cf569e8fc58313dcc89bc53fcbcaf30d20e0"),
		new("item", "earlymodern_container_waf_tea_caddy_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb22ed3ff8dd941ac7a1e0a2412c0154b790a386a6f78147ea02a9e77b10904d"),
		new("item", "earlymodern_container_waf_tea_pot_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ab464c71c86f5eaba08067e65f9639ba5db247084da2103a3eccfe4738b2f2a"),
		new("item", "earlymodern_deccan_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "93c6e64a52830d32e61714d73c1e8af3c745e7e6017a8126692385c206cd757f"),
		new("item", "earlymodern_door_and_braced_door_629", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b9de5481f8d9306b048f56a28a98f0b7164b1eebab8733c01f172fdb798e4d6"),
		new("item", "earlymodern_door_and_courtyard_gate_635", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2123c1399b98bec878e40f6b3815012ea8d0169c3f5e2db526601abf1191ae3"),
		new("item", "earlymodern_door_and_lattice_gate_634", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b625c896bd6d3bdcfbcfbd77d0ccb0bbcd7198910eb8deb99351588aab91052"),
		new("item", "earlymodern_door_bra_garden_gate_561", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c3e00e195a675734c49bfa1b116c37e58520548674bec417c51c6775762c467"),
		new("item", "earlymodern_door_bra_panel_door_552", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "161fe3b95eb7e265ee7bbd4e59676747b70a557239ab3109dd9d1cfb27b28e7e"),
		new("item", "earlymodern_door_car_panel_door_647", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdfb76754947de71d10b8efbbb693cd852a1a42fbac1bf6797a745a067907f57"),
		new("item", "earlymodern_door_ena_shutter_575", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba2a83fafac281b77532183f0551e61f3b434d6e351118ff7d8db1069f0d340f"),
		new("item", "earlymodern_door_ena_strike_plate_582", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "723b46129d272060731b115d7ac87a38219f6954363b1e3a539c4058eed58f8b"),
		new("item", "earlymodern_door_eth_grille_505", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "577e60db3eac426fcf47e746df3f44c64ebfe36719a7ca61f63425fe25bce06a"),
		new("item", "earlymodern_door_fr_warehouse_gate_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "23cc940516174d3136b9f4531145359b089ff6c6ecca7f22c635d5b1ad050d0f"),
		new("item", "earlymodern_door_gb_lockable_door_41", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb0c40d93d37dfade11a317b16160627cca100f129373489e12b759948d47f37"),
		new("item", "earlymodern_door_gmt_screen_671", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7701177dcdc278e1008ac668b1be2a01716b07b908e9b9240d6afe417f9f7ac6"),
		new("item", "earlymodern_door_gmt_shutter_670", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "192377d7f4f60aa680ad05eec0b5f6a7c58739d846d00ec82f86eaa19e113f73"),
		new("item", "earlymodern_door_hre_warded_key_95", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a9bc9563c3af7e07be9f48dd8349f58ee81d0dfe89d33ba0a07b38a49419189"),
		new("item", "earlymodern_door_ib_warehouse_gate_66", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1013d88ccdf359b8ca5e0650185d6f92a808377c28d1ef64dbec52f3e1c936a8"),
		new("item", "earlymodern_door_it_courtyard_gate_103", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e98c58bd2bcdc84f30b6c4125bb2a0b1bf2fbe4ba2730ee1bd0231152c2651cc"),
		new("item", "earlymodern_door_it_good_warded_lock_112", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca5773cd6a002eab3bcc834af3149cbc6313c7a885826ff41e7783dd63deef4f"),
		new("item", "earlymodern_door_mca_screen_614", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a4c06cc25d274f699d497a90913d4df4306095af405a11ef250e99ab96a7223"),
	];
}
