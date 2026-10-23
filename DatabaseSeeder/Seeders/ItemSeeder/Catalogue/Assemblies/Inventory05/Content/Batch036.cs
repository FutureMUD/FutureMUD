// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch036() =>
	[
		new("item", "earlymodern_military_melee_voulge_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "687be4b24fd1afe9ab310217cc06958fe885eab14aea534968e1baf348ddb6b0"),
		new("item", "earlymodern_military_ranged_east_asian_military_crossbow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "42f3c77eddbff8be8500fa24afb4d339eac249bae45ad154a7c6c889774e1a65"),
		new("item", "earlymodern_military_ranged_goats_foot_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "77aeab75c9a4095e1213b045de152539c0555b311eb36294de71a24f2dd8475c"),
		new("item", "earlymodern_military_ranged_goats_foot_crossbow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e54853d66c5795308f95904fcb91f01a3b9bffe960daa12a9db4e954ef9939af"),
		new("item", "earlymodern_military_ranged_hardwood_blowgun_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "237e224185862a801160da41b4a15fe2c8b9fb41232d2535c8975aebe9b5aa1e"),
		new("item", "earlymodern_military_ranged_heavy_war_javelin_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c9459b718030c16a924f1fa32153c3c7d1ec092c0ad126f5ade34014f053b43"),
		new("item", "earlymodern_military_ranged_heavy_war_javelin_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc57172fcc3084e82fe68a497526dfe9e6009d9f5a3e343511f3600021b97ba5"),
		new("item", "earlymodern_military_ranged_light_hunting_crossbow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f142441877a627a563290f2006699ba34c57a8f56a1b06d1890cf848fe75a199"),
		new("item", "earlymodern_military_ranged_recurved_horse_bow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e96d2e31a1ecba3ee9e9ec933f9712b2787c774ef551505bc9b51de387236d8"),
		new("item", "earlymodern_military_ranged_spanning_hook_crossbow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d7cdd9779bbed14eec8a06c155647eed588f42bbd32129694b9e4f76638b6a8"),
		new("item", "earlymodern_military_shield_rawhide_shield_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "86ba13736bdc083a7ba4ad7c73c049315b081a08b7e6168a2eb24dbf9b6d1a95"),
		new("item", "earlymodern_military_shield_rawhide_shield_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6183961c7835af5f9d033f30f18e31de72e1212c07844c8b742c5915a91bda10"),
		new("item", "earlymodern_military_tool_windlass", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2d1380105584436cbc76ed65d03970ae185cebbd8012c839e62b2c75227e1a6"),
		new("item", "earlymodern_military_training_practice_cutlass_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "74868d7e747bff67d51ea3c724801b669edecac3fb1a205e9f6483c8b361f965"),
		new("item", "earlymodern_military_training_practice_lance_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9997f863db3320d53408d831ee1515d5963d67821b83a5434f10abf61c7451a0"),
		new("item", "earlymodern_military_training_practice_pike_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "81cff379226e58c4cc16b20bd59907c1b31f2e3a8d53c8a27c6498724ada90c2"),
		new("item", "earlymodern_military_training_practice_rapier_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9310abd39fb355d0be79c797164216321ed01f66b7cdf19d1b83f353b546f033"),
		new("item", "earlymodern_ming_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9c9fb7e8cf443518e54f8bd0ba359cf4fac0660fdf2882df99c7c82da3484fa"),
		new("item", "earlymodern_ming_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "47ca9ad72ddba495029fdf3fd41b5d6186213c8d755a8c856c9aef624e35df4e"),
		new("item", "earlymodern_noble_jewellery_gem_earrings", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "faa37b569751e3afc92757a89574f58bf26a4ad8239a8396f59728714d0f658f"),
		new("item", "earlymodern_noble_shared_silk_petticoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "15d0ae974a8d2dc39d957724cd594e603857c10009d2763a581d2ac3da187973"),
		new("item", "earlymodern_northern_clothing_knitted_wool_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3710be56901dd3db97f7a7044cc4a63770105816679c22c3d666aac1483549b"),
		new("item", "earlymodern_northern_clothing_wool_mittens", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "da5ab99b69d4e0cbfeed36efa784885e8d6199baf593a62b87b5fddcf0a6a64c"),
		new("item", "earlymodern_northern_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "10c7b2304b12239f234b6106cba12fe7483271195ae68401f7f4ea357d85ee9c"),
		new("item", "earlymodern_qing_clothing_long_sidefastened_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "14e624eeba66a4e492dd99b1fc4ced7cd2035dc23fea011b93f29dabd48e40a8"),
		new("item", "earlymodern_qing_military_banner_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "408afa2714d1cf63d991c53a1b9f1beac795697d7aeb219e3d582379b63a99ac"),
		new("item", "earlymodern_religious_eastern_panagia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d80dd8c77be304affda8041d701366216ce2924407b4e88226c1c2cf83c66997"),
		new("item", "earlymodern_religious_hindu_sacred_thread", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1061c686d5d89edcfafd5fe7e08dde5fcc85cd49eef7a1c4bcd49d1954ba2711"),
		new("item", "earlymodern_religious_jain_mouthcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "762bfedf0ca4f7447f8c1757d910e3f466bc4e705cfd05d48d465ade7edbb321"),
		new("item", "earlymodern_religious_jewish_kittel", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebdaf51070b8525b81e17fa49196a19631567a050980179f99e6ac8abb90ac05"),
		new("item", "earlymodern_religious_ottoman_ulema_turban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "edaf46b9d416deb81680617eced19b20cf0667fb7dd41dc8df02053ef7e0cfba"),
		new("item", "earlymodern_religious_tibetan_shamtab", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "582d5188b76f6cedc01fe9a5738276fa031c611aa51d8a53a1dc0e6fcd2cac96"),
		new("item", "earlymodern_russian_clothing_round_fur_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2481c7336111efe86b050ea44628c4c5a61de0b2c1e028d28a175e3d72a2115"),
		new("item", "earlymodern_ryukyu_military_guard_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e37a46cbf438a010c5eb9f6e4a891e4037a7abf8543418a2f97128e8c6f19aa0"),
		new("item", "earlymodern_science_weight_set", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "df69376cfbf6a0b282a5b8def3a18f597e01cfcaf812edb76da74e25d3fe9225"),
		new("item", "earlymodern_spanishcolonial_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1da7e4689c962d081fcacdab6376d9f766cb4ad68716411b44b933faa04fa4ba"),
		new("item", "earlymodern_steppe_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "34896c0db675d3f4d3705a7e7583381c35ac3ab2e719a816d9b0e9234965c607"),
		new("item", "earlymodern_western_clothing_full_wool_cloak", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d17adc202122f7a573ae2a02e0d28094c137a9cb98e0bb3d6b14bcd0be96a0f"),
		new("item", "earlymodern_western_clothing_plain_linen_shirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "195180c317c0ad95fd3d44218d576bae2fb58a3d0b75409692e2173572087b1b"),
		new("item", "earlymodern_western_clothing_wool_bonnet", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f00b098135d4f4ddb3728a05698dd289242f72d36a03f07ab84487dd0f7e800"),
	];
}
