#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources02Stock
{
	public static CatalogueItemDefinition EarlymodernWesternClothingLinenNeckcloth() => new()
	{
		Metadata = new("earlymodern_western_clothing_linen_neckcloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "neckcloth",
		ShortDescription = "a $colour linen neckcloth",
		FullDescription = "A $colour linen neckcloth is cut and sewn from linen so that the outward silhouette of the neckcloth remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the neckcloth carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90,
		Cost = 7.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Scarf", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 21 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternClothingLinenShoulderKerchief() => new()
	{
		Metadata = new("earlymodern_western_clothing_linen_shoulder_kerchief", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kerchief",
		ShortDescription = "a $colour linen shoulder kerchief",
		FullDescription = "A $colour linen shoulder kerchief is cut and sewn from linen so that the outward silhouette of the kerchief remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the kerchief its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500,
		Cost = 26.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 24 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternClothingLinenWorkApron() => new()
	{
		Metadata = new("earlymodern_western_clothing_linen_work_apron", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
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
		BuilderNotes = "New Early Modern prototype; used in 19 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternClothingLongBroadclothCoat() => new()
	{
		Metadata = new("earlymodern_western_clothing_long_broadcloth_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a long $colour broadcloth coat",
		FullDescription = "A long $colour broadcloth coat is cut and sewn from broadcloth so that the outward silhouette of the coat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500,
		Cost = 221.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern prototype; used in 8 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternClothingLongLinenShift() => new()
	{
		Metadata = new("earlymodern_western_clothing_long_linen_shift", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shift",
		ShortDescription = "a long $colour linen shift",
		FullDescription = "A long $colour linen shift is cut and sewn from linen so that the outward silhouette of the shift remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the shift carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 740,
		Cost = 42.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 37 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternClothingPlainLeatherMules() => new()
	{
		Metadata = new("earlymodern_western_clothing_plain_leather_mules", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mules",
		ShortDescription = "a pair of plain leather mules",
		FullDescription = "A pair of plain leather mules is cut and stitched from leather so that the outward silhouette of the mules remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The mules is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650,
		Cost = 33.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "New Early Modern prototype; used in 1 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternClothingPlainLinenShirt() => new()
	{
		Metadata = new("earlymodern_western_clothing_plain_linen_shirt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shirt",
		ShortDescription = "a $colour plain linen shirt",
		FullDescription = "A $colour plain linen shirt is cut and sewn from linen so that the outward silhouette of the shirt remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the shirt bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260,
		Cost = 14.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 38 outfit(s). Second-pass military admission; used in 59 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternClothingPlainMantuaGown() => new()
	{
		Metadata = new("earlymodern_western_clothing_plain_mantua_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a plain $colour mantua gown",
		FullDescription = "A plain $colour mantua gown is cut and sewn from broadcloth so that the outward silhouette of the gown remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the gown its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1340,
		Cost = 196.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Open_Robe", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New Early Modern prototype; used in 13 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternClothingPlainWoolWaistcoat() => new()
	{
		Metadata = new("earlymodern_western_clothing_plain_wool_waistcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "waistcoat",
		ShortDescription = "a plain $colour wool waistcoat",
		FullDescription = "A plain $colour wool waistcoat is cut and sewn from wool so that the outward silhouette of the waistcoat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the waistcoat bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 400,
		Cost = 22.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 22 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternClothingWoolBonnet() => new()
	{
		Metadata = new("earlymodern_western_clothing_wool_bonnet", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "bonnet",
		ShortDescription = "a $colour wool bonnet",
		FullDescription = "A $colour wool bonnet is cut and sewn from wool so that the outward silhouette of the bonnet remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the bonnet around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 13.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 5 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryBroadServiceBonnet() => new()
	{
		Metadata = new("earlymodern_western_military_broad_service_bonnet", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "bonnet",
		ShortDescription = "a broad $colour wool military bonnet",
		FullDescription = "A broad $colour wool military bonnet is cut and sewn from wool so that the outward silhouette of the bonnet remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the bonnet around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240,
		Cost = 20.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryBuffLeatherCoat() => new()
	{
		Metadata = new("earlymodern_western_military_buff_leather_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a heavy buff leather coat",
		FullDescription = "A heavy buff leather coat is cut and stitched from leather so that the outward silhouette of the coat remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3500,
		Cost = 260.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 5 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryCanvasGaiters() => new()
	{
		Metadata = new("earlymodern_western_military_canvas_gaiters", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gaiters",
		ShortDescription = "a pair of $colour canvas gaiters",
		FullDescription = "A pair of $colour canvas gaiters is cut and sewn from canvas so that the outward silhouette of the gaiters remains clear. The canvas falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The gaiters is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 18.0m,
		Skinnable = true,
		Material = "canvas",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Leg_Wraps", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 19 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryCockedUniformHat() => new()
	{
		Metadata = new("earlymodern_western_military_cocked_uniform_hat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a military $colour cocked felt hat",
		FullDescription = "A military $colour cocked felt hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230,
		Cost = 24.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 25 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryDragoonCap() => new()
	{
		Metadata = new("earlymodern_western_military_dragoon_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a tall $colour wool dragoon cap",
		FullDescription = "A tall $colour wool dragoon cap is cut and sewn from wool so that the outward silhouette of the cap remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280,
		Cost = 30.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryFittedWaistcoat() => new()
	{
		Metadata = new("earlymodern_western_military_fitted_waistcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "waistcoat",
		ShortDescription = "a fitted $colour military waistcoat",
		FullDescription = "A fitted $colour military waistcoat is cut and sewn from broadcloth so that the outward silhouette of the waistcoat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the waistcoat bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520,
		Cost = 55.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 39 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryFurGrenadierCap() => new()
	{
		Metadata = new("earlymodern_western_military_fur_grenadier_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a tall fur-fronted grenadier cap",
		FullDescription = "A tall fur-fronted grenadier cap is cut and stitched from fur so that the outward silhouette of the cap remains clear. The fur shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 480,
		Cost = 80.0m,
		Skinnable = false,
		Material = "fur",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Strong"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 2 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryHighFeltServiceCap() => new()
	{
		Metadata = new("earlymodern_western_military_high_felt_service_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a high-crowned $colour felt service cap",
		FullDescription = "A high-crowned $colour felt service cap is cut and sewn from felt so that the outward silhouette of the cap remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230,
		Cost = 25.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryLeatherNeckStock() => new()
	{
		Metadata = new("earlymodern_western_military_leather_neck_stock", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "stock",
		ShortDescription = "a stiff black leather neck stock",
		FullDescription = "A stiff black leather neck stock is cut and stitched from leather so that the outward silhouette of the stock remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the stock carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 130,
		Cost = 14.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Scarf", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 8 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryLongUniformCoat() => new()
	{
		Metadata = new("earlymodern_western_military_long_uniform_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a long-skirted $colour uniform coat",
		FullDescription = "A long-skirted $colour uniform coat is cut and sewn from broadcloth so that the outward silhouette of the coat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1700,
		Cost = 150.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Strong", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 15 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryMarineShortCoat() => new()
	{
		Metadata = new("earlymodern_western_military_marine_short_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a short-skirted $colour marine coat",
		FullDescription = "A short-skirted $colour marine coat is cut and sewn from broadcloth so that the outward silhouette of the coat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200,
		Cost = 120.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 6 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryMitreGrenadierCap() => new()
	{
		Metadata = new("earlymodern_western_military_mitre_grenadier_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a tall $colour cloth grenadier cap",
		FullDescription = "A tall $colour cloth grenadier cap is cut and sewn from broadcloth so that the outward silhouette of the cap remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360,
		Cost = 55.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryOfficerFrockCoat() => new()
	{
		Metadata = new("earlymodern_western_military_officer_frock_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a finely laced $colour officer's coat",
		FullDescription = "A finely laced $colour officer's coat is cut and sewn from broadcloth so that the outward silhouette of the coat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1600,
		Cost = 300.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 8 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitaryShortServiceCoat() => new()
	{
		Metadata = new("earlymodern_western_military_short_service_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a short $colour military coat",
		FullDescription = "A short $colour military coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100,
		Cost = 90.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternMilitarySplitSkirtRidingCoat() => new()
	{
		Metadata = new("earlymodern_western_military_split_skirt_riding_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a split-skirt $colour riding coat",
		FullDescription = "A split-skirt $colour riding coat is cut and sewn from broadcloth so that the outward silhouette of the coat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1600,
		Cost = 180.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 6 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternNavalHeavyWatchCoat() => new()
	{
		Metadata = new("earlymodern_western_naval_heavy_watch_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a heavy $colour shipboard watch coat",
		FullDescription = "A heavy $colour shipboard watch coat is cut and sewn from broadcloth so that the outward silhouette of the coat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100,
		Cost = 160.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Strong", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternNavalOfficerBoatCloak() => new()
	{
		Metadata = new("earlymodern_western_naval_officer_boat_cloak", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a full $colour naval boat cloak",
		FullDescription = "A full $colour naval boat cloak is cut and sewn from broadcloth so that the outward silhouette of the cloak remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800,
		Cost = 180.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Closed)", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternNavalOfficerCockedHat() => new()
	{
		Metadata = new("earlymodern_western_naval_officer_cocked_hat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a fine $colour cocked hat",
		FullDescription = "A fine $colour cocked hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 240,
		Cost = 70.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 17 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternNavalOfficerDressCoat() => new()
	{
		Metadata = new("earlymodern_western_naval_officer_dress_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a long $colour naval officer's coat",
		FullDescription = "A long $colour naval officer's coat is cut and sewn from broadcloth so that the outward silhouette of the coat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1650,
		Cost = 320.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 5 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternNavalOfficerDressWaistcoat() => new()
	{
		Metadata = new("earlymodern_western_naval_officer_dress_waistcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "waistcoat",
		ShortDescription = "a fine $colour naval waistcoat",
		FullDescription = "A fine $colour naval waistcoat is cut and sewn from broadcloth so that the outward silhouette of the waistcoat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the waistcoat bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 480,
		Cost = 100.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 11 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternNavalOfficerTropicalCoat() => new()
	{
		Metadata = new("earlymodern_western_naval_officer_tropical_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a light $colour cotton officer's coat",
		FullDescription = "A light $colour cotton officer's coat is cut and sewn from cotton so that the outward silhouette of the coat remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 900,
		Cost = 130.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWesternNavalOfficerUndressFrock() => new()
	{
		Metadata = new("earlymodern_western_naval_officer_undress_frock", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "frock",
		ShortDescription = "a plain $colour naval officer's frock",
		FullDescription = "A plain $colour naval officer's frock is cut and sewn from broadcloth so that the outward silhouette of the frock remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the frock carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500,
		Cost = 190.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 4 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalBuddhistBlackMonasticRobe() => new()
	{
		Metadata = new("medieval_buddhist_black_monastic_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a black hemp monastic robe",
		FullDescription = "A black hemp monastic robe is cut and sewn from hemp so that the outward silhouette of the robe remains clear. The hemp falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 540,
		Cost = 18.0m,
		Skinnable = true,
		Material = "hemp",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalBuddhistNunRobe() => new()
	{
		Metadata = new("medieval_buddhist_nun_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a dark hemp nun robe",
		FullDescription = "A dark hemp nun robe is cut and sewn from hemp so that the outward silhouette of the robe remains clear. The hemp falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 18.0m,
		Skinnable = true,
		Material = "hemp",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalBuddhistPatchedKasaya() => new()
	{
		Metadata = new("medieval_buddhist_patched_kasaya", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kasaya",
		ShortDescription = "a patched cotton kasaya",
		FullDescription = "A patched cotton kasaya is cut and sewn from cotton so that the outward silhouette of the kasaya remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the kasaya its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320,
		Cost = 24.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Mantle", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};
}
