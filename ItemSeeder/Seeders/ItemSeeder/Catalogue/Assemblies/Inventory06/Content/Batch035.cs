// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch035() =>
	[
		new("item", "earlymodern_military_melee_parade_halberd", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d113a481d2e24170eadba34eb1d8be3d9c7d700326d2ff8847492a9c4005e597"),
		new("item", "earlymodern_military_melee_steppe_cavalry_sabre_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea07c600ef7709fac07c88c17bbc8f38c217778f5738b0c6d2eb5974c8a0d504"),
		new("item", "earlymodern_military_melee_two_handed_great_sword_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "48df4ed072f00576b04e32ba5832adc7459f97c9d5a9855a6e03868a432f0858"),
		new("item", "earlymodern_military_melee_walloon_sword_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "602c334303f2910bb1e44415dd14207e731cb20d5def0a25b11b8ace2efa31b1"),
		new("item", "earlymodern_military_naval_swivel_gun_carriage", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8c681881170e890f2eea65141481c36ce7b8d740354e75d7c0f7d7d0b97b511"),
		new("item", "earlymodern_military_ranged_african_throwing_knife_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc48b7e2dc78e4912402fbdd3ab820cc601474e928cba53cbf9a5fd3d98ebd42"),
		new("item", "earlymodern_military_ranged_assegai_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a993d154691db45737c3653431b235d94b0f11ccf9aeb033b40b82f492ce175"),
		new("item", "earlymodern_military_ranged_bamboo_prod_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2fd242c92b76597baacf957c08eb778c633ca62113dc4ea2471b46b0e730dee5"),
		new("item", "earlymodern_military_ranged_composite_war_bow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "150312a0a9cbcdacf60fada5008cfead25639c530e5826061a0145860c1a7555"),
		new("item", "earlymodern_military_ranged_cranequin_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd7d836825e784904c4c7677bd9e2d86d53ca46d483b9d9a37181a152d7d0dfc"),
		new("item", "earlymodern_military_ranged_goats_foot_crossbow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcea686997c299762aa3c60833fc31996b9d037ee2452b23cce8373ac01a0b84"),
		new("item", "earlymodern_military_ranged_heavy_war_javelin_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "647e303da3415288deb558d1562c8436b09074cb8ffd2f21c4f57b1f419afe57"),
		new("item", "earlymodern_military_ranged_naval_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b707dca2855f5634817ffc78680381ed994486fae6c3e51759901377fe8f50a"),
		new("item", "earlymodern_military_ranged_pellet_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a295d0f791c9c6efc86b56b580831c16eafbdb2754290742d5dcc87f008dc724"),
		new("item", "earlymodern_military_ranged_persian_composite_bow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "09dd578f41b63d50795ade7c947b8cf9b6a9d053affcb66a3b542f04da3e87a6"),
		new("item", "earlymodern_military_ranged_plains_short_bow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee7caae1320e88cbfebdbce1c7b83eea5ee5c9ce5045e58d37510083f23a5a56"),
		new("item", "earlymodern_military_ranged_stonebow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "16f72edf778237079b2e9f774a7a256756a5587fe2b5b46694ee05a353f7de2d"),
		new("item", "earlymodern_military_ranged_weighted_throwing_club_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d4308a25187473efe2eef53c08ad87956399a07e54df2fba1d773d14295e6ca"),
		new("item", "earlymodern_military_ranged_woodland_hunting_bow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "26674c28ff2138a0f4ab76f8e65983103fe99155c794b59dea8e0343fbd5af8e"),
		new("item", "earlymodern_military_shield_rawhide_shield_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "defc6f2f3c65893ffad5a4baba16ec268b7f969a8434000dc7d7f3ca7ab790a2"),
		new("item", "earlymodern_military_shield_steel_dhal_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "04ef81be81474b197b1fa5c16a072ee7c0d272b9fde6c6355e4c6e267a10453d"),
		new("item", "earlymodern_military_tool_spanning_hook", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "29ededef92a8cd2630c9ab03853be35c5a4d08a75d509de6675004444813b28e"),
		new("item", "earlymodern_military_training_practice_backsword_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "37573f0196b7c59847a8f101cdfebc0a0663a96f8cd0977eb4c931bba9537002"),
		new("item", "earlymodern_military_training_practice_bayonet_musket_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "59104f90d0dc1f081301d6f0363f1e51db76f7daf04caba703c1bd7852386cd2"),
		new("item", "earlymodern_military_training_practice_mace_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ace323ced4d4c7db271ceb00ecf694e07683ecb09c4afda7728b06330c2dce75"),
		new("item", "earlymodern_military_training_practice_quarterstaff_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b06af690277d2a24c3733ef1653534a434d72aa4136d163e0869e57ac4f76ce2"),
		new("item", "earlymodern_ming_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "84136c7d74bc0001fd55f6d4dbbc5fb579b006437e3f893317336b101cfb7038"),
		new("item", "earlymodern_noble_shared_fine_cotton_loin_drawers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e975145ee501b186a1ad15cf4d6d68939ca8c16a5db7d017da5539407d0ddca3"),
		new("item", "earlymodern_northamerican_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "974b6b6ef2b12825c2df262a41acbc1c4d4753cfe045d40e1a7cb4a9e2c25487"),
		new("item", "earlymodern_northern_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5eb4c8973e4c0d97661a33db471b7fc97918270d5a770db42ea1b7f5ae54001"),
		new("item", "earlymodern_persianate_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "272676f1967a1d1242b0857e9769e9266be1836367e9a983566c99d1981d25c2"),
		new("item", "earlymodern_persianate_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f29d585a45b391b9a95d2bb6dea3d41061893d618b063ab93c4190c52172452"),
		new("item", "earlymodern_qing_clothing_crosscollar_innerrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2da15dd46386061f7af883dbd179317b6dfd5014963eac539c946e5fc24b906c"),
		new("item", "earlymodern_religious_ethiopian_clerical_cape", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd525477aef3530a9c732d913d108cec5313828ce48fe95333649573798433a7"),
		new("item", "earlymodern_religious_local_shell_necklace", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdfb1661a3fa8a39c3f8a8dce65694683430971d6c7177897b3f9f30f894430a"),
		new("item", "earlymodern_religious_reformed_preaching_bands", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "38df493da35a62c42ed6528223f055160f6c99a6c511396e7eb33af30ded8bb8"),
		new("item", "earlymodern_religious_theravada_upperrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac417a6bc54d472672a1a0d2d9eef9e5607c0a54bd4ae2058106fb0ddcfb066d"),
		new("item", "earlymodern_religious_zoroastrian_prayer_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7925fecb51074defd7667ebb6d31e5572310a78c013eb97f717a29f9ef12e72"),
		new("item", "earlymodern_russian_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c7e3e09003e7b5830f91a71362e481580a30e878c4cf56d53dd5b46a72e223d"),
		new("item", "earlymodern_ryukyu_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d67f57658f0e069ee96ed9c298571c7d0c269aef7e68dfc44ca3a90fc44b3649"),
	];
}
