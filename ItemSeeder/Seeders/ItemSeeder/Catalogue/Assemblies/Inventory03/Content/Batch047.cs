// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch047() =>
	[
		new("item", "medieval_church_altar_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "864bfdd0c60fd4342e82f82ab8de441a5c2f0a05ddce255096135b3fbe30a735"),
		new("item", "medieval_church_candle_pricket_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb014dc59ef1a4bb2119a4bb6eb58485d64582402916daaeef8dedab5b4fd9ea"),
		new("item", "medieval_cotton_waist_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1aed90837b42d6bf09333c9092bd2d049d858d89e73041c450083da240e0989c"),
		new("item", "medieval_decor_east_asian_cypress_frame_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb452214a66d1f22ae98b0e70cdc0ea848f3d0da2f01f039075bffe9752b6d29"),
		new("item", "medieval_decor_east_asian_silk_scroll_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b9654472c8a1eb663485bedd65c0580f791bc63793689702046ee659618ff41"),
		new("item", "medieval_decor_painted_pine_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdb2ac70cc6a64372b4d90c2fad56d7e64ba0886cb6a026bed7b35acbc4f1051"),
		new("item", "medieval_decor_small_bedside_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b49232de36af493799d0abb9b3e5ab304a74248e460d9352587a7e64fcc56912"),
		new("item", "medieval_decor_wooden_folding_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e1b66b5a5192b475eb2dc375e566efa4681a8a1fc8bf65093157ed2b5963b9e"),
		new("item", "medieval_domestic_bead_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7465293546a776117f2d06cf5c1faeda16349d9fa11c4b30c976a387f188d09"),
		new("item", "medieval_domestic_bedding_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ed5ec1fc8988520776dda71f53b1fb6b166848993978aa0a9101ab1640e282f"),
		new("item", "medieval_domestic_glass_display_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e5804f9743afa3d3d4d53532dd75868a3a7955e1a4b126490808bf79fadff0c"),
		new("item", "medieval_domestic_hemp_marketing_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "323971f582aaaa9487976e691a4437967e0d36d894ebb351c03b8c678d0e908d"),
		new("item", "medieval_domestic_household_spending_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b09ff76308da1ea237cb6b6caa816f2031f4ddcb9b5d0047d48691fba154770"),
		new("item", "medieval_domestic_hunting_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "89d6a2ecceeb24849ec51bea84494e8b812a175c1d5d787f2328e767dd572b2e"),
		new("item", "medieval_domestic_ink_and_pen_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e988d993176771169ec3eb0dc9c1621d4b09cc4375e2f50c1ea75488b1a143ab"),
		new("item", "medieval_domestic_laundry_carry_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a18dc0557aaf514a3c57d1ef12c97f04bad1a43b5a9c1cabc5bcd71d69de5efa"),
		new("item", "medieval_domestic_leather_writing_wallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "46ac3f4d09d0aecda813025f7244055744af44964bda8c61e1d028007df86356"),
		new("item", "medieval_domestic_linen_armoire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "51338bfb42b50bc20c3b8bc9fbe4f7539c0a747d8760dee67aeabda491068c54"),
		new("item", "medieval_domestic_linen_laundry_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b23949cd97c99018d8941046b500a8d6cb9030c5086605eda8c7dec771f664a0"),
		new("item", "medieval_domestic_root_cellar_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "42e4c54bb59c26c0a0465226d720ececea307fe18d7d532f0b61e82ae76b8177"),
		new("item", "medieval_domestic_side_table_storage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "add60a621e5088fe1dca1a51bc55418d2654b9fe3367ab9f50fd8122936c4310"),
		new("item", "medieval_domestic_small_dresser", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1e559bbe4cea2122c9ba43fdb46b847acae56bef96ae5e0638710478f3a3ec1"),
		new("item", "medieval_domestic_soft_cloth_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b74b0775f456398f1b4e0ddc27d850c5855d543dbed53ad95feec49d1c9c9488"),
		new("item", "medieval_domestic_table_drawer_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "477015e681d5130da8701c52da36a9faa78b149680c1c8b14c509965fa7c34e1"),
		new("item", "medieval_domestic_under_bed_blanket_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1adf7b0d2355d6da8f3cdb0c96c70ec3a8db7b2ef096b1fc9da4101cd805b61"),
		new("item", "medieval_door_east_asian_lockable_panel_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2e8f20cf09c5bbfb53d9ededa7dd7886a1c4f5064416216cf66957982e4da65"),
		new("item", "medieval_door_mediterranean_villa_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "80cfc351c9bebc50e4ddd1df14cccb9355578728a11c46eb6b32f071964036aa"),
		new("item", "medieval_door_shared_lockable_warehouse_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4554ee3f4c37a557ee9576511ff355e6b1d20905b1f9102e3148beea1e11ef6"),
		new("item", "medieval_door_shared_stable_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "94ae923152239620556a3881a4e0319c632d03add184a9074043542fe60f75a0"),
		new("item", "medieval_drug_ephedra_brew_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "00da6d34f1bafc1e91f0b424c7364f2f30d6806745c17434f499a215ae9da285"),
		new("item", "medieval_drug_mint_lozenge_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "56a7b5a1ee57c6ad93a36053a550d56e6f00cf8e4b7867262ea05c4f2ca948f6"),
		new("item", "medieval_drug_soporific_smoke_cone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e30b67f414e7456512373c892c87b215a4264945c603720719b0bd75b2642bd7"),
		new("item", "medieval_eastern_christian_iconostasis_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c168966550be7a94b474755463752010c43a0d5e67062520dcca5d40929b491c"),
		new("item", "medieval_eastern_christian_prothesis_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b08a206a10fa02a9998a4e3e0bc436345541053cb444dda3aa6508181d7ef593"),
		new("item", "medieval_fine_silk_deel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dedce531f081d3d68baa9ae8d7e12e52ca977ef81987388954b1a006f5fa7d29"),
		new("item", "medieval_food_abbasid_bread_tharid", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd47254bd9ddad0f56589e6278253527356f0559e16f0b67663d26e9b67df7af"),
		new("item", "medieval_food_cameline_sauced_meat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f784b8423443f5b33dd83419875fa4805dbc778e4647ddcc6ab846eedd2f95a1"),
		new("item", "medieval_food_garrison_wheat_harisa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "736d7968de7cc626dfffdfb9bf3c1a4ebde86dfd42cb3c9762b0ab70728d2152"),
		new("item", "medieval_food_idli_fermenting_batter_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "75d6cbdec098e922860a56359db6f3e93c60faca8ae7d7435e6385e4be4b6dce"),
		new("item", "medieval_food_infirmary_bread_panada", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9bfad2556ec516ed6ac159470d61e0d6e7af31b8e9fde564583d4f1f6c77226e"),
	];
}
