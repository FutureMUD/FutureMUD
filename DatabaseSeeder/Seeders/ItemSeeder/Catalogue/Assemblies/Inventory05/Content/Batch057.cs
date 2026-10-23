// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch057() =>
	[
		new("item", "preindustrial_writing_trimmed_reed_pen", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "efba07109778c51c93b1a362bb2201fffdc33c43b302b6ffc016ad609ed889da"),
		new("item", "primary_production_bloomery_furnace", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "43d59bc4a3fc504f46e18c35c5625b45b335d35d42371bd47877528b5e77ffcb"),
		new("item", "primary_production_flat_chisel", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "38aa2093ed841b4aa0936af27ed7ee64b37cbc13565bf03cbe799dfe1681f583"),
		new("item", "primary_production_slag_skimmer", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2df4b6db8817dff1bd895d0eb725dd64a1f56ffdc489f96d4250e408f4ba7c7c"),
		new("item", "renaissance_africancourt_beaded_chestmantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "02a14cbbf8b804d053ecbfcdcdb6b33f8b6dcd9252014a1ac149bceaea553aff"),
		new("item", "renaissance_africancourt_broadsleeve_courtrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "63ed5aed1148d1c32ce3247e5090a06f64418c0c7dc069960410fc4e16be4431"),
		new("item", "renaissance_africancourt_doublelayer_wrapper", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ce1e728d14c29c255d9d4ce0a2826202f298271ab3f3699fe9b531840104ac5"),
		new("item", "renaissance_africancourt_featherwork_shouldermantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d45658cfd8781d8e22506f0827132cfc753257aa300518d8682f219e70ea3d7b"),
		new("item", "renaissance_africancourt_hide_prestigecloak", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "716bd7347a855a3a288e779eb97559c4f412a455f5f5bc1e57b391a4d9d6e5b2"),
		new("item", "renaissance_art_prepared_canvas", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a200b22e5df564a5a73e3355b10f257b776427b5e0aa76fe21efcefac6e38bf5"),
		new("item", "renaissance_container_and_trade_document_coffer_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ee0371a6a29dbca6056ff4a18a8666270870164c0f307c49faa8911f5c36b28"),
		new("item", "renaissance_container_cen_liquid_medium_storage_jar_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e10dce84a6a9a1831c534145504089d1306148e1b688716b2aefdccab233cd3"),
		new("item", "renaissance_container_mar_domestic_table_service_box_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a719b86ea5ad8275dae14f5371a645a850f394a82c217b0548b3b270c063a8a"),
		new("item", "renaissance_container_mar_trade_sealable_archive_coffer_07", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5352db89e831fbe5cb171a872f78799d8e765084fe0bcb5fc6dd7ae7c3cbe462"),
		new("item", "renaissance_container_mar_trade_strong_trade_chest_05", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7136c945ff69a6af87b532263190ea1e28bf6ef736db71f93948ad2d2f90cff"),
		new("item", "renaissance_container_mea_liquid_great_storage_vat_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b909ff9793585d2aefa5bc7002b55474caeefcb168b76636906173ddf5fafe5"),
		new("item", "renaissance_container_mes_trade_large_trade_cask_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e901c2b50372c9508349994076f9055c7591ce966cb21fdfa2090e460937428"),
		new("item", "renaissance_container_msea_trade_sealed_transport_jar_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07cd3dda87a4bfc9a30ba864ddf1c5fafca0435d857fe75ae66ffe2bff7f0ecc"),
		new("item", "renaissance_container_nba_personal_shoulder_satchel_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1e01ad13130a3bb85e0ce85bf63dd4bb1e0039c59adf154b1bc638395120d6b"),
		new("item", "renaissance_container_ott_personal_document_wallet_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae2a2ec87b85ae41ef8c03a04aa6f880f7915788cfbbb81a8aaf0d03223b180e"),
		new("item", "renaissance_container_ott_trade_segmented_packing_crate_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "33b9fd583a1718314f09dd5dd1e9aded07a45c854e8a08009d6a96ce0ed3eeb8"),
		new("item", "renaissance_container_rse_liquid_stoppered_flask_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "edc98606a8c43701dceac5652e45007701da1aae200ffac395ef8a3edb20ba71"),
		new("item", "renaissance_container_rse_trade_large_trade_cask_02", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d47eab5897eb019be9694793aba5640e8c54494a0891efd54334350f1ee3d87"),
		new("item", "renaissance_container_rse_trade_strong_trade_chest_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc4312cd034556251b6e22f60a5af67c5f80fadcf3c8442c196d4b81053b1deb"),
		new("item", "renaissance_container_sai_personal_sample_roll_case_01", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b412824d5b451ca950e274bc5181afed6e216a88548f6f0b979cf59602dcfb9"),
		new("item", "renaissance_container_wer_trade_large_trade_cask_06", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "872d39bde19367d5abf9e1edd25e51d95b52137d263adbf20a9f1b08e9e575e4"),
		new("item", "renaissance_domestic_mesoamerican_market_wooden_platter", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6670cb4d4c9aefb92b143fc698725170694192e02b75ec529f586b13b072f10b"),
		new("item", "renaissance_domestic_steppe_caravan_water_bottle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a059a089f1acd4352d77e650239976460851c47b6aa242a484a5277725afa3a"),
		new("item", "renaissance_domestic_urban_kitchen_wash_basin", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe690cbaf51ccc4e43b7d79d666ef27bd80c13745754586885ed5fedff06845f"),
		new("item", "renaissance_domestic_west_african_court_gourd_bowl", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "57c6481efa0635e6c086edc408e9d33dcb95c50046d7ac437b8de7de39b1a270"),
		new("item", "renaissance_door_aca_braced_door_422", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "10013d935f77a936c4680bfb17fc55d360720bb100251233ebf8a23cf3fe47fe"),
		new("item", "renaissance_door_aca_ring_key_440", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82d4c0c75d6ee4e9a78858b4394dbfd26aec0feb8e21785de0974f79b01e6c3a"),
		new("item", "renaissance_door_and_braced_door_576", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68199d480b7df3d78bf1d16ad069a78e04f2c0ff697f88f0501c88edb0553ed6"),
		new("item", "renaissance_door_car_garden_gate_598", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a424a70fb4c2784aa24fb1f7d5aa71487c58bc1324e89c68f45e10d52342fd0e"),
		new("item", "renaissance_door_cef_warehouse_gate_121", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3edb3b17d941ee307bdf1c19b17033c8637d948b4c9e6a359e6369748f59393"),
		new("item", "renaissance_door_cen_warded_key_75", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f12c806ea8a346fc7af644bb437cb19ede9253541e81be7ec01ec8aec45bbb51"),
		new("item", "renaissance_door_common_braced_door_114", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ef2fe4ad33883dbc3694593f75199aa782175066fff083e2725d36ec42ba6ec"),
		new("item", "renaissance_door_common_courtyard_gate_92", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ff2794a8ab0eeb58f4fb0bf659d6238f93c946ffd59eaa09aef36861976dd6f"),
		new("item", "renaissance_door_common_door_bar_158", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0659b9ce781b4274a1124ec81b67e807a2677ddc6141dcf01283ec836ca739ff"),
		new("item", "renaissance_door_common_door_latch_216", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd7257e6ed58669e9d71998d6f035da4c59e2a6b8e209555a7f06b536beeb108"),
	];
}
