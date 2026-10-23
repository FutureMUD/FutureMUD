#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical03Stock
{
	public static CatalogueItemDefinition MedievalTradeSmallSampleCasket() => new()
	{
		Metadata = new("medieval_trade_small_sample_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3723", []),
		Noun = "casket",
		ShortDescription = "a small sample casket",
		LongDescription = null,
		FullDescription = "This small sample casket is a small, well-made casket built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 38.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine small casket for sample stones, jewels, spice lots, and demonstration wares.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeSpiceDisplayTray() => new()
	{
		Metadata = new("medieval_trade_spice_display_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3753", []),
		Noun = "tray",
		ShortDescription = "a cedar spice tray",
		LongDescription = null,
		FullDescription = "This cedar spice tray is a small, well-made tray built from cedar boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
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
		BuilderNotes = "Shallow tray for small bowls, packets, or measured spice samples on a stall.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeStonewareLiddedJar() => new()
	{
		Metadata = new("medieval_trade_stoneware_lidded_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3784", []),
		Noun = "jar",
		ShortDescription = "a stoneware lidded jar",
		LongDescription = null,
		FullDescription = "This stoneware lidded jar is a medium-sized, workmanlike jar formed from stoneware. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2800.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rigid lidded jar for dry powders, salt, spices, seed, and small food stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeTannerBarkBarrel() => new()
	{
		Metadata = new("medieval_trade_tanner_bark_barrel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3814", []),
		Noun = "barrel",
		ShortDescription = "a tanner's bark barrel",
		LongDescription = null,
		FullDescription = "This tanner's bark barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12500.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Coopered barrel for bark, tan, dry lime, and tanning-house material.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeTannerBarkBin() => new()
	{
		Metadata = new("medieval_trade_tanner_bark_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3844", []),
		Noun = "bin",
		ShortDescription = "a tanner's bark bin",
		LongDescription = null,
		FullDescription = "This tanner's bark bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8400.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
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
		BuilderNotes = "Open bin for bark, tan, lime, and other dry tanner's materials.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeTeakCottonHamper() => new()
	{
		Metadata = new("medieval_trade_teak_cotton_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3875", []),
		Noun = "hamper",
		ShortDescription = "a teak cotton hamper",
		LongDescription = null,
		FullDescription = "This teak cotton hamper is a large, well-made hamper built from teak boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Sturdier open hamper for cotton bales, dyed cloth, and workshop textiles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeTerracottaGrainJar() => new()
	{
		Metadata = new("medieval_trade_terracotta_grain_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3906", []),
		Noun = "jar",
		ShortDescription = "a terracotta grain jar",
		LongDescription = null,
		FullDescription = "This terracotta grain jar is a large, workmanlike jar formed from terracotta. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9000.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "terracotta",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Small_Drum"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fired storage jar for grain, pulses, dried fruit, and dry household or market goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeVentilatedFishCrate() => new()
	{
		Metadata = new("medieval_trade_ventilated_fish_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3936", []),
		Noun = "crate",
		ShortDescription = "a ventilated fish crate",
		LongDescription = null,
		FullDescription = "This ventilated fish crate is a medium-sized, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
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
		BuilderNotes = "Open crate for fish, shellfish, salted catch bundles, or damp market goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWagonCargoChest() => new()
	{
		Metadata = new("medieval_trade_wagon_cargo_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3967", []),
		Noun = "chest",
		ShortDescription = "a wagon cargo chest",
		LongDescription = null,
		FullDescription = "This wagon cargo chest is a large, workmanlike chest built from ash boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 13200.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Road-travel chest for wagon freight, wrapped tools, merchant stores, and estate goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWalnutMerchantBox() => new()
	{
		Metadata = new("medieval_trade_walnut_merchant_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:3997", []),
		Noun = "box",
		ShortDescription = "a walnut merchant box",
		LongDescription = null,
		FullDescription = "This walnut merchant box is a medium-sized, well-made box built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
		BuilderNotes = "Better-finished trade box for valuable packets, ledgers, seals, and merchant samples.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWaxBlockCrate() => new()
	{
		Metadata = new("medieval_trade_wax_block_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4027", []),
		Noun = "crate",
		ShortDescription = "a wax-block crate",
		LongDescription = null,
		FullDescription = "This wax-block crate is a medium-sized, workmanlike crate built from beech boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Open crate for beeswax blocks, tallow stock, seal wax, and candle-making materials.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWaxTabletShippingBox() => new()
	{
		Metadata = new("medieval_trade_wax_tablet_shipping_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4058", []),
		Noun = "box",
		ShortDescription = "a wax-tablet shipping box",
		LongDescription = null,
		FullDescription = "This wax-tablet shipping box is a medium-sized, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Writing Materials / Document Containers",
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
		BuilderNotes = "Rigid box for writing tablets, samples, small books, or other flat trade goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWaxedCanvasSpiceSack() => new()
	{
		Metadata = new("medieval_trade_waxed_canvas_spice_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4089", []),
		Noun = "sack",
		ShortDescription = "a waxed canvas spice sack",
		LongDescription = null,
		FullDescription = "This waxed canvas spice sack is a small, well-made sack made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wax-treated soft container for valuable dry spices, resins, aromatics, or dyestuffs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWaxedTradeBarrel() => new()
	{
		Metadata = new("medieval_trade_waxed_trade_barrel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4120", []),
		Noun = "barrel",
		ShortDescription = "a waxed trade barrel",
		LongDescription = null,
		FullDescription = "This waxed trade barrel is a large, well-made barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 15000.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-sealed barrel for dry goods that must be protected from damp; still not a liquid container.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWillowMarketBasket() => new()
	{
		Metadata = new("medieval_trade_willow_market_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4151", []),
		Noun = "basket",
		ShortDescription = "a willow market basket",
		LongDescription = null,
		FullDescription = "This willow market basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
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
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
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
		BuilderNotes = "Open basket for loose produce, small parcels, and market-table goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWoodenPowderTub() => new()
	{
		Metadata = new("medieval_trade_wooden_powder_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4182", []),
		Noun = "tub",
		ShortDescription = "a wooden powder tub",
		LongDescription = null,
		FullDescription = "This wooden powder tub is a medium-sized, workmanlike tub built from beech boards. Staves form a broad open vessel with a flat bottom and a thick rim. The sides flare slightly, leaving the inside easy to reach. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
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
		BuilderNotes = "Open tub for dry powders, grain, meal, plaster, pigments, or sorted workshop material.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWoolFleeceSack() => new()
	{
		Metadata = new("medieval_trade_wool_fleece_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4213", []),
		Noun = "sack",
		ShortDescription = "a wool fleece sack",
		LongDescription = null,
		FullDescription = "This wool fleece sack is a medium-sized, workmanlike sack made from woven wool. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 640.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft sack for loose fleeces, wool locks, cloth scraps, and other compressible textile goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWoolHamper() => new()
	{
		Metadata = new("medieval_trade_wool_hamper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4244", []),
		Noun = "hamper",
		ShortDescription = "a wool sorting hamper",
		LongDescription = null,
		FullDescription = "This wool sorting hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Large open hamper for sorting fleeces, cloth scraps, raw wool, and bulky textile goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWoolSortingSack() => new()
	{
		Metadata = new("medieval_trade_wool_sorting_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4275", []),
		Noun = "sack",
		ShortDescription = "a wool sorting sack",
		LongDescription = null,
		FullDescription = "This wool sorting sack is a medium-sized, workmanlike sack made from woven wool. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft sack for separating clean wool, waste fleece, combings, and sorted textile fibre.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeWroughtIronNailBin() => new()
	{
		Metadata = new("medieval_trade_wrought_iron_nail_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4306", []),
		Noun = "bin",
		ShortDescription = "a wrought-iron nail bin",
		LongDescription = null,
		FullDescription = "This wrought-iron nail bin is a small, workmanlike bin worked from wrought iron. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Open_Bin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small metal bin for nails, tacks, rivets, clamps, and other dense hardware.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeDocksideBondChest() => new()
	{
		Metadata = new("medieval_locking_trade_dockside_bond_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4337", []),
		Noun = "chest",
		ShortDescription = "a dockside bond chest",
		LongDescription = null,
		FullDescription = "This dockside bond chest is a medium-sized, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 21000.0,
		Cost = 270.0m,
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
		BuilderNotes = "Locking chest for port bonds, tally sticks, and cargo records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeBazaarCashBox() => new()
	{
		Metadata = new("medieval_locking_trade_bazaar_cash_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4367", []),
		Noun = "lockbox",
		ShortDescription = "a bazaar cash lockbox",
		LongDescription = null,
		FullDescription = "This bazaar cash lockbox is a small, well-made lockbox worked from brass. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 135.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
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
		BuilderNotes = "Metal lockbox for shop counters and market stalls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeTeakSpiceChest() => new()
	{
		Metadata = new("medieval_locking_trade_teak_spice_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4397", []),
		Noun = "chest",
		ShortDescription = "a teak spice chest",
		LongDescription = null,
		FullDescription = "This teak spice chest is a medium-sized, well-made chest built from teak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 280.0m,
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
		BuilderNotes = "Locking teak chest for valuable spices, dyes, or aromatics.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeCedarMerchantCasket() => new()
	{
		Metadata = new("medieval_locking_trade_cedar_merchant_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4427", []),
		Noun = "casket",
		ShortDescription = "a cedar merchant casket",
		LongDescription = null,
		FullDescription = "This cedar merchant casket is a small, well-made casket built from cedar boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 3000.0,
		Cost = 115.0m,
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
		BuilderNotes = "Small locking casket for seals, paper money substitutes, tallies, or compact valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeCaravanPayChest() => new()
	{
		Metadata = new("medieval_locking_trade_caravan_pay_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4457", []),
		Noun = "chest",
		ShortDescription = "a caravan pay chest",
		LongDescription = null,
		FullDescription = "This caravan pay chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 270.0m,
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
		BuilderNotes = "Locking pay chest for caravans, guards, and merchants on the road.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalAngloDanishBronzePinnedTradeCoffer() => new()
	{
		Metadata = new("medieval_regional_anglo_danish_bronze_pinned_trade_coffer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4487", []),
		Noun = "coffer",
		ShortDescription = "a bronze-pinned trade coffer",
		LongDescription = null,
		FullDescription = "This bronze-pinned trade coffer is a large, well-made coffer built from birch boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 11800.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Regional addition: North-Sea style coffer with metal pins for trade goods and household stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalAngloNormanPanelledLedgerChest() => new()
	{
		Metadata = new("medieval_regional_anglo_norman_panelled_ledger_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4517", []),
		Noun = "chest",
		ShortDescription = "a panelled ledger chest",
		LongDescription = null,
		FullDescription = "This panelled ledger chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 12200.0,
		Cost = 86.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Regional addition: panelled chest for ledgers, writs, stall records, and cloth or coin packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalCarolingianBandedBeechGoodsChest() => new()
	{
		Metadata = new("medieval_regional_carolingian_banded_beech_goods_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4548", []),
		Noun = "chest",
		ShortDescription = "a banded beech goods chest",
		LongDescription = null,
		FullDescription = "This banded beech goods chest is a large, workmanlike chest built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 11600.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Regional addition: banded storage chest for hall goods, dues, provisions, and court supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalChristianIberianOlivewoodSaffronBox() => new()
	{
		Metadata = new("medieval_regional_christian_iberian_olivewood_saffron_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4578", []),
		Noun = "box",
		ShortDescription = "an olive-wood saffron box",
		LongDescription = null,
		FullDescription = "This olive-wood saffron box is a small, well-made box built from olive wood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "olive wood",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
		BuilderNotes = "Regional addition: fine small box for saffron, coin, spices, and luxury packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalEarlyAngloSaxonAshTallyChest() => new()
	{
		Metadata = new("medieval_regional_early_anglo_saxon_ash_tally_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4608", []),
		Noun = "chest",
		ShortDescription = "an ash tally chest",
		LongDescription = null,
		FullDescription = "This ash tally chest is a large, workmanlike chest built from ash boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10200.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Regional addition: plain dry chest suitable for estate tallies, dues, and local market goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalFatimidLinenCaravanBale() => new()
	{
		Metadata = new("medieval_regional_fatimid_linen_caravan_bale", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4638", []),
		Noun = "bale",
		ShortDescription = "a linen caravan bale",
		LongDescription = null,
		FullDescription = "This linen caravan bale is a large, workmanlike bale made from woven linen. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional addition: light bale wrapper for cloth, packets, and caravan-packed market goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalGoryeoPineMerchantChest() => new()
	{
		Metadata = new("medieval_regional_goryeo_pine_merchant_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4669", []),
		Noun = "chest",
		ShortDescription = "a pine merchant chest",
		LongDescription = null,
		FullDescription = "This pine merchant chest is a large, well-made chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 9800.0,
		Cost = 66.0m,
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
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional addition: low, portable chest for cloth, tea, papers, and market goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalHeianCypressDocumentBox() => new()
	{
		Metadata = new("medieval_regional_heian_cypress_document_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4699", []),
		Noun = "box",
		ShortDescription = "a cypress document box",
		LongDescription = null,
		FullDescription = "This cypress document box is a medium-sized, well-made box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2200.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Writing Materials / Document Containers",
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
		BuilderNotes = "Regional addition: long dry box for documents, scrolls, textiles, and small elite stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalHighEnglishWoolStapleChest() => new()
	{
		Metadata = new("medieval_regional_high_english_wool_staple_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4730", []),
		Noun = "chest",
		ShortDescription = "a wool-staple trade chest",
		LongDescription = null,
		FullDescription = "This wool-staple trade chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 14800.0,
		Cost = 92.0m,
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
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional addition: trade chest for graded wool, cloth bolts, tallies, and merchant markings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalGermanStonewareMerchantJar() => new()
	{
		Metadata = new("medieval_regional_german_stoneware_merchant_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4760", []),
		Noun = "jar",
		ShortDescription = "a stoneware merchant jar",
		LongDescription = null,
		FullDescription = "This stoneware merchant jar is a medium-sized, well-made jar formed from stoneware. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional addition: tough lidded jar for dry stock, salt, dyestuffs, and market goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalIrishHideBoundProvisionChest() => new()
	{
		Metadata = new("medieval_regional_irish_hide_bound_provision_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4790", []),
		Noun = "chest",
		ShortDescription = "a hide-bound provision chest",
		LongDescription = null,
		FullDescription = "This hide-bound provision chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12500.0,
		Cost = 58.0m,
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
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional addition: rugged chest with hide binding for provisions, wool, and travel stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalMagyarBronzeMountedSaddleCrate() => new()
	{
		Metadata = new("medieval_regional_magyar_bronze_mounted_saddle_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4820", []),
		Noun = "crate",
		ShortDescription = "a bronze-mounted saddle crate",
		LongDescription = null,
		FullDescription = "This bronze-mounted saddle crate is a medium-sized, well-made crate built from ash boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Regional addition: rigid mounted-travel crate for camp goods and mobile trade lots.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalNorseShipboardSeaChest() => new()
	{
		Metadata = new("medieval_regional_norse_shipboard_sea_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4851", []),
		Noun = "chest",
		ShortDescription = "a shipboard sea chest",
		LongDescription = null,
		FullDescription = "This shipboard sea chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12800.0,
		Cost = 64.0m,
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
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional addition: travel chest suited to boat, shore, and cargo handling.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalRusBirchMerchantBox() => new()
	{
		Metadata = new("medieval_regional_rus_birch_merchant_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4881", []),
		Noun = "box",
		ShortDescription = "a birch merchant box",
		LongDescription = null,
		FullDescription = "This birch merchant box is a medium-sized, workmanlike box built from birch boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
		BuilderNotes = "Regional addition: light lidded box for furs, wax, tablets, and river-trade parcels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalScottishWoolStapleChest() => new()
	{
		Metadata = new("medieval_regional_scottish_wool_staple_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4911", []),
		Noun = "chest",
		ShortDescription = "an iron-bound wool chest",
		LongDescription = null,
		FullDescription = "This iron-bound wool chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 15000.0,
		Cost = 88.0m,
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
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional addition: heavier chest for wool staple goods, cloth, and upland trade stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalSeljukWalnutCaravanChest() => new()
	{
		Metadata = new("medieval_regional_seljuk_walnut_caravan_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4941", []),
		Noun = "chest",
		ShortDescription = "a walnut caravan chest",
		LongDescription = null,
		FullDescription = "This walnut caravan chest is a large, well-made chest built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 11800.0,
		Cost = 90.0m,
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
				"Container_Trunk"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional addition: travel chest for caravan goods, arms fittings, cloth, and compact valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalSongBambooTeaCrate() => new()
	{
		Metadata = new("medieval_regional_song_bamboo_tea_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:4971", []),
		Noun = "crate",
		ShortDescription = "a bamboo tea crate",
		LongDescription = null,
		FullDescription = "This bamboo tea crate is a medium-sized, well-made crate built from split bamboo. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1600.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
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
		BuilderNotes = "Regional addition: light crate for tea cakes, packets, porcelain stock, or shop goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRegionalSteppeFeltWrappedCargoBundle() => new()
	{
		Metadata = new("medieval_regional_steppe_felt_wrapped_cargo_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5002", []),
		Noun = "bundle",
		ShortDescription = "a felt-wrapped cargo bundle",
		LongDescription = null,
		FullDescription = "This felt-wrapped cargo bundle is a large, workmanlike bundle made from pressed felt. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional addition: soft wrapped cargo bundle for mobile camps, pack animals, and caravan goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticAlmsPouch() => new()
	{
		Metadata = new("medieval_domestic_alms_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5033", []),
		Noun = "pouch",
		ShortDescription = "an alms pouch",
		LongDescription = null,
		FullDescription = "This alms pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small belt pouch used for alms coins, tokens, or modest household charity money.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticAlmsPurse() => new()
	{
		Metadata = new("medieval_domestic_alms_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5065", []),
		Noun = "purse",
		ShortDescription = "an alms purse",
		LongDescription = null,
		FullDescription = "This alms purse is a very small, workmanlike purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Personal purse for alms money, tokens, or small household distributions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticAshDocumentCase() => new()
	{
		Metadata = new("medieval_domestic_ash_document_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5097", []),
		Noun = "case",
		ShortDescription = "an ash document case",
		LongDescription = null,
		FullDescription = "This ash document case is a small, workmanlike case built from ash boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rigid personal case for folded records, small charters, tally slips, and protected documents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBambooBackBasket() => new()
	{
		Metadata = new("medieval_domestic_bamboo_back_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5128", []),
		Noun = "basket",
		ShortDescription = "a bamboo back basket",
		LongDescription = null,
		FullDescription = "This bamboo back basket is a medium-sized, workmanlike basket built from split bamboo. The body is woven around a firm rim, with a flat base and open mouth. The weave tightens at the lower corners where the load bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light rigid back basket for warm-climate household carrying and produce loads.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBambooQuiver() => new()
	{
		Metadata = new("medieval_domestic_bamboo_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5159", []),
		Noun = "quiver",
		ShortDescription = "a bamboo quiver",
		LongDescription = null,
		FullDescription = "This bamboo quiver is a small, workmanlike quiver built from split bamboo. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light quiver for arrows, slim canes, rolled papers, or craft rods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBambooScrollTube() => new()
	{
		Metadata = new("medieval_domestic_bamboo_scroll_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5190", []),
		Noun = "tube",
		ShortDescription = "a bamboo scroll tube",
		LongDescription = null,
		FullDescription = "This bamboo scroll tube is a small, workmanlike tube built from split bamboo. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light tubular case for rolled documents, patterns, maps, or scrolls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBambooShoulderBasket() => new()
	{
		Metadata = new("medieval_domestic_bamboo_shoulder_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5221", []),
		Noun = "basket",
		ShortDescription = "a bamboo shoulder basket",
		LongDescription = null,
		FullDescription = "This bamboo shoulder basket is a small, workmanlike basket built from split bamboo. The body is woven around a firm rim, with a flat base and open mouth. The weave tightens at the lower corners where the load bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light rigid basket worn from the shoulder for warm-climate domestic errands.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBasketBackpack() => new()
	{
		Metadata = new("medieval_domestic_basket_backpack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5252", []),
		Noun = "pack",
		ShortDescription = "a basket backpack",
		LongDescription = null,
		FullDescription = "This basket backpack is a medium-sized, workmanlike pack built from willow boards. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1050.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Woven back basket for vegetables, laundry, kindling, or household carrying.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBeadBox() => new()
	{
		Metadata = new("medieval_domestic_bead_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5283", []),
		Noun = "box",
		ShortDescription = "a bead box",
		LongDescription = null,
		FullDescription = "This bead box is a very small, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small lidded box for beads, toggles, buttons, and delicate household ornaments.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBeadSachet() => new()
	{
		Metadata = new("medieval_domestic_bead_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5313", []),
		Noun = "sachet",
		ShortDescription = "a bead sachet",
		LongDescription = null,
		FullDescription = "This bead sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small sachet for loose beads, buttons, charms, or game counters.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBeadedPurse() => new()
	{
		Metadata = new("medieval_domestic_beaded_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5345", []),
		Noun = "purse",
		ShortDescription = "a beaded purse",
		LongDescription = null,
		FullDescription = "This beaded purse is a very small, well-made purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 95.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorated purse for feast-day coins, beads, and small valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBeadworkPouch() => new()
	{
		Metadata = new("medieval_domestic_beadwork_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5377", []),
		Noun = "pouch",
		ShortDescription = "a beadwork pouch",
		LongDescription = null,
		FullDescription = "This beadwork pouch is a very small, well-made pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 100.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine pouch with decorative beadwork for jewellery, beads, or feast-day personal items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBeeswaxPouch() => new()
	{
		Metadata = new("medieval_domestic_beeswax_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5409", []),
		Noun = "pouch",
		ShortDescription = "a beeswax pouch",
		LongDescription = null,
		FullDescription = "This beeswax pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 100.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small pouch for wax lumps, sealing scraps, candle ends, or protected sticky goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBeltQuiver() => new()
	{
		Metadata = new("medieval_domestic_belt_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5440", []),
		Noun = "quiver",
		ShortDescription = "a leather belt quiver",
		LongDescription = null,
		FullDescription = "This leather belt quiver is a small, workmanlike quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Belt-attached quiver for arrows, bolts, bodkins, or household rod-like tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBeltedRingPurse() => new()
	{
		Metadata = new("medieval_domestic_belted_ring_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5471", []),
		Noun = "purse",
		ShortDescription = "a belted ring purse",
		LongDescription = null,
		FullDescription = "This belted ring purse is a very small, workmanlike purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small purse for rings, pins, tokens, and modest personal valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBirchScrollTube() => new()
	{
		Metadata = new("medieval_domestic_birch_scroll_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5502", []),
		Noun = "tube",
		ShortDescription = "a birch scroll tube",
		LongDescription = null,
		FullDescription = "This birch scroll tube is a small, workmanlike tube built from birch boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tubular case for rolled paper, parchment, household patterns, or written instructions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBlanketRollPack() => new()
	{
		Metadata = new("medieval_domestic_blanket_roll_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5533", []),
		Noun = "pack",
		ShortDescription = "a blanket-roll pack",
		LongDescription = null,
		FullDescription = "This blanket-roll pack is a medium-sized, workmanlike pack made from woven wool. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980.0,
		Cost = 15.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pack shaped around a rolled blanket or cloak with space for small personal goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBoneLaceCase() => new()
	{
		Metadata = new("medieval_domestic_bone_lace_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5565", []),
		Noun = "case",
		ShortDescription = "a bone lace case",
		LongDescription = null,
		FullDescription = "This bone lace case is a tiny, well-made case worked from bone. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 60.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small rigid case for lace needles, fine hooks, and delicate trim.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBoneNeedleCase() => new()
	{
		Metadata = new("medieval_domestic_bone_needle_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5596", []),
		Noun = "case",
		ShortDescription = "a bone needle case",
		LongDescription = null,
		FullDescription = "This bone needle case is a tiny, well-made case worked from bone. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hard little case for needles, fine pins, small hooks, or delicate sewing pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBookSatchel() => new()
	{
		Metadata = new("medieval_domestic_book_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5627", []),
		Noun = "satchel",
		ShortDescription = "a book satchel",
		LongDescription = null,
		FullDescription = "This book satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 600.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stiffened satchel for books, folded papers, or protected household records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBoxwoodPenCase() => new()
	{
		Metadata = new("medieval_domestic_boxwood_pen_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5658", []),
		Noun = "case",
		ShortDescription = "a boxwood pen case",
		LongDescription = null,
		FullDescription = "This boxwood pen case is a very small, well-made case built from boxwood. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 120.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "boxwood",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow case for reed pens, small brushes, stylus pieces, or writing tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticBroadWaistPurse() => new()
	{
		Metadata = new("medieval_domestic_broad_waist_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5689", []),
		Noun = "purse",
		ShortDescription = "a broad waist purse",
		LongDescription = null,
		FullDescription = "This broad waist purse is a small, well-made purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. The opening and interior are plain to see, making the storage space easy to inspect. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 240.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Wear_Waist"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broader purse worn at the waist for coins, keys, compact writing pieces, or small valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticButtonAndTogglePouch() => new()
	{
		Metadata = new("medieval_domestic_button_and_toggle_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5720", []),
		Noun = "pouch",
		ShortDescription = "a toggle-and-button pouch",
		LongDescription = null,
		FullDescription = "This toggle-and-button pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small pouch for toggles, buttons, clasp pieces, pins, and clothing repair odds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCandleLighterPouch() => new()
	{
		Metadata = new("medieval_domestic_candle_lighter_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5752", []),
		Noun = "pouch",
		ShortDescription = "a candle-lighter pouch",
		LongDescription = null,
		FullDescription = "This candle-lighter pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pouch for tinder, wick ends, candle stubs, and small lighting tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCanvasErrandSatchel() => new()
	{
		Metadata = new("medieval_domestic_canvas_errand_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5783", []),
		Noun = "satchel",
		ShortDescription = "a canvas errand satchel",
		LongDescription = null,
		FullDescription = "This canvas errand satchel is a small, workmanlike satchel made from coarse canvas. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 430.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hard-wearing satchel for errands, market goods, mending, and daily carrying.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCanvasFlapPouch() => new()
	{
		Metadata = new("medieval_domestic_canvas_flap_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5815", []),
		Noun = "pouch",
		ShortDescription = "a canvas flap pouch",
		LongDescription = null,
		FullDescription = "This canvas flap pouch is a very small, workmanlike pouch made from coarse canvas. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stouter cloth pouch with a flap for tools, mending gear, or spare fastenings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCanvasToolRoll() => new()
	{
		Metadata = new("medieval_domestic_canvas_tool_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5847", []),
		Noun = "roll",
		ShortDescription = "a canvas tool roll",
		LongDescription = null,
		FullDescription = "This canvas tool roll is a small, workmanlike roll made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 250.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Roll-up carrier for small tools, awls, knives, pins, and repair pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCanvasTravelPack() => new()
	{
		Metadata = new("medieval_domestic_canvas_travel_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5879", []),
		Noun = "pack",
		ShortDescription = "a canvas travel pack",
		LongDescription = null,
		FullDescription = "This canvas travel pack is a medium-sized, workmanlike pack made from coarse canvas. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 820.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stout pack for domestic travel, errands, and household removals.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCedarScentCase() => new()
	{
		Metadata = new("medieval_domestic_cedar_scent_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5911", []),
		Noun = "case",
		ShortDescription = "a cedar scent case",
		LongDescription = null,
		FullDescription = "This cedar scent case is a very small, well-made case built from cedar boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 140.0,
		Cost = 20.0m,
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
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small cedar case for scented packets, combs, pins, or dressing-table goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCharcoalDustPacket() => new()
	{
		Metadata = new("medieval_domestic_charcoal_dust_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5942", []),
		Noun = "packet",
		ShortDescription = "a charcoal-dust packet",
		LongDescription = null,
		FullDescription = "This charcoal-dust packet is a tiny, workmanlike packet made from layered paper. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tiny packet for charcoal dust, lamp-black, sketching powder, or household marking material.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCharmPouch() => new()
	{
		Metadata = new("medieval_domestic_charm_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:5974", []),
		Noun = "pouch",
		ShortDescription = "a charm pouch",
		LongDescription = null,
		FullDescription = "This charm pouch is a very small, workmanlike pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small drawstring pouch for tokens, charms, keepsakes, or domestic devotional pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticChildCarryBag() => new()
	{
		Metadata = new("medieval_domestic_child_carry_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6006", []),
		Noun = "bag",
		ShortDescription = "a small carry bag",
		LongDescription = null,
		FullDescription = "This small carry bag is a small, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small shoulder bag for a child or servant carrying food, cloth, or little household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticChildToyPouch() => new()
	{
		Metadata = new("medieval_domestic_child_toy_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6038", []),
		Noun = "pouch",
		ShortDescription = "a child's toy pouch",
		LongDescription = null,
		FullDescription = "This child's toy pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small pouch for toys, counters, shells, beads, and harmless keepsakes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticClaspedPurse() => new()
	{
		Metadata = new("medieval_domestic_clasped_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6070", []),
		Noun = "purse",
		ShortDescription = "a clasped purse",
		LongDescription = null,
		FullDescription = "This clasped purse is a very small, well-made purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 125.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-made purse with a visible clasp for household valuables and personal keepsakes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticClaspedTabletWallet() => new()
	{
		Metadata = new("medieval_domestic_clasped_tablet_wallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6101", []),
		Noun = "wallet",
		ShortDescription = "a clasped tablet wallet",
		LongDescription = null,
		FullDescription = "This clasped tablet wallet is a small, well-made wallet made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 280.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stiff wallet for wax tablets, small notes, stylus pieces, and account slips.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCombCase() => new()
	{
		Metadata = new("medieval_domestic_comb_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6132", []),
		Noun = "case",
		ShortDescription = "a leather comb case",
		LongDescription = null,
		FullDescription = "This leather comb case is a very small, workmanlike case made from worked leather. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Slim case for a comb, hair sticks, pins, or small grooming implements.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCombPouch() => new()
	{
		Metadata = new("medieval_domestic_comb_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6163", []),
		Noun = "pouch",
		ShortDescription = "a comb pouch",
		LongDescription = null,
		FullDescription = "This comb pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow pouch suited to a comb, hair pins, small grooming tools, or wrapped personal pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCosmeticBox() => new()
	{
		Metadata = new("medieval_domestic_cosmetic_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6194", []),
		Noun = "box",
		ShortDescription = "a cosmetic box",
		LongDescription = null,
		FullDescription = "This cosmetic box is a small, well-made box built from sandalwood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360.0,
		Cost = 36.0m,
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
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small fragrant-wood box for powders, combs, scent sachets, and personal dressing goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticCosmeticSachet() => new()
	{
		Metadata = new("medieval_domestic_cosmetic_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6224", []),
		Noun = "sachet",
		ShortDescription = "a cosmetic sachet",
		LongDescription = null,
		FullDescription = "This cosmetic sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small sachet for powdered cosmetic, scent, or dressing-table matter.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticDoubleStrapWaterskin() => new()
	{
		Metadata = new("medieval_domestic_double_strap_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6256", []),
		Noun = "waterskin",
		ShortDescription = "a double-strap waterskin",
		LongDescription = null,
		FullDescription = "This double-strap waterskin is a medium-sized, well-made waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 760.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-strapped waterskin for longer journeys or shared household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticDrawstringPurse() => new()
	{
		Metadata = new("medieval_domestic_drawstring_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6288", []),
		Noun = "purse",
		ShortDescription = "a drawstring purse",
		LongDescription = null,
		FullDescription = "This drawstring purse is a very small, workmanlike purse made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft drawstring purse for small change, rings, counters, or tokens.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticDriedFlowerSachet() => new()
	{
		Metadata = new("medieval_domestic_dried_flower_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6320", []),
		Noun = "sachet",
		ShortDescription = "a dried-flower sachet",
		LongDescription = null,
		FullDescription = "This dried-flower sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small scent sachet for folded clothing, bedding, or fine storage chests.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticDriedHerbSachet() => new()
	{
		Metadata = new("medieval_domestic_dried_herb_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6352", []),
		Noun = "sachet",
		ShortDescription = "a dried-herb sachet",
		LongDescription = null,
		FullDescription = "This dried-herb sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tiny sachet for dried household herbs and scenting mixtures.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticDyestuffSachet() => new()
	{
		Metadata = new("medieval_domestic_dyestuff_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6384", []),
		Noun = "sachet",
		ShortDescription = "a dyestuff sachet",
		LongDescription = null,
		FullDescription = "This dyestuff sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small sachet for a pinch of dye, pigment, alum, or textile-working powder.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticEmbroideredAlmsPurse() => new()
	{
		Metadata = new("medieval_domestic_embroidered_alms_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6416", []),
		Noun = "purse",
		ShortDescription = "an embroidered alms purse",
		LongDescription = null,
		FullDescription = "This embroidered alms purse is a very small, well-made purse made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 85.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine purse for alms money, prayer tokens, and feast-day coins.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticFeltSaddlePack() => new()
	{
		Metadata = new("medieval_domestic_felt_saddle_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6448", []),
		Noun = "pack",
		ShortDescription = "a felt saddle pack",
		LongDescription = null,
		FullDescription = "This felt saddle pack is a medium-sized, workmanlike pack made from pressed felt. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft felt pack for travel goods; suitable as a carried domestic pack even when not on a mount.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticFineSilkPouch() => new()
	{
		Metadata = new("medieval_domestic_fine_silk_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6480", []),
		Noun = "pouch",
		ShortDescription = "a fine silk pouch",
		LongDescription = null,
		FullDescription = "This fine silk pouch is a very small, well-made pouch made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 50.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light elite pouch for jewellery, scent packets, small letters, or formal household tokens.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticFisherNetBag() => new()
	{
		Metadata = new("medieval_domestic_fisher_net_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6512", []),
		Noun = "bag",
		ShortDescription = "a net-carrying bag",
		LongDescription = null,
		FullDescription = "This net-carrying bag is a medium-sized, workmanlike bag made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough pack for nets, cord, hooks, floats, and domestic fishing gear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticFlatTokenPurse() => new()
	{
		Metadata = new("medieval_domestic_flat_token_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6544", []),
		Noun = "purse",
		ShortDescription = "a flat token purse",
		LongDescription = null,
		FullDescription = "This flat token purse is a very small, workmanlike purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Flat cloth purse for market tokens, alms coins, or small counters.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticFleeceLinedPouch() => new()
	{
		Metadata = new("medieval_domestic_fleece_lined_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6576", []),
		Noun = "pouch",
		ShortDescription = "a fleece-lined pouch",
		LongDescription = null,
		FullDescription = "This fleece-lined pouch is a very small, well-made pouch made from dressed fur. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 120.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft-lined pouch for delicate ornaments, beads, or small precious objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticFoldedLinenCarrier() => new()
	{
		Metadata = new("medieval_domestic_folded_linen_carrier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6607", []),
		Noun = "carrier",
		ShortDescription = "a folded linen carrier",
		LongDescription = null,
		FullDescription = "This folded linen carrier is a small, workmanlike carrier made from woven linen. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light carrier for folded linen, napkins, mending, or wrapped parcels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticFoldingSewingWallet() => new()
	{
		Metadata = new("medieval_domestic_folding_sewing_wallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6639", []),
		Noun = "wallet",
		ShortDescription = "a folding sewing wallet",
		LongDescription = null,
		FullDescription = "This folding sewing wallet is a small, workmanlike wallet made from woven linen. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Folding wallet for needles, small scissors, thread, patches, and pins.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticForagerPack() => new()
	{
		Metadata = new("medieval_domestic_forager_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6671", []),
		Noun = "pack",
		ShortDescription = "a forager pack",
		LongDescription = null,
		FullDescription = "This forager pack is a medium-sized, workmanlike pack made from woven hemp. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 11.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough back pack for gathered produce, herbs, mushrooms, or kindling.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticForagingShoulderBag() => new()
	{
		Metadata = new("medieval_domestic_foraging_shoulder_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6703", []),
		Noun = "bag",
		ShortDescription = "a foraging shoulder bag",
		LongDescription = null,
		FullDescription = "This foraging shoulder bag is a small, workmanlike bag made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 340.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough shoulder bag for gathered herbs, kindling, mushrooms, roots, or household foraging.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticFramePack() => new()
	{
		Metadata = new("medieval_domestic_frame_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6735", []),
		Noun = "pack",
		ShortDescription = "a wooden frame pack",
		LongDescription = null,
		FullDescription = "This wooden frame pack is a medium-sized, workmanlike pack built from ash boards. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1900.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rigid-framed pack for larger domestic loads, bundles, and awkward household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticFurLinedPouch() => new()
	{
		Metadata = new("medieval_domestic_fur_lined_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6766", []),
		Noun = "pouch",
		ShortDescription = "a fur-lined pouch",
		LongDescription = null,
		FullDescription = "This fur-lined pouch is a very small, well-made pouch made from dressed fur. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 130.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft-lined pouch for fragile beads, polished stones, small metalwork, or delicate personal objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticGamePiecePouch() => new()
	{
		Metadata = new("medieval_domestic_game_piece_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6797", []),
		Noun = "pouch",
		ShortDescription = "a game-piece pouch",
		LongDescription = null,
		FullDescription = "This game-piece pouch is a very small, workmanlike pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 75.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft pouch for dice, counters, game pieces, knucklebones, or similar small objects.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
