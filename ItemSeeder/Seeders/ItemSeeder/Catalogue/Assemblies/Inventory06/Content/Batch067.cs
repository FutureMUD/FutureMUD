// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch067() =>
	[
		new("prog", "CrWhyKCelHouCra48TScr-2930", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4088d686f29022fce3a9259808b7bcc0dc50ba754db21376c15eed96b0a419f"),
		new("prog", "CrWhyKEgyFoo57", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4dc5fa64ea45514e5024c6d9e4ad8f604237ca8e53b05cd25b2d665677b46c62"),
		new("prog", "CrWhyKEtrHouCra44TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "da240baec0063d67193d585fbdac8d1910705b8665be23a5c21f068dd37f5566"),
		new("prog", "CrWhyKEtrTex12TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "245f42f5cfad9301985c1e6bdbbab8fe58188b751b5c59c5faa4d03b858e19d5"),
		new("prog", "CrWhyKGerTex8TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3a14cb5c784d8d781e31d07e565bd04d52c07b2a716b8ac886684001291918c"),
		new("prog", "CrWhyKHisWorFou1TCar-315", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd798bd3628b527722a4b6e122dbc7cdf11fdbe40bf80d00fc8df7e4bbf3eadc"),
		new("prog", "CrWhyKMedFooPro73TBre-3920", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e40e0d2e39a1facec1acbd671dd0535a0d54009fff80957ef96933c9abbe0a1"),
		new("prog", "CrWhyKMedIndFou67TSme-915", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5456521baa693878aea6ba812c68fd59789d084d3940141f2536faa9ff376155"),
		new("prog", "CrWhyKPreIndFooCatPro74TCoo-715", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9f2cde0f3664ddc1f3f010281f9e9dbe5649475e07bf9278bc1e80a437f4efa"),
		new("prog", "CrWhyKPreIndFooCatPro74TMil-3810", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e4d51b503ee88d2e8a903d3564fcb9d120717bcaa56da2cd0f7cdb6910236e6"),
		new("prog", "CrWhyKPunTex10TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0470a2ee3a5aa33dc7514df6b2b5798e2a4e423461b31900d6ae313041acc5bf"),
		new("prog", "CrWhyKRenandEarModJew80TGem-2515", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac59b684d634384a62360672ce2fd2b2e2a0cdfcd695aa3a772e2f1284e39ec4"),
		new("prog", "CrWhyKScySarTex14TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "62cd01308fbe69c5f4377a0e474da2c2c5fdbd431ee2150330005f4795352ab7"),
		new("prog", "HasButchering", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "235efcfca999833a3f20b7b1dddb64c2db92d5c4e45797d03b09b2f6c6351809"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_elastic_starch_delight_mass", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8383dd54f4dc80e82d40ba0b9a61a6ab5a53f7ea3675337dcf0050c3c12a0fa3"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_tokugawa_tempura_assortment", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca2c18a5754513effbee088bf0e600ca97a1adf5de6333df526c1221b75afc83"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Porridge_earlymodern_food_qing_maize_millet_relief_porridge", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "55ba7d2cc25ed23615e5c1819a7a95c05fbdb8aa5863ff861cec9e0b9f8d2f9c"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_caraway_seed_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9fd7d128965d79cbf9f0403c5080ea12723d61c1940de7ba933a360b1b870c1a"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_raw_raised_pie_coffin", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "47806752bf794388050fedde6228bb407ab99399124d7fa893c835af3ae45ea0"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_almond_frumenty_for_lent", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ea0ea002959c9b9fc0a9aa1fb5088f2649a3ecc33e397f47d4ee5e07540502c"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_sahel_court_millet_couscous", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bd2b15c1fc18cf314d10b0f2b7b55fa698ac4a3684d1b247f3fae5d85ded50a"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Stew_medieval_food_alms_house_bread_pottage", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e3cab1731c40156c1310bfe08cd5441ac43e53778ad48bb4ff3c37a8e7fe2ed"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_milk_reduced_peda_tablets", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d830b393679bd56e2488565b9964856d2fc3edd62588af91729e066738c6bb86"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_bazmaward_fasting_roll", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "621f30f6b635f7b25b5c2429fa33fc72077887b790d782c147ba63be2cc84322"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_byzantine_fasting_stuffed_leaves", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c611c495b179e8fb6c5e14d3caafd73405dca9fbc58fb4bac72e2b412f60d696"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_empanada_filled_raw_round", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6258aa0a4545bcd1475efdb1a1238fce64bda5dce403a8b85289ad5b241a7901"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_pulled_sugar_working_lump", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "105a0a5b2ba0c12c8f50a0ba7648ad53d13ef434e9c94ef8cfef191d5370a5cb"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_dried_rice_noodle_bundle", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67135c8b1232af62177fec73e1cd28af590bc3fe00098088a05f274917531ab0"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Noodle_preindustrial_food_fresh_noodle_bundle", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7155261e14e3959c2b130173064236f1fa8fc472dd76ecfb18a446d9d2d4a74e"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_boiled_wheat_bread", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a5cfe2ebb2849f5a873355656c28c25343c90e27b0eee50f7126c4514045ba5"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_fermented_millet_flatbread", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "00a0dc74c3d99de6476e4fcb778edd4ad27639ca20442405901b36187b9c27ba"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Condiment_preindustrial_food_egg_herb_spread", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "26b2427f6e351e453100b8c2797f0945ccbddb5f8024599c1695a0c85aca558e"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_baked_berry_pudding", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b124b0ceeadda2eeb0351c365f0db33201239432e4dc942af5bcdd99f6f824ef"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_oat_groats", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6c1c6c7e3ee2add29ae8fbe6057e7fb450916318a3a594fee294db4f17a1409"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_rye_berries", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b553c2b762a6398fac7abdee8e304872c1cb6f18290fb052dabcba5234fdcef"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_caravan_sesame_grain_bar", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9298274722fd274545857cf17b8d2ab5c4692a7082580de6f2040da59c91c0b7"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_bean_maize_porridge", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "88a52804ff3c50509b34378bf995b385863f8c5c1f50a8dfac2cc17dca40b459"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_berry_meat_travel_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a46c1b457b905565d3e321487da9517e8831937fa4b9b4cf1e15c8a6320e4046"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Pulse_preindustrial_food_aubergine_and_lentil_mash", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87543393cf41d5d1c25e14860d2a1977acc91f6ebfe130fc75dc2e65670bccfe"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_lentil_and_garlic_broth", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3bcbed659e3e9bd93e2f1dea0c2e525acacdb0f568872aed76cf298796cb546"),
	];
}
