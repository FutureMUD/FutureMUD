// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("craft", "Writing and Administration/make sizing pot", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_gelatine_sizing_pot"], ItemSeederOwnershipPolicy.StockAggregate, "d2965193d1070fecf49322b756de36101efc7420750003ee0e40bf0281a20f09"),
		new("craft", "Writing and Administration/make smoothed potsherd ostracon", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_potsherd_ostracon"], ItemSeederOwnershipPolicy.StockAggregate, "bcede24638c9792d7e672e10d614ac9cdd2a5992e456afd547a5f5c4258093f6"),
		new("craft", "Writing and Administration/make student wax tablet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_student_wax_tablet"], ItemSeederOwnershipPolicy.StockAggregate, "d7fe409620dc7a1d747cffa3164e020c751c2cd61d8ea2e22d630c6035c41306"),
		new("craft", "Writing and Administration/make wax seal cake", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_wax_seal_cake"], ItemSeederOwnershipPolicy.StockAggregate, "def47102885993f5f09ebe93248fa92e957cdeb8935913b5616a92562412a4cd"),
		new("craft", "Writing and Administration/make wax-sealable document", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_wax_sealable_document"], ItemSeederOwnershipPolicy.StockAggregate, "d0bab30d627bbeb8696952a36092f01b603fd7bcdfd324a7a8b519d1e2166595"),
		new("craft", "Writing and Administration/make wooden practice tablet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_wooden_practice_tablet"], ItemSeederOwnershipPolicy.StockAggregate, "b83a1666d466de8431fad2d9c414bf3cdffaea1735a745ed5cf6470ff3299146"),
		new("craft", "Writing and administration/bind clerk's beltable document pouch", "crafts", ["renaissance"], ["item:renaissance_personal_civic_guild_clerks_document_pouch"], ItemSeederOwnershipPolicy.StockAggregate, "423c372bb51b8d26894931546884aec3b1fd1767a009c26fa2c4e5f2c4c33a65"),
		new("craft", "Writing and administration/bind low scholar's book chest", "crafts", ["renaissance"], ["item:renaissance_furniture_east_asian_literati_scholar_book_chest"], ItemSeederOwnershipPolicy.StockAggregate, "6783670609d20d0681c6bd681ca96a46e30f6a7c4628c281f1dc79d8a4d496e7"),
		new("craft", "Writing and administration/bind massive gabled linden writing desk", "crafts", ["renaissance"], ["item:renaissance_furniture_cen_writing_desk_04"], ItemSeederOwnershipPolicy.StockAggregate, "50c5d15aa9586bbec04107dc5f5ced3689297fecf4e61947126fe01744b771b4"),
		new("craft", "Writing and administration/bind rattan silk-pattern portable writing box", "crafts", ["renaissance"], ["item:renaissance_container_sem_personal_portable_writing_box_01"], ItemSeederOwnershipPolicy.StockAggregate, "5c6d991a7e49c3ccdaa58237c586dc6dde7f6ce468fc7bb596e10d1a89a6e555"),
		new("craft", "Writing and administration/bind stitched printed music book", "crafts", ["renaissance"], ["item:renaissance_writing_printed_music_book"], ItemSeederOwnershipPolicy.StockAggregate, "670eaaeb452f07d473620215eec2132fca0ecca901cd18eaf3050d2c9c42b66f"),
		new("craft", "Writing and administration/bind willow wax-seal portable writing box", "crafts", ["renaissance"], ["item:renaissance_container_cef_personal_portable_writing_box_01"], ItemSeederOwnershipPolicy.StockAggregate, "0e1deaf8173c48457ee626ca5924a4c7836b17a131635b8e06d8c0560a8e19db"),
		new("item", "adjacent_antiquity_felt_mittens", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4495afc317913514dfa1a5b76cc43824300848bbcd7408d0dbf7b967a50976fc"),
		new("item", "adjacent_antiquity_fine_linen_sandals", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ff3869d59dc09f35fface4dd13f8427018615a75733bb5c8f1c03c4f168e261"),
		new("item", "adjacent_antiquity_split_riding_skirt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6e82a7cdf739d667e4df0c6a1dd9e83f7dfd12e1763e62dc92a682cc5bd596f"),
		new("item", "antiquity_accessory_anatolian_arrow_quiver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c09b588c969c687d69170c9d7f6b70acd3bc78ffe635329994677a76e0a254d3"),
		new("item", "antiquity_accessory_common_javelin_quiver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "76526664d4ff4f32bdec98a890ca4688d9add18286263ad65649844bec843a53"),
		new("item", "antiquity_accessory_common_leather_arrow_quiver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "90bf890c7d19c5a4904ed6c5ff69adf24ca568cbaf785c51b3533faa5433264f"),
		new("item", "antiquity_accessory_punic_short_sword_sheath", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce07a762cee8d527f5b9d0827e6a8067e12c018c67c38baff1383506344c0736"),
		new("item", "antiquity_accessory_roman_pilum_carrying_sleeve", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d57d51ff51469db0a301afbdbaa0f76448253a1af83ad72bb4775dd66cb835b1"),
		new("item", "antiquity_annealing_lehr", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "38057e2b85aa3efe0b5a95e92f9a3290e9762f50b2581e0dcf1eb7cd4fa436e1"),
		new("item", "antiquity_bronze_clad_civic_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "62a4fba06a389afca8c5c3ced460f56ae771fbc2252bc98b3aa07cefac3803d4"),
		new("item", "antiquity_bronze_cooking_pot", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a85cb54ce5130d01752e96a3231e516abc8857c634d2069d2b1ab187dcf89a56"),
		new("item", "antiquity_bronze_incense_censer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4a99f374ebefbdd539a99bc36982759f28a842e95460f15d7277159cc2e86c0"),
		new("item", "antiquity_bronze_latticed_gate", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "96cbb5707e68a31cc76b65547de44b62b928eec10293a50ce64af72f0c96a4c9"),
		new("item", "antiquity_bronze_scraper_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0feead49084f26032959e0855c1aa2e750ed3096873a282155470a030a26b399"),
		new("item", "antiquity_bronze_textile_shears", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b99130951449e561717aa4d6010f87a14a84870946956e412a986a41347c094"),
		new("item", "antiquity_bronze_workshop_chisel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bd0cf213200e18787a37d60bc78ed6022f5545a40ab9cfa3faae98832706a4e"),
		new("item", "antiquity_canvas_travel_pack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c99814a37849917fec55a83f83031672ee9e04ca5c16703502a9fcdd19122759"),
		new("item", "antiquity_cedar_panel_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc7901ed0c2d1c27066bf1ead76c5ee83ac711969dd0bc92827b9649f896756a"),
		new("item", "antiquity_coarse_hemp_grain_sack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0282af9fa2fd3aa11d173ee9acc8239a84c3809adc2d84b79a28fcff9971e304"),
		new("item", "antiquity_egyptian_leather_archer_bracer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbb7b91aff05f3544c80f02e522d7a8d4ba5225791739b584252cc4dfc713465"),
		new("item", "antiquity_etruscan_silver_inlaid_bronze_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e12fb8e5d64431c549c0e293fc9550f6bfb5c608e0ea0b0abf36eb469e480c3c"),
		new("item", "antiquity_false_weight_set", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa2507e96ee02a45dded3c6ddd60e01c80ad631b875ba2a2fbff262f239bc755"),
		new("item", "antiquity_field_repair_bundle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0fca1ff4b5c859891f151f7e941c1ec47f0d342951028778fae7288250fd9607"),
		new("item", "antiquity_fine_bordered_tablecloth", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e91503a320ef47f68f05dd2888ba388c9cd011f72e809cfc5019ee98b216bc8"),
		new("item", "antiquity_fine_long_wool_gown", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6d0705e6c3972057d3e6866db2143becaa14695b0d12982cac9c897db351bcd"),
		new("item", "antiquity_fine_patterned_anaxyrides", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "10e1c15fc6ec6e500059409a9a87ebd014b8e1718ced6c3fcbfef7b6625f5db8"),
		new("item", "antiquity_fine_wool_kandys", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "104e7c4449986420356fdec482b422a68ae742c1b7fc4d100dfb18c27c08dcf2"),
		new("item", "antiquity_folded_textile_stand", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4aa346ff27aca8eccba2cee8fcc9965c385c20c51c41a65076cbb38835bb4c4"),
	];
}
