// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch028() =>
	[
		new("item", "antiquity_food_aging_spiced_wine_amphora", "antiquity", ["antiquity"], ["item:antiquity_food_finished_spiced_wine_amphora"], ItemSeederOwnershipPolicy.RequiredRelationship, "7b871d2c5c09a6dafa2f64e25829595ce922e7bd0214fe4f8a0a08d47f9e2417"),
		new("item", "antiquity_food_anatolian_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "88857a5a5641e7cf58a2271f7761fd44fac5cd143e389034474d1534f9301289"),
		new("item", "antiquity_food_celtic_pulse_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecd1dfb2b38764b24535c3b295bb3b07f58146e0ebe9928cf7539100714d5bef"),
		new("item", "antiquity_food_etruscan_preserved_meat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "abdca7d181c156f4258ed111b954ed006dd81a55571c90f60e6215cea5559ff1"),
		new("item", "antiquity_food_finished_garum_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "816c070894318127435b2c4e0287c06a86636a58f613ba851bb35656ae8f1816"),
		new("item", "antiquity_food_germanic_fruit_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "644c97821b89318130d32e32721e8ab565febcead2c18a930af123760ceee829"),
		new("item", "antiquity_food_hellenic_stuffed_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "04dcf8001ec4a7a35488ea5dc1f39c1ef1d6c37293538206cb0a339608194c29"),
		new("item", "antiquity_food_kushite_preserved_meat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6eb1e1c3d2e83b5e998896230c0657c49b2a5b37a748757d2c92ee4234295012"),
		new("item", "antiquity_food_persian_pulse_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "960d43540c214f1aaa0eb2cdd9a76972acda6a236ef00ad590545704d96e4b59"),
		new("item", "antiquity_food_punic_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8736c3b6580b0c9115f11a7a5108602298e7b575630e11dcb817ea1da671108c"),
		new("item", "antiquity_food_roman_preserved_meat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "872dfee59c5e2fc63fc3af4f228473bb554e5696acbbfd146643576f761ea737"),
		new("item", "antiquity_food_scythian_sarmatian_pulse_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce78ceaffb13d725d2265e1a4fd767ca95cbd31c97742413314c58d4e079ca35"),
		new("item", "antiquity_food_smoking_rack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5038778e731120956358d201ff139926b03e74e2f3b61b869c674cb1eb0a1270"),
		new("item", "antiquity_glory_hole_furnace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bf055e9218288c53553a6b08ea04735110db1578b40cfc0ab158c3471e2577c"),
		new("item", "antiquity_gut_suture_spool", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9014b7bf6df34a165baccc1644bd682cbd699b9209591744a61ca312bea2e1c4"),
		new("item", "antiquity_heavy_felt_riding_coat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b548dbed2dcd69fdec4fd14e7b5fc18805dab647e31765ea0255542e59ad0e5"),
		new("item", "antiquity_heavy_wool_zeira", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "61ec2434e868ae6df6684c71d76e6d0519fe6050a4e8bb4cf55773e503759f17"),
		new("item", "antiquity_large_gate_bar_lock", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2305ac3796e19e2ff1b7055a515e1bc51790ae8125ef4feac43da1bf69edd60"),
		new("item", "antiquity_leather_awl_punch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7adde5170eb73277c06133682b2517837188886bf72a87f475fcb2109e450f8"),
		new("item", "antiquity_leather_codex_pouch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4252813f19a5d5f681059fd58714ccdccbce54dc89ad59a167a76433f03d948c"),
		new("item", "antiquity_lidded_ceramic_pyxis", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac1a070272c419115b22c5f19a3059ab491049d459dacbec88e52e66dacf6703"),
		new("item", "antiquity_linen_breastband", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0235d8f08c452489f93e7abba9f880fa4841959177d48ae43603b62f1f4aab0e"),
		new("item", "antiquity_linen_shoulder_shawl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a896a8222027b31e464a34f1fc1805329d1042241a9f9c536f6161f7732cb0e9"),
		new("item", "antiquity_liquid_blue_glass_cosmetic_bottle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "85647edaa68f16f10d3d337d146f0b4f9e03a76b5110ac156d5905012fb6a61d"),
		new("item", "antiquity_liquid_bronze_belt_flask", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "316a4e8c749692cc12ed0936ab8e1f155b7511843420bb829cf841b859a667d3"),
		new("item", "antiquity_liquid_glass_alabastron", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "841301b9d4131e8bec82fd253c8bd17fec50c2f0cd08600ca2afbd176afda404"),
		new("item", "antiquity_liquid_red_slipped_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "697091cb5646357d9874e90dc6d1afa153be59bc23ae7cc57ec89dc3d17c4d03"),
		new("item", "antiquity_liquid_small_glass_unguent_bottle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "01d4ce3b25a6b15526e8daf8e077052a77cec71d9fe8fe882a04613d97a43407"),
		new("item", "antiquity_lockable_palisade_camp_gate", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca0572973fa0f6e84c1f5bacf58834042131ff9334af883b2a4f03400eec1b47"),
		new("item", "antiquity_lockable_studded_plank_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "62e963c0694f8f48a744bd260572dd8e5bd437027302f853eb5d9e0dc5a0b948"),
		new("item", "antiquity_long_store_table", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f58b5bb4c44fe8bbb73e4fe84a79bf477c65c54681bd868ab465dfb7cee54863"),
		new("item", "antiquity_low_backed_chair", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc82bc18eef54d880ca588ec2db70d2d7ad84a7d41d1e4acffc0dc4424adfb5d"),
		new("item", "antiquity_mint_infusion_bundle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "82bfe47230c81717b0e7ea7aba7064871054974b58951343ed32bf73561f9ce8"),
		new("item", "antiquity_oak_brain_tanning_bucket", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "69f8773e0a2a70d296a682146afdf1132af0220eff4d0a1be717f5174f64352c"),
		new("item", "antiquity_padded_wooden_peg_leg", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2278a9b79b41f2677d9430c8f9ae404374ec282cab8c241c62ca18023fa3d5ae"),
		new("item", "antiquity_papyrus_strip_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8cf05782be031566de42f694aa62826c52e871e07e822222dec1f74da0afc24c"),
		new("item", "antiquity_persian_felt_armoured_kyrbasia", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fcba25620a204ece6356ba8a910b4fa0baab71ea6fccfb3337aa1cd5c999bbd"),
		new("item", "antiquity_plain_wooden_bed", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dcd107a6de4699c9029af0f7f7e1713686bc8197dbed50c46e8cfa342b243efd"),
		new("item", "antiquity_plain_wooden_stool", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b93d0481b6bc1cf0e326a13cd2ef77e842081afecb484fd9b767c562e3fb558"),
		new("item", "antiquity_punic_crested_bronze_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "db202d596bb7a3875afc939247356414e5d5c1d1db66411ee805c00fe7c96841"),
	];
}
