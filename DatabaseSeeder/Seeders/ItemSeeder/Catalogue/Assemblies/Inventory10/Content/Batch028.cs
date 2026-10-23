// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch028() =>
	[
		new("item", "antiquity_lit_updraft_kiln", "antiquity", ["antiquity"], ["item:antiquity_updraft_kiln"], ItemSeederOwnershipPolicy.RequiredRelationship, "148397a875c85fc056eeb71012fbf80fde9f22a7a2068a87d734a49ab41e14b6"),
		new("item", "antiquity_lockable_cedar_panel_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b60ba2c1ba1df109575bee445afd1f40f7c45d3b2e254ac3c202ca703fd539b6"),
		new("item", "antiquity_lockable_oak_house_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e61452871c30232932d16789c4533c2ca2878d1b11226610277cd46626bf70d"),
		new("item", "antiquity_long_sleeved_wool_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f051a79e0ab6e8bbe50606d1f330c690b3c05d48a54f1d5faf9a587596a32e93"),
		new("item", "antiquity_merchant_tax_tablet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a60c0da7a8f92383c15b82dc52317400885ba4998f2d6608b624814f13b8102"),
		new("item", "antiquity_overlapping_wool_skirt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "10c7d6ca09703b8ac3c45dab4afc6602e731258e8000d2a15f8ae36ba9efe8da"),
		new("item", "antiquity_palisade_camp_gate", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5de2da4061277447c89d8ee159ee2d841d3f1c684c268de81a5a06c5e78aeae"),
		new("item", "antiquity_patterned_carpet_door_hanging", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b69b7458fb1c94ab616f557a21212ff895e72d55cfdee42bfe2f64f2e9d2ebb3"),
		new("item", "antiquity_plain_side_table", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1dd69eadc4290de31af4642877bce11afb0d6f729e440361be16fa5770d3679"),
		new("item", "antiquity_punic_fine_painted_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "651c092451f836ebca072d2e55ed839f87e83cf77038aee94c8b237a2fde7143"),
		new("item", "antiquity_roman_lorica_hamata_fine_mail_shirt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "296efd715a21b59c6b0d87aaa1fd880da5b206098d8e691e2e8bccd6cb2e95e0"),
		new("item", "antiquity_rush_seated_stool", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e2fcc37e02fa24563db7f1b04f861321e3ffdc8a8bdd8340f8a928bbea01218"),
		new("item", "antiquity_shield_persian_round_dhal", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd8ce8df2c4a3279f9d2286ddd2e99c08302c8fa24e29366da4d30934cf0732e"),
		new("item", "antiquity_tableware_egyptian_alabaster_offering_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bed1d522ad35db853869a814b10a8b61d5eae0f9ca2cff830a5716b30d11d43"),
		new("item", "antiquity_tableware_egyptian_blue_glass_goblet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "83c2c132ea4d656e7ee77402f0a3abfa3d31362447df35a9670c75b7e156fd26"),
		new("item", "antiquity_tableware_persian_bronze_banquet_tray", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b63b4081aa960b3f75c18ebc1e099d05b4790921341758acf49445d450df00b"),
		new("item", "antiquity_tavern_gambling_set", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "76656ef9ea0954b81354e896e7d54ad5229a5fc3b8ff9f255bcdaea641988e17"),
		new("item", "antiquity_walnut_sideboard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b65837e13558cf6533905fcbf39e23e325b5f2cffdf2a764dcc04b97f9e16e7c"),
		new("item", "antiquity_weapon_anatolian_bronze_spear", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5815393553abd7c00808e63d1b517fda1cd00f71dbfb998f3058dc8b775c78bc"),
		new("item", "antiquity_weapon_anatolian_curved_short_sword", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f948e4fd8cf532914e7402d511bdc19b9e231e82103bf5fb627265e82e7a3765"),
		new("item", "antiquity_weapon_celtic_leaf_bladed_spear", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "83f5774690ba67e48a66eeca579f34b586a68f664e637e2f2816be26db3af351"),
		new("item", "antiquity_weapon_egyptian_khopesh", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c8149fae26da973954dc2e8b108c03e84e76811a69889f9e3d25a73b7477bc0"),
		new("item", "antiquity_weapon_egyptian_stone_mace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "200c680c7522a0818f89035609904e73f098a799077c13f23919dbc6a78685d7"),
		new("item", "antiquity_weapon_etruscan_light_javelin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9581fbc2f9fac3abf9a439d63023fcb2d27cf65e2664d35f6e3f3b50cb42e931"),
		new("item", "antiquity_weapon_kushite_iron_spear", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c54375361deb2d6f022d49ef958d7b4f6cbcc28c129a233e00f0abb98e58f0e4"),
		new("item", "antiquity_weapon_kushite_light_javelin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "403d998c3f914ae4bb8cf2c1dd0e00d23c1d7da07ddc7ed58024acc3c7a8d031"),
		new("item", "antiquity_weapon_punic_socketed_infantry_spear", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa4e9a0144972f44b36df15faa4ea61c636af0c60a2b11a71ed26b9b0e09a8d4"),
		new("item", "antiquity_weapon_scythian_sagaris", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7149aeb3b34e5bfe8190990db00685ebbccf8c26d1b5fa0a7aaef2f0b1dc491f"),
		new("item", "antiquity_willow_bark_packets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "72b8700e92f8b4fc1609b93d9b7dec562263242e3627cbe8afb9248a208f522f"),
		new("item", "antiquity_wool_braccae", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "462a47f8dd7b21b80fc5b22ca80e0b55176fac8352b5248babff695c330b80a7"),
		new("item", "antiquity_wool_travel_leg_wraps", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd75628ec50266ab96c550cf62f08b391773e89527f17109536f2b4a1b9931e2"),
		new("item", "antiquity_woollen_bedroll", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "932075c681df4cae63b6ab729ba0213346433b287942abab1d0ca493cf9a3c38"),
		new("item", "earlymodern_andean_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7d1d0c92f969bc7c280783afc3cbced8c081489c147ef62fbf72363ab91fbcb"),
		new("item", "earlymodern_brazil_noble_male_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef6839e979f496299fb94f285fe629ebd68c3413edb6d5f867c58ab3fd7f6f89"),
		new("item", "earlymodern_british_clothing_linen_smock_frock", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6b7649d099385b183039b2f911d4d17ef55f6422e84ab604fd248afb62f22c4"),
		new("item", "earlymodern_british_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81778b3ba4e04e4741c5633b2947a0acdf1c5205443045787cbead1d330b1ad3"),
		new("item", "earlymodern_caribbean_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b5f470040a8acf7f4b66c2a2c086bc1f692af4b39f7fff5ed6e83e4e3dc37c8"),
		new("item", "earlymodern_colonial_clothing_broad_felt_sunhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "abfcd10e23c346ce7592b7b022264337d1653688072237528874e54c0da13abb"),
		new("item", "earlymodern_container_and_travel_coffer_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "253e46291fb92d327ea2ed514b243868152caa938d9425dc0f23c4300e204df0"),
		new("item", "earlymodern_container_cna_account_box_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4d4a153a8d147f3fbd37ae42d26cd8a8769aadb794a262c474a16bbea71fd6c"),
	];
}
