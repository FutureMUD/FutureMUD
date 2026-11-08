#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalReligiousStoneOfferingBasin() => new()
	{
		Metadata = new("medieval_religious_stone_offering_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13066", []),
		Noun = "basin",
		ShortDescription = "a stone offering basin",
		LongDescription = null,
		FullDescription = "This stone offering basin is a medium-sized, workmanlike basin cut from limestone. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9800.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "limestone",
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
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad solid-offering basin for dry offerings, flower heads, grains, salt, ash, or votive tokens.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousVotiveCandle() => new()
	{
		Metadata = new("medieval_religious_votive_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13100", []),
		Noun = "candle",
		ShortDescription = "a beeswax votive candle",
		LongDescription = null,
		FullDescription = "This beeswax votive candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Small timed candle for votive, memorial, or offering use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousWallDevotionalPanel() => new()
	{
		Metadata = new("medieval_religious_wall_devotional_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13131", []),
		Noun = "panel",
		ShortDescription = "a wall devotional panel",
		LongDescription = null,
		FullDescription = "This wall devotional panel is a medium-sized, workmanlike panel built from pine boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3200.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
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
		BuilderNotes = "Wall-hung devotional panel suitable for painted, carved, inscribed, or skinned religious imagery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousWallShrineShelf() => new()
	{
		Metadata = new("medieval_religious_wall_shrine_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13161", []),
		Noun = "shelf",
		ShortDescription = "a wall shrine shelf",
		LongDescription = null,
		FullDescription = "This wall shrine shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2300.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
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
				"Container_Wall_Shelf"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow wall shelf intended to hold lamps, offerings, icons, tablets, or small religious vessels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousWaterEwer() => new()
	{
		Metadata = new("medieval_religious_water_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13194", []),
		Noun = "ewer",
		ShortDescription = "a ritual water ewer",
		LongDescription = null,
		FullDescription = "This ritual water ewer is a small, workmanlike ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1400.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Pouring vessel for water, diluted wine, or lustral liquid in a religious setting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchAltarTable() => new()
	{
		Metadata = new("medieval_church_altar_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13226", []),
		Noun = "altar",
		ShortDescription = "an oak church altar",
		LongDescription = null,
		FullDescription = "This oak church altar is a large, well-made altar built from oak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 36000.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods"
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
		BuilderNotes = "Substantial altar table with surface behaviour for chalices, books, candles, or offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchAspersoryBucket() => new()
	{
		Metadata = new("medieval_church_aspersory_bucket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13260", []),
		Noun = "bucket",
		ShortDescription = "a bronze aspersory bucket",
		LongDescription = null,
		FullDescription = "This bronze aspersory bucket is a medium-sized, well-made bucket worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 60.0m,
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
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable holy-water vessel for sprinkling rites; no personal-wear or jewellery behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchBaptismalEwer() => new()
	{
		Metadata = new("medieval_church_baptismal_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13292", []),
		Noun = "ewer",
		ShortDescription = "a brass baptismal ewer",
		LongDescription = null,
		FullDescription = "This brass baptismal ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
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
		BuilderNotes = "Pouring vessel for baptismal or washing water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchBronzeThurible() => new()
	{
		Metadata = new("medieval_church_bronze_thurible", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13324", []),
		Noun = "thurible",
		ShortDescription = "a bronze thurible",
		LongDescription = null,
		FullDescription = "This bronze thurible is a small, well-made thurible worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 65.0m,
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
		BuilderNotes = "Swinging incense censer represented as a small solid-fuel burner, not as a wearable or jewellery item.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchCandlePricketStand() => new()
	{
		Metadata = new("medieval_church_candle_pricket_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13356", []),
		Noun = "stand",
		ShortDescription = "a candle-pricket stand",
		LongDescription = null,
		FullDescription = "This candle-pricket stand is a medium-sized, workmanlike stand worked from wrought iron. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5600.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Iron candle stand surface for setting tapers or votive lights; separate candles provide actual light.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchChoirStall() => new()
	{
		Metadata = new("medieval_church_choir_stall", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13389", []),
		Noun = "stall",
		ShortDescription = "a carved choir stall",
		LongDescription = null,
		FullDescription = "This carved choir stall is a large, well-made stall built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods"
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
		BuilderNotes = "Individual choir or clerical seat with carved timberwork; not an outfit or personal effect.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchEagleLectern() => new()
	{
		Metadata = new("medieval_church_eagle_lectern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13420", []),
		Noun = "lectern",
		ShortDescription = "a brass eagle lectern",
		LongDescription = null,
		FullDescription = "This brass eagle lectern is a large, well-made lectern worked from brass. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Recognisable church lectern form with a broad reading surface; the bird detail is decorative only.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchFuneralBier() => new()
	{
		Metadata = new("medieval_church_funeral_bier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13454", []),
		Noun = "bier",
		ShortDescription = "a plain funeral bier",
		LongDescription = null,
		FullDescription = "This plain funeral bier is a large, workmanlike bier built from oak boards. A long flat platform rests on carrying rails, with low side edges and a plain upper surface. The handles are smoothed where bearers grip them. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16000.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Funerary Goods",
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
		BuilderNotes = "Church bier or trestled surface for funerary display and procession preparation.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchGospelBookStand() => new()
	{
		Metadata = new("medieval_church_gospel_book_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13485", []),
		Noun = "stand",
		ShortDescription = "a gospel book stand",
		LongDescription = null,
		FullDescription = "This gospel book stand is a medium-sized, well-made stand built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods",
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
		BuilderNotes = "Portable stand for a large service book or gospel volume.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchHolyWaterStoup() => new()
	{
		Metadata = new("medieval_church_holy_water_stoup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13519", []),
		Noun = "stoup",
		ShortDescription = "a holy-water stoup",
		LongDescription = null,
		FullDescription = "This holy-water stoup is a medium-sized, well-made stoup cut from marble. A cupped basin sits on a sturdy bracket, with a thick rim around the water hollow. The inner surface is polished from repeated touch. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8600.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "marble",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Small stone holy-water vessel for a church entrance or chapel wall.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchIncenseBoat() => new()
	{
		Metadata = new("medieval_church_incense_boat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13551", []),
		Noun = "box",
		ShortDescription = "an incense boat box",
		LongDescription = null,
		FullDescription = "This incense boat box is a very small, well-made box worked from brass. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small lidded container for incense grains or aromatic resin beside a thurible.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchKneelingBench() => new()
	{
		Metadata = new("medieval_church_kneeling_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13583", []),
		Noun = "kneeler",
		ShortDescription = "a low kneeling bench",
		LongDescription = null,
		FullDescription = "This low kneeling bench is a medium-sized, workmanlike kneeler built from pine boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture",
				"Market / Religious Goods / Devotional Goods"
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
		BuilderNotes = "Low church kneeler or prayer bench for posture and room furnishing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchOakPew() => new()
	{
		Metadata = new("medieval_church_oak_pew", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13614", []),
		Noun = "pew",
		ShortDescription = "an oak church pew",
		LongDescription = null,
		FullDescription = "This oak church pew is a large, workmanlike pew built from oak boards. A long seat runs between upright ends, with a straight back board and a narrow ledge along the rear. The front edge is polished by use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30000.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
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
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long church seating form, using bench mechanics and a narrow surface for small placed objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchPulpit() => new()
	{
		Metadata = new("medieval_church_pulpit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13648", []),
		Noun = "pulpit",
		ShortDescription = "a carved wooden pulpit",
		LongDescription = null,
		FullDescription = "This carved wooden pulpit is a large, well-made pulpit built from oak boards. A raised speaking enclosure stands above a small base, with a front panel and a narrow ledge for a book. The upper rail is polished by hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 32000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods"
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
		BuilderNotes = "Raised preaching or reading furnishing with a sloped surface for notes or scripture.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchPyxCupboard() => new()
	{
		Metadata = new("medieval_church_pyx_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13681", []),
		Noun = "cupboard",
		ShortDescription = "a pyx cupboard",
		LongDescription = null,
		FullDescription = "This pyx cupboard is a medium-sized, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7600.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
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
		BuilderNotes = "Small church cupboard for pyxes, small vessels, folded cloths, and service goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchReliquaryDisplayCase() => new()
	{
		Metadata = new("medieval_church_reliquary_display_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13713", []),
		Noun = "case",
		ShortDescription = "a reliquary display case",
		LongDescription = null,
		FullDescription = "This reliquary display case is a medium-sized, well-made case made from glass. A small framed compartment sits behind a guarded front, with the edges finished more carefully than the back. The base is steady enough for display. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3200.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Display_Case"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Transparent church display case for relic containers or visually important sacred objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchRoodScreenPanel() => new()
	{
		Metadata = new("medieval_church_rood_screen_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13745", []),
		Noun = "screen",
		ShortDescription = "a carved rood-screen panel",
		LongDescription = null,
		FullDescription = "This carved rood-screen panel is a large, well-made screen built from oak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative dividing screen panel for a church interior; inert unless a later pass gives it doorway behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchSacristyCupboard() => new()
	{
		Metadata = new("medieval_church_sacristy_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13775", []),
		Noun = "cupboard",
		ShortDescription = "a sacristy cupboard",
		LongDescription = null,
		FullDescription = "This sacristy cupboard is a large, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
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
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large service cupboard for candles, incense, linens, books, vessels, and other sacristy stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchSanctuaryLamp() => new()
	{
		Metadata = new("medieval_church_sanctuary_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13807", []),
		Noun = "lamp",
		ShortDescription = "a hanging sanctuary lamp",
		LongDescription = null,
		FullDescription = "This hanging sanctuary lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1400.0,
		Cost = 70.0m,
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
		BuilderNotes = "Hanging liquid-fuel sanctuary lamp suited to a church or chapel.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchStoneBaptismalFont() => new()
	{
		Metadata = new("medieval_church_stone_baptismal_font", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13838", []),
		Noun = "font",
		ShortDescription = "a stone baptismal font",
		LongDescription = null,
		FullDescription = "This stone baptismal font is a large, workmanlike font cut from limestone. A cupped basin sits on a sturdy bracket, with a thick rim around the water hollow. The inner surface is polished from repeated touch. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 58000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "limestone",
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
		BuilderNotes = "Large water-holding font for baptisms or ritual washing; liquid-container behaviour only.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChurchWallCrucifix() => new()
	{
		Metadata = new("medieval_church_wall_crucifix", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13870", []),
		Noun = "crucifix",
		ShortDescription = "a wall-hung crucifix",
		LongDescription = null,
		FullDescription = "This wall-hung crucifix is a medium-sized, workmanlike crucifix built from oak boards. A carved cross shape carries a raised central figure, with small fixing marks on the back. The front is smoothed around the most handled edges. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 36.0m,
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
		BuilderNotes = "Wall-hung cruciform furnishing for churches, chapels, or domestic devotional rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingChurchOffertoryChest() => new()
	{
		Metadata = new("medieval_locking_church_offertory_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13900", []),
		Noun = "chest",
		ShortDescription = "a locked offertory chest",
		LongDescription = null,
		FullDescription = "This locked offertory chest is a large, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 32000.0,
		Cost = 300.0m,
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
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking offertory chest for coin, gifts, or parish stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingChurchReliquaryCasket() => new()
	{
		Metadata = new("medieval_locking_church_reliquary_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13930", []),
		Noun = "casket",
		ShortDescription = "a locked reliquary casket",
		LongDescription = null,
		FullDescription = "This locked reliquary casket is a small, well-made casket worked from bronze. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
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
		BuilderNotes = "Small lockable casket for church relic storage and display-adjacent use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingChurchSacristyStrongbox() => new()
	{
		Metadata = new("medieval_locking_church_sacristy_strongbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13960", []),
		Noun = "strongbox",
		ShortDescription = "a sacristy strongbox",
		LongDescription = null,
		FullDescription = "This sacristy strongbox is a medium-sized, well-made strongbox built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
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
		BuilderNotes = "Locking strongbox for a sacristy, vestry, or church office.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianAnalogionLectern() => new()
	{
		Metadata = new("medieval_eastern_christian_analogion_lectern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13990", []),
		Noun = "lectern",
		ShortDescription = "an angled analogion lectern",
		LongDescription = null,
		FullDescription = "This angled analogion lectern is a medium-sized, well-made lectern built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7600.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
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
		BuilderNotes = "Sloped lectern for service books or gospel texts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianBaptismalBasin() => new()
	{
		Metadata = new("medieval_eastern_christian_baptismal_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14024", []),
		Noun = "basin",
		ShortDescription = "a bronze baptismal basin",
		LongDescription = null,
		FullDescription = "This bronze baptismal basin is a medium-sized, well-made basin worked from bronze. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 92.0m,
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
				"LContainer_Drum"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Water-holding baptismal or washing basin for church use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianCandleSandTray() => new()
	{
		Metadata = new("medieval_eastern_christian_candle_sand_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14056", []),
		Noun = "tray",
		ShortDescription = "a candle-sand tray",
		LongDescription = null,
		FullDescription = "This candle-sand tray is a medium-sized, workmanlike tray worked from brass. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Its formal proportions give it a public, ritual, and institutional presence. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3300.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Tray for sand, tapers, or votive candles; actual light comes from separate candle items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianChainCenser() => new()
	{
		Metadata = new("medieval_eastern_christian_chain_censer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14089", []),
		Noun = "censer",
		ShortDescription = "a chain-hung censer",
		LongDescription = null,
		FullDescription = "This chain-hung censer is a small, well-made censer worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1250.0,
		Cost = 58.0m,
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
		BuilderNotes = "Suspended church censer for charcoal and incense.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianChantingBench() => new()
	{
		Metadata = new("medieval_eastern_christian_chanting_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14121", []),
		Noun = "bench",
		ShortDescription = "a chanting bench",
		LongDescription = null,
		FullDescription = "This chanting bench is a large, workmanlike bench built from beech boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
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
		BuilderNotes = "Two-place bench for singers or readers in a church interior.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianChrismCruet() => new()
	{
		Metadata = new("medieval_eastern_christian_chrism_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14155", []),
		Noun = "cruet",
		ShortDescription = "a glass chrism cruet",
		LongDescription = null,
		FullDescription = "This glass chrism cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 180.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Small oil container for chrism-like or perfumed ritual oil.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianGospelLectern() => new()
	{
		Metadata = new("medieval_eastern_christian_gospel_lectern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14187", []),
		Noun = "lectern",
		ShortDescription = "a gospel lectern",
		LongDescription = null,
		FullDescription = "This gospel lectern is a medium-sized, well-made lectern built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 6800.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
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
		BuilderNotes = "Reading surface for large sacred books and liturgical texts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianHangingLamp() => new()
	{
		Metadata = new("medieval_eastern_christian_hanging_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14221", []),
		Noun = "lamp",
		ShortDescription = "a hanging icon lamp",
		LongDescription = null,
		FullDescription = "This hanging icon lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1150.0,
		Cost = 62.0m,
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
		BuilderNotes = "Suspended liquid-fuel lamp suited to an icon or sanctuary area.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianIconStand() => new()
	{
		Metadata = new("medieval_eastern_christian_icon_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14252", []),
		Noun = "stand",
		ShortDescription = "a carved icon stand",
		LongDescription = null,
		FullDescription = "This carved icon stand is a medium-sized, well-made stand built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5600.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
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
		BuilderNotes = "Open display stand for one or more wall-sized icons or devotional panels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianIconostasisPanel() => new()
	{
		Metadata = new("medieval_eastern_christian_iconostasis_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14285", []),
		Noun = "panel",
		ShortDescription = "an iconostasis panel",
		LongDescription = null,
		FullDescription = "This iconostasis panel is a large, well-made panel built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 17000.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative screen panel used to define a sacred area, with skins able to supply exact painted imagery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianIncenseCasket() => new()
	{
		Metadata = new("medieval_eastern_christian_incense_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14315", []),
		Noun = "casket",
		ShortDescription = "an incense casket",
		LongDescription = null,
		FullDescription = "This incense casket is a small, well-made casket built from cedar boards. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Small dry container for incense, aromatic wood, resin, or grains.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianProthesisTable() => new()
	{
		Metadata = new("medieval_eastern_christian_prothesis_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14347", []),
		Noun = "table",
		ShortDescription = "a prothesis service table",
		LongDescription = null,
		FullDescription = "This prothesis service table is a large, well-made table built from cypress boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 21000.0,
		Cost = 120.0m,
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
		BuilderNotes = "Side service table with surface behaviour for vessels, cloths, bread, or ritual equipment.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianReliquaryIconCase() => new()
	{
		Metadata = new("medieval_eastern_christian_reliquary_icon_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14381", []),
		Noun = "case",
		ShortDescription = "a reliquary icon case",
		LongDescription = null,
		FullDescription = "This reliquary icon case is a medium-sized, well-made case made from glass. A small framed compartment sits behind a guarded front, with the edges finished more carefully than the back. The base is steady enough for display. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3400.0,
		Cost = 115.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Display_Case"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Display case for an icon panel, relic container, or sealed devotional object.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianTetrapodTable() => new()
	{
		Metadata = new("medieval_eastern_christian_tetrapod_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14413", []),
		Noun = "table",
		ShortDescription = "a tetrapod offering table",
		LongDescription = null,
		FullDescription = "This tetrapod offering table is a large, well-made table built from walnut boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 19000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
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
		BuilderNotes = "Four-legged service table for icons, books, candles, and offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianVigilLamp() => new()
	{
		Metadata = new("medieval_eastern_christian_vigil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14447", []),
		Noun = "lamp",
		ShortDescription = "a bronze vigil lamp",
		LongDescription = null,
		FullDescription = "This bronze vigil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
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
		BuilderNotes = "Small liquid-fuel vigil lamp for an icon stand, sanctuary corner, or shrine shelf.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEasternChristianWallIconPanel() => new()
	{
		Metadata = new("medieval_eastern_christian_wall_icon_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14478", []),
		Noun = "panel",
		ShortDescription = "a wall icon panel",
		LongDescription = null,
		FullDescription = "This wall icon panel is a medium-sized, workmanlike panel built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 34.0m,
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
		BuilderNotes = "Wall devotional panel for sacred imagery; not a personal charm or jewellery piece.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicAblutionBasin() => new()
	{
		Metadata = new("medieval_islamic_ablution_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14508", []),
		Noun = "basin",
		ShortDescription = "a ceramic ablution basin",
		LongDescription = null,
		FullDescription = "This ceramic ablution basin is a medium-sized, workmanlike basin formed from ceramic. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Water-holding basin for ritual washing before prayer.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicAblutionEwer() => new()
	{
		Metadata = new("medieval_islamic_ablution_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14540", []),
		Noun = "ewer",
		ShortDescription = "a brass ablution ewer",
		LongDescription = null,
		FullDescription = "This brass ablution ewer is a small, workmanlike ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Pouring vessel for ablution water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicBrassHangingLamp() => new()
	{
		Metadata = new("medieval_islamic_brass_hanging_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14572", []),
		Noun = "lamp",
		ShortDescription = "a brass mosque lamp",
		LongDescription = null,
		FullDescription = "This brass mosque lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 60.0m,
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
		BuilderNotes = "Metal liquid-fuel lamp for a mosque, madrasa, or prayer hall.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicFoldingBookStand() => new()
	{
		Metadata = new("medieval_islamic_folding_book_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14603", []),
		Noun = "stand",
		ShortDescription = "a folding book stand",
		LongDescription = null,
		FullDescription = "This folding book stand is a small, well-made stand built from boxwood. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "boxwood",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Wares",
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
		BuilderNotes = "Low sloped stand for scripture reading or study; furnishing rather than personal jewellery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicIncenseBurner() => new()
	{
		Metadata = new("medieval_islamic_incense_burner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14637", []),
		Noun = "burner",
		ShortDescription = "a brass incense burner",
		LongDescription = null,
		FullDescription = "This brass incense burner is a small, well-made burner worked from brass. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
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
		BuilderNotes = "Small burner for fragrant wood or incense in a religious or ceremonial hall.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicMadrasaTextShelves() => new()
	{
		Metadata = new("medieval_islamic_madrasa_text_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14669", []),
		Noun = "shelves",
		ShortDescription = "madrasa text shelves",
		LongDescription = null,
		FullDescription = "These madrasa text shelves are large, well-made shelves built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 17000.0,
		Cost = 78.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
		BuilderNotes = "Open shelves for religious study texts, legal works, and manuscript bundles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicMihrabPanel() => new()
	{
		Metadata = new("medieval_islamic_mihrab_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14703", []),
		Noun = "panel",
		ShortDescription = "a carved mihrab panel",
		LongDescription = null,
		FullDescription = "This carved mihrab panel is a large, well-made panel built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 14000.0,
		Cost = 115.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative niche-like direction panel for a mosque or prayer hall.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicMosqueGlassLamp() => new()
	{
		Metadata = new("medieval_islamic_mosque_glass_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14733", []),
		Noun = "lamp",
		ShortDescription = "a glass mosque lamp",
		LongDescription = null,
		FullDescription = "This glass mosque lamp is a small, well-made lamp made from glass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 75.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Hanging liquid-fuel mosque lamp with fragile glass construction.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicQiblaDirectionPanel() => new()
	{
		Metadata = new("medieval_islamic_qibla_direction_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14764", []),
		Noun = "panel",
		ShortDescription = "a qibla direction panel",
		LongDescription = null,
		FullDescription = "This qibla direction panel is a medium-sized, workmanlike panel built from cedar boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
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
		BuilderNotes = "Wall panel marking prayer direction without using figurative imagery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicQuranCabinet() => new()
	{
		Metadata = new("medieval_islamic_quran_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14794", []),
		Noun = "cabinet",
		ShortDescription = "a Qur'an cabinet",
		LongDescription = null,
		FullDescription = "This Qur'an cabinet is a large, well-made cabinet built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
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
		BuilderNotes = "Cabinet for bound or wrapped sacred texts, commentaries, and recitation books.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicReedMatRack() => new()
	{
		Metadata = new("medieval_islamic_reed_mat_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14827", []),
		Noun = "rack",
		ShortDescription = "a reed mat rack",
		LongDescription = null,
		FullDescription = "This reed mat rack is a large, workmanlike rack built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7000.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Religious Items",
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
		BuilderNotes = "Open rack for communal prayer mats without making the mats themselves personal gear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicSandalShelf() => new()
	{
		Metadata = new("medieval_islamic_sandal_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14859", []),
		Noun = "shelf",
		ShortDescription = "a mosque sandal shelf",
		LongDescription = null,
		FullDescription = "This mosque sandal shelf is a large, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9600.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Religious Items",
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
		BuilderNotes = "Open shelf for footwear at a prayer hall entrance.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicWaterStorageJar() => new()
	{
		Metadata = new("medieval_islamic_water_storage_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14891", []),
		Noun = "jar",
		ShortDescription = "an ablution water jar",
		LongDescription = null,
		FullDescription = "This ablution water jar is a large, workmanlike jar formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "terracotta",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Simple Wares"
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
		BuilderNotes = "Large ceramic liquid vessel for stored washing water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicWoodenMinbarSteps() => new()
	{
		Metadata = new("medieval_islamic_wooden_minbar_steps", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14923", []),
		Noun = "minbar",
		ShortDescription = "a carved wooden minbar",
		LongDescription = null,
		FullDescription = "This carved wooden minbar is a large, well-made minbar built from walnut boards. A raised speaking enclosure stands above a small base, with a front panel and a narrow ledge for a book. The upper rail is polished by hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 36000.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stepped mosque pulpit furnishing; no seat, container, or door mechanics are asserted.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingIslamicCharityLockbox() => new()
	{
		Metadata = new("medieval_locking_islamic_charity_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14953", []),
		Noun = "box",
		ShortDescription = "a charity lockbox",
		LongDescription = null,
		FullDescription = "This charity lockbox is a small, well-made box built from cedar boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 120.0m,
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
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lockable charity box for mosque or charitable-house settings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingIslamicQuranChest() => new()
	{
		Metadata = new("medieval_locking_islamic_quran_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:14983", []),
		Noun = "chest",
		ShortDescription = "a locked Qur'an chest",
		LongDescription = null,
		FullDescription = "This locked Qur'an chest is a large, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 260.0m,
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
		BuilderNotes = "Locking chest for wrapped books and religious texts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishBimahTable() => new()
	{
		Metadata = new("medieval_jewish_bimah_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15013", []),
		Noun = "table",
		ShortDescription = "a bimah reading table",
		LongDescription = null,
		FullDescription = "This bimah reading table is a large, well-made table built from oak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 100.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
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
		BuilderNotes = "Central reading table surface for scrolls, books, and service objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishBronzeLampstand() => new()
	{
		Metadata = new("medieval_jewish_bronze_lampstand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15047", []),
		Noun = "lampstand",
		ShortDescription = "a bronze seven-branched lampstand",
		LongDescription = null,
		FullDescription = "This bronze seven-branched lampstand is a medium-sized, well-made lampstand worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Recognisable ritual lampstand furnishing; actual light should come from separate candle or lamp items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishGenizahChest() => new()
	{
		Metadata = new("medieval_jewish_genizah_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15077", []),
		Noun = "chest",
		ShortDescription = "a genizah chest",
		LongDescription = null,
		FullDescription = "This genizah chest is a large, workmanlike chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18500.0,
		Cost = 65.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Funerary Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
		BuilderNotes = "Chest for worn religious papers, damaged texts, or ritually retired writing materials.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishMemorialCandle() => new()
	{
		Metadata = new("medieval_jewish_memorial_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15110", []),
		Noun = "candle",
		ShortDescription = "a memorial beeswax candle",
		LongDescription = null,
		FullDescription = "This memorial beeswax candle is a small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Long-burning candle for memorial or household religious observance.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishNerTamidLamp() => new()
	{
		Metadata = new("medieval_jewish_ner_tamid_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15141", []),
		Noun = "lamp",
		ShortDescription = "a ner tamid lamp",
		LongDescription = null,
		FullDescription = "This ner tamid lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 70.0m,
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
		BuilderNotes = "Hanging perpetual-style liquid-fuel lamp for a synagogue furnishing set.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishReadingLectern() => new()
	{
		Metadata = new("medieval_jewish_reading_lectern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15172", []),
		Noun = "lectern",
		ShortDescription = "a synagogue reading lectern",
		LongDescription = null,
		FullDescription = "This synagogue reading lectern is a medium-sized, well-made lectern built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7200.0,
		Cost = 68.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
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
		BuilderNotes = "Sloped reading surface for scrolls or books.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishScrollCabinet() => new()
	{
		Metadata = new("medieval_jewish_scroll_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15206", []),
		Noun = "cabinet",
		ShortDescription = "a Torah scroll cabinet",
		LongDescription = null,
		FullDescription = "This Torah scroll cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 120.0m,
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
		BuilderNotes = "Cabinet for scrolls, wrappings, and synagogue texts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishScrollRest() => new()
	{
		Metadata = new("medieval_jewish_scroll_rest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15239", []),
		Noun = "rest",
		ShortDescription = "a wooden scroll rest",
		LongDescription = null,
		FullDescription = "This wooden scroll rest is a small, well-made rest built from boxwood. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "boxwood",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Wares",
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
		BuilderNotes = "Small support for resting a scroll or rolled text during reading.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishSynagogueBench() => new()
	{
		Metadata = new("medieval_jewish_synagogue_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15273", []),
		Noun = "bench",
		ShortDescription = "a synagogue bench",
		LongDescription = null,
		FullDescription = "This synagogue bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
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
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long seating bench for a synagogue or teaching house.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishWallTabletPanel() => new()
	{
		Metadata = new("medieval_jewish_wall_tablet_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15307", []),
		Noun = "panel",
		ShortDescription = "a wall tablet panel",
		LongDescription = null,
		FullDescription = "This wall tablet panel is a medium-sized, workmanlike panel formed from stoneware. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3400.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wall panel for inscribed or symbolic religious display, with exact writing handled by skins or writing blocks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishWashingBasin() => new()
	{
		Metadata = new("medieval_jewish_washing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15337", []),
		Noun = "basin",
		ShortDescription = "a ceramic washing basin",
		LongDescription = null,
		FullDescription = "This ceramic washing basin is a medium-sized, workmanlike basin formed from ceramic. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5600.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
		BuilderNotes = "Liquid-holding basin for washing water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewishWashingEwer() => new()
	{
		Metadata = new("medieval_jewish_washing_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15369", []),
		Noun = "ewer",
		ShortDescription = "a brass washing ewer",
		LongDescription = null,
		FullDescription = "This brass washing ewer is a small, workmanlike ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1400.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Pouring vessel for handwashing or ritual washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingJewishTorahArkChest() => new()
	{
		Metadata = new("medieval_locking_jewish_torah_ark_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15401", []),
		Noun = "ark",
		ShortDescription = "a wooden Torah ark",
		LongDescription = null,
		FullDescription = "This wooden Torah ark is a large, well-made ark built from cedar boards. Small doors close the front of the case, with a raised threshold and careful trim around the opening. The interior is protected and formal. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 46000.0,
		Cost = 520.0m,
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
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large built-in-lock container profile for protected scroll storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingJewishTzedakahBox() => new()
	{
		Metadata = new("medieval_locking_jewish_tzedakah_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15431", []),
		Noun = "box",
		ShortDescription = "a tzedakah box",
		LongDescription = null,
		FullDescription = "This tzedakah box is a small, well-made box built from walnut boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 120.0m,
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
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small locking box for charity coin and offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduAbhishekaBasin() => new()
	{
		Metadata = new("medieval_hindu_abhisheka_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15461", []),
		Noun = "basin",
		ShortDescription = "a stone abhisheka basin",
		LongDescription = null,
		FullDescription = "This stone abhisheka basin is a medium-sized, well-made basin cut from soapstone. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8200.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "soapstone",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Liquid-holding basin for ceremonial pouring and washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduBrassOfferingTray() => new()
	{
		Metadata = new("medieval_hindu_brass_offering_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15493", []),
		Noun = "tray",
		ShortDescription = "a brass offering tray",
		LongDescription = null,
		FullDescription = "This brass offering tray is a medium-sized, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1600.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
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
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tray for flowers, lamps, powders, food offerings, or ritual vessels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduBronzeOilLamp() => new()
	{
		Metadata = new("medieval_hindu_bronze_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15527", []),
		Noun = "lamp",
		ShortDescription = "a bronze temple oil lamp",
		LongDescription = null,
		FullDescription = "This bronze temple oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1000.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
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
		BuilderNotes = "Liquid-fuel ritual lamp suitable for temple or household shrine use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduCarvedShrineScreen() => new()
	{
		Metadata = new("medieval_hindu_carved_shrine_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15558", []),
		Noun = "screen",
		ShortDescription = "a carved shrine screen",
		LongDescription = null,
		FullDescription = "This carved shrine screen is a large, well-made screen built from teak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 16000.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative screen panel for a temple niche, household shrine, or ritual alcove.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduFloorOfferingStand() => new()
	{
		Metadata = new("medieval_hindu_floor_offering_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15588", []),
		Noun = "stand",
		ShortDescription = "a low floor offering stand",
		LongDescription = null,
		FullDescription = "This low floor offering stand is a medium-sized, well-made stand built from sandalwood. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3800.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sandalwood",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Low floor stand for lamps, bowls, flower offerings, or small ritual goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduGarlandRack() => new()
	{
		Metadata = new("medieval_hindu_garland_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15621", []),
		Noun = "rack",
		ShortDescription = "a flower-garland rack",
		LongDescription = null,
		FullDescription = "This flower-garland rack is a medium-sized, workmanlike rack built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2200.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Simple Wares"
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
		BuilderNotes = "Rack or shelf for garlands, flower strings, and offering preparation.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduHouseholdShrineCupboard() => new()
	{
		Metadata = new("medieval_hindu_household_shrine_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15654", []),
		Noun = "cupboard",
		ShortDescription = "a household shrine cupboard",
		LongDescription = null,
		FullDescription = "This household shrine cupboard is a medium-sized, well-made cupboard built from teak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 9500.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
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
		BuilderNotes = "Small shrine cupboard for lamps, vessels, images, flowers, powders, or wrapped offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduImagePedestal() => new()
	{
		Metadata = new("medieval_hindu_image_pedestal", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15686", []),
		Noun = "pedestal",
		ShortDescription = "a carved image pedestal",
		LongDescription = null,
		FullDescription = "This carved image pedestal is a large, well-made pedestal built from teak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pedestal for a large sacred image or statue; the image itself is intentionally not specified.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduIncenseBurner() => new()
	{
		Metadata = new("medieval_hindu_incense_burner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15716", []),
		Noun = "burner",
		ShortDescription = "a brass incense burner",
		LongDescription = null,
		FullDescription = "This brass incense burner is a small, well-made burner worked from brass. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
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
		BuilderNotes = "Solid-fuel burner for incense and fragrant woods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduKalashaPot() => new()
	{
		Metadata = new("medieval_hindu_kalasha_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15748", []),
		Noun = "pot",
		ShortDescription = "a brass kalasha pot",
		LongDescription = null,
		FullDescription = "This brass kalasha pot is a small, well-made pot worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
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
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Ritual liquid vessel suitable for water, scented water, or festival use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduMultiWickLamp() => new()
	{
		Metadata = new("medieval_hindu_multi_wick_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15780", []),
		Noun = "lamp",
		ShortDescription = "a multi-wick brass lamp",
		LongDescription = null,
		FullDescription = "This multi-wick brass lamp is a medium-sized, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 85.0m,
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
		BuilderNotes = "Larger liquid-fuel lamp form for festival or altar lighting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduPujaShelf() => new()
	{
		Metadata = new("medieval_hindu_puja_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15811", []),
		Noun = "shelf",
		ShortDescription = "a carved puja shelf",
		LongDescription = null,
		FullDescription = "This carved puja shelf is a medium-sized, well-made shelf built from sandalwood. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sandalwood",
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
		BuilderNotes = "Wall shelf for lamps, offerings, images, flowers, or household shrine goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduPujaWaterEwer() => new()
	{
		Metadata = new("medieval_hindu_puja_water_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15844", []),
		Noun = "ewer",
		ShortDescription = "a puja water ewer",
		LongDescription = null,
		FullDescription = "This puja water ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1350.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
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
		BuilderNotes = "Pouring vessel for ritual water or scented liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduStoneOfferingBowl() => new()
	{
		Metadata = new("medieval_hindu_stone_offering_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15876", []),
		Noun = "bowl",
		ShortDescription = "a stone offering bowl",
		LongDescription = null,
		FullDescription = "This stone offering bowl is a small, workmanlike bowl cut from soapstone. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "soapstone",
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
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open bowl for rice, flowers, ash, pigment, or other solid offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduTempleAltarTable() => new()
	{
		Metadata = new("medieval_hindu_temple_altar_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15910", []),
		Noun = "altar",
		ShortDescription = "a carved temple altar",
		LongDescription = null,
		FullDescription = "This carved temple altar is a large, well-made altar built from teak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 30000.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
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
		BuilderNotes = "Temple service table surface for lamps, offerings, vessels, and display objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHinduTempleSupplyCupboard() => new()
	{
		Metadata = new("medieval_hindu_temple_supply_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15944", []),
		Noun = "cupboard",
		ShortDescription = "a temple supply cupboard",
		LongDescription = null,
		FullDescription = "This temple supply cupboard is a large, well-made cupboard built from teak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 25000.0,
		Cost = 115.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
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
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large storage cupboard for lamps, trays, cloths, oils, incense, and vessels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingHinduSandalwoodStoreBox() => new()
	{
		Metadata = new("medieval_locking_hindu_sandalwood_store_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:15976", []),
		Noun = "box",
		ShortDescription = "a sandalwood temple-store box",
		LongDescription = null,
		FullDescription = "This sandalwood temple-store box is a small, well-made box built from sandalwood. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sandalwood",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Small lockable box for fragrant powders, offerings, or temple stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingHinduTempleDonationChest() => new()
	{
		Metadata = new("medieval_locking_hindu_temple_donation_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16006", []),
		Noun = "chest",
		ShortDescription = "a locked temple donation chest",
		LongDescription = null,
		FullDescription = "This locked temple donation chest is a large, well-made chest built from teak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 36000.0,
		Cost = 340.0m,
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
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large locking donation chest for temple or shrine precincts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistAltarTable() => new()
	{
		Metadata = new("medieval_buddhist_altar_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16036", []),
		Noun = "altar",
		ShortDescription = "a lacquered altar table",
		LongDescription = null,
		FullDescription = "This lacquered altar table is a large, well-made altar built from walnut boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
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
		BuilderNotes = "Altar table for lamps, incense, offerings, scrolls, and display objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistIncenseBox() => new()
	{
		Metadata = new("medieval_buddhist_incense_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16070", []),
		Noun = "box",
		ShortDescription = "a sandalwood incense box",
		LongDescription = null,
		FullDescription = "This sandalwood incense box is a small, well-made box built from sandalwood. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sandalwood",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
		BuilderNotes = "Dry container for incense sticks, powdered incense, or fragrant wood chips.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistIncenseBurner() => new()
	{
		Metadata = new("medieval_buddhist_incense_burner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16102", []),
		Noun = "burner",
		ShortDescription = "a bronze incense burner",
		LongDescription = null,
		FullDescription = "This bronze incense burner is a small, well-made burner worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 46.0m,
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
		BuilderNotes = "Small burner for incense, charcoal, or fragrant wood.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistLotusOilLamp() => new()
	{
		Metadata = new("medieval_buddhist_lotus_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16134", []),
		Noun = "lamp",
		ShortDescription = "a lotus-shaped oil lamp",
		LongDescription = null,
		FullDescription = "This lotus-shaped oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1150.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
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
		BuilderNotes = "Liquid-fuel lamp for Buddhist altar or shrine lighting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistMeditationBench() => new()
	{
		Metadata = new("medieval_buddhist_meditation_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16165", []),
		Noun = "bench",
		ShortDescription = "a meditation bench",
		LongDescription = null,
		FullDescription = "This meditation bench is a medium-sized, workmanlike bench built from cypress boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
		BuilderNotes = "Low bench for seated or kneeling practice; a furnishing rather than an outfit or personal charm.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistMemorialLamp() => new()
	{
		Metadata = new("medieval_buddhist_memorial_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16196", []),
		Noun = "lamp",
		ShortDescription = "a memorial oil lamp",
		LongDescription = null,
		FullDescription = "This memorial oil lamp is a small, workmanlike lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 34.0m,
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
		BuilderNotes = "Small oil lamp for memorial, votive, or altar use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistMemorialTabletShelf() => new()
	{
		Metadata = new("medieval_buddhist_memorial_tablet_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:16227", []),
		Noun = "shelf",
		ShortDescription = "a memorial tablet shelf",
		LongDescription = null,
		FullDescription = "This memorial tablet shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 55.0m,
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
		BuilderNotes = "Open shelf for memorial tablets, small plaques, offerings, or lamps.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
