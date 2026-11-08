// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch048() =>
	[
		new("item", "industrialised_weapons_police_military_0265_workshop_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "e4cfc57fe26660d84e0e3b9486c12b0c4351cd02959aee3b27f9729e0e8ff2ba"),
		new("item", "jewellery_blue_glazed_ceramic_bracelets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9be24b03c0a6b97aa0e00edcdf199ab88fcfc89de52ed8dc858e88d5f25c191"),
		new("item", "jewellery_bone_and_amber_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce9b7e27a6a3e1099dbf678deb839eaf9ed1ef81d9f258d7ce0d8156d622f713"),
		new("item", "jewellery_bronze_heavy_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec223b8d25e0919a7ebba2b3e102abecab2c414d665ff1a46467aa02e0db3e94"),
		new("item", "jewellery_bronze_square_seal_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "740892847e8db27955089ad308bb0931ccb72171acde0362e633bb6e964e7501"),
		new("item", "jewellery_copper_ankle_rings_valley", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3403c630f4dec10f6a2050b338f12773f002f19be9f5a1b76f5d623fbbeec7a2"),
		new("item", "jewellery_gold_lapis_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "312f178dd98353581a59fabddc2a184fa0c5947a39d2900e03aa8c38950e1d56"),
		new("item", "jewellery_gold_pearl_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2d9d1b43c63bcbb8f21edca33c293dc5ae94e8f5b29a049aef070f63b7b54fc"),
		new("item", "jewellery_gold_rosette_plate_diadem", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a39238a99a8cdf7e226ad8b9634360cf95de45da9e2d14d6347671731ef4a834"),
		new("item", "jewellery_gold_serpent_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b887a1dba87dfe71de8c911379234866b344b573ab0762d78a75371c8a3d97cf"),
		new("item", "medieval_buddhist_black_monastic_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "85dd42280554c4c58dcde353f04a7d3fcb7a0c9255808325ecdfea20254381c0"),
		new("item", "medieval_buddhist_sutra_lectern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1b1137131e21624466b56377e9f3d29b7ee0981fd77913bd4e032dcd175ccd7"),
		new("item", "medieval_church_gospel_book_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9fee9ac5688ec7fa23b0ff5eca0a913947e99dd06fec4bde6e90d7c06ac5599"),
		new("item", "medieval_cotton_head_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8477044ebc2f88054fed9ffbe1691db6b4616d582bc4285dffe4f2ce5c8f4c97"),
		new("item", "medieval_decor_household_wall_banner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "06e92035899eac6b4e2d74d1fe61a2e4a60f358ace1cfe9574881728a345e6ca"),
		new("item", "medieval_decor_islamicate_brass_wall_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8d28d9e24093999188e6dc2be37c5467a6aeaa478a2e746739ee4906b2d3cb7"),
		new("item", "medieval_decor_mediterranean_bright_floor_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "673992d043bfe7717227bdc1a475407443653f5f9cf045f447e6542784174293"),
		new("item", "medieval_decor_mediterranean_linen_courtyard_curtain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f352696eb8d2a2aaef6cc3dc85e07ae5c94436abd9b8c016ee0447a691034cff"),
		new("item", "medieval_decor_northern_antler_wall_piece", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6b6351ff88e9a3ae6201b1077560706fbad3d1f0d50d7021af1608a38aa6996"),
		new("item", "medieval_domestic_belt_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "700c556c8c2adbb5b78df4e270521c357d3b1ee88deb4ef571d7bfaedf072b0d"),
		new("item", "medieval_domestic_bench_surface_storage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9135783bb99d322e9878b8917892cffd139dcc6f628dae633d945340c196cd22"),
		new("item", "medieval_domestic_boot_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfe0e5549379fc0af38f2da7a20e49b90dc03eeee69a8f483b43922f0ce60105"),
		new("item", "medieval_domestic_cot_surface_storage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "02e4ab08b4e15b5b03499692f0c19c73b816275104f45d9c9bd3d651d880103a"),
		new("item", "medieval_domestic_counter_surface", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cc6dda8f69b8a4400bf0956846061a644768a1794d7208b9d8740e2779da0fa"),
		new("item", "medieval_domestic_fine_vessel_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9fc4c3e11a829ddc8d97512e2bcbf947a5663a430457a7b7731038967d49eca4"),
		new("item", "medieval_domestic_folded_linen_carrier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "df268b236b6b28e8b7b8aa36ca5ee5749fc6234e30e58c3cf1254b8868c22a84"),
		new("item", "medieval_domestic_hanging_garment_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5b01c7d87589344bdff41caed9df60f05c4248dabe124c195089d2a299cbc72"),
		new("item", "medieval_domestic_ivory_comb_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbdc1140624e36ba8393945d9bff3c82edde949109a549e0088d9a7612b68a52"),
		new("item", "medieval_domestic_large_bedding_ark", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "70c380ae94ab597abc80a2965b41ecabdc648f4d1ec0e1978586d95ea1e61e9a"),
		new("item", "medieval_domestic_loom_accessory_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b07089ed45a36a4f925534b17d8bed17fa62092bfadf7f87645518917710229a"),
		new("item", "medieval_domestic_narrow_oak_shelf_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "014f7884d6a512ac46a75f9500eaa5acf218bad2123d7d80f89137919284658b"),
		new("item", "medieval_domestic_oak_footlocker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ea0727c4a53d15cf2778d3c08b42b137b6c7db5609782f609896c96b4c3aaa6"),
		new("item", "medieval_domestic_pantry_token_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8b5f376a1da49c6e92f84cdf9557eaf99c94695b997bcc0150872ac9fb325db"),
		new("item", "medieval_domestic_paper_pattern_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e22d276908526172d512df253e754edfbc595c253fc0e96793a7920591a0ab57"),
		new("item", "medieval_domestic_personal_account_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e62b97a638becd4653ce759cdc7c2d44a97ff58cc5ae07982b3f141cf3bf182e"),
		new("item", "medieval_domestic_ring_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4ec28710c3de0aa35269254e95b13db595697251ed90cc0ccb03ccbcef61736"),
		new("item", "medieval_domestic_ring_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1aed9f7dfbd5f97395d6c079e5b81633cbd5353185bc1ed42ce97bbc760d1856"),
		new("item", "medieval_domestic_servant_linen_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d529505b0516bf4e03e64a8d4957f93945d5eb8bfed1d21a69336be15c6d733"),
		new("item", "medieval_domestic_shoe_carry_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3426ce5b4e31154df35db1e17d323aa196230f65758613bd7400ccf7a5adf572"),
		new("item", "medieval_domestic_small_prayer_book_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ecccf7c4ecb243192d50e7d3b6615f97381628689c4f2c7abc580f8aac08d9f"),
	];
}
