// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch055() =>
	[
		new("item", "preindustrial_food_pie_crust_blank", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "619ebbb1fae3823f91d0ec7807d59be89b6402bb4a819644ff00e87f4ee2c3f8"),
		new("item", "preindustrial_food_pigeon_peas_with_roots", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "324faf8752b48a05f234928527a3d6026f69b47d1028aafa8f94ebdcaa9fcf90"),
		new("item", "preindustrial_food_plain_chickpea_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1075d40cdc621e6cb339fd061b483205340f40f17b4f306384c4597ffd207281"),
		new("item", "preindustrial_food_plain_clotted_cream", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "46e312c8e65223dc889a0873fd721f2ad7ce920ef964f2c0594acc5ba3b2c28d"),
		new("item", "preindustrial_food_plain_salted_pork", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "13ca9c8b54b3ef64f623c4eb6a208743549775723738e6ac13e391e5cbeab45d"),
		new("item", "preindustrial_food_plain_wheat_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7104479a6ff3e84e59bbc326bb4a3fd5fb546b0bf94fd35bd2237800876b5968"),
		new("item", "preindustrial_food_plum_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3044fcb22eff5e3e0d758bfce7cfacbb14c45f13bdbecb1a61ae1ad01e4dd876"),
		new("item", "preindustrial_food_pork_garlic_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a65b074a33258cf77844d9b165ebf042d8d44838434dcbf764d748a7fec776c1"),
		new("item", "preindustrial_food_potted_game", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6debadf0cceb6c6dc319750f0e6c93490dfe615c5611c4d76b4267e0f3910521"),
		new("item", "preindustrial_food_potted_pork", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfa15d20cca3dd86e60dfe4ebd9885218e8d18b345ddade7c0558e21bfb15259"),
		new("item", "preindustrial_food_pressed_green_cheese_blank", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffae401aa24fe15422df6e369aff97906d1ea96c19aebc338237d95ddc969620"),
		new("item", "preindustrial_food_ring_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3edc81d1e9b35e2bccb8e6321b51a81c2f88d1a70ac731c7bb16c1c1db56c922"),
		new("item", "preindustrial_food_roast_kidneys", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "817655557c0686ea3febf1634c17c89e80df852dc6fadb88ba4bf9ecdee07d66"),
		new("item", "preindustrial_food_roasted_field_mushrooms", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b3a3ad88260abc3017badc2f6ca15cfc636c23cec968a147a8cf8d9e704c567"),
		new("item", "preindustrial_food_roasted_fish_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "420aa1fb91d19a2db22a5beadc740eb6eb28c009f3cbead7db1839343a8f56f8"),
		new("item", "preindustrial_food_rosewater_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "babfb335a15cf5312409f6782a13b28d8316810c1bd8175d29877699f323ed6d"),
		new("item", "preindustrial_food_rye_greens_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f5e96595cf0b0daa979999c7280a1a8d2a391b649dc9f9a46c65c0eea30e942"),
		new("item", "preindustrial_food_saffron_braised_lamb", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3095f817fa445fb6125689de2305407f192db9d3fa8cc8f8abc6f1b278e10bf"),
		new("item", "preindustrial_food_saffron_chicken", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff10a658f254f250fd8fbdb293de4f8194f2939d7b8f4a855985a220e0d7736c"),
		new("item", "preindustrial_food_salted_asparagus_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "223c8cd651b2f90a34f7f55f13a7290c022689113783ee7c0ab1a0a31d695669"),
		new("item", "preindustrial_food_salted_broad_bean_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a439f9d6f3f05f924ad257eb9453178c8faa42cb24b46f27a9c5316de121f6c"),
		new("item", "preindustrial_food_salted_cabbage_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d292b28f0639ef7fd99b42fd18ea78d71293cccefdbd55761b2f259a2ef18d3"),
		new("item", "preindustrial_food_salted_fish_ration_cubes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d3d859479f4884cda257703dea985125d25d65b7d3f5b37fc068da9b6c9b2f3"),
		new("item", "preindustrial_food_salted_roasted_nuts", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dde856bbec636cad7e0a8612d0f3bd92b3205f56583577e0f242d9b609ad7a89"),
		new("item", "preindustrial_food_savoury_spice_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "02954cdba6571fe0b6ea83c6d01c396aeb1d16579c8f1819df8da6b9edb9ff9c"),
		new("item", "preindustrial_food_savoury_sticky_rice_parcel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "70612841383b75e2c277ea95bf2332ab31efdf382cac4bf40436b091a5988ead"),
		new("item", "preindustrial_food_shellfish_garlic_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a226dcfdb99b4f5ba4d1e60618cd982576d380cc60f0ca7d41b2a6b672351221"),
		new("item", "preindustrial_food_skin_scrap_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "806c88ad7a0caec6c18f387ffbc0ac09ac387bdd9acc269f382e87d61ded37c0"),
		new("item", "preindustrial_food_smoke_cured_sausage_links", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "20976d6dabc02d1ac7fba28da5732d09614543c9c352d8bae990c59bd77a0565"),
		new("item", "preindustrial_food_smoked_goat_strips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc474dc0bd0ec7ca6d6a60920c3f40201e145adb39941ed700d2fe4d7f46f482"),
		new("item", "preindustrial_food_smoked_hard_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e24a37f8188bd87bb2d61d4b68815ec1f3182a775a784968b57e810dc9f9bacf"),
		new("item", "preindustrial_food_smoked_mussel_skewer", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbb46cfb4970a5ea18795933005d1236088d8093173b9aa12a464ae8869d6d8c"),
		new("item", "preindustrial_food_sorghum_and_onion_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "24d4d3eb679274df9a4dce330386bf89c0ff1a0f46987eeb145bd4e1d9918313"),
		new("item", "preindustrial_food_sour_cured_fish_chunks", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "849005a1c64fc8ceef4f669b93733a040a7783ed1229a9b5870012458e25944c"),
		new("item", "preindustrial_food_sour_dried_grain_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3daf2d503f55bccc194130fe178e1f855865555c93907d071839039356239555"),
		new("item", "preindustrial_food_sour_vegetable_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "962e5c916acab684feb7b05150a90144f5ad918b6e174960cf51f2edd46628cb"),
		new("item", "preindustrial_food_spiced_pulse_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "35e94c89ff3743d24f2c47e200009052dd935333d5f49cc8c64da74731251e2b"),
		new("item", "preindustrial_food_sprouted_grain_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c7f013632bb86108bc7f42035ab2a2c0bf26760107275ad96aef9d6a9c17a8b"),
		new("item", "preindustrial_food_stale_bread_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7142f4a18001c804a3859b1c14d9816b2ae67ee43531e0fcbc36591ea24ffe68"),
		new("item", "preindustrial_food_steamed_grain_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b5e0f6fefe2c34c654988652e00d03b259cfe803edec1dc5108146a0fda439b"),
	];
}
