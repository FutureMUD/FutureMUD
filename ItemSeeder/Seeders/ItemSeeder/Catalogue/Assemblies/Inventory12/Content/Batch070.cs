// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch070() =>
	[
		new("prog", "CrWhyKAncHouCra38TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c94649c5760793916a6da70ea9443282c48c71f512ee98fe72e830048de3a55"),
		new("prog", "CrWhyKAncJewCra23TBon-2215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9cb331889402fed51ddfd6e25f425204f9f0ca4a2cb64d4ce9ccad6ed6e5811"),
		new("prog", "CrWhyKAncMedTreSup30TMed-2615", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fbddaf9ca0b04ff3f2160b4d467b80559ae93a8dcdc21efafb210e1c3709aa2"),
		new("prog", "CrWhyKAncMetVes39TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d5dc560dbba8f45ef23a2a4d735033c8785bae6800b90aa2a117361a936a187"),
		new("prog", "CrWhyKAncScrandCodBin29TLea-630", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a848673d5d7904104c83e51cfedff077fa0c7b3badcb09ef1c168e3142117dfb"),
		new("prog", "CrWhyKAncScrandCodBin29TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6345c574aa97fdc95ff7bcd2d1729da6de016df1217b6b6f6d578dc7a270651"),
		new("prog", "CrWhyKAncWea19TWea-1825", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "647330b357d58213c539fd0f2d11d7625ff99227eee33098f6e0aea7768b815e"),
		new("prog", "CrWhyKCelHouCra48TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "da0a717fb902d972b8db5ed00d2e6fda14f257726efc6b89b8d9d2340baac8a6"),
		new("prog", "CrWhyKEgyTex5TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9367368d35527fce09f2cd6f12f6be90c5644e05da4e8abc5e86d51ee2134abf"),
		new("prog", "CrWhyKGerHouCra49TCar-320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "74b3f9e0034f8822c8547c34747439a5cbe5834f79e6a0b1daf49c1d9e6d5e81"),
		new("prog", "CrWhyKHisWorFou1TCoo-710", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "71bcb28f286583e90ea5faffd966e68cab64e669f9b9cfed0138515ab9d6144f"),
		new("prog", "CrWhyKKusTex9TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "00a4bf0bb6fc2292fc1c68e9a23d5e78b8b998986ddf3d0b5d0c263b48f9b817"),
		new("prog", "CrWhyKKusTex9TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5bf6198ef74fbb2f596ef37269fce149dff36850496b79e84771f95f71f6e39"),
		new("prog", "CrWhyKMedFooPro73TBla-420", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce22184c68be9ae4ce4f2ff816060423e8e9da6161a6c5f010ca30ebf6a28cfe"),
		new("prog", "CrWhyKMedIndFou67TTai-510", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d66b6a1e3b2000f8cc74c5bbc05386bf384ff4f9b08113ddb52d3231b0d1287"),
		new("prog", "CrWhyKPriProHisComWor2TLab-815", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7a7cedeb9884e2b6b87555b1224660b8491d167795c2df040714708aad68b0d"),
		new("prog", "CrWhyKPriProHisComWor2TSme-925", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa6b73bf781957908bfbc94dbf3b541b6b6fccdf871b6ca12d2f72a2282e174a"),
		new("prog", "CrWhyKPriProHisComWor2TSur-1215", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "84ce30ca0ba4a02f6c9a5cbcfd2b53d1c0c49f3b77dad016607e84c55aa05db0"),
		new("prog", "CrWhyKPunTex10TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "acbfb136d3e6dd8790791c3be258914d3448db852fa589e2bec21a84b6f13003"),
		new("prog", "CrWhyKRenFinIteCra81TApo-3615", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7ce989660e461781596484756d1294a1935e248bebdc84a88aa51b05c730235"),
		new("prog", "CrWhyTLab-80", "crafts", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "8aa281a27539502ec4e7d17c3bd95bbf84c89db37995d85312637db7052032e1"),
		new("prog", "HasLabouring", "foundations", [], [], ItemSeederOwnershipPolicy.StockAggregate, "5225a8f0a82ce71b55bf02156ea90d61415e0b92d29f96d9283c67e9bf083bde"),
		new("prog", "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dumpling_earlymodern_food_british_steamed_suet_dumplings", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c8bcabc80456be987646dd0795d031b01b8b207dedb5152d8d0c42fcd0a334f"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_coloured_sugar_paste_sheet", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "74269d27e3165b362d92fdbf1f1aedd93134fdecd4820cc2a7c4da6a07627492"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Fish_medieval_food_fasting_trencher_fish_sops", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "782ad1349bc7270e4d10f7b53035abd18d8998e49d0e8174fc11e6af685a51ea"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_jaggery_like_millet_payasa", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "759c12f99b317f6db4fe12494e8eb7033a8c2905ad940b32eef9240729fecc38"),
		new("prog", "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_malt_sugar_animal_figure", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1125bf28c526921355e7349885e47639e0c76d95326b1f9e7d76aa21e75ac4a1"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_andean_quinoa_like_grain_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "41966c6cba4257b8cfb4f7ae02307bcc6ddfd54bbf65c04506af7b7d271e14d9"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_songhai_millet_couscous_feast", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18764a1d932f20fdfe181f3c98b1f4b0c192c4e23396ada4af8b64e62ac33318"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Pulse_renaissance_food_benin_bean_leaf_pudding", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a5019a688af96159cbfb96428c613581482e53c5f64792fadfe36cb4d1d7606"),
		new("prog", "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_aztec_amaranth_deity_figure", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "08762684b5672e87be08070f2ed5a6d27dc16c62612722c987919a340f9a7351"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_brined_meat_joint", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b75c3f87f87d8903a400807c37101a49665b818308e8792591dcb890243d274"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_grain_fermentation_starter_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "705f3ca96eb1de6e530d0471f3ed932a24e7b54e428def50d43e3ff517f30d9e"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Intermediate_Noodle_preindustrial_food_rice_noodle_sheet", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d31f093f3f842366f3c08fedcbdd226e377077a9f9ef5baca24106da06091709"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_barley_campaign_biscuit", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f942420dd6ae1a8f1df34f904da9cb4588b6df2285481f0dcba69e8470422c96"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_fermented_rice_flatbread", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fa9dd8a8d962418c68dee460c62207ff9bb5da006dc466a4c96b90df3d52487"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Condiment_preindustrial_food_cabbage_relish", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a79b3ba483e2e58055e36d3063ad5d819c84e57ccbacc1987cfb110b196a6cc"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_boiled_millet_bread_ball", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "389dcaedea84e0250bb77f29cb2dab14bed8073b084767b2410371e8658f81b0"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_millet", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d893dc7e23e7683441c6a33b562a5d2d8fb4f28fedd6e95955118c1d7c3fb93e"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_broad_rice_noodles", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "120ca489faace1cdbf474ddda5a9c61e1e6e9270bd2957324739045301e45fb9"),
	];
}
