#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalDomesticTravelGearWardrobe() => new()
	{
		Metadata = new("medieval_domestic_travel_gear_wardrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16597", []),
		Noun = "wardrobe",
		ShortDescription = "a travel-gear wardrobe",
		LongDescription = null,
		FullDescription = "This travel-gear wardrobe is a large, workmanlike wardrobe built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 88.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wardrobe for packs, cloaks, boots, spare straps, and outdoor household gear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticUnderBedBlanketBox() => new()
	{
		Metadata = new("medieval_domestic_under_bed_blanket_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16627", []),
		Noun = "box",
		ShortDescription = "an under-bed blanket box",
		LongDescription = null,
		FullDescription = "This under-bed blanket box is a large, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 13800.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low blanket box sized for under-bed or foot-of-bed storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWallCupboard() => new()
	{
		Metadata = new("medieval_domestic_wall_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16657", []),
		Noun = "cupboard",
		ShortDescription = "a wall-hung cupboard",
		LongDescription = null,
		FullDescription = "This wall-hung cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12800.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Raised cupboard for keeping household goods away from floor damp and animals.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWallPegsShelf() => new()
	{
		Metadata = new("medieval_domestic_wall_pegs_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16687", []),
		Noun = "shelf",
		ShortDescription = "a wall-peg shelf",
		LongDescription = null,
		FullDescription = "This wall-peg shelf is a medium-sized, workmanlike shelf built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2800.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wall_Shelf"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wall shelf for keys, lamps, pouches, and small household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWalnutChestOfDrawers() => new()
	{
		Metadata = new("medieval_domestic_walnut_chest_of_drawers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16718", []),
		Noun = "dresser",
		ShortDescription = "a walnut chest of drawers",
		LongDescription = null,
		FullDescription = "This walnut chest of drawers is a large, well-made dresser built from walnut boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 165.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Dresser"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine drawer chest for elite chamber goods, clothing, and valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWalnutRobeWardrobe() => new()
	{
		Metadata = new("medieval_domestic_walnut_robe_wardrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16748", []),
		Noun = "wardrobe",
		ShortDescription = "a walnut robe wardrobe",
		LongDescription = null,
		FullDescription = "This walnut robe wardrobe is a large, well-made wardrobe built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 24500.0,
		Cost = 155.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine wardrobe for robes, gowns, veils, belts, and elite household dress.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWalnutSmallCabinet() => new()
	{
		Metadata = new("medieval_domestic_walnut_small_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16778", []),
		Noun = "cabinet",
		ShortDescription = "a walnut small cabinet",
		LongDescription = null,
		FullDescription = "This walnut small cabinet is a medium-sized, well-made cabinet built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7600.0,
		Cost = 78.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine small cabinet for valuables, writing goods, keepsakes, or elite chamber storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWashstandDrawer() => new()
	{
		Metadata = new("medieval_domestic_washstand_drawer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16808", []),
		Noun = "stand",
		ShortDescription = "a washstand drawer",
		LongDescription = null,
		FullDescription = "This washstand drawer is a medium-sized, workmanlike stand built from beech boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Nightstand"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small drawer stand for towels, combs, soaps, and washing pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWeaponRack() => new()
	{
		Metadata = new("medieval_domestic_weapon_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16838", []),
		Noun = "rack",
		ShortDescription = "a household weapon rack",
		LongDescription = null,
		FullDescription = "This household weapon rack is a large, workmanlike rack built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7600.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Weapon_Rack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rack for weapons, staves, hunting gear, and similar long household objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWideBeechDishShelves() => new()
	{
		Metadata = new("medieval_domestic_wide_beech_dish_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16869", []),
		Noun = "shelves",
		ShortDescription = "a set of beech dish shelves",
		LongDescription = null,
		FullDescription = "These set of beech dish shelves are large, workmanlike shelves built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8200.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Wide shelves for plates, bowls, pitchers, and serving vessels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWideCellarShelves() => new()
	{
		Metadata = new("medieval_domestic_wide_cellar_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16900", []),
		Noun = "shelves",
		ShortDescription = "a set of cellar shelves",
		LongDescription = null,
		FullDescription = "These set of cellar shelves are large, workmanlike shelves built from elm boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8800.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "elm",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Open shelving for cellar goods, jars, spare vessels, and household stock.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWideLarderShelves() => new()
	{
		Metadata = new("medieval_domestic_wide_larder_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16931", []),
		Noun = "shelves",
		ShortDescription = "a set of larder shelves",
		LongDescription = null,
		FullDescription = "These set of larder shelves are large, workmanlike shelves built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9400.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Stout open shelves for pantry jars, food boxes, sacks, and household stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWideLinenShelves() => new()
	{
		Metadata = new("medieval_domestic_wide_linen_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16962", []),
		Noun = "shelves",
		ShortDescription = "a set of linen shelves",
		LongDescription = null,
		FullDescription = "These set of linen shelves are large, workmanlike shelves built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8600.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Wide shelves for folded linens, bedding, curtains, and domestic cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWideOakShelves() => new()
	{
		Metadata = new("medieval_domestic_wide_oak_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16993", []),
		Noun = "shelves",
		ShortDescription = "a set of wide oak shelves",
		LongDescription = null,
		FullDescription = "These set of wide oak shelves are large, workmanlike shelves built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Broad shelving for household stores, crockery, folded cloth, tools, and bins.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWidePantryShelf() => new()
	{
		Metadata = new("medieval_domestic_wide_pantry_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17024", []),
		Noun = "shelf",
		ShortDescription = "a wide pantry shelf",
		LongDescription = null,
		FullDescription = "This wide pantry shelf is a large, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Wide shelves for dry stores, baskets, bowls, and packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWidePineStoreShelves() => new()
	{
		Metadata = new("medieval_domestic_wide_pine_store_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17055", []),
		Noun = "shelves",
		ShortDescription = "a set of pine store shelves",
		LongDescription = null,
		FullDescription = "These set of pine store shelves are large, workmanlike shelves built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7600.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
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
		BuilderNotes = "Large simple shelving for domestic storerooms and household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWillowLaundryHamper() => new()
	{
		Metadata = new("medieval_domestic_willow_laundry_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17086", []),
		Noun = "hamper",
		ShortDescription = "a willow laundry hamper",
		LongDescription = null,
		FullDescription = "This willow laundry hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open hamper for laundry, bedding, linens, and folded household cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWinterBeddingChest() => new()
	{
		Metadata = new("medieval_domestic_winter_bedding_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17117", []),
		Noun = "chest",
		ShortDescription = "a winter bedding chest",
		LongDescription = null,
		FullDescription = "This winter bedding chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large chest for blankets, furs, coverlets, and seasonal bedding.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWinterMittenBin() => new()
	{
		Metadata = new("medieval_domestic_winter_mitten_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17147", []),
		Noun = "bin",
		ShortDescription = "a winter mitten bin",
		LongDescription = null,
		FullDescription = "This winter mitten bin is a medium-sized, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3200.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open bin for mittens, hats, scarves, and cold-weather accessories.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWoolMendingHamper() => new()
	{
		Metadata = new("medieval_domestic_wool_mending_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17178", []),
		Noun = "hamper",
		ShortDescription = "a wool mending hamper",
		LongDescription = null,
		FullDescription = "This wool mending hamper is a medium-sized, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open hamper for mending, cloth scraps, thread pouches, and unfinished household repairs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWritingDeskDrawers() => new()
	{
		Metadata = new("medieval_domestic_writing_desk_drawers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17209", []),
		Noun = "desk",
		ShortDescription = "a writing desk with drawers",
		LongDescription = null,
		FullDescription = "This writing desk with drawers is a large, well-made desk built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 135.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Drawers"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Writing desk with drawer storage for tablets, papers, pens, seal pouches, and account goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWritingTableDrawers() => new()
	{
		Metadata = new("medieval_domestic_writing_table_drawers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17239", []),
		Noun = "drawers",
		ShortDescription = "writing-table drawers",
		LongDescription = null,
		FullDescription = "These writing-table drawers are medium-sized, well-made drawers built from walnut boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Drawers"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine drawers for pens, wax, paper, account pieces, and seals.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticBedchamberCoinChest() => new()
	{
		Metadata = new("medieval_locking_domestic_bedchamber_coin_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17269", []),
		Noun = "chest",
		ShortDescription = "a bedchamber coin chest",
		LongDescription = null,
		FullDescription = "This bedchamber coin chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 20000.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking chest for coin, tokens, and valued domestic goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticBeddingLockChest() => new()
	{
		Metadata = new("medieval_locking_domestic_bedding_lock_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17299", []),
		Noun = "chest",
		ShortDescription = "a bedding lock chest",
		LongDescription = null,
		FullDescription = "This bedding lock chest is a large, workmanlike chest built from pine boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26000.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large locking chest for bedding, blankets, or seasonal cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticDeedChest() => new()
	{
		Metadata = new("medieval_locking_domestic_deed_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17329", []),
		Noun = "chest",
		ShortDescription = "a household deed chest",
		LongDescription = null,
		FullDescription = "This household deed chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking chest for property records, charters, and household papers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticDowryChest() => new()
	{
		Metadata = new("medieval_locking_domestic_dowry_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17359", []),
		Noun = "chest",
		ShortDescription = "a lockable dowry chest",
		LongDescription = null,
		FullDescription = "This lockable dowry chest is a large, well-made chest built from walnut boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 34000.0,
		Cost = 280.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine locking chest for dowry goods, heirlooms, or household display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticGuesthouseStrongbox() => new()
	{
		Metadata = new("medieval_locking_domestic_guesthouse_strongbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17389", []),
		Noun = "strongbox",
		ShortDescription = "a guesthouse strongbox",
		LongDescription = null,
		FullDescription = "This guesthouse strongbox is a medium-sized, well-made strongbox built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 16000.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking strongbox for an inn, guesthouse, or rented chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticHeavySafeChest() => new()
	{
		Metadata = new("medieval_locking_domestic_heavy_safe_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17419", []),
		Noun = "chest",
		ShortDescription = "a heavy safe chest",
		LongDescription = null,
		FullDescription = "This heavy safe chest is a large, well-made chest worked from wrought iron. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 70000.0,
		Cost = 760.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy safe-chest profile for elite households, manor offices, or institutional rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticHeirloomCoffer() => new()
	{
		Metadata = new("medieval_locking_domestic_heirloom_coffer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17449", []),
		Noun = "coffer",
		ShortDescription = "an heirloom coffer",
		LongDescription = null,
		FullDescription = "This heirloom coffer is a medium-sized, well-made coffer built from walnut boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 15000.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking coffer for family heirlooms and valued household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticHouseholdStrongChest() => new()
	{
		Metadata = new("medieval_locking_domestic_household_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17479", []),
		Noun = "chest",
		ShortDescription = "a household strong chest",
		LongDescription = null,
		FullDescription = "This household strong chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 42000.0,
		Cost = 420.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy built-in-lock chest for household wealth and records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticIronboundSafeChest() => new()
	{
		Metadata = new("medieval_locking_domestic_ironbound_safe_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17509", []),
		Noun = "chest",
		ShortDescription = "an iron-bound safe chest",
		LongDescription = null,
		FullDescription = "This iron-bound safe chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 52000.0,
		Cost = 520.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large oak safe chest reinforced with iron bands.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticLinenStrongChest() => new()
	{
		Metadata = new("medieval_locking_domestic_linen_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17539", []),
		Noun = "chest",
		ShortDescription = "a linen strong chest",
		LongDescription = null,
		FullDescription = "This linen strong chest is a large, well-made chest built from cedar boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 30000.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking chest for household linens, blankets, and better textiles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticLockingFootlocker() => new()
	{
		Metadata = new("medieval_locking_domestic_locking_footlocker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17569", []),
		Noun = "footlocker",
		ShortDescription = "a locking footlocker",
		LongDescription = null,
		FullDescription = "This locking footlocker is a medium-sized, workmanlike footlocker built from oak boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact chest for clothing, bedding, or personal effects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticMedicineChest() => new()
	{
		Metadata = new("medieval_locking_domestic_medicine_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17599", []),
		Noun = "chest",
		ShortDescription = "a locked medicine chest",
		LongDescription = null,
		FullDescription = "This locked medicine chest is a medium-sized, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 13000.0,
		Cost = 185.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Built-in-lock chest for medicines, salves, and household stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticOakLockingChest() => new()
	{
		Metadata = new("medieval_locking_domestic_oak_locking_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17629", []),
		Noun = "chest",
		ShortDescription = "a locking oak chest",
		LongDescription = null,
		FullDescription = "This locking oak chest is a large, workmanlike chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 32000.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large household chest with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticSilverChest() => new()
	{
		Metadata = new("medieval_locking_domestic_silver_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17659", []),
		Noun = "chest",
		ShortDescription = "a household silver chest",
		LongDescription = null,
		FullDescription = "This household silver chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stout lockable chest for silver tableware or valuable household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticSmallBronzeCasket() => new()
	{
		Metadata = new("medieval_locking_domestic_small_bronze_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17689", []),
		Noun = "casket",
		ShortDescription = "a bronze-bound casket",
		LongDescription = null,
		FullDescription = "This bronze-bound casket is a small, well-made casket worked from bronze. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2800.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small metal casket for compact valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticWallNicheLockbox() => new()
	{
		Metadata = new("medieval_locking_domestic_wall_niche_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17719", []),
		Noun = "lockbox",
		ShortDescription = "a wall-niche lockbox",
		LongDescription = null,
		FullDescription = "This wall-niche lockbox is a small, workmanlike lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small lockbox intended for storage niches, cabinets, or bedside use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticNorthernSeaChest() => new()
	{
		Metadata = new("medieval_locking_domestic_northern_sea_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17749", []),
		Noun = "chest",
		ShortDescription = "a sea-chest footlocker",
		LongDescription = null,
		FullDescription = "This sea-chest footlocker is a medium-sized, workmanlike chest built from pine boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 17000.0,
		Cost = 135.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking footlocker suited to travel, shipboard storage, or northern households.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticWesternMouldedChest() => new()
	{
		Metadata = new("medieval_locking_domestic_western_moulded_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17779", []),
		Noun = "chest",
		ShortDescription = "a moulded walnut chest",
		LongDescription = null,
		FullDescription = "This moulded walnut chest is a large, well-made chest built from walnut boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 34000.0,
		Cost = 300.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine lockable chest with moulded panels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticMediterraneanCypressChest() => new()
	{
		Metadata = new("medieval_locking_domestic_mediterranean_cypress_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17809", []),
		Noun = "chest",
		ShortDescription = "a cypress linen chest",
		LongDescription = null,
		FullDescription = "This cypress linen chest is a large, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 28000.0,
		Cost = 230.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cypress locking chest for linens and private household stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticIslamicateCedarChest() => new()
	{
		Metadata = new("medieval_locking_domestic_islamicate_cedar_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17839", []),
		Noun = "chest",
		ShortDescription = "a carved cedar chest",
		LongDescription = null,
		FullDescription = "This carved cedar chest is a large, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 30000.0,
		Cost = 270.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Carved cedar chest with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticSouthAsianTeakStrongbox() => new()
	{
		Metadata = new("medieval_locking_domestic_south_asian_teak_strongbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17869", []),
		Noun = "strongbox",
		ShortDescription = "a teak strongbox",
		LongDescription = null,
		FullDescription = "This teak strongbox is a medium-sized, well-made strongbox built from teak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 300.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy teak strongbox for household valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingDomesticEastAsianCypressChest() => new()
	{
		Metadata = new("medieval_locking_domestic_east_asian_cypress_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17899", []),
		Noun = "chest",
		ShortDescription = "a cypress household chest",
		LongDescription = null,
		FullDescription = "This cypress household chest is a medium-sized, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lockable household chest for papers, clothing, and private goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalAbbasidCedarDocumentCabinet() => new()
	{
		Metadata = new("medieval_abbasid_cedar_document_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17929", []),
		Noun = "cabinet",
		ShortDescription = "a cedar document cabinet",
		LongDescription = null,
		FullDescription = "This cedar document cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 15000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Enclosed cabinet for papers, cases, writing goods, and household records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalAndalusianCarvedCedarSpiceCabinet() => new()
	{
		Metadata = new("medieval_andalusian_carved_cedar_spice_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17959", []),
		Noun = "cabinet",
		ShortDescription = "a carved cedar spice cabinet",
		LongDescription = null,
		FullDescription = "This carved cedar spice cabinet is a medium-sized, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7800.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact cabinet for spices, scents, remedies, and fine dry household stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalAngloDanishBronzeFittedHouseholdChest() => new()
	{
		Metadata = new("medieval_anglo_danish_bronze_fitted_household_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:17989", []),
		Noun = "chest",
		ShortDescription = "a bronze-fitted household chest",
		LongDescription = null,
		FullDescription = "This bronze-fitted household chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 23000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional metal-fitted household chest for textiles and valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalAngloNormanPanelledWalnutSideboard() => new()
	{
		Metadata = new("medieval_anglo_norman_panelled_walnut_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18019", []),
		Noun = "sideboard",
		ShortDescription = "a panelled walnut sideboard",
		LongDescription = null,
		FullDescription = "This panelled walnut sideboard is a large, well-made sideboard built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 175.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Panelled serving storage for tableware and high-status domestic vessels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalByzantineWalnutDisplayShelves() => new()
	{
		Metadata = new("medieval_byzantine_walnut_display_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18049", []),
		Noun = "shelves",
		ShortDescription = "a walnut display shelf",
		LongDescription = null,
		FullDescription = "These walnut display shelf are large, well-made shelves built from walnut boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 8500.0,
		Cost = 100.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine display shelving for vessels, icons, lamps, and treasured household pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCapetianFrenchMouldedWalnutCupboard() => new()
	{
		Metadata = new("medieval_capetian_french_moulded_walnut_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18080", []),
		Noun = "cupboard",
		ShortDescription = "a moulded walnut cupboard",
		LongDescription = null,
		FullDescription = "This moulded walnut cupboard is a large, well-made cupboard built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 145.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Refined enclosed cupboard for elite chamber storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCarolingianFrankishTabletBandedLinenChest() => new()
	{
		Metadata = new("medieval_carolingian_frankish_tablet_banded_linen_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18110", []),
		Noun = "chest",
		ShortDescription = "a tablet-banded linen chest",
		LongDescription = null,
		FullDescription = "This tablet-banded linen chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 21000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorated chest for linens, clothing, and household display goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChristianIberianOlivewoodLinenChest() => new()
	{
		Metadata = new("medieval_christian_iberian_olivewood_linen_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18140", []),
		Noun = "chest",
		ShortDescription = "an olivewood linen chest",
		LongDescription = null,
		FullDescription = "This olivewood linen chest is a large, well-made chest built from olive wood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 20000.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "olive wood",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional hardwood chest for linens and chamber textiles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEarlyAngloSaxonAshLinenCupboard() => new()
	{
		Metadata = new("medieval_early_anglo_saxon_ash_linen_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18170", []),
		Noun = "cupboard",
		ShortDescription = "an ash linen cupboard",
		LongDescription = null,
		FullDescription = "This ash linen cupboard is a large, workmanlike cupboard built from ash boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14000.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Practical regional cupboard for linens and clothing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFatimidPaintedLinenCupboard() => new()
	{
		Metadata = new("medieval_fatimid_painted_linen_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18200", []),
		Noun = "cupboard",
		ShortDescription = "a painted linen cupboard",
		LongDescription = null,
		FullDescription = "This painted linen cupboard is a large, well-made cupboard built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 15000.0,
		Cost = 115.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorated cupboard for folded linen, fine cloth, and warm-climate household storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalGoryeoKoreaPineClothingChest() => new()
	{
		Metadata = new("medieval_goryeo_korea_pine_clothing_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18230", []),
		Noun = "chest",
		ShortDescription = "a pine clothing chest",
		LongDescription = null,
		FullDescription = "This pine clothing chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 17000.0,
		Cost = 68.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Clothing chest for folded garments, bedding, and household textiles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeianKamakuraJapanBambooPaperShelf() => new()
	{
		Metadata = new("medieval_heian_kamakura_japan_bamboo_paper_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18260", []),
		Noun = "shelf",
		ShortDescription = "a bamboo paper shelf",
		LongDescription = null,
		FullDescription = "This bamboo paper shelf is a medium-sized, workmanlike shelf built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3000.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wall_Shelf"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light shelf for paper packets, writing goods, small boxes, and domestic objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeianKamakuraJapanCypressLowStorageBox() => new()
	{
		Metadata = new("medieval_heian_kamakura_japan_cypress_low_storage_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18291", []),
		Noun = "box",
		ShortDescription = "a low cypress storage box",
		LongDescription = null,
		FullDescription = "This low cypress storage box is a large, well-made box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 12500.0,
		Cost = 92.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low storage box for folded robes, bedding, papers, and domestic goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHighEnglishBritishJoinedOakDresser() => new()
	{
		Metadata = new("medieval_high_english_british_joined_oak_dresser", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18321", []),
		Noun = "dresser",
		ShortDescription = "a joined oak dresser",
		LongDescription = null,
		FullDescription = "This joined oak dresser is a large, well-made dresser built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Dresser"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Joined drawer furniture for clothing, linen, and valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHolyRomanEmpireGermanHeavyOakWardrobe() => new()
	{
		Metadata = new("medieval_holy_roman_empire_german_heavy_oak_wardrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18351", []),
		Noun = "wardrobe",
		ShortDescription = "a heavy oak wardrobe",
		LongDescription = null,
		FullDescription = "This heavy oak wardrobe is a large, well-made wardrobe built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 32000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Substantial wardrobe for garments, furs, and domestic storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIrishGaelicHideBoundOakChest() => new()
	{
		Metadata = new("medieval_irish_gaelic_hide_bound_oak_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18381", []),
		Noun = "chest",
		ShortDescription = "a hide-bound oak chest",
		LongDescription = null,
		FullDescription = "This hide-bound oak chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 86.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rugged hide-bound domestic chest for textiles and household stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMagyarBronzeMountedTravelChest() => new()
	{
		Metadata = new("medieval_magyar_bronze_mounted_travel_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18411", []),
		Noun = "chest",
		ShortDescription = "a bronze-mounted travel chest",
		LongDescription = null,
		FullDescription = "This bronze-mounted travel chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal-fitted chest suited to mobile households and travel goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNormanArchedOakArmoire() => new()
	{
		Metadata = new("medieval_norman_arched_oak_armoire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18441", []),
		Noun = "armoire",
		ShortDescription = "an arched oak armoire",
		LongDescription = null,
		FullDescription = "This arched oak armoire is a large, well-made armoire built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 30000.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Armoire"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Arched armoire for garments, fine textiles, and household chests.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNorseVikingAgeShipCarvedClothesChest() => new()
	{
		Metadata = new("medieval_norse_viking_age_ship_carved_clothes_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18471", []),
		Noun = "chest",
		ShortDescription = "a ship-carved clothes chest",
		LongDescription = null,
		FullDescription = "This ship-carved clothes chest is a large, well-made chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 21000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional carved storage chest for clothing, blankets, and domestic valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNorthIndianRajputTeakClothesChest() => new()
	{
		Metadata = new("medieval_north_indian_rajput_teak_clothes_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18501", []),
		Noun = "chest",
		ShortDescription = "a teak clothes chest",
		LongDescription = null,
		FullDescription = "This teak clothes chest is a large, well-made chest built from teak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 19000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hardwood chest for cotton garments, fine cloth, and domestic valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRusNovgorodPineWinterStorageChest() => new()
	{
		Metadata = new("medieval_rus_novgorod_pine_winter_storage_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18531", []),
		Noun = "chest",
		ShortDescription = "a pine winter storage chest",
		LongDescription = null,
		FullDescription = "This pine winter storage chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stout chest for winter cloth, furs, bedding, and household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalScottishGaelicLowlandElmWinterBlanketBox() => new()
	{
		Metadata = new("medieval_scottish_gaelic_lowland_elm_winter_blanket_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18561", []),
		Noun = "box",
		ShortDescription = "an elm winter blanket box",
		LongDescription = null,
		FullDescription = "This elm winter blanket box is a large, workmanlike box built from elm boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20000.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "elm",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cold-climate blanket box for heavy bedding and outerwear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSeljukAyyubidMamlukTooledChestForWares() => new()
	{
		Metadata = new("medieval_seljuk_ayyubid_mamluk_tooled_chest_for_wares", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18591", []),
		Noun = "chest",
		ShortDescription = "a tooled household chest",
		LongDescription = null,
		FullDescription = "This tooled household chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 19000.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorated chest for robes, linens, and valuable domestic goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSongChinaBambooHouseholdShelves() => new()
	{
		Metadata = new("medieval_song_china_bamboo_household_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18621", []),
		Noun = "shelves",
		ShortDescription = "a bamboo household shelf",
		LongDescription = null,
		FullDescription = "These bamboo household shelf are large, workmanlike shelves built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Light shelving for bowls, baskets, paper goods, and domestic objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSongChinaCypressScrollCabinet() => new()
	{
		Metadata = new("medieval_song_china_cypress_scroll_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18652", []),
		Noun = "cabinet",
		ShortDescription = "a cypress scroll cabinet",
		LongDescription = null,
		FullDescription = "This cypress scroll cabinet is a large, well-made cabinet built from cypress boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 13000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine cabinet for scrolls, papers, brushes, and household records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSouthIndianCholaBambooLaundryHamper() => new()
	{
		Metadata = new("medieval_south_indian_chola_bamboo_laundry_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18682", []),
		Noun = "hamper",
		ShortDescription = "a bamboo laundry hamper",
		LongDescription = null,
		FullDescription = "This bamboo laundry hamper is a large, workmanlike hamper built from split bamboo. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light warm-climate hamper for laundry, cloth, and domestic soft goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSouthIndianCholaSandalwoodJewelCabinet() => new()
	{
		Metadata = new("medieval_south_indian_chola_sandalwood_jewel_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18713", []),
		Noun = "cabinet",
		ShortDescription = "a sandalwood jewel cabinet",
		LongDescription = null,
		FullDescription = "This sandalwood jewel cabinet is a medium-sized, well-made cabinet built from sandalwood. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7200.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sandalwood",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fragrant cabinet for jewels, scent, dressing goods, and small valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSteppeTurkicMongolFeltLinedTravelCoffer() => new()
	{
		Metadata = new("medieval_steppe_turkic_mongol_felt_lined_travel_coffer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:18743", []),
		Noun = "coffer",
		ShortDescription = "a felt-lined travel coffer",
		LongDescription = null,
		FullDescription = "This felt-lined travel coffer is a large, well-made coffer made from pressed felt. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 9000.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft-lined coffer form for mobile household goods and travel textiles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedBambooScreenHanging() => new()
	{
		Metadata = new("medieval_door_shared_bamboo_screen_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:11", []),
		Noun = "screen",
		ShortDescription = "a bamboo doorway screen",
		LongDescription = null,
		FullDescription = "This bamboo doorway screen is a large, workmanlike screen built from split bamboo. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3000.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bamboo screen-like doorway barrier with weak door behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedBarnDoor() => new()
	{
		Metadata = new("medieval_door_shared_barn_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:40", []),
		Noun = "door",
		ShortDescription = "a broad barn door",
		LongDescription = null,
		FullDescription = "This broad barn door is a very large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 62000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad plank door for barns and granaries.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedBathhouseScreenDoor() => new()
	{
		Metadata = new("medieval_door_shared_bathhouse_screen_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:69", []),
		Noun = "screen",
		ShortDescription = "a bathhouse screen door",
		LongDescription = null,
		FullDescription = "This bathhouse screen door is a large, workmanlike screen built from cedar boards. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 17000.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light screen-like door for baths, washrooms, and changing spaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedBoardedYardGate() => new()
	{
		Metadata = new("medieval_door_shared_boarded_yard_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:98", []),
		Noun = "gate",
		ShortDescription = "a boarded yard gate",
		LongDescription = null,
		FullDescription = "This boarded yard gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 52000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Common yard gate; gate behaviour can be seen and fired through.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedBoneBeadDoorwayHanging() => new()
	{
		Metadata = new("medieval_door_shared_bone_bead_doorway_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:127", []),
		Noun = "hanging",
		ShortDescription = "a bone bead doorway hanging",
		LongDescription = null,
		FullDescription = "This bone bead doorway hanging is a large, workmanlike hanging worked from bone. Strings of beads hang from a narrow header, leaving small gaps between each strand. The lower ends are uneven from movement through the passage. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2800.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Beaded doorway hanging that marks a passage without making a secure barrier.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedCarpetDoorwayHanging() => new()
	{
		Metadata = new("medieval_door_shared_carpet_doorway_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:156", []),
		Noun = "hanging",
		ShortDescription = "a carpet doorway hanging",
		LongDescription = null,
		FullDescription = "This carpet doorway hanging is a large, well-made hanging made from woven wool. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy woven doorway hanging used as a soft barrier.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedCartGate() => new()
	{
		Metadata = new("medieval_door_shared_cart_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:185", []),
		Noun = "gate",
		ShortDescription = "a cart-yard gate",
		LongDescription = null,
		FullDescription = "This cart-yard gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 64000.0,
		Cost = 135.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wide yard gate suitable for carts and wagons.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedCellarDoor() => new()
	{
		Metadata = new("medieval_door_shared_cellar_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:214", []),
		Noun = "door",
		ShortDescription = "a braced cellar door",
		LongDescription = null,
		FullDescription = "This braced cellar door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 42000.0,
		Cost = 96.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Secure cellar door, smashable and uninstallable only from the hinge side by component behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedCellarGrate() => new()
	{
		Metadata = new("medieval_door_shared_cellar_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:243", []),
		Noun = "grate",
		ShortDescription = "a cellar stair grate",
		LongDescription = null,
		FullDescription = "This cellar stair grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 52000.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Secure barred grate for cellar stairs or storage entries.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedCountingHouseGrille() => new()
	{
		Metadata = new("medieval_door_shared_counting_house_grille", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:272", []),
		Noun = "grate",
		ShortDescription = "a counting-house grille",
		LongDescription = null,
		FullDescription = "This counting-house grille is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 50000.0,
		Cost = 230.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lockable barred grille for counting rooms, pay windows, or secure counters.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedCourtyardGrilleGate() => new()
	{
		Metadata = new("medieval_door_shared_courtyard_grille_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:301", []),
		Noun = "gate",
		ShortDescription = "a barred courtyard gate",
		LongDescription = null,
		FullDescription = "This barred courtyard gate is a very large, well-made gate worked from wrought iron. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 86000.0,
		Cost = 360.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Very large barred gate for courtyards and enclosed compounds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedFeltDoorwayHanging() => new()
	{
		Metadata = new("medieval_door_shared_felt_doorway_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:330", []),
		Noun = "hanging",
		ShortDescription = "a felt doorway hanging",
		LongDescription = null,
		FullDescription = "This felt doorway hanging is a large, workmanlike hanging made from pressed felt. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3200.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Thicker felt hanging for drafts and privacy, not a secure door.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedFurDoorwayHanging() => new()
	{
		Metadata = new("medieval_door_shared_fur_doorway_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:359", []),
		Noun = "hanging",
		ShortDescription = "a fur doorway hanging",
		LongDescription = null,
		FullDescription = "This fur doorway hanging is a large, workmanlike hanging made from dressed fur. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Warm fur doorway hanging for cold interiors and drafty doors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedGardenGate() => new()
	{
		Metadata = new("medieval_door_shared_garden_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:388", []),
		Noun = "gate",
		ShortDescription = "a garden gate",
		LongDescription = null,
		FullDescription = "This garden gate is a large, workmanlike gate built from willow boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light garden or orchard gate with weak gate behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedGranaryDoor() => new()
	{
		Metadata = new("medieval_door_shared_granary_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:417", []),
		Noun = "door",
		ShortDescription = "a raised granary door",
		LongDescription = null,
		FullDescription = "This raised granary door is a large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35000.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Secure granary door designed for food stores and raised storehouses.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedHeavyHallDoor() => new()
	{
		Metadata = new("medieval_door_shared_heavy_hall_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:446", []),
		Noun = "door",
		ShortDescription = "a heavy hall door",
		LongDescription = null,
		FullDescription = "This heavy hall door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 50000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy plank door for halls, courts, and substantial households.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedInnChamberDoor() => new()
	{
		Metadata = new("medieval_door_shared_inn_chamber_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:475", []),
		Noun = "door",
		ShortDescription = "an inn chamber door",
		LongDescription = null,
		FullDescription = "This inn chamber door is a large, workmanlike door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25500.0,
		Cost = 68.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Modest interior door for inn chambers or rented rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedInteriorChamberDoor() => new()
	{
		Metadata = new("medieval_door_shared_interior_chamber_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:504", []),
		Noun = "door",
		ShortDescription = "an interior chamber door",
		LongDescription = null,
		FullDescription = "This interior chamber door is a large, workmanlike door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lighter door for bedchambers, storerooms, and inner rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedIronboundDoor() => new()
	{
		Metadata = new("medieval_door_shared_ironbound_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:533", []),
		Noun = "door",
		ShortDescription = "an iron-bound door",
		LongDescription = null,
		FullDescription = "This iron-bound door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 57000.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Oak door strengthened with visible iron strapping and no built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLeatherStripHanging() => new()
	{
		Metadata = new("medieval_door_shared_leather_strip_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:562", []),
		Noun = "hanging",
		ShortDescription = "a leather strip doorway hanging",
		LongDescription = null,
		FullDescription = "This leather strip doorway hanging is a large, workmanlike hanging made from worked leather. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Strip-hanging for workshops, stables, or storerooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLinenDoorwayCurtain() => new()
	{
		Metadata = new("medieval_door_shared_linen_doorway_curtain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:591", []),
		Noun = "curtain",
		ShortDescription = "a linen doorway curtain",
		LongDescription = null,
		FullDescription = "This linen doorway curtain is a large, workmanlike curtain made from woven linen. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light doorway curtain with weak door behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLivestockPenGate() => new()
	{
		Metadata = new("medieval_door_shared_livestock_pen_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:620", []),
		Noun = "gate",
		ShortDescription = "a livestock pen gate",
		LongDescription = null,
		FullDescription = "This livestock pen gate is a large, workmanlike gate built from ash boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple gate for pens, folds, and stockyards.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableBarnDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_barn_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:649", []),
		Noun = "door",
		ShortDescription = "a lockable barn door",
		LongDescription = null,
		FullDescription = "This lockable barn door is a very large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 66000.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large barn door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableCartGate() => new()
	{
		Metadata = new("medieval_door_shared_lockable_cart_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:678", []),
		Noun = "gate",
		ShortDescription = "a lockable cart-yard gate",
		LongDescription = null,
		FullDescription = "This lockable cart-yard gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 70000.0,
		Cost = 205.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wide cart gate with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableCellarDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_cellar_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:707", []),
		Noun = "door",
		ShortDescription = "a lockable braced cellar door",
		LongDescription = null,
		FullDescription = "This lockable braced cellar door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 45000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cellar door with built-in lock and hinge-side security behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableChamberDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_chamber_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:736", []),
		Noun = "door",
		ShortDescription = "a lockable chamber door",
		LongDescription = null,
		FullDescription = "This lockable chamber door is a large, well-made door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26500.0,
		Cost = 92.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Inner-room door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableCottageDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_cottage_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:765", []),
		Noun = "door",
		ShortDescription = "a lockable cottage door",
		LongDescription = null,
		FullDescription = "This lockable cottage door is a large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30500.0,
		Cost = 88.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain cottage-scale door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableCourtyardGrilleGate() => new()
	{
		Metadata = new("medieval_door_shared_lockable_courtyard_grille_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:794", []),
		Noun = "gate",
		ShortDescription = "a lockable barred courtyard gate",
		LongDescription = null,
		FullDescription = "This lockable barred courtyard gate is a very large, well-made gate worked from wrought iron. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 92000.0,
		Cost = 450.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lockable very large barred gate for courtyards and compounds.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
