// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch036() =>
	[
		new("item", "earlymodern_military_ranged_military_shortbow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "286152c5bcf8ea3b77eede2c36a5c0e510fa1e5b4eabe357662fdd9c0bcaf7ea"),
		new("item", "earlymodern_military_ranged_rattan_self_bow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "03f9f37f6511ac352463a7286e5a6f2cc7aa779ed11eeea841dbb0a129aeb16d"),
		new("item", "earlymodern_military_ranged_rattan_self_bow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e026cedcac58650c5790562a71490ff113331264775301562f21ed20f98a7993"),
		new("item", "earlymodern_military_ranged_south_east_asian_bamboo_bow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "84a20d1dbcc4138758c1520cb9e8a8ccf11da5afdb7e89db932918a295a50eec"),
		new("item", "earlymodern_military_ranged_south_east_asian_bamboo_bow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b63314fe5a02c941e98dfaf480f3427cc0e0adc72f2a4c60a465ed19e6b00b22"),
		new("item", "earlymodern_military_ranged_throwing_tomahawk_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8744c1506932835d41e87f453e1fe560b10d9d0c278f40cb964ef5fb9e4bf28"),
		new("item", "earlymodern_military_rank_brass_breast_badge", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa73b971b54530316fed06b1089e8b7392207a9aa6192a1112d95aa61f824dd2"),
		new("item", "earlymodern_military_training_practice_knife_sound", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "983f703b724a0fdb6d8c3b208c6b8e4d97985d3b16534da0e9f9e57cd5b95289"),
		new("item", "earlymodern_military_training_practice_sabre_rough", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "38b29bba8cef8940c0f401e49897d729bc71cd8ad314e3974baff79d17be7419"),
		new("item", "earlymodern_noble_shared_buckled_court_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ac17378cca52d7b5687dde0490e9fe780b84fbc596e190397c00c9b152dfaa1"),
		new("item", "earlymodern_noble_shared_lace_head_veil", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb418a8210734c3ae0c882257d16d8e99305713bd248ad2e314c747c7b9773a7"),
		new("item", "earlymodern_northamerican_clothing_tradecloth_shirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b2a6cf1c65cfe402b7a11befd38a4f27e0fa1a82ce76c8a63032e2a892903bd"),
		new("item", "earlymodern_northamerican_military_tradecloth_warcoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b3fc026f66dbd8877737a898eac922809e4807045471cdb455e98fbfacff21a"),
		new("item", "earlymodern_northern_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7aedb85047d9158e6e435da50f76a1ecbc255ecc5e66b3cd4ad372b5857a6cb3"),
		new("item", "earlymodern_qing_clothing_cloth_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c8eb8d480b5f09cd35a6d734799806754c6e58bdc0621ba7ea9433dd2c0751c"),
		new("item", "earlymodern_religious_joseon_simui", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be1dc987559c44bd614501ad33087e7859cf593512f34ac07bbc0380089c6b46"),
		new("item", "earlymodern_religious_tibetan_yellow_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "27a0aff69e9dc2c9a046b0d086d330cbb067a5da20793a22ea1de5766bee338e"),
		new("item", "earlymodern_religious_western_biretta", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "05b4fa2baf8d3e154f9a33f8bf58020feed958c2da5add4af7e2295f91729333"),
		new("item", "earlymodern_religious_western_episcopal_ring", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fcb4836463296eddee9c237db74c71aefb9db8d80ac3ab2fc6772ccb08c7339"),
		new("item", "earlymodern_religious_western_narrow_chasuble", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4ed106a42533127fa0aa97b071ea6d239974ec3f856ae8883fb986aaa007bf9"),
		new("item", "earlymodern_religious_western_pectoral_cross", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8bbe44187ed66dab857e0eb9decb6eb8e3d688680ac622fc35010cdf3cc2dd6"),
		new("item", "earlymodern_science_parallel_rules", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8891957594aebd4d127ae5c69f7c5fad3b448a7339f0aca7b044495e2ffbe6ef"),
		new("item", "earlymodern_spanishcolonial_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f8781748a503c254444c9131951ec96e424474c4b6af845094d305ac49996e2"),
		new("item", "earlymodern_spanishcolonial_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7493a58f2c1c48bd80fc591edd4bdb96251901ddbcff3f99edfd6503501da133"),
		new("item", "earlymodern_steppe_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f463785d079ac7c1f2cdd45bd9ee750761009edda3b20fdb1fdbc0a631151ea2"),
		new("item", "earlymodern_steppe_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a932a586b6df166335559c9540b41881e18eea9b05029ad09f0a538a18a61149"),
		new("item", "earlymodern_swahili_military_service_turban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e038ae83bd146c31bacba917cea494e4980b55d1ed7a95d000004d32663dcb1f"),
		new("item", "earlymodern_westafrican_military_sleeveless_overrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0823b2660af686a4280b4b76ac67f73ccab6d0dbac4e964efa34c20690959cb9"),
		new("item", "earlymodern_westafrican_military_stripwoven_war_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e496aca6ef8bed9d590da8ce8e1b3a37bc70fb2bfd780af6fdaa4b1a2574de10"),
		new("item", "earlymodern_western_clothing_long_linen_shift", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c38567524ffa6d6574ff276630b47e03b3c90146f509d85c821f72050e978b2f"),
		new("item", "earlymodern_western_military_broad_service_bonnet", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2fa86758f59ea8b8fad047a1250dc3b5749390fa5ca6b48319c897ae7057c682"),
		new("item", "earlymodern_writing_bill_of_exchange", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d44ff795e516bec891ed1e76eb35cc1f2ebe893d9e037df28ccdee7dd8328af0"),
		new("item", "earlymodern_writing_port_register", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfb664b3bc85237e97b7af8a98357a24c97df8347fe8c846dfad3427b778f0fc"),
		new("item", "historic_oil_lamp", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b30e0ca9df84e5c17477202b7e99fb361aff2de122f73b972e1748c5e61e6f7"),
		new("item", "historic_sewing_needle", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5781b7af4f890fa5ff022b6894974ccfcd10dfc6e5bf201e7df6c2cbf324aa23"),
		new("item", "historic_updraft_kiln", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "401b305855274ccdd97e8cce075ec8674926dd425bc541719f05d1cd91acffb4"),
		new("item", "industrial_agriculture_forestry_fishing_0024_wall_mounted_fork", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "442f83e4f558ab54a830a6d3caf4393301b44dd65f4d8cf4504bd2ae9f3fd0b7"),
		new("item", "industrial_cleaning_personal_care_0001_wall_mounted_waste", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f01c566e8b4dd6e35407ab2fed991a8c148c7849bd5059eddc4060f2c0ba97ea"),
		new("item", "industrial_clothing_footwear_uniforms_0014_plain_coat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6775482068f30eda41915163f8238735ce32c67acde50486f0d460a41f18822"),
		new("item", "industrial_clothing_footwear_uniforms_0061_folding_rag", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bd01d27a2087d7c80c5e263fc53508a64e6ff4f7d3d85499d4c8e975c3d253e"),
	];
}
