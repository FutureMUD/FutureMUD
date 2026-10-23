// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Papyrus_Sheet_Surface", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "00979c72f12241a00dafc127b9a3a8f5c902a44a9a57b33f248c9bc4d6c77daa"),
		new("component", "Antiquity_Wax_Diptych_Surface", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "529b0acca26c18eebafc33013d52e51d8ca15a90f0ea9a0b7dbc6df5463c5ab3"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_british_caraway_seed_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "10016ccf9e34dcb56a4c9533d890c2db021e1ea7804ca3be477def48493e9ac8"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_petit_four_selection", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d95627abad48784971456a7a92333de0ab7cee306c52256d09382229a6aa6ba"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_roux_veloute_fowl", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a91529da17e4dc44043560a3163b8467cba22c718bb953d611bb2807f48ca1ce"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_german_spiced_chocolate_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b7aeaebc2cdaa301f5065c09bac0ac9a883a4a3048ab74a29a24f7c0cc48162"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_joseon_royal_fruit_punch_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "868298b4a6621f1921818c14b840d062340949609580b72376b367b2728f7822"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_joseon_tteokguk_ritual_soup", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1732fcd0fdede5dce7dae46cd0d4febc212c0e0bbdd6711076d582964931f514"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_kongo_maize_groundnut_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b327d44cd319d114623aa7aa43793a7b8efa5e3b9d2717eaa39a8c3c09bf11b9"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mediterranean_maize_cheese_porridge", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c486ede94fae7c0991c7350a702770ed9b622ba4f1e59906fae94ddf67609ef"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_naval_salmagundi_platter", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb095eaa01b30f8c4c0263dddc65b4e02ef7185c5f37551d2747d9c29b03985c"),
		new("component", "PreparedFood_Catalogue_medieval_food_abbasid_bread_tharid", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "78c8eb85fcd56d4782a9b4bab84111667b720065c8388abb75825c5cdab5cddf"),
		new("component", "PreparedFood_Catalogue_medieval_food_bird_filled_battalia_pie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d82e512a33c95d1848ec59e755a0e7c477b77a28a6c593f6107155495f246c93"),
		new("component", "PreparedFood_Catalogue_medieval_food_egredouce_meat_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e38083e134fbf3da45991a9016ef9ef03f9b2663e7a481007e1fb0b9e3b099fe"),
		new("component", "PreparedFood_Catalogue_medieval_food_ethiopian_royal_layered_injera", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1eda8d1da4dcf146dc063d46d54f73c28ec6f4c54fa8fe60dd4f92e8f4ed669"),
		new("component", "PreparedFood_Catalogue_medieval_food_isfidbaj_white_stew", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9e6d83f27a779c9e4e1e58313bbcde8338008a30e54c63439da22d66e3b2a87"),
		new("component", "PreparedFood_Catalogue_medieval_food_lenten_fish_rissoles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d30998f2bd718a29dc304b0b04f2d610a255334c33a0eacc339ea04556b3bcc6"),
		new("component", "PreparedFood_Catalogue_medieval_food_persianate_sour_cherry_rice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "945e88976f03a64621e8b2ddc6c9e40e769c6604abb2cca1708e3cdab9f2eb59"),
		new("component", "PreparedFood_Catalogue_medieval_food_royal_betel_spice_sweet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a8ce3e2184fb4fdc35eaa86a4d9b50397dfdf93abfec57f7b9fd35960a084df"),
		new("component", "PreparedFood_Catalogue_medieval_food_royal_chicken_tharid", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1218f31a677da2b25cbb8e0501bb8799d4ed6b91afe218096dfe0771007101cd"),
		new("component", "PreparedFood_Catalogue_medieval_food_sahelian_army_millet_meat_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "43a9b45e7393fdad4d9d71f80e8a8a37719d963731760ae5ebbf97c2a3a2398e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_ash_baked_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa12f4c132ca32dacc16effd5c178147e8104e4803cba6d04edd6f3d0230187b"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_asparagus_and_herb_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f2b5a94484a403f061616fdc429c63a28bd192bf38f8d09c9249d4067bfa207"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_aubergine_and_lentil_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "afdb927f6d11c8fee9f851ffb9919a5fb56ba7e52ad62c361d455abaf83402d6"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_gruel_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2fa2404aea6f45e04a0c7bcc3dd890c2d306527ab643487170eb8ff0e7050455"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c54d74027128fce971b5bc035ef13fd16353ee31e71b41054ccfe5572cf6c59e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_onion_hash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fffd6eaabab66aa4fb3c82bde5ec40dd0580182768fe87edd89f78d5378f9c67"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beef_mushroom_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53c7c52e21c3d093bc962e4119f1b51b176e93ba7c253b96b025e4fc15bb0d6f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_garlic_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e714a8327682e40032c389beb02ea7e7b47c49592be6b506c48a2fd7e70e10e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_gourd_pieces", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eee831f9c7cd38c0f2ba884530d4652d9f371e5525ed33e0f1d32537c516e187"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_grain_with_onions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "052dadd634f791a17589efcf95a4c003ee62a9010b18f7a507585567e8a1b2d4"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_parsnip_batons", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1949b91d1c1f040f821b08982f498cc415fa4d69a9cf81ed9ec9954f22a5d61b"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_spleen", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8889b64946aef7cb2d817fb6a04c337fb8005810d3814a8fede83b3fd05906db"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bone_marrow", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1bc7e526661826d335017a41956fe2a0c23704125388a08af0d7684b6840502f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_braided_fish_fillet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67a43a605ff9a79b485c7e2b8e32974123a0fbd999d6ac48bfc7dfaaca1bd795"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_brown_barley_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1bf54966a48b38811ebfb2cb28fb6282b0b6051af70009e0dbbe1c9284eb2a23"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cabbage_herb_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b52dd0671356cd32bcecc95867815e42da22acec84caff4a36cdabc4c4e17eb4"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cacao_sugar_custard", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f37cba248b522737102011d3b76230ef9f03f9f53713c66d929b574512f57026"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_charred_summer_squash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0248d9a2e85b7c0bebfa060bff38f96a9f6537ba3249094b9921a120ef801c6"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cheese_and_onion_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "74ed7a5ddf0ecaea73a1d83f597fe6789b102c9c831c34b6978c59482390d3ea"),
	];
}
