#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources02Stock
{
	public static CatalogueItemDefinition EarlymodernReligiousWesternEpiscopalRing() => new()
	{
		Metadata = new("earlymodern_religious_western_episcopal_ring", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "ring",
		ShortDescription = "a heavy gold episcopal ring",
		FullDescription = "A heavy gold episcopal ring is worked and joined from gold so that the outward silhouette of the ring remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the ring carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Great,
		WeightInGrams = 28,
		Cost = 920.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Rings", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Ring"],
		BuilderNotes = "Functional office is builder-facing only; exact stone, arms, inscription, and hierarchy are skins. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternLaceCotta() => new()
	{
		Metadata = new("earlymodern_religious_western_lace_cotta", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cotta",
		ShortDescription = "a short lace-edged white cotta",
		FullDescription = "A short lace-edged white cotta is cut and sewn from linen so that the outward silhouette of the cotta remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cotta its recognisable proportion when worn. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 360,
		Cost = 160.0m,
		Skinnable = false,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Tabard", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Shorter and more elaborately edged choir/service over-vestment than the shared plain surplice; admit only where local liturgical use supports it. Used in 3 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternMozzetta() => new()
	{
		Metadata = new("earlymodern_religious_western_mozzetta", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mozzetta",
		ShortDescription = "a short buttoned clerical shoulder cape",
		FullDescription = "A short buttoned clerical shoulder cape is cut and sewn from broadcloth so that the outward silhouette of the mozzetta remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the mozzetta carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 420,
		Cost = 220.0m,
		Skinnable = false,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoulders", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Short shoulder cape for senior Latin clerical and choir dress; exact rank colour and piping are skins. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternNarrowChasuble() => new()
	{
		Metadata = new("earlymodern_religious_western_narrow_chasuble", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "chasuble",
		ShortDescription = "a narrow embroidered silk chasuble",
		FullDescription = "A narrow embroidered silk chasuble is cut and sewn from silk so that the outward silhouette of the chasuble remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the chasuble carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 760,
		Cost = 720.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Poncho", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Early Modern narrowed liturgical cut; exact colour, season, embroidery, and institution are skins. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternPectoralCross() => new()
	{
		Metadata = new("earlymodern_religious_western_pectoral_cross", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cross",
		ShortDescription = "a plain silver pectoral cross",
		FullDescription = "A plain silver pectoral cross is worked and joined from silver so that the outward silhouette of the cross remains clear. The silver has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the cross carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 110,
		Cost = 420.0m,
		Skinnable = false,
		Material = "silver",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Generic senior-clergy or mission pectoral cross; exact cross form, office, and devotional design are skins. Used in 3 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternRosaryBeads() => new()
	{
		Metadata = new("earlymodern_religious_western_rosary_beads", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "beads",
		ShortDescription = "a long string of dark prayer beads",
		FullDescription = "A long string of dark prayer beads is shaped and bound from wood so that the outward silhouette of the beads remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The contact points and fastenings are kept smooth, while the visible body of the beads carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 95,
		Cost = 52.0m,
		Skinnable = false,
		Material = "wood",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Bead Strings", "Market / Religious Goods / Devotional Goods"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "Wearable devotional bead string; exact count, medal, cross, and local devotion are skins. Used in 11 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternShortClericalCape() => new()
	{
		Metadata = new("earlymodern_religious_western_short_clerical_cape", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cape",
		ShortDescription = "a short black clerical cape",
		FullDescription = "A short black clerical cape is cut and sewn from broadcloth so that the outward silhouette of the cape remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cape its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 620,
		Cost = 160.0m,
		Skinnable = false,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cape", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Short clerical outer cape suitable for secular clergy, missionaries, schools, and urban ministry; exact order or institution is a skin. Used in 4 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternTeachingSisterCoif() => new()
	{
		Metadata = new("earlymodern_religious_western_teaching_sister_coif", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coif",
		ShortDescription = "a broad white linen sister coif",
		FullDescription = "A broad white linen sister coif is cut and sewn from linen so that the outward silhouette of the coif remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the coif around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150,
		Cost = 52.0m,
		Skinnable = false,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Coif", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Broad practical coif for teaching, nursing, charitable, or enclosed communities; exact congregation is a skin. Used in 2 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousWesternTippet() => new()
	{
		Metadata = new("earlymodern_religious_western_tippet", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tippet",
		ShortDescription = "a long black clerical tippet",
		FullDescription = "A long black clerical tippet is cut and sewn from broadcloth so that the outward silhouette of the tippet remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the tippet carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 220,
		Cost = 96.0m,
		Skinnable = false,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Scarf", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Choir, preaching, academic, and clerical shoulder scarf; exact institution and office remain skins. Used in 3 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousZoroastrianKusti() => new()
	{
		Metadata = new("earlymodern_religious_zoroastrian_kusti", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kusti",
		ShortDescription = "a $colour wool kusti",
		FullDescription = "Fine strands of $colour wool are closely worked into this long, narrow girdle. The cord has a supple, slightly textured surface and an even thickness through most of its length. Its ends divide into small tasselled bundles, with the strands gathered tightly before they spread loose. There is no buckle or metal fitting; the light cord is long enough to wind around the waist and secure by tying.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 65,
		Cost = 32.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Sash", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousZoroastrianPadan() => new()
	{
		Metadata = new("earlymodern_religious_zoroastrian_padan", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mouthcloth",
		ShortDescription = "a white cotton priestly mouth veil",
		FullDescription = "A white cotton priestly mouth veil is cut and sewn from cotton so that the outward silhouette of the mouthcloth remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the mouthcloth around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 22,
		Cost = 24.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mask", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Priestly mouth veil used before sacred fire or during ritual. It has no gag, identity-obscuring, or breathing mechanic. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousZoroastrianPrayerCap() => new()
	{
		Metadata = new("earlymodern_religious_zoroastrian_prayer_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a plain $colour cotton prayer cap",
		FullDescription = "This close-fitting cap is sewn from light $colour cotton into a low, rounded crown. Narrow seams run upward from a neatly bound lower edge, shaping the cloth without a projecting brim. A thin lining follows the same outline inside, covering the raw edges of the joins. The outer surface is plain and softly flexible, with no badge, tassel or embroidered band around the brow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 70,
		Cost = 40.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Skullcap", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousZoroastrianPriestRobe() => new()
	{
		Metadata = new("earlymodern_religious_zoroastrian_priest_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a long white cotton priestly robe",
		FullDescription = "A long white cotton priestly robe is cut and sewn from cotton so that the outward silhouette of the robe remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 820,
		Cost = 180.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "White ritual robe for an admitted Zoroastrian priestly context; exact jama, sleeve, fastening, grade, and community are skins. Used in 1 fourth-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernReligiousZoroastrianSudreh() => new()
	{
		Metadata = new("earlymodern_religious_zoroastrian_sudreh", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sudreh",
		ShortDescription = "a plain $colour cotton sudreh",
		FullDescription = "This lightweight shirt is made from fine $colour cotton with a straight body and short sleeves. Its neckline opens at the front, where a small folded section of cloth is secured beneath the opening. Narrow seams join the panels, and a simple turned hem finishes the lower edge. The close weave is unlined and undecorated, with no attached sash, outer buttons or contrasting embroidery.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 170,
		Cost = 32.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing", "Market / Religious Goods / Ritual Supplies"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shirt", "Armour_LightClothing", "Insulation_Minor", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianClothingCheckedWoolWrapSkirt() => new()
	{
		Metadata = new("earlymodern_russian_clothing_checked_wool_wrap_skirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "skirt",
		ShortDescription = "a $colour1 and $colour2 checked wool wrap skirt",
		FullDescription = "A $colour1 and $colour2 checked wool wrap skirt is cut and sewn from wool so that the outward silhouette of the skirt remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the skirt, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 770,
		Cost = 28.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Moderate", "Variable_2BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 2 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianClothingHighLeatherBoots() => new()
	{
		Metadata = new("earlymodern_russian_clothing_high_leather_boots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of high leather boots",
		FullDescription = "A pair of high leather boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1220,
		Cost = 72.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_High_Boots", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "New Early Modern prototype; used in 3 outfit(s). Second-pass military admission; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianClothingLinenWorkApron() => new()
	{
		Metadata = new("earlymodern_russian_clothing_linen_work_apron", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "apron",
		ShortDescription = "a $colour linen work apron",
		FullDescription = "A $colour linen work apron is cut and sewn from linen so that the outward silhouette of the apron remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the apron carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 210,
		Cost = 10.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Apron", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 4 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianClothingLongLinenHeadVeil() => new()
	{
		Metadata = new("earlymodern_russian_clothing_long_linen_head_veil", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "veil",
		ShortDescription = "a long $colour linen head veil",
		FullDescription = "A long $colour linen head veil is cut and sewn from linen so that the outward silhouette of the veil remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the veil around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120,
		Cost = 10.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Head_Veil", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 5 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianClothingLongLinenRubakha() => new()
	{
		Metadata = new("earlymodern_russian_clothing_long_linen_rubakha", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shirt",
		ShortDescription = "a long $colour linen shirt",
		FullDescription = "A long $colour linen shirt is cut and sewn from linen so that the outward silhouette of the shirt remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the shirt bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480,
		Cost = 22.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long-Sleeved_Tunic", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 2 outfit(s). Second-pass military admission; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianClothingNarrowWoolTrousers() => new()
	{
		Metadata = new("earlymodern_russian_clothing_narrow_wool_trousers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "trousers",
		ShortDescription = "a pair of narrow $colour wool trousers",
		FullDescription = "A pair of narrow $colour wool trousers is cut and sewn from wool so that the outward silhouette of the trousers remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the trousers, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620,
		Cost = 22.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 4 outfit(s). Second-pass military admission; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianClothingRoundFurCap() => new()
	{
		Metadata = new("earlymodern_russian_clothing_round_fur_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a round fur cap",
		FullDescription = "A round fur cap is cut and stitched from fur so that the outward silhouette of the cap remains clear. The fur shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220,
		Cost = 28.0m,
		Skinnable = false,
		Material = "fur",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_HeavyClothing", "Insulation_Strong"],
		BuilderNotes = "New Early Modern prototype; used in 1 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianClothingShortWoolZipun() => new()
	{
		Metadata = new("earlymodern_russian_clothing_short_wool_zipun", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a close $colour short wool coat",
		FullDescription = "A close $colour short wool coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720,
		Cost = 33.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 2 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianMilitaryStreltsyServiceCoat() => new()
	{
		Metadata = new("earlymodern_russian_military_streltsy_service_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a long $colour guard service coat",
		FullDescription = "A long $colour guard service coat is cut and sewn from broadcloth so that the outward silhouette of the coat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500,
		Cost = 110.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_HeavyClothing", "Insulation_Strong", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianMilitaryTallGuardCap() => new()
	{
		Metadata = new("earlymodern_russian_military_tall_guard_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a tall $colour wool guard cap",
		FullDescription = "A tall $colour wool guard cap is cut and sewn from wool so that the outward silhouette of the cap remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 340,
		Cost = 55.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_russian_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a tall fur-brimmed $colour court cap",
		FullDescription = "A tall fur-brimmed $colour court cap is cut and stitched from fur so that the outward silhouette of the cap remains clear. The fur shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 248,
		Cost = 808.0m,
		Skinnable = true,
		Material = "fur",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Russian / Petrine and post-Petrine; headwear. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_russian_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a sleeveless $colour silk court overgown",
		FullDescription = "A sleeveless $colour silk court overgown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1183,
		Cost = 1520.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Russian / Petrine and post-Petrine; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_russian_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a long $colour silk state gown",
		FullDescription = "A long $colour silk state gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1183,
		Cost = 3270.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Russian / Petrine and post-Petrine; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_russian_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a deep-furred $colour velvet travelling cloak",
		FullDescription = "A deep-furred $colour velvet travelling cloak is cut and sewn from velvet so that the outward silhouette of the cloak remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1322,
		Cost = 1280.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Russian / Petrine and post-Petrine; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianNobleJewellery() => new()
	{
		Metadata = new("earlymodern_russian_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "chain",
		ShortDescription = "a heavy gem-set gold neck chain",
		FullDescription = "A heavy gem-set gold neck chain is worked and joined from gold so that the outward silhouette of the chain remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the chain carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 265,
		Cost = 2605.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Neck Rings", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Neck_Ring"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for Russian / Petrine and post-Petrine. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_russian_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "caftan",
		ShortDescription = "a full-skirted $colour silk court caftan",
		FullDescription = "A full-skirted $colour silk court caftan is cut and sewn from silk so that the outward silhouette of the caftan remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the caftan its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 913,
		Cost = 1252.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Russian / Petrine and post-Petrine; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_russian_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a gem-guarded $colour velvet state mantle",
		FullDescription = "A gem-guarded $colour velvet state mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1113,
		Cost = 2915.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Russian / Petrine and post-Petrine; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianNobleMaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_russian_noble_male_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "caftan",
		ShortDescription = "a deep-furred $colour wool travelling caftan",
		FullDescription = "A deep-furred $colour wool travelling caftan is cut and sewn from wool so that the outward silhouette of the caftan remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the caftan its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1322,
		Cost = 1280.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Russian / Petrine and post-Petrine; male travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRussianNobleRegalia() => new()
	{
		Metadata = new("earlymodern_russian_noble_regalia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "crown",
		ShortDescription = "a high gem-set gold crown",
		FullDescription = "A high gem-set gold crown is worked and joined from gold so that the outward silhouette of the crown remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the crown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 265,
		Cost = 4005.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Crowns", "Market / Jewellery / Regalia"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Crown"],
		BuilderNotes = "New culture-family highest-echelon regalia prototype for Russian / Petrine and post-Petrine. Exact rank, dynasty, office, and motif are skins. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRyukyuClothingBroadWovenSash() => new()
	{
		Metadata = new("earlymodern_ryukyu_clothing_broad_woven_sash", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sash",
		ShortDescription = "a broad $colour woven sash",
		FullDescription = "A broad $colour woven sash is cut and sewn from cotton so that the outward silhouette of the sash remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the sash carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140,
		Cost = 8.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sash", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour", "Belt_6"],
		BuilderNotes = "New Early Modern prototype; used in 6 outfit(s). Second-pass military admission; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernRyukyuClothingTiedHeadcloth() => new()
	{
		Metadata = new("earlymodern_ryukyu_clothing_tied_headcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headcloth",
		ShortDescription = "a tied $colour cotton headcloth",
		FullDescription = "A tied $colour cotton headcloth is cut and sewn from cotton so that the outward silhouette of the headcloth remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the headcloth carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80,
		Cost = 6.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Kerchief", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 7 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
