// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch062() =>
	[
		new("item", "renaissance_jewellery_rse_torc_529", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2f26a983eb1259868a5b304590be63d91f68bd8c9c57f970f4f8437f7daaeab"),
		new("item", "renaissance_jewellery_rse_waist_chain_525", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4deeed68096dc0433524d300ec4be6a1f8e5ded742821e0a5c77e83c69796ee5"),
		new("item", "renaissance_jewellery_sai_signet_ring_482", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f6c79e9452a8286b50f56fa13451bd6fe3e92fb82ec1a68c8c5507a837ad86e"),
		new("item", "renaissance_jewellery_sai_votive_token_505", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "561cf8f672c4529b325e2ed19d40ea24fd14daf206b09e4d56224e32c099fc98"),
		new("item", "renaissance_jewellery_sas_bead_necklace_244", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab68b944e3429f5f06e002f6b6d42ec4c6e066a88dad7e2741c34b6e5b226087"),
		new("item", "renaissance_jewellery_sas_belt_ornament_256", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a0d4228bd0574f28c07c6d625bec564410ca38d95809d67de955604da59c724"),
		new("item", "renaissance_jewellery_sem_brooch_370", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "45a02729da5e37d6cecb09f03dc7bfd540376dfca9cbddca9ff668df0a917426"),
		new("item", "renaissance_jewellery_sem_earrings_369", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c117f8391b5c9295c749e8a7ada5207023bf913ad6ddbefa0f535ccab24ce66a"),
		new("item", "renaissance_jewellery_sem_incense_holder_388", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd46ecfef608d1a460f79d770b2559c826218cc06574f723b7785fc38ef84d6b"),
		new("item", "renaissance_jewellery_sem_prayer_beads_381", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "74405529d23d6a011729c108cfb827a0a99676cb8b78442622c3077b67508d93"),
		new("item", "renaissance_jewellery_sem_torc_379", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07a9016585c1d9b92c368371367fc91d55d7a8fe5a68f52a42568c3a1e580fff"),
		new("item", "renaissance_jewellery_stp_bead_necklace_424", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c42eee8ebbf5d8780dfb571f49e9aaba45532178839d74b89c86456ef15f22a1"),
		new("item", "renaissance_jewellery_wer_earrings_09", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbad6090e1634e1102908535b1f0aa40f94a370726569961a14ef5b3017da7ba"),
		new("item", "renaissance_jewellery_wer_offering_bowl_27", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbbf32a30716364f42092cd92cdc3b11e0a1ad42fc8d7e36f84522659783f985"),
		new("item", "renaissance_joseon_long_crossfront_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfef56e176352200ca69a99bc9145909ef72aef8cdb0674dc4c803cd6182a233"),
		new("item", "renaissance_joseon_short_crossfront_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e4c3d3444238dc3eecdcadaa9bd6754ed33d34eea9415060c3aaac94acd81c7"),
		new("item", "renaissance_medical_catheter", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0225b44601511e0dc9733dd2730d573738ddc28f005b1af0a4836550ef9f9fad"),
		new("item", "renaissance_medical_cinnamon_stock", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed9adf55c4fe4f84c3a3f2d5a1f6663140dadc2a6ed378408a1ddad84c2d4ead"),
		new("item", "renaissance_medical_public_pump_key", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f969055a51d51022b1354a95b0e9f5eb6a10ba2890acbefa5a142436569cab19"),
		new("item", "renaissance_medical_suture_needle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "31fbb8c8c4e3965bf9d33d23d500d8c86e7ebcfb766fd731153ea10b023c6acd"),
		new("item", "renaissance_medical_weapon_salve_pot", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "acea60fdc719fbc91356c2cdecc1c55db5bd6e5c7388e7e8dcbf4286cca35e38"),
		new("item", "renaissance_military_barbute_open", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0bc935951a0627a66120af6b33c4fb2fd44a5a6d7d64bad576fbf48b5521828"),
		new("item", "renaissance_military_brigandine_livery_vest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a74ee564104ee0668674f73cbd58a5ede1aad50523b7b23630cf1e866efe000e"),
		new("item", "renaissance_military_cabasset_ridged", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d682e5ad36869319a93747198f6ef57271a929b4694e5ec55ccdee9d9ae56c54"),
		new("item", "renaissance_military_criniere_ottoman", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "028255a2404e0640686e33e3333f1ba83a7a7eb17124f90cc470e9db7fc19c0d"),
		new("item", "renaissance_military_falcon_cast_iron", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2278271ef0934465869cc401143ffc5c53b7a6b3d218b9148bed4295ecc1e450"),
		new("item", "renaissance_military_infantry_colour", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "efba5c4ea705438b71ce48e97a9112fa62f15c95442b1a3e855b1dcad36cf6f5"),
		new("item", "renaissance_military_joseon_brigandine", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b513e926933df9ab121fae8b295ba896c18a8b581941deeccd040e6555a3ffe"),
		new("item", "renaissance_military_kusazuri_japanese", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6b168b1d9d48c1d95aa8abfa244b2d4f2d62505766571e5920bc38a653687af"),
		new("item", "renaissance_military_lance_vamplate", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a67bfa961be7a9872a91cbc1ca9831c5cdb6a0be1cdcf5cb1b89a3a5158bf77"),
		new("item", "renaissance_military_linstock_ash", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e534a019ea6e47ae60e925560bbd9b7e79f514d65104f52e3c665328ae306d6"),
		new("item", "renaissance_military_muscovite_caparison", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82c6f064e5dc214eed11d3904d7431056fa20b673cbed511e0cf8f31dab1544f"),
		new("item", "renaissance_military_partisan_leaf_blade", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d9ea941c92d1bbe93285143e99f01ac665f9ae433272809ee750667572fcaf60"),
		new("item", "renaissance_military_rajput_mail_coif", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3079ee9a270398f406edc6ccd43f741939b2ee999b5f438760853a3705b9783e"),
		new("item", "renaissance_military_tofangchi_coat_padded", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3f3c6a4b5412168526ec319bdcb03873772e719fff593b21fff73755bffc7e7"),
		new("item", "renaissance_ming_crosscollar_innerrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d61ad69a51fb3808bd770e52957dcc17a9ee21f78a1229997ca17ddc63ff4c27"),
		new("item", "renaissance_ming_narrow_ankletrousers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "391c4c24cad23b3ef0993c06eee2cbaed963fe3a38863d30ad8f8b3f3c4cd8cd"),
		new("item", "renaissance_ming_narrowsleeve_workerrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ba0bf4a3f31198ca792b0502a1e7652f2b6393320ea0b0a3382b7f6166c6091"),
		new("item", "renaissance_ottoman_janissary_bork", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "92df82fc57c03fac453807f120215188ceee7cb4af8877644e83658756264304"),
		new("item", "renaissance_ottoman_loose_layered_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd1e2e6b4d2a39d566c5439c917ddac87bb549ebc032d5dce218c4f7ce030403"),
	];
}
