// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("commodity-spoilage", "Salted meat commodity spoilage", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "708f35df5821ad494c4e95f1761baaf306b6f1ac1552e63a8264dbd31f62c582"),
		new("component", "PreparedFood_Antiquity_Condiment", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "44ad971ed5e9dd117266bd8c3be53877728a989711059f7364e8acf97dfa63ef"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_brazilian_sugar_egg_convent_sweet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3401fbb2594756d3e7edf636bbca31dead758c7e06aad956b86a7933e8fcb847"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_cape_malay_spiced_meat_pie", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fdf5a952a3b2a696a7e5eaedc5a5ae683b5c71ba8991a15b1b4b0cbb47c4628"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_meringue_kisses", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ce0296f8f6c849c8bb2593d3f4b946039b9571967fd4aff1f5b02ab6af04b7a"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_pistachio_like_ice_cream", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "53dfcb43ed2781080bdde825e70945807a0ecc24f41a69ab335bd953714dd94e"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_kongo_cassava_fufu", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c15d1a59cc67b161a7bc3d3a9a510ee5c1562a6a3bad468ae0810155ff58fed2"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_tea_sesame_biscuit", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "694442d9c0de7053fa0c30439accb177ce67d53dae23832532aadb27acee70cf"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_sakura_like_mochi", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ed0d93e3202bfe50d68d4928166a802584b20124aa5ea5c4a7a1d341a2ef7ce"),
		new("component", "PreparedFood_Catalogue_medieval_food_banana_judhab_court_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "da0e041122bbfcbe1f101f9e59557664ca7e98a57a855b0541a3f949b3bbcd5a"),
		new("component", "PreparedFood_Catalogue_medieval_food_curd_rice_temple_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6d244d10bb80b8d390a728ae9c476f63dec69fd89d7cc2a1e2d7f286e9f63a6"),
		new("component", "PreparedFood_Catalogue_medieval_food_dragon_boat_zongzi_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4e45336fb58a9dc691198c669453ba79844a521999b7b6d468f66492bbf939c"),
		new("component", "PreparedFood_Catalogue_medieval_food_fasting_trencher_fish_sops", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "db47a48638cba5e16683c3bd271540aa5398e4b6e03540c42a7b17273a79a8a5"),
		new("component", "PreparedFood_Catalogue_medieval_food_funeral_frumenty_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0cc8d4a23a3f0e2c81d16f53ef373b99ad538a175e660afd7dc676161115047"),
		new("component", "PreparedFood_Catalogue_medieval_food_honeyed_ginger_bread_mass", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b221cacc4bac8d7952a84bf8cbfe95bfbf5c33013d1f60f8147f13a1d3eb3cd"),
		new("component", "PreparedFood_Catalogue_medieval_food_jaggery_like_millet_payasa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "faaf74640d889c066dd23a4aa18f1c27afbe5d255906f0fa4f3657d8bff1d9d9"),
		new("component", "PreparedFood_Catalogue_medieval_food_japanese_shojin_mock_eel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7646b88cff2f533b8121a1138af144b406059b285e01af6479d3fba42c042014"),
		new("component", "PreparedFood_Catalogue_medieval_food_ma_muniyya_sweet_meat_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a81bd4f3f51e25ce7e768a9bd2b0ad7442987f8b053883681cc8ddf739fed83b"),
		new("component", "PreparedFood_Catalogue_medieval_food_mande_smoked_fish_grain_balls", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7cf8dbbb87461b394c9de4a9074c536c5eb6e2e875ff842de8e489085c5d294"),
		new("component", "PreparedFood_Catalogue_medieval_food_medieval_pulse_flour_noodle_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "73c8409b7589da8dbe485e8ecd8d772b44b2983891208de493baae88978f823c"),
		new("component", "PreparedFood_Catalogue_medieval_food_qatayif_cheese_pockets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca283d97e5a5e9f30569e2e07299197dda121cb8ca5e1ba6ed6156c797413266"),
		new("component", "PreparedFood_Catalogue_medieval_food_song_clear_wonton_soup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9cc2f5dba092904a72e5c77ab150193a7bdce4b88befe59edc23cf3d60c0b0a"),
		new("component", "PreparedFood_Catalogue_medieval_food_sour_tamarind_rice_offering", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "de287966c75def044c14a1bed98b0bcd16ab8e2b411c4022da402c1a26222e3a"),
		new("component", "PreparedFood_Catalogue_medieval_food_sultanate_almond_chicken_rice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e5b98373ba9751f66d3960518a3e2377e3d6e52991c50b249258b70262d98fc"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_ale_braised_beef", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ded16b4d749d7e90ca11e28bee3bdc1fd4138d1cec5f4c410a35d8828cc61f55"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_and_mushroom_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "801ff846fc38e3bc04a0c88aeac3eb9cd7fcaed7fade56f5ad8e248c54b816fa"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_root_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4933868456da21b7782f1ea997c03df1d014b3806b7c507f99a45c90728f9cbb"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_leek_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "15bd98653d52c3a1e19aeafb93773de0d8f7995ee41d518915b3b99791d1efcf"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bitter_fisher_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1d496eabc8a19955bf2e77a6f00c7031677260247db237eca17c1003d8782d1"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_black_bean_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87bd9602a95e3ca4104cb49de0724e53e7f94362c6485c9a44f5bb7322bf7ee2"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_blackened_crust_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "64709008d3b3ff7eebb29f4d2f711d8f756f9899a6df9d1e4ca22a46168a53e2"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_boar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0be6d64bc73d780098cbb4377a6f4749277d512eac6526ff85c633066ec3b3ce"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_onions_with_herbs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4b8d3c4c2825cd89b12677ffc40bc913223eab734294f3a05742cdc6ee81ba5"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_tongue", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7caf2ddef9756dd3cba2a49d340e5de98c08bb33e8da3561061a14a440b1688b"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_whelk_bowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f6980fdf22a5194a2dea1bfd706102a2b846fc3f8a607a279a89aa250997dbd"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_brain_fritters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "45d71df1190eb3a9c5a93d1df7719a69428d9f30c5e5f115af8d5bcdbf581631"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_branded_bread_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a67236969c10965ab36b9de747f66b99d0370fb776577245debe7d49af21f1c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_broad_cut_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7621eb9f42e37965f38457c9f49765d217b76e877c678088ee055fdb51919072"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_broth_sopped_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ddc90cc0cbf0312d4b91924f86a2d95232de128d3e89b8646c571f59b6999a8"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cabbage_braised_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "33674786a2a4d6085699eb82a2f971f15b47e2b9932a66c5ebb955a242689eea"),
	];
}
