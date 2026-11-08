// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch067() =>
	[
		new("prog", "CrUseKPriProHisComWor2TGla-1125", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9afde82412160a23998378784be6ede5854776e64980007a4271b87d3e20c0d"),
		new("prog", "CrUseKPunTex10TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d5888f9b823ca12666db8d6b9140eba9623a0135ccefba9b3d58a5e274684ce"),
		new("prog", "CrUseKRenFinIteCra81TBla-415", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa103692ee8941539b2acbbf9761a2f661f3ad67faf9fc044fc428ac15ca6bc0"),
		new("prog", "CrUseKRenandEarModJoi78TCar-315", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be667ca58ace5ae9f051306a0976fd93624d7ad7c70981c0c6d5cd21a8936006"),
		new("prog", "CrUseKScySarFoo66", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "62f7379e0831715bf012429c04dddcce341ac0f362ca65f081fb9800fdac0e7b"),
		new("prog", "CrWhyKAncApi53TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5c01157d0ceaa4ab79f8659befa6b8cd81ffda6bbd05f26e336de7788f5e909"),
		new("prog", "CrWhyKAncArm20TArm-1925", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7efe81d66ef37ab1c2a59b94d36761949ea7595dd9cccb797dc2706aa9dd55d"),
		new("prog", "CrWhyKAncArm20TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "24742e24578a4b94668d33e8f65158e3a3444a2add9c76931a234c3555ae2e99"),
		new("prog", "CrWhyKAncCerVes16TPot-225", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee32909d9aeaa9f4f680bf41a1302bba195ddf280fc775c20d91ba1c4f795738"),
		new("prog", "CrWhyKAncComCloCra22TTai-510", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e85a38c7adf077199e0a1dd48dcbd20ba3763aebd0afd0969ebf81c9403a844c"),
		new("prog", "CrWhyKAncCoo35TCoo-2715", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7208f0745abdc5f848ac02443f6f070731da2e94dd7bec2e09719d8627e3bbf"),
		new("prog", "CrWhyKAncHouCra38TGla-1115", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "58997ad23ee8ae6bc74ad44e153578931bbf036b2b2a6738de6adbb502a7ef6e"),
		new("prog", "CrWhyKAncJewCra23TSil-2415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2e112f377ad5493907a519092018d423ebcbc8a13109f103e6e154d7c207991"),
		new("prog", "CrWhyKAncScrandCodBin29TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6b2c1f4af04b09c48e4a5c2e2c7da4f82a21bd846765cd0684a2fe67211e6dd"),
		new("prog", "CrWhyKAncStoBonandHorCar40TMas-1015", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "599fe4c5db35ac233302b75be5a609240115c1165ac1192211f3064288acbeb4"),
		new("prog", "CrWhyKAncWea19TWea-1820", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f773837eb0d591c8934654b3a160cd1cf851544619d765084266389bf691c51f"),
		new("prog", "CrWhyKEgyTex5TTai-510", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6eb8dcc3387bd7d65e18b0934e258e64a55a7246f8f8aff85aec6d818717fc07"),
		new("prog", "CrWhyKGerHouCra49TCar-325", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7cd02fd4d46f43565e966c5d0305d8bfa24d4ecf6d3767f6ee004f2a9e63adc"),
		new("prog", "CrWhyKHelFoo56", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "50b1bc46037731e705ec0e33999dd1f0c8a589919badc6ce1f832eb52b83c8ae"),
		new("prog", "CrWhyKKusTex9TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "70805ae86bea14af860d2dbb34838efd4825c8023227e5ef4da8521769aaf4a2"),
		new("prog", "CrWhyKMedFooPro73TBla-425", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53fba141c631f3d8ab0b0df64075f6e370397af43d893b9fdda20d714eabbf50"),
		new("prog", "CrWhyKMedFooPro73TPot-225", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aafa30c1c0d00111cb40da64702c62237d21e38a8646de693e8b46dcd5fc6fcc"),
		new("prog", "CrWhyKPerTex11TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b1348aa5fc9a5f64cb0fcb9ead34f82326c508de5b54674b15118d76dbabc35"),
		new("prog", "CrWhyKPriProHisComWor2TLab-810", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d256c75cfcbd2e3d4b01f87d125223dd41b8fc774f0e9ba48032271e019faa02"),
		new("prog", "CrWhyKPriProHisComWor2TPai-1320", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d170a871e409b0f53164a9a189b0c195eee7e6a531b240f4d386b3cc3f264f28"),
		new("prog", "CrWhyKPriProHisComWor2TSme-920", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "013f63648d98fa676ce4bd6aaa590aee2ca716486c5d4c499de465d4646419f3"),
		new("prog", "CrWhyKPunTex10TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "98099e693a103c3da98d3a597dcb7fb5ac080db4b488a9fdb89a22a24ba04c4a"),
		new("prog", "HasThreshing", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "02168a39505f6583e06abc379f433352946d7c18badfbefb1e4a9bf22328ab3e"),
		new("prog", "ItemSeederAntiquityFood_fish_sauce_relish", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cecbb29d19ea7808e12bbf028981db897f049f33f09b4474f558cbed21b8683"),
		new("prog", "ItemSeederAntiquityFood_porridge", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "23e9f5be60333c641255eccb3bcecd6b5c200a445a6ab5569860c0896517dcc7"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_brazilian_cassava_fish_pirao", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5fddda61ef2143b0b0f0385c8fe9f46ca5de48677e2c440809dfd4025ce1a58"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_bath_sugar_bun", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fedc8129fb7a453d988bebc8cd1b7a30fcf030ae13309da7e664705681922e48"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Preserved_medieval_food_sahel_royal_roasted_meat_powder", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "95e71d29762d7a35ea2370ce42b38d849471c7ea12e12f52dbf1d1841f932edb"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_court_spice_wafer_roll", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e18e1698ed82c2c2f185ad924ff48ce53756d748689df984c5e71aff2bbee48"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_song_braised_gluten_mock_meat", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1f0f83441795e5a224705b27cd540133c7e0c3c5dbb6b42a15316bec1559f90"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Stew_renaissance_food_andean_chuno_meat_stew", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a915e732e6a87794b67beb7960b922b7e8ee690e17157bf57d0ad7ea802a5b1"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_andean_honeyed_maize_offering", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1a1647e4d22b38eab606af82e6a77d33b09f5e8941434568601edd1a1ce6e20"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_mushroom_filling_portion", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b54aff7914e3db7b181c5fc96df19149584ba3147d7a5a87e54943bfec74ca7"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Meat_preindustrial_food_bread_meat_sops", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d56952e24dc0c1509fd32b5844ca2135bdf30436c46a2c7c914dbf7140e32eb1"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Nut_preindustrial_food_fruit_nut_pastry", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce1bd159dcee397e66147a559afed11c6f4d94f61d07c5c53d78bc3f1cd822c8"),
	];
}
