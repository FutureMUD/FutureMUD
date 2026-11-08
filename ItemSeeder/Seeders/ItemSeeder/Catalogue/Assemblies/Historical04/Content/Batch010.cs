#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalFurnitureSimpleCradle() => new()
	{
		Metadata = new("medieval_furniture_simple_cradle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4673", []),
		Noun = "cradle",
		ShortDescription = "a simple wooden cradle",
		LongDescription = null,
		FullDescription = "This simple wooden cradle is a large, workmanlike cradle built from pine boards. A small sleeping box sits on curved rockers, with low sides and rounded ends. The inner surface is smooth enough for blankets. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8000.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small rocking-style infant bed represented as a narrow cot surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSimpleHallBench() => new()
	{
		Metadata = new("medieval_furniture_simple_hall_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4702", []),
		Noun = "bench",
		ShortDescription = "a simple hall bench",
		LongDescription = null,
		FullDescription = "This simple hall bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15000.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Three-seat bench with a surface that can support placed items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSimpleRopeBed() => new()
	{
		Metadata = new("medieval_furniture_simple_rope_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4732", []),
		Noun = "bed",
		ShortDescription = "a simple rope bed",
		LongDescription = null,
		FullDescription = "This simple rope bed is a very large, workmanlike bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42000.0,
		Cost = 100.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wooden bed frame with a rope-slung surface; uses bed-surface container behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSimpleSideboardTable() => new()
	{
		Metadata = new("medieval_furniture_simple_sideboard_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4761", []),
		Noun = "table",
		ShortDescription = "a plain sideboard table",
		LongDescription = null,
		FullDescription = "This plain sideboard table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26000.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long open table used like a sideboard surface without enclosed storage behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSimpleStool() => new()
	{
		Metadata = new("medieval_furniture_simple_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4791", []),
		Noun = "stool",
		ShortDescription = "a simple wooden stool",
		LongDescription = null,
		FullDescription = "This simple wooden stool is a small, workmanlike stool built from pine boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2200.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small single-person stool for household, kitchen, workshop, or stable use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSimpleWritingBench() => new()
	{
		Metadata = new("medieval_furniture_simple_writing_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4820", []),
		Noun = "bench",
		ShortDescription = "a narrow writing bench",
		LongDescription = null,
		FullDescription = "This narrow writing bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Two-seat bench useful with writing tables, teaching tables, or work desks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSlopedWritingDesk() => new()
	{
		Metadata = new("medieval_furniture_sloped_writing_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4850", []),
		Noun = "desk",
		ShortDescription = "a sloped writing desk",
		LongDescription = null,
		FullDescription = "This sloped writing desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 21000.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Writing desk with a visible sloped top and a surface suited to manuscripts or tablets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSquareWorkTable() => new()
	{
		Metadata = new("medieval_furniture_square_work_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4880", []),
		Noun = "table",
		ShortDescription = "a square wooden work table",
		LongDescription = null,
		FullDescription = "This square wooden work table is a large, workmanlike table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 17000.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy four-place work and preparation table with a broad flat surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureStewardsTable() => new()
	{
		Metadata = new("medieval_furniture_stewards_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4910", []),
		Noun = "table",
		ShortDescription = "a steward's service table",
		LongDescription = null,
		FullDescription = "This steward's service table is a large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 23000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Well-made service and account table for household officers, stewards, and clerks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureStoutWorkChair() => new()
	{
		Metadata = new("medieval_furniture_stout_work_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4940", []),
		Noun = "chair",
		ShortDescription = "a stout work chair",
		LongDescription = null,
		FullDescription = "This stout work chair is a medium-sized, workmanlike chair built from beech boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8000.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy practical chair for workshops, kitchens, guardrooms, and service spaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureStuffedLinenPallet() => new()
	{
		Metadata = new("medieval_furniture_stuffed_linen_pallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4969", []),
		Noun = "pallet",
		ShortDescription = "a stuffed linen pallet",
		LongDescription = null,
		FullDescription = "This stuffed linen pallet is a large, workmanlike pallet made from woven linen. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6500.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft stuffed pallet for sleeping on floors, platforms, or simple frames.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureTallDeskStool() => new()
	{
		Metadata = new("medieval_furniture_tall_desk_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4998", []),
		Noun = "stool",
		ShortDescription = "a tall desk stool",
		LongDescription = null,
		FullDescription = "This tall desk stool is a medium-sized, workmanlike stool built from oak boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Taller single stool for high tables, counters, and desk surfaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureTallReadingStand() => new()
	{
		Metadata = new("medieval_furniture_tall_reading_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5027", []),
		Noun = "stand",
		ShortDescription = "a tall reading stand",
		LongDescription = null,
		FullDescription = "This tall reading stand is a large, well-made stand built from ash boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 9000.0,
		Cost = 75.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tall free-standing reading surface for proclamations, lessons, accounts, or recitation.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureTallWorkTable() => new()
	{
		Metadata = new("medieval_furniture_tall_work_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5056", []),
		Noun = "table",
		ShortDescription = "a tall work table",
		LongDescription = null,
		FullDescription = "This tall work table is a large, workmanlike table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Raised work surface for standing tasks, measuring, cutting, sorting, or laying out tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureTallyTableDesk() => new()
	{
		Metadata = new("medieval_furniture_tally_table_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5086", []),
		Noun = "desk",
		ShortDescription = "a tally-marked desk",
		LongDescription = null,
		FullDescription = "This tally-marked desk is a large, workmanlike desk built from beech boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Desk top scarred with tally-like marks and ordinary clerkly wear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureTavernCommonTable() => new()
	{
		Metadata = new("medieval_furniture_tavern_common_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5116", []),
		Noun = "table",
		ShortDescription = "a stained tavern table",
		LongDescription = null,
		FullDescription = "This stained tavern table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 21000.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough six-place common table for inns, alehouses, guardrooms, and servants' halls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureThickSleepingMat() => new()
	{
		Metadata = new("medieval_furniture_thick_sleeping_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5146", []),
		Noun = "mat",
		ShortDescription = "a thick sleeping mat",
		LongDescription = null,
		FullDescription = "This thick sleeping mat is a large, workmanlike mat made from coarse jute. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5400.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jute",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Coarse thick sleeping mat for simple household or camp sleeping arrangements.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureThreeLeggedStool() => new()
	{
		Metadata = new("medieval_furniture_three_legged_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5175", []),
		Noun = "stool",
		ShortDescription = "a three-legged stool",
		LongDescription = null,
		FullDescription = "This three-legged stool is a small, workmanlike stool built from ash boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small three-legged stool intended to sit firmly on uneven floors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureThreeLeggedTable() => new()
	{
		Metadata = new("medieval_furniture_three_legged_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5204", []),
		Noun = "table",
		ShortDescription = "a three-legged small table",
		LongDescription = null,
		FullDescription = "This three-legged small table is a medium-sized, workmanlike table built from elm boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6800.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "elm",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact small table with a three-legged stance suitable for uneven floors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureTiltedDraftingTable() => new()
	{
		Metadata = new("medieval_furniture_tilted_drafting_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5234", []),
		Noun = "table",
		ShortDescription = "a tilted drafting table",
		LongDescription = null,
		FullDescription = "This tilted drafting table is a large, well-made table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 21000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sloped table for plans, patterns, diagrams, maps, and careful writing work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureTrayStandTable() => new()
	{
		Metadata = new("medieval_furniture_tray_stand_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5264", []),
		Noun = "stand",
		ShortDescription = "a folding tray stand",
		LongDescription = null,
		FullDescription = "This folding tray stand is a medium-sized, workmanlike stand built from ash boards. A rectangular top rests on hinged legs that fold beneath it. The joints are pinned and rubbed bright where the frame moves. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5000.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light stand-like table for holding a tray, ewer, platter, or small household vessel.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureTrestleBench() => new()
	{
		Metadata = new("medieval_furniture_trestle_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5294", []),
		Noun = "bench",
		ShortDescription = "a trestle hall bench",
		LongDescription = null,
		FullDescription = "This trestle hall bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 13000.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable-looking trestle bench with a long plank seat and simple supports.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureTrestleDiningTable() => new()
	{
		Metadata = new("medieval_furniture_trestle_dining_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5324", []),
		Noun = "table",
		ShortDescription = "a trestle dining table",
		LongDescription = null,
		FullDescription = "This trestle dining table is a very large, workmanlike table built from oak boards. A long board top rests across two trestle supports, with simple pegs holding the parts in line. The underside shows scuffs from assembly and storage. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30000.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Eight",
				"Container_Large_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Eight-place trestle dining table with a removable-looking plank top and broad surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWashTable() => new()
	{
		Metadata = new("medieval_furniture_wash_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5354", []),
		Noun = "table",
		ShortDescription = "a plain wash table",
		LongDescription = null,
		FullDescription = "This plain wash table is a large, workmanlike table built from elm boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18500.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "elm",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Household wash table sized for basins, ewers, cloths, and other washing goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWickerCradle() => new()
	{
		Metadata = new("medieval_furniture_wicker_cradle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5384", []),
		Noun = "cradle",
		ShortDescription = "a woven willow cradle",
		LongDescription = null,
		FullDescription = "This woven willow cradle is a large, workmanlike cradle built from willow boards. A small sleeping box sits on curved rockers, with low sides and rounded ends. The inner surface is smooth enough for blankets. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4500.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light woven cradle with a narrow surface suitable for bedding.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWideBedFrame() => new()
	{
		Metadata = new("medieval_furniture_wide_bed_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5413", []),
		Noun = "bed",
		ShortDescription = "a wide wooden bed",
		LongDescription = null,
		FullDescription = "This wide wooden bed is a huge, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 64000.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wide bed frame with a large surface for bedding and placed objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWindowBench() => new()
	{
		Metadata = new("medieval_furniture_window_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5442", []),
		Noun = "bench",
		ShortDescription = "a narrow window bench",
		LongDescription = null,
		FullDescription = "This narrow window bench is a large, workmanlike bench built from ash boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow two-seat bench intended for a wall, window bay, or chamber side.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWoodenDaybedSeat() => new()
	{
		Metadata = new("medieval_furniture_wooden_daybed_seat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5472", []),
		Noun = "daybed",
		ShortDescription = "a wooden daybed seat",
		LongDescription = null,
		FullDescription = "This wooden daybed seat is a large, well-made daybed built from oak boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Raised three-person daybed-like seat with a couch surface, distinct from enclosed bed storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWoolBeddingRoll() => new()
	{
		Metadata = new("medieval_furniture_wool_bedding_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5502", []),
		Noun = "bedroll",
		ShortDescription = "a rolled wool bedroll",
		LongDescription = null,
		FullDescription = "This rolled wool bedroll is a medium-sized, workmanlike bedroll made from woven wool. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable bedding roll represented as a narrow cot-like surface when set down.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWoolSittingMat() => new()
	{
		Metadata = new("medieval_furniture_wool_sitting_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5531", []),
		Noun = "mat",
		ShortDescription = "a wool sitting mat",
		LongDescription = null,
		FullDescription = "This wool sitting mat is a medium-sized, workmanlike mat made from woven wool. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2500.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Chair_Double"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft two-person floor-seating mat using chair behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWoolSleepingPallet() => new()
	{
		Metadata = new("medieval_furniture_wool_sleeping_pallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5560", []),
		Noun = "pallet",
		ShortDescription = "a wool sleeping pallet",
		LongDescription = null,
		FullDescription = "This wool sleeping pallet is a large, workmanlike pallet made from woven wool. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7600.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Warmer padded sleeping pallet with a narrow surface profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWorkStool() => new()
	{
		Metadata = new("medieval_furniture_work_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5589", []),
		Noun = "stool",
		ShortDescription = "a sturdy work stool",
		LongDescription = null,
		FullDescription = "This sturdy work stool is a medium-sized, workmanlike stool built from oak boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain robust stool for workshops, kitchens, barns, and servants' rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernBoxBedFrame() => new()
	{
		Metadata = new("medieval_furniture_northern_box_bed_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5618", []),
		Noun = "bed",
		ShortDescription = "a boxed timber bed",
		LongDescription = null,
		FullDescription = "This boxed timber bed is a very large, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 56000.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy timber bed with boxed sides and a broad surface; no storage behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernCarvedChestTable() => new()
	{
		Metadata = new("medieval_furniture_northern_carved_chest_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5647", []),
		Noun = "table",
		ShortDescription = "a carved end table",
		LongDescription = null,
		FullDescription = "This carved end table is a medium-sized, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 9000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Side_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Carved side table for a high seat, chamber, or hall corner; no storage behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernCarvedHighSeat() => new()
	{
		Metadata = new("medieval_furniture_northern_carved_high_seat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5677", []),
		Noun = "seat",
		ShortDescription = "a carved northern high seat",
		LongDescription = null,
		FullDescription = "This carved northern high seat is a large, well-made seat built from oak boards. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Formal carved high seat for a northern hall or dais; public wording remains broad.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernFurSleepingBench() => new()
	{
		Metadata = new("medieval_furniture_northern_fur_sleeping_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5706", []),
		Noun = "bench",
		ShortDescription = "a fur-covered sleeping bench",
		LongDescription = null,
		FullDescription = "This fur-covered sleeping bench is a large, well-made bench made from dressed fur. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 12000.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long padded bench-seat with fur covering and couch-surface behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernHeavyWorkbench() => new()
	{
		Metadata = new("medieval_furniture_northern_heavy_workbench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5736", []),
		Noun = "table",
		ShortDescription = "a heavy timber workbench",
		LongDescription = null,
		FullDescription = "This heavy timber workbench is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 32000.0,
		Cost = 75.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy bench-height work table for timber halls and workshops.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernLongFireBench() => new()
	{
		Metadata = new("medieval_furniture_northern_long_fire_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5766", []),
		Noun = "bench",
		ShortDescription = "a long fireside bench",
		LongDescription = null,
		FullDescription = "This long fireside bench is a very large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 23000.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy bench for a hearth wall, hall fire, or long-house style interior.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernNarrowBunk() => new()
	{
		Metadata = new("medieval_furniture_northern_narrow_bunk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5796", []),
		Noun = "bunk",
		ShortDescription = "a narrow plank bunk",
		LongDescription = null,
		FullDescription = "This narrow plank bunk is a large, workmanlike bunk built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 17000.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow plank sleeping surface for ships, guardrooms, or crowded household spaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernPlankHallBench() => new()
	{
		Metadata = new("medieval_furniture_northern_plank_hall_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5825", []),
		Noun = "bench",
		ShortDescription = "a plank-built hall bench",
		LongDescription = null,
		FullDescription = "This plank-built hall bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14000.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long simple bench with plank construction suited to timber halls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernShipboardTable() => new()
	{
		Metadata = new("medieval_furniture_northern_shipboard_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5855", []),
		Noun = "table",
		ShortDescription = "a pegged shipboard table",
		LongDescription = null,
		FullDescription = "This pegged shipboard table is a large, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14000.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact pegged table suited to ships, camps, and small timber rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernTrestleFeastTable() => new()
	{
		Metadata = new("medieval_furniture_northern_trestle_feast_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5885", []),
		Noun = "table",
		ShortDescription = "a long plank feast table",
		LongDescription = null,
		FullDescription = "This long plank feast table is a huge, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 62000.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Twenty",
				"Container_Large_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large plank table for a northern hall, feast, or assembly.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernTripodStool() => new()
	{
		Metadata = new("medieval_furniture_northern_tripod_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5915", []),
		Noun = "stool",
		ShortDescription = "a carved tripod stool",
		LongDescription = null,
		FullDescription = "This carved tripod stool is a small, workmanlike stool built from ash boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Carved three-legged stool suitable for hall, workshop, or hearth use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNorthernWoolSleepingRoll() => new()
	{
		Metadata = new("medieval_furniture_northern_wool_sleeping_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5944", []),
		Noun = "bedroll",
		ShortDescription = "a checked wool bedroll",
		LongDescription = null,
		FullDescription = "This checked wool bedroll is a medium-sized, workmanlike bedroll made from woven wool. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4400.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Warm wool bedding roll for northern travel and hall sleeping.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternAccountingDesk() => new()
	{
		Metadata = new("medieval_furniture_western_accounting_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:5973", []),
		Noun = "desk",
		ShortDescription = "a western account desk",
		LongDescription = null,
		FullDescription = "This western account desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 23000.0,
		Cost = 165.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Good-quality desk for accounts, letters, seals, and household or guild paperwork.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternCanopyBedFrame() => new()
	{
		Metadata = new("medieval_furniture_western_canopy_bed_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6003", []),
		Noun = "bed",
		ShortDescription = "a posted canopy bed",
		LongDescription = null,
		FullDescription = "This posted canopy bed is a huge, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 76000.0,
		Cost = 280.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large posted bed intended for hangings or curtains; no curtain mechanics are implied.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternCarvedSettle() => new()
	{
		Metadata = new("medieval_furniture_western_carved_settle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6032", []),
		Noun = "settle",
		ShortDescription = "a carved hall settle",
		LongDescription = null,
		FullDescription = "This carved hall settle is a large, well-made settle built from oak boards. A long seat is set below a high back, with enclosed ends that make it feel sheltered. The sitting edge is rounded from use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 28000.0,
		Cost = 155.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "High-backed carved three-seat settle for halls, solars, and chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternJoinedArmchair() => new()
	{
		Metadata = new("medieval_furniture_western_joined_armchair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6062", []),
		Noun = "chair",
		ShortDescription = "a joined armchair",
		LongDescription = null,
		FullDescription = "This joined armchair is a large, well-made chair built from walnut boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 13500.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine single chair with arms and close-fitted joinery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternJoinedDiningTable() => new()
	{
		Metadata = new("medieval_furniture_western_joined_dining_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6091", []),
		Noun = "table",
		ShortDescription = "a joined dining table",
		LongDescription = null,
		FullDescription = "This joined dining table is a large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Well-joined six-place dining table for townhouses, manors, and guild halls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternPaddedBench() => new()
	{
		Metadata = new("medieval_furniture_western_padded_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6121", []),
		Noun = "bench",
		ShortDescription = "a linen-padded bench",
		LongDescription = null,
		FullDescription = "This linen-padded bench is a large, well-made bench made from woven linen. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 11500.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bench_Double",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Two-seat padded bench for a chamber, solar, or refined hall corner.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternRefectoryTable() => new()
	{
		Metadata = new("medieval_furniture_western_refectory_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6151", []),
		Noun = "table",
		ShortDescription = "a refectory-style table",
		LongDescription = null,
		FullDescription = "This refectory-style table is a very large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Twelve",
				"Container_Large_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long communal table for halls, schools, dormitories, and institutional dining.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternSlopedBookStand() => new()
	{
		Metadata = new("medieval_furniture_western_sloped_book_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6181", []),
		Noun = "stand",
		ShortDescription = "a sloped book stand",
		LongDescription = null,
		FullDescription = "This sloped book stand is a medium-sized, workmanlike stand built from oak boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Angled wooden support for books, ledgers, tablets, and papers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternTallStool() => new()
	{
		Metadata = new("medieval_furniture_western_tall_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6210", []),
		Noun = "stool",
		ShortDescription = "a tall joined stool",
		LongDescription = null,
		FullDescription = "This tall joined stool is a medium-sized, well-made stool built from oak boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Well-joined tall stool for counters, desks, and high tables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternTrestleBenchPair() => new()
	{
		Metadata = new("medieval_furniture_western_trestle_bench_pair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6239", []),
		Noun = "bench",
		ShortDescription = "a trestle dining bench",
		LongDescription = null,
		FullDescription = "This trestle dining bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14000.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long bench suited to a trestle table or formal hall service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternTruckleBed() => new()
	{
		Metadata = new("medieval_furniture_western_truckle_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6269", []),
		Noun = "bed",
		ShortDescription = "a low truckle bed",
		LongDescription = null,
		FullDescription = "This low truckle bed is a large, workmanlike bed built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low narrow bed surface that can sit beneath or beside a larger bed.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureWesternWainscotChair() => new()
	{
		Metadata = new("medieval_furniture_western_wainscot_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6298", []),
		Noun = "chair",
		ShortDescription = "a panel-backed chair",
		LongDescription = null,
		FullDescription = "This panel-backed chair is a large, well-made chair built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 13000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Single chair with a broad panel-like back and formal presence.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanBasinStand() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_basin_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6327", []),
		Noun = "stand",
		ShortDescription = "a basin-height table stand",
		LongDescription = null,
		FullDescription = "This basin-height table stand is a medium-sized, workmanlike stand built from olive wood. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7000.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "olive wood",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small stand-like table for basins, ewers, lamps, or chamber washing goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanBenchCouch() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_bench_couch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6357", []),
		Noun = "bench",
		ShortDescription = "a low couch-bench",
		LongDescription = null,
		FullDescription = "This low couch-bench is a large, well-made bench built from cypress boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 19000.0,
		Cost = 105.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Two-seat low bench with couch-like proportions and refined joinery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanBronzeFittedStool() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_bronze_fitted_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6387", []),
		Noun = "stool",
		ShortDescription = "a bronze-fitted stool",
		LongDescription = null,
		FullDescription = "This bronze-fitted stool is a medium-sized, well-made stool built from walnut boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Single stool with visible bronze fittings; primary material remains wood.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanCourtyardBench() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_courtyard_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6416", []),
		Noun = "bench",
		ShortDescription = "a courtyard wooden bench",
		LongDescription = null,
		FullDescription = "This courtyard wooden bench is a large, workmanlike bench built from cypress boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 13000.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain three-seat bench suited to courtyards, shaded rooms, and outdoor household spaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanCypressTable() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_cypress_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6446", []),
		Noun = "table",
		ShortDescription = "a cypress dining table",
		LongDescription = null,
		FullDescription = "This cypress dining table is a large, well-made table built from cypress boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 21000.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished cypress table for warm-climate dining rooms, courtyards, or chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanDaybed() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_daybed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6476", []),
		Noun = "daybed",
		ShortDescription = "a cypress daybed",
		LongDescription = null,
		FullDescription = "This cypress daybed is a very large, well-made daybed built from cypress boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 32000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low daybed or sleeping frame for warm chambers and covered verandas.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanFoldedChair() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_folded_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6505", []),
		Noun = "chair",
		ShortDescription = "a folding frame chair",
		LongDescription = null,
		FullDescription = "This folding frame chair is a medium-sized, well-made chair built from walnut boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5600.0,
		Cost = 75.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable folding-frame single chair for travel, halls, or formal chamber use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanLowCouch() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_low_couch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6534", []),
		Noun = "couch",
		ShortDescription = "a low wooden dining couch",
		LongDescription = null,
		FullDescription = "This low wooden dining couch is a large, well-made couch built from olive wood. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 25000.0,
		Cost = 145.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "olive wood",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low couch-like seat for reclining, day use, or chamber seating.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanLowGuestBed() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_low_guest_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6564", []),
		Noun = "bed",
		ShortDescription = "a low guest bed",
		LongDescription = null,
		FullDescription = "This low guest bed is a very large, workmanlike bed built from olive wood. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "olive wood",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low bed surface for guest chambers, urban houses, and courtyard dwellings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanTileTopTable() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_tile_top_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6593", []),
		Noun = "table",
		ShortDescription = "a tiled-top table",
		LongDescription = null,
		FullDescription = "This tiled-top table is a large, well-made table formed from ceramic. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 30000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy table with a ceramic primary material for a tiled or inset top.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanTripodTable() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_tripod_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6623", []),
		Noun = "table",
		ShortDescription = "a small tripod table",
		LongDescription = null,
		FullDescription = "This small tripod table is a medium-sized, well-made table built from olive wood. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 6200.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "olive wood",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small three-legged table for cups, lamps, basin work, or chamber service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMediterraneanWritingStand() => new()
	{
		Metadata = new("medieval_furniture_mediterranean_writing_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6653", []),
		Noun = "stand",
		ShortDescription = "a cypress writing stand",
		LongDescription = null,
		FullDescription = "This cypress writing stand is a medium-sized, well-made stand built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine angled writing or reading stand with a warm-climate wood profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateBolsterDaybed() => new()
	{
		Metadata = new("medieval_furniture_islamicate_bolster_daybed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6682", []),
		Noun = "daybed",
		ShortDescription = "a bolstered daybed",
		LongDescription = null,
		FullDescription = "This bolstered daybed is a very large, well-made daybed made from woven silk. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine daybed-like seat with bolstered textile surface and couch behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateCarpetPlatform() => new()
	{
		Metadata = new("medieval_furniture_islamicate_carpet_platform", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6712", []),
		Noun = "platform",
		ShortDescription = "a padded sitting platform",
		LongDescription = null,
		FullDescription = "This padded sitting platform is a large, well-made platform made from woven wool. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 9000.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Chair_Quad",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft four-person sitting platform distinct from decorative carpets and wall hangings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateCarvedStool() => new()
	{
		Metadata = new("medieval_furniture_islamicate_carved_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6742", []),
		Noun = "stool",
		ShortDescription = "a carved low stool",
		LongDescription = null,
		FullDescription = "This carved low stool is a medium-sized, well-made stool built from cedar boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3500.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low single stool suited to reception rooms, baths, and chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateCushionSet() => new()
	{
		Metadata = new("medieval_furniture_islamicate_cushion_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6771", []),
		Noun = "cushion",
		ShortDescription = "a broad cushion seat",
		LongDescription = null,
		FullDescription = "This broad cushion seat is a large, well-made cushion made from woven cotton. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Chair_Double"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad two-person cushion for reception rooms, chambers, and low table seating.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateDivanPlatform() => new()
	{
		Metadata = new("medieval_furniture_islamicate_divan_platform", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6800", []),
		Noun = "divan",
		ShortDescription = "a padded divan platform",
		LongDescription = null,
		FullDescription = "This padded divan platform is a very large, well-made divan made from woven cotton. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Quad",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad padded seating platform for reception rooms and household chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateInlaidSideTable() => new()
	{
		Metadata = new("medieval_furniture_islamicate_inlaid_side_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6830", []),
		Noun = "table",
		ShortDescription = "an inlaid side table",
		LongDescription = null,
		FullDescription = "This inlaid side table is a medium-sized, well-made table built from sandalwood. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 6500.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sandalwood",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Side_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine small side table with decorative inlay described without special mechanics.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateLowBedstead() => new()
	{
		Metadata = new("medieval_furniture_islamicate_low_bedstead", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6860", []),
		Noun = "bedstead",
		ShortDescription = "a low cedar bedstead",
		LongDescription = null,
		FullDescription = "This low cedar bedstead is a very large, well-made bedstead built from cedar boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 36000.0,
		Cost = 145.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low polished bedstead suited to urban chambers and curtained sleeping spaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateLowFeastTable() => new()
	{
		Metadata = new("medieval_furniture_islamicate_low_feast_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6889", []),
		Noun = "table",
		ShortDescription = "a low feast table",
		LongDescription = null,
		FullDescription = "This low feast table is a large, well-made table built from cedar boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 16000.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low table for shared dishes, cups, and floor-cushion seating.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateLowWritingDesk() => new()
	{
		Metadata = new("medieval_furniture_islamicate_low_writing_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6919", []),
		Noun = "desk",
		ShortDescription = "a low cedar writing desk",
		LongDescription = null,
		FullDescription = "This low cedar writing desk is a medium-sized, well-made desk built from cedar boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8000.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low writing desk suitable for cushion seating and manuscript work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateMerchantDesk() => new()
	{
		Metadata = new("medieval_furniture_islamicate_merchant_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6949", []),
		Noun = "desk",
		ShortDescription = "a merchant's low desk",
		LongDescription = null,
		FullDescription = "This merchant's low desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 17000.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low account desk for ledgers, coins, seals, letters, and commercial paperwork.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateReadingStand() => new()
	{
		Metadata = new("medieval_furniture_islamicate_reading_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:6979", []),
		Noun = "stand",
		ShortDescription = "a folding reading stand",
		LongDescription = null,
		FullDescription = "This folding reading stand is a small, well-made stand built from cedar boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Folding reading stand for books, tablets, or letters without religious specificity.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureIslamicateScreenedCouch() => new()
	{
		Metadata = new("medieval_furniture_islamicate_screened_couch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7008", []),
		Noun = "couch",
		ShortDescription = "a low reception couch",
		LongDescription = null,
		FullDescription = "This low reception couch is a large, well-made couch built from walnut boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 21000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Three-person couch-like reception seat with a usable surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianBambooStool() => new()
	{
		Metadata = new("medieval_furniture_south_asian_bamboo_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7038", []),
		Noun = "stool",
		ShortDescription = "a woven bamboo stool",
		LongDescription = null,
		FullDescription = "This woven bamboo stool is a small, workmanlike stool built from split bamboo. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light single-person stool of woven or lashed bamboo.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianBolsterSeat() => new()
	{
		Metadata = new("medieval_furniture_south_asian_bolster_seat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7067", []),
		Noun = "seat",
		ShortDescription = "a bolstered floor seat",
		LongDescription = null,
		FullDescription = "This bolstered floor seat is a large, workmanlike seat made from woven cotton. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4800.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Chair_Double"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Two-person soft floor seat with bolsters and no storage behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianCarvedSwingBench() => new()
	{
		Metadata = new("medieval_furniture_south_asian_carved_swing_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7096", []),
		Noun = "bench",
		ShortDescription = "a carved swing-bench seat",
		LongDescription = null,
		FullDescription = "This carved swing-bench seat is a large, well-made bench built from teak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Two-seat carved bench-like seat; no swinging mechanics are implied.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianCharpoyBed() => new()
	{
		Metadata = new("medieval_furniture_south_asian_charpoy_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7126", []),
		Noun = "bed",
		ShortDescription = "a rope-woven charpoy bed",
		LongDescription = null,
		FullDescription = "This rope-woven charpoy bed is a very large, workmanlike bed built from teak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 78.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rope-woven bed frame suited to warm-climate household and courtyard sleeping.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianChowkiTable() => new()
	{
		Metadata = new("medieval_furniture_south_asian_chowki_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7155", []),
		Noun = "table",
		ShortDescription = "a low chowki table",
		LongDescription = null,
		FullDescription = "This low chowki table is a medium-sized, well-made table built from teak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 6500.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low square table or platform for household service, seating-adjacent work, or display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianDaybed() => new()
	{
		Metadata = new("medieval_furniture_south_asian_daybed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7185", []),
		Noun = "daybed",
		ShortDescription = "a teak daybed",
		LongDescription = null,
		FullDescription = "This teak daybed is a very large, well-made daybed built from teak boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 28000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low daybed surface for courtyards, shaded rooms, and warm-climate chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianFloorCushion() => new()
	{
		Metadata = new("medieval_furniture_south_asian_floor_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7214", []),
		Noun = "cushion",
		ShortDescription = "a cotton floor cushion",
		LongDescription = null,
		FullDescription = "This cotton floor cushion is a medium-sized, workmanlike cushion made from woven cotton. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Single cotton cushion for floor seating and low tables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianLowCot() => new()
	{
		Metadata = new("medieval_furniture_south_asian_low_cot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7243", []),
		Noun = "cot",
		ShortDescription = "a low rope-woven cot",
		LongDescription = null,
		FullDescription = "This low rope-woven cot is a large, workmanlike cot built from split bamboo. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light low cot with a woven surface for sleep, rest, or travel.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianLowPlatformBench() => new()
	{
		Metadata = new("medieval_furniture_south_asian_low_platform_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7272", []),
		Noun = "platform",
		ShortDescription = "a low wooden platform",
		LongDescription = null,
		FullDescription = "This low wooden platform is a large, workmanlike platform built from teak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Quad",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low platform for sitting, resting, or placing light household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianLowWritingBoard() => new()
	{
		Metadata = new("medieval_furniture_south_asian_low_writing_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7302", []),
		Noun = "board",
		ShortDescription = "a low writing board",
		LongDescription = null,
		FullDescription = "This low writing board is a small, workmanlike board built from teak boards. A stable frame carries the weight, with the working surface placed where hands naturally reach it. The edges are worn smooth by household use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1900.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small low writing board for accounts, letters, or lessons.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianMerchantDesk() => new()
	{
		Metadata = new("medieval_furniture_south_asian_merchant_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7331", []),
		Noun = "desk",
		ShortDescription = "a low merchant's desk",
		LongDescription = null,
		FullDescription = "This low merchant's desk is a medium-sized, well-made desk built from teak boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8000.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low desk surface for coin trays, account papers, seals, and trade records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSouthAsianTeakChair() => new()
	{
		Metadata = new("medieval_furniture_south_asian_teak_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7361", []),
		Noun = "chair",
		ShortDescription = "a carved teak chair",
		LongDescription = null,
		FullDescription = "This carved teak chair is a medium-sized, well-made chair built from teak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8500.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Single carved chair with warm-climate hardwood construction.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianBambooBench() => new()
	{
		Metadata = new("medieval_furniture_east_asian_bamboo_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7390", []),
		Noun = "bench",
		ShortDescription = "a bamboo two-seat bench",
		LongDescription = null,
		FullDescription = "This bamboo two-seat bench is a large, workmanlike bench built from split bamboo. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8000.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light two-seat bench suitable for chambers, verandas, and courtyards.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianBambooStool() => new()
	{
		Metadata = new("medieval_furniture_east_asian_bamboo_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7420", []),
		Noun = "stool",
		ShortDescription = "a bamboo square stool",
		LongDescription = null,
		FullDescription = "This bamboo square stool is a medium-sized, workmanlike stool built from split bamboo. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2200.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light square stool suitable for low desks or household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianCotFrame() => new()
	{
		Metadata = new("medieval_furniture_east_asian_cot_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7449", []),
		Noun = "cot",
		ShortDescription = "a narrow bamboo cot",
		LongDescription = null,
		FullDescription = "This narrow bamboo cot is a large, workmanlike cot built from split bamboo. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8000.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light narrow cot frame with a simple surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianDaybedCouch() => new()
	{
		Metadata = new("medieval_furniture_east_asian_daybed_couch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7478", []),
		Noun = "daybed",
		ShortDescription = "a wooden daybed couch",
		LongDescription = null,
		FullDescription = "This wooden daybed couch is a very large, well-made daybed built from cedar boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 145.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Three-person daybed-like couch for study, reception, or rest.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianFloorCushion() => new()
	{
		Metadata = new("medieval_furniture_east_asian_floor_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7508", []),
		Noun = "cushion",
		ShortDescription = "a square floor cushion",
		LongDescription = null,
		FullDescription = "This square floor cushion is a medium-sized, workmanlike cushion made from woven cotton. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Single square cushion for floor seating beside low tables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianLowDiningTable() => new()
	{
		Metadata = new("medieval_furniture_east_asian_low_dining_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7537", []),
		Noun = "table",
		ShortDescription = "a low lacquered table",
		LongDescription = null,
		FullDescription = "This low lacquered table is a large, well-made table built from split bamboo. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 12000.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low table for floor seating, tea service, writing, or shared dishes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianOfficialDesk() => new()
	{
		Metadata = new("medieval_furniture_east_asian_official_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7567", []),
		Noun = "desk",
		ShortDescription = "a polished official desk",
		LongDescription = null,
		FullDescription = "This polished official desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 20000.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Formal desk surface for documents, seals, brushes, and books.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianPlatformBed() => new()
	{
		Metadata = new("medieval_furniture_east_asian_platform_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7597", []),
		Noun = "bed",
		ShortDescription = "a low platform bed",
		LongDescription = null,
		FullDescription = "This low platform bed is a very large, well-made bed built from cedar boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 32000.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low platform bed surface for mattresses, bedding, or sleeping mats.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
