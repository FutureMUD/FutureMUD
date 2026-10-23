#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits02Stock
{
	public static CatalogueOutfitDefinition RenaissanceOutfitOttomanJanissaryService() => new(
		Metadata: new("renaissance_outfit_ottoman_janissary_service", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:preindustrial_clothing_simple_woven_sash", "item:renaissance_ottoman_collarless_inner_shirt", "item:renaissance_ottoman_full_salwar_trousers", "item:renaissance_ottoman_janissary_bork", "item:renaissance_ottoman_short_cavalry_jacket", "item:renaissance_ottoman_short_fitted_vest", "item:renaissance_shared_clothing_ankle_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Ottoman Janissary service dress",
		Description: "Admission: named Ottoman Janissary corps context, approximately 1500-1600. Purpose: complete institutional service silhouette; weapon, armour, unit insignia, and rank marks remain separate.",
		Items:
		[
			new("renaissance_ottoman_collarless_inner_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_full_salwar_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_short_fitted_vest", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_short_cavalry_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ottoman_janissary_bork", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_ankle_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitLatinPriestMass() => new(
		Metadata: new("renaissance_outfit_latin_priest_mass", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_latin_amice", "item:medieval_latin_chasuble", "item:medieval_latin_linen_cincture", "item:medieval_latin_maniple", "item:medieval_latin_stole", "item:medieval_latin_white_alb", "item:medieval_soft_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Latin priest vested for Mass",
		Description: "Admission: Latin-rite institution continuing the established vestment forms. Purpose: reuse the complete medieval liturgical system rather than cloning chasuble, alb, stole, and related items under Renaissance prefixes.",
		Items:
		[
			new("medieval_latin_amice", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_white_alb", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_linen_cincture", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_stole", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_maniple", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_chasuble", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitBuddhistFormalOfficiant() => new(
		Metadata: new("renaissance_outfit_buddhist_formal_officiant", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_buddhist_formal_kesa", "item:medieval_buddhist_plain_underrobe", "item:medieval_buddhist_travelling_mantle", "item:medieval_hemp_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance East Asian Buddhist formal officiant",
		Description: "Admission: named East Asian Buddhist temple or monastery where the kesa form is locally appropriate. Purpose: reuse stable medieval monastic prototypes that remain credible in 1400-1600.",
		Items:
		[
			new("medieval_buddhist_plain_underrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_formal_kesa", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_travelling_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_hemp_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
