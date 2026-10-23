#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalFurnitureEastAsianReedLikeMat() => new()
	{
		Metadata = new("medieval_furniture_east_asian_reed_like_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7626", []),
		Noun = "mat",
		ShortDescription = "a woven bamboo sitting mat",
		LongDescription = null,
		FullDescription = "This woven bamboo sitting mat is a large, workmanlike mat built from split bamboo. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3800.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
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
		BuilderNotes = "Large four-person floor seating mat represented by chair behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianRoundStool() => new()
	{
		Metadata = new("medieval_furniture_east_asian_round_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7655", []),
		Noun = "stool",
		ShortDescription = "a round drum stool",
		LongDescription = null,
		FullDescription = "This round drum stool is a medium-sized, well-made stool formed from ceramic. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8500.0,
		Cost = 65.0m,
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
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Round ceramic single seat for refined rooms, courtyards, or garden chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianScholarDesk() => new()
	{
		Metadata = new("medieval_furniture_east_asian_scholar_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7684", []),
		Noun = "desk",
		ShortDescription = "a low scholar's desk",
		LongDescription = null,
		FullDescription = "This low scholar's desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 15000.0,
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
				"Table_Four",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low desk for writing, brush work, books, and formal study.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianSilkCushion() => new()
	{
		Metadata = new("medieval_furniture_east_asian_silk_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7714", []),
		Noun = "cushion",
		ShortDescription = "a silk floor cushion",
		LongDescription = null,
		FullDescription = "This silk floor cushion is a medium-sized, well-made cushion made from woven silk. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 65.0m,
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
		BuilderNotes = "Fine single cushion for reception rooms, study spaces, and low tables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianTeaTable() => new()
	{
		Metadata = new("medieval_furniture_east_asian_tea_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7743", []),
		Noun = "table",
		ShortDescription = "a small tea table",
		LongDescription = null,
		FullDescription = "This small tea table is a medium-sized, well-made table built from split bamboo. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 52.0m,
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
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small low table for cups, bowls, writing tools, or domestic tea service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureEastAsianWritingStand() => new()
	{
		Metadata = new("medieval_furniture_east_asian_writing_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7773", []),
		Noun = "stand",
		ShortDescription = "a low writing stand",
		LongDescription = null,
		FullDescription = "This low writing stand is a small, well-made stand built from cedar boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 40.0m,
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
		BuilderNotes = "Small low stand for a book, writing board, or brush work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppeCollapsibleStool() => new()
	{
		Metadata = new("medieval_furniture_steppe_collapsible_stool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7802", []),
		Noun = "stool",
		ShortDescription = "a collapsible camp stool",
		LongDescription = null,
		FullDescription = "This collapsible camp stool is a small, workmanlike stool made from worked leather. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
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
		BuilderNotes = "Compact folding stool for riding camps, wagons, and caravan stops.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppeFeltSittingMat() => new()
	{
		Metadata = new("medieval_furniture_steppe_felt_sitting_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7831", []),
		Noun = "mat",
		ShortDescription = "a felt sitting mat",
		LongDescription = null,
		FullDescription = "This felt sitting mat is a large, workmanlike mat made from pressed felt. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
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
				"Chair_Double"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Two-person felt floor-seating mat for tents, camps, and cold ground.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppeFeltSleepingMat() => new()
	{
		Metadata = new("medieval_furniture_steppe_felt_sleeping_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7860", []),
		Noun = "mat",
		ShortDescription = "a felt sleeping mat",
		LongDescription = null,
		FullDescription = "This felt sleeping mat is a large, workmanlike mat made from pressed felt. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4800.0,
		Cost = 28.0m,
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
		BuilderNotes = "Portable felt sleeping surface for tent, wagon, or camp use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppeFineCushionSeat() => new()
	{
		Metadata = new("medieval_furniture_steppe_fine_cushion_seat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7889", []),
		Noun = "cushion",
		ShortDescription = "a patterned felt cushion",
		LongDescription = null,
		FullDescription = "This patterned felt cushion is a medium-sized, well-made cushion made from pressed felt. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1700.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
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
		BuilderNotes = "Fine single cushion for a tent interior, wagon, or elite travelling household.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppeFoldingCampTable() => new()
	{
		Metadata = new("medieval_furniture_steppe_folding_camp_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7918", []),
		Noun = "table",
		ShortDescription = "a folding camp table",
		LongDescription = null,
		FullDescription = "This folding camp table is a medium-sized, workmanlike table built from ash boards. A rectangular top rests on hinged legs that fold beneath it. The joints are pinned and rubbed bright where the frame moves. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6500.0,
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
				"Table_Four",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable table for camps, caravans, and temporary encampments.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppeLeatherBedroll() => new()
	{
		Metadata = new("medieval_furniture_steppe_leather_bedroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7948", []),
		Noun = "bedroll",
		ShortDescription = "a leather-bound bedroll",
		LongDescription = null,
		FullDescription = "This leather-bound bedroll is a medium-sized, workmanlike bedroll made from worked leather. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 42.0m,
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
		BuilderNotes = "Travel bedroll with leather as the primary visible material.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppeLowTravelTable() => new()
	{
		Metadata = new("medieval_furniture_steppe_low_travel_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:7977", []),
		Noun = "table",
		ShortDescription = "a low travel table",
		LongDescription = null,
		FullDescription = "This low travel table is a medium-sized, workmanlike table built from birch boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
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
		BuilderNotes = "Low table for portable meals, account work, and tent interiors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppePackCot() => new()
	{
		Metadata = new("medieval_furniture_steppe_pack_cot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8007", []),
		Noun = "cot",
		ShortDescription = "a folding pack cot",
		LongDescription = null,
		FullDescription = "This folding pack cot is a large, workmanlike cot built from birch boards. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8500.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
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
		BuilderNotes = "Light folding cot designed for portability and camp use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppeTentChestTable() => new()
	{
		Metadata = new("medieval_furniture_steppe_tent_chest_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8036", []),
		Noun = "table",
		ShortDescription = "a low tent table",
		LongDescription = null,
		FullDescription = "This low tent table is a medium-sized, workmanlike table built from birch boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7000.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
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
		BuilderNotes = "Low tent table for bowls, cups, lamps, and travel goods; not a storage chest.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurnitureSteppeWagonBench() => new()
	{
		Metadata = new("medieval_furniture_steppe_wagon_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8066", []),
		Noun = "bench",
		ShortDescription = "a narrow wagon bench",
		LongDescription = null,
		FullDescription = "This narrow wagon bench is a large, workmanlike bench built from birch boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
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
		BuilderNotes = "Narrow two-seat bench for wagons, camps, or portable interiors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatBedchamberEmberPot() => new()
	{
		Metadata = new("medieval_heat_bedchamber_ember_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8096", []),
		Noun = "pot",
		ShortDescription = "a bedchamber ember pot",
		LongDescription = null,
		FullDescription = "This bedchamber ember pot is a small, workmanlike pot formed from earthenware. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1400.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small covered pot for safely carrying or holding a modest ember charge.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatBrassWarmingPan() => new()
	{
		Metadata = new("medieval_heat_brass_warming_pan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8125", []),
		Noun = "pan",
		ShortDescription = "a brass warming pan",
		LongDescription = null,
		FullDescription = "This brass warming pan is a small, well-made pan worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2100.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Covered warming pan with a shallow fuel chamber and a long practical handle.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatBronzeTableBrazier() => new()
	{
		Metadata = new("medieval_heat_bronze_table_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8154", []),
		Noun = "brazier",
		ShortDescription = "a bronze table brazier",
		LongDescription = null,
		FullDescription = "This bronze table brazier is a medium-sized, well-made brazier worked from bronze. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3900.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine small brazier for a table, counter, or chamber where controlled heat is useful.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatCastIronRoomBrazier() => new()
	{
		Metadata = new("medieval_heat_cast_iron_room_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8183", []),
		Noun = "brazier",
		ShortDescription = "a cast-iron room brazier",
		LongDescription = null,
		FullDescription = "This cast-iron room brazier is a large, well-made brazier worked from cast iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 12500.0,
		Cost = 84.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cast iron",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy room brazier with a deep bowl and stout feet for sustained heat.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatCharcoalHandWarmer() => new()
	{
		Metadata = new("medieval_heat_charcoal_hand_warmer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8212", []),
		Noun = "warmer",
		ShortDescription = "a charcoal hand warmer",
		LongDescription = null,
		FullDescription = "This charcoal hand warmer is a small, well-made warmer worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small vented warming vessel intended for a limited charcoal charge.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatClayCharcoalBrazier() => new()
	{
		Metadata = new("medieval_heat_clay_charcoal_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8241", []),
		Noun = "brazier",
		ShortDescription = "a clay charcoal brazier",
		LongDescription = null,
		FullDescription = "This clay charcoal brazier is a medium-sized, workmanlike brazier formed from fired clay. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3200.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "clay",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low clay brazier blackened by charcoal use around the bowl and rim.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatCourtyardFireBasin() => new()
	{
		Metadata = new("medieval_heat_courtyard_fire_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8270", []),
		Noun = "basin",
		ShortDescription = "a courtyard fire basin",
		LongDescription = null,
		FullDescription = "This courtyard fire basin is a large, workmanlike basin worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9600.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad metal basin for outdoor warmth and gathering light in a courtyard or yard.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatHallFloorBrazier() => new()
	{
		Metadata = new("medieval_heat_hall_floor_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8299", []),
		Noun = "brazier",
		ShortDescription = "a hall floor brazier",
		LongDescription = null,
		FullDescription = "This hall floor brazier is a large, well-made brazier worked from cast iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 14200.0,
		Cost = 96.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cast iron",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large floor brazier for a hall, common room, watch chamber, or feasting space.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatIronEmberPan() => new()
	{
		Metadata = new("medieval_heat_iron_ember_pan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8328", []),
		Noun = "pan",
		ShortDescription = "an iron ember pan",
		LongDescription = null,
		FullDescription = "This iron ember pan is a small, workmanlike pan worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Handled metal pan for carrying embers or warming a small place.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatSmallCharcoalBrazier() => new()
	{
		Metadata = new("medieval_heat_small_charcoal_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8357", []),
		Noun = "brazier",
		ShortDescription = "a small charcoal brazier",
		LongDescription = null,
		FullDescription = "This small charcoal brazier is a small, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact brazier for personal warmth, small chambers, or workshop tasks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatStandingFireBasket() => new()
	{
		Metadata = new("medieval_heat_standing_fire_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8386", []),
		Noun = "basket",
		ShortDescription = "a standing fire basket",
		LongDescription = null,
		FullDescription = "This standing fire basket is a medium-sized, workmanlike basket worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4600.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open iron basket raised on legs, intended for glowing fuel rather than storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatTerracottaFireBowl() => new()
	{
		Metadata = new("medieval_heat_terracotta_fire_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8415", []),
		Noun = "bowl",
		ShortDescription = "a terracotta fire bowl",
		LongDescription = null,
		FullDescription = "This terracotta fire bowl is a medium-sized, workmanlike bowl formed from terracotta. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "terracotta",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Reddish fire bowl meant to cradle embers or charcoal for light warmth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatWorkshopCoalBrazier() => new()
	{
		Metadata = new("medieval_heat_workshop_coal_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8444", []),
		Noun = "brazier",
		ShortDescription = "a workshop coal brazier",
		LongDescription = null,
		FullDescription = "This workshop coal brazier is a large, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8800.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy workshop brazier with broad feet and blackened sides.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatWroughtIronBrazier() => new()
	{
		Metadata = new("medieval_heat_wrought_iron_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8473", []),
		Noun = "brazier",
		ShortDescription = "a wrought-iron portable brazier",
		LongDescription = null,
		FullDescription = "This wrought-iron portable brazier is a medium-sized, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5400.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable metal brazier for charcoal or other solid fuel, suited to warming a small room or work area.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightAshWatchTorch() => new()
	{
		Metadata = new("medieval_light_ash_watch_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8502", []),
		Noun = "torch",
		ShortDescription = "an ash watch torch",
		LongDescription = null,
		FullDescription = "This ash watch torch is a small, workmanlike torch built from ash boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_3Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavier torch for watchmen, night patrols, and road or wall service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightBeechWallTorch() => new()
	{
		Metadata = new("medieval_light_beech_wall_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8531", []),
		Noun = "torch",
		ShortDescription = "a beech wall torch",
		LongDescription = null,
		FullDescription = "This beech wall torch is a small, workmanlike torch built from beech boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 690.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain torch sized for a wall bracket or carried use in a hall, yard, or gatehouse.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightBirchBarkTorch() => new()
	{
		Metadata = new("medieval_light_birch_bark_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8560", []),
		Noun = "torch",
		ShortDescription = "a birch-bark torch",
		LongDescription = null,
		FullDescription = "This birch-bark torch is a small, workmanlike torch built from birch boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 430.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light torch wrapped with curled bark for quick fire and short outdoor use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightBrassHangingLamp() => new()
	{
		Metadata = new("medieval_light_brass_hanging_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8589", []),
		Noun = "lamp",
		ShortDescription = "a brass hanging lamp",
		LongDescription = null,
		FullDescription = "This brass hanging lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal lamp with loops and a suspended body for hanging from a chain, beam, or bracket.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightBrassWallSconce() => new()
	{
		Metadata = new("medieval_light_brass_wall_sconce", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8618", []),
		Noun = "sconce",
		ShortDescription = "a brass wall sconce",
		LongDescription = null,
		FullDescription = "This brass wall sconce is a small, well-made sconce worked from brass. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 760.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Wall_Shelf"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished brass wall support for chamber lighting, made as a decorative holder rather than a light source.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightBrightWorkCandle() => new()
	{
		Metadata = new("medieval_light_bright_work_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8647", []),
		Noun = "candle",
		ShortDescription = "a bright work candle",
		LongDescription = null,
		FullDescription = "This bright work candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 130.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle_Bright"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Brighter candle for close work, accounts, sewing, cutting, or inspections after dark.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightBronzeLampTray() => new()
	{
		Metadata = new("medieval_light_bronze_lamp_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8676", []),
		Noun = "tray",
		ShortDescription = "a bronze lamp tray",
		LongDescription = null,
		FullDescription = "This bronze lamp tray is a small, well-made tray worked from bronze. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 950.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine tray sized for small lamps, wick tools, or oil spills; it provides surface storage only.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightBronzeTableLamp() => new()
	{
		Metadata = new("medieval_light_bronze_table_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8705", []),
		Noun = "lamp",
		ShortDescription = "a bronze table oil lamp",
		LongDescription = null,
		FullDescription = "This bronze table oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Durable bronze lamp for a table, counter, desk, or high-status chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightBundleOfBrands() => new()
	{
		Metadata = new("medieval_light_bundle_of_brands", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8734", []),
		Noun = "brand",
		ShortDescription = "a bundle of pine brands",
		LongDescription = null,
		FullDescription = "This bundle of pine brands is a small, workmanlike brand built from pine boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Several thin firebrands bound together for short-lived carried light.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightCandleStorageRack() => new()
	{
		Metadata = new("medieval_light_candle_storage_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8763", []),
		Noun = "rack",
		ShortDescription = "a candle storage rack",
		LongDescription = null,
		FullDescription = "This candle storage rack is a medium-sized, workmanlike rack built from pine boards. A row of shallow sockets and notches runs across the frame, with soot marks above the most used places. The lower rail is smoothed by handling. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wide_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open rack for spare candles, tapers, spills, and small lighting goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightCedarResinTorch() => new()
	{
		Metadata = new("medieval_light_cedar_resin_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8792", []),
		Noun = "torch",
		ShortDescription = "a cedar resin torch",
		LongDescription = null,
		FullDescription = "This cedar resin torch is a small, workmanlike torch built from cedar boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 600.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Resin-scented torch suited to gates, yards, docks, and dark approaches.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightClaySaucerLamp() => new()
	{
		Metadata = new("medieval_light_clay_saucer_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8821", []),
		Noun = "lamp",
		ShortDescription = "a clay saucer oil lamp",
		LongDescription = null,
		FullDescription = "This clay saucer oil lamp is a very small, workmanlike lamp formed from fired clay. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "clay",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open saucer-like lamp body with a pinched place for a wick and liquid fuel.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightCoveredBoatLantern() => new()
	{
		Metadata = new("medieval_light_covered_boat_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8850", []),
		Noun = "lantern",
		ShortDescription = "a covered boat lantern",
		LongDescription = null,
		FullDescription = "This covered boat lantern is a small, well-made lantern worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1350.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shielded brass lantern with a covered flame suitable for damp air and night travel.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightCupWaxCandle() => new()
	{
		Metadata = new("medieval_light_cup_wax_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8879", []),
		Noun = "candle",
		ShortDescription = "a small cup candle",
		LongDescription = null,
		FullDescription = "This small cup candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low candle set into a shallow cup-like body for stable table or shelf use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightDeskLantern() => new()
	{
		Metadata = new("medieval_light_desk_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8908", []),
		Noun = "lantern",
		ShortDescription = "a small desk lantern",
		LongDescription = null,
		FullDescription = "This small desk lantern is a small, well-made lantern worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 980.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stable lantern for reading, accounts, copying, or close table work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightDoubleWickedCandle() => new()
	{
		Metadata = new("medieval_light_double_wicked_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8937", []),
		Noun = "candle",
		ShortDescription = "a double-wicked candle",
		LongDescription = null,
		FullDescription = "This double-wicked candle is a very small, well-made candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 150.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle_Bright"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-made candle with two visible wicks, intended to cast stronger light for work or display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightEarthenwareWickLamp() => new()
	{
		Metadata = new("medieval_light_earthenware_wick_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8966", []),
		Noun = "lamp",
		ShortDescription = "an earthenware wick lamp",
		LongDescription = null,
		FullDescription = "This earthenware wick lamp is a very small, workmanlike lamp formed from earthenware. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small fired earthenware oil lamp with a rounded reservoir and wick mouth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightFiredClayPinchedLamp() => new()
	{
		Metadata = new("medieval_light_fired_clay_pinched_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:8995", []),
		Noun = "lamp",
		ShortDescription = "a pinched fired-clay lamp",
		LongDescription = null,
		FullDescription = "This pinched fired-clay lamp is a very small, workmanlike lamp formed from fired clay. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fired clay",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain hand-pinched lamp with an open oil bowl and narrowed wick point.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightGatehouseCressetTorch() => new()
	{
		Metadata = new("medieval_light_gatehouse_cresset_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9024", []),
		Noun = "cresset",
		ShortDescription = "a gatehouse cresset torch",
		LongDescription = null,
		FullDescription = "This gatehouse cresset torch is a medium-sized, workmanlike cresset worked from wrought iron. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3100.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Torch_3Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Iron cresset-style torch made for strong open flame near a gate or watch post.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightGlassWindowLantern() => new()
	{
		Metadata = new("medieval_light_glass_window_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9053", []),
		Noun = "lantern",
		ShortDescription = "a glass-window lantern",
		LongDescription = null,
		FullDescription = "This glass-window lantern is a small, well-made lantern made from glass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lantern with glass panes set into a metal frame, suitable for brighter indoor display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightGlazedCeramicLamp() => new()
	{
		Metadata = new("medieval_light_glazed_ceramic_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9082", []),
		Noun = "lamp",
		ShortDescription = "a glazed ceramic lamp",
		LongDescription = null,
		FullDescription = "This glazed ceramic lamp is a very small, well-made lamp formed from glazed ceramic. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 380.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glazed ceramic",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small glazed lamp with a smoother finish and enclosed reservoir.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightHallHangingLantern() => new()
	{
		Metadata = new("medieval_light_hall_hanging_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9111", []),
		Noun = "lantern",
		ShortDescription = "a hall hanging lantern",
		LongDescription = null,
		FullDescription = "This hall hanging lantern is a medium-sized, well-made lantern worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Larger hanging lantern intended to light a hall, refectory, or shared chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightHangingLampFrame() => new()
	{
		Metadata = new("medieval_light_hanging_lamp_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9140", []),
		Noun = "frame",
		ShortDescription = "a hanging lamp frame",
		LongDescription = null,
		FullDescription = "This hanging lamp frame is a medium-sized, workmanlike frame worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1900.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Iron frame meant to cradle a small lamp or candle dish while suspended.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightHempRushlight() => new()
	{
		Metadata = new("medieval_light_hemp_rushlight", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9169", []),
		Noun = "rushlight",
		ShortDescription = "a hemp-wick rushlight",
		LongDescription = null,
		FullDescription = "This hemp-wick rushlight is a tiny, workmanlike rushlight made from woven hemp. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cheap small light made around a fibrous wick, useful where only weak illumination is needed.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightHornWindowLantern() => new()
	{
		Metadata = new("medieval_light_horn_window_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9198", []),
		Noun = "lantern",
		ShortDescription = "a horn-window lantern",
		LongDescription = null,
		FullDescription = "This horn-window lantern is a small, well-made lantern worked from horn. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 950.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lantern with translucent horn panels protecting the flame from draughts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightIronCandleTray() => new()
	{
		Metadata = new("medieval_light_iron_candle_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9227", []),
		Noun = "tray",
		ShortDescription = "an iron candle tray",
		LongDescription = null,
		FullDescription = "This iron candle tray is a small, workmanlike tray worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shallow metal tray for carrying or grouping candles; it is a support surface, not a light source on its own.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightIronTorchBracket() => new()
	{
		Metadata = new("medieval_light_iron_torch_bracket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9256", []),
		Noun = "bracket",
		ShortDescription = "an iron torch bracket",
		LongDescription = null,
		FullDescription = "This iron torch bracket is a small, workmanlike bracket worked from wrought iron. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Wall_Shelf"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stout bracket shaped to support a torch or brand against a wall.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightLampMendingTray() => new()
	{
		Metadata = new("medieval_light_lamp_mending_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9285", []),
		Noun = "tray",
		ShortDescription = "a lamp-mending tray",
		LongDescription = null,
		FullDescription = "This lamp-mending tray is a small, workmanlike tray built from beech boards. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shallow tray for wicks, snuffers, lamp pins, and small lighting repairs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightLanternCarryingHook() => new()
	{
		Metadata = new("medieval_light_lantern_carrying_hook", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9314", []),
		Noun = "hook",
		ShortDescription = "a lantern carrying hook",
		LongDescription = null,
		FullDescription = "This lantern carrying hook is a small, workmanlike hook worked from wrought iron. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Curved iron hook for carrying or hanging a lantern; it has no container or light behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightLiddedOilLamp() => new()
	{
		Metadata = new("medieval_light_lidded_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9342", []),
		Noun = "lamp",
		ShortDescription = "a lidded oil lamp",
		LongDescription = null,
		FullDescription = "This lidded oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal oil lamp with a fitted lid, handle, and narrowed wick opening.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightLinenWickNightLight() => new()
	{
		Metadata = new("medieval_light_linen_wick_night_light", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9371", []),
		Noun = "night-light",
		ShortDescription = "a linen-wick night-light",
		LongDescription = null,
		FullDescription = "This linen-wick night-light is a tiny, workmanlike night-light made from woven linen. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 40.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle_Long"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Very small dim light for marking a chamber or passage through the night.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightLongBeeswaxTaper() => new()
	{
		Metadata = new("medieval_light_long_beeswax_taper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9400", []),
		Noun = "taper",
		ShortDescription = "a long beeswax taper",
		LongDescription = null,
		FullDescription = "This long beeswax taper is a tiny, workmanlike taper made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Slender household taper intended to give modest light over a longer interval.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightLongWatchCandle() => new()
	{
		Metadata = new("medieval_light_long_watch_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9429", []),
		Noun = "candle",
		ShortDescription = "a long-burning watch candle",
		LongDescription = null,
		FullDescription = "This long-burning watch candle is a small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle_Long"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Thick slow-burning candle for night watches, sickrooms, and long household vigils.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightOakHandTorch() => new()
	{
		Metadata = new("medieval_light_oak_hand_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9458", []),
		Noun = "torch",
		ShortDescription = "an oak hand torch",
		LongDescription = null,
		FullDescription = "This oak hand torch is a small, workmanlike torch built from oak boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stout hand torch made from an oak shaft with a wrapped burning head.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightOakLampStand() => new()
	{
		Metadata = new("medieval_light_oak_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9487", []),
		Noun = "stand",
		ShortDescription = "an oak lamp stand",
		LongDescription = null,
		FullDescription = "This oak lamp stand is a medium-sized, workmanlike stand built from oak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small stand with a flat top for a lamp, lantern, or candle dish.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightOakTorchStand() => new()
	{
		Metadata = new("medieval_light_oak_torch_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9516", []),
		Noun = "stand",
		ShortDescription = "an oak torch stand",
		LongDescription = null,
		FullDescription = "This oak torch stand is a medium-sized, workmanlike stand built from oak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Freestanding wooden stand with a metal cup or socket for a torch or lantern.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightPineLanternPole() => new()
	{
		Metadata = new("medieval_light_pine_lantern_pole", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9545", []),
		Noun = "pole",
		ShortDescription = "a pine lantern pole",
		LongDescription = null,
		FullDescription = "This pine lantern pole is a large, workmanlike pole built from pine boards. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain pole with notches and an iron hook for suspending a lantern.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightPinePitchTorch() => new()
	{
		Metadata = new("medieval_light_pine_pitch_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9573", []),
		Noun = "torch",
		ShortDescription = "a pine pitch torch",
		LongDescription = null,
		FullDescription = "This pine pitch torch is a small, workmanlike torch built from pine boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_3Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pitch-rich pine torch intended for stronger outdoor light and longer burning.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightPineSpillTaper() => new()
	{
		Metadata = new("medieval_light_pine_spill_taper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9602", []),
		Noun = "taper",
		ShortDescription = "a pine spill taper",
		LongDescription = null,
		FullDescription = "This pine spill taper is a tiny, workmanlike taper built from pine boards. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Thin resinous spill used to transfer fire from hearth to candle, lamp, or torch.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightPlainDippedCandle() => new()
	{
		Metadata = new("medieval_light_plain_dipped_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9631", []),
		Noun = "candle",
		ShortDescription = "a plain dipped candle",
		LongDescription = null,
		FullDescription = "This plain dipped candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple repeatedly dipped candle with a slightly uneven body and ordinary wick.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightPricketCandlestick() => new()
	{
		Metadata = new("medieval_light_pricket_candlestick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9660", []),
		Noun = "candlestick",
		ShortDescription = "a pricket candlestick",
		LongDescription = null,
		FullDescription = "This pricket candlestick is a small, well-made candlestick worked from bronze. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 880.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Candlestick with a central spike and shallow base, useful as a candle support.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightRingChandelier() => new()
	{
		Metadata = new("medieval_light_ring_chandelier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9689", []),
		Noun = "chandelier",
		ShortDescription = "a ring chandelier",
		LongDescription = null,
		FullDescription = "This ring chandelier is a large, well-made chandelier worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 7400.0,
		Cost = 74.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy ring with candle cups or small lamp rests, intended as a suspended support rather than an active light source.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightRolledWaxCandle() => new()
	{
		Metadata = new("medieval_light_rolled_wax_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9718", []),
		Noun = "candle",
		ShortDescription = "a rolled beeswax candle",
		LongDescription = null,
		FullDescription = "This rolled beeswax candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 100.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rolled wax candle with the faint ridges of layered sheet wax around the body.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightRushlightHolder() => new()
	{
		Metadata = new("medieval_light_rushlight_holder", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9747", []),
		Noun = "holder",
		ShortDescription = "a rushlight holder",
		LongDescription = null,
		FullDescription = "This rushlight holder is a small, workmanlike holder worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple clamp-and-dish holder for a small rushlight or taper; it is a support object only.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightShipboardSignalFire() => new()
	{
		Metadata = new("medieval_light_shipboard_signal_fire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9776", []),
		Noun = "beacon",
		ShortDescription = "a shipboard signal fire",
		LongDescription = null,
		FullDescription = "This shipboard signal fire is a medium-sized, workmanlike beacon worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SignalFire"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Contained signal-fire basket for docks, ships, harbour walls, and night signalling.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightShortBeeswaxTaper() => new()
	{
		Metadata = new("medieval_light_short_beeswax_taper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9805", []),
		Noun = "taper",
		ShortDescription = "a short beeswax taper",
		LongDescription = null,
		FullDescription = "This short beeswax taper is a tiny, workmanlike taper made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle_Bright"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Short clean-burning taper for brief domestic tasks, desk work, or bedside use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightStableLantern() => new()
	{
		Metadata = new("medieval_light_stable_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9834", []),
		Noun = "lantern",
		ShortDescription = "a stable lantern",
		LongDescription = null,
		FullDescription = "This stable lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1450.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain protected lantern for barns, stables, sheds, and outbuildings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightTarredRopeTorch() => new()
	{
		Metadata = new("medieval_light_tarred_rope_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9863", []),
		Noun = "torch",
		ShortDescription = "a tarred rope torch",
		LongDescription = null,
		FullDescription = "This tarred rope torch is a small, workmanlike torch made from woven hemp. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Torch with a wrapped rope head, meant to hold fuel and burn steadily.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightTerracottaSpoutedLamp() => new()
	{
		Metadata = new("medieval_light_terracotta_spouted_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9892", []),
		Noun = "lamp",
		ShortDescription = "a terracotta spouted lamp",
		LongDescription = null,
		FullDescription = "This terracotta spouted lamp is a very small, workmanlike lamp formed from terracotta. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 330.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "terracotta",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Reddish terracotta lamp with a pinched spout for the wick and a low oil chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightThickHouseholdCandle() => new()
	{
		Metadata = new("medieval_light_thick_household_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9921", []),
		Noun = "candle",
		ShortDescription = "a thick household candle",
		LongDescription = null,
		FullDescription = "This thick household candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Ordinary household candle with enough wax for sustained room or table light.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightTravelOilLantern() => new()
	{
		Metadata = new("medieval_light_travel_oil_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9950", []),
		Noun = "lantern",
		ShortDescription = "a travel oil lantern",
		LongDescription = null,
		FullDescription = "This travel oil lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact shielded lantern with a carrying handle and reinforced frame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightTwoSpoutLamp() => new()
	{
		Metadata = new("medieval_light_two_spout_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:9979", []),
		Noun = "lamp",
		ShortDescription = "a two-spout oil lamp",
		LongDescription = null,
		FullDescription = "This two-spout oil lamp is a small, workmanlike lamp formed from earthenware. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broader earthenware lamp with two wick points for slightly wider illumination.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWalnutCandleStand() => new()
	{
		Metadata = new("medieval_light_walnut_candle_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10008", []),
		Noun = "stand",
		ShortDescription = "a walnut candle stand",
		LongDescription = null,
		FullDescription = "This walnut candle stand is a medium-sized, well-made stand built from walnut boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-finished stand intended to support a candlestick or small lamp.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWatchSignalBasket() => new()
	{
		Metadata = new("medieval_light_watch_signal_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10037", []),
		Noun = "beacon",
		ShortDescription = "a watch signal basket",
		LongDescription = null,
		FullDescription = "This watch signal basket is a medium-sized, workmanlike beacon worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3800.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SignalFire"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal basket for a bright warning or signalling flame on a tower, wall, or hilltop.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWillowYardTorch() => new()
	{
		Metadata = new("medieval_light_willow_yard_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10066", []),
		Noun = "torch",
		ShortDescription = "a willow yard torch",
		LongDescription = null,
		FullDescription = "This willow yard torch is a small, workmanlike torch built from willow boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light torch for yard work, stables, or brief errands between buildings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWorkshopLantern() => new()
	{
		Metadata = new("medieval_light_workshop_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10095", []),
		Noun = "lantern",
		ShortDescription = "a workshop lantern",
		LongDescription = null,
		FullDescription = "This workshop lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rugged lantern for benches, sheds, and workshops where the flame needs protection.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWroughtIronOilLantern() => new()
	{
		Metadata = new("medieval_light_wrought_iron_oil_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10124", []),
		Noun = "lantern",
		ShortDescription = "a wrought-iron oil lantern",
		LongDescription = null,
		FullDescription = "This wrought-iron oil lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1350.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy lantern frame with a protected reservoir, handle, and darkened ironwork.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWroughtIronWallSconce() => new()
	{
		Metadata = new("medieval_light_wrought_iron_wall_sconce", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10153", []),
		Noun = "sconce",
		ShortDescription = "a wrought-iron wall sconce",
		LongDescription = null,
		FullDescription = "This wrought-iron wall sconce is a small, workmanlike sconce worked from wrought iron. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Wall_Shelf"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wall sconce with a small support shelf or cup for a candle or lamp; it does not produce light by itself.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatNorthernWinterBrazier() => new()
	{
		Metadata = new("medieval_heat_northern_winter_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10182", []),
		Noun = "brazier",
		ShortDescription = "a winter iron brazier",
		LongDescription = null,
		FullDescription = "This winter iron brazier is a large, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9800.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large blackened brazier suited to cold chambers, workshops, and guardrooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightNorthernBirchSplintTorch() => new()
	{
		Metadata = new("medieval_light_northern_birch_splint_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10211", []),
		Noun = "torch",
		ShortDescription = "a birch splint torch",
		LongDescription = null,
		FullDescription = "This birch splint torch is a small, workmanlike torch built from birch boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 390.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fast-lighting splint torch for short outdoor or outbuilding use.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
