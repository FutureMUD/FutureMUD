// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch061() =>
	[
		new("item", "renaissance_furniture_nba_counting_desk_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aff26806a73715c1c9b412149b647cf3aedbeed16d8a67e04331fef9554e5359"),
		new("item", "renaissance_furniture_ott_lock_fitted_weapon_cabinet_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "69dfa389930c9c0886ad83c0c00a5e9593cbecf991df0e780164661b78112015"),
		new("item", "renaissance_furniture_ott_open_display_shelves_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40341efdc7312238a1bd7405113221b41e3484efb981548f72e2c14f2b605036"),
		new("item", "renaissance_furniture_ott_robe_cabinet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc80c9618075a5ae4b7c0f2a67f178ae7743945b16be3a496bc28262a5137932"),
		new("item", "renaissance_furniture_pip_campaign_armour_tree_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "97a7ad6a1ef866c4245a37b654dc32900ae5fd91f4be63f017159509b0203427"),
		new("item", "renaissance_furniture_pip_open_display_cabinet_05", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc9db24e286d566bf036ab162932b6cf89a804bf3dafdeb07606176c178349cb"),
		new("item", "renaissance_furniture_sahel_redsea_indianocean_manuscript_cupboard", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9474677fc4857ae68a1dc5970bf37f74ba5bd244ab798a6b46c8f267158690b9"),
		new("item", "renaissance_furniture_sahel_redsea_indianocean_water_storage_vat", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "104e89f70f539aee2ee653d282cc09548b9dc78d61732dad4cd331e4f8f627ae"),
		new("item", "renaissance_furniture_sas_low_drawer_chest_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d61f6cabdd72de96a20508efec88c6e5e351e89d92a6462f606888b8852f8f07"),
		new("item", "renaissance_furniture_steppe_caravan_campaign_chest_table", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "656101ad2f771adb760b41bc8f02a56178b9f1871994d37184bece9ec4cf0f52"),
		new("item", "renaissance_furniture_stp_campaign_desk_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e7eac24e21096ca79698d583508227a498bb4230297dc7a41ec7c1932c531e6"),
		new("item", "renaissance_furniture_warehouse_dock_bottle_rack", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f74ee31ba07a1ee8528c81dbc746c04181230dc0ea1962c1105932aef33b34b8"),
		new("item", "renaissance_furniture_wer_campaign_weapon_rack_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b3febd0e01368f9aedffb11f8721827ae983d6b6eecad92f7aa142ef72e624a"),
		new("item", "renaissance_furniture_wer_end_table_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "32c43ad8a652bbd90f702b0ea7182fd0f63675332a728f75a689164423322c80"),
		new("item", "renaissance_furniture_wer_lamp_table_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4d1270046c77ebf4847ef18fd423f2d239744b6c05c180ceedbe972a0fb90ea"),
		new("item", "renaissance_furniture_wer_lock_fitted_display_cabinet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "377fa00d5422a8912dc63afcf807bee228579da9a67eae58860caccdc6427da7"),
		new("item", "renaissance_furniture_wer_tall_wardrobe_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5251b5d2267dd414ae6adea838ace3038b345d166204d5a87fac0c126369a721"),
		new("item", "renaissance_furniture_wer_wall_cupboard_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d28d6d9763e93bf952f3592f1331612a2018e2e8204fb4cd0ad94985eed374e7"),
		new("item", "renaissance_globalmaritime_knotted_headscarf", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b9d569a797430286b76877866d98c6dc332799cb2c28fde1a665c111d64e341"),
		new("item", "renaissance_indianocean_embroidered_roundcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3977a7324353d66d77dcd82ebe3259f1fb7a3c4c640dbf69033e90f62383cf91"),
		new("item", "renaissance_indianocean_short_coastaltunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dac69492c626a30824c9601c010ec4bce0da7d32b848cbac9fce09ec3604f510"),
		new("item", "renaissance_indianocean_stitched_coastal_slippers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1578ab4b9224ac172909f756cb24efd3590e55d7e39a05374e960585f086830"),
		new("item", "renaissance_indopersian_fullskirt_jama", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c12b40ac68b02e0c3a217ccb8155ff0dbf76f82319d9ccb5eb7bf21ca11de68"),
		new("item", "renaissance_japanese_unlined_summerrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ae7b252ebfecc1d4b3b86e83df84df783b975bf499bd662001140420bcc7652"),
		new("item", "renaissance_jewellery_car_brooch_640", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "382287139b67fbcd2fb1315bf86403c15a76804f3c80e2047d0905c062563b33"),
		new("item", "renaissance_jewellery_car_hairpin_642", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "59c6258fee80098359037a36881ba371366d437dbd009204c3e4c994142c7549"),
		new("item", "renaissance_jewellery_car_prayer_cord_654", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "530b26a758832507587a9c25d15f606c408fbc72f6680941f4d7b53b744b241c"),
		new("item", "renaissance_jewellery_car_waist_chain_645", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d78da7ba848aed6bc7c63103b94977ff81cf2490bdd20a07a73cd3d7fadd1bd4"),
		new("item", "renaissance_jewellery_cef_votive_token_145", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8c4b73230bfc126c3e8dd98f9270efb96356d1383d90d0ac681363b78106d68"),
		new("item", "renaissance_jewellery_cen_bead_necklace_64", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "362ace3f1c2bc66e597ae197041c83ce0c605935d2e2bb354d802b6f63fcb54c"),
		new("item", "renaissance_jewellery_col_wreath_710", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "660e0b661259ad83cac4f3f610b4a63c2053174d1a742d6604d7e31fffcdf402"),
		new("item", "renaissance_jewellery_common_bead_necklace_184", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5318f29341c2e766d76b42b5db796b26db89f60a0dd0913e540639edad42eefc"),
		new("item", "renaissance_jewellery_common_brooch_160", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d1bd0f83ba624b2aabc9b905d2837b98d918c38eb74ada4d99e38c82623399e"),
		new("item", "renaissance_jewellery_common_chain_necklace_125", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "577dc56181988abd08b289fd2bb9ef7e536815bb8aac048db673bb89d9864f34"),
		new("item", "renaissance_jewellery_common_circlet_73", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f24210378ae931628f276545368d6a7c0d97aa4ff1a276bc3b9a9f5f914f505"),
		new("item", "renaissance_jewellery_common_devotional_badge_203", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf6ade5b02aab12597e27f710047ca710b32f7a1e362b7db0bd5daaa065a8f37"),
		new("item", "renaissance_jewellery_common_devotional_badge_83", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "34e0e678704f426a702e74f43508fea59e1a6171f21e887b3c72a017d9c2bcc1"),
		new("item", "renaissance_jewellery_common_diadem_134", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9ed00105a31b0cf09abecd92b562e1cb65539f9f0eb7265bf9c9ae376680d0a"),
		new("item", "renaissance_jewellery_common_hairpin_12", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dcbe8728e50d6a60bb7b7e0ea9b30fc376def2cd875008f004dc8e6dce246d3"),
		new("item", "renaissance_jewellery_common_incense_holder_208", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a1ba5921278855faf82415c85985a9ccda8ab51c945e9c9e61f3140f303c42b"),
	];
}
