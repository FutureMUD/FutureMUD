// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("craft", "Writing and Administration/make large liturgical codex", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_liturgical_codex"], ItemSeederOwnershipPolicy.StockAggregate, "c94c805405a72cdff77d1b6bf09cdada2bae2119c6f13c514baf18083854772a"),
		new("craft", "Writing and Administration/make locking deed chest", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_locking_deed_chest"], ItemSeederOwnershipPolicy.StockAggregate, "df7516a4c5495555235791163eefedf1339d6d44dfa0dd879cab1cce516f7ded"),
		new("craft", "Writing and Administration/make mould and deckle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_papermaker_mould_deckle"], ItemSeederOwnershipPolicy.StockAggregate, "dffe9b6ec4efb0751fd3459e7f974b677d69ac13db6526f81ca9a7f7fe748940"),
		new("craft", "Writing and Administration/make rag-beating trough", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_rag_beating_trough"], ItemSeederOwnershipPolicy.StockAggregate, "47b6f0d2e7e31b52007b38634da2e1407a6b8765087d8d1a9c2ca6069446d5ff"),
		new("craft", "Writing and Administration/make rag-sorting knife", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_rag_sorting_knife"], ItemSeederOwnershipPolicy.StockAggregate, "d81acc81f7f5a35439a0b77e475aa2dbe685e7ffb57abe027e4816e9ee29d73f"),
		new("craft", "Writing and Administration/make scroll roller rod", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_scroll_roller_rod"], ItemSeederOwnershipPolicy.StockAggregate, "6ad1a78f8f1cccf68735daa618b714f556f924eac8a078e8d613bd44da445776"),
		new("craft", "Writing and Administration/make sealable palm-leaf bundle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_sealable_palm_leaf_bundle"], ItemSeederOwnershipPolicy.StockAggregate, "cb6dbbd8d3e5cbc2756c14290dbb9bbe5f3711899ed3ef0494a7a9d9372fa3d9"),
		new("craft", "Writing and Administration/make set of parchment pegs", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_parchment_pegs"], ItemSeederOwnershipPolicy.StockAggregate, "484c2791e05ecbf826ca62363e47d4415f015024bbcb9072952fa17f57efd92e"),
		new("craft", "Writing and Administration/make wax writing tablet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_wax_tablet"], ItemSeederOwnershipPolicy.StockAggregate, "cab3660651316f3f0b6c767942e968c3e9cdde01fd80a656b6fb59a5684e16c3"),
		new("craft", "Writing and administration/bind cypress tribute-paper portable writing box", "crafts", ["renaissance"], ["item:renaissance_container_mea_personal_portable_writing_box_01"], ItemSeederOwnershipPolicy.StockAggregate, "abbd5058ac072a7cd0ae0d8497e5d959ca0dbf23438e70829243d199b7f1dd33"),
		new("item", "adjacent_antiquity_cotton_field_wrap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c57292d1b43c6a3cfc48623bdca692b7dc0cd955f64d0618dc8cf7e4c2e43842"),
		new("item", "adjacent_antiquity_horseherder_wool_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a48fad2bbd52ef1731048233e5f3ec38e5e7a45aaae10fdd2200894ed8151c3"),
		new("item", "adjacent_antiquity_temple_linen_shift", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "999c6fac49c9ddd0b684bc052b35bb6014e216a8b0c945bd3d98bb051a04be67"),
		new("item", "adjacent_antiquity_wool_riding_trousers", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e707d677458276ad209e4dc797fc41576e63d1d070706bf4d44d1a4412e7f7c6"),
		new("item", "antiquity_accessory_common_broad_weapon_belt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7732ced5092b727e8587aaa50267d32862572a585e6f42b41466a93645e7dc00"),
		new("item", "antiquity_accessory_common_straight_sword_scabbard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1bcc1a7528f7f80b7742ea77d44aaea838870fef5a0886b9c071472939532ffe"),
		new("item", "antiquity_birchbark_storage_box", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "faa56a894802dd9fbf3354f85050b98e58ff47834027157670a1402be8339ff4"),
		new("item", "antiquity_bronze_display_stand", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "36156e80e27f43cc4b9d03dccff7d3b20cd123835de38464cd7fe973fac4a9a9"),
		new("item", "antiquity_bronze_leather_creaser", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1b9475f79be01d6e0e657616bfeeb60b777bf45cb8132fa66dab827922ef057"),
		new("item", "antiquity_carved_household_bench", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d9fa6d79e576113849195bee22f1d71e6d7991cc35fa864741866cf50b1d7ba"),
		new("item", "antiquity_cedar_document_cabinet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed872f2286f8f0a1047f4ac7f0d5d2b6a66c60676f348b0571b12e127868bc30"),
		new("item", "antiquity_celtic_peaked_bronze_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5665d490c7acdbea3954c51edaf94f781ac6d3c82f068844b62e814b9dcf6313"),
		new("item", "antiquity_cypress_shrine_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e3fa33f11b200bad2ffef9dbcd920758d93f75be9c40e6cd2a30115ebf9026a"),
		new("item", "antiquity_document_shelves", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebf7bf62c9885cb0e62dfb38f4cf23c4619775d50dac1875b3199ef4204cea46"),
		new("item", "antiquity_egyptian_bronze_scale_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1e306baea0380f23ff6db3c51e9bc89513ae1150cc5b2d1254f6fb684db0703"),
		new("item", "antiquity_ephedra_brew_packets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "003d4ea7c9888af6151e0854075cfff7c57e400b313a1557425d9d57438ba722"),
		new("item", "antiquity_fine_bordered_wool_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0fc64216f4af5aa1bb7d0fbf246ce29ee65fe5d3218c2220880c2f55e0f4dc6"),
		new("item", "antiquity_fine_front_knotted_girdle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4bb62953e16ddee657ece6efa8054b68fb8d285efd1e1dffc22ef357e735359"),
		new("item", "antiquity_fine_papyrus_sandals", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecadf1c1565587a83c5f1e2de737b9378a03fb8560555b272e24b518e6ff9968"),
		new("item", "antiquity_fine_ritual_wool_mantle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "823a952063c3b6f38185c464ea320ce1e85590d1d5b12143ab94974147bba676"),
		new("item", "antiquity_fine_wool_braccae", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "628ed514ee8088b15dacd1eed0a6ddd4492779fa5a96a76c21fad089b167d09e"),
		new("item", "antiquity_food_anatolian_pulse_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "153445e6a9e3eac437ba2925ef8b0461250a227920ba8e4d8c3cc42eaa39e1e9"),
		new("item", "antiquity_food_celtic_fish_sauce_relish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "953615b68554bbe2a3ae4132a1fa0253c8524d60bc2d3f5a4725ca8258c96d89"),
		new("item", "antiquity_food_etruscan_stuffed_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "92d1cc3ff54f08d97a5114796bcb32ff11bbe0b4cdbc0a560f6e8401d1684ba5"),
		new("item", "antiquity_food_fermenting_garum_amphora", "antiquity", ["antiquity"], ["item:antiquity_food_finished_garum_amphora"], ItemSeederOwnershipPolicy.RequiredRelationship, "b415531820e73090b4c031a3e99d600264f3522769e6da953fd48e1a01cc8ddf"),
		new("item", "antiquity_food_finished_spiced_beer_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a1d614b6e64bced95cba4eeb0d13a4ded78f481c827aa0ccdd3697a0a28b973"),
		new("item", "antiquity_food_germanic_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "15777a56ac43b5e1e75e6186dd5209e5ceca7ef3e544a5457a0489892c53c3fb"),
		new("item", "antiquity_food_germanic_preserved_meat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7776227d14fc8080e73aebe70c50c96ba9d2cc5f9b3ee7cc983d51173766cb4"),
		new("item", "antiquity_food_hellenic_porridge", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "96a7e9ac484ab1c71b2445bd32bb5625037ce50b9a509f6dce1dd4c03884f7c6"),
		new("item", "antiquity_food_kushite_stuffed_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "62c2d643bca5f1d5af122cf2d54b7a80805b786914317ae15ec10621ca313a20"),
	];
}
