// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch062() =>
	[
		new("item", "renaissance_medical_alum_stock", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c5c2ac8bf2d370a5f5a2bb93bf7dc62784b6b9237ef0dc19fd5019435019f94"),
		new("item", "renaissance_medical_corpse_shroud", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9adc2c8efd2c2021bc3002b0292759813d1b1e5ce25a39b539f7c80c818b519"),
		new("item", "renaissance_medical_leg_brace", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcea946cd2fcfeefbe189b9ccc63f354ec13bb4c3f4a227e434f30b7ce01cce6"),
		new("item", "renaissance_medical_pesthouse_bed_roll", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "115499735888926517ce2855c9a1764efed9d5aac8cb129577f994b9a12821a0"),
		new("item", "renaissance_medical_pill_tile", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd3c0076d7f83febfbc2bf8cf73174a32349493cc98fbc172f36250b9aa135bb"),
		new("item", "renaissance_medical_powdered_pearl_packet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e75ad448e27f792921feec7071fed23527deaec79c8e6884fd67859941979056"),
		new("item", "renaissance_mesoamerican_rectangular_blouse", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e77132795b97860a0c05fe2fa5b8b5983c73ceabce7f450e34a072a8e72f36c"),
		new("item", "renaissance_military_buckler_fist_grip", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7e82f75174472c9892ecd71330805c7de96d4b579f93aabb9ab082b8808abe9"),
		new("item", "renaissance_military_burgonet_cheekpieces", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e3d429994d382e2ba082bb47a11fd6f6f8b56dc6d33ba96c437dc49d6e8a479"),
		new("item", "renaissance_military_cartridge_bandolier", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddc6dd81256b711cc99e9ca1883fda7453d42e7412efa1e033e01820b240eda3"),
		new("item", "renaissance_military_field_point_bolts", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a47d8b9f915a5146dc83856a7c8fa15d643bde8062db99986cfe79439d3428b"),
		new("item", "renaissance_military_hussar_peytral", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49c0fc5fc68aac4d34bdef43e2143ee30eadc673e26a96308d8f4cc0444bac40"),
		new("item", "renaissance_military_minion_field_gun", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "672f3ef0c948d11937a177e6fce4992b32502a5cb79cd803f8cc5ab410b58a51"),
		new("item", "renaissance_military_tournament_helm_frogmouth", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "09ae4d2e74d576f0d1d53c8d14d26daccfbbf9ed12eeda319ea06f8a76c6ae5a"),
		new("item", "renaissance_northamerican_feathered_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f10541d64766b84751bf02659bb339888fc8ef2f2af0f3c5f2a470bb3fd7781"),
		new("item", "renaissance_northamerican_fur_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6b09b083cdf1dc63343530c3fae546da4edd20c7f24abb2dd8fe9495b392842"),
		new("item", "renaissance_northamerican_furlined_moccasins", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1aef0db1e9cf5431d8b7319249b57f8186f8c91e2ce0e3b54bccc7ae0f194ae"),
		new("item", "renaissance_northamerican_paired_hideleggings", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e205fab482feba1f6b7ec1f1b8d14554c8546361e5b62cd68bcd27d9a6f9e621"),
		new("item", "renaissance_persianate_asymmetric_underrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dd25527065247017826db3b4deacb562f396f8f1f663eb68d5d1991f633f173"),
		new("item", "renaissance_personal_east_asian_literati_document_wallet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "724f6bb9e906c7135a279bd30f37186a157b67a9144b96c60edd35da0162ebb8"),
		new("item", "renaissance_personal_japanese_ryukyuan_cloth_bundle_bag", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5842fa7a81a3331d666cfb5f76f488ac8afcb760b085d6243a93d23a72abc5c1"),
		new("item", "renaissance_personal_persianate_court_signet_pouch", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5450d519c0fae45b4abd3fbd385258fc5003429430db5410f9b9cfde6947e3a7"),
		new("item", "renaissance_sahel_conical_leatherhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76e11d2340a71d532070c0dc6110b57f4108e7ffcac1f544a55b3773fad42f8d"),
		new("item", "renaissance_seasia_palmleaf_conicalhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd6c13395ec83fc6862e7acd6aafcbab2d34724b8a4faa26cc1ae17d423fa93b"),
		new("item", "renaissance_seasia_pleated_courtwaistcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "31a7620beafc2c38d4f713a666e8b76f5f172cd013e77f79c43261e7a95c6fa1"),
		new("item", "renaissance_shared_clothing_folding_hand_fan", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7faa578c93169e240b670be1dac274542a2878c57056fbfc9540b03fbac9e5d3"),
		new("item", "renaissance_shared_clothing_frontlaced_ankleboots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "85cb2535674d1cb88c34cce11eb4c4ad761846e21eade98a99cdc43a54d55e50"),
		new("item", "renaissance_shared_clothing_narrow_trousers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f26af8c54a79f386230eecc0fd40df260e1a4335099a00912f2d6d62df0ea3be"),
		new("item", "renaissance_southasian_quilted_cotton_longcoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9bb0b5576cb597f412afe40ec19400e2b0f4e89f5c9b7d118210cac73faf2f0"),
		new("item", "renaissance_southasian_sleeveless_courtovercoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c8e07251eb7b34377cb3db13c9b2858990b1d897f3b0730ed042f1e94082300"),
		new("item", "renaissance_trade_andean_storehouse_maize_store_basket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0367d254c26746e39f27af1b6aeec5b63af435989885860ba8fea039b98412c2"),
		new("item", "renaissance_trade_apothecary_perfumer_prescription_archive", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e19521b693abde762dca96b08d34e20b352450ad859da182a820e56e563d0292"),
		new("item", "renaissance_trade_atlantic_contact_iron_tool_exchange_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e33e6ff7d8581febe8bb12b8ebdc1e0e2e47d5e6cfcb8ef21453f4dc7ddd3b94"),
		new("item", "renaissance_trade_civic_guild_standard_weight_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9375a0606d32ccb85e6b0a6a94e890525d1f9d838a4f02725bfcb601a7e0241a"),
		new("item", "renaissance_trade_mesoamerican_market_cacao_market_basket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f336c58f7003970da01081a573dc0c66bbd150f0f7af97608ba41bdd801db60d"),
		new("item", "renaissance_western_pattens", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c08a5072e69d13dcc37cc4089b6feb7f3c7a88b7aee86f0f7977018a9f6f99e7"),
		new("item", "renaissance_western_short_cloak", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8eb32956f0407ff492f97a9406f25237756dee1a889dc6acc2dfc5eb7b89d53f"),
		new("item", "vehicle_preindustrial_wooden_oar", "vehicles", ["antiquity", "earlymodern", "medieval", "renaissance", "revolution"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e3378282492bb3babbde085252962b267e2393c877226da0ffa3d1cbdae9159"),
		new("knowledge", "Ancient Medical Treatment Supplies", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5584ab99a4c99f9836ebbd500cdc381cdd281b685eda70ab71c17b46ecdf8e8"),
		new("knowledge", "Medieval Writing and Administration", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d160fecf93903cc81f037af3254940f04140dd5fe2029582040974cf297b5bf"),
	];
}
