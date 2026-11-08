// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch060() =>
	[
		new("item", "renaissance_jewellery_common_pin_131", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e291cc8b0f727a4382db7e8a198965215769ae96b5d3307f075b0ee983991544"),
		new("item", "renaissance_jewellery_common_prayer_beads_171", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6913e06712154ed536e4905019e98ce93faab22837d62b0b1f882eeeb7dda0e5"),
		new("item", "renaissance_jewellery_common_processional_ornament_30", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6943be2962116f5c0494bbc2ae63b5bbbb1233f7ad38a05f77d71d1ffe1e5698"),
		new("item", "renaissance_jewellery_common_temple_ring_138", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d78c7c8e305916926bdcdaed94e9ca534682e0ee8382db47859d88ddd2fb8764"),
		new("item", "renaissance_jewellery_eal_devotional_badge_293", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eae55acdac79fffaea656c7b0786b9a028c623bf0c0aa129d489fc2991edb733"),
		new("item", "renaissance_jewellery_eon_belt_ornament_166", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6a098704f155304345f4b3c22ae44c3dbc18d962fb31e067b964e9b08e62b0d"),
		new("item", "renaissance_jewellery_iba_armlet_37", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af406e2e124308bebc5b583ebf913e323946988fc945d214d4c28ccbba077113"),
		new("item", "renaissance_jewellery_mea_bracelet_336", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fcf7f202bc3ccfd80f75cb7bcf1da5e2964b767993314fb674b6c0887174c30"),
		new("item", "renaissance_jewellery_mea_brooch_340", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "303ce54c69d73184b02c07ad3dd5e127684f138d834e2c40d6c4e7b5bc2a4af7"),
		new("item", "renaissance_jewellery_mea_torc_349", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eadf4f6a9e1b660c5fd346b71f55a1c1716319b98a52c352ac567ba2d39032f6"),
		new("item", "renaissance_jewellery_mes_votive_token_595", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b037a0db3663b97421ae49ba8c01b9f8be15f1e12562939c2418dddc1eab170"),
		new("item", "renaissance_jewellery_nac_ring_661", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81efca7dca44bf62428030397207ec206ba9faf548e23224f342a22c56241009"),
		new("item", "renaissance_jewellery_nba_processional_ornament_120", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "95e2a0b8dbd998ac982cea492668ec3ac0cbf3f00613bdd573115052a8beab4d"),
		new("item", "renaissance_jewellery_nba_votive_token_115", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab89d4a554a00f22af90a862e48b6325267723f4c4ebd28d20f7a820151009e5"),
		new("item", "renaissance_jewellery_ott_prayer_beads_201", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "79837f8e97365de518898340734b0583d33127af0717b8c1c8798dffe75833c8"),
		new("item", "renaissance_jewellery_ott_prayer_cord_204", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "edd67f5911cdee954b4eb41f2efa53a7a85894ee4aee5e9fbcb71bde431895af"),
		new("item", "renaissance_jewellery_ott_processional_ornament_210", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd37fca53547c52ceada6d764bb4ab4ea0d86114b3460b1c17a1b8be4abfee23"),
		new("item", "renaissance_jewellery_pip_diadem_224", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e978c83f0e4738da60a04d61b08ba6809c236df0383d32d9f7d724aa011cdd82"),
		new("item", "renaissance_jewellery_pip_scripture_box_236", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7ddb1733eec2aa86fc9e109c33bbb48ca07cff2feacc54d0f2fe1978561eb86"),
		new("item", "renaissance_jewellery_rse_temple_ring_528", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b8b2530266c034d388dea87167164145de9b3afd95c011f2ae5f0f9657650ad"),
		new("item", "renaissance_jewellery_sai_armlet_487", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7097bdd4656041166e04ac842b75de0b198976f314f04fe0d2b396cfe8289d5"),
		new("item", "renaissance_jewellery_sai_bracelet_486", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e88eaf7c59cfa210af98bace809d5528490ad929fa5d30cffd6b8e651947e172"),
		new("item", "renaissance_jewellery_sai_circlet_493", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "079c3fa6ecb7ab3608a3e1c8b821d3b551a72b9a58532b953d3914a8abeb9c17"),
		new("item", "renaissance_jewellery_sai_devotional_medal_502", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a411fd697f4f02caf2528e59868f6c71d48671e4e9da2c843ee785b24dd2aea6"),
		new("item", "renaissance_jewellery_sas_hairpin_252", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6538c8b428a2c3a46e5e73e9ee4395f28c9becb6644bacea5b27972324e79771"),
		new("item", "renaissance_jewellery_sem_diadem_374", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d46b6ed975ba85415193d3e9d0239cb911bc4b2fb0f7de7d1a1c4a05651bb257"),
		new("item", "renaissance_jewellery_sem_wreath_380", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ce2ad952f2f27b6725e9efbb48c48977e461eae03fc28059417ebf0f33577ac"),
		new("item", "renaissance_jewellery_stp_bracelet_426", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8dc02f513885954eda238ec80b25e26cb1c0f2ad136da4e0dd7d79787af8933a"),
		new("item", "renaissance_jewellery_stp_circlet_433", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9757bc4405e7df8d9100f591977ee9a1f9cd2d98963e7208c5a67b2df26ab1c"),
		new("item", "renaissance_jewellery_stp_hairpin_432", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "29993600ac55bcf2ecfb2a0a8ef1d88d74ad2279bf208db333329734f9f9ab21"),
		new("item", "renaissance_jewellery_stp_offering_bowl_447", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c62aa60b32e7546f0c1eb5dba09228f55021aa5e87624462fa80a7eef710bfd"),
		new("item", "renaissance_jewellery_stp_waist_chain_435", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1263aca0bd6323f5c3132643c92dad9dde7a303a030c5650f94340bd32635845"),
		new("item", "renaissance_jewellery_wer_anklet_08", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d3c3193e3f9c5c08173ba54c4f7003eb4091bfe31c0fa323ff499c85c7418b5"),
		new("item", "renaissance_jewellery_wer_scripture_box_26", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "294918c90f47032da52591b7bcdc29115a9e99734dd433cc599d2d1cc40fa12a"),
		new("item", "renaissance_joseon_white_clothshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "95902f7b0fbf4e06cd58d04fff952d719d1c62d49cc23ecbc27e1f938ce6e2f6"),
		new("item", "renaissance_medical_campaign_medicine_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3fab824d37d3b72c84d8728a60d2a22e2dcb24de99dd967959f301b07272ced2"),
		new("item", "renaissance_medical_guaiacum_wood_stock", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc83937b2bf42ac44141c659e2584763bc9c598c6fe290ba886d28218f502027"),
		new("item", "renaissance_medical_invalid_chair", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "778ad5e821b6302fe7fb9aee0df85030e9ac4f42ae540675920b22d3de93ecdb"),
		new("item", "renaissance_medical_padded_splint", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dddc950b695b19e4f3546d4c6315e85686719748a5c424434de2e5be2aea9721"),
		new("item", "renaissance_medical_parchment_dose_packet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "408145a384764d1b35030aa4da7a4b10c69b86784833e740ad08b09802c5198c"),
	];
}
