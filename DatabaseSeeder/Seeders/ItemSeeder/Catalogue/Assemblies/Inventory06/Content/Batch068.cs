// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch068() =>
	[
		new("tag", "Eras / Historic", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "13d2f10604c08930e2b340435f1a275480696cd1bb7980008a635fc8344abf4e"),
		new("tag", "Food and Drink / Antiquity Food / Fermenting Foods", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e33c3c6e536134e86bdb584df3afefa5185e328ab4b540a5ec42ab31e3415979"),
		new("tag", "Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / Pulse", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "580b82cf5dfe130d5d100228406b03f2e283f27f48729b2e3f1d244767396b0f"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Offal", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbe07d93cf4b1f4b1cc725b780d8e91f09ef1d09c74716e1f822a0b6d137c783"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Oil", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49ddbf4e2bc8f694362888eebd1df47ca4421b56081cdbf37abcc2112e6c5497"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Family / Preserved", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "03efb244f4c9b865a1ffdc4fb98ab5e60c942fafbbc3bc0fbb8252febcaa3300"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Scope / Early Modern", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "afbb7b4195132ade2cd812e2c54999d39209ad564513bc0ac50ada2e53614cd8"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Social Register / Ordinary", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b9921d63c713784caaa39e7842361441ff6a3885ddd060de1b7f86e380636f6"),
		new("tag", "Food and Drink / Prepared Foods / Pre-Industrial Catalogue / Social Register / Rich", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "273dd7dc74bef82b9c3720a4fb36d7a3488b1c48f50d7b357dc3e2937ae9410b"),
		new("tag", "Food and Drink / Vessels", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6b56084d9b6f830b53626be71140161cebaf2172874a450a1fe8143b48c05fb"),
		new("tag", "Functions / Material Functions / Fire", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "043bb8f825d9d06b2698c7a443e4b5d745e8a16733102a2f4163f02be052a2ff"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Alkali Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cc3ecd3ea35a9c38b4d2fc1f3bbc950586a4e8cc9e4cbba36a00fa79fc599d7"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Bog Iron Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "270d39bf3518e44857b8db18147353a90ade9c3f036d49917c14cf5b8624c588"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Natron Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e1f2a96ad9971cfe5516689d5f21b05a49e911ce9111fb9e113abe1ec5f5c25"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Peat Bog Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac1449e347ef76e737867aeac34e1f6d9b15e0a11c0ceecc8580af849c603dca"),
		new("tag", "Functions / Material Functions / Primary Production / Primary Production Resource / Saltpeter Resource", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed226ffb276af317c2cc2f594ddb816cc9eda7f152eab0fe41ddd006959cfebe"),
		new("tag", "Functions / Tools / Foodmaking Tools / Fermentation Amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "599daf778667fcc43c69dea43c0901a2c56b1578cf19fe5c73c36fc851510b67"),
		new("tag", "Functions / Tools / Milling Tools", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d539d0e09d54acbb3199263871b0db28a9246690b7495202381e8882365adc4a"),
		new("tag", "Functions / Tools / Primary Production Tools / Leaching Tub", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f90cd07fa59611d1fb664f9f4faecee1bb257d30b71f1872c6515f7f295b2b4"),
		new("tag", "Functions / Tools / Primary Production Tools / Solar Salt Pan", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "de3fb6b0f959f75e0b4145f16be15a754fd79425530bb9a8baa867d28b30792b"),
		new("tag", "Functions / Tools / Saltworking Tool", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "71ec57691043448c52d760b5a1467ced7434e573cec994266d2da24407b39317"),
		new("tag", "Market / Transportation / Vehicles", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2171417ede876fc624c7740bcdf92e1e3d6efc67c3ed8d2f40f95837b6beefb"),
		new("tag", "Materials / Food Products", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c7db480eb89f00f13261801bf6769ee3bf2ddf3b9c90d218f619b91c9361819"),
		new("tag", "Materials / Food Products / Antiquity Food Commodities / Rotten Food Commodity", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6a29418a3ec1329a8d13949f1f4b7254008beed28fc926f1d4f90712ac7b443"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Family / Soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "339b4fd5eca72269e8bb69eccd809b97df33f79d8a90747d3cac91a8b99e27c0"),
		new("tag", "Materials / Food Products / Pre-Industrial Food Commodities / Raw Meat Commodity", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2de6e2efca23e2f4c35c792e9356c75c82b01249f1bec32c7a63de307002058e"),
		new("tag", "Materials / Natural Materials / Food / Oilseed Crop", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "27ac9840d2c21d8270938da7d306eb0adb3033b1a4a3225d20d0e54b14232691"),
		new("vehicle", "vehicle_antiquity_two_wheeled_handcart", "vehicles", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "05bcf270d14fc5523ae2c31e57d04025ebd57cc7b66fc0189af4bed3129386be"),
		new("vehicle", "vehicle_earlymodern_packet_ship", "vehicles", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8e2d056ee3946c69d360b31c6e42950a085a07bc347872d61cfcb26ea098d41"),
		new("vehicle", "vehicle_earlymodern_whaleboat", "vehicles", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddf8b09d809ca5d8439d17f7b5d61fa513ada64319b219e1dcdf7451b34c1340"),
		new("vehicle", "vehicle_medieval_longship", "vehicles", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4baf04520ee5f948668769e0a593134fb92caf4d599d77167ee108df4c829ab"),
		new("vehicle", "vehicle_medieval_timber_wagon", "vehicles", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5840dabaa81903d46d7004781604e017aa15cdb24c254e62bbecca20c3092812"),
		new("vehicle", "vehicle_revolution_factory_delivery_wagon", "vehicles", ["revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "e401b3d34148e1bd4470ab7619f1ec23c4f942bd55c970193bd42132ddc1f129"),
	];
}
