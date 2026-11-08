// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory13Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_jewellery_elite_ruby_coronet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e63e57c503d694844a0056f1f2fd3553b0bd377edc6f082d89d743f0efa80495"),
		new("item", "medieval_jewellery_garland_wilted_laurel_wreath", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_laurel_wreath"], ItemSeederOwnershipPolicy.RequiredRelationship, "e55931acceb0b81842721e04471a908a02080dc6b8aa57207e545f704b56006a"),
		new("item", "medieval_jewellery_garland_wilted_marigold_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_marigold_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, "725715b0eb9d30f50b924a33347a0e34d6634d8218334431dec9f9c1e986bdef"),
		new("item", "medieval_jewellery_head_bamboo_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c4fce0c9a92357fa1c6bc3f4cc6b2e824094d438205f36386e8c9456570a383"),
		new("item", "medieval_jewellery_head_bronze_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fec72b14f98421bfcff9212880d60ef0ee86c23f0ab3b5a1212baefc2ad9e764"),
		new("item", "medieval_jewellery_head_gold_forehead_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f95f2de9e5f7cb18610be52908d3a70bcfcee1d1ce37dff0690b7aef222fc31"),
		new("item", "medieval_jewellery_head_pearl_forehead_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c618e42778e1cfb1bc6dba8d7fae1583e3f7695ee7615ecdde7b64a140df599"),
		new("item", "medieval_jewellery_limb_bell_less_bronze_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a97d16737c31707e0a69ffa3e7b35bc23ea110444a5615c8e774f155f84e5457"),
		new("item", "medieval_jewellery_limb_bronze_cuff_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f32972bf3d9de04ed67499ada7759a6f2a2ac1b0bf3252753a84a9e5dcc8dbb7"),
		new("item", "medieval_jewellery_limb_finish_silver_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a667daa8146e338ff00e9daa314c603c6c5acbf17f54aff652c92206a33176a"),
		new("item", "medieval_jewellery_limb_mother_of_pearl_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3eb4e9cf4b9665c1843fd7bbc6836b753d27e5561089e2b87d71725fc2b5ff50"),
		new("item", "medieval_jewellery_neck_gilded_bronze_neck_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b635482adbf83b0af74a5ea71918800fe60105b4370672627caacedbc180316"),
		new("item", "medieval_jewellery_neck_horn_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "95803e7a224f29fcf49894ef3dab8b37a201207522275766defe75252bbf9748"),
		new("item", "medieval_jewellery_neck_jade_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc25270553cbb1f60fa2e623cfa9c787e387898d37e2aeaa9ab084919f4b5775"),
		new("item", "medieval_jewellery_neck_lapis_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ac1b48bb5536b14b6fc6b6b16150af5be9610231134f186e0de089e8dd459dc"),
		new("item", "medieval_jewellery_neck_plain_bronze_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e67c65705f42b8feb41e18aa8b78b40b59f5890e9a75d89f2b1879f8acdf99e7"),
		new("item", "medieval_jewellery_ring_blackened_niello_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b965a5417e5cae31f72aee5ccd4d6b1b64b0e5d8f1863e6e6eda970997153c72"),
		new("item", "medieval_jewellery_ring_finish_silver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b6ac203ea2853ef89780974c6bec938186c8a5d4e18543707f6a159e13e46ea"),
		new("item", "medieval_jewish_wall_tablet_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "048412eb52cb11e14f76d0dc632b0eb3e4435abf4dea88d8241a43152475a822"),
		new("item", "medieval_latin_archbishop_pallium", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9debba69aadce7a39a3ff4f2a66e909fec2ec211950244fffb5c90b0dfbd405c"),
		new("item", "medieval_latin_processional_cope", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfecc19d5929e74cf10b0266e81aad225b769d89562645a7ffcbb3758934f38d"),
		new("item", "medieval_latin_rope_cincture", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfdfd26f6619f91d0410b33b591b4c6e029dbf0a4f866210e81be135be7f53b8"),
		new("item", "medieval_light_islamicate_bronze_chain_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb8e4c65562ff69a71b0d86e0c1b2a1feea5b9804eb97e40e6f511bd47821587"),
		new("item", "medieval_light_lantern_carrying_hook", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6269c2a6942249838543b886654aaca7ccfe3a00bdcc52c15ee5e880cc67ecf"),
		new("item", "medieval_light_long_watch_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4906eeafba31782cbbd38d9fd4c4ee02a8b1481dd0c5ea43fce535d32bd9815"),
		new("item", "medieval_light_mediterranean_bronze_hanging_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ebab4c9e782e40ed19e45083a92fa64399d12543ce1eed70e741e0a08ec0538"),
		new("item", "medieval_light_northern_iron_cresset_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "be079319662ff17db0adca00ec61116c686c24207f6fbc0226029628fec35a94"),
		new("item", "medieval_light_northern_pine_pitch_brand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f35366f51ea028fde87150f16adce949545a70414bab5cf53d124f063a61af4"),
		new("item", "medieval_light_south_asian_glass_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "625d6ff868868d5c84de9cfa0ad66a78d75346add54f6c6177e403f4a70c6e65"),
		new("item", "medieval_light_steppe_lamp_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e999110532ced7ba2f3bf1e234bb44c00f34e68e7a5bfb7ab03bf5503672935"),
		new("item", "medieval_locking_domestic_locking_footlocker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "49f4dc9de77f233bba365ada2a60de6c297c135b6cac60fa700b07b6b722ebf3"),
		new("item", "medieval_locking_personal_keepsake_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "63825db0eaab788411b0b44d00378e096a1773802077c1e9a423ba90ff97d5af"),
		new("item", "medieval_locking_trade_clerk_coin_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a92b97fc370e5cdcf1f1d8f8f43fac6eb2bdae0f84ac954b84420d39e9d51100"),
		new("item", "medieval_medical_aloe_burn_dressing", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6930ca6297d28c2aac565281eaee9f871a7cde25f1af44d1774932915b5719fb"),
		new("item", "medieval_medical_bronze_arterial_clamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c5db9bbf366bba260c6a4bb03de019be06bf618fefed9aac25f5b023c7017b4"),
		new("item", "medieval_medical_bronze_cautery_iron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "94b3920a186c7fb41c5c98d2d6f5caddd1daa56c55f1e4547c355ef7025a8038"),
		new("item", "medieval_medical_padded_leg_splints", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "88326c633a768fd34efa6a844a1ab6c570e161a770882bf2c3e57f020c2314d4"),
		new("item", "medieval_medical_padded_willow_crutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "62ff72228189c87397187d4b374f735526bc1f093652b6390280d54e6be7609c"),
		new("item", "medieval_medical_stone_mortar_and_pestle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "894d932a054201348e3c5b24b9f2fe47ec9a1e1a13e6b5fb4b3f49511c9ec252"),
		new("item", "medieval_medical_vinegar_wound_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a908172c8a7ef12349be7aff1e5f87b8fb37bdebf935aa5f816bc842928e7e5"),
	];
}
