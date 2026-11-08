#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalFurnitureAccountDesk() => new()
	{
		Metadata = new("medieval_furniture_account_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1721", []),
		Noun = "desk",
		ShortDescription = "an account-keeper's desk",
		LongDescription = null,
		FullDescription = "This account-keeper's desk is a large, well-made desk built from oak boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 150.0m,
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
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy desk surface for counting, writing, seals, and household or guild accounts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureBacklessBench() => new()
	{
		Metadata = new("medieval_furniture_backless_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1751", []),
		Noun = "bench",
		ShortDescription = "a backless wooden bench",
		LongDescription = null,
		FullDescription = "This backless wooden bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 11000.0,
		Cost = 28.0m,
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
		BuilderNotes = "Plain three-seat bench for halls, kitchens, workshops, or dormitories.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureBakersTable() => new()
	{
		Metadata = new("medieval_furniture_bakers_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1781", []),
		Noun = "table",
		ShortDescription = "a flour-dusted baker's table",
		LongDescription = null,
		FullDescription = "This flour-dusted baker's table is a large, workmanlike table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 66.0m,
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
				"Table_Six",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad smooth-topped table for kneading, cooling loaves, and laying out baking vessels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureBedsideTable() => new()
	{
		Metadata = new("medieval_furniture_bedside_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1811", []),
		Noun = "table",
		ShortDescription = "a small bedside table",
		LongDescription = null,
		FullDescription = "This small bedside table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 24.0m,
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
				"Container_Side_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small table for bedside lamps, cups, basins, books, or chamber goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureButcherBlockTable() => new()
	{
		Metadata = new("medieval_furniture_butcher_block_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1841", []),
		Noun = "table",
		ShortDescription = "a thick butcher-block table",
		LongDescription = null,
		FullDescription = "This thick butcher-block table is a large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 36000.0,
		Cost = 120.0m,
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
		BuilderNotes = "Heavy block-topped work table for butchery, curing, and other rough household or trade preparation.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCampaignTable() => new()
	{
		Metadata = new("medieval_furniture_campaign_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1871", []),
		Noun = "table",
		ShortDescription = "a folding campaign table",
		LongDescription = null,
		FullDescription = "This folding campaign table is a medium-sized, well-made table built from ash boards. A rectangular top rests on hinged legs that fold beneath it. The joints are pinned and rubbed bright where the frame moves. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 9000.0,
		Cost = 90.0m,
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
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact folding table for travel, military camps, counting houses, and temporary halls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCandleTable() => new()
	{
		Metadata = new("medieval_furniture_candle_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1901", []),
		Noun = "table",
		ShortDescription = "a small candle table",
		LongDescription = null,
		FullDescription = "This small candle table is a medium-sized, workmanlike table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6500.0,
		Cost = 22.0m,
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
				"Container_Side_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small table for lamps, candle trays, cups, or other chamber-side objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCanvasFieldCot() => new()
	{
		Metadata = new("medieval_furniture_canvas_field_cot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1931", []),
		Noun = "cot",
		ShortDescription = "a canvas field cot",
		LongDescription = null,
		FullDescription = "This canvas field cot is a large, workmanlike cot made from coarse canvas. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7200.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
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
		BuilderNotes = "Portable cot with canvas as the primary visible material for camp or travel use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCarvedArmchair() => new()
	{
		Metadata = new("medieval_furniture_carved_armchair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1960", []),
		Noun = "chair",
		ShortDescription = "a carved armchair",
		LongDescription = null,
		FullDescription = "This carved armchair is a large, well-made chair built from walnut boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 13000.0,
		Cost = 150.0m,
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
		BuilderNotes = "Fine single chair with arms and carved visible joinery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCarvedCourtTable() => new()
	{
		Metadata = new("medieval_furniture_carved_court_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1989", []),
		Noun = "table",
		ShortDescription = "a carved court table",
		LongDescription = null,
		FullDescription = "This carved court table is a large, well-made table built from walnut boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 220.0m,
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
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine carved table for a court chamber, solar, hall dais, or elite household room.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCarvedCradle() => new()
	{
		Metadata = new("medieval_furniture_carved_cradle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2019", []),
		Noun = "cradle",
		ShortDescription = "a carved wooden cradle",
		LongDescription = null,
		FullDescription = "This carved wooden cradle is a large, well-made cradle built from walnut boards. A small sleeping box sits on curved rockers, with low sides and rounded ends. The inner surface is smooth enough for blankets. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 9000.0,
		Cost = 100.0m,
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
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine infant cradle with visible carved sides and a narrow surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCarvedLecternStand() => new()
	{
		Metadata = new("medieval_furniture_carved_lectern_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2048", []),
		Noun = "stand",
		ShortDescription = "a carved reading stand",
		LongDescription = null,
		FullDescription = "This carved reading stand is a large, well-made stand built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 11000.0,
		Cost = 120.0m,
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
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine carved stand usable for public reading, accounts, or display without religious specificity.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCarvedThrone() => new()
	{
		Metadata = new("medieval_furniture_carved_throne", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2077", []),
		Noun = "throne",
		ShortDescription = "a carved wooden throne",
		LongDescription = null,
		FullDescription = "This carved wooden throne is a large, well-made throne built from walnut boards. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 300.0m,
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
		BuilderNotes = "Elaborate single seat with carved arms and a tall back; not a storage object.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureChamberSettle() => new()
	{
		Metadata = new("medieval_furniture_chamber_settle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2106", []),
		Noun = "settle",
		ShortDescription = "a small chamber settle",
		LongDescription = null,
		FullDescription = "This small chamber settle is a large, workmanlike settle built from beech boards. A long seat is set below a high back, with enclosed ends that make it feel sheltered. The sitting edge is rounded from use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 19000.0,
		Cost = 85.0m,
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
				"Bench_Double",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Two-seat high-backed bench suited to a private chamber or quiet hearth corner.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureChildsSmallTable() => new()
	{
		Metadata = new("medieval_furniture_childs_small_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2136", []),
		Noun = "table",
		ShortDescription = "a child-sized small table",
		LongDescription = null,
		FullDescription = "This child-sized small table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 16.0m,
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
				"Table_Four",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small low table for children, light chamber use, or cramped household corners.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureClerkStool() => new()
	{
		Metadata = new("medieval_furniture_clerk_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2166", []),
		Noun = "stool",
		ShortDescription = "a clerk's writing stool",
		LongDescription = null,
		FullDescription = "This clerk's writing stool is a medium-sized, workmanlike stool built from ash boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3200.0,
		Cost = 18.0m,
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
		BuilderNotes = "Simple single stool sized for desks, counters, and writing tables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureClerksDesk() => new()
	{
		Metadata = new("medieval_furniture_clerks_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2195", []),
		Noun = "desk",
		ShortDescription = "a clerk's narrow desk",
		LongDescription = null,
		FullDescription = "This clerk's narrow desk is a large, workmanlike desk built from oak boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 17500.0,
		Cost = 90.0m,
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
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow desk surface for account rolls, ledgers, seals, and correspondence.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCoarseHempPallet() => new()
	{
		Metadata = new("medieval_furniture_coarse_hemp_pallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2225", []),
		Noun = "pallet",
		ShortDescription = "a coarse hemp pallet",
		LongDescription = null,
		FullDescription = "This coarse hemp pallet is a large, workmanlike pallet made from woven hemp. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7000.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
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
		BuilderNotes = "Cheap stuffed sleeping pallet for servants, camps, and poor households.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCompactBench() => new()
	{
		Metadata = new("medieval_furniture_compact_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2254", []),
		Noun = "bench",
		ShortDescription = "a compact two-seat bench",
		LongDescription = null,
		FullDescription = "This compact two-seat bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10000.0,
		Cost = 30.0m,
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
				"Bench_Double",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Two-seat bench for smaller rooms, entries, and chamber walls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCopyistsTable() => new()
	{
		Metadata = new("medieval_furniture_copyists_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2284", []),
		Noun = "table",
		ShortDescription = "a copyist's table",
		LongDescription = null,
		FullDescription = "This copyist's table is a large, well-made table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 20000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
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
		BuilderNotes = "Broad writing and copying table for manuscripts, drafts, or formal documents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCountingTable() => new()
	{
		Metadata = new("medieval_furniture_counting_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2314", []),
		Noun = "table",
		ShortDescription = "a narrow counting table",
		LongDescription = null,
		FullDescription = "This narrow counting table is a large, well-made table built from walnut boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 16000.0,
		Cost = 120.0m,
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
		BuilderNotes = "Long narrow table for coin counting, account tablets, tallies, and small document bundles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCushionedBench() => new()
	{
		Metadata = new("medieval_furniture_cushioned_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2344", []),
		Noun = "bench",
		ShortDescription = "a cushioned wooden bench",
		LongDescription = null,
		FullDescription = "This cushioned wooden bench is a large, well-made bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 16000.0,
		Cost = 90.0m,
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
		BuilderNotes = "Three-seat bench with visible textile cushioning and a usable bench surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureCushionedChair() => new()
	{
		Metadata = new("medieval_furniture_cushioned_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2374", []),
		Noun = "chair",
		ShortDescription = "a cushioned wooden chair",
		LongDescription = null,
		FullDescription = "This cushioned wooden chair is a large, well-made chair built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 11000.0,
		Cost = 95.0m,
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
		BuilderNotes = "Single wooden chair with visible textile padding and a better-finished frame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureDaybedFrame() => new()
	{
		Metadata = new("medieval_furniture_daybed_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2403", []),
		Noun = "daybed",
		ShortDescription = "a plain wooden daybed",
		LongDescription = null,
		FullDescription = "This plain wooden daybed is a very large, workmanlike daybed built from beech boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30000.0,
		Cost = 95.0m,
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
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long wooden daybed frame treated as a bed surface rather than a couch seat.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureDisplayTable() => new()
	{
		Metadata = new("medieval_furniture_display_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2432", []),
		Noun = "table",
		ShortDescription = "a plain display table",
		LongDescription = null,
		FullDescription = "This plain display table is a large, workmanlike table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 17000.0,
		Cost = 60.0m,
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
		BuilderNotes = "Table with a clear surface for laying out goods, household objects, or wares for inspection.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureDocumentSortingTable() => new()
	{
		Metadata = new("medieval_furniture_document_sorting_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2462", []),
		Noun = "table",
		ShortDescription = "a document sorting table",
		LongDescription = null,
		FullDescription = "This document sorting table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
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
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large desk-like table for bundles, seals, accounts, records, and sorting work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureDyerSortingTable() => new()
	{
		Metadata = new("medieval_furniture_dyer_sorting_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2492", []),
		Noun = "table",
		ShortDescription = "a stained sorting table",
		LongDescription = null,
		FullDescription = "This stained sorting table is a large, workmanlike table built from elm boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 55.0m,
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
				"Table_Six",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Work table with worn, stained boards suited to sorting cloth, skeins, or workshop goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureFeastTable() => new()
	{
		Metadata = new("medieval_furniture_feast_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2522", []),
		Noun = "table",
		ShortDescription = "a massive feast table",
		LongDescription = null,
		FullDescription = "This massive feast table is a huge, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 74000.0,
		Cost = 260.0m,
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
		BuilderNotes = "Large twenty-place feasting table for great halls, courts, guild halls, or large households.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureFeltSleepingMat() => new()
	{
		Metadata = new("medieval_furniture_felt_sleeping_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2552", []),
		Noun = "mat",
		ShortDescription = "a felt sleeping mat",
		LongDescription = null,
		FullDescription = "This felt sleeping mat is a large, workmanlike mat made from pressed felt. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4800.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
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
		BuilderNotes = "Rollable felt-like sleeping mat represented as a cot-width surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureFineDeskChair() => new()
	{
		Metadata = new("medieval_furniture_fine_desk_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2581", []),
		Noun = "chair",
		ShortDescription = "a fine desk chair",
		LongDescription = null,
		FullDescription = "This fine desk chair is a medium-sized, well-made chair built from walnut boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8500.0,
		Cost = 110.0m,
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
		BuilderNotes = "Fine single chair intended for a writing desk, account table, or solar chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureFineSilkCushion() => new()
	{
		Metadata = new("medieval_furniture_fine_silk_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2610", []),
		Noun = "cushion",
		ShortDescription = "a fine silk floor cushion",
		LongDescription = null,
		FullDescription = "This fine silk floor cushion is a medium-sized, well-made cushion made from woven silk. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 85.0m,
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
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine single floor cushion for elite chambers, receptions, and low seating.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureFoldedStool() => new()
	{
		Metadata = new("medieval_furniture_folded_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2639", []),
		Noun = "stool",
		ShortDescription = "a folding wooden stool",
		LongDescription = null,
		FullDescription = "This folding wooden stool is a small, well-made stool built from ash boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2100.0,
		Cost = 34.0m,
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
		BuilderNotes = "Portable folding stool; mechanically a simple single seat.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureFoldedTrestleTable() => new()
	{
		Metadata = new("medieval_furniture_folded_trestle_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2668", []),
		Noun = "table",
		ShortDescription = "a folding trestle table",
		LongDescription = null,
		FullDescription = "This folding trestle table is a large, workmanlike table built from pine boards. A rectangular top rests on hinged legs that fold beneath it. The joints are pinned and rubbed bright where the frame moves. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15000.0,
		Cost = 54.0m,
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
		BuilderNotes = "Portable table with trestle-style supports; treated mechanically as a normal four-place table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureFoldingWritingDesk() => new()
	{
		Metadata = new("medieval_furniture_folding_writing_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2698", []),
		Noun = "desk",
		ShortDescription = "a folding writing desk",
		LongDescription = null,
		FullDescription = "This folding writing desk is a medium-sized, well-made desk built from ash boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8500.0,
		Cost = 95.0m,
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
		BuilderNotes = "Portable folding desk with a writing surface but no special storage behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureFurSleepingRoll() => new()
	{
		Metadata = new("medieval_furniture_fur_sleeping_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2728", []),
		Noun = "bedroll",
		ShortDescription = "a fur sleeping roll",
		LongDescription = null,
		FullDescription = "This fur sleeping roll is a medium-sized, well-made bedroll made from dressed fur. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 6200.0,
		Cost = 85.0m,
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
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Warm fur-lined sleeping roll for travel or cold chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureGamesTable() => new()
	{
		Metadata = new("medieval_furniture_games_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2757", []),
		Noun = "table",
		ShortDescription = "a marked games table",
		LongDescription = null,
		FullDescription = "This marked games table is a large, workmanlike table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
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
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Four-place table with faint marked squares and lines suitable for games, dice, and boards.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureGuardroomCot() => new()
	{
		Metadata = new("medieval_furniture_guardroom_cot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2787", []),
		Noun = "cot",
		ShortDescription = "a guardroom wooden cot",
		LongDescription = null,
		FullDescription = "This guardroom wooden cot is a large, workmanlike cot built from pine boards. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 40.0m,
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
		BuilderNotes = "Plain narrow cot suited to watches, barracks, gatehouses, and communal rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureGuestBedstead() => new()
	{
		Metadata = new("medieval_furniture_guest_bedstead", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2816", []),
		Noun = "bedstead",
		ShortDescription = "a guest-chamber bedstead",
		LongDescription = null,
		FullDescription = "This guest-chamber bedstead is a very large, well-made bedstead built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 48000.0,
		Cost = 145.0m,
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
		BuilderNotes = "Better-finished bedstead suited to a guest chamber, inn room, or household solar.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureHallboardTable() => new()
	{
		Metadata = new("medieval_furniture_hallboard_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2845", []),
		Noun = "table",
		ShortDescription = "a narrow hallboard table",
		LongDescription = null,
		FullDescription = "This narrow hallboard table is a large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
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
		BuilderNotes = "Narrow formal serving table for hall vessels, bread, candles, and household display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureHangingCradleFrame() => new()
	{
		Metadata = new("medieval_furniture_hanging_cradle_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2875", []),
		Noun = "cradle",
		ShortDescription = "a small hanging cradle",
		LongDescription = null,
		FullDescription = "This small hanging cradle is a large, workmanlike cradle built from willow boards. A small sleeping box sits on curved rockers, with low sides and rounded ends. The inner surface is smooth enough for blankets. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5000.0,
		Cost = 32.0m,
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
		BuilderNotes = "Light infant cradle with visible suspension points; no hanging mechanics are implied.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureHearthBench() => new()
	{
		Metadata = new("medieval_furniture_hearth_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2904", []),
		Noun = "bench",
		ShortDescription = "a low hearth bench",
		LongDescription = null,
		FullDescription = "This low hearth bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14000.0,
		Cost = 36.0m,
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
				"Bench_Double",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low bench for hearthside sitting, warming, and household work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureHeavyOakBedstead() => new()
	{
		Metadata = new("medieval_furniture_heavy_oak_bedstead", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2934", []),
		Noun = "bedstead",
		ShortDescription = "a heavy oak bedstead",
		LongDescription = null,
		FullDescription = "This heavy oak bedstead is a huge, well-made bedstead built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 68000.0,
		Cost = 220.0m,
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
		BuilderNotes = "Large heavy bedstead with visible posts and a broad surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureHempSittingMat() => new()
	{
		Metadata = new("medieval_furniture_hemp_sitting_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2963", []),
		Noun = "mat",
		ShortDescription = "a coarse hemp sitting mat",
		LongDescription = null,
		FullDescription = "This coarse hemp sitting mat is a medium-sized, workmanlike mat made from woven hemp. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3000.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
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
		BuilderNotes = "Cheap two-person floor mat for workrooms, kitchens, camps, or common rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureHighBackedChair() => new()
	{
		Metadata = new("medieval_furniture_high_backed_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:2992", []),
		Noun = "chair",
		ShortDescription = "a high-backed chair",
		LongDescription = null,
		FullDescription = "This high-backed chair is a large, well-made chair built from oak boards. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 14500.0,
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
		BuilderNotes = "Tall-backed elite or formal chair with a single seating position.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureInnRoundTable() => new()
	{
		Metadata = new("medieval_furniture_inn_round_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3021", []),
		Noun = "table",
		ShortDescription = "a sturdy inn table",
		LongDescription = null,
		FullDescription = "This sturdy inn table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 19000.0,
		Cost = 54.0m,
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
		BuilderNotes = "Practical four-place table suited to a rented chamber, common room, or small dining room.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureJoinedChair() => new()
	{
		Metadata = new("medieval_furniture_joined_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3051", []),
		Noun = "chair",
		ShortDescription = "a joined wooden chair",
		LongDescription = null,
		FullDescription = "This joined wooden chair is a medium-sized, well-made chair built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8500.0,
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
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-made single chair with joined frame construction and a firm seat.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureKitchenPrepTable() => new()
	{
		Metadata = new("medieval_furniture_kitchen_prep_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3080", []),
		Noun = "table",
		ShortDescription = "a scarred kitchen prep table",
		LongDescription = null,
		FullDescription = "This scarred kitchen prep table is a large, workmanlike table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 56.0m,
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
				"Table_Six",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hard-wearing preparation table marked by ordinary household cutting, scraping, and washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLadderbackChair() => new()
	{
		Metadata = new("medieval_furniture_ladderback_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3110", []),
		Noun = "chair",
		ShortDescription = "a ladder-back chair",
		LongDescription = null,
		FullDescription = "This ladder-back chair is a medium-sized, workmanlike chair built from ash boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6500.0,
		Cost = 34.0m,
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
		BuilderNotes = "Single chair with a narrow back made from horizontal slats.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLapWritingBoard() => new()
	{
		Metadata = new("medieval_furniture_lap_writing_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3139", []),
		Noun = "board",
		ShortDescription = "a lap writing board",
		LongDescription = null,
		FullDescription = "This lap writing board is a small, well-made board built from walnut boards. A stable frame carries the weight, with the working surface placed where hands naturally reach it. The edges are worn smooth by household use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2200.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small writing board with desk-surface behaviour for compact work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLargeFloorMat() => new()
	{
		Metadata = new("medieval_furniture_large_floor_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3168", []),
		Noun = "mat",
		ShortDescription = "a broad floor sitting mat",
		LongDescription = null,
		FullDescription = "This broad floor sitting mat is a large, workmanlike mat made from coarse jute. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jute",
		Tags = [
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Chair_Quad"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large four-person floor-seating mat for low tables and communal rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLaundryTable() => new()
	{
		Metadata = new("medieval_furniture_laundry_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3197", []),
		Noun = "table",
		ShortDescription = "a broad laundry table",
		LongDescription = null,
		FullDescription = "This broad laundry table is a large, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20000.0,
		Cost = 50.0m,
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
				"Table_Six",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad utilitarian table for folding cloth, sorting linens, or staging wash basins.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLeatherFloorCushion() => new()
	{
		Metadata = new("medieval_furniture_leather_floor_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3227", []),
		Noun = "cushion",
		ShortDescription = "a leather floor cushion",
		LongDescription = null,
		FullDescription = "This leather floor cushion is a medium-sized, workmanlike cushion made from worked leather. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2200.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
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
		BuilderNotes = "Durable single floor cushion covered in leather.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLeatherTravelBedroll() => new()
	{
		Metadata = new("medieval_furniture_leather_travel_bedroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3256", []),
		Noun = "bedroll",
		ShortDescription = "a leather travel bedroll",
		LongDescription = null,
		FullDescription = "This leather travel bedroll is a medium-sized, workmanlike bedroll made from worked leather. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
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
		BuilderNotes = "Durable travel bedroll for camps, caravans, and poor lodgings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLongHallBench() => new()
	{
		Metadata = new("medieval_furniture_long_hall_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3285", []),
		Noun = "bench",
		ShortDescription = "a long hall bench",
		LongDescription = null,
		FullDescription = "This long hall bench is a very large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 60.0m,
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
		BuilderNotes = "Long three-seat bench intended for halls, refectories, guardrooms, and feasting spaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLongHallTable() => new()
	{
		Metadata = new("medieval_furniture_long_hall_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3315", []),
		Noun = "table",
		ShortDescription = "a long hall table",
		LongDescription = null,
		FullDescription = "This long hall table is a very large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42000.0,
		Cost = 120.0m,
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
		BuilderNotes = "Twelve-place hall table for communal meals, meetings, and household service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowBookStand() => new()
	{
		Metadata = new("medieval_furniture_low_book_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3345", []),
		Noun = "stand",
		ShortDescription = "a low book stand",
		LongDescription = null,
		FullDescription = "This low book stand is a small, workmanlike stand built from cedar boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 24.0m,
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
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low angled reading support for use on the floor, bed, or low table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowBoxBed() => new()
	{
		Metadata = new("medieval_furniture_low_box_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3374", []),
		Noun = "bed",
		ShortDescription = "a low box bed",
		LongDescription = null,
		FullDescription = "This low box bed is a very large, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 52000.0,
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
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Solid low bed frame with boxed-in sides; no enclosed storage component.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowDeskCushion() => new()
	{
		Metadata = new("medieval_furniture_low_desk_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3403", []),
		Noun = "cushion",
		ShortDescription = "a low desk cushion",
		LongDescription = null,
		FullDescription = "This low desk cushion is a medium-sized, workmanlike cushion made from woven cotton. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1400.0,
		Cost = 22.0m,
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
		BuilderNotes = "Single floor cushion sized for low desks and writing boards.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowDiningTable() => new()
	{
		Metadata = new("medieval_furniture_low_dining_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3432", []),
		Noun = "table",
		ShortDescription = "a low dining table",
		LongDescription = null,
		FullDescription = "This low dining table is a large, workmanlike table built from cedar boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14500.0,
		Cost = 48.0m,
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
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low table intended for floor seating or cushion seating rather than high chairs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowDivanSeat() => new()
	{
		Metadata = new("medieval_furniture_low_divan_seat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3462", []),
		Noun = "divan",
		ShortDescription = "a low padded divan",
		LongDescription = null,
		FullDescription = "This low padded divan is a large, well-made divan made from woven cotton. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 12000.0,
		Cost = 110.0m,
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
				"Chair_Triple",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low three-person padded seat usable in chambers, reception rooms, or sleeping spaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowFootstool() => new()
	{
		Metadata = new("medieval_furniture_low_footstool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3492", []),
		Noun = "stool",
		ShortDescription = "a low wooden footstool",
		LongDescription = null,
		FullDescription = "This low wooden footstool is a small, workmanlike stool built from pine boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
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
		BuilderNotes = "Low single stool or footstool for chamber seating, hearthside work, or simple household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowPlatformSeat() => new()
	{
		Metadata = new("medieval_furniture_low_platform_seat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3521", []),
		Noun = "platform",
		ShortDescription = "a low seating platform",
		LongDescription = null,
		FullDescription = "This low seating platform is a large, workmanlike platform built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 50.0m,
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
				"Chair_Quad",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad low platform for several seated occupants; not treated as a storage container.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowRoundTable() => new()
	{
		Metadata = new("medieval_furniture_low_round_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3551", []),
		Noun = "table",
		ShortDescription = "a low round table",
		LongDescription = null,
		FullDescription = "This low round table is a large, workmanlike table built from cedar boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15000.0,
		Cost = 52.0m,
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
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low circular table for shared dishes, cups, writing boards, or household service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowSideTable() => new()
	{
		Metadata = new("medieval_furniture_low_side_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3581", []),
		Noun = "table",
		ShortDescription = "a low side table",
		LongDescription = null,
		FullDescription = "This low side table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 20.0m,
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
				"Table_Four",
				"Container_Side_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low portable side table with a simple surface for small household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowSleepingPlatform() => new()
	{
		Metadata = new("medieval_furniture_low_sleeping_platform", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3611", []),
		Noun = "platform",
		ShortDescription = "a low sleeping platform",
		LongDescription = null,
		FullDescription = "This low sleeping platform is a very large, workmanlike platform built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 32000.0,
		Cost = 70.0m,
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
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low raised platform with a broad bed-surface behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowWoodenChair() => new()
	{
		Metadata = new("medieval_furniture_low_wooden_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3640", []),
		Noun = "chair",
		ShortDescription = "a low wooden chair",
		LongDescription = null,
		FullDescription = "This low wooden chair is a medium-sized, workmanlike chair built from pine boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5800.0,
		Cost = 24.0m,
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
		BuilderNotes = "Low single seat suited to floor-level tasks or a cramped chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureLowWritingDesk() => new()
	{
		Metadata = new("medieval_furniture_low_writing_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3669", []),
		Noun = "desk",
		ShortDescription = "a low writing desk",
		LongDescription = null,
		FullDescription = "This low writing desk is a medium-sized, workmanlike desk built from cedar boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
		Cost = 55.0m,
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
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low desk intended for use while seated on a stool, cushion, or floor mat.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMapTable() => new()
	{
		Metadata = new("medieval_furniture_map_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3699", []),
		Noun = "table",
		ShortDescription = "a broad map table",
		LongDescription = null,
		FullDescription = "This broad map table is a very large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 34000.0,
		Cost = 150.0m,
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
				"Table_Eight",
				"Container_Large_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wide table with enough surface for maps, plans, manuscripts, and measuring tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMerchantAccountTable() => new()
	{
		Metadata = new("medieval_furniture_merchant_account_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3729", []),
		Noun = "table",
		ShortDescription = "a merchant's account table",
		LongDescription = null,
		FullDescription = "This merchant's account table is a large, well-made table built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 21000.0,
		Cost = 160.0m,
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
		BuilderNotes = "Well-finished table for account books, tally sticks, seals, coin trays, and written reckoning.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureMilkingStool() => new()
	{
		Metadata = new("medieval_furniture_milking_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3759", []),
		Noun = "stool",
		ShortDescription = "a low milking stool",
		LongDescription = null,
		FullDescription = "This low milking stool is a small, workmanlike stool built from beech boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 7.0m,
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
		BuilderNotes = "Low practical stool for animal work, hearth work, and other crouched tasks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNarrowCot() => new()
	{
		Metadata = new("medieval_furniture_narrow_cot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3788", []),
		Noun = "cot",
		ShortDescription = "a narrow wooden cot",
		LongDescription = null,
		FullDescription = "This narrow wooden cot is a large, workmanlike cot built from pine boards. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 42.0m,
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
		BuilderNotes = "Narrow cot or small bed surface for cramped rooms, servants, guards, or travel lodging.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureNarrowSideTable() => new()
	{
		Metadata = new("medieval_furniture_narrow_side_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3817", []),
		Noun = "table",
		ShortDescription = "a narrow side table",
		LongDescription = null,
		FullDescription = "This narrow side table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7200.0,
		Cost = 24.0m,
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
				"Container_Side_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small side table for a chamber, hall corner, bedside, or service area.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePaddedDaySeat() => new()
	{
		Metadata = new("medieval_furniture_padded_day_seat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3847", []),
		Noun = "seat",
		ShortDescription = "a padded day seat",
		LongDescription = null,
		FullDescription = "This padded day seat is a large, workmanlike seat made from woven wool. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6500.0,
		Cost = 60.0m,
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
				"Chair_Double",
				"Container_Couch_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft two-person day seat with couch-surface behaviour for placed items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePaddedFloorCushion() => new()
	{
		Metadata = new("medieval_furniture_padded_floor_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3877", []),
		Noun = "cushion",
		ShortDescription = "a padded floor cushion",
		LongDescription = null,
		FullDescription = "This padded floor cushion is a medium-sized, workmanlike cushion made from woven wool. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
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
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft portable single seat represented by chair behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePeddlersDisplayTable() => new()
	{
		Metadata = new("medieval_furniture_peddlers_display_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3906", []),
		Noun = "table",
		ShortDescription = "a peddler's display table",
		LongDescription = null,
		FullDescription = "This peddler's display table is a large, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000.0,
		Cost = 38.0m,
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
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light portable table for laying out wares at a stall, inn yard, or market corner.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePlainHighSeat() => new()
	{
		Metadata = new("medieval_furniture_plain_high_seat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3936", []),
		Noun = "seat",
		ShortDescription = "a plain high seat",
		LongDescription = null,
		FullDescription = "This plain high seat is a large, well-made seat built from oak boards. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 15000.0,
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
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Formal single high seat suitable for a dais, hall, or household head.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePlainReadingStand() => new()
	{
		Metadata = new("medieval_furniture_plain_reading_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3965", []),
		Noun = "stand",
		ShortDescription = "a plain reading stand",
		LongDescription = null,
		FullDescription = "This plain reading stand is a medium-sized, workmanlike stand built from oak boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 38.0m,
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
		BuilderNotes = "Angled stand for books, tablets, account rolls, or scrolls; not tied to any religious use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePlainSingleChair() => new()
	{
		Metadata = new("medieval_furniture_plain_single_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:3994", []),
		Noun = "chair",
		ShortDescription = "a plain wooden chair",
		LongDescription = null,
		FullDescription = "This plain wooden chair is a medium-sized, workmanlike chair built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7000.0,
		Cost = 30.0m,
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
		BuilderNotes = "Plain single chair for ordinary household, tavern, or workshop use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePlainSmallTable() => new()
	{
		Metadata = new("medieval_furniture_plain_small_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4023", []),
		Noun = "table",
		ShortDescription = "a plain small wooden table",
		LongDescription = null,
		FullDescription = "This plain small wooden table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14000.0,
		Cost = 42.0m,
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
		BuilderNotes = "Four-place general household table with ordinary item-on-surface behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePlainWoodenBed() => new()
	{
		Metadata = new("medieval_furniture_plain_wooden_bed", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4053", []),
		Noun = "bed",
		ShortDescription = "a plain wooden bed",
		LongDescription = null,
		FullDescription = "This plain wooden bed is a very large, workmanlike bed built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 36000.0,
		Cost = 82.0m,
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
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain wooden bedstead represented as a bed surface rather than a storage item.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePlainWritingDesk() => new()
	{
		Metadata = new("medieval_furniture_plain_writing_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4082", []),
		Noun = "desk",
		ShortDescription = "a plain writing desk",
		LongDescription = null,
		FullDescription = "This plain writing desk is a large, workmanlike desk built from ash boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 19000.0,
		Cost = 96.0m,
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
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Desk surface for writing and reading; drawer storage is a separate container behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePlankServingTable() => new()
	{
		Metadata = new("medieval_furniture_plank_serving_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4112", []),
		Noun = "table",
		ShortDescription = "a plank serving table",
		LongDescription = null,
		FullDescription = "This plank serving table is a large, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 46.0m,
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
				"Table_Six",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain service table for platters, jugs, bread boards, and kitchen-to-hall serving work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePolishedSmallTable() => new()
	{
		Metadata = new("medieval_furniture_polished_small_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4142", []),
		Noun = "table",
		ShortDescription = "a polished small table",
		LongDescription = null,
		FullDescription = "This polished small table is a large, well-made table built from walnut boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 15000.0,
		Cost = 110.0m,
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
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Well-finished small table with a polished surface and careful joinery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePortableBookStand() => new()
	{
		Metadata = new("medieval_furniture_portable_book_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4172", []),
		Noun = "stand",
		ShortDescription = "a portable book stand",
		LongDescription = null,
		FullDescription = "This portable book stand is a medium-sized, workmanlike stand built from oak boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 34.0m,
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
		BuilderNotes = "Small angled stand for an open book, wax tablet, or tablet board.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePortableCot() => new()
	{
		Metadata = new("medieval_furniture_portable_cot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4201", []),
		Noun = "cot",
		ShortDescription = "a portable folding cot",
		LongDescription = null,
		FullDescription = "This portable folding cot is a large, workmanlike cot built from ash boards. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9500.0,
		Cost = 50.0m,
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
				"Container_Cot_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light folding cot with a narrow surface; no storage behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePortableWritingTable() => new()
	{
		Metadata = new("medieval_furniture_portable_writing_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4230", []),
		Noun = "table",
		ShortDescription = "a portable writing table",
		LongDescription = null,
		FullDescription = "This portable writing table is a medium-sized, well-made table built from ash boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7500.0,
		Cost = 80.0m,
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
		BuilderNotes = "Small table surface for writing that can be moved between rooms or taken on a journey.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurniturePostedBedFrame() => new()
	{
		Metadata = new("medieval_furniture_posted_bed_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4260", []),
		Noun = "bed",
		ShortDescription = "a posted wooden bed",
		LongDescription = null,
		FullDescription = "This posted wooden bed is a huge, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 70000.0,
		Cost = 240.0m,
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
		BuilderNotes = "Large posted bed frame suited to elite chambers and curtain hangings, though no hanging behaviour is implied.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureRaisedSleepingPlatform() => new()
	{
		Metadata = new("medieval_furniture_raised_sleeping_platform", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4289", []),
		Noun = "platform",
		ShortDescription = "a raised sleeping platform",
		LongDescription = null,
		FullDescription = "This raised sleeping platform is a very large, well-made platform built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 46000.0,
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
				"Container_Bed_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad raised platform suitable for bedding, mats, and chamber sleeping arrangements.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureRecordChamberTable() => new()
	{
		Metadata = new("medieval_furniture_record_chamber_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4318", []),
		Noun = "table",
		ShortDescription = "a record-chamber table",
		LongDescription = null,
		FullDescription = "This record-chamber table is a very large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 32000.0,
		Cost = 150.0m,
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
				"Table_Eight",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large formal table for records, charts, ledgers, and sealed documents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureRoundEatingTable() => new()
	{
		Metadata = new("medieval_furniture_round_eating_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4348", []),
		Noun = "table",
		ShortDescription = "a round wooden eating table",
		LongDescription = null,
		FullDescription = "This round wooden eating table is a large, workmanlike table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15500.0,
		Cost = 48.0m,
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
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Round four-place eating table for domestic rooms, taverns, and small halls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureRoundFloorCushion() => new()
	{
		Metadata = new("medieval_furniture_round_floor_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4378", []),
		Noun = "cushion",
		ShortDescription = "a round floor cushion",
		LongDescription = null,
		FullDescription = "This round floor cushion is a medium-sized, workmanlike cushion made from woven wool. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 16.0m,
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
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Round padded cushion for floor seating and low tables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureRoundStool() => new()
	{
		Metadata = new("medieval_furniture_round_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4407", []),
		Noun = "stool",
		ShortDescription = "a round wooden stool",
		LongDescription = null,
		FullDescription = "This round wooden stool is a medium-sized, workmanlike stool built from elm boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3400.0,
		Cost = 16.0m,
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
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact round stool with a single seating position.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSchoolDesk() => new()
	{
		Metadata = new("medieval_furniture_school_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4436", []),
		Noun = "desk",
		ShortDescription = "a simple school desk",
		LongDescription = null,
		FullDescription = "This simple school desk is a large, workmanlike desk built from pine boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000.0,
		Cost = 45.0m,
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
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain writing surface for teaching, copying, lessons, or apprentices' work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureScribeChair() => new()
	{
		Metadata = new("medieval_furniture_scribe_chair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4466", []),
		Noun = "chair",
		ShortDescription = "a scribe's wooden chair",
		LongDescription = null,
		FullDescription = "This scribe's wooden chair is a medium-sized, workmanlike chair built from beech boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 42.0m,
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
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain single chair suited to long desk work and copying.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureScribesDesk() => new()
	{
		Metadata = new("medieval_furniture_scribes_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4495", []),
		Noun = "desk",
		ShortDescription = "a scribe's writing desk",
		LongDescription = null,
		FullDescription = "This scribe's writing desk is a large, well-made desk built from beech boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
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
		BuilderNotes = "Orderly writing desk with space for papers, ink vessels, tablets, and writing tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSecretarysDesk() => new()
	{
		Metadata = new("medieval_furniture_secretarys_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4525", []),
		Noun = "desk",
		ShortDescription = "a secretary's writing desk",
		LongDescription = null,
		FullDescription = "This secretary's writing desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
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
		BuilderNotes = "Fine writing desk for a private chamber, chancery, solar, or household office.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureServantsPalletFrame() => new()
	{
		Metadata = new("medieval_furniture_servants_pallet_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4555", []),
		Noun = "frame",
		ShortDescription = "a servant's pallet frame",
		LongDescription = null,
		FullDescription = "This servant's pallet frame is a large, workmanlike frame built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16000.0,
		Cost = 28.0m,
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
		BuilderNotes = "Plain low frame for a narrow pallet or sleeping mat.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSettleBench() => new()
	{
		Metadata = new("medieval_furniture_settle_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4584", []),
		Noun = "settle",
		ShortDescription = "a high-backed settle",
		LongDescription = null,
		FullDescription = "This high-backed settle is a large, well-made settle built from oak boards. A long seat is set below a high back, with enclosed ends that make it feel sheltered. The sitting edge is rounded from use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 140.0m,
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
		BuilderNotes = "Three-seat high-backed bench for a hall, solar, chamber, or tavern corner.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureShallowCounterTable() => new()
	{
		Metadata = new("medieval_furniture_shallow_counter_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4614", []),
		Noun = "table",
		ShortDescription = "a shallow counter table",
		LongDescription = null,
		FullDescription = "This shallow counter table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28000.0,
		Cost = 78.0m,
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
		BuilderNotes = "High practical table for counters, workshops, store rooms, or household service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureShipBunk() => new()
	{
		Metadata = new("medieval_furniture_ship_bunk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:4644", []),
		Noun = "bunk",
		ShortDescription = "a narrow wooden bunk",
		LongDescription = null,
		FullDescription = "This narrow wooden bunk is a large, workmanlike bunk built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 17000.0,
		Cost = 38.0m,
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
		BuilderNotes = "Narrow bunk-like sleeping surface for ships, barracks, towers, or cramped lodgings.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
