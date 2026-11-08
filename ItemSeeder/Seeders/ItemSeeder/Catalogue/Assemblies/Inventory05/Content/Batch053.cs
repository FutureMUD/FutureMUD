// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch053() =>
	[
		new("item", "preindustrial_food_boiled_oat_groats", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f70873c739db118fafebbca43d300fe8cba05d92b79861e601c21c326d7a25b6"),
		new("item", "preindustrial_food_branny_ration_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fce349080aee348555463239816ba2e5d5edfcffc9fd2b0908d94eed93003891"),
		new("item", "preindustrial_food_brined_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "19aceb827718929df62429bc9fc1ad3e20b436aa12f16aa1305b007da3a66864"),
		new("item", "preindustrial_food_broad_beans_with_cabbage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75b01391d1d28772aebe22343e1d74654563a45799df9f8c1efc09d32ff13c33"),
		new("item", "preindustrial_food_buttered_cabbage_ribbons", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6a37a6070eaaf64115632a3c0ea4ec7e8e66431fa132030b29e92c77aa38065"),
		new("item", "preindustrial_food_cacao_fruit_confection", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f15b18445511ea2504656726832746b6b40f32723dc3163aa530173276510783"),
		new("item", "preindustrial_food_cacao_spiced_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e676f93ffdfbc0ddcc857b062a0959ca521fa8ca2d53df21ff412ded35962b1"),
		new("item", "preindustrial_food_candied_whole_fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "96cfcb0fddbf4df13148e1745fcf9dd83ff000273b32a0582012ccccefd0c19b"),
		new("item", "preindustrial_food_charred_carrot_roots", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f950bd6c4d22b16f94955777d66fd552525dc474f3876dbce941d74ffa766328"),
		new("item", "preindustrial_food_cheese_baked_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61195b9a6878cb5224d7ec6ea6d9889947a4b7b75b7ba4e906f69b6e2ff6c0da"),
		new("item", "preindustrial_food_chickpea_and_lentil_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "84f24218f85ce22821475f536e2095051fd13b19c41bd6a88893acd51b5b7650"),
		new("item", "preindustrial_food_clay_baked_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e034f0a3d76cf24821543725fb54ee30e76a13504254a614dbc8e8c87f8be16d"),
		new("item", "preindustrial_food_clear_clam_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ecdd5e25358c0ae3bc8e2a7a8613933721dc39ab12c8bd90ab5b0637aa456af"),
		new("item", "preindustrial_food_cornmeal_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c974f4a0f367def6fa2c2e1b76913e1345b1f16bea1614ee1f0317e06e5e615"),
		new("item", "preindustrial_food_crisp_fried_shellfish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36271cc2f27435de5f5eebc9d7b98a9964c173f1d45335066aec1bd3f3e95111"),
		new("item", "preindustrial_food_curd_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff3c84f787f925040715f9a6997e58b2242c5942c108f9eb59a9d9717e4dd320"),
		new("item", "preindustrial_food_dandelion_leaf_salad", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df22cc7e18840cfe2baea05cd39bf3d276d7da3c0de669bc14edb5d0b7fd2f62"),
		new("item", "preindustrial_food_dried_mushroom_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3344e041233ebaf136dadc72408f0e51e97afacab050daae8191984372397c00"),
		new("item", "preindustrial_food_dried_pear_slices", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "344f5d75ca0eb80f2130419faffad1bf68447aa27d686aff75f9a69d54f3c403"),
		new("item", "preindustrial_food_dried_sea_greens_sheet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e93410267d4cc4e23125ecda144df1699c508929b9a905582750ac4e345216b1"),
		new("item", "preindustrial_food_egg_herb_spread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8902d2b32432f340fbbf6d9089a469bf708fefb524009061500713b4d5571b5"),
		new("item", "preindustrial_food_eggs_baked_with_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8530e176ebe5cacb84d26d31d4952a9d3072f62af5141bc64635e1a9097bb15f"),
		new("item", "preindustrial_food_ember_roasted_fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a77dc39197f33f17c8b70de3624c694b2605d43acba5fc8c448c3a884084f9b7"),
		new("item", "preindustrial_food_fermented_fish_cakes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcc9d6f9a758bd300b941e52b992c39011025482a8ef192e342c1fa886fb8c3d"),
		new("item", "preindustrial_food_fermented_grain_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0b5bf437afab1f644f704999d7999a5107bb8cdb71000edf1f2d98ab5dc7f28"),
		new("item", "preindustrial_food_fermented_vegetable_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d948f0d291a512bbe4d7026cadc1fafa5fdff89d6eefbeae8eb9c1ee5d777911"),
		new("item", "preindustrial_food_festival_stamped_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c584d6408594ea0eab98ac56fa2e778e6530c35fdbc59942e7064b38d9c7cae8"),
		new("item", "preindustrial_food_field_workers_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcc555ed38eaee9f66ee2ec499a702203e60697b0648fad1d38ed8bcb903dcff"),
		new("item", "preindustrial_food_filled_pasta_parcels", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "786d3264a60e67eb2b0765fd28d105db94c1aa5c25355ca2fd8bd7040fabfcec"),
		new("item", "preindustrial_food_fish_egg_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2fddb58d4b8ddd962a40ff4562f9dd6a3f7d8da38512b7bd0f89068e4a2d545"),
		new("item", "preindustrial_food_fish_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a907253c605ffcb62f4bbc17810271c1ec4b0dad69fbf5db4e08a124709fb59"),
		new("item", "preindustrial_food_flaky_wheat_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e1cae6664f055c7f56cb266dde1946485f61514c045ad570237ff57efc0d198"),
		new("item", "preindustrial_food_fowl_and_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c95b0d3ceb96d2f65ccd0151c31dc0c37a3fe99c67c2cbdc642f39e650e59dc"),
		new("item", "preindustrial_food_fresh_grape_bunch", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1ef065c97fab2eadb99ef7c86f5396481fd525520050630becadfffd68d7d6e"),
		new("item", "preindustrial_food_fresh_pineapple_slices", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9abf852ce34c8714fb143463543d32ee7b361e7459c8d09fdb7e43e5ab416b37"),
		new("item", "preindustrial_food_fried_roe_cakes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "076259bf1cf8996fde8f93fed1e9d596f2ee5ae34b9added61d017e33d4165b3"),
		new("item", "preindustrial_food_fruit_and_cream", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "111fd5dc0300b3f8cde5a375afe86d408043eeaf53c27ed6d3fce3853a13d4dd"),
		new("item", "preindustrial_food_fruit_and_fresh_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c69168f4a3034f1efc3c4c7d49bd8cd89cf4f1e6339dd45ec3dbe3c895e45f0"),
		new("item", "preindustrial_food_fruit_bread_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "377b9c3f460e2b9fbf1e5432a6ec8e2d956ee603276c92eb85dfd1327df576da"),
		new("item", "preindustrial_food_fruit_rolled_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6127b566a0d85276f9c90e2f3b6fa67aa58a986ccdadfb4978071586da54b935"),
	];
}
