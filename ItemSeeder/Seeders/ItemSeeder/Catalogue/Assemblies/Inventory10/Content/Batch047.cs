// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch047() =>
	[
		new("item", "industrialised_weapons_police_military_0250_folding_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "a39bf058bf634aba0649cdc57c8ad459eac940a5d2ab1e43ada58b368084e2a2"),
		new("item", "jewellery_bronze_ankle_hoops", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff36afb754a1965b9ed7188515ec1745729615d8d749613091f09c6023fc22b9"),
		new("item", "jewellery_copper_lion_brooch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b4a1e751fd467089ec4aadbe5383f4451a1cc5260881f8da839970dd0ee7271"),
		new("item", "jewellery_cowrie_shell_girdle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fafa23045f1b91bb4487e2cfbebcd5b7251865079aa72b06b1297ee3e8aba452"),
		new("item", "jewellery_glass_tube_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5208ac340f1a93d5e7bf566dfb84a28b8ed9c580e51811070065eb9c4f511d12"),
		new("item", "jewellery_gold_animal_cloak_pin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "82dcba2a03b61b45ac446397d6681539a7b01fdd06b684262c02c971befdcff7"),
		new("item", "jewellery_gold_rosette_sea_diadem", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bb7a6ea1b6491ed5a4542e60ea0829e331f4cafc9fd8f157c258fa4ea430869"),
		new("item", "jewellery_gold_strap_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee074538f01daea6262954df8dc184506b54c57748fed52fa265612d04227bf2"),
		new("item", "medieval_andalusian_carved_cedar_spice_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3771fdd99b139d816231114ac44b0361a526500abd4774422c6823d6ce5196cb"),
		new("item", "medieval_child_linen_shift", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "84bea56801828333863b949ab26b63e14d4c11dfb5c0b6f650618f39740fef52"),
		new("item", "medieval_church_choir_stall", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ce3fbadb79ee02859d99cb7622158d2fc37b7148e99349f10f53132ee9eaec6"),
		new("item", "medieval_cotton_dhoti", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e80fe98eee6517e0591441c4b68582f1a944efc2e6a8dc31dc764d15f15856a"),
		new("item", "medieval_daoist_altar_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8af964c8a99b6e768fd98be594e388a8c85bd4d84aa499120b0f6040c0b463f4"),
		new("item", "medieval_decor_carved_household_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "152484b04b09bfa2cf9d94124912efce658d2a17dea3dd708abf9d5ea712344d"),
		new("item", "medieval_decor_fleece_floor_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "88bc0ca109ea88308c9207c9f2813a4b10737f1c2a23c623c096f82d95bd0b3f"),
		new("item", "medieval_decor_islamicate_cotton_window_curtain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcb911f270b03106d9b17a07d60b874a47621369769e071015bfa3e6f70a2021"),
		new("item", "medieval_decor_padded_floor_throw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b8026c47befe57f31f910836a4c9bbf1e80d9459399f8d30837b5a6e2f69ea6"),
		new("item", "medieval_decor_steppe_felt_floor_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "07ee98818dea8f7a0492c4a19bb6d36c1ac2fcc309e38996e77e167ff3063d28"),
		new("item", "medieval_decor_western_oak_tracery_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab8fa630d7c8e12951c035216b306ddf05511342b7c7e21bb94df8ee6b74d242"),
		new("item", "medieval_domestic_bed_storage_surface", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "24cad6639f3e981b0ae1c75c7839ca6d329d0a1903ab4335d18951dcfab3aff0"),
		new("item", "medieval_domestic_bedside_nightstand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "869f7daf6e3dee8da687f88fbc68ebff142170444b9cf3acdd9d3b5299058f75"),
		new("item", "medieval_domestic_blanket_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2bbfea84f853acaf424cd3152a8272852d638a22f323b88adbfbf996b456d1f"),
		new("item", "medieval_domestic_bread_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0cfbcfea6621ac79aacd84f78aa1bacd30dd6bd89f9325f0db15b9b8d76ec27"),
		new("item", "medieval_domestic_canvas_tool_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a50118c57b426d6cf928af45d6b4f60de24126ddc7a287513faa36aff4228418"),
		new("item", "medieval_domestic_dried_herb_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "be1b1ab018a5198d8f248251993df4b0538b2e7a74b6a9bcbc1b49186ce0ff8d"),
		new("item", "medieval_domestic_fur_storage_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "15807bf20d6010af0b85f075c516a5d02213ef43c7556cada229590a321ab896"),
		new("item", "medieval_domestic_keepsake_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6350942d7a7c1917d9ffb98ae90d2a87b4c66ad2447f7f8d698f21bef1ea17ef"),
		new("item", "medieval_domestic_kitchen_hutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "300a7e216ab1e34d2fbcbed387698cf6e1bc93d4029b5b65aeee665325c18b10"),
		new("item", "medieval_domestic_leather_coin_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "747de26e19ffca3c0f177fbf7f6f93f7f8cc1269ac6cffb0d0233051fbf30da0"),
		new("item", "medieval_domestic_low_chamber_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6057e634bae29c92c1aa322fc0da8b1f01eb33c282099b6e83d63155be1f0562"),
		new("item", "medieval_domestic_mending_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7483cd09adc0e6c8bbfd7e47dff2e4ef7029105d1cf2477cfba32abe0d7e02af"),
		new("item", "medieval_domestic_narrow_cloak_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a05bd338f60a2cef8099d7e32c8f99012ba2858a95cb6207ca7d65d2fbd68ee5"),
		new("item", "medieval_domestic_oak_storage_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b183987ffae9eecbd0700b4e7d4f1130d23abb914fc091ae37abaad40ed935b5"),
		new("item", "medieval_domestic_plain_nightstand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa8f3c898c706622da34a8dca0e9c41a62026b775ee588023b5c927459d2e47f"),
		new("item", "medieval_domestic_scribe_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d238236156738074f2c4c6af0b64771b808bcf8b112b8b904f88b3cd77119d35"),
		new("item", "medieval_domestic_toy_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "19d8efa44682eed2193e09fe108dc2e913404a886d99ee432e3aa52f7740112a"),
		new("item", "medieval_domestic_travel_gear_wardrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7386e22c131e921544b3dd8af44127d5b91165e5603a8623c1f549967ad7537e"),
		new("item", "medieval_domestic_wall_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "858e12f0a7ffac03320874910db00df5feac3fd57ecbd434f3c8c6addb063a16"),
		new("item", "medieval_domestic_wide_larder_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "941e66f8612e146c380fe6aacb7d91f4b12c6ee54e8e87182d619d40d5b2ea0e"),
		new("item", "medieval_domestic_wide_linen_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a46f9f02ee8352b66a0e9b58cbe8bb8f7507c6db83a4aedbd631d9024227b81"),
	];
}
