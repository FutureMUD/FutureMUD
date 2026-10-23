#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalTablewareBrassTankard() => new()
	{
		Metadata = new("medieval_tableware_brass_tankard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:444", []),
		Noun = "tankard",
		ShortDescription = "a brass tankard",
		LongDescription = null,
		FullDescription = "This brass tankard is a small, well-made tankard worked from brass. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_UKPint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bright metal tankard for ale or mead.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassWaterBucket() => new()
	{
		Metadata = new("medieval_tableware_brass_water_bucket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:475", []),
		Noun = "bucket",
		ShortDescription = "a brass water bucket",
		LongDescription = null,
		FullDescription = "This brass water bucket is a medium-sized, well-made bucket worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3200.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal water vessel for chambers, kitchens, or guests.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBreadPlatter() => new()
	{
		Metadata = new("medieval_tableware_bread_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:505", []),
		Noun = "platter",
		ShortDescription = "a broad bread platter",
		LongDescription = null,
		FullDescription = "This broad bread platter is a medium-sized, workmanlike platter built from beech boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1000.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad platter for bread rounds, trenchers, and flat loaves.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBreadTrencher() => new()
	{
		Metadata = new("medieval_tableware_bread_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:536", []),
		Noun = "trencher",
		ShortDescription = "a thick bread trencher",
		LongDescription = null,
		FullDescription = "This thick bread trencher is a small, workmanlike trencher made from firm baked bread. The centre is pressed shallow, while the rim is left thick and firm enough to hold food. The crust is dry and browned across the upper edge. The crust is firm and dry, with a shallow centre pressed down to take food.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bread",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Edible trencher-like serving base; modelled as an open solid-food container for table service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBreakfastTray() => new()
	{
		Metadata = new("medieval_tableware_breakfast_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:567", []),
		Noun = "tray",
		ShortDescription = "a low breakfast tray",
		LongDescription = null,
		FullDescription = "This low breakfast tray is a medium-sized, well-made tray built from cedar boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low tray for carrying a small meal or chamber service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBronzeDrinkingCup() => new()
	{
		Metadata = new("medieval_tableware_bronze_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:598", []),
		Noun = "cup",
		ShortDescription = "a bronze drinking cup",
		LongDescription = null,
		FullDescription = "This bronze drinking cup is a very small, well-made cup worked from bronze. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished metal cup for higher-status drink service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBronzeEwer() => new()
	{
		Metadata = new("medieval_tableware_bronze_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:629", []),
		Noun = "ewer",
		ShortDescription = "a bronze table ewer",
		LongDescription = null,
		FullDescription = "This bronze table ewer is a small, well-made ewer worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1350.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine metal ewer for table or chamber service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBronzeGoblet() => new()
	{
		Metadata = new("medieval_tableware_bronze_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:660", []),
		Noun = "goblet",
		ShortDescription = "a bronze table goblet",
		LongDescription = null,
		FullDescription = "This bronze table goblet is a small, well-made goblet worked from bronze. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_WineGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine goblet using a wine-glass capacity profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBronzeHandBasin() => new()
	{
		Metadata = new("medieval_tableware_bronze_hand_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:691", []),
		Noun = "basin",
		ShortDescription = "a bronze hand basin",
		LongDescription = null,
		FullDescription = "This bronze hand basin is a medium-sized, well-made basin worked from bronze. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3300.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine hand basin for formal washing service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBronzePitcher() => new()
	{
		Metadata = new("medieval_tableware_bronze_pitcher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:721", []),
		Noun = "pitcher",
		ShortDescription = "a bronze water pitcher",
		LongDescription = null,
		FullDescription = "This bronze water pitcher is a small, well-made pitcher worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1700.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal pitcher for formal water service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBronzeServingPlatter() => new()
	{
		Metadata = new("medieval_tableware_bronze_serving_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:752", []),
		Noun = "platter",
		ShortDescription = "a bronze serving platter",
		LongDescription = null,
		FullDescription = "This bronze serving platter is a medium-sized, well-made platter worked from bronze. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1750.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
		BuilderNotes = "Metal platter for formal hall service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareButcheredMeatTray() => new()
	{
		Metadata = new("medieval_tableware_butchered_meat_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:783", []),
		Noun = "tray",
		ShortDescription = "a butchered-meat tray",
		LongDescription = null,
		FullDescription = "This butchered-meat tray is a medium-sized, workmanlike tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy tray for carrying raw or cooked meat portions within a household.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareButterDish() => new()
	{
		Metadata = new("medieval_tableware_butter_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:814", []),
		Noun = "dish",
		ShortDescription = "a small butter dish",
		LongDescription = null,
		FullDescription = "This small butter dish is a very small, workmanlike dish formed from stoneware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 350.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small table dish for butter, curds, or soft cheese.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCarvedBoneTray() => new()
	{
		Metadata = new("medieval_tableware_carved_bone_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:845", []),
		Noun = "tray",
		ShortDescription = "a carved bone tray",
		LongDescription = null,
		FullDescription = "This carved bone tray is a small, well-made tray worked from bone. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small decorated tray for refined table service or guest offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCarvedIvoryBowl() => new()
	{
		Metadata = new("medieval_tableware_carved_ivory_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:876", []),
		Noun = "bowl",
		ShortDescription = "a carved ivory bowl",
		LongDescription = null,
		FullDescription = "This carved ivory bowl is a very small, well-made bowl worked from ivory. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivory",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small luxury bowl for spices, nuts, sweetmeats, or display service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCeramicDecanter() => new()
	{
		Metadata = new("medieval_tableware_ceramic_decanter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:907", []),
		Noun = "decanter",
		ShortDescription = "a ceramic wine decanter",
		LongDescription = null,
		FullDescription = "This ceramic wine decanter is a small, well-made decanter formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Ceramic decanter for table wine service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCeramicPaintedPlate() => new()
	{
		Metadata = new("medieval_tableware_ceramic_painted_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:938", []),
		Noun = "plate",
		ShortDescription = "a painted ceramic plate",
		LongDescription = null,
		FullDescription = "This painted ceramic plate is a small, well-made plate formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorated ceramic plate suited to wealthier household service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCheeseBoard() => new()
	{
		Metadata = new("medieval_tableware_cheese_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:969", []),
		Noun = "board",
		ShortDescription = "a broad cheese board",
		LongDescription = null,
		FullDescription = "This broad cheese board is a medium-sized, workmanlike board built from maple boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1050.0,
		Cost = 13.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "maple",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Serving board for cheeses, fruit, and table knives.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCheesePlate() => new()
	{
		Metadata = new("medieval_tableware_cheese_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1000", []),
		Noun = "plate",
		ShortDescription = "a cheese plate",
		LongDescription = null,
		FullDescription = "This cheese plate is a small, workmanlike plate built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wooden plate for cheese, fruit, and other dry table portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCoarseClayBowl() => new()
	{
		Metadata = new("medieval_tableware_coarse_clay_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1031", []),
		Noun = "bowl",
		ShortDescription = "a coarse clay bowl",
		LongDescription = null,
		FullDescription = "This coarse clay bowl is a small, plain bowl formed from fired clay. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Poor,
		WeightInGrams = 720.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "clay",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low-status open bowl for rough household or servant use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCommunalMazer() => new()
	{
		Metadata = new("medieval_tableware_communal_mazer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1062", []),
		Noun = "mazer",
		ShortDescription = "a communal maple mazer",
		LongDescription = null,
		FullDescription = "This communal maple mazer is a small, well-made mazer built from maple boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "maple",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Pint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared wooden drinking bowl for ale, mead, or spiced drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCordedWoodenPlatter() => new()
	{
		Metadata = new("medieval_tableware_corded_wooden_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1093", []),
		Noun = "platter",
		ShortDescription = "a corded wooden platter",
		LongDescription = null,
		FullDescription = "This corded wooden platter is a medium-sized, workmanlike platter built from pine boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 800.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cheap platter with a simple corded edge for grip.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCoveredEarthenwareJug() => new()
	{
		Metadata = new("medieval_tableware_covered_earthenware_jug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1124", []),
		Noun = "jug",
		ShortDescription = "a covered earthenware jug",
		LongDescription = null,
		FullDescription = "This covered earthenware jug is a small, workmanlike jug formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Covered jug for table drink; cover is descriptive, not an openable component.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCoveredSaltDish() => new()
	{
		Metadata = new("medieval_tableware_covered_salt_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1155", []),
		Noun = "dish",
		ShortDescription = "a lidded salt dish",
		LongDescription = null,
		FullDescription = "This lidded salt dish is a very small, well-made dish worked from pewter. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 320.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine salt dish; the component models it as open table service rather than a locking container.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCoveredWaterskin() => new()
	{
		Metadata = new("medieval_tableware_covered_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1186", []),
		Noun = "waterskin",
		ShortDescription = "a covered waterskin",
		LongDescription = null,
		FullDescription = "This covered waterskin is a small, workmanlike waterskin made from cured animal skin. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "animal skin",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Skin vessel for water; included here as household and travel liquid ware, not wearable gear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareCupBearingTray() => new()
	{
		Metadata = new("medieval_tableware_cup_bearing_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1217", []),
		Noun = "tray",
		ShortDescription = "a cup-bearing tray",
		LongDescription = null,
		FullDescription = "This cup-bearing tray is a medium-sized, workmanlike tray built from ash boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tray sized for moving multiple cups or beakers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareDeepEatingBowl() => new()
	{
		Metadata = new("medieval_tableware_deep_eating_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1248", []),
		Noun = "bowl",
		ShortDescription = "a deep eating bowl",
		LongDescription = null,
		FullDescription = "This deep eating bowl is a small, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Deep bowl for stew-like or grain-heavy meals; not intended as liquid storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareDrinkingHorn() => new()
	{
		Metadata = new("medieval_tableware_drinking_horn", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1279", []),
		Noun = "horn",
		ShortDescription = "a polished drinking horn",
		LongDescription = null,
		FullDescription = "This polished drinking horn is a small, well-made horn worked from horn. The drinking mouth is polished smooth, and the tapering body curves down to a capped point. A small resting foot keeps it steady when set down. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Pint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished horn vessel for feasts, halls, or travellers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareEarthenwareBeaker() => new()
	{
		Metadata = new("medieval_tableware_earthenware_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1310", []),
		Noun = "beaker",
		ShortDescription = "an earthenware beaker",
		LongDescription = null,
		FullDescription = "This earthenware beaker is a very small, workmanlike beaker formed from earthenware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 340.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Straight-sided cup for common drinks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareEarthenwareBottle() => new()
	{
		Metadata = new("medieval_tableware_earthenware_bottle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1341", []),
		Noun = "bottle",
		ShortDescription = "an earthenware bottle",
		LongDescription = null,
		FullDescription = "This earthenware bottle is a small, workmanlike bottle formed from earthenware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_PintBottle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small ceramic bottle for household liquids.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareEarthenwareDinnerPlate() => new()
	{
		Metadata = new("medieval_tableware_earthenware_dinner_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1372", []),
		Noun = "plate",
		ShortDescription = "an earthenware dinner plate",
		LongDescription = null,
		FullDescription = "This earthenware dinner plate is a small, workmanlike plate formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Common ceramic plate for individual table service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareEarthenwareDrinkingCup() => new()
	{
		Metadata = new("medieval_tableware_earthenware_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1403", []),
		Noun = "cup",
		ShortDescription = "an earthenware drinking cup",
		LongDescription = null,
		FullDescription = "This earthenware drinking cup is a very small, workmanlike cup formed from earthenware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Common cup for water, ale, watered wine, or milk.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareEarthenwareFlagon() => new()
	{
		Metadata = new("medieval_tableware_earthenware_flagon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1434", []),
		Noun = "flagon",
		ShortDescription = "an earthenware flagon",
		LongDescription = null,
		FullDescription = "This earthenware flagon is a small, workmanlike flagon formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Common ceramic flagon for drink service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareEarthenwareMug() => new()
	{
		Metadata = new("medieval_tableware_earthenware_mug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1465", []),
		Noun = "mug",
		ShortDescription = "an earthenware mug",
		LongDescription = null,
		FullDescription = "This earthenware mug is a small, workmanlike mug formed from earthenware. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_HalfPint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Handled clay mug for table drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareEarthenwareTableJug() => new()
	{
		Metadata = new("medieval_tableware_earthenware_table_jug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1496", []),
		Noun = "jug",
		ShortDescription = "an earthenware table jug",
		LongDescription = null,
		FullDescription = "This earthenware table jug is a small, workmanlike jug formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Household jug for water, milk, ale, or watered wine.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareEarthenwareWashBasin() => new()
	{
		Metadata = new("medieval_tableware_earthenware_wash_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1527", []),
		Noun = "basin",
		ShortDescription = "an earthenware wash basin",
		LongDescription = null,
		FullDescription = "This earthenware wash basin is a medium-sized, workmanlike basin formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Common household basin for washing hands, cups, or utensils.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareEwerTray() => new()
	{
		Metadata = new("medieval_tableware_ewer_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1557", []),
		Noun = "tray",
		ShortDescription = "an ewer standing tray",
		LongDescription = null,
		FullDescription = "This ewer standing tray is a small, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
		BuilderNotes = "Metal tray for setting an ewer or flagon without wetting the table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareFineCeramicBowl() => new()
	{
		Metadata = new("medieval_tableware_fine_ceramic_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1588", []),
		Noun = "bowl",
		ShortDescription = "a fine ceramic bowl",
		LongDescription = null,
		FullDescription = "This fine ceramic bowl is a small, well-made bowl formed from ceramic. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 640.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Well-fired decorated bowl for higher-status service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareFineCourtCup() => new()
	{
		Metadata = new("medieval_tableware_fine_court_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1619", []),
		Noun = "cup",
		ShortDescription = "a fine court cup",
		LongDescription = null,
		FullDescription = "This fine court cup is a very small, finely made cup worked from gold. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 360.0,
		Cost = 420.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_SmallWineGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Exceptional precious-metal cup for court or treasury service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareFishPlatter() => new()
	{
		Metadata = new("medieval_tableware_fish_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1650", []),
		Noun = "platter",
		ShortDescription = "a long fish platter",
		LongDescription = null,
		FullDescription = "This long fish platter is a medium-sized, workmanlike platter formed from earthenware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 13.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long platter for fish or other elongated dishes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareFlatFishPlate() => new()
	{
		Metadata = new("medieval_tableware_flat_fish_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1681", []),
		Noun = "plate",
		ShortDescription = "a flat fish plate",
		LongDescription = null,
		FullDescription = "This flat fish plate is a small, workmanlike plate formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Oblong plate for small fish, cutlets, or long portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareFruitDish() => new()
	{
		Metadata = new("medieval_tableware_fruit_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1712", []),
		Noun = "dish",
		ShortDescription = "a shallow fruit dish",
		LongDescription = null,
		FullDescription = "This shallow fruit dish is a small, workmanlike dish formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shallow dish for fruit, nuts, or small preserved foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareFruitPlatter() => new()
	{
		Metadata = new("medieval_tableware_fruit_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1743", []),
		Noun = "platter",
		ShortDescription = "a shallow fruit platter",
		LongDescription = null,
		FullDescription = "This shallow fruit platter is a medium-sized, workmanlike platter formed from stoneware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1450.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shallow platter for fruit, nuts, or preserved goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareGlassBeaker() => new()
	{
		Metadata = new("medieval_tableware_glass_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1774", []),
		Noun = "beaker",
		ShortDescription = "a glass beaker",
		LongDescription = null,
		FullDescription = "This glass beaker is a very small, well-made beaker made from glass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 190.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine glass beaker for wine, ale, or clear drinks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareGlassBottle() => new()
	{
		Metadata = new("medieval_tableware_glass_bottle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1805", []),
		Noun = "bottle",
		ShortDescription = "a stoppered glass bottle",
		LongDescription = null,
		FullDescription = "This stoppered glass bottle is a small, well-made bottle made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_WineBottle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stoppered bottle profile for wine, oil, vinegar, or fine liquids.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareGlassDecanter() => new()
	{
		Metadata = new("medieval_tableware_glass_decanter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1836", []),
		Noun = "decanter",
		ShortDescription = "a glass wine decanter",
		LongDescription = null,
		FullDescription = "This glass wine decanter is a small, well-made decanter made from glass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 780.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Transparent decanter for wine or spiced drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareGlassDrinkingCup() => new()
	{
		Metadata = new("medieval_tableware_glass_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1867", []),
		Noun = "cup",
		ShortDescription = "a glass drinking cup",
		LongDescription = null,
		FullDescription = "This glass drinking cup is a very small, well-made cup made from glass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 180.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fragile transparent cup for fine drink service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareGlassGoblet() => new()
	{
		Metadata = new("medieval_tableware_glass_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1898", []),
		Noun = "goblet",
		ShortDescription = "a glass table goblet",
		LongDescription = null,
		FullDescription = "This glass table goblet is a small, well-made goblet made from glass. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_WineGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fragile glass goblet for fine wine or spiced drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareGlassOilFlask() => new()
	{
		Metadata = new("medieval_tableware_glass_oil_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1929", []),
		Noun = "flask",
		ShortDescription = "a glass oil flask",
		LongDescription = null,
		FullDescription = "This glass oil flask is a very small, well-made flask made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 180.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine flask for oil, scent, or medicine.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareGreenwareTableBowl() => new()
	{
		Metadata = new("medieval_tableware_greenware_table_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1960", []),
		Noun = "bowl",
		ShortDescription = "a green-glazed table bowl",
		LongDescription = null,
		FullDescription = "This green-glazed table bowl is a small, well-made bowl formed from ceramic. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 680.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Glazed bowl for refined table presentation; skins can vary colour or motif.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareHandledOakTray() => new()
	{
		Metadata = new("medieval_tableware_handled_oak_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:1991", []),
		Noun = "tray",
		ShortDescription = "a handled oak tray",
		LongDescription = null,
		FullDescription = "This handled oak tray is a medium-sized, workmanlike tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 15.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "General domestic tray for carrying cups, bowls, and table wares.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareHandledPorringer() => new()
	{
		Metadata = new("medieval_tableware_handled_porringer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2022", []),
		Noun = "porringer",
		ShortDescription = "a handled earthenware porringer",
		LongDescription = null,
		FullDescription = "This handled earthenware porringer is a small, workmanlike porringer formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 640.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Handled bowl for spoon foods, broths, and soft meals; component remains an open container.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareHoneyFlask() => new()
	{
		Metadata = new("medieval_tableware_honey_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2053", []),
		Noun = "flask",
		ShortDescription = "a honey flask",
		LongDescription = null,
		FullDescription = "This honey flask is a very small, workmanlike flask formed from earthenware. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small flask for honey, syrup, or thick sweet liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareHoneycombPlate() => new()
	{
		Metadata = new("medieval_tableware_honeycomb_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2084", []),
		Noun = "plate",
		ShortDescription = "a honeycomb plate",
		LongDescription = null,
		FullDescription = "This honeycomb plate is a small, workmanlike plate formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small plate for honeycomb, sweetmeats, or sticky table treats.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareHornDrinkingCup() => new()
	{
		Metadata = new("medieval_tableware_horn_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2115", []),
		Noun = "cup",
		ShortDescription = "a horn drinking cup",
		LongDescription = null,
		FullDescription = "This horn drinking cup is a very small, workmanlike cup worked from horn. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hollowed horn cup for table or camp drinking.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareHouseholdWaterskin() => new()
	{
		Metadata = new("medieval_tableware_household_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2146", []),
		Noun = "waterskin",
		ShortDescription = "a household waterskin",
		LongDescription = null,
		FullDescription = "This household waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 15.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Leather water container for kitchen, cart, or field use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareHumbleClayCup() => new()
	{
		Metadata = new("medieval_tableware_humble_clay_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2177", []),
		Noun = "cup",
		ShortDescription = "a rough clay cup",
		LongDescription = null,
		FullDescription = "This rough clay cup is a very small, plain cup formed from fired clay. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 300.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "clay",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Very cheap clay cup for rough inns or poor households.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLargeAleStein() => new()
	{
		Metadata = new("medieval_tableware_large_ale_stein", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2208", []),
		Noun = "stein",
		ShortDescription = "a large ale stein",
		LongDescription = null,
		FullDescription = "This large ale stein is a small, well-made stein formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 980.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Stein"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large stoneware drinking vessel using a stein-sized capacity profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLargeHallPlatter() => new()
	{
		Metadata = new("medieval_tableware_large_hall_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2239", []),
		Noun = "platter",
		ShortDescription = "a large hall platter",
		LongDescription = null,
		FullDescription = "This large hall platter is a large, workmanlike platter built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2900.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large wooden platter for communal hall portions; still portable.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLargeServingBowl() => new()
	{
		Metadata = new("medieval_tableware_large_serving_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2270", []),
		Noun = "bowl",
		ShortDescription = "a large serving bowl",
		LongDescription = null,
		FullDescription = "This large serving bowl is a medium-sized, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large open serving bowl for bread, fruit, greens, or thick foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLargeStorageUrn() => new()
	{
		Metadata = new("medieval_tableware_large_storage_urn", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2301", []),
		Noun = "urn",
		ShortDescription = "a large storage urn",
		LongDescription = null,
		FullDescription = "This large storage urn is a large, workmanlike urn formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Quadrantal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large urn for water, oil, or other household liquid storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLargeWashBasin() => new()
	{
		Metadata = new("medieval_tableware_large_wash_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2332", []),
		Noun = "basin",
		ShortDescription = "a large wash basin",
		LongDescription = null,
		FullDescription = "This large wash basin is a medium-sized, workmanlike basin formed from ceramic. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Liquid-holding basin for household handwashing or rinsing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLargeWaterPitcher() => new()
	{
		Metadata = new("medieval_tableware_large_water_pitcher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2362", []),
		Noun = "pitcher",
		ShortDescription = "a large water pitcher",
		LongDescription = null,
		FullDescription = "This large water pitcher is a medium-sized, workmanlike pitcher formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Congius"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Larger household pitcher for refilling cups or basins.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLaundryRinsingBasin() => new()
	{
		Metadata = new("medieval_tableware_laundry_rinsing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2393", []),
		Noun = "basin",
		ShortDescription = "a laundry rinsing basin",
		LongDescription = null,
		FullDescription = "This laundry rinsing basin is a large, workmanlike basin formed from earthenware. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9800.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Drum"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large household basin for rinsing cloth or soaking wares.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLeatherCanteen() => new()
	{
		Metadata = new("medieval_tableware_leather_canteen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2423", []),
		Noun = "canteen",
		ShortDescription = "a leather water canteen",
		LongDescription = null,
		FullDescription = "This leather water canteen is a small, workmanlike canteen made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Canteen"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable water vessel for work, travel, or bedchamber use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLeatherDrinkingCup() => new()
	{
		Metadata = new("medieval_tableware_leather_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2454", []),
		Noun = "cup",
		ShortDescription = "a leather drinking cup",
		LongDescription = null,
		FullDescription = "This leather drinking cup is a very small, workmanlike cup made from worked leather. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Supple leather cup for rough travel or household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLeatherJack() => new()
	{
		Metadata = new("medieval_tableware_leather_jack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2485", []),
		Noun = "jack",
		ShortDescription = "a leather drinking jack",
		LongDescription = null,
		FullDescription = "This leather drinking jack is a small, workmanlike jack made from worked leather. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_UKPint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hardened leather drinking vessel for ale or water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLeatherTravelFlask() => new()
	{
		Metadata = new("medieval_tableware_leather_travel_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2516", []),
		Noun = "flask",
		ShortDescription = "a leather travel flask",
		LongDescription = null,
		FullDescription = "This leather travel flask is a small, workmanlike flask made from worked leather. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable flask for short journeys, messengers, or bedside liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLiddedStonewareStein() => new()
	{
		Metadata = new("medieval_tableware_lidded_stoneware_stein", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2547", []),
		Noun = "stein",
		ShortDescription = "a lidded stoneware stein",
		LongDescription = null,
		FullDescription = "This lidded stoneware stein is a small, well-made stein formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1120.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Stein"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large lidded vessel for ale; lid is descriptive, not a separate openable component.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLinenLinedTray() => new()
	{
		Metadata = new("medieval_tableware_linen_lined_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2578", []),
		Noun = "tray",
		ShortDescription = "a linen-lined tray",
		LongDescription = null,
		FullDescription = "This linen-lined tray is a medium-sized, well-made tray built from plain wood. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 920.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tray with a fitted textile pad for finer domestic service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareLongBreadBoard() => new()
	{
		Metadata = new("medieval_tableware_long_bread_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2609", []),
		Noun = "board",
		ShortDescription = "a long bread board",
		LongDescription = null,
		FullDescription = "This long bread board is a medium-sized, workmanlike board built from oak boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long board for bread loaves, trenchers, or slicing at table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareMapleEatingBoard() => new()
	{
		Metadata = new("medieval_tableware_maple_eating_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2640", []),
		Noun = "board",
		ShortDescription = "a maple eating board",
		LongDescription = null,
		FullDescription = "This maple eating board is a small, workmanlike board built from maple boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "maple",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Flat personal board for bread, cheese, cut meat, or fruit.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareMarketSampleTray() => new()
	{
		Metadata = new("medieval_tableware_market_sample_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2671", []),
		Noun = "tray",
		ShortDescription = "a small sample tray",
		LongDescription = null,
		FullDescription = "This small sample tray is a small, workmanlike tray built from willow boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light tray for samples, street food, or small table portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareMeatPlatter() => new()
	{
		Metadata = new("medieval_tableware_meat_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2702", []),
		Noun = "platter",
		ShortDescription = "a heavy meat platter",
		LongDescription = null,
		FullDescription = "This heavy meat platter is a medium-sized, workmanlike platter built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy platter for roasts, cut meats, or shared hall portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareMedicineFlask() => new()
	{
		Metadata = new("medieval_tableware_medicine_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2733", []),
		Noun = "flask",
		ShortDescription = "a medicine flask",
		LongDescription = null,
		FullDescription = "This medicine flask is a very small, well-made flask made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 160.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small flask for medicines, cordials, or infusions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareMilkJug() => new()
	{
		Metadata = new("medieval_tableware_milk_jug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2764", []),
		Noun = "jug",
		ShortDescription = "a small milk jug",
		LongDescription = null,
		FullDescription = "This small milk jug is a small, workmanlike jug formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small jug for milk, cream, or watered dairy service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareMountedDrinkingHorn() => new()
	{
		Metadata = new("medieval_tableware_mounted_drinking_horn", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2795", []),
		Noun = "horn",
		ShortDescription = "a brass-mounted drinking horn",
		LongDescription = null,
		FullDescription = "This brass-mounted drinking horn is a small, well-made horn worked from horn. The drinking mouth is polished smooth, and the tapering body curves down to a capped point. A small resting foot keeps it steady when set down. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 340.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Pint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Horn vessel with bright metal fittings described as part of its presentation.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareNarrowNeckOilJar() => new()
	{
		Metadata = new("medieval_tableware_narrow_neck_oil_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2826", []),
		Noun = "jar",
		ShortDescription = "a narrow-necked oil jar",
		LongDescription = null,
		FullDescription = "This narrow-necked oil jar is a medium-sized, workmanlike jar formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3200.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "terracotta",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Congius"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow-necked jar for oil or other precious household liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareNarrowWineTray() => new()
	{
		Metadata = new("medieval_tableware_narrow_wine_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2857", []),
		Noun = "tray",
		ShortDescription = "a narrow wine tray",
		LongDescription = null,
		FullDescription = "This narrow wine tray is a small, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 760.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
		BuilderNotes = "Narrow tray for cups, small goblets, or a decanter.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareOakCup() => new()
	{
		Metadata = new("medieval_tableware_oak_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2888", []),
		Noun = "cup",
		ShortDescription = "an oak drinking cup",
		LongDescription = null,
		FullDescription = "This oak drinking cup is a very small, workmanlike cup built from oak boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 210.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Durable wooden cup for ale, water, or table drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareOakTankard() => new()
	{
		Metadata = new("medieval_tableware_oak_tankard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2919", []),
		Noun = "tankard",
		ShortDescription = "an oak tankard",
		LongDescription = null,
		FullDescription = "This oak tankard is a small, workmanlike tankard built from oak boards. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 580.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_UKPint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wooden tankard for ale or cider service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareOakTrencherBoard() => new()
	{
		Metadata = new("medieval_tableware_oak_trencher_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2950", []),
		Noun = "trencher",
		ShortDescription = "an oak trencher board",
		LongDescription = null,
		FullDescription = "This oak trencher board is a small, workmanlike trencher built from oak boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Reusable wooden trencher for bread, meat, cheese, and cut portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareOakenFeastBoard() => new()
	{
		Metadata = new("medieval_tableware_oaken_feast_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:2981", []),
		Noun = "board",
		ShortDescription = "an oaken feast board",
		LongDescription = null,
		FullDescription = "This oaken feast board is a large, well-made board built from oak boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 3400.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large board for laying out shared feast portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareOilAmphora() => new()
	{
		Metadata = new("medieval_tableware_oil_amphora", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3012", []),
		Noun = "amphora",
		ShortDescription = "an earthenware oil amphora",
		LongDescription = null,
		FullDescription = "This earthenware oil amphora is a large, workmanlike amphora formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8900.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large ceramic vessel for oil storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareOilCruet() => new()
	{
		Metadata = new("medieval_tableware_oil_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3043", []),
		Noun = "cruet",
		ShortDescription = "a small oil cruet",
		LongDescription = null,
		FullDescription = "This small oil cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 190.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine cruet for pouring oil at table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePaintedCeramicPlatter() => new()
	{
		Metadata = new("medieval_tableware_painted_ceramic_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3074", []),
		Noun = "platter",
		ShortDescription = "a painted ceramic platter",
		LongDescription = null,
		FullDescription = "This painted ceramic platter is a medium-sized, well-made platter formed from ceramic. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1650.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorated platter for feasts or wealthier tables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePairedTableCruet() => new()
	{
		Metadata = new("medieval_tableware_paired_table_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3105", []),
		Noun = "cruet",
		ShortDescription = "a paired table cruet",
		LongDescription = null,
		FullDescription = "This paired table cruet is a small, well-made cruet formed from ceramic. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small double-bodied vessel represented by one liquid-container profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePewterDrinkingCup() => new()
	{
		Metadata = new("medieval_tableware_pewter_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3136", []),
		Noun = "cup",
		ShortDescription = "a pewter drinking cup",
		LongDescription = null,
		FullDescription = "This pewter drinking cup is a very small, well-made cup worked from pewter. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft-metal cup for a wealthier table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePewterEwer() => new()
	{
		Metadata = new("medieval_tableware_pewter_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3167", []),
		Noun = "ewer",
		ShortDescription = "a pewter table ewer",
		LongDescription = null,
		FullDescription = "This pewter table ewer is a small, well-made ewer worked from pewter. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1450.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft-metal ewer for household or inn service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePewterFlagon() => new()
	{
		Metadata = new("medieval_tableware_pewter_flagon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3198", []),
		Noun = "flagon",
		ShortDescription = "a pewter table flagon",
		LongDescription = null,
		FullDescription = "This pewter table flagon is a small, well-made flagon worked from pewter. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lidded-looking table flagon for wine or ale; no separate lock/openable behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePewterGoblet() => new()
	{
		Metadata = new("medieval_tableware_pewter_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3229", []),
		Noun = "goblet",
		ShortDescription = "a pewter table goblet",
		LongDescription = null,
		FullDescription = "This pewter table goblet is a small, well-made goblet worked from pewter. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_WineGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft-metal goblet for wine, ale, or ceremonial toasts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePewterServingPlatter() => new()
	{
		Metadata = new("medieval_tableware_pewter_serving_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3260", []),
		Noun = "platter",
		ShortDescription = "a pewter serving platter",
		LongDescription = null,
		FullDescription = "This pewter serving platter is a medium-sized, well-made platter worked from pewter. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1650.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
		BuilderNotes = "Soft-metal platter for higher-status table service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePewterTankard() => new()
	{
		Metadata = new("medieval_tableware_pewter_tankard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3291", []),
		Noun = "tankard",
		ShortDescription = "a pewter tankard",
		LongDescription = null,
		FullDescription = "This pewter tankard is a small, well-made tankard worked from pewter. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 760.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_UKPint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal tankard for a higher-status tavern or household.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePlainDrinkingHorn() => new()
	{
		Metadata = new("medieval_tableware_plain_drinking_horn", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3322", []),
		Noun = "horn",
		ShortDescription = "a plain drinking horn",
		LongDescription = null,
		FullDescription = "This plain drinking horn is a small, workmanlike horn worked from horn. The drinking mouth is polished smooth, and the tapering body curves down to a capped point. A small resting foot keeps it steady when set down. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_HalfPint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple horn drinking vessel for common use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePlainWoodenBowl() => new()
	{
		Metadata = new("medieval_tableware_plain_wooden_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3353", []),
		Noun = "bowl",
		ShortDescription = "a plain wooden bowl",
		LongDescription = null,
		FullDescription = "This plain wooden bowl is a small, workmanlike bowl built from plain wood. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Everyday bowl for dry or thick foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePolishedBoneDish() => new()
	{
		Metadata = new("medieval_tableware_polished_bone_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3384", []),
		Noun = "dish",
		ShortDescription = "a polished bone dish",
		LongDescription = null,
		FullDescription = "This polished bone dish is a very small, well-made dish worked from bone. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 220.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small polished dish for fine condiments, sweetmeats, or gaming-table snacks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePorcelainServingDish() => new()
	{
		Metadata = new("medieval_tableware_porcelain_serving_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3415", []),
		Noun = "dish",
		ShortDescription = "a porcelain serving dish",
		LongDescription = null,
		FullDescription = "This porcelain serving dish is a medium-sized, well-made dish formed from porcelain. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "porcelain",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine serving dish for elite or imported table service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePorcelainTablePlate() => new()
	{
		Metadata = new("medieval_tableware_porcelain_table_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3446", []),
		Noun = "plate",
		ShortDescription = "a porcelain table plate",
		LongDescription = null,
		FullDescription = "This porcelain table plate is a small, well-made plate formed from porcelain. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 540.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "porcelain",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine pale plate for elite, imported, or court-facing table service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePorridgeBowl() => new()
	{
		Metadata = new("medieval_tableware_porridge_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3477", []),
		Noun = "bowl",
		ShortDescription = "a wooden porridge bowl",
		LongDescription = null,
		FullDescription = "This wooden porridge bowl is a small, workmanlike bowl built from maple boards. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "maple",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wooden bowl for porridge and other thick foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewarePottageBowl() => new()
	{
		Metadata = new("medieval_tableware_pottage_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3508", []),
		Noun = "bowl",
		ShortDescription = "a broad pottage bowl",
		LongDescription = null,
		FullDescription = "This broad pottage bowl is a small, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad bowl for pottage, porridge, and spoon foods; modelled as an open food container.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
