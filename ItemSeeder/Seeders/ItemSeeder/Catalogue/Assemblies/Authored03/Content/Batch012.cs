#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Authored03Stock
{
















































































	public static CatalogueItemDefinition MedievalFoodCoarseBreadLoaf() => new()
	{
		Metadata = new("medieval_food_coarse_bread_loaf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "loaf",
		ShortDescription = "a coarse brown bread loaf",
		LongDescription = null,
		FullDescription = "This heavy round loaf has a dark, flour-dusted crust and a dense crumb flecked with coarse meal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bread",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_Bread"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodFlatbread() => new()
	{
		Metadata = new("medieval_food_flatbread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "bread",
		ShortDescription = "a broad griddle-baked flatbread",
		LongDescription = null,
		FullDescription = "This broad, flexible flatbread is browned in patches and carries faint flour and smoke marks from a hot griddle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bread",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_Bread"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodHardBread() => new()
	{
		Metadata = new("medieval_food_hard_bread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "bread",
		ShortDescription = "a hard travel bread",
		LongDescription = null,
		FullDescription = "This twice-baked slab of bread is pale, dry, and hard enough to keep through a long journey when stored away from damp.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bread",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_HardBread"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodGrainPottage() => new()
	{
		Metadata = new("medieval_food_grain_pottage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "pottage",
		ShortDescription = "a bowlful of thick grain pottage",
		LongDescription = null,
		FullDescription = "This thick spoonable pottage is made from simmered coarse grain, with a soft texture and a plain toasted aroma.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bread",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_Pottage"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodMeatPottage() => new()
	{
		Metadata = new("medieval_food_meat_pottage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "pottage",
		ShortDescription = "a bowlful of meat and grain pottage",
		LongDescription = null,
		FullDescription = "This rich pottage combines softened grain with small pieces of cooked meat in a thick savoury broth.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "meat",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_Pottage"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodSaltedMeatRation() => new()
	{
		Metadata = new("medieval_food_salted_meat_ration", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "ration",
		ShortDescription = "a ration of salted meat",
		LongDescription = null,
		FullDescription = "These firm cuts of meat have been heavily salted and packed together as a compact stored ration.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "meat",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_PreservedProvision"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodDriedMeatRation() => new()
	{
		Metadata = new("medieval_food_dried_meat_ration", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "ration",
		ShortDescription = "a ration of dried meat",
		LongDescription = null,
		FullDescription = "These dark strips of lean meat have been dried until tough and light, making a durable travelling ration.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "meat",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_PreservedProvision"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodSmokedMeatRation() => new()
	{
		Metadata = new("medieval_food_smoked_meat_ration", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "ration",
		ShortDescription = "a ration of smoked meat",
		LongDescription = null,
		FullDescription = "These browned cuts of meat carry a dark smoky rind and have been bundled as a compact preserved ration.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 430,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "meat",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_PreservedProvision"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodSaltedFishRation() => new()
	{
		Metadata = new("medieval_food_salted_fish_ration", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "ration",
		ShortDescription = "a ration of salted fish",
		LongDescription = null,
		FullDescription = "These firm fish pieces have been packed heavily with salt and bundled as a compact stored ration.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "fish",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_PreservedProvision"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodDriedFishRation() => new()
	{
		Metadata = new("medieval_food_dried_fish_ration", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "ration",
		ShortDescription = "a ration of dried fish",
		LongDescription = null,
		FullDescription = "These split fish pieces have been air-dried until light and leathery, ready for travel or later soaking.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 340,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "fish",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_PreservedProvision"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFoodSmokedFishRation() => new()
	{
		Metadata = new("medieval_food_smoked_fish_ration", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:162", []),
		Noun = "ration",
		ShortDescription = "a ration of smoked fish",
		LongDescription = null,
		FullDescription = "These fish pieces have a golden-brown surface and a strong woodsmoke aroma from slow curing above a low fire.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 390,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "fish",
		Tags = ["Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "PreparedFood_Medieval_PreservedProvision"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIndustryStockPlankBundle() => new()
	{
		Metadata = new("medieval_industry_stock_plank_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalComponentGaps.cs:75", []),
		Noun = "bundle",
		ShortDescription = "a bundle of worked planks",
		LongDescription = null,
		FullDescription = "This cord-tied bundle holds straight worked planks with squared edges and saw marks, ready for chests, doors, shelves, shields, or furniture frames.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000,
		Cost = 24.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Market / Raw Materials", "Market / Construction Materials", "Functions / Material Functions / Household Craft Stock / Furniture Timber Stock"],
		Components = ["Holdable", "Destroyable_Furniture"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIndustryStockTimberBeam() => new()
	{
		Metadata = new("medieval_industry_stock_timber_beam", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalComponentGaps.cs:75", []),
		Noun = "beam",
		ShortDescription = "a squared timber beam",
		LongDescription = null,
		FullDescription = "This heavy timber beam has been hewn and roughly squared for furniture, doors, frames, presses, or workshop apparatus.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000,
		Cost = 30.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Market / Raw Materials", "Market / Construction Materials", "Functions / Material Functions / Household Craft Stock / Furniture Timber Stock"],
		Components = ["Holdable", "Destroyable_Furniture"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIndustryStockShaftBlanks() => new()
	{
		Metadata = new("medieval_industry_stock_shaft_blanks", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalComponentGaps.cs:75", []),
		Noun = "bundle",
		ShortDescription = "a bundle of shaft blanks",
		LongDescription = null,
		FullDescription = "These long ash blanks have been split, shaved, and roughly straightened for tool handles, arrows, spears, poles, or furniture pegs.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3200,
		Cost = 18.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = ["Market / Raw Materials", "Functions / Material Functions / Antiquity Equipment Stock / Weapon Shaft Stock"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIndustryStockBowStave() => new()
	{
		Metadata = new("medieval_industry_stock_bow_stave", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalComponentGaps.cs:75", []),
		Noun = "stave",
		ShortDescription = "a seasoned bow stave",
		LongDescription = null,
		FullDescription = "This long ash stave has been split and seasoned until the grain runs cleanly along its length, ready for bowyer work.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100,
		Cost = 42.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = ["Market / Raw Materials", "Functions / Material Functions / Antiquity Equipment Stock / Bow Stave"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIndustryStockShieldBoard() => new()
	{
		Metadata = new("medieval_industry_stock_shield_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalComponentGaps.cs:75", []),
		Noun = "board",
		ShortDescription = "a shield-board blank",
		LongDescription = null,
		FullDescription = "This joined wooden board is roughly shaped for a shield face, with the central boss space still uncut and the surface unfinished.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600,
		Cost = 28.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Market / Raw Materials", "Functions / Material Functions / Antiquity Equipment Stock / Shield Board Stock"],
		Components = ["Holdable", "Destroyable_Furniture"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIndustryStockHandleBlanks() => new()
	{
		Metadata = new("medieval_industry_stock_handle_blanks", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalComponentGaps.cs:75", []),
		Noun = "bundle",
		ShortDescription = "a bundle of handle blanks",
		LongDescription = null,
		FullDescription = "Short pieces of ash and beech have been squared and shaved into rough handle blanks for knives, files, chisels, hammers, and small tools.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900,
		Cost = 10.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = ["Market / Raw Materials", "Functions / Material Functions / Household Craft Stock / Carved Wood Stock"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIndustryStockYarnSkein() => new()
	{
		Metadata = new("medieval_industry_stock_yarn_skein", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalComponentGaps.cs:75", []),
		Noun = "skein",
		ShortDescription = "a skein of spun yarn",
		LongDescription = null,
		FullDescription = "This skein of clean spun yarn is looped and tied so it can be dyed, traded, or wound for weaving.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 250,
		Cost = 8.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = ["Market / Raw Materials / Cloth", "Functions / Material Functions / Textile Commodity / Spun Yarn"],
		Components = ["Holdable", "Destroyable_Clothing"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIndustryStockSewingThread() => new()
	{
		Metadata = new("medieval_industry_stock_sewing_thread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalComponentGaps.cs:75", []),
		Noun = "spool",
		ShortDescription = "a spool of sewing thread",
		LongDescription = null,
		FullDescription = "This small spool carries tightly wound linen thread for garments, books, pouches, and light leather work.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = ["Market / Raw Materials / Cloth", "Functions / Material Functions / Textile Commodity / Spun Yarn"],
		Components = ["Holdable", "Destroyable_Clothing"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIndustryStockCordBundle() => new()
	{
		Metadata = new("medieval_industry_stock_cord_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalComponentGaps.cs:75", []),
		Noun = "bundle",
		ShortDescription = "a bundle of twisted cord",
		LongDescription = null,
		FullDescription = "Several lengths of twisted hemp cord are looped and tied for lacing, binding, scrolls, stitching heavy goods, or small fittings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 8.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = ["Market / Raw Materials", "Functions / Material Functions / String"],
		Components = ["Holdable", "Destroyable_Clothing"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};
}
