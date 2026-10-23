#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical03Stock
{
	public static CatalogueItemDefinition MedievalSteppeRidingCaftan() => new()
	{
		Metadata = new("medieval_steppe_riding_caftan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3016", []),
		Noun = "caftan",
		ShortDescription = "a $colour riding caftan",
		LongDescription = null,
		FullDescription = "This $colour riding caftan is a front-opening coat with long sleeves, a beltable waist, and a skirt shaped for movement. The front edges and cuffs are the most carefully finished parts of the garment. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 880.0,
		Cost = 40.0m,
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

	public static CatalogueItemDefinition MedievalFineSteppeCaftan() => new()
	{
		Metadata = new("medieval_fine_steppe_caftan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3047", []),
		Noun = "caftan",
		ShortDescription = "a fine $colour silk-banded caftan",
		LongDescription = null,
		FullDescription = "This fine $colour silk-banded caftan is a front-opening coat with decorative bands at the neck, front edges, and cuffs. The skirt has enough flare for riding, and the sleeves are cut to fall cleanly from the shoulder. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 920.0,
		Cost = 125.0m,
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

	public static CatalogueItemDefinition MedievalSteppeFeltBoots() => new()
	{
		Metadata = new("medieval_steppe_felt_boots", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3078", []),
		Noun = "boots",
		ShortDescription = "a pair of felt riding boots",
		LongDescription = null,
		FullDescription = "These felt riding boots are tall, warm boots shaped from dense felt with reinforced soles and a firm lower edge. The shafts are broad enough to fit over trousers while still narrowing toward the ankle. The natural felt surface is plainly visible, with a compacted texture suited to cold, dry travel.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_Boots",
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

	public static CatalogueItemDefinition MedievalSteppeFeltCap() => new()
	{
		Metadata = new("medieval_steppe_felt_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3108", []),
		Noun = "cap",
		ShortDescription = "a $colour steppe felt cap",
		LongDescription = null,
		FullDescription = "This $colour steppe felt cap is a steppe felt cap with a firm crown and a plain lower edge. The felt is dense enough to keep its shape without much internal structure. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 9.0m,
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

	public static CatalogueItemDefinition MedievalSteppeSilkBelt() => new()
	{
		Metadata = new("medieval_steppe_silk_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3139", []),
		Noun = "belt",
		ShortDescription = "a fine $colour silk belt",
		LongDescription = null,
		FullDescription = "This fine $colour silk belt is a long, narrow band of silk woven to sit flat around the waist. The ends are finished cleanly so they can hang in ordered folds after being wrapped. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 120.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Waist",
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

	public static CatalogueItemDefinition MedievalWoolDeel() => new()
	{
		Metadata = new("medieval_wool_deel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3170", []),
		Noun = "robe",
		ShortDescription = "a $colour belted steppe robe",
		LongDescription = null,
		FullDescription = "This $colour belted steppe robe is a belted robe with a wrapping front, long sleeves, and a skirt shaped for riding or walking. The cloth crosses the chest in firm folds and is meant to be controlled by a waist belt. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980.0,
		Cost = 46.0m,
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

	public static CatalogueItemDefinition MedievalFineSilkDeel() => new()
	{
		Metadata = new("medieval_fine_silk_deel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3201", []),
		Noun = "robe",
		ShortDescription = "a fine $colour silk steppe robe",
		LongDescription = null,
		FullDescription = "This fine $colour silk steppe robe is a belted robe with a wrapping front, long sleeves, and a skirt shaped for riding or walking. The cloth crosses the chest in firm folds and is meant to be controlled by a waist belt. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 700.0,
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

	public static CatalogueItemDefinition MedievalRusLinenRubakha() => new()
	{
		Metadata = new("medieval_rus_linen_rubakha", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3232", []),
		Noun = "rubakha",
		ShortDescription = "a $colour linen rubakha",
		LongDescription = null,
		FullDescription = "This $colour linen rubakha is a long shirt-like garment with a slit neck, narrow cuffs, and straight body panels. The hem falls low enough to blouse over a belt, and the sleeves are cut for work beneath a heavier coat. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 430.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
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

	public static CatalogueItemDefinition MedievalFineRusRubakha() => new()
	{
		Metadata = new("medieval_fine_rus_rubakha", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3263", []),
		Noun = "rubakha",
		ShortDescription = "a fine $colour embroidered rubakha",
		LongDescription = null,
		FullDescription = "This fine $colour embroidered rubakha is a long shirt-like garment with a small embroidered-looking facing at the neck and cuffs. The hem falls low enough to blouse over a belt, and the sleeves are cut for work beneath a heavier coat. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 450.0,
		Cost = 54.0m,
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

	public static CatalogueItemDefinition MedievalRusWoolPorty() => new()
	{
		Metadata = new("medieval_rus_wool_porty", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3294", []),
		Noun = "porty",
		ShortDescription = "a pair of $colour wool porty",
		LongDescription = null,
		FullDescription = "These $colour wool porty are practical legwear with a gathered waist, straight seams, and enough room through the seat for riding or work. The legs narrow enough to sit beneath boots or wraps without excessive bunching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
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

	public static CatalogueItemDefinition MedievalRusOnuchi() => new()
	{
		Metadata = new("medieval_rus_onuchi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3325", []),
		Noun = "onuchi",
		ShortDescription = "a pair of $colour onuchi leg wraps",
		LongDescription = null,
		FullDescription = "These $colour onuchi leg wraps are long narrow strips of cloth made to wind around the lower legs or feet. The ends are left thin enough to tuck back under the wrapping, while the body of each strip has enough texture to grip itself. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Leggings",
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

	public static CatalogueItemDefinition MedievalRusPoneva() => new()
	{
		Metadata = new("medieval_rus_poneva", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3356", []),
		Noun = "poneva",
		ShortDescription = "a $colour1 and $colour2 woven poneva",
		LongDescription = null,
		FullDescription = "This $colour1 and $colour2 woven poneva is a wrapped wool skirt made from patterned panels with a visible overlap. The waist edge is firm enough to hold a girdle, while the lower hem hangs in straight folds. The $colour1 and $colour2 yarns show in alternating bands across the weave, making the poneva patterned in the cloth rather than painted on top.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 24.0m,
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
				"Variable_2BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRusSleevelessOvergown() => new()
	{
		Metadata = new("medieval_rus_sleeveless_overgown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3387", []),
		Noun = "overgown",
		ShortDescription = "a $colour sleeveless overgown",
		LongDescription = null,
		FullDescription = "This $colour sleeveless overgown is a long wool garment with open armholes and a sleeveless upper body. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
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

	public static CatalogueItemDefinition MedievalRusShuba() => new()
	{
		Metadata = new("medieval_rus_shuba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3418", []),
		Noun = "shuba",
		ShortDescription = "a $colour fur-lined shuba",
		LongDescription = null,
		FullDescription = "This $colour fur-lined shuba is a warm overcoat with wool outside and fur visible along the collar, cuffs, and opening. The body is cut generously so it can close over a rubakha, trousers, and winter layers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 120.0m,
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

	public static CatalogueItemDefinition MedievalRusFurHat() => new()
	{
		Metadata = new("medieval_rus_fur_hat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3449", []),
		Noun = "hat",
		ShortDescription = "a fur-trimmed wool hat",
		LongDescription = null,
		FullDescription = "This fur-trimmed wool hat is a rounded cap with a wool crown and fur marking the lower edge. The trim adds warmth around the brow and ears without covering the whole hat in pelt. Natural variations in the fur and wool show where the materials meet at the brim.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 220.0,
		Cost = 36.0m,
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

	public static CatalogueItemDefinition MedievalCottonDhoti() => new()
	{
		Metadata = new("medieval_cotton_dhoti", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3479", []),
		Noun = "dhoti",
		ShortDescription = "a $colour cotton dhoti",
		LongDescription = null,
		FullDescription = "This $colour cotton dhoti is a long wrapped lower garment made from a rectangular length of cotton. The fabric is folded and passed around the waist and legs, making its shape depend on the wrapping rather than stitching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
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

	public static CatalogueItemDefinition MedievalFineCottonDhoti() => new()
	{
		Metadata = new("medieval_fine_cotton_dhoti", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3510", []),
		Noun = "dhoti",
		ShortDescription = "a fine $colour cotton dhoti",
		LongDescription = null,
		FullDescription = "This fine $colour cotton dhoti is a long wrapped lower garment made from a rectangular length of cotton. The fabric is folded and passed around the waist and legs, making its shape depend on the wrapping rather than stitching. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Loincloth",
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

	public static CatalogueItemDefinition MedievalCottonUttariya() => new()
	{
		Metadata = new("medieval_cotton_uttariya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3541", []),
		Noun = "uttariya",
		ShortDescription = "a $colour cotton uttariya",
		LongDescription = null,
		FullDescription = "This $colour cotton uttariya is a long draped shoulder cloth meant to pass over one or both shoulders. The edges are kept straight, and the fabric is light enough to fold without making a bulky knot. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
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

	public static CatalogueItemDefinition MedievalFineSilkUttariya() => new()
	{
		Metadata = new("medieval_fine_silk_uttariya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3572", []),
		Noun = "uttariya",
		ShortDescription = "a fine $colour silk uttariya",
		LongDescription = null,
		FullDescription = "This fine $colour silk uttariya is a long draped shoulder cloth meant to pass over one or both shoulders. The edges are kept straight, and the fabric is light enough to fold without making a bulky knot. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 180.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalCottonTurban() => new()
	{
		Metadata = new("medieval_cotton_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3603", []),
		Noun = "turban",
		ShortDescription = "a $colour cotton turban",
		LongDescription = null,
		FullDescription = "This $colour cotton turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 200.0,
		Cost = 11.0m,
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

	public static CatalogueItemDefinition MedievalFineSilkTurbanIndian() => new()
	{
		Metadata = new("medieval_fine_silk_turban_indian", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3634", []),
		Noun = "turban",
		ShortDescription = "a fine $colour silk turban",
		LongDescription = null,
		FullDescription = "This fine $colour silk turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 170.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Turban",
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

	public static CatalogueItemDefinition MedievalFineSilkBodice() => new()
	{
		Metadata = new("medieval_fine_silk_bodice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3671", []),
		Noun = "bodice",
		ShortDescription = "a fine $colour silk bodice",
		LongDescription = null,
		FullDescription = "This fine $colour silk bodice is a short fitted upper-body garment with a close waist, shaped sides, and narrow shoulder coverage. It is meant to sit with a wrapped lower cloth or skirt, keeping the torso neat without heavy layering. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalCottonLowerCloth() => new()
	{
		Metadata = new("medieval_cotton_lower_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3702", []),
		Noun = "cloth",
		ShortDescription = "a $colour cotton lower cloth",
		LongDescription = null,
		FullDescription = "This $colour cotton lower cloth is a rectangular lower cloth meant to wrap at the waist and fall in plain vertical folds. The fabric is simple, flexible, and edged for repeated tying. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
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

	public static CatalogueItemDefinition MedievalSouthIndianVeshti() => new()
	{
		Metadata = new("medieval_south_indian_veshti", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3733", []),
		Noun = "veshti",
		ShortDescription = "a $colour cotton veshti",
		LongDescription = null,
		FullDescription = "This $colour cotton veshti is a rectangular cotton lower cloth wrapped around the waist and falling in straight folds. The edges are clean and visible, and the front can be tucked to keep the hem clear for movement. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
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

	public static CatalogueItemDefinition MedievalSilkBorderedVeshti() => new()
	{
		Metadata = new("medieval_silk_bordered_veshti", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3764", []),
		Noun = "veshti",
		ShortDescription = "a fine $colour silk-bordered veshti",
		LongDescription = null,
		FullDescription = "This fine $colour silk-bordered veshti is a rectangular cotton lower cloth wrapped around the waist and falling in straight folds. The edges are clean and visible, and the front can be tucked to keep the hem clear for movement. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 320.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Loincloth",
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

	public static CatalogueItemDefinition MedievalSouthIndianAngavastram() => new()
	{
		Metadata = new("medieval_south_indian_angavastram", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3795", []),
		Noun = "angavastram",
		ShortDescription = "a $colour cotton angavastram",
		LongDescription = null,
		FullDescription = "This $colour cotton angavastram is a broad shoulder cloth that can be folded, draped, or drawn around the upper body. The cloth is rectangular and plainly edged, relying on the wearer's wrapping for its final shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230.0,
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

	public static CatalogueItemDefinition MedievalFineAngavastram() => new()
	{
		Metadata = new("medieval_fine_angavastram", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3826", []),
		Noun = "angavastram",
		ShortDescription = "a fine $colour silk angavastram",
		LongDescription = null,
		FullDescription = "This fine $colour silk angavastram is a broad shoulder cloth that can be folded, draped, or drawn around the upper body. The cloth is rectangular and plainly edged, relying on the wearer's wrapping for its final shape. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 170.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalCottonWrapSkirt() => new()
	{
		Metadata = new("medieval_cotton_wrap_skirt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3857", []),
		Noun = "skirt",
		ShortDescription = "a $colour cotton wrap skirt",
		LongDescription = null,
		FullDescription = "This $colour cotton wrap skirt is a wrapped skirt made from a broad cloth panel with a visible overlap and plain hem. The fabric falls in practical folds and can be gathered by a sash or belt at the waist. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
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

	public static CatalogueItemDefinition MedievalFineSilkWrapSkirt() => new()
	{
		Metadata = new("medieval_fine_silk_wrap_skirt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3888", []),
		Noun = "skirt",
		ShortDescription = "a fine $colour silk wrap skirt",
		LongDescription = null,
		FullDescription = "This fine $colour silk wrap skirt is a wrapped skirt made from a broad cloth panel with a visible overlap and plain hem. The fabric falls in practical folds and can be gathered by a sash or belt at the waist. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Skirt",
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

	public static CatalogueItemDefinition MedievalSongCrossCollarRobe() => new()
	{
		Metadata = new("medieval_song_cross_collar_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3919", []),
		Noun = "robe",
		ShortDescription = "a $colour cross-collar robe",
		LongDescription = null,
		FullDescription = "This $colour cross-collar robe is a long cloth robe with a diagonal front overlap and straight sleeves. The collar line crosses the chest cleanly, and the skirt falls in narrow vertical folds. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalSongRoundCollarRobe() => new()
	{
		Metadata = new("medieval_song_round_collar_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3950", []),
		Noun = "robe",
		ShortDescription = "a $colour round-collar robe",
		LongDescription = null,
		FullDescription = "This $colour round-collar robe is a long silk robe with a rounded neck opening, long sleeves, and a controlled straight fall. The front and cuffs are neatly finished, giving the garment a more formal silhouette than a work jacket. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 650.0,
		Cost = 96.0m,
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

	public static CatalogueItemDefinition MedievalSongBeizi() => new()
	{
		Metadata = new("medieval_song_beizi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:3981", []),
		Noun = "beizi",
		ShortDescription = "a long $colour beizi over-robe",
		LongDescription = null,
		FullDescription = "This long $colour beizi over-robe is a long over-robe with a straight open front, narrow sleeves, and side slits that let the underlayers move. The cloth falls in vertical lines from shoulder to lower hem, giving the garment a composed outer silhouette. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 92.0m,
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

	public static CatalogueItemDefinition MedievalSongRuJacket() => new()
	{
		Metadata = new("medieval_song_ru_jacket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4012", []),
		Noun = "jacket",
		ShortDescription = "a $colour cross-collar jacket",
		LongDescription = null,
		FullDescription = "This $colour cross-collar jacket is a short cross-collar jacket with straight sleeves, a wrapped front, and a tidy waist-length body. The garment is cut for movement, with the front overlap and cuffs giving it most of its visible shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalSongChangqunSkirt() => new()
	{
		Metadata = new("medieval_song_changqun_skirt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4043", []),
		Noun = "skirt",
		ShortDescription = "a $colour long pleated skirt",
		LongDescription = null,
		FullDescription = "This $colour long pleated skirt is a long skirt with narrow pleats falling from a firm waist edge. The pleats give the cloth movement while keeping the upper line orderly. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalSongTrousers() => new()
	{
		Metadata = new("medieval_song_trousers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4074", []),
		Noun = "trousers",
		ShortDescription = "a pair of $colour cloth trousers",
		LongDescription = null,
		FullDescription = "These $colour cloth trousers are practical legwear with a gathered waist, straight seams, and enough room through the seat for riding or work. The legs narrow enough to sit beneath boots or wraps without excessive bunching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 14.0m,
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

	public static CatalogueItemDefinition MedievalSongClothShoes() => new()
	{
		Metadata = new("medieval_song_cloth_shoes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4105", []),
		Noun = "shoes",
		ShortDescription = "a pair of cloth shoes",
		LongDescription = null,
		FullDescription = "These cloth shoes are light, low footwear with soft uppers and flat soles. The seams are kept close to the edges, and the shape is simple enough to disappear beneath long robes or trousers. The fabric surface is plain and practical, with wear most visible around the toe and heel.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_Shoes",
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

	public static CatalogueItemDefinition MedievalSongOfficialCap() => new()
	{
		Metadata = new("medieval_song_official_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4135", []),
		Noun = "cap",
		ShortDescription = "a black official cap",
		LongDescription = null,
		FullDescription = "This black official cap is a structured headpiece with a dark cloth body and crisp, formal lines. Its shape is controlled and symmetrical, with the edges finished cleanly to hold the cap's profile. The black surface is plain and deliberate, relying on silhouette rather than bright ornament.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150.0,
		Cost = 70.0m,
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

	public static CatalogueItemDefinition MedievalSongHeadscarf() => new()
	{
		Metadata = new("medieval_song_headscarf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4165", []),
		Noun = "headscarf",
		ShortDescription = "a $colour cloth headscarf",
		LongDescription = null,
		FullDescription = "This $colour cloth headscarf is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Kerchief",
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

	public static CatalogueItemDefinition MedievalGoryeoCrossCollarJacket() => new()
	{
		Metadata = new("medieval_goryeo_cross_collar_jacket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4196", []),
		Noun = "jacket",
		ShortDescription = "a $colour cross-collar jacket",
		LongDescription = null,
		FullDescription = "This $colour cross-collar jacket is a short cross-collar jacket with straight sleeves, a wrapped front, and a tidy waist-length body. The garment is cut for movement, with the front overlap and cuffs giving it most of its visible shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalGoryeoBaji() => new()
	{
		Metadata = new("medieval_goryeo_baji", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4227", []),
		Noun = "baji",
		ShortDescription = "a pair of $colour baji trousers",
		LongDescription = null,
		FullDescription = "These $colour baji trousers are loose trousers with a gathered waist, full seat, and narrowing lower legs. The cloth is cut for easy movement beneath a jacket or robe, with plain seams and practical hems. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380.0,
		Cost = 16.0m,
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

	public static CatalogueItemDefinition MedievalGoryeoChima() => new()
	{
		Metadata = new("medieval_goryeo_chima", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4258", []),
		Noun = "chima",
		ShortDescription = "a $colour chima skirt",
		LongDescription = null,
		FullDescription = "This $colour chima skirt is a full skirt with a gathered upper edge and a broad fall from waist to lower hem. The fabric spreads in soft folds and is meant to sit below a short jacket or over-robe. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalGoryeoPoRobe() => new()
	{
		Metadata = new("medieval_goryeo_po_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4289", []),
		Noun = "po",
		ShortDescription = "a $colour po over-robe",
		LongDescription = null,
		FullDescription = "This $colour po over-robe is a long over-robe with broad sleeves, a wrapped front, and a steady fall below the knee. It is cut to sit over a jacket and trousers or skirt, adding a cleaner outer line. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 90.0m,
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

	public static CatalogueItemDefinition MedievalGoryeoSilkOverrobe() => new()
	{
		Metadata = new("medieval_goryeo_silk_overrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4320", []),
		Noun = "over-robe",
		ShortDescription = "a fine $colour silk over-robe",
		LongDescription = null,
		FullDescription = "This fine $colour silk over-robe is a substantial outer robe with padded body panels and long sleeves. The added thickness gives the robe a rounded fall and makes the edges sit away from the underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 650.0,
		Cost = 140.0m,
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

	public static CatalogueItemDefinition MedievalGoryeoHeadcloth() => new()
	{
		Metadata = new("medieval_goryeo_headcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4351", []),
		Noun = "headcloth",
		ShortDescription = "a $colour cloth headcloth",
		LongDescription = null,
		FullDescription = "This $colour cloth headcloth is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 100.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Kerchief",
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

	public static CatalogueItemDefinition MedievalGoryeoFeltHat() => new()
	{
		Metadata = new("medieval_goryeo_felt_hat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4382", []),
		Noun = "hat",
		ShortDescription = "a $colour felt hat",
		LongDescription = null,
		FullDescription = "This $colour felt hat is a shaped felt hat with a rounded crown and a firm lower edge. The felt holds a simple profile, with little ornament beyond the shape and colour of the material. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150.0,
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

	public static CatalogueItemDefinition MedievalJapaneseKosode() => new()
	{
		Metadata = new("medieval_japanese_kosode", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4413", []),
		Noun = "kosode",
		ShortDescription = "a $colour kosode robe",
		LongDescription = null,
		FullDescription = "This $colour kosode robe is a straight-seamed robe with small sleeve openings, a wrapped front, and a narrow collar. The body is made from rectangular panels, giving it a clean fall and visible folded overlap at the chest. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalJapaneseHakama() => new()
	{
		Metadata = new("medieval_japanese_hakama", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4444", []),
		Noun = "hakama",
		ShortDescription = "a pair of $colour hakama",
		LongDescription = null,
		FullDescription = "These $colour hakama are divided skirt-like trousers with deep folds falling from the waist. The legs are broad and straight, giving the garment a formal drape while still allowing movement. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
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

	public static CatalogueItemDefinition MedievalJapaneseSuikan() => new()
	{
		Metadata = new("medieval_japanese_suikan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4475", []),
		Noun = "suikan",
		ShortDescription = "a $colour suikan robe",
		LongDescription = null,
		FullDescription = "This $colour suikan robe is a robe with a straight body, open sleeves, and ties that control the loose outer fall. The panels are arranged to show broad, flat surfaces and a clean drape over underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 600.0,
		Cost = 100.0m,
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

	public static CatalogueItemDefinition MedievalJapaneseHitatare() => new()
	{
		Metadata = new("medieval_japanese_hitatare", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4506", []),
		Noun = "hitatare",
		ShortDescription = "a $colour hitatare robe",
		LongDescription = null,
		FullDescription = "This $colour hitatare robe is a robe with wide sleeves, a squared body, and a practical tied front. The panels are arranged to show broad, flat surfaces and a clean drape over underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 110.0m,
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

	public static CatalogueItemDefinition MedievalJapaneseUchigi() => new()
	{
		Metadata = new("medieval_japanese_uchigi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4537", []),
		Noun = "uchigi",
		ShortDescription = "a fine $colour uchigi robe",
		LongDescription = null,
		FullDescription = "This fine $colour uchigi robe is a formal silk robe with broad sleeves, a straight body, and a soft layered fall. It is meant to sit among other robes, showing most clearly at the collar, sleeve edges, and hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 640.0,
		Cost = 150.0m,
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

	public static CatalogueItemDefinition MedievalJapaneseKariginu() => new()
	{
		Metadata = new("medieval_japanese_kariginu", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4568", []),
		Noun = "kariginu",
		ShortDescription = "a fine $colour kariginu robe",
		LongDescription = null,
		FullDescription = "This fine $colour kariginu robe is a robe with wide sleeves, a loose body, and a formal hunting-robe silhouette. The panels are arranged to show broad, flat surfaces and a clean drape over underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 150.0m,
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

	public static CatalogueItemDefinition MedievalJapaneseEboshi() => new()
	{
		Metadata = new("medieval_japanese_eboshi", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4599", []),
		Noun = "eboshi",
		ShortDescription = "a black eboshi cap",
		LongDescription = null,
		FullDescription = "This black eboshi cap is a tall, folded headpiece with a crisp dark surface and a narrow base. The shape rises cleanly from the head, giving it a distinctive formal silhouette. The black cloth is plain and smooth, with the emphasis on line rather than ornament.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 80.0,
		Cost = 44.0m,
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

	public static CatalogueItemDefinition MedievalJapaneseHempSandals() => new()
	{
		Metadata = new("medieval_japanese_hemp_sandals", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4629", []),
		Noun = "sandals",
		ShortDescription = "a pair of hemp sandals",
		LongDescription = null,
		FullDescription = "These woven hemp sandals are light, open footwear made from braided plant-fibre straps and simple flat soles. The weave is coarse but regular, leaving the toes and much of the foot exposed. The natural hemp surface is plainly visible, with small slubs and a practical, unpolished finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_Sandals",
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

	public static CatalogueItemDefinition MedievalJapaneseHeadcloth() => new()
	{
		Metadata = new("medieval_japanese_headcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4659", []),
		Noun = "headcloth",
		ShortDescription = "a $colour linen headcloth",
		LongDescription = null,
		FullDescription = "This $colour linen headcloth is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Kerchief",
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

	public static CatalogueItemDefinition MedievalWoolOverTunic() => new()
	{
		Metadata = new("medieval_wool_over_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4690", []),
		Noun = "overtunic",
		ShortDescription = "a heavy $colour wool overtunic",
		LongDescription = null,
		FullDescription = "This heavy $colour wool overtunic is cut larger than an under-tunic, with broad sleeves and a weighty skirt for layering. The wool is dense and slightly bulky, making the garment useful over lighter clothing in cold air. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
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
				"Wear_Long-Sleeved_Tunic",
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

	public static CatalogueItemDefinition MedievalLinedWoolHose() => new()
	{
		Metadata = new("medieval_lined_wool_hose", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4721", []),
		Noun = "hose",
		ShortDescription = "a pair of thick $colour wool hose",
		LongDescription = null,
		FullDescription = "These thick $colour wool hose are close-fitting leg coverings made from dense wool, shaped to add warmth beneath outer clothing. The feet and ankles are cut with practical seams, while the upper edges are finished to sit under a tunic or breeches. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Stockings",
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

	public static CatalogueItemDefinition MedievalWoolFootwraps() => new()
	{
		Metadata = new("medieval_wool_footwraps", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4752", []),
		Noun = "footwraps",
		ShortDescription = "a pair of thick $colour wool footwraps",
		LongDescription = null,
		FullDescription = "These thick $colour wool footwraps are long narrow strips of cloth made to wind around the lower legs or feet. The ends are left thin enough to tuck back under the wrapping, while the body of each strip has enough texture to grip itself. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Socks",
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

	public static CatalogueItemDefinition MedievalWoolMittens() => new()
	{
		Metadata = new("medieval_wool_mittens", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4783", []),
		Noun = "mittens",
		ShortDescription = "a pair of $colour wool mittens",
		LongDescription = null,
		FullDescription = "These $colour wool mittens are simple wool hand coverings with a separate thumb and a rounded body for the fingers. The cuffs are long enough to tuck beneath sleeves or cloak folds. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Gloves"
			],
		Components = [
				"Holdable",
				"Wear_Mittens",
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

	public static CatalogueItemDefinition MedievalLeatherWinterMittens() => new()
	{
		Metadata = new("medieval_leather_winter_mittens", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4814", []),
		Noun = "mittens",
		ShortDescription = "a pair of fur-lined leather mittens",
		LongDescription = null,
		FullDescription = "These fur-lined leather mittens are thick hand coverings with leather outside and warm fur visible at the cuff. The thumb is separately shaped, while the main body keeps the fingers together for warmth. Natural variations in the leather and fur show at the seams, cuff, and flexed places.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 180.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Gloves"
			],
		Components = [
				"Holdable",
				"Wear_Mittens",
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

	public static CatalogueItemDefinition MedievalLinedLeatherBoots() => new()
	{
		Metadata = new("medieval_lined_leather_boots", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4844", []),
		Noun = "boots",
		ShortDescription = "a pair of lined leather boots",
		LongDescription = null,
		FullDescription = "These lined leather boots are sturdy outdoor footwear with leather uppers, firm soles, and a warm inner lining. The shafts rise high enough to protect the ankle and lower leg, while the seams are placed away from the worst rubbing points. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 980.0,
		Cost = 56.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_Boots",
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

	public static CatalogueItemDefinition MedievalHeavyHoodedWoolCloak() => new()
	{
		Metadata = new("medieval_heavy_hooded_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4874", []),
		Noun = "cloak",
		ShortDescription = "a heavy $colour hooded wool cloak",
		LongDescription = null,
		FullDescription = "This heavy $colour hooded wool cloak is a hooded outer cloth meant to drape over the shoulders and upper body. The hood is cut as part of the weather-facing shape, falling forward enough to shadow the face without becoming a mask. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1650.0,
		Cost = 46.0m,
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

	public static CatalogueItemDefinition MedievalFurFacedWinterCloak() => new()
	{
		Metadata = new("medieval_fur_faced_winter_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4905", []),
		Noun = "cloak",
		ShortDescription = "a fine $colour fur-faced cloak",
		LongDescription = null,
		FullDescription = "This fine $colour fur-faced cloak is a broad outer cloth meant to drape over the shoulders and upper body. Fur shows at the opening and edges, adding thickness where wind would otherwise slip beneath the outer cloth. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1950.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Closed)",
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

	public static CatalogueItemDefinition MedievalWaxedSkinCape() => new()
	{
		Metadata = new("medieval_waxed_skin_cape", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4936", []),
		Noun = "cape",
		ShortDescription = "a waxed skin cape",
		LongDescription = null,
		FullDescription = "This waxed skin cape is a weather-facing outer layer made from treated animal skin with a broad shoulder fall. The surface has a dull waxy sheen, and the edges are left sturdy rather than decorative. Natural variations in the skin show at the edges, seams, and flexed places.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "animal skin",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cape",
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

	public static CatalogueItemDefinition MedievalHeavyCheckedWoolMantle() => new()
	{
		Metadata = new("medieval_heavy_checked_wool_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4966", []),
		Noun = "mantle",
		ShortDescription = "a heavy $colour1 and $colour2 checked mantle",
		LongDescription = null,
		FullDescription = "This heavy $colour1 and $colour2 checked mantle is a broad outer cloth meant to drape over the shoulders and upper body. The checked weave remains visible across the folds, breaking the broad surface into regular blocks of colour. The $colour1 and $colour2 threads are woven into an even checked pattern, giving the mantle visible contrast without relying on applied ornament.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 42.0m,
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
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_2BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeavyWoolBrat() => new()
	{
		Metadata = new("medieval_heavy_wool_brat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:4997", []),
		Noun = "brat",
		ShortDescription = "a heavy $colour wool brat",
		LongDescription = null,
		FullDescription = "This heavy $colour wool brat is a broad wool cloak-mantle with a rectangular fall and enough weight to hold a strong shoulder drape. The edges are the most visible part, especially where the cloth is folded back from the body. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1400.0,
		Cost = 42.0m,
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

	public static CatalogueItemDefinition MedievalFurTrimmedBrat() => new()
	{
		Metadata = new("medieval_fur_trimmed_brat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5028", []),
		Noun = "brat",
		ShortDescription = "a fine $colour fur-trimmed brat",
		LongDescription = null,
		FullDescription = "This fine $colour fur-trimmed brat is a broad wool cloak-mantle with a rectangular fall and enough weight to hold a strong shoulder drape. The edges are the most visible part, especially where the cloth is folded back from the body. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
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

	public static CatalogueItemDefinition MedievalWoolCowlHood() => new()
	{
		Metadata = new("medieval_wool_cowl_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5059", []),
		Noun = "hood",
		ShortDescription = "a $colour wool cowl hood",
		LongDescription = null,
		FullDescription = "This $colour wool cowl hood is a roomy hood with a deep face opening and a short drape around the shoulders. The wool gathers in heavy folds at the neck, adding warmth where cloaks often leave gaps. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hoodie",
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

	public static CatalogueItemDefinition MedievalHeavyWoolSurcoat() => new()
	{
		Metadata = new("medieval_heavy_wool_surcoat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5090", []),
		Noun = "surcoat",
		ShortDescription = "a heavy $colour wool surcoat",
		LongDescription = null,
		FullDescription = "This heavy $colour wool surcoat is a sleeveless wool overgarment with open sides and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
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
				"Wear_Sleeveless_Tunic",
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

	public static CatalogueItemDefinition MedievalLinedWoolManto() => new()
	{
		Metadata = new("medieval_lined_wool_manto", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5121", []),
		Noun = "manto",
		ShortDescription = "a lined $colour wool manto",
		LongDescription = null,
		FullDescription = "This lined $colour wool manto is a broad mantle with a generous curved or rectangular fall over the shoulders. It is made to frame the upper body and cover the outer garment beneath without close tailoring. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 42.0m,
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

	public static CatalogueItemDefinition MedievalWoolPaenula() => new()
	{
		Metadata = new("medieval_wool_paenula", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5152", []),
		Noun = "paenula",
		ShortDescription = "a hooded $colour wool paenula",
		LongDescription = null,
		FullDescription = "This hooded $colour wool paenula is a closed, weather-facing cloak with a rounded body and an attached hood. The cloth hangs as a broad shell around the shoulders, with the front falling in heavy protective folds. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 38.0m,
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

	public static CatalogueItemDefinition MedievalHeavySagion() => new()
	{
		Metadata = new("medieval_heavy_sagion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5183", []),
		Noun = "sagion",
		ShortDescription = "a heavy $colour sagion cloak",
		LongDescription = null,
		FullDescription = "This heavy $colour sagion cloak is a substantial wool shoulder-cloak with a dense fall and weather-ready weight. It gathers near one shoulder and hangs in broad folds across the body. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1150.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
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

	public static CatalogueItemDefinition MedievalHeavyWoolBurnous() => new()
	{
		Metadata = new("medieval_heavy_wool_burnous", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5214", []),
		Noun = "burnous",
		ShortDescription = "a heavy $colour wool burnous",
		LongDescription = null,
		FullDescription = "This heavy $colour wool burnous is a heavy wool cloak with a hooded upper edge and a loose body meant to wrap around the shoulders. The cloth falls in deep folds and leaves the front easy to draw close against wind or dust. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 42.0m,
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

	public static CatalogueItemDefinition MedievalPaddedWoolJubba() => new()
	{
		Metadata = new("medieval_padded_wool_jubba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5245", []),
		Noun = "jubba",
		ShortDescription = "a padded $colour wool jubba",
		LongDescription = null,
		FullDescription = "This padded $colour wool jubba is a substantial robe with quilted thickness through the body and sleeves. The front hangs open or overlaps in heavy folds, making it suitable as a cool-season outer layer. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 58.0m,
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

	public static CatalogueItemDefinition MedievalWoolIzarWrap() => new()
	{
		Metadata = new("medieval_wool_izar_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5276", []),
		Noun = "izar",
		ShortDescription = "a thick $colour wool izar",
		LongDescription = null,
		FullDescription = "This thick $colour wool izar is a broad thick wool wrap with enough length to pass around the body and fall in layered folds. The edges are plain and straight, letting the drape and overlap form most of the garment's shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
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

	public static CatalogueItemDefinition MedievalFurLinedCaftan() => new()
	{
		Metadata = new("medieval_fur_lined_caftan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5307", []),
		Noun = "caftan",
		ShortDescription = "a fine $colour fur-lined caftan",
		LongDescription = null,
		FullDescription = "This fine $colour fur-lined caftan is a front-opening coat with warm fur visible at the collar, cuffs, and inner edge. The skirt is cut for riding and walking, with side fullness that keeps the garment from binding. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1400.0,
		Cost = 150.0m,
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

	public static CatalogueItemDefinition MedievalFeltWinterDeel() => new()
	{
		Metadata = new("medieval_felt_winter_deel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5338", []),
		Noun = "robe",
		ShortDescription = "a $colour felt-lined steppe robe",
		LongDescription = null,
		FullDescription = "This $colour felt-lined steppe robe is a heavy belted outer robe with a dense inner lining and a front that wraps across the body. The sleeves and skirt are roomy, allowing it to sit over trousers and other cold-weather layers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1350.0,
		Cost = 56.0m,
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

	public static CatalogueItemDefinition MedievalQuiltedWinterTrousers() => new()
	{
		Metadata = new("medieval_quilted_winter_trousers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5369", []),
		Noun = "trousers",
		ShortDescription = "a pair of quilted $colour winter trousers",
		LongDescription = null,
		FullDescription = "These quilted $colour winter trousers are padded legwear stitched in rows to hold the filling evenly. They are cut roomy enough to pull over lighter layers, with reinforced hems where boots would rub. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Trousers",
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

	public static CatalogueItemDefinition MedievalFurEarflapHat() => new()
	{
		Metadata = new("medieval_fur_earflap_hat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5400", []),
		Noun = "hat",
		ShortDescription = "a fur earflap hat",
		LongDescription = null,
		FullDescription = "This fur earflap hat is a warm head covering with soft fur over the crown and flaps that fall to cover the ears. The shape is practical and rounded, with edges made thick enough to block wind. Natural variations in the fur show at the tips, seams, and places where the nap changes direction.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
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

	public static CatalogueItemDefinition MedievalFeltStockings() => new()
	{
		Metadata = new("medieval_felt_stockings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5430", []),
		Noun = "stockings",
		ShortDescription = "a pair of thick felt stockings",
		LongDescription = null,
		FullDescription = "These thick felt stockings are close, warm leg coverings made from dense felt with a soft but compact surface. They are shaped to sit inside boots or beneath trousers, with plain edges and little ornament. The natural felt surface is plainly visible, with a practical, unpolished finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Stockings",
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

	public static CatalogueItemDefinition MedievalWoolShawl() => new()
	{
		Metadata = new("medieval_wool_shawl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5460", []),
		Noun = "shawl",
		ShortDescription = "a thick $colour wool shawl",
		LongDescription = null,
		FullDescription = "This thick $colour wool shawl is a rectangular wrap with enough breadth to cover the shoulders and upper body. The cloth is thick and soft, falling in warm folds when drawn close. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500.0,
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

	public static CatalogueItemDefinition MedievalThickCottonShawl() => new()
	{
		Metadata = new("medieval_thick_cotton_shawl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5491", []),
		Noun = "shawl",
		ShortDescription = "a thick $colour cotton shawl",
		LongDescription = null,
		FullDescription = "This thick $colour cotton shawl is a rectangular wrap with enough breadth to cover the shoulders and upper body. The cloth is thick and soft, falling in warm folds when drawn close. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
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

	public static CatalogueItemDefinition MedievalFineGoatHairShawl() => new()
	{
		Metadata = new("medieval_fine_goat_hair_shawl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5522", []),
		Noun = "shawl",
		ShortDescription = "a fine $colour goat-hair shawl",
		LongDescription = null,
		FullDescription = "This fine $colour goat-hair shawl is a rectangular wrap with enough breadth to cover the shoulders and upper body. The cloth is thick and soft, falling in warm folds when drawn close. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 320.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cashmere",
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

	public static CatalogueItemDefinition MedievalHeavyCottonAngavastram() => new()
	{
		Metadata = new("medieval_heavy_cotton_angavastram", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5553", []),
		Noun = "angavastram",
		ShortDescription = "a thick $colour cotton angavastram",
		LongDescription = null,
		FullDescription = "This thick $colour cotton angavastram is a broad shoulder cloth that can be folded, draped, or drawn around the upper body. The cloth is rectangular and plainly edged, relying on the wearer's wrapping for its final shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
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
				"Wear_Mantle",
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

	public static CatalogueItemDefinition MedievalPaddedSilkOverrobe() => new()
	{
		Metadata = new("medieval_padded_silk_overrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5584", []),
		Noun = "over-robe",
		ShortDescription = "a padded $colour silk over-robe",
		LongDescription = null,
		FullDescription = "This padded $colour silk over-robe is a substantial outer robe with padded body panels and long sleeves. The added thickness gives the robe a rounded fall and makes the edges sit away from the underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
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
				"Wear_Robe",
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

	public static CatalogueItemDefinition MedievalHeavyBeiziOverrobe() => new()
	{
		Metadata = new("medieval_heavy_beizi_overrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5615", []),
		Noun = "beizi",
		ShortDescription = "a heavy $colour beizi over-robe",
		LongDescription = null,
		FullDescription = "This heavy $colour beizi over-robe is a long over-robe with a straight open front, narrow sleeves, and side slits that let the underlayers move. The cloth falls in vertical lines from shoulder to lower hem, giving the garment a composed outer silhouette. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 760.0,
		Cost = 130.0m,
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

	public static CatalogueItemDefinition MedievalPaddedPoRobe() => new()
	{
		Metadata = new("medieval_padded_po_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5646", []),
		Noun = "po",
		ShortDescription = "a padded $colour po robe",
		LongDescription = null,
		FullDescription = "This padded $colour po robe is a long over-robe with broad sleeves, a wrapped front, and a steady fall below the knee. It is cut to sit over a jacket and trousers or skirt, adding a cleaner outer line. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 135.0m,
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

	public static CatalogueItemDefinition MedievalLinedKosode() => new()
	{
		Metadata = new("medieval_lined_kosode", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5677", []),
		Noun = "kosode",
		ShortDescription = "a lined $colour kosode robe",
		LongDescription = null,
		FullDescription = "This lined $colour kosode robe is a straight-seamed robe with small sleeve openings, a wrapped front, and an added lining for warmth. The collar runs in a clean diagonal line, and the sleeves hang in measured rectangles. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
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

	public static CatalogueItemDefinition MedievalLayeredUchigiSet() => new()
	{
		Metadata = new("medieval_layered_uchigi_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5708", []),
		Noun = "uchigi",
		ShortDescription = "a layered $colour uchigi set",
		LongDescription = null,
		FullDescription = "This layered $colour uchigi set is a group of formal silk robe layers with straight panels and broad sleeves. The nested collars and hems create visible tiers, making the colour strongest where several layers overlap. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 220.0m,
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

	public static CatalogueItemDefinition MedievalLinenWorkApron() => new()
	{
		Metadata = new("medieval_linen_work_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5739", []),
		Noun = "apron",
		ShortDescription = "a $colour linen work apron",
		LongDescription = null,
		FullDescription = "This $colour linen work apron is a plain protective cloth with a straight fall over the front of the body. The upper edge and ties are simple, keeping the garment easy to put on over ordinary working clothes. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
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

	public static CatalogueItemDefinition MedievalHempWorkApron() => new()
	{
		Metadata = new("medieval_hemp_work_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5770", []),
		Noun = "apron",
		ShortDescription = "a coarse hemp work apron",
		LongDescription = null,
		FullDescription = "This hemp work apron is a coarse protective layer with a straight bib or waist fall and plain ties. The cloth is sturdy, rough-textured, and meant to take stains before the clothing beneath does. The natural hemp surface is plainly visible, with small slubs and a practical, unpolished finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
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

	public static CatalogueItemDefinition MedievalLinenSleeveGuards() => new()
	{
		Metadata = new("medieval_linen_sleeve_guards", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5802", []),
		Noun = "sleeve guards",
		ShortDescription = "a pair of $colour linen sleeve guards",
		LongDescription = null,
		FullDescription = "These $colour linen sleeve guards are narrow protective wraps for the forearms, made to cover cuffs and keep sleeves close during work. The cloth is light and washable, with simple ties or tucked ends controlling the fit. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 4.0m,
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

	public static CatalogueItemDefinition MedievalLeatherWorkGloves() => new()
	{
		Metadata = new("medieval_leather_work_gloves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5833", []),
		Noun = "gloves",
		ShortDescription = "a pair of leather work gloves",
		LongDescription = null,
		FullDescription = "These leather work gloves are plain hand coverings with sturdy palms, short cuffs, and practical seams between the fingers. The leather is thick enough to protect against abrasion without becoming armour. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Standard Clothing",
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

	public static CatalogueItemDefinition MedievalCleanLinenCoif() => new()
	{
		Metadata = new("medieval_clean_linen_coif", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5863", []),
		Noun = "coif",
		ShortDescription = "a clean $colour linen coif",
		LongDescription = null,
		FullDescription = "This clean $colour linen coif is a close-fitting head covering with rounded crown panels and ties below the jaw or at the nape. The edges are narrow and smooth, keeping the cloth neat beneath a hood, cap, or veil. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 85.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Coif",
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

	public static CatalogueItemDefinition MedievalLinenWorkCap() => new()
	{
		Metadata = new("medieval_linen_work_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5894", []),
		Noun = "cap",
		ShortDescription = "a $colour linen work cap",
		LongDescription = null,
		FullDescription = "This $colour linen work cap is a simple cloth cap with a rounded crown and a narrow lower edge. It is cut for everyday wear, sitting close to the head without broad brim or heavy ornament. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 6.0m,
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

	public static CatalogueItemDefinition MedievalWideWorkSash() => new()
	{
		Metadata = new("medieval_wide_work_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5925", []),
		Noun = "sash",
		ShortDescription = "a wide $colour work sash",
		LongDescription = null,
		FullDescription = "This wide $colour work sash is a broad wool band meant to gather loose clothing close to the body. The weave is firm rather than delicate, with squared ends and enough breadth to spread pressure across the waist. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 8.0m,
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

	public static CatalogueItemDefinition MedievalCanvasWorkTabard() => new()
	{
		Metadata = new("medieval_canvas_work_tabard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5956", []),
		Noun = "tabard",
		ShortDescription = "a $colour canvas work tabard",
		LongDescription = null,
		FullDescription = "This $colour canvas work tabard is a loose sleeveless canvas overgarment with open sides and a straight fall from the shoulders. It is cut to protect the front and back of clothing while leaving the arms free for work. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tabard",
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

	public static CatalogueItemDefinition MedievalDrabDyersApron() => new()
	{
		Metadata = new("medieval_drab_dyers_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:5987", []),
		Noun = "apron",
		ShortDescription = "a stained $colour dye-worker's apron",
		LongDescription = null,
		FullDescription = "This stained $colour dye-worker's apron is a linen work layer marked by uneven colour and old splashes in the cloth. It has a plain protective fall and simple ties, prioritising coverage over neat display. The $colour tone is uneven and work-stained, strongest along the creases, edges, and places most handled.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
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
				"Variable_DrabColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShortWoolWorkTunic() => new()
	{
		Metadata = new("medieval_short_wool_work_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6018", []),
		Noun = "tunic",
		ShortDescription = "a short $colour wool work tunic",
		LongDescription = null,
		FullDescription = "This short $colour wool work tunic is a short wool garment made from straight body panels with sleeves set in simply at the shoulder. The proportions are tidier than a field garment, with the hem kept clear enough for work in streets or workshops. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 440.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tunic",
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

	public static CatalogueItemDefinition MedievalCleanTownWoolTunic() => new()
	{
		Metadata = new("medieval_clean_town_wool_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6049", []),
		Noun = "tunic",
		ShortDescription = "a neat $colour town tunic",
		LongDescription = null,
		FullDescription = "This neat $colour town tunic is a straight wool garment made from straight body panels with sleeves set in simply at the shoulder. The proportions are tidier than a field garment, with the hem kept clear enough for work in streets or workshops. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tunic",
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

	public static CatalogueItemDefinition MedievalTuckedWoolWorkGown() => new()
	{
		Metadata = new("medieval_tucked_wool_work_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:6080", []),
		Noun = "gown",
		ShortDescription = "a tucked $colour work gown",
		LongDescription = null,
		FullDescription = "This tucked $colour work gown is a long wool garment with a skirt that can be tucked up clear of work. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
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
				"Wear_Gown",
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
}
