// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch028() =>
	[
		new("item", "antiquity_liquid_leather_belt_oil_flask", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e48aecfc1f7e2bbf83fc4f5d1a26a4030e12097cc9d685621f629ee828670ba5"),
		new("item", "antiquity_liquid_plain_pouring_jug", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee10ca8a7a3e10226ad3ad25698e45144b5ba6147230c7569afd3e17da4de5fa"),
		new("item", "antiquity_lockable_planked_courtyard_gate", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "86f84e758aa674772d224cc1ebe9e049fa95db614d20bff0fd8a50fd93e5d71f"),
		new("item", "antiquity_low_dining_couch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2fdc41964a0fd1d38d2217f9058f0ffc10b6cd7d143a0dcdd63bac3d24c25d8"),
		new("item", "antiquity_pack_saddle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a775a14c24963d23ddedbd9fc9c8b0fa35ed90e73b4c46a3a73540f2af6e0150"),
		new("item", "antiquity_papyrus_burnishing_shell", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "184eb499a1ad08d22a7a424aebb18208c60f872f56e03b19b7acdfbb58d73168"),
		new("item", "antiquity_papyrus_sandals", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a270ab0044b9a53556266656162c33fb3d1bd28a1ea3e6d618df06a2dfda2f91"),
		new("item", "antiquity_papyrus_scroll_tie", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4db971ff206a8f1dec497aab2bddd9507b53ae5b4626b2bf9638f049a753baaf"),
		new("item", "antiquity_patterned_wool_carpet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "638bb04674af40c366ac4e7088ee98ff7ccd2980a897c4b693afdaa5decb4ab8"),
		new("item", "antiquity_persian_iron_helmet_with_aventail", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a1b4eab6f0b1805ddb4be2d89e0a3bb1ec1d36976255471f74110879f90c082"),
		new("item", "antiquity_persian_lamellar_greaves", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a29e3a3149a384f0e3828f2824a572b1b5608f7bf2ca668e862bae5f34e0a6e7"),
		new("item", "antiquity_pigment_shell", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a538d41649b0d35586bd3a272f81f9177d703e061c0d42b181a0fa41dec4db02"),
		new("item", "antiquity_plain_leather_sandals", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba7d625d1b37aa8e376c52f6d62398c21c5fe288787526532e0c4b1e9844741e"),
		new("item", "antiquity_roman_fine_padded_subarmalis", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "05ea5a3b4e47c9f9e94414c1cbda7aa4ea38d13b091b8610f0d6b1db27ea816f"),
		new("item", "antiquity_roman_lorica_squamata_gilded_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "014ee455a83e6ecd5ceeb22682c748e00953ea750ff113f896873ee1f3767362"),
		new("item", "antiquity_roman_segmented_iron_armguard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c56caed92a701c26132e278a77b39b63ebd135d406e684390d1e53ece83e9fb"),
		new("item", "antiquity_round_three_legged_table", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "75bad980a46e98a9373e633d24ed1981360487acf1a9b89e49538632fc3af65b"),
		new("item", "antiquity_sarmatian_mail_aventail_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "039a11a400d30ba3421581924f624b69f0c1ff20a2cec9883075eaafa9da7c2a"),
		new("item", "antiquity_shield_germanic_painted_round_shield", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "60d234ec8e49bd4d2013178bf62f1bf6495d37f22a2ce465f4f9fbf6773a0aca"),
		new("item", "antiquity_shield_hellenic_aspis", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "67ea24e52fafb34d760724d5aedf6ed073f1f8c01c28f56be5a4e2a6825d78af"),
		new("item", "antiquity_simple_papyrus_scroll", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "807d162690ae2298fa05ac82a9810a07b0fefc3611f75ca008c6f65d505836ba"),
		new("item", "antiquity_sistrum", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b2847f431b7c5be91b2e2ad1a63c87b5b90471d7934ba65d38cdcae75060dac"),
		new("item", "antiquity_stacked_sample_case", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "527cb0801628b619a2caf81a6d37608a9bf51736aab066631fce3b51045b91f9"),
		new("item", "antiquity_tableware_germanic_horn_beaker", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9eef7cf0af458b745e41fc1bbf22768f5123f6fedbf3999ef549876f80333bf"),
		new("item", "antiquity_tableware_italic_lanx_platter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "197e003d910504410e9c6dac93cc12d59b3db4a42b083b7bc365b9944d4258d3"),
		new("item", "antiquity_tableware_nubian_black_burnished_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "af1218587ae0b4e2e7addb043dc64a1f52b6d6dab93ba5059cd3e6ac1957acc8"),
		new("item", "antiquity_tableware_nubian_redware_plate", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a17b2160bacf1d5eacd17a21e837be67aafd60113f5936be61263577c63f17bb"),
		new("item", "antiquity_tableware_persian_silver_footed_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d363e033451709bcda06211b8f398fa872a225b879da33efe6049c13831d3d5c"),
		new("item", "antiquity_tableware_punic_red_slip_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "174659dc2d4332bad05de09b7d481fc8ad2d96e9a7b122a961d05a687aa39989"),
		new("item", "antiquity_tall_hooped_trade_barrel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffb20a506018b5674a411c86236e9b19b2621e18df6fb4d34b08f729fe3f8519"),
		new("item", "antiquity_tall_lamp_stand", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d486ccf6b44d1698851d9e2b66cdd9d8ce6b4b53b79b38e55f53e3abe9c57acd"),
		new("item", "antiquity_temple_decree_board", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0006db5a383fff00074f4b300af64b3ac42928f0c3c2aa14ddf7118b76baece2"),
		new("item", "antiquity_temple_purification_basin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab526f8bb004182d0e8dee904106335893509c451e9031462c7a347cdbace9da"),
		new("item", "antiquity_vinegar_wound_cloth", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d38fb8fe0f22e6a39f95ffd2368440733bf70147717e7300db73015c4d000ee8"),
		new("item", "antiquity_wax_spatula", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a66522d189f938a10ff2f5d7f14c4baa138936065e78a4d1ef6c960836e65699"),
		new("item", "antiquity_weapon_anatolian_crescent_axe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b713deda67a4d7f822fc787f8fa0a7b9c4a025c7743a78652feab57de1bbdcc"),
		new("item", "antiquity_weapon_hellenic_xiphos", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "88b64b8b4d02c59f721477a732a77d3945db16a1d3d768dd1ef141778909d927"),
		new("item", "antiquity_wooden_barred_wicket", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "73c2318d2fba1f30f360fd48f0ebf1518cd467e8e1eabcb6cfe891791f3d5c17"),
		new("item", "antiquity_wool_hand_wraps", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "05b4f275ffe5ba2621263d8484d77de5472d9695ba9169408d3703ee841a7fa0"),
		new("item", "earlymodern_andean_military_service_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "70a1a5ff0f7af4b1526bb7a16395902d4221339597d52bf5874752ebb437dd2d"),
	];
}
