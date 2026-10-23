// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Wax_Triptych_Surface", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "34ed7288469966a72663ad9625c7c8a7deb2efb46c496d0643bb014ef25ee1b3"),
		new("component", "PreparedFood_Antiquity_Porridge", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ef0f0d9e7b627fa2fb8c482afc09698f035fe3bb1d3cbbd73da7e81961cf4ea"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_benin_plantain_like_meat_pottage", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2c9b49974188e78a8198063d66c802ea12b306067b6a691a5ac2f5f0879bac8"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_layered_trifle", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7dafe93adda3d9ffc7ccaf4d808a334dcd64db6e1a06f58b1b05a86d4d25a73"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_dutch_spiced_speculaas", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8b2fae283bc2e1674cb87ff5dfbe357e4ef66406b56198e63526695f86a7f54"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_chocolate_almond_dragees", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ab5d0898d8306247f2a50b4937e89dfbdee17b5e08d05ebe3e205ffa6aa6cd4"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_kongo_cassava_leaf_packet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b09e0739272e96eafd1a8b7b9e266a7f95624ac1a71fe773580f8636b9c1647b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mesoamerican_cheese_chile_tamale", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6833ca5e77f64056f1b763be0a84a13a0b5665a5fc40a5d75478362a9d392cff"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_punch_house_spiced_seed_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "039fdd90afab86b413014f558ef258c246cb5df705dcc81d19e582ef2a936050"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_safavid_coffee_house_saffron_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e26b6d849a8ee24a8fb38770a3d995eabac0718084cd362fb695c909190d08d9"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_senegambian_groundnut_meat_stew", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a30171734bc55ddf12a09079c28984f188cb3e4964c98f9122cc955970195946"),
		new("component", "PreparedFood_Catalogue_medieval_food_heraldic_jellied_meat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1ca9d242795e00d51d5b2408af2fa354c038838c07f781b2c3bc23bcb0a0417"),
		new("component", "PreparedFood_Catalogue_medieval_food_lamprey_spice_pie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e4a2812f580f79a9b8f641140adb938ea1ca95353b458226cbb8ae362f3fddf"),
		new("component", "PreparedFood_Catalogue_medieval_food_lauzinaj_almond_lozenges", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a08a884804ab9f4f3f1fc15de1d3ce2b478d21f39bc2aa9df24c6413e114894a"),
		new("component", "PreparedFood_Catalogue_medieval_food_mid_autumn_moon_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "59786a126ee1434a33b031d1a6c3aa1a4b249331f794a5acf0c923a8c010a302"),
		new("component", "PreparedFood_Catalogue_medieval_food_monastery_honey_wafer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9464ad7e0f7fd5ec57ebb9d1a0cb36b7b18695c62724faaef1a00ed4668a1fa"),
		new("component", "PreparedFood_Catalogue_medieval_food_sahel_court_millet_couscous", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "928769bcaee864a0fba44929cd3c256f0ba8eb43c8942e0d766e6d68a482fbc0"),
		new("component", "PreparedFood_Catalogue_medieval_food_sesame_honey_halva_slab", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "12679522270b8c021a1dc3e39375225e403aeca30247d31203214c7e9086fd45"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_cabbage_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "19cb19724fb5d6d0a554b70aeb883ddbb46c71dbd48fdd20ed4f8b800551bb90"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_supper_roll", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f29a1804d1c81c09fc687ffa8a6d41175c1c23d0fa6644351050535c54caa6d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_toast", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "deeaf139e5787cec5f13072cd9915125ebfb46a5feb121abe388fd30f8b94037"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_berry_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7bc4e0a53694b1822cabe504c3b4b8cf61fcb1508ec958f2d8b674cd7c48dcea"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_berry_griddle_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca54a8afd493810de0e8f663b2950a5534c3ba415216418fc8f010ada5e1a19c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_carrot_rounds", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "774c0afc60f7bf22f2581100774f15c8a44f2c6475236e334add6bf55fa51e4a"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_fish_dumpling", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f032f320b6c652986aa8cf683c9cfc9b0cc24764e5b0c1a3b9dabf0fef18890"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_lungs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "04f2ec112f99c172880b5844ab37176691a8bea041e9948730ce3d68187cbe81"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_broth_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a260e187b9abde770d3bf8b3fb897089da41a59f127590a2e739e2f1a1e2f07f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_buttered_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e27d4fb51f4260e8a26fb31b3003e5efb87c6298f53c5f7519e63ecbb906257d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cabbage_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "97afb4b9c2ffd6ee17485cbb9115c19e74deb96f616fa1cb97d7d51615192c19"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_candied_whole_fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3aed8e9de7a00e1c2f200797b8a112c997e97c9b49106214c31c2fdefe66619b"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_caravan_date_meat_block", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bdcc5fc09282be596948828a595b5e095f45a7cbf9736fc01a0cad1bb69bc38"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_carrot_and_onion_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0294373b9b1a4cfd25ee3ddec4cc269aacde1ac6c2d5a4a2ec2aecb41157f44"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cassava_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "77298a011c4052f5bf1aaf7e9a59c26c190c86b1cdb3bd9b08188aa5612bc7a1"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cheese_baked_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "88f8e3b3098e2ddc541183e93bd5d9afc3f0ee8e2136785d9f79c7cd2276ed5d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cheese_omelette", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eae89876b664e03273898947cc4541205f0c5acf214c49a432a03e25262d2b5d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_chicken_and_cabbage_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "afb8cabac6b552daf1335de6f1396dfa536b64bff8d808cf575af94acf65d702"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_chicken_leek_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a21c7ac7dc1c602c3b1ccdb7e2cf0631f8934a1ab6fa49f13e20f54e5f94691c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_coarse_meat_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a74d34e33d0738bc5555ee601606a6d8fcd003455ce115e6ab007751bc86a4e4"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_crisp_fried_shellfish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "79f6f3df3a7f2e1020483b1820766036a1a1104a91957c18d158b796444292fb"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_crumb_baked_oysters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dda35bac32b2d31d834119135a1ec33872f4b65e8e5bb9d3092bbd21c93cd1c8"),
	];
}
