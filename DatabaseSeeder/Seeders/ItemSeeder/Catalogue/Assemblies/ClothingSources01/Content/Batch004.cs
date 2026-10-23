#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources01Stock
{
	public static CatalogueItemDefinition RenaissanceInstitutionLiturgicalAlb() => new()
	{
		Metadata = new("renaissance_institution_liturgical_alb", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a long $colour linen alb",
		FullDescription = "This long alb is made from $colour linen with a plain neckline and close, full-length sleeves. The body widens gently below the shoulders into loose vertical folds. Narrow seams lie flat against the unlined cloth, and a simply turned hem finishes the lower edge. Its smooth surface has no embroidered panels, lace inserts or attached girdle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 33.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe_Layer_0_5_NonBulky", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceInstitutionMonasticHabit() => new()
	{
		Metadata = new("renaissance_institution_monastic_habit", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "habit",
		ShortDescription = "a $colour long belted monastic habit",
		FullDescription = "A $colour long belted monastic habit is cut and sewn from wool so that the outward silhouette of the habit remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the habit its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 42.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: order colour and cut mostly skins; local institution required",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceInstitutionMonasticScapular() => new()
	{
		Metadata = new("renaissance_institution_monastic_scapular", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "scapular",
		ShortDescription = "a long $colour wool scapular",
		FullDescription = "Two long panels of $colour wool hang from the joined shoulders of this scapular. A simple opening passes over the head, leaving the cloth to fall down the front and back with the sides open. The panels are broad and straight, their edges secured by narrow turned hems. There are no sleeves, pockets or attached hood, and the plain, softly napped surface has no badge or contrasting trim.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 700,
		Cost = 55.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Religious Clothing", "Institution / Religious"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Tabard", "Armour_LightClothing", "Insulation_Moderate", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceInstitutionPhysiciansRoundcap() => new()
	{
		Metadata = new("renaissance_institution_physicians_roundcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour full round physician cap",
		FullDescription = "A $colour full round physician cap is cut and sewn from velvet so that the outward silhouette of the cap remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 50.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Physician, senior scholar, jurist, and civic learned-office dress where a round cap is locally supported.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceInstitutionPlainCassock() => new()
	{
		Metadata = new("renaissance_institution_plain_cassock", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cassock",
		ShortDescription = "a long $colour close-buttoned cassock",
		FullDescription = "This long cassock is cut from $colour broadcloth with a close-fitting upper body and a fuller lower skirt. A line of small covered buttons follows the front opening, rising toward a modest standing collar. The sleeves taper gently at the wrists, and the closely worked seams keep the shoulders smooth. Its dense outer surface has an even finish, with no attached sash, embroidered insignia or contrasting piping.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050,
		Cost = 100.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Religious Clothing", "Institution / Religious"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe_Layer_0_75_NonBulky", "Armour_LightClothing", "Insulation_Moderate", "Variable_Colour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceInstitutionPreachingGown() => new()
	{
		Metadata = new("renaissance_institution_preaching_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a full-sleeved preaching gown",
		FullDescription = "A full-sleeved preaching gown is cut and sewn from broadcloth so that the outward silhouette of the gown remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the gown its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200,
		Cost = 120.0m,
		Skinnable = false,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Religious Clothing", "Institution / Religious"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Moderate"],
		BuilderNotes = "Renaissance institutional admission required by documented Early Modern religious outfits.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceInstitutionProcessionalCope() => new()
	{
		Metadata = new("renaissance_institution_processional_cope", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cope",
		ShortDescription = "a $colour long clasped ceremonial cope",
		FullDescription = "A $colour long clasped ceremonial cope is cut and sewn from silk so that the outward silhouette of the cope remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cope its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050,
		Cost = 112.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: hooded shield and embroidery are skins; procession/liturgy gate",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseBroadSedgeTravelhat() => new()
	{
		Metadata = new("renaissance_japanese_broad_sedge_travelhat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a $colour broad sedge travel hat",
		FullDescription = "A $colour broad sedge travel hat is shaped and bound from straw so that the outward silhouette of the hat remains clear. The straw keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 6.0m,
		Skinnable = true,
		Material = "straw",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Traveller, pilgrim, courier, rural, river, and foot-soldier contexts; ties and crown depth are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseBroadsleeveFormalrobe() => new()
	{
		Metadata = new("renaissance_japanese_broadsleeve_formalrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a $colour broad-sleeved formal robe",
		FullDescription = "A $colour broad-sleeved formal robe is cut and sewn from silk so that the outward silhouette of the robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 980,
		Cost = 120.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: court, shrine/temple, warrior ceremony, performance gate",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseDividedRidingHakama() => new()
	{
		Metadata = new("renaissance_japanese_divided_riding_hakama", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "trousers",
		ShortDescription = "a $colour divided pleated riding trousers",
		FullDescription = "A $colour divided pleated riding trousers is cut and sewn from silk so that the outward silhouette of the trousers remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the trousers, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 620,
		Cost = 72.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: warrior, courier, equestrian, formal skins; distinct divided construction",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseFieldTrousers() => new()
	{
		Metadata = new("renaissance_japanese_field_trousers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "trousers",
		ShortDescription = "a pair of close $colour ramie field trousers",
		FullDescription = "A pair of close $colour ramie field trousers is cut and sewn from ramie cloth so that the outward silhouette of the trousers remains clear. The ramie cloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the trousers, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450,
		Cost = 24.0m,
		Skinnable = true,
		Material = "ramie cloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 3 outfit(s). Second-pass military admission; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseFullPleatedHakama() => new()
	{
		Metadata = new("renaissance_japanese_full_pleated_hakama", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hakama",
		ShortDescription = "a full pleated $colour lower garment",
		FullDescription = "A full pleated $colour lower garment is cut and sewn from silk so that the outward silhouette of the hakama remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the hakama, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 360,
		Cost = 134.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 3 outfit(s). Second-pass military admission; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseHeavyOuterrobe() => new()
	{
		Metadata = new("renaissance_japanese_heavy_outerrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a $colour heavy lined outer robe",
		FullDescription = "A $colour heavy lined outer robe is cut and sewn from silk so that the outward silhouette of the robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1220,
		Cost = 144.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: court, warrior household, formal, theatrical skins; uchikake-like anchor",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseLacqueredConicalhat() => new()
	{
		Metadata = new("renaissance_japanese_lacquered_conicalhat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a lacquered wooden conical hat",
		FullDescription = "A lacquered wooden conical hat is shaped and bound from wood so that the outward silhouette of the hat remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230,
		Cost = 8.0m,
		Skinnable = false,
		Material = "wood",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseQuiltedSleevelessvest() => new()
	{
		Metadata = new("renaissance_japanese_quilted_sleevelessvest", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "vest",
		ShortDescription = "a $colour quilted sleeveless vest",
		FullDescription = "A $colour quilted sleeveless vest is cut and sewn from cotton so that the outward silhouette of the vest remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the vest bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 21.6m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: winter, worker, warrior-underlayer, household skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseShortOpenjacket() => new()
	{
		Metadata = new("renaissance_japanese_short_openjacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a short open-front $colour jacket",
		FullDescription = "A short open-front $colour jacket is cut and sewn from silk so that the outward silhouette of the jacket remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jacket bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 420,
		Cost = 202.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 6 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseShoulderwingVest() => new()
	{
		Metadata = new("renaissance_japanese_shoulderwing_vest", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "vest",
		ShortDescription = "a $colour stiff shoulder-wing vest",
		FullDescription = "A $colour stiff shoulder-wing vest is cut and sewn from broadcloth so that the outward silhouette of the vest remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the vest bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: kataginu-like anchor; warrior, official, pageant, formal household admission",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseSmallsleeveWraprobe() => new()
	{
		Metadata = new("renaissance_japanese_smallsleeve_wraprobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a small-sleeved $colour wrap robe",
		FullDescription = "Broad panels of $colour silk form this small-sleeved wrap robe. A continuous collar follows the overlapping front edges, and shallow sleeve openings interrupt the straight outer outline. The silk falls in smooth folds beneath a softly lustrous surface. Fine seams and plain hems finish the unpadded garment, with no permanently attached sash or accompanying jacket.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 580,
		Cost = 255.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe_Layer_0_75_NonBulky", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseSoftFoldedCourtcap() => new()
	{
		Metadata = new("renaissance_japanese_soft_folded_courtcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour tall folded soft cap",
		FullDescription = "A $colour tall folded soft cap is cut and sewn from silk so that the outward silhouette of the cap remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 40.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: eboshi-like anchor; court, warrior, shrine, formal-status skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseSplittoeSocks() => new()
	{
		Metadata = new("renaissance_japanese_splittoe_socks", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "socks",
		ShortDescription = "a pair of $colour split-toe socks",
		FullDescription = "A pair of $colour split-toe socks is cut and sewn from cotton so that the outward silhouette of the socks remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the socks carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 100,
		Cost = 6.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Socks", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 10 outfit(s). Second-pass military admission; used in 6 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseStiffLacqueredEboshi() => new()
	{
		Metadata = new("renaissance_japanese_stiff_lacquered_eboshi", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour stiff lacquered folded cap",
		FullDescription = "A $colour stiff lacquered folded cap is cut and sewn from silk so that the outward silhouette of the cap remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 40.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Warrior, court, shrine, official, and formal household gate; distinct rigid treatment from the soft court cap.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseStrawRaincape() => new()
	{
		Metadata = new("renaissance_japanese_straw_raincape", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cape",
		ShortDescription = "a $colour layered straw rain cape",
		FullDescription = "A $colour layered straw rain cape is shaped and bound from straw so that the outward silhouette of the cape remains clear. The straw keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cape its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1050,
		Cost = 16.8m,
		Skinnable = true,
		Material = "straw",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: exact straw audit; farmer, traveller, courier, foot-soldier admissions",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseTiedStrawTravelsandals() => new()
	{
		Metadata = new("renaissance_japanese_tied_straw_travelsandals", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "sandals",
		ShortDescription = "a pair of $colour tightly braided straw road sandals",
		FullDescription = "A pair of $colour tightly braided straw road sandals is shaped and bound from straw so that the outward silhouette of the sandals remains clear. The straw keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The sandals is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 7.2m,
		Skinnable = true,
		Material = "straw",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sandals", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Road, pilgrimage, courier, field, and foot-soldier use; dense braid and long securing cords distinguish this form from generic woven sandals.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseUnlinedSummerrobe() => new()
	{
		Metadata = new("renaissance_japanese_unlined_summerrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "an unlined $colour ramie summer robe",
		FullDescription = "An unlined $colour ramie summer robe is cut and sewn from ramie cloth so that the outward silhouette of the robe remains clear. The ramie cloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720,
		Cost = 46.0m,
		Skinnable = true,
		Material = "ramie cloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseWoodenClogs() => new()
	{
		Metadata = new("renaissance_japanese_wooden_clogs", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "clogs",
		ShortDescription = "a pair of raised $colour wooden clogs",
		FullDescription = "Two transverse supports raise each of these $colour wooden clogs above the ground. The flat upper boards have rounded corners and smooth footbeds, with long grain lines running from heel to toe. Soft cloth thongs emerge through the boards and divide around a short loop between the toes. The wood is evenly worked along the edges, and its visible grain continues down the sides of the raised supports.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700,
		Cost = 15.0m,
		Skinnable = true,
		Material = "wood",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_WoodColour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJapaneseWovenStrawcoat() => new()
	{
		Metadata = new("renaissance_japanese_woven_strawcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a $colour long woven straw coat",
		FullDescription = "A $colour long woven straw coat is shaped and bound from straw so that the outward silhouette of the coat remains clear. The straw keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1220,
		Cost = 21.6m,
		Skinnable = true,
		Material = "straw",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: exact straw audit; rain/snow travel presentation, no waterproof mechanic claim",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJoseonBroadsleeveScholaroverrobe() => new()
	{
		Metadata = new("renaissance_joseon_broadsleeve_scholaroverrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "over-robe",
		ShortDescription = "a broad-sleeved $colour scholar over-robe",
		FullDescription = "A broad-sleeved $colour scholar over-robe is cut and sewn from silk so that the outward silhouette of the over-robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the over-robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 730,
		Cost = 370.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 1 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJoseonFullGatheredWrapskirt() => new()
	{
		Metadata = new("renaissance_joseon_full_gathered_wrapskirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "skirt",
		ShortDescription = "a full gathered $colour wrap skirt",
		FullDescription = "A full gathered $colour wrap skirt is cut and sewn from silk so that the outward silhouette of the skirt remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the skirt, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 460,
		Cost = 168.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 3 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJoseonLongCrossfrontJacket() => new()
	{
		Metadata = new("renaissance_joseon_long_crossfront_jacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a long cross-front $colour silk jacket",
		FullDescription = "A long cross-front $colour silk jacket is cut and sewn from silk so that the outward silhouette of the jacket remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jacket bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 420,
		Cost = 202.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 3 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJoseonLongCrossfrontUnderrobe() => new()
	{
		Metadata = new("renaissance_joseon_long_crossfront_underrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "under-robe",
		ShortDescription = "a long cross-front $colour ramie under-robe",
		FullDescription = "A long cross-front $colour ramie under-robe is cut and sewn from ramie cloth so that the outward silhouette of the under-robe remains clear. The ramie cloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the under-robe bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780,
		Cost = 55.0m,
		Skinnable = true,
		Material = "ramie cloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long-Sleeved_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 10 outfit(s). Mapped to a dedicated under-gown layer beneath jackets and over-robes. Second-pass military admission; used in 6 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJoseonNarrowbrimHorsehairhat() => new()
	{
		Metadata = new("renaissance_joseon_narrowbrim_horsehairhat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a $colour narrow-brim horsehair scholar hat",
		FullDescription = "A $colour narrow-brim horsehair scholar hat is shaped and bound from horsehair so that the outward silhouette of the hat remains clear. The horsehair keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 8.0m,
		Skinnable = true,
		Material = "horsehair",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Less formal and less expansive than the tall broad-brim official form; scholar, town, and travel gate.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJoseonPaddedWinterhood() => new()
	{
		Metadata = new("renaissance_joseon_padded_winterhood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a $colour padded winter head hood",
		FullDescription = "A $colour padded winter head hood is cut and sewn from cotton so that the outward silhouette of the hood remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300,
		Cost = 14.4m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hood", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Women, children, household, travel, and cold-season town dress; exact shape and status locally gated.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJoseonPleatedOfficialovercoat() => new()
	{
		Metadata = new("renaissance_joseon_pleated_officialovercoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a $colour pleated long official coat",
		FullDescription = "A $colour pleated long official coat is cut and sewn from silk so that the outward silhouette of the coat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1220,
		Cost = 144.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: official/rank/institution gate; colour and insignia are skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJoseonRoundedOfficialcap() => new()
	{
		Metadata = new("renaissance_joseon_rounded_officialcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour rounded formal official cap",
		FullDescription = "A $colour rounded formal official cap is cut and sewn from silk so that the outward silhouette of the cap remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 40.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: court/official/ritual gate; winged variants require a new silhouette",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceJoseonShortCrossfrontJacket() => new()
	{
		Metadata = new("renaissance_joseon_short_crossfront_jacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a short cross-front $colour ramie jacket",
		FullDescription = "A short cross-front $colour ramie jacket is cut and sewn from ramie cloth so that the outward silhouette of the jacket remains clear. The ramie cloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jacket bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520,
		Cost = 36.0m,
		Skinnable = true,
		Material = "ramie cloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 6 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
