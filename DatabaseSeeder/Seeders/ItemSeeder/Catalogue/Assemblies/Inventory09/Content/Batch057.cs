// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch057() =>
	[
		new("item", "preindustrial_trade_teak_cotton_hamper", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfd7f8c5385020242e26350cdc3f1d5c03c7df9b7d87bed651e2a66870fc93ad"),
		new("item", "preindustrial_trade_terracotta_grain_jar", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a21cd14259274121faa94517ee9416d44375fdd0118d1be73c5ab4f36c2960b8"),
		new("item", "preindustrial_trade_wool_sorting_sack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "17d33737c6e4d776fceea2059864f53361757ceacf84b88948dda0f4e9e186f1"),
		new("item", "preindustrial_water_stone_public_well", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1c1f119d17d073f61864d8553699362ab9d8539552f04a7c562deee40a4b158"),
		new("item", "preindustrial_writing_parchment_deed_sheet", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b1bf126b90a7cabbaac465559795f86b3a4dcdec8c52616f7e2e227f17555eb"),
		new("item", "preindustrial_writing_rag_paper_petition_leaf", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a21b9abd23cbadbda381e62a7d7c7ad8641246446352dc807fc3f8904bacd51c"),
		new("item", "primary_production_copperas_deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "725ca6e41c9b25f330090dd753ce660b6a2146a1397cf71d0aedcb2e2b72aaa9"),
		new("item", "primary_production_salt_pan", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2286e91053012959f39e9fbe6b6e979c85db53fac9fb9ebd4b744e697a621d6d"),
		new("item", "primary_production_sledgehammer", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9fd14ea06947bbc3e910396655eecc4cae63215f6bb1ad656b9d71e2cda7cd53"),
		new("item", "renaissance_africancourt_longsleeve_sideslit_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3d620e8cc783858bfecf7eb922aaec09e747028b0007ae62a1048198e732cd7"),
		new("item", "renaissance_africancourt_panelled_danceskirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d03401bc6dda211caca660899cb6f9066d22077502a9f16f166dcdacb0f40690"),
		new("item", "renaissance_andean_braided_fibresandals", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "10c344d5f1ab5c6edaa9b8c0fdbf9990ed867fc1a1a7d0eb48d79e1ec1f0f35c"),
		new("item", "renaissance_andean_wrapped_fulllengthdress", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d4751c67dca4a4173429513b7f5d7161735031ac495acaad5fee8ea4e05f54f"),
		new("item", "renaissance_art_proportional_dividers", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6210981060714e640c9d9ac3fdf90bb7cf710d60a6702203d4433b36b225415f"),
		new("item", "renaissance_art_silverpoint_stylus", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "83b9864f56c43889ecfa3f3c6b7d2023cbd7b3df32105b282f5411091127b937"),
		new("item", "renaissance_container_aca_personal_portable_writing_box_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e48fd1ea0a002d736ce2bb517ad0d4bd2c85cede672375861f1c1aaf3ee98b3"),
		new("item", "renaissance_container_cen_domestic_condiment_box_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b22e132f86204a73176e69398e67ad82df814454a2e9808a7fbc3eb0254505ea"),
		new("item", "renaissance_container_eal_personal_shoulder_basket_04", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e3600323ec72d1d3d9a12f330aabcb01aef43f4aaa01c342780269c44b68e3c"),
		new("item", "renaissance_container_iba_domestic_table_service_box_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "73337a0deadb5376576836955442de8df181f08e285bd5950f0fd4eb80d98ff7"),
		new("item", "renaissance_container_iba_personal_portable_writing_box_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7530a9266f3bcc467b11d679c2081b6552b57d004c60fa93a7781fbd22222d41"),
		new("item", "renaissance_container_jpn_personal_fitted_belt_case_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "34727f0ef6d81183915751fc7572d67b2254ecde4392fda032602130c8299b1b"),
		new("item", "renaissance_container_mar_domestic_lidded_food_basket_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76fa4491558424a8486a8f05f98d4e45f8a78bba6e4eab9afe656e4a2783ca76"),
		new("item", "renaissance_container_mar_trade_segmented_packing_crate_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cd04c64a282be5a7d4b0de9232547a1bc60288f9b68e2c7c0d45acbe8ed756f"),
		new("item", "renaissance_container_nba_domestic_bread_basket_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "339a7cb9990b6efb9c8dac1ba36fb7aaec47cc0eeeeaa169ce00224d11bc5c8b"),
		new("item", "renaissance_container_nba_liquid_large_pitcher_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81ef03b11b001069077bbbdfbe3c1669953d3d7674ceff3f65c90b4f79d36014"),
		new("item", "renaissance_container_sai_trade_liquid_transport_rundlet_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7032040fd443ace01927df1b00d07128e69d884b6223e2e50533616570c4d9c"),
		new("item", "renaissance_domestic_andean_storehouse_wooden_platter", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82370df2bb9569d9e796751bcd45248b3610c9f4484abd1291010b3c0d58a386"),
		new("item", "renaissance_domestic_atlantic_contact_colonial_pitcher", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f652078d4c3e50f4509febbcc97976049adaedbf5d007e2ca80432a5a8b3bfb8"),
		new("item", "renaissance_domestic_atlantic_contact_mission_plate", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "342fcd44d17cd458b294154dcc15b16ebd5cb838d0135005ad2fafa784a8b0f9"),
		new("item", "renaissance_domestic_civic_guild_pewter_charger", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0de45b4e94c71c7bc7e797c9995a146416482318060ed9033d99dfb26db2121d"),
		new("item", "renaissance_domestic_sahel_redsea_indianocean_palm_fibre_basket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cba14d6f1ab7ffc812d63b3ab8d0de2bdfab062515a26adf9a4948faa52ef582"),
		new("item", "renaissance_domestic_southeast_asian_port_water_jar", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "08e3f1ed3b961de2cbdfe3d9d175abc3486b30bdb12bdd17b2d221b91b61ead3"),
		new("item", "renaissance_domestic_tavern_inn_wine_glass", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1125f5b41da9aaa64bbf382d379904ccb19b3ca78b20f48d96fbcd3d5aaf4b5a"),
		new("item", "renaissance_domestic_urban_kitchen_pitcher", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0081dc67fd2b79782a52ff42e1996c68bef6d11416c9766345af474888a41510"),
		new("item", "renaissance_door_aca_panel_door_421", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "666fad3aa7e86573c1492558c25c2173f74bb1d45b52ea02ba405789a6a63d9e"),
		new("item", "renaissance_door_and_grille_571", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "10e5798b10c902d612ad5e40d499956d648e5319597f3cb7b661528b19738bfb"),
		new("item", "renaissance_door_and_hasp_585", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "63a60e2f43b3fa49759b89f9e25ae79d9a348b276f54525f249afcbbc1871b12"),
		new("item", "renaissance_door_and_lattice_gate_567", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dcacdf0f3499acd9b219cc17afd1d5ec91ee82e613f6280b0709f1f35201233"),
		new("item", "renaissance_door_and_lockable_door_563", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "904ea754f9a6aef46ca30109a4d856c492143524424997ae2084b5407f738102"),
		new("item", "renaissance_door_and_panel_door_575", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d42f7fb85fd50d72ee2efa6547d7271876ddc8c637e7b375cb5fe1dce0ff828e"),
	];
}
