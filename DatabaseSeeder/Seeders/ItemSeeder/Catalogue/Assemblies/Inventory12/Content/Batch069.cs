// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch069() =>
	[
		new("outfit", "renaissance_military_armour_outfit_japanese_samurai", "renaissance", ["renaissance"], ["item:renaissance_military_do_japanese_lamellar", "item:renaissance_military_kabuto_ridged", "item:renaissance_military_kote_japanese", "item:renaissance_military_kusazuri_japanese", "item:renaissance_military_menpo_japanese", "item:renaissance_military_sode_japanese", "item:renaissance_military_suneate_japanese"], ItemSeederOwnershipPolicy.StockAggregate, "3982708a7a5526e4e7058beab5da3e420c9c895163f345ab1ad80fe472a47934"),
		new("outfit", "renaissance_military_armour_outfit_tatar_horse_archer", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_helmet_tatar_conical", "item:renaissance_military_hussar_karwasze", "item:renaissance_military_mail_shirt_tatar", "item:renaissance_military_sipahi_greaves_lamed"], ItemSeederOwnershipPolicy.StockAggregate, "d721f51aaeb438b3bd8dcbc56b5c59426d7dbecf0f5e0b25235e00a8bb1099b9"),
		new("outfit", "renaissance_outfit_ottoman_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_ottoman_collarless_inner_shirt", "item:renaissance_ottoman_full_salwar_trousers", "item:renaissance_ottoman_pointed_slippers", "item:renaissance_ottoman_short_entari_coat", "item:renaissance_ottoman_wrapped_cap_turban", "item:renaissance_shared_clothing_drawstring_drawers"], ItemSeederOwnershipPolicy.StockAggregate, "66e2aa8be34b5834a80adcbecb4691a08fb0aeded63f0726c27ddf63d20bd7c4"),
		new("outfit", "renaissance_outfit_south_asian_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_southasian_bunched_ankletrousers", "item:renaissance_southasian_curvedtoe_shoes", "item:renaissance_southasian_headbanded_courtveil", "item:renaissance_southasian_long_crossover_robe", "item:renaissance_southasian_short_sideslit_tunic"], ItemSeederOwnershipPolicy.StockAggregate, "9de7f32ba805efbe305f7894e77d69a64d15a39cda45a966dcef0c987011cfb8"),
		new("prog", "CrApKAncCerVes16TPot-225", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aad1c3e4d5761bf75deb05d40c1bf31b0b873c11d09b84bd2bc3841be879be03"),
		new("prog", "CrApKAncComCloCra22TGla-1120", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "18a781ffe9464f3f9e73a81d62f5384043684e62b89ef66c725690d681eb6b8a"),
		new("prog", "CrApKAncHouCra38TSil-2415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1973cd8ce29a4bf6d27c36b4bf6dfa3be495cefb26116923f991ace12c51e1dd"),
		new("prog", "CrApKAncLigandHea15TSur-125", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6ead47c3156e46889384a6340f621fd3338c513798bef47fb1c7d3f140aca1e"),
		new("prog", "CrApKAncMetVes39TBla-415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "42c038609cb662764d68809b6d3c09c091d4b3dfaf8208d844a5cf0bd7fd0b9c"),
		new("prog", "CrApKAncScrImp28TBla-415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d11bef4c0c7624edbf3a4be7706bcc8be2f0a053205174d270d74ece52f5507c"),
		new("prog", "CrApKAncScrImp28TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d64d260518008d23300c4fb06117e1b7116e6f64e5caebe29f61cf4c43510da5"),
		new("prog", "CrApKAncScrandCodBin29TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "82dd658f3999656ae8c2f052c3025ab5bdc6294af9ff9adff735efdc3c22a09a"),
		new("prog", "CrApKAncStoBonandHorCar40TScr-2930", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2780ea1e69bac48e962d78cdcc37b60edc7375c100cfec35c0841d2178bf167a"),
		new("prog", "CrApKAncTabMak26TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b66ff04a54314de0e8e7b2203bcbfffddd2dd83c4a2317b0b81d0a28753f699"),
		new("prog", "CrApKAncTexPro3TWea-151", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "813045ed51810b4e475ced164398f140bbd0e925227e674dfcefbf16098633f7"),
		new("prog", "CrApKAncToo17TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d55ad6c5d2338559b600c007dc69bf46335019d60a43de3e16ef795343e2a2b2"),
		new("prog", "CrApKCelTex7TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5012b288fa37c1febc3ed59f009af663daf8170160e521ca0268ec52ff39746e"),
		new("prog", "CrApKCelTex7TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e79acae0faae69554d9ad7bf2ca5e8b8539f2a5d8740b1324349f2d62cdb146"),
		new("prog", "CrApKGerTex8TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f016d454ff45b592473c6707e64d07447024df342b3dcbd97589e0d641c0f545"),
		new("prog", "CrApKGerTex8TTai-545", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3871cc2990f63850bc3f8020de9cfe40421ba77b1c17f34efa0024de3d02c61"),
		new("prog", "CrApKMedFooPro73TBas-2310", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "973b084dac1616a83c157a2d87628aa8a251f13aeaa060a66d8ff82bba4b45f6"),
		new("prog", "CrApKPriProHisComWor2TPai-1315", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "56f8579c14b8ee36ef490a875755b6b9539676a0a395cfad49962f36c4583bf7"),
		new("prog", "CrApKPriProHisComWor2TPot-225", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "196684000136b37b35438de8cb8c4854f8142e6b03b134d4c29d2d59f3383642"),
		new("prog", "CrUseKAncGla18TGla-1120", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "96ac5368b9b7b130a2bc970dba387144d09f0084ac2ca08cffeb93f78b8fa14f"),
		new("prog", "CrUseKAncProandMobAid32TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7c2b9920fa58cea32c44a82b01fd7f7a69ba0cfa6a66885796de4f5d753e676"),
		new("prog", "CrUseKAncTabMak26TPot-215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "af3a4d6de9572e0d43ce8644ec88ed991b5103920812c1d34048bb4521d51531"),
		new("prog", "CrUseKAncToo17TBla-415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e8c873a0d67a91dbb9b467baaef75f08b5f210eefddafcd789a25a8ebc7907f"),
		new("prog", "CrUseKAncToo17TPot-215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0fd2fcb488fa0f36feacf7130e1197e2e831ca2ff0803dddb474686740fc8835"),
		new("prog", "CrUseKCelTex7TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd5acb19063501bb31a40a5fcf0527033abc1466707c15ea2f62a6523951a4ed"),
		new("prog", "CrUseKGerTex8TTai-510", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "312ec7520887804255e992c605ea38b68ab57b08b87ee46af4c0b55a7d066bd8"),
		new("prog", "CrUseKHelTex4TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9dd5f5bc59b05915878b9c12539ba18290381debac45a2876d918fc7eb0f153"),
		new("prog", "CrUseKMedIndFou67TSme-910", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "150c84635eba269324c61c374c66d3ca68d1350fd4f4253718d077c8b04781e9"),
		new("prog", "CrUseKPreIndFooCatPro74TCoo-710", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eae492d669c332fe63dfac91b2750b6af3b77abee05de5bdd6e124c725d903a1"),
		new("prog", "CrUseKPriProHisComWor2TBla-430", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "86672a3b039b95e205cbf32883ed7f364b7afc6f5b8e261d384020befdd2b193"),
		new("prog", "CrUseKRenandEarModJew80TCar-315", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca696e7e4f5651f32edecbfa54464908a270818f8867b56a56bde44a629d0b96"),
		new("prog", "CrUseKRenandEarModJew80TScr-2915", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b887be979007fd641f27e9ef9f31c33d20d3044bc95749621abe8f9f8bd01c95"),
		new("prog", "CrWhyKAncAgrPro54TCoo-720", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f5f9ee2439d98bde40a40112c04d1de369fc036698db899fa769c244a4c5084"),
		new("prog", "CrWhyKAncArm20TArm-1920", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e85a8318d9f7ceecb56d73ddcc8aa7d1b52db988db7c55e18685e1810a6a2899"),
		new("prog", "CrWhyKAncCerVes16TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d794ae4b0e260e79d8ce59de9dd4eb308ac1a4ddfd55fe1216006bfd7b85af80"),
		new("prog", "CrWhyKAncHidandLea52TLea-61", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b5c32d237d359cb4191461d6b5c91870aab4a0d46b240703adaaefe7664b52d"),
	];
}
