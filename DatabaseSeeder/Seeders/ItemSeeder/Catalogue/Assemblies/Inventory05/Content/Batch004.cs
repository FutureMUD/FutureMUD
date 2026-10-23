// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch004() =>
	[
		new("component", "PreparedFood_Catalogue_preindustrial_food_saffron_braised_lamb", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a768df134c903c275414f0578a1469895e3816233809eed3c27974822b76e8f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_salted_asparagus_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdde9635ea7db83c8fbea4e2debb7057b84995f04377b9352e037ccffa780cc3"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_salted_fish_ration_cubes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c80fbb688f8ebaccd47ba3f74657e3725ad1e7403cc9081ac6e9c382db0d980"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_scrambled_brains", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "48190efb19efa23b0c4ecf2b8c9c22c3401ec28d0d0cc3c06b7d1a112aa4b0ac"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_scrambled_egg_with_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b43ca0b1744d45d275baa5ed999c002714e2f76901d682e6f4b449e9b087c44"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sesame_honey_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c97d748d70b2862554d9c1b5d0dd26ec277430810e18df6c34c46e9d30b86047"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_shipboard_grain_slab", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d75332109a33f3f02387122ffc120a21e5ef462b29a4566c3bd381a814c4c1a7"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sliced_fresh_pears", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebd671741976ddf91b4fb8d5564134408807b682a4095a0e340daa495dba4089"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_smoked_goat_strips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "85d97f12da3c13eb71c932c8cd003617b76d0f5fb9193051e7ee0bd6a7e8c7cf"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sorghum_and_onion_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "444c386529a579fc00d85c6848a96392d4cd9dd3759dac94f2e7ab06ff6651fe"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sour_gourd_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bfde8e5faa29ea0097e0cdedd1c0c719e3986b6037f4f89a02064a141b26acf"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sour_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b490257fa48bceb2ddbbd22b6dd2981d9dcd8d53b5973f21f5ba3b6594aae4b"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_soured_bean_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b681109738540c4129189c3e64d02da1c36166fbe4eda62e1db2e06b3de2cdbf"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_spiced_fried_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "57fd8bb22476c28139032dcc81f373f93e4b18bc5760ed5dd1c50e8f928fc125"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_spinach_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4223a01566ecd1922cfa460d96700172a773e389900adbbcde9b900e0eaff50e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_stuffed_meat_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6504bf5137c36fab651ab5d54a3f9c98035d4a89a891b96ac3285754bab6f7c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_stuffed_okra", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5459dc44d53eca11e930b50f4b24d836aa1983e2bcda64736e2bc751af3a484a"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sugar_fruit_compote", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c5e4827b5b3095d3ae9224e05e42cf96206828b71e5f2a688c5aa75f36a84bc"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_thick_pork_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6122eea02045e72cac1489e83bcd5e403cd1324488b2bd0a21e905ee084cf24"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_thin_dried_wheat_wafers", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2066cc014efbdfbd8a69a5b540e367afd0b2506db4ff92c3d48296e4b81b1ceb"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_toasted_corn_kernel_pouch", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfcc9195485acb27a02c135f9626294f7898b18876e385f225328a827b629837"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_wheat_and_pulse_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "30a49141479e875c84f95a3bbe1ff265a7df1fc38f8f2f9cd73ac5a0064abf60"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_wheatsheaf_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2b8c6cf5cd17ee2d8b1bf02f9598676bdc90c5050d802351164db8d49b9c8ae"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_wind_dried_meat_ribbons", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66fa7ab87e0c42e28a0494dedad45af09725dfaf2a2cea2c61b6842f77ecc654"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_wine_poached_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c7dc59cc74ca3413644d4c32cf8f66f2b7d3594299ffa95ebe1e1d72bf18ef3"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_yogurt_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc156111411b5a632a4a08dbd4c8740d3e53fd206b1e5ab4b47e0e950db90b19"),
		new("component", "PreparedFood_Catalogue_renaissance_food_andean_army_charqui_ration", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "64e6ec4d106c6f02091764e40682c1757b3e29ce08a79493af83ba4e4dd0ca54"),
		new("component", "PreparedFood_Catalogue_renaissance_food_andean_chicha_residue_bread", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82bda14b652d0c6f1cc72318989af2f7f0f193e0f59d5f83e74321c7b8dab579"),
		new("component", "PreparedFood_Catalogue_renaissance_food_andean_pepper_stuffed_tuber", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "62a30e2295a8591af2d379578ee26a04573056f2dc41600879f1354d2694ce88"),
		new("component", "PreparedFood_Catalogue_renaissance_food_andean_royal_maize_tamale", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a99406c93af755f2be29f03eebc07128f48b4cf01b34c71a24cede38cb08047f"),
		new("component", "PreparedFood_Catalogue_renaissance_food_aztec_amaranth_deity_figure", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f99f8042b94ae4f757d363ad711947f41c57973eb0b68776dfacddf9ecd83be"),
		new("component", "PreparedFood_Catalogue_renaissance_food_aztec_lake_algae_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "91bcfbb5c0bf7a9eb7519ac6d7d964c5118382aef8bd8316b85b28a7a6db7fdc"),
		new("component", "PreparedFood_Catalogue_renaissance_food_english_banquet_marchpane", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39bb643d730f992ff9f2a18d76dc035767d943ed83e2aba7ce8c4b55704d9d69"),
		new("component", "PreparedFood_Catalogue_renaissance_food_french_cream_tart", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bce64c728db8f6c33be939ea20e3335d9b077b41dc73a42dbb3e20ad6b6732fa"),
		new("component", "PreparedFood_Catalogue_renaissance_food_french_sugared_anise_comfits", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4129ac46b6b0844eac8822a767a49c5c2c6b972b3a091feae434e432a9d9ec04"),
		new("component", "PreparedFood_Catalogue_renaissance_food_german_moulded_lebkuchen", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c2acf3a793596089e15c8ea2ab0b22550ad616d94d14e85f59a1fc2a5b97f4b"),
		new("component", "PreparedFood_Catalogue_renaissance_food_iberian_tuna_empanada", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "979c1550847b96f994cc1ce9dc0081108719b14c58d301480099da5b474bdf6d"),
		new("component", "PreparedFood_Catalogue_renaissance_food_italian_buttered_macaroni", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8190f4d11d1f3494ed83ac9c9288bdaa57507b341ce4cc52ac45205ff3fa967"),
		new("component", "PreparedFood_Catalogue_renaissance_food_italian_revested_capon_showpiece", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f323f1c0d4acd8ec913bbd0f84c953f55cef3393d2524f6f718c39d68fa5a458"),
		new("component", "PreparedFood_Catalogue_renaissance_food_ming_tea_house_soup_dumplings", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "55605bc625dc23429ffbf24f4d5edec01b58d16c454e63a492f3d651df06954e"),
	];
}
