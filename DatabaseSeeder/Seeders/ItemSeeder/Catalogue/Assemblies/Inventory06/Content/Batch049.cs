// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch049() =>
	[
		new("item", "medieval_jewellery_garland_dried_chamomile_wrist_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0aedb7f278ca4ae5e793cdc77db888a1b401026b8ec43aae820c92ff5996bc34"),
		new("item", "medieval_jewellery_garland_fresh_marigold_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_marigold_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, "ae17336cdf9739742eec701a3e38c1d76b76bbacfa32221992ab4b8897649951"),
		new("item", "medieval_jewellery_head_pair_of_silver_hairpins", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe18f9d38423ac77fc120ce2be7d041c7745b431595c2d22c121f9e0b3e747ab"),
		new("item", "medieval_jewellery_head_silver_hair_plaque", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c67ae7bf1a56ca2e8d0775c516a40561d88c517199433329e8a04c0d2c50292"),
		new("item", "medieval_jewellery_limb_brass_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b61864beb3f4c4311fed04512ba494675f0183d8ab89a709bdcdd27af6843c5"),
		new("item", "medieval_jewellery_limb_glass_bead_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5a8e029ea35fc3ce96d8a7cc9dbe78377c12278e5ec78c72f57b934ae9bf2ee"),
		new("item", "medieval_jewellery_limb_gold_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "80b8fb51eb95e2512445299b82eb280ff1103e49c5a0923767e4b3ec077c6c29"),
		new("item", "medieval_jewellery_neck_carved_amber_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "db84bb2e833161a8d8ba501f2b54bf2458b088d05d9e6a8be4734cdd5c242933"),
		new("item", "medieval_jewellery_neck_silver_crescent_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fcb216d84ee45fe3a61fb63ab1fbf52a21b257e8495c9e1ce34baeee070c92f"),
		new("item", "medieval_jewellery_ring_child_sized_copper_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d6c92c2ac0dc8da32f338daeed9e52774ca13cbf044b4974302e6c5177a2e8f"),
		new("item", "medieval_jewellery_ring_coral_set_silver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "30f68b6a1d31546ac120eab583bf5f0b4f2f40fc42ef400de87235452eebccc1"),
		new("item", "medieval_jewellery_ring_emerald_court_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f749bf92599cc6ae161384c3c7b168d1c7e86acdbc37635b232ef986463a1f0"),
		new("item", "medieval_jewellery_ring_hammered_bronze_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa4b8fb2857b5d019fd2a25b89faad396db0c89a94f8a60d1f7c02a945e4e116"),
		new("item", "medieval_jewellery_ring_knotwork_love_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f805aafba92b8ea6bbc17bfd06a1a440edd1d8f81e938a982f0a82722b71af24"),
		new("item", "medieval_jewellery_ring_lapis_signet_like_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2587794bf52f29cde9dd5fd537e4148525faa44a78120dbc0f9454caf983b19d"),
		new("item", "medieval_jewellery_ring_motif_gold_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4c3dda913beeb7c9ea0c2b35b474dfa87964e0afc6e927faf0bc49d526a4205"),
		new("item", "medieval_jewellery_ring_personal_seal_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "80bb067b11f7719abd1409511c235194fa8b4b23f01156467aec9b6478d901ce"),
		new("item", "medieval_jewellery_waist_gold_girdle_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7a802a22b45452a1a0f0e2a86a8ffee095566edc02752018bd95ea17b17d5d5"),
		new("item", "medieval_jewellery_waist_set_of_bronze_belt_plaques", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfad76689eb64e432b5ea6c866186d31ea795cad1bc837c908f1f707a1732292"),
		new("item", "medieval_latin_simple_nun_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "68cbb88ec0ba43fe5a430d78ef3849e576bb985c46b624297db72f2ede38c5e7"),
		new("item", "medieval_latin_white_friar_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eff9448f517e69f81867e07409412767e998eb63950227d9833e58b0fdfe3a31"),
		new("item", "medieval_leather_work_gloves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d849364126070815edd7aa0d3702f4bec7bf98781c1d5ab33c1a70456904d3d1"),
		new("item", "medieval_light_ash_watch_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad161c8e353deafdff6cfab8bd72d8b637e9e3e8e9dfa3e3b719ef603ff27a70"),
		new("item", "medieval_light_east_asian_bamboo_paper_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "074a03e1a99893da77ee02d8b065f4fdff4899ee94f23b1bc21fa1a53a2a6474"),
		new("item", "medieval_light_east_asian_bronze_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c21761c4023e693d02276a034f106c1560060927fffc299df4c487e5ca8ca5bc"),
		new("item", "medieval_light_east_asian_cypress_frame_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bf7b195eba42d3d063cf9181a5591bf796f38d498ff9ac42247b40194dabc22"),
		new("item", "medieval_light_east_asian_pine_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5f6e54c88af092807c40d12135aa0306ecde5509cd683e308095863c12c5699"),
		new("item", "medieval_light_fired_clay_pinched_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb97ec291bba08e12c6b641a4a22e8d389222a25301fc090cd63bd7882c1b3cf"),
		new("item", "medieval_light_horn_window_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "086d03f5619c6384db414db00231a73d84c4aee2188e453f0a7ab5539d9d49b5"),
		new("item", "medieval_light_iron_candle_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "09c0300684caf3ea9d78a8ca26cf596ae23fc39c44f7183add0d158f99fe1d00"),
		new("item", "medieval_light_mediterranean_brass_pierced_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4add266087faa1ebd274cfb284562726171336bc8c1774045b09c41166408900"),
		new("item", "medieval_light_northern_horn_stable_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a06d4f69c602a3c5357ec2da688a7cc4875f8b8008949c962b46360d6c774f75"),
		new("item", "medieval_light_plain_dipped_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b24a670ab07f0863237e0200422a68343827397b5555bb2d02cf8280d0edfc58"),
		new("item", "medieval_light_south_asian_bronze_standing_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "60dfb0bd76149233d5529ab266875df3f4a656cd5fda9d20867ec0334f229708"),
		new("item", "medieval_light_steppe_bronze_camp_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eddebddaf81fb18a39d4ed383922c7b1648626f71136dba9746257a7124a2e35"),
		new("item", "medieval_lined_wool_hose", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9ef2bedcc08b064117e0e3d02afeb820213f4e73bbf995dddb14e0cc21d5148"),
		new("item", "medieval_linen_chainse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4d561c7daec0d7ec0d52e8aa7041be1c3ff80a8ecb6089bb625c2a977d10f02"),
		new("item", "medieval_locking_church_offertory_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a59fd52deb6a8b6162162be3d114927c33126adb56729346dcdb4c180c60c343"),
		new("item", "medieval_locking_domestic_bedchamber_coin_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddff44fed07ac494dcd0f3ad705471e0e6bd621cc0122ac85c40210882f29793"),
		new("item", "medieval_locking_jewish_torah_ark_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "555eeede7c309544f0cee0c3e81717395151c36284156ef87cfd5ca831f99e77"),
	];
}
