// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch036() =>
	[
		new("item", "earlymodern_military_ranged_shuriken_star_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c50749fe0860b91986f4bbb7ff05dc4d530e02504345111c47b32a8a58ad618"),
		new("item", "earlymodern_military_ranged_throwing_tomahawk_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4463ff68bc55a4be44c95ae53fcf873e009613ad4aef0c25ccf4c9503fa9f17c"),
		new("item", "earlymodern_military_ranged_wall_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aeebdfdbf6d4768567cdfbf262a3aa78e109223e19066a416592b43f4282a6b3"),
		new("item", "earlymodern_military_rank_single_fringed_epaulette", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39c59b232ad9ad1205919e8108af6c718745c5df72de14ea54be0af4edae4828"),
		new("item", "earlymodern_military_shield_oval_rotella_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3d8f8c513e2b2a9b87b18d36194b893e4f1baf1f536bbd9458156406db8b3f8"),
		new("item", "earlymodern_military_shield_steel_buckler_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "557f775019c49db4a6769a8ec1730d72b1c483d8e6f39143d859849a996ef98d"),
		new("item", "earlymodern_military_training_practice_axe_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e684f01ef2583c81cab9ccbdfd5ba24532b71a9926399ad72f78e164c409cd7f"),
		new("item", "earlymodern_military_training_practice_flail_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf369f6b49a0cb2926f4ee20f5cabf12823dbeb3f06093602beb997c74544fca"),
		new("item", "earlymodern_military_training_practice_mace_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c426943e80612c31173c3fb1029a8d079ae00dbbf2e7c6c8cccad71d0d143b9b"),
		new("item", "earlymodern_military_training_practice_quarterstaff_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f35a687ccf834dec5efe33402845c4b27c1f6899d6ff218146eada77d59a0a2"),
		new("item", "earlymodern_military_training_practice_short_spear_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b83651b1e4882e361b1e899bc0e6f6424398d494461d159860e9d938d4f85cb"),
		new("item", "earlymodern_mughal_military_quilted_jama", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3717af881f3875df89b67889e2071d1b793a45277028e64f55cf111da860128"),
		new("item", "earlymodern_mughal_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76120dccea8c38a9a3c72a228ce8219333fb41ce917dd3e7ac740a1e6a341839"),
		new("item", "earlymodern_noble_shared_fine_hide_leggings", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53c1a24862044a4c4b54a10a9d3ebbaac1ff4897ae464d92325755886fc3fc99"),
		new("item", "earlymodern_noble_shared_fine_silk_inner_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f65009c93ba789ea502b77f1169e53f417591922699963356dd5ef6458a639f0"),
		new("item", "earlymodern_noble_shared_silk_waistcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a79458d10ca69aecd686f8aabf864a676e2f18ffd0d3e4e142c06d57bf1ec9b"),
		new("item", "earlymodern_northern_clothing_striped_wool_skirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1eb37419e890773db530f07a9549f417d124502d4159738f40f5035ccdf34bda"),
		new("item", "earlymodern_ottoman_military_long_service_kaftan", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f26098757e52ff7a16a161252e38aecf5e6bf83be2c4f0076f5f7494c7303077"),
		new("item", "earlymodern_ottoman_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee5d30426d08ff42625026a277f131a755cd7a1c14fdc988ec57c89f226b7150"),
		new("item", "earlymodern_persianate_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e8124a14df2b6236cc3716979efb998b3abdff1e28548404d5613e870799414"),
		new("item", "earlymodern_religious_bektashi_taj", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3fd421506694c22d2a0539f390e8762237c5ca478d65ffb2cf2a12f240de708"),
		new("item", "earlymodern_religious_buddhist_prayer_beads", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "69fa979d197a556820a7c94c49a99cc4d1380f82100fe07968cbd1540e9abd45"),
		new("item", "earlymodern_religious_ethiopian_white_shamma", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82af33db2785441892abca6eb345676142303082ac2e39df3bcca439ab75ac4c"),
		new("item", "earlymodern_religious_local_feather_headband", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c8c1ed3133b87254ff0c01a2aab1260bea8a83d1e4f8f6e527bfb2c4adea697"),
		new("item", "earlymodern_religious_theravada_nun_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec64acc2b2f220e24d3f08301947be499d01d57ab403637819ae4ba3204bcc70"),
		new("item", "earlymodern_religious_western_tippet", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "92cf1d0d3e08fb42f8754a216f65d4e445f1a647c5e253ca2884715fb01e78ac"),
		new("item", "earlymodern_russian_clothing_high_leather_boots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcec0f8b22b193d859bc7805af7d6b12c857e7c60b393c350ae8fbb2c85c0122"),
		new("item", "earlymodern_russian_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e13f26238f01597099eea6a17e2325080b4ab74c65704340be3f28320f947fd6"),
		new("item", "earlymodern_science_spirit_thermometer", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4501766b3026994403fdcd5b9b0dfc1c9096095951b7ed7bc5d00bac44c7b43b"),
		new("item", "earlymodern_spanishcolonial_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "45894ed8fc5850837014e3410ad187b48d726a805c40adc62a9f053628d41d9d"),
		new("item", "earlymodern_steppe_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "03ac1cfcd75700f9758847602565dfc70f447d6e95ee76f22d63e5acb22617b1"),
		new("item", "earlymodern_swahili_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "38d6812fc0cf7fe7d30ead3746b22552a6e4837a53a442f23475f9ef5b836995"),
		new("item", "earlymodern_western_clothing_cocked_felt_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2086c7b827ff82ec48f0a7aaaeac6d8d5c3e2ab7dd00301da5dc275dfffa307f"),
		new("item", "earlymodern_western_clothing_full_wool_petticoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ab02188a4efe37c2e49ce8a38b3bbe5c3d3244a0cd2139ae0c1caa9a9291da8"),
		new("item", "earlymodern_western_clothing_knee_breeches", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dca905334b5cff0458d612030d036e5ec7e8720630277f5fcf5c2d340878a100"),
		new("item", "earlymodern_western_clothing_linen_work_apron", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7581d498c5927e9222c41ca58d3769dd4ae12c8191f2a41755a1ddbdc274ae8c"),
		new("item", "historic_warp_weighted_loom", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "74af02fc8a923f59369b0c8f88d85761ed644a2d77f5870d91bcd4da82a52df5"),
		new("item", "industrial_agriculture_forestry_fishing_0006_field_shears", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1bab8d18cda16cb250e49c483e1153a93ce35685c0f73be13dd726a774fb05b"),
		new("item", "industrial_agriculture_forestry_fishing_0012_countertop_hoe", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "77a3b1371a40759263cb4cc64232cd9f2aa91b60e0808f35b124a53089bc918d"),
		new("item", "industrial_automation_access_control_0004_precision_relay", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "24923d95120ec3db321e66c0cc61e8ce97de51ea11cb2cbef2319895bce1f242"),
	];
}
