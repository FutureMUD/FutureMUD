// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch035() =>
	[
		new("item", "earlymodern_maghrebi_clothing_pointed_leather_slippers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d90c7fe217f60d7d87af9ba5b78b2ab73d2aca1fd834d9e8e4c47117f0004e80"),
		new("item", "earlymodern_maghrebi_naval_officer_command_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c37f184ed23173429c1b62ba6e1030f92e0af7c8a1440e16f5b1778e61831038"),
		new("item", "earlymodern_mainlandseasia_military_crossfront_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "65462966e0aba1a5d1ebe4f81001ea8ccf53481b8f369a1e7129e500aef40619"),
		new("item", "earlymodern_mainlandseasia_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "91bdbe0a0d752c7162e6aa2d3b61abcb4f208c6e849cc837fd584e259b223fff"),
		new("item", "earlymodern_maritimeseasia_clothing_loose_long_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a15fd744e99071cfc888eeee6a5984489f3bdaa2ba25561ce2d41db93510a99"),
		new("item", "earlymodern_maritimeseasia_military_command_headcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad76b9c71d7de0e89063c46fdffde401b9ff65ba447220efc7f73d2d30e570c1"),
		new("item", "earlymodern_medical_epsom_salts_stock", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ca8c72e333e5c5c9693f98c401c2781d83652fd7d75f92a960befa44134f66f"),
		new("item", "earlymodern_military_accessory_boarding_axe_edge_case_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "98c490dfc0a21ffb2d9624337384fad9da6358ca8e4d302e8623fa15a6618df2"),
		new("item", "earlymodern_military_accessory_cartridge_bandolier_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2820c7bf0a015154a1af938298f20966f4198ac427dc1321283205a81c5fe920"),
		new("item", "earlymodern_military_accessory_cavalry_standard_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "47b7f7eb38db04a40e6249a99e43db1c22d821a737f005443c04b148e750e289"),
		new("item", "earlymodern_military_accessory_infantry_colour_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "94f51a98a610c90f03d543177eb38adb8e44ef3020540a128eb8fffaddc041b9"),
		new("item", "earlymodern_military_accessory_lance_pennant_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a3ee133fe3b4eba8cee02c38ee79cd73d578b8ddd1a10f1da111c73fb35ae92"),
		new("item", "earlymodern_military_accessory_lance_pennant_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "da74efac3936aad2cf88bbd2e919f404e4319f03b3bf3d8ffce7e11fa14f6e13"),
		new("item", "earlymodern_military_accessory_long_gun_carrying_case_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bd75610d8c7b0bbdc549a8eba9e86c11b7b8e93dd11a0ce689fa28fec6f0b28"),
		new("item", "earlymodern_military_accessory_officers_field_writing_case_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "05c160bfb6bf5dd5fb4a1bfef7ede8307f9b32b9d02c62a43451115b794c22cf"),
		new("item", "earlymodern_military_accessory_officers_field_writing_case_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "177d47ce46b40d658af119d1f2b91e1a4c177b7ecd916fc0525fe09bb1546778"),
		new("item", "earlymodern_military_accessory_rifle_carrying_sling_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "62dae5133da0153b9c97373efb533225c0efce092e558efaf974bbcf1c1b932c"),
		new("item", "earlymodern_military_accessory_rifle_carrying_sling_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "30a042bb07cadb1be778989cf48d56aa9cd17afc0b7e8630c3807ed932b26e3e"),
		new("item", "earlymodern_military_accessory_soldiers_haversack_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ad5f559a3b6d782a94a7325cb9acec1942590a2afa36004c2146d0d69eb4939"),
		new("item", "earlymodern_military_accessory_speaking_trumpet_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "20f04bdc3c78ca00d6bb543f95f02d3dd05db47e2317315d9dbf5f8fae78d079"),
		new("item", "earlymodern_military_accessory_speaking_trumpet_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "64b39eb287536f7b27eb3e5554772bea32295a2045e5f83ec4ec47ab5d6b9d6b"),
		new("item", "earlymodern_military_accessory_tomahawk_sheath_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cff8c08a7395dca8f88e0ad400a6191d57c71eab915860a028947bfb59ae971b"),
		new("item", "earlymodern_military_accessory_yumi_bow_sleeve_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb50098d9a3aba21dc28f405bc8ff469d036d3b9fe87cad4b316ca2de427a48c"),
		new("item", "earlymodern_military_accessory_yumi_bow_sleeve_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "99814c9ae490e7c19b9cdfc083075c225480bd80752bdabd6d4db61b349d31b5"),
		new("item", "earlymodern_military_ammunition_ash_field_arrow", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "95638b725b5075ce884a51c531c5bcad9edaf7446c0035841a91fe55f33d0ab5"),
		new("item", "earlymodern_military_ammunition_naval_crossbow_bolt", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ea1450825bfd6b5a4d120b6ba905e745851549ce40f85b24d35d9c613c458c6"),
		new("item", "earlymodern_military_ammunition_paper_cartridge_055", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "62d2ddc42309c618166d8fca3cd4b16b99a7e4cc2214ac180bcce2ac14135a94"),
		new("item", "earlymodern_military_ammunition_paper_cartridge_060", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "101e9d21d1327cb6909d04a0fe6dca740768ae4fbc2bf15d18d56bd7486548f5"),
		new("item", "earlymodern_military_armour_brigandine_vest_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "234cc0729c8da7c504315a952187b33b9cf932df6ed79a57e9eb2344debbbf09"),
		new("item", "earlymodern_military_armour_chain_and_plate_sleeves_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b00bd4da3661ea0b98eb4561b6794b2b7a2312079a455eeacba1476cfab7e36"),
		new("item", "earlymodern_military_armour_conical_banner_helmet_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b90fc8c1598e0ff61cbebcf310ddb72f2e4a2eaea209287a3dedded5f0e9d6c5"),
		new("item", "earlymodern_military_armour_full_field_harness_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "73505908d47709e548ab64d64a55c1f23196d3977a1072840530e9031e5fe3f0"),
		new("item", "earlymodern_military_armour_half_armour_harness_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7afa40292124697ea72914d77aae63058a4235f7dea3c09730a952934f668f7c"),
		new("item", "earlymodern_military_armour_hineno_kabuto_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "06b66125189ce1eae91aea1e92393c27faf2fd26e8198e59999798ff0e7d123a"),
		new("item", "earlymodern_military_armour_kulah_khud_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0ad1ff45c6af380d3a305df70e1b3a4935ee0e77193f47fb90958103a787584"),
		new("item", "earlymodern_military_armour_lacquered_jingasa_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "98af577ce3cab8c1ed82d93549e6114f4c526d1a2b2aade9e370c2870ab07ad0"),
		new("item", "earlymodern_military_armour_leather_greaves_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "929d0cce4e6fb804f1d48dd1aa5d5612f9873468769dfa4183960809e2ef1a1d"),
		new("item", "earlymodern_military_armour_leather_spaulders_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b601bff686c4dc96a86907d605646a20f4b9566a3b0284d42bc8dd5b49c5a6bd"),
		new("item", "earlymodern_military_armour_lobster_tailed_pot_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "90df973a3161a80639816ac5c6b01bb19d0238239d875584b5db2b82053070c9"),
		new("item", "earlymodern_military_armour_mail_fighting_gloves_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5183d93bfa0c7e33ad1caae10b7c412305b43f277006f18330ad665e96adfc71"),
	];
}
