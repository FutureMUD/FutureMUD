// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory15Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch032() =>
	[
		new("item", "earlymodern_furniture_ryu_wardrobe_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8818f23a73976acc02e556b948324ebf70787497e263922ce2abf0e7aee5fc9f"),
		new("item", "earlymodern_furniture_sca_clock_case_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cd979ab79006e24fbcd7ad40b759317b9c8f6a0b05c683e78ffedd74cfcac65"),
		new("item", "earlymodern_furniture_sic_archive_wall_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "be05b904c278e0261114eb64932895044cb3653ba9cb17b08d62c509460bb59f"),
		new("item", "earlymodern_furniture_spa_bookcase_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1b0c18e28786dc19ab759a867aa00dfa768fe77686b3cdb23f1c50f5593a1e4"),
		new("item", "earlymodern_furniture_spa_dining_table_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9861d34a017237d342d0ffc6ff908d5cd4e617146dc9b61e7326e65cbec0c633"),
		new("item", "earlymodern_furniture_swa_display_plinth_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "67ff2b0911cd8c39a573b210ee8a4e909ec6d8b3dcd35de3cb4acf6a746b81c1"),
		new("item", "earlymodern_furniture_waf_armour_stand_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "72c8ed9e283953af6df93d525158f914f4a679eb88b47d2d40ea5eda03cc2640"),
		new("item", "earlymodern_headwear_ethiopian_wrapped_white_hood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af74b3b2f006aac015c33baa008220ea6750f2bad945d35e2651d65ca0ad85b6"),
		new("item", "earlymodern_headwear_narrowbrim_town_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b816a133a31e6263d70a7ac9fd2e5c6130b3b2fa6159733dfc34fbad4b42eab"),
		new("item", "earlymodern_household_chocolate_pot", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5a43a380d73dd987268e5e0685c28ab30fa188a22c07a1627d57d793ec5da82"),
		new("item", "earlymodern_household_playing_cards", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f75b96bb99bafb9a49d93d9eeca98d214d9454e31cd5a31700e737d6c0d79130"),
		new("item", "earlymodern_household_tobacco_pipe", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5442e959a9c7083674f4b0725c54be2e59c49299d8b11587be6a0ae0abe9c35"),
		new("item", "earlymodern_jewellery_and_chain_necklace_665", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "57573dc8f71897a5fa25fb5fbd6c39979de8be102e0eac2ac08d6db900c1f1eb"),
		new("item", "earlymodern_jewellery_and_ring_661", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1a4388cb0c3dcb882c1f1969a2726589c39c3fcda1708f8dea50ec9c227a76d"),
		new("item", "earlymodern_jewellery_bra_earrings_589", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f4f42ed0132275f73a27806bd472658c62581695e3932ae11cb66f716b95a19"),
		new("item", "earlymodern_jewellery_ena_armlet_607", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e043e7f1620ece8b4300ca2e267461b04e83c840a7079461c4bb8d7b339d91ce"),
		new("item", "earlymodern_jewellery_eth_waist_chain_535", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5699ed41bd5de6542cfbe48b00cc97c3e5db233f52bba8652a9d704d5d1c7368"),
		new("item", "earlymodern_jewellery_fr_bracelet_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3d436109cf238a14516243bd880055891f285bfc9c227155f7822d8d7d5d12a"),
		new("item", "earlymodern_jewellery_gb_badge_57", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "954e3dbe195437e001bcb10a885c175f326051b3e7d52730184e2326e98f49b5"),
		new("item", "earlymodern_jewellery_gb_belt_ornament_56", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c64a0d2b7a2db950fa31c615709e85c61e57ad496927bd042a686cbd4d80b153"),
		new("item", "earlymodern_jewellery_ias_bead_necklace_444", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "33de9baa46bfc75cf82a4b2b048dd3f31b496bdce28355c95409da1333da110d"),
		new("item", "earlymodern_jewellery_ib_badge_77", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ee703da8d5cd1a0254c68ff15cafef78b5c6d9947b02a73520d42d064ca710f"),
		new("item", "earlymodern_jewellery_mag_anklet_208", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc5b3907ef46b8982c6887e4f8cf1b231298bcaf2a99ec50683cf0db7231f855"),
		new("item", "earlymodern_jewellery_mca_badge_657", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d89284ac23a5c77c8d9e134a339bc2c39ecd8c0ae5bf08c49c79c0636bc7c109"),
		new("item", "earlymodern_jewellery_mca_ring_641", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "354e2f1262619254a98d6a7d1f5fdff82f36c2c742e80aea87bd45a6ebbb6507"),
		new("item", "earlymodern_jewellery_ming_earrings_329", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "207b47582e33e3b77fe55c4cf8cbc0492cd77c9310db80c32b7dc4794b27d91b"),
		new("item", "earlymodern_jewellery_mrd_anklet_268", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6d1181efe7beb1421f6abbcace53d29f3bfdcfcc815cbde8924c1db9a865afb"),
		new("item", "earlymodern_jewellery_msea_brooch_430", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "497ee0e953bc513d5ff1f6bdfb880ed3133ccc848319f6aae26dbd083c7d63b0"),
		new("item", "earlymodern_jewellery_msea_torc_439", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "50b1c255838d35c3bc94f1715b01f83f613429db6b4f6ab9251ff0e6ecfea350"),
		new("item", "earlymodern_jewellery_ott_waist_chain_195", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7117c24be27add2eebe692453c556227acc442b390de30713723144b74e301cf"),
		new("item", "earlymodern_jewellery_plh_temple_ring_158", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d945c8772ae79d09597173d9a8670e7db787cb7b24154ef82e8b4184185a1ca"),
		new("item", "earlymodern_jewellery_qing_circlet_313", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "afae74b9827769b1dea8a75415360035e427dc893a21fd16eba6f28dc0d2773d"),
		new("item", "earlymodern_jewellery_qing_waist_chain_315", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7aaf46b7d2340ce44b744c66df16c25429c272e7b298b65fa1093e3fe3a2a0cb"),
		new("item", "earlymodern_jewellery_rus_temple_ring_178", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed274a3fbd9cc2356a396eff4932fc2f2c16a5d4fcf68ba277051cd7cdc6f2d7"),
		new("item", "earlymodern_jewellery_rus_wreath_180", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "985e635443fff89a249992892c06316e379a2a2dfae615c10909471614d2abfa"),
		new("item", "earlymodern_jewellery_saf_waist_chain_235", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "692021fc38e703d06b1e270eb8f15ecd3dcb7434341ccf20a68d0a80d86d5b85"),
		new("item", "earlymodern_jewellery_sahel_armlet_507", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e3f814e6b1d690c73266edb32d22e576b9b39b1972325f9e09b55031a2f09b5"),
		new("item", "earlymodern_jewellery_sahel_bracelet_506", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5740608c68c3fe5eb73dcdf08b2f3a123857e386799caa3dbcde35a3bf7d7cc8"),
		new("item", "earlymodern_jewellery_sahel_waist_chain_515", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7640464499d88dda4061fd4b8d985e3a34904dd11969b2610cb85868ea8f117f"),
		new("item", "earlymodern_jewellery_sea_belt_ornament_416", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e3bdd98536c29e5b03219b4cb0f2bb2fc67080f6b75a8aaa801e6da64c06bcb"),
	];
}
