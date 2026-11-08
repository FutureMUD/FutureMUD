// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch061() =>
	[
		new("item", "renaissance_jewellery_common_pin_71", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8f5bdb8671d7451589056d766ddbf5cf128eb23aabe1d4048ffec23f14f81b2"),
		new("item", "renaissance_jewellery_common_portable_panel_209", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1cf71b6e9890f580125e51ca43c4b4060da9d0ac38fc2ec532426b4815d5c789"),
		new("item", "renaissance_jewellery_common_prayer_cord_54", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce98c6daa70e8279833674a9951b68e46a2662bebc9e2f6160fec4bb725ee302"),
		new("item", "renaissance_jewellery_common_waist_chain_45", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3581ab0182f49dd68587e25cbb7dc3cf08e62403ea11dc9b9c6666c973d7c4c6"),
		new("item", "renaissance_jewellery_eal_chain_necklace_275", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ec50b2f7093b8b13d61ea3608ba4e81f2b0d20b76a662c5e8de19e24c9460e5"),
		new("item", "renaissance_jewellery_eal_signet_ring_272", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "93c410489ab864da89abb40f4f7cbe4e60d945e360222bbe8a58e3527a88f39c"),
		new("item", "renaissance_jewellery_eon_brooch_160", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "48e1a0248bc40558e991c6aedacc542415b430d136f715416338538bb43f67d2"),
		new("item", "renaissance_jewellery_eon_prayer_cord_174", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "93307b1ca6a4243de20c21222c0f2fbf6e8376b03b8a517b07fbdaece341ed4f"),
		new("item", "renaissance_jewellery_eon_votive_token_175", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e31390949d0e6a5dc06d7818cdf67e731b1cf41a4ccb78a007b4293d75bd1226"),
		new("item", "renaissance_jewellery_iba_chain_necklace_35", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec42478a084f1ba8ff50a06d1e9958c11493ccf79b120d622d8fe1475d23c131"),
		new("item", "renaissance_jewellery_jpn_pendant_303", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d08e6494f1d70a48055e6105e53abe4ba93559c4c627ad19adef92cd01a28763"),
		new("item", "renaissance_jewellery_mes_bead_necklace_574", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cdc5f8eeeb382fa86bdd49eecc689b6b1c4381fb6a0ef870690cff312e9471d9"),
		new("item", "renaissance_jewellery_mes_devotional_medal_592", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "345a72d0d383f5b0048d277004933af0920a38205e27d2d10579b71b016c6e8f"),
		new("item", "renaissance_jewellery_mes_scripture_box_596", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bc09742e05c9980fb38c7ea1ae68f550b04b300cc4b96f07be64c6655b71d9a"),
		new("item", "renaissance_jewellery_msea_badge_407", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f1701f964cb8855ab5e6c1c8a278188d006243dc0baf5047720740318b173fb"),
		new("item", "renaissance_jewellery_msea_scripture_box_416", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db12dccb11028a3aef8d423986bbaa8c6baa3491dd08d564792f162ff8b1e704"),
		new("item", "renaissance_jewellery_msea_wreath_410", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a50f3df464bee2473c61346559a97f064dc1894c6849efc1459511be236990f8"),
		new("item", "renaissance_jewellery_nac_circlet_673", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b2b5323af2420fea8e0160f5452d4527988c9f40bed356e9f04c68cc3cb8927"),
		new("item", "renaissance_jewellery_nba_devotional_medal_112", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ac6fd2f9cdef443188ffcd627477958f9ee1b2ca7baaee86074b52e8f464c6e"),
		new("item", "renaissance_jewellery_nba_portable_panel_119", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7cadbde162214c7cc8c9aa4fe253ce42efa4dd5d7b62fd9ff346087dc4f7d72"),
		new("item", "renaissance_jewellery_nba_scripture_box_116", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68c4e6813674a81d5b53cba12f369da1bda6237c5c82f7925d8f28bd48e371c7"),
		new("item", "renaissance_jewellery_ott_armlet_187", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e13927d43d2130f6c08f5db7e33d9b2f2931aef7c268c1a8d6414f149501aabc"),
		new("item", "renaissance_jewellery_ott_bracelet_186", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c76a6d70b4302ade538ea5d39dce9f1e32af1f884c893f79dd1a7aa43f309476"),
		new("item", "renaissance_jewellery_ott_circlet_193", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "02eb97e82aaf51bbf77e718fa41640e2c56c35cdd60b426bf0781d9d436faa2b"),
		new("item", "renaissance_jewellery_ott_waist_chain_195", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "264c52df28f3304c11285955521a8e306178d08226f60fb6ffd22a34e216a3f2"),
		new("item", "renaissance_jewellery_ott_wreath_200", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39ba6f9220c8d409ecc76fece2551a958adc1e6bcbcf56fb9f594c1df4e9c52a"),
		new("item", "renaissance_jewellery_pip_armlet_217", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "31cd9b4cb5cdb450e98a683712532dff01e8b3b82b969a74516aadbe92c3f100"),
		new("item", "renaissance_jewellery_pip_offering_bowl_237", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b699ba59ef0e3a6b367f7766a43c34eaddc565455cdca27b6ce288bd9a379bf"),
		new("item", "renaissance_jewellery_rse_armlet_517", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e5a5144a47d192a72cb7e5330f8e438259d614c337c5949b2b62bcb90087bc2"),
		new("item", "renaissance_jewellery_rse_bracelet_516", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3364281e2edce7c38b6f3108038731e766bc21ef77b22f9d81aa4394a42c528f"),
		new("item", "renaissance_jewellery_sai_temple_ring_498", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b2bd2a5ccf7a6bbcb6dfc88f800f5614ca6ba610823bc8a5b382e05f71e5673"),
		new("item", "renaissance_jewellery_sas_pin_251", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ec194e8f2514b74f3cb3df6642fc29764c8f3f213743e1b0d70a5b7e91eb185"),
		new("item", "renaissance_jewellery_sas_scripture_box_266", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "731be7b938356fb6557e1e93a338ca78605ec3f67632d90a06735c0c48fb5a02"),
		new("item", "renaissance_jewellery_sas_temple_ring_258", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f349ade018534bb9c590aea07d99fe66696c5d8f0143c2618edf7d4aa74fedc2"),
		new("item", "renaissance_jewellery_sem_devotional_badge_383", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a59a940ab8135b6ff4dc2a0acf47f94a75f4e4cbbe6771a8707bfa09fcea11ec"),
		new("item", "renaissance_jewellery_sem_prayer_cord_384", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5033c5ebe0d3db20aaadf4dcf7aadc9377542b9e1935a4c5825829aaded8d641"),
		new("item", "renaissance_jewellery_stp_devotional_medal_442", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a5460ce65913b8ea07e6e60d16dce07bcfdfcfa05091a081db327f6500773bb"),
		new("item", "renaissance_jewellery_stp_pin_431", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66330881e2de4537e527dc11dfed24aa4144980dea5a24a1550600becdf99da5"),
		new("item", "renaissance_jewellery_wer_prayer_beads_21", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f482155504caca3446311c9c542912fbc220c08478df276beabae07229c9b4e"),
		new("item", "renaissance_jewellery_wer_prayer_cord_24", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4f25914ab0a9e652dbb02e4c04df70212caad8be68aebfc2f8aca05bafc628a"),
	];
}
