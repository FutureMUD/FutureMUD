// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch061() =>
	[
		new("item", "renaissance_jewellery_common_badge_167", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3527767bcefd5558e1da6a0c343924498d95371beb91ec8d229148714872b2c5"),
		new("item", "renaissance_jewellery_common_belt_ornament_106", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16d259dca6e75ae9df10162ce5ef395da0aacb496b830fec222a8f7a87b0b639"),
		new("item", "renaissance_jewellery_common_bracelet_36", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "192b6d700e6cac1637ddb4ccfab01700842cd71659a7ceac8ccbf18533d3df7d"),
		new("item", "renaissance_jewellery_common_devotional_medal_172", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "08aab1a21d20289c1997f402ba14e0b5d5711848ff576a88c0ab7868f12ca325"),
		new("item", "renaissance_jewellery_common_devotional_medal_22", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba8c2d730b6b900e6ad1ca75225f0788b6ee5bdbce1885f9767e3a2ae052068d"),
		new("item", "renaissance_jewellery_common_diadem_14", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6816b1f2a21d75e6f69d72826b0134f005d8ec4c6cee63c2b4962cb8350c03d9"),
		new("item", "renaissance_jewellery_common_earrings_129", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81870cfd46253be361f69692a78600346df392043326d52003a6b605524a0a2f"),
		new("item", "renaissance_jewellery_common_incense_holder_148", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cacc4aea5709a5e2701833264b510e2c7d26bfe41e903b4907814a6c3407cc30"),
		new("item", "renaissance_jewellery_common_portable_panel_89", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d97ed8854f5e537dc699a2461d1840f89f0ba3eec636fd25d1733bab621a43b4"),
		new("item", "renaissance_jewellery_common_prayer_beads_51", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "80a53b0fad03558115c2b8cef4ce9ed229b8d103d42eed198b65d5312b5fe653"),
		new("item", "renaissance_jewellery_common_prayer_cord_144", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c1d4f9011d48bd06d67b65861ce491dd929051c8e475f0753505d88088ffc59"),
		new("item", "renaissance_jewellery_common_scripture_box_176", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb035a86f1ca3a87cf209a0ca148eadfdd2a8c0866e8de18b7131eb49b206c05"),
		new("item", "renaissance_jewellery_common_scripture_box_26", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bf89cc1ec09b8b07a372329fb3b669aa809dd0b460b4ddbdb9ff7600867316c"),
		new("item", "renaissance_jewellery_common_torc_49", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "718a84d7d577b96a78e9acbbab88e0e259bd1d072aa13a6e7d99589172c7abb8"),
		new("item", "renaissance_jewellery_common_wreath_110", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b05306952c971a50b0c55f4bacd2aaa42330e4289e29314f36aeb63e04668896"),
		new("item", "renaissance_jewellery_eal_devotional_medal_292", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f22412597653ded525e58a5d8d95322e8a1c584ed8ad032a3c88bc1ed63fa2b0"),
		new("item", "renaissance_jewellery_eal_diadem_284", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "27163747685105f0ca6b20a853f8f70ce7ea4bb4d0ae78bdfc8f07528c06d488"),
		new("item", "renaissance_jewellery_eal_temple_ring_288", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e90d8f1a7f25b45ef66b922c39591cf466b10fd1239c4d77deb1cc3703c7b411"),
		new("item", "renaissance_jewellery_eon_earrings_159", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4493f66e3a808e02961c4f7ad2a13f0cec20087abd67ea9a87d1eefb7e2bac7e"),
		new("item", "renaissance_jewellery_eon_prayer_beads_171", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "96a21bd7d890e16f8208910aa5cc4180d892306c5ef6f87ac87c0c81caea2129"),
		new("item", "renaissance_jewellery_iba_anklet_38", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e0dacef67a5606203b87c21c6140cb20c31799e1b435397f6f91ecba657695a"),
		new("item", "renaissance_jewellery_iba_diadem_44", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd053a9ab5012eabfc4b41d563d735f48ef5d549a4a56497276a6946f6a4c73d"),
		new("item", "renaissance_jewellery_iba_portable_panel_59", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc99f03861cb9c1f20b74ec968f9883e70414430f71934b52f473efe0da7df46"),
		new("item", "renaissance_jewellery_iba_scripture_box_56", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b143c1005af36c1d2294fc777221836470ed0ea3cbd7a26ae5350ecf6888ac24"),
		new("item", "renaissance_jewellery_iba_wreath_50", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9e762893d46364595e0938b99b2c04f3fb48e917527adb20b3798f2f74c2140"),
		new("item", "renaissance_jewellery_ind_devotional_badge_563", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "00310be8873f63210067c6d2e5acaa65d05aee4f5674831e5c69bf9b027fe4d3"),
		new("item", "renaissance_jewellery_ind_prayer_cord_564", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd8a394c771761607f9241b7466b3acb45455e8e0420788a83d2a94fdb4ebd55"),
		new("item", "renaissance_jewellery_ind_processional_ornament_570", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67013108559ee5e166a4b7ead355ad36518d303d11daf06166104fc80cf97736"),
		new("item", "renaissance_jewellery_ind_torc_559", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "03cb34f0e70bfc0c147b53eeaffe61d6542ec18502168d2063da4237356f6311"),
		new("item", "renaissance_jewellery_jpn_anklet_308", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "931e8a97006bd65f9a61fac5f009bac1583d83c8c11ffefda593cb1d997ca406"),
		new("item", "renaissance_jewellery_jpn_bead_necklace_304", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5ff4efbc545d8c50ea2e08afb2e9a083936287d42cac821d5fb7f14c5dc48d6"),
		new("item", "renaissance_jewellery_jpn_chain_necklace_305", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ba09a4bb319a3f6befefa944935a60f6f3a97f75e89756119c631bf241e540b"),
		new("item", "renaissance_jewellery_mea_anklet_338", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7b04e61a88476e21baf2a647308b7b36553f873ddbf095a55f7a46a2d3b4e58"),
		new("item", "renaissance_jewellery_mea_belt_ornament_346", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a2110874028a56ce6e9a61d3223afec0bec61724616adfb9c6016d73df99d73"),
		new("item", "renaissance_jewellery_mea_pin_341", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "924bdc56909f7ab2425dcc8215a543129bbc05cc4c918f7df1f54cbc818acf8c"),
		new("item", "renaissance_jewellery_msea_diadem_404", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d81d7a7c8a2061d46860cfa131d837f7a9c7d7ba47a1d7ebfd67526b6411260b"),
		new("item", "renaissance_jewellery_msea_earrings_399", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb36edfe0f2af606a7e1d029f91792ccdc870aaabc6555e016675120f8dfdb4a"),
		new("item", "renaissance_jewellery_msea_portable_panel_419", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b101d6e9e110c5fef8ebe011e32df2ec5860f345a937f01d24e6d11c3b53b18d"),
		new("item", "renaissance_jewellery_pip_incense_holder_238", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f75859d8be7c92f4ccc8d1bd928bc347c9dc6e3090649db3a464825ae99f564"),
		new("item", "renaissance_jewellery_sas_anklet_248", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "34d8a0856eac58ee87c2840a38b2602c61ab7feb8d8ebdfba186500361a4c062"),
	];
}
