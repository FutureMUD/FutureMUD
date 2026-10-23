// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch059() =>
	[
		new("item", "renaissance_door_sas_master_warded_lock_242", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "24615a91c528cc2f2da958eb1465e6fb9d69ac32ac8a5b92ea0740c2637f49b5"),
		new("item", "renaissance_door_sas_wicket_238", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8749f3a009eb412d79c2f475a4be922184d0e72bded14a99e1f4b1e0db4ef87a"),
		new("item", "renaissance_door_sem_warded_key_355", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "191da56536f47d67e8d6077b285b0c2fd6b5c137c21f28b81556d47e22454b56"),
		new("item", "renaissance_door_stp_braced_door_408", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e90e6b373efb4ad15b32ca2922f3f2c29bf7fd43545feb6f464fc1a8441191b2"),
		new("item", "renaissance_door_stp_hinge_pair_420", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "13256d2d6c79030db1b93910a35ebaf58f4f820efd7d4732ff8a759d986460b2"),
		new("item", "renaissance_door_stp_panel_door_393", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0edf87e4778210a79f435fa6949f2cb8d84207141ea467edf273cd1e0146ac6e"),
		new("item", "renaissance_door_wer_grille_11", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbedee312a34cf1e415229b3939aae3bb76a25b87fdddb60bb667aa3fedb56c3"),
		new("item", "renaissance_door_wer_lockable_door_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "52c51e413ad29ec9725f62d55743a2ce10ae44028bf85770d68b815e6a3ed0dd"),
		new("item", "renaissance_door_wer_warded_key_19", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5d01fb3c53349a474ef31ede9cc80f3bc92ef952e35dba6f9f14659694bc03c"),
		new("item", "renaissance_drug_tobacco_smoke_roll", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6302f2bf7c470b62a7fad49a9b35d6c3c217cdee591423d7a50ab928ff2d0562"),
		new("item", "renaissance_food_andean_chicha_residue_bread", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db5fc1b616b6f0cdec0caaa7562d26c0d52a4230ee88527b10a666ae029534fd"),
		new("item", "renaissance_food_andean_freeze_dried_potato_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ad72633663d1433d8460c85c4e616f62582fe93906116fee2c6a91a75c05079"),
		new("item", "renaissance_food_andean_pepper_stuffed_tuber", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f158bec5689e2d32c37476575d3943e5bd13f1ad1320b6e4a819f93203534bb"),
		new("item", "renaissance_food_aztec_lake_algae_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "240be8292e09fde0844c7aa635901238ba5bc1dcde5d2d5ba72e7493953cb05e"),
		new("item", "renaissance_food_aztec_nixtamal_tortilla_stack", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a186e9351b2f6b7510d7d78355034f7544a23b89260e004424d0bbce6126ac7"),
		new("item", "renaissance_food_baklava_unbaked_layered_tray", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6388702c776926b78f469a648597444b91b735c8e86fff3a4bb61304b390c922"),
		new("item", "renaissance_food_benin_coral_feast_fish", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "941c67588c58a07d9f6b4964bdf27b5a30899b19b5fd57d3cc7a7a2d521005a8"),
		new("item", "renaissance_food_charqui_salted_drying_strips", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "54c4c21f0e5b03e7c08c3a2be642131153efb6111ddb0b1f41af8a0b2a90ed65"),
		new("item", "renaissance_food_chuno_thawing_press_bundle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6ef0ef2eea0a2ddbc65a8f96207887c2eaf403b1e018957eac1e09865fa8ff3"),
		new("item", "renaissance_food_english_sucket_citrus_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "24ed5969b523ee42ea43bb23c0035f61c84e134c0c711f574bc1074fb758ea21"),
		new("item", "renaissance_food_french_cream_tart", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "59b70c0bcadab7e168b8d9952d6e78a151e67e388e1f73584daa224edc7a7349"),
		new("item", "renaissance_food_french_sugared_anise_comfits", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49ae47c1c49c14ceda6f67292ca0330e5df5966ca57a3268f4ea764b38b91db3"),
		new("item", "renaissance_food_french_truffled_like_game_pate", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9249942d2dfc8368c727b7508d87f3920510de2247658bbd5ad8ef447cb982dc"),
		new("item", "renaissance_food_iberian_olla_podrida_feast", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cab934b2a7362adb47c808ca237293d9aed397d3a0ab95c8107d8587ccc9468"),
		new("item", "renaissance_food_iberian_tuna_empanada", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8cfaeb3af443bbc2c4ff03dc735646a753a9476a3c5f54b9dd18a8edc18efdd2"),
		new("item", "renaissance_food_italian_buttered_macaroni", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "129b6c2e6cecdcb4d36c45650b009471e8ceaee7d74e6b32f44a0c009eb5e7e2"),
		new("item", "renaissance_food_italian_fish_jelly_medallions", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e3eab131c12b93402ad28202f6f2748d3eddb2fa4d41248f74d890d428a95a0"),
		new("item", "renaissance_food_italian_greens_tortelli", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c97526c2a2717edc549431aa266ccd8eafeed3d0a1139a39fd224feec6a3cc7c"),
		new("item", "renaissance_food_italian_revested_capon_showpiece", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5cfae3310452ff433e973a73df8f08dc98e0e3fcc5c3033a2e3da29ca988e62b"),
		new("item", "renaissance_food_italian_rice_flour_gnocchi", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc24693e03a134ff1275c37f4fe43751935622017b1fe02a69efbe3ea85bd306"),
		new("item", "renaissance_food_italian_spinach_ricotta_torta", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9d8cef12c18438d1d75151552a7796e3cb703438139b6551767cbd5d9e4e22a"),
		new("item", "renaissance_food_japanese_grilled_miso_rice_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c36bac842c2cfec9e7feb617a2c2abd59eeea5d543df7b60efe2a6264377e1f"),
		new("item", "renaissance_food_ming_sesame_paste_noodles", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75166212b2d2d0939c7d168e65ea7f0a42ae3ba6aef3755fd08e5631f268a272"),
		new("item", "renaissance_food_ming_sweet_bean_pastry", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75331a4eb25df2d31b47915a193de6c1c397cf2bd10b11938dcf4263e45a68c8"),
		new("item", "renaissance_food_ming_tea_house_soup_dumplings", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cf62802b81b8156a8049b0299c8383d80616e9a8a49d7e5dd713db0de284011"),
		new("item", "renaissance_food_mughal_mango_like_fruit_pickle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "44f790f45578dfbb66df0433f68b6066729e9f245c29e19440719586ce6cb13a"),
		new("item", "renaissance_food_mughal_royal_haleem", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db193c755859afb7f2267f816fa011699705971e2bff61fae924098e8e97df4f"),
		new("item", "renaissance_food_mughal_stuffed_sweet_flatbread", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1566b869e2e045c76a2cff6cd596b203b945b64ecdb094e46b48490bf77801a6"),
		new("item", "renaissance_food_ottoman_borek_meat_coils", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8488a6c76fcc9acca4e6dc2e6e5164847bb8626ce19d38ce50fcf201ba5f69c6"),
		new("item", "renaissance_food_ottoman_kadayif_nut_nest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61ee9968cb7402c5c7be6369d85d928fef0ef29cff390960ef6c46ff1b173a98"),
	];
}
