// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch004() =>
	[
		new("component", "PreparedFood_Catalogue_preindustrial_food_seared_scallop_medallions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9411e1ee9bec85bb7eabd0b29bbb6e65acfc8a83b3ed43dd17979cdbc8307f9"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sesame_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ae3cf494b428664c641e79dbdb731304d406043a9f56c9d6f70e10bc8883429"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_shellfish_crumb_cakes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "35333a6e8193f16eef767935e08be0f36b98dad6146351bd67394cc04e7b5a8d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_shellfish_mushroom_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffaed1225a758dae9b400ebbeaaec9354c62d3b830473606bdda3e40f73bf6af"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_shellfish_vegetable_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6ccec4b6f94f2fbe7e8b98112e282054aa9a13108542558637158921ca1fb26"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sorghum_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b4b227e50bd20d2d9280a7eb6c4cddfd7fc26123ddf3a77ed7fe227a81e07f8"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sorghum_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eeed7e18055c24b6b5d9a08e25798453c94644b47392e59774858dab8c09727c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sour_cream_root_dish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c92e9f244d6a1341f4f53e344549c7ec1823af5eec3a8274b840ed557ab60b1"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sour_fresh_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e33e28fcf79c2366843343ca138f5e4caba91d260df3bb6105e926072b34ffa2"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sour_turnip_store", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "27746dbdd1a2ea8e3db3d8c3e40134443ee79b006778ae5dfc43df409fbf9bfa"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_spiced_nut_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd130d0849e49957837156d4d153301ee08b382d5c3e4e59b85cf1a0a7c73685"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_squash_blossom_parcels", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "701f5f7b1b6089c0bcb90f669805a989d82b7de0a2bb554936a35624043d2f95"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_steamed_bread_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d66eebc98018baf22d2c78ca5ff17031476a8752b11e247531838a51de80e77"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_stuffed_garden_leaves", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1e5ee65ae0295a6e1051f44ef75a2b52efd8396f2b387cfc0e9ba33a7328621"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_stuffed_onions_with_pulses", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca8a5f007e730622d546d36dcfc884ecb9fa1f39174e0bb2dba9a76c6e0e16a9"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_stuffed_squash_bowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a29098d54491e76c7cbf4008dbe0f83962e48c380897f99535769ff2dd1e7644"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sugar_dusted_fritters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8cb082cd10054aa66142c75712d52bb53db88a66b2503508bef036ce89439197"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sweet_cheese_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d8482f217d23c1ef80d70a627d4c8c72dbcdf61d92b69a71adb69803b7fa76d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_sweet_sticky_rice_parcel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4625e9f09c3bd93543966ac8f8373a9a45de2344a573c324ea7b70b9d24901d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_thick_cabbage_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cb4739358a75d93934cee55521904db1eb28df49eb06dd04dbecbca94c9a5f9"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_thick_carrot_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "38cf5f30773f88a8afc9246f24446f92bc7e02891478a90e88f435668125fc3c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_thick_chicken_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "77419585f5184655091d6bbbe81fd02cb03ff6a5581a7548d16d0a532128069c"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_thick_mutton_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eac9337c6ff633ea3e2eafb5e379dc11031604d2a9da1c20679e1b196e622d2b"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_twice_dried_barley_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c80e5bda202091c6b85eaf97d3b76e8c6ffec4557b632b83f634bcb6aea7b9fe"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_whipped_cream", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2598612195f191cb2eec569d1f79f4d726479ff8e5676d515fcac1e32e1ddd12"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_white_bean_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "99f1d6e7b6b6c785ddb720cb0cc313f1f01fd7f05e94b69d52965c8ee2c94051"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_wine_braised_mutton", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "71f11e97a9d656dc7dec5246107c9596999a78fcd9900fee1030edfef6eef541"),
		new("component", "PreparedFood_Catalogue_renaissance_food_andean_seaweed_tuber_packet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6eb41c94ed0b32987026a7a19d4916f10947646fd70d716ac0584f0325cfcc7"),
		new("component", "PreparedFood_Catalogue_renaissance_food_aztec_market_insect_seed_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ad6460a7bad7555de4751fea2f61bd66737543bedc338865a46979ecad1db63"),
		new("component", "PreparedFood_Catalogue_renaissance_food_french_herb_salad_illusion", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "57a77fa33e0aaae03c3ca40cb9b6a1ad892b4f11262b9ba675879369e53c5dad"),
		new("component", "PreparedFood_Catalogue_renaissance_food_iberian_cream_filled_fritters", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d380e3e2deb8961b19a9cd2e3f1f326472971e706a7316db1345773bc16c2f89"),
		new("component", "PreparedFood_Catalogue_renaissance_food_italian_rice_custard_torta", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5551c5dd37d28a4ccc07835c8aff018e15fec0974ddcdb3612f7f506642dae69"),
		new("component", "PreparedFood_Catalogue_renaissance_food_italian_vermicelli_almond_fast", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "58a9519a6bd2a486e4ea70938594c7502e66856620b54c498f5a2a24cfee9227"),
		new("component", "PreparedFood_Catalogue_renaissance_food_mughal_nargisi_meatballs", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75f63f2709b3885bfd83eef57c6cf248bfb4aede95a3e5b16a876a1566d8f859"),
		new("component", "PreparedFood_Catalogue_renaissance_food_portuguese_guinea_contact_stew", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7b3c541e571f67f94e097286d03f1c4f8b162fa1e40c9852b0d83187d0cce0a"),
		new("component", "PreparedFood_Catalogue_renaissance_food_safavid_almond_chicken_paste", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f2de26b1c07c8926bffb3a17ce9cc5ce9bedde4786265148f7696bb7747282a"),
		new("component", "PreparedFood_Catalogue_renaissance_food_safavid_crisp_bottom_rice", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8616f571e4292d68d73e21d34d699185d85a5e8af6a55450f42914b929e92e7a"),
		new("component", "PreparedFood_Catalogue_renaissance_food_spanish_candied_sweet_potato", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6f52e476b91b6f216f21b79ba8204b0dfa5a52641d660a406e6a297420ae600"),
		new("component", "PreparedFood_Catalogue_renaissance_food_swahili_sugar_almond_pastry", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c4b84b26dfa1bcd343a177b4b840b939b09fcc839f0b3eeec1221becccc7924"),
		new("component", "PreparedFood_Medieval_Pottage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a594dc3256b632c19f7dc22950be7a839398ee51871325890c566e9a00686c44"),
	];
}
