// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch067() =>
	[
		new("outfit", "renaissance_military_armour_outfit_ming_palace_guard", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_brigandine_livery_vest", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_brigandine_panelled", "item:renaissance_military_ming_helmet_brimmed", "item:renaissance_military_ming_leg_guards"], ItemSeederOwnershipPolicy.StockAggregate, "a6986dd57bb4a7e95801cb3482d89c9e57a69cf10358594b5cc5e66b01c8c3d3"),
		new("outfit", "renaissance_military_armour_outfit_safavid_qizilbash", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_bazubands_safavid", "item:renaissance_military_char_aina_safavid", "item:renaissance_military_mail_shirt_safavid", "item:renaissance_military_safavid_helmet_turban", "item:renaissance_military_sipahi_greaves_lamed"], ItemSeederOwnershipPolicy.StockAggregate, "3dd28643ae54dee82145f89588a0338cf3c59efdf54cae25bdf065fa8cb82a90"),
		new("outfit", "renaissance_military_armour_outfit_safavid_tofangchi", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_bazubands_safavid", "item:renaissance_military_safavid_helmet_turban", "item:renaissance_military_sipahi_greaves_lamed", "item:renaissance_military_tofangchi_coat_padded"], ItemSeederOwnershipPolicy.StockAggregate, "6977ec6a3d6458a05113bb7cd320bfebafce949b2f409d228886b796dc6e4bfb"),
		new("outfit", "renaissance_outfit_latin_priest_mass", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_latin_amice", "item:medieval_latin_chasuble", "item:medieval_latin_linen_cincture", "item:medieval_latin_maniple", "item:medieval_latin_stole", "item:medieval_latin_white_alb", "item:medieval_soft_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, "51ec1bd6cc77b1f720f458a3ec7754930dbb95d1a08180d73c9ce0909a6bafd5"),
		new("outfit", "renaissance_outfit_mesoamerican_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_mesoamerican_panelled_loincloth", "item:renaissance_mesoamerican_shouldertied_cloak", "item:renaissance_mesoamerican_woven_fibresandals", "item:renaissance_mesoamerican_woven_headcap"], ItemSeederOwnershipPolicy.StockAggregate, "1781c92393764a8397318921e70c03af443f14acde372ccb27505af436553aa8"),
		new("outfit", "renaissance_outfit_ottoman_janissary_service", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:preindustrial_clothing_simple_woven_sash", "item:renaissance_ottoman_collarless_inner_shirt", "item:renaissance_ottoman_full_salwar_trousers", "item:renaissance_ottoman_janissary_bork", "item:renaissance_ottoman_short_cavalry_jacket", "item:renaissance_ottoman_short_fitted_vest", "item:renaissance_shared_clothing_ankle_boots"], ItemSeederOwnershipPolicy.StockAggregate, "fa322bdb3982115c13dcd3c2b6e3d9b4d0f997ae8b1b362d078d85cbc37e8fc1"),
		new("outfit", "renaissance_outfit_western_court_lady", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_breast_wrap", "item:renaissance_western_fine_latchet_shoes", "item:renaissance_western_fitted_court_gown", "item:renaissance_western_fitted_dress_gloves", "item:renaissance_western_french_hood", "item:renaissance_western_full_petticoat", "item:renaissance_western_partlet", "item:renaissance_western_spanish_farthingale", "item:renaissance_western_square_neck_chemise", "item:renaissance_western_stiffened_pair_of_bodies"], ItemSeederOwnershipPolicy.StockAggregate, "67c0b77ca73c28f289922a9fbd8d1ee3e48eb3ddf67834b23471f2f520485461"),
		new("prog", "CrApKAnaHouCra43TSil-2440", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d72d0bdd4f116e44e6a327e0624141d64728f30ba021709dc2d65577a1ac2b2"),
		new("prog", "CrApKAnaTex13TTai-545", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c13ce461e25bea6ad6bba748ddc59b3f4c4aa954444c6ca47510e078bd5d3df1"),
		new("prog", "CrApKAncApi53TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7f8cea8677c1bb783a2cbd3beb8bac7b563dd617edcef6b4156e347651ef017"),
		new("prog", "CrApKAncEquCra21TBla-415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "10acf1cb5b874cb84b0a602d5b536cd66adfd09c687854db85227344f9e29cfd"),
		new("prog", "CrApKAncGla18TGla-1115", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1bb1727a07c09d7cf19f4abaeac0ad136573be8bf9267a07c68c31241a93b5de"),
		new("prog", "CrApKAncGla18TGla-1120", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d603bdd4eebca4428a647d4d2f7dd6b072afa090834627f874541593d979d234"),
		new("prog", "CrApKAncHidandLea52TLea-615", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffa288a5a49e114b8fb79aac5b70dccc8da09b80f9ab576d5b8a7da6d2199f84"),
		new("prog", "CrApKAncHidandLea52TLea-620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f69b9b7028636537d64af795f209731bdeaa6feffca4b6e9497ce896313a828"),
		new("prog", "CrApKAncJewCra23TSil-2415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f168d2fde9b40136b8705bf86ce5bb619971cb93da3ead915c42c11857966c3"),
		new("prog", "CrApKAncProandMobAid32TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "791851ad713d6d1f63d3a91fa251774eb9950bb2cc405569b7930c020a1551d5"),
		new("prog", "CrApKAncTexPro3TSpi-161", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b65bffab6953d2e4e64f99ce1868891979754b740eaa0299035a1cd19795c926"),
		new("prog", "CrApKAncWea19TFle-2115", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "218b22e89f72bfa553e98d4bfd44ffba203f6d2e77ff499e911fbb23b1124b86"),
		new("prog", "CrApKEgyTex5TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "935367b28d18e99511039bb4b4546226555b4e51d256e243cc720c98b6903755"),
		new("prog", "CrApKHelHouCra41TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e24190e6c5956f123cd52bae1c8aa36be61bd60232c9e5d2bbe6a3061894d3b"),
		new("prog", "CrApKHelTex4TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a133964a8e131c1e9163c7efd39d733f59cb2b0d1db7b982f6a118147c6bd81"),
		new("prog", "CrApKHelTex4TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca4b4fbefc5de70c1df5406091a4ee99ea963f2204539c53fa8f788689073e97"),
		new("prog", "CrApKKusHouCra45TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0287a2c5acb21d0e46623478e90d46d24ef38c15a0603cc33ea972432ef1fab"),
		new("prog", "CrApKPerTex11TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8ffb83241cc574fe4aa714b73790b8ead1438dfe42f19e25248a2245cde640d"),
		new("prog", "CrApKPerTex11TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "74ec763aeab8a382fd228e7b00e926ecb9d44ffc4a37d6cf07108daa5fb80c21"),
		new("prog", "CrApKPreIndFooCatPro74TCoo-710", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "74b219b2a217b734d415921f622274ecf694cb566802961f84bc663c2b592f64"),
		new("prog", "CrApKPriProHisComWor2TLab-810", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cac80a2e327d2a91173fd88b54cd640b646ab4738f079f794f44871aae219976"),
		new("prog", "CrApKPunHouCra46TGla-1135", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a51a08fa16e23b28ca9160b2b3c8564b2deefd2f6599f267308a730365fe9765"),
		new("prog", "CrUseKAncAgrPro54TCoo-710", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b57bae92db10f4cb6bab8194eedabff1fc06ef51bbeab3e66e4a520b879d56ce"),
		new("prog", "CrUseKAncApi53TBla-420", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "869d7aca50dcfb172aa2e6dd0f7af17106a6761f149aeacc27fc85b3d284e812"),
		new("prog", "CrUseKAncHidandLea52TLea-640", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "81bf14f864e5e62c14fd657aee79c027c5c99f0a5c0196d0e017fc1da33d9ede"),
		new("prog", "CrUseKAncJewCra23TSil-2435", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4295f611c4b08af2dd38ca3f00068936a91a2e7dcbcfde9ba2e3084a77fe4e9"),
		new("prog", "CrUseKAncMedTreSup30TCar-320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d137ec42b54e7560411f1a6a4444ea1c6ff4d06d8dadfa0512766350138c2fa9"),
		new("prog", "CrUseKAncPapMak24TBas-2315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a133cee7a0d7da3b096306b77c35ef46a475b3a4537c405849d5dd49f9ae5992"),
		new("prog", "CrUseKAncPapMak24TBas-2320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2961d8cb569dba6002c2002b595933e265453578d4143a4d7128ece612f8d645"),
		new("prog", "CrUseKAncScrandCodBin29TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "93c59d5cefd4e370e218d8a1c24f713c65979aef66f1c1f42c52144349192be0"),
		new("prog", "CrUseKAncTexPro3TWea-1510", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1513aa4df2b7fc093ecb745ec874b08d716cf5caf44a2bea0f1e500d1dc04859"),
		new("prog", "CrUseKEgyHouCra42TMas-1015", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "920c76eb054d791ddfacf2f68e10f76ba7c3f732df91a9484b6934bd1b3e986a"),
		new("prog", "CrUseKMedFooPro73TBas-2310", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "96d79afeffddea408aff96e44791d2a284abd2fb279b0e2d1653cfb6258688ff"),
	];
}
