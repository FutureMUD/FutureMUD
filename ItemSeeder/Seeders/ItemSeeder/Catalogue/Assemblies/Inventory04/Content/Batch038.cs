// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch038() =>
	[
		new("item", "earlymodern_military_melee_square_faced_war_hammer_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "93e80654f61b35257698bf2b66a956e7870ad5fad92675b877416c8c0eb57301"),
		new("item", "earlymodern_military_melee_straight_cavalry_pallasch_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "68f4d929ab4b3c69579e8ab3b36f220efc9899f17af6ca1f58260e4f49c67156"),
		new("item", "earlymodern_military_melee_weighted_war_club_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "247e8d23c46bfd8f7725b0c5b4cea2b524b1278b6cf95b15a1cc111b66edca6d"),
		new("item", "earlymodern_military_melee_willow_leaf_dao_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "22e58dfa70fbf32d2c4b1995c2c066580be2142fe38f9e9d897688eb456dd89c"),
		new("item", "earlymodern_military_melee_winged_war_spear_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a07d3d34ac09b8dbbb54178d68fa5a6cd8758606cab09a54724921dc1c2a9b2"),
		new("item", "earlymodern_military_naval_boarding_grapnel_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0cbde8736cb862008531a9570038ae7edddf1bdc131ccc17c06dea89100b83d"),
		new("item", "earlymodern_military_naval_boarding_net_reinforced", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "65a33481953ba230fd202eea25d8d0a45a94e84aa6902e219b24a0ce684fcbaa"),
		new("item", "earlymodern_military_naval_boat_hook_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b953f045eb08550fbe0b0651f5886826835e5e3cf2a1f3a70a34933db8e4193"),
		new("item", "earlymodern_military_ranged_ash_shortbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d2fe6a759f2426295cd0bbf9235e806ba16beae37b2a0bc100aa488b0859dab"),
		new("item", "earlymodern_military_ranged_bamboo_throwing_spear_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8aa24450a7def710f77faa19288b6fe4d777fa942a81ee99a115dc08627910d0"),
		new("item", "earlymodern_military_ranged_light_composite_bow_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d11170edf039156936fa36e991e1dcf6b769fa818eb17be8ba84a8865978f2fa"),
		new("item", "earlymodern_military_ranged_light_repeating_crossbow_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff844066117cab9fac9a3503c464f7399693e9f7ef8cb1f42b2e86ba53b3cb83"),
		new("item", "earlymodern_military_ranged_long_hunting_blowgun_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b2a2d660176a2803c833e2fdc6971f90c202ac46d9c6285ac45eb33743de054"),
		new("item", "earlymodern_military_ranged_short_hunting_blowgun_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5a024bdf599ecc74bffcea206c84d5ac080a6122ee4877217d06942d579f8b9"),
		new("item", "earlymodern_military_ranged_small_throwing_axe_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3cf5c7634a099bb8353738e50afe1946d0a91864639991e526aa11996fd0fef"),
		new("item", "earlymodern_military_ranged_staff_sling_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fe64503ba05277df75f131dfaca29e5d9f410f07ef16fc1ca412d46246d24ab"),
		new("item", "earlymodern_military_shield_field_pavise_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4879e41deef3465f03e0e77bdcb381e7518bbdf8236132699fb8062f3291e6e"),
		new("item", "earlymodern_military_shield_infantry_target_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7bdbbbf587cd58859e42c64f7d265084a858ff0ec012569dfd03833142c574b"),
		new("item", "earlymodern_military_shield_wooden_buckler_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d149acf2ce760e083f815b126e80f80bcc40ef2eebf219db932a1e8f557597a"),
		new("item", "earlymodern_military_shield_woven_wicker_shield_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "20bf97c6139b036b9a9c332019f553d88949ed9c9cb0dae9ad347055d7b29f29"),
		new("item", "earlymodern_ming_military_quilted_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c030a4db0e772ead142b2403b44294cd6b42ecbd902e6d0ae1738e6dca55c7d"),
		new("item", "earlymodern_mughal_military_flared_jama", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87958b5136b30524bea36f7024e77875ade6da72c25f56bb36355fb2c4c01c76"),
		new("item", "earlymodern_noble_shared_fine_linen_drawers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2ec7e184bced943980f968c6d06d3e136acda293513fa1eea8b7db5adb830db"),
		new("item", "earlymodern_noble_shared_fine_silk_sash", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8bbd5c6ee5155de880033652102f9d63e53bb84f30aa64e43fe625f507d1df87"),
		new("item", "earlymodern_northern_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "670b55d764fd8224a22f7383fd721768434dea9aebd6e5ac9e23d19116068cb9"),
		new("item", "earlymodern_persianate_military_guard_overvest", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "caa213994f9991d723c4d05dfa22c40d5abf97f1eabbd3a39a297f89065c130c"),
		new("item", "earlymodern_persianate_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "712a206842245c8cc408488e2cf548ac6e9cd3b83177592dd08739ad6862a433"),
		new("item", "earlymodern_persianate_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f91889217090213533fbe7dd5be38383226925d8987e3094caefe69c6c1a912f"),
		new("item", "earlymodern_qing_clothing_pleated_long_skirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8284c41486cec692589b38e21094499fe04248c9f4c4a80a35034c77c3b1fdf5"),
		new("item", "earlymodern_qing_clothing_round_skullcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db2ab939994b362f98dc0fa5eb007118238022cb3893640fe48a2c2820394e83"),
		new("item", "earlymodern_qing_military_officer_riding_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "493baddc20cd85c02a8262b6484478310e7b225ed9a4e15b8af820693a989762"),
		new("item", "earlymodern_qing_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d04305e94981ae6f5b943b1c3d7b934ffd63d2b18f469652377a6263a1f8f272"),
		new("item", "earlymodern_redsea_clothing_narrow_cotton_trousers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e3b6fa22abaeea2807a3bc4cc70ee6c4da99d0e97e9e51e5ff74573ef4e2602"),
		new("item", "earlymodern_religious_daoist_cloud_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0dcdad7737a7eac14ba21812a3eae88496c05c2ba1f89ee5de2734a3b3b47199"),
		new("item", "earlymodern_religious_jewish_head_phylactery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd9f2da844374b52facce22a40d0828a94723e45425f0be36d7825548a41c392"),
		new("item", "earlymodern_religious_joseon_ritual_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cba167b016f9dc6e7cc9b9e40af3deb45d855fa68c4d36a6a4edaf757a3d4f83"),
		new("item", "earlymodern_religious_local_fringed_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d80553d46662a8217913c9c6305be5648a0044e590259a673300ee3306ab7f71"),
		new("item", "earlymodern_religious_reformed_square_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbfa194d5fa4f662c705abf761cc8057d9f5297e17db4bf14b985d52ca3904c2"),
		new("item", "earlymodern_religious_ryukyu_noro_necklace", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9eb8bc328970ee3e2e05a6a49afc8cf6ef32a824650ac3078ef0daf068089bc"),
		new("item", "earlymodern_religious_sufi_prayer_beads", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4dd086234ff986336de6e65e7b72c7c1a970cc05ccc5a43534594e651263e6c7"),
	];
}
