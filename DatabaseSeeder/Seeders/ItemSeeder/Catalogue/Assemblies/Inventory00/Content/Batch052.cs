// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch052() =>
	[
		new("item", "medieval_writing_labeling_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b171d6475cdf2af06f92406487e627d49c3f30fb19df741a53c6a538f3ace500"),
		new("item", "medieval_writing_locking_document_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e3e671105c7e3f51ea5fe563302411cdbcc0c314ce17ca0c09c1a0d30d0a4e9"),
		new("item", "medieval_writing_potsherd_ostracon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c07a3c913aee696b3ac3ae9941be588018bb6c4164741ffd4538377eb7e0626a"),
		new("item", "medieval_writing_quill_curing_sand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d950cfb0fb6c21a513b810113b91e3670f1f13bfe95cffadbe19ac6361f985aa"),
		new("item", "medieval_writing_rag_paper_petition_leaf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "49d9eea0656e8bfb044a9f5393484731b9ec1e48d616d863a965914db3556688"),
		new("item", "medieval_writing_school_tablet_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc4777b1aa19509794b6b775cda1c3da77b06e0c2dbf5b704de004e4912e5fc4"),
		new("item", "medieval_writing_scribe_tool_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa769a7c94e5aa070426d4b0ce0abc9fe4d57c06d86cdeed35b983a481b5a045"),
		new("item", "medieval_writing_small_wax_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "135de05026e851241f27dfa9032e34ce651706137af8dc897b6d233a70960ff4"),
		new("item", "medieval_writing_wax_tablet_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa20fbd1f6eb05d7cc5abff4fddac48dbdfc1e0aa760e8b8a6771ac511508ec5"),
		new("item", "medieval_writing_wooden_account_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8598673f14e6e4db0e8cfe975184324ddd1727ff4950877b946d151eb4e09c96"),
		new("item", "preindustrial_door_bone_bead_doorway_hanging", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "220d6b2a524bb7716e48c398e984efefdd00fc668c6f58bcb5d54b8b21adf1e7"),
		new("item", "preindustrial_door_courtyard_gate_22", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e4c641e8ce9b38718833ec7a3a739a2eac1d01a728c8a53d7daec43e33f857b"),
		new("item", "preindustrial_door_escutcheon_75", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3b3bfae6a933ed09e1128d2a461caf9a0e1bdd632bf76cbac87b9164d358e37"),
		new("item", "preindustrial_door_garden_gate_24", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e1823b7cf2ef9bc4456e6cdfa7c7a53fb93423ba1ed527c1ba0143a12019b4e"),
		new("item", "preindustrial_door_grille_53", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0eb786292b01440aae50e74c74ea2a9c897346c51a65d324c416f56dea1e9f8"),
		new("item", "preindustrial_door_hook_latch_56", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a793f4b32ca4a41ee0db3e785ad80078f07d170e7c13d0909bf0cced92d42fc"),
		new("item", "preindustrial_door_lockable_door_03", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1b83dad769db1594f37517245a4310c89d3c24390ea2cbb898bc16b42fa8d8a"),
		new("item", "preindustrial_door_lockable_warehouse_door", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8d293149dec21dc1b607d0428bf25ba415df0ec0751808485554dd7206a0a88"),
		new("item", "preindustrial_door_master_warded_lock_63", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "efd6746139c7129b40bc383155d6ebe75406256fd05bbcb3b85a3970657a4d72"),
		new("item", "preindustrial_door_warded_key_79", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66d6e0ac7d837a6a36deb51906ffa935a8eefaba5f55b9427e35b66da298ea76"),
		new("item", "preindustrial_firearms_ramrod", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ec0a3d26149c33316942bcd8efae578668f1c7d6ff7f570c27af5546c88cbe9"),
		new("item", "preindustrial_food_ash_buried_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f85c4f936f0a570405ac38e9c76db7758f8e2874eafebde76dcc46a5e9e840ba"),
		new("item", "preindustrial_food_baked_fresh_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb9441e740575a66fa9506a8a0394b18aeff90442d301afe6e52a8882716bbed"),
		new("item", "preindustrial_food_baked_noodle_dish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a2a3a848acd5429814aa77840a47957f1015cf23e24eb59265e0cc98b98d1d1"),
		new("item", "preindustrial_food_barley_campaign_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d09cda0fe160e68cf9abad6c492ed3ed07a1bc760080cb1f62384b39c5eaa45"),
		new("item", "preindustrial_food_barley_salt_cracker", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68912104b98b76aa2c78f27ed8e0c2db946af4a8cada15ba5fde79d3ecd17e07"),
		new("item", "preindustrial_food_bean_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9fbb68dabf2dbf3c041f1fa1b48ee04a19abb0126339d8a26c502f61d9e05bfd"),
		new("item", "preindustrial_food_bean_meat_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "17597dfc91519bd4dd151a0440fee7b3bf5098444ef157f76e3c726760ffb119"),
		new("item", "preindustrial_food_beetroot_and_cabbage_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f075221be4c3d1b31554536b1235fd5fedfc34038f68fd14c8d3bdfcc450e9f6"),
		new("item", "preindustrial_food_blue_maize_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "25ca22a668d7e9d228f5be4e59c63e85e52cfcacda13a0c4cb8edc0e0bf065dd"),
		new("item", "preindustrial_food_boiled_cowpeas", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "47196acf644f831965bc284bd2bdd1296b650ac56f453307b3f54d26f5df1774"),
		new("item", "preindustrial_food_boiled_onions_with_herbs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eab8fdd7286d189a1810f1b84617100a84ac25dca43b52ac3aabb82479725d1e"),
		new("item", "preindustrial_food_boiled_whelk_bowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5d1b128ae76ddb2bffa9cc67246b69d8e7d5ac86539d3a9683931935502046b"),
		new("item", "preindustrial_food_boiling_sugar_mass", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "28353d2d4390b920dd2024f361a62e7afb1994a4aca852a6075cd592e1e1fa5a"),
		new("item", "preindustrial_food_bread_stuffing_mix", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c75849beb15e6be6098c36cfdc3f0e3051ee9cf75b0516f6773db58d674bf006"),
		new("item", "preindustrial_food_broad_bean_and_turnip_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed5f17d9b97a97958dce6c66e2268fe5cb46bc0312938b4ae82161cdcdc3b967"),
		new("item", "preindustrial_food_cabbage_and_onion_hash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81a263a0eb42c5936dc7f27f7741b4e7d52e1ca9b3bd0c5e8b01874f38d1cb88"),
		new("item", "preindustrial_food_cabbage_garlic_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d0cca8dffd0396521813747436202d0f342dd3d6dd664ef1c25e39efbc2f2f9"),
		new("item", "preindustrial_food_carrot_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0a5f3af6975634caa922f2f8a5f47362af88a88712c2c9e67622de0a5cda4ed"),
		new("item", "preindustrial_food_cheese_curd_portion", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e219513d9f80bdca8b9d4f238f1b1abdd91e42db200f979ba1753a02c9b598b8"),
	];
}
