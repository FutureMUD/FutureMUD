// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_food_dutch_oliebol_currant_fritter", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d18571715d1ec9cacc66f5dab87c6d9f0cf10918b2d4806194371a57ddf85d2c"),
		new("item", "earlymodern_food_french_pistachio_like_ice_cream", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b357d5f2ec537fcd02f79e0a00af2db165c1dce5f8c158f03d6a73a61742979"),
		new("item", "earlymodern_food_ground_chocolate_conching_paste", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4c5cf7133e383fe2a8b97633e10d5b76077f42a91ca67adbd9dadaa16b4abd1"),
		new("item", "earlymodern_food_lowcountry_okra_rice_pot", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dbb7f949b90fe92e831a6542d7885f7fb9fed54c3db13b5da924e6feee0a1aa"),
		new("item", "earlymodern_food_ottoman_imperial_kebab_array", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cecff8f86042bb7a1e00d68ba0bfd9ea494aeccd8611f19013495c276d36f3d"),
		new("item", "earlymodern_food_plains_bison_maize_pouch", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb14c0b663cad241418c53cccdb788a00a7afa45a3f2000c16c0e953b8e83a2f"),
		new("item", "earlymodern_food_portable_soup_reduction_slab", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "49fed76432de553551fd254e4ba7b6be8da541524838da190d744fe252d34cf3"),
		new("item", "earlymodern_food_tokugawa_grilled_eel_rice", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f4494509399e253317f09540d5d965c99659e71399f9eb11d88b861829a028e"),
		new("item", "earlymodern_food_tokugawa_sushi_box", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a97945df403f90a74d6cb2a1a33f5b33a8db6669ddba27a7927321371bcf4aee"),
		new("item", "earlymodern_food_tomato_conserve_reduction_base", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e47072da7d27d93c7c426809d8a1873a8ad34c0db43aacdfc6955c1b18ef55da"),
		new("item", "earlymodern_footwear_joseon_padded_wintershoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d4de795f68cf31eeea30c811eafc2a85f9984da62942d96ea6d0ff75c75ae4c"),
		new("item", "earlymodern_footwear_pointed_womens_courtshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68ebb3e2bb4be23a80ed8a5c966adcf5ba369c8c17d02ad8a4dfd390123a12f2"),
		new("item", "earlymodern_footwear_steppe_quilted_ridingboots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ccf35ed5efa2715155b3cab1ac9b67f32defb506d28a70f32d1ff8bb896e8e6"),
		new("item", "earlymodern_french_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "752e35dd3e76e3f53ee2f0ae9c41f4da4b06c6d8a9b4a3bfca7b2d9fa7a55807"),
		new("item", "earlymodern_furniture_bra_display_cabinet_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "014fe214f319cbb77e5ca84dc9a9896445f6a7316016d8a8bd73f14c4a4dc5c8"),
		new("item", "earlymodern_furniture_bra_linen_press_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "08b34b4ada436267408efb35c78bb78e1f0a18de1f67bfb021c20e5e0b7f446e"),
		new("item", "earlymodern_furniture_cna_bench_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1ba5b10e7ea297796293373871b75661c1a3c88d7498bd3712e52d5620c3772"),
		new("item", "earlymodern_furniture_cna_clock_case_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d350a0a16acb4148a49caf3a45ced53c3a8491c28b4528f2c733d874de83171f"),
		new("item", "earlymodern_furniture_cna_linen_press_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "96adc57517d268ba9648c4ddd91eb0215f4cf598f6f8e02432d3dcce5a85dc46"),
		new("item", "earlymodern_furniture_dut_counting_desk_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f49744d97ea105e791ad71834516f331e303c875211da1931678bd61d80f93b"),
		new("item", "earlymodern_furniture_edo_armoire_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5a3141774ed0353871f84854cae56c8c6105f2a4d0c8046b5f095a3af3b2b00"),
		new("item", "earlymodern_furniture_eng_display_plinth_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "92b923d83e0e8ce1c352b7baa6d3824870025f5175de952e10a110753a8d290f"),
		new("item", "earlymodern_furniture_eng_mirror_case_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3460966bbc53d34007c582f757f7cbad3cef58d6b409ad8165aa699c670509ec"),
		new("item", "earlymodern_furniture_eth_screen_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "db00813f90ba8366df39beb08ae6388148dc90a17e00b7d516ca4d91c5f357ff"),
		new("item", "earlymodern_furniture_frb_weapon_rack_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca1b7026f819213a45fa069c74c0d9252f0efee5e498c3a26aa4f10e590e5152"),
		new("item", "earlymodern_furniture_ias_counting_desk_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f1ad7da5a526cc374411b263373ce0a9ab24be272ca6002596261a7fc4978ab"),
		new("item", "earlymodern_furniture_ita_armour_stand_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e844a5bb32ca62d74ef17c8d833894d15a9031ecb87728c217d64d059a1ae132"),
		new("item", "earlymodern_furniture_jos_cupboard_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dcc96beddfa71b8af837aa07848fba8d2792e2f37990bb4b224ebeb6deed3bdb"),
		new("item", "earlymodern_furniture_mrd_dining_table_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a0c277008158667b66a908c7ed6277d98aae73498294beddc3ccd7b8e12587b"),
		new("item", "earlymodern_furniture_mug_clock_case_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2fbeac84f2d58e2a67249b555cf5089ff82dd5410a709ed028eeab034529a0b5"),
		new("item", "earlymodern_furniture_ott_dining_table_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "43ea858021a3cd763554c1106608a2c4a2c2482302e18c7e911a128487e9be70"),
		new("item", "earlymodern_furniture_plh_weapon_rack_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5ab9eef03b63ff914270aa104c2b4ac353ba687874112df8eaa7ece0659b370"),
		new("item", "earlymodern_furniture_qin_bookcase_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "41aac5ee995c97f264b5dfbb21ef8cd17886f687a7415385fcc9d7006ba46e18"),
		new("item", "earlymodern_furniture_rus_display_plinth_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa458e090660905cfc4b5913f021a26f269ffd124de6c0c9181e9d340f7cd7f1"),
		new("item", "earlymodern_furniture_ryu_drawer_chest_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bf01e408e737534b299546f3a167dfb87d228bc18d249b95073fd762d4ad78d"),
		new("item", "earlymodern_furniture_saf_armour_stand_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e774b6fedd76e05e1e2fc5a10eef8584fe9ae38cab8aa5c84d42c97696f0a62f"),
		new("item", "earlymodern_furniture_sea_armour_stand_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2ec96b60c7e86c36a04989c8afb13cc137ec773424771a022ed129726339a2d"),
		new("item", "earlymodern_furniture_sea_display_plinth_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d46b58f90de456d02d794d4a87aea5eadae7b7c465c224c516682cb95ac61cbb"),
		new("item", "earlymodern_german_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1055a796b6fed3946d262bd617ce63fdc6804d4e4511a3cf40ddb4d557e9de8a"),
		new("item", "earlymodern_globalmaritime_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40c3063d6f1e94f07fa2076ad292e13efc830b20b5707a27d203c53401f6b8d2"),
	];
}
