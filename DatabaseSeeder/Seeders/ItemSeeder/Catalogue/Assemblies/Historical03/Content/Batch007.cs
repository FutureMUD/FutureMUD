#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical03Stock
{
	public static CatalogueItemDefinition MedievalLinenMarketVeil() => new()
	{
		Metadata = new("medieval_linen_market_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6111", []),
		Noun = "veil",
		ShortDescription = "a plain $colour market veil",
		LongDescription = null,
		FullDescription = "This plain $colour market veil is a simple rectangular cloth for covering the hair and shoulders during public work. The edges are narrow and practical, with no heavy ornament to snag or weigh it down. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBroochedMarketMantle() => new()
	{
		Metadata = new("medieval_brooched_market_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6142", []),
		Noun = "mantle",
		ShortDescription = "a neat $colour brooched mantle",
		LongDescription = null,
		FullDescription = "This neat $colour brooched mantle is a broad outer cloth meant to drape over the shoulders and upper body. The fastening is plainly visible near the shoulder, gathering the cloth into heavy folds without concealing how it is held. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 860.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLeatherTraderBelt() => new()
	{
		Metadata = new("medieval_leather_trader_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6173", []),
		Noun = "belt",
		ShortDescription = "a broad leather trader's belt",
		LongDescription = null,
		FullDescription = "This broad leather trader's belt is cut wider than a common waist strap, with a firm body and a plain fastening. The edges are rubbed smooth and the outer face shows creases from being drawn tight over working clothes. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolMarketHeadcloth() => new()
	{
		Metadata = new("medieval_wool_market_headcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6203", []),
		Noun = "headcloth",
		ShortDescription = "a $colour wool market headcloth",
		LongDescription = null,
		FullDescription = "This $colour wool market headcloth is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 130.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Kerchief",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTownLeine() => new()
	{
		Metadata = new("medieval_town_leine", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6234", []),
		Noun = "léine",
		ShortDescription = "a clean $colour town léine",
		LongDescription = null,
		FullDescription = "This clean $colour town léine is a long linen tunic with full sleeves and a generous body cut from straight panels. The length and loose fall make it useful as both underlayer and visible garment. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWorkWoolCotte() => new()
	{
		Metadata = new("medieval_work_wool_cotte", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6265", []),
		Noun = "cotte",
		ShortDescription = "a plain $colour wool work cotte",
		LongDescription = null,
		FullDescription = "This plain $colour wool work cotte is a long wool body garment cut close enough to layer under a surcoat or mantle. The sleeves are narrow through the forearm, and the skirt has enough fullness for walking. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges. With plain seams and a serviceable fall at the cuffs, neck, and hem.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 540.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTownWorkKirtle() => new()
	{
		Metadata = new("medieval_town_work_kirtle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6296", []),
		Noun = "kirtle",
		ShortDescription = "a tucked $colour work kirtle",
		LongDescription = null,
		FullDescription = "This tucked $colour work kirtle is a long wool garment with a skirt that can be tucked up clear of work. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSleevelessWorkSurcoat() => new()
	{
		Metadata = new("medieval_sleeveless_work_surcoat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6327", []),
		Noun = "surcoat",
		ShortDescription = "a plain $colour sleeveless work surcoat",
		LongDescription = null,
		FullDescription = "This plain $colour sleeveless work surcoat is a sleeveless wool overgarment with open sides and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Sleeveless_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalPlainWoolShopGown() => new()
	{
		Metadata = new("medieval_plain_wool_shop_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6358", []),
		Noun = "gown",
		ShortDescription = "a neat $colour wool shop gown",
		LongDescription = null,
		FullDescription = "This neat $colour wool shop gown is a long wool garment with a tidy fall suitable for counter, desk, or workshop wear. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 760.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalClerkWoolGown() => new()
	{
		Metadata = new("medieval_clerk_wool_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6389", []),
		Noun = "gown",
		ShortDescription = "a plain $colour clerk's gown",
		LongDescription = null,
		FullDescription = "This plain $colour clerk's gown is a long wool garment with a tidy fall suitable for counter, desk, or workshop wear. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 780.0,
		Cost = 56.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNeatWoolTownHood() => new()
	{
		Metadata = new("medieval_neat_wool_town_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6420", []),
		Noun = "hood",
		ShortDescription = "a neat $colour wool town hood",
		LongDescription = null,
		FullDescription = "This neat $colour wool town hood is a wool head covering with a rounded crown and a fall that protects the neck and upper shoulders. The face opening is plainly finished, and the lower edge sits comfortably over other garments. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 240.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMerchantsWoolHood() => new()
	{
		Metadata = new("medieval_merchants_wool_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6451", []),
		Noun = "hood",
		ShortDescription = "a fine $colour merchant's hood",
		LongDescription = null,
		FullDescription = "This fine $colour merchant's hood is a neatly cut wool hood with a controlled face opening and a tidy shoulder fall. The finish is cleaner than rough weather wear, with attention at the edge most visible around the face. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenShopWimple() => new()
	{
		Metadata = new("medieval_linen_shop_wimple", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6482", []),
		Noun = "wimple",
		ShortDescription = "a clean $colour shop wimple",
		LongDescription = null,
		FullDescription = "This clean $colour shop wimple is a linen head and neck covering shaped by smooth folds around the throat and lower face. The fabric is light but close, leaving the outer edge neat enough to sit under a veil or hood. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenBakerCap() => new()
	{
		Metadata = new("medieval_linen_baker_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6513", []),
		Noun = "cap",
		ShortDescription = "a $colour linen baker's cap",
		LongDescription = null,
		FullDescription = "This $colour linen baker's cap is a light work cap with a soft crown and a narrow band to hold it close. The cloth is washable-looking and plain, suited to keeping hair contained around flour and ovens. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 100.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightLinenWorkCamisa() => new()
	{
		Metadata = new("medieval_light_linen_work_camisa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6544", []),
		Noun = "camisa",
		ShortDescription = "a light $colour linen work camisa",
		LongDescription = null,
		FullDescription = "This light $colour linen work camisa is a light linen shirt with straight-cut sleeves, a simple neck slit, and a loose body. The cloth is fine enough for warm weather or under-layering beneath woollen garments. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCottonMarketToca() => new()
	{
		Metadata = new("medieval_cotton_market_toca", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6575", []),
		Noun = "toca",
		ShortDescription = "a $colour cotton market toca",
		LongDescription = null,
		FullDescription = "This $colour cotton market toca is a wrapped headcloth with enough length to cover the hair and frame the sides of the face. The fabric is light and foldable, with the edge kept plain for repeated tying. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolWorkPellote() => new()
	{
		Metadata = new("medieval_wool_work_pellote", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6606", []),
		Noun = "pellote",
		ShortDescription = "a plain $colour wool work pellote",
		LongDescription = null,
		FullDescription = "This plain $colour wool work pellote is a sleeveless wool overgarment with deep side openings and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Sleeveless_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalPlainWorkDalmatic() => new()
	{
		Metadata = new("medieval_plain_work_dalmatic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6637", []),
		Noun = "dalmatic",
		ShortDescription = "a plain $colour work dalmatic",
		LongDescription = null,
		FullDescription = "This plain $colour work dalmatic is a loose robe with broad sleeves, a straight body, and a dignified fall from shoulder to hem. The cut leaves room for an underlayer, while the neck and cuffs are the clearest places for visible finishing. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 740.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShortWorkSagion() => new()
	{
		Metadata = new("medieval_short_work_sagion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6668", []),
		Noun = "sagion",
		ShortDescription = "a short $colour work sagion",
		LongDescription = null,
		FullDescription = "This short $colour work sagion is a compact wool cloak cut to cover the shoulders without hanging too low over the arms. The edges are plain and the fastening area is kept simple for daily town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenShopMaforion() => new()
	{
		Metadata = new("medieval_linen_shop_maforion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6699", []),
		Noun = "maforion",
		ShortDescription = "a plain $colour shop maforion",
		LongDescription = null,
		FullDescription = "This plain $colour shop maforion is a broad head-and-shoulder veil with a long fall around the face and upper body. The cloth is cut generously enough to layer over a robe without losing its clean outline. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 130.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShortCottonWorkQamis() => new()
	{
		Metadata = new("medieval_short_cotton_work_qamis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6730", []),
		Noun = "qamis",
		ShortDescription = "a short $colour cotton work qamis",
		LongDescription = null,
		FullDescription = "This short $colour cotton work qamis is a short, workmanlike shirt-like garment with straight side seams, long sleeves, and a simple neck opening. The cloth hangs freely from the shoulders and can be belted or left loose over trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenWorkSirwal() => new()
	{
		Metadata = new("medieval_linen_work_sirwal", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6761", []),
		Noun = "sirwal",
		ShortDescription = "a pair of $colour linen work sirwal",
		LongDescription = null,
		FullDescription = "These $colour linen work sirwal are loose drawstring trousers with a roomy seat and tapered lower legs. The cloth gathers softly at the waist and falls in practical folds around the thighs. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 330.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenShopTurban() => new()
	{
		Metadata = new("medieval_linen_shop_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6792", []),
		Noun = "turban",
		ShortDescription = "a clean $colour linen shop turban",
		LongDescription = null,
		FullDescription = "This clean $colour linen shop turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 170.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCottonWaistApron() => new()
	{
		Metadata = new("medieval_cotton_waist_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6823", []),
		Noun = "apron",
		ShortDescription = "a $colour cotton waist apron",
		LongDescription = null,
		FullDescription = "This $colour cotton waist apron is a plain protective cloth with a straight fall over the front of the body. The upper edge and ties are simple, keeping the garment easy to put on over ordinary working clothes. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 170.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSleevelessWorkJubba() => new()
	{
		Metadata = new("medieval_sleeveless_work_jubba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6854", []),
		Noun = "jubba",
		ShortDescription = "a sleeveless $colour work jubba",
		LongDescription = null,
		FullDescription = "This sleeveless $colour work jubba is a loose wool overgarment with open armholes and a straight body. It is cut to add a protective layer over a qamis while leaving the arms free for labour. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Sleeveless_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBazaarMerchantJubba() => new()
	{
		Metadata = new("medieval_bazaar_merchant_jubba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6885", []),
		Noun = "jubba",
		ShortDescription = "a neat $colour bazaar jubba",
		LongDescription = null,
		FullDescription = "This neat $colour bazaar jubba is a loose wool robe with long sleeves, a straight body, and a generous front opening. It is cut to layer over lighter garments, with the edges and cuffs given the most visible finish. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalScribeLinenSleeves() => new()
	{
		Metadata = new("medieval_scribe_linen_sleeves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6916", []),
		Noun = "sleeves",
		ShortDescription = "a pair of $colour linen scribe sleeves",
		LongDescription = null,
		FullDescription = "These $colour linen scribe sleeves are separate forearm coverings made to protect robe or tunic sleeves from ink and desk wear. The fabric is light, smooth, and gathered just enough to stay clear of the hands. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Gloves"
			],
		Components = [
				"Holdable",
				"Wear_Bracers",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMarketIzarWrap() => new()
	{
		Metadata = new("medieval_market_izar_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6947", []),
		Noun = "izar",
		ShortDescription = "a $colour market izar wrap",
		LongDescription = null,
		FullDescription = "This $colour market izar wrap is a broad linen wrap with enough length to pass around the body and fall in layered folds. The edges are plain and straight, letting the drape and overlap form most of the garment's shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShortWorkCaftan() => new()
	{
		Metadata = new("medieval_short_work_caftan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6978", []),
		Noun = "caftan",
		ShortDescription = "a short $colour work caftan",
		LongDescription = null,
		FullDescription = "This short $colour work caftan is a practical front-opening coat with a trimmed skirt and sleeves kept clear of the hands. It fastens close enough for movement in a market or workshop, without the length of a formal robe. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRusTownRubakha() => new()
	{
		Metadata = new("medieval_rus_town_rubakha", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7009", []),
		Noun = "rubakha",
		ShortDescription = "a neat $colour town rubakha",
		LongDescription = null,
		FullDescription = "This neat $colour town rubakha is a long shirt-like garment with a slit neck, narrow cuffs, and straight body panels. The hem falls low enough to blouse over a belt, and the sleeves are cut for work beneath a heavier coat. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 430.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTuckedPonevaWorkSkirt() => new()
	{
		Metadata = new("medieval_tucked_poneva_work_skirt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7040", []),
		Noun = "poneva",
		ShortDescription = "a tucked $colour poneva work skirt",
		LongDescription = null,
		FullDescription = "This tucked $colour poneva work skirt is a wrapped wool skirt arranged to keep the lower edge clear of dirty floors or tools. The cloth overlaps at the waist and falls in practical folds across the hips. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Skirt",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurriersLeatherApron() => new()
	{
		Metadata = new("medieval_furriers_leather_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7071", []),
		Noun = "apron",
		ShortDescription = "a leather furrier's apron",
		LongDescription = null,
		FullDescription = "This leather furrier's apron is a broad work apron with a long fall and smooth enough surface to shed hair and scraps. The upper edge sits high on the body, while the lower portion is plain and durable. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFeltMarketCap() => new()
	{
		Metadata = new("medieval_felt_market_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7101", []),
		Noun = "cap",
		ShortDescription = "a neat $colour felt market cap",
		LongDescription = null,
		FullDescription = "This neat $colour felt market cap is a market felt cap with a firm crown and a plain lower edge. The felt is dense enough to keep its shape without much internal structure. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWideLeatherWorkBelt() => new()
	{
		Metadata = new("medieval_wide_leather_work_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7132", []),
		Noun = "belt",
		ShortDescription = "a wide leather work belt",
		LongDescription = null,
		FullDescription = "This wide leather work belt is a broad strap of sturdy leather with a plain buckle and reinforced holes. It is shaped to sit firmly over work clothes, with scuffed edges and a practical, unadorned face. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMountedTraderCoat() => new()
	{
		Metadata = new("medieval_mounted_trader_coat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7162", []),
		Noun = "coat",
		ShortDescription = "a neat $colour mounted trader's coat",
		LongDescription = null,
		FullDescription = "This neat $colour mounted trader's coat is a long wool coat with a practical front opening and a skirt cut for sitting in a saddle. The cloth is better finished than field wear, but the seams remain sturdy and work-minded. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShortCottonWorkDhoti() => new()
	{
		Metadata = new("medieval_short_cotton_work_dhoti", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7193", []),
		Noun = "dhoti",
		ShortDescription = "a short $colour cotton work dhoti",
		LongDescription = null,
		FullDescription = "This short $colour cotton work dhoti is a short work lower garment made from a rectangular length of cotton. The fabric is folded and passed around the waist and legs, making its shape depend on the wrapping rather than stitching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCottonWorkWaistcloth() => new()
	{
		Metadata = new("medieval_cotton_work_waistcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7224", []),
		Noun = "waistcloth",
		ShortDescription = "a $colour cotton workshop waistcloth",
		LongDescription = null,
		FullDescription = "This $colour cotton workshop waistcloth is a practical rectangular wrap for tying around the waist while working. The cloth is light enough to tuck up, and the edges are plainly finished to survive repeated washing and tying. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWeaversCottonShouldercloth() => new()
	{
		Metadata = new("medieval_weavers_cotton_shouldercloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7255", []),
		Noun = "shouldercloth",
		ShortDescription = "a $colour weaver's shouldercloth",
		LongDescription = null,
		FullDescription = "This $colour weaver's shouldercloth is a practical cotton drape sized to cover one shoulder and part of the chest. The cloth is light, foldable, and easy to shift aside while working with thread or tools. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMarketCottonTurban() => new()
	{
		Metadata = new("medieval_market_cotton_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7286", []),
		Noun = "turban",
		ShortDescription = "a clean $colour market turban",
		LongDescription = null,
		FullDescription = "This clean $colour market turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeBorderedUttariya() => new()
	{
		Metadata = new("medieval_trade_bordered_uttariya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7317", []),
		Noun = "uttariya",
		ShortDescription = "a $colour trade-bordered uttariya",
		LongDescription = null,
		FullDescription = "This $colour trade-bordered uttariya is a long draped shoulder cloth meant to pass over one or both shoulders. The edges are kept straight, and the fabric is light enough to fold without making a bulky knot. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 230.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSouthIndianWorkVeshti() => new()
	{
		Metadata = new("medieval_south_indian_work_veshti", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7348", []),
		Noun = "veshti",
		ShortDescription = "a tucked $colour cotton work veshti",
		LongDescription = null,
		FullDescription = "This tucked $colour cotton work veshti is a rectangular cotton lower cloth wrapped around the waist and falling in straight folds. The edges are clean and visible, and the front can be tucked to keep the hem clear for movement. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 250.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCottonBazaarSari() => new()
	{
		Metadata = new("medieval_cotton_bazaar_sari", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7379", []),
		Noun = "sari",
		ShortDescription = "a tucked $colour cotton bazaar sari",
		LongDescription = null,
		FullDescription = "This tucked $colour cotton bazaar sari is a long wrapped garment with enough length to form both lower drape and upper fold. The cloth relies on pleating, tucking, and wrapping rather than seams, leaving the edge visible along the fall. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSongWorkShanJacket() => new()
	{
		Metadata = new("medieval_song_work_shan_jacket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7410", []),
		Noun = "jacket",
		ShortDescription = "a plain $colour work shan jacket",
		LongDescription = null,
		FullDescription = "This plain $colour work shan jacket is a short cross-collar jacket with straight sleeves, a wrapped front, and a tidy waist-length body. The garment is cut for movement, with the front overlap and cuffs giving it most of its visible shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 330.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ramie",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Jacket",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSongShortWorkTrousers() => new()
	{
		Metadata = new("medieval_song_short_work_trousers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7441", []),
		Noun = "trousers",
		ShortDescription = "a pair of short $colour work trousers",
		LongDescription = null,
		FullDescription = "These short $colour work trousers are practical trousers cut shorter through the lower leg for workshop and market movement. The waist is gathered plainly, and the hems are kept simple so they can be tucked or left loose. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSongShopkeeperRobe() => new()
	{
		Metadata = new("medieval_song_shopkeeper_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7472", []),
		Noun = "robe",
		ShortDescription = "a neat $colour shopkeeper's robe",
		LongDescription = null,
		FullDescription = "This neat $colour shopkeeper's robe is a long silk robe with orderly sleeves, a tidy collar, and a fall suited to indoor trade or counter work. It is more polished than a labourer's jacket but less ornate than court dress. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 560.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSongMerchantHeadscarf() => new()
	{
		Metadata = new("medieval_song_merchant_headscarf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7503", []),
		Noun = "headscarf",
		ShortDescription = "a neat $colour merchant headscarf",
		LongDescription = null,
		FullDescription = "This neat $colour merchant headscarf is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Kerchief",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSongCottonWorkApron() => new()
	{
		Metadata = new("medieval_song_cotton_work_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7534", []),
		Noun = "apron",
		ShortDescription = "a $colour cotton work apron",
		LongDescription = null,
		FullDescription = "This $colour cotton work apron is a plain protective cloth with a straight fall over the front of the body. The upper edge and ties are simple, keeping the garment easy to put on over ordinary working clothes. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalGoryeoWorkJeogori() => new()
	{
		Metadata = new("medieval_goryeo_work_jeogori", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7565", []),
		Noun = "jeogori",
		ShortDescription = "a plain $colour work jeogori",
		LongDescription = null,
		FullDescription = "This plain $colour work jeogori is a short cross-collar jacket with narrow sleeves and ties at the front. The body ends high at the waist, leaving room for trousers or a skirt below. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 340.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ramie",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Jacket",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalGoryeoWorkBaji() => new()
	{
		Metadata = new("medieval_goryeo_work_baji", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7596", []),
		Noun = "baji",
		ShortDescription = "a pair of $colour work baji",
		LongDescription = null,
		FullDescription = "These $colour work baji are loose trousers with a gathered waist, full seat, and narrowing lower legs. The cloth is cut for easy movement beneath a jacket or robe, with plain seams and practical hems. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 340.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ramie",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalGoryeoMarketChima() => new()
	{
		Metadata = new("medieval_goryeo_market_chima", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7627", []),
		Noun = "chima",
		ShortDescription = "a plain $colour market chima",
		LongDescription = null,
		FullDescription = "This plain $colour market chima is a full skirt with a gathered upper edge and a broad fall from waist to lower hem. The fabric spreads in soft folds and is meant to sit below a short jacket or over-robe. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 430.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ramie",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Skirt",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalGoryeoShopPo() => new()
	{
		Metadata = new("medieval_goryeo_shop_po", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7658", []),
		Noun = "po",
		ShortDescription = "a neat $colour shop po",
		LongDescription = null,
		FullDescription = "This neat $colour shop po is a long over-robe with broad sleeves, a wrapped front, and a steady fall below the knee. It is cut to sit over a jacket and trousers or skirt, adding a cleaner outer line. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 580.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJapaneseWorkKosode() => new()
	{
		Metadata = new("medieval_japanese_work_kosode", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7689", []),
		Noun = "kosode",
		ShortDescription = "a plain $colour work kosode",
		LongDescription = null,
		FullDescription = "This plain $colour work kosode is a straight-seamed robe with small sleeve openings, a wrapped front, and a narrow collar. The body is made from rectangular panels, giving it a clean fall and visible folded overlap at the chest. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ramie",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJapaneseWorkHakama() => new()
	{
		Metadata = new("medieval_japanese_work_hakama", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7720", []),
		Noun = "hakama",
		ShortDescription = "a pair of $colour work hakama",
		LongDescription = null,
		FullDescription = "These $colour work hakama are divided skirt-like trousers with deep folds falling from the waist. The legs are broad and straight, giving the garment a formal drape while still allowing movement. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJapaneseHempWorkApron() => new()
	{
		Metadata = new("medieval_japanese_hemp_work_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7751", []),
		Noun = "apron",
		ShortDescription = "a hemp work apron",
		LongDescription = null,
		FullDescription = "This hemp work apron is a coarse protective layer with a straight bib or waist fall and plain ties. The cloth is sturdy, rough-textured, and meant to take stains before the clothing beneath does. The natural hemp surface is plainly visible, with small slubs and a practical, unpolished finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 170.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJapaneseYumakiWrap() => new()
	{
		Metadata = new("medieval_japanese_yumaki_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7781", []),
		Noun = "yumaki",
		ShortDescription = "a $colour yumaki waist wrap",
		LongDescription = null,
		FullDescription = "This $colour yumaki waist wrap is a simple hemp cloth tied around the waist as an apron-like working layer. The fabric is rectangular, foldable, and light enough to overlap without becoming bulky. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinBlackMonasticHabit() => new()
	{
		Metadata = new("medieval_latin_black_monastic_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7812", []),
		Noun = "habit",
		ShortDescription = "a black wool monastic habit",
		LongDescription = null,
		FullDescription = "This black wool monastic habit is cut as a long, loose robe with full sleeves and a plain round neck. The cloth falls in heavy vertical folds from shoulder to ankle, leaving enough breadth for an undertunic beneath. Its colour and cut are deliberately austere, with only the dark wool, simple seams, and even hem giving it shape.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinBlackScapular() => new()
	{
		Metadata = new("medieval_latin_black_scapular", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7842", []),
		Noun = "scapular",
		ShortDescription = "a black wool scapular",
		LongDescription = null,
		FullDescription = "This black wool scapular is made as a long, narrow over-panel to hang down the front and back of the body. The shoulders are plain and the sides are left open, so the garment reads as a simple vertical strip rather than a coat. The wool is closely woven but unadorned, with a straight hem and restrained black surface.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tabard",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinBlackCowl() => new()
	{
		Metadata = new("medieval_latin_black_cowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7872", []),
		Noun = "cowl",
		ShortDescription = "a black wool cowl",
		LongDescription = null,
		FullDescription = "This black wool cowl is a heavy hooded overgarment made to fall in a single dark mass around the shoulders. The hood is deep, the body loose, and the hem broad enough to drape over a habit beneath it. Its surface is plain and matte, with only the weight of the wool and the shadow of the hood giving it emphasis.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteMonasticTunic() => new()
	{
		Metadata = new("medieval_latin_white_monastic_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7902", []),
		Noun = "tunic",
		ShortDescription = "an undyed wool monastic tunic",
		LongDescription = null,
		FullDescription = "This undyed wool monastic tunic is a long, straight robe with full sleeves and a modest neckline. The pale wool keeps a natural cream cast, broken only by simple seams at the shoulders, sleeves, and sides. It is practical rather than fine, shaped to hang plainly from the body under a scapular, cowl, or mantle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 820.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinBlackOverScapular() => new()
	{
		Metadata = new("medieval_latin_black_over_scapular", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7932", []),
		Noun = "scapular",
		ShortDescription = "a black wool over-scapular",
		LongDescription = null,
		FullDescription = "This black wool over-scapular is a severe sleeveless panel meant to be worn over a pale habit. It hangs as two straight strips joined at the shoulders, framing the body without closing at the sides. The contrast between its dark wool and plain geometry makes it visibly distinct without adding ornament.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tabard",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteCarthusianHabit() => new()
	{
		Metadata = new("medieval_latin_white_carthusian_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7962", []),
		Noun = "habit",
		ShortDescription = "a white wool hermit's habit",
		LongDescription = null,
		FullDescription = "This white wool hermit's habit is cut as a roomy, ankle-length robe with broad sleeves and a sober fall. The cloth is undyed, thick, and slightly rough, giving the garment a pale but workmanlike appearance. It is deliberately spare in construction, with plain seams, a simple waistline, and no decorative edging.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinCanonBlackHabit() => new()
	{
		Metadata = new("medieval_latin_canon_black_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:7992", []),
		Noun = "habit",
		ShortDescription = "a black wool canon's habit",
		LongDescription = null,
		FullDescription = "This black wool canon's habit is a long clerical robe with ample sleeves and a neat, restrained cut. The wool is darker and smoother than most labouring garments, but it keeps a sober face without bright trim. Its hem and cuffs are carefully finished, giving it a disciplined appearance suitable for choir or school use.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteRochet() => new()
	{
		Metadata = new("medieval_latin_white_rochet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8022", []),
		Noun = "rochet",
		ShortDescription = "a white linen rochet",
		LongDescription = null,
		FullDescription = "This white linen rochet is a clean, knee-length clerical overgarment with narrow sleeves and a squared body. The linen is lighter than wool but crisply finished, allowing it to fall in pale folds over a darker habit or gown. Its edges are plain and tidy, relying on whiteness and neat cutting rather than decoration.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 320.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinFriarGreyHabit() => new()
	{
		Metadata = new("medieval_latin_friar_grey_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8052", []),
		Noun = "habit",
		ShortDescription = "a grey wool friar's habit",
		LongDescription = null,
		FullDescription = "This grey wool friar's habit is a loose, ankle-length robe with simple sleeves and a practical hooded fall. The wool is coarse enough to show its weave, with a muted grey colour that avoids costly dye. Its shape is deliberately plain, meant to be gathered with a cord rather than tailored closely.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinRopeCincture() => new()
	{
		Metadata = new("medieval_latin_rope_cincture", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8082", []),
		Noun = "cincture",
		ShortDescription = "a knotted rope cincture",
		LongDescription = null,
		FullDescription = "This rope cincture is twisted from sturdy hemp into a rough belt for tying around a habit. Several visible knots break the line of the cord, leaving the ends to hang plainly at one side. The fibre is pale, dry, and uneven, more practical than decorative.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteFriarHabit() => new()
	{
		Metadata = new("medieval_latin_white_friar_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8112", []),
		Noun = "habit",
		ShortDescription = "a white wool friar's habit",
		LongDescription = null,
		FullDescription = "This white wool friar's habit is a long, simple robe with full sleeves and a straight front. The undyed wool has a pale cream tone, kept clean enough to stand out beneath a darker travelling cloak. Its construction is modest but orderly, with a close neckline, even cuffs, and a hem meant to clear the feet.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinBlackFriarCappa() => new()
	{
		Metadata = new("medieval_latin_black_friar_cappa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8142", []),
		Noun = "cappa",
		ShortDescription = "a black wool friar's cappa",
		LongDescription = null,
		FullDescription = "This black wool friar's cappa is a full open cloak made to hang over a pale habit. The cloth gathers heavily at the shoulders and spreads in a dark sweep down the body. A simple hood or cape-like upper fold gives it sheltering weight without bright trim or fittings.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinBrownFriarHabit() => new()
	{
		Metadata = new("medieval_latin_brown_friar_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8172", []),
		Noun = "habit",
		ShortDescription = "a brown wool friar's habit",
		LongDescription = null,
		FullDescription = "This brown wool friar's habit is a plain robe with a deep body, loose sleeves, and enough length to cover the ankles. The brown wool is practical and subdued, with a slightly uneven texture that shows the hand of the cloth. It is shaped to be belted at the waist and worn under a mantle when needed.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 800.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinWhiteFriarMantle() => new()
	{
		Metadata = new("medieval_latin_white_friar_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8202", []),
		Noun = "mantle",
		ShortDescription = "a white wool friar's mantle",
		LongDescription = null,
		FullDescription = "This white wool friar's mantle is a broad outer wrap with a pale, even face and cleanly finished borders. It drapes across the shoulders and down the back, leaving the arms free beneath the loose fall of cloth. The mantle is finer than a labourer's cloak but remains modest, with no ornament beyond its colour and full sweep.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 640.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinSimpleNunHabit() => new()
	{
		Metadata = new("medieval_latin_simple_nun_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8232", []),
		Noun = "habit",
		ShortDescription = "a dark wool nun's habit",
		LongDescription = null,
		FullDescription = "This dark wool nun's habit is a long, full robe with modest sleeves and a straight, unshaped body. The dark cloth is plain and heavy, falling in quiet folds from the shoulders to the feet. Its cut is conservative, leaving the garment defined by coverage, simplicity, and an orderly hem.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 820.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinNunWimpleVeil() => new()
	{
		Metadata = new("medieval_latin_nun_wimple_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8262", []),
		Noun = "veil",
		ShortDescription = "a white linen wimple and veil",
		LongDescription = null,
		FullDescription = "This white linen wimple and veil is arranged as layered headcloths that frame the face and cover the throat. The linen is plain but clean, with folded edges that sit smoothly beneath a darker veil or hood. Its appearance is modest and structured, giving the head and shoulders a pale, enclosed outline.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinPoorClareGreyHabit() => new()
	{
		Metadata = new("medieval_latin_poor_clare_grey_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8292", []),
		Noun = "habit",
		ShortDescription = "a grey wool sister's habit",
		LongDescription = null,
		FullDescription = "This grey wool sister's habit is a plain, ankle-length robe with simple sleeves and a loosely falling body. The cloth is muted and inexpensive, with a rough grey surface that avoids display. It is shaped for a cord belt and veil, making its austerity visible in every line.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinPoorClareVeil() => new()
	{
		Metadata = new("medieval_latin_poor_clare_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8322", []),
		Noun = "veil",
		ShortDescription = "a plain linen sister's veil",
		LongDescription = null,
		FullDescription = "This plain linen sister's veil is a simple white head covering with little shaping beyond its folded front edge. It falls over the hair and shoulders in modest layers, leaving the face open. The fabric is light, matte, and undecorated, suited to an austere habit rather than courtly dress.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinBlackClericalGown() => new()
	{
		Metadata = new("medieval_latin_black_clerical_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8352", []),
		Noun = "gown",
		ShortDescription = "a black wool clerical gown",
		LongDescription = null,
		FullDescription = "This black wool clerical gown is a long, sober garment with straight sleeves and a restrained neckline. The cloth is smooth enough for formal town or church use, but its colour and lack of ornament keep it severe. It hangs neatly from shoulder to ankle, with a plain front and carefully finished hems.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinProcessionalCope() => new()
	{
		Metadata = new("medieval_latin_processional_cope", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8382", []),
		Noun = "cope",
		ShortDescription = "a fine $colour silk cope",
		LongDescription = null,
		FullDescription = "This fine $colour silk cope is a great open cloak with a broad semicircular sweep and a smooth outer face. The silk gathers at the shoulders before spreading into a full ceremonial fall, showing the $colour most clearly across the back. Its edge is carefully finished, making the whole garment read as formal outerwear rather than a travelling cloak.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinBishopMitre() => new()
	{
		Metadata = new("medieval_latin_bishop_mitre", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8413", []),
		Noun = "mitre",
		ShortDescription = "a fine white silk mitre",
		LongDescription = null,
		FullDescription = "This fine white silk mitre is a tall folded headpiece with two stiffened peaks rising above the brow. The silk is pale and smooth, with carefully joined seams holding the upright form. Narrow hanging bands fall from the back, making the headwear unmistakably formal without adding other carried objects.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 160.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinBishopGloves() => new()
	{
		Metadata = new("medieval_latin_bishop_gloves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8443", []),
		Noun = "gloves",
		ShortDescription = "a pair of fine white liturgical gloves",
		LongDescription = null,
		FullDescription = "These fine white liturgical gloves are sewn from smooth silk with close fingers and neat wrist openings. The fabric has a pale sheen and lies cleanly against the hand when worn. Their finish is more delicate than working gloves, with the emphasis on spotless cloth and careful seams.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Gloves"
			],
		Components = [
				"Holdable",
				"Wear_Gloves",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLatinArchbishopPallium() => new()
	{
		Metadata = new("medieval_latin_archbishop_pallium", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8473", []),
		Noun = "pallium",
		ShortDescription = "a white wool pallium band",
		LongDescription = null,
		FullDescription = "This white wool pallium band is a narrow looped vestment with hanging ends that fall over the chest and back. The wool is fine, pale, and tightly woven, giving the small garment a clean ceremonial presence. Dark cross-like markings are worked into the band as visible decoration rather than separate metal fittings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140.0,
		Cost = 100.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Scarf",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternMonasticMantle() => new()
	{
		Metadata = new("medieval_eastern_monastic_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8505", []),
		Noun = "mantle",
		ShortDescription = "a black wool monastic mantle",
		LongDescription = null,
		FullDescription = "This black wool monastic mantle is a wide outer wrap with enough breadth to cover the shoulders and much of the body. Its folds are heavy and dark, giving the garment a solemn, enclosing silhouette. The edges are simple and unembroidered, with visual weight supplied by the cloth itself.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternBlackKlobuk() => new()
	{
		Metadata = new("medieval_eastern_black_klobuk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8535", []),
		Noun = "klobuk",
		ShortDescription = "a black veiled klobuk",
		LongDescription = null,
		FullDescription = "This black veiled klobuk combines a firm dark cap with a hanging veil that falls around the back and sides. The wool is matte and close, with the veil softening the hard line of the cap. It frames the head in a severe, recognisable shape without relying on jewels or bright colour.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternWomensMonasticVeil() => new()
	{
		Metadata = new("medieval_eastern_womens_monastic_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8565", []),
		Noun = "veil",
		ShortDescription = "a black monastic veil",
		LongDescription = null,
		FullDescription = "This black monastic veil is a plain wool head covering made to fall over the hair, shoulders, and upper back. The fabric is soft enough to fold close to the face but heavy enough to keep a dark, settled outline. It is modest and undecorated, with only the clean fall of cloth marking its form.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 200.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternOrarion() => new()
	{
		Metadata = new("medieval_eastern_orarion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8597", []),
		Noun = "orarion",
		ShortDescription = "a $colour silk orarion",
		LongDescription = null,
		FullDescription = "This $colour silk orarion is a long narrow vestment meant to lie over the shoulder in a clear vertical line. The silk is smooth and formal, making the $colour surface catch light along its length. Its edges are straight and carefully finished, giving the band a precise liturgical appearance.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 130.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Scarf",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternSakkos() => new()
	{
		Metadata = new("medieval_eastern_sakkos", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8634", []),
		Noun = "sakkos",
		ShortDescription = "a fine $colour silk sakkos",
		LongDescription = null,
		FullDescription = "This fine $colour silk sakkos is a square-cut outer vestment with broad sleeves and a structured, tunic-like body. The sides are visibly joined by small fastenings or ties, leaving the garment formal but not closely fitted. Its $colour silk and deliberate shape make it more imposing than an ordinary robe.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 800.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternOmophorion() => new()
	{
		Metadata = new("medieval_eastern_omophorion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8665", []),
		Noun = "omophorion",
		ShortDescription = "a fine white silk omophorion",
		LongDescription = null,
		FullDescription = "This fine white silk omophorion is a broad band made to lie over the shoulders and fall across the chest. The silk is pale, smooth, and substantial enough to hold its line over heavier vestments. Dark cross-like markings break the white field, giving the band an episcopal and ceremonial appearance.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 220.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Scarf",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicPlainImamTurban() => new()
	{
		Metadata = new("medieval_islamic_plain_imam_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8699", []),
		Noun = "turban",
		ShortDescription = "a plain white cotton imam turban",
		LongDescription = null,
		FullDescription = "This plain white cotton imam turban is made from a long length of cloth wrapped into a neat, layered head covering. The cotton is clean and soft, with overlapping folds forming a modest rounded shape. No jewel or badge marks it, leaving its presence in the whiteness and regularity of the wrap.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicScholarsJubba() => new()
	{
		Metadata = new("medieval_islamic_scholars_jubba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8729", []),
		Noun = "jubba",
		ShortDescription = "a dark wool scholar's jubba",
		LongDescription = null,
		FullDescription = "This dark wool scholar's jubba is a long outer robe with full sleeves and a dignified, loose body. The wool is smoother than common work cloth, giving the dark surface a measured formal weight. It is cut for layering over a qamis and sirwal, with plain hems and a calm, authoritative silhouette.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicTaylasan() => new()
	{
		Metadata = new("medieval_islamic_taylasan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8759", []),
		Noun = "taylasan",
		ShortDescription = "a dark wool taylasan hood",
		LongDescription = null,
		FullDescription = "This dark wool taylasan is a hooded head covering with a short shoulder fall and a sober drape. The wool is fine enough for learned or official wear, but its colour remains restrained. Its shape frames the head and upper shoulders without becoming a full cloak.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 240.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicQadiTurban() => new()
	{
		Metadata = new("medieval_islamic_qadi_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8789", []),
		Noun = "turban",
		ShortDescription = "a fine white scholar's turban",
		LongDescription = null,
		FullDescription = "This fine white scholar's turban is wrapped from a long, clean length of cotton into a careful formal shape. The folds are regular and layered, giving the headwear more presence than a simple working wrap. Its colour is plain, but the neatness of the cotton and the size of the winding give it dignity.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSufiPatchedKhirqa() => new()
	{
		Metadata = new("medieval_sufi_patched_khirqa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8819", []),
		Noun = "khirqa",
		ShortDescription = "a patched wool khirqa",
		LongDescription = null,
		FullDescription = "This patched wool khirqa is a loose robe assembled from visibly different pieces of worn cloth. The patches vary in tone and texture, but the whole garment keeps a roughly ordered robe shape with long sleeves and a full body. It looks intentionally poor and mended, using its roughness as its most visible feature.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSufiWoolSash() => new()
	{
		Metadata = new("medieval_sufi_wool_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8849", []),
		Noun = "sash",
		ShortDescription = "a coarse wool Sufi sash",
		LongDescription = null,
		FullDescription = "This coarse wool Sufi sash is a plain length of cloth meant to bind a robe at the waist. The wool is rough, dark, and slightly uneven, with frayed-looking ends left visible. It is more severe than decorative, adding a practical horizontal line to otherwise loose clothing.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSufiFeltCap() => new()
	{
		Metadata = new("medieval_sufi_felt_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8879", []),
		Noun = "cap",
		ShortDescription = "a plain felt dervish cap",
		LongDescription = null,
		FullDescription = "This plain felt dervish cap is a simple, close head covering shaped from dense felt. Its sides are unadorned, and the crown is rounded without embroidery or metal fittings. The material gives it a soft but durable outline suited to rough travel or simple lodgings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSufiCoarseWoolCloak() => new()
	{
		Metadata = new("medieval_sufi_coarse_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8909", []),
		Noun = "cloak",
		ShortDescription = "a coarse wool Sufi cloak",
		LongDescription = null,
		FullDescription = "This coarse wool Sufi cloak is a heavy open wrap with a rough, plainly woven surface. It falls from the shoulders in uneven dark folds, large enough to cover a patched robe or plain qamis beneath. The cloak is austere rather than elegant, with a practical weight and little concern for polish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishScholarsRobe() => new()
	{
		Metadata = new("medieval_jewish_scholars_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8945", []),
		Noun = "robe",
		ShortDescription = "a dark wool scholar's robe",
		LongDescription = null,
		FullDescription = "This dark wool scholar's robe is a long, dignified garment with full sleeves and a restrained front. The wool is smooth enough to suggest careful making, while the dark colour keeps it sober. It is shaped for town and synagogue use, with neat hems and an uncluttered fall over inner layers.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduWhiteUttariya() => new()
	{
		Metadata = new("medieval_hindu_white_uttariya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:8977", []),
		Noun = "uttariya",
		ShortDescription = "a white cotton uttariya",
		LongDescription = null,
		FullDescription = "This white cotton uttariya is a rectangular shoulder cloth made to pass over the upper body in a simple drape. The cotton is thin, clean, and unpatterned, showing soft folds where it crosses the shoulder. It is modest and light, meant to sit over bare skin or a simple underlayer without ornament.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduOchreAsceticWrap() => new()
	{
		Metadata = new("medieval_hindu_ochre_ascetic_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9009", []),
		Noun = "wrap",
		ShortDescription = "an ochre cotton ascetic wrap",
		LongDescription = null,
		FullDescription = "This ochre cotton ascetic wrap is a long rectangular cloth arranged to cover the body in loose folds. The cotton has a warm earth-coloured tone and a plain, unshaped surface. Its form is intentionally simple, relying on wrapping and tucking rather than tailoring.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduOchreShoulderCloth() => new()
	{
		Metadata = new("medieval_hindu_ochre_shoulder_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9039", []),
		Noun = "cloth",
		ShortDescription = "an ochre cotton shoulder cloth",
		LongDescription = null,
		FullDescription = "This ochre cotton shoulder cloth is a narrow rectangular drape for the upper body. The fabric is light and plain, with its warm colour showing most clearly along the broad hanging panels. It can fall over one shoulder or both, giving a simple ascetic layer above a lower wrap.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistPatchedKasaya() => new()
	{
		Metadata = new("medieval_buddhist_patched_kasaya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9073", []),
		Noun = "kasaya",
		ShortDescription = "a patched cotton kasaya",
		LongDescription = null,
		FullDescription = "This patched cotton kasaya is a rectangular mantle built from many sewn panels. The cotton pieces are arranged in a deliberate grid, with seams forming a visible patchwork field across the cloth. It drapes over the shoulder and upper body, looking formal through pattern and construction rather than luxury.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistNunRobe() => new()
	{
		Metadata = new("medieval_buddhist_nun_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9103", []),
		Noun = "robe",
		ShortDescription = "a dark hemp nun robe",
		LongDescription = null,
		FullDescription = "This dark hemp nun robe is a plain wrapped garment with long sleeves and a full, modest body. The hemp is firm and matte, with a darkened surface that hides wear better than pale cloth. Its lines are simple and practical, leaving the robe suited to layered monastic dress.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistBlackMonasticRobe() => new()
	{
		Metadata = new("medieval_buddhist_black_monastic_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9133", []),
		Noun = "robe",
		ShortDescription = "a black hemp monastic robe",
		LongDescription = null,
		FullDescription = "This black hemp monastic robe is a straight wrapped garment with close sleeves and a plain collar line. The hemp has a dry, matte surface, darkened into a sober black without decorative contrast. Its cut is practical and restrained, keeping the focus on length, folds, and simplicity.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 540.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSteppeRitualFeltCoat() => new()
	{
		Metadata = new("medieval_steppe_ritual_felt_coat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9179", []),
		Noun = "coat",
		ShortDescription = "a felt-lined ritual coat",
		LongDescription = null,
		FullDescription = "This felt-lined ritual coat is a long front-opening garment with loose sleeves and a practical steppe cut. Dense felt gives it body and warmth, while the outer surface remains plain enough to resemble travelling clothing. Its ritual character comes from its deliberate use as a special outer layer rather than from metal fittings or carried objects.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};
}
