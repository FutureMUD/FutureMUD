// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory15Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch053() =>
	[
		new("item", "medieval_writing_parchment_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8ea3b8c1c7c41aaf45c2ef57360382bd12bd5fde878969e3573204c82272548"),
		new("item", "medieval_writing_plain_rag_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4675bafebb8127a58d8df5e72fe46569219dbeec2d01fd7b16ef7aa9957ce191"),
		new("item", "medieval_writing_qalam_cutter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e306847748f172f1cbafe189f4708a8f9f04f940af4ffe0df8bfe366f9a05246"),
		new("item", "medieval_writing_rag_paper_account_slip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b58ccfdc0fad49bfd7713c75e12766ab2f0bb369961e2ade5cd90cd06fe5f2e0"),
		new("item", "medieval_writing_scroll_smoothing_stone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3231a120e8992165493a051f3e2e522190bf6473c83bde602f3cf9f97851a322"),
		new("item", "medieval_writing_small_clay_inkwell", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "171c99c58abd1264b7635248f8a27ca53d9e11662b3c06d1082dc2b378f0429e"),
		new("item", "medieval_writing_small_parchment_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a713a1e76172e157f9f8ae54fd762fddb21af54f23bbbe36168f09a35a7af941"),
		new("item", "medieval_writing_spare_quill_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "69c6998bb4a30ea4efa81a6b41505c2bd164cffb19889e33e70649ba0eff23b9"),
		new("item", "medieval_writing_temple_palm_leaf_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "32249125dbb87f3fe3dfa7e5df79ae1ae095de0bdc7f43d3ccc34aa077ac4d59"),
		new("item", "preindustrial_door_barrier_12", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7893d9c9dd56cb0900ffdc317942465d92da1045b49c05d47d640fa6f7f622a8"),
		new("item", "preindustrial_door_hatch_13", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ccd954cdf521fa56263b2c6b2c4d86e5774ab3569de32459a61952801286ed95"),
		new("item", "preindustrial_door_panel_door_01", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d145f85765f027d57d807daa0b4fecc7c52a08732938db11975dc972e47a53ec"),
		new("item", "preindustrial_door_secure_door_04", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8de61aad47c2b6f8e4625fd6d68e8701a2644b9b02f36fa37b67da047ea27f7f"),
		new("item", "preindustrial_door_strike_plate_72", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab674a0cb7fc3abb60129459a4f020fdd439e2355c30245b725893259da9c533"),
		new("item", "preindustrial_food_barley_and_leek_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ecfd2a3e860bed153b86efa41d80970b9491f82e1eab3fe4b82725e37faff96"),
		new("item", "preindustrial_food_bean_and_beet_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5c2d3ed77adbe21d5488f0482b5a7aa2ce1ef2264c4ad8bf13122142270815d"),
		new("item", "preindustrial_food_berry_pulp_travel_sheet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dc56e6cbb4c2dd82ed9d82dd5146ffe6bf735b6d4cb94a168aa6eabac37ff74"),
		new("item", "preindustrial_food_boiled_chickpeas", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "46a7fedfebd00ef2bd0fd8a861fb196b1b572b7f9bcd9a8a7c277a0319e1b991"),
		new("item", "preindustrial_food_boiled_fish_roe", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82ee36a523da9f5511818a1e8154fa1463d0b60dd4673915cd48184408929d7c"),
		new("item", "preindustrial_food_boiled_rye_berries", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "387064b18ca11a9ee5a67797ffd035b44789c3c40b1ba0f50fddb53904d7f1cf"),
		new("item", "preindustrial_food_bone_marrow", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a357437add0edd55f527c364206e9c9fa3f046eb0b271fbdab586affbcd981b"),
		new("item", "preindustrial_food_butter_short_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78e58ba163b69594a4f4963b1fede804e81da68b91a9c5d88497589b9ffdce68"),
		new("item", "preindustrial_food_buttermilk_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d22ae3b9f212fa273623d66d5c1fe2a535917bb28d4fdc39901c4bd3c4d89d84"),
		new("item", "preindustrial_food_cabbage_and_leek_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc13266508daca3569c630b3e609cc9042d4e30b2413d532bb5bb29119f9b52c"),
		new("item", "preindustrial_food_cabbage_root_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fed026040fabae2229b3c286437fc6969ff064c79a6cbf46f072612d70c562b6"),
		new("item", "preindustrial_food_cabbage_with_mustard_seed", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "93b30a01a4facdbfb600ac4382d06de0725ac9da46474388ef7c355645a438a0"),
		new("item", "preindustrial_food_cacao_maize_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "14ec544e0ee96d9193733bfb4d151c055b395db071e9cc541c340fbbe432fb7d"),
		new("item", "preindustrial_food_catalogue_liquid_amphora", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67f8637c8c8731d4a06271f5f0a2af733a3ad32f5f69bac23aa7cd5e70cf305b"),
		new("item", "preindustrial_food_cheese_lace_cracker", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df4cb5c9b6ddfd75b4f7869ba5b8237efabcab28fd8102964564912c21f30b74"),
		new("item", "preindustrial_food_chickpea_and_garlic_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8dc9a418eeca50f092506a43a3003a749e2995b6ccd23b4208f57602ff2177a4"),
		new("item", "preindustrial_food_chickpeas_with_onions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01cb61bfe319d0498333bfe0578d08abd1a61af1086281ed812eebc8a8b383cc"),
		new("item", "preindustrial_food_chili_maize_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "80c6b23345e1a694955367c9f34251694c5d58816e917d95767a56e90a4ff555"),
		new("item", "preindustrial_food_churning_cream", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e37738107dcb62d5754e8eb1f3059761ad15133eb8e14ed7e18b39da9c342388"),
		new("item", "preindustrial_food_clay_oven_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d136366a940fe635ac18d888be989a324b67859884e98d3275a1e10995bec9a"),
		new("item", "preindustrial_food_cream_enriched_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f09050176fcb7fcd836607c1c5a83d148cea8f380235b24ae5078d359e81dd7"),
		new("item", "preindustrial_food_cream_scrambled_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3e32730eae8bd5e3089fee805e8bd25f8963bd9fac36bd9b5683991938a0f06"),
		new("item", "preindustrial_food_crisp_wheat_rusk", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2091beac25b563d4239bf7ccb70572c9cb67d78baf45e73b576238019957f318"),
		new("item", "preindustrial_food_crustacean_cakes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "45f3b169d1fd97b4e67f16308af65969123d4f8bdc4d2d3722d490a69c77a0da"),
		new("item", "preindustrial_food_cured_meat_rind", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d0095fef0183dd1bb0909f032dc7279480062b5a87d43716ae582e0a5329d38"),
		new("item", "preindustrial_food_dark_rye_rusk", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cb8f1df81ff32e98a7d3d31dfd80b345522e154f613b0b5bc0dbf47be31e9df"),
	];
}
