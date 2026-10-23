// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch049() =>
	[
		new("item", "medieval_furniture_fur_sleeping_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9fb3be4011655e096e7810ea2aa3b0396fd763a489761a86bad992b255f1b35"),
		new("item", "medieval_furniture_hearth_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd79da77a06761c01b874754218cc5dad7513bba42fc2fa427384c9379b13a22"),
		new("item", "medieval_furniture_islamicate_cushion_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c50012c65ae69aae84ad853456132a62e09d5cce054aa5cb480811bddea1a6b"),
		new("item", "medieval_furniture_large_floor_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a181b1839bcefa947e987de333184cd52025e6664f807ccaeda4389377f36ab"),
		new("item", "medieval_furniture_mediterranean_low_couch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "53d45bd5a30e0ac395c01f1b678d9087b040084b1e5b29461d0ed11f289c97a6"),
		new("item", "medieval_furniture_plain_writing_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "034d73f5b48222fc5f2b4786b08f3f9731277760e4c2c320d72b9a13fa48506b"),
		new("item", "medieval_furniture_simple_rope_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f9039502548f3fe17db39db66499e6bfdbfe1900fd06df91ab6c41b4cc6b654"),
		new("item", "medieval_furniture_south_asian_chowki_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "93194ee0cebbb498cb16281d6c8825a72b8d4fdbc5cf0175fabf2a5c598f2599"),
		new("item", "medieval_furniture_south_asian_floor_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "22840059758cf5f624940859f966ace017c6588cc9f4e47c009dd4f8f27f145a"),
		new("item", "medieval_furniture_western_refectory_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "01785e73d52c1d1baef511810c47d441ae193ca5f0d3f26137d407b93745a75d"),
		new("item", "medieval_furniture_window_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aeb94c1a1582d92980c3ace2bc16fb609b1f4c5d2bdf1cb6c4c1d7db597e6885"),
		new("item", "medieval_heat_steppe_portable_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e28c60ab36db85f9e3ab497d87252d4ba8dea0b9fdc120ada8878c5d5e94673"),
		new("item", "medieval_heavy_beizi_overrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b6487f1fe55d67566d8d70b53a2ab7be46f9fcb3e19f8ef2f73595e82bf58cf"),
		new("item", "medieval_heavy_hooded_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed8bc6146bfb76647de6e431af164d21ed8054ed1d851e737d00ef3a6393c9dd"),
		new("item", "medieval_hindu_stone_offering_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8c2ef8918891280caa2c2288d47a51ae9fa908f743d09a1dd3de95eac5604a0"),
		new("item", "medieval_industry_stock_repair_supply_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "837ab0a9e8cd8aff58c59550ed118a07ead4796cf756cef26ebf30d8f19e6755"),
		new("item", "medieval_industry_stock_weapon_head_blank", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "807a164c81065a31fd320351e47f755b98f87a0c410d81ce118d78cf7e5273a0"),
		new("item", "medieval_jain_white_ascetic_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "009296707cb9b4d53999ef6db5d538755c0dbed90617f401426d351bf892f1cd"),
		new("item", "medieval_japanese_eboshi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "389ae658b054001f80e23edb68b456587fbcb2c7020a94f11dde6d4e76f3f8a1"),
		new("item", "medieval_jewellery_garland_dried_flower_neck_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "da56f9d2a17146aef9f43cfebca1245da4f52101344204042a3d1ad6ecd01c5c"),
		new("item", "medieval_jewellery_garland_dried_lotus_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e839ae2d9e5bec48e89142398f0a5c86a90488d1fc070cfd8b0270040bb72748"),
		new("item", "medieval_jewellery_garland_fresh_chamomile_wrist_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_chamomile_wrist_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, "aaf48b700dd35058cdce72e8f47a3bd4956368ea17983a045f32ef2b4aeaf550"),
		new("item", "medieval_jewellery_garland_fresh_daisy_head_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_daisy_head_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, "3ca66c4025bf9793846258998dd721c4a2623818a89f3cd8585de3aff86b5c01"),
		new("item", "medieval_jewellery_head_motif_hair_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a2f635cd69959fac69fc42d081ebfde20c6f61afec9fd864ae9c8ff753f8139"),
		new("item", "medieval_jewellery_head_silver_gilt_coronet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa1920f4d517326c5143415d158c28e9db32a666f68ea3f63f3c110123795ef0"),
		new("item", "medieval_jewellery_limb_copper_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "493cf7bf36e6541cf38b20aa99b7826c67592a2630a0ab510dbe51139be9d38e"),
		new("item", "medieval_jewellery_limb_leather_cord_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "353fce313881bb00445b042489df1a02fa227acbc177e2b6c8219fd2447d55f0"),
		new("item", "medieval_jewellery_limb_shell_set_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f990359d1e6b0332ffda32ce4ec2524193b9d2a606fa56b0465260f9ca1b0b5d"),
		new("item", "medieval_jewellery_neck_bone_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd537a5bc8a992de4e20bce18b0ffeba0dc19434bbfc7a514ec36318288ed57f"),
		new("item", "medieval_jewellery_neck_brass_merchant_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d40641caec2087428d36b0dff96d9a7abd1e96a913eca7fefd1a12068c1ec3e"),
		new("item", "medieval_jewellery_neck_quartz_bead_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5959f5f9d774262990673e0f04f2fe715ba1cc945a1e39cb34f9c46261240c9"),
		new("item", "medieval_jewellery_neck_silver_gilt_collar_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "547bfb35eafd5d99cf4703ffba69aa24ae2a4e92c0ec16c9b60fe06cc7e061bd"),
		new("item", "medieval_jewellery_neck_small_pendant_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1af4c4901614a393671a80e30ee312346716290b2b45e16f27ead018328bd8ca"),
		new("item", "medieval_jewellery_neck_turquoise_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e8c38dcaccdce4764380844ea79af856970e308c8d4b44018eb2f2a97ee4181"),
		new("item", "medieval_jewellery_ring_onyx_silver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3485de61913cf7ab13f409688c01d10691bb56e296f01858dfe7db0bbb2360d"),
		new("item", "medieval_jewellery_ring_signet_of_silver_gilt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc9b735d1c02c41cf5e3baf605bc043a7f4ca13b48d5f1222fef2ad12c3ee308"),
		new("item", "medieval_jewellery_ring_small_gold_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f627debeba2f467f4669878adabf6315da7266ab51bbbe0753a9bbff0ddf60a1"),
		new("item", "medieval_jewellery_ring_stamped_brass_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d774489fe72355a3cc4a03452fc78445b351e4fb0a0285bc97ac2d84fb1a9b1"),
		new("item", "medieval_jewellery_waist_bronze_belt_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bb72bd1ffc9be103d3aceaaffedcac98a1b5ec0c0de10f93afdaf02e7deaf34"),
		new("item", "medieval_jewellery_waist_bronze_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0246ad3b40255c692c768f7f493a4d6b4fb8eee949518910ae0a956907a673b6"),
	];
}
