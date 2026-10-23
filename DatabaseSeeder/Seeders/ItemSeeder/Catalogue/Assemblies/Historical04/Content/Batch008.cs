#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanFlatbreadServingPlatter() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_flatbread_serving_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6632", []),
		Noun = "platter",
		ShortDescription = "a flatbread serving platter",
		LongDescription = null,
		FullDescription = "This flatbread serving platter is a medium-sized, workmanlike platter built from plain wood. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
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
		BuilderNotes = "Low platter for flatbread, dates, and shared table foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanGlazedSherbetCup() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_glazed_sherbet_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6663", []),
		Noun = "cup",
		ShortDescription = "a glazed sherbet cup",
		LongDescription = null,
		FullDescription = "This glazed sherbet cup is a very small, well-made cup formed from ceramic. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 220.0,
		Cost = 20.0m,
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
		BuilderNotes = "Small glazed cup for sweetened drinks, cordials, or cool water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanLongNeckedCeramicJug() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_long_necked_ceramic_jug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6694", []),
		Noun = "jug",
		ShortDescription = "a long-necked ceramic jug",
		LongDescription = null,
		FullDescription = "This long-necked ceramic jug is a small, well-made jug formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
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
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long-necked jug form for water or sherbet-like drinks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanLusterwareBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_lusterware_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6725", []),
		Noun = "bowl",
		ShortDescription = "a lustred ceramic bowl",
		LongDescription = null,
		FullDescription = "This lustred ceramic bowl is a small, well-made bowl formed from ceramic. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 34.0m,
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
		BuilderNotes = "Fine decorated bowl for table presentation; dry/open container profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanRosewaterFlask() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_rosewater_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6756", []),
		Noun = "flask",
		ShortDescription = "a rosewater flask",
		LongDescription = null,
		FullDescription = "This rosewater flask is a very small, well-made flask made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 160.0,
		Cost = 32.0m,
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
		BuilderNotes = "Small glass flask for rosewater or scented liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanWaterCoolingJar() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_water_cooling_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6787", []),
		Noun = "jar",
		ShortDescription = "a porous water-cooling jar",
		LongDescription = null,
		FullDescription = "This porous water-cooling jar is a large, workmanlike jar formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7600.0,
		Cost = 24.0m,
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
		BuilderNotes = "Porous ceramic water jar for domestic cooling and storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedIslamicateUrbanWideCouscousBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_islamicate_urban_wide_couscous_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6818", []),
		Noun = "bowl",
		ShortDescription = "a wide grain bowl",
		LongDescription = null,
		FullDescription = "This wide grain bowl is a medium-sized, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
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
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wide open bowl for grain, pottage, or shared thick foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianBananaLeafTray() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_banana_leaf_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6849", []),
		Noun = "tray",
		ShortDescription = "a broad leaf tray",
		LongDescription = null,
		FullDescription = "This broad leaf tray is a medium-sized, workmanlike tray made from layered leaf fibre. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leaf",
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
		BuilderNotes = "Large disposable serving tray made from broad leaves.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianBrassLota() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_brass_lota", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6880", []),
		Noun = "lota",
		ShortDescription = "a brass lota vessel",
		LongDescription = null,
		FullDescription = "This brass lota vessel is a small, well-made lota worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 700.0,
		Cost = 32.0m,
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
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rounded water vessel for drinking, washing, or domestic pouring.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianCopperLota() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_copper_lota", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6911", []),
		Noun = "lota",
		ShortDescription = "a copper lota vessel",
		LongDescription = null,
		FullDescription = "This copper lota vessel is a small, well-made lota worked from copper. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
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
		BuilderNotes = "Rounded copper water vessel for household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianCurryBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_curry_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6942", []),
		Noun = "bowl",
		ShortDescription = "a small curry bowl",
		LongDescription = null,
		FullDescription = "This small curry bowl is a small, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
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
		BuilderNotes = "Small open bowl for thick stews, sauces, or side dishes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianGheeFlask() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_ghee_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:6973", []),
		Noun = "flask",
		ShortDescription = "a ghee flask",
		LongDescription = null,
		FullDescription = "This ghee flask is a very small, well-made flask worked from brass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 24.0m,
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
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small vessel for clarified butter or oil.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianLeafPlatter() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_leaf_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7004", []),
		Noun = "platter",
		ShortDescription = "a stitched leaf platter",
		LongDescription = null,
		FullDescription = "This stitched leaf platter is a small, workmanlike platter made from layered leaf fibre. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leaf",
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
		BuilderNotes = "Disposable or cheap leaf platter for simple food service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianSpiceCupSet() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_spice_cup_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7035", []),
		Noun = "cup",
		ShortDescription = "a small spice cup",
		LongDescription = null,
		FullDescription = "This small spice cup is a very small, well-made cup worked from brass. A small hollow sits inside a thick rim, with a flat base beneath it. The lip is polished where fingers have lifted it from the table. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 160.0,
		Cost = 16.0m,
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
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small metal cup for dry spices or condiments; open-container profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianStoneGrindingBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_stone_grinding_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7066", []),
		Noun = "bowl",
		ShortDescription = "a stone condiment bowl",
		LongDescription = null,
		FullDescription = "This stone condiment bowl is a small, workmanlike bowl cut from soapstone. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "soapstone",
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
		BuilderNotes = "Heavy bowl for chutneys, salt, or thick condiments; not a tool component.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianThaliPlatter() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_thali_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7097", []),
		Noun = "platter",
		ShortDescription = "a round thali platter",
		LongDescription = null,
		FullDescription = "This round thali platter is a medium-sized, well-made platter worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 42.0m,
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
		BuilderNotes = "Broad metal platter for several small dishes at once.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSouthAsianWaterPot() => new()
	{
		Metadata = new("medieval_tableware_shared_south_asian_water_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7128", []),
		Noun = "pot",
		ShortDescription = "a red clay water pot",
		LongDescription = null,
		FullDescription = "This red clay water pot is a large, workmanlike pot formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8200.0,
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
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large water pot for domestic storage and cooling.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianBambooServingTray() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_bamboo_serving_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7159", []),
		Noun = "tray",
		ShortDescription = "a bamboo serving tray",
		LongDescription = null,
		FullDescription = "This bamboo serving tray is a medium-sized, workmanlike tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
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
		BuilderNotes = "Light tray for cups, bowls, and tea or meal service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianBambooSteamingTray() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_bamboo_steaming_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7190", []),
		Noun = "tray",
		ShortDescription = "a bamboo steaming tray",
		LongDescription = null,
		FullDescription = "This bamboo steaming tray is a medium-sized, workmanlike tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 600.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
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
		BuilderNotes = "Tray-like food vessel for dumplings, buns, or steamed foods; does not provide heat mechanics.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianCeladonTableBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_celadon_table_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7221", []),
		Noun = "bowl",
		ShortDescription = "a celadon table bowl",
		LongDescription = null,
		FullDescription = "This celadon table bowl is a small, well-made bowl formed from ceramic. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 34.0m,
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
		BuilderNotes = "Greenish fine ceramic bowl for refined table service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianCeramicTeapot() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_ceramic_teapot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7252", []),
		Noun = "pot",
		ShortDescription = "a ceramic tea pot",
		LongDescription = null,
		FullDescription = "This ceramic tea pot is a small, well-made pot formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 38.0m,
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
		BuilderNotes = "Small pouring pot using a jug capacity profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianCoveredFoodBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_covered_food_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7283", []),
		Noun = "bowl",
		ShortDescription = "a covered food bowl",
		LongDescription = null,
		FullDescription = "This covered food bowl is a small, well-made bowl formed from porcelain. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 38.0m,
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
		BuilderNotes = "Covered-looking bowl for rice or prepared foods; cover is descriptive only.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianLowLacqueredTray() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_low_lacquered_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7314", []),
		Noun = "tray",
		ShortDescription = "a low lacquered tray",
		LongDescription = null,
		FullDescription = "This low lacquered tray is a medium-sized, well-made tray built from plain wood. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 32.0m,
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
		BuilderNotes = "Low polished tray; lacquer effect is presentation rather than a material entry.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianPorcelainRiceBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_porcelain_rice_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7345", []),
		Noun = "bowl",
		ShortDescription = "a porcelain rice bowl",
		LongDescription = null,
		FullDescription = "This porcelain rice bowl is a small, well-made bowl formed from porcelain. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 340.0,
		Cost = 32.0m,
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
		BuilderNotes = "Fine bowl for rice or small foods; dry/open container profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianRiceWineFlask() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_rice_wine_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7376", []),
		Noun = "flask",
		ShortDescription = "a rice-wine flask",
		LongDescription = null,
		FullDescription = "This rice-wine flask is a very small, well-made flask formed from ceramic. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
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
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small flask for rice wine or other table drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianSmallTeaCup() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_small_tea_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7407", []),
		Noun = "cup",
		ShortDescription = "a small tea cup",
		LongDescription = null,
		FullDescription = "This small tea cup is a tiny, well-made cup formed from porcelain. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "porcelain",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Pony"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tiny cup for tea, medicine, or refined drink service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianSoyCruet() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_soy_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7438", []),
		Noun = "cruet",
		ShortDescription = "a small sauce cruet",
		LongDescription = null,
		FullDescription = "This small sauce cruet is a very small, well-made cruet formed from ceramic. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 160.0,
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
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small pouring vessel for sauce, vinegar, or oil.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianStonewareTeaCup() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_stoneware_tea_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7469", []),
		Noun = "cup",
		ShortDescription = "a stoneware tea cup",
		LongDescription = null,
		FullDescription = "This stoneware tea cup is a very small, workmanlike cup formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 12.0m,
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
				"LContainer_WhiskeyGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small cup for tea or warm drink service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedEastAsianWideNoodleBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_east_asian_wide_noodle_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7500", []),
		Noun = "bowl",
		ShortDescription = "a wide noodle bowl",
		LongDescription = null,
		FullDescription = "This wide noodle bowl is a small, workmanlike bowl formed from stoneware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
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
		BuilderNotes = "Wide bowl for noodles or grain dishes; not a liquid-storage container.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSteppeAndCaravanCampDrinkingBowl() => new()
	{
		Metadata = new("medieval_tableware_shared_steppe_and_caravan_camp_drinking_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7531", []),
		Noun = "bowl",
		ShortDescription = "a camp drinking bowl",
		LongDescription = null,
		FullDescription = "This camp drinking bowl is a small, workmanlike bowl built from plain wood. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
		Cost = 6.0m,
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
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable wooden bowl for water, milk, or broth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSteppeAndCaravanFeltWrappedCanteen() => new()
	{
		Metadata = new("medieval_tableware_shared_steppe_and_caravan_felt_wrapped_canteen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7562", []),
		Noun = "canteen",
		ShortDescription = "a felt-wrapped canteen",
		LongDescription = null,
		FullDescription = "This felt-wrapped canteen is a small, workmanlike canteen built from plain wood. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 18.0m,
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
		BuilderNotes = "Flat canteen with protective wrap described visually.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSteppeAndCaravanHideWaterskin() => new()
	{
		Metadata = new("medieval_tableware_shared_steppe_and_caravan_hide_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7593", []),
		Noun = "waterskin",
		ShortDescription = "a hide waterskin",
		LongDescription = null,
		FullDescription = "This hide waterskin is a small, workmanlike waterskin made from cured animal skin. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 13.0m,
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
		BuilderNotes = "Travel water vessel suited to mounted or caravan households.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSteppeAndCaravanLeatherKumisFlask() => new()
	{
		Metadata = new("medieval_tableware_shared_steppe_and_caravan_leather_kumis_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7624", []),
		Noun = "flask",
		ShortDescription = "a leather kumis flask",
		LongDescription = null,
		FullDescription = "This leather kumis flask is a small, workmanlike flask made from worked leather. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
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
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable flask for fermented milk or other travelling drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSteppeAndCaravanPortableCheeseTray() => new()
	{
		Metadata = new("medieval_tableware_shared_steppe_and_caravan_portable_cheese_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7655", []),
		Noun = "tray",
		ShortDescription = "a portable cheese tray",
		LongDescription = null,
		FullDescription = "This portable cheese tray is a small, workmanlike tray made from worked leather. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
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
		BuilderNotes = "Light tray or mat-like surface for cheese, curds, or travel foods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSteppeAndCaravanSmallFermentingSkin() => new()
	{
		Metadata = new("medieval_tableware_shared_steppe_and_caravan_small_fermenting_skin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7686", []),
		Noun = "skin",
		ShortDescription = "a small fermenting skin",
		LongDescription = null,
		FullDescription = "This small fermenting skin is a medium-sized, workmanlike skin made from cured animal skin. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "animal skin",
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
		BuilderNotes = "Skin vessel for fermenting or carrying dairy drink.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSteppeAndCaravanTravelMeatBoard() => new()
	{
		Metadata = new("medieval_tableware_shared_steppe_and_caravan_travel_meat_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7717", []),
		Noun = "board",
		ShortDescription = "a travel meat board",
		LongDescription = null,
		FullDescription = "This travel meat board is a small, workmanlike board built from birch boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 6.0m,
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
		BuilderNotes = "Compact board for cutting or serving dried meat and cheese.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareSharedSteppeAndCaravanWoodenSaddleCup() => new()
	{
		Metadata = new("medieval_tableware_shared_steppe_and_caravan_wooden_saddle_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:7748", []),
		Noun = "cup",
		ShortDescription = "a saddle-carried wooden cup",
		LongDescription = null,
		FullDescription = "This saddle-carried wooden cup is a very small, workmanlike cup built from plain wood. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 7.0m,
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
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small rugged cup for travel, herding, or caravan use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalToolButchersKnife() => new()
	{
		Metadata = new("medieval_tool_butchers_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:96", []),
		Noun = "knife",
		ShortDescription = "a broad iron butcher's knife",
		LongDescription = null,
		FullDescription = "This broad wrought-iron knife has a thick spine, a keen working edge, and an ash handle pinned firmly through the tang for breaking down heavy raw cuts.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 18.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = ["Functions / Tools / Butcher Tools / Meat Cutting Tools / Butcher's Knife", "Market / Professional Tools / Standard Tools"],
		Components = ["Holdable", "Destroyable_Weapon"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalToolCookingKnife() => new()
	{
		Metadata = new("medieval_tool_cooking_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:115", []),
		Noun = "knife",
		ShortDescription = "an iron cooking knife",
		LongDescription = null,
		FullDescription = "This practical wrought-iron kitchen knife has a medium blade, a plain wooden handle, and a fine edge suited to chopping vegetables, trimming dough, and preparing cooked food.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 12.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = ["Functions / Tools / Cooking / Cooking Utensils / Cooking Knife", "Market / Professional Tools / Standard Tools"],
		Components = ["Holdable", "Destroyable_Weapon"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalToolThreshingFlail() => new()
	{
		Metadata = new("medieval_tool_threshing_flail", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:134", []),
		Noun = "flail",
		ShortDescription = "a jointed wooden threshing flail",
		LongDescription = null,
		FullDescription = "This long ash handle is joined by a short leather hinge to a heavier oak swipple, forming a durable flail for beating grain loose from straw.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1700.0,
		Cost = 8.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Functions / Tools / Foodmaking Tools / Threshing Flail", "Market / Professional Tools / Standard Tools"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalToolWinnowingBasket() => new()
	{
		Metadata = new("medieval_tool_winnowing_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:153", []),
		Noun = "basket",
		ShortDescription = "a broad woven winnowing basket",
		LongDescription = null,
		FullDescription = "This broad, shallow willow basket has a tightly woven base and low rim, allowing threshed grain to be tossed into a breeze while chaff blows free.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 7.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = ["Functions / Tools / Agricultural Tools / Winnowing Basket", "Market / Professional Tools / Standard Tools"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalToolCookingPot() => new()
	{
		Metadata = new("medieval_tool_cooking_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:172", []),
		Noun = "pot",
		ShortDescription = "a soot-darkened bronze cooking pot",
		LongDescription = null,
		FullDescription = "This round-bellied bronze pot has a reinforced rim, a fitted carrying bail, and a blackened base from long use over hearth fires.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 55.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Functions / Tools / Cooking / Cookware / Cooking Pot", "Market / Professional Tools / Standard Tools"],
		Components = ["Holdable", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWorkshopLauterTun() => new()
	{
		Metadata = new("medieval_workshop_lauter_tun", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFoodProduction.cs:191", []),
		Noun = "tun",
		ShortDescription = "a slatted wooden lauter tun",
		LongDescription = null,
		FullDescription = "This broad oak tun has tight coopered staves, a slatted false bottom, and a plugged outlet low on one side for separating sweet wort from spent grain.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 19000.0,
		Cost = 68.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Functions / Tools / Brewing Tools / Lauter Tun", "Market / Professional Tools / Standard Tools"],
		Components = ["Holdable", "Destroyable_Furniture"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterBrackishWellhead() => new()
	{
		Metadata = new("medieval_water_brackish_wellhead", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:11", []),
		Noun = "wellhead",
		ShortDescription = "a brackish wellhead",
		LongDescription = null,
		FullDescription = "This brackish wellhead is a large, workmanlike wellhead cut from stone. A raised rim surrounds the draw opening, with worn contact marks where rope and bucket have passed. The top edge is smooth from repeated use. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 48000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_BrackishWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wellhead or draw point for brackish water that is not clean potable supply.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterBronzeLaverBasin() => new()
	{
		Metadata = new("medieval_water_bronze_laver_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:41", []),
		Noun = "basin",
		ShortDescription = "a bronze laver basin",
		LongDescription = null,
		FullDescription = "This bronze laver basin is a medium-sized, well-made basin worked from bronze. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 4300.0,
		Cost = 70.0m,
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
				"Sink_20L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better metal basin for handwashing, fine service, and chamber use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterBucketFillingStand() => new()
	{
		Metadata = new("medieval_water_bucket_filling_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:71", []),
		Noun = "stand",
		ShortDescription = "a bucket-filling cistern stand",
		LongDescription = null,
		FullDescription = "This bucket-filling cistern stand is a medium-sized, workmanlike stand built from oak boards. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
		Cost = 36.0m,
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
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Raised filling stand for buckets, pails, and small tubs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterButcherWashSink() => new()
	{
		Metadata = new("medieval_water_butcher_wash_sink", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:101", []),
		Noun = "sink",
		ShortDescription = "a butcher's wash sink",
		LongDescription = null,
		FullDescription = "This butcher's wash sink is a large, workmanlike sink cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Sink_50L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large stone sink for butchery, dyeing, fish cleaning, or other messy household work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterControlledStoneBasin() => new()
	{
		Metadata = new("medieval_water_controlled_stone_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:131", []),
		Noun = "basin",
		ShortDescription = "a controlled stone water basin",
		LongDescription = null,
		FullDescription = "This controlled stone water basin is a large, well-made basin cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 32000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"WaterSource_ProgControlled"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Prog-controlled water basin for areas where supply depends on local script or builder logic.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterCoveredCisternChest() => new()
	{
		Metadata = new("medieval_water_covered_cistern_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:161", []),
		Noun = "cistern",
		ShortDescription = "a covered timber cistern",
		LongDescription = null,
		FullDescription = "This covered timber cistern is a large, workmanlike cistern built from oak boards. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 92.0m,
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
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Covered timber cistern casing for a managed household or yard water supply.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterEarthenwareLaverBasin() => new()
	{
		Metadata = new("medieval_water_earthenware_laver_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:191", []),
		Noun = "basin",
		ShortDescription = "an earthenware laver basin",
		LongDescription = null,
		FullDescription = "This earthenware laver basin is a medium-sized, workmanlike basin formed from earthenware. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
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
				"Sink_20L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad fillable laver basin for household washing and rinsing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterIronWellPump() => new()
	{
		Metadata = new("medieval_water_iron_well_pump", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:221", []),
		Noun = "pump",
		ShortDescription = "an iron well pump",
		LongDescription = null,
		FullDescription = "This iron well pump is a large, well-made pump worked from wrought iron. A handle rises from the pump body, with a spout projecting over the filling space. The base plate is broad and marked by bolt holes. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Infinite_PublicPumpWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdier hand-pump fixture for a well, cistern, or controlled public water point.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterLakeDrawBasin() => new()
	{
		Metadata = new("medieval_water_lake_draw_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:251", []),
		Noun = "basin",
		ShortDescription = "a lake-water draw basin",
		LongDescription = null,
		FullDescription = "This lake-water draw basin is a large, workmanlike basin cut from stone. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28000.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_LakeWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Draw basin representing self-refilling lake-water access.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterLargeLaundryTub() => new()
	{
		Metadata = new("medieval_water_large_laundry_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:281", []),
		Noun = "tub",
		ShortDescription = "a large laundry tub",
		LongDescription = null,
		FullDescription = "This large laundry tub is a large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 46.0m,
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
				"Destroyable_Furniture",
				"Sink_50L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large fillable tub for laundry, soaking cloth, and heavy household washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterLaundryRinseTrough() => new()
	{
		Metadata = new("medieval_water_laundry_rinse_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:311", []),
		Noun = "trough",
		ShortDescription = "a laundry rinse trough",
		LongDescription = null,
		FullDescription = "This laundry rinse trough is a large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 33000.0,
		Cost = 78.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Sink_50L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large fillable rinse trough for laundry, cloth preparation, and soaking.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterOakBathTub() => new()
	{
		Metadata = new("medieval_water_oak_bath_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:341", []),
		Noun = "tub",
		ShortDescription = "an oak bath tub",
		LongDescription = null,
		FullDescription = "This oak bath tub is a very large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30000.0,
		Cost = 120.0m,
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
				"Destroyable_Furniture",
				"Bathtub"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large wooden bathing tub for bathhouse, inn, or wealthy household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterOakLivestockTrough() => new()
	{
		Metadata = new("medieval_water_oak_livestock_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:371", []),
		Noun = "trough",
		ShortDescription = "an oak livestock trough",
		LongDescription = null,
		FullDescription = "This oak livestock trough is a large, workmanlike trough built from oak boards. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 48.0m,
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
				"Destroyable_Furniture",
				"Infinite_PublicTroughWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Timber water trough suited to stables, yards, and byres.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterOakWashTub() => new()
	{
		Metadata = new("medieval_water_oak_wash_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:401", []),
		Noun = "tub",
		ShortDescription = "an oak wash tub",
		LongDescription = null,
		FullDescription = "This oak wash tub is a large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
		Cost = 28.0m,
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
				"Destroyable_Furniture",
				"Sink_20L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wooden wash tub for laundry, scullery work, or household rinsing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterPinePlankBathTub() => new()
	{
		Metadata = new("medieval_water_pine_plank_bath_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:431", []),
		Noun = "tub",
		ShortDescription = "a pine plank bath tub",
		LongDescription = null,
		FullDescription = "This pine plank bath tub is a very large, workmanlike tub built from pine boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26000.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Planked wooden tub sized for bathing and hot-water service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterPublicStoneCistern() => new()
	{
		Metadata = new("medieval_water_public_stone_cistern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:461", []),
		Noun = "cistern",
		ShortDescription = "a public stone cistern",
		LongDescription = null,
		FullDescription = "This public stone cistern is a very large, workmanlike cistern cut from stone. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 52000.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Self-refilling public cistern for communal potable water supply.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterPublicStoneTrough() => new()
	{
		Metadata = new("medieval_water_public_stone_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:491", []),
		Noun = "trough",
		ShortDescription = "a public stone water trough",
		LongDescription = null,
		FullDescription = "This public stone water trough is a very large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 52000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicTroughWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Self-refilling trough for animals, carts, washing, and public water access.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterRefillingOakTub() => new()
	{
		Metadata = new("medieval_water_refilling_oak_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:521", []),
		Noun = "tub",
		ShortDescription = "a refilling oak water tub",
		LongDescription = null,
		FullDescription = "This refilling oak water tub is a large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20000.0,
		Cost = 60.0m,
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
				"Destroyable_Furniture",
				"Infinite_WaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Generic self-refilling water tub for scripted household or service-yard use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterRiverFillingPlank() => new()
	{
		Metadata = new("medieval_water_river_filling_plank", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:551", []),
		Noun = "plank",
		ShortDescription = "a river filling plank",
		LongDescription = null,
		FullDescription = "This river filling plank is a medium-sized, workmanlike plank built from oak boards. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7000.0,
		Cost = 20.0m,
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
				"Destroyable_Furniture",
				"Infinite_RiverWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Planked river-edge filling point for buckets, tubs, and washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterRiverWashStand() => new()
	{
		Metadata = new("medieval_water_river_wash_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:581", []),
		Noun = "stand",
		ShortDescription = "a river wash stand",
		LongDescription = null,
		FullDescription = "This river wash stand is a large, workmanlike stand built from oak boards. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000.0,
		Cost = 32.0m,
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
				"Destroyable_Furniture",
				"Infinite_RiverWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Frame and wash point representing access to a self-refilling river water source.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSaltwaterWashTrough() => new()
	{
		Metadata = new("medieval_water_saltwater_wash_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:611", []),
		Noun = "trough",
		ShortDescription = "a saltwater wash trough",
		LongDescription = null,
		FullDescription = "This saltwater wash trough is a large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30000.0,
		Cost = 56.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SaltWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wash trough drawing from salt water for coastal cleaning, fish work, or rinsing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSmallCeramicHandBasin() => new()
	{
		Metadata = new("medieval_water_small_ceramic_hand_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:641", []),
		Noun = "basin",
		ShortDescription = "a small ceramic hand basin",
		LongDescription = null,
		FullDescription = "This small ceramic hand basin is a small, workmanlike basin formed from ceramic. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 10.0m,
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
				"Sink_5L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small fillable basin for hand washing, shaving, rinsing cloths, or bedside water use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterStoneBathBasin() => new()
	{
		Metadata = new("medieval_water_stone_bath_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:671", []),
		Noun = "basin",
		ShortDescription = "a stone bath basin",
		LongDescription = null,
		FullDescription = "This stone bath basin is a very large, well-made basin cut from stone. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 76000.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Very heavy stone bathing basin for a bathhouse, courtly chamber, or wealthy household.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterStoneHandwashingBasin() => new()
	{
		Metadata = new("medieval_water_stone_handwashing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:701", []),
		Noun = "basin",
		ShortDescription = "a stone handwashing basin",
		LongDescription = null,
		FullDescription = "This stone handwashing basin is a medium-sized, workmanlike basin cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Sink_5L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy stone basin for handwashing near a hall, kitchen, or chamber door.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterStoneScullerySink() => new()
	{
		Metadata = new("medieval_water_stone_scullery_sink", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:731", []),
		Noun = "sink",
		ShortDescription = "a stone scullery sink",
		LongDescription = null,
		FullDescription = "This stone scullery sink is a large, workmanlike sink cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Sink_20L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy fillable sink for scullery, kitchen, and workshop washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterStoneSpringBasin() => new()
	{
		Metadata = new("medieval_water_stone_spring_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:761", []),
		Noun = "basin",
		ShortDescription = "a stone spring basin",
		LongDescription = null,
		FullDescription = "This stone spring basin is a large, workmanlike basin cut from stone. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30000.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stone basin built around a self-refilling spring source.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterStoneWashingTrough() => new()
	{
		Metadata = new("medieval_water_stone_washing_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:791", []),
		Noun = "trough",
		ShortDescription = "a stone washing trough",
		LongDescription = null,
		FullDescription = "This stone washing trough is a large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25000.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Sink_50L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy fillable stone trough for rinsing, scullery work, and yard washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterStoneWellhead() => new()
	{
		Metadata = new("medieval_water_stone_wellhead", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:821", []),
		Noun = "wellhead",
		ShortDescription = "a stone wellhead",
		LongDescription = null,
		FullDescription = "This stone wellhead is a very large, workmanlike wellhead cut from stone. A raised rim surrounds the draw opening, with worn contact marks where rope and bucket have passed. The top edge is smooth from repeated use. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 68000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_WaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable/installable wellhead prototype for a generic self-refilling water source.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSwampDrainBasin() => new()
	{
		Metadata = new("medieval_water_swamp_drain_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:851", []),
		Noun = "basin",
		ShortDescription = "a swamp-water drain basin",
		LongDescription = null,
		FullDescription = "This swamp-water drain basin is a large, plain basin built from plain wood. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Poor,
		WeightInGrams = 16000.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SwampWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough draw basin for swamp or fen water; useful as an environmental source rather than clean household water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterTimberSpringBox() => new()
	{
		Metadata = new("medieval_water_timber_spring_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:881", []),
		Noun = "box",
		ShortDescription = "a timber spring box",
		LongDescription = null,
		FullDescription = "This timber spring box is a large, workmanlike box built from oak boards. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16000.0,
		Cost = 54.0m,
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
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Timber spring box for collecting clean spring water at a yard or roadside.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterTimberWellhead() => new()
	{
		Metadata = new("medieval_water_timber_wellhead", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:911", []),
		Noun = "wellhead",
		ShortDescription = "a timber wellhead",
		LongDescription = null,
		FullDescription = "This timber wellhead is a large, workmanlike wellhead built from oak boards. A raised rim surrounds the draw opening, with worn contact marks where rope and bucket have passed. The top edge is smooth from repeated use. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 78.0m,
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
				"Destroyable_Furniture",
				"Infinite_WaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Timber wellhead or draw-point for a household, yard, or village water supply.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterVillageHandPump() => new()
	{
		Metadata = new("medieval_water_village_hand_pump", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:941", []),
		Noun = "pump",
		ShortDescription = "a village hand pump",
		LongDescription = null,
		FullDescription = "This village hand pump is a large, workmanlike pump worked from wrought iron. A handle rises from the pump body, with a spout projecting over the filling space. The base plate is broad and marked by bolt holes. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Infinite_PublicPumpWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hand-pump style public potable water source for villages, yards, or service courts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterNorthernBogwaterTrough() => new()
	{
		Metadata = new("medieval_water_northern_bogwater_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:971", []),
		Noun = "trough",
		ShortDescription = "a bog-water trough",
		LongDescription = null,
		FullDescription = "This bog-water trough is a large, plain trough built from oak boards. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Poor,
		WeightInGrams = 18000.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SwampWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough trough for fen, bog, or marsh water access.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterNorthernSpringTrough() => new()
	{
		Metadata = new("medieval_water_northern_spring_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1001", []),
		Noun = "trough",
		ShortDescription = "a cold spring trough",
		LongDescription = null,
		FullDescription = "This cold spring trough is a large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30000.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stone trough fed by a cold spring, suitable for northern yards or roadside supply.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterNorthernStaveBathTub() => new()
	{
		Metadata = new("medieval_water_northern_stave_bath_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1031", []),
		Noun = "tub",
		ShortDescription = "a stave-built bath tub",
		LongDescription = null,
		FullDescription = "This stave-built bath tub is a very large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28000.0,
		Cost = 105.0m,
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
				"Destroyable_Furniture",
				"Bathtub"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stave-built wooden bath tub suited to cold-climate bathhouse and hall use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterWesternLavatoriumBasin() => new()
	{
		Metadata = new("medieval_water_western_lavatorium_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1061", []),
		Noun = "basin",
		ShortDescription = "a lavatorium stone basin",
		LongDescription = null,
		FullDescription = "This lavatorium stone basin is a large, well-made basin cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 36000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Sink_50L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large formal washing basin for institutional, refectory, or hall settings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterWesternManorWellPump() => new()
	{
		Metadata = new("medieval_water_western_manor_well_pump", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1091", []),
		Noun = "pump",
		ShortDescription = "a manor well pump",
		LongDescription = null,
		FullDescription = "This manor well pump is a large, well-made pump worked from wrought iron. A handle rises from the pump body, with a spout projecting over the filling space. The base plate is broad and marked by bolt holes. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 23000.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Infinite_PublicPumpWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy pump for a manor yard, service court, or enclosed well.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterWesternTownCistern() => new()
	{
		Metadata = new("medieval_water_western_town_cistern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1121", []),
		Noun = "cistern",
		ShortDescription = "a town conduit cistern",
		LongDescription = null,
		FullDescription = "This town conduit cistern is a very large, well-made cistern cut from stone. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 62000.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Communal cistern or conduit head for town water supply.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterMediterraneanCourtyardFountain() => new()
	{
		Metadata = new("medieval_water_mediterranean_courtyard_fountain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1151", []),
		Noun = "basin",
		ShortDescription = "a courtyard fountain basin",
		LongDescription = null,
		FullDescription = "This courtyard fountain basin is a large, well-made basin cut from marble. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 46000.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "marble",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_WaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Self-refilling courtyard fountain basin for wealthy domestic or public settings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterMediterraneanLimestoneCisternBasin() => new()
	{
		Metadata = new("medieval_water_mediterranean_limestone_cistern_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1181", []),
		Noun = "basin",
		ShortDescription = "a limestone cistern basin",
		LongDescription = null,
		FullDescription = "This limestone cistern basin is a large, workmanlike basin cut from limestone. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 38000.0,
		Cost = 100.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "limestone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Limestone draw basin linked to a managed cistern supply.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterMediterraneanMarbleBath() => new()
	{
		Metadata = new("medieval_water_mediterranean_marble_bath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1211", []),
		Noun = "basin",
		ShortDescription = "a marble bath basin",
		LongDescription = null,
		FullDescription = "This marble bath basin is a very large, well-made basin cut from marble. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 82000.0,
		Cost = 300.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "marble",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy marble bath basin for elite bathhouse or courtyard bathing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterMediterraneanTerracottaCistern() => new()
	{
		Metadata = new("medieval_water_mediterranean_terracotta_cistern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1241", []),
		Noun = "cistern",
		ShortDescription = "a terracotta cistern jar",
		LongDescription = null,
		FullDescription = "This terracotta cistern jar is a large, workmanlike cistern formed from terracotta. A rounded jar body rises to a covered mouth, with a thick shoulder and a steady base. Dark water marks gather below the rim where vessels are filled. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "terracotta",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Infinite_PublicCisternWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large terracotta cistern vessel for warm-climate household water storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterIslamicateAblutionBasin() => new()
	{
		Metadata = new("medieval_water_islamicate_ablution_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1271", []),
		Noun = "basin",
		ShortDescription = "a marble ablution basin",
		LongDescription = null,
		FullDescription = "This marble ablution basin is a large, well-made basin cut from marble. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 42000.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "marble",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Public or courtyard washing basin suited to ablution and household washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterIslamicateBrassWashBasin() => new()
	{
		Metadata = new("medieval_water_islamicate_brass_wash_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1301", []),
		Noun = "basin",
		ShortDescription = "a brass washing basin",
		LongDescription = null,
		FullDescription = "This brass washing basin is a medium-sized, well-made basin worked from brass. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 82.0m,
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
				"Sink_20L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bright metal fillable basin for urban household washing and guest hospitality.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterIslamicateHammamBath() => new()
	{
		Metadata = new("medieval_water_islamicate_hammam_bath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1331", []),
		Noun = "basin",
		ShortDescription = "a hammam bath basin",
		LongDescription = null,
		FullDescription = "This hammam bath basin is a very large, well-made basin cut from stone. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 78000.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large bath basin for bathhouse or elite domestic washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterIslamicateSabilCistern() => new()
	{
		Metadata = new("medieval_water_islamicate_sabil_cistern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1361", []),
		Noun = "cistern",
		ShortDescription = "a public sabil cistern",
		LongDescription = null,
		FullDescription = "This public sabil cistern is a very large, well-made cistern cut from stone. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 64000.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Charitable public cistern or drinking-water fixture for an urban street or courtyard.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSouthAsianBrassWashingBasin() => new()
	{
		Metadata = new("medieval_water_south_asian_brass_washing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1391", []),
		Noun = "basin",
		ShortDescription = "a brass washing basin",
		LongDescription = null,
		FullDescription = "This brass washing basin is a medium-sized, well-made basin worked from brass. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3400.0,
		Cost = 78.0m,
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
				"Sink_20L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal basin for washing, rinsing, and household water service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSouthAsianPublicTrough() => new()
	{
		Metadata = new("medieval_water_south_asian_public_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1421", []),
		Noun = "trough",
		ShortDescription = "a stone public water trough",
		LongDescription = null,
		FullDescription = "This stone public water trough is a very large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 54000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicTroughWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Public water trough for travellers, animals, and household drawing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSouthAsianStepwellDrawBasin() => new()
	{
		Metadata = new("medieval_water_south_asian_stepwell_draw_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1451", []),
		Noun = "basin",
		ShortDescription = "a stepwell draw basin",
		LongDescription = null,
		FullDescription = "This stepwell draw basin is a large, workmanlike basin cut from stone. A raised rim surrounds the draw opening, with worn contact marks where rope and bucket have passed. The top edge is smooth from repeated use. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Draw basin representing a spring-fed or stepwell water source.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSouthAsianTeakBathTub() => new()
	{
		Metadata = new("medieval_water_south_asian_teak_bath_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1481", []),
		Noun = "tub",
		ShortDescription = "a teak bathing tub",
		LongDescription = null,
		FullDescription = "This teak bathing tub is a very large, well-made tub built from teak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 27000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large teak tub for bathing in a warm-climate household or bathhouse.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterEastAsianBambooSpringTrough() => new()
	{
		Metadata = new("medieval_water_east_asian_bamboo_spring_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1511", []),
		Noun = "trough",
		ShortDescription = "a bamboo spring trough",
		LongDescription = null,
		FullDescription = "This bamboo spring trough is a large, workmanlike trough built from split bamboo. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8000.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bamboo-lined trough fed by a small spring or watercourse.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterEastAsianCypressBathTub() => new()
	{
		Metadata = new("medieval_water_east_asian_cypress_bath_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1541", []),
		Noun = "tub",
		ShortDescription = "a cypress bath tub",
		LongDescription = null,
		FullDescription = "This cypress bath tub is a very large, well-made tub built from cypress boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large cypress bath tub for household bathing or inn use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterEastAsianCypressWashBasin() => new()
	{
		Metadata = new("medieval_water_east_asian_cypress_wash_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1571", []),
		Noun = "basin",
		ShortDescription = "a cypress wash basin",
		LongDescription = null,
		FullDescription = "This cypress wash basin is a medium-sized, well-made basin built from cypress boards. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Sink_20L"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Clean wooden wash basin for domestic, inn, or bathhouse use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterEastAsianStoneGardenBasin() => new()
	{
		Metadata = new("medieval_water_east_asian_stone_garden_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1601", []),
		Noun = "basin",
		ShortDescription = "a stone garden water basin",
		LongDescription = null,
		FullDescription = "This stone garden water basin is a large, well-made basin cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 36000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stone water basin suited to a garden, courtyard, or refined household setting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSteppeCampTrough() => new()
	{
		Metadata = new("medieval_water_steppe_camp_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1631", []),
		Noun = "trough",
		ShortDescription = "a camp water trough",
		LongDescription = null,
		FullDescription = "This camp water trough is a large, workmanlike trough built from plain wood. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14000.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicTroughWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable/installable trough for a camp, herd, caravan, or way station.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSteppeCaravanCisternBox() => new()
	{
		Metadata = new("medieval_water_steppe_caravan_cistern_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1661", []),
		Noun = "cistern",
		ShortDescription = "a caravan cistern box",
		LongDescription = null,
		FullDescription = "This caravan cistern box is a large, well-made cistern built from oak boards. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Boxed cistern fixture for a caravanserai yard or mobile-camp support setting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWaterSteppeRiverDrawFrame() => new()
	{
		Metadata = new("medieval_water_steppe_river_draw_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:1691", []),
		Noun = "frame",
		ShortDescription = "a river draw frame",
		LongDescription = null,
		FullDescription = "This river draw frame is a large, workmanlike frame built from plain wood. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10000.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_RiverWaterSource"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light frame marking a controlled river-water drawing point for camp use.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
