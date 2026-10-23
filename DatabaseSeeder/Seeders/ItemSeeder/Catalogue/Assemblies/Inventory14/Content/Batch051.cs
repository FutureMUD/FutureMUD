// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory14Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch051() =>
	[
		new("item", "medieval_jewellery_garland_fresh_lotus_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_lotus_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, "8eeb35312c4ffb14ba712a9da4722902a66f176771a49ef26744cb77d846a20e"),
		new("item", "medieval_jewellery_head_ivy_head_wreath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c55c1a2f351b5e3bf6f8ac97de048684af376c2ae2b45159d33500eb4e767d5d"),
		new("item", "medieval_jewellery_limb_gilded_copper_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e31cc2faf81be9d305b01dc8bc5aa075b4b73113b205190cf0508ab1f14467c"),
		new("item", "medieval_jewellery_neck_coral_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4cbb320423e54eca11ecb0bdd752020288845c740ca3fb6ea09774980e4c27aa"),
		new("item", "medieval_jewellery_neck_gold_pendant_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "08f3e24b5d2ce0447ae63b35c9c45da22ee6616353aaa318af1ec186a89a4b68"),
		new("item", "medieval_jewellery_neck_jade_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "547d565b2f9fce4757506861c888a9f0f48ae9d9e61c9a0bc7508ca12cb5bfc9"),
		new("item", "medieval_jewellery_neck_leather_token_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1ea65b35ab7eb0985ad9b9ce2e589c24e2e0e92b28ee3a8edcafd60e3c8e457"),
		new("item", "medieval_jewellery_neck_red_silk_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "33cc6f74161e5a0e2b20ecba6d81c5081d397d1352835128cb40fb292fc23392"),
		new("item", "medieval_jewellery_pin_finish_bronze_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9f2eb7c25ab554cbf83776a93ae750afd52787eac05b5c3bcdc5f1938564aa4"),
		new("item", "medieval_jewellery_pin_gold_headed_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "85a27b7a5d52cebbbfe9f71384e3d2a40168787a3ad743384b991115a4bf5b89"),
		new("item", "medieval_jewellery_ring_carved_bone_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7eec7c6ae3782b43484ba1cf203a9fbbd46988c70f37786809e2fcebe69d6d2c"),
		new("item", "medieval_jewellery_ring_plain_pewter_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ddd64fd20d8a48822abc1d7b99638cf188368a2d008c7b8e29257cb62d97713"),
		new("item", "medieval_jewish_synagogue_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e5a5442affeaf918e591670a16f7b0311a8281d520a8d6cf2380feb3cdd8e10"),
		new("item", "medieval_latin_stole", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef472e4b163f7c8c03590ca80304cb63f79c5cffa8b2fea0cd5d3feae5937ca1"),
		new("item", "medieval_leather_winter_mittens", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9136bce6c1353c508ddcfc9d886e82c1fd404f4e844d9e0cf3bbd817abd4785d"),
		new("item", "medieval_light_glazed_ceramic_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "475f47ba7c648fcb7bf78fa41eea905969042325c1d217b55eb27b581dd69da5"),
		new("item", "medieval_light_hall_hanging_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e654864664c4cea84482bf0deab86c79ea38dba79a190c24c633e8fa4c21d662"),
		new("item", "medieval_light_northern_oak_hearth_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "98118d231afed9edf015242092f3f160a82d949ff7d4e823d41d12ff8b622921"),
		new("item", "medieval_light_two_spout_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "04d3f218cefde1f5862b98d738b235ce65b6d7323ee888b76c2de28514e525cf"),
		new("item", "medieval_lined_leather_boots", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "76a2fc664d06ed6b50c9681d1c47615e1f76b85c6374536398bc443d7961e97d"),
		new("item", "medieval_linen_market_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8069030386d05f5c0558409c728ab49c2f3cf4c859a562bf44f4240a9ac8292a"),
		new("item", "medieval_linen_shop_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8cccc299e171436fd4be7d4b7f17e8bf5563fb3f97cd06ea45d8eb221e933725"),
		new("item", "medieval_locking_domestic_heirloom_coffer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ece0c816ddceb2f0ed03cb11dc9543beedab9750802bb74688eee4cc1ed0c2e0"),
		new("item", "medieval_locking_domestic_wall_niche_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "448bd48e147f0a5e5352aa67f851dff4a2ec1efc5f98fb361ef651afc530bca7"),
		new("item", "medieval_locking_trade_money_changer_cash_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c88b3e04e7d1cd1b33562bcdf0084105b88f7f3a46a54feacf656487d8c26e0c"),
		new("item", "medieval_locking_trade_toll_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "28e50af249d3e18318002dd9917b82f986a3e67a012a4be282ea6e8dcbf62b6a"),
		new("item", "medieval_medical_honeyed_poultice_cloth_stock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac48a1b0cbdc46079d94f604a4f4e2aa0b13b8df3bc068e86e74ad68da3d47ac"),
		new("item", "medieval_medical_horn_dosing_spoon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b58651ea849afae2328de3b6ee27c6d37ff37d7ff7816984a42aa16c064091c1"),
		new("item", "medieval_medical_plain_tending_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "40dd43c4fbd999636350c51f970b4096e27448d0a7ddb33ada0075dc6211f2c2"),
		new("item", "medieval_medical_wool_pressure_compress", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "54825a910b9d27e9dd1e5769af531232fabbba55b9fb2f72ee9ac416d320ef69"),
		new("item", "medieval_merchants_wool_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eec7580a21bbc14ec947d0cb60f457aee8411e5ffe64912245349770865be180"),
		new("item", "medieval_military_balanced_military_pick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "49329f1e84edce809564ab2b558131e775f958b2895f76471e8ed4ba8ff18f65"),
		new("item", "medieval_military_boiled_leather_bracers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbaf3e899c9d01b0bb5d372b875aed2464d6ea4952be8071ea1639fed78b902c"),
		new("item", "medieval_military_bundle_of_linen_bowstrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d300fb0facae613d74572b8f8671043b6be011f3b58f7aa4021c7073e254194e"),
		new("item", "medieval_military_canvas_longbow_sleeve", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1752b0936b03c701fc43d9b8af3a0a4830df597ef815eb59567e90004f9663da"),
		new("item", "medieval_military_conical_steel_helm", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cf2675827c170ad9968265ab24355b20cbee482a2beb353efb59ce23292a3cc"),
		new("item", "medieval_military_crossbow_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e9aaac2a346c52b4eeb9edccea4ba38f4790f0cfe409de5e883ef599954890f"),
		new("item", "medieval_military_fine_dyed_dagger_sheath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "99451e2d75321a2f16b0e98b78aeb805ba8bcc5840c971ed8ca76605084b6180"),
		new("item", "medieval_military_fine_long_gripped_war_sword", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "babc403fbceae05fb4434a284481d650db011ff855ba77e883367952fbbf185a"),
		new("item", "medieval_military_heavy_war_javelin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6324ca77890d2e282b9cb455583f4a298edaf056c57fbb7c33c251f8403e989e"),
	];
}
