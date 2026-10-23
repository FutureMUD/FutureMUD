#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources02Stock
{
	public static CatalogueItemDefinition EarlymodernQingNavalOfficerCommandCoat() => new()
	{
		Metadata = new("earlymodern_qing_naval_officer_command_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a fine $colour naval command coat",
		FullDescription = "A fine $colour naval command coat is cut and sewn from silk so that the outward silhouette of the coat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100,
		Cost = 180.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernQingNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_qing_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a domed $colour velvet court hat",
		FullDescription = "A domed $colour velvet court hat is cut and sewn from velvet so that the outward silhouette of the hat remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 262,
		Cost = 892.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Qing China; headwear. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernQingNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_qing_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a long $colour silk court robe-gown",
		FullDescription = "A long $colour silk court robe-gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1232,
		Cost = 1730.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Qing China; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernQingNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_qing_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "an embroidered $colour silk formal robe-gown",
		FullDescription = "An embroidered $colour silk formal robe-gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1232,
		Cost = 3480.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Qing China; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernQingNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_qing_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a fur-edged $colour silk travelling cloak",
		FullDescription = "A fur-edged $colour silk travelling cloak is cut and sewn from silk so that the outward silhouette of the cloak remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1378,
		Cost = 1420.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Qing China; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernQingNobleJewellery() => new()
	{
		Metadata = new("earlymodern_qing_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "necklace",
		ShortDescription = "a long jade-and-gold court necklace",
		FullDescription = "A long jade-and-gold court necklace is cut and sewn from jade so that the outward silhouette of the necklace remains clear. The jade falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the necklace carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 138,
		Cost = 2920.0m,
		Skinnable = false,
		Material = "jade",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for Qing China. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernQingNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_qing_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a round-collared $colour silk court robe",
		FullDescription = "A round-collared $colour silk court robe is cut and sewn from silk so that the outward silhouette of the robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 962,
		Cost = 1448.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Qing China; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernQingNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_qing_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a full $colour velvet ceremonial mantle",
		FullDescription = "A full $colour velvet ceremonial mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1162,
		Cost = 3160.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Qing China; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernQingNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_qing_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a fur-lined $colour silk travelling robe",
		FullDescription = "A fur-lined $colour silk travelling robe is cut and sewn from silk so that the outward silhouette of the robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1378,
		Cost = 1420.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Qing China; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernQingNobleRegalia() => new()
	{
		Metadata = new("earlymodern_qing_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "ornament",
		ShortDescription = "a gem-set gold head ornament",
		FullDescription = "A gem-set gold head ornament is worked and joined from gold so that the outward silhouette of the ornament remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the ornament carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 300,
		Cost = 4320.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Diadems", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Diadem"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for Qing China. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRedseaClothingNarrowCottonTrousers() => new()
	{
		Metadata = new("earlymodern_redsea_clothing_narrow_cotton_trousers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "trousers",
		ShortDescription = "a pair of narrow $colour cotton trousers",
		FullDescription = "A pair of narrow $colour cotton trousers is cut and sewn from cotton so that the outward silhouette of the trousers remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the trousers, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500,
		Cost = 21.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 6 outfit(s). Second-pass military admission; used in 7 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousBektashiTaj() => new()
	{
		Metadata = new("earlymodern_religious_bektashi_taj", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a twelve-gored felt dervish cap",
		FullDescription = "A twelve-gored felt dervish cap is cut and sewn from felt so that the outward silhouette of the cap remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260,
		Cost = 88.0m,
		Skinnable = false,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Twelve-gored Bektashi-associated taj; exact colour, crown, lodge, and lineage are skins. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousBuddhistPrayerBeads() => new()
	{
		Metadata = new("earlymodern_religious_buddhist_prayer_beads", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "beads",
		ShortDescription = "a long loop of dark monastic beads",
		FullDescription = "A long loop of dark monastic beads is shaped and bound from wood so that the outward silhouette of the beads remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The contact points and fastenings are kept smooth, while the visible body of the beads carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 105,
		Cost = 52.0m,
		Skinnable = false,
		Material = "wood",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Bead Strings", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Wearable Buddhist prayer beads; exact count, school, material, and use are skins. Used in 14 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousCapuchinPointedCowl() => new()
	{
		Metadata = new("earlymodern_religious_capuchin_pointed_cowl", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cowl",
		ShortDescription = "a long-pointed brown wool cowl",
		FullDescription = "A long-pointed brown wool cowl is cut and sewn from wool so that the outward silhouette of the cowl remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cowl its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820,
		Cost = 120.0m,
		Skinnable = false,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Closed)", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "Distinctive long pointed hood and shoulder cowl of the Capuchin reform, approved in 1528 and continuing through the Early Modern period. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousDaoistCloudShoes() => new()
	{
		Metadata = new("earlymodern_religious_daoist_cloud_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of $colour cloud-toed shoes",
		FullDescription = "These cloth shoes have $colour cotton uppers with softly rounded sides and raised, curling toes. A narrow seam follows each instep, joining panels whose close weave lies smooth against the padded lining. Several layers of cloth form the flat soles, secured by rows of small stitches. The lifted toe fronts make the outline distinctive, while the cuffs and outer panels carry no embroidered badge or contrasting border.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420,
		Cost = 96.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousDaoistLotusCrown() => new()
	{
		Metadata = new("earlymodern_religious_daoist_lotus_crown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "crown",
		ShortDescription = "a small gilt lotus ritual crown",
		FullDescription = "A small gilt lotus ritual crown is worked and joined from brass so that the outward silhouette of the crown remains clear. The brass has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the crown carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 260,
		Cost = 360.0m,
		Skinnable = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Coronets", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Coronet"],
		BuilderNotes = "Daoist ritual crown; exact rank, deity, register, and ceremony are skins/admissions. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEastasianBrocadeKasaya() => new()
	{
		Metadata = new("earlymodern_religious_eastasian_brocade_kasaya", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kasaya",
		ShortDescription = "a richly pieced silk monastic mantle",
		FullDescription = "A richly pieced silk monastic mantle is cut and sewn from silk so that the outward silhouette of the kasaya remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the kasaya its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 840,
		Cost = 980.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Early Modern East Asian formal monastic mantle, including seventeenth- and eighteenth-century Japanese kesa made from prized textiles. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEasternCrownMitre() => new()
	{
		Metadata = new("earlymodern_religious_eastern_crown_mitre", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mitre",
		ShortDescription = "a jewel-edged crown-shaped mitre",
		FullDescription = "A jewel-edged crown-shaped mitre is cut and sewn from silk so that the outward silhouette of the mitre remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the mitre around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 420,
		Cost = 1320.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Crown-shaped Orthodox mitre; date and church admission required, especially for seventeenth-century Russian use. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEasternEpigonation() => new()
	{
		Metadata = new("earlymodern_religious_eastern_epigonation", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "epigonation",
		ShortDescription = "a diamond-shaped embroidered epigonation",
		FullDescription = "A diamond-shaped embroidered epigonation is cut and sewn from silk so that the outward silhouette of the epigonation remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the epigonation carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 170,
		Cost = 240.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Girdle_Ornament", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Liturgical side ornament for bishops and honoured priests; exact cross, embroidery, and award are skins. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEasternPanagia() => new()
	{
		Metadata = new("earlymodern_religious_eastern_panagia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "pendant",
		ShortDescription = "a painted oval panagia pendant",
		FullDescription = "A painted oval panagia pendant is worked and joined from silver so that the outward silhouette of the pendant remains clear. The silver has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the pendant carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Great,
		WeightInGrams = 145,
		Cost = 920.0m,
		Skinnable = false,
		Material = "silver",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Bishop or senior-abbot devotional medallion; exact icon and office are skins. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEasternPectoralCross() => new()
	{
		Metadata = new("earlymodern_religious_eastern_pectoral_cross", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cross",
		ShortDescription = "a silver Eastern pectoral cross",
		FullDescription = "A silver Eastern pectoral cross is worked and joined from silver so that the outward silhouette of the cross remains clear. The silver has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the cross carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 120,
		Cost = 460.0m,
		Skinnable = false,
		Material = "silver",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Clerical pectoral cross; exact form, inscription, office, and local tradition are skins. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEasternPrayerRope() => new()
	{
		Metadata = new("earlymodern_religious_eastern_prayer_rope", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "prayer rope",
		ShortDescription = "a loop of dark knotted prayer cord",
		FullDescription = "A loop of dark knotted prayer cord is cut and sewn from wool so that the outward silhouette of the prayer rope remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the prayer rope bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 55,
		Cost = 36.0m,
		Skinnable = false,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Bracelet"],
		BuilderNotes = "Wearable Eastern Christian prayer rope; knot count, tassel, local name, blessing, and devotional use are skins/admissions. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEasternSkufia() => new()
	{
		Metadata = new("earlymodern_religious_eastern_skufia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "skufia",
		ShortDescription = "a soft black clerical skufia",
		FullDescription = "A soft black clerical skufia is cut and sewn from broadcloth so that the outward silhouette of the skufia remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the skufia carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 110,
		Cost = 72.0m,
		Skinnable = false,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Skullcap", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Soft pointed or rounded clerical cap; local Orthodox tradition and honour are builder admissions. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEthiopianClericalCape() => new()
	{
		Metadata = new("earlymodern_religious_ethiopian_clerical_cape", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cape",
		ShortDescription = "an embroidered white clerical cape",
		FullDescription = "An embroidered white clerical cape is cut and sewn from cotton so that the outward silhouette of the cape remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cape its recognisable proportion when worn. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 760,
		Cost = 260.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cape", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Embroidered ceremonial cape for Ethiopian clergy and processions; exact cross pattern and rank are skins. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEthiopianPectoralCross() => new()
	{
		Metadata = new("earlymodern_religious_ethiopian_pectoral_cross", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cross",
		ShortDescription = "a broad silver clerical cross pendant",
		FullDescription = "A broad silver clerical cross pendant is worked and joined from silver so that the outward silhouette of the cross remains clear. The silver has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the cross carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 180,
		Cost = 540.0m,
		Skinnable = false,
		Material = "silver",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Wearable Ethiopian clerical cross; exact lattice, inscription, and local church are skins. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEthiopianPriestTurban() => new()
	{
		Metadata = new("earlymodern_religious_ethiopian_priest_turban", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "turban",
		ShortDescription = "a wrapped white cotton priestly turban",
		FullDescription = "A wrapped white cotton priestly turban is cut and sewn from cotton so that the outward silhouette of the turban remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the turban around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260,
		Cost = 56.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "White wrapped clerical headwear; exact winding and office are skins. Used in 3 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousEthiopianWhiteShamma() => new()
	{
		Metadata = new("earlymodern_religious_ethiopian_white_shamma", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shamma",
		ShortDescription = "a broad white cotton priestly shamma",
		FullDescription = "A broad white cotton priestly shamma is cut and sewn from cotton so that the outward silhouette of the shamma remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the shamma its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 780,
		Cost = 96.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "White shoulder and body wrap for Ethiopian clerical and monastic admissions; red borders or local weave are skins. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousHinduPrayerMala() => new()
	{
		Metadata = new("earlymodern_religious_hindu_prayer_mala", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mala",
		ShortDescription = "a loop of polished prayer beads",
		FullDescription = "A loop of polished prayer beads is shaped and bound from wood so that the outward silhouette of the mala remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The contact points and fastenings are kept smooth, while the visible body of the mala carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 85,
		Cost = 48.0m,
		Skinnable = false,
		Material = "wood",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Bead Strings", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Generic Indic devotional bead loop; sect, bead species, count, and use are skins/admissions. Used in 6 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousHinduSacredThread() => new()
	{
		Metadata = new("earlymodern_religious_hindu_sacred_thread", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "thread",
		ShortDescription = "a fine $colour cotton shoulder thread",
		FullDescription = "Several fine strands of $colour cotton form this long, closed loop of thread. They lie alongside one another with a slight twist, gathering into a compact knot where their ends are joined. The loop is long enough to pass over one shoulder and fall diagonally across the torso. Its unadorned strands have a soft, slightly fibrous surface, without beads, a clasp or a separate pendant.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 15,
		Cost = 24.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Bandolier", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousIberianConfraternityRobe() => new()
	{
		Metadata = new("earlymodern_religious_iberian_confraternity_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a full hooded penitential robe",
		FullDescription = "A full hooded penitential robe is cut and sewn from linen so that the outward silhouette of the robe remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 940,
		Cost = 96.0m,
		Skinnable = false,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Processional or confraternity robe; colour, emblem, local devotion, and whether the face is exposed are skins. No identity-obscuring component is implied. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousJainMouthcloth() => new()
	{
		Metadata = new("earlymodern_religious_jain_mouthcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mouthcloth",
		ShortDescription = "a square $colour cotton mouthcloth",
		FullDescription = "This small square of $colour cotton is woven thinly enough for its crossed threads to show against the light. Narrow hems enclose all four edges, with closely spaced stitches keeping the corners flat. The cloth is unlined and carries no stiffening, buckle or padded insert. Its modest size allows it to rest across the mouth without extending over the whole face, and its plain surface has no embroidery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 18,
		Cost = 18.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mask", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousJapaneseRakusu() => new()
	{
		Metadata = new("earlymodern_religious_japanese_rakusu", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "rakusu",
		ShortDescription = "a small patched monastic bib",
		FullDescription = "A small patched monastic bib is cut and sewn from silk so that the outward silhouette of the rakusu remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the rakusu its recognisable proportion when worn. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 220,
		Cost = 180.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Tabard", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Compact Japanese Zen monastic vestment; school, lineage, ring, and colour are skins/admissions. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousJewishArmPhylactery() => new()
	{
		Metadata = new("earlymodern_religious_jewish_arm_phylactery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "phylactery",
		ShortDescription = "a black leather arm phylactery",
		FullDescription = "A black leather arm phylactery is cut and stitched from leather so that the outward silhouette of the phylactery remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The body and openings are proportioned for close, practical wear, with reinforcement placed where the phylactery bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 110,
		Cost = 84.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Bracer", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Wearable prayer object only; exact text and ritual validity are not encoded by the base item. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousJewishGartel() => new()
	{
		Metadata = new("earlymodern_religious_jewish_gartel", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gartel",
		ShortDescription = "a plain woven prayer gartel",
		FullDescription = "A plain woven prayer gartel is cut and sewn from wool so that the outward silhouette of the gartel remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gartel carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 100,
		Cost = 36.0m,
		Skinnable = false,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sash", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Prayer sash or girdle; community, date, and practice require local admission. Used in 3 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousJewishHeadPhylactery() => new()
	{
		Metadata = new("earlymodern_religious_jewish_head_phylactery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "phylactery",
		ShortDescription = "a small black leather head phylactery",
		FullDescription = "A small black leather head phylactery is cut and stitched from leather so that the outward silhouette of the phylactery remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the phylactery carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 95,
		Cost = 84.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Headband", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Wearable prayer object only; exact text and ritual validity are not encoded by the base item. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
