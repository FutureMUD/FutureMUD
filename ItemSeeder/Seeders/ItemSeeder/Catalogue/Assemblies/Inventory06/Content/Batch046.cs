// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch046() =>
	[
		new("item", "industrialised_weapons_police_military_0056_portable_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "faaabff2a4bea7cfe2d2e4776d7d7ebbb2029245ffdc6ea932d0b75c4140ae59"),
		new("item", "industrialised_weapons_police_military_0070_workshop_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "3f175683f0b8f982a4f1df568fecf32b97fae57a2f752e332b9944e135b430a3"),
		new("item", "industrialised_weapons_police_military_0096_service_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "877e8a989186debc033068085edecb6eafe4170b3edec6d1448c475d807ddbf6"),
		new("item", "industrialised_weapons_police_military_0119_economy_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "566bb0a6b240aee2d2de1bbad3eb0dedbfced926cffd668df1f70c7e1a4230bb"),
		new("item", "industrialised_weapons_police_military_0142_stackable_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "9487268e61df45525705c279b98bc3cf87c2d2919ab1e16ff695cd06c690b9b9"),
		new("item", "industrialised_weapons_police_military_0161_countertop_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "26ae5b43a84a628fe68bc3d1ad58562e01dbe05727bd4b1e2a9611c4e423d0cf"),
		new("item", "industrialised_weapons_police_military_0165_countertop_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "8df00863f247b3e30f1cd0c8e6d1aaff34e7686125ded24d0825ee6d5f77fa08"),
		new("item", "industrialised_weapons_police_military_0166_countertop_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbb58617f46c4852f5b84fcdae65aa9bd511374dd07dcfd7f445742aa3273690"),
		new("item", "industrialised_weapons_police_military_0178_wall_mounted_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "a293314fb93cc08b6d2e1ac2570c73bee0ebc2cc6b6c45b2bfe80e73c5469c3c"),
		new("item", "industrialised_weapons_police_military_0199_utility_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e29f04a8f59fcc112a1d5ffe93099af8ab4ca2cd44954478a31d24377b7345a4"),
		new("item", "industrialised_weapons_police_military_0206_plain_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "f27e3c53f73165596312048528f7fad90404d4926155b8b8ec29b8478ac21217"),
		new("item", "industrialised_weapons_police_military_0228_compact_helmet", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e85b8caa6102a963df13a9cfe78e6cb2203dd68fa4c132a7d7b0253e53ddb3f"),
		new("item", "industrialised_weapons_police_military_0262_workshop_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1ba6b5f24117eb0f25efe499c632b8d06ca2d1a73ae470c8c59ad476f4ce381"),
		new("item", "jewellery_bone_scarab_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8868848d6f8d63c3b9381f53daf509ba2db86714e500398d4f74bd177a61bab"),
		new("item", "jewellery_bronze_link_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "856e4f0ac3eb932c65f6b5a849320bee5cde39f8c8a3614bdd3745bae5a3fc3a"),
		new("item", "jewellery_gold_bossed_circlet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1225b9b2dfd2ae513155f68bce1f05b6e2e569ae999216bda510d51008c6f79d"),
		new("item", "jewellery_gold_carnelian_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ed67d760e0aa2b7070b59e70827d553a17ecbd8fef6544fe4ddf6b9786e8354"),
		new("item", "jewellery_gold_griffin_armlet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ffd91acc7eec177b2213e423fbf1f7f413d89d08bbfd47121b64acbe381ed2d"),
		new("item", "jewellery_gold_leaf_diadem", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7a4d6a1517c4c3a95b14033a51a752cbd748cfbea771922040dacae84ae33b7"),
		new("item", "jewellery_gold_openwork_bracelets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e46fbf53dd484da0c0f1768c815d9854751b741f8d4db27bff1f338d4610a1b"),
		new("item", "medieval_bone_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6373c1c6d617057b3f976ce7b6780a572e7c90c6391cd7d64442d32af27d6638"),
		new("item", "medieval_bronze_bracelets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8d9fc6e135521bd02a05342897b4eece79df05b99f7b4b01aac9c84c25ee783"),
		new("item", "medieval_carolingian_frankish_tablet_banded_alms_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9c8c91e8f101cdd71c1775ce5446594f25fa9a236f36f5e693d96c2ea14ff5b"),
		new("item", "medieval_child_wool_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "522b6019ff0c1ed17d65703f13fe5ae218ad9972713673c2284d2697d7458d56"),
		new("item", "medieval_church_kneeling_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8a51428bc182b0dd73116509c9e524682ca6913ca26cc286380af531862d1f4"),
		new("item", "medieval_church_pyx_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e96af42f55e751912e06ad8565363731a17a387fc9e94774adb6b32a271c2065"),
		new("item", "medieval_church_wall_crucifix", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bd2089908bdfd2b2dc047844e87fe301c9201d9f3667adf63121c7ab567daa1"),
		new("item", "medieval_cotton_lower_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8bcd32e561db1b03be3c7069c610556744a2d1f7fd28402a94b2e98257ac6861"),
		new("item", "medieval_daoist_painted_altar_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb3752388ae42c998599b39832f25e652e81770aa89e6e70ba34cf0e65052576"),
		new("item", "medieval_daoist_water_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "75ad94264043566fe0cea1ae3b5c260bbad9d74f1d5fbe7d8609f657c2c91f42"),
		new("item", "medieval_decor_east_asian_painted_paper_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "97756d4f791d98f21ac9e61fab20dea79b22ec8ccba68d4b38c0d9befcb172a4"),
		new("item", "medieval_decor_hide_floor_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2fe8dc17cdd822e75198e20db5fdbf45df1a2c50e3c0975fb003f6b57c9ca76d"),
		new("item", "medieval_decor_islamicate_pierced_brass_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f56e3ece0a07683ae36aa3ce4423fe71981dbe7c3e41076a02f447e13e772e0"),
		new("item", "medieval_decor_linen_pillow_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0298838104044eb77f26764e445b35313396c3f163a3be0343a6f2a39b34d8d"),
		new("item", "medieval_decor_mediterranean_mosaic_tile_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1413fc831167d1e99972103a354ead70e658256c70597d6949a6e4032421bf70"),
		new("item", "medieval_decor_mediterranean_painted_plaster_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb27b89b0b7a417d47b1769b1bdb28bb1041e259aa23cc8189e2ae33f7a3c203"),
		new("item", "medieval_decor_northern_canvas_room_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "530fdcf1249d1c535c16d067baad376c86c69fc6e14d274397902e5148527ee0"),
		new("item", "medieval_decor_northern_checked_bed_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac1395f3ab88454d2fa90062ca5698c542bdb6e90d48ec82ba90452fb99f25df"),
		new("item", "medieval_decor_painted_ceiling_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7eb14debf6f4331dbee92f9e74e8748765257f46cce4ae4951998bc20af59404"),
		new("item", "medieval_decor_south_asian_bamboo_window_blind", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b470c34f0c5b0428e49d5729ee6cfe837b71601d4b770b5dcf69c25765dd2b88"),
	];
}
