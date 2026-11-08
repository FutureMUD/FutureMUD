// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch053() =>
	[
		new("item", "preindustrial_food_boiled_mushroom_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "643a910a87029cf1ea74aa5488f651906acf3b0878a4a907b43eda8bb2a63bd2"),
		new("item", "preindustrial_food_bread_crumb_dumpling", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd8a84f2180d3f50f2ba035982593f0904531841ad7294947d0f6eabaf0bb48a"),
		new("item", "preindustrial_food_broth_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3bd7445d0d3364b93e2ca498323f21ec39188942e930833500c91846711da43"),
		new("item", "preindustrial_food_brown_rice_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2528f3352a660f1bce9e379e91db9e4af0bd257988d6ec384592474da209a45"),
		new("item", "preindustrial_food_butchers_trimmings_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ca7fe799931740aa648a4473ad0c4ec20ff2c3d982d3bb2a9e3cedf9e13bef3"),
		new("item", "preindustrial_food_cabbage_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfbcd0b5706783472fd28c58b42943339bf5a20b53e2eefb504d3a6529241894"),
		new("item", "preindustrial_food_candied_flower_sweet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c350e6be5b300a778e52d145591dfdce73262e5440f4efd1b1602f7f6c75fe3"),
		new("item", "preindustrial_food_caravan_date_meat_block", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5220804b1245eb9fcc380078353a80873bd12ebb25a7aa97c6de5d344c178384"),
		new("item", "preindustrial_food_charred_cabbage_quarters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7292ea2856a5773525db68acdc416ca4c382d1a4507d5479cc39bfb32df243fc"),
		new("item", "preindustrial_food_charred_fish_scraps", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f55605269b73383de44b0a43e65596ff3ed96b57e11c2cebe8082838219edea7"),
		new("item", "preindustrial_food_charred_parsnip_roots", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18a07d5456e19653e11d3eeb800f74abfd76da805706a7fe21567b1558e06a62"),
		new("item", "preindustrial_food_cheese_and_fruit_plate", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3aeb7443e0dded049763ca88bcf44b16019facbde3da4c0b649736801d3d503f"),
		new("item", "preindustrial_food_chicken_and_cabbage_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "99399d2f6e2e59420e639f0d9d8b73714eb400d37a9097a592a7b54570c077a9"),
		new("item", "preindustrial_food_chicken_broth_meal", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "416366897377c947b4b3edbfc11d0da06bafe8a50f3952951156aa4f78c4efdd"),
		new("item", "preindustrial_food_chicken_leek_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd1160e6435783cb60ca75c97d0c2eb24b9b5c7554d8422c8718d56e81eaf9fb"),
		new("item", "preindustrial_food_chickpea_greens_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2d3fc1142fd43883a80e103c3da3a845b8674ff9d69a71d0a018c8edd475d5f"),
		new("item", "preindustrial_food_chopped_egg_salad", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "410fb1a2a7c429d094c1006807cb95a5b026eacfe29772d8657734b2d353f3bf"),
		new("item", "preindustrial_food_cream_soaked_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c431fd0684e83fa15e162b26f56da802e2c754a1568a0432c714417b0b6cca63"),
		new("item", "preindustrial_food_crisp_meat_skin", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "204abfa914b360f0623db40a31a82f8e13e587421061bb9fce75b2b728f8cd28"),
		new("item", "preindustrial_food_crumb_baked_oysters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "da524094ee539be2d5ed8e56527642f16bbe9490300bd419d8e802b9b750789d"),
		new("item", "preindustrial_food_date_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d99489c98908d428b66a9bbf8b5b3dbcbde2950df0dfc5dfba6d283afec49c2"),
		new("item", "preindustrial_food_dried_grape_must_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a726407d6bed57cb7f4344b592a83d50c2e39b194323301410a04b7f59b293a"),
		new("item", "preindustrial_food_dried_leafy_green_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6e2cbea4df9b7f923237261d2c4158d713ef60d03462e09da6ed1363707097f"),
		new("item", "preindustrial_food_dried_millet_noodle_bundle", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dd9496b3079dc7a002d81bb97e9c0102aed473b5d026a49f1ca3b7d176cba5a"),
		new("item", "preindustrial_food_dried_okra_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "73bfdaf68871c63702edfaa3e1bc9431a2acd5c7dcac1edc797c5f35f9c22c3e"),
		new("item", "preindustrial_food_dried_plum_leather", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e7a6461e2ee14adfdebc6a8ac34dfab561ff6ca8c970048d5912cf4a56ccbeb"),
		new("item", "preindustrial_food_dripping_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca7da2fad446b1e8ff428d65268862525b29a0ecfb35a91665439c2c4fa3f7ba"),
		new("item", "preindustrial_food_dripping_rye_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "97ef0c2bf2cf0c61d15e58288ef8c73a9ac18e11c325be216aa8b1ce83346e80"),
		new("item", "preindustrial_food_duck_and_barley_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7bb2e303b477b3b20ac58ca7b7876a0d341ed67be997358843b798bb2fff706b"),
		new("item", "preindustrial_food_eggs_baked_in_cream", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d834009f2a64b80c06138fa6c4cc041b9d76faacf1bcea9dbf4ae0bdf344cc8f"),
		new("item", "preindustrial_food_famine_husk_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2151ba7863479621e1a41372e4677f1916a12385079274a8a785070cdd6363f"),
		new("item", "preindustrial_food_fat_enriched_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "592ebd977a5a29aa234f765bc29b413981055c88abe10c5a1265c1ed5bd0e71a"),
		new("item", "preindustrial_food_fat_sealed_meat_joint", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "827d4d89a06ca3538fc66b8def9b605625e61b3a09c04034483a7d3fe0f22cef"),
		new("item", "preindustrial_food_fat_sealed_smoked_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81d95c283ca75ce6325188877502254227bae645aa75e56f2bbca0234f4efd25"),
		new("item", "preindustrial_food_fish_and_greens_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "808f7c4102825d47a774fd7d677973f3400f2f4efd2954697007f1181fbc09a4"),
		new("item", "preindustrial_food_fish_and_rice_packet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb8f261d3eb782c8ba098b3c8b64a27ef34b1f11e356c9c95b5faad732dbf035"),
		new("item", "preindustrial_food_fowl_broth_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cea90c002277ebef2cc82dad80dc5ca5ebed7e37231d278a9353c5fe2fd9ab71"),
		new("item", "preindustrial_food_fowl_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0784758e8cc88e74f86f7e6965cf6ac78839905ec1dacee2458355e58d848b7"),
		new("item", "preindustrial_food_fresh_cheese_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01250eaf2e40c8bdcaf0ed50ae6907a384594a053cc96608c90a5b5377aad9a1"),
		new("item", "preindustrial_food_fried_fish_tails", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a776a5a3a175f591453f9d1c0bf9aea3c1024e048a50cb8f9ec5805d8557efab"),
	];
}
