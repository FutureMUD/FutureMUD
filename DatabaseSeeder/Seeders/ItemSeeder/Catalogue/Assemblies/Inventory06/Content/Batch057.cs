// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch057() =>
	[
		new("item", "primary_production_brick_clamp", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9b547809c289d8470b8c36f9da482fc37387e37280ac61f231deedb303bb51a"),
		new("item", "primary_production_shovel", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f3f36ee8a3ea613ad81dd398b57e5b78a4187eef07e67067f84fafc1399f50a"),
		new("item", "renaissance_africancourt_sleeveless_straighttunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7cdc290e1f6e3037384fb4f1d8a0917d56ec60d59c053ae1f0f430c652aad88"),
		new("item", "renaissance_africancourt_tall_wovencap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "26c4ad572479aa231db35ebe1637a04b1c1dcb0ea6d1dc0e578eb745f88e8847"),
		new("item", "renaissance_art_carving_chisel", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8826722e8e37271662ee02d4dc19e1ed8bc438faffdf8e62e8501c93e6a0cfca"),
		new("item", "renaissance_art_plaster_cast", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2bd8adc559fdb40fa3d34013a08f6c3575d3b67c4d74c057f33ce923274efa9"),
		new("item", "renaissance_container_eal_liquid_small_drinking_cup_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e16674d93efd73d22f771d46856cfe6bf705cbacc9936ad2f4e727c79af58926"),
		new("item", "renaissance_container_eal_personal_portable_writing_box_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb5312a9e4546fd80c4123bf45401600cf5bbe0f939177dc72d0249bbe6e5af8"),
		new("item", "renaissance_container_ind_personal_back_pack_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b4a11b453d2d5b855a8a699f1ce268258ac42705fe55a8e4205a3f596464f3d"),
		new("item", "renaissance_container_mar_trade_large_trade_cask_09", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1cce9dbb38e5e10c8305a2c09962f7e88954b689b7bb923f3fc69b7979e8c330"),
		new("item", "renaissance_container_mea_personal_portable_writing_box_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5af956eb9c997502016efe7e816c0a97ec5b91bb09144c104d5914c5bf1312c2"),
		new("item", "renaissance_container_mes_personal_document_wallet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "88c897a81a7b3be87e658aaafa26b56afd41d1a6630cb9938a86401174337d41"),
		new("item", "renaissance_container_msea_domestic_tea_herb_caddy_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f65f369936fb9f60e2488a69017b83dcf86fe8363c852bea6ed5caac61a0b317"),
		new("item", "renaissance_container_ott_personal_fitted_belt_case_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49a2253b7100aabd472a036c5ab5c9fe51969b307ab6f94fffb4ace088aaa8fc"),
		new("item", "renaissance_container_ott_trade_large_transport_jar_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "893c4f2355089fe967ed974bf98a3adc3854a39f52a377e3350dd5fa9919959f"),
		new("item", "renaissance_container_pip_personal_shoulder_satchel_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "136a94a91a3e740230dc7c177fc2af39ed93e08846376cb9685c2747b9ac75c7"),
		new("item", "renaissance_container_rse_personal_sample_roll_case_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5defe3e155625800ae734dec671417eb04d096a4d6d19dd6c01a2ed07d35be61"),
		new("item", "renaissance_domestic_apothecary_perfumer_ointment_pot", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f29d41fc9c2bf1e28009c9f56d71074b0fa5445dd8977261ef7091e3592428b2"),
		new("item", "renaissance_domestic_japanese_ryukyuan_tea_bowl", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5ed3cb47731a17fca1a9273966d6be9a3b82ce127705aa44e7aeec9fe3435db"),
		new("item", "renaissance_domestic_mesoamerican_market_cacao_cup", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "03e5a86cc569b3768df0b9d4f2ac05c10f5c17dbe897af5d565855f082f473a2"),
		new("item", "renaissance_domestic_ottoman_bazaar_long_spouted_ewer", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7be4cb7215228b8f2c87f5beca427ddc7a1b20a8b9e402d6fcf902d0013020e0"),
		new("item", "renaissance_domestic_south_asian_merchant_dining_tray", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c53ef0c21ee6a1cf2515f21eb7b03155a84a7eb7b1e11311581bbfd8a64e16d7"),
		new("item", "renaissance_domestic_tavern_inn_pewter_tankard", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9d62842fc7192beb774342e5cf70aba5a769f527228f40118c287709d57e99d"),
		new("item", "renaissance_domestic_western_counting_house_pewter_plate", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ea676e58a973f69f9fbd165a9773a8f4b2ee848a9198f9e3e0e9135756d2f61"),
		new("item", "renaissance_door_and_screen_566", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "714163afbec1db9da4e16402823f6024c350cf057c9cd805873ef370eb37f3df"),
		new("item", "renaissance_door_and_shutter_565", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5edce84c84942e7ecc800ba5046da57a69b992d4b6c9bc837e79a74ddfea2407"),
		new("item", "renaissance_door_car_master_warded_lock_606", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6faecf77baee6c1db1d6cdd16164f4304d076bf980d975adbc1d32176d1dead6"),
		new("item", "renaissance_door_cef_strike_plate_139", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0451fc2e1f3726954c88cfd4d792c2b863de870b61f2d2304bf4d1f657f042e"),
		new("item", "renaissance_door_col_door_latch_665", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e58ce684c4a8ee7cbf248c5a15bd643e46843ef754b8ee03101d936e2a7db587"),
		new("item", "renaissance_door_common_courtyard_gate_08", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e307f88834c82144a4afc3bd1d777a49109dd126cbe79b904541c7de2a3b2013"),
		new("item", "renaissance_door_common_grille_53", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5167a0f9f62fbd69ba7cb44eb21eed2cb0883b0dfb5ce3111dd1e32f9e692e8"),
		new("item", "renaissance_door_common_hatch_27", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be8156124cf0374447cf45b5861cb420ad5fa87a332097fb472993277006ae81"),
		new("item", "renaissance_door_common_hatch_41", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c1807c841ac52ccbaef87c65a083b751660eee182220567b8f037d03c18c794"),
		new("item", "renaissance_door_common_keyring_194", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c1be421461e525916bf828df433258458c0ca4e932b2000ec908016ae5b667c"),
		new("item", "renaissance_door_common_panel_door_57", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36ebaba96ea15e6984de4bb80029e76da4ba6d8c39a7db6b723928d00fecc472"),
		new("item", "renaissance_door_common_screen_76", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ce182c872d4551dbb831797b4c834d9e511712e59aacabb364b5ae913410e8c"),
		new("item", "renaissance_door_common_secure_door_74", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76a782253bc36c7ca2d29219256274c4f5d1f4cd9ef1a20d1795af5f7f5aa4b7"),
		new("item", "renaissance_door_common_wicket_112", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "14b4f991234e0e9c83647a6a5040a86f4c60477b2850d235fd52d1810a577486"),
		new("item", "renaissance_door_common_wicket_84", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3f3f5f81d08fffa7fd45b13c4da0020c21c6117ec02e0260c8913346943af8f"),
		new("item", "renaissance_door_eal_grille_263", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "834823fd7541d02b939071b717076a5fd8b88a2c2be164521c9214cb1b0c3325"),
	];
}
