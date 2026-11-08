#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources01Stock
{
	public static CatalogueItemDefinition RenaissanceSouthasianSplitRidingWaistcloth() => new()
	{
		Metadata = new("renaissance_southasian_split_riding_waistcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "waistcloth",
		ShortDescription = "a $colour divided riding waistcloth",
		FullDescription = "A $colour divided riding waistcloth is cut and sewn from cotton so that the outward silhouette of the waistcloth remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the waistcloth, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620,
		Cost = 21.6m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: wrapped to form two leg divisions; cavalry, courier, court, rural skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSouthasianStitchedLoindrawers() => new()
	{
		Metadata = new("renaissance_southasian_stitched_loindrawers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "drawers",
		ShortDescription = "a pair of close $colour cotton loin drawers",
		FullDescription = "A pair of close $colour cotton loin drawers is cut and sewn from cotton so that the outward silhouette of the drawers remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the drawers carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150,
		Cost = 7.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shorts", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 13 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSouthasianToepostWoodensandals() => new()
	{
		Metadata = new("renaissance_southasian_toepost_woodensandals", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "sandals",
		ShortDescription = "a pair of $colour toe-post wooden sandals",
		FullDescription = "These sandals consist of shaped $colour wooden footboards with a short post rising near the front of each one. The posts end in rounded knobs, leaving a narrow neck for the toes to grip. The boards are smooth on top and thicker beneath the heels, with gently rounded edges following the outline of the foot. Visible grain runs through the plain wood, uninterrupted by straps, inlays or a painted border.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450,
		Cost = 10.0m,
		Skinnable = true,
		Material = "wood",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sandals", "Armour_LightClothing", "Insulation_Minor", "Variable_WoodColour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSteppeFurEarflaphat() => new()
	{
		Metadata = new("renaissance_steppe_fur_earflaphat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a furred ear-flap hat",
		FullDescription = "A furred ear-flap hat is cut and stitched from fur so that the outward silhouette of the hat remains clear. The fur shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220,
		Cost = 28.0m,
		Skinnable = false,
		Material = "fur",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_HeavyClothing", "Insulation_Strong"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 4 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSteppePointedFeltcap() => new()
	{
		Metadata = new("renaissance_steppe_pointed_feltcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a pointed $colour felt riding cap",
		FullDescription = "A pointed $colour felt riding cap is cut and sewn from felt so that the outward silhouette of the cap remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 170,
		Cost = 12.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 6 outfit(s). Second-pass military admission; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSteppeQuiltedRidingrobe() => new()
	{
		Metadata = new("renaissance_steppe_quilted_ridingrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a quilted side-fastened $colour riding robe",
		FullDescription = "A quilted side-fastened $colour riding robe is cut and sewn from cotton so that the outward silhouette of the robe remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 810,
		Cost = 40.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_HeavyClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Second-pass military admission; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSteppeShortSplitRidingcoat() => new()
	{
		Metadata = new("renaissance_steppe_short_split_ridingcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a short split-skirt $colour riding coat",
		FullDescription = "A short split-skirt $colour riding coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720,
		Cost = 33.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 4 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSteppeSidefastenedFurcoat() => new()
	{
		Metadata = new("renaissance_steppe_sidefastened_furcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a side-fastened $colour fur-lined long coat",
		FullDescription = "A side-fastened $colour fur-lined long coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 990,
		Cost = 88.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 4 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSteppeSleevelessRidingvest() => new()
	{
		Metadata = new("renaissance_steppe_sleeveless_ridingvest", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "vest",
		ShortDescription = "a sleeveless leather riding vest",
		FullDescription = "A sleeveless leather riding vest is cut and stitched from leather so that the outward silhouette of the vest remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The body and openings are proportioned for close, practical wear, with reinforcement placed where the vest bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450,
		Cost = 30.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 5 outfit(s). Second-pass military admission; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSteppeSoftHighboots() => new()
	{
		Metadata = new("renaissance_steppe_soft_highboots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of soft high leather boots",
		FullDescription = "A pair of soft high leather boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1220,
		Cost = 72.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_High_Boots", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 10 outfit(s). Second-pass military admission; used in 6 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceSteppeWideRidingtrousers() => new()
	{
		Metadata = new("renaissance_steppe_wide_ridingtrousers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "trousers",
		ShortDescription = "a pair of wide $colour wool riding trousers",
		FullDescription = "A pair of wide $colour wool riding trousers is cut and sewn from wool so that the outward silhouette of the trousers remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the trousers, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620,
		Cost = 22.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Trousers", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 5 outfit(s). Second-pass military admission; used in 6 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternBackLacedBodice() => new()
	{
		Metadata = new("renaissance_western_back_laced_bodice", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "bodice",
		ShortDescription = "a $colour back-laced fitted bodice",
		FullDescription = "A $colour back-laced fitted bodice is cut and sewn from broadcloth so that the outward silhouette of the bodice remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the bodice bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: requires assisted-dressing context only in builder notes, not mechanics",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternBroadPlumedRidinghat() => new()
	{
		Metadata = new("renaissance_western_broad_plumed_ridinghat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a $colour broad plumed riding hat",
		FullDescription = "A $colour broad plumed riding hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 12.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Mounted court, hunting, courier, officer-presentation, and pageant use; plume and hatband are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternChopines() => new()
	{
		Metadata = new("renaissance_western_chopines", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "overshoes",
		ShortDescription = "a pair of $colour tall platform overshoes",
		FullDescription = "A pair of $colour tall platform overshoes is shaped and bound from wood so that the outward silhouette of the overshoes remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The contact points and fastenings are kept smooth, while the visible body of the overshoes carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380,
		Cost = 9.6m,
		Skinnable = true,
		Material = "wood",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Overshoes", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Venetian/Mediterranean elite gate; moderate and tall variants may need separate sizes, not prototypes",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternClothJerkin() => new()
	{
		Metadata = new("renaissance_western_cloth_jerkin", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "jerkin",
		ShortDescription = "a $colour fitted cloth jerkin",
		FullDescription = "A $colour fitted cloth jerkin is cut and sewn from broadcloth so that the outward silhouette of the jerkin remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jerkin bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: plain, guarded, slashed, and livery skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternDressApron() => new()
	{
		Metadata = new("renaissance_western_dress_apron", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "apron",
		ShortDescription = "a $colour fine full dress apron",
		FullDescription = "A $colour fine full dress apron is cut and sewn from linen so that the outward silhouette of the apron remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the apron, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680,
		Cost = 19.8m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: narrow/full, plain/lace-edged, domestic and status-display skins; distinct from work apron by cut",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternEmbroideredLinenNightcap() => new()
	{
		Metadata = new("renaissance_western_embroidered_linen_nightcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "nightcap",
		ShortDescription = "a $colour embroidered linen nightcap",
		FullDescription = "A $colour embroidered linen nightcap is cut and sewn from linen so that the outward silhouette of the nightcap remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the nightcap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 11.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Late-sixteenth-century prosperous and elite informal indoor wear; simpler quilted skins may admit scholars and merchants.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFalseSleeveOvergown() => new()
	{
		Metadata = new("renaissance_western_false_sleeve_overgown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "overgown",
		ShortDescription = "a $colour false-sleeve overgown",
		FullDescription = "A $colour false-sleeve overgown is cut and sewn from broadcloth so that the outward silhouette of the overgown remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the overgown its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1220,
		Cost = 72.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: decorative dependent sleeves are integral silhouette",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFeatheredVelvetBonnet() => new()
	{
		Metadata = new("renaissance_western_feathered_velvet_bonnet", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "bonnet",
		ShortDescription = "a $colour feathered velvet bonnet",
		FullDescription = "A $colour feathered velvet bonnet is cut and sewn from velvet so that the outward silhouette of the bonnet remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the bonnet around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 50.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Court, merchant, diplomatic, and performance skins; feather placement remains cosmetic.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFineLatchetShoes() => new()
	{
		Metadata = new("renaissance_western_fine_latchet_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of $colour fine latchet shoes",
		FullDescription = "A pair of $colour fine latchet shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 690,
		Cost = 44.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Prosperous town, court, legal, diplomatic, and formal household footwear; tie or buckle skin.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFittedCourtGown() => new()
	{
		Metadata = new("renaissance_western_fitted_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a $colour fitted court gown",
		FullDescription = "A $colour fitted court gown is cut and sewn from velvet so that the outward silhouette of the gown remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the gown its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 980,
		Cost = 140.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Dress", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: silk/satin/brocade skins; heraldry and jewellery remain separate presentation",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFittedDressGloves() => new()
	{
		Metadata = new("renaissance_western_fitted_dress_gloves", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "gloves",
		ShortDescription = "a pair of $colour fitted dress gloves",
		FullDescription = "A pair of $colour fitted dress gloves is cut and stitched from leather so that the outward silhouette of the gloves remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The body and openings are proportioned for close, practical wear, with reinforcement placed where the gloves bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 20.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gloves", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: plain, embroidered, slashed-cuff, scented-presentation skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFittedKirtle() => new()
	{
		Metadata = new("renaissance_western_fitted_kirtle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "kirtle",
		ShortDescription = "a $colour close-fitted kirtle",
		FullDescription = "A $colour close-fitted kirtle is cut and sewn from broadcloth so that the outward silhouette of the kirtle remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the kirtle its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 56.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Dress", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: square/rounded neck and narrow/full skirt skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFlatCap() => new()
	{
		Metadata = new("renaissance_western_flat_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour broad flat cap",
		FullDescription = "A $colour broad flat cap is cut and sewn from felt so that the outward silhouette of the cap remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 12.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: narrow/broad crown; plain, feathered, badge, and velvet skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFrenchHood() => new()
	{
		Metadata = new("renaissance_western_french_hood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a $colour crescent-framed court hood",
		FullDescription = "A $colour crescent-framed court hood is cut and sewn from velvet so that the outward silhouette of the hood remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 50.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: restrained and jewelled-presentation skins; sixteenth-century court gate",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFrontLacedBodice() => new()
	{
		Metadata = new("renaissance_western_front_laced_bodice", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "bodice",
		ShortDescription = "a $colour front-laced fitted bodice",
		FullDescription = "A $colour front-laced fitted bodice is cut and sewn from broadcloth so that the outward silhouette of the bodice remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the bodice bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: square/rounded neck; work, urban, and fine skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFrontOpeningGown() => new()
	{
		Metadata = new("renaissance_western_front_opening_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "overgown",
		ShortDescription = "a $colour front-opening overgown",
		FullDescription = "A $colour front-opening overgown is cut and sewn from broadcloth so that the outward silhouette of the overgown remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the overgown its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1220,
		Cost = 72.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: reveals kirtle/petticoat; plain, furred, and guarded skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFrontOpeningKirtle() => new()
	{
		Metadata = new("renaissance_western_front_opening_kirtle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "kirtle",
		ShortDescription = "a $colour front-opening kirtle",
		FullDescription = "A $colour front-opening kirtle is cut and sewn from broadcloth so that the outward silhouette of the kirtle remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the kirtle its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 56.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Dress", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: laced, hooked, or buttoned skins; no fastener mechanics claimed",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFullCircleCloak() => new()
	{
		Metadata = new("renaissance_western_full_circle_cloak", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a $colour full circular cloak",
		FullDescription = "A $colour full circular cloak is cut and sewn from broadcloth so that the outward silhouette of the cloak remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1050,
		Cost = 56.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: calf/ankle length; hooded version uses common hooded-cloak prototype instead",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFullPetticoat() => new()
	{
		Metadata = new("renaissance_western_full_petticoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "petticoat",
		ShortDescription = "a $colour full sewn petticoat",
		FullDescription = "A $colour full sewn petticoat is cut and sewn from wool so that the outward silhouette of the petticoat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the petticoat, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680,
		Cost = 25.2m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: plain, quilted, guarded, and silk skins; under- or visible-skirt admission",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFurlinedLowshoes() => new()
	{
		Metadata = new("renaissance_western_furlined_lowshoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of $colour fur-lined leather low shoes",
		FullDescription = "A pair of $colour fur-lined leather low shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 690,
		Cost = 44.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Warm low footwear for northern and central European winter town, court, household, coach, and road use; lining is structural rather than cosmetic.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternFurredGown() => new()
	{
		Metadata = new("renaissance_western_furred_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a $colour fur-edged long gown",
		FullDescription = "A $colour fur-edged long gown is cut and sewn from broadcloth so that the outward silhouette of the gown remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the gown its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1220,
		Cost = 72.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: scholar, magistrate, merchant, and court skins; audit exact fur material",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternGableHood() => new()
	{
		Metadata = new("renaissance_western_gable_hood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a $colour angular framed court hood",
		FullDescription = "A $colour angular framed court hood is cut and sewn from velvet so that the outward silhouette of the hood remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 50.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: English/Tudor gate; plain and rich skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternGatheredLinenShirt() => new()
	{
		Metadata = new("renaissance_western_gathered_linen_shirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "shirt",
		ShortDescription = "a $colour gathered linen shirt",
		FullDescription = "A $colour gathered linen shirt is cut and sewn from linen so that the outward silhouette of the shirt remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the shirt bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480,
		Cost = 15.4m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: low or moderate neck gathering; plain work, fine urban, and embroidered court skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternHangingSleeveOvergown() => new()
	{
		Metadata = new("renaissance_western_hanging_sleeve_overgown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "overgown",
		ShortDescription = "a $colour hanging-sleeve overgown",
		FullDescription = "A $colour hanging-sleeve overgown is cut and sewn from velvet so that the outward silhouette of the overgown remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the overgown its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1220,
		Cost = 180.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: open or closed hanging sleeve skins; court and ceremonial admission",
		AllowLegacyShortDescriptionMatch = false,
	};
}
