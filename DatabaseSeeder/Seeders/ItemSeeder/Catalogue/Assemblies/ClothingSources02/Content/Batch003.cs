#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources02Stock
{
	public static CatalogueItemDefinition EarlymodernMilitaryRankBroadOfficerSash() => new()
	{
		Metadata = new("earlymodern_military_rank_broad_officer_sash", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sash",
		ShortDescription = "a broad $colour silk officer's sash",
		FullDescription = "A broad $colour silk officer's sash is cut and sewn from silk so that the outward silhouette of the sash remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the sash carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 320,
		Cost = 88.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sash", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour", "Belt_6"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 79 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankEmbroideredClothBadge() => new()
	{
		Metadata = new("earlymodern_military_rank_embroidered_cloth_badge", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "badge",
		ShortDescription = "an embroidered $colour cloth badge",
		FullDescription = "An embroidered $colour cloth badge is cut and sewn from silk so that the outward silhouette of the badge remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the badge carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 45,
		Cost = 30.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Badge", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 46 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankPairedFringedEpaulettes() => new()
	{
		Metadata = new("earlymodern_military_rank_paired_fringed_epaulettes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "epaulettes",
		ShortDescription = "a pair of fringed $colour epaulettes",
		FullDescription = "A pair of fringed $colour epaulettes is cut and sewn from silk so that the outward silhouette of the epaulettes remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the epaulettes carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 220,
		Cost = 96.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Spaulders", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 10 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankSilkCommandScarf() => new()
	{
		Metadata = new("earlymodern_military_rank_silk_command_scarf", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "scarf",
		ShortDescription = "a fine $colour silk command scarf",
		FullDescription = "A fine $colour silk command scarf is cut and sewn from silk so that the outward silhouette of the scarf remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the scarf carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140,
		Cost = 54.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Scarf", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 28 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankSingleFringedEpaulette() => new()
	{
		Metadata = new("earlymodern_military_rank_single_fringed_epaulette", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "epaulette",
		ShortDescription = "a fringed $colour shoulder epaulette",
		FullDescription = "A fringed $colour shoulder epaulette is cut and sewn from silk so that the outward silhouette of the epaulette remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the epaulette carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 110,
		Cost = 52.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Epaulette", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 15 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankWoolServiceSash() => new()
	{
		Metadata = new("earlymodern_military_rank_wool_service_sash", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sash",
		ShortDescription = "a broad $colour wool service sash",
		FullDescription = "A broad $colour wool service sash is cut and sewn from wool so that the outward silhouette of the sash remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the sash carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360,
		Cost = 32.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sash", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour", "Belt_6"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 20 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryUniformPlainLeatherCrossbelt() => new()
	{
		Metadata = new("earlymodern_military_uniform_plain_leather_crossbelt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "crossbelt",
		ShortDescription = "a broad leather service crossbelt",
		FullDescription = "A broad leather service crossbelt is cut and stitched from leather so that the outward silhouette of the crossbelt remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the crossbelt carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 400,
		Cost = 30.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Bandolier", "Armour_LightClothing", "Insulation_Minor", "Belt_6"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 50 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryUniformWhiteLeatherCrossbelt() => new()
	{
		Metadata = new("earlymodern_military_uniform_white_leather_crossbelt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "crossbelt",
		ShortDescription = "a broad white leather crossbelt",
		FullDescription = "A broad white leather crossbelt is cut and stitched from leather so that the outward silhouette of the crossbelt remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the crossbelt carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 38.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Bandolier", "Armour_LightClothing", "Insulation_Minor", "Belt_6"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 25 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingMilitaryBrimmedHat() => new()
	{
		Metadata = new("earlymodern_ming_military_brimmed_hat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a broad-brimmed $colour military hat",
		FullDescription = "A broad-brimmed $colour military hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280,
		Cost = 25.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 5 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingMilitaryQuiltedJacket() => new()
	{
		Metadata = new("earlymodern_ming_military_quilted_jacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a quilted $colour military jacket",
		FullDescription = "A quilted $colour military jacket is cut and sewn from cotton so that the outward silhouette of the jacket remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jacket bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300,
		Cost = 70.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingMilitaryRoundcollarServiceRobe() => new()
	{
		Metadata = new("earlymodern_ming_military_roundcollar_service_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a round-collared $colour service robe",
		FullDescription = "A round-collared $colour service robe is cut and sewn from cotton so that the outward silhouette of the robe remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900,
		Cost = 65.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNavalOfficerOverrobe() => new()
	{
		Metadata = new("earlymodern_ming_naval_officer_overrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a straight-collared $colour naval over-robe",
		FullDescription = "A straight-collared $colour naval over-robe is cut and sewn from silk so that the outward silhouette of the robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1000,
		Cost = 160.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_ming_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a structured black silk court hat",
		FullDescription = "A structured black silk court hat is cut and sewn from silk so that the outward silhouette of the hat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 264,
		Cost = 904.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "New culture-family highest-echelon prototype for Late Ming survival / transition; headwear. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_ming_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a panel-skirted $colour silk court gown",
		FullDescription = "A panel-skirted $colour silk court gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1239,
		Cost = 1760.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Late Ming survival / transition; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_ming_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a broad-sleeved $colour silk formal gown",
		FullDescription = "A broad-sleeved $colour silk formal gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1239,
		Cost = 3510.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Late Ming survival / transition; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_ming_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a long $colour silk travelling cloak",
		FullDescription = "A long $colour silk travelling cloak is cut and sewn from silk so that the outward silhouette of the cloak remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1386,
		Cost = 1440.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Late Ming survival / transition; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNobleJewellery() => new()
	{
		Metadata = new("earlymodern_ming_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "plaques",
		ShortDescription = "a set of carved jade belt plaques",
		FullDescription = "A set of carved jade belt plaques is cut and sewn from jade so that the outward silhouette of the plaques remains clear. The jade falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the plaques carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 305,
		Cost = 2965.0m,
		Skinnable = false,
		Material = "jade",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Belt Plaques", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Belt_Plaques"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for Late Ming survival / transition. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_ming_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a round-collared $colour silk court robe",
		FullDescription = "A round-collared $colour silk court robe is cut and sewn from silk so that the outward silhouette of the robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 969,
		Cost = 1476.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Late Ming survival / transition; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_ming_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad-sleeved $colour velvet ceremonial mantle",
		FullDescription = "A broad-sleeved $colour velvet ceremonial mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1169,
		Cost = 3195.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Late Ming survival / transition; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_ming_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a straight-collared $colour silk travelling cloak",
		FullDescription = "A straight-collared $colour silk travelling cloak is cut and sewn from silk so that the outward silhouette of the cloak remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1386,
		Cost = 1440.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Late Ming survival / transition; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMingNobleRegalia() => new()
	{
		Metadata = new("earlymodern_ming_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "diadem",
		ShortDescription = "a chased-gold court diadem",
		FullDescription = "A chased-gold court diadem is worked and joined from gold so that the outward silhouette of the diadem remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the diadem carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 305,
		Cost = 4365.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Diadems", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Diadem"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for Late Ming survival / transition. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalMilitaryFanCrestedTurban() => new()
	{
		Metadata = new("earlymodern_mughal_military_fan_crested_turban", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "turban",
		ShortDescription = "a fan-crested $colour officer's turban",
		FullDescription = "A fan-crested $colour officer's turban is cut and sewn from silk so that the outward silhouette of the turban remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the turban around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 350,
		Cost = 90.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 5 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalMilitaryFlaredJama() => new()
	{
		Metadata = new("earlymodern_mughal_military_flared_jama", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jama",
		ShortDescription = "a flared $colour military jama",
		FullDescription = "A flared $colour military jama is cut and sewn from cotton so that the outward silhouette of the jama remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the jama its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1000,
		Cost = 110.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalMilitaryQuiltedJama() => new()
	{
		Metadata = new("earlymodern_mughal_military_quilted_jama", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jama",
		ShortDescription = "a quilted $colour campaign jama",
		FullDescription = "A quilted $colour campaign jama is cut and sewn from cotton so that the outward silhouette of the jama remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the jama its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500,
		Cost = 75.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalMilitarySleevelessCommandCoat() => new()
	{
		Metadata = new("earlymodern_mughal_military_sleeveless_command_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a sleeveless $colour command overcoat",
		FullDescription = "A sleeveless $colour command overcoat is cut and sewn from silk so that the outward silhouette of the coat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 700,
		Cost = 140.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_mughal_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "turban",
		ShortDescription = "a fan-crested $colour silk court turban",
		FullDescription = "A fan-crested $colour silk court turban is cut and sewn from silk so that the outward silhouette of the turban remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the turban around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 256,
		Cost = 856.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mughal / Indo-Persian; headwear. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_mughal_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a layered $colour silk court gown",
		FullDescription = "A layered $colour silk court gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1211,
		Cost = 1640.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mughal / Indo-Persian; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_mughal_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a full-panelled $colour silk formal gown",
		FullDescription = "A full-panelled $colour silk formal gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1211,
		Cost = 3390.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mughal / Indo-Persian; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_mughal_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "an embroidered $colour silk travelling cloak",
		FullDescription = "An embroidered $colour silk travelling cloak is cut and sewn from silk so that the outward silhouette of the cloak remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1354,
		Cost = 1360.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mughal / Indo-Persian; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalNobleJewellery() => new()
	{
		Metadata = new("earlymodern_mughal_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "necklace",
		ShortDescription = "a pearl-and-gem gold court necklace",
		FullDescription = "A pearl-and-gem gold court necklace is worked and joined from gold so that the outward silhouette of the necklace remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the necklace carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 129,
		Cost = 2785.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for Mughal / Indo-Persian. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_mughal_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jama",
		ShortDescription = "a full-skirted $colour silk court jama",
		FullDescription = "A full-skirted $colour silk court jama is cut and sewn from silk so that the outward silhouette of the jama remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the jama its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 941,
		Cost = 1364.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mughal / Indo-Persian; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_mughal_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a wide $colour velvet ceremonial mantle",
		FullDescription = "A wide $colour velvet ceremonial mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1141,
		Cost = 3055.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mughal / Indo-Persian; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_mughal_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a quilted $colour silk riding coat",
		FullDescription = "A quilted $colour silk riding coat is cut and sewn from silk so that the outward silhouette of the coat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1354,
		Cost = 1360.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mughal / Indo-Persian; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMughalNobleRegalia() => new()
	{
		Metadata = new("earlymodern_mughal_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "ornament",
		ShortDescription = "a jewelled gold turban ornament",
		FullDescription = "A jewelled gold turban ornament is worked and joined from gold so that the outward silhouette of the ornament remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the ornament carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 129,
		Cost = 4185.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Forehead Ornaments", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Forehead_Ornament"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for Mughal / Indo-Persian. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernNobleJewelleryGemEarrings() => new()
	{
		Metadata = new("earlymodern_noble_jewellery_gem_earrings", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "earrings",
		ShortDescription = "a pair of gem-set gold earrings",
		FullDescription = "A pair of gem-set gold earrings is worked and joined from gold so that the outward silhouette of the earrings remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the earrings carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 32,
		Cost = 2200.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Earrings", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Earrings"],
		BuilderNotes = "Shared highest-echelon foundation or jewellery prototype. Used in 108 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
