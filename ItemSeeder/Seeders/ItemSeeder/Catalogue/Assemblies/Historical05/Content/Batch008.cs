#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalBuddhistOfferingBowl() => new()
	{
		Metadata = new("medieval_buddhist_offering_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16260", []),
		Noun = "bowl",
		ShortDescription = "a porcelain offering bowl",
		LongDescription = null,
		FullDescription = "This porcelain offering bowl is a small, well-made bowl formed from porcelain. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "porcelain",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
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
		BuilderNotes = "Open bowl for rice, fruit, flowers, water cups represented as solid offerings, or other altar goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistOfferingTray() => new()
	{
		Metadata = new("medieval_buddhist_offering_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16294", []),
		Noun = "tray",
		ShortDescription = "a lacquered offering tray",
		LongDescription = null,
		FullDescription = "This lacquered offering tray is a medium-sized, well-made tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 950.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Flat tray for arranging flowers, incense, offerings, lamps, and ritual supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistSutraCabinet() => new()
	{
		Metadata = new("medieval_buddhist_sutra_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16328", []),
		Noun = "cabinet",
		ShortDescription = "a sutra cabinet",
		LongDescription = null,
		FullDescription = "This sutra cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
		BuilderNotes = "Cabinet for sutra scrolls, booklets, cases, and associated service materials.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistSutraLectern() => new()
	{
		Metadata = new("medieval_buddhist_sutra_lectern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16361", []),
		Noun = "lectern",
		ShortDescription = "a sutra reading lectern",
		LongDescription = null,
		FullDescription = "This sutra reading lectern is a medium-sized, workmanlike lectern built from split bamboo. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
		BuilderNotes = "Reading surface for sutras, commentaries, or ritual recitations.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingBuddhistReliquaryBox() => new()
	{
		Metadata = new("medieval_locking_buddhist_reliquary_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16395", []),
		Noun = "box",
		ShortDescription = "a stupa reliquary box",
		LongDescription = null,
		FullDescription = "This stupa reliquary box is a small, well-made box worked from bronze. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 220.0m,
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
		BuilderNotes = "Small bronze lockbox for reliquary or shrine storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingBuddhistSutraChest() => new()
	{
		Metadata = new("medieval_locking_buddhist_sutra_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16425", []),
		Noun = "chest",
		ShortDescription = "a locked sutra chest",
		LongDescription = null,
		FullDescription = "This locked sutra chest is a large, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 280.0m,
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
		BuilderNotes = "Locking chest for wrapped sutras, papers, or monastery texts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDaoistAltarLamp() => new()
	{
		Metadata = new("medieval_daoist_altar_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16455", []),
		Noun = "lamp",
		ShortDescription = "a Daoist altar lamp",
		LongDescription = null,
		FullDescription = "This Daoist altar lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 950.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Liquid-fuel lamp for altar or shrine lighting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDaoistAltarTable() => new()
	{
		Metadata = new("medieval_daoist_altar_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16486", []),
		Noun = "altar",
		ShortDescription = "a Daoist altar table",
		LongDescription = null,
		FullDescription = "This Daoist altar table is a large, well-made altar built from cypress boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 115.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
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
		BuilderNotes = "Altar table for lamps, incense, tablets, offerings, and ritual documents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDaoistBronzeIncenseUrn() => new()
	{
		Metadata = new("medieval_daoist_bronze_incense_urn", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16520", []),
		Noun = "urn",
		ShortDescription = "a bronze incense urn",
		LongDescription = null,
		FullDescription = "This bronze incense urn is a medium-sized, well-made urn worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 4800.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Freestanding incense urn represented as a solid-fuel burner.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDaoistOfferingTray() => new()
	{
		Metadata = new("medieval_daoist_offering_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16552", []),
		Noun = "tray",
		ShortDescription = "a lacquered offering tray",
		LongDescription = null,
		FullDescription = "This lacquered offering tray is a medium-sized, workmanlike tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
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
		BuilderNotes = "Tray for fruit, paper, incense, lamps, or dry ritual offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDaoistPaintedAltarScreen() => new()
	{
		Metadata = new("medieval_daoist_painted_altar_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16586", []),
		Noun = "screen",
		ShortDescription = "a painted altar screen",
		LongDescription = null,
		FullDescription = "This painted altar screen is a large, well-made screen built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 12000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Painted screen panel for a shrine or altar backdrop.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDaoistScriptureCabinet() => new()
	{
		Metadata = new("medieval_daoist_scripture_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16616", []),
		Noun = "cabinet",
		ShortDescription = "a Daoist scripture cabinet",
		LongDescription = null,
		FullDescription = "This Daoist scripture cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 19000.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
		BuilderNotes = "Cabinet for scriptures, registers, talisman papers, and ritual manuscripts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDaoistTabletShelf() => new()
	{
		Metadata = new("medieval_daoist_tablet_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16649", []),
		Noun = "shelf",
		ShortDescription = "a spirit-tablet shelf",
		LongDescription = null,
		FullDescription = "This spirit-tablet shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3200.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Open shelf for tablets, plaques, lamps, and offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDaoistTalismanPaperChest() => new()
	{
		Metadata = new("medieval_daoist_talisman_paper_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16682", []),
		Noun = "chest",
		ShortDescription = "a talisman-paper chest",
		LongDescription = null,
		FullDescription = "This talisman-paper chest is a medium-sized, well-made chest built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7600.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
		BuilderNotes = "Dry chest for paper talismans, ritual writing stock, scrolls, or folded documents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDaoistWaterEwer() => new()
	{
		Metadata = new("medieval_daoist_water_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16715", []),
		Noun = "ewer",
		ShortDescription = "a ritual water ewer",
		LongDescription = null,
		FullDescription = "This ritual water ewer is a small, well-made ewer worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Pouring vessel for ritual water or cleansing liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShintoKamidanaShelf() => new()
	{
		Metadata = new("medieval_shinto_kamidana_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16747", []),
		Noun = "shelf",
		ShortDescription = "a kamidana shrine shelf",
		LongDescription = null,
		FullDescription = "This kamidana shrine shelf is a medium-sized, well-made shelf built from cypress boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2200.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Raised shrine shelf for offerings, small vessels, lamps, or plaques.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShintoOfferingStand() => new()
	{
		Metadata = new("medieval_shinto_offering_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16780", []),
		Noun = "stand",
		ShortDescription = "a whitewood offering stand",
		LongDescription = null,
		FullDescription = "This whitewood offering stand is a medium-sized, well-made stand built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Temple Offerings",
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
		BuilderNotes = "Low offering stand for rice, salt, greenery, lamps, or vessels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShintoPurificationBasin() => new()
	{
		Metadata = new("medieval_shinto_purification_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16813", []),
		Noun = "basin",
		ShortDescription = "a stone purification basin",
		LongDescription = null,
		FullDescription = "This stone purification basin is a large, well-made basin cut from granite. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 65000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "granite",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Drum"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large liquid-holding basin for purification water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShintoSakeVesselStand() => new()
	{
		Metadata = new("medieval_shinto_sake_vessel_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16845", []),
		Noun = "stand",
		ShortDescription = "a sake-vessel stand",
		LongDescription = null,
		FullDescription = "This sake-vessel stand is a small, workmanlike stand built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Temple Offerings",
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
		BuilderNotes = "Small stand or tray for liquid vessels and shrine offerings; vessels remain separate items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShintoShrineDonationBox() => new()
	{
		Metadata = new("medieval_shinto_shrine_donation_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16878", []),
		Noun = "box",
		ShortDescription = "a slatted shrine donation box",
		LongDescription = null,
		FullDescription = "This slatted shrine donation box is a large, workmanlike box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Open-lidded or slatted donation box for offerings and coins without built-in lock mechanics.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShintoShrineScreenPanel() => new()
	{
		Metadata = new("medieval_shinto_shrine_screen_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16910", []),
		Noun = "screen",
		ShortDescription = "a pale shrine screen panel",
		LongDescription = null,
		FullDescription = "This pale shrine screen panel is a large, well-made screen built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 11000.0,
		Cost = 65.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain pale timber screen panel for marking a shrine area or ritual backdrop.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShintoStoneShrineLamp() => new()
	{
		Metadata = new("medieval_shinto_stone_shrine_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16940", []),
		Noun = "lamp",
		ShortDescription = "a stone shrine lamp",
		LongDescription = null,
		FullDescription = "This stone shrine lamp is a medium-sized, well-made lamp cut from granite. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 12000.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "granite",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stone-bodied liquid-fuel lamp for shrine approaches or altar areas.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShintoVotiveTabletRack() => new()
	{
		Metadata = new("medieval_shinto_votive_tablet_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16971", []),
		Noun = "rack",
		ShortDescription = "a votive tablet rack",
		LongDescription = null,
		FullDescription = "This votive tablet rack is a large, workmanlike rack built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Open display rack for hung or rested votive tablets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNorthernAncestorTabletShelf() => new()
	{
		Metadata = new("medieval_northern_ancestor_tablet_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:17004", []),
		Noun = "shelf",
		ShortDescription = "an ancestor tablet shelf",
		LongDescription = null,
		FullDescription = "This ancestor tablet shelf is a medium-sized, well-made shelf built from yew boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3200.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "yew",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
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
		BuilderNotes = "Shelf for ancestor plaques, small vessels, and memorial offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNorthernBlotOfferingBowl() => new()
	{
		Metadata = new("medieval_northern_blot_offering_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:17037", []),
		Noun = "bowl",
		ShortDescription = "a bronze offering bowl",
		LongDescription = null,
		FullDescription = "This bronze offering bowl is a small, workmanlike bowl worked from bronze. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Open bowl for grain, salt, bloodless offerings, ash, or symbolic ritual goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNorthernLibationBowl() => new()
	{
		Metadata = new("medieval_northern_libation_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:17071", []),
		Noun = "bowl",
		ShortDescription = "a stone libation bowl",
		LongDescription = null,
		FullDescription = "This stone libation bowl is a medium-sized, workmanlike bowl cut from sandstone. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7200.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sandstone",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Amphora_Sextarius"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Liquid-holding bowl for libations, washing, or poured offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNorthernSacredPostPanel() => new()
	{
		Metadata = new("medieval_northern_sacred_post_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:17104", []),
		Noun = "post",
		ShortDescription = "a carved sacred post",
		LongDescription = null,
		FullDescription = "This carved sacred post is a large, workmanlike post built from oak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15000.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large carved cult or ancestor post for a hall, grove edge, or household sacred corner.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNorthernShrineBench() => new()
	{
		Metadata = new("medieval_northern_shrine_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:17134", []),
		Noun = "bench",
		ShortDescription = "a shrine-hall bench",
		LongDescription = null,
		FullDescription = "This shrine-hall bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 19000.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
		BuilderNotes = "Simple two-place sacred-hall bench with ordinary bench behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCleanLinenDressingStock() => new()
	{
		Metadata = new("medieval_medical_clean_linen_dressing_stock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:12", []),
		Noun = "bundle",
		ShortDescription = "a bundle of clean linen strips",
		LongDescription = null,
		FullDescription = "A tight bundle of narrow linen strips is tied with plain cord. The cloth has been washed, dried, and folded with the ends aligned for quick tearing into dressings. It looks like stock for a healer rather than ordinary rag cloth.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Material Functions / Medical Craft Stock / Dressing Stock",
				"Functions / Medical Treatment / Bandage"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWoolWoundPackingStock() => new()
	{
		Metadata = new("medieval_medical_wool_wound_packing_stock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:40", []),
		Noun = "bundle",
		ShortDescription = "a bundle of clean wool packing",
		LongDescription = null,
		FullDescription = "This bundle holds soft locks of pale wool wrapped inside a linen cloth. The wool is combed loose and springy, suitable for pressure packing or padding heavier dressings. A faint sheepy smell remains under the cleaner cloth scent.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Material Functions / Medical Craft Stock / Dressing Stock",
				"Functions / Medical Treatment / Tend Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCottonDressingStack() => new()
	{
		Metadata = new("medieval_medical_cotton_dressing_stack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:68", []),
		Noun = "stack",
		ShortDescription = "a stack of cotton dressing squares",
		LongDescription = null,
		FullDescription = "Flat cotton squares are stacked into a neat packet and tied through the middle. The fabric is softer than plain linen and cut to an even size for covering grazes, burns, or salved wounds. The edges show a little fraying from knife-cut preparation.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140.0,
		Cost = 8.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Material Functions / Medical Craft Stock / Dressing Stock",
				"Functions / Medical Treatment / Bandage"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalHoneyedPoulticeClothStock() => new()
	{
		Metadata = new("medieval_medical_honeyed_poultice_cloth_stock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:96", []),
		Noun = "packet",
		ShortDescription = "a sticky packet of honeyed cloths",
		LongDescription = null,
		FullDescription = "Several folded cloths sit inside a waxed wrap, their fibres darkened by honey. The packet smells sweet and faintly floral, with enough tackiness that the folds pull apart reluctantly. It is clearly prepared as dressing stock rather than table linen.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 160.0,
		Cost = 12.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "honey",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Poultice Stock",
				"Functions / Medical Treatment / Antiseptic Dressing"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalVinegarCleaningClothBundle() => new()
	{
		Metadata = new("medieval_medical_vinegar_cleaning_cloth_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:124", []),
		Noun = "bundle",
		ShortDescription = "a bundle of vinegar-sharp cloths",
		LongDescription = null,
		FullDescription = "Small linen cloths are wrapped in a waxed outer rag to keep their sharp vinegar smell from fading. The cloth is damp but not dripping, and several corners are folded outward for quick grip. It is meant for wiping dirt, old blood, and crust from wounds.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 6.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugWillowBarkBundle() => new()
	{
		Metadata = new("medieval_drug_willow_bark_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:151", []),
		Noun = "bundle",
		ShortDescription = "a bundle of shaved willow bark",
		LongDescription = null,
		FullDescription = "Thin strips of willow bark are shaved into curling ribbons and tied with hemp thread. The inner bark shows pale tan fibres with a dry, faintly bitter smell. It is ready to steep, grind, or portion into simple medicine stock.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Decoction Stock",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugYarrowStypticBundle() => new()
	{
		Metadata = new("medieval_drug_yarrow_styptic_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:180", []),
		Noun = "bundle",
		ShortDescription = "a bundle of dried yarrow heads",
		LongDescription = null,
		FullDescription = "Small dried yarrow heads and feathered leaves are bundled with a thin linen tie. The herb has faded to dusty green and straw, but the bitter green scent is still present when handled. It is suitable for crushing into pads, powders, or styptic dressings.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60.0,
		Cost = 6.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "yarrow",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Styptic"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMandrakeRootPacket() => new()
	{
		Metadata = new("medieval_drug_mandrake_root_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:208", []),
		Noun = "packet",
		ShortDescription = "a packet of cut mandrake root",
		LongDescription = null,
		FullDescription = "Pieces of pale mandrake root lie in a small parchment packet closed with thread. The roots are irregular, tough, and earthy, with a sharp bitter odour that clings to the paper. A red warning knot marks the packet as something to measure carefully.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 45.0,
		Cost = 24.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "mandrake",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Material Functions / Medical Craft Stock / Decoction Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugHenbaneLeafPacket() => new()
	{
		Metadata = new("medieval_drug_henbane_leaf_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:236", []),
		Noun = "packet",
		ShortDescription = "a packet of dried henbane leaves",
		LongDescription = null,
		FullDescription = "Dried henbane leaves are packed into a narrow paper fold, dark and brittle against the pale wrapping. The packet smells musty and acrid, and a black tie marks it apart from ordinary kitchen herbs. It is prepared for smoke or fumigation rather than cooking.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 35.0,
		Cost = 22.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "henbane",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Material Functions / Medical Craft Stock / Fumigation Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugEphedraTwigPacket() => new()
	{
		Metadata = new("medieval_drug_ephedra_twig_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:264", []),
		Noun = "packet",
		ShortDescription = "a packet of dried ephedra twigs",
		LongDescription = null,
		FullDescription = "Jointed green-brown twigs are bundled inside a small linen packet. The pieces are trimmed to a length that can be measured out by handful or pinch. The smell is dry, grassy, and faintly resinous.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 50.0,
		Cost = 14.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "ephedra",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Material Functions / Medical Craft Stock / Decoction Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugFoxgloveLeafPacket() => new()
	{
		Metadata = new("medieval_drug_foxglove_leaf_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:292", []),
		Noun = "packet",
		ShortDescription = "a warning-tied foxglove packet",
		LongDescription = null,
		FullDescription = "Several dark, crinkled foxglove leaves sit in a waxed paper packet bound with a red thread. The packet is small and carefully folded, as if meant for apothecary hands rather than common kitchen use. The dry leaves have a dusty, green-bitter smell.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 30.0,
		Cost = 28.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "foxglove",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugAlumStypticPowderPacket() => new()
	{
		Metadata = new("medieval_drug_alum_styptic_powder_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:320", []),
		Noun = "packet",
		ShortDescription = "a packet of pale alum powder",
		LongDescription = null,
		FullDescription = "A small parchment packet holds pale mineral powder that clumps slightly where it has drawn damp from the air. The fold is tight and clean, with a faint chalky dust along the crease. It is sized for careful pinches rather than bulk trade.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 40.0,
		Cost = 10.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "alum",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Styptic"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugPoppyLatexResinCake() => new()
	{
		Metadata = new("medieval_drug_poppy_latex_resin_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:347", []),
		Noun = "cake",
		ShortDescription = "a dark poppy-latex resin cake",
		LongDescription = null,
		FullDescription = "This small resinous cake is wrapped in oiled cloth, dark brown and tacky at the edges. It has a heavy bitter smell and has been pressed into a flat disk for shaving tiny amounts. The careful wrapping suggests valued medicine rather than ordinary spice.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 35.0,
		Cost = 40.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "resin",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugGarlicSalveHerbPacket() => new()
	{
		Metadata = new("medieval_drug_garlic_salve_herb_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:375", []),
		Noun = "packet",
		ShortDescription = "a pungent garlic-salve packet",
		LongDescription = null,
		FullDescription = "Crushed garlic and dry salve herbs are folded into a thick linen packet. The smell is strong, sharp, and savoury enough to cut through the beeswax around it. It is plainly a healer’s stock packet, not a kitchen bundle.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "garlic",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Salve Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMintGingerDecoctionPacket() => new()
	{
		Metadata = new("medieval_drug_mint_ginger_decoction_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:403", []),
		Noun = "packet",
		ShortDescription = "a mint-and-ginger decoction packet",
		LongDescription = null,
		FullDescription = "Dried mint leaves and sliced ginger are mixed in a neat paper packet. The packet smells bright, warming, and sweet-sharp, with the ingredients visible through the loose fold at the top. It is ready to steep into a small pot of tonic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 8.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "mint",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Decoction Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugTheriacSpiceElectuaryStock() => new()
	{
		Metadata = new("medieval_drug_theriac_spice_electuary_stock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:431", []),
		Noun = "jar",
		ShortDescription = "a jar of theriac spice stock",
		LongDescription = null,
		FullDescription = "A small glazed jar contains a dark, sticky blend of honey, spice, and powdered medicine stock. The lid is cloth-capped and sealed around the rim with beeswax. Its smell is rich, bitter, sweet, and resinous all at once.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 220.0,
		Cost = 70.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "spice",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Antidote"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalGutSutureHank() => new()
	{
		Metadata = new("medieval_medical_gut_suture_hank", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:459", []),
		Noun = "hank",
		ShortDescription = "a hank of cleaned gut suture",
		LongDescription = null,
		FullDescription = "Cleaned gut thread is wound into a careful hank and tied to a small bone toggle. The strands are pale, slightly translucent, and uneven in thickness, but twisted firmly enough for coarse surgical use. A linen wrap keeps the thread from tangling.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 10.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "gut",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Functions / Material Functions / Medical Craft Stock / Suture Stock",
				"Functions / Medical Treatment / Suture Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWillowSplintBlankBundle() => new()
	{
		Metadata = new("medieval_medical_willow_splint_blank_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:488", []),
		Noun = "bundle",
		ShortDescription = "a bundle of willow splint blanks",
		LongDescription = null,
		FullDescription = "Straight pieces of willow are shaved smooth and tied into a narrow bundle. Each strip is drilled near the ends for future cord ties, but none have padding yet. They are raw stock for fracture splints rather than finished treatment gear.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 8.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Material Functions / Medical Craft Stock / Splint Stock",
				"Functions / Medical Treatment / Splint"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalRoughLinenBandage() => new()
	{
		Metadata = new("medieval_medical_rough_linen_bandage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:518", []),
		Noun = "bandage",
		ShortDescription = "a rough linen bandage",
		LongDescription = null,
		FullDescription = "This strip of linen has been torn rather than cut, leaving frayed edges and a few stubborn creases. It is serviceable enough for emergency binding but plainly not fine infirmary stock. Small knots in the weave make it feel coarse under the fingers.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 55.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Simple Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Poor"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCleanLinenBandage() => new()
	{
		Metadata = new("medieval_medical_clean_linen_bandage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:547", []),
		Noun = "bandage",
		ShortDescription = "a clean linen bandage",
		LongDescription = null,
		FullDescription = "A narrow length of clean linen is rolled around itself into a compact bandage. The cloth is plain, pale, and closely woven, with enough length for wrapping an arm, hand, or smaller wound. Its loose end is tucked neatly under the roll.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Simple"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalNarrowLinenBandageRoll() => new()
	{
		Metadata = new("medieval_medical_narrow_linen_bandage_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:575", []),
		Noun = "roll",
		ShortDescription = "a narrow linen bandage roll",
		LongDescription = null,
		FullDescription = "This long, narrow bandage is rolled tight around a small reed core. The linen has been cut evenly and pressed flat, with only a few loose threads along one edge. It is suited to controlled wrapping rather than hurried field tearing.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWoolPressureCompress() => new()
	{
		Metadata = new("medieval_medical_wool_pressure_compress", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:603", []),
		Noun = "compress",
		ShortDescription = "a dense wool pressure compress",
		LongDescription = null,
		FullDescription = "Clean wool is packed into a thick pad and held inside a simple linen cover. The compress is springy but firm, made to be pressed hard over a bleeding or bruised place. Its corners are tied down so the wool does not spill loose.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 130.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCottonWoundCompress() => new()
	{
		Metadata = new("medieval_medical_cotton_wound_compress", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:631", []),
		Noun = "compress",
		ShortDescription = "a soft cotton wound compress",
		LongDescription = null,
		FullDescription = "Several layers of cotton are folded into a soft square and lightly stitched at the edges. The pad is clean, pale, and more delicate than a wool compress, with enough body to absorb without feeling bulky. It has the look of careful apothecary preparation.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 95.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalSilkWoundDressing() => new()
	{
		Metadata = new("medieval_medical_silk_wound_dressing", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:659", []),
		Noun = "dressing",
		ShortDescription = "a fine silk wound dressing",
		LongDescription = null,
		FullDescription = "Fine silk is folded around a small inner pad and tied with two narrow threads. The fabric is smooth, light, and closely woven, making the dressing seem more courtly than field-made. It is compact enough to tuck into a physician’s case.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Great,
		WeightInGrams = 50.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Great"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalHoneyedLinenDressing() => new()
	{
		Metadata = new("medieval_medical_honeyed_linen_dressing", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:688", []),
		Noun = "dressing",
		ShortDescription = "a honeyed linen wound dressing",
		LongDescription = null,
		FullDescription = "This folded dressing is darkened with honey and pressed into a tidy, sticky pad. A waxed wrap keeps it from smearing the healer’s pouch, and the sweet smell is obvious as soon as it is handled. The linen underneath remains visible at the corners.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Antiseptic Dressing",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Great",
				"TopicalCream_Honey_Poultice"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalVinegarWoundCloth() => new()
	{
		Metadata = new("medieval_medical_vinegar_wound_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:720", []),
		Noun = "cloth",
		ShortDescription = "a vinegar-soaked wound cloth",
		LongDescription = null,
		FullDescription = "A small square of linen is damp with sharp vinegar and folded inside a waxed scrap. The cloth is meant for wiping rather than wrapping, with one corner left dry enough to grip. It smells clean in a sour, practical way.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Clean_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWineRinsedWoundCloth() => new()
	{
		Metadata = new("medieval_medical_wine_rinsed_wound_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:748", []),
		Noun = "cloth",
		ShortDescription = "a wine-rinsed wound cloth",
		LongDescription = null,
		FullDescription = "This folded cloth is stained faint rose-brown from watered wine. It is wrapped in plain linen and smells of sour fruit and old cask, not perfume. The preparation is tidy enough for a travelling healer’s roll.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 50.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Clean_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalYarrowStypticPad() => new()
	{
		Metadata = new("medieval_medical_yarrow_styptic_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:776", []),
		Noun = "pad",
		ShortDescription = "a yarrow-stuffed styptic pad",
		LongDescription = null,
		FullDescription = "Crushed yarrow is packed between two small linen faces, making a green-bitter pad meant to be pressed down hard. Flecks of herb show through the weave near the seams. A simple thread tie keeps the pad from opening in a pouch.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "yarrow",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Yarrow_Styptic"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalAlumStypticPad() => new()
	{
		Metadata = new("medieval_medical_alum_styptic_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:808", []),
		Noun = "pad",
		ShortDescription = "an alum-dusted styptic pad",
		LongDescription = null,
		FullDescription = "A small folded pad has been dusted with pale alum powder and wrapped to keep the mineral from spilling. The cloth feels dry and chalky through the outer fold. It is made for quick pressure on a cut rather than broad bandaging.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 50.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "alum",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Bandage"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Alum_Styptic"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalAloeBurnDressing() => new()
	{
		Metadata = new("medieval_medical_aloe_burn_dressing", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:839", []),
		Noun = "dressing",
		ShortDescription = "an aloe-slick burn dressing",
		LongDescription = null,
		FullDescription = "This dressing is folded around a cool, slick herbal salve and sealed in waxed cloth. The surface has a greenish stain where the medicine has soaked into the linen. It is shaped for laying flat over a burned patch of skin.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 85.0,
		Cost = 15.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Aloe_Burn_Salve"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalHerbalBurnDressing() => new()
	{
		Metadata = new("medieval_medical_herbal_burn_dressing", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:870", []),
		Noun = "dressing",
		ShortDescription = "a green herbal burn dressing",
		LongDescription = null,
		FullDescription = "A neat linen pad is filled with a soft green salve and folded into a waxed wrapper. Its smell is grassy, bitter, and faintly resinous. The cloth is broad enough to cover a palm-sized burn without being bulky.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "herb",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Herbal_Burn_Salve"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalGarlicSalveCloth() => new()
	{
		Metadata = new("medieval_medical_garlic_salve_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:901", []),
		Noun = "cloth",
		ShortDescription = "a garlic-salve wound cloth",
		LongDescription = null,
		FullDescription = "This small wound cloth has been smeared with a pungent garlic salve and folded under a beeswaxed cover. The smell is strong enough to announce it before the packet is opened. It is intended for laying over a small wound rather than wrapping a limb.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "garlic",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Garlic_Salve"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWoundPackingWad() => new()
	{
		Metadata = new("medieval_medical_wound_packing_wad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:930", []),
		Noun = "wad",
		ShortDescription = "a linen wound-packing wad",
		LongDescription = null,
		FullDescription = "Strips of linen are folded and twisted into a soft wad that can be pushed into an awkward wound. The cloth is plain and unmedicated, with the fibres left loose enough to swell when damp. It is wrapped in another scrap to keep it clean.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalFieldBandagePacket() => new()
	{
		Metadata = new("medieval_medical_field_bandage_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:958", []),
		Noun = "packet",
		ShortDescription = "a field bandage packet",
		LongDescription = null,
		FullDescription = "A rough leather tie holds together a bandage, a small compress, and two short binding cords. The packet is compact and plain, made to fit a belt pouch or saddlebag. Dust marks on the outer cloth suggest hard campaign use.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Tend Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Simple",
				"Tend_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalMonasticDressingPacket() => new()
	{
		Metadata = new("medieval_medical_monastic_dressing_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:988", []),
		Noun = "packet",
		ShortDescription = "a monastic dressing packet",
		LongDescription = null,
		FullDescription = "Several careful dressings are folded inside a plain white linen cover marked with a small cross in brown thread. The packet is orderly rather than ornamental, with each pad stacked squarely. It smells faintly of honey, vinegar, and stored cloth.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 170.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Standard Medicine",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Wound Cleaning"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"Clean_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalApothecaryFoldedDressing() => new()
	{
		Metadata = new("medieval_medical_apothecary_folded_dressing", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1019", []),
		Noun = "dressing",
		ShortDescription = "an apothecary folded dressing",
		LongDescription = null,
		FullDescription = "This dressing is folded into a crisp square and held by a small paper band. Herbal flecks and a faint resin smell mark it as something prepared behind an apothecary counter. The cloth is clean, even, and easy to open one-handed.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 75.0,
		Cost = 13.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Honey_Poultice"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalHorseWoundBandage() => new()
	{
		Metadata = new("medieval_medical_horse_wound_bandage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1050", []),
		Noun = "bandage",
		ShortDescription = "a broad horse wound bandage",
		LongDescription = null,
		FullDescription = "A broad strip of strong linen is rolled around a stout wooden pin. It is wider and heavier than human dressings, with enough length to bind a leg or flank on a large animal. The cloth is plain but tough, made for stable use.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalPlasteredPoulticePad() => new()
	{
		Metadata = new("medieval_medical_plastered_poultice_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1078", []),
		Noun = "pad",
		ShortDescription = "a plastered herbal poultice pad",
		LongDescription = null,
		FullDescription = "A thick pad of cloth is plastered with a green-brown herbal paste and folded in on itself. The paste has dried enough not to drip, but it remains pliant under the fingers. The smell is earthy, sour, and medicinal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "herb",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single",
				"TopicalCream_Honey_Poultice"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalLinenSuturePacket() => new()
	{
		Metadata = new("medieval_medical_linen_suture_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1109", []),
		Noun = "packet",
		ShortDescription = "a packet of linen sutures",
		LongDescription = null,
		FullDescription = "Waxed linen thread is wound onto a small slip of wood and tucked inside a folded cloth. The thread is coarse but clean, with a needle-hole awl mark beside it for quick threading. It is a simple single-use surgical supply.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalSilkSutureCard() => new()
	{
		Metadata = new("medieval_medical_silk_suture_card", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1137", []),
		Noun = "card",
		ShortDescription = "a card of fine silk sutures",
		LongDescription = null,
		FullDescription = "Fine silk thread is wrapped around a thin parchment card and secured with a careful cross-tie. The thread is smoother and more regular than ordinary linen suture, with a soft sheen in the light. A small needle is tucked under the last loop.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalGutSutureSpool() => new()
	{
		Metadata = new("medieval_medical_gut_suture_spool", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1166", []),
		Noun = "spool",
		ShortDescription = "a small gut suture spool",
		LongDescription = null,
		FullDescription = "Clean gut thread is wound around a little bone spool with care. The strand is pale, strong-looking, and slightly uneven, the sort of material used when a healer wants thread that belongs in the body rather than ordinary stitching. A linen cover protects the spool.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 40.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gut",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBronzeSutureNeedle() => new()
	{
		Metadata = new("medieval_medical_bronze_suture_needle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1194", []),
		Noun = "needle",
		ShortDescription = "a curved bronze suture needle",
		LongDescription = null,
		FullDescription = "This small bronze needle has a smooth curve, a drilled eye, and a polished point. It is thicker than a seamstress needle and shaped for gripping with forceps or fingers. The metal has darkened slightly where it rests in its cloth wrap.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit",
				"Functions / Tools / Surgical Tools / Surgical Suture Needle"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Suture_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBoneSutureNeedle() => new()
	{
		Metadata = new("medieval_medical_bone_suture_needle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1223", []),
		Noun = "needle",
		ShortDescription = "a polished bone suture needle",
		LongDescription = null,
		FullDescription = "A small curved needle has been carved from bone and rubbed smooth. The eye is broad and the point is serviceable rather than delicate, suited to coarse stitching. It rests inside a narrow scrap of leather to protect the tip.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit",
				"Functions / Tools / Surgical Tools / Surgical Suture Needle"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCauteryDressingPad() => new()
	{
		Metadata = new("medieval_medical_cautery_dressing_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1252", []),
		Noun = "pad",
		ShortDescription = "a thick cautery dressing pad",
		LongDescription = null,
		FullDescription = "This heavy pad is built from wool wrapped in linen, with darkened scorch marks along one face. It is meant to follow hot-iron work, not to serve as an ordinary bandage. The bulk and smoky smell make its purpose visible.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWoundCleaningKit() => new()
	{
		Metadata = new("medieval_medical_wound_cleaning_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1282", []),
		Noun = "kit",
		ShortDescription = "a wound-cleaning kit",
		LongDescription = null,
		FullDescription = "This compact kit holds folded cloths, a blunt scraper, and a small spatula inside a leather roll. The pieces are plain but tidy, meant for removing dirt and old blood before heavier treatment begins. The roll ties closed with two waxed cords.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Clean_Kit",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalVinegarWashKit() => new()
	{
		Metadata = new("medieval_medical_vinegar_wash_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1311", []),
		Noun = "kit",
		ShortDescription = "a vinegar wound-wash kit",
		LongDescription = null,
		FullDescription = "A leather pouch holds waxed cloths, wiping swabs, and space for a small sour-smelling wash vessel. Its outer flap is stained where damp cloth has rested against it. The kit is practical and compact, designed for repeated cleaning rather than display.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Clean_Kit"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalHoneyVinegarAntisepticKit() => new()
	{
		Metadata = new("medieval_medical_honey_vinegar_antiseptic_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1339", []),
		Noun = "kit",
		ShortDescription = "a honey-and-vinegar dressing kit",
		LongDescription = null,
		FullDescription = "This kit gathers honeyed pads, vinegar cloths, and a little wooden spatula in a waxed leather wrap. The mixture of sweet and sharp smells is unmistakable. Everything is arranged to clean a wound and then cover it quickly.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Antiseptic Dressing",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Antiseptic_Kit",
				"TopicalCream_Honey_Poultice"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalPlainTendingRoll() => new()
	{
		Metadata = new("medieval_medical_plain_tending_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1370", []),
		Noun = "roll",
		ShortDescription = "a plain wound-tending roll",
		LongDescription = null,
		FullDescription = "A roll of linen contains compresses, strips, packing cloths, and a few simple ties. It is not a surgical set, but it gives a healer enough supplies for routine tending. The outside is marked by use, with darker patches at the fold lines.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 540.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Kit",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalTravelTendingRoll() => new()
	{
		Metadata = new("medieval_medical_travel_tending_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1399", []),
		Noun = "roll",
		ShortDescription = "a travel healer’s tending roll",
		LongDescription = null,
		FullDescription = "This leather-backed roll opens to reveal folded cloths, narrow bandages, a scraping spatula, and several waxed packets. It is built to travel, with loops that keep every piece from falling out on the road. The outer hide is scuffed from saddlebag use.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Wound Cleaning"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Kit",
				"Clean_Kit"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalSuturingKit() => new()
	{
		Metadata = new("medieval_medical_suturing_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1429", []),
		Noun = "kit",
		ShortDescription = "a compact suturing kit",
		LongDescription = null,
		FullDescription = "A small leather roll holds thread, needles, a bronze probe, and tiny wiping cloths. The needles sit in a stiffened flap and the thread is wound on bone and wood spools. It is plain but complete enough for ordinary stitching work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Suture_Kit",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalFineSuturingCase() => new()
	{
		Metadata = new("medieval_medical_fine_suturing_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1458", []),
		Noun = "case",
		ShortDescription = "a fine suturing case",
		LongDescription = null,
		FullDescription = "This narrow case is covered in dark leather and fitted with small loops for silk, gut, and linen sutures. Bronze and bone needles sit separately so their points do not rub dull. It is a careful surgical supply, made for an infirmary or court physician.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Suture_Kit_Good",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalFieldSurgeonRoll() => new()
	{
		Metadata = new("medieval_medical_field_surgeon_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1488", []),
		Noun = "roll",
		ShortDescription = "a field surgeon’s leather roll",
		LongDescription = null,
		FullDescription = "Heavy leather folds around bandages, sutures, a probe, and small metal tools. The roll is stained and repaired, but the straps still hold the contents firmly. It is meant to be opened beside a wounded person rather than displayed on a shelf.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Suture Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"FieldMedkit",
				"Suture_Kit"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBattlefieldMedkit() => new()
	{
		Metadata = new("medieval_medical_battlefield_medkit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1519", []),
		Noun = "kit",
		ShortDescription = "a battlefield medicine kit",
		LongDescription = null,
		FullDescription = "This stout shoulder kit contains broad bandages, packed compresses, splints, sutures, and several waxed remedy packets. The leather is scuffed and darkened by travel, with a wide strap for carrying across a camp. It is built for trauma rather than household sickness.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Wound Cleaning"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"FieldMedkit",
				"Clean_Kit",
				"Tend_Kit"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalInfirmaryChest() => new()
	{
		Metadata = new("medieval_medical_infirmary_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1551", []),
		Noun = "chest",
		ShortDescription = "a stocked infirmary medicine chest",
		LongDescription = null,
		FullDescription = "This wooden chest is divided into small compartments for bandages, thread, salve pots, and medicine vessels. The lid is plain but sturdy, with scratches from repeated opening. It is heavy enough to belong in an infirmary more than on a traveller’s belt.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 9500.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Standard Medicine",
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Surgical Supplies",
				"Market / Medicine / Apothecary Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"FieldMedkit",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalMonasticInfirmaryChest() => new()
	{
		Metadata = new("medieval_medical_monastic_infirmary_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1582", []),
		Noun = "chest",
		ShortDescription = "a monastic infirmary chest",
		LongDescription = null,
		FullDescription = "A pale wooden chest holds ordered rows of cloth, simple salves, suture spools, and labelled packets. A modest cross is cut into the lid rather than painted brightly. The whole piece is careful, clean, and practical, suited to a monastery sickroom.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 11000.0,
		Cost = 300.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Surgical Supplies",
				"Market / Medicine / Herbal Medicine"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"FieldMedkit",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugApothecaryRemedyCase() => new()
	{
		Metadata = new("medieval_drug_apothecary_remedy_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1613", []),
		Noun = "case",
		ShortDescription = "an apothecary remedy case",
		LongDescription = null,
		FullDescription = "This case opens into small cells for vials, paper packets, pellets, and salve pots. The inner dividers are stained by herbs and resin, and the leather outer shell has a careful flap closure. It is meant for prepared remedies rather than surgery.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1350.0,
		Cost = 96.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCaravanMedicineRoll() => new()
	{
		Metadata = new("medieval_medical_caravan_medicine_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1642", []),
		Noun = "roll",
		ShortDescription = "a caravan medicine roll",
		LongDescription = null,
		FullDescription = "A long leather roll carries compact dressings, bitter herb packets, waxed salves, and a few empty vessels. The ties are reinforced for travel, and the outside smells of dust, horse, and old spice. It is a practical kit for long roads and poor access to town healers.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Kit",
				"Clean_Kit",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalShipboardWoundChest() => new()
	{
		Metadata = new("medieval_medical_shipboard_wound_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1673", []),
		Noun = "chest",
		ShortDescription = "a shipboard wound chest",
		LongDescription = null,
		FullDescription = "This small chest is tar-darkened at the seams and packed with cloth rolls, cord ties, salve pots, and scraping tools. The wood is heavy for its size, made to survive damp decks. Salt marks show on the iron fittings.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7200.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Surgical Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"FieldMedkit",
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalHorsePhysickKit() => new()
	{
		Metadata = new("medieval_medical_horse_physick_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1702", []),
		Noun = "kit",
		ShortDescription = "a horse physick kit",
		LongDescription = null,
		FullDescription = "Broad dressings, strong ties, hoof cloths, and large cleaning pads are packed inside this worn leather kit. It is scaled for animals rather than people, with stout thread and heavy linen. Stable smells of leather, vinegar, and horsehair cling to it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Kit",
				"Bandage_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBurnCareBox() => new()
	{
		Metadata = new("medieval_medical_burn_care_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1731", []),
		Noun = "box",
		ShortDescription = "a burn-care dressing box",
		LongDescription = null,
		FullDescription = "This small wooden box carries slick salve cloths, soft pads, and waxed packets for covering burned skin. The contents smell of honey, green herbs, and beeswax. Its compartments are shallow so the dressings do not crush each other.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1550.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Tend_Kit",
				"TopicalCream_Herbal_Burn_Salve"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalStypticBox() => new()
	{
		Metadata = new("medieval_medical_styptic_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1762", []),
		Noun = "box",
		ShortDescription = "a styptic powder box",
		LongDescription = null,
		FullDescription = "A palm-sized wooden box contains folded pads, yarrow bundles, and pale mineral powder packets. The lid fits tightly to keep the contents dry. Everything inside is arranged for small cuts, shaving wounds, and quick pressure rather than broad nursing.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"TopicalCream_Yarrow_Styptic"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalFractureCareBundle() => new()
	{
		Metadata = new("medieval_medical_fracture_care_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1791", []),
		Noun = "bundle",
		ShortDescription = "a fracture-care splint bundle",
		LongDescription = null,
		FullDescription = "Straight wooden splints, linen pads, and tie cords are bundled in a heavy cloth sleeve. The pieces are pre-smoothed and ready to bind around a limb after setting. It is bulky but far faster than carving splints after the injury.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Limb_Immobilising"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCuppingKit() => new()
	{
		Metadata = new("medieval_medical_cupping_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1820", []),
		Noun = "kit",
		ShortDescription = "a small cupping kit",
		LongDescription = null,
		FullDescription = "Several small cups, a scraping cloth, and a simple lamp plate are wrapped inside a padded pouch. The cups are visible through gaps in the cloth, their rims polished smooth from repeated use. It is a healer’s tool kit, though it carries no automatic treatment of its own.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Medical Tools / Cupping Vessel"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCauteryKit() => new()
	{
		Metadata = new("medieval_medical_cautery_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1849", []),
		Noun = "kit",
		ShortDescription = "a wrapped cautery kit",
		LongDescription = null,
		FullDescription = "A narrow iron, thick pads, and a little charcoal packet are secured in a heavy leather wrap. Dark scorch marks stain the inner layer where the hot tool has been set down before. The kit looks harsh, deliberate, and practical.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Cautery Iron"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalLinenMedicineStrainer() => new()
	{
		Metadata = new("medieval_medical_linen_medicine_strainer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1880", []),
		Noun = "strainer",
		ShortDescription = "a linen medicine strainer",
		LongDescription = null,
		FullDescription = "Tight linen is stretched over a small willow hoop and stitched in place. The weave is close enough to catch grit, resin flecks, and steeped herbs from medicine liquids. The hoop has a small cord loop for hanging beside a hearth or apothecary bench.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Herbal Medicine",
				"Functions / Tools / Medical Tools / Medicine Strainer"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalStoneMortarAndPestle() => new()
	{
		Metadata = new("medieval_medical_stone_mortar_and_pestle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1908", []),
		Noun = "mortar",
		ShortDescription = "a stone mortar and pestle",
		LongDescription = null,
		FullDescription = "This compact stone mortar is paired with a blunt pestle worn smooth by grinding. Its bowl is darkened by old herbs and mineral powders, with a few chips along the rim. It is heavy enough to stay steady while crushing medicine stock.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1450.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Apothecary Goods",
				"Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCeramicApothecaryMortar() => new()
	{
		Metadata = new("medieval_medical_ceramic_apothecary_mortar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1936", []),
		Noun = "mortar",
		ShortDescription = "a glazed ceramic apothecary mortar",
		LongDescription = null,
		FullDescription = "This small glazed mortar has a pale bowl and a matching pestle with a rounded end. Herbal stains have settled into hairline scratches inside the glaze. It is cleaner and finer than rough stone, suited to an apothecary counter.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glazed ceramic",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Apothecary Goods",
				"Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBronzeOintmentSpatula() => new()
	{
		Metadata = new("medieval_medical_bronze_ointment_spatula", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1964", []),
		Noun = "spatula",
		ShortDescription = "a bronze ointment spatula",
		LongDescription = null,
		FullDescription = "A slim bronze spatula has one flattened end for lifting salve and one rounded end for stirring. The handle is narrow, plain, and slightly dark from age. It is small enough to live inside a medicine roll.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Apothecary Goods",
				"Functions / Tools / Medical Tools / Ointment Spatula"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBoneOintmentSpatula() => new()
	{
		Metadata = new("medieval_medical_bone_ointment_spatula", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:1992", []),
		Noun = "spatula",
		ShortDescription = "a polished bone ointment spatula",
		LongDescription = null,
		FullDescription = "This spatula is carved from pale bone and rubbed smooth along its rounded blade. A shallow groove down the handle gives the fingers purchase when working with slick salves. It is light, cheap, and easy to clean by scraping.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Apothecary Goods",
				"Functions / Tools / Medical Tools / Ointment Spatula"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalHornDosingSpoon() => new()
	{
		Metadata = new("medieval_medical_horn_dosing_spoon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2020", []),
		Noun = "spoon",
		ShortDescription = "a small horn dosing spoon",
		LongDescription = null,
		FullDescription = "A little spoon of polished horn has a shallow bowl and a short handle. The rim is smoothed thin enough for careful pouring, but not marked with exact measures. Its dark translucent colour gives it a humble apothecary look.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalGlassCuppingVessel() => new()
	{
		Metadata = new("medieval_medical_glass_cupping_vessel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2047", []),
		Noun = "cup",
		ShortDescription = "a small glass cupping vessel",
		LongDescription = null,
		FullDescription = "This rounded glass cup has a thick lip and a slightly smoky tint. The base is broad enough to sit upright, while the mouth is polished smooth. It is fragile but clear enough to show the skin beneath when pressed down.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Surgical Supplies",
				"Functions / Tools / Medical Tools / Cupping Vessel"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalClayCuppingCup() => new()
	{
		Metadata = new("medieval_medical_clay_cupping_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2075", []),
		Noun = "cup",
		ShortDescription = "a clay cupping cup",
		LongDescription = null,
		FullDescription = "A small fired-clay cup has a rounded belly, a thick rim, and soot darkening near the base. It is opaque and sturdier than glass, with a rougher finish. A healer could warm it quickly over a flame.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fired clay",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Surgical Supplies",
				"Functions / Tools / Medical Tools / Cupping Vessel"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};
}
