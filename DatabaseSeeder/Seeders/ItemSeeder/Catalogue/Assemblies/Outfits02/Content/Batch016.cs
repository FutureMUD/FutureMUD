#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits02Stock
{
	public static CatalogueOutfitDefinition RenaissanceOutfitIberianOrdinary() => new(
		Metadata: new("renaissance_outfit_iberian_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_iberian_full_shipboard_breeches", "item:renaissance_iberian_knitted_sailor_cap", "item:renaissance_iberian_short_sailor_jacket", "item:renaissance_shared_clothing_ankle_boots", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_western_open_neck_work_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Iberian Atlantic Mariner Dress",
		Description: "Admission: Iberian Atlantic, 1500-1600; maritime admission. Purpose: sailor, dockworker, or coastal traveller.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_open_neck_work_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_iberian_full_shipboard_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_iberian_short_sailor_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_iberian_knitted_sailor_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_ankle_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitIberianStatus() => new(
		Metadata: new("renaissance_outfit_iberian_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_iberian_broad_crowned_hat", "item:renaissance_western_fine_latchet_shoes", "item:renaissance_western_full_circle_cloak", "item:renaissance_western_high_collar_shirt", "item:renaissance_western_short_skirted_doublet", "item:renaissance_western_venetian_breeches"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Iberian Atlantic Prosperous Dress",
		Description: "Admission: Iberian Atlantic, 1500-1600; prosperous/status gate. Purpose: merchant, officer-presentation, or household formality.",
		Items:
		[
			new("renaissance_western_high_collar_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_short_skirted_doublet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_venetian_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_full_circle_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_iberian_broad_crowned_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_fine_latchet_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitCentralEuropeanOrdinary() => new(
		Metadata: new("renaissance_outfit_central_european_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_frontier_fur_brimmed_cap", "item:renaissance_frontier_wide_hungarian_trousers", "item:renaissance_shared_clothing_ankle_boots", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_western_long_skirted_jerkin", "item:renaissance_western_open_neck_work_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Central European Town Dress",
		Description: "Admission: Central European, 1450-1600. Purpose: ordinary town, artisan, or travelling dress.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_open_neck_work_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_long_skirted_jerkin", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_wide_hungarian_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_fur_brimmed_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_ankle_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitCentralEuropeanStatus() => new(
		Metadata: new("renaissance_outfit_central_european_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_frontier_split_skirt_riding_boots", "item:renaissance_frontier_split_sleeve_noblecoat", "item:renaissance_frontier_tall_fur_hat", "item:renaissance_western_high_collar_shirt", "item:renaissance_western_pluderhosen", "item:renaissance_western_slashed_doublet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Central European Noble Dress",
		Description: "Admission: Central European, 1500-1600; court/noble gate. Purpose: court, civic office, or noble household dress.",
		Items:
		[
			new("renaissance_western_high_collar_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_slashed_doublet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_pluderhosen", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_split_sleeve_noblecoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_tall_fur_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_split_skirt_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitNorthernBalticOrdinary() => new(
		Metadata: new("renaissance_outfit_northern_baltic_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_frontier_knitted_northern_cap", "item:renaissance_shared_clothing_hooded_cloak", "item:renaissance_shared_clothing_leg_wraps", "item:renaissance_shared_clothing_long_undershirt", "item:renaissance_shared_clothing_low_leather_shoes", "item:renaissance_shared_clothing_straight_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Northern Baltic Work Dress",
		Description: "Admission: Northern Baltic, 1450-1600. Purpose: rural, port, workshop, or cold-road dress.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_straight_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_leg_wraps", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_hooded_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_knitted_northern_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitNorthernBalticStatus() => new(
		Metadata: new("renaissance_outfit_northern_baltic_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_frontier_felted_winter_boots", "item:renaissance_frontier_fur_brimmed_cap", "item:renaissance_frontier_wide_hungarian_trousers", "item:renaissance_shared_clothing_fur_lined_coat", "item:renaissance_western_high_collar_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Northern Baltic Prosperous Winter Dress",
		Description: "Admission: Northern Baltic, 1450-1600; prosperous/status gate. Purpose: merchant, civic officer, or prosperous household winter dress.",
		Items:
		[
			new("renaissance_western_high_collar_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_wide_hungarian_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_fur_lined_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_fur_brimmed_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_felted_winter_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitCentralEasternFrontierOrdinary() => new(
		Metadata: new("renaissance_outfit_central_eastern_frontier_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_frontier_belted_wool_caftan", "item:renaissance_frontier_fur_brimmed_cap", "item:renaissance_frontier_wide_hungarian_trousers", "item:renaissance_shared_clothing_ankle_boots", "item:renaissance_shared_clothing_long_undershirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Central-Eastern Frontier Ordinary Dress",
		Description: "Admission: Central Eastern Frontier, 1450-1600. Purpose: town, caravan, mounted-work, or household dress.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_wide_hungarian_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_belted_wool_caftan", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_fur_brimmed_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_ankle_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitCentralEasternFrontierStatus() => new(
		Metadata: new("renaissance_outfit_central_eastern_frontier_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_frontier_split_skirt_riding_boots", "item:renaissance_frontier_split_sleeve_noblecoat", "item:renaissance_frontier_tall_fur_hat", "item:renaissance_western_pleated_linen_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Central-Eastern Frontier Noble Dress",
		Description: "Admission: Central Eastern Frontier, 1450-1600; noble/status gate. Purpose: noble household, diplomatic, or mounted-retainer dress.",
		Items:
		[
			new("renaissance_western_pleated_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_split_sleeve_noblecoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_tall_fur_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_split_skirt_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitEasternOrthodoxNorthernOrdinary() => new(
		Metadata: new("renaissance_outfit_eastern_orthodox_northern_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_frontier_close_buttoned_longcoat", "item:renaissance_frontier_felted_winter_boots", "item:renaissance_frontier_knitted_northern_cap", "item:renaissance_shared_clothing_full_trousers", "item:renaissance_shared_clothing_straight_underrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Eastern Orthodox Northern Ordinary Dress",
		Description: "Admission: Eastern Orthodox Northern, 1450-1600. Purpose: ordinary town, household, or cold-weather travel dress.",
		Items:
		[
			new("renaissance_shared_clothing_straight_underrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_full_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_close_buttoned_longcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_knitted_northern_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_felted_winter_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitEasternOrthodoxNorthernStatus() => new(
		Metadata: new("renaissance_outfit_eastern_orthodox_northern_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_frontier_long_furred_overcoat", "item:renaissance_frontier_sleeveless_long_gown", "item:renaissance_frontier_split_skirt_riding_boots", "item:renaissance_frontier_tall_fur_hat", "item:renaissance_shared_clothing_long_undershirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Eastern Orthodox Northern Court Dress",
		Description: "Admission: Eastern Orthodox Northern, 1450-1600; court/status gate. Purpose: court, embassy, senior household, or ceremonial dress.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_sleeveless_long_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_long_furred_overcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_tall_fur_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_frontier_split_skirt_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOttomanOrdinary() => new(
		Metadata: new("renaissance_outfit_ottoman_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_ottoman_collarless_inner_shirt", "item:renaissance_ottoman_full_salwar_trousers", "item:renaissance_ottoman_pointed_slippers", "item:renaissance_ottoman_short_entari_coat", "item:renaissance_ottoman_wrapped_cap_turban", "item:renaissance_shared_clothing_drawstring_drawers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Ottoman Urban Ordinary Dress",
		Description: "Admission: Ottoman Islamicate, 1450-1600. Purpose: ordinary urban, merchant, artisan, or household dress.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_collarless_inner_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_full_salwar_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_short_entari_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_wrapped_cap_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_pointed_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitOttomanStatus() => new(
		Metadata: new("renaissance_outfit_ottoman_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_ottoman_collarless_inner_shirt", "item:renaissance_ottoman_fitted_kaftan", "item:renaissance_ottoman_full_outdoor_overrobe", "item:renaissance_ottoman_full_salwar_trousers", "item:renaissance_ottoman_high_court_turban", "item:renaissance_ottoman_pointed_slippers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Ottoman Court Dress",
		Description: "Admission: Ottoman Islamicate, 1450-1600; court/office gate. Purpose: court, office, diplomatic, or ceremonial dress.",
		Items:
		[
			new("renaissance_ottoman_collarless_inner_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_full_salwar_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_fitted_kaftan", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_full_outdoor_overrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_high_court_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_pointed_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitPersianateOrdinary() => new(
		Metadata: new("renaissance_outfit_persianate_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_persianate_close_ankle_trousers", "item:renaissance_persianate_conical_turban", "item:renaissance_persianate_long_inner_shirt", "item:renaissance_persianate_pointed_slippers", "item:renaissance_persianate_short_fitted_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Persianate Urban Ordinary Dress",
		Description: "Admission: Persianate Indo-Persian, 1501-1600. Purpose: ordinary urban, learned, merchant, or household dress.",
		Items:
		[
			new("renaissance_persianate_long_inner_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_persianate_close_ankle_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_persianate_short_fitted_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_persianate_conical_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_persianate_pointed_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitPersianateStatus() => new(
		Metadata: new("renaissance_outfit_persianate_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_persianate_broad_skirt_courtrobe", "item:renaissance_persianate_closedback_embroideredslippers", "item:renaissance_persianate_long_inner_shirt", "item:renaissance_persianate_tall_turban", "item:renaissance_persianate_wide_pleated_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Persianate Safavid Court Dress",
		Description: "Admission: Persianate Indo-Persian, 1501-1600; court/status gate. Purpose: court, diplomatic, office, or ceremonial dress.",
		Items:
		[
			new("renaissance_persianate_long_inner_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_persianate_wide_pleated_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_persianate_broad_skirt_courtrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_persianate_tall_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_persianate_closedback_embroideredslippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitSouthAsianOrdinary() => new(
		Metadata: new("renaissance_outfit_south_asian_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_southasian_flat_wound_turban", "item:renaissance_southasian_pleated_waistcloth", "item:renaissance_southasian_short_sideslit_tunic", "item:renaissance_southasian_stitched_loindrawers", "item:renaissance_southasian_toepost_woodensandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance South Asian Ordinary Dress",
		Description: "Admission: South Asian, 1450-1600; local admission required. Purpose: ordinary household, market, artisan, or rural dress.",
		Items:
		[
			new("renaissance_southasian_stitched_loindrawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_short_sideslit_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_pleated_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_flat_wound_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_toepost_woodensandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitSouthAsianStatus() => new(
		Metadata: new("renaissance_outfit_south_asian_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_southasian_bunched_ankletrousers", "item:renaissance_southasian_curvedtoe_shoes", "item:renaissance_southasian_headbanded_courtveil", "item:renaissance_southasian_long_crossover_robe", "item:renaissance_southasian_short_sideslit_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance South Asian Court Dress",
		Description: "Admission: South Asian, 1526-1600; court/status gate. Purpose: court, diplomatic, prosperous, or ceremonial dress.",
		Items:
		[
			new("renaissance_southasian_bunched_ankletrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_short_sideslit_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_long_crossover_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_headbanded_courtveil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_curvedtoe_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitEastAsianLiteratiOrdinary() => new(
		Metadata: new("renaissance_outfit_east_asian_literati_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_ming_black_cloth_scholarshoes", "item:renaissance_ming_crosscollar_innerrobe", "item:renaissance_ming_long_sidefastened_jacket", "item:renaissance_ming_soft_scholarcap", "item:renaissance_ming_wide_drawstring_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Ming Literati Ordinary Dress",
		Description: "Admission: East Asian Literati, Ming-facing, 1450-1600. Purpose: scholar, clerk, merchant, or prosperous household dress.",
		Items:
		[
			new("renaissance_ming_crosscollar_innerrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_wide_drawstring_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_long_sidefastened_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_soft_scholarcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_black_cloth_scholarshoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitEastAsianLiteratiStatus() => new(
		Metadata: new("renaissance_outfit_east_asian_literati_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_ming_cloth_courtboots", "item:renaissance_ming_crosscollar_innerrobe", "item:renaissance_ming_fourcorner_scholarcap", "item:renaissance_ming_roundcollar_longrobe", "item:renaissance_ming_wide_drawstring_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Ming Official Dress",
		Description: "Admission: East Asian Literati, Ming-facing, 1450-1600; official gate. Purpose: court, examination office, diplomatic, or senior learned dress.",
		Items:
		[
			new("renaissance_ming_crosscollar_innerrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_wide_drawstring_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_roundcollar_longrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_fourcorner_scholarcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_cloth_courtboots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitJapaneseOrdinary() => new(
		Metadata: new("renaissance_outfit_japanese_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_japanese_field_trousers", "item:renaissance_japanese_lacquered_conicalhat", "item:renaissance_japanese_smallsleeve_wraprobe", "item:renaissance_japanese_splittoe_socks", "item:renaissance_japanese_wooden_clogs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Japanese Ordinary Travel Dress",
		Description: "Admission: Japanese, 1450-1600; local and status admission required. Purpose: ordinary travel, rural, messenger, or household dress.",
		Items:
		[
			new("renaissance_japanese_smallsleeve_wraprobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_field_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_splittoe_socks", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_wooden_clogs", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_lacquered_conicalhat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitJapaneseStatus() => new(
		Metadata: new("renaissance_outfit_japanese_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_japanese_full_pleated_hakama", "item:renaissance_japanese_shoulderwing_vest", "item:renaissance_japanese_smallsleeve_wraprobe", "item:renaissance_japanese_stiff_lacquered_eboshi", "item:renaissance_japanese_wooden_clogs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Japanese Formal Dress",
		Description: "Admission: Japanese, 1450-1600; court/warrior/status gate. Purpose: formal audience, court, senior retainer, or ceremonial dress.",
		Items:
		[
			new("renaissance_japanese_smallsleeve_wraprobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_full_pleated_hakama", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_shoulderwing_vest", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_stiff_lacquered_eboshi", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_wooden_clogs", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitMaritimeEastAsianOrdinary() => new(
		Metadata: new("renaissance_outfit_maritime_east_asian_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_ryukyu_pleated_wrapskirt", "item:renaissance_ryukyu_short_wrapjacket", "item:renaissance_ryukyu_widesleeve_summerrobe", "item:renaissance_shared_clothing_cloth_headwrap", "item:renaissance_shared_clothing_textile_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Ryukyuan Ordinary Dress",
		Description: "Admission: Maritime East Asian, Ryukyuan-facing, 1450-1600. Purpose: ordinary port, household, island-court service, or market dress.",
		Items:
		[
			new("renaissance_ryukyu_widesleeve_summerrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ryukyu_pleated_wrapskirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ryukyu_short_wrapjacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_cloth_headwrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_textile_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitMaritimeEastAsianStatus() => new(
		Metadata: new("renaissance_outfit_maritime_east_asian_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_ryukyu_formal_courtcap", "item:renaissance_ryukyu_pleated_wrapskirt", "item:renaissance_ryukyu_short_wrapjacket", "item:renaissance_ryukyu_widesleeve_summerrobe", "item:renaissance_shared_clothing_fine_velvet_slippers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Ryukyuan Court Dress",
		Description: "Admission: Maritime East Asian, Ryukyuan-facing, 1450-1600; court gate. Purpose: court, embassy, maritime office, or ceremonial dress.",
		Items:
		[
			new("renaissance_ryukyu_widesleeve_summerrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ryukyu_pleated_wrapskirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ryukyu_short_wrapjacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ryukyu_formal_courtcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_fine_velvet_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitSeasiaMainlandOrdinary() => new(
		Metadata: new("renaissance_outfit_seasia_mainland_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_seasia_palmleaf_conicalhat", "item:renaissance_seasia_sewn_tubeskirt", "item:renaissance_seasia_short_collarless_jacket", "item:renaissance_shared_clothing_short_undershirt", "item:renaissance_shared_clothing_textile_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance South-east Asian Mainland Ordinary Dress",
		Description: "Admission: South-east Asian Mainland, 1450-1600; local admission required. Purpose: household, market, rural, or artisan dress.",
		Items:
		[
			new("renaissance_shared_clothing_short_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_sewn_tubeskirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_short_collarless_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_palmleaf_conicalhat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_textile_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitSeasiaMainlandStatus() => new(
		Metadata: new("renaissance_outfit_seasia_mainland_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_seasia_embroidered_silk_courtshoes", "item:renaissance_seasia_long_crossfront_courtrobe", "item:renaissance_seasia_pleated_courtwaistcloth", "item:renaissance_seasia_sleeveless_courtvest", "item:renaissance_seasia_structured_headclothcrown"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance South-east Asian Mainland Court Dress",
		Description: "Admission: South-east Asian Mainland, 1450-1600; court/status gate. Purpose: court, diplomatic, ceremonial, or elite household dress.",
		Items:
		[
			new("renaissance_seasia_pleated_courtwaistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_long_crossfront_courtrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_sleeveless_courtvest", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_structured_headclothcrown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_embroidered_silk_courtshoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitMaritimeSeasiaOrdinary() => new(
		Metadata: new("renaissance_outfit_maritime_seasia_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_seasia_short_collarless_jacket", "item:renaissance_seasia_short_maritime_trousers", "item:renaissance_shared_clothing_cloth_headwrap", "item:renaissance_shared_clothing_rain_cape", "item:renaissance_shared_clothing_textile_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Maritime South-east Asian Ordinary Dress",
		Description: "Admission: Maritime South-east Asian, 1450-1600; port or shipboard gate. Purpose: port, coastal, shipboard, or market dress.",
		Items:
		[
			new("renaissance_seasia_short_maritime_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_short_collarless_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_cloth_headwrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_rain_cape", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_textile_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitMaritimeSeasiaStatus() => new(
		Metadata: new("renaissance_outfit_maritime_seasia_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_seasia_embroidered_silk_courtshoes", "item:renaissance_seasia_long_crossfront_courtrobe", "item:renaissance_seasia_pleated_courtwaistcloth", "item:renaissance_seasia_sleeveless_courtvest", "item:renaissance_seasia_structured_headclothcrown"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Maritime South-east Asian Court Dress",
		Description: "Admission: Maritime South-east Asian, 1450-1600; court/status gate. Purpose: court, port office, diplomatic, or ceremonial dress.",
		Items:
		[
			new("renaissance_seasia_pleated_courtwaistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_long_crossfront_courtrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_sleeveless_courtvest", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_structured_headclothcrown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_seasia_embroidered_silk_courtshoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitSteppeOrdinary() => new(
		Metadata: new("renaissance_outfit_steppe_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_long_undershirt", "item:renaissance_steppe_pointed_feltcap", "item:renaissance_steppe_short_split_ridingcoat", "item:renaissance_steppe_soft_highboots", "item:renaissance_steppe_wide_ridingtrousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Steppe and Caravan Riding Dress",
		Description: "Admission: Steppe and Caravan, 1450-1600. Purpose: ordinary mounted, caravan, herding, or courier dress.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_steppe_wide_ridingtrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_steppe_short_split_ridingcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_steppe_pointed_feltcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_steppe_soft_highboots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitSteppeStatus() => new(
		Metadata: new("renaissance_outfit_steppe_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_long_undershirt", "item:renaissance_shared_clothing_quilted_jacket", "item:renaissance_shared_clothing_shoulder_shawl", "item:renaissance_steppe_fur_earflaphat", "item:renaissance_steppe_sidefastened_furcoat", "item:renaissance_steppe_soft_highboots", "item:renaissance_steppe_wide_ridingtrousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Steppe and Caravan Status Dress",
		Description: "Admission: Steppe and Caravan, 1450-1600; status/ceremonial gate. Purpose: senior retainer, court, diplomatic, or winter ceremonial dress.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_steppe_wide_ridingtrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_quilted_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_steppe_sidefastened_furcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_shoulder_shawl", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_steppe_fur_earflaphat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_steppe_soft_highboots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitAfricanCourtAtlanticOrdinary() => new(
		Metadata: new("renaissance_outfit_african_court_atlantic_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_africancourt_broad_waistwrapper", "item:renaissance_africancourt_shortsleeve_tunic", "item:renaissance_shared_clothing_cloth_headwrap", "item:renaissance_shared_clothing_leather_sandals", "item:renaissance_shared_clothing_wrapped_loincloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance African Court Atlantic Ordinary Dress",
		Description: "Admission: African Court Atlantic, 1450-1600; narrower local gate required. Purpose: ordinary household, market, artisan, or local court-service dress.",
		Items:
		[
			new("renaissance_shared_clothing_wrapped_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_africancourt_shortsleeve_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_africancourt_broad_waistwrapper", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_cloth_headwrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitAfricanCourtAtlanticStatus() => new(
		Metadata: new("renaissance_outfit_african_court_atlantic_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_africancourt_broadsleeve_courtrobe", "item:renaissance_africancourt_broad_waistwrapper", "item:renaissance_africancourt_longsleeve_sideslit_tunic", "item:renaissance_africancourt_raffia_shouldercape", "item:renaissance_africancourt_raised_woodensandals", "item:renaissance_africancourt_structured_crownheadcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance African Court Atlantic Status Dress",
		Description: "Admission: African Court Atlantic, 1450-1600; narrower court/status gate required. Purpose: court, office, diplomatic, or ceremonial dress.",
		Items:
		[
			new("renaissance_africancourt_broad_waistwrapper", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_africancourt_longsleeve_sideslit_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_africancourt_broadsleeve_courtrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_africancourt_raffia_shouldercape", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_africancourt_structured_crownheadcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_africancourt_raised_woodensandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
