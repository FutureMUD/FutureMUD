// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch005() =>
	[
		new("component", "PreparedFood_Catalogue_renaissance_food_mughal_stuffed_sweet_flatbread", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "14aa72dbd79aff5d935329ce05a6536eb65f5c6d1fd199001448e84663dd861f"),
		new("component", "PreparedFood_Catalogue_renaissance_food_ottoman_baklava_palace_tray", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0ba12275962a3244670320a658e1fce9ffbcb524a5be484627df6250bb93040"),
		new("component", "PreparedFood_Catalogue_renaissance_food_safavid_fruit_leather_rollups", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6889059cffaac00a90af07f65d47aa3f0f3a1c93a001b0e86d3ac1954b2e7c7c"),
		new("component", "PreparedFood_Catalogue_renaissance_food_songhai_cavalry_meat_grain_cake", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c53adfe4fb4746e53749a1b2e6c3439248ce0d08c8564082a87b75a93d57318b"),
		new("craft", "Agriculture, forestry and fishing/make folding cotton basket for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0049_folding_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0049_folding_basket"], ItemSeederOwnershipPolicy.StockAggregate, "fcb008d7a8b579983f42ddd99f645d56018b1170f93792d80878f1c35f105c7a"),
		new("craft", "Agriculture, forestry and fishing/make heavy-duty oak hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0032_heavy_duty_hoe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0032_heavy_duty_hoe"], ItemSeederOwnershipPolicy.StockAggregate, "8872bbebe198cb4192e90bdd3c34053152754f645e7539d31565fcd1e248f5a5"),
		new("craft", "Armourcrafting/make bronze muscle cuirass pattern 2", "crafts", ["antiquity"], ["item:antiquity_roman_bronze_muscle_cuirass"], ItemSeederOwnershipPolicy.StockAggregate, "96d4230c8f81134cadf92fe04a3cb389052b45f008ea8dfc816e217380f547e2"),
		new("craft", "Armourcrafting/make bronze scale corselet", "crafts", ["antiquity"], ["item:antiquity_persian_bronze_scale_corselet"], ItemSeederOwnershipPolicy.StockAggregate, "1c0c64d6712e021a6cb88390654ef082b5445f95ca1214cd810205e1351826a8"),
		new("craft", "Armourcrafting/make iron lamellar greaves", "crafts", ["antiquity"], ["item:antiquity_persian_lamellar_greaves"], ItemSeederOwnershipPolicy.StockAggregate, "3ee2aa6455c4f38ac553298688548f2b2886d966906d3468ce5be6097554cd09"),
		new("craft", "Armourcrafting/make iron mail lorica", "crafts", ["antiquity"], ["item:antiquity_roman_lorica_hamata_mail_shirt"], ItemSeederOwnershipPolicy.StockAggregate, "0af9f7c27326a942d6d72ff5d3fb0de827ea87c110b9a16550c4a0dad39aac8b"),
		new("craft", "Armourcrafting/make large round aspis", "crafts", ["antiquity"], ["item:antiquity_shield_hellenic_aspis"], ItemSeederOwnershipPolicy.StockAggregate, "2f1e148d3d2d628fad8ccc2798aff80c5b0491b63c8603c7c77671f7c60347f1"),
		new("craft", "Armourcrafting/make open bronze war helmet", "crafts", ["antiquity"], ["item:antiquity_anatolian_ionian_bronze_helmet"], ItemSeederOwnershipPolicy.StockAggregate, "45f97304031378f9ded80432de92ef71389ddd98aa33e150f00afc2f9008191a"),
		new("craft", "Armourcrafting/make round bossed war shield", "crafts", ["antiquity"], ["item:antiquity_shield_celtic_round_bossed_shield"], ItemSeederOwnershipPolicy.StockAggregate, "b82058ea92b9ab6df49229eacad0983b2500227ee0a446a5db7293f45b5b39bf"),
		new("craft", "Armourcrafting/raise wrought iron helmet bowl stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "399568eb88a1ca28774f1583618a62d10eb0e65f1b69b5518752ff4b78fb35f2"),
		new("craft", "Basketry/make merchant contract bundle", "crafts", ["antiquity"], ["item:antiquity_merchant_contract_bundle"], ItemSeederOwnershipPolicy.StockAggregate, "3b8897e0203b4f918f4e6362167e8e2f2ef9ce594c165e9ab662e07d38a8bf79"),
		new("craft", "Blacksmithing/cast bronze vessel blank", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "569bc2e3d6ce24a3b8bf9d14a35cf74a92c9852d1a174a70a17155e606ed42d8"),
		new("craft", "Blacksmithing/cast wrought iron vessel blank", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7b7dd680978579396cff746cb9b82999ae8597a0dc8826e76b6c9099ec89938"),
		new("craft", "Blacksmithing/forge bronze door hardware stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2e7fe876200d2773ce203ec9d04dfcf143cbf302f5ed3b7a5d6078e38a576cd"),
		new("craft", "Blacksmithing/forge wrought iron door hardware stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e82c41db73e7cd888b5fee5e960fd397f4743e45f1f1b81b34bbcaaa32bd3bc"),
		new("craft", "Blacksmithing/make bronze animal headband", "crafts", ["antiquity"], ["item:jewellery_bronze_animal_headband"], ItemSeederOwnershipPolicy.StockAggregate, "bd9b10b1ff92bd34e275a8304c112137a4bc3cafaf08087363ab25a8b58cf1a6"),
		new("craft", "Blacksmithing/make bronze drop-loop earrings", "crafts", ["antiquity"], ["item:jewellery_bronze_drop_loop_earrings"], ItemSeederOwnershipPolicy.StockAggregate, "c798ad73e7d2107464a760ebef207b15dce5ddb4e685ccef3e46d324cc4cff47"),
		new("craft", "Blacksmithing/make bronze leather edge beveller", "crafts", ["antiquity"], ["item:antiquity_bronze_edge_beveller"], ItemSeederOwnershipPolicy.StockAggregate, "72e6ec364e5b81479f15ae2b5cda3c728ffd958100fe25a67b3309f6c2082029"),
		new("craft", "Blacksmithing/make bronze loop earrings", "crafts", ["antiquity"], ["item:jewellery_bronze_loop_earrings"], ItemSeederOwnershipPolicy.StockAggregate, "54c5d3eebb6ea8188c3fecd6858a20cd848a289662f4fde9ab65825e4d1c554b"),
		new("craft", "Blacksmithing/make bronze oracular tripod", "crafts", ["antiquity"], ["item:antiquity_oracular_tripod"], ItemSeederOwnershipPolicy.StockAggregate, "c5fc8b9bdef3200402cf1a5571085acaf020c06149c20535c047f4d937c963d0"),
		new("craft", "Blacksmithing/make bronze portable gnomon", "crafts", ["antiquity"], ["item:antiquity_bronze_portable_gnomon"], ItemSeederOwnershipPolicy.StockAggregate, "ea3325c4c114d9801e939ddf362978962c69d4154a1a09666958360a0fca5e05"),
		new("craft", "Blacksmithing/make bronze sistrum", "crafts", ["antiquity"], ["item:antiquity_sistrum"], ItemSeederOwnershipPolicy.StockAggregate, "fc9495519922e4d62e144f2b23772e2b38ace46dd016d1b1b1bfa59f2d48436b"),
		new("craft", "Blacksmithing/make bronze tapered anklets", "crafts", ["antiquity"], ["item:jewellery_bronze_tapered_anklets"], ItemSeederOwnershipPolicy.StockAggregate, "2817a102581f3a1baf06aec83f614286c49d1f3edb8bb9bc47b9cc8396d24d21"),
		new("craft", "Blacksmithing/make bronze winged fibula", "crafts", ["antiquity"], ["item:jewellery_bronze_winged_fibula"], ItemSeederOwnershipPolicy.StockAggregate, "208750774fe843bc1b760eb78b26101750c774d29d717ef5a3844ccef3204eb9"),
		new("craft", "Blacksmithing/make bronze wire ring", "crafts", ["antiquity"], ["item:jewellery_bronze_wire_ring"], ItemSeederOwnershipPolicy.StockAggregate, "d543277f567e960e4e09723b6275aeec75fdecd11913465c149ec3cd66dcd2bf"),
		new("craft", "Blacksmithing/make bronze workshop chisel", "crafts", ["antiquity"], ["item:antiquity_bronze_workshop_chisel"], ItemSeederOwnershipPolicy.StockAggregate, "62b819d7242a42213ef18d2ead8a98bacbb75dbcc5322450e982bc6f19e484df"),
		new("craft", "Blacksmithing/make bronze-clad civic door", "crafts", ["antiquity"], ["item:antiquity_bronze_clad_civic_door"], ItemSeederOwnershipPolicy.StockAggregate, "f5bd41a33d760a277cb33c35645fd0be0cb69deb585a6c809cb67966cbc4fce8"),
		new("craft", "Bonecarving/make bone hair pin", "crafts", ["antiquity"], ["item:jewellery_bone_hair_pin"], ItemSeederOwnershipPolicy.StockAggregate, "0794d67cc55173c1bee183abdee009d7369c2587794d689cd5dc97d745329be9"),
		new("craft", "Bookbinding/bind small parchment codex", "crafts", ["antiquity"], ["item:antiquity_parchment_codex"], ItemSeederOwnershipPolicy.StockAggregate, "e43cca81e6c74929788257c7bdeb787f02e74466a74a5f030d74911ad0e0e556"),
		new("craft", "Carpentry and joinery/build arched-top heavy-framed oak bookcase", "crafts", ["renaissance"], ["item:renaissance_furniture_col_bookcase_01"], ItemSeederOwnershipPolicy.StockAggregate, "c3fc2f19a0ed7547eded3a5658a0f99a77caf03935214df69394f904d6cd7157"),
		new("craft", "Carpentry and joinery/build bird cedar lockable door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_lockable_door_395"], ItemSeederOwnershipPolicy.StockAggregate, "e0b765a1f42a5b01f87bed0044292feaa4d4dc6cd6dfd02bf476429975af71ab"),
		new("craft", "Carpentry and joinery/build bird oak courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_jpn_courtyard_gate_288"], ItemSeederOwnershipPolicy.StockAggregate, "4d57ee4900afabe14443f5092d347e91e493ed0bc217428f4add4261b60d25ef"),
		new("craft", "Carpentry and joinery/build bird pine braced door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_braced_door_287"], ItemSeederOwnershipPolicy.StockAggregate, "d9dfac793d6640388ae25814e31430a6c01c0540955c12048333d84f2a9841ca"),
		new("craft", "Carpentry and joinery/build braced Andean ceramic-jar stand", "crafts", ["renaissance"], ["item:renaissance_furniture_andean_storehouse_jar_stand"], ItemSeederOwnershipPolicy.StockAggregate, "32a6e04fd3644b651cd0dcf86af5d53581e05ca0a8d93d47bdcbad45a5b98e67"),
		new("craft", "Carpentry and joinery/build braided cedar courtyard gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_sai_courtyard_gate_456"], ItemSeederOwnershipPolicy.StockAggregate, "255f8e2d126fbbb0ad7534b226544c22ec72500874846dcf3b1d1e07c654a8b6"),
		new("craft", "Carpentry and joinery/build braided cedar door leaf pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_aca_panel_door_421"], ItemSeederOwnershipPolicy.StockAggregate, "d753cd80494eb20bcb69dda61c02eee2690d2d9b560a8bd529f550b17268236d"),
	];
}
