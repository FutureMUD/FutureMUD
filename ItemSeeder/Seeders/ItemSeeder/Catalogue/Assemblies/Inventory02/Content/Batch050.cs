// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_furniture_campaign_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "331a440011f81d7ea143378b17d653c33087ff4fb45cfba82364b5276add9e9e"),
		new("item", "medieval_furniture_cushioned_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e56272e5ccb8981183c9bfbf6ca2eb103902c2160bfa018f71e39e57f9d0a4d9"),
		new("item", "medieval_furniture_inn_round_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8bd9228bfa8fed725a8301b5bb7ab53aab7f8ab144027b5a70058cc9b9321374"),
		new("item", "medieval_furniture_kitchen_prep_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "013ec1366f12a83d451b1863b60930a0ae4c5fdbcc45ccb65f25207c60d508b5"),
		new("item", "medieval_furniture_northern_carved_chest_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "383534b55cd8ee1f5c6a7ff86c63d138f856705719807b8aef35fb5dafa50ccc"),
		new("item", "medieval_furniture_plain_high_seat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc9fc5a02ef8cc5f88524e0dd1d619a454db5bcd8cfa5477ad80eb449f146c94"),
		new("item", "medieval_furniture_plain_small_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "73a1525189a8e40d47f3c420a5810b0a48485d7f91da13efedd0854e4ce94e81"),
		new("item", "medieval_furniture_stout_work_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "85eb9a9140c2de3ce1eb987bf2db06ee2f2e70c2f0727f4f484774a87485ca18"),
		new("item", "medieval_furniture_work_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d53d69d8106f7d8a75b502fbeab637c419a99cdf9914cf2a09347172839ecc27"),
		new("item", "medieval_goryeo_headcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3f5a50a8ce4da6254948229ac3c042c1428ebba98cf2bd3f85c43ea227f3fec"),
		new("item", "medieval_goryeo_work_baji", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd71068f4a54720bbb394a44ddd6ff7c1563eb9263e7b60646b6587987edff7c"),
		new("item", "medieval_heat_bronze_table_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "55012ea7997ad8c4c9f2cc1a74564c795da9df64a2ca37a9d99b4f2c8a54680c"),
		new("item", "medieval_heat_east_asian_cast_iron_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8a175a41d1bf22bec48325ecd89303dbf6cebfb14e48f7f99e4449a850cd3be"),
		new("item", "medieval_heavy_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "623484bff35e1d9e00327acf0b1727ff0c55ec386b236b78c17a2532c53bfaef"),
		new("item", "medieval_hindu_brass_offering_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8099b52176bff2300fc2b8419c72f610b6c7141c6a59c8bdba34c032133b610"),
		new("item", "medieval_industry_stock_armour_scale_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "22a52f89cfc5ab8c00a381bb3ad4d757f42c9905fa3dd96418603354031f4138"),
		new("item", "medieval_industry_stock_glass_pane_blank", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7aaa892a49f3ebc7499c79320b07be543ca0bbb55aed0cd757d99feea4b6c237"),
		new("item", "medieval_industry_stock_rope_coil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f68e9c855c1842218dad20353a53487b2f79841c88b408613a282a34604f6a2"),
		new("item", "medieval_islamic_taylasan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d311f077205102aa079048891e1d55de8e06f024a4e4f6269a10902feea3ac5"),
		new("item", "medieval_jain_white_shoulder_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "07ef50d55c1a7dcc9f79fc25a39c6aa7eee3b2246b3d06559017f15d6ce3fe79"),
		new("item", "medieval_jewellery_brooch_coral_set_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "edb9541ab4c1e728867214a5f085c0a26bcf15c966a6a5f3277b0b85e136eee5"),
		new("item", "medieval_jewellery_brooch_lapis_set_court_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9623dcf6a910cf16799366261ccdc2a8b56eb7b1c1b6709a7789ca8110d7bb78"),
		new("item", "medieval_jewellery_brooch_shell_faced_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed6844143517912b978a7d5171643a6b651e72dd0ab192d306717190d1418eb7"),
		new("item", "medieval_jewellery_common_plain_bone_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cc47ff9e72fa8a91247b64cb69142a9bf1a697784aaee224cedf3c6b79a1fa6"),
		new("item", "medieval_jewellery_common_shell_chip_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a121b768bc536cdf4e81a0fc7edb790a800b44f23971aa12f3f948ba6d17fd7"),
		new("item", "medieval_jewellery_ear_pair_of_conch_shell_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "44665b8b8f84b9cfb263b889443ad5fc353f8a399785f216aa8a089d7980b1d6"),
		new("item", "medieval_jewellery_ear_pair_of_jade_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab7893a315ca9923675fecd1ebe537e5b224530382ed0097db7b39384f06b234"),
		new("item", "medieval_jewellery_garland_dried_marigold_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "56c63b0835f7a58c91e4f8cdbdeb5882b59da7eb958f66588ea513585dd800e1"),
		new("item", "medieval_jewellery_head_plain_brass_circlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf59dafb524643ae60b9812cc46a0e1276a2f9b77ff6fd1f47c1ba40e7aeda30"),
		new("item", "medieval_jewellery_limb_coral_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "36a3ccd87284f2fc11b6b719a1518fada0d0ff32aa452b783b3cd358e292338e"),
		new("item", "medieval_jewellery_limb_silk_knot_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d635408519fc803539025c170d6668b23861252bd8753f6a37611c4676d4e9ee"),
		new("item", "medieval_jewellery_limb_steppe_silver_arm_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "64e075f2ae362af98a9336620713c901f104bf9a04d1571822300921298b09e2"),
		new("item", "medieval_jewellery_neck_beadpattern_glass_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "27cd4311b454fe9e95e9646620405a9bc64dfc97394d205a4aac1c18af91ffeb"),
		new("item", "medieval_jewellery_pin_brass_ring_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4eed2bd6f7226d97610fdf3746df870828976a94d1ae31e298cedc3be3a8ed0"),
		new("item", "medieval_jewellery_pin_rock_crystal_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "20d627f63cc1df3c78a5d6163a60a8e6d2ad84821727ab2ddb5062334d76833b"),
		new("item", "medieval_jewellery_ring_sapphire_court_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d31b259dd29fc51723e3ff5af957750a635bdd022ec96d190838d439a6c382e0"),
		new("item", "medieval_jewellery_ring_silver_gilt_finger_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "81775c78835f3ae19c9b88b843d07613ea95de1f9e84db748fb60e027524e008"),
		new("item", "medieval_jewish_washing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "96a2ef1d355cccec34acb92b72817c19383a0352d3f2f37c386ce7678be6a7d1"),
		new("item", "medieval_latin_bishop_mitre", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e28419a6d332ca3c2e8d162d129474988e28ccd13d4faf0e0343054fac5e5f01"),
		new("item", "medieval_latin_black_scapular", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "08b7ea2c19504846071b8e1ddd700157c393161fb7946c1503f4210efa0fb9e6"),
	];
}
