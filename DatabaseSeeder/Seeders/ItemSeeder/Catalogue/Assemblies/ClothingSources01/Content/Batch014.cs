#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources01Stock
{
	public static CatalogueItemDefinition RenaissanceWesternSideLacedHalfboots() => new()
	{
		Metadata = new("renaissance_western_side_laced_halfboots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "half-boots",
		ShortDescription = "a pair of $colour side-laced leather half-boots",
		FullDescription = "A pair of $colour side-laced leather half-boots is cut and stitched from leather so that the outward silhouette of the half-boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The half-boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1050,
		Cost = 60.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Ankle-to-lower-calf town and travel boots with a visible side closure; merchant, courier, guard, and country-household admissions.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSlashedDoublet() => new()
	{
		Metadata = new("renaissance_western_slashed_doublet", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "doublet",
		ShortDescription = "a $colour slashed-sleeve doublet",
		FullDescription = "A $colour slashed-sleeve doublet is cut and sewn from broadcloth so that the outward silhouette of the doublet remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the doublet bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: restrained, mercenary-influenced, and court skins; slash colours remain cosmetic",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSlashedPuffedCap() => new()
	{
		Metadata = new("renaissance_western_slashed_puffed_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour slashed and puffed court cap",
		FullDescription = "A $colour slashed and puffed court cap is cut and sewn from velvet so that the outward silhouette of the cap remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 50.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Early- and mid-sixteenth-century court, mercenary-influenced, pageant, and prosperous urban gate; lining colours are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSleevelessDoublet() => new()
	{
		Metadata = new("renaissance_western_sleeveless_doublet", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "doublet",
		ShortDescription = "a $colour fitted sleeveless doublet",
		FullDescription = "A $colour fitted sleeveless doublet is cut and sewn from broadcloth so that the outward silhouette of the doublet remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the doublet bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: plain work, merchant, quilted, and velvet court skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSleevelessJerkin() => new()
	{
		Metadata = new("renaissance_western_sleeveless_jerkin", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "jerkin",
		ShortDescription = "a $colour sleeveless skirted jerkin",
		FullDescription = "A $colour sleeveless skirted jerkin is cut and sewn from broadcloth so that the outward silhouette of the jerkin remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jerkin bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: short/long skirt skins; merchant, artisan, and court-servant admissions",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSleevelessKirtle() => new()
	{
		Metadata = new("renaissance_western_sleeveless_kirtle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "kirtle",
		ShortDescription = "a $colour sleeveless fitted kirtle",
		FullDescription = "A $colour sleeveless fitted kirtle is cut and sewn from broadcloth so that the outward silhouette of the kirtle remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the kirtle its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 56.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Dress", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: intended to display chemise or detachable sleeves",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSleevelessOverbodice() => new()
	{
		Metadata = new("renaissance_western_sleeveless_overbodice", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "over-bodice",
		ShortDescription = "a $colour sleeveless over-bodice",
		FullDescription = "A $colour sleeveless over-bodice is cut and sewn from velvet so that the outward silhouette of the over-bodice remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the over-bodice bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 560,
		Cost = 90.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: worn above chemise or undergown; plain wool and luxury silk skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSpanishFarthingale() => new()
	{
		Metadata = new("renaissance_western_spanish_farthingale", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "support",
		ShortDescription = "a $colour conical skirt support",
		FullDescription = "A $colour conical skirt support is cut and sewn from canvas so that the outward silhouette of the support remains clear. The canvas falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the support, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900,
		Cost = 21.6m,
		Skinnable = true,
		Material = "canvas",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Skirt_Support", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: late-fifteenth/sixteenth-century Iberian and court diffusion gate",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSplitHose() => new()
	{
		Metadata = new("renaissance_western_split_hose", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hose",
		ShortDescription = "a pair of $colour split hose",
		FullDescription = "A pair of $colour split hose is cut and sewn from wool so that the outward silhouette of the hose remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the hose carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520,
		Cost = 19.6m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Chausses", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: two-colour mi-parti skin permitted; one paired item",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSquareNeckBodice() => new()
	{
		Metadata = new("renaissance_western_square_neck_bodice", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "bodice",
		ShortDescription = "a $colour square-necked bodice",
		FullDescription = "A $colour square-necked bodice is cut and sewn from broadcloth so that the outward silhouette of the bodice remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the bodice bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: plain, guarded, brocade-skin, and velvet-skin families",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSquareNeckChemise() => new()
	{
		Metadata = new("renaissance_western_square_neck_chemise", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "chemise",
		ShortDescription = "a $colour square-necked chemise",
		FullDescription = "A $colour square-necked chemise is cut and sewn from linen so that the outward silhouette of the chemise remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the chemise its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 33.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: calf or ankle hem; narrow/full sleeves; Western court and urban outfits",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSquareNeckGown() => new()
	{
		Metadata = new("renaissance_western_square_neck_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a $colour square-necked fitted gown",
		FullDescription = "A $colour square-necked fitted gown is cut and sewn from broadcloth so that the outward silhouette of the gown remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the gown its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 56.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Dress", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: plain, velvet, satin, and guarded skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSquareToedShoes() => new()
	{
		Metadata = new("renaissance_western_square_toed_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of $colour broad square-toed shoes",
		FullDescription = "A pair of $colour broad square-toed shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 690,
		Cost = 44.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: plain, slashed, velvet-covered, and court skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternStandingRuff() => new()
	{
		Metadata = new("renaissance_western_standing_ruff", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "ruff",
		ShortDescription = "a $colour stiff standing ruff",
		FullDescription = "A $colour stiff standing ruff is cut and sewn from linen so that the outward silhouette of the ruff remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the ruff carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 170,
		Cost = 8.8m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Partlet", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: small, medium, and cartwheel skins only if coverage remains compatible; late-century gate",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternStiffenedPairOfBodies() => new()
	{
		Metadata = new("renaissance_western_stiffened_pair_of_bodies", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "bodies",
		ShortDescription = "a $colour stiffened fitted bodies",
		FullDescription = "A $colour stiffened fitted bodies is cut and sewn from canvas so that the outward silhouette of the bodies remains clear. The canvas falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the bodies bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 16.2m,
		Skinnable = true,
		Material = "canvas",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: late-sixteenth-century gate; canvas, satin-covered, and court skins; no modern corset terminology publicly",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternStomacheredBodice() => new()
	{
		Metadata = new("renaissance_western_stomachered_bodice", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "bodice",
		ShortDescription = "a $colour stomacher-fronted bodice",
		FullDescription = "A $colour stomacher-fronted bodice is cut and sewn from broadcloth so that the outward silhouette of the bodice remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the bodice bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: separate-looking front panel is integral to this prototype; rich textile skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternSugarloafFelthat() => new()
	{
		Metadata = new("renaissance_western_sugarloaf_felthat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a $colour sugarloaf-crowned felt hat",
		FullDescription = "A $colour sugarloaf-crowned felt hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 12.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Late-sixteenth-century Western and Central European town, traveller, religious-dissenter precursor, and colonial-contact admission.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternTrainedGown() => new()
	{
		Metadata = new("renaissance_western_trained_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a $colour long-trained formal gown",
		FullDescription = "A $colour long-trained formal gown is cut and sewn from velvet so that the outward silhouette of the gown remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the gown its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 980,
		Cost = 140.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Dress", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: court, bridal, civic-pageant, and performance contexts; train is descriptive",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternTrunkHose() => new()
	{
		Metadata = new("renaissance_western_trunk_hose", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "hose",
		ShortDescription = "a $colour rounded trunk hose",
		FullDescription = "A $colour rounded trunk hose is cut and sewn from broadcloth so that the outward silhouette of the hose remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the hose, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Breeches", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: modest/full volume; paning requires separate prototype below",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternVenetianBreeches() => new()
	{
		Metadata = new("renaissance_western_venetian_breeches", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "breeches",
		ShortDescription = "a $colour loose knee-length breeches",
		FullDescription = "A $colour loose knee-length breeches is cut and sewn from broadcloth so that the outward silhouette of the breeches remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the breeches, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620,
		Cost = 36.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Breeches", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: full but unpaned; urban, court, and maritime-contact skins",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternWheelFarthingale() => new()
	{
		Metadata = new("renaissance_western_wheel_farthingale", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "support",
		ShortDescription = "a $colour wheel-shaped skirt support",
		FullDescription = "A $colour wheel-shaped skirt support is cut and sewn from canvas so that the outward silhouette of the support remains clear. The canvas falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. A reinforced waist carries the support, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900,
		Cost = 21.6m,
		Skinnable = true,
		Material = "canvas",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Skirt_Support", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: late-sixteenth-century elite gate; not ordinary workwear",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternWiredLinenCoif() => new()
	{
		Metadata = new("renaissance_western_wired_linen_coif", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "coif",
		ShortDescription = "a $colour wired-front linen coif",
		FullDescription = "A $colour wired-front linen coif is cut and sewn from linen so that the outward silhouette of the coif remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the coif around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 11.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: Structured female head cap with wired or stiffened front edge; date, region, and veil combination require local admission.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWesternWorkSmock() => new()
	{
		Metadata = new("renaissance_western_work_smock", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceClothingItemSpecs" , []),
		Noun = "smock",
		ShortDescription = "a $colour loose work smock",
		FullDescription = "A $colour loose work smock is cut and sewn from linen so that the outward silhouette of the smock remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the smock its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 33.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "Inferred Renaissance outfit-manifest dependency. Catalogue admission: knee/calf length; reinforced or plain; field, workshop, laundry, and market work",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAfricanatlanticClothingFullHeadwrap() => new()
	{
		Metadata = new("earlymodern_africanatlantic_clothing_full_headwrap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headwrap",
		ShortDescription = "a full $colour cotton headwrap",
		FullDescription = "A full $colour cotton headwrap is cut and sewn from cotton so that the outward silhouette of the headwrap remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the headwrap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220,
		Cost = 15.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 10 outfit(s). Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAfricanatlanticClothingShortCottonBlouse() => new()
	{
		Metadata = new("earlymodern_africanatlantic_clothing_short_cotton_blouse", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "blouse",
		ShortDescription = "a short $colour cotton blouse",
		FullDescription = "A short $colour cotton blouse is cut and sewn from cotton so that the outward silhouette of the blouse remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the blouse bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300,
		Cost = 15.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shirt", "Armour_LightClothing", "Insulation_Minor", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern prototype; used in 7 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanMilitaryCommandSash() => new()
	{
		Metadata = new("earlymodern_andean_military_command_sash", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sash",
		ShortDescription = "a broad $colour camelid-wool command sash",
		FullDescription = "A broad $colour camelid-wool command sash is cut and sewn from camelid wool so that the outward silhouette of the sash remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the sash carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360,
		Cost = 48.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sash", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour", "Belt_6"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanMilitaryServiceTunic() => new()
	{
		Metadata = new("earlymodern_andean_military_service_tunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a long $colour camelid-wool service tunic",
		FullDescription = "A long $colour camelid-wool service tunic is cut and sewn from camelid wool so that the outward silhouette of the tunic remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the tunic bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780,
		Cost = 50.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long-Sleeved_Tunic", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 5 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanMilitaryShortMilitiaCoat() => new()
	{
		Metadata = new("earlymodern_andean_military_short_militia_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a short $colour wool militia coat",
		FullDescription = "A short $colour wool militia coat is cut and sewn from wool so that the outward silhouette of the coat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1000,
		Cost = 70.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_andean_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headband",
		ShortDescription = "a finely woven $colour royal headband",
		FullDescription = "A finely woven $colour royal headband is cut and sewn from wool so that the outward silhouette of the headband remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the headband carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 298,
		Cost = 1108.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Headband", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Andean colonial and Indigenous; headwear. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_andean_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a wrapped full-length $colour wool court gown",
		FullDescription = "A wrapped full-length $colour wool court gown is cut and sewn from wool so that the outward silhouette of the gown remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1358,
		Cost = 2270.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Andean colonial and Indigenous; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_andean_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a tapestry-woven $colour formal wrap-gown",
		FullDescription = "A tapestry-woven $colour formal wrap-gown is cut and sewn from wool so that the outward silhouette of the gown remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1358,
		Cost = 4020.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Andean colonial and Indigenous; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_andean_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shawl",
		ShortDescription = "a broad fringed $colour wool travelling shawl",
		FullDescription = "A broad fringed $colour wool travelling shawl is cut and sewn from wool so that the outward silhouette of the shawl remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the shawl its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1522,
		Cost = 1780.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Andean colonial and Indigenous; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanNobleJewellery() => new()
	{
		Metadata = new("earlymodern_andean_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "necklace",
		ShortDescription = "a broad chased-gold pectoral necklace",
		FullDescription = "A broad chased-gold pectoral necklace is worked and joined from gold so that the outward silhouette of the necklace remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the necklace carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 192,
		Cost = 3730.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Necklaces", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Necklace"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for Andean colonial and Indigenous. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_andean_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "a fine tapestry-woven $colour wool court tunic",
		FullDescription = "A fine tapestry-woven $colour wool court tunic is cut and sewn from wool so that the outward silhouette of the tunic remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the tunic its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1088,
		Cost = 1952.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Robe", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Andean colonial and Indigenous; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernAndeanNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_andean_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a broad tapestry-woven $colour ceremonial mantle",
		FullDescription = "A broad tapestry-woven $colour ceremonial mantle is cut and sewn from wool so that the outward silhouette of the mantle remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1288,
		Cost = 3790.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for Andean colonial and Indigenous; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
