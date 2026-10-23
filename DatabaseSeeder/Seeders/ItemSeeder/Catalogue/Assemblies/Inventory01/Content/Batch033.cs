// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch033() =>
	[
		new("item", "earlymodern_jewellery_it_hairpin_112", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc2bb3d26f9e226af9c0ea0b39704d929739b243b95e01bea2841dcee2410705"),
		new("item", "earlymodern_jewellery_jos_belt_ornament_356", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a2d3a7701d58135f6b7fbe168de36aa016222aa2334544b07d5808af30aaebc"),
		new("item", "earlymodern_jewellery_jos_wreath_360", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "911eeef61b78d696065691bc6af91df244426277ca9d1d2b34c941a23bba976c"),
		new("item", "earlymodern_jewellery_mag_signet_ring_202", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c9e5e3da235af1595985199e346e6c6c24593efd758d6ba1c67ee686e726d9d"),
		new("item", "earlymodern_jewellery_mca_pin_651", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c15f04572ddd5cfeb55e2cfacf35d4bad63575a6cbca2edea20995145293cba"),
		new("item", "earlymodern_jewellery_mca_temple_ring_658", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "81de47a700c95808ca55e0122ef59c087abf9fe7699812e8b2c3276c5fd9e993"),
		new("item", "earlymodern_jewellery_mrd_belt_ornament_276", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e93ca25ea681cc44f332a2ab495011540bb5095f0e54dfbca685f4c3250a6e3"),
		new("item", "earlymodern_jewellery_mug_signet_ring_242", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1edea938a38836b81cde9b8beab754f7c0a6416998fb448fd646fa030f01f8f7"),
		new("item", "earlymodern_jewellery_qing_brooch_310", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "12600f86096bb749e902d0c426d1abc59c8b18c640f87f14b5d2b3cb4d0f3222"),
		new("item", "earlymodern_jewellery_rus_badge_177", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7f79c31a5513c3656a029388cd293c3d79cb8ea5934b3832885ddfb6c312e74"),
		new("item", "earlymodern_jewellery_ryu_chain_necklace_385", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b744ae63ae231ae3b1ccbe71fddde0b3b3f181b92ad8f16b7488c38db0b3818"),
		new("item", "earlymodern_jewellery_sahel_brooch_510", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "14b1534c6cd182fa01476655867b24f2d05c81c0199a46590e62158c8c84ecc4"),
		new("item", "earlymodern_jewellery_sea_anklet_408", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9716f99b419913a0459a3cf4833b5c956794f0d07ff1763f1bc68c3a3aa5b7cf"),
		new("item", "earlymodern_jewellery_sea_badge_417", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c75f773daa9a954447c4f296427b341321206818712aa931e7c71c215b2656f"),
		new("item", "earlymodern_jewellery_spa_pendant_563", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6b8247b8da9442153dd542f578dbbf9744ebff30a208d2ea3d57de0f8929899"),
		new("item", "earlymodern_jewellery_swa_earrings_549", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b977da03b4d5ccc7917fa68ace4879ec727d032b97d2bb600802bfc7720557b"),
		new("item", "earlymodern_jewellery_wa_waist_chain_475", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a2cf191479f7fe7d3fc5c77c98214d7a534ecffd9474b0beb302f9a9d7ea7ea"),
		new("item", "earlymodern_joseon_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8cec554aa1dba393f83be4734adb7164f107bec54b262a7e692708730559113"),
		new("item", "earlymodern_maghrebi_clothing_round_felt_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "577f40ffc6d8e3d63749937a05424041809aa22c727cb90d8f1085e6017d6f2b"),
		new("item", "earlymodern_maghrebi_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "434af5141482d72c3efc514196d6e1ea46ab5e2539f58dc2225ec863acd25d1c"),
		new("item", "earlymodern_maghrebi_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a787a9a0bb3679fd09c167f1430817146583163ff5e44497262565a10230c48"),
		new("item", "earlymodern_maghrebi_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcac09a35d54d5468040097b51b11bd8315f3ff739dae5b8f9fd021362ca5809"),
		new("item", "earlymodern_mainlandseasia_military_short_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec3588acd5ab243cb11c8a5c61e5feaf413a21c1d29f76547f209452277edcf9"),
		new("item", "earlymodern_mainlandseasia_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c3d666b7e024fb0e45b37827808c87f8a16ac2e27b0ff4302f508aa1d49b25a"),
		new("item", "earlymodern_medical_variolation_lancet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a9110ee0c5ff9f7830344aac02c01c3eba1ae9e64e73b86ec1ff682f1ad904e"),
		new("item", "earlymodern_mesoamerican_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c5138e9a3906ecf8a0455ebe6cb922c8ea1f05dffab23b2ee6e40d58cb19570"),
		new("item", "earlymodern_military_accessory_camp_kettle", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6167f6fe1f25ba74d2cd60f3ecade76503b774ec88b4b5134da4723570bf1917"),
		new("item", "earlymodern_military_accessory_field_drum_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "deb2c9f65362c6df2886757ed049d2c193dd317832a04396ccaadf2821da8845"),
		new("item", "earlymodern_military_accessory_gunflint_pouch_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9361f5ca2069c7606267ca7dd8bbfab18bb85d4e6f1234473bdf2336adadca1"),
		new("item", "earlymodern_military_accessory_horse_pistol_holster_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0beba1d2de9dac3022773f86c48d49fe4f7c6dcd04af5ac1816a3f5e3288e15e"),
		new("item", "earlymodern_military_accessory_leather_cartridge_box_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6a667c5239cb54b006aea6e9e7d729e15629bfb18da6d22049709e01928f6d5"),
		new("item", "earlymodern_military_accessory_leather_cartridge_box_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b6890ca6a2254e08ef0696879e3355eb88f88aee8b1a179b3ce1ee1c39c73cd"),
		new("item", "earlymodern_military_accessory_mail_sorting_tray", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d2b5452804c7bee81b83d168874b34258f1fd055811f7e9dbb05b7043056d6a"),
		new("item", "earlymodern_military_accessory_metal_field_canteen_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "748e0ddd982976bcc5a5040bbb99c304aec40131dcd13e91ac7659501f0e33b4"),
		new("item", "earlymodern_military_accessory_pistol_travelling_case_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "05c2e36b5a8d1f053ced9a71f9752d067574e9393f31c59eb39b5348aa8d7fdf"),
		new("item", "earlymodern_military_accessory_pistol_travelling_case_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "67370df0a32fb3600245fccb22d85793f552efd0ba98f8bbf8c584d285e2cc67"),
		new("item", "earlymodern_military_accessory_rolled_campaign_blanket_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8bd11ef70b9a4d262a92b1cd62517e4e9a0f9504723e03257b0269adbd0cf91"),
		new("item", "earlymodern_military_accessory_rolled_campaign_blanket_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddaaaed052ebc197391792c61a0622644c257bac6f7210a81d0f82a5ea5b60c1"),
		new("item", "earlymodern_military_accessory_saddle_bow_case_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6438bb4d33d79083b15e19e57549bb69c0335912ea3e15335444a8127c012697"),
		new("item", "earlymodern_military_accessory_socket_bayonet_scabbard_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6c46716c9fa43525302f5db71e3e1768fd654ce7988a07b7d9406158e11f8f4"),
	];
}
