// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch049() =>
	[
		new("item", "medieval_jewellery_garland_wilted_violet_wrist_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_violet_wrist_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, "247007c447e6223c3767ca5438cd13c952ce6c5a3d49f3bad0ea985b88a56bb0"),
		new("item", "medieval_jewellery_head_dried_flower_wreath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "056149ab8d8bb0cc4140bdf6f370669d71243b0754c19037fccff95959e6e9f2"),
		new("item", "medieval_jewellery_head_lacquered_bamboo_hair_comb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4473be4ef16760fffe42d2a89283abb087c32b32b9dd0f7fd854e3048f54e8a"),
		new("item", "medieval_jewellery_head_mother_of_pearl_comb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fe0008aa49757e0cb5ca51d3ca9bc59b1c2cddc5f148640c78ff51a1c1ff0e3"),
		new("item", "medieval_jewellery_head_pair_of_wire_temple_rings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa229e8209ef571592cee29f80f645ffd91ea27b88632f6c9745ad7d762d23c6"),
		new("item", "medieval_jewellery_head_silver_circlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ce4d023d8c765635559f3a9b417e95755617f0fc6c622d31f6028c74fb2983d"),
		new("item", "medieval_jewellery_head_tortoiseshell_hair_comb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7e7659f071730ec8f1dbfefcc938fbd7f3e72ebd6c7c9f5549ee5f9eaecb938"),
		new("item", "medieval_jewellery_limb_braided_hemp_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7701545392474ad38dda8511c8d5e2f35a79ff129934c695f602e52327153700"),
		new("item", "medieval_jewellery_neck_chalcedony_bead_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "82a3c642816cc24ca9ac033740290a3a151ef522739fa34daa76ee40edd53f5d"),
		new("item", "medieval_jewellery_neck_faience_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3da4363e94ff32b0b4aa94414eac34c8c79e476ca3b501ae569699e2d995dea8"),
		new("item", "medieval_jewellery_neck_heavy_court_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a35e2603c5e3526939da0eac4abfe1416ebd161f0cfc869c948c0d6506ff8b21"),
		new("item", "medieval_jewellery_neck_heavy_gold_torc", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5634c5a3ce5f1063c8aef101035839e63855dc17a6ef94c704bf9d0840776748"),
		new("item", "medieval_jewellery_ring_gilded_copper_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1337d91a1205cc5cdf245e1fb9cde03988a023f5badcb99619489d22a1071fa"),
		new("item", "medieval_jewellery_ring_turquoise_brass_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9d0891a126c1c58fa9a8ae38c498c1f252377257373ee8c8d44b6963af1d8b5"),
		new("item", "medieval_jewellery_waist_silver_belt_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a8304b17cbbe36ca68b4cae417f85c4940b919f77ab55c89558acc5fdad7b29"),
		new("item", "medieval_jewish_genizah_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd0f09cd453a3f7ca811913f70fe8db51b77b96238fa2ec0485f2e73f377c595"),
		new("item", "medieval_jewish_ner_tamid_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "67eb58afa03f4fc4adaae472658f3c4f4c43379e2ec89b90b8a0889c2a66e464"),
		new("item", "medieval_latin_white_friar_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c86f88f806120e2fc0043a32acd98adcc2c3fec13533020d1c5bc3bf4506f3c"),
		new("item", "medieval_latin_white_monastic_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6418365cc39302806d573eef3e8fa064b3565168a13e1354740c8bd402673a3d"),
		new("item", "medieval_leather_hawking_glove", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecbf8da6fac6e13abdb4f996c6373a5eddfad5e7bcc5db783ababfc90f39319c"),
		new("item", "medieval_light_gatehouse_cresset_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f84fcbdb67b6b50d2b7eecf267d0d2701af295aebc129aa6f78fe532068c0688"),
		new("item", "medieval_light_lidded_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c97871a2dc86354ded5596f3174139fe469361186bf2f2479893fb761f673e8"),
		new("item", "medieval_light_pine_pitch_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3570c0b6cfee8f7c595d94f0af1e61ab1b61bfb57cd269d06840c290b210e7f9"),
		new("item", "medieval_light_south_asian_brass_cup_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad90fd35b0c8542171bc91fad70b4b2465199ec55b257d8898a1dc6618a570c6"),
		new("item", "medieval_light_south_asian_teak_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf366813ae3dff40606b4f27d97c82d0f51bd4134953dc48572f575acf26f091"),
		new("item", "medieval_lined_scholars_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4928bdcc6fe1abd0036e6f76e08569d1627a76b61355aa7dd450b2682b65c1a"),
		new("item", "medieval_linen_breastband", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4060e9e8ff3699d620d85e1ded62c5e4be936a5fc4be8e4ee2792d5b2035ec8"),
		new("item", "medieval_linen_chemise", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d41ebb1a5a7180c53dd5d7ddef7a510976eef039f9ae5405054093175d3b3c2e"),
		new("item", "medieval_linen_saerk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4fcea7827a7ab5b7697e160a00b5153e07ca52cead9660b3e22d31cfd97332ce"),
		new("item", "medieval_locking_domestic_deed_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe3c098b7ff5fc0f9b6ead508fde8aab9f136364c36e453cf9b12f58205ebd84"),
		new("item", "medieval_locking_domestic_ironbound_safe_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "46bfe55acb3c63b3671c262c1ac2194d40be2403e2111a510a1bc441240445ed"),
		new("item", "medieval_locking_domestic_linen_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ceabeca1f3be69366d1aefe85872b374da39cf8a79a34fa7d04a97c7f4f8c8c"),
		new("item", "medieval_locking_domestic_northern_sea_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb443ba6009a39709e6746ef57a4fc9097b72375787fc4d5535e5be81f295f5f"),
		new("item", "medieval_locking_trade_market_rent_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "453511460c31c8cfb648bd4fab051a3539c7ce86ab5b14c9dc03e7220927a3a2"),
		new("item", "medieval_locking_trade_spice_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e086342d577b3bf1a0727f452250e1fa33ce3a981e16575f2b59a735da4ba593"),
		new("item", "medieval_magyar_bronze_mounted_belt_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "59b9081bd35d6c5d3460d31c99ffd9b0b6be6af3ed3c0bb49a76cb07a5f41a00"),
		new("item", "medieval_medical_garlic_salve_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c05d3757c6a122d48b0d158b3f9d2930bad6fa45901fe72c7aa6d0c1bed23cd"),
		new("item", "medieval_medical_monastic_dressing_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b670a8d043d54942599b7afb9038f2c1daaedbd479de5a65a07ac33e3d39ee25"),
		new("item", "medieval_medical_right_ornamental_foot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f33ee4f95b2d413e424da0fe6661ee13b412ea337a43b33c51c48290f68744d"),
		new("item", "medieval_medical_vinegar_wash_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6331710149a2637c3ae75fad11239e9f9adaabe10d25a162fdd2279432e4b39f"),
	];
}
