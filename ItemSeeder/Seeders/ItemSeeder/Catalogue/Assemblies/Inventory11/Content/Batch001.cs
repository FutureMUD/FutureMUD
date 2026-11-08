// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory11Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("commodity-spoilage", "Raw meat commodity spoilage", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6549eebabf07f3eb8371e5ec8dcd9244f5d6be694efd104413c1a1bbb3970e2"),
		new("component", "PreparedFood_Antiquity_Fruit", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "50f568ffc5a25f2295eecdd2a42817f395ad209bec0de71383cb0cd8994b9397"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_cloth_boiled_plum_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f257b927fb65e64dc06a7ad195e9951e3a944dab3faeaa38bef95c96200dbab4"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_steamed_suet_dumplings", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "80852c3440cdcf18b0c46a894770aaaf1453e023e29a4ed09d9092c88c46d728"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_caribbean_enslaved_saltfish_cornmeal", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "894f5227b7257aaf310750f43c95ed8bd8a69fd275d3620756c0e59c1e5ff1e9"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_caribbean_rum_fruit_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2e5f475f4233e9cd029817c8e30f7524d8fe649ff68b718b23e7ce31dfca45e"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_coffee_house_brioche", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f938adfe50edb42b8bf5de9214f3a6500ed84fca89b1f42be4b8c1c89e007d77"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_asure_palace_garnish", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6780a36866764b6747de8f4eb15a1590e347f4cedea9a90c167e4ed51adf8251"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_coffee_house_baklava", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8b47bbaa01363fe8dc64aca544f07b67cb104af4b77c3c3f0a71c75a906cfc1"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_coffee_house_lokma", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d8064eb2681d215386ee4962668ae857f2ec50a1a894db685db0ace5d5ff066"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_preserved_cherry_meat", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6e1ddd7a6ca3c229dbb9e446e84fc944ad7c598dee171dd3649cc35db141e9b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_ramadan_pide_loaf", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e6252f06b4f31f926ab56f5b8ad70937bd39b70b0b9b1aa0916d4469ba36046"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_mooncake_salted_yolk", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f4d19cfbba6d5f863c6c1841643c167dabb67d904ba683e1bc226e397d46ff6"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_osmanthus_like_rice_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "11e18e07710ddba787bf95b5fa8869940aa2381a24b2165308769fece3737c4c"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_safavid_sour_fruit_leather_soup", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "01875a6d27b6f498d4e0eb66099c7d102b855d9d6ff6a7dca2b34bc822f089f3"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_swahili_clove_meat_pilau", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e50cef8a8d17a06abc33b2879f86acd6bb046b35b68ac0e45106d1e6070f352"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_castella_tea_slice", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d4911770c37542ee79b577081483517fd14603a93a9470d6cac1c78b286561c"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_west_african_rice_bean_feast", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ac86303a533c5875e6e8b64d1379958a3770086f63c4268b587bd65c4a2187b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_woodlands_maple_sugar_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf8a7d6872024ed7e66c75ccc46a53bb493483ec45c7cbab211cf75f409fbff8"),
		new("component", "PreparedFood_Catalogue_medieval_food_buddhist_mock_duck_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0bc3dfe1266d4c5b9343ba6e0849bd532df7234529efee7821298f981886122"),
		new("component", "PreparedFood_Catalogue_medieval_food_byzantine_honey_vinegar_fish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "db7b2454f54d81be7df898ac5f7f2844cebe54c19dfda768ec2cd35dc9857583"),
		new("component", "PreparedFood_Catalogue_medieval_food_ethiopian_spiced_meat_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a6694c8739030c50dca17cc7d5c58379e66b66bd6dcfa5778b2a9cd66d12e71"),
		new("component", "PreparedFood_Catalogue_medieval_food_fasting_couscous_fruit_mound", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1da1f14508255e60a0c53495077a6c37c3c92cc6a1ff4d420c276ad960cd403c"),
		new("component", "PreparedFood_Catalogue_medieval_food_fasting_mushroom_jiaozi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1f2003d722e6abae434ab761ef37e19eaa5ae97d7e94955dfd6f5480e82ca09"),
		new("component", "PreparedFood_Catalogue_medieval_food_galantine_of_game", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ca3a498a3fd1b8d4d892b2ccbd21f03abea238c4b6af0512c343c203db4b2c4"),
		new("component", "PreparedFood_Catalogue_medieval_food_khushkananaj_date_filling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4cfa1170c5ba18bb36db4521e151248572288bb67c0fafbf5e34aabe7e25d8d5"),
		new("component", "PreparedFood_Catalogue_medieval_food_korean_songpyeon_festival_cakes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cef78b0257e99bd4d477bc039208d89cbc5e6d80267ef436667cc9674549436e"),
		new("component", "PreparedFood_Catalogue_medieval_food_lenten_almond_fish_pottage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3262a980cf11d73dc73f517b4cac8b0d02aefe1a48c2b2e10a6d9c08001ebb9"),
		new("component", "PreparedFood_Catalogue_medieval_food_manti_yogurt_dumpling_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "00d97f916f902cbca4ebc67cfb9bf8a3b9bfa51d8b7c7446c44fb97850f6c770"),
		new("component", "PreparedFood_Catalogue_medieval_food_pistachio_like_nut_halwa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e92c1db00b5b0626048c1ca7357496b65252e65466e2046b772ff30ee993ed5"),
		new("component", "PreparedFood_Catalogue_medieval_food_royal_snow_cooling_sweet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "871adc00a98517e88409a2364428fc09be0039a0651295cba9fd4dd27f15a2e6"),
		new("component", "PreparedFood_Catalogue_medieval_food_saffroned_frumenty_with_meat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a8ed2a44b0c60fd87755166d32217a8832533d455825b42e4af78d88d4ea190"),
		new("component", "PreparedFood_Catalogue_medieval_food_sahel_market_meat_rice_paste", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9bd4edbac2a35e71179b75a97d2298eab2a4edd1e723a36ebfe6c777f46f6929"),
		new("component", "PreparedFood_Catalogue_medieval_food_small_hand_coffin_pie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f6561889b9c590012ff2c7d3188597795bd9647ebd998bd1161c62590cc5911"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_asparagus_and_leek_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbdea993abe1001368c45094bb366136feb315be01a438a4930665ca7568840d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_root_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "095c98f1d223fa272f0ccbcc2e227cd43acbe740774b83c03bf7eac4ece69e29"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beef_and_lentil_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "957af8566c57bdfb1ac242b8336995bfe93847838cee691aa8776381b27c4f64"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_and_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5350a71732922e6c6d221dcb1867f4c8edf8a9c4c2195d0fbc58b1eaa82ce854"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_barley", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1474d1f09a0c3cdcd446c040dad7153e66ff0d0518e3f54c3836a37a5cb3ce28"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_turnip_chunks", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e919e9ef01a7a3e857338076c99c9898e209e8abfd89f5135db9330eeb6e2cab"),
	];
}
