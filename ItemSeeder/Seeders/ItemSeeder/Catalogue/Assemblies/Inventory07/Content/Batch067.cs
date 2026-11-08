// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch067() =>
	[
		new("prog", "CrApKAncSurIns33TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "88c413a48736986e4a6e6f1822f443595e2eb8ecb1a3683080b02065929fe11b"),
		new("prog", "CrApKAncTexPro3TFar-141", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d4cd7ad383a64374011cb984e7cd6bb61a7066478db89b4f166c3521354b144"),
		new("prog", "CrApKAncToo17TLea-615", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ba717170e69393f972d5e29aef0cbb2ad9585c14b4cd38dd701926b05225c76"),
		new("prog", "CrApKAncWooandJoi34TCar-320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f440ae1e81b693fca1b5e8dd7e5a815cd571fb2cd517098636dcc7b690d2292"),
		new("prog", "CrApKCelHouCra48TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "df9c59cb01e2d1ed45e00a671cc14e9fb52900261912044b6bed326c1ca4424e"),
		new("prog", "CrApKCelHouCra48TScr-2930", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3220bfb270c4320cf9df159db9aa965dcbe1c62aa2cad0d6525c6735fd82c372"),
		new("prog", "CrApKEgyFoo57", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc38f12b196a0e0c1e5725ec4606c996d5414e15fbec69eb4ca5fa53d0a91149"),
		new("prog", "CrApKEtrTex12TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f30440c3281042d934e2f7807da4b93eb7d6e6562872fc682532e5cd90e4e5f9"),
		new("prog", "CrApKEtrTex12TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8f9fa587634a294fc38a04df1dc0dfda6fe7ffccef842399ed31e373bdd0a64"),
		new("prog", "CrApKGunManCorBlaPow77TPow-4225", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e2e2389551741e23440f09f1b3bd09a02a8b36c769912f1addfbe4f67a26a61"),
		new("prog", "CrApKHisWorFou1TCar-315", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4554b9a6bbf3a9da467b7f3897cdc78cc56747e3b47a00548f70d11a396fa1cf"),
		new("prog", "CrApKKusTex9TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce790cda77aad2d6f2147a6111e4e00fe0009412281e2242b8e084ea697b41cf"),
		new("prog", "CrApKKusTex9TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "49c7749dd1821cbc1289d84b2b3b6244b9c0afbd76c5fa278affa0d955fc575b"),
		new("prog", "CrApKMedFooPro73TBla-420", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f69ad01fd4e0fe4e99ca65687e1a9f03842d30f53cc11b9210c93ebb053ea6b"),
		new("prog", "CrApKMedFooPro73TBre-3915", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "31cbb145272ea7194a8a8f8d5f6e0fd78fb80243dbc1258f3185f45c93b7319f"),
		new("prog", "CrApKMedFooPro73TCoo-2725", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1457ad8c6be7ecf3d78853964d8c24c2b31145e44021508681413055c18146a8"),
		new("prog", "CrApKPreIndFooCatPro74TBak-4110", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd7a742d1a670ab63b8254e9f40d1928fc03d3c5c9274165bd7a2495011a77dc"),
		new("prog", "CrApKPreIndFooCatPro74TMil-3810", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2debd3e6e5aa485eda8b3cb1453f769ea73dae964f5d2dc12d912077f64c8b0"),
		new("prog", "CrApKPriProHisComWor2TPot-215", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdadfd7a7c6eaa21b1c5f67e0d574f7c49b777d6ea2215c637509391bf2d7c08"),
		new("prog", "CrApKPriProHisComWor2TSur-1215", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5385f9abab263376ffc61542377a34042c6c28691cc2990cca80a504de254269"),
		new("prog", "CrApKPunFoo62", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f15fe19144de00dbbce0255e671c302772de2c4abb1c9a95edfff2e352d96a7c"),
		new("prog", "CrApKRenandEarModJew80TGem-2515", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "208d2985d25b5305caa417a0e13b1928008be5b94fa16cb9b07d99f678f54798"),
		new("prog", "CrApKScySarTex14TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c22490f9ececc0b86dd86bed48bc0828bd3bb61f8e343ad2ccb012348d73e91b"),
		new("prog", "CrUseKAncCoo35TCoo-2730", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0afdec937b876a0107aa877a4264b6f0f590f0cada05805f178dd3ee0659b6d3"),
		new("prog", "CrUseKAncHouCra38TLac-3015", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "07b906775ea2bc57d44d2d0c7f398469ac4ed52f5ef9c56cde7840ed3fdaffb6"),
		new("prog", "CrUseKAncJewCra23TBla-415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7181e6632fc30213fe081d9424484d4ff343137fc6b3c9115f02be607ef5ab1"),
		new("prog", "CrUseKAncJewCra23TBon-2220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5c91693ea911bab307ec8265a4ba7db4ae0b3c0513097a2df2ba4d677175cd2"),
		new("prog", "CrUseKAncLeaCon37TLea-615", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "69c14d81642c1b510986673497364dfd3174980d933fd836ccec1a5375c634e9"),
		new("prog", "CrUseKAncTexPro3TDye-1710", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad6289d807824ac6a201047a0d10f79469586e66ea0c1c8badd0c92da1f3b712"),
		new("prog", "CrUseKAncToo17TPot-225", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "792e662835fc3abbd66d76bcddfea5e7e7bbf9aea3edba7c6d7fda03e4881cec"),
		new("prog", "CrUseKEgyHouCra42TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "34dfe193a98efffda803c3f25cf7fee1d652feb9a69de3b4674453e4227c765a"),
		new("prog", "CrUseKEgyTex5TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ee391630ebde0df64b864a87aea618e336cffb7fdfae33518144965118dc65b"),
		new("prog", "CrUseKEgyTex5TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8921db02cc54ce8c5158de8ac4b06f0fc967a74cdf9374bd77e98ce935139d37"),
		new("prog", "CrUseKGerTex8TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca08c8b4dc5b11166c0081619db0d088fdb8d29195012256be6bdbe582a01b9d"),
		new("prog", "CrUseKHelHouCra41TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "184eecea121bdd91159475ecd40f97e115f2e1b83111f7acae6ffa6181f7fdb8"),
		new("prog", "CrUseKHisWorFou1TCoo-710", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "03458d48f0b70cdd0a6c75d9ca9203d9d46171647e7343b9a28b6e33ef0b2cb6"),
		new("prog", "CrUseKKusTex9TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e4d3eacadeae861b47edfd1870d47963f250ab0ddadeb3f1037d65b1c6da997"),
		new("prog", "CrUseKMedIndFou67TLea-615", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "42ba53ba3c2b79ab08f0c8863eef7dbc348f40aaaa9ee19900ff329d60c26c71"),
		new("prog", "CrUseKMedIndFou67TSpi-1610", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4bcb30e89e4494c48a2ba7d3c26de38fb6f3260e6a65be3945ab59246eebfa74"),
		new("prog", "CrUseKPriProHisComWor2TSme-925", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "65f92ff8d3145029f2b3e2adf178a45a2ffcf5ff5df8fb43af31cd4690e8b72f"),
	];
}
