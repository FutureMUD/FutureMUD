#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits01Stock
{
	public static CatalogueOutfitDefinition EarlymodernOutfit0627() => new(
		Metadata: new("earlymodern_outfit_0627", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_ottoman_noble_jewellery", "item:earlymodern_ottoman_noble_male_court_garment", "item:earlymodern_ottoman_noble_male_formal_mantle", "item:earlymodern_ottoman_noble_regalia"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Ottoman male formal or state-occasion dress",
		Description: "Grouping: Ottoman. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_male_formal_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0628() => new(
		Metadata: new("earlymodern_outfit_0628", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_lace_head_veil", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_ottoman_noble_female_court_gown", "item:earlymodern_ottoman_noble_jewellery"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Ottoman female everyday courtly wear",
		Description: "Grouping: Ottoman. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_lace_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0629() => new(
		Metadata: new("earlymodern_outfit_0629", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_high_riding_boots", "item:earlymodern_noble_shared_lace_head_veil", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_ottoman_noble_female_court_gown", "item:earlymodern_ottoman_noble_female_travel_outerwear", "item:earlymodern_ottoman_noble_jewellery"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Ottoman female outdoor or travel attire",
		Description: "Grouping: Ottoman. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_female_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_high_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_lace_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0630() => new(
		Metadata: new("earlymodern_outfit_0630", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_ottoman_noble_female_formal_gown", "item:earlymodern_ottoman_noble_jewellery", "item:earlymodern_ottoman_noble_regalia"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Ottoman female formal or state-occasion dress",
		Description: "Grouping: Ottoman. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_female_formal_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ottoman_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0631() => new(
		Metadata: new("earlymodern_outfit_0631", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_maghrebi_noble_court_headwear", "item:earlymodern_maghrebi_noble_jewellery", "item:earlymodern_maghrebi_noble_male_court_garment", "item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_jewellery_pearl_necklace", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maghrebi male everyday courtly wear",
		Description: "Grouping: Maghrebi / North African. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_pearl_necklace", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0632() => new(
		Metadata: new("earlymodern_outfit_0632", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_maghrebi_noble_court_headwear", "item:earlymodern_maghrebi_noble_jewellery", "item:earlymodern_maghrebi_noble_male_court_garment", "item:earlymodern_maghrebi_noble_male_travel_outerwear", "item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_high_riding_boots", "item:earlymodern_noble_shared_silk_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maghrebi male outdoor or travel attire",
		Description: "Grouping: Maghrebi / North African. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_male_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_high_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0633() => new(
		Metadata: new("earlymodern_outfit_0633", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_maghrebi_noble_jewellery", "item:earlymodern_maghrebi_noble_male_court_garment", "item:earlymodern_maghrebi_noble_male_formal_mantle", "item:earlymodern_maghrebi_noble_regalia", "item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maghrebi male formal or state-occasion dress",
		Description: "Grouping: Maghrebi / North African. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_male_formal_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0634() => new(
		Metadata: new("earlymodern_outfit_0634", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_maghrebi_noble_female_court_gown", "item:earlymodern_maghrebi_noble_jewellery", "item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_lace_head_veil", "item:earlymodern_noble_shared_silk_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maghrebi female everyday courtly wear",
		Description: "Grouping: Maghrebi / North African. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_lace_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0635() => new(
		Metadata: new("earlymodern_outfit_0635", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_maghrebi_noble_female_court_gown", "item:earlymodern_maghrebi_noble_female_travel_outerwear", "item:earlymodern_maghrebi_noble_jewellery", "item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_high_riding_boots", "item:earlymodern_noble_shared_lace_head_veil", "item:earlymodern_noble_shared_silk_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maghrebi female outdoor or travel attire",
		Description: "Grouping: Maghrebi / North African. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_female_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_high_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_lace_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0636() => new(
		Metadata: new("earlymodern_outfit_0636", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_maghrebi_noble_female_formal_gown", "item:earlymodern_maghrebi_noble_jewellery", "item:earlymodern_maghrebi_noble_regalia", "item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_silk_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maghrebi female formal or state-occasion dress",
		Description: "Grouping: Maghrebi / North African. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_female_formal_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_maghrebi_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0637() => new(
		Metadata: new("earlymodern_outfit_0637", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_jewellery_pearl_necklace", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_persianate_noble_court_headwear", "item:earlymodern_persianate_noble_jewellery", "item:earlymodern_persianate_noble_male_court_garment"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Safavid male everyday courtly wear",
		Description: "Grouping: Safavid / post-Safavid Persianate. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_pearl_necklace", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0638() => new(
		Metadata: new("earlymodern_outfit_0638", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_high_riding_boots", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_persianate_noble_court_headwear", "item:earlymodern_persianate_noble_jewellery", "item:earlymodern_persianate_noble_male_court_garment", "item:earlymodern_persianate_noble_male_travel_outerwear"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Safavid male outdoor or travel attire",
		Description: "Grouping: Safavid / post-Safavid Persianate. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_male_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_high_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0639() => new(
		Metadata: new("earlymodern_outfit_0639", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_persianate_noble_jewellery", "item:earlymodern_persianate_noble_male_court_garment", "item:earlymodern_persianate_noble_male_formal_mantle", "item:earlymodern_persianate_noble_regalia"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Safavid male formal or state-occasion dress",
		Description: "Grouping: Safavid / post-Safavid Persianate. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_male_formal_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0640() => new(
		Metadata: new("earlymodern_outfit_0640", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_lace_head_veil", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_persianate_noble_female_court_gown", "item:earlymodern_persianate_noble_jewellery"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Safavid female everyday courtly wear",
		Description: "Grouping: Safavid / post-Safavid Persianate. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_lace_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0641() => new(
		Metadata: new("earlymodern_outfit_0641", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_high_riding_boots", "item:earlymodern_noble_shared_lace_head_veil", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_persianate_noble_female_court_gown", "item:earlymodern_persianate_noble_female_travel_outerwear", "item:earlymodern_persianate_noble_jewellery"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Safavid female outdoor or travel attire",
		Description: "Grouping: Safavid / post-Safavid Persianate. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_female_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_high_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_lace_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0642() => new(
		Metadata: new("earlymodern_outfit_0642", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_silk_trousers", "item:earlymodern_persianate_noble_female_formal_gown", "item:earlymodern_persianate_noble_jewellery", "item:earlymodern_persianate_noble_regalia"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Safavid female formal or state-occasion dress",
		Description: "Grouping: Safavid / post-Safavid Persianate. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_female_formal_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_persianate_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0643() => new(
		Metadata: new("earlymodern_outfit_0643", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_mughal_noble_court_headwear", "item:earlymodern_mughal_noble_jewellery", "item:earlymodern_mughal_noble_male_court_garment", "item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_waistcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Mughal male everyday courtly wear",
		Description: "Grouping: Mughal / Indo-Persian. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0644() => new(
		Metadata: new("earlymodern_outfit_0644", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_mughal_noble_court_headwear", "item:earlymodern_mughal_noble_jewellery", "item:earlymodern_mughal_noble_male_court_garment", "item:earlymodern_mughal_noble_male_travel_outerwear", "item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_leather_sandals", "item:earlymodern_noble_shared_silk_waistcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Mughal male outdoor or travel attire",
		Description: "Grouping: Mughal / Indo-Persian. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_male_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0645() => new(
		Metadata: new("earlymodern_outfit_0645", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_mughal_noble_jewellery", "item:earlymodern_mughal_noble_male_court_garment", "item:earlymodern_mughal_noble_male_formal_mantle", "item:earlymodern_mughal_noble_regalia", "item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_waistcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Mughal male formal or state-occasion dress",
		Description: "Grouping: Mughal / Indo-Persian. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_male_formal_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0646() => new(
		Metadata: new("earlymodern_outfit_0646", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_mughal_noble_female_court_gown", "item:earlymodern_mughal_noble_jewellery", "item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_jewellery_hair_ornament", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_silk_petticoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Mughal female everyday courtly wear",
		Description: "Grouping: Mughal / Indo-Persian. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_hair_ornament", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0647() => new(
		Metadata: new("earlymodern_outfit_0647", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_mughal_noble_female_court_gown", "item:earlymodern_mughal_noble_female_travel_outerwear", "item:earlymodern_mughal_noble_jewellery", "item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_jewellery_hair_ornament", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_leather_sandals", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_silk_petticoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Mughal female outdoor or travel attire",
		Description: "Grouping: Mughal / Indo-Persian. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_female_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_hair_ornament", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0648() => new(
		Metadata: new("earlymodern_outfit_0648", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_mughal_noble_female_formal_gown", "item:earlymodern_mughal_noble_jewellery", "item:earlymodern_mughal_noble_regalia", "item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_silk_petticoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Mughal female formal or state-occasion dress",
		Description: "Grouping: Mughal / Indo-Persian. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_female_formal_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_mughal_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0649() => new(
		Metadata: new("earlymodern_outfit_0649", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_deccan_noble_court_headwear", "item:earlymodern_deccan_noble_jewellery", "item:earlymodern_deccan_noble_male_court_garment", "item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_jewellery_pearl_necklace", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_waistcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maratha male everyday courtly wear",
		Description: "Grouping: Maratha / Rajput / Deccan. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_pearl_necklace", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0650() => new(
		Metadata: new("earlymodern_outfit_0650", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_deccan_noble_court_headwear", "item:earlymodern_deccan_noble_jewellery", "item:earlymodern_deccan_noble_male_court_garment", "item:earlymodern_deccan_noble_male_travel_outerwear", "item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_leather_sandals", "item:earlymodern_noble_shared_silk_waistcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maratha male outdoor or travel attire",
		Description: "Grouping: Maratha / Rajput / Deccan. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_male_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0651() => new(
		Metadata: new("earlymodern_outfit_0651", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_deccan_noble_jewellery", "item:earlymodern_deccan_noble_male_court_garment", "item:earlymodern_deccan_noble_male_formal_mantle", "item:earlymodern_deccan_noble_regalia", "item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_waistcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maratha male formal or state-occasion dress",
		Description: "Grouping: Maratha / Rajput / Deccan. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_male_formal_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0652() => new(
		Metadata: new("earlymodern_outfit_0652", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_deccan_noble_female_court_gown", "item:earlymodern_deccan_noble_jewellery", "item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_jewellery_hair_ornament", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_silk_petticoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maratha female everyday courtly wear",
		Description: "Grouping: Maratha / Rajput / Deccan. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_hair_ornament", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0653() => new(
		Metadata: new("earlymodern_outfit_0653", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_deccan_noble_female_court_gown", "item:earlymodern_deccan_noble_female_travel_outerwear", "item:earlymodern_deccan_noble_jewellery", "item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_jewellery_hair_ornament", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_leather_sandals", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_silk_petticoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maratha female outdoor or travel attire",
		Description: "Grouping: Maratha / Rajput / Deccan. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_female_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_female_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_hair_ornament", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0654() => new(
		Metadata: new("earlymodern_outfit_0654", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_deccan_noble_female_formal_gown", "item:earlymodern_deccan_noble_jewellery", "item:earlymodern_deccan_noble_regalia", "item:earlymodern_noble_jewellery_gem_earrings", "item:earlymodern_noble_jewellery_gold_bracelets", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_silk_inner_robe", "item:earlymodern_noble_shared_silk_petticoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Maratha female formal or state-occasion dress",
		Description: "Grouping: Maratha / Rajput / Deccan. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_silk_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_female_formal_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_deccan_noble_regalia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gem_earrings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_bracelets", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0655() => new(
		Metadata: new("earlymodern_outfit_0655", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_embroidered_slippers", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_silk_sash", "item:earlymodern_noble_shared_silk_waistcloth", "item:earlymodern_southindian_noble_court_headwear", "item:earlymodern_southindian_noble_jewellery", "item:earlymodern_southindian_noble_male_court_garment"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern South Indian male everyday courtly wear",
		Description: "Grouping: South Indian / coastal trade. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_southindian_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_embroidered_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_silk_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_southindian_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_southindian_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0656() => new(
		Metadata: new("earlymodern_outfit_0656", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_noble_jewellery_gold_signet_ring", "item:earlymodern_noble_shared_dress_gloves", "item:earlymodern_noble_shared_fine_cotton_inner_tunic", "item:earlymodern_noble_shared_fine_cotton_loin_drawers", "item:earlymodern_noble_shared_fine_leather_sandals", "item:earlymodern_noble_shared_silk_waistcloth", "item:earlymodern_southindian_noble_court_headwear", "item:earlymodern_southindian_noble_jewellery", "item:earlymodern_southindian_noble_male_court_garment", "item:earlymodern_southindian_noble_male_travel_outerwear"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern South Indian male outdoor or travel attire",
		Description: "Grouping: South Indian / coastal trade. Collection: Noble, royal, and court-equivalent outfit manifests.",
		Items:
		[
			new("earlymodern_noble_shared_fine_cotton_loin_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_cotton_inner_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_silk_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_southindian_noble_male_court_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_southindian_noble_male_travel_outerwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_shared_dress_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_southindian_noble_court_headwear", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_southindian_noble_jewellery", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_noble_jewellery_gold_signet_ring", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
