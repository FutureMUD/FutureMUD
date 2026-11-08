// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("commodity-spoilage", "Broth base commodity spoilage", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9855128b8c8b98a11e32573fee1766814a595cf7849129ed59f4f40934a8b2a1"),
		new("commodity-spoilage", "Prepared meat commodity spoilage", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d0cc2cab29dd371a18677625adb6e10e21fa792140b7986ecdc11435a2b5eda"),
		new("component", "Antiquity_Bone_Stylus", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ed732bae295de315d2203e770559e28472339e6d177296f33a394b6e997bb0c"),
		new("component", "PreparedFood_Antiquity_LuxurySweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dbbb77c2634d93da3bd45b178c66c99e2922894a2b70170069c551ac9f53192"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_hot_cross_bun", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "05a7eaba7e9665f6ec34920389e2ce199d93c10ededecab404067dfcb755e011"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_company_kedgeree_predecessor", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7c89e5401e2fac535fb9411515a3a8333030307293ee8e2500f69629e58fe7d"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_mushroom_ragout", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1814899e05468b48cbca33b887e6560aab89009f6e0268961ffeafb5f63d4fd0"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_puff_pastry_cream_slice", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "440e37786d515e6330565e371d136460cf9345f53d7debb12776b2da7dd9bb74"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_white_fricassee_chicken", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cafb95581327a246037078f92efc9c100129e805ceac4d23ebefc091ccd0afcf"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_iberian_chocolate_churros", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fab402462f5a668c95b7162281989443dad73d2edc6047026e58d5f854acd0ce"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mediterranean_stuffed_tomatoes", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "77628f3855b9280a481e56b306a5494493f7e949df1bb4fe67c8cc2b315b8413"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_silver_leaf_barfi", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0ad53a73f0505f99b7eca8da77085d19f4572c4aa93be908e72333f9275de44"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_imperial_kebab_array", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c24473a7ccf233092775044d857be1357a7099765a182d22511ce741edd5470"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_ottoman_shipboard_bulgur_pilav", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd38726ef2355a28e83d74efc240267006d80439a83a75c2bfd7b42c91a14d14"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_plains_bison_maize_pouch", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d8c7c9730aae81d194f4fa2d6cdf7ac7b777f97c9e6fc179de30eb14f1561ad"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_safavid_stuffed_fruit_platter", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd0237fdf0c8ce2a1864f106ec5ad0291577cee3e7ec3d127fa9fbba09b7bf03"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_grilled_eel_rice", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4594d46d1e0f44547509dcb076b4566987fecf29378dc6444a2bddf6b3dc01bc"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_miso_pickled_fish", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f7f1728e23204cd156c54b90ba168981d7d0b0e2076781c894a46ab0089ccd7"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_tokugawa_sushi_box", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a37b3c2d7332251b3f5d4edd08b4fdcc5cf3409754bf1b7dab61b2d1225f2a75"),
		new("component", "PreparedFood_Catalogue_medieval_food_black_pepper_sauce_game", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f266fedf35a6e3d69b31b61bbcd905378aeccdda2aa1282d0e177a1def286718"),
		new("component", "PreparedFood_Catalogue_medieval_food_byzantine_wine_jellied_fowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2ab54e06b8b154986d57712e754dc8b74232c75e4da460102e86cfaed608587"),
		new("component", "PreparedFood_Catalogue_medieval_food_chicken_harisa_feast_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "21a386f87b99cb90ae230fc5ca2ced15f709398963b9ee8d86317f41e279893b"),
		new("component", "PreparedFood_Catalogue_medieval_food_ethiopian_honey_bread_offering", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8972407df7f3c888544ad88d049918bd0fd7c011dededbc081c066c60f207fd5"),
		new("component", "PreparedFood_Catalogue_medieval_food_green_sauce_cold_fowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c609a5e99840bc8631863881317338bb77153fd9852eab856904bdb9bec8203d"),
		new("component", "PreparedFood_Catalogue_medieval_food_monastery_tofu_skin_parcel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5cf886b234b4845a6a2cc98de82f8b6d296929c0c4720d95a1bd1de62a19cac8"),
		new("component", "PreparedFood_Catalogue_medieval_food_new_year_crescent_jiaozi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "021a599ac488d59e582040ce0eec314c01cd9ac41a28af6bf923e25373411b38"),
		new("component", "PreparedFood_Catalogue_medieval_food_pottage_filled_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5723f7a33be31eb910f1cf5d799f5a3a9fb3c167a71772302c047e95c8964e7"),
		new("component", "PreparedFood_Catalogue_medieval_food_song_braised_gluten_mock_meat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3965a44deaf25f45a61653f7a810b44344ee0a966e63ea03e41fe9c9159537f"),
		new("component", "PreparedFood_Catalogue_medieval_food_song_sesame_honey_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3af49a0acc57255f8b490b97874c2123d273d7b3f7be8d8d57043b4cbfd9e2ea"),
		new("component", "PreparedFood_Catalogue_medieval_food_sweet_pulse_stuffed_poli", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "929b7889f096abbd1e0f6407de03b85f8de1db502c62586d7d504bdd1c6b2601"),
		new("component", "PreparedFood_Catalogue_medieval_food_temple_sesame_sweet_ball", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a36fce7e3ff5aa971ebd05114076d99c6275fcc410167d456597e9f59d4cca9"),
		new("component", "PreparedFood_Catalogue_medieval_food_white_fasting_blaunche", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ec157fdcf6dd49bac2264b268629faad449d5cff2720896d5c261dadb01e4bb"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_aged_cheese_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe7b2c076445815dc57fb47e8c2c715598481029ff92281624f37f1ca90ae6bd"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_apple_batter_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ad330976003820ec07564f5ad5ce6aa2a611e8b906de83f99d4a3c1b86baf7a"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_ash_buried_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b646a61aea7b6af65ef93bf376e37ec4fce4f12fdaf9537db9463b5abe738cc0"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_asparagus_with_mushrooms", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8655a284ac97aa73da7d5e3f676eedec97eca6757b9d73ec58ac66ffeda9360"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_baked_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cef7de8f41227ac1d8bbcb10115423007f6930536528461193233d66f6088bf"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_batter_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d1d0ee4c68ed9571fe602954539854ed927943f3e84c6416b5eff123f29b227"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_cabbage_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "57b7b448520a9ce90c8d5b507d4acf0b750f12df12b5dd5861dfc0289851f538"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_onion_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b585001a895361145a3b6cbad08250002cb71852e6a6f408582b2dabb9383c3"),
	];
}
