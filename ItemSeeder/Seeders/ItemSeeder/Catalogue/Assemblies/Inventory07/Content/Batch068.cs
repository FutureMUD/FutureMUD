// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch068() =>
	[
		new("prog", "CrUseKPunTex10TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "828bc05029bd8ce45b6089b3984251acebf533898781cdabee9411ff6cd617cd"),
		new("prog", "CrUseKRenFinIteCra81TTai-515", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fa750190a288c2700e11dfd65be158576775900c11e2b936ee2342be86daf4e"),
		new("prog", "CrUseKScySarHouCra47TLea-620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2459975674a4ad72ffc00016393fa9c37ecd34f936589b3edfb8466ac85f8622"),
		new("prog", "CrUseKScySarTex14TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd673dac00e11740d65212304d6f03e7f57cefe8933b01f4e9819e4308e312b2"),
		new("prog", "CrUseKScySarTex14TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c3d05304c8d0d4d2e9696e5da5f4398d33d4123a8e09f9e2b89a054f7bcd2de"),
		new("prog", "CrWhyKAnaTex13TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "23cd2ae1805271ba6463ed717bb260a2a0bf4616306a90f560e86647dd0380f6"),
		new("prog", "CrWhyKAnaTex13TTai-545", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "22a1c5e08c5ed18b13fb9e8caf19dc0498f1875b88d5c4c6aec0c37f12a44068"),
		new("prog", "CrWhyKAncAgrPro54TCoo-710", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c33fbf547d19c9d98d2aeaff20102b22cbbcd0f770a462743c8b428db384a46"),
		new("prog", "CrWhyKAncComCloCra22TGla-1120", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "df104e035cd23c76ab0729510c008a3cd12a8b50878b5ad337a4ba4449e2e081"),
		new("prog", "CrWhyKAncComCloCra22TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf672a8f99ecc47028e6b9d399a12448dc329e7af3f7ae2a6d09901085d78865"),
		new("prog", "CrWhyKAncMetVes39TBla-415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e52794329dbfde6a4d0e01a100de653ce86acdd20f5fabe1e1fb96b9a8e77ab"),
		new("prog", "CrWhyKAncTexPro3TWea-1510", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0fafd15dd57f54a08d9a154912b48a19035b8ce5f488ba5680fc1623291c4cda"),
		new("prog", "CrWhyKAncWea19TWea-1815", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a03c99ce4d66b533487ce35742d0091f76d6e887881ade6823148b27e717323c"),
		new("prog", "CrWhyKMedIndFou67TGol-3525", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18b6012abfe6fa8125d74fc21b4c149066bf026d00c73826b1f95c86ad4a871f"),
		new("prog", "CrWhyKPerTex11TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbd92a8c66a1b1e922805d989bda7242f800f479cb4e8d63d782256db191d5a6"),
		new("prog", "CrWhyKPriProHisComWor2TPai-1315", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f33379273bc483ce4a79fb9a460e4fc42f9e2ce3955d6f22d447a9371930808"),
		new("prog", "CrWhyKRenFinIteCra81TGla-1115", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab6f9e8fdc1a0e6cb32bca460c34e6551f64b1500217dff95fb7de6e4c81d754"),
		new("prog", "CrWhyKRenandEarModLoc79TBla-415", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d79c5ac6b8f5ecf07e493d7e2a20124ec33ae84262612f3f359f7b9c3559a21f"),
		new("prog", "HasCarpentry", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "02143fcab88ab0411de4035119ca573886d4ec6af1f4f8a7c09b0802d97c9b24"),
		new("prog", "HasForestry", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "2dac62a98334e0cb0880deebc95891ef26cc0cbb5e56b097ddb1f3a455447ae7"),
		new("prog", "HasMilling", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "dc7f1aff8d8fd0752f7cd37559335d9528f009a7552eaab33af36f0695206451"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_pulled_sugar_decoration_ribbon", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f36bd0c0c5caa105415a57fc5b0edac7414a0ae2f262d2d0a4ac0b88274721cb"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_soaking_salt_cod_bundle", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6deb925d6325dd4788d4cdfab4b816641c1cc6f5f2d6f0207ff27624d006af6b"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Condiment_earlymodern_food_company_mango_like_chutney_toast", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6c8390a0ed234aa88155e05ba7b601d932bb811d912e560b27b920ede452644"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_tokugawa_miso_pickled_fish", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "84f52b175ac561c7f67d5f66e0edd8c55e9ae495f8f5d32de025fe2a45eb6cde"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_joseon_tteokguk_ritual_soup", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16bef0b8159e464de5dfc13b8430457b59cd8465f074cc84631ce759281826a3"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_caribbean_plantation_sugar_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "99cc4d2fa795e28b15c27d46a544c16adea15597a681d8f5abbdbcc771196cc7"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_faludhaj_starch_confection", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07207d8538e4da72fd606b465b6e0bbfe3e6f634973f65ea1dc8d26213a04be4"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_capon_forcemeat_bowl", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "afff92e024364aceea81df0d0332f04e2a5dce5a2a0f072de4dee68faa39c1a0"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Fish_renaissance_food_andean_sun_dried_fish_maize_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e16406e4fdf85ccb1cc6226bec9b415dc02452f2485c0d65e9c003f6bd80a800"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Stew_renaissance_food_joseon_royal_sinseollo", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "19996c49ef89aa737c2e44627d0368c4dacedf3f97bbd8392e68bc3a313af60a"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_spanish_candied_sweet_potato", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9f7a1ed7c92e929c7acd8c6d64f8f0836ea5852f1acb43ff5f6cdb8f7aa373f"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_ale_soaked_bread", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e51243b24247fbf1afc97bb8e7b3a7d6cbb7038f0da338f7a5e7f49517572d7"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_bean_filled_steamed_bun", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0afe8a704c5f0fafd87cb0a1c55fc2f5b02090840a89a757a2bed74f9bdf138"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_corn_hearth_flatbread", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d9140fd8f94c4639e5c88c418cdbe724a8d1ade400a9d01e4dca5697a0e4ff7"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Condiment_preindustrial_food_fish_herb_paste", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f070dadaea6ec999e72d36367632da0159d6da32d924edec49309bba21ff592c"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_buttered_bread", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b5b0635fed48a835d9679880e9dec714ddda3385b17a24ca3cc87e37b68ef55"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_apple_filled_dumplings", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b17d9ca8fef2f82a629097468c7a1b33091ccdaacbd5b2cac2ad5424f290537"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_bread_crumb_dumpling", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a74abe94588fadd679841b854baef5a36092d3f4c7752fd1690f19cbc47aad7a"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_berry_pulp_travel_sheet", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d62a4887a7254e76348ce12a0e4268709539fc8cb93fed6afae8886b14a90568"),
	];
}
