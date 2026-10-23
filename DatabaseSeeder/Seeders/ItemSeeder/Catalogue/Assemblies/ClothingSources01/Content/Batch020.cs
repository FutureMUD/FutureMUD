#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources01Stock
{
	public static CatalogueItemDefinition EarlymodernFrenchNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_french_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a fur-lined $colour broadcloth riding cloak",
		FullDescription = "A fur-lined $colour broadcloth riding cloak is cut and sewn from broadcloth so that the outward silhouette of the cloak remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1258,
		Cost = 1120.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for French / Baroque court and urban; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchNobleRegalia() => new()
	{
		Metadata = new("earlymodern_french_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coronet",
		ShortDescription = "a pearl-set gold coronet",
		FullDescription = "A pearl-set gold coronet is worked and joined from gold so that the outward silhouette of the coronet remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the coronet carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 225,
		Cost = 3645.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Coronets", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Coronet"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for French / Baroque court and urban. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierMilitaryBraidedDolman() => new()
	{
		Metadata = new("earlymodern_frontier_military_braided_dolman", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "dolman",
		ShortDescription = "a braided $colour military dolman",
		FullDescription = "A braided $colour military dolman is cut and sewn from broadcloth so that the outward silhouette of the dolman remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the dolman bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 950,
		Cost = 150.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierMilitaryFoldedFeltCap() => new()
	{
		Metadata = new("earlymodern_frontier_military_folded_felt_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a folded $colour felt frontier cap",
		FullDescription = "A folded $colour felt frontier cap is cut and sewn from felt so that the outward silhouette of the cap remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230,
		Cost = 22.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierMilitaryFurTrimmedPelisse() => new()
	{
		Metadata = new("earlymodern_frontier_military_fur_trimmed_pelisse", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "pelisse",
		ShortDescription = "a fur-trimmed $colour shoulder pelisse",
		FullDescription = "A fur-trimmed $colour shoulder pelisse is cut and sewn from wool so that the outward silhouette of the pelisse remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the pelisse its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 900,
		Cost = 160.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierMilitarySplitSkirtCavalryCaftan() => new()
	{
		Metadata = new("earlymodern_frontier_military_split_skirt_cavalry_caftan", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "caftan",
		ShortDescription = "a split-skirt $colour cavalry caftan",
		FullDescription = "A split-skirt $colour cavalry caftan is cut and sewn from broadcloth so that the outward silhouette of the caftan remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the caftan its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1400,
		Cost = 180.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierMilitaryTallFurCap() => new()
	{
		Metadata = new("earlymodern_frontier_military_tall_fur_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a tall cylindrical fur military cap",
		FullDescription = "A tall cylindrical fur military cap is cut and stitched from fur so that the outward silhouette of the cap remains clear. The fur shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420,
		Cost = 90.0m,
		Skinnable = false,
		Material = "fur",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Strong"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_frontier_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a fur-brimmed $colour velvet court cap",
		FullDescription = "A fur-brimmed $colour velvet court cap is cut and sewn from velvet so that the outward silhouette of the cap remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 246,
		Cost = 796.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Polish-Lithuanian / Hungarian frontier; headwear. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_frontier_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a long $colour silk court gown",
		FullDescription = "A long $colour silk court gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1176,
		Cost = 1490.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Polish-Lithuanian / Hungarian frontier; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_frontier_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a panelled $colour silk state gown",
		FullDescription = "A panelled $colour silk state gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1176,
		Cost = 3240.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Polish-Lithuanian / Hungarian frontier; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_frontier_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a fur-lined $colour velvet riding cloak",
		FullDescription = "A fur-lined $colour velvet riding cloak is cut and sewn from velvet so that the outward silhouette of the cloak remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1314,
		Cost = 1260.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Polish-Lithuanian / Hungarian frontier; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierNobleJewellery() => new()
	{
		Metadata = new("earlymodern_frontier_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "plaques",
		ShortDescription = "a set of gemmed gold belt plaques",
		FullDescription = "A set of gemmed gold belt plaques is worked and joined from gold so that the outward silhouette of the plaques remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the plaques carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 260,
		Cost = 2560.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Belt Plaques", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Belt_Plaques"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for Polish-Lithuanian / Hungarian frontier. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_frontier_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a long $colour silk split-sleeved court coat",
		FullDescription = "A long $colour silk split-sleeved court coat is cut and sewn from silk so that the outward silhouette of the coat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 906,
		Cost = 1224.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Polish-Lithuanian / Hungarian frontier; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_frontier_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad $colour velvet ceremonial mantle",
		FullDescription = "A broad $colour velvet ceremonial mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1106,
		Cost = 2880.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Polish-Lithuanian / Hungarian frontier; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_frontier_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a fur-lined $colour wool riding coat",
		FullDescription = "A fur-lined $colour wool riding coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1314,
		Cost = 1260.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Polish-Lithuanian / Hungarian frontier; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrontierNobleRegalia() => new()
	{
		Metadata = new("earlymodern_frontier_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "diadem",
		ShortDescription = "a pearl-set gold diadem",
		FullDescription = "A pearl-set gold diadem is worked and joined from gold so that the outward silhouette of the diadem remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the diadem carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 260,
		Cost = 3960.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Diadems", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Diadem"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for Polish-Lithuanian / Hungarian frontier. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGermanNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_german_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a plumed $colour felt court hat",
		FullDescription = "A plumed $colour felt court hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 240,
		Cost = 760.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for German / HRE / Austrian; headwear. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGermanNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_german_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a square-necked $colour silk court gown",
		FullDescription = "A square-necked $colour silk court gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1155,
		Cost = 1400.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for German / HRE / Austrian; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGermanNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_german_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "an embroidered $colour silk state gown",
		FullDescription = "An embroidered $colour silk state gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1155,
		Cost = 3150.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for German / HRE / Austrian; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGermanNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_german_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a fur-lined $colour velvet travelling cloak",
		FullDescription = "A fur-lined $colour velvet travelling cloak is cut and sewn from velvet so that the outward silhouette of the cloak remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1290,
		Cost = 1200.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for German / HRE / Austrian; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGermanNobleJewellery() => new()
	{
		Metadata = new("earlymodern_german_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "chain",
		ShortDescription = "a gemmed gold chain of estate",
		FullDescription = "A gemmed gold chain of estate is worked and joined from gold so that the outward silhouette of the chain remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the chain carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 245,
		Cost = 2425.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Neck Rings", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Neck_Ring"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for German / HRE / Austrian. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGermanNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_german_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a braid-guarded $colour silk court coat",
		FullDescription = "A braid-guarded $colour silk court coat is cut and sewn from silk so that the outward silhouette of the coat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 885,
		Cost = 1140.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for German / HRE / Austrian; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGermanNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_german_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a fur-edged $colour velvet princely mantle",
		FullDescription = "A fur-edged $colour velvet princely mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1085,
		Cost = 2775.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for German / HRE / Austrian; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGermanNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_german_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a fur-collared $colour broadcloth riding cloak",
		FullDescription = "A fur-collared $colour broadcloth riding cloak is cut and sewn from broadcloth so that the outward silhouette of the cloak remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1290,
		Cost = 1200.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for German / HRE / Austrian; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGermanNobleRegalia() => new()
	{
		Metadata = new("earlymodern_german_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coronet",
		ShortDescription = "a gem-set gold princely coronet",
		FullDescription = "A gem-set gold princely coronet is worked and joined from gold so that the outward silhouette of the coronet remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the coronet carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 245,
		Cost = 3825.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Coronets", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Coronet"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for German / HRE / Austrian. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_globalmaritime_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a feathered $colour felt command hat",
		FullDescription = "A feathered $colour felt command hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 302,
		Cost = 1132.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Global maritime and chartered-company trade; headwear. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_globalmaritime_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a fitted $colour silk director-household gown",
		FullDescription = "A fitted $colour silk director-household gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1372,
		Cost = 2330.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Global maritime and chartered-company trade; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_globalmaritime_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a long-trained $colour silk formal gown",
		FullDescription = "A long-trained $colour silk formal gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1372,
		Cost = 4080.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Global maritime and chartered-company trade; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_globalmaritime_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a hooded $colour broadcloth sea-travel cloak",
		FullDescription = "A hooded $colour broadcloth sea-travel cloak is cut and sewn from broadcloth so that the outward silhouette of the cloak remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1538,
		Cost = 1820.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Global maritime and chartered-company trade; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeNobleJewellery() => new()
	{
		Metadata = new("earlymodern_globalmaritime_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "brooch",
		ShortDescription = "a gem-set gold command brooch",
		FullDescription = "A gem-set gold command brooch is worked and joined from gold so that the outward silhouette of the brooch remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the brooch carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 198,
		Cost = 3820.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Brooches", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Brooch"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for Global maritime and chartered-company trade. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_globalmaritime_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a gold-laced $colour silk command coat",
		FullDescription = "A gold-laced $colour silk command coat is cut and sewn from silk so that the outward silhouette of the coat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1102,
		Cost = 2008.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Global maritime and chartered-company trade; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_globalmaritime_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a gold-edged $colour velvet ceremonial mantle",
		FullDescription = "A gold-edged $colour velvet ceremonial mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1302,
		Cost = 3860.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Global maritime and chartered-company trade; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_globalmaritime_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a long $colour broadcloth sea-travel cloak",
		FullDescription = "A long $colour broadcloth sea-travel cloak is cut and sewn from broadcloth so that the outward silhouette of the cloak remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1538,
		Cost = 1820.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Global maritime and chartered-company trade; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeNobleRegalia() => new()
	{
		Metadata = new("earlymodern_globalmaritime_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "chain",
		ShortDescription = "a heavy gold charter chain",
		FullDescription = "A heavy gold charter chain is worked and joined from gold so that the outward silhouette of the chain remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the chain carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 400,
		Cost = 5220.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Neck Rings", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Neck_Ring"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for Global maritime and chartered-company trade. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeOfficerCompanyFrock() => new()
	{
		Metadata = new("earlymodern_globalmaritime_officer_company_frock", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "frock",
		ShortDescription = "a long $colour company officer's frock",
		FullDescription = "A long $colour company officer's frock is cut and sewn from broadcloth so that the outward silhouette of the frock remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the frock carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1580,
		Cost = 230.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
