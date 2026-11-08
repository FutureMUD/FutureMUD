// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch046() =>
	[
		new("item", "industrialised_tools_repair_machinery_0366_precision_vise", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "274d8ff351a5098a80e6b72ea6dbf2fbc3cba35637d43af8451e335bfda4fcc4"),
		new("item", "industrialised_tools_repair_machinery_0388_lined_drill", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e15288753c687246ecda8e68bb0c8cd1ad7b26812ee97dd80a72932b559722ac"),
		new("item", "industrialised_transport_support_spares_0011_reinforced_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "8cda71f92b6dd2720079a05440ef91a96bb5927131ce4edbd40610e198e31e0f"),
		new("item", "industrialised_transport_support_spares_0019_reinforced_chain", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "189f52c689ac6cbb5d3f3bd7c4331616f5ebedeea0752a0a843776dcd29a9f8e"),
		new("item", "industrialised_transport_support_spares_0028_compact_jack", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "35ea7b8920464480a79f213900537729f3a4f722e9d5872b5bede23b41ded7fa"),
		new("item", "industrialised_transport_support_spares_0032_heavy_duty_wheel", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed28c6addc6a8b1338972fb8d29673b73ffeea1157706525020099caa7438a3e"),
		new("item", "industrialised_transport_support_spares_0047_folding_pump", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "b885a522aeb73463d2a491bff5a4edff9d6fac429d4b8c07f1b267d3435bbb25"),
		new("item", "industrialised_transport_support_spares_0074_institutional_bearing", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "95dc6f018f1af50c4f436be3f094c2a6ab928130f0eed00c39cb2c12d6ce5a98"),
		new("item", "industrialised_transport_support_spares_0148_stackable_jack", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebb3bd8518b40df89dd972e0fe097966c6312ef5c51cfe5e6a321930f4cfa64e"),
		new("item", "industrialised_transport_support_spares_0161_countertop_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e961ef6ebc34ccde8d6da16b5102aa49e1170ca19bb133a44289b5102792cdbc"),
		new("item", "industrialised_transport_support_spares_0166_countertop_lamp", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "21deda0dc9820bef0494c4cd4dbc49a260c3d66ededb15f7136da23069c48974"),
		new("item", "industrialised_transport_support_spares_0169_countertop_chain", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac540f09e43246b4c8dcfe3fb22fc8602aae5712288f0258a760eb97c73d1b46"),
		new("item", "industrialised_transport_support_spares_0183_travel_axle", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b4e1341fc81e831f666c75e31c9899405b1fb53ce8f3ce179f3696e0b97b523"),
		new("item", "industrialised_weapons_police_military_0009_plain_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "401e3268a3906f5a3f998aa468d9ccde8b54d72d86afb269f4fa3a98667fe656"),
		new("item", "industrialised_weapons_police_military_0054_portable_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "03a268845d4a5107e81d0363b6fd6767447dfd7b0e0260be055d83e61caa3b3e"),
		new("item", "industrialised_weapons_police_military_0059_portable_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "627950ee83f89a41b68a908b6fa2fe0364c349c51974283f58516899da57301e"),
		new("item", "industrialised_weapons_police_military_0062_workshop_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "7196b71d29a7a2e0cf7bfc8750a3e04baf8d8b50676e39b9f49f0cc962c4a0e4"),
		new("item", "industrialised_weapons_police_military_0063_workshop_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dde4005c5940927f2b6eb3f04e6e265a3a3493908d040a0f0c4709f378a2270"),
		new("item", "industrialised_weapons_police_military_0085_weatherproof_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "91be28547299aa3ab357a75d6d5db30bc86caecebadf95854d53e264714b7e45"),
		new("item", "industrialised_weapons_police_military_0094_service_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "aedc90cbec43871482cdb7ef8e092d121bd8f004137210606feed6d9bc667eeb"),
		new("item", "industrialised_weapons_police_military_0128_lined_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "85fd32461c01fde1bc34249ac352d9349c5f6a3e66df11fb6efa544d9476e00e"),
		new("item", "industrialised_weapons_police_military_0131_ventilated_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3f9b11b2ad27224338d93d8162d0ef0e0bfdb1cb6b55e8ec7ea76dac5745b75"),
		new("item", "industrialised_weapons_police_military_0155_field_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "310263e3cee524ca13e683d45ef315a7dce31aab99140617ca76cbaaca67eff9"),
		new("item", "industrialised_weapons_police_military_0164_countertop_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "d347412f2b12bd82a18f460478ae5d0ad3a929351bdf0259255f306bf2509048"),
		new("item", "industrialised_weapons_police_military_0197_utility_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "969cee05e083bf7ca1c3cd8e0521d68918def2f2d134d4fc6213bc1477254fb9"),
		new("item", "industrialised_weapons_police_military_0232_heavy_duty_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "02b42b305be22f41bce619efc341fc42c8b865bfa3251cf5d6767b73e56fae2b"),
		new("item", "industrialised_weapons_police_military_0248_folding_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d529facdc9b379bceb0453ba46977d1266d8bf78beffa2ef960f73258b13724"),
		new("item", "industrialised_weapons_police_military_0251_portable_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ae2f213be71b24b7cdcd9be63dda4deaa2316dcde55652a9f79d5d24ad99f05"),
		new("item", "industrialised_weapons_police_military_0268_workshop_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "39b0bf2e5912a817e1fd5ead1a6e9479042091dc19c1ef33f46e561985c9303c"),
		new("item", "jewellery_agate_bead_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "edcaab103fbd3c9855ba489047097938e87dfc102d18cc938f0205f884ef898e"),
		new("item", "jewellery_bronze_crossbar_fibula", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "74be3f6009308e9bbaaa166cc0adcb49f4a0f7f78d331722f7dd5be13fc288fa"),
		new("item", "jewellery_bronze_disc_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6726e40446473dcf644638a38283437b127fa60b4414ab6a2b01cc71b83f9122"),
		new("item", "jewellery_bronze_dress_fibula", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9c9f7a9360fc827087e1d27b030f0f2b272e78d4da071940cf1be863da29e47"),
		new("item", "jewellery_bronze_wire_drop_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "876e1620613dc8b2fa5e446a86f9cf13818e983bf3dd3987f76d73f0ce18c725"),
		new("item", "jewellery_gold_carnelian_bead_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b12fa69654cb3de734431bd9654d2f717daf9f2b10741eb7d140da6922910531"),
		new("item", "jewellery_gold_crescent_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb4a6d2a6754f04b91cad13ada46f88325b2375ea759ac6897325ad3ef24b77c"),
		new("item", "jewellery_gold_palmette_diadem", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9082febdc93fd4ce25727462c6ed17f7657e5ce2a9acefb2a28c123dac336ece"),
		new("item", "jewellery_gold_rosette_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4fc45b849a450284ef1d23c560a9167d7212c062223c840e8e80c32b37591c45"),
		new("item", "jewellery_gold_rosette_sea_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "18c680a6d108ec552beb075f965ec5ae0d94dba7d648bd372eb79720bc005487"),
		new("item", "jewellery_red_inlaid_silver_torc", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b74c9434a9cfbde8b976c6e69165c02624d276c71035ec4331d49f15dde88c4d"),
	];
}
