// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch068() =>
	[
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_dried_yoghurt_cakes", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a541fcb66271781005bc291778570e735db12c89277872b8ebe48b923ad5df5d"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_rye_and_cabbage_soup", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "698245c49a67058effaccacd0b0a0e4d2e6866650945ceff8df06c3bce44e093"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Sweet_preindustrial_food_apple_fruit_paste", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "56467406b7a0e50206b2e4d359402ddd2ef469d41218446db10249016fa1b038"),
		new("prog", "ItemSeederPreIndustrialFood_Shared_Prepared_Sweet_preindustrial_food_berry_barley_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8b43a3247b4f61094a7c90289dcd6bdb2df97a709dab15dafce963f1fded1d2"),
		new("tag", "Era / Modern Era", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "a953d059a2843dd8c3b8e8ae77062df1428d77e51830dfc829f48a2b607b9c5e"),
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / Oil", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "45b7c496895d85aedc129bf52016ea93e071ba10528cea8db79bf41e1abf1567"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ee0f0b4527f67dbd5103561b0f5cec774e0327e83ee7579cdf9575b793b811a"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Intermediate", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d50109e284b4ed8de983733385846e891aebabb28b9a9af12cbee12cd2e5e1ae"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Scope / Medieval", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "98d9321e8060d51ed2128285dc8bc415b3819c12f8917f79f25dd7fa01eb76ad"),
		new("tag", "Functions / Civic Fixtures", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "85abfa0e746ba10d376a47a3ee6713d573716f1d6f70345d30291782feee0fc7"),
		new("tag", "Functions / Household Items / Household Religious Items", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6b07e6739b76c2f1c0f52149db921f5b095fae010e4ef2955a6bff415011665"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Copper Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6de88f3e706db69f6e357630dcafaf50f3a21e494f3fd37f37a2a94b04e219a"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Stone Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "631ca35521a9c7eb0d4f2c30c72b2796b1f51c82fbd286d75ec319866897ef81"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Visible Resource Deposit / Vitriol Copperas Deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16e83774ac1d6a5baf8dd7ac8f78c3173bf3bf6a0da1aade5779883f145b8e4c"),
		new("tag", "Functions / Tools / Foodmaking Tools / Bake Oven", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "472e5169e5a72d6e093de23daf134d1cb509c054b72c92519fa3d817bd59899c"),
		new("tag", "Functions / Tools / Kiln Tool", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "56429ea6d80843b00b5af445319ad16558b0d62be647edbf40cde8f8d29ce5a5"),
		new("tag", "Functions / Tools / Smelting Tools", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16409f817a649c4165ca7e9ac8199b3a089fb019716531a0d5d7eccc200529aa"),
		new("tag", "Functions / Tools / Textilecraft Tools / Dyeing Tools / Dye Vat", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e671472cf452d3404afa77e72b4ce756b0510dbe655e1c195cdf7c5cbaee183"),
		new("tag", "Functions / Tools / Textilecraft Tools / Weaving Tools / Hand Loom", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3aff63c12f7bbc086ae0fbf883d6eb8217ae1d9893e8c72248b8f22335c1694f"),
		new("tag", "Functions / Vehicles / Vehicle Equipment", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7445a84ee21a7045373ae3ddea3456658f5f4671a84bedb5398483fdf0ac310"),
		new("tag", "Functions / Vehicles / Vehicle Equipment / Propulsion Equipment", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "57dd152cb8c64512b14edcb82593483d4bd0ab3fce3429cc7c3f86eea79506f7"),
		new("tag", "Market / Household Goods / Standard Furniture", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "589aa61b2daa5121a09cdf986b33991c33537a419cb361e2e955f84c878353af"),
		new("tag", "Market / Military Goods", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6b183d7a43f0eed56246b1312d3712e09a82f2529027fd8d4cbbbffe710d14d"),
		new("tag", "Market / Transportation / Vehicles / Terrestrial Vehicles", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "6276a3dbda0fbd77c00f45fe528b053e979d8f21f594fbad14f3811d772358bf"),
		new("tag", "Materials", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fee80dbc92d32fd857d5afeb54bc5e326134f90e6a8f61648e4d6dbfae326b95"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Dumpling", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c78835aedf0b643112f9a759bfb5a71726992603cc27463b1d62a5afc37f3c76"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Fruit Drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff1653dfe0f45a150f662536c581adc0532d7e27de1e74113d3c96fd0f5d613b"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Offal", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "32620a38e283e9dee115f61b2e20c9439c1368e2441b7f19ed431c8c15da6d58"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Shellfish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "667259bd3539c5962aa4ff2a891b99b34161e92713cfdd063f936b9d31b1d593"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Malted Grain Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "04bd6f995cf9a7b4983667b2de4c642e220372214d86fb992fe31ee5179f9cc0"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Wort Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e908a11b25e192ee656cc7172d84023f9ee343199bceb94b2a4c89fec14a9327"),
		new("tag", "Materials / Liquids / Ritual Offerings / Lamp Oil", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "88f08d6073e30f8cc9363c570ebad42aaa16f514e9a563148228a7949d5a4c49"),
		new("vehicle", "vehicle_medieval_trading_cog", "vehicles", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6634e5b843e824103b2f6a9dbff3c8aa817fd03e69d447326b0de36684516347"),
		new("vehicle", "vehicle_preindustrial_river_punt", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5841dc40a4fe895d9137d41e54116b97d5c9077a8093553af94ac032c1fb010"),
		new("vehicle", "vehicle_renaissance_lateen_pinnace", "vehicles", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e69768fe1aceeca1dd60e8b5fc8ce5cd38164dc4086bd1c8ab9c4309738e0553"),
		new("vehicle", "vehicle_renaissance_ship_launch", "vehicles", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "693543f1b18bdc27e945126c7216a9765249f2faa34781628582046989d2eb2d"),
		new("vehicle", "vehicle_revolution_canal_narrowboat", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "52bde0d241e4fd35c2a226bef1f4b3f2fd3528fc3b2b87831e2363fba1cf642d"),
		new("vehicle", "vehicle_revolution_canal_skiff", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2b23196a178f87d5705901131b631802df96825bd9e82b17cdbd6723d78473c"),
	];
}
