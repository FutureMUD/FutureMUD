// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch053() =>
	[
		new("item", "medieval_writing_reed_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "97041965f9bc98ae278ef0602e3d6d25e1010c63f09bf4a21c6e84896847627e"),
		new("item", "medieval_writing_sealable_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8df0b6e270df506b1b2059ae16045c31b5301f3ddba61198df3e3fc0dcfa65c"),
		new("item", "medieval_writing_sealed_parchment_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebf7f0a86d335df2db95225f46e6484ecaa9908aa1a062a3af2972b27bf20033"),
		new("item", "medieval_writing_sewing_support_cords", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "131794a5a3175172710ae5de51888d9cb5200505542b013c1704c3b5d177be6b"),
		new("item", "medieval_writing_silk_scroll_wrapper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecc96e4b71f4d554b8778e7f283d66fd3951bf8c1862cd74108ae71dd4e238be"),
		new("item", "medieval_writing_standing_copy_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "57bfa6b59aacb008a987b5d8733f599b4ca9e54a1db22554a59a849d95692c25"),
		new("item", "medieval_writing_sutra_printing_block", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2abd28b09bc14985a251fc0837e98ceb7fada323a41bd4340570f35471693397"),
		new("item", "medieval_writing_woodblock_ink_dauber", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a9e32703b6081c82bc2e606988072dc316e744e71721e64ab3c4a53153020fa"),
		new("item", "medieval_writing_woodblock_proof_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "871ced866525b573c4148f7596cb62a94e84862d86fe2b837d2dcaf76c6d2e53"),
		new("item", "preindustrial_clothing_simple_woven_sash", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "562ccaad3753b0094492e37be9be88bcfabf9a9df5466a3212b1646fccb2a8b6"),
		new("item", "preindustrial_door_barrier_40", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9316152a243382b592811ac29c12d1a894dc6078014649b91d85b6f4fa8d2505"),
		new("item", "preindustrial_door_lattice_gate_07", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "683eaacd42ff648a5d51a0c14a32d3347e1796fc236372b531a50545d62b6077"),
		new("item", "preindustrial_door_lockable_stable_door", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1dd04182dcf33a427e78df3e9d2bab6adf9b930b534fe9d76d900d8d95581ac"),
		new("item", "preindustrial_door_master_warded_lock_78", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "90c8184ed1d806cbf902bbb5006156d9928520971c44518001ba443a827c0792"),
		new("item", "preindustrial_firearms_musket_wadding_packet", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6196b43c30d7c75e59ca971f143944c38ac183c6e9a981192409f78873d6465d"),
		new("item", "preindustrial_firearms_powder_flask", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d10f48a4d3a124b33232987068268b6aece72c2138ac1ee7f6cbbe9f04e5070"),
		new("item", "preindustrial_firearms_powder_horn", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cc07d6a9d9bec2d570170be14b2b280dd1a46b25e09f1562f9fee80b1799280"),
		new("item", "preindustrial_food_apple_oat_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "73f98e8591fdfd376a0a4667410785a7b80b67e3ee3f736614da749b70612969"),
		new("item", "preindustrial_food_asparagus_with_mushrooms", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef7f122ba90a7268637a67ddddb263d61ba7cafa12baa837e7546b7121d96453"),
		new("item", "preindustrial_food_barley_and_mushroom_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d519610208b74ce04b8fbdd27a6af02b37dfdf859807bf9f2e8f101bba112ac"),
		new("item", "preindustrial_food_barley_batter_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee04bfae2066eb95db44355103b909717dc78d429c122ccb336cbfc9654d4816"),
		new("item", "preindustrial_food_barley_crusted_fried_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "15b7c4ec6d3a4e0015d87ec81019426791d2f8ccb91dcc4892f47a475def30d9"),
		new("item", "preindustrial_food_bean_filled_maize_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a820520749317c510eec997a983b6a1a036193b0dc32ccb4be9c16d99cefa83"),
		new("item", "preindustrial_food_beetroot_and_onion_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7f2f34a989dd84f819314b8f042627f84d180e0ceb7581dd7cbbfb8d0b15c55"),
		new("item", "preindustrial_food_beetroot_onion_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ef77fe0a986da0b7afdeac211b584c1e0a1941ba4a22858c6a5aab6fc4379d1"),
		new("item", "preindustrial_food_berry_honey_ration_bar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "58d4eeaf7eb1e4f19ff9b4ac390f00f9376b7b057fb148cbf5e030d2f2bf2187"),
		new("item", "preindustrial_food_berry_meat_travel_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2eb99912a66816ae652ec65f6b7ec259f9736da96293c7dd6bfa9630c0831a54"),
		new("item", "preindustrial_food_bitter_fisher_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dec1501a5422c3fe28e6765fc9bc44f7925ed4ea8fde4618f96f4d1a8821a411"),
		new("item", "preindustrial_food_black_bean_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7dc8de431b503be467c15c24f32924dcea9d0a505cd15faefd70cf91c112b58"),
		new("item", "preindustrial_food_boiled_bone_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9915b68798ea17a2ff368e2947b5e75eea7701076f36cef145b5e79fcf50f3a9"),
		new("item", "preindustrial_food_boiled_grain_with_mushrooms", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5c651da5435c28a81435004e0e787c82cadb09b16029eaa9760640ccfabc66a"),
		new("item", "preindustrial_food_boiled_liver", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "415ce24dd1019d10bf939b6cb26b77167fe2de0d74da57fec7863f60300d7ac0"),
		new("item", "preindustrial_food_boiled_rabbit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e54f9e4d0edf899a07c597dcb3a5e5a302e359b3cc845cdcf2b7050006d0de9f"),
		new("item", "preindustrial_food_boiled_wheat_berries", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "38755fef27418a7c479e7e8b79d1824e7087b33fe423be439602f48011bf076b"),
		new("item", "preindustrial_food_brain_fritters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "98918e13a4a1d64abcda2e8731518c5cafd06822ff500db8c8e968485fe9c7d5"),
		new("item", "preindustrial_food_bread_bound_meat_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1ba2f9d5f5080bfd823ea25d56c9372e0076a5ddfa294b5f307cae372ab8fd3"),
		new("item", "preindustrial_food_brined_white_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "64314fa33cd86db3889e145832546135aaca9b6efc9ca2bc9e8c5e1d424b900f"),
		new("item", "preindustrial_food_butter_basted_fish_fillet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9de84280ce8d58b3f5d32394df219bd35a1e6734646e026f77ec9e1a1487fa4e"),
		new("item", "preindustrial_food_buttered_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea8332e9153800b4cf50afe119ff3647d8767fb2d81a0f2efca16cd509206df0"),
		new("item", "preindustrial_food_buttered_shellfish_morsels", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a33f947c096d0b60f95e193d091a0473f5494edf3ad709cb353b48314c125505"),
	];
}
