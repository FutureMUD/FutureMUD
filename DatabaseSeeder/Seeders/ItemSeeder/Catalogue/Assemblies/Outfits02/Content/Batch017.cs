#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits02Stock
{
	public static CatalogueOutfitDefinition RenaissanceOutfitSahelianOrdinary() => new(
		Metadata: new("renaissance_outfit_sahelian_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_sahel_broad_rectangular_robe", "item:renaissance_sahel_narrow_longtunic", "item:renaissance_sahel_veryfull_trousers", "item:renaissance_shared_clothing_leather_sandals", "item:renaissance_shared_clothing_soft_brimless_cap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Sahelian Islamic Ordinary Dress",
		Description: "Admission: Sahelian Islamic, 1450-1600; narrower local gate required. Purpose: ordinary urban, caravan, learned, merchant, or household dress.",
		Items:
		[
			new("renaissance_sahel_narrow_longtunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_sahel_veryfull_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_sahel_broad_rectangular_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_soft_brimless_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitSahelianStatus() => new(
		Metadata: new("renaissance_outfit_sahelian_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_sahel_broad_rectangular_robe", "item:renaissance_sahel_embroidered_necktunic", "item:renaissance_sahel_scholar_turban", "item:renaissance_sahel_soft_ridingboots", "item:renaissance_sahel_veryfull_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Sahelian Islamic Scholar-Court Dress",
		Description: "Admission: Sahelian Islamic, 1450-1600; scholar/court gate. Purpose: scholar, court, legal, diplomatic, or senior merchant dress.",
		Items:
		[
			new("renaissance_sahel_embroidered_necktunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_sahel_veryfull_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_sahel_broad_rectangular_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_sahel_scholar_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_sahel_soft_ridingboots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitRedSeaOrdinary() => new(
		Metadata: new("renaissance_outfit_red_sea_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_redsea_full_shoulderwrap", "item:renaissance_redsea_narrow_cotton_tunic", "item:renaissance_redsea_rawhide_sandals", "item:renaissance_redsea_white_headhood"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Red Sea Highland Ordinary Dress",
		Description: "Admission: Red Sea, 1450-1600; narrower local gate required. Purpose: ordinary highland, road, household, or rural dress.",
		Items:
		[
			new("renaissance_redsea_narrow_cotton_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_redsea_full_shoulderwrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_redsea_white_headhood", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_redsea_rawhide_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitRedSeaStatus() => new(
		Metadata: new("renaissance_outfit_red_sea_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_redsea_embroidered_shouldercape", "item:renaissance_redsea_full_shoulderwrap", "item:renaissance_redsea_long_courtshirt", "item:renaissance_redsea_quilted_white_ridinghood", "item:renaissance_redsea_rawhide_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Red Sea Court-Religious Dress",
		Description: "Admission: Red Sea, 1450-1600; court or institution gate. Purpose: court, church-associated, diplomatic, or ceremonial dress.",
		Items:
		[
			new("renaissance_redsea_long_courtshirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_redsea_full_shoulderwrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_redsea_embroidered_shouldercape", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_redsea_quilted_white_ridinghood", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_redsea_rawhide_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitIndianOceanOrdinary() => new(
		Metadata: new("renaissance_outfit_indian_ocean_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_indianocean_embroidered_roundcap", "item:renaissance_indianocean_sewn_wrapskirt", "item:renaissance_indianocean_short_coastaltunic", "item:renaissance_indianocean_toeloop_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Indian Ocean Port Ordinary Dress",
		Description: "Admission: Indian Ocean, 1450-1600; narrower port gate required. Purpose: ordinary port, sailor, artisan, merchant, or household dress.",
		Items:
		[
			new("renaissance_indianocean_short_coastaltunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_sewn_wrapskirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_embroidered_roundcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_toeloop_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitIndianOceanStatus() => new(
		Metadata: new("renaissance_outfit_indian_ocean_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_indianocean_long_coastalrobe", "item:renaissance_indianocean_open_merchantcoat", "item:renaissance_indianocean_port_turban", "item:renaissance_indianocean_stitched_coastal_slippers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Indian Ocean Merchant Dress",
		Description: "Admission: Indian Ocean, 1450-1600; merchant/status gate. Purpose: merchant, broker, scholar, diplomatic, or ceremonial dress.",
		Items:
		[
			new("renaissance_indianocean_long_coastalrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_open_merchantcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_port_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_stitched_coastal_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitMesoamericanOrdinary() => new(
		Metadata: new("renaissance_outfit_mesoamerican_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_mesoamerican_panelled_loincloth", "item:renaissance_mesoamerican_shouldertied_cloak", "item:renaissance_mesoamerican_woven_fibresandals", "item:renaissance_mesoamerican_woven_headcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Mesoamerican Ordinary Dress",
		Description: "Admission: Mesoamerican, 1450-1600; narrower polity/community gate required. Purpose: ordinary household, agricultural, market, or porter dress.",
		Items:
		[
			new("renaissance_mesoamerican_panelled_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_shouldertied_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_woven_headcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_woven_fibresandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitMesoamericanStatus() => new(
		Metadata: new("renaissance_outfit_mesoamerican_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_mesoamerican_featheredge_crown", "item:renaissance_mesoamerican_full_ceremonialtunic", "item:renaissance_mesoamerican_heelstrap_sandals", "item:renaissance_mesoamerican_painted_barkclothcape"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Mesoamerican Office-Ceremonial Dress",
		Description: "Admission: Mesoamerican, 1450-1600; office/ritual/court gate. Purpose: office, court, diplomatic, ritual, or military-status dress.",
		Items:
		[
			new("renaissance_mesoamerican_full_ceremonialtunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_painted_barkclothcape", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_featheredge_crown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_heelstrap_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitAndeanOrdinary() => new(
		Metadata: new("renaissance_outfit_andean_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_andean_braided_fibresandals", "item:renaissance_andean_fringed_camelidshawl", "item:renaissance_andean_straight_sleevelesstunic", "item:renaissance_andean_woven_earflapcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Andean Highland Ordinary Dress",
		Description: "Admission: Andean, 1450-1600; narrower polity/community gate required. Purpose: ordinary highland, labour, messenger, or household dress.",
		Items:
		[
			new("renaissance_andean_straight_sleevelesstunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_fringed_camelidshawl", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_woven_earflapcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_braided_fibresandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitAndeanStatus() => new(
		Metadata: new("renaissance_outfit_andean_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_andean_ceremonial_broadmantle", "item:renaissance_andean_fine_tapestrytunic", "item:renaissance_andean_fringed_ceremonialapron", "item:renaissance_andean_long_headcloth", "item:renaissance_andean_rawhide_roadsandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Andean State-Ceremonial Dress",
		Description: "Admission: Andean, 1450-1600; state/office/ritual gate. Purpose: state, court, office, diplomatic, or ceremonial dress.",
		Items:
		[
			new("renaissance_andean_fine_tapestrytunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_ceremonial_broadmantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_fringed_ceremonialapron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_long_headcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_rawhide_roadsandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitCaribbeanContactOrdinary() => new(
		Metadata: new("renaissance_outfit_caribbean_contact_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_caribbean_cotton_loinapron", "item:renaissance_caribbean_sleeveless_cottontunic", "item:renaissance_caribbean_woven_fibresandals", "item:renaissance_caribbean_woven_shouldercape"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Caribbean Local-Continuity Dress",
		Description: "Admission: Caribbean Contact, locally anchored pre-contact or contact date. Purpose: local-continuity household, work, or travel dress.",
		Items:
		[
			new("renaissance_caribbean_cotton_loinapron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_caribbean_sleeveless_cottontunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_caribbean_woven_shouldercape", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_caribbean_woven_fibresandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitCaribbeanContactStatus() => new(
		Metadata: new("renaissance_outfit_caribbean_contact_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_caribbean_cotton_wrapskirt", "item:renaissance_caribbean_gathered_rawhide_shoes", "item:renaissance_caribbean_sleeveless_cottontunic", "item:renaissance_caribbean_woven_shouldercape", "item:renaissance_shared_clothing_cloth_headwrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Caribbean Contact Status Dress",
		Description: "Admission: Caribbean Contact, explicit local/contact date; status gate. Purpose: local status, diplomatic, ritual, dance, or contact-hybrid dress.",
		Items:
		[
			new("renaissance_caribbean_cotton_wrapskirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_caribbean_sleeveless_cottontunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_caribbean_woven_shouldercape", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_cloth_headwrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_caribbean_gathered_rawhide_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitColonialAtlanticOrdinary() => new(
		Metadata: new("renaissance_outfit_colonial_atlantic_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_contact_broadbrim_sunhood", "item:renaissance_contact_hybrid_lacedtunic", "item:renaissance_contact_tradecloth_panelskirt", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Colonial Atlantic Contact Dress",
		Description: "Admission: Colonial Atlantic, explicit post-contact date and production-status gate. Purpose: locally made hybrid, trade-cloth, mission-labour, or port dress.",
		Items:
		[
			new("renaissance_contact_hybrid_lacedtunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_contact_tradecloth_panelskirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_contact_broadbrim_sunhood", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitColonialAtlanticStatus() => new(
		Metadata: new("renaissance_outfit_colonial_atlantic_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_contact_mission_catechistrobe", "item:renaissance_shared_clothing_close_cloth_cap", "item:renaissance_shared_clothing_long_undershirt", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Colonial Atlantic Mission Dress",
		Description: "Admission: Colonial Atlantic, explicit post-contact mission/institution gate. Purpose: mission issue, adopted, imposed, or locally made institutional dress.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_contact_mission_catechistrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_close_cloth_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitGlobalMaritimeOrdinary() => new(
		Metadata: new("renaissance_outfit_global_maritime_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_globalmaritime_knotted_headscarf", "item:renaissance_maritime_knitted_waistcoat", "item:renaissance_maritime_loose_decktrousers", "item:renaissance_maritime_sleeveless_decksmock", "item:renaissance_shared_clothing_ropesole_deckshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Global Maritime Deck Dress",
		Description: "Admission: Global Maritime, 1450-1600; crew-local admission required. Purpose: ordinary sailor, fisher, dockworker, or cargo-work dress.",
		Items:
		[
			new("renaissance_maritime_sleeveless_decksmock", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_maritime_loose_decktrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_maritime_knitted_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_globalmaritime_knotted_headscarf", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_ropesole_deckshoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitGlobalMaritimeStatus() => new(
		Metadata: new("renaissance_outfit_global_maritime_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_maritime_canvas_watchcoat", "item:renaissance_maritime_loose_decktrousers", "item:renaissance_maritime_tarred_brimhat", "item:renaissance_maritime_tarred_highshoes", "item:renaissance_shared_clothing_leather_gloves", "item:renaissance_western_open_neck_work_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Global Maritime Officer Dress",
		Description: "Admission: Global Maritime, 1450-1600; officer/pilot/merchant gate. Purpose: officer, pilot, factor, merchant, or senior shipboard dress.",
		Items:
		[
			new("renaissance_western_open_neck_work_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_maritime_loose_decktrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_maritime_canvas_watchcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_leather_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_maritime_tarred_brimhat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_maritime_tarred_highshoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayFieldLabourer() => new(
		Metadata: new("renaissance_outfit_overlay_field_labourer", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_straw_brimmed_hat", "item:renaissance_western_dress_apron", "item:renaissance_western_hobnailed_fieldshoes", "item:renaissance_western_open_neck_work_shirt", "item:renaissance_western_work_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western Field Labourer",
		Description: "Admission: Western European Renaissance, 1450-1600; field-work gate. Purpose: field labourer minimum family.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_open_neck_work_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_work_smock", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_dress_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_straw_brimmed_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_hobnailed_fieldshoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayArtisan() => new(
		Metadata: new("renaissance_outfit_overlay_artisan", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_close_cloth_cap", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_low_leather_shoes", "item:renaissance_western_dress_apron", "item:renaissance_western_open_neck_work_shirt", "item:renaissance_western_sleeveless_jerkin", "item:renaissance_western_venetian_breeches"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western Urban Artisan",
		Description: "Admission: Western European Renaissance, 1450-1600; guild/workshop gate. Purpose: artisan minimum family.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_open_neck_work_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_venetian_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_sleeveless_jerkin", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_dress_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_close_cloth_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayMerchant() => new(
		Metadata: new("renaissance_outfit_overlay_merchant", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_indianocean_long_coastalrobe", "item:renaissance_indianocean_open_merchantcoat", "item:renaissance_indianocean_port_turban", "item:renaissance_indianocean_stitched_coastal_slippers", "item:renaissance_shared_clothing_leather_gloves", "item:renaissance_shared_clothing_short_undershirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Indian Ocean Port Merchant",
		Description: "Admission: Indian Ocean, 1450-1600; merchant/broker gate. Purpose: merchant minimum family.",
		Items:
		[
			new("renaissance_shared_clothing_short_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_long_coastalrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_open_merchantcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_leather_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_port_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_indianocean_stitched_coastal_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayScholarNotary() => new(
		Metadata: new("renaissance_outfit_overlay_scholar_notary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_institution_academic_robe", "item:renaissance_institution_academic_squarecap", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_western_fine_latchet_shoes", "item:renaissance_western_high_collar_shirt", "item:renaissance_western_long_skirted_doublet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western Scholar-Notary",
		Description: "Admission: Western European Renaissance, 1450-1600; university/legal/office gate. Purpose: scholar, notary, or administrator minimum family.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_high_collar_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_long_skirted_doublet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_institution_academic_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_institution_academic_squarecap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_fine_latchet_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayPrinter() => new(
		Metadata: new("renaissance_outfit_overlay_printer", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_close_cloth_cap", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_low_leather_shoes", "item:renaissance_western_dress_apron", "item:renaissance_western_open_neck_work_shirt", "item:renaissance_western_venetian_breeches"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western Printer-Book Worker",
		Description: "Admission: Western European Renaissance, 1450-1600; print workshop gate. Purpose: printer or book-worker minimum family.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_open_neck_work_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_venetian_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_dress_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_close_cloth_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlaySailor() => new(
		Metadata: new("renaissance_outfit_overlay_sailor", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_globalmaritime_knotted_headscarf", "item:renaissance_maritime_canvas_watchcoat", "item:renaissance_maritime_knitted_waistcoat", "item:renaissance_maritime_loose_decktrousers", "item:renaissance_maritime_sleeveless_decksmock", "item:renaissance_shared_clothing_ropesole_deckshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Global Maritime Sailor",
		Description: "Admission: Global Maritime, 1450-1600; crew-local gate. Purpose: sailor or dockworker minimum family.",
		Items:
		[
			new("renaissance_maritime_sleeveless_decksmock", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_maritime_loose_decktrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_maritime_knitted_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_maritime_canvas_watchcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_globalmaritime_knotted_headscarf", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_ropesole_deckshoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayApothecary() => new(
		Metadata: new("renaissance_outfit_overlay_apothecary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_institution_physicians_roundcap", "item:renaissance_shared_clothing_cloth_gloves", "item:renaissance_western_dress_apron", "item:renaissance_western_fine_latchet_shoes", "item:renaissance_western_long_shift", "item:renaissance_western_loose_house_gown"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western Apothecary-Medical Worker",
		Description: "Admission: Western European Renaissance, 1450-1600; medical/guild gate. Purpose: apothecary or medical-worker minimum family.",
		Items:
		[
			new("renaissance_western_long_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_loose_house_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_dress_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_cloth_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_institution_physicians_roundcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_fine_latchet_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayArtistMusician() => new(
		Metadata: new("renaissance_outfit_overlay_artist_musician", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_western_dress_apron", "item:renaissance_western_feathered_velvet_bonnet", "item:renaissance_western_fine_latchet_shoes", "item:renaissance_western_gathered_linen_shirt", "item:renaissance_western_sleeveless_jerkin", "item:renaissance_western_venetian_breeches"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western Court Artist-Musician",
		Description: "Admission: Western European Renaissance, 1450-1600; court/performance gate. Purpose: artist or musician minimum family.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_gathered_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_venetian_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_sleeveless_jerkin", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_dress_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_feathered_velvet_bonnet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_fine_latchet_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayActorPageant() => new(
		Metadata: new("renaissance_outfit_overlay_actor_pageant", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_ceremonial_mantle", "item:renaissance_shared_clothing_detachable_sleeves", "item:renaissance_shared_clothing_half_mask", "item:renaissance_shared_clothing_short_undershirt", "item:renaissance_western_laced_buskins", "item:renaissance_western_paned_trunk_hose"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western Actor-Pageant Costume",
		Description: "Admission: Western European Renaissance, 1500-1600; performance/pageant gate. Purpose: actor or pageant-participant minimum family.",
		Items:
		[
			new("renaissance_shared_clothing_short_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_paned_trunk_hose", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_detachable_sleeves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_ceremonial_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_half_mask", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_laced_buskins", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayGuardServant() => new(
		Metadata: new("renaissance_outfit_overlay_guard_servant", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_ottoman_collarless_inner_shirt", "item:renaissance_ottoman_full_salwar_trousers", "item:renaissance_ottoman_short_cavalry_jacket", "item:renaissance_ottoman_short_fitted_vest", "item:renaissance_ottoman_small_court_turban", "item:renaissance_shared_clothing_ankle_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Ottoman Court Guard-Servant",
		Description: "Admission: Ottoman Islamicate, 1450-1600; court/service-household gate. Purpose: guard or court-servant minimum family.",
		Items:
		[
			new("renaissance_ottoman_collarless_inner_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_full_salwar_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_short_fitted_vest", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_short_cavalry_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_small_court_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_ankle_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOverlayReligiousFunctionary() => new(
		Metadata: new("renaissance_outfit_overlay_religious_functionary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_institution_closed_monastic_sandals", "item:renaissance_institution_full_cowl", "item:renaissance_institution_monastic_scapular", "item:renaissance_institution_plain_cassock", "item:renaissance_shared_clothing_close_cloth_cap", "item:renaissance_shared_clothing_long_undershirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western Monastic Functionary",
		Description: "Admission: Western European Renaissance, 1450-1600; named monastic institution gate. Purpose: culture-specific religious-functionary minimum family.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_institution_plain_cassock", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_institution_monastic_scapular", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_institution_full_cowl", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_close_cloth_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_institution_closed_monastic_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitWesternCourtLady() => new(
		Metadata: new("renaissance_outfit_western_court_lady", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_breast_wrap", "item:renaissance_western_fine_latchet_shoes", "item:renaissance_western_fitted_court_gown", "item:renaissance_western_fitted_dress_gloves", "item:renaissance_western_french_hood", "item:renaissance_western_full_petticoat", "item:renaissance_western_partlet", "item:renaissance_western_spanish_farthingale", "item:renaissance_western_square_neck_chemise", "item:renaissance_western_stiffened_pair_of_bodies"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western court lady in structured formal dress",
		Description: "Admission: western European court or highest urban ceremony, approximately 1500-1600, with farthingale form date-gated locally. Purpose: complete female court silhouette rather than another isolated gown.",
		Items:
		[
			new("renaissance_shared_clothing_breast_wrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_square_neck_chemise", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_stiffened_pair_of_bodies", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_full_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_spanish_farthingale", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_fitted_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_partlet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_french_hood", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_fitted_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_fine_latchet_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitLandsknechtCampaign() => new(
		Metadata: new("renaissance_outfit_landsknecht_campaign", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_ankle_boots", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_footed_stockings", "item:renaissance_western_feathered_velvet_bonnet", "item:renaissance_western_pleated_linen_shirt", "item:renaissance_western_pluderhosen", "item:renaissance_western_short_cloak", "item:renaissance_western_slashed_doublet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Central European Landsknecht campaign dress",
		Description: "Admission: German/HRE and associated mercenary contexts, approximately 1500-1570. Purpose: distinctive slashed and voluminous military clothing; armour and weapons remain separate.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_pleated_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_slashed_doublet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_pluderhosen", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_short_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_feathered_velvet_bonnet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_ankle_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitVenetianSenator() => new(
		Metadata: new("renaissance_outfit_venetian_senator", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item-skin:renaissance_skin_venetian_senatorial_red_gown", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_western_fine_latchet_shoes", "item:renaissance_western_furred_gown", "item:renaissance_western_high_collar_shirt", "item:renaissance_western_joined_hose", "item:renaissance_western_long_skirted_doublet", "item:renaissance_western_round_bonnet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Venetian senatorial civic dress",
		Description: "Admission: Venetian-style high civic office, approximately 1450-1600. Purpose: formal magistrate ensemble using a scarlet senatorial skin on the existing furred gown; the round bonnet remains a restrained black presentation.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_high_collar_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_joined_hose", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_long_skirted_doublet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_furred_gown", "renaissance_skin_venetian_senatorial_red_gown", null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_round_bonnet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_fine_latchet_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
