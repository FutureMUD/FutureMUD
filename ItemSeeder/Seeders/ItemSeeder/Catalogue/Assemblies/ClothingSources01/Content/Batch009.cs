#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources01Stock
{
	public static CatalogueItemDefinition RenaissanceSeasiaFittedDancebodice() => new()
	{
		Metadata = new("renaissance_seasia_fitted_dancebodice", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "bodice",
		ShortDescription = "a $colour fitted ceremonial dance bodice",
		FullDescription = "A $colour fitted ceremonial dance bodice is cut and sewn from silk so that the outward silhouette of the bodice remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the bodice bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 560,
		Cost = 72.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: performance/temple/court gate; bead and metallic decoration are skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaFullCeremonialskirt() => new()
	{
		Metadata = new("renaissance_seasia_full_ceremonialskirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "skirt",
		ShortDescription = "a $colour full panelled ceremonial skirt",
		FullDescription = "A $colour full panelled ceremonial skirt is cut and sewn from silk so that the outward silhouette of the skirt remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the skirt, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 680,
		Cost = 72.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: court, temple, performance, bridal skins; train only as description",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaLongCrossfrontCourtrobe() => new()
	{
		Metadata = new("renaissance_seasia_long_crossfront_courtrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a long cross-front $colour silk robe",
		FullDescription = "A long cross-front $colour silk robe is cut and sewn from silk so that the outward silhouette of the robe remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 580,
		Cost = 255.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 5 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaPalmleafConicalhat() => new()
	{
		Metadata = new("renaissance_seasia_palmleaf_conicalhat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a broad conical fibre hat",
		FullDescription = "A broad conical fibre hat is shaped and bound from barkcloth so that the outward silhouette of the hat remains clear. The barkcloth keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140,
		Cost = 10.0m,
		Skinnable = false,
		Material = "barkcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 5 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaPleatedCourtwaistcloth() => new()
	{
		Metadata = new("renaissance_seasia_pleated_courtwaistcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "waistcloth",
		ShortDescription = "a pleated $colour silk waistcloth",
		FullDescription = "A pleated $colour silk waistcloth is cut and sewn from silk so that the outward silhouette of the waistcloth remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the waistcloth, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 460,
		Cost = 168.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaSewnTubeskirt() => new()
	{
		Metadata = new("renaissance_seasia_sewn_tubeskirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "skirt",
		ShortDescription = "a sewn $colour cotton tubular skirt",
		FullDescription = "A sewn $colour cotton tubular skirt is cut and sewn from cotton so that the outward silhouette of the skirt remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the skirt, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 630,
		Cost = 26.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 6 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaShortCollarlessJacket() => new()
	{
		Metadata = new("renaissance_seasia_short_collarless_jacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a short collarless $colour cotton jacket",
		FullDescription = "A short collarless $colour cotton jacket is cut and sewn from cotton so that the outward silhouette of the jacket remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jacket bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 580,
		Cost = 32.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 9 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaShortMaritimeTrousers() => new()
	{
		Metadata = new("renaissance_seasia_short_maritime_trousers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "trousers",
		ShortDescription = "a pair of short full $colour cotton trousers",
		FullDescription = "A pair of short full $colour cotton trousers is cut and sewn from cotton so that the outward silhouette of the trousers remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the trousers, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500,
		Cost = 21.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 7 outfit(s). Second-pass military admission; used in 14 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaSleevelessCourtvest() => new()
	{
		Metadata = new("renaissance_seasia_sleeveless_courtvest", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "vest",
		ShortDescription = "a long sleeveless $colour silk vest",
		FullDescription = "A long sleeveless $colour silk vest is cut and sewn from silk so that the outward silhouette of the vest remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the vest bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 230,
		Cost = 134.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 8 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaSplitRidingWaistcloth() => new()
	{
		Metadata = new("renaissance_seasia_split_riding_waistcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "waistcloth",
		ShortDescription = "a divided $colour cotton riding waistcloth",
		FullDescription = "A divided $colour cotton riding waistcloth is cut and sewn from cotton so that the outward silhouette of the waistcloth remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the waistcloth, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500,
		Cost = 21.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Second-pass military admission; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaStructuredHeadclothcrown() => new()
	{
		Metadata = new("renaissance_seasia_structured_headclothcrown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "headcloth",
		ShortDescription = "a $colour crown-shaped structured headcloth",
		FullDescription = "A $colour crown-shaped structured headcloth is cut and sewn from silk so that the outward silhouette of the headcloth remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the headcloth around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260,
		Cost = 48.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: court/official/ritual gate; folding and colour are skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSeasiaWrappedMonasticrobe() => new()
	{
		Metadata = new("renaissance_seasia_wrapped_monasticrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a $colour draped monastic shoulder robe",
		FullDescription = "A $colour draped monastic shoulder robe is cut and sewn from cotton so that the outward silhouette of the robe remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 36.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: named Buddhist institution and local date gate; colour/order skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingAnkleBoots() => new()
	{
		Metadata = new("renaissance_shared_clothing_ankle_boots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of ankle leather boots",
		FullDescription = "A pair of ankle leather boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950,
		Cost = 51.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 13 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingBreastWrap() => new()
	{
		Metadata = new("renaissance_shared_clothing_breast_wrap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "wrap",
		ShortDescription = "a $colour folded breast wrap",
		FullDescription = "A $colour folded breast wrap is cut and sewn from linen so that the outward silhouette of the wrap remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the wrap bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80,
		Cost = 6.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Bra", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 65 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingBrimmedLeatherTravelhat() => new()
	{
		Metadata = new("renaissance_shared_clothing_brimmed_leather_travelhat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a $colour brimmed leather travel hat",
		FullDescription = "A $colour brimmed leather travel hat is cut and stitched from leather so that the outward silhouette of the hat remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 20.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Hard-wearing travel, hunting, courier, pastoral, and maritime-contact form; no weatherproof mechanic.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingCeremonialMantle() => new()
	{
		Metadata = new("renaissance_shared_clothing_ceremonial_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a $colour long ceremonial mantle",
		FullDescription = "A $colour long ceremonial mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 620,
		Cost = 110.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: train/no train; clasp/ties; silk/brocade skins court, civic, religious, diplomatic, and performance contexts only",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingChincordRidinghat() => new()
	{
		Metadata = new("renaissance_shared_clothing_chincord_ridinghat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a $colour chin-cord riding hat",
		FullDescription = "A $colour chin-cord riding hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 12.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Low or moderate crown with securing cord; courier, hunting, caravan, and mounted-work admissions.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingCloseClothCap() => new()
	{
		Metadata = new("renaissance_shared_clothing_close_cloth_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a close-fitting $colour cloth cap",
		FullDescription = "A close-fitting $colour cloth cap is cut and sewn from linen so that the outward silhouette of the cap remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140,
		Cost = 12.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 6 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingClothGloves() => new()
	{
		Metadata = new("renaissance_shared_clothing_cloth_gloves", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "gloves",
		ShortDescription = "a pair of $colour cloth gloves",
		FullDescription = "A pair of $colour cloth gloves is cut and sewn from wool so that the outward silhouette of the gloves remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the gloves bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 14.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gloves", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: short/long cuff; plain/embroidered; silk skin cool-climate, court, clerical, scholarly, ceremonial admissions",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingClothHeadwrap() => new()
	{
		Metadata = new("renaissance_shared_clothing_cloth_headwrap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "headwrap",
		ShortDescription = "a long $colour cloth headwrap",
		FullDescription = "A long $colour cloth headwrap is cut and sewn from cotton so that the outward silhouette of the headwrap remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the headwrap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220,
		Cost = 15.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 49 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingCrossfrontJacket() => new()
	{
		Metadata = new("renaissance_shared_clothing_crossfront_jacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a $colour cross-front jacket",
		FullDescription = "A $colour cross-front jacket is cut and sewn from cotton so that the outward silhouette of the jacket remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jacket bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760,
		Cost = 28.8m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: left/right overlap; tied/belted; silk/ramie skins East, South-east, Central, and contact-zone Asian admissions",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingDetachableSleeves() => new()
	{
		Metadata = new("renaissance_shared_clothing_detachable_sleeves", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "sleeves",
		ShortDescription = "a pair of $colour detachable sleeves",
		FullDescription = "A pair of $colour detachable sleeves is cut and sewn from silk so that the outward silhouette of the sleeves remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the sleeves bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 260,
		Cost = 36.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Detachable_Sleeves", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: tight/full; tied/laced; linen/wool/velvet skins Western European, Ottoman, Persianate, South Asian and courtly layered admissions where construction supports it",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingDrawstringDrawers() => new()
	{
		Metadata = new("renaissance_shared_clothing_drawstring_drawers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "drawers",
		ShortDescription = "a pair of $colour loose drawstring drawers",
		FullDescription = "A pair of $colour loose drawstring drawers is cut and sewn from linen so that the outward silhouette of the drawers remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the drawers carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140,
		Cost = 7.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shorts", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 52 outfit(s). Second-pass military admission; used in 81 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFaceVeil() => new()
	{
		Metadata = new("renaissance_shared_clothing_face_veil", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "veil",
		ShortDescription = "a $colour draped face veil",
		FullDescription = "A $colour draped face veil is cut and sewn from silk so that the outward silhouette of the veil remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the veil around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 180,
		Cost = 36.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Veil", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: nose-to-chin/full lower face; tied/pinned; cotton skin culture/date/status-gated Ottoman, Persianate, South Asian, African, and ceremonial admissions",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFeltBrimmedHat() => new()
	{
		Metadata = new("renaissance_shared_clothing_felt_brimmed_hat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a $colour brimmed felt hat",
		FullDescription = "A $colour brimmed felt hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 170,
		Cost = 12.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 21 outfit(s). Second-pass military admission; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFeltOvershoes() => new()
	{
		Metadata = new("renaissance_shared_clothing_felt_overshoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "overshoes",
		ShortDescription = "a pair of $colour felt overshoes",
		FullDescription = "A pair of $colour felt overshoes is cut and sewn from felt so that the outward silhouette of the overshoes remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the overshoes carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380,
		Cost = 14.4m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Overshoes", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Northern, steppe, mountain, and winter-household layer worn over shoes or footwraps.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFineVelvetSlippers() => new()
	{
		Metadata = new("renaissance_shared_clothing_fine_velvet_slippers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "slippers",
		ShortDescription = "a pair of $colour fine velvet slippers",
		FullDescription = "A pair of $colour fine velvet slippers is cut and sewn from velvet so that the outward silhouette of the slippers remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The slippers is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 690,
		Cost = 110.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Court, prosperous household, diplomatic, scholar, and ceremonial indoor footwear; embroidery and jewels are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFoldedPilgrimhat() => new()
	{
		Metadata = new("renaissance_shared_clothing_folded_pilgrimhat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a $colour folded-brim pilgrim hat",
		FullDescription = "A $colour folded-brim pilgrim hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 12.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Road, shrine, confraternity, and long-distance travel admission; badges and shells remain separate accessories.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFoldingHandFan() => new()
	{
		Metadata = new("renaissance_shared_clothing_folding_hand_fan", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "fan",
		ShortDescription = "a $colour folding hand fan",
		FullDescription = "A $colour folding hand fan is shaped and bound from wood so that the outward silhouette of the fan remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The contact points and fastenings are kept smooth, while the visible body of the fan carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80,
		Cost = 6.4m,
		Skinnable = true,
		Material = "wood",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: paper/silk leaf; plain/painted; court/merchant skins late-period East Asian, European court/contact, Ottoman and maritime trade admissions; date gate required",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFootedStockings() => new()
	{
		Metadata = new("renaissance_shared_clothing_footed_stockings", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "stockings",
		ShortDescription = "a pair of $colour footed wool stockings",
		FullDescription = "A pair of $colour footed wool stockings is cut and sewn from wool so that the outward silhouette of the stockings remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The stockings is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280,
		Cost = 13.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Stockings", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 39 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFootwraps() => new()
	{
		Metadata = new("renaissance_shared_clothing_footwraps", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "footwraps",
		ShortDescription = "a pair of $colour cloth footwraps",
		FullDescription = "A pair of $colour cloth footwraps is cut and sewn from linen so that the outward silhouette of the footwraps remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The footwraps is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 200,
		Cost = 12.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Stockings", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 4 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFrontOpeningLongCoat() => new()
	{
		Metadata = new("renaissance_shared_clothing_front_opening_long_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a $colour front-opening long coat",
		FullDescription = "A $colour front-opening long coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1220,
		Cost = 50.4m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: calf/ankle; fitted/loose; fur-lined skin steppe, Eastern European, Ottoman, Persianate, and northern travel contexts",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFrontlacedAnkleboots() => new()
	{
		Metadata = new("renaissance_shared_clothing_frontlaced_ankleboots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of $colour front-laced ankle boots",
		FullDescription = "A pair of $colour front-laced ankle boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1050,
		Cost = 60.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Travel, work, guard, courier, and mounted admissions; lower and more fitted than high riding boots.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFullTrousers() => new()
	{
		Metadata = new("renaissance_shared_clothing_full_trousers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "trousers",
		ShortDescription = "a pair of full $colour cotton trousers",
		FullDescription = "A pair of full $colour cotton trousers is cut and sewn from cotton so that the outward silhouette of the trousers remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the trousers, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500,
		Cost = 21.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 13 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSharedClothingFurLinedCoat() => new()
	{
		Metadata = new("renaissance_shared_clothing_fur_lined_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a $colour fur-lined travelling coat",
		FullDescription = "A $colour fur-lined travelling coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1230,
		Cost = 127.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
