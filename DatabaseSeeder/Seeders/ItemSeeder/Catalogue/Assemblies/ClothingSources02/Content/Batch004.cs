#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources02Stock
{
	public static CatalogueItemDefinition EarlymodernNobleJewelleryGoldBracelets() => new()
	{
		Metadata = new("earlymodern_noble_jewellery_gold_bracelets", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "bracelets",
		ShortDescription = "a pair of chased gold bracelets",
		FullDescription = "A pair of chased gold bracelets is worked and joined from gold so that the outward silhouette of the bracelets remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the bracelets bends or fastens rather than spread as decoration. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 180,
		Cost = 2400.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Bracelets", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Bracelets"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 108 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleJewelleryGoldSignetRing() => new()
	{
		Metadata = new("earlymodern_noble_jewellery_gold_signet_ring", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "ring",
		ShortDescription = "a deeply engraved gold signet ring",
		FullDescription = "A deeply engraved gold signet ring is worked and joined from gold so that the outward silhouette of the ring remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the ring carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 24,
		Cost = 1800.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Rings", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Ring", "SealStamp_Medieval_NobleSignetRing"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 108 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleJewelleryHairOrnament() => new()
	{
		Metadata = new("earlymodern_noble_jewellery_hair_ornament", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "ornament",
		ShortDescription = "a gem-set gold hair ornament",
		FullDescription = "A gem-set gold hair ornament is worked and joined from gold so that the outward silhouette of the ornament remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the ornament carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 85,
		Cost = 2000.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Hair Ornaments", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Hair_Ornament"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 53 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleJewelleryPearlNecklace() => new()
	{
		Metadata = new("earlymodern_noble_jewellery_pearl_necklace", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "necklace",
		ShortDescription = "a lustrous graduated pearl necklace",
		FullDescription = "A lustrous graduated pearl necklace is cut and sewn from pearl so that the outward silhouette of the necklace remains clear. The pearl falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the necklace carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 95,
		Cost = 2600.0m,
		Skinnable = false,
		Material = "pearl",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 19 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedBuckledCourtShoes() => new()
	{
		Metadata = new("earlymodern_noble_shared_buckled_court_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of fine buckled court shoes",
		FullDescription = "A pair of fine buckled court shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 720,
		Cost = 420.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 48 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedDressGloves() => new()
	{
		Metadata = new("earlymodern_noble_shared_dress_gloves", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gloves",
		ShortDescription = "a pair of fine $colour dress gloves",
		FullDescription = "A pair of fine $colour dress gloves is cut and stitched from leather so that the outward silhouette of the gloves remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The body and openings are proportioned for close, practical wear, with reinforcement placed where the gloves bends or fastens rather than spread as decoration. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 240,
		Cost = 300.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gloves", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 72 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedEmbroideredSlippers() => new()
	{
		Metadata = new("earlymodern_noble_shared_embroidered_slippers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "slippers",
		ShortDescription = "a pair of embroidered silk court slippers",
		FullDescription = "A pair of embroidered silk court slippers is cut and sewn from silk so that the outward silhouette of the slippers remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The slippers is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 420,
		Cost = 380.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 66 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineCottonInnerTunic() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_cotton_inner_tunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a fine $colour cotton inner tunic",
		FullDescription = "A fine $colour cotton inner tunic is cut and sewn from cotton so that the outward silhouette of the tunic remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the tunic bends or fastens rather than spread as decoration. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 420,
		Cost = 260.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long-Sleeved_Tunic", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 57 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineCottonLoinDrawers() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_cotton_loin_drawers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "drawers",
		ShortDescription = "a pair of fine $colour cotton loin drawers",
		FullDescription = "A pair of fine $colour cotton loin drawers is cut and sewn from cotton so that the outward silhouette of the drawers remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the drawers carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 150,
		Cost = 110.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shorts", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 27 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineHideLeggings() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_hide_leggings", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "leggings",
		ShortDescription = "a pair of finely dressed hide leggings",
		FullDescription = "A pair of finely dressed hide leggings is cut and stitched from leather so that the outward silhouette of the leggings remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the leggings carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 620,
		Cost = 380.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Leggings", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineLeatherSandals() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_leather_sandals", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sandals",
		ShortDescription = "a pair of finely worked leather sandals",
		FullDescription = "A pair of finely worked leather sandals is cut and stitched from leather so that the outward silhouette of the sandals remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The sandals is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 440,
		Cost = 260.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sandals", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 60 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineLinenDrawers() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_linen_drawers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "drawers",
		ShortDescription = "a pair of fine $colour linen drawers",
		FullDescription = "A pair of fine $colour linen drawers is cut and sewn from linen so that the outward silhouette of the drawers remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the drawers carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 160,
		Cost = 120.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shorts", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 36 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineLinenShift() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_linen_shift", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shift",
		ShortDescription = "a fine long $colour linen shift",
		FullDescription = "A fine long $colour linen shift is cut and sewn from linen so that the outward silhouette of the shift remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the shift bends or fastens rather than spread as decoration. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 620,
		Cost = 300.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long-Sleeved_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 36 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineLinenShirt() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_linen_shirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shirt",
		ShortDescription = "a fine $colour linen court shirt",
		FullDescription = "A fine $colour linen court shirt is cut and sewn from linen so that the outward silhouette of the shirt remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the shirt bends or fastens rather than spread as decoration. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 360,
		Cost = 220.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shirt", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 36 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineSilkInnerRobe() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_silk_inner_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a fine $colour silk inner robe",
		FullDescription = "A fine $colour silk inner robe is cut and sewn from silk so that the outward silhouette of the robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the robe bends or fastens rather than spread as decoration. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 620,
		Cost = 540.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long-Sleeved_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 87 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineSilkSash() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_silk_sash", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sash",
		ShortDescription = "a broad fine $colour silk sash",
		FullDescription = "A broad fine $colour silk sash is cut and sewn from silk so that the outward silhouette of the sash remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the sash carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 260,
		Cost = 340.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sash", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour", "Belt_6"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 72 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedFineWoolWrapLower() => new()
	{
		Metadata = new("earlymodern_noble_shared_fine_wool_wrap_lower", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "skirt",
		ShortDescription = "a fine $colour wool wrap skirt",
		FullDescription = "A fine $colour wool wrap skirt is cut and sewn from wool so that the outward silhouette of the skirt remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the skirt, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 720,
		Cost = 360.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 9 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedHighRidingBoots() => new()
	{
		Metadata = new("earlymodern_noble_shared_high_riding_boots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of polished high riding boots",
		FullDescription = "A pair of polished high riding boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1650,
		Cost = 620.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_LightClothing", "Insulation_Moderate"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 36 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedLaceHeadVeil() => new()
	{
		Metadata = new("earlymodern_noble_shared_lace_head_veil", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "veil",
		ShortDescription = "a fine $colour lace head veil",
		FullDescription = "A fine $colour lace head veil is cut and sewn from lace so that the outward silhouette of the veil remains clear. The lace falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the veil around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 90,
		Cost = 360.0m,
		Skinnable = true,
		Material = "lace",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Head_Veil", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 34 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedSilkFacedStays() => new()
	{
		Metadata = new("earlymodern_noble_shared_silk_faced_stays", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "stays",
		ShortDescription = "a pair of silk-faced $colour stays",
		FullDescription = "A pair of silk-faced $colour stays is cut and sewn from silk so that the outward silhouette of the stays remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the stays carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 760,
		Cost = 620.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Stays", "Armour_HeavyClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 36 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedSilkKneeBreeches() => new()
	{
		Metadata = new("earlymodern_noble_shared_silk_knee_breeches", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "breeches",
		ShortDescription = "a pair of fine $colour silk knee breeches",
		FullDescription = "A pair of fine $colour silk knee breeches is cut and sewn from silk so that the outward silhouette of the breeches remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the breeches, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 520,
		Cost = 460.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Breeches", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 36 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedSilkPetticoat() => new()
	{
		Metadata = new("earlymodern_noble_shared_silk_petticoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "petticoat",
		ShortDescription = "a full fine $colour silk petticoat",
		FullDescription = "A full fine $colour silk petticoat is cut and sewn from silk so that the outward silhouette of the petticoat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the petticoat, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 620,
		Cost = 500.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 60 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedSilkStockings() => new()
	{
		Metadata = new("earlymodern_noble_shared_silk_stockings", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "stockings",
		ShortDescription = "a pair of fine $colour silk stockings",
		FullDescription = "A pair of fine $colour silk stockings is cut and sewn from silk so that the outward silhouette of the stockings remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The stockings is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 230,
		Cost = 320.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Stockings", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 36 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedSilkTrousers() => new()
	{
		Metadata = new("earlymodern_noble_shared_silk_trousers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "trousers",
		ShortDescription = "a pair of fine $colour silk trousers",
		FullDescription = "A pair of fine $colour silk trousers is cut and sewn from silk so that the outward silhouette of the trousers remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the trousers, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 500,
		Cost = 440.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 51 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedSilkWaistcloth() => new()
	{
		Metadata = new("earlymodern_noble_shared_silk_waistcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "waistcloth",
		ShortDescription = "a long fine $colour silk waistcloth",
		FullDescription = "A long fine $colour silk waistcloth is cut and sewn from silk so that the outward silhouette of the waistcloth remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the waistcloth, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 500,
		Cost = 430.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 57 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleSharedSoftHideMoccasins() => new()
	{
		Metadata = new("earlymodern_noble_shared_soft_hide_moccasins", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "moccasins",
		ShortDescription = "a pair of finely worked hide moccasins",
		FullDescription = "A pair of finely worked hide moccasins is cut and stitched from leather so that the outward silhouette of the moccasins remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The moccasins is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 560,
		Cost = 300.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNorthamericanClothingTiedTradeclothHeadband() => new()
	{
		Metadata = new("earlymodern_northamerican_clothing_tied_tradecloth_headband", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headband",
		ShortDescription = "a tied $colour wool headband",
		FullDescription = "A tied $colour wool headband is cut and sewn from wool so that the outward silhouette of the headband remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the headband carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 390,
		Cost = 16.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Headband", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 9 outfit(s). Second-pass military admission; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNorthamericanClothingTradeclothShirt() => new()
	{
		Metadata = new("earlymodern_northamerican_clothing_tradecloth_shirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shirt",
		ShortDescription = "a loose $colour wool trade-cloth shirt",
		FullDescription = "A loose $colour wool trade-cloth shirt is cut and sewn from wool so that the outward silhouette of the shirt remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the shirt bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360,
		Cost = 15.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shirt", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 4 outfit(s). Second-pass military admission; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNorthamericanClothingTradeclothWrapSkirt() => new()
	{
		Metadata = new("earlymodern_northamerican_clothing_tradecloth_wrap_skirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "skirt",
		ShortDescription = "a $colour wool trade-cloth wrap skirt",
		FullDescription = "A $colour wool trade-cloth wrap skirt is cut and sewn from wool so that the outward silhouette of the skirt remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the skirt, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 770,
		Cost = 28.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 2 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNorthamericanClothingWrappedBlanketCoat() => new()
	{
		Metadata = new("earlymodern_northamerican_clothing_wrapped_blanket_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a wrapped $colour wool blanket coat",
		FullDescription = "A wrapped $colour wool blanket coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1230,
		Cost = 61.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 3 outfit(s). Second-pass military admission; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNorthamericanMilitaryFeatheredHeadband() => new()
	{
		Metadata = new("earlymodern_northamerican_military_feathered_headband", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headband",
		ShortDescription = "a feathered leather war headband",
		FullDescription = "A feathered leather war headband is cut and stitched from leather so that the outward silhouette of the headband remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the headband carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 160,
		Cost = 42.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Headband", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNorthamericanMilitarySilverGorget() => new()
	{
		Metadata = new("earlymodern_northamerican_military_silver_gorget", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gorget",
		ShortDescription = "an engraved silver crescent gorget",
		FullDescription = "An engraved silver crescent gorget is worked and joined from silver so that the outward silhouette of the gorget remains clear. The silver has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the gorget carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 240,
		Cost = 120.0m,
		Skinnable = false,
		Material = "silver",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Gorget", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNorthamericanMilitaryTradeclothWarcoat() => new()
	{
		Metadata = new("earlymodern_northamerican_military_tradecloth_warcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a wrapped $colour trade-cloth war coat",
		FullDescription = "A wrapped $colour trade-cloth war coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100,
		Cost = 76.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNorthamericanNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_northamerican_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headband",
		ShortDescription = "a broad feather-and-shell headband",
		FullDescription = "A broad feather-and-shell headband is cut and sewn from feather so that the outward silhouette of the headband remains clear. The feather falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the headband carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 294,
		Cost = 1084.0m,
		Skinnable = false,
		Material = "feather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Headband", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "New culture-family highest-echelon prototype for Indigenous North American regional families; headwear. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNorthamericanNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_northamerican_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "dress",
		ShortDescription = "a full $colour wool trade-cloth wrap dress",
		FullDescription = "A full $colour wool trade-cloth wrap dress is cut and sewn from wool so that the outward silhouette of the dress remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the dress carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1344,
		Cost = 2210.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Indigenous North American regional families; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
