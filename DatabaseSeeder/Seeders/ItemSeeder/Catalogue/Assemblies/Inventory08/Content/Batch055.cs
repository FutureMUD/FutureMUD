// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch055() =>
	[
		new("item", "preindustrial_food_herb_baked_shellfish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "69ab8cd56bccd7b1c7f3f60f63352ef9d5fe4a1f19a4b70165db30162788826b"),
		new("item", "preindustrial_food_herb_oil_toast", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c6ef75c8f2fd8ae60a7d4da98e9ab50988ab38d1521d777490127191676c038"),
		new("item", "preindustrial_food_herb_scrambled_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a697c40803082fb948cdaea0474b485e751bfa83d6d0783e4e049f60d351c48d"),
		new("item", "preindustrial_food_herbed_cultured_cream", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8399546b381b59fc33cd9d81c674f37cad6565133e3fb5414138d142b5f763ad"),
		new("item", "preindustrial_food_honey_grain_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "210a461ce47fe7430d941280a2da165e8e21671cfe788cb950d2f209e448f0fc"),
		new("item", "preindustrial_food_honeyed_celebration_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2649ee46eb6d4dd2c7ba1a224890fdace972fa756b91e1b77311b7c831c0f0a3"),
		new("item", "preindustrial_food_jellied_game", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc1e7da51cdc7f89406bf647aebd465328cf66695cd95e87f47fb30894e81399"),
		new("item", "preindustrial_food_jellied_pork", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ebcc1bbe4216c241009ff1fcb0031e6ff0e63374645bb8b017219ebba41dad8"),
		new("item", "preindustrial_food_leek_and_greens_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "54274e4e5728e70b6eac43a5fc0a6417b7ad5f10ba840fea5dd48ea66b60cbef"),
		new("item", "preindustrial_food_leek_and_onion_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f73ebc9126ccc0fff15317db95bb639630f7e410505a1c1a1442ceb7ee9e867b"),
		new("item", "preindustrial_food_leek_barley_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b51c59a24e579ac5604e2d4cc19a87abaa9efec1d2c6bf9bc1b0a3cfd09c8ddc"),
		new("item", "preindustrial_food_leek_braised_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "adef79e03c1e7f74bc82eadaeefc2966ec84aeb4e92ff58689f1fb190698c373"),
		new("item", "preindustrial_food_leek_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a30d61739758e7f13f75468e887b61a7f42ed49463a7730cec5567669a21fa7c"),
		new("item", "preindustrial_food_liver_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfbc55795a2da9b1690c1e0c2b8fcadc89e6e6f93a0de473af63f495899dba9d"),
		new("item", "preindustrial_food_maize_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d266ae101510b5ce1a42206938502be0d8f333096695616f14a3eb31aa0e5c10"),
		new("item", "preindustrial_food_maize_wrapper_rounds", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61453048e40cbe138f454feacf1111dede087aba2cc15d7b21f0e4ccaac8d5f1"),
		new("item", "preindustrial_food_malt_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cffa9f2fabe5ee7c7612a551943baa640cb0afc3412eba16f395aff8acacc587"),
		new("item", "preindustrial_food_mashed_winter_squash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f56ba5ada72ef11d3a2e25a0f8ad52e3fc07afe77d8406489210f8d4c2f3f6fc"),
		new("item", "preindustrial_food_measured_campaign_ration_bundle", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16b771896974bb4ab6e69b8b736fc962123d58bcc971a8a65741dfda3c3218f6"),
		new("item", "preindustrial_food_milk_braised_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a714cfe04d137525897209cd6c6ddfe58357132b454cc3c9ce751ff4351114df"),
		new("item", "preindustrial_food_mushroom_and_leek_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07da6ea735549b55bc1675a1819756292ee3691dfb9185bb9a13cc321c88434f"),
		new("item", "preindustrial_food_mushroom_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a193fa0cc4735320c35a2c9a21e4c6e127cb7a5766603a396badb65749ed901"),
		new("item", "preindustrial_food_mutton_garlic_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "86b1620d1834f1eb2bcd255e4836185df6bb4401af9537d4eaac34863349bd69"),
		new("item", "preindustrial_food_nut_braised_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "398707c5e80531c700f91d215ae04bdbb51dd6c592ff4d2e3b0a21008a57649b"),
		new("item", "preindustrial_food_nut_bread_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aecd244741be73899dcc997ddac2e06ada1e149f7fd79e85af57703e2cab0385"),
		new("item", "preindustrial_food_nut_fresh_curds", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "531647730f54e177178310190e82295457a174cd5f90b5ac199f3338e3cf141d"),
		new("item", "preindustrial_food_nut_milk_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "983ef209ddbcb3bdcb0e009af34e66491f27cac793009550e40ccab4753d6b0e"),
		new("item", "preindustrial_food_okra_and_lentil_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67b8b468d0f8869bd487029b5f3d664d3dfd9d2a7e2d78ffea3ba10e4bc58f1c"),
		new("item", "preindustrial_food_onion_greens_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db1f638baaf82869365f4e28560c41388f439af274600535b5e0b7a947e633f1"),
		new("item", "preindustrial_food_pea_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4482fd744a4cbb02b81bb4e9ae1b8b95b64a86cb44256c92efd9e200a33a019"),
		new("item", "preindustrial_food_pea_meat_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3211e48433fc0dc04e05efcdfd61070c59858dca52706b019e3711b69774db4"),
		new("item", "preindustrial_food_pear_batter_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "228c68ef10c4a5b0075e68d80ae0cae0ca32eaba71d675a5da2586159e8a2410"),
		new("item", "preindustrial_food_pear_rye_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "676ad752b0919fbb6dfb6b9003216b614614fc41abc5d3d131a339bf045ce2f3"),
		new("item", "preindustrial_food_penny_wheat_roll", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad195b069890deff508c84f52b18c674a2cdf0e32e1f6fc1101349298358b1a0"),
		new("item", "preindustrial_food_pepper_barley_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "64165e344d208d6e5c429ddbee2b53a4b7002f426a73f4073728c93d961cf09a"),
		new("item", "preindustrial_food_pepper_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e228fe640c0f50c43be0cbeb248b6128b3839d1878fbd4c3f8484e6c266209b"),
		new("item", "preindustrial_food_plain_boiled_egg", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eef0c0ddedfd7ed40dfdd80399d09d812ab8ffabda9ff8ebcc7f1a94a286f9f9"),
		new("item", "preindustrial_food_plain_boiled_mutton", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36ba7b6b527cf7d7e9d9245c53e6956aaf28ced4ed9121e00795343ac5444bf6"),
		new("item", "preindustrial_food_plain_wheat_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0bbd96c17762198e64079450169b65111b6c0cea1a02b7067f4def78659616f"),
		new("item", "preindustrial_food_plum_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fb2e75ec079e4e2a80ae439c7f2aa5b2265d2253b6a833b8da0175e96fce535"),
	];
}
