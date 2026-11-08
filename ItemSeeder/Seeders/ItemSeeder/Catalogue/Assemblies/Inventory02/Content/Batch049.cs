// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch049() =>
	[
		new("item", "medieval_domestic_household_accounts_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5aee5df1c6ed24cabb6b70eb62622b29baee63a93305604f524b8980e99dd6d"),
		new("item", "medieval_domestic_leather_belt_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "00bf46831ca99298c68f5e5f92883596196749ed32d65474a11cf5401fcfe8dc"),
		new("item", "medieval_domestic_medicine_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6513c7e253523a063abd3e4a1c214bb28436201b64d7be79a3f38fa106c9a21a"),
		new("item", "medieval_domestic_oiled_document_wallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3810296fa772571a8bbd1ea9c3172c7ae280d6a7ae1a463fc50df0f89f851bfa"),
		new("item", "medieval_domestic_plain_oak_wardrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c33ecd352bd3f68b2fc9e636a63a76e4a24a691d7b21f850beea69918b87a26"),
		new("item", "medieval_domestic_spice_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3a68ba40e1ce5ae99356d5b8b06d591f700a16b52b13dc4117ffe69b08bdf53"),
		new("item", "medieval_domestic_strap_hung_bottle_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "76344cd139632fabf7800f507d41ac8dac6e9d1e0c5ef71c9dbe4f02ded55f0f"),
		new("item", "medieval_domestic_walnut_robe_wardrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0383b7c5f6e4851ec46a72020b7d3bc34f9ac282dc2bd7c989ca51334b0be2f1"),
		new("item", "medieval_domestic_winter_bedding_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4118209ebcf6d2cae4a828988fc73358aaa72e50be74aa66feaa99bc303bb364"),
		new("item", "medieval_domestic_woven_willow_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a2d55c2c16dd61c0151e0f302d7816266f16e2ee8346be320c345b56f7bcc8c"),
		new("item", "medieval_door_shared_cellar_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "54eca7d0d5a785db8c870ce82d93e28d5247f4f49dc95dcf6a3fd122d11fe494"),
		new("item", "medieval_door_shared_lockable_postern_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "126733f2e7f84853413b1765593c917ffb6f46cfc574291a980045379f2aacc6"),
		new("item", "medieval_door_shared_plank_household_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "58f37fe251578cbdbc9ebce162b166d1b879c8a11bfceebdecbd47f3fe80fceb"),
		new("item", "medieval_door_shared_simple_cottage_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab61b8caa1e2be065b28b5f4833edacf3f36c6c3499ba0b87cdf2723d3ef2865"),
		new("item", "medieval_door_steppe_and_caravan_steppe_lockable_caravanserai_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "08ca07853351f31ccdd2f5af89809e12cdb114ba299db13768a5abfb02e838dc"),
		new("item", "medieval_door_western_european_western_manor_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "020054c47fc8dceb0b505582d8093519d70d4d37f656f66abb7badfb07e0e63e"),
		new("item", "medieval_drug_aloe_burn_salve_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e46d628774723c225d94ff9af7683a36988e4f42e32b76a2702e0dceb682a4fd"),
		new("item", "medieval_drug_garlic_honey_poultice_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "01245e7aa697405af30ed7cc7ce2369c2fcab057e5b52b2972450c8392576680"),
		new("item", "medieval_felt_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "44c83a2030b269a7bb9705785976e810c2288cd90c110c72cd7cbe063732aa76"),
		new("item", "medieval_fine_bordered_brat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ee5dad0271ec1423d7d2f45e8aef1134c2b646d64f92c5b9131b3ba69df22a0"),
		new("item", "medieval_fine_bordered_leine", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "53940dadaf8664486223eb7c82ff5b35d919478e4f5f2e57b118c80cca9b24e4"),
		new("item", "medieval_food_almond_lamb_harisa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d357c58bd25a4837c85ae6be251523cd48ada8a50a50383af435f8ac8078a29"),
		new("item", "medieval_food_almond_rice_muhallabiya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2fc96c0623e918d9e050bb2cb1af490be5ae41f4f4e0648f679b2f7d4eae3c4"),
		new("item", "medieval_food_almond_thickened_fasting_sauce_fish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "df8c6f6b482b4f272745337c3fcc48a456a095e6aaf6236ef8e1e64e54da3471"),
		new("item", "medieval_food_apricot_judhab_pudding", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4750d0cdc6d2ad1cd9b1bc8b4d49b2ba262850c339d3a9311c4e7d7146cf4298"),
		new("item", "medieval_food_baozi_uncooked_pleated_buns", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "97440759d3fdc3deddd80038ec7828ae82bfac83304ccf2c650f681f60c7f75d"),
		new("item", "medieval_food_chaucerian_market_hot_pie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f459fd169db3d3aea6632893c58696c026836873da0be927ca5952ba65c06e7f"),
		new("item", "medieval_food_ethiopian_monastic_pulse_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b3928b4ccc4a6f045da9af54145835671b8eef1068e24d7f16baeeb721e2f58"),
		new("item", "medieval_food_fermented_steamed_idli_cakes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c21adf6dbeb162670b23a1ca203e38fdcd2dc7f1ad8e7bbf17fcd3238d7b72bb"),
		new("item", "medieval_food_filled_unbaked_meat_coffin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a242d2d1f85f16d0e3f9ad4e952507e0cef843aee2a17f4762e49dc2ad2052ff"),
		new("item", "medieval_food_green_herb_tansy", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0c8fcd3473c1983b13e1e5b58f70980c9f386a1876088e2e6a75c4a7dc4e1d8"),
		new("item", "medieval_food_infirmary_almond_rice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0033c42c340d57081757e4288c12f5b86751648e5ae3f261a83106bb056a975d"),
		new("item", "medieval_food_japanese_bean_yokan_block", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2bd7f2efd3393f2b0f57156d91e93b66a88b85cd4151b6ee5ea0f4b5acb24e7"),
		new("item", "medieval_food_meat_rice_muhallabiya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "05b54cb737dfc608c69c05232f5ce51f27d7d09f29db5ad6f492d87735cded8b"),
		new("item", "medieval_food_persianate_herb_kuku", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "87d96989f1f76aaf976577bbe2cde123e337968a780ad57ed7c8f924b4825b70"),
		new("item", "medieval_food_royal_heraldic_subtlety", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "59622317869cb5ce12e8bde31d174001e89bf353972895d7f826e8d376fa506d"),
		new("item", "medieval_food_sainted_figure_marchpane", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "31d9ac012675d3d5b537aba888672333e9b90a41e424213a9ed8fc053488f6df"),
		new("item", "medieval_food_samurai_miso_rice_ball", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1aa096cc5f18eaf0b952e0873d7b88cabf7346eee4b8b729f76ab21cff278b02"),
		new("item", "medieval_food_west_african_leaf_wrapped_grain_pudding", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "661a667beaaeedc46faa55d5b0ab873c2553039ba54756d4aa0e80bf7ff26c79"),
		new("item", "medieval_furniture_bakers_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6ecfe6194ea5c5068bac5c104b2de50950a9a7088830e45f1d53a9b59339ffc"),
	];
}
