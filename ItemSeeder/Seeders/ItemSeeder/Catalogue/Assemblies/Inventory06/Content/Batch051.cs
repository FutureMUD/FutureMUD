// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch051() =>
	[
		new("item", "medieval_military_training_war_hammer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d59a9184c221b43e517bfe89e3a55fc3c91de43808bade125843a3f513f318d9"),
		new("item", "medieval_military_worn_war_hammer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "90fa96bf8e2713d124328343eba26054a8df8cfe77a7c74ed9b199e4a82e877a"),
		new("item", "medieval_norman_split_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5f153071af625afedcc6c2ac15adb9a606b6967ca483630143a1b4dc43a3cd2"),
		new("item", "medieval_northern_blot_offering_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "53e1d462d111bd139be2fb0097110189a0b1ca41218c600c5eb3db648159001d"),
		new("item", "medieval_repair_fine_metal_tool_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "751507434d731df8de296d83d45c2e20166457b6570cf80b77df1fd954461818"),
		new("item", "medieval_repair_metal_tool_repair_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "280eb4443c1260e524280a481759ec3259004a4fe6b1a0b89e1a16f93125c77c"),
		new("item", "medieval_repair_pot_menders_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "432737fd4200bf479f5cb3094a072acd84c969e1c90cdf7b8fd5db693d28238d"),
		new("item", "medieval_rus_poneva", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca07ea2bb3a7648bfac518db632496612c034eb4e008e9cbc61b2b3eb1cea841"),
		new("item", "medieval_rus_shuba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "55eb4d5988a814466fd46de648d86c9e5570026e8fc75ebc6d0e22963bee772a"),
		new("item", "medieval_rus_sleeveless_overgown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a72e240ec5b9987170e3c9b831d5b310dd17c2fa817149b8903b67f4292f718"),
		new("item", "medieval_shared_barrel_bit_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ad61661c107823a5f37003b7f7352618c8ad8e46882dc5379145f14c4f614f9"),
		new("item", "medieval_shared_fine_brass_warded_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c99e23905a6a878c2da499fa12486ccbda535ba366f3893837979145131ddcb"),
		new("item", "medieval_shared_fine_chamber_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe4a40db8fc5d7f469a824ac08ac51deb8e8f6b80a20e06ba89878f971a266e5"),
		new("item", "medieval_shared_large_keeper_keyring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e09f07706a116d7d9c1eb0b6695481d3f32ae10f6a706d52bd65f972b05a88bb"),
		new("item", "medieval_shared_rust_spotted_warded_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7e84bfffd882e9fca1742a8880304ee519c4d8bea6359fe184013d52876efe5"),
		new("item", "medieval_shared_shop_shutter_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "efef282036bbc84a597b66179a5b93619ed5fceb961112c544843719f5db9289"),
		new("item", "medieval_shared_shutter_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "38cfab04029ac8f104d4f232f0bde8c6ccb8e7cb4f12640d542f8fcef9b19863"),
		new("item", "medieval_shared_simple_iron_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f351848c205accfcee497e20f32bf6c8662c25e10c668234631133954407dc1d"),
		new("item", "medieval_shared_small_chamber_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "01c7672f6ab6dfca13bd2a177a28d36a391ee9e008e9b145fe71aecdc4ce9e30"),
		new("item", "medieval_shinto_miko_white_kosode", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5172f51d20ac94da8c5d3fc0e86bf8909509fdbe1529731b7dfb882be3ee6109"),
		new("item", "medieval_shinto_sake_vessel_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f50307dd4fbd14e6da8180653efff5db18c5fd455870088af2daddef70501c4"),
		new("item", "medieval_simple_bronze_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5e74266f4062b347edc6d012628c707eb816bf922ecb2c3099028d3f8033c40"),
		new("item", "medieval_song_merchant_headscarf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dcb6f5683fac776e7f35c64c05b3ae7d533b97ea8ba9b61a97240e0660c83cea"),
		new("item", "medieval_tableware_carved_ivory_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a6652f6b88708b628f32cca297f07fcd9950924511773e73d683a59c8841aae"),
		new("item", "medieval_tableware_coarse_clay_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b70d260db8c738c8c0f7fa025e129b77f55e7082612b120c1f15cffdbfb909e8"),
		new("item", "medieval_tableware_leather_canteen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfaa59815e89374125ae403e0e3793d789e809c9e45e9bb27f3f0163fe954cdd"),
		new("item", "medieval_tableware_market_sample_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed7fa0d12dff3dc8e81c587229f9069cf6ae3533efd8136a89c0f1fee4e73f42"),
		new("item", "medieval_tableware_oak_trencher_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0f34694991f708037f2a3669f4374458140857927d8fbf270afc94805f6efab"),
		new("item", "medieval_tableware_rimmed_earthenware_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0221a07be9cddd41d78cb431053266d34f012eea83153ed8edcba414820488b1"),
		new("item", "medieval_tableware_sauce_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "17a5d66de54dce20356979025d169b6fe3fcb21def80023aae2302221374b8f8"),
		new("item", "medieval_tableware_shared_islamicate_urban_date_serving_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d17e5321a094b5eaf21bb0717bb578b60dd7b7f821dd6e62f960e4a9701fa51"),
		new("item", "medieval_tableware_shared_mediterranean_olive_oil_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f3df80504c51eaf4928c5e1b8e13cd0d472405a8c817ea743ba9dd1ac055969"),
		new("item", "medieval_tableware_shared_northern_north_sea_horn_feast_set_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d16a38a9a68e31bfa8f71c923a8e765d2c434ae25cc3b2e9073c309a178c1b74"),
		new("item", "medieval_tableware_shared_steppe_and_caravan_hide_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "12f5d777d44345881e7e77b86839e50f5106e3bb0521b781fce5b88a363874c3"),
		new("item", "medieval_tableware_shared_western_european_manor_wash_set_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "006ac3cb7152121bea0a3ec5a9007cd6555ff8121d98e6f2e367a9db607815ff"),
		new("item", "medieval_tableware_small_oil_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "059903e703717e63555730417ce7857c4f9d2cd54059cd647a2279e8e89b3f24"),
		new("item", "medieval_tableware_small_side_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7851984af1e72594b99f231d7d8172e7139d3e543ab7244cbc280bef83eef21"),
		new("item", "medieval_tableware_small_wine_cask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a1aef6ede833fa5b0c1e4c74137f3113660ab2a4bb066616727181d7c773ee3"),
		new("item", "medieval_tableware_stoneware_mug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcf279c6900c44746fc1757aea10bc9a17bc9d6af62e96515d072e1c70a054fc"),
		new("item", "medieval_tableware_wide_mouthed_water_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8b0c0322859fe5dfd5fc67dc23801a426c6f547b80762aa6bdddaff1cc912d7"),
	];
}
