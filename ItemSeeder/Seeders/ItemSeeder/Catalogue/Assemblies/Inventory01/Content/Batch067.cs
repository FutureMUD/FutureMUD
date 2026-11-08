// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch067() =>
	[
		new("prog", "CrWhyKPriProHisComWor2TSme-935", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f528c29d15cff1468bb359ce37d6996626707c8ba22fab08a83e354a41b01f6"),
		new("prog", "HasArmourcrafting", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "f6d223a0465070c8c99f3137d52446b2e232115c4b5fbc092d5f62528a688d9b"),
		new("prog", "HasBasicPharmacology", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "7e13476925634632e650b6b229bfeda7da2b06205a29ea3a54db1c485d06a9f1"),
		new("prog", "HasCarpentryAdept", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "1b1ad94dd361e3029635c59682510e25207f824058698c78d5c0674709bfca29"),
		new("prog", "HasCooking", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "863a82c99876937c7163358e9e2be852086ada74f2a0c63cf9776857729d56b3"),
		new("prog", "HasWeaponcrafting", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "6c6c14fe3081df88e0b8d7d31744c94f0dae3d27faea446fcd711d24e6d21504"),
		new("prog", "ItemSeederAntiquityFood_honeyed_pastry", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed20839ac9de6493da87088acfa675aeddb73e4ad142169d8131cff8ed7194e7"),
		new("prog", "ItemSeederAntiquityFood_oilseed_cake", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "703ce28f762e74300626fc06a3373ad50d84f6a09fea3bfae01b21faab703052"),
		new("prog", "ItemSeederAntiquityFood_spiced_meat_stew", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6640b0638724915842d0af857309d4fee04b70d676f7a126a2ffef5dd3761255"),
		new("prog", "ItemSeederAntiquityFood_sweet", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "627783d007af54146538aae5a2c5c73f5f47463ae2ee69a6296f0c81a22a32b6"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_laminated_puff_pastry_block", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d98bb45f48ac29457d2ab51ce157b8011763801f7fdf2de26b693fbcb5576dcd"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_sushi_vinegar_rice_cooling", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36789044f121b65cba1fbeb8fb6aef3338481c8e3cad6595f92edf6e45ac71ed"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_company_kedgeree_predecessor", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "240e83be1b45ad7cd2748b0a5eff0db64598947070b3f866e9f59a0047025404"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_british_portable_soup_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c35a900e94af372825f36f072d48b11bfc8e9fcc038995ef6e40fb2e7dcf5142"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_joseon_chile_cabbage_kimchi", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "89bf071448a913569117dbf95730e386f94a372059e806590628d02da1343c65"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_baozi_uncooked_pleated_buns", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4d957eb50b9bfddc9381d4243c194a6f2541343d8fa2a231d3602bdfaf1bc7a"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_japanese_zen_sesame_tofu", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a7bfb4a045f8f8a16e057c544594021cbcc3168b511e84c39628cad43ac6705"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Bread_renaissance_food_ottoman_coffee_house_sesame_ring", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a4192b14d859611899ba6134a3e40f20bb0e54b269359f9fc0ec9beaa0ee397"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_andean_royal_maize_tamale", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "686061249a57ea5f2d7c544800d18646594c0e7196f3c2487ed6dfa738fec449"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_japanese_grilled_miso_rice_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49525e3e59226496a8d8f384ffbebd82f298173d317349d0570c6decb019562a"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Poultry_renaissance_food_atlantic_ship_turkey_pastie", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "377dff675e60ea7ff58f12d177d313486b45d15281e5ce41af2618c83baba671"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_braided_bread_dough", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7bea739a70f7bcdf00e52b71f6df87c2083d5e3bf2e68451e47a505a60619b7"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_candying_fruit_jar", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78253fade70f3eb29cb523677bcd9fd42e590dc16b8896fb1cea440f938020b3"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_cultured_milk_clabber", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "35d55170043c7bc153b6526a0d839523e7d4b8ccf200aaa0aa23887b7f2ccce3"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Condiment_preindustrial_food_cacao_spice_relish", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "617ea5171510e07e378bec9da7b694f33591f8cccf825a4c3fff1787be32a042"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_baked_quince", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6732a2281a7e5c4b0e14c53e23336824c35d317ece0287023006b6875e5ea920"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_famine_husk_gruel", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb3787f03c9a2a9a3072f786f402d72c08f5d95bba4c7af8af282245407ee4bc"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Sweet_preindustrial_food_honey_yoghurt_sweet", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f72f364e3cc212d30779f5156be4387fd28ff284fc26c48dab8b57d7ff4b340a"),
		new("tag", "Era / Information Age Era", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e38bf24fdce8d01748ac574d00306313f03ad45ad0bfa1c03cdd15e23e39449"),
		new("tag", "Food and Drink / Antiquity Food", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d49879b2d2a65ec7714afd8bf7bb98ec998f2649213d5b71bb9dee3c1743e3c"),
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / Chocolate", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "42cc7f9eaf17d204034cf0cfef4bf76ee63cadfa406e4757d23154c5bf3e1b8e"),
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / Tea", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2c060f6e17c424ba08228da1794f0f52c1fd9e53f9718de5044dd05a5c74a95"),
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Scope / Renaissance", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "581e3019b868083233722f836c2a2b84a0c4e1940132f76240415a31c86c60fb"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Grain", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "46968886d692fba9a58a6b016829a81728840f5bfe9444c337703b0b59149af2"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "013e0bd6c63de12186986cbf3e3b69d83bc1190ef3cb6dc83411cb5c2796771a"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Noodle", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3dacd64ac7a0a7afcd1c1043dc76ceed2525ff6db04445edf5efe0b1f87995ea"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Nut", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "60f1e94549737cb3c61e93a2e717a99bc8477684acfc856bbb81b43129a5eee1"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Vegetable", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a90fb5dfbf834a59d21b4662825174a3bdc7e080379241183b5597873940bda"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Scope", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "159b198cfe590420723aeea0d355a7cd0958394819a0f1b54ff0d09520bb2ff7"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Social Register / Bleak", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8238c5f242c5aac06a27ee13c700f9c709f324f6114065317694a15513fd1f6"),
	];
}
