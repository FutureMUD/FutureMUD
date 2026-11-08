// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch030() =>
	[
		new("item", "earlymodern_door_ott_good_warded_lock_188", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c95003294d70f512ba9af55309de8d6edd6647eaaf885f598c182eadf8f8e2b"),
		new("item", "earlymodern_door_ott_master_warded_lock_189", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1013454c6811ada9c1241ee770dc12ba607539c68507138ec1d3be90f8a09e86"),
		new("item", "earlymodern_door_plh_warded_lock_149", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aca83e63f1efb6175803ea3a62118799d6db27f9b4af24afc6dc9fb7815cf307"),
		new("item", "earlymodern_door_qing_good_warded_lock_302", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc04070bf16af6f31a150b6af81bbc1c38617e939e16d4b2c091f1f541e61174"),
		new("item", "earlymodern_door_qing_grille_296", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "728ecfef9b741df54096290158f9634af2006d055345cb452ddf478c52992571"),
		new("item", "earlymodern_door_qing_master_warded_lock_303", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef828bf082526c49008e44827cf8d146b789edd51a229a33455e3375202e942b"),
		new("item", "earlymodern_door_rus_grille_163", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "63fc56b5339e6d02fcf0a5d38392bb5fdd9897c82efba0b54f366c458154459c"),
		new("item", "earlymodern_door_rus_lockable_door_155", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5097e17c63b06fb5cb73175bd54085280fc32cf1d7b98a40fb8a1f0d3cb00469"),
		new("item", "earlymodern_door_ryu_warehouse_gate_370", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6fba6a0f9890a545ebbb52615e881bcafd7fc2c3000f76873ae2f3c40f3b868"),
		new("item", "earlymodern_door_sahel_courtyard_gate_483", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9937aa8d747808d6c26bdbd1e8747a3f32cdb2006a50a0421e573a1deeca283d"),
		new("item", "earlymodern_door_sahel_panel_door_476", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e24ce8ca4bf21fd9815f5ab42029d01ea136054482b3abdcf325995934166c6d"),
		new("item", "earlymodern_door_sea_good_warded_lock_397", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "691729caa112669b0fa9ed0166badbc632e1863f644bb7b2d982c44ca2bd1916"),
		new("item", "earlymodern_door_sea_lockable_door_383", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "abfc34e7482dedf09bff9c81676627d1658ab343a629c8d85aece169af43a2ed"),
		new("item", "earlymodern_door_sic_garden_gate_276", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "79be64ae4b744adf2222661b3b777b62ebee691c09f7f726c291ced8cbee47ca"),
		new("item", "earlymodern_door_wa_keyring_451", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0d8d65886d5c21eb445dbf744e383df99cf439186513881f39cdf80e9d1cc12"),
		new("item", "earlymodern_door_wa_screen_443", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4dd5c2b977972f1224c269b84af3a003a5c468aad6211b2c69525398c68fa404"),
		new("item", "earlymodern_door_wa_shutter_442", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba8a2a98ed8ecdaf53028025b6f0bdb32311e2d99e7a1754bcb379d41c268f2d"),
		new("item", "earlymodern_drug_peruvian_balsam_salve_pot", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd0105993cc8380b1bbca1bb369c8a822f949541677394c1166ff05741d658cb"),
		new("item", "earlymodern_edo_clothing_short_cotton_work_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "32b4a64304b060bf8cbde76c9a2c089cd14b74def4c341d01b039cacc4f57dbb"),
		new("item", "earlymodern_food_benin_plantain_like_meat_pottage", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "14eb89d1a4d494fba5e8aa9db5a94836815286d5bf466e7cd2281bd00d5215d3"),
		new("item", "earlymodern_food_british_layered_trifle", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d53e409bf6cf438793be6c6b210e4c26c7893e6f782a63d8459dcf226a12006f"),
		new("item", "earlymodern_food_british_minced_tongue_sandwich", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "94fc7790e433ef5854a54a67019e8be558ab21435d881a4e0d33acf64fca9e28"),
		new("item", "earlymodern_food_british_yorkshire_batter_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6385e4cbc07041b7b456db205e4518aeec922877d25c3f50785c70525e861961"),
		new("item", "earlymodern_food_caribbean_enslaved_saltfish_cornmeal", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3392f72e483b784c66a5843ccd3350ea8eba1dd2a33af18218fbd58241cb03b"),
		new("item", "earlymodern_food_caribbean_molasses_tamarind_sweet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "df8328060d97795bdfe39bd84a298bdfe5bb028de3dab2657340265c48853744"),
		new("item", "earlymodern_food_caribbean_rum_fruit_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a78a5de07ca5dddb1057bbd433ee28da20142c7cfae5a384d452c3557a49fb8"),
		new("item", "earlymodern_food_dutch_spiced_speculaas", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a61bce0075d9c51062f6f1f2c014268050df88f9cab49a71de05b8ae65af7ba5"),
		new("item", "earlymodern_food_french_coffee_house_brioche", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5070bbc6a968775c1e7240de4d370bf32424152882c8089104cd0ffa52822aa"),
		new("item", "earlymodern_food_goan_vindaloo_like_pork", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5649b9b35eb545bea849e40eee14b6accfcb7bd3b7d76c8175e769bf03dfb940"),
		new("item", "earlymodern_food_louisiana_african_rice_fritter", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2584e4a8563b4cc0277b8f5a40df21fc31f2a5493a767a344db1f06794241164"),
		new("item", "earlymodern_food_ottoman_preserved_cherry_meat", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "08c0907a99c2ffc97e4605f78f2c75706166a5a23a29c8cb713b94ff3688c307"),
		new("item", "earlymodern_food_qing_beggars_chicken", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e888ad1e9764427c9c7effff72fcf63efbc782aad2e1f56ada6eada8e802144"),
		new("item", "earlymodern_food_qing_chile_oil_noodles", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d3e3567c3774c2239dd4bebb66f414168ae83162f559ab43d92e4d401989e98"),
		new("item", "earlymodern_food_qing_osmanthus_like_rice_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecdef74d119d07958a6999e2d01a5bbdbcb47be1e1ed4e72f78bb5ce3b5cf062"),
		new("item", "earlymodern_food_safavid_coffee_house_saffron_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a1f81e693d1f9eb430b5dcc1c5f726e7089c0def76fcb1cc22a730f29c35214"),
		new("item", "earlymodern_food_senegambian_groundnut_meat_stew", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9bc7020c4d594ba42b03f5a6338d1b4a854372fcd795a635c25ec3975792195"),
		new("item", "earlymodern_food_tea_table_cucumber_like_pickle_toast", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a538fde410639f2aa4a3bff2da1581d7874812104909495d0ae0bf40bba181e0"),
		new("item", "earlymodern_food_tied_raw_plum_pudding_cloth", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5cd741e9a7e6287632794996ba790e8c7a2878027671157ea5e83745776d8d1"),
		new("item", "earlymodern_food_tokugawa_castella_tea_slice", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0848f35471091ce8834ea2158face752056c487fb1f694fd176d31f7d3570cb"),
		new("item", "earlymodern_food_tokugawa_famine_sweet_potato", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "102727a5dc49fd936336c1ded69c86b69fa2c8b3cf99fb9930f0367f8cd3dbf1"),
	];
}
