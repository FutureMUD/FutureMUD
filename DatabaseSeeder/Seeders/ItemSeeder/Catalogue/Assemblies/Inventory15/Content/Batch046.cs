// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory15Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch046() =>
	[
		new("item", "industrialised_transport_support_spares_0121_lined_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe168c015bc2d8a5db91840744e7b4ce27cd13ca75df06292522191fe60b327a"),
		new("item", "industrialised_transport_support_spares_0140_ventilated_tyre", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "59b6d41cf3849bf956d8c1bcb7cb3a63684bfff397d0a90a13545c8fdfeb3169"),
		new("item", "industrialised_transport_support_spares_0144_stackable_bearing", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "75389ecf8d60439d275ed34b6d9287660f327add10504e8758b77811caf7e4c5"),
		new("item", "industrialised_transport_support_spares_0146_stackable_lamp", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "b38b6f4f4b8e9a6fc7ee7a12676e20f7a2ed4d404f7ded56dad80138d092abc7"),
		new("item", "industrialised_transport_support_spares_0149_stackable_chain", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5d144dec2f13917fcd172e344d7803985b733a4fee6bb6a58177b4ed1224848"),
		new("item", "industrialised_transport_support_spares_0163_countertop_axle", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "35bd7c9697fc5531eb2875d715f12a2d04d320cbd5fe07c5f9979b9b64a055b2"),
		new("item", "industrialised_transport_support_spares_0170_countertop_tyre", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "cd5ed10710ca3bd940e9edeeb245eea9572efa0633b667c943528740dbb5af9c"),
		new("item", "industrialised_transport_support_spares_0171_wall_mounted_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "83ff17e4cd9596c23cd93329da1bf831276e19672a146337f714634ed3614822"),
		new("item", "industrialised_transport_support_spares_0186_travel_lamp", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "9437ac8df9b4ea66e9d58364b7c0c6f4334233e583de2e06d32176ee28cd2b18"),
		new("item", "industrialised_weapons_police_military_0010_plain_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "25b277fae85e05f5c5eab4f735d47f8d121587e2fe71fbb8cfb716948fdf1cfd"),
		new("item", "industrialised_weapons_police_military_0024_compact_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "64160ac793a85528834adb0acd7ae9168d1fff9a2f58911e504d09955030891f"),
		new("item", "industrialised_weapons_police_military_0034_heavy_duty_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "d177ba2d04f5771d20eff81053af882282dc69c6001eaf0688ad2b0f07151266"),
		new("item", "industrialised_weapons_police_military_0046_folding_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "adf5ba4728acb4add339ce00a3568ff83529ec3e9e8c0a30ff554952e80a7cb9"),
		new("item", "industrialised_weapons_police_military_0066_workshop_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cc27c99611219f7087d473581e091bce04f8ca70fa441145f9d1ab4c52a4d4e"),
		new("item", "industrialised_weapons_police_military_0092_service_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "8408617252e910c88886cf54a755e34cc28d948367cc7254b9763e3ec09a14e0"),
		new("item", "industrialised_weapons_police_military_0104_precision_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "c72bfab0307622893597615bb11eadfe638433beb8c6e9031aa0de2b8b143918"),
		new("item", "industrialised_weapons_police_military_0118_economy_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "01f3128f62e5e11f021214cce61729d1448da29f87ca1e6ee0cb7a39ab20ba03"),
		new("item", "industrialised_weapons_police_military_0120_economy_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "499606b2968b3475714b664b094589116c1c3e4f88ed38311a6df14d5226afbd"),
		new("item", "industrialised_weapons_police_military_0127_lined_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c80e7e33cfbc386877c1bfaf4c4d6644272c43746c52a2812f9f6dc7343970e"),
		new("item", "industrialised_weapons_police_military_0152_field_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "02304113f57fc81a5faf95daed8bc6b6589808716af69b465b64961ed35e0448"),
		new("item", "industrialised_weapons_police_military_0162_countertop_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "daae8fbb7172d9a92f3c7fb1093ca8e05ffa0f59ff8357eb0dc86d41ed7be493"),
		new("item", "industrialised_weapons_police_military_0170_countertop_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "651076bc5e93db1549553f476fb67df3bf273e425d8f66b365c77cca1a23b0b6"),
		new("item", "industrialised_weapons_police_military_0179_wall_mounted_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "275d63513bd7baf62ea78964c9b208c2402300da215c3c704bbdad2436e35e0d"),
		new("item", "industrialised_weapons_police_military_0184_travel_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "98d5a57535630868c86cb4e3e0a7798a32e590b7447581272a0953d9340a663e"),
		new("item", "industrialised_weapons_police_military_0236_heavy_duty_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "12647240df7db2b42a27a0f052371acd88de94e5a072fa8309d3cf359cd92636"),
		new("item", "jewellery_blue_glazed_ceramic_headband", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e5638eb42de9b0c577611805d264abba7c914b371fe5c3db0eef67f922d4cf4"),
		new("item", "jewellery_bronze_spiral_bracelet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9fa04930238d92e1b5f5b6a8b665b8452de7b153419f46ce956672fe07a714c1"),
		new("item", "jewellery_carved_ivory_bangle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "01b47941c60888ad51c0ed78ac7a8b31fbe9920ff80811d14c36eba48c34d6f3"),
		new("item", "jewellery_copper_ankle_rings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d6deae1a8daaed4d9361fd0b5259b66266ba48a7874474e8ca643cf1842a2f7"),
		new("item", "jewellery_copper_ram_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "934ad0962bd5d72833a091fbb9544adab7ba9a8f979b6a84125a2be5c68f784e"),
		new("item", "jewellery_gold_carnelian_seal_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ee535f3cdd583b8c96aa397f7126ef69a9770af24d07995fe6e7d90c17b161b"),
		new("item", "jewellery_gold_cobra_frontlet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2817b38408ab3336ea75693ae7f65a2c9157aac246fc3695b681393044e6110"),
		new("item", "jewellery_gold_griffin_armlets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "89e2e7728d39a4fe7afcb7371d97c939f454a2d1ab983e1e9254b1cf1250888a"),
		new("item", "jewellery_gold_ivory_belt_plaque", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "453a4d6a542bc29aee3b4f61fef821186b752bcdcb269cfcfcc7d6631ab70ea5"),
		new("item", "jewellery_gold_jasper_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9fb595ea4f5318fd118e4e01e1cb6d792c60f09824cafea60e3f0f81b840fe23"),
		new("item", "jewellery_gold_ram_head_pectoral", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f5e07f7358f2804a5371c7cadba3af6707960d864c055394a9fa8febd5637ba"),
		new("item", "jewellery_shell_and_bone_browband", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3b8623bdc8e1737637fdef991646577c18353d02f084f669bff7c76044c25d8"),
		new("item", "jewellery_silver_gilt_brow_band", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "01e9eba10d69808c2ffb935827b28ee763fb634069af94d6b73bf1258269c0a7"),
		new("item", "medieval_ankle_leather_boots", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2165aa490068d0a2328e8537df5375d98851bc565e417d1068976ac0440cd545"),
		new("item", "medieval_buddhist_meditation_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b2b01c76a872ebfe552a4ba68612a0585b7b43e1bc114abaca8eababafc911e"),
	];
}
