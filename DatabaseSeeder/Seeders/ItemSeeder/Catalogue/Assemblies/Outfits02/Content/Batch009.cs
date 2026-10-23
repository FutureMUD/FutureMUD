#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits02Stock
{
	public static CatalogueOutfitDefinition MedievalOutfit0022() => new(
		Metadata: new("medieval_outfit_0022", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_leather_shoes", "item:medieval_fine_silk_surcoat", "item:medieval_fine_wool_cotte", "item:medieval_fur_trimmed_mantle", "item:medieval_iron_buckled_leather_belt", "item:medieval_linen_braies", "item:medieval_wool_chausses"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval High English / British elite male",
		Description: "Builder-facing medieval clothing outfit for High English / British elite male.",
		Items:
		[
			new("medieval_linen_braies", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_chausses", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_wool_cotte", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_silk_surcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fur_trimmed_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0023() => new(
		Metadata: new("medieval_outfit_0023", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_linen_chainse", "item:medieval_linen_wimple", "item:medieval_simple_wool_mantle", "item:medieval_simple_woven_sash", "item:medieval_soft_leather_shoes", "item:medieval_wool_kirtle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval High English / British common female",
		Description: "Builder-facing medieval clothing outfit for High English / British common female.",
		Items:
		[
			new("medieval_linen_chainse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_kirtle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_wimple", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_simple_wool_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0024() => new(
		Metadata: new("medieval_outfit_0024", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_barbette_veil", "item:medieval_fine_leather_shoes", "item:medieval_fine_wool_kirtle", "item:medieval_fine_woven_girdle", "item:medieval_fur_trimmed_mantle", "item:medieval_linen_chainse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval High English / British elite female",
		Description: "Builder-facing medieval clothing outfit for High English / British elite female.",
		Items:
		[
			new("medieval_linen_chainse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_woven_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_wool_kirtle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_barbette_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fur_trimmed_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0025() => new(
		Metadata: new("medieval_outfit_0025", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_gaelic_leine", "item:medieval_linen_undertunic", "item:medieval_plain_leather_belt", "item:medieval_soft_leather_shoes", "item:medieval_wool_brat", "item:medieval_wool_trews"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Irish / Gaelic common male",
		Description: "Builder-facing medieval clothing outfit for Irish / Gaelic common male.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_gaelic_leine", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_trews", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_brat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0026() => new(
		Metadata: new("medieval_outfit_0026", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_ankle_leather_boots", "item:medieval_fine_bordered_brat", "item:medieval_fine_bordered_leine", "item:medieval_iron_buckled_leather_belt", "item:medieval_linen_undertunic", "item:medieval_wool_trews"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Irish / Gaelic elite male",
		Description: "Builder-facing medieval clothing outfit for Irish / Gaelic elite male.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_bordered_leine", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_trews", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_bordered_brat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0027() => new(
		Metadata: new("medieval_outfit_0027", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_gaelic_leine", "item:medieval_linen_chemise", "item:medieval_linen_veil", "item:medieval_simple_woven_sash", "item:medieval_soft_leather_shoes", "item:medieval_wool_brat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Irish / Gaelic common female",
		Description: "Builder-facing medieval clothing outfit for Irish / Gaelic common female.",
		Items:
		[
			new("medieval_linen_chemise", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_gaelic_leine", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_brat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0028() => new(
		Metadata: new("medieval_outfit_0028", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_bordered_brat", "item:medieval_fine_bordered_leine", "item:medieval_fine_leather_shoes", "item:medieval_fine_linen_veil", "item:medieval_fine_woven_girdle", "item:medieval_linen_chemise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Irish / Gaelic elite female",
		Description: "Builder-facing medieval clothing outfit for Irish / Gaelic elite female.",
		Items:
		[
			new("medieval_linen_chemise", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_woven_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_bordered_leine", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_linen_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_bordered_brat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0029() => new(
		Metadata: new("medieval_outfit_0029", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_ankle_leather_boots", "item:medieval_gaelic_leine", "item:medieval_linen_undertunic", "item:medieval_plain_leather_belt", "item:medieval_wool_brat", "item:medieval_wool_trews"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Scottish / Gaelic-Lowland common male",
		Description: "Builder-facing medieval clothing outfit for Scottish / Gaelic-Lowland common male.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_gaelic_leine", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_trews", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_brat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0030() => new(
		Metadata: new("medieval_outfit_0030", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_bordered_brat", "item:medieval_fine_leather_shoes", "item:medieval_fine_wool_cotte", "item:medieval_iron_buckled_leather_belt", "item:medieval_linen_braies", "item:medieval_linen_coif", "item:medieval_wool_chausses"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Scottish / Gaelic-Lowland elite male",
		Description: "Builder-facing medieval clothing outfit for Scottish / Gaelic-Lowland elite male.",
		Items:
		[
			new("medieval_linen_braies", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_chausses", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_wool_cotte", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_bordered_brat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_coif", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0031() => new(
		Metadata: new("medieval_outfit_0031", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_linen_chemise", "item:medieval_linen_veil", "item:medieval_simple_woven_sash", "item:medieval_soft_leather_shoes", "item:medieval_wool_brat", "item:medieval_wool_kirtle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Scottish / Gaelic-Lowland common female",
		Description: "Builder-facing medieval clothing outfit for Scottish / Gaelic-Lowland common female.",
		Items:
		[
			new("medieval_linen_chemise", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_kirtle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_brat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0032() => new(
		Metadata: new("medieval_outfit_0032", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_leather_shoes", "item:medieval_fine_linen_veil", "item:medieval_fine_wool_kirtle", "item:medieval_fine_woven_girdle", "item:medieval_fur_trimmed_mantle", "item:medieval_linen_chainse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Scottish / Gaelic-Lowland elite female",
		Description: "Builder-facing medieval clothing outfit for Scottish / Gaelic-Lowland elite female.",
		Items:
		[
			new("medieval_linen_chainse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_woven_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_wool_kirtle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_linen_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fur_trimmed_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0033() => new(
		Metadata: new("medieval_outfit_0033", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_carolingian_cloak", "item:medieval_frankish_knee_tunic", "item:medieval_linen_undertunic", "item:medieval_narrow_wool_trousers", "item:medieval_plain_leather_belt", "item:medieval_soft_leather_shoes", "item:medieval_wool_leg_wraps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Carolingian / Frankish common male",
		Description: "Builder-facing medieval clothing outfit for Carolingian / Frankish common male.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_frankish_knee_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_narrow_wool_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_leg_wraps", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_carolingian_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0034() => new(
		Metadata: new("medieval_outfit_0034", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_ankle_leather_boots", "item:medieval_fine_frankish_tunic", "item:medieval_fur_lined_cloak", "item:medieval_iron_buckled_leather_belt", "item:medieval_linen_coif", "item:medieval_linen_undertunic", "item:medieval_narrow_wool_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Carolingian / Frankish elite male",
		Description: "Builder-facing medieval clothing outfit for Carolingian / Frankish elite male.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_frankish_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_narrow_wool_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fur_lined_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_coif", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0035() => new(
		Metadata: new("medieval_outfit_0035", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_carolingian_cloak", "item:medieval_linen_chemise", "item:medieval_linen_veil", "item:medieval_plain_leather_belt", "item:medieval_soft_leather_shoes", "item:medieval_straight_wool_gown"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Carolingian / Frankish common female",
		Description: "Builder-facing medieval clothing outfit for Carolingian / Frankish common female.",
		Items:
		[
			new("medieval_linen_chemise", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_straight_wool_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_carolingian_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0036() => new(
		Metadata: new("medieval_outfit_0036", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_bordered_wool_gown", "item:medieval_fine_leather_shoes", "item:medieval_fine_linen_veil", "item:medieval_fine_woven_girdle", "item:medieval_fur_trimmed_mantle", "item:medieval_linen_chemise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Carolingian / Frankish elite female",
		Description: "Builder-facing medieval clothing outfit for Carolingian / Frankish elite female.",
		Items:
		[
			new("medieval_linen_chemise", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_woven_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_bordered_wool_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_linen_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fur_trimmed_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0037() => new(
		Metadata: new("medieval_outfit_0037", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_capetian_cotte", "item:medieval_half_circle_mantle", "item:medieval_linen_braies", "item:medieval_linen_coif", "item:medieval_plain_leather_belt", "item:medieval_soft_leather_shoes", "item:medieval_wool_chausses"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Capetian French common male",
		Description: "Builder-facing medieval clothing outfit for Capetian French common male.",
		Items:
		[
			new("medieval_linen_braies", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_chausses", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_capetian_cotte", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_half_circle_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_coif", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0038() => new(
		Metadata: new("medieval_outfit_0038", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_leather_shoes", "item:medieval_fine_silk_surcoat", "item:medieval_fine_wool_cotte", "item:medieval_fur_trimmed_mantle", "item:medieval_iron_buckled_leather_belt", "item:medieval_linen_braies", "item:medieval_wool_chausses"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Capetian French elite male",
		Description: "Builder-facing medieval clothing outfit for Capetian French elite male.",
		Items:
		[
			new("medieval_linen_braies", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_chausses", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_wool_cotte", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_silk_surcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fur_trimmed_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0039() => new(
		Metadata: new("medieval_outfit_0039", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_half_circle_mantle", "item:medieval_linen_chainse", "item:medieval_linen_wimple", "item:medieval_simple_woven_sash", "item:medieval_soft_leather_shoes", "item:medieval_wool_kirtle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Capetian French common female",
		Description: "Builder-facing medieval clothing outfit for Capetian French common female.",
		Items:
		[
			new("medieval_linen_chainse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_kirtle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_wimple", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_half_circle_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0040() => new(
		Metadata: new("medieval_outfit_0040", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_barbette_veil", "item:medieval_fine_bliaut", "item:medieval_fine_leather_shoes", "item:medieval_fine_woven_girdle", "item:medieval_fur_trimmed_mantle", "item:medieval_linen_chainse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Capetian French elite female",
		Description: "Builder-facing medieval clothing outfit for Capetian French elite female.",
		Items:
		[
			new("medieval_linen_chainse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_woven_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_bliaut", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_barbette_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fur_trimmed_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0041() => new(
		Metadata: new("medieval_outfit_0041", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_ankle_leather_boots", "item:medieval_capetian_cotte", "item:medieval_gugel_hood", "item:medieval_heavy_wool_cloak", "item:medieval_linen_braies", "item:medieval_plain_leather_belt", "item:medieval_wool_hose"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Holy Roman Empire / German common male",
		Description: "Builder-facing medieval clothing outfit for Holy Roman Empire / German common male.",
		Items:
		[
			new("medieval_linen_braies", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_hose", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_capetian_cotte", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_gugel_hood", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_heavy_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0042() => new(
		Metadata: new("medieval_outfit_0042", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_silk_surcoat", "item:medieval_fine_wool_cotte", "item:medieval_fur_trimmed_mantle", "item:medieval_high_leather_boots", "item:medieval_iron_buckled_leather_belt", "item:medieval_linen_braies", "item:medieval_wool_chausses"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Holy Roman Empire / German elite male",
		Description: "Builder-facing medieval clothing outfit for Holy Roman Empire / German elite male.",
		Items:
		[
			new("medieval_linen_braies", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_chausses", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_high_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_wool_cotte", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_silk_surcoat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fur_trimmed_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0043() => new(
		Metadata: new("medieval_outfit_0043", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_gugel_hood", "item:medieval_linen_chainse", "item:medieval_linen_wimple", "item:medieval_simple_woven_sash", "item:medieval_soft_leather_shoes", "item:medieval_wool_kirtle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Holy Roman Empire / German common female",
		Description: "Builder-facing medieval clothing outfit for Holy Roman Empire / German common female.",
		Items:
		[
			new("medieval_linen_chainse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_kirtle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_wimple", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_gugel_hood", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0044() => new(
		Metadata: new("medieval_outfit_0044", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_leather_shoes", "item:medieval_fine_linen_veil", "item:medieval_fine_wool_kirtle", "item:medieval_fine_woven_girdle", "item:medieval_fur_trimmed_mantle", "item:medieval_linen_chainse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Holy Roman Empire / German elite female",
		Description: "Builder-facing medieval clothing outfit for Holy Roman Empire / German elite female.",
		Items:
		[
			new("medieval_linen_chainse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_woven_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_wool_kirtle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_linen_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fur_trimmed_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0045() => new(
		Metadata: new("medieval_outfit_0045", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_christian_iberian_camisa", "item:medieval_iberian_manto", "item:medieval_linen_coif", "item:medieval_plain_leather_belt", "item:medieval_soft_leather_shoes", "item:medieval_wool_hose", "item:medieval_wool_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Christian Iberian common male",
		Description: "Builder-facing medieval clothing outfit for Christian Iberian common male.",
		Items:
		[
			new("medieval_christian_iberian_camisa", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_hose", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_iberian_manto", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_coif", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0046() => new(
		Metadata: new("medieval_outfit_0046", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_christian_iberian_camisa", "item:medieval_fine_iberian_manto", "item:medieval_fine_leather_shoes", "item:medieval_fine_silk_pellote", "item:medieval_fine_wool_cotte", "item:medieval_iron_buckled_leather_belt", "item:medieval_wool_chausses"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Christian Iberian elite male",
		Description: "Builder-facing medieval clothing outfit for Christian Iberian elite male.",
		Items:
		[
			new("medieval_christian_iberian_camisa", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_chausses", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_iron_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_wool_cotte", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_silk_pellote", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_iberian_manto", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0047() => new(
		Metadata: new("medieval_outfit_0047", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_christian_iberian_camisa", "item:medieval_iberian_manto", "item:medieval_linen_toca", "item:medieval_simple_woven_sash", "item:medieval_soft_leather_shoes", "item:medieval_wool_saya"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Christian Iberian common female",
		Description: "Builder-facing medieval clothing outfit for Christian Iberian common female.",
		Items:
		[
			new("medieval_christian_iberian_camisa", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_saya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_toca", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_iberian_manto", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0048() => new(
		Metadata: new("medieval_outfit_0048", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_iberian_manto", "item:medieval_fine_leather_shoes", "item:medieval_fine_linen_veil", "item:medieval_fine_silk_pellote", "item:medieval_fine_wool_saya", "item:medieval_fine_woven_girdle", "item:medieval_linen_chainse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Christian Iberian elite female",
		Description: "Builder-facing medieval clothing outfit for Christian Iberian elite female.",
		Items:
		[
			new("medieval_linen_chainse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_woven_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_wool_saya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_silk_pellote", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_iberian_manto", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_linen_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0049() => new(
		Metadata: new("medieval_outfit_0049", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_cotton_qamis", "item:medieval_cotton_sirwal", "item:medieval_linen_izar", "item:medieval_soft_leather_slippers", "item:medieval_wool_burnous", "item:medieval_wrapped_cotton_turban"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Andalusian common male",
		Description: "Builder-facing medieval clothing outfit for Andalusian common male.",
		Items:
		[
			new("medieval_linen_izar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_cotton_sirwal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wrapped_cotton_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_cotton_qamis", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_burnous", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0050() => new(
		Metadata: new("medieval_outfit_0050", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_fine_burnous", "item:medieval_fine_leather_shoes", "item:medieval_fine_silk_qamis", "item:medieval_fine_silk_sirwal", "item:medieval_fine_silk_turban", "item:medieval_fine_tiraz_robe", "item:medieval_linen_izar"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Andalusian elite male",
		Description: "Builder-facing medieval clothing outfit for Andalusian elite male.",
		Items:
		[
			new("medieval_linen_izar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_silk_sirwal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_silk_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_silk_qamis", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_tiraz_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_fine_burnous", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0051() => new(
		Metadata: new("medieval_outfit_0051", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_cotton_head_veil", "item:medieval_cotton_qamis", "item:medieval_cotton_sirwal", "item:medieval_soft_leather_slippers", "item:medieval_wool_burnous", "item:medieval_wool_jubba"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Andalusian common female",
		Description: "Builder-facing medieval clothing outfit for Andalusian common female.",
		Items:
		[
			new("medieval_cotton_sirwal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_cotton_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_cotton_qamis", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_jubba", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_burnous", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
