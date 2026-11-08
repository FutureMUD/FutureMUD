// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_door_swa_escutcheon_528", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "39ba0c8d056e4ccb11d9b99b6ae7daf55219b576d531a15c1769dc2763665a97"),
		new("item", "earlymodern_door_swa_grille_524", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "de6a306670da6bd613aed356aad84e2b404f4d746b1bcc47d3133793e13f35de"),
		new("item", "earlymodern_drug_jesuit_bark_tonic_flask", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "17ffbdf4117d2e1a5cfb3c8ad731a3643b93a6c33642daa7af0ec1f08b494459"),
		new("item", "earlymodern_dutch_clothing_full_canvas_breeches", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ffa856b86da39a9b772a659564a472183145f1432d2fd101d6998f3c2b7f9ad"),
		new("item", "earlymodern_ethiopian_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0dcb7b7e30a844a527e135d1c76c6636d51d3b4088166e063eec53b8295ee30"),
		new("item", "earlymodern_ethiopian_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8712ceafb6ea0388495b37bf4d7e4e82dbfab08706d423447a18626b52489dbb"),
		new("item", "earlymodern_food_british_cloth_boiled_plum_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2db92868a3e09ee0f3b428f3007265a4e28e9d8c3e5b2ddceeb3fb6db7b8e154"),
		new("item", "earlymodern_food_british_pound_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7796b92ff9c8b36e0e009d3e1e1a6bf14812a70fbb2eed1337e230bbeffb1914"),
		new("item", "earlymodern_food_company_madras_curry_plate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "46e2ebfd28bf4231cc14a8e07547857e59437320faf1efa06532a7dd65122ea4"),
		new("item", "earlymodern_food_french_bechamel_baked_fish", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f929855800499aae17579ff6308839cfb8b46c56f983e862872aca536602bdad"),
		new("item", "earlymodern_food_kongo_cassava_leaf_packet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b15f9f7d215726521591b6af67ee757bd7a323e1e4447d68f0da6b7b68a1ad2"),
		new("item", "earlymodern_food_ottoman_coffee_house_lokma", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "981c53fe1e7d7240c41d9bdc391512bc77ba538a9898483b00d1c30d1e42daa4"),
		new("item", "earlymodern_food_qing_mooncake_salted_yolk", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b504a16761cccfaaffb963c224af47b5703e916982306b44c7ae633662b3134f"),
		new("item", "earlymodern_food_safavid_sour_fruit_leather_soup", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b1d2bb7535d588c0d9cea2fbba88b1383debe00c6f45915d940d65577d75195"),
		new("item", "earlymodern_food_swahili_clove_meat_pilau", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "66bb28f600b3bc117ba32eb1610f475c2b50c360ee3a8b366c88c3d2bde52a81"),
		new("item", "earlymodern_food_tokugawa_tempura_assortment", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b33fb6ce7403415f6ffe5d4a69bedd34b9f2b8cceb02daf115e9018ee18050b"),
		new("item", "earlymodern_food_west_african_rice_bean_feast", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4783556ad3b52d8d581ffc7937d14f536c15793c456dd83b73233d7c03b078c"),
		new("item", "earlymodern_footwear_closed_monastic_slippers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0da4f1ab67474a4308d71c18213399c05a96b659d4cea7d0d3991ce7059e96c"),
		new("item", "earlymodern_footwear_persian_heeled_ridingshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cdd81ba4e7dae957ecadbf159a3aee3b9b2e263ef2481210e67a2a618260260b"),
		new("item", "earlymodern_footwear_redsea_stitched_highland_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9099d72f6c3e52296af0ea2fb596a5fff01139f426f65589bc248e8bbf509d37"),
		new("item", "earlymodern_footwear_velvet_house_slippers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bd42bc1a94688a7e96ff0fc1b44d42647311507111087de371e654b5654d5a6"),
		new("item", "earlymodern_frontier_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3e0bff1e52f0def66a0c18f8c5d0982f8dfd144e43760f1a8529aa849efa506"),
		new("item", "earlymodern_frontier_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "929df324c0d2bbb932a0a45cdaafef9653629e9f172b415abbc0c042cc927681"),
		new("item", "earlymodern_furniture_and_armoire_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2afc5e768929f9bd63916515b049b572e18517022b5e89b6a526da675cafffc7"),
		new("item", "earlymodern_furniture_and_display_cabinet_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "20104c9c12c0cc1b83f924966fe60b5d87955d2238ab7f8ed5a7a421afbf0d3a"),
		new("item", "earlymodern_furniture_and_linen_press_08", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffbf0d74ba4e9739d4aac00d30938347da77cd6b476e4584808f8256956030dd"),
		new("item", "earlymodern_furniture_bra_drawer_chest_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b416f1f3821a38db6a498b56f717c6c3b9a6e4389cc1db0306162803b928da18"),
		new("item", "earlymodern_furniture_cna_display_plinth_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aacc35e26fa001894a096d032a9ab6a23c1a761ea2ce03c7880ba741086d84ad"),
		new("item", "earlymodern_furniture_edo_writing_desk_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e117e2602891f6b0298b3175b5a608460febdbac1da4b86ba3bb3f80395351f"),
		new("item", "earlymodern_furniture_eth_armour_stand_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1347cd9cfe629454ee55cc170ec827eba525712f01afbf419a8c41281c647092"),
		new("item", "earlymodern_furniture_eth_weapon_rack_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "82e1d4447cac50481b53a1399bab5c5d463c1e6c6b19ca84ba2f8465f4ce368c"),
		new("item", "earlymodern_furniture_frb_wardrobe_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e2b4aaf2ff72c9b0128a592c8402ddabbf11be4c5d9bf51c552946640847443"),
		new("item", "earlymodern_furniture_ibe_clock_case_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a6dcb7ee4e5999824294161ebb7bc76fe93ea82b8f9daf3d17402a1138e2d0c"),
		new("item", "earlymodern_furniture_ina_linen_press_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ec1b6fe1b5a1c98b215da506eb24624f8930c02d320f03e023afeadc6dd9e17"),
		new("item", "earlymodern_furniture_ita_writing_desk_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "10e7aaf151847f32e06ddb1628504ed2839a24dece46c4b4f6b74f068e113c86"),
		new("item", "earlymodern_furniture_jos_counting_desk_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "143ad0fb5494647ae4c34d96462fddef543831deccd0ef438313514cf850fa1b"),
		new("item", "earlymodern_furniture_kon_clock_case_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1c1586601862ff12dc35d53e4cfa244620f2ff357be8b9c69d6e8691d911f48"),
		new("item", "earlymodern_furniture_mag_dining_table_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d02be373192013ea31f4414cd6f6ba87ae674237fa19f60e48c267203569326d"),
		new("item", "earlymodern_furniture_mse_bookcase_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6d5b41629aa53807fa59a6790a814909ff61edbd856c07a0e7be0dfdbee9b21"),
		new("item", "earlymodern_furniture_mse_writing_desk_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "185305857506ebb0d7fc6b1c2d4807a80496c707df207309548a6e69a68c6132"),
	];
}
