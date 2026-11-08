// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch067() =>
	[
		new("prog", "CrApKPreIndFooCatPro74TMil-3815", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39b718cafe43c3d170aba226315224e25f567bda6def578aa1e6dc0658a39276"),
		new("prog", "CrApKPriProHisComWor2TPai-1320", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4701858d5362986dd5451477ebf479ac829dcaa783d35e048989fff016af9d8"),
		new("prog", "CrApKRenFinIteCra81TGla-1115", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "95d85fc52994560620f9b7875c6ba6305d4b31c34f7a7fbe7413a57eee7fd09a"),
		new("prog", "CrApKRenFinIteCra81TLea-615", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7eb994752690521a0bf3426367036a035eed84ae213a64e9fe92677d638bc56a"),
		new("prog", "CrApKScySarTex14TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "667927d4999033cf4c2013e3c2cbe6d1e3ddf778b8bd19cdcf427f5a3dda4f3e"),
		new("prog", "CrUseKAnaHouCra43TSil-2440", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ec493df94b87aa1530da570f95224600e3e3cf20d616db1df6c2aafdbe09a43"),
		new("prog", "CrUseKAncAgrPro54TFar-1410", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bea6c956d3bb669d16f75ee703384a4520e7ddb878a045aa6173e9cbc717a128"),
		new("prog", "CrUseKAncGla18TGla-1115", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e59d33c47e0d61f132a2309bb7a8fa7b81ebda8480a62f5853421f829d2ad0fd"),
		new("prog", "CrUseKAncTabMak26TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3774400714171794b1e12b83e45963428837eaa50d3ae9a7934a41b58d0e7b78"),
		new("prog", "CrUseKAncTexPro3TSpi-161", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f90d06058d767fbc2e7aafc68e1778ca7986eceff7b84e04d224c8011e056a5f"),
		new("prog", "CrUseKAncToo17TBla-420", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b6ccd7d6290c9373ee4d05eb70b0bee0d1428f9f963d8f5d134c7f8a9e53f96"),
		new("prog", "CrUseKAncToo17TGla-1125", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b2178855d53d78f88b4211de91a0b858dc41814a9ce500132a283742f29fac7"),
		new("prog", "CrUseKCelTex7TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cff82581e426d16b1f0d777f5bcbd92731f600d9a15d07f542f037dfc72d0c2"),
		new("prog", "CrUseKEgyTex5TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4003bab61c0fb30f9dfaaaa79f22d42c50dc102cecdad88fd1d8093c5dfdf50"),
		new("prog", "CrUseKGerFoo60", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b4697ba0b911ffc841f266096f30900120536e6ab83a368c2c4cce5678ea161"),
		new("prog", "CrUseKGerTex8TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f708269a63795b02cf5a00de4d5ec51e03c80afea463645aab4b2e459423673"),
		new("prog", "CrUseKHelHouCra41TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0d55c1a5f2c064ae54cc95ff0d86a482acd4d98d25c8aaa312d8e74a0b06075"),
		new("prog", "CrUseKHelTex4TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3575c470e9f22f1a9f96ab8515f360658411b568203dedcffe6368a74e7b351c"),
		new("prog", "CrUseKKusTex9TTai-510", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "24294348c4ba8f5247374b3472168b9ca24b866fa9711c1bb89c1144a4802a4b"),
		new("prog", "CrUseKMedIndFou67TCar-320", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce01ab3de4276bde31f68526d15063b18bc94c90a7ded6039b6f625d44f307bb"),
		new("prog", "CrUseKMedIndFou67TGol-3525", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d60fde6de9f5aeef4e65bcad04c7a9cfde6d0b1da807a7ef35407e5d2bbed524"),
		new("prog", "CrUseKMedRepCra75TCar-310", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8bc882f3b2aa143efd89dab25acfd2d80813c71f762b7ef58e6ac1319900b567"),
		new("prog", "CrUseKPerHouCra51TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5326e733eb15eb6216877a32c961ca9c2c75d3d0cab265378839f0a97fde8cd5"),
		new("prog", "CrUseKPriProHisComWor2TSme-920", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "94b8b1327d573ed25b91df3f5ae31b0bb902be9353a9482cb4afb889cebfd133"),
		new("prog", "CrUseKPunHouCra46TGla-1135", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a99d3e560e47ad8ac423f5659b526c2f5cc89f1f1f60ccb7ff306efa0de5107e"),
		new("prog", "CrUseKPunTex10TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd9522a0c05fcb61a4a41a924b6f94f2e7359f2247cfc8d29e867bd175885059"),
		new("prog", "CrUseKScySarTex14TTai-545", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "645c85090926b91ce474bf41c70009cf86afa8b8a1b851f623d4e9e9c04037d3"),
		new("prog", "CrWhyKAnaTex13TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb0f1c8347ec052e58312ed0aa0a6484d0ea01a551e0fb476862b29aa43d14b0"),
		new("prog", "CrWhyKAncApi53TCar-325", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d002cdefd8bfe067e958002153232646eaa5196e06e94e8198eb3b1e80c4a5ea"),
		new("prog", "CrWhyKAncArm20TArm-1915", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d3ec49268828d4088db30a20a2b63a0f8ada8985ecca082cadc21f4e901c282"),
		new("prog", "CrWhyKAncArm20TLea-615", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae1804e6a640255dc6015a6ef24514516ee8520d4423259ef0591744de9f141f"),
		new("prog", "CrWhyKAncBas36TBas-2315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a885d39ce1e386b134447d16032bb451c0a4d859faa7d65d0b78aeaf2eada9b"),
		new("prog", "CrWhyKAncCerVes16TPot-215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "da3633d0dfc3f2a7d8f814175fdb7c1c25309f470f7d46b95eb3b3d3811f6dd2"),
		new("prog", "CrWhyKAncJewCra23TBon-2220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "77838c27ff715cc272c0f2ec0f29e4219b8f9bacf5f2c33cee3cb900bdb5afdd"),
		new("prog", "CrWhyKAncJewCra23TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "84db5221d4679a09e26bd0db51d9997c7f2a453aaf1492cf165454f00fc3d795"),
		new("prog", "CrWhyKAncMedTreSup30TMed-2620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c867896f40db75658cb2dd2c4b228fec4363617413dd9aed183cc9c317bddbb"),
		new("prog", "CrWhyKAncScrandCodBin29TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9225fa17adec5edc14b6c9b617f0cc6c6e968c54867a3edc8023b2b0f2750eb"),
		new("prog", "CrWhyKAncTexPro3TWea-1515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "131cb2c72e436ddfa635359b0aafb3a2410a1d345a60e0992366efc9755a20e2"),
		new("prog", "CrWhyKAncToo17TBla-418", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac0f2278a628e56fbd5d0e17659b5f7cbbab7130193cd3f2e8fbc712883b9be5"),
		new("prog", "CrWhyKAncWea19TWea-1810", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0876620c9cc91aec90aeeb25d6d10a9fec46ec097ce361cda3ef9300476c1b4"),
	];
}
