// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch053() =>
	[
		new("item", "preindustrial_door_escutcheon_60", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2afb8a1cffbfb6f6405cdf6a1ef6194c5c49d3277bdc41407b55487d3634f11f"),
		new("item", "preindustrial_door_good_warded_lock_62", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5738eae478707687f9b2814a7bd6298046fcde29bbf7ff72b4875becbff4b42"),
		new("item", "preindustrial_door_panel_door_15", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "677b9a9172d8552d09d3d9aaef54bdd0c806d3b458fb8515276df9ad11c3e810"),
		new("item", "preindustrial_door_warded_key_64", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b81b7c29b36006349f64674ebc440a1981b335680feec5015bba59b481d0e1a5"),
		new("item", "preindustrial_door_wicket_42", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "10429c9f1e28c021db0759ef5f2bf8f46b8604301e480d4828486b88a6854066"),
		new("item", "preindustrial_food_bean_and_leek_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8e2ab7cc37342429aefa34048f9954e1ad0fffdf1ab75568f48fd9d0d1451e1"),
		new("item", "preindustrial_food_bean_maize_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "126187313edf7a839d6435004b6ea606ce852feaa99e7a81465260e8b2a69d43"),
		new("item", "preindustrial_food_beetroot_and_leek_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "56bfb3f21f4840d72780e7744a5a670dc39f97ce20a16637aeafc2cad23a1d37"),
		new("item", "preindustrial_food_black_eyed_pea_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "070ed517a7bff697e5877168b4be643b41477a337760e74f780f879c9499c5c2"),
		new("item", "preindustrial_food_blood_and_grain_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "275277eec5cac99e785a986a161b55b5d3760ac601d8677ae9b984ddd208cdf6"),
		new("item", "preindustrial_food_boiled_aubergine", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a69ad39b06633a9fa2c2a35358138a173e8a07426af8fb0f15714d76574fd753"),
		new("item", "preindustrial_food_boiled_barley_dough_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac47174d6be420298b3873f25c35915d6d44853f918cec548ae0338392a1764b"),
		new("item", "preindustrial_food_boiled_grain_with_dripping", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d4eda48d7068f826b7501c6fad35658a1f370489f4911c3d7fcc84bfcaebf0e"),
		new("item", "preindustrial_food_boiled_pigeon", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1cfcc10822189662777ccb17b106842d79286d1d20ccd791a2d5c25deaa92c7"),
		new("item", "preindustrial_food_boned_fish_fillet_sheet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d4dc70132a26461fdf885471222217a20218da855dd0efb86533b3b28f9cdf9"),
		new("item", "preindustrial_food_bread_and_leek_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82056cac8c4f2460d59a6a88dd37c88934c4fcbf681a16c5edd686ffc36e64ad"),
		new("item", "preindustrial_food_broth_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "61de77cd110a58db0b18e7ab5245a4e2a85d926c2696b8f2210d5ff0860e642f"),
		new("item", "preindustrial_food_browned_butter_spread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b12b2a9da6e006a95b86f5373f720be67bc10f59d86a4b41b3ad106631dd73b"),
		new("item", "preindustrial_food_cacao_honey_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5f8cf9024a45abf58b0d33bf9d124c8df9e6b2102aa3c7823e47b98e0c14ad3"),
		new("item", "preindustrial_food_cacao_nut_confection", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e3ec98fb6dfedb8b9f68d6b2164c65c162201d7d91d87de129abfb306842d5b"),
		new("item", "preindustrial_food_charred_aubergine_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0407fd373d01ae636ffc285937783e5b7717cfa7e92cf711ef383f34b116f59"),
		new("item", "preindustrial_food_cheese_and_leek_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8baccfe2d0c00052c492e2c9fa9deaf8a91e5ae8fa2a99f3bebdef25fb713b1"),
		new("item", "preindustrial_food_cheese_capped_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81e469ba3c8613bffef07de7437924c4f6a50b1f9b82c8f1ef79f7b2e6172f51"),
		new("item", "preindustrial_food_cheese_grain_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "367917bc359ffa25df9bf1870e1ae256e1f705550c84036e17fd8e106d1483c3"),
		new("item", "preindustrial_food_cheese_scrambled_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d3965a12fdbb974af36e06e3128682352a8859be5a27fdca9a57f7e59a743a2"),
		new("item", "preindustrial_food_chicken_root_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7155cc5f707c0a5f41e1f59bc70fc72f1e33f21e10595ceb1e026819ac01911"),
		new("item", "preindustrial_food_cold_sesame_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b36d79a62d7da0ef19e73830eafeab6926d23344f3c347613f691cb7e4678737"),
		new("item", "preindustrial_food_cold_set_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7c38e23f7ace798b7f7b29e7c48485827feda07f5502f56f62ca2ce8c1babfa"),
		new("item", "preindustrial_food_cowpeas_with_millet_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3afb3f7075d866c6f4f265a0743b4bfa4627f94519d1b96d78067bd33a294b4b"),
		new("item", "preindustrial_food_crustacean_rice_pilaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ead2e43cce0273f30c660f8dc2853c494d271b5170e2f44f462f972e28d7219"),
		new("item", "preindustrial_food_crustacean_shell_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "556df5ae696aaadadb47b5c6e76fc2a7dc9838b712847450b565f5d0bdc100a8"),
		new("item", "preindustrial_food_date_rice_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "902ef4729aa65000a718032c7659e9f96b073cc07e2612e28fddb0fda1f1a3f0"),
		new("item", "preindustrial_food_dried_fish_flake_packet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a54e80918242e1ff85f8cae6792dd5c5ba16151df2af4ff31336cc81afa5753"),
		new("item", "preindustrial_food_dried_meat_and_grain_block", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b7c9e31c464c74ee1e25d69667c7e0d4ff7d3c820a728fe0986ed6a699688ce"),
		new("item", "preindustrial_food_dripping_basted_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f803ec7f6430029286b36387b5b68a2659c01e9efbcf711bfe10409e4f0b9bb"),
		new("item", "preindustrial_food_egg_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e4d4170d04c11dd26a37a52af655a9e6e270e90bd799210f7288e87221e75840"),
		new("item", "preindustrial_food_egg_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3cc4812ddf53d8bc5139861627b5c50354054915e0a63067159d7104792f272"),
		new("item", "preindustrial_food_fermented_fruit_paste_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4fd921733a596f7cd2cdda7662211e79b31324ef15ccffb0a7af1250d6938dd"),
		new("item", "preindustrial_food_fish_and_barley_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6798884b905a61a4c652c8afa6ebec526701e62da7efde01f04cb68029fdfa3"),
		new("item", "preindustrial_food_fish_and_rice_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "04a11f6580276e5aad5237bd5a7bfc97754e549a7dbf7df6732927684d4caf36"),
	];
}
