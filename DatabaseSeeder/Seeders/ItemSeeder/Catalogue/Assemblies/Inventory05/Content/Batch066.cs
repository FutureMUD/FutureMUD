// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch066() =>
	[
		new("outfit", "renaissance_military_mount_outfit_polish_hungarian_hussar", "renaissance", ["renaissance"], ["item:renaissance_military_barding_caparison_brigandine", "item:renaissance_military_barding_croupiere", "item:renaissance_military_barding_flanchards", "item:renaissance_military_hussar_chanfron", "item:renaissance_military_hussar_peytral"], ItemSeederOwnershipPolicy.StockAggregate, "f45590eb4918b8724a84ea1592bee4be754ac2679740f14871acfad52283c953"),
		new("outfit", "renaissance_outfit_andean_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_andean_braided_fibresandals", "item:renaissance_andean_fringed_camelidshawl", "item:renaissance_andean_straight_sleevelesstunic", "item:renaissance_andean_woven_earflapcap"], ItemSeederOwnershipPolicy.StockAggregate, "22a1b4b11e7060092dc931846d3c737a3da103f87744eb347b0ec6fd508a6a28"),
		new("outfit", "renaissance_outfit_central_european_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_frontier_fur_brimmed_cap", "item:renaissance_frontier_wide_hungarian_trousers", "item:renaissance_shared_clothing_ankle_boots", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_western_long_skirted_jerkin", "item:renaissance_western_open_neck_work_shirt"], ItemSeederOwnershipPolicy.StockAggregate, "ce7c0e7d93140c19f2ce2de626032f37eba6e8ede4d18c9596952dcbf81b70dc"),
		new("outfit", "renaissance_outfit_maritime_seasia_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_seasia_short_collarless_jacket", "item:renaissance_seasia_short_maritime_trousers", "item:renaissance_shared_clothing_cloth_headwrap", "item:renaissance_shared_clothing_rain_cape", "item:renaissance_shared_clothing_textile_sandals"], ItemSeederOwnershipPolicy.StockAggregate, "e40f52aa21a8255b35ef35675abd0c3af829022b50583b870e62666e00074519"),
		new("outfit", "renaissance_outfit_overlay_field_labourer", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_straw_brimmed_hat", "item:renaissance_western_dress_apron", "item:renaissance_western_hobnailed_fieldshoes", "item:renaissance_western_open_neck_work_shirt", "item:renaissance_western_work_smock"], ItemSeederOwnershipPolicy.StockAggregate, "2d8c1e5f78e89cab2c46fa425ea5f5c61f80b294e1d53ccd7223af9cb7f26077"),
		new("outfit", "renaissance_outfit_sahelian_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_sahel_broad_rectangular_robe", "item:renaissance_sahel_narrow_longtunic", "item:renaissance_sahel_veryfull_trousers", "item:renaissance_shared_clothing_leather_sandals", "item:renaissance_shared_clothing_soft_brimless_cap"], ItemSeederOwnershipPolicy.StockAggregate, "1c3e33755b431380603678b6ad0ff14894c6f8fc0dd88518b56c330d8dbb21b0"),
		new("prog", "CrApKAncHouCra38TLac-3015", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab12f7b01215f7a8f62bc561416467f71072a28fb57fe308d4767c78726ed535"),
		new("prog", "CrApKAncInkandPigMak27TPot-215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b7afcdc1be07cd4d9a42fd3a1269b548a56bd166c1ebc69dd55798d2fd1e5c9"),
		new("prog", "CrApKAncJewCra23TBla-415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e5556ea59a5890886179d3793a26d05cc38125735018cce53a75d1a64d3e671"),
		new("prog", "CrApKAncJewCra23TBon-2215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7b64d9d0fa4f61c392489db5601010bf4b77d05f15dd1f7b5bc0fea9aa50df9"),
		new("prog", "CrApKAncToo17TCar-316", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "83668a1597fc223776046bc1c5cc7a35fdd9fd59c59c34cd272a44a0c4286877"),
		new("prog", "CrApKAncToo17TScr-2916", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "362271982eb165960c88dbbd07568b8b565d52bdd9c274195a8ea0fa327b9c96"),
		new("prog", "CrApKCelHouCra48TCar-325", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "13fb938cba9a15e247a2a60db8e4a15d53ea4ad0e774a634e36330c96801fc45"),
		new("prog", "CrApKEgyHouCra42TMas-1030", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bfa37761202c579da3e6ff1c4044c1e9834b15c050d8504c16454356b923cdf"),
		new("prog", "CrApKGerTex8TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fefae335857bf003a79fc49140b7dc25646da8f4423d00107f1c058916c44695"),
		new("prog", "CrApKHisWorFou1TPot-215", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0562862cd548335fc8c870980bba6b17b4003fcf480a601eeea1ff7b5df22a9"),
		new("prog", "CrApKMedIndFou67TCar-310", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a318378cd027c08cbd440a20fe57376382e1e0bce13d937277e14549181ddef"),
		new("prog", "CrApKMedIndFou67TSme-915", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf706f4e78d791dc378626eb48b48a0b3a1684331319c3b1b39da4616b5f6ce0"),
		new("prog", "CrApKPriProHisComWor2TGla-1125", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a5ec90e0fce3fc42b5a38cec1950e7dfce593251d38973d7389a47be299626b"),
		new("prog", "CrApKPriProHisComWor2TSme-925", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a732db40048d75c1dfff1b02b2df1feaa1324e9dd52ab70a82d19ed50f7498b6"),
		new("prog", "CrApKPunTex10TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcc7d81208dfa2d1ff5944f6738fdfdc6b45fe1ae0a8809ee4f1b52f2d47c481"),
		new("prog", "CrApKRenFinIteCra81TTai-515", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7713d8671d93b00dd1e95f71040c7b8e96f568b1f5c75d83836ca4abd3c6613c"),
		new("prog", "CrApKRomTex6TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4e0014c183fe8037c112688b85852574d83c29043322f03371dee1e11d5089a"),
		new("prog", "CrApKScySarHouCra47TLea-620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f759544d1a083b332e4f8275f64f7fd6766688b9166bbb855ed192554bc13d24"),
		new("prog", "CrUseKAnaTex13TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "49c25501032b1bbf728c3a8058e51c5658168bed4875416d6f5811a25f953a67"),
		new("prog", "CrUseKAncArm20TArm-1915", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8886d38f6e469ba52dee24db212a7933fc501acac8a02c59ff8774e31cfb9170"),
		new("prog", "CrUseKAncHidandLea52TLea-61", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e8d86556e86883e0303eff5f2c09dd720081d139500c656274f6aa05fa2e80a"),
		new("prog", "CrUseKAncJewCra23TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "64fb2a29fcd39c5aee7440192e03490925d94d5351b4894252def19e75987c96"),
		new("prog", "CrUseKAncJewCra23TGla-1125", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d8ef2cc9da035679df391ac12cdc49401db6b18077b7081bfdc5685b1def4b6"),
		new("prog", "CrUseKAncMedTreSup30TMed-2615", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f967c08b5e8d6909d9057dae79b3f93f28946d9bf90a7fc493ee959627e9482e"),
		new("prog", "CrUseKAncSurIns33TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e26a95bcd0ddc1f0a8c41b172a422d6c54fe4b1dcf128970f6f79c0f1ff07660"),
		new("prog", "CrUseKAncToo17TPot-216", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebee2f407886f0075fa37fa9426d7ef5098cb0c10dbe3c7d841d6c9cc31a85be"),
		new("prog", "CrUseKAncWea19TWea-1810", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "edf0489af0daad630c8ca4faa70e184043350b3f283a60d296b9f9a1959e796f"),
		new("prog", "CrUseKAncWooandJoi34TCar-320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d28a94cb57b2e92892b64a918076a6d451936eb02d57c7b5888983549c7d6d37"),
		new("prog", "CrUseKAntFoo55", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8c1936ffb5625b477b163db6fa818814c49f031fa28aaa69327d338fd2eaeac"),
		new("prog", "CrUseKCelHouCra48TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "42f62fef7b45078de90de735522ab49f61006c201fedeb8aca96fe47188883ad"),
		new("prog", "CrUseKCelHouCra48TScr-2930", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ee3e3df453a295f9066a9e65c530c513fd568cae22139976f450678f162acef"),
		new("prog", "CrUseKCelTex7TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a414190f6ce9a7f39cc41abd4db69bbae8b3cee6643902cb4efeab5378af876"),
		new("prog", "CrUseKEgyFoo57", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae33561809aa0badec0248d61b2a9e252401d538c87358dc884c4b4be0742a29"),
		new("prog", "CrUseKEtrTex12TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "77c54644682307c86bd679e7eb09b1211e96f62bc3b75cb2e75a5d37afd0692c"),
	];
}
