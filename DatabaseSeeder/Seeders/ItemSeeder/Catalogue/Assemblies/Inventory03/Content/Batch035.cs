// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch035() =>
	[
		new("item", "earlymodern_military_ranged_heavy_siege_staff_sling_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f556d4d0fc3f8f372817c5dd9408bee3da633284e9697358d6f2fb68b5dd5ead"),
		new("item", "earlymodern_military_ranged_hunting_shortbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b11535290e91ce4b97ef8c478e8797450258ca0ba3aa4fe98252a627e0f6ffa0"),
		new("item", "earlymodern_military_ranged_hunting_shortbow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a89ce04c8ef0178b4fc5f2d18b229431831341e7cb11097263846143760b6209"),
		new("item", "earlymodern_military_ranged_military_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "13617bd1262f068f53c7662d3500f9955660e1e935d3fe878d72bc1a0bee9d2a"),
		new("item", "earlymodern_military_ranged_military_crossbow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "18bdc1309ac36be782dd731baf4acf832f85e639a0ef8157e1355186c69efcdd"),
		new("item", "earlymodern_military_ranged_palmwood_hunting_bow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a43de170672aeaded12ddb46f458548a3db5509a0635ade90e8d9144d5f0b8e9"),
		new("item", "earlymodern_military_ranged_palmwood_hunting_bow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "465d16880b6cd1c1a3c9f683d9bf126825cc57b11bc82a574a6304cae6590d3c"),
		new("item", "earlymodern_military_ranged_pistol_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "38c888f923f213ba58c0a3d1e20aa433b354663f17a4883ff09b9e9c0fca28fa"),
		new("item", "earlymodern_military_ranged_pistol_crossbow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b75e01018ef0b3ec50bea1a3fc2012b1662ee3bba923f4d14569d6969a156bb3"),
		new("item", "earlymodern_military_ranged_plains_short_bow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aff19e44adb182e94726c325de88313f3e4676f3c0ba7fd4e153c905af038657"),
		new("item", "earlymodern_military_ranged_repeating_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b1e4bf7a8c11634a758d9245ba93c58805e59b2f4bd929b6bff0ca533aa19f6"),
		new("item", "earlymodern_military_ranged_repeating_crossbow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2615023941ef95c966d6264ecd16904061ac6e9a22ffb189bcfd82707476825a"),
		new("item", "earlymodern_military_ranged_shuriken_star_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b79ed4952ec48cf1bbe4a19432d920f383c2bbe5b430c1547fb07982ac5bad7"),
		new("item", "earlymodern_military_ranged_shuriken_star_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "af711138d98aeb70db6d28739c3b79e695410ab072f6f10a60e998112dde9d92"),
		new("item", "earlymodern_military_ranged_stonebow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f008f772860dc8b4696d0afe14aa6e66f27c0e31dfabf412a87db696df884b1d"),
		new("item", "earlymodern_military_ranged_weighted_throwing_club_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "db28dc2a453104dc09afc0cc02866086a52d04ff2490e898083214ce545b75f8"),
		new("item", "earlymodern_military_ranged_woodland_hunting_bow_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6515ce5d9784c5633ad2372738242ea3a87f8143d571f411abe4734a3971c104"),
		new("item", "earlymodern_military_rank_silk_command_scarf", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8fdeab92f6309cadaeb13bfe26eef517e76725a05a96fc76204b14bd5685f49"),
		new("item", "earlymodern_military_shield_oval_rotella_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "745c675da24485c72e0ad26844c0ddd96a0745ba135545214845694d1493d84f"),
		new("item", "earlymodern_military_shield_oval_rotella_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "634477713aead27279a8761db9e9e60acfb6b4fa92f69720e86895070971f7c6"),
		new("item", "earlymodern_military_shield_steel_buckler_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "67d6c51cf61a2f1d5fdd5c5b055624c5acd7bcf1a8e6bd45083d5bb31803d25f"),
		new("item", "earlymodern_military_shield_steel_buckler_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3585e2037fedd62774237e7414f172a366a46b62fbfd25078aa26958bbab5799"),
		new("item", "earlymodern_military_shield_steel_dhal_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea0beec5b365b9369ae0de087283b6e7f6980170213079a2270e649c5e7fa873"),
		new("item", "earlymodern_military_training_practice_cutlass_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "650ba91bc79543d5ded9811000490415e74557be7e90610ffea245c1189fc686"),
		new("item", "earlymodern_military_training_practice_rapier_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5e5b05c4ead053723139701705ec6fdc4d411a9e67e9f2441370714096f02b2"),
		new("item", "earlymodern_military_training_practice_sabre_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "38bb186c6ddbd385321fbe33bc73906b49e2214699f64fb97edb1600fd0030e4"),
		new("item", "earlymodern_military_uniform_white_leather_crossbelt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b48c875b16322d866b6f4d893d2b793abd1262bc8259403d07713ed43c4b77f"),
		new("item", "earlymodern_ming_military_brimmed_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3de5e54b7c51a4270484fddab11631f01adaa65cf5b116ef5cb7e90451a3f70"),
		new("item", "earlymodern_ming_naval_officer_overrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "411e9a45d2cd45b44c03b4a1353df12f93d0ada4fa1398e1a4c8b03aa69c69d3"),
		new("item", "earlymodern_mughal_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c813fa391636c0ecb80cca5971427cfa79e5e4375035e2fa00f4125976f2ad5"),
		new("item", "earlymodern_noble_jewellery_hair_ornament", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "84b363ec4082e766d7fda08b424d4996d8aff8317f11850b3930ccb61c8f33e1"),
		new("item", "earlymodern_noble_jewellery_pearl_necklace", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18a046fbd569c8c568a5e9b99fd8db9e3d012dcb03094a842806f8402ed01afa"),
		new("item", "earlymodern_noble_shared_fine_leather_sandals", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f3398a325c1eb584ed48754b128b326ba8aa4fed4088a45522340a5d8318ce4"),
		new("item", "earlymodern_northamerican_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec2f493a91c5daf5066ec84380a70584d9a5bb6a68cb658285b785fc565393e9"),
		new("item", "earlymodern_ottoman_military_artillery_felt_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f84d2671ba4c8b6d2f42a4c29f5ee384fcead7118d35d3d7555b13dce6c0496"),
		new("item", "earlymodern_ottoman_naval_officer_command_kaftan", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "23631692f17e58164abeb22ef9510f4e2d03b3358fc780b6ceddcdcd7ec5756f"),
		new("item", "earlymodern_ottoman_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f50630e2c989b6f2a4bea36ca447752e767e2e0baa1fd9cf65f7843650f66bbf"),
		new("item", "earlymodern_persianate_military_flared_cavalry_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2421ea8e007ab9f57d5b293ef2e788ee04ccfafe7878bacdb78a8574e6a4816a"),
		new("item", "earlymodern_persianate_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "223cab7f25f8592e73b38f7a6e01905659f65a1da9bf35415c44c5ba42e68883"),
		new("item", "earlymodern_persianate_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce0d38f8cece1160b127dd461035ef908d986dae7f6aa48bc8589913c9969957"),
	];
}
