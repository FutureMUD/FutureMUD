#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources02Stock
{
	public static CatalogueItemDefinition MedievalBuddhistTravellingMantle() => new()
	{
		Metadata = new("medieval_buddhist_travelling_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a dark wool monastic travelling mantle",
		FullDescription = "A dark wool monastic travelling mantle is cut and sewn from wool so that the outward silhouette of the mantle remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760,
		Cost = 24.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Cloak_(Open)", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalDaoistCloudSleevedRobe() => new()
	{
		Metadata = new("medieval_daoist_cloud_sleeved_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a fine $colour cloud-sleeved robe",
		FullDescription = "This fine $colour cloud-sleeved robe is a silk ritual garment with broad sleeves and a loose cross-collar body. The sleeves spread widely from the arm, making the $colour silk fall in soft ceremonial folds. Its shape is more expansive than an ordinary robe, giving the garment a deliberate temple-formal presence.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 680,
		Cost = 130.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Variable_FineColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalDaoistCrossCollarRobe() => new()
	{
		Metadata = new("medieval_daoist_cross_collar_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a $colour cross-collared cotton robe",
		FullDescription = "The front panels of this $colour cotton robe overlap beneath a long cross-collar. Ties hold the wrapped closure against a straight, loosely cut body. Long sleeves retain a plain outline, without the broad extensions of a cloud-sleeved overrobe. The cotton is smooth and closely woven, and narrow seams and hems provide its only visible finishing.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520,
		Cost = 24.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Robe_Layer_0_5_NonBulky", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalDaoistRitualCap() => new()
	{
		Metadata = new("medieval_daoist_ritual_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a $colour cloth ritual cap",
		FullDescription = "Stiffened panels of $colour cotton give this cap a compact, angular outline. Small seams join the panels without rounding away their distinct edges. The lower opening is neatly finished and sits beneath a crown that holds its shape without a projecting brim. Its surface is plain, with the close weave and slight changes of plane visible instead of embroidered ornament.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90,
		Cost = 12.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Hat", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternBlackKlobuk() => new()
	{
		Metadata = new("medieval_eastern_black_klobuk", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "klobuk",
		ShortDescription = "a black veiled klobuk",
		FullDescription = "A black veiled klobuk is cut and sewn from wool so that the outward silhouette of the klobuk remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the klobuk around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240,
		Cost = 18.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Wear_Hat", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternBlackRiassa() => new()
	{
		Metadata = new("medieval_eastern_black_riassa", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "riassa",
		ShortDescription = "a $colour wool riassa",
		FullDescription = "This riassa is cut from ample lengths of $colour wool, with a loose body and broad sleeves. The cloth falls in long folds from the shoulders to the lower hem. Plain seams and narrow edge finishing leave the wool's soft, matte surface exposed. Neither the sleeve ends nor the front edges carry contrasting braid, making the broad outline of the robe its principal feature.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850,
		Cost = 30.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternEpimanikia() => new()
	{
		Metadata = new("medieval_eastern_epimanikia", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "epimanikia",
		ShortDescription = "a pair of $colour silk epimanikia",
		FullDescription = "These $colour silk epimanikia are fitted cuffs made to wrap around the lower sleeves. The silk panels are small but carefully shaped, with ties or narrow bands visible at the inner wrist. Their $colour surface gives a bright, formal finish to the hands and forearms.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 80,
		Cost = 50.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Gloves"],
		Components = ["Holdable", "Wear_Bracers", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Variable_FineColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternEpitrachelion() => new()
	{
		Metadata = new("medieval_eastern_epitrachelion", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "epitrachelion",
		ShortDescription = "a $colour silk epitrachelion",
		FullDescription = "This $colour silk epitrachelion is a paired stole joined at the neck and falling in two long panels down the front. The $colour silk has a formal sheen, broken by the fold where the fabric sits around the collar. Its length, symmetry, and careful edging make it distinct from an ordinary scarf.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 190,
		Cost = 70.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Scarf", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Variable_FineColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternKamilavkion() => new()
	{
		Metadata = new("medieval_eastern_kamilavkion", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kamilavkion",
		ShortDescription = "a $colour felt kamilavkion",
		FullDescription = "Dense $colour felt gives this kamilavkion its stiff cylindrical shape. Straight sides rise from the lower rim to a flat, neatly bounded crown. The surface is matte and closely packed, with little loose fibre projecting from it. A plain finished edge runs around the opening, and there is no brim, tassel or attached veil to break the upright outline.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 170,
		Cost = 20.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Hat", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternMonasticMantle() => new()
	{
		Metadata = new("medieval_eastern_monastic_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a black wool monastic mantle",
		FullDescription = "A black wool monastic mantle is cut and sewn from wool so that the outward silhouette of the mantle remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950,
		Cost = 34.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Mantle", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternOmophorion() => new()
	{
		Metadata = new("medieval_eastern_omophorion", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "omophorion",
		ShortDescription = "a fine white silk omophorion",
		FullDescription = "A fine white silk omophorion is cut and sewn from silk so that the outward silhouette of the omophorion remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the omophorion carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 220,
		Cost = 120.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Scarf", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternOrarion() => new()
	{
		Metadata = new("medieval_eastern_orarion", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "orarion",
		ShortDescription = "a $colour silk orarion",
		FullDescription = "A $colour silk orarion is cut and sewn from silk so that the outward silhouette of the orarion remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the orarion carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 130,
		Cost = 60.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Scarf", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Variable_FineColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternPhelonion() => new()
	{
		Metadata = new("medieval_eastern_phelonion", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "phelonion",
		ShortDescription = "a fine $colour silk phelonion",
		FullDescription = "This fine $colour silk phelonion is a wide sleeveless outer vestment with a rounded fall from the shoulders. The front is cut to clear the hands while the back drops in a fuller sweep, showing the $colour silk in deep folds. Its form resembles a ceremonial mantle, but the head opening and continuous body give it a distinctive liturgical shape.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 780,
		Cost = 170.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Poncho", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Variable_FineColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternSakkos() => new()
	{
		Metadata = new("medieval_eastern_sakkos", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sakkos",
		ShortDescription = "a fine $colour silk sakkos",
		FullDescription = "A fine $colour silk sakkos is cut and sewn from silk so that the outward silhouette of the sakkos remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the sakkos its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 800,
		Cost = 210.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing", "Variable_FineColour"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternSticharion() => new()
	{
		Metadata = new("medieval_eastern_sticharion", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sticharion",
		ShortDescription = "a $colour linen sticharion",
		FullDescription = "Long panels of $colour linen give this sticharion a straight, nearly floor-length fall. Close sleeves extend from a simply finished shoulder line. Fine, even seams lie flat against the cloth rather than forming raised decorative bands. The lower hem is carefully turned, and the lightly creased linen shows an unbroken surface without embroidery or contrasting trim.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 480,
		Cost = 40.0m,
		Skinnable = true,
		Material = "linen",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Robe_Layer_0_5_NonBulky", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalEasternWomensMonasticVeil() => new()
	{
		Metadata = new("medieval_eastern_womens_monastic_veil", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "veil",
		ShortDescription = "a black monastic veil",
		FullDescription = "A black monastic veil is cut and sewn from wool so that the outward silhouette of the veil remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the veil around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 200,
		Cost = 12.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Wear_Veil", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalHinduKaupina() => new()
	{
		Metadata = new("medieval_hindu_kaupina", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "kaupina",
		ShortDescription = "a plain $colour cotton kaupina",
		FullDescription = "This kaupina consists of a narrow length of light $colour cotton. Its centre broadens enough to form a small covering panel, while the remaining cloth folds into a compact loincloth. Fine hems finish the exposed edges without adding bulky reinforcement. There is no embroidery or contrasting border, and the plain weave is visible across the unbroken centre.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90,
		Cost = 4.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Loincloth", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalHinduOchreAsceticWrap() => new()
	{
		Metadata = new("medieval_hindu_ochre_ascetic_wrap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "wrap",
		ShortDescription = "an ochre cotton ascetic wrap",
		FullDescription = "An ochre cotton ascetic wrap is cut and sewn from cotton so that the outward silhouette of the wrap remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the wrap its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 14.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalHinduOchreShoulderCloth() => new()
	{
		Metadata = new("medieval_hindu_ochre_shoulder_cloth", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloth",
		ShortDescription = "an ochre cotton shoulder cloth",
		FullDescription = "An ochre cotton shoulder cloth is cut and sewn from cotton so that the outward silhouette of the cloth remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloth its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160,
		Cost = 8.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Mantle", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalHinduWhitePriestDhoti() => new()
	{
		Metadata = new("medieval_hindu_white_priest_dhoti", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "dhoti",
		ShortDescription = "a $colour cotton dhoti",
		FullDescription = "A long rectangle of $colour cotton forms this dhoti, with firm edges and a narrow finished hem at either end. The cloth is light enough to gather into closely stacked folds. Its length allows a section to pass between the legs while the remaining breadth forms the lower drape. There are no sewn leg tubes, buttons or fitted waistband to interrupt the continuous fabric.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360,
		Cost = 16.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Loincloth", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalHinduWhiteUttariya() => new()
	{
		Metadata = new("medieval_hindu_white_uttariya", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "uttariya",
		ShortDescription = "a white cotton uttariya",
		FullDescription = "A white cotton uttariya is cut and sewn from cotton so that the outward silhouette of the uttariya remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the uttariya its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 10.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Mantle", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalIslamicPlainImamQamis() => new()
	{
		Metadata = new("medieval_islamic_plain_imam_qamis", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "qamis",
		ShortDescription = "a plain $colour cotton qamis",
		FullDescription = "This qamis has a long, straight body cut from light $colour cotton. Its sleeves follow the arms without the deep hanging folds of a wide-sleeved robe. The front is unornamented, and narrow seams join the cloth with little raised bulk. An even hem finishes the lower edge, beneath broad panels whose close weave remains opaque despite their lightness.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 18.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalIslamicPlainImamTurban() => new()
	{
		Metadata = new("medieval_islamic_plain_imam_turban", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "turban",
		ShortDescription = "a plain white cotton imam turban",
		FullDescription = "A plain white cotton imam turban is cut and sewn from cotton so that the outward silhouette of the turban remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the turban around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260,
		Cost = 14.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Wear_Turban", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalIslamicScholarsJubba() => new()
	{
		Metadata = new("medieval_islamic_scholars_jubba", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jubba",
		ShortDescription = "a dark wool scholar's jubba",
		FullDescription = "A dark wool scholar's jubba is cut and sewn from wool so that the outward silhouette of the jubba remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the jubba its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 850,
		Cost = 48.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalJainWhiteAsceticRobe() => new()
	{
		Metadata = new("medieval_jain_white_ascetic_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a plain $colour cotton robe",
		FullDescription = "Light $colour cotton forms the broad, loosely wrapped body of this robe. The fabric falls in long folds rather than following a fitted waist. Narrow hems secure the edges, and the exposed cloth has a plain, close weave without embroidery. Its simple construction leaves the overlap and drape visible without added braid, metallic trim or a contrasting border.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 16.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalJainWhiteShoulderWrap() => new()
	{
		Metadata = new("medieval_jain_white_shoulder_wrap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "wrap",
		ShortDescription = "a $colour cotton shoulder wrap",
		FullDescription = "This shoulder wrap is a light rectangle of $colour cotton, broad through the middle and straight along its sides. Narrow hems enclose the cut edges with small, even stitches. The plain weave remains visible across the undecorated surface. Its soft folds gather readily over the shoulders, with no sleeves, clasps or fitted collar to hold a prescribed shape.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160,
		Cost = 8.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Mantle", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalJewishScholarsRobe() => new()
	{
		Metadata = new("medieval_jewish_scholars_robe", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "robe",
		ShortDescription = "a dark wool scholar's robe",
		FullDescription = "A dark wool scholar's robe is cut and sewn from wool so that the outward silhouette of the robe remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the robe its recognisable proportion when worn. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820,
		Cost = 44.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalJewishSkullcap() => new()
	{
		Metadata = new("medieval_jewish_skullcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "skullcap",
		ShortDescription = "a $colour wool skullcap",
		FullDescription = "Shallow seams divide the $colour wool of this skullcap into curved panels. They meet at the crown and spread towards a narrow, evenly finished rim. The soft fabric forms a close, rounded cup with no brim or hanging flap. Small stitches secure the turned edge, leaving the outside untrimmed and the wool's fine nap visible between the seams.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60,
		Cost = 6.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Skullcap", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalJewishTallitGadol() => new()
	{
		Metadata = new("medieval_jewish_tallit_gadol", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tallit",
		ShortDescription = "a $colour1 wool tallit gadol",
		FullDescription = "A broad rectangle of $colour1 wool forms this tallit, with narrow $colour2 stripes woven across its ends. The light cloth gathers into soft folds without losing the straight line of its edges. Long, knotted fringes hang separately from the four corners. Shorter threads finish the ends, leaving the long sides plain and the striped bands clearly visible.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 420,
		Cost = 40.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_2Colour", "Wear_Mantle", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalJewishTallitKatan() => new()
	{
		Metadata = new("medieval_jewish_tallit_katan", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "tallit",
		ShortDescription = "a $colour wool tallit katan",
		FullDescription = "Two straight panels of $colour wool hang from the shoulders of this small tallit. A simple opening at the centre admits the head, while the open sides leave four distinct corners. Each corner carries a long knotted fringe below the neatly finished edge. The lightly woven body is otherwise plain, with little bulk to interrupt the fall of another garment over it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230,
		Cost = 22.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Variable_Colour", "Wear_Vest", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinBishopGloves() => new()
	{
		Metadata = new("medieval_latin_bishop_gloves", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gloves",
		ShortDescription = "a pair of fine white liturgical gloves",
		FullDescription = "A pair of fine white liturgical gloves is cut and sewn from silk so that the outward silhouette of the gloves remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the gloves bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 90,
		Cost = 80.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Gloves"],
		Components = ["Holdable", "Wear_Gloves", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinBishopMitre() => new()
	{
		Metadata = new("medieval_latin_bishop_mitre", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mitre",
		ShortDescription = "a fine white silk mitre",
		FullDescription = "A fine white silk mitre is cut and sewn from silk so that the outward silhouette of the mitre remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the mitre around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 160,
		Cost = 140.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Market / Clothing / Luxury Clothing", "Functions / Worn Items / Headwear"],
		Components = ["Holdable", "Wear_Hat", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinBlackCowl() => new()
	{
		Metadata = new("medieval_latin_black_cowl", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cowl",
		ShortDescription = "a black wool cowl",
		FullDescription = "A black wool cowl is cut and sewn from wool so that the outward silhouette of the cowl remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cowl its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650,
		Cost = 18.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Cloak_(Closed)", "Destroyable_Clothing", "Insulation_Strong", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinBlackFriarCappa() => new()
	{
		Metadata = new("medieval_latin_black_friar_cappa", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cappa",
		ShortDescription = "a black wool friar's cappa",
		FullDescription = "A black wool friar's cappa is cut and sewn from wool so that the outward silhouette of the cappa remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cappa its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720,
		Cost = 24.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Cloak_(Open)", "Destroyable_Clothing", "Insulation_Moderate", "Armour_HeavyClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition MedievalLatinBlackMonasticHabit() => new()
	{
		Metadata = new("medieval_latin_black_monastic_habit", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "habit",
		ShortDescription = "a black wool monastic habit",
		FullDescription = "A black wool monastic habit is cut and sewn from wool so that the outward silhouette of the habit remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the habit its recognisable proportion when worn. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900,
		Cost = 24.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Robe", "Destroyable_Clothing", "Insulation_Moderate", "Armour_LightClothing"],
		BuilderNotes = "Early Modern admission of an exact Medieval clothing definition.",
		AllowLegacyShortDescriptionMatch = false,
	};
}
