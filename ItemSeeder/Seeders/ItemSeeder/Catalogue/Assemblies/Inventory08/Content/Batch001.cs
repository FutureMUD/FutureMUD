// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("commodity-spoilage", "Smoked meat commodity spoilage", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4c4767d92fecdadc345d4e178c5f82ab2497da4cb973d1f39bd35253ba83768"),
		new("component", "Antiquity_Ostracon_Surface", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3eb133a578285939b063999abc8887fb5488c834b29f146ba39c349317dcb1a"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_caribbean_callaloo_like_greens", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "922b816f86dc6a0a5b48fb5330f0cb6b8c9cd161cae50a786719da48831bd7c3"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_chocolate_house_savory_meat_puff", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6dea2024198277d5760e5aff7f8e76584c1c9e08ec351db0fc7de17a8c2be2e"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_dutch_company_spice_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "05abb081ffaa785aa34981ad3e5b18a873e0d666205751515dc4f6206d929d51"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_candied_chestnut", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "91e8c6d4301eeafc010a6ed50d1bc9245dd9a0706632b57576f6660b52b7b05a"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_cheese_gougere_puffs", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6d687099f65704d65587e73a85922f52d222e03eb6e33589a9e0fb881e2bf62"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_german_potato_dumplings", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cef4aa17b9736ae5f36025f6b38ec4d2c79dc6a2c529fb9c89d3789d881d668"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_goan_coconut_like_fish_curry", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2201be3d2964bf0b14e5ec00948a0b8f741ccbf7309b6694e7e3a8dfd4108d2"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_iberian_crema_burnt_sugar", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3857f0efa2e3599c1c961f0c23aff0f1868714675c392d2a5d318fa418538c9"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_joseon_royal_gujeolpan", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "69ac2162d1b9d16c37495c190aa800d20509a1f909392d9e5a1f8f46b577fa84"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_joseon_royal_neobiani", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "906ef7f0351f7b119b38e9049582f17b11e9b58d9668cc738e85a6396634e658"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mesoamerican_convent_mole", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "617565091e0fe1ddf4954035202b2260a71b1a8c29992d71e5133d0627a4b7ea"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mexican_barbacoa_consomme_bowl", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cfcd9f057e9a1c78fd99b2f6239939c96e54122b75e5eb09ec743b044221fe8"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_kofta_nut_sauce", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebba4ac2cf8a1421bef81fdc8b78f1592d1fce717031784bfc597b413cae3119"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_tomato_meat_qorma", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf3371435e0ca8d28611c04602da457d70f048578823310e48d23dead54be180"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_naval_lobscouse_pot", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "009140e76a9920056b9469348e2076814fa9d05e6fcb393910ee141528be508a"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_shark_fin_like_braise", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8fa27592cd4cfb6b4cab5cb31e1b6a5e3ad300628f3edd4afea962bd535375f"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_senegambian_tomato_fish_rice", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a1a2ca30f5159b47b75fe123827ae90f0790e6f60508ecf0036cd315e266fb6"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_bento_theatre_box", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "75d5f5426854f3881547ce3b3f104970dfc08e3ad107d46b972abc6e68c4cb31"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_nerikiri_sweet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d1a8e8d1295e6217d4ce70b86eb9bd5f4a2858a639ee75ac906f39bc10f8b28"),
		new("component", "PreparedFood_Catalogue_medieval_food_almond_lamb_harisa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "73431ff68870cf2f93d62f1f37efc3893be6ee89f75d333d8d9b94f77782faba"),
		new("component", "PreparedFood_Catalogue_medieval_food_infirmary_almond_rice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d819ed90f18e7c859655b8e1e6fc87fe14c309fff2967ae070172a3acffcd4e7"),
		new("component", "PreparedFood_Catalogue_medieval_food_japanese_mochi_offering", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d98b715d239ded469aa47c856bd85ac1827ed86706151947d445a5b2331d729"),
		new("component", "PreparedFood_Catalogue_medieval_food_longevity_noodle_feast", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "801f62542800b47fd6167ef98362a1ffd589cb2769faae609790b94b6d5e4e5c"),
		new("component", "PreparedFood_Catalogue_medieval_food_persianate_herb_kuku", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "204bd980ae6e9f83e483cb21e8db4e89b9a06235355575dc4eb5b96493397962"),
		new("component", "PreparedFood_Catalogue_medieval_food_royal_heraldic_subtlety", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1382653c0c3b4b148ee5fb525d5aba2973f1afef7156168b1e3e01034ab9068b"),
		new("component", "PreparedFood_Catalogue_medieval_food_song_market_filled_baozi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d998ab1cf1c94cf39ab34dc1f365ff929a52b1919456710b67c66426726a10c6"),
		new("component", "PreparedFood_Catalogue_medieval_food_stockfish_almond_pie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6168521fba1121df20c857286fc8d6c8eb3b42107288ddf0cb5b9d7c4f5774ba"),
		new("component", "PreparedFood_Catalogue_medieval_food_swan_gilded_roast_service", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "948723d0a327d1c4d02eb4816c993122d45a9b9c042e78bebe68345adbeb4f33"),
		new("component", "PreparedFood_Catalogue_medieval_food_tutmaj_yogurt_noodles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e04ffd04cef625850196c2789971652cb534dcc4b9ac393deabe5eb2423b835"),
		new("component", "PreparedFood_Catalogue_medieval_food_yuan_mutton_baozi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6ed10e93612e91da019e9fe169dd52983bb13c6ce62ced8f0e92c051a0dbcc2"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_leek_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f23cf7e708e4634330dd6ddad862052a4257916c6f171a3e185bb7b2af7b504"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_maize_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "449e857b0c405691701d5e1faa010840be2585706521367254676d74885ee0d8"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beef_and_barley_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bb1b2ef1aafa6a3103a49f0d8a2f508826591dfd1d5f0457c962e8646b78180"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beef_and_cabbage_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f26c8d3ae0582cd19f7f4c08127971871a4aa79e2795adb8d637859b2e3e2a75"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_berry_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3738a447300de90ac6d383b53bbde2429099fa5ac2688e08ff557ec6384d5d0f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_black_eyed_pea_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5d4d7f41c14fb3c37f94162251c42c3b3b573080834d31bfa5bb1ba0f63818d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_blood_and_grain_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e159864d309395f081f37d6f063e3ad29b2464dd577367c649e51acc2b9964e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_blood_gravy", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8391fb0398214d5950dc7e345e54c364beab7165876424d6ad72f9f757af5568"),
	];
}
