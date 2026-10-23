// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch032() =>
	[
		new("item", "earlymodern_furniture_ott_armour_stand_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "155b02adeb6e5b56b9b4b43fa71e3aacde260e615e0cf68d00f00ecb75f448d5"),
		new("item", "earlymodern_furniture_ott_side_table_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8a5a4fe1569f152946e30ac453b1c0eb75008d1420de6cb0c34abc370d7396f"),
		new("item", "earlymodern_furniture_qin_armoire_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe4ff20ed1cf2f32826aff170764dff1bd54f44f1ad1fcbe82f57df1fffb6111"),
		new("item", "earlymodern_furniture_qin_cupboard_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "67de38fd139ed75ae35dac11801896a04827764a252d10e90788b0a79194474e"),
		new("item", "earlymodern_furniture_rus_archive_wall_11", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d07482502d9580b05dd264122b629a7b8c3ee95d5c313abab2749b2c11b38088"),
		new("item", "earlymodern_furniture_rus_armoire_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c8c8405d32c22f1236b3bec0c3852688283b272b2be3f96441c43518d94bb12"),
		new("item", "earlymodern_furniture_rus_side_table_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1471f22618bcfe655ca5176aca27211aec76bb163975c2aa93db7b19f3da7b97"),
		new("item", "earlymodern_furniture_ryu_display_plinth_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aaabc1c194e03200deff951b562305e7e97288c7eea8b9b675685c8d88f55b88"),
		new("item", "earlymodern_furniture_saf_bookcase_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f71ada77b2fa5f0134d727b468e8f79e7357035a0ddd08b6f26e3dadeb3b9540"),
		new("item", "earlymodern_furniture_saf_dining_table_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc21b990c28202daea2d7d8a3b3ea48935c1d19e240a633fe334626d53c22556"),
		new("item", "earlymodern_furniture_saf_wardrobe_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "59473242a9f60462f177e4d178b7a55152b98e48df9a23024f61d139bd664d71"),
		new("item", "earlymodern_furniture_swa_dining_table_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c232b423227e0b45dc4eba4415834a5a49d81bc1f451c14ab52a652795a51780"),
		new("item", "earlymodern_german_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c9fcbdacb598b243129d962fd68429447838c48d51d1d832aef5eb5f63f5947"),
		new("item", "earlymodern_headwear_andean_tasseled_fourcorner_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "62e2e2313684a132c109ab0af177b24c2c5a0db4cebf82561c2403bafcab2a43"),
		new("item", "earlymodern_headwear_furtrim_mourning_hood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "90ff4c882275b78bdc38ad6c80e53444927186acad56f96fecc61439d2c49a1c"),
		new("item", "earlymodern_headwear_long_stocking_seaman_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3833b3329bb0872159fdd7cd4748d5d026fa61cdb58c63c2ab6ad292409e8801"),
		new("item", "earlymodern_headwear_straw_bergere_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf3656c4023b69a37fd0fefb3de4b94d1cbf25114044cc98004c76ed3c83cce5"),
		new("item", "earlymodern_headwear_tarpaulin_seaman_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "46eb1819c0c55612314f52e991f66fde4172cadab7e486af545c89236590b821"),
		new("item", "earlymodern_iberian_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1962154c8490cec9c094e29dc0f6c2ff4751c6786b33684d598bd25924fa1ecf"),
		new("item", "earlymodern_italian_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa3101f2faef4f9802677893b62785fbd53826c1fc840033c7d1c91b361d8448"),
		new("item", "earlymodern_jewellery_and_temple_ring_678", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4c5c4ca682c3a87b369a4ccc6c50639bfed589e18fcdc23a9390d800ba5bce4"),
		new("item", "earlymodern_jewellery_bra_circlet_593", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eec41af2477542cd337568fd030a2d7ba8fa507ea935eec5c2f43caf463c40f0"),
		new("item", "earlymodern_jewellery_car_wreath_700", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "763a40975aa4d473ec75bdb93cf123427b77d3a99bd0439dd8d106d13fbabb57"),
		new("item", "earlymodern_jewellery_edo_belt_ornament_376", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9937c02fd6fc8fc42be67862ee2d166917c389de045975c13651aaa5cd23edb"),
		new("item", "earlymodern_jewellery_edo_diadem_374", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "17c437a5f2fd82d00aa2e28ef8229eae538170144ce66f63aa73adc6f1096db2"),
		new("item", "earlymodern_jewellery_edo_signet_ring_362", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b35e3a892edda8cdc78727de171c32ec2f08db4d916cd47ca6fc31711422fba"),
		new("item", "earlymodern_jewellery_ena_earrings_609", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff6ffb926e8fed527ce2c811850ad0d05ebc4bd377380655ecda0947ae961d95"),
		new("item", "earlymodern_jewellery_ena_torc_619", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c09e6dfa1f81f5f8ab16144061555661b0f205788f80e6b5d9ac28969677aede"),
		new("item", "earlymodern_jewellery_eth_earrings_529", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0099ebf1447396450660d31be9a43f2fbf8751fbd4f196b1de9ab94ccce967b"),
		new("item", "earlymodern_jewellery_fr_waist_chain_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "310373720d3e0052a912ea11c02723f7bd3aa67f1ce5f107c5f7a5cfef7ef8e5"),
		new("item", "earlymodern_jewellery_gb_bead_necklace_44", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3364bfb026a2a5b5ae94548268b7c982836a14d24a3a73ac41ede532b3fbf9da"),
		new("item", "earlymodern_jewellery_gb_chain_necklace_45", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "03e527b2c006698741ed2ee206aa036672f526a7296e900497eced8e0d019d77"),
		new("item", "earlymodern_jewellery_gb_diadem_54", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "88307bf73bbb2287e4b52e976f7ea261207edde2fa86d0f36aad64f2820a0aa4"),
		new("item", "earlymodern_jewellery_gb_pin_51", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "70bd72b45f5d851b52eb7aabe483c8b815b230e0079721e90e179510c5b5cf94"),
		new("item", "earlymodern_jewellery_gb_ring_41", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ff9215fefd87dacac235fc4eb1b5fccbbb66aa3b1d5f3cc9d5439a62a92e051"),
		new("item", "earlymodern_jewellery_gmt_chain_necklace_705", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f9e3b577d34a1ff696d938525e10c3dfc865020fdc2288f680823bdb7042064"),
		new("item", "earlymodern_jewellery_gmt_wreath_720", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1e8bda3384ad695bef2a1f936d378a39f632be8e68193cf6f579d71006a0b55"),
		new("item", "earlymodern_jewellery_ias_chain_necklace_445", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ea44245bd25a9a11264e7ca3597f0831a83fa48cfa4e0b75cb7ae3e33a8bfca"),
		new("item", "earlymodern_jewellery_ias_ring_441", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "acd875716788d7ec08e46c88056359f8868c1769e4747f62c82649b9ddfdaf4c"),
		new("item", "earlymodern_jewellery_ib_ring_61", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3188be275cfcae6376a649bfce8741096544f8ac9d5dd4f473d6958bc24f68f4"),
	];
}
