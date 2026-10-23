// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("craft", "Writing and Administration/make temple palm-leaf bundle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_temple_palm_leaf_bundle"], ItemSeederOwnershipPolicy.StockAggregate, "e8fcc8389d716866e756e0f8b02c6b7e31a44d2d0a186818431fd4942d0c0744"),
		new("craft", "Writing and administration/bind hard leather courier document pouch", "crafts", ["renaissance"], ["item:renaissance_personal_steppe_caravan_document_pouch"], ItemSeederOwnershipPolicy.StockAggregate, "230cbea6faa08d34cecaa089b937bd51e4c4c34c9dc0d38d71290ea6b61efbbc"),
		new("craft", "Writing and administration/bind openwork bamboo writing table", "crafts", ["renaissance"], ["item:renaissance_furniture_sem_writing_table_01"], ItemSeederOwnershipPolicy.StockAggregate, "9b78a703f623b3285cc5f7fa8950044e3be340dbb83ccab40c19aa4b23e47bbe"),
		new("craft", "Writing and administration/bind tall black-lacquered bamboo writing table", "crafts", ["renaissance"], ["item:renaissance_furniture_jpn_writing_table_02"], ItemSeederOwnershipPolicy.StockAggregate, "07fa7736a15d7d7982f33376d98bd6260037c95f6ce380baae4c1869b5afd903"),
		new("craft", "Writing and administration/bind woven palm-fibre document satchel", "crafts", ["renaissance"], ["item:renaissance_personal_southeast_asian_port_document_satchel"], ItemSeederOwnershipPolicy.StockAggregate, "ba2917ad0e9a92991857be5f3e713d639dc745e37602130feb23f2878d4ed0be"),
		new("item", "adjacent_antiquity_fine_pointed_leather_shoes", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f51c0f7307370510c0399d0e3aae9222eef436c8df5a1d9181b1d68d6febe326"),
		new("item", "adjacent_antiquity_fringed_linen_robe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7b026d62120f4ff0053e74e1a72b4bff0ffd08bd8ce1400fad38839485fab70"),
		new("item", "antiquity_accessory_common_small_knife_sheath", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9aa9225a1758b3c47d4fee22b4ebfb487cd8b1c51fb9a8e6b305caf9184ec32f"),
		new("item", "antiquity_blood_offering_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed4f5e789c9ff83e60ba45a8d4f0cd675d0167a94b698247b470f1556d03e85a"),
		new("item", "antiquity_broad_linen_storage_bag", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1cc5bd12cae2e1d2b248933213cbbffc33835430941bd952608f00de524b41b7"),
		new("item", "antiquity_bronze_edge_beveller", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b5a3ae32e8afefcc393307ae9d03399b0358a46899a69faf8bf2eadd1c67f51"),
		new("item", "antiquity_bronze_leather_paring_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1bc60fb832842d5fbe2a6dc0ab4084ac39f105a0eb84b6fa78c081207d906d25"),
		new("item", "antiquity_clay_smelting_furnace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f7293caf7f2fdf4050da7881a3cfe37f8b4693b5f2c482ddc3ad1c622c34737"),
		new("item", "antiquity_cypress_capsa_scroll_box", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a755e57da8dc25cf28f031b9445e28affbfe201bbcae667bca71a058290d009a"),
		new("item", "antiquity_egyptian_blue_war_crown_cap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdeb24ddfa56aa648ab3cabca1f206b733f96e0edbd1b9c088ac0ae2d001cf84"),
		new("item", "antiquity_fine_linen_headcloth", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e80b6787abebd34ce8f9da239d0a7248c531885b2e884164d0d13222a0a5f01"),
		new("item", "antiquity_fine_long_linen_chiton", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "57fa3f4824fd2802223ab478ae1796070af9d7fe62c3f8b4057b3e94f02e839a"),
		new("item", "antiquity_food_egyptian_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad3cff9ea145f4ce23e84cc662eb4749bf45b0f73a61a627a04917f35e3d1e47"),
		new("item", "antiquity_food_germanic_honeyed_pastry", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebc9ea6c82a60d56ce94abe5215f0810f56a570867f65e83c4a2fd0e2a5edac6"),
		new("item", "antiquity_food_germanic_oilseed_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "720bda4abc49a8f2013b8e4232ea12b23fa214cf21a34c24dcd4e891fc1b7bde"),
		new("item", "antiquity_food_germanic_sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "42b41e5edf269413a69c5c5a0734217e8dbc1b56101a5960b9b21ee9b571180a"),
		new("item", "antiquity_food_hellenic_fish_sauce_relish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bac7089786459c3db933a5740a22686c6ad201370959fe983a2b6cdd2f4a796"),
		new("item", "antiquity_germanic_iron_banded_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1271482ac45bd30c1be992b70c5ad9bc8b9ad4aa5dcecdcdb835020660c187e"),
		new("item", "antiquity_glass_display_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "65a7f4e9801317d4399aa618feddae62d5374cf21831a579639177fd84567dbd"),
		new("item", "antiquity_heavy_wool_cloak", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "862e93815b04ca6470f3e2b75df94ae004b926b962924d4912f66564b765456b"),
		new("item", "antiquity_hide_sleeping_roll", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a4cea23e82d095afd5fe3b9c1c68ac2ccb6b5c1ef55f65c16166abf0562f869"),
		new("item", "antiquity_ivory_loaded_die", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c27f509ca83a392dc3146e4177afcb964b08cd48b7bce04258f3ecfd642f68bf"),
		new("item", "antiquity_kushite_leather_kilt_guard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e3f5f0020bdf7e3a0885c8b103a564c61c957ee7b6e5d39b8582e77fe6aad5c"),
		new("item", "antiquity_large_storage_cupboard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b3c045d98a2132d53abc84e52c55069bdb05ac544f5f3a79c536586095d7173"),
		new("item", "antiquity_leather_tent_door_flap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "85dd1a02d827ede62b46e568dae9596d3f3d6e14aaa020aac3ae43debd8414a9"),
		new("item", "antiquity_liquid_cedar_stoppered_merchant_jar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "38b413bb0bcbd6deb1f0d093e2587bed9e89f689e7bdca8f25e845f02c27fdea"),
		new("item", "antiquity_liquid_oinochoe_wine_jug", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5df8a8a85dd00ac9bb5f6dd2207b459c5cda0a00ac020afdbb3c7df6097bb537"),
		new("item", "antiquity_liquid_painted_kushite_libation_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d9abfad5c196bad43cb63e38c1fb28a168ceb1d70064f0241c1268dd312695d"),
		new("item", "antiquity_liquid_slender_alabastron", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a44ea37909ea7335ddb23760eecbeae70d87f4f3f28c835329888c9c39ded73e"),
		new("item", "antiquity_liquid_terracotta_water_jar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a4079ba2e9f76b257eca695e19f017215eef6c9a7a6f52c5342a7a43f9a2ff7"),
		new("item", "antiquity_liquid_tooled_leather_flask", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3ff6c9113c7e7521b387f0b5578d7818b0ba6a2fd34eca9fec4769372e0cc82"),
		new("item", "antiquity_lit_workshop_hearth", "antiquity", ["antiquity"], ["item:antiquity_workshop_hearth"], ItemSeederOwnershipPolicy.RequiredRelationship, "1fa7160042e8eeab026e867d36b6fbbce5dcf71e7ff91012007ae7b1ef4b722e"),
		new("item", "antiquity_mandrake_draught_vial", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0b80ccfaa93379358ffe427374ab1e21fc1f09d6231e522080284387aa8760e"),
		new("item", "antiquity_market_canvas_stall", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "569a226daf891739060c9a938044be73b1509a2016c6a5568b90f3987e33f6a6"),
		new("item", "antiquity_oak_stave_dry_cask", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2f699383e5842addb2d960411a9bf9a5a89a04d78a884b74db421030f406dbc"),
	];
}
