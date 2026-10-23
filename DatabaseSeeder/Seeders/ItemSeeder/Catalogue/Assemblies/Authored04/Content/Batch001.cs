#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Authored04Stock
{
	public static CatalogueItemDefinition RenaissanceWritingSketchbook() => new()
	{
		Metadata = new("renaissance_writing_sketchbook", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "sketchbook",
		ShortDescription = "a stitched artist's sketchbook",
		LongDescription = null,
		FullDescription = "A palm-sized stitched book holds heavy paper leaves for studies, measurements, and workshop drawings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 8.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_Small_90_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance drawing and workshop notebook.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingCommonplaceBook() => new()
	{
		Metadata = new("renaissance_writing_commonplace_book", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "book",
		ShortDescription = "a leather-bound commonplace book",
		LongDescription = null,
		FullDescription = "A leather-bound volume has ordered leaves for copied quotations, household notes, accounts, and observations.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 650,
		Cost = 16.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_Small_200_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance personal manuscript form.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingPilotBook() => new()
	{
		Metadata = new("renaissance_writing_pilot_book", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "book",
		ShortDescription = "a bound pilot book",
		LongDescription = null,
		FullDescription = "A water-stained bound book is arranged for bearings, hazards, soundings, and coastal sailing notes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 760,
		Cost = 24.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_Small_200_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance navigation record; no navigation bonus is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingPortolanChart() => new()
	{
		Metadata = new("renaissance_writing_portolan_chart", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "chart",
		ShortDescription = "a rolled portolan chart",
		LongDescription = null,
		FullDescription = "A broad parchment chart is ruled with rhumb lines and coastal margins, ready for builder-authored ports and bearings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 95,
		Cost = 12.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Paper"],
		Components = ["Holdable", "Destroyable_Misc", "Paper_A3"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance chart format; no automatic route knowledge is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingPrintedPrayerBook() => new()
	{
		Metadata = new("renaissance_writing_printed_prayer_book", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "book",
		ShortDescription = "a small printed prayer book",
		LongDescription = null,
		FullDescription = "A compact bound prayer book has closely set leaves and a plain protective cover for regular devotional use.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520,
		Cost = 14.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_Small_90_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance printed-book format; content remains builder-authored.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingPrintedMusicBook() => new()
	{
		Metadata = new("renaissance_writing_printed_music_book", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "book",
		ShortDescription = "a stitched printed music book",
		LongDescription = null,
		FullDescription = "A stitched music book has wide pages suitable for printed staves, part names, and rehearsal annotations.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 460,
		Cost = 12.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_Small_90_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance printed-music format; no musical effect is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingWoodblock() => new()
	{
		Metadata = new("renaissance_writing_woodblock", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "woodblock",
		ShortDescription = "a carved printing woodblock",
		LongDescription = null,
		FullDescription = "A close-grained wooden block has a raised carved face prepared for ink and repeated impression work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 750,
		Cost = 11.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Writing Implements"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance print-workshop stock; it makes no fixed-image claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingCopperEngravingPlate() => new()
	{
		Metadata = new("renaissance_writing_copper_engraving_plate", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "plate",
		ShortDescription = "a polished copper engraving plate",
		LongDescription = null,
		FullDescription = "A thin polished copper plate has a clean face for incised lines, mirrored lettering, and careful hand printing.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 940,
		Cost = 28.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Writing Implements"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance engraving-workshop stock.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingEngravingBurin() => new()
	{
		Metadata = new("renaissance_writing_engraving_burin", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "burin",
		ShortDescription = "a graver's steel burin",
		LongDescription = null,
		FullDescription = "A short steel burin with a mushroom-shaped wooden handle is ground for controlled engraving cuts.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 180,
		Cost = 16.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Writing Implements"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance engraving hand tool.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingEtchingNeedle() => new()
	{
		Metadata = new("renaissance_writing_etching_needle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "needle",
		ShortDescription = "an etching needle",
		LongDescription = null,
		FullDescription = "A fine pointed needle is set in a narrow handle for scratching prepared plates and retouching drawn lines.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Writing Implements"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance plate-preparation tool.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingSealMatrix() => new()
	{
		Metadata = new("renaissance_writing_seal_matrix", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "matrix",
		ShortDescription = "a brass seal matrix",
		LongDescription = null,
		FullDescription = "A small brass matrix has a flat engraved face intended for a household, office, or guild seal design.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 160,
		Cost = 22.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Writing Implements"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance sealing stock; no seal-stamp mechanics are claimed.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingSealingWaxStick() => new()
	{
		Metadata = new("renaissance_writing_sealing_wax_stick", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "wax",
		ShortDescription = "a red sealing-wax stick",
		LongDescription = null,
		FullDescription = "A hard coloured stick of beeswax compound is sized to melt in small portions over a folded document or parcel.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85,
		Cost = 3.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Writing Implements"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance document-sealing consumable presentation.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingCourierSatchel() => new()
	{
		Metadata = new("renaissance_writing_courier_satchel", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "satchel",
		ShortDescription = "a buckled courier satchel",
		LongDescription = null,
		FullDescription = "A weathered leather satchel has a buckled flap and a narrow document compartment for letters and folded records.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 920,
		Cost = 20.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Document Containers"],
		Components = ["Holdable", "Destroyable_Misc", "Container_Document_Satchel"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance courier and document-carrying container.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceWritingArchiveChest() => new()
	{
		Metadata = new("renaissance_writing_archive_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "chest",
		ShortDescription = "an iron-bound archive chest",
		LongDescription = null,
		FullDescription = "A heavy oak chest is fitted with iron bands and divided space for bundled papers, books, and sealed office records.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 38000,
		Cost = 160.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Renaissance Era", "Market / Writing Materials / Document Containers"],
		Components = ["Destroyable_Furniture", "Container_Archive_Chest"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance installed archive storage; intentionally not holdable.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtPainterEasel() => new()
	{
		Metadata = new("renaissance_art_painter_easel", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "easel",
		ShortDescription = "a folding painter's easel",
		LongDescription = null,
		FullDescription = "A jointed oak easel has an adjustable ledge and rear leg for holding a panel or canvas in a workshop.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6400,
		Cost = 42.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Destroyable_Furniture"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance workshop easel; fixed work position is descriptive only.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtPreparedCanvas() => new()
	{
		Metadata = new("renaissance_art_prepared_canvas", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "canvas",
		ShortDescription = "a primed linen canvas",
		LongDescription = null,
		FullDescription = "A stretched linen canvas has a pale prepared face ready for paint, charcoal studies, or a builder-authored skin.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 820,
		Cost = 14.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance painting surface.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtWoodenPalette() => new()
	{
		Metadata = new("renaissance_art_wooden_palette", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "palette",
		ShortDescription = "an oval wooden paint palette",
		LongDescription = null,
		FullDescription = "An oval hardwood palette has a thumb hole and a smooth face for arranging small pools of pigment.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280,
		Cost = 7.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance painter's hand tool.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtHogBrush() => new()
	{
		Metadata = new("renaissance_art_hog_brush", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "brush",
		ShortDescription = "a hog-bristle paint brush",
		LongDescription = null,
		FullDescription = "A short wooden brush carries a tightly bound bundle of stiff bristles for working paint into a prepared surface.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90,
		Cost = 4.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance painter's brush.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtPigmentShell() => new()
	{
		Metadata = new("renaissance_art_pigment_shell", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "shell",
		ShortDescription = "a small pigment shell",
		LongDescription = null,
		FullDescription = "A shallow shell is clean enough to hold a little ground pigment, oil, or wash during careful painting work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45,
		Cost = 2.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance pigment-serving vessel.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtGraniteMuller() => new()
	{
		Metadata = new("renaissance_art_granite_muller", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "muller",
		ShortDescription = "a granite pigment muller",
		LongDescription = null,
		FullDescription = "A rounded granite hand stone has a flat working face for grinding pigments on a slab.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800,
		Cost = 10.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "granite",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance pigment-grinding hand tool.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtDrawingBoard() => new()
	{
		Metadata = new("renaissance_art_drawing_board", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "board",
		ShortDescription = "a smooth drawing board",
		LongDescription = null,
		FullDescription = "A thin smooth board gives an artist or surveyor a firm portable backing for paper and parchment sheets.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900,
		Cost = 8.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance drawing and field-note support.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtSilverpointStylus() => new()
	{
		Metadata = new("renaissance_art_silverpoint_stylus", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "stylus",
		ShortDescription = "a silverpoint drawing stylus",
		LongDescription = null,
		FullDescription = "A slim handled stylus carries a soft silver point for fine lines on prepared paper or ground panels.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 85,
		Cost = 18.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance drawing implement.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtProportionalDividers() => new()
	{
		Metadata = new("renaissance_art_proportional_dividers", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "dividers",
		ShortDescription = "a pair of proportional dividers",
		LongDescription = null,
		FullDescription = "Two brass arms pivot on a marked joint for transferring and comparing proportions in drawing, carving, and surveying.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 210,
		Cost = 24.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance proportional-measurement tool; no automatic calculation is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtCarvingChisel() => new()
	{
		Metadata = new("renaissance_art_carving_chisel", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "chisel",
		ShortDescription = "a narrow carving chisel",
		LongDescription = null,
		FullDescription = "A small steel-edged chisel set in an oak handle is ground for controlled cuts in wood, wax, and soft plaster.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240,
		Cost = 11.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance carving tool.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtWaxModel() => new()
	{
		Metadata = new("renaissance_art_wax_model", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "model",
		ShortDescription = "a small wax study model",
		LongDescription = null,
		FullDescription = "A palm-sized beeswax model preserves the rough planes and proportions of a proposed figure or ornament.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360,
		Cost = 9.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance sculptural study; no morph or mould mechanic is claimed.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtPlasterCast() => new()
	{
		Metadata = new("renaissance_art_plaster_cast", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "cast",
		ShortDescription = "a white plaster study cast",
		LongDescription = null,
		FullDescription = "A white plaster cast records a hand, face, or ornament in durable workshop form for drawing and carving practice.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3400,
		Cost = 18.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "plaster",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance workshop study object.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceArtPatternBook() => new()
	{
		Metadata = new("renaissance_art_pattern_book", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "book",
		ShortDescription = "a bound pattern book",
		LongDescription = null,
		FullDescription = "A bound paper book leaves room for copied decorative patterns, workshop diagrams, and measured ornament.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 540,
		Cost = 14.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Book_Small_90_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance design reference; content remains builder-authored.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceScienceBrassQuadrant() => new()
	{
		Metadata = new("renaissance_science_brass_quadrant", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "quadrant",
		ShortDescription = "a graduated brass quadrant",
		LongDescription = null,
		FullDescription = "A quarter-circle brass instrument bears simple degree marks and a plumb line for visual observation and demonstration.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720,
		Cost = 35.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance observational instrument; no automatic measurement is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceScienceSectorRule() => new()
	{
		Metadata = new("renaissance_science_sector_rule", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "sector",
		ShortDescription = "a folding brass sector",
		LongDescription = null,
		FullDescription = "A folding pair of engraved brass rules provides proportional scales for workshop calculation and plotting.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420,
		Cost = 28.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance calculation aid; no automatic calculation is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceScienceSandglass() => new()
	{
		Metadata = new("renaissance_science_sandglass", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "sandglass",
		ShortDescription = "a half-hour sandglass",
		LongDescription = null,
		FullDescription = "Fine sand runs between two clear glass bulbs held in a simple wooden frame for a visibly bounded interval.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 390,
		Cost = 12.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance timing prop; no timekeeping mechanics are claimed.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceScienceHorizontalSundial() => new()
	{
		Metadata = new("renaissance_science_horizontal_sundial", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "sundial",
		ShortDescription = "a brass horizontal sundial",
		LongDescription = null,
		FullDescription = "A flat brass plate and raised gnomon cast a shifting shadow across engraved hour lines in suitable sunlight.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 980,
		Cost = 30.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance timekeeping prop; no automatic time mechanic is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceScienceArmillarySphere() => new()
	{
		Metadata = new("renaissance_science_armillary_sphere", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "sphere",
		ShortDescription = "a small brass armillary sphere",
		LongDescription = null,
		FullDescription = "Nested brass rings surround a small central globe in a display of celestial circles and axes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600,
		Cost = 85.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance astronomical display; no observation bonus is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition RenaissanceNavigationSoundingLine() => new()
	{
		Metadata = new("renaissance_navigation_sounding_line", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "line",
		ShortDescription = "a coiled lead sounding line",
		LongDescription = null,
		FullDescription = "A tarred line is neatly coiled around a shaped lead weight for taking manual soundings from a boat or quay.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4100,
		Cost = 16.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "lead",
		Tags = ["Era / Renaissance Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Renaissance maritime hand tool; no automatic depth measurement is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};



































































}
