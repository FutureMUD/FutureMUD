#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources01Stock
{
	public static CatalogueItemDefinition RenaissanceAndeanFeatherworkTunic() => new()
	{
		Metadata = new("renaissance_andean_featherwork_tunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a $colour feather-panelled ceremonial tunic",
		FullDescription = "A $colour feather-panelled ceremonial tunic is shaped and bound from featherwork so that the outward silhouette of the tunic remains clear. The featherwork keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The body and openings are proportioned for close, practical wear, with reinforcement placed where the tunic bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 480,
		Cost = 70.0m,
		Skinnable = true,
		Material = "featherwork",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: **material dependency**; court/state/ritual gate and trade-status note",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanFineTapestrytunic() => new()
	{
		Metadata = new("renaissance_andean_fine_tapestrytunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a $colour fine tapestry-woven tunic",
		FullDescription = "A $colour fine tapestry-woven tunic is cut and sewn from camelid wool so that the outward silhouette of the tunic remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the tunic bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480,
		Cost = 23.8m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shirt", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: **material dependency**; court/state/office/ceremonial gate; motif is skin",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanFourcornerWovencap() => new()
	{
		Metadata = new("renaissance_andean_fourcorner_wovencap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour four-cornered camelid-wool cap",
		FullDescription = "A $colour four-cornered camelid-wool cap is cut and sewn from camelid wool so that the outward silhouette of the cap remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 17.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Locally woven cap with a visibly four-cornered crown for Andean regional, office, festival, and status admissions; tassels and motifs remain skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanFringedCamelidshawl() => new()
	{
		Metadata = new("renaissance_andean_fringed_camelidshawl", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "shawl",
		ShortDescription = "a fringed $colour camelid-wool shawl",
		FullDescription = "A fringed $colour camelid-wool shawl is cut and sewn from camelid wool so that the outward silhouette of the shawl remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the shawl its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650,
		Cost = 32.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 4 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanFringedCeremonialapron() => new()
	{
		Metadata = new("renaissance_andean_fringed_ceremonialapron", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "apron",
		ShortDescription = "a $colour fringed ceremonial waist apron",
		FullDescription = "A $colour fringed ceremonial waist apron is cut and sewn from camelid wool so that the outward silhouette of the apron remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the apron, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680,
		Cost = 30.6m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: **material dependency**; court/ritual/performance/status gate",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanLongHeadcloth() => new()
	{
		Metadata = new("renaissance_andean_long_headcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "headcloth",
		ShortDescription = "a long $colour camelid-wool headcloth",
		FullDescription = "A long $colour camelid-wool headcloth is cut and sewn from camelid wool so that the outward silhouette of the headcloth remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the headcloth around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 250,
		Cost = 18.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanPairedShouldermantle() => new()
	{
		Metadata = new("renaissance_andean_paired_shouldermantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad pinned $colour camelid-wool shoulder mantle",
		FullDescription = "A broad pinned $colour camelid-wool shoulder mantle is cut and sewn from camelid wool so that the outward silhouette of the mantle remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650,
		Cost = 32.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 3 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanPinnedWrapskirt() => new()
	{
		Metadata = new("renaissance_andean_pinned_wrapskirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "skirt",
		ShortDescription = "a pinned broad $colour camelid-wool wrap skirt",
		FullDescription = "A pinned broad $colour camelid-wool wrap skirt is cut and sewn from camelid wool so that the outward silhouette of the skirt remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the skirt, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 740,
		Cost = 31.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 1 outfit(s). Second-pass military admission; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanRawhideRoadsandals() => new()
	{
		Metadata = new("renaissance_andean_rawhide_roadsandals", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "sandals",
		ShortDescription = "a pair of $colour rawhide road sandals",
		FullDescription = "A pair of $colour rawhide road sandals is cut and stitched from rawhide so that the outward silhouette of the sandals remains clear. The rawhide shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The sandals is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 15.6m,
		Skinnable = true,
		Material = "rawhide",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sandals", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Highland road, agricultural, porter, courier, and military-support contexts; altitude and local community required.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanShortWorktunic() => new()
	{
		Metadata = new("renaissance_andean_short_worktunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a short coarse $colour camelid-wool tunic",
		FullDescription = "A short coarse $colour camelid-wool tunic is cut and sewn from camelid wool so that the outward silhouette of the tunic remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the tunic bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 440,
		Cost = 20.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sleeveless_Tunic", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 1 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanStraightSleevelesstunic() => new()
	{
		Metadata = new("renaissance_andean_straight_sleevelesstunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a straight sleeveless $colour camelid-wool tunic",
		FullDescription = "A straight sleeveless $colour camelid-wool tunic is cut and sewn from camelid wool so that the outward silhouette of the tunic remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the tunic bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 440,
		Cost = 20.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sleeveless_Tunic", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanWovenEarflapcap() => new()
	{
		Metadata = new("renaissance_andean_woven_earflapcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a woven $colour camelid-wool ear-flap cap",
		FullDescription = "A woven $colour camelid-wool ear-flap cap is cut and sewn from camelid wool so that the outward silhouette of the cap remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 15.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceAndeanWrappedFulllengthdress() => new()
	{
		Metadata = new("renaissance_andean_wrapped_fulllengthdress", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "dress",
		ShortDescription = "a wrapped full-length $colour camelid-wool dress",
		FullDescription = "A wrapped full-length $colour camelid-wool dress is cut and sewn from camelid wool so that the outward silhouette of the dress remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the dress its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 890,
		Cost = 42.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Dress", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceCaribbeanCottonLoinapron() => new()
	{
		Metadata = new("renaissance_caribbean_cotton_loinapron", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "apron",
		ShortDescription = "a panelled $colour cotton loin apron",
		FullDescription = "A panelled $colour cotton loin apron is cut and sewn from cotton so that the outward silhouette of the apron remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the apron, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140,
		Cost = 7.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Breechcloth", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 1 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceCaribbeanCottonWrapskirt() => new()
	{
		Metadata = new("renaissance_caribbean_cotton_wrapskirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "skirt",
		ShortDescription = "a short $colour cotton wrap skirt",
		FullDescription = "A short $colour cotton wrap skirt is cut and sewn from cotton so that the outward silhouette of the skirt remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the skirt, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 510,
		Cost = 21.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceCaribbeanFeatheredHeadband() => new()
	{
		Metadata = new("renaissance_caribbean_feathered_headband", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "headband",
		ShortDescription = "a $colour narrow feathered headband",
		FullDescription = "A $colour narrow feathered headband is shaped and bound from featherwork so that the outward silhouette of the headband remains clear. The featherwork keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the headband around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 50.0m,
		Skinnable = true,
		Material = "featherwork",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: **material dependency**; status/ritual/dance gate",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceCaribbeanGatheredRawhideShoes() => new()
	{
		Metadata = new("renaissance_caribbean_gathered_rawhide_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of $colour side-stitched rawhide shoes",
		FullDescription = "A pair of $colour side-stitched rawhide shoes is cut and stitched from rawhide so that the outward silhouette of the shoes remains clear. The rawhide shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 690,
		Cost = 28.6m,
		Skinnable = true,
		Material = "rawhide",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Soft closed footwear for narrower Caribbean and Atlantic-contact admissions where a side-stitched hide upper is locally supported; not universal island dress.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceCaribbeanSleevelessCottontunic() => new()
	{
		Metadata = new("renaissance_caribbean_sleeveless_cottontunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a loose sleeveless $colour cotton tunic",
		FullDescription = "A loose sleeveless $colour cotton tunic is cut and sewn from cotton so that the outward silhouette of the tunic remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the tunic bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380,
		Cost = 17.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sleeveless_Tunic", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 5 outfit(s). Second-pass military admission; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceCaribbeanWovenFibresandals() => new()
	{
		Metadata = new("renaissance_caribbean_woven_fibresandals", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "sandals",
		ShortDescription = "a pair of $colour woven fibre sandals",
		FullDescription = "A pair of $colour woven fibre sandals is cut and sewn from canvas so that the outward silhouette of the sandals remains clear. The canvas falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The sandals is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 370,
		Cost = 11.0m,
		Skinnable = true,
		Material = "canvas",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sandals", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Second-pass military admission; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceCaribbeanWovenShouldercape() => new()
	{
		Metadata = new("renaissance_caribbean_woven_shouldercape", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cape",
		ShortDescription = "a short $colour cotton shoulder cape",
		FullDescription = "A short $colour cotton shoulder cape is cut and sewn from cotton so that the outward silhouette of the cape remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cape its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 27.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 3 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceContactBroadbrimSunhood() => new()
	{
		Metadata = new("renaissance_contact_broadbrim_sunhood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a $colour broad-brimmed cloth sun hood",
		FullDescription = "A $colour broad-brimmed cloth sun hood is cut and sewn from canvas so that the outward silhouette of the hood remains clear. The canvas falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300,
		Cost = 10.8m,
		Skinnable = true,
		Material = "canvas",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hood", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: colonial/contact labour, maritime, mission, travel gate; never universal local continuity",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceContactHybridButtonedWrapjacket() => new()
	{
		Metadata = new("renaissance_contact_hybrid_buttoned_wrapjacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "front",
		ShortDescription = "a $colour wrap jacket with buttoned front",
		FullDescription = "A $colour wrap jacket with buttoned front is cut and sewn from cotton so that the outward silhouette of the front remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the front bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760,
		Cost = 28.8m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: contact-zone craft/fastening gate; button functionality remains descriptive unless component supports it",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceContactHybridLacedtunic() => new()
	{
		Metadata = new("renaissance_contact_hybrid_lacedtunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "lacing",
		ShortDescription = "a $colour side-slit cotton tunic with front lacing",
		FullDescription = "A $colour side-slit cotton tunic with front lacing is cut and sewn from cotton so that the outward silhouette of the lacing remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the lacing bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480,
		Cost = 16.8m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: genuine contact-zone hybrid only; local cut plus introduced lacing materially visible",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceContactMissionCatechistrobe() => new()
	{
		Metadata = new("renaissance_contact_mission_catechistrobe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a $colour plain long mission-associated robe",
		FullDescription = "A $colour plain long mission-associated robe is cut and sewn from linen so that the outward silhouette of the robe remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 33.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: imposed/adopted/locally made status must be recorded; mission institution required",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceContactTradeclothPanelskirt() => new()
	{
		Metadata = new("renaissance_contact_tradecloth_panelskirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "skirt",
		ShortDescription = "a $colour pieced trade-cloth panel skirt",
		FullDescription = "A $colour pieced trade-cloth panel skirt is cut and sewn from cotton so that the outward silhouette of the skirt remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the skirt, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680,
		Cost = 21.6m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Skirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: imported cloth/local construction recorded explicitly; panel piecing integral",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierBeltedWoolCaftan() => new()
	{
		Metadata = new("renaissance_frontier_belted_wool_caftan", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "caftan",
		ShortDescription = "a belted $colour wool caftan",
		FullDescription = "A belted $colour wool caftan is cut and sewn from wool so that the outward silhouette of the caftan remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the caftan its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1230,
		Cost = 61.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 2 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierCloseButtonedLongcoat() => new()
	{
		Metadata = new("renaissance_frontier_close_buttoned_longcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a close-buttoned $colour long coat",
		FullDescription = "A close-buttoned $colour long coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the coat its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1230,
		Cost = 61.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 5 outfit(s). Second-pass military admission; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierFeltedWinterBoots() => new()
	{
		Metadata = new("renaissance_frontier_felted_winter_boots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of felted winter boots",
		FullDescription = "A pair of felted winter boots is cut and sewn from felt so that the outward silhouette of the boots remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760,
		Cost = 34.0m,
		Skinnable = false,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_LightClothing", "Insulation_Moderate"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 4 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierFurBrimmedCap() => new()
	{
		Metadata = new("renaissance_frontier_fur_brimmed_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a soft $colour fur-brimmed cap",
		FullDescription = "A soft $colour fur-brimmed cap is cut and sewn from wool so that the outward silhouette of the cap remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 13.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 5 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierFurTrimmedShortCoat() => new()
	{
		Metadata = new("renaissance_frontier_fur_trimmed_short_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a $colour fur-trimmed short coat",
		FullDescription = "A $colour fur-trimmed short coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760,
		Cost = 33.6m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: hip/thigh length; merchant, noble, frontier, and winter skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierFurlinedRidingcap() => new()
	{
		Metadata = new("renaissance_frontier_furlined_ridingcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour fur-lined riding cap",
		FullDescription = "A $colour fur-lined riding cap is cut and sewn from wool so that the outward silhouette of the cap remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 14.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Central/Eastern frontier, Muscovite, Baltic, courier, and mounted winter dress; distinguish from tall ceremonial fur hats.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierHighFeltCap() => new()
	{
		Metadata = new("renaissance_frontier_high_felt_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour high-crowned felt cap",
		FullDescription = "A $colour high-crowned felt cap is cut and sewn from felt so that the outward silhouette of the cap remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 12.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: plain, corded, feathered, and military-status skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierKnittedNorthernCap() => new()
	{
		Metadata = new("renaissance_frontier_knitted_northern_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a long $colour knitted wool cap",
		FullDescription = "A long $colour knitted wool cap is cut and sewn from wool so that the outward silhouette of the cap remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 13.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 4 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierLongFurredOvercoat() => new()
	{
		Metadata = new("renaissance_frontier_long_furred_overcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "overcoat",
		ShortDescription = "a $colour long furred overcoat",
		FullDescription = "A $colour long furred overcoat is cut and sewn from broadcloth so that the outward silhouette of the overcoat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the overcoat its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1220,
		Cost = 72.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: calf/ankle; broad/narrow collar; northern and Muscovite trade networks",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceFrontierMuscoviteLongCaftan() => new()
	{
		Metadata = new("renaissance_frontier_muscovite_long_caftan", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "caftan",
		ShortDescription = "a full-skirted $colour long caftan",
		FullDescription = "A full-skirted $colour long caftan is cut and sewn from broadcloth so that the outward silhouette of the caftan remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the caftan its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1340,
		Cost = 196.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "Cross-era Renaissance stable-reference admission; used in 3 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
