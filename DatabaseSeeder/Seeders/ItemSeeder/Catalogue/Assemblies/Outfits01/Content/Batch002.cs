#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits01Stock
{
	public static CatalogueOutfitDefinition AntiquityOutfit0031() => new(
		Metadata: new("antiquity_outfit_0031", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_knee_length_wool_tunica", "item:antiquity_linen_loincloth", "item:antiquity_roman_plated_military_belt", "item:antiquity_roman_reinforced_caligae", "item:antiquity_roman_wool_focale", "item:antiquity_roman_wool_sagum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Italic legionary service dress",
		Description: "Admission: Roman Republican or Imperial military service context; clothing only, with armour and weapons supplied separately. Purpose: complete off-duty, camp, march, or under-armour clothing.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_knee_length_wool_tunica", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_roman_wool_focale", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_roman_plated_military_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_roman_reinforced_caligae", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_roman_wool_sagum", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0032() => new(
		Metadata: new("antiquity_outfit_0032", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item-skin:antiquity_skin_senatorial_broad_striped_tunica", "item:antiquity_fine_leather_sandals", "item:antiquity_fine_linen_tunica", "item:antiquity_fine_toga_praetexta", "item:antiquity_linen_loincloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Italic senatorial magistrate civic dress",
		Description: "Admission: Roman civic office or senatorial ceremony. Purpose: formal public dress using an existing toga praetexta and a seeded broad-striped tunica skin rather than duplicate behaviour prototypes.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_linen_tunica", "antiquity_skin_senatorial_broad_striped_tunica", null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_toga_praetexta", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0033() => new(
		Metadata: new("antiquity_outfit_0033", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_egyptian_spotted_hide_priest_mantle", "item:antiquity_fine_front_knotted_girdle", "item:antiquity_fine_papyrus_sandals", "item:antiquity_linen_loincloth", "item:antiquity_pleated_linen_shendyt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Egyptian sem-priest temple dress",
		Description: "Admission: named Egyptian temple and funerary-ritual context. Purpose: specialised priestly ensemble distinguished by its spotted hide mantle.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_pleated_linen_shendyt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_front_knotted_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_egyptian_spotted_hide_priest_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_papyrus_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0034() => new(
		Metadata: new("antiquity_outfit_0034", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item-skin:antiquity_skin_vestal_white_priestly_veil", "item:adjacent_antiquity_priestly_linen_veil", "item:antiquity_fine_leather_sandals", "item:antiquity_fine_long_linen_tunica", "item:antiquity_italic_white_wool_infula", "item:antiquity_linen_breastband", "item:antiquity_wool_stola"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Italic Vestal-style priestess ceremonial dress",
		Description: "Admission: named Italic female priestly institution. Purpose: formal white layered dress using a ritual headband and a white skin on the existing priestly veil.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_long_linen_tunica", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_stola", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_italic_white_wool_infula", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("adjacent_antiquity_priestly_linen_veil", "antiquity_skin_vestal_white_priestly_veil", null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0001() => new(
		Metadata: new("earlymodern_outfit_0001", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_western_clothing_broadcloth_work_coat", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_knitted_wool_stockings", "item:earlymodern_western_clothing_plain_linen_shirt", "item:preindustrial_clothing_plain_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_low_leather_shoes", "item:renaissance_shared_clothing_soft_brimless_cap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French rural field labourer male",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_broadcloth_work_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knitted_wool_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_soft_brimless_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0002() => new(
		Metadata: new("earlymodern_outfit_0002", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_full_wool_cloak", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_plain_linen_shirt", "item:earlymodern_western_clothing_plain_wool_waistcoat", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_felt_brimmed_hat", "item:renaissance_shared_clothing_footed_stockings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French prosperous rural vintner male",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_wool_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_felt_brimmed_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0003() => new(
		Metadata: new("earlymodern_outfit_0003", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_knitted_wool_stockings", "item:earlymodern_western_clothing_linen_day_cap", "item:earlymodern_western_clothing_linen_neckcloth", "item:earlymodern_western_clothing_plain_linen_shirt", "item:earlymodern_western_clothing_plain_wool_waistcoat", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French urban journeyman artisan male",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_wool_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knitted_wool_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_neckcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_day_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0004() => new(
		Metadata: new("earlymodern_outfit_0004", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_french_clothing_long_skirted_coat", "item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_cocked_felt_hat", "item:earlymodern_western_clothing_fine_broadcloth_waistcoat", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_linen_neckcloth", "item:earlymodern_western_clothing_plain_linen_shirt", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_footed_stockings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French urban master tradesman male",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_broadcloth_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_french_clothing_long_skirted_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_cocked_felt_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_neckcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0005() => new(
		Metadata: new("earlymodern_outfit_0005", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_cocked_felt_hat", "item:earlymodern_western_clothing_fine_broadcloth_waistcoat", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_linen_neckcloth", "item:earlymodern_western_clothing_long_broadcloth_coat", "item:earlymodern_western_clothing_plain_linen_shirt", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_footed_stockings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French bourgeois clerk male",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_broadcloth_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_long_broadcloth_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_cocked_felt_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_neckcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0006() => new(
		Metadata: new("earlymodern_outfit_0006", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fitted_wool_bodice", "item:earlymodern_western_clothing_full_wool_cloak", "item:earlymodern_western_clothing_full_wool_petticoat", "item:earlymodern_western_clothing_linen_day_cap", "item:earlymodern_western_clothing_linen_work_apron", "item:earlymodern_western_clothing_long_linen_shift", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French rural field worker female",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fitted_wool_bodice", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_work_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_day_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0007() => new(
		Metadata: new("earlymodern_outfit_0007", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_british_clothing_checked_linen_apron", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fitted_wool_bodice", "item:earlymodern_western_clothing_full_wool_cloak", "item:earlymodern_western_clothing_full_wool_petticoat", "item:earlymodern_western_clothing_linen_day_cap", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_long_linen_shift", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French prosperous rural householder female",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fitted_wool_bodice", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_british_clothing_checked_linen_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_day_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0008() => new(
		Metadata: new("earlymodern_outfit_0008", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_french_clothing_lace_trimmed_day_cap", "item:earlymodern_french_clothing_short_casaquin_jacket", "item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fine_linen_apron", "item:earlymodern_western_clothing_full_wool_petticoat", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_long_linen_shift"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French urban seamstress female",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_french_clothing_short_casaquin_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_linen_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_french_clothing_lace_trimmed_day_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0009() => new(
		Metadata: new("earlymodern_outfit_0009", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_french_clothing_lace_trimmed_day_cap", "item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fine_linen_apron", "item:earlymodern_western_clothing_full_wool_cloak", "item:earlymodern_western_clothing_full_wool_petticoat", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_long_linen_shift", "item:earlymodern_western_clothing_plain_mantua_gown"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French urban shopkeeper female",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_mantua_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_linen_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_french_clothing_lace_trimmed_day_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0010() => new(
		Metadata: new("earlymodern_outfit_0010", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_french_clothing_lace_trimmed_day_cap", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fine_linen_apron", "item:earlymodern_western_clothing_full_wool_petticoat", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_long_linen_shift", "item:earlymodern_western_clothing_plain_leather_mules", "item:earlymodern_western_clothing_plain_mantua_gown", "item:renaissance_shared_clothing_leather_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern French bourgeois household manager female",
		Description: "Grouping: French / Baroque court and urban. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_mantua_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_linen_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_leather_mules", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_french_clothing_lace_trimmed_day_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_leather_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0011() => new(
		Metadata: new("earlymodern_outfit_0011", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_dutch_clothing_full_canvas_breeches", "item:earlymodern_dutch_clothing_short_wool_work_jacket", "item:earlymodern_dutch_clothing_white_linen_cap", "item:earlymodern_dutch_clothing_wooden_clogs", "item:earlymodern_western_clothing_knitted_wool_stockings", "item:earlymodern_western_clothing_plain_linen_shirt", "item:preindustrial_clothing_plain_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch rural peat cutter male",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_short_wool_work_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_full_canvas_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knitted_wool_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_wooden_clogs", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_white_linen_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0012() => new(
		Metadata: new("earlymodern_outfit_0012", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_dutch_clothing_full_canvas_breeches", "item:earlymodern_dutch_clothing_short_wool_work_jacket", "item:earlymodern_western_clothing_plain_linen_shirt", "item:earlymodern_western_clothing_plain_wool_waistcoat", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_felt_brimmed_hat", "item:renaissance_shared_clothing_footed_stockings", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch prosperous dairy farmer male",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_wool_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_short_wool_work_jacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_full_canvas_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_felt_brimmed_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0013() => new(
		Metadata: new("earlymodern_outfit_0013", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_dutch_clothing_full_canvas_breeches", "item:earlymodern_dutch_clothing_white_linen_cap", "item:earlymodern_western_clothing_knitted_wool_stockings", "item:earlymodern_western_clothing_linen_neckcloth", "item:earlymodern_western_clothing_plain_linen_shirt", "item:earlymodern_western_clothing_plain_wool_waistcoat", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch urban guild artisan male",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_wool_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_full_canvas_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knitted_wool_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_white_linen_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_neckcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0014() => new(
		Metadata: new("earlymodern_outfit_0014", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_dutch_clothing_full_canvas_breeches", "item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_fine_broadcloth_waistcoat", "item:earlymodern_western_clothing_linen_neckcloth", "item:earlymodern_western_clothing_long_broadcloth_coat", "item:earlymodern_western_clothing_plain_linen_shirt", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_felt_brimmed_hat", "item:renaissance_shared_clothing_footed_stockings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch urban shopkeeper male",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_broadcloth_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_long_broadcloth_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_full_canvas_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_felt_brimmed_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_neckcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0015() => new(
		Metadata: new("earlymodern_outfit_0015", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_french_clothing_long_skirted_coat", "item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_cocked_felt_hat", "item:earlymodern_western_clothing_fine_broadcloth_waistcoat", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_linen_neckcloth", "item:earlymodern_western_clothing_plain_linen_shirt", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_footed_stockings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch mercantile clerk male",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_broadcloth_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_french_clothing_long_skirted_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_cocked_felt_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_neckcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0016() => new(
		Metadata: new("earlymodern_outfit_0016", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_dutch_clothing_striped_wool_petticoat", "item:earlymodern_dutch_clothing_white_linen_cap", "item:earlymodern_dutch_clothing_wooden_clogs", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fitted_wool_bodice", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_linen_work_apron", "item:earlymodern_western_clothing_long_linen_shift"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch rural dairy worker female",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fitted_wool_bodice", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_striped_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_work_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_wooden_clogs", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_white_linen_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0017() => new(
		Metadata: new("earlymodern_outfit_0017", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_british_clothing_checked_linen_apron", "item:earlymodern_dutch_clothing_short_wool_overjacket", "item:earlymodern_dutch_clothing_striped_wool_petticoat", "item:earlymodern_dutch_clothing_white_linen_cap", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_full_wool_cloak", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_long_linen_shift", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch prosperous rural householder female",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_short_wool_overjacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_striped_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_british_clothing_checked_linen_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_white_linen_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0018() => new(
		Metadata: new("earlymodern_outfit_0018", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_dutch_clothing_striped_wool_petticoat", "item:earlymodern_dutch_clothing_white_linen_cap", "item:earlymodern_dutch_clothing_wooden_clogs", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fitted_wool_bodice", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_linen_work_apron", "item:earlymodern_western_clothing_long_linen_shift", "item:renaissance_shared_clothing_neck_kerchief"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch urban market vendor female",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fitted_wool_bodice", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_striped_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_work_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_wooden_clogs", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_white_linen_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_neck_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0019() => new(
		Metadata: new("earlymodern_outfit_0019", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_dutch_clothing_short_wool_overjacket", "item:earlymodern_dutch_clothing_white_linen_cap", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fine_linen_apron", "item:earlymodern_western_clothing_full_wool_petticoat", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_long_linen_shift", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch urban craftswoman female",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_short_wool_overjacket", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_linen_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_white_linen_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0020() => new(
		Metadata: new("earlymodern_outfit_0020", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_dutch_clothing_white_linen_cap", "item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fine_linen_apron", "item:earlymodern_western_clothing_full_wool_petticoat", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_long_linen_shift", "item:earlymodern_western_clothing_plain_mantua_gown", "item:renaissance_shared_clothing_leather_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Dutch burgher merchant household female",
		Description: "Grouping: Dutch Republic / Low Countries. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_mantua_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_linen_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_dutch_clothing_white_linen_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_leather_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0021() => new(
		Metadata: new("earlymodern_outfit_0021", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_british_clothing_linen_smock_frock", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_knitted_wool_stockings", "item:earlymodern_western_clothing_plain_linen_shirt", "item:preindustrial_clothing_plain_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_low_leather_shoes", "item:renaissance_shared_clothing_soft_brimless_cap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern English rural field labourer male",
		Description: "Grouping: English / British Stuart-Georgian. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_british_clothing_linen_smock_frock", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knitted_wool_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_soft_brimless_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0022() => new(
		Metadata: new("earlymodern_outfit_0022", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_western_clothing_broadcloth_work_coat", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_plain_linen_shirt", "item:earlymodern_western_clothing_plain_wool_waistcoat", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_felt_brimmed_hat", "item:renaissance_shared_clothing_footed_stockings", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern English prosperous tenant farmer male",
		Description: "Grouping: English / British Stuart-Georgian. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_wool_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_broadcloth_work_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_felt_brimmed_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0023() => new(
		Metadata: new("earlymodern_outfit_0023", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_knitted_wool_stockings", "item:earlymodern_western_clothing_linen_day_cap", "item:earlymodern_western_clothing_linen_neckcloth", "item:earlymodern_western_clothing_plain_linen_shirt", "item:earlymodern_western_clothing_plain_wool_waistcoat", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern English urban journeyman artisan male",
		Description: "Grouping: English / British Stuart-Georgian. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_wool_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knitted_wool_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_day_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_neckcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0024() => new(
		Metadata: new("earlymodern_outfit_0024", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_cocked_felt_hat", "item:earlymodern_western_clothing_fine_broadcloth_waistcoat", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_linen_neckcloth", "item:earlymodern_western_clothing_long_broadcloth_coat", "item:earlymodern_western_clothing_plain_linen_shirt", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_footed_stockings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern English urban shopkeeper male",
		Description: "Grouping: English / British Stuart-Georgian. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_broadcloth_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_long_broadcloth_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_cocked_felt_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_neckcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0025() => new(
		Metadata: new("earlymodern_outfit_0025", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_french_clothing_long_skirted_coat", "item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_fine_broadcloth_waistcoat", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_linen_neckcloth", "item:earlymodern_western_clothing_plain_linen_shirt", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_felt_brimmed_hat", "item:renaissance_shared_clothing_footed_stockings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern English bourgeois scrivener male",
		Description: "Grouping: English / British Stuart-Georgian. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_plain_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fine_broadcloth_waistcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_french_clothing_long_skirted_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_knee_breeches", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_footed_stockings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_buckled_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_felt_brimmed_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_neckcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0026() => new(
		Metadata: new("earlymodern_outfit_0026", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_british_clothing_linen_mob_cap", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fitted_wool_bodice", "item:earlymodern_western_clothing_full_wool_petticoat", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_linen_work_apron", "item:earlymodern_western_clothing_long_linen_shift", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern English rural field worker female",
		Description: "Grouping: English / British Stuart-Georgian. Collection: First-pass civilian outfit manifests.",
		Items:
		[
			new("earlymodern_western_clothing_long_linen_shift", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_canvas_stays", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_fitted_wool_bodice", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_full_wool_petticoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_work_apron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_low_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_british_clothing_linen_mob_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_western_clothing_linen_shoulder_kerchief", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
