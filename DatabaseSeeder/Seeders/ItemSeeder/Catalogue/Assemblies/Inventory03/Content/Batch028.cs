// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch028() =>
	[
		new("item", "antiquity_tableware_punic_red_slip_serving_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "edb9482d9d84d5f5f5712e81154c7a278adf636ee7b68617ad744ab021533c3f"),
		new("item", "antiquity_temple_bar_lever_lock", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f795f29783f155877cddb259f378ae89486ea5bb7737c6969e9fb12ff5800b29"),
		new("item", "antiquity_votive_offering_basin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e14fb7b98f4fc15ccd3d15701d08e7af8aef2cfabe44e2a82bb41c3dcf038924"),
		new("item", "antiquity_weapon_common_hardwood_club", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "075a2b7dba2ab7b84902f33911b5683fc02387ec35d5a96de50587ce756fb9dd"),
		new("item", "antiquity_weapon_common_iron_dagger", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac37242089190420c58e9e3b6eb9adcee2f692dc5d02576dd1ee874e27841dee"),
		new("item", "antiquity_weapon_etruscan_bronze_tipped_spear", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "406b9bccfc92a02732f69477f0db25d3daf80a1bf39fdd163605f899d0cdf493"),
		new("item", "antiquity_weapon_hellenic_dory_spear", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f2ddd747996530a92c60060c2ac68f388bbe85b8d1474e5f74e0ba36dc0468f"),
		new("item", "antiquity_weapon_hellenic_short_war_bow", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "16b99a8c6936445ba2138e09efec8f00b7c0046f43f492a137b0c8fd86425753"),
		new("item", "antiquity_wicker_beehive", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f22725a6d30d4fa7379bd83924a15671f9805a46fb65e21e025859e45142b8f"),
		new("item", "antiquity_wooden_measuring_rod", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c65d35b9466070af3ef557df3cee873304f0e08e80c920e2f3aa54ab95cb976"),
		new("item", "antiquity_wool_wrap_skirt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "162a4abd391cfc241359cf7835518b13ba114d1f3e1bfa3fa5a676baea1627cb"),
		new("item", "earlymodern_africanatlantic_clothing_full_headwrap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "35af61e228ba80c3a336ccf814b1f6aee92f5db90d1f2ad592f96b2e08bed207"),
		new("item", "earlymodern_andean_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a97e812fbd5c0fb13019fb6f1c28794224e4f29d7aaae9c630af22fda81543e6"),
		new("item", "earlymodern_centraleuropean_clothing_short_wool_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e45f28e11f9ffaf974c11ae4b2ee883ba4bc2142856b9bbf8355b015367b761a"),
		new("item", "earlymodern_colonial_clothing_linen_work_apron", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b506868d40e8be2a11d982cf6b61c8e0383aecca0ebeb0f9d7b94bbf30f0b32"),
		new("item", "earlymodern_colonial_military_short_militia_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "725385d31de0ff13c47294304c13e3920d650d36d9c1f37d881a87976abad327"),
		new("item", "earlymodern_container_and_tea_caddy_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "37fe9dc5f21fe0636a65a630bdb31d88bc52fca85bd04d94db3f304cdbd8ac8d"),
		new("item", "earlymodern_container_bra_spice_basket_28", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b267d43e7721c8c63070c03506467b9ab9df9fc70e1e1858855229befa29232"),
		new("item", "earlymodern_container_cna_linen_hamper_28", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "23eb250785085a3c30bf9d5af39c57a0a00ed50d3db40974726e6c94a85807c7"),
		new("item", "earlymodern_container_cna_sample_case_30", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8273c501d2b4e629414554970e811e39a738d8641868eaa7de3307f13fbf23d1"),
		new("item", "earlymodern_container_cna_tea_caddy_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "843858e8375f89033ccf606b3279a4856e8e82da0ad709f5e2a1f592d557f7c8"),
		new("item", "earlymodern_container_dut_linen_hamper_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1755206b26a3c8716cc44a387cf64e99f7706d6852af8ea6306456b1666c722"),
		new("item", "earlymodern_container_eth_travel_coffer_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc518ac0ff1e1241e9fcb70f7797d23794378d2dce07ebc93be90c9051c6d00b"),
		new("item", "earlymodern_container_gmt_punch_bowl_29", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0902c7177ddc443cbed5d26880ed7464a1e7427c206ed7fe026037fdc877449"),
		new("item", "earlymodern_container_gmt_travel_coffer_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "17d7cbc35ca8ebdd4f7e177c6625e70248c4981cc1cd2f9c3a0a30cccecba705"),
		new("item", "earlymodern_container_ina_coffee_pot_13", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb7907526abee9337970482edeb325792367c8721de147c0d29a15f8778271e1"),
		new("item", "earlymodern_container_ita_document_case_31", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "048ea66c69b8702b9401f5ddddc2eb8d56af6dd58982c06acfcada98a46fe65f"),
		new("item", "earlymodern_container_ita_punch_bowl_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6bf9f09a30f2d28474ac6e545c379a67d026656f10072a5bf25126670087cb0"),
		new("item", "earlymodern_container_mag_linen_hamper_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "15694f8d0a9db022d757a1ec7effe1f4703d3282eec7d4298c72a8839ad13282"),
		new("item", "earlymodern_container_mag_purse_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "af03f1a9aece445f50fbcb87737efde34a33f841ec57ef5de94f3878ec4974c6"),
		new("item", "earlymodern_container_mrd_tea_pot_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9959a603c9b6178150adc3be006531685504f2c21fd0cbe58c1f208d761b4e7a"),
		new("item", "earlymodern_container_ott_bottle_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e2b693c2a799bedd1f201c12f57df2ff93cc712e92045de3de44ff373b16c42"),
		new("item", "earlymodern_container_ott_ewer_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c94a71ea58c9ea26a0e895ba604a60c3733b32b57fb8162bce70ff5077258841"),
		new("item", "earlymodern_container_rus_ewer_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b31c5f287e76968c68d34dd47a9040d55dfdb04da41f7effa908d4909d46d168"),
		new("item", "earlymodern_container_rus_tankard_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "70db23a523765bba6efaf758daba54b62d7298162304ddbd74fc0ec152603466"),
		new("item", "earlymodern_container_ryu_document_case_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0393ba1afeb7ea5a9e47b2fa2a5786ac3f44f714a9e6c9f62729be6c38a8f8a9"),
		new("item", "earlymodern_container_sca_tea_pot_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6d6c4afce6d768a978036db4b20feb2b1f86e636c07c0507616ba037def7541"),
		new("item", "earlymodern_container_sea_spice_basket_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b27174abc77f0b18578ff8186306dc72493dbf90fb649270670f4c5ed3d36df1"),
		new("item", "earlymodern_container_spa_lockbox_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea2d1f0de2041b2bd271bf756e0ac688f7926ee14e98346d1ed186e03181e6d8"),
		new("item", "earlymodern_container_swa_tea_caddy_12", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d83cac977c5e924ce9136ea0d77d0ab4af4bbe0838f6163e1b9a5a070500c67e"),
	];
}
