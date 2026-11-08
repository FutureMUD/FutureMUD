// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch047() =>
	[
		new("item", "industrialised_tools_repair_machinery_0363_precision_wrench", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "1f1b344998530c994bb92c286a09d544b581bcf91446da9a8eb49488c5e13875"),
		new("item", "industrialised_transport_support_spares_0031_heavy_duty_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "7bccfeb27ddedcf3eb106f9fe10494ea27593239e3d96629a1617563a66b464a"),
		new("item", "industrialised_transport_support_spares_0046_folding_lamp", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "81c491b42a2a7c573da0b3a9a333bab1dc4d6f02231c76fb48d3062f5b4148fc"),
		new("item", "industrialised_transport_support_spares_0066_workshop_lamp", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "3759d17b2ec7a525ed39e390be2ade648cf33b8cb9a9eb39533e532c3a886ab9"),
		new("item", "industrialised_transport_support_spares_0088_weatherproof_jack", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0ca101fad68106c565a783dea5a3d7b5749fe05a3629e9497b56f91241d7db9"),
		new("item", "industrialised_transport_support_spares_0153_field_axle", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d6d584ad90d6a694535f19df39b40def33ca7f9776e9205cf83dccdbe01f0c7"),
		new("item", "industrialised_transport_support_spares_0162_countertop_wheel", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "b052bd14264d8b06676f3a8bd3bafa4542bea5f095462632d138ec547bdbaf15"),
		new("item", "industrialised_transport_support_spares_0189_travel_chain", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "32bbd88fdb8cee20b932582629cd4e1b251fa20e6a9f6e1d5152d7995f3cff12"),
		new("item", "industrialised_transport_support_spares_0192_utility_wheel", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a0d7bf7ec589cb069310f24cceffc8378623d9b6bed2d5a81030a5a05217f95"),
		new("item", "industrialised_weapons_police_military_0025_compact_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "33ca9358aad0b73ed8a53b71fb7f999bb0a06fe094eb80def64b4a15d6ac898a"),
		new("item", "industrialised_weapons_police_military_0026_compact_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "eeed51075580f9e07f7e999929f86dbd3e1f87cc3c27ffa21e1a49eba197e1cc"),
		new("item", "industrialised_weapons_police_military_0060_portable_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "a28ae234070b92337b5b774f9c34ebdbc7dfbce213fccdfc3b554a506586a1e4"),
		new("item", "industrialised_weapons_police_military_0072_institutional_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "d522efaa2e8dd783a0b030720b9195b6e6e840f195703b6e744a521c878b48ba"),
		new("item", "industrialised_weapons_police_military_0078_institutional_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "50e14792f51a651701e16a64955d4b6fd9508d7ef08d30bd6f920b3dd5f135e0"),
		new("item", "industrialised_weapons_police_military_0083_weatherproof_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "a544e23dc64b25e2b7c3f0e5fc029511f15790f88c8a518780ba16bcee06f9cb"),
		new("item", "industrialised_weapons_police_military_0146_stackable_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3639143ae7db96f4f19249014fdea712c69f14a5bba4d6377827a378d5ef4e2"),
		new("item", "industrialised_weapons_police_military_0171_wall_mounted_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "433769c662710ee93fa874dd628f8a338d808c56dff9b3a04f1c602fb11719b3"),
		new("item", "industrialised_weapons_police_military_0213_reinforced_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e01784b936e1ec5a512140485e8a765b4efd5f91f10b2e09131c6cfd6a6fd8c7"),
		new("item", "industrialised_weapons_police_military_0218_reinforced_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a727178eebb427f0df20da5355626c103d6cd896fd4880e71f69c7c2a833535"),
		new("item", "industrialised_weapons_police_military_0229_compact_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "6dc506009a8221bfc736634697b69cd83d036534ed19a7ad1f8ec250432cf2c6"),
		new("item", "industrialised_weapons_police_military_0246_folding_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "364556ad0f475832dd1196ce972a51fbc6e398d322b1e6c8fb815a29960ecaab"),
		new("item", "industrialised_weapons_police_military_0253_portable_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "8816dfd0118ea2e17030591bd2a8981cee5ecf6b1d323ad3bf6c327af1c506db"),
		new("item", "industrialised_weapons_police_military_0266_workshop_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd01eea0a69f5f8e9a06503c962169d7908f36438bf397a98b8c1f7cc9e5b0a8"),
		new("item", "industrialised_weapons_police_military_0269_workshop_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "08a23fcb17b1380b11e683415715eeb952ec234f7ba1e11494d751dd748a51c5"),
		new("item", "jewellery_blue_glass_ram_amulet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "974f0559b97203caee37c8708b8df3384351eef79a1a4020c9dde80849fae708"),
		new("item", "jewellery_bronze_pair_bracelets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "752c9eb391e3e7b32d524ff51817e4cac0583aca575d83962607e54dd4d73df9"),
		new("item", "jewellery_bronze_seal_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "19df8a936cdac279b50f8c589519222602e03919b76a847bec5e150d5315f51b"),
		new("item", "jewellery_bronze_winged_fibula", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2650fe31dbcd077d09320d6fe6bbcbe2a2921b040fffd3d08c1f257a0c50ca6e"),
		new("item", "jewellery_copper_wire_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7333deb92618db3b9aa0ee12ae9fe7e93077d0953ae9d22ecb2b17e4dd2d8b78"),
		new("item", "jewellery_gold_carnelian_bracelet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a002b1f6ae6a59a5efb080e895703c6547fac8363ca7b7a7325156b751d40551"),
		new("item", "jewellery_gold_spiral_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "38f455bb025140c9585064d08bb0479a5461b9d53043fe54eda5f613acc65c55"),
		new("item", "jewellery_gold_stag_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "48498a04e029ae99e68de7db44ad38ff21c5ed4ef90f5c4446154096a6005115"),
		new("item", "jewellery_gold_usekh_collar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "df5ecff982f142d4f55e6c21b326f33627c24a07ac4e04a876c0a493bbb61297"),
		new("item", "jewellery_iron_wire_bracelet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "90ec5b88d288ae691fc7a16541b7ce8a9f62db65bc34a7a9f90126c871403ceb"),
		new("item", "jewellery_shell_copper_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "92b0f79b2dc2cef01e41c2e67e2f264528c039d8dd11331dd1b39dccefd46cf4"),
		new("item", "medieval_amber_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfa7a165f9020bf2c48acd06dcd442c030404fe1e3cf6bfccd5b8b572413b28f"),
		new("item", "medieval_canvas_work_tabard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a983a2666dae914def7cbf23280d9ba801993f83154bdd4a2adc2d445ba3e560"),
		new("item", "medieval_capetian_cotte", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa5c1008e839e2fa334d14ddd0d6f0935005c2dbfd93d177bcc4ac78322bc4b3"),
		new("item", "medieval_child_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a44ca0b073345e79b4a322b313946679872edb100837687bcf68719a639442fa"),
		new("item", "medieval_church_baptismal_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3b0790b57e95f467f204577fe82540c7c0741ddddf728583e51280eda1ee0e9"),
	];
}
