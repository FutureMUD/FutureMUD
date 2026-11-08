#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources02Stock
{
	public static CatalogueItemDefinition EarlymodernMainlandseasiaNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_mainlandseasia_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a light $colour silk river-travel cloak",
		FullDescription = "A light $colour silk river-travel cloak is cut and sewn from silk so that the outward silhouette of the cloak remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1418,
		Cost = 1520.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mainland South-east Asian courts; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMainlandseasiaNobleRegalia() => new()
	{
		Metadata = new("earlymodern_mainlandseasia_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "ornament",
		ShortDescription = "a chased-gold waist ornament",
		FullDescription = "A chased-gold waist ornament is worked and joined from gold so that the outward silhouette of the ornament remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the ornament carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 153,
		Cost = 4545.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Waist Ornaments", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Waist_Ornament"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for Mainland South-east Asian courts. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaClothingLightOpenBlouse() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_clothing_light_open_blouse", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "blouse",
		ShortDescription = "a light $colour cotton open blouse",
		FullDescription = "A light $colour cotton open blouse is cut and sewn from cotton so that the outward silhouette of the blouse remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the blouse bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 580,
		Cost = 32.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 4 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaClothingLooseLongTunic() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_clothing_loose_long_tunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a loose $colour cotton long tunic",
		FullDescription = "A loose $colour cotton long tunic is cut and sewn from cotton so that the outward silhouette of the tunic remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the tunic bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 540,
		Cost = 23.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long-Sleeved_Tunic", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 4 outfit(s). Second-pass military admission; used in 7 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaClothingPatternedSarong() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_clothing_patterned_sarong", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sarong",
		ShortDescription = "a $colour1 and $colour2 patterned cotton sarong",
		FullDescription = "A $colour1 and $colour2 patterned cotton sarong is cut and sewn from cotton so that the outward silhouette of the sarong remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the sarong, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 630,
		Cost = 26.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_2BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 6 outfit(s). Second-pass military admission; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaMilitaryCommandHeadcloth() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_military_command_headcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headcloth",
		ShortDescription = "a folded $colour maritime command headcloth",
		FullDescription = "A folded $colour maritime command headcloth is cut and sewn from silk so that the outward silhouette of the headcloth remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the headcloth around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 280,
		Cost = 60.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 6 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaMilitaryOfficerOverrobe() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_military_officer_overrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a fine $colour maritime command over-robe",
		FullDescription = "A fine $colour maritime command over-robe is cut and sewn from silk so that the outward silhouette of the robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 950,
		Cost = 150.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaMilitaryShortCoat() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_military_short_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a short $colour coastal military coat",
		FullDescription = "A short $colour coastal military coat is cut and sewn from cotton so that the outward silhouette of the coat remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650,
		Cost = 50.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNavalOfficerJacket() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_naval_officer_jacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a fitted $colour naval officer's jacket",
		FullDescription = "A fitted $colour naval officer's jacket is cut and sewn from broadcloth so that the outward silhouette of the jacket remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jacket bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 850,
		Cost = 125.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headcloth",
		ShortDescription = "a jewelled $colour silk folded headcloth",
		FullDescription = "A jewelled $colour silk folded headcloth is cut and sewn from silk so that the outward silhouette of the headcloth remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the headcloth around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 274,
		Cost = 964.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Maritime South-east Asian trade worlds; headwear. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a tubular-skirted $colour silk court gown",
		FullDescription = "A tubular-skirted $colour silk court gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1274,
		Cost = 1910.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Maritime South-east Asian trade worlds; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a panelled $colour silk formal gown",
		FullDescription = "A panelled $colour silk formal gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1274,
		Cost = 3660.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Maritime South-east Asian trade worlds; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloth",
		ShortDescription = "a fringed $colour silk travelling shoulder cloth",
		FullDescription = "A fringed $colour silk travelling shoulder cloth is cut and sewn from silk so that the outward silhouette of the cloth remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloth its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1426,
		Cost = 1540.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Maritime South-east Asian trade worlds; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNobleJewellery() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "necklace",
		ShortDescription = "a heavy gold pendant necklace",
		FullDescription = "A heavy gold pendant necklace is worked and joined from gold so that the outward silhouette of the necklace remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the necklace carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 156,
		Cost = 3190.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for Maritime South-east Asian trade worlds. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a long $colour silk port-court jacket",
		FullDescription = "A long $colour silk port-court jacket is cut and sewn from silk so that the outward silhouette of the jacket remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the jacket its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1004,
		Cost = 1616.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Maritime South-east Asian trade worlds; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad $colour velvet ceremonial mantle",
		FullDescription = "A broad $colour velvet ceremonial mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1204,
		Cost = 3370.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Maritime South-east Asian trade worlds; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a light $colour silk rain-travel cloak",
		FullDescription = "A light $colour silk rain-travel cloak is cut and sewn from silk so that the outward silhouette of the cloak remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1426,
		Cost = 1540.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Maritime South-east Asian trade worlds; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMaritimeseasiaNobleRegalia() => new()
	{
		Metadata = new("earlymodern_maritimeseasia_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "chain",
		ShortDescription = "a gemmed gold waist chain",
		FullDescription = "A gemmed gold waist chain is worked and joined from gold so that the outward silhouette of the chain remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the chain carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 156,
		Cost = 4590.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Waist Chains", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Waist_Chain"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for Maritime South-east Asian trade worlds. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanMilitaryCommandHeadband() => new()
	{
		Metadata = new("earlymodern_mesoamerican_military_command_headband", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headband",
		ShortDescription = "a broad $colour woven command headband",
		FullDescription = "A broad $colour woven command headband is cut and sewn from cotton so that the outward silhouette of the headband remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the headband carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150,
		Cost = 34.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Headband", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanMilitaryMilitiaJacket() => new()
	{
		Metadata = new("earlymodern_mesoamerican_military_militia_jacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a short $colour cotton militia jacket",
		FullDescription = "A short $colour cotton militia jacket is cut and sewn from cotton so that the outward silhouette of the jacket remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jacket bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 600,
		Cost = 42.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanMilitaryQuiltedTunic() => new()
	{
		Metadata = new("earlymodern_mesoamerican_military_quilted_tunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a quilted $colour cotton military tunic",
		FullDescription = "A quilted $colour cotton military tunic is cut and sewn from cotton so that the outward silhouette of the tunic remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the tunic bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100,
		Cost = 58.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long-Sleeved_Tunic", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_mesoamerican_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a close-woven $colour court cap",
		FullDescription = "A close-woven $colour court cap is cut and sewn from cotton so that the outward silhouette of the cap remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 296,
		Cost = 1096.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mesoamerican colonial and Indigenous; headwear. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_mesoamerican_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "an embroidered $colour cotton court gown",
		FullDescription = "An embroidered $colour cotton court gown is cut and sewn from cotton so that the outward silhouette of the gown remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1351,
		Cost = 2240.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mesoamerican colonial and Indigenous; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_mesoamerican_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a panelled $colour silk formal gown",
		FullDescription = "A panelled $colour silk formal gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1351,
		Cost = 3990.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mesoamerican colonial and Indigenous; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_mesoamerican_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad $colour wool travelling mantle",
		FullDescription = "A broad $colour wool travelling mantle is cut and sewn from wool so that the outward silhouette of the mantle remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1514,
		Cost = 1760.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mesoamerican colonial and Indigenous; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanNobleJewellery() => new()
	{
		Metadata = new("earlymodern_mesoamerican_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "necklace",
		ShortDescription = "a gold-and-jade court necklace",
		FullDescription = "A gold-and-jade court necklace is worked and joined from gold so that the outward silhouette of the necklace remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the necklace carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 189,
		Cost = 3685.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for Mesoamerican colonial and Indigenous. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_mesoamerican_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a long embroidered $colour cotton court tunic",
		FullDescription = "A long embroidered $colour cotton court tunic is cut and sewn from cotton so that the outward silhouette of the tunic remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the tunic its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1081,
		Cost = 1924.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mesoamerican colonial and Indigenous; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_mesoamerican_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a gold-bordered $colour velvet ceremonial mantle",
		FullDescription = "A gold-bordered $colour velvet ceremonial mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1281,
		Cost = 3755.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mesoamerican colonial and Indigenous; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_mesoamerican_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad $colour wool travelling mantle",
		FullDescription = "A broad $colour wool travelling mantle is cut and sewn from wool so that the outward silhouette of the mantle remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1514,
		Cost = 1760.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Mesoamerican colonial and Indigenous; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMesoamericanNobleRegalia() => new()
	{
		Metadata = new("earlymodern_mesoamerican_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "ornament",
		ShortDescription = "a gem-set gold forehead ornament",
		FullDescription = "A gem-set gold forehead ornament is worked and joined from gold so that the outward silhouette of the ornament remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the ornament carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 189,
		Cost = 5085.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Forehead Ornaments", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Forehead_Ornament"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for Mesoamerican colonial and Indigenous. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankBraidedShoulderCord() => new()
	{
		Metadata = new("earlymodern_military_rank_braided_shoulder_cord", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cord",
		ShortDescription = "a braided $colour shoulder cord",
		FullDescription = "A braided $colour shoulder cord is cut and sewn from ribbon so that the outward silhouette of the cord remains clear. The ribbon falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the cord carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 100,
		Cost = 44.0m,
		Skinnable = true,
		Material = "ribbon",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoulder", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 7 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankBraidedShoulderKnots() => new()
	{
		Metadata = new("earlymodern_military_rank_braided_shoulder_knots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "knots",
		ShortDescription = "a pair of braided $colour shoulder knots",
		FullDescription = "A pair of braided $colour shoulder knots is cut and sewn from ribbon so that the outward silhouette of the knots remains clear. The ribbon falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the knots carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 160,
		Cost = 64.0m,
		Skinnable = true,
		Material = "ribbon",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoulders", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 11 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankBrassBreastBadge() => new()
	{
		Metadata = new("earlymodern_military_rank_brass_breast_badge", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "badge",
		ShortDescription = "a stamped brass breast badge",
		FullDescription = "A stamped brass breast badge is worked and joined from brass so that the outward silhouette of the badge remains clear. The brass has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the badge carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 90,
		Cost = 36.0m,
		Skinnable = true,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Badge", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 2 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankBrassBuckledSwordBelt() => new()
	{
		Metadata = new("earlymodern_military_rank_brass_buckled_sword_belt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "belt",
		ShortDescription = "a brass-buckled leather sword belt",
		FullDescription = "A brass-buckled leather sword belt is cut and stitched from leather so that the outward silhouette of the belt remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the belt carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520,
		Cost = 74.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Waist", "Armour_LightClothing", "Insulation_Minor", "Belt_Large"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 47 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryRankBrassOfficerGorget() => new()
	{
		Metadata = new("earlymodern_military_rank_brass_officer_gorget", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gorget",
		ShortDescription = "a polished brass officer's gorget",
		FullDescription = "A polished brass officer's gorget is worked and joined from brass so that the outward silhouette of the gorget remains clear. The brass has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the gorget carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 320,
		Cost = 96.0m,
		Skinnable = true,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Gorget", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Reusable skinnable service/rank accessory with valid base presentation; used in 16 military outfit(s). Exact appointment and insignia belong in skins.",
		AllowLegacyShortDescriptionMatch = false,
	};
}
