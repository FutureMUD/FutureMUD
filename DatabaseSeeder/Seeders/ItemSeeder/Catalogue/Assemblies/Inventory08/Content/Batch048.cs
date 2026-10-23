// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch048() =>
	[
		new("item", "medieval_door_east_asian_pine_panel_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed5e72f27881b8d4c655830e9c6072075e56594dc3c9e0241fc6745d77514d61"),
		new("item", "medieval_door_islamicate_urban_islamicate_lockable_cedar_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f79be636e34eae41bb2fd8dd7b3c3f0454a2f02c65909c0f8a174dfcfcde6bcf"),
		new("item", "medieval_door_northern_north_sea_northern_lockable_palisade_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fd4710b1b04a4d13de3be1b7c7606a96ca62b6107d8ba1f79a74d778f74866b"),
		new("item", "medieval_door_shared_lockable_chamber_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "63a1f19f6610d45da754ed7c1b44b0fea94c7ec0f433ecd59ca1734324bbfe52"),
		new("item", "medieval_door_shared_lockable_town_gate_leaf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c5352c29eb3bc4bfab02337814611070bfcbef56393f494c1417667ea358fb0"),
		new("item", "medieval_door_shared_palisade_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "19e54e8fbdf3152140e0e63bcc063169a974f05998fd2b5fac07131bfa3f6fa1"),
		new("item", "medieval_door_shared_wicket_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "832e678acfac1cac442bd7ddb5ea1b89150ec96b42da79e6df91c2a3061b6f96"),
		new("item", "medieval_door_steppe_and_caravan_steppe_lattice_door_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0dcd47f1b957bb85e57a88cae72e34f53dd22cff5f38ba17d17f759d6c3f9208"),
		new("item", "medieval_drug_alum_wool_pressure_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a8c0f9e40fdf3a77c10a53e227d96372b431df4ad21867e0a2608ca28aed1df"),
		new("item", "medieval_drug_ephedra_honey_cakes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a218ae517f1421d36208612c56cccbc19033e9e9dee312d8268cff140a011c37"),
		new("item", "medieval_drug_foxglove_warning_beads", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5373d46abffbbe402ea035e2cc4f6229b66c39c54a2ac3ae2077038cc5d8e949"),
		new("item", "medieval_drug_henbane_leaf_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "571bd2271d49fae8436436a506f2c68fb07e80f61bc0c3ea4e313f1600cf690a"),
		new("item", "medieval_drug_honey_poultice_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c09dbaa60ead152e7477d2e7145f83cddbfbf4a54a0cc852bbbc9bd36c1c8ab2"),
		new("item", "medieval_early_anglo_saxon_ash_linen_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d841439acef692d4ff73f76dd83fc0d2c254c366378e1378dc45a1cd93d0ee5f"),
		new("item", "medieval_east_asian_pierced_keyhole_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f04862c69b2fae04645e558a651d12ee143afd67868df138c7b20dc7c2c8b2b"),
		new("item", "medieval_eastern_christian_reliquary_icon_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d76a87d914845cc19010183c443a95e887ef73c30ba5aa4d62d6309ede2023ed"),
		new("item", "medieval_fine_angavastram", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac89d20b2e06a2ecf2bddd8c9a72bf38b5b641f3cbe6a17064bec6fb7ff33eaa"),
		new("item", "medieval_fine_bliaut", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d37f094bb01aa06fb70f35888effb59b7b2fea6c98e34138c9cfd59f432f1fbd"),
		new("item", "medieval_fine_checked_wool_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "51827f49d7994742c49cbd3434bb2ed1cd414ec9a7b867a043e00bcf1208c3e9"),
		new("item", "medieval_fine_silk_pellote", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3faf868307bd0a500b8ff14f718d9e9d2d472c20cd653d61b98c3f3d2d045df"),
		new("item", "medieval_fine_wool_kirtle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4a733177648152e11c5795ebe2d03836cf14ddc37981f15fafe94458e8ef0cb"),
		new("item", "medieval_fisher_headcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8875da05c2702cc667d4dd46409b9017392250b54cb883aa9dd199338a9fe60e"),
		new("item", "medieval_food_banana_judhab_court_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a07a21f754f4c744a8dd641d8d728599db3036dc215752ca89c5d30ce7ac942d"),
		new("item", "medieval_food_bazmaward_filled_bread_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "55843c766055e4247082b845fcb94950b755f836ecc7ffe3279cded91dadd099"),
		new("item", "medieval_food_byzantine_wine_jellied_fowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa74bf9abc5957d4d01c44c9d144568e654d2c82bd6c93c5b167025e61b04a17"),
		new("item", "medieval_food_fasting_trencher_fish_sops", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "88cfefdc6b7bd9a89da5f8030b3fb1d002d858f64aca7fe6cb180da488b4a7c4"),
		new("item", "medieval_food_gilded_court_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9013ad7bef2c7f61648fdeb0e7d32be74b48744a92402aa4475b875d12fd0581"),
		new("item", "medieval_food_green_sauce_cold_fowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b2bf7a82eb2a377cf4c24f85bb9e952af7ad55e4c265f7bbb2f09952371e5b0"),
		new("item", "medieval_food_ma_muniyya_sweet_meat_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a4a5b014ec7844b0f7253724d42d38760b500c0017a6f1c48a9324d3df0ae78"),
		new("item", "medieval_food_monastery_tofu_skin_parcel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "373f64930ca708082ae4ea3cd1039b80216dc08693e6e4b90709edcaafb94a72"),
		new("item", "medieval_food_monastic_almond_pottage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ae23708748e7e443d13b44152bfdf9b2c5089d05f1af9221e6790a97f8a0810"),
		new("item", "medieval_food_new_year_crescent_jiaozi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "47c68b222f1ed571775dae74159aa03f9e2f10b13d3672bfedc98a653ddb8cef"),
		new("item", "medieval_food_peacock_revested_feast", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb5f1ad6971c5a69264bd4ce86255f00d6355e2ac6b729354b48d4672d22a9ff"),
		new("item", "medieval_food_persianate_fruit_meatballs", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "900aeeb7a1831205a4a3ade7cc34f06c3e00b4c1f841edc65d1578adee14121c"),
		new("item", "medieval_food_persianate_rice_crust_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd1d0024e6cf5813805e0c2f72aafc23a436dc9607545d6ca1ac97b3bd1f3dd8"),
		new("item", "medieval_food_pounded_mortrews_base", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ca3f34ef74f0bf1e750938980110bf0dbcd5a2ed3aac1b274f27242cb6354b2"),
		new("item", "medieval_food_royal_spiced_khichri", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "461aa526756a3e453405473264f0064cb02ca44150b58d09d0b2cb8d20939aef"),
		new("item", "medieval_food_song_clear_wonton_soup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffda3b1b57c277687e3bacd747c4b5fd2a67d20b645931527911eebc50d1e605"),
		new("item", "medieval_food_south_asian_market_kachori", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a5071c99aec70a4818017caa5edb462ac9b92095ea343343286cf70ee5e334f"),
		new("item", "medieval_food_sultanate_almond_meat_halwa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0fa7cae39045afa6d973b8b6c9918752add701b6cd93842584f4152e92ea3c9"),
	];
}
