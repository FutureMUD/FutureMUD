// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch060() =>
	[
		new("item", "renaissance_furniture_eon_bookcase_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "baa02783f2cc21028349e593e6bda37189af4c99b7905d2fa252ebd0efd2fe8d"),
		new("item", "renaissance_furniture_eon_open_display_shelves_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e45bc2dfba122750de9da3b8b93d7a953aa0f5f7082de8e446d2d688e7dfb19"),
		new("item", "renaissance_furniture_ind_mail_stand_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d41d24e53ab0f29969ea9e16a2236cc0f3efe0e470c8530a347808b300bd6387"),
		new("item", "renaissance_furniture_ind_side_table_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0efa01e703be7d5638ec3ab6b63942b3c932fa9ac34d60e79403fab25653828c"),
		new("item", "renaissance_furniture_japanese_ryukyuan_tea_utensil_cupboard", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "805d8c6f59edf12fe0d7e67d0aa21294072c263a71a0562bd328c66827b3b191"),
		new("item", "renaissance_furniture_jpn_open_display_cabinet_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ec8969dbfffa257f879a8ef473431f94ee173b7844e4ce3f809b7c74d4a9795"),
		new("item", "renaissance_furniture_mar_hanging_cabinet_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eef7998c19c890abab93454139697421e110f33a3c1525e166d29fedb1fc9470"),
		new("item", "renaissance_furniture_nba_chest_of_drawers_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3d3b44117c9a3a262db63cd794640821fbf1a8891428bfa02559de3b7effd8f"),
		new("item", "renaissance_furniture_nba_lock_fitted_weapon_cabinet_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ec421b79fd86ed74cd177c4ea2db1039519b8aebfc1545b1161570e556948a6"),
		new("item", "renaissance_furniture_nba_secretary_cabinet_01_lockable", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "acb41f08d427aa80e813481ccb7ce1929e9e78b5db4078483815a1ae98ce6ed5"),
		new("item", "renaissance_furniture_ott_lock_fitted_nightstand_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "45c4e48517a5d59ead53473fdbb6721261f1057b8f871bd3dd3c7e4becb7c525"),
		new("item", "renaissance_furniture_ott_travelling_wardrobe_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd915dda031988998c9d979fde6839ecb528850ce8debca22f3770deb4e9db8c"),
		new("item", "renaissance_furniture_ott_vessel_display_stand_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "73e70835f376309234583847e16d62bf349e1f41958b35d8fcfb34c869d1f698"),
		new("item", "renaissance_furniture_rse_campaign_desk_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c80ed39cb9a6be7cc3f13778615e20b55483f9a0499c9011720b2f60a60481bd"),
		new("item", "renaissance_furniture_sahel_redsea_indianocean_ceramic_display_shelf", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1e6df79fef7941ecf4fae52fb1edb86542b84cc065c414942806333f86a2356"),
		new("item", "renaissance_furniture_sas_lock_fitted_drawer_chest_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ba44ba702542c7f1c393fe44efadcc659b0a460854824f8837b25de84e99324"),
		new("item", "renaissance_furniture_sas_matchlock_rack_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e24a2fa36b4b453ef5a8b3cc633ca161e969198d4a6a8f4f297932536c547214"),
		new("item", "renaissance_furniture_sem_statue_pedestal_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d16e7c055336c7b0e48562a220c890b507f49584e527f4f8f8d0931341ef348"),
		new("item", "renaissance_furniture_sem_urn_stand_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "effb23855e54599a64fa32d74eecdd258dd497bd100b04ceb72776dcdf61b522"),
		new("item", "renaissance_furniture_south_asian_merchant_vessel_shelf", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01254ebe8c3d9895cf1e9c515f044ae35ed5ae8bafffce3d4833e2a3e8fab0f6"),
		new("item", "renaissance_furniture_wer_two_door_wardrobe_05", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "58250357e5ba19274a70c414508c088cfc88f53a2a66d115e04caed45e6a7be7"),
		new("item", "renaissance_jewellery_aca_bracelet_456", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b30917ea14b9497fa1911147a6ee8c78d70522009f706f2c22d8a8549225083d"),
		new("item", "renaissance_jewellery_aca_brooch_460", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "236760d4def8e1da15929efd541b0f7dcef56bb5cfcf52afb0179ac6eb948526"),
		new("item", "renaissance_jewellery_aca_offering_bowl_477", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ac8a06e43bccdf745d616e195235879b521ac1d952df78b3afd2f9d4ca48125"),
		new("item", "renaissance_jewellery_aca_processional_ornament_480", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "17a871ee032de311678fbcfdcd999bf7fb88fb161df108f2638c61846cd88656"),
		new("item", "renaissance_jewellery_and_devotional_medal_622", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3886526d778dc47fd907986759806d22001664ed213297df50d1c8157da4192b"),
		new("item", "renaissance_jewellery_and_portable_panel_629", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bcfe4f231ef21220bcccb4ada76ce8afed394ddb22e1e5fc1c50db87355e0bfb"),
		new("item", "renaissance_jewellery_car_bead_necklace_634", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cfeb64d134f7df1e09ce20dd228620e7353af5b64c587fd95c59ca7b755f841"),
		new("item", "renaissance_jewellery_car_chain_necklace_635", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be8db21455307db5fc01d64c4471c05e890ca83ca6d2c891fd8c6e342ecc1cc0"),
		new("item", "renaissance_jewellery_cef_ring_121", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff079a1957fce34cb29f5d62425c5d1eba2f5d806c280ad26fccb6d9c6e6bb4d"),
		new("item", "renaissance_jewellery_cen_armlet_67", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd44e63307533dab7c2a4c98fa34e7c2b19e32b38e537efc6b857dedc99252d0"),
		new("item", "renaissance_jewellery_cen_votive_token_85", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d19f1668382efa1bff4f52ee05c47d74a9ea15fa41a48567df1398fce32644f8"),
		new("item", "renaissance_jewellery_cen_waist_chain_75", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fcd27f6140e4d29949e3c69cd358294fcc7be1a9c86b5421e5080faa6f948de"),
		new("item", "renaissance_jewellery_col_circlet_703", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b26ca23b3b7d311bfff0d3f5e9a10d1f15f0b249e1c0b95005de91e38824efd2"),
		new("item", "renaissance_jewellery_col_prayer_cord_714", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3db604c6d56f55d653d6df1ac44fce6ed0c85847219d94410e02edada7b97157"),
		new("item", "renaissance_jewellery_common_armlet_127", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce714348026f57014086c18e1c1e2aabe5d709c3666f8ff38b6d12a3bbfdb870"),
		new("item", "renaissance_jewellery_common_belt_ornament_166", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae4d3302dfb6c9e6aab6fcfd43f9cf3d4e1471dc82828b07bab7bd88ab5a437c"),
		new("item", "renaissance_jewellery_common_brooch_190", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9a13bed086f078fda3080ee96764b8a2b55d92a35dba215aa3915468674cdcb"),
		new("item", "renaissance_jewellery_common_chain_necklace_155", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "065db0afc63a514c62e2cde3cf2f0fc21e35c2f0710ba2fca15a877b5161f072"),
		new("item", "renaissance_jewellery_common_chain_necklace_65", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a1a7d3cd8aa966c889810446a9f9bff2bdd900d2adfdf0ab4e6b45ca704c445"),
	];
}
