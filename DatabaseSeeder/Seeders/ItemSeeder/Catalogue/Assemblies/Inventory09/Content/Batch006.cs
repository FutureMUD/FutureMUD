// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch006() =>
	[
		new("craft", "Blacksmithing/make bronze incense censer", "crafts", ["antiquity"], ["item:antiquity_bronze_incense_censer"], ItemSeederOwnershipPolicy.StockAggregate, "44d064d6b25b6828031a68d8c13a9fc38abd9f290cb3b9e76138bf0a262f881d"),
		new("craft", "Blacksmithing/make bronze leather awl punch", "crafts", ["antiquity"], ["item:antiquity_leather_awl_punch"], ItemSeederOwnershipPolicy.StockAggregate, "2366a155867688639e0f5df42ab6d9d83b4ec7dc2058e83b2ae71f90da3dfacd"),
		new("craft", "Blacksmithing/make bronze papyrus strip knife", "crafts", ["antiquity"], ["item:antiquity_papyrus_strip_knife"], ItemSeederOwnershipPolicy.StockAggregate, "eaae1a886bc2db78e17bedfc2a34b0e236d5377a0e3e0257f5f858f62bff3609"),
		new("craft", "Blacksmithing/make bronze textile shears", "crafts", ["antiquity"], ["item:antiquity_bronze_textile_shears"], ItemSeederOwnershipPolicy.StockAggregate, "155d81bd66cf2c5f7ae2ce077f383211ec448197bbc2e6efa5b0f29b4a4b509e"),
		new("craft", "Blacksmithing/make bronze wood chisel", "crafts", ["antiquity"], ["item:antiquity_bronze_wood_chisel"], ItemSeederOwnershipPolicy.StockAggregate, "70638940f28ed9e1f3b0175ef917d365094ae50f40ebcfa9daab5ace3a171cca"),
		new("craft", "Blacksmithing/make copper ankle rings pattern 2", "crafts", ["antiquity"], ["item:jewellery_copper_ankle_rings_valley"], ItemSeederOwnershipPolicy.StockAggregate, "b1940fda5fac5d51b51625902a12123a8b26207b264d6774844e4f46f737cbee"),
		new("craft", "Blacksmithing/make curule-style bronze stool", "crafts", ["antiquity"], ["item:antiquity_folding_curule_stool"], ItemSeederOwnershipPolicy.StockAggregate, "0990f25202b67eb8fb04e969d1d8749ad4119be0ddfbf317e451da195040ea69"),
		new("craft", "Blacksmithing/make false weight set", "crafts", ["antiquity"], ["item:antiquity_false_weight_set"], ItemSeederOwnershipPolicy.StockAggregate, "f753fcffc76442d32441426c2d0dab45e46d5771635b289ea56a0659ed6baec5"),
		new("craft", "Blacksmithing/make iron wire bracelet", "crafts", ["antiquity"], ["item:jewellery_iron_wire_bracelet"], ItemSeederOwnershipPolicy.StockAggregate, "9123b5fdec50c70e89544d69910556070926fc2f376ac3f085636724c4bf8e27"),
		new("craft", "Blacksmithing/make low bronze workshop anvil", "crafts", ["antiquity"], ["item:antiquity_bronze_anvil"], ItemSeederOwnershipPolicy.StockAggregate, "8d596124be68bb5406f493c3ab9023534719e13924119a5a34c2411e287fcc56"),
		new("craft", "Blacksmithing/make shallow bronze phiale", "crafts", ["antiquity"], ["item:antiquity_tableware_hellenic_phiale"], ItemSeederOwnershipPolicy.StockAggregate, "84020a928adfc88498ab559f6c6a9af5438297d95f2e2f47b55d7fe2bdb9f9e2"),
		new("craft", "Blacksmithing/make small bronze belt flask", "crafts", ["antiquity"], ["item:antiquity_liquid_bronze_belt_flask"], ItemSeederOwnershipPolicy.StockAggregate, "6c03833785afe0992ffdf289ec1036018ec5b99d523b5ba59c270dcb38675960"),
		new("craft", "Blacksmithing/make tall bronze lamp stand", "crafts", ["antiquity"], ["item:antiquity_tall_lamp_stand"], ItemSeederOwnershipPolicy.StockAggregate, "5c91d271c6ec439acc129e5e42f0d2932f2d60f8becb18f4d37a91129c4206d5"),
		new("craft", "Blacksmithing/make wine measure cup", "crafts", ["antiquity"], ["item:antiquity_wine_measure_cup"], ItemSeederOwnershipPolicy.StockAggregate, "6d826a887e27a7a7143ff4580b798bb25b61a40ab6f65d55bc89fbce59c21321"),
		new("craft", "Bonecarving/make carved ivory bangle", "crafts", ["antiquity"], ["item:jewellery_carved_ivory_bangle"], ItemSeederOwnershipPolicy.StockAggregate, "ab158824cde8b0ebf2b7bf2611df0b17e830d34d83ecb86d6a5b9d8d5e205714"),
		new("craft", "Bookbinding/prepare codex binding stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c92a75992cf87023220ecf0c83ae49f532e5741b62b28c543556c2a700a5321"),
		new("craft", "Carpentry and joinery/build $colour strapped pattens", "crafts", ["renaissance"], ["item:renaissance_western_pattens"], ItemSeederOwnershipPolicy.StockAggregate, "c4188319f3ca4b6686006ab4fa790ad06c6616fdb1f33c414c7832fc21a39d99"),
		new("craft", "Carpentry and joinery/build $colour wooden-soled overshoes", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_wooden_soled_overshoes"], ItemSeederOwnershipPolicy.StockAggregate, "60c66646a61bf59473fe78f042efcb43b64a88615cc8d4188816c8b7fe92692a"),
		new("craft", "Carpentry and joinery/build bird bronze lattice gate", "crafts", ["earlymodern", "renaissance"], ["item:preindustrial_door_lattice_gate_21"], ItemSeederOwnershipPolicy.StockAggregate, "258dd4d2fb6e7f4f3a97f849b6c8f0beefa00e454682b62d0b2fbb6cb8026f0b"),
		new("craft", "Carpentry and joinery/build bird oak garden gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ib_garden_gate_67"], ItemSeederOwnershipPolicy.StockAggregate, "02718441deefa5302b4ff3814501db992c8b93a749a251493bd2bb56a8276d7b"),
		new("craft", "Carpentry and joinery/build bird pine secure door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_kongo_secure_door_460"], ItemSeederOwnershipPolicy.StockAggregate, "60e8d7983bf40247cb40deb9af044819888aa6ccb707172a3d8dcbc6b366b2ea"),
		new("craft", "Carpentry and joinery/build bolted ship's mess table", "crafts", ["renaissance"], ["item:renaissance_furniture_shipboard_purser_mess_table"], ItemSeederOwnershipPolicy.StockAggregate, "db57fd5f57a3b85ea25409efd772544cc0ce08841d3f95a60c49bbe690845aab"),
		new("craft", "Carpentry and joinery/build braided teak courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_mca_courtyard_gate_616"], ItemSeederOwnershipPolicy.StockAggregate, "b99c393ae134984070baa4e7b1ae2e2d4ec5d82c4ac83a4e0fe8987aa9e4e2b8"),
		new("craft", "Carpentry and joinery/build braided teak screen door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ming_screen_310"], ItemSeederOwnershipPolicy.StockAggregate, "027211b1ecc806763b4fca51341abb0b8d574c8492ea9b4e2b59ec0ae8317084"),
		new("craft", "Carpentry and joinery/build brass-faced bazaar till chest", "crafts", ["renaissance"], ["item:renaissance_trade_ottoman_bazaar_till_chest"], ItemSeederOwnershipPolicy.StockAggregate, "c0677073e0c282d6893283308ecd3c1cf868a3eb7a5ceda50091dd12ba44e918"),
		new("craft", "Carpentry and joinery/build broad slotted cedar large storage cabinet", "crafts", ["renaissance"], ["item:renaissance_furniture_mar_large_storage_cabinet_02"], ItemSeederOwnershipPolicy.StockAggregate, "ee042f37ee5e6b2c00c6c18a77b1afed3b6ae6aefb4741fe12cd51af4628cc70"),
		new("craft", "Carpentry and joinery/build broad slotted oak nightstand", "crafts", ["renaissance"], ["item:renaissance_furniture_mar_nightstand_02"], ItemSeederOwnershipPolicy.StockAggregate, "5d42644dcdef83ab01c5fe0988f5ad495ebda9da70a1862a9396e56fba525f84"),
		new("craft", "Carpentry and joinery/build checker cedar courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_22"], ItemSeederOwnershipPolicy.StockAggregate, "cf8ca278a36bac9c080887b3e9e40e03f8fe7ca453d8df7744c45b2b5138e187"),
		new("craft", "Carpentry and joinery/build checker cedar screen door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_gb_screen_44"], ItemSeederOwnershipPolicy.StockAggregate, "b178475be24a8d19ca55a16a80556cdb905d372c09ccfd618a5c59d5a9eed34c"),
		new("craft", "Carpentry and joinery/build checker pine lockable door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_gmt_lockable_door_668"], ItemSeederOwnershipPolicy.StockAggregate, "53af21787cc0cb08d0cc4453e8a1ab0688311129ab1f39cc9629bc5ecd0209e2"),
		new("craft", "Carpentry and joinery/build cloud pine secure door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_bra_secure_door_555"], ItemSeederOwnershipPolicy.StockAggregate, "e29db2561572e9d52a23f26c0e834083a8538f7ca356a88e81939f85eb1165f9"),
		new("craft", "Carpentry and joinery/build cloud teak wicket gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_pip_wicket_210"], ItemSeederOwnershipPolicy.StockAggregate, "202462b11a9eacda0ca814cbe15d186c398b9139253b0bd02e08530501c2ef31"),
		new("craft", "Carpentry and joinery/build cloud wood secure door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_secure_door_60"], ItemSeederOwnershipPolicy.StockAggregate, "8a2e82d39ab5efc92d711753cf6a7f191db538f33a5533a044b37212cce0a7ac"),
		new("craft", "Carpentry and joinery/build cloud wood shuttered opening", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_saf_shutter_214"], ItemSeederOwnershipPolicy.StockAggregate, "fc2348e77aa0bae8a2205553971193ece4137dc4ddccbfb0aa7ed66271d97216"),
		new("craft", "Carpentry and joinery/build deep brass-vessel shelf stand", "crafts", ["renaissance"], ["item:renaissance_furniture_south_asian_merchant_vessel_shelf"], ItemSeederOwnershipPolicy.StockAggregate, "df3cc66ad4d6ec1ff6de4e34fc63ff88c271b105c069ea65d8f79e1725ff8c2b"),
		new("craft", "Carpentry and joinery/build fitted brassweight trader's box", "crafts", ["renaissance"], ["item:renaissance_trade_west_african_court_brassweight_box"], ItemSeederOwnershipPolicy.StockAggregate, "a57e6212573a1f6e5766f4c5dfec590efd553b6e297d11e79bf26672cb906029"),
		new("craft", "Carpentry and joinery/build fixed tankard-washing vat", "crafts", ["renaissance"], ["item:renaissance_furniture_tavern_inn_tankard_washing_vat"], ItemSeederOwnershipPolicy.StockAggregate, "d59a219ee9b4775adbe7134442b1e2966bbbdec28fd3a22d062bdb8e57f7b8fb"),
		new("craft", "Carpentry and joinery/build flower-head cedar secure door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_secure_door_137"], ItemSeederOwnershipPolicy.StockAggregate, "f5d64a37f30d600cc48650600b82158170e85d9e4f9886bf68d585e638fa0c25"),
		new("craft", "Carpentry and joinery/build flower-head oak lockable door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_kongo_lockable_door_459"], ItemSeederOwnershipPolicy.StockAggregate, "2ffd1a0d6f8a22a2c94bdc576ed8640721ae602b0d364ed41ca38f62a20e4766"),
		new("craft", "Carpentry and joinery/build flower-head oak service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_hatch_13"], ItemSeederOwnershipPolicy.StockAggregate, "db414d51b2976002dd8d84a5c6c609e249a1d886cca4f1f753f0b71e8cbaf6f4"),
	];
}
