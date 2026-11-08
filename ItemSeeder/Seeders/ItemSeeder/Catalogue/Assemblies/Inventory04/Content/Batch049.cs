// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch049() =>
	[
		new("item", "jewellery_simple_bronze_torc", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3cef8a82a7e6b1a3fe482000efc07e691cf9040f4ab2ce2f03319f1ed27960a"),
		new("item", "jewellery_turquoise_bead_headband", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "65ab445cf645eb65b4f83f8d45ad1af8584cafa526f80fe2d74a0865d65b27e3"),
		new("item", "medieval_barbette_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4e589c719d2e96580953a128c352b37cd5079082d5e3feb4ca9f405a8912803"),
		new("item", "medieval_cotton_wrap_skirt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4527ef17d2568a700e9636bdddae8567513b032bcaa12d30d9511bc3aa867f84"),
		new("item", "medieval_daoist_cloud_sleeved_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "83d0564d7763de6492ce889249059f51de6a0d2722eb3a875ee5ac04a12307d7"),
		new("item", "medieval_decor_embroidered_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d07e7a5b71f534323cb89f03b2480ae92673b62b8e3f67f568e037312c435ac"),
		new("item", "medieval_decor_islamicate_glazed_tile_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a30748d7d9750363cc7a10b83ee8bee417bb2cb7fb8263b707d55ac41a44fc4"),
		new("item", "medieval_decor_narrow_hall_runner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f621e373ba41cda8b6ec94d2d5bf79de63b2d5d03bede0b8f4f2ef05e3ba88de"),
		new("item", "medieval_decor_plain_wool_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff3c5077f08d8aae17d968caabe6bc2af3dfedac06debff14bb748380e597a42"),
		new("item", "medieval_decor_small_chair_throw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "32286cbbe5aa351148f123bf2de40c1d04cc9f41a4b9dfd2d6d40cf938a2124b"),
		new("item", "medieval_decor_steppe_leather_tent_liner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7b53be0ea52fba8f03f8dcfc1a7b21e843d15e0ea8a855c0c44d3a8e6879340"),
		new("item", "medieval_decor_wool_stair_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2521836d49e532cb1cef0b43df0beb96e63834f874523039215a6686303994dc"),
		new("item", "medieval_domestic_alms_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d31ea90b6f6745f9cca03192bc0289665564dbd5793459ee4516559a055d79a6"),
		new("item", "medieval_domestic_beaded_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0ac4dade827bcb71e876501b0929b97ee829b1c5898e7db9a005722ce052667"),
		new("item", "medieval_domestic_beadwork_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ccd2321a9282096c5854c68aa05eca2c57b1b765c993f70acb7903835b2e5f0"),
		new("item", "medieval_domestic_beeswax_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2114f4d39fee69748cff4ebf02cf7213e3c7afe6fa5523786d42162bcd67fe30"),
		new("item", "medieval_domestic_charcoal_dust_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa96c730e040679fd38a41f2910b83002e6f0554ca813c3228d14aff2f618bd3"),
		new("item", "medieval_domestic_charm_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a0b2d9cc01ef96ab5e69c5660c765051efaa67a92c2e6c48869449161c27e34"),
		new("item", "medieval_domestic_corner_cup_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8fddf06ab44706b4df356a03e9e364fc1b22ae56573ef6e2dd672184d081467"),
		new("item", "medieval_domestic_cosmetic_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0964fafd0ef67b24e42cbb786d912325bae1f5e85ac85ce5f96a573bdd8f6262"),
		new("item", "medieval_domestic_desk_surface", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d34d87c00ecdf0ff3fbaa86622f0418399833c79b8bf3d31a240f4b8b96aca6f"),
		new("item", "medieval_domestic_drinking_vessel_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eac6a08ecb9aa8f5d091361245cdc2ec87c6802ffb3f6dcab4e23e16c9bcb0dd"),
		new("item", "medieval_domestic_firewood_niche_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d01527581cb110abd72b49efd58ce9f53e4e5654f0d31b3ddba988cc78f4709a"),
		new("item", "medieval_domestic_folding_sewing_wallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8b0b1870214e47ffe5910f820f7b043401f0d61c8a69b16207ea892e373e638"),
		new("item", "medieval_domestic_foraging_shoulder_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec1ab76b2e91e9eaf150115a923dfe7be522f333dd43460bf7d40570fd7648bb"),
		new("item", "medieval_domestic_hall_service_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "42387cafbd3e361f77795266745ede0184fb6ce379904a8a27ce7e3dce272304"),
		new("item", "medieval_domestic_larder_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d859870df26e80415a6b76b684356f323bd00acf85e58f11f7ac5d703e114ab0"),
		new("item", "medieval_domestic_large_armoire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f2684cf40357038bc80052fcbf552a2f276d3c1916d2f68caedb571a9fb1695"),
		new("item", "medieval_domestic_laundry_back_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4ca8af088305bea2729a21f3474ee9c4e2b42683c20b133957b437b5eac1651"),
		new("item", "medieval_domestic_market_tote", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4b87051e9ede7e1364a9899d845253858ab7be37e8a2c60bb7086632d564c1d"),
		new("item", "medieval_domestic_moth_herb_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c09a7fc22a549f529ebb2c101e763d4d32b28996380a2cd16ade35a38cc294c"),
		new("item", "medieval_domestic_narrow_scroll_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6986a77e9362a75d5a0df987d90ee5767acdbab1eea6571f3fec5708e898cae7"),
		new("item", "medieval_domestic_plain_clothes_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5585e3456c3bb0db420f860d52faf1ad0dda736e52715ce54950140828dd9f11"),
		new("item", "medieval_domestic_seal_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c70a42923b953e2ff915c9959447b0005f887797f8136a6a14b66c5ec9aa430"),
		new("item", "medieval_domestic_serving_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5004e4c5293db4285b956018eba82c44ae602040ce02d0db3564dde61404ee14"),
		new("item", "medieval_domestic_sewing_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "79ae5ca7fb04db0534e6ca6dad82283883a95094d810ab90a5f7c6717621696f"),
		new("item", "medieval_domestic_shoulder_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "497c99d96e5eb90294e8018e1a4e4186eda3bbdd05cde7903f965fb3cc2206f8"),
		new("item", "medieval_domestic_shoulder_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "88abac0e2da4d448ec2b80f5fb9e4c8cead8bf00778551785d5e4a5db60363fd"),
		new("item", "medieval_domestic_small_pine_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2db303dd25c25dbac4c8a3f99e53cc7a6da1c38c3d23091e4efa0c655a5048c"),
		new("item", "medieval_domestic_wall_pegs_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d083b003187372043678805b1b6e830aa54abb25245fd220a84236fb3ce018a9"),
	];
}
