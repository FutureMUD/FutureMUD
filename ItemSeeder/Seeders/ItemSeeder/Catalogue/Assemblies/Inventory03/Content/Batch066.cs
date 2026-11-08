// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch066() =>
	[
		new("prog", "CrApKAncHouCra38TScr-2915", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "53f7478da25eab1d413b6e0afca183e4abf172b334b766e15738673e62641a3d"),
		new("prog", "CrApKAncHouCra38TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b86f037a389758a3202e485baafc94536870102459915e28272521ca7a4454ab"),
		new("prog", "CrApKAncLigandHea15TRop-2015", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b824e852837f1fd42621d5ab9889911d6df70ac131efa9065ebb061376e0ef1a"),
		new("prog", "CrApKAncScrandCodBin29TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fb22471bad059e9dcd4638b994791bb09b00af0083a0f15f2365759c7b0b59d"),
		new("prog", "CrApKAncToo17TCar-310", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cdf4267932a522cb731c7bd547b47bfdb97233248859500db2598411dace7ab"),
		new("prog", "CrApKAncToo17TPot-216", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "798f1130e9cb8e618253df40a64a838386b65bc5a749b9470ae8a1cc008c94c9"),
		new("prog", "CrApKAncWooandJoi34TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "57920312fe38dddaabe821efceed4f3a4ca19f3276c225a315e3e71ebec50067"),
		new("prog", "CrApKAntFoo55", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad26a72c13dd8fe832ab156e034652f8a5401e7d887ce9dc106000779af455f2"),
		new("prog", "CrApKCelTex7TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bba4f39103fec240a6391022fbb0ff4c950ed50b0e8a7cc38aa86ca8caf16788"),
		new("prog", "CrApKCelTex7TTai-545", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f5ef6a61e85830331c56882e2b6a02e9988b20662e5f79bb3fd1479fd44da12"),
		new("prog", "CrApKGerTex8TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a18d30f7a19645b8ae37cd8cbcfd55ef13f39c602cff71c0ceaed24888966b54"),
		new("prog", "CrApKGerTex8TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "86dfd5ea9e08a3a595052b9e719992de47df23ead2deae3390aa03608fcee87f"),
		new("prog", "CrApKMedFooPro73TBre-3920", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0bbc90df3d74aa8c0eec5463954e5025812cb7b8583d86f9de7697d3e15b15f5"),
		new("prog", "CrApKMedIndFou67TPar-3215", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9c27187d7740d9445cc2ad53ad454d7c6649d43058f985f7c62b48a97a69559"),
		new("prog", "CrApKMedMedandApoCra70TApo-3610", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ecd296a4831c3de436c9440fe285b57f36579f69181ce375e769d232f3197ea"),
		new("prog", "CrApKPriProHisComWor2TPot-220", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87925e7d31d51d2e3ac14a8ba9b6f86afa43cb2a55001de20bbaf6dff3d77052"),
		new("prog", "CrApKRenFinIteCra81TCar-315", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dccb6efa470c9a0339794576f7de72a4170615b312805a1868ad425b7c29c188"),
		new("prog", "CrApKRenandEarModJew80TGla-1115", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3878f702f696e896a38f22a44a5e5c8d96809d057f31166c5d1f061d2edaacdd"),
		new("prog", "CrApKRomHouCra50TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c193af82d2b660bbd6458839930191d46a93d34e7cf3560139535cee46e2376"),
		new("prog", "CrApKScySarHouCra47TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6355af87adfe605094c4429ab7ddf2f3b8e5f27209894c950801bc94b24e324b"),
		new("prog", "CrApKScySarHouCra47TSil-2440", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "656e79dce7207843744bf589fe1f4481da17ae8a9be3069913035903c3db0de1"),
		new("prog", "CrUseKAnaFoo65", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b29070fdec2f95b0e9e1361ee98c38f70c049d9798e3efbf4833b2d2b70293da"),
		new("prog", "CrUseKAncApi53TFar-1415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd9b9b43cdfa4bda4f769eba6c2d3e229abc4af9b067d78089ee4f625544798d"),
		new("prog", "CrUseKAncArm20TLea-615", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e2866a80afc5cb56b4603d8f4f67e3189597b23a5979168111b5d8140c41d7c"),
		new("prog", "CrUseKAncJewCra23TBla-420", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ef24ec5afe058f8b5599ef59ea12314acd0fa2933ff5ee1ca62e252d86249f8"),
		new("prog", "CrUseKAncJewCra23TBon-2215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb9ba0d1bd044c8282971490e4aaf2cb199e84a787941c2644f4b0cab4b51b43"),
		new("prog", "CrUseKAncLeaCon37TLea-620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ded5430d338f0e8bfb78f0055586cf0e405043be2372b4a472a4f67ab6ad044"),
		new("prog", "CrUseKAncStoBonandHorCar40TScr-2915", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1df25237e71e9469fdb8522320a18d296af2c8a48f7d1127820d98ec8b18f5b9"),
		new("prog", "CrUseKAncToo17TGla-1115", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "42fa706d8458f1ea12f4bc10ac7c95c2fcea1f312fe39659c0fee02fbb67f31f"),
		new("prog", "CrUseKAncToo17TScr-2916", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d563c53b4b46bf8e9eeabf72c957d34c21f9aef704862695e493a1b3718e96f5"),
		new("prog", "CrUseKAncWea19TRop-2015", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a7b6a81ef3b4e9345fa6b486d28fffba6f5393075d1a399f239e435bd517487"),
		new("prog", "CrUseKCelTex7TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1d58befe558ca4e46f0d6c3e0e958dc78fd9fd81dbab667ee4ad06791031aec"),
		new("prog", "CrUseKGerTex8TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "02bf02559b1082259c36287c72df767ac014788a3ff389f4a5d842630a04c6aa"),
		new("prog", "CrUseKHelTex4TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1145423fb12cae5c900034bc5f24fbe86a249f5eb8bc884d10ce1d5c9a961ec"),
		new("prog", "CrUseKHisWorFou1TTai-515", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "752332ee938809f08a90d732cf5887ae75c9b8b0db4cf9414a541f7bdfde0fd5"),
		new("prog", "CrUseKMedEquCra68TWea-1810", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab58317478a9a621ecd6782a9acc37cf35aa8b7ad6c23904dae89b114ab2d0ae"),
		new("prog", "CrUseKMedIndFou67TCar-310", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af21705ecdaca324486e57298202a353902a3ce87f10a18458eb9756cbf33dd1"),
		new("prog", "CrUseKMedIndFou67TPap-3325", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b394dd7e663fe3fd339cfcc8ef92ec6c9f2e2943b3bca2c206dbe79f57d81a4e"),
		new("prog", "CrUseKMedIndFou67TSme-915", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "09ede63634c3705c03e01e532a24b338223e76abf916ae7edffa62581d60da0d"),
		new("prog", "CrUseKPreIndFooCatPro74TCoo-715", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "23a8acc04b1fb4b901c1d9e274f06646d161b720227270ad6d7a4b5c1e941197"),
	];
}
