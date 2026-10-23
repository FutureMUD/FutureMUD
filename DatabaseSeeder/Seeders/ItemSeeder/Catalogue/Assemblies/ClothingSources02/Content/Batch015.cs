#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources02Stock
{
	public static CatalogueItemDefinition MedievalLatinBlackOverScapular() => new()
	{
		Metadata = new("medieval_latin_black_over_scapular", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "scapular",
		ShortDescription = "a black wool over-scapular",
		FullDescription = "A black wool over-scapular is cut and sewn from wool so that the outward silhouette of the scapular remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the scapular its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280,
		Cost = 8.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Tabard", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinBlackScapular() => new()
	{
		Metadata = new("medieval_latin_black_scapular", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "scapular",
		ShortDescription = "a black wool scapular",
		FullDescription = "A black wool scapular is cut and sewn from wool so that the outward silhouette of the scapular remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the scapular its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260,
		Cost = 8.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Tabard", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinBrownFriarHabit() => new()
	{
		Metadata = new("medieval_latin_brown_friar_habit", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "habit",
		ShortDescription = "a brown wool friar's habit",
		FullDescription = "A brown wool friar's habit is cut and sewn from wool so that the outward silhouette of the habit remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the habit its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 800,
		Cost = 22.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinDalmaticVestment() => new()
	{
		Metadata = new("medieval_latin_dalmatic_vestment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "dalmatic",
		ShortDescription = "a fine $colour silk dalmatic",
		FullDescription = "This fine $colour silk dalmatic is a loose, straight vestment with broad sleeves and a squared body. The $colour silk hangs smoothly from the shoulders, leaving the sleeve openings wide and visible. Its formal cut gives it a robe-like dignity while keeping the shape distinct from a closed cloak or mantle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 650,
		Cost = 140.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Variable_FineColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinFriarGreyHabit() => new()
	{
		Metadata = new("medieval_latin_friar_grey_habit", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "habit",
		ShortDescription = "a grey wool friar's habit",
		FullDescription = "A grey wool friar's habit is cut and sewn from wool so that the outward silhouette of the habit remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the habit its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850,
		Cost = 20.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinLinenCincture() => new()
	{
		Metadata = new("medieval_latin_linen_cincture", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cincture",
		ShortDescription = "a $colour linen cincture",
		FullDescription = "Several strands of $colour linen are worked together into this narrow cincture. The braided surface is firm along its length but supple enough to bend into a close knot. Simple finished ends hang below the tied portion without metal tips or separate fittings. Its even width and shallow ridges give the cord a distinct texture against a smooth robe.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80,
		Cost = 6.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Belts"],
		Components = ["Holdable", "Variable_Colour", "Wear_Waist_Layer_0_1", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinManiple() => new()
	{
		Metadata = new("medieval_latin_maniple", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "maniple",
		ShortDescription = "a $colour silk maniple",
		FullDescription = "This $colour silk maniple is a short band of fine cloth made to hang from the forearm. The silk is light and smooth, with the $colour shade most visible across the folded central panel. Its ends are carefully squared, giving the small vestment a deliberate and formal outline.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 60,
		Cost = 30.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Bracer", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Variable_FineColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinNunWimpleVeil() => new()
	{
		Metadata = new("medieval_latin_nun_wimple_veil", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "veil",
		ShortDescription = "a white linen wimple and veil",
		FullDescription = "A white linen wimple and veil is cut and sewn from linen so that the outward silhouette of the veil remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the veil around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220,
		Cost = 12.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Wear_Veil", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinPoorClareGreyHabit() => new()
	{
		Metadata = new("medieval_latin_poor_clare_grey_habit", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "habit",
		ShortDescription = "a grey wool sister's habit",
		FullDescription = "A grey wool sister's habit is cut and sewn from wool so that the outward silhouette of the habit remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the habit its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760,
		Cost = 18.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinPoorClareVeil() => new()
	{
		Metadata = new("medieval_latin_poor_clare_veil", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "veil",
		ShortDescription = "a plain linen sister's veil",
		FullDescription = "A plain linen sister's veil is cut and sewn from linen so that the outward silhouette of the veil remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the veil around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 8.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Wear_Veil", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinRopeCincture() => new()
	{
		Metadata = new("medieval_latin_rope_cincture", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cincture",
		ShortDescription = "a knotted rope cincture",
		FullDescription = "A knotted rope cincture is cut and sewn from hemp so that the outward silhouette of the cincture remains clear. The hemp falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the cincture carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160,
		Cost = 4.0m,
		Skinnable = true,
		Material = "hemp",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Belts"],
		Components = ["Holdable", "Wear_Sash", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinSimpleNunHabit() => new()
	{
		Metadata = new("medieval_latin_simple_nun_habit", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "habit",
		ShortDescription = "a dark wool nun's habit",
		FullDescription = "A dark wool nun's habit is cut and sewn from wool so that the outward silhouette of the habit remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the habit its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 820,
		Cost = 22.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinStole() => new()
	{
		Metadata = new("medieval_latin_stole", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "stole",
		ShortDescription = "a $colour silk stole",
		FullDescription = "This $colour silk stole is a long, narrow band of cloth with neatly weighted ends and a smooth lustre. The silk catches light along its length, making the $colour tone appear richer where the fabric bends. Its construction is formal and restrained, with straight edges and a clearly ceremonial fall.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 110,
		Cost = 50.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Scarf", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Variable_FineColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteAlb() => new()
	{
		Metadata = new("medieval_latin_white_alb", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "alb",
		ShortDescription = "a white linen alb",
		FullDescription = "A white linen alb is cut and sewn from linen so that the outward silhouette of the alb remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the alb its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 460,
		Cost = 36.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteCarthusianHabit() => new()
	{
		Metadata = new("medieval_latin_white_carthusian_habit", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "habit",
		ShortDescription = "a white wool hermit's habit",
		FullDescription = "A white wool hermit's habit is cut and sewn from wool so that the outward silhouette of the habit remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the habit its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950,
		Cost = 30.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteFriarHabit() => new()
	{
		Metadata = new("medieval_latin_white_friar_habit", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "habit",
		ShortDescription = "a white wool friar's habit",
		FullDescription = "A white wool friar's habit is cut and sewn from wool so that the outward silhouette of the habit remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the habit its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780,
		Cost = 28.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteFriarMantle() => new()
	{
		Metadata = new("medieval_latin_white_friar_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a white wool friar's mantle",
		FullDescription = "A white wool friar's mantle is cut and sewn from wool so that the outward silhouette of the mantle remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 640,
		Cost = 28.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Mantle", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteMonasticTunic() => new()
	{
		Metadata = new("medieval_latin_white_monastic_tunic", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tunic",
		ShortDescription = "an undyed wool monastic tunic",
		FullDescription = "An undyed wool monastic tunic is cut and sewn from wool so that the outward silhouette of the tunic remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the tunic its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 820,
		Cost = 24.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalShintoMikoRedHakama() => new()
	{
		Metadata = new("medieval_shinto_miko_red_hakama", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hakama",
		ShortDescription = "a pair of pleated $colour hemp hakama",
		FullDescription = "Long pleats shape the $colour hemp of these broad-legged hakama. They open below the waist into generous folds while the separate legs retain wide, evenly hemmed openings. Cloth ties extend from the upper edge for wrapping and fastening. The panels are undecorated, leaving the crisp fold lines and the slight texture of the hemp visible across their breadth.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 460,
		Cost = 28.0m,
		Skinnable = true,
		Material = "hemp",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Legwear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Trousers", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalShintoMikoWhiteKosode() => new()
	{
		Metadata = new("medieval_shinto_miko_white_kosode", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kosode",
		ShortDescription = "a $colour hemp kosode",
		FullDescription = "This $colour hemp kosode has a straight body and a neatly overlapping front. Its sleeves end in small openings, leaving the surrounding cloth to gather in shallow folds. Narrow seams join the panels, and a continuous collar band follows the wrapped neckline. The hemp is plain and carefully finished, without an attached sash or a contrasting embroidered border.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 24.0m,
		Skinnable = true,
		Material = "hemp",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalShintoPriestEboshi() => new()
	{
		Metadata = new("medieval_shinto_priest_eboshi", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "eboshi",
		ShortDescription = "a $colour lacquered eboshi",
		FullDescription = "This tall headpiece has a folded silk body stiffened beneath a smooth $colour lacquer surface. Its narrow crown rises upright above a close-fitting lower band, with crisp changes of angle along the folded sides. The coating follows the underlying creases without hiding their sharp outline. A fine seam runs down the back, and the plain outer faces catch small highlights along their raised edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 110,
		Cost = 38.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Wear_Hat", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing", "Variable_LacquerColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalShintoPriestHakama() => new()
	{
		Metadata = new("medieval_shinto_priest_hakama", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hakama",
		ShortDescription = "a pair of $colour hemp hakama",
		FullDescription = "These hakama have broad legs cut from crisp $colour hemp, with deep pleats descending from the waist. The folds give them a full, almost skirt-like outline while retaining separate leg openings. Long cloth ties extend from the upper edge to secure the garment. Evenly turned hems finish the broad lower openings, and the fabric carries no applied embroidery or contrasting trim.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 480,
		Cost = 34.0m,
		Skinnable = true,
		Material = "hemp",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Legwear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Trousers", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalShintoWhiteJoeRobe() => new()
	{
		Metadata = new("medieval_shinto_white_joe_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jōe",
		ShortDescription = "a $colour hemp jōe robe",
		FullDescription = "Broad sleeves spread from the straight body of this $colour hemp jōe. The crisp cloth holds distinct folds along the shoulders and the wrapped front. Carefully finished edges follow the collar, sleeve openings and lower hem without contrasting decoration. The hemp's fine, slightly uneven fibres remain visible across the otherwise plain surface.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 520,
		Cost = 42.0m,
		Skinnable = true,
		Material = "hemp",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalSteppeRitualFeltCoat() => new()
	{
		Metadata = new("medieval_steppe_ritual_felt_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a felt-lined ritual coat",
		FullDescription = "A felt-lined ritual coat is cut and sewn from felt so that the outward silhouette of the coat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100,
		Cost = 34.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Long_Coat", "Destroyable_Clothing", "Insulation_Strong", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalSteppeRitualFurCap() => new()
	{
		Metadata = new("medieval_steppe_ritual_fur_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a fur-trimmed ritual cap",
		FullDescription = "A fur-trimmed ritual cap is cut and stitched from fur so that the outward silhouette of the cap remains clear. The fur shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220,
		Cost = 24.0m,
		Skinnable = true,
		Material = "fur",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Wear_Hat", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalSteppeRitualSash() => new()
	{
		Metadata = new("medieval_steppe_ritual_sash", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sash",
		ShortDescription = "a braided wool ritual sash",
		FullDescription = "A braided wool ritual sash is cut and sewn from wool so that the outward silhouette of the sash remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the sash carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190,
		Cost = 12.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Belts"],
		Components = ["Holdable", "Wear_Sash", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Belt_6"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalSufiCoarseWoolCloak() => new()
	{
		Metadata = new("medieval_sufi_coarse_wool_cloak", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a coarse wool Sufi cloak",
		FullDescription = "A coarse wool Sufi cloak is cut and sewn from wool so that the outward silhouette of the cloak remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980,
		Cost = 22.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Cloak_(Open)", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalSufiFeltCap() => new()
	{
		Metadata = new("medieval_sufi_felt_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a plain felt dervish cap",
		FullDescription = "A plain felt dervish cap is cut and sewn from felt so that the outward silhouette of the cap remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140,
		Cost = 10.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Wear_Hat", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalSufiPatchedKhirqa() => new()
	{
		Metadata = new("medieval_sufi_patched_khirqa", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "khirqa",
		ShortDescription = "a patched wool khirqa",
		FullDescription = "A patched wool khirqa is cut and sewn from wool so that the outward silhouette of the khirqa remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the khirqa its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780,
		Cost = 18.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalSufiWoolSash() => new()
	{
		Metadata = new("medieval_sufi_wool_sash", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sash",
		ShortDescription = "a coarse wool Sufi sash",
		FullDescription = "A coarse wool Sufi sash is cut and sewn from wool so that the outward silhouette of the sash remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the sash carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 6.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Belts"],
		Components = ["Holdable", "Wear_Sash", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Belt_6"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialClothingIronBuckledLeatherBelt() => new()
	{
		Metadata = new("preindustrial_clothing_iron_buckled_leather_belt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "belt",
		ShortDescription = "an iron-buckled leather belt",
		FullDescription = "An iron-buckled leather belt is cut and stitched from leather so that the outward silhouette of the belt remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the belt carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 240,
		Cost = 30.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Pre-Industrial Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Waist", "Armour_LightClothing", "Insulation_Minor", "Belt_4"],
		BuilderNotes = "Implemented pre-industrial alias; used in 51 outfit(s). Implemented shared pre-industrial dependency. Second-pass military admission; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialClothingPlainLeatherBelt() => new()
	{
		Metadata = new("preindustrial_clothing_plain_leather_belt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "belt",
		ShortDescription = "a plain leather belt",
		FullDescription = "A plain leather belt is cut and stitched from leather so that the outward silhouette of the belt remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the belt carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 10.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Pre-Industrial Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Waist", "Armour_LightClothing", "Insulation_Minor", "Belt_2"],
		BuilderNotes = "Implemented pre-industrial alias; used in 19 outfit(s). Implemented shared pre-industrial dependency. Second-pass military admission; used in 3 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialClothingSimpleWovenSash() => new()
	{
		Metadata = new("preindustrial_clothing_simple_woven_sash", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sash",
		ShortDescription = "a $colour woven sash",
		FullDescription = "A $colour woven sash is cut and sewn from wool so that the outward silhouette of the sash remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the sash carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120,
		Cost = 6.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Pre-Industrial Era", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sash", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour", "Belt_6"],
		BuilderNotes = "Implemented pre-industrial alias; used in 96 outfit(s). Implemented shared pre-industrial dependency.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialMilitarySupportSimpleSwordBelt() => new()
	{
		Metadata = new("preindustrial_military_support_simple_sword_belt", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "belt",
		ShortDescription = "a simple sword belt",
		FullDescription = "A simple sword belt is cut and stitched from leather so that the outward silhouette of the belt remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The contact points and fastenings are kept smooth, while the visible body of the belt carries the shape and surface detail that distinguish it at a glance. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 32.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Pre-Industrial Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods"],
		Components = ["Holdable", "Wear_Waist", "Belt_4", "Destroyable_Clothing"],
		BuilderNotes = "Implemented shared alias; admitted in 63 military outfit(s) rather than cloned as an Early Modern row.",
		AllowLegacyShortDescriptionMatch = false,
	};
}
