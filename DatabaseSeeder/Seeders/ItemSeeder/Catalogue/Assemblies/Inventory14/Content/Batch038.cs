// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory14Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch038() =>
	[
		new("item", "earlymodern_swahili_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "382c8ce176cff2f559eaae4c361444aeffd74217ee6e8555caa2aa472d1400ee"),
		new("item", "earlymodern_western_military_cocked_uniform_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7a68b65fe381001f2b58eedd22f10d38d61f2a16ac8c8d3f6ced43f81cfbc30"),
		new("item", "earlymodern_western_naval_officer_undress_frock", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87dc1bd7f058f000cfb5dbce2cf71adffec1275ab41f97a0f0efc344e9a54205"),
		new("item", "earlymodern_writing_passport", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f7d59c8a7ef7b6c9f371a671f9df7c7ecbe93a6291254bfc163278cb0a484ea"),
		new("item", "historic_bellows", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3423420ebbf7c664f55b2f5914ae07c1e784de219e3613b45347bfcfc12ffce"),
		new("item", "historic_workshop_hammer", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "28b8fbaaca9323bbe2e1feebf43ed1fc4336dfe1bf809d23e99a76d4eb1d52b1"),
		new("item", "industrial_agriculture_forestry_fishing_0001_field_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "bad2d7375da609de0a2218e8ae4b24265f40e6d6db23cab4f9a7452268b8edd1"),
		new("item", "industrial_agriculture_forestry_fishing_0007_field_net", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e27532965b7ade07093d29a233e896d529e92a2c1eca3fb23c961556bbb2ac5"),
		new("item", "industrial_agriculture_forestry_fishing_0013_countertop_rake", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ec8e0875cc666552c7995a5eed2954825ee7450bb81ceee2f1d8d91296b362f"),
		new("item", "industrial_clothing_footwear_uniforms_0025_reinforced_scarf", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "77604c7e601d82e3393fa225d5a8900097e2acfcdbb1912d90aaad1e98429349"),
		new("item", "industrial_clothing_footwear_uniforms_0034_compact_trousers", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "87515e14fa65ae2b394686c99c2582241b70a5e06f098c00c8743abd575bed01"),
		new("item", "industrial_clothing_footwear_uniforms_0054_heavy_duty_gloves", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "71cbc78bc55fc81fe232be53c687653143445379ee7f5e728f07e3f2fffd7c38"),
		new("item", "industrial_clothing_footwear_uniforms_0065_folding_boots", "industrial", ["industrial"], ["item:industrial_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.RequiredRelationship, "f774989111568a212eebad0cce9c94f826986669517884b770f839c48a5f06d6"),
		new("item", "industrial_construction_utilities_0020_compact_conduit", "industrial", ["industrial"], ["item:industrial_construction_utilities_0001_reinforced_rubble"], ItemSeederOwnershipPolicy.RequiredRelationship, "fef2e5d2f6062329c2c34731a506d66007c733d00e9a1f9f137c23070019fe82"),
		new("item", "industrial_construction_utilities_0022_heavy_duty_brick", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b60a0088081a2ad705302ca79ea014c5512bccb37790ba8a7393c87e1233a44b"),
		new("item", "industrial_electrical_power_lighting_0005_heavy_duty_fuse", "industrial", ["industrial"], ["item:industrial_electrical_power_lighting_0001_heavy_duty_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "4ae2faec43601a0e026673f9de3289d22a7448068ee2ff2737745edfa91b9d6a"),
		new("item", "industrial_food_drink_packaged_0006_stackable_sausage", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5046430dbefb9f47cd4f235be70df8185d0aa46dd92b55b730f44ebbdaf6bd04"),
		new("item", "industrial_food_drink_packaged_0025_countertop_ration", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "8b93046945c3e6d627deea27a509a81a4f50e2a5a2b4c80021b97024fa84560f"),
		new("item", "industrial_kitchen_appliances_0016_weatherproof_grater", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "84ed70afe16e84e8010b90ffcf8f7789fb1b9df35c8053681aecaab878e726ac"),
		new("item", "industrial_medical_mobility_0015_precision_stretcher", "industrial", ["industrial"], ["item:industrial_medical_mobility_0001_service_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "ea81f65da06dcf081183656d115046ea077bc417ba38cba176414af162fa0cff"),
		new("item", "industrial_office_school_mail_payment_0012_utility_envelope", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "72ca4c0ae8aa3262a84a22031b2dd514671fdc0d363dd39542edb4cc0fd686b5"),
		new("item", "industrial_office_school_mail_payment_0017_utility_pen", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "591759f5e76527b3f82df77b0aad982f010c1906db5bf8fc96be967c802ec23c"),
		new("item", "industrial_ppe_ballistic_0023_wall_mounted_gloves", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd1073ecc73774747f8edd3b1ed932680dbf2802be17644ee28c58bf2a4c1a91"),
		new("item", "industrial_ppe_ballistic_0024_wall_mounted_boots", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad5c4dba2e993ce4c924d565158448a68e09619010bf0117fc4e3c55a2b48fd6"),
		new("item", "industrial_ppe_ballistic_0033_travel_gloves", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "750a05ad83aa15d32909dc1f2d276a58543a7daa981e125c661e50636d832dcf"),
		new("item", "industrial_raw_materials_chemicals_waste_0007_stackable_pellet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "40ec30a9dcebd5893a25620ccb6a3cf59e531be3a5b888fe0440c1d8e735a605"),
		new("item", "industrial_raw_materials_chemicals_waste_0013_field_sheet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "bda57a0b486c10f886f55745f2a2e3910e9d2dfad58205d248a68ad711609f09"),
		new("item", "industrial_science_education_0016_field_model", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a7b3f7be0ad580895d38f2be056a3a57cd3f8c96fad42159be06d800e8b1b05"),
		new("item", "industrial_science_education_0020_field_case", "industrial", ["industrial"], ["item:industrial_science_education_0001_stackable_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "c85ec14ba5d4a8c1a758afb4e726960a6e7b53a98f7278dd652b10998506f9bc"),
		new("item", "industrial_tools_repair_machinery_0002_lined_saw", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "75d5a82e8b06af2dfdc94212fb50a3a1068d844894524dd582b5a3155842273d"),
		new("item", "industrial_tools_repair_machinery_0006_lined_grinder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "73c5aada08ae10e567cbd672b0fbf8faed31d1b4636a8e40fd3f32a23dc39e61"),
		new("item", "industrial_tools_repair_machinery_0024_stackable_vise", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c1340399c6481b57cae6eedef5ed95c160be9285da8fa834b9dada1dc232a24"),
		new("item", "industrial_tools_repair_machinery_0050_countertop_saw", "industrial", ["industrial"], ["item:industrial_tools_repair_machinery_0001_lined_plane"], ItemSeederOwnershipPolicy.RequiredRelationship, "49113c1aac5e9a37ee39e613a760a26648818aa7b562ade951d7822956fa81d4"),
		new("item", "industrial_tools_repair_machinery_0054_countertop_grinder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "860fd9bfc34f6baca63e51fb97f27dd74f9363f7af991188054969499ecf4d82"),
		new("item", "industrial_transport_support_spares_0005_reinforced_spring", "industrial", ["industrial"], ["item:industrial_transport_support_spares_0001_reinforced_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "f271b3e0ae629a505dc514b783dca0b02239a27a3ab956e6683e208ed9fb3fef"),
		new("item", "industrial_transport_support_spares_0018_compact_jack", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3890c916e7b0b720473dece1a264c774b8025d7919bb9561623ae86a171e0f38"),
		new("item", "industrial_weapons_police_military_0004_institutional_scabbard", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "57bd3ab0c717d2ff84eecd88577d20f048c6d525d38d5ff4ed57d0c989137b3d"),
		new("item", "industrial_weapons_police_military_0005_institutional_cartridge", "industrial", ["industrial"], ["item:industrial_weapons_police_military_0001_institutional_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "f090c1c940640baa9b2955c91babd72ffbda2c8ef5e68511f18286206ccca76d"),
		new("item", "industrial_weapons_police_military_0016_weatherproof_magazine", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e097a2b491a3fd95cf73b8ba299dc14d817a366d74b18b409d7c7edf8cc3dae"),
		new("item", "industrial_weapons_police_military_0031_precision_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "951b24c59aadbcb2777ea8740e7ca0339f8f3702f143e0059beb9887848c1b9a"),
	];
}
