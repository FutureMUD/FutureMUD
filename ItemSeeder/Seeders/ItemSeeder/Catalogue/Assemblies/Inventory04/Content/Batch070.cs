// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch070() =>
	[
		new("prog", "CrWhyKAncHidandLea52TLea-610", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "85b7f86e17b84223cd1c4fcfdc7086441ed3d73098d0bcaaf9cf0fc77d8baac0"),
		new("prog", "CrWhyKAncMetVes39TSil-2415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c601a4d138534135fdb50018d9abf7e82b6f2072f3bc47038706536fe423e204"),
		new("prog", "CrWhyKAncProandMobAid32TCar-320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "efead385c78949c1fffb4afc1e5c55ceff18e18d46efcca0cb169eb7f6d6b20d"),
		new("prog", "CrWhyKAncSurIns33TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "01da520cda751b8f76aa60f5cb1f80a80341d05e9bc930178ab623b929781604"),
		new("prog", "CrWhyKAncToo17TCar-316", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "90de26e6ecb6fb0bb8094e2c06d56f813e10e12fe63360b770000e75e39f9400"),
		new("prog", "CrWhyKAncToo17TPot-225", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4166aa39f853c1d24f1d383dd6fb4538ab13c015783b9451882a5ea341e0479"),
		new("prog", "CrWhyKEtrTex12TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5aa07349040f3da796dcb8d5515761d70be376f2188a3c7acc08a69c05803ffc"),
		new("prog", "CrWhyKHisWorFou1TBla-415", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd9b6325248acd0365b6210e1c12e64a8e688fb088aeb972d9266b4d61e894f4"),
		new("prog", "CrWhyKHisWorFou1TPot-215", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a707e652d66898d17cdd3700269cd9d34ea5d684c19fe2bc01ff3b7cfe3d16c"),
		new("prog", "CrWhyKHisWorFou1TTai-515", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c13dae08959acea6acaacc60b014e468e5e09efe9f21571613a59a271b0e7b73"),
		new("prog", "CrWhyKMedFooPro73TThr-3710", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa62cd60ca5d5d1b7b7427d90fa8e6457168e260aa7685e67317474d5ecc1db6"),
		new("prog", "CrWhyKMedIndFou67TBla-425", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "557f80d691829b4b57b13f3552bde19db6f923c165822226e986fd7617863869"),
		new("prog", "CrWhyKMedIndFou67TPap-3325", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "30d8934573e0d83d8d65527b311aceda0ef725df9df1d3e9c33d09bda63211f7"),
		new("prog", "CrWhyKMedIndFou67TPot-225", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1235bd5e34db948080af98718fc6489c4a5155fda429f7474ede016351c2a59"),
		new("prog", "CrWhyKMedWriandAdm69TBoo-3410", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3ea4b54d342bfda5a4e6b42bf445a2fb6c3da6cfbcaf48b5f44a34ff0633b7d"),
		new("prog", "CrWhyKRenandEarModJew80TSil-2415", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "098414b9645820d7b133126ecb0433b6cf6cea22b1d8c1546df0763ea1ddb729"),
		new("prog", "CrWhyKRomTex6TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6414b3d009624233d9829bec067d6d20ab1c1523baf02aa13af7f8ecbb04e659"),
		new("prog", "CrWhyKScySarHouCra47TCar-325", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aeec82794253dcbb518791579624ea77a1b78a03b1fe505b2bd472cafd1a5f33"),
		new("prog", "HasPottery", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "cfba6097f45d22eb23455db3be2527f9bf4ead2696d3d5e790a9058eab446499"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Bread_earlymodern_food_caribbean_cassava_bammy", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "52f2b73e6a916ff229296e7f19ce2f94d5db45089958c71ceae504b5b44adcea"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Egg_earlymodern_food_andean_chuno_egg_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "83e17230f5351742617b13200df0b4ed7b790664f12dc6f611d5a708ddeee1a7"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_naval_salmagundi_platter", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0452ff2bcccbc431f884b1b4964fa117aeab7fb80febf2407b5b625675e17d60"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_sussex_like_pond_pudding", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d05ba41ad6dc699c5e768a06a14dcf44fdfa6adc8881a23ea26b30a955f3186"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Dumpling_medieval_food_buddhist_vegetable_baozi", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "087cc0977102ae52baa6ea453222f44f6c93e99a5e6ae8ec9ef3fbb07c600d30"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Fish_medieval_food_white_fasting_blaunche", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4a0713e143fc4d12a75fa3df71173904e5183e658e7dfb70ce8de39aef4b8c0"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Soup_medieval_food_korean_mandu_broth", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac3b05a9815154c9306be8a85a582711cbb5bd9dc9e50158f6951b7d6984e0cd"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_uncooked_ravioli_tray", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc315b3aece6f0369e87e6d872616eddd3280c9685e024b89c09e1f6c7967b87"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_bread_crumb_gnocchi", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "89fef22e943bbeb9d12e9da812f08b106ecc20d7e2b3a791e376a81700ae716d"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Meat_renaissance_food_aztec_market_insect_seed_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "99d3ae2e3553c14ebd3dd716524a059b639107af32638ac01a36a040322b498c"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Soup_renaissance_food_benin_palace_pepper_soup", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "85dff3ee7dfe47c92b4e4b58479c7abd2fe27e1cdad05d60eb5071a080e6b3b0"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_aged_hard_cheese", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "91f3e2a7df64accd4bb075ab7d48415bc86c8875f48d4fd35a9315817679a551"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_buttered_roots", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3a54c8ccf4547e6917cf19b0020fbe0f3269f6831d990f08a6c07454e2887af"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_sorghum", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49b45d9da8f369d6074186e8c0910c3ffa4d4e5ab6b745e144afefa750c1134e"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_broth_boiled_rice", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a90fbff4aa64297b05f4f568c691e88c2cb74535a902a9392e99fc9560030ec4"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Nut_preindustrial_food_boiled_chestnut_bowl", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7203b8a3929e7c1bc974091ccf1fee2f47e29e7afb9002d28f159fdcc7ded924"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_palm_oil_porridge", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cc6f16a15608058ed8f327c67f22fc59d447efc57a757a5df891afd803ffa2e"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Shellfish_preindustrial_food_baked_stuffed_clams", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc66cb05c7ca39c697ecc901cf071a5e1c60e452988b6a132bec413ca8fc0103"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Vegetable_preindustrial_food_asparagus_and_herb_mash", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "291c06df4a70b1c6201cc15a6007594b124699c0a7fc4073abc86d95239d3e06"),
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / Dairy", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eff39ed37fb63417f3fc2f42b17dbdc0a551e45a1c9552106d54f17f376c453d"),
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / Grain", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ea4582b6689ad178aeab1914fd443ebba6c0ca55a7cd6f246e45bbb6c257beb"),
	];
}
