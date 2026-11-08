// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch071() =>
	[
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / Syrup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8974955534b4613f35a6a1f1237115b8d41049588c683b5717a5afd059c72dc3"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed4272aac1caa19c613b50eb0f8703344224515c63fb5aaa27e43c176f548367"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe6231abc8b8601c08547bdb43f23d66f9a8493bb1a5966a5d219e09e38fa9e6"),
		new("tag", "Functions / Household Items / Leisure Goods", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0fe4f927004201513d30e35858fa3308b0f46e023f1111b1678e5aa535ec47d"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Pigment Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2d5339d1975e95b5e9fa95c3f4dd9e4422f4d27a0bdbd11c9754c65326881e7"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Salt Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5119985748d2f8f814358a5aa56af0e947e1c19f46b1140034e756fb147bab2"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Visible Resource Deposit / Chalcopyrite Deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c64740a1165d28c6e93229c4d56ce6ac9ccc92982c6b8f6bee7126b92ba4cc8"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Visible Resource Deposit / Placer Gold Deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75a58788197e8b9aa20622786e4c98c210758bc4461f2a1c1fa97eff2cc8df54"),
		new("tag", "Functions / Military Equipment", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "859726dc3efafcb19157a3e4d4d7ecbcc11b12385b724952320f5a64768c9c2e"),
		new("tag", "Functions / Tools / Foodmaking Tools / Threshing Flail", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9125f279b53180b395dab58717cc2240623ca9eb0d089cc986d243ffe598849b"),
		new("tag", "Functions / Tools / Metalworking Tools", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16fc372e649d01ad1bce31ce3ea37fdeffd7356e57c76d28aec956c2da6fb12f"),
		new("tag", "Functions / Tools / Primary Production Tools / Ore Washing Trough", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "43ed8a7ce0d8d9f1364136b51454f901769a48c5bcd5fb0a520f67872638f2a7"),
		new("tag", "Functions / Tools / Smelting Tools / Crucible Tongs", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e2eafde76cb015fca109aa70561d249181aed05e42cf3b924dac4be74ecdd6f"),
		new("tag", "Functions / Vehicles / Vehicle Equipment / Hitching Equipment", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "63080758b61684ee7029a18d1675f945bff71ad8cdd3c7a006cf15f2a739c43a"),
		new("tag", "Market / Transportation / Vehicles / Aquatic Vehicles", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cb112b5bb8a9ad4b235f8e5469c443c587a6e37fa228a61653d3eff1e0c9680"),
		new("tag", "Materials / Food Products / Antiquity Food Commodities / Brined Fruit Commodity", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "792221c7cadebfd2f927eb1dfe3bb7e3826cbd35918dbff9f49fea23f647eaae"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Bran Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "96d70afabe0826d3f3efcb6dea39b331d43a829a57397625aa37a3160944d8dd"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Dough Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "26cfa98c1fed5249a7e22ade1a42a02142c55a223793166b9b581600d0851b4f"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Condiment", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18595088fa490b4f3f1ded22918f28c0550c4b00c0f51ed614a6a3f89c2d426a"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Spirit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc74374779b4db501ee220b52bbca5013f29d970bc7e90a68e91d1b5df10644c"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Fruit Must Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea3e08000fe782d5fe3e939d32358f815a17f6108818ccc094f51183812719e4"),
		new("tag", "Materials / Natural Materials / Food", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "57e2ff18ca82808cefe4b2dd0a5a11abe4797e8bc124b10423291203cb3fafd2"),
		new("vehicle", "vehicle_antiquity_river_galley", "vehicles", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "158d4d94eb18618f360c725ea4772ecc37083463e13276376d47378d5a9a8863"),
	];
}
