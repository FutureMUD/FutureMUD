// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch032() =>
	[
		new("item", "earlymodern_headwear_lace_lappet_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53378dbbeaceb7560c33f3218f4db95a55b66917fb08f19fbb275ca2e4a12b05"),
		new("item", "earlymodern_household_coffeehouse_table", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "79194e1aac8a49a7ac359f34e85ca85e12c71029443c2fad367be7a1cd7cbdd2"),
		new("item", "earlymodern_household_escritoire", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fbf7415748e2875be466aaf338ff7606e2a09ee554632010aaa1eca8708baa1"),
		new("item", "earlymodern_jewellery_bra_badge_597", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "51a6aa0ba1bedb18df6a7d8bc4e622e016ea0c5bbb35fc4a1cef7f5d810108bb"),
		new("item", "earlymodern_jewellery_bra_diadem_594", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f08155308c9ce6fc39a9834002c6fe07b77575e770183f19a73903ff8311aff7"),
		new("item", "earlymodern_jewellery_bra_ring_581", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "28934b49ff6ec09fa33ee3cf082e9b43c67904f31f34329e13234caf2020c9bd"),
		new("item", "earlymodern_jewellery_eth_bead_necklace_524", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c092915c9c667710ec6b56272d58acd7f59c6b046b11945ba080854d1a72afe2"),
		new("item", "earlymodern_jewellery_eth_belt_ornament_536", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad3d87e9379217688e8fc54d8c5ed9afe3b4eecc4c2504e1780adb34f5ec3323"),
		new("item", "earlymodern_jewellery_eth_temple_ring_538", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a28f5c28cca67f217f448919fdda65bd32ef2070be39a4e475a5290f1a4d284"),
		new("item", "earlymodern_jewellery_gb_earrings_49", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b53ae1e3ce238765265e7fbb503232f0efa4faac695694bd3c293b8a6399212"),
		new("item", "earlymodern_jewellery_gb_waist_chain_55", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "384b6c27c0b0062f659ae94a9b6ea6a4dcb659f326835d8df37b49a0d73c39e3"),
		new("item", "earlymodern_jewellery_hre_torc_99", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5268e2b3b8d6dfb38750cb5e07a9766ed6dbbbdf10a6e3350662af9d46eaa3eb"),
		new("item", "earlymodern_jewellery_ias_hairpin_452", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d49d379881c9880406fc7a6919b721b541197599b841b567db293524a6f049d7"),
		new("item", "earlymodern_jewellery_ib_brooch_70", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "adc4a8d4fb7a78eb76600cb613cde6902b40eeb97d05540e980d2c8fc4291607"),
		new("item", "earlymodern_jewellery_ib_waist_chain_75", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c18508a877c57fc5e0e2afd1390b8ec383e4275d5bb54af651134639d567d65"),
		new("item", "earlymodern_jewellery_ina_signet_ring_622", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a3030a0e7d2e1b3dcee0da5a763ca9a8a2ec13470682b67a306ef6ae6187fac"),
		new("item", "earlymodern_jewellery_ina_temple_ring_638", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed4fbfb52d5406d8fdf625e772896094829b725b3228eda297683b77076d8ce1"),
		new("item", "earlymodern_jewellery_it_pendant_103", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d45c4f0a2984d64058331c3cf101e2c7128ab4e8aac7683effa6cca29c7510e5"),
		new("item", "earlymodern_jewellery_kongo_ring_481", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b487479dde18af214def8c337f5af99d806721e6af34537a048f34c0574030d6"),
		new("item", "earlymodern_jewellery_mca_earrings_649", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a28aa09b63cc63ddd949b24a8e54c25b9658a45396cd502bfeeaf39c13eb192"),
		new("item", "earlymodern_jewellery_ming_badge_337", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2e82f16e404ad151326546e892a67b69734eba115b67c7cf78b209b375cc50b"),
		new("item", "earlymodern_jewellery_ming_pendant_323", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "82f5d54aac794b2f68996758bb3b2f57d25706f063096500d29e64f27831e512"),
		new("item", "earlymodern_jewellery_msea_bead_necklace_424", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1a4a625435c74851b1036bad2359983e0585c72956b3757141a0e1782eef490"),
		new("item", "earlymodern_jewellery_msea_diadem_434", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4861f6e2a81db7ec927a3c8c6d7fb2e94e5a5f8b407689a444f2ef6a03dc261f"),
		new("item", "earlymodern_jewellery_msea_wreath_440", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9eaa588619c72c947a732e6c4b95c47e123a54bad98fc7287dbf4d6778fcb371"),
		new("item", "earlymodern_jewellery_nl_anklet_28", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6695de5f56d7d5e9b367608086783dbaee4709aabe4e5a5ef8f03a8478622a5"),
		new("item", "earlymodern_jewellery_nl_pendant_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9706cd0c8b52d6c252e380c90a1465f2e09178cf5b947052f7f11a4fc245b66e"),
		new("item", "earlymodern_jewellery_nl_wreath_40", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "018ad4392ee884bb52ca6341675df6b094b6318a388ef38511ad633f1004a170"),
		new("item", "earlymodern_jewellery_ott_temple_ring_198", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec5eeb817940580312aa73e31dad393460578586907ccb2df5fbb16ad4d40635"),
		new("item", "earlymodern_jewellery_plh_circlet_153", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a6d82af9904cfbfb6f302acfe62be54012a7ea855c562663ab1726c21085c48"),
		new("item", "earlymodern_jewellery_plh_hairpin_152", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c275822d8805b5270669687815e86c0a2b239e1dc764df6d0da1ed7190469cd1"),
		new("item", "earlymodern_jewellery_plh_waist_chain_155", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fae0c71960de277dced55e107c4a4e502e3ec58be7d9254658cec87820810f8b"),
		new("item", "earlymodern_jewellery_rus_waist_chain_175", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e689f0b400582101030466470b66d63ea629074dc6fedb3ee7beac15e88644ad"),
		new("item", "earlymodern_jewellery_saf_chain_necklace_225", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "441fc59223bd67b60cd42e708661eaa96829ae8e055eb047409c4d8e38262801"),
		new("item", "earlymodern_jewellery_saf_temple_ring_238", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dac1df69a472a546463ae34038d6e56fca442202385f4b6b6eb9165a9ce28f7"),
		new("item", "earlymodern_jewellery_spa_brooch_570", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "801d9dd612ec668f2683c6a80d112829c0f374cdc4aa547eb5eba48df5cfef69"),
		new("item", "earlymodern_jewellery_spa_circlet_573", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7553993cf6f853a6ffb171c16a04476b696e47bdacfcd35c0afb2a8dbb46766"),
		new("item", "earlymodern_jewellery_spa_earrings_569", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "536cbd2431165f89783a7e12e2590ffb13163e25d892236261abb4417c4a9d52"),
		new("item", "earlymodern_jewellery_spa_torc_579", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "119bd16b521e658c5c3608e46de9c0b829aeb554b75631db762414e4d450743a"),
		new("item", "earlymodern_jewellery_swa_bead_necklace_544", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "42996d1de1c90d696739178ccbd5ebdbb4c1280d857fc8026a2976d7149e51e6"),
	];
}
