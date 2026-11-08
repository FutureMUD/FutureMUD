// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_domestic_wide_beech_dish_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "076344f4fba722379bb6d438c36cf6f3b8b3328f05b6db40f29269195b08377b"),
		new("item", "medieval_domestic_wide_pantry_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c6b4a6783c4c9903a4da4cb3879949d04b7bce75181d3ddc1b13b5fa0befb97"),
		new("item", "medieval_domestic_wide_pine_store_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d320b1407abaa5a7965676ab94e09fc1058c711d35354549207b6b7b09ea4b4"),
		new("item", "medieval_door_northern_north_sea_northern_longhouse_hall_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "556acaac9727ec4247572b125c5ecb3917b2383faa067c4aaa5842edfab14964"),
		new("item", "medieval_door_shared_leather_strip_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "54afe914ed3e5cacdd20b8500043c3788ef51faa17ed0ddf567b1c8286dd6efc"),
		new("item", "medieval_door_shared_lockable_inn_chamber_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f9d9435827dc2cbe526b61306a3b888d9f102a9dfbc7f844aec2f9b2570d068"),
		new("item", "medieval_door_shared_lockable_pantry_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b7154f56e3dec79c6edaa11c613cdc3cc5f639fce39cf380fc9670921723bcf"),
		new("item", "medieval_door_shared_lockable_portcullis_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "25fd68cdded046538d943d9eb1bd6fda4d57433b81a20555f044261e9413dd99"),
		new("item", "medieval_door_shared_mill_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d5d81dfe73b8deb72b8edebfc2d6ff5125539d53f48e4bc2d8ea2afc31b9e1d"),
		new("item", "medieval_door_shared_postern_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8290acd6a5d951cce647067d17b1798c1f1bf6c32c75f33192ee76f7b06e7c3"),
		new("item", "medieval_door_shared_wrought_iron_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6dab73993274436c4cff15df58784ae626b8d996680849d527f70df64c4dddc7"),
		new("item", "medieval_door_steppe_and_caravan_steppe_iron_barrier_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bad467769b86c3da5590484d95c5aabf980d308bc03e5c98490678fe23b2fb1b"),
		new("item", "medieval_door_western_european_western_lockable_manor_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "093efb2b30849e1d73acb7d48d4de090369ff7e584e6ba33201c6d2ce5138abf"),
		new("item", "medieval_drab_dyers_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "400920cf08b12f1edac51baa627d05416bdc99e5d80584a20366f40a8825a110"),
		new("item", "medieval_drug_alum_styptic_powder_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb479320056e2b6612f6a2b0ecb277f47f5dd55466ec31d24211f4dde8732a16"),
		new("item", "medieval_drug_bronchial_fumigation_censer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba18f8620f998f9dbb2d7429e70a4d47ec5aedd8f279339a9bfc78208be4a6d3"),
		new("item", "medieval_drug_garlic_salve_herb_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "04c21b8a588cf5e67455383c221a37878b4da49d89daeaa2a660a1d87f1c243b"),
		new("item", "medieval_drug_mandrake_root_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d6eb4364b0a5ae9fcd2e653b59424810bb7ddba255d01bea651fbe25965a92b"),
		new("item", "medieval_drug_theriac_spice_electuary_stock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c3a46e6cc741b6fc14c1baabf2f29880f5b0e8b28ce345b4b54611cf7c30bb1"),
		new("item", "medieval_eastern_black_riassa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "898ac985dee7f8a435123d342d83651adacb51449e37f5292dd9e3497fa6de2b"),
		new("item", "medieval_eastern_epitrachelion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "41d1f715a97a9f8d30f1a25c6ead129903c25c3c3522df5e5bdcb1c46000ec33"),
		new("item", "medieval_fine_steppe_caftan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "26a35a3b8a4a76d84f3d2d087bb8eb5ba0920826903c4c31d457ec59c7acda86"),
		new("item", "medieval_fine_woven_girdle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4d8a1f582c46c161f3e1db45a29cb7b2c872afdb650921e2585b6d11a574f3a"),
		new("item", "medieval_food_almond_milk_mortrews", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f29da7b2a1214a72c19f41fdba592e2903121b775b8e23a06c8dff53c595617"),
		new("item", "medieval_food_buddhist_mushroom_noodle_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1e91f12da249cf5abee2b4563bfb17bd5ace82bbddb4af93eff4daf09cf13c9"),
		new("item", "medieval_food_buddhist_vegetable_baozi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e02f3fd20d7db82cdf6a7d48a9b200d77f4d625a64c038e3f1954a72e7d554de"),
		new("item", "medieval_food_byzantine_court_stuffed_fowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0bc23919f9827e8e790e2bade845977957ef22b1f1389da826b26ec31058d27b"),
		new("item", "medieval_food_chicken_harisa_feast_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "271b26d0c036dd072d8f1bbc5a5136fea920f3afd28790c83efa97066ba57d02"),
		new("item", "medieval_food_curd_rice_temple_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9a6671f3e07a87669d759c45567d0848f1066028816a34d0d33a6245be5d211"),
		new("item", "medieval_food_dried_fish_ration", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "98f5d90e33603579dcbe964f2b717fca5d53fa6d0d18793e869c3715a98c0810"),
		new("item", "medieval_food_ethiopian_christian_fast_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "640e0ed103ce2ee6d38b8661237bdb55d1d72d7f929f95439bcf9f17663e09af"),
		new("item", "medieval_food_ethiopian_honey_bread_offering", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "43904cd8733be4f1c966fdf26841c9a6e809c294996a21587868bb858e0ef06d"),
		new("item", "medieval_food_flatbread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "13d09755f52844e4c439417cdae0732076226314891fd200ba4f8a419f5edda2"),
		new("item", "medieval_food_herb_cheese_tart_de_bry", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "212e2c0bd3c9635cf0bedeef0225cf1e54051c1a2954b58c94e5515c33a037ab"),
		new("item", "medieval_food_jaggery_like_millet_payasa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "48b20bc3731c81ca0a456a2e96cdb624545bfff6723c9bc74bc5177f6b2ab159"),
		new("item", "medieval_food_japanese_shojin_mock_eel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d0bb25a7b6a30c456d246e6aece87c14101030bd8e86b87ca4f267ea791192b"),
		new("item", "medieval_food_japanese_zen_sesame_tofu", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "57180cb56bd96f914355d1a1f76347aebeefbdeb83264fd1a40d7b216c4fe669"),
		new("item", "medieval_food_malt_sugar_animal_figure", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d63ef88ac6a884cfa88e7487f6190c4c13df6254a36722fa5779a66d2936f760"),
		new("item", "medieval_food_medieval_pulse_flour_noodle_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "724b9e5afda10bbecb228f59d000235c8e0d8ca0908b56e5e0304d07478b2831"),
		new("item", "medieval_food_qatayif_cheese_pockets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3a6aeee61a2c4f50a51deb0425dfb6c0675203860250929069e8f5597957420"),
	];
}
