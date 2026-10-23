#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalTablewareRaisedEdgePlatter() => new()
	{
		Metadata = new("medieval_tableware_raised_edge_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3539", []),
		Noun = "platter",
		ShortDescription = "a raised-edge platter",
		LongDescription = null,
		FullDescription = "This raised-edge platter is a medium-sized, workmanlike platter formed from earthenware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
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
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Raised-edge platter for loose cooked foods and shared portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareRaisedFootDish() => new()
	{
		Metadata = new("medieval_tableware_raised_foot_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3570", []),
		Noun = "dish",
		ShortDescription = "a raised-foot table dish",
		LongDescription = null,
		FullDescription = "This raised-foot table dish is a small, well-made dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 760.0,
		Cost = 26.0m,
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
		BuilderNotes = "Raised dish for presenting fruit, sweetmeats, or fine portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareRefectoryFoodTray() => new()
	{
		Metadata = new("medieval_tableware_refectory_food_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3601", []),
		Noun = "tray",
		ShortDescription = "a refectory food tray",
		LongDescription = null,
		FullDescription = "This refectory food tray is a medium-sized, workmanlike tray built from beech boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1000.0,
		Cost = 10.0m,
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
		BuilderNotes = "Plain service tray suited to communal dining halls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareRelishTray() => new()
	{
		Metadata = new("medieval_tableware_relish_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3632", []),
		Noun = "tray",
		ShortDescription = "a narrow relish tray",
		LongDescription = null,
		FullDescription = "This narrow relish tray is a small, well-made tray formed from ceramic. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
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
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow tray for pickles, relishes, or small sauces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareRimmedEarthenwarePlate() => new()
	{
		Metadata = new("medieval_tableware_rimmed_earthenware_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3663", []),
		Noun = "plate",
		ShortDescription = "a rimmed earthenware plate",
		LongDescription = null,
		FullDescription = "This rimmed earthenware plate is a small, workmanlike plate formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
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
		BuilderNotes = "Low-rimmed individual plate for sauced or crumbly foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareRimmedWoodenPlate() => new()
	{
		Metadata = new("medieval_tableware_rimmed_wooden_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3694", []),
		Noun = "plate",
		ShortDescription = "a rimmed wooden plate",
		LongDescription = null,
		FullDescription = "This rimmed wooden plate is a small, workmanlike plate built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 8.0m,
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
		BuilderNotes = "Low-rimmed plate that keeps loose dry foods gathered.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareRimmedWoodenTray() => new()
	{
		Metadata = new("medieval_tableware_rimmed_wooden_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3725", []),
		Noun = "tray",
		ShortDescription = "a rimmed wooden tray",
		LongDescription = null,
		FullDescription = "This rimmed wooden tray is a medium-sized, workmanlike tray built from pine boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 9.0m,
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
		BuilderNotes = "Simple rimmed tray for carrying cups or small dishes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareRinsingBowl() => new()
	{
		Metadata = new("medieval_tableware_rinsing_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3756", []),
		Noun = "bowl",
		ShortDescription = "a rinsing bowl",
		LongDescription = null,
		FullDescription = "This rinsing bowl is a small, workmanlike bowl formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 16.0m,
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
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Liquid-holding bowl for rinsing fingers, cups, or small utensils.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareRoundWoodenPlate() => new()
	{
		Metadata = new("medieval_tableware_round_wooden_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3786", []),
		Noun = "plate",
		ShortDescription = "a round wooden plate",
		LongDescription = null,
		FullDescription = "This round wooden plate is a small, workmanlike plate built from oak boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 460.0,
		Cost = 6.0m,
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
		BuilderNotes = "Simple round plate for common household meals.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareRushBottomedServiceTray() => new()
	{
		Metadata = new("medieval_tableware_rush_bottomed_service_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3817", []),
		Noun = "tray",
		ShortDescription = "a rush-bottomed service tray",
		LongDescription = null,
		FullDescription = "This rush-bottomed service tray is a medium-sized, workmanlike tray built from willow boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 6.0m,
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
		BuilderNotes = "Light woven service tray for bread, fruit, or dry goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSaltDish() => new()
	{
		Metadata = new("medieval_tableware_salt_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3848", []),
		Noun = "dish",
		ShortDescription = "a tiny salt dish",
		LongDescription = null,
		FullDescription = "This tiny salt dish is a tiny, workmanlike dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
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
		BuilderNotes = "Tiny open dish for salt or other dry table seasoning.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSauceDish() => new()
	{
		Metadata = new("medieval_tableware_sauce_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3879", []),
		Noun = "dish",
		ShortDescription = "a lipped sauce dish",
		LongDescription = null,
		FullDescription = "This lipped sauce dish is a very small, workmanlike dish formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
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
		BuilderNotes = "Small lipped dish for thick sauces, relishes, or dipping condiments.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSealedWaterUrn() => new()
	{
		Metadata = new("medieval_tableware_sealed_water_urn", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3910", []),
		Noun = "urn",
		ShortDescription = "a sealed water urn",
		LongDescription = null,
		FullDescription = "This sealed water urn is a large, well-made urn formed from stoneware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 13500.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Sturdier urn for storing clean water or wine.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareServingPlatterEarthenware() => new()
	{
		Metadata = new("medieval_tableware_serving_platter_earthenware", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3941", []),
		Noun = "platter",
		ShortDescription = "an earthenware serving platter",
		LongDescription = null,
		FullDescription = "This earthenware serving platter is a medium-sized, workmanlike platter formed from earthenware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1700.0,
		Cost = 11.0m,
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
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Common ceramic platter for shared foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareServingPlatterStoneware() => new()
	{
		Metadata = new("medieval_tableware_serving_platter_stoneware", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:3972", []),
		Noun = "platter",
		ShortDescription = "a stoneware serving platter",
		LongDescription = null,
		FullDescription = "This stoneware serving platter is a medium-sized, workmanlike platter formed from stoneware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1900.0,
		Cost = 18.0m,
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
		BuilderNotes = "Sturdier serving platter for repeated household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareShallowBeechTray() => new()
	{
		Metadata = new("medieval_tableware_shallow_beech_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4003", []),
		Noun = "tray",
		ShortDescription = "a shallow beech tray",
		LongDescription = null,
		FullDescription = "This shallow beech tray is a medium-sized, workmanlike tray built from beech boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
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
		BuilderNotes = "Light tray for household service and table clearing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareShavingBasin() => new()
	{
		Metadata = new("medieval_tableware_shaving_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4034", []),
		Noun = "basin",
		ShortDescription = "a small shaving basin",
		LongDescription = null,
		FullDescription = "This small shaving basin is a small, well-made basin worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1600.0,
		Cost = 36.0m,
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
		BuilderNotes = "Small metal basin for grooming and washing; represented by a jug-sized liquid profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareShotSizedMedicinalCup() => new()
	{
		Metadata = new("medieval_tableware_shot_sized_medicinal_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4064", []),
		Noun = "cup",
		ShortDescription = "a tiny medicinal cup",
		LongDescription = null,
		FullDescription = "This tiny medicinal cup is a tiny, well-made cup made from glass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 18.0m,
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
				"LContainer_ShotGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tiny glass for measured medicinal draughts or strong cordials.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSilveredGoblet() => new()
	{
		Metadata = new("medieval_tableware_silvered_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4095", []),
		Noun = "goblet",
		ShortDescription = "a silver table goblet",
		LongDescription = null,
		FullDescription = "This silver table goblet is a small, finely made goblet worked from silver. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 480.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
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
		BuilderNotes = "Luxury silver goblet for elite tables, ceremonial gifts, or treasure rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallBreadPlate() => new()
	{
		Metadata = new("medieval_tableware_small_bread_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4126", []),
		Noun = "plate",
		ShortDescription = "a small bread plate",
		LongDescription = null,
		FullDescription = "This small bread plate is a very small, workmanlike plate built from plain wood. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 4.0m,
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
		BuilderNotes = "Compact plate for bread portions or small side servings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallBreakingBoard() => new()
	{
		Metadata = new("medieval_tableware_small_breaking_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4157", []),
		Noun = "board",
		ShortDescription = "a small breaking board",
		LongDescription = null,
		FullDescription = "This small breaking board is a small, workmanlike board built from beech boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450.0,
		Cost = 6.0m,
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
		BuilderNotes = "Small board for bread-breaking, cheese-cutting, and quick service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallCarvedWoodenPlate() => new()
	{
		Metadata = new("medieval_tableware_small_carved_wooden_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4188", []),
		Noun = "plate",
		ShortDescription = "a carved wooden plate",
		LongDescription = null,
		FullDescription = "This carved wooden plate is a small, well-made plate built from walnut boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
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
		BuilderNotes = "Finer wooden plate with shallow carved ornament around the rim.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallCarvingPlatter() => new()
	{
		Metadata = new("medieval_tableware_small_carving_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4219", []),
		Noun = "platter",
		ShortDescription = "a small carving platter",
		LongDescription = null,
		FullDescription = "This small carving platter is a medium-sized, workmanlike platter built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1350.0,
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
		BuilderNotes = "Platter used for carving smaller roasts or poultry at table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallCordialCup() => new()
	{
		Metadata = new("medieval_tableware_small_cordial_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4250", []),
		Noun = "cup",
		ShortDescription = "a small cordial cup",
		LongDescription = null,
		FullDescription = "This small cordial cup is a tiny, well-made cup made from glass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 28.0m,
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
				"LContainer_Liqueur"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Very small cup for cordial, strong drink, or medicinal draughts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallGlassGoblet() => new()
	{
		Metadata = new("medieval_tableware_small_glass_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4281", []),
		Noun = "goblet",
		ShortDescription = "a small glass goblet",
		LongDescription = null,
		FullDescription = "This small glass goblet is a very small, well-made goblet made from glass. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 200.0,
		Cost = 38.0m,
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
				"LContainer_SmallWineGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Smaller goblet for wine or liqueur-like portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallLaverBasin() => new()
	{
		Metadata = new("medieval_tableware_small_laver_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4312", []),
		Noun = "basin",
		ShortDescription = "a small laver basin",
		LongDescription = null,
		FullDescription = "This small laver basin is a medium-sized, well-made basin worked from bronze. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3400.0,
		Cost = 60.0m,
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
		BuilderNotes = "Fine basin for washing hands at table or in a chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallMazer() => new()
	{
		Metadata = new("medieval_tableware_small_mazer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4342", []),
		Noun = "mazer",
		ShortDescription = "a small wooden mazer",
		LongDescription = null,
		FullDescription = "This small wooden mazer is a small, well-made mazer built from walnut boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Fine wooden drinking bowl for individual service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallNutDish() => new()
	{
		Metadata = new("medieval_tableware_small_nut_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4373", []),
		Noun = "dish",
		ShortDescription = "a small nut dish",
		LongDescription = null,
		FullDescription = "This small nut dish is a very small, workmanlike dish built from walnut boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
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
		BuilderNotes = "Compact dish for nuts, dried fruit, or table nibbles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallOilFlask() => new()
	{
		Metadata = new("medieval_tableware_small_oil_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4404", []),
		Noun = "flask",
		ShortDescription = "a small earthenware oil flask",
		LongDescription = null,
		FullDescription = "This small earthenware oil flask is a very small, workmanlike flask formed from earthenware. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
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
		BuilderNotes = "Small flask for oil, scent, medicine, or concentrated drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallServingBowl() => new()
	{
		Metadata = new("medieval_tableware_small_serving_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4435", []),
		Noun = "bowl",
		ShortDescription = "a small serving bowl",
		LongDescription = null,
		FullDescription = "This small serving bowl is a small, workmanlike bowl formed from stoneware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 10.0m,
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
		BuilderNotes = "Small shared bowl for table portions or side dishes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallSideDish() => new()
	{
		Metadata = new("medieval_tableware_small_side_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4466", []),
		Noun = "dish",
		ShortDescription = "a small side dish",
		LongDescription = null,
		FullDescription = "This small side dish is a very small, workmanlike dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
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
		BuilderNotes = "Small open dish for pickles, herbs, sauces, or side portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallTableAmphora() => new()
	{
		Metadata = new("medieval_tableware_small_table_amphora", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4497", []),
		Noun = "amphora",
		ShortDescription = "a small table amphora",
		LongDescription = null,
		FullDescription = "This small table amphora is a medium-sized, workmanlike amphora formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 16.0m,
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
		BuilderNotes = "Smaller amphora-like vessel for table or pantry use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallTasterCup() => new()
	{
		Metadata = new("medieval_tableware_small_taster_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4528", []),
		Noun = "cup",
		ShortDescription = "a tiny taster cup",
		LongDescription = null,
		FullDescription = "This tiny taster cup is a tiny, workmanlike cup worked from pewter. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Pony"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tiny vessel for tasting wine, ale, or medicine.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallWineCask() => new()
	{
		Metadata = new("medieval_tableware_small_wine_cask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4559", []),
		Noun = "cask",
		ShortDescription = "a small wine cask",
		LongDescription = null,
		FullDescription = "This small wine cask is a medium-sized, workmanlike cask built from oak boards. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_GallonCask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small household cask for wine, ale, vinegar, or water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallWineCup() => new()
	{
		Metadata = new("medieval_tableware_small_wine_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4590", []),
		Noun = "cup",
		ShortDescription = "a small wine cup",
		LongDescription = null,
		FullDescription = "This small wine cup is a very small, well-made cup formed from ceramic. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 220.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_SmallWineGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small cup suited to wine or watered wine; not necessarily made of glass.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSmallWineFlagon() => new()
	{
		Metadata = new("medieval_tableware_small_wine_flagon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4621", []),
		Noun = "flagon",
		ShortDescription = "a small wine flagon",
		LongDescription = null,
		FullDescription = "This small wine flagon is a small, workmanlike flagon formed from stoneware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1000.0,
		Cost = 13.0m,
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
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact flagon for wine or cordial service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSpiceDish() => new()
	{
		Metadata = new("medieval_tableware_spice_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4652", []),
		Noun = "dish",
		ShortDescription = "a small spice dish",
		LongDescription = null,
		FullDescription = "This small spice dish is a tiny, workmanlike dish built from plain wood. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 3.0m,
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
		BuilderNotes = "Small open dish for dry spice pinches or powdered condiments.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSpiceTray() => new()
	{
		Metadata = new("medieval_tableware_spice_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4683", []),
		Noun = "tray",
		ShortDescription = "a small spice tray",
		LongDescription = null,
		FullDescription = "This small spice tray is a small, well-made tray built from walnut boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
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
		BuilderNotes = "Small tray for condiment dishes, salt, or costly spices.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSpoutedPitcher() => new()
	{
		Metadata = new("medieval_tableware_spouted_pitcher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4714", []),
		Noun = "pitcher",
		ShortDescription = "a spouted ceramic pitcher",
		LongDescription = null,
		FullDescription = "This spouted ceramic pitcher is a small, well-made pitcher formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 24.0m,
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
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine spouted pitcher for cleaner pouring at table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareStaveBuiltTankard() => new()
	{
		Metadata = new("medieval_tableware_stave_built_tankard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4745", []),
		Noun = "tankard",
		ShortDescription = "a stave-built tankard",
		LongDescription = null,
		FullDescription = "This stave-built tankard is a small, workmanlike tankard built from oak boards. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 640.0,
		Cost = 14.0m,
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
		BuilderNotes = "Stave-built tankard with hoop-like construction described visually only.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareStonewareBeaker() => new()
	{
		Metadata = new("medieval_tableware_stoneware_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4776", []),
		Noun = "beaker",
		ShortDescription = "a stoneware beaker",
		LongDescription = null,
		FullDescription = "This stoneware beaker is a very small, workmanlike beaker formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 390.0,
		Cost = 8.0m,
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
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Durable beaker for household or tavern use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareStonewareBottle() => new()
	{
		Metadata = new("medieval_tableware_stoneware_bottle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4807", []),
		Noun = "bottle",
		ShortDescription = "a stoneware bottle",
		LongDescription = null,
		FullDescription = "This stoneware bottle is a small, workmanlike bottle formed from stoneware. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 10.0m,
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
				"LContainer_QuartBottle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy bottle for ale, vinegar, or table liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareStonewareDinnerPlate() => new()
	{
		Metadata = new("medieval_tableware_stoneware_dinner_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4838", []),
		Noun = "plate",
		ShortDescription = "a stoneware dinner plate",
		LongDescription = null,
		FullDescription = "This stoneware dinner plate is a small, workmanlike plate formed from stoneware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 9.0m,
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
		BuilderNotes = "Sturdier stoneware plate for repeated household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareStonewareDrinkingCup() => new()
	{
		Metadata = new("medieval_tableware_stoneware_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4869", []),
		Noun = "cup",
		ShortDescription = "a stoneware drinking cup",
		LongDescription = null,
		FullDescription = "This stoneware drinking cup is a very small, workmanlike cup formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 8.0m,
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
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy cup for everyday household drinking.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareStonewareMug() => new()
	{
		Metadata = new("medieval_tableware_stoneware_mug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4900", []),
		Noun = "mug",
		ShortDescription = "a stoneware mug",
		LongDescription = null,
		FullDescription = "This stoneware mug is a small, workmanlike mug formed from stoneware. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560.0,
		Cost = 10.0m,
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
				"LContainer_HalfPint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy handled mug for household or inn service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareStonewarePorringer() => new()
	{
		Metadata = new("medieval_tableware_stoneware_porringer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4931", []),
		Noun = "porringer",
		ShortDescription = "a stoneware porringer",
		LongDescription = null,
		FullDescription = "This stoneware porringer is a small, workmanlike porringer formed from stoneware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 10.0m,
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
		BuilderNotes = "Sturdy porringer for common household table use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareStonewareTableJug() => new()
	{
		Metadata = new("medieval_tableware_stoneware_table_jug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4962", []),
		Noun = "jug",
		ShortDescription = "a stoneware table jug",
		LongDescription = null,
		FullDescription = "This stoneware table jug is a small, workmanlike jug formed from stoneware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 14.0m,
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
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy jug for repeated table and kitchen service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareStonewareWashingBasin() => new()
	{
		Metadata = new("medieval_tableware_stoneware_washing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:4993", []),
		Noun = "basin",
		ShortDescription = "a stoneware washing basin",
		LongDescription = null,
		FullDescription = "This stoneware washing basin is a medium-sized, workmanlike basin formed from stoneware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5600.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
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
		BuilderNotes = "Sturdy wash basin for kitchen or chamber work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSweetmeatTray() => new()
	{
		Metadata = new("medieval_tableware_sweetmeat_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5023", []),
		Noun = "tray",
		ShortDescription = "a sweetmeat tray",
		LongDescription = null,
		FullDescription = "This sweetmeat tray is a small, well-made tray worked from ivory. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 340.0,
		Cost = 95.0m,
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
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small luxury tray for sweetmeats, spiced nuts, or delicacies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareTableBeerCask() => new()
	{
		Metadata = new("medieval_tableware_table_beer_cask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5054", []),
		Noun = "cask",
		ShortDescription = "a table beer cask",
		LongDescription = null,
		FullDescription = "This table beer cask is a medium-sized, workmanlike cask built from oak boards. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4500.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_GallonCask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small service cask for beer or ale at a hall or tavern table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareTableEwer() => new()
	{
		Metadata = new("medieval_tableware_table_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5085", []),
		Noun = "ewer",
		ShortDescription = "an earthenware table ewer",
		LongDescription = null,
		FullDescription = "This earthenware table ewer is a small, workmanlike ewer formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
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
		BuilderNotes = "Pouring vessel for drink or washing water using a decanter-sized profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareTableOliveDish() => new()
	{
		Metadata = new("medieval_tableware_table_olive_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5116", []),
		Noun = "dish",
		ShortDescription = "a narrow olive dish",
		LongDescription = null,
		FullDescription = "This narrow olive dish is a very small, workmanlike dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
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
		BuilderNotes = "Narrow dish for olives, pickles, or preserved fruit.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareTallAleBeaker() => new()
	{
		Metadata = new("medieval_tableware_tall_ale_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5147", []),
		Noun = "beaker",
		ShortDescription = "a tall ale beaker",
		LongDescription = null,
		FullDescription = "This tall ale beaker is a small, workmanlike beaker formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 10.0m,
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
				"LContainer_Pint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Taller beaker for ale or other common drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareTavernCarryTray() => new()
	{
		Metadata = new("medieval_tableware_tavern_carry_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5178", []),
		Noun = "tray",
		ShortDescription = "a tavern carry tray",
		LongDescription = null,
		FullDescription = "This tavern carry tray is a medium-sized, workmanlike tray built from pine boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780.0,
		Cost = 7.0m,
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
		BuilderNotes = "Cheap tray for cups, bowls, and rough tavern service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareTerracottaServingPlatter() => new()
	{
		Metadata = new("medieval_tableware_terracotta_serving_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5209", []),
		Noun = "platter",
		ShortDescription = "a terracotta serving platter",
		LongDescription = null,
		FullDescription = "This terracotta serving platter is a medium-sized, workmanlike platter formed from terracotta. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "terracotta",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
		BuilderNotes = "Warm-fired platter for common table service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareTerracottaTablePlate() => new()
	{
		Metadata = new("medieval_tableware_terracotta_table_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5240", []),
		Noun = "plate",
		ShortDescription = "a terracotta table plate",
		LongDescription = null,
		FullDescription = "This terracotta table plate is a small, workmanlike plate formed from terracotta. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 670.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "terracotta",
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
		BuilderNotes = "Warm-coloured fired-clay plate for common meals.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareTwoHandledServingTray() => new()
	{
		Metadata = new("medieval_tableware_two_handled_serving_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5271", []),
		Noun = "tray",
		ShortDescription = "a two-handled serving tray",
		LongDescription = null,
		FullDescription = "This two-handled serving tray is a medium-sized, well-made tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1450.0,
		Cost = 22.0m,
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
		BuilderNotes = "Sturdier tray with side grips for hall service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareVinegarAmphora() => new()
	{
		Metadata = new("medieval_tableware_vinegar_amphora", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5302", []),
		Noun = "amphora",
		ShortDescription = "a vinegar amphora",
		LongDescription = null,
		FullDescription = "This vinegar amphora is a large, workmanlike amphora formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8800.0,
		Cost = 28.0m,
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
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large vessel for vinegar or sour wine.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareVinegarCruet() => new()
	{
		Metadata = new("medieval_tableware_vinegar_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5333", []),
		Noun = "cruet",
		ShortDescription = "a small vinegar cruet",
		LongDescription = null,
		FullDescription = "This small vinegar cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
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
		BuilderNotes = "Fine cruet for vinegar or sharp sauces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareVinegarFlask() => new()
	{
		Metadata = new("medieval_tableware_vinegar_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5364", []),
		Noun = "flask",
		ShortDescription = "a small vinegar flask",
		LongDescription = null,
		FullDescription = "This small vinegar flask is a very small, workmanlike flask formed from stoneware. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
		Cost = 8.0m,
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
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small flask for vinegar or sharp table liquids.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWarmCrockPlatter() => new()
	{
		Metadata = new("medieval_tableware_warm_crock_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5395", []),
		Noun = "platter",
		ShortDescription = "a thick crock platter",
		LongDescription = null,
		FullDescription = "This thick crock platter is a medium-sized, workmanlike platter formed from stoneware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2200.0,
		Cost = 18.0m,
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
		BuilderNotes = "Thick stoneware platter for hearty food service; does not provide heat mechanics.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWashEwer() => new()
	{
		Metadata = new("medieval_tableware_wash_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5426", []),
		Noun = "ewer",
		ShortDescription = "a handwashing ewer",
		LongDescription = null,
		FullDescription = "This handwashing ewer is a small, well-made ewer formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
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
		BuilderNotes = "Ewer for washing hands before or after meals.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWaterPitcher() => new()
	{
		Metadata = new("medieval_tableware_water_pitcher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5457", []),
		Noun = "pitcher",
		ShortDescription = "a water pitcher",
		LongDescription = null,
		FullDescription = "This water pitcher is a small, workmanlike pitcher formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
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
		BuilderNotes = "Handled pitcher for water service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWaterStorageJar() => new()
	{
		Metadata = new("medieval_tableware_water_storage_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5488", []),
		Noun = "jar",
		ShortDescription = "a water storage jar",
		LongDescription = null,
		FullDescription = "This water storage jar is a large, workmanlike jar formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9500.0,
		Cost = 26.0m,
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
		BuilderNotes = "Large ceramic vessel for household water storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWhiskeySizedSpiritCup() => new()
	{
		Metadata = new("medieval_tableware_whiskey_sized_spirit_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5519", []),
		Noun = "cup",
		ShortDescription = "a small spirit cup",
		LongDescription = null,
		FullDescription = "This small spirit cup is a tiny, workmanlike cup formed from ceramic. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_WhiskeyGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small cup for concentrated drink, medicine, or spirits in fantasy settings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWhitewareWineCup() => new()
	{
		Metadata = new("medieval_tableware_whiteware_wine_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5550", []),
		Noun = "cup",
		ShortDescription = "a whiteware wine cup",
		LongDescription = null,
		FullDescription = "This whiteware wine cup is a very small, well-made cup formed from ceramic. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 240.0,
		Cost = 18.0m,
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
				"LContainer_SmallWineGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pale ceramic wine cup for finer tables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWideMouthedWaterJar() => new()
	{
		Metadata = new("medieval_tableware_wide_mouthed_water_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5581", []),
		Noun = "jar",
		ShortDescription = "a wide-mouthed water jar",
		LongDescription = null,
		FullDescription = "This wide-mouthed water jar is a medium-sized, workmanlike jar formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3400.0,
		Cost = 17.0m,
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
				"LContainer_Amphora_Congius"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wide-mouthed jar for water kept near the hearth or door.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWideTableDish() => new()
	{
		Metadata = new("medieval_tableware_wide_table_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5612", []),
		Noun = "dish",
		ShortDescription = "a wide table dish",
		LongDescription = null,
		FullDescription = "This wide table dish is a small, workmanlike dish formed from stoneware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 10.0m,
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
		BuilderNotes = "Broad open dish for shared table portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWillowPeelingDish() => new()
	{
		Metadata = new("medieval_tableware_willow_peeling_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5643", []),
		Noun = "dish",
		ShortDescription = "a willow peeling dish",
		LongDescription = null,
		FullDescription = "This willow peeling dish is a small, workmanlike dish built from willow boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
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
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light open dish for vegetable peelings, scraps, or simple kitchen service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWineAmphora() => new()
	{
		Metadata = new("medieval_tableware_wine_amphora", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5674", []),
		Noun = "amphora",
		ShortDescription = "an earthenware wine amphora",
		LongDescription = null,
		FullDescription = "This earthenware wine amphora is a large, workmanlike amphora formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
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
		BuilderNotes = "Large amphora-like vessel for wine or other stored liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWoodHoopedWaterBucket() => new()
	{
		Metadata = new("medieval_tableware_wood_hooped_water_bucket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5705", []),
		Noun = "bucket",
		ShortDescription = "a wood-hooped water bucket",
		LongDescription = null,
		FullDescription = "This wood-hooped water bucket is a medium-sized, workmanlike bucket built from oak boards. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Handled bucket-like vessel for carrying water inside a household.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWoodenAleBowl() => new()
	{
		Metadata = new("medieval_tableware_wooden_ale_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5735", []),
		Noun = "bowl",
		ShortDescription = "a wooden ale bowl",
		LongDescription = null,
		FullDescription = "This wooden ale bowl is a small, workmanlike bowl built from maple boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "maple",
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
		BuilderNotes = "Small rounded drinking bowl for ale or water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWoodenCanteen() => new()
	{
		Metadata = new("medieval_tableware_wooden_canteen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5766", []),
		Noun = "canteen",
		ShortDescription = "a wooden water canteen",
		LongDescription = null,
		FullDescription = "This wooden water canteen is a small, workmanlike canteen built from plain wood. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 600.0,
		Cost = 15.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
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
		BuilderNotes = "Flat portable water vessel for travel or work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWoodenDrinkingCup() => new()
	{
		Metadata = new("medieval_tableware_wooden_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5797", []),
		Noun = "cup",
		ShortDescription = "a wooden drinking cup",
		LongDescription = null,
		FullDescription = "This wooden drinking cup is a very small, workmanlike cup built from beech boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
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
		BuilderNotes = "Light carved cup for common drink service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWoodenMug() => new()
	{
		Metadata = new("medieval_tableware_wooden_mug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5828", []),
		Noun = "mug",
		ShortDescription = "a wooden table mug",
		LongDescription = null,
		FullDescription = "This wooden table mug is a small, workmanlike mug built from oak boards. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380.0,
		Cost = 9.0m,
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
				"LContainer_HalfPint"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Handled wooden mug for everyday ale or water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWoodenPorringer() => new()
	{
		Metadata = new("medieval_tableware_wooden_porringer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5859", []),
		Noun = "porringer",
		ShortDescription = "a wooden porringer",
		LongDescription = null,
		FullDescription = "This wooden porringer is a small, workmanlike porringer built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
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
		BuilderNotes = "Light wooden porringer for thick foods or children's servings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareWoodenTableJug() => new()
	{
		Metadata = new("medieval_tableware_wooden_table_jug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5890", []),
		Noun = "jug",
		ShortDescription = "a wooden table jug",
		LongDescription = null,
		FullDescription = "This wooden table jug is a small, workmanlike jug built from oak boards. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 800.0,
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
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wooden jug for common household drink service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedNorthernNorthSeaBirchBarkCup() => new()
	{
		Metadata = new("medieval_tableware_shared_northern_north_sea_birch_bark_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5921", []),
		Noun = "cup",
		ShortDescription = "a birch-bark drinking cup",
		LongDescription = null,
		FullDescription = "This birch-bark drinking cup is a very small, workmanlike cup built from birch boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
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
		BuilderNotes = "Light northern cup made from bark-like birch material for travel or simple household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedNorthernNorthSeaFlatOatcakeTrencher() => new()
	{
		Metadata = new("medieval_tableware_shared_northern_north_sea_flat_oatcake_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5952", []),
		Noun = "trencher",
		ShortDescription = "a flat oatcake trencher",
		LongDescription = null,
		FullDescription = "This flat oatcake trencher is a small, workmanlike trencher made from firm baked bread. The centre is pressed shallow, while the rim is left thick and firm enough to hold food. The crust is dry and browned across the upper edge. The crust is firm and dry, with a shallow centre pressed down to take food.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
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
		BuilderNotes = "Edible flat trencher suited to northern bread or oatcake service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedNorthernNorthSeaHornFeastSetTray() => new()
	{
		Metadata = new("medieval_tableware_shared_northern_north_sea_horn_feast_set_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:5983", []),
		Noun = "tray",
		ShortDescription = "a horn-cup feast tray",
		LongDescription = null,
		FullDescription = "This horn-cup feast tray is a medium-sized, well-made tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
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
		BuilderNotes = "Tray shaped to carry drinking horns or cups at a feast.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedNorthernNorthSeaShipCarvedDrinkingBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_northern_north_sea_ship_carved_drinking_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6014", []),
		Noun = "bowl",
		ShortDescription = "a ship-carved drinking bowl",
		LongDescription = null,
		FullDescription = "This ship-carved drinking bowl is a small, well-made bowl built from pine boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
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
		BuilderNotes = "Decorated wooden drinking bowl suited to coastal hall culture.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedNorthernNorthSeaSmokedFishPlatter() => new()
	{
		Metadata = new("medieval_tableware_shared_northern_north_sea_smoked_fish_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6045", []),
		Noun = "platter",
		ShortDescription = "a smoked-fish platter",
		LongDescription = null,
		FullDescription = "This smoked-fish platter is a medium-sized, workmanlike platter built from birch boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
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
		BuilderNotes = "Long light platter for smoked fish or preserved foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedNorthernNorthSeaStaveAleTankard() => new()
	{
		Metadata = new("medieval_tableware_shared_northern_north_sea_stave_ale_tankard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6076", []),
		Noun = "tankard",
		ShortDescription = "a hooped stave tankard",
		LongDescription = null,
		FullDescription = "This hooped stave tankard is a small, workmanlike tankard built from oak boards. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 16.0m,
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
		BuilderNotes = "North-sea style stave-built tankard for ale or mead.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedWesternEuropeanGildedCourtGoblet() => new()
	{
		Metadata = new("medieval_tableware_shared_western_european_gilded_court_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6107", []),
		Noun = "goblet",
		ShortDescription = "a gilded court goblet",
		LongDescription = null,
		FullDescription = "This gilded court goblet is a small, finely made goblet worked from gold. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 500.0,
		Cost = 450.0m,
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
				"LContainer_WineGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Precious court-facing goblet for elite halls and treasuries.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedWesternEuropeanManorWashSetTray() => new()
	{
		Metadata = new("medieval_tableware_shared_western_european_manor_wash_set_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6138", []),
		Noun = "tray",
		ShortDescription = "a manor wash-set tray",
		LongDescription = null,
		FullDescription = "This manor wash-set tray is a medium-sized, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800.0,
		Cost = 60.0m,
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
		BuilderNotes = "Metal tray meant to support an ewer-and-basin service set.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedWesternEuropeanMonasticRefectoryTrencher() => new()
	{
		Metadata = new("medieval_tableware_shared_western_european_monastic_refectory_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6169", []),
		Noun = "trencher",
		ShortDescription = "a refectory trencher",
		LongDescription = null,
		FullDescription = "This refectory trencher is a small, workmanlike trencher built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 5.0m,
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
		BuilderNotes = "Plain reusable trencher for communal dining.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedWesternEuropeanPewterDinnerPlate() => new()
	{
		Metadata = new("medieval_tableware_shared_western_european_pewter_dinner_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6200", []),
		Noun = "plate",
		ShortDescription = "a pewter dinner plate",
		LongDescription = null,
		FullDescription = "This pewter dinner plate is a small, well-made plate worked from pewter. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 38.0m,
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
		BuilderNotes = "Higher-status metal plate for urban or manorial service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedWesternEuropeanPewterSaltCellar() => new()
	{
		Metadata = new("medieval_tableware_shared_western_european_pewter_salt_cellar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6231", []),
		Noun = "cellar",
		ShortDescription = "a pewter salt cellar",
		LongDescription = null,
		FullDescription = "This pewter salt cellar is a very small, well-made cellar worked from pewter. A small hollow sits inside a thick rim, with a flat base beneath it. The lip is polished where fingers have lifted it from the table. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 42.0m,
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
		BuilderNotes = "Prominent table salt vessel; open-container profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedWesternEuropeanTinGlazedServingDish() => new()
	{
		Metadata = new("medieval_tableware_shared_western_european_tin_glazed_serving_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6262", []),
		Noun = "dish",
		ShortDescription = "a tin-glazed serving dish",
		LongDescription = null,
		FullDescription = "This tin-glazed serving dish is a medium-sized, well-made dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 38.0m,
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
		BuilderNotes = "Bright glazed serving dish for wealthy western tables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedMediterraneanBronzeHandwashingBasin() => new()
	{
		Metadata = new("medieval_tableware_shared_mediterranean_bronze_handwashing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6293", []),
		Noun = "basin",
		ShortDescription = "a bronze handwashing basin",
		LongDescription = null,
		FullDescription = "This bronze handwashing basin is a medium-sized, well-made basin worked from bronze. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 58.0m,
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
		BuilderNotes = "Basin suited to formal handwashing before meals.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedMediterraneanMarbleFruitPlatter() => new()
	{
		Metadata = new("medieval_tableware_shared_mediterranean_marble_fruit_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6323", []),
		Noun = "platter",
		ShortDescription = "a marble fruit platter",
		LongDescription = null,
		FullDescription = "This marble fruit platter is a medium-sized, well-made platter cut from marble. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 75.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "marble",
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
		BuilderNotes = "Heavy luxury platter for fruit or sweetmeats.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedMediterraneanOliveOilCruet() => new()
	{
		Metadata = new("medieval_tableware_shared_mediterranean_olive_oil_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6354", []),
		Noun = "cruet",
		ShortDescription = "an olive-oil cruet",
		LongDescription = null,
		FullDescription = "This olive-oil cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 210.0,
		Cost = 30.0m,
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
		BuilderNotes = "Small vessel for olive oil at table or in the kitchen.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedMediterraneanPaintedTableAmphora() => new()
	{
		Metadata = new("medieval_tableware_shared_mediterranean_painted_table_amphora", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6385", []),
		Noun = "amphora",
		ShortDescription = "a painted table amphora",
		LongDescription = null,
		FullDescription = "This painted table amphora is a medium-sized, well-made amphora formed from ceramic. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2800.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Decorated smaller amphora for table or pantry service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedMediterraneanShallowOilDish() => new()
	{
		Metadata = new("medieval_tableware_shared_mediterranean_shallow_oil_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6416", []),
		Noun = "dish",
		ShortDescription = "a shallow oil dish",
		LongDescription = null,
		FullDescription = "This shallow oil dish is a very small, workmanlike dish formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
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
		BuilderNotes = "Small open dish for oil-soaked bread, olives, or condiments.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedMediterraneanTerracottaWaterAmphora() => new()
	{
		Metadata = new("medieval_tableware_shared_mediterranean_terracotta_water_amphora", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6447", []),
		Noun = "amphora",
		ShortDescription = "a terracotta water amphora",
		LongDescription = null,
		FullDescription = "This terracotta water amphora is a large, workmanlike amphora formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9400.0,
		Cost = 30.0m,
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
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Mediterranean-style household water amphora.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedMediterraneanTwoHandledWineJar() => new()
	{
		Metadata = new("medieval_tableware_shared_mediterranean_two_handled_wine_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6478", []),
		Noun = "jar",
		ShortDescription = "a two-handled wine jar",
		LongDescription = null,
		FullDescription = "This two-handled wine jar is a medium-sized, workmanlike jar formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 20.0m,
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
		BuilderNotes = "Handled ceramic jar for wine or watered wine.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedMediterraneanWineMixingBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_mediterranean_wine_mixing_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6509", []),
		Noun = "bowl",
		ShortDescription = "a wine-mixing bowl",
		LongDescription = null,
		FullDescription = "This wine-mixing bowl is a medium-sized, well-made bowl formed from ceramic. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
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
				"LContainer_Amphora_Congius"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open-looking but liquid-holding bowl for mixing or serving wine.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanBrassWashBasin() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_brass_wash_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6540", []),
		Noun = "basin",
		ShortDescription = "an engraved brass basin",
		LongDescription = null,
		FullDescription = "This engraved brass basin is a medium-sized, well-made basin worked from brass. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3400.0,
		Cost = 58.0m,
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
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine basin for washing, guest service, or table ritual.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanDateServingTray() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_date_serving_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6570", []),
		Noun = "tray",
		ShortDescription = "a date-serving tray",
		LongDescription = null,
		FullDescription = "This date-serving tray is a small, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 660.0,
		Cost = 30.0m,
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
		BuilderNotes = "Metal tray for dates, sweetmeats, or small guest offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanEngravedBrassEwer() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_engraved_brass_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6601", []),
		Noun = "ewer",
		ShortDescription = "an engraved brass ewer",
		LongDescription = null,
		FullDescription = "This engraved brass ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1450.0,
		Cost = 55.0m,
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
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine engraved pouring vessel for water or scented drink.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
