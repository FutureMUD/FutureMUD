#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Authored03Stock
{































































	public static CatalogueItemDefinition PreindustrialLeisureBoneDicePair() => new()
	{
		Metadata = new("preindustrial_leisure_bone_dice_pair", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "dice",
		ShortDescription = "a pair of bone dice",
		LongDescription = null,
		FullDescription = "Two small bone cubes are marked with dark pips for games, wagers, and casual table play.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30,
		Cost = 2.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Dice_d6"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial dice using the existing die component.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureDiceCup() => new()
	{
		Metadata = new("preindustrial_leisure_dice_cup", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "cup",
		ShortDescription = "a leather dice cup",
		LongDescription = null,
		FullDescription = "A stiff leather cup has a weighted base and rolled rim for shaking dice before a throw.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial dice accessory.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureNineMensMorrisBoard() => new()
	{
		Metadata = new("preindustrial_leisure_nine_mens_morris_board", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "board",
		ShortDescription = "a nine men's morris board",
		LongDescription = null,
		FullDescription = "A square wooden board is marked with nested lines and intersections for a traditional counter game.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1400,
		Cost = 12.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial board-game surface; game resolution remains social or builder-led.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureDraughtsBoard() => new()
	{
		Metadata = new("preindustrial_leisure_draughts_board", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "board",
		ShortDescription = "a chequered draughts board",
		LongDescription = null,
		FullDescription = "A flat wooden board is divided into alternating dark and light squares for counters and simple positional play.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300,
		Cost = 12.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial board-game surface; game resolution remains social or builder-led.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureChessBoard() => new()
	{
		Metadata = new("preindustrial_leisure_chess_board", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "board",
		ShortDescription = "a carved chess board",
		LongDescription = null,
		FullDescription = "A folding wooden board has a chequered face and shallow interior for a set of carved playing pieces.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1900,
		Cost = 28.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Container_PreIndustrial_CompartmentBox"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial strategy-game board; game resolution remains social or builder-led.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureBackgammonBoard() => new()
	{
		Metadata = new("preindustrial_leisure_backgammon_board", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "board",
		ShortDescription = "an inlaid race-game board",
		LongDescription = null,
		FullDescription = "A folding board is inlaid with pointed tracks and a divided interior for dice and race counters.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800,
		Cost = 26.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Container_PreIndustrial_CompartmentBox"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial race-game board; game resolution remains social or builder-led.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureWoodenCounters() => new()
	{
		Metadata = new("preindustrial_leisure_wooden_counters", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "counters",
		ShortDescription = "a pouch of wooden game counters",
		LongDescription = null,
		FullDescription = "A drawstring leather pouch holds smooth wooden counters for boards, races, tallies, and simple games.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Container_Pouch"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial game counters.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureSpinningTop() => new()
	{
		Metadata = new("preindustrial_leisure_spinning_top", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "top",
		ShortDescription = "a painted spinning top",
		LongDescription = null,
		FullDescription = "A small painted wooden top has a pointed iron tip and a wrapped cord for quick spinning games.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120,
		Cost = 3.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial toy.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureHoop() => new()
	{
		Metadata = new("preindustrial_leisure_hoop", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "hoop",
		ShortDescription = "a willow play hoop",
		LongDescription = null,
		FullDescription = "A light willow hoop is bound into a smooth circle for rolling races and outdoor play.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial toy.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureRagDoll() => new()
	{
		Metadata = new("preindustrial_leisure_rag_doll", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "doll",
		ShortDescription = "a cloth rag doll",
		LongDescription = null,
		FullDescription = "A simple stuffed cloth doll has stitched features and a tied yarn head for a child's toy or comfort object.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 3.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial toy.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisureCarvedWhistle() => new()
	{
		Metadata = new("preindustrial_leisure_carved_whistle", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "whistle",
		ShortDescription = "a carved wooden whistle",
		LongDescription = null,
		FullDescription = "A small carved wooden whistle gives a clear simple note when blown through its narrow mouthpiece.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80,
		Cost = 2.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial toy and signal prop; no command or signal mechanic is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialLeisurePullHorse() => new()
	{
		Metadata = new("preindustrial_leisure_pull_horse", "shared-preindustrial", ["medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "horse",
		ShortDescription = "a carved pull horse",
		LongDescription = null,
		FullDescription = "A small carved wooden horse has four simple wheels and a cord for a child to pull across a floor or yard.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 460,
		Cost = 6.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shared pre-industrial wheeled toy.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition AntiquityBronzeHandSaw() => new()
	{
		Metadata = new("antiquity_bronze_hand_saw", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "saw",
		ShortDescription = "a bronze hand saw",
		LongDescription = null,
		FullDescription = "A short bronze saw with a riveted wooden grip and a straight row of hand-filed teeth, suitable for cutting boards, dowels, and small furniture members.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520,
		Cost = 24.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Saws / Hand Saw"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBronzeAdze() => new()
	{
		Metadata = new("antiquity_bronze_adze", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "adze",
		ShortDescription = "a bronze woodworking adze",
		LongDescription = null,
		FullDescription = "A bronze adze blade is lashed firmly across a smooth wooden haft, giving a furniture maker enough bite to dress boards, hollow stools, and rough vessel staves.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1150,
		Cost = 30.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Adze"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBronzeWoodChisel() => new()
	{
		Metadata = new("antiquity_bronze_wood_chisel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "chisel",
		ShortDescription = "a bronze wood chisel",
		LongDescription = null,
		FullDescription = "A narrow bronze chisel with a faceted wooden handle, kept sharp for cutting mortises, trimming pegs, and carving small household fittings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280,
		Cost = 18.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood Chisel"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBronzeWorkshopChisel() => new()
	{
		Metadata = new("antiquity_bronze_workshop_chisel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "chisel",
		ShortDescription = "a bronze workshop chisel",
		LongDescription = null,
		FullDescription = "A general bronze chisel with a plain wooden handle, useful for rough shaping, small notches, and tool finishing where a specialist edge is not needed.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320,
		Cost = 16.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Cutting and Shaping Tools / Chisel"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBronzeWorkshopHammer() => new()
	{
		Metadata = new("antiquity_bronze_workshop_hammer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "hammer",
		ShortDescription = "a bronze workshop hammer",
		LongDescription = null,
		FullDescription = "A compact bronze hammer with a square face and a smooth wooden haft, heavy enough for peening, riveting, and general workshop striking.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760,
		Cost = 20.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Striking Tools / Hammer", "Functions / Tools / Metalworking Tools / Forge Hammer"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBronzeWorkshopPliers() => new()
	{
		Metadata = new("antiquity_bronze_workshop_pliers", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "pliers",
		ShortDescription = "a pair of bronze workshop pliers",
		LongDescription = null,
		FullDescription = "A pair of bronze pliers with short jaws and a wrapped grip, made for bending wire, holding hot fittings briefly, and closing small rings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360,
		Cost = 14.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Gripping Tools / Pliers"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBronzePlasterTrowel() => new()
	{
		Metadata = new("antiquity_bronze_plaster_trowel", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "trowel",
		ShortDescription = "a bronze finishing trowel",
		LongDescription = null,
		FullDescription = "A flat bronze trowel with a stubby wooden handle, used to smooth plaster, clay slips, pigments, and other spread workshop mixtures.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 310,
		Cost = 11.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Finishing Tools / Trowel"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquitySharpeningWhetstone() => new()
	{
		Metadata = new("antiquity_sharpening_whetstone", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "whetstone",
		ShortDescription = "a grooved sharpening whetstone",
		LongDescription = null,
		FullDescription = "A palm-sized stone worn into shallow grooves by knives, chisels, and needles. It is kept damp when used to freshen working edges.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Sharpening"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBronzeWoodAuger() => new()
	{
		Metadata = new("antiquity_bronze_wood_auger", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "auger",
		ShortDescription = "a bronze wood auger",
		LongDescription = null,
		FullDescription = "A bronze auger with a simple cross-grip, used to bore peg holes, hinge sockets, bung holes, and narrow openings in wooden vessels.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 470,
		Cost = 26.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood Auger"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityWoodenSmoothingPlane() => new()
	{
		Metadata = new("antiquity_wooden_smoothing_plane", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "plane",
		ShortDescription = "a wooden smoothing plane with a bronze iron",
		LongDescription = null,
		FullDescription = "A blocky wooden smoothing plane holds a small bronze iron in a wedged throat, letting a joiner level tabletops, chest boards, and visible furniture faces.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900,
		Cost = 28.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Planer"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityWoodenJoineryClamp() => new()
	{
		Metadata = new("antiquity_wooden_joinery_clamp", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "clamp",
		ShortDescription = "a wooden joinery clamp",
		LongDescription = null,
		FullDescription = "A wooden clamp with opposing jaws and a wedge key, useful for holding glued boards, pegged frames, or leather-covered furniture while the work sets.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600,
		Cost = 16.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood Clamp"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBowDrill() => new()
	{
		Metadata = new("antiquity_bow_drill", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "drill",
		ShortDescription = "a bow drill with a bronze bit",
		LongDescription = null,
		FullDescription = "A simple bow drill with a corded bow, wooden hand block, and small bronze bit, suited to slow, controlled holes in wood, bone, shell, and soft stone.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 430,
		Cost = 18.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Bow Drill"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityWoodRasp() => new()
	{
		Metadata = new("antiquity_wood_rasp", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "rasp",
		ShortDescription = "a bronze-toothed wood rasp",
		LongDescription = null,
		FullDescription = "A bronze rasp punched with coarse teeth and fitted with a wooden grip, used to soften corners and fair irregular curves before final smoothing.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380,
		Cost = 15.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Rasp"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityCoopersAdze() => new()
	{
		Metadata = new("antiquity_coopers_adze", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "adze",
		ShortDescription = "a cooper's bronze adze",
		LongDescription = null,
		FullDescription = "A short-handled cooper's adze with a curved bronze bit, shaped for hollowing and dressing the inner faces of staves.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250,
		Cost = 32.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Cooper's Adze"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityCoopersCroze() => new()
	{
		Metadata = new("antiquity_coopers_croze", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "croze",
		ShortDescription = "a cooper's croze",
		LongDescription = null,
		FullDescription = "A wooden-bodied croze carries a bronze cutter for cutting the grooves that receive cask heads and bucket bottoms.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520,
		Cost = 22.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Croze"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityHoopDriver() => new()
	{
		Metadata = new("antiquity_hoop_driver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "driver",
		ShortDescription = "a wooden hoop driver",
		LongDescription = null,
		FullDescription = "A blunt wooden hoop driver with a broad striking face, made for walking hoops down over staves without splitting them.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900,
		Cost = 8.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Hoop Driver"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBungBorer() => new()
	{
		Metadata = new("antiquity_bung_borer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "borer",
		ShortDescription = "a bronze bung borer",
		LongDescription = null,
		FullDescription = "A bronze-edged bung borer set into a wooden brace, used to open neat round holes in casks, tubs, and brewing vessels.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620,
		Cost = 24.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Bung Borer"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityBasketKnife() => new()
	{
		Metadata = new("antiquity_basket_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "knife",
		ShortDescription = "a bronze basket knife",
		LongDescription = null,
		FullDescription = "A small bronze knife with a hooked tip for trimming willow, reed, bark, and splints during basketry and matting work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 10.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Basketry Tools / Basket Knife"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityReedSplitter() => new()
	{
		Metadata = new("antiquity_reed_splitter", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "splitter",
		ShortDescription = "a bronze reed splitter",
		LongDescription = null,
		FullDescription = "A bronze reed splitter with three narrow fins that divide reeds and willow rods into even splints for baskets, trays, and seats.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85,
		Cost = 9.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Basketry Tools / Reed Splitter"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityWeavingBodkin() => new()
	{
		Metadata = new("antiquity_weaving_bodkin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "bodkin",
		ShortDescription = "a bronze weaving bodkin",
		LongDescription = null,
		FullDescription = "A smooth bronze bodkin used to open tight woven courses, tuck ends, and draw bindings through basketry or leather work.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45,
		Cost = 7.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Basketry Tools / Weaving Bodkin"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityPackingBone() => new()
	{
		Metadata = new("antiquity_packing_bone", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "bone",
		ShortDescription = "a polished packing bone",
		LongDescription = null,
		FullDescription = "A polished bone tool for pressing weavers close, creasing folds, and settling splints without cutting their fibres.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Basketry Tools / Packing Bone"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityLinenDyeStrainer() => new()
	{
		Metadata = new("antiquity_linen_dye_strainer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "strainer",
		ShortDescription = "a linen dye strainer",
		LongDescription = null,
		FullDescription = "A tight linen strainer stretched over a light willow hoop, used to separate dregs, lake pigment, and plant matter from prepared dye liquor.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Dyeing Tools / Dye Strainer"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityIndigoBeatingPaddle() => new()
	{
		Metadata = new("antiquity_indigo_beating_paddle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "paddle",
		ShortDescription = "an indigo beating paddle",
		LongDescription = null,
		FullDescription = "A flat wooden paddle with a dark-stained face, made for beating indigo vats and lifting oxygen through the liquor until the colour takes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 540,
		Cost = 7.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Dyeing Tools / Indigo Beating Paddle"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityRopeHookBar() => new()
	{
		Metadata = new("antiquity_rope_hook_bar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "hook",
		ShortDescription = "a bronze rope hook",
		LongDescription = null,
		FullDescription = "A stout bronze hook mounted on a short wooden bar, useful for twisting cordage, holding tension, and starting small rope lays.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 410,
		Cost = 10.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Ropemaking Tools / Rope Hook"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition AntiquityTwineShuttle() => new()
	{
		Metadata = new("antiquity_twine_shuttle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.AntiquityHouseholdTools.cs:297", []),
		Noun = "shuttle",
		ShortDescription = "a wooden twine shuttle",
		LongDescription = null,
		FullDescription = "A flat wooden twine shuttle notched at both ends, sized for carrying cord through tight turns and binding lamp wicks, nets, or small ropes.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70,
		Cost = 3.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = ["Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Ropemaking Tools / Twine Shuttle"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};
}
