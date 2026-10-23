// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_locking_personal_bazaar_jewel_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "446d291ea23c13fd68b731ff3aaaeb9d9a731e6250c63d3cec80bb5e347cc4ca"),
		new("item", "medieval_locking_personal_teak_keepsake_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5281cd49309dad61f0c7eb62b6210b8c886532e7ce1e8888158f3658798c5b26"),
		new("item", "medieval_locking_trade_bazaar_cash_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e89969fcbe69e28f070c4673d3a6c22907a00b919f035fe5f26201e73c5428c"),
		new("item", "medieval_locking_trade_fair_takings_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcac796603511407e2d6ae4d8cd75bb306fd97e77d49199e5a9ea910510a9d7c"),
		new("item", "medieval_locking_trade_jeweller_parcel_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "60679ab00933d8471b8f639b1134fe76a4cbbf3f8c8963a140a93caea9da8cdc"),
		new("item", "medieval_long_wool_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8b70f90f94008fead45188dbf85d5d211eb093371cf3f769d28efbccd603ab1"),
		new("item", "medieval_medical_apothecary_balance_scale", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8eac26c0ef282f74e5e20b257d5be593cb1e6bc376c5e689e4b191c89a9d5ea2"),
		new("item", "medieval_medical_bronze_surgical_probe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d3ddf022698402cc911552fa7ea89af9d5f07d6d4bd6688d13b5e87c3ae2e62"),
		new("item", "medieval_medical_canvas_field_stretcher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "faf590a79519c37136466e226332940c398e1a1f62c7171c8a8caa623a68171c"),
		new("item", "medieval_medical_cotton_dressing_stack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "111bbd6606ff8c88b0b169b0d492c0c7dad00247ece1a94a2b08d11e21f0d2a2"),
		new("item", "medieval_medical_glass_cupping_vessel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fb6f71d9e72a7da993ac81d6d1a48792a8a5823f765277d4c247a959a4b931f"),
		new("item", "medieval_medical_herbal_burn_dressing", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "31e7460851822fb788d7459bff2c687d05844bf3e090bc0831239450947431c1"),
		new("item", "medieval_medical_plastered_poultice_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "be7c2bcac01aeb642f5be52bb0aa3b0fc899acebe81eca098b83bdd8a83e242f"),
		new("item", "medieval_medical_travel_tending_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5e7d1e9dba1974c3d77955e3e7caad2a6dea9ce75232ee16287b5dceb44839f"),
		new("item", "medieval_medical_wine_rinsed_wound_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3717816b7207f4bc4824e59655dba26a1198a9219f0d1ea531c62be7b11e4390"),
		new("item", "medieval_military_barrel_lid_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec2343bb2f6cf48c4753794f13acda03392e2aaeb9f37d0aa00c94277d105272"),
		new("item", "medieval_military_bow_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8556079a7622b29efb2c6555b59eff71fb5b9e9a4ed92c3176e78402179e04cb"),
		new("item", "medieval_military_braided_linen_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c84cfd425152219ac2c3dfba31ba413023acfedfffe4630d0d287cb90318ec4c"),
		new("item", "medieval_military_broad_war_arrow_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eddf81465988d23e60b94e934e4d7f4bc137d6c5b4558608fbf5d364b8d2679d"),
		new("item", "medieval_military_broad_war_round_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f7f57f9fdd6ae39e32acccfe6e0ff05cdd76472cb53292e1d57318014aa6d4b"),
		new("item", "medieval_military_bronze_scale_cuirass", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "61a6fba74245b2aa7e800f06e2eb63b9ebee4c3e349ed857a7d629ee70c47421"),
		new("item", "medieval_military_bundle_of_hemp_bowstrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "128c203c3ffb6e1c930c94499558a1bd68f6d42fb818b7aeca7ad8f2c9f74821"),
		new("item", "medieval_military_elm_self_longbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bddbe04a91c0167b43480e850a82f993d71e1058b946bcf53e4a2701d68fed9"),
		new("item", "medieval_military_fine_steel_pike", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6edac83b091cd45068769364576be6df289559d884cafc275a98c2a5b193e1d1"),
		new("item", "medieval_military_heavy_gambeson", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0811330f3cbb53c36218ff4e362cc450461ae84fc99bc08df3f8caf690caeda5"),
		new("item", "medieval_military_heavy_war_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f722318f18d1939329475d5a9d4862c09b9c409dc33fe30947f30ef7fdacbb0b"),
		new("item", "medieval_military_iron_lamellar_cuirass", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3532d0d5a86eda2fa946b61ecf593aa99355abab79925ad4a13d65e1f4df67c8"),
		new("item", "medieval_military_lamellar_brassarts", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc3605316f64cb4754ca4a96416da170ffe5d2549721e24da7a35215db135c73"),
		new("item", "medieval_military_lamellar_greaves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a157995c037c19d5c0ec94d73f4aca89bab629dc41997f9f809e57175dee5d4"),
		new("item", "medieval_military_lamellar_skirt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb108fca29a1119f40318175387ab2c839e7a8236895c7a24aa71353fff0e6e1"),
		new("item", "medieval_military_laminar_arm_guards", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8918b61e9255deb4243b09ac3f1ffe5fdd3c64a4cceb94aba5b54360d82ea1fb"),
		new("item", "medieval_military_laminar_greaves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5298609d1d00300f8f63bd02e944bbed2ca89666e629c32b7918817e19f7923d"),
		new("item", "medieval_military_leather_faced_round_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d9ad8672693b6eab9115bcaae800511ca3a52662726edee765da1082980cc6b"),
		new("item", "medieval_military_leather_gorget", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac34ac93a7883ae4510a41034fba94af4af1c7aba0ab4cfbf5f7355a597c93f0"),
		new("item", "medieval_military_mounted_weapon_harness", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "51f2e5928aef19dc706e69ad95777a58387228f15c5a6110b23700b19255762f"),
		new("item", "medieval_military_plain_shoulder_arrow_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d6c551f986d06c747eaa57a42fd439414216ab5498a31a6ad5c4ea46844cab3"),
		new("item", "medieval_military_plain_surcoat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "efb5cc46c568176dc881380d2304b3d9ffb7de583570636870dd60f06525c5a1"),
		new("item", "medieval_military_polearm_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e5d990063fb31b067470df5caac00eb889e084684624bdef252dbd4d177721d"),
		new("item", "medieval_military_rawhide_lamellar_cuirass", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f3bbe282062369358161100221ebfd5f641ae67c2d7ea38c6a53fc4a69c345f"),
		new("item", "medieval_military_sleeveless_padded_jack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "26c929a56104843fcffa8a930b553fcc56b0ef72b9967ff608a59dfcdc4b9fcd"),
	];
}
