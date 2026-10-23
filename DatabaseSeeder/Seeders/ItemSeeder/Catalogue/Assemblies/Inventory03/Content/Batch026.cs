// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch026() =>
	[
		new("craft", "Writing and Administration/make piece of parchment pumice pattern 2", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_pumice_piece"], ItemSeederOwnershipPolicy.StockAggregate, "8958f342d2909be64d1f1111e2e41c5959a6b0862c75113c0bb337fa1a2663dc"),
		new("craft", "Writing and Administration/make rag-paper scroll", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_rag_paper_scroll"], ItemSeederOwnershipPolicy.StockAggregate, "56d6732e39261fba4965c367a22f9fbabcffa94c84438c8bac0335941f6dbb9b"),
		new("craft", "Writing and Administration/make ruled rag-paper sheet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_ruled_rag_paper_sheet"], ItemSeederOwnershipPolicy.StockAggregate, "f82175c589dee79f8acc5f6d5c4445cff3cfbdad072bfd932af06f45a27ee811"),
		new("craft", "Writing and Administration/make sealable archive chest", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_sealable_archive_chest"], ItemSeederOwnershipPolicy.StockAggregate, "7e57e94df80deece2202d44e2187038e7c826803e36c95de31ba9454987dad73"),
		new("craft", "Writing and Administration/make silk manuscript wrapper", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_silk_manuscript_wrapper"], ItemSeederOwnershipPolicy.StockAggregate, "411fd325725e38b6d57e80e1c42a2aaf0acfb1d36ca8cf1f84c3bbf787b06a50"),
		new("craft", "Writing and Administration/make small archive box", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_small_archive_box"], ItemSeederOwnershipPolicy.StockAggregate, "4f4cbc6658f89d9655accf3896f393024db8cf76ef8da02cdc6f2cd2c219506a"),
		new("craft", "Writing and Administration/make stone pigment muller", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_stone_pigment_muller"], ItemSeederOwnershipPolicy.StockAggregate, "22e104bf471f8486166dac94db52601b80b98caf8585299f6cf791872c45d82e"),
		new("craft", "Writing and administration/bind broad painted lacquer writing desk", "crafts", ["renaissance"], ["item:renaissance_furniture_pip_writing_desk_01"], ItemSeederOwnershipPolicy.StockAggregate, "445e7433a66106c0db9fdd3040e0ef2bc2f34b0f28ba7a8c6bc9d12a0f5c3fff"),
		new("craft", "Writing and administration/bind buckled courier satchel", "crafts", ["renaissance"], ["item:renaissance_writing_courier_satchel"], ItemSeederOwnershipPolicy.StockAggregate, "2f8b3eca61841a30a3103458366dfa14ada0bd7805d440e273eac0a3a60d138d"),
		new("craft", "Writing and administration/bind folding paper document wallet", "crafts", ["renaissance"], ["item:renaissance_personal_east_asian_literati_document_wallet"], ItemSeederOwnershipPolicy.StockAggregate, "3b8cd74498999bbe01648466eef2136528292ba3f425613d6af1813a1c41ccb3"),
		new("craft", "Writing and administration/bind low open-backed lacquer document drawers", "crafts", ["renaissance"], ["item:renaissance_furniture_eal_document_drawers_01"], ItemSeederOwnershipPolicy.StockAggregate, "e66fbc1e2234d3a25b624d8d515da0637b8c6f84e521e8a9a54e8c72669ee20e"),
		new("craft", "Writing and administration/bind plain-woven leather metal-ornament document wallet", "crafts", ["renaissance"], ["item:renaissance_container_and_personal_document_wallet_02"], ItemSeederOwnershipPolicy.StockAggregate, "03c3ee184098bdc2bb51c68554c39e73d529b2a21a19e54610937223ab59727b"),
		new("craft", "Writing and administration/bind quilted cotton bead-lot document wallet", "crafts", ["renaissance"], ["item:renaissance_container_ind_personal_document_wallet_03"], ItemSeederOwnershipPolicy.StockAggregate, "ccbcf7ffe7257218fd7cae33518eedfc09917a411494f8354ad0c3808c77fca9"),
		new("craft", "Writing and administration/bind sealed notarial instrument", "crafts", ["renaissance"], ["item:renaissance_writing_notarial_instrument"], ItemSeederOwnershipPolicy.StockAggregate, "76abac05822f7d4c8edd4826568c9d4026bebb4336b3d6fa95b92aa2e3b716e2"),
		new("craft", "Writing and administration/bind small slatted bamboo writing desk", "crafts", ["renaissance"], ["item:renaissance_furniture_mea_writing_desk_01"], ItemSeederOwnershipPolicy.StockAggregate, "8f7c5f7537db00070e5a5f2a2a4ff40edb74c7dc209556a98b8155732e25de3b"),
		new("item", "adjacent_antiquity_felt_hand_mitts", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "49e3c7ef092101c0bb305d241218272fba4afcabd9d793a03a9acaf15c821da5"),
		new("item", "adjacent_antiquity_fringed_wool_mantle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d009934cfc52c090ad8f7399baf8fe2fbc2750d2d4587b2d8ee80341a846790"),
		new("item", "adjacent_antiquity_plain_cotton_headcloth", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "07377acad8b0d6a6d96fb925cc90e505a37daee7ddd97033616ec9dc469fd0bd"),
		new("item", "antiquity_accessory_egyptian_linen_arrow_quiver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "85c1e9b4e7cbee0f7f3f47c55a25807ea643db3ae08acd3ed232c7da53cac580"),
		new("item", "antiquity_accessory_hellenic_kopis_sheath", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7bc67d1c8fd117f000c99990dc93c1a81a6791d5ab38e2050e9b657eedc14831"),
		new("item", "antiquity_anatolian_bronze_greaves", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f46aa456b5cba2ed7b9af5162ac801b3db58e41c0620c9f0d2b2018cae980b58"),
		new("item", "antiquity_armoury_supply_counter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e45976c6887e52d5eee583eec4b5860ccb5ff069dfa6766511a67afaf1a3fe7"),
		new("item", "antiquity_bee_smoke_pot", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ee0a2eca6cac070762efc3fca36e76bb6fd8fbf3350a01b2eb58cda6a816bb6"),
		new("item", "antiquity_bronze_forge_tongs", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7aeae0a2f0031ee8e441529b0d2fe474e538b437fa92930e587fd0050d8a1182"),
		new("item", "antiquity_bronze_surgical_probe", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2ff16751f205d8acc38339ebd3fcbc84a9baa693e696246496606b5231e78bd"),
		new("item", "antiquity_celtic_fine_riveted_mail_shirt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d1c7f54b501bb3fcd7c957da6ff4f16175922710faf04b6e65afc0f7b0c481f"),
		new("item", "antiquity_dye_vat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f44350398990b47fed467ddae15491b835d8c6e632befd77c6bf1cfc17481521"),
		new("item", "antiquity_felt_riding_mittens", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a14fffeff1d87508c2c1d99d91a2a77fc08a77da8bc5169f2bdc1e5ef21ccef3"),
		new("item", "antiquity_felt_tent_door_flap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7142fc61051848bdd0875be43c4762a0b3483041c553a44c64586d9bcc0d972"),
		new("item", "antiquity_fine_tapered_wool_trousers", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d254815834bb357ccb881e64f7dc925f527c4f74c43c41027f16cd5ebc80891"),
		new("item", "antiquity_fine_temple_linen_veil", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7e82663748aadafedc6f62b0025e409fe92ed7638874bc6d3b838a5761fcb44"),
		new("item", "antiquity_food_anatolian_stuffed_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "094286803540bac69ac6dcab6060b04663256df1668c4959fde3246214f71b66"),
		new("item", "antiquity_food_brined_fruit", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "37c5b37fb63bbcf5a28049add8048709b9e3e2e012617be5a588a96619288fc3"),
		new("item", "antiquity_food_celtic_preserved_meat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aac2280e81eff19a93ae5d8e90c772fb2d1a12d3d42abfa94cd7cec2e75afcd9"),
		new("item", "antiquity_food_celtic_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1cc4a0506c47578cbcfcb5cd32d7b1fbec0d1392dd7ae2c73db57a17243ae2c4"),
		new("item", "antiquity_food_finished_kumis_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d206c8df037cf7b58a9781f7152789cb9f60b1e08b57a280b777fe6e960cdfe"),
		new("item", "antiquity_food_hellenic_honeyed_pastry", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8439976506a622ecf8e76c0df66a8e79e3f14f514d0ec3939327e9ca928c8ea"),
		new("item", "antiquity_food_hellenic_oilseed_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2913a53065735d966b38342fe53e7efa5655ed3952aba6293589461486be48bd"),
		new("item", "antiquity_food_hellenic_sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0703725edcda7160cf75c13c9c66288f25b98bbe06e2cf7cd5dd4b761d2af96"),
		new("item", "antiquity_food_persian_preserved_meat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3abd6a9d230868424750eb40242584cf7350b0f4ba0eb1ff5a3af04a6b8ce14"),
	];
}
