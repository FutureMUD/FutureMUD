// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("item", "antiquity_food_persian_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cca54b452d0d6022ce00c675375c03d06fa7cc0e301590c2df8efcd19590e293"),
		new("item", "antiquity_food_punic_stuffed_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "93087ed05b3d1b0517647bc3e21fad217a2d9542d833b3d8468975ece6afa974"),
		new("item", "antiquity_food_scythian_sarmatian_preserved_meat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "849d78d1eab9c055a017aaad833e7553c07e37261fffded558df64b335b61668"),
		new("item", "antiquity_food_scythian_sarmatian_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8718b0760e433b9ca965ef9cfebf1a11c2d5bba0eff03daa3196b40d5ff1dde9"),
		new("item", "antiquity_garlic_salve_pot", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6bdc25c4eb2140e92b4f01812bb5bb4cfe387adb4a0571fbc53eaff678d5161"),
		new("item", "antiquity_germanic_broad_leather_war_belt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dc9fabbfa12fa222f7b304405568e589011181886033b54ae8d07794819e18d"),
		new("item", "antiquity_heavy_wool_curtain", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "34c0021e5054e01cfdd18cb19bc21b91b2c8bd1f10023fb52c7e3be14440ad90"),
		new("item", "antiquity_hellenic_bronze_bell_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "10921586a8181907e1a174d54611820a62dc4ebd742ce913c60cf587d4192dcf"),
		new("item", "antiquity_hinged_wax_diptych", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "592381bb937aa7a84bb124efd7b14a7801394333189004e1c869d1cd1a40e561"),
		new("item", "antiquity_hooded_wool_paenula", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0dd5db3b0bcb7993000b334ded58f64518a28374248dc08bf865262ffc05ecee"),
		new("item", "antiquity_kithara", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "01d13846528e18b31814cbf6fb37c4cd698283318365acb0a14c3ac51bd87b94"),
		new("item", "antiquity_kushite_bronze_scale_royal_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d586fb42a19e8a06892f9e4881fafa39481ef12a24ff30048a0dd2ea5ef1e034"),
		new("item", "antiquity_lacquer_brush", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea6d2166d3d1ac76c0e0fde86e404475139d4cff8357eadf6b2c7a4aba64115f"),
		new("item", "antiquity_large_iron_keyring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "62a69e00aed583151db96331202e5f23a755b4ea904d891f7fca652d1a08b4ab"),
		new("item", "antiquity_liquid_broad_mixing_krater", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "23883857b661e65897f627db1e8f1c3b54243660e0724d6f3c8f0a9dce7ef93d"),
		new("item", "antiquity_liquid_broad_table_krater", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d014e531926ec702de00e13b9b9d9a5b16d8188338d5e5acbf5c3bae824c9e08"),
		new("item", "antiquity_liquid_bronze_handwashing_basin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "34f17e331c25c37ee1eeb6e1c15606f61779424a362cdab5abe65245fb4a4f27"),
		new("item", "antiquity_liquid_meroitic_painted_beer_jar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aea11331ccf2878a9cfd85c6cf24db4236cf2b8e374126183ca701cbcf2e41b0"),
		new("item", "antiquity_liquid_red_clay_water_jar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef5a2457f9e08a024824806e9c7dab31cbf2df249079db4032a042dca07f5fcd"),
		new("item", "antiquity_liquid_reed_wrapped_canteen", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f50d834bbc79e3682cc2154747dca90ba9bedf51e4f477775d2af723ce98d54"),
		new("item", "antiquity_liquid_tall_kantharos", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "92dacc1ff3022ebc7eefd477482d934175e67fceaa46eabc571a5b811e48edd3"),
		new("item", "antiquity_lockable_cypress_shrine_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "58464ed3a04b4561702ce4f5c4c5eaf0c65ceb630828def40e241837a3da84dc"),
		new("item", "antiquity_papyrus_lashed_light_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f78d0c069aa44d60f8b9e0322b31a141e7adda06fa265dcf3994c9cfbe26cb3"),
		new("item", "antiquity_pigment_muller", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7772c141d51b19975c930691078af55c721d44fa5ab5b91c6a6fd3db96d4efe6"),
		new("item", "antiquity_plain_slide_key", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "72f26c8c11a6de4bd3040dc3ea1406fa4407c5e015be4c2e3f989dfe3f90f80f"),
		new("item", "antiquity_roman_leather_field_boots", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dacb82d36f2f8e568365b93527e6037a90121b0e05da45883d963f91b08ab91"),
		new("item", "antiquity_roman_plated_military_belt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "757cf4aae8c8eeadcade7e9fa3872badb3e63b3abda1498488d809f9afba7bab"),
		new("item", "antiquity_roman_reinforced_caligae", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea0d15dd53fa4a457e25d7f53f49dcba3e17482cab6fc81f68f67fb30694f9fa"),
		new("item", "antiquity_shield_anatolian_tall_hide_shield", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2222be2a031fefef920003f9615fa4ad55f70ca2c4310e8099e6c881f551e790"),
		new("item", "antiquity_shield_anatolian_wicker_pelta", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aac2372d261ba5518ccfec62caee4dc6a3869bdbe592b5dedab7ffe4833129db"),
		new("item", "antiquity_shield_kushite_wicker_hide_shield", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ae5763b31ec8dab36a02ada9568ff6d271ef4acf9217d932b13dd16040c1ac0"),
		new("item", "antiquity_shield_punic_wicker_light_shield", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "63560a187985c7e56aa8b8d4fadf8455025d1a54c4634304274d9c4fe8b28ef4"),
		new("item", "antiquity_short_fitted_linen_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1efc9284634aea6755be80192ead878241eb3f217c990f5c50c01b7b16ed6f43"),
		new("item", "antiquity_simple_clay_oil_lamp", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "224ce7d7f28b8e8fca3a4653458de132f6098afc09c67546259b59f91fb0b72f"),
		new("item", "antiquity_simple_walking_cane", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "38c5a351b767a99a72f2c4d6e98302398838a0a795ab9da6157aa550bad37a17"),
		new("item", "antiquity_stone_chisel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "920003c6a73f9616c61299a229642a165a98be48ea2bab37309e4ae00c8e132f"),
		new("item", "antiquity_stone_grain_measure", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "469a723a6c279c9689ee087f7e21f8fa645557b6f880c7f400399f0de1e62906"),
		new("item", "antiquity_tableware_etruscan_bronze_ladle_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb3decb59b2df636eaa74ec4628fd152a3bb422c9c4c640894a8a9c9b5c0adc9"),
		new("item", "antiquity_tableware_nubian_bronze_offering_tray", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "927c82d065e656775d6b4f02a7609dcbea0b7103437f33c9113644d5ac543921"),
		new("item", "antiquity_tableware_punic_bronze_libation_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c6ba7ed691f5a6309902fbdc0519322aa5ad8e4c32d1a19fc800afd187521c3"),
	];
}
