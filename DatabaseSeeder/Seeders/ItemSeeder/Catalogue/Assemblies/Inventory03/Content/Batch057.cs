// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch057() =>
	[
		new("item", "preindustrial_trade_large_oak_hogshead", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cede33c78cba4aa2e4e1792b003bb750d08138b63b59cc940bab6623b1a80b59"),
		new("item", "preindustrial_trade_pottery_shard_barrel", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d22cea9f630bc248f24042f66ee919ee244e8d11829439005b790ea8dc9b9031"),
		new("item", "preindustrial_trade_river_cargo_chest", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "43f8ab7027e1b0fa2d8291c7fe2e069f41ed94096169220c39f52d16d44f80e1"),
		new("item", "preindustrial_workshop_lit_smelting_furnace", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:preindustrial_workshop_smelting_furnace"], ItemSeederOwnershipPolicy.RequiredRelationship, "fcc4bdbf100d0a916b5188329fc2c2b7685e306e6743d6a418e90613b53eb538"),
		new("item", "preindustrial_writing_blank_parchment_charter", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bac92dc0b8e04bc68cf7a4aa875f6c84d5fd63899a368dda4f7bfaabd75905c1"),
		new("item", "preindustrial_writing_broad_reed_pen", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c20edd95f1778406a4fe4f28ea10cd0eec50a9ec2c4aba79d111cb5bd6b517a1"),
		new("item", "preindustrial_writing_east_asian_brush", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8768b281fc456c7ceac860c5b2c68fa523a03a1063a1f6d8bd9c1c160c05f85"),
		new("item", "preindustrial_writing_goose_quill_pen", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "da9047e791f2df381fe97349d748e5906f376f11995c4b5e6970fae071101a1e"),
		new("item", "primary_production_bog_iron_deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7ae6584f7a0e18815ed3c9357806784adcff6eea774617094ee9f347db83e16"),
		new("item", "primary_production_native_copper_deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e555a18c6605a90af5682a0805ab4776985f746fc05f9c8a3f93f70df65d7230"),
		new("item", "primary_production_ore_washing_trough", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d527c07c4f4cc35173d95abfef130cf6f226e3a5e90b56e614e10dffa756ff1a"),
		new("item", "renaissance_andean_pinned_wrapskirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "29429d7aed05863c0fdd81df4fe5e0d6ae68b3f38e6e74aef98721d8f37a427d"),
		new("item", "renaissance_art_granite_muller", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "438b9a367a6369fb2e93d19668669e02d1e8c37ac9f1a4ab1483b31043b31428"),
		new("item", "renaissance_caribbean_gathered_rawhide_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "436a2f20258dfdd66cce52020bfddc791a7561748703d5194571d34e302be739"),
		new("item", "renaissance_contact_broadbrim_sunhood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9983f8fd2cadaca602f95820300a3ac467edfe846a9bc9ec15674ba523adff63"),
		new("item", "renaissance_contact_hybrid_buttoned_wrapjacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d509aa0c84655283121bc059797768eaae3810a8d4bc456dd7f36f545b1f938f"),
		new("item", "renaissance_container_aca_domestic_household_hamper_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a330ffd041a53f4174c87c05c87273dce8b9a918e1dddd3a70862cefa39673c"),
		new("item", "renaissance_container_cen_trade_strong_trade_chest_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f230994a682ecbccabad9f94425c890336932c1adfd309e931dad4f0aeb74c4"),
		new("item", "renaissance_container_eal_liquid_household_rundlet_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca500485ae00f1ac694c8f112090d0a35cebeec073c294cd97d5fe7992ae2517"),
		new("item", "renaissance_container_jpn_liquid_pouring_ewer_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "da4496f910c3864a5ceaa41ad0a78cd867bc36dc4ec4114c4afc4b623e20b7e4"),
		new("item", "renaissance_container_mar_trade_small_trade_keg_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbe3436827d59cab77916cb87919f96e22895f978feea9876d5c643bb15d7fa0"),
		new("item", "renaissance_container_mea_trade_deep_trade_hamper_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d108cd80d79865651ef293f6ab4e5b16bbc2763ac3205cd671baee26020fa9b7"),
		new("item", "renaissance_container_nac_trade_fitted_sample_case_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0382f3fc283e49bead867e17b6da68c0fdeafc94cad76510138a07b2cf32570"),
		new("item", "renaissance_container_nba_trade_locking_trade_coffer_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dd68777b77863cb54c869c0c578af99a97d68c5874bdb3d08d42e8400921d9c"),
		new("item", "renaissance_container_wer_trade_locking_trade_coffer_05", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f2e809fc708b531695b8d842f8a3caff3f73581fd4726dcb2f827e66bda5f9a"),
		new("item", "renaissance_domestic_andean_storehouse_pointed_storage_jar", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b77b02d6d109a43cdeda8567935f08a5a81314be3616a379825f732307fcca0"),
		new("item", "renaissance_domestic_ottoman_bazaar_brass_tray", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a480b02ec6e4deaaa9e587e73687a07436b7f12a36a64be282de3cda0c85663"),
		new("item", "renaissance_domestic_shipboard_purser_oil_flask", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "593a52e241787273f418252d3259a2d3e7a70aa3b1e8294b3fe6a3d1470f3070"),
		new("item", "renaissance_domestic_steppe_caravan_copper_cauldron", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d36b294b00f70dd22c6b048d5554578b4f38b79bf5b1e81f18c9af5e5480c8f"),
		new("item", "renaissance_domestic_steppe_caravan_horn_cup", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6298550b5f396492ea4684685ebf50b5f1043ca6f17574442277d1a08af79e3e"),
		new("item", "renaissance_domestic_tavern_inn_ale_pitcher", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce91e760f9a684a4618260b5fa2b383cfc21bc521684e25c7035e3a253e282f7"),
		new("item", "renaissance_domestic_tavern_inn_service_tray", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a39fab1ee4c0a98531605e32ab04e30a7d69a4bd3c8ac5f008dde98d777a971"),
		new("item", "renaissance_domestic_urban_kitchen_pot", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "daadd01986cc2412f36e870b15fdb9aee5a33567afd34a8aeb4082b4d1340fff"),
		new("item", "renaissance_domestic_western_counting_house_brass_ewer", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c113240c08cfa0875b7c9a5812d18ece17b1735dca22c49500ed464b692fe26"),
		new("item", "renaissance_door_car_barrier_600", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9236acd1033d7550ac0405355bd2616e395b0c05c4f122b7035ae8c324d778c2"),
		new("item", "renaissance_door_cef_hinge_pair_140", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd79ca03d2f944614c6990ef216620db2ae1d8817c5b22ffdaa365d78fab5723"),
		new("item", "renaissance_door_cef_panel_door_127", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4beb297641ac90516e1134908c58f8861ef92fa2548cf0b2d017c48a8e383089"),
		new("item", "renaissance_door_col_grille_655", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e9d25379113802c905926f39dfec51a3dd5d66e380d448e9e40751c309f222d"),
		new("item", "renaissance_door_col_screen_650", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "65131dd30be15025cd4485b41661e604b218b2cc1135fd9d22ae01b56d066432"),
		new("item", "renaissance_door_common_braced_door_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eba1ad4c32d00823672304e73a2dfd168588658f3b9f40d6a81e80f3e7e2c178"),
	];
}
