// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_locking_domestic_medicine_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "736f8df36b10e52f9cbc3ddf255afd74b36a9cf208b25a8ecabc9c5332cb5c78"),
		new("item", "medieval_locking_religious_treasury_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cb0ca80afda29d0a4f028f4ab2a301db0ea3577a9655f4e227601cee7bc56fe"),
		new("item", "medieval_locking_trade_customs_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "202fa315469f14867569541481cadd11759dfee344a4f1dddcb79ffcba3b73aa"),
		new("item", "medieval_locking_trade_dockside_bond_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c67fc8622b28347c7c25a5b7ed7b6b38ba61a97480aba2dbd76ca375b37f2d1"),
		new("item", "medieval_locking_trade_teak_spice_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b747d80e38d8c9d51acf75776da7a9807a5a2d6cb3bcbfbf90c134cc497b7e2a"),
		new("item", "medieval_locking_trade_weights_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1570c1fbb8cef1382d29cad5fc151ada6769c635ed357520b6a97f112c1c05e0"),
		new("item", "medieval_medical_battlefield_medkit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9745980f3f272da5d68bb64309984657e6923de9e104434d1120dc9dd43e44ec"),
		new("item", "medieval_medical_bronze_forceps", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3857f12a754bf4e720554f0e3c960e83b278b3fd37618be80cfb0028b136475"),
		new("item", "medieval_medical_clay_cupping_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "42f67295c914b978d2a2c6f9be9bcdb649af3382bd4cb0c8cae4ebfd39633ce3"),
		new("item", "medieval_military_cavalry_adarga", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dadcce28dba922658fa010ce29932f2d600f39fe818dd4f66ba84e5a05cf6c2"),
		new("item", "medieval_military_compact_heater_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f562fedd3a0692d30ebe25b8cd6bd78df0851a0d8d3dc7360396f99372f967f"),
		new("item", "medieval_military_field_point_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "60d264d0e554ff17f8feb427379805e057aa60a5b3e7c65aef3f04f8a861c5f5"),
		new("item", "medieval_military_fine_hand_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2de5919a27735e2c1d60096042882b1a603ce60ee93c18350e4a45cbbaecfc11"),
		new("item", "medieval_military_guarded_leather_gloves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ed75b590eee612ab1ee2b18b780cad61360d88a97d1645733e4a01d5eb856e4"),
		new("item", "medieval_military_laced_hip_plates", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bbf78a0ae83be43373322873fb65dbc126a0e677dfa4b4e3dde28244d905c5d"),
		new("item", "medieval_military_lamellar_croupiere", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "308f0e1a4426d1d2817b54a8250f96a87225e332e9462128102181c0d124194f"),
		new("item", "medieval_military_laminar_cuirass", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0dfe8c2854434662d63bab590302dbeca77b161e86564ae3e397d3ff92e7b202"),
		new("item", "medieval_military_leather_faced_tower_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "415e493214f5c4317fefa05f8d61899d203aa89af09cd3ba18c562643da76543"),
		new("item", "medieval_military_long_cavalry_kite_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "995734b027de28f1a833a2272e32d69ddf9a4c5438c1685d882c76dd1e03e3fa"),
		new("item", "medieval_military_padded_arming_doublet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0e89798ce7eb4b145990867f927203c214b6a2429c7ebb9b489f7b18729e820"),
		new("item", "medieval_military_painted_standing_pavise", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "82c183e922b29ca906901415719943360aa88294df69bfbe9ca0a4d1c7110f52"),
		new("item", "medieval_military_plain_arming_sword", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c179e0a0aa393e1ee5ad4c2cbb522dc52445f91fc75df7a10f852c95a4e33d81"),
		new("item", "medieval_military_plain_caparison", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "23be66de48f20fb29619002533a18cb873b2ec7710b8baf31eefb8b16d66ffb0"),
		new("item", "medieval_military_plain_round_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "78c705133f0d1119726e974573b0932b3a6548d3cf6096f45467afc572983dd2"),
		new("item", "medieval_military_plain_spangenhelm", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfa5a5c021192ac67ca66c7d59d48d3253fa31380b86b5c22cfdfe7fcda24691"),
		new("item", "medieval_military_plain_war_saddle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2711294e0942089af5a1c8153aee56f2dca6a9e19238efbae8ba34c947c7e932"),
		new("item", "medieval_military_rough_field_point_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1cacbe1af3f96914120a60e4f060a891fc4fbba63cc11d4730706ee6fe64056c"),
		new("item", "medieval_military_scale_bracers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "80175336caf2cf7f66e785c9d920cf8508b6c5c7553a9dfdd688b470caa5e57b"),
		new("item", "medieval_military_short_sword_waster", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ecfd6afea61b8afeba236a834094a5347a71188daf2cf3b599fd0ab3ea8e56b"),
		new("item", "medieval_military_sinew_bowstring_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e630a64c8cf210398472c7139e0b2681ab2841d2d68c6b1c842392b205457b1"),
		new("item", "medieval_military_spiked_steel_flail", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f0001863872d7c512c652926837d08a6a0fdd73e3a7eb3b1b7b05c62ec850c3"),
		new("item", "medieval_military_training_mace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eea4786b6d6e684e48e85660669e99cf48cac80263ecf4445330b0de5752a665"),
		new("item", "medieval_military_wooden_crossbow_bolt_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "98c5f80e38b330d8e71f4e42740b71b2fd886e0efdb331e26ee0a421752dba1c"),
		new("item", "medieval_military_worn_hand_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca256b6fa86c5c102263a4a94b736c585fce39f6fdffcd1648a1baefccbd9530"),
		new("item", "medieval_mounted_trader_coat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae42c228eb03fadb60920458e276def7d69229e708591b045536f61a5fb511ee"),
		new("item", "medieval_north_indian_rajput_teak_clothes_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "04bd92860c0fe25b43e0a16cbf6dcecd9808e55e50c0d957fc9a1ca41d8ab873"),
		new("item", "medieval_religious_wall_shrine_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e02da50f15b6857019f5950c8616d464ab9c40749de8508b6a829c85969f5af"),
		new("item", "medieval_repair_cloth_mending_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dadc1cc0c0e5b1062cef17f76a569bc10f5dfc62e0d3bcb5ab423f454085b7c5"),
		new("item", "medieval_repair_fine_hard_organic_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf1b1f58fb9333dee33ea9f6819272faa89cf5b75bdc2be6cb552f9632904291"),
		new("item", "medieval_repair_leather_repair_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9c9aa9a522eaa51ec3e0d061a858f9fb79b8ce4a54fb92d21ca3c98427695ae"),
	];
}
