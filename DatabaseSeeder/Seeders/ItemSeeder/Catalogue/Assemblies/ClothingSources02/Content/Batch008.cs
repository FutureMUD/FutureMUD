#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources02Stock
{
	public static CatalogueItemDefinition EarlymodernReligiousJewishKittel() => new()
	{
		Metadata = new("earlymodern_religious_jewish_kittel", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kittel",
		ShortDescription = "a plain white linen kittel",
		FullDescription = "A plain white linen kittel is cut and sewn from linen so that the outward silhouette of the kittel remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the kittel its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 680,
		Cost = 96.0m,
		Skinnable = false,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "White ritual robe for locally admitted festival, penitential, marriage, or funerary contexts; not universal everyday dress. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousJoseonRitualCap() => new()
	{
		Metadata = new("earlymodern_religious_joseon_ritual_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a black folded scholar ritual cap",
		FullDescription = "A black folded scholar ritual cap is cut and sewn from silk so that the outward silhouette of the cap remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 160,
		Cost = 120.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Black Confucian scholar cap admitted with simui; exact bokgeon or related form is a skin. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousJoseonSimui() => new()
	{
		Metadata = new("earlymodern_religious_joseon_simui", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "simui",
		ShortDescription = "a white black-edged ritual scholar robe",
		FullDescription = "A white black-edged ritual scholar robe is cut and sewn from ramie so that the outward silhouette of the simui remains clear. The ramie falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the simui its recognisable proportion when worn. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 920,
		Cost = 220.0m,
		Skinnable = false,
		Material = "ramie",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Joseon Confucian scholar’s ceremonial simui; collar form, black edging, and philosophical symbolism are builder notes. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousLocalBeadworkCollar() => new()
	{
		Metadata = new("earlymodern_religious_local_beadwork_collar", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "collar",
		ShortDescription = "a broad patterned beadwork collar",
		FullDescription = "A broad patterned beadwork collar is cut and sewn from glass so that the outward silhouette of the collar remains clear. The glass falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the collar carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 300,
		Cost = 420.0m,
		Skinnable = false,
		Material = "glass",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Neck Rings", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Neck_Ring"],
		BuilderNotes = "Shared local-ritual overlay; pattern, meaning, community, and office must be locally specified. Used in 4 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousLocalFeatherHeadband() => new()
	{
		Metadata = new("earlymodern_religious_local_feather_headband", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headband",
		ShortDescription = "a feathered woven ritual headband",
		FullDescription = "A feathered woven ritual headband is cut and sewn from feather so that the outward silhouette of the headband remains clear. The feather falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the headband carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 140,
		Cost = 160.0m,
		Skinnable = false,
		Material = "feather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Headband", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Shared local-ritual overlay. Feather species, legality, culture, office, and ceremony must be defined locally. Used in 3 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousLocalFringedMantle() => new()
	{
		Metadata = new("earlymodern_religious_local_fringed_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad fringed ritual mantle",
		FullDescription = "A broad fringed ritual mantle is cut and sewn from wool so that the outward silhouette of the mantle remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 900,
		Cost = 220.0m,
		Skinnable = false,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "Shared local-ritual overlay. Textile, fringe, colour, office, and community must be narrowed before implementation. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousLocalShellNecklace() => new()
	{
		Metadata = new("earlymodern_religious_local_shell_necklace", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "necklace",
		ShortDescription = "a long polished shell ritual necklace",
		FullDescription = "A long polished shell ritual necklace is cut and sewn from shell so that the outward silhouette of the necklace remains clear. The shell falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the necklace carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 220,
		Cost = 160.0m,
		Skinnable = false,
		Material = "shell",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Shared local-ritual overlay; shell species, trade route, meaning, and community must be locally specified. Used in 3 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousLocalWhiteWrapper() => new()
	{
		Metadata = new("earlymodern_religious_local_white_wrapper", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "wrapper",
		ShortDescription = "a full white ritual wrapper",
		FullDescription = "A full white ritual wrapper is cut and sewn from cotton so that the outward silhouette of the wrapper remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the wrapper its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 760,
		Cost = 96.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Shared warm-climate ritual wrapper. Local evidence, institution, gender practice, and ceremony must be specified before use. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousLocalWovenHeadband() => new()
	{
		Metadata = new("earlymodern_religious_local_woven_headband", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headband",
		ShortDescription = "a patterned woven ritual headband",
		FullDescription = "A patterned woven ritual headband is cut and sewn from cotton so that the outward silhouette of the headband remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the headband carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70,
		Cost = 48.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Headband", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Shared local-ritual overlay. A local manifest must define culture, material, pattern, office, and evidence before use. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousMevleviHirka() => new()
	{
		Metadata = new("earlymodern_religious_mevlevi_hirka", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a dark sleeveless dervish cloak",
		FullDescription = "A dark sleeveless dervish cloak is cut and sewn from wool so that the outward silhouette of the cloak remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820,
		Cost = 120.0m,
		Skinnable = false,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "Dark Mevlevi outer cloak removed for the sema; exact lodge colour and trim are skins. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousMevleviSikke() => new()
	{
		Metadata = new("earlymodern_religious_mevlevi_sikke", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a tall brown felt dervish hat",
		FullDescription = "A tall brown felt dervish hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 320,
		Cost = 96.0m,
		Skinnable = false,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Tall felt Mevlevi sikke, grounded in seventeenth-century Ottoman depictions. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousMevleviTennure() => new()
	{
		Metadata = new("earlymodern_religious_mevlevi_tennure", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a full white whirling robe",
		FullDescription = "A full white whirling robe is cut and sewn from cotton so that the outward silhouette of the robe remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050,
		Cost = 120.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Full-skirted Mevlevi ceremonial robe; ritual movement is descriptive and supplies no special mechanic. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousOttomanUlemaFurRobe() => new()
	{
		Metadata = new("earlymodern_religious_ottoman_ulema_fur_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a dark fur-edged ulema over-robe",
		FullDescription = "A dark fur-edged ulema over-robe is cut and sewn from broadcloth so that the outward silhouette of the robe remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 1680,
		Cost = 420.0m,
		Skinnable = false,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "Ottoman learned, judicial, and mosque-office over-robe; fur type, colour, and office are skins and admissions. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousOttomanUlemaTurban() => new()
	{
		Metadata = new("earlymodern_religious_ottoman_ulema_turban", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "turban",
		ShortDescription = "a high layered white ulema turban",
		FullDescription = "A high layered white ulema turban is cut and sewn from cotton so that the outward silhouette of the turban remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the turban around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 420,
		Cost = 160.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Structured Ottoman learned-office turban; exact profile and rank are skins, not universal Muslim dress. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousReformedClericalRuff() => new()
	{
		Metadata = new("earlymodern_religious_reformed_clerical_ruff", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "ruff",
		ShortDescription = "a small pleated clerical ruff",
		FullDescription = "A small pleated clerical ruff is cut and sewn from linen so that the outward silhouette of the ruff remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the ruff carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 95,
		Cost = 120.0m,
		Skinnable = false,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Neck_Ring", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Late sixteenth- through seventeenth-century clerical collar form; date and regional admission required. Used in 3 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousReformedPreachingBands() => new()
	{
		Metadata = new("earlymodern_religious_reformed_preaching_bands", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "bands",
		ShortDescription = "a pair of $colour linen preaching bands",
		FullDescription = "Two flat strips of $colour linen hang side by side from a narrow neckband. Their squared lower ends are neatly hemmed, and the close weave gives each strip a crisp, even surface. The band is slender enough to lie beneath a separate collar, leaving the two wider panels visible at the front. There is no lace or embroidered border, only small stitches following the straight outer edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 35,
		Cost = 48.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Scarf", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousReformedSquareCap() => new()
	{
		Metadata = new("earlymodern_religious_reformed_square_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a soft square preaching cap",
		FullDescription = "A soft square preaching cap is cut and sewn from broadcloth so that the outward silhouette of the cap remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 120,
		Cost = 72.0m,
		Skinnable = false,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Reformed, Lutheran, university, and learned-ministry cap; avoid treating it as universal Protestant dress. Used in 5 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousRyukyuNoroHeadcloth() => new()
	{
		Metadata = new("earlymodern_religious_ryukyu_noro_headcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headcloth",
		ShortDescription = "a broad white ramie ritual headcloth",
		FullDescription = "A broad white ramie ritual headcloth is cut and sewn from ramie so that the outward silhouette of the headcloth remains clear. The ramie falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the headcloth around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 190,
		Cost = 88.0m,
		Skinnable = false,
		Material = "ramie",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Veil", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Noro ceremonial head covering; exact wrapping and island tradition are skins. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousRyukyuNoroNecklace() => new()
	{
		Metadata = new("earlymodern_religious_ryukyu_noro_necklace", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "necklace",
		ShortDescription = "a long necklace of polished ritual beads",
		FullDescription = "A long necklace of polished ritual beads is cut and sewn from glass so that the outward silhouette of the necklace remains clear. The glass falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the necklace carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 280,
		Cost = 920.0m,
		Skinnable = false,
		Material = "glass",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Bead Strings", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Noro ceremonial necklace; exact magatama, heirloom status, stone, and royal office are skins/admissions. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousRyukyuNoroRobe() => new()
	{
		Metadata = new("earlymodern_religious_ryukyu_noro_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a full white ramie ceremonial robe",
		FullDescription = "A full white ramie ceremonial robe is cut and sewn from ramie so that the outward silhouette of the robe remains clear. The ramie falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 760,
		Cost = 180.0m,
		Skinnable = false,
		Material = "ramie",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "White ceremonial robe for locally appointed Ryukyuan noro priestesses; not a universal Okinawan women’s garment. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousSafavidTaj() => new()
	{
		Metadata = new("earlymodern_religious_safavid_taj", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a red twelve-gored Sufi taj",
		FullDescription = "A red twelve-gored Sufi taj is cut and sewn from felt so that the outward silhouette of the cap remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 280,
		Cost = 140.0m,
		Skinnable = false,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Safavid and Persianate Sufi headwear anchor; exact order and court association require local admission. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousSikhChola() => new()
	{
		Metadata = new("earlymodern_religious_sikh_chola", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "chola",
		ShortDescription = "a long $colour cotton chola",
		FullDescription = "This long robe is cut from $colour cotton with a fitted upper body and a fuller skirt below the waist. Its sleeves taper gently toward their cuffs, while overlapping front panels lie flat across the chest. Small cloth ties secure the overlap without a row of projecting buttons. The lower panels spread into loose folds, and narrow hems finish their edges without added braid, embroidery or contrasting facings.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 720,
		Cost = 120.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousSikhKachera() => new()
	{
		Metadata = new("earlymodern_religious_sikh_kachera", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kachera",
		ShortDescription = "a pair of plain $colour cotton kachera",
		FullDescription = "These loose underdrawers are made from $colour cotton with a broad, gathered waist. A cloth drawstring passes through the folded waistband, and the two legs narrow toward their finished lower openings. Extra cloth through the seat forms soft folds rather than a closely fitted seam line. The light fabric has a plain weave, with small reinforced joins at the crotch and no outer pockets or decorative trim.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 180,
		Cost = 56.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shorts", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousSikhKanga() => new()
	{
		Metadata = new("earlymodern_religious_sikh_kanga", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "comb",
		ShortDescription = "a small wooden hair comb",
		FullDescription = "A small wooden hair comb is shaped and bound from wood so that the outward silhouette of the comb remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The contact points and fastenings are kept smooth, while the visible body of the comb carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 25,
		Cost = 36.0m,
		Skinnable = false,
		Material = "wood",
		Tags = ["Era / Early Modern Era", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Hair_Comb"],
		BuilderNotes = "Initiated Sikh article of faith worn with uncut hair; no grooming mechanic is implied. Used in 4 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousSikhKara() => new()
	{
		Metadata = new("earlymodern_religious_sikh_kara", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kara",
		ShortDescription = "a plain iron kara bracelet",
		FullDescription = "A plain iron kara bracelet is cut and sewn from mild steel so that the outward silhouette of the kara remains clear. The mild steel falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the kara bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 95,
		Cost = 96.0m,
		Skinnable = false,
		Material = "mild steel",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Bracelets", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Bracelet"],
		BuilderNotes = "Initiated Sikh article of faith; public wording remains a plain iron bracelet. Used in 4 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousSufiPrayerBeads() => new()
	{
		Metadata = new("earlymodern_religious_sufi_prayer_beads", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "beads",
		ShortDescription = "a long string of polished prayer beads",
		FullDescription = "A long string of polished prayer beads is shaped and bound from wood so that the outward silhouette of the beads remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The contact points and fastenings are kept smooth, while the visible body of the beads carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 90,
		Cost = 48.0m,
		Skinnable = false,
		Material = "wood",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Bead Strings", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Wearable devotional bead string used only where local Sufi or learned practice supports it. Used in 13 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousTheravadaNunRobe() => new()
	{
		Metadata = new("earlymodern_religious_theravada_nun_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a plain white cotton nun robe",
		FullDescription = "A plain white cotton nun robe is cut and sewn from cotton so that the outward silhouette of the robe remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 720,
		Cost = 72.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "White or pale Theravada female renunciant robe; exact institution and region require local admission. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousTheravadaUnderrobe() => new()
	{
		Metadata = new("earlymodern_religious_theravada_underrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a $colour cotton under-robe",
		FullDescription = "This under-robe is made from broad panels of $colour cotton joined by flat seams. Its generous expanse of cloth gathers into shallow folds, with an even hem running around the lower edge. The plain weave is light and closely worked, without a separate lining or padded border. Repeated stitching strengthens the panel joins, leaving the large outer surfaces undecorated and the edges flexible enough to fold closely.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 620,
		Cost = 72.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousTheravadaUpperrobe() => new()
	{
		Metadata = new("earlymodern_religious_theravada_upperrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad $colour cotton mantle",
		FullDescription = "A broad expanse of $colour cotton forms this soft, unlined mantle. Flat seams divide the cloth into large panels, and a narrow doubled hem encloses the outer edges. It has no fitted sleeves or shaped collar; its outline comes from the folds of the cloth itself. The weave remains visible across the undecorated surface, with small, even stitches reinforcing the places where the panels meet.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 780,
		Cost = 88.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousTibetanMantle() => new()
	{
		Metadata = new("earlymodern_religious_tibetan_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad $colour wool mantle",
		FullDescription = "This broad mantle is made from dense $colour wool with a soft, closely napped face. Long seams join its panels into an ample rectangle, and doubled hems run along the outer edges. It has neither sleeves nor a fitted neck opening, allowing the cloth to gather into heavy, rounded folds. Small stitches secure the panel joins and corners, with no fringe, clasp or embroidered border to interrupt the plain outline.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 940,
		Cost = 120.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousTibetanShamtab() => new()
	{
		Metadata = new("earlymodern_religious_tibetan_shamtab", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shamtab",
		ShortDescription = "a pleated $colour wool shamtab",
		FullDescription = "This long skirt is formed from substantial $colour wool arranged in broad vertical pleats. The upper edge is folded into a firm band, while the pleats open out below it into a wide, straight fall. A deep turned hem adds a little weight along the lower edge. The cloth has a close, softly napped surface, and the visible stitching is confined to the waistband, panel joins and hem.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 720,
		Cost = 96.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousTibetanVest() => new()
	{
		Metadata = new("earlymodern_religious_tibetan_vest", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "vest",
		ShortDescription = "a sleeveless $colour wool vest",
		FullDescription = "This sleeveless vest is cut from close-woven $colour wool, with broad front panels and a short, straight lower edge. Its arm openings are generously cut and bound with narrow strips of the same cloth. Flat seams join the shoulders and sides, leaving little raised bulk against the body. The softly napped outer surface is plain, without pockets, metal fasteners or a separate ornamental collar.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 520,
		Cost = 88.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousTibetanYellowHat() => new()
	{
		Metadata = new("earlymodern_religious_tibetan_yellow_hat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a tall folded yellow monastic hat",
		FullDescription = "A tall folded yellow monastic hat is cut and sewn from wool so that the outward silhouette of the hat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 380,
		Cost = 180.0m,
		Skinnable = false,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "Ceremonial yellow monastic hat; use is sect-, rank-, and occasion-gated. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternBiretta() => new()
	{
		Metadata = new("earlymodern_religious_western_biretta", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "biretta",
		ShortDescription = "a stiff square clerical cap",
		FullDescription = "A stiff square clerical cap is cut and sewn from broadcloth so that the outward silhouette of the biretta remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the biretta around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140,
		Cost = 96.0m,
		Skinnable = false,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Clerical and academic square cap; exact colour, piping, office, and regional form are skins. Used in 5 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternClericalSkullcap() => new()
	{
		Metadata = new("earlymodern_religious_western_clerical_skullcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "skullcap",
		ShortDescription = "a close silk clerical skullcap",
		FullDescription = "A close silk clerical skullcap is cut and sewn from silk so that the outward silhouette of the skullcap remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the skullcap carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 55,
		Cost = 100.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Skullcap", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Close clerical skullcap; colour and the rank or institution it signals are skins. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
