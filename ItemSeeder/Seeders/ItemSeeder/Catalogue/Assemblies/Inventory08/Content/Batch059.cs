// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch059() =>
	[
		new("item", "renaissance_door_eal_warehouse_gate_261", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8eb02d531940223dc246bdded24f90ff669a53279ad5937b4e7cdfe6128f0feb"),
		new("item", "renaissance_door_eon_master_warded_lock_158", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3c0c8f68d8a774c899a67ad18f4e3e0a5d0c128bfc7882d7cf96aad5cd59edf"),
		new("item", "renaissance_door_iba_lattice_gate_35", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61311501e17dfa23b9b1be0c8b504e0828d5e1e32150f5575961130f2277a463"),
		new("item", "renaissance_door_iba_panel_door_29", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3ec4eec833cfb0e7dae10eb4f9065cd8b7015a24d28d3b883edf73404da2388"),
		new("item", "renaissance_door_iba_warded_key_47", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "11520e930eae915b17e42e6b815d4009fd46aa32280ecbb190b7809e72be6cb7"),
		new("item", "renaissance_door_ind_lattice_gate_511", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af1b67f3afdab946ef69439579d664bb4d2fca0c26a968428e13d12373d793e5"),
		new("item", "renaissance_door_jpn_door_bar_303", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "23bee4e7d00b50cca207c9a25297d74a0e848f4dfb36bb3dd7eda672cda7c53a"),
		new("item", "renaissance_door_jpn_panel_door_281", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "959cf1235dce5fb4c7bce53f42b3019f80dd7609f0fcbac6a36389c396a7ff3a"),
		new("item", "renaissance_door_jpn_secure_door_284", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "728221ba0b0160fc9b9d88a5f9b9ab3a286785b615c294ac2fe0d9a35af8b4a6"),
		new("item", "renaissance_door_mea_courtyard_gate_316", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ab0a695aa5198f2460bd1ffe1e6b9a370620a24c05ced3a1fbe89f6545db4d8"),
		new("item", "renaissance_door_mes_lockable_door_535", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "518dfff291ad4063feb7b37c75f8d081aeb4bb8504a46e373b4d34ed6374a51b"),
		new("item", "renaissance_door_msea_warehouse_gate_373", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3375d5cff66d2587fc39b9f21068e2bcc1d4d4bf45732b0402854ccab2323d0"),
		new("item", "renaissance_door_nba_master_warded_lock_102", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0255d4ae3e4f53ee3c402c3e8b6aca34d3bebfdf05b7afce940c8c84be0e4b6"),
		new("item", "renaissance_door_nba_warehouse_gate_93", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c907541c539b08ad0583f9b6fd67c63e7225d5bbea7cda358991ae81345b00a"),
		new("item", "renaissance_door_ott_door_latch_189", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a3a7517660e469483104a3f8d0f7576883c19d119bd105cfccb7cf600c3f6d2"),
		new("item", "renaissance_door_ott_panel_door_169", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9aac781cf50120dadc620b04da5c1b65e0479b9f2eb2722b99a4b1a82cf45ec0"),
		new("item", "renaissance_door_pip_barrier_208", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb84e38e048b4123bcbcbd526acc5925cb4402b28ca88c103427088a5aa163f6"),
		new("item", "renaissance_door_pip_master_warded_lock_214", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f3e0d9f39d06ecca24ccf58b9a2242ee26a83b7bc9fe7d15f873fff0099c9ae"),
		new("item", "renaissance_door_rse_door_bar_499", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a60ce25d8975e17c619014ed30169e1ddcecda5d9ac3a24baf0f042a72ba363d"),
		new("item", "renaissance_door_rse_hasp_501", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "00ac84d33ba23106110d297824316eb924ae9600eae7259efeb81c8c6c3876f6"),
		new("item", "renaissance_door_rse_warehouse_gate_485", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f55ea8d05522f785e4de02a6650281b2c9a9000f7082ce036294db3d894c52f"),
		new("item", "renaissance_door_sas_door_bar_247", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b3f6de777d991589922f56072def7ac80c6cba20e49c1b97c75ed82be14efcb"),
		new("item", "renaissance_door_sas_grille_235", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2301906c130f53cb83ead8fe3f67d97c5c8dfc9f1ef7b04b9ae02a3781e09087"),
		new("item", "renaissance_door_sas_hook_latch_250", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5c62218eb680e2a0a7283f661a25e31ed6eacb233506d3b959f5da194d2b1dd"),
		new("item", "renaissance_door_sas_lockable_door_227", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b7380c894818986677f2707a273500a29225596b1c0932d5812a94b1d28f526"),
		new("item", "renaissance_door_sas_lockable_door_241", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bdd9dee4073d8f9568c1082f8c98da44a9b8cbd538b1824b09ebaf5062e863f"),
		new("item", "renaissance_door_sas_panel_door_239", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a390541d624513b18f38aa83850e83f7792cad99e593de14deef881c13ab64b4"),
		new("item", "renaissance_door_sem_hinge_pair_364", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4479f22cdff4a748ec7ec76c12745d404772069d6ae8e32071e8a7dc529f9418"),
		new("item", "renaissance_door_sem_hook_latch_362", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "813b534c6dd8453a67730b9d4d576f4f474a844693d849144de578bc8aa95b6f"),
		new("item", "renaissance_door_sem_screen_342", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c133dc67799010850a8976508dc5a530bbb228253530380bfa65c1c5a667c2fe"),
		new("item", "renaissance_door_sem_secure_door_340", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7788aba55ec0a3289cddad7dba6488e118b9ea90f8354e801e61fc50a63f942b"),
		new("item", "renaissance_door_sem_strike_plate_363", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa722542b584eaa16de2bbeecb4e1f78db1ec40c17d0340f3b611e8d1303b94d"),
		new("item", "renaissance_door_stp_hatch_405", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d81ea2f115033650176fe6262a00c328bee229a35c3a2ca7dba95397194065b"),
		new("item", "renaissance_door_stp_warded_key_411", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3c8816c4f83337c10184fec6b588c8369518c250e412d7ec4d99e093f115de0"),
		new("item", "renaissance_door_wer_garden_gate_10", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1234ef18d9dc550bccd259e0e8bbc2cb81dbe6ac4db253d1b4f3a64aaf4ec954"),
		new("item", "renaissance_drug_myrrh_tincture_vial", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0ecacd8f94717db4413ecef2d4f1daf97912ef766a90d8f925f8fa9db005340"),
		new("item", "renaissance_food_andean_honeyed_maize_offering", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e25eafe7e9711db957176e73b317836cad659014a8c066979856bbb2a4c301d"),
		new("item", "renaissance_food_english_cream_trifle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d9f3385fa9eb8da9b94333bf362d522788eddf6e93f4d8a706239d9c1d5498d"),
		new("item", "renaissance_food_german_marzipan_guild_figure", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "73502ae7cd1cd997cfe571242d25df2287d218b599b356dce322f8da7a130a05"),
		new("item", "renaissance_food_italian_fried_liver_croquettes", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "269dc30fa70527702f16c41db8e41045f55a91658b096439748a985e081b3a8c"),
	];
}
