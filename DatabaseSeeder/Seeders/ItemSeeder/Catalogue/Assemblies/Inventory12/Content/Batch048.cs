// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch048() =>
	[
		new("item", "industrialised_transport_support_spares_0142_stackable_wheel", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0c342786e283a8832e3c638cc10489be46fa7cb138c483318b5a2e8305bf18a"),
		new("item", "industrialised_transport_support_spares_0180_wall_mounted_tyre", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_transport_support_spares_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "5ce18e0f84b745cc6f657302a27cc40e6034e01b2a578e867e73752ac3ac9ce0"),
		new("item", "industrialised_transport_support_spares_0188_travel_jack", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9f5539eb6f0c28c0c363a1ef3f375bce1ef80ecc07f8000475b0d27602dc022"),
		new("item", "industrialised_transport_support_spares_0206_plain_lamp", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "63350161e2fc64ed9672ec81e443546dea936245b224f22b503b78e5f32f28ff"),
		new("item", "industrialised_weapons_police_military_0028_compact_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8a137e6cc55f14762eb21557f1ed032a6e6c40ce826c31e41e955a473631b3d"),
		new("item", "industrialised_weapons_police_military_0076_institutional_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1b9d7790cc0c0eb86706080af350810295186f6e326fd4f8d0ec38ac75b4c9a"),
		new("item", "industrialised_weapons_police_military_0086_weatherproof_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1bb9a27a0640e78f3c1fded1ec26ceca6a0223a49fdba1e47c63d482bf44734"),
		new("item", "industrialised_weapons_police_military_0087_weatherproof_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "429851fb6febb97ea0b1a3c5754111fa6e084693e47b4cfd8b13a762da16de7b"),
		new("item", "industrialised_weapons_police_military_0097_service_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "615026c52ece98dbafc3be994c7de167219e1edddf0dc8e8afb0ba249f7f7e3f"),
		new("item", "industrialised_weapons_police_military_0143_stackable_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "979315ad738d4ed4b345b5268b6fd2e6206a5ee83d0e9341b9de781695605a20"),
		new("item", "industrialised_weapons_police_military_0154_field_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f54697af563f54eae4ec5179dbdd3961de2048b2e158592ab8a0f9191d3d199"),
		new("item", "industrialised_weapons_police_military_0172_wall_mounted_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac116d2339d4c3bd7fe3c34a871f23c9ed7f77c0db65c063a242dd57b2a72103"),
		new("item", "industrialised_weapons_police_military_0190_travel_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "f67811e6166826611f00b2b658663d42656f0f572ed23f3ed1661e238d43f669"),
		new("item", "industrialised_weapons_police_military_0195_utility_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "cf38a3cb0f4da33850a2417cbd4a2c1a73c6848f3665ea9ec847b3c0ddc09e68"),
		new("item", "industrialised_weapons_police_military_0201_plain_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "302eff8e628906fa637ad707de044a24a68148aa2c337cc5205cca14b919e88c"),
		new("item", "industrialised_weapons_police_military_0204_plain_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f6f7db158561d81acfec55d917beeb48fe4734f88ca342883080823834bb166"),
		new("item", "industrialised_weapons_police_military_0211_reinforced_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "219e40767b460fb9a56756d1f0579efdfaa6f89ad458e9adfd0d0a82592c6221"),
		new("item", "industrialised_weapons_police_military_0217_reinforced_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "50dda5866537cc373f2d6a0e2ef67d7000f68bbe2b242ead499fa28710bd93b3"),
		new("item", "jewellery_amber_disc_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ab487c9009aad967d715c86af8a0efcf296e24bd86ba652114d2cba686a3ee5"),
		new("item", "jewellery_blue_melon_bead_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b5e445393f1d288c8931c54b94b2ac98736a81e523508b3c7823484a6ef9496"),
		new("item", "jewellery_bronze_animal_cloak_pin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6abf6fe2c9b8ac9d00e9835e2cc57e67198a2a8ae1b0685d1209ab03b012ac72"),
		new("item", "jewellery_bronze_open_bracelet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3435e2089908a1d888ce6c44a04ade56775e4dd160fd587f0c93cf73b8d499ce"),
		new("item", "jewellery_bronze_studded_fibula", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9939bc52dcd4bd48a2c19da6f2918a63bc08ca2e2d39d5019b00bb9cefea2e9e"),
		new("item", "jewellery_copper_hoop_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd0c2ab1038d87d3ceadcc820b65059dc44decf71a50abc5b83ed0cfc8a65eda"),
		new("item", "jewellery_copper_seal_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a723308d4eeda64a5e2b24f94d6435d67956251dd7cf8de62d783c35e6ef9d3"),
		new("item", "jewellery_glass_bead_bracelet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec1db3aee90a254565efe09e1221f2a42c99d8184f76b6c32bd2da8306caf315"),
		new("item", "jewellery_gold_pearl_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b388ccc7b25ccf12ea3d38ef0f506d18acc2ac3f44e7332c8e370ea5bd5526c"),
		new("item", "jewellery_gold_ram_head_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "17f0caddfacdca357a074dbe3b4dea624598af5c9a3ee4ec059829b2641ee079"),
		new("item", "jewellery_gold_snake_head_armlet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a680809f4ba03a89cfee8e1250211207a24ec050a628cd003c9aabde524af02"),
		new("item", "jewellery_gold_turquoise_drop_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddf8e11de08371678b237bd24562b3049979378547f1ca147fae0b3367ab09ee"),
		new("item", "jewellery_gold_turquoise_pendant_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c02dc5dc4db2b8523f1be7c1d4da0ea9991db75918f3677b99cc653045f6871d"),
		new("item", "jewellery_shell_disc_headband", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8dbb4a803483728c1de2227f1a9bd99ab9177a2f8acdd8bc56125b904638ce6"),
		new("item", "jewellery_small_bronze_wire_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f7b5362e5c28f46343590e53caae6a980f91d08ebdf52b5f1504293bb144ca3"),
		new("item", "medieval_abbasid_reed_pen_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "401ccce72f2ce19f3ba812c1b4331610c3ed2c036789d0ee16e21dba7d1af37d"),
		new("item", "medieval_bronze_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "161a2b342e53d914c37e81b6aad4248d8824fba4265d1f6d100c0cfda88db1cf"),
		new("item", "medieval_buddhist_incense_burner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "188661eb496c62a567539eb49f00178e00af56445f758a59688a6dcb0c1da050"),
		new("item", "medieval_buddhist_patched_kasaya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0cdd64a0d5c95b7170a90caa3e14b684dfb0b903ac379fc4465da04bf9215da"),
		new("item", "medieval_byzantine_walnut_display_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b02f23beb00609706d7903d5ac41c5fdaefb24fde9194f27076bbdd3a6d9b69b"),
		new("item", "medieval_church_eagle_lectern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ed98cae67d84792003e6d3bc3591fb5399d9651ed1b27b105978707715f1435"),
		new("item", "medieval_church_reliquary_display_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd5ba13616f9109a1345c466afe4b8bd9ea53d3b64933746c2c71de1b907247c"),
	];
}
