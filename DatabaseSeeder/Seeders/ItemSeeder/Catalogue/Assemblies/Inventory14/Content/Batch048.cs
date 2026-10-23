// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory14Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch048() =>
	[
		new("item", "industrialised_weapons_police_military_0139_ventilated_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a6d587b337854d370dfe638ec1b4695f00452c3fdbd254358207a4f01a3dbd0"),
		new("item", "industrialised_weapons_police_military_0156_field_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "116082e92a073bde3ef980990dc42847071c62cec203d0d8f288dc0327424911"),
		new("item", "industrialised_weapons_police_military_0200_utility_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "62296758ae4e182a36e8c682123098ff986f20a468855c77d6669c64341a32d6"),
		new("item", "industrialised_weapons_police_military_0223_compact_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "59a997caca05e1505c89134cd95562bf907e50456928768535d2526243f56246"),
		new("item", "industrialised_weapons_police_military_0238_heavy_duty_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed1d974968abc200b580e4900aa97ba08dae284757db583710b6f3a920c1df81"),
		new("item", "industrialised_weapons_police_military_0242_folding_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "68a022b13f3f8ab24755872383c6e6db726a8b7c4d4de715ceb09a7d120df90c"),
		new("item", "industrialised_weapons_police_military_0254_portable_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "254bb25b7f6f04cddc7cdbc34074c9bc55e1beb5e360817234444d882f1b08fa"),
		new("item", "industrialised_weapons_police_military_0259_portable_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc74030e2a928bdf2af486a7dbb407a4df4f0349a28ac7f0019967a86918b764"),
		new("item", "industrialised_weapons_police_military_0263_workshop_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "98ebb778c186b7b4d8a9af1f11efea0bd79d826b2ca78a20e84aa0511b5eb1a7"),
		new("item", "jewellery_bronze_disc_browband", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "15e756adfac98dbe23423ba43d151a6f12600a7cd96d3a097a6ed68c891e76fa"),
		new("item", "jewellery_gold_amber_drop_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfe175d908c9a0175ba96d4ec8ffb1f42a930323d4f904dd6febfacc7beb3d68"),
		new("item", "jewellery_gold_carnelian_bracelets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "715b408b2a2056b9474f29c678b1543a6af4a7bc897a729020a1f21fc96cbaa6"),
		new("item", "jewellery_gold_serpent_bracelets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "795b5272f5a2b56507446b6d5ea73e3cdf295ae738b52d5612b3dcf8024c809c"),
		new("item", "jewellery_gold_studded_fibula", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b100357a9937bf260dfa5d7de7f4a6a3b2d6f0628d8eb818c09706e853b6a71"),
		new("item", "jewellery_shell_and_bone_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f6d87473b189ed53400422c86e5b721b074f56f425e248bf3cc154665ea46fd"),
		new("item", "medieval_annular_bronze_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "00612d5e9956b83706a6b35391001ec4ca3ac48f00a778997dd7279dde9f82b7"),
		new("item", "medieval_bronze_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "65d00852895dc1e606e6ca2fd060dd2cb5a41f6f944dacb6538f140dbd7c6d11"),
		new("item", "medieval_buddhist_memorial_tablet_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb8c6680939a753381dcc0a55f2401d33212030f4817c0622b43dde6826a8981"),
		new("item", "medieval_buddhist_nun_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d68a2d50dcc91d0fdaa12e587f7acf405cf607e89c9c71aaca50ae23c2d4b4f6"),
		new("item", "medieval_capetian_french_moulded_walnut_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9e5c3900f7bcd2a7e5ea458c5b7657e714b4eb24fee955791c4532c0506a3be"),
		new("item", "medieval_carolingian_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "985123f1ea7d6494a0d6d03ab10b7521af61d6df904baf51d182151f140db662"),
		new("item", "medieval_church_bronze_thurible", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "180405ed9b96b8a593e24a844f53987758ae1c11a92ca36334da8e9070f6775a"),
		new("item", "medieval_church_funeral_bier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ba7ec3833b55bedc05b5066188451655404cc0cbfa088298990e275423a7d0f"),
		new("item", "medieval_church_oak_pew", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0056dbb4b378996f6bf8b573f6f2edfe12e01c53b7128cc0d55f864e7a81b294"),
		new("item", "medieval_church_pulpit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8afaba858a49d2850d2d8b21fa26578b0051521b98bcdc7274ac24b56a1e75cb"),
		new("item", "medieval_clean_linen_coif", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3f9495d3ac1a9aa3c0b36e8b5a2a1b966ccb05d61f9cf52ed157555bffab689"),
		new("item", "medieval_cotton_sirwal", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cef81c65ebdf608efa28cc082383bc58fae1ffee11b39dab53907aebf1945b8"),
		new("item", "medieval_decor_ceramic_tile_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ec851f1b43134a92c6aa659996954ee6e5ab5e96a95c0a0a44a799064c7300c"),
		new("item", "medieval_decor_cotton_floor_spread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "30152ec93ab54c1971b4fbb5ee24f3f593fb175939fc97cc95482d8806d5d5f9"),
		new("item", "medieval_decor_islamicate_cedar_lattice_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d0182ba96e830fa4dfb449e902f32ee06305959dd26e604199bb226e54023ec"),
		new("item", "medieval_decor_linen_wall_curtain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "20f7be8378771ebc49c29980498be385d0b1b440408985e30f0632297dc86273"),
		new("item", "medieval_decor_mediterranean_marble_relief_plaque", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e33db5d2521c036a9a49205c9786ebf02a8d5edacb46ab0a3a3c456743ffe3f6"),
		new("item", "medieval_decor_round_floor_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "80b8eb2cfc3727f3dfbd3522a443e57a74482a64ab5a6d2e05ee4b8a1bdd6ab2"),
		new("item", "medieval_decor_south_asian_sandalwood_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c82d0b2a346ade0760121f7d3b022fa45e4e9abdba932f1fdf5e017958b4a2aa"),
		new("item", "medieval_decor_south_asian_silk_canopy_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cde2ed66ee1e24849b944b885f7cdfeee2ad2787a9fa7ea1b8febbc8670d753"),
		new("item", "medieval_decor_western_flowered_tapestry", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a738e2a7ed68f7693ffb557cbbe15443f44b13f7afad7ccd78205e24425eaae"),
		new("item", "medieval_decor_wool_bench_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f840c17f7a7877eb82c86501c5cdba4e08804dda73b4562adea991344c60c2bc"),
		new("item", "medieval_decor_woven_bed_coverlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f0fa7206a639b5665767e946fa5cea5b92c0b5a7792522d50ef8c882bb5c166"),
		new("item", "medieval_domestic_bamboo_scroll_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d331fc490e1f917acb39df41b43f8ab1376c0402ffad4dcf21d84fa6331513f"),
		new("item", "medieval_domestic_birch_scroll_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "df2fb22799a692f04a90b0ebb2134ab685eecde0339a271f2b8f14ca1e23be6d"),
	];
}
