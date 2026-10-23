// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch068() =>
	[
		new("prog", "CrWhyKEgyHouCra42TMas-1030", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0ef760428dd30a7274b50fa72a9cd30b666b3fb90bc93f7be3aec13263976a5"),
		new("prog", "CrWhyKMedEquCra68TWea-1810", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4b8d0ea82a45f30a84c851679ca3b929d9717c0b59a47feabbd05b6f8b64e06"),
		new("prog", "CrWhyKPerTex11TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "97dc0f2f809d8921277e094efa9b4962b76c81482aa2074251bc6e09bd3464d7"),
		new("prog", "CrWhyKPunTex10TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9dc49bbe9454fa2c00bdb22f84bd793d0e163c22c48993146e04a3cb26ab8ed"),
		new("prog", "CrWhyKRenFinIteCra81TBla-415", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "31ec9ebe825cbcd3dbe018e7ec1d3d953889df3563d267dcc29cb7e159514e46"),
		new("prog", "CrWhyKRenFinIteCra81TBoo-3415", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea2748c76f18347311546ebcecfdd4b4618bec856ca90a7d73b033247d69f4f9"),
		new("prog", "CrWhyKRenFinIteCra81TTai-515", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "606137f1ebe2ed35cfbe7427bb35940269c9bb424cc748afdd25d46060fd84a3"),
		new("prog", "CrWhyKRomFoo58", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "620ea480c59f4e86cb86dd8983fd3ef9066fd78c65742b086342a3a23fd7cc2a"),
		new("prog", "CrWhyKRomHouCra50TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "782385cce7bcc0ad810500b41b8a875432910a6b7991e969b5f1669ad8aeb544"),
		new("prog", "CrWhyKScySarHouCra47TLea-620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d472c0a9b9ce663f8c19b30c8920f304c2ca9dbf010ee8c8a709616beea1754"),
		new("prog", "HasTailoring", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "3f4e67ba5c59e96c1a2e8956b42ded1470ea72e3ae2182ad9514a5ee8da4cc80"),
		new("prog", "ItemSeederAntiquityFood_fruit_platter", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2eb72b7eab6906fb5f6b7cc6f547e3aef0fb38cccd28f00566f1e04e8eab7040"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Vegetable_earlymodern_food_lowcountry_okra_rice_pot", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "94ff81cef2628d59192896136ca34ab1e481a6bceb79c9ba05ae22fff11b66af"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Meat_medieval_food_abbasid_bread_tharid", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc3e6c0900490117805968a3bcf73e2026711b94822126d1d4a99d61750d1dfb"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Poultry_medieval_food_bird_filled_battalia_pie", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "340b4cfc77fbb030d5846fba9e0ccadef19776ed778048b72d30c35fe7487304"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Condiment_renaissance_food_joseon_new_chile_preserve", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7771c9dff686b3eebbfbf15c72efc860686d53c9b376db04c05c9c3eb77b5bbb"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Meat_renaissance_food_andean_guinea_pig_pachamanca", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd39b6a1e0b15c157cdd58cfd6da5161e1c8d13920915852f88055837eb560f2"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_iberian_egg_yolk_sweet", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b678b2551a92b88ef2011e6bacf6c768b4e06966474b1ccee48b676cf85dbe38"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_kongo_honey_sorghum_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f27d37c536731bc7b2d74400ab3b41c011976039694779725eee4315def15e66"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_dried_millet_noodle_bundle", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c16249155cedf1486eac9231ba7ee8b8d2250001b66d60bea4945257e74bfd5"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_honey_nut_filling_portion", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "732fee63575bc4100498257d107bc90a1ccdc67edbd6a410ed84f3f61700c0a7"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_date_and_barley_ration", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "980a108d5a3b028e36923683e587e35c4a19f553f4fa2fd816def0fe2ff09230"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_barley_batter_fritter", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dcea746735cc26eed283b9af960dd46b113af1ec475cfbfba2467085e35e93fe"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Offal_preindustrial_food_blood_and_grain_pot", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbe8a6f08cc42fb49d8b55461afd5c16fa301935d79aecff8e33e8f08c17d612"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_beetroot_garlic_soup", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "90534afcff3c4ab929e803e845d8b2855c553efbec5f7caca761763e3eac9d48"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_cheese_and_barley_pot", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b954f7edd12bccccbb44a9157c4fb425a7fbad811468d96b88a277f8a418f347"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Sweet_preindustrial_food_cacao_honey_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8fdf98265ba575e8c4a73e2387a1e1583d22612c15ac900e8b1612b0016b5a4"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Sweet_preindustrial_food_cacao_nut_confection", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "413200d0c5701e7bcc2428338ae2aaaaa67d0675f46562569afff01feb0e0093"),
		new("tag", "Eras", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ade95a902d6e9ce5aaff06a4952b335f1315d986a2c30fc7e1db78fdbea66465"),
		new("tag", "Food and Drink", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b724dc9fdf9fca46984da85d01e39cb7c4c1dee8117fd287a09c63a5fcbefbaf"),
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / Condiment", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a97edca02ce934d4ed76796c6879fd99ee33f2d28e4ec377bf4ea78acc11bdd6"),
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / Sauce", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b43c881590614bbdb9f3d85eb706a3df03297b8c568e89ebd0c859e74c101742"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "89de147c1f03ce835e20a7ae19b25107c365d262e4ab26038c5e104a6e66830c"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Dumpling", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e1a74f0671cb90fcd6052201458895c74c0f0bf766a50a68f737791e8f1f869"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Fermented Drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f852d366a1625d85a00efdc0668efbe5dfc81f8e5a61e6bc0e64cdbb78f200cf"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e32ff77753b2c02352729bd8896f4e3e0f2495f241c945bf6e65b5b41a432f7"),
		new("tag", "Food and Drink / Vessels / Fermenting Vessel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2350e76698b7584f81e8bde14d3092ba8cef9518e4017f430051025d33d0104"),
		new("tag", "Functions", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2b210eaabeb67c679900a8a97d552da950cfd06cae4ef2afc0421cb569fca22"),
		new("tag", "Functions / Animal Equipment / Draft Gear", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8077527d92481149ff2ba9503e8957ab8a027114777c3a37fb04918a64f374c4"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Clay Pit Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f1b6508c3db953fdf21d7954af710afa25986772a999b1588bbaae1ef51e9b9"),
	];
}
