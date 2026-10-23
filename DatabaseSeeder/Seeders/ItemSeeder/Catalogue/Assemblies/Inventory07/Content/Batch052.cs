// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch052() =>
	[
		new("item", "medieval_military_plain_war_spear", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aed45172cf1fe0893f4eea6e38dec8ed43d53ac6e866fb4e7e1a13c9b0d4dd89"),
		new("item", "medieval_military_practice_long_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb12c2bc74019ffdaa9d303eecbda640cb042da5877669aa3aea3f57ce7e5268"),
		new("item", "medieval_military_target_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e92e0c92074d55989320d077881445d24e1dfcdea11cbb2651fe3aa89574fa7"),
		new("item", "medieval_military_weathered_armour_tabard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "353b902bdb49922035f08c3c14faf999dac17eef4c3878355cd6f3946b7f44c2"),
		new("item", "medieval_military_wool_braided_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e77b93c92eccd3c4a2be48e940598215c716f7573b3d2cd070e10eab752fe32c"),
		new("item", "medieval_military_worn_round_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "441cef5d180ecc55bbfb2c504134c942352d92aec914c7d2c90f0aece6dd3776"),
		new("item", "medieval_military_worn_sword_scabbard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "47c4fd6f63d9d774daa48e1ab8e7f2914d294af47b1052b88e74fe26dbba5024"),
		new("item", "medieval_norse_viking_age_ship_carved_birch_document_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed27228b807870da6256ded0d8a119b24ebc7c76fa2d1ecb21c0c1bc19f5e77a"),
		new("item", "medieval_regional_high_english_wool_staple_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "182e6dff188d6a8bbd1bf6f381863c587c7ecf6e63be58c7170fe2dd79aa615c"),
		new("item", "medieval_religious_offering_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0066513573ffc89719218633bc660ec0a07aac6392ed7c27fa943df47a00395e"),
		new("item", "medieval_religious_votive_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7093636e5971c73223ebf8fdf52af5c17463a1a80e101f6c84778f077270aabe"),
		new("item", "medieval_rus_wool_porty", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "26994f98e8b6883ae3af74fbc8d70a02a70aeb3a712be8a79809ed9579760833"),
		new("item", "medieval_shared_chest_hasp_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "80d61ef07cf62772dc5b94918b44934cc3974d518118387b23f0b6970e744b4e"),
		new("item", "medieval_silk_bordered_veshti", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eaeef2655e1c9df5622151693a1fe9ea1442763fb1679a41dea60f04875f4309"),
		new("item", "medieval_song_beizi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e1549067e9a46b8eba57ff75d6d5b980ae1a2fa3fb61c518fa8b96834d69155"),
		new("item", "medieval_song_china_bamboo_household_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "871c53717e4e16130ce440c78a456f97fe1031cf5a9b5296a4698d418ac04abe"),
		new("item", "medieval_song_china_bamboo_scroll_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc0d2bb6d31facb367ef44622715f1d981b4b997babc96df68230ddfa3e517f8"),
		new("item", "medieval_tableware_brass_hand_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcaff1fb527aab460e5c03501b4cbe87ce3d3269d72a250049786ef50414520c"),
		new("item", "medieval_tableware_flat_fish_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8eed7cba4deb0689a5a3cc39dfa71bbdd37068e8c12795aed1920947019bab44"),
		new("item", "medieval_tableware_handled_oak_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "77dd94468573d6750c44f654f9e4216115bca8876686319e31d8864863549dcf"),
		new("item", "medieval_tableware_handled_porringer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a861415aca2974920c654a4a566b4433cb09d1ff9971a63dd41e5ccdbcc1991"),
		new("item", "medieval_tableware_large_serving_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "98f5ecb5ed4fcdd87a623c86f84509fc102d2e073a3533b2e12de7713c55ae6d"),
		new("item", "medieval_tableware_large_wash_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f54de2a2c48c65a7694f96ac540ecb3c20f59b4189a4c1ab292381eacf29666"),
		new("item", "medieval_tableware_painted_ceramic_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7664b0b3ea64dde77d2ce23ae1855795e784ab36f4bca820a2b193540ee68fc"),
		new("item", "medieval_tableware_pewter_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6957feb7dbd2084073d50b959ce52cf70bc2bacf823ebdf05703800dc33b1b17"),
		new("item", "medieval_tableware_porridge_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "036d9d108cf82a3254657ffdbfccafa98169c9f3292a5ea9a08dc4af0b79de76"),
		new("item", "medieval_tableware_shared_east_asian_stoneware_tea_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dcb4efe47e8563606522f20030c8f3aa7c07798fa60df8b158a0072b62c89a01"),
		new("item", "medieval_tableware_shared_mediterranean_bronze_handwashing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c16688e82ee17ffef34393bb83b69e41c09a8648f9a3e4308d06d34abc87c528"),
		new("item", "medieval_tableware_shared_mediterranean_marble_fruit_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a6a5a5d3976d67d03c621801494843af27ff11ab08dd4926afc1d4dadfeec8a"),
		new("item", "medieval_tableware_shared_mediterranean_painted_table_amphora", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dd90ae52ef708df4ad8a04727ba12bc890d84ff9410e6785cced76145c9a6e9"),
		new("item", "medieval_tableware_shared_northern_north_sea_ship_carved_drinking_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "775fab5113244fc49e88bb517d74ae29fa29ed170d9c03a0df749c32e0753e2e"),
		new("item", "medieval_tableware_small_cordial_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f4ba219527d221a36ff9338c6ce44e743229eef02d63de493b48f3036e83ec3"),
		new("item", "medieval_tableware_small_table_amphora", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "052cf40702a6397627b4f6d82ffee6f99c517a9ec754ab46bfa5aaefad596e45"),
		new("item", "medieval_tableware_stoneware_bottle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "82ddc70f7918263af15c119353f22f50b961a5ff8341b6bcf07f3bec091fa90f"),
		new("item", "medieval_tableware_two_handled_serving_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "12b4fe6b0e7d28c876b4ed3c9239f0a094bdd7ff457722c341ff6409f5930c8b"),
		new("item", "medieval_tableware_wide_table_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "08c9a4a84a284209a9b8f829654c24e5455e7a783f36e8dfca070e0e5a8717cb"),
		new("item", "medieval_tool_armourers_pliers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bdfb0429f18773d34c45fa5acf8799bce4c820344f77b99f49e3758bea36e04d"),
		new("item", "medieval_tool_basket_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "af588e70d3a0d40441d1757bee16e1edd0475769ed154c89936e10d631c213ae"),
		new("item", "medieval_tool_bookbinders_needle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c25e16620fef5ec3dbe7c24c005bec4603224a17ba2a1b099e7ea0cc079aa8d"),
		new("item", "medieval_tool_drawplate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4bec98bcac3ef4eb2f2aa1e0fc691d1026c0e9296c294fd15672c4767441fd68"),
	];
}
