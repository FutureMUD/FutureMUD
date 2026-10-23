#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalLightNorthernHornStableLantern() => new()
	{
		Metadata = new("medieval_light_northern_horn_stable_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10240", []),
		Noun = "lantern",
		ShortDescription = "a horn stable lantern",
		LongDescription = null,
		FullDescription = "This horn stable lantern is a small, workmanlike lantern worked from horn. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 880.0,
		Cost = 26.0m,
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
		BuilderNotes = "Practical horn-panel lantern for barns, stables, byres, and winter passages.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightNorthernIronCressetBasket() => new()
	{
		Metadata = new("medieval_light_northern_iron_cresset_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10269", []),
		Noun = "cresset",
		ShortDescription = "an iron cresset basket",
		LongDescription = null,
		FullDescription = "This iron cresset basket is a medium-sized, workmanlike cresset worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3400.0,
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
		BuilderNotes = "Open iron cresset basket for fuel and night work in cold or windy places.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightNorthernLongHempRushlight() => new()
	{
		Metadata = new("medieval_light_northern_long_hemp_rushlight", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10298", []),
		Noun = "rushlight",
		ShortDescription = "a long hemp rushlight",
		LongDescription = null,
		FullDescription = "This long hemp rushlight is a tiny, workmanlike rushlight made from woven hemp. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34.0,
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
				"Candle_Long"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Dim slow-burning rushlight for watchful nights and small chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightNorthernOakHearthLampStand() => new()
	{
		Metadata = new("medieval_light_northern_oak_hearth_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10327", []),
		Noun = "stand",
		ShortDescription = "an oak hearth-lamp stand",
		LongDescription = null,
		FullDescription = "This oak hearth-lamp stand is a medium-sized, workmanlike stand built from oak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4800.0,
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
		BuilderNotes = "Stout wooden stand intended to keep a lamp or candle dish near the hearth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightNorthernPinePitchBrand() => new()
	{
		Metadata = new("medieval_light_northern_pine_pitch_brand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10356", []),
		Noun = "brand",
		ShortDescription = "a pine pitch brand",
		LongDescription = null,
		FullDescription = "This pine pitch brand is a small, workmanlike brand built from pine boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 460.0,
		Cost = 4.0m,
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
				"Torch_2Hour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Resin-rich pine brand suited to cold yards, tracks, and timber buildings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightNorthernSoapstoneGreaseLamp() => new()
	{
		Metadata = new("medieval_light_northern_soapstone_grease_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10385", []),
		Noun = "lamp",
		ShortDescription = "a soapstone grease lamp",
		LongDescription = null,
		FullDescription = "This soapstone grease lamp is a small, workmanlike lamp cut from soapstone. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "soapstone",
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
		BuilderNotes = "Dense stone lamp suited to northern households and workrooms, with a shallow fuel reservoir.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatWesternChamberBrazier() => new()
	{
		Metadata = new("medieval_heat_western_chamber_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10414", []),
		Noun = "brazier",
		ShortDescription = "a chamber warming brazier",
		LongDescription = null,
		FullDescription = "This chamber warming brazier is a medium-sized, well-made brazier worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 4700.0,
		Cost = 72.0m,
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
		BuilderNotes = "Well-made warming brazier for a private chamber or better lodging.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWesternChamberCandlestick() => new()
	{
		Metadata = new("medieval_light_western_chamber_candlestick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10443", []),
		Noun = "candlestick",
		ShortDescription = "a brass chamber candlestick",
		LongDescription = null,
		FullDescription = "This brass chamber candlestick is a small, well-made candlestick worked from brass. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 26.0m,
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
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished candlestick with a broad base for chamber or table use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWesternFeastCandle() => new()
	{
		Metadata = new("medieval_light_western_feast_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10472", []),
		Noun = "candle",
		ShortDescription = "a beeswax feast candle",
		LongDescription = null,
		FullDescription = "This beeswax feast candle is a small, well-made candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 240.0,
		Cost = 8.0m,
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
		BuilderNotes = "Large bright candle suitable for a feast board, guild table, or high chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWesternGlazedChamberLamp() => new()
	{
		Metadata = new("medieval_light_western_glazed_chamber_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10501", []),
		Noun = "lamp",
		ShortDescription = "a glazed chamber lamp",
		LongDescription = null,
		FullDescription = "This glazed chamber lamp is a very small, well-made lamp formed from glazed ceramic. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 410.0,
		Cost = 10.0m,
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
		BuilderNotes = "Refined glazed oil lamp for a chamber, writing room, or table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWesternHallLantern() => new()
	{
		Metadata = new("medieval_light_western_hall_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10530", []),
		Noun = "lantern",
		ShortDescription = "a bronze hall lantern",
		LongDescription = null,
		FullDescription = "This bronze hall lantern is a medium-sized, well-made lantern worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 68.0m,
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
		BuilderNotes = "Large bronze lantern with a formal, well-finished frame for a hall or manor chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWesternIronCresset() => new()
	{
		Metadata = new("medieval_light_western_iron_cresset", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10559", []),
		Noun = "cresset",
		ShortDescription = "a wrought-iron cresset",
		LongDescription = null,
		FullDescription = "This wrought-iron cresset is a medium-sized, workmanlike cresset worked from wrought iron. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3300.0,
		Cost = 28.0m,
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
		BuilderNotes = "Iron cresset intended for strong flame in a hall, yard, or street-front setting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightWesternOakTorchStand() => new()
	{
		Metadata = new("medieval_light_western_oak_torch_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10588", []),
		Noun = "stand",
		ShortDescription = "an oak hall torch stand",
		LongDescription = null,
		FullDescription = "This oak hall torch stand is a medium-sized, workmanlike stand built from oak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5600.0,
		Cost = 30.0m,
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
		BuilderNotes = "Standing timber support for a torch, brand, or lantern in a hall.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatMediterraneanLowBronzeBrazier() => new()
	{
		Metadata = new("medieval_heat_mediterranean_low_bronze_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10617", []),
		Noun = "brazier",
		ShortDescription = "a low bronze brazier",
		LongDescription = null,
		FullDescription = "This low bronze brazier is a medium-sized, well-made brazier worked from bronze. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 4300.0,
		Cost = 74.0m,
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
		BuilderNotes = "Low bronze brazier for a courtyard, reception room, or shaded interior.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightMediterraneanBrassPiercedLantern() => new()
	{
		Metadata = new("medieval_light_mediterranean_brass_pierced_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10646", []),
		Noun = "lantern",
		ShortDescription = "a pierced brass lantern",
		LongDescription = null,
		FullDescription = "This pierced brass lantern is a small, well-made lantern worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1150.0,
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
		BuilderNotes = "Brass lantern with pierced openings that shield and pattern the light.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightMediterraneanBronzeHangingLamp() => new()
	{
		Metadata = new("medieval_light_mediterranean_bronze_hanging_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10675", []),
		Noun = "lamp",
		ShortDescription = "a bronze hanging oil lamp",
		LongDescription = null,
		FullDescription = "This bronze hanging oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050.0,
		Cost = 34.0m,
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
		BuilderNotes = "Suspended bronze lamp with small loops and a rounded fuel body.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightMediterraneanCeramicSaucerLamp() => new()
	{
		Metadata = new("medieval_light_mediterranean_ceramic_saucer_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10704", []),
		Noun = "lamp",
		ShortDescription = "a ceramic saucer lamp",
		LongDescription = null,
		FullDescription = "This ceramic saucer lamp is a very small, workmanlike lamp formed from ceramic. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
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
		BuilderNotes = "Small saucer-shaped lamp for a niche, table, or outdoor step.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightMediterraneanClayWindowLamp() => new()
	{
		Metadata = new("medieval_light_mediterranean_clay_window_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10733", []),
		Noun = "lamp",
		ShortDescription = "a clay window lamp",
		LongDescription = null,
		FullDescription = "This clay window lamp is a small, workmanlike lamp formed from fired clay. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 430.0,
		Cost = 5.0m,
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
		BuilderNotes = "Compact oil lamp shaped to sit on a sill, niche, or wall recess.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightMediterraneanMarbleLampStand() => new()
	{
		Metadata = new("medieval_light_mediterranean_marble_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10762", []),
		Noun = "stand",
		ShortDescription = "a marble lamp stand",
		LongDescription = null,
		FullDescription = "This marble lamp stand is a medium-sized, well-made stand cut from marble. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 14500.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "marble",
		Tags = [
				"Functions / Household Items / Household Lighting",
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
		BuilderNotes = "Heavy stone stand for a lamp or candle dish, decorative but still movable with effort.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightMediterraneanOlivewoodTorch() => new()
	{
		Metadata = new("medieval_light_mediterranean_olivewood_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10791", []),
		Noun = "torch",
		ShortDescription = "an olivewood hand torch",
		LongDescription = null,
		FullDescription = "This olivewood hand torch is a small, workmanlike torch built from olive wood. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 590.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "olive wood",
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
		BuilderNotes = "Regional hardwood torch for outdoor courtyards, lanes, and walls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightMediterraneanTerracottaLamp() => new()
	{
		Metadata = new("medieval_light_mediterranean_terracotta_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10820", []),
		Noun = "lamp",
		ShortDescription = "a terracotta wick lamp",
		LongDescription = null,
		FullDescription = "This terracotta wick lamp is a very small, workmanlike lamp formed from terracotta. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 4.0m,
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
		BuilderNotes = "Low reddish lamp with a wick spout and oil chamber, suited to warm-climate interiors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatIslamicateCharcoalScentBrazier() => new()
	{
		Metadata = new("medieval_heat_islamicate_charcoal_scent_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10849", []),
		Noun = "brazier",
		ShortDescription = "a charcoal scent brazier",
		LongDescription = null,
		FullDescription = "This charcoal scent brazier is a small, well-made brazier worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1850.0,
		Cost = 44.0m,
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
		BuilderNotes = "Vented brazier for small charcoal charges and household scenting, not a religious censer.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightIslamicateBrassCresset() => new()
	{
		Metadata = new("medieval_light_islamicate_brass_cresset", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10878", []),
		Noun = "cresset",
		ShortDescription = "a brass cresset lamp",
		LongDescription = null,
		FullDescription = "This brass cresset lamp is a medium-sized, well-made cresset worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 52.0m,
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
		BuilderNotes = "Open metal lamp form with a broad fuel body and decorative pierced work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightIslamicateBronzeChainLamp() => new()
	{
		Metadata = new("medieval_light_islamicate_bronze_chain_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10907", []),
		Noun = "lamp",
		ShortDescription = "a bronze chain-hung lamp",
		LongDescription = null,
		FullDescription = "This bronze chain-hung lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1380.0,
		Cost = 46.0m,
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
		BuilderNotes = "Bronze lamp body set up for hanging by short chains from a bracket or beam.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightIslamicateCedarLampShelf() => new()
	{
		Metadata = new("medieval_light_islamicate_cedar_lamp_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10936", []),
		Noun = "shelf",
		ShortDescription = "a carved cedar lamp shelf",
		LongDescription = null,
		FullDescription = "This carved cedar lamp shelf is a small, well-made shelf built from cedar boards. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 980.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Lighting",
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
		BuilderNotes = "Carved wooden shelf for holding a small lamp or incense-safe vessel; it is only a support surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightIslamicateCourtyardLantern() => new()
	{
		Metadata = new("medieval_light_islamicate_courtyard_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10965", []),
		Noun = "lantern",
		ShortDescription = "a courtyard oil lantern",
		LongDescription = null,
		FullDescription = "This courtyard oil lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1450.0,
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
		BuilderNotes = "Rugged shielded lantern for a courtyard, passage, roof terrace, or shopfront.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightIslamicateGlazedOilLamp() => new()
	{
		Metadata = new("medieval_light_islamicate_glazed_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:10994", []),
		Noun = "lamp",
		ShortDescription = "a glazed ceramic oil lamp",
		LongDescription = null,
		FullDescription = "This glazed ceramic oil lamp is a very small, well-made lamp formed from glazed ceramic. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 390.0,
		Cost = 11.0m,
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
		BuilderNotes = "Glazed oil lamp with a smooth reservoir and narrowed wick channel.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightIslamicateLongNeckLamp() => new()
	{
		Metadata = new("medieval_light_islamicate_long_neck_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11023", []),
		Noun = "lamp",
		ShortDescription = "a long-necked oil lamp",
		LongDescription = null,
		FullDescription = "This long-necked oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 930.0,
		Cost = 36.0m,
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
		BuilderNotes = "Metal lamp with a drawn-out neck and handle for careful pouring and lighting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightIslamicatePiercedBrassLantern() => new()
	{
		Metadata = new("medieval_light_islamicate_pierced_brass_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11052", []),
		Noun = "lantern",
		ShortDescription = "a pierced brass courtyard lantern",
		LongDescription = null,
		FullDescription = "This pierced brass courtyard lantern is a small, well-made lantern worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1220.0,
		Cost = 48.0m,
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
		BuilderNotes = "Urban brass lantern with patterned piercings and a protected flame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatSouthAsianBrassCharcoalBrazier() => new()
	{
		Metadata = new("medieval_heat_south_asian_brass_charcoal_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11081", []),
		Noun = "brazier",
		ShortDescription = "a brass charcoal brazier",
		LongDescription = null,
		FullDescription = "This brass charcoal brazier is a medium-sized, well-made brazier worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 60.0m,
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
		BuilderNotes = "Polished brass brazier sized for a modest charcoal charge.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSouthAsianBambooCourtyardTorch() => new()
	{
		Metadata = new("medieval_light_south_asian_bamboo_courtyard_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11110", []),
		Noun = "torch",
		ShortDescription = "a bamboo courtyard torch",
		LongDescription = null,
		FullDescription = "This bamboo courtyard torch is a small, workmanlike torch built from split bamboo. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
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
		BuilderNotes = "Light bamboo-shafted torch for courtyards, lanes, and short outdoor tasks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSouthAsianBrassCupLamp() => new()
	{
		Metadata = new("medieval_light_south_asian_brass_cup_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11139", []),
		Noun = "lamp",
		ShortDescription = "a brass cup oil lamp",
		LongDescription = null,
		FullDescription = "This brass cup oil lamp is a very small, workmanlike lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 12.0m,
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
		BuilderNotes = "Small brass cup lamp for a shelf, threshold, courtyard, or household niche.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSouthAsianBronzeStandingLamp() => new()
	{
		Metadata = new("medieval_light_south_asian_bronze_standing_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11168", []),
		Noun = "lamp",
		ShortDescription = "a bronze standing oil lamp",
		LongDescription = null,
		FullDescription = "This bronze standing oil lamp is a medium-sized, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 86.0m,
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
		BuilderNotes = "Tall bronze oil lamp with a raised stem and small reservoir cups.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSouthAsianBronzeWallLamp() => new()
	{
		Metadata = new("medieval_light_south_asian_bronze_wall_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11197", []),
		Noun = "sconce",
		ShortDescription = "a bronze wall-lamp bracket",
		LongDescription = null,
		FullDescription = "This bronze wall-lamp bracket is a small, well-made sconce worked from bronze. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 34.0m,
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
				"Container_Wall_Shelf"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bronze wall bracket shaped to support a small lamp or candle cup.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSouthAsianGlassLantern() => new()
	{
		Metadata = new("medieval_light_south_asian_glass_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11226", []),
		Noun = "lantern",
		ShortDescription = "a glass-panel lantern",
		LongDescription = null,
		FullDescription = "This glass-panel lantern is a small, well-made lantern made from glass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1150.0,
		Cost = 44.0m,
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
		BuilderNotes = "Glass-panel lantern with a protected flame and hand-carry frame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSouthAsianTeakLampStand() => new()
	{
		Metadata = new("medieval_light_south_asian_teak_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11255", []),
		Noun = "stand",
		ShortDescription = "a teak lamp stand",
		LongDescription = null,
		FullDescription = "This teak lamp stand is a medium-sized, well-made stand built from teak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3900.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
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
		BuilderNotes = "Carved teak stand for a lamp, bowl, or candle dish.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSouthAsianTerracottaDiyaLamp() => new()
	{
		Metadata = new("medieval_light_south_asian_terracotta_diya_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11284", []),
		Noun = "lamp",
		ShortDescription = "a terracotta cup lamp",
		LongDescription = null,
		FullDescription = "This terracotta cup lamp is a tiny, workmanlike lamp formed from terracotta. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 2.0m,
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
		BuilderNotes = "Tiny terracotta oil cup with a pinched wick lip.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatEastAsianCastIronBrazier() => new()
	{
		Metadata = new("medieval_heat_east_asian_cast_iron_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11313", []),
		Noun = "brazier",
		ShortDescription = "a cast-iron brazier",
		LongDescription = null,
		FullDescription = "This cast-iron brazier is a medium-sized, well-made brazier worked from cast iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 6200.0,
		Cost = 72.0m,
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
		BuilderNotes = "Compact iron brazier suited to a chamber, tea room, workroom, or winter interior.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightEastAsianBambooHandTorch() => new()
	{
		Metadata = new("medieval_light_east_asian_bamboo_hand_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11342", []),
		Noun = "torch",
		ShortDescription = "a bamboo hand torch",
		LongDescription = null,
		FullDescription = "This bamboo hand torch is a small, workmanlike torch built from split bamboo. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 410.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
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
		BuilderNotes = "Light torch with a bamboo shaft, good for short outdoor movement.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightEastAsianBambooPaperLantern() => new()
	{
		Metadata = new("medieval_light_east_asian_bamboo_paper_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11371", []),
		Noun = "lantern",
		ShortDescription = "a bamboo paper lantern",
		LongDescription = null,
		FullDescription = "This bamboo paper lantern is a small, workmanlike lantern built from split bamboo. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
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
		BuilderNotes = "Light bamboo-framed lantern with a paper shade around a small candle light.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightEastAsianBronzeOilLamp() => new()
	{
		Metadata = new("medieval_light_east_asian_bronze_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11400", []),
		Noun = "lamp",
		ShortDescription = "a bronze oil lamp",
		LongDescription = null,
		FullDescription = "This bronze oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 32.0m,
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
		BuilderNotes = "Compact bronze oil lamp for a room, desk, or household shrine-neutral alcove.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightEastAsianCypressFrameLantern() => new()
	{
		Metadata = new("medieval_light_east_asian_cypress_frame_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11429", []),
		Noun = "lantern",
		ShortDescription = "a cypress frame lantern",
		LongDescription = null,
		FullDescription = "This cypress frame lantern is a small, well-made lantern built from cypress boards. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Candle_Long"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wood-framed lantern intended to shield a slow candle flame behind pale panels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightEastAsianPaperCandleShade() => new()
	{
		Metadata = new("medieval_light_east_asian_paper_candle_shade", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11458", []),
		Noun = "shade",
		ShortDescription = "a paper candle shade",
		LongDescription = null,
		FullDescription = "This paper candle shade is a small, workmanlike shade made from layered paper. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Folded paper shade for softening candle light; it has no light-source behaviour by itself.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightEastAsianPineLampStand() => new()
	{
		Metadata = new("medieval_light_east_asian_pine_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11486", []),
		Noun = "stand",
		ShortDescription = "a low pine lamp stand",
		LongDescription = null,
		FullDescription = "This low pine lamp stand is a medium-sized, workmanlike stand built from pine boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 22.0m,
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
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low timber stand for a lamp, candle lantern, or small vessel.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightEastAsianPorcelainTableLamp() => new()
	{
		Metadata = new("medieval_light_east_asian_porcelain_table_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11515", []),
		Noun = "lamp",
		ShortDescription = "a porcelain table lamp",
		LongDescription = null,
		FullDescription = "This porcelain table lamp is a small, well-made lamp formed from porcelain. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 680.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "porcelain",
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
		BuilderNotes = "Porcelain-bodied lamp with a smooth pale finish and a small fuel reservoir.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatSteppePortableBrazier() => new()
	{
		Metadata = new("medieval_heat_steppe_portable_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11544", []),
		Noun = "brazier",
		ShortDescription = "a portable camp brazier",
		LongDescription = null,
		FullDescription = "This portable camp brazier is a medium-sized, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
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
		BuilderNotes = "Compact brazier for camps, tents, caravans, and temporary shelters.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeatSteppeWillowFireBasket() => new()
	{
		Metadata = new("medieval_heat_steppe_willow_fire_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11573", []),
		Noun = "basket",
		ShortDescription = "a willow fire basket",
		LongDescription = null,
		FullDescription = "This willow fire basket is a medium-sized, workmanlike basket built from willow boards. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light basket-like fuel holder for small camp embers, reinforced enough for careful use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSteppeBronzeCampLamp() => new()
	{
		Metadata = new("medieval_light_steppe_bronze_camp_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11602", []),
		Noun = "lamp",
		ShortDescription = "a compact bronze camp lamp",
		LongDescription = null,
		FullDescription = "This compact bronze camp lamp is a very small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 24.0m,
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
		BuilderNotes = "Small durable oil lamp suited to a tent, wagon, camp, or roadside lodging.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSteppeCedarResinTorch() => new()
	{
		Metadata = new("medieval_light_steppe_cedar_resin_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11631", []),
		Noun = "torch",
		ShortDescription = "a cedar resin travel torch",
		LongDescription = null,
		FullDescription = "This cedar resin travel torch is a small, workmanlike torch built from cedar boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560.0,
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
		BuilderNotes = "Resinous torch for road, camp, and caravan use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSteppeHornRidingLantern() => new()
	{
		Metadata = new("medieval_light_steppe_horn_riding_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11660", []),
		Noun = "lantern",
		ShortDescription = "a horn riding lantern",
		LongDescription = null,
		FullDescription = "This horn riding lantern is a small, well-made lantern worked from horn. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 30.0m,
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
		BuilderNotes = "Compact horn-panel lantern made to be carried or hung during travel.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSteppeLampBox() => new()
	{
		Metadata = new("medieval_light_steppe_lamp_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11689", []),
		Noun = "box",
		ShortDescription = "a travelling lamp box",
		LongDescription = null,
		FullDescription = "This travelling lamp box is a small, workmanlike box built from pine boards. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1150.0,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact box for carrying a small lamp, wick pieces, and lighting tools; it is storage rather than a light source.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLightSteppeTravelLantern() => new()
	{
		Metadata = new("medieval_light_steppe_travel_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11718", []),
		Noun = "lantern",
		ShortDescription = "a leather-covered travel lantern",
		LongDescription = null,
		FullDescription = "This leather-covered travel lantern is a small, workmanlike lantern made from worked leather. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1050.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
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
		BuilderNotes = "Travel lantern with protective leather covering and a compact fuel body.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorBedCanopyCloth() => new()
	{
		Metadata = new("medieval_decor_bed_canopy_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11747", []),
		Noun = "cloth",
		ShortDescription = "a bed canopy cloth",
		LongDescription = null,
		FullDescription = "This bed canopy cloth is a large, well-made cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1900.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad canopy cloth meant to dress a bed frame or suspended sleeping space.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorBedCurtainPair() => new()
	{
		Metadata = new("medieval_decor_bed_curtain_pair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11775", []),
		Noun = "curtain",
		ShortDescription = "a pair of bed curtains",
		LongDescription = null,
		FullDescription = "This pair of bed curtains is large, workmanlike curtain made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pair of hanging bed curtains for privacy and decoration; they are not doorway curtains.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorBoneInlayPanel() => new()
	{
		Metadata = new("medieval_decor_bone_inlay_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11803", []),
		Noun = "panel",
		ShortDescription = "a bone-inlaid wall panel",
		LongDescription = null,
		FullDescription = "This bone-inlaid wall panel is a small, well-made panel worked from bone. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative wall panel set with pale bone inlay in simple patterns.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorBorderedValance() => new()
	{
		Metadata = new("medieval_decor_bordered_valance", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11831", []),
		Noun = "valance",
		ShortDescription = "a bordered cloth valance",
		LongDescription = null,
		FullDescription = "This bordered cloth valance is a medium-sized, well-made valance made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 650.0,
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
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative hanging strip for a bed, shelf, canopy, or wall edge.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorBrassWallMirror() => new()
	{
		Metadata = new("medieval_decor_brass_wall_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11859", []),
		Noun = "mirror",
		ShortDescription = "a brass-framed wall mirror",
		LongDescription = null,
		FullDescription = "This brass-framed wall mirror is a small, well-made mirror worked from brass. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small mirror with a brass frame and visible suspension loop.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorCarvedHouseholdBoard() => new()
	{
		Metadata = new("medieval_decor_carved_household_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11887", []),
		Noun = "board",
		ShortDescription = "a carved household board",
		LongDescription = null,
		FullDescription = "This carved household board is a medium-sized, workmanlike board built from oak boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2800.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Decorations",
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
		BuilderNotes = "Carved display board for a hall, workshop, or household room.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorCarvedOakWallPanel() => new()
	{
		Metadata = new("medieval_decor_carved_oak_wall_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11915", []),
		Noun = "panel",
		ShortDescription = "a carved oak wall panel",
		LongDescription = null,
		FullDescription = "This carved oak wall panel is a medium-sized, well-made panel built from oak boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Decorations",
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
		BuilderNotes = "Portable carved panel suitable for hanging or leaning against a wall.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorCeilingCanopyCloth() => new()
	{
		Metadata = new("medieval_decor_ceiling_canopy_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11943", []),
		Noun = "cloth",
		ShortDescription = "a ceiling canopy cloth",
		LongDescription = null,
		FullDescription = "This ceiling canopy cloth is a large, well-made cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad cloth intended to be suspended above a bed, dais, or seat.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorCeramicTilePanel() => new()
	{
		Metadata = new("medieval_decor_ceramic_tile_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11971", []),
		Noun = "panel",
		ShortDescription = "a ceramic tile panel",
		LongDescription = null,
		FullDescription = "This ceramic tile panel is a medium-sized, well-made panel formed from ceramic. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Panel made from fitted decorative tiles, meant for a wall, hearth surround, or chamber display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorCheckedWoolRug() => new()
	{
		Metadata = new("medieval_decor_checked_wool_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11999", []),
		Noun = "rug",
		ShortDescription = "a checked wool rug",
		LongDescription = null,
		FullDescription = "This checked wool rug is a medium-sized, workmanlike rug made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2200.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Checked household rug suited to a bench, bed, or sitting space.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorChestCoverCloth() => new()
	{
		Metadata = new("medieval_decor_chest_cover_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12027", []),
		Noun = "cloth",
		ShortDescription = "a chest cover cloth",
		LongDescription = null,
		FullDescription = "This chest cover cloth is a medium-sized, workmanlike cloth made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative cloth sized to cover a chest, coffer, or storage bench.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorCoarseWoolFloorRug() => new()
	{
		Metadata = new("medieval_decor_coarse_wool_floor_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12055", []),
		Noun = "rug",
		ShortDescription = "a coarse wool floor rug",
		LongDescription = null,
		FullDescription = "This coarse wool floor rug is a medium-sized, workmanlike rug made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain woven rug for a room, chamber, stall, or sleeping space.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorCottonFloorSpread() => new()
	{
		Metadata = new("medieval_decor_cotton_floor_spread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12083", []),
		Noun = "spread",
		ShortDescription = "a cotton floor spread",
		LongDescription = null,
		FullDescription = "This cotton floor spread is a large, workmanlike spread made from woven cotton. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light floor spread for warm rooms, guest spaces, and low seating arrangements.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEmbroideredWallHanging() => new()
	{
		Metadata = new("medieval_decor_embroidered_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12111", []),
		Noun = "hanging",
		ShortDescription = "an embroidered wall hanging",
		LongDescription = null,
		FullDescription = "This embroidered wall hanging is a large, well-made hanging made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wool hanging with visible embroidered borders and worked panels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorFeastTableRunner() => new()
	{
		Metadata = new("medieval_decor_feast_table_runner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12139", []),
		Noun = "runner",
		ShortDescription = "a woven feast-table runner",
		LongDescription = null,
		FullDescription = "This woven feast-table runner is a large, well-made runner made from woven linen. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 950.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long decorative runner for a trestle, feast board, or high table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorFeltFloorPad() => new()
	{
		Metadata = new("medieval_decor_felt_floor_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12167", []),
		Noun = "pad",
		ShortDescription = "a thick felt floor pad",
		LongDescription = null,
		FullDescription = "This thick felt floor pad is a medium-sized, workmanlike pad made from pressed felt. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Thick felt pad for kneeling, sitting, or softening a hard floor; it has no seating component.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorFleeceFloorRug() => new()
	{
		Metadata = new("medieval_decor_fleece_floor_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12195", []),
		Noun = "rug",
		ShortDescription = "a soft fleece floor rug",
		LongDescription = null,
		FullDescription = "This soft fleece floor rug is a medium-sized, workmanlike rug made from dressed fur. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1700.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft pelt-like rug with a warm woolly surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorFurWallDrape() => new()
	{
		Metadata = new("medieval_decor_fur_wall_drape", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12223", []),
		Noun = "drape",
		ShortDescription = "a fur-backed wall drape",
		LongDescription = null,
		FullDescription = "This fur-backed wall drape is a large, well-made drape made from dressed fur. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Warm wall drape with fur backing and a plain textile face.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorGlassWallMirror() => new()
	{
		Metadata = new("medieval_decor_glass_wall_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12251", []),
		Noun = "mirror",
		ShortDescription = "a small glass wall mirror",
		LongDescription = null,
		FullDescription = "This small glass wall mirror is a small, well-made mirror made from glass. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 700.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small glass mirror backed and framed for display on a wall or chest top.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorHearthsideRug() => new()
	{
		Metadata = new("medieval_decor_hearthside_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12279", []),
		Noun = "rug",
		ShortDescription = "a hearthside wool rug",
		LongDescription = null,
		FullDescription = "This hearthside wool rug is a medium-sized, workmanlike rug made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Dense rug intended for a hearthside, bench-end, or warm sitting corner.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorHempFloorMat() => new()
	{
		Metadata = new("medieval_decor_hemp_floor_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12307", []),
		Noun = "mat",
		ShortDescription = "a plaited hemp floor mat",
		LongDescription = null,
		FullDescription = "This plaited hemp floor mat is a medium-sized, workmanlike mat made from woven hemp. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hard-wearing plaited mat for an entry, service room, or work space.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorHideFloorRug() => new()
	{
		Metadata = new("medieval_decor_hide_floor_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12335", []),
		Noun = "rug",
		ShortDescription = "a tanned hide floor rug",
		LongDescription = null,
		FullDescription = "This tanned hide floor rug is a medium-sized, workmanlike rug made from cured animal skin. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1900.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "animal skin",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Flat tanned hide used as a simple floor covering.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorHouseholdWallBanner() => new()
	{
		Metadata = new("medieval_decor_household_wall_banner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12363", []),
		Noun = "banner",
		ShortDescription = "a household wall banner",
		LongDescription = null,
		FullDescription = "This household wall banner is a medium-sized, well-made banner made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative banner for a hall or chamber; skins can define exact symbols or colours.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIvoryInlayPanel() => new()
	{
		Metadata = new("medieval_decor_ivory_inlay_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12391", []),
		Noun = "panel",
		ShortDescription = "an ivory-inlaid panel",
		LongDescription = null,
		FullDescription = "This ivory-inlaid panel is a small, well-made panel worked from ivory. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 650.0,
		Cost = 96.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivory",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small decorative panel with pale inlay pieces set into a darker backing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorLargeHallCarpet() => new()
	{
		Metadata = new("medieval_decor_large_hall_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12419", []),
		Noun = "carpet",
		ShortDescription = "a large hall carpet",
		LongDescription = null,
		FullDescription = "This large hall carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 7200.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large heavy carpet for a hall, solar, or public receiving room.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorLinenFloorCloth() => new()
	{
		Metadata = new("medieval_decor_linen_floor_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12447", []),
		Noun = "cloth",
		ShortDescription = "a plain linen floor cloth",
		LongDescription = null,
		FullDescription = "This plain linen floor cloth is a medium-sized, workmanlike cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Washable floor cloth for a clean chamber, nursery, or work corner.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorLinenPillowCover() => new()
	{
		Metadata = new("medieval_decor_linen_pillow_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12475", []),
		Noun = "cover",
		ShortDescription = "a linen pillow cover",
		LongDescription = null,
		FullDescription = "This linen pillow cover is a small, workmanlike cover made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative removable cover for a pillow or cushion, without any wearable behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorLinenWallCurtain() => new()
	{
		Metadata = new("medieval_decor_linen_wall_curtain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12503", []),
		Noun = "curtain",
		ShortDescription = "a linen wall curtain",
		LongDescription = null,
		FullDescription = "This linen wall curtain is a large, workmanlike curtain made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
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
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Loose curtain intended for a wall, alcove, or window area rather than a doorway.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorNarrowHallRunner() => new()
	{
		Metadata = new("medieval_decor_narrow_hall_runner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12531", []),
		Noun = "runner",
		ShortDescription = "a narrow wool hall runner",
		LongDescription = null,
		FullDescription = "This narrow wool hall runner is a large, workmanlike runner made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long narrow floor textile for passages, aisles, and the space beside benches or beds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPaddedFloorThrow() => new()
	{
		Metadata = new("medieval_decor_padded_floor_throw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12559", []),
		Noun = "throw",
		ShortDescription = "a padded floor throw",
		LongDescription = null,
		FullDescription = "This padded floor throw is a medium-sized, workmanlike throw made from woven wool. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1700.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft layered textile that can be spread on a floor or platform for comfort.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPaddedSeatCover() => new()
	{
		Metadata = new("medieval_decor_padded_seat_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12587", []),
		Noun = "cover",
		ShortDescription = "a padded seat cover",
		LongDescription = null,
		FullDescription = "This padded seat cover is a small, workmanlike cover made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft cover for an existing chair or bench; it has no chair component by itself.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPaintedCeilingPanel() => new()
	{
		Metadata = new("medieval_decor_painted_ceiling_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12615", []),
		Noun = "panel",
		ShortDescription = "a painted ceiling panel",
		LongDescription = null,
		FullDescription = "This painted ceiling panel is a medium-sized, well-made panel built from pine boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Decorations",
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
		BuilderNotes = "Light painted panel intended for overhead or high-wall decoration.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPaintedPinePanel() => new()
	{
		Metadata = new("medieval_decor_painted_pine_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12643", []),
		Noun = "panel",
		ShortDescription = "a painted pine wall panel",
		LongDescription = null,
		FullDescription = "This painted pine wall panel is a medium-sized, workmanlike panel built from pine boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Decorations",
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
		BuilderNotes = "Light timber panel with simple painted border work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPaintedScreenPanel() => new()
	{
		Metadata = new("medieval_decor_painted_screen_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12671", []),
		Noun = "screen",
		ShortDescription = "a painted screen panel",
		LongDescription = null,
		FullDescription = "This painted screen panel is a large, well-made screen built from pine boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 3800.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Decorations",
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
		BuilderNotes = "Freestanding painted panel for a room corner, alcove, or private chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPaintedWallCloth() => new()
	{
		Metadata = new("medieval_decor_painted_wall_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12699", []),
		Noun = "cloth",
		ShortDescription = "a painted wall cloth",
		LongDescription = null,
		FullDescription = "This painted wall cloth is a large, well-made cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1400.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Linen wall cloth painted with faded decorative shapes and borders.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPlainTableCover() => new()
	{
		Metadata = new("medieval_decor_plain_table_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12727", []),
		Noun = "cover",
		ShortDescription = "a plain linen table cover",
		LongDescription = null,
		FullDescription = "This plain linen table cover is a medium-sized, workmanlike cover made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple cover for a table, chest top, or serving board.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPlainWoolWallHanging() => new()
	{
		Metadata = new("medieval_decor_plain_wool_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12755", []),
		Noun = "hanging",
		ShortDescription = "a plain wool wall hanging",
		LongDescription = null,
		FullDescription = "This plain wool wall hanging is a large, workmanlike hanging made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain wall textile for warmth, decoration, and softening bare walls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPlasterWallPlaque() => new()
	{
		Metadata = new("medieval_decor_plaster_wall_plaque", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12783", []),
		Noun = "plaque",
		ShortDescription = "a plaster wall plaque",
		LongDescription = null,
		FullDescription = "This plaster wall plaque is a small, workmanlike plaque moulded from plaster. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "plaster",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cast plaster plaque for household wall decoration.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPolishedBronzeWallMirror() => new()
	{
		Metadata = new("medieval_decor_polished_bronze_wall_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12811", []),
		Noun = "mirror",
		ShortDescription = "a polished bronze wall mirror",
		LongDescription = null,
		FullDescription = "This polished bronze wall mirror is a small, well-made mirror worked from bronze. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished bronze mirror plate in a simple hanging frame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorPortableDisplayScreen() => new()
	{
		Metadata = new("medieval_decor_portable_display_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12839", []),
		Noun = "screen",
		ShortDescription = "a portable display screen",
		LongDescription = null,
		FullDescription = "This portable display screen is a large, well-made screen built from walnut boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 7200.0,
		Cost = 86.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Decorations",
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
		BuilderNotes = "Good-quality freestanding screen with a polished wood frame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorRoomDividerHanging() => new()
	{
		Metadata = new("medieval_decor_room_divider_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12867", []),
		Noun = "hanging",
		ShortDescription = "a room-divider hanging",
		LongDescription = null,
		FullDescription = "This room-divider hanging is a large, workmanlike hanging made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large textile divider for a chamber or workroom; it has no door or movement-blocking behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorRoundFloorMat() => new()
	{
		Metadata = new("medieval_decor_round_floor_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12895", []),
		Noun = "mat",
		ShortDescription = "a round woven floor mat",
		LongDescription = null,
		FullDescription = "This round woven floor mat is a medium-sized, workmanlike mat made from woven hemp. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Round plaited mat for a small table, stool cluster, or bare floor.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorShelfCoverCloth() => new()
	{
		Metadata = new("medieval_decor_shelf_cover_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12923", []),
		Noun = "cloth",
		ShortDescription = "a shelf cover cloth",
		LongDescription = null,
		FullDescription = "This shelf cover cloth is a small, workmanlike cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small cloth for dressing a shelf, chest top, or side table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSilkTableCover() => new()
	{
		Metadata = new("medieval_decor_silk_table_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12951", []),
		Noun = "cover",
		ShortDescription = "a fine silk table cover",
		LongDescription = null,
		FullDescription = "This fine silk table cover is a medium-sized, well-made cover made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine table cover for an elite chamber, feast setting, or display surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSilkWallHanging() => new()
	{
		Metadata = new("medieval_decor_silk_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:12979", []),
		Noun = "hanging",
		ShortDescription = "a silk wall hanging",
		LongDescription = null,
		FullDescription = "This silk wall hanging is a large, well-made hanging made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 92.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine wall hanging with a smooth lustrous surface and narrow worked edging.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSmallBedsideMat() => new()
	{
		Metadata = new("medieval_decor_small_bedside_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13007", []),
		Noun = "mat",
		ShortDescription = "a small bedside floor mat",
		LongDescription = null,
		FullDescription = "This small bedside floor mat is a small, workmanlike mat made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small soft mat for the side of a bed, cot, pallet, or sleeping platform.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSmallChairThrow() => new()
	{
		Metadata = new("medieval_decor_small_chair_throw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13035", []),
		Noun = "throw",
		ShortDescription = "a small chair throw",
		LongDescription = null,
		FullDescription = "This small chair throw is a small, workmanlike throw made from woven wool. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 600.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small textile throw for a chair, stool, or high seat.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorStoryWovenTapestry() => new()
	{
		Metadata = new("medieval_decor_story_woven_tapestry", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13063", []),
		Noun = "tapestry",
		ShortDescription = "a story-woven tapestry",
		LongDescription = null,
		FullDescription = "This story-woven tapestry is a large, well-made tapestry made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 96.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large figured tapestry intended for narrative, heraldic, household, or fantasy-world skin variants.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
