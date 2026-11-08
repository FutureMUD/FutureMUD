// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch031() =>
	[
		new("item", "earlymodern_door_mrd_grille_258", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ebf9b84ddbfd649aa45190ef28fef1cbfabd45f6fa8e03314ba761cf9e76a97"),
		new("item", "earlymodern_door_msea_secure_door_403", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "942c5e17072518b8092c6c227436ced7deb25c1f4b34ba4e9b0ac44fc5c0c6f9"),
		new("item", "earlymodern_door_mug_lattice_gate_235", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7059dfd8a246c304e190d3f37a80a56ace2b0f59f38e631d8fba32b1071f2d21"),
		new("item", "earlymodern_door_mug_master_warded_lock_246", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9a5c6b0602400e2e6c5dffbcb45412011f152ba602816767cffcd2f616a08ad"),
		new("item", "earlymodern_door_ott_courtyard_gate_179", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b217ddf6c424fa0a38446348dc15525d329ba215d7c46085a51326c8284d4f82"),
		new("item", "earlymodern_door_ott_lockable_door_174", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "be2795db136d7fb281b97341b66c64f7542e512e9334c3d339d6725fa9d72ecf"),
		new("item", "earlymodern_door_plh_warded_key_152", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "41321852f1887df558cc91a5dc853a9f411be6fea1f2ba6aa9ac6034c3420846"),
		new("item", "earlymodern_door_ryu_master_warded_lock_379", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "51568b3586a8f3e81689837622576c12a6777a339537031b60eadd152085f134"),
		new("item", "earlymodern_door_sahel_hinge_pair_488", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ef61aeb1387f50cb0183661eb270ea3d46f2e72c617e58e0387e8d09738ebec"),
		new("item", "earlymodern_door_sic_master_warded_lock_284", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "32ea873a1419baf2d6b89a69d11eb853df9614349f90d1a334f865b435e8e6b7"),
		new("item", "earlymodern_door_spa_grille_543", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c0730d1017b293a746499d5be8da95c47d59047f6f7cf05f23f6ee8b699870d"),
		new("item", "earlymodern_door_swa_lockable_door_516", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2068966fe6563ee19ac17d292a47a5c4ee8405df916ff60d3064504e9bd223b4"),
		new("item", "earlymodern_door_swa_warehouse_gate_522", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cd23c92a0461da169cc10255e4a995dd21e6ca5273eba5fa911a97809e0f954"),
		new("item", "earlymodern_edo_clothing_narrow_woven_obi", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d256ddc11c6b48cafdb1f2c662be7fddbfaa7ed33ca5a5f458ce9d3b02b328a6"),
		new("item", "earlymodern_edo_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af07faa4017aad30bdbfab65bc8f2008bcbb93681c2a12d41c0f22debb6cc3aa"),
		new("item", "earlymodern_food_brazilian_enslaved_bean_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a3b449a1c6502a6c359d0216ee944c14750b246e00b8343a8dc752fc3d36ea6"),
		new("item", "earlymodern_food_british_syllabub_cream", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "733ce5ed6e49bd9c980bb6c7e9206656d9ee3ef17f174a6d6d7e7ad2e3774058"),
		new("item", "earlymodern_food_dutch_shipboard_cheese_ration", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6ee7fa7de489353fcd5b0b7f7eb391056563d6ab94c08492d369bfe6186b90b"),
		new("item", "earlymodern_food_german_potato_pancakes", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6765049df71eb33e2af59c42045d84baef447fd72dfe1da62a3fc3354fe3239"),
		new("item", "earlymodern_food_iberian_cod_fritter_plate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7b9f1145134bc9d7b1ef74a328ec497aee7865816ce00fa6de7dac7d1bed76c"),
		new("item", "earlymodern_food_iberian_new_world_tomato_fry", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0504b97af448dfeb6d057a4a9d1b277bb8413d3217054b9d2af0682eeb7c995e"),
		new("item", "earlymodern_food_mediterranean_maize_cheese_porridge", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "de3b9b5ffe7e88936e670fb1868269d577590fbcc2dc122f26fc17d04c592aaa"),
		new("item", "earlymodern_food_mesoamerican_chocolate_pan_dulce", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eec1c85fb6bb077bdd43bc114cf49c38d0de114c39ae84c4551df2aa305adebb"),
		new("item", "earlymodern_food_mughal_jalebi_yogurt_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0caae83ce81799d3be6c691bb701f78c2a67c7d3d54bc0d7035f19591c4436a7"),
		new("item", "earlymodern_food_plantation_molasses_ship_biscuit", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4ce1fcd62908f314232b4b8dead2e6724449b05cd10f34783b06fd6b91ed3b5"),
		new("item", "earlymodern_food_qing_candied_sweet_potato", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f62058dbb7b141c484e2349094c38cf5293902f5ad2e3fad4667c00a3a49ebe5"),
		new("item", "earlymodern_food_slave_ship_coarse_bean_ration", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa112cd1da270c620d4872c1a10d4d427a275f7eebf28404e82fd5f54760b3ba"),
		new("item", "earlymodern_food_tea_cake_proving_dough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5fb1d0f56f8d7ead02713ac9da056f3b772e329e64e88469037ea07a5584f45"),
		new("item", "earlymodern_footwear_heeled_riding_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e63e4fcd91b489205b11b9e05032d5b2b1e6fb73df617adb7562e87f4f290adc"),
		new("item", "earlymodern_footwear_qing_felt_winterboots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a68ed914f3511bba4e54e67dcf01f56c9a0e424030b2ae29f3c710bdd9082d80"),
		new("item", "earlymodern_french_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b137803a0e93be9c06563630b961908a9e6d89081181d423daf4e41832f35098"),
		new("item", "earlymodern_furniture_and_drawer_chest_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa80b8a37078a68a90751eeb5100f6800b0bcbf5d94b5a0c48639b0f6b71d7a2"),
		new("item", "earlymodern_furniture_cna_counting_desk_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc186eb045b6e08f9f376590131518999290e351d8aebc141c4927fc943f58d8"),
		new("item", "earlymodern_furniture_dut_armoire_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "47ca611009d2648ecac0bf8c6d7c9447cb97e068721629bb460a5d99a615ba33"),
		new("item", "earlymodern_furniture_dut_dining_table_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "25a3e2ecbb93073d6f558ed834df2c52c09f9a518d8148744185c3d36a00a0e1"),
		new("item", "earlymodern_furniture_eng_display_cabinet_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c04575a9333d305670deb1b39438451d287db9eb97c2b1ce23b38ad94e15b2c"),
		new("item", "earlymodern_furniture_frb_display_cabinet_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "06b43404a91f2759ca568ad05f474485e12b464a29f4fba2843196702c9b9c73"),
		new("item", "earlymodern_furniture_ger_weapon_rack_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6a8c64642eee08372048c37eac995b728c9a75c6c958037cc9d18fdfbbef091"),
		new("item", "earlymodern_furniture_gmt_armour_stand_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e683bb3149c024426f6f86a3c7a1c37ad56dffb1e63ee499e97a60e0527c040c"),
		new("item", "earlymodern_furniture_gmt_cupboard_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a642eccbb3de0eef51ae8a84bf148da3fb65c5aab6c739504038d116dac1c57"),
	];
}
