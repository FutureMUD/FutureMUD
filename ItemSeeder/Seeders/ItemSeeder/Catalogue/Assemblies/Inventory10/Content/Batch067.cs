// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch067() =>
	[
		new("prog", "CrUseKPerHouCra51TSil-2440", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "65a81f1cecd1464acd9caf279b2f32a4ee39924eac44b9cf8fa8e7d7963806c7"),
		new("prog", "CrUseKPreIndFooCatPro74TBre-3920", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb94d4ed3960dfb4279eb96b8a5975d5ff04951b7048f1f9ee5cb6f6cf477f65"),
		new("prog", "CrUseKPriProHisComWor2TPot-225", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b9c04a318e221b48f0ec4a960f6179f3ec75034de360dab9d1aab6abea2de22"),
		new("prog", "CrUseKPriProHisComWor2TSme-935", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cd916973a0eaa4a2d609392a51da8b03cccfcbafca8d3bfcfd997f57e4192a0"),
		new("prog", "CrUseKRenandEarModJew80TWea-1515", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fdaf8d3d182f8dd354e5bbcb12160b783fe3376ccfaf573d3ed7c50cbbf6a0d"),
		new("prog", "CrUseKScySarTex14TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbf3e764f3b036b733cb02a7b32c3738c90aa60f2ee92379a208a6ce3a3e64f0"),
		new("prog", "CrWhyKAncJewCra23TGla-1125", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4db3cc9cb5c6d08cc97e54078fd0a68e177569a6940cb0a94ea7e69f3e6aaa0"),
		new("prog", "CrWhyKAncToo17TLea-615", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "279da93d64f0dd870268c3e344047bb7d0293635f23f9d35c95ea3a7b04fcdd1"),
		new("prog", "CrWhyKAncWea19TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c01531a483ec6020d9a609a93f2010de2b17e46e59fa22dda5f3620170867be9"),
		new("prog", "CrWhyKAntFoo55", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b02063c2a43f71461a70489b7e94d5ee760b9af73fb305eb06a66a39a82d4228"),
		new("prog", "CrWhyKEgyHouCra42TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5cd2768229cd02e8a23876c6de9577ccb97bdf03d679bce0c88ebc6f6c58b911"),
		new("prog", "CrWhyKEgyTex5TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ceed87885f81eb3471cf24a9c1a75c34a77d7c731c9fd5a9881d8d01ca3d6cc"),
		new("prog", "CrWhyKEtrFoo64", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "14aa351f9057ab2ced797dd0a2922dad6129773619d562a44d0baec20abcc454"),
		new("prog", "CrWhyKHelHouCra41TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "555242144223221ada924a93f194235d63e15ef1680361d7f175472227454cbc"),
		new("prog", "CrWhyKKusHouCra45TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e3d5f967dde69b8fdb15a3dcc0734dcdd9678b0ed7d542c71e31a2ee697d83e"),
		new("prog", "CrWhyKKusTex9TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6087a649958420b73058b0f13b955ca9863e9e84e1bd8199892ffdf436564b78"),
		new("prog", "CrWhyKMedFooPro73TCar-320", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c33f4750c5bf39c4cef943f81bb2b87fd77d720e764d48e7839e094c105f651c"),
		new("prog", "CrWhyKMedIndFou67TLea-615", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7780e0eb5196018aba583da2506529034e63a02360b1a9419d00bfca79ef255"),
		new("prog", "CrWhyKMedMedandApoCra70TApo-3610", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "98d84659ada98aab7aca44cc9f9a8dfac8a818178f4f5b7cfaf43fe15ccce5e2"),
		new("prog", "CrWhyKPerTex11TTai-545", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b547fae64b74fdb74150c00ea05d137092b0123ce4bb3d0b6eb219f18fad6d1f"),
		new("prog", "CrWhyKPreIndFooCatPro74TBak-4110", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4a039d316257654897cfe12403ad3d1f03ef213b6827ee9cb2a3b72703c9381"),
		new("prog", "CrWhyKPreIndFooCatPro74TBut-4010", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b62b579c0ec111caec6bc407b564e779d545db9a6ca86c4207b37d730634eff1"),
		new("prog", "CrWhyKPriProHisComWor2TMas-1025", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1bf51eea9e556d16b17b65fd6bb138c0f77178fcc61dbaa46eaea965236e3f81"),
		new("prog", "CrWhyKPriProHisComWor2TPot-215", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5c79bef31289721d662777480a7e7522938ad6568848cf3aa89f6a99a61f40b"),
		new("prog", "CrWhyKPriProHisComWor2TPot-220", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "19b8b9c047f8c34ba2cbc5b5c0d5ac141c4d6cb7ebd2e6a421f83a5b97756211"),
		new("prog", "CrWhyKPriProHisComWor2TSme-930", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9fba84f1f7968f202dd4f1db64f3101b75a61f274eb757619a9fe290633d9be"),
		new("prog", "CrWhyKPunHouCra46TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4df9b3c8f8d7f5a0cb281530501f321a9d018aa190c1293633c3c565c391518"),
		new("prog", "CrWhyKPunTex10TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac7a8bf02ca529c31a7805f6ae9437d06dab6dfbf3f8c834aedff160b73f34ea"),
		new("prog", "CrWhyKRenFinIteCra81TCar-315", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e60adc5c4c716116a03e8d431c3668b9f0454599a85917cf5ff9cf3c0d09edd"),
		new("prog", "CrWhyKScySarFoo66", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ba5a6700511cd23f07cb0f7527b0017a4331e735117f18b12acf56e33560a0e"),
		new("prog", "HasConstruction", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "121aff88f59ca33dbeeedbbf76e5f4453453788ab30ac8d51d7ab837f1d329af"),
		new("prog", "ItemSeederAntiquityFood_flatbread", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e58408ec48981aaff167c7b48abdeab66b8b610108e554bbee32bfc2bd3b69e2"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_cassava_detox_press_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4369f89991f94aae46f7046535d5760c6919435e1cc46f17dbac69afd003979e"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dairy_earlymodern_food_french_cheese_gougere_puffs", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1726794da2fcaff6750be3083b2c273118cef8ec4690fa9b193736a92c78fadf"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Meat_earlymodern_food_qing_imperial_manhan_platter", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "59481ff9a6472838844d2aebeee84140ffd1b2cff445b1ecee5ac32d56731b22"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_qing_bird_nest_like_soup", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5abedab1ea93ea4440e7b1781465147ce33fe7538f9ca55b1a539ad186520eaa"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_injera_fermenting_batter", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2337a718f088f935954488f9c4378fb8876b638d23ed9f740025352b6624d4a"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Egg_medieval_food_egg_and_fruit_fast_pie", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b81d47857faf03ae830d0908605e2c0a1fdddcab2bc738907130a789c7566093"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Poultry_medieval_food_judhab_layered_chicken_bread", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "101d868f2c8de2ea02487138af565a1d846bd48b10de497edf379d2ee4bb37c0"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Pulse_medieval_food_byzantine_monastic_lentil_paste", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "28d774d205e9cafdf04aeed54de1522179a0174d44c4f492bc62a8acc062006c"),
	];
}
