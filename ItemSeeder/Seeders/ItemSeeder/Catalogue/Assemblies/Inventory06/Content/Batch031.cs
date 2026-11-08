// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_footwear_indianocean_ropesole_deckshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c4e28458791ae909e22e54177b327192cf7614985f8575fd23de108995a64f1"),
		new("item", "earlymodern_footwear_turncuff_sea_boots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "48165fed109b7094d1c9aa5ca63de827008e1fa8632c53f880beba40d9e1a2e6"),
		new("item", "earlymodern_frontier_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6853cb0168963af79a073e4ac3532c91fb17009a367f7af41ff86d3e3c4a474"),
		new("item", "earlymodern_furniture_cna_armoire_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7da91b69b7e240ec85e7d7e02269b1ebf4704c23c6e71cb690f3bf6966f8c35"),
		new("item", "earlymodern_furniture_dut_cupboard_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "28e612fa93e0363f567bea92c6bde45793a053146e7211d2d725530922649789"),
		new("item", "earlymodern_furniture_dut_display_plinth_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d90e035b366a0c743bbc9aaa6583b9469082a6a96e7cea0896b90d576e05ebbe"),
		new("item", "earlymodern_furniture_dut_side_table_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b21e94de2f816983528647624f9cc57384539c71998f0a711aea921a5edf3c3"),
		new("item", "earlymodern_furniture_dut_weapon_rack_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "79c210220c0aa4dc9abfecab816562d9ebf34de07230cca43c3a4650da24cba8"),
		new("item", "earlymodern_furniture_eng_drawer_chest_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d38f4ac7ce6aab3bb0fea2b749b7f171da928e28416fb77a729ea1ed949944a"),
		new("item", "earlymodern_furniture_frb_clock_case_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e89825337c14680f88dfe8bbcdde44c46f178ea0f41ae5d6e58fd60fd9658b5"),
		new("item", "earlymodern_furniture_ger_counting_desk_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d002382a1b0700b3f5b26ed6ae792de15ee8a0be089df9579f9d00553260bf4"),
		new("item", "earlymodern_furniture_ias_drawer_chest_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9e90cc736acf21d02c42fac2cc8900d3ed91928a015440ddd1feb1e50583a40"),
		new("item", "earlymodern_furniture_ibe_weapon_rack_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a84b6e0a35c929c12508bb0b12b598c63e4f36f167fe53b31303fa1fd3457b24"),
		new("item", "earlymodern_furniture_ina_armoire_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ca4b84d4d62c25cb4c06e036c39ef92d47f5fb1fdbde7631eead7e383404355"),
		new("item", "earlymodern_furniture_ita_wardrobe_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c145268f7f1e86af1f5403821c79ac0ac0c71212e0137a85970f26f8dc47ec7b"),
		new("item", "earlymodern_furniture_ita_weapon_rack_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0e3fcb99246ca486bb7287e3d06d90185e96d529bc923d4b983c071ceeea5b1"),
		new("item", "earlymodern_furniture_mag_mirror_case_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2fc780c90411bca2023c96433c8b329c87febeb2a128d4355b4b054a5f1a7e85"),
		new("item", "earlymodern_furniture_mes_drawer_chest_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "449e56abad703991c4a7debfc6b8b2dbc117a6aa1c54c1e893628b54c975c0c2"),
		new("item", "earlymodern_furniture_mes_screen_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7a5e04b755a634d9530b897fd4bc7492a960068c627de14c7fb5636647d2ed2"),
		new("item", "earlymodern_furniture_mrd_linen_press_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "301c1515baa2bdeb5f17cb68de890b9cf30f59c963ae126dfc3efe96f1c0daf1"),
		new("item", "earlymodern_furniture_mse_display_cabinet_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e574a1eae5c6c69e81a13a26875c04e0927686566f916a24c585bcd1e295c424"),
		new("item", "earlymodern_furniture_mug_display_cabinet_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "044045dec2ad89f1c514874ff0eb6f4e6061698011c1269b828fdccd335cccde"),
		new("item", "earlymodern_furniture_mug_linen_press_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b33a5e8efedb7da984517446b8f2b460614c7d52cb2c2b6940f4dff1c55779b"),
		new("item", "earlymodern_furniture_plh_armour_stand_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2275eb3c489ed3ca18052f92cafba720feccc0538a9ae99bcef5af5ee2ae5cc9"),
		new("item", "earlymodern_furniture_rus_dining_table_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ad8b77622a269761ab5e3705a0e16d666bf5e2a2000ecc93fd5d8eead8dc208"),
		new("item", "earlymodern_furniture_rus_screen_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d5f026be5e3bb07caf7cc5803692a76c4bbefe215aca407543561a8721ff2a6"),
		new("item", "earlymodern_furniture_saf_clock_case_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e92f4a2e662cdd8571cb5d9e753b450b1620b3fb8517b8e8a5279a4cef75c3e0"),
		new("item", "earlymodern_furniture_sah_dining_table_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb39b43ac9e0d69393e676c6cc62da2925045714aa060a0fedde9033ed5bb791"),
		new("item", "earlymodern_furniture_sca_armour_stand_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c18dc623baf5647e6a8c5f2304e6b8fb0ebc288b05a1386726a84425d5a737d9"),
		new("item", "earlymodern_furniture_sca_display_cabinet_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b77e114a462c908ac13af29a9038ded40314201a23ba2db528edaa511c5ce613"),
		new("item", "earlymodern_furniture_spa_armour_stand_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b185ff388215f21c6ff076369149c9b1a1cab56704143bbdda7f0f7853bffbf2"),
		new("item", "earlymodern_furniture_spa_display_cabinet_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "473d9cf0a490a7304fd4a0fc5b3a5d4a19ae80544ef48305c8b034db85fd87c9"),
		new("item", "earlymodern_furniture_swa_archive_wall_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "202e55fc411276984913b23b46c575d09cfad2e21577a4ba668e67e5e5ed0c73"),
		new("item", "earlymodern_furniture_swa_weapon_rack_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7a01b2d1ce226c72a0698ae71beb216cf5db6091fe93ce8ebad4218b948c3bd"),
		new("item", "earlymodern_furniture_waf_counting_desk_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2fcaeaa379729c950a6f32ec879652a14e50af3b7445645fe958cd67eeac42b8"),
		new("item", "earlymodern_german_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "934df44a6bda674258fe21a67089ff5f430016f46e1248f2a97112e9daca02e8"),
		new("item", "earlymodern_german_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2573c8d194c7d393386d95356ed908e7c985e6f4b7b368d22055f2dba25a1565"),
		new("item", "earlymodern_globalmaritime_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "319915904b17c974e0df4679ff132fa54f40cbd75deb1abc17403cca5327ef63"),
		new("item", "earlymodern_headwear_edo_black_silk_townhood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4e48ca4cfb4d3526da95ec4ae212c522829e57728a15009418a899219dcfecf"),
		new("item", "earlymodern_headwear_furlined_kavuk_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2558a296fd593b48b2c6408942677aa1b6991298c54ebe476c84844bbf14836"),
	];
}
