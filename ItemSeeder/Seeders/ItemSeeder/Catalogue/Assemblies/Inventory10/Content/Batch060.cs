// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch060() =>
	[
		new("item", "renaissance_institution_academic_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3fcb19a0d1c8bfb1bd16cb3fab371d68f1e78e33cffa2ec6d234e418933aa9ec"),
		new("item", "renaissance_japanese_broad_sedge_travelhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f24923f9d8b3ad00693a88a73b8a059649bae5739d77fd51ca2daf3f0cde5091"),
		new("item", "renaissance_jewellery_aca_belt_ornament_466", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "85f4bd6df6d2d35418012a3a72345d2887d8d29ec8429423417baac86e92739d"),
		new("item", "renaissance_jewellery_aca_chain_necklace_455", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1312c75e0601315ef20388052e1f2090e5d9f7d5f68f8997295faf6d4f728e5b"),
		new("item", "renaissance_jewellery_aca_ring_451", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5724e2298739e815854bfbfd59b8a0b595cbfd10a5d5a0f30c889f0a1a327a3"),
		new("item", "renaissance_jewellery_aca_temple_ring_468", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "22a1285b14eea5f629b8068813a28c2de6d1f5c3edf83b09db92133d54d8a084"),
		new("item", "renaissance_jewellery_and_prayer_beads_621", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa63ee5c24e4b0e47c26dcd9c6b8dd5365fe04ae2472bfaf4770049bd06fdf50"),
		new("item", "renaissance_jewellery_and_torc_619", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "403c0359aa0338ee452dbf9526582a5c8f897efbb626644e001d9994b3d82f86"),
		new("item", "renaissance_jewellery_car_armlet_637", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "daa50e5ec93a9c1a693025386795880e3b27c99fe0c90ce0114528496e90ec99"),
		new("item", "renaissance_jewellery_car_torc_649", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "392b0b314a8def1a35cd14d7baff12faf6bee9da61a04862b0c6cf952018b457"),
		new("item", "renaissance_jewellery_cef_prayer_cord_144", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8a8517b7e38095c23719deaf870b9f237924b2e3333a779ee9a5223c7189747"),
		new("item", "renaissance_jewellery_cen_portable_panel_89", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d11a0184f78c3cbe7e4d204cf04aa32a2aaf2f05af52c01f48ee48da4e65b3e4"),
		new("item", "renaissance_jewellery_cen_scripture_box_86", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61c2de7579d594fc896ad8731d1f0bc44bd07552c299d091ea619f3182304a26"),
		new("item", "renaissance_jewellery_cen_signet_ring_62", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f9eabddf466013621e600a34b8f6417dcdc1c6fbb5b2f1d3943b1cfe6ec6006"),
		new("item", "renaissance_jewellery_col_badge_707", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bce8dd7d8ecdf9f249a3f7782592b9adc708bb4351106d58365a214bb44b17c"),
		new("item", "renaissance_jewellery_common_bracelet_156", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bc788fcce950ec1b30efdfd177bbf6d6f1288119f7b47ec428b1fdcf943af1f"),
		new("item", "renaissance_jewellery_common_bracelet_66", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67f2c90106e4be1ee292bda983b5362c4004a3be9ad2060003d93fb880633a8d"),
		new("item", "renaissance_jewellery_common_brooch_70", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb7792542d33b46d20f5234d06d5be5625b343233ecbacc937b03b8d871dda74"),
		new("item", "renaissance_jewellery_common_chain_necklace_95", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1242d8960dc1c5970d30935a7ebdf6d39f9eb6e9340da7578697a0cde661bdc9"),
		new("item", "renaissance_jewellery_common_hairpin_102", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ddeab3bc812f7b165c55963fda410605a080390f70621f07e76e5918ca2b490"),
		new("item", "renaissance_jewellery_common_pendant_123", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4dec1c51074b7b4cc303bdd956e2a52f0faceff045acb77d16fb2e37f71e55b"),
		new("item", "renaissance_jewellery_common_prayer_beads_201", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "560049489c02ee1b67ea89fe4bb633159ab2a6f5bb56dadd65aed4c1eb1a37fe"),
		new("item", "renaissance_jewellery_common_processional_ornament_90", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c96437578a14319959c257d7a588ab50c936c95156c0ad36016eb5ecc3ee4c3"),
		new("item", "renaissance_jewellery_common_temple_ring_198", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d687c9850a85189e0f4dafc6e3de031c38c8158186826631718cc08e0527f28"),
		new("item", "renaissance_jewellery_common_torc_19", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dca37cfa35ba8e75ee23eb1979393bf7726a45a7fa9d33b929d72af735aa9a2b"),
		new("item", "renaissance_jewellery_common_waist_chain_75", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3527e641ce1ee30febc87e05c321a48b73a32775e98c215e0425948eede6681e"),
		new("item", "renaissance_jewellery_common_wreath_50", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "02130780f67c62d1971733cf567e859070ac9a4a90c11993b1ab75d429c5f9e9"),
		new("item", "renaissance_jewellery_eal_circlet_283", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "96ae3d411c9538fddfc69361705763c37305ba28c330e1c317bc9f6a6bc3c0f1"),
		new("item", "renaissance_jewellery_eon_badge_167", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36774ef3cc59625f84887c607808774561fc14198659f0615584bb3d76b7e559"),
		new("item", "renaissance_jewellery_iba_bracelet_36", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "753f461ce3a751819553fbf165ab6ace97d7a6eea39605ecc274ac23899326b0"),
		new("item", "renaissance_jewellery_iba_circlet_43", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9733fe113580456e4d52d81bed713693ade298f25e21c54f3f1e0e3876109eeb"),
		new("item", "renaissance_jewellery_iba_offering_bowl_57", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e663cfd35fc88e633b13e957585dccfd689d64318bfe4cf069b6c97caa1de326"),
		new("item", "renaissance_jewellery_jpn_brooch_310", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a565d922cccc8095db4a4c7bfcf573aa8b1894f627c56a6a99eff6c88ab7340"),
		new("item", "renaissance_jewellery_jpn_devotional_badge_323", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "741e566b46d91f84d27df27c961bb47bf75805c3a7779dbe383e6ea55313059a"),
		new("item", "renaissance_jewellery_mea_earrings_339", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc154c6def7f25cf7026ed9ec20e3c1dbc13f6e84d9eee652701b38e55c9a103"),
		new("item", "renaissance_jewellery_mea_prayer_cord_354", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c56b0fdaf3c57e4396a498bc092b88ab51e9558cb8705b1ebeb0af271bef0aa1"),
		new("item", "renaissance_jewellery_mes_circlet_583", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae9547f22ba216e36c4e6138df45e94a438140601c1ab464f1b62849ad700d56"),
		new("item", "renaissance_jewellery_mes_devotional_badge_593", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "60cfca64af9b503dccea464e453f92f9ee7933aa6cf4e3a53b325db4bf4bb95a"),
		new("item", "renaissance_jewellery_mes_prayer_cord_594", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "719bd203f403678df635acf3fa5e5da925961d66a9ef0be6a0117a7394fa91e1"),
		new("item", "renaissance_jewellery_msea_chain_necklace_395", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2bab47a2968335bf2cec6bad0656517fe495c893274caf65a02270a0501af5a"),
	];
}
