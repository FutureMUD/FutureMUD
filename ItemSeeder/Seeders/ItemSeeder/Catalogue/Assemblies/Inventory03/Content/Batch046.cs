// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch046() =>
	[
		new("item", "industrialised_transport_support_spares_0208_plain_jack", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8124b85c32569bb735167081bdd398c7c00315d3080aaac9c1219e70d000935"),
		new("item", "industrialised_weapons_police_military_0003_plain_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cd42918198df75d527654755ebda5c28853a1de335f9ae155bc2234667ed9b3"),
		new("item", "industrialised_weapons_police_military_0018_reinforced_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0c99ffc78dbaa4a155d6265c306f53fff35b58f03e2eb34553e963bd06c9a60"),
		new("item", "industrialised_weapons_police_military_0027_compact_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "45a9959725b0b902c7b552248325ddd55348b91b7920f78008098c47a66fe052"),
		new("item", "industrialised_weapons_police_military_0035_heavy_duty_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "11f11d93d9cba9600a398a5a9fee5e14cff98833b933e0708f8a7e5c3eb3a271"),
		new("item", "industrialised_weapons_police_military_0041_folding_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "27e28eae4fe5fa32ced9a552ad037e03c48e48874bf58da1eac7676322be18b2"),
		new("item", "industrialised_weapons_police_military_0053_portable_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "398b341e24c998f3352ddd64222539c00664ecaa890f0d8c64e965cbc54dfd7c"),
		new("item", "industrialised_weapons_police_military_0058_portable_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8f4350c385a7a18fb49ecf034ed75a91fd711f62929c3b16c9c6e3c59a9e890"),
		new("item", "industrialised_weapons_police_military_0069_workshop_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "256c419d2288525c7feda55280ee3ee5c3e948f275bd855d3f09bcb02e908516"),
		new("item", "industrialised_weapons_police_military_0073_institutional_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae81182ebbb1d98c3a8d1ed62110709dbf1ba809c1c11d840cb318742658d970"),
		new("item", "industrialised_weapons_police_military_0112_economy_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea94f513807d94d649c38417f285bfb4510cce98f563045e5e42b347a40abe21"),
		new("item", "industrialised_weapons_police_military_0163_countertop_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa77afc547e9cc84c5e50c6ec8c017ed5e9a8a2c7d9c82d8c52e1d1b89e1e536"),
		new("item", "industrialised_weapons_police_military_0176_wall_mounted_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "7daa773759d4ae41bf0e17d48fd36f73a1f24e81e83f9791273e4bddaddb230a"),
		new("item", "industrialised_weapons_police_military_0181_travel_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "193c5cb65c2d74e74c5354c32eaf38f40f85a62a6aa98f9ada32cc60bec60079"),
		new("item", "industrialised_weapons_police_military_0192_utility_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "41a87446e6d58cdcdd288424634eeaa54d29b7be2069909c70a0bd537f731551"),
		new("item", "industrialised_weapons_police_military_0226_compact_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e654bd39878e56f456d0d3371bfa91fdd6d1c776520bc93cfb6a93af1dd94733"),
		new("item", "industrialised_weapons_police_military_0237_heavy_duty_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca81e5b30f6db4f33f21ec81b8603f6915b9b29e2b256ff105e862fd44efe4f5"),
		new("item", "industrialised_weapons_police_military_0239_heavy_duty_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3fb7d30ed25f9cd26e350c44731185857639e9ef0228529bf26839decd4ea80"),
		new("item", "industrialised_weapons_police_military_0244_folding_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "de986b47e6f53f1bf1755e7b7b8d0ce6f3eaff8058c4b42771add90e4bda8ff5"),
		new("item", "industrialised_weapons_police_military_0260_portable_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "9affbb7b332021435e2a5e92d0b70674f4744844cec6b45d92bf902e07280fdd"),
		new("item", "jewellery_bronze_animal_belt_plaques", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8797491d21a1325c9a7bb06870422e0152cf470485050df2f3d79abe065f1a7b"),
		new("item", "jewellery_bronze_coil_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "68918cde585aa0153834f0165e361bfd1e83991fce2f36613fbc64e1f8fc1d98"),
		new("item", "jewellery_bronze_open_armlet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "176cd0e14741435f03db0ebbf4827cd43daaf1b4d860f1d96b1b445551063949"),
		new("item", "jewellery_bronze_tapered_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "62f7b25f02712560023dd40355ccf7d91d46a06b34a7a0fd8525908e1b46174e"),
		new("item", "jewellery_electrum_ankle_rings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c5cb48cc945ac60c1abc1d82a13df64c5f10157dd3b3c537b970ec2419e97bf"),
		new("item", "jewellery_electrum_spiral_bracelets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7938f52aa19eba927ee1007d4e21f880a38380eb7a0f9e75a5be8fcdd71a580b"),
		new("item", "jewellery_gold_cage_ball_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b3aa2510b6014a6eb5289d306d286b26605a16dad46aaf99e95de7b5c4e9d8b8"),
		new("item", "jewellery_gold_garnet_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "99536970c053ed29715e6a3bbde58892d1d4d94664002b7ec155e83248e07090"),
		new("item", "jewellery_gold_lapis_hair_pin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a48cbdb74a3851021778c5198c40750adb75ba9014ea4c6f15b54720b741722"),
		new("item", "jewellery_gold_ram_head_diadem", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1cbc051b6375869d3690933fc1b54e367351645bb4b5b433da26f5025d386610"),
		new("item", "jewellery_gold_scarab_dress_pin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ac6a7dba0508efa080169426580a085479dcea174343e8373c34f41e61f1e18"),
		new("item", "jewellery_gold_scarab_signet_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ca3c11fb1d9fc04930a9e958823fb490a9814b403eedde4e8332641fb7f60ec"),
		new("item", "jewellery_gold_turquoise_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c30a9e307143bbcf0f93881581cc143838c1d19d006e22566b32b64defd33b56"),
		new("item", "jewellery_heavy_gold_torc", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dcc3564824d2907f0a461494a67ba4192a959ab649771ee52f2b8325952cd49"),
		new("item", "jewellery_shell_laced_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e890dc67dab621c7efcc55c07cd6966a4111b466ce26d9474cda8cf6f69f3cb3"),
		new("item", "medieval_apprentice_linen_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a698a382ec257cf2eddc81c89b73aec617bdd31b14d563f58a4209348f227f70"),
		new("item", "medieval_bazaar_merchant_jubba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "50c83c2109b3889bcbf6692c166be9a6bcf688201b65cb2f54c11ce49834ca63"),
		new("item", "medieval_bronze_mounted_leather_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "be4636b14dedbf33631cb2e8e37cadfca5653430d89df37e54a792b6bfbf58c9"),
		new("item", "medieval_checked_wool_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "984d05ed5e5d7cb70b8392ccbc5eb68d0367525548cba7be0260a2fa35439fec"),
		new("item", "medieval_child_leather_shoes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "641c61dfc842ea05ff1fae666dbb423f87bd753a3381075aa6a7620c484fb23b"),
	];
}
