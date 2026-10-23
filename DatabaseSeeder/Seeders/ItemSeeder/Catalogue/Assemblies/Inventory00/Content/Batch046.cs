// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch046() =>
	[
		new("item", "industrialised_weapons_police_military_0215_reinforced_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "94ee22476491dac88981b663779ff4bb8da1faf92d0b08b249cfd1eb2824d2b3"),
		new("item", "industrialised_weapons_police_military_0216_reinforced_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6edc773dbb0421d30467297d234e3c02783fa75ec34499d4b6276194f7d250e"),
		new("item", "industrialised_weapons_police_military_0233_heavy_duty_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ef55c659d50bd67750ed4106800fa723ad04ded75029e04df0bb4b7cda85662"),
		new("item", "industrialised_weapons_police_military_0256_portable_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "66fbbd8e43e70a38ea6572e8a34ac347071d3ea1b08e0270afa2848ae42dcda9"),
		new("item", "jewellery_bone_dress_pin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2888de55f6c2f1c452e01d3e7c3ea8f635fb3dddf8b54203e12d197b482b20e"),
		new("item", "jewellery_glass_pendant_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bc7d23b456e99fcf06d68e2f826e09a72348bd48e695946019f23facc8b44f3"),
		new("item", "jewellery_gold_boat_pendant_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "161756b47488de37ea8dee231c195ae47352afefa58605fd61870e623894aed2"),
		new("item", "jewellery_gold_carnelian_signet_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e36ba75ef7da506b44d15ea5ec58f0cc4ac649bba231ddd975cdb9aa95e94c5c"),
		new("item", "jewellery_gold_filigrane_glass_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a66c7083c319d153d678455c92099a77e213fa60846ef1ae8a466f904c0aa63d"),
		new("item", "jewellery_gold_foil_amber_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "019964975f5ed878d014b410146fa0bafaf02c82955d53855fa2174851b7f1b0"),
		new("item", "jewellery_gold_lion_seal_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "521fad66f24db63e23a53412fea67d475193505fd9bf4117c099a603123c1d21"),
		new("item", "jewellery_gold_quartz_disc_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6270fc4503eb1f7c5ffdb5a532ea56de281a622da0ecc3c22e96a5e00b1b1249"),
		new("item", "jewellery_iron_signet_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ded773c08945ae76ef6764613798064f8d842439ff54bbde5877df3ff6c3777"),
		new("item", "medieval_buddhist_sutra_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "643498a27038dea40f5bd8c3fbd6d20587bea33cfe36887d6c443b99c6f17d9c"),
		new("item", "medieval_christian_iberian_olivewood_document_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e12fda46e648d48fef5a6bb7402a0358b076751a07b95b992647d3b1212daa9e"),
		new("item", "medieval_christian_iberian_olivewood_linen_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "84323b79315b8eed96ed2c08673173a98129b05d2818ea2438b307443915f659"),
		new("item", "medieval_church_holy_water_stoup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5108045e1b56ecd8d9c673771c5d199dcae8df1e6861b9d26e3faae455923199"),
		new("item", "medieval_church_rood_screen_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "93ba1ab0956b9c8c0c8213d111a80447c97819413f8f8619db1511c8d98c4eb6"),
		new("item", "medieval_cotton_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee7f21618fbf8d5692f6d8bd332c0ee993091dddc8d8d4b56a90b822bdd70b6f"),
		new("item", "medieval_decor_east_asian_paper_folding_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3275b1beee81c74b4d2d16dd6e4db86c27c96feebc584bf946d61f877329cc3e"),
		new("item", "medieval_decor_felt_floor_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "681286468f71c66f3dd29fe2ebb3acbd3af0b59d8e68379812bcbf2ed62f815b"),
		new("item", "medieval_decor_hearthside_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac5ffe92741dcd4462c4e1b6f66d82e803a1a4da708d396646e635366124dae1"),
		new("item", "medieval_decor_islamicate_geometric_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb7cefa9e7ead4432374e25fa7080508835da58211d2c00c652dee7bd1485359"),
		new("item", "medieval_decor_islamicate_silk_room_divider", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f58fde359ab2d819842ea3694f0fa95e76839115d76301bd883a2d847fd6d3f"),
		new("item", "medieval_decor_steppe_travel_room_divider", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0740299d4ee5cf496a2c32737db737a41f43ba9e73757a0ed4e99a4db95a47a"),
		new("item", "medieval_decor_western_wool_dais_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c40adb946278074f4331f06dbe3a97ed77bf60ce5bd15033abdaf1080c0ad731"),
		new("item", "medieval_domestic_bamboo_back_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "18cd8f701aa31eac8180c87d807ca04249a8e26187fe556f5fa2edb8e6421f5b"),
		new("item", "medieval_domestic_bamboo_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8de1c2ab5eb2df5b1edb41c85c8b9a2647407b327a7bc6c1c711dc387e8b040"),
		new("item", "medieval_domestic_bead_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "49c80e8c9f1be75169f8773a566b58a5edfbd941222f34aae698c5ee25f70286"),
		new("item", "medieval_domestic_bedside_night_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e72f024d057086e0571ab36e42628a2ce17635787f103ed415177a53a0a64a5e"),
		new("item", "medieval_domestic_belted_ring_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe62d0869290ff23fed6c461ae895774ae29dfc6103700a6fc00633df91266f5"),
		new("item", "medieval_domestic_bone_lace_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c02466cc53da747199071f1a0dc4644a4824148f6edb11b4c6155f55fe5a36a"),
		new("item", "medieval_domestic_bread_hutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "95c3922325d713f8a3c8e57d07e64e4dc8892c5a1bc27d9dfc419dfc1c7851cd"),
		new("item", "medieval_domestic_candle_lighter_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "efa6fff5585ea1662251b1c590cb7f4c644f17a76f0878bf880b2f247c6c532e"),
		new("item", "medieval_domestic_child_carry_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5714ddcbb947ce05ff8960d7e3b94eeea748fd190fc71404f500ca293747a79c"),
		new("item", "medieval_domestic_comb_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9878d7673d9a68fb9368f01cc3ca3d5ceea5589d3a8632d71a35488497a6b059"),
		new("item", "medieval_domestic_devotional_display_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d3c59eb04d64a3c8ae24130a6abab3f26618d50c660dba65ba7ffc349529e17"),
		new("item", "medieval_domestic_display_hutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a21b1caf15aff52f94dbe8411e4e196e82b7ef8f9ae8d38a507df5c90e60d9d"),
		new("item", "medieval_domestic_goatskin_style_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa7dde14922e1d401409b3cc41decffb82abae3632597129884351b462747219"),
		new("item", "medieval_domestic_herb_gathering_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a8325856c68c85bc85fb71294c42a836bc7c8ba177d97880e3bb9fb8fb3f0dd"),
	];
}
