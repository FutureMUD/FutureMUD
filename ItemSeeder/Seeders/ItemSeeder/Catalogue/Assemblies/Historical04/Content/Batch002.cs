#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalDomesticHallCoffer() => new()
	{
		Metadata = new("medieval_domestic_hall_coffer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13563", []),
		Noun = "coffer",
		ShortDescription = "an oak hall coffer",
		LongDescription = null,
		FullDescription = "This oak hall coffer is a large, workmanlike coffer built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 82.0m,
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
		BuilderNotes = "Hall coffer for mixed household storage, clothing, table linens, and guest goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHallServiceSideboard() => new()
	{
		Metadata = new("medieval_domestic_hall_service_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13593", []),
		Noun = "sideboard",
		ShortDescription = "a hall service sideboard",
		LongDescription = null,
		FullDescription = "This hall service sideboard is a large, workmanlike sideboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 19000.0,
		Cost = 78.0m,
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
				"Container_Sideboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large sideboard for hall service, dishes, cups, and folded cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHallSideboard() => new()
	{
		Metadata = new("medieval_domestic_hall_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13623", []),
		Noun = "sideboard",
		ShortDescription = "a hall sideboard",
		LongDescription = null,
		FullDescription = "This hall sideboard is a large, workmanlike sideboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 105.0m,
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
				"Container_Sideboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hall sideboard for cups, trays, pitchers, pouches, and guest-service goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHangingGarmentCupboard() => new()
	{
		Metadata = new("medieval_domestic_hanging_garment_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13653", []),
		Noun = "cupboard",
		ShortDescription = "a hanging garment cupboard",
		LongDescription = null,
		FullDescription = "This hanging garment cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15500.0,
		Cost = 58.0m,
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
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tall cupboard for hanging garments, cloaks, hoods, and spare belts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHearthShelf() => new()
	{
		Metadata = new("medieval_domestic_hearth_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13683", []),
		Noun = "shelf",
		ShortDescription = "a hearth shelf",
		LongDescription = null,
		FullDescription = "This hearth shelf is a medium-sized, workmanlike shelf built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3200.0,
		Cost = 18.0m,
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
		BuilderNotes = "Shelf suited to utensils, small pots, tinder, and hearth-side domestic pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHearthToolBin() => new()
	{
		Metadata = new("medieval_domestic_hearth_tool_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13714", []),
		Noun = "bin",
		ShortDescription = "a hearth-tool bin",
		LongDescription = null,
		FullDescription = "This hearth-tool bin is a medium-sized, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
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
				"Container_Open_Bin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open bin for hearth tools, tongs, pokers, kindling, and fire supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHerbDrawerBox() => new()
	{
		Metadata = new("medieval_domestic_herb_drawer_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13745", []),
		Noun = "box",
		ShortDescription = "an herb drawer box",
		LongDescription = null,
		FullDescription = "This herb drawer box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 680.0,
		Cost = 32.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small lidded box for herb packets, scent sachets, and remedy packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHerbDryingShelf() => new()
	{
		Metadata = new("medieval_domestic_herb_drying_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13775", []),
		Noun = "shelf",
		ShortDescription = "an herb-drying shelf",
		LongDescription = null,
		FullDescription = "This herb-drying shelf is a medium-sized, workmanlike shelf built from willow boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
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
				"Container_Wall_Shelf"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open shelf for drying herbs, flowers, small packets, and kitchen plants.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticIconDisplayShelves() => new()
	{
		Metadata = new("medieval_domestic_icon_display_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13806", []),
		Noun = "shelves",
		ShortDescription = "a set of devotional display shelves",
		LongDescription = null,
		FullDescription = "These set of devotional display shelves are large, well-made shelves built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 7200.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
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
		BuilderNotes = "Display shelves for household devotional pieces, lamps, small vessels, or keepsakes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticInkAndPenBox() => new()
	{
		Metadata = new("medieval_domestic_ink_and_pen_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13837", []),
		Noun = "box",
		ShortDescription = "an ink-and-pen box",
		LongDescription = null,
		FullDescription = "This ink-and-pen box is a small, well-made box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 700.0,
		Cost = 28.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small box for pens, ink pieces, wax, and writing supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticKeepsakeChest() => new()
	{
		Metadata = new("medieval_domestic_keepsake_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13867", []),
		Noun = "chest",
		ShortDescription = "a keepsake chest",
		LongDescription = null,
		FullDescription = "This keepsake chest is a medium-sized, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7600.0,
		Cost = 64.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Smaller chest for keepsakes, personal papers, jewellery boxes, and fine pouches.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticKindlingBasket() => new()
	{
		Metadata = new("medieval_domestic_kindling_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13897", []),
		Noun = "basket",
		ShortDescription = "a kindling basket",
		LongDescription = null,
		FullDescription = "This kindling basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 8.0m,
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
		BuilderNotes = "Open basket for kindling, small fuel pieces, and hearth supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticKitchenBowlCupboard() => new()
	{
		Metadata = new("medieval_domestic_kitchen_bowl_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13928", []),
		Noun = "cupboard",
		ShortDescription = "a kitchen bowl cupboard",
		LongDescription = null,
		FullDescription = "This kitchen bowl cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 13000.0,
		Cost = 44.0m,
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
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Kitchen cupboard for bowls, cups, plates, and everyday serving wares.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticKitchenHutch() => new()
	{
		Metadata = new("medieval_domestic_kitchen_hutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13958", []),
		Noun = "hutch",
		ShortDescription = "an oak kitchen hutch",
		LongDescription = null,
		FullDescription = "This oak kitchen hutch is a large, workmanlike hutch built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26000.0,
		Cost = 105.0m,
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
				"Container_Hutch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Kitchen hutch for dishes, bowls, jars, towels, and cooking vessels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticKitchenServiceHutch() => new()
	{
		Metadata = new("medieval_domestic_kitchen_service_hutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:13988", []),
		Noun = "hutch",
		ShortDescription = "a kitchen service hutch",
		LongDescription = null,
		FullDescription = "This kitchen service hutch is a large, workmanlike hutch built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15000.0,
		Cost = 54.0m,
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
				"Container_Hutch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Kitchen hutch for bowls, cups, spoons, towels, and common wares.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLarderCupboard() => new()
	{
		Metadata = new("medieval_domestic_larder_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14018", []),
		Noun = "cupboard",
		ShortDescription = "a larder cupboard",
		LongDescription = null,
		FullDescription = "This larder cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15500.0,
		Cost = 68.0m,
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
		BuilderNotes = "Cupboard for dry foods, sealed jars, bread, and kitchen stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLargeArmoire() => new()
	{
		Metadata = new("medieval_domestic_large_armoire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14048", []),
		Noun = "armoire",
		ShortDescription = "a large oak armoire",
		LongDescription = null,
		FullDescription = "This large oak armoire is a large, well-made armoire built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 32000.0,
		Cost = 130.0m,
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
		BuilderNotes = "Large enclosed armoire for garments, chests, folded bedding, and valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLargeBeddingArk() => new()
	{
		Metadata = new("medieval_domestic_large_bedding_ark", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14078", []),
		Noun = "ark",
		ShortDescription = "a large bedding ark",
		LongDescription = null,
		FullDescription = "This large bedding ark is a large, workmanlike ark built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
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
				"Container_Blanket_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large chest-like ark for blankets, quilts, winter bedding, and spare cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLargeOakCabinet() => new()
	{
		Metadata = new("medieval_domestic_large_oak_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14108", []),
		Noun = "cabinet",
		ShortDescription = "a large oak cabinet",
		LongDescription = null,
		FullDescription = "This large oak cabinet is a large, workmanlike cabinet built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 19000.0,
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
				"Container_Large_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large enclosed cabinet for domestic stores, linens, vessels, and household tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLargePineCabinet() => new()
	{
		Metadata = new("medieval_domestic_large_pine_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14138", []),
		Noun = "cabinet",
		ShortDescription = "a large pine cabinet",
		LongDescription = null,
		FullDescription = "This large pine cabinet is a large, workmanlike cabinet built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15500.0,
		Cost = 56.0m,
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
				"Container_Large_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large simple cabinet for bulky household goods and daily storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLargeTableSurface() => new()
	{
		Metadata = new("medieval_domestic_large_table_surface", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14168", []),
		Noun = "table",
		ShortDescription = "a large household table",
		LongDescription = null,
		FullDescription = "This large household table is a very large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 36000.0,
		Cost = 120.0m,
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
				"Container_Large_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large table surface for household work, dining wares, trays, and containers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLargeWalnutCabinet() => new()
	{
		Metadata = new("medieval_domestic_large_walnut_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14199", []),
		Noun = "cabinet",
		ShortDescription = "a large walnut cabinet",
		LongDescription = null,
		FullDescription = "This large walnut cabinet is a large, well-made cabinet built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 17500.0,
		Cost = 130.0m,
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
				"Container_Large_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine cabinet for elite household goods, vessels, writing boxes, and display storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLaundrySortingHamper() => new()
	{
		Metadata = new("medieval_domestic_laundry_sorting_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14229", []),
		Noun = "hamper",
		ShortDescription = "a laundry sorting hamper",
		LongDescription = null,
		FullDescription = "This laundry sorting hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1400.0,
		Cost = 12.0m,
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
		BuilderNotes = "Open hamper for dirty linen, towels, clothing, and washday sorting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenArmoire() => new()
	{
		Metadata = new("medieval_domestic_linen_armoire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14260", []),
		Noun = "armoire",
		ShortDescription = "a linen armoire",
		LongDescription = null,
		FullDescription = "This linen armoire is a large, workmanlike armoire built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28500.0,
		Cost = 110.0m,
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
				"Container_Armoire"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large armoire for sheets, coverlets, table linens, towels, and clothing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenCupboard() => new()
	{
		Metadata = new("medieval_domestic_linen_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14290", []),
		Noun = "cupboard",
		ShortDescription = "a linen cupboard",
		LongDescription = null,
		FullDescription = "This linen cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15000.0,
		Cost = 72.0m,
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
		BuilderNotes = "Enclosed cupboard for folded linens, towels, bedding, and household cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenDresser() => new()
	{
		Metadata = new("medieval_domestic_linen_dresser", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14320", []),
		Noun = "dresser",
		ShortDescription = "a linen dresser",
		LongDescription = null,
		FullDescription = "This linen dresser is a large, workmanlike dresser built from beech boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 21000.0,
		Cost = 86.0m,
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
				"Container_Dresser"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Drawer storage for folded cloth, linens, towels, and bedding pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenLaundryHamper() => new()
	{
		Metadata = new("medieval_domestic_linen_laundry_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14350", []),
		Noun = "hamper",
		ShortDescription = "a linen laundry hamper",
		LongDescription = null,
		FullDescription = "This linen laundry hamper is a large, workmanlike hamper made from woven linen. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
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
		BuilderNotes = "Light open hamper for household laundry and soft goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenPressCupboard() => new()
	{
		Metadata = new("medieval_domestic_linen_press_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14381", []),
		Noun = "cupboard",
		ShortDescription = "a linen press cupboard",
		LongDescription = null,
		FullDescription = "This linen press cupboard is a large, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
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
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large cupboard for pressed linen, napery, and folded household cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenShelf() => new()
	{
		Metadata = new("medieval_domestic_linen_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14411", []),
		Noun = "shelf",
		ShortDescription = "a linen shelf",
		LongDescription = null,
		FullDescription = "This linen shelf is a large, workmanlike shelf built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5400.0,
		Cost = 34.0m,
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
		BuilderNotes = "Broad shelves for folded linen, towels, and domestic cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenSideboard() => new()
	{
		Metadata = new("medieval_domestic_linen_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14442", []),
		Noun = "sideboard",
		ShortDescription = "a linen sideboard",
		LongDescription = null,
		FullDescription = "This linen sideboard is a large, workmanlike sideboard built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20000.0,
		Cost = 80.0m,
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
				"Container_Sideboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sideboard for table linens, towels, napery, and serving pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenSortingHamper() => new()
	{
		Metadata = new("medieval_domestic_linen_sorting_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14472", []),
		Noun = "hamper",
		ShortDescription = "a linen sorting hamper",
		LongDescription = null,
		FullDescription = "This linen sorting hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 16.0m,
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
		BuilderNotes = "Large open hamper for sorting folded cloth, washing, and household linens.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenWallShelf() => new()
	{
		Metadata = new("medieval_domestic_linen_wall_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14503", []),
		Noun = "shelf",
		ShortDescription = "a linen wall shelf",
		LongDescription = null,
		FullDescription = "This linen wall shelf is a large, workmanlike shelf built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4600.0,
		Cost = 26.0m,
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
		BuilderNotes = "Shelf for folded cloth, towels, household linens, and stored clothing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLoomAccessoryChest() => new()
	{
		Metadata = new("medieval_domestic_loom_accessory_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14534", []),
		Noun = "chest",
		ShortDescription = "a weaving chest",
		LongDescription = null,
		FullDescription = "This weaving chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 70.0m,
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
		BuilderNotes = "Chest for shuttles, heddle pieces, fibre packets, and domestic textile tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLoomThreadCabinet() => new()
	{
		Metadata = new("medieval_domestic_loom_thread_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14564", []),
		Noun = "cabinet",
		ShortDescription = "a loom-thread cabinet",
		LongDescription = null,
		FullDescription = "This loom-thread cabinet is a large, well-made cabinet built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 14500.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
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
		BuilderNotes = "Cabinet for skeins, shuttles, threads, and domestic textile work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLowBenchSurface() => new()
	{
		Metadata = new("medieval_domestic_low_bench_surface", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14594", []),
		Noun = "bench",
		ShortDescription = "a low storage bench",
		LongDescription = null,
		FullDescription = "This low storage bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 54.0m,
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
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bench surface for baskets, folded cloth, tools, and domestic objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLowBookShelves() => new()
	{
		Metadata = new("medieval_domestic_low_book_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14625", []),
		Noun = "shelves",
		ShortDescription = "a set of low book shelves",
		LongDescription = null,
		FullDescription = "These set of low book shelves are medium-sized, workmanlike shelves built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5400.0,
		Cost = 30.0m,
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
				"Container_Bookcase_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low shelves for books, tablets, teaching boards, and household records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLowChamberSideboard() => new()
	{
		Metadata = new("medieval_domestic_low_chamber_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14656", []),
		Noun = "sideboard",
		ShortDescription = "a low chamber sideboard",
		LongDescription = null,
		FullDescription = "This low chamber sideboard is a large, workmanlike sideboard built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15000.0,
		Cost = 62.0m,
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
				"Container_Sideboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lower sideboard for chamber vessels, boxes, cloth, and dressing goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMedicineCupboard() => new()
	{
		Metadata = new("medieval_domestic_medicine_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14686", []),
		Noun = "cupboard",
		ShortDescription = "a medicine cupboard",
		LongDescription = null,
		FullDescription = "This medicine cupboard is a medium-sized, well-made cupboard built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8200.0,
		Cost = 64.0m,
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
		BuilderNotes = "Small cupboard for remedies, herb packets, salves, and household treatment goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMedicineNightstand() => new()
	{
		Metadata = new("medieval_domestic_medicine_nightstand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14716", []),
		Noun = "nightstand",
		ShortDescription = "a medicine nightstand",
		LongDescription = null,
		FullDescription = "This medicine nightstand is a medium-sized, well-made nightstand built from cedar boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5600.0,
		Cost = 52.0m,
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
				"Container_Nightstand"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small cedar nightstand for remedy packets, bottles, cloths, and cups.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNarrowBeechCupRack() => new()
	{
		Metadata = new("medieval_domestic_narrow_beech_cup_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14746", []),
		Noun = "rack",
		ShortDescription = "a narrow beech cup rack",
		LongDescription = null,
		FullDescription = "This narrow beech cup rack is a large, workmanlike rack built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4800.0,
		Cost = 26.0m,
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
				"Container_Narrow_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow rack for cups, small bowls, drinking vessels, and serving pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNarrowCloakCupboard() => new()
	{
		Metadata = new("medieval_domestic_narrow_cloak_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14777", []),
		Noun = "cupboard",
		ShortDescription = "a narrow cloak cupboard",
		LongDescription = null,
		FullDescription = "This narrow cloak cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18500.0,
		Cost = 76.0m,
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
		BuilderNotes = "Narrow wardrobe-like cupboard for cloaks, hoods, belts, and travel garments.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNarrowHerbShelf() => new()
	{
		Metadata = new("medieval_domestic_narrow_herb_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14807", []),
		Noun = "shelf",
		ShortDescription = "a narrow herb shelf",
		LongDescription = null,
		FullDescription = "This narrow herb shelf is a medium-sized, workmanlike shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2500.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow shelves for herb bundles, sachets, remedies, and small jars.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNarrowLampRack() => new()
	{
		Metadata = new("medieval_domestic_narrow_lamp_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14838", []),
		Noun = "rack",
		ShortDescription = "a narrow lamp rack",
		LongDescription = null,
		FullDescription = "This narrow lamp rack is a large, workmanlike rack built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5400.0,
		Cost = 28.0m,
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
				"Container_Narrow_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open rack for lamps, candles, oil vessels, and lighting supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNarrowOakShelfRack() => new()
	{
		Metadata = new("medieval_domestic_narrow_oak_shelf_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14869", []),
		Noun = "rack",
		ShortDescription = "a narrow oak shelf rack",
		LongDescription = null,
		FullDescription = "This narrow oak shelf rack is a large, workmanlike rack built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6400.0,
		Cost = 32.0m,
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
				"Container_Narrow_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow shelf rack for a hall, pantry, chamber, or small storeroom.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNarrowPinePantryRack() => new()
	{
		Metadata = new("medieval_domestic_narrow_pine_pantry_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14900", []),
		Noun = "rack",
		ShortDescription = "a narrow pine pantry rack",
		LongDescription = null,
		FullDescription = "This narrow pine pantry rack is a large, workmanlike rack built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 24.0m,
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
				"Container_Narrow_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple pantry rack for jars, sacks, crockery, and small food containers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNarrowScrollRack() => new()
	{
		Metadata = new("medieval_domestic_narrow_scroll_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14931", []),
		Noun = "rack",
		ShortDescription = "a narrow scroll rack",
		LongDescription = null,
		FullDescription = "This narrow scroll rack is a large, well-made rack built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 5000.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open rack for scroll tubes, papers, tablets, and writing cases.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNarrowShoeRack() => new()
	{
		Metadata = new("medieval_domestic_narrow_shoe_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14962", []),
		Noun = "rack",
		ShortDescription = "a narrow shoe rack",
		LongDescription = null,
		FullDescription = "This narrow shoe rack is a medium-sized, workmanlike rack built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
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
				"Container_Narrow_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low narrow rack for shoes, sandals, boots, or household footgear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNarrowSpiceCupboard() => new()
	{
		Metadata = new("medieval_domestic_narrow_spice_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:14993", []),
		Noun = "cupboard",
		ShortDescription = "a narrow spice cupboard",
		LongDescription = null,
		FullDescription = "This narrow spice cupboard is a medium-sized, well-made cupboard built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 6200.0,
		Cost = 48.0m,
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
		BuilderNotes = "Narrow cupboard for spices, scent packets, remedies, and small jars.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNeedleworkStandDrawer() => new()
	{
		Metadata = new("medieval_domestic_needlework_stand_drawer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15023", []),
		Noun = "drawer",
		ShortDescription = "a needlework stand drawer",
		LongDescription = null,
		FullDescription = "This needlework stand drawer is a medium-sized, workmanlike drawer built from beech boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 24.0m,
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
				"Container_Desk_Drawers"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small drawer for thread, needles, patches, and textile work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNurseryChest() => new()
	{
		Metadata = new("medieval_domestic_nursery_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15053", []),
		Noun = "chest",
		ShortDescription = "a nursery chest",
		LongDescription = null,
		FullDescription = "This nursery chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14000.0,
		Cost = 40.0m,
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
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Chest for children's clothing, bedding, toys, and small household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticOakChestOfDrawers() => new()
	{
		Metadata = new("medieval_domestic_oak_chest_of_drawers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15083", []),
		Noun = "dresser",
		ShortDescription = "an oak chest of drawers",
		LongDescription = null,
		FullDescription = "This oak chest of drawers is a large, workmanlike dresser built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 105.0m,
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
				"Container_Dresser"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Drawer furniture for garments, linen, pouches, and personal goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticOakFootlocker() => new()
	{
		Metadata = new("medieval_domestic_oak_footlocker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15113", []),
		Noun = "footlocker",
		ShortDescription = "an oak footlocker",
		LongDescription = null,
		FullDescription = "This oak footlocker is a medium-sized, workmanlike footlocker built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 11000.0,
		Cost = 52.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy compact trunk for clothes, shoes, and personal containers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticOakHouseholdCupboard() => new()
	{
		Metadata = new("medieval_domestic_oak_household_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15143", []),
		Noun = "cupboard",
		ShortDescription = "an oak household cupboard",
		LongDescription = null,
		FullDescription = "This oak household cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16000.0,
		Cost = 70.0m,
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
		BuilderNotes = "Sturdy cupboard for ordinary domestic storage in a hall, chamber, or kitchen.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticOakSmallCabinet() => new()
	{
		Metadata = new("medieval_domestic_oak_small_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15173", []),
		Noun = "cabinet",
		ShortDescription = "an oak small cabinet",
		LongDescription = null,
		FullDescription = "This oak small cabinet is a medium-sized, workmanlike cabinet built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8600.0,
		Cost = 46.0m,
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
				"Container_Small_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy compact cabinet for valuables, papers, vessels, or personal containers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticOakStorageChest() => new()
	{
		Metadata = new("medieval_domestic_oak_storage_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15203", []),
		Noun = "chest",
		ShortDescription = "an oak storage chest",
		LongDescription = null,
		FullDescription = "This oak storage chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 21000.0,
		Cost = 80.0m,
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
		BuilderNotes = "Stout chest for household valuables, clothing, bedding, or stored wares.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPaintedSmallCabinet() => new()
	{
		Metadata = new("medieval_domestic_painted_small_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15233", []),
		Noun = "cabinet",
		ShortDescription = "a painted small cabinet",
		LongDescription = null,
		FullDescription = "This painted small cabinet is a medium-sized, well-made cabinet built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7200.0,
		Cost = 48.0m,
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
				"Container_Small_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorated compact cabinet for chamber goods, small vessels, and personal storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPantryPacketCabinet() => new()
	{
		Metadata = new("medieval_domestic_pantry_packet_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15263", []),
		Noun = "cabinet",
		ShortDescription = "a pantry packet cabinet",
		LongDescription = null,
		FullDescription = "This pantry packet cabinet is a large, workmanlike cabinet built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12500.0,
		Cost = 54.0m,
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
				"Container_Large_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cabinet for packets, jars, sachets, and small dry household stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPineClothesPress() => new()
	{
		Metadata = new("medieval_domestic_pine_clothes_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15293", []),
		Noun = "press",
		ShortDescription = "a pine clothes press",
		LongDescription = null,
		FullDescription = "This pine clothes press is a large, workmanlike press built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 21000.0,
		Cost = 68.0m,
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
				"Container_Wardrobe"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple clothes press for garments, folded linens, and chamber storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainArmoire() => new()
	{
		Metadata = new("medieval_domestic_plain_armoire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15323", []),
		Noun = "armoire",
		ShortDescription = "a plain pine armoire",
		LongDescription = null,
		FullDescription = "This plain pine armoire is a large, workmanlike armoire built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26000.0,
		Cost = 80.0m,
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
				"Container_Armoire"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large cupboard-like armoire for broad domestic storage and clothing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainClothesPress() => new()
	{
		Metadata = new("medieval_domestic_plain_clothes_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15353", []),
		Noun = "press",
		ShortDescription = "a plain clothes press",
		LongDescription = null,
		FullDescription = "This plain clothes press is a large, workmanlike press built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14500.0,
		Cost = 50.0m,
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
				"Container_Large_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain domestic press for folded clothing and spare bedding.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainDeskDrawers() => new()
	{
		Metadata = new("medieval_domestic_plain_desk_drawers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15383", []),
		Noun = "desk",
		ShortDescription = "a plain desk with drawers",
		LongDescription = null,
		FullDescription = "This plain desk with drawers is a large, workmanlike desk built from pine boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 21000.0,
		Cost = 72.0m,
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
				"Container_Desk_Drawers"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple writing-table storage for household records, pens, and small tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainNightstand() => new()
	{
		Metadata = new("medieval_domestic_plain_nightstand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15413", []),
		Noun = "nightstand",
		ShortDescription = "a plain pine nightstand",
		LongDescription = null,
		FullDescription = "This plain pine nightstand is a medium-sized, workmanlike nightstand built from pine boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 28.0m,
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
				"Container_Nightstand"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple nightstand with storage for small chamber items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainOakWardrobe() => new()
	{
		Metadata = new("medieval_domestic_plain_oak_wardrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15443", []),
		Noun = "wardrobe",
		ShortDescription = "a plain oak wardrobe",
		LongDescription = null,
		FullDescription = "This plain oak wardrobe is a large, workmanlike wardrobe built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26000.0,
		Cost = 95.0m,
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
		BuilderNotes = "Wardrobe for clothing, cloaks, belts, shoes, and folded domestic textiles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainPineChest() => new()
	{
		Metadata = new("medieval_domestic_plain_pine_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15473", []),
		Noun = "chest",
		ShortDescription = "a plain pine chest",
		LongDescription = null,
		FullDescription = "This plain pine chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14500.0,
		Cost = 42.0m,
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
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Basic lidded chest for clothes, linens, bedding, or household stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainPineCounter() => new()
	{
		Metadata = new("medieval_domestic_plain_pine_counter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15503", []),
		Noun = "counter",
		ShortDescription = "a plain pine counter",
		LongDescription = null,
		FullDescription = "This plain pine counter is a large, workmanlike counter built from pine boards. A stable frame carries the weight, with the working surface placed where hands naturally reach it. The edges are worn smooth by household use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 21000.0,
		Cost = 60.0m,
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
				"Container_Counter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple counter surface for household work and temporary storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainPineHutch() => new()
	{
		Metadata = new("medieval_domestic_plain_pine_hutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15534", []),
		Noun = "hutch",
		ShortDescription = "a plain pine kitchen hutch",
		LongDescription = null,
		FullDescription = "This plain pine kitchen hutch is a large, workmanlike hutch built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 21000.0,
		Cost = 68.0m,
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
				"Container_Hutch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple kitchen hutch for ordinary crockery, food jars, and domestic wares.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainPineSideboard() => new()
	{
		Metadata = new("medieval_domestic_plain_pine_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15564", []),
		Noun = "sideboard",
		ShortDescription = "a plain pine sideboard",
		LongDescription = null,
		FullDescription = "This plain pine sideboard is a large, workmanlike sideboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 56.0m,
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
				"Container_Sideboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple sideboard for cups, plates, folded cloth, and household tableware.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainPineWallShelf() => new()
	{
		Metadata = new("medieval_domestic_plain_pine_wall_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15594", []),
		Noun = "shelf",
		ShortDescription = "a plain pine wall shelf",
		LongDescription = null,
		FullDescription = "This plain pine wall shelf is a medium-sized, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1900.0,
		Cost = 10.0m,
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
				"Container_Wall_Shelf"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple shelf for ordinary domestic storage above tables, beds, or work areas.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainSmallCabinet() => new()
	{
		Metadata = new("medieval_domestic_plain_small_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15625", []),
		Noun = "cabinet",
		ShortDescription = "a plain small cabinet",
		LongDescription = null,
		FullDescription = "This plain small cabinet is a medium-sized, workmanlike cabinet built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6800.0,
		Cost = 28.0m,
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
				"Container_Small_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact enclosed cabinet for simple domestic goods and chamber storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlateHutch() => new()
	{
		Metadata = new("medieval_domestic_plate_hutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15655", []),
		Noun = "hutch",
		ShortDescription = "a plate hutch",
		LongDescription = null,
		FullDescription = "This plate hutch is a large, workmanlike hutch built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14500.0,
		Cost = 58.0m,
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
				"Container_Hutch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hutch arranged for plates, platters, bowls, and serving pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPrayerBeadBox() => new()
	{
		Metadata = new("medieval_domestic_prayer_bead_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15685", []),
		Noun = "box",
		ShortDescription = "a prayer-bead box",
		LongDescription = null,
		FullDescription = "This prayer-bead box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 400.0,
		Cost = 30.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small cedar box for beads, devotional pieces, keepsakes, and scent sachets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticRingAndPinBox() => new()
	{
		Metadata = new("medieval_domestic_ring_and_pin_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15715", []),
		Noun = "box",
		ShortDescription = "a ring-and-pin box",
		LongDescription = null,
		FullDescription = "This ring-and-pin box is a very small, well-made box built from boxwood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 180.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "boxwood",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small box for rings, pins, brooches, and fine domestic objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticRobeWardrobe() => new()
	{
		Metadata = new("medieval_domestic_robe_wardrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15745", []),
		Noun = "wardrobe",
		ShortDescription = "a robe wardrobe",
		LongDescription = null,
		FullDescription = "This robe wardrobe is a large, well-made wardrobe built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
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
				"Container_Wardrobe"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tall wardrobe for robes, cloaks, belts, and folded fine clothing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticRootCellarBin() => new()
	{
		Metadata = new("medieval_domestic_root_cellar_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15775", []),
		Noun = "bin",
		ShortDescription = "a root-cellar bin",
		LongDescription = null,
		FullDescription = "This root-cellar bin is a large, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7000.0,
		Cost = 20.0m,
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
		BuilderNotes = "Open bin for roots, onions, cabbages, and pantry vegetables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticRushStorageBin() => new()
	{
		Metadata = new("medieval_domestic_rush_storage_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15806", []),
		Noun = "bin",
		ShortDescription = "a rush storage bin",
		LongDescription = null,
		FullDescription = "This rush storage bin is a large, workmanlike bin built from willow boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 12.0m,
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
		BuilderNotes = "Open bin for rushes, straw, reeds, and floor-covering materials.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticRushworkHamper() => new()
	{
		Metadata = new("medieval_domestic_rushwork_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15837", []),
		Noun = "hamper",
		ShortDescription = "a rushwork hamper",
		LongDescription = null,
		FullDescription = "This rushwork hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 11.0m,
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
		BuilderNotes = "Light woven hamper for cloth, bedding, and household soft goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticServantLinenHamper() => new()
	{
		Metadata = new("medieval_domestic_servant_linen_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15868", []),
		Noun = "hamper",
		ShortDescription = "a servant linen hamper",
		LongDescription = null,
		FullDescription = "This servant linen hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2200.0,
		Cost = 14.0m,
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
		BuilderNotes = "Simple open hamper for work linens, towels, bedding, and laundry rounds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticServingCupboard() => new()
	{
		Metadata = new("medieval_domestic_serving_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15899", []),
		Noun = "cupboard",
		ShortDescription = "a serving cupboard",
		LongDescription = null,
		FullDescription = "This serving cupboard is a large, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 86.0m,
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
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better cupboard for feast dishes, pitchers, cups, and serving cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticServingSideboard() => new()
	{
		Metadata = new("medieval_domestic_serving_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15929", []),
		Noun = "sideboard",
		ShortDescription = "an oak serving sideboard",
		LongDescription = null,
		FullDescription = "This oak serving sideboard is a large, workmanlike sideboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 23000.0,
		Cost = 98.0m,
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
				"Container_Sideboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sideboard for serving dishes, cloths, cups, and table goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSewingChest() => new()
	{
		Metadata = new("medieval_domestic_sewing_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15959", []),
		Noun = "chest",
		ShortDescription = "a sewing chest",
		LongDescription = null,
		FullDescription = "This sewing chest is a medium-sized, well-made chest built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8200.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Household sewing chest for cloth scraps, thread, needles, patterns, and mending tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSewingDrawerChest() => new()
	{
		Metadata = new("medieval_domestic_sewing_drawer_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:15989", []),
		Noun = "chest",
		ShortDescription = "a sewing drawer chest",
		LongDescription = null,
		FullDescription = "This sewing drawer chest is a medium-sized, well-made chest built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8800.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
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
		BuilderNotes = "Drawer chest for thread, needles, lace, patches, and textile work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticShoeBin() => new()
	{
		Metadata = new("medieval_domestic_shoe_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16019", []),
		Noun = "bin",
		ShortDescription = "a shoe bin",
		LongDescription = null,
		FullDescription = "This shoe bin is a medium-sized, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3800.0,
		Cost = 16.0m,
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
		BuilderNotes = "Open bin for shoes, sandals, boots, and household footgear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticShoeShelf() => new()
	{
		Metadata = new("medieval_domestic_shoe_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16050", []),
		Noun = "shelf",
		ShortDescription = "a shoe shelf",
		LongDescription = null,
		FullDescription = "This shoe shelf is a medium-sized, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3000.0,
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
				"Container_Narrow_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low shelf for shoes, sandals, clogs, and spare footgear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSideTableStorage() => new()
	{
		Metadata = new("medieval_domestic_side_table_storage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16081", []),
		Noun = "table",
		ShortDescription = "a side table with storage",
		LongDescription = null,
		FullDescription = "This side table with storage is a medium-sized, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9500.0,
		Cost = 48.0m,
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
				"Container_Side_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Side-table container surface for lamps, bowls, books, and small chamber goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSideboardCupboards() => new()
	{
		Metadata = new("medieval_domestic_sideboard_cupboards", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16112", []),
		Noun = "sideboard",
		ShortDescription = "a cupboarded sideboard",
		LongDescription = null,
		FullDescription = "This cupboarded sideboard is a large, well-made sideboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 23000.0,
		Cost = 112.0m,
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
				"Container_Sideboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sideboard with enclosed storage for vessels, cloth, knives, and serving goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSingleOakWallShelf() => new()
	{
		Metadata = new("medieval_domestic_single_oak_wall_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16142", []),
		Noun = "shelf",
		ShortDescription = "a single oak wall shelf",
		LongDescription = null,
		FullDescription = "This single oak wall shelf is a medium-sized, workmanlike shelf built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2800.0,
		Cost = 18.0m,
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
		BuilderNotes = "Single wall shelf for cups, bowls, books, candles, or small household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSmallDresser() => new()
	{
		Metadata = new("medieval_domestic_small_dresser", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16173", []),
		Noun = "dresser",
		ShortDescription = "a small pine dresser",
		LongDescription = null,
		FullDescription = "This small pine dresser is a large, workmanlike dresser built from pine boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 17000.0,
		Cost = 60.0m,
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
				"Container_Dresser"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Chest-of-drawers style storage for folded garments, cloth, and small chamber goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSmallFootlocker() => new()
	{
		Metadata = new("medieval_domestic_small_footlocker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16203", []),
		Noun = "footlocker",
		ShortDescription = "a small footlocker",
		LongDescription = null,
		FullDescription = "This small footlocker is a medium-sized, workmanlike footlocker built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8200.0,
		Cost = 30.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact lidded footlocker for personal effects and chamber storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSmallPineCupboard() => new()
	{
		Metadata = new("medieval_domestic_small_pine_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16233", []),
		Noun = "cupboard",
		ShortDescription = "a small pine cupboard",
		LongDescription = null,
		FullDescription = "This small pine cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 11500.0,
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
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Enclosed household cupboard for pots, cloth, food packets, and everyday goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSmallTableSurface() => new()
	{
		Metadata = new("medieval_domestic_small_table_surface", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16263", []),
		Noun = "table",
		ShortDescription = "a small household table",
		LongDescription = null,
		FullDescription = "This small household table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 11000.0,
		Cost = 44.0m,
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
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small table surface for domestic containers, cups, lamps, and personal effects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSpiceCupboard() => new()
	{
		Metadata = new("medieval_domestic_spice_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16294", []),
		Noun = "cupboard",
		ShortDescription = "a cedar spice cupboard",
		LongDescription = null,
		FullDescription = "This cedar spice cupboard is a medium-sized, well-made cupboard built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7800.0,
		Cost = 62.0m,
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
		BuilderNotes = "Compact cupboard for spice jars, sachets, scented packets, and small dry stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSpiceDrawerChest() => new()
	{
		Metadata = new("medieval_domestic_spice_drawer_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16324", []),
		Noun = "chest",
		ShortDescription = "a spice drawer chest",
		LongDescription = null,
		FullDescription = "This spice drawer chest is a medium-sized, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 9500.0,
		Cost = 78.0m,
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
				"Container_Dresser"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small drawer chest for spice packets, remedies, scent sachets, and kitchen valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSpiceWallShelf() => new()
	{
		Metadata = new("medieval_domestic_spice_wall_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16354", []),
		Noun = "shelf",
		ShortDescription = "a spice wall shelf",
		LongDescription = null,
		FullDescription = "This spice wall shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
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
		BuilderNotes = "Small shelf for jars, packets, sachets, and aromatic domestic goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSpinningWoolBasket() => new()
	{
		Metadata = new("medieval_domestic_spinning_wool_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16385", []),
		Noun = "basket",
		ShortDescription = "a spinning-wool basket",
		LongDescription = null,
		FullDescription = "This spinning-wool basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 9.0m,
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
		BuilderNotes = "Open basket for wool, flax, thread, and domestic fibre work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticTableDrawerBox() => new()
	{
		Metadata = new("medieval_domestic_table_drawer_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16416", []),
		Noun = "drawer",
		ShortDescription = "a table drawer box",
		LongDescription = null,
		FullDescription = "This table drawer box is a medium-sized, workmanlike drawer built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4000.0,
		Cost = 28.0m,
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
				"Container_Desk_Drawers"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Drawer box for a table, suited to knives, wax, notes, and small domestic tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticTableLinenBox() => new()
	{
		Metadata = new("medieval_domestic_table_linen_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16446", []),
		Noun = "box",
		ShortDescription = "a table-linen box",
		LongDescription = null,
		FullDescription = "This table-linen box is a large, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000.0,
		Cost = 52.0m,
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
		BuilderNotes = "Long box for tablecloths, napery, folded towels, and serving cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticThreadAndLaceBox() => new()
	{
		Metadata = new("medieval_domestic_thread_and_lace_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16476", []),
		Noun = "box",
		ShortDescription = "a thread-and-lace box",
		LongDescription = null,
		FullDescription = "This thread-and-lace box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500.0,
		Cost = 14.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small box for thread, lace, trim, and textile mending goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticTowelCupboard() => new()
	{
		Metadata = new("medieval_domestic_towel_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16506", []),
		Noun = "cupboard",
		ShortDescription = "a towel cupboard",
		LongDescription = null,
		FullDescription = "This towel cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 11200.0,
		Cost = 42.0m,
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
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple cupboard for towels, washing cloths, and chamber linens.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticToyBasket() => new()
	{
		Metadata = new("medieval_domestic_toy_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16536", []),
		Noun = "basket",
		ShortDescription = "a toy basket",
		LongDescription = null,
		FullDescription = "This toy basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 8.0m,
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
		BuilderNotes = "Open basket for toys, balls, counters, and children's household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticToyChest() => new()
	{
		Metadata = new("medieval_domestic_toy_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:16567", []),
		Noun = "chest",
		ShortDescription = "a small toy chest",
		LongDescription = null,
		FullDescription = "This small toy chest is a medium-sized, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7000.0,
		Cost = 24.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low chest for toys, game pieces, teaching tablets, and children's goods.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
