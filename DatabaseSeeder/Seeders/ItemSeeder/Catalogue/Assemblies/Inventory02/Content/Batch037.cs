// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch037() =>
	[
		new("item", "earlymodern_military_melee_spontoon_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3638dd91782729ea321b752ce5725269b2eb5113b8d9215b2aab3b4dd98f273e"),
		new("item", "earlymodern_military_melee_stiletto_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "11224fc8d1ea399aff8a58d51b5f497f6f502ee52ffe104fc90b21e37e92febc"),
		new("item", "earlymodern_military_melee_tabar_battle_axe_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b59b358375e4680e21a60cd3dcaadef7ad7f1064fd00ca88f84ec032ef57d2fd"),
		new("item", "earlymodern_military_melee_tachi_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "086e20f1d47e03d60afd41b46b98e953760fb3a9040ed7b1401521d3768e5be8"),
		new("item", "earlymodern_military_melee_voulge_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "395b369fc5e95ccc08c465b2fb56010ea93c0304e3b77c92347d9d060f381443"),
		new("item", "earlymodern_military_naval_boat_hook_issue", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef4513d54a68de10804fbad8654cd6cc54aeeef6d79757bf980ffbb9f151df30"),
		new("item", "earlymodern_military_naval_cannon_sponge", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0261356d393bc695ecb6e4b1c85e7cf9e25963268ed731713083ef4ad0005df2"),
		new("item", "earlymodern_military_naval_gun_handspike", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5020ace3a4fd4d2a892f3f35c021e542ebbddaf5b7c86007baddbc1b7c51c36"),
		new("item", "earlymodern_military_naval_shot_garland", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc0d335dd156375ddf59e47cd1fa2c41e72837b84883b028c17e254b2a4887fc"),
		new("item", "earlymodern_military_naval_tarred_fire_bucket_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec52cd62e715c9c19d546d5529ca9d27c8f9422ac6eace900b7e55bd5f98d15b"),
		new("item", "earlymodern_military_naval_tarred_fire_bucket_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "caa2a7cefec3b3f8327dc53ee41a029ae2826e3fdf52a44b48bf5f96bf94012a"),
		new("item", "earlymodern_military_ranged_balanced_throwing_dagger_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ccd35fa4b9bfba61cf64dbb9206387c4421aa50756c2d9c8eb4e24ee1fe25700"),
		new("item", "earlymodern_military_ranged_east_asian_military_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f22b8019cd1399d79b16c9d4ea35aff4bb9550ab869200a282a3ae80eb60ecee"),
		new("item", "earlymodern_military_ranged_elm_war_bow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "301a87339e9361ff831bc5bf6b4f544eb8833ad53b1d736bf612ff750b2472a4"),
		new("item", "earlymodern_military_ranged_hardwood_blowgun_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1704110f2bfdef072487d74f8eec1a248a7c016ce8fb028c2d05aeed621d0d1d"),
		new("item", "earlymodern_military_ranged_leather_war_sling_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "976e113c697534ad885508d75da201bc5e1e12393135859dc86613fa6583540a"),
		new("item", "earlymodern_military_ranged_light_hunting_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "81a321afc34168a43de7711ba2fa1a78f0693971a83cd9d129d4592ca0d109ef"),
		new("item", "earlymodern_military_ranged_military_shortbow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e21fe71e223c6fbf6d35b0b3e10f7fdcbfb6dc09794290201ddc055ccc51db9"),
		new("item", "earlymodern_military_ranged_mughal_composite_bow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c109dddbd92a59d9259f0d6e6f1763353de18b23c2ade66e80b3a3e3c71c2cda"),
		new("item", "earlymodern_military_ranged_rattan_self_bow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "68f3be89ccf3155cb0803c2e79ebae5f906a648b99e7b121fc4e4139b0b2c634"),
		new("item", "earlymodern_military_ranged_recurved_horse_bow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2eb946639a6339911f07541d4ea0b4da8d84b2e94f6790fbdfa1dfa9595b271a"),
		new("item", "earlymodern_military_ranged_south_east_asian_bamboo_bow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "35a4025d752ccda65ac42f5e984926cd129a8ad06098927f463dd4ed016f5cb8"),
		new("item", "earlymodern_military_ranged_spanning_hook_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "661cbe9261aaee8a398d3fd5972deb2799bccad791c9b12da5efb23a5d9c110c"),
		new("item", "earlymodern_military_ranged_wall_crossbow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "477f2a3a41b820065e860343cb0b3347f1e82ab31dee657b02637c04c9e80998"),
		new("item", "earlymodern_military_training_practice_dagger_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f13cfc4bdc48211c7f71b0c99a524a9f34d871063829862804377ea3c8f0201"),
		new("item", "earlymodern_military_training_practice_glaive_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f75e693a49be5beee5ef101518875ced3cd3358f51ebee522cb82683653a6fed"),
		new("item", "earlymodern_military_training_practice_halberd_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ba18b29cfb28cbf159f86290278ad9e08a9db4f7bfbbebc89e8b005cdc0042a"),
		new("item", "earlymodern_military_training_practice_smallsword_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8b33fb994fdcb1ab602f5d39392d9d2547b553d5a1d4bf2d0fb5c831f9ab76b"),
		new("item", "earlymodern_ming_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9adf9783ea7c9dfcdb338e7f1d1fcbed77a9e4eaa1c1b5a8e5b13de378911bc"),
		new("item", "earlymodern_noble_shared_soft_hide_moccasins", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f44b511bf66a19521521f42b2ef8cdc387c6fb4509a61e770b1c17cc4a6c40b8"),
		new("item", "earlymodern_northern_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d388d83de3364a7c328afbaf6c2e4ff520c7810e9e197a1889748e95b084554"),
		new("item", "earlymodern_ottoman_military_sleeveless_service_vest", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "149991c4d04125817337582028a3e495880f337b38e00be00f26ab219e9ad0c3"),
		new("item", "earlymodern_persianate_military_tall_turban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd38276ab1af4c228b18d23801ccfb0d919aee3270b55164e7f8d5c9ec2121d7"),
		new("item", "earlymodern_qing_clothing_head_kerchief", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa2ec50dd551536619fb171d223f76c883614dc2c0f51b56bf64fac7ea054c81"),
		new("item", "earlymodern_qing_naval_officer_command_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "941ee8321c44ce5902ffb2f8564015f3c607cc9ff262b1c0f6b27ccde71a76bd"),
		new("item", "earlymodern_religious_capuchin_pointed_cowl", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a6877e5929f90df380a0aebfdbab1a9bb67af91a1405a8190a1065f3d1889a8"),
		new("item", "earlymodern_religious_ethiopian_pectoral_cross", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9d6623be7d4c64812b04fc34417330c5a2f52f85604715151fa1c53eb64751d"),
		new("item", "earlymodern_religious_sikh_kachera", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7515a725e44ee3ee6bade1a2743d68b2d84311532b4868168fd2c563e1f6cfb6"),
		new("item", "earlymodern_religious_western_rosary_beads", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d58435ca385cae7cd97824162506ac0e49a83b359c9c4aa9abd7675921365b6"),
		new("item", "earlymodern_russian_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b44f59fb09753bef88644f13576a87801725c8bfb5febfd4f60be0232654494a"),
	];
}
