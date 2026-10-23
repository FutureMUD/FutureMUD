// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch047() =>
	[
		new("item", "medieval_domestic_large_pine_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "54ea69d5d462271a0242b4adcb6f530f31ffdbdff9d4c9d71bc1d7184770d14a"),
		new("item", "medieval_domestic_linen_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9f22f53627db2517f3e5c16ae6f89858a94882e97915f20f81d5f0bf69e7f6f"),
		new("item", "medieval_domestic_medicine_nightstand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a82ebb4276c24695733eb32842350f0c87395ea30a01a03058747e1d2742efb"),
		new("item", "medieval_domestic_medicine_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8090e0d720442bba78063194458ce25ef998d784a98c19cf71b1164c5a7ee9f"),
		new("item", "medieval_domestic_narrow_pine_pantry_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "08030d3752be023ea324e5ed4967a00018cf52a9c2962977d454c8bd07dfac36"),
		new("item", "medieval_domestic_painted_keepsake_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fff41ad3c60fdb41e6069a689592bb903ded8db73ec6cbbdf0e2dfe8cde06fcf"),
		new("item", "medieval_domestic_painted_leather_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a124b6ae7f4638ef1a13ba40c77d42d4763f5d37221070768f0b907e26bd68b8"),
		new("item", "medieval_domestic_plain_small_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b97d50532e4a7ef6aeddce1e076195743e80829501d9ffd78377486fafcdd181"),
		new("item", "medieval_domestic_portable_relic_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4ef6c4534845a210792e7ee794a1c4a33243afd600b72244a02ab645f037c25"),
		new("item", "medieval_domestic_repair_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d067fe337948992ad87c74f50b25c5b700c0aec3b2e913e91bff7cdd0ed93494"),
		new("item", "medieval_domestic_sewing_drawer_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8497e0069626ca913abaf3577c4ca03152f20bd81b14320edaba7980de9373c2"),
		new("item", "medieval_domestic_sideboard_cupboards", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1cc4b9bef4c48ce79fb1a1e98cf120d5f33c14dd3902efe20dd9777c3acc566f"),
		new("item", "medieval_domestic_small_spool_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "17995e97509fce0cd52e1f7db0b2206455df53bcecc093f574b056876f4fedc6"),
		new("item", "medieval_domestic_spinning_wool_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcf7ad1eca93deee0e8077d041988f34ff66975a90efc3880cd160bb23891f26"),
		new("item", "medieval_domestic_writing_desk_drawers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ed7992e7665e69ef571ffd166807edfe3096dde0bca49641781566e799cba37"),
		new("item", "medieval_door_east_asian_paper_screen_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb653d2712b25ba3f96db0570279bc6f85030c826c38e2df83d78fd03350a0b7"),
		new("item", "medieval_door_islamicate_urban_islamicate_storehouse_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f27192965cb312d9d1e088d903b2fbabf0b28446352d02085f7b1d6ab93f769c"),
		new("item", "medieval_door_shared_granary_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "128e9195914ab211007a18226e4eda055fc1c4662d658104fb2cfef7da4428be"),
		new("item", "medieval_door_shared_lockable_plank_household_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fff02bb56165590bb07e56f3dc1ea4eb941c3db4698d6be3e0a00a00c21e629"),
		new("item", "medieval_door_shared_lockable_shop_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5b9402649420c77ff3bb31527d3b16d1e539877c08c4b0745f40847d759cf65"),
		new("item", "medieval_drug_horn_medicine_vial", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b072f74652209ef5f2726f90e46ede0e5468d19adb375918c4b3f6dd38f1bd95"),
		new("item", "medieval_drug_mint_ginger_decoction_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b8f6e331821b1b42b6a98bda3276c060c281b51b423c3fb5577fbcb48654f07"),
		new("item", "medieval_drug_mint_ginger_pastilles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "35d5c4f83a442dc3b560c49b050e40eaa86c2263a42649dfeb39f0a41354551f"),
		new("item", "medieval_eastern_christian_candle_sand_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "759f46553820592176fbf749e4cb3ec5e959d454eec61a86179d3f84fab8f360"),
		new("item", "medieval_fine_linen_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a5b116fd98098e33c10b62f19ff8b524a414cb88e9013f417dcd1f9cb7abe0b"),
		new("item", "medieval_fine_silk_sari", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed6591b220ca99766ff731e2b16a3b87c015a4d696b106d5ebce1ea1374bfe99"),
		new("item", "medieval_fine_wool_hangerok", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ac3f3ea379d5c4f10b28e13e36dc58071cda3202b3b7d01f25a99bf01690233"),
		new("item", "medieval_food_byzantine_spiced_meat_paste", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a25d92229ef09cb153b5c58940fd46748139bfbbdfd2dce3de880119b777a60"),
		new("item", "medieval_food_funeral_frumenty_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1da54cc4c08db16811780355fc8ba86cac810767d3c1c8b47f946720d6d74f4f"),
		new("item", "medieval_food_honeyed_ginger_bread_mass", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d70cf6d701575d465b5e0f4bb35dcc399468978648c115657a3f02099678df98"),
		new("item", "medieval_food_hospital_pilgrim_stew", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "548e342c33a9d1efff1a4ce76ce6d964bf86c679ab636b1e3a8c5916683293eb"),
		new("item", "medieval_food_itriyya_almond_fast_noodles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2dd77e8a0b73b632574705d5ff56c2bf88b7396a2c3de4f318e62fbcdbc369e7"),
		new("item", "medieval_food_mande_smoked_fish_grain_balls", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c96689d9e771a2fd9f9f7d3a12a916e791087902b61524a66035832a8a785ce"),
		new("item", "medieval_food_moon_cake_unbaked_rounds", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb91a3698325d50a1a6336ea45e465c201cea62302cb607457162317d9a5972d"),
		new("item", "medieval_food_pottage_filled_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6684a5f52baff346d7de2ec01188431b4167e4819e6dec4e91be176a1922c4a"),
		new("item", "medieval_food_sultanate_almond_chicken_rice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f680e58bcae5ddedb7ec14bd35697548b0f74fea9dfd7bf6ca15f2028136b173"),
		new("item", "medieval_food_sweet_spiced_egg_tart", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f8b881e219c163fdeab9b3956c0ad6b323a695563135d9ecbb69226b3c6abfa"),
		new("item", "medieval_food_temple_sesame_sweet_ball", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb133dd3de188c3efb27816597b7b9ae6dc1303bf80aacbf99bb9ce3ff5fb1fa"),
		new("item", "medieval_food_white_fasting_blaunche", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d3df54608d626ab323f0f6d1f698c7cf56136579750358439dc124839f657f3"),
		new("item", "medieval_furniture_carved_cradle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "00e81a34b8143887c9193f206036e0ea14468d7f04974be912856e3be874aadc"),
	];
}
