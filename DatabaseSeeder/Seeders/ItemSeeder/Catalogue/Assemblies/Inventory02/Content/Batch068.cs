// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch068() =>
	[
		new("prog", "CrUseKMedFooPro73TBla-425", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7875c5e938e9fcadc8cb318c442b508c09bcb8081a115e71b83a68f7a33d9718"),
		new("prog", "CrUseKPerTex11TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c4b9bce7cd3df7bddf0fdf8c72a5bec61ab5ea64f5a9249fd53f85759040843"),
		new("prog", "CrUseKPreIndFooCatPro74TMil-3815", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d1c98160c3373946323080a912b9270a91332e21b652b2032e47d99f0957e28"),
		new("prog", "CrUseKRenFinIteCra81TGla-1115", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "14f5874900b2f47f73bb202ba298e2fa98917e9e2e146b99f75081eec557118b"),
		new("prog", "CrUseKRenandEarModLoc79TBla-415", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff239245b60ddc5b160f4716348b8f22cb2eac742bcbfb5a188f60c7d0d9f64f"),
		new("prog", "CrUseKRomTex6TTai-550", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c4b20d539c346febc15c18cb23d85c4df7d98525dbd16f9b70d10effbc3dad2"),
		new("prog", "CrWhyKAncCoo35TBla-415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "73431ba08e68e83bfe922c75d31a238711b957657a91c54f68386c4bc33c3eed"),
		new("prog", "CrWhyKAncHidandLea52TLea-645", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "42ba9a8844d07353a03fcf57fd182da523265f8cbd056bc0fe813ba94cc6c836"),
		new("prog", "CrWhyKAncInkandPigMak27TPot-215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "02b55f5158246959ede56d29746dc4583706b66b162c0ffbeb44e644de5ac213"),
		new("prog", "CrWhyKAncJewCra23TGem-2525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e48c7818622eebe4b51b12bb67497933f82fa039c3610b3f73e3970aa4126129"),
		new("prog", "CrWhyKAncTexPro3TFar-141", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "05ce36778cb6fcbd962643ce26778cc595a4c6f93b32cb5bccf51cce23b2df55"),
		new("prog", "CrWhyKAncToo17TPot-216", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "763cfb8a70d34fdb0693e38fa431737f3cae88fd59b4b2986938c361c405d970"),
		new("prog", "CrWhyKAncToo17TTai-512", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "28c6984fff40381d0b1d2649f058f67555963679d69fcb5bbe065855b745f646"),
		new("prog", "CrWhyKCelTex7TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c744a26354da95a95d61d066ed2bcdc212d2384bd298bde2886e9a87bf29b6e"),
		new("prog", "CrWhyKEtrTex12TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "987f6ebb4051f36888280e500a721badab638d1790039fe97fe171aff437255c"),
		new("prog", "CrWhyKGunManCorBlaPow77TPow-4225", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "05d3a5de059a06f4a2941ddf5313facbcc88d482b0e70ed363c7266a61f7c233"),
		new("prog", "CrWhyKHelTex4TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1de16257b5b11f469703c7d366c8449028f8ca5a5a77b02a1074d4bc5fcf5c26"),
		new("prog", "CrWhyKMedIndFou67TPar-3215", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f6ef08580fe46687b87760acc744302eadd908a83c015d6103a9e235d2dd064"),
		new("prog", "CrWhyKPriProHisComWor2TGla-1125", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cde6c7c17539c712ddd72a732c1b53f939ca3a399c78c18807bb42eac24aae2"),
		new("prog", "CrWhyKRomTex6TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "41970af6cd216102667abc8a4cc2f3db4589161729d830a773c4894190ae92cc"),
		new("prog", "CrWhyKScySarHouCra47TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ec80254eb229c529dfb1e21026815106a39dc61011843bce3ccc0c3bc34fa9a"),
		new("prog", "CrWhyKScySarTex14TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d11e6c869c7bf91a85e95a3b4b4ae24d7c6eefdc3ef04f085af3784860248e8"),
		new("prog", "ItemSeederAntiquityFood_pulse_stew", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d260d67bf1b480128d8e78ffe9f46e4ccee898b1426f72074fd2070a2cbfec8"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_caribbean_enslaved_cornmeal_ration", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c71b474b33358adbac6e71fefca60bf025daab09869a04e259e47d0050d63c7"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_joseon_royal_gujeolpan", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af7ddd4d9a45a9ed7b87631bc2fc183e86a0bd4378fd09a5cab039529d38e4fc"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_dutch_shipboard_cheese_ration", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "491dc02fa65c15f27a03730b6e9aac0eae4069931d49356802cfa7e39da98542"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_ethiopian_royal_layered_injera", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "11ce79abc9c882e4d7a6e33d38873c45d3bdf202417c8b8f4ea552cdca3f56c4"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_buddhist_mock_duck_roll", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b894c80862747fc5ce9e7f1578394f6fc682a0d95940bee87ea5e956e6af732"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Bread_renaissance_food_andean_chicha_residue_bread", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d547ee1da41d9d335bd8263aecc0c910a3ef3f0e419841ac3ad4b3c62864ee11"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Preserved_renaissance_food_andean_army_charqui_ration", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ed3fad5debbed7a74b0015bd704f569a2fdf6748ab48b02b892c20b2c4c1b89"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_brining_cheese_wheel", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8e3b1787a3a0ab090ce19fb864e4ee2a208f57bb593892e3c222ffdb8d5ff65"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_maize_dumpling_dough", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "412122584b59c70c5052d0181acfc17bb61dc954d16d5c9f28a76dfb9616c8bf"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_buttered_grain", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a39bf838cf644cfdeb8e5e1d9cea2d53dd9cf387cb01ae9a2b11c50228b520d"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Fish_preindustrial_food_barley_crusted_fried_fish", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "567ccdaff8037573d1d79435080bb327814f93e166333b03c1c06042d1e9cebd"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_army_grain_ration_disc", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "150ea840cd99540b35553ddc155aed5115e89a762b201e623638964f105a00e2"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_yoghurt_and_greens_broth", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c220e2b2a6b5ff33d0cf5c37540942de7c236626bbd02a886f59acfe6dc18767"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Sweet_preindustrial_food_honey_grain_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e8465ca9cf8a69c5687c8d5f0217ed167e15bc9ed7568fe9532eff270e8c441"),
		new("tag", "Era / Early Modern Era", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea9fb57f0cd8b5c3f78c946425d1f624f9f271018b0db7cea7414d9b259d5b40"),
		new("tag", "Era / Nuclear Era", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "2967d3b658f9d8690dbf18640f653ec5830e9e555cce1c4b611d2f62a977f696"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Coffee", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a192e3dc500eb1730ffd048d3dcd552d01225a7862aee8bccbfbd48604b976f"),
	];
}
