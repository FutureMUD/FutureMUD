// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch004() =>
	[
		new("component", "PreparedFood_Catalogue_preindustrial_food_soft_ripened_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f046ce4f28dcd32fe00e63e3d484d91d69c8ecc50e2392a9ed20098e94fa7697"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sour_milk_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61209b640dab32bb9c85875e839998539adb5d8d17e75132a32827ce79e24d4f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sour_plum_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e39111249e81983b5fcc91b8693cbffa9d81533c83c2edf95abc0da61bb1211"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_spiced_cured_fat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "04d88cbc1fd51ed62506823c92fc2702bd327447e7d751a991e301d699b93dca"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_spiced_feast_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1e89122b4c9b711cb25cb81979fdfc080c79700054563f9c34f298a4a57251c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_spiced_fried_dough_ring", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "406fdda40104d18422ea31f13cda2c90488b9d5e44c950aaf2567ca454230ba4"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_spiced_rice", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c78759bb00929a90da6e89c1df56b2513d0d88ea03c8e162663fcc7ebbf9cbf8"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_split_pea_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e554db86ba038b4d9e6cc472e842904a7171cf03599ab621e74bdb1cfd61ae6c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_spongy_grain_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "afb263ce0aba4c059741d8982747366f9b706b4fb4043ed65f5ae960f7086664"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_squash_and_bean_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "91258982e1fb4b5f31cace639b9903e6c95f38103dcaa273575861c798875c05"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_stale_bread_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4fc725e2aa70528b031e6091f39c942eb121d5f207bfd71113d49db60cfc756b"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_steamed_barley_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ae610bbdff1ebe3ad804b4d58002ecf5038be3f6729e1981d352055caf4676e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_steamed_leaf_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a94ba08c33285ab9ac789da7f40aee094c1a418045b9515049f98f0536680de"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_steamed_millet_round", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d135930573cd834c90853a05fd4fd1d83209520a57bf5a13b88b0fa6fb60fbba"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_steamed_sorghum_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfe0434e2b047f64e609c9007642b8055b44219e835b3b890717e88d25b093bb"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_stuffed_aubergine", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1a7dd2e309aceb7bdc27728c8cdab8bb9f832cad158e6e364f833524a799686"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_stuffed_mushroom_caps", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2578fdc8254c436ce16266d1ee15736db3a7d37c2678a99d9b9364de2654b902"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sun_dried_fish_strips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f5db8cdb866d176ad67850dcb83858e75291b40a0f75547bf55781a454b0fe3"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_syrup_preserved_fruit_crock", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4145bbc9346a607d0c38452bbf9abff348911afbb0cd1797212bee8c5a7ac7f0"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_thin_ration_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eebb33eec1d9ac5b59645e09b0f4a5dcff1cc4cdd0df3fc7bc222c60b85b737d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_three_pulse_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "19ed6e1d1797f7ce0b008c4b2b5b8342599a2a32177cd18ed5ae3685113aef7f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_turnip_greens_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e75f0f56dcad6a7fe0c395d4a6f072b3e3d756b17ff357e1f34b8dbcfbe11620"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_vinegar_sopped_ration", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "417d7539c0883bf727bfd99aa3979ef333cb4e81e4fed3efd69130b0599ad598"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_vinegar_squash_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5f63fcd1a7dcbe909ef2cb40af0bdeda22730601b63f790297fa0e822d575d4"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_weed_seed_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8838077ffebfca02c2e82d82327a80982e91183454309911fab191c9a49b671"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_wheat_berry_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b39ebf5bd91d726512411017f6af73dfae736c28e8e079a7ecdc9214c346b70"),
		new("component", "PreparedFood_Catalogue_renaissance_food_andean_quinoa_like_grain_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07e6b059584160c01fad0384645cb658a4ba53ba7914a241cffe494ad13eb83c"),
		new("component", "PreparedFood_Catalogue_renaissance_food_german_fruit_stollen_loaf", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76ad5740d3684610c22ff330e08c626125f85688093788653853da6beab104f0"),
		new("component", "PreparedFood_Catalogue_renaissance_food_iberian_chicken_manjar_blanco", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fa440727f97495f18b370b6404b0902175982d5a67b9257ce175772d5bc1484"),
		new("component", "PreparedFood_Catalogue_renaissance_food_iberian_quince_membrillo", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "89459df0e1e5e938bb0f46d82c3fc4936c347fc607ffb3273203ed145d399683"),
		new("component", "PreparedFood_Catalogue_renaissance_food_italian_artichoke_torta", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "85d1fdc6399f38bd1e3555c1996c94c713fa74e42000a2b8344f1a343b318858"),
		new("component", "PreparedFood_Catalogue_renaissance_food_japanese_castella_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cd86ccb24e15176835b946851372b68096ca0ee91abcd6aa465e3f9e26b8595"),
		new("component", "PreparedFood_Catalogue_renaissance_food_japanese_tea_ceremony_wagashi", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ced91b0541bc22ceb2029ed3fc2977e2a935b49ef4f7d863b921886217f30478"),
		new("component", "PreparedFood_Catalogue_renaissance_food_mande_ground_seed_leaf_sauce", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "69ed9c904d5f6e00882425f3d192666ad3b3315cfa46b953c5b998711de06796"),
		new("component", "PreparedFood_Catalogue_renaissance_food_ming_buddhist_mock_goose", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b96a9cf713d4786f14513a2833654001dc6e7a3d8920a8720d26ca3fff66b794"),
		new("component", "PreparedFood_Catalogue_renaissance_food_ming_lotus_root_stuffed_rice", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "85d6440d46f7c06738524e289bece3a6061f9e8068683912bc6f48d17b1bf52a"),
		new("component", "PreparedFood_Catalogue_renaissance_food_mughal_new_chile_pickle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b33ac3c54c97a60e30142dde035c2a9479476debaa4ec45cb0753e6a36922d81"),
		new("component", "PreparedFood_Catalogue_renaissance_food_ottoman_coffee_house_sesame_ring", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cae2c9ddcb46b6892cac88f7e7d5b97aec6da9078896ae64f8c111b98fea0b3d"),
		new("component", "PreparedFood_Catalogue_renaissance_food_ottoman_imperial_meatballs", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2728d385d43b4652eac839ea16849fbc7c9be69ac18adf847e046765e98039e0"),
		new("component", "PreparedFood_Catalogue_renaissance_food_songhai_millet_couscous_feast", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82dd809a421dfacde50ba96eb52cbe3ac0bf1fd4427218f5ae054a43a9a9eed2"),
	];
}
