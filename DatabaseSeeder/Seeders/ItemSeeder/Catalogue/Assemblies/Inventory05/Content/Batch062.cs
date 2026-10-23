// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch062() =>
	[
		new("item", "renaissance_maritime_loose_decktrousers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f33d5c2d6f2292105dfbe42cced0e9518f51b1377ccda99244c45549dfaf58f"),
		new("item", "renaissance_medical_apothecary_mortar", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "911df7340faa7c93a2eb5521d67773ce41f94f0bee4741b1d1bc7b8a5581cd85"),
		new("item", "renaissance_medical_arm_sling", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "30330b6115664e2a661b219ff9e027ddbf2705bbc094fcdf292d9b866b230fbd"),
		new("item", "renaissance_medical_sympathetic_powder_vial", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "543e1c1faae6332befafe39361bb82b4bf739695e114891f53c0bd7e9f0b1e12"),
		new("item", "renaissance_mesoamerican_painted_barkclothcape", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50acc38cc8ea84123f19838469cc0b7f937342d7719a6f24d6c819d5192365ab"),
		new("item", "renaissance_military_artillery_case_shot_three_pound", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea3e6fad18489db886f280523a0c6dc6a8c6e92eefdb05a5a64648e26d0f789d"),
		new("item", "renaissance_military_barding_caparison_brigandine", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a8a8612c767d275a8e1c48538274416ce6b9ebc6dfaeb3f987ee18ddd0dcf56"),
		new("item", "renaissance_military_breastplate_milanese", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8774ebb30e36673d3a66898bfb33e0ae880207124b3d350d32e591bc67f721d"),
		new("item", "renaissance_military_caparison_japanese", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c9a8e5e34e2e770a1e5d34fd13567e48497ef7c75b39a45fd6fb788da87ea16"),
		new("item", "renaissance_military_cuisses_articulated", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc3ab480470003bcf819fa1e4a5065c19cb10769158400ee23da15f362954897"),
		new("item", "renaissance_military_gorget_two_piece", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "91f9513c95c2963f6ad04f96e7649633da6d60de96d4072373a143dbfa74fae9"),
		new("item", "renaissance_military_hussar_chanfron", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc9acef278c4cf413f3933c19ae5301dd0944be891a1071d1bc242c5be9f4170"),
		new("item", "renaissance_military_kris_wavy_blade", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc606262fc5bd2ac4c0477fae8a6b217ada4964810280ed9a93b979c1c5fa29d"),
		new("item", "renaissance_military_mail_shirt_ethiopian", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b01c8a5135038e145114739fd1f7e7bcc4abbb1bdc4f60d352132e1a6bb38542"),
		new("item", "renaissance_military_matchlock_pistol", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b52b96646bc288171cc95ff76d20ee75d79263e2ef3f2885ed750124bb090fdc"),
		new("item", "renaissance_military_targe_bossed", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61cc3be200eb3985bf29bc1b5c010b15dda6cc78c41810725be60ab2d1888d31"),
		new("item", "renaissance_ming_roundcollar_longrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81666ef4676314eb9ef0f3cc967c5c23b74fff5af326f816d9d368db3b6a51e9"),
		new("item", "renaissance_northamerican_hide_breechcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "711c25480a050a6964241a09bd159a2fd56c0b2a7d5477723cc63d9173dadc9d"),
		new("item", "renaissance_ottoman_long_entari_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a03baf100ef209d759c15f2cca813220cde56616f13e915a09b4030081fbb56a"),
		new("item", "renaissance_persianate_wide_pleated_trousers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6184605258ef3380a3d9c53dcc419f1a26c030a84abce4696451960d34cd8d04"),
		new("item", "renaissance_personal_ottoman_bazaar_document_satchel", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "94f19e7316911e50b417deae1112f1dd6822f2f602d6b3bad7031f1df69b861f"),
		new("item", "renaissance_personal_tavern_inn_score_wallet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "845d36f08766778fee82331b61998e896e96cf73a0324f2dd1c15d3cd824380b"),
		new("item", "renaissance_redsea_narrow_cotton_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac91a64733d00ce3e5dc5e7690175cb64e2447247af280ca17d0ad70a05501da"),
		new("item", "renaissance_repair_medical_instrument_repair_kit", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5b398b8acfc25bd3473c00fea72248effa4b4cdc57d884827a9f755fd7854df"),
		new("item", "renaissance_sahel_leather_ridingcoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a050a8a378f28acb4bd22c0099896cdf17f52a4242444be228a41a92c19030f2"),
		new("item", "renaissance_science_sector_rule", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a347cdbf8af35634a84b42dee8a2b4c4b9a1203af9fb0d6b279a5b1ff3ff860d"),
		new("item", "renaissance_shared_clothing_chincord_ridinghat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fc46780b30fa892b97b4b6120cbe2ebd00aa5f8b3be501f8c6a21c357ca9284"),
		new("item", "renaissance_shared_clothing_footed_stockings", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f6e772a27784a1e2a1e64e4ac90cdce4750ecc347e0c04a0df152cfc245e3a7"),
		new("item", "renaissance_shared_clothing_latchet_walkingshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c013ef29408fd80f107c2a7da27a959bbbb5d3ead9897c760d837d1d36cbc82"),
		new("item", "renaissance_shared_clothing_long_undershirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8777fb3ec0d6232acfad2ce8addd49adbc2ebc66f7064667183ff6b27501e7fd"),
		new("item", "renaissance_shared_clothing_soft_brimless_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "77c4c857418ec8bb27b28bccf307e3540e09f737a397d5199960bef46b759fd8"),
		new("item", "renaissance_shared_clothing_straight_underrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b228d5bd4aa5d1d3bbc53f41bf6041cf3018e0d65c3413252ed1adbd53e5e598"),
		new("item", "renaissance_southasian_chintied_ridingturban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c81cd06e986caf7efbd6028e3798a959a3ccbeee16e0df8d7c2f8a7852bdafdc"),
		new("item", "renaissance_southasian_fancrested_turban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0136d3b7e2e403d1bbe6f11ebba712e27f04111683f420e09a467a938dc12701"),
		new("item", "renaissance_southasian_gathered_longskirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "515c36e0a61f1ec92236ed68a052d56be53b37ed5bee466c3caccbc5a9100806"),
		new("item", "renaissance_southasian_long_sideslit_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "11b8343b762a607aee5d0ed580ee271e0d56554d7a6e73b23a849448bb4cc753"),
		new("item", "renaissance_steppe_quilted_ridingrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2450afb84fb3b7bd94073d89d08c8457d76f223e8cea317eace4dc6ba0c0787"),
		new("item", "renaissance_steppe_sidefastened_furcoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2c6d7ca8a834af355d8c3dbcc80645a6c13acc8fad9aa0780571c47fa3355b7"),
		new("item", "renaissance_steppe_soft_highboots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d593c5b8e6dc6d6e9bdb732f82f45dc2a815f9636c6a3f5cb624fb2ebae2ed4f"),
		new("item", "renaissance_trade_civic_guild_civic_till_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8d7f6fa215bcff9eb824c27087a1d379b7100914301325473a04a33de2404c9"),
	];
}
