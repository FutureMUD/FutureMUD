// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory11Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch046() =>
	[
		new("item", "industrialised_weapons_police_military_0012_reinforced_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "288b268cca7d42d279d1e422692db02fcf17ab9a1a1ada86f66de665f9eb1a65"),
		new("item", "industrialised_weapons_police_military_0029_compact_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "14b8e95e83caf3f001680cfdbffad1444fc9a1896eeb1d92219debb4a319db05"),
		new("item", "industrialised_weapons_police_military_0031_heavy_duty_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "864659f77bb8ea2c75d37163b06d962087865209a7dff19d3d9d7a6b7ca46df7"),
		new("item", "industrialised_weapons_police_military_0045_folding_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "6e80f657ac0e7d6cf378bf5be114462a7c893825796e5336d05f425ab5d2be24"),
		new("item", "industrialised_weapons_police_military_0067_workshop_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "b940ecb1f70d829d7ac5468282b33f526a91a5971e3f21d6400b8913f9d28191"),
		new("item", "industrialised_weapons_police_military_0098_service_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9a9fa720e599344f8b850e4b639a6ad4c37bb0c9fab864fdb1ad555029857a9"),
		new("item", "industrialised_weapons_police_military_0107_precision_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "78394a46af2eb7dffbbd8f868891c0d487fafc5ce5ba9670601fe8749aef25fa"),
		new("item", "industrialised_weapons_police_military_0124_lined_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ae324ca0697c19ac233c86d96ca3b83b0490d3c22b67bc44adcd5bc74ece697"),
		new("item", "industrialised_weapons_police_military_0160_field_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "4bd29a9df90a60739184a70d153844234bea69cc2a74fd2703d1714cb65bc4fc"),
		new("item", "industrialised_weapons_police_military_0168_countertop_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "a012c8fe1c7e2950a3a35a0b4d0453e40f7a0c57a75655b2c9292dc8441957b3"),
		new("item", "industrialised_weapons_police_military_0177_wall_mounted_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "53aff32784e7c06716bd9658d215e6268a1a506a399f2ca4947d218285d9472b"),
		new("item", "industrialised_weapons_police_military_0185_travel_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "78f31e9a2207cd2b9aba27443517bd34d32bdbe6726e966e072f3d784d413dd4"),
		new("item", "industrialised_weapons_police_military_0249_folding_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b67ed2db7db1cc4cfa9bffb2dbd64dab55366fe26b45656bcf80c9d14aa6f60"),
		new("item", "industrialised_weapons_police_military_0264_workshop_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "b72855b61f0221d030205ec9af86ebf9cb730bfe7e1818239f83b0b5d87473fe"),
		new("item", "jewellery_angular_marked_silver_fibula", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3bed47a933918bdfaa3f64f4f17e4b28962cec3cac7aa4ea21541a08ecbb102"),
		new("item", "jewellery_blue_glass_head_amulet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbac48fd9d3ee1774dc42b6fd80b73c9878f7bb14cae0f7f619363470f2885e3"),
		new("item", "jewellery_bronze_disc_bracelet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8501f4496223f514e1fce0991aada0daac8718d460efd00663365908817e0ab"),
		new("item", "jewellery_electrum_rosette_diadem", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1bf4b35e9af8e6e7deee6daeae135c9efd07bbe39046b405ddfe38e212f3b1fc"),
		new("item", "jewellery_electrum_winged_pectoral", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "452bda3f0b0b55a6b0a9372283ec652547047e1248f601c175bde95a8d63e377"),
		new("item", "jewellery_gold_glass_pendant_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f57d7d71fdf97fb7a9e7182c446f53c2bc584207a18af6f7e24007b5bf597da"),
		new("item", "jewellery_gold_inlaid_turquoise_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f627d2d224f1d44c5ef023411cfa9fbb1607966144f548baa0dc9f532cbd8781"),
		new("item", "jewellery_gold_sphinx_fibula", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a5132c26ddb10e6e0112761b3c0858113302cf163c8b4fb45ad708640768a5e"),
		new("item", "medieval_apprentice_canvas_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6a76df106bc67d289e4beacf61c921a03063ac336479c58d5fb907aa02ab933"),
		new("item", "medieval_broad_brim_felt_hat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f6bc3db1df7e2552069412af28831b8e95830c0c7086a6e5e5f17ddf9e3e917"),
		new("item", "medieval_buddhist_incense_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a1d22bcaae30f7db787e9e2e060f9221f3bf4312d3cc7c46bafcf4675fcee0b"),
		new("item", "medieval_carolingian_frankish_tablet_banded_linen_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7f151f7021292a83e32b8690089487125f7ffe58fe81e51ffe2bc837ecd2a6b"),
		new("item", "medieval_christian_iberian_camisa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "df50d22e1d58fc14ebf500f6ce2bd83354192e4a01f6f32d4d39356aabee1525"),
		new("item", "medieval_clerk_wool_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3aac26d4ec15efe95423d4b3ae4f0684a565cf50ba1d32d0c6e8898fc5e6a657"),
		new("item", "medieval_daoist_scripture_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "192fdb6cd1a48e5a2db8f52749f26936d795ed6f1f6f734d0dbcb87b8f65cd23"),
		new("item", "medieval_decor_carved_oak_wall_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf75d3e6393af9db732906918c99cc9b07cd2e00fa3657b18230d5135a3440a7"),
		new("item", "medieval_decor_east_asian_bamboo_window_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d5ef8e16aa47b883fade2907b9b59d20fddc7efe7d3458e18b9f28696477a18"),
		new("item", "medieval_decor_east_asian_paper_room_divider", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a3161c973d45ec8ab94ae66f0103d5291b8a5cca53a8b7372fd5a42ad609e9d"),
		new("item", "medieval_decor_east_asian_pine_display_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a29f10285a9d8f8fa6b029cf3a9f077b1429a95accd3cf4577769cdcb26e391"),
		new("item", "medieval_decor_glass_wall_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f61811086b3a335eb66ab765a4e6a2db1fbcf15a41fa3fb13e5edd17fb4316f5"),
		new("item", "medieval_decor_plaster_wall_plaque", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "adc58a892ecccec14f8b734a986a5cf0b6ceb6c2ed30d4baa752b2350d168f9b"),
		new("item", "medieval_decor_silk_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "84580028cc148481af264ed4d3d6ae97a43f0e3489a45af4942baf23f3a6345d"),
		new("item", "medieval_decor_steppe_wool_tent_band", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8023a0b6334be5bbc680f9c7ec8934fc0f6d49cf957dc7fe8169a85dc9604af"),
		new("item", "medieval_decor_striped_wool_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "062e824a6f72a567bbe1cd17016abc4c7ee42b6944bec537f42c22824ae31f67"),
		new("item", "medieval_domestic_ash_peg_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb4728ebeccd3777fe662ed3fc4d18242468b53c34aac6a4716254b88d9827c6"),
		new("item", "medieval_domestic_blanket_ark", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c479f3d655beff5ada5b7fe81aa0b62c0b4b93c9393d63865ae7db182e5826c"),
	];
}
