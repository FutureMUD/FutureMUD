// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch069() =>
	[
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Scope / Renaissance", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "381ba7012bc68e32b7b63528e082d1b486e9df7c9f224a04ea59e3f849a527ee"),
		new("tag", "Food and Drink / Raw Ingredients / Raw Fish Cut", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d1d65c3c7c3b7e4fcdf589c84335934c3497b77aca9166d4aa473a8e400ff46"),
		new("tag", "Food and Drink / Vessels / Beverage Serving Vessel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c0aad33b066827e7d0a3fd9f4418ae5eaeafc4fcc6be96d0de18215cf5085b1"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Galena Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0ff018aee73089e3e048e22a45bed4fff425df9f81c49233db9d68c5165a587"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Gold Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e27fdc7ea99988369d0688bf0b02d1c892a13fe5d787c46831db96c80fbfd625"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Iron Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e16274889bbeefcc3dc84a8d105624ea586da680c67e887db5bc7c16e23a5bbf"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Jade Greenstone Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4681ad6f834520f9c3d1ad135b9ab523026dbdc5ddf10c373f57cd9ea3428b9"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Malachite Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "708ce85bc5219560980574f95b7939401b3648bf31e654d4ed84cd82042d87c6"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Mineral Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "96f1511d5a0ff8ef3a9d857d8c32579d49b32f3160222177f20c932ff41edb21"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Visible Resource Deposit / Shell Lime Deposit", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "160f0972bdf4212d313520386fecd9758f6931363d0937945698017da148a1ba"),
		new("tag", "Functions / Security Tools", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "650de90977e23e5ffd0d162f783b5f3ba934f1b89687b3f64dcd4724182ff75b"),
		new("tag", "Functions / Separation / Shearing / Shears", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "adaaabd25eda4d5108463d6f1d877f4483753097178a0d4d86462dfb9b661c36"),
		new("tag", "Functions / Tools / Masonry Tool", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8d51b8820d026dc9cc9f18c0a3ddc23b577738326a2730539d030017963da7c"),
		new("tag", "Functions / Tools / Pottery Tools / Kiln", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16e81e1979533256f76361ca0f1bcbe5f9987fc223a93a60fa1954a56c6dcfd3"),
		new("tag", "Functions / Tools / Smelting Tool", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d70ff54e89c649884175b68281d1669b57e62f00f8c584037f4431b52968d6db"),
		new("tag", "Functions / Tools / Smelting Tools / Furnace Bellows", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "525dfc9512c1c28e04c70e42a762dcbb12d385cc24914e717f74b703aa565ed7"),
		new("tag", "Functions / Tools / Textilecraft Tools / Weaving Tools / Warp-Weighted Loom", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a23cd19c4c46f9f274ef210df3851eead925739d0d29e033a5f20f94bffed699"),
		new("tag", "Functions / Vehicles", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "900394f3051e4153c72406f9ef41587f1a33857cb3bf8d2f751881e345806a6f"),
		new("tag", "Materials / Food Products / Antiquity Food Commodities", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a979690fe3f17af0f60119bd4e4286f427e76e74d1ec86e4023f2bad8bf4ccfc"),
		new("tag", "Materials / Food Products / Antiquity Food Commodities / Finished Beverage Stock", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dce24f5234258f1cf4e0d13237949f2cf07b2abffd6b1f2c6211887bf9a06df6"),
		new("tag", "Materials / Food Products / Antiquity Food Commodities / Rendered Fat Commodity", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb0dc3ebf43cf5d88bae84d18aed7a10dc1ac8051a213176d6e6d8c62045c5f9"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Grain Drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c00ef1d21c608525bf0441fa9a985c9b08b013c0fb74ccdbf191a9d0f639be2"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Pulse", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "061b895cb66f03b0636997ede8363a6ed821cfa6d2f6e966dfd0f86b21d6243d"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a0fb7b31f7cd1e2c235ea8c22415d95bb0ee85d3ce9193b4b8d4c44f069b896"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Salted Fish Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e58208b4c2800639138a798aae0debfa90b8bc230781839673ce7e0c8de26d7d"),
		new("tag", "Materials / Natural Materials / Food / Fruit Must Crop", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e65060a65997963e183f83a814d4812399d7517f1a1fdba3feb1a3dc463630cf"),
		new("tag", "Materials / Natural Materials / Food / Pulse Crop", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5905a6f8e6f816729e6fe29df8b1a001babae1c91686a5d2ea013ec6e598d69"),
		new("vehicle", "vehicle_revolution_delivery_tricycle", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "01f5944f033cf0508e89b9db6e1421fadfe6e1f4efa769dda8efb215d4bb374d"),
		new("vehicle", "vehicle_revolution_hearse", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c5ceb18da332c0afeee95dcb905d92ac65c8872616b036f1f6cafe02d1a1286"),
		new("vehicle", "vehicle_revolution_lifeboat", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "62b4055fcbf865739664fa9c3d44a3868cf1a3d3c27d95d05cb2cbe47f37ab11"),
		new("vehicle", "vehicle_revolution_pilot_cutter", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "8934ccca8a36ff783b7724a7c8d75dccc9445ec44a7cf580703c9bf7ff0e81ae"),
	];
}
