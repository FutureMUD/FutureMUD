// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch056() =>
	[
		new("item", "preindustrial_food_boiled_head_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be4882f1e57d8a3aaebfa2197cda37f27efff082db619e46d4240eb198db858d"),
		new("item", "preindustrial_food_boiled_heart", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "17d11c36e1a4b7deef6ebe0b4376c9af7467489fa8a3a6830eaa17de96775392"),
		new("item", "preindustrial_food_boiled_intestines", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "34a584fbda92cc02a2183e4fc62948f32155627b1322de35e2a2ddbc2933bca1"),
		new("item", "preindustrial_food_boiled_venison", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fdde8e6034abd5a8ecd17a8936e258e11bc29ef82348141b2ab009b8faa7f18"),
		new("item", "preindustrial_food_braised_meat_skin", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d2fb6215f9b1153f85929bb824ef047db584bacc0c819e2ea643ce8192c3585"),
		new("item", "preindustrial_food_bread_and_cheese_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "14953c9fc5ea0e3ce5aabedd244890eac820f171f131e8f0a4ce28d723d8a311"),
		new("item", "preindustrial_food_bread_stuffed_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "37bb86993ce68cf85d2e87cab452043f3fff993d61d50b8d8b290031b1ce06e8"),
		new("item", "preindustrial_food_broad_cut_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "51e0b835cd53faee3bc086c0f5c6d1259726a5c9564737582b31560a76aa5e3b"),
		new("item", "preindustrial_food_broth_sopped_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "145e96dc34484297dced7d3680454c93dfcfeec6976c100df1f58c39b78bf185"),
		new("item", "preindustrial_food_burnt_scrap_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "872f29825e6811e42a736e93018f010b965be6d1d85be1ed7acb6accb3452250"),
		new("item", "preindustrial_food_butter_basted_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc095abab4da90e1e3fae3f5fe3604f6884a0c15872a198ea5a82cd480c20617"),
		new("item", "preindustrial_food_cabbage_braised_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0eadb53eea29325369aca4eea6394ab247fae2a23bbfd46b5cc6157e81a5dfeb"),
		new("item", "preindustrial_food_cabbage_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "747deef3899aea85e0c1897e95248478b74d64b00d6c5aacd2d6646a1780c726"),
		new("item", "preindustrial_food_cabbage_grain_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f2e71b7a3afba301a5de5600b665d6f60f2bf535f3d2c67fa2cdc66b192ceae"),
		new("item", "preindustrial_food_cabbage_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cc86b7b67e495688037711d4193c79bc26d9289c894c89af3263b6005660674"),
		new("item", "preindustrial_food_cacao_spice_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0d237faa8ce23d0d917917bdd75aa3bfc206a5e32c48b37c15c63e50494fbb2"),
		new("item", "preindustrial_food_candied_ginger_fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e47b7a54f565f87b7b2075261c64edceadd1c761c33f5f4d10483767ebf4589"),
		new("item", "preindustrial_food_carrot_and_pea_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "212baeb4a64968b6a321d7714a381ec1843244e7a76d23394f068ad671d5b0a4"),
		new("item", "preindustrial_food_cheese_in_brine", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a0fb35c68f5c7109a23006cac9bf2054ebfbcc96e8acfe3471c9cc71b4277e8"),
		new("item", "preindustrial_food_chestnut_honey_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6249f10663c5ce9a41a0942e47159184f12a56cdb1884f77f7d212d759e59f4d"),
		new("item", "preindustrial_food_chickpea_cabbage_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ee52c1c5da8f9fe0d8434810716e875cd3c2647be171e1c9f8005426ea7935e"),
		new("item", "preindustrial_food_crisp_lentil_grain_crepe", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d310b89b56dbd1098bf989f617cfbcd2ffd2cd555935b8f47b253090e4b242c"),
		new("item", "preindustrial_food_cross_marked_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "71907b3c15189d87fba6a774dd7de64e9a5e720bd8c6d0b22896a5b63543f0b7"),
		new("item", "preindustrial_food_crushed_shellfish_crumb_mix", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e27e6c740352b9a81b4a0dc4aab8d68234bea1522e127ada243734e74313252"),
		new("item", "preindustrial_food_cured_fatback", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9cb48fdc3dfe065774be22bde074d6af10706d62dd7f6d115fc30f74967bc93"),
		new("item", "preindustrial_food_dried_leek_ribbons", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8be2b54e5784b11223259912ca6120b29dc114a179537ce20862855eef0fdf1f"),
		new("item", "preindustrial_food_dried_mutton_strips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76f7282a615981919d6110ba57afb9ef902bc365a5d7e4529147a8c5c41cf126"),
		new("item", "preindustrial_food_egg_and_bread_sops", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e15ae55771ada9ba9d1d7594caa1c3150a7c50054e3ef5fee9066f5180bd2e7b"),
		new("item", "preindustrial_food_egg_barley_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6e357d90b0fd677fffd0675e3b141632a5ee3488c825889b3f57770c42a397a"),
		new("item", "preindustrial_food_egg_glazed_feast_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca60c2811df414dad08183b02498eda610313d16d46313cd738d0d41aeb16b2b"),
		new("item", "preindustrial_food_famine_husk_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "219545197ea502689bba5bfe98c260c000560097db64bd2f7990428c526ae317"),
		new("item", "preindustrial_food_feast_grain_platter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a721a97c44f02253431a84280323ab4aaf997ef5e1e12275bbb7a9bc98e38b6"),
		new("item", "preindustrial_food_fig_grain_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01b8c9717577153db9b9444653df15f9c39b5f1d61c8a725da240864133f546e"),
		new("item", "preindustrial_food_fish_mushroom_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d0d98662acb6b047ae410f01e5a2486f753867eab9de579bf1f71371ad7a5a3"),
		new("item", "preindustrial_food_fish_omelette", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c75b1156ae4c3a8bab735e85a62be7dec7fdc6b910d1c0296970907149c9b19"),
		new("item", "preindustrial_food_fish_rice_congee", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "308fab069507644ff5de5c62c2730ffdc0317c51e197343823eeed147e6a7e94"),
		new("item", "preindustrial_food_flattened_dried_sausage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8140b62a06f25af32467a7a86a216c3364c75dfb7a2a9d96958dd7e83af24f34"),
		new("item", "preindustrial_food_flour_dusted_fried_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f6210ceab7d7165cbfee006b9a9b51fbf753aa8f29589871a65078f342b4418"),
		new("item", "preindustrial_food_fowl_meatballs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "37e6bc9f287d575ed74128e83823b2e3c94049e3704909783d08d9cdb4df3090"),
		new("item", "preindustrial_food_fresh_berry_bowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dc369b6f7331c4d74f85abb54248dbef98334f1eb2ebe2f98a97d50f46c52c0"),
	];
}
