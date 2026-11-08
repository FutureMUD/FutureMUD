// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch026() =>
	[
		new("craft", "Weaving/full wool garment cloth", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "237e800a46a7e4a591201bd5f775241af3ba2b31719285a482fe21f7baf7f8ed"),
		new("craft", "Writing and Administration/make birch-bark practice strip", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_birch_bark_practice_strip"], ItemSeederOwnershipPolicy.StockAggregate, "321badfaa809b771a44e9d34dd2abcec0533c1b62f46c574f13626ea396911b3"),
		new("craft", "Writing and Administration/make bundle of bamboo slips", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_bamboo_slip_bundle"], ItemSeederOwnershipPolicy.StockAggregate, "344f34c602275cda6e97dba5738d95f173b661614579c4462aac97c8d1dead8c"),
		new("craft", "Writing and Administration/make estate account roll", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_estate_roll"], ItemSeederOwnershipPolicy.StockAggregate, "d73a89b5db8c76604181d7d5e75b35a20de76f260fdb9bca2b39c78b5b4f054b"),
		new("craft", "Writing and Administration/make fine calligraphy brush", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_fine_calligraphy_brush"], ItemSeederOwnershipPolicy.StockAggregate, "417d3cac98402d16af900ce9b7077c46af1b4d21ace430e6cbbd752336145449"),
		new("craft", "Writing and Administration/make lacquered book box", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_lacquered_book_box"], ItemSeederOwnershipPolicy.StockAggregate, "18647c55da57fe43915687699d1fd6f1ee4f19ac7b02fef46929ba26679b51a2"),
		new("craft", "Writing and Administration/make lead seal matrix", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_lead_seal_matrix"], ItemSeederOwnershipPolicy.StockAggregate, "946373817a6933a79c10e2b9c066b8d617ad39c2e9176ce1544e8983e11cbd1a"),
		new("craft", "Writing and Administration/make notary's document satchel", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_notary_satchel"], ItemSeederOwnershipPolicy.StockAggregate, "93cf262e5adfc5fb222d2693ccb33d4919b2b518cdf04de7b9a966c75419df2c"),
		new("craft", "Writing and Administration/make parchment pounce bag", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_parchment_pounce_bag"], ItemSeederOwnershipPolicy.StockAggregate, "9ac7584b4c43c80c10b80c65e720aea1141fa6cff752d002bca32dbebf0b9e52"),
		new("craft", "Writing and Administration/make polished bone stylus", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_bone_stylus"], ItemSeederOwnershipPolicy.StockAggregate, "1bb510d9e4267e5a206db33d7d1505a0ba661b67a105cbd23efbbf91a0270401"),
		new("craft", "Writing and Administration/make rag-paper account slip", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_rag_paper_account_slip"], ItemSeederOwnershipPolicy.StockAggregate, "41a90e1762bf4c8af1b5f089560aeffa75fbd90576f24cd7e4025ee027741e3b"),
		new("craft", "Writing and Administration/make scholastic parchment codex", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_scholastic_codex"], ItemSeederOwnershipPolicy.StockAggregate, "1498811a4d0890682d54b4d7d02c6b62ce734918068101b43f8ff2aed404f18f"),
		new("craft", "Writing and Administration/make set of sewing support cords", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_sewing_support_cords"], ItemSeederOwnershipPolicy.StockAggregate, "5849d1e351445f9abadc3d44c2a8379a6f8f0486f8137c54fd7d32a2685edf1f"),
		new("craft", "Writing and Administration/make small clay inkwell", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_small_clay_inkwell"], ItemSeederOwnershipPolicy.StockAggregate, "efe35c7882e5b5a66d91543808b9f2da1e902c9efe04f1bd241cd511487f2d4a"),
		new("craft", "Writing and administration/bind bound pilot book", "crafts", ["renaissance"], ["item:renaissance_writing_pilot_book"], ItemSeederOwnershipPolicy.StockAggregate, "4f0952819b82366aa087b38ff81f57a31e9adefa31f50d4a2c6ab577b4ec38b2"),
		new("craft", "Writing and administration/bind massive slotted teak writing desk", "crafts", ["renaissance"], ["item:renaissance_furniture_mar_writing_desk_01"], ItemSeederOwnershipPolicy.StockAggregate, "8749ef81cdc254c0f231b4d4ad81fbdf70fc4ebbebf009280dfd328f42f92e31"),
		new("craft", "Writing and administration/bind wax-sealable coastal document chest", "crafts", ["renaissance"], ["item:renaissance_trade_west_african_court_coastal_document_chest"], ItemSeederOwnershipPolicy.StockAggregate, "54ca0d11fd1fa6166b96314cb86bed7e933b1339329775c67e17b102156acbb2"),
		new("item", "adjacent_antiquity_bordered_wool_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "13e761854f9cfe13750fe9e36998d843038404659d8eb0cf125d7c5484c1d81a"),
		new("item", "adjacent_antiquity_narrow_linen_kilt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "35e27ac803bc789bbd9b9951b4241e3288936bca9592322698315d7e6f08cbce"),
		new("item", "adjacent_antiquity_riding_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "36fa530bc5f3bd065bc7355da7459c27a11c02832ced3c6d381641861bd81967"),
		new("item", "antiquity_accessory_celtic_broad_baldric", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0161ea0c0144362ac7340cfc1be331cf8893a149fa0890ab16a499ba49867a1a"),
		new("item", "antiquity_accessory_common_long_sword_scabbard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a3b7b813d58899230e07afe7ed6ba307a915a0a04350f195d0f2e2f7b1e764e"),
		new("item", "antiquity_accessory_etruscan_spear_sling", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbc6b5e3d22abf116dbf7eb7f2262ed38e3eabebadec870cda335a7c343ee8bc"),
		new("item", "antiquity_accessory_hellenic_bolt_quiver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "96e61b187e64a7b212bf5431ad833c16cf49418eeff05becf5cde17f124ea1a4"),
		new("item", "antiquity_ammo_common_lead_sling_bullet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "21a3519efbbf91fe01e223b7d8db3be770e1499ab55a0f67124258de60025277"),
		new("item", "antiquity_anatolian_fine_winged_bronze_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b29c4edd9a9d35d7686fc9beef8f9f4b2c43bc86100a964b59b88e4c0168e33a"),
		new("item", "antiquity_barracks_roster_board", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6155d4638150241969cac66d3654d6d4540b231d0955eb790252b65efa644a80"),
		new("item", "antiquity_beaded_kohl_pouch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef8d7c638c08a165307c55f2d6bd338c70d1927aeb31ca723e435a0c4b0b0f3b"),
		new("item", "antiquity_bronze_balance_scale", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c62e07f72bbad8bc6b47b3e39372145c9859f907965282221b676fa70761a017"),
		new("item", "antiquity_bronze_brazier", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d07265dbaff70aad1d4ab840d9b9e3243c440a345545b0f251f40261160b5d09"),
		new("item", "antiquity_bronze_hide_scraper", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9424cb5ca91d6b00073ff08bde2bb361f6e336762b1a0dad65c99cea2d9b4700"),
		new("item", "antiquity_bronze_lockpick_set", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6b1bed0cf16e37bcf6336a8a7c8d4717352c10f7d9702bad5908755b42f0b3f"),
		new("item", "antiquity_bronze_pen_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "75849d6124d269f4bfb645ae376996b38549dd8a2080c9ded45c2227b1884f90"),
		new("item", "antiquity_canvas_field_stretcher", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cfd9752044251836f923969b39f9a05f5cf50e74e12e136b86f700d4be1c4a0"),
		new("item", "antiquity_celtic_crested_bronze_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "49b3a1801b6953ec91f3d60ec6fb4b5d867a131565086b35e03554f5a7f76962"),
		new("item", "antiquity_checked_wool_leg_wraps", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3ebafdd818c3c68e5410d151cbd30060d262f26ee29312ccee24b7a4d886238"),
		new("item", "antiquity_curved_back_klismos_chair", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2470fe98f9701ef0fb14d60f9fb867b987b501c802b72976a009158290dd654f"),
		new("item", "antiquity_cypress_storage_cupboard", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a8d12c2561292fcbb139c8268106b800a53b518397724833002883f025b093a"),
		new("item", "antiquity_double_aulos", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a181567a5af0fde136f341c7f4fed0c7538db8b03fc50b81f8be2c9c63fbfbf9"),
		new("item", "antiquity_egyptian_spotted_hide_priest_mantle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbf1c24b3acd40ffdb77965f576d5b3082b74ce76e230d6c8f6065e53750532b"),
	];
}
