// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch036() =>
	[
		new("item", "earlymodern_qing_clothing_short_riding_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e57f54ac616d36152f6bb1c23602024092974896210fd88e7123199319ad7fa"),
		new("item", "earlymodern_religious_ethiopian_priest_turban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cf3ef0f0a976acaadfb8a14d71c0cf7fce65ebf6daf9d727fd9c913628d365d"),
		new("item", "earlymodern_religious_iberian_confraternity_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "59495e7470b5c220790fe53d80f54dd86c4cb8c731abb9d58f7af66adf6d4e5f"),
		new("item", "earlymodern_religious_reformed_clerical_ruff", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0fc8562683763c874b0d74c9408984f5c8a805dbd5905098129d4f7c731dbf87"),
		new("item", "earlymodern_russian_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b38e796100ff2997ae3d70b200ca86932e665f5a3c36493806a26e81fd9f3011"),
		new("item", "earlymodern_russian_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "79cda0f1633612ee1cc9c7ad99b7c0a1b044ae842cc18aa46b1ea82991e9f762"),
		new("item", "earlymodern_russian_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d82090ba35b0abc8ba21b757e9122c00399fc0be01cf2d3dcd384dad6d59e44e"),
		new("item", "earlymodern_ryukyu_clothing_tied_headcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be54d18f83bcb32761c4a89291b3f0e8311d26c860456ae3a42624e0319c9990"),
		new("item", "earlymodern_ryukyu_clothing_woven_fibre_sandals", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "27cdfe2c2514e8552e709b401382b5f9b09f8208e866b3791f1be1da3dbc9b6d"),
		new("item", "earlymodern_ryukyu_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0377afab5529d39769531856c4af27f5aea4b3e44a0f14659debad6e0a03a2cb"),
		new("item", "earlymodern_sahel_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4da8b7b585589d65786239096f19c3a829fd592887dc1e19b932138442d5049"),
		new("item", "earlymodern_science_insect_case", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed1b268a033c8db7047c41e3e8dbc3599576776600d1500b8498c404d2c79b32"),
		new("item", "earlymodern_seasia_clothing_light_shoulder_cloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e10727b0ad85f21b1e1bb9b97556b02af45042e55f7b7ebecff3d28eb48486e3"),
		new("item", "earlymodern_southindian_military_folded_command_turban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b176276c1259115f6088d2dfc4b499cb7e885bc06e9c6bbfab948b3b71a13fa"),
		new("item", "earlymodern_southindian_military_short_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2314b684a4fb1b05d3cb3438847e4766ad90eb967965367119b679a0bf4f9cce"),
		new("item", "earlymodern_southindian_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4dc1f2d088e022c7387d209daf60574d5abd741ca0cc6bfb550d59f427c4b116"),
		new("item", "earlymodern_westafrican_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf9599bc22ff3b9bb86eb58b7815a774aaf305f429bc856040dec945f4bd052c"),
		new("item", "earlymodern_westcentralafrica_military_long_tradecloth_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "98625132f7029cd8abf14e21ff02ea332f866afb02fd86e12180c84c4bd08b74"),
		new("item", "earlymodern_western_clothing_linen_neckcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "04f059f8beaf0ac8dca3b748cffdc689b35b35897de3b4557e98a4a50fbdda1a"),
		new("item", "earlymodern_western_naval_officer_dress_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1b415f44bef4a01871971572a0ff812f0555d238131fe2550db8b96ed7213ed"),
		new("item", "earlymodern_writing_atlas", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "59c7d02b3ce740fa837f25eab1d5e289548cd3b0475e83b12a59d6717aacb5a4"),
		new("item", "earlymodern_writing_ship_log", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddccdd4667aaf7cd7f2460f7c817389388f2b4daa502ee0f09cf6a5677a92ffa"),
		new("item", "historic_drop_spindle", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be143a13608c0bf4c304de1e463f301c07675a1adfe74a5a9980b2c1f54c4e66"),
		new("item", "historic_lit_updraft_kiln", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:historic_updraft_kiln"], ItemSeederOwnershipPolicy.RequiredRelationship, "e9e354aae76656e2ca73582e3315ad6670763a44871eb546707c3b3357eb3837"),
		new("item", "industrial_agriculture_forestry_fishing_0010_field_yoke", "industrial", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0001_field_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "2b3295182ae8ff916fbcb09ce33e3ad64af9f433c877d8feb3ec496534550d17"),
		new("item", "industrial_automation_access_control_0005_precision_alarm", "industrial", ["industrial"], ["item:industrial_automation_access_control_0001_precision_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "8e36eca0793ee183bfbe400d3519a11c197807ab43f024752aac2defdb8e95eb"),
		new("item", "industrial_clothing_footwear_uniforms_0010_plain_scarf", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "ad16ede16c7d6071a4e3e0a24da1959dfa51b9d7d4401ddf0bb93852b8d6c01f"),
		new("item", "industrial_clothing_footwear_uniforms_0016_reinforced_rag", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3be8f3d15a8fc345c493e14590cf5626e946a2ddff99b22c879164d709e6cd17"),
		new("item", "industrial_clothing_footwear_uniforms_0018_reinforced_shirt", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "724b633eae2cec95b6cb4dbb96be26678885056f5540895405ba61f6d4359731"),
		new("item", "industrial_clothing_footwear_uniforms_0019_reinforced_trousers", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f80538295a96bd44a6f2ead0b12df6cda614175e96626abb92ba0319fc12bebd"),
		new("item", "industrial_clothing_footwear_uniforms_0020_reinforced_boots", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "e93668019260d3d1f72a176eb9ae0f1ab4aaa7888734f0faa976a36e1da734ba"),
		new("item", "industrial_clothing_footwear_uniforms_0049_heavy_duty_trousers", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a22e7027fe9458bb0464744d3659f4f00a1ed3931399ac57830206b792661e3b"),
		new("item", "industrial_electrical_power_lighting_0024_portable_switch", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a9066bda747abcf4eead67374ea2ba2fcde548ee4e7f5a1348c5ec19e430090"),
		new("item", "industrial_electrical_power_lighting_0028_portable_junction", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "008fd7e4490aea9eff3ebb43ec81122896dcdb70ebe9fd84cfa4e55be52f1f9f"),
		new("item", "industrial_emergency_rescue_civic_0001_lined_helmet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e55d0d1c562577793f82a4c3e1cee2fac35f531a6bd2f652014d1df3db357cf"),
		new("item", "industrial_emergency_rescue_civic_0008_ventilated_ladder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e28e6ecb31dcda76b222248d96770b90c0dcceb692ece5a27d5da85cc7439c1"),
		new("item", "industrial_food_drink_packaged_0008_stackable_sweet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "27bbd0ac5355bf3626b8bcdf5344616589eea4e1aba15fb534273e1b4d3c8a26"),
		new("item", "industrial_food_drink_packaged_0012_field_biscuit", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d12d6771884dce70646fabbbdc8ff937d25a063f5b9eb84af7b75d4ef9241df"),
		new("item", "industrial_food_drink_packaged_0032_countertop_sweet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "f09b29ebde9d67716197792d4979ded5cfd5ea2d3110f27a5d4cdf1b4fc39d90"),
		new("item", "industrial_household_furniture_storage_0009_ventilated_stool", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "966cd9f1451e243a6877f0c4bab9d7bcbb947193ed1c62253b5dd9b1cd892ea8"),
	];
}
