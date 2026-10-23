// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Wax_Tablet_Surface", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "572a246a28e3824f0b514a82160e02b3036aad3e2e23ae826ccd2ceba481f3b4"),
		new("component", "PreparedFood_Antiquity_LuxuryBread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "890c2e7cc19ece29b487e63fb4345166a0f3be08fa34b6cd8bee5b7f1bb21a16"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_andean_potato_cheese_pie", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b3031dc478375b5700e23c3c2bb7cb6774f3e50d80f1e4dc6f9d0aa2c8d430a"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_brazilian_cassava_fish_pirao", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "dac56e97aa25c51b8f4b9cee8e018e7586d7dda0ebcf9ec33b4b661637c26a18"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_dutch_pickled_herring_onion_plate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3a130c83bd09cc6e65427db53e863f866f5502b2ef070944d4fa737f12bc7be"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_chocolate_ice_cream", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c761bd49290bce9c61459109a569f708573deaceb763cd8b9d279d0de51383a0"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_choux_cream_puffs", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d6671c43f600e6724e2a3b924f862c42f7e9bd709d012240ea67be66c72f8cc"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_german_coffee_house_kugelhopf", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "69eae0af01ac9921fbfd538a4b1024a291971e692a08b5d49ae6df580a8611f0"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_iberian_pepper_stuffed_cod", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1d6dfbfa3cbb408a8733eea8bd7850a0596811effe4a9f4c1cbb14f57539dae"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_company_dinner_pilaf", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "caa5d29363a932d84ad40f3c1d6510ec11d57fa28f25277b59b4aa7bbf9e337c"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_naval_sea_pie_layers", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d46f962031d3aafa1f0a1b8ac8d1441c9483cb497ba50aabff8cc350e5ed80fc"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_bird_nest_like_soup", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5da86e26e3c4293d9190e8dc0c875eae79a468d1c03467c52e7d1b18c114af7"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_lions_head_meatballs", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b750f6d328dc8caea2607ef7fed1b143c9a7fed6868e2ab97ef959d0098dd6a7"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_senegambian_chile_fish_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e3f6bdf62264e5dd2d23be01a5fa7c813634a58c584b40f21f9e0ba4d482877"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_swahili_portuguese_custard_pastry", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "60f95bc459197d428c1c1080d0f746f900694a4b0caaea7ca6b3851783243ba7"),
		new("component", "PreparedFood_Catalogue_medieval_food_fermented_steamed_idli_cakes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "187f38442e3bd1082f3601a227d5b2c0425e391d354120159846f51a2d14a08f"),
		new("component", "PreparedFood_Catalogue_medieval_food_japanese_miso_glazed_tofu", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc56b42015c26bdf9e906c794ff13253ad4b35c21236067b97c1e76e4383dc4f"),
		new("component", "PreparedFood_Catalogue_medieval_food_japanese_sweet_bean_manju", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "716c252fd14285460e3f6b5589fb1fb101fdbf40cd6f43864b65998c74953907"),
		new("component", "PreparedFood_Catalogue_medieval_food_lenten_fish_coffin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b744d8d4554778e79b71bd733e7f494656ba6f9624b70fdd1372d3af49e3b76c"),
		new("component", "PreparedFood_Catalogue_medieval_food_milk_reduced_peda_tablets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2155314dd56b8d7600f496c8aa2b23f13dfa2e97ae31a9947d831fc9e58def79"),
		new("component", "PreparedFood_Catalogue_medieval_food_norman_herb_stuffed_piglet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a952a682c4675509c287dd8597e770d131d086cebcd401a7e31059ce7c0031cc"),
		new("component", "PreparedFood_Catalogue_medieval_food_pork_fruit_powdered_pie", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dc65386abb79e3c108b9452d9edad4d520bfc9ad9d1192eb7d866258372f51e"),
		new("component", "PreparedFood_Catalogue_medieval_food_samurai_miso_rice_ball", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "85ee692835b22b9bd418ea11541a44507464465b706db7d19b1e4f0957c5fbaf"),
		new("component", "PreparedFood_Catalogue_medieval_food_sweet_fruit_tansy", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c5f8e499ccad12c1b250262b2d1230e4d227a65b4813474e623494953352bbe"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_ash_preserved_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfd4bb2e2e5f54b9cb174bc65d1516064703fd281f1d09b1bb937711bbd4e942"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_asparagus_with_peas", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d7d547e9d890180d5bc391b5a24016a349cc47ccf342363eb501ba796052cee"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_garlic_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "85b3bce9df0f90fe7b832da3f68b990bdc11681018a5792ff2fe674183d70da8"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ffc68c5b7508b8cc5b78870a523e0d54fad34ddc9c2cd888cee871c6d6e7dc8"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beef_herb_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "112884030abec524ba77c7d069586451dbecb50b0c3b67b44240b88581316cb5"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_black_rice_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c791181e8e0b99ee7b9c3e58542bc4b68e76b144ddcf162881c994cfcd7a021"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_asparagus_spears", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6805cc5b075ec0c55e817268f8c77c0d3151f697a09c43924f9e72d60dc9556e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_aubergine", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c69866d42747252ddffa29f1a99453c788ac129d21d42243447f9c9c81040340"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_barley_dough_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b56b48bae096ce5b918302205e11843a54d29d00b31d12aa71ef0abcf6c67023"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_millet_bread_ball", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8558ed69d3de0efdfbbcff99e9801154e9a012c66f43ea40323a3f47a5f08225"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_braised_young_gourds", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7899ff8ed0bf7945f13de2fbab85bce386d9865e4930bbbffc47ffecbfdfa044"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_broad_bean_herb_patties", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "206ba6c02db1f5845b5b3616265613ad7ea126b5e24bc0782250af2ef674c1a0"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_brown_lentil_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "48c0349c82058d0cc8f1a365af8216c57f26683dc93aa016d96a4815f88e4c6f"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cabbage_and_pea_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "53db319a5b3e0cd8b2400cfc06c32750bb80c401de27ff750814c7a1dc53cf74"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_cabbage_heart_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4ad31e8a427420a43ed3c98e81c4987b6ac7850603d5282f1ad046620af05fd"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_caraway_rye_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b462ca400b340cbfd4c1cc7e9d806161ae64d95c51a92ea8811356933691f66"),
	];
}
