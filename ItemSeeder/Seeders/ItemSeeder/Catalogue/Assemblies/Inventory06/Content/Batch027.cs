// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("craft", "Writing and administration/bind bound port customs register", "crafts", ["renaissance"], ["item:renaissance_writing_port_customs_register"], ItemSeederOwnershipPolicy.StockAggregate, "6a9487401f410b7dc40f4f2fec3caf714fb8b02e2a3cf489ab1cdad49ee7b821"),
		new("craft", "Writing and administration/bind lashed leather camelid-cloth document coffer", "crafts", ["renaissance"], ["item:renaissance_container_and_trade_document_coffer_01"], ItemSeederOwnershipPolicy.StockAggregate, "24c9649231f2efc5f4f07f1ba1348eea9c0cc5cfc616df11057304c4c5cd4d70"),
		new("craft", "Writing and administration/bind long deep-carved wood writing desk", "crafts", ["renaissance"], ["item:renaissance_furniture_aca_writing_desk_02"], ItemSeederOwnershipPolicy.StockAggregate, "620dde19c534b9b747af644d02ad26de988fc8821fb26de64d4378cf71e04c25"),
		new("item", "adjacent_antiquity_fine_ritual_cotton_mantle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "252e018e74be8f5d951eeee3383a5efca99fe2ef4237ff0c00bab94d7176858e"),
		new("item", "adjacent_antiquity_linen_shoulder_cloth", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa0b83df4e5f8f1a5368f307707030ee17dc977ff14a3d19a0ed11d4aeede600"),
		new("item", "adjacent_antiquity_sleeveless_linen_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "93fcaa4b7b8608db5fd0a43962ee0e7587ae349c875e7b80bc8099a5bbd0fcfa"),
		new("item", "adjacent_antiquity_tall_linen_headdress", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dabc0bfc2ad8c0636b1d0282efa591dcecf378377d8535d30772cf551618d083"),
		new("item", "antiquity_accessory_etruscan_bronze_fitted_scabbard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "541fa23150803e5f5d595929c5497544b6c1d5d4d9f10da677032b814d7373fb"),
		new("item", "antiquity_accessory_germanic_axe_belt_loop", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "41f803549b688c16e059cf1b8ba2c7d562622493e22273552122b26d3a7a96f4"),
		new("item", "antiquity_accessory_roman_military_weapon_belt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe33042392c35130640a058ce6eb980eb922f888118284e68ba364d591381f40"),
		new("item", "antiquity_bronze_stylus", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb3ad2d20f830958c431bd6fb93bdeede282b22425e1d50a0372de1843cc453d"),
		new("item", "antiquity_candle_mould", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "542bd58bf67995a306984147c4db44345dc2e2bdd5671dfffad69c1e62606704"),
		new("item", "antiquity_carved_wooden_screen", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "298cc820ca5d6a1edf4e9b40e2bf5a5acef055d42bb4cc4bf70943cae7445cb3"),
		new("item", "antiquity_cedar_canopic_chest", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b54054cda8cded6fa2e9240c917b2c1d01009799cd2e8de038e7b8a182742f98"),
		new("item", "antiquity_cedar_cosmetic_box", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1345fbf166e7e8418cbb133e20958b7e16d9df59c8f23c0c16653f826256b800"),
		new("item", "antiquity_cedar_display_hutch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d059daa329d5bd907e85aca486e786101a24fe6b3203123bd1d2dd8216ba4fd"),
		new("item", "antiquity_clay_stamp", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b523608f8fe2c86060b03d8e3bb225a91917364b3e7d523fd5de743fa23c594"),
		new("item", "antiquity_crucible_tongs", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5a3b782975328bc84b15803d02ce07b3e306ae066ef05e8ef056324cdb7cfc6"),
		new("item", "antiquity_etruscan_pteruges_leather_girdle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "df0448feb1a929e60ba9b96e870bb05ccd2452847c0f98448273749fba59fff4"),
		new("item", "antiquity_fine_bronze_tumbler_lock", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e79a97acb43d3909dff0387677d3351371a70109a19334762a95753723c07001"),
		new("item", "antiquity_fine_carved_headrest", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "95d2193e14b3a4fac099aeaf86ca7221c497ed176f04f5fd09eb7073218cf86b"),
		new("item", "antiquity_fine_sleeved_wool_gown", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "856d70cf3fcfbc0d6f0767a5bac79a0ca7816934b61eb5c5e4c8464cc23445d4"),
		new("item", "antiquity_folded_tablet_wallet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7f4e788aa47ab12cddd36333cd95d005ab69422ff4cdda2b6e3cfdcf44672fa"),
		new("item", "antiquity_food_anatolian_fish_sauce_relish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "90ffba87d53cf189803775915de57f6b57ef89a8844a2517ad846f61c52615d1"),
		new("item", "antiquity_food_egyptian_meat_dish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "18fe7e08c020f2f97e78457cf94af5e0931704b92b09b81c7113aa4796fb0069"),
		new("item", "antiquity_food_fermenting_date_beer_amphora", "antiquity", ["antiquity"], ["item:antiquity_food_finished_date_beer_amphora"], ItemSeederOwnershipPolicy.RequiredRelationship, "4fe9f971e9e73abcfb5e5dfae5a5e30e0b2b920ddad25f043bc03f0ad3d17125"),
		new("item", "antiquity_food_germanic_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f6529ffd5c733e6706937cfe917bf50d67d175c627677a4cd9d20a480147838"),
		new("item", "antiquity_food_germanic_pulse_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "96b03cce49ed4f0db85a4e0aad30e6e43d60a6c38f24d1e6f42a49e2d4e6c5fc"),
		new("item", "antiquity_food_punic_fish_sauce_relish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "25a612cefc2f61dd8a050588c60fd5c207b99089dbc67c6ca8711a4116696d30"),
		new("item", "antiquity_forum_notice_board", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f45d37608e50280fd186fa66af2d816d22f13d0cc2368f95a4f0c4a3368c2c53"),
		new("item", "antiquity_germanic_decorated_spangenhelm", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "522702b3424c5afe89df1550ca1ebbf01d93364b83838e1a70ca8b2a0a14c90e"),
		new("item", "antiquity_glass_blowpipe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "82b58cadbd5c6cee538a84975defec415c39a3cc5b228637e7af369f1ab8f343"),
		new("item", "antiquity_kushite_leather_archer_bracer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d1856286d28ec1a51fffaaf380bf3967de33d4e2a1cc88835ec45dccf7489e8"),
		new("item", "antiquity_leather_dispatch_satchel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e02b42165b72bad60dfe0aae92b18b727b3d146b64bd7387258eebfc8842a6d"),
		new("item", "antiquity_leather_edge_beveller", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2005aa3af3f5e62870ce4d9c8ab0d9a135b6a35964721b7c9cbf9e5def2d131d"),
		new("item", "antiquity_light_linen_head_veil", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "45b7100a3c72fe1d18185069948c3b480479f9af468efec6e90c3157bf2cb9b3"),
		new("item", "antiquity_liquid_anatolian_belt_oil_flask", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "30c8adee746cf574e083bfbecf3f50015cf7bd7f42dff17bde9825d34fb1a640"),
		new("item", "antiquity_liquid_anatolian_stag_rhyton", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a57245ef78f64391f9d219e0987aa052475a6a171ae4589828c7a64f093dafb"),
		new("item", "antiquity_liquid_athlete_oil_aryballos", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef69085ba1d3a95031945342b073633e2b4e22e3d7eaecbae38133515fea9a57"),
		new("item", "antiquity_liquid_bronze_belt_aryballos", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c2ef1ebe001a24043f55165545c858f293480efd27c40a1a4aa97faef1f51e4"),
	];
}
