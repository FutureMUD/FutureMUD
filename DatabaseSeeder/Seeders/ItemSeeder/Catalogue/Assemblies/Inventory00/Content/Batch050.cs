// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_military_nasal_helm", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "63d2da123df683cc10b794051978c95a5cb5bf1d2432c58bdf81e249fd2c9c0d"),
		new("item", "medieval_military_plain_axe_headed_polearm", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb460ab3a04180235aa41b34746a5985104961178a202b33fb9129c52c0ee71d"),
		new("item", "medieval_military_plain_war_bridle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae767e4497433cf5fc03db898c3f79edd1d651ac4ef84095d49fa0643a05b01d"),
		new("item", "medieval_military_reed_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5f826c82ec0705f048b070e0f3ea149be05618abbf27e9dde8fd6fd00194087"),
		new("item", "medieval_military_reinforced_large_weapon_sheath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "909de5d3ef5931de49df6ccebb2662ea726cc8b1cb6c7a2fec7b79cef7cfd8b0"),
		new("item", "medieval_military_round_shield_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c550bb78cbf630b46624bc6a616c2a9b53599923e3dfa1d5540f0ce1cb7cbaa7"),
		new("item", "medieval_military_simple_armour_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f2a062b02eecbf9fe8fa416995e0e518184776afc8d7abc5d9c4ff0c68f5702"),
		new("item", "medieval_military_spare_crossbow_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c5172db3a1ee8faa1cd5036c0ab0cbf3c43444ae2c33ac1b10bee16c3b22f73"),
		new("item", "medieval_military_training_military_pick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3d2279cf1f2530bb6e28ee5dcd9f308f1219283c84faf25bb718fbaea4c333c"),
		new("item", "medieval_military_training_spear", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b78ec1a0570a66caf47bccdef0c8b2e4a9ec32d496e8cfaa943ddbd2b6d8115"),
		new("item", "medieval_military_wall_spear_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "79acc33b7308ff63b686dc8c22731fb454bb6e6dd8c8b94ced35e258d4a03932"),
		new("item", "medieval_military_wooden_practice_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "76d9d098f43ab9526391c9569ed7bb052a7b1bffcc02f2d5a70c72a02a302926"),
		new("item", "medieval_military_worn_kite_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e43ecfe76238a48f984a3cc443c06eb88023129fcd57a070848825d1d9f0f84b"),
		new("item", "medieval_military_worn_military_pick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a752946721c59a3328428c197fc92a5786eceea8fdcde1d98cf80dcbc356900b"),
		new("item", "medieval_neat_wool_town_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "af40c3409e62f49cca424d3947145ffe00a7e93b0f6cc76bfda52257001aa09a"),
		new("item", "medieval_religious_bronze_offering_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d9df719c00479f9c055fd095976a6bb0317f4f5faa4b51080a7e3cae1edb861"),
		new("item", "medieval_repair_simple_wood_repair_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d71ad78ec07c024362b2c410b0deac7b40aa1a0c29efe6a8a1cc7b5bfb6f9a57"),
		new("item", "medieval_rus_novgorod_pine_winter_storage_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b602f5624912a8bb03a1af4e4d562bae293ba8da5663e786646df3591d09e25"),
		new("item", "medieval_shared_bronze_keyring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7338090029464a6c3f2767ccc42aa0a334674e2df9216e509c28211c5141f9e8"),
		new("item", "medieval_shared_drop_bar_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3e45697ffc0ebd7c4a3bed63d20230b3e9b8153b64d433a6a422ad909b44f6b"),
		new("item", "medieval_shared_hook_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f5117169af5a9e88d19e9594b3e3c0ed4143d288c669fc57983a2770b7fb979"),
		new("item", "medieval_shared_strongbox_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f77e633ab460b2fd489f45288a8e67482b3acd2fa0c834c16bc32ca10675f283"),
		new("item", "medieval_shinto_priest_eboshi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7f60eb97aafe5c4a7a9152f6a76e1d0724b4ce1dca96c4a81468b3ccfa3b7e0"),
		new("item", "medieval_soft_campagi_shoes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "91b0ba2ba0be7d6b474e84d35e075e8c7a4a388d12b26fcb4a58fbd29225b74d"),
		new("item", "medieval_song_official_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d52e54e98cbe4fee06910442ad0671af5e061fbbaff7a875a3acff4d452c1c31"),
		new("item", "medieval_steppe_felt_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e973c12b27a7638365c033baefe64472f092c8249abc24d7da50123cbae3ed76"),
		new("item", "medieval_steppe_travel_coffer_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "70787f348c211cb98444e97bc3724d999c6b926424e950a05da1014b4fde577c"),
		new("item", "medieval_tableware_breakfast_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f8df0f8406643c7853f15c5e7b83b9405a7452d28268a16efb859f163c59ccc"),
		new("item", "medieval_tableware_cheese_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffe5de09bdc1a279fad228de7757b0428aff37fec84d625fdc99406af4f7d2fb"),
		new("item", "medieval_tableware_communal_mazer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d3619466e9e5b77c8b0ae9de3517855a518904bded2bf5addd54077a032e969"),
		new("item", "medieval_tableware_earthenware_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1558ec4373bb95f24681a84ae4fe8211b150d437b22980834170c36ab5b1f3f"),
		new("item", "medieval_tableware_earthenware_wash_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac925e65ba9c6275891a5ea68f06995b12c2a3e6d18d4843119b55ed4ac412c8"),
		new("item", "medieval_tableware_large_ale_stein", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bdf2f57b788f63acd3da76aa9596f99821281fc8d5cd65a35f9083ba350707a2"),
		new("item", "medieval_tableware_narrow_neck_oil_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "81d63b75ceee58369753d54ae90c525f52354f6b6ebe3b94c9f97c7059c7ce98"),
		new("item", "medieval_tableware_oil_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f952a31c393926c63fcfe12f3d38965949e16b4ee35c5ef67bcab48cd3ad4f23"),
		new("item", "medieval_tableware_relish_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a1f9f23a257833cd9b88f394255d244d1d7ff2100c21d0184d4c24c90ae11fb"),
		new("item", "medieval_tableware_rush_bottomed_service_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ab7502f61f97a7fc09ecd1d058ec81568a2482a74fa5e80d77dd45c51eb542f"),
		new("item", "medieval_tableware_shared_islamicate_urban_glazed_sherbet_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c4ca0f470f6efb70b616992da2a530f863bdd1037c3d2b971b52a8e840cd49f"),
		new("item", "medieval_tableware_shared_steppe_and_caravan_leather_kumis_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f53f6a1c59db1d0ee16850f1418739859891dc2c3997b68470ff35954e5f0eee"),
		new("item", "medieval_tableware_shared_steppe_and_caravan_small_fermenting_skin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9257387bdbb3fe6789a39c82b256d6e13114e57862c0647f56d59e8e782824b0"),
	];
}
