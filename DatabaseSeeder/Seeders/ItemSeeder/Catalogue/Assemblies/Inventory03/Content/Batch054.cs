// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch054() =>
	[
		new("item", "preindustrial_food_garlic_simmered_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39903b7c2378cb81bf509186320100627b2e1c8868f28167d36eff3628fec0ad"),
		new("item", "preindustrial_food_grain_meat_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76ffebd7f7a0c41aec938b5913ee5bfd624bcdc823ec256c3d088fbd00e0147a"),
		new("item", "preindustrial_food_greens_millet_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "713f7714f71bc244082896be129e4afdaa81efc313e5b4a14669edb1664f4c7f"),
		new("item", "preindustrial_food_grilled_meat_rib", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b771ca6ccaecda13581e5c904e2e4a6888451060863b747bb99116cac8b9d754"),
		new("item", "preindustrial_food_guava_sugar_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ac2b242cafe28d42e3221c80d185914b60306891d563868e93d7cacf7749aad"),
		new("item", "preindustrial_food_hazelnut_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "90e6b1fed28b7a6b5acf7cb0c4b119ba2eaebabbc9f168c24f1b3af48bb16b96"),
		new("item", "preindustrial_food_hearth_roasted_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8377300bb988af7e45e8f5f850d091024ac21382b3380b553b8ea420550038cc"),
		new("item", "preindustrial_food_herb_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5086c1d4780d3732c069dd0a750b04dea72c984fcf65507fe097bab210f311e"),
		new("item", "preindustrial_food_herb_wrapped_dry_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7136f1ae52002df111e79f81df0cc050b6a5c191a1feddfa70de916419732a6"),
		new("item", "preindustrial_food_honey_fried_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f127e9278b9bd5eaa321579e677444a4844e69987fb6d6d9ef423fb2368e4a0"),
		new("item", "preindustrial_food_honey_glazed_dried_fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5412f0f7f1f5994585f4ecfa2620f57ed9f8850695798f1c8e1bb55ccbccc068"),
		new("item", "preindustrial_food_honey_poached_dried_pears", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb0590cb991fd10ab2de63f9334e6d7cdd15174ac56752e66687d4d3ad8dddc6"),
		new("item", "preindustrial_food_hunters_mixed_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aff7431e1f14ee12c8ae6041b17709f0eb5049e6e5b955eee1d61871d8049e66"),
		new("item", "preindustrial_food_intestine_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0f0fb100cbb3e4156ccbd28e26f6c7f8ef4d47fe6ffaa4d28260d452165562b"),
		new("item", "preindustrial_food_leaf_fired_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "47409dbf1635901de7cd462e54a438662b74dbd10a79eeab5e7cb1faf1201de6"),
		new("item", "preindustrial_food_leek_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "83cec6170d4718eefe375602d7d1b2844a66789a1636c2d92f213a9d4a5ed56b"),
		new("item", "preindustrial_food_lentils_with_roasted_roots", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b54a84237660faf7b17174ff0ab1fb0425659b50011ab1c1b9cc5725b7183f5e"),
		new("item", "preindustrial_food_meat_conserve", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb1c28ca88332bb33e941fb5827eaa2af19721a1dca5a7fdbb1f302e1025a7b3"),
		new("item", "preindustrial_food_meat_filled_maize_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f650a3d948a18f9828c45621fd6fff7c11b2bf52198a78d72294169671542c6e"),
		new("item", "preindustrial_food_milk_poached_white_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca640b89bee418314b7a0e9d575dc81e0d4b29b5d808e6b9c93e7b6be71d1acf"),
		new("item", "preindustrial_food_mixed_grain_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8b7d085a55f63b9dbc4cb64fda11b4aaa1a955c23f1838b6e6f0058b2afa142"),
		new("item", "preindustrial_food_mushroom_and_onion_hash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "20c46804528380e5fe308e5133a57319b234be7b3516a9dd17b521dc56e48b19"),
		new("item", "preindustrial_food_mushroom_broth_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "51458e71554417ff5640b38e41df4c99e79505b04523f1d0f1f45a36ad5036d2"),
		new("item", "preindustrial_food_mushroom_filled_crepe", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "449bb5e15c422fb35bc7c81d3f70a32d30acb151c2628e6909f4d2b47891113a"),
		new("item", "preindustrial_food_mushroom_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f7cce342b3d3aca8fce2b66813b219beb48d6d9b2daa693c1c224ef2138b30c"),
		new("item", "preindustrial_food_mussel_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff126ae37dcb4cc2a83294ff00baa652ed63c78b1112445e7a72b18bc498cac6"),
		new("item", "preindustrial_food_mutton_cabbage_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40fad7adcb8d9cd0fe589ab2f5710ee453ee12c4e5f71321ad7a52953ff84be1"),
		new("item", "preindustrial_food_noodle_dough_coil", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4dc58dcf74ff4d02c31ca7fb3dea74a95e9b11d338adc1156d462cbd256b7b6e"),
		new("item", "preindustrial_food_nut_honey_spread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9b99711bfcfeaaf1044c5537a5a00f400cf5a3bf9502f050637f5b3020e7ba9"),
		new("item", "preindustrial_food_nut_stuffed_figs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af47af5a241b49e9540151b34342a7958cbc8f490b1336c0b31218daf5fb8cc2"),
		new("item", "preindustrial_food_nut_stuffed_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b7ec57e465c170c173730885d23609bef3e4ada93d9fee41c1d2f27da9434fc"),
		new("item", "preindustrial_food_nut_stuffed_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f92842eff358362d2a9b3278865ab5071b9693c69615d845ae1d8d0f697dc68c"),
		new("item", "preindustrial_food_onion_bed_roast", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "543e5ae38e6c9ca21a11e7c508519bffeceac1d8bba018b4518f346455fd8d8e"),
		new("item", "preindustrial_food_onion_drop_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "51c481887338fe37ca71a61506d75ffaf73cda7fdc1f7e6b6cc8884ded83a7e7"),
		new("item", "preindustrial_food_onion_stewed_broad_beans", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0523b892488b6537a674f75ebb300faa30e742c01ffe034cab4f65fef9a84439"),
		new("item", "preindustrial_food_overnight_sour_rye", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "572b3b24d0e864962ba8951f1f2f0781cdebc93445c704c1bff9fd893ce45b07"),
		new("item", "preindustrial_food_parsnip_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5935bd2f9b1e147db0fce88db311f18edf584495fba699e818ea375a3363776"),
		new("item", "preindustrial_food_pea_and_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca185959817ba7552aa32b3f7fa09dcc2344f5ce690548fdb35fe44ff3d4d748"),
		new("item", "preindustrial_food_peppered_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "90cfdbee9f51f7d94e1169e0aa08f8b7ecccad44b3153726b60023e2a641135d"),
		new("item", "preindustrial_food_pickled_onion_crock", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66c4bfe58ae44dd87eba401f0c213416dfb9b80953d0067ce1605c4ee086516d"),
	];
}
