// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_furniture_jos_display_plinth_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "01e46eb50372361650fa9055995d5f42897adb8e5171ac71672bfdaf8efea8e5"),
		new("item", "earlymodern_furniture_kon_drawer_chest_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a27ab4492cd5c5ebbdfaa2aaa18bced5fd4c187bc2c328a8ebaf3e8d66b97668"),
		new("item", "earlymodern_furniture_mug_archive_wall_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8304ec370611e1a446aebeab0fabc2dfd0e832f54c178807ca75d6c6ae07f534"),
		new("item", "earlymodern_furniture_plh_writing_desk_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f27812eb57677a7c98e571475f3451dcd1d0b1390e4d3cba4af1439386a0e2c"),
		new("item", "earlymodern_furniture_qin_counting_desk_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d97780abd8a683412cb1f0d70741e7a1d3794922cebbab28117e4b7cb8a612c8"),
		new("item", "earlymodern_furniture_qin_wardrobe_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7305b5cc129b0505576594ba0575395b1a91ad13f640fe70d898c45f629766f"),
		new("item", "earlymodern_furniture_sah_weapon_rack_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e32d5694b64cb5b298aae72e372d47edfec18ad0935031cdbb851edca8fcd5ee"),
		new("item", "earlymodern_furniture_sca_mirror_case_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2290b54c081d193fbf70a08e768330687e4f1a2cf306e51f3be846c27e114f8"),
		new("item", "earlymodern_furniture_sca_writing_desk_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "77cb145c60a204e295428ed55f00f8352e8d160558a825f01e1bf96152f5931f"),
		new("item", "earlymodern_furniture_spa_drawer_chest_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd68d48c50d635024ee1d92dbfe86c8a4a4c16396dab8e12804badb742094250"),
		new("item", "earlymodern_household_glass_display_cabinet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "177e220180accdebb4e10fd6bfb44c95f6fec56b8f669b55bf382972d2dc4835"),
		new("item", "earlymodern_household_tavern_glass_bottle", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "48d57d737abfd10640d8b36a6df585ba3fbe4b75ac589d2be52567433142711d"),
		new("item", "earlymodern_household_tavern_tankard", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "62b5d641019e64d18530624ff3d82938047b5deae236c6b31137acad960f8981"),
		new("item", "earlymodern_iberian_clothing_short_wool_cape", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "35aaefe71c1341639e2fc2e31c5afefeabbbcae4f52ff51b631b552a29bfc4b4"),
		new("item", "earlymodern_jewellery_and_anklet_668", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "162f0ba567ac7ec8ce409f9cd5f3ae8501e213af3b20ed021a4dd75939a90c9f"),
		new("item", "earlymodern_jewellery_and_badge_677", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6da9337eab60fae7d71f79d7453cbd47ddd684bdca14ce11f4e2541fb72be9c"),
		new("item", "earlymodern_jewellery_car_armlet_687", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "46eb20e17ea0fa3613053183804daa1106219f19ae8fab1971dd14a48e5a977f"),
		new("item", "earlymodern_jewellery_edo_badge_377", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e738458cdeb859cd2a359897807d256eacb78621172c087566b1c33d36d87f7"),
		new("item", "earlymodern_jewellery_edo_pendant_363", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "119d157cdb09bd301875b79b152d43345c44c5073f7052c660088a919071bf7c"),
		new("item", "earlymodern_jewellery_ena_bracelet_606", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8b72e07dad2ca9e9a9bc88ecaccdaf957b0bcfafcb8a4ed6856d7f8fba6c03c"),
		new("item", "earlymodern_jewellery_hre_bead_necklace_84", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb08fc5532e0ab1d762ce761ebf5fd9a5e2cbe6e7f149e00d9be25713703d14a"),
		new("item", "earlymodern_jewellery_hre_chain_necklace_85", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "de1fe469d83763f5cc8efe2ea9a5a5d72ac68cbc8b58bc33621c96a232b3d049"),
		new("item", "earlymodern_jewellery_hre_ring_81", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "612ec0bca1f928b31016a350c7cf2b0c40ed4e951bce935ddedfda5e93ae900b"),
		new("item", "earlymodern_jewellery_ib_belt_ornament_76", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "81f4550bb79049ae632af789f5e9bfdbbf0b8a47d23ce99fde4903a8fd3cb838"),
		new("item", "earlymodern_jewellery_ib_temple_ring_78", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b6601746726f4d38a2b2d1c8c2a8c4aceccb56bfacb8ea061cdb0f6124aebc1"),
		new("item", "earlymodern_jewellery_ina_brooch_630", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "151be5878e4457f9fda94b5313f532fd8b0105d6417aab69fabc2ae08c68f472"),
		new("item", "earlymodern_jewellery_ina_circlet_633", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7934710a4d34b4e48b72df7ae36c58c7a159545706a04439eec54a1fa36373cc"),
		new("item", "earlymodern_jewellery_ina_hairpin_632", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d5c4b556478e8b96e1b8765751e7dac30dbf12d69cbfd82a17e29884cf4c363"),
		new("item", "earlymodern_jewellery_ina_torc_639", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aaf39b4f0818e7e58dac9dc5b140853f40f05f2e0a5ce557fb0ec9b869b384c4"),
		new("item", "earlymodern_jewellery_it_brooch_110", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aed6f1dc4f0c4e9de056c57f558a40872e23baa6da1cc77bb31a92f180e6e8ce"),
		new("item", "earlymodern_jewellery_it_earrings_109", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4efc64f5d6653b95f0f1366c12072c0af0747dc4b1e31dadfa20ad40a6335ba9"),
		new("item", "earlymodern_jewellery_jos_pendant_343", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dcd4b7bce3cc44e0e9e5e04a504b0ab861cd43701ebb3de985dd94a65402bec5"),
		new("item", "earlymodern_jewellery_kongo_brooch_490", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "347fbad7c9167b20c1a1a178305978ce1c606a187c6e8dc2195101be9f2acb24"),
		new("item", "earlymodern_jewellery_mag_chain_necklace_205", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae91901b640cf82ead009c4ef2adda5e410e08103ad4d6accb12297ed26d97c8"),
		new("item", "earlymodern_jewellery_mag_ring_201", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cdcb3d62b040e5af6f93f02a69f5fce8577d7267b2b5cdfdc299cb5b805bc44a"),
		new("item", "earlymodern_jewellery_mag_wreath_220", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc0d31655f026a053d782aaecf2963c9dfe70844f41e5d42cd384fb0ef95dd51"),
		new("item", "earlymodern_jewellery_mrd_pendant_263", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "665bee22ff66b5b5aca3f27b7bfa80ee76d8fb0e52263f353e6a0923522c4a0c"),
		new("item", "earlymodern_jewellery_msea_earrings_429", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "594f9754c7a536d9e9330feb092cc734fd36150367d40139f0960d53ba02e344"),
		new("item", "earlymodern_jewellery_msea_hairpin_432", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2282f4ded5d84f8dd29c5377316f85ae73b91b720a8842cd51a4bf4231d603e"),
		new("item", "earlymodern_jewellery_mug_diadem_254", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a966f2d9c4417850f3e5022c3f247ed78e22bc0e08267f3bad58400f46e003bd"),
	];
}
