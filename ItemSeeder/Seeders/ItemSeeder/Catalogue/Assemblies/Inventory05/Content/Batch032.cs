// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch032() =>
	[
		new("item", "earlymodern_furniture_mrd_bench_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4633c7503c30603dfd962ca2b265ccbc7efe45dadf53cdfae768df96636733bc"),
		new("item", "earlymodern_furniture_mse_drawer_chest_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eca9ef71e8c0880ae4047167b752498b3ca9b2666226214915ddbf79cc2a7110"),
		new("item", "earlymodern_furniture_ott_bench_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a49b6ae769674bb72cb42d343387f8da7a9e4bd5e7b83c2925c4caad46c8550"),
		new("item", "earlymodern_furniture_ott_linen_press_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "17b1d695e551b844bdf494eefcb74102dad1b8bca8760fc0d16b6df898e03474"),
		new("item", "earlymodern_furniture_qin_archive_wall_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eddb24998c57d8e3d751a02f3ed46848f8576e92fea03c3e2f17f881cc0b9fba"),
		new("item", "earlymodern_furniture_rus_display_cabinet_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6285948ab1a570310ce2c81b61e3fea1b6b6a451cf3d8f1ebebeb423afaa435d"),
		new("item", "earlymodern_furniture_spa_display_plinth_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf2cded24ee07e11fca366955106469d73f8024da57fee0425a4b4d39d07f140"),
		new("item", "earlymodern_furniture_waf_dining_table_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "188595ff5ed76b752edaa31918b110cac669e369f15a5ebd405e806dbe838552"),
		new("item", "earlymodern_furniture_waf_side_table_09", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0e2e34bc2dc45a892a8527e27023ae82f746022c8a52cd33ef0618b135de445"),
		new("item", "earlymodern_globalmaritime_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3496aef88b7f3eaf99ca4456b152191ea692a24bfb0a60c33926101e9bbf0186"),
		new("item", "earlymodern_globalmaritime_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "960c3f2f05bc6393e5dd7728feafa58253a500fca0431948380c85e569b46942"),
		new("item", "earlymodern_globalmaritime_officer_company_frock", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fafc88bf0089fed57d232ffa24b4f8081fd45991c636c05cd6637a03046e143d"),
		new("item", "earlymodern_headwear_caribbean_palm_sunhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e0d9acf9b44381aa66b96ae350225f97aef46a720b86a5234b1c1a151e1d63b"),
		new("item", "earlymodern_headwear_fullbottom_court_wig", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3453124a77de23efa3062b1cb42e292163267c31f661e77a2e3297ea4faaec2e"),
		new("item", "earlymodern_household_pendulum_clock_case", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b0aff55e9ead888a0e76507587996c25fd9c8f77a3d47d0995ec42c69a6427b"),
		new("item", "earlymodern_household_spittoon", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "66568f795ad5a26b2ddee64074f4e837b1c3d2adab827275955b31d871fdfa13"),
		new("item", "earlymodern_iberian_clothing_short_fitted_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd9fcfcb720d7e467a23dd747f86295b87bc3c2862479c959b918426637a23be"),
		new("item", "earlymodern_iberian_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50a9f8ea1165b16b7eaeb4ce62605c30d7115eeb2de4533c5b80528e30626048"),
		new("item", "earlymodern_iberian_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9181b24f29d814dfa818fa7a3c34ac71a5b249e0a136b7aec0e6e414529dba29"),
		new("item", "earlymodern_italian_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1a648b4f21cdf64354d089230af73ad402a08e072e7576d94dd018e375dcb77"),
		new("item", "earlymodern_italian_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "56640d83b7caae2e001f3c8c1732d97e355ad95c474b115c25ba99836291b003"),
		new("item", "earlymodern_jewellery_and_signet_ring_662", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0bbc333940a81dbdd4d05677bac2c7326cc5c9083106f1d2e24ab9e9e334b510"),
		new("item", "earlymodern_jewellery_bra_hairpin_592", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "581ba553677eb17d47c7c9a540c93bbd19ee4063a1627b69ac05bb98d763b187"),
		new("item", "earlymodern_jewellery_car_circlet_693", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aaeb143e49b6a22ed1ccb0e12f653d05e39b06e682e897aae893a1d586292285"),
		new("item", "earlymodern_jewellery_car_earrings_689", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e91e4ee144c2c4c9a3dbc38d72e986b333e860822ee981bfacaed42235e06569"),
		new("item", "earlymodern_jewellery_edo_temple_ring_378", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4678b366f598a08a9ff2d66c8f96f75c354bf534a8647646aa242469bbcd852b"),
		new("item", "earlymodern_jewellery_eth_brooch_530", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1c8c07cd17dd929ed60046d6a58cbf3ed7717f2f5ec024a5c97dbb7a3a11ab6"),
		new("item", "earlymodern_jewellery_gmt_anklet_708", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "93cc990648d1db16f58456058d7bf16ee6d8349184e1b250cb6f121e54c6908e"),
		new("item", "earlymodern_jewellery_gmt_ring_701", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f8045bff53a5df42718c90bdd8170b624e3e114344947f84b9e9fdc4c399dde"),
		new("item", "earlymodern_jewellery_gmt_signet_ring_702", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e311ed8dc4a7a7e04d64f2666f75bba1b96c861aa78b41a4f1ecf72865846ed"),
		new("item", "earlymodern_jewellery_ias_anklet_448", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eda54bcdbea2f93e308d339d6fd872d9adcac056c2becb8509ecebc37413c498"),
		new("item", "earlymodern_jewellery_ias_badge_457", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6db322b0353abad361cb1a0b8b6bd022edeba4765efca3ea21c34bd736d9e73e"),
		new("item", "earlymodern_jewellery_it_armlet_107", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5158cffe800ff99531e53f48ad0e64b78ac1ed0ac3afbe2199da8527ec15d38a"),
		new("item", "earlymodern_jewellery_it_bracelet_106", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d99a27e909c71b02b5c37815b11bd80512330668e58ae4dd014e63861844113d"),
		new("item", "earlymodern_jewellery_jos_anklet_348", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ceeb2b9a4cce083e0cd5ae9f48db2cbb08e6ad152c4da67bcbb1bd8ae47936c"),
		new("item", "earlymodern_jewellery_jos_signet_ring_342", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f44c20157c1ab3f68a6e006e97880546da482b2ef0d121dae0d0080e5da64a11"),
		new("item", "earlymodern_jewellery_kongo_armlet_487", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "60741a94b6ab59bcca638825546cae5bc6a3e2ce1617a9faae3d8e8f953a7627"),
		new("item", "earlymodern_jewellery_mag_bead_necklace_204", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e960f8141dc47d1c472cb4504df9099c7d24bc3f9e7384404d82d91a4fdeb0f0"),
		new("item", "earlymodern_jewellery_mag_belt_ornament_216", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e078ee93a7adef232f1800698464fbbc6ff96cdef720f7409b9adcc39c84b94"),
		new("item", "earlymodern_jewellery_mag_pin_211", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa89bebbbf0677d78dcdb7a14568f4e3620cdb0e69cfefbe7d4bc93ec2bff907"),
	];
}
