// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_jewellery_mag_waist_chain_215", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a32856f6b9139bbd0577524a7a8db679f0ea1ee2611b70aeca4f3966c95805d6"),
		new("item", "earlymodern_jewellery_mca_hairpin_652", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3896db753bf298030e74eeca241abbacf6f101a07e29870bfd161fe455a5284"),
		new("item", "earlymodern_jewellery_ming_belt_ornament_336", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4150f5834ae2bb8a3081dac4038582c39ac9414ca8b2a98535f67284f807f4e"),
		new("item", "earlymodern_jewellery_mrd_armlet_267", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "12806d2b3c216259fd9daad4074b497f3e01cafe47a950232a27c4ae6aaa0831"),
		new("item", "earlymodern_jewellery_mug_armlet_247", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "80d07456939f7cab4704008acee5acbdd2553389f90f06c049532bedc90cbe3c"),
		new("item", "earlymodern_jewellery_mug_waist_chain_255", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2083b5d493ffa7869041667c3968472137ba124cdccd4ccd2b5a821426df066"),
		new("item", "earlymodern_jewellery_nl_chain_necklace_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c482ee6a5c6b1bb0d8364067cc7c5b2bfedc97b02ee2d911daa82c93c23ace3"),
		new("item", "earlymodern_jewellery_nl_diadem_34", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "288891fc7d87c53bec9243cb7d8be35cb37ce37a7e31a3bae2d40005a69ac844"),
		new("item", "earlymodern_jewellery_ott_ring_181", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac487febe8f916b97d778ad0f84ea0a5bd04b4776edfc1ee21db028700fc31dc"),
		new("item", "earlymodern_jewellery_qing_chain_necklace_305", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "79c2fbc216d0f2444158a65b7571ad0a22d3b133bb3cf4c1b105525853ea5656"),
		new("item", "earlymodern_jewellery_qing_ring_301", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f03b80e7834f1d77ecd2d4c413fdad986b22693f2e278d1f6700d9207d9c79fc"),
		new("item", "earlymodern_jewellery_rus_armlet_167", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "86c04f799cb931e3aa3ea07e51af8b463f8bd83b6187b4e32068e13dd36890e0"),
		new("item", "earlymodern_jewellery_rus_bracelet_166", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9f24604eee88bcc72add93c178fced7b551205905c129c706c80288049a9c63"),
		new("item", "earlymodern_jewellery_ryu_earrings_389", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d22a2593509d45a25a4d37cb5821d63b17389fcc6e065e462e98418ed23b7b4"),
		new("item", "earlymodern_jewellery_sahel_anklet_508", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5f021025e51263e01a2751b19601c1cd1760d95bb848666525d827b51fb6096"),
		new("item", "earlymodern_jewellery_sahel_pendant_503", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "76db753ddd48db0b58c25f7027622006c182786ffa1f83e6233493bc6e9671e4"),
		new("item", "earlymodern_jewellery_scb_circlet_133", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e07780a5eb9309b40c49ad26ef2df19fb5796d16f9d8c02dcf0c2b5d3145638"),
		new("item", "earlymodern_jewellery_scb_earrings_129", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc278664d5b5f41eb0317b00fd2b0796af5df3647d4aa6390d46b31b7c7d1243"),
		new("item", "earlymodern_jewellery_scb_waist_chain_135", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0004a7a6681209620c9610fd107b83531795a80f92eeb6539c35c94e473cc2cd"),
		new("item", "earlymodern_jewellery_swa_ring_541", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "151bcdb6d7faf4e78ba17ee1aa14f5ddde9b3e99fd2895bf18bc492a702e23be"),
		new("item", "earlymodern_jewellery_wa_chain_necklace_465", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "be3502ac7e5c84c3065a5edb0bc1b8eefede3aaa6708eb1e397b3469c4392960"),
		new("item", "earlymodern_jewellery_wa_signet_ring_462", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9ac783c4f64a2eb2df5ba4230c62cb1a41a138ac6cf7e1b7d5bbac04635947a"),
		new("item", "earlymodern_kongo_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9040aaeeeb9767535a91628fc2fe874af5a858db1cabf8d2e858739ab5492165"),
		new("item", "earlymodern_maghrebi_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d29eb1fda84c38dc1cb6398855ec25ecbe0aa45dac95f6707f8ac08280c58bf3"),
		new("item", "earlymodern_mainlandseasia_military_command_headcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d83520a5a183cfb5f961b81c961419e115a546f2240bbf1c4acfb218e72c4762"),
		new("item", "earlymodern_maritimeseasia_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea7dc4ea075bc487ba8b12ae79a559f1429b8d61c790655e16f290b99aa86101"),
		new("item", "earlymodern_medical_powder_drawer", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cbd05932c4e7be827290b1bec8e44df94af758235926dbdb2bb1f982acce4f9"),
		new("item", "earlymodern_mesoamerican_military_quilted_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "29e2f6998ccf56c1d1e793da52425e6064dc031b869674b7064fe13eb11c4e75"),
		new("item", "earlymodern_mesoamerican_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3216e1fc69389da684c88ee601b4397a733943517fcf9f89057e430053eb9516"),
		new("item", "earlymodern_military_accessory_boarding_axe_edge_case_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "75dbd7860b6fe9120caa71eff0a32f73dd007ad9dac1a6ff6699f7abe87482bb"),
		new("item", "earlymodern_military_accessory_campaign_bread_bag_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "120bc0480bdb00340d2bac07e43e982292d34ce200dfb9215fac738cf68cc50d"),
		new("item", "earlymodern_military_accessory_cartridge_bandolier_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce786bb9bc0c26bba90f29ec7c10050da13410aa998eb9f5876c48e1c340d156"),
		new("item", "earlymodern_military_accessory_cavalry_standard_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c0deb7ffc1031fe2caa21f1ce92670a623812a8d34e2d4c297d102a19d6eb15"),
		new("item", "earlymodern_military_accessory_cuirass_hanger_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3298b7c20f82d6c8002d3dc82af27cf1580fee2761e3b389166c1f342e3d0f4"),
		new("item", "earlymodern_military_accessory_gauntlet_storage_box_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "38ef0e592ee14e5517d33e7ccfb788dbb8f9f1d575972dacbaf10ae84c5406c4"),
		new("item", "earlymodern_military_accessory_infantry_colour_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "06b8c203e638ecec8fafc86dc55923bd62066cbae83d8ca7f110b86cd06c7952"),
		new("item", "earlymodern_military_accessory_kettle_drum_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "02edd8c7c307f3d89e2034bca4e52f7aa96252b95c0f94136f877ab77de7da2b"),
		new("item", "earlymodern_military_accessory_long_gun_carrying_case_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "84158aefe8d66979ab0fc339011792e17287b9b7dc4a2fc5fc939132095878b2"),
		new("item", "earlymodern_military_accessory_match_cord_pouch_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a7ebb7809d0a0461b2a4f85c7b9fc98ef3e6d72bb31a4cb25811afce8cb72f7"),
		new("item", "earlymodern_military_accessory_soldiers_haversack_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e3c54cff21a6772fae194feb37eac98cef5ebb18e0cba0d104b23c7766aa072"),
	];
}
