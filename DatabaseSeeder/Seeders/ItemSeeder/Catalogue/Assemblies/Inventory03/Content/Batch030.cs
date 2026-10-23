// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch030() =>
	[
		new("item", "earlymodern_door_wa_secure_door_441", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fd3f518dd757c92add324d64040ab9ca2aa55722dcaf3e4c3ce7b02759f9bac"),
		new("item", "earlymodern_drug_daffy_s_elixir_bottle", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dfde4aa97c2a746a4083fc3134d02b5168cda7b51f66d607350dccf08fa91ee"),
		new("item", "earlymodern_dutch_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f1d706e6cbae956121974ca66999f192a909002d2b084e06fd43a8863131fd0"),
		new("item", "earlymodern_edo_clothing_narrow_work_apron", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c41214676faf46ca96dafb4cbadf10615e167b523ea2e5389e28d7f476e4eebc"),
		new("item", "earlymodern_edo_military_officer_field_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a1dcdb7023ce0ee121a01f002d94d807fb9decbef6ac24ed01b78da44bfa983"),
		new("item", "earlymodern_edo_noble_female_formal_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01012e46df4a20a5f525edbefec980a436ad762e5bf4d8e6ffb881b06b2159a8"),
		new("item", "earlymodern_edo_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ee72677e8b4f646762a7fa0a01016370edb2c836891a9b859a1ef91e23cce4c"),
		new("item", "earlymodern_ethiopian_military_officer_cape", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef29de3c1a5ae186d2b0b4c357ed57d339d6340c294c6203cbb009c5bc58e0be"),
		new("item", "earlymodern_ethiopian_military_service_headcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b8b6b14a9f86f1ff0cd09abc8ad9108f61cafe9ecf05058343c3e2c728f6d10"),
		new("item", "earlymodern_food_cape_pickled_fish_curry", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d258709937020945d346b53b5e833f6bdea8ff329b8ef9a873642fd7f65434f"),
		new("item", "earlymodern_food_caribbean_cassava_bammy", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0bf5229c84a25330d0b2c71815e178b0dbe8870d61d92b8580e818df44154841"),
		new("item", "earlymodern_food_french_fruit_sorbet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e9611f85f9be246895efe1d76c63cb5f84b82b53bf15d79ecaeb4bedb021c12"),
		new("item", "earlymodern_food_german_apple_strudel", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4cf6acd875a884b48ff6a541f1bc49ba02e286385475135767a6d93d280d7ef4"),
		new("item", "earlymodern_food_german_spiced_chocolate_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "29f378e9eb9a54115ba063ab179e152f38a0a73d3404d091ca65c8689cdb9dce"),
		new("item", "earlymodern_food_gold_coast_smoked_fish_maize", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "20028df21c5b78875ca55545985b0c556156665d0348c838734ee99928f2e9c7"),
		new("item", "earlymodern_food_joseon_royal_fruit_punch_cake", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1eb8aad74ac857b1a4ca204906529426bfa55f07618fac1dd73d59a8bcd3f263"),
		new("item", "earlymodern_food_joseon_tteokguk_ritual_soup", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b506da76a370b9fe0a0b4a2833de7ba8cf2bc8b08ea80341fd30537cdcbe35cb"),
		new("item", "earlymodern_food_mughal_murgh_musallam", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "09ca3c4c3f8e3b1b2beb31130fb3e9530cb42364c412852b3297729e6d975b23"),
		new("item", "earlymodern_food_mughal_raw_kebab_marinade", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a7fbad066b65670b0d4c1d0e17e7fa074e24c651a1315f05c6da04a7caeaa0b"),
		new("item", "earlymodern_food_naval_salmagundi_platter", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e493b12a5bbc54cae70a9a1d5c76b31b057f19330ed373c4a353c5f82d7dfc8"),
		new("item", "earlymodern_food_ottoman_green_pepper_dolma", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf5e41128161d5b1bb5a90841ba3fe5437c82b3e79afdab04f8a5a6bcf60d3e4"),
		new("item", "earlymodern_food_qing_eight_treasure_duck", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f38f1861d59cadc4546991eb0a496b3aba43c1f01dba3954d743f4657462128f"),
		new("item", "earlymodern_food_safavid_rose_nut_nougat", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b2721f15daa29d3ac97d024ac30767443ce9a6204455dd9f26943bf22c472ee"),
		new("item", "earlymodern_food_safavid_tea_almond_biscuit", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "166eb5e804a02d4bee7bee035a133777356e345c87a6897307a470d083b73758"),
		new("item", "earlymodern_food_tea_smoking_cured_duck", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3ba67ad87d001571c7be9eb8bb838bc7dd002d87f07e818732e3e1739c28e56"),
		new("item", "earlymodern_frontier_military_fur_trimmed_pelisse", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a57ffae06282019ef5f6028f0d4d6d1ae91b906b4d1e423e5d336dd7939d26ff"),
		new("item", "earlymodern_furniture_and_archive_wall_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b2c4552a198b8caa0087941bdfc19f4fce3208e6ca25f7af8d708ad71ae8612"),
		new("item", "earlymodern_furniture_car_archive_wall_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "68e0a277bb70b7c6a0c8c5cc73c1e2ec8401a2e680d6c93035aee9de65d6a026"),
		new("item", "earlymodern_furniture_car_display_plinth_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd86b3b69fe2d6231b249a36163fe4564447c201b1ec71e3a3b3ddc3e4012dd7"),
		new("item", "earlymodern_furniture_cna_dining_table_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "82366653193fe72e796a8d2b986536c8d1764071fbfcc3f513107d30a0b7fd81"),
		new("item", "earlymodern_furniture_cna_drawer_chest_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4298577e540d0a5efb735b5d8447847af81a1831d2b5791ff4586ed7db4ddc35"),
		new("item", "earlymodern_furniture_cna_screen_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5a0969785c172277b55b35a8aab9bd5476db406b6857645d452ab6a9bb5306a"),
		new("item", "earlymodern_furniture_dut_display_cabinet_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d1bf064fb057c56c5d4f7c8483b9a978be4838928444b4fcfa452108d47b3b7"),
		new("item", "earlymodern_furniture_dut_linen_press_02", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "40fd282515275439c0475a35349e71db3179dc8d449856828d0c027c2a631beb"),
		new("item", "earlymodern_furniture_dut_screen_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "038550811d803695bb7e7572fb81271155e60bd16c61e093f765ff515faf3f70"),
		new("item", "earlymodern_furniture_ibe_counting_desk_06", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5986597f4cff6f2f17b12e28d8ad3a199cce98fb6b41a2c646a2145a05e2d3a0"),
		new("item", "earlymodern_furniture_ibe_side_table_10", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bffa9de41ffa9044fd1e36ad5ccd23ff328cfb585bde3309742f900128005a6"),
		new("item", "earlymodern_furniture_ina_bench_03", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d03484c85f1f8c85adb59b3a32f2e7a177a055a6f86a08c1d74920e797e75ec"),
		new("item", "earlymodern_furniture_ita_counting_desk_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "21cbc9c50ce3115ac9c6ffa191eabc078ae49a6dbc89f21810ad67877c1299f6"),
		new("item", "earlymodern_furniture_jos_armoire_04", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "014480e1ef356a22494167492355cd7a4b34e76ef8ad113ef833692157227572"),
	];
}
