// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_jewish_bronze_lampstand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c44ae194dd4ce8699ee8f6c10d988396ded26db89a331c8c73aca655e36d9cdf"),
		new("item", "medieval_jewish_washing_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "349b7f4fe633741ae86ee5892d5b8c72c43a71fd624560f3571ab0ed489bc3b1"),
		new("item", "medieval_latin_black_cowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b572f11b8026dad83ba7c0c261e8f53f8976fa1bc6297eed6ac9d52d36123cc1"),
		new("item", "medieval_latin_poor_clare_grey_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "79b1f52bd68e6f2544f4c88ed19311271d68f471387785f7ddba1939f2a538aa"),
		new("item", "medieval_latin_white_alb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "98929f8ef2ee6968460244430fb86310a78dc1b39585e648e276404035f7dc5f"),
		new("item", "medieval_latin_white_carthusian_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6a0abd3726d742366c8f655b93af1f31bfa8fa97918459fdb00850f785ba396"),
		new("item", "medieval_leather_smith_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "59674b446cde7566aac614a33f15beba940c1aee8557fb87814965bfe2e8a399"),
		new("item", "medieval_leather_trader_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6b7cc4b3b71f1525340bf41601ec12156b23098e8884234687242dc0a4ff0eb"),
		new("item", "medieval_light_double_wicked_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c297d1b6ae2e06bc7bcfac541117b2d2618c9ca4e62b5b2d880c644c5022519"),
		new("item", "medieval_light_hemp_rushlight", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef390fb67af090497cca33a1d9d411866122997a85397bedd9d3211b99803ec4"),
		new("item", "medieval_light_mediterranean_ceramic_saucer_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b24d1ba8f60748ba96118f7ebe8bd649d8f826e75d3acc9c5e2fdf21bf79ae38"),
		new("item", "medieval_light_oak_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "50dde9f1ae9ba5d053b5114f52234783f1b86db756cf696657dc938a6ac09597"),
		new("item", "medieval_light_south_asian_bronze_wall_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f49c4d71a62d35776f10b84c4e9d9f11e58c4b713c0126aabc88e97dde4e8b7f"),
		new("item", "medieval_light_steppe_travel_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d180fd45cfe82edf52654a64f24fcc03458289bca470439f86cb28b99a9d367"),
		new("item", "medieval_light_western_hall_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e14dd6d0db733e0914a22180c923ecc2c45f3866a771785761382f739e961514"),
		new("item", "medieval_linen_qamis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "21293d5ce4a9fb4e8b307998481d0f2561a0d4d673666aea7919606122c0f2aa"),
		new("item", "medieval_locking_personal_ivory_inlaid_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d61ad454c81c824c5ca209725a4f72211ef8206888f1c20b903fa698dea62e56"),
		new("item", "medieval_locking_religious_donation_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a33b790c7b5d3549ae01b21d73d0436e4e1f2fc13ee41bb0c2651040f58d358e"),
		new("item", "medieval_medical_cautery_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea5b0a83d072102137c96447eaefac765e05ce3485cae1d7086667604192a9e0"),
		new("item", "medieval_medical_field_surgeon_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d66dc8df3e1e08bbcca7245c562c942e0f7ec50ae4f4d0dd3ca816e7e6a28d6c"),
		new("item", "medieval_medical_wooden_peg_foot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "23b1bac4b50ae2a144252267532988d3d9814bd626bdf474bcdc883d628128fc"),
		new("item", "medieval_military_broad_seax_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9b5aa829ab08428e83c1fdec87615a07c10e8d37444083169da00335d5418c9"),
		new("item", "medieval_military_bronze_fitted_sword_scabbard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe3862a110ef8ebf40f09c5a63fb26e594d14542a306134055a5a92a935b61d5"),
		new("item", "medieval_military_fine_greathelm", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eefb7955e2e83d29f4b8fac33ac51244b831afc1839c6923cbc5855159502814"),
		new("item", "medieval_military_fine_pointed_kite_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdd646e69604150a9bd31efec5cacb3a606086ac7e0dac803bc21c67c1add8e2"),
		new("item", "medieval_military_fine_sword_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "25722e2f6d70b9589f6d178a8ca855f17f7227be47d6456f68f204a450a33c25"),
		new("item", "medieval_military_fine_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbce2a29d4e9d342e09a327e9d93c7152870586fda4b65c14b28cf227da72ec6"),
		new("item", "medieval_military_flanged_steel_mace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff9c81260ff94a07b0b4eea185f65421f25d296ee7d5a701b46c2ba28c7fdacc"),
		new("item", "medieval_military_flat_topped_greathelm", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e345395aeb0ccee8fbcdbc20abc8f7dde9f135fdd572e598d1ddf5e224bde96"),
		new("item", "medieval_military_heavy_padded_coif", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc7f5f06b2541f95b77aabb77e9f16d4d76c91d3ca59ac17045a7458248d2481"),
		new("item", "medieval_military_light_riders_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b41506bc1b8148904889e49b2882bbc27959134f610c639c8f4805cb4954cc4b"),
		new("item", "medieval_military_light_riding_spear", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c3483ae3462a7a6ef382af6c410191b4766b2b2907c8e3da7c41fefe7fede56"),
		new("item", "medieval_military_mail_backed_gauntlets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "65b5ed8429a0e175a5d9e6d496fc19a360ff848968c1e80199f8996b26dba1fd"),
		new("item", "medieval_military_mail_mufflers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1f134e46246e95a76b13a035eb41bd8c736e3d52c2475ed5a62e4ada6deeac5"),
		new("item", "medieval_military_nasal_spangenhelm", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e18934ffb6a606d7131b078f06453e6b663560f9ee0ab91efc37d9862325cef5"),
		new("item", "medieval_military_painted_hide_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7807b7aa187f4dbed5896ff6a9525875b71de38e0daf9463983cb149e57f31b1"),
		new("item", "medieval_military_plain_long_gripped_war_sword", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d0b7892d904e74a113bb213348a0f6b35188a51bcb0c724b97c4cc97229d72f"),
		new("item", "medieval_military_plain_military_pick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "be5633c2b57646235dc566ccdce9b57486a0b3293ff143446783078441b38d89"),
		new("item", "medieval_military_practice_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c60418cbaba44bf9be6cd4986c1cc12203f736b317e14d1555001dfca7bdb9b"),
		new("item", "medieval_military_rattan_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "339ec53540ff76f883e8e4bb9740a2f1820eceef1a92bf312aaf542cd9f22f86"),
	];
}
