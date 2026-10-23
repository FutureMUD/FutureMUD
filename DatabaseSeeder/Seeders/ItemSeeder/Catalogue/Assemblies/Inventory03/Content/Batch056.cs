// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch056() =>
	[
		new("item", "preindustrial_food_steamed_mussel_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8fe805032d4927e9145ed834e00809beccd42f8ada2dc2906ee134b6840c026"),
		new("item", "preindustrial_food_stuffed_bread_dough_blank", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7aea1b33898a754c9244f0adc8859b922f8abd672a3b56fc3f2783c10848e2c"),
		new("item", "preindustrial_food_stuffed_meat_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8207966fc1a979f66a5be84b487ec380ed96501072830e36a59da7313dafcb96"),
		new("item", "preindustrial_food_stuffed_meat_roll", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fd7366ec9337c16f0e8970e89819696427988da0e4040e22af3c2e2f9a96a57"),
		new("item", "preindustrial_food_sugar_custard_tart", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d2f13904b87d14223a839c2c974ce34315cf5b9c1a05acbd5a6d14421f34bb8"),
		new("item", "preindustrial_food_sugar_feast_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4088e5577fb8e7062a63fc1594ac955139c4c5b8ef7c64c60f2c23c2fb080954"),
		new("item", "preindustrial_food_sweet_dumpling_filling", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d40fa617779f4ba91c46936378707e809939bc4c70c8f1eae938f231d957a2dc"),
		new("item", "preindustrial_food_tamarind_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "421c4110865608110901705467cbb78b953772b397e739da9a6955f6f6cb68bb"),
		new("item", "preindustrial_food_thick_fish_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "79c6b74db03a2d0af4d5d7b7a64579cc0e89dcba9c26cd979f50ba58d2c2f51c"),
		new("item", "preindustrial_food_tied_raw_meat_roll", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "398fc13557633d6623595724d8f2b494f38c5d109599bf2df3a41b4cb80dcc3b"),
		new("item", "preindustrial_food_toasted_pulse_crumble", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "efe7efece35bdf1e9b6ed9f643efed6375dba29d55330aa719b74e25b0fe32d3"),
		new("item", "preindustrial_food_turnip_greens_with_onions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2505fd25afb92b2f5d7ce145c92289c2d8ecc36fb78e4cf904d7fdc6b5c55ea"),
		new("item", "preindustrial_food_turnips_with_herbs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6fe5b989351a1fcf5d7e75403b70634f4da128e1030ac31befd2ec4f5cb0f05"),
		new("item", "preindustrial_food_vinegar_mushroom_salad", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f66a0be22f5be27614701f153d5af25e40e84e7442290446446ba999446c452"),
		new("item", "preindustrial_food_whey_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f76ccfcd6f4f33cdc100c34e8defd0b4a4292533977dd0531ccaaebb2692df1"),
		new("item", "preindustrial_food_white_bean_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "622c0296adb1880e45cb934ea7154d29ad43f6407292ec585ad37f2d9230f9c2"),
		new("item", "preindustrial_food_white_sieved_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "254a955313c6649677a40c98499149d8a26ae4b2eaa331214c6fa7efdbd3ebbb"),
		new("item", "preindustrial_food_yeasted_pocket_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "44daddfd10ac3176a3523aa1847692a16f6e067f5b41064013acfee8d93d4b6e"),
		new("item", "preindustrial_jewellery_bead_necklace_04", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca16d86f2b22d3ec134b8ef5335ee621a4c2dc02c71c35e582a668138d113fd6"),
		new("item", "preindustrial_jewellery_chain_necklace_05", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "252c04f33b86bedc739b450bb011527a6a2f84f54e98f32e4ee6ccdb038d17a4"),
		new("item", "preindustrial_jewellery_diadem_54", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4c4513165000e2c82af7bcebcc4d7b9ac92e9123f0bd51556fb557f45709761"),
		new("item", "preindustrial_jewellery_pin_51", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "83a62cd94a7cc8c189e3fc1ce99849c646b352913d624c474d30c1abc33736e1"),
		new("item", "preindustrial_jewellery_ring_01", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "102b3db6f352de8e4dd732fd7104971ca4c388a26fd0b1f110fa9a161305bcbd"),
		new("item", "preindustrial_jewellery_signet_ring_42", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3530065a7eb1ba61ea54700490c4780c15e421328e84cb12ab40cd3f00a55a8"),
		new("item", "preindustrial_leisure_dice_cup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea3586d0cedf3f42e1dbb8c8eef160e2e006adad94a1a110de3ccc91b19be905"),
		new("item", "preindustrial_military_support_bow_rack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b7a3c1dd71c1adc7fead3be707a38592013d2cd2f1b0711f16106253d19240f"),
		new("item", "preindustrial_military_support_bronze_fitted_sword_scabbard", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ffdfbcee691b8f8b45d9d79b31e2f0e586b6933fde45c3e0d94e6c87cc837a7"),
		new("item", "preindustrial_military_support_iron_armoury_rack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e53fd1f07756b9d72b4256111d0ba50d1a2d16d4b1954a168fcbd23e5497166"),
		new("item", "preindustrial_military_support_riding_weapon_belt", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc8c2dace70f85ec0ad27f5a071e1ca9808d0b978a317817d20a4a161c729cc0"),
		new("item", "preindustrial_military_support_rough_rawhide_knife_sheath", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cef855f4bb2014934099834a0971cdfd9819a5aeb359e2a5723a47041898404"),
		new("item", "preindustrial_military_support_simple_armour_stand", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1548958f468c012a889532ca1af67f4782913baaabd2c73b89419382c17491de"),
		new("item", "preindustrial_printing_chase", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "95ee6e1c43715982cbf14321ca6e5b684f11a85fc32b852ff89e846d1b4ea90c"),
		new("item", "preindustrial_time_water_clock", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a8235339182841abfe91e3f6f2016de82d0009ca4997927e2d516689e53a8d0"),
		new("item", "preindustrial_tool_charcoal_rake", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fc382e1e56e8f83b7db61a17894d3ab2f99702ad1cdab0ba8358da0c9119331"),
		new("item", "preindustrial_tool_potters_rib", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "79fedf1e4a187ad75137231c7fff80daa7a218c851d646392a56573527e279c7"),
		new("item", "preindustrial_tool_wood_clamp", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b5270a321c2b0137ec6ec903a84ed6cc150d5af367ab5c18d27ab61f9743fc4"),
		new("item", "preindustrial_trade_bowyer_sinew_box", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df4169e57f5b926115bbdaf4f724759b0c51e7d6b8d1e29fec9cb97a0797a005"),
		new("item", "preindustrial_trade_caravan_cargo_chest", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2b1a5e66680c27e5a1c4f37f07df3ecae1067e0e38f1efa0c533804ac89f620"),
		new("item", "preindustrial_trade_fair_takings_lockbox", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec6b3910f283e5cc0b0b7a0754a6ad842baf6db3bf272ce40a2e30f7d881a32b"),
		new("item", "preindustrial_trade_jute_meal_sack", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "42357fe85610b2a6b35fb09f2b84ad5979d0f9fc7a0379968de3f1e39f901dd1"),
	];
}
