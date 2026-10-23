// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch060() =>
	[
		new("item", "renaissance_food_japanese_shojin_sesame_dressing", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "60ccace5509a575ecfa7a103af98f0875cf93e5c7f209350f8d3476106d69edc"),
		new("item", "renaissance_food_maya_pumpkin_seed_sauce_fish", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "de0aea3b746eae2f07f5d6fb7b4382d14ca2545ed81494f4651ce3d338be78cd"),
		new("item", "renaissance_food_safavid_rose_almond_paste", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b7fea336af156e92df7f3fa9993d5a84a0199c8399138646f272eb31951d2a5"),
		new("item", "renaissance_food_toledo_almond_marzipan", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8d037a3cd38b6cd223abd2b4dd0b7674bc95ea12139d45269527ec4f24f9036"),
		new("item", "renaissance_furniture_and_harness_stand_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b17ad0c9e536bf44fd0f989c373df7cc478b386c8fb55a30c65993d15e24c86"),
		new("item", "renaissance_furniture_car_harness_stand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f203799cac95f66e22a00b24b9c036830f6dcf98336a2b4811374bf9cc9a646"),
		new("item", "renaissance_furniture_car_spear_rack_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fbdc1c8a331de9200549cde8aef59cf5f53af1d33255c151214db05f4bdd782c"),
		new("item", "renaissance_furniture_cen_folding_display_stand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0631a07553e6b637d31abc07c9e4725aeb96d52c2782e37bf10d9bb61d7d248f"),
		new("item", "renaissance_furniture_cen_lock_fitted_wardrobe_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "80666db60643e6fd5cd354d82602608a60643fdd316ecf431ebccb4429f3f1bc"),
		new("item", "renaissance_furniture_col_escritoire_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd474e9694412f6c4ceff112c13417512eab2fe8f33b93ba8b8a533ae3521a41"),
		new("item", "renaissance_furniture_col_map_drawers_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "42f8601e9c77a403bdf5b5a2a5a88098cb86550926acdcd42bf0459f79797f6d"),
		new("item", "renaissance_furniture_eon_writing_desk_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "613f099cd408cba9ac06e1ffadf29f1b5f74c6cc7b1dc52bc87866d3e1fa7b96"),
		new("item", "renaissance_furniture_iba_campaign_desk_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7010085792987dbe588780287f9a984972a6bb39f91ab1fae3cf0525acc20b29"),
		new("item", "renaissance_furniture_iba_harness_stand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "24e206eff37881eb9af58101224f6003fd4ef0cbfe98bad819a33157a16abdce"),
		new("item", "renaissance_furniture_iba_vessel_stand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4426566c9283980b46d449f7548674bc6611f5346dc866e83b9337686f8fb11"),
		new("item", "renaissance_furniture_ind_folding_side_table_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f526fd0f29d067ae1ea71b35a6a162676c811a46fcda1fc11ae2c7d33f95490"),
		new("item", "renaissance_furniture_mar_wall_cupboard_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d5c40222426d65483923811025b62cc48effb79afe4d6dad11ff0eac59921d3"),
		new("item", "renaissance_furniture_mea_campaign_armour_tree_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01fbe968902f12bc19427cf7c197855aaac65f7d38d4d2357b69cb5fd0670d57"),
		new("item", "renaissance_furniture_mea_folding_display_stand_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "becd421f315350872e9e66ac1dd011ecc713832de0ea7058cb9f458a309289b4"),
		new("item", "renaissance_furniture_mes_helmet_cuirass_stand_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "036ff815c5b15e1f9d29c450613b54aea7b46b2d303e252a83426c116b9f9a24"),
		new("item", "renaissance_furniture_msea_end_table_02_lockable", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "57b986a0ef7710ae1463db7865c411ef4aab4313bbe42b6fda33d5721265eee8"),
		new("item", "renaissance_furniture_nba_matchlock_rack_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9386edd7ab49e97b6603eb52c7049aaf132166c125d74c2817122347d2a2aa8e"),
		new("item", "renaissance_furniture_ottoman_bazaar_brassware_shelves", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "548464d592a8f7c024681293a8b4ec9ae49d7039f04e349ffa559f85d7cf1550"),
		new("item", "renaissance_furniture_pip_freestanding_weapon_rack_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "41db9efadd8e3f179d110f69fe469785c641155c0095eacefdd621a04457b9a1"),
		new("item", "renaissance_furniture_shipboard_purser_chart_cupboard", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aadf37ed1b7a8a9bc294cb7e5bba43874e8c0d0eecf93e0b0503d22781aeedbb"),
		new("item", "renaissance_furniture_steppe_caravan_low_folding_table", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c48f7959965fcc686cf96974ea1b6abe297bbb276c15ad46f5014fd43357395"),
		new("item", "renaissance_furniture_stp_armour_tree_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "361d3847b9eeaefd14362d93a03327b933481578f4d0cdbf0c60e96c59b9ae1f"),
		new("item", "renaissance_furniture_stp_mail_stand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f3259c95307a070bf966239078ece2afc3cc57b8ab5c42f086253fff7e91149"),
		new("item", "renaissance_furniture_urban_kitchen_dresser", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfcc832a87f55c427c302e3a562dbc5741c340c6f3b0addfb835de955e926d4c"),
		new("item", "renaissance_furniture_wer_secretary_cabinet_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67599bc5a8be71cbc4dc0ad2ded687544f4bae0fb3d3047aaa2d078fc2611e6a"),
		new("item", "renaissance_furniture_wer_urn_stand_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc2c83f03c0965f1706b4c8915b162bea3998f24a127961934ffc99b548903cd"),
		new("item", "renaissance_furniture_west_african_court_serving_table", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ec391bb778a44acc9ad48beabbfbbe478b7fbe91f6389a12f73cf440ff9472a"),
		new("item", "renaissance_iberian_high_sea_boots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4e84c7ff82eba30ffb9d55bc2895e46d5d2ffbc646aa514c0118c142e8a3cdc"),
		new("item", "renaissance_iberian_short_sailor_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "da94c2f492ab3c0399cfdc2a74df653188863d1cf8bbde370c823b7fe325b4ac"),
		new("item", "renaissance_institution_liturgical_alb", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aba8cea44345fb6fe52872a30f786bdb8e52f1e452662ab85120beb32065eddf"),
		new("item", "renaissance_japanese_tied_straw_travelsandals", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "149015a5c0d587fe3575878f4007d7f9e1c33bf13da04f08c78e89097a0a9178"),
		new("item", "renaissance_jewellery_aca_portable_panel_479", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8216f674b4b85f64d2534f061167e532a40f47bbc82c89b1036cd0fe54f9597"),
		new("item", "renaissance_jewellery_car_earrings_639", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbb046561c56c4b72e36c56b7d13889f5d717b40262dd86052073ec8c8778029"),
		new("item", "renaissance_jewellery_cen_belt_ornament_76", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e091fb9ab87cfc967b32ad7df74211c8e27940806655757112bd941aaaaf2ed"),
		new("item", "renaissance_jewellery_cen_devotional_medal_82", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6087c8ffbbf5637714524c60d31a6f7555aced55048a36686f355196499901ce"),
	];
}
