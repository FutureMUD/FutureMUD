// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch028() =>
	[
		new("item", "adjacent_antiquity_tall_felt_cap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a34e9f16c1ed2a51e5bda2ab0823335e7225321b10ab0324dfb1b96626133b26"),
		new("item", "antiquity_accessory_common_tall_arrow_quiver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d03fcea21dba4036105c3c01fd7b2a18d84e4afdd076bd684be0a38aaf3c04cf"),
		new("item", "antiquity_accessory_egyptian_khopesh_sheath", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4d280f3c39a31ec8da982e6c3e94f1c22f2fbb87e66460ffcc03121e96b30d6"),
		new("item", "antiquity_accessory_punic_javelin_quiver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "494ef7197797c125e679a12e870a9bab9bd1fcffb73b7d5d44c5bfe3ffd4b92c"),
		new("item", "antiquity_accessory_roman_plumbata_pouch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c120f35e9f371fcda9c1973b63897335f097614e018b59aa7edb0ad2ded8d817"),
		new("item", "antiquity_alabaster_display_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "08c30e76a208ea191b9019add3dc2dc1619992afe90091bafb06bda67a33a7df"),
		new("item", "antiquity_anatolian_ionian_bronze_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "73461bb3f3cbf1d7d98f717e45aa6fbdde3186d1461ada152c5eb739971cb660"),
		new("item", "antiquity_bone_knucklebones", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a4a66ea3cdebbcb0cefeb3514c5a5d96eb6f8db3d4b26f5eb96f7a3b5d7b94f"),
		new("item", "antiquity_bone_suture_needle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b833c902b8821dc4c45f010314a64501d816e1d7ec7adffc7abe6c6d667fd352"),
		new("item", "antiquity_bronze_anvil", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "23bb691b2f7846f92654f1f5f348b81710d85fd9bf85be76bb213ff4cb246400"),
		new("item", "antiquity_bronze_bound_chest", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "14a4d57db295d796dbc65e28b5d5c02b6def547558010953eb9b91d150b3a4a4"),
		new("item", "antiquity_bronze_fountain_spout", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c74fadacac57484e7be63e3de95a05dc8f22b686c118bf67586c1d5e4dd26b89"),
		new("item", "antiquity_bronze_leather_wax_pot", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbc35269876f51e9ef093bccc5cda1c7b954ec95ee63ccac3b5c66a45bb5524a"),
		new("item", "antiquity_bronze_warded_door_lock", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c164f6a2297174c9967aa60017346218589fe9348986ec743b4b335b75e1b540"),
		new("item", "antiquity_celtic_bronze_studded_leather_belt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "317ac58df7a6252a568f3d709bc5f407635338d15dd72868a06d17fc2615895a"),
		new("item", "antiquity_ceremonial_temple_key", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c258a1b09c365f0e63fd64967871d01afc7ca45b676eb12483393dcfc9abc89"),
		new("item", "antiquity_distaff", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b6a151670cb45c546c79f1fb4ba287b51bbd1d234ac5dae1b83945ca1981aae"),
		new("item", "antiquity_egyptian_leather_scale_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "97765b89eb3f8166f19d3650ac55afd8e359237d1c04668d59d9a76522a01753"),
		new("item", "antiquity_felt_hand_mitts", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f79704493289015ab511de26680d94aeeb8e3cec6305b2b36a955fe6426a2651"),
		new("item", "antiquity_fine_seeress_wool_mantle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "259320e81718f403f03279cef634c0f01a8ff0b3eb1dc3a0c0c5e77e2a4e1850"),
		new("item", "antiquity_food_egyptian_porridge", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "01d9ee7628f87412961ce103c32298ebf86f8b4b43ba8678280bc4f2bac602b9"),
		new("item", "antiquity_food_etruscan_sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "03b08025d5c163cbc83ef38912d715157961f9d1b730f90a938645015b5583f9"),
		new("item", "antiquity_food_fermenting_kumis_amphora", "antiquity", ["antiquity"], ["item:antiquity_food_finished_kumis_amphora"], ItemSeederOwnershipPolicy.RequiredRelationship, "59f15cf432524a433be16213dc9b9805b80f9e173baef0beb0b9a412510c90df"),
		new("item", "antiquity_food_finished_spiced_wine_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "26579b4dde6b64b1abdb292003c26e165b75389111b3d965d111bd28f4056ace"),
		new("item", "antiquity_food_germanic_stuffed_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "abfa21ec8a13175464531667e1c1d369f222f856f89d816afb833f5a6ef420c8"),
		new("item", "antiquity_food_grain_sieve", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1c93b505d8b9a1f128611599dd70bd88be916c78e0d9ed851ff08af1907c93d"),
		new("item", "antiquity_food_kushite_sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "80e0efec5d3fcfd9b12a3066c55783de8400ace12173846d6e737b0d6ae2572f"),
		new("item", "antiquity_food_roman_sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "af02c510b006974716231c982dbe4e385bc162575b90d081c3ca51512d981343"),
		new("item", "antiquity_front_knotted_girdle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "04bb2e7dcbeba9b2eaf78dcd56661e59f4d35fc4cf353d50b07dfe7e28afa7d9"),
		new("item", "antiquity_fur_door_hanging", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b4e2ff3dc2be41dd5df18ccab6a2958da8ef60e44803dba20ba26ecb20617d3"),
		new("item", "antiquity_germanic_padded_wool_undercoat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ea289e2bb6e754b46a0e812b36d2a625971a1845c97ece15634cc8e06c6a902"),
		new("item", "antiquity_honey_press", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4d7c61ccad86d52849143b8f3616aef91d06248fb425afac0d833f73026430f"),
		new("item", "antiquity_kushite_sand_armoured_sandals", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "054fd86720a806ffdf0c3f9a53800d3b784ce0ef104ddb0c1a302241679acadf"),
		new("item", "antiquity_liquid_bronze_libation_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbd4a926a1ecc79f0f2bbaf3b978fe0c93eb270075e66dd6a047a5a21c1b4288"),
		new("item", "antiquity_liquid_sealed_oil_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f27031157251d379b3979db886abb8fcd2c028be9f893f8f8ec02aa3b5ba738"),
		new("item", "antiquity_long_folded_linen_robe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "33195e435f43648ef2eae882cfaa49710e0a4698fc5cfe280a383d63d71e0c7b"),
		new("item", "antiquity_market_counter_chest", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a9a6123b0789809f2338494151cd13d8f66a74a55a57af19ec7fc93d00e8933"),
		new("item", "antiquity_medicine_strainer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "abb17e9ab005bf3c22f9ce963f666d2b804675352b166101e5c5afb0a03ba241"),
		new("item", "antiquity_narrow_display_shelves", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "571b49bdaf3d4304c0c555889744d795469581aaa759925001bf8c231e9e0afb"),
		new("item", "antiquity_painted_clay_eye", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "71b7f6a0529662d53cefc80eabad5be57f096cc0c103e479c136d656075cff04"),
	];
}
