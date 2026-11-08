// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch061() =>
	[
		new("item", "renaissance_jewellery_common_devotional_medal_112", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa9e41a90b60d7a18cbdb840faf5e3c901b4b599a67e9010d551522c7de1a9ad"),
		new("item", "renaissance_jewellery_common_diadem_74", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f27db0f273cfb8222153007a49a50e268aded0568f7c6c53fe48d9d77d8b4766"),
		new("item", "renaissance_jewellery_common_hairpin_132", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8871d1c4ec937c4bd9283d7229ebfe0208ee83c8c52fd99ca6ac36de411e3aac"),
		new("item", "renaissance_jewellery_common_ring_61", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7ba93fa1ad673a668e2e87cb2e64078b8e996b0fb3e0bd1b5a5e0b2096297f1"),
		new("item", "renaissance_jewellery_common_scripture_box_116", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "56c209470797b1043912a8769a4dd565d744175f510bab3d6a364b0e71dbbf25"),
		new("item", "renaissance_jewellery_common_votive_token_145", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f004817f641d2c2ac8657a8e68f39ab723e09ea7fe9ab19ae3062407ab037d8a"),
		new("item", "renaissance_jewellery_common_wreath_170", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9226b181d0b200363b713542860dc333852be3002b594e29a7a8a70c8cdf0752"),
		new("item", "renaissance_jewellery_common_wreath_20", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "15eec27c57f60e12fb974f869cd836a65af48828ee8ceb2ad7082e030b92631b"),
		new("item", "renaissance_jewellery_eal_anklet_278", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3db11e0a5c7b9d737d8db0b10b92692fd55ee1beea3117e1b7db712530aab0d"),
		new("item", "renaissance_jewellery_eal_processional_ornament_300", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "52fe8ad45bee3fc8a2adb5590df614795bc41803cca25ddf09006edcfe10b215"),
		new("item", "renaissance_jewellery_eal_ring_271", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f704c89620195acc7d40bd3efbebb996b5dd10e10390c81a46a0b09f81b83712"),
		new("item", "renaissance_jewellery_eon_armlet_157", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a115ebacd86745785a5de554f36ebea4d7dc9ad12ada096f2e1de8d658cdfb8a"),
		new("item", "renaissance_jewellery_eon_torc_169", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "972dd35ef2f3e668b6cfad436a3f4e5e416dfad2676cdadf3200dfe545ba74df"),
		new("item", "renaissance_jewellery_eon_waist_chain_165", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6550610a44f3414a27c6592be460e32b585187aca1b5210aa1b39349c37d8f0d"),
		new("item", "renaissance_jewellery_iba_ring_31", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "23c7bc063aceff8ce599db8386ff3e082a2b232a434e935d2610b6a152edf0c8"),
		new("item", "renaissance_jewellery_ind_hairpin_552", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2056a8f812b931893158a1868f954bd157d2fed65cf958f64eb8bfdd6294428d"),
		new("item", "renaissance_jewellery_ind_offering_bowl_567", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a03c8f0a4331ce3f6ff7770c00c3c74c7f355001fe44076d638b84cf56c81f5"),
		new("item", "renaissance_jewellery_jpn_portable_panel_329", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6588cd2120c72a2061227568ede8547a8b6cf286667a3d1245ff880bf277ead"),
		new("item", "renaissance_jewellery_mea_bead_necklace_334", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "828d408e9a8a0d68105a44162350b9424ada2000fd4c4fd8aaebf39766c7ea51"),
		new("item", "renaissance_jewellery_mea_devotional_medal_352", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d1e56db44de75dc2956efe3b7534f2f345f1f6c098f839b13d06c485d82b3f2"),
		new("item", "renaissance_jewellery_mea_diadem_344", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b28c73da53b0a407902c6610e3913660c334783305087b974486a7a0fa4f81df"),
		new("item", "renaissance_jewellery_mea_signet_ring_332", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7164b16974e1bedf3d47e5d3f04aefc9a6948402071983bc8edbca9bfb39336"),
		new("item", "renaissance_jewellery_msea_armlet_397", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa609af489caa9844a62dc5ed32540017efdaf8eb9b0896104ab64f631b3252f"),
		new("item", "renaissance_jewellery_msea_bracelet_396", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad3d0fb26ed05eac0bb39decafc298cac26abbd8e708aa9f733af281277f9e35"),
		new("item", "renaissance_jewellery_nac_incense_holder_688", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e2154b5df7b3614a690b3eef669ca0d1febcc1c4c024a3d220c84a16afbfc61"),
		new("item", "renaissance_jewellery_nac_offering_bowl_687", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "83c5572ac8bd357ceb66629e4e9a6b65337ab8f0fbee2d043636b259339067a0"),
		new("item", "renaissance_jewellery_nba_earrings_99", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8430509f525b3aee1ef4a13e56068f72e30a0886cba44309c262b4d6e1820bf"),
		new("item", "renaissance_jewellery_ott_hairpin_192", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ccc73cda4fa61d63fe6d10b5bb5ba8ecf056e39ea8ee35dca0d4f14544ed24c"),
		new("item", "renaissance_jewellery_pip_bracelet_216", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "72288ec94d15b5c60b559ec1a60d9dd5e6ea3b7eb4fbca6a40ca0097fd98ef35"),
		new("item", "renaissance_jewellery_pip_votive_token_235", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ed9cdcec64d36c891d14e344329270917ee53b4a0945c11e9a022128362e5b9"),
		new("item", "renaissance_jewellery_rse_brooch_520", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e547886ccbe0f0cddfa726162b75d3ba0fe44c75a2ead547b9a7bd5c570a955"),
		new("item", "renaissance_jewellery_rse_circlet_523", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2fb1c493515d64976bdcc0e7ed8d200653a84a353d3bc14b54b58b7ade950de"),
		new("item", "renaissance_jewellery_sai_bead_necklace_484", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "08c6a48f73029f20a2c27a410a231c672a92ce3d3ebab8df4f95b742681e7046"),
		new("item", "renaissance_jewellery_sai_belt_ornament_496", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c9feaed28b1645e4f9ab05fc9c389df408b70c25def8ea2bcfd334f42e7b67f"),
		new("item", "renaissance_jewellery_sas_devotional_medal_262", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9bd25581b326e47d2e30522bc4be624899773b4281f8b6bd3287db5ebc52213f"),
		new("item", "renaissance_jewellery_sas_portable_panel_269", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c1fd976547e4e9ebb955d0a460523b350cca209ae22bb4cb4f83712bce37c42"),
		new("item", "renaissance_jewellery_sem_processional_ornament_390", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "157024d0404b9ad53b2ed1b481c1fda536dfef66be2a68b2af2db4ed1d0ff7af"),
		new("item", "renaissance_jewellery_stp_scripture_box_446", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "79b9aae128dc7e510c2e3448238fda9bb7ab4411406d9cf90fc10788000ff4a4"),
		new("item", "renaissance_jewellery_wer_devotional_badge_23", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2f0b43959a68c169aa29e6e761f9940db518c014c3e6fdf678564bc05d1faba"),
		new("item", "renaissance_joseon_long_crossfront_underrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a521e64e74057d7ef4acddba33b867e4f814bf92da939159595031b6131bf23"),
	];
}
