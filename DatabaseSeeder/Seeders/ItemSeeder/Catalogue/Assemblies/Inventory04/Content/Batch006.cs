// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch006() =>
	[
		new("craft", "Blacksmithing/make bronze clay knife", "crafts", ["antiquity"], ["item:antiquity_clay_knife"], ItemSeederOwnershipPolicy.StockAggregate, "115e966967dc89fed06235137efedd5debcc2b4b712edebc904ba071de6209e3"),
		new("craft", "Blacksmithing/make bronze disc earrings", "crafts", ["antiquity"], ["item:jewellery_bronze_disc_earrings"], ItemSeederOwnershipPolicy.StockAggregate, "fe4f22d318dac1d3e887e856a1ca4440cfa95e29e55e7c20a9df2ac13f31111e"),
		new("craft", "Blacksmithing/make bronze drop earrings", "crafts", ["antiquity"], ["item:jewellery_bronze_wire_drop_earrings"], ItemSeederOwnershipPolicy.StockAggregate, "d3f6a80c3aa012df49fce57855448c828d1f3d4932bc435ac8f94551ed684bd1"),
		new("craft", "Blacksmithing/make bronze hoop stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea20a6ac067925f59b90e436c0a4259d6f93bd9fd7214f899ea0b6a9802cef9d"),
		new("craft", "Blacksmithing/make bronze leather gouge", "crafts", ["antiquity"], ["item:antiquity_bronze_leather_gouge"], ItemSeederOwnershipPolicy.StockAggregate, "3fa1ae6743cfb4126b738e1f00fb6cbec1130ddae052542e02041d694c270d50"),
		new("craft", "Blacksmithing/make bronze lidded cista", "crafts", ["antiquity"], ["item:antiquity_bronze_cista_box"], ItemSeederOwnershipPolicy.StockAggregate, "4879e1447f7427d7479f103794eef1473f801ebdf22ada640481155ae370d464"),
		new("craft", "Blacksmithing/make bronze oil-lamp shrine", "crafts", ["antiquity"], ["item:antiquity_oil_lamp_shrine"], ItemSeederOwnershipPolicy.StockAggregate, "ff64bdedf22bf25c7475efc27624ce38a7c96235ec3d6d474a02c9deac66c903"),
		new("craft", "Blacksmithing/make bronze tripod stand", "crafts", ["antiquity"], ["item:antiquity_bronze_tripod_stand"], ItemSeederOwnershipPolicy.StockAggregate, "a537b524219e600c83188215d241ffaa4143044b6ab8bd6a4b5566f59c884686"),
		new("craft", "Blacksmithing/make bronze warded door lock", "crafts", ["antiquity"], ["item:antiquity_bronze_warded_door_lock"], ItemSeederOwnershipPolicy.StockAggregate, "3be93d8c910f2b4b8076693b1f4d555095f630fa5d2eb8749d5444c0f1c866ec"),
		new("craft", "Blacksmithing/make copper bell anklets", "crafts", ["antiquity"], ["item:jewellery_copper_bell_anklets"], ItemSeederOwnershipPolicy.StockAggregate, "13b41c063de3e80b114f59345f2789d9cc1b29ccdf54a61854998bfd28409c1f"),
		new("craft", "Blacksmithing/make copper seal ring", "crafts", ["antiquity"], ["item:jewellery_copper_seal_ring"], ItemSeederOwnershipPolicy.StockAggregate, "5e4fe2205c1b4c934470617b9f7aebc8845bc4352ca5403df56e3673286f4e9d"),
		new("craft", "Blacksmithing/make cord of guard keys", "crafts", ["antiquity"], ["item:antiquity_guard_key_cord"], ItemSeederOwnershipPolicy.StockAggregate, "839bb4e0735b07b3c49ce6cb37829d089b5ee174045521e56c59e5091ef4239f"),
		new("craft", "Blacksmithing/make forge tongs", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:historic_forge_tongs"], ItemSeederOwnershipPolicy.StockAggregate, "46708b6b88c184d2696580cd85ccb92409978fe9d309b6a2575df79e15f2e648"),
		new("craft", "Blacksmithing/make iron signet ring", "crafts", ["antiquity"], ["item:jewellery_iron_signet_ring"], ItemSeederOwnershipPolicy.StockAggregate, "c9ffe90c32a0a6a35e2f2297476fd63107e4088b003dcbefeebc6220c4c0fede"),
		new("craft", "Blacksmithing/make locksmith's installation kit", "crafts", ["antiquity"], ["item:antiquity_lock_installation_kit"], ItemSeederOwnershipPolicy.StockAggregate, "b928be7d5a62c324cb4f7adbdae97fb02dbbf29b4a02279e1eb4291e811c660a"),
		new("craft", "Blacksmithing/make long iron gate key", "crafts", ["antiquity"], ["item:antiquity_large_gate_key"], ItemSeederOwnershipPolicy.StockAggregate, "50e97da2a5636baf7d180fe3a8f306bc99c6c0d089d47ddda225ab95082c65a1"),
		new("craft", "Blacksmithing/make massive barred gateway", "crafts", ["antiquity"], ["item:antiquity_massive_barred_gateway"], ItemSeederOwnershipPolicy.StockAggregate, "265f0cf8e9efc4f91cd343a11c8e3617447eb0c9fdf8bc1b708251d190ec52fd"),
		new("craft", "Blacksmithing/make polished bronze burnisher", "crafts", ["antiquity"], ["item:antiquity_bronze_burnisher"], ItemSeederOwnershipPolicy.StockAggregate, "414feab017ea000fec619014a21ba1e45482b2e482266b58176d3a88be967d13"),
		new("craft", "Blacksmithing/make round bronze camp kettle", "crafts", ["antiquity"], ["item:antiquity_liquid_steppe_bronze_kettle"], ItemSeederOwnershipPolicy.StockAggregate, "e487438d3175e3dda30abab1be62fa9d2a53cfda44a34cf0320315942e142499"),
		new("craft", "Blacksmithing/make shallow bronze libation bowl", "crafts", ["antiquity"], ["item:antiquity_liquid_bronze_libation_bowl"], ItemSeederOwnershipPolicy.StockAggregate, "5ed2a59d6e65f07e3772c2692d3b954f71702d46b8789df7ce69079942a1769b"),
		new("craft", "Bonecarving/make bone and amber necklace", "crafts", ["antiquity"], ["item:jewellery_bone_and_amber_necklace"], ItemSeederOwnershipPolicy.StockAggregate, "c9ba6f273db44ed7c63e7e3d86ec29bccf5a0d289e655a90cb52b25a7d7b1b6c"),
		new("craft", "Candlemaking/make pitch-soaked torch", "crafts", ["antiquity"], ["item:antiquity_pitch_soaked_torch"], ItemSeederOwnershipPolicy.StockAggregate, "dded4b56a79bab819f84a80835365eba9146d36d6e5fccfc20ede27b9704f890"),
		new("craft", "Candlemaking/purify beeswax candlemaking stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c638cad2af0a3520618e961753317464578c2acc723cbb8ad945889ce372f97c"),
		new("craft", "Carpentry and joinery/build asymmetrical cedar textile drawers", "crafts", ["renaissance"], ["item:renaissance_furniture_jpn_textile_drawers_03"], ItemSeederOwnershipPolicy.StockAggregate, "fdc969d5f5e66042aa7b3e0ef20b0ed46ad6656898a6d2583e9b7127560699c8"),
		new("craft", "Carpentry and joinery/build bird cedar secure door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_secure_door_156"], ItemSeederOwnershipPolicy.StockAggregate, "1f23fe0548e5493d9d2af5a23785521b309d4a55e3efa1cbb643fb8ce7c026ca"),
		new("craft", "Carpentry and joinery/build bird teak braced door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_msea_braced_door_401"], ItemSeederOwnershipPolicy.StockAggregate, "b3facfc14940de8754d6f8cea4b750ae77415ac6600ecbb0186876397f99b750"),
		new("craft", "Carpentry and joinery/build bird wrought iron metal grille pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_grille_403"], ItemSeederOwnershipPolicy.StockAggregate, "c8f9883aee7c65b00c0ef5f9dab23fa795593b147d78ca04e0557af3308a4cdc"),
		new("craft", "Carpentry and joinery/build brass-faced wood textile drawers", "crafts", ["renaissance"], ["item:renaissance_furniture_aca_textile_drawers_01"], ItemSeederOwnershipPolicy.StockAggregate, "df7e0404bb20f6a863dc6041174b0a982d090bab915b6bb5a3d62012f5478f66"),
		new("craft", "Carpentry and joinery/build broad cedar carpet chest", "crafts", ["renaissance"], ["item:renaissance_furniture_persianate_court_carpet_chest"], ItemSeederOwnershipPolicy.StockAggregate, "c4dc458f3585fd966c8268ce5d5a5884aeced58c443bcbdb259364747efeea6a"),
		new("craft", "Carpentry and joinery/build broad oak serving counter", "crafts", ["renaissance"], ["item:renaissance_furniture_tavern_inn_bar_counter"], ItemSeederOwnershipPolicy.StockAggregate, "7feafa0400f3115021f812f71d8cdf2b07955b9d4011192608a6d2428fade7a2"),
		new("craft", "Carpentry and joinery/build broad shell-inlaid teak low drawer chest", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_low_drawer_chest_01"], ItemSeederOwnershipPolicy.StockAggregate, "df46908e347bf9d790f9360fb4692f50c0bec5cf215b22ff995f1642adbef994"),
		new("craft", "Carpentry and joinery/build carved cedar household serving tray", "crafts", ["renaissance"], ["item:renaissance_container_col_domestic_serving_tray_02"], ItemSeederOwnershipPolicy.StockAggregate, "9c2384c79e09c952160f36f77ed75f896c72b8a6bd386a44d48bd5f9767027fc"),
		new("craft", "Carpentry and joinery/build cedar salt-sample large trade cask", "crafts", ["renaissance"], ["item:renaissance_container_rse_trade_large_trade_cask_02"], ItemSeederOwnershipPolicy.StockAggregate, "a55b5dc5490970755fa8e27c01538522927cc23747a443a291f14fff3bf37534"),
		new("craft", "Carpentry and joinery/build checker bronze warehouse gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_eth_warehouse_gate_503"], ItemSeederOwnershipPolicy.StockAggregate, "b5741e69b99e0871c0fb9f0f87f8033b1f029770793381be19aec609bda30d10"),
		new("craft", "Carpentry and joinery/build checker oak lattice gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_msea_lattice_gate_371"], ItemSeederOwnershipPolicy.StockAggregate, "e691dc9446bbdc69876cf0d0e4dbf81e64db3d50870be2dd558d29441252caed"),
		new("craft", "Carpentry and joinery/build checker pine braced door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_142"], ItemSeederOwnershipPolicy.StockAggregate, "1e99ad41b7e2a3f4c202faac37a944425078260e114f4b8c4b7e81681475040c"),
		new("craft", "Carpentry and joinery/build compact slatted bamboo household cupboard", "crafts", ["renaissance"], ["item:renaissance_furniture_jpn_household_cupboard_01"], ItemSeederOwnershipPolicy.StockAggregate, "f1fbfd2558e17666930e571d1d1d327ad4f5dab9c8d8b1ceb458e58fe9452147"),
		new("craft", "Carpentry and joinery/build compartmented mission supply chest", "crafts", ["renaissance"], ["item:renaissance_trade_atlantic_contact_mission_supply_chest"], ItemSeederOwnershipPolicy.StockAggregate, "0dbee95b6f22e354d13fa67c1622743c5185b7525d0ccc9d46e95f61b7d480c4"),
		new("craft", "Carpentry and joinery/build deep pale-boarded oak counting desk", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_counting_desk_02"], ItemSeederOwnershipPolicy.StockAggregate, "ff0745ab522f5005687d15debec1047361a503c7c5be8d0bd4f14dc3a1076159"),
		new("craft", "Carpentry and joinery/build deep panelled pantry cupboard", "crafts", ["renaissance"], ["item:renaissance_furniture_urban_kitchen_pantry_cupboard"], ItemSeederOwnershipPolicy.StockAggregate, "e00e7d26446ffe01909d751fc745a956e81429da0bc2d648742a83676db24070"),
	];
}
