// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch049() =>
	[
		new("item", "medieval_industry_stock_plank_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "53db6412ec3bc6faabac26677322192d75e78829c2f87093d07730d82b50b76d"),
		new("item", "medieval_industry_stock_seal_wax_stick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fb09f199e955835b4d9d4665ea503074c03696e636adbb5bfbb9f91377bf3eb"),
		new("item", "medieval_iron_buckled_leather_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f3eec251d60e91e7adc54e994bdd923f8f8939d7a066448489fecfdd43f73e4"),
		new("item", "medieval_islamic_ablution_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bae16fde4b9aaa6a1e8c67d870215e725df99a3ab6c11f1c902cb16b116edbc1"),
		new("item", "medieval_islamic_mihrab_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2904313ebbe0a36d3aea9cf181aab45b670f2579774b05a177bd6fe3f95ed5d1"),
		new("item", "medieval_japanese_work_kosode", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "98b6d0b55b739e745759fea2b0baa0cce2051beda23deae562bd3ccdf2fe6496"),
		new("item", "medieval_jewellery_brooch_broad_silver_cloak_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "91527183ed2171c18b288021bc25c83e38059804ebdb74dbc422f9cdbe8459e9"),
		new("item", "medieval_jewellery_brooch_fine_gold_court_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "716336517964aed78487bfbe8f2512236923632dc9f36cfcea70c5defd51c50a"),
		new("item", "medieval_jewellery_common_little_shell_child_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3b963f8fbf505822ae5bfd9ab5f614aff5878e0e004d8bf9a936cc1a4ceeb8b"),
		new("item", "medieval_jewellery_common_twisted_copper_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4f8acb5edc4e7008d29bab7b7a2b4a47d287d63b83200e40622829b00116af9"),
		new("item", "medieval_jewellery_elite_pearl_and_gold_collar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f633438f921eb9a086ecfefdde93ce020d259ace07eb60136e7047bff9f3f5fa"),
		new("item", "medieval_jewellery_elite_pearl_forehead_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7050f3ef39c3d915adffcf12d08aa6c67a267ff060675d543465733db3313097"),
		new("item", "medieval_jewellery_elite_sapphire_diadem", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "99dc83e2b93bb1b7a58db7bd55101b6c2e0505f2cdd17760c554f37562a261c8"),
		new("item", "medieval_jewellery_elite_silver_gilt_noble_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0eab2f33597c11e7b5a492912d52103266adfa30e5142e5ee12abfee23142022"),
		new("item", "medieval_jewellery_head_flower_head_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e1cfa7e064bb8b51c301e3da3b3d98f7b519bff011a6bf9ad622c30ddfd16a8"),
		new("item", "medieval_jewellery_head_laurel_festival_wreath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e58f728d029c4454ad12a1c1c21dd0655b882e00634ad4857c29d19e244c3ad"),
		new("item", "medieval_jewellery_limb_bronze_upper_arm_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c475beed3e04375f818ce6cdaef3ef5ca5dc44a68cdcbd006b7a548da8cd4944"),
		new("item", "medieval_jewellery_limb_conch_shell_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a81fbca38fa30c2689154efe417e5444037ee31f36cdc7732020ede00bde02e"),
		new("item", "medieval_jewellery_limb_heavy_bronze_arm_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "64cc9cccd507913c288389d697406c22c9dc009caec2463f9111fb5cf79565cd"),
		new("item", "medieval_jewellery_limb_motif_brass_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d18b08a11d3ed9e6be3535ffaea28ab1f14950c7d16d182f0632226d0ef57636"),
		new("item", "medieval_jewellery_limb_narrow_glass_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e503b782809130bea4d7adc812f19268b2a1f21bae0925ea9e7e12dc12ebb235"),
		new("item", "medieval_jewellery_limb_paired_conch_bangle_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8309a3ab5b1e6d878aa41d1c42c489583529c824cdcee0a4d6ad6c7a3e556e6a"),
		new("item", "medieval_jewellery_neck_agate_bead_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "62fbd31b04bcdee988ec57091298edb50b78fa05313b9e73642f092b19c4f723"),
		new("item", "medieval_jewellery_neck_double_strand_silver_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f27098be0d4e4c7365cb73168c9ec5b20494b69aa697d3460e48a3bc56b658dc"),
		new("item", "medieval_jewellery_pin_bone_garment_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "34c08df841cd15751a5a364f306afae64e097526581a3f785f88adf0a70c953c"),
		new("item", "medieval_jewellery_ring_amethyst_court_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "70b8c11e397b43ba2ce63f13f93c98a0fb6c0e244efc76b33f685aa33a8d96ee"),
		new("item", "medieval_jewellery_ring_paired_courtship_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2244b826d39361aac9cbe12bf9badcc1715c4d0e7d7b8665fcfd81f5dcce6a37"),
		new("item", "medieval_jewellery_waist_brass_girdle_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "747b472e1cf79ce67ba6f039cfef98784a75ffb5bf028f5723c9cf92754df659"),
		new("item", "medieval_jewellery_waist_set_of_gilded_belt_plaques", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b774952ece45bb04694c755ebfc3635079fa89008ccd72b14b3dc7ac014119b0"),
		new("item", "medieval_jewellery_waist_shape_belt_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "40f80fe205912af3bde11b4a189a67e5ec3b065aa6cb8d3792beff063110efe3"),
		new("item", "medieval_jewish_tallit_gadol", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "216dc82341c80f2f42fbd97b8316d3a496eb1579ee29b53e5f24e10f1ca469d8"),
		new("item", "medieval_jewish_tallit_katan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3af14a2a732040ac8fedce1718c1e9d4252a4a8790496f82df6fa7c6e7dd52c8"),
		new("item", "medieval_layered_uchigi_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a7240bdcd26bd82fc8dcfa1c45770ceca0cbc82bea712936e6e185c6091d0f7"),
		new("item", "medieval_light_birch_bark_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b112117a904da43335559828a696347f7faa1ce0157b1c591ae1146fe38e55f8"),
		new("item", "medieval_light_clay_saucer_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "06f89cb7a7b16f19458afb28285061cf7b456d1bc963f506504690222a35a3c0"),
		new("item", "medieval_light_east_asian_porcelain_table_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1eaee4458ecb1bf6e5df551c23403792069b730d335fed247fbd1c330ae3658c"),
		new("item", "medieval_light_islamicate_cedar_lamp_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d0b7f443403698316f5ed56d366d27e7740975d11765b6385736aa9a14b8cf5"),
		new("item", "medieval_light_pricket_candlestick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4aceb32371204a46953f962db997de80ccad33ed927baf269ac784f2b91bdd10"),
		new("item", "medieval_light_travel_oil_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a5d2c605574779590aa5ef5da4c4df7d46b07e53c5442a5a081ccba1dcf45ee"),
		new("item", "medieval_linen_izar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfc4919c989eb8e2ee4aa8eb4ecbe9fd75cd49d9fc184d5cd5948c2bed41d1e3"),
	];
}
